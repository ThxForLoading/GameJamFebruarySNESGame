using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TilemapChanger : MonoBehaviour
{
    [Header("Ice")]
    [SerializeField] Tilemap waterTileMap;
    [SerializeField] Tile iceTile;
    [SerializeField] Tilemap iceTileMap;
    [SerializeField] Tilemap obstacles;
    [Header("Fire")]
    [SerializeField] Tilemap fireTileMap;
    [SerializeField] Tile fireTile;
    [SerializeField] Tilemap fireDestructibleTileMap;
    [Header("Plant")]
    [SerializeField] Tilemap plantTileMap;
    [SerializeField] Tile plantBottomTile;
    [SerializeField] Tile plantTile;
    [SerializeField] Tile plantTopTile;
    [SerializeField] Tilemap fertileGround;
    [SerializeField] Tile groundPlantTile;
    [Header("Other")]
    [SerializeField] public Tilemap groundTileMap;
    [SerializeField] Tilemap savePointsTileMap;

    GameObject gridTileMap;

    private void Awake()
    {
        gridTileMap = GameObject.FindGameObjectWithTag("TilemapGrid");
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

    public bool CanPlacePlantGround(Vector3Int position)
    {
        if (obstacles.GetTile(position) != null) return false;
        if (waterTileMap.GetTile<Tile>(position) != null) return false;
        if (iceTileMap.GetTile<Tile>(position) != null) return false;
        if (savePointsTileMap.GetTile<Tile>(position) != null) return false;
        return true;
    }

    public bool CanPlacePlantClimb(Vector3Int position)
    {
        if(fertileGround.GetTile(position) == null) return false;
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

    public void RemoveBurnableTile(Vector3Int position)
    {
        fireDestructibleTileMap.SetTile(position, null);
    }

    public void PlaceIceTileat(Vector3Int position)
    {
        iceTileMap.SetTile(position, iceTile);
    }

    public void PlaceFireTileAt(Vector3Int position)
    {
        fireTileMap.SetTile(position, fireTile);
    }

    public void PlaceGroundPlantTileAt(Vector3Int position)
    {
        plantTileMap.SetTile(position, groundPlantTile);
    }

    public void PlacePlantBottomTileAt(Vector3Int position)
    {
        plantTileMap.SetTile(position,plantBottomTile);
    }

    public void PlacePlantMiddleTileAt(Vector3Int position)
    {
        plantTileMap.SetTile(position, plantTile);
    }

    public void PlacePlantTopTileAt(Vector3Int position)
    {
        plantTileMap.SetTile (position, plantTopTile);
    }
}
