using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace R4;

public static class TypeExtensions
{
	#region Inheritance

	public static bool HasInterface<T>(this Type instance)
		=>
		instance
			.GetInterfaces()
			.Any(x => typeof(T).IsAssignableFrom(x));

	public static bool InheritsFrom<T>(this Type instance)
		where T : class
		=> typeof(T).IsAssignableFrom(instance);

	public static bool InheritsFrom(this Type instance, Type baseType)
		=> baseType.IsAssignableFrom(instance);

	public static bool IsSystemType(this Type instance)
		=> instance.Namespace.StartsWithIgnoreCase("System");

	public static bool IsScalar(this Type instance)
	{
		if (instance == typeof(string))
			return true;

		if (instance == typeof(bool))
			return true;

		if (instance == typeof(bool?))
			return true;

		if (instance == typeof(int))
			return true;

		if (instance == typeof(int) || instance == typeof(int?))
			return true;

		if (instance == typeof(long) || instance == typeof(long?))
			return true;

		if (instance == typeof(float) || instance == typeof(float?))
			return true;

		if (instance == typeof(double) || instance == typeof(double?))
			return true;

		if (instance == typeof(DateTime) || instance == typeof(DateTime?))
			return true;

		return false;
	}

	#endregion

	#region Generic

	public static object ConstructGeneric(this Type instance, Type genericParameter)
	{
		var genericType = instance.MakeGenericType(new Type[] { genericParameter });
		var constructor = genericType.GetConstructor(new Type[] { });
		return constructor.Invoke(new object[] { });
	}

	public static MethodInfo GetGenericMethod(this Type instance, string methodName, Type genericParameter)
	{
		var method = instance.GetMethod(methodName);
		var result = method.MakeGenericMethod(new Type[] { genericParameter });
		return result;
	}

	#endregion

	#region Attributes

	public static bool HasAttribute<T>(this Type instance, bool inherit = true)
		where T : Attribute
	{
		var result = Attribute.IsDefined(instance, typeof(T), inherit);
		return result;
	}

	public static bool HasAttribute<T>(this PropertyInfo instance, bool inherit = true)
		where T : Attribute
	{
		var result = Attribute.IsDefined(instance, typeof(T), inherit);
		return result;
	}

	public static bool HasDefaultConstructor(this Type instance)
	{
		return instance.GetConstructor(Type.EmptyTypes) != null;
	}

	public static T GetAttribute<T>(this Type instance, bool inherit = true)
		where T : Attribute
	{
		var result = instance
			.GetCustomAttributes(typeof(T), inherit)
			.FirstOrDefault() as T;

		return result;
	}

	public static IEnumerable<PropertyWithAttributePair<TAttribute>> GetPropertiesWithAttribute<TAttribute>(this Type instance)
		 where TAttribute : Attribute
	{
		return instance
			.GetProperties()
			.Select(x =>
			{
				var attributes = x.GetCustomAttributes(typeof(TAttribute), true);
				if (attributes.Length == 0) return null;

				var attr = attributes[0] as TAttribute;
				var obj = new PropertyWithAttributePair<TAttribute>(x, attr);
				return obj;
			})
			.WhereNotNull()
			.ToList();
	}

	public class PropertyWithAttributePair<TAttribute>
		where TAttribute : Attribute
	{
		public PropertyWithAttributePair(PropertyInfo propertyInfo, TAttribute attribute)
		{
			PropertyInfo = propertyInfo;
			Attribute = attribute;
		}

		public PropertyInfo PropertyInfo { get; private set; }
		public TAttribute Attribute { get; private set; }
	}

	#endregion
}
