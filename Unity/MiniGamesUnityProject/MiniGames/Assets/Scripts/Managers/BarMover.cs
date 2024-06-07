using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BarMover : MonoBehaviour
{
    public MiniGameManager miniGameManager;
    public RectTransform bar; // Reference to the moving bar's RectTransform
    public RectTransform segments; // Reference to the colored segments' RectTransform
    public Button stopButton; // Reference to the button
    public Text scoreText; // Reference to the UI Text for displaying the score

    public float speed = 300f; // Speed of the bar
    private bool movingRight = true;
    private int score = 0, MaxScore = 12;
    private float offset = 0.5f;
    private bool isStopped = false;
    void OnEnable()
    {
        scoreText.text = "Score: " + score.ToString() + "/" + MaxScore;
    }

    void Start()
    {
        if (miniGameManager == null)
        {
            Debug.LogWarning($"minigamemanager is not connected to {gameObject.name}");
        }
        stopButton.onClick.AddListener(OnStopButtonPressed);
    }

    void Update()
    {
        if (!isStopped)
        {
            MoveBar();
        }
    }

    void MoveBar()
    {
        if (movingRight)
        {
            bar.anchoredPosition += Vector2.right * speed * Time.deltaTime;
            if (bar.anchoredPosition.x >= (segments.rect.width - offset) / 2)
                movingRight = false;
        }
        else
        {
            bar.anchoredPosition -= Vector2.right * speed * Time.deltaTime;
            if (bar.anchoredPosition.x <= (-segments.rect.width + offset) / 2)
                movingRight = true;
        }
    }

    void OnStopButtonPressed()
    {
        if (!isStopped)
        {
            // Determine the score based on the bar's position
            float normalizedPosition = (bar.anchoredPosition.x + (segments.rect.width + offset) / 2) / (segments.rect.width + offset);

            if (normalizedPosition < 0.2f || normalizedPosition > 0.8f) // Red zone
            {
                score -= 1;
            }
            else if ((normalizedPosition >= 0.2f && normalizedPosition < 0.4f) || (normalizedPosition > 0.6f && normalizedPosition <= 0.8f)) // Orange zone
            {
                score += 1;
            }
            else if (normalizedPosition >= 0.4f && normalizedPosition <= 0.6f) // Green zone
            {
                score += 2;
            }

            scoreText.text = "Score: " + score.ToString() + "/" + MaxScore;

            if (score >= MaxScore)
            {
                miniGameManager.OnMiniGameFinished();
                return;
            }

            // Stop the bar temporarily
            StartCoroutine(StopBarTemporarily());
        }
    }

    IEnumerator StopBarTemporarily()
    {
        isStopped = true;
        yield return new WaitForSeconds(0.1f); // Stop for 0.1 seconds
        isStopped = false;
    }

    void OnDisable()
    {
        StopAllCoroutines();
        score = 0;
        scoreText.text = "Score: " + score.ToString() + "/" + MaxScore;

    }
}
