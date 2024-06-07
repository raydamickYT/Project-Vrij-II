using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarManager : MonoBehaviour
{
    [SerializeField] private Slider slider; //nog veranderen dat de max hiervan wordt gezet aan het begin van het spel (moet de de totaal aantal players zijn)
    public Action EnableSlider, DisableSlider;

    // Start is called before the first frame update
    void Start()
    {
        EnableSlider = () =>
        {
            slider.gameObject.SetActive(true); //hij kan nu zichzelf gwn aan en uit zetten.
            if (slider.value != 0)
            {
                // slider.value = 0;
            }
        };
        DisableSlider = () =>
        {
            slider.gameObject.SetActive(false); //hij kan nu zichzelf gwn aan en uit zetten.
            if (slider.value != 0)
            {
                // slider.value = 0;
            }
        };
        DisableSlider?.Invoke();

    }
    void OnEnable()
    {
        slider.value = 0;
        DelegateManager.Instance.UpdateSliderDelegate += UpdateSliderProgress;
    }
    void OnDisable()
    {
        slider.value = 0;
        DelegateManager.Instance.UpdateSliderDelegate -= UpdateSliderProgress;
    }

    public void UpdateSliderProgress(float Inputs)
    {
        slider.value = Inputs;
    }
    public void SetSliderMax(int maxValue)
    {
        slider.maxValue = maxValue;
    }

    void OnDestroy()
    {
        DelegateManager.Instance.UpdateSliderDelegate -= UpdateSliderProgress;
    }
}
