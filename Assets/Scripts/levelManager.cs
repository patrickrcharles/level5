using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelManager : MonoBehaviour {

    public float xMinEnemy, xMaxEnemy, zMinEnemy, zMaxEnemy, yMinEnemy, yMaxEnemy;
    public float xMinPlayer, xMaxPlayer, zMinPlayer, zMaxPlayer, yMinPlayer, yMaxPlayer;
    public float navMeshOffset128, navMeshOffset192, navMeshOffset256, navMeshOffset1024;
    public float lineOfFireRange;
    public float attackStartDelay;

    public float attackRange;
    public float attackShootRange;

    public float bummedOutTime;


    public List<Transform> spawnPoints;
    public static levelManager instance;

    public float levelTimeScale;

    private void Awake()
    {
        //Debug.Log("levelManger - Awake()");
        instance = this;
    }

    private void Start()
    {
        Time.timeScale = levelTimeScale;
    }

}
