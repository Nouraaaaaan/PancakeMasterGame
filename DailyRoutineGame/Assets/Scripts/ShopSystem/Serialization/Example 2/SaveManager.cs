using UnityEngine;
using System.IO;

public static class SaveManager
{
    public static string directory = "/SaveData/";
    public static string filename = "MyData";

    public static void save(SaveObject saveObject)
    {
        //string dir = Application.persistentDataPath + directory;
        string dir = Path.Combine(Application.persistentDataPath, "SaveData");
        //Debug.Log(dir);

        if (!Directory.Exists(dir))
        {
            //Debug.Log("Create directory !");
            Directory.CreateDirectory(dir);
        }

        string json = JsonUtility.ToJson(saveObject);
        //File.WriteAllText(dir + "MyData.txt", json);
        File.WriteAllText(Path.Combine(dir, "MyData.txt"), json);
    }

    public static SaveObject Load()
    {
        //string fullPath = Application.persistentDataPath + directory + filename;
        string fullPath = Path.Combine(Application.persistentDataPath, "SaveData", "MyData.txt");

        SaveObject saveObject = new SaveObject();

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            saveObject = JsonUtility.FromJson<SaveObject>(json);
        }
        else
        {
            Debug.Log("File doen't exit !");
        }

        return saveObject;
    }

}
