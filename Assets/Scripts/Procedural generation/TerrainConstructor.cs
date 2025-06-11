using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro.EditorUtilities;
using UnityEngine;

//This class constructs a terrain
public class TerrainConstructor : MonoBehaviour
{
    [SerializeField] public int baseChunkSize = 32;
    [SerializeField] int height = 32;
    [SerializeField] int generationSeed = 121;
    [SerializeField] bool debugValues;

    [Header("Insert here game object with generator, which you want to use")]
    [SerializeField] Generator terrainGenerator;
    [SerializeField] BiomGenerator biomGenerator;

    [SerializeField] bool demo;

    BiomData[,] m_biomMap;

    void Awake()
    {
        terrainGenerator.Seed = generationSeed;
    }

    public ChunkController ConstructTerrain(Vector2 offset)
    {
        TerrainData terrainData = new TerrainData();

        terrainData.size = new Vector3(baseChunkSize, height, baseChunkSize);
        terrainData.heightmapResolution = baseChunkSize;

        ApplyBiom(terrainData, offset);
        ApplyHeights(terrainData, offset);

        GameObject terrain = Terrain.CreateTerrainGameObject(terrainData);
        Vector3 terrainPosition = new Vector3(offset.x * baseChunkSize - baseChunkSize / 2, -height/2, offset.y * baseChunkSize - baseChunkSize / 2);
        GameObject terrainGameObject = Instantiate(terrain, terrainPosition, Quaternion.identity);
        terrainGameObject.name = $"{offset.x} {offset.y}";
        Destroy(terrain);
        ChunkController newController = terrainGameObject.AddComponent<ChunkController>();
        newController.chunkIndex = offset;
        return newController;
    }

    private void ApplyHeights(TerrainData terrainData, Vector2 offset)
    {
        float[,] heightMap = terrainGenerator.GenerateMatrix(baseChunkSize, (int)offset.y, (int)offset.x, m_biomMap);
        if (debugValues) DebugMap(heightMap);
        terrainData.SetHeights(0, 0, heightMap);
    }

    private void ApplyBiom(TerrainData terrainData, Vector2 offset)
    {
        m_biomMap = biomGenerator.ApplyBiom(terrainData, offset, baseChunkSize);
    }

    private void DebugMap(float[,] map)
    {
        string resString = "";
        for (int i = 0; i < baseChunkSize; i++) { 
            string currentRow = "";
            for(int j = 0; j < baseChunkSize; j++)
            {
                currentRow += $"{map[i,j]} ";
            }
            Debug.Log(currentRow);
            resString+= currentRow + "\n";
        }
        //Debug.Log(resString);
    }
}
