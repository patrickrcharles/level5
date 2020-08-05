using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyConnector : MonoBehaviour
{
    [SerializeField]
    protected Joystick joystick;
    [SerializeField]
    protected JoyButton joyButton;

    // Start is called before the first frame update
    void Start()
    {
        joystick = FindObjectOfType<Joystick>();
        joyButton = FindObjectOfType<JoyButton>();

    }

    // Update is called once per frame
    void Update()
    {
        var rigidbody = GameLevelManager.Instance.PlayerState.RigidBody;
        rigidbody.velocity = new Vector3(joystick.Horizontal * 100f,
                    rigidbody.velocity.y,
                    joystick.Vertical * 100);
    }
}
