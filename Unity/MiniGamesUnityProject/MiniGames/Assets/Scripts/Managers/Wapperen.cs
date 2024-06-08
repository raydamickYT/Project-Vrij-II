using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointerMovementGame : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public GameObject Button;
    public Sprite BellowOpen, BellowClose;
    private Vector2 startPosition;
    private bool isPointerDown = false;
    private bool isClosed = false;
    private float movementThreshold = 10.0f; // Drempelwaarde voor beweging

    public MiniGameManager miniGameManager;
    public RectTransform bar; // Reference to the moving slider's RectTransform

    public float downSpeed = 0.5f; // Speed at which the slider moves down every frame
    public float upSpeed = 0.5f; // Speed at which the slider moves up when button is pressed
    private bool isGameOver = false, hasBlown = false;
    private bool isMovingUp;
    public float TopBorder = 3.2f, bottomborder = 0;

    void Start()
    {
        if (miniGameManager == null)
        {
            Debug.LogWarning($"minigamemanager is not connected to {gameObject.name}");
        }
    }
    void AddPoints()
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

    void MoveBar()
    {
        if (!isMovingUp)
        {
            if (bar != null)
            {
                bar.anchoredPosition -= Vector2.up * downSpeed * Time.deltaTime;
                if (bar.anchoredPosition.y <= bottomborder)
                {
                    isMovingUp = true;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (isClosed)
        {
            if (BellowClose != null && !hasBlown)
            {
                Button.GetComponent<Image>().sprite = BellowClose;
                AddPoints();
                hasBlown = true;
            }

        }
        else
        {
            if (BellowOpen != null)
                Button.GetComponent<Image>().sprite = BellowOpen;
            hasBlown = false;
        }
    }
    private void Update()
    {
        if (!isGameOver)
        {
            MoveBar();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPosition = eventData.position;
        isPointerDown = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isPointerDown) return;

        float movementDelta = eventData.position.x - startPosition.x;

        if (movementDelta > movementThreshold)
        {
            isClosed = false;
        }
        else if (movementDelta < -movementThreshold)
        {
            isClosed = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPointerDown)
        {
            isPointerDown = false;
        }
    }


    private void OnDisable()
    {
        isMovingUp = false;
        isGameOver = false;
        hasBlown = false;
        if (Button != null)
            Button.GetComponent<Image>().sprite = BellowClose;

    }
}
