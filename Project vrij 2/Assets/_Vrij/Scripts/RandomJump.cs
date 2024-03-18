using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class RandomJump
{
    public Slider CanJumpSlider;
    public float RandJump;
    public float Border = 50;
    public RandomJump(Slider _slider)
    {
        CanJumpSlider = _slider;
        UpdateSlider(50);
    }

    public bool RandJumpVoid()
    {
        RandJump = Random.Range(0, 100);
        if (RandJump > Border)
        {
            UpdateSlider(RandJump);
            return true;
        }
        else
        {
            UpdateSlider(RandJump);
            return false;
        }
    }
    private void UpdateSlider(float value)
    {
        CanJumpSlider.value = value;
    }
    
}
