using UnityEngine;
using UnityEngine.UI;

public class GloryBar : MonoBehaviour
{
    [SerializeField] Slider slider;

    public void SetMaxGlory(float maxGlory)
    {
        slider.maxValue = maxGlory;
    }

    public void SetGlory(float currentGlory)
    {
        slider.value = currentGlory;
    }
}
