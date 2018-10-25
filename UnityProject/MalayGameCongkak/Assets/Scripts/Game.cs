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

public enum GameMode
{
    bestOf,
    burningVillages,
    speedCongkak
}

public enum GameState
{
    PickingStartSlot,
    BothTurns,
    P1Turn,
    P2Turn,
    GameEnd,
    Paused,
    Animating
}

public class Game : MonoBehaviour {

    public static Game instance;

    //Game Settings    
    public static StartStyle startStyle = StartStyle.together;
    public static GameMode gameMode = GameMode.bestOf;
    public static int miscNum = 1;

    [Header("Prefabs")]
    [SerializeField] private GameObject prefabMarble;

    [Header("Preset Objects")]
    [SerializeField] private Slot[] slots;

    [Header("Slot Sprites")]
    [SerializeField] private Sprite sprDef;
    [SerializeField] private Sprite[] sprPlayer;
    [SerializeField] private Sprite sprP01;

    [Header("Hand")]
    [SerializeField] private float speed = 20;
    [SerializeField] private GameObject[] handObject;
    private List<GameObject>[] marblesHand = { new List<GameObject>(), new List<GameObject>() };

    [Header("Game")]
    private int[] nextSlot = { -1, -1 };
    private bool[] turnDone = { false, false };
    private int doneFirst = -1;
    public GameState turn = GameState.PickingStartSlot;
    private List<int>[] noGoSlots = { new List<int> { 7 }, new List<int> { 15 } };

    //game mode variables
    private int[] wins = { 0, 0 };
    private float[] timeRemaining = { 0, 0 };
    public static int marblePerSlot = 7;

    [Header("Interface")]
    [SerializeField] GameObject[] texts;
    [SerializeField] Text winsText;
    [SerializeField] Image[] timeBar;

    [Header("Sounds")]
    [SerializeField] private AudioSource popSource;
    [SerializeField] private AudioClip pop;
    [SerializeField] private AudioClip take;
    [SerializeField] private AudioClip money;

    [Header("Scenes")]
    [SerializeField] private string mainMenuName;

	void Start ()
    {
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
	
	void Update ()
    {
        if (turn != GameState.Paused)
        {
            UpdateSlotSprite();

            //Time
            if (gameMode == GameMode.speedCongkak)
            {
                switch (turn)
                {
                    case GameState.BothTurns:
                        timeRemaining[0] -= Time.deltaTime;
                        timeRemaining[1] -= Time.deltaTime;
                        if (timeRemaining[0] <= 0)
                        {
                            turnDone[0] = true;
                        }
                        if (timeRemaining[1] <= 0)
                        {
                            turnDone[1] = true;
                        }
                        break;
                    case GameState.P1Turn:
                        timeRemaining[0] -= Time.deltaTime;
                        if (timeRemaining[0] <= 0)
                        {
                            turnDone[0] = true;
                        }
                        break;
                    case GameState.P2Turn:
                        timeRemaining[1] -= Time.deltaTime;
                        if (timeRemaining[1] <= 0)
                        {
                            turnDone[1] = true;
                        }
                        break;
                }

                timeBar[0].transform.localScale = new Vector3(timeRemaining[0] / (miscNum + 1), 1, 1);
                timeBar[1].transform.localScale = new Vector3(timeRemaining[1] / (miscNum + 1), 1, 1);
            }

            //Move marbles
            for (int x = 0; x < 2; x++)
            {
                foreach (GameObject i in marblesHand[x])
                {
                    i.GetComponent<Rigidbody2D>().velocity = (handObject[x].transform.position - i.transform.position).normalized * speed;
                }
            }

            //Text Display
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

            //Click Input
            Touch[] touches = Input.touches;
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
                                    else if (slot.slotID >= 8 && slot.slotID < 15)
                                    {
                                        nextSlot[1] = slot.slotID;
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
                                    PlayerTurn(1, slot);
                                    break;
                                case GameState.P1Turn:
                                    //P1 Turn
                                    PlayerTurn(0, slot);
                                    break;
                                case GameState.P2Turn:
                                    //P2 Turn
                                    PlayerTurn(1, slot);
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

    private void CheckForBurned()
    {
        slots[15].SurrenderMarbles(0);
        int target = 0;
        foreach (GameObject marble in marblesHand[0].ToArray())
        {
            if (slots[target].MarbleAmount() == marblePerSlot)
            {
                target++;
                if (noGoSlots[0].Contains(target))
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
                if (noGoSlots[0].Contains(target))
                {
                    break;
                }
            }
            slots[target].StoreMarbles(marble);
            marblesHand[1].Remove(marble);
        }
        slots[7].StoreMarbles(marblesHand[1]);
        marblesHand[1] = new List<GameObject>();

        foreach (Slot i in slots)
        {
            bool empty = i.MarbleAmount() <= 0;
            bool notHome = i.slotID != 7 && i.slotID != 15;
            bool notNoGo = !noGoSlots[0].Contains(i.slotID) && !noGoSlots[1].Contains(i.slotID);
            if (empty && notHome && notNoGo)
            {
                noGoSlots[0].Add(i.slotID);
                noGoSlots[1].Add(i.slotID);
            }
        }
    }

    private void ResetMarbles()
    {
        //Move all marbles to one list
        slots[15].SurrenderMarbles(0);
        slots[7].SurrenderMarbles(0);

        //Place all marble in their places
        int target = 0;
        foreach (GameObject marble in marblesHand[0].ToArray())
        {
            if (slots[target].MarbleAmount() == 1)
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

    private void SetupTurn(int player)
    {
        turnDone[0] = false;
        turnDone[1] = false;
        switch (player)
        {
            case 0:
                turn = GameState.P1Turn;
                nextSlot[0] = -1;
                nextSlot[1] = -2;
                if (gameMode == GameMode.speedCongkak)
                {
                    timeRemaining[0] = miscNum + 1;
                    timeRemaining[1] = 0;
                }
                break;
            case 1:
                turn = GameState.P2Turn;
                nextSlot[1] = -1;
                nextSlot[0] = -2;
                if (gameMode == GameMode.speedCongkak)
                {
                    timeRemaining[1] = miscNum + 1;
                    timeRemaining[0] = 0;
                }
                break;
            case 2:
                turn = GameState.PickingStartSlot;
                nextSlot[0] = -1;
                nextSlot[1] = -1;
                doneFirst = -1;
                if (gameMode == GameMode.speedCongkak)
                {
                    timeRemaining[0] = miscNum + 1;
                    timeRemaining[1] = miscNum + 1;
                }
                break;
        }
    }

    private int PlayerValidSlotsNumber(int player)
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

    private void PlayerTurn(int player, Slot slot)
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

                    if (gameMode == GameMode.speedCongkak)
                    {
                        timeRemaining[player] = miscNum;
                    }

                    popSource.PlayOneShot(take);
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

                    if (gameMode == GameMode.speedCongkak)
                    {
                        timeRemaining[player] = miscNum;
                    }

                    popSource.PlayOneShot(pop);

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
                                popSource.PlayOneShot(money);

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

    private Slot FindSlotOnTouch(Touch touch)
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

    private void ProgressSlot(int player)
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

    void UpdateSlotSprite()
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
}
