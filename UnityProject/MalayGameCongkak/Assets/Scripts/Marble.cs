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
    Golden
}

public class Marble : MonoBehaviour {

    [SerializeField] Sprite[] sprites;
    [SerializeField] SpriteRenderer sr;

    private void Start()
    {
        if (SaveData.currentSave.displayType == 2)
        {
            gameObject.SetActive(false);
        }

        sr.sprite = sprites[(int) SaveData.currentSave.selectedDesign];
    }

}
