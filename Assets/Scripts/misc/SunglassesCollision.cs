using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunglassesCollision : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteRenderer spriteRenderer;
    bool sunglassesDisabled;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !sunglassesDisabled)
        {
            Debug.Log("collision : sunglasses + Player");
            sunglassesDisabled = true;
            TheyLiveManager.instance.TheyLiveEnabled = true;
            spriteRenderer.enabled = false;
        }
    }
}
