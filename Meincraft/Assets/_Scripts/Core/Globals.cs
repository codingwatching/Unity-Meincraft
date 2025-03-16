using UnityEngine;

public enum BlockType : byte
{
    AIR = 0,
    GRASS = 1,
    DIRT = 2,
    STONE = 3,
    BEDROCK = 4
}
public class Globals
{
    public static readonly int ChunkSize = 16;
    public static readonly int ChunkHeight = 256;
    public static readonly int PlayerHeight = 2;

    public static readonly Vector2Int[] Directions_2D = new[]
    {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0)
    };
    public static readonly Vector3Int[] Directions_3D = new[]
    {
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 0, -1),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, 0, 0)
    };
}
