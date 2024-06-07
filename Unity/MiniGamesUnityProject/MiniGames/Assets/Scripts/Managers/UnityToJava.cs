using System.ComponentModel;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using System;


public class UnityToJavaScript : MonoBehaviour
{
    public int Count = 10;
    void Start()
    {
        // Voorbeeld om een bericht naar JavaScript te sturen bij de start van het spel
        SendMessageToJavaScript("Unity game is gestart!", "info");
    }

    public void SendMessageToJavaScript(string message, string type)
    {
        var jsonMessage = JsonUtility.ToJson(new UnityMessage { type = type, message = message });
        Application.ExternalCall("receiveMessageFromUnity", jsonMessage);
    }

    // Deze methode wordt aangeroepen door JavaScript
    public void ReceiveMessageFromJavaScript(string message)
    {
        Debug.Log("Bericht ontvangen van JavaScript: " + message);
        if (message.Contains("ShowButton"))
        {
            Debug.Log("showbutton");
            if (message.Contains("count"))
            {
                Debug.Log("Count");
                // Zoek naar het getal achter "count"
                string pattern = @"count"":\s*(\d+)";
                Match match = Regex.Match(message, pattern);

                if (match.Success)
                {
                    Count = int.Parse(match.Groups[1].Value);
                    Debug.Log("Extracted count value: " + Count);
                    GameManager.Instance.OnStartButtonPressed();

                    // Verwerk de countValue zoals nodig
                    // Bijvoorbeeld:
                    // GameManager.Instance.SetCountValue(countValue);
                }
                else
                {
                    Debug.LogError("Kon het getal achter 'count' niet vinden.");
                }
            }
        }
        // var unityMessage = JsonUtility.FromJson<UnityMessage>(message.data);
        // Debug.Log("parsed message " + unityMessage);
        // if (unityMessage != null && unityMessage.type == "ShowButton")
        // {
        //     int miniGameTime = unityMessage.count;
        //     Debug.Log("MiniGameTime: " + miniGameTime);
        //     GameManager.Instance.OnStartButtonPressed();
        // }
        // switch (message)
        // {
        //     case string a when a.Contains("ShowButton"):
        //         // Zoek naar het getal achter "MiniGameTime:"
        //         string pattern = @"Count:\s*(\d+)";
        //         Match match = Regex.Match(a, pattern);

        //         if (match.Success)
        //         {
        //             int miniGameTime = int.Parse(match.Groups[1].Value);
        //             Debug.Log("MiniGameTime: " + miniGameTime);

        //             // Verwerk de miniGameTime zoals nodig
        //             // Bijvoorbeeld:
        //             // GameManager.Instance.SetMiniGameTime(miniGameTime);
        //         }
        //         break;
        // }

        // Verwerk het bericht zoals nodig
    }

    // Voorbeeld functie die wordt aangeroepen wanneer een mini-game is voltooid
    public void OnMiniGameComplete(bool success)
    {
        string resultMessage = success ? "Mini-game voltooid!" : "Mini-game gefaald!";
        SendMessageToJavaScript(resultMessage, "PerformUnityAction");
        // GameManager.Instance.sceneLoader.ShowScene("Assets/Scenes/WaitingScreen.unity");
        Debug.Log(resultMessage);
        Debug.Log("unload sample scene");
        // GameManager.Instance.sceneLoader.HideScene(GameManager.Instance.sceneLoader.SelectedMiniGame);
    }

    // Methode om aan te roepen bij een knopdruk
    public void OnButtonPress()
    {
        SendMessageToJavaScript("Knop is ingedrukt!", "info");
        OnMiniGameComplete(true);
    }
}

[System.Serializable]
public class UnityMessage
{
    public string type;
    public string message;
    public int count;
}
