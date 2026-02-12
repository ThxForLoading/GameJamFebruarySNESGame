using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyContactDamage : MonoBehaviour
{
    [SerializeField] int damage = 1;


    [SerializeField] float hitCooldownSeconds = 0.5f;

    [SerializeField] LayerMask playerMask;

    private float nextHitTime = 0f;
    private Collider2D hitbox;
    private Animator anim;

    void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        hitbox.isTrigger = true;

        anim = GetComponentInParent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other) => TryHit(other);
    void OnTriggerStay2D(Collider2D other) => TryHit(other);

    private void TryHit(Collider2D other)
    {
        if (Time.time < nextHitTime) return;
        if ((playerMask.value & (1 << other.gameObject.layer)) == 0) return;
        if (!other.TryGetComponent<IDamageable>(out var dmg)) return;

        // Direction from enemy -> player (push player away)
        Vector2 hitDir = ((Vector2)other.transform.position - (Vector2)transform.position);
        if (hitDir.sqrMagnitude < 0.0001f) hitDir = Vector2.up;
        hitDir.Normalize();

        dmg.TakeDamage(damage, hitDir);

        if (anim) anim.SetTrigger("HitPlayer");

        nextHitTime = Time.time + hitCooldownSeconds;
    }
}
