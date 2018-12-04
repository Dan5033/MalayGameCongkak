using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    [SerializeField] private Canvas pauseCanvas;
    private GameState lastState;
    [SerializeField] private string mainMenuSceneName;

    public void PauseGame()
    {
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
        VideoAdManager.DestroyAd();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
        VideoAdManager.DestroyAd();
    }
}
