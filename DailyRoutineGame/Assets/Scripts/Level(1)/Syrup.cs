using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Syrup : MonoBehaviour
{
    [Header("Syrup Attributes")]
    public GameObject SyrupObject;
    public ParticleSystem SyrupVFX;
    public Transform SyrupInitialPos;
    public Transform SyrupFinalPos;
    public GameObject PouringPoint;
    public GameObject SyrupPaintingQuad;
    

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && (PancakeLevelManager.Instance.CanAddSyrup))
        {
            MoveSyrup();
            PancakeLevelManager.Instance.SyrupOrder = "withSyrup";
            PancakeLevelManager.Instance.CanAddSyrup = false;
        }
    }

    public void MoveSyrup()
    {
        PancakeLevelManager.Instance.currentState = PancakeLevelManager.State.SyrupState;

        SyrupObject.transform.DOMove(SyrupFinalPos.position, 1f);
        SyrupObject.transform.DOLocalRotate(new Vector3(0f, 0f, -51.728f), 1f);
        SyrupPaintingQuad.SetActive(true);
    }

    public void PouringSyrup()
    {
        SyrupVFX.Play();
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
