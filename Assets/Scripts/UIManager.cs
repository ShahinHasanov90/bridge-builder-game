using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject mainMenuPanel;
    public GameObject gamePanel;
    public GameObject levelCompletePanel;
    public GameObject gameOverPanel;

    [Header("Game UI Elements")]
    public TextMeshProUGUI budgetText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI timerText;
    public Image strengthIndicator;

    [Header("Building Buttons")]
    public Button beamButton;
    public Button jointButton;
    public Button cableButton;
    public Button testButton;
    public Button resetButton;

    [Header("Menu Buttons")]
    public Button startButton;
    public Button settingsButton;
    public Button quitButton;
    public Button nextLevelButton;
    public Button retryButton;
    public Button menuButton;

    private GameManager gameManager;
    private LevelManager levelManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        levelManager = FindObjectOfType<LevelManager>();
        
        SetupButtonListeners();
        ShowMainMenu();
    }

    void SetupButtonListeners()
    {
        // Building buttons
        beamButton.onClick.AddListener(() => SelectBuildingElement(0));
        jointButton.onClick.AddListener(() => SelectBuildingElement(1));
        cableButton.onClick.AddListener(() => SelectBuildingElement(2));
        testButton.onClick.AddListener(() => StartTest());
        resetButton.onClick.AddListener(() => ResetLevel());

        // Menu buttons
        startButton.onClick.AddListener(() => StartGame());
        settingsButton.onClick.AddListener(() => OpenSettings());
        quitButton.onClick.AddListener(() => QuitGame());
        nextLevelButton.onClick.AddListener(() => LoadNextLevel());
        retryButton.onClick.AddListener(() => RetryLevel());
        menuButton.onClick.AddListener(() => ShowMainMenu());
    }

    void SelectBuildingElement(int elementType)
    {
        gameManager.SelectElementType((BridgeElement.ElementType)elementType);
        
        // Görsel geri bildirim
        beamButton.GetComponent<Image>().color = elementType == 0 ? Color.green : Color.white;
        jointButton.GetComponent<Image>().color = elementType == 1 ? Color.green : Color.white;
        cableButton.GetComponent<Image>().color = elementType == 2 ? Color.green : Color.white;
    }

    public void UpdateBudgetText(float budget)
    {
        budgetText.text = $"Bütçe: {budget:F0} ₺";
    }

    public void UpdateLevelText(int level)
    {
        levelText.text = $"Seviye {level + 1}";
    }

    public void UpdateTimer(float time)
    {
        timerText.text = $"Süre: {time:F1}";
    }

    public void UpdateStrengthIndicator(float strength)
    {
        strengthIndicator.fillAmount = strength / 100f;
        strengthIndicator.color = Color.Lerp(Color.red, Color.green, strength / 100f);
    }

    void StartTest()
    {
        testButton.interactable = false;
        levelManager.StartLevelTest();
    }

    void ResetLevel()
    {
        testButton.interactable = true;
        gameManager.ResetLevel();
    }

    void StartGame()
    {
        HideAllPanels();
        gamePanel.SetActive(true);
        gameManager.StartNewGame();
    }

    void OpenSettings()
    {
        // Ayarlar panelini aç
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void LoadNextLevel()
    {
        HideAllPanels();
        gamePanel.SetActive(true);
        levelManager.LoadNextLevel();
    }

    void RetryLevel()
    {
        HideAllPanels();
        gamePanel.SetActive(true);
        levelManager.RetryLevel();
    }

    void ShowMainMenu()
    {
        HideAllPanels();
        mainMenuPanel.SetActive(true);
    }

    public void ShowLevelComplete()
    {
        HideAllPanels();
        levelCompletePanel.SetActive(true);
    }

    public void ShowGameOver()
    {
        HideAllPanels();
        gameOverPanel.SetActive(true);
    }

    void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        gamePanel.SetActive(false);
        levelCompletePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }
}