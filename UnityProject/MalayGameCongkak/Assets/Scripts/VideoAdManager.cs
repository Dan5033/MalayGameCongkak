using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoAdManager : MonoBehaviour {

	void Start ()
    {

#if UNITY_ANDROID
        //string appId = "ca-app-pub-2580657966473956~6871886768";
        string appId = "ca-app-pub-3940256099942544~3347511713";
#elif UNITY_IPHONE
            string appId = "ca-app-pub-3940256099942544~1458002511";
#else
            string appId = "unexpected_platform";
#endif
    }

    void Update ()
    {
		
	}
}
