using UnityEngine;

public class ChaseBehaviour : StateMachineBehaviour
{
    [SerializeField] private EnemyMovement movement;
    [SerializeField] private Transform target;

    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        movement = animator.GetComponent<EnemyMovement>();
        target = movement.Target;
    }

    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        if (!target) return;
        movement.Chase(target.position);
    }

    override public void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        movement.Stop();
    }
}
