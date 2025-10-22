using NUnit.Framework.Constraints;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultRiverGenerator", menuName = "TerrainLayers/DefaultRiverLayer")]
public class RiverCarver : BaseRiverCarver
{
    [SerializeField] int perlinNoisePeriod;
    
    [Header("This parameter determines range from 0, at which values of Perlin noise river would start inflict height\n(this means that this parameter must be around 0.2)")]
    [SerializeField] float riverRange;
    private PerlinNoise m_perlinNoiseGenerator;

    public override int Seed
    {
        get { return m_seed; }
        set
        {
            m_seed = value;
            if (m_perlinNoiseGenerator != null)
            {
                m_perlinNoiseGenerator.Seed = value;
            }
        }
    }

    private void Awake()
    {
        m_perlinNoiseGenerator = new PerlinNoise(m_seed);
    }

    public override void ApplyLayer(ChunkData chunkData)
    {
        Vector2 offset = new Vector2(chunkData.offset.y, chunkData.offset.x);

        float[,] riverPerlinNoise = new float[chunkData.size, chunkData.size];
        riverPerlinNoise = m_perlinNoiseGenerator.GetPerlinNoiseInArea(chunkData.size, offset, perlinNoisePeriod);

        float[,] riverAffilationMap = new float[chunkData.size, chunkData.size];
        for(int x=0;x< chunkData.size; x++)
        {
            for(int y=0;y< chunkData.size; y++)
            {
                float absolutePerlin = Mathf.Abs(riverPerlinNoise[x, y]);
                float pointAffilation = Mathf.Max(0,1 - absolutePerlin/riverRange);
                riverAffilationMap[x,y] = pointAffilation;
            }
        }
        ApplyRivers(chunkData, riverAffilationMap);
    }
    private void ApplyRivers(ChunkData chunkData, float[,] riverAffilationMap)
    {
        TerrainData terrainData = chunkData.terrain.terrainData;

        float[,] newChunkHeights = new float[chunkData.size, chunkData.size];

        int terrainLayersCount = terrainData.terrainLayers.Length;

        UnityEngine.TerrainLayer[] tempContainer = new UnityEngine.TerrainLayer[terrainLayersCount + 1]; 
        for(int i = 0; i < terrainLayersCount; i++)
        {
            tempContainer[i] = terrainData.terrainLayers[i];
        }
        tempContainer[terrainLayersCount] = riverBiom.terrainLayer;
        terrainData.terrainLayers = tempContainer;

        float[,] terrainHeightMap = terrainData.GetHeights(0, 0, chunkData.size, chunkData.size);
        float[,,] alphaMaps = terrainData.GetAlphamaps(0, 0, chunkData.size, chunkData.size);

        for (int x = 0; x < chunkData.size; x++)
        {
            for (int y = 0; y < chunkData.size; y++)
            {
                float pointAffilation = riverAffilationMap[x, y];
                if (pointAffilation < 0) continue;
                float pointHeight = terrainHeightMap[x, y];
                float newHeight = pointHeight + (minRiverHeight - pointHeight) * pointAffilation;
                newChunkHeights[x, y] = newHeight;

                if (newHeight <= m_maxRiverHeight)
                {
                    chunkData.biomMap[x, y] = new BiomData(riverBiom, 0, 0);
                    for (int i = 0; i < terrainLayersCount; i++)
                    {
                        alphaMaps[x, y, i] = 0;
                    }
                    alphaMaps[x, y, terrainLayersCount] = 1;
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, alphaMaps);
        terrainData.SetHeights(0, 0, newChunkHeights);
    }
}
