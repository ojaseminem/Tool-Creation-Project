using UnityEngine;

namespace Controllers.CameraPanAndZoom
{
    public class CameraPanAndZoomController : MonoBehaviour
    {
        public float zoomOutMin = 1f;
        public float zoomOutMax = 8f;
        
        private Vector3 m_TouchStart;
        private Camera m_Camera;

        private void Awake() => m_Camera = Camera.main;
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_TouchStart = m_Camera.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;
                Zoom(difference * 0.01f);
            }
            
            else if (Input.GetMouseButton(0))
            {
                Vector3 dir = m_TouchStart - m_Camera.ScreenToWorldPoint(Input.mousePosition);
                m_Camera.transform.position += dir;
            }
            Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }

        private void Zoom(float increment)
        {
            m_Camera.orthographicSize = Mathf.Clamp(m_Camera.orthographicSize - increment, zoomOutMin, zoomOutMax);
        }
    }
}