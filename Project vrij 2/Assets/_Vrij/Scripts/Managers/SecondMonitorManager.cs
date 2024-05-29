using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondMonitorManager : MonoBehaviour
{
    void Start()
    {
        // Check of er meer dan één monitor is aangesloten
        if (Display.displays.Length > 1)
        {
            // Activeer de tweede monitor
            Display.displays[1].Activate();
        }
    }
}
