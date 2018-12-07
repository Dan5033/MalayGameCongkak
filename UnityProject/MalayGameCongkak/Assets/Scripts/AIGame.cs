using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Difficulty
{
    easy,
    medium,
    hard
}

public class AIGame : Game {

    public static Difficulty difficulty = Difficulty.medium;
    protected AI ai;
    protected float aiThinking = 1;
    protected float waitTime = 0;

    [SerializeField] protected Button ffButton;

    new protected void Start ()
    {

        //Create AISystem
        ai = new AI(difficulty,noGoSlots[1]);
        aiThinking = ai.WaitTime();
        waitTime = ai.WaitTime();

        base.Start();
    }
	
	void Update ()
    {
        //Move Marbles in hand into a line
        MoveHandMarbles();

        if (turn != GameState.Paused)
        {
            UpdateSlotSprite();

            //Time
            ProgressTime();
            if (turn != GameState.Paused)
            {
                aiThinking -= Time.deltaTime;
            }

            //Change instruction display
            InstructionDisplay();

            //AI
            if (aiThinking <= 0 && nextSlot[1] > -2)
            {
                switch (turn)
                {
                    case GameState.PickingStartSlot:
                        //Pick a slot
                        if (nextSlot[1] < 0)
                        {
                            ai.UpdateWorld(slots);
                            nextSlot[1] = ai.NextMove();
                        }
                        break;
                    case GameState.BothTurns:
                    case GameState.P2Turn:
                        if (nextSlot[1] == -1)
                        {
                            ai.UpdateWorld(slots);
                            int tgt = ai.NextMove();
                            PlayerTurn(1, slots[tgt]);
                        } else
                        {
                            PlayerTurn(1, slots[nextSlot[1]]);
                        }
                        break;
                }
                aiThinking = waitTime;
            }

            //Player's Turn
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

            switch (turn)
            {
                case GameState.PickingStartSlot:
                    foreach (Touch i in touches)
                    {
                        Slot slot = FindSlotOnTouch(i);
                        if (slot != null)
                        {
                            //Select start
                            if (slot.slotID >= 0 && slot.slotID <= 6)
                            {
                                nextSlot[0] = slot.slotID;
                                AudioController.instance.PlaySoundEffect(Context.MarblePlace);
                            }

                            //If both player slected
                            if (nextSlot[0] > -1 && nextSlot[1] > -1)
                            {
                                //Start Game
                                turn = GameState.BothTurns;
                                //Give player marbles
                                slots[nextSlot[0]].SurrenderMarbles(0);
                                slots[nextSlot[1]].SurrenderMarbles(1);
                                //Move Slot by 1
                                ProgressSlot(0);
                                ProgressSlot(1);
                            }
                        }
                    }
                    break;
                case GameState.BothTurns:
                    foreach (Touch i in touches)
                    {
                        Slot slot = FindSlotOnTouch(i);
                        if (slot != null)
                        {
                            //First Turn
                            PlayerTurn(0, slot);
                            PlayerTurn(1, slot);
                        }
                    }

                    //Record who done first. He will move next
                    if (doneFirst < 0)
                    {
                        if (turnDone[0])
                        {
                            doneFirst = 0;
                        }
                        else if (turnDone[1])
                        {
                            doneFirst = 1;
                        }
                    }

                    //If time is up, surrender all marble in hand to the opponent
                    if (turnDone[0])
                    {
                        if (marblesHand[0].Count > 0)
                        {
                            slots[7].StoreMarbles(marblesHand[0]);
                            marblesHand[0] = new List<GameObject>();
                        }
                    }
                    else if (turnDone[1])
                    {
                        if (marblesHand[1].Count > 0)
                        {
                            slots[15].StoreMarbles(marblesHand[1]);
                            marblesHand[1] = new List<GameObject>();
                        }
                    }

                    //Setup Next Turn
                    if (turnDone[0] && turnDone[1])
                    {
                        turn = (GameState)((int)GameState.P1Turn + 1);
                        SetupTurn(doneFirst);
                    }
                    break;
                case GameState.P1Turn:
                    foreach (Touch i in touches)
                    {
                        Slot slot = FindSlotOnTouch(i);
                        if (slot != null)
                        {
                            //P1 Turn
                            PlayerTurn(0, slot);
                        }
                    }

                    if (turnDone[0])
                    {
                        //Setup next turn
                        if (PlayerValidSlotsNumber(1) > 0)
                        {
                            SetupTurn(1);
                        }
                        else
                        {
                            SetupTurn(0);
                        }

                        //Surrender held marbles to the opponent
                        if (marblesHand[0].Count > 0)
                        {
                            slots[7].StoreMarbles(marblesHand[0]);
                            marblesHand[0] = new List<GameObject>();
                        }
                    }
                    break;
                case GameState.P2Turn:
                    //Turn Done checker
                    if (turnDone[1])
                    {
                        if (PlayerValidSlotsNumber(0) > 0)
                        {
                            SetupTurn(0);
                        }
                        else
                        {
                            SetupTurn(1);
                        }
                        if (marblesHand[1].Count > 0)
                        {
                            slots[15].StoreMarbles(marblesHand[1]);
                            marblesHand[1] = new List<GameObject>();
                        }
                    }
                    break;
                case GameState.GameEnd:
                    if (touches.Length > 0)
                    {
                        if (roundsToWin > 0)
                        {
                            if (wins[0] == roundsToWin || wins[1] == roundsToWin)
                            {
                                turn = GameState.ResultScreen;
                                endGameScreen.EnableCanvas(true);
                            }
                            else
                            {
                                ResetMarbles();
                            }
                        }
                        else
                        {
                            ResetMarbles();
                        }
                    }
                    break;
            }

            //Game End
            if (turn != GameState.ResultScreen && turn != GameState.GameEnd && PlayerValidSlotsNumber(0) == 0 && PlayerValidSlotsNumber(1) == 0 && marblesHand[0].Count == 0 && marblesHand[1].Count == 0)
            {
                turn = GameState.GameEnd;
                if (slots[7].MarbleAmount() > slots[15].MarbleAmount())
                {
                    wins[1]++;
                    winner = 1;
                }
                else if (slots[7].MarbleAmount() < slots[15].MarbleAmount())
                {
                    wins[0]++;
                    winner = 0;
                }
                winsText.text = wins[0] + "-" + wins[1];
            }

            if (turnDone[0])
            {
                ffButton.interactable = true;
            } else
            {
                ffButton.interactable = false;
            }
        }

    }

