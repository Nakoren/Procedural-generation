using UnityEngine;

public abstract class TerrainLayer : ScriptableObject
{

    protected int m_seed;
    bool m_initialized = false;

    protected abstract void Init();
    protected abstract void CalculateLayer(ChunkData chunkData);

    private void OnEnable()
    {
        //SO does like to set private bool variables to True, so I had to reset it here
        m_initialized = false;
    }

    public virtual void ApplyLayer(ChunkData chunkData)
    {
        if (!m_initialized)
        {
            Init();
            m_initialized = true;
        }
        CalculateLayer(chunkData);
    }
    public virtual int Seed { get { return m_seed; } set { m_seed = value; } }
}
