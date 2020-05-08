using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private int playerId;
    [SerializeField]
    private float money;
    [SerializeField]
    private int experience;
    [SerializeField]
    private int level;

    public static PlayerStats instance;

    void Start()
    {
        instance = this;

        // this should be loaded from saved data
    }


    public float Money
    {
        get => money;
        set => money = value;
    }

    public int Experience
    {
        get => experience;
        set => experience = value;
    }

    public int Level
    {
        get => level;
        set => level = value;
    }
}
