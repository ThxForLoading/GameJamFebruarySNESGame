using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class TilemapChanger : MonoBehaviour
{
    [SerializeField] Tilemap waterTileMap;
    [SerializeField] Tile iceTile;
    [SerializeField] Tilemap iceTileMap;
    [SerializeField] Tilemap obstacles;

    void Start()
    {
        PlaceIceTileat(new Vector3Int(0, 0, 0));
        RemoveObstacle(new Vector3Int(5,2,0));
    }

    void Update()
    {
        
    }



    public void RemoveObstacle(Vector3Int position)
    {
        obstacles.SetTile(position, null);
    }

    public void PlaceIceTileat(Vector3Int position)
    {
        iceTileMap.SetTile(position, iceTile);
    }
}
