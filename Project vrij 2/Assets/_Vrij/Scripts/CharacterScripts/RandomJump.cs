using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class RandomJump
{
    public float RandJump;
    public float Border = 50;
    public RandomJump()
    {
        DelegateManager.Instance.UpdateSliderDelegate?.Invoke(50);
    }

    public bool RandJumpVoid()
    {
        RandJump = Random.Range(0, 100);
        if (RandJump > Border)
        {
            // UpdateSlider(RandJump);
            DelegateManager.Instance.UpdateSliderDelegate?.Invoke(RandJump);
            return true;
        }
        else
        {
            // UpdateSlider(RandJump);
            DelegateManager.Instance.UpdateSliderDelegate?.Invoke(RandJump);
            return false;
        }
    }


}
