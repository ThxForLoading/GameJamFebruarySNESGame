using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MashDetector : MonoBehaviour
{
    [SerializeField] private InputActionReference shoulderLeft;
    [SerializeField] private InputActionReference shoulderRight;
    [SerializeField] private float timeWindow = 1.0f;
    [SerializeField] private int pressesRequired = 6;
    [SerializeField] private float durationToTrigger = 1.0f;

    [SerializeField] private float drainRate = 0.2f;

    private readonly Queue<float> _pressTimesL = new();
    private readonly Queue<float> _pressTimesR = new();

    private float _dualMashTimer = 0f;
    private bool _triggered = false;

    private void OnEnable()
    {
        shoulderLeft.action.Enable();
        shoulderRight.action.Enable();

        shoulderLeft.action.started += OnMashL;
        shoulderRight.action.started += OnMashR;
    }

    private void OnDisable()
    {
        shoulderLeft.action.started -= OnMashL;
        shoulderRight.action.started -= OnMashR;

        shoulderLeft.action.Disable();
        shoulderRight.action.Disable();
    }

    public void OnMashL(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        RegisterPress(_pressTimesL);
        Debug.Log("Left shoulder pressed");
    }

    public void OnMashR(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        RegisterPress(_pressTimesR);
        Debug.Log("Right shoulder pressed.");
    }

    private void RegisterPress(Queue<float> pressTimes)
    {
        float now = Time.time;
        pressTimes.Enqueue(now);

        while (pressTimes.Count > 0 && now - pressTimes.Peek() > timeWindow)
            pressTimes.Dequeue();
    }

    private bool IsMashing(Queue<float> pressTimes)
    {
        float now = Time.time;

        // Keep window clean even if no new presses come in this frame
        while (pressTimes.Count > 0 && now - pressTimes.Peek() > timeWindow)
            pressTimes.Dequeue();

        return pressTimes.Count >= pressesRequired;
    }

    private void Update()
    {
        bool mashA = IsMashing(_pressTimesL);
        bool mashB = IsMashing(_pressTimesR);

        bool dualMashing = mashA && mashB;

        if (dualMashing)
        {
            _dualMashTimer += Time.deltaTime;

            if (!_triggered && _dualMashTimer >= durationToTrigger)
            {
                _triggered = true;
                OnDualMashTriggered();
            }
        }
        else
        {
            _dualMashTimer = Mathf.Max(0f, _dualMashTimer - Time.deltaTime * drainRate);
            _triggered = false;
        }
    }

    private void OnDualMashTriggered()
    {
        Debug.Log("Dual mash triggered! Spawn fire!");
    }

}
