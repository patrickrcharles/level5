
using System;
using System.Collections.Generic;
using UnityEngine;
using Level5.Core;
using Level5.Core.Match;

public class CharacterProfile : MonoBehaviour
{
    [SerializeField] private int playerId;
    [SerializeField] private int userid;
    [SerializeField] public bool isCpu;  
    [SerializeField] private bool isDefensiveCpuPlayer;
    [SerializeField] private string playerDisplayName;
    [SerializeField] private string playerObjectName;
    [SerializeField] private Sprite playerPortrait;
    [SerializeField] public Sprite winPortrait;
    [SerializeField] public Sprite losePortrait;

    private float jumpStatFloor = 3.5f;
    private float jumpStatCeiling = 6;

    private float speedStatFloor = 2.5f;
    private float speedStatCeiling = 6.5f;

    [SerializeField] private float accuracy2pt;
    [SerializeField] private float accuracy3pt;
    [SerializeField] private float accuracy4pt;
    [SerializeField] private float accuracy7pt;
    [SerializeField] private float fadeaway;

    [SerializeField] private string shooterProfilePrefabName;

    [SerializeField] private float jumpForce;
    [SerializeField] private float speed;
    [SerializeField] private float runSpeedHasBall;

    [SerializeField] private float runSpeed;
    [SerializeField] private float inAirSpeed;

    [SerializeField] private int range;
    [SerializeField] private int release;

    [SerializeField] private int luck;
    [SerializeField] private int clutch;

    [SerializeField] private int shootAngle;

    [SerializeField] private int level;
    [SerializeField] private int experience;
    [SerializeField] private int pointsAvailable;
    [SerializeField] private int pointsUsed;
    [SerializeField] private decimal money;
    [SerializeField] private bool isFighter;
    [SerializeField] private bool isShooter;
    [SerializeField] private bool isLocked;
    [SerializeField] private CpuBaseStats.ShooterType cpuType;

    // Match-only CPU context (#71). Runtime-only: never serialized into a prefab, and set by
    // SpawnCoordinator - the only thing that knows both the roster's primary slot and the resolved
    // match rules - rather than discovered here through GameLevelManager.instance. preparedBaseCpuLevel
    // is captured at prepare time specifically so a repeated ApplyPreparedCpuMatchInitialization call
    // (e.g. a hypothetical retry) always resolves the Hardcore bump from the same pre-Hardcore base,
    // never from a level this method already boosted.
    private bool hasPreparedMatchContext;
    private int preparedBaseCpuLevel;
    private int preparedPrimaryHumanLevel;

    // AUD-012 Phase 2b Slice 27: the match/persistence context this profile used to discover for
    // itself through MatchRuntime and LoadedData. Runtime-only - none of it is serialized into a
    // prefab - and all of it is supplied by SpawnCoordinator, the composition point that already owns
    // the resolved rules for this match. preparedRules is deliberately the one field both the human
    // and the CPU preparation paths write: Start() needs rules for either kind of participant, and
    // two independent rule fields could disagree about the same match. It is also the only record
    // that rules arrived - a plain C# class, so the null check is a real one - rather than a bool
    // beside it that a later edit could forget to keep in step.
    private ResolvedMatchRules preparedRules;
    private Func<int, CharacterProfile> preparedProfileResolver;
    private CheerleaderSelection preparedCheerleader;

    void Start()
    {
        fadeaway =  level < 50 ? 50 : level;
        InAirSpeed = (float)fadeaway / 100;
        bool ranCpuMatchInitialization = isCpu && !isDefensiveCpuPlayer;
        if (ranCpuMatchInitialization)
        {
            ApplyPreparedCpuMatchInitialization();
        }

        // Slice 27: the rules come from PrepareHumanMatchContext/PrepareCpuMatchContext instead of
        // MatchRuntime.Rules. Every participant SpawnCoordinator registers - human or CPU, configured
        // match or directly entered scene - is prepared before its Start() runs, and that is the same
        // answer MatchRuntime.Rules gave: GameLevelManager.Awake reads MatchRuntime.Rules once and
        // hands that exact instance to the coordinator. An instance that reached here without
        // composition keeps its context-free initialization above and skips only this override.
        // Deliberately not an early return: a missing context must skip *this override only*, not
        // whatever else Start grows later, which is what "continue context-free initialization" means.
        if (preparedRules == null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // One report per participant: an unprepared CPU has already said so in
            // ApplyPreparedCpuMatchInitialization, and both messages have the same single cause.
            if (!ranCpuMatchInitialization)
            {
                Debug.LogWarning(
                    $"CharacterProfile '{playerObjectName}' reached Start with no match rules prepared "
                    + "by SpawnCoordinator. Profile initialization continues; only the Arcade/easy "
                    + "maximum-stat override is skipped, because there are no rules to read it from.",
                    this);
            }
#endif
        }
        else if (preparedRules.ArcadeMode || MatchDifficulties.ToInt(preparedRules.Difficulty) == 0 )
        {
            Accuracy2Pt = 100;
            Accuracy3Pt = 100;
            Accuracy4Pt = 100;
            Accuracy7Pt = 100;
            Release = 100;
            Range = 150;
            Clutch = 100;
            Luck = 10;
        }
    }

