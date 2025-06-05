using UnityEngine;

public class TerrainGenerator : Generator
{
    public float scale = 1f;

    public override int Seed { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public override float[,] GenerateMatrix(int size, int height, int xOffStep, int yOffStep)
    {
        float[,] heights = new float[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float xCoord = (float)x / size * height;
                float yCoord = (float)y / size * height;
                heights[x, y] = Mathf.PerlinNoise(xCoord, yCoord);
            }
        }

        return heights;
    }
}