using System.Collections.Generic;
using UnityEngine;

namespace Tools.ShapeEditorTool
{
    public class ShapeCreator : MonoBehaviour
    {
        [HideInInspector]
        public List<Vector3> points = new List<Vector3>();

        public float handleRadius = 1f;

    }
}