    public CharacterProfile() 
    {
        //Debug.Log("CharacterProfile() ");
        //if (isCpu)
        //{
        //    intializeCpuShooterStats();
        //}
    }

    /// <summary>
    /// Gives this human profile the saved-data lookup, the cheerleader and the resolved rules that
    /// <see cref="intializeShooterStatsFromProfile"/> and <see cref="Start"/> need, before either
    /// runs. Called by <c>SpawnCoordinator</c> for every registered human - configured match or
    /// directly entered scene - so this profile no longer reaches <c>LoadedData.instance</c> or
    /// <c>MatchRuntime</c> for any of it (AUD-012 Phase 2b Slice 27).
    ///
    /// Runtime-only state, and the resolver is a plain <see cref="Func{T, TResult}"/> rather than a
    /// service or repository interface: composition already knows how to answer "the saved profile for
    /// this character id", and this type only has to be able to ask.
    /// </summary>
    public void PrepareHumanMatchContext(
        Func<int, CharacterProfile> profileResolver,
        CheerleaderSelection cheerleader,
        ResolvedMatchRules rules)
    {
        preparedProfileResolver = profileResolver;
        preparedCheerleader = cheerleader;
        preparedRules = rules;
    }

    /// <summary>
    /// Rebuilds this profile from the saved data for <paramref name="characterId"/>, plus the
    /// match cheerleader's bonuses.
    ///
    /// The character id is a parameter rather than the primary slot's id read in here: this runs for
    /// every human in the match, and reading slot zero's id meant a second local human was rebuilt as
    /// - and credited as - the first one. The cheerleader is still match-wide, because the match
    /// configuration only carries one.
    ///
    /// Slice 27: the saved profile, the cheerleader and the rules all arrive through
    /// <see cref="PrepareHumanMatchContext"/>, which composition must call first. No
    /// <c>LoadedData</c> fallback is kept here - an unprepared call is a composition defect and
    /// reports itself as one rather than quietly rebuilding from global state.
    /// </summary>
    public void intializeShooterStatsFromProfile(int characterId)
    {
        if (preparedProfileResolver == null || preparedCheerleader == null || preparedRules == null)
        {
            Debug.LogError(
                $"CharacterProfile '{playerObjectName}' was asked to rebuild character id {characterId} "
                + "from saved data with no human match context prepared. Composition "
                + "(SpawnCoordinator.InitializeHumanProfile) must call PrepareHumanMatchContext first; "
                + "nothing on this profile was changed.",
                this);
            return;
        }

        CharacterProfile temp = preparedProfileResolver(characterId);
        if (temp == null)
        {
            Debug.LogError($"CharacterProfile could not resolve the selected player profile for character id {characterId}.");
            return;
        }

        experience = temp.Experience;
        level = CharacterLevel.FromExperience(temp.Experience);
        fadeaway = level;
        InAirSpeed = fadeaway / 10;
        playerObjectName = temp.playerObjectName != null ? temp.playerObjectName : "";
        playerDisplayName = temp.playerDisplayName;
        playerId = temp.playerId;

        Speed = temp.speed;
        RunSpeed = temp.runSpeed;
        runSpeedHasBall = temp.runSpeedHasBall;

        JumpForce = temp.jumpForce;
        shootAngle = temp.shootAngle;

        Accuracy2Pt = temp.accuracy2pt;
        Accuracy3Pt = temp.accuracy3pt + preparedCheerleader.BonusThreeAccuracy;
        Accuracy4Pt = temp.accuracy4pt + preparedCheerleader.BonusFourAccuracy;
        Accuracy7Pt = temp.accuracy7pt + preparedCheerleader.BonusSevenAccuracy;

        Range = temp.range + preparedCheerleader.BonusRange;
        Release = temp.release + preparedCheerleader.BonusRelease;

        clutch = temp.clutch + preparedCheerleader.BonusClutch;

        pointsAvailable = temp.PointsAvailable;
        pointsUsed = temp.PointsUsed;

        // if 3/4/All point contest, disable Luck/citical %
        if (preparedRules.IsThreePointContest
            || preparedRules.IsFourPointContest
            || preparedRules.IsSevenPointContest
            || preparedRules.IsAllPointContest)
        {
            Luck = 0;
            clutch = 0;
        }
        else
        {
            Luck = temp.Luck + preparedCheerleader.BonusLuck;
        }
    }
    /// <summary>
    /// Context-free CPU baseline calculation (#71): derives accuracy/release/range/luck/clutch from
    /// this profile's current Level and CpuType only. Never reads match, GameOptions or
    /// GameLevelManager state, and never changes Level - safe to call during menu-boot CPU catalog
    /// loading (no gameplay scene exists yet) and safe to call more than once on the same instance
    /// (Resources.LoadAll returns cached asset instances a retry can revisit): calling it twice with
    /// an unchanged Level/CpuType always produces the same output.
    /// </summary>
    public void InitializeCpuBaselineStats()
    {
        calculateAccuracyAttributeRatings();

        Range = CpuBaseStats.RANGE + (level * 5);
        int luckCalc = CpuBaseStats.LUCK + (level / CpuBaseStats.LUCK_DIVIDER);
        Luck = luckCalc <= 10 ? luckCalc : 10;
        clutch = level <= 100 ? level : 100;

        if (isDefensiveCpuPlayer)
        {
            inAirSpeed = ((float)level / 100) * 3;
        }
    }

