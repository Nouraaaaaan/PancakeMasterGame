using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Sweeter : MonoBehaviour
{
    [Header("Sweets Spawning Attributes")]
    public GameObject[] SweetsPrefabs;
    public Transform[] SpawnPoints;
    public GameObject Holder;
    int randomPrefab;
    int NumberOfSpawnedSweets = 0;
    int MaxSpawnedSweets = 40;

    [Header("Sweeter Animation Attributes")]
    public Transform SweeterInitialPos;
    public Transform SweeterNewPos;


    private bool CanInstantiate;
    private float counter = 0.1f;

    private bool Finished = false;


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
        if (Input.GetMouseButtonDown(0) && (PancakeLevelManager.Instance.CanAddSweets))
        {
            PancakeLevelManager.Instance.MoveSweeter();
            PancakeLevelManager.Instance.SweetsOrder = "withSweets";
            PancakeLevelManager.Instance.CanAddSweets = false;
        }
    }

    private void OnMouseDrag()
    {
        if (Finished == false)
        {
            CanInstantiate = true;
        }
    }

    private void OnMouseUp()
    {
        CanInstantiate = false;
    }

    private void InstantiateSweet()
    {
        //Debug.Log("Instantiate Sweets !");   

        foreach (var spawnPoint in SpawnPoints)
        {
            randomPrefab = Random.Range(0, SweetsPrefabs.Length);
            var sweet = Instantiate(SweetsPrefabs[randomPrefab], spawnPoint.position, Quaternion.identity);
            sweet.transform.parent = Holder.transform;
        }

        NumberOfSpawnedSweets++;
        if (NumberOfSpawnedSweets >= MaxSpawnedSweets)
        {
            Finished = true;
            CanInstantiate = false;
            PancakeLevelManager.Instance.FinishSweetingStage();
        }
    }

}
