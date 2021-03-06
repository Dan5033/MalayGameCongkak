﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {

    private List<GameObject> marbles = new List<GameObject>();
    [SerializeField] float speed = 20;
    [SerializeField] SpriteRenderer sr;
    public int slotID = 0;

	void Update () {
		foreach (GameObject i in marbles)
        {
            i.GetComponent<Rigidbody2D>().velocity = (transform.position - i.transform.position).normalized * speed;
        }

        name = MarbleAmount().ToString();
	}

    //Get Data
    public int MarbleAmount()
    {
        return marbles.Count;
    }

    //Slot Actions
    public void StoreMarbles(GameObject item)
    {
        item.layer = 10 + slotID;
        marbles.Add(item);
    }

    public void StoreMarbles(List<GameObject> item)
    {
        foreach(GameObject i in item)
        {
            StoreMarbles(i);
        }
    }

    public void SurrenderMarbles(int player)
    {
        Game.instance.HoldMarbles(marbles,player);
        marbles = new List<GameObject>();
    }

    public void BurnSlot()
    {
        sr.color = new Color(0.5f, 0.5f, 0.5f);
    }

    public void UnBurnSlot()
    {
        sr.color = new Color(1, 1, 1);
    }
}
