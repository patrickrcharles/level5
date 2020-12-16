using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float lifetime;
    public float projectileForce;
    public Vector3 projectileForceThrown;
    new Rigidbody rigidbody;
    //PlayerController playerController;
    public bool thrownProjectile;
    public bool impactProjectile;
    public bool facingRight;
    [SerializeField]
    GameObject impactExplosionPrefab;

    void Awake()
    {
        rigidbody = transform.root.GetComponent<Rigidbody>();
        //playerController = GameLevelManager.instance.PlayerState;
        if (!thrownProjectile)
        {
            applyForceToDirectionFacingProjectile(projectileForce);
            Destroy(transform.root.gameObject, lifetime);
        }
        if (thrownProjectile && !impactProjectile)
        {
            applyForceToDirectionFacingProjectile(projectileForceThrown);
            impactExplosionPrefab = Resources.Load("Prefabs/projectile/projectile_impact_explosion") as GameObject;
            Destroy(transform.root.gameObject, lifetime);
        }
        if (!thrownProjectile && !impactProjectile)
        {
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
            rigidbody.AddForce(force.x, force.y,force.z, ForceMode.VelocityChange);
        }
        if (!facingRight)
        {
            rigidbody.AddForce(-force.x, force.y, force.z, ForceMode.VelocityChange);
        }
    }
    void DestroyProjectile()
    {
        Destroy(transform.root.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("collision between : " + gameObject.tag + " and " + other.tag);
        // destroy thrown projectile on impact
        if (thrownProjectile // thrown projectile          
            && !impactProjectile // does NOT explode on impact. bullet, laser, etc        
            && (other.CompareTag("ground") || other.CompareTag("enemyHitbox") || other.CompareTag("playerHitbox"))) // if hit ground or enemy
        {
            // get position of impact to instantiate explosion object
            Vector3 transformAtImpact = other.gameObject.transform.position;
            Vector3 spawnPoint = new Vector3(transformAtImpact.x, 0, transformAtImpact.z);

            // explode object
            Instantiate(impactExplosionPrefab, spawnPoint, Quaternion.identity);
            //DestroyProjectile();
        }
    }
}
