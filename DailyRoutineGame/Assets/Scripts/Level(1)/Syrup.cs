using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;
using DG.Tweening;

public class Syrup : MonoBehaviour
{
    public enum SyrupType
    {
        withChocolateSyrup,
        withStrawberrySyrup,
        withMapleSyrup
    }
    public SyrupType syrupType;

    public Color SyrupColor;

    [Header("Syrup Attributes")]
    public GameObject SyrupObject;
    public Transform SyrupInitialPos;
    public Transform SyrupFinalPos;

    [Header("Syrup Pouring Attributes")]
    public bool CanPourSyrup;
    public ParticleSystem SyrupVFX; 
    public GameObject PouringPoint;

    [Header("Syrup Painting Attributes")]
    public GameObject SyrupMesh;
    private GameObject mesh;
    [SerializeField]
    public Brush Syrupbrush;

    [Header("Special Attributes")]
    public bool IsSpecialSyrup;


    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && (PancakeLevelManager.Instance.CanAddSyrup))
        {      
            PancakeLevelManager.Instance.CurrentSyrup = this;
            PancakeLevelManager.Instance.SyrupOrder = syrupType.ToString();
            PancakeLevelManager.Instance.CanAddSyrup = false;
            PancakeLevelManager.Instance.SetSyrupColor(SyrupColor);

            if (IsSpecialSyrup)
            {
                AdsManager.ins.ShowRewardedVideo(AdsManager.RewardType.SpecialSyrup);
            }
            else
            {
                MoveSyrup();
            }
        }
    }

    public void MoveSyrup()
    {
        PancakeLevelManager.Instance.currentState = PancakeLevelManager.State.SyrupState;
        
        SyrupObject.transform.DOMove(SyrupFinalPos.position, 0.5f);
        SyrupObject.transform.DOLocalRotate(new Vector3(0f, 0f, -51.728f), 0.5f).OnComplete(EnableSyrupPouring);
    }

    private void EnableSyrupPouring()
    {
        CanPourSyrup = true;

        PancakeLevelManager.Instance.StartObiFluid();
        
        StartCoroutine(ReturnSyrupCoroutine());
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
        StopPouringSyrup();
        PancakeLevelManager.Instance.FinishObi();

        SyrupObject.transform.DOMove(SyrupInitialPos.position, 1f);
        SyrupObject.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 1f).OnComplete(FinishSyrupState);
    }

    private IEnumerator ReturnSyrupCoroutine()
    {
        yield return new WaitForSeconds(4f);
        ReturnSyrupToInitialPosition();
    }

    private void FinishSyrupState()
    {
        PancakeLevelManager.Instance.FinishSyrupState();
    }

}
