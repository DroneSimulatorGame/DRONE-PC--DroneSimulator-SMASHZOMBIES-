using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissleActivate : MonoBehaviour
{
    public GameObject missileLauncher;
    private Building building;
    public int cost = 100;
    public GameObject MissleUI;
    public GameObject NotEnough;
    public TextMeshProUGUI notEnoughText;
    public TextMeshProUGUI missleCostText;
    public GameObject beforeActivationUI;
    public GameObject afterActivationUI;




    private int missleLevel = 0;

    void Start()
    {
        missleLevel = PlayerPrefs.GetInt("MissleLevels7", 0);
        if (missleLevel > 0)
        {
            missileLauncher.GetComponent<MissileLauncher>().enabled = true;
            cost = 0;
            missleCostText.text = $"{cost}";
            beforeActivationUI.SetActive(false);
            afterActivationUI.SetActive(true);
        }
        else
        {
            missleCostText.text = $"{cost}";
            beforeActivationUI.SetActive(true);
            afterActivationUI.SetActive(false);
        }
    }

    public void ActivateMissle()
    {
        if (missleLevel > 0) return;

        if (GameManager.Instance.gold >= cost)
        {
            GameManager.Instance.gold -= cost;
            missileLauncher.GetComponent<MissileLauncher>().enabled = true;
            missleLevel = 1;
            cost = 0;
            missleCostText.text = $"{cost}";
            beforeActivationUI.SetActive(false);
            afterActivationUI.SetActive(true);
            PlayerPrefs.SetInt("MissleLevels6", missleLevel);
            ParticleSystemManager.Instance.MissilePurchased();
        }
        else
        {
            notEnoughText.text = $"{cost - GameManager.Instance.gold}";
            MissleUI.SetActive(false);
            NotEnough.SetActive(true);
            beforeActivationUI.SetActive(true);
            afterActivationUI.SetActive(false);
        }
    }
}
