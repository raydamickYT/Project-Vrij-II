using UnityEngine;

public class UnityToJavaScript : MonoBehaviour
{
    void Start()
    {
        // Voorbeeld om een bericht naar JavaScript te sturen bij de start van het spel
        SendMessageToJavaScript("Unity game is gestart!");
    }

    public void SendMessageToJavaScript(string message)
    {
        Application.ExternalCall("receiveMessageFromUnity", message);
    }

    // Voorbeeld functie die wordt aangeroepen wanneer een mini-game is voltooid
    public void OnMiniGameComplete(bool success)
    {
        string resultMessage = success ? "Mini-game voltooid!" : "Mini-game gefaald!";
        SendMessageToJavaScript(resultMessage);
    }
}
