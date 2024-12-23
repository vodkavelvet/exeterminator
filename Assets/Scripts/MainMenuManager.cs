using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject levelSelectionUI;
    public Button playButton;
    public Button exitButton;
    public Button levelSelectionButton;

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
        levelSelectionButton.onClick.AddListener(OnLevelSelectionBackButtonClicked);
    }
    void OnPlayButtonClicked()
    {
        levelSelectionUI.SetActive(true);
        mainMenuUI.SetActive(false);
    }

    void OnExitButtonClicked()
    {
        Application.Quit();
    }

    void OnLevelSelectionBackButtonClicked()
    {
        levelSelectionUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

}
