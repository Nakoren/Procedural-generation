using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

//This class constructs a terrain
public class TerrainConstructor : MonoBehaviour
{
    [SerializeField] int baseTerrainSize = 32;
    [SerializeField] int height = 32;
    [SerializeField] int generationSeed = 121;
    [SerializeField] bool debugValues;

    [Header("Insert here game object with generator, which you want to use")]
    [SerializeField] Generator terrainGenerator;
    [SerializeField] Terrain defaultTerrain;

    [SerializeField] bool demo;

    // Start is called before the first frame update
    void Start()
    {
        terrainGenerator.Seed = generationSeed;
    }

    public void ConstructTerrain(int xOffset, int yOffset)
    {
        //baseTerrainSize = Generator.SetNumTo2Pow(baseTerrainSize);
        float[,] heightMap = terrainGenerator.GenerateMatrix(baseTerrainSize, height, xOffset, yOffset);

        if (demo)
        {
            StartCoroutine(DemoCoroutine(heightMap));
        }
        else
        {
            if (debugValues) DebugMap(heightMap);

            defaultTerrain.terrainData.size = new Vector3(baseTerrainSize, 10, baseTerrainSize);
            //newTerrain.terrainData = new TerrainData();
            defaultTerrain.terrainData.heightmapResolution = baseTerrainSize;
            defaultTerrain.terrainData.SetHeights(0, 0, heightMap);
            //Debug.Log("Finished");
        }
    }

    private IEnumerator DemoCoroutine(float[,] heightMap)
    {
        defaultTerrain.terrainData.size = new Vector3(baseTerrainSize, height, baseTerrainSize);
        defaultTerrain.terrainData.heightmapResolution = baseTerrainSize;
        float[,] tempMap = new float[baseTerrainSize, baseTerrainSize];
        for(int x = 0; x < baseTerrainSize; x++)
        {
            for (int y = 0; y < baseTerrainSize; y++)
            {
                tempMap[x,y] = heightMap[x,y];
                defaultTerrain.terrainData.SetHeights(0, 0, tempMap);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void DebugMap(float[,] map)
    {
        string resString = "";
        for (int i = 0; i < baseTerrainSize; i++) { 
            string currentRow = "";
            for(int j = 0; j < baseTerrainSize; j++)
            {
                currentRow += $"{map[i,j]} ";
            }
            Debug.Log(currentRow);
            resString+= currentRow + "\n";
        }
        //Debug.Log(resString);
    }
}
