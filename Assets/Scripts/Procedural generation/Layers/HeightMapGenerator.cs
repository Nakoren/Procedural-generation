using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "DefaultHeightMapGenerator", menuName = "TerrainLayers/DefaultHeightMapLayer")]
public class HeightMapGenerator : BaseTerrainGenerator
{ 
    [SerializeField] int lowFrequencyPeriod;
    [SerializeField] int middleFrequencyPeriod;
    [SerializeField] int highFrequencyPeriod;

    [SerializeField] float defaultLowFrequencyAmplitude;
    [SerializeField] float defaultMiddleFrequencyAmplitude;
    [SerializeField] float defaultHighFrequencyAmplitude;

    [Header("Defines the minimum terrainHeight in range [0,1]\nNote: if set to 0 then rivers won't generate")]
    
    PerlinNoise m_perlinNoiseGenerator;

    override public int Seed
    {
        get { return m_seed; }
        set
        {
            m_seed = value;
            m_perlinNoiseGenerator.Seed = value;
        }
    }
    override protected void Init()
    {
        m_perlinNoiseGenerator = new PerlinNoise(m_seed);
    }

    override protected void CalculateLayer(ChunkData chunkData)
    {
        int size = chunkData.size;
        int xOffStep = (int)chunkData.offset.x;
        int yOffStep = (int)chunkData.offset.y;
        if(m_perlinNoiseGenerator == null) { Init(); }
        float[,] lowFrequencyNoise = GenerateSingleOctaveNoise(size, lowFrequencyPeriod, xOffStep, yOffStep);
        float[,] middleFrequencyNoise = GenerateSingleOctaveNoise(size, middleFrequencyPeriod, xOffStep, yOffStep);
        float[,] highFrequencyNoise = GenerateSingleOctaveNoise(size, highFrequencyPeriod, xOffStep, yOffStep);

        float[,] summ = new float[size,size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                BiomData currentBiomData = chunkData.biomMap[i, j];
                float biomAffilation = currentBiomData.biom.GetBiomAffilation(currentBiomData);
                float finalLowFrequency = Lerp(defaultLowFrequencyAmplitude, currentBiomData.biom.biomLowFrequencyAmplitude, biomAffilation);
                float finalMiddleFrequency = Lerp(defaultMiddleFrequencyAmplitude, currentBiomData.biom.biomMiddleFrequencyAmplitude, biomAffilation);
                float finalHighFrequency = Lerp(defaultHighFrequencyAmplitude, currentBiomData.biom.biomHighFrequencyAmplitude, biomAffilation);

                //summarizing all noises
                float nonModifiedHeight = lowFrequencyNoise[i, j] * finalLowFrequency + 
                    middleFrequencyNoise[i, j] * finalMiddleFrequency + 
                    highFrequencyNoise[i, j] * finalHighFrequency;
                //correcting height, so it will be normalized and placed above minimumHeight
                float modifiedHeight = minTerrainHeight + (nonModifiedHeight * (1 - minTerrainHeight));
                summ[i, j] = modifiedHeight;
            }
        }
        chunkData.SetTerrainHeightMap(summ);
    }

    public float[,] GenerateSingleOctaveNoise(int size, int period, int xOffSet, int yOffSet)
    {
        float[,] resMatrix = new float[size, size];
        resMatrix = m_perlinNoiseGenerator.GetPerlinNoiseInArea(size, new Vector2(yOffSet, xOffSet), period);
        float[,] normalizedMatrix = NormalizeToPositive(resMatrix);
        return normalizedMatrix;
    }

    private float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
}