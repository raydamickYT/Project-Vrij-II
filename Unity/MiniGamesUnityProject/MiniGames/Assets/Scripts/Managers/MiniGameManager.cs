using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    public Button TestButton;
    [SerializeField]
    private bool miniGameFinished;
    public Slider slider; // Maak de slider publiek zodat deze in de Inspector kan worden ingesteld
    public float gameDuration = 10f; // Duur van de minigame in seconden

    void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.unityToJava != null && GameManager.Instance.unityToJava.Count > 0)
            {
                gameDuration = GameManager.Instance.unityToJava.Count;
            }
            else
            {
                gameDuration = 10;
            }
        }
        else
        {
            gameDuration = 10;
        }

        if (slider == null)
        {
            slider = FindObjectOfType<Canvas>().GetComponentInChildren<Slider>();
            slider.maxValue = gameDuration;
            slider.value = gameDuration;
        }
        else
        {
            slider.maxValue = gameDuration;
            slider.value = gameDuration;
        }

        StartCoroutine(TimeToFinishMiniGame());
    }

    void Awake()
    {
        if (TestButton != null)
        {
            TestButton.onClick.AddListener(OnMiniGameFinished);
        }

        if (slider == null)
        {
            Debug.LogError("Slider is not assigned");
        }
    }


    public void OnMiniGameFinished()
    {
        miniGameFinished = true;
        GameEnded();
    }

    public void GameEnded()
    {
        GameManager.Instance.MiniGameEnded(miniGameFinished);
    }

    private IEnumerator TimeToFinishMiniGame()
    {
        Debug.Log("Coroutinwerkt");
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
        StopAllCoroutines();
        miniGameFinished = false;
    }
}
