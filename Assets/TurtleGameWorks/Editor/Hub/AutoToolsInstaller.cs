using UnityEditor;
using UnityEngine;

namespace TurtleGameWorks.AutoTools
{
    /// <summary>
    /// Automatically opens the AutoTools Hub the first time this package is installed
    /// in a project. Uses EditorPrefs keyed by project path so it only triggers once per project.
    /// </summary>
    [InitializeOnLoad]
    public static class AutoToolsInstaller
    {
        private static readonly string InstalledKey =
            $"AutoTools_Installed_{Application.dataPath.GetHashCode()}";

        static AutoToolsInstaller()
        {
            if (!EditorPrefs.GetBool(InstalledKey, false))
            {
                // Defer to next editor frame so everything is fully loaded
                EditorApplication.delayCall += OnFirstInstall;
            }
        }

        private static void OnFirstInstall()
        {
            EditorPrefs.SetBool(InstalledKey, true);

            bool open = EditorUtility.DisplayDialog(
                "AutoTools Hub — Welcome! 🎉",
                "AutoTools Hub by TurtleGameWorks has been detected in this project.\n\n" +
                "Would you like to open the Hub now?\n\n" +
                "You can always access it via:\nTools > TurtleGameWorks > AutoTools Hub",
                "Open Hub",
                "Later"
            );

            if (open)
                AutoToolsHub.ShowWindow();
        }

        /// <summary>
        /// Reset the install flag for this project (useful for testing the first-run experience).
        /// </summary>
        [MenuItem("Tools/TurtleGameWorks/Reset Install Flag (Debug)")]
        public static void ResetInstallFlag()
        {
            EditorPrefs.DeleteKey(InstalledKey);
            Debug.Log("[AutoTools] Install flag reset. Hub will show welcome dialog on next domain reload.");
        }
    }
}
