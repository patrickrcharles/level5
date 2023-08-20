
using UnityEngine;

public class BasketBallShotMadeCollision : MonoBehaviour
{
    [SerializeField]
    BasketBallShotMade basketBallShotMade;

    void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.CompareTag("basketball") || other.gameObject.CompareTag("basketballAuto"))/*&& (!playerState.hasBasketball || !autoPlayerState.hasBasketball) */
            //&& !isColliding
            && gameObject.name.Equals("basketBallMadeShot2")
            && basketBallShotMade.ShotMade1)
        {
            PlayerIdentifier pi = other.GetComponent<PlayerIdentifier>();
            basketBallShotMade.ShotMade2 = true;
            basketBallShotMade.shotMade(pi.gameStats, pi);
            pi.gameStats.calculateConsecutiveShot(pi.basketBallState);
            //todo:  deal with consecutive shots logic here
            // add to gamestats on basketball
            // define shot made/ missed logic
            // this is a made shot = true
            // save previous shot status variable, previousShotMade.
            // put consecutive shots logic in a function call
        }
    }
}

