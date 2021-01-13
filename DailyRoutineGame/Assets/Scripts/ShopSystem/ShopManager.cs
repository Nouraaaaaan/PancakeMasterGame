using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ShopManager : MonoBehaviour
{
    public SaveTest SaveTest;

    #region Initialization Region
    public int CurrentCurrency;
    public Text CurrentCurrencyText;

    [System.Serializable]
    public struct ShopItem
    {
        public string name;
        public int price;
        public GameObject model;
        public bool sold;
    }

    public ShopItem[] Items;

    public Button[] buttons;

    #endregion

    #region CallBack Region
    private void Awake()
    {
        string dir = Path.Combine(Application.persistentDataPath, "SaveData");

        Debug.Log(dir);

        if (!Directory.Exists(dir))
        {
            SaveTest.Save();
        }            
    }

    private void Start()
    {
        //SaveTest.SaveObject.PlayerCurrency = 1000;
        //SaveTest.Save();
      
        SaveTest.Load();
        LoadSavedCurrency();
        LoadBoughtItem();
        ShowAllSoldItems();
        UpdateButtons();
    }

    #endregion

    public void OnClicked(Button button)
    {
        if(button.interactable)
        {
            BuyItem(GetItemByName(button.name));
        }
    }

    private void BuyItem(int index)
    {
        if (!Items[index].sold)
        {
            Items[index].sold = true;
            SaveTest.SaveObject.BoughtItems[index] = true;
            SaveTest.Save();

            buttons[index].transform.GetChild(0).transform.gameObject.SetActive(false);

            ShowItemModel(index);
            UpdateCurrencyValue(index);
            UpdateCurrencyUI();
            UpdateButtons();
        }
        
    }

    private void LoadBoughtItem()
    {
        for (int i = 0; i < SaveTest.SaveObject.BoughtItems.Length; i++)
        {
            if (SaveTest.SaveObject.BoughtItems[i] == true)
            {
                Items[i].sold = true;
            }
        }
    }

    private int GetItemByName(string name)
    {
        int index = 0;

        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].name.Equals(name))
            {
                index = i;
                break;
            }
        }

        return index;
    }

    private void ShowItemModel(int index)
    {
        if (Items[index].model != null)
        {
            Items[index].model.SetActive(true);
        }
    }

    private void ShowAllSoldItems()
    {
        foreach (var item in Items)
        {
            if (item.sold)
            {
                item.model.SetActive(true);
            }
        }
    }

    private void UpdateButtons()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].price > CurrentCurrency)
            {
                //Debug.Log("Diable button "+i);
                buttons[i].interactable = false;
            }
        }
    }

    #region Currency Region
    private void LoadSavedCurrency()
    {
        int playerCurrency = SaveTest.SaveObject.PlayerCurrency;
        CurrentCurrency = playerCurrency;
        UpdateCurrencyUI();
    }

    private void UpdateCurrencyValue(int index)
    {
        Debug.Log("Update Currency Value");
        CurrentCurrency -= Items[index].price;
        SaveTest.SaveObject.PlayerCurrency = CurrentCurrency;
        SaveTest.Save();
    }

    private void UpdateCurrencyUI()
    {
        CurrentCurrencyText.text = CurrentCurrency.ToString();
    }

    #endregion

}
