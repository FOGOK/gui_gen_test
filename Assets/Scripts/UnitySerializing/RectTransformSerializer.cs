using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace UnitySerializing
{
    public class RectTransformSerializer : JsonConverter
    {
        private static readonly string[] RectTransformAllowedProps =
        {
            "name", "anchorMin", "anchorMax"/*, "rotation"*/, "pivot", "position", "localScale", "sizeDelta",
        };
        
        private readonly Dictionary<string, Action<RectTransform, object>> _customInitializers;

        public RectTransformSerializer()
        {
            _customInitializers = new Dictionary<string, Action<RectTransform, object>>
            {
//                ["offsetMin"] = (transform, o) =>
//                {
//                    var offsetMinRaw = (Vector2) o;
//                    var offsetMin = new Vector2(Mathf.Abs(offsetMinRaw.x), Mathf.Abs(offsetMinRaw.y));
//                    
//                },
//                ["offsetMax"] = (transform, o) =>
//                {
//                    
//                }
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            RectTransform rectTransform = (RectTransform) value;           
            
            writer.WriteStartObject();
            
            writer.WriteType(typeof(RectTransform));
            
            rectTransform.SerializeProps(RectTransformAllowedProps, writer, serializer);

            {
                writer.WritePropertyName("components");
                writer.WriteStartArray();
                SerializeComponents(writer, rectTransform, serializer);
                writer.WriteEndArray();
            }
            

            {
                writer.WritePropertyName("childs");
                writer.WriteStartArray();
                for (int i = 0; i < rectTransform.childCount; i++)
                {
                    var child = rectTransform.GetChild(i);
                    if (child != null)
                        child.Serialize(writer, serializer);                        
                }
                writer.WriteEndArray();
            }
            
            
            writer.WriteEndObject();
        }

        private void SerializeComponents(JsonWriter writer, RectTransform rectTransform, JsonSerializer serializer)
        {            
            var components = rectTransform.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                component.Serialize(writer, serializer);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {            
            var rectTransform = new GameObject().AddComponent<RectTransform>();
            
            if (SerializerUtils.CurrentSelectedRectTransform != null)
                rectTransform.transform.SetParent(SerializerUtils.CurrentSelectedRectTransform);
            
            //pass first type
            if (reader.Depth == 0)
            {
                reader.Read();
                reader.ReadAsString();
            }
            rectTransform.SetAllSimilarProps(reader, typeof(RectTransform), serializer, _customInitializers);
            
            return rectTransform;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(RectTransform);
        }
    }
}