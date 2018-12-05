using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum StartStyle
{
    P1,
    P2,
    together
}

public enum GameState
{
    PickingStartSlot,
    BothTurns,
    P1Turn,
    P2Turn,
    GameEnd,
    ResultScreen,
    Paused,
    Animating
}

public enum AfterStyle
{
    RoundWinner,
    RoundLoser,
    P1Start,
    P2Start,
    StartTogether
}

public class Game : MonoBehaviour {

    public static Game instance;

    //Game Settings    
    public static int marblePerSlot = 7;
    public static StartStyle startStyle = StartStyle.together;
    public static AfterStyle afterStyle = AfterStyle.StartTogether;
    public static int roundsToWin = 1;
    public static bool burntVillages = true;
    public static float timePerTurn = 0;

    [Header("Prefabs")]
    [SerializeField] protected GameObject prefabMarble;

    [Header("Preset Objects")]
    [SerializeField] protected Slot[] slots;
    [SerializeField] protected AudioController audioController;

    [Header("Slot Sprites")]
    [SerializeField] protected Sprite sprDef;
    [SerializeField] protected Sprite[] sprPlayer;
    [SerializeField] protected Sprite sprP01;

    [Header("Hand")]
    [SerializeField] protected float speed = 20;
    [SerializeField] protected GameObject[] handObject;
    protected List<GameObject>[] marblesHand = { new List<GameObject>(), new List<GameObject>() };

    [Header("Game")]
    protected int[] nextSlot = { -1, -1 };
    protected bool[] turnDone = { false, false };
    protected int doneFirst = -1;
    public GameState turn = GameState.PickingStartSlot;
    protected List<int>[] noGoSlots = { new List<int> { 7 }, new List<int> { 15 } };
    public int winner = -1;

    //game mode variables
    protected int[] wins = { 0, 0 };
    protected float[] timeRemaining = { 0, 0 };

    [Header("Interface")]
    [SerializeField] protected GameObject[] texts;
    [SerializeField] protected Text winsText;
    [SerializeField] protected Image[] timeBar;
    [SerializeField] protected GameObject fireAnimation;
    [SerializeField] protected EndGameScreen endGameScreen;
    
    [Header("Scenes")]
    [SerializeField] protected string mainMenuName;

	protected void Start ()
    {
        VideoAdManager.RequestInterstitial();

        //Singleton fucntion
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(this);
        }

        //Creating marbles
		for (int i = 0; i < 16; i++)
        {
            if (i != 7 && i != 15)
            {
                for (var j = 0; j < marblePerSlot; j++)
                {
                    GameObject marble = Instantiate(prefabMarble);
                    marble.transform.position = handObject[Mathf.RoundToInt(Random.value)].transform.position + new Vector3(Random.value,Random.value,0);
                    slots[i].StoreMarbles(marble);
                }
            }
        }

        //Start Style
        SetupTurn((int) startStyle);


        //Game Mode UI
        if (roundsToWin > 1)
        {
            winsText.enabled = true;
        } else
        {
            winsText.enabled = false;
        }

        if (timePerTurn > 0)
        {
            timeBar[0].enabled = true;
            timeBar[1].enabled = true;
        } else
        {
            timeBar[0].enabled = false;
            timeBar[1].enabled = false;
        }

