using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseRiverCarver : MonoBehaviour
{
    [SerializeField] protected float minRiverHeight;
    [SerializeField] protected float maxRiverHeight;

    [SerializeField] protected Biom riverBiom;

    [SerializeField] protected int seed;

    public virtual int Seed
    {
        get { return seed; }
        set
        {
            seed = value;
        }
    }

    public abstract void CarveRivers(TerrainData terrainData, Vector2 offset, int size, Biom[,] biomMap, out Biom[,] updateBiomMap);
}
