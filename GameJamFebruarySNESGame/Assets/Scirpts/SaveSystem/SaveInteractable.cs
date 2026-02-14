using UnityEngine;

public class SaveInteractable : MonoBehaviour, IInteractable
{
    public string prompt = "Save";

    [SerializeField] SaveInteractPlayer  saveInteract;
    public void Interact(PlayerInteractor interactor)
    {
        saveInteract.BeginSaveInteraction();
        
    }

    public string GetPrompt() => prompt;
}
