using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using DanielLochner.Assets.SimpleSideMenu;
using DG.Tweening;
using MoreMountains.NiceVibrations;

public class ShopManager : MonoBehaviour
{
    public SaveTest SaveTest;
    public Button TapOneButton;

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
        public bool CurrentlyUsed;
        public GameObject DefaultObject;
    }

    [Header("Tables")]
    public GameObject MessyDinerTables;
    public ShopItem[] TableItems;
    public Button[] TableButtons;

    [Header("Decorations")]
    public ShopItem[] DecorationsItems;
    public Button[] DecorationsButtons;

    [Header("Floor")]
    public ShopItem[] FloorsItems;
    public Button[] FloorsButtons;

    [Header("UI Manager")]
    public Button TablesTabButton;

    [Header("Upgrade Attributes")]
    public ParticleSystem ConfettiVFX;
    public GameObject MessyDiner;
    public GameObject SimpleDiner;


    #endregion

    #region CallBack Region
    private void Awake()
    {
        string dir = Path.Combine(Application.persistentDataPath, "SaveData");
        //Debug.Log(dir);

        if (!Directory.Exists(dir))
        {
            SaveTest.Save();
        }
    }

    private void Start()
    {
        /*
        TapOneButton.gameObject.SetActive(true);
        TapOneButton.Select();
        */

        SaveTest.Load();
        LoadSavedCurrency();
        LoadItems();

        ShowAllSoldItems();
        UpdateButtons();

        CheckForDefaultTablesObjects();
        ChechForDinerStoreUpgrade();
    }

    #endregion

    #region TablesRegion
    public void OnClick_TableItem(Button button)
    {
        if (button.interactable)
        {
            PlayButtonHapticEffect();
            HapticsManager.Instance.HapticPulse(HapticTypes.SoftImpact);
            Buy_TableItem(GetByName_TableItem(button.name));
        }
    }

    private void Buy_TableItem(int index)
    {
        //Debug.Log("Buy table item of index :"+index);

        if (!TableItems[index].sold)
        {
            SFXManager.Instance.PlaySoundEffect(1);

            TableItems[index].sold = true;
            SaveTest.SaveObject.Tables[index] = true;
            SaveTest.Save();

            TableButtons[index].transform.GetChild(1).transform.gameObject.SetActive(false);

            Show_TabletemModel(index);

            UpdateCurrencyValue(TableItems[index].price);
            UpdateCurrencyUI();
            UpdateButtons();

            MessyDinerTables.SetActive(false); //will be removed later.
            if (TableItems[index].DefaultObject != null)
            {
                TableItems[index].DefaultObject.SetActive(false);
            }
        }
        else if (TableItems[index].sold && !(TableItems[index].CurrentlyUsed))
        {
            Show_TabletemModel(index);
        }
    }

    private int GetByName_TableItem(string name)
    {
        int index = 0;

        for (int i = 0; i < TableItems.Length; i++)
        {
            if (TableItems[i].name.Equals(name))
            {
                index = i;
                break;
            }
        }

        return index;
    }

    private void Show_TabletemModel(int index)
    {
        //Disable all items.
        foreach (var item in TableItems)
        {
            item.model.SetActive(false);
        }

        //All other items are not currently used.
        for (int i = 0; i < TableItems.Length; i++)
        {
            TableItems[i].CurrentlyUsed = false;
            SaveTest.SaveObject.CurrentlyUsedTables[i] = false;
        }
        TableItems[index].CurrentlyUsed = true;
        SaveTest.SaveObject.CurrentlyUsedTables[index] = true;

        //Saving.
        SaveTest.SaveObject.Tables[index] = true;
        SaveTest.Save();


        //Show model.
        if (TableItems[index].model != null)
        {
            TableItems[index].model.SetActive(true);
        }
    }

    private void CheckForDefaultTablesObjects()
    {
        foreach (var item in TableItems)
        {
            if(item.sold)
            {
                if (item.DefaultObject != null)
                {
                    item.DefaultObject.SetActive(false);
                    MessyDinerTables.SetActive(false); //will be removed later.
                }
            }
        }

        foreach (var item in DecorationsItems)
        {
            if (item.sold)
            {
                if (item.DefaultObject != null)
                {
                    item.DefaultObject.SetActive(false);
                }
            }
        }

        foreach (var item in FloorsItems)
        {
            if (item.sold)
            {
                if (item.DefaultObject != null)
                {
                    item.DefaultObject.SetActive(false);
                }
            }
        }
    }

    #endregion

    #region DecorationsRegion
    public void OnClick_DecorationItem(Button button)
    {
        
        if (button.interactable)
        {
            PlayButtonHapticEffect();
            Buy_DecorationItem(GetByName_DecorationItem(button.name));
        }
    }

    private void Buy_DecorationItem(int index)
    {
        //Debug.Log("Buy_DecorationItem : " + index);

        if (!DecorationsItems[index].sold)
        {
            SFXManager.Instance.PlaySoundEffect(1);
            DecorationsItems[index].sold = true;
            SaveTest.SaveObject.Decorations[index] = true;
            SaveTest.Save();

            DecorationsButtons[index].transform.GetChild(1).transform.gameObject.SetActive(false);

            Show_DecorationtemModel(index);
            UpdateCurrencyValue(DecorationsItems[index].price);
            UpdateCurrencyUI();
            UpdateButtons();

            if (DecorationsItems[index].DefaultObject != null)
            {
                DecorationsItems[index].DefaultObject.SetActive(false);
            }
        }

    }

    private int GetByName_DecorationItem(string name)
    {
        int index = 0;

        for (int i = 0; i < DecorationsItems.Length; i++)
        {
            if (DecorationsItems[i].name.Equals(name))
            {
                index = i;
                break;
            }
        }

        return index;
    }

    private void Show_DecorationtemModel(int index)
    {
        if (DecorationsItems[index].model != null)
        {
            DecorationsItems[index].model.SetActive(true);
        }
    }

    #endregion

    #region FloorsRegion
    public void OnClick_FloorItem(Button button)
    {
        if (button.interactable)
        {
            PlayButtonHapticEffect();
            Buy_FloorItem(GetByName_FloorItem(button.name));
        }
    }

    private void Buy_FloorItem(int index)
    {
        //Debug.Log("Buy_DecorationItem : " + index);

        if (!FloorsItems[index].sold)
        {
            SFXManager.Instance.PlaySoundEffect(1);

            FloorsItems[index].sold = true;
            SaveTest.SaveObject.Floors[index] = true;
            SaveTest.Save();

            FloorsButtons[index].transform.GetChild(1).transform.gameObject.SetActive(false);

            Show_FloorItemModel(index);
            UpdateCurrencyValue(FloorsItems[index].price);
            UpdateCurrencyUI();
            UpdateButtons();

            if (FloorsItems[index].DefaultObject != null)
            {
                FloorsItems[index].DefaultObject.SetActive(false);
            }
        }

    }

    private int GetByName_FloorItem(string name)
    {
        int index = 0;

        for (int i = 0; i < FloorsItems.Length; i++)
        {
            if (FloorsItems[i].name.Equals(name))
            {
                index = i;
                break;
            }
        }

        return index;
    }

    private void Show_FloorItemModel(int index)
    {
        if (FloorsItems[index].model != null)
        {
            FloorsItems[index].model.SetActive(true);
        }
    }

    #endregion

    public SimpleSideMenu SimpleSideMenu;

    private void LoadItems()
    {
        //Tables & Chairs.
        for (int i = 0; i < SaveTest.SaveObject.Tables.Length; i++)
        {
            if (SaveTest.SaveObject.Tables[i] == true)
            {
                TableItems[i].sold = true;
                TableButtons[i].transform.GetChild(1).transform.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < SaveTest.SaveObject.Tables.Length; i++)
        {
            if (SaveTest.SaveObject.CurrentlyUsedTables[i] == true)
            {
                TableItems[i].CurrentlyUsed = true;
            }
        }

        //Decorations.
        for (int i = 0; i < SaveTest.SaveObject.Decorations.Length; i++)
        {
            if (SaveTest.SaveObject.Decorations[i] == true)
            {
                DecorationsItems[i].sold = true;
                DecorationsButtons[i].transform.GetChild(1).transform.gameObject.SetActive(false);
            }
        }

        //Floor.
        for (int i = 0; i < SaveTest.SaveObject.Floors.Length; i++)
        {
            if (SaveTest.SaveObject.Floors[i] == true)
            {
                FloorsItems[i].sold = true;
                FloorsButtons[i].transform.GetChild(1).transform.gameObject.SetActive(false);
            }
        }
    }

    private void ShowAllSoldItems()
    {
        foreach (var item in TableItems)
        {
            if ((item.sold) && (item.CurrentlyUsed))
            {
                item.model.SetActive(true);
            }
        }

        foreach (var item in DecorationsItems)
        {
            if (item.sold)
            {
                item.model.SetActive(true);
            }
        }

        foreach (var item in FloorsItems)
        {
            if (item.sold)
            {
                item.model.SetActive(true);
            }
        }
    }

    private void UpdateButtons()
    {
        for (int i = 0; i < TableItems.Length; i++)
        {
            if (TableItems[i].price > CurrentCurrency)
            {
                TableButtons[i].interactable = false;
            }
        }

        for (int i = 0; i < DecorationsItems.Length; i++)
        {
            if (DecorationsItems[i].price > CurrentCurrency)
            {
                if(DecorationsButtons[i] != null)
                   DecorationsButtons[i].interactable = false;
            }
        }

        for (int i = 0; i < FloorsItems.Length; i++)
        {
            if (FloorsItems[i].price > CurrentCurrency)
            {
                FloorsButtons[i].interactable = false;
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

    
    private void UpdateCurrencyValue(int value)
    {
        //Debug.Log("Update Currency Value");

        CurrentCurrency -= value;

        SaveTest.SaveObject.PlayerCurrency = CurrentCurrency;

        SaveTest.Save();
    }

    

    private void UpdateCurrencyUI()
    {
        CurrentCurrencyText.text = CurrentCurrency.ToString();
    }

    #endregion

    #region UIManager
    public void Onclick_BackButton()
    {
        PlayButtonHapticEffect();
        SceneManager.LoadScene("MainScene");
    }

    public RectTransform RectTransform;
    public void OpenStoreUI()
    {
        RectTransform.DOAnchorPos3DY(1160, 0.5f);
    }

    public void CloseStoreUI()
    {
        RectTransform.DOAnchorPos3DY(722f, 0.5f);
    }

    #endregion

    private void ChechForDinerStoreUpgrade()
    {
        //Debug.Log(SaveTest.SaveObject.NumberOfInitialCustomers);

        if (SaveTest.SaveObject.NumberOfInitialCustomers == 3)
        {
            StartCoroutine(UpgradeDiner());
        }
        else if(SaveTest.SaveObject.NumberOfInitialCustomers > 3)
        {
            MessyDiner.SetActive(false);
            SimpleDiner.SetActive(true);
        }
    }

    IEnumerator UpgradeDiner()
    {
        yield return new WaitForSeconds(2f);

        ConfettiVFX.Play();
        SFXManager.Instance.PlaySoundEffect(0);
        MessyDiner.SetActive(false);
        SimpleDiner.SetActive(true);
    }

    public void PlayButtonHapticEffect()
    {
        HapticsManager.Instance.HapticPulse(HapticTypes.SoftImpact);
    }
}
