using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Sweeter : MonoBehaviour
{
    [Header("Sweets Spawning Attributes")]
    public GameObject SweetsPrefab;
    public Transform[] SpawnPoints;
    public GameObject Holder;
    int randomPoint;

    [Header("Sweeter Animation Attributes")]
    public Transform SweeterInitialPos;
    public Transform SweeterNewPos;

    [Header("Sweeter Clicking Attributes")]
    public bool CanClick = true;
    public  int MaxClicks;
    private int NumberOfClicks = 0;
    

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && CanClick && (NumberOfClicks <= MaxClicks))
        {
            //Debug.Log("Click on sweeter !");
            NumberOfClicks++;
            ShakeSweetCan();
            InstantiateSweet();
        }
        else if ((NumberOfClicks > MaxClicks) && CanClick)
        {
            CanClick = false;
            PancakeLevelManager.Instance.FinishSweetingStage();
        }
    }

    private void InstantiateSweet()
    {
        randomPoint = Random.Range(0, SpawnPoints.Length);
        var sweet = Instantiate(SweetsPrefab, SpawnPoints[randomPoint].position, Quaternion.identity);
        sweet.transform.parent = Holder.transform;
    }

    private void ShakeSweetCan()
    {
        CanClick = false;
        transform.DOMove(SweeterNewPos.position, 0.2f).OnComplete(ReturnSweetCanToInitialPosition);
        SFXManager.Instance.PlaySoundEffect(0);
    }

    private void ReturnSweetCanToInitialPosition()
    {
        CanClick = true;
        transform.DOMove(SweeterInitialPos.position, 0.2f);
    }

}
