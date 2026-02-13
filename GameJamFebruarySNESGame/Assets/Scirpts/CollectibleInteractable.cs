using System.Collections;
using UnityEngine;

public class CollectibleInteractable : MonoBehaviour, IInteractable
{
    [Header("Identity")]
    [SerializeField] private string collectibleId;
    [SerializeField] private string prompt = "Collect";

    [Header("Progress")]
    [SerializeField] private bool autoSaveOnCollect = true;

    [Header("Player Feedback")]
    [SerializeField] private string playerAnimTrigger = "Collect";
    [SerializeField] private float lockMovementSeconds = 0.6f;
    [SerializeField] private Animator playerAnim;

    [Header("Visual Swap")]
    [SerializeField] private SpriteRenderer idleSprite;
    [SerializeField] private SpriteRenderer collectedSprite;
    [SerializeField] private float collectedSpriteLifetime = 0.25f; // after sequence

    [Header("Alignment")]
    [SerializeField] private bool alignPlayerBeforeAnim = true;
    [SerializeField] private float alignDuration = 0.12f;
    [SerializeField] private float desiredDistance = 0.35f;
    [SerializeField] private LayerMask blockingMask;
    [SerializeField] private float playerCollisionRadius = 0.2f;

    private bool collectedThisSession;
    private GameObject audioManager;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(collectibleId))
            collectibleId = System.Guid.NewGuid().ToString();
    }
#endif

    void Awake()
    {
        // Ensure collected sprite starts hidden
        if (collectedSprite) collectedSprite.enabled = false;
        if (idleSprite) idleSprite.enabled = true;
    }

    void Start()
    {
        var prog = CollectibleProgress.Instance;
        if (prog != null && prog.IsCollected(collectibleId))
        {
            // Already collected: hide idle, hide collected, optionally disable collider
            if (idleSprite) idleSprite.enabled = false;
            if (collectedSprite) collectedSprite.enabled = false;

            var col = GetComponent<Collider2D>();
            if (col) col.enabled = false;
            return;
        }

        audioManager = GameObject.FindGameObjectWithTag("AudioManager");
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (collectedThisSession) return;

        var prog = CollectibleProgress.Instance;
        if (prog == null) { Debug.LogError("CollectibleProgress missing."); return; }

        if (!prog.TryCollect(collectibleId))
            return;

        collectedThisSession = true;

        // Disable collider so you can't interact again
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        // Swap visuals immediately
        if (idleSprite) idleSprite.enabled = false;
        if (collectedSprite) collectedSprite.enabled = true;

        // Run the sequence on THIS object (we are not disabling it)
        StartCoroutine(CollectSequence(interactor));
    }

    public string GetPrompt() => prompt;

    private IEnumerator CollectSequence(PlayerInteractor interactor)
    {
        yield return StartCoroutine(PlayCollectOnPlayer(interactor));

        if (autoSaveOnCollect && SaveHandler.instance != null && interactor != null)
        {
            var saveInteract = interactor.GetComponentInParent<SaveInteractPlayer>();
            if (saveInteract != null)
            {
                yield return saveInteract.BeginSaveInteractionRoutine();
            }
            else
            {
                Debug.LogWarning("Collectible: SaveInteractPlayer not found on player.");
            }
        }


        if (collectedSpriteLifetime > 0f)
            yield return new WaitForSecondsRealtime(collectedSpriteLifetime);

        if (collectedSprite)
            Destroy(collectedSprite.gameObject);

    }

    private IEnumerator PlayCollectOnPlayer(PlayerInteractor interactor)
    {
        var controller = interactor != null ? interactor.ControllerCore : null;
        var playerTf = interactor != null ? interactor.transform : null;

        if (controller != null) controller.lockMovement = true;

        if (playerTf != null)
        {
            Vector2 toCollectible = (Vector2)(transform.position - playerTf.position);
            if (toCollectible.sqrMagnitude < 0.0001f) toCollectible = Vector2.up;
            Vector2 faceDir = toCollectible.normalized;

            if (controller != null)
                controller.FacingDirection = faceDir;

            if (alignPlayerBeforeAnim)
            {
                Vector2 targetPos = (Vector2)transform.position - faceDir * desiredDistance;

                if (!Physics2D.OverlapCircle(targetPos, playerCollisionRadius, blockingMask))
                {
                    float end = Time.realtimeSinceStartup + alignDuration;
                    Vector2 start = playerTf.position;

                    while (Time.realtimeSinceStartup < end)
                    {
                        float t = 1f - ((end - Time.realtimeSinceStartup) / alignDuration);
                        Vector2 next = Vector2.Lerp(start, targetPos, t);

                        if (Physics2D.OverlapCircle(next, playerCollisionRadius, blockingMask))
                            break;

                        playerTf.position = next;
                        yield return null;
                    }

                    playerTf.position = targetPos;
                }
            }
        }

        // Trigger PLAYER animator (reliable)
        if (playerAnim != null && !string.IsNullOrEmpty(playerAnimTrigger))
            playerAnim.SetTrigger(playerAnimTrigger);

        if (audioManager != null)
            audioManager.GetComponent<AudioManager>().PlayCollectAudio();

        yield return new WaitForSecondsRealtime(lockMovementSeconds);

        if (controller != null) controller.lockMovement = false;
    }
}
