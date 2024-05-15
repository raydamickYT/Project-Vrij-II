using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputLib : MonoBehaviour
{
    // public List<int> InputAmount = new List<int>(); //lijst die bijhoud hoeveel inputs er succesvol zijn
    public int ConnectedClients = 0, InputAmount = 0;


    void OnEnable()
    {
        DelegateManager.Instance.AddInputToListDelegate += AddToList;
        DelegateManager.Instance.WipeInputListDelegate += WipeInputList;
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

    private void AddToList()
    {
        InputAmount++;
    }

    private void WipeInputList()
    {
        InputAmount=0;
    }

    void OnDestroy()
    {
        DelegateManager.Instance.AddInputToListDelegate -= AddToList;
    }

}
