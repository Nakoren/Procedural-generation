using UnityEngine;

public class BiomData
{
	public Biom biom;
	public int humidity;
	public int temperature;

	public BiomData(Biom biom, int humidity, int temperature)
	{
		this.biom = biom;
		this.humidity = humidity;
		this.temperature = temperature;
	}
}