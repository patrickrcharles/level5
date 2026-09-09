using System.Collections;
using UnityEngine;

/// <summary>
/// AUD-002: the player's damage/knockdown/lightning/shrink reaction coroutines, extracted from
/// <c>PlayerController</c> so this one cohesive concern - freeze the rigidbody, play a reaction
/// animation, wait for it, restore state - can be read without wading through movement, input, and
/// basketball code in the same 1000+ line file.
///
/// Deliberately a plain object, not a <c>MonoBehaviour</c>: the coroutines still run under
/// <c>PlayerController</c>'s own <c>StartCoroutine</c> exactly as before (a plain method can return an
/// <c>IEnumerator</c> for any <c>MonoBehaviour</c> to drive), so no prefab, component lifecycle, or
/// <c>GetComponent</c> wiring changes.
///
/// AUD-012 Phase 2b Slice 32: reaches its host through <see cref="IPlayerDamageReactionHost"/> rather
/// than the concrete <c>PlayerController</c>, which is what let this type move into
/// <c>Level5.Player</c> - <c>PlayerController</c> stays in <c>Assembly-CSharp</c> and every flag this
/// reads and writes (take-damage, knockdown, lock, avoided-knockdown, shrink, current animator state)
/// still lives there, because its own <c>Update()</c>/<c>FixedUpdate()</c> gate movement on the same
/// flags every frame. Splitting that coupling is a larger, separate decision than this slice; see
/// AUD-002/AUD-007 in docs/architecture-audit.md. The shrink reaction's camera lookup is reached the
/// same way, through <see cref="IPlayerDamageReactionHost.GetShrinkCamera"/> rather than naming
/// <c>CameraManager</c> directly.
///
/// No logic changed from the original methods - only where they live and how they reach the host's
/// state.
/// </summary>
public sealed class PlayerDamageReactions
{
    private readonly IPlayerDamageReactionHost host;

    public PlayerDamageReactions(IPlayerDamageReactionHost host)
    {
        this.host = host;
    }

    public IEnumerator PlayerTakeDamage(float takeDamageTime)
    {
        host.RigidBody.constraints =
            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        host.Anim.SetBool("takeDamage", true);
        host.Anim.Play("takeDamage");

        float startTime = Time.time;
        float endTime = startTime + takeDamageTime;
        yield return new WaitUntil(() => Time.time > endTime);
        host.Anim.SetBool("takeDamage", false);
        yield return new WaitUntil(() => host.CurrentState != host.TakeDamageStateHash);

        host.TakeDamage = false;
        host.KnockedDown = false;
        host.Locked = false;

        host.RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public IEnumerator PlayerFreezeForXSeconds(float time)
    {
        Debug.Log("freeze player");
        host.RigidBody.constraints =
            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        host.Anim.SetBool("takeDamage", true);
        host.Anim.Play("takeDamage");

        float startTime = Time.time;
        float endTime = startTime + time;
        yield return new WaitUntil(() => Time.time > endTime);
        host.Anim.SetBool("takeDamage", false);
        yield return new WaitUntil(() => host.CurrentState != host.TakeDamageStateHash);

        host.RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public IEnumerator PlayerKnockedDown(float knockDownTime)
    {
        host.RigidBody.constraints =
            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        host.Anim.SetBool("knockedDown", true);
        host.Anim.Play("knockedDown");

        float startTime = Time.time;
        float endTime = startTime + knockDownTime;
        yield return new WaitUntil(() => Time.time > endTime);
        host.Anim.SetBool("knockedDown", false);
        yield return new WaitUntil(() => host.CurrentState != host.KnockedDownStateHash);

        host.KnockedDown = false;
        host.TakeDamage = false;
        host.Locked = false;

        host.RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public IEnumerator PlayerDisintegrated()
    {
        host.Locked = true;
        host.RigidBody.constraints =
            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        host.Anim.Play("disintegrated");
        yield return new WaitUntil(() => host.CurrentState == host.DisintegratedStateHash);
        yield return new WaitForSeconds(2);
        host.MarkDead();
        host.RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public IEnumerator PlayerStruckByLightning()
    {
        host.RigidBody.constraints =
            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        host.Anim.Play("lightning");
        yield return new WaitUntil(() => host.CurrentState == host.LightningStateHash);
        yield return new WaitUntil(() => host.CurrentState != host.LightningStateHash);
        host.KnockedDown = true;
        host.RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public IEnumerator ShrinkPlayer()
    {
        host.IsShrunk = true;
        host.RigidBody.constraints =
            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        host.Anim.Play("lightning");
        yield return new WaitUntil(() => host.CurrentState == host.LightningStateHash);
        yield return new WaitUntil(() => host.CurrentState != host.LightningStateHash);
        host.KnockedDown = true;
        host.RigidBody.constraints = RigidbodyConstraints.FreezeRotation;

        Transform transform = host.ActorTransform;
        Vector3 originalScale = transform.localScale;
        Vector3 newScale = transform.localScale / 2;

        // AUD-054: the restore below used the literal 50 rather than the value captured here, so a
        // camera at any other FOV was permanently retuned by shrinking once.
        Camera shrinkCamera = host.GetShrinkCamera();
        float camFOV = shrinkCamera != null ? shrinkCamera.fieldOfView : 0f;

        transform.localScale = newScale;
        if (shrinkCamera != null)
        {
            shrinkCamera.fieldOfView = camFOV / 2;
        }

        yield return new WaitForSeconds(10);

        transform.localScale = originalScale;
        host.FacingRight = transform.localScale.x > 0 ? true : false;
        if (shrinkCamera != null)
        {
            shrinkCamera.fieldOfView = camFOV;
        }
        host.IsShrunk = false;
    }

    public void PlayerAvoidKnockedDown()
    {
        host.Anim.Play("knockedDown");
        host.AvoidedKnockDown = false;
        host.Locked = false;
    }
}
