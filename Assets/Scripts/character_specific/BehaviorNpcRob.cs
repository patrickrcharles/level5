using UnityEngine;

public class BehaviorNpcRob : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;
    // Start is called before the first frame update
    [SerializeField]
    Animator anim;

    private AudioSource audioSource;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        enemies = GameObject.FindGameObjectsWithTag("enemy");
        if (GameLevelManager.instance.Basketball != null)
        {
            audioSource = GameObject.FindWithTag("basketball").GetComponent<AudioSource>();
        }
    }

    private void LightningStrike()
    {
        //enemies = GameObject.FindGameObjectsWithTag("enemy");
        //Debug.Log("tets lighting animation");
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            //enemyController.playAnimation("lightning");
            //enemyController.playAnimation("lightning");
            //enemyController.playAnimation("enemy_generic_lightning");
            if (enemyController.SpriteRenderer.isVisible)
            {
                //StartCoroutine(enemyController.struckByLighning());
                StartCoroutine(enemyController.struckByLighning());
            }
            //enemyController.GetComponentInChildren<Animator>().Play("enemy_generic_lightning");
            //Debug.Log("test lighting animation");
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
