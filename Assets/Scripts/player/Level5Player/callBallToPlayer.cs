using Level5.Core.Match;
﻿using UnityEngine;

/// <summary>
/// Pulls a loose basketball back to its owning participant, and owns whether calling the ball is
/// available at all under this match's rules.
///
/// AUD-012 Phase 2b Slice 24: the rules behind that availability policy now arrive through
/// explicit composition (<see cref="BindMatchRules"/>, called by <c>SpawnCoordinator</c> for both
/// human and CPU participants) instead of a direct <c>MatchRuntime.Rules</c> read in
/// <see cref="Start"/>. The policy itself is unchanged and still lives here - composition supplies
/// the rules, it does not decide whether calling the ball is enabled.
/// </summary>
public class CallBallToPlayer : MonoBehaviour
{
    [SerializeField]
    internal float pullSpeed;
    [SerializeField]
    private Vector3 pullDirection;
    [SerializeField]
    private BasketBallState _basketBallState;
    [SerializeField]
    private bool locked;
    [SerializeField]
    public bool CallEnabled = true;

    /// <summary>
    /// The rules this match is being played under, bound once by composition. Not serialized: it is
    /// runtime-only, set after the component already exists, and <see cref="ResolvedMatchRules"/> is
    /// not itself <c>[Serializable]</c>.
    /// </summary>
    private ResolvedMatchRules matchRules;

    public bool Locked { get => locked; set => locked = value; }

    /// <summary>
    /// Explicit match-rules binding from <c>SpawnCoordinator</c>, called once during participant
    /// composition - from both the human and the CPU registration path - and therefore before Unity
    /// calls <see cref="Start"/>. Binding has no gameplay side effects of its own.
    ///
    /// Same bind-once shape <c>BasketBall</c>/<c>BasketBallState</c> already use, including the guard
    /// ordering: the already-bound branch is checked before the null-argument branch, so a null second
    /// call after a real bind reports "already bound" rather than "remaining unbound" - the original
    /// valid reference is kept either way, and the log should say so.
    /// </summary>
    public void BindMatchRules(ResolvedMatchRules rules)
    {
        if (matchRules != null)
        {
            Debug.LogError($"CallBallToPlayer on '{gameObject.name}' already has bound match rules; ignoring a second BindMatchRules call.", this);
            return;
        }

        if (rules == null)
        {
            Debug.LogError($"CallBallToPlayer on '{gameObject.name}' was bound with null match rules; remaining unbound.", this);
            return;
        }

        matchRules = rules;
    }

    private void Start()
    {
        Locked = false;
        pullSpeed = 2.3f;

        // A participant composed through SpawnCoordinator always has rules by now - both registration
        // paths bind during GameLevelManager.Awake, before any Start runs. Reaching here unbound is a
        // composition defect, so this one instance fails closed (no calling the ball) and says so,
        // rather than reaching back into MatchRuntime or disabling the whole player object.
        if (matchRules == null)
        {
            Debug.LogError($"CallBallToPlayer on '{gameObject.name}' reached Start() with no bound match rules; calling the ball is disabled for this participant.", this);
            CallEnabled = false;
            return;
        }

        if (matchRules.Hardcore && matchRules.EnemiesOnly)
        {
            CallEnabled = false;
            if (matchRules.IsThreePointContest || matchRules.IsFourPointContest || matchRules.IsSevenPointContest || matchRules.IsAllPointContest)
            {
                CallEnabled = true;
            }
        }
    }


    public void pullBallToPlayer(GameObject basketBall)
    {
        //if (!MatchRuntime.Rules.Hardcore)
        //{
            Rigidbody basketballRigidBody = basketBall.GetComponent<Rigidbody>();

            Vector3 tempDirection = basketballRigidBody.transform.position;
            pullDirection = transform.position - tempDirection;
            basketballRigidBody.linearVelocity = pullDirection * pullSpeed;
        //}
    }
    public void pullBallToPlayerAuto(GameObject basketBallAuto)
    {
        //if (!MatchRuntime.Rules.Hardcore)
        //{
            Rigidbody basketballRigidBody = basketBallAuto.GetComponent<Rigidbody>();

            Vector3 tempDirection = basketBallAuto.transform.position;
            pullDirection = transform.position - tempDirection;
            basketballRigidBody.linearVelocity = pullDirection * pullSpeed;
        //}
    }
}
