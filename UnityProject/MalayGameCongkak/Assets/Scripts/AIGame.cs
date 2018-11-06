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
    private AI ai;
    private float aiThinking = 1;

    // Use this for initialization
    void Start () {
        //Singleton fucntion
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        startStyle = StartStyle.together;
        gameMode = GameMode.bestOf;
        miscNum = 2;
        marblePerSlot = 7;

        //Create AISystem
        ai = new AI(difficulty);
        aiThinking = ai.WaitTime();

        //Creating marbles
        for (int i = 0; i < 16; i++)
        {
            if (i != 7 && i != 15)
            {
                for (var j = 0; j < marblePerSlot; j++)
                {
                    GameObject marble = Instantiate(prefabMarble);
                    marble.transform.position = handObject[Mathf.RoundToInt(Random.value)].transform.position + new Vector3(Random.value, Random.value, 0);
                    slots[i].StoreMarbles(marble);
                }
            }
        }

        //Start Style
        SetupTurn((int)startStyle);

        //Game Mode UI
        switch (gameMode)
        {
            case GameMode.bestOf:
                winsText.enabled = true;
                break;
            case GameMode.burningVillages:
                break;
            case GameMode.speedCongkak:
                timeBar[0].enabled = true;
                timeBar[1].enabled = true;
                break;
        }

        UpdateSlotSprite();
    }
	
	// Update is called once per frame
	void Update ()
    {
        MoveHandMarbles();

        if (turn != GameState.Paused)
        {
            UpdateSlotSprite();
            ProgressTime();
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
                aiThinking = ai.WaitTime();
            }

            if (turn != GameState.Paused)
            {
                aiThinking -= Time.deltaTime;
            }

            //Player's Turn
            Touch[] touches = new Touch[1];
            if (Input.GetMouseButtonDown(0))
            {
                touches[0] = new Touch();
                touches[0].phase = TouchPhase.Began;
                touches[0].position = Input.mousePosition;
            }
            foreach (Touch i in touches)
            {
                if (i.phase == TouchPhase.Began)
                {
                    //Check if game is over
                    if (turn == GameState.GameEnd)
                    {
                        switch (gameMode)
                        {
                            case GameMode.bestOf:
                                int requiredWins = Mathf.CeilToInt((1.0f + miscNum * 2.0f) / 2.0f);
                                if (wins[0] >= requiredWins || wins[1] >= requiredWins)
                                {
                                    SceneManager.LoadScene(mainMenuName);
                                }
                                else
                                {
                                    ResetMarbles();
                                }
                                break;
                            case GameMode.burningVillages:
                                CheckForBurned();

                                if (noGoSlots[0].Count - 1 > miscNum + 1)
                                {
                                    SceneManager.LoadScene(mainMenuName);
                                }
                                break;
                            case GameMode.speedCongkak:
                                SceneManager.LoadScene(mainMenuName);
                                break;
                        }

                        SetupTurn((int)startStyle);
                        UpdateSlotSprite();
                    }
                    else
                    {
                        Slot slot = FindSlotOnTouch(i);
                        if (slot != null)
                        {
                            switch (turn)
                            {
                                case GameState.PickingStartSlot:
                                    //Select start
                                    if (slot.slotID >= 0 && slot.slotID <= 6)
                                    {
                                        nextSlot[0] = slot.slotID;
                                        popSource.PlayOneShot(pop);
                                    }

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
                                    break;
                                case GameState.BothTurns:
                                    //First Turn
                                    PlayerTurn(0, slot);
                                    break;
                                case GameState.P1Turn:
                                    //P1 Turn
                                    PlayerTurn(0, slot);
                                    break;
                            }
                        }
                    }
                }

                //Game End
                if (turn != GameState.GameEnd && PlayerValidSlotsNumber(0) == 0 && PlayerValidSlotsNumber(1) == 0 && marblesHand[0].Count == 0 && marblesHand[1].Count == 0)
                {
                    turn = GameState.GameEnd;
                    if (gameMode == GameMode.bestOf)
                    {
                        if (slots[7].MarbleAmount() > slots[15].MarbleAmount())
                        {
                            wins[1]++;
                        }
                        else if (slots[7].MarbleAmount() < slots[15].MarbleAmount())
                        {
                            wins[0]++;
                        }
                        winsText.text = wins[0] + "-" + wins[1];
                    }
                }
            }

            //Turn Done Detector
            switch (turn)
            {
                case GameState.BothTurns:
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

                    //Setup Next TUrn
                    if (turnDone[0] && turnDone[1])
                    {
                        turn = (GameState)((int)GameState.P1Turn + 1);
                        SetupTurn(doneFirst);
                    }
                    break;
                case GameState.P1Turn:
                    if (turnDone[0])
                    {
                        if (PlayerValidSlotsNumber(1) > 0)
                        {
                            SetupTurn(1);
                        }
                        else
                        {
                            SetupTurn(0);
                        }
                    }
                    break;
                case GameState.P2Turn:
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
                    }
                    break;
            }
        }

    }
}
