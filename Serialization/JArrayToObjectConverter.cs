namespace Ecng.Serialization
{
	using System;
	using System.Reflection;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public class JArrayToObjectConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType) => true;

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			existingValue ??= Activator.CreateInstance(objectType);
			
			var array = JArray.Load(reader);
			var fields = objectType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			
			for (var i = 0; i < fields.Length; i++)
			{
				var field = fields[i];
				var token = array[i];
				field.SetValue(existingValue, token.ToObject(field.FieldType));
			}

			return existingValue;
		}

		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			=> throw new NotSupportedException();
	}
}