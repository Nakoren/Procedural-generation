using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro.EditorUtilities;
using UnityEngine;

//This class constructs a terrain
[CreateAssetMenu(fileName = "TerrainConstructor", menuName = "TerrainConstructor")]
public class TerrainConstructor : ScriptableObject
{
	[SerializeField] 
	public int baseChunkSize = 32;

	[SerializeField] 
	int height = 32;

	[Header("Set \"Uniform seed\" to use single seed for all generators")]
	[SerializeField] 
	public bool uniformSeed;

	[SerializeField] 
	int generationSeed = 121;

	[Header("Insert here TerrainLayers, which you want to use")]
	[SerializeField] 
	public TerrainLayer[] TerrainLayers;

	private GameObject m_terrainContainer;
	private bool m_initialized = false;

	private void OnEnable()
	{
		m_initialized = false;
	}

	void Init()
	{
		foreach (TerrainLayer layer in TerrainLayers)
		{
			layer.Init();
			if (uniformSeed)
			{
				layer.Seed = generationSeed;
			}
		}
		
			
		if(m_terrainContainer == null)
		{
			m_terrainContainer = new GameObject("Container");
		}
		m_initialized = true;
	}

	public ChunkController ConstructTerrain(Vector2 offset)
	{
		if(!m_initialized)
		{
			Init();
		}
		TerrainData terrainData = new TerrainData();

		terrainData.size = new Vector3(baseChunkSize, height, baseChunkSize);
		terrainData.heightmapResolution = baseChunkSize;

		GameObject terrain = Terrain.CreateTerrainGameObject(terrainData);

		ChunkData chunkData = new ChunkData(
			terrain.GetComponent<Terrain>(),
			offset,
			baseChunkSize,
			new BiomData[baseChunkSize, baseChunkSize]
		);
		foreach (TerrainLayer layer in TerrainLayers)
		{
			layer.ApplyLayer(chunkData);
		}
		Vector3 terrainPosition = new Vector3(offset.x * baseChunkSize - baseChunkSize / 2, 0, offset.y * baseChunkSize - baseChunkSize / 2);
		GameObject terrainGameObject = Instantiate(terrain, terrainPosition, Quaternion.identity);
		terrainGameObject.transform.parent = m_terrainContainer.transform;
		terrainGameObject.name = $"{offset.x} {offset.y}";
		Destroy(terrain);

		ChunkController newController = terrainGameObject.AddComponent<ChunkController>();
		newController.chunkIndex = offset;
		return newController;
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
