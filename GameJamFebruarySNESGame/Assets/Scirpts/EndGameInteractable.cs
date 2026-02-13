using UnityEngine;

public class EndGameInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private int requiredCount = 5;
    [SerializeField] private string creditsSceneName = "Credits";

    GameObject sceneHandler;
    private void Awake()
    {
        sceneHandler = GameObject.FindGameObjectWithTag("SceneHandler");
    }

    public void Interact(PlayerInteractor interactor)
    {
        var prog = CollectibleProgress.Instance;
        if (prog == null) return;

        if (prog.TotalCollected < requiredCount)
        {
            Debug.Log($"Need {requiredCount - prog.TotalCollected} more.");
            return;
        }
        
        if (sceneHandler != null)
        {
            sceneHandler.GetComponent<SceneHandler>().LoadScene(creditsSceneName);
        }
    }

    public string GetPrompt()
    {
        var prog = CollectibleProgress.Instance;
        if (prog == null) return "Locked";
        return prog.TotalCollected >= requiredCount ? "End Game" : "Locked";
    }
}
