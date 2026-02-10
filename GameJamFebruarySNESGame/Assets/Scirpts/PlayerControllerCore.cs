using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerCore : MonoBehaviour
{
    [SerializeField] private InputActionReference move;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask iceLayer;
    [SerializeField] public bool lockMovement;

    public float moveSpeed = 3f;
    public float collisionRadius = 0.2f;

 

    private Vector2 input;
    private Vector2 currentVelocity;
    public Vector2 FacingDirection;

    //Ice stuff
    private bool onIce;
    private bool isSliding;
    Vector2 slidingDirection;


    void OnEnable() => move.action.Enable();
    void OnDisable() => move.action.Disable();

    void Update()
    {
        ReadInput();
    }

    private void FixedUpdate()
    {
        if (lockMovement) return;

        CheckIce();

        if (onIce)
        {
            IceMove();
        }
        else
        {
            Move();
        }
    }

    void ReadInput()
    {
        if(isSliding) return;

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
        isSliding = false;
        MoveWithCollision(input);
    }

    void IceMove()
    {
        if (!isSliding && input != Vector2.zero)
        {
            isSliding = true;
            slidingDirection = input;
            FacingDirection = slidingDirection;
        }

        if (!isSliding) return;

        bool moved = MoveWithCollision(slidingDirection);

        if (!moved)
        {
            isSliding = false;
            slidingDirection = Vector2.zero;
        }
    }

    bool MoveWithCollision(Vector2 direction)
    {
        Vector2 delta = direction * moveSpeed * Time.deltaTime;
        if (delta == Vector2.zero) return false;

        Vector2 pos = transform.position;

        Vector3 temp = CanWalkCollision(pos, delta);

        if(temp != Vector3.zero)
        {
            transform.position += temp;
            return true;
        }

        return false;

        //if (!Physics2D.OverlapCircle(pos + delta, collisionRadius, obstacleLayer))
        //{
        //    transform.position = pos + delta;
        //    return true;
        //}

        //if (!Physics2D.OverlapCircle(pos + new Vector2(delta.x, 0), collisionRadius, obstacleLayer))
        //{
        //    transform.position += new Vector3(delta.x, 0, 0);
        //    return true;
        //}

        //if (!Physics2D.OverlapCircle(pos + new Vector2(0, delta.y), collisionRadius, obstacleLayer))
        //{
        //    transform.position += new Vector3(0, delta.y, 0);
        //    return true;
        //}

        //return false;
    }

    private void OnDrawGizmosSelected()
    { 
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position, collisionRadius); 
    }

    void CheckIce()
    {
        onIce = Physics2D.OverlapCircle(transform.position, collisionRadius, iceLayer);
    }

    Vector3 CanWalkCollision(Vector2 pos, Vector2 delta)
    {
        Vector3 temp = new Vector3();
        if (!Physics2D.OverlapCircle(pos + new Vector2(delta.x, 0), collisionRadius, obstacleLayer))
        {
            temp.x = delta.x;
        }
        if (!Physics2D.OverlapCircle(pos + new Vector2(0, delta.y), collisionRadius, obstacleLayer))
        {
            temp.y = delta.y;
        }
        return temp;
    }
}
