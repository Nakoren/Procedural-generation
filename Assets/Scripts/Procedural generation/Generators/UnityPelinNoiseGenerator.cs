using UnityEngine;

public class TerrainGenerator : BaseTerrainGenerator
{
    public float scale = 1f;

    public override int Seed { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public override float[,] GenerateMatrix(int size, int xOffStep, int yOffStep, BiomData[,] biomMap)
    {
        float[,] heights = new float[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float xCoord = (float)x / size;
                float yCoord = (float)y / size;
                heights[x, y] = Mathf.PerlinNoise(xCoord, yCoord);
            }
        }

        return heights;
    }
}