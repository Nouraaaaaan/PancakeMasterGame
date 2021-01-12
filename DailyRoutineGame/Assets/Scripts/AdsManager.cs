using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager ins;
    public static BannerView bannerView;
    public InterstitialAd interstitial;

    public int levelsToPlayAD = 3;
    private int levelCount;

    [SerializeField] bool disableAds = false;

    private RewardedAd rewardVideo;
    AdRequest AdMobVideoRequest;

    private RewardType rewardType;
    private bool noAds = false; // used with purchaser when user buy no ads

    #region AD_IDs

#if UNITY_ANDROID   // test ids
    private readonly string videoAdMobId = "ca-app-pub-3940256099942544/5224354917";    //test id
    private readonly string bannerUniteID = "ca-app-pub-3940256099942544/6300978111";    // test id
    private readonly string interstitialUnitID = "ca-app-pub-3940256099942544/1033173712"; // test id
#elif UNITY_IPHONE
      private readonly string videoAdMobId = "ca-app-pub-8505877826751443/9780697346";
      private readonly string adbannerUniteIDUnitId = "ca-app-pub-8505877826751443/1677994018";
      private readonly string interstitialUnitID = "ca-app-pub-8505877826751443/3154727210";
#else
    private readonly string videoAdMobId = "unexpected_platform";
      private readonly string adbannerUniteIDUnitId = "unexpected_platform";
      private readonly string interstitialUnitID = "unexpected_platform";
#endif

    #endregion

    private void Awake()
    {
        if (ins == null)
        {
            ins = this;
        }
        else
        {
            Debug.LogError("AdmobManager Singletone Error");
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        if (disableAds)
        {
            return;
        }

        RequestInterstitial();
        RequestRewardedVideo();
        //ShowIntersetial();
        ShowBanner();
    }

    public void CheckToPlayAD()
    {
        if (noAds || disableAds)
        {
            return;
        }

        levelCount++;
        if (levelCount > levelsToPlayAD)
        {
            levelCount = 0;
            ShowIntersetial();
        }
    }

    #region Banner AD
    private void RequestBanner()
    {
        bannerView = new BannerView(bannerUniteID, new AdSize(AdSize.FullWidth, 35), AdPosition.Bottom);
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
    }

    public void HideBanner()
    {
        bannerView.Hide();
    }

    private void RequestTopBanner()
    {
        bannerView = new BannerView(bannerUniteID, new AdSize(AdSize.FullWidth, 35), AdPosition.Top);
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
    }

    public void ShowBanner()
    {
        if (noAds || disableAds)
        {
            return;
        }

        RequestBanner();
        bannerView.Show();
    }

    public void ShowTopBanner()
    {
        RequestTopBanner();
        bannerView.Show();
    }
    #endregion

    #region Intrstitial Ad
    public void RequestInterstitial()
    {
        this.interstitial = new InterstitialAd(interstitialUnitID);
        AdRequest request = new AdRequest.Builder().Build();
        this.interstitial.LoadAd(request);

        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;
    }

    public void ShowIntersetial()
    {
        if (noAds || disableAds)
        {
            return;
        }

        if (interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
        else
        {
            RequestInterstitial();
        }
    }
    #endregion

    #region Rewarded Video
    public void RequestRewardedVideo()
    {
        rewardVideo = new RewardedAd(videoAdMobId);
        AdMobVideoRequest = new AdRequest.Builder().Build();
        rewardVideo.LoadAd(AdMobVideoRequest);

        rewardVideo.OnAdLoaded += HandleRewardedAdLoaded;
        rewardVideo.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        rewardVideo.OnAdOpening += HandleRewardedAdOpening;//mute Audio
        rewardVideo.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        rewardVideo.OnUserEarnedReward += HandleUserEarnedReward;
        rewardVideo.OnAdClosed += HandleRewardedAdClosed;//resume Audio
    }

    public bool CheckRewardVideoLoadeed()
    {
        return rewardVideo.IsLoaded();
    }


    public void ShowRewardedVideo(RewardType type)
    {
        if (rewardVideo.IsLoaded())
        {
            rewardType = type;
            rewardVideo.Show();
        }
        else
        {
            RequestRewardedVideo();
        }
    }
    #endregion

    public void RemoveADs()
    {
        Debug.Log("Remove Ads Called");
        bannerView.Destroy();
        noAds = true;
        //UIManager.instance.DisableNoadsFromSettings();
        //GameManager.instance.diableNoAdsButton = true;
    }

    #region Intersetial and Rewarded CallBacks
    //=====================Intersetial CallBacks=================

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
        AudioListener.volume = 0;
        Time.timeScale = 0;
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
        AudioListener.volume = 1;
        Time.timeScale = 1;
        //interstitial.Destroy();
        //PromoVideoManager.ShowRandomPromo();

    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

    //==========================================================
    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdFailedToLoad event received with message: " + args.Message);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");//Mute Audio
        AudioListener.volume = 0;
        Time.timeScale = 0;
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdFailedToShow event received with message: " + args.Message);

    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received"); //Resume Audio
        RequestRewardedVideo();
        AudioListener.volume = 1;
        Time.timeScale = 1;

        switch (rewardType)
        {
            default:
                break;

            case RewardType.SpecialSyrup:
                {
                    if (PancakeLevelManager.Instance.CurrentSyrup != null)
                    {
                        PancakeLevelManager.Instance.CurrentSyrup.MoveSyrup();
                    }
                }
                break;

            case RewardType.SpecialTopping:           
                {
                    if (PancakeLevelManager.Instance.CurrentSweeter != null)
                    {
                        PancakeLevelManager.Instance.CurrentSweeter.MoveSweeter();
                    }
                }
                break;
        }
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print("HandleRewardedAdRewarded event received for " + amount.ToString() + type);
        // give the user his gift

    }

    #endregion
    public enum RewardType
    {
        SpecialSyrup, SpecialTopping
    }
}
