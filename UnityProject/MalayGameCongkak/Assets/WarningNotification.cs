using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningNotification : MonoBehaviour {

    Vector3 center;
    Coroutine oldCR;
    Coroutine newCR;
    [SerializeField] Image blackScreen;
    int number = 0;

    // Use this for initialization
    void Start() {
        center = transform.position;
        center.x = Screen.width / 2; ;
    }

    // Update is called once per frame
    void Update() {

    }

    public void Show()
    {
        Vector3 start = center + new Vector3(-Screen.height, 0, 0);
        transform.position = start;

        blackScreen.enabled = true;
        newCR = StartCoroutine(MoveToDestination(center,number));
        number++;
    }

    public void Hide()
    {
        transform.position = center;

        blackScreen.enabled = false;
        newCR = StartCoroutine(MoveToDestination(center + new Vector3(Screen.height, 0, 0), number));
        number++;
    }
    
    IEnumerator MoveToDestination(Vector3 destination,int num)
    {
        if (oldCR != null)
        {
            StopCoroutine(oldCR);
        }

        yield return null;
        oldCR = newCR;
        yield return null;

        float distance = Vector3.Distance(transform.position, destination);
        while (distance > 1)
        {
            transform.position = Vector3.Lerp(transform.position, destination,0.3f);
            yield return null;
        }
    }
}
