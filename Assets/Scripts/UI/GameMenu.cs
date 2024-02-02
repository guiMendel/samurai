using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameMenu : MonoBehaviour
{
    public Object mainMenuScene;

    bool leftReady = false;
    bool rightReady = false;

    Label leftLabel;
    Label rightLabel;
    VisualElement container;

    public void Show()
    {
        container.style.display = DisplayStyle.Flex;
    }

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        container = root.Q<VisualElement>("Container");
        leftLabel = root.Q<Label>("LeftRematch");
        rightLabel = root.Q<Label>("RightRematch");

        container.style.display = DisplayStyle.None;
    }

    private void ToggleReady(bool isLeft)
    {
        bool newValue;
        Label label;

        if (isLeft)
        {
            newValue = !leftReady;
            leftReady = newValue;
            label = leftLabel;
        }
        else
        {
            newValue = !rightReady;
            rightReady = newValue;
            label = rightLabel;
        }

        label.text = newValue ? "Ready!" : "Attack for rematch";

        if (leftReady && rightReady)
            TriggerRematch();
    }

    private void TriggerRematch()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene.name);
    }
}
