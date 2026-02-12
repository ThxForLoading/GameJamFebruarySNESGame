using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class SaveSlotHighlighter : MonoBehaviour
{
    [SerializeField] GameObject highlight;

    [SerializeField] private int slotIndex;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text sceneText;
    [SerializeField] private TMP_Text timeText;

    private void OnEnable()
    {
        ReloadText();
    }


    public void ReloadText()
    {
        SaveData data = SaveHandler.instance.GetSaveDataForSlot(slotIndex);

        if (data == null)
        {
            titleText.text = "Save Slot " + (slotIndex + 1);
            sceneText.text = "Stage: -";
            timeText.text = "Playtime: -";
        }
        else
        {
            titleText.text = "Save Slot " + (slotIndex + 1);
            sceneText.text = "Stage: " + data.sceneName;
            timeText.text = "Playtime: " + FormatTime(data.playTime);
        }
    }

    private string FormatTime(float seconds)
    {
        int hours = Mathf.FloorToInt(seconds / 3600);
        int minutes = Mathf.FloorToInt((seconds % 3600) / 60);
        return hours + ":" + minutes;
    }


    public void HighlightThisSlot()
    {
        highlight.SetActive(true);
    }

    public void DehighlightThisSlot()
    {
        highlight.SetActive(false);
    }
}
