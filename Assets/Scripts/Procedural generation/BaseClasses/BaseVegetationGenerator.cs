using UnityEngine;

public abstract class BaseVegetationGenerator : TerrainLayer
{

    public virtual int Seed
    {
        get { return m_seed; }
        set
        {
            m_seed = value;
        }
    }

}
