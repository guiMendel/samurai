using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public Object gameScene;

    public void StartGame()
    {
        SceneManager.LoadScene(gameScene.name);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting game!");
    }

    public void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button startButton = root.Q<Button>("Start");
        Button quitButton = root.Q<Button>("Quit");

        startButton.clicked += StartGame;
        quitButton.clicked += QuitGame;
    }
}