    public void FastForwardAI()
    {
        if (ffButton.interactable)
        {
            waitTime = 0.1f;
            if (aiThinking > waitTime)
            {
                aiThinking = waitTime;
            }
        }
    }

    protected override void InstructionDisplay()
    {
        switch (turn)
        {
            case GameState.PickingStartSlot:
                texts[0].GetComponent<Text>().text = "Choose Starting House";
                break;
            case GameState.BothTurns:
                texts[0].GetComponent<Text>().text = "Go!";
                break;
            case GameState.P1Turn:
                texts[0].GetComponent<Text>().text = "Your Turn";
                break;
            case GameState.P2Turn:
                texts[0].GetComponent<Text>().text = "";
                break;
            case GameState.GameEnd:
                int am1 = slots[15].GetComponent<Slot>().MarbleAmount();
                int am2 = slots[7].GetComponent<Slot>().MarbleAmount();
                if (am1 > am2)
                {
                    texts[0].GetComponent<Text>().text = "You Win";
                }
                else if (am2 > am1)
                {
                    texts[0].GetComponent<Text>().text = "You Lose";
                }
                else
                {
                    texts[0].GetComponent<Text>().text = "Draw";
                }
                break;
            case GameState.Paused:
                texts[0].GetComponent<Text>().text = "Paused";
                break;
        }
    }

    protected override void UISetup()
    {
        if (roundsToWin > 1)
        {
            winsText.enabled = true;
        }
        else
        {
            winsText.enabled = false;
        }

        if (timePerTurn > 0)
        {
            timeBar[0].enabled = true;
        }
        else
        {
            timeBar[0].enabled = false;
        }

        UpdateSlotSprite();
    }

    protected override void ProgressTime()
    {
        if (timePerTurn > 0)
        {
            switch (turn)
            {
                case GameState.BothTurns:
                    timeRemaining[0] = Mathf.Max(timeRemaining[0] - Time.deltaTime, 0);
                    if (timeRemaining[0] <= 0)
                    {
                        turnDone[0] = true;
                        nextSlot[0] = -2;
                    }
                    break;
                case GameState.P1Turn:
                    timeRemaining[0] -= Time.deltaTime;
                    if (timeRemaining[0] <= 0)
                    {
                        turnDone[0] = true;
                        nextSlot[0] = -2;
                    }
                    break;
            }

            timeBar[0].transform.localScale = new Vector3(timeRemaining[0] / timePerTurn, 1, 1);
        }
    }

    public void ResetAISpeed()
    {
        waitTime = ai.WaitTime();
    }
}
