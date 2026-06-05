using UnityEngine;

public abstract class BaseMountainLayer : TerrainLayer
{
	[SerializeField] protected float minMountainsHeight;
	[SerializeField] protected float maxMountainsHeight;

	[SerializeField] protected Biom MountainBiom;
}
