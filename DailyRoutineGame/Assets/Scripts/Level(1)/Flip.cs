using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Flip : MonoBehaviour
{
    public GameObject Pancake;
    public Rigidbody PancakeRB;


    private void Start()
    {
        StartCoroutine(FlipPancake());
    }

    IEnumerator FlipPancake()
    {
        PancakeRB.AddForce(Vector3.up * 6, ForceMode.VelocityChange);
        yield return new WaitForSeconds(0.5f);
        Pancake.transform.DORotate(new Vector3(0f, 0f, 180f), 0.5f);
    }
}
