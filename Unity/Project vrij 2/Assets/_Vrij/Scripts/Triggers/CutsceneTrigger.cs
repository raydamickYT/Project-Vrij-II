using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutsceneTrigger : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Verwijzing naar de VideoPlayer component
    public RawImage rawImage; // Verwijzing naar de RawImage component


    private void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += EndReached; // Voeg een event toe voor wanneer de video eindigt
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayCutscene();
        }
    }

    private void PlayCutscene()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            videoPlayer.loopPointReached += EndReached; // Voeg een event toe voor wanneer de video eindigt
        }
        if (rawImage != null)
        {
            rawImage.enabled = true;
        }
    }


    private void EndReached(VideoPlayer vp)
    {
        if (videoPlayer != null)
        {
            Debug.Log("Done");
            videoPlayer.Stop(); // Stop de VideoPlayer
        }
        if (rawImage != null)
        {
            rawImage.enabled = false;
        }
    }
}
