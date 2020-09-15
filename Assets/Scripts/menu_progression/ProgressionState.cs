using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionState  : MonoBehaviour
{
    int addTo3;
    int addTo4;
    int addTo7;
    int pointsAvailable;
    int pointsUsedThisSession;

    int accuracy3;
    int accuracy4;
    int accuracy7;

    int range;
    int release;
    int luck;
    int level;
    int experience;

    int playerId;

    public static ProgressionState instance;

    public int AddTo3 { get => addTo3; set => addTo3 = value; }
    public int AddTo4 { get => addTo4; set => addTo4 = value; }
    public int AddTo7 { get => addTo7; set => addTo7 = value; }
    public int PointsAvailable { get => pointsAvailable; set => pointsAvailable = value; }
    public int PointsUsedThisSession { get => pointsUsedThisSession; set => pointsUsedThisSession = value; }
    public int Accuracy3 { get => accuracy3; set => accuracy3 = value; }
    public int Accuracy4 { get => accuracy4; set => accuracy4 = value; }
    public int Accuracy7 { get => accuracy7; set => accuracy7 = value; }
    public int Range { get => range; set => range = value; }
    public int Release { get => release; set => release = value; }
    public int Luck { get => luck; set => luck = value; }
    public int Level { get => level; set => level = value; }
    public int Experience { get => experience; set => experience = value; }

    private void Awake()
    {
        instance = this;
    }

    public void initializeState(CharacterProfile characterProfile)
    {
        accuracy3 = (int)characterProfile.Accuracy3Pt;
        accuracy4 = (int)characterProfile.Accuracy4Pt;
        accuracy7 = (int)characterProfile.Accuracy7Pt;

        range = (int)characterProfile.Range;
        release = (int)characterProfile.Release;
        luck = (int)characterProfile.Luck;

        pointsAvailable = (int)characterProfile.PointsAvailable;
        level = (int)characterProfile.Level;
        experience = (int)characterProfile.Experience;

        playerId = characterProfile.PlayerId;

    }

    public void clearState()
    {
        accuracy3 = 0;
        accuracy4 = 0;
        accuracy7 = 0;

        range = 0;
        release = 0;
        luck = 0;

        pointsAvailable = 0;
        level = 0;
        level = 0;

        playerId = 0;

    }
}
