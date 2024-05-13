using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WebSocketSharp;

public class WebSocketWorker : MonoBehaviour
{
    private static WebSocketWorker instance;
    private WebSocket ws;
    public int ConnectedClients = 0;

    public static WebSocketWorker Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WebSocketWorker>();
                if (instance == null)
                {
                    // Optioneel: maak een nieuwe NetworkManager als er geen bestaat.
                    GameObject go = new GameObject("WebSocketWorker");
                    instance = go.AddComponent<WebSocketWorker>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    public string Url = "ws://localhost:3000/unity"; //die /unity er achter is om aan de webserver te laten zien dat dit unity is die connect

    public WebSocket WebSocket
    {
        get { return ws; }
        private set { ws = value; }
    }

    string DataReceived;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        WebSocketSetup();
    }
    private void WebSocketSetup()
    {
        if (!Url.StartsWith("ws://") && !Url.StartsWith("wss://"))
        {
            Debug.LogError("Invalid URL: " + Url);
            return; // Stop further execution if URL is invalid
        }

        ws = new WebSocket(Url);
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
            Debug.Log("Message received: " + e.Data);
            try
            {
                var message = JsonUtility.FromJson<ServerMessage>(e.Data);
                if (message.type == "count")
                {
                    ConnectedClients = message.count;
                    Debug.Log("Connected clients: " + message.count);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error parsing JSON: " + ex.Message);
            }
        };
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (ws != null && ws.IsAlive)
            {
                ws.Send("hello");
                Debug.Log("keypressed");
            }
            else
            {
                Debug.LogError("Server is niet verbonden. Check de url");
                return;
            }
        }
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
        }
    }



    [System.Serializable]
    public class ServerMessage
    {
        public string type;
        public int count;
    }
}
