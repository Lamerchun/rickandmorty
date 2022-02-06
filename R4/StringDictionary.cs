using System;
using System.Collections.Generic;

namespace R4;

public class StringDictionary : Dictionary<string, string>
{
	public StringDictionary()
		: base(StringComparer.OrdinalIgnoreCase) { }

	public StringDictionary(IEnumerable<KeyValuePair<string, string>> pairs)
		: base(StringComparer.OrdinalIgnoreCase)
	{
		foreach (var p in pairs)
			Add(p.Key, p.Value);
	}

	public StringDictionary(IDictionary<string, string> dictionary)
		: base(dictionary, StringComparer.OrdinalIgnoreCase)
	{
	}

	public StringDictionary(IEnumerable<string> values)
		: base(StringComparer.OrdinalIgnoreCase)
	{
		foreach (var value in values.Safe())
			Add(value, value);
	}

	public static implicit operator StringDictionary(KeyValuePair<string, string>[] source)
		=> new(source);
}
