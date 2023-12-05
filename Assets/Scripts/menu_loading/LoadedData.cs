using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedData : MonoBehaviour
{
    // load start screen data for players/ friend/ mode /level
    [SerializeField]
    private List<CharacterProfile> playerSelectedData;
    [SerializeField]
    private List<CharacterProfile> cpuPlayerSelectedData;
    // list off cheerleader profile data
    [SerializeField]
    private List<CheerleaderProfile> cheerleaderSelectedData;
    // list off level  data
    [SerializeField]
    private List<LevelSelected> levelSelectedData;
    //mode selected objects
    [SerializeField]
    private List<StartScreenModeSelected> modeSelectedData;

    [SerializeField] private bool dataLoaded;

    float timeoutStart;
    float timeoutEnd;
    float timeoutMax = 10;

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

    private void Update()
    {
        if (!dataLoaded)
        {
            if (Time.time > timeoutEnd)
            {
                dataLoaded = true;
            }
        }
    }

    IEnumerator LoadStartScreenData()
    {
        timeoutStart = Time.time;
        timeoutEnd = Time.time + timeoutMax;

        yield return new WaitUntil(() => LoadManager.instance.playerDataLoaded);
        playerSelectedData = LoadManager.instance.PlayerSelectedData;

        yield return new WaitUntil(() => LoadManager.instance.cpuPlayerDataLoaded);
        cpuPlayerSelectedData = LoadManager.instance.CpuPlayerSelectedData;

        yield return new WaitUntil(() => LoadManager.instance.cheerleaderDataLoaded);
        cheerleaderSelectedData = LoadManager.instance.CheerleaderSelectedData;

        yield return new WaitUntil(() => LoadManager.instance.levelDataLoaded);
        levelSelectedData = LoadManager.instance.LevelSelectedData;

        yield return new WaitUntil(() => LoadManager.instance.modeDataLoaded);
        modeSelectedData = LoadManager.instance.ModeSelectedData;


        if (playerSelectedData != null
            && cpuPlayerSelectedData != null
            && cheerleaderSelectedData != null
            && levelSelectedData != null
            && modeSelectedData != null)
        {
            dataLoaded = true;
        }
    }

    public CharacterProfile getSelectedCharacterProfile(int charid)
    {
        CharacterProfile temp = new CharacterProfile();
        //CharacterProfile temp = gameObject.AddComponent<CharacterProfile>();

        temp = playerSelectedData.Find(x => x.PlayerId == charid);

        return temp;
    }

    public List<CharacterProfile> PlayerSelectedData { get => playerSelectedData; }
    public List<CheerleaderProfile> CheerleaderSelectedData { get => cheerleaderSelectedData; }
    public List<LevelSelected> LevelSelectedData { get => levelSelectedData; }
    public List<StartScreenModeSelected> ModeSelectedData { get => modeSelectedData; }
    public bool DataLoaded { get => dataLoaded; }
    public List<CharacterProfile> CpuPlayerSelectedData { get => cpuPlayerSelectedData; set => cpuPlayerSelectedData = value; }
}
