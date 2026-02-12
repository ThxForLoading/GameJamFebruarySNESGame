using System.IO;
using Unity.Cinemachine;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveHandler : MonoBehaviour
{
    private string saveLocation;
    public string saveName = "saveSlot";
    public int currentSlot = 0;

    public static SaveHandler instance;

    private float sessionPlaytime = 0;

    private SaveData pendingLoadData;

    private void Awake()
    {
        if (instance == null)           //GRRR singleton
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (pendingLoadData == null) return;

        ApplySaveData(pendingLoadData);
        pendingLoadData = null;
    }

    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, saveName + currentSlot + ".json");
    }

    private void Update()
    {
        sessionPlaytime += Time.deltaTime;
    }

    public void SaveGame()
    {
        var progress = CollectibleProgress.Instance;

        SaveData saveData = new SaveData()
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform.position,
            mapBoundary = FindFirstObjectByType<CinemachineConfiner2D>().BoundingShape2D.gameObject.name,
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            playTime = sessionPlaytime,

            totalCollected = progress ? progress.TotalCollected : 0,
            collectedIds = progress ? progress.ExportIds() : null
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData, true));
    }

    public void LoadGame()
    {
        if (!File.Exists(saveLocation))
        {
            Debug.Log($"No save found in slot {currentSlot}, creating new one.");
            SaveGame();
            return;
        }

        SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));

        GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPos;

        FindFirstObjectByType<CinemachineConfiner2D>().BoundingShape2D = GameObject.Find(saveData.mapBoundary).GetComponent<PolygonCollider2D>();

        sessionPlaytime = saveData.playTime;

        if (CollectibleProgress.Instance != null)
        {
            CollectibleProgress.Instance.ImportIds(saveData.collectedIds);
        } 
    }

    public SaveData GetSaveDataForSlot(int slot)
    {
        string path = Path.Combine(Application.persistentDataPath, saveName + slot + ".json");

        if (!File.Exists(path))
            return null;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public void BeginLoadFromSlot(int slot)
    {
        currentSlot = slot;
        pendingLoadData = GetSaveDataForSlot(slot);
    }

    void ApplySaveData(SaveData data)
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = data.playerPos;

        FindFirstObjectByType<CinemachineConfiner2D>().BoundingShape2D = GameObject.Find(data.mapBoundary).GetComponent<PolygonCollider2D>();

        sessionPlaytime = data.playTime;

        if (CollectibleProgress.Instance != null)
        {
            CollectibleProgress.Instance.ImportIds(data.collectedIds);
        }
            
    }
}
