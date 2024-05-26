using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
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
                    GameObject go = new GameObject("WebSocketWorker");
                    instance = go.AddComponent<WebSocketWorker>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    public string Url = "ws://localhost:3000/unity";
    public bool EnableDebugLogging = true;
    private bool isReconnecting = false;

    public WebSocket WebSocket
    {
        get { return ws; }
        private set { ws = value; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        WebSocketSetup();

        // Delegates
        DelegateManager.Instance.TextEventTriggerDetected += SendMessageToServer;
    }

    private async void WebSocketSetup()
    {
        if (!Url.StartsWith("ws://") && !Url.StartsWith("wss://"))
        {
            Debug.LogError("Invalid URL: " + Url);
            return;
        }

        ws = new WebSocket(Url);

        ws.OnOpen += (sender, e) =>
        {
            if (EnableDebugLogging) Debug.Log("WebSocket connection opened.");
        };

        ws.OnError += async (sender, e) =>
        {
            Debug.LogError("Error from WebSocket connection: " + e.Message);
            await AttemptReconnect();
        };

        ws.OnClose += async (sender, e) =>
        {
            Debug.Log("WebSocket connection closed: " + e.Reason);
            await AttemptReconnect();
        };

        ws.OnMessage += (sender, e) =>
        {
            if (EnableDebugLogging) Debug.Log("Message received: " + e.Data);
            try
            {
                var message = JsonUtility.FromJson<ServerMessage>(e.Data);

                if (message == null || string.IsNullOrEmpty(message.type))
                {
                    Debug.LogError("Invalid or incomplete message data.");
                    return;
                }

                switch (message.type)
                {
                    case "count":
                        ConnectedClients = message.count;
                        if (EnableDebugLogging) Debug.Log("Connected clients: " + message.count);
                        break;
                    case "PerformUnityAction":
                        DelegateManager.Instance.AddInputToListDelegate?.Invoke();
                        if (EnableDebugLogging) Debug.Log("Action performed successfully");
                        break;
                    default:
                        Debug.LogWarning("Received unknown type: " + message.type);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error parsing JSON: " + ex.Message);
            }
        };

        await ConnectWebSocketAsync();
    }

    private Task ConnectWebSocketAsync()
    {
        var tcs = new TaskCompletionSource<bool>();

        ws.OnOpen += (sender, e) => tcs.SetResult(true);
        ws.ConnectAsync();
        
        return tcs.Task;
    }

    private async Task AttemptReconnect()
    {
        if (isReconnecting) return;
        isReconnecting = true;

        while (!ws.IsAlive)
        {
            if (EnableDebugLogging) Debug.Log("Attempting to reconnect...");
            await Task.Delay(5000); // Wait for 5 seconds before retrying
            await ConnectWebSocketAsync();
        }

        isReconnecting = false;
    }

    private void Update()
    {
        // Optional: other updates can be done here
    }

    private void SendMessageToServer(Text TextData, string type)
    {
        if (ws != null && ws.IsAlive)
        {
            ServerMessage msg = new ServerMessage
            {
                message = TextData != null ? TextData.text : "",
                type = type
            };
            string jsonMessage = JsonUtility.ToJson(msg);
            ws.Send(jsonMessage);
            if (EnableDebugLogging) Debug.Log("Message sent to server: " + jsonMessage);
        }
        else
        {
            Debug.LogError("Server is niet verbonden. Check de URL.");
        }
    }

    private void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
        }
    }

    [Serializable]
    public class ServerMessage
    {
        public string type;
        public bool success;
        public int count;
        public string message;
    }
}
