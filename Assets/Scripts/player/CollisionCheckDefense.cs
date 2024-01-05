using UnityEngine;

public class CollisionCheckDefense : MonoBehaviour
{
    [SerializeField] AutoPlayerDefense autoPlayerDefense;
    [SerializeField] bool isLocked;

    private void Start()
    {
        autoPlayerDefense = GetComponentInParent<AutoPlayerDefense>();
    }

    private void Update()
    {
        if (autoPlayerDefense.grounded)
        {
            isLocked = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("basketball") 
            && gameObject.CompareTag("cpuDefenseBlockBox")
            && !isLocked
            && autoPlayerDefense.playerIdentifier.playerController.InAir)
        {
            isLocked = true;
            autoPlayerDefense.blockedShots++;
            Debug.Log("shot blocked");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("basketball") 
            && gameObject.CompareTag("cpuDefenseBlockBox"))
        {
            isLocked = false;
        }
    }
}