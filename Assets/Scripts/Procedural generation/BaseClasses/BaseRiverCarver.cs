using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseRiverCarver : TerrainLayer
{
    [SerializeField] protected float minRiverHeight;

    [SerializeField] protected Biom riverBiom;

    protected float m_maxRiverHeight;

    public override int Seed
    {
        get { return m_seed; }
        set
        {
            m_seed = value;
        }
    }

    public float MaxRiverHeight { 
        set { 
            if(value >= minRiverHeight) m_maxRiverHeight = value; 
            else m_maxRiverHeight = minRiverHeight;
        } 
        get { return m_maxRiverHeight; }
    }
}
