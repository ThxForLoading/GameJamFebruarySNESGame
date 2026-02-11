using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    [SerializeField] float knockbackDistance = 0.8f;
    [SerializeField] float knockbackDuration = 0.12f;
    [SerializeField] AnimationCurve falloff = AnimationCurve.Linear(0, 1, 1, 0);

    private Vector2 direction;
    private float startTime;
    private bool active;


    public void Trigger(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.up;
        direction = dir.normalized;

        startTime = Time.time;
        active = true;
        Debug.Log("knockback triggered!");

    }

    public bool IsActive => active;


    public Vector2 ConsumeDelta()
    {
        if (!active) return Vector2.zero;

        float t = (Time.time - startTime) / Mathf.Max(0.0001f, knockbackDuration);
        if (t >= 1f)
        {
            active = false;
            return Vector2.zero;
        }


        float baseSpeed = knockbackDistance / knockbackDuration;
        float curve = Mathf.Max(0f, falloff.Evaluate(t));

        return direction * (baseSpeed * curve * Time.fixedDeltaTime);
    }
}
