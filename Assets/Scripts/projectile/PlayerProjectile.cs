using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float lifetime;
    public float projectileForce;
    public Vector3 projectileForceThrown;
    new Rigidbody rigidbody;
    PlayerController playerController;
    public bool thrownProjectile;
    public bool impactProjectile;

    GameObject impactExplosionPrefab;

    void Start()
    {
        rigidbody = transform.root.GetComponent<Rigidbody>();
        playerController = GameLevelManager.instance.PlayerState;
        if (!thrownProjectile)
        {
            applyForceToDirectionFacingProjectile(projectileForce);
            Destroy(transform.root.gameObject, lifetime);
        }
        if (thrownProjectile && !impactProjectile)
        {
            applyForceToDirectionFacingProjectile(projectileForceThrown);
            impactExplosionPrefab = Resources.Load("Prefabs/projectile/projectile_impact_explosion") as GameObject;
            //Destroy(transform.root.gameObject, lifetime);
        }
        if (!thrownProjectile && impactProjectile)
        {
            Destroy(transform.root.gameObject, lifetime);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("collision between : " + gameObject.name + " and " + other.name);
    //    if (other.CompareTag("enemyHitbox"))
    //    {
    //        Debug.Log("destroy " + transform.root.gameObject.name);
    //        Destroy(transform.root.gameObject);
    //        //Debug.Log("destroy " + transform.root.gameObject.name);
    //    }
    //}

    public void applyForceToDirectionFacingProjectile(float force)
    {
        // get direction facing
        if (playerController.facingRight)
        {
            Debug.Log(" shoot right");
            rigidbody.AddForce(force, 0, 0, ForceMode.VelocityChange);
        }
        if (!playerController.facingRight)
        {
            Debug.Log(" shoot left");
            rigidbody.AddForce(-force, 0, 0, ForceMode.VelocityChange);
        }
    }
    public void applyForceToDirectionFacingProjectile(Vector3 force)
    {
        // get direction facing
        if (playerController.facingRight)
        {
            rigidbody.AddForce(force.x, force.y,force.z, ForceMode.VelocityChange);
        }
        if (!playerController.facingRight)
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
        // destroy thrown projectile on impact
        if (thrownProjectile 
            && !impactProjectile
            && (other.CompareTag("ground") || other.CompareTag("enemyHitbox")))
        {
            Debug.Log("collision between : " + gameObject.name + " and " + other.name);
            Vector3 transformAtImpact = gameObject.transform.position;
            Vector3 spawnPoint = new Vector3(transformAtImpact.x, 0, transformAtImpact.z);
            // explode object
            //Debug.Log("projectile should explode");
            //Debug.Log("instantiate fire object");
            Instantiate(impactExplosionPrefab, spawnPoint, Quaternion.identity);
            DestroyProjectile();
        }
    }
}
