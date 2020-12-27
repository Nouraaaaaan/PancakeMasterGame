using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pancake : MonoBehaviour
{
    public Rigidbody PancakeRB;

    public void Flip()
    {
        StartCoroutine(FlipPancake());
    }

    IEnumerator FlipPancake()
    {
        PancakeRB.AddForce(Vector3.up * 6, ForceMode.VelocityChange);

        yield return new WaitForSeconds(0.5f);

        gameObject.transform.DORotate(new Vector3(0f, 0f, 180f), 0.5f);
    }
}
