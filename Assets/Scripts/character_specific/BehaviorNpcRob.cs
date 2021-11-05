using UnityEngine;

public class BehaviorNpcRob : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;
    private AudioSource audioSource;
    private GameObject spriteObject;

    private void Start()
    {
        spriteObject = transform.GetComponentInChildren<SpriteRenderer>().gameObject;
        if (GameOptions.customCamera)
        {
            spriteObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        enemies = GameObject.FindGameObjectsWithTag("enemy");
        if (GameLevelManager.instance.Basketball != null)
        {
            audioSource = GameObject.FindWithTag("basketball").GetComponent<AudioSource>();
        }
    }

    private void LightningStrike()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                if (enemyController.SpriteRenderer.isVisible)
                {
                    StartCoroutine(enemyController.struckByLighning());
                }
            }
        }
    }
    private void DestroyRob()
    {
        Destroy(transform.root.gameObject);
    }

    public void playSfxCloudOfSmoke()
    {
        audioSource.PlayOneShot(SFXBB.instance.turnIntoBat);
    }
}
