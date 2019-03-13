using System;
using Newtonsoft.Json;
using UnityEngine;

namespace UnitySerializing
{
    public class RectSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Rect rect = (Rect) value;

            writer.WriteStartObject();
            writer.WriteType(typeof(Rect));
            writer.WritePropertyName("x");
            writer.WriteValue(rect.x);
            writer.WritePropertyName("y");
            writer.WriteValue(rect.y);
            writer.WritePropertyName("w");
            writer.WriteValue(rect.width);
            writer.WritePropertyName("h");
            writer.WriteValue(rect.height);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            reader.Read();
            float x = (float) reader.ReadAsDouble();
            reader.Read();
            float y = (float) reader.ReadAsDouble();
            reader.Read();
            float width = (float) reader.ReadAsDouble();
            reader.Read();
            float height = (float) reader.ReadAsDouble();
            return new Rect(x, y, width, height);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Rect);
        }
    }
}