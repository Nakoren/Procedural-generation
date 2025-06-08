using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainController : MonoBehaviour
{
    [SerializeField] TerrainConstructor terrainConstructor;
    [SerializeField] Observer observer;
    [Header("Range outside which chunks will be deleted")]
    [SerializeField] int clearRange;
    [Header("Range which determines a loading distance for chunks during expansion")]
    [SerializeField] int loadRange;
    [Header("Range which determines min distance to unloaded chunks without expansion")]
    [SerializeField] int visionRange;

    private int m_chunkSize;
    private Vector2 m_prevChunk;
    private Vector2 m_currentChunk;

    private List<ChunkController> m_loadedChunksList = new List<ChunkController>();

    private void Start()
    {
        m_chunkSize = terrainConstructor.baseChunkSize;
        observer.onPositionChange += OnPlayerMove;

        Vector3 defaultObserverPosition = observer.transform.position;
        m_currentChunk = new Vector2(defaultObserverPosition.x, defaultObserverPosition.z);

        UpdateChunks();
    }

    public void OnPlayerMove(Vector3 newPosition)
    {
        float xChunkPosition = GetChunkIndexOfValue(newPosition.x);
        float zChunkPosition = GetChunkIndexOfValue(newPosition.z);

        m_currentChunk = new Vector2(xChunkPosition, zChunkPosition);
        if (m_currentChunk - m_prevChunk != Vector2.zero)
        {
            OnChunkChange();
        }
    }

    private void OnChunkChange()
    {
        m_prevChunk = m_currentChunk;
        Debug.Log($"Move to chunk {m_currentChunk.x} {m_currentChunk.y}");
        UpdateChunks();
    }

    private void UpdateChunks()
    {
        ClearChunks();
        if (CheckForExpansionRequired())
        {
            Expand();
        }
    }

    private bool CheckForExpansionRequired()
    {
        for (int i = -visionRange; i <= visionRange * 2; i++)
        {
            for (int j = -visionRange; j <= visionRange * 2; j++)
            {
                if (!m_loadedChunksList.Exists(el=>el.chunkIndex == new Vector2(m_currentChunk.x + i, m_currentChunk.y + j)))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void Expand()
    {
        for (int i = -loadRange; i <= loadRange; i++)
        {
            for (int j = -loadRange; j <= loadRange; j++)
            {
                Vector2 chunkIndex = new Vector2(m_currentChunk.x + i, m_currentChunk.y + j);
                if (!m_loadedChunksList.Exists(el => el.chunkIndex == chunkIndex))
                {
                    m_loadedChunksList.Add(terrainConstructor.ConstructTerrain(chunkIndex));
                }
            }
        }
    }

    private void ClearChunks()
    {
        if(m_loadedChunksList == null) { return; }

        for(int i=0; i<m_loadedChunksList.Count; i++)
        {
            ChunkController currentChunk = m_loadedChunksList[i];
            Vector2 chunkPosition = currentChunk.chunkIndex;
            if (Mathf.Max(Mathf.Abs(m_currentChunk.x - chunkPosition.x), Mathf.Abs(m_currentChunk.y - chunkPosition.y)) >= clearRange)
            {
                currentChunk.DestroyChunk();
                m_loadedChunksList.Remove(currentChunk);
            }
        }
    }

    private int GetChunkIndexOfValue(float val)
    {
        float relativePosition = (val + (m_chunkSize / 2)) / m_chunkSize;
        if (relativePosition > 0)
        {
            return Mathf.FloorToInt(relativePosition);
        }
        else
        {
            return Mathf.CeilToInt(relativePosition);
        }
    }
}

