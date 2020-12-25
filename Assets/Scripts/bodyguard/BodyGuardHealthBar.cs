using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyGuardHealthBar : MonoBehaviour
{
    [SerializeField]
    BodyGuardHealth bodyGuardHealth;
    [SerializeField]
    public Slider healthSlider;

    public Slider Slider => healthSlider;

    public static PlayerHealthBar instance;

    // Start is called before the first frame update
    void Start()
    {

        bodyGuardHealth = transform.parent.GetComponentInChildren<BodyGuardHealth>();
        healthSlider = GetComponentInChildren<Slider>();
        setHealthSliderValue();
    }

    // Update is called once per frame
    public void setHealthSliderValue()
    {
        healthSlider.value = bodyGuardHealth.Health;
        //healthSliderValueText.text = healthSlider.value.ToString("0") + "%";
        //Debug.Log("slider.value : " + slider.value.ToString());
    }
}
