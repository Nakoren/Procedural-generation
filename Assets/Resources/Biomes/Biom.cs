using System.Runtime.InteropServices.WindowsRuntime;
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

    static public Biom[,] ConvertTerrainDataArray(BiomData[,] data)
    {
        Biom[,] commonBiomMap = new Biom[data.GetLength(0), data.GetLength(0)];
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(0); j++)
            {
                commonBiomMap[i, j] = data[i, j].biom;
            }
        }
        return commonBiomMap;
    }

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
        return Mathf.Pow(t,1/(float)1.1);
    }

    private float GetAffilation(float value, float range, float center)
    {
        float radius = range / 2;
        return (radius - Mathf.Abs(center - value)) / radius;
    }
}
