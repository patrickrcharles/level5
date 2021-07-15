using UnityEngine;

public class RacingGroundCheck : MonoBehaviour
{
    public float initialHeight, finalHeight;
    [SerializeField]
    private RacingVehicleController playerController;


    private void Start()
    {
        playerController = RacingGameManager.instance.PlayerController;
        //basketBallState = GameLevelManager.instance.Basketball.GetComponent<BasketBallState>();
        //basketBallState = BasketBall.instance.BasketBallState;
    }


    public void OnTriggerStay(Collider other)
    {
        //Debug.Log("collision --- other.gameObject.layer : " + other.gameObject.layer + "     gameObject.transform.parent.tag : " + gameObject.transform.parent.tag);
        // later 11 is ground/terrain
        if (other.gameObject.layer == 11 && gameObject.transform.parent.CompareTag("Player"))
        {

            playerController.Grounded = true;
            playerController.InAir = false;
            playerController.SetPlayerAnim("jump", false);

        }
        if (other.gameObject.CompareTag("grindable") && gameObject.transform.parent.CompareTag("Player"))
        {

            playerController.IsGrinding = true;
            playerController.Grounded = false;
            playerController.InAir = false;
            playerController.PlayAnim("knockedDown");

        }
    }

    public void OnTriggerExit(Collider other)
    {
        //Debug.Log("collision --- other.gameObject.layer : " + other.gameObject.layer + "     gameObject.transform.parent.tag : " + gameObject.transform.parent.tag);
        if (other.gameObject.layer == 11 && gameObject.transform.parent.CompareTag("Player"))
        {
            playerController.Grounded = false;
            playerController.InAir = true;
            playerController.SetPlayerAnim("jump", true);
        }
        if (other.gameObject.CompareTag("grindable") && gameObject.transform.parent.CompareTag("Player"))
        {
            playerController.IsGrinding = false;
            playerController.Grounded = false;
            playerController.InAir = true;
            playerController.SetPlayerAnim("jump", true);

        }
    }
}