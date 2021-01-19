using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    [Header("Syrup Order Attributes")]
    public Image SyrupOrderImage;
    public Image SyrupNoteImage;
    public enum SyrupOrder
    {
        withChocolateSyrup,
        withStrawberrySyrup,
        withMapleSyrup
    }
    public SyrupOrder CustomerSyrupOrder;
    public Sprite[] SyrupsImages;
    public Sprite[] SyrupsNoteImages;


    [Header("Topping Order Attributes")]
    public Image ToppingOrderImage;
    public Image ToppingNoteImage;
    public enum SweetsOrder
    {
        withStrawberry,
        withChocolate,
        withBlueBerries,
        withSprinkles

    }
    public SweetsOrder CustomerSweetsOrder;
    public Sprite[] SweetsImages;
    public Sprite[] ToppingNoteImages;


    public void GenerateRandomOrder()
    {
        GenerateRandomSyrupOrder();
        GenerateRandomToppingOrder();
    }

    #region Syrup Methods
    private void GenerateRandomSyrupOrder()
    {
        int randomSyrupIndex = Random.Range(0, 3);

        CustomerSyrupOrder = (SyrupOrder)randomSyrupIndex;

        SetSyrupOrderImage(randomSyrupIndex);
    }

    private void SetSyrupOrderImage(int index)
    {
        SyrupOrderImage.sprite = SyrupsImages[index];
        SyrupNoteImage.sprite = SyrupsNoteImages[index];
    }

    
    #endregion

    #region Toppings Methods
    private void GenerateRandomToppingOrder()
    {
        int sweetSyrupIndex = Random.Range(0, 4);

        CustomerSweetsOrder = (SweetsOrder)sweetSyrupIndex;

        SetToppingOrderImage(sweetSyrupIndex);
    }

    private void SetToppingOrderImage(int index)
    {
        ToppingOrderImage.sprite = SweetsImages[index];
        ToppingNoteImage.sprite = ToppingNoteImages[index];
    }

    #endregion
}
