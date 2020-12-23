using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sweeter : MonoBehaviour
{
    public GameObject SweetsPrefab;
    public bool CanClick = true;

    public Transform[] SpawnPoints;
    int randomPoint;

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && CanClick)
        {
            Debug.Log("Click on sweeter !");
            
            randomPoint = Random.Range(0, SpawnPoints.Length);

            Instantiate(SweetsPrefab, SpawnPoints[randomPoint].position, Quaternion.identity);
        }
    }

}
