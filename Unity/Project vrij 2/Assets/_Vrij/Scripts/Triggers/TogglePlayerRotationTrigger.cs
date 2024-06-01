using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TogglePlayerRotationTrigger : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            var rb = other.GetComponent<Rigidbody2D>();

            // Controleer of de rotatie momenteel vergrendeld is
            if ((rb.constraints & RigidbodyConstraints2D.FreezeRotation) == RigidbodyConstraints2D.FreezeRotation)
            {
                // Als de rotatie vergrendeld is, ontgrendel deze
                rb.constraints &= ~RigidbodyConstraints2D.FreezeRotation;
            }
            else
            {
                // Als de rotatie niet vergrendeld is, vergrendel deze
                rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
            }
        }
    }
}
