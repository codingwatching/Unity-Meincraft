using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Meincraft/New Terrain Generator",fileName ="New Terrain Generator")]
public class TerrainGenerator : ScriptableObject
{
    [SerializeField] private int BaseHeight = 64;

    public int WorldSizeInChunks = 64;

    [Space(10)] public int Seed = 1337;
    [Serializable] public class NoiseData
    {
        public FastNoiseLite.NoiseType NoiseType;
        public float Frequency = 0.01f;
        public float Amplitude = 1f;
    }
    public NoiseData[] NoiseDatas;

    private FastNoiseLite[] noises;

    public void Initialize()
    {
        noises = new FastNoiseLite[NoiseDatas.Length];
        for (int i = 0; i < NoiseDatas.Length; i++)
        {
            noises[i] = new FastNoiseLite(Seed);
            noises[i].SetNoiseType(NoiseDatas[i].NoiseType);
            noises[i].SetFrequency(NoiseDatas[i].Frequency);
        }
        Random.InitState(Seed);
    }
    public byte[,,] GetBlocks(Vector2Int chunkStackPosition)
    {
        var result = new byte[Globals.ChunkSize, Globals.ChunkHeight, Globals.ChunkSize];

        for (int x = 0; x < Globals.ChunkSize; x++)
        {
            for (int z = 0; z < Globals.ChunkSize; z++)
            {                
                int globalXPos = chunkStackPosition.x * Globals.ChunkSize + x;
                int globalZPos = chunkStackPosition.y * Globals.ChunkSize + z;
                float height = GetHeight(globalXPos, globalZPos);

                for (int y = 0; y < height; y++)
                {
                    if (y == 0)
                    {
                        result[x, y, z] = (byte)BlockType.BEDROCK;
                        continue;
                    }
                    if (height - y <= 1) result[x, y, z] = (byte)BlockType.GRASS;
                    else
                    {
                        int dirtHeight = (int)(Random.value * 7);
                        dirtHeight = Mathf.Clamp(dirtHeight ,3, 7);
                        if(height - dirtHeight <= y)
                        {
                            result[x, y, z] = (byte)BlockType.DIRT;
                        }
                        else if(y>0)
                        {
                            result[x, y, z] = (byte)BlockType.STONE;
                            if (y < 7)
                            {
                                if (((Random.value * Random.value) + (result[x, y - 1, z] == (byte)BlockType.BEDROCK ? 0.25f : 0f)) > 0.4f)
                                {
                                    result[x, y, z] = (byte)BlockType.BEDROCK;
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    public float GetHeight(int x, int z)
    {
        float result = BaseHeight;

        for (int i = 0; i < noises.Length; i++)
        {
            float noise = noises[i].GetNoise(x, z);
            noise = (noise + 1) / 2f; //normalize
            result += noise * NoiseDatas[i].Amplitude;
        }
        return result;
    }
}
