using UnityEngine;

public class DiamondSquareGenerator: SeededGenerator
{
	private TimeLimitedChunkMap m_globalDataMap;
	private int m_chunkSize;
	private float m_roughness;

	public enum StepType
	{
		Diamond,
		Square
	}

	public DiamondSquareGenerator(int seed, int chunkSize, float roughness, int dataSaveTime)
	{
		m_seed = seed;
		m_chunkSize = chunkSize;
		m_roughness = roughness;

		//TODO: Add chunkSize verification

		m_globalDataMap = new TimeLimitedChunkMap(chunkSize, dataSaveTime);
	}

	public override float GetValueAtPoint(Vector2 position)
	{
		//If point point is already hashed
		if (m_globalDataMap.Contains(position))
		{
			return m_globalDataMap.Get(position);
		}
		int halfChunkSize = m_chunkSize / 2;
		float heightValueAtPoint = GetRandomValueAtPoint(position);

		//If this point is anchor point
		if (((position.x + halfChunkSize) % (m_chunkSize - 1) == 0) && ((position.y + halfChunkSize) % (m_chunkSize - 1) == 0))
		{
			return SetHashAndReturn(position, heightValueAtPoint);
		}

		Vector2 positionInChunk = GetPositionInChunk(position);
		int requiredStep = GetRequiredStep(positionInChunk);

		Vector2 point1, point2, point3, point4;

		if(GetStepType(positionInChunk, requiredStep) == StepType.Diamond) {
			point1 = new Vector2(position.x, position.y + requiredStep);
			point2 = new Vector2(position.x, position.y - requiredStep);
			point3 = new Vector2(position.x - requiredStep, position.y);
			point4 = new Vector2(position.x + requiredStep, position.y);
		}
		else
		{
			point1 = new Vector2(position.x - requiredStep, position.y + requiredStep);
			point2 = new Vector2(position.x + requiredStep, position.y + requiredStep);
			point3 = new Vector2(position.x - requiredStep, position.y - requiredStep);
			point4 = new Vector2(position.x + requiredStep, position.y - requiredStep);
		}

		float point1Height = GetValueAtPoint(point1);
		float point2Height = GetValueAtPoint(point2);
		float point3Height = GetValueAtPoint(point3);
		float point4Height = GetValueAtPoint(point4);

		float averageNeighbourHeight = (point1Height + point2Height + point3Height + point4Height) / 4;
		float modifier = (float)1 / ((m_chunkSize - 1) / requiredStep);
		float heightChange = (heightValueAtPoint - 0.5f) * modifier;
		float pointHeight = averageNeighbourHeight + heightChange * m_roughness;
		return SetHashAndReturn(position, pointHeight);
	}

	private float SetHashAndReturn(Vector2 position ,float value)
	{
		m_globalDataMap.Set(position, value);
		return value;
	}

	private float GetRandomValueAtPoint(Vector2 position)
	{
		int x = (int)position.x;
		int y = (int)position.y;
		int hashedPosition = (((x + 1) * 781) % 7852 * (m_seed * 698) % 8213) + ((x + 1) * m_seed ^ y) + (((y + 1) * 356) % 6842 * (m_seed * 4256) % 4258);
		Random.InitState(hashedPosition);
		return Random.value;
	}

	private int GetRequiredStep(Vector2 position)
	{
		int curStep = (m_chunkSize - 1) / 2;
		while((position.x % curStep != 0) || (position.y % curStep != 0))
		{
			curStep /= 2;
		}
		return curStep;
	}

	private StepType GetStepType(Vector2 position, int step)
	{
		float summIndex = position.x + position.y;
		if(summIndex % (step * 2) == 0)
		{
			return StepType.Square;
		}
		else
		{
			return StepType.Diamond;
		}
	}

	private Vector2 GetPositionInChunk(Vector2 position)
	{
		int halfChunkSize = m_chunkSize / 2;

		Vector2 res =new Vector2(
			(position.x - halfChunkSize) % ((float)m_chunkSize - 1),
			(position.y - halfChunkSize) % ((float)m_chunkSize - 1)
			);

		//In case of negative coordinates we need to adjust negative Moduloes to positibe
		if(res.x <0)
		{
			res.x += m_chunkSize - 1;
		}
		if (res.y < 0)
		{
			res.y += m_chunkSize - 1;
		}
		return res;
	}
}