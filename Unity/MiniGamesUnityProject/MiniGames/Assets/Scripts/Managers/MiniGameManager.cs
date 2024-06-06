using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager instance;
    public Button TestButton;
    [SerializeField]
    private bool miniGameFinished;
    public Slider slider; // Maak de slider publiek zodat deze in de Inspector kan worden ingesteld
    public float gameDuration = 10f; // Duur van de minigame in seconden

    void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Instance bestaat al");
        }
        if (slider == null)
        {
            slider = GetComponent<Canvas>().GetComponent<Slider>();
        }
        else
        {
            slider.maxValue = gameDuration;
            slider.value = gameDuration;
        }
    }

    void Awake()
    {
        if (TestButton != null)
        {
            TestButton.onClick.AddListener(OnMiniGameFinished2);
        }
        else
        {
            Debug.LogError("TestButton is not assigned");
        }

        if (slider == null)
        {
            Debug.LogError("Slider is not assigned");
        }
    }

    void Start()
    {
        // Start de coroutine om de minigame timer bij te houden
        StartCoroutine(TimeToFinishMiniGame());
    }

    public void OnMiniGameFinished2()
    {
        miniGameFinished = true;
        GameEnded();
    }

    public void GameEnded()
    {
        GameManager.Instance.sceneLoader.ShowScene("StartScreen");
        GameManager.Instance.sceneLoader.HideScene(SceneLoader.Instance.SelectedMiniGame);
        GameManager.Instance.MiniGameEnded(miniGameFinished);
    }

    private IEnumerator TimeToFinishMiniGame()
    {
        float timeRemaining = gameDuration;

        while (timeRemaining > 0 && !miniGameFinished)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;

            // Update de slider
            if (slider != null)
            {
                slider.value = timeRemaining;
            }
        }

        // Einde van de timer of minigame
        if (!miniGameFinished)
        {
            GameEnded();
        }
    }

    void OnDisable()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
