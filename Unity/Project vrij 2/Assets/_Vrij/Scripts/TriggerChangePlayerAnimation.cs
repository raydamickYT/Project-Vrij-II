using UnityEngine;

public class TriggerChangePlayerAnimation : MonoBehaviour
{
    // The player GameObject
    public GameObject player;

    // Animation state names
    public string normalAnimation = "Normal";
    public string collidedAnimation = "Collided";

    // Reference to the Animator component on the player
    private Animator playerAnimator;

    // To track if the player's animation is currently in the collided state
    private static bool isCollided = false;

    void Start()
    {
        if (player != null)
        {
            // Get the Animator component from the player GameObject
            playerAnimator = player.GetComponent<Animator>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.gameObject == player)
        {
            // Toggle the collided state
            isCollided = !isCollided;

            // Switch player's animation state
            if (playerAnimator != null)
            {
                if (isCollided)
                {
                    playerAnimator.Play(collidedAnimation);
                }
                else
                {
                    playerAnimator.Play(normalAnimation);
                }
            }
        }
    }
}

