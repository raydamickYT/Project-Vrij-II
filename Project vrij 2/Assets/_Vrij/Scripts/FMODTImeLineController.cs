using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Voor EventTrigger
using FMODUnity;
using FMOD.Studio;

public class FMODTimelineController : MonoBehaviour
{
    public Slider timelineSlider;
    public StudioEventEmitter musicEmitter;
    public GameObject player; // Het karakter dat door het level beweegt
    public Transform startPosition; // Beginpositie van het level
    public Transform endPosition; // Eindpositie van het level

    private EventInstance musicInstance;
    private PLAYBACK_STATE playbackState;
    private Rigidbody2D PlayerRigidBody;
    private Collider2D PlayerCollider;
    private int totalMusicLength;
    private bool isDragging = false;

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

        if (musicInstance.isValid())
        {
            musicInstance.setTimelinePosition(0);
            musicInstance.start();
        }
        if (player != null)
        {
            PlayerRigidBody = player.GetComponent<Rigidbody2D>();
            PlayerCollider = player.GetComponent<Collider2D>();
        }
    }

    void Update()
    {
        if (musicInstance.isValid())
        {
            if (!isDragging)
            {
                int timelinePosition;
                musicInstance.getTimelinePosition(out timelinePosition);
                timelineSlider.value = timelinePosition / 1000f; // Zet tijd in seconden
                UpdatePlayerPosition(timelinePosition);
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
        if (musicInstance.isValid() && isDragging)
        {
            int timelinePosition = (int)(value * 1000); // Zet tijd in milliseconden
            musicInstance.setTimelinePosition(timelinePosition);
            UpdatePlayerPosition(timelinePosition);
        }
    }

    public void OnPointerDown()
    {
        isDragging = true;
        if (PlayerRigidBody != null)
        {
            PlayerRigidBody.bodyType = RigidbodyType2D.Static;
        }
        if (PlayerCollider != null)
        {
            PlayerCollider.enabled = false;
        }
    }

    public void OnPointerUp()
    {
        isDragging = false;
        if (PlayerCollider != null)
        {
            PlayerCollider.enabled = true;
        }
        if (PlayerRigidBody != null)
        {
            PlayerRigidBody.bodyType = RigidbodyType2D.Dynamic;
        }
        OnTimelineSliderChanged(timelineSlider.value); // Update de muziekpositie wanneer de gebruiker de slider loslaat
    }

    private void UpdatePlayerPosition(int timelinePosition)
    {
        if (player != null && startPosition != null && endPosition != null)
        {
            float normalizedTime = (float)timelinePosition / totalMusicLength;
            Vector3 newPosition = Vector3.Lerp(startPosition.position, endPosition.position, normalizedTime);
            var TempYPos = newPosition.y + 1;

            // Zorg ervoor dat de speler alleen horizontaal beweegt:
            if (isDragging)
            {
                newPosition.y = TempYPos;
            }
            else
            {
                newPosition.y = player.transform.position.y;
                newPosition.z = player.transform.position.z;
            }

            player.transform.position = newPosition;
        }
    }
}
