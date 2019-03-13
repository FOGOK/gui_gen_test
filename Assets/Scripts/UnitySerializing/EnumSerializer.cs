using System;
using Newtonsoft.Json;
using UnityEngine;

namespace UnitySerializing
{
    public class EnumSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                return;
            }

            Enum e = (Enum)value;
            writer.WriteStartObject();
            writer.WriteType(value.GetType());
            writer.WritePropertyName("enumVal");
            writer.WriteValue((int)Enum.Parse(e.GetType(), e.ToString()));
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {            
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        var i = reader.ReadAsInt32();
                        return Enum.Parse(objectType, i.ToString(), true);
                       
                }
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            Type t = (IsNullableType(objectType))
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            return t.IsEnum;
        }
        
        
        public static bool IsNullableType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}