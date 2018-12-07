using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]

public class SaveData {
    public static SaveData currentSave;
    const string folderName = "SaveFiles";
    const string fileExtension = ".sav";

    //Volumes
    public float BGMVol = 1;
    public float SFXVol = 1;

    //Tutorial
    public bool tutorialCompleted = false;
    public bool tutorialBadgeShown = false;

    //Masters
    public bool[] defeated = new bool[7];
    public bool[] defeatAnimation = new bool[7];

    public SaveData()
    {
    }

    public static void SaveGame()
    {
        //Combine a path properly
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);

        //Create Save folder
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string dataPath = Path.Combine(folderPath, "SaveData" + fileExtension);

        //Create binaryformatter
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, currentSave);
            Debug.Log("File saved at: " + dataPath);
        }
    }

    public static void LoadGame()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        string dataPath = Path.Combine(folderPath, "SaveData" + fileExtension);

        if (File.Exists(dataPath))
        {
            //Create Binary formatter
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (FileStream fileStream = File.Open(dataPath, FileMode.Open))
            {
                currentSave =  (SaveData)binaryFormatter.Deserialize(fileStream);
                Debug.Log("File loaded from: " + dataPath);
            }
        } else
        {
            currentSave = new SaveData();
            SaveGame();
        }
    }
}
