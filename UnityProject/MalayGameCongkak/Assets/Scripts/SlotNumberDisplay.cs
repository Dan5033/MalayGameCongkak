using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotNumberDisplay : MonoBehaviour {

    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject parent;
    private Text[] displays = new Text[16];

    private void Awake()
    {
        if (JSONSaveData.currentSave.displayType != 0)
        {
            for (int i = 0; i < 16; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.transform.SetParent(parent.transform);

                if (i == 7 || i == 15)
                {
                    obj.transform.localScale = new Vector3(2, 2, 2);
                }
                else
                {
                    obj.transform.localScale = new Vector3(1, 1, 1);
                }

                displays[i] = obj.transform.GetChild(0).GetComponent<Text>();
            }
        }
    }

    public void ResetNumbers(Slot[] slots, float rotation = 0)
    {
        if (JSONSaveData.currentSave.displayType != 0)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                displays[i].transform.parent.position = Camera.main.WorldToScreenPoint(slots[i].transform.position);
                displays[i].transform.parent.eulerAngles = new Vector3(0, 0, rotation);
                displays[i].text = slots[i].MarbleAmount() + "";
            }
        }
    }
}
