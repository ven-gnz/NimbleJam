using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TerrainType : byte
{
    Empty = 0,
    Rock = 1,
    Soft = 2
}

[System.Serializable]
public class TerrainTileMapping
{
    public TileBase tile;
    public TerrainType terrain;
}


public class TerrainQueryHelper : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return tilemap.WorldToCell(worldPosition);
    }


    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TerrainTileMapping[] terrainMappings;

    private readonly Dictionary<Vector3Int, GameObject> terrainColliders = new();

    private byte[] terrainData;

    private int width;
    public int Width => width;
    private int height;
    public int Height => height;
    private BoundsInt bounds;

    private void Awake()
    {
        SampleTilemap();
        CreateTerrainColliders();
    }

    private void SampleTilemap()
    {
        bounds = tilemap.cellBounds;

        width = bounds.size.x;
        height = bounds.size.y;

        terrainData = new byte[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3Int cell = new Vector3Int(
                    bounds.xMin + x,
                    bounds.yMin + y,
                    0
                );

                TileBase tile = tilemap.GetTile(cell);

                terrainData[y * width + x] =
                    GetTerrainByte(tile);
            }
        }

    }


    private byte GetTerrainByte(TileBase tile)
    {

        foreach (var mapping in terrainMappings)
        {
            if (mapping.tile == tile)
                return (byte)mapping.terrain;
        }

        Debug.LogError($"No terrain mapping found for tile: {tile.name} - complete the mapping in editor");
        return (byte)TerrainType.Empty;
    }

    public TerrainType GetTerrain(Vector3Int cell)
    {
        int x = cell.x - bounds.xMin;
        int y = cell.y - bounds.yMin;

        if (x < 0 || x >= width ||
            y < 0 || y >= height)
        {
            return TerrainType.Empty;
        }

        return (TerrainType)terrainData[y * width + x];
    }

    private void CreateTerrainColliders()
    {
        foreach (Vector3Int cell in GetAllCells())
        {
            TerrainType terrain = GetTerrain(cell);

            if (terrain == TerrainType.Empty)
                continue;

            CreateTerrainCollider(cell);
        }
    }

    private IEnumerable<Vector3Int> GetAllCells()
    {
        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                yield return new Vector3Int(x, y, 0);
            }
        }
    }

    private void CreateTerrainCollider(Vector3Int cell)
    {
        GameObject obj = new GameObject(
            $"Terrain_{cell.x}_{cell.y}");

        obj.transform.SetParent(transform);

        obj.transform.position =
            tilemap.GetCellCenterWorld(cell);

        BoxCollider2D collider =
            obj.AddComponent<BoxCollider2D>();

        collider.size = tilemap.cellSize;

        terrainColliders.Add(cell, obj);

        // Debug visualization
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = tilemap.GetSprite(cell);
        renderer.color = new Color(1f, 0f, 0f, 0.25f);

        Debug.Log($"cellSize = {tilemap.cellSize}");
        Debug.Log($"localScale = {tilemap.transform.localScale}");
        Debug.Log($"lossyScale = {tilemap.transform.lossyScale}");
        Debug.Log($"cellGap = {tilemap.cellGap}");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (var cell in terrainColliders.Values)
        {
            Gizmos.DrawWireCube(
                cell.transform.position,
                new Vector3(0.8f, 0.8f, 0.0f)
            );
        }
    }

}
