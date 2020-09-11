using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RangeMeter : MonoBehaviour
{

    [SerializeField]
    CharacterProfile characterProfile;
    [SerializeField]
    Slider slider;
    public Slider Slider => slider;

    [SerializeField]
    Text sliderText;
    Text statText;
    const string sliderTextName = "slider_value_text";
    const string statsTextName = "slider_stats_text";

    [SerializeField]
    float range;

    // Start is called before the first frame update
    void Start()
    {
        characterProfile = GameLevelManager.instance.Player.GetComponent<CharacterProfile>();
        slider = GetComponentInChildren<Slider>();
        sliderText = GameObject.Find(sliderTextName).GetComponent<Text>();
        statText = GameObject.Find(statsTextName).GetComponent<Text>();

        range = characterProfile.Range;

        statText.text = "range:" + range + " ft";

        InvokeRepeating("setSliderValue", 0, 0.1f);
    }

    void setSliderValue()
    {
        slider.value = (range / (GameLevelManager.instance.PlayerState.playerDistanceFromRim * 6)) * 100;
        sliderText.text = slider.value.ToString("0") + "%";
        //Debug.Log("slider.value : " + slider.value.ToString());
    }
}
