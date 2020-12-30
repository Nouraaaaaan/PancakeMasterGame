using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    [Header("Syrup Order Attributes")]
    public Text SyrupText;
    public enum SyrupOrder
    {
        withSyrup,
        noSyrup
    }
    public SyrupOrder CustomerSyrupOrder;
    

    [Header("Sweets Order Attributes")]
    public Text SweetsText;
    public enum SweetsOrder
    {
        withSweets,
        noSweets
    }
    public SweetsOrder CustomerSweetsOrder;
    

    private void Start()
    {
        GenerateRandomOrder();
    }

    public void GenerateRandomOrder()
    {
        GenerateRandomSyrupOrder();
        GenerateRandomSweetsOrder();
    }

    private void GenerateRandomSyrupOrder()
    {
        CustomerSyrupOrder = (SyrupOrder)Random.Range(0, 1);

        SyrupText.text = CustomerSyrupOrder.ToString();
    }

    private void GenerateRandomSweetsOrder()
    {
        CustomerSweetsOrder = (SweetsOrder)Random.Range(0, 1);

        SweetsText.text = CustomerSweetsOrder.ToString();
    }
}
