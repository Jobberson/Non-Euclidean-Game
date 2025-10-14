using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    private Dictionary<string, GameTimer> timers = new Dictionary<string, GameTimer>();

    public static TimerManager Instance;

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

        foreach (var timer in timers.Values)
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

    public bool TimerExists(string id)
    {
        return timers.ContainsKey(id);
    }

    public void RemoveTimer(string id)
    {
        if (timers.ContainsKey(id))
        {
            timers.Remove(id);
        }
    }

    public Dictionary<string, GameTimer> GetAllTimers()
    {
        return timers;
    }
}
