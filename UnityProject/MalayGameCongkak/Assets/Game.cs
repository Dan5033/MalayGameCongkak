using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public static Game instance;

    [Header("Prefabs")]
    [SerializeField] private GameObject prefabMarble;

    [Header("Preset Objects")]
    [SerializeField] private Slot[] slots;
    private List<GameObject> marbles1 = new List<GameObject>();
    private List<GameObject> marbles2 = new List<GameObject>();

    [Header("Slot Sprites")]
    [SerializeField] private Sprite sprDef;
    [SerializeField] private Sprite sprP1;
    [SerializeField] private Sprite sprP2;

    [Header("Hand")]
    [SerializeField] private float speed = 20;
    [SerializeField] private GameObject hand1;
    [SerializeField] private GameObject hand2;

    [Header("Game")]
    //Start Position
    private int nextSlot1 = -1;
    private int nextSlot2 = -1;
    private bool turnDone1 = false;
    private bool turnDone2 = false;
    [SerializeField] private int turn = -1;

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
                    slots[i].StoreMarbles(marble);
                }
            }
        }

        UpdateSlotSprite();
    }
	
	void Update ()
    {
        //Move marbles
        foreach (GameObject i in marbles1)
        {
            i.GetComponent<Rigidbody2D>().velocity = (hand1.transform.position - i.transform.position).normalized * speed;
        }

        foreach (GameObject i in marbles2)
        {
            i.GetComponent<Rigidbody2D>().velocity = (hand2.transform.position - i.transform.position).normalized * speed;
        }

        //Click Input
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D[] ray = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (ray.Length > 0)
            {
                foreach(RaycastHit2D i in ray)
                {
                    GameObject target = i.collider.gameObject;
                    if (target.tag == "Slot")
                    {
                        Slot slot = target.GetComponent<Slot>();
                        switch (turn)
                        {
                            case -1:
                                //Select start
                                if (slot.slotID >= 0 && slot.slotID <= 6) {
                                    nextSlot1 = slot.slotID;
                                    UpdateSlotSprite();
                                } else if (slot.slotID >= 8 && slot.slotID < 16)
                                {
                                    nextSlot2 = slot.slotID;
                                    UpdateSlotSprite();
                                }
                                if (nextSlot1 > -1 && nextSlot2 > -1)
                                {
                                    turn = 0;
                                    slots[nextSlot1].SurrenderMarbles(1);
                                    slots[nextSlot2].SurrenderMarbles(2);
                                    nextSlot1 = (int)Mathf.Repeat(nextSlot1 - 1, 16);
                                    if (nextSlot1 == 7)
                                    {
                                        nextSlot1--;
                                    }
                                    nextSlot2 = (int) Mathf.Repeat(nextSlot2 - 1, 16);
                                    if (nextSlot2 == 15)
                                    {
                                        nextSlot2--;
                                    }
                                    UpdateSlotSprite();
                                }
                                break;
                            case 0:
                                //First Turn
                                if (!turnDone1)
                                {
                                    if (nextSlot1 > -1)
                                    {
                                        if (slot.slotID == nextSlot1)
                                        {
                                            //Move marble
                                            slot.StoreMarbles(marbles1[0]);
                                            marbles1.RemoveAt(0);

                                            //Check if hand empty
                                            if (marbles1.Count == 0)
                                            {
                                                if (slot.slotID == 15)
                                                {
                                                    nextSlot1 = -1;
                                                }
                                                else if (slot.MarbleAmount() > 1)
                                                {
                                                    slot.SurrenderMarbles(1);
                                                    nextSlot1 = (int)Mathf.Repeat(nextSlot1 - 1, 16);
                                                    if (nextSlot1 == 7)
                                                    {
                                                        nextSlot1--;
                                                    }
                                                }
                                                else
                                                {
                                                    nextSlot1 = -2;
                                                    turnDone1 = true;
                                                }
                                            }
                                            else
                                            {
                                                nextSlot1 = (int)Mathf.Repeat(nextSlot1 - 1, 16);
                                                if (nextSlot1 == 7)
                                                {
                                                    nextSlot1--;
                                                }
                                            }

                                        }
                                    } else if (nextSlot1 == -1)
                                    {
                                        if (slot.slotID >= 0 && slot.slotID <= 6)
                                        {
                                            slot.SurrenderMarbles(1);
                                            nextSlot1 = (int)Mathf.Repeat(slot.slotID - 1, 16);
                                            if (nextSlot1 == 7)
                                            {
                                                nextSlot1--;
                                            }
                                        }
                                    }
                                }
                                UpdateSlotSprite();
                                break;
                            case 1:
                                //P1 Turn
                                break;
                            case 2:
                                //P2 Turn
                                break;
                        }
                        /*
                        if (marbles.Count == 0)
                        {
                            //Pickup marbles
                            //If slot is the basic ones
                            if (slot.slotID != 7 && slot.slotID != 15)
                            {
                                slot.SurrenderMarbles();
                                Game.instance.slotStart = slot.slotID;
                            }
                        } else
                        {
                            //Place marbles
                            //If slot is the next one
                            if ((slot.slotID == slotStart - 1) || (slotStart == 0 && slot.slotID == 15))
                            {
                                slot.StoreMarbles(marbles[0]);
                                marbles.RemoveAt(0);

                                slotStart = (int) Mathf.Repeat(slotStart - 1, 15);
                         
                            }
                        }
                        break;
                        */
                    }
                }
            }
        }
    }

    public bool HoldMarbles(List<GameObject> marbles, int player)
    {
        switch (player)
        {
            case 1:
                if (marbles1.Count > 0)
                {
                    return false;
                }
                else
                {
                    marbles1 = marbles;
                    return true;
                }
            case 2:
                if (marbles2.Count > 0)
                {
                    return false;
                }
                else
                {
                    marbles2 = marbles;
                    return true;
                }
        }
        return false;
    }

    void UpdateSlotSprite()
    {
        for (int i = 0; i < 16; i++)
        {
            slots[i].GetComponent<SpriteRenderer>().sprite = sprDef;
        }

        if (nextSlot1 == -1)
        {
            for (int i = 0; i < 7; i++)
            {
                slots[i].GetComponent<SpriteRenderer>().sprite = sprP1;
            }
        } else if (nextSlot1 > -1)
        {
            slots[nextSlot1].GetComponent<SpriteRenderer>().sprite = sprP1;
        }

        if (nextSlot2 == -1)
        {
            for (int i = 8; i < 15; i++)
            {
                slots[i].GetComponent<SpriteRenderer>().sprite = sprP2;
            }
        } else if (nextSlot2 > -1)
        {
            slots[nextSlot2].GetComponent<SpriteRenderer>().sprite = sprP2;
        }
    }
}
