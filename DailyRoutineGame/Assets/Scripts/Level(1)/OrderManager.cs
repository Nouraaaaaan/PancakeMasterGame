using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    [Header("Syrup Order Attributes")]
    public Image SyrupImage;
    public enum SyrupOrder
    {
        withChocolateSyrup,
        withStrawberrySyrup,
        withMapleSyrup
    }
    public SyrupOrder CustomerSyrupOrder;
    public Sprite[] SyrupsImages;


    [Header("Sweets Order Attributes")]
    public Image SweetImage;
    public enum SweetsOrder
    {
        withStrawberry,
        withChocolate,
        withBlueBerries,
        withSprinkles

    }
    public SweetsOrder CustomerSweetsOrder;
    public Sprite[] SweetsImages;
    

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
        int randomSyrupIndex = Random.Range(0, 3);

        CustomerSyrupOrder = (SyrupOrder)randomSyrupIndex;

        SetSyrupImage(randomSyrupIndex);
    }

    private void SetSyrupImage(int index)
    {
        SyrupImage.sprite = SyrupsImages[index];
    }

    private void GenerateRandomSweetsOrder()
    {
        int sweetSyrupIndex = Random.Range(0, 4);

        CustomerSweetsOrder = (SweetsOrder)sweetSyrupIndex;

        SetSweetImage(sweetSyrupIndex);
    }

    private void SetSweetImage(int index)
    {
        SweetImage.sprite = SweetsImages[index];
    }
}
