using UnityEngine;

public class TriggerChangePlayerAnimation : MonoBehaviour
{
    // The player GameObject
    public GameObject player;

    // Animation clips

    public AnimationClip collidedAnimation;
    public AnimationClip normalAnimation;
    public Animation playerAnims;

    // Reference to the Animator component on the player
    private Animator playerAnimator;

    // Animator override controller
    private AnimatorOverrideController animatorOverrideController;

    // To track if the player's animation is currently in the collided state
    

    void Start()
    {
        if (player != null)
        {
            playerAnims = player.GetComponent<Animation>();
            Debug.Log("Player");
            // Get the Animator component from the player GameObject
            playerAnimator = player.GetComponent<Animator>();

            // Create an AnimatorOverrideController and assign it to the player's Animator
            if (playerAnimator != null)
            {
                Debug.Log("PlayerAnima");
                animatorOverrideController = new AnimatorOverrideController(playerAnimator.runtimeAnimatorController);
                playerAnimator.runtimeAnimatorController = animatorOverrideController;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is the player
        if (collision.gameObject == player)
        {
            playerAnims.clip = normalAnimation;

            playerAnims.Play("Base Layer.Normal");

            // Switch player's animation state
            //if (playerAnimator != null && animatorOverrideController != null)
            //{
            //    animatorOverrideController["Normal"] = collidedAnimation;
            //    Debug.Log("IsCollided" + this.gameObject.name);
            //    // Force the Animator to restart the state
            //    playerAnimator.Play("Normal", 0, 0);

            //}
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {

            if (playerAnimator != null && animatorOverrideController != null)
            {
                animatorOverrideController["Normal"] = normalAnimation;
                Debug.Log("IsCollided" + this.gameObject.name);
                // Force the Animator to restart the state
                //playerAnimator.Play("Normal", 0, 0);
                Debug.Log("TriggerOnExit");
            }
        }
    }
}
