using UnityEngine;

public class groundcheck : MonoBehaviour
{
    public float initialHeight, finalHeight;

    [SerializeField] private GameObject _player;

    private playercontrollerscript _playerState;


    // ReSharper disable once UnusedMember.Local
    private void Start()
    {
        _player = GameLevelManager.instance.Player;
        _playerState = GameLevelManager.instance.PlayerState;
    }


    public void OnTriggerStay(Collider other)
    {
        // later 11 is ground/terrain
        if (other.gameObject.layer == 11)
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
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 11) //ground layer
        {
            // height when player exits ground (fall/jump etc.)
            //initialHeight = player.transform.position.y;
            _playerState.grounded = false;
            _playerState.inAir = true;
            _playerState.setPlayerAnim("jump", true);
        }
    }
}