    /// <summary>
    /// Gives this CPU profile the match context Hardcore/contest initialization needs, before
    /// <see cref="Start"/> applies it. Called by <c>SpawnCoordinator</c> - the only thing that knows
    /// both the roster's primary slot and the resolved match rules - rather than this profile
    /// discovering either through <c>GameLevelManager.instance</c> (#71).
    ///
    /// <see cref="hasPreparedMatchContext"/> tracks that the CPU path specifically ran, which the
    /// rules reference alone cannot say - a human is prepared with rules and no CPU context. It is
    /// derived from the same condition as those rules, so one call cannot claim a context whose rules
    /// <see cref="ApplyPreparedCpuMatchInitialization"/> would then dereference: a prepare with no
    /// rules leaves this profile unprepared and takes the safe baseline. Production never passes null
    /// here - <c>GameLevelManager</c> resolves the rules before it builds the coordinator - so this
    /// only decides which way an already-broken caller fails.
    /// </summary>
    public void PrepareCpuMatchContext(int primaryHumanLevel, ResolvedMatchRules rules)
    {
        preparedBaseCpuLevel = level;
        preparedPrimaryHumanLevel = primaryHumanLevel;
        preparedRules = rules;
        hasPreparedMatchContext = rules != null;
    }

    /// <summary>
    /// Applies the Hardcore level bump and contest Luck/Clutch suppression using the context
    /// <see cref="PrepareCpuMatchContext"/> stored, then recalculates the baseline off the result.
    ///
    /// Falls back to a safe baseline - no Hardcore bump, no invented primary level - when no context
    /// was prepared, which should only happen for a CPU that reached <see cref="Start"/> without
    /// going through <c>SpawnCoordinator</c>.
    /// </summary>
    public void ApplyPreparedCpuMatchInitialization()
    {
        if (!hasPreparedMatchContext)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogWarning(
                $"CharacterProfile '{playerObjectName}' initialized as a CPU with no match context "
                + "prepared by SpawnCoordinator. Using baseline CPU stats with no Hardcore level bump.",
                this);
#endif
            InitializeCpuBaselineStats();
            return;
        }

        level = CpuDifficultyLevelPolicy.Resolve(preparedBaseCpuLevel, preparedPrimaryHumanLevel, preparedRules.Hardcore);

        InitializeCpuBaselineStats();

