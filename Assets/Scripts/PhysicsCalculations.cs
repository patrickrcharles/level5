using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCalculations : MonoBehaviour {

    public float x; // = x-coordinate(horizontal)
    public float y; // y-coordinate(vertical)
    public float Vx; // x-velocity
    public float Vy; // y - veloctiy
    public float t; // time
    public float A; // initial angle
    public float V0; // intial velocity
    public float g; // acceleration due to gravity
    public float V0y; // initial y velocity
    public float V0x;  //
    

    void Awake()
    {
        Vx = V0 * Mathf.Cos(A);
        Vy = V0 * Mathf.Sin(A) - g * t;
        x = V0 * Mathf.Cos(A) * t;
        y = V0 * Mathf.Sin(A) * t - (1 / 2) * g * (t * t);
    }


    float maxHeightOfTrajectory(float v0y, float gravity)
    {
        return ((v0y * v0y) / gravity);
    }

    float timeOfTrajectory(float v0, float gravity)
    {
        return ((2 *v0)/gravity);
    }

    float rangeOfTrajectory(float v0, float angle, float gravity)
    {
        return (((v0*v0)*Mathf.Sin(2* angle))/gravity);
    }

    float GetAngle(float height, Vector3 startLocation, Vector3 endLocation)
    {
        float range = Mathf.Sqrt(Mathf.Pow(startLocation.x - endLocation.x, 2) + Mathf.Pow(startLocation.z - endLocation.z, 2));
        float offsetHeight = endLocation.y - startLocation.y;
        float g = -Physics.gravity.y;

        float verticalSpeed = Mathf.Sqrt(2 * g * height);
        float travelTime = Mathf.Sqrt(2 * (height - offsetHeight) / g) + Mathf.Sqrt(2 * height / g);
        float horizontalSpeed = range / travelTime;
        float velocity = Mathf.Sqrt(Mathf.Pow(verticalSpeed, 2) + Mathf.Pow(horizontalSpeed, 2));

        return -Mathf.Atan2(verticalSpeed / velocity, horizontalSpeed / velocity) + Mathf.PI;
    }

    Vector3 calculateThrowSpeed(Vector3 origin, Vector3 target, float timeToTarget)
    {
        // calculate vectors
        Vector3 toTarget = target - origin;
        Vector3 toTargetXZ = toTarget;
        toTargetXZ.y = 0;

        // calculate xz and y
        float y = toTarget.y;
        float xz = toTargetXZ.magnitude;

        // calculate starting speeds for xz and y. Physics forumulase deltaX = v0 * t + 1/2 * a * t * t
        // where a is "-gravity" but only on the y plane, and a is 0 in xz plane.
        // so xz = v0xz * t => v0xz = xz / t
        // and y = v0y * t - 1/2 * gravity * t * t => v0y * t = y + 1/2 * gravity * t * t => v0y = y / t + 1/2 * gravity * t
        float t = timeToTarget;
        float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
        float v0xz = xz / t;

        // create result vector for calculated starting speeds
        Vector3 result = toTargetXZ.normalized;        // get direction of xz but with magnitude 1
        result *= v0xz;                                // set magnitude of xz to v0xz (starting speed in xz plane)
        result.y = v0y;                                // set y to v0y (starting speed of y plane)

        return result;
    }

    /*
     * Simply call the function with the origin position, target position and how long 
     * you want the throw to take (which will affect the angle). Then apply the result 
     * on the object you want to throw with rigidbody.AddForce(throwSpeed, ForceMode.VelocityChange);
     * */
}
