using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float lifetime;
    public float projectileForce;
    public Vector3 projectileForceThrown;
    public Vector3 projectileForceSniper;
    new Rigidbody rigidbody;
    //PlayerController playerController;
    public bool thrownProjectile;
    public bool impactProjectile;
    [SerializeField]
    public bool sniperProjectile;
    public bool facingRight;
    [SerializeField]
    GameObject impactExplosionPrefab;
    [SerializeField]
    GameObject impactSniperGroundPrefab;
    [SerializeField]
    GameObject impactSniperPlayerPrefab;
    [SerializeField]
    AudioSource audioSource;

    void Start()
    {
        rigidbody = transform.root.GetComponent<Rigidbody>();
        audioSource = transform.root.GetComponent<AudioSource>();

        //playerController = GameLevelManager.instance.PlayerState;
        if (!thrownProjectile && !sniperProjectile)
        {
            applyForceToDirectionFacingProjectile(projectileForce);
            Destroy(transform.root.gameObject, lifetime);
        }
        if (thrownProjectile && !impactProjectile && !sniperProjectile)
        {
            applyForceToDirectionFacingProjectile(projectileForceThrown);
            impactExplosionPrefab = Resources.Load("Prefabs/projectile/projectile_impact_explosion") as GameObject;
            Destroy(transform.root.gameObject, lifetime);
        }
        if (!thrownProjectile && !impactProjectile && !sniperProjectile)
        {
            Destroy(transform.root.gameObject, lifetime);
        }
        if (sniperProjectile)
        {
            impactSniperGroundPrefab = Resources.Load("Prefabs/projectile/projectile_impact_ground") as GameObject;
            impactSniperPlayerPrefab = Resources.Load("Prefabs/projectile/projectile_impact_player") as GameObject;
            audioSource.clip = null;
            //Debug.Log("projectileForceSniper : " + projectileForceSniper );
            applyForceToDirectionVector(projectileForceSniper * 10);
            Destroy(transform.root.gameObject, lifetime);
        }
    }

    public void applyForceToDirectionFacingProjectile(float force)
    {
        // get direction facing
        if (facingRight)
        {
            rigidbody.AddForce(force, 0, 0, ForceMode.VelocityChange);
        }
        if (!facingRight)
        {
            rigidbody.AddForce(-force, 0, 0, ForceMode.VelocityChange);
        }
    }
    public void applyForceToDirectionFacingProjectile(Vector3 force)
    {
        // get direction facing
        if (facingRight)
        {
            rigidbody.AddForce(force.x, force.y, force.z, ForceMode.VelocityChange);
        }
        if (!facingRight)
        {
            rigidbody.AddForce(-force.x, force.y, force.z, ForceMode.VelocityChange);
        }
    }

    public void applyForceToDirectionVector(Vector3 force)
    {
        // remove rigidbody constraints
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.AddForce(force.x, force.y, force.z, ForceMode.VelocityChange);
    }

    void DestroyProjectile()
    {
        Destroy(transform.root.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("trigger this : " + gameObject.tag + "     other : " + other.gameObject.tag);
        if (thrownProjectile // thrown projectile          
            && !impactProjectile // does NOT explode on impact. bullet, laser, etc
            && !sniperProjectile
            && (other.gameObject.CompareTag("ground")
            || other.CompareTag("enemyHitbox")
            || other.CompareTag("playerHitbox"))) // if hit ground or enemy
        {
            // get position of impact to instantiate explosion object
            Vector3 transformAtImpact = other.gameObject.transform.position;
            Vector3 spawnPoint = new Vector3(transformAtImpact.x, 0, transformAtImpact.z);

            // explode object
            Instantiate(impactExplosionPrefab, spawnPoint, Quaternion.identity);

            DestroyProjectile();
        }
        if (sniperProjectile
            && impactProjectile
            && (other.gameObject.CompareTag("ground") || other.gameObject.layer == 11))
        {
            //Debug.Log("--- in ground");
            // get position of impact to instantiate explosion object
            //Vector3 transformAtImpact = other.gameObject.transform.position;

            //Vector3 transformAtImpact = transform.position;
            Vector3 transformAtImpact = SniperManager.instance.PlayerPosAtShoot;
            Vector3 spawnPoint = new Vector3(transformAtImpact.x, 0, transformAtImpact.z);

            //Debug.Log("trigger this : " + gameObject.tag + "     other : " + other.gameObject.name);
            //Debug.Log("--- transformAtImpact : " + transformAtImpact);
            //Debug.Log("--- spawnPoint : " + spawnPoint);

            Instantiate(impactSniperGroundPrefab, spawnPoint, Quaternion.identity);

            DestroyProjectile();
        }
        if (sniperProjectile
            && impactProjectile
            && (other.gameObject.CompareTag("enemyHitbox")
            || other.gameObject.CompareTag("playerHitbox")))
        {
            //Debug.Log("--- in player");
            // get position of impact to instantiate explosion object
            //Vector3 transformAtImpact = other.gameObject.transform.position;
            Vector3 transformAtImpact = SniperManager.instance.PlayerPosAtShoot;
            //Vector3 spawnPoint = new Vector3(transformAtImpact.x, 0, transformAtImpact.z);
            //Vector3 transformAtImpact = transform.position;
            Vector3 spawnPoint = new Vector3(transformAtImpact.x, 0, transformAtImpact.z);

            //Debug.Log("--- transformAtImpact : " + transformAtImpact);
            //Debug.Log("--- spawnPoint : " + spawnPoint);

            Instantiate(impactSniperPlayerPrefab, spawnPoint, Quaternion.identity);
            DestroyProjectile();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log("collision this : " + gameObject.tag + "     other : " + other.gameObject.name);
    }
}
