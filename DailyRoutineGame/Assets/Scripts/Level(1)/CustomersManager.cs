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
        //SwapCustomers();
    }

    private void MoveCustomerAway()
    {
        StartCoroutine(MoveCustomerAwayCoroutine());
    }

    private IEnumerator MoveCustomerAwayCoroutine()
    {
        Customers[0].transform.DOMove(ExitPos.position, 12f).OnComplete(SetLastCustomerPosition);
        Customers[0].transform.LookAt(ExitPos);
        Customers[0].GetComponent<Animator>().SetBool("walk", true);

        SwapCustomers();
        yield return new WaitForSeconds(2f);

        MoveCustomers();
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
        for (int i = 0; i < Customers.Length - 1; i++)
        {
            Customers[i].transform.DOMove(QueuePositions[i].transform.position, 1f);
            Customers[i].transform.LookAt(QueuePositions[i].transform);
            //CustomerAnimators[i].SetBool("walk", true);
            Customers[i].GetComponent<Animator>().SetBool("walk", true);
        }

        yield return new WaitForSeconds(1f);

        SetCustomersIdle();
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

    private void SetLastCustomerPosition()
    {
        Customers[QueuePositions.Length - 1].transform.position = QueuePositions[QueuePositions.Length - 1].position;
    }
}
