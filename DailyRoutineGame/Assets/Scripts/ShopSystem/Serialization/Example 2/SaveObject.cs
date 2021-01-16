using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveObject
{
    public int PlayerCurrency;

    public bool[] Tables;
    public bool[] CurrentlyUsedTables;

    public bool[] Decorations;

    public bool[] Floors;
}
