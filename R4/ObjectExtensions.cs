using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace R4;

public static class ObjectExtensions
{
	#region Generic Invoke

	public static object InvokeGeneric(this object instance, string methodName, Type genericParameter, params object[] parameters)
	{
		var type =
			instance.GetType();

		var method =
			type
				.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.Name == methodName && x.IsGenericMethod)
				.First();

		var genericMethod =
			method.MakeGenericMethod(new Type[] { genericParameter });

		if (!parameters.HasContent())
			parameters = new object[] { };

		return genericMethod.Invoke(instance, parameters);
	}

	public static Task<object> InvokeByMethodNameAsync(this object instance, string methodName, params object[] parameters)
	{
		var type = instance.GetType();
		var method = type.GetMethod(methodName);
		return InvokeByMethodAsync(instance, method, parameters);
	}

	public static Task<object> InvokeByMethodNameAsync(this object instance, string methodName, Type[] methodSignatureParameters, params object[] parameters)
	{
		var type = instance.GetType();
		var method = type.GetMethod(methodName, methodSignatureParameters);
		return InvokeByMethodAsync(instance, method, parameters);
	}

	public static async Task<object> InvokeByMethodAsync(this object instance, MethodInfo method, params object[] parameters)
	{
		if (!parameters.HasContent())
			parameters = new object[] { };

		var invokeResult = method.Invoke(instance, parameters);

		if (invokeResult == null)
			return null;

		var invokeResultType = invokeResult.GetType();
		if (!invokeResultType.InheritsFrom<Task>())
			return invokeResult;

		var task = invokeResult as Task;
		await task;

		return
			invokeResultType
				.GetProperty(nameof(Task<object>.Result))
				.GetValue(invokeResult);
	}

	public static object InvokeByMethodName(this object instance, string methodName, params object[] parameters)
	{
		var type = instance.GetType();
		var method = type.GetMethod(methodName);

		if (!parameters.HasContent())
			parameters = new object[] { };

		return
			method.Invoke(instance, parameters);
	}

	#endregion

	#region Value

	public static bool IsNullOrDefault<T>(this T argument)
	{
		if (argument == null)
			return true;

		if (Equals(argument, default(T)))
			return true;

		var methodType = typeof(T);

		if (Nullable.GetUnderlyingType(methodType) != null)
			return false;

		var argumentType =
			argument.GetType();

		if (!argumentType.IsValueType || argumentType == methodType)
			return false;

		object obj = Activator.CreateInstance(argument.GetType());
		return obj.Equals(argument);
	}

	#endregion

	#region Properies

	public static void InstantiateDefaultContructors(this object instance)
	{
		var type = instance.GetType();

		if (type.IsScalar())
			return;

		if (type.IsSystemType())
			return;

		var properties =
			type.GetProperties();

		foreach (var property in properties)
		{
			if (property.PropertyType.IsScalar())
				continue;

			var value =
				property.GetValue(instance);

			if (value != null)
			{
				if (value is System.Collections.IEnumerable)
					continue;

				InstantiateDefaultContructors(value);
				continue;
			}

			var defaultContructor =
				property.PropertyType.GetConstructor(new Type[] { });

			if (defaultContructor == null)
				continue;

			var createdValue =
				Activator.CreateInstance(property.PropertyType);

			InstantiateDefaultContructors(createdValue);
			property.SetValue(instance, createdValue);
		}
	}

	#endregion

	#region JSON

	private static readonly JsonSerializerSettings _CamelCaseSettings
		= new() { ContractResolver = new CamelCasePropertyNamesContractResolver() };

	private static readonly JsonSerializerSettings _SnakeCaseSettings
		= new() { ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() } };

	private static readonly JsonSerializerSettings _TypeInformationSettings
		= new() { TypeNameHandling = TypeNameHandling.All };

	public static string ToJson(this object instance, bool useSnakeCase = false)
	{
		if (useSnakeCase)
			return JsonConvert.SerializeObject(instance, Formatting.None, _SnakeCaseSettings);

		return JsonConvert.SerializeObject(instance, Formatting.None, _CamelCaseSettings);
	}

	public static string ToJsonMd5(this object instance)
		=> instance?.ToJson().ToMd5();

	public static string ToJsonWithTypeInformation(this object instance)
		=> JsonConvert.SerializeObject(instance, Formatting.None, _TypeInformationSettings);

	public static string ToStringOrEmpty(this object instance)
		=> instance != null ? instance.ToString() : "";

	#endregion

	#region Conversion

	public static double ToDouble(this object instance, double onFail = 0d)
	{
		if (instance == null)
			return onFail;

		if (instance is double _double)
			return _double;

		if (double.TryParse(instance.ToString(), out double result))
			return result;

		return onFail;
	}

	public static float ToFloat(this object instance, float onFail = 0f)
	{
		if (instance == null)
			return onFail;

		if (instance is float _float)
			return _float;

		if (float.TryParse(instance.ToString(), out float result))
			return result;

		return onFail;
	}

	public static int ToInt32(this object instance, int onFail = 0)
	{
		if (instance == null)
			return onFail;

		if (instance is int _int)
			return _int;

		if (instance is float _float)
			return (int)Math.Round(_float);

		if (instance is double _double)
			return (int)Math.Round(_double);

		if (int.TryParse(instance.ToString(), out int result))
			return result;

		return onFail;
	}

	public static long ToLong(this object instance, long onFail = 0L)
	{
		if (instance == null)
			return onFail;

		if (instance is int)
			return (long)instance;

		if (instance is float f)
			return (long)Math.Round(f);

		if (instance is double d)
			return (long)Math.Round(d);

		if (long.TryParse(instance.ToString(), out long result))
			return result;

		return onFail;
	}

	public static bool ToBoolean(this object instance, bool onFail = false)
	{
		if (instance == null)
			return onFail;

		if (instance is bool _bool)
			return _bool;

		var str = instance.ToString();
		if (str.EqualsIgnoreCase("true") || str == "1")
			return true;

		if (str.EqualsIgnoreCase("false") || str == "0")
			return false;

		return onFail;
	}

	#endregion

	#region Traverse

	public static IEnumerable<TCallbackType> GetCustomObjects<TCallbackType>(this object instance)
		where TCallbackType : class
	{
		var result = new List<TCallbackType>();

		TraverseProperties<TCallbackType>(
			instance: instance,
			callback: (i, p, x) => result.Add(x)
			);

		return result;
	}

	public static void TraverseCustomObjects<TObject, TCallbackType>(
		this TObject instance,
		Action<TCallbackType> callback
		)
		where TCallbackType : class
	{
		TraverseProperties<TCallbackType>(
			instance,
			callback: (i, p, x) => callback(x)
		);
	}

	public static void TraverseProperties<T>(
		this object instance,
		Action<object, PropertyInfo, T> callback
		)
		where T : class
	{
		if (instance == null)
			return;

		var type = instance.GetType();
		var inspectType = typeof(T);

		if (type == inspectType)
			callback(instance, null, instance as T);

		if (type.Name.StartsWith(nameof(KeyValuePair)))
		{
			var value =
				type.GetProperty("Value")
				.GetValue(instance);

			TraverseProperties(value, callback);
			return;
		}

		var properties =
			type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

		if (type.Namespace.StartsWithIgnoreCase("System"))
			return;

		foreach (var property in properties)
		{
			var value =
				property.GetValue(instance);

			if (value == null)
				continue;

			if (!property.PropertyType.IsScalar())
			{
				if (property.PropertyType.InheritsFrom<IEnumerable<T>>())
				{
					foreach (var item in value as System.Collections.IEnumerable)
						callback(instance, property, item as T);
				}
				else if (property.PropertyType.InheritsFrom<System.Collections.IEnumerable>())
				{
					foreach (var item in value as System.Collections.IEnumerable)
						TraverseProperties(item, callback);
				}
			}

			if (property.PropertyType.InheritsFrom(inspectType))
			{
				callback(instance, property, value as T);
				continue;
			}

			if (property.PropertyType.IsScalar())
				continue;

			TraverseProperties(value, callback);
		}
	}

	#endregion
}

