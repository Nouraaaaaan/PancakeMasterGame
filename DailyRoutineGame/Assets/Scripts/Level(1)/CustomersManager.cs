using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CustomersManager : MonoBehaviour
{
    public GameObject[] Customers;
    public Animator[] CustomerAnimators;
    private GameObject currentCustomer = null;

    public Transform[] QueuePositions;
    public Transform ExitPos;

    public void NextCustomer()
    {
        MoveCustomerAway();
        SwapCustomers();
    }

    private void MoveCustomerAway()
    {
        Customers[0].transform.DOMove(ExitPos.position, 3f).OnComplete(MoveCustomers);
        Customers[0].transform.LookAt(ExitPos);

        //CustomerAnimators[0].SetBool("walk", true);
        Customers[0].GetComponent<Animator>().SetBool("walk", true);
    }

    private void SwapCustomers()
    {
        currentCustomer = Customers[0];

        for (int i = 0; i < Customers.Length - 1; i++)
        {
            Customers[i] = Customers[i + 1];
        }

        Customers[Customers.Length - 1] = currentCustomer;

    }

    private void MoveCustomers()
    {
        StartCoroutine(MoveCustomersCorotinue());
    }
    
    private IEnumerator MoveCustomersCorotinue()
    {
        for (int i = 0; i < Customers.Length; i++)
        {
            Customers[i].transform.DOMove(QueuePositions[i].transform.position, 1f);
            Customers[i].transform.LookAt(QueuePositions[i].transform);
            CustomerAnimators[i].SetBool("walk", true);
        }

        yield return new WaitForSeconds(1f);

        SetCustomersIdle();

        //CheckVipCustomer();
    }

    private void SetCustomersIdle()
    {
        foreach (var customer in CustomerAnimators)
        {
            customer.SetBool("walk", false);
        }       
    }

    public bool CheckVipCustomer()
    {
        if (Customers[0].gameObject.tag.Equals("VipCustomer"))
        {
            //Debug.Log(" Here's a VIP customer ! ");
            return true;
        }
        else
        {
            //Debug.Log(Customers[0].gameObject.tag);
            return false;
        }
    }

}
