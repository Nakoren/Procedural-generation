using UnityEngine;

public abstract class BaseMountainLayer : TerrainLayer
{
	[SerializeField] protected float min_mountains_height;
	[SerializeField] protected float max_mountains_height;

	[SerializeField] protected Biom MountainBiom;
}
