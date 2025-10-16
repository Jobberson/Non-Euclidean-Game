using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static GameManager Instance;

    public void OnCheckpointReached()
    {
        SaveLoadManager.Instance.SaveCurrentScene();
    }
}