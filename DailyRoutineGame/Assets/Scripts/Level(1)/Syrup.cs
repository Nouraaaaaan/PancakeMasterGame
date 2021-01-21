﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using UnityEngine.EventSystems;

public class Syrup : MonoBehaviour
{
    public enum SyrupType
    {
        withChocolateSyrup,
        withStrawberrySyrup,
        withMapleSyrup,
        HotSauce
    }
    public SyrupType syrupType;

    public Color SyrupColor;

    [Header("Syrup Attributes")]
    public GameObject SyrupObject;
    public Transform SyrupInitialPos;
    public Transform SyrupFinalPos;

    [Header("Syrup Pouring Attributes")]
    public bool CanPourSyrup;

    [Header("Syrup Painting Attributes")]
    public GameObject SyrupMesh;
    private GameObject mesh;
    [SerializeField]
    public Brush Syrupbrush;

    [Header("Special Attributes")]
    public bool IsSpecialSyrup;
    public bool IsHotSauce;

    [Header("Dragging Attributes")]
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
        if (Input.GetMouseButtonDown(0) && (PancakeLevelManager.Instance.CanAddSyrup) && (!IsPointerOverUIObject()))
        {
            lastMousePos = Input.mousePosition;
            Emitter.transform.position = PouringPoint.transform.position;

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

    void OnMouseDrag()
    {
        if (IsPointerOverUIObject())
            return;

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

        //Haptic.
        HapticsManager.Instance.HapticPulse(HapticTypes.Selection);
    }

    public void MoveSyrup()
    {
        PancakeLevelManager.Instance.currentState = PancakeLevelManager.State.SyrupState;
        
        SyrupObject.transform.DOMove(SyrupFinalPos.position, 0.5f);
        SyrupObject.transform.DOLocalRotate(new Vector3(0f, 0f, -51.728f), 0.5f).OnComplete(EnableSyrupPouring);
    }

    private void EnableSyrupPouring()
    {
        //vfx.
        SFXManager.Instance.PlaySoundEffect(5);

        CanPourSyrup = true;
        PancakeLevelManager.Instance.StartObiFluid();
        StartCoroutine(ReturnSyrupCoroutine());
    }

    public void ReturnSyrupToInitialPosition()
    {
        
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

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
