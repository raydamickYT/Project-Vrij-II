using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelegateManager : MonoBehaviour
{
    #region  Instance Setup
    private static DelegateManager instance;
    public static DelegateManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DelegateManager>();
                if (instance == null)
                {
                    // Optioneel: maak een nieuwe NetworkManager als er geen bestaat.
                    GameObject go = new GameObject("DelegateManager");
                    instance = go.AddComponent<DelegateManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }
    #endregion

    public Action<Text, string> TextEventTriggerDetected;
    public Action OtherEventTriggerDetected;
    public Action<float> UpdateSliderDelegate;
    public Action ExecuteJumpDelegate;
    public Action AddInputToListDelegate, WipeInputListDelegate;


    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // Destroy(gameObject); //comment als je events niet meer werken
        }
    }


    public void OnDestroy()
    {
        // TextEventTriggerDetected = null;
        OtherEventTriggerDetected = null;
    }
}
