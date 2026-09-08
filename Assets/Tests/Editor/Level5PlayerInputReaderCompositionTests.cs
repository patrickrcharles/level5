using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Level5.Core;
using Level5.Core.Match;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// AUD-012 Phase 2b Slice 26: <see cref="PlayerInputReader"/> moved into <c>Level5.Input</c> by losing
/// its last two <c>Assembly-CSharp</c> edges. This fixture covers the two behaviours that move made
/// load-bearing, and nothing else - input mappings, action semantics and the touch gesture model are
/// unchanged by that slice and are not re-tested here.
///
/// 1. <see cref="PlayerInputReader.TouchBlockHeld"/> now reads <c>PlayerTouchInputState.BlockHeld</c>
///    alone, having dropped a second read of <c>TouchInputController.instance.HoldDetected</c> that
///    was only ever written in lockstep with it.
/// 2. The legacy mobile joystick axes reach the reader by explicit composition
///    (<c>GameLevelManager</c> -&gt; <see cref="SpawnCoordinator.BindHumanLegacyTouchMovement"/> -&gt;
///    <see cref="PlayerController.BindLegacyTouchMovementReader"/>) instead of by the reader calling
///    <c>GameLevelManager.instance.Joystick</c>. The interesting risk in that inversion is lifetime: a
///    <c>PlayerController</c> destroys and rebuilds its reader whenever gameplay controls are released
///    and reacquired, so a source bound once at spawn has to survive every rebuild.
///
/// The bound source is asserted through the reader's own stored delegate rather than through
/// <see cref="PlayerInputReader.ReadMove"/>, because <c>ReadMove</c> only consults the fallback under
/// <c>(UNITY_ANDROID || UNITY_IOS) &amp;&amp; !UNITY_EDITOR</c>. What is checkable there - and what
/// actually regresses if composition breaks - is whether the reader a controller is currently using
/// resolves to the source composed for it.
///
/// The arithmetic that source feeds is covered directly instead, through
/// <c>PlayerInputReader.ScaleByTouchDistance</c>. <c>ReadLegacyTouchMove</c> itself still cannot be
/// driven from a test - it returns early whenever <c>Input.touchCount</c> is zero, which it always is
/// on a CI runner - so the scaling was split into that pure static helper precisely so the axis
/// mapping and attenuation are checkable without a device. Mirrors
/// <see cref="Level5PlayerControllerArenaContextTests"/>, which reaches
/// <c>PlayerController.ResolveDropShadowHeight</c> the same way.
/// </summary>
public class Level5PlayerInputReaderCompositionTests
{
    /// <summary>
    /// <c>PlayerControls.Dispose()</c> destroys its <c>InputActionAsset</c> with
    /// <c>UnityEngine.Object.Destroy</c>, which the editor refuses outside play mode and reports as an
    /// error. That is an artifact of exercising the real acquire/release path in an EditMode test, not
    /// a defect in the path, so the one test that drives it expects this message explicitly rather than
    /// avoiding the path. Every other test here is handed <see cref="controls"/> directly and never
    /// touches <c>PlayerControlsProvider</c>.
    /// </summary>
    private static readonly Regex DisposeOutsidePlayMode = new Regex("Destroy may not be called from edit mode");

    private readonly List<GameObject> spawned = new List<GameObject>();
    private PlayerRegistry registry;
    private PlayerControls controls;

