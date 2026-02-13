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

    [Header("Alignment")]
    [SerializeField] private bool alignPlayerBeforeAnim = true;
    [SerializeField] private float alignDuration = 0.12f;          
    [SerializeField] private float desiredDistance = 0.35f;         
    [SerializeField] private LayerMask blockingMask;                
    [SerializeField] private float playerCollisionRadius = 0.2f;   

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
        var playerTf = interactor.transform; 
        if (controller != null) controller.lockMovement = true;

        // 1) Face the collectible
        Vector2 toCollectible = (transform.position - playerTf.position);
        if (toCollectible.sqrMagnitude < 0.0001f) toCollectible = Vector2.up;

        Vector2 faceDir = toCollectible.normalized;

        // If your PlayerControllerCore uses FacingDirection for animations:
        controller.FacingDirection = faceDir;

        // 2) Optionally align / nudge player to interaction point
        if (alignPlayerBeforeAnim)
        {
            // Put the player at a point "desiredDistance" away from the collectible, along the line from collectible -> player
            Vector2 targetPos = (Vector2)transform.position - faceDir * desiredDistance;

            // Only move if the target position isn't blocked
            if (!Physics2D.OverlapCircle(targetPos, playerCollisionRadius, blockingMask))
            {
                float endTime = Time.time + alignDuration;
                Vector2 startPos = playerTf.position;

                while (Time.time < endTime)
                {
                    float t = 1f - ((endTime - Time.time) / alignDuration);
                    Vector2 next = Vector2.Lerp(startPos, targetPos, t);

                    // stop early if path becomes blocked (safety)
                    if (Physics2D.OverlapCircle(next, playerCollisionRadius, blockingMask))
                        break;

                    playerTf.position = next;
                    yield return null;
                }

                playerTf.position = targetPos;
            }
        }

        // 3) Trigger animation
        var anim = interactor.GetComponentInParent<Animator>();
        if (anim != null && !string.IsNullOrEmpty(playerAnimTrigger))
            anim.SetTrigger(playerAnimTrigger);

        // 4) Wait and unlock
        yield return new WaitForSeconds(lockMovementSeconds);

        if (controller != null)
            controller.lockMovement = false;
    }

}
