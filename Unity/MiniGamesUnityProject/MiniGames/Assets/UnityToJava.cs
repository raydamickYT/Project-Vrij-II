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
        Application.ExternalCall("unity received message");

        // Verwerk het bericht zoals nodig
    }

    // Deze methode wordt aangeroepen door JavaScript
    public void ReceiveMessageFromJavaScript(string message)
    {
        Debug.Log("Bericht ontvangen van JavaScript: " + message);
        // Verwerk het bericht zoals nodig
    }

    // Voorbeeld functie die wordt aangeroepen wanneer een mini-game is voltooid
    public void OnMiniGameComplete(bool success)
    {
        string resultMessage = success ? "Mini-game voltooid!" : "Mini-game gefaald!";
        SendMessageToJavaScript(resultMessage, "PerformUnityAction");
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
