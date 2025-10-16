using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Button newGameButton;
    public Button continueButton;

    private void Awake()
    {
        Instance = this;
    }

    public void SetContinueButtonActive(bool isActive)
    {
        continueButton.interactable = isActive;
    }

    public void SetupMenuButtons()
    {
        newGameButton.onClick.AddListener(() => SaveLoadManager.Instance.NewGame());
        continueButton.onClick.AddListener(() => SaveLoadManager.Instance.ContinueGame());
    }
}