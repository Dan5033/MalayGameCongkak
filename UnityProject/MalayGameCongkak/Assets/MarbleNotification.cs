﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarbleNotification : MonoBehaviour {

    [SerializeField] private RectTransform notification;
    [SerializeField] private Text noteText;
    [SerializeField] private Image notePreview;
    [SerializeField] private Sprite[] marbleSprites;

    private void Start()
    {
        notification.anchoredPosition = new Vector2(-119.85f, -30.59998f);
        notification.transform.localScale = new Vector3(1,1,1);
        StartCoroutine(LifeCycle());
    }

    public void Setup(MarbleDesign design)
    {
        string[] names = new string[]
        {
                "Basic Marble",
                "Lone Snowman",
                "Lit Darkness",
                "Wealth",
                "Calm Crescent",
                "14 Stripes",
                "Freedom",
                "Master of Masters"
        };

        noteText.text = names[(int)design];
        notePreview.sprite = marbleSprites[(int)design];
    }

    IEnumerator LifeCycle()
    {
        float time = 0;
        while (time < 2)
        {
            time += Time.deltaTime;
            notification.anchoredPosition += new Vector2(0.1f, 0);
            yield return null;
        }

        Vector3 dest = notification.anchoredPosition + new Vector2(1000, 0);
        while (Vector2.Distance(notification.anchoredPosition, dest) > 10)
        {
            notification.anchoredPosition = Vector2.Lerp(notification.anchoredPosition, dest, 0.1f);
            yield return null;
        }

        Destroy(gameObject);
    }
}