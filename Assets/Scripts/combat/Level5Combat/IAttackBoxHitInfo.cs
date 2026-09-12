/// <summary>
/// Neutral hit metadata for whatever attack box a collision hitbox just touched, without the
/// hitbox-processing side (<c>PlayerCollisions</c>/<c>AutoPlayerCollisions</c>, in <c>Level5.Player</c>)
/// depending on the concrete <c>EnemyAttackBox</c> (<c>Level5.Enemy</c>) or <c>PlayerAttackBox</c>
/// (<c>Level5.Player</c>) type of whichever one it was. <c>Level5.Enemy</c> has no reference to
/// <c>Level5.Player</c> and this must not create one in the other direction either - see
/// docs/systems-restructure-plan.md's collision cluster notes.
///
/// Exposes only what collision processing actually reads. <see cref="IsRake"/> and
/// <see cref="IsKilledOnIdle"/> only ever come from an <c>EnemyAttackBox</c> today -
/// <c>PlayerAttackBox</c> implements them as a constant <see langword="false"/>, not a duplicate
/// field, since a player-authored attack box has no such concept.
/// </summary>
public interface IAttackBoxHitInfo
{
    int AttackDamage { get; }

    bool KnockDownAttack { get; }

    bool DisintegrateAttack { get; }

    bool IsRake { get; }

    bool IsKilledOnIdle { get; }
}
