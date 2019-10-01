using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommentNotification : MonoBehaviour {

    [SerializeField] RectTransform main;
    [SerializeField] RectTransform yes;
    [SerializeField] RectTransform later;
    [SerializeField] RectTransform no;
    [SerializeField] Image bg;

    Vector2 mainDest;
    Vector2 yesDest;
    Vector2 laterDest;
    Vector2 noDest;

    // Use this for initialization
    void Start()
    {
        mainDest = main.position;
        yesDest = yes.position;
        laterDest = later.position;
        noDest = no.position;

        bg.enabled = false;
        main.position += new Vector3(3000, 0);
        yes.position += new Vector3(3000, 0);
        later.position += new Vector3(3000, 0);
        no.position += new Vector3(3000, 0);
    }

    public void ShowNotification()
    {
        StartCoroutine(Show());
    }

    public void HideNotification()
    {
        StartCoroutine(Hide());
    }

    public void OpenStorePage()
    {
        Application.OpenURL("market://details?id=com.GammaResources.MG.Congkak");
        StartCoroutine(Hide());
    }

    public void NeverShowAgain()
    {
        PlayerPrefs.SetInt("MatchNum", -1);
        StartCoroutine(Hide());
    }

    public void ShowLater()
    {
        PlayerPrefs.SetInt("MatchNum", 1);
        StartCoroutine(Hide());
    }

    IEnumerator MoveObject(RectTransform transform, Vector3 dest, float speed)
    {
        while (Vector3.Distance(transform.position,dest) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, dest, speed);
            yield return null;
        }
    }

    IEnumerator Show()
    {
        StopCoroutine("Hide");

        bg.enabled = true;
        StartCoroutine(MoveObject(main, mainDest, 0.1f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(MoveObject(yes, yesDest, 0.1f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(MoveObject(later, laterDest, 0.1f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(MoveObject(no, noDest, 0.1f));
    }

    IEnumerator Hide()
    {
        StopCoroutine("Show");

        StartCoroutine(MoveObject(main, mainDest + new Vector2(3000, 0), 0.1f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(MoveObject(yes, yesDest + new Vector2(3000, 0), 0.1f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(MoveObject(later, laterDest + new Vector2(3000, 0), 0.1f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(MoveObject(no, noDest + new Vector2(3000, 0), 0.1f));
        yield return new WaitForSeconds(0.1f);
        bg.enabled = false;
    }
}
