using UnityEngine;
public class CheckUI : MonoBehaviour
{
    [Header("Game Panels")]
    [SerializeField] private GameObject[] gamePanels;
    private bool isAnyPanelOpen = false;
    private static CheckUI instance;
    public static CheckUI Instance { get { return instance; } }

    // Mouse visibility control variables
    private bool wasGameCursorLocked = false;
    [Header("Mouse Settings")]
    [SerializeField] private CursorLockMode gameplayCursorMode = CursorLockMode.Locked;
    [SerializeField] private bool gameplayCursorVisible = false;
    [SerializeField] private CursorLockMode uiCursorMode = CursorLockMode.None;
    [SerializeField] private bool uiCursorVisible = true;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        // Set initial cursor state
        UpdateCursorState(false);
    }

    private void Update()
    {
        bool previousPanelState = isAnyPanelOpen;
        CheckGamePanels();

        // If panel state changed, update cursor
        if (previousPanelState != isAnyPanelOpen)
        {
            UpdateCursorState(isAnyPanelOpen);
        }
    }

    private void CheckGamePanels()
    {
        isAnyPanelOpen = false;
        if (gamePanels != null && gamePanels.Length > 0)
        {
            foreach (GameObject panel in gamePanels)
            {
                if (panel != null && panel.activeInHierarchy)
                {
                    isAnyPanelOpen = true;
                    break;
                }
            }
        }
    }

    // Update cursor state based on UI visibility
    private void UpdateCursorState(bool uiActive)
    {
        if (uiActive)
        {
            // UI is active, show system cursor and unlock it
            Cursor.lockState = uiCursorMode;
            Cursor.visible = uiCursorVisible;
        }
        else
        {
            // No UI is active, use gameplay cursor settings
            Cursor.lockState = gameplayCursorMode;
            Cursor.visible = gameplayCursorVisible;
        }
    }

    public bool IsAnyPanelOpen()
    {
        return isAnyPanelOpen;
    }

    public void AddPanel(GameObject panel)
    {
        if (panel == null) return;
        GameObject[] newPanels = new GameObject[gamePanels.Length + 1];
        gamePanels.CopyTo(newPanels, 0);
        newPanels[gamePanels.Length] = panel;
        gamePanels = newPanels;
    }

    public void AddPanels(GameObject[] panels)
    {
        if (panels == null || panels.Length == 0) return;
        GameObject[] newPanels = new GameObject[gamePanels.Length + panels.Length];
        gamePanels.CopyTo(newPanels, 0);
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] != null)
            {
                newPanels[gamePanels.Length + i] = panels[i];
            }
        }
        gamePanels = newPanels;
    }

    // Manually toggle cursor mode
    public void SetUIMode(bool active)
    {
        UpdateCursorState(active);
    }
}