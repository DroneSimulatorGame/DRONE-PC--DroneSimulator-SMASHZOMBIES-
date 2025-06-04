using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class Timer : MonoBehaviour
{
    [Header("Timer UI references:")]
    [SerializeField] private Image uiFillImage;
    [SerializeField] private TextMeshProUGUI uiText; // TextMeshPro

    public int Duration { get; private set; }
    public bool IsPaused { get; private set; }
    private int remainingDuration;

    private UnityAction onTimerEndAction;

    private void Awake()
    {
        ResetTimer();
        gameObject.SetActive(false);  // Timer initially inactive
    }

    private void ResetTimer()
    {
        uiText.text = "100%"; // Display as percentage (initial state)
        uiFillImage.fillAmount = 1f;
        Duration = remainingDuration = 0;
        IsPaused = false;
    }

    public Timer SetTimerFromExternal(int seconds)
    {
        Duration = remainingDuration = seconds;
        gameObject.SetActive(true);  // Activate the timer when setting time
        return this;
    }

    public Timer OnEnd(UnityAction action)
    {
        onTimerEndAction = action;
        return this;
    }

    public void Begin()
    {
        StopAllCoroutines();
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        while (remainingDuration > 0)
        {
            remainingDuration--;
            UpdateUI(remainingDuration);
            yield return new WaitForSeconds(1f);
        }
        End();
    }

    private void UpdateUI(int seconds)
    {
        float percentage = (float)(Duration - seconds) / Duration;
        uiText.text = Mathf.CeilToInt(percentage * 100f) + "%";
        uiFillImage.fillAmount = percentage;
    }

    public void End()
    {
        if (onTimerEndAction != null)
            onTimerEndAction.Invoke();
        ResetAndHide();
    }

    public void ResetAndHide()
    {
        ResetTimer();
        gameObject.SetActive(false);  // Deactivate the timer when time is up
    }

    public void ReduceTime(int reduction)
    {
        remainingDuration = Mathf.Max(0, remainingDuration - reduction);
    }
}
