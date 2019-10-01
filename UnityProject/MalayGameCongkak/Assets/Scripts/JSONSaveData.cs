
using UnityEngine;

public class JSONSaveData
{
    public static JSONSaveData currentSave;
    public float playTime = 0;

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

    public JSONSaveData()
    {
        marbleUnlocked[0] = true;
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

    public new string ToString()
    {
        return JsonUtility.ToJson(this);
    }

    public void Reformat()
    {
        bool[] newFormat = new bool[(int)MarbleDesign.Golden + 1];
        for (int i = 0; i < (int) MarbleDesign.Golden; i++)
        {
            if (marbleUnlocked.Length > i)
            {
                newFormat[i] = marbleUnlocked[i];
            }
        }
        newFormat[(int)MarbleDesign.Golden] = marbleUnlocked[marbleUnlocked.Length - 1];

        marbleUnlocked = newFormat;
    }

}
