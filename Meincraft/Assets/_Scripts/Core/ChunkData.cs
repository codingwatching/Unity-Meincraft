using System.Collections.Generic;
using UnityEngine;

public class ChunkPosition
{
    public Vector2Int Position { get; private set; }
    public int x => Position.x;
    public int y => Position.y;
    
    public ChunkPosition(int x, int z)
    {
        Position = new Vector2Int(x, z);
    }
    
    public ChunkPosition(Vector2Int position)
    {
        this.Position = position;
    }

    public Vector3Int ToVector3Int()
    {
        return new Vector3Int(Position.x, 0, Position.y);
    }
    public override bool Equals(object obj)
    {
        if (obj is ChunkPosition other)
        {
            return Position.Equals(other.Position);
        }
        return false;
    }
    
    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }
    
    public override string ToString()
    {
        return $"({Position.x}, {Position.y})";
    }
}

public class ChunkData
{
    private byte[,,] _blocks;
    
    public ChunkPosition ChunkPosition;

    public ChunkData(byte[,,] blocks, Vector2Int chunkPosition)
    {
        ChunkPosition = new ChunkPosition(chunkPosition);
        _blocks = blocks;
    }

    public bool IsWithinChunk(int x, int y, int z)
    {
        return x >= 0 && x < Globals.ChunkSize &&
               y >= 0 && y < Globals.ChunkHeight &&
               z >= 0 && z < Globals.ChunkSize;
    }
    public bool IsWithinChunk(Vector3Int position)
    {
        return IsWithinChunk(position.x, position.y, position.z);
    }
    #region BLOCK_API
    public byte GetBlock_GlobalPos(int x, int y, int z)
    {
        return GetBlock(x - ChunkPosition.x, y, z - ChunkPosition.y);
    }
    
    public byte GetBlock_GlobalPos(Vector3Int blockPos)
    {
        return GetBlock(blockPos.x - ChunkPosition.x, blockPos.y, blockPos.z - ChunkPosition.y);
    }

    public byte GetBlock(int x, int y, int z)
    {
        // Check if the coordinates are within this chunk
        if (x >= 0 && x < Globals.ChunkSize && 
            y >= 0 && y < Globals.ChunkHeight && 
            z >= 0 && z < Globals.ChunkSize)
        {
            return _blocks[x, y, z];
        }
        
        // If outside this chunk, get block from world
        return World.Instance.GetBlock(
            ChunkPosition.x + x,
            y,
            ChunkPosition.y + z
        );
    }

    public byte GetBlock(Vector3Int blockPos)
    {
        return GetBlock(blockPos.x, blockPos.y, blockPos.z);
    }

    public void SetBlock(int x, int y, int z, byte block)
    {
        if (x >= 0 && x < Globals.ChunkSize && 
            y >= 0 && y < Globals.ChunkHeight && 
            z >= 0 && z < Globals.ChunkSize)
        {
            _blocks[x, y, z] = block;
        }
    }

    public void SetBlock(Vector3Int blockPos, byte block)
    {
        SetBlock(blockPos.x, blockPos.y, blockPos.z, block);
    }

    public void SetBlock_Global(int x, int y, int z, byte block)
    {
        SetBlock(x - ChunkPosition.x, y, z - ChunkPosition.y, block);
    }
    
    public void SetBlock_Global(Vector3Int blockPos, byte block)
    {
        SetBlock(blockPos.x - ChunkPosition.x, blockPos.y, blockPos.z - ChunkPosition.y, block);
    }
    #endregion
}