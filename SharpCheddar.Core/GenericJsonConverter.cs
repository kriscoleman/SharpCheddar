using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SharpCheddar.Core
{
    public class GenericJsonConverter<Tin, Tout> : JsonConverter
        where Tout : Tin, new()
    {
        public override bool CanConvert(Type objectType)
            =>
                objectType == typeof(Tin)
                || typeof(IEnumerable<Tin>).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

        // this doesn't support inheritance fully, perhaps we should switch this to .IsAssignableTo or something
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (typeof(IEnumerable<Tin>).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo())
                && objectType.GetTypeInfo().IsGenericType)
            {
                return GetIEnumerableResults(reader, serializer);
            }

            var target = serializer.Deserialize<JObject>(reader);
            var result = new Tout();
            serializer.Populate(target.CreateReader(), result);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        private static object GetIEnumerableResults(JsonReader reader, JsonSerializer serializer)
        {
            var results = new List<Tin>();

            var target = serializer.Deserialize<JArray>(reader);
            foreach (var obj in target)
            {
                var @out = new Tout();
                serializer.Populate(obj.CreateReader(), @out);
                results.Add(@out);
            }

            return results;
        }
    }
}