
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
            basketBallShotMade.shotMade(pi.GetComponent<GameStats>(), pi);
        }
    }
}

