using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : Singleton<MainMenuController>
{
    public Button newGameButton;
    public Button continueButton;
    public Button quitButton;
    public Button settingsButton;

    public GameObject settingsPanel;

    private void Start()
    {
        SetContinueButtonActive(SaveLoadManager.Instance.HasSave());

        newGameButton.onClick.AddListener(() => SaveLoadManager.Instance.NewGame());
        continueButton.onClick.AddListener(() => SaveLoadManager.Instance.ContinueGame());
        quitButton.onClick.AddListener(QuitGame);
        settingsButton.onClick.AddListener(OpenSettings);
    }

    public void SetContinueButtonActive(bool isActive)
    {
        continueButton.interactable = isActive;
    }

    private void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    private void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }
}