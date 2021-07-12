using UnityEngine;

public class RacingGroundCheck : MonoBehaviour
{
    public float initialHeight, finalHeight;
    private VehicleRacingController playerController;


    private void Start()
    {
        playerController = RacingGameManager.instance.PlayerController;
        //basketBallState = GameLevelManager.instance.Basketball.GetComponent<BasketBallState>();
        //basketBallState = BasketBall.instance.BasketBallState;
    }


    public void OnTriggerStay(Collider other)
    {
        // later 11 is ground/terrain
        if (other.gameObject.layer == 11 && gameObject.transform.parent.CompareTag("Player"))
        {
            //initialHeight = _player.transform.position.y;
            //if (finalHeight - initialHeight > 1)
            //{
            //    //Debug.Log("fall distance : " + (finalHeight - initialHeight));
            //}

            playerController.Grounded = true;
            playerController.InAir = false;
            playerController.SetPlayerAnim("jump", false);

            //reset state flags
            //CallBallToPlayer.instance.Locked = false;
        }
        if (other.gameObject.layer == 11 && gameObject.transform.parent.CompareTag("basketball"))
        {
            //initialHeight = _player.transform.position.y;
            //if (finalHeight - initialHeight > 1)
            //{
            //    //Debug.Log("fall distance : " + (finalHeight - initialHeight));
            //}

            //basketBallState.Grounded = true;
            //basketBallState.InAir = false;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 11 && gameObject.transform.parent.CompareTag("Player"))
        {
            playerController.Grounded = false;
            playerController.InAir = true;
            playerController.SetPlayerAnim("jump", true);
        }

        //if (other.gameObject.layer == 11 && gameObject.transform.parent.CompareTag("basketball"))
        //{
        //    //initialHeight = _player.transform.position.y;
        //    //if (finalHeight - initialHeight > 1)
        //    //{
        //    //    //Debug.Log("fall distance : " + (finalHeight - initialHeight));
        //    //}

        //    //basketBallState.Grounded = false;
        //    //basketBallState.InAir = true;
        //}
    }
}