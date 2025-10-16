using UnityEngine;
using System.Collections.Generic;

public static class SceneData
{
    private static Dictionary<string, object> data = new Dictionary<string, object>();

    public static void Set(string key, object value)
    {
        if (data.ContainsKey(key))
            data[key] = value;
        else
            data.Add(key, value);
    }

    public static T Get<T>(string key)
    {
        if (data.TryGetValue(key, out var value))
            return (T)value;

        return default;
    }

    public static bool Has(string key) => data.ContainsKey(key);

    public static void Clear() => data.Clear();
}

/*

//* Example Usage
// Before triggering the scene transition:

SceneData.Set("PlayerName", "Pedro");
SceneData.Set("PuzzleSolvedCount", 3);
EventManager.Instance.TriggerEventString("FadeToScene", "Chapter2");

// In the next scene, you can retrieve it like this:

string name = SceneData.Get<string>("PlayerName");
int count = SceneData.Get<int>("PuzzleSolvedCount");

*/