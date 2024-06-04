using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public string CurrentSceneName, NextSceneName;
    public void OnStartButtonClicked()
    {
        GameManager.Instance.OnStartButtonPressed();
    }

    public void OnButtonPressed()
    {
        GameManager.Instance.OnButtonPressed(CurrentSceneName, NextSceneName);
    }
}
