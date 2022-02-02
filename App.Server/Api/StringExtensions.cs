using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace us
{
	public static class JObjectExtensions
	{
		private static readonly JsonSerializerSettings _CamelCaseSettings
			= new() { ContractResolver = new CamelCasePropertyNamesContractResolver() };


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
				ObjectCreationHandling = ObjectCreationHandling.Replace,
				NullValueHandling = NullValueHandling.Include,
				TypeNameHandling = TypeNameHandling.Auto,
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

		public static string ToJson(this object instance)
			=> JsonConvert.SerializeObject(instance, Formatting.None, _CamelCaseSettings);

		public static T ToObjectByJson<T>(this string instance)
		{
			if (string.IsNullOrWhiteSpace(instance) || instance == "[]")
				return default;

			return JsonConvert.DeserializeObject<T>(instance, SerializerSettings);
		}
	}
}
