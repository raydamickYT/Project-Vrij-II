using System.ComponentModel;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class UnityToJavaScript : MonoBehaviour
{
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
        switch (message)
        {
            case string a when a.Contains("ShowButton"):
                Debug.Log("ShowButton");
                // Zoek naar het getal achter "MiniGameTime:"
                string pattern = @"MiniGameTime:\s*(\d+)";
                Match match = Regex.Match(a, pattern);

                if (match.Success)
                {
                    int miniGameTime = int.Parse(match.Groups[1].Value);
                    Debug.Log("MiniGameTime: " + miniGameTime);

                    // Verwerk de miniGameTime zoals nodig
                    // Bijvoorbeeld:
                    // GameManager.Instance.SetMiniGameTime(miniGameTime);
                }
                GameManager.Instance.OnStartButtonPressed();
                break;
        }
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
}
