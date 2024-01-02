using UnityEngine;

public class GroundCheckDefense : MonoBehaviour
{
    [SerializeField] AutoPlayerDefense autoPlayerDefense;

    private void Start()
    {
        autoPlayerDefense = GetComponentInParent<AutoPlayerDefense>();
    }

    public void OnTriggerStay(Collider other)
    {
        // later 11 is ground/terrain
        if (other.gameObject.layer == 11 && gameObject.transform.parent.CompareTag("autoPlayer"))
        {
            autoPlayerDefense.grounded = true;
            autoPlayerDefense.inAir = false;
            autoPlayerDefense.SetPlayerAnim("jump", false);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 11 && gameObject.transform.parent.CompareTag("autoPlayer"))
        {
            autoPlayerDefense.grounded = false;
            autoPlayerDefense.inAir = true;
            autoPlayerDefense.SetPlayerAnim("jump", true);
        }
    }
}