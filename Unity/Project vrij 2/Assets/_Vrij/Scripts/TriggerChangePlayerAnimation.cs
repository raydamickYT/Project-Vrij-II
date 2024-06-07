using UnityEngine;

public class TriggerChangePlayerAnimation : MonoBehaviour
{
    // The player GameObject
    public GameObject player;
    // Animation clips
    public AnimationClip collidedAnimation;
    public AnimationClip normalAnimation;

    // Reference to the Animator component on the player
    private Animator playerAnimator;

    void Start()
    {
        if (player != null)
        {
            // Get the Animator component from the player GameObject
            playerAnimator = player.GetComponent<Animator>();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is the player
        if (collision.gameObject == player)
        {
            // Toggle the collided state
            playerAnimator.Play(collidedAnimation.name, 0, 0);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            playerAnimator.Play(normalAnimation.name, 0, 0);
        }
    }
}