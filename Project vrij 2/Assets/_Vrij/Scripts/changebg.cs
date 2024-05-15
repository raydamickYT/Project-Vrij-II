using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changebg : MonoBehaviour
{
    // Je kunt deze waarde direct in de Unity Editor aanpassen
    public Color newBackgroundColor = Color.blue;

    void Start()
    {
        // Verkrijg de Camera component en verander de achtergrondkleur
        Camera.main.backgroundColor = newBackgroundColor;
        DelegateManager.Instance.ExecuteJumpDelegate += SetBackgroundColor;
    }
    public void SetBackgroundColor()
    {
        Debug.Log("executed");
        // Camera.main.backgroundColor = Color.blue;
    }

    void OnDestroy()
    {
        DelegateManager.Instance.ExecuteJumpDelegate -= SetBackgroundColor;

    }
}
