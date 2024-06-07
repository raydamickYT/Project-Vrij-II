using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BalanceBar : MonoBehaviour
{
    public MiniGameManager miniGameManager;
    public RectTransform bar; // Reference to the moving slider's RectTransform
    public Slider ScoreSlider;
    public float downSpeed = 50f; // Speed at which the slider moves down every frame
    private bool isGameOver = false;
    private bool isAtBottom, isPressingButton;
    public float TopBorder = 3.2f, bottomborder = 0;
    private int score = 0;
    private float timeAccumulator;

    void Start()
    {
        if (miniGameManager == null)
        {
            Debug.LogWarning($"minigamemanager is not connected to {gameObject.name}");
        }
        if (ScoreSlider == null)
        {
            Debug.LogWarning($"slider is not assigned on {gameObject.name}");
        }
    }

    void Update()
    {
        if (!isGameOver)
        {
            MoveBar();
            checkPos();
        }
        if (score > 100)
        {
            isGameOver = true;
            miniGameManager.OnMiniGameFinished();
        }

    }

    void checkPos()
    {
        float currentPos = bar.anchoredPosition3D.y;

        if (currentPos > -0.26 && currentPos < 0.56)
        {
            timeAccumulator += Time.deltaTime;
            if (timeAccumulator >= 0.1f) // Award points every second
            {
                score += 1;
                timeAccumulator = 0.0f;
                ScoreSlider.value = score;
            }
        }
    }

    void MoveBar()
    {
        if (!isAtBottom && !isPressingButton)
        {
            bar.anchoredPosition -= Vector2.up * downSpeed * Time.deltaTime;
            if (bar.anchoredPosition.y <= bottomborder)
            {
                isAtBottom = true;
            }
        }
        if (isPressingButton)
        {
            if (bar.anchoredPosition.y <= TopBorder)
            {
                bar.anchoredPosition += Vector2.up * downSpeed * Time.deltaTime;
            }
        }
    }

    public void OnPressButton()
    {
        if (!isPressingButton)
            isPressingButton = true;

        if (isAtBottom)
            isAtBottom = false;
    }

    public void OnReleaseButton()
    {
        isPressingButton = false;
    }


    private void OnDisable()
    {
        isPressingButton = false;
        isAtBottom = false;
        isGameOver = false;
        ScoreSlider.value = 0;
    }
}
