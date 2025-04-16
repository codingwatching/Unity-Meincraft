using System;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType : byte
{
    GRASS = 0,
    DIRT = 1,
    STONE = 2,
    BEDROCK = 3,
    WATER = 4,
    OAK_LOG = 5,
    OAK_LEAVES = 6,
    AIR = 255
}
public class Globals
{
    public static readonly int ChunkSize = 16;
    public static readonly int ChunkHeight = 256;
    public static readonly int PlayerHeight = 2;


    [Flags]
    public enum Direction
    {
        UP = 0,
        DOWN = 1,
        FRONT = 2,
        BACK = 3,
        LEFT = 4,
        RIGHT = 5
    }
    public static readonly Dictionary<Direction ,Vector2Int> Directions_2D = new Dictionary<Direction ,Vector2Int>
    {
        {Direction.FRONT, Vector2Int.up},
        {Direction.BACK, Vector2Int.down},
        {Direction.LEFT, Vector2Int.left},
        {Direction.RIGHT, Vector2Int.right},
    };
    public static readonly Dictionary<Direction ,Vector3Int> Directions_3D = new Dictionary<Direction ,Vector3Int>
    {
        {Direction.UP, Vector3Int.up},
        {Direction.DOWN, Vector3Int.down},
        {Direction.FRONT, Vector3Int.forward},
        {Direction.BACK, Vector3Int.back},
        {Direction.LEFT, Vector3Int.left},
        {Direction.RIGHT, Vector3Int.right}
    };

    public static Direction InvertDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.UP:
                return Direction.DOWN;
            case Direction.DOWN:
                return Direction.UP;
            case Direction.FRONT:
                return Direction.BACK;
            case Direction.BACK:
                return Direction.FRONT;
            case Direction.LEFT:
                return Direction.RIGHT;
            case Direction.RIGHT:
                return Direction.LEFT;
            default:
                return dir;
        }
    }
}
