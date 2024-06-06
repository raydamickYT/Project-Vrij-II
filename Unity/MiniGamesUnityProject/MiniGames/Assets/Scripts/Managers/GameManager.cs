using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SceneLoader sceneLoader;
    [SerializeField]
    private bool isDebugging;
    private UnityToJavaScript unityToJava;

    void Awake()
    {
        if (unityToJava == null)
        {
            unityToJava = GetComponent<UnityToJavaScript>();
        }
    }

    private void Start()
    {
        if (!isDebugging)
        {
            // Load the start screen initially
            sceneLoader.LoadScenes("StartScreen");
        }
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //start een minigame
            sceneLoader.LoadRandomMinigame();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            sceneLoader.HideScene("StartScreen");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            sceneLoader.ShowScene("StartScreen");
        }
    }
    public void MiniGameEnded(bool Succeeded)
    {
        unityToJava.OnMiniGameComplete(Succeeded);
    }

    public void OnStartButtonPressed()
    {
        // Unload the start screen and load a random minigame
        sceneLoader.LoadRandomMinigame();
        sceneLoader.ShowScene(sceneLoader.SelectedMiniGame);
        sceneLoader.HideScene("StartScreen");
    }

    public void OnButtonPressed(string currentScene, string nextScene)
    {
        // Unload the start screen and load a random minigame
        sceneLoader.ShowScene(nextScene);
        sceneLoader.HideScene(currentScene);
    }

    public void OnMinigameCompleted()
    {
        // Unload the current minigame and load the waiting screen
        sceneLoader.LoadRandomMinigame();
    }
}
