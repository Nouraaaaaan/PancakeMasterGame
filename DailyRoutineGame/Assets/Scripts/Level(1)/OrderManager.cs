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
        withChocolateSyrup,
        withStrawberrySyrup,
        withMapleSyrup
    }
    public SyrupOrder CustomerSyrupOrder;
    

    [Header("Sweets Order Attributes")]
    public Text SweetsText;
    public enum SweetsOrder
    {
        withStrawberry,
        withChocolate,
        withBlueBerries,
        withSprinkles

    }
    public SweetsOrder CustomerSweetsOrder;
    

    private void Start()
    {
        //GenerateRandomOrder();
    }

    public void GenerateRandomOrder()
    {
        GenerateRandomSyrupOrder();
        GenerateRandomSweetsOrder();
    }

    private void GenerateRandomSyrupOrder()
    {
        CustomerSyrupOrder = (SyrupOrder)Random.Range(0, 2);

        SyrupText.text = CustomerSyrupOrder.ToString();
    }

    private void GenerateRandomSweetsOrder()
    {
        CustomerSweetsOrder = (SweetsOrder)Random.Range(0, 3);

        SweetsText.text = CustomerSweetsOrder.ToString();
    }
}
