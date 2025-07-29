using UnityEngine;
using UnityEngine.UI; // Add this for UI components

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuUI;       // Assign your main menu panel or canvas here
    public GameObject[] gameplayObjects; // Assign all gameplay-related objects here to enable/disable
    public Button playButton;           // Assign your play button here

    // Static flag to control game state - ALL gameplay scripts should check this
    public static bool isGameStarted = false;

    void Start()
    {
        Debug.Log("MainMenu Start() called");

        // CRITICAL: Set game as not started FIRST
        isGameStarted = false;

        // Auto-find MainMenuUI if not assigned
        if (MainMenuUI == null)
        {
            Debug.Log("MainMenuUI not assigned, searching for it...");
            // Try to find by common names
            GameObject menuGO = GameObject.Find("MainMenuUI");
            if (menuGO == null) menuGO = GameObject.Find("MainMenu");
            if (menuGO == null) menuGO = GameObject.Find("Menu");
            if (menuGO == null) menuGO = GameObject.Find("UI");
            if (menuGO == null) menuGO = GameObject.Find("Canvas");

            if (menuGO != null)
            {
                MainMenuUI = menuGO;
                Debug.Log("Found MainMenuUI: " + menuGO.name);
            }
            else
            {
                Debug.LogError("Could not find MainMenuUI! Please assign it manually or name your menu GameObject 'MainMenuUI', 'MainMenu', 'Menu', 'UI', or 'Canvas'");
            }
        }

        // Show menu and pause game
        if (MainMenuUI != null)
        {
            MainMenuUI.SetActive(true);
            Debug.Log("MainMenuUI shown: " + MainMenuUI.name);
        }
        else
        {
            Debug.LogError("MainMenuUI is NULL! Cannot show menu!");
        }

        Time.timeScale = 0f;

        // Disable gameplay objects at start
        foreach (GameObject obj in gameplayObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // NUCLEAR OPTION: Disable ALL scripts except UI and this menu
        DisableAllGameplayScripts();

        // Auto-find play button if not assigned
        if (playButton == null)
        {
            Debug.Log("Play button not assigned, searching for it...");
            // Try to find button by name
            GameObject buttonGO = GameObject.Find("PlayButton");
            if (buttonGO == null) buttonGO = GameObject.Find("Play Button");
            if (buttonGO == null) buttonGO = GameObject.Find("Play");
            if (buttonGO == null) buttonGO = GameObject.Find("StartButton");
            if (buttonGO == null) buttonGO = GameObject.Find("Start");

            if (buttonGO != null)
            {
                playButton = buttonGO.GetComponent<Button>();
                Debug.Log("Found play button: " + buttonGO.name);
            }

            // If still null, try to find any button in the main menu UI
            if (playButton == null && MainMenuUI != null)
            {
                Button[] buttons = MainMenuUI.GetComponentsInChildren<Button>();
                if (buttons.Length > 0)
                {
                    playButton = buttons[0]; // Use the first button found
                    Debug.Log("Using first button found in MainMenuUI: " + playButton.gameObject.name);
                }
            }
        }

        // Connect the play button to StartGame method
        if (playButton != null)
        {
            playButton.onClick.AddListener(StartGame);
            Debug.Log("Play button connected successfully!");
        }
        else
        {
            Debug.LogError("Play button is NULL! Please assign it in the Inspector or make sure your button is named 'PlayButton', 'Play', 'StartButton', or 'Start'!");
        }
    }

    void DisableAllGameplayScripts()
    {
        // Find and disable ALL scripts that might be running gameplay
        MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour script in allScripts)
        {
            // Don't disable UI scripts, this script, or essential Unity components
            if (script != this &&
                !script.GetType().Name.Contains("UI") &&
                !script.GetType().Name.Contains("Canvas") &&
                !script.GetType().Name.Contains("GraphicRaycaster") &&
                !script.GetType().Name.Contains("EventSystem") &&
                !script.GetType().Name.Contains("Button") &&
                !script.GetType().Name.Contains("Image") &&
                !script.GetType().Name.Contains("Text") &&
                script.GetType() != typeof(MainMenu))
            {
                script.enabled = false;
                Debug.Log("Disabled script: " + script.GetType().Name + " on " + script.gameObject.name);
            }
        }
        Debug.Log("All gameplay scripts disabled - game is completely stopped!");
    }

    void EnableAllGameplayScripts()
    {
        // Re-enable all scripts that were disabled
        MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>(true); // Include inactive objects
        foreach (MonoBehaviour script in allScripts)
        {
            // Re-enable all scripts except this menu script and UI components
            if (script != this &&
                !script.GetType().Name.Contains("UI") &&
                !script.GetType().Name.Contains("Canvas") &&
                !script.GetType().Name.Contains("GraphicRaycaster") &&
                !script.GetType().Name.Contains("EventSystem") &&
                !script.GetType().Name.Contains("Button") &&
                !script.GetType().Name.Contains("Image") &&
                !script.GetType().Name.Contains("Text") &&
                script.GetType() != typeof(MainMenu))
            {
                script.enabled = true;
                Debug.Log("Re-enabled script: " + script.GetType().Name + " on " + script.gameObject.name);
            }
        }
        Debug.Log("All gameplay scripts re-enabled!");
    }

    void Update()
    {
        // Check for left mouse click to start game
        if (Input.GetMouseButtonDown(0)) // 0 = left mouse button
        {
            // Only start game if main menu is currently active
            if (MainMenuUI != null && MainMenuUI.activeInHierarchy)
            {
                StartGame();
            }
        }

        // Alternative: Check if mouse is over the button and clicked
        if (playButton != null && Input.GetMouseButtonDown(0))
        {
            // Check if mouse is over the button using RectTransform
            RectTransform buttonRect = playButton.GetComponent<RectTransform>();
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                buttonRect, Input.mousePosition, null, out mousePos);
            if (buttonRect.rect.Contains(mousePos))
            {
                Debug.Log("Mouse clicked on button area!");
                StartGame();
            }
        }
    }

    // Simple test method - try connecting your button to this first
    public void TestButtonClick()
    {
        Debug.Log("TEST BUTTON CLICKED - BUTTON WORKS!");
    }

    // Called when Play button is clicked
    public void StartGame()
    {
        Debug.Log("StartGame() called - Button is working!");

        // Set game as started
        isGameStarted = true;

        // Hide menu with detailed debugging
        if (MainMenuUI != null)
        {
            Debug.Log("Attempting to hide MainMenuUI: " + MainMenuUI.name + " (currently active: " + MainMenuUI.activeInHierarchy + ")");
            MainMenuUI.SetActive(false);
            Debug.Log("MainMenuUI hidden successfully! Now active: " + MainMenuUI.activeInHierarchy);
        }
        else
        {
            Debug.LogError("MainMenuUI is NULL! Cannot hide menu!");
        }

        // Enable gameplay objects
        foreach (GameObject obj in gameplayObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log("Enabled: " + obj.name);
            }
        }

        // Re-enable all gameplay scripts
        EnableAllGameplayScripts();

        // Resume game
        Time.timeScale = 1f;
        Debug.Log("Game started successfully! ALL systems now active.");
    }
}