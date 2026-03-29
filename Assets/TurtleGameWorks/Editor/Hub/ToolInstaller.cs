using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TurtleGameWorks.AutoTools
{
    /// <summary>
    /// Handles install / remove of individual tools by doing a sparse git clone
    /// of just the tool's source paths at the specific tag, then copying the files
    /// into this project's Assets folder.
    ///
    /// Tracks installation state per project in EditorPrefs.
    /// </summary>
    public static class ToolInstaller
    {
        // ── Constants ────────────────────────────────────────────────────────
        public const string RepoUrl      = "https://github.com/ojaseminem/Tool-Creation-Project.git";
        public const string InstallRoot  = "Assets/TurtleGameWorks/InstalledTools";

        private const string PrefPrefix  = "AutoTools_Installed_";

        // ── State Queries ────────────────────────────────────────────────────

        public static bool IsInstalled(ToolEntry tool)
            => EditorPrefs.GetBool(PrefKey(tool), false);

        private static string PrefKey(ToolEntry tool)
            => $"{PrefPrefix}{Application.dataPath.GetHashCode()}_{tool.GitTag}";

        // ── Install ──────────────────────────────────────────────────────────

        public static void Install(ToolEntry tool, Action<string> onProgress, Action<bool, string> onDone)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), $"autotools_{tool.GitTag.Replace("/", "_")}_{Guid.NewGuid():N}");

            try
            {
                Directory.CreateDirectory(tempDir);

                // ── 1. Init a bare repo and set up sparse-checkout ──────────
                onProgress?.Invoke($"Initialising git in temp folder…");
                RunGit(tempDir, "init");
                RunGit(tempDir, $"remote add origin \"{RepoUrl}\"");
                RunGit(tempDir, "config core.sparseCheckout true");

                // Write sparse-checkout patterns (one per source path)
                var sparseFile = Path.Combine(tempDir, ".git", "info", "sparse-checkout");
                var patterns   = tool.SourcePaths.Select(p => p.TrimEnd('/') + "/").ToArray();
                // If a source path is a single file (no trailing dir), use it directly
                var finalPatterns = tool.SourcePaths
                    .Select(p => p.EndsWith(".cs") || p.EndsWith(".meta") ? p : p.TrimEnd('/') + "/**")
                    .ToArray();
                File.WriteAllLines(sparseFile, finalPatterns);

                // ── 2. Fetch only the specific tag (shallow) ─────────────────
                onProgress?.Invoke($"Fetching tag '{tool.GitTag}' from GitHub…");
                RunGit(tempDir, $"fetch --depth=1 origin refs/tags/{tool.GitTag}:refs/tags/{tool.GitTag}");

                // ── 3. Checkout the tag ─────────────────────────────────────
                onProgress?.Invoke($"Checking out files…");
                RunGit(tempDir, $"checkout tags/{tool.GitTag}");

                // ── 4. Copy files into the project ──────────────────────────
                onProgress?.Invoke($"Copying files into project…");
                var installPath = Path.Combine(
                    Application.dataPath.Replace("Assets", ""),
                    InstallRoot,
                    tool.InstallSubFolder
                ).Replace('\\', '/');

                if (Directory.Exists(installPath))
                    Directory.Delete(installPath, true);
                Directory.CreateDirectory(installPath);

                foreach (var sourcePath in tool.SourcePaths)
                {
                    var fromPath = Path.Combine(tempDir, sourcePath.Replace('/', Path.DirectorySeparatorChar));
                    if (Directory.Exists(fromPath))
                    {
                        CopyDirectory(fromPath, installPath);
                    }
                    else if (File.Exists(fromPath))
                    {
                        File.Copy(fromPath, Path.Combine(installPath, Path.GetFileName(fromPath)), true);
                    }
                    else
                    {
                        Debug.LogWarning($"[AutoTools] Source path not found in checkout: {sourcePath}");
                    }
                }

                // ── 5. Mark as installed ────────────────────────────────────
                EditorPrefs.SetBool(PrefKey(tool), true);

                AssetDatabase.Refresh();
                onDone?.Invoke(true, $"'{tool.Name}' installed successfully.");
            }
            catch (Exception e)
            {
                onDone?.Invoke(false, $"Install failed: {e.Message}");
                Debug.LogError($"[AutoTools] Install error for '{tool.Name}': {e}");
            }
            finally
            {
                // Clean up temp dir
                try { if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true); }
                catch { /* best effort */ }
            }
        }

        // ── Remove ───────────────────────────────────────────────────────────

        public static void Remove(ToolEntry tool, Action<bool, string> onDone)
        {
            try
            {
                var projectRoot = Application.dataPath.Replace("Assets", "").TrimEnd('/', '\\');
                var installPath = Path.Combine(projectRoot,
                    InstallRoot.Replace('/', Path.DirectorySeparatorChar),
                    tool.InstallSubFolder);

                if (Directory.Exists(installPath))
                {
                    // Also delete Unity .meta file for the folder
                    var metaPath = installPath.TrimEnd(Path.DirectorySeparatorChar) + ".meta";
                    Directory.Delete(installPath, true);
                    if (File.Exists(metaPath)) File.Delete(metaPath);
                }

                EditorPrefs.SetBool(PrefKey(tool), false);
                AssetDatabase.Refresh();
                onDone?.Invoke(true, $"'{tool.Name}' removed.");
            }
            catch (Exception e)
            {
                onDone?.Invoke(false, $"Remove failed: {e.Message}");
            }
        }

        // ── Git Helper ────────────────────────────────────────────────────────

        private static void RunGit(string workingDir, string args)
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName               = "git",
                Arguments              = args,
                WorkingDirectory       = workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                UseShellExecute        = false,
                CreateNoWindow         = true
            };

            using var proc = System.Diagnostics.Process.Start(psi);
            proc!.WaitForExit(60_000); // 60s timeout per command

            if (proc.ExitCode != 0)
            {
                var err = proc.StandardError.ReadToEnd();
                throw new Exception($"git {args}\n{err}");
            }
        }

        // ── File Copy ─────────────────────────────────────────────────────────

        private static void CopyDirectory(string source, string dest)
        {
            Directory.CreateDirectory(dest);
            foreach (var file in Directory.GetFiles(source))
                File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), true);
            foreach (var dir in Directory.GetDirectories(source))
                CopyDirectory(dir, Path.Combine(dest, Path.GetFileName(dir)));
        }
    }
}
