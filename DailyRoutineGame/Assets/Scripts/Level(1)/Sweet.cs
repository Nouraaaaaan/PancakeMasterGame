using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sweet : MonoBehaviour
{
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.tag.Equals("Pancake"))
        {
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }  
    }
    
}
