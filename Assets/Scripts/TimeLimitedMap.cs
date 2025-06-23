using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

//This class represents a map with ability of self- and controlled clearing
public class TimeLimitedMap<K,T>
{
    private int m_timeLimit;
    private List<T> m_objectList = new List<T>();
    private List<K> m_keyList = new List<K>();
    private List<int> m_elementTimers = new List<int>();

    public bool deleteOnTimer;

    public TimeLimitedMap()
    {
        m_timeLimit = 0;
        deleteOnTimer = false;
    }
    public TimeLimitedMap(int timeLimit, bool deleteOnTimer) { 
        m_timeLimit = timeLimit;
        this.deleteOnTimer = deleteOnTimer;
    }
    private void UpdateTimer()
    {
        if (!deleteOnTimer) return;
        for(int i = 0; i < m_elementTimers.Count; i++)
        {
            m_elementTimers[i]--;
            if (m_elementTimers[i] <= 0)
            {
                m_objectList.RemoveAt(i);
                m_keyList.RemoveAt(i);
                m_elementTimers.RemoveAt(i);
            }
        }
    }

    public T Get(K key)
    {
        T resValue = default(T);
        for (int i = 0; i < m_keyList.Count; i++)
        {
            if (m_keyList[i].Equals(key))
            {
                resValue = m_objectList[i];
            }
        }
        UpdateTimer();
        return resValue;
    }

    public void Set(K key, T value)
    {
        UpdateTimer();
        for (int i = 0;i < m_keyList.Count; i++)
        {
            if (m_keyList[i].Equals(key)) {
                m_objectList[i] = value;
                m_elementTimers[i] = m_timeLimit;
                return;
            }
        }
        m_keyList.Add(key);
        m_objectList.Add(value);
        m_elementTimers.Add(m_timeLimit);
    }

    public void Remove(K key)
    {
        for(int i = 0; i < m_keyList.Count; i++)
        {
            if (m_keyList[i].Equals(key))
            {
                m_keyList.RemoveAt(i);
                m_objectList.RemoveAt(i);
                break;
            }
        }
        UpdateTimer();
    }

    public void Reset()
    {
        m_keyList = new List<K>();
        m_objectList = new List<T>();
    }

}
