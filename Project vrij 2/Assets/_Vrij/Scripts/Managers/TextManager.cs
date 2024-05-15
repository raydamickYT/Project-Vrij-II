using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public Text localText;
    public void Awake()
    {
        DelegateManager.Instance.TextEventTriggerDetected += ShowText;
        if (localText == null)
        {
            Debug.LogWarning("Text has not been assigned in TextManager");
        }
        else
        {
            localText.gameObject.SetActive(false);
        }
    }

    public void ShowText(Text Data)
    {
        Debug.Log(Data.text);
        localText.text = Data.text;
        StartCoroutine(TextTimer(4));
    }

    void OnDestroy()
    {
        DelegateManager.Instance.TextEventTriggerDetected -= ShowText;
    }

    IEnumerator TextTimer(int Time)
    {
        localText.gameObject.SetActive(true);
        yield return new WaitForSeconds(Time);
        localText.gameObject.SetActive(false);
    }
}
