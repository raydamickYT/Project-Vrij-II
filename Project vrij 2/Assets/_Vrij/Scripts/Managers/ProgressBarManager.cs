using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarManager : MonoBehaviour
{
    [SerializeField] private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
    }
    void OnEnable()
    {
        DelegateManager.Instance.UpdateSliderDelegate += UpdateSliderProgress;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateSliderProgress(float data)
    {
        slider.value = data;
    }

    void OnDestroy()
    {
        DelegateManager.Instance.UpdateSliderDelegate -= UpdateSliderProgress;
    }
}
