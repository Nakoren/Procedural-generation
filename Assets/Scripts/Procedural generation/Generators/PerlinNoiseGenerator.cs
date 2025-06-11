using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PerlinNoiseGenerator : Generator
{ 
    [SerializeField] int lowFrequencyPeriod;
    [SerializeField] int middleFrequencyPeriod;
    [SerializeField] int highFrequencyPeriod;

    [SerializeField] float defaultLowFrequencyAmplitude;
    [SerializeField] float defaultMiddleFrequencyAmplitude;
    [SerializeField] float defaultHighFrequencyAmplitude;

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

                //summ[i, j] = lowFrequencyNoise[i, j] * defaultLowFrequencyAmplitude + middleFrequencyNoise[i, j] * defaultMiddleFrequencyAmplitude + highFrequencyNoise[i, j] * defaultHighFrequencyAmplitude;
                summ[i, j] = lowFrequencyNoise[i, j] * finalLowFrequency + middleFrequencyNoise[i, j] * finalMiddleFrequency + highFrequencyNoise[i, j] * finalHighFrequency;
            }
        }
        return summ;
    }

    public float[,] GenerateSingleOctaveNoise(int size, int period, int xOffSet, int yOffSet)
    {
        float[,] resMatrix = new float[size, size];
        int halfSize = size / 2;
        Vector2 areaCenter = new Vector2(size * xOffSet - xOffSet, size * yOffSet - yOffSet);
        for (int x = 0; x < size; x++)
        {
            float temp = 0;
            for (int y = 0; y < size; y++)
            {
                Vector2 pointPosition = new Vector2(areaCenter.x + (x - halfSize), areaCenter.y + (y - halfSize));
                temp = m_perlinNoiseGenerator.GetValueAtPoint(pointPosition, period);
                resMatrix[x, y] = temp;
            }
        }
        float[,] normalizedMatrix = NormalizeToPositive(resMatrix);
        return normalizedMatrix;
    }

    private float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
}