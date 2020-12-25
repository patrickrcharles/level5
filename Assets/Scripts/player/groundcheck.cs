using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public float initialHeight, finalHeight;
    private PlayerController playerController;

    [SerializeField]
    private BasketBallState basketBallState;

    private void Start()
    {
        playerController = GameLevelManager.instance.PlayerState;
        basketBallState = GameLevelManager.instance.Basketball.GetComponent<BasketBallState>();
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
            playerController.inAir = false;
            playerController.setPlayerAnim("jump", false);
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
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 11 && gameObject.transform.parent.CompareTag("Player"))
        {
            playerController.Grounded = false;
            playerController.inAir = true;
            playerController.setPlayerAnim("jump", true);
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