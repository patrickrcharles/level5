using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private int playerId;
    [SerializeField]
    private float money;
    [SerializeField]
    private int currentExperience;
    [SerializeField]
    private int currentLevel;

    public static PlayerStats instance;

    void Start()
    {
        instance = this;
        // this should be loaded from saved data
        playerId = GameOptions.playerId;
    }


    public float Money
    {
        get => money;
        set => money = value;
    }

    public int Experience
    {
        get => currentExperience;
        set => currentExperience = value;
    }

    public int Level
    {
        get => currentLevel;
        set => currentLevel = value;
    }
}
