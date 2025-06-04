using UnityEngine;
using TMPro; // Add TextMeshPro namespace

public class ZombieSteelDrop : MonoBehaviour
{
    public int minSteelAmount = 1;
    public int maxSteelAmount = 3;

    // Add references to GameObject and TextMeshPro



    private Animator animator;
    private bool isOpened = false;
    private int steelAmount;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Determine steel amount at start
        steelAmount = Random.Range(minSteelAmount, maxSteelAmount + 1);
    }

    void OnMouseDown()
    {
        if (isOpened) return;
        isOpened = true;

        // Play animation
        if (animator != null)
            animator.SetTrigger("Open");


        // Reward and destroy will be delayed slightly to wait for the animation to finish
        Invoke(nameof(CollectReward), 0f); // adjust delay to your animation length
    }

    void CollectReward()
    {
        GameManager.Instance.steel += steelAmount;
        Debug.Log($"Steel Drop: +{steelAmount} Steel");
        WaveManager.Instance.ShowSteelDropReward(steelAmount);
        Destroy(gameObject, 2f);
    }
}