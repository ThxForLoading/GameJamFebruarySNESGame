using NUnit.Framework.Internal;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    public Collider2D col;
    public Transform Target;

    [SerializeField] float patrolSpeed = 2.5f;
    [SerializeField] float chaseSpeed = 4f;
    [SerializeField] float returnSpeed = 3.0f;

    [SerializeField] Transform[] patrolPoints;
    [SerializeField] float waypointTolerance = 0.2f;

    private int patrolIndex = 0;

    [SerializeField] private bool returningToPatrol = false;
    [SerializeField] private Vector2 returnPoint;

    [SerializeField] private LayerMask blockingMask;      
    [SerializeField] private float castSkin = 0.02f;       


    private readonly RaycastHit2D[] castHits = new RaycastHit2D[8];
    private ContactFilter2D castFilter;
    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!col) col = GetComponent<Collider2D>();

        castFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = blockingMask,
            useTriggers = false
        };
    }

    public void StartPatrol()
    {
        returningToPatrol = false;

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Stop();
            return;
        }

        patrolIndex = Mathf.Clamp(patrolIndex, 0, patrolPoints.Length - 1);
    }

    public void UpdatePatrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Stop();
            return;
        }

        if (returningToPatrol)
        {
            TryMoveTowards(returnPoint, returnSpeed);

            if (Vector2.Distance(rb.position, returnPoint) <= waypointTolerance)
            {
                returningToPatrol = false;
                patrolIndex = FindNearestPatrolIndex(rb.position);
            }
            return;
        }

        var current = patrolPoints[patrolIndex];
        if (!current)
        {
            Stop();
            return;
        }

        TryMoveTowards(current.position, patrolSpeed);

        if (Vector2.Distance(rb.position, current.position) <= waypointTolerance)
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
    }

    public void Chase(Vector2 targetPos)
    {
        returningToPatrol = false;
        TryMoveTowards(targetPos, chaseSpeed);
    }

    public void StartReturnToPatrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            returningToPatrol = false;
            Stop();
            return;
        }

        var current = patrolPoints[patrolIndex];
        if (!current)
        {
            returningToPatrol = false;
            Stop();
            return;
        }

        returnPoint = current.position;
        returningToPatrol = true;
    }

    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
    }

    private void TryMoveTowards(Vector2 destination, float speed)
    {
        Vector2 delta = destination - rb.position;
        if (delta.sqrMagnitude < 0.0001f)
        {
            Stop();
            return;
        }

        Vector2 dir = delta.normalized;
        float distanceThisStep = speed * Time.fixedDeltaTime;

        int hitCount = col.Cast(dir, castFilter, castHits, distanceThisStep + castSkin);

        if (hitCount > 0)
        {
            Stop();
            return;
        }

        rb.linearVelocity = dir * speed;
    }

    private int FindNearestPatrolIndex(Vector2 pos)
    {
        int bestIndex = 0;
        float bestDistSq = float.PositiveInfinity;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (!patrolPoints[i]) continue;

            float d = ((Vector2)patrolPoints[i].position - pos).sqrMagnitude;
            if (d < bestDistSq)
            {
                bestDistSq = d;
                bestIndex = i;
            }
        }
        return bestIndex;
    }
}
