using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGameScreen : MonoBehaviour {

    [Header("UI Objects")]
    [SerializeField] Canvas endGameCanvas;
    [SerializeField] GameObject title;
    [SerializeField] Text winnerText;
    [SerializeField] GameObject rematchButton;
    [SerializeField] GameObject homeButton;

    [SerializeField] string mainMenuName = "MainMenu";

	void Start ()
    {
		
	}
	
	void Update ()
    {
	}

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuName);
        VideoAdManager.DestroyAd();
    }

    public void Rematch()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        VideoAdManager.DestroyAd();
    }

    public void EnableCanvas()
    {
        endGameCanvas.enabled = true;
        StartCoroutine(MoveTitle());
        if (Game.instance.winner == 0)
        {
            winnerText.text = "P1 WINS";
        } else
        {
            winnerText.text = "P2 WINS";
        }
    }

    IEnumerator MoveTitle()
    {
        Vector3 destination = new Vector3(Screen.width / 2, Screen.height * 3 / 4, 0);
        Vector3 rematchDest = new Vector3(Screen.width / 2, Screen.height * 2 / 4, 0);
        Vector3 homeDest = new Vector3(Screen.width / 2, Screen.height * 1 / 4, 0);
        while (Vector3.Distance(title.transform.position,destination) > Mathf.Epsilon)
        {
            title.transform.position = Vector3.Lerp(title.transform.position, destination, 0.1f);
            rematchButton.transform.position = Vector3.Lerp(rematchButton.transform.position, rematchDest, 0.1f);
            homeButton.transform.position = Vector3.Lerp(homeButton.transform.position, homeDest, 0.1f);
            yield return null;
        }
    }
}
