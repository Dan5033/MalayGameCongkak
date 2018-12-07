using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public static class AdManager {

    public static BannerView bannerView;
    public static InterstitialAd interstitial;

	public static void InitializeAdMob ()
    {
#if UNITY_ANDROID
        string appId = "ca-app-pub-2580657966473956~6871886768";
#else
            string appId = "unexpected_platform";
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);
    }

    public static void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-2580657966473956/3602849309";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
        
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    public static void DestroyBanner()
    {
        bannerView.Destroy();
    }

    public static void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-2580657966473956/3702785606";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitId);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the interstitial with the request.
        interstitial.LoadAd(request);
    }

    public static void ShowInterstitial()
    {
        if (interstitial.IsLoaded())
        {
            interstitial.Show();
        }
    }

    public static void DestroyInsterstitial()
    {
        interstitial.Destroy();
    }
}
