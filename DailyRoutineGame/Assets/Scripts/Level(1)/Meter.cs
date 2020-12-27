using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Meter : MonoBehaviour
{
    public GameObject Arrow;
    public float Speed;
    public Vector3[] Path;

    public void MoveArrow()
    {
        Arrow.transform.DOPath(Path, Speed, PathType.Linear,PathMode.Full3D).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopArrow()
    {
        Arrow.transform.DOKill();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Arrow"))
        {
            PancakeLevelManager.Instance.IsArrowInsideGreenArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Arrow"))
        {
            PancakeLevelManager.Instance.IsArrowInsideGreenArea = false;
        }
    }
}
