using UnityEngine;

public class BehaviorNpcRob : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;
    private AudioSource audioSource;

    private void Start()
    {
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
