using UnityEngine;

public class EnemyAttackBox : MonoBehaviour, IAttackBoxHitInfo
{
    public bool knockDownAttack;
    public bool disintegrateAttack;
    public int attackDamage;
    public bool isRake;
    public bool isKilledOnIdle;

    // AUD-012 Phase 2b: explicit implementation so collision processing (PlayerCollisions/
    // AutoPlayerCollisions, Level5.Player) can read this through the neutral IAttackBoxHitInfo
    // contract instead of naming this concrete type - Level5.Enemy has no reference to Level5.Player
    // and this must not create one in the other direction. The fields above remain authoritative;
    // these are a live read-only view over them, not a duplicate.
    int IAttackBoxHitInfo.AttackDamage => attackDamage;
    bool IAttackBoxHitInfo.KnockDownAttack => knockDownAttack;
    bool IAttackBoxHitInfo.DisintegrateAttack => disintegrateAttack;
    bool IAttackBoxHitInfo.IsRake => isRake;
    bool IAttackBoxHitInfo.IsKilledOnIdle => isKilledOnIdle;
}
