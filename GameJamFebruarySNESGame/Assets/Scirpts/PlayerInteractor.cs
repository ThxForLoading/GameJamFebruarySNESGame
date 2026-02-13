using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private PlayerControllerCore controllerCore;

    public PlayerControllerCore ControllerCore { get { return controllerCore; } }

    private IInteractable current;

    void OnEnable() => interactAction.action.Enable();
    void OnDisable() => interactAction.action.Disable();



    void Update()
    {
        if (interactAction.action.WasPressedThisFrame() && current != null)
        {
            current.Interact(this);
        }  
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
            current = interactable;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null && ReferenceEquals(current, interactable))
            current = null;
    }
}
