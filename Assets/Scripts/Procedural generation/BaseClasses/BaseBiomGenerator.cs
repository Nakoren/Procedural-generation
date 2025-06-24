using UnityEngine;

public abstract class BaseBiomGenerator : MonoBehaviour
{
    [SerializeField] protected Biom[] biomsList;
    [SerializeField] protected Biom placeHolderBiom;
    [SerializeField] protected int biomGenerationSeed;

    public virtual int Seed
    {
        get { return biomGenerationSeed; }
        set
        {
            biomGenerationSeed = value;
        }
    }

    public abstract void ApplyBiom(TerrainData terrainData, Vector2 offset, int size, out BiomData[,] resBiomData);
}
