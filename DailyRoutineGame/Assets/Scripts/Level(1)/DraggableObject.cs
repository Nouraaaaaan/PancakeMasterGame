using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    public float DragSpeed;

    [Space(20)]

    public float MaxPosX;
    public float MinPosX;

    [Space(20)]

    public float MaxPosZ;
    public float MinPosZ;

    Vector3 lastMousePos;

    private void OnMouseDown()
    {
        lastMousePos = Input.mousePosition;
    }


    void OnMouseDrag()
    {
        Vector3 delta = Input.mousePosition - lastMousePos;
        Vector3 pos = transform.position;

        pos.x += delta.x * DragSpeed * -1f;
        pos.z += delta.y * DragSpeed * -1f;

        if ((MinPosX <= pos.x) && (MaxPosX >= pos.x) && (MinPosZ <= pos.z) && (MaxPosZ >= pos.z))
        {
            transform.position = pos;
        }
        
        lastMousePos = Input.mousePosition;
    }

}
