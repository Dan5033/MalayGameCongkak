using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialGame : AIGame {

    private int sequence = 0;
    private bool textShown = false;

    [Header("Text System")]
    private bool skip = false;
    [SerializeField] private RectTransform textBox;
    [SerializeField] private RectTransform textPotrait;
    [SerializeField] private Text text;

    [SerializeField] private SpriteRenderer blackScreen;
    [SerializeField] private SpriteMask maskVillage;
    [SerializeField] private SpriteMask maskStore;
    [SerializeField] private SpriteMask maskTarget1;
    [SerializeField] private SpriteMask maskTarget2;

    private Vector3 potraitDest;
    private Vector3 boxDest;
    private int pointer = 0;

    private Coroutine boxHide;
    private Coroutine potraitHide;

    /*
     *  0. So you really want to learn how to play congkak? Very well...
     *  1. These are called villages, they usually host 7 meeples
     *  2. Your objective is to put as many meeple into your storehouse as you can
     *  3. At the start of the game, you pick a village you want to start at. Let's pick this village.
     *  4. Now, place a meeple into the next villages counter clockwise. I'll highlight which village is next.
     *  5. Well look at that, the last meeple went into your storehouse. This means you get another turn. Lets take this one instead.
     *  6. Now your last meeple landed in an occupied village. This means you take all of the meeple there and continue.
     *  7. Oh no. You landed in my empty village, this means your turn ends here. My turn.
     *  8. Well, looks like in landed in my own empty village, that means I get to attack village and take your meeples.
     *  9. That's it. If you're ready to play against me for real, go to the Versus mode.
     *  10. I'll be waiting
     */

    private new void Start ()
    {
        master = Masters.Free;

        base.Start();

        //Make Fast Forward Unusable
        ffButton.interactable = false;

        //get destination postion
        potraitDest = textPotrait.anchoredPosition;
        boxDest = textBox.anchoredPosition;

        //Send text objects out screen
        textPotrait.anchoredPosition = potraitDest + new Vector3(0, 10000, 0);
        textBox.anchoredPosition = boxDest + new Vector3(0, 10000, 0);

        nextSlot = new int[] { -2, -2 };
        UpdateSlotSprite();

        string[] display =
        {
            "So you really want to learn how to play congkak?",
            " Very well...",
            "These are called villages, they usually host 7 meeples"
        };
        StartCoroutine(TextBoxEnter(display, 0.01f));
    }
	
	void Update ()
    {
        //Line up marbles in hands
        MoveHandMarbles();

        if (Input.GetMouseButtonDown(0))
        {
            skip = true;
        } else
        {
            skip = false;
        }

        if (turn != GameState.Paused && !textShown)
        {
            string[] display;
            Touch[] touches;
            switch (sequence)
            {
                case 0:
                    StartCoroutine(Highlight(maskVillage, 2));
                    sequence++;
                    break;
                case 1:
                    display = new string[]
                    {
                        "Your objective is to put as many meeple into your storehouse as you can"
                    };
                    StartCoroutine(TextBoxEnter(display, 0.01f));
                    sequence++;
                    break;
                case 2:
                    StartCoroutine(Highlight(maskStore, 2));
                    sequence++;
                    break;
                case 3:
                    display = new string[]
                    {
                        "At the start of the game, you pick a village you want to start at.",
                        " Let's pick this village."
                    };
                    StartCoroutine(TextBoxEnter(display, 0.01f));
                    sequence++;

                    SetupTurn((int) StartStyle.P1);
                    UpdateSlotSprite();
                    break;
                case 4:
                    StartCoroutine(Highlight(maskTarget1, 2));
                    sequence++;
                    break;
                case 5:
                    //Do every step
                    UpdateSlotSprite();

                    //Text Display
                    InstructionDisplay();

                    //Click Input
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
                        case GameState.P1Turn:
                            foreach (Touch i in touches)
                            {
                                Slot slot = FindSlotOnTouch(i);
                                if (slot != null)
                                {
                                    if (slot.slotID == 6)
                                    {
                                        //P1 Turn
                                        PlayerTurn(0, slot);
                                        sequence = 7;
                                    } else
                                    {
                                        sequence = 6;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                case 6:
                    //Wrong slot
                    display = new string[]
                    {
                        "Nope! Not that one.",
                        "Pick this one."
                    };
                    StartCoroutine(TextBoxEnter(display, 0.01f));
                    sequence = 4;
                    break;
                case 7:
                    display = new string[]
                    {
                        "Now, place a meeple into the next villages counter clockwise.",
                        " I'll highlight which village is next."
                    };
                    StartCoroutine(TextBoxEnter(display, 0.01f));
                    sequence++;
                    break;
                case 8:
                    //Do every step
                    UpdateSlotSprite();

                    //Text Display
                    InstructionDisplay();

                    //Click Input
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
                        case GameState.P1Turn:
                            foreach (Touch i in touches)
                            {
                                Slot slot = FindSlotOnTouch(i);
                                if (slot != null)
                                {
                                    //P1 Turn
                                    PlayerTurn(0, slot);

                                    if (slot.slotID == 15 && marblesHand[0].Count == 0)
                                    {
                                        sequence++;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                case 9:
                    display = new string[]
                    {
                        "Well look at that, the last meeple went into your storehouse.",
                        "This means you get another turn.",
                        "Lets take this one this time."
                    };
                    StartCoroutine(TextBoxEnter(display, 0.01f));
                    sequence++;
                    break;
                case 10:
                    StartCoroutine(Highlight(maskTarget2, 2));
                    sequence++;
                    break;
                case 11:
                    //Do every step
                    UpdateSlotSprite();

                    //Text Display
                    InstructionDisplay();

                    //Click Input
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
                        case GameState.P1Turn:
                            foreach (Touch i in touches)
                            {
                                Slot slot = FindSlotOnTouch(i);
                                if (slot != null)
                                {
                                    if (slot.slotID != 5 && marblesHand[0].Count == 0)
                                    {
                                        sequence++;
                                    } else
                                    {

                                        if (marblesHand[0].Count == 1)
                                        {
                                            sequence = 13;
                                        }
                                        //P1 Turn
                                        PlayerTurn(0, slot);
                                    }
                                }
                            }
                            break;
                    }
                    break;
                case 12:
                    //Wrong slot
                    display = new string[]
                    {
                        "Not the right one.",
                        "Pick this one."
                    };
                    StartCoroutine(TextBoxEnter(display, 0.01f));
                    sequence = 10;
                    break;
                case 13:
                    display = new string[]
                    {
                        "Your last meeple landed in an occupied village.",
                        "This means you take all of the meeple there and continue."
                    };
                    StartCoroutine(TextBoxEnter(display, 0.01f));
                    sequence++;
                    break;
                case 14:
                    //Do every step
                    UpdateSlotSprite();

                    //Text Display
                    InstructionDisplay();

                    //Click Input
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
                        case GameState.P1Turn:
                            foreach (Touch i in touches)
                            {
                                Slot slot = FindSlotOnTouch(i);
                                if (slot != null)
                                {
                                    //P1 Turn
                                    PlayerTurn(0, slot);

                                    //Destroy scripted hole
                                    if (slot.slotID == 0)
                                    {
                                        slots[10].SurrenderMarbles(1);
                                        foreach (GameObject x in marblesHand[1])
                                        {
                                            Destroy(x);
                                        }
                                        marblesHand[1] = new List<GameObject>();
                                    }

                                    if (marblesHand[0].Count == 0)
                                    {
                                        sequence = 15;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                case 15:
                    display = new string[]
                    {
                        "Oh no! Your last meeple landed in my empty village",
                        "This means your turn ends here.",
                        "My turn."
                    };
                    StartCoroutine(TextBoxEnter(display, 0.01f));
                    sequence++;

                    SetupTurn(1);
                    break;
                case 16:
                    ffButton.interactable = true;

                    //Do every step
                    UpdateSlotSprite();

                    //Text Display
                    InstructionDisplay();

                    switch (turn)
                    {
                        case GameState.P2Turn:
                            if (aiThinking <= 0)
                            {
                                if (nextSlot[1] == -1)
                                {
                                    PlayerTurn(1, slots[14]);
                                }
                                else if (nextSlot[1] > -1)
                                {
                                    PlayerTurn(1, slots[nextSlot[1]]);
                                }

                                if (nextSlot[1] == -2)
                                {
                                    SetupTurn(0);
                                    sequence++;
                                }

                                aiThinking = waitTime;
                            } else
                            {
                                aiThinking -= Time.deltaTime;
                            }
                            break;
                    }
                    break;
                case 17:
                    display = new string[]
                    {
                        "Well...",
                        "My last meeple landed in my own empty village",
                        "This means I get to attack your village by taking meeples in the opposite village.",
                        "You can also do this if you landed in your own empty village.",
                        "Your Turn."
                    };
                    StartCoroutine(TextBoxEnter(display, 0.01f));
                    SetupTurn(0);
                    sequence++;
                    break;
                case 18:
                    //Do every step
                    UpdateSlotSprite();

                    //Text Display
                    InstructionDisplay();

                    //Click Input
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
                        case GameState.P1Turn:
                            foreach (Touch i in touches)
                            {
                                Slot slot = FindSlotOnTouch(i);
                                if (slot != null)
                                {
                                    //P1 Turn
                                    PlayerTurn(0, slot);

                                    if (nextSlot[0] == -2)
                                    {
                                        SetupTurn(1);
                                        sequence++;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                case 19:
                    display = new string[]
                    {
                        "Nice one.",
                        "This will go on until all of the villages are empty.",
                        "When that happens, the one with the most meeples wins!",
                        "My Turn."
                    };
                    StartCoroutine(TextBoxEnter(display, 0.01f));
                    SetupTurn(1);
                    sequence++;
                    break;
                case 20:
                    if (aiThinking <= 0)
                    {
                        if (nextSlot[1] == -1)
                        {
                            int selected = 8;
                            while (slots[selected].MarbleAmount() == 0)
                            {
                                selected++;
                                if (selected == 15)
                                {
                                    selected = 8;
                                }
                            }
                            PlayerTurn(1, slots[selected]);
                        }
                        else if (nextSlot[1] > -1)
                        {
                            PlayerTurn(1, slots[nextSlot[1]]);
                        }
                        if (nextSlot[1] == -2)
                        {
                            SetupTurn(0);
                            sequence++;
                        }

                        aiThinking = waitTime;
                    }
                    else
                    {
                        aiThinking -= Time.deltaTime;
                    }
                    break;
                case 21:
                    display = new string[]
                    {
                        "Games are usually played in Best of 3 format.",
                        "And the board is reset in between each games.",
                        "Except...",
                        "If you play with Burn Rules.",
                        "That means the meeple in your storehouse will only fill your own village",
                        "Any empty village after that will be Burned.",
                        "That means both sides won't be able to place any meeple in it."
                    };
                    StartCoroutine(TextBoxEnter(display, 0.01f));
                    SetupTurn(0);
                    sequence++;
                    break;
                case 22:
                    //Do every step
                    UpdateSlotSprite();

                    //Text Display
                    InstructionDisplay();

                    //Click Input
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
                        case GameState.P1Turn:
                            foreach (Touch i in touches)
                            {
                                Slot slot = FindSlotOnTouch(i);
                                if (slot != null)
                                {
                                    //P1 Turn
                                    PlayerTurn(0, slot);

                                    if (nextSlot[0] == -2)
                                    {
                                        SetupTurn(1);
                                        sequence++;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                case 23:
                    display = new string[]
                    {
                        "And I think that's all the rules.",
                        "I'll be here if you need a revision.",
                        "But if you want to play against me for real...",
                        "I'll be waiting for you in Versus Mode."
                    };
                    StartCoroutine(TextBoxEnter(display, 0.01f));
                    sequence++;
                    break;
                case 24:
                    GPGSHandler.instance.UnlockAchievement(GPGSIds.achievement_congkak_student);
                    JSONSaveData.currentSave.tutorialCompleted = true;
                    JSONSaveData.SaveGame();
                    SceneManager.LoadScene("Mode1P");
                    break;
            }
        }
	}

    IEnumerator TextBoxEnter(string[] text, float delay)
    {
        textShown = true;

        //Reset Text Display
        this.text.text = "";

        if (potraitHide != null)
        {
            StopCoroutine(potraitHide);
        }

        if (boxHide != null)
        {
            StopCoroutine(boxHide);
        }

        //Move graphics into position
        if (skip)
        {
            textPotrait.anchoredPosition = potraitDest;
            textBox.anchoredPosition = boxDest;
            skip = false;
        } else
        {
            StartCoroutine(ObjectEnter(textPotrait, potraitDest));
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(ObjectEnter(textBox, boxDest));
            yield return new WaitForSeconds(0.5f);
        }

        //Pragraph display
        for (int i = 0; i < text.Length; i++)
        {
            //text display
            while (pointer < text[i].Length)
            {
                if (skip)
                {
                    pointer = text[i].Length;
                    skip = false;
                } else
                {
                    pointer++;
                }
                this.text.text = text[i].Substring(0, pointer);
                yield return new WaitForSeconds(delay);
            }

            while (!skip)
            {
                yield return null;
            }
            skip = false;
            pointer = 0;
        }

        //Hide graphics
        potraitHide = StartCoroutine(ObjectEnter(textPotrait, potraitDest + new Vector3(0, 10000, 0)));
        boxHide = StartCoroutine(ObjectEnter(textBox, boxDest + new Vector3(0, 10000, 0)));

        textShown = false;
    }

    IEnumerator ObjectEnter(RectTransform item, Vector3 dest)
    {
        while (Vector3.Distance(item.anchoredPosition,dest) > 10)
        {
            item.anchoredPosition = Vector3.Lerp(item.anchoredPosition, dest,0.1f);
            yield return null;
        }
    }

    IEnumerator Highlight(SpriteMask mask, float seconds)
    {
        textShown = true;

        //Enable mask
        mask.enabled = true;

        //Fade in
        float alpha = 0;
        while (alpha < 0.8f)
        {
            alpha = Mathf.Lerp(alpha, 1, 0.1f);
            Color color = blackScreen.color;
            color.a = alpha;
            blackScreen.color = color;
            yield return null;
        }
        //Wait
        float time = 0;
        while (time < seconds)
        {
            time += Time.deltaTime;

            if (skip)
            {
                time = seconds;
                skip = false;
            }
            yield return null;
        }
        //Fade out
        while (alpha > 0.01f)
        {
            alpha = Mathf.Lerp(alpha, -0.2f, 0.1f);
            Color color = blackScreen.color;
            color.a = alpha;
            blackScreen.color = color;
            yield return null;
        }

        //Disable Mask
        mask.enabled = false;

        textShown = false;
    }
}
