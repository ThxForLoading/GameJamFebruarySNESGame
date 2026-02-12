using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }
    public GameObject root;      
    public TMP_Text bodyText;    

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Hide();
    }

    public void Show(string text)
    {
        if (bodyText) bodyText.text = text;
        if (root) root.SetActive(true);
    }

    public void Hide()
    {
        if (root) root.SetActive(false);
    }

    public void Toggle(string text)
    {
        if (!root) return;
        if (root.activeSelf) Hide();
        else Show(text);
    }

    public bool IsOpen => root != null && root.activeSelf;
}
