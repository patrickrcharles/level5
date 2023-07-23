using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public float initialHeight, finalHeight;
    private PlayerController playerController;
    [SerializeField]
    private AutoPlayerController autoPlayerController;

    [SerializeField]
    private BasketBallState basketBallState;

    private void Start()
    {
        playerController = GameLevelManager.instance.PlayerController1;
        autoPlayerController = GameLevelManager.instance.AutoPlayerController;
        basketBallState = BasketBallState.instance;
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
             playerController.CallBallToPlayer.Locked = false;
        }
        // later 11 is ground/terrain
        if (other.gameObject.layer == 11 && gameObject.transform.parent.CompareTag("autoPlayer"))
        {
            //initialHeight = _player.transform.position.y;
            //if (finalHeight - initialHeight > 1)
            //{
            //    //Debug.Log("fall distance : " + (finalHeight - initialHeight));
            //}

            autoPlayerController.Grounded = true;
            autoPlayerController.InAir = false;
            autoPlayerController.SetPlayerAnim("jump", false);

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

            basketBallState.Grounded = true;
            basketBallState.InAir = false;
            if (playerController != null)
            {
                playerController.CallBallToPlayer.Locked = false;
            }
            if (autoPlayerController != null)
            {
                autoPlayerController.CallBallToPlayer.Locked = false;
            }
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
        if (other.gameObject.layer == 11 && gameObject.transform.parent.CompareTag("autoPlayer"))
        {
            autoPlayerController.Grounded = false;
            autoPlayerController.InAir = true;
            autoPlayerController.SetPlayerAnim("jump", true);
        }

        if (other.gameObject.layer == 11 && gameObject.transform.parent.CompareTag("basketball"))
        {
            //initialHeight = _player.transform.position.y;
            //if (finalHeight - initialHeight > 1)
            //{
            //    //Debug.Log("fall distance : " + (finalHeight - initialHeight));
            //}

            basketBallState.Grounded = false;
            basketBallState.InAir = true;
        }
    }
}