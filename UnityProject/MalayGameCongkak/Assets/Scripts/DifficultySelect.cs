﻿using System.Collections;
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
    TokSenah,
    Free
}

public class DifficultySelect : MonoBehaviour {

    [SerializeField] RectTransform parent;
    [SerializeField] float disp;
    [SerializeField] MasterPotrait[] masterPotraits;
    private int pointer = 0;
    private Coroutine slide;

    private Vector3 offset;
    private Vector3 velocity;

    private bool validTouch = true;

    void Start ()
    {
        ResetLocks();
	}

    private void Update()
    {
        //Save offset
        if (Input.GetMouseButtonDown(0))
        {
            offset = parent.transform.position - Input.mousePosition;
            validTouch = true;
        }

        //Move potraits
        if (Input.GetMouseButton(0))
        {
            Vector3 position = Input.mousePosition + offset;
            position.y = parent.transform.position.y;
            velocity = position - parent.transform.position;
            parent.transform.position = position;

            if (velocity.magnitude > 3)
            {
                validTouch = false;
            }
        } else
        {
            //velocity
            parent.transform.position += velocity;
            velocity = Vector3.Lerp(velocity, Vector3.zero, 0.1f);
        }

        //Correction
        Vector3 pos = parent.anchoredPosition;
        if (pos.x > 0)
        {
            pos.x = 0;
            parent.anchoredPosition = Vector3.Lerp(parent.anchoredPosition, pos, 0.5f);
        } else if (pos.x < -disp*(masterPotraits.Length - 1))
        {
            pos.x = -disp * (masterPotraits.Length - 1);
            parent.anchoredPosition = Vector3.Lerp(parent.anchoredPosition, pos, 0.5f);
        }
    }

    public void GoToScene(string name)
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        SceneManager.LoadScene(name);
    }

    public void StartMatch(int master)
    {
        //Make sure doen't accidentally start level
        if (velocity.magnitude  < 3 && validTouch)
        {
            switch ((Masters)master)
            {
                case Masters.Safiya:
                    AIGame.style = Style.Simple;
                    AIGame.marblePerSlot = 7;
                    AIGame.startStyle = StartStyle.together;
                    AIGame.afterStyle = AfterStyle.StartTogether;
                    AIGame.roundsToWin = 2;
                    AIGame.burntVillages = false;
                    AIGame.timePerTurn = 0;
                    break;
                case Masters.Lee:
                    //5 seconds per turn
                    AIGame.style = Style.Speed;
                    AIGame.marblePerSlot = 7;
                    AIGame.startStyle = StartStyle.together;
                    AIGame.afterStyle = AfterStyle.StartTogether;
                    AIGame.roundsToWin = 2;
                    AIGame.burntVillages = false;
                    AIGame.timePerTurn = 5;
                    break;
                case Masters.Murugam:
                    //Long game
                    AIGame.style = Style.Defense;
                    AIGame.marblePerSlot = 7;
                    AIGame.startStyle = StartStyle.together;
                    AIGame.afterStyle = AfterStyle.StartTogether;
                    AIGame.roundsToWin = 3;
                    AIGame.burntVillages = false;
                    AIGame.timePerTurn = 0;
                    break;
                case Masters.Kamal:
                    //Burn Rule Enabled
                    AIGame.style = Style.Predictive;
                    AIGame.marblePerSlot = 7;
                    AIGame.startStyle = StartStyle.together;
                    AIGame.afterStyle = AfterStyle.StartTogether;
                    AIGame.roundsToWin = 2;
                    AIGame.burntVillages = true;
                    AIGame.timePerTurn = 0;
                    break;
                case Masters.Eric:
                    //I start first
                    AIGame.style = Style.Balanced;
                    AIGame.marblePerSlot = 7;
                    AIGame.startStyle = StartStyle.P2;
                    AIGame.afterStyle = AfterStyle.P2Start;
                    AIGame.roundsToWin = 2;
                    AIGame.burntVillages = false;
                    AIGame.timePerTurn = 0;
                    break;
                case Masters.Esther:
                    //3 Marble
                    AIGame.style = Style.Offensive;
                    AIGame.marblePerSlot = 3;
                    AIGame.startStyle = StartStyle.together;
                    AIGame.afterStyle = AfterStyle.StartTogether;
                    AIGame.roundsToWin = 2;
                    AIGame.burntVillages = false;
                    AIGame.timePerTurn = 0;
                    break;
                case Masters.TokSenah:
                    //Hard mode
                    AIGame.style = Style.Complex;
                    AIGame.marblePerSlot = 7;
                    AIGame.startStyle = StartStyle.together;
                    AIGame.afterStyle = AfterStyle.RoundWinner;
                    AIGame.roundsToWin = 2;
                    AIGame.burntVillages = true;
                    AIGame.timePerTurn = 5;
                    break;
            }
            AIGame.master = (Masters)master;

            GoToScene("GameAI");
        }
    }

    private void ResetLocks()
    {
        masterPotraits[(int)Masters.Safiya].SetLocked(false);

        bool granny = true;

        if (JSONSaveData.currentSave.defeated[0])
        {
            for (int i = 1; i < 6; i++)
            {
                masterPotraits[i].SetLocked(false);

                granny = granny && JSONSaveData.currentSave.defeated[i];
            }
        } else
        {
            for (int i = 1; i < 6; i++)
            {
                masterPotraits[i].SetLocked(true);
            }
            granny = false;
        }

        if (granny)
        {
            masterPotraits[(int)Masters.TokSenah].SetLocked(false);
        } else
        {
            masterPotraits[(int)Masters.TokSenah].SetLocked(true);
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
