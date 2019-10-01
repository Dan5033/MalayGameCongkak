using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [Header("Move")]
    public Rigidbody2D[] marbles;
    [SerializeField] private GameObject settings;

    [Header("New Marble")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private Sprite[] marbleSprite;

    [SerializeField] CommentNotification cn;

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        Marble.sprites = marbleSprite;

        AdManager.instance.InitializeAdMob();
        AdManager.instance.RequestBanner();
    }

    void Start ()
    {
        foreach (Rigidbody2D i in marbles)
        {
            i.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            i.angularVelocity = Random.Range(-1, 1);
        }

        #region Marble Unlock

        //Unlock New Marbles
        System.DateTime today = System.DateTime.Now;
        int day = PlayerPrefs.GetInt("DateDay", today.Day);
        int month = PlayerPrefs.GetInt("DateMonth", today.Month);
        int year = PlayerPrefs.GetInt("DateYear", today.Year);

        if (day <= 24 && month <= 12 && year <= 2018)
        {
            PlayerPrefs.SetInt("IsBetaTester", 1);
        }
        if (PlayerPrefs.HasKey("IsBetaTester") && PlayerPrefs.GetInt("IsBetaTester") == 1)
        {
            UnlockMarble(MarbleDesign.BT);
        }

        if (day >= 25 && day <= 31 && month == 12 && year == 2018)
        {
            PlayerPrefs.SetInt("IsEarlyAdopter", 1);
        }
        if (PlayerPrefs.HasKey("IsEarlyAdopter") && PlayerPrefs.GetInt("IsEarlyAdopter") == 1)
        {
            UnlockMarble(MarbleDesign.MGC);
        }

        if (JSONSaveData.currentSave.defeated[(int) Masters.TokSenah])
        {
            UnlockMarble(MarbleDesign.Golden);
        }

        switch(day + "/" + month)
        {
            case "25/12":
                UnlockMarble(MarbleDesign.Christmas);
                break;
            case "6/11":
                UnlockMarble(MarbleDesign.Deepavali);
                break;
            case "15/6":
                UnlockMarble(MarbleDesign.Eid);
                break;
            case "16/2":
                UnlockMarble(MarbleDesign.CNY);
                break;
            case "16/9":
                UnlockMarble(MarbleDesign.MalaysiaDay);
                break;
            case "31/8":
                UnlockMarble(MarbleDesign.Independence);
                break;
        }

        #endregion

        GPGSHandler.instance.UpdateAchievements();

        int matchNum = PlayerPrefs.GetInt("MatchNum",1);
        if (matchNum > -1)
        {
            if (matchNum == 0)
            {
                cn.ShowNotification();
                matchNum++;
            }
        }
    }

    private void Update()
    {
        settings.transform.Rotate(new Vector3(0, 0, 1));

        #region Test Functions
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerPrefs.SetInt("MatchNum", 0);
            Debug.Log("Match Num: " + PlayerPrefs.GetInt("MatchNum"));
        }

        if (Input.GetKey(KeyCode.KeypadEnter))
        {
            JSONSaveData.currentSave.tutorialCompleted = !JSONSaveData.currentSave.tutorialCompleted;
            Debug.Log("Tutorial Completed: " + JSONSaveData.currentSave.tutorialCompleted);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            JSONSaveData.currentSave.defeated[0] = !JSONSaveData.currentSave.defeated[0];
            Debug.Log("Master 0 defeated: " + JSONSaveData.currentSave.defeated[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            JSONSaveData.currentSave.defeated[1] = !JSONSaveData.currentSave.defeated[1];
            Debug.Log("Master 1 defeated: " + JSONSaveData.currentSave.defeated[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            JSONSaveData.currentSave.defeated[2] = !JSONSaveData.currentSave.defeated[2];
            Debug.Log("Master 2 defeated: " + JSONSaveData.currentSave.defeated[2]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            JSONSaveData.currentSave.defeated[3] = !JSONSaveData.currentSave.defeated[3];
            Debug.Log("Master 3 defeated: " + JSONSaveData.currentSave.defeated[3]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            JSONSaveData.currentSave.defeated[4] = !JSONSaveData.currentSave.defeated[4];
            Debug.Log("Master 4 defeated: " + JSONSaveData.currentSave.defeated[4]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            JSONSaveData.currentSave.defeated[5] = !JSONSaveData.currentSave.defeated[5];
            Debug.Log("Master 5 defeated: " + JSONSaveData.currentSave.defeated[5]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            JSONSaveData.currentSave.defeated[6] = !JSONSaveData.currentSave.defeated[6];
            Debug.Log("Master 6 defeated: " + JSONSaveData.currentSave.defeated[6]);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UnlockMarble(MarbleDesign.Christmas);
            Debug.Log("ASD");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            UnlockMarble(MarbleDesign.CNY);
            Debug.Log("ASD");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UnlockMarble(MarbleDesign.Deepavali);
            Debug.Log("ASD");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnlockMarble(MarbleDesign.Eid);
            Debug.Log("ASD");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            UnlockMarble(MarbleDesign.Golden);
            Debug.Log("ASD");
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            UnlockMarble(MarbleDesign.Independence);
            Debug.Log("ASD");
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            UnlockMarble(MarbleDesign.MalaysiaDay);
            Debug.Log("ASD");
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            UnlockMarble(MarbleDesign.MGC);
            Debug.Log("ASD");
        }
#endif
        #endregion
    }

    public void CloseGame()
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        Application.Quit();
    }

    public void GoToRoom(string sceneName)
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        SceneManager.LoadScene(sceneName);
    }

    private void UnlockMarble(MarbleDesign design)
    {
        if (!JSONSaveData.currentSave.marbleUnlocked[(int) design])
        {
            JSONSaveData.currentSave.marbleUnlocked[(int) design] = true;

            MarbleNotification obj = Instantiate(prefab).GetComponent<MarbleNotification>();

            obj.transform.SetParent(transform);
            obj.Setup(design);
        }
    }
}
