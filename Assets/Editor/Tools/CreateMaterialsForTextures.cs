using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor.Tools
{
    public class CreateMaterialsForTextures : ScriptableWizard
    {
        public Shader shader;

        [MenuItem("Tools/CreateMaterialsForTextures")]
        static void CreateWizard()
        {
            DisplayWizard<CreateMaterialsForTextures>("Create Materials", "Create");
        }

        private void OnEnable()
        {
            shader = Shader.Find("Diffuse");
        }

        private void OnWizardCreate()
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                var textures = Selection.GetFiltered(typeof(Texture), SelectionMode.Assets).Cast<Texture>();
                foreach (var tex in textures)
                {
                    var path = AssetDatabase.GetAssetPath(tex);
                    path = path.Substring(0, path.LastIndexOf(".", StringComparison.Ordinal)) + ".mat";
                    if (AssetDatabase.LoadAssetAtPath(path, typeof(Material)) != null)
                    {
                        var existingMat = AssetDatabase.LoadAssetAtPath<Material>(path);
                        existingMat.shader = shader;
                        AssetDatabase.RenameAsset(path, "Mat_" + existingMat.name);
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        var mat = new Material(shader) { mainTexture = tex };
                        AssetDatabase.CreateAsset(mat, path);
                        var newMat = AssetDatabase.LoadAssetAtPath<Material>(path);
                        AssetDatabase.RenameAsset(path, "Mat_" + newMat.name);
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
