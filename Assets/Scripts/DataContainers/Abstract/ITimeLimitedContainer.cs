using System;
using UnityEngine;

public abstract class TimeLimitedContainer<KeyType, DataType>
{
	public Action onDataClear;

	public abstract void Set(KeyType key, DataType data);

	public abstract DataType? Get(KeyType key);

	public abstract bool Contains(KeyType key);

}