    [SetUp]
    public void SetUp()
    {
        registry = new PlayerRegistry();
        controls = new PlayerControls();
        PlayerTouchInputState.Clear();
    }

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject go in spawned)
        {
            if (go != null)
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
        }

        spawned.Clear();

        // Not Dispose(): see DisposeOutsidePlayMode. DestroyImmediate reclaims the same asset without
        // the editor complaining.
        controls.Disable();
        UnityEngine.Object.DestroyImmediate(controls.asset);
        controls = null;

        PlayerTouchInputState.Clear();
    }

    // ==================== TouchBlockHeld ====================

    [Test]
    public void TouchBlockHeld_IsFalseWhenNoTouchBlockIsHeld()
    {
        PlayerTouchInputState.BlockHeld = false;

        Assert.That(new PlayerInputReader(null).TouchBlockHeld, Is.False);
    }

    [Test]
    public void TouchBlockHeld_IsTrueWhileTheTouchBlockIsHeld()
    {
        PlayerTouchInputState.BlockHeld = true;

        Assert.That(new PlayerInputReader(null).TouchBlockHeld, Is.True);
    }

    [Test]
    public void TouchBlockHeld_IsReleasedByTheTouchStateClearThatOutlivesAScene()
    {
        // TouchInputController.OnDisable clears the touch state when the controller goes away; that is
        // the path that also used to zero its own HoldDetected flag, and the one this property now
        // depends on alone.
        PlayerInputReader reader = new PlayerInputReader(null);
        PlayerTouchInputState.BlockHeld = true;
        Assert.That(reader.TouchBlockHeld, Is.True);

        PlayerTouchInputState.Clear();

        Assert.That(reader.TouchBlockHeld, Is.False);
    }

    // ==================== legacy touch scaling ====================

    [Test]
    public void ScaleByTouchDistance_NoScreenRange_PassesBothAxesThroughUnchanged()
    {
        // screenXRange/screenYRange are Screen.width/10 and Screen.height/10; a zero range means
        // "do not attenuate this axis", which is also the only way to observe the raw axis mapping.
        Vector2 scaled = ScaleByTouchDistance(
            legacyMovement: new Vector2(0.25f, -0.75f),
            touchPosition: new Vector2(500f, 500f),
            startTouchPosition: Vector2.zero,
            screenXRange: 0f,
            screenYRange: 0f);

        Assert.That(scaled.x, Is.EqualTo(0.25f).Within(0.0001f), "joystick horizontal must drive x");
        Assert.That(scaled.y, Is.EqualTo(-0.75f).Within(0.0001f), "joystick vertical must drive y");
    }

    [Test]
    public void ScaleByTouchDistance_ScalesEachAxisByItsOwnDisplacement()
    {
        // The transposition guard: distinct displacements per axis, so swapping the two range
        // calculations - the exact mistake the mobile-only compilation gap would have hidden - fails
        // here. Horizontal is dragged 25% of its range, vertical 50% of its own.
        Vector2 scaled = ScaleByTouchDistance(
            legacyMovement: new Vector2(1f, 1f),
            touchPosition: new Vector2(25f, 100f),
            startTouchPosition: Vector2.zero,
            screenXRange: 100f,
            screenYRange: 200f);

        Assert.That(scaled.x, Is.EqualTo(0.25f).Within(0.0001f));
        Assert.That(scaled.y, Is.EqualTo(0.5f).Within(0.0001f));
    }

    [Test]
    public void ScaleByTouchDistance_DragBeyondTheRange_LeavesThatAxisAtFullMagnitude()
    {
        // Attenuation only applies below 1; at or past the range the joystick reads at full strength.
        Vector2 scaled = ScaleByTouchDistance(
            legacyMovement: new Vector2(0.8f, 0.8f),
            touchPosition: new Vector2(100f, 400f),
            startTouchPosition: Vector2.zero,
            screenXRange: 100f,
            screenYRange: 200f);

        Assert.That(scaled.x, Is.EqualTo(0.8f).Within(0.0001f));
        Assert.That(scaled.y, Is.EqualTo(0.8f).Within(0.0001f));
    }

    [Test]
    public void ScaleByTouchDistance_DraggingBackwards_AttenuatesByAbsoluteDistance()
    {
        // Mathf.Abs: dragging left/down is the same distance as dragging right/up, and must never
        // flip the sign of the joystick's own axis.
        Vector2 scaled = ScaleByTouchDistance(
            legacyMovement: new Vector2(1f, -1f),
            touchPosition: new Vector2(-50f, -50f),
            startTouchPosition: Vector2.zero,
            screenXRange: 100f,
            screenYRange: 100f);

        Assert.That(scaled.x, Is.EqualTo(0.5f).Within(0.0001f));
        Assert.That(scaled.y, Is.EqualTo(-0.5f).Within(0.0001f));
    }

    // ==================== legacy movement composition ====================

    [Test]
    public void BindHumanLegacyTouchMovement_HumanParticipant_ReachesTheReaderItIsUsing()
    {
        Vector2 axes = new Vector2(0.5f, -0.25f);
        PlayerIdentifier human = RegisterHumanParticipant(pid: 0);
        MakeCoordinator().BindHumanLegacyTouchMovement(() => axes);

        PlayerInputReader reader = BuildInputReader(human.playerController);

        Assert.That(ReadBoundLegacyMovement(reader), Is.EqualTo(axes));
    }

    [Test]
    public void BindHumanLegacyTouchMovement_MultipleHumans_AllReachTheSameSource()
    {
        Vector2 axes = new Vector2(-1f, 1f);
        PlayerIdentifier first = RegisterHumanParticipant(pid: 0);
        PlayerIdentifier second = RegisterHumanParticipant(pid: 1);
        MakeCoordinator().BindHumanLegacyTouchMovement(() => axes);

        Assert.That(ReadBoundLegacyMovement(BuildInputReader(first.playerController)), Is.EqualTo(axes));
        Assert.That(ReadBoundLegacyMovement(BuildInputReader(second.playerController)), Is.EqualTo(axes));
    }

    [Test]
    public void BindHumanLegacyTouchMovement_CpuParticipant_IsUnaffected()
    {
        RegisterHumanParticipant(pid: 0);
        PlayerIdentifier cpu = RegisterCpuParticipant(pid: 1);

        Assert.DoesNotThrow(() => MakeCoordinator().BindHumanLegacyTouchMovement(() => Vector2.one));

        Assert.IsNull(cpu.playerController, "a CPU participant has no PlayerController to bind player input to");
    }

    [Test]
    public void BindHumanLegacyTouchMovement_HumanMissingPlayerController_LogsAndContinues()
    {
        // Same fail-closed shape as the arena-context pass: one broken participant must not abort
        // binding for the humans registered after it.
        GameObject actorGo = Spawn("human-actor-no-controller");
        actorGo.AddComponent<CharacterProfile>();
        PlayerIdentifier identifier = actorGo.AddComponent<PlayerIdentifier>();
        identifier.setIds(0, false);
        identifier.player = actorGo;
        registry.Add(identifier);
        PlayerIdentifier intact = RegisterHumanParticipant(pid: 1);

        LogAssert.Expect(LogType.Error, new Regex("no PlayerController"));
        Assert.DoesNotThrow(() => MakeCoordinator().BindHumanLegacyTouchMovement(() => Vector2.one));

        Assert.That(ReadBoundLegacyMovement(BuildInputReader(intact.playerController)), Is.EqualTo(Vector2.one));
    }

    [Test]
    public void AnUnboundControllerReadsNoLegacyMovementRatherThanThrowing()
    {
        // Every non-mobile scene, and any gameplay scene with no joystick object, leaves this unbound -
        // the same "no legacy movement" answer the old null-joystick guard produced.
        PlayerIdentifier human = RegisterHumanParticipant(pid: 0);

        Assert.That(ReadBoundLegacyMovement(BuildInputReader(human.playerController)), Is.EqualTo(Vector2.zero));
    }

    // ==================== reader lifetime ====================

    [Test]
    public void TheBoundSourceIsReadLiveNotSnapshottedWhenTheReaderIsBuilt()
    {
        // The joystick's axes change every frame, so the composed source has to be something the reader
        // asks for at ReadMove() time - not a value captured when it was constructed or bound.
        Vector2 axes = new Vector2(1f, 0f);
        PlayerIdentifier human = RegisterHumanParticipant(pid: 0);
        MakeCoordinator().BindHumanLegacyTouchMovement(() => axes);
        PlayerInputReader reader = BuildInputReader(human.playerController);
        Assert.That(ReadBoundLegacyMovement(reader), Is.EqualTo(new Vector2(1f, 0f)));

        axes = new Vector2(-0.75f, 0.5f);

        Assert.That(
            ReadBoundLegacyMovement(reader),
            Is.EqualTo(new Vector2(-0.75f, 0.5f)),
            "the reader must read the composed legacy joystick axes at the point of use, not a value "
            + "captured when the reader was built");
    }

    [Test]
    public void AReaderBuiltAfterCompositionStillReceivesTheBoundSource()
    {
        // Binding happens during GameLevelManager.Awake's spawn pass, but a spawned controller does not
        // build its first reader until its own Start(). Order must not matter.
        Vector2 axes = new Vector2(0.1f, 0.2f);
        PlayerIdentifier human = RegisterHumanParticipant(pid: 0);
        MakeCoordinator().BindHumanLegacyTouchMovement(() => axes);

        Assert.That(ReadBoundLegacyMovement(BuildInputReader(human.playerController)), Is.EqualTo(axes));
    }

    [Test]
    public void AReaderRebuiltAfterInputReleaseAndReacquireStillReceivesTheBoundSource()
    {
        // The real disable/re-enable path: OnDisable releases this player's gameplay controls and drops
        // the reader entirely; OnEnable reacquires and rebuilds one. Nothing rebinds the joystick in
        // between, so the controller - not the reader - has to be what holds the composed source.
        Vector2 axes = new Vector2(-0.3f, 0.8f);
        PlayerIdentifier human = RegisterHumanParticipant(pid: 0);
        PlayerController controller = human.playerController;
        MakeCoordinator().BindHumanLegacyTouchMovement(() => axes);

        SetPrivateField(controller, "hasStarted", true);
        InvokePrivate(controller, "InitializeInput");
        PlayerInputReader first = (PlayerInputReader)GetPrivateField(controller, "inputReader");
        Assert.IsNotNull(first, "InitializeInput must produce an input reader for a human participant");

        // Invoked directly: PlayerController carries no [ExecuteAlways], so the editor does not
        // dispatch these messages for a component toggled outside play mode. This is still the real
        // release/reacquire pair, provider calls and all.
        LogAssert.Expect(LogType.Error, DisposeOutsidePlayMode);
        InvokePrivate(controller, "OnDisable");
        Assert.IsNull(GetPrivateField(controller, "inputReader"), "OnDisable must drop the reader");
        InvokePrivate(controller, "OnEnable");

        PlayerInputReader rebuilt = (PlayerInputReader)GetPrivateField(controller, "inputReader");
        Assert.IsNotNull(rebuilt, "OnEnable must rebuild the reader once the controller has started");
        Assert.AreNotSame(first, rebuilt, "this test only means anything if the reader was actually rebuilt");
        Assert.That(ReadBoundLegacyMovement(rebuilt), Is.EqualTo(axes));

        LogAssert.Expect(LogType.Error, DisposeOutsidePlayMode);
        PlayerControlsProvider.ReleaseGameplayControls(0);
    }

    [Test]
    public void AReaderRebuiltByTheControlsSetterStillReceivesTheBoundSource()
    {
        // The third construction site: assigning PlayerController.Controls replaces the reader outright.
        Vector2 axes = new Vector2(0.9f, -0.9f);
        PlayerIdentifier human = RegisterHumanParticipant(pid: 0);
        PlayerController controller = human.playerController;
        MakeCoordinator().BindHumanLegacyTouchMovement(() => axes);

        controller.Controls = controls;

        PlayerInputReader reader = (PlayerInputReader)GetPrivateField(controller, "inputReader");
        Assert.IsNotNull(reader);
        Assert.That(ReadBoundLegacyMovement(reader), Is.EqualTo(axes));
    }

    // ==================== helpers ====================

    private GameObject Spawn(string name)
    {
        GameObject go = new GameObject(name);
        spawned.Add(go);
        return go;
    }

    private PlayerIdentifier RegisterHumanParticipant(int pid)
    {
        GameObject actorGo = Spawn($"human-actor-{pid}");
        actorGo.AddComponent<CharacterProfile>();
        actorGo.AddComponent<PlayerController>();
        PlayerIdentifier identifier = actorGo.AddComponent<PlayerIdentifier>();
        identifier.setIds(pid, false);
        identifier.player = actorGo;
        identifier.setPlayer(actorGo);
        registry.Add(identifier);

        // Stand in for what InitializeInput() would have acquired from PlayerControlsProvider, so
        // TryEnsureInputReader below builds a reader without this fixture owning provider state. The
        // one test that does need the real acquire/release path calls InitializeInput itself.
        SetPrivateField(identifier.playerController, "controls", controls);
        return identifier;
    }

    private PlayerIdentifier RegisterCpuParticipant(int pid)
    {
        GameObject actorGo = Spawn($"cpu-actor-{pid}");
        actorGo.AddComponent<CharacterProfile>();
        actorGo.AddComponent<AutoPlayerController>();
        PlayerIdentifier identifier = actorGo.AddComponent<PlayerIdentifier>();
        identifier.setIds(pid, true);
        identifier.autoPlayer = actorGo;
        identifier.setAutoPlayer(actorGo);
        registry.Add(identifier);
        return identifier;
    }

    private SpawnCoordinator MakeCoordinator()
    {
        return new SpawnCoordinator(
            new SpawnCoordinator.SpawnLocations(),
            registry,
            new ResolvedMatchRules(combatMode: CombatMode.Standard, enemiesEnabled: false, hardcore: false, enemiesOnly: false),
            new PlayerRoster(new PlayerSlot[0]),
            GameModeId.None);
    }

    /// <summary>
    /// Drives <c>PlayerController.TryEnsureInputReader</c> - the shared path every gameplay frame takes
    /// to obtain a reader, and the one that rebuilds a missing one - rather than reaching for whichever
    /// reader instance happens to already exist.
    /// </summary>
    private static PlayerInputReader BuildInputReader(PlayerController controller)
    {
        MethodInfo tryEnsure = typeof(PlayerController).GetMethod(
            "TryEnsureInputReader", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(tryEnsure, "PlayerController must declare TryEnsureInputReader(out PlayerInputReader)");

        object[] args = new object[] { null };
        bool built = (bool)tryEnsure.Invoke(controller, args);
        Assert.IsTrue(built, "TryEnsureInputReader must produce a reader for an enabled human controller");
        return (PlayerInputReader)args[0];
    }

    /// <summary>
    /// The legacy joystick axes the given reader currently resolves to. Reached through the reader's
    /// stored delegate because the branch that consumes it (<c>ReadLegacyTouchMove</c>) is compiled out
    /// of an Editor run - see this fixture's summary.
    /// </summary>
    private static Vector2 ReadBoundLegacyMovement(PlayerInputReader reader)
    {
        Func<Vector2> source = (Func<Vector2>)GetPrivateField(reader, "legacyTouchMovementReader");
        Assert.IsNotNull(
            source,
            "every PlayerInputReader a PlayerController builds must be given its legacy movement source");
        return source.Invoke();
    }

    /// <summary>
    /// Invokes <c>PlayerInputReader.ScaleByTouchDistance</c>, the pure static the touch-distance
    /// scaling was split into. Private because nothing outside the reader should call it; reached the
    /// same way <see cref="Level5PlayerControllerArenaContextTests"/> reaches
    /// <c>ResolveDropShadowHeight</c>.
    /// </summary>
    private static Vector2 ScaleByTouchDistance(
        Vector2 legacyMovement,
        Vector2 touchPosition,
        Vector2 startTouchPosition,
        float screenXRange,
        float screenYRange)
    {
        MethodInfo scale = typeof(PlayerInputReader).GetMethod(
            "ScaleByTouchDistance", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(scale, "PlayerInputReader must declare ScaleByTouchDistance(...)");

        return (Vector2)scale.Invoke(null, new object[]
        {
            legacyMovement, touchPosition, startTouchPosition, screenXRange, screenYRange,
        });
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

    private static void InvokePrivate(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, $"{target.GetType().Name} must declare {methodName}()");
        method.Invoke(target, null);
    }
}
