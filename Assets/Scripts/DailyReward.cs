using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DailyReward : MonoBehaviour
{
    [Header("Reward Settings")]
    public int rewardAmount = 5; // Mukofot miqdori

    [Header("UI Elements")]
    public TextMeshProUGUI countdownText; // UIdagi matn uchun

    private DateTime lastRewardTime;
    //private TimeSpan rewardInterval = TimeSpan.FromHours(24); // 24 soat
    private TimeSpan rewardInterval = TimeSpan.FromHours(24);
    public Button ClaimReward;
    public Animator animator;
    public RewerdCollectionGold goldcollect;


    private void Start()
    {
        // So'nggi mukofot olingan vaqtni yuklash
        string lastRewardTimeString = PlayerPrefs.GetString("LastRewardTime2", DateTime.MinValue.ToString());
        lastRewardTime = DateTime.Parse(lastRewardTimeString);

        CheckRewardAvailability();
        InvokeRepeating("CheckRewardAvailability", 1f, 1f); // Har 60 soniyada tekshiradi
    }

    private void Update()
    {
        //CheckRewardAvailability();
        UpdateCountdownUI();
    }

    private void UpdateCountdownUI()
    {
        DateTime currentTime = NetworkTimeManager.Instance.GetCurrentNetworkTime();
        TimeSpan timeLeft = rewardInterval - (currentTime - lastRewardTime);

        if (timeLeft.TotalSeconds > 0)
        {
            countdownText.text = $"{timeLeft.Hours:D2}:{timeLeft.Minutes:D2}:{timeLeft.Seconds}";
        }
        else
        {
            countdownText.text = "00:00:00";
        }
    }


    private void CheckRewardAvailability()
    {
        DateTime currentTime = NetworkTimeManager.Instance.GetCurrentNetworkTime();

        if (currentTime - lastRewardTime >= rewardInterval)
        {
            animator.SetBool("isReady", true);
            ClaimReward.enabled = true;
            countdownText.enabled = false;
            
            //GiveReward();
        }
        else
        {
            animator.SetBool("isReady", false);
            ClaimReward.enabled = false;
            countdownText.enabled = true;
        }
    }

    public void GiveReward()
    {
        goldcollect.OnButtonClickedGold();

        // O'yinchiga mukofotni bering
        Debug.Log($"Reward granted: {rewardAmount} coins!");

        // O'yinchiga mukofotni qo'shing (o'yinchining tanga hisobini yangilash)
        GameManager.Instance.gold += rewardAmount; 
        ClaimReward.enabled = false;
        // So'nggi mukofot vaqtini yangilang
        lastRewardTime = NetworkTimeManager.Instance.GetCurrentNetworkTime();
        countdownText.enabled = true;
        animator.SetBool("isReady", false);
        PlayerPrefs.SetString("LastRewardTime2", lastRewardTime.ToString());
        PlayerPrefs.Save();
    }
}
