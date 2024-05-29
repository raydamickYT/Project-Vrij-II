using System;
using System.Threading.Tasks;
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

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        WebSocketSetup();

        //delegates
        DelegateManager.Instance.TextEventTriggerDetected += SendMessageToServer;
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

                if (message == null || string.IsNullOrEmpty(message.type))
                {
                    Debug.LogError("Invalid or incomplete message data.");
                    return;
                }

                switch (message.type)
                {
                    case "count":
                        ConnectedClients = message.count;
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
    }

    void Update()
    {
        // //stress test
        // Parallel.For(0, 100, (index) =>
        // {
        //     ServerMessage msg = new ServerMessage { message = "message", type = "type" };
        //     string jsonMessage = JsonUtility.ToJson(msg);
        //     ws.Send(jsonMessage);
        // });

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     if (ws != null && ws.IsAlive)
        //     {
        //         ServerMessage msg = new ServerMessage { message = "hello", type = "ShowButton" };
        //         string jsonMessage = JsonUtility.ToJson(msg);
        //         ws.Send(jsonMessage);
        //         // ws.Send(JsonUtility.ToJson(new ServerMessage { message = "hello", type = "ShowButton" }));
        //         Debug.Log("keypressed");
        //     }
        //     else
        //     {
        //         Debug.LogError("Server is niet verbonden. Check de url");
        //         return;
        //     }
        // }
    }

    public void SendMessageToServer(string message, string type)
    {
        if (ws != null && ws.IsAlive)
        {
            ServerMessage msg = new ServerMessage { message = message, type = type };
            string jsonMessage = JsonUtility.ToJson(msg);
            ws.Send(jsonMessage);
            Debug.Log("Message sent to server: " + jsonMessage);
        }
        else
        {
            Debug.LogError("Server is niet verbonden. Check de url");
        }
    }

    void OnDestroy()
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
