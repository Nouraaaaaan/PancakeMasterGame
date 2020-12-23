using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcingCakeMaster : MonoBehaviour
{
    public enum Instrument
    {
        Icing,
        Smoothing
    }
    public Instrument instrument;

    [SerializeField] Cake cake;

    [Header("Icing")]
    [SerializeField] float spawnRadius = 0.05f;
    [SerializeField] GameObject icingPrefab;
    [SerializeField] LayerMask icingLayer;

    [Header("Smootihing")]
    [SerializeField] float smootingRadius = 1f;

    bool isPlacing = false;
    Vector3 hitPoint;
    Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (instrument == Instrument.Icing)
            PlaceIcing();
        else if (instrument == Instrument.Smoothing)
            Smoothing();
    }

    void PlaceIcing()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isPlacing = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPlacing = false;
        }

        if (isPlacing)
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 50f))
            {
                hitPoint = hit.point;

                if (!Physics.CheckSphere(hitPoint, spawnRadius, icingLayer))
                {
                    Instantiate(icingPrefab, hitPoint, Quaternion.identity, cake.transform.parent);
                }
            }
        }
    }

    void Smoothing()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isPlacing = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPlacing = false;
        }

        if (isPlacing)
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 50f))
            {
                hitPoint = hit.point;

                cake.ReturToNormal(hitPoint, smootingRadius);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (isPlacing && mainCamera != null)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(mainCamera.transform.position, hitPoint);
            Gizmos.color = Color.red;

            if (instrument == Instrument.Icing)
                Gizmos.DrawSphere(hitPoint, spawnRadius);
            else
                Gizmos.DrawSphere(hitPoint, smootingRadius);
        }
    }

    public void ChangeToIcong()
    {
        instrument = Instrument.Icing;
    }

    public void ChangeToSmoothing()
    {
        instrument = Instrument.Smoothing;
    }
}
