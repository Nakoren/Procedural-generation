using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "DefaultMountainLayer", menuName = "TerrainLayers/DefaultMountainLayer")]
public class DefaultMountainsLayer : BaseMountainLayer
{
	[SerializeField] private int mountainAreaGenerationPeriod;
	[SerializeField] private float mountainNoiseRange;
	[SerializeField] private float maxAffilationValue;

	[SerializeField] private int mountainGenerationPeriod;
	[SerializeField] private float roughness;
	[SerializeField] private int dataSaveTime;

	[SerializeField] private float modificationValue;

	private DiamondSquareGenerator m_diamondSquareGenerator;
	private PerlinNoise m_perlinNoiseGenerator;

	public override void Init()
	{
		m_diamondSquareGenerator = new DiamondSquareGenerator(m_seed, mountainGenerationPeriod, roughness, dataSaveTime);
		m_perlinNoiseGenerator = new PerlinNoise(m_seed);
	}

	protected override void CalculateLayer(ChunkData chunkData)
	{
		Vector2 offset = new Vector2(chunkData.offset.y, chunkData.offset.x);

		float[,] mountainAreaPerlinNoise = new float[chunkData.size, chunkData.size];
		mountainAreaPerlinNoise = m_perlinNoiseGenerator.GetPerlinNoiseInArea(chunkData.size, offset, mountainAreaGenerationPeriod);

		//Getting mountain affilation matrix
		float[,] mountainAffilationMap = new float[chunkData.size, chunkData.size];
		for (int x = 0; x < chunkData.size; x++)
		{
			for (int y = 0; y < chunkData.size; y++)
			{
				float absolutePerlin = Mathf.Abs(mountainAreaPerlinNoise[x, y]);
				float pointAffilation = Mathf.Max(0, 1 - absolutePerlin / mountainNoiseRange);
				mountainAffilationMap[x, y] = pointAffilation;
			}
		}

		//Getting updated heigt map for mountains
		float[,] updatedHeightMap = new float[chunkData.size, chunkData.size];
		float[,] chunkHeightMap = chunkData.GetTerrainHeightMap();
		int halfSize = mountainGenerationPeriod / 2;
		//Vector2 areaCenter = new Vector2(chunkData.size * offset.x, chunkData.size * offset.y);
		Vector2 areaCenter = new Vector2(chunkData.size * offset.x - offset.x, chunkData.size * offset.y - offset.y);
		for (int x=0; x < chunkData.size; x++)
		{
			for (int y = 0; y < chunkData.size; y++)
			{
				float pointAffilation = Mathf.Min(mountainAffilationMap[x, y] / maxAffilationValue, 1);
				Vector2 pointPosition = new Vector2(areaCenter.x + (x - halfSize), areaCenter.y + (y - halfSize));
				float DSValueAtPoint = m_diamondSquareGenerator.GetValueAtPoint(pointPosition);
				float modifiedDSValue = GetModifiedHeight(DSValueAtPoint);
				float mountainHeightValue = minMountainsHeight + (maxMountainsHeight - minMountainsHeight) * modifiedDSValue;
				float pointHeight = chunkHeightMap[x, y];
				float newHeightValue = pointHeight + (mountainHeightValue - pointHeight) * pointAffilation;
				updatedHeightMap[x, y] = newHeightValue;
			}
		}

		//Adding new terrainLayer to chunk
		int terrainLayersCount = chunkData.GetTerrainLayersCount();
		UnityEngine.TerrainLayer[] tempContainer = new UnityEngine.TerrainLayer[terrainLayersCount + 1];
		for (int i = 0; i < terrainLayersCount; i++)
		{
			tempContainer[i] = chunkData.GetTerrainLayer(i);
		}
		tempContainer[terrainLayersCount] = MountainBiom.terrainLayer;
		chunkData.SetTerrainLayers(tempContainer);

		//Getting updated alpha map for mountains (Temp relization until creation of separated generation for mountain bioms)
		float[,,] alphaMaps = chunkData.GetAlphaMaps();
		for (int x = 0; x < chunkData.size; x++)
		{
			for (int y = 0; y < chunkData.size; y++)
			{
				float pointAffilation = Mathf.Min(mountainAffilationMap[x, y] / maxAffilationValue, 1);
				if (pointAffilation == 0) continue;
				chunkData.biomMap[x, y] = new BiomData(MountainBiom, 0, 0);
				for (int i = 0; i < terrainLayersCount; i++)
				{
					alphaMaps[x, y, i] = 0;
				}
				alphaMaps[x, y, terrainLayersCount] = 1;
			}
		}
		chunkData.SetAlphaMaps(alphaMaps);
		chunkData.SetTerrainHeightMap(updatedHeightMap);
	}

	private float GetModifiedHeight(float value)
	{
		//Note: It is need to be done to avoid NaN value
		bool isNegative = value < 0;
		float res = Mathf.Pow(Mathf.Abs(value), modificationValue);
		if (isNegative) res *= -1;
		return res;
	}
}
