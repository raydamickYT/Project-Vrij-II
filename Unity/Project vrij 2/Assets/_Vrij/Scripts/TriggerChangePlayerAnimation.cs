using UnityEngine;

public class TriggerChangePlayerAnimation : MonoBehaviour
{
    // The player GameObject
    // public GameObject player;
    // Animation clips
    public AnimationClip collidedAnimation;
    public AnimationClip normalAnimation;

    // Reference to the Animator component on the player
    // private Animator playerAnimator;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.tag == "Player")
        {
            // Toggle the collided state
            other.GetComponent<Animator>().Play(collidedAnimation.name, 0, 0);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Animator>().Play(normalAnimation.name, 0, 0);

        }
    }
}