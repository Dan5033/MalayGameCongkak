
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using System.Collections;

public class GPGSHandler : MonoBehaviour {

    public static GPGSHandler instance;
    const string fileExt = ".gr";
    const string fileName = "SaveData";
    static string dataPath;
    bool loadComplete = false;
    [SerializeField] Text loadingText; 

    private void Awake()
    {
        dataPath = Path.Combine(Application.persistentDataPath, fileName + fileExt);

        DontDestroyOnLoad(this);

        //Singleton Pattern
        if (instance == null)
        {
            instance = this;
        } else if (instance != this) {
            Destroy(this);
        }

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        SignIn();
    }

    private void Update()
    {
        if (JSONSaveData.currentSave != null)
        {
            JSONSaveData.currentSave.playTime += Time.deltaTime;
        }
    }

    private void SignIn()
    {
        Social.localUser.Authenticate(success => {
            if (success)
            {
                LoadingText("Authentication Succeeded");
            } else
            {
                LoadingText("Authentication Failed");
            }
            StartUpSequence();
            });
    }

    private void LoadingText(string str)
    {
        if (SceneManager.GetActiveScene().name == "LoadingScreen")
        {
            loadingText.text = str;
        } 
        Debug.Log(str);
    }

    public void StartUpSequence()
    {
        StartCoroutine(LoadMainMenu());

        string oldPath = Path.Combine(Application.persistentDataPath, "SaveFiles");
        oldPath = Path.Combine(oldPath, "SaveData.sav");
        //Check if old format exists
        if (File.Exists(oldPath) && !File.Exists(dataPath))
        {
            LoadingText("Old Save Format Detected");

            //Load Old format
            SaveData oldSave;
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (FileStream fileStream = File.Open(oldPath, FileMode.Open))
            {
                oldSave = (SaveData)binaryFormatter.Deserialize(fileStream);
            }

            LoadingText("Converting Old Save");
            //Save into new format
            JSONSaveData.currentSave = JSONSaveData.ConvertToNew(oldSave);
            SaveGame();

            loadComplete = true;
            //Done
        } else
        {
            LoadGame();
        }

        
    }

    IEnumerator LoadMainMenu()
    {
        while (!loadComplete)
        {
            yield return null;
        }

        JSONSaveData.currentSave.Reformat();
        SceneManager.LoadScene("MainMenu");
    }

    #region Achievements
    public void ShowAchievements()
    {
        Social.ShowAchievementsUI();
    }

    public void UnlockAchievement(string id)
    {
        Social.ReportProgress(id, 100, success => { });
    }

    public void UpdateAchievements()
    {
        if (JSONSaveData.currentSave.tutorialCompleted)
        {
            UnlockAchievement(GPGSIds.achievement_congkak_student);
        }
        if (JSONSaveData.currentSave.defeated[0])
        {
            instance.UnlockAchievement(GPGSIds.achievement_the_journey_begins);
        }
        if (JSONSaveData.currentSave.defeated[6])
        {
            instance.UnlockAchievement(GPGSIds.achievement_the_journey_ends);
        }
        bool unlocked = true;
        for (int i = 0; i < (int)MarbleDesign.Independence + 1; i++)
        {
            unlocked = unlocked && JSONSaveData.currentSave.marbleUnlocked[i];
        }
        if (unlocked)
        {
            instance.UnlockAchievement(GPGSIds.achievement_seasonal_collector);
        }
    }
    #endregion

    #region Leaderboards
    public void ShowLeaderboards()
    {
        Social.ShowLeaderboardUI();
    }

    public void ReportMaster()
    {
        System.DateTime now = System.DateTime.Now;
        int score = now.Year * 10000 + now.Month * 100 + now.Day;
        Social.ReportScore(score, GPGSIds.leaderboard_master_of_masters, success => { });
    }
    #endregion

    #region Events
    public void IncrementEvent(string id)
    {
        PlayGamesPlatform.Instance.Events.IncrementEvent(id, 1);
    }
    #endregion

    #region Save/Load Functions
    #region Save Game
    public void SaveGame()
    {
        //If user logged in to GPGS
        if (false)
        {
            LoadingText("User Authenticated");
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithManualConflictResolution(fileName,DataSource.ReadCacheOrNetwork, true, ConflictCallback, SaveOpenedCallback);
        } else
        {
            SaveGameLocal();
        }
    }

    private void SaveGameLocal()
    {
        LoadingText("File Saved Locally");
        string jsonString = JSONSaveData.currentSave.ToString();

        using (StreamWriter streamWriter = File.CreateText(dataPath))
        {
            streamWriter.Write(jsonString);
        }
    }

    private void SaveGameCloud(ISavedGameMetadata game)
    {
        string stringToSave = JSONSaveData.currentSave.ToString();
        //Save to local as well just in case
        LoadingText("Creating Local Backup");
        SaveGameLocal();

        //encoding to byte array
        byte[] dataToSave = Encoding.ASCII.GetBytes(stringToSave);
        //updating metadata with new description
        SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();
        LoadingText("Saving to Cloud");
        //uploading data to the cloud
        ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, update, dataToSave,OnSavedGameDataWritten);
    }

    private void SaveOpenedCallback(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        //if we are connected to the internet
        if (status == SavedGameRequestStatus.Success)
        {
            SaveGameCloud(game);
        }
        //if we couldn't successfully connect to the cloud, runs while on device,
        //the same code that is in else statements in LoadData() and SaveData()
        else
        {
            SaveGameLocal();
        }
    }

    private void OnSavedGameDataWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        //Save Completed
        if (status == SavedGameRequestStatus.Success)
        {
            LoadingText("Cloud Save Succeeded");
        } else
        {
            LoadingText("Cloud Save Failed");
        }
    }
    #endregion

    #region Load game
    public void LoadGame()
    {
        //if logged in to GPGS
        if (false)
        {
            LoadingText("User Authenticated");
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithManualConflictResolution(fileName,DataSource.ReadCacheOrNetwork, true, ConflictCallback, LoadOpenedCallback);
        } else
        {
            JSONSaveData.currentSave = LoadGameLocal();
        }
    }

    private JSONSaveData LoadGameLocal(bool complete = true)
    {
        if (File.Exists(dataPath))
        {
            LoadingText("Save file loaded from local");
            using (StreamReader streamReader = File.OpenText(dataPath))
            {
                string jsonString = streamReader.ReadToEnd();
                loadComplete = complete;
                return JsonUtility.FromJson<JSONSaveData>(jsonString);
            }
        } else
        {
            loadComplete = complete;
            LoadingText("No Save File found. Creating New File");
            return new JSONSaveData();
        }
    }

    private void LoadGameCloud(ISavedGameMetadata game)
    {
        ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, LoadCompletedCallback);
    }

    private void LoadOpenedCallback(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        //if we are connected to the internet
        if (status == SavedGameRequestStatus.Success)
        {
            LoadingText("Loading Save from Cloud");
            LoadGameCloud(game);
        }
        //if we couldn't successfully connect to the cloud, runs while on device,
        //the same code that is in else statements in LoadData() and SaveData()
        else
        {
            JSONSaveData.currentSave =  LoadGameLocal();
        }
    }

    private void LoadCompletedCallback(SavedGameRequestStatus status, byte[] savedData)
    {
        //if reading of the data was successful
        if (status == SavedGameRequestStatus.Success)
        {
            //if we've never played the game before, savedData will have length of 0
            if (savedData.Length == 0)
            {
                LoadingText("No save in cloud");
                JSONSaveData.currentSave = LoadGameLocal(false);
                return;
            } else
            {
                //Compare cloud and local
                JSONSaveData jsonCloud = JsonUtility.FromJson<JSONSaveData>(Encoding.ASCII.GetString(savedData));
                JSONSaveData jsonLocal = LoadGameLocal();

                LoadingText("Comparing files");
                if (jsonCloud.playTime >= jsonLocal.playTime)
                {
                    LoadingText("Loading from cloud");
                    loadComplete = true;
                    JSONSaveData.currentSave = jsonCloud;
                } else
                {
                    LoadingText("Loading from local");
                    loadComplete = true;
                    JSONSaveData.currentSave = jsonLocal;
                }
            }
        } else
        {
            JSONSaveData.currentSave = LoadGameLocal();
        }
    }
    #endregion

    private void ConflictCallback(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData,ISavedGameMetadata unmerged, byte[] unmergedData)
    {
        LoadingText("Conflict detected");
        if (originalData == null)
            resolver.ChooseMetadata(unmerged);
        else if (unmergedData == null)
            resolver.ChooseMetadata(original);
        else
        {
            //decoding byte data into string
            string originalStr = Encoding.ASCII.GetString(originalData);
            string unmergedStr = Encoding.ASCII.GetString(unmergedData);

            JSONSaveData originalJson;
            JSONSaveData unmergedJson;
            //parsing
            try
            {
                originalJson = JsonUtility.FromJson<JSONSaveData>(originalStr);
            }
            catch (System.Exception e)
            {
                originalJson = null;
                Debug.Log("Error on original" + e);
                throw;
            };

            try
            {
                unmergedJson = JsonUtility.FromJson<JSONSaveData>(unmergedStr);
            }
            catch (System.Exception e)
            { 
                unmergedJson = null;
                Debug.Log("Error on unmerged" + e);
                throw;
            };

            if (originalJson == null)
            {
                if (unmergedJson == null)
                {
                    JSONSaveData.currentSave = LoadGameLocal();
                } else
                {
                    resolver.ChooseMetadata(unmerged);
                }
            } else
            {
                if (unmergedJson == null)
                {
                    resolver.ChooseMetadata(original);
                }
                else
                {
                    //Comparison
                    if (originalJson.playTime >= unmergedJson.playTime)
                    {
                        resolver.ChooseMetadata(original);
                    }
                    else
                    {
                        resolver.ChooseMetadata(unmerged);
                    }
                }
            }
        }
        LoadingText("Conflict resolved");
    }

    #endregion


}
