using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTrigger : MonoBehaviour
{
    public GameObject EventTrigger;
    public int Dist, DistTime;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if (playerRigidbody != null && EventTrigger != null)
        // {
        //     Dist = CalculateDistance();
        //     DistTime = CalculateTimeToEvent();
        // }
    }

    int CalculateDistance()
    {
        // Bereken de afstand tussen dit object en het EventTrigger object
        return (int)Mathf.Abs(transform.position.x - EventTrigger.transform.position.x);
    }

    public int CalculateTimeToEvent(float Speed)
    {
        // Als de snelheid 0 is, kan de tijd niet worden berekend
        if (Speed == 0)
        {
            return -1; // Retourneer -1 als de snelheid 0 is
        }

        // Bereken de tijd om de afstand af te leggen
        return (int)(CalculateDistance() / Speed);
    }
}
