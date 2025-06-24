using NUnit.Framework.Constraints;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RiverCarver : BaseRiverCarver
{
    [SerializeField] int perlinNoisePeriod;
    
    [Header("This parameter determines range from 0, at which values of Perlin noise river would start inflict height\n(this means that this parameter must be around 0.2)")]
    [SerializeField] float riverRange;
    private PerlinNoise m_perlinNoiseGenerator;

    public override int Seed
    {
        get { return seed; }
        set
        {
            seed = value;
            if (m_perlinNoiseGenerator != null)
            {
                m_perlinNoiseGenerator.Seed = value;
            }
        }
    }

    private void Awake()
    {
        m_perlinNoiseGenerator = new PerlinNoise(seed);
    }

    public override void CarveRivers(TerrainData terrainData, Vector2 offset, int size, Biom[,] biomMap, out Biom[,] updateBiomMap)
    {
        offset = new Vector2(offset.y, offset.x);

        float[,] riverPerlinNoise = new float[size, size];
        riverPerlinNoise = m_perlinNoiseGenerator.GetPerlinNoiseInArea(size, offset, perlinNoisePeriod);

        float[,] riverAffilationMap = new float[size, size];
        for(int x=0;x<size; x++)
        {
            for(int y=0;y<size; y++)
            {
                float absolutePerlin = Mathf.Abs(riverPerlinNoise[x, y]);
                float pointAffilation = Mathf.Max(0,1 - absolutePerlin/riverRange);
                riverAffilationMap[x,y] = pointAffilation;
            }
        }
        ApplyRivers(terrainData, size, riverAffilationMap, biomMap, out updateBiomMap);
    }
    private void ApplyRivers(TerrainData terrainData, int size, float[,] riverAffilationMap, Biom[,] biomMap, out Biom[,] updateBiomMap)
    {
        float[,] newChunkHeights = new float[size, size];

        int terrainLayersCount = terrainData.terrainLayers.Length;

        TerrainLayer[] tempContainer = new TerrainLayer[terrainLayersCount+1]; 
        for(int i = 0; i < terrainLayersCount; i++)
        {
            tempContainer[i] = terrainData.terrainLayers[i];
        }
        tempContainer[terrainLayersCount] = riverBiom.terrainLayer;
        terrainData.terrainLayers = tempContainer;

        float[,] terrainHeightMap = terrainData.GetHeights(0, 0, size, size);
        float[,,] alphaMaps = terrainData.GetAlphamaps(0, 0, size, size);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float pointAffilation = riverAffilationMap[x, y];
                if (pointAffilation < 0) continue;
                float pointHeight = terrainHeightMap[x, y];
                float newHeight = pointHeight + (minRiverHeight - pointHeight) * pointAffilation;
                newChunkHeights[x, y] = newHeight;

                if (newHeight <= maxRiverHeight)
                {
                    biomMap[x, y] = riverBiom;
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
        updateBiomMap = biomMap;
    }
}
