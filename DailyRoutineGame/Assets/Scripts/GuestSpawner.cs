using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestSpawner : MonoBehaviour
{
    public GameObject[] GuestSpawners;
    
    public float SpawnRate;
    float counter;
    public float Speed = 0.01f;

    [System.Serializable]
    public struct Path
    {
        public Transform[] Points;
    }

    public List<Path> PathList;

    private int guestIndex;
    private GameObject guest;

    private int GuestSpawnerNumber;

    public List<GameObject> MovingGuests;
    public float minimumDistanceToFollow = 0.1f;
    private Vector3 pos;


    private void Start()
    {
        counter = 0;
        GuestSpawnerNumber = 2;

        //SpawnGuest();
    }

    private void Update()
    {
        if (counter > 0)
        {
            counter -= Time.deltaTime;
            return;
        }
        else
        {
            counter = SpawnRate;
            SpawnGuest();
        }
        
    }

    void SpawnGuest()
    {
        //Debug.Log("SpawnGuest");

        for (int i = 0; i < GuestSpawnerNumber; i++)
        {    
            guestIndex = Random.Range(0, 3);
            //Debug.Log("guestIndex : " + guestIndex);
            guest = ObjectPooler.Instance.SpwanFromPool("car" + guestIndex, GuestSpawners[i].transform.position, GuestSpawners[i].transform.rotation);
             

             if (guest != null)
             {
                MovingGuests.Add(guest);
                int index = MovingGuests.IndexOf(guest);
                StartCoroutine(delayMoveTowardsPath(PathList[i].Points, MovingGuests[index]));
            }
        }

    }

    IEnumerator delayMoveTowardsPath(Transform[] list, GameObject car)
    {
        yield return new WaitForSeconds(0);

        float distance = 0;
        
        for (int i = 0; i < list.Length; i++)
        {
            distance = Vector3.Distance(car.transform.position, list[i].position);

            while (distance > minimumDistanceToFollow)
            {
                //Debug.Log("distance > minimumDistanceToFollow");
                pos = Vector3.MoveTowards(car.transform.position, list[i].position, Speed * Time.deltaTime);
                //car.transform.LookAt(pos);
                car.transform.position = pos;

                distance = Vector3.Distance(car.transform.position, list[i].position);
                yield return null;

            }
        }

        car.SetActive(false);
        MovingGuests.Remove(car);
    }


}
