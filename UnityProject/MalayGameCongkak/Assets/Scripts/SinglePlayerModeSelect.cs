using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SinglePlayerModeSelect : MonoBehaviour {

    [SerializeField] Button btVersus;
    [SerializeField] Button btFreePlay;

    [SerializeField] GameObject tutorialBadge;
    private Vector3 tutorialBadgeDest;

	void Start () {
        tutorialBadgeDest = tutorialBadge.transform.position;
	}
	
	void Update () {
		if (SaveData.currentSave.tutorialCompleted)
        {
            TutorialComplete();
        }

        if (SaveData.currentSave.tutorialBadgeShown)
        {
            btVersus.interactable = true;
        }
	}

    public void GoToRoom(string sceneName)
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        SceneManager.LoadScene(sceneName);
    }

    void TutorialComplete()
    {
        tutorialBadge.transform.position = tutorialBadgeDest + new Vector3(10000, 0, 0);
        StartCoroutine(MoveObject(tutorialBadge, tutorialBadgeDest));
    }

    IEnumerator MoveObject(GameObject gameObject, Vector3 dest)
    {
        while (Vector3.Distance(gameObject.transform.position,dest) > 100)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, dest, 0.1f);
            yield return null;
        }

        gameObject.transform.position = dest;
    }
}