        UpdateSlotSprite();
    }
	
	void Update ()
    {
        //Move marbles
        MoveHandMarbles();

        if (turn != GameState.Paused)
        {
            //Do every step
            UpdateSlotSprite();

            //Time
            ProgressTime();

            //Text Display
            InstructionDisplay();

            //Click Input
            Touch[] touches;
            if (Input.GetMouseButtonDown(0))
            {
                touches = new Touch[1];
                touches[0] = new Touch();
                touches[0].phase = TouchPhase.Began;
                touches[0].position = Input.mousePosition;
            } else
            {
                touches = Input.touches;
            }

            //Game Turn
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
                                audioController.PlaySoundEffect(Context.MarblePlace);
                            }
                            else if (slot.slotID >= 8 && slot.slotID < 15)
                            {
                                nextSlot[1] = slot.slotID;
                                audioController.PlaySoundEffect(Context.MarblePlace);
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
                    foreach (Touch i in touches)
                    {
                        Slot slot = FindSlotOnTouch(i);
                        if (slot != null)
                        {
                            //P2 Turn
                            PlayerTurn(1, slot);
                        }
                    }

                    //Turn down checker
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
                                endGameScreen.EnableCanvas(false);
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
                VideoAdManager.ShowAd();
            }
        }
    }

    protected void InstructionDisplay()
    {
        switch (turn)
        {
            case GameState.PickingStartSlot:
                texts[0].GetComponent<Text>().text = "Choose Starting Village";
                texts[1].GetComponent<Text>().text = "Choose Starting Village";
                break;
            case GameState.BothTurns:
                texts[0].GetComponent<Text>().text = "Go!";
                texts[1].GetComponent<Text>().text = "Go!";
                break;
            case GameState.P1Turn:
                texts[0].GetComponent<Text>().text = "Your Turn";
                texts[1].GetComponent<Text>().text = "";
                break;
            case GameState.P2Turn:
                texts[0].GetComponent<Text>().text = "";
                texts[1].GetComponent<Text>().text = "Your Turn";
                break;
            case GameState.GameEnd:
                int am1 = slots[15].GetComponent<Slot>().MarbleAmount();
                int am2 = slots[7].GetComponent<Slot>().MarbleAmount();
                if (am1 > am2)
                {
                    texts[0].GetComponent<Text>().text = "You Win";
                    texts[1].GetComponent<Text>().text = "You Lose";
                }
                else if (am2 > am1)
                {
                    texts[0].GetComponent<Text>().text = "You Lose";
                    texts[1].GetComponent<Text>().text = "You Win";
                }
                else
                {
                    texts[0].GetComponent<Text>().text = "Draw";
                    texts[1].GetComponent<Text>().text = "Draw";
                }
                break;
            case GameState.Paused:
                texts[0].GetComponent<Text>().text = "Paused";
                texts[1].GetComponent<Text>().text = "Paused";
                break;
            case GameState.Animating:
                texts[0].GetComponent<Text>().text = "";
                texts[1].GetComponent<Text>().text = "";
                break;
        }
    }

    protected void MoveHandMarbles()
    {
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < marblesHand[x].Count; y++)
            {
                GameObject i = marblesHand[x][y];
                Vector3 dest = new Vector3(i.transform.position.x, i.transform.position.y + 0.5f*y,0);
                i.GetComponent<Rigidbody2D>().velocity = (handObject[x].transform.position - dest) * Time.deltaTime * speed;
            }
        }
    }

    protected void ProgressTime()
    {
        if (timePerTurn > 0)
        {
            switch (turn)
            {
                case GameState.BothTurns:
                    timeRemaining[0] = Mathf.Max(timeRemaining[0] - Time.deltaTime , 0);
                    timeRemaining[1] = Mathf.Max(timeRemaining[1] - Time.deltaTime, 0);
                    if (timeRemaining[0] <= 0)
                    {
                        turnDone[0] = true;
                        nextSlot[0] = -2;
                    }
                    if (timeRemaining[1] <= 0)
                    {
                        turnDone[1] = true;
                        nextSlot[1] = -2;
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
                case GameState.P2Turn:
                    timeRemaining[1] -= Time.deltaTime;
                    if (timeRemaining[1] <= 0)
                    {
                        turnDone[1] = true;
                        nextSlot[1] = -2;
                    }
                    break;
            }

            timeBar[0].transform.localScale = new Vector3(timeRemaining[0] / timePerTurn, 1, 1);
            timeBar[1].transform.localScale = new Vector3(timeRemaining[1] / timePerTurn, 1, 1);
        }
    }

    protected void CheckForBurned()
    {
        slots[15].SurrenderMarbles(0);
        int target = 0;
        foreach (GameObject marble in marblesHand[0].ToArray())
        {
            if (slots[target].MarbleAmount() == marblePerSlot)
            {
                target++;
                if (target == 7)
                {
                    break;
                }
            }
            slots[target].StoreMarbles(marble);
            marblesHand[0].Remove(marble);
        }
        slots[15].StoreMarbles(marblesHand[0]);
        marblesHand[0] = new List<GameObject>();

        slots[7].SurrenderMarbles(1);
        target = 8;
        foreach (GameObject marble in marblesHand[1].ToArray())
        {
            if (slots[target].MarbleAmount() == marblePerSlot)
            {
                target++;
                if (target == 15)
                {
                    break;
                }
            }
            slots[target].StoreMarbles(marble);
            marblesHand[1].Remove(marble);
        }
        slots[7].StoreMarbles(marblesHand[1]);
        marblesHand[1] = new List<GameObject>();

        //Burning Villages
        noGoSlots[0] = new List<int> { 7 };
        noGoSlots[1] = new List<int> { 15 };
        foreach (Slot i in slots)
        {
            bool empty = i.MarbleAmount() <= 0;
            bool notHome = i.slotID != 7 && i.slotID != 15;
            if (empty && notHome)
            {
                noGoSlots[0].Add(i.slotID);
                noGoSlots[1].Add(i.slotID);
                GameObject fireObj = Instantiate(fireAnimation);
                fireObj.transform.position = i.transform.position; ;
                i.BurnSlot();
            } else
            {
                i.UnBurnSlot();
            }
        }
    }

    protected void ResetMarbles()
    {
        if (burntVillages)
        {
            CheckForBurned();
            
            if (noGoSlots[0].Count - 1 >= 7 || noGoSlots[1].Count - 1 >= 7)
            {
                ReturnToMainMenu();
            }
        } else
        {
            //Move all marbles to one list
            slots[15].SurrenderMarbles(0);
            slots[7].SurrenderMarbles(0);

            //Place all marble in their places
            int target = 0;
            foreach (GameObject marble in marblesHand[0].ToArray())
            {
                if (slots[target].MarbleAmount() == marblePerSlot)
                {
                    target++;
                    if (target == 7)
                    {
                        target++;
                    }
                    else if (target > 14)
                    {
                        break;
                    }
                }
                slots[target].StoreMarbles(marble);
            }
            marblesHand[0] = new List<GameObject>();
        }

        SetupTurn(afterStyle);
    }

    protected void SetupTurn(int player)
    {
        switch (player)
        {
            case 0:
                turnDone[0] = false;
                turn = GameState.P1Turn;
                nextSlot[0] = -1;
                nextSlot[1] = -2;
                if (timePerTurn > 0)
                {
                    timeRemaining[0] = timePerTurn;
                    timeRemaining[1] = 0;
                }
                break;
            case 1:
                turnDone[1] = false;
                turn = GameState.P2Turn;
                nextSlot[1] = -1;
                nextSlot[0] = -2;
                if (timePerTurn > 0)
                {
                    timeRemaining[0] = 0;
                    timeRemaining[1] = timePerTurn;
                }
                break;
            case 2:
                turnDone[0] = false;
                turnDone[1] = false;
                turn = GameState.PickingStartSlot;
                nextSlot[0] = -1;
                nextSlot[1] = -1;
                doneFirst = -1;
                if (timePerTurn > 0)
                {
                    timeRemaining[0] = timePerTurn;
                    timeRemaining[1] = timePerTurn;
                }
                break;
        }
    }

    protected void SetupTurn(AfterStyle style)
    {
        switch (style)
        {
            case AfterStyle.P1Start:
                SetupTurn(0);
                break;
            case AfterStyle.P2Start:
                SetupTurn(1);
                break;
            case AfterStyle.RoundLoser:
                if (winner == 0)
                {
                    SetupTurn(1);
                }
                else
                {
                    SetupTurn(0);
                }
                break;
            case AfterStyle.RoundWinner:
                if (winner == 0)
                {
                    SetupTurn(0);
                } else
                {
                    SetupTurn(1);
                }
                break;
            case AfterStyle.StartTogether:
                SetupTurn(2);
                break;
        }
    }

    protected int PlayerValidSlotsNumber(int player)
    {
        int amount = 0;

        for (int i = player*8; i < 7 + player*8; i++)
        {
            if (!noGoSlots[player].Contains(i) && slots[i].MarbleAmount() > 0)
            {
                amount++;
            }
        }

        return amount;
    }

    protected void PlayerTurn(int player, Slot slot)
    {
        if (!turnDone[player])
        {
            if (nextSlot[player] == -1)
            {
                //Pick any of the slots
                bool correctChoiceP0 = player == 0 && slot.slotID >= 0 && slot.slotID < 7 && slot.MarbleAmount() > 0;
                bool correctChoiceP1 = player == 1 && slot.slotID >= 8 && slot.slotID < 15 && slot.MarbleAmount() > 0;
                if (correctChoiceP0 || correctChoiceP1)
                {
                    slot.SurrenderMarbles(player);
                    nextSlot[player] = slot.slotID;
                    ProgressSlot(player);

                    if (timePerTurn > 0)
                    {
                        timeRemaining[player] = timePerTurn;
                    }

                    audioController.PlaySoundEffect(Context.MarblePlace);
                }
            }
            else if (nextSlot[player] > -1)
            {
                //Pick only the right slot
                if (nextSlot[player] == slot.slotID)
                {
                    //Move one marble
                    slot.StoreMarbles(marblesHand[player][0]);
                    marblesHand[player].RemoveAt(0);
                    ProgressSlot(player);

                    if (timePerTurn > 0)
                    {
                        timeRemaining[player] = timePerTurn;
                    }

                    audioController.PlaySoundEffect(Context.MarblePlace);

                    //Check if hand is empty
                    if (marblesHand[player].Count == 0)
                    {
                        //Select any when last marble is home
                        if ((player == 0 && slot.slotID == 15) || (player == 1 && slot.slotID == 7))
                        {
                            if (PlayerValidSlotsNumber(player) > 0)
                            {
                                nextSlot[player] = -1;
                            } else
                            {
                                turnDone[player] = true;
                            }
                        }
                        else if (slot.MarbleAmount() > 1)
                        {
                            //Pickup all in the surrent slot
                            slot.SurrenderMarbles(player);
                        }
                        else if (slot.MarbleAmount() == 1)
                        {
                            //Steal all
                            bool homeP0 = player == 0 && slot.slotID >= 0 && slot.slotID < 7;
                            bool homeP1 = player == 1 && slot.slotID >= 8 && slot.slotID < 15;

                            if (homeP1 || homeP0)
                            {
                                audioController.PlaySoundEffect(Context.HouseBomb);

                                slot.SurrenderMarbles(player);
                                slots[14 - slot.slotID].SurrenderMarbles(player);

                                if (homeP0)
                                {
                                    slots[15].StoreMarbles(marblesHand[player]);
                                    marblesHand[player] = new List<GameObject>();
                                }
                                else if (homeP1)
                                {
                                    slots[7].StoreMarbles(marblesHand[player]);
                                    marblesHand[player] = new List<GameObject>();
                                }
                            }

                            nextSlot[player] = -2;
                            turnDone[player] = true;
                        }
                    }
                }
            }
        }
    }

    protected Slot FindSlotOnTouch(Touch touch)
    {
        RaycastHit2D[] ray = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
        if (ray.Length > 0)
        {
            foreach(RaycastHit2D i in ray)
            {
                GameObject target = i.collider.gameObject;
                if (target.tag == "Slot")
                {
                    return target.GetComponent<Slot>();
                }
            }
        }
        return null;
    }

    protected void ProgressSlot(int player)
    {
        //Progress clock wise
        nextSlot[player] = (int)Mathf.Repeat(nextSlot[player] - 1, 16);

        while (noGoSlots[player].Contains(nextSlot[player]))
        {
            nextSlot[player]--;
        }
    }

    public void HoldMarbles(List<GameObject> marbles, int player)
    {
        foreach(GameObject i in marbles)
        {
            marblesHand[player].Add(i);
        }
    }

    protected void UpdateSlotSprite()
    {
        //Reset all slots
        for (int i = 0; i < 16; i++)
        {
            slots[i].GetComponent<SpriteRenderer>().sprite = sprDef;
        }

        //Color Single
        for (int i = 0; i < 2; i++)
        {
            if (nextSlot[i] == -1)
            {
                switch (i)
                {
                    case 0:
                        for (int j = 0; j < 7; j++)
                        {
                            if (slots[j].GetComponent<Slot>().MarbleAmount() > 0)
                            {
                                slots[j].GetComponent<SpriteRenderer>().sprite = sprPlayer[i];
                            }
                        }
                        break;
                    case 1:
                        for (int j = 8; j < 15; j++)
                        {
                            if (slots[j].GetComponent<Slot>().MarbleAmount() > 0)
                            {
                                slots[j].GetComponent<SpriteRenderer>().sprite = sprPlayer[i];
                            }
                        }
                        break;
                }
            }
            else if (nextSlot[i] > -1)
            {
                slots[nextSlot[i]].GetComponent<SpriteRenderer>().sprite = sprPlayer[i];
            }
        }
        if (nextSlot[0] == nextSlot[1] && nextSlot[0] > -1)
        {
            //Color if same location
            slots[nextSlot[0]].GetComponent<SpriteRenderer>().sprite = sprP01;
        }
    }

    protected void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuName);
        VideoAdManager.DestroyAd();
    }
}
