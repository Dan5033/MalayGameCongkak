using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [Header("Scene Names")]
    [SerializeField] private string modeSelectName = "";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GoToModeSelection()
    {
        SceneManager.LoadScene(modeSelectName);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
