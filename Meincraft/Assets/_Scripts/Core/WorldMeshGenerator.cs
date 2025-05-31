using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class TerrainMeshData
{
    public readonly MeshBuilder SolidMeshBuilder;
    public readonly MeshBuilder WaterMeshBuilder;
    public readonly Vector2Int ChunkPosition;

    public TerrainMeshData(MeshBuilder solidMeshBuilder, MeshBuilder waterMeshBuilder, Vector2Int chunkPosition)
    {
        SolidMeshBuilder = solidMeshBuilder;
        WaterMeshBuilder = waterMeshBuilder;
        ChunkPosition = chunkPosition;
    }
}
public static class WorldMeshGenerator
{
    public static async Task<TerrainMeshData> BuildMesh(Chunk chunk, BlockLibrary blockLibrary)
    {
        MeshBuilder solidMeshBuilder = new MeshBuilder();
        MeshBuilder waterMeshBuilder = new MeshBuilder();

        return await Task.Run(() =>
        {
            for (int x = 0; x < Globals.ChunkSize; x++) 
            {
                for (int z = 0; z < Globals.ChunkSize; z++)
                {
                    for (int y = 0; y < Globals.ChunkHeight; y++)
                    {
                        byte block = chunk.Data.GetBlock(x, y, z);
                        if (block != (byte) BlockType.AIR)
                        {
                            foreach (var dirKvp in Globals.Directions_3D)
                            {
                                var dir = dirKvp.Value;
                                int nx = x + dir.x;
                                int ny = y + dir.y;
                                int nz = z + dir.z;
                                if (TryGetBlock(nx, ny, nz, out byte targetBlock, chunk))
                                {
                                    if (block == (byte) BlockType.WATER)
                                    {
                                        if (targetBlock != (byte) BlockType.AIR) continue;
                                        waterMeshBuilder.AddFace(blockLibrary[block].MeshData.GetFaceData(dirKvp.Key), dirKvp.Key, new Vector3Int(x, y, z), blockLibrary[block].GetTextureSliceIndex(dirKvp.Key), blockLibrary[block].DefaultColor);
                                    }
                                    else
                                    {
                                        if(targetBlock is (byte) BlockType.AIR or (byte) BlockType.WATER || blockLibrary[targetBlock].IsTransparent)
                                        {
                                            solidMeshBuilder.AddFace(blockLibrary[block].MeshData.GetFaceData(dirKvp.Key),dirKvp.Key, new Vector3Int(x, y, z), blockLibrary[block].GetTextureSliceIndex(dirKvp.Key), blockLibrary[block].DefaultColor);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            bool TryGetBlock(int x, int y, int z, out byte block, in Chunk c)
                {
                    block = (byte)BlockType.AIR;
            
                    if (c.Data.IsWithinChunk(x, y, z))
                    {
                        block = c.Data.GetBlock(x, y, z);
                        return true;
                    }
                    else
                    {
                        //Get from neighbors
                        // Handle corner cases
                        if (x < 0 && z < 0 && 
                            c.ChunkNeighbors.ContainsKey(Globals.Direction.LEFT) && 
                            c.ChunkNeighbors[Globals.Direction.LEFT].ChunkNeighbors.ContainsKey(Globals.Direction.BACK))
                        {
                            block = c.ChunkNeighbors[Globals.Direction.LEFT].ChunkNeighbors[Globals.Direction.BACK].Data.GetBlock(
                                Globals.ChunkSize - 1, y, Globals.ChunkSize - 1);
                            return true;
                        }
                        if (x < 0 && c.ChunkNeighbors.ContainsKey(Globals.Direction.LEFT))
                        {
                            block = c.ChunkNeighbors[Globals.Direction.LEFT].Data.GetBlock(Globals.ChunkSize - 1, y, z);
                            return true;
                        }
                        if (x > Globals.ChunkSize - 1 && c.ChunkNeighbors.ContainsKey(Globals.Direction.RIGHT))
                        {
                            block = c.ChunkNeighbors[Globals.Direction.RIGHT].Data.GetBlock(0, y, z);
                            return true;
                        }
            
                        if (z < 0 && c.ChunkNeighbors.ContainsKey(Globals.Direction.BACK))
                        {
                            block = c.ChunkNeighbors[Globals.Direction.BACK].Data.GetBlock(x, y, Globals.ChunkSize - 1);
                            return true;
                        }
                        if (z > Globals.ChunkSize - 1 && c.ChunkNeighbors.ContainsKey(Globals.Direction.FRONT))
                        {
                            block = c.ChunkNeighbors[Globals.Direction.FRONT].Data.GetBlock(x, y, 0);
                            return true;
                        }
                    }
                    
                    return false;
                }

            return new TerrainMeshData(solidMeshBuilder, waterMeshBuilder, chunk.Data.ChunkPosition.Position);
        });

    }
}
