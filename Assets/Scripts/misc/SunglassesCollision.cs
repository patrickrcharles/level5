using UnityEngine;

public class SunglassesCollision : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteRenderer spriteRenderer;
    bool sunglassesDisabled;
    [SerializeField]
    GameObject CameraPostProcessing;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        CameraPostProcessing.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !sunglassesDisabled)
        {
            sunglassesDisabled = true;
            TheyLiveManager.instance.TheyLiveEnabled = true;
            spriteRenderer.enabled = false;
            CameraPostProcessing.SetActive(true);
        }
    }
}
