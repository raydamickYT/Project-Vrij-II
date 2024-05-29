using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputLib : MonoBehaviour
{
    public int ConnectedClients = 0, InputAmount = 0;

    void OnEnable()
    {
        DelegateManager.Instance.AddInputToListDelegate += AddToList;
        DelegateManager.Instance.WipeInputListDelegate += WipeInputList;
    }

    void FixedUpdate()
    {
        Debug.Log("input amount: " + InputAmount);
        if (WebSocketWorker.Instance != null)
        {
            ConnectedClients = WebSocketWorker.Instance.ConnectedClients;
        }
        else
        {
            Debug.LogWarning("websocketworked bestaat nog niet");
        }
    }

    private void AddToList()
    {
        InputAmount++;
    }

    private void WipeInputList()
    {
        InputAmount = 0;
        WebSocketWorker.Instance.SendMessageToServer("", "HideButton"); // Stuur een bericht naar de client om de knop te verbergen
    }

    void OnDestroy()
    {
        DelegateManager.Instance.AddInputToListDelegate -= AddToList;
        DelegateManager.Instance.WipeInputListDelegate -= WipeInputList;
    }
}
