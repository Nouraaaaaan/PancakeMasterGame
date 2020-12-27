using System.Collections;
using UnityEngine;
using System.Collections.Generic;
#if ADS_ADMOB
using GoogleMobileAds.Api;
#endif
#if ADS_UNITY
using UnityEngine.Monetization;
using UnityEngine.Advertisements;
#endif

public class ServicesManager : MonoBehaviour {

    public static ServicesManager instance { get; set; }

    [HideInInspector] public int rewardedCoins;

    #region ADMOB
    [HideInInspector] public bool enableTestMode;
    [HideInInspector] public string appID;
    [HideInInspector] public string bannerID;
#if ADS_ADMOB
    [HideInInspector] public AdPosition bannerPosition;
#endif
    [HideInInspector] public string interstitialID;
    [HideInInspector] public string rewardedVideoAdsID;

#if ADS_ADMOB
    private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardBasedVideoAd rewardVideoAd;
#endif

    #endregion
    #region UnityAds
    [HideInInspector] public bool testMode;
    [HideInInspector] public string gameID;
    [HideInInspector] public string bannerPlacementID;
    [HideInInspector] public string videoAdPlacementID;
    [HideInInspector] public string rewardedVideoAdPlacementID;
    #endregion
    #region IAP
    [HideInInspector] public string noAdsID;
    #endregion


    bool isRewardAdded;

    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start ()
    {
       DontDestroyOnLoad(this.gameObject);

        if (enableTestMode)
        {
            bannerID = "ca-app-pub-3940256099942544/6300978111";
            interstitialID = "ca-app-pub-3940256099942544/1033173712";
            rewardedVideoAdsID = "ca-app-pub-3940256099942544/5224354917";
        }

       InitializeAdmob();
       InitializeUnityAds();
	}
    #region Admob
    private void RequestBannerAdmob()
    {
#if ADS_ADMOB
        bannerView = new BannerView(bannerID,AdSize.Banner,AdPosition.Bottom);

        AdRequest request = new AdRequest.Builder().Build();

        bannerView.LoadAd(request);
#endif
    }
    private void RequestInterstialAdmob()
    {
#if ADS_ADMOB
        this.interstitial = new InterstitialAd(interstitialID);

        AdRequest request = new AdRequest.Builder().Build();

        this.interstitial.LoadAd(request);
#endif
    }
    private void RequestRewardedVideoAdAdmob()
    {
        isRewardAdded = false;

#if ADS_ADMOB
        this.rewardVideoAd = RewardBasedVideoAd.Instance;

        this.rewardVideoAd.OnAdRewarded += HandleRewardBasedVideoRewarded;

        AdRequest request = new AdRequest.Builder().Build();

        this.rewardVideoAd.LoadAd(request, rewardedVideoAdsID);
#endif
    }
    public void InitializeAdmob()
    {
#if ADS_ADMOB
        MobileAds.Initialize(appID);

        this.RequestInterstialAdmob();
        this.RequestRewardedVideoAdAdmob();
#endif
    }
    public void InitializeBannerAdmob()
    {
        #if ADS_ADMOB
        MobileAds.Initialize(appID);
#endif

        this.RequestBannerAdmob();
    }
    public void ShowBannerAdmob()
    {
        #if ADS_ADMOB
        if(this.bannerView != null)
          this.bannerView.Show();
#endif
    }
    public void DestroyBannerAdmob()
    {
#if ADS_ADMOB
        this.bannerView.Destroy();
#endif
    }
    public void ShowInterstitialAdmob()
    {
#if ADS_ADMOB
        if (this.interstitial.IsLoaded())
        {
            Debug.Log("Interstitial was loaded succesfully!");

            this.interstitial.Show();
        }
#endif
    }
    public void ShowRewardedVideoAdAdmob()
    {
#if ADS_ADMOB
        if (rewardVideoAd.IsLoaded())
        {
            Debug.Log("Rewarded was loaded succesfully!");

            rewardVideoAd.Show();
        }
#endif
    }
#if ADS_ADMOB
    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;

        GameManager.Instance.RewardedCompleted(true);
    }
#endif
    #endregion
    #region UnityAds
    public void InitializeUnityAds()
    {
#if ADS_UNITY
        Monetization.Initialize(gameID, testMode);
        Advertisement.Initialize(gameID, testMode);
#endif
    }
#if ADS_UNITY
    private IEnumerator RequestInterstialUnityAds()
    {
        while (!Monetization.IsReady(videoAdPlacementID))
        {
            yield return new WaitForSeconds(0.25f);
        }

        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent(videoAdPlacementID) as ShowAdPlacementContent;

        if (ad != null)
        {
            ad.Show();
        }
    }

    IEnumerator RequestRewardedVideoAdUnityAds()
    {
        while (!Monetization.IsReady(rewardedVideoAdPlacementID))
        {
            yield return null;
        }

        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent(rewardedVideoAdPlacementID) as ShowAdPlacementContent;

        if (ad != null)
        {
            ad.Show(AdFinished);
        }

    }
    IEnumerator RequestBannerUnityAds()
    {

        while (!Advertisement.IsReady("banner"))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.Show(bannerPlacementID);

    }
    void AdFinished(UnityEngine.Monetization.ShowResult result)
    {
        if (result == UnityEngine.Monetization.ShowResult.Finished)
        {
             GameManager.instance.RewardedCompleted(true);
        }
    }
#endif
    public void ShowBannerUnityAds()
    {
#if ADS_UNITY
        StartCoroutine(RequestBannerUnityAds());
#endif
    }
    public void ShowInterstitialUnityAds()
    {
        #if ADS_UNITY
        StartCoroutine(RequestInterstialUnityAds());
#endif
    }
    public void ShowRewardedVideoUnityAds()
    {
        #if ADS_UNITY
        StartCoroutine(RequestRewardedVideoAdUnityAds());
#endif
    }
#endregion
}
