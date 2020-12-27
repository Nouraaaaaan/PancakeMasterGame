using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : Singleton<Shop>
{
    [SerializeField] GameObject itemHolderPrefab;
    [SerializeField] CakeData[] cakeDatas;
    [SerializeField] Transform shopContent;

    public List<CakeData> UnlockedCakeData
    {
        get
        {
            List<CakeData> ItemHolderList = new List<CakeData>();
            CakeData[] cakeData = Resources.LoadAll<CakeData>("Cakes");

            for (int i = 0; i < cakeData.Length; i++)
            {
                if (cakeData[i].IsUnlocked)
                    ItemHolderList.Add(cakeData[i]);
            }

            return ItemHolderList;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < cakeDatas.Length; i++)
        {
            Instantiate(itemHolderPrefab, shopContent).GetComponent<ItemHolder>().SetItem(cakeDatas[i]);
        }
    }
}
