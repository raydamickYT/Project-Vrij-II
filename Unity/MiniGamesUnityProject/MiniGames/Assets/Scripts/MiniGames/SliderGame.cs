using UnityEngine;
using UnityEngine.UI;

public class BarMoverGame : MonoBehaviour
{
    public MiniGameManager miniGameManager;
    public RectTransform bar; // Reference to the moving slider's RectTransform
    public Button pressButton; // Reference to the button

    public float downSpeed = 2f; // Speed at which the slider moves down every frame
    public float upSpeed = 1f; // Speed at which the slider moves up when button is pressed
    private bool isGameOver = false;
    private bool isMovingUp;
    public float TopBorder = 3.2f, bottomborder = 0;

    void Start()
    {
        if (miniGameManager == null)
        {
            Debug.LogWarning($"minigamemanager is not connected to {gameObject.name}");
        }
        pressButton.onClick.AddListener(OnPressButton);
    }

    void Update()
    {
        if (!isGameOver)
        {
            MoveBar();
        }
    }

    void MoveBar()
    {
        if (!isMovingUp)
        {
            bar.anchoredPosition -= Vector2.up * downSpeed * Time.deltaTime;
            if (bar.anchoredPosition.y <= bottomborder)
            {
                isMovingUp = true;
            }
        }
    }

    void OnPressButton()
    {
        var tempPos = bar.anchoredPosition;
        tempPos += Vector2.up * upSpeed;
        isMovingUp = false;

        if (tempPos.y >= TopBorder)
        {
            Debug.Log("minigamecomplete");
            isGameOver = true;
            miniGameManager.OnMiniGameFinished();
        }
        else
        {
            bar.anchoredPosition = tempPos;
        }
    }

    private void OnDisable()
    {
        isMovingUp = false;
        isGameOver = false;
    }
}
