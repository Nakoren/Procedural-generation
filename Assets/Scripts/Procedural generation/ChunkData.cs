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
}
