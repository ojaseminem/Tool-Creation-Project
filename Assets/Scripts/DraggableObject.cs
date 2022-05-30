using UnityEngine;

public class DraggableObject : MonoBehaviour
{

    public float moveValue;
    private Vector3 _mOffset;
    private float _mZCoord;

    private void OnMouseDown()
    {
        _mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        _mOffset = gameObject.transform.position - GetMouseWorldPos();
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = _mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        transform.position = Vector3.Slerp( transform.position, GetMouseWorldPos() + _mOffset, moveValue * Time.deltaTime);
        
    }
}
