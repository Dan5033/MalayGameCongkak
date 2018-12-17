using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MarbleDesign
{
    Basic,
    Christmas,
    Deepavali,
    CNY,
    Eid,
    MalaysiaDay,
    Independence,
    BT,
    Golden
}

public class Marble : MonoBehaviour {

    [SerializeField] SpriteRenderer sr;

    public static Sprite[] sprites;

    private void Start()
    {
        if (JSONSaveData.currentSave.displayType == 2)
        {
            gameObject.SetActive(false);
        }

        sr.sprite = sprites[(int) JSONSaveData.currentSave.selectedDesign];
    }

}
