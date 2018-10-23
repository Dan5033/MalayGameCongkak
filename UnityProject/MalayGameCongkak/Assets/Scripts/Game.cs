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
    [SerializeField] private int turn = -1;

    [Header("Interface")]
    [SerializeField] GameObject[] texts;

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
                for (var j = 0; j < 7; j++)
                {
                    GameObject marble = Instantiate(prefabMarble);
                    marble.transform.position = handObject[Mathf.RoundToInt(Random.value)].transform.position + new Vector3(Random.value,Random.value,0);
                    slots[i].StoreMarbles(marble);
                }
            }
        }

        UpdateSlotSprite();
    }
	
	void Update ()
    {
        //Move marbles
        for (int x = 0; x < 2; x++)
        {
            foreach (GameObject i in marblesHand[x])
            {
                i.GetComponent<Rigidbody2D>().velocity = (handObject[x].transform.position - i.transform.position).normalized * speed;
            }
        }

        //Text Input
        switch (turn)
        {
            case -1:
                texts[0].GetComponent<Text>().text = "Choose Starting Village";
                texts[1].GetComponent<Text>().text = "Choose Starting Village";
                break;
            case 0:
                texts[0].GetComponent<Text>().text = "Go!";
                texts[1].GetComponent<Text>().text = "Go!";
                break;
            case 1:
                texts[0].GetComponent<Text>().text = "Your Turn";
                texts[1].GetComponent<Text>().text = "";
                break;
            case 2:
                texts[0].GetComponent<Text>().text = "";
                texts[1].GetComponent<Text>().text = "Your Turn";
                break;
            case 3:
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
        }

        //Click Input
        Touch[] touches = Input.touches;
        foreach (Touch i in touches)
        {
            Slot slot = FindSlotOnTouch(i);
            if (slot != null)
            {
                switch (turn)
                {
                    case -1:
                        //Select start
                        if (slot.slotID >= 0 && slot.slotID <= 6)
                        {
                            nextSlot[0] = slot.slotID;
                        }
                        else if (slot.slotID >= 8 && slot.slotID < 16)
                        {
                            nextSlot[1] = slot.slotID;
                        }


                        if (nextSlot[0] > -1 && nextSlot[1] > -1)
                        {
                            //Start Game
                            turn = 0;
                            //Give player marbles
                            slots[nextSlot[0]].SurrenderMarbles(0);
                            slots[nextSlot[1]].SurrenderMarbles(1);
                            //Move Slot by 1
                            ProgressSlot(0);
                            ProgressSlot(1);
                        }
                        break;
                    case 0:
                        //First Turn
                        //Lane priority
                        if (slot.slotID >= 7 && slot.slotID < 16)
                        {
                            PlayerTurn(1, slot);
                            PlayerTurn(0, slot);
                        }
                        else if ((slot.slotID >= 0 && slot.slotID < 7) || slot.slotID == 15)
                        {
                            PlayerTurn(0, slot);
                            PlayerTurn(1, slot);
                        }

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

                        if (turnDone[0] && turnDone[1])
                        {
                            turn = doneFirst + 1;
                            turnDone[0] = false;
                            turnDone[1] = false;
                            nextSlot[doneFirst] = -1;
                        }
                        break;
                    case 1:
                        //P1 Turn
                        PlayerTurn(0, slot);

                        if (turnDone[0])
                        {
                            turnDone[0] = false;
                            turnDone[1] = false;
                            turn = 2;
                            nextSlot[1] = -1;
                            nextSlot[0] = -2;
                        }
                        break;
                    case 2:
                        //P2 Turn
                        PlayerTurn(1, slot);

                        if (turnDone[1])
                        {
                            turnDone[0] = false;
                            turnDone[1] = false;
                            turn = 1;
                            nextSlot[0] = -1;
                            nextSlot[1] = -2;
                        }
                        break;
                    case 3:
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                        break;
                }

                UpdateSlotSprite();
            }

            //Lose State
            bool gameEnd = true;
            foreach(Slot x in slots)
            {
                for(int y = 0; y < 16; y++)
                {
                    if (y != 7 && y != 15)
                    {
                        if (x.MarbleAmount() > 0)
                        {
                            gameEnd = false;
                        }
                    }
                }
            }

            if (gameEnd)
            {
                turn = 3;
            }
            
        }
    }

    private void PlayerTurn(int i, Slot slot)
    {
        if (!turnDone[i])
        {
            if (nextSlot[i] == -1)
            {
                //Pick any of the slots
                bool correctChoiceP0 = i == 0 && slot.slotID >= 0 && slot.slotID < 7 && slot.MarbleAmount() > 0;
                bool correctChoiceP1 = i == 1 && slot.slotID >= 8 && slot.slotID < 15 && slot.MarbleAmount() > 0;
                if (correctChoiceP0 || correctChoiceP1)
                {
                    slot.SurrenderMarbles(i);
                    nextSlot[i] = slot.slotID;
                    ProgressSlot(i);
                }
            }
            else if (nextSlot[i] > -1)
            {
                //Pick only the right slot
                if (nextSlot[i] == slot.slotID)
                {
                    //Move one marble
                    slot.StoreMarbles(marblesHand[i][0]);
                    marblesHand[i].RemoveAt(0);
                    ProgressSlot(i);

                    //Check if hand is empty
                    if (marblesHand[i].Count == 0)
                    {
                        //Select any when last marble is home
                        if ((i == 0 && slot.slotID == 15) || (i == 1 && slot.slotID == 7))
                        {
                            nextSlot[i] = -1;
                        }
                        else if (slot.MarbleAmount() > 1)
                        {
                            //Pickup all in the surrent slot
                            slot.SurrenderMarbles(i);
                        } else if (slot.MarbleAmount() == 1)
                        {
                            //Steal all
                            bool homeP0 = i == 0 && slot.slotID >= 0 && slot.slotID < 7;
                            bool homeP1 = i == 1 && slot.slotID >= 8 && slot.slotID < 15;

                            if (homeP1 || homeP0)
                            {
                                slot.SurrenderMarbles(i);
                                slots[14 - slot.slotID].SurrenderMarbles(i);

                                if (homeP0)
                                {
                                    slots[15].StoreMarbles(marblesHand[i]);
                                    marblesHand[i] = new List<GameObject>();
                                } else if (homeP1)
                                {
                                    slots[7].StoreMarbles(marblesHand[i]);
                                    marblesHand[i] = new List<GameObject>();
                                }
                            }

                            nextSlot[i] = -2;
                            turnDone[i] = true;
                        }
                        else
                        {
                            nextSlot[i] = -2;
                            turnDone[i] = true;
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

        //Skip enemy base
        if ((player == 0 && nextSlot[player] == 7) || (player == 1 && nextSlot[player] == 15))
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
