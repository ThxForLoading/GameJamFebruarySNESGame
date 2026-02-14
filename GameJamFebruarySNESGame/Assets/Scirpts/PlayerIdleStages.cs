using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleStages : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerControllerCore controller; 

    [Header("Idle Timings (seconds)")]
    [SerializeField] private float drowsyAfter = 18f;
    [SerializeField] private float sleepAfter = 30f;

    [Header("Movement Detection")]
    [SerializeField] private float minMoveDistancePerSecond = 0.02f;

    [Header("Optional Input = Activity")]
    [SerializeField] private InputActionReference[] activityActions;

    private static readonly int IdleStageHash = Animator.StringToHash("IdleStage");

    private Vector3 lastPos;
    private float idleTimer;
    private int currentStage;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!controller) controller = GetComponent<PlayerControllerCore>();
        lastPos = transform.position;
        SetStage(0);
    }

    void OnEnable()
    {
        if (activityActions == null) return;
        foreach (var a in activityActions)
            a?.action?.Enable();
    }

    void Update()
    {

        if (controller != null && controller.lockMovement)
        {
            ResetIdle();
            lastPos = transform.position;
            return;
        }

        bool moved = DetectMovement();
        bool pressed = DetectAnyActionPressed();

        if (moved || pressed)
        {
            ResetIdle();
            return;
        }

        idleTimer += Time.deltaTime;

        if (idleTimer >= sleepAfter) SetStage(2);
        else if (idleTimer >= drowsyAfter) SetStage(1);
        else SetStage(0);
    }

    private bool DetectMovement()
    {
        Vector3 pos = transform.position;
        float dist = Vector3.Distance(pos, lastPos);
        lastPos = pos;

        float distPerSecond = dist / Mathf.Max(Time.deltaTime, 0.0001f);
        return distPerSecond > minMoveDistancePerSecond;
    }

    private bool DetectAnyActionPressed()
    {
        if (activityActions == null) return false;

        for (int i = 0; i < activityActions.Length; i++)
        {
            var act = activityActions[i]?.action;
            if (act == null) continue;

            if (act.WasPressedThisFrame())
                return true;
        }

        return false;
    }

    private void ResetIdle()
    {
        idleTimer = 0f;
        SetStage(0);
    }

    private void SetStage(int stage)
    {
        if (currentStage == stage) return;
        currentStage = stage;

        if (animator)
            animator.SetInteger(IdleStageHash, stage);
    }
    public void NotifyActivity() => ResetIdle();
}
