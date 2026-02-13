using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }
    public GameObject root;      
    public TMP_Text bodyText;

    public float wordDelay = 0.15f;

    private Coroutine typingRoutine;
    private bool isTyping;

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
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        root.SetActive(true);
        typingRoutine = StartCoroutine(TypeWords(text));
    }

    public void Hide()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        root.SetActive(false);
        isTyping = false;
    }

    public void Toggle(string text)
    {
        if (root.activeSelf) Hide();
        else Show(text);
    }
    private IEnumerator TypeWords(string text)
    {
        isTyping = true;
        bodyText.text = "";

        string[] words = text.Split(' ');

        for (int i = 0; i < words.Length; i++)
        {
            bodyText.text += words[i];

            if (i < words.Length - 1)
                bodyText.text += " ";

            yield return new WaitForSeconds(wordDelay);
        }

        isTyping = false;
    }
    public bool IsOpen => root != null && root.activeSelf;
}
