using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField]
    PlayerHealth playerHealth;

    [SerializeField]
    public Slider healthSlider;
    [SerializeField]
    public Slider blockSlider;

    [SerializeField]
    Text characterNameText;
    [SerializeField]
    Text healthSliderValueText;

    //[SerializeField]
    //Text damageDisplayValueText;
    //[SerializeField]
    //GameObject damageDisplayObject;

    const string characterNameName = "health_slider_character_text";
    const string healthSliderValueName = "health_slider_value_text";
    //const string damagaeDisplayValueName = "player_damage_display_text";

    public static PlayerHealthBar instance;
    public Slider Slider => healthSlider;

    // Start is called before the first frame update
    void Start()
    {
        if (GameOptions.enemiesEnabled)
        {
            instance = this;
            playerHealth = GameLevelManager.instance.Player.GetComponentInChildren<PlayerHealth>();
            //healthSlider = GetComponentInChildren<Slider>();
            healthSlider = transform.Find("health_bar").GetComponent<Slider>();
            blockSlider = transform.Find("block_bar").GetComponent<Slider>();

            healthSlider.maxValue = playerHealth.MaxHealth;
            blockSlider.maxValue = playerHealth.MaxBlock;

            characterNameText = GameObject.Find(characterNameName).GetComponent<Text>();
            healthSliderValueText = GameObject.Find(healthSliderValueName).GetComponent<Text>();

            characterNameText.text = GameLevelManager.instance.Player.GetComponent<CharacterProfile>().PlayerDisplayName;
            setHealthSliderValue();
            setBlockSliderValue();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }


    public void setHealthSliderValue()
    {
        //Debug.Log("playerHealth.Health : " + playerHealth.Health);

        healthSlider.value = playerHealth.Health;
        healthSliderValueText.text = healthSlider.value.ToString("0") + " / " + playerHealth.MaxHealth;
        //Debug.Log("slider.value : " + slider.value.ToString());
    }
    public void setBlockSliderValue()
    {
        //Debug.Log("playerHealth.Block : " + playerHealth.Block);

        blockSlider.value = playerHealth.Block;

        //healthSliderValueText.text = healthSlider.value.ToString("0") + "%";
        //Debug.Log("slider.value : " + slider.value.ToString());
    }

    public IEnumerator DisplayDamageTakenValue(int damage)
    {
        //transform.localScale = temp;

        GameLevelManager.instance.PlayerState.DamageDisplayValueText.text = "-" + damage.ToString();
        yield return new WaitForSeconds(.6f);
        GameLevelManager.instance.PlayerState.DamageDisplayValueText.text = "";
    }
}
