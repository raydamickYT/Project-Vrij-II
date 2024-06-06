 using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager instance;
    [SerializeField]
    private string currentScene = "replace this string";


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void OnMiniGameFinished2()
    {
        Debug.Log("h");
        GameManager.Instance.sceneLoader.LoadScenes("Assets/Scenes/StartScreen.unity");
        GameManager.Instance.sceneLoader.UnloadScenes("Assets/Scenes/MiniGames/" + currentScene + ".unity");
    }


    void OnDestroy()
    {
        instance = null;
    }
}
