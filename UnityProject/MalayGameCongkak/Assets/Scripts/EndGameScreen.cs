using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGameScreen : MonoBehaviour {

    [Header("UI Objects")]
    [SerializeField] Canvas endGameCanvas;
    [SerializeField] Text winnerText;
    [SerializeField] Image potrait;
    [SerializeField] Image box;
    [SerializeField] Text text;

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        AdManager.DestroyInsterstitial();
    }

    public void Rematch()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        AdManager.DestroyInsterstitial();
    }

    public void EnableCanvas(bool ai, string text, Sprite master)
    {
        endGameCanvas.enabled = true;
        potrait.sprite = master;
        StartCoroutine(TextDisplay(text, 0.01f));
        if (ai)
        {
            if (Game.instance.winner == 0)
            {
                winnerText.text = "YOU WIN";
            }
            else
            {
                winnerText.text = "AI WINS";
            }
        } else
        {
            if (Game.instance.winner == 0)
            {
                winnerText.text = "P1 WINS";
            }
            else
            {
                winnerText.text = "P2 WINS";
            }
        }
    }

    IEnumerator TextDisplay(string text, float delay)
    {
        int pointer = 0;

        while (pointer < text.Length)
        {
            pointer++;

            this.text.text = text.Substring(0, pointer);
            yield return new WaitForSeconds(delay);
        }
    }
}
