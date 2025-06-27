using UnityEngine;

public abstract class BaseVegetationGenerator : MonoBehaviour
{
    [SerializeField] protected int seed;

    public virtual int Seed
    {
        get { return seed; }
        set
        {
            seed = value;
        }
    }

    public abstract void ApplyVegetation(Terrain terrain, Vector2 offset, int size, Biom[,] biomMap);

}
