using System.IO;
using Unity.Cinemachine;
using UnityEngine;

public class SaveHandler : MonoBehaviour
{
    private string saveLocation;
    public string saveName = "saveData";

    public static SaveHandler instance;

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

    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, saveName + ".json");
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData()
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform.position,
            mapBoundary = FindFirstObjectByType<CinemachineConfiner2D>().BoundingShape2D.gameObject.name
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));

            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPos;

            FindFirstObjectByType<CinemachineConfiner2D>().BoundingShape2D = GameObject.Find(saveData.mapBoundary).GetComponent<PolygonCollider2D>();
        }
        else
        {
            SaveGame();
        }
    }
}
