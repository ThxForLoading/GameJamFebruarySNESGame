using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float sightRange = 6f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float dist = Vector2.Distance(transform.position, target.position);

        animator.SetBool("CanSeeTarget", dist <= sightRange);
        animator.SetBool("InAttackRange", dist <= attackRange);
        animator.SetBool("LostTarget", dist > sightRange * 1.2f);
    }
}
