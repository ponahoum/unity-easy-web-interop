
using System;
using UnityEngine;
namespace Nahoum.UnityJSInterop
{
    /// <summary>
    /// A simple serializer that converts an object to a JSON string
    /// </summary>
    public static class ObjectSerializer
    {
        public static string ToJson(object toSerialize)
        {
            string baseJson = "{\"value\":%value%}";
            if (toSerialize == null)
                baseJson = baseJson.Replace("%value%", "null");
            if(toSerialize.GetType().IsArray)
                baseJson = baseJson.Replace("%value%", SerializeArray(toSerialize));
            else
                baseJson = baseJson.Replace("%value%", SerializeNativeType(toSerialize));
            return baseJson;
        }

        private static string SerializeNativeType(object targetObject){
            if(targetObject == null)
                return "null";
            else if(targetObject is int || targetObject is float || targetObject is double || targetObject is long)
                return targetObject.ToString().Replace(",", ".");
            else if(targetObject is string)
                return "\"" + targetObject.ToString() + "\"";
            else if(targetObject is byte)
                return targetObject.ToString();
            else if(targetObject is sbyte)
                return targetObject.ToString();
            else if(targetObject is bool)
                return targetObject.ToString().ToLower();
            else if(targetObject is Type asType)
                return "\"" + asType.FullName + "\"";
            else
                return JsonUtility.ToJson(targetObject);
        }

        private static string SerializeArray(object array){
            string result = "[";
            var asArray = (System.Array)array;
            foreach(var item in asArray){
                result += SerializeNativeType(item) + ",";
            }
            if(asArray.Length > 0)
                result = result.Remove(result.Length - 1);

            result += "]";
            return result;
        }
    }
}