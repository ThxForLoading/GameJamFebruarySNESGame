using UnityEngine;

public class ProgressSpriteReveal : MonoBehaviour
{
    [Header("Auto-fill from children")]
    [SerializeField] private bool autoFillFromChildren = true;

    [SerializeField] private SpriteRenderer[] sprites;

    private int lastTotal = -1;

    void Awake()
    {
        if ((sprites == null || sprites.Length == 0) && autoFillFromChildren)
        {
            sprites = GetComponentsInChildren<SpriteRenderer>(true);
        }
    }

    void Start()
    {
        Refresh();
    }

    void Update()
    {
        var prog = CollectibleProgress.Instance;
        if (prog == null) return;

        int total = prog.TotalCollected;
        if (total != lastTotal)
            Refresh();
    }

    private void Refresh()
    {
        var prog = CollectibleProgress.Instance;
        if (prog == null) return;

        int total = prog.TotalCollected;
        lastTotal = total;

        if (sprites == null || sprites.Length == 0) return;

        int revealCount = Mathf.Clamp(total, 0, sprites.Length);

        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] == null) continue;
            sprites[i].enabled = (i < revealCount);
        }
    }
}
