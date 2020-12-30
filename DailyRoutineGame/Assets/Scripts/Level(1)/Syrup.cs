using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;
using DG.Tweening;

public class Syrup : MonoBehaviour
{
    [Header("Syrup Attributes")]
    public GameObject SyrupObject;
    public Transform SyrupInitialPos;
    public Transform SyrupFinalPos;

    [Header("Syrup Pouring Attributes")]
    public bool CanPourSyrup;
    public ParticleSystem SyrupVFX; 
    public GameObject PouringPoint;

    [Header("Syrup Painting Attributes")]
    public GameObject SyrupPaintingQuad;
    [SerializeField]
    public Brush Syrupbrush;


    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && (PancakeLevelManager.Instance.CanAddSyrup))
        {
            MoveSyrup();
            PancakeLevelManager.Instance.CurrentSyrup = this;
            PancakeLevelManager.Instance.SyrupOrder = "withSyrup";
            PancakeLevelManager.Instance.CanAddSyrup = false;
        }
    }

    public void MoveSyrup()
    {
        PancakeLevelManager.Instance.currentState = PancakeLevelManager.State.SyrupState;

        SyrupObject.transform.DOMove(SyrupFinalPos.position, 0.5f);
        SyrupObject.transform.DOLocalRotate(new Vector3(0f, 0f, -51.728f), 0.5f).OnComplete(EnableSyrupPouring);
        SyrupPaintingQuad.SetActive(true);
    }

    private void EnableSyrupPouring()
    {
        CanPourSyrup = true;
    }

    public void PouringSyrup()
    {
        if (CanPourSyrup)
        {
            SyrupVFX.Play();
        }     
    }

    public void StopPouringSyrup()
    {
        SyrupVFX.Stop();
    }

    public void ReturnSyrupToInitialPosition()
    {
        SyrupObject.transform.DOMove(SyrupInitialPos.position, 1f);
        SyrupObject.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 1f);
    }

    public void FreezeSyrupRigidBody()
    {
        SyrupObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

}
