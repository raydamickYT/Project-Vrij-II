using UnityEngine;

public class CollisionActivator : MonoBehaviour
{
    // Reference to the GameObject you want to turn on
    public GameObject targetObject;

    // Method called when the collider attached to this GameObject collides with another collider
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Target object is not assigned.");
        }
    }

}
