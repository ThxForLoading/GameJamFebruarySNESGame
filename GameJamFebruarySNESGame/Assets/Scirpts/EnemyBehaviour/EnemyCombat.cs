using System.Collections;
using UnityEngine;

public class EnemyCombat : MonoBehaviour, IDamageable
{
    [SerializeField] public float attackCooldown = 1f;
    [SerializeField] private float lastAttackTime;

    [Header("Health")]
    public int maxHp = 1;
    public int currentHp;

    [Header("I-frames")]
    public float invulnSeconds = 0.1f;
    private float invulnUntil;

    [Header("Death")]
    public bool destroyOnDeath = true;
    [SerializeField] private float deathFlashDuration = 0.15f;
    [SerializeField] private int flashCount = 2;
    [SerializeField] private float translucentAlpha = 0.4f;
    [SerializeField] private Color grayTint = new Color(0.6f, 0.6f, 0.6f, 1f);


    void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int amount, Vector2 hitDirection)
    {
        if (amount <= 0) return;
        if (Time.time < invulnUntil) return;

        invulnUntil = Time.time + invulnSeconds;

        currentHp -= amount;

        Debug.Log($"{name} took {amount} damage. HP: {currentHp}/{maxHp}");

        if (currentHp <= 0)
            Die();
    }

    private void Die()
    {
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Stop movement
        var movement = GetComponent<EnemyMovement>();
        if (movement) movement.Stop();

        // Disable AI
        var animator = GetComponent<Animator>();
        if (animator) animator.enabled = false;

        // Disable colliders so it no longer blocks anything
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        // Get sprite renderers (supports multiple)
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        // Flash twice
        for (int i = 0; i < flashCount; i++)
        {
            SetAlpha(renderers, 0f); // invisible
            yield return new WaitForSeconds(deathFlashDuration);

            SetAlpha(renderers, 1f); // visible
            yield return new WaitForSeconds(deathFlashDuration);
        }

        // Final gray + translucent state
        foreach (var sr in renderers)
        {
            sr.color = new Color(grayTint.r, grayTint.g, grayTint.b, translucentAlpha);
        }

        yield return new WaitForSeconds(0.25f);

        Destroy(gameObject);
    }
    private void SetAlpha(SpriteRenderer[] renderers, float alpha)
    {
        foreach (var sr in renderers)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }
    public void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        Debug.Log("Enemy attacks!");
    }
}
