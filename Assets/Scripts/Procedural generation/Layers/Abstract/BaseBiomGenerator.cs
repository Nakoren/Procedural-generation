using UnityEngine;

public abstract class BaseBiomGenerator : TerrainLayer
{
	[SerializeField] protected Biom[] biomsList;
	[SerializeField] protected Biom placeHolderBiom;
	[SerializeField] protected int biomGenerationSeed;

	public override int Seed
	{
		get { return biomGenerationSeed; }
		set
		{
			biomGenerationSeed = value;
		}
	}
}
