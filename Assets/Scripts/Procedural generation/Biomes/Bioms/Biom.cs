using UnityEngine;

[CreateAssetMenu(fileName = "Biom", menuName = "Scriptable Objects/Biom")]
public class Biom : ScriptableObject
{
    [Header("Set humidity and temperature from -100 to 100")]
    public float bottomHumidity;
    public float topHumidity;

    public float bottomTemperature;
    public float topTemperature;

    public int biomHeight;
    public TerrainLayer terrainLayer;

    public bool CheckBiom(float humidity, float temperature)
    {
        return ((humidity > bottomHumidity) && (humidity <= topHumidity)) && ((temperature > bottomTemperature) && (temperature <= topTemperature));
    }
}
