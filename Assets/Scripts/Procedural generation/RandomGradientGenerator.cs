using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class both creates gradients and hashing them for optimization
public class RandomGradientGenerator
{
    int m_seed;
    public RandomGradientGenerator(int seed)
    {
        this.m_seed = seed;
    }

    public int Seed
    {
        get { return m_seed; }
        set { m_seed = value; }
    }

    public Vector2 GetGradientAtPoint(Vector2 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;

        //Hash function for coordinates and seed, created using "Slam numpad method"
        int hashedVectorForX = (((x + 1) << 5) * (m_seed >> 41)) + (x * m_seed ^ y) + (((y + 1) << 3) * (m_seed >> 7));
        int hashedVectorForY = (((x + 1) << 7599) * (m_seed >> 6983)) + (x * m_seed ^ y) + (((y + 1) << 4356) * (m_seed >> 425306));
        Random.InitState(hashedVectorForX);
        float xVal = Random.Range(-100,100) / (float)100;
        Random.InitState(hashedVectorForY);
        float yVal = Random.Range(-100, 100) / (float)100;
        return new Vector2(xVal, yVal);
    }
}
