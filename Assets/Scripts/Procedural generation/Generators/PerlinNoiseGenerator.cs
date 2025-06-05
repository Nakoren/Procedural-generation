using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PerlinNoiseGenerator : Generator
{ 
    [SerializeField] int lowFrequencyPeriod;
    [SerializeField] int middleFrequencyPeriod;
    [SerializeField] int highFrequencyPeriod;

    PerlinNoise m_perlinNoiseGenerator;

    private void Start()
    {
        m_perlinNoiseGenerator = new PerlinNoise(middleFrequencyPeriod, m_generationSeed, 1);
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

    
    public override float[,] GenerateMatrix(int size, int height, int xOffStep, int yOffStep)
    {
        float[][] resultMatrix = new float[size][];
        for (int i = 0; i < size; i++)
        {
            resultMatrix[i] = new float[size];
        }

        float[,] resMatrix = new float[size, size];
        int halfSize = size / 2;
        Vector2 areaCenter = new Vector2(size * xOffStep, size * yOffStep);
        for (int x = 0; x < size; x++)
        {
            float temp = 0;
            float test;
            bool firstRow = true;
            for(int y = 0; y < size; y++)
            {
                Vector2 pointPosition = new Vector2(areaCenter.x + (x - halfSize), areaCenter.y + (y - halfSize));
                temp = m_perlinNoiseGenerator.GetValueAtPoint(pointPosition);
                resMatrix[x, y] = temp;
                resultMatrix[x][y] = temp;
                if (firstRow) test = temp;
                firstRow = false;
            }
            firstRow = true;
        }
        DebugFirstRow(resultMatrix);
        float[,] normalizedMatrix = NormalizeToPositive(resMatrix);
        //DebugFirstRow(normalizedMatrix);
        return normalizedMatrix;
    
    }
}