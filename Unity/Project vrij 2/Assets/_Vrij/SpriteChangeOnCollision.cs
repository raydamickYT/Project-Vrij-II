using UnityEngine;

public class SpriteChangeOnCollision : MonoBehaviour
{
    // Reference to the Animator component
    private Animator animator;

    // Tags to check for collision
    public string collisionTag = "Collidable";

    // Animation state names
    public string normalAnimation = "Normal";
    public string collidedAnimation = "Collided";

    // To track if the object is currently in the collided state
    private bool isCollided = false;

    void Start()
    {
        // Get the Animator component from the GameObject
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision object's tag matches the specified tag
        if (collision.gameObject.CompareTag(collisionTag))
        {
            // Toggle the collided state
            isCollided = !isCollided;

            // Switch animation state
            if (animator != null)
            {
                if (isCollided)
                {
                    animator.Play(collidedAnimation);
                }
                else
                {
                    animator.Play(normalAnimation);
                }
            }
        }
    }
}
