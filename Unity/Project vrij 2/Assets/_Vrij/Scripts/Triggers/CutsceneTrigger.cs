using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutsceneTrigger : MonoBehaviour
{
    private VideoPlayer videoPlayer; // Verwijzing naar de VideoPlayer component
    public RawImage rawImage; // Verwijzing naar de RawImage component
    public bool IsPauseTrigger;


    private void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }
        else
        {
            videoPlayer.Prepare();
            videoPlayer.loopPointReached += EndReached; // Voeg een event toe voor wanneer de video eindigt
        }

        if (rawImage != null)
        {
            rawImage.enabled = false;
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
            int videolenght = (int)videoPlayer.length;
            Debug.Log("Length of the video" + videolenght);
            if (IsPauseTrigger)
            {
                DelegateManager.Instance.StartTimerDelegate?.Invoke(videolenght);
            }
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
