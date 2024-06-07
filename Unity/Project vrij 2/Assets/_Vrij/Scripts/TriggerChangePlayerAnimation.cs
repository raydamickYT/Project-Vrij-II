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

    // Animator override controller
    private AnimatorOverrideController animatorOverrideController;

    // To track if the player's animation is currently in the collided state
    private static bool isCollided = false;

    void Start()
    {
        if (player != null)
        {
            // Get the Animator component from the player GameObject
            playerAnimator = player.GetComponent<Animator>();

            // Create an AnimatorOverrideController and assign it to the player's Animator
            if (playerAnimator != null)
            {
                animatorOverrideController = new AnimatorOverrideController(playerAnimator.runtimeAnimatorController);
                playerAnimator.runtimeAnimatorController = animatorOverrideController;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.gameObject == player)
        {
            // Toggle the collided state
           

            // Switch player's animation state
            if (playerAnimator != null && animatorOverrideController != null)
            {
                animatorOverrideController["Normal"] = collidedAnimation;
                Debug.Log("IsCollided" + this.gameObject.name);
                // Force the Animator to restart the state
                playerAnimator.Play("Normal", 0, 0);
               
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerAnimator != null && animatorOverrideController != null)
        {
            animatorOverrideController["Normal"] = normalAnimation;
            Debug.Log("IsCollided" + this.gameObject.name);
            // Force the Animator to restart the state
            playerAnimator.Play("Normal", 0, 0);
            Debug.Log("TriggerOnExit");
        }
    }
}
