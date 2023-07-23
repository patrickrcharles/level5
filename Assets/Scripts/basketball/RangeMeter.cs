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
    const string sliderTextName = "range_slider_value_text";
    const string statsTextName = "range_slider_stats_text";

    [SerializeField]
    float range;

    void Start()
    {
        if (GameLevelManager.instance.AutoPlayer)
        {
            characterProfile = GameLevelManager.instance.AutoPlayer.GetComponent<CharacterProfile>();
        }
        else
        {
            characterProfile = GameLevelManager.instance.Player1.GetComponent<CharacterProfile>();
        }
        slider = GetComponentInChildren<Slider>();
        sliderText = GameObject.Find(sliderTextName).GetComponent<Text>();
        statText = GameObject.Find(statsTextName).GetComponent<Text>();

        //range = characterProfile.Range;

        statText.text = "range : " + transform.root.GetComponent<CharacterProfile>().Range + " ft";

        InvokeRepeating("setSliderValue", 0, 0.1f);

        if (!GameLevelManager.instance.IsAutoPlayer && ( GameOptions.hardcoreModeEnabled || GameOptions.EnemiesOnlyEnabled || GameOptions.battleRoyalEnabled
            || !GameOptions.gameModeHasBeenSelected))
        {
            gameObject.SetActive(false);
        }
    }

    void setSliderValue()
    {
        if (slider != null && sliderText != null)
        {
            float distance = GameLevelManager.instance.IsAutoPlayer ? GameLevelManager.instance.AutoPlayerController.PlayerDistanceFromRim : GameLevelManager.instance.PlayerController1.PlayerDistanceFromRim;
            slider.value = (characterProfile.Range / (distance * 6)) * 100;
            sliderText.text = slider.value.ToString("0") + "%";
            //Debug.Log("slider.value : " + slider.value.ToString());
        }
    }
}
