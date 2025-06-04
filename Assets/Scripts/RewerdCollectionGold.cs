using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RewerdCollectionGold : MonoBehaviour
{
    public GameObject steelPrefab; // Prefab for the steel object
    public Transform SteelParent; // Parent transform for instantiating steel objects
    public Transform SteelStart; // Starting position of the steel objects
    public Transform SteelEnd; // Ending position of the steel objects
    public float movementDuration; // Duration of the movement animation
    public Ease moveEase; // Easing for movement animation
    public int steelAmount; // Amount of steel to collect
    public float steelPerDelay; // Delay per steel collection
    public float totalDelay; // Total delay for all collections
    public float overallDelay; // Overall delay before starting the collection
    public AudioSource audioSource; // Reference to the AudioSource for sound effects

    public void OnButtonClickedGold()
    {
        StartCoroutine(ExecuteAfterDelay(overallDelay));
    }

    private IEnumerator ExecuteAfterDelay(float delay)
    {
        // Wait for the specified overall delay
        yield return new WaitForSeconds(delay);

        // Play the audio source sound effect
        audioSource.Play();

        // Calculate the delay per steel item
        steelPerDelay = totalDelay / steelAmount;

        // Start showing steel items
        for (int i = 0; i < steelAmount; i++)
        {
            var targetDelay = i * steelPerDelay;
            ShowSteel(targetDelay);
        }
    }

    public void ShowSteel(float delay)
    {
        var steelObject = Instantiate(steelPrefab, SteelParent);
        var offset = new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), 0f);
        var startPos = offset + SteelStart.position;
        steelObject.transform.position = startPos;

        // Use the original scale of the prefab
        steelObject.transform.localScale = new Vector3(.1f, .1f, .1f);
        steelObject.transform.DOScale(Vector3.one, delay);

        // Move the steel object and destroy it after the animation is complete
        steelObject.transform.DOMove(SteelEnd.position, movementDuration)
            .SetEase(moveEase)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                // Destroy the cloned steel object after the animation completes
                Destroy(steelObject);
            });
    }
}
