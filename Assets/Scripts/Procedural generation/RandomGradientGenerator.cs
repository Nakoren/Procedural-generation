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
        int hashedVector = (((x + 1) * 781) % 7852 * (m_seed * 698) % 8213) + ((x + 1) * m_seed ^ y) + (((y + 1) * 356) % 6842 * (m_seed * 4256) % 4258);
        Random.InitState(hashedVector);
        float angle = Random.Range(0,360);
        float xVal = Mathf.Sin(angle);
        float yVal = Mathf.Cos(angle);
        return new Vector2(xVal, yVal);
    }
}
