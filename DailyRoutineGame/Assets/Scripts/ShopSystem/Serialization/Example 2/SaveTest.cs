using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTest : MonoBehaviour
{
    public SaveObject SaveObject;

    public void Save()
    {
        //Debug.Log("saving...");
        SaveManager.save(SaveObject);
    }

    public void Load()
    {
        SaveObject = SaveManager.Load();
    }
}
