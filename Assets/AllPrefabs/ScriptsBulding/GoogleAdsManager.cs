//using System;
//using System.Collections;
//using GoogleMobileAds.Api;
//using TMPro;
//using UnityEngine;
//using UnityEngine.Events;

//public class GoogleAdsManager : MonoBehaviour
//{
//    public static GoogleAdsManager Instance { get; private set; }

//    [SerializeField] private string rewardedId;
//    private RewardedAd _rewardedAd;
//    private bool _isLoading = false;
//    private DateTime _lastAdShowTime;

//    [Header("Ads Events For Game")]
//    public UnityAction RewardedEndEvent;

//    // Minimal time interval between ads (seconds)
//    [SerializeField] private float minTimeBetweenAds = 30f;

//    // Maximum retry attempts for loading ads
//    [SerializeField] private int maxRetryAttempts = 3;
//    private int currentRetryAttempt = 0;


//    //------------------------
//    [SerializeField] private RewerdCollection steelcollect;
//    [SerializeField] private GameObject internetErrorUI;



//    [SerializeField] private GameObject WaitforAD;








//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private void Start()
//    {
//        MobileAds.RaiseAdEventsOnUnityMainThread = true;
//        MobileAds.Initialize(initStatus =>
//        {
//            LoadRewardedAd();
//            RewardedEndEvent += () => StartCoroutine(GiveRewardedRewardWithDelay());
//        });

//        _lastAdShowTime = DateTime.MinValue;
//    }

//    private void LoadRewardedAd()
//    {
//        if (_isLoading) return;

//        _isLoading = true;

//        if (_rewardedAd != null)
//        {
//            _rewardedAd.Destroy();
//            _rewardedAd = null;
//        }

//        Debug.Log("Loading the rewarded ad.");
//        var adRequest = new AdRequest();

//        RewardedAd.Load(rewardedId, adRequest, (RewardedAd ad, LoadAdError error) =>
//        {
//            _isLoading = false;

//            if (error != null || ad == null)
//            {
//                Debug.LogError($"Rewarded ad failed to load: {error}");
//                RetryLoadingAd();
//                //ADD INTRERNET CHECK
//                return;
//            }

//            Debug.Log("Rewarded ad loaded successfully.");
//            _rewardedAd = ad;
//            currentRetryAttempt = 0;
//            SetupRewardedAdEvents(_rewardedAd);
//        });
//    }

//    private void RetryLoadingAd()
//    {
//        if (currentRetryAttempt < maxRetryAttempts)
//        {
//            currentRetryAttempt++;
//            float delay = Mathf.Pow(2, currentRetryAttempt); // Exponential backoff
//            StartCoroutine(RetryLoadingWithDelay(delay));
//        }
//    }

//    private IEnumerator RetryLoadingWithDelay(float delay)
//    {
//        yield return new WaitForSecondsRealtime(delay); // Real vaqtdan foydalanadi
//        LoadRewardedAd();
//    }

//    public bool CanShowAd()
//    {
//        if (_rewardedAd == null || !_rewardedAd.CanShowAd())
//            return false;

//        float timeSinceLastAd = (float)(DateTime.Now - _lastAdShowTime).TotalSeconds;
//        return timeSinceLastAd >= minTimeBetweenAds;
//    }

//    public void ShowRewardedAd()
//    {
//        // Check for internet connection
//        if (Application.internetReachability == NetworkReachability.NotReachable)
//        {
//            Debug.Log("No internet connection available.");
//            if (internetErrorUI != null)
//            {
//                internetErrorUI.SetActive(true); // Show the internet error UI
//            }
//            return;
//        }



//        if (!CanShowAd())
//        {
//            float remainingTime = minTimeBetweenAds - (float)(DateTime.Now - _lastAdShowTime).TotalSeconds;
//            if (remainingTime > 0)
//            {
//                WaitforAD.SetActive(true);
//                Debug.Log($"Please wait {remainingTime:F1} seconds before showing next ad");
//                return;
//            }

//            LoadRewardedAd();
//            return;
//        }

//        _rewardedAd.Show((Reward reward) =>
//        {
//            _lastAdShowTime = DateTime.Now;
//            if (RewardedEndEvent != null)
//                RewardedEndEvent.Invoke();
//            Debug.Log($"Rewarded ad rewarded the user. Type: {reward.Type}, amount: {reward.Amount}.");
//            GoldToSteelConverter.Instance.AddSteel200();
//            steelcollect.OnButtonClicked();
//            //ADD STEEL EFFECT
//        });
//    }

//    private IEnumerator GiveRewardedRewardWithDelay()
//    {
//        // Add small delay to ensure proper synchronization
//        yield return new WaitForSeconds(0.5f);
//        try
//        {
//            UpgradeManager.Instance.ADD15Minut();
//            Debug.Log("Rewarded Ads Reward Given");
//        }
//        catch (Exception e)
//        {
//            Debug.LogError($"Error giving reward: {e.Message}");
//        }
//    }

//    private void SetupRewardedAdEvents(RewardedAd ad)
//    {
//        ad.OnAdPaid += (AdValue adValue) =>
//        {
//            Debug.Log($"Rewarded ad paid {adValue.Value} {adValue.CurrencyCode}.");
//        };

//        ad.OnAdImpressionRecorded += () =>
//        {
//            Debug.Log("Rewarded ad recorded an impression.");
//        };

//        ad.OnAdClicked += () =>
//        {
//            Debug.Log("Rewarded ad was clicked.");
//        };

//        ad.OnAdFullScreenContentOpened += () =>
//        {
//            Debug.Log("Rewarded ad full screen content opened.");
//        };

//        ad.OnAdFullScreenContentClosed += () =>
//        {
//            Debug.Log("Rewarded ad full screen content closed.");
//            LoadRewardedAd();
//        };

//        ad.OnAdFullScreenContentFailed += (AdError error) =>
//        {
//            Debug.LogError($"Rewarded ad failed to open full screen content with error: {error}");
//            LoadRewardedAd();
//        };
//    }

//    private void OnDestroy()
//    {
//        if (_rewardedAd != null)
//        {
//            _rewardedAd.Destroy();
//            _rewardedAd = null;
//        }
//    }
//}