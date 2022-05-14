using UnityEngine;

namespace Controllers.ShowAllTargetsCamera
{
    public class TrackingTargetsController : MonoBehaviour {

        [SerializeField] 
        Transform[] targets;

        [SerializeField] 
        float boundingBoxPadding = 2f;

        [SerializeField]
        float minimumOrthographicSize = 8f;

        [SerializeField]
        float zoomSpeed = 20f;

        Camera m_Camera;

        void Awake () 
        {
            m_Camera = GetComponent<Camera>();
            m_Camera.orthographic = true;
        }

        void LateUpdate()
        {
            Rect boundingBox = CalculateTargetsBoundingBox();
            transform.position = CalculateCameraPosition(boundingBox);
            m_Camera.orthographicSize = CalculateOrthographicSize(boundingBox);
        }

        Rect CalculateTargetsBoundingBox()
        {
            float minX = Mathf.Infinity;
            float maxX = Mathf.NegativeInfinity;
            float minY = Mathf.Infinity;
            float maxY = Mathf.NegativeInfinity;

            foreach (Transform target in targets) {
                Vector3 position = target.position;

                minX = Mathf.Min(minX, position.x);
                minY = Mathf.Min(minY, position.y);
                maxX = Mathf.Max(maxX, position.x);
                maxY = Mathf.Max(maxY, position.y);
            }

            return Rect.MinMaxRect(minX - boundingBoxPadding, maxY + boundingBoxPadding, maxX + boundingBoxPadding, minY - boundingBoxPadding);
        }

        Vector3 CalculateCameraPosition(Rect boundingBox)
        {
            Vector2 boundingBoxCenter = boundingBox.center;

            return new Vector3(boundingBoxCenter.x, boundingBoxCenter.y, -10f);
        }

        float CalculateOrthographicSize(Rect boundingBox)
        {
            float orthographicSize = m_Camera.orthographicSize;
            Vector3 topRight = new Vector3(boundingBox.x + boundingBox.width, boundingBox.y, 0f);
            Vector3 topRightAsViewport = m_Camera.WorldToViewportPoint(topRight);
       
            if (topRightAsViewport.x >= topRightAsViewport.y)
                orthographicSize = Mathf.Abs(boundingBox.width) / m_Camera.aspect / 2f;
            else
                orthographicSize = Mathf.Abs(boundingBox.height) / 2f;

            return Mathf.Clamp(Mathf.Lerp(m_Camera.orthographicSize, orthographicSize, Time.deltaTime * zoomSpeed), minimumOrthographicSize, Mathf.Infinity);
        }
    }
}
