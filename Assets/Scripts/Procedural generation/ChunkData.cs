using System.Linq;
using UnityEngine;

public class ChunkData
{
	public Vector2 offset;
	public int size;
	public Terrain terrain;
	public BiomData[,] biomMap;

	public ChunkData(Terrain terrain, Vector2 offset, int size, BiomData[,] biomMap)
	{
		this.terrain = terrain;
		this.offset = offset;
		this.size = size;
		this.biomMap = biomMap; 
	}
	public ChunkData(Vector2 offset, int size) {
		this.terrain = new Terrain();
		this.offset = offset;
		this.size = size;
		this.biomMap = new BiomData[size, size];
	}

	public void SetTerrainHeightMap(float[,] heightMap)
	{
		terrain.terrainData.SetHeights(0,0, heightMap);
	}

	public float[,] GetTerrainHeightMap() => terrain.terrainData.GetHeights(0, 0, size, size);

	public void SetAlphaMaps(float[,,] alphaMaps) => terrain.terrainData.SetAlphamaps(0, 0, alphaMaps);

	public float[,,] GetAlphaMaps() => terrain.terrainData.GetAlphamaps(0, 0, size, size);

	public int GetTerrainLayersCount() => terrain.terrainData.terrainLayers.Count();

	public UnityEngine.TerrainLayer GetTerrainLayer(int index) => terrain.terrainData.terrainLayers[index];

	public void SetTerrainLayers(UnityEngine.TerrainLayer[] terrainLayers) {
		terrain.terrainData.terrainLayers = terrainLayers;
	}
}
