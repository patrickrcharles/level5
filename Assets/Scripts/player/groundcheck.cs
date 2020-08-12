using UnityEngine;

public class groundcheck : MonoBehaviour
{
    public float initialHeight, finalHeight;

    private GameObject _player;
    private PlayerController _playerState;

    [SerializeField]
    private BasketBallState basketBallState;


    // ReSharper disable once UnusedMember.Local
    private void Start()
    {
        _player = GameLevelManager.instance.Player;
        _playerState = GameLevelManager.instance.PlayerState;
        basketBallState = GameLevelManager.instance.Basketball.GetComponent<BasketBallState>();
    }


    public void OnTriggerStay(Collider other)
    {
        // later 11 is ground/terrain
        if (other.gameObject.layer == 11 && gameObject.transform.parent.CompareTag("Player"))
        {
            initialHeight = _player.transform.position.y;
            if (finalHeight - initialHeight > 1)
            {
                //Debug.Log("fall distance : " + (finalHeight - initialHeight));
            }

            _playerState.grounded = true;
            _playerState.inAir = false;
            //playerState.jump = false;
            _playerState.setPlayerAnim("jump", false);
        }
        if (other.gameObject.layer == 11 && gameObject.transform.parent.CompareTag("basketball"))
        {
            initialHeight = _player.transform.position.y;
            if (finalHeight - initialHeight > 1)
            {
                //Debug.Log("fall distance : " + (finalHeight - initialHeight));
            }

            basketBallState.Grounded = true;
            basketBallState.InAir = false;
            //playerState.jump = false;
            //.setPlayerAnim("jump", false);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 11 && gameObject.transform.parent.CompareTag("Player"))
        {
            // height when player exits ground (fall/jump etc.)
            //initialHeight = player.transform.position.y;
            _playerState.grounded = false;
            _playerState.inAir = true;
            _playerState.setPlayerAnim("jump", true);
        }

        if (other.gameObject.layer == 11 && gameObject.transform.parent.CompareTag("basketball"))
        {
            initialHeight = _player.transform.position.y;
            if (finalHeight - initialHeight > 1)
            {
                //Debug.Log("fall distance : " + (finalHeight - initialHeight));
            }

            basketBallState.Grounded = false;
            basketBallState.InAir = true;
            //playerState.jump = false;
            //.setPlayerAnim("jump", false);
        }
    }
}