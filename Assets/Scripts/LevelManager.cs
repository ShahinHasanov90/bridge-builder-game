using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public float targetDistance;
        public float maxBudget;
        public float minStrength;
        public Vector2 startPoint;
        public Vector2 endPoint;
    }

    public LevelData[] levels;
    public int currentLevel = 0;
    public GameObject vehiclePrefab;
    public float vehicleSpeed = 5f;
    
    private bool isLevelComplete = false;
    private GameObject currentVehicle;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        SetupLevel(currentLevel);
    }

    void SetupLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("Geçersiz seviye indeksi!");
            return;
        }

        LevelData level = levels[levelIndex];
        gameManager.initialBudget = level.maxBudget;
        gameManager.ResetLevel();

        // Başlangıç ve bitiş noktalarını oluştur
        CreateStartEndPoints(level.startPoint, level.endPoint);
    }

    void CreateStartEndPoints(Vector2 start, Vector2 end)
    {
        // Başlangıç ve bitiş platformlarını oluştur
        GameObject startPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject endPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);

        startPlatform.transform.position = start;
        endPlatform.transform.position = end;

        // Platform boyutlarını ayarla
        startPlatform.transform.localScale = new Vector3(2f, 0.5f, 1f);
        endPlatform.transform.localScale = new Vector3(2f, 0.5f, 1f);

        // Platformları statik yap
        startPlatform.AddComponent<BoxCollider2D>();
        endPlatform.AddComponent<BoxCollider2D>();
        
        Rigidbody2D startRb = startPlatform.AddComponent<Rigidbody2D>();
        Rigidbody2D endRb = endPlatform.AddComponent<Rigidbody2D>();
        
        startRb.isKinematic = true;
        endRb.isKinematic = true;
    }

    public void StartLevelTest()
    {
        if (!gameManager.isTesting)
        {
            gameManager.StartTest();
            StartCoroutine(TestLevel());
        }
    }

    IEnumerator TestLevel()
    {
        yield return new WaitForSeconds(1f); // Köprünün yerleşmesi için bekle

        // Test aracını oluştur
        currentVehicle = Instantiate(vehiclePrefab, levels[currentLevel].startPoint, Quaternion.identity);
        Rigidbody2D vehicleRb = currentVehicle.GetComponent<Rigidbody2D>();
        
        // Aracı hareket ettir
        vehicleRb.velocity = new Vector2(vehicleSpeed, 0);

        // Test sonucunu bekle
        StartCoroutine(CheckTestResult());
    }

    IEnumerator CheckTestResult()
    {
        float timeout = 10f; // Test için maksimum süre
        float timer = 0f;

        while (timer < timeout && !isLevelComplete && currentVehicle != null)
        {
            // Aracın pozisyonunu kontrol et
            if (Vector2.Distance(currentVehicle.transform.position, levels[currentLevel].endPoint) < 1f)
            {
                LevelComplete();
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (!isLevelComplete)
        {
            LevelFailed();
        }
    }

    void LevelComplete()
    {
        isLevelComplete = true;
        Debug.Log("Seviye tamamlandı!");
        
        // Sonraki seviyeye geç
        currentLevel++;
        if (currentLevel < levels.Length)
        {
            StartCoroutine(PrepareNextLevel());
        }
        else
        {
            Debug.Log("Tüm seviyeler tamamlandı!");
        }
    }

    void LevelFailed()
    {
        Debug.Log("Seviye başarısız!");
        if (currentVehicle != null)
        {
            Destroy(currentVehicle);
        }
        gameManager.ResetLevel();
    }

    IEnumerator PrepareNextLevel()
    {
        yield return new WaitForSeconds(2f);
        SetupLevel(currentLevel);
    }
}