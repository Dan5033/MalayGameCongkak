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

    private void Awake()
    {
        SaveData.LoadGame();
    }

    void Start ()
    {
        foreach (Rigidbody2D i in marbles)
        {
            i.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            i.angularVelocity = Random.Range(-1, 1);
        }

        AdManager.InitializeAdMob();
        AdManager.RequestBanner();

        //Unlock New Marbles
        if (SaveData.currentSave.defeated[6])
        {
            UnlockMarble(MarbleDesign.Golden);
        }

        int month  = System.DateTime.Now.Month;
        int day = System.DateTime.Now.Day;

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
    }

    private void Update()
    {
        settings.transform.Rotate(new Vector3(0, 0, 1));

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.KeypadEnter))
        {
            SaveData.currentSave.tutorialCompleted = !SaveData.currentSave.tutorialCompleted;
            Debug.Log("Tutorial Completed: " + SaveData.currentSave.tutorialCompleted);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SaveData.currentSave.defeated[0] = !SaveData.currentSave.defeated[0];
            Debug.Log("Master 0 defeated: " + SaveData.currentSave.defeated[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SaveData.currentSave.defeated[1] = !SaveData.currentSave.defeated[1];
            Debug.Log("Master 1 defeated: " + SaveData.currentSave.defeated[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SaveData.currentSave.defeated[2] = !SaveData.currentSave.defeated[2];
            Debug.Log("Master 2 defeated: " + SaveData.currentSave.defeated[2]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SaveData.currentSave.defeated[3] = !SaveData.currentSave.defeated[3];
            Debug.Log("Master 3 defeated: " + SaveData.currentSave.defeated[3]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SaveData.currentSave.defeated[4] = !SaveData.currentSave.defeated[4];
            Debug.Log("Master 4 defeated: " + SaveData.currentSave.defeated[4]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SaveData.currentSave.defeated[5] = !SaveData.currentSave.defeated[5];
            Debug.Log("Master 5 defeated: " + SaveData.currentSave.defeated[5]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SaveData.currentSave.defeated[6] = !SaveData.currentSave.defeated[6];
            Debug.Log("Master 6 defeated: " + SaveData.currentSave.defeated[6]);
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
#endif
    }

    public void CloseGame()
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        AdManager.bannerView.Destroy();
        Application.Quit();
    }

    public void GoToRoom(string sceneName)
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        SceneManager.LoadScene(sceneName);
        AdManager.bannerView.Destroy();
    }

    private void UnlockMarble(MarbleDesign design)
    {
        if (!SaveData.currentSave.marbleUnlocked[(int) design])
        {
            SaveData.currentSave.marbleUnlocked[(int)design] = true;

            MarbleNotification obj = Instantiate(prefab).GetComponent<MarbleNotification>();

            obj.transform.SetParent(transform);
            obj.Setup(design);
        }
    }
}
