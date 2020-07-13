using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsTableAllTime : MonoBehaviour
{
    const string twoName = "two_points";
    const string threeName = "three_points";
    const string fourName = "four_points";
    const string sevenName = "seven_points";
    const string moneyballName = "moneyball";
    const string hitByCarName = "hit_by_car";
    const string totalDistanceName = "total_distance";
    const string totalShotsName = "total_shots";
    const string totalPointsName = "total_points";
    const string timePlayedName = "time_played";

    Text twoText;
    Text threeText;
    Text fourText;
    Text sevenText;
    Text moneyBallText;
    Text hitByCarText;
    Text totalDistanceText;
    Text totalShotsText;
    Text totalPointsText;
    Text timePlayedText;

    // Start is called before the first frame update
    void Awake()
    {
        // find object, get second child objects text.
        // 1st child : score description, 2nd : actual score

        twoText = GameObject.Find(twoName).transform.GetChild(1).GetComponent<Text>();
        threeText = GameObject.Find(threeName).transform.GetChild(1).GetComponent<Text>();
        fourText = GameObject.Find(fourName).transform.GetChild(1).GetComponent<Text>();
        sevenText = GameObject.Find(sevenName).transform.GetChild(1).GetComponent<Text>();
        moneyBallText = GameObject.Find(moneyballName).transform.GetChild(1).GetComponent<Text>();
        hitByCarText = GameObject.Find(hitByCarName).transform.GetChild(1).GetComponent<Text>();
        totalDistanceText = GameObject.Find(totalDistanceName).transform.GetChild(1).GetComponent<Text>();
        totalShotsText = GameObject.Find(totalShotsName).transform.GetChild(1).GetComponent<Text>();
        totalPointsText = GameObject.Find(totalPointsName).transform.GetChild(1).GetComponent<Text>();
        timePlayedText = GameObject.Find(timePlayedName).transform.GetChild(1).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
