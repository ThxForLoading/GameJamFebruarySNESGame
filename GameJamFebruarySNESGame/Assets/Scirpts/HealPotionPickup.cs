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
    private Animator animator;

    [Header("Filter")]
    [SerializeField] private LayerMask playerMask;

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

        used = true;
        healTarget.Heal(healAmount);

        var controller = other.GetComponentInParent<PlayerControllerCore>();
        var animator = other.GetComponentInParent<PlayerAnimationRefs>().Animator;
        


        // Run sequence on PLAYER so disabling potion doesn't kill coroutine
        if (controller != null)
            controller.StartCoroutine(UseSequence(controller, animator));
        else
            StartCoroutine(UseSequence(controller, animator)); // fallback

        gameObject.SetActive(false);
    }

    private IEnumerator UseSequence(PlayerControllerCore controller, Animator animator)
    {
        if (controller != null) controller.lockMovement = true;

        if (animator != null && !string.IsNullOrEmpty(playerAnimTrigger))
            animator.SetTrigger(playerAnimTrigger);

        yield return new WaitForSecondsRealtime(lockMovementSeconds);

        if (controller != null) controller.lockMovement = false;
    }
}
