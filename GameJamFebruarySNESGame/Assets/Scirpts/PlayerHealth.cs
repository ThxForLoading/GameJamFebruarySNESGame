using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int maxHp = 10;
    public int hp;

    private PlayerKnockback knockback;

    void Awake()
    {
        hp = maxHp;
        knockback = GetComponent<PlayerKnockback>();
    }

    public void TakeDamage(int amount, Vector2 hitDirection)
    {
        hp = Mathf.Max(0, hp - amount);
        Debug.Log($"Player took {amount} damage. HP: {hp}");

        if (knockback != null)
            knockback.Trigger(hitDirection);

        if (hp == 0)
        {
            Debug.Log("Player died");
            // respawn player?
        }
    }
}