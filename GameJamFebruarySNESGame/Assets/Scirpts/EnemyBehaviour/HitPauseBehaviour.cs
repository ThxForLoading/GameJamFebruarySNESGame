using UnityEngine;

public class HitPauseBehaviour : StateMachineBehaviour
{
    public float pauseSeconds = 1.0f;

    private EnemyMovement movement;
    private float exitTime;
    private static readonly int PauseFinished = Animator.StringToHash("PauseFinished");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        movement = animator.GetComponent<EnemyMovement>();
        if (movement) movement.Stop();

        animator.SetBool(PauseFinished, false);
        exitTime = Time.time + pauseSeconds;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (movement) movement.Stop();

        if (Time.time >= exitTime)
            animator.SetBool(PauseFinished, true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(PauseFinished, false);
    }
}
