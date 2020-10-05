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
    private string experienceGained;

    [SerializeField]
    public bool characterUnlock;
    [SerializeField]
    public bool cheerleaderUnlock;
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

    public int AchievementId { get => achievementId; set => achievementId = value; }
    public string AchievementName { get => achievementName; set => achievementName = value; }
    public string AchievementDescription { get => achievementDescription; set => achievementDescription = value; }
    public string ExperienceGained { get => experienceGained; set => experienceGained = value; }
    public bool CharacterUnlock { get => characterUnlock; set => characterUnlock = value; }
    public bool CheerleaderUnlock { get => cheerleaderUnlock; set => cheerleaderUnlock = value; }
    public bool LevelUnlock { get => levelUnlock; set => levelUnlock = value; }
    public bool ModeUnlock { get => modeUnlock; set => modeUnlock = value; }
    public bool OtherUnlock { get => otherUnlock; set => otherUnlock = value; }
    public int PlayerId { get => playerId; set => playerId = value; }
    public int CheerleaderId { get => cheerleaderId; set => cheerleaderId = value; }
    public int LevelId { get => levelId; set => levelId = value; }
    public int ModeId { get => modeId; set => modeId = value; }
    public int ActivationValueInt { get => activationValueInt; set => activationValueInt = value; }
    public float ActivationValueFloat { get => activationValueFloat; set => activationValueFloat = value; }
    public int ActivationValueProgressionInt { get => activationValueProgressionInt; set => activationValueProgressionInt = value; }
    public float ActivationValueProgressionFloat { get => activationValueProgressionFloat; set => activationValueProgressionFloat = value; }
    public bool Consecutive { get => consecutive; set => consecutive = value; }
    public bool Count { get => count; set => count = value; }
    public bool IsProgressiveCount { get => isProgressiveCount; set => isProgressiveCount = value; }
    public bool IsProgressiveTime { get => isProgressiveTime; set => isProgressiveTime = value; }
    public bool Time { get => time; set => time = value; }
    public bool Three { get => three; set => three = value; }
    public bool Four { get => four; set => four = value; }
    public bool Seven { get => seven; set => seven = value; }
    public int PlayerRequiredToUseId { get => playerRequiredToUseId; set => playerRequiredToUseId = value; }
    public int CheerleaderRequiredToUseId { get => cheerleaderRequiredToUseId; set => cheerleaderRequiredToUseId = value; }
    public int LevelRequiredToUseId { get => levelRequiredToUseId; set => levelRequiredToUseId = value; }
    public int ModeRequiredToUseId { get => modeRequiredToUseId; set => modeRequiredToUseId = value; }
    public bool IsLocked { get => isLocked; set => isLocked = value; }

    // new achievement object
    //public Achievement(int aid, string name, string description, int activateInt, int progressValue, int islockedValue)
    //{
    //    achievementId = aid;
    //    activationValueInt = activateInt;
    //    activationValueProgressionInt = progressValue;
    //    achievementName = name;
    //    achievementDescription = description;

    //    if (islockedValue == 1)
    //    {
    //        IsLocked = true;
    //    }
    //    if (islockedValue == 0)
    //    {
    //        IsLocked = false;
    //    }
    //}
    public Achievement() { }

    public void checkUnlockConditions(int playid, int cheerid, int lid, int mid, int activateValue)
    {
        bool condition1 = false;
        bool condition2 = false;
        bool condition3 = false;

        Debug.Log("checkUnlockConditions(  " + playid + ", " + cheerid + " , " + lid + " , " + mid + " , " + activateValue);
        Debug.Log("     characterUnlock : " + characterUnlock + "  cheerUnlock : " + cheerleaderUnlock);
        // if unlocks character and islocked
        if (IsLocked && characterUnlock)
        {
            if (count && !isProgressiveCount)
            {
                if (checkPlayerUnlockConditions(playid, lid, mid, ref condition1, ref condition2, ref condition3)
                    && checkActivateValue(activateValue))
                {
                    isLocked = false;
                }
            }
            // other unlock : if requires count but is progressive counter
            if (count && isProgressiveCount)
            {
                if (checkPlayerUnlockConditions(playid, lid, mid, ref condition1, ref condition2, ref condition3))
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
            Debug.Log("         cheerleader unlock  ");
            // cheerleader unlock :  if requires count but isnt progressive counter
            if (count && !isProgressiveCount)
            {
                Debug.Log("         if (count && !isProgressiveCount)");
                if (checkCheerleaderUnlockConditions(cheerid, lid, mid, ref condition1, ref condition2, ref condition3)
                    && checkActivateValue(activateValue))
                {
                    Debug.Log("         if (checkCheerleaderUnlockConditions(cheerid, lid, mid, ref condition1, ref condition2, ref condition3) && checkActivateValue(activateValue))");
                    IsLocked = false;
                }
            }
            // cheerleader unlock : if requires count but is progressive counter
            if (count && isProgressiveCount)
            {
                Debug.Log("         if (count && isProgressiveCount)");
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

        Debug.Log("activatevalue : " + activateValue);
        Debug.Log("totalValue : " + totalValue);
        if (LESS_THAN)
        {
            if (totalValue < activationValueInt)
            {
                activate = true;
                Debug.Log("return true");
            }
        }
        if (GREATER_THAN)
        {
            if (totalValue >= activationValueInt)
            {
                activate = true;
                activationValueProgressionInt += activateValue;
                Debug.Log("return true");
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
                Debug.Log("return true");
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
