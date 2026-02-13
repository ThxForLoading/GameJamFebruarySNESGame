using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerControllerCore : MonoBehaviour
{
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference aButton;
    [SerializeField] private InputActionReference bButton;

    [SerializeField] private InputActionReference xRButton;
    [SerializeField] private InputActionReference yLButton;
    [SerializeField] private InputActionReference bRButton;



    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask iceLayer;
    [SerializeField] private LayerMask waterLayer;
    [SerializeField] private LayerMask savePointLayer;
    [SerializeField] private LayerMask fireDestructibleLayer;
    [SerializeField] public bool lockMovement;

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private PlayerKnockback knockback;
    //assign a tilemap so the player can interact with certain tiles
    Tilemap tileMap;

    public float moveSpeed = 3f;
    public float collisionRadius = 0.2f;

    private SpellHandler spellHandler;
 

    private Vector2 input;
    private Vector2 currentVelocity;
    public Vector2 FacingDirection;

    //Ice stuff
    private bool onIce;
    private bool isSliding;
    Vector2 slidingDirection;


    void OnEnable() => move.action.Enable();
    void OnDisable() => move.action.Disable();

    private void Awake()
    {
        spellHandler = GetComponent<SpellHandler>();
        knockback = GetComponent<PlayerKnockback>();
        xRButton.action.performed += PlayerCastIce;
        yLButton.action.performed += PlayerCastFire;
        bRButton.action.performed += PlayerCastPlant;

        if(GameObject.FindGameObjectWithTag("TileHandler") != null)
        {
            tileMap = GameObject.FindGameObjectWithTag("TileHandler").GetComponent<TilemapChanger>().groundTileMap;
        }
    }

    private void OnDestroy()
    {
        xRButton.action.performed -= PlayerCastIce;
        yLButton.action.performed -= PlayerCastFire;
        bRButton.action.performed -= PlayerCastPlant;
    }

    private void PlayerCastFire(InputAction.CallbackContext context)
    {
        spellHandler.castFire();
    }

    private void PlayerCastIce(InputAction.CallbackContext context)
    {
        spellHandler.castIce();
    }

    private void PlayerCastPlant(InputAction.CallbackContext context)
    {
        spellHandler.castPlant();
    }

    void Update()
    {
        ReadInput();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (lockMovement) return;

        if (knockback != null && knockback.IsActive)
        {
            Vector2 kbDelta = knockback.ConsumeDelta();

            if (kbDelta != Vector2.zero)
            {
                // Use your existing collision resolution, but it expects a delta.
                ApplyDeltaWithCollision(kbDelta);
            }

            // Optional: disable player control while being knocked back
            return;
        }


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
    void UpdateAnimator()
    {
        Vector2 dirSource;

        if (lockMovement || isSliding)
        {
            dirSource = FacingDirection;
            animator.SetFloat("Speed", 0f);
        }
        else
        {
            dirSource = input != Vector2.zero ? input : FacingDirection;
            animator.SetFloat("Speed", input.sqrMagnitude);
        }

        Vector2Int dir = Get8Direction(dirSource);

        if (dir.x != 0) spriteRenderer.flipX = dir.x < 0;

        dir = RemapToRight(dir);

        animator.SetInteger("DirX", dir.x);
        animator.SetInteger("DirY", dir.y);
    }

    Vector2Int RemapToRight(Vector2Int dir)
    {
        if (dir.x < 0)
            dir.x = 1;

        return dir;
    }

    void ReadInput()
    {
        if(isSliding) return;

        input = move.action.ReadValue<Vector2>();

        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        if (input != Vector2.zero && !lockMovement)
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
    }

    private void OnDrawGizmosSelected()
    { 
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position, collisionRadius);

        if (tileMap == null) return;

        Gizmos.color = Color.red;
        Vector3Int cell = GetSelectedTile();
        Vector3 center = tileMap.GetCellCenterWorld(cell);
        Gizmos.DrawWireCube(center, Vector3.one * 0.9f);
    }

    void CheckIce()
    {
        onIce = Physics2D.OverlapCircle(transform.position, collisionRadius, iceLayer);
    }

    Vector3 CanWalkCollision(Vector2 pos, Vector2 delta)
    {
        LayerMask collisionMask = obstacleLayer | waterLayer | savePointLayer | fireDestructibleLayer;

        Vector3 temp = new Vector3();
        if (!Physics2D.OverlapCircle(pos + new Vector2(delta.x, 0), collisionRadius, collisionMask))
        {
            temp.x = delta.x;
        }
        if (!Physics2D.OverlapCircle(pos + new Vector2(0, delta.y), collisionRadius, collisionMask))
        {
            temp.y = delta.y;
        }
        return temp;
    }

    public Vector3Int GetSelectedTile()
    {
        if (tileMap == null) return new Vector3Int();

        Vector3 worldPos = transform.position + new Vector3(FacingDirection.x, FacingDirection.y, 0);

        Vector3Int tilePos = tileMap.WorldToCell(worldPos);

        return tilePos;
    }

    Vector2Int Get8Direction(Vector2 dir)
    {
        if (dir == Vector2.zero)
            return Vector2Int.zero;

        dir.Normalize();

        float absX = Mathf.Abs(dir.x);
        float absY = Mathf.Abs(dir.y);

        const float diagonalThreshold = 0.35f;

        int x = 0;
        int y = 0;

        if (absX > diagonalThreshold && absY > diagonalThreshold)
        {
            x = dir.x > 0 ? 1 : -1;
            y = dir.y > 0 ? 1 : -1;
        }
        else if (absX > absY)
        {
            x = dir.x > 0 ? 1 : -1;
            y = 0;
        }
        else
        {
            x = 0;
            y = dir.y > 0 ? 1 : -1;
        }

        return new Vector2Int(x, y);
    }

    public void ActivateDarkness()
    {
        spellHandler.EnableDarkness();
    }

    public void DeactivateDarkness()
    {
        spellHandler.DisableDarkness();
    }

    void ApplyDeltaWithCollision(Vector2 delta)
    {
        if (delta == Vector2.zero) return;

        Vector2 pos = transform.position;
        Vector3 temp = CanWalkCollision(pos, delta);

        if (temp != Vector3.zero)
            transform.position += temp;
    }
}
