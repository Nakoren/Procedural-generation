using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class both creates gradients and hashing them for optimization
public class RandomGradientGenerator
{
    private int m_seed;
    private TimeLimitedMap<Vector2, Vector2> m_gradientCashMap;

    public RandomGradientGenerator(int seed)
    {
        this.m_seed = seed;
        m_gradientCashMap = new TimeLimitedMap<Vector2, Vector2>();
    }

    public int Seed
    {
        get { return m_seed; }
        set { m_seed = value; }
    }

    public Vector2 GetGradientAtPoint(Vector2 position)
    {
        
        Vector2 cashValue = m_gradientCashMap.Get(position);
        if (cashValue != default(Vector2)) {
            return cashValue;
        }
        else
        {
            int x = (int)position.x;
            int y = (int)position.y;

            int hashedVectorForX = (((x + 1) * 781) % 7852 * (m_seed * 698) % 8213) + ((x + 1) * m_seed ^ y) + (((y + 1) * 356) % 6842 * (m_seed * 4256) % 4258);
            int hashedVectorForY = (((x + 1) * 378) % 4185 * (m_seed * 178) % 7264) + ((x + 1) * m_seed ^ y) + (((y + 1) * 731) % 7521 * (m_seed * 7823) % 9421);
            Random.InitState(hashedVectorForX);
            float xVal = (float)Random.Range(-100, 100) / 100;
            Random.InitState(hashedVectorForY);
            float yVal = (float)Random.Range(-100, 100) / 100;
            Vector2 res = new Vector2(xVal, yVal);

            m_gradientCashMap.Set(position, res);

            return res;
        }
    }

    public void ResetCash()
    {
        m_gradientCashMap.Reset();
    }
}
