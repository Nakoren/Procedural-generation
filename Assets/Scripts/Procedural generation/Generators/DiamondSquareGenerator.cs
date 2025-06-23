using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


//This generator creates a big-sized chunk using Diamond-Square algorithm. It is not usable for infinite generation
public class DiamondSquareGenerator : BaseTerrainGenerator
{
    [SerializeField] private float generationRoughness;
    [SerializeField] private int BreakInterval;
    [SerializeField] private int SmoothRange = 4;

    public override int Seed { get => m_generationSeed; set => m_generationSeed = value; }

    public override float[,] GenerateMatrix(int size, int xOffStep, int yOffStep, BiomData[,] biomMap)
    {
        size = SetNumTo2Pow(size);
        float[,] resMatrix = new float[size, size];
        
        //init step
        resMatrix[0, 0] = GetPositionRandomValue((xOffStep - 1) * size, (yOffStep - 1) * size);

        resMatrix[size - 1, 0] = GetPositionRandomValue((xOffStep) * size, (yOffStep - 1) * size);

        resMatrix[0, size - 1] = GetPositionRandomValue((xOffStep - 1) * size, (yOffStep) * size);

        resMatrix[size - 1, size - 1] = GetPositionRandomValue((xOffStep) * size, (yOffStep) * size);

        int curInterval = size - 1;
        while (true) {
            if (curInterval < BreakInterval) break;

            int radius = curInterval / 2;

            //square step
            for (int i = 0; i < (size - 1) / curInterval; i++) {
                int heightInd = (i * curInterval) + radius;
                for (int j = 0; j < (size - 1) / curInterval; j++)
                {
                    int lengthInd = (j * curInterval) + radius;
                    float topLeftVal = resMatrix[heightInd + radius, lengthInd - radius];
                    float topRightVal = resMatrix[heightInd + radius, lengthInd + radius];
                    float bottomLeftVal = resMatrix[heightInd - radius, lengthInd - radius];
                    float bottomRightVal = resMatrix[heightInd - radius, lengthInd + radius];

                    //float currentOffsetModifier = (float)curInterval / (float)(size - 1);
                    float randomOffset = GetPositionOffset(heightInd, lengthInd, curInterval);

                    float value = (topLeftVal + topRightVal + bottomLeftVal + bottomRightVal) / 4 + randomOffset;
                    resMatrix[heightInd, lengthInd] = value;
                }
            }

            //diamond step - horizontal lines
            for (int i = 0; i <= (size - 1) / curInterval; i++)
            {
                int heightInd = (i * curInterval);
                for (int j = 0; j < (size - 1) / curInterval; j++)
                {
                    int lengthInd = (j * curInterval) + radius;
                    int totalSumm = 2;

                    float topVal = 0;
                    if (i != 0) { topVal = resMatrix[heightInd - radius, lengthInd]; totalSumm++; }

                    float bottomVal = 0;
                    if (i != (size - 1) / curInterval) { bottomVal = resMatrix[heightInd + radius, lengthInd]; totalSumm++; }

                    float leftVal = resMatrix[heightInd, lengthInd - radius];
                    float rightVal = resMatrix[heightInd, lengthInd + radius];

                    //float currentOffsetModifier = (float)curInterval / (float)(size - 1);
                    float randomOffset = GetPositionOffset(heightInd, lengthInd, curInterval);

                    float value = (topVal + bottomVal + leftVal+ rightVal) / totalSumm + randomOffset;

                    resMatrix[heightInd, lengthInd] = value;
                }
            }

            //diamond step - vertical lines
            for (int i = 0; i < (size - 1) / curInterval; i++)
            {
                int heightInd = (i * curInterval) + radius;
                for (int j = 0; j <= (size - 1) / curInterval; j++)
                {
                    int lengthInd = (j * curInterval);
                    int totalSumm = 2;

                    float leftVal = 0;
                    if (j != 0) { leftVal = resMatrix[heightInd, lengthInd - radius]; totalSumm++; }

                    float rightVal = 0;
                    if (j != (size - 1) / curInterval) { rightVal = resMatrix[heightInd, lengthInd + radius]; totalSumm++; }

                    float topVal = resMatrix[heightInd + radius, lengthInd];
                    float bottomVal = resMatrix[heightInd + radius, lengthInd];

                    //float currentOffsetModifier = (float)curInterval / (float)(size - 1);
                    float randomOffset = GetPositionOffset(heightInd, lengthInd, curInterval);

                    float value = (topVal + bottomVal + leftVal + rightVal) / totalSumm + randomOffset;

                    resMatrix[heightInd, lengthInd] = value;
                }
            }

            curInterval /= 2;
        }

        return NormalizeToPositive(resMatrix);
    }

    private float GetPositionRandomValue(int x, int y)
    {
        int hashedInts = ((((x + 1) << 5) * (m_generationSeed >> 41)) + (x * m_generationSeed ^ y) + (((y + 1) << 3) * (m_generationSeed >> 7)));
        UnityEngine.Random.InitState(hashedInts);
        return UnityEngine.Random.Range(0, 10);
    }

    private float GetPositionOffset(int x, int y, double intervalModifier)
    {
        int hashedInts = ((((x + 1) << 5) * (m_generationSeed >> 41)) + (x * m_generationSeed ^ y) + (((y + 1) << 3) * (m_generationSeed >> 7)));
        UnityEngine.Random.InitState(hashedInts);
        float randomBorders = (float)intervalModifier * generationRoughness;
        //float randomBorders = (float)intervalModifier * generationRoughness * generationHeight;
        float offset = UnityEngine.Random.Range(-1 * randomBorders, randomBorders);
       
        return offset;
    }

    
    private float[,] SmoothOut(float[,] source)
    {
        for (int x = 0; x < source.GetLength(0); x++)
        {
            for (int y = 0; y < source.GetLength(1); y++)
            {
                int topDist = Mathf.Min(SmoothRange, x);
                int botDist = Mathf.Min(SmoothRange, source.GetLength(0) - x);
                int leftDist = Mathf.Min(SmoothRange, y);
                int rightDist = Mathf.Min(SmoothRange, source.GetLength(1) - y);

            }
        }

        return source;
    }
}
