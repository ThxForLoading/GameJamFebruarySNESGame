using UnityEngine;

public class SaveSlotHighlighter : MonoBehaviour
{
    [SerializeField] GameObject highlight;

    public void HighlightThisSlot()
    {
        highlight.SetActive(true);
    }

    public void DehighlightThisSlot()
    {
        highlight.SetActive(false);
    }
}
