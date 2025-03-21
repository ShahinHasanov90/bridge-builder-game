using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject beamPrefab;
    public GameObject jointPrefab;
    public GameObject cablePrefab;

    [Header("UI Elements")]
    public Text budgetText;
    public Text strengthText;
    public Button testButton;
    public Button resetButton;

    [Header("Game Settings")]
    public float initialBudget = 1000f;
    public float currentBudget;
    public bool isBuilding = true;
    public bool isTesting = false;

    private List<BridgeElement> bridgeElements = new List<BridgeElement>();
    private BridgeElement.ElementType selectedElementType;
    private Camera mainCamera;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mainCamera = Camera.main;
        currentBudget = initialBudget;
        UpdateUI();
    }

    void Update()
    {
        if (isBuilding && !isTesting)
        {
            HandleBuilding();
        }
    }

    void HandleBuilding()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            PlaceElement(mousePosition);
        }
    }

    public void SelectElementType(int type)
    {
        selectedElementType = (BridgeElement.ElementType)type;
    }

    void PlaceElement(Vector2 position)
    {
        GameObject prefab = null;
        switch (selectedElementType)
        {
            case BridgeElement.ElementType.Beam:
                prefab = beamPrefab;
                break;
            case BridgeElement.ElementType.Joint:
                prefab = jointPrefab;
                break;
            case BridgeElement.ElementType.Cable:
                prefab = cablePrefab;
                break;
        }

        if (prefab != null)
        {
            GameObject newElement = Instantiate(prefab, position, Quaternion.identity);
            BridgeElement bridgeElement = newElement.GetComponent<BridgeElement>();
            bridgeElement.Initialize(selectedElementType);

            if (currentBudget >= bridgeElement.cost)
            {
                currentBudget -= bridgeElement.cost;
                bridgeElements.Add(bridgeElement);
                bridgeElement.Place();
                UpdateUI();
            }
            else
            {
                Destroy(newElement);
                Debug.Log("Yetersiz bütçe!");
            }
        }
    }

    public void StartTest()
    {
        isTesting = true;
        foreach (var element in bridgeElements)
        {
            if (element != null)
            {
                element.Place();
            }
        }
    }

    public void ResetLevel()
    {
        isTesting = false;
        currentBudget = initialBudget;
        foreach (var element in bridgeElements)
        {
            if (element != null)
            {
                Destroy(element.gameObject);
            }
        }
        bridgeElements.Clear();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (budgetText != null)
        {
            budgetText.text = $"Bütçe: {currentBudget:F2}";
        }
    }
}