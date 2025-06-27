using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseRiverCarver : MonoBehaviour
{
    [SerializeField] protected float minRiverHeight;

    [SerializeField] protected Biom riverBiom;

    [SerializeField] protected int seed;

    protected float m_maxRiverHeight;

    public virtual int Seed
    {
        get { return seed; }
        set
        {
            seed = value;
        }
    }

    public float MaxRiverHeight { 
        set { 
            if(value >= minRiverHeight) m_maxRiverHeight = value; 
            else m_maxRiverHeight = minRiverHeight;
        } 
        get { return m_maxRiverHeight; }
    }


    public abstract void CarveRivers(TerrainData terrainData, Vector2 offset, int size, Biom[,] biomMap, out Biom[,] updateBiomMap);
}
