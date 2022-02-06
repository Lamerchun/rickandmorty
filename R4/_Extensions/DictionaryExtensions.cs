using System.Collections.Generic;

namespace R4;

public static class DictionaryExtensions
{
	public static TValue GetSafe<TKey, TValue>(this IDictionary<TKey, TValue> instance, TKey key)
	{
		if (key == null)
			return default;

		if (!instance.ContainsKey(key))
			return default;

		return instance[key];
	}

	public static bool HasContent<TKey, TValue>(this IDictionary<TKey, TValue> instance)
	{
		if (instance?.Count > 0)
			return true;

		return false;
	}
}
