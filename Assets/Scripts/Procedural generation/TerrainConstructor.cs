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

    [Header("Insert here game object with generator, which you want to use")]
    [SerializeField] Generator terrainGenerator;
    [SerializeField] BiomGenerator biomGenerator;
    [SerializeField] RiverCarver riverCarver;
    [SerializeField] VegetationGenerator vegetationGenerator;

    BiomData[,] m_biomDataMap;
    Biom[,] m_biomMap;

    void Awake()
    {
        terrainGenerator.Seed = generationSeed;
        //biomGenerator.Seed = generationSeed;
        riverCarver.Seed = generationSeed;
        vegetationGenerator.Seed = generationSeed;
    }

    public ChunkController ConstructTerrain(Vector2 offset)
    {
        TerrainData terrainData = new TerrainData();

        terrainData.size = new Vector3(baseChunkSize, height, baseChunkSize);
        terrainData.heightmapResolution = baseChunkSize;

        ApplyBiom(terrainData, offset);
        ApplyHeights(terrainData, offset);
        CarveRivers(terrainData, offset);

        GameObject terrain = Terrain.CreateTerrainGameObject(terrainData);
        Vector3 terrainPosition = new Vector3(offset.x * baseChunkSize - baseChunkSize / 2, 0, offset.y * baseChunkSize - baseChunkSize / 2);
        GameObject terrainGameObject = Instantiate(terrain, terrainPosition, Quaternion.identity);
        terrainGameObject.name = $"{offset.x} {offset.y}";
        Destroy(terrain);

        GenerateVegetation(terrainGameObject.GetComponent<Terrain>(), offset);

        ChunkController newController = terrainGameObject.AddComponent<ChunkController>();
        newController.chunkIndex = offset;
        return newController;
    }

    private void ApplyHeights(TerrainData terrainData, Vector2 offset)
    {
        float[,] heightMap = terrainGenerator.GenerateMatrix(baseChunkSize, (int)offset.y, (int)offset.x, m_biomDataMap);
        terrainData.SetHeights(0, 0, heightMap);
    }

    private void ApplyBiom(TerrainData terrainData, Vector2 offset)
    {
        biomGenerator.ApplyBiom(terrainData, offset, baseChunkSize, out m_biomDataMap);
        m_biomMap = Biom.ConvertTerrainDataArray(m_biomDataMap);
    }

    private void CarveRivers(TerrainData terrainData, Vector2 offset)
    {
        riverCarver.CarveRivers(terrainData, offset, baseChunkSize, m_biomMap, out m_biomMap);
    }

    private void GenerateVegetation(Terrain terrain ,Vector2 offset)
    {
        vegetationGenerator.ApplyVegetation(terrain, offset, baseChunkSize, m_biomMap);
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
