using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace R4;

public static class StringExtensions
{
	#region Compare

	public static bool HasContent(this string instance)
		=> !string.IsNullOrWhiteSpace(instance);

	#endregion

	#region Value

	public static string ContentOr(this string instance, params string[] firstWithContent)
	{
		if (instance.HasContent()) return instance;
		var result = firstWithContent.FirstWithContent();
		return result;
	}

	public static string ContentOr(this string instance, Func<string> func)
	{
		if (instance.HasContent()) return instance;
		var result = func();
		return result;
	}

	public static string ContentOrNull(this string instance)
	{
		if (instance.HasContent()) return instance;
		return null;
	}

	#endregion

	#region Html

	public static string HtmlEncode(this string instance)
	{
		if (!instance.HasContent())
			return instance;

		return HttpUtility.HtmlEncode(instance);
	}

	public static string HtmlDecode(this string instance)
	{
		if (!instance.HasContent())
			return instance;

		return HttpUtility.HtmlDecode(instance);
	}

	public static string UrlEncode(this string instance)
	{
		if (!instance.HasContent())
			return instance;

		return HttpUtility.UrlEncode(instance);
	}

	public static string UrlDencode(this string instance)
	{
		if (!instance.HasContent())
			return instance;

		return HttpUtility.UrlDecode(instance);
	}

	#endregion

	#region Convert

	public static string ToMd5(this string instance, bool phpCompatible = true)
	{
		if (!instance.HasContent())
			return instance;

		return CryptographyHelper.GetMd5Hash(instance, phpCompatible: phpCompatible);
	}

	public static string ToBase64String(this string instance, Encoding encoding = null)
	{
		if (encoding == null)
			encoding = Encoding.ASCII;

		return encoding.GetBytes(instance).ToBase64String();
	}

	#endregion

	#region Split

	public static List<string> SplitSafe(
		this string instance,
		char separator = ',',
		bool removeEmptyEntries = true
		)
		=> SplitSafe(instance, separator.ToString(), removeEmptyEntries);

	public static List<string> SplitSafe(
		this string instance,
		string separator,
		bool removeEmptyEntries = true
		)
		=> SplitSafe(instance, new[] { separator }, removeEmptyEntries);

	public static List<string> SplitSafe(
		this string instance,
		IEnumerable<string> separator,
		bool removeEmptyEntries = true
		)
	{
		if (!instance.HasContent()) return new List<string>();

		var options =
			removeEmptyEntries
				? StringSplitOptions.RemoveEmptyEntries
				: StringSplitOptions.None;

		return instance
			.Split(separator.ToArray(), options)
			.Select(x => x.Trim())
			.ToList();
	}

	public static List<string> SplitConfig(this string instance, string[] lineSeparators = null)
	{
		if (!instance.HasContent())
			return new List<string>();

		if (!lineSeparators.HasContent())
			lineSeparators = new[] { "\n" };

		var result =
			instance
				.SplitSafe(lineSeparators)
				.Select(x => x.RemoveLineComment())
				.ToList();

		return result;
	}

	public static StringDictionary ParseConfig(this string instance, string[] lineSeparators = null)
	{
		var result = new StringDictionary();

		if (!instance.HasContent())
			return result;

		var lines =
			instance
				.SplitConfig(lineSeparators);

		foreach (var line in lines)
		{
			var key = line;
			var value = null as string;

			var separatorIndex = line.IndexOf(":");
			if (separatorIndex < 0) separatorIndex = line.IndexOf("=");

			if (separatorIndex >= 0)
			{
				key = line.Substring(0, separatorIndex).Trim();
				value = line[(separatorIndex + 1)..].Trim();
				if (!value.HasContent()) value = null;
			}

			if (!key.HasContent()) continue;
			result.Add(key, value);
		}

		return result;
	}

	#endregion

	#region Formats checking

	public static char NumberDecimalSeparatorChar = Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator[0];
	public static string NumberDecimalSeparator = Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
	public static string NumberGroupSeparator = Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberGroupSeparator;

	public static bool IsAlphaNumeric(string str)
	{
		var result = Regex.IsMatch(str, "^[A-Za-z0-9ÄäÖöÜüß]+$", RegexOptions.IgnoreCase);
		return result;
	}

	public static bool IsInt32(this string instance)
	{
		if (!instance.HasContent()) return false;

		return int.TryParse(instance, out _);
	}

