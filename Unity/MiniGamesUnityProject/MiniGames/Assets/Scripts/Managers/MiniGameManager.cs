using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager instance;


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

    public void OnMiniGameFinished()
    {
        // GameManager.Instance.sceneLoader.LoadScenes("Assets/Scenes/WaitingScreen.unity");
        // GameManager.Instance.sceneLoader.UnloadScenes("Assets/Scenes/SampleScene.unity");
    }


    void OnDestroy()
    {
        instance = null;
    }
}
