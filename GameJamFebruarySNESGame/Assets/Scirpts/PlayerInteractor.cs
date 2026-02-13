using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private PlayerControllerCore controllerCore;

    public PlayerControllerCore ControllerCore { get { return controllerCore; } }

    private IInteractable current;

    private GameObject audioManager;

    void OnEnable() => interactAction.action.Enable();
    void OnDisable() => interactAction.action.Disable();

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager");

        if (audioManager == null) Debug.Log("Audiomanager was not found, playing no audio");
    }

    void Update()
    {
        if (interactAction.action.WasPressedThisFrame() && current != null)
        {
            if (audioManager != null) audioManager.GetComponent<AudioManager>().PlayTalkAudio();
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
