using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackQueue : MonoBehaviour
{
    public List<GameObject> attackPositionGameobjects;
    public List<PlayerAttackPosition> playerAttackPositions;
    const string attackPositionsTag = "playerAttackQueuePosition";

    public List<GameObject> AttackPositionGameobjects { get => attackPositionGameobjects; set => attackPositionGameobjects = value; }
    public List<PlayerAttackPosition> PlayerAttackPositions { get => playerAttackPositions; set => playerAttackPositions = value; }

    // Start is called before the first frame update
    void Start()
    {
        GameObject temp = transform.Find("attackPositions").gameObject;
        foreach (Transform t in temp.transform)
        {
            if(t.CompareTag(attackPositionsTag))
            {
                AttackPositionGameobjects.Add(t.gameObject);
                PlayerAttackPositions.Add(t.gameObject.GetComponent<PlayerAttackPosition>());
            }
        }
    }

    public bool addEnemyToQueue( GameObject enemy, bool inAttackQueue)
    {
        foreach(PlayerAttackPosition pos in playerAttackPositions)
        {
            if (!pos.enemyEngaged && !inAttackQueue)
            {
                Debug.Log(enemy.name + " added to enemy queue");
                pos.engaged = true;
                pos.enemyEngaged = enemy;
                return true;
            }
        }
        return false;
    }
    public bool removeEnemyToQueue(GameObject enemy)
    {
        foreach (PlayerAttackPosition pos in playerAttackPositions)
        {
            if (enemy.name.Equals(pos.enemyEngaged.name))
            {
                Debug.Log(enemy.name + " removed from enemy queue");
                pos.engaged = false;
                pos.enemyEngaged = null;
                return true;
            }
        }
        return false;
    }
}
