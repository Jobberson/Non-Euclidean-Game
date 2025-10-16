using UnityEngine;
using UnityEngine.SceneManagement;
using AASave;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    public SaveSystem saveSystem;
    
    public static SaveLoadManager Instance;

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

    private void Start()
    {
        // Check if save exists and enable Continue button accordingly
        bool hasSave = saveSystem.DoesDataExists("CurrentScene");
        UIManager.Instance.SetContinueButtonActive(hasSave);
    }

    public void NewGame()
    {
        // delete previous save
        if (saveSystem.DoesDataExists("CurrentScene"))
        {
            saveSystem.Delete("CurrentScene");
        }

        // Load first scene
        SceneManager.LoadScene("Scene_01");
    }

    public void ContinueGame()
    {
        if (saveSystem.DoesDataExists("CurrentScene"))
        {
            string sceneToLoad = saveSystem.Load("CurrentScene", "Scene_01").AsString();
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            NewGame();
        }
    }

    public void SaveCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        saveSystem.Save("CurrentScene", currentScene);
    }
}