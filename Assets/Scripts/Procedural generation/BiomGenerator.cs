using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class BiomData
{
    public Biom biom;
    public int humidity;
    public int temperature;

    public BiomData(Biom biom, int humidity, int temperature)
    {
        this.biom = biom;
        this.humidity = humidity;
        this.temperature = temperature;
    }
}

public class BiomGenerator : MonoBehaviour
{
    [SerializeField] Biom[] biomsList;
    [SerializeField] Biom placeHolderBiom;

    [SerializeField] int perlinNoisePeriod;
    [SerializeField] int biomGenerationSeed;

    int m_humidityGenerationSeed;
    int m_temperatureGenerationSeed;

    private PerlinNoise m_humidityPerlinNoise;
    private PerlinNoise m_temperaturePerlinNoise;

    public int Seed
    {
        get { return biomGenerationSeed; }
        set { 
            biomGenerationSeed = value; 
            SplitBiomSeed();
        }
    }

    private void Awake()
    {
        SplitBiomSeed();
        m_humidityPerlinNoise = new PerlinNoise(m_humidityGenerationSeed);
        m_temperaturePerlinNoise = new PerlinNoise(m_temperatureGenerationSeed);
    }

    public void ApplyBiom(TerrainData terrainData, Vector2 offset, int size, out BiomData[,] resBiomData)
    {
        offset = new Vector2(offset.y, offset.x);
        Vector2 areaCenter = new Vector2(size * offset.x - offset.x, size * offset.y - offset.y);
        BiomData[,] biomMap = GetBiomMap(size, areaCenter);

        List<Biom> usedBioms = new List<Biom>();
        for(int i = 0; i < biomMap.GetLength(0); i++)
        {
            for(int j = 0; j < biomMap.GetLength(1); j++)
            {
                if (!usedBioms.Contains(biomMap[i, j].biom))
                {
                    usedBioms.Add(biomMap[i, j].biom);
                }
            }
        }

        ApplyBiomTextures(terrainData, size, usedBioms, biomMap);
        resBiomData = biomMap;
    }

    private BiomData[,] GetBiomMap(int size, Vector2 areaCenter)
    {
        BiomData[,] biomMap = new BiomData[size, size];
        int halfSize = size / 2;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 pointPosition = new Vector2(areaCenter.x + (x - halfSize), areaCenter.y + (y - halfSize));

                BiomData currentBiom = GetBiomAtPoint(pointPosition);
                biomMap[x, y] = currentBiom;    
            }
        }
        return biomMap;
    }

    private BiomData GetBiomAtPoint(Vector2 point)
    {
        float humidityPerlinValue = Mathf.Max(m_humidityPerlinNoise.GetValueAtPoint(point, perlinNoisePeriod) * (float)1.8, -1);
        float biomHumidity = Mathf.Min(((humidityPerlinValue / 2) + (float)0.5) * 100, 100);
        float temperaturePerlinValue = Mathf.Max(m_temperaturePerlinNoise.GetValueAtPoint(point, perlinNoisePeriod) * (float)1.8, -1);
        float biomTemperature = Mathf.Min(((temperaturePerlinValue / 2) + (float)0.5) * 100,100);

        Biom resBiom = null;
        foreach(Biom biom in biomsList)
        {
            if(biom.CheckBiom(biomHumidity, biomTemperature))
            {
                resBiom = biom;
            }
        }
        if (resBiom == null) {
            resBiom = placeHolderBiom;
        }
        return new BiomData(resBiom, (int)biomHumidity, (int)biomTemperature);
    }

    private void ApplyBiomTextures(TerrainData terrainData, int size, List<Biom> usedBioms, BiomData[,] biomMap) {
        List<TerrainLayer> layerList = new List<TerrainLayer>();
        foreach (Biom biom in usedBioms)
        {
            layerList.Add(biom.terrainLayer);
        }
        terrainData.terrainLayers = layerList.ToArray();

        float[,,] alphaMaps = new float[size, size, layerList.Count];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int layerInd = 0; layerInd < usedBioms.Count; layerInd++)
                {
                    Biom targetBiom = biomMap[x, y].biom;
                    if (targetBiom == usedBioms[layerInd])
                    {
                        alphaMaps[x, y, layerInd] = 1;
                    }
                    else
                    {
                        alphaMaps[x, y, layerInd] = 0;
                    }
                }
            }
        }
        terrainData.alphamapResolution = size;
        terrainData.SetAlphamaps(0, 0, alphaMaps);
    }

    //Split a biom seed to a 2 pseudo-random seeds
    private void SplitBiomSeed()
    {
        m_humidityGenerationSeed = (biomGenerationSeed ^ 1657);
        if(m_humidityPerlinNoise != null) m_humidityPerlinNoise.Seed = m_humidityGenerationSeed;

        m_temperatureGenerationSeed = (biomGenerationSeed ^ 953);
        if (m_temperaturePerlinNoise != null) m_temperaturePerlinNoise.Seed = m_temperatureGenerationSeed;
    }
}