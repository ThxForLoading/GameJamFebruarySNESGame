using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class SpellHandler : MonoBehaviour
{
    PlayerControllerCore playerController;

    [SerializeField] int fireRange = 3;
    [SerializeField] float fireSpreadSpeed = 0.5f;
    [SerializeField] int iceRange = 5;
    [SerializeField] float iceSpreadSpeed = 0.5f;

    private bool isCasting = false;

    TilemapChanger changer;

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

    public void castFire()
    {
        if (isCasting) return;

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
        foreach (Vector3Int tile in affectedTiles)
        {
            if (!changer.CanPlaceFire(tile))
            {
                isCasting = false;
                yield break;
            }

            changer.PlaceFireTileAt(tile);
            yield return new WaitForSeconds(fireSpreadSpeed);
            StartCoroutine(extinguishFire(tile));
        }
        isCasting = false;
    }

    IEnumerator extinguishFire(Vector3Int target)
    {
        yield return new WaitForSeconds(fireSpreadSpeed + 0.3f);
        changer.RemoveFire(target);
    }

    public void castIce()
    {
        if(isCasting) return;

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
        foreach (Vector3Int tile in affectedTiles)
        {
            if (!changer.CanPlaceIce(tile))
            {
                isCasting = false;
                yield break;
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

        Vector3Int startingLocation = playerController.GetSelectedTile();

    }

    public void castLight()
    {
        if (isCasting) return;

        //Get overlay and then if the overlay is darkness, remove or lessen the darkness
    }
}
