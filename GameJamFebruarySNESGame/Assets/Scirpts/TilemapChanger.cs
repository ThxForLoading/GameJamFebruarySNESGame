using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapChanger : MonoBehaviour
{
    [SerializeField] Tilemap waterTileMap;
    [SerializeField] Tile iceTile;
    [SerializeField] Tilemap iceTileMap;
    [SerializeField] Tilemap obstacles;
    [SerializeField] Tilemap fireTileMap;
    [SerializeField] Tile fireTile;
    [SerializeField] public Tilemap groundTileMap;

    GameObject gridTileMap;

    private void Awake()
    {
        gridTileMap = GameObject.FindGameObjectWithTag("TilemapGrid");
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    public bool CanPlaceIce(Vector3Int position)
    {
        if(obstacles.GetTile(position) != null) return false;
        if(waterTileMap.GetTile(position) == null) return false;
        return true;
    }

    public bool CanPlaceFire(Vector3Int position)
    {
        if (obstacles.GetTile(position) != null) return false;
        if (waterTileMap.GetTile<Tile>(position) != null) return false;
        return true;
    }

    public void RemoveObstacle(Vector3Int position)
    {
        obstacles.SetTile(position, null);
    }

    public void RemoveWater(Vector3Int position)
    {
        waterTileMap.SetTile(position, null);
    }

    public void RemoveFire(Vector3Int position)
    {
        fireTileMap.SetTile(position, null);
    }

    public void PlaceIceTileat(Vector3Int position)
    {
        iceTileMap.SetTile(position, iceTile);
    }

    public void PlaceFireTileAt(Vector3Int position)
    {
        fireTileMap.SetTile(position, fireTile);
    }
}
