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

    [System.Serializable]
    class JsonArrayWrapper
    {
        public string[] jsons;
    }
}
