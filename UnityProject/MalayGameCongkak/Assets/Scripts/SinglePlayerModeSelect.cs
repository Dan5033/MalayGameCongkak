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
        if (SaveData.currentSave.tutorialBadgeShown)
        {
            tutorialBadge.SetActive(true);
            btVersus.interactable = true;
            textVersus.text = "Versus";
        } else
        {
            if (SaveData.currentSave.tutorialCompleted)
            {
                TutorialComplete();
            }
        }

        if (SaveData.currentSave.defeated[0])
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
        tutorialBadge.transform.position = tutorialBadgeDest + new Vector3(10000, 0, 0);
        StartCoroutine(MoveObject(tutorialBadge, tutorialBadgeDest));
        SaveData.currentSave.tutorialBadgeShown = true;
        SaveData.SaveGame();
    }

    IEnumerator MoveObject(GameObject gameObject, Vector3 dest)
    {
        while (Vector3.Distance(gameObject.transform.position,dest) > 10)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, dest, 0.1f);
            yield return null;
        }

        gameObject.transform.position = dest;
    }
}
