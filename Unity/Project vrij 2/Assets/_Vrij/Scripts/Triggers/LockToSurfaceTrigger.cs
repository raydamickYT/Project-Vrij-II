using UnityEngine;

public class LockToSurfaceTrigger : MonoBehaviour
{
    private Rigidbody2D playerRb;
    private bool isPlayerLocked = false;
    private Transform slopeTransform;
    private Vector3 offset;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerRb = other.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                // Vergrendel de speler aan het schuine oppervlak
                playerRb.gravityScale = 0; // Stop zwaartekracht
                offset = other.transform.position - transform.position;
                isPlayerLocked = true;
                slopeTransform = transform;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isPlayerLocked)
            {
                // Ontgrendel de speler van het schuine oppervlak
                playerRb.gravityScale = 1; // Herstel zwaartekracht
                isPlayerLocked = false;
                slopeTransform = null;
                playerRb = null;
            }
        }
    }

    void Update()
    {
        if (isPlayerLocked && slopeTransform != null)
        {
            // Zorg ervoor dat de speler de beweging van het schuine oppervlak volgt
            playerRb.transform.position = slopeTransform.position + offset;
        }
    }
}
