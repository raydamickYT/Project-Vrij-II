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
        // Debug.Log("Bericht ontvangen van JavaScript: " + message);
        // if (message.Contains("ShowButton"))
        // {
        //     Debug.Log("showbutton");
        //     if (message.Contains("count"))
        //     {
        //         Debug.Log("Count");
        //         // Zoek naar het getal achter "count"
        //         string pattern = @"count"":\s*(\d+)";
        //         Match match = Regex.Match(message, pattern);

        //         if (match.Success)
        //         {
        //             Count = int.Parse(match.Groups[1].Value);
        //             Debug.Log("Extracted count value: " + Count);
        //             GameManager.Instance.OnStartButtonPressed();

        //             // Verwerk de countValue zoals nodig
        //             // Bijvoorbeeld:
        //             // GameManager.Instance.SetCountValue(countValue);
        //         }
        //         else
        //         {
        //             Debug.LogError("Kon het getal achter 'count' niet vinden.");
        //         }
        //     }
        // }
        // else if (message.Contains("ShowButterFly"))
        // {
        //     Debug.Log("showButton");
        //     if (message.Contains("count"))
        //     {
        //         Debug.Log("Count");
        //         // Zoek naar het getal achter "count"
        //         string pattern = @"count"":\s*(\d+)";
        //         Match match = Regex.Match(message, pattern);

        //         if (match.Success)
        //         {
        //             Count = int.Parse(match.Groups[1].Value);
        //             Debug.Log("Extracted count value: " + Count);
        //             GameManager.Instance.sceneLoader.LoadScenes(GameManager.Instance.sceneLoader.ButterflyGame);

        //             // Verwerk de countValue zoals nodig
        //             // Bijvoorbeeld:
        //             // GameManager.Instance.SetCountValue(countValue);
        //         }
        //         else
        //         {
        //             Debug.LogError("Kon het getal achter 'count' niet vinden.");
        //         }
        //     }
        // }
        switch (message)
        {
            case string a when a.Contains("ShowButton"):
                Debug.Log("showbutton");
                HandleCount(message, () => GameManager.Instance.OnStartButtonPressed());
                break;

            case string b when b.Contains("ShowButterFly"):
                Debug.Log("showButterfly");
                HandleCount(message, () => GameManager.Instance.ShowButterFlyGame());
                break;
        }
    }

    void HandleCount(string message, Action onSuccess)
    {
        if (message.Contains("count"))
        {
            Debug.Log("Count");
            string pattern = @"count"":\s*(\d+)";
            Match match = Regex.Match(message, pattern);

            if (match.Success)
            {
                Count = int.Parse(match.Groups[1].Value);
                Debug.Log("Extracted count value: " + Count);
            }
            else
            {
                Debug.LogError("Kon het getal achter 'count' niet vinden.");
            }
            onSuccess?.Invoke();
        }
    }

    // Voorbeeld functie die wordt aangeroepen wanneer een mini-game is voltooid
    public void OnMiniGameComplete(bool success)
    {
        string resultMessage = success ? "Mini-game voltooid!" : "Mini-game gefaald!";
        if (success)
        {
            SendMessageToJavaScript(resultMessage, "PerformUnityAction");
        }
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
