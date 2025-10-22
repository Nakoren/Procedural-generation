using UnityEngine;

public abstract class TerrainLayer : ScriptableObject
{
    protected int m_seed;
    public abstract void ApplyLayer(ChunkData chunk);

    public virtual int Seed { get { return m_seed; } set { m_seed = value; } }
}
