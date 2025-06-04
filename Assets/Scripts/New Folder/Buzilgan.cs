using UnityEngine;

public class Buzilgan : MonoBehaviour
{
    [SerializeField] private GameObject buildingPrefab;  // Make sure this is assigned in the Inspector
    public int cost;
    public string objectName;

    public int index = 0;

    void Start()
    {
        if (buildingPrefab == null)
        {
            Debug.LogError("Building prefab is not assigned in the Inspector.");
        }
        index = gameObject.GetComponent<Building>().index;
    }

    // Update is called once per frame
    void Update()
    {
        //if (gameObject.name == "1Shtab" || gameObject.name == "2Shtab" || gameObject.name == "3Shtab")
        //{
        //    if ()
        //    {

        //    }
        //}
        // Detect mouse clicks
        if (Input.GetMouseButtonDown(0) && ForUi.UInstance.TopMenuePanel.activeSelf)  // Left mouse button click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the clicked object is this building
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && !ForUi.UInstance.Repair.activeSelf)
                {
                    Debug.Log("Hit Object: " + hit.collider.gameObject.name);

                    if (hit.collider.gameObject == gameObject)  // If the clicked object is this building
                    {
                        TakeStats();
                        //GameManager.Instance.steel -= cost;
                        //ReplaceBuilding();
                    }

                }
                else
                {
                    Debug.LogWarning("No object was hit by the raycast.");
                }
            }
        }
    }

    private void TakeStats()
    {
        ShowRestoreUI.Instance.destroyedObject = gameObject;
        ShowRestoreUI.Instance.nameText.text = objectName;
        ShowRestoreUI.Instance.costText.text = $"{cost}";
        ShowRestoreUI.Instance.cost = cost;
        ShowRestoreUI.Instance.topMenuUI.SetActive(false);
        ShowRestoreUI.Instance.restoreUI.SetActive(true);
    }

    // Function to replace the building
    public void ReplaceBuilding()
    {
        if (buildingPrefab != null)
        {
            // Get the current position and rotation of the broken building
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            GoldToSteelConverter converter = FindObjectOfType<GoldToSteelConverter>();
            converter.UpdateBalance();

            //restoreSound.Play();
            // Instantiate a new building prefab at the position and rotation of the current building
            if (buildingPrefab.layer != 12) { ParticleSystemManager.Instance.PlayBuildRestore(index); }
            else { ParticleSystemManager.Instance.PlayWallRestore(transform.position, transform.rotation); }
            GameObject building = Instantiate(buildingPrefab, position, rotation);

            Building buildingScript = building.GetComponent<Building>();
            if (buildingScript != null)
            {
                buildingScript.SetIndex(index);
            }
            // Destroy the broken building (this GameObject)
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Building prefab is not set!");
        }
    }

    //public void SetIndex(int buildingIndex)
    //{
    //    index = buildingIndex;
    //}
}
