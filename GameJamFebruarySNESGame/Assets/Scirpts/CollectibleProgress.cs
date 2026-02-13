using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleProgress : MonoBehaviour
{
    public static CollectibleProgress Instance { get; private set; }

    public event Action OnProgressLoaded;
    public event Action<int> OnTotalChanged; // passes new total

    private readonly HashSet<string> collected = new();
    public int TotalCollected => collected.Count;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool IsCollected(string id) => !string.IsNullOrEmpty(id) && collected.Contains(id);

    public bool TryCollect(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;

        bool added = collected.Add(id);
        if (added)
            OnTotalChanged?.Invoke(TotalCollected);

        return added;
    }

    public string[] ExportIds()
    {
        var arr = new string[collected.Count];
        collected.CopyTo(arr);
        return arr;
    }

    public void ImportIds(string[] ids)
    {
        collected.Clear();
        if (ids != null)
        {
            for (int i = 0; i < ids.Length; i++)
                if (!string.IsNullOrEmpty(ids[i]))
                    collected.Add(ids[i]);
        }

        OnProgressLoaded?.Invoke();
        OnTotalChanged?.Invoke(TotalCollected);
    }
}
