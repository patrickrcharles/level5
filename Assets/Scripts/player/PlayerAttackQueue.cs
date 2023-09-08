using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAttackQueue : MonoBehaviour
{
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
    [SerializeField]
    List<GameObject> bodyGuards;
    [SerializeField]
    List<GameObject> enemiesQueued;
    [SerializeField]
    bool bodyGuardEngaged;

    //public static PlayerAttackQueue instance;

    // Start is called before the first frame update
    void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    private void Start()
    {
        // if fighting only game mode/ hardcore OFF
        if (GameOptions.EnemiesOnlyEnabled && !GameOptions.hardcoreModeEnabled)
        {
            maxEnemiesQueued = 4;
        }
        // if fighting only game mode/ hardcore ON
        if (GameOptions.EnemiesOnlyEnabled && GameOptions.hardcoreModeEnabled)
        {
            maxEnemiesQueued = 8;
        }
        // if only hardcore ON
        if (!GameOptions.EnemiesOnlyEnabled && GameOptions.hardcoreModeEnabled)
        {
            maxEnemiesQueued = 6;
        }
        // if only hardcore ON
        if (GameOptions.battleRoyalEnabled)
        {
            maxEnemiesQueued = 20;
        }
        //default
        else
        {
            maxEnemiesQueued = 4;
        }

        //        //#if UNITY_ANDROID && !UNITY_EDITOR
        //#if UNITY_ANDROID && !UNITY_EDITOR
        //            maxNumberOfEnemies = maxNumberOfEnemies/2;
        //#endif

        // check if attack slot open
        if (currentEnemiesQueued < maxEnemiesQueued)
        {
            attackSlotOpen = true;
        }
        // if queue is full
        if (currentEnemiesQueued == maxEnemiesQueued)
        {
            attackQueueLocked = true;
        }
        else
        {
            attackQueueLocked = false;
        }

        // check for and add bodyguards to list
        bodyGuards = getBodyGuards();
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

    public IEnumerator removeEnemyFromAttackQueue(GameObject enemy, int attackPostionId)
    {
        //yield return new WaitForSeconds( 0.1f);
        yield return new WaitUntil(() => !LockAttackQueue);
        LockAttackQueue = true;

        PlayerAttackPosition playerAttackPosition = attackPositions[attackPostionId].GetComponent<PlayerAttackPosition>();

        playerAttackPosition.engaged = false;
        playerAttackPosition.enemyEngaged = null;

        currentEnemiesQueued--;
        attackSlotOpen = isAttackSlotOpen();

        enemiesQueued.Remove(enemy);
        LockAttackQueue = false;
    }

    public void removeEnemyFromQueue(GameObject enemy, int attackPostionId)
    {
        //yield return new WaitUntil( () => !LockAttackQueue);
        //yield return new WaitForSeconds(0.1f);
        LockAttackQueue = true;

        PlayerAttackPosition playerAttackPosition = attackPositions[attackPostionId].GetComponent<PlayerAttackPosition>();

        playerAttackPosition.engaged = false;
        playerAttackPosition.enemyEngaged = null;

        currentEnemiesQueued--;
        attackSlotOpen = isAttackSlotOpen();

        enemiesQueued.Remove(enemy);

        LockAttackQueue = false;
    }

    public IEnumerator RequestAddToQueue(GameObject enemy)
    {
        // wait for resource to unlock
        yield return new WaitUntil(() => LockAttackQueue == false);

        LockAttackQueue = true;

        // check attack slots again
        attackSlotOpen = isAttackSlotOpen();

        EnemyDetection enemyDetection = enemy.GetComponent<EnemyDetection>();

        if (attackSlotOpen && !enemyDetection.Attacking)
        {
            foreach (GameObject go in attackPositions)
            {
                PlayerAttackPosition attackPosition = go.GetComponent<PlayerAttackPosition>();
                //EnemyDetection enemyDetection = enemy.GetComponent<EnemyDetection>();

                // find open attack position
                // attack pos not engaged + enemy is not currently attacking
                if (!attackPosition.engaged
                    && !enemyDetection.Attacking
                    && currentEnemiesQueued < maxEnemiesQueued)
                {
                    attackPosition.engaged = true;
                    attackPosition.enemyEngaged = enemy;

                    enemyDetection.Attacking = true;
                    if (!bodyGuardEngaged)
                    {
                        enemyDetection.AttackPositionId = attackPosition.attackPositionId;
                        // this triggers 'pursue player' in enemy controller 
                        enemyDetection.PlayerSighted = true;

                        currentEnemiesQueued++;
                        // add enemy to queue
                        enemiesQueued.Add(enemy);
                        AttackSlotOpen = false;
                    }
                    else
                    {
                        bodyGuardEngaged = true;
                    }
                }
                if (GameOptions.battleRoyalEnabled
                    && attackPosition.engaged
                    && !enemyDetection.Attacking
                    && currentEnemiesQueued < maxEnemiesQueued)
                {
                    //attackPosition.engaged = true;
                    //attackPosition.enemyEngaged = enemy;

                    enemyDetection.Attacking = true;
                    if (!bodyGuardEngaged)
                    {
                        enemyDetection.AttackPositionId = attackPosition.attackPositionId;
                        // this triggers 'pursue player' in enemy controller 
                        enemyDetection.PlayerSighted = true;

                        currentEnemiesQueued++;
                        // add enemy to queue
                        enemiesQueued.Add(enemy);
                        AttackSlotOpen = false;
                    }
                    else
                    {
                        bodyGuardEngaged = true;
                    }
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

    List<GameObject> getBodyGuards()
    {
        List<GameObject> tempList = null;
        GameObject[] bodyGuardList;
        bodyGuardList = GameObject.FindGameObjectsWithTag("bodyGuard");

        tempList = bodyGuardList.ToList();

        return tempList;
    }

    public int CurrentEnemiesQueued { get => currentEnemiesQueued; set => currentEnemiesQueued = value; }
    public bool LockAttackQueue { get => attackQueueLocked; set => attackQueueLocked = value; }
    public bool AttackSlotOpen { get => attackSlotOpen; set => attackSlotOpen = value; }
    public GameObject[] AttackPositions { get => attackPositions; set => attackPositions = value; }
    public List<GameObject> EnemiesQueued { get => enemiesQueued; set => enemiesQueued = value; }
    public List<GameObject> BodyGuards { get => bodyGuards; set => bodyGuards = value; }
    public bool BodyGuardEngaged { get => bodyGuardEngaged; set => bodyGuardEngaged = value; }
}
