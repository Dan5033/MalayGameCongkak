using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SinglePlayerModeSelect : MonoBehaviour {

    [SerializeField] Button btVersus;
    [SerializeField] Text textVersus;
    [SerializeField] Button btFreePlay;
    [SerializeField] Text textFree;

    [SerializeField] GameObject tutorialBadge;
    private Vector3 tutorialBadgeDest;

	void Start () {
        tutorialBadgeDest = tutorialBadge.transform.position;
	}
	
	void Update () {
        if (JSONSaveData.currentSave.tutorialBadgeShown)
        {
            tutorialBadge.SetActive(true);
            btVersus.interactable = true;
            textVersus.text = "Versus";
        } else
        {
            if (JSONSaveData.currentSave.tutorialCompleted)
            {
                TutorialComplete();
            }
        }

        if (JSONSaveData.currentSave.defeated[0])
        {
            btFreePlay.interactable = true;
            textFree.text = "Free Play";
        }
	}

    public void GoToRoom(string sceneName)
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        SceneManager.LoadScene(sceneName);
    }

    void TutorialComplete()
    {
        tutorialBadge.SetActive(true);
        JSONSaveData.currentSave.tutorialBadgeShown = true;
        GPGSHandler.instance.SaveGame();
    }
}
