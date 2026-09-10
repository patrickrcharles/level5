using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// AUD-012 Phase 2b Slice 35: <see cref="PlayerDunk"/> now resolves its controller through
/// <see cref="IPlayerDunkHost"/> and its human basketball association through
/// <see cref="IPlayerControllerParticipantState"/>, instead of the concrete <c>PlayerController</c>/
/// <c>PlayerIdentifier</c>. These tests drive <see cref="PlayerDunk"/> directly against a single fake
/// component implementing both contracts - no real <c>PlayerController</c>/<c>PlayerIdentifier</c>,
/// scene or Play Mode required - proving the changed seams: host/participant resolution, the
/// rim-relative left/right decision, launch's host locking/Rigidbody/animation calls, and
/// <see cref="PlayerDunk.TriggerDunkSequence"/>'s freeze/animation/state/ball-reset sequence. Mirrors
/// the fake-host shape <see cref="Level5PlayerDamageReactionsTests"/> already established, adapted for
/// <see cref="PlayerDunk"/> resolving its host via <c>GetComponent</c> in <c>Start()</c> rather than
/// constructor injection.
///
/// <c>BasketBall</c>/<c>BasketBallState</c> are real components (already-legal <c>Level5.Basketball</c>
/// dependencies, unchanged by this slice) with the minimum state <c>playerDunk()</c>/
/// <c>TriggerDunkSequence()</c> touch - their own <c>Start()</c> is deliberately never invoked, so no
/// <c>GameStats</c>/<c>Animator</c> rig is needed; per the slice's non-goals this does not broaden into
/// scoring/shooting-system coverage.
/// </summary>
public class Level5PlayerDunkSeamTests
{
    private readonly List<GameObject> spawned = new List<GameObject>();

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject go in spawned)
        {
            if (go != null)
            {
                Object.DestroyImmediate(go);
            }
        }

        spawned.Clear();
    }

    private GameObject Spawn(string name)
    {
        GameObject go = new GameObject(name);
        spawned.Add(go);
        return go;
    }

    private static void InvokePrivate(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, $"{target.GetType().Name} must declare {methodName}()");
        method.Invoke(target, null);
    }

    private static object GetPrivateField(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"{target.GetType().Name} must declare a field named '{fieldName}'");
        return field.GetValue(target);
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"{target.GetType().Name} must declare a field named '{fieldName}'");
        field.SetValue(target, value);
    }

    /// <summary>
    /// An actor carrying <see cref="FakeDunkHost"/> (satisfying both <see cref="IPlayerDunkHost"/> and
    /// <see cref="IPlayerControllerParticipantState"/>) and <see cref="PlayerDunk"/>, plus a basketball
    /// object carrying real <c>BasketBall</c>/<c>BasketBallState</c> components, after <c>Start()</c>.
    /// </summary>
    private sealed class DunkFixture
    {
        public PlayerDunk Dunk;
        public FakeDunkHost Host;
        public BasketBall BasketBall;
        public BasketBallState BasketBallState;
    }

    /// <summary>
    /// Builds a <see cref="DunkFixture"/> and runs <c>PlayerDunk.Start()</c>, expecting the two "scene
    /// object not found" errors <c>SceneObjects.Find</c> logs for the dunk-position markers, which no
    /// test scene provides - that only disables <c>PlayerCanDunk</c>, not host/participant/basketball
    /// resolution.
    /// </summary>
    private DunkFixture BuildAndStart()
    {
        GameObject actorGo = Spawn("dunk-actor");
        FakeDunkHost host = actorGo.AddComponent<FakeDunkHost>();
        PlayerDunk dunk = actorGo.AddComponent<PlayerDunk>();

        GameObject basketballGo = Spawn("basketball");
        BasketBall basketBall = basketballGo.AddComponent<BasketBall>();
        BasketBallState basketBallState = basketballGo.AddComponent<BasketBallState>();
        // BasketBall.Start() is deliberately never invoked (see the class doc comment) - its own
        // private basketBallState field, which updateBasketBallStateShotTypeOnShoot reads
        // unconditionally, is wired directly so playerDunk()'s shot-type bookkeeping call does not NRE.
        SetPrivateField(basketBall, "basketBallState", basketBallState);
        host.BasketballObject = basketballGo;

        LogAssert.Expect(LogType.Error, new Regex("dunk_position_left"));
        LogAssert.Expect(LogType.Error, new Regex("dunk_position_right"));
        InvokePrivate(dunk, "Start");

        return new DunkFixture { Dunk = dunk, Host = host, BasketBall = basketBall, BasketBallState = basketBallState };
    }

    // ==================== host/participant resolution ====================

    [Test]
    public void StartResolvesIPlayerDunkHostRatherThanAConcretePlayerController()
    {
        DunkFixture fixture = BuildAndStart();

        object resolvedHost = GetPrivateField(fixture.Dunk, "playerHost");

        Assert.That(resolvedHost, Is.SameAs(fixture.Host));
        Assert.That(resolvedHost, Is.Not.InstanceOf<PlayerController>());
    }

    [Test]
    public void StartResolvesTheSameBasketballAndBasketBallStateTheParticipantExposes()
    {
        DunkFixture fixture = BuildAndStart();

        Assert.That(GetPrivateField(fixture.Dunk, "basketBall"), Is.SameAs(fixture.BasketBall));
        Assert.That(GetPrivateField(fixture.Dunk, "basketBallState"), Is.SameAs(fixture.BasketBallState));
    }

    // ==================== rim-relative left/right decision + launch ====================

    [Test]
    public void PlayerDunk_RimToTheRight_LaunchesTowardDunkPositionLeftUsingHostRimVector()
    {
        DunkFixture fixture = BuildAndStart();
        ConfigureForLaunch(fixture);

        fixture.Host.BasketballRimVector = new Vector3(10f, 0f, 0f); // actor sits at the origin - rim is to its right

        fixture.Dunk.playerDunk();

        Assert.That(fixture.Host.CallBallLocked, Is.True);
        Assert.That(fixture.BasketBallState.Locked, Is.True);
        Assert.That(fixture.Host.FaceBasketballGoalCalled, Is.True);
        Assert.That(fixture.Host.RigidBody.linearVelocity.x, Is.GreaterThan(0f),
            "a rim to the right of the actor must launch toward dunkPositionLeft (+X), reading BasketballRimVector off the host");
        Assert.That(fixture.Host.AnimationsPlayed, Does.Contain("inair_dunk"));
        Assert.That(fixture.Host.Locked, Is.False, "Launch must clear the host lock once the velocity is applied");
    }

    [Test]
    public void PlayerDunk_RimToTheLeft_LaunchesTowardDunkPositionRightUsingHostRimVector()
    {
        DunkFixture fixture = BuildAndStart();
        ConfigureForLaunch(fixture);

        fixture.Host.BasketballRimVector = new Vector3(-10f, 0f, 0f); // rim is to the actor's left

        fixture.Dunk.playerDunk();

        Assert.That(fixture.Host.RigidBody.linearVelocity.x, Is.LessThan(0f),
            "a rim to the left of the actor must launch toward dunkPositionRight (-X), reading BasketballRimVector off the host");
        Assert.That(fixture.Host.AnimationsPlayed, Does.Contain("inair_dunk"));
        Assert.That(fixture.Host.Locked, Is.False);
    }

    /// <summary>
    /// Symmetric left/right dunk-position markers (SceneObjects.Find has no scene marker to resolve,
    /// so the production defaults stay Vector3.zero - these overrides stand in for what a resolved
    /// marker would have set) and the minimal real basketball state <c>playerDunk()</c> reads
    /// unconditionally (shot-type bookkeeping is skipped by construction - every flag defaults false)
    /// before the left/right decision.
    /// </summary>
    private void ConfigureForLaunch(DunkFixture fixture)
    {
        fixture.Dunk.transform.position = Vector3.zero;
        SetPrivateField(fixture.Dunk, "dunkPositionLeft", new Vector3(4f, 0f, 0f));
        SetPrivateField(fixture.Dunk, "dunkPositionRight", new Vector3(-4f, 0f, 0f));

        fixture.BasketBallState.BasketBallTarget = Spawn("shot-target");
        fixture.BasketBall.BasketBallPosition = Spawn("ball-position");

        fixture.Host.RigidBody = Spawn("actor-rigidbody").AddComponent<Rigidbody>();
    }

    // ==================== TriggerDunkSequence ====================

    [Test]
    public void TriggerDunkSequence_FreezesPlaysDunkAnimThenRestoresBallAndResetsHasBasketball()
    {
        DunkFixture fixture = BuildAndStart();

        GameObject targetGo = Spawn("rim-target");
        targetGo.transform.position = new Vector3(5f, 10f, 15f);
        fixture.BasketBallState.BasketBallTarget = targetGo;

        Rigidbody ballRigidBody = fixture.BasketBall.gameObject.AddComponent<Rigidbody>();
        ballRigidBody.linearVelocity = new Vector3(1f, 2f, 3f);
        fixture.BasketBall.Rigidbody = ballRigidBody;

        fixture.Host.HasBasketball = true;

        IEnumerator routine = fixture.Dunk.TriggerDunkSequence();

        Assert.That(routine.MoveNext(), Is.True, "step 1: freeze position, play the dunk animation, yield at the first CurrentState wait");
        Assert.That(fixture.Host.FreezePositionCalled, Is.True);
        Assert.That(fixture.Host.AnimationsPlayed, Does.Contain("dunk"));

        Assert.That(routine.MoveNext(), Is.True, "step 2: nothing between the two dunk-state waits");

        Assert.That(routine.MoveNext(), Is.False, "step 3: mark thrown, unfreeze, reposition the ball above the rim, reset hasBasketball");
        Assert.That(fixture.BasketBallState.Thrown, Is.True);
        Assert.That(fixture.Host.UnfreezePositionCalled, Is.True);
        Assert.That(ballRigidBody.linearVelocity, Is.EqualTo(Vector3.zero));
        Assert.That(fixture.BasketBall.transform.position, Is.EqualTo(targetGo.transform.position));
        Assert.That(fixture.Host.HasBasketball, Is.False);
        Assert.That(
            fixture.Host.AnimationBoolsSet.ContainsKey("hasBasketball") && fixture.Host.AnimationBoolsSet["hasBasketball"] == false,
            Is.True);
    }

    // ==================== test double ====================

    private sealed class FakeDunkHost : MonoBehaviour, IPlayerDunkHost, IPlayerControllerParticipantState
    {
        public Rigidbody RigidBody { get; set; }
        public Vector3 BasketballRimVector { get; set; }
        public int CurrentState { get; set; }
        public int DunkStateHash { get; set; }
        public bool Locked { get; set; }
        public bool HasBasketball { get; set; }

        public bool CallBallLocked { get; private set; }
        public bool FaceBasketballGoalCalled { get; private set; }
        public bool FreezePositionCalled { get; private set; }
        public bool UnfreezePositionCalled { get; private set; }
        public List<string> AnimationsPlayed { get; } = new List<string>();
        public Dictionary<string, bool> AnimationBoolsSet { get; } = new Dictionary<string, bool>();

        public void SetCallBallLocked(bool locked) => CallBallLocked = locked;
        public void FaceBasketballGoal() => FaceBasketballGoalCalled = true;
        public void PlayAnimation(string animationName) => AnimationsPlayed.Add(animationName);
        public void SetAnimationBool(string parameterName, bool value) => AnimationBoolsSet[parameterName] = value;
        public void FreezePosition() => FreezePositionCalled = true;
        public void UnfreezePosition() => UnfreezePositionCalled = true;

        // IPlayerControllerParticipantState
        public int PlayerId { get; set; }
        public bool IsCpu { get; set; }
        public GameObject BasketballObject { get; set; }
    }
}
