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

    public Action<Text> TextEventTriggerDetected;
    public Action OtherEventTriggerDetected;
    public Action<float> UpdateSliderDelegate;


    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void OnDestroy()
    {
        TextEventTriggerDetected = null;
        OtherEventTriggerDetected = null;
    }
}
