using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] List<Color> colors;
    [SerializeField] ColorTriggers[] colorTriggers;

    [Header("Components")]
    [SerializeField] MeshFilter rawCake;
    [SerializeField] MeshFilter playerCake;
    [SerializeField] MeshFilter goalCake;
    [SerializeField] Gameplay gameplay;

    int level = 1;

    [System.Serializable]
    public class ColorTriggers
    {
        public ColorTrigger[] colorTriggers;
    }

    private void Awake()
    {
        //level = PlayerPrefs.GetInt("Level", 1);
       // Random.InitState(level);
        SetMeshes();       
    }

    public void SetMeshes()
    {
        int r = 6;// Random.Range(0, Shop.Instance.UnlockedCakeData.Count);
       
        Mesh currentMesh = Shop.Instance.UnlockedCakeData[r].cakeMesh;

        rawCake.mesh = currentMesh;
        playerCake.mesh = currentMesh;
        goalCake.mesh = currentMesh;

        rawCake.GetComponent<MeshCollider>().sharedMesh = currentMesh;
        playerCake.GetComponent<Cake>().icingAmountNeeded = Shop.Instance.UnlockedCakeData[r].icingNeeded;
        playerCake.transform.localScale = new Vector3(playerCake.transform.localScale.x, Shop.Instance.UnlockedCakeData[r].finalCakeYSize, playerCake.transform.localScale.z);

        CreateColorSet(r);
    }

    public Color GetColor()
    {
        int r = Random.Range(0, colors.Count);

        Color colorToReturn = colors[r];
        colors.RemoveAt(r);

        return colorToReturn;
    }

    public void CreateColorSet(int meshID)
    {
        int r = Random.Range(0, colorTriggers[meshID].colorTriggers.Length);

        Instantiate(colorTriggers[meshID].colorTriggers[r].gameObject, goalCake.transform).GetComponent<ColorTrigger>().Init();
    }
}
