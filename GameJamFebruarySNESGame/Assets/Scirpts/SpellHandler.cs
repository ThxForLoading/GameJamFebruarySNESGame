using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpellHandler : MonoBehaviour
{
    PlayerControllerCore playerController;

    [Header("Fire")]
    [SerializeField] int fireRange = 3;
    [SerializeField] float fireSpreadSpeed = 0.5f;
    [SerializeField] private Tilemap fireTileMap;     // drag the same one used by TilemapChanger
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private int fireDamage = 1;
    [SerializeField] private Vector2 fireHitBoxScale = new Vector2(0.9f, 0.9f);
    [Header("Ice")]
    [SerializeField] int iceRange = 5;
    [SerializeField] float iceSpreadSpeed = 0.5f;
    [Header("Plants")]
    [SerializeField] int plantRange = 5;
    [SerializeField] float plantGrowSpeed = 0.5f;
    [Header("Light")]
    [SerializeField] GameObject darknessOverlay;
    [SerializeField] GameObject lightOverlay;


    private Animator animator;
    private bool isCasting = false;

    TilemapChanger changer;

    private GameObject audioManager;

    private void Awake()
    {
        playerController = GetComponent<PlayerControllerCore>();
        if(GameObject.FindGameObjectWithTag("TileHandler") != null)
        {
            changer = GameObject.FindGameObjectWithTag("TileHandler").GetComponent<TilemapChanger>();
        }
        if(playerController == null || changer == null)
        {
            Debug.Log("Playercontroller missing, Tilemapchanger missing or Spellhandler not correctly assigned");
            isCasting = true;
        }
    }

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager");

        if (audioManager == null) Debug.Log("Audiomanager was not found, playing no audio");

        if(fireTileMap == null) fireTileMap = changer.GetFireTileMap();
    }

    public void castFire()
    {
        if (isCasting) return;

        animator = playerController.GetComponentInParent<PlayerAnimationRefs>().Animator;
        animator.SetTrigger("Cast");
        //Go forward the range of tiles and place fire, then trigger fire interactions on the affected tiles
        Vector3Int startingLocation = playerController.GetSelectedTile();
        Vector2 direction = playerController.FacingDirection;

        if (direction.x > 0 && direction.y > 0) return;

        isCasting = true;

        Vector3Int[] affectedTiles = new Vector3Int[fireRange];
        int temp = 1;
        if (Mathf.Abs(direction.x) > 0)
        {
            if (direction.x < 0) temp = -1;
            for (int i = 0; i < fireRange; i++)
            {
                affectedTiles[i] = startingLocation + new Vector3Int(i * temp, 0, 0);
            }
        }
        else
        {
            if (direction.y < 0) temp = -1;
            for (int i = 0; i < fireRange; i++)
            {
                affectedTiles[i] = startingLocation + new Vector3Int(0, i * temp, 0);
            }
        }

        if (affectedTiles.Length > 0)
        {
            StartCoroutine(fireInLine(affectedTiles));
        }

    }

    IEnumerator fireInLine(Vector3Int[] affectedTiles)
    {
        bool playedSound = false;
        foreach (Vector3Int tile in affectedTiles)
        {
            if (!changer.CanPlaceFire(tile))
            {
                isCasting = false;
                yield break;
            }
            else if (!playedSound)
            {
                if (audioManager != null) audioManager.GetComponent<AudioManager>().PlayFireAudio();
                playedSound = true;
            }

                changer.PlaceFireTileAt(tile);
            changer.RemoveBurnableTile(tile);
            DamageEnemiesOnCell(tile);
            yield return new WaitForSeconds(fireSpreadSpeed);
            StartCoroutine(extinguishFire(tile));
        }
        isCasting = false;
    }

    IEnumerator extinguishFire(Vector3Int target)
    {
        yield return new WaitForSeconds(fireSpreadSpeed + 0.3f);
        DamageEnemiesOnCell(target);
        changer.RemoveFire(target);
    }
    private void DamageEnemiesOnCell(Vector3Int cellPos)
    {
        Vector3 worldCenter = fireTileMap.GetCellCenterWorld(cellPos);

        Vector3 cellSize = fireTileMap.cellSize;
        Vector2 boxSize = new Vector2(cellSize.x * fireHitBoxScale.x, cellSize.y * fireHitBoxScale.y);

        Collider2D[] hits = Physics2D.OverlapBoxAll(worldCenter, boxSize, 0f, enemyMask);

        foreach (var hit in hits)
        {
            var dmg = hit.GetComponentInParent<IDamageable>();
            if (dmg == null) continue;

            Vector2 hitPoint = hit.ClosestPoint(worldCenter);
            Vector2 dir = ((Vector2)hit.transform.position - (Vector2)worldCenter);
            if (dir.sqrMagnitude < 0.0001f) dir = Vector2.up;
            dir.Normalize();

            dmg.TakeDamage(fireDamage, dir);
        }
    }

    public void castIce()
    {
        if(isCasting) return;

        animator = playerController.GetComponentInParent<PlayerAnimationRefs>().Animator;
        animator.SetTrigger("Cast");
        //Go forward and replace water with ice tiles in the desired range
        Vector3Int startingLocation = playerController.GetSelectedTile();
        Vector2 direction = playerController.FacingDirection;

        if (Mathf.Abs(direction.x) > 0 && Mathf.Abs(direction.y) > 0) return;

        isCasting = true;

        Vector3Int[] affectedTiles = new Vector3Int[iceRange];
        int temp = 1;
        if (Mathf.Abs(direction.x) > 0)
        {
            if(direction.x < 0) temp = -1;
            for (int i = 0; i < iceRange; i++)
            {
                affectedTiles[i] = startingLocation + new Vector3Int(i * temp, 0, 0);
            }
        }
        else
        {
            if(direction.y < 0) temp = -1;
            for (int i = 0; i < iceRange; i++)
            {
                affectedTiles[i] = startingLocation + new Vector3Int(0, i * temp, 0);
            }
        }

        if (affectedTiles.Length > 0)
        {
            
            StartCoroutine(iceInLine(affectedTiles));
        }
        
    }

    IEnumerator iceInLine(Vector3Int[] affectedTiles)
    {
        bool playedSound = false;
        foreach (Vector3Int tile in affectedTiles)
        {
            if (!changer.CanPlaceIce(tile))
            {
                isCasting = false;
                yield break;
            }
            else if (!playedSound)
            {
                if (audioManager != null) audioManager.GetComponent<AudioManager>().PlayIceAudio();
                playedSound = true;
            }

                //Debug.Log("Creating Ice");
                changer.RemoveWater(tile);
            changer.PlaceIceTileat(tile);
            yield return new WaitForSeconds(iceSpreadSpeed);
        }
        isCasting = false;
    }

    public void castPlant()
    {
        if (isCasting) return;

        animator = playerController.GetComponentInParent<PlayerAnimationRefs>().Animator;
        animator.SetTrigger("Cast");

        Vector3Int startingLocation = playerController.GetSelectedTile();

        if (changer.CanPlacePlantClimb(startingLocation))
        {
            isCasting = true;

            Vector3Int[] affectedTiles = new Vector3Int[plantRange];


            for (int i = 0; i < plantRange; i++)
            {
                affectedTiles[i] = startingLocation + new Vector3Int(0, i, 0);      //Plant always grows up
            }

            if (affectedTiles.Length > 0)
            {
                if (audioManager != null) audioManager.GetComponent<AudioManager>().PlayPlantAudio();
                StartCoroutine(GrowPlantInLine(affectedTiles));
            }

        }
        else if (changer.CanPlacePlantGround(startingLocation))
        {
            //place tile with plant over ground
            if (audioManager != null) audioManager.GetComponent<AudioManager>().PlayPlantAudio();
            changer.PlaceGroundPlantTileAt(startingLocation);
        }

        
    }

    IEnumerator GrowPlantInLine(Vector3Int[] affectedTiles)
    {
        //Plant should use bottom piece for 0, top piece for last tile and regular plant piece for the rest
        for (int i = 0;i < affectedTiles.Length; i++)
        {
            if(i == affectedTiles.Length - 1)
            {
                changer.RemoveObstacle(affectedTiles[i]);
                changer.PlacePlantTopTileAt(affectedTiles[i]);
            }
            else if (i == 0)
            {
                changer.RemoveObstacle(affectedTiles[i]);
                changer.PlacePlantBottomTileAt(affectedTiles[i]);
            }
            else
            {
                changer.RemoveObstacle(affectedTiles[i]);
                changer.PlacePlantMiddleTileAt(affectedTiles[i]);
            }
            yield return new WaitForSeconds(plantGrowSpeed);
        }
        isCasting = false;
    }

    public void castLight()
    {
        if (isCasting) return;

        animator = playerController.GetComponentInParent<PlayerAnimationRefs>().Animator;
        animator.SetTrigger("Cast");

        if (audioManager != null) audioManager.GetComponent<AudioManager>().PlayLightAudio();

        //Get overlay and then if the overlay is darkness, remove or lessen the darkness
        if (darknessOverlay.activeSelf)
        {
            darknessOverlay.SetActive(false);
            if (!lightOverlay.activeSelf)
            {
                lightOverlay.SetActive(true);
            }
        }
    }

    public void EnableDarkness()
    {
        if (!darknessOverlay.activeSelf)
        {
            darknessOverlay.SetActive(true);
        }
    }

    public void DisableDarkness()
    {
        darknessOverlay.SetActive(false);
        lightOverlay.SetActive(false);
    }
}
