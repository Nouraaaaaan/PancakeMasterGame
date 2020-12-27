using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillerCollisionPoint : MonoBehaviour
{
    public bool IsCollidedWithFiller;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Filler") && (!this.IsCollidedWithFiller))
        {
            this.IsCollidedWithFiller = true;
            PancakeLevelManager.Instance.UpdateNumberOfFilledPoints();
        }
        
    }
}
