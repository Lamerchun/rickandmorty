using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace R4;

public interface IAssemblyService : ISingletonService
{
	Type GetTypeInCodeAssemblies(string fullName);
	IEnumerable<Type> GetTypesInCodeAssemblies(Func<Type, bool> filter);
}

public class AssemblyService : IAssemblyService
{
	private static readonly string[] AssemblyNamePrefixes
		= new[] {
				"R4",
				"App",
				"us"
		};

	public static IEnumerable<Assembly> GetCodeAssemblies()
	{
		var allAssemblies =
			AppDomain.CurrentDomain.GetAssemblies();

		var result = new List<Assembly>();
		var addedNames = new HashSet<string>();

		foreach (var prefix in AssemblyNamePrefixes)
		{
			foreach (var assembly in allAssemblies)
			{
				if (addedNames.ContainsIgnoreCase(assembly.FullName))
					continue;

				if (
					!assembly.FullName.StartsWithIgnoreCase(prefix + ",")
					&& !assembly.FullName.StartsWithIgnoreCase(prefix + ".")
					)
					continue;

				result.Add(assembly);
				addedNames.Add(assembly.FullName);
			}
		}

		return result;
	}

	public Type GetTypeInCodeAssemblies(string fullName)
	{
		foreach (var assembly in GetCodeAssemblies())
		{
			var type =
				assembly
					.GetTypes()
					.FirstOrDefault(x => x.FullName == fullName);

			if (type == null)
				type =
				assembly
					.GetTypes()
					.FirstOrDefault(x => x.FullName.EndsWithIgnoreCase($".{fullName}"));

			if (type != null)
				return type;
		}

		return default;
	}

	public IEnumerable<Type> GetTypesInCodeAssemblies(Func<Type, bool> filter)
	{
		var result = new List<Type>();

		foreach (var assembly in GetCodeAssemblies())
		{
			var types =
				assembly
					.GetTypes()
					.Where(filter);

			result.AddSafe(types);
		}

		return result;
	}
}
