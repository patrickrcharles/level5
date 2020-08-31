using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class LoadedData : MonoBehaviour
{
    // load start screen data for players/ friend/ mode /level
    [SerializeField]
    private List<ShooterProfile> playerSelectedData;
    // list off cheerleader profile data
    [SerializeField]
    private List<StartScreenCheerleaderSelected> cheerleaderSelectedData;
    // list off level  data
    [SerializeField]
    private List<StartScreenLevelSelected> levelSelectedData;
    //mode selected objects
    [SerializeField]
    private List<StartScreenModeSelected> modeSelectedData;

    internal bool dataLoaded;

    public static LoadedData instance;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        StartCoroutine(LoadStartScreenData());
    }

    IEnumerator LoadStartScreenData()
    {
        yield return new WaitUntil(() => LoadManager.instance.playerDataLoaded);
        playerSelectedData = LoadManager.instance.PlayerSelectedData;

        yield return new WaitUntil(() => LoadManager.instance.cheerleaderDataLoaded);
        cheerleaderSelectedData = LoadManager.instance.CheerleaderSelectedData;

        yield return new WaitUntil(() => LoadManager.instance.levelDataLoaded);
        levelSelectedData = LoadManager.instance.LevelSelectedData;

        yield return new WaitUntil(() => LoadManager.instance.modeDataLoaded);
        modeSelectedData = LoadManager.instance.ModeSelectedData;

        if (playerSelectedData != null
            && cheerleaderSelectedData != null
            && levelSelectedData != null
            && modeSelectedData != null)
        {
            dataLoaded = true;
        }
    }

    public List<ShooterProfile> PlayerSelectedData { get => playerSelectedData; set => playerSelectedData = value; }
    public List<StartScreenCheerleaderSelected> CheerleaderSelectedData { get => cheerleaderSelectedData; set => cheerleaderSelectedData = value; }
    public List<StartScreenLevelSelected> LevelSelectedData { get => levelSelectedData; set => levelSelectedData = value; }
    public List<StartScreenModeSelected> ModeSelectedData { get => modeSelectedData; set => modeSelectedData = value; }

}
