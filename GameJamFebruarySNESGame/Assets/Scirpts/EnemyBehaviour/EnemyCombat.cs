using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] public float attackCooldown = 1f;
    [SerializeField] private float lastAttackTime;

    public void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        Debug.Log("Enemy attacks!");
    }
}
