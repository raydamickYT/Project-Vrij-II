using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Voor EventTrigger
using FMODUnity;
using FMOD.Studio;

public class FMODTimelineController : MonoBehaviour
{
    public Slider timelineSlider;
    public Button PauseButton;
    public StudioEventEmitter musicEmitter;
    public GameObject player; // Het karakter dat door het level beweegt
    public Transform startPosition; // Beginpositie van het level
    public Transform endPosition; // Eindpositie van het level
    public Text timeText; // Text component om de huidige tijd weer te geven

    private EventInstance musicInstance;
    private PLAYBACK_STATE playbackState;
    private int totalMusicLength;
    private bool isDraggingPlayer = false, isPaused = false;
    private Rigidbody2D playerRigidbody;
    private Collider2D playerCollider;
    private Vector3 velocity = Vector3.zero; // Velocity voor SmoothDamp
    public float smoothTime = 0.3f; // De tijd die nodig is om de beweging te dempen

    void Start()
    {
        if (musicEmitter != null)
        {
            musicInstance = musicEmitter.EventInstance;
            musicInstance.getDescription(out var eventDescription);
            eventDescription.getLength(out totalMusicLength); // Lengte in milliseconden
        }

        if (timelineSlider != null)
        {
            timelineSlider.minValue = 0;
            timelineSlider.maxValue = totalMusicLength / 1000f; // Lengte in seconden
            timelineSlider.onValueChanged.AddListener(OnTimelineSliderChanged);
        }

        if (PauseButton != null)
        {
            PauseButton.onClick.AddListener(TogglePause);
        }

        if (musicInstance.isValid())
        {
            musicInstance.setTimelinePosition(0);
            musicInstance.start();
        }

        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerCollider = player.GetComponent<Collider2D>();
        }
    }

    void Update()
    {
        if (musicInstance.isValid())
        {
            int timelinePosition;
            musicInstance.getTimelinePosition(out timelinePosition);

            if (!isDraggingPlayer)
            {
                timelineSlider.value = timelinePosition / 1000f; // Zet tijd in seconden
                UpdatePlayerPosition(timelinePosition);
                UpdateTimeText(timelinePosition);
            }

            musicInstance.getPlaybackState(out playbackState);
            if (playbackState == PLAYBACK_STATE.STOPPED)
            {
                // Reset of loop het level
                musicInstance.setTimelinePosition(0);
                musicInstance.start();
            }
        }
    }

    public void OnTimelineSliderChanged(float value)
    {
        if (musicInstance.isValid() && isDraggingPlayer)
        {
            int timelinePosition = (int)(value * 1000); // Zet tijd in milliseconden
            musicInstance.setTimelinePosition(timelinePosition);
            UpdatePlayerPosition(timelinePosition);
            UpdateTimeText(timelinePosition);
        }
    }

    public void OnPointerDown()
    {
        isDraggingPlayer = true;
        if (playerRigidbody != null)
        {
            playerRigidbody.gravityScale = 0;
        }
        if (playerCollider != null)
        {
            playerCollider.enabled = false; // Schakel de Collider2D uit
        }
    }

    public void OnPointerUp()
    {
        isDraggingPlayer = false;
        if (playerRigidbody != null)
        {
            playerRigidbody.gravityScale = 1;
        }
        if (playerCollider != null)
        {
            playerCollider.enabled = true; // Schakel de Collider2D weer in
        }
    }

    private void UpdatePlayerPosition(int timelinePosition)
    {
        if (player != null && startPosition != null && endPosition != null)
        {
            float normalizedTime = (float)timelinePosition / totalMusicLength;
            Vector3 targetPosition = Vector3.Lerp(startPosition.position, endPosition.position, normalizedTime);

            // If dragging, set the position directly without smoothing
            if (isDraggingPlayer)
            {
                player.transform.position = new Vector3(targetPosition.x, player.transform.position.y, player.transform.position.z);
                velocity = Vector3.zero; // Reset velocity to prevent jump on release
            }
            else
            {
                // Use SmoothDamp for smooth movement
                player.transform.position = Vector3.SmoothDamp(player.transform.position, targetPosition, ref velocity, smoothTime);
            }
        }
    }

    private void UpdateTimeText(int timelinePosition)
    {
        if (timeText != null)
        {
            int minutes = timelinePosition / 60000;
            int seconds = timelinePosition / 1000 % 60;
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            musicInstance.setPaused(true);
            PauseButton.GetComponentInChildren<Text>().text = "Resume";
        }
        else
        {
            musicInstance.setPaused(false);
            PauseButton.GetComponentInChildren<Text>().text = "Pause";
        }
    }
}
