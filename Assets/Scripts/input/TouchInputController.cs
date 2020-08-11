
using UnityEngine;
using TouchPhase = UnityEngine.TouchPhase;

public class TouchInputController : MonoBehaviour {

	private Vector2 startTouchPosition, endTouchPosition;
	private bool jumpAllowed = false;
	private bool shootAllowed = false;

	float swipeUpTolerance;
	float swipeDistance;

    public void Awake()
    {
		swipeUpTolerance = Screen.height / 7;
    }

    // Use this for initialization
    // Update is called once per frame
    public void FixedUpdate() 
	{
		SwipeCheck();
		if (jumpAllowed)
		{
			GameLevelManager.Instance.PlayerState.TouchControlJump();
			jumpAllowed = false;
		}

		tapCheck();
		if (shootAllowed)
		{
			GameLevelManager.Instance.PlayerState.touchControlShoot();
			shootAllowed = false;
		}
	}

	private void SwipeCheck()
	{
		// get initial touch position
		if (Input.touchCount > 0 
			&& Input.GetTouch(0).phase == TouchPhase.Began)
		{
			startTouchPosition = Input.GetTouch(0).position;
		}

		// if swipe finished
		if (Input.touchCount > 0 
			&& Input.GetTouch(0).phase == TouchPhase.Ended) 
		{
			endTouchPosition = Input.GetTouch(0).position;
			swipeDistance = endTouchPosition.y - startTouchPosition.y;

			if (swipeDistance > swipeUpTolerance
				&& GameLevelManager.Instance.PlayerState.grounded)
			{
				jumpAllowed = true;
			}
		}
	}

	private void tapCheck()
	{
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began )
			{
				shootAllowed = true;
			}
		}
	}
}
