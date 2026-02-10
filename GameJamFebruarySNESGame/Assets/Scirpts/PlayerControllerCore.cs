using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerCore : MonoBehaviour
{
    [SerializeField] private InputActionReference move;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] public bool lockMovement;

    public float moveSpeed = 3f;
    public float collisionRadius = 0.2f;

    private Vector2 input;
    public Vector2 FacingDirection { get; private set; }

    void OnEnable() => move.action.Enable();
    void OnDisable() => move.action.Disable();

    void Update()
    {
        ReadInput();
    }

    private void FixedUpdate()
    {
        if (lockMovement) return;

        Move();
    }

    void ReadInput()
    {
        input = move.action.ReadValue<Vector2>();

        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        if (input != Vector2.zero)
        {
            FacingDirection = input;
        }
    }

    void Move()
    {
        Vector2 delta = input * moveSpeed * Time.deltaTime;
        if (delta == Vector2.zero) return;

        Vector2 pos = transform.position;

        if (!Physics2D.OverlapCircle(pos + delta, collisionRadius, obstacleLayer))
        {
            transform.position = pos + delta;
            return;
        }

        if (!Physics2D.OverlapCircle(pos + new Vector2(delta.x, 0), collisionRadius, obstacleLayer))
        {
            transform.position += new Vector3(delta.x, 0, 0);
            return;
        }

        if (!Physics2D.OverlapCircle(pos + new Vector2(0, delta.y), collisionRadius, obstacleLayer))
        {
            transform.position += new Vector3(0, delta.y, 0);
        }
    }

    private void OnDrawGizmosSelected()
    { 
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position, collisionRadius); 
    }
}
