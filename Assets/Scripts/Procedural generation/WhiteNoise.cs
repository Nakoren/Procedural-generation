using UnityEngine;

public class WhiteNoise
{
    int m_seed;
    
    public WhiteNoise(int seed)
    {
        m_seed = seed;
    }

    public int Seed
    {
        get { return m_seed; }
        set { m_seed = value; }
    }

    public float[,] GetNoiseInArea(Vector2 centerPosition, int size)
    {
        float[,] noise = new float[size*2+1, size*2+1];
        for(int i = -size; i <= size; i++)
        {
            for(int j= -size; j <= size; j++)
            {
                Vector2 point = new Vector2(centerPosition.x + size, centerPosition.y + size);
                noise[i + size, j + size] = GetValueAtPoint(point);
            }
        }
        return noise;
    }

    public float GetValueAtPoint(Vector2 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;
        int hashVector = (((x + 1) * 717) % 7443 * (m_seed + 1 * 478) % 3415) + ((x + 1) * m_seed + 1 ^ y) + (((y + 1) * 1328) % 861 * (m_seed + 1 * 451) % 441);
        Random.InitState(hashVector);
        float value = Random.Range(0, 100)/(float)100;
        return value;
    }

}
