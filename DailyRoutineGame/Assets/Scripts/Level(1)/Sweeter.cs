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
    public Transform SweeterFinalPos;


    private bool CanInstantiate;
    private float counter = 0.1f;

    private bool Finished = false;
    private bool arrived = false;

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
            MoveSweeter();
            PancakeLevelManager.Instance.SweetsOrder = "withSweets";
            PancakeLevelManager.Instance.CanAddSweets = false;
        }
    }

    private void OnMouseDrag()
    {
        if ((!Finished) && (arrived))
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
        Debug.Log(NumberOfSpawnedSweets);

        if (NumberOfSpawnedSweets >= MaxSpawnedSweets)
        {
            Finished = true;
            CanInstantiate = false;
            PancakeLevelManager.Instance.FinishSweetingStage();
        }
    }

    public void MoveSweeter()
    {
        transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 1f);
        transform.DOMove(SweeterFinalPos.position, 1f).OnComplete(SweeterAnimationFinished);
    }

    public void ReturnSweeter()
    {
        transform.DOLocalRotate(new Vector3(0f, 0f, 134.041f), 1f);
        transform.DOMove(SweeterInitialPos.position, 1f);
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

}
