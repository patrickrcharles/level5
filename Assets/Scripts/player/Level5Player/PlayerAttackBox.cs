
using UnityEngine;

public class PlayerAttackBox : MonoBehaviour, IAttackBoxHitInfo
{
    public int attackDamage;
    public bool knockDownAttack;
    public bool disintegrateAttack;
    public bool isProjectile;

    // AUD-012 Phase 2b: explicit implementation so collision processing (PlayerCollisions/
    // AutoPlayerCollisions) can read either this or an EnemyAttackBox through the same neutral
    // contract. A player-authored attack box has no rake/killed-on-idle concept, so those are a
    // constant false rather than a duplicate field.
    int IAttackBoxHitInfo.AttackDamage => attackDamage;
    bool IAttackBoxHitInfo.KnockDownAttack => knockDownAttack;
    bool IAttackBoxHitInfo.DisintegrateAttack => disintegrateAttack;
    bool IAttackBoxHitInfo.IsRake => false;
    bool IAttackBoxHitInfo.IsKilledOnIdle => false;
}
