using UnityEngine;

[CreateAssetMenu(fileName = "New Cake", menuName = "Create Cake")]
public class CakeData : ScriptableObject
{
    [Header("Shop Settings")]
    public int cost = 0;
    public Sprite icon;
    public bool IsPreunlocked = false;

    [Header("Cake Settings")]
    public Mesh cakeMesh;
    public int icingNeeded;
    public float finalCakeYSize;

    public bool IsUnlocked
    {
        get
        {
            if (IsPreunlocked)
                return true;

            return PlayerPrefs.GetInt(name + "isUnlocked", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(name + "isUnlocked", value ? 1 : 0);
        }
    }
}
