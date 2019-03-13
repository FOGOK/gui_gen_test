using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace UnitySerializing
{
    public class ComponentSerializer<T> : JsonConverter 
        where T : Component
    {
        private string[] _allowedProps;
        
        private readonly Dictionary<string, Action<T, object>> _customInitializers;
        private readonly Dictionary<string, Func<T, object>> _customValGetters;

        public ComponentSerializer(string[] allowedProps, Dictionary<string, Func<T, object>> customValGetters, Dictionary<string, Action<T, object>> customInitializers)
        {
            _allowedProps = allowedProps;
            _customInitializers = customInitializers;
            _customValGetters = customValGetters;
        }
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            T component = (T)value;
            
            writer.WriteStartObject();
            writer.WriteType(component.GetType());
            component.SerializeProps(_allowedProps, writer, serializer, _customValGetters);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var currentSelectedGo = SerializerUtils.CurrentSelectedRectTransform.gameObject;
            if (currentSelectedGo == null)
            {
                Debug.LogError($"currentSelectedGo == null in {typeof(T)} type serializer");
                return null;
            }
            
            var component = currentSelectedGo.AddComponent<T>();
            component.SetAllSimilarProps(reader, objectType, serializer, _customInitializers);
            
            //component has added to go, return
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }
    }
}