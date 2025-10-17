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
        if (!newGameButton || !continueButton || !quitButton || !settingsButton)
        {
            Debug.LogError("One or more buttons are not assigned in the inspector!", this);
            return;
        }

        if (!settingsPanel)
            Debug.LogWarning("Settings panel is not assigned.", this);

        SetContinueButtonActive(SaveLoadManager.Instance?.HasSave() ?? false);

        newGameButton.onClick.AddListener(() => SaveLoadManager.Instance.NewGame());
        continueButton.onClick.AddListener(() => SaveLoadManager.Instance.ContinueGame());
        quitButton.onClick.AddListener(QuitGame);
        settingsButton.onClick.AddListener(OpenSettings);
    }

    private void OnEnable()
    {
        settingsPanel?.SetActive(false);
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

    private void DisableAllButtons()
    {
        newGameButton.interactable = false;
        continueButton.interactable = false;
        quitButton.interactable = false;
        settingsButton.interactable = false;
    }

}