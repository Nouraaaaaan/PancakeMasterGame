using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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


    public bool CanInstantiate;
    private float counter = 0.1f;

    public bool Finished = false;
    public bool arrived = false;

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
        //Debug.Log(NumberOfSpawnedSweets);

        if (NumberOfSpawnedSweets >= MaxSpawnedSweets)
        {
            Finished = true;
            CanInstantiate = false;
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
        //transform.DOLocalRotate(new Vector3(0f, 0f, 134.041f), 1f);
        //Debug.Log(SweeterInitialPos.transform.rotation.eulerAngles.z);
        transform.DOLocalRotate(new Vector3(0f, 0f, SweeterInitialPos.transform.rotation.eulerAngles.z), 1f);
        transform.DOMove(SweeterInitialPos.position, 1f).OnComplete(DisableSweetingStage);
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

}
