using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpinAnimation : MonoBehaviour
{
    public GameObject ItemToSpin;

    // Start is called before the first frame update
    void Start()
    {
        //ItemToSpin.transform.DORotate(new Vector3(0f, 0f, 180f), 1f).SetLoops(-1,LoopType.Incremental);
        ItemToSpin.transform.DORotate(new Vector3(0f, 0f, 180f), 5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }
}
