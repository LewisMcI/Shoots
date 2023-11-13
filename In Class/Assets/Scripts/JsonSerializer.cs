using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonSerializer : MonoBehaviour
{
    public static string SerializeJsonArray(string[] jsonArray)
    {
        return JsonUtility.ToJson(new JsonArrayWrapper { jsons = jsonArray });
    }

    public static string[] DeserializeJsonArray(string serializedJsonArray)
    {
        // Deserialize the JSON array back into a list of JSON strings
        JsonArrayWrapper wrapper = JsonUtility.FromJson<JsonArrayWrapper>(serializedJsonArray);
        return new List<string>(wrapper.jsons).ToArray();
    }
    public static string SerializeUlongArray(ulong[] ulongArray)
    {
        string serializedArray = string.Join(",", ulongArray);
        return serializedArray;
    }
    public static ulong[] DeserializeUlongArray(string serializedArray)
    {
        string[] stringValues = serializedArray.Split(',');
        ulong[] ulongArray = new ulong[stringValues.Length];

        for (int i = 0; i < stringValues.Length; i++)
        {
            if (ulong.TryParse(stringValues[i], out ulong value))
            {
                ulongArray[i] = value;
            }
            else
            {
                // Handle parsing errors as needed (e.g., throw an exception or use a default value).
                // Here, we set the element to 0 for simplicity.
                ulongArray[i] = 0;
            }
        }

        return ulongArray;

    }
    [System.Serializable]
    class JsonArrayWrapper
    {
        public string[] jsons;
    }
}
