using UnityEngine;

public class SweetItem : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("target"))
        {
            transform.parent = collision.transform;
            GetComponent<Rigidbody>().isKinematic = true;
            //Vibration.Vibrate(35);
        }
    }

   /* private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("target"))
        {
            transform.parent = other.transform;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }*/
}
