using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableSyrup : MonoBehaviour
{
    public float DragSpeed;

    [Space(20)]

    public float MaxPosX;
    public float MinPosX;

    [Space(20)]

    public float MaxPosZ;
    public float MinPosZ;

    Vector3 lastMousePos;

    [Space(20)]
    public GameObject PouringPoint;
    public GameObject Emitter;

    private void OnMouseDown()
    {
        lastMousePos = Input.mousePosition;
        Emitter.transform.position = PouringPoint.transform.position;
    }


    void OnMouseDrag()
    {
        Emitter.transform.position = PouringPoint.transform.position;

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
