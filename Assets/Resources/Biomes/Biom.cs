using NUnit.Framework;
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

    [Header("This parameter defines chance of vegetation spawning at random point (0 - 100):")]
    public int vegetationSpawnChance;
    [Header("This parameter means range at which 2 vegetations can inflict each other")]
    public int vegetationIsolationRange;

    public GameObject[] vegetationObjects;

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
        return Mathf.Pow(t,(float)1.5);
    }

    private float GetAffilation(float value, float range, float center)
    {
        float radius = range / 2;
        return (radius - Mathf.Abs(center - value)) / radius;
    }

    public bool CheckPlantSpawn(int chance)
    {
        return chance < vegetationSpawnChance;
    }

    public GameObject GetRandomPlantAtPoint(Vector2 position, int seed)
    {
        if(vegetationObjects.Length == 0) return null;
        int x = (int)position.x;
        int y = (int)position.y;
        int hashedVector = ((x + 1) * seed * 781) % 771 * (((x + 1) * seed * 923) % 1431) * ((x + 1) * seed ^ y);
        Random.InitState(hashedVector);
        return vegetationObjects[Random.Range(0,vegetationObjects.Length - 1)];
    }
}
