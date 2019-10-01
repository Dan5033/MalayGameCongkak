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
    MGC,
    Golden
}

public class Marble : MonoBehaviour {

    [SerializeField] SpriteRenderer sr;

    public static Sprite[] sprites;
    public static string[] names =
    {
            "Basic Marble",
            "Lone Snowman",
            "Lit Darkness",
            "Wealth",
            "Calm Crescent",
            "14 Stripes",
            "Freedom",
            "BT",
            "Loyalist",
            "Master of Masters"
    };

    private void Start()
    {
        if (JSONSaveData.currentSave.displayType == 2)
        {
            gameObject.SetActive(false);
        }

        sr.sprite = sprites[(int) JSONSaveData.currentSave.selectedDesign];
    }

}
