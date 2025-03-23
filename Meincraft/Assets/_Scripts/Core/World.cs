using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class World : Singleton<World>
{
    [SerializeField] TerrainGenerator terrainGenerator;
    [SerializeField] BlockLibrary blockLibrary;
    [SerializeField] Player player;
    [SerializeField] private bool SpawnPlayerAtWorldCenter = false;
    
    [Space(10)]
    [SerializeField] private ChunkPooling chunkPool;
    
    Vector3Int playerCurrentChunkCoordinates;

    [Space(10)] public int ChunkLoadRadius = 6;
    [SerializeField] private int chunkLoadPerFrame = 4;
    [SerializeField] private int chunkRemovePerFrame = 4;
    
    
    Queue<Vector2Int> _chunksToLoad = new Queue<Vector2Int>();
    Queue<Vector2Int> _chunksToRemove = new Queue<Vector2Int>();
    HashSet<Vector2Int> _activeChunks = new HashSet<Vector2Int>();

    Dictionary<Vector2Int, Chunk> _chunks = new Dictionary<Vector2Int, Chunk>();

    
    [Space(10)]
    [SerializeField] Chunk ChunkPrefab;
    
    //Cached variables
    private int _worldWidthBlockCount;
    private int _chunkCenter;
    private float _squaredChunkLoadRadius;

    private void Start()
    {
        _squaredChunkLoadRadius = Mathf.Pow(ChunkLoadRadius + 0.5f, 2);
        chunkPool.InitializePool((int)_squaredChunkLoadRadius);
        
        //Precompute frequently used values for avoid repeated calculations
        _worldWidthBlockCount = terrainGenerator.WorldSizeInChunks * Globals.ChunkSize;
        _chunkCenter = Globals.ChunkSize / 2;
        
        terrainGenerator.Initialize();
        
        int xPos, zPos;
        if (SpawnPlayerAtWorldCenter)
        {
            xPos = _worldWidthBlockCount/2;
            zPos = _worldWidthBlockCount/2;
        }
        else
        {
            //Get random position in random chunk
            int randomChunkX = Random.Range(0, terrainGenerator.WorldSizeInChunks / 2);
            int randomChunkZ = Random.Range(0, terrainGenerator.WorldSizeInChunks / 2);
            xPos = (randomChunkX * Globals.ChunkSize) + _chunkCenter;
            zPos = (randomChunkZ * Globals.ChunkSize) + _chunkCenter;
        }
        int height = GetSurfaceHeight(xPos, zPos);
        player.Spawn(new Vector3(xPos + 0.5f, height + 1.5f, zPos + 0.5f));
        
        playerCurrentChunkCoordinates = GetPlayerChunkCoordinates();
        LoadChunksAroundPlayer(GetChunkStacksAroundPlayer());
        
    }
    private void Update()
    {
        Vector3Int playerChunkCoordinates = GetPlayerChunkCoordinates();
        if (playerCurrentChunkCoordinates != playerChunkCoordinates)
        {
            playerCurrentChunkCoordinates = playerChunkCoordinates;
            OnPlayerChunkChanged();
        }
        
        int loadedChunks = 0;
        while (loadedChunks < chunkLoadPerFrame)
        {
            if (_chunksToLoad.TryDequeue(out Vector2Int chunkToLoad))
            {
                if (_chunks.TryGetValue(chunkToLoad, out Chunk chunk))
                {
                    chunk.Clear();
                    chunk.GenerateMeshData();
                    chunk.Load();
                    _activeChunks.Add(chunkToLoad);
                }
            }

            loadedChunks++;
        }

        int removedChunks = 0;
        while (removedChunks < chunkRemovePerFrame)
        {
            if (_chunksToRemove.TryDequeue(out Vector2Int chunkToRemove))
            {
                if (_chunks.TryGetValue(chunkToRemove, out Chunk c))
                {
                    c.UnLoad();
                }
                _activeChunks.Remove(chunkToRemove);
            }

            removedChunks++;
        }
    }

    private void OnPlayerChunkChanged()
    {
        Vector2Int[] chunkStacksAroundPlayer = GetChunkStacksAroundPlayer();
        
        List<Vector2Int> chunksToRemoveXZ = new List<Vector2Int>();
        
        //Cleaning the queue for avoid repetitions
        _chunksToRemove.Clear();
        //if the chunk is loaded but outside the view distance
        foreach (var loadedChunkStack in _activeChunks.Except(chunkStacksAroundPlayer))
        {
            chunksToRemoveXZ.Add(loadedChunkStack);
            _chunksToRemove.Enqueue(loadedChunkStack);
        }
        
        LoadChunksAroundPlayer(chunkStacksAroundPlayer);
    }

    void LoadChunksAroundPlayer(Vector2Int[] chunksAroundPlayerXZ)
    {
        _chunksToLoad.Clear();//Cleaning the queue for avoid repetitions
        Vector2Int playerChunkStack = new Vector2Int(playerCurrentChunkCoordinates.x, playerCurrentChunkCoordinates.z);
    
        //Sort chunk stacks by distance from player to load chunk stacks close to player firstly  
        Array.Sort(chunksAroundPlayerXZ, (a, b) => {
            float distA = Vector2Int.Distance(a, playerChunkStack);
            float distB = Vector2Int.Distance(b, playerChunkStack);
            return distA.CompareTo(distB);
        });

        for (int i = 0; i < chunksAroundPlayerXZ.Length; i++)
        {
            Vector2Int chunkXZ = new Vector2Int(chunksAroundPlayerXZ[i].x, chunksAroundPlayerXZ[i].y);
            if(!_chunks.ContainsKey(chunkXZ))
            {
                _chunks.Add(chunkXZ ,CreateChunk(chunkXZ));
            }
            
            if(_activeChunks.Contains(chunkXZ)) continue;//Chunk is already loaded
            _chunksToLoad.Enqueue(chunkXZ);
        }
    }

    Chunk CreateChunk(Vector2Int chunkCoordinates)
    {
        ChunkPosition chunkPosition = new ChunkPosition(chunkCoordinates);
        ChunkData chunkData = new ChunkData(terrainGenerator.GetBlocks(chunkCoordinates), chunkCoordinates * Globals.ChunkSize);
        GameObject chunkObject = chunkPool.GetObjectFromPool();
        chunkObject.transform.SetPositionAndRotation(chunkPosition.ToVector3Int() * Globals.ChunkSize, Quaternion.identity);
                
        return new Chunk(chunkData ,chunkObject, blockLibrary);
    }

    Vector2Int[] GetChunkStacksAroundPlayer()
    {
        List<Vector2Int> result = new List<Vector2Int>();
    

        for (int x = -ChunkLoadRadius; x <= ChunkLoadRadius; x++)
        {
            for (int y = -ChunkLoadRadius; y <= ChunkLoadRadius; y++)
            {
                Vector2Int chunkCoordinates = new Vector2Int(playerCurrentChunkCoordinates.x + x, playerCurrentChunkCoordinates.z + y);
            
                if (IsInViewDistance(chunkCoordinates))
                {
                    result.Add(chunkCoordinates);
                }
            }
        }

        return result.ToArray();
    }
    bool IsInViewDistance(Vector2Int pos) 
    {
        // Calculate the offset from player to the given position
        int xOffset = pos.x - playerCurrentChunkCoordinates.x;
        int yOffset = pos.y - playerCurrentChunkCoordinates.z;
        
        // Calculate squared distance (same as in GetChunkStacksAroundPlayer)
        float squaredDistance = xOffset * xOffset + yOffset * yOffset;
        
        // Check if the position is within the load radius
        if (squaredDistance <= _squaredChunkLoadRadius) 
        {
            // Check world boundaries
            if (pos.x >= 0 && pos.x < terrainGenerator.WorldSizeInChunks && 
                pos.y >= 0 && pos.y < terrainGenerator.WorldSizeInChunks) 
            {
                return true;
            }
        }
        
        return false;
    }
    public void RemoveBlock(Vector3Int blockPos)
    {
        if (GetChunkAtCoord(blockPos.x, blockPos.z, out Chunk targetChunk))
        {
            if (targetChunk.Data.GetBlock_GlobalPos(blockPos.x, blockPos.y, blockPos.z) == (byte)BlockType.BEDROCK)//Temporary
            {
                return;
            }
            targetChunk.Data.SetBlock_Global(blockPos, (byte)BlockType.AIR);
            
            targetChunk.Clear();
            targetChunk.GenerateMeshData();
            targetChunk.Load();
            targetChunk.UpdateNeighbors(blockPos.x - targetChunk.Data.ChunkPosition.x,  blockPos.z - targetChunk.Data.ChunkPosition.y);
        }
        
    }
    public void AddBlock(Vector3Int blockPos)
    {
        if (player.CheckIntersects(blockPos))
        {
            Debug.Log("You are trying to place a block inside player");
            return;
        }
        
        if (GetChunkAtCoord(blockPos.x, blockPos.z, out Chunk targetChunk))
        {
            targetChunk.Data.SetBlock_Global(blockPos, (byte)BlockType.DIRT);
            
            targetChunk.Clear();
            targetChunk.GenerateMeshData();
            targetChunk.Load();
            targetChunk.UpdateNeighbors(blockPos.x - targetChunk.Data.ChunkPosition.x, blockPos.z - targetChunk.Data.ChunkPosition.y);
        }
    }
    public bool GetChunkAtCoord(float x, float z, out Chunk chunk)
    {
        Vector2Int chunkCoordinates = new Vector2Int(
            Mathf.FloorToInt(x / Globals.ChunkSize),
            Mathf.FloorToInt(z / Globals.ChunkSize));
        if (_chunks.TryGetValue(chunkCoordinates, out chunk))
        {
            return true;
        }

        return false;
    }

    public Vector3Int GetPlayerChunkCoordinates()
    {
        return new Vector3Int(
            Mathf.FloorToInt(player.transform.position.x / Globals.ChunkSize),
            Mathf.FloorToInt(player.transform.position.y / Globals.ChunkSize),
            Mathf.FloorToInt(player.transform.position.z / Globals.ChunkSize));
    }
    public byte GetBlock(int x, int y, int z)
    {
        Vector2Int chunkCoords = new Vector2Int(Mathf.FloorToInt(x / Globals.ChunkSize), Mathf.FloorToInt(z / Globals.ChunkSize));
        
        if (_chunks.TryGetValue(chunkCoords, out Chunk chunk))
        {
            int localX = x - (chunkCoords.x * Globals.ChunkSize);
            int localZ = z - (chunkCoords.y * Globals.ChunkSize);
            
            return chunk.Data.GetBlock(localX, y, localZ);
        }
        
        return (byte)BlockType.AIR;
    }
    public bool TryGetBlock(int x, int y, int z, out byte block)
    {
        block = (byte)BlockType.AIR;
        if (!CheckCoordIsInWorldBorders(x, y, z))
            return true;//Return true because empty = air
        
        Vector2Int chunkCoords = new Vector2Int(Mathf.FloorToInt(x / Globals.ChunkSize), Mathf.FloorToInt(z / Globals.ChunkSize));
        
        if (_chunks.TryGetValue(chunkCoords, out Chunk chunk))
        {
            int localX = x - (chunkCoords.x * Globals.ChunkSize);
            int localZ = z - (chunkCoords.y * Globals.ChunkSize);

            if (chunk.Data.IsWithinChunk(localX, y, localZ))
            {
                block = chunk.Data.GetBlock(localX, y, localZ);
                return true;
            }
        }

        return false;
    }
    public bool CheckCoordIsInWorldBorders(int x, int y, int z)
    {
        return (x < _worldWidthBlockCount && x >= 0 && y < Globals.ChunkHeight && y >= 0 && z < _worldWidthBlockCount && z >= 0);
    }
    public int GetSurfaceHeight(int x, int z)
    {
        return Mathf.FloorToInt(terrainGenerator.GetHeight(x, z)) + 1;
    }

    public Bounds GetBlockBounds(int x, int y, int z)
    {
        return new Bounds(new Vector3(x, y, z) + Vector3.one / 2, new Vector3Int(1, 1, 1));
    }
}
