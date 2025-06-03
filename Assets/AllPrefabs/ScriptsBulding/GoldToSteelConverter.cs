using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GoldToSteelConverter : MonoBehaviour
{
    public static GoldToSteelConverter Instance;
    public Slider goldToSteelSlider;
    public Button convertButton;
    public TMP_Text GoldText;
    public TMP_Text SteelText;

    public TMP_Text EXGoldText;
    public TMP_Text EXSteelText;

    private int gold;
    private int steel;
    private int previousSteel;
    private int exchangeRate = 500;
    private float nimadur = 5f;

    public float duration = 3f;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        UpdateBalance();
        UpdateSlider();

        // Make sure the exchange texts are initialized with initial values
        UpdateConversionText((int)goldToSteelSlider.value);

        goldToSteelSlider.onValueChanged.AddListener(value => UpdateConversionText((int)value));
        convertButton.onClick.AddListener(ConvertGoldToSteel);
    }

    private void Update()
    {
        if (gold != GameManager.Instance.gold || steel != GameManager.Instance.steel || nimadur <= 0)
        {
            UpdateBalance();
            nimadur = 5f;
        }
        else if (nimadur > 0f)
        {
            nimadur -= Time.deltaTime;
        }
    }

    public void UpdateBalance()
    {
        gold = GameManager.Instance.gold;
        steel = GameManager.Instance.steel;
        GoldText.text = $"{gold}";

        StartCoroutine(AnimateSteelIncrease(previousSteel, steel, duration));
        //SteelText.text = $"{steel}";

        // Update the slider after updating balance
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        // Preserve current slider value
        int currentValue = (int)goldToSteelSlider.value;

        goldToSteelSlider.maxValue = gold;

        // Set a reasonable initial value if the slider is at 0
        if (currentValue == 0 && gold > 0)
        {
            currentValue = Mathf.Min(gold, 1); // Start with at least 1 gold if possible
        }

        goldToSteelSlider.value = Mathf.Clamp(currentValue, 0, gold);
        UpdateConversionText((int)goldToSteelSlider.value);
    }

    private void UpdateConversionText(int value)
    {
        if (EXGoldText != null && EXSteelText != null)
        {
            EXGoldText.text = $"{value}";
            EXSteelText.text = $"{value * exchangeRate}";
        }
        else
        {
            //Debug.LogError("Conversion Text fields are not assigned in the inspector.");
        }
    }

    private void ConvertGoldToSteel()
    {
        int goldAmount = (int)goldToSteelSlider.value;

        if (goldAmount > 0 && goldAmount <= gold)
        {
            gold -= goldAmount;
            steel += goldAmount * exchangeRate;

            //Debug.Log($"{goldAmount} gold converted to {goldAmount * exchangeRate} steel. Total gold: {gold}, Total steel: {steel}");

            GameManager.Instance.gold = gold;
            GameManager.Instance.steel = steel;
            UpdateBalance();
            UpdateSlider();
        }
        else
        {
            //Debug.Log("Invalid conversion amount.");
        }
    }

    public void AddGold100()
    {
        AddGold(100);
    }

    public void AddGold300()
    {
        AddGold(300);
    }

    public void AddGold1000()
    {
        AddGold(1000);
    }

    public void AddGold10000()
    {
        AddGold(10000);
    }

    public void AddSteel200()
    {
        Add200(200);
    }

    public void AddGold(int amount)
    {
        gold += amount;
        GameManager.Instance.gold = gold;

        //Debug.Log($"{amount} gold added. Total gold: {gold}");

        UpdateSlider();
        UpdateBalance();
    }

    public void Add200(int amount)
    {
        steel += amount;
        GameManager.Instance.steel = steel;

        //Debug.Log($"{amount} gold added. Total gold: {steel}");

        UpdateSlider();
        UpdateBalance();
    }

    IEnumerator AnimateSteelIncrease(int previousSteel1, int steel, float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            int currentCoins = Mathf.RoundToInt(Mathf.Lerp(previousSteel1, steel, elapsedTime / duration));
            SteelText.text = currentCoins.ToString();
            yield return null;
        }
        SteelText.text = steel.ToString();
        previousSteel = steel;
    }
}