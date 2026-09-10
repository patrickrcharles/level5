using System.Collections;

/// <summary>
/// AUD-012 Phase 2b Slice 33: the narrow slice of the sniper runtime that
/// <see cref="PlayerController"/>'s idle-sniper check needs, expressed as a contract so that check can
/// stop naming the concrete <c>SniperManager</c> (still <c>Assembly-CSharp</c>).
///
/// <c>SniperManager</c> implements this. Deliberately not a general sniper/weapon/projectile
/// abstraction - it exists only to name the two things <see cref="PlayerController"/> reads/writes on
/// the runtime it locks and hands a coroutine to. Sniper-enabled policy, idle timing, the 150-second
/// threshold, random-delay generation and coroutine execution all stay owned by
/// <see cref="PlayerController"/> - see its <c>checkIdleTimeForSniper</c>.
/// </summary>
public interface IPlayerIdleSniperRuntime
{
    bool Locked { get; set; }

    IEnumerator GetInstantKillRoutine(float shootDelay);
}
