using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeightMapGenerator : BaseTerrainGenerator
{ 
    [SerializeField] int lowFrequencyPeriod;
    [SerializeField] int middleFrequencyPeriod;
    [SerializeField] int highFrequencyPeriod;

    [SerializeField] float defaultLowFrequencyAmplitude;
    [SerializeField] float defaultMiddleFrequencyAmplitude;
    [SerializeField] float defaultHighFrequencyAmplitude;

    [Header("Defines the minimum terrainHeight in range [0,1]\nNote: if set to 0 then rivers won't generate")]
    [SerializeField] float minTerrainHeight;

    PerlinNoise m_perlinNoiseGenerator;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        m_perlinNoiseGenerator = new PerlinNoise(m_generationSeed);
    }

    override public int Seed
    {
        get { return m_generationSeed; }
        set
        {
            m_generationSeed = value;
            m_perlinNoiseGenerator.Seed = value;
        }
    }

    public override float[,] GenerateMatrix(int size, int xOffStep, int yOffStep, BiomData[,] biomMap)
    {
        if(m_perlinNoiseGenerator == null) { Init(); }
        float[,] lowFrequencyNoise = GenerateSingleOctaveNoise(size, lowFrequencyPeriod, xOffStep, yOffStep);
        float[,] middleFrequencyNoise = GenerateSingleOctaveNoise(size, middleFrequencyPeriod, xOffStep, yOffStep);
        float[,] highFrequencyNoise = GenerateSingleOctaveNoise(size, highFrequencyPeriod, xOffStep, yOffStep);
        float[,] summ = new float[size,size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                BiomData currentBiomData = biomMap[i, j];
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
        return summ;
    }

    public float[,] GenerateSingleOctaveNoise(int size, int period, int xOffSet, int yOffSet)
    {
        float[,] resMatrix = new float[size, size];
        resMatrix = m_perlinNoiseGenerator.GetPerlinNoiseInArea(size, new Vector2(xOffSet, yOffSet), period);
        float[,] normalizedMatrix = NormalizeToPositive(resMatrix);
        return normalizedMatrix;
    }

    private float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
}