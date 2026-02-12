using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [TextArea(3, 8)]
    public string dialogueText = "Hello there!";

    public string prompt = "Talk";

    [Header("Behaviour")]
    public bool toggleDialogue = true;
    public bool lockPlayerMovementWhileOpen = true;

    public void Interact(PlayerInteractor interactor)
    {
        if (DialogueUI.Instance == null) return;

        if (toggleDialogue)
            DialogueUI.Instance.Toggle(dialogueText);
        else
            DialogueUI.Instance.Show(dialogueText);

        if (lockPlayerMovementWhileOpen && interactor != null && interactor.ControllerCore != null)
            interactor.ControllerCore.lockMovement = DialogueUI.Instance.IsOpen;
    }

    public string GetPrompt() => prompt;
}
