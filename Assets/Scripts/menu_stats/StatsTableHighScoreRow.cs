using UnityEngine;
using UnityEngine.UI;

public class StatsTableHighScoreRow : MonoBehaviour
{
    const string scoreName = "score";
    const string characterName = "character";
    const string levelName = "level";
    const string dateName = "date";
    const string hardcoreName = "hardcore";
    const string userName = "userName";

    public Text userNameText;
    public Text scoreText;
    public Text characterText;
    public Text levelText;
    public Text dateText;
    public Text hardcoreText;

    [SerializeField]
    public string Score;
    [SerializeField]
    public string Character;
    [SerializeField]
    public string Level;
    [SerializeField]
    public string Date;
    [SerializeField]
    public string HardcoreEnabled;
    [SerializeField]
    public string UserName;
    [SerializeField]
    public string HardCoreEnabled;
    [SerializeField]
    public string TrafficEnabled;
    [SerializeField]
    public string EnemiesEnabled;
    [SerializeField]
    public string Platform;

    // Start is called before the first frame update
    void Start()
    {
        userNameText = transform.GetChild(0).GetComponent<Text>();
        scoreText = transform.GetChild(1).GetComponent<Text>();
        characterText = transform.GetChild(2).GetComponent<Text>();
        levelText = transform.GetChild(3).GetComponent<Text>();
        dateText = transform.GetChild(4).GetComponent<Text>();
        hardcoreText = transform.GetChild(5).GetComponent<Text>();
    }

    private void Update()
    {
        scoreText.text = Score;
        characterText.text = Character;
        levelText.text = Level;
        dateText.text = Date;
        hardcoreText.text = HardcoreEnabled;
        userNameText.text = UserName;

        //Debug.Log("score = " + score + " | character =" +character + " | level = " + level + " | date = " +date);
    }

    //public StatsTableHighScoreRow(string scor, string charact, string lvl, string dat, string hrdcor, string uname)
    //{
    //    UserName = uname;
    //    Score = scor;
    //    Character = charact;
    //    Level = lvl;
    //    Date = dat;
    //    HardcoreEnabled = hrdcor;
    //}

    public void  setRowValues(string scor, string charact, string lvl, string dat, string hrdcor, string uname)
    {
        UserName = uname;
        Score = scor;
        Character = charact;
        Level = lvl;
        Date = dat;
        HardcoreEnabled = hrdcor;
    }
}
