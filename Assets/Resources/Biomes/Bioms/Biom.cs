using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Biom", menuName = "Scriptable Objects/Biom")]
public class Biom : ScriptableObject
{
    [Header("Set humidity and temperature from -100 to 100")]
    public float bottomHumidity;
    public float topHumidity;

    public float bottomTemperature;
    public float topTemperature;

    public float biomLowFrequencyAmplitude;
    public float biomMiddleFrequencyAmplitude;
    public float biomHighFrequencyAmplitude;

    public TerrainLayer terrainLayer;

    public bool CheckBiom(float humidity, float temperature)
    {
        return ((humidity >= bottomHumidity) && (humidity <= topHumidity)) && ((temperature >= bottomTemperature) && (temperature <= topTemperature));
    }
    public float GetBiomAffilation(BiomData biomData)
    {
        float humidityRange = topHumidity - bottomHumidity;
        float temperatureRange = topTemperature - bottomTemperature;

        float humidityCenter = bottomHumidity + humidityRange / 2;
        float temperatureCenter = bottomTemperature + temperatureRange / 2;

        float humidityAffilation = GetAffilation(biomData.humidity, humidityRange, humidityCenter);
        float temperatureAffilation = GetAffilation(biomData.temperature, temperatureRange, temperatureCenter);

        float t = Mathf.Min(humidityAffilation, temperatureAffilation);
        //return t;
        return 6 * Mathf.Pow(t, 5) - 15 * Mathf.Pow(t, 4) + 10 * Mathf.Pow(t, 3);
    }

    private float GetAffilation(float value, float range, float center)
    {
        float radius = range / 2;
        return (radius - Mathf.Abs(center - value)) / radius;
    }
}
