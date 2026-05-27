using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.LightingExplorerTableColumn;

public class TimeLimitedChunkMap : TimeLimitedContainer<Vector2, float>
{
	private Dictionary<Vector2, float> m_map = new Dictionary<Vector2, float>();
	private Dictionary<Vector2, int> m_timeMap = new Dictionary<Vector2, int>();

	private int m_existTime;
	private int m_chunkSize;

	public TimeLimitedChunkMap(int existTime, int chunkSize)
	{
		m_existTime = existTime;
		m_chunkSize = chunkSize;
	}

	public override void Set(Vector2 key, float data)
	{
		if (!m_map.ContainsKey(key))
		{
			m_map.Add(key, data);
		}
		else
		{
			m_map[key] = data;
		}
		Vector2 chunkPosition = GetChunkByPosition(key);
		AddOrResetTimer(chunkPosition);
	}

	public override float Get(Vector2 key)
	{
		if (!m_map.ContainsKey(key))
		{
			return 0;
		}
		float valueToReturn = m_map[key];
		DecreaseTimer();
		Vector2 chunkPosition = GetChunkByPosition(key);
		AddOrResetTimer(chunkPosition);
		return valueToReturn;
	}

	public override bool Contains(Vector2 key)
	{
		return m_map.ContainsKey(key);
	}

	private Vector2 GetChunkByPosition(Vector2 position)
	{
		Vector2 relativePosition = new Vector2((position.x + (m_chunkSize / 2)) / m_chunkSize, (position.y + (m_chunkSize / 2)) / m_chunkSize);
		Vector2 chunkPosition = new Vector2(
			relativePosition.x > 0 ? Mathf.FloorToInt(relativePosition.x) : Mathf.CeilToInt(relativePosition.x),
			relativePosition.y > 0 ? Mathf.FloorToInt(relativePosition.y) : Mathf.CeilToInt(relativePosition.y)
			);
		return chunkPosition;
	}

	private List<Vector2> GetAllPositionsInChunk(Vector2 chunk) { 
		List<Vector2> chunkPositions = new List<Vector2>();
		int halfChunkSize = Mathf.FloorToInt(m_chunkSize / 2);
		Vector2 startPosition = new Vector2(
			m_chunkSize * chunk.x - halfChunkSize,
			m_chunkSize * chunk.y - halfChunkSize
			);
		for(int i = 0; i < m_chunkSize; i++)
		{
			for(int j = 0; j < m_chunkSize; j++)
			{
				chunkPositions.Add(new Vector2(startPosition.x + i,startPosition.y + j)); 
			}
		}
		return chunkPositions;
	}

	private void ClearMapChunk(Vector2 chunkIndex)
	{
		List<Vector2> valuesToRemove = GetAllPositionsInChunk(chunkIndex);
		foreach(Vector2 valueKey in valuesToRemove)
		{
			m_map.Remove(valueKey);
		}
	}

	private void AddOrResetTimer(Vector2 chunkIndex)
	{
		if (m_timeMap.ContainsKey(chunkIndex))
		{
			m_timeMap[chunkIndex] = m_existTime;
		}
		else
		{
			m_timeMap.Add(chunkIndex, m_existTime);
		}
	}

	private void DecreaseTimer()
	{
		foreach (Vector2 chunkKey in m_timeMap.Keys)
		{
			if (m_timeMap[chunkKey] <= 1)
			{
				m_timeMap.Remove(chunkKey);
				ClearMapChunk(chunkKey);
				if (onDataClear != null)
				{
					onDataClear.Invoke();
				}
			}
		}
	}
}

