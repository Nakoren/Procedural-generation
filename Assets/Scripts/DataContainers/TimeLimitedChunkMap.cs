using System.Collections.Generic;
using UnityEngine;

public class TimeLimitedChunkMap<DataType> : TimeLimitedContainer<Vector2, DataType>
{
	private Dictionary<Vector2, DataType> m_Map;
	private Dictionary<Vector2, int> m_timeMap;

	private int m_existTime;
	private int m_chunkSize;

	public override DataType this[Vector2 key] { 
		get
		{
			if (m_Map[key] == null)
			{
				return default(DataType);
			}
			DataType valueToReturn = m_Map[key];
			DecreaseTimer();
			Vector2 chunkPosition = GetChunkByPosition(key);
			AddOrResetTimer(chunkPosition);
			return valueToReturn;
		}
		set
		{
			if (m_Map[key] == null)
			{
				m_Map.Add(key, value);
			}
			else
			{
				m_Map[key] = value;
			}
			Vector2 chunkPosition = GetChunkByPosition(key);
			AddOrResetTimer(chunkPosition);
		}
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
			m_Map.Remove(valueKey);
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
