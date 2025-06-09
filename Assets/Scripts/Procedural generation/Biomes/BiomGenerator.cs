using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BiomGenerator : MonoBehaviour
{
    [SerializeField] Biom[] biomsList;
    [SerializeField] Biom defaultBiom;
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
        }
    }

    private void Awake()
    {
        SplitBiomSeed();
        m_humidityPerlinNoise = new PerlinNoise(m_humidityGenerationSeed);
        m_temperaturePerlinNoise = new PerlinNoise(m_temperatureGenerationSeed);
    }

    public void ApplyBiom(TerrainData terrainData, Vector2 offset, int size)
    {
        offset = new Vector2(offset.y, offset.x);
        Biom[,] biomMap = new Biom[size, size];
        List<Biom> usedBioms = new List<Biom>(); 
        Vector2 areaCenter = new Vector2(size * offset.x - offset.x, size * offset.y - offset.y);
        int halfSize = size / 2;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 pointPosition = new Vector2(areaCenter.x + (x - halfSize), areaCenter.y + (y - halfSize));

                Biom currentBiom = GetBiomAtPoint(pointPosition);
                biomMap[x, y] = currentBiom;
                if (!usedBioms.Contains(currentBiom))
                {
                    usedBioms.Add(currentBiom);
                }
            }
        }

        List<TerrainLayer> layerList = new List<TerrainLayer>();
        foreach (Biom biom in usedBioms)
        {
            layerList.Add(biom.terrainLayer);
        }
        terrainData.terrainLayers = layerList.ToArray();
        
        float[,,] alphaMaps = new float[size,size, layerList.Count];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for(int layerInd = 0; layerInd < usedBioms.Count; layerInd++)
                {
                    Biom targetBiom = biomMap[x, y];
                    if(targetBiom == usedBioms[layerInd]) {
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

    private Biom GetBiomAtPoint(Vector2 point)
    {
        float humidityPerlinValue = m_humidityPerlinNoise.GetValueAtPoint(point, perlinNoisePeriod) * (float)1.8;
        float biomHumidity = Mathf.Min(((humidityPerlinValue / 2) + (float)0.5) * 100 * (float)1.8,100);
        float temperaturePerlinValue = m_temperaturePerlinNoise.GetValueAtPoint(point, perlinNoisePeriod) * (float)1.8;
        float biomTemperature = Mathf.Min(((temperaturePerlinValue / 2) + (float)0.5) * 100 ,100);

        Biom resBiom = null;
        foreach(Biom biom in biomsList)
        {
            if(biom.CheckBiom(biomHumidity, biomTemperature))
            {
                resBiom = biom;
            }
        }
        if (resBiom == null) resBiom = defaultBiom;
        return resBiom;
    }

    private void SplitBiomSeed()
    {
        m_humidityGenerationSeed = biomGenerationSeed ^ 1657;
        if(m_humidityPerlinNoise != null) m_humidityPerlinNoise.Seed = m_humidityGenerationSeed;

        m_temperatureGenerationSeed = biomGenerationSeed ^ 953;
        if (m_temperaturePerlinNoise != null) m_temperaturePerlinNoise.Seed = m_temperatureGenerationSeed;
    }
}
