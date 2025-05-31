using UnityEngine;

public class ChunkData
{
    private byte[,,] _blocks;
    
    public ChunkPosition ChunkPosition;

    public ChunkData(Vector2Int chunkPosition)
    {
        ChunkPosition = new ChunkPosition(chunkPosition);
        _blocks = new byte[Globals.ChunkSize,Globals.ChunkHeight,Globals.ChunkSize];
    }

    public void SetBlocks(byte[,,] blocks)
    {
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
        return _blocks[x, y, z];
    }

    public byte GetBlock(Vector3Int blockPos)
    {
        return GetBlock(blockPos.x, blockPos.y, blockPos.z);
    }

    public void SetBlock(int x, int y, int z, byte block)
    {
        if(IsWithinChunk(x,y,z)) _blocks[x, y, z] = block;
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