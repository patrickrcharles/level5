using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackQueue : MonoBehaviour
{
    //public List<GameObject> attackPositionGameobjects;
    //public List<PlayerAttackPosition> playerAttackPositions;
    //const string attackPositionsTag = "playerAttackQueuePosition";

    //public List<GameObject> AttackPositionGameobjects { get => attackPositionGameobjects; set => attackPositionGameobjects = value; }
    //public List<PlayerAttackPosition> PlayerAttackPositions { get => playerAttackPositions; set => playerAttackPositions = value; }
    [SerializeField]
    bool attackSlotOpen;
    [SerializeField]
    bool attackQueueLocked = false;
    [SerializeField]
    int currentEnemiesQueued;
    [SerializeField]
    int maxEnemiesQueued;
    [SerializeField]
    GameObject[] attackPositions;

    public static PlayerAttackQueue instance;

    public int CurrentEnemiesQueued { get => currentEnemiesQueued; set => currentEnemiesQueued = value; }
    public bool LockAttackQueue { get => attackQueueLocked; set => attackQueueLocked = value; }
    public bool AttackSlotOpen { get => attackSlotOpen; set => attackSlotOpen = value; }
    public GameObject[] AttackPositions { get => attackPositions; set => attackPositions = value; }

    // Start is called before the first frame update
    void Awake()
    {
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
        // default set up
        if (maxEnemiesQueued == 0 && GameOptions.hardcoreModeEnabled)
        {
            maxEnemiesQueued = 4;
        }
        else
        {
            maxEnemiesQueued = 2;
        }

        if (currentEnemiesQueued < maxEnemiesQueued)
        {
            attackSlotOpen = true;
        }
        if (currentEnemiesQueued == maxEnemiesQueued)
        {
            attackQueueLocked = true;
        }
        else
        {
            attackQueueLocked = false;
        }

        getAttackPositions();
        // check for update status every 2 seconds
        // currently an issue with KillEnemy in enemy controller.
        // on kill, sometimes the attackSlotOpen is not set to true
        InvokeRepeating("updateAttackSlotStatus", 0, 2);
    }

    private void getAttackPositions()
    {
        attackPositions = GameObject.FindGameObjectsWithTag("playerAttackQueuePosition");
        int attackPosId = 0;
        foreach (GameObject go in attackPositions)
        {
            go.GetComponent<PlayerAttackPosition>().attackPositionId = attackPosId;
            attackPosId++;
        }
    }

    public IEnumerator removeEnemyFromAttackQueue(int attackPostionId)
    {
        //yield return new WaitForSeconds( 0.1f);
        yield return new WaitUntil( () => !LockAttackQueue);
        LockAttackQueue = true;

        PlayerAttackPosition playerAttackPosition = attackPositions[attackPostionId].GetComponent<PlayerAttackPosition>();

        playerAttackPosition.engaged = false;
        playerAttackPosition.enemyEngaged = null;

        currentEnemiesQueued--;
        attackSlotOpen = isAttackSlotOpen();

        LockAttackQueue = false;
    }

    public void removeEnemyFromQueue(int attackPostionId)
    {

        //yield return new WaitUntil( () => !LockAttackQueue);
        //yield return new WaitForSeconds(0.1f);
        LockAttackQueue = true;

        PlayerAttackPosition playerAttackPosition = attackPositions[attackPostionId].GetComponent<PlayerAttackPosition>();

        //Debug.Log("------------------------" + playerAttackPosition.enemyEngaged.name + " REMOVED from attack queue");

        playerAttackPosition.engaged = false;
        playerAttackPosition.enemyEngaged = null;

        currentEnemiesQueued--;
        attackSlotOpen = isAttackSlotOpen();

        LockAttackQueue = false;
    }

    public IEnumerator RequestAddToQueue(GameObject enemy)
    {      
        // wait for resource to unlock
        yield return new WaitUntil( ()=>LockAttackQueue == false);

        LockAttackQueue = true;

        // check attack slots again
        attackSlotOpen = isAttackSlotOpen();

        EnemyDetection enemyDetection = enemy.GetComponent<EnemyDetection>();

        if (attackSlotOpen && !enemyDetection.Attacking)
        {
            foreach(GameObject go in attackPositions)
            {
                PlayerAttackPosition attackPosition = go.GetComponent<PlayerAttackPosition>();
                //EnemyDetection enemyDetection = enemy.GetComponent<EnemyDetection>();

                // find open attack position
                // attack pos not engaged + enemy is not currently attacking
                if(!attackPosition.engaged 
                    && !enemyDetection.Attacking
                    && currentEnemiesQueued < maxEnemiesQueued)
                {
                    attackPosition.engaged = true;
                    attackPosition.enemyEngaged = enemy;

                    enemyDetection.Attacking = true;
                    enemyDetection.AttackPositionId = attackPosition.attackPositionId;
                    // this triggers 'pursue player' in enemy controller 
                    enemyDetection.PlayerSighted = true;

                    currentEnemiesQueued++;
                    AttackSlotOpen = false;

                    //Debug.Log("++++++++++++++++++++++++" + enemy.name + " added to attack queue");
                }
            }
        }
        LockAttackQueue = false;
    }

    bool isAttackSlotOpen()
    {
        if (currentEnemiesQueued < maxEnemiesQueued)
        {
            attackSlotOpen = true;
        }
        if (currentEnemiesQueued == maxEnemiesQueued)
        {
            attackSlotOpen = true;
        }

        return attackSlotOpen;
    }

    void updateAttackSlotStatus()
    {
        if (!attackQueueLocked)
        {
            if (currentEnemiesQueued < maxEnemiesQueued)
            {
                attackSlotOpen = true;
            }
            if (currentEnemiesQueued == maxEnemiesQueued)
            {
                attackSlotOpen = true;
            }
        }
    }
}
