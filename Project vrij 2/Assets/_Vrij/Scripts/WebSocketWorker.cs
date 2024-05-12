using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WebSocketSharp;

public class WebSocketWorker : MonoBehaviour
{
    public string url;
    WebSocket ws;
    string DataReceived;

    void Start()
    {
        if (!url.StartsWith("ws://") && !url.StartsWith("wss://"))
        {
            Debug.LogError("Invalid URL: " + url);
            return; // Stop further execution if URL is invalid
        }

        ws = new WebSocket(url);
        ws.Connect();

        ws.OnError += (sender, e) =>
        {
            Debug.LogError("Error from WebSocket connection: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket connection closed: " + e.Reason);
        };

        ws.OnMessage += (sender, e) =>
        {
            DataReceived = e.Data;
            Debug.Log("Data Received: " + DataReceived);
        };

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
                ws.Send("hello");
                Debug.Log("keypressed");

        }
    }
}
