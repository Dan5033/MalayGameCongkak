using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotDisplay : MonoBehaviour {

    [SerializeField] private Slot slot;
    [SerializeField] private Text text;

	void Update () {
        text.text = slot.MarbleAmount().ToString();
	}
}
