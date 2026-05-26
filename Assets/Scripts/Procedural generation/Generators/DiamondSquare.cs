using UnityEngine;

public class DiamondSquare
{
	private TimeLimitedChunkMap m_globalDataMap;
	private int m_seed;
	private int m_chunkSize;
	private float m_roughness;

	public int Seed
	{
		get => m_seed;
		set {
			m_seed = value;
		}
	}
	public DiamondSquare(int seed, int chunkSize, float roughness)
	{
		m_seed = seed;
		m_chunkSize = chunkSize;
		m_roughness = roughness;
	}

	public float GetValueAtPoint(Vector2 position)
	{
		Vector2 positionInChunk = GetPositionInChunk(position);
		return 0;
	}

	private int GetRequiredStep(Vector2 position)
	{
		int curStep = (m_chunkSize - 1) / 2;
		while(position.x % curStep != 0)
		{
			curStep /= 2;
		}
		return curStep;
	}

	private Vector2 GetPositionInChunk(Vector2 position)
	{
		int halfChunkSize = (m_chunkSize - 1) / 2;
		return new Vector2(
			(position.x + halfChunkSize) % m_chunkSize,
			(position.y + halfChunkSize) % m_chunkSize
			);
	}

}
