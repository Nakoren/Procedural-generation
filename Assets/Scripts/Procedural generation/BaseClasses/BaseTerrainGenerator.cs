using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTerrainGenerator : MonoBehaviour
{
    [SerializeField] protected int seed;
    [SerializeField] protected float minTerrainHeight;

    public float MinTerrainHeight
    {
        set { minTerrainHeight = value; }
        get { return minTerrainHeight; }
    }

    abstract public float[,] GenerateMatrix(int size, int xOffStep, int yOffStep, BiomData[,] biomMap);

    abstract public int Seed { get; set; }

    protected float[,] NormalizeToPositive(float[,] source)
    {
        for(int i = 0; i < source.GetLength(0); i++)
        {
            for(int j = 0; j < source.GetLength(1); j++)
            {
                source[i,j] = (source[i,j]/2) + (float)0.5;
            }
        }
        return source;
    }

    protected float[,] Normalize(float[,] source)
    {
        float max = 0;
        for (int i = 0; i < source.GetLength(0); i++)
        {
            for (int j = 0; j < source.GetLength(1); j++)
            {
                max = Mathf.Max(max, source[i,j]);
            }
        }
        for (int i = 0; i < source.GetLength(0); i++)
        {
            for (int j = 0; j < source.GetLength(1); j++)
            {
                source[i,j] /= max;
            }
        }
        return source;
    }
    protected void DebugList<T>(List<T> list)
    {
        string res = "";
        foreach(T el in list)
        {
            res += el.ToString() + "; ";
        }
        Debug.Log(res); 
    }
    
    protected void DebugFirstRow(float[][] matrix)
    {
        string res = "";
        for (int i = 0; i < matrix[0].Length; i++)
        {
            if (i % 10 == 0) { res += '\n'; }
            res += matrix[0][i] + " ";
        }
        Debug.Log(res);
    }
}
