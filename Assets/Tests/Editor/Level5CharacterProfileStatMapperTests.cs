using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// AUD-012 Phase 2b Slice 28: <c>CharacterProfileStatMapper</c> is the one live production writer of
/// <c>CharacterProfile.IsLocked</c> (<c>internal set</c>) - the reason it moved into
/// <c>Level5.Player</c> together with <c>CharacterProfile</c>, <c>RuntimeCharacterStats</c> and
/// <c>CharacterStats</c> rather than <c>CharacterProfile</c> moving alone. This proves that write still
/// works from the same assembly, and that <c>Apply</c> still copies ordinary stat fields in the same
/// call.
/// </summary>
public class Level5CharacterProfileStatMapperTests
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

    private CharacterProfile MakeProfile()
    {
        GameObject go = new GameObject("mapper_test_profile");
        spawned.Add(go);
        return go.AddComponent<CharacterProfile>();
    }

    private static RuntimeCharacterStats MakeRuntimeStats(bool unlocked)
    {
        return new RuntimeCharacterStats
        {
            characterId = "mapper-test",
            legacyPlayerId = 7,
            displayName = "Mapper Test",
            experience = 12000,
            level = 42,
            pointsSpent = 3,
            unlocked = unlocked,
            stats = new CharacterStats
            {
                accuracy2Pt = 88,
                accuracy3Pt = 61,
                accuracy4Pt = 55,
                accuracy7Pt = 40,
                jumpForce = 5.5f,
                speed = 3.25f,
                runSpeed = 4.75f,
                runSpeedHasBall = 4.25f,
                range = 120,
                release = 90,
                luck = 8,
                shootAngle = 63,
            },
        };
    }

    [TestCase(false, true)]
    [TestCase(true, false)]
    public void ApplyMapsUnlockedToIsLockedInverted(bool unlocked, bool expectedIsLocked)
    {
        CharacterProfile profile = MakeProfile();
        RuntimeCharacterStats runtimeStats = MakeRuntimeStats(unlocked);

        CharacterProfileStatMapper.Apply(profile, runtimeStats);

        Assert.That(profile.IsLocked, Is.EqualTo(expectedIsLocked));
    }

    [Test]
    public void ApplyCopiesRepresentativeNumericFieldsInTheSameCall()
    {
        CharacterProfile profile = MakeProfile();
        RuntimeCharacterStats runtimeStats = MakeRuntimeStats(unlocked: true);

        CharacterProfileStatMapper.Apply(profile, runtimeStats);

        Assert.That(profile.Accuracy2Pt, Is.EqualTo(runtimeStats.stats.accuracy2Pt));
        Assert.That(profile.Range, Is.EqualTo(runtimeStats.stats.range));
        Assert.That(profile.Level, Is.EqualTo(runtimeStats.level));
        Assert.That(profile.PlayerId, Is.EqualTo(runtimeStats.legacyPlayerId));
    }
}
