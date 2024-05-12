using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class ConnectionCounter : MonoBehaviour
{
    public Text ConnectionText;
    public int ConnectedClients = 0;

   void Update()
    {
        ConnectionText.text = "connected clients: " + ConnectedClients;
    }

    void FixedUpdate()
    {
        if (WebSocketWorker.Instance != null)
        {
            ConnectedClients = WebSocketWorker.Instance.ConnectedClients;
        }
        else
        {
            Debug.LogWarning("websocketworked bestaat nog niet");
        }
    }
}
