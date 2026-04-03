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

    public float[,] GetNoiseInArea(Vector2 centerPosition, int range)
    {
        float[,] noise = new float[range*2+1, range*2+1];
        for(int i = -range; i <= range; i++)
        {
            for(int j= -range; j <= range; j++)
            {
                Vector2 point = new Vector2(centerPosition.x + i, centerPosition.y + j);
                noise[i + range, j + range] = GetValueAtPoint(point);
            }
        }
        return noise;
    }

    public float GetValueAtPoint(Vector2 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;
        int hashVector = (((x + 1) * 717) % 7443 * (m_seed + 1 * 478) % 3415) + ((x + 1) * m_seed + 1 ^ y)%3478 + (((y + 1) * 1328) % 861 * (m_seed + 1 * 451) % 441);
        Random.InitState(hashVector);
        float value = Random.Range(0, 100)/(float)100;
        return value;
    }

}
