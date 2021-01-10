using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyrupPoints : MonoBehaviour
{
    public bool IsCollidedWithFiller;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Syrup") && (!this.IsCollidedWithFiller))
        {
            this.IsCollidedWithFiller = true;
            //PancakeLevelManager.Instance.UpdateNumberOfSyrupPoints();
        }

    }
}
