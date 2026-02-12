using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPos;
    public string mapBoundary;

    public string sceneName;

    public float playTime;

    public int totalCollected;
    public string[] collectedIds;
    
    //Additional stuff
}
