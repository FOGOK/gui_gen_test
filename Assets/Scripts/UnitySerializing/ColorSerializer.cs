using System;
using Newtonsoft.Json;
using UnityEngine;

namespace UnitySerializing
{
    public class ColorSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Color col = (Color) value;

            writer.WriteStartObject();
            writer.WriteType(typeof(Color));
            writer.WritePropertyName("r");
            writer.WriteValue(col.r);
            writer.WritePropertyName("g");
            writer.WriteValue(col.g);
            writer.WritePropertyName("b");
            writer.WriteValue(col.b);
            writer.WritePropertyName("a");
            writer.WriteValue(col.a);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            reader.Read();
            float r = (float) reader.ReadAsDouble();
            reader.Read();
            float g = (float) reader.ReadAsDouble();
            reader.Read();
            float b = (float) reader.ReadAsDouble();
            reader.Read();
            float a = (float) reader.ReadAsDouble();
            return new Color(r, g, b, a);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }
    }
}