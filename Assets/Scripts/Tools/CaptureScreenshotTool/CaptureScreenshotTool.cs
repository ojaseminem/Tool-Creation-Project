using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tools.CaptureScreenshotTool
{
    public static class CaptureScreenshotTool
    {
        public const string KEditorPref = "CaptureScreenshotPath";
        public const string KMenuPath = "My Menu/Capture Screenshot";
        
        [MenuItem(KMenuPath + " _F11")]
        public static void CaptureScreenshotToolMenuItem()
        {
            var path = EditorPrefs.GetString(KEditorPref);
            if (string.IsNullOrWhiteSpace(path))
                path = GetDefaultPath();
            
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            var filepath = Path.Combine(path,
                // ReSharper disable once UseFormatSpecifierInInterpolation
                // ReSharper disable once StringLiteralTypo
                $"{Application.productName}_{DateTime.Now.ToString("yyyymmddhhmmss")}.png");
            
            ScreenCapture.CaptureScreenshot(filepath, 1);
        }

        public static string GetDefaultPath()
        {
            var defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Screenshots");
            return defaultPath;
        }

    }

    public class CaptureScreenshotSettingsProvider : SettingsProvider
    {
        private CaptureScreenshotSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope)
        { }

        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);
            
            GUILayout.Space((20f));

            var path = EditorPrefs.GetString(CaptureScreenshotTool.KEditorPref);

            if (string.IsNullOrWhiteSpace(path))
                path = CaptureScreenshotTool.GetDefaultPath();

            var changedPath = EditorGUILayout.TextField(path);

            if (string.CompareOrdinal(path, changedPath) != 0)
            {
                EditorPrefs.SetString(CaptureScreenshotTool.KEditorPref, changedPath);
            }
            
            GUILayout.Space(10f);
            // ReSharper disable once InvertIf
            // ReSharper disable once HeapView.ObjectAllocation
            if (GUILayout.Button("Reset to Default", GUILayout.Width(150f)))
            {
                EditorPrefs.DeleteKey((CaptureScreenshotTool.KEditorPref));
                Repaint();
            }
        }

        [SettingsProvider]

        public static SettingsProvider CreateCaptureScreenshotSettingsProvider()
        {
            // ReSharper disable once SuggestVarOrType_SimpleTypes
            CaptureScreenshotSettingsProvider captureScreenshotSettingsProvider =
                new CaptureScreenshotSettingsProvider(CaptureScreenshotTool.KMenuPath);
            return captureScreenshotSettingsProvider;
        }
    }
}
