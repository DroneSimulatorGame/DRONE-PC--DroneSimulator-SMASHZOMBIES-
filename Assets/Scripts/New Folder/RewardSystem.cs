using UnityEngine;
using TMPro;
using System.Collections;

public class RewardSystem : MonoBehaviour
{

    public TextMeshProUGUI steelPanel;  // UI for showing rewards/stars
    public TextMeshProUGUI xpPanel;    // An array to store rewards based on stars, e.g., rewards[0] = 1-star reward, rewards[1] = 2-star reward, etc.

    private int steel = 0;
    private int xp = 0;
    private int xpLevel = 1;
    public int stars = 0;
    public float delay = 0.5f;

    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    private int loseSteel = 500;
    public TextMeshProUGUI LoseSteelPanel;
    public GoldToSteelConverter converter;

    public GameObject losestar1;
    public GameObject losestar2;
    public GameObject losestar3;

    public XPManager xpManager;

    public GameObject xpStar1;
    public GameObject xpStar2;
    public GameObject xpStar3;

    private int buildingLayer;
    private int initialBuildingCount;
    private int remainingBuildingCount;

    public bool headquarterActive = true;

    private void Start()
    {
        buildingLayer = LayerMask.NameToLayer("Building");  // Ensure that "Building" is the correct name of your layer
        xpLevel = xpManager.xpLevel;
    }

    // Call this function when the wave starts
    public void StartWave()
    {
        // Count all objects on the "Building" layer at the start of the wave
        initialBuildingCount = CountObjectsOnLayer(buildingLayer);
    }

    // Call this function when the wave ends
    public void EndWave()
    {
        // Count all objects on the "Building" layer at the end of the wave
        remainingBuildingCount = CountObjectsOnLayer(buildingLayer);

        // Calculate the percentage of remaining buildings
        float remainingPercentage = (float)remainingBuildingCount / initialBuildingCount * 100;

        // Determine the number of stars based on the remaining percentage
        stars = CalculateStars(remainingPercentage);
        // Display the star rating and reward

        if (headquarterActive)
        {
            CalculateRewards(stars);
            ActivateStars();
            DisplayRewards();
            DisplayXP();
            //Debug.Log("you win!!!!");
        }
        else { DisplayLoseRewards(); headquarterActive = true; }

    }



    // Function to count objects on a specific layer
    private int CountObjectsOnLayer(int layer)
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int count = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == layer)
            {
                count++;
            }
        }

        return count;
    }

    // Function to calculate stars based on the remaining percentage
    private int CalculateStars(float remainingPercentage)
    {
        if (remainingPercentage <= 0)
        {
            return 0;
        }
        else if (remainingPercentage <= 50)
        {
            return 1;  // 1 star for <= 50%
        }
        else if (remainingPercentage <= 80)
        {
            return 2;  // 2 stars for <= 80%
        }
        else
        {
            return 3;  // 3 stars for > 80%
        }
    }

    // Function to display the rewards based on the number of stars
    private void CalculateRewards(int stars)
    {
        if (stars == 1)
        {
            steel = 700;
            xp = 50;
        }
        else if (stars == 2)
        {
            steel = 1000;
            xp = 100;
        }
        else if (stars == 3)
        {
            steel = 1500;
            xp = 150;
        }
        else
        {
            loseSteel = 500;
            xp = 0;
            losestar1.SetActive(false);
            losestar2.SetActive(false);
            losestar3.SetActive(false);
        }

        //Debug.Log("You earned " + stars + " star(s)!");
    }

    private void DisplayRewards()
    {

        GameManager.Instance.steel += steel;
        steelPanel.text = $"{steel}";
        xpPanel.text = $"{xp}";
        converter.UpdateBalance();
    }


    public void DisplayLoseRewards()
    {

        GameManager.Instance.steel += loseSteel;
        LoseSteelPanel.text = $"{loseSteel}";
        converter.UpdateBalance();
    }

    private void XPUpdate()
    {
        xpManager.value += xp;
        if (xpManager.value >= 1000)
        {
            xpLevel += 1;
            xpManager.value -= 1000;
            xpManager.xpLevel = xpLevel;
        }
        return;
    }
    private void DisplayXP()
    {
        XPUpdate();
        if (xpLevel == 1)
        {
            xpStar1.SetActive(true);
            xpStar2.SetActive(false);
            xpStar3.SetActive(false);
        }
        else if (xpLevel == 2)
        {
            xpStar1.SetActive(false);
            xpStar2.SetActive(true);
            xpStar3.SetActive(false);
        }
        else if (xpLevel == 3)
        {
            xpStar1.SetActive(false);
            xpStar2.SetActive(false);
            xpStar3.SetActive(true);
        }
    }
    public void ActivateStars()
    {
        // Deactivate all stars initially
        star1.SetActive(false);
        star2.SetActive(false);
        star3.SetActive(false);

        // Activate the stars based on the count
        StartCoroutine(ActivateStarsCoroutine());
    }

    private IEnumerator ActivateStarsCoroutine()
    {
        if (stars >= 1)
        {
            yield return new WaitForSeconds(delay);
            star1.SetActive(true);
        }

        if (stars >= 2)
        {
            yield return new WaitForSeconds(delay);
            star2.SetActive(true);
        }

        if (stars >= 3)
        {
            yield return new WaitForSeconds(delay);
            star3.SetActive(true);
        }
    }
}
