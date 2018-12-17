using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterPotrait : MonoBehaviour {

    [SerializeField] Masters master;
    [SerializeField] Image potrait;
    [SerializeField] Image defeatedBadge;
    [SerializeField] Button button;
    [SerializeField] Text masterName;
    [SerializeField] Text description;
    private string nameString;
    private string descString;

    private void Awake()
    {
        nameString = masterName.text;
        descString = description.text;

        bool defeated = JSONSaveData.currentSave.defeated[(int)master];
        if (defeated)
        {
            defeatedBadge.enabled = true;
        }
    }

    public void SetLocked(bool locked)
    {
        if (locked)
        {
            potrait.color = Color.black;
            button.interactable = false;
            masterName.text = "???";
            description.text = "Locked";
        } else
        {
            potrait.color = Color.white;
            button.interactable = true;
            masterName.text = nameString;
            description.text = descString;
        }
    }
}
