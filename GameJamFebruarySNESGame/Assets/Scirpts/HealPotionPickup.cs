using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HealPotionPickup : MonoBehaviour
{
    [Header("Heal")]
    [SerializeField] private int healAmount = 6;
    [SerializeField] private bool consumeIfFullHealth = false;

    [Header("Player Feedback")]
    [SerializeField] private string playerAnimTrigger = "SmashPotion";
    [SerializeField] private float lockMovementSeconds = 0.5f;

    [Header("Filter")]
    [SerializeField] private LayerMask playerMask;

    [Header("Optional")]
    [SerializeField] private bool autoSaveOnUse = true;
    [SerializeField] private GameObject useVfxPrefab;

    private bool used;

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;
        if ((playerMask.value & (1 << other.gameObject.layer)) == 0) return;

        var healTarget = other.GetComponentInParent<IHealable>();
        if (healTarget == null) return;

        if (!consumeIfFullHealth && !healTarget.CanHeal)
            return;

        healTarget.Heal(healAmount);
        used = true;

        // Feedback (lock + animation)
        var controller = other.GetComponentInParent<PlayerControllerCore>();
        var animator = other.GetComponentInParent<Animator>();
        StartCoroutine(UseSequence(controller, animator));

        if (useVfxPrefab)
            Instantiate(useVfxPrefab, transform.position, Quaternion.identity);

        gameObject.SetActive(false);

        if (autoSaveOnUse && SaveHandler.instance != null)
            SaveHandler.instance.SaveGame();
    }

    private IEnumerator UseSequence(PlayerControllerCore controller, Animator animator)
    {
        if (controller != null) controller.lockMovement = true;

        if (animator != null && !string.IsNullOrEmpty(playerAnimTrigger))
            animator.SetTrigger(playerAnimTrigger);

        yield return new WaitForSeconds(lockMovementSeconds);

        if (controller != null) controller.lockMovement = false;
    }
}
