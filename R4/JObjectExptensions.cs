using System;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace R4;

public static class JObjectExtensions
{
	#region Serializer Settings

	public class DoubleSerializer : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var doubleValue = value.ToDouble().ToString("#.#").Replace(".", ",");
			serializer.Serialize(writer, doubleValue);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (objectType == typeof(double) || objectType == typeof(double?))
			{
				object result = null;

				var jsonObject = JValue.Load(reader);
				var stringValue = jsonObject.ToString();
				if (stringValue.HasContent())
				{
					if (!stringValue.IsNumeric())
					{
						var isNullable = objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>);
						if (isNullable) return null;
					}
					else
					{
						result = stringValue.ToDouble();
					}
				}

				return result;
			}
			else
			{
				reader.Skip();
			}
			return null;
		}

		public override bool CanConvert(Type objectType)
		{
			var result = objectType == typeof(double) || objectType == typeof(double?);
			return result;
		}
	}

	public class IntSerializer : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var intValue = value.ToInt32();
			serializer.Serialize(writer, intValue);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (objectType == typeof(int) || objectType == typeof(int?))
			{
				object result = null;

				var jsonObject = JValue.Load(reader);
				var stringValue = jsonObject.ToString();
				if (stringValue.HasContent())
				{
					if (!stringValue.IsNumeric())
					{
						var isNullable = objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>);
						if (isNullable) return null;
					}
					else
					{
						result = stringValue.ToInt32();
					}
				}

				return result;
			}
			else
			{
				reader.Skip();
			}
			return null;
		}

		public override bool CanConvert(Type objectType)
		{
			var result = objectType == typeof(int) || objectType == typeof(int?); ;
			return result;
		}
	}

	private static JsonSerializerSettings _SerializerSettings = null;
	private static JsonSerializerSettings SerializerSettings
	{
		get
		{
			if (_SerializerSettings != null)
				return _SerializerSettings;

			_SerializerSettings = CreateDefaultSerializerSettings();

			return _SerializerSettings;
		}
	}

	private static JsonSerializerSettings CreateDefaultSerializerSettings()
		=>
		new()
		{
			ReferenceLoopHandling = ReferenceLoopHandling.Error,
			DateTimeZoneHandling = DateTimeZoneHandling.Local,
				//Converters = new List<JsonConverter>() { new DoubleSerializer(), new IntSerializer() },
				ObjectCreationHandling = ObjectCreationHandling.Replace,
			NullValueHandling = NullValueHandling.Include,
			TypeNameHandling = TypeNameHandling.Auto,
			MissingMemberHandling = MissingMemberHandling.Ignore
		};

	private static JsonSerializerSettings _SerializerSettingsSnake = null;
	private static JsonSerializerSettings SerializerSettingsSnake
	{
		get
		{
			if (_SerializerSettingsSnake != null)
				return _SerializerSettingsSnake;

			_SerializerSettingsSnake = CreateDefaultSerializerSettings();

			_SerializerSettingsSnake.ContractResolver = new DefaultContractResolver
			{
				NamingStrategy = new SnakeCaseNamingStrategy { ProcessDictionaryKeys = true }
			};

			return _SerializerSettingsSnake;
		}
	}

	#endregion

	#region Create, Equals, Copy

	public static T CopyByJson<T>(this T instance)
	{
		if (instance == null) return default;

		var result = default(T);
		var sourceJson = instance.ToJsonWithTypeInformation();

		var type = typeof(T);
		if (type.IsAbstract || type.IsInterface)
		{
			var method = typeof(JObjectExtensions).GetMethod(nameof(ToObjectByJson), new[] { typeof(string), typeof(bool) });
			var instanceType = instance.GetType();
			var genericMethod = method.MakeGenericMethod(new[] { instanceType });
			result = (T)genericMethod.Invoke(null, new object[] { sourceJson, false });
		}
		else
		{
			result = sourceJson.ToObjectByJson<T>();
		}

		return result;
	}

	public static T ToObjectByJson<T>(this string instance, bool fromSnakeCase = false)
	{
		if (!instance.HasContent() || instance == "[]")
			return default;

		var settings =
			fromSnakeCase
				? SerializerSettingsSnake
				: SerializerSettings;

		return JsonConvert.DeserializeObject<T>(instance, settings);
	}

	public static bool EqualsByJson(this object instance, object compareToObject)
	{
		var instanceJson = instance.ToJson();
		var compareJson = compareToObject.ToJson();
		return instanceJson == compareJson;
	}

	public static object ToUntypedObjectByJson(this string instance)
	{
		if (!instance.HasContent() || instance == "[]") return null;
		var settings = SerializerSettings;
		return JsonConvert.DeserializeObject(instance, settings);
	}

	#endregion

	#region Diff

	public static JObject GetJsonDiff(this object source, object target)
	{
		var result = new JObject();

		if (source == null) source = new object();
		if (target == null) target = new object();

		var sourceObject = JObject.FromObject(source);
		var targetObject = JObject.FromObject(target);

		CrawlChanges(result, sourceObject, targetObject);

		return result;
	}

	public static JObject GetJsonUndoAtFirstLevel(this object source, object target)
	{
		var diff = GetJsonDiff(source, target);
		var result = new JObject();

		if (!diff.HasValues)
			return result;

		var sourceObject =
			source == null
				? null
				: JObject.FromObject(source);

		foreach (var property in diff.Children<JProperty>())
			result[property.Name] = sourceObject?.GetValue(property.Name);

		return result;
	}

	private static void CrawlChanges(this JToken container, JToken sourceToken, JToken targetToken)
	{
		if (container == null)
			return;

		var keys = new[] { sourceToken, targetToken }
			.WhereNotNull()
			.SelectMany(
				x =>
					x?.Children<JProperty>()
						.Safe()
						.Select(y => y?.Name)
						.WhereNotNull()
						.Distinct()
			)
			.ToList();

		foreach (var propertyName in keys.Safe())
		{
			var property = null as JProperty;

			if (sourceToken?[propertyName] is JToken token)
				property = token.Parent as JProperty;

			else property = new JProperty(propertyName, null);

			var targetProperty = targetToken?[propertyName].Parent as JProperty;

			var hasChanged = !JToken.DeepEquals(property, targetProperty);
			if (!hasChanged) continue;

			if (property.Value is JValue)
			{
				container[propertyName] = targetToken?[propertyName];
			}
			else
			{
				var obj = new JObject();
				container[propertyName] = obj;
				CrawlChanges(obj, property.Value, targetProperty.Value);
			}
		}
	}

	#endregion

	#region Populate, Apply

	public static void ApplyJson(this object instance, JObject values)
	{
		var json = values.ToJson();
		ApplyJson(instance, json);
	}

	public static void ApplyJson(this object instance, string json)
	{
		var settings = SerializerSettings;
		JsonConvert.PopulateObject(json, instance, settings);
	}

	public static void ApplyJsonValues<T>(this T instance, T source)
		=> instance.ApplyJson(source.ToJson());

	public static void Set(this JObject instance, string path, object value)
	{
		var obj = instance;

		var token = path.SplitSafe(".");
		for (int i = 0; i < token.Count - 1; i++)
		{
			var field = token[i];
			if (obj[field] == null) obj[field] = new JObject();
			obj = obj[field] as JObject;
		}

		var lastField = token.Last();
		obj[lastField] = JValue.FromObject(value);
	}

	#endregion
}
