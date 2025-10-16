using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    private readonly Dictionary<string, GameTimer> timers = new();

    public static TimerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        // Copy to avoid "collection modified" errors if timers remove themselves
        var activeTimers = new List<GameTimer>(timers.Values);
        foreach (var timer in activeTimers)
        {
            timer.Update(deltaTime);
        }
    }

    public void CreateTimer(string id, float duration, bool isLooping, System.Action onComplete)
    {
        if (timers.ContainsKey(id))
        {
            Debug.LogWarning($"Timer with ID '{id}' already exists.");
            return;
        }

        GameTimer timer = new GameTimer(id, duration, isLooping, onComplete);
        timers.Add(id, timer);
    }

    public void StopTimer(string id)
    {
        if (timers.TryGetValue(id, out var timer))
        {
            timer.Stop();
        }
    }

    public void PauseTimer(string id)
    {
        if (timers.TryGetValue(id, out var timer))
        {
            timer.Pause();
        }
    }

    public void ResumeTimer(string id)
    {
        if (timers.TryGetValue(id, out var timer))
        {
            timer.Resume();
        }
    }

    public bool TimerExists(string id) => timers.ContainsKey(id);

    public void RemoveTimer(string id)
    {
        timers.Remove(id);
    }

    public Dictionary<string, GameTimer> GetAllTimers() => timers;
}
