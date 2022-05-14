using System;
using UnityEngine;
namespace Tools.PixelatedGameTool
{
    [ExecuteInEditMode]
    public class PixelatedGameToolShaderHandler : MonoBehaviour
    {
        public Material effectMaterial;
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest, effectMaterial);
        }
    }
}
