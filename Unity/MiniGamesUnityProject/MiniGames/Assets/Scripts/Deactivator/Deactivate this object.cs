using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deactivatethisobject : MonoBehaviour
{
    public bool isDebugging;
    private void OnEnable()
    {
        if (!isDebugging)
        {
            this.gameObject.SetActive(false);
        }
    }
}
