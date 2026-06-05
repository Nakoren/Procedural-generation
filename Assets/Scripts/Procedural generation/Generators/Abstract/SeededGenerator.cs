using UnityEngine;

public abstract class SeededGenerator 
{
	protected int m_seed = 0;

	public int Seed
	{
		get => m_seed;
		set
		{
			m_seed = value;
		}
	}

	public abstract float GetValueAtPoint(Vector2 position);
}
