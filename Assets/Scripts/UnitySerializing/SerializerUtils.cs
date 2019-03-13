using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace UnitySerializing
{
    public static class SerializerUtils
    {
        public static void SerializeProps<T>(this T component, string[] propsNames, JsonWriter writer, JsonSerializer serializer, Dictionary<string, Func<T, object>> customValGetters = null)
            where T : Component
        {
            var compAllProps = component.GetType().GetProperties().Where(q => q.CanRead).ToList(); // cache ?
            
            foreach (var propName in propsNames)
            {
                var propertyInfo = compAllProps.FirstOrDefault(q => q.Name == propName);
                if (propertyInfo == null)
                {
                    Debug.LogWarning($"Not found prop with \"{propName}\" name in {component.GetType()} type.");
                    continue;
                }
                        
                    
                object tval;
                if (customValGetters != null && customValGetters.ContainsKey(propertyInfo.Name))
                {
                    tval = customValGetters[propertyInfo.Name](component);
                }
                else
                {
                    tval = propertyInfo.GetValue(component, null);
                }
                
                if (tval != null)
                {
                    if (tval.GetType().IsNotSerializableType(serializer))
                    {
                        Debug.LogWarning($"Not found serializer to {tval.GetType()} type. Property name: {propertyInfo.Name} in {component.name}");
                        continue;
                    }
                    writer.WritePropertyName(propertyInfo.Name);
                    serializer.Serialize(writer, tval);
                }
                else
                {
                    Debug.LogWarning($"{propertyInfo.Name} is null in {component.GetType()} type");
                } 
            }
        }
        
        public static void WriteType(this JsonWriter writer, Type t)
        {
            writer.WritePropertyName("type");
            writer.WriteValue(t.FullName);
        }

        public static RectTransform CurrentSelectedRectTransform { get; private set; }
        
        //deserializer
        public static void SetAllSimilarProps<T>(this T parentObj, JsonReader reader, Type objectType, JsonSerializer serializer, Dictionary<string, Action<T, object>> customInitializers = null)
            where T : Component
        {
            var currentDepth = reader.Depth;
            while (reader.Read())
            {
                // if reached end object - return
                if (reader.Depth != currentDepth)
                    break;
                
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        var name = (string)reader.Value;
                        
                        reader.Read();
                        object value = null;
                        
                        //read type
                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            value = DeserializeObject(name, reader, serializer);
                        }
                        else if (reader.TokenType == JsonToken.StartArray)
                        {
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.StartObject)
                                {
                                    CurrentSelectedRectTransform = parentObj as RectTransform;
                                    
                                    DeserializeObject(name, reader, serializer);
                                }
                                if (reader.TokenType == JsonToken.EndArray)
                                    break;
                            }
                        }
                        else
                        {   
                            value = reader.Value;   
                        }

                        if (value != null)
                        {
                            if (customInitializers != null && customInitializers.ContainsKey(name))
                            {
                                customInitializers[name](parentObj, value);       
                            }
                            else
                            {
                                PropertyInfo prop = objectType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                                if (prop != null && prop.CanWrite)
                                {
                                    //Debug.LogError("set " + value + " to " + name);
                                    SetValue(parentObj, prop.Name, value);
                                }    
                            }
                            
                        }

                        break;
                }
            }
        }
        
        public static void SetValue(object inputObject, string propertyName, object propertyVal)
        {
            //find out the type
            Type type = inputObject.GetType();

            //get the property information based on the type
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(propertyName);

            //find the property type
            Type propertyType = propertyInfo.PropertyType;

            //Convert.ChangeType does not handle conversion to nullable types
            //if the property type is nullable, we need to get the underlying type of the property
            var targetType = IsNullableType(propertyType) ? Nullable.GetUnderlyingType(propertyType) : propertyType;

            //Returns an System.Object with the specified System.Type and whose value is
            //equivalent to the specified object.
            propertyVal = Convert.ChangeType(propertyVal, targetType);

            //Set the value of the property
            propertyInfo.SetValue(inputObject, propertyVal, null);

        }
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        public static void Serialize(this object toserobj, JsonWriter writer, JsonSerializer serializer)
        {
            if (toserobj.GetType().IsNotSerializableType(serializer))
            {
                Debug.LogWarning($"Not found serializer to {toserobj.GetType()} type.");
                return;
            }
            serializer.Serialize(writer, toserobj);
        }

        public static bool IsNotSerializableType(this Type type, JsonSerializer serializer) =>
            serializer.Converters.All(q => !q.CanConvert(type)) && !type.IsPrimitive && type != typeof(string) && !type.IsEnum; 

        private static Assembly[] assemblies = {typeof(Transform).Assembly, typeof(Image).Assembly, typeof(HorizontalWrapMode).Assembly}; 
        
        private static object DeserializeObject(string name, JsonReader reader, JsonSerializer serializer)
        {
            object value;
            reader.Read();
            var typestr = reader.ReadAsString();
            Type type;
            if (GetTypeFromUnity(name, typestr, out type)) return null;

            //Debug.LogError("trying deserialize " + name + " prop with " + type + " type (" + typestr + ")");           
            value = serializer.Deserialize(reader, type);
            return value;
        }

        private static bool GetTypeFromUnity(string componentName, string typestr, out Type type)
        {
            type = null;
            foreach (var assembly in assemblies)
            {
                type = assembly.GetType(typestr);
                if (type != null)
                    break;
            }

            if (type == null)
            {
                Debug.LogWarning($"Invalid type in json! {typestr} type in {componentName} is not found in [{string.Join(", ", assemblies.Select(q => $"[{string.Join(", ", q.Modules.Select(t => t.Name))}]"))}] assemblies");
                return true;
            }

            return false;
        }
    }
}