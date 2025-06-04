using UnityEngine;
using TMPro; // Add TextMeshPro namespace

public class ZombieGoldDrop : MonoBehaviour
{
    public int minGoldAmount = 1;
    public int maxGoldAmount = 5;

    // Add references to GameObject and TextMeshPro


    private Animator animator;
    private bool isOpened = false;
    private int goldAmount;

    private void Awake()
    {
        animator = GetComponent<Animator>();

    }

    void Start()
    {
        // Determine gold amount at start
        goldAmount = Random.Range(minGoldAmount, maxGoldAmount + 1);
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
        GameManager.Instance.gold += goldAmount;
        Debug.Log($"Gold Drop: +{goldAmount} Gold");
        WaveManager.Instance.ShowGoldDropReward(goldAmount);
        Destroy(gameObject, 2f);
    }
}