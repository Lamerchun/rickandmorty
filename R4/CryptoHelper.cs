using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace R4;

public static class CryptographyHelper
{
	#region SHA512

	public static string HashWithSha512(string content)
	{
		using var provider = SHA512.Create();
		var hash = provider.ComputeHash(Encoding.UTF8.GetBytes(content));
		var result = BitConverter.ToString(hash);
		return result.Replace("-", string.Empty).ToLower();
	}

	public static bool ValidateWithSha512(string hash, string content)
	{
		var validationHash = HashWithSha512(content);
		return hash.EqualsIgnoreCase(validationHash);
	}

	#endregion

	#region SHA1

	public static string HashWithSha1(string content)
	{
		using var provider = SHA1.Create();
		var hash = provider.ComputeHash(Encoding.Default.GetBytes(content));
		var result = BitConverter.ToString(hash);
		return result.Replace("-", string.Empty).ToLower();
	}

	public static bool ValidateWithSha1(string hash, string content)
	{
		var validationHash = HashWithSha1(content);
		return hash.EqualsIgnoreCase(validationHash);
	}

	#endregion

	#region MD5

	public static string GetMd5Hash(string input, bool phpCompatible = false)
		=> GetMd5Hash(Encoding.Default.GetBytes(input), phpCompatible);

	public static string GetMd5Hash(byte[] input, bool phpCompatible = false)
	{
		if (!input.HasContent())
			return null;

		using var md5 = MD5.Create();
		var hashedInput = md5.ComputeHash(input);
		return CreateMd5Hash(hashedInput, phpCompatible);
	}

	public static string GetMd5Hash(Stream input, bool phpCompatible = false)
	{
		if (input == null)
			return null;

		using var md5 = MD5.Create();
		var hashedInput = md5.ComputeHash(input);
		return CreateMd5Hash(hashedInput, phpCompatible);
	}

	private static string CreateMd5Hash(byte[] hashedInput, bool phpCompatible = false)
	{
		if (!hashedInput.HasContent())
			return null;

		if (!phpCompatible)
			return BitConverter.ToString(hashedInput);

		var sb = new StringBuilder();
		for (int i = 0; i < hashedInput.Length; i++)
			sb.AppendFormat("{0:x2}", hashedInput[i]);

		return sb.ToString();
	}

	#endregion
}
