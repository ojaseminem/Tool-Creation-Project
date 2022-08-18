using UnityEngine;

namespace Gun_Scene.Assets.Mini_First_Person_Controller.Scripts.Components
{
    [ExecuteInEditMode]
    public class Zoom : MonoBehaviour
    {
        private Camera m_Camera;
        public float defaultFOV = 60;
        public float maxZoomFOV = 15;
        [Range(0, 1)]
        public float currentZoom;
        public float sensitivity = 1;


        void Awake()
        {
            // Get the camera on this gameObject and the defaultZoom.
            m_Camera = GetComponent<Camera>();
            if (m_Camera)
            {
                defaultFOV = m_Camera.fieldOfView;
            }
        }

        void Update()
        {
            // Update the currentZoom and the camera's fieldOfView.
            currentZoom += Input.mouseScrollDelta.y * sensitivity * .05f;
            currentZoom = Mathf.Clamp01(currentZoom);
            m_Camera.fieldOfView = Mathf.Lerp(defaultFOV, maxZoomFOV, currentZoom);
        }
    }
}
