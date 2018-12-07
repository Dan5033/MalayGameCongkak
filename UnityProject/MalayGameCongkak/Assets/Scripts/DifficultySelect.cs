using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Masters
{
    Safiya,
    Lee,
    Murugam,
    Kamal,
    Eric,
    Esther,
    TokSenah
}

public class DifficultySelect : MonoBehaviour {

    [SerializeField] RectTransform parent;
    [SerializeField] float disp;
    [SerializeField] Image[] potraits;
    [SerializeField] Button[] game;
    [SerializeField] Text[] names;
    [SerializeField] Text[] descs;
    private string[] nameList = new string[7];
    private string[] descList = new string[7];
    private int pointer = 0;
    private Coroutine slide;
    void Start ()
    {
        for (int i = 0; i < names.Length; i++)
        {
            potraits[i].color = Color.black;
            game[i].interactable = false;
            nameList[i] = names[i].text;
            descList[i] = descs[i].text;

            names[i].text = "???";
            descs[i].text = "Locked";
        }

        ResetLocks();
	}

    private void Update()
    {
        Touch[] touches;
        if (Input.GetMouseButtonDown(0))
        {
            touches = new Touch[1];
            touches[0] = new Touch();
            touches[0].phase = TouchPhase.Began;
            touches[0].position = Input.mousePosition;
        }
        else
        {
            touches = Input.touches;
        }

        //Move selection
        foreach (Touch i in touches)
        {
            if (i.position.x > Screen.width*2/3)
            {
                //Move Right
                pointer++;
            } else if (i.position.x < Screen.width/3)
            {
                //Move Left
                pointer--;
            }

            pointer = Mathf.Clamp(pointer,0, 6);

        }

        if (slide != null)
        {
            StopCoroutine(slide);
        }
        slide = StartCoroutine(SlideMasters());
    }

    public void GoToScene(string name)
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        SceneManager.LoadScene(name);
    }

    public void StartMatch(int master)
    {
        switch ((Masters) master)
        {
            case Masters.Safiya:
                AIGame.difficulty = Difficulty.easy;
                AIGame.marblePerSlot = 7;
                AIGame.startStyle = StartStyle.together;
                AIGame.afterStyle = AfterStyle.StartTogether;
                AIGame.roundsToWin = 2;
                AIGame.burntVillages = false;
                AIGame.timePerTurn = 0;
                break;
            case Masters.Lee:
                //8 seconds per turn
                AIGame.difficulty = Difficulty.medium;
                AIGame.marblePerSlot = 7;
                AIGame.startStyle = StartStyle.together;
                AIGame.afterStyle = AfterStyle.StartTogether;
                AIGame.roundsToWin = 2;
                AIGame.burntVillages = false;
                AIGame.timePerTurn = 8;
                break;
            case Masters.Murugam:
                //Long game
                AIGame.difficulty = Difficulty.medium;
                AIGame.marblePerSlot = 7;
                AIGame.startStyle = StartStyle.together;
                AIGame.afterStyle = AfterStyle.StartTogether;
                AIGame.roundsToWin = 3;
                AIGame.burntVillages = false;
                AIGame.timePerTurn = 0;
                break;
            case Masters.Kamal:
                //Burn Rule Enabled
                AIGame.difficulty = Difficulty.medium;
                AIGame.marblePerSlot = 7;
                AIGame.startStyle = StartStyle.together;
                AIGame.afterStyle = AfterStyle.StartTogether;
                AIGame.roundsToWin = 2;
                AIGame.burntVillages = true;
                AIGame.timePerTurn = 0;
                break;
            case Masters.Eric:
                //I start first
                AIGame.difficulty = Difficulty.medium;
                AIGame.marblePerSlot = 7;
                AIGame.startStyle = StartStyle.P2;
                AIGame.afterStyle = AfterStyle.P2Start;
                AIGame.roundsToWin = 2;
                AIGame.burntVillages = false;
                AIGame.timePerTurn = 0;
                break;
            case Masters.Esther:
                //3 Marble
                AIGame.difficulty = Difficulty.medium;
                AIGame.marblePerSlot = 3;
                AIGame.startStyle = StartStyle.together;
                AIGame.afterStyle = AfterStyle.StartTogether;
                AIGame.roundsToWin = 2;
                AIGame.burntVillages = false;
                AIGame.timePerTurn = 0;
                break;
            case Masters.TokSenah:
                //Hard mode
                AIGame.difficulty = Difficulty.hard;
                AIGame.marblePerSlot = 7;
                AIGame.startStyle = StartStyle.together;
                AIGame.afterStyle = AfterStyle.RoundWinner;
                AIGame.roundsToWin = 2;
                AIGame.burntVillages = true;
                AIGame.timePerTurn = 5;
                break;
        }

        GoToScene("GameAI");
    }

    private void ResetLocks()
    {
        int safiya = (int)Masters.Safiya;
        names[safiya].text = nameList[safiya];
        descs[safiya].text = descList[safiya];
        potraits[safiya].color = Color.white;
        game[safiya].interactable = true;


        bool granny = true;

        if (SaveData.currentSave.defeated[0])
        {
            for (int i = 1; i < 6; i++)
            {
                names[i].text = nameList[i];
                descs[i].text = descList[i];
                potraits[i].color = Color.white;
                game[i].interactable = true;

                granny = granny && SaveData.currentSave.defeated[i];
            }
        } else
        {
            granny = false;
        }

        if (granny)
        {
            int senah = (int)Masters.TokSenah;
            names[senah].text = nameList[senah];
            descs[senah].text = descList[senah];
            potraits[senah].color = Color.white;
            game[senah].interactable = true;
        }
    }

    IEnumerator SlideMasters()
    {
        Vector3 dest = parent.anchoredPosition;
        dest.x = -disp * pointer;
        while (Vector3.Distance(parent.anchoredPosition, dest) > 10)
        {
            parent.anchoredPosition = Vector3.Lerp(parent.anchoredPosition, dest, 0.1f);
            yield return null;
        }
    }
}