	public static bool IsNumeric(this string instance)
	{
		var hasDecimal = false;
		var foundDigit = false;

		instance = instance?.Replace(NumberGroupSeparator, "");
		if (!instance.HasContent()) return false;

		if (instance == "-") return false;
		if (instance[0] == '-') instance = instance[1..];

		for (var i = 0; i < instance.Length; i++)
		{
			// Check for decimal
			if (instance[i] == NumberDecimalSeparatorChar)
			{
				if (hasDecimal) // 2nd decimal
					return false;
				else // 1st decimal
				{
					if (!foundDigit)
						return false;

					// inform loop decimal found and continue 
					hasDecimal = true;
					continue;
				}
			}
			// check if number
			if (!char.IsNumber(instance[i])) return false;
			else foundDigit = true;
		}

		return true;
	}

	public static bool IsValidEMail(this string instance)
	{
		if (!instance.HasContent()) return false;
		return Regex.IsMatch(instance, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
	}

	#endregion

	#region Filtering

	public static string FormatSafe(this string instance, Func<string, string> factory)
	{
		if (!instance.HasContent())
			return instance;

		return factory(instance);
	}

	public static string LengthSafe(this string instance, int maxLength, string tail = "...")
	{
		if (
			!instance.HasContent()
			|| maxLength < 1
			)
			return "";

		if (instance.Length <= maxLength)
			return instance;

		var tailLength =
			tail.HasContent()
			? tail.Length
			: 0;

		var sublength = maxLength - tailLength;
		if (sublength <= 0)
			return instance.Substring(0, maxLength);

		return instance.Substring(0, sublength) + tail;
	}

	public static string RemoveLineComment(this string instance)
	{
		if (!instance.HasContent()) return instance;

		var index = instance.IndexOf("#");
		if (index < 0) index = instance.IndexOf("//");

		if (index < 0) return instance;

		var result = instance.Substring(0, index).Trim();
		return result;
	}

	public static string TrimSafe(this string instance)
	{
		if (instance == null) return "";
		var result = instance.Trim();
		return result;
	}

	public static string TrimSafe(this string instance, string token)
	{
		var result = instance
			.TrimSafeStart(token)
			.TrimSafeEnd(token);
		return result;
	}

	public static string TrimSafeStart(this string instance, string token)
	{
		if (!instance.HasContent()) return "";

		if (instance.StartsWithIgnoreCase(token))
			instance = instance[token.Length..];

		var result = instance.TrimStart();
		return result;
	}

	public static string TrimSafeEnd(this string instance, string token)
	{
		if (instance == null) return "";

		if (instance.EndsWithIgnoreCase(token))
			instance = instance.Substring(0, instance.Length - token.Length);

		var result = instance.TrimEnd();
		return result;
	}

	public static string StripHtmlTags(this string instance)
	{
		if (!instance.HasContent()) return instance;
		var result = HtmlHelper.StripTags(instance);
		return result;
	}

	public static string StripHtmlTagBlocks(this string instance, params string[] tags)
	{
		if (!instance.HasContent()) return instance;
		var result = HtmlHelper.WithoutTagBlocks(instance, tags);
		return result;
	}

	public static string StripHtmlComments(this string instance)
	{
		if (!instance.HasContent()) return instance;
		var result = HtmlHelper.WithoutComments(instance);
		return result;
	}

	public static string StripWordPastedXml(this string instance)
	{
		if (!instance.HasContent()) return instance;
		if (!instance.ContainsIgnoreCase("<w:WordDocument>")) return instance;

		var result = instance.StripHtmlTagBlocks("xml");
		result = result.StripHtmlComments();

		return result;
	}

	public static string FilterHtmlWhitespace(this string instance)
	{
		if (!instance.HasContent()) return instance;
		var result = HtmlHelper.FilterWhitespace(instance);
		return result;
	}

	public static string FilterAlpha(this string instance)
	{
		if (!instance.HasContent()) return instance;
		var result = "";

		for (int i = 0; i < instance.Length; i++)
		{
			bool pass = char.IsLetter(instance[i]);
			if (pass) result += instance[i];
		}

		return result;
	}

	public static string FilterNumeric(this string instance, bool allowDecimalSeparator = true)
	{
		if (!instance.HasContent()) return instance;

		if (instance.StartsWith("+") || instance.StartsWith("-"))
			instance = instance[1..];

		var result = "";

		for (int i = 0; i < instance.Length; i++)
		{
			bool pass = char.IsDigit(instance[i]) || (allowDecimalSeparator && instance[i] == NumberDecimalSeparatorChar);
			if (pass) result += instance[i];
		}

		return result;
	}

	public static string FilterAlphaNumeric(this string instance, IEnumerable<char> additional = null, string replace = null)
	{
		if (!instance.HasContent()) return instance;
		var result = "";

		for (int i = 0; i < instance.Length; i++)
		{
			var pass = char.IsLetterOrDigit(instance[i]) || (additional != null && additional.Contains(instance[i]));
			if (pass) result += instance[i];
			else result += replace;
		}

		return result;
	}

	public static string FilterLinearMultiple(this string instance, char single)
	{
		if (!instance.HasContent()) return instance;
		var result = "";

		var last = ' ';
		for (int i = 0; i < instance.Length; i++)
		{
			var c = instance[i];
			var pass = c != single || c != last;
			if (pass) result += c;
			last = c;
		}

		return result;
	}

	public static string FilterUrlAllowed(this string instance, bool keepUmlauts = false)
	{
		if (!instance.HasContent()) return instance;

		var result = "";
		for (int i = 0; i < instance.Length; i++)
		{
			var c = instance[i];
			if (c == ' ' || c == '/' || c == '.')
			{
				result += "-";
			}
			else if (char.IsLetterOrDigit(c) || c == '-')
			{
				if (!keepUmlauts) result += GetUmlautAsciiFormat(c);
				else result += c;
			}
		}
		result = result.Replace("---", "-");
		result = result.Replace("--", "-");
		return result;
	}

	public static string GetUmlautAsciiFormat(this char instance)
	{
		var result = instance.ToString();

		if (instance == 'Ä') result = "Ae";
		else if (instance == 'ä') result = "ae";
		else if (instance == 'Ö') result = "Oe";
		else if (instance == 'ö') result = "oe";
		else if (instance == 'Ü') result = "Ue";
		else if (instance == 'ü') result = "ue";
		else if (instance == 'ß') result = "ss";

		return result;
	}

	public static int FilterAutoIdFromTrashFormat(this string instance)
	{
		if (!instance.HasContent()) return 0;
		if (instance.Length >= 6 && !instance.Contains("-")) return 0;

		var tokens = instance.SplitSafe("-");
		if (!tokens.HasContent()) return 0;

		var result = tokens.ElementAt(0).ToInt32();
		return result;
	}

	public static int ToInt32IfIsNumeric(this string instance)
	{
		if (!instance.IsNumeric()) return 0;
		var result = instance.ToInt32();
		return result;
	}

	public static string ReplaceSafe(this string instance, string oldValue, string newValue)
	{
		if (!instance.HasContent()) return instance;
		var result = instance.Replace(oldValue, newValue);
		return result;
	}

	public static string ReplaceAt(this string instance, int startIndex, int length, string newString)
	{
		var result = instance.Remove(startIndex, length).Insert(startIndex, newString);
		return result;
	}

	#endregion

	#region Comparision & Finding

	public static bool EqualsSafe(this string instance, string compareTo)
	{
		if (instance == null) return compareTo == null;
		return instance.Equals(compareTo);
	}

	public static bool EqualsIgnoreCase(this string instance, string compareTo)
	{
		if (instance == null) return compareTo == null;
		return instance.Equals(compareTo, StringComparison.OrdinalIgnoreCase);
	}

	public static bool Equals(this string instance, string compareTo, StringComparison stringComparison)
	{
		if (instance == null) return compareTo == null;
		return String.Compare(instance, compareTo, stringComparison) == 0;
	}

	public static bool InIgnoreCase(this string instance, params string[] compareTo)
		=> compareTo.Any(x => x.EqualsIgnoreCase(instance));

	public static bool ContainsIgnoreCase(this string instance, string compareTo)
		=> instance.IndexOfWithIgnoreCase(compareTo) >= 0;

	public static bool ContainsAnyIgnoreCase(this string instance, IEnumerable<string> anyOf)
		=> anyOf.Any(x => instance.IndexOfWithIgnoreCase(x) >= 0);

	public static bool Contains(this string instance, string compareTo, StringComparison stringComparison)
		=> instance.IndexOf(compareTo, stringComparison) >= 0;

	public static bool StartsWithIgnoreCase(this string instance, string compareTo)
	{
		if (instance == null) return false;
		return instance.StartsWith(compareTo, StringComparison.OrdinalIgnoreCase);
	}

	public static bool EndsWithIgnoreCase(this string instance, string compareTo)
	{
		if (instance == null) return false;
		return instance.EndsWith(compareTo, StringComparison.OrdinalIgnoreCase);
	}

	public static int IndexOfWithIgnoreCase(this string instance, string compareTo, int startIndex = 0)
	{
		if (!instance.HasContent()) return -1;
		return instance.IndexOf(compareTo ?? "", startIndex, StringComparison.OrdinalIgnoreCase);
	}

	public static int NthIndexOf(this string instance, int nth, char compareTo)
	{
		if (!instance.HasContent()) return -1;

		var result = -1;
		var count = 0;
		for (int i = 0; i < instance.Length; i++)
		{
			if (instance[i] == compareTo) count++;
			if (count == nth)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public static int NthIndexOfWithIgnoreCase(this string instance, int nth, string compareTo)
	{
		if (!instance.HasContent()) return -1;

		var index = -1;
		var count = 0;

		while (true)
		{
			index = instance.IndexOfWithIgnoreCase(compareTo ?? "", index + 1);

			if (index < 0)
				return -1;

			if (++count == nth)
				return index;
		}
	}

	public static int Count(this string instance, char toCount)
	{
		var result = 0;
		for (int i = 0; i < instance.Length; i++)
		{
			if (instance[i] == toCount) result++;
		}
		return result;
	}

	#endregion

	#region Substrings

	public static string SubstringSafe(this string instance, int startIndex, int length)
	{
		if (!instance.HasContent()) return instance;
		if (startIndex + length > instance.Length) return instance[startIndex..];
		return instance.Substring(startIndex, length);
	}

	public static string LeftSafe(this string instance, int length)
	{
		if (!instance.HasContent())
			return instance;

		if (length == 0)
			return instance;

		if (length >= instance.Length)
			return instance;

		if (length < 0)
		{
			if (-length > instance.Length)
				return "";

			return instance.Substring(0, instance.Length + length);
		}

		return instance.Substring(0, length);
	}

	public static string LeftSafe(this string instance, string untilIndexOf)
	{
		if (!instance.HasContent()) return null;
		var index = instance.IndexOf(untilIndexOf);
		if (index < 0) return instance;
		return instance.Substring(0, index);
	}

	public static string RightSafe(this string instance, int length)
	{
		if (!instance.HasContent())
			return instance;

		if (length == 0)
			return instance;

		if (length >= instance.Length)
			return instance;

		if (length < 0)
		{
			if (-length > instance.Length)
				return "";

			return instance[-length..];
		}

		return instance[^length..];
	}

	public static string LastToken(this string instance, char separator)
	{

		var token = instance.SplitSafe(separator);
		if (!token.HasContent()) return null;

		var result = token.Last();
		return result;
	}

	public static IEnumerable<string> Slice(this string instance, string separator)
	{
		var token = instance.SplitSafe(separator);
		if (!token.HasContent() || token.Count() != 2)
			return null;

		var result = token;
		return result;
	}

	public static List<string> SplitOnLength(this string instance, int maxLength)
	{
		if (!instance.HasContent()) return new List<string>();
		var totalLength = instance.Length;

		if (totalLength <= maxLength)
			return new List<string> { instance };

		var tokenCount = Math.Ceiling(totalLength / (double)maxLength);
		var result = new List<string>();
		for (int i = 0; i < tokenCount; i++)
		{
			var startIndex = i * maxLength;

			var token =
				i < tokenCount - 1
				? instance.Substring(startIndex, maxLength)
				: instance[startIndex..];

			result.Add(token);
		}
		return result;
	}

	public static string ReplaceAll(this string instance, IEnumerable<string> strings, string newValue)
	{
		if (!instance.HasContent()) return instance;
		var result = instance;

		foreach (string s in strings)
			result = result.Replace(s, newValue);

		return result;
	}

	#endregion

	#region Path

	private static string CombinePath(string path1, string path2, char separator, char alternativeSeparator)
	{
		if (!path1.HasContent())
			path1 = "";

		if (!path2.HasContent())
			path2 = "";

		path1 = path1.Replace(alternativeSeparator, separator);
		path2 = path2.Replace(alternativeSeparator, separator);

		if (path1.HasContent())
			if (!path1.EndsWith(separator))
				path1 += separator;

		if (path2.StartsWith(separator))
			path2 = path2[1..];

		return path1 + path2;
	}

	public static string CombineLocalPath(this string instance, string path)
		=> CombinePath(instance, path, '\\', '/');

	public static string CombineWebPath(this string instance, string path)
		=> CombinePath(instance, path, '/', '\\');

	public static string RemoveQueryParameter(this string instance, string parameterName)
	{
		while (true)
		{
			var parameterStart =
				instance.IndexOfWithIgnoreCase(parameterName);

			if (parameterStart < 0)
				break;

			var left =
				instance.Substring(0, parameterStart);

			var newUrl = left;

			var parameterEnd =
				instance.IndexOf("&", parameterStart);

			if (parameterEnd >= 0)
				newUrl += instance[(parameterEnd + 1)..];

			instance = newUrl;
		}

		if (instance.EndsWith("?"))
			return instance[0..^1];

		return instance;
	}

	#endregion
}
