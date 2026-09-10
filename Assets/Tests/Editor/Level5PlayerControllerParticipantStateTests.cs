using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// AUD-012 Phase 2b Slice 34: <see cref="PlayerIdentifier"/> now implements
/// <see cref="IPlayerControllerParticipantState"/>, the narrow contract <c>PlayerController</c> uses
/// instead of the concrete identifier. These tests establish the mapping is real - <c>PlayerId</c>/
/// <c>IsCpu</c>/<c>BasketballObject</c> read the same public <c>pid</c>/<c>isCpu</c>/<c>basketball</c>
/// fields every other caller already uses, <c>BasketballObject</c> specifically returns the human
/// <c>basketball</c> rather than <c>autoBasketball</c>, and the interface stays a live view rather than
/// a value captured at resolution time. Mirrors the shape
/// <see cref="Level5PlayerIdleSniperRuntimeTests"/> already established for
/// <c>IPlayerIdleSniperRuntime</c>.
/// </summary>
public class Level5PlayerControllerParticipantStateTests
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

    [Test]
    public void PlayerIdentifierImplementsIPlayerControllerParticipantState()
    {
        PlayerIdentifier identifier = Spawn("identifier").AddComponent<PlayerIdentifier>();

        Assert.That(identifier, Is.InstanceOf<IPlayerControllerParticipantState>());
    }

    [Test]
    public void PlayerIdMapsDirectlyToPid()
    {
        PlayerIdentifier identifier = Spawn("identifier").AddComponent<PlayerIdentifier>();
        identifier.pid = 3;
        IPlayerControllerParticipantState participant = identifier;

        Assert.That(participant.PlayerId, Is.EqualTo(3));
    }

    [Test]
    public void IsCpuMapsDirectlyToIsCpu()
    {
        PlayerIdentifier identifier = Spawn("identifier").AddComponent<PlayerIdentifier>();
        identifier.isCpu = true;
        IPlayerControllerParticipantState participant = identifier;

        Assert.That(participant.IsCpu, Is.True);
    }

    [Test]
    public void BasketballObjectMapsDirectlyToTheHumanBasketballField()
    {
        PlayerIdentifier identifier = Spawn("identifier").AddComponent<PlayerIdentifier>();
        GameObject humanBall = Spawn("human-basketball");
        identifier.basketball = humanBall;
        IPlayerControllerParticipantState participant = identifier;

        Assert.That(participant.BasketballObject, Is.SameAs(humanBall));
    }

    [Test]
    public void BasketballObjectDoesNotReturnAutoBasketball()
    {
        PlayerIdentifier identifier = Spawn("identifier").AddComponent<PlayerIdentifier>();
        identifier.basketball = Spawn("human-basketball");
        identifier.autoBasketball = Spawn("auto-basketball");
        IPlayerControllerParticipantState participant = identifier;

        Assert.That(
            participant.BasketballObject,
            Is.Not.SameAs(identifier.autoBasketball),
            "PlayerController's Start() only ever consumed the human basketball field - BasketballObject must not fall back to autoBasketball");
    }

    [Test]
    public void ThePropertiesReadLiveBackingStateRatherThanASnapshot()
    {
        PlayerIdentifier identifier = Spawn("identifier").AddComponent<PlayerIdentifier>();
        identifier.pid = 0;
        IPlayerControllerParticipantState participant = identifier;
        Assert.That(participant.PlayerId, Is.EqualTo(0));

        identifier.pid = 7;

        Assert.That(
            participant.PlayerId,
            Is.EqualTo(7),
            "IPlayerControllerParticipantState must read the current field value at the point of use, not one captured when the reference was obtained");
    }

    [Test]
    public void InitializeInputDisablesTheControllerForACpuParticipantThroughTheInterfacePath()
    {
        // PlayerController.InitializeInput() now resolves GetComponent<IPlayerControllerParticipantState>()
        // rather than the concrete PlayerIdentifier; this proves the existing CPU guard still fires
        // through that interface path, without needing the full MatchRuntime/composition rig the
        // human branch would require.
        GameObject actorGo = Spawn("cpu-actor");
        PlayerController controller = actorGo.AddComponent<PlayerController>();
        PlayerIdentifier identifier = actorGo.AddComponent<PlayerIdentifier>();
        identifier.setIds(0, true);

        LogAssert.Expect(LogType.Error, new Regex("cannot own input for a CPU player"));
        InvokePrivate(controller, "InitializeInput");

        Assert.That(controller.enabled, Is.False);
    }

    private static void InvokePrivate(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, $"{target.GetType().Name} must declare {methodName}()");
        method.Invoke(target, null);
    }
}
