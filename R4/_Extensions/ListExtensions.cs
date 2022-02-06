using System;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace R4;

public static class ListExtensions
{
	#region Adding

	public static void AddIfNotExists<T>(this ICollection<T> instance, T item)
	{
		if (instance == null) throw new Exception($"{nameof(AddIfNotExists)} fails, instance is NULL.");
		if (!instance.Contains(item)) instance.Add(item);
	}

	public static void AddIfNotExists<T>(this ICollection<T> instance, IEnumerable<T> items)
	{
		if (!items.HasContent()) return;

		foreach (var item in items)
			instance.AddIfNotExists(item);
	}

	public static void AddSafe<T>(this ICollection<T> instance, IEnumerable<T> items)
	{
		if (!items.HasContent()) return;

		foreach (var item in items)
		{
			if (item == null) continue;
			instance.Add(item);
		}
	}

	public static void AddSafe<T>(this ICollection<T> instance, T item)
	{
		if (item == null) return;
		instance.Add(item);
	}

	public static void AddSafe(this ICollection<string> instance, string item)
	{
		if (!item.HasContent())
			return;

		instance.Add(item);
	}

	public static void AddSafe(this ICollection<string> instance, IEnumerable<string> items)
	{
		if (!items.HasContent()) return;

		foreach (var item in items)
			AddSafe(instance, item);
	}

	#endregion

	#region Move

	public static void Move<T>(this IList<T> instance, Func<T, bool> predicate, int index)
	{
		var item = instance.FirstOrDefault(predicate);
		Move(instance, item, index);
	}

	public static void Move<T>(this IList<T> instance, T item, int index)
	{
		if (item == null) return;

		instance.Remove(item);
		instance.Insert(index, item);
	}

	#endregion

	#region Removing

	public static bool RemoveIfExists<T>(this List<T> instance, T item)
	{
		var result = instance.Contains(item);
		if (result) instance.Remove(item);
		return result;
	}

	#endregion

	#region Sorting / Randomizing

	public static void FisherYatesShuffle<T>(this IList<T> items)
	{
		var itemIndex = items.Count;

		while (itemIndex > 1)
		{
			//Generate a new random number using RNGCryptoServiceProvider as Random() produces less-than-random results
			var box = new byte[1];
			do
			{
				RandomNumberGenerator.Fill(box);
			}
			while (!(box[0] < itemIndex * (byte.MaxValue / itemIndex)));

			var randomIndex = (box[0] % itemIndex);
			itemIndex--;

			//Swap the indexed and random positions
			T value = items[randomIndex];
			items[randomIndex] = items[itemIndex];
			items[itemIndex] = value;
		}
	}

	#endregion

	#region Concurrent Helpers

	public static List<T> CopyWithout<T>(this List<T> instance, Func<T, bool> predicate)
		=> new(instance.Where(x => !predicate(x)));

	public static List<T> CopyWith<T>(this List<T> instance, IEnumerable<T> list)
		=> instance.Union(list).ToList();

	public static List<T> CopyWithout<T>(this List<T> instance, IEnumerable<T> list)
	{
		var result = new List<T>(instance);
		foreach (var item in list)
			result.RemoveIfExists(item);
		return result;
	}

	public static List<T> CopyEnqueue<T>(this List<T> instance, T obj, int maxSize)
	{
		List<T> result;
		if (instance.Count < maxSize)
		{
			result = new List<T>(instance);
		}
		else
		{
			result = new List<T>(instance.Take(maxSize - 1));
		}
		result.Insert(0, obj);
		return result;
	}

	#endregion

	#region Select

	public static List<T> GetRadius<T>(this List<T> instance, T center, int radius)
	{
		var centerIndex =
			instance.IndexOf(center);

		if (centerIndex < 0)
			throw new IndexOutOfRangeException();

		var length =
			(radius * 2) + 1;

		var leftIndex =
			centerIndex - radius;

		if (leftIndex <= 0)
			return
				instance
					.Take(length)
					.ToList();

		var rightIndex =
			centerIndex + radius;

		if (rightIndex > instance.Count - 1)
			return
				instance
					.Skip(instance.Count - length)
					.Take(length)
					.ToList();

		return
			instance
				.Skip(leftIndex)
				.Take(length)
				.ToList();
	}

	#endregion
}
