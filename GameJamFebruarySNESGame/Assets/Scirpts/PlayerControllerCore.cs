using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerCore : MonoBehaviour
{
    [SerializeField] private InputActionReference move;
    public float tileSize = 1f;
    public float moveSpeed = 6f;

    private bool isMoving;
    private Vector2 input;

    void OnEnable()
    {
        move.action.Enable();
    }

    void OnDisable()
    {
        move.action.Disable();
    }

    void Start()
    {
        transform.position = SnapToGrid(transform.position);
    }

    void Update()
    {
        if (!isMoving)
        {
            Vector2 dir = GetDirection();
            if (dir != Vector2.zero)
            {
                Vector3 targetPos = transform.position + (Vector3)(dir * tileSize);
                StartCoroutine(Move(targetPos));
            }
        }
    }

    Vector2 GetDirection()
    {
        input = move.action.ReadValue<Vector2>();

        int x = Mathf.RoundToInt(input.x);
        int y = Mathf.RoundToInt(input.y);

        if (x != 0 && y != 0)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                y = 0;
            else
                x = 0;
        }

        return new Vector2(x, y);
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;

        transform.position = SnapToGrid(targetPos);
    }

    Vector3 SnapToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x / tileSize) * tileSize,
            Mathf.Round(pos.y / tileSize) * tileSize,
            pos.z
        );
    }
}
