using System;
using UnityEngine;

public abstract class TimeLimitedContainer<KeyType, DataType>
{
    public Action onDataClear;

    public abstract DataType this[KeyType key]
    {
        get;
        set;
    }
}
