using System.Collections;
using UnityEngine;

public class CollectibleInteractable : MonoBehaviour, IInteractable
{
    [Header("Identity")]
    [SerializeField] private string collectibleId;
    [SerializeField] private string prompt = "Collect";

    [Header("Collect Animation")]
    [SerializeField] private string playerAnimTrigger = "Collect";
    [SerializeField] private float lockMovementSeconds = 0.6f;
    [SerializeField] private bool autoSaveOnCollect = true;

    private bool collectedThisSession;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(collectibleId))
            collectibleId = System.Guid.NewGuid().ToString();
    }
#endif

    void Start()
    {
        var prog = CollectibleProgress.Instance;
        if (prog != null && prog.IsCollected(collectibleId))
            gameObject.SetActive(false);
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (collectedThisSession) return;

        var prog = CollectibleProgress.Instance;
        if (prog == null) return;

        if (!prog.TryCollect(collectibleId))
            return;

        collectedThisSession = true;

        // Trigger player animation + lock movement briefly
        if (interactor != null)
            StartCoroutine(PlayCollectOnPlayer(interactor));

        // Remove collectible immediately (or after delay if you prefer)
        gameObject.SetActive(false);

        if (autoSaveOnCollect && SaveHandler.instance != null)
            SaveHandler.instance.SaveGame();
    }

    public string GetPrompt() => prompt;

    private IEnumerator PlayCollectOnPlayer(PlayerInteractor interactor)
    {
        var controller = interactor.ControllerCore;
        if (controller != null)
            controller.lockMovement = true;

        var anim = interactor.GetComponentInParent<Animator>();
        if (anim != null && !string.IsNullOrEmpty(playerAnimTrigger))
            anim.SetTrigger(playerAnimTrigger);

        yield return new WaitForSeconds(lockMovementSeconds);

        if (controller != null)
            controller.lockMovement = false;
    }
}
