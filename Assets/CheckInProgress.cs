using UnityEngine;
using System.Collections.Generic;

public class CheckInProgress : MonoBehaviour
{
    public GameObject inProgressButton; // Reference to the "InProgress" button
    public GameObject playButton;       // Reference to the "Play" button
    private float timerForCheckDestroyed = 2f;

    private bool isThereDestroyed;
    private bool isThereProgress;

    // List to manage in-progress items
    private List<string> inProgressList = new List<string>();

    private void Start()
    {
        CheckForDestroyed();
    }


    private void Update()
    {
        if (timerForCheckDestroyed <= 0) 
        {   
            CheckForDestroyed(); 
            timerForCheckDestroyed = 2f;
            if (isThereDestroyed || isThereProgress)
            {
                inProgressButton.SetActive(true);
                playButton.SetActive(false);
            }
            else 
            {
                inProgressButton.SetActive(false);
                playButton.SetActive(true);
            }
        }
        else { timerForCheckDestroyed -= Time.deltaTime; }
    }
    // Method to add an element to the in-progress list
    public void AddToInProgressList(string item)
    {
        if (!inProgressList.Contains(item))
        {
            inProgressList.Add(item);
            CheckingProgress();
        }
        else { //Debug.Log("hammasi yaxshi");
               }
    }

    // Method to remove an element from the in-progress list
    public void RemoveFromInProgressList(string item)
    {
        if (inProgressList.Contains(item))
        {
            inProgressList.Remove(item);
            CheckingProgress();
        }
        else { //Debug.Log(item + " progressdan topilmadi");
               }
    }

    public void CheckingProgress()
    {
        // Check if the in-progress list has elements
        isThereProgress = inProgressList.Count > 0;
    }

    public void CheckForDestroyed()
    {
        Buzilgan[] buzilganlar = FindObjectsOfType<Buzilgan>();

        isThereDestroyed = buzilganlar.Length > 0;
    }

}
