using System.Collections;
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

    const string characterNameName = "health_slider_character_text";
    const string healthSliderValueName = "health_slider_value_text";
    public static PlayerHealthBar instance;

    // Start is called before the first frame update
    void Start()
    {
        //GameOptions.sniperEnabled = true; // test flag
        if (GameOptions.enemiesEnabled || GameOptions.sniperEnabled || GameOptions.EnemiesOnlyEnabled)
        {
            instance = this;
            playerHealth = GameLevelManager.instance.Player.GetComponentInChildren<PlayerHealth>();
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
        healthSlider.value = playerHealth.Health;
        healthSliderValueText.text = healthSlider.value.ToString("0") + " / " + playerHealth.MaxHealth;
    }
    public void setBlockSliderValue()
    {
        blockSlider.value = playerHealth.Block;
    }

    public IEnumerator DisplayDamageTakenValue(int damage)
    {
        //transform.localScale = temp;
        GameLevelManager.instance.PlayerController.DamageDisplayValueText.text = "-" + damage.ToString();
        yield return new WaitForSeconds(0.7f);
        GameLevelManager.instance.PlayerController.DamageDisplayValueText.text = "";
    }
    public IEnumerator DisplayCustomMessageOnDamageDisplay(string message)
    {

        GameLevelManager.instance.PlayerController.DamageDisplayValueText.text = message;
        yield return new WaitForSeconds(0.7f);
        GameLevelManager.instance.PlayerController.DamageDisplayValueText.text = "";
    }
}
