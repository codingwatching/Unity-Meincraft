using System;
using System.Collections.Generic;
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


    public enum Direction
    {
        UP = 0,
        DOWN = 1,
        FRONT = 2,
        BACK = 3,
        LEFT = 4,
        RIGHT = 5
    }
    public static readonly Vector3Int[] Directions_2D = new Vector3Int[]
    {
        Vector3Int.forward,
        Vector3Int.back,
        Vector3Int.left,
        Vector3Int.right
    };
    public static readonly Vector3Int[] Directions_3D = new Vector3Int[]
    {
        Vector3Int.up ,
        Vector3Int.down ,
        Vector3Int.forward,
        Vector3Int.back,
        Vector3Int.left,
        Vector3Int.right
    };
}
