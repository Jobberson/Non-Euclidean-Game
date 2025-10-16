using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnCheckpointReached()
    {
        SaveLoadManager.Instance.SaveCurrentScene();
    }
}