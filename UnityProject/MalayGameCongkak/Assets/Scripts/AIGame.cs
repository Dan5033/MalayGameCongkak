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
    public static Masters master = Masters.Free;
    protected AI ai;
    protected float aiThinking = 1;
    protected float waitTime = 0;
    protected float mood = 0;

    [Header("Chatter")]
    [SerializeField] private Image box;
    [SerializeField] private Text quotes;
    [SerializeField] private Image potrait;
    [SerializeField] private GameObject mask;
    private Coroutine runningText;

    [SerializeField] protected Button ffButton;

    new protected void Start ()
    {
        //Create AISystem
        ai = new AI(difficulty,noGoSlots[1]);
        aiThinking = ai.WaitTime();
        waitTime = ai.WaitTime();

        //Game Setup
        base.Start();

        //UI Setup
        if (master == Masters.Free)
        {
            if (box != null)
            {
                box.enabled = false;
                quotes.enabled = false;
                mask.SetActive(false);
            }
        } else
        {
            potrait.sprite = masterPix[(int)master];
        }

        runningText = StartCoroutine(TextBoxDisplay(GenerateQuote(master,Situation.GameStart), 0.01f));
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
                            mood = mood - Mathf.Sign(mood);
                        }
                        break;
                    case GameState.BothTurns:
                    case GameState.P2Turn:
                        if (nextSlot[1] == -1)
                        {
                            int difference = slots[15].MarbleAmount() - slots[7].MarbleAmount();

                            ai.UpdateWorld(slots);
                            int tgt = ai.NextMove();
                            PlayerTurn(1, slots[tgt]);

                            if (difference > 0 && slots[15].MarbleAmount() - slots[7].MarbleAmount() < 0)
                            {
                                runningText = StartCoroutine(TextBoxDisplay(GenerateQuote(master, Situation.Turnabout), 0.01f));
                            } else if (Random.value > 0.01f)
                            {
                                runningText = StartCoroutine(TextBoxDisplay(GenerateQuote(master, Situation.MyMove), 0.01f));
                            }
                        } else
                        {
                            PlayerTurn(1, slots[nextSlot[1]]);
                        }
                        break;
                }
                aiThinking = waitTime;
            }

            //Quotes
            if (slots[15].MarbleAmount() - slots[7].MarbleAmount() > 20 && mood >= 0)
            {
                mood = -10;
                runningText = StartCoroutine(TextBoxDisplay(GenerateQuote(master, Situation.Losing), 0.01f));
            } else if (slots[15].MarbleAmount() - slots[7].MarbleAmount() < -20 && mood <= 0)
            {
                mood = 10;
                runningText = StartCoroutine(TextBoxDisplay(GenerateQuote(master, Situation.Winning), 0.01f));
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
                            if (Random.value > 0.5 && nextSlot[0] == -1)
                            {
                                runningText = StartCoroutine(TextBoxDisplay(GenerateQuote(master, Situation.YourMove), 0.01f));
                            }
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
                            runningText = StartCoroutine(TextBoxDisplay(GenerateQuote(master, Situation.MyTurn), 0.01f));
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
                            runningText = StartCoroutine(TextBoxDisplay(GenerateQuote(master, Situation.YourTurn), 0.01f));
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
                                if (wins[0] > wins[1])
                                {
                                    if (master == Masters.Free)
                                    {
                                        endGameScreen.EnableCanvas(true, GenerateTips(), PickDefeatedPotraits());
                                    } else
                                    {
                                        endGameScreen.EnableCanvas(true, GenerateQuote(master, Situation.GameLost), masterPix[(int)master]);
                                    }
                                    if (master != Masters.Free)
                                    {
                                        JSONSaveData.currentSave.defeated[(int)master] = true;
                                        JSONSaveData.SaveGame();
                                    }
                                } else
                                {
                                    if (master == Masters.Free)
                                    {
                                        endGameScreen.EnableCanvas(true, GenerateTips(), PickDefeatedPotraits());
                                    }
                                    else
                                    {
                                        endGameScreen.EnableCanvas(true, GenerateQuote(master, Situation.GameWon), masterPix[(int)master]);
                                    }
                                }
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
                    runningText = StartCoroutine(TextBoxDisplay(GenerateQuote(master, Situation.RoundWon), 0.01f));
                }
                else if (slots[7].MarbleAmount() < slots[15].MarbleAmount())
                {
                    wins[0]++;
                    winner = 0;
                    runningText = StartCoroutine(TextBoxDisplay(GenerateQuote(master, Situation.RoundLost), 0.01f));
                }
                winsText.text = wins[0] + "-" + wins[1];
                AdManager.instance.ShowInterstitial();
            }

            //FF Button
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

    IEnumerator TextBoxDisplay(string text, float delay)
    {
        if (master != Masters.Free && text != quotes.text)
        {
            int pointer = 0;

            while (pointer < text.Length)
            {
                pointer++;

                quotes.text = text.Substring(0, pointer);
                yield return new WaitForSeconds(delay);
            }
        }
    }

    public enum Situation
    {
        GameStart,
        YourMove,
        MyMove,
        YourTurn,
        MyTurn,
        Losing,
        Turnabout,
        Winning,
        RoundWon,
        RoundLost,
        GameWon,
        GameLost
    }

    public string GenerateQuote(Masters master, Situation sit)
    {
        string result = "[This is a base Quote]";

        switch (master)
        {
            case Masters.Safiya:
                switch (sit)
                {
                    case Situation.GameStart:
                        result = "You're here! Let's do this...";
                        break;
                    case Situation.YourMove:
                        result = "Not a bad move.";
                        break;
                    case Situation.MyMove:
                        result = "Let's see if you can beat this.";
                        break;
                    case Situation.YourTurn:
                        result = "Go ahead.";
                        break;
                    case Situation.MyTurn:
                        result = "Hmmm...";
                        break;
                    case Situation.Losing:
                        result = "Hohoho... You're really getting ahead.";
                        break;
                    case Situation.Turnabout:
                        result = "Haha! Finally... I caught up.";
                        break;
                    case Situation.Winning:
                        result = "C'mon! You can do it.";
                        break;
                    case Situation.RoundWon:
                        result = "Phew... Good round.";
                        break;
                    case Situation.RoundLost:
                        result = "I won't lose next time.";
                        break;
                    case Situation.GameWon:
                        result = "Well played. Come back again when you're ready.";
                        break;
                    case Situation.GameLost:
                        result = "The student finally beats the teacher. Well played.";
                        break;
                }
                break;
            case Masters.Lee:
                switch (sit)
                {
                    case Situation.GameStart:
                        result = "So you're the new kid, huh?";
                        break;
                    case Situation.YourMove:
                        result = "Interesting...";
                        break;
                    case Situation.MyMove:
                        result = "Speed is everything";
                        break;
                    case Situation.YourTurn:
                        result = "Be quick about it";
                        break;
                    case Situation.MyTurn:
                        result = "...";
                        break;
                    case Situation.Losing:
                        result = "Grr... Not yet.";
                        break;
                    case Situation.Turnabout:
                        result = "Looks like the table has turned";
                        break;
                    case Situation.Winning:
                        result = "Don't be the hare... C'mon, Lee!";
                        break;
                    case Situation.RoundWon:
                        result = "Looks like my speed trumps you. This time.";
                        break;
                    case Situation.RoundLost:
                        result = "I need to be faster.";
                        break;
                    case Situation.GameWon:
                        result = "HAH! You gotta be faster, kiddo.";
                        break;
                    case Situation.GameLost:
                        result = "Argh... My speed... failed me? Impossible.";
                        break;
                }
                break;
            case Masters.Murugam:
                switch (sit)
                {
                    case Situation.GameStart:
                        result = "We might be here a while. Did you bring food?";
                        break;
                    case Situation.YourMove:
                        result = "(munch munch munch)";
                        break;
                    case Situation.MyMove:
                        result = "I'll do this.";
                        break;
                    case Situation.YourTurn:
                        result = "Go on. I'll just have a drink.";
                        break;
                    case Situation.MyTurn:
                        result = "I'll take my time. Go grab a bite.";
                        break;
                    case Situation.Losing:
                        result = "Someone hold my muruku.";
                        break;
                    case Situation.Turnabout:
                        result = "You can do this, Murugam.";
                        break;
                    case Situation.Winning:
                        result = "I guess I can grab lunch first?";
                        break;
                    case Situation.RoundWon:
                        result = "Time for a break. Tea?";
                        break;
                    case Situation.RoundLost:
                        result = "Hmm... I need tea to think.";
                        break;
                    case Situation.GameWon:
                        result = "Good Game! Come and have dinner together!";
                        break;
                    case Situation.GameLost:
                        result = "Hmm... I think I ate too much. (burp)";
                        break;
                }
                break;
            case Masters.Kamal:
                switch (sit)
                {
                    case Situation.GameStart:
                        result = "So you're the one that beaten Safiya?";
                        break;
                    case Situation.YourMove:
                        result = "Hmm... I see.";
                        break;
                    case Situation.MyMove:
                        result = "Your defeat is ensured.";
                        break;
                    case Situation.YourTurn:
                        result = "I'm waiting...";
                        break;
                    case Situation.MyTurn:
                        result = "Maybe there's opening.. no? Hmm...";
                        break;
                    case Situation.Losing:
                        result = "Geh... I can't embarass our family.";
                        break;
                    case Situation.Turnabout:
                        result = "Don't get cocky, Kamal.";
                        break;
                    case Situation.Winning:
                        result = "(Scanning the board vigorously)";
                        break;
                    case Situation.RoundWon:
                        result = "Phew... Good round.";
                        break;
                    case Situation.RoundLost:
                        result = "Hmm... Give me a second to analyze this.";
                        break;
                    case Situation.GameWon:
                        result = "You've got ways ahead of you. Don't give up.";
                        break;
                    case Situation.GameLost:
                        result = "Wow... You really are good. No wonder Safiya lost.";
                        break;
                }
                break;
            case Masters.Eric:
                switch (sit)
                {
                    case Situation.GameStart:
                        result = "I'll go first okay?";
                        break;
                    case Situation.YourMove:
                        result = "Interesting...";
                        break;
                    case Situation.MyMove:
                        result = "This should be okay.";
                        break;
                    case Situation.YourTurn:
                        result = "I guess I'll allow you to make a turn.";
                        break;
                    case Situation.MyTurn:
                        result = "Take five. This is going to be a while.";
                        break;
                    case Situation.Losing:
                        result = "Did I move first? There must be a mistake";
                        break;
                    case Situation.Turnabout:
                        result = "Don't give an inch, Eric.";
                        break;
                    case Situation.Winning:
                        result = "Looks like I'll go first again.";
                        break;
                    case Situation.RoundWon:
                        result = "Who strikes first, wins.";
                        break;
                    case Situation.RoundLost:
                        result = "This can't be right.";
                        break;
                    case Situation.GameWon:
                        result = "You did good, kid.";
                        break;
                    case Situation.GameLost:
                        result = "Hmm... You aren't half bad. Respect.";
                        break;
                }
                break;
            case Masters.Esther:
                switch (sit)
                {
                    case Situation.GameStart:
                        result = "Ugh.. I don't have time for this.";
                        break;
                    case Situation.YourMove:
                        result = "Really?";
                        break;
                    case Situation.MyMove:
                        result = "This.. and done.";
                        break;
                    case Situation.YourTurn:
                        result = "Just be quick about it.";
                        break;
                    case Situation.MyTurn:
                        result = "Can't I just win now?";
                        break;
                    case Situation.Losing:
                        result = "You sure you aren't cheating, kid?";
                        break;
                    case Situation.Turnabout:
                        result = "Who's the best? Esther that's who.";
                        break;
                    case Situation.Winning:
                        result = "I guess that's the best you can do.";
                        break;
                    case Situation.RoundWon:
                        result = "Are we done?";
                        break;
                    case Situation.RoundLost:
                        result = "Hey! Are you cheating?";
                        break;
                    case Situation.GameWon:
                        result = "You're 10 years too early, kiddo.";
                        break;
                    case Situation.GameLost:
                        result = "Ugh.. It's bit like I wanted to win anyways.";
                        break;
                }
                break;
            case Masters.TokSenah:
                switch (sit)
                {
                    case Situation.GameStart:
                        result = "Nice to meet you. Let's have fun.";
                        break;
                    case Situation.YourMove:
                        result = "Oh! Interesting...";
                        break;
                    case Situation.MyMove:
                        result = "Now watch carefully.";
                        break;
                    case Situation.YourTurn:
                        result = "Now what shall you do?";
                        break;
                    case Situation.MyTurn:
                        result = "Let's see...";
                        break;
                    case Situation.Losing:
                        result = "I'm pleasantly suprised, child.";
                        break;
                    case Situation.Turnabout:
                        result = "I guess that wouldn't last long.";
                        break;
                    case Situation.Winning:
                        result = "Should I go easy on you?";
                        break;
                    case Situation.RoundWon:
                        result = "Here's what I think you can do to improve...";
                        break;
                    case Situation.RoundLost:
                        result = "Oh wow... I haven't had a game this interesting in years.";
                        break;
                    case Situation.GameWon:
                        result = "Don't worry. Keep trying, alright?";
                        break;
                    case Situation.GameLost:
                        result = "I am stunned. You definitely deserve the title Master of Masters.";
                        break;
                }
                break;
            default:
                switch (sit)
                {
                    case Situation.GameStart:
                        result = "[Insert Greeting]";
                        break;
                    case Situation.YourMove:
                        result = "[Insert Praise]";
                        break;
                    case Situation.MyMove:
                        result = "[Insert Brag]";
                        break;
                    case Situation.YourTurn:
                        result = "[Insert Polite Gesture]";
                        break;
                    case Situation.MyTurn:
                        result = "[Insert Thinking]";
                        break;
                    case Situation.Losing:
                        result = "[Insert Anguish]";
                        break;
                    case Situation.Turnabout:
                        result = "[Insert Relief]";
                        break;
                    case Situation.Winning:
                        result = "[Insert Snarky Remark]";
                        break;
                    case Situation.RoundWon:
                        result = "[Insert Relax]";
                        break;
                    case Situation.RoundLost:
                        result = "[Insert Determination]";
                        break;
                    case Situation.GameWon:
                        result = "[Insert GG]";
                        break;
                    case Situation.GameLost:
                        result = "[Insert Sadness]";
                        break;
                }
                break;
        }

        return result;
    }

}
