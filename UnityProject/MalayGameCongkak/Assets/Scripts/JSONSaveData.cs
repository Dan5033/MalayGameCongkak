using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class JSONSaveData
{

    public static JSONSaveData currentSave;
    const string fileName = "SaveDara.gr";

    //Settings
    public float BGMVol = 1;
    public float SFXVol = 1;
    public int displayType = 0;

    //Tutorial
    public bool tutorialCompleted = false;
    public bool tutorialBadgeShown = false;

    //Masters
    public bool[] defeated = new bool[7];

    //Unlocks
    public bool[] marbleUnlocked = new bool[(int)MarbleDesign.Golden + 1];
    public MarbleDesign selectedDesign = MarbleDesign.Basic;

    static string dataPath = Path.Combine(Application.persistentDataPath, "SaveData.gr");

    public JSONSaveData()
    {
        marbleUnlocked[0] = true;
    }

    public static void SaveGame()
    {
        string dataPath = Path.Combine(Application.persistentDataPath, "SaveData.gr");

        string jsonString = JsonUtility.ToJson(currentSave);

        using (StreamWriter streamWriter = File.CreateText(dataPath))
        {
            streamWriter.Write(jsonString);
        }
    }

    public static void LoadGame()
    {
        using (StreamReader streamReader = File.OpenText(dataPath))
        {
            string jsonString = streamReader.ReadToEnd();
            currentSave = JsonUtility.FromJson<JSONSaveData>(jsonString);
        }
    }

    public static void StartUpSequence()
    {
        string oldPath = Path.Combine(Application.persistentDataPath, "SaveFiles");
        oldPath = Path.Combine(oldPath, "SaveData.sav");
        //Check if old format exists
        if (File.Exists(oldPath))
        {
            Debug.Log("Old Save Found");

            SaveData oldSave;

            //Load Old format
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (FileStream fileStream = File.Open(oldPath, FileMode.Open))
            {
                Debug.Log("Old Save Loaded");
                oldSave = (SaveData)binaryFormatter.Deserialize(fileStream);
            }

            //Save into new format
            Debug.Log("Old Save Converted");
            currentSave = ConvertToNew(oldSave);
            SaveGame();

            //Delete Old
            Debug.Log("Old Save Deleted");
            File.Delete(oldPath);

            //Done
        } else if (!File.Exists(dataPath))
        {
            Debug.Log("No Data Found");
            //Create New Save
            Debug.Log("New Data Created");
            currentSave = new JSONSaveData();
            //Save Game
            SaveGame();
            //Done
        } else
        {
            //Load Game
            LoadGame();

            //Done
        }
    }

    public static JSONSaveData ConvertToNew(SaveData file)
    {
        JSONSaveData newSave = new JSONSaveData();

        newSave.BGMVol = file.BGMVol;
        newSave.SFXVol = file.SFXVol;
        newSave.displayType = file.displayType;
        newSave.tutorialCompleted = file.tutorialCompleted;
        newSave.tutorialBadgeShown = file.tutorialBadgeShown;
        newSave.displayType = file.displayType;

        for (int i = 0; i < file.defeated.Length; i++)
        {
            newSave.defeated[i] = file.defeated[i];
        }

        for (int i = 0; i < file.marbleUnlocked.Length - 1; i++)
        {
            newSave.marbleUnlocked[i] = file.marbleUnlocked[i];
        }

        newSave.marbleUnlocked[(int)MarbleDesign.Golden] = file.marbleUnlocked[file.marbleUnlocked.Length - 1];

        newSave.selectedDesign = file.selectedDesign;

        return newSave;
    }

}
