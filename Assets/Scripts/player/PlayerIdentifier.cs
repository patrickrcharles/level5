using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdentifier : MonoBehaviour
{
    public int pid;
    public int bid;
    public int bsid;
    public bool isCpu;
    [SerializeField]
    public GameObject player;
    [SerializeField]
    public GameObject basketball;
    [SerializeField]
    public GameObject autoPlayer;
    [SerializeField]
    public GameObject autoBasketball;
    public PlayerController playerController;
    public AutoPlayerController autoPlayerController;
    public BasketBall basketBallController;
    public BasketBallAuto basketBallAutoController;
    public BasketBallState basketBallState;


    public void setIds(int pid, int bid, int bsid, bool isCpu)
    {
        this.pid = pid;
        this.bid = bid;
        this.bsid = bsid;
        this.isCpu = isCpu;
    }
    public void setPlayer(GameObject player)
    {
        this.player = player;
        playerController = player.GetComponent<PlayerController>();
    }
    public void setAutoPlayer(GameObject autoPlayer)
    {
        this.autoPlayer = autoPlayer;
        autoPlayerController = autoPlayer.GetComponent<AutoPlayerController>();
    }
    public void setBasketball(GameObject basketball)
    {
        this.basketball = basketball;
        basketBallController = basketball.GetComponent<BasketBall>();
        basketBallState = basketball.GetComponent<BasketBallState>();
    }
    public void setAutoBasketball(GameObject autoBasketball)
    {
        this.autoBasketball = autoBasketball;
        basketBallAutoController = autoBasketball.GetComponent<BasketBallAuto>();
        basketBallState = autoBasketball.GetComponent<BasketBallState>();
    }
}
