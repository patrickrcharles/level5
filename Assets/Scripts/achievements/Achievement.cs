using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

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
    bool cheerleaderUnlock;
    [SerializeField]
    bool levelUnlock;
    [SerializeField]
    bool modeUnlock;
    [SerializeField]
    bool otherUnlock;
    [SerializeField]
    private int playerId;
    [SerializeField]
    private int cheerleaderId;
    [SerializeField]
    private int levelId;
    [SerializeField]
    private int modeId;

    [SerializeField]
    private int activationValueInt;
    [SerializeField]
    private float activationValueFloat;
    [SerializeField]
    private int activationValueProgressionInt;
    [SerializeField]
    private float activationValueProgressionFloat;

    // other custom 
    [SerializeField]
    bool consecutive;
    [SerializeField]
    bool count;
    [SerializeField]
    bool isProgressiveCount;
    [SerializeField]
    bool isProgressiveTime;
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
    private int cheerleaderRequiredToUseId;
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
    public int CheerleaderId { get => cheerleaderId; set => cheerleaderId = value; }

    // new achievement object
    public Achievement(int aid, int islockedValue)
    {
        achievementId = aid;
        if (islockedValue == 1)
        {
            IsLocked = true;
        }
        if (islockedValue == 0)
        {
            IsLocked = false;
        }
    }

    public void checkUnlockConditions(int playid, int cheerid, int lid, int mid, int activateValue, BasketBallStats basketBallStats)
    {
        bool condition1 = false;
        bool condition2 = false;
        bool condition3 = false;

        // if unlocks character and islocked
        if (IsLocked && characterUnlock)
        {
            if (count && !isProgressiveCount)
            {
                if (checkPlayerUnlockConditions(playerId, lid, mid, ref condition1, ref condition2, ref condition3)
                    && checkActivateValue(activateValue))
                {
                    isLocked = false;
                }
            }
            // other unlock : if requires count but is progressive counter
            if (count && isProgressiveCount)
            {
                if (checkPlayerUnlockConditions(playerId, lid, mid, ref condition1, ref condition2, ref condition3))            
                {
                    // if progressive activate value reached
                    if (checkActivateValueProgressive(activateValue))
                    {
                        isLocked = false;
                    }
                    // else, update progression
                    else
                    {
                        activationValueProgressionInt += activateValue;
                    }
                }
            }
        }
        // if unlocks cheerleader and islocked
        if (IsLocked && cheerleaderUnlock)
        {
            // cheerleader unlock :  if requires count but isnt progressive counter
            if (count && !isProgressiveCount)
            {
                if (checkCheerleaderUnlockConditions(cheerid, lid, mid, ref condition1, ref condition2, ref condition3)
                    && checkActivateValue(activateValue))
                {
                    IsLocked = false;
                }
            }
            // cheerleader unlock : if requires count but is progressive counter
            if (count && isProgressiveCount)
            {
                // if conditions met and value met :: unlock
                if (checkCheerleaderUnlockConditions(cheerid, lid, mid, ref condition1, ref condition2, ref condition3))
                {
                    // if progressive activate value reached
                    if (checkActivateValueProgressive(activateValue))
                    {
                        isLocked = false;
                    }
                    // else, update progression
                    else
                    {
                        activationValueProgressionInt += activateValue;
                    }
                }
            }
        }
        // if unlocks other (anything)
        if (IsLocked && otherUnlock)
        {
            // cheerleader unlock :  if requires count but isnt progressive counter
            if (count && !isProgressiveCount)
            {
                if (checkPlayerUnlockConditions(playerId, lid, mid, ref condition1, ref condition2, ref condition3)
                    && checkActivateValue(activateValue))
                {
                    isLocked = false;
                }
            }
            // other unlock : if requires count but is progressive counter
            if (count && isProgressiveCount)
            {
                // if conditions met
                if (checkPlayerUnlockConditions(playerId, lid, mid, ref condition1, ref condition2, ref condition3))
                {
                    // if progressive activate value reached
                    if (checkActivateValueProgressive(activateValue))
                    {
                        isLocked = false;
                    }
                    // else, update progression
                    else
                    {
                        activationValueProgressionInt += activateValue;
                    }
                }
            }
        }
        //// if achievement unlocked, display notification
        //if(isLocked == false)
        //{
        //    StartCoroutine(displayAchievementUnlockNotification(3));
        //}
    }

    public bool checkActivateValue(int activateValue)
    {
        bool activate = false;

        if (EQUAL_TO)
        {
            if (activateValue == activationValueInt)
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
            if (activateValue >= activationValueInt)
            {
                activate = true;
                activationValueProgressionInt += activateValue;
            }
            else
            {
                activate = false;
            }
        }

        return activate;
    }
    // for adding total count
    public bool checkActivateValueProgressive(int activateValue)
    {
        bool activate = false;
        int totalValue = activateValue + activationValueProgressionInt;

        if (LESS_THAN)
        {
            if (totalValue < activationValueInt)
            {
                activate = true;
            }
        }
        if (GREATER_THAN)
        {
            if (totalValue >= activationValueInt)
            {
                activate = true;
                activationValueProgressionInt += activateValue;
            }
            // if less than activation, add to progressive value
            if (totalValue < activationValueInt)
            {
                activate = false;
            }
        }
        return activate;
    }
    // for adding total time
    public bool checkActivateValueProgressive(float activateValue)
    {
        bool activate = false;
        float totalValue = activateValue + activationValueProgressionFloat;
        if (GREATER_THAN)
        {
            if (totalValue >= activationValueInt)
            {
                activate = true;
            }
        }
        return activate;
    }

    private bool checkPlayerUnlockConditions(int pid, int lid, int mid, ref bool condition1, ref bool condition2, ref bool condition3)
    {
        if (playerRequiredToUseId == 0)
        {
            condition1 = true;
        }
        if (levelRequiredToUseId == 0)
        {
            condition2 = true;
        }
        if (modeRequiredToUseId == 0)
        {
            condition3 = true;
        }
        // if requires specific player, this points to a pid
        if (pid == playerRequiredToUseId && !condition1 && characterUnlock)
        {
            condition1 = true;
        }
        // if requires specific cheerleader, this points to a cid
        if (pid == cheerleaderRequiredToUseId && !condition1 && characterUnlock)
        {
            condition1 = true;
        }

        if (lid == levelRequiredToUseId && !condition2)
        {
            condition2 = true;
        }
        if (mid == modeRequiredToUseId && !condition3)
        {
            condition3 = true;
        }
        if (condition1 && condition2 && condition3)
        {
            return true;
        }
        return false;
    }
    private bool checkCheerleaderUnlockConditions(int cid, int lid, int mid, ref bool condition1, ref bool condition2, ref bool condition3)
    {
        if (cheerleaderRequiredToUseId == 0)
        {
            condition1 = true;
        }
        if (levelRequiredToUseId == 0)
        {
            condition2 = true;
        }
        if (modeRequiredToUseId == 0)
        {
            condition3 = true;
        }
        if (cid == cheerleaderRequiredToUseId && !condition1 && cheerleaderUnlock)
        {
            condition1 = true;
        }
        if (lid == levelRequiredToUseId && !condition2)
        {
            condition2 = true;
        }
        if (mid == modeRequiredToUseId && !condition3)
        {
            condition3 = true;
        }
        if (condition1 && condition2 && condition3)
        {
            return true;
        }
        return false;
    }
}
