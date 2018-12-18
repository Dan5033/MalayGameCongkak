using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using System;

public enum AdState
{
    Requested,
    Loaded,
    LoadFail,
    Destroyed
}

public class AdManager: MonoBehaviour {

    public static AdManager instance;

    public BannerView bannerView;
    public InterstitialAd interstitial;

    public bool bannerUp = false;
    public bool interstitialUp = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        } else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu" && bannerUp)
        {
            DestroyBanner();
            bannerUp = false;
        }
    }

    public void InitializeAdMob ()
    {
#if UNITY_ANDROID
        string appId = "ca-app-pub-2580657966473956~6871886768";
#else
            string appId = "unexpected_platform";
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);
    }

    public void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-2580657966473956/3602849309";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        bannerView.OnAdLoaded += HandleOnAdLoaded;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        bannerUp = true;
    }

    public void DestroyBanner()
    {
         bannerView.Destroy();
    }

    #region Interstitial

    public void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-2580657966473956/3702785606";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitId);
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when the ad is closed.
        interstitial.OnAdClosed += HandleOnAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the interstitial with the request.
        interstitial.LoadAd(request);
    }

    public void ShowInterstitial()
    {
        if (interstitial.IsLoaded())
        {
            interstitial.Show();
            interstitialUp = true;
        }
    }

    public void DestroyInsterstitial()
    {
        interstitial.Destroy();
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        interstitialUp = false;
        RequestInterstitial();
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        interstitialUp = false;
    }

    #endregion
}
