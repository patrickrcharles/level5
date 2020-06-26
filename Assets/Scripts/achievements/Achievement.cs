using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Achievement : MonoBehaviour
{
    [SerializeField]
    public int achievementId;
    [SerializeField]
    public string achievementName;
    [SerializeField]
    private string achievementDescription;

    [SerializeField]
    bool characterUnlock;
    [SerializeField]
    bool levelUnlock;
    [SerializeField]
    bool modeUnlock;
    [SerializeField]
    private int playerId;
    [SerializeField]
    private int levelId;
    [SerializeField]
    private int modeId;

    [SerializeField]
    private int activationValueInt;
    [SerializeField]
    private float activationValueFloat;

    // other custom 
    [SerializeField]
    bool consecutive;
    [SerializeField]
    bool count;
    [SerializeField]
    bool time;
    [SerializeField]
    bool three;
    [SerializeField]
    bool four;
    [SerializeField]
    bool seven;
    [SerializeField]
    private int playerRequiredToUseId;
    [SerializeField]
    private int levelRequiredToUseId;
    [SerializeField]
    private int modeRequiredToUseId;

    [SerializeField]
    private bool GREATER_THAN;
    [SerializeField]
    private bool LESS_THAN;
    [SerializeField]
    private bool EQUAL_TO;

    [SerializeField]
    bool isLocked;

    public int PlayerId { get => playerId; set => playerId = value; }
    public bool IsLocked { get => isLocked; set => isLocked = value; }
    public string AchievementDescription { get => achievementDescription; set => achievementDescription = value; }

    public void checkCharacterUnlockConditions(int pid, int lid, int mid, int activateValue)
    {
        bool condition1 = false;
        bool condition2 = false;
        bool condition3 = false;

        // if unlocks character and requires counter
        if (characterUnlock && count && IsLocked)
        {
            Debug.Log("aid: " + achievementId + " pid = " + pid + " lid : " + lid + " mid :" + mid + " " + activationValueInt + " :" + activateValue);
            Debug.Log("unlockable : " + achievementName);

            if (playerRequiredToUseId == 0 && checkActivateValue(activateValue))
            {
                condition1 = true;
            }
            if (levelRequiredToUseId == 0 && checkActivateValue(activateValue))
            {
                condition2 = true;
            }
            if (modeRequiredToUseId == 0 && checkActivateValue(activateValue))
            {
                condition3 = true;
            }
            // != 0 is check to see achievement needspid, lid, or mid
            if (pid == playerRequiredToUseId && checkActivateValue(activateValue) && !condition1)
            {
                condition1 = true;
            }          
            if(lid == levelRequiredToUseId && checkActivateValue(activateValue) && !condition2)
            {
                condition2 = true;
            }
            if (mid == modeRequiredToUseId && checkActivateValue(activateValue) && !condition3)
            {
                condition3 = true;
            }
            if (condition1 && condition2 && condition3)
            {
                IsLocked = false;
                messageLog.instance.toggleMessageDisplay("character id: " + PlayerId + " unlocked ");
                //Debug.Log("====================================  character id: " + playerId + " unlocked ");
            }
        }
    }

    public bool checkActivateValue(int activateValue)
    {
        bool activate = false;

        if (EQUAL_TO)
        {
            if(activateValue == activationValueInt)
            {
                activate = true;
            }
        }
        if (LESS_THAN)
        {
            if (activateValue < activationValueInt)
            {
                activate = true;
            }
        }
        if (GREATER_THAN)
        {
            //Debug.Log(activate + " > " + activationValueInt);
            if (activateValue >= activationValueInt)
            {
                activate = true;
            }
        }
        Debug.Log("activate : " + activate);
        return activate;
    }
}
