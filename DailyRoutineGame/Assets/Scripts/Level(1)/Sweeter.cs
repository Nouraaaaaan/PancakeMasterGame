﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using UnityEngine.EventSystems;

public class Sweeter : MonoBehaviour
{
    public string OrderType;

    [Header("Sweets Spawning Attributes")]
    public GameObject[] SweetsPrefabs;
    public Transform[] SpawnPoints;
    public GameObject Holder;
    int randomPrefab;
    public int NumberOfSpawnedSweets = 0;
    int MaxSpawnedSweets = 40;

    [Header("Sweeter Animation Attributes")]
    public Transform SweeterInitialPos;
    public Transform SweeterFinalPos;
    public Vector3 FinalRotationValue;
    public float TransitionSpeed;

    [Header("Special Sweeter Attributes")]
    public bool IsSpecialSweeter;

    [Header("SFX")]
    private bool StartPlayingSound;


    public bool CanInstantiate;
    private float counter = 0.1f;

    public bool Finished = false;
    public bool arrived = false;

    [Header("Dragging Attributes")]
    public float DragSpeed;
    Vector3 lastMousePos;
    [Space(20)]
    public float MaxPosX;
    public float MinPosX;
    [Space(20)]
    public float MaxPosZ;
    public float MinPosZ;

    private void Update()
    {
        counter -= Time.deltaTime;

        if (counter <= 0)
        {
            counter = 0.1f;

            if (CanInstantiate)
            {
                InstantiateSweet();
            }
        }
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && (PancakeLevelManager.Instance.CanAddSweets) && (!IsPointerOverUIObject()))
        {
            lastMousePos = Input.mousePosition;

            PancakeLevelManager.Instance.CurrentSweeter = this;
            PancakeLevelManager.Instance.SweetsOrder = OrderType;
            PancakeLevelManager.Instance.CanAddSweets = false;

            if (IsSpecialSweeter)
            {
                AdsManager.ins.ShowRewardedVideo(AdsManager.RewardType.SpecialTopping);
            }
            else
            {
                MoveSweeter();
            }
        }
    }

    private void OnMouseDrag()
    {
        if ((!Finished) && (arrived) && (!IsPointerOverUIObject()))
        {
            CanInstantiate = true;
            UpdatePosition();
        }   
    }

    private void OnMouseUp()
    {
        CanInstantiate = false;
    }

    private void InstantiateSweet()
    {
        //vfx.
        if(!SFXManager.Instance.AudioSource.isPlaying)
        {
            SFXManager.Instance.PlaySoundEffect(0);

            //Haptic.
            HapticsManager.Instance.HapticPulse(HapticTypes.SoftImpact);
        }
           
        foreach (var spawnPoint in SpawnPoints)
        {
            randomPrefab = Random.Range(0, SweetsPrefabs.Length);
            var sweet = Instantiate(SweetsPrefabs[randomPrefab], spawnPoint.position, Quaternion.identity);
            sweet.transform.parent = Holder.transform;
        }

        NumberOfSpawnedSweets++;

        if ((NumberOfSpawnedSweets >= MaxSpawnedSweets) && (CanInstantiate))
        {
            CanInstantiate = false;
            Finished = true;
            PancakeLevelManager.Instance.FinishSweetingStage();
        }
    }

    public void MoveSweeter()
    {
        transform.DOLocalRotate(FinalRotationValue, 1f);
        transform.DOMove(SweeterFinalPos.position, TransitionSpeed).OnComplete(SweeterAnimationFinished);
    }

    public void ReturnSweeter()
    {
        transform.DOLocalRotate(new Vector3(0f, 0f, SweeterInitialPos.transform.rotation.eulerAngles.z), 1f);
        transform.DOMove(SweeterInitialPos.position, 1f).OnComplete(DisableSweetingStage);

        //vfx.
        SFXManager.Instance.StopLoopingOption();
        SFXManager.Instance.StopSoundEffect();
    }

    private void SweeterAnimationFinished()
    {
        arrived = true;
    }

    public void ClearChildren()
    {
        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[Holder.transform.childCount];

        int i = 0;

        //Find all child obj and store to that array
        foreach (Transform child in Holder.transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child.gameObject);
        }

    }

    private void DisableSweetingStage()
    {
        PancakeLevelManager.Instance.DiableSweetingStage();
    }

    private void UpdatePosition()
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

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

}
