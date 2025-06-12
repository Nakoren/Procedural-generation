using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

//This class is a realization of Perlin noise, a core for this project
//Each object of this class creates a single-octave Perlin noise whith specific period, seed and amplitude
//Use other classes to create multi-octave perlin noise
//Use method GetValueAtPoint to get value at specific point
public class PerlinNoise
{
    RandomGradientGenerator m_gradientGenerator;
    int m_seed;

    public PerlinNoise(int seed)
    {
        m_gradientGenerator = new RandomGradientGenerator(seed);
    }

    public int Seed
    {
        get { return m_seed; }
        set { 
            m_seed = value;
            m_gradientGenerator = new RandomGradientGenerator(value);
        }
    }

    public float[,] GetPerlinNoiseInArea(int size, Vector2 offset, int period)
    {
        float[,] noise = new float[size, size];
        Vector2 areaCenter = new Vector2(size * offset.x - offset.x, size * offset.y - offset.y);
        float halfSize = size / 2;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 pointPosition = new Vector2(areaCenter.x + (x - halfSize), areaCenter.y + (y - halfSize));
                noise[x, y] = GetValueAtPoint(pointPosition, period);
            }
        }
        return noise;
    }
    public float[,] GetMultiActaveNoiseInArea(int size, Vector2 offset, int minPeriod, int octaves)
    {
        float[,] noise = new float[size, size];
        int curPeriod = minPeriod;
        int curOctave = 1;
        float currentModifier = 1;
        float summModifier = 0;
        do
        {
            summModifier += currentModifier;
            if (curPeriod % 2 == 0) curPeriod++;
            float[,] currentOctaveNoise = GetPerlinNoiseInArea(size, offset, curPeriod);
            for(int i=0; i < size; i++)
            {
                for(int j=0;j < size; j++)
                {
                    noise[i, j] += currentOctaveNoise[i, j] * currentModifier;
                }
            }
            curPeriod *= 2;
            currentModifier /= 2;

        } while (curOctave < octaves);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                noise[i, j] += noise[i, j] / summModifier;
            }
        }
        return noise;
    }
    
    public float GetValueAtPoint(Vector2 position, int period)
    {
        int halfPeriod = period / 2;

        //Calculation of center point of point's area
        //Note: this realization uses a world separation, where center of area with index (0,0) is placed at point (0,0)
        Vector2 areaCenter;

        //Calculating center of area where point is placed
        areaCenter = new Vector2(Mathf.Round(position.x / (period-1)) * (period - 1), Mathf.Round(position.y / (period - 1)) * (period - 1));

        //Calculation of relative positions of point in the area
        float horizontalPositionInArea = (Mathf.Abs((areaCenter.x - halfPeriod) - position.x)) / (float)period;
        float verticalPositionInArea = (Mathf.Abs((areaCenter.y - halfPeriod) - position.y)) / (float)period;

        //Calculation of corners of the area where point is placed
        Vector2 topLeftCorner = new Vector2((int)areaCenter.x - halfPeriod, (int)areaCenter.y + halfPeriod);
        Vector2 topRightCorner = new Vector2((int)areaCenter.x + halfPeriod, (int)areaCenter.y + halfPeriod);
        Vector2 botLeftCorner = new Vector2((int)areaCenter.x - halfPeriod, (int)areaCenter.y - halfPeriod);
        Vector2 botRightCorner = new Vector2((int)areaCenter.x + halfPeriod, (int)areaCenter.y - halfPeriod);

        //Getting gradients at the corners
        Vector2 topLeftGradient = m_gradientGenerator.GetGradientAtPoint(topLeftCorner);
        Vector2 topRightGradient = m_gradientGenerator.GetGradientAtPoint(topRightCorner);
        Vector2 botLeftGradient = m_gradientGenerator.GetGradientAtPoint(botLeftCorner);
        Vector2 botRightGradient = m_gradientGenerator.GetGradientAtPoint(botRightCorner);

        //Getting directional vectors from point to corners
        Vector2 topLeftToPoint = (position - topLeftCorner) / period;
        Vector2 topRightToPoint = (position - topRightCorner) / period;
        Vector2 botLeftToPoint = (position - botLeftCorner) / period;
        Vector2 botRightToPoint = (position - botRightCorner) / period;

        //Getting DOT product of gradients and directional vectors
        float tlDot = Dot(topLeftToPoint, topLeftGradient);
        float trDot = Dot(topRightToPoint, topRightGradient);
        float blDot = Dot(botLeftToPoint, botLeftGradient);
        float brDot = Dot(botRightToPoint, botRightGradient);

        //Getting approximation curve of point at different axis
        float xQunticCurveValue = Fade(horizontalPositionInArea);
        float yQunticCurveValue = Fade(verticalPositionInArea);

        //Approximation of horizontal borders
        float xTop = Lerp(tlDot, trDot, xQunticCurveValue);
        float xBot = Lerp(blDot, brDot, xQunticCurveValue);

        //Approximation of approximations
        float result = Lerp(xBot, xTop, yQunticCurveValue);
        return result;
    }
    static float Dot(Vector2 a, Vector2 b)
    {
        return a.x * b.x + a.y * b.y;
    }
    static float Fade(float t)
    {
        return 6*Mathf.Pow(t,5) - 15 * Mathf.Pow(t, 4) + 10 * Mathf.Pow(t, 3);
    }
    static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    private int GetChunkIndexOfValue(float val, float step)
    {
        float relativePosition = (val + (step / 2)) / step;
        if (relativePosition > 0) 
        {
            return Mathf.FloorToInt(relativePosition);
            
        }
        else
        {
            return Mathf.CeilToInt(relativePosition);
        }
    }
}
