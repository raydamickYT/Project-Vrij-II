using System;
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

    public string Url = "ws://localhost:3000";

    public WebSocket WebSocket
    {
        get { return ws; }
        private set { ws = value; }
    }

    void Awake()
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

        DelegateManager.Instance.TextEventTriggerDetected += SendMessageToServer;
    }

    private void WebSocketSetup()
    {
        Debug.Log(Url);
        if (!Url.StartsWith("ws://") && !Url.StartsWith("wss://"))
        {
            Debug.LogError("Invalid URL: " + Url);
            return; // Stop further execution if URL is invalid
        }

        ws = new WebSocket(Url);

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connection opened");
        };

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
                if (message == null || string.IsNullOrEmpty(message.type))
                {
                    Debug.LogError("Invalid or incomplete message data.");
                    return;
                }

                switch (message.type)
                {
                    case "count":
                        ConnectedClients = message.count/2;
                        Debug.Log("Connected clients: " + message.count);
                        break;
                    case "PerformUnityAction":
                        DelegateManager.Instance.AddInputToListDelegate?.Invoke();
                        Debug.Log("Action performed successfully");
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

        try
        {
            ws.Connect();
            Debug.Log("WebSocket connecting...");
        }
        catch (Exception ex)
        {
            Debug.LogError("WebSocket connection exception: " + ex.Message);
        }
    }

    private void SendMessageToServer(Text TextData, string type, int Time)
    {
        if (ws != null && ws.IsAlive)
        {
            if (TextData != null)
            {
                ServerMessage msg = new ServerMessage { message = TextData.text, type = type, count = Time };
                string jsonMessage = JsonUtility.ToJson(msg);
                ws.Send(jsonMessage);
                Debug.Log("keypressed");
            }
            else
            {
                Debug.Log(Time + " " + type);
                ServerMessage msg = new ServerMessage { message = "", type = type, count = Time };
                string jsonMessage = JsonUtility.ToJson(msg);
                ws.Send(jsonMessage);
            }
        }
        else
        {
            Debug.LogError("Server is niet verbonden. Check de url");
            return;
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
        public bool success;
        public int count;
        public string message;
    }
}
