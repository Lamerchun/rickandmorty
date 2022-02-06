using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R4;

public static class IEnumerableExtensions
{
	#region Content

	public static int CountSafe<T>(this IEnumerable<T> instance)
	{
		if (instance == null)
			return 0;

		if (instance is IList<T> listT)
			return listT.Count;

		if (instance is IList list)
			return list.Count;

		return instance.Count();
	}

	public static bool HasContent<T>(this IEnumerable<T> instance)
		=> instance.CountSafe() > 0;

	public static IEnumerable<T> Safe<T>(this IEnumerable<T> instance)
	{
		if (instance == null)
			return Enumerable.Empty<T>();

		return instance;
	}

	#endregion

	#region Selecting

	public static bool ContainsIgnoreCase(this IEnumerable<string> instance, string content)
	{
		if (!instance.HasContent()) return false;
		return instance.Any(x => x.EqualsIgnoreCase(content));
	}

	public static string FirstWithContent(this IEnumerable<string> instance)
	{
		if (!instance.HasContent()) return null;
		return instance.FirstOrDefault(x => x.HasContent());
	}

	public static float? FirstWithContent(this IEnumerable<float?> instance)
	{
		if (!instance.HasContent()) return null;
		return instance.FirstOrDefault(x => x.GetValueOrDefault() > 0);
	}

	public static float FirstWithValue(this IEnumerable<float> instance)
	{
		if (!instance.HasContent()) return 0f;
		return instance.FirstOrDefault(x => x > 0);
	}

	public static T PickRandom<T>(this IEnumerable<T> instance)
	{
		if (!instance.HasContent()) return default;
		var random = new Random();
		var index = random.Next(0, instance.Count());
		return instance.ElementAt(index);
	}

	public static int IndexOf<T>(this IEnumerable<T> instance, Func<T, bool> predicate)
	{
		var result = 0;
		var found = false;

		foreach (var item in instance)
		{
			if (predicate(item))
			{
				found = true;
				break;
			};
			result++;
		}

		return found ? result : -1;
	}

	public static List<T> Copy<T>(this IEnumerable<T> instance, Func<T, bool> predicate = null)
	{
		if (predicate == null)
			return new List<T>(instance);

		return
			new List<T>(instance.Where(predicate));
	}

	public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		if (source == null) yield break;

		var seenKeys = new HashSet<TKey>();
		foreach (TSource element in source)
		{
			if (seenKeys.Add(keySelector(element)))
				yield return element;
		}
	}

	public static HashSet<string> ToHashSet(this IEnumerable<string> instance)
	{
		if (instance == null) return null;
		return new HashSet<string>(instance);
	}

	public static IEnumerable<T> ToCarousel<T>(this IEnumerable<T> instance, T startWith)
	{
		var result = new List<T>();
		var allImages = instance.ToList();
		var startIndex = allImages.IndexOf(startWith);

		for (var i = 0; i < allImages.Count; i++)
			result.Add(allImages[(i + startIndex) % allImages.Count]);

		return result;
	}

	public static async Task<IEnumerable<T>> WhenAll<T>(this IEnumerable<Task<T>> instance)
		=> await Task.WhenAll(instance);

	public static Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(
		this IEnumerable<TSource> instance,
		Func<TSource, Task<TResult>> selector)
	{
		return instance.Select(selector).WhenAll();
	}

	#endregion

	#region Filtering

	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> instance)
	{
		if (instance == null) return instance;
		var result = instance.Where(x => x != null);
		return result;
	}

	public static IEnumerable<string> WhereHasContent(this IEnumerable<string> instance)
	{
		if (instance == null) return instance;
		var result = instance.Where(x => x.HasContent());
		return result;
	}

	#endregion

	#region Join

	public static string JoinSafe(this IEnumerable<string> instance, string separator = ", ", bool removeNoContents = true)
	{
		if (!instance.HasContent()) return null;
		if (removeNoContents) instance = instance.WhereHasContent();
		return string.Join(separator, instance);
	}

	public static string JoinSafe(this IEnumerable<int> instance, string separator = ", ")
	{
		if (!instance.HasContent()) return null;
		return string.Join(separator, instance);
	}

	#endregion

	#region Handler

	public static async Task HandleAsync<T>(this IEnumerable<Func<T, Task<bool>>> instance, T parameter)
	{
		foreach (var handler in instance)
		{
			if (await handler(parameter))
				return;
		}

		throw new NoHandlerFoundException();
	}

	public class NoHandlerFoundException : Exception { }

	#endregion
}
