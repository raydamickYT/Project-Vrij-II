using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SceneLoader sceneLoader;

    private void Start()
    {
        // Load the start screen initially
        sceneLoader.LoadScenes("StartScreen");
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OnStartButtonPressed()
    {
        // Unload the start screen and load a random minigame
        sceneLoader.LoadRandomMinigame();
        sceneLoader.UnloadScenes("StartScreen");
    }

    public void OnButtonPressed(string currentScene, string nextScene)
    {
        // Unload the start screen and load a random minigame
        sceneLoader.LoadScenes(nextScene);
        sceneLoader.UnloadScenes(currentScene);
    }

    public void OnMinigameCompleted()
    {
        // Unload the current minigame and load the waiting screen
        sceneLoader.LoadRandomMinigame();
    }
}
