using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Sweeter : MonoBehaviour
{
    public GameObject Holder;

    public GameObject SweetsPrefab;
    public bool CanClick = true;

    public Transform[] SpawnPoints;
    int randomPoint;

    public Transform SweeterInitialPos;
    public Transform SweeterNewPos;

    private int NumberOfClicks = 0;
    public int MaxClicks;

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && CanClick && (NumberOfClicks <= MaxClicks))
        {
            Debug.Log("Click on sweeter !");

            NumberOfClicks++;

            ShakeSweetCan();

            randomPoint = Random.Range(0, SpawnPoints.Length);
            var sweet = Instantiate(SweetsPrefab, SpawnPoints[randomPoint].position, Quaternion.identity);
            sweet.transform.parent = Holder.transform;
        }
    }

    private void ShakeSweetCan()
    {
        CanClick = false;
        transform.DOMove(SweeterNewPos.position, 0.2f).OnComplete(ReturnSweetCanToInitialPosition);
    }

    private void ReturnSweetCanToInitialPosition()
    {
        CanClick = true;
        transform.DOMove(SweeterInitialPos.position, 0.2f);
    }

}
