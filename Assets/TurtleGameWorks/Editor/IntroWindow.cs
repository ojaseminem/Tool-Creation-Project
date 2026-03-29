using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

public class IntroWindow : EditorWindow
{
    [MenuItem("Window/Asset Store Intro")]
    public static void ShowWindow()
    {
        IntroWindow window = GetWindow<IntroWindow>("Asset Store Intro");
        window.minSize = new Vector2(400, 200);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Welcome to Your Asset Store Tool Package!");

        if (GUILayout.Button("Documentation"))
        {
            // Open the documentation URL in the browser
            Application.OpenURL("https://your-documentation-url.com");
        }

        if (GUILayout.Button("Setup"))
        {
            // Perform setup tasks here
            InstallDependencies();
        }
    }

    private void InstallDependencies()
    {
        AddRequest request = Client.Add("com.unity.nuget.newtonsoft-json"); // Install the Newtonsoft.Json package

        Debug.Log(request);
        EditorApplication.update += ProgressCheck; // Start checking the installation progress

        void ProgressCheck()
        {
            if (request.IsCompleted) // Installation completed
            {
                if (request.Status == StatusCode.Success)
                {
                    Debug.Log("Newtonsoft.Json package installed successfully.");
                }
                else
                {
                    Debug.Log("Failed to install Newtonsoft.Json package. Error: " + request.Error.message);
                }

                EditorApplication.update -= ProgressCheck; // Stop checking the installation progress
            }
        }
    }
}