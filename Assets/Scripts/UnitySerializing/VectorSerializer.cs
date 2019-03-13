using System;
using Newtonsoft.Json;
using UnityEngine;

namespace UnitySerializing
{
    public class VectorSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Vector2)
            {
                Vector2 vec = (Vector2) value;

                writer.WriteStartObject();
                writer.WriteType(typeof(Vector2));
                writer.WritePropertyName("x");
                writer.WriteValue(vec.x);
                writer.WritePropertyName("y");
                writer.WriteValue(vec.y);
                writer.WriteEndObject();
            }
            else
            {
                Vector3 vec = (Vector3) value;

                writer.WriteStartObject();
                writer.WriteType(typeof(Vector3));
                writer.WritePropertyName("x");
                writer.WriteValue(vec.x);
                writer.WritePropertyName("y");
                writer.WriteValue(vec.y);
                writer.WritePropertyName("z");
                writer.WriteValue(vec.z);
                writer.WriteEndObject();
            }
            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(Vector2))
            {
                reader.Read();
                float x = (float) reader.ReadAsDouble();
                reader.Read();
                float y = (float) reader.ReadAsDouble();
                return new Vector2(x, y);
            }

            else
            {
                reader.Read();
                float x = (float) reader.ReadAsDouble();
                reader.Read();
                float y = (float) reader.ReadAsDouble();
                reader.Read();
                float z = (float) reader.ReadAsDouble();
                return new Vector3(x, y, z);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2) || objectType == typeof(Vector3);
        }
    }
}