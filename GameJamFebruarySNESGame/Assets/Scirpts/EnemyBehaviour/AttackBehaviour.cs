using UnityEngine;

public class AttackBehaviour : StateMachineBehaviour
{
    [SerializeField] private EnemyMovement movement;
    [SerializeField] private EnemyCombat combat;

    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        movement = animator.GetComponent<EnemyMovement>();
        combat = animator.GetComponent<EnemyCombat>();

        movement.Stop();
    }

    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        combat.TryAttack();
    }

    override public void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        // possibly reset attack triggers or animation flags
    }
}
