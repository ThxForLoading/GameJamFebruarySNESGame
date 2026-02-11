using UnityEngine;

public class PatrolBehaviour : StateMachineBehaviour
{
    [SerializeField] private EnemyMovement movement;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        movement = animator.GetComponent<EnemyMovement>();
        movement.StartPatrol();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        movement.UpdatePatrol();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        movement.Stop();
    }
}
