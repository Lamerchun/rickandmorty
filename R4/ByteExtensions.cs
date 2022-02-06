using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace R4;

public static class ByteExtensions
{
	#region Conversions

	public static MemoryStream ToMemoryStream(this byte[] instance)
		=> new(instance);

	public static bool IsPng(this byte[] instance)
	{
		if (!instance.HasContent()) return false;
		var sub = instance.Take(4).ToArray();
		var header = System.Text.Encoding.ASCII.GetString(sub);
		var result = header.EndsWithIgnoreCase("png");
		return result;
	}

	public static string ToASCIIString(this byte[] instance)
	{
		System.Text.ASCIIEncoding enc = new();
		return enc.GetString(instance);
	}

	public static byte[] Base64ToByteArray(this string instance)
	{
		if (!instance.HasContent())
			return null;

		return Convert.FromBase64String(FixBase64ForImage(instance));
	}

	private static string FixBase64ForImage(string str)
		=>
		str
			.Replace("\r\n", string.Empty)
			.Replace(" ", string.Empty);

	public static string ToBase64String(this byte[] instance, Base64FormattingOptions formatOption = Base64FormattingOptions.None)
		=> Convert.ToBase64String(instance, formatOption);

	#endregion

	#region Checksum

	public static string ToMd5(this byte[] instance, bool phpCompatible = true)
		=> CryptographyHelper.GetMd5Hash(instance, phpCompatible: phpCompatible);

	#endregion
}