        // if 3/4/All point contest, disable Luck/citical %
        if (preparedRules.IsThreePointContest
            || preparedRules.IsFourPointContest
            || preparedRules.IsSevenPointContest
            || preparedRules.IsAllPointContest)
        {
            Luck = 0;
            clutch = 0;
        }
    }

    public void calculateAccuracyAttributeRatings()
    {
        int three = 0;
        int four = 0;
        int seven = 0;
        int remainder = 0;
        int release = 0;

        Accuracy2Pt = 90;
        if (cpuType == CpuBaseStats.ShooterType.Three)
        {
            three = (int)(level * 0.5f);
            four = (int)(level * 0.15f);
            seven = (int)(level * 0.15f);
            release = (int)(level * 0.2f);

            remainder = level - (three + four + seven + release);
        }
        if (cpuType == CpuBaseStats.ShooterType.Four)
        {
            three = (int)(level * 0.15f);
            four = (int)(level * 0.5f);
            seven = (int)(level * 0.15f);
            release = (int)(level * 0.2f);
            remainder = level - (three + four + seven + release);
        }
        if (cpuType == CpuBaseStats.ShooterType.Seven)
        {
            three = (int)(level * 0.15f);
            four = (int)(level * 0.15f);
            seven = (int)(level * 0.5f);
            release = (int)(level * 0.2f);
            remainder = level - (three + four + seven + release);
        }
        if (three > 25) { remainder += (three - 25); three = 25; }
        if (four > 25) { remainder += (four - 25); four = 25; }
        if (seven > 25) { remainder += (seven - 25); seven = 25; }
        if (release > 25) { remainder += (release - 25); release = 25; }

        // redistribute points
        int[] attributes = new int[] { three, four, seven, release };
        //int icount = 0;
        //int jcount = 0;
        for (int i = 0; i < remainder; i++)
        {
            //icount++;
            for (int j = 0; j < attributes.Length; j++)
            {
                if (attributes[j] < 25)
                {
                    attributes[j]++;
                    //jcount++;
                    j++;
                }
            }
        }

        //Debug.Log("---icount : "+ icount);
        //Debug.Log("------jcount : "+ jcount);
        //Debug.Log("exit while loop ");
        Accuracy3Pt = CpuBaseStats.ACCURACY_3PT + attributes[0];
        Accuracy4Pt = CpuBaseStats.ACCURACY_4PT + attributes[1];
        Accuracy7Pt = CpuBaseStats.ACCURACY_7PT + attributes[2];
        Release = CpuBaseStats.RELEASE + +attributes[3];
    }

    //private void calculateCpu3ptAccuracy(int accuracy)
    //{
    //    int levelpoints = level;
    //    if (cpuType == CpuBaseStats.ShooterType.Three)
    //    {
    //        if(levelpoints > 10) 
    //        {
    //            accuracy3pt += 10;
    //            levelpoints -= 10;
    //        }
    //    }
    //}
    //private void calculateCpu4ptAccuracy(int accuracy)
    //{

    //}
    //private void calculateCpu7ptAccuracy(int accuracy)
    //{

    //}

    public float calculateJumpValueToPercent()
    {
        //modifier
        float modifier = 100 / ((jumpStatCeiling - jumpStatFloor) * 10);
        // percent
        float percent = (JumpForce - jumpStatFloor) * modifier * 10;
        return percent;
    }
    public float calculateSpeedToPercent()
    {
        //modifier
        float modifier = 100 / ((speedStatCeiling - speedStatFloor) * 10);
        // percent
        float percent = (runSpeed - speedStatFloor) * modifier * 10;
        return percent;
    }
    public float RunSpeedHasBall
    {
        get => runSpeedHasBall;
        set => runSpeedHasBall = value;
    }
    public int PlayerId
    {
        get => playerId;
        set => playerId = value;
    }
    public int Level
    {
        get => level;
        set => level = value;
    }
    public int Experience
    {
        get => experience;
        set => experience = value;
    }
    public string PlayerDisplayName
    {
        get => playerDisplayName;
        set => playerDisplayName = value;
    }
    public string PlayerObjectName
    {
        get => playerObjectName;
        set => playerObjectName = value;
    }
    public float Accuracy2Pt
    {
        get => accuracy2pt;
        set => accuracy2pt = value;
    }
    public float Accuracy3Pt
    {
        get => accuracy3pt;
        set => accuracy3pt = value;
    }
    public float Accuracy4Pt
    {
        get => accuracy4pt;
        set => accuracy4pt = value;
    }
    public float Accuracy7Pt
    {
        get => accuracy7pt;
        set => accuracy7pt = value;
    }
    public float JumpForce
    {
        get => jumpForce;
        set => jumpForce = value;
    }
    public float Speed
    {
        get => speed;
        set => speed = value;
    }
    public float RunSpeed
    {
        get => runSpeed;
        set => runSpeed = value;
    }
    public int Luck
    {
        get => luck;
        set => luck = value;
    }
    public int ShootAngle
    {
        get => shootAngle;
        set => shootAngle = value;
    }
    public Sprite PlayerPortrait { get => playerPortrait; set => playerPortrait = value; }
    public int PointsAvailable { get => pointsAvailable; set => pointsAvailable = value; }
    public int PointsUsed { get => pointsUsed; set => pointsUsed = value; }
    public int Range { get => range; set => range = value; }
    public int Release { get => release; set => release = value; }
    public bool IsFighter { get => isFighter; set => isFighter = value; }
    public bool IsLocked { get; internal set; }
    public bool IsShooter { get => isShooter; set => isShooter = value; }
    public int Clutch { get => clutch; set => clutch = value; }
    public int Userid { get => userid; set => userid = value; }
    public float InAirSpeed { get => inAirSpeed; set => inAirSpeed = value; }

    /// <summary>The authored CPU shooter identity. Read-only: set by the character asset, not
    /// gameplay code. Used by #54's shot-selection policy to break accuracy ties.</summary>
    public CpuBaseStats.ShooterType CpuType => cpuType;
}
