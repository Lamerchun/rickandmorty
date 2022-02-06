using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace R4;

public static class HtmlHelper
{
	#region Filter

	private static Regex _WhitespaceRegex;

	private static readonly string[] _Expressions = new string[] {
			@"\>\s*\<(?:(?!\b(?:a|span|div|input|label|select)\b))",
			@"\>\s+\f*",
			@"\f*\s+\<"
		};

	private static readonly string[] _Replacements = new string[] {
			"><",
			"> ",
			" <"
		};

	private static void InitExpressions()
	{
		if (_WhitespaceRegex == null)
		{
			var reStr = String.Join("|", _Expressions.Select(x => "(" + x + ")"));
			_WhitespaceRegex = new Regex(reStr, RegexOptions.Compiled);
		}
	}

	public static string FilterWhitespace(string content)
	{
		if (!content.HasContent()) return content;
		InitExpressions();

		content = _WhitespaceRegex.Replace(content, match =>
		{
			for (var i = 1; i <= match.Groups.Count; ++i)
			{
				if (match.Groups[i].Success)
					return _Replacements[i - 1];
			}

			return match.Value;
		});

		return content;
	}

	public static string StripTags(string html)
	{
		var array = new char[html.Length];
		var arrayIndex = 0;
		var inside = false;

		for (int i = 0; i < html.Length; i++)
		{
			var let = html[i];
			if (let == '<')
			{
				inside = true;
				continue;
			}
			if (let == '>')
			{
				inside = false;
				continue;
			}
			if (!inside)
			{
				array[arrayIndex] = let;
				arrayIndex++;
			}
		}
		var result = new string(array, 0, arrayIndex);
		return result;
	}

	#endregion

	#region Tag Blocks

	public static string WithoutTagBlocks(string html, params string[] tags)
	{
		if (!tags.HasContent()) return html;

		var result = html;

		foreach (var tag in tags)
		{
			var localIndex = 0;

			while (true)
			{
				var openTag = "<" + tag;
				var start = result.IndexOfWithIgnoreCase(openTag, localIndex);
				if (start > 0) localIndex = start;
				if (start < 0) break;

				var closeTag = "</" + tag + ">";
				var end = result.IndexOfWithIgnoreCase(closeTag, start);

				if (end < 0)
				{
					closeTag = "/>";
					end = result.IndexOf(closeTag, start);
				}
				else
				{
					while (true)
					{
						var partLength = end - start - openTag.Length;
						var part = result.Substring(localIndex + openTag.Length, partLength);
						var containsNestedTag = part.ContainsIgnoreCase(openTag);
						if (!containsNestedTag) break;

						var newEnd = result.IndexOfWithIgnoreCase(closeTag, end + closeTag.Length);
						if (newEnd > 0) end = newEnd;
						else break;
					}
				}

				if (end > 0) localIndex = end;
				else localIndex += openTag.Length;

				if (end < 0) continue;

				end += closeTag.Length;
				var length = end - start;
				result = result.ReplaceAt(start, length, "");

				localIndex = start;
			}

			int index = localIndex;
		}

		return result;
	}

	public static string WithoutComments(string html)
	{
		var result = html;

		var index = 0;

		while (true)
		{
			var openTag = "<!--";
			var start = result.IndexOfWithIgnoreCase(openTag, index);
			if (start > 0) index = start;
			if (start < 0) break;

			var closeTag = "-->";
			var end = result.IndexOfWithIgnoreCase(closeTag, start);

			if (end < 0) break;

			if (end > 0) index = end;
			else index += openTag.Length;

			end += closeTag.Length;
			var length = end - start;
			result = result.ReplaceAt(start, length, "");

			index = start;
		}

		return result;
	}

	#endregion

	#region Strip Html

	public static string GetHtmlWithoutScriptTagAndAttributes(string html)
	{
		var doc =
			new HtmlAgilityPack.HtmlDocument();

		doc.LoadHtml(html);

		doc.
			DocumentNode
				.Descendants()
				.Where(n => n.Name.EqualsIgnoreCase("script"))
				.ToList()
				.ForEach(n => n.Remove());

		foreach (var node in doc.DocumentNode.SelectNodes("//*").Safe())
		{
			node.Attributes
				.Where(x => x.Name.StartsWithIgnoreCase("on"))
				.ToList()
				.ForEach(n => n.Remove());
		}

		using var writer = new StringWriter();
		doc.Save(writer);
		return writer.ToString();
	}

	#endregion
}
