using UnityEditor;
using UnityEngine;

namespace TurtleGameWorks.Editor.CreateFoldersTool
{
    public static class CreateFoldersTool
    {
        [MenuItem("Assets/Create/TurtleGameWorks/CreateProjectFolders", false, 0)]
        private static void CreateProjectFolders()
        {
            CreateFolder("Audio");
            CreateFolder("Art");
            CreateFolder("Art/Animations");
            CreateFolder("Art/Animations/Skeletons");
            CreateFolder("Art/Materials");
            CreateFolder("Art/Materials/Textures");
            CreateFolder("Art/Models");
            CreateFolder("Prefabs");
            CreateFolder("Resources");
            CreateFolder("Scenes");
            CreateFolder("Scripts");
            CreateFolder("VFX");

            Debug.Log("Project folders created successfully!");
        }

        private static void CreateFolder(string folderPath)
        {
            // Split the folder path into individual folders
            string[] folders = folderPath.Split('/');
            string currentPath = "Assets";

            // Create each folder if it does not exist
            foreach (string folder in folders)
            {
                string newFolderPath = currentPath + "/" + folder;
                if (!AssetDatabase.IsValidFolder(newFolderPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folder);
                }
                currentPath = newFolderPath;
            }
        }
    }
}