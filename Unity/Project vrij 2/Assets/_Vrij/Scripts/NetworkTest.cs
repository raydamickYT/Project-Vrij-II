using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkTest : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(CheckInternetConnection());
    }

    IEnumerator CheckInternetConnection()
    {
        string url = "https://www.google.com";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Connection successful!");
                Debug.Log("Received: " + webRequest.downloadHandler.text);
            }
        }
    }
}
