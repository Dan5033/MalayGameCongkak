using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    [SerializeField] private Canvas pauseCanvas;
    private GameState lastState;
    [SerializeField] private string mainMenuSceneName;

    public void PauseGame(int player)
    {
        if (player == 0)
        {
            pauseCanvas.transform.eulerAngles = new Vector3(0, 0, 0);
        } else if (player == 1)
        {
            pauseCanvas.transform.eulerAngles = new Vector3(0, 0, 180);
        }

        lastState = Game.instance.turn;
        Game.instance.turn = GameState.Paused;

        pauseCanvas.enabled = true;
    }

    public void ResumeGame()
    {
        Game.instance.turn = lastState;

        pauseCanvas.enabled = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        AdManager.instance.DestroyBanner();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
        AdManager.instance.DestroyBanner();
    }
}
