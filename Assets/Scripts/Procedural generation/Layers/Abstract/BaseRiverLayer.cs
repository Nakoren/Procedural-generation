using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseRiverLayer : TerrainLayer
{
	[SerializeField] protected float minRiverHeight;

	[SerializeField] protected float maxRiverHeight;

	[SerializeField] protected Biom riverBiom;

	public override int Seed
	{
		get { return m_seed; }
		set
		{
			m_seed = value;
		}
	}
}
