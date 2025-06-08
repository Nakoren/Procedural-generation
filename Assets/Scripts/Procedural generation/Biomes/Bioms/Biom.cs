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

    public bool CheckBiom(int humidity, int temperature)
    {
        return ((humidity > bottomHumidity) && (humidity < topHumidity)) && ((temperature > bottomTemperature) && (temperature < topTemperature));
    }
}
