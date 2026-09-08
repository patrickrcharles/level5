# Systems Restructure Plan

Last updated: 2026-08-20 (Phase 2 detailed)

The brief: no ship date, no feature deadline — the game is done when the systems are correct,
structured and maintainable. That removes the usual reason to defer structural work, so this plan
sequences by **leverage** rather than by visible progress.

This complements [Architecture Remediation Plan](architecture-remediation-plan.md), which is a
combat-first, nine-phase plan describing *what* to build. This one describes *what order* to unlock
it in, because several of those phases are currently unverifiable for a structural reason.

## What "enterprise grade" should mean here

One owner per piece of state, one implementation per behaviour, contracts that fail at build time
rather than in a play session, and changes that can be tested before they ship.

It should **not** mean a DI container, an ECS migration, a service-locator layer, or interfaces
introduced ahead of a second implementation. `AGENTS.md` rejects those, and no defect found so far
was caused by missing infrastructure. Each was caused by *two implementations of one thing*, or by
state with no clear owner.

## The measured problem

See [Appendix: how these were measured](#appendix-how-these-were-measured) for exact commands.

| From | To | Refs | Back |
| --- | --- | --- | --- |
| `basketball` | `player` | 19 | 15 |
| `player` | `game manager` | 19 | 15 |
| `basketball` | `game manager` | 16 | 9 |

A three-way cycle: `player` <-> `basketball` <-> `game manager` <-> `player`.
[The shot lifecycle](shot-lifecycle.md) records the `player`/`basketball` half; the measurement shows
`game manager` is the third side, so cutting one edge frees nothing.

Consequences, observed rather than theorised:

- **218 `.cs` files, 4 assembly definitions.** Gameplay lives in the predefined `Assembly-CSharp`,
  which an `.asmdef` cannot reference. Play-mode tests for gameplay therefore live in a folder with
  *no* asmdef — a workaround documented in `Level5GameplayPlayModeTests` itself.
- **~3,700 lines of human/CPU duplicate pairs**, which diverge silently. AUD-001 found one kill
  healing 5/2 down one path and 7/3 down the other; a single ball-visibility bug had to be fixed
  twice, in two files, in one commit.
- **Five controllers move a dynamic Rigidbody with `MovePosition`.** One fixed, four outstanding.
- **124 live scene searches.** A rename fails during play, not during build.

## Sequence

Every phase lands with its invariant asserted in the validator or a test. A phase that cannot be
verified is not done. Every phase leaves the game playable and the suite green.

### Phase 0 — Scene contract inventory

Every later phase depends on scene and prefab wiring, so contract work starts here rather than
waiting for Phase 6. This phase does not remove scene searches. It records what each system a later
phase touches resolves by name, and adds the validator check as that system migrates.

**Exit:** every system scheduled in Phases 1-5 has its required scene and prefab objects declared, in
the `RequiredSceneObjectNames` style the menus now use.

### Phase 1 — Cut the player/basketball/game-manager cycle

The keystone, split into slices because it is the highest-risk system in the repository. Each slice
ships and is verified on its own.

- **1a — Attempt/result contract.** Introduce the shot result travelling one way. No consumer moves.
- **1b — Match-stats owner behind a `GameStats` facade.** `GameStats` and the marker counters
  deliberately remain inline today because they feed scoring and win conditions rather than
  presentation (AUD-010, In Progress). The new owner goes in behind a compatibility facade, and no
  call site changes behaviour in this slice.
- **1c — Migrate consumers one at a time**, each with its own verification pass, including prefab and
  component migration.
- **1d — Invert the `game manager` edge**, then assert direction.

Preserved oddities from [the shot lifecycle](shot-lifecycle.md) are carried explicitly and named in
whichever slice touches them. They are behaviour, not accidents.

**Exit:** the three edges are one-directional, asserted by a dependency test that fails on a new
back-reference so the cycle cannot reform.

### Phase 2 — Assembly split

Blocked on two things, not one. The cycle is the first. The second is that an asmdef must name every
package assembly it uses, and a wrong name is a project-wide compile failure (AUD-012 note,
2026-08-07) — which is why this was never attempted blind.

That second blocker is now resolved. `Library/ScriptAssemblies` shows which references are
asmdef-based and which are precompiled. The 2a canary (below) proved this table rather than
inferring it, and corrected one entry: `Analytics` does not need to be listed. The runtime code that
looked like it needed it (`AnaylticsManager.cs`) uses `UnityEngine.Analytics.*`, which is the
`UnityAnalyticsModule` engine module and is referenced automatically; the asmdef-based `Analytics`
assembly is a separate, Editor-only assembly (`com.unity.analytics`'s Editor window code,
`includePlatforms: ["Editor"]`) that no runtime code actually touches.

| Reference | Kind |
| --- | --- |
| `Unity.InputSystem`, `Unity.Mathematics` | asmdef assemblies — must be listed |
| `Newtonsoft.Json`, `Mono.Data.Sqlite`, `UnityEngine.Analytics` (`UnityAnalyticsModule`) | precompiled or engine — automatic |
| `Unity.IO.LowLevel.Unsafe` | unused — a dead `using` in `AutoPlayerDefense.cs` with no API calls; no production type currently needs it |

**2a result (2026-08-20):** `Level5.AssemblyFeasibility` compiled clean headless on 6000.5.7f1
referencing only `Level5.Core`, `Unity.InputSystem` and `Unity.Mathematics`, while a probe method
also called `Newtonsoft.Json.JsonConvert`, `Mono.Data.Sqlite.SqliteConnection` and
`UnityEngine.Analytics.Analytics.CustomEvent` with no extra reference declared. Full EditMode
(463/463) and PlayMode (9/9) suites passed with the canary present; both passed again after it was
deleted. No production runtime source has moved assemblies yet.

Phase 2 establishes enforceable compile-time boundaries around Level5 runtime code without changing
gameplay, scenes, prefabs, rendering, input behaviour, or public runtime behaviour. It is
structurally invisible: URP assets, renderer data/features, volume profiles, post-processing,
shaders, materials, lighting, cameras, scenes, prefab behaviour and import settings do not change. A
visual difference during verification is a regression.

Phase 2 begins only after the Phase 1d ratchet is in place as the current game-manager boundary
guard. Remaining player↔basketball coupling does not block the 2a spike, but cyclic dependencies may
block specific 2b production assembly boundaries.

#### 2a — Runtime assembly feasibility canary

Do not migrate a production gameplay folder as the spike. Create a temporary, disposable runtime
assembly (e.g. `Level5.AssemblyFeasibility`) whose only purpose is to prove the exact
assembly-reference behaviour the real migration needs:

- runtime assembly, not Editor-only; `autoReferenced: true`; `overrideReferences: false` initially
- references `Level5.Core` where useful, and explicitly references every asmdef-based package
  assembly being proven
- exercises representative public types from those assemblies in compile-only probe code
- exercises representative precompiled/plugin dependencies without explicitly adding them, where
  Unity should auto-reference them

At minimum, verify the package assumptions already recorded above: `Unity.InputSystem`,
`Unity.Mathematics`, the Analytics assembly this project uses, Newtonsoft.Json and
Mono.Data.Sqlite automatic/precompiled reference behaviour, and `Unity.IO.LowLevel.Unsafe` if a
production runtime candidate needs it. Do not add URP, Cinemachine, TMP, Addressables or other
package references merely because the packages exist — only if relevant to a planned production
assembly or needed to verify an unresolved assembly name.

With the canary active: launch the pinned Unity version in batch mode, force script compilation and
fail on any compiler error, run repository validation, and run the full EditMode and PlayMode
suites. Record the exact successful assembly names, whether each dependency is asmdef-based or
precompiled, and any plugin/platform constraints discovered. Then delete the canary and confirm the
project returns to a green baseline.

**2a exit:** headless compilation succeeds with the canary active; package assembly names and
precompiled-reference assumptions are proven rather than inferred; full EditMode and PlayMode suites
are green; the canary is removed; no production runtime source has moved assemblies yet.

#### 2b0 — Production assembly boundary gate

Before adding an asmdef around any production folder, calculate its actual dependency closure.
Build a source-to-assembly dependency graph covering the Level5 runtime code intended for migration.
For every proposed production assembly, determine: source files it owns; other Level5 assemblies it
requires; remaining dependencies on `Assembly-CSharp`; package asmdef references; precompiled/plugin
references; Editor-only dependencies; platform-specific constraints; unsafe-code requirements;
generated-code ownership; serialization/reflection risks. Compute strongly connected components.

A proposed production assembly may be created only when: it does not require a type remaining in
`Assembly-CSharp`; its dependency graph is acyclic; all direct custom-assembly references can be
declared explicitly; runtime source does not depend on an Editor assembly; its recursive folder
ownership is understood.

Do not treat the Phase 1d ratchet as proof the entire original three-way cycle is gone. Remeasure
player→basketball, basketball→player, player→game manager, game manager→player, basketball→game
manager and game manager→basketball specifically. If player and basketball still form a strongly
connected component, do not create separate player and basketball asmdefs yet — prefer the smallest
behaviour-preserving dependency inversion that removes the remaining cycle, and do not create a
permanent coarse `Level5.Gameplay` assembly solely to hide a cyclic design unless that architecture
is chosen for independent reasons.

**Remeasured (2026-08-20), against `dev` at `3c15cd47c` (Phase 1d landed):** still a strongly
connected component. The `game manager → player`/`game manager → basketball` direction is the one
Phase 1d actually addressed, and `Level5GameManagerEdgeTests` (the ratchet) is authoritative for it
rather than a fresh grep: it allowlists exactly 5 files —
`GameLevelManager.cs`, `GameRules.cs`, `MatchHudPresenter.cs`, `SpawnCoordinator.cs`, `Pause.cs` —
each with a documented reason (roster/spawn-identity bookkeeping, a `GameStats` read that is
load-bearing for `getExperienceGainedFromSession()` and the persistence layer, or the deferred
HUD-polling design pass). Reduced from the plan's raw ~68/~46 count, but not zero, and not eliminated
— narrowed and pinned. The reverse direction (`player → game manager`, `basketball → game manager`)
was never in Phase 1d's scope and remains extensive: a rough re-run of the coupling method (declared
types per folder, whole-word matches, comment lines excluded) found non-trivial mentions in both
directions for all three pairs (`player ↔ basketball`, `player ↔ game manager`,
`basketball ↔ game manager`). **Conclusion: player, basketball and game manager remain one strongly
connected component. None of the three gets its own production asmdef in this pass of 2b** — 2b picks
a leaf candidate outside this triangle instead.

Before changing the assembly identity of production types, search for `[SerializeReference]`,
`Type.GetType`, `Assembly.Load`/`Assembly.LoadFrom`, assembly-qualified type-name strings, Newtonsoft
`TypeNameHandling`, custom reflection serializers, and persistence code that records managed type
names. If none exist for the candidate types, record that result; if they do, add migration/
regression coverage before moving those types.

**2b0 exit:** every production asmdef candidate has an explicit, acyclic dependency manifest.

#### 2b — Incremental runtime assembly migration

Migrate runtime code leaf-first. The current top-level folder layout is not assumed to equal the
desired assembly architecture — boundaries follow dependencies and ownership. For each assembly:
choose one dependency-closed candidate; add its asmdef without unrelated refactoring; keep source
paths and `.meta` files stable wherever practical; preserve namespaces, serialized fields and
component contracts; set `autoReferenced: true` and start with `overrideReferences: false`; add only
required asmdef-to-asmdef/package references; compile immediately; run relevant focused tests, then
the full EditMode suite, then the full PlayMode suite; do not begin the next assembly until green.
One assembly boundary should be independently revertible.

**Slice 1 — `Level5.Input` (2026-08-20).** `Assets/Scripts/input` is not a clean leaf as a whole (10
of its 13 files reach into `game manager`/menu/player types); a 3-file subset was
(`PlayerControls.cs` — the generated Input Actions wrapper, `PlayerControlsProvider.cs`,
`PlayerTouchInputState.cs`), referencing only `Unity.InputSystem` and `UnityEngine`. Moved with their
`.meta` files (GUIDs preserved) into a new `Assets/Scripts/input/Level5Input/` sub-folder — the
disqualified 10 files stay put in `Assets/Scripts/input`, still in `Assembly-CSharp`. None of the
three declares a `MonoBehaviour`, so no scene/prefab component reference was at risk; all three are in
the global namespace already, so no consumer call site changed. `Level5.Input.asmdef`:
`autoReferenced: true`, `overrideReferences: false`, references `["Unity.InputSystem"]`. Headless
compile clean; full EditMode 463/463 and PlayMode 9/9 both passed with the new asmdef present;
`validate-repository.ps1` passed. Consumers (`Pause.cs`, `GameLevelManager.cs`,
`PlayerController.cs`, `SniperCameraController.cs`, `RacingGameManager.cs`,
`UserAccountManager.cs`, `ProgressionManager.cs`, and the two `StartScreen*` menu files) needed no
changes — `Assembly-CSharp` auto-references it the same way it already does `Level5.Core`.

**Slice 2 — `Level5.Combat` (2026-08-20).** Same pattern: 7 of `combat`'s 9 files
(`ICombatAgent.cs`, `ICombatDetection.cs`, `IDamageable.cs`, `CombatReservation.cs`,
`CombatTacticalState.cs`, `CombatTargetSelector.cs`, `DamageInfo.cs`) reference only `System`/
`UnityEngine`; the other two (`ActorHealth.cs` — `MatchRuntime.Rules.Hardcore`; `CombatCredit.cs` —
`GameLevelManager`, `PlayerIdentifier`, `GameStats`, `BasketBall`) stay in `Assembly-CSharp`. Moved
into `Assets/Scripts/combat/Level5Combat/` with `.meta` files intact. None of the seven is a
`MonoBehaviour` (two interfaces, two structs, one enum, two static classes) — no scene/prefab
component reference at risk. `Level5.Combat.asmdef`: no references needed at all — pure
`System`/`UnityEngine`. Headless compile clean; full EditMode 463/463 and PlayMode 9/9 passed;
`validate-repository.ps1` passed. Consumed by `enemy`, `bodyguard` and `player`, none of which
needed changes.

**Slices 3-9 (2026-08-20), one verification pass covering seven independent single-domain leaves.**
Each was hand-verified file-by-file (full contents read, not just `using` statements — this project
mostly doesn't use namespaces, so a grep for `using` misses same-namespace/global references) before
moving, after a candidate the automated sweep called clean turned out not to be: `versus/VersusRuntime.cs`
instantiates `FileVersusSeriesRepository`, which calls `AtomicFile.WriteAllText`, and `AtomicFile` is
declared in `Assets/Scripts/player/CharacterProgressStore.cs` — inside the blocked triangle. Dropped
that candidate; kept the following seven, each moved into a new `Level5<Name>/` sub-folder with
`.meta` files intact, `autoReferenced: true`:

| Assembly | Files (leaf subset only) | References |
| --- | --- | --- |
| `Level5.Enemy` | `EnemyAttackBox.cs` (MonoBehaviour, primitive fields only) | none |
| `Level5.PlayerRacing` | `RacingVehicleProfile.cs` (MonoBehaviour, primitive fields only) | none |
| `Level5.Vehicle` | `VehicleMove.cs` (MonoBehaviour, `UnityEngine` only) | none |
| `Level5.MenuProgression` | `MatchProgressionResult.cs` (`[Serializable]`, `System` only, no Inspector exposure found) | none |
| `Level5.Utility` | `LegacyAchievementRecord.cs`, `LegacyNextSceneMarker.cs`, `SceneObjects.cs`, `UtilityFunctions.cs` (uses `PercentChance`) | `Level5.Core` |
| `Level5.Misc` | `ConfirmDialogue.cs`, `FPSDisplay.cs`, `PlayerTips.cs`, `SunglassesCollision.cs`, `TheyLiveManager.cs` (mutually self-contained: `SunglassesCollision` reads `TheyLiveManager.instance`) | none |
| `Level5.Models` | `ServerMessageModel.cs`, `UserReportModel.cs` (`System` only) — `HighScoreModel.cs` in the same folder stays put, still reaching for `GameStats`/`MatchRuntime`/`PlayerIdentifier`/`GameOptions` | none |

None of the MonoBehaviour types here (`EnemyAttackBox`, `RacingVehicleProfile`, `VehicleMove`,
`LegacyAchievementRecord`, `LegacyNextSceneMarker`, `ConfirmDialogue`, `FPSDisplay`,
`SunglassesCollision`, `TheyLiveManager`) is referenced by a `[SerializeField]` on another
still-in-`Assembly-CSharp` type — checked by grep before moving — so no prefab/scene component
reference was put at risk beyond the GUID-preservation the `.meta` move already guarantees. Headless
compile clean; full EditMode 463/463 and PlayMode 9/9 both passed with all seven present;
`validate-repository.ps1` passed. No consumer file needed a change.

**Slice 10 — `Level5.MenuStart` (2026-08-20), and a correction to the automated sweep.** The sweep
that found slices 3-9 also named an 8-file `menu_start` subset as clean
(`CheerleaderProfile.cs`, `EndRoundData.cs`, `LevelCatalog.cs`, `LevelPreset.cs`, two
`player_select/*` files, `StartMenuSelectionState.cs`, `StartMenuUiObjects.cs`). Hand-verifying it
file-by-file (full contents, not a `using`-statement scan — this project mostly doesn't namespace
its code) found half of it wrong: `EndRoundData.cs` holds a `List<LevelSelected>` field;
`LevelPreset.cs` and `LevelCatalog.cs` both take a `LevelSelected` parameter; and `LevelSelected.cs`
itself (`Assets/Scripts/menu_start/LevelSelected.cs`, not in the proposed set) reaches
`cpuPlayer.GetComponent<CharacterProfile>()` — `CharacterProfile` is declared in the blocked `player`
folder, so all three are transitively blocked. `StartMenuSelectionState.cs` reaches `GameOptions`
(`Assets/Scripts/menu_start/GameOptions.cs`, the legacy global config class
`Level5MatchArchitectureTests` is already migrating call sites off) directly. The four
`player_select/*` files reach `CharacterProfile`, `EndRoundData`, `PlayerData` and `LoadedData` —
all blocked. Only `CheerleaderProfile.cs` and `StartMenuUiObjects.cs` survive: both `MonoBehaviour`s
with `UnityEngine`/`UnityEngine.UI`-typed fields only, no other type referenced. Moved into
`Assets/Scripts/menu_start/Level5MenuStart/`; `Level5.MenuStart.asmdef` needs no references. Headless
compile clean; full EditMode 463/463 and PlayMode 9/9 passed; `validate-repository.ps1` passed.

**Correction (code review, 2026-08-21):** `CheerleaderProfile` *is* referenced by `[SerializeField]`
elsewhere - `StartManager.cs`, `LoadedData.cs` and `LoadManager.cs` each hold a
`[SerializeField] private List<CheerleaderProfile> ...` field, split across two lines. The single-
line grep pattern used for slices 3-9 (`SerializeField.*TypeName` on one line) missed this shape.
Still safe: Unity serializes a `List<T>` of `UnityEngine.Object`-derived elements (which
`CheerleaderProfile`, a `MonoBehaviour`, is) by each element's GUID/fileID, not by an
assembly-qualified type name, so moving which assembly the type compiles into doesn't touch the
reference - confirmed independently by a full GUID diff across all 27 moved `.meta` files (unchanged)
and a repo-wide `asm: Assembly-CSharp` grep across every `.unity`/`.prefab`/`.asset` (zero hits). The
finding is a documentation-accuracy correction, not a functional regression: nothing needs to move
back, but "checked by grep before moving" for the earlier slices should be read as "checked with a
same-line pattern," not as a proof of absence for multi-line attribute/field pairs.

The lesson for any further sweep: trust an automated "ruled out" finding (it cites a concrete
disqualifying reference, easy to verify) more than an automated "this is clean" finding (an absence
claim, and the one place it was checked by hand instead of trusted, it missed a two-hop chain through
a getter property). Read full file contents before moving anything, every time.

**Slice 11 — `Level5.Basketball` (2026-09-04), the first leaf out of the former player/basketball/game-manager
triangle.** The 2026-08-20 remeasurement above found player, basketball and game manager still one strongly
connected component. Between then and this slice, a separate run of `AUD-010` Phase 2b0 (PRs #94-#99: binding
`BasketBallShotMarker` to `ResolvedMatchRules`, `BasketBall`/`BasketBallAuto` shot telemetry to a callback,
moving `PickupObject` into `Level5.Misc`, and removing basketball's last live `BehaviorNpcCritical` reference)
cut basketball's outbound edges into `Assembly-CSharp` to zero without moving basketball itself into an asmdef
yet — leaving basketball a clean leaf even though `player` and `game manager` remain coupled to each other and
to basketball's own former call sites. This slice is `AUD-012` Phase 2b: give `Assets/Scripts/basketball`
(12 production `.cs` files plus the ignored `Legacy~/` folder, fully commented out) its own asmdef, source files
left in place.

Re-verified against `dev` at `248648d0b` (PR #99 landed) rather than trusting the 2026-08-20 measurement: every
live (non-comment, non-XML-doc) identifier in `Assets/Scripts/basketball/*.cs` resolves to `Level5.Core`
(including the `Level5.Core.Match` sub-namespace — one asmdef, not a separate assembly), `Level5.Utility`
(`UtilityFunctions.FindDeepChild`/`RollPercent`, the copy under `Utility/Level5Utility/`, not the same-named
loose files still in `Assembly-CSharp`), `Level5.Audio` (`SFXBB`), `Level5.Constants` (`Constants.DISTANCE_*`)
or `Level5.Misc` (`PickupObject`, moved there by PR #98). Every remaining mention of `GameLevelManager`,
`SpawnCoordinator`, `GameRules`, `MatchRuntime`, `AnaylticsManager`, `BehaviorNpcCritical` or `PlayerController`
inside the basketball folder is inside an XML-doc `<see cref>`/`<c>` tag or a commented-out line — grepped
individually, none is live code. Reverse edge also checked (new for this slice, since this is the first
migrated assembly with external `Assembly-CSharp` consumers reaching into it): 32 files outside
`Assets/Scripts/basketball` reference a basketball type (`SpawnCoordinator`, `GameRules`, `GameLevelManager`,
`PlayerController`, `AutoPlayerController`, `versus/*`, `database/*`, etc.) — counted by excluding
basketball's own 13 self-referencing files (12 production files plus the ignored `Legacy~` one) from the
raw 45-file grep match. Basketball declares no `internal`
member (one stale comment mentions a former `internal set`, already gone), no `protected` member, and no
external type inherits from a basketball type — every top-level basketball type is `public` — so none of those
32 external call sites relies on same-assembly visibility the new boundary would break. Assembly-sensitive type
identity checked and clean: no `[SerializeReference]`, `Type.GetType`, `Assembly.Load`/`LoadFrom`,
`AssemblyQualifiedName` or `TypeNameHandling` touches a basketball type anywhere in the repo.

`Level5.Basketball.asmdef`: `autoReferenced: true`, `overrideReferences: false`, references
`["Level5.Core", "Level5.Utility", "Level5.Audio", "Level5.Constants", "Level5.Misc"]` — no `Unity.ugui`
reference needed despite basketball's `UnityEngine.UI` usage, consistent with every other Phase 2 slice's
`autoReferenced` cascade. Headless Unity 6000.5.7f1 compile clean (`Level5.Basketball.dll`, 144 defines, 300
references, zero new `CS` errors; the only warnings anywhere in the log are pre-existing `CS0618
FindObjectsSortMode` obsolete-API notices in unrelated PlayMode/EditMode test files). Added
`BasketballProductionTypesCompileIntoLevel5Basketball` to `Level5ProductionAssemblyBoundaryTests.cs`
(`typeof(BasketBall)`/`typeof(GameStats)`/`typeof(BasketBallShotMarker)`'s assembly name now asserted equal to
`"Level5.Basketball"`); that fixture's two pre-existing text-scan guards already cover `Level5.Basketball`
automatically, since they discover every non-test `.asmdef` under `Assets/Scripts`/`Assets/Level5` at run time
rather than from a hand-maintained list. Focused EditMode run (this fixture only): 3/3 passed. Ran the existing
`Level5BasketBallShotTelemetryCompositionPlayModeTests` fixture — which drives the real
`SpawnCoordinator.GiveBall` → basketball composition path, not a hand-built substitute — as the one real
composition-path check: 1/1 passed. Per this repository's risk-based validation policy, the full EditMode/
PlayMode suites were not re-run for an assembly-boundary-only change with no behavior modification; PR CI owns
that broader regression coverage.

This does **not** close `AUD-012`/Phase 2 as a whole, and does not by itself unblock 2c: `player` and
`game manager` still form a cycle with each other (and still hold the call sites basketball used to reach into,
now removed from basketball's side only), so the asmdef-free `Level5GameplayPlayModeTests` workaround — whose
four files also instantiate `GameStats`/`MatchController`/`PlayerController` directly, not just `BasketBall` —
remains blocked on migrating `player`/`game manager` themselves, unchanged from the 2026-08-20 finding.

**Slice 12 — `Level5.Match` (2026-09-05), one file out of `game manager`.** `MatchController.cs`
(`Assets/Scripts/game manager/MatchController.cs`) is the scene-facing wrapper around `MatchLifecycle`
(`Level5.Core.Match`, already in `Level5.Core`): it owns the `MatchController.instance` singleton, the
`Ending`/`Completed` events, and the first-request-wins `RequestEnd` door, with no lifecycle logic of its
own. Its only live identifiers are `System`, `UnityEngine` and `Level5.Core.Match` types
(`MatchLifecycle`, `MatchPhase`, `MatchEndCause`, `MatchEndReason`); every historical mention of
`GameLevelManager`, `GameRules`, `MatchRuntime`, `LevelRuntimeContext`, a player type or a basketball
type lives only in this file's own XML-doc comments, not in code. That made it a second dependency-closed
leaf out of the game-manager folder even though `player`/`game manager` remain a mutually coupled pair
overall — same shape as Slice 11 pulling `Level5.Basketball` out of the triangle without the triangle
itself being cut.

Consumer accessibility checked against the two production call sites: `GameRules.cs` only holds a
private `MatchController matchController` field and calls `FindAnyObjectByType<MatchController>()` /
`AddComponent<MatchController>()`; `LevelRuntimeContext.cs` only exposes its own field through a public
`MatchController MatchController` property and calls `GetComponent`/`FindAnyObjectByType`. Both use
already-public cross-assembly members, so no visibility change was needed. Assembly-sensitive type
identity checked and clean: no `[SerializeReference]`, `Type.GetType`, `Assembly.Load`/`LoadFrom`,
`AssemblyQualifiedName` or `TypeNameHandling` touches `MatchController` anywhere in the repo; the
Editor-test files that mention it (`Level5BasketballShotMarkerSessionTests.cs`,
`Level5BasketballMoneyBallStateTests.cs`, `Level5BasketballMarkerOwnershipTests.cs`,
`Level5BasketballShotPipelineTests.cs`) do so only in comments. `MatchController.cs.meta`'s GUID
(`07d257b14f484874ab1d0e5ff085d777`) was confirmed before the move and is unchanged after it — verified
by reading the `.meta` post-move rather than assuming `git mv` preserved it.

Moved `MatchController.cs`/`.cs.meta` together into a new `Assets/Scripts/game manager/Level5Match/`
sub-folder (not the `game manager` root, which would recursively pull in every still-coupled file in that
folder). `Level5.Match.asmdef`: `autoReferenced: true`, `overrideReferences: false`, references
`["Level5.Core"]` only. Headless Unity 6000.5.7f1 batch compile clean (`Level5.Match.dll`, 144 defines,
296 references, zero new `CS` errors). Added `MatchControllerCompilesIntoLevel5Match` to
`Level5ProductionAssemblyBoundaryTests.cs` (mirrors `BasketballProductionTypesCompileIntoLevel5Basketball`
for the new leaf); focused run of that fixture: 4/4 passed, including the two pre-existing text-scan
guards which cover `Level5.Match` automatically since they discover every non-test `.asmdef` at run
time. Ran the existing PlayMode singleton coverage
(`Level5GameplayPlayModeTests.ASceneScopedSingletonReleasesItsStaticWhenDestroyed`, which claims and then
destroys a real `MatchController` and asserts the static claims/clears correctly) as the one real
lifecycle check: 1/1 passed. Per this repository's risk-based validation policy, the full EditMode/
PlayMode suites were not re-run for an assembly-boundary-only change with no behavior modification; PR CI
owns that broader regression coverage.

This does **not** close `AUD-012`/Phase 2, and does not unblock 2c: `player` and `game manager` still
form a cycle with each other, and three of the workaround folder's files still instantiate or look up a
type that remains `Assembly-CSharp` directly — `Level5GameplayPlayModeTests.cs` (`VersusRuntime`/
`VersusMatchReporter`/`VersusCatalogs`/`ActiveVersusAttempt` in `Assets/Scripts/versus`, and
`ActiveMatch`/`MatchCatalogs` in `Assets/Scripts/menu_start` — not `GameStats`, which Slice 11 already
moved into `Level5.Basketball`), `BasketballVisibilityTests.cs` and `PlayerMovementPhysicsTests.cs`
(`PlayerController`) — so the asmdef-free workaround is still needed. See the updated 2c status below.

**Slice 13 — `Level5.Versus` (2026-09-06), two files out of `versus`.** `VersusCatalogs.cs` and
`DefaultCompetitiveRulesets.cs` (`Assets/Scripts/versus/`) are the versus folder's only
dependency-closed pair: `VersusCatalogs` loads authored `CompetitiveRulesetDefinition` assets from
`Resources/Versus/Rulesets` and falls back to `DefaultCompetitiveRulesets.CreateAll()` for any id an
asset doesn't cover; `DefaultCompetitiveRulesets` is the code-authored ruleset registry itself. Their
only live identifiers are `System.Collections.Generic`, `UnityEngine`, `Level5.Core.Versus`
(`CompetitiveRulesetCatalog`, `CompetitiveRuleset`, `CompetitiveRulesetDefinition`,
`VersusDomainException`, `RulesetId`, `ComparisonKey`, `AttemptMetric`, `VersusCapability`) and
`Level5.Core.Match` (`GameModeId`) — all already in `Level5.Core` — plus each other
(`VersusCatalogs.Build()` calls `DefaultCompetitiveRulesets.CreateAll()`). Every other file in the
folder (`ActiveVersusAttempt`, `FileVersusSeriesRepository`, `VersusRuntime`, `VersusLauncher`,
`VersusMatchReporter`, `GameStatsAttemptResults`) still reaches `ActiveMatch`, `AtomicFile`, or each
other and stayed in `Assembly-CSharp`, unchanged from the Phase 2b0 finding — consistent with Slice
11 and 12 pulling a single dependency-closed leaf out of a still-coupled folder rather than cutting
the folder itself.

Consumer accessibility checked against the three production call sites: `VersusRuntime.cs` reads
`VersusCatalogs.Rulesets` (public static property); `Assets/Scripts/Dev/VersusDevConsole.cs` calls
`VersusCatalogs.Rulesets.Supporting(...)`; `Assets/Tests/PlayModeGameplay/Level5GameplayPlayModeTests.cs`
(the asmdef-free 2c workaround) calls `VersusCatalogs.Reset()`. All three already used public
cross-assembly members, so no visibility change was needed. Assembly-sensitive type identity checked
and clean: no `[SerializeReference]`, `Type.GetType`, `Assembly.Load`/`LoadFrom`, `AssemblyQualifiedName`
or `TypeNameHandling` touches either type anywhere in the repo, and neither ruleset ids, versions, nor
any other persisted versus data are keyed on a type's assembly. `VersusCatalogs.cs.meta`'s GUID
(`f623cb04469457b4c832cbf29a2430d2`) and `DefaultCompetitiveRulesets.cs.meta`'s GUID
(`10289957d0f3a8143bfc06db00ac7753`) were confirmed before the move and unchanged after it, verified by
reading both `.meta` files post-move.

Moved both files/`.meta`s together into a new `Assets/Scripts/versus/Level5Versus/` sub-folder (not the
`versus` root, which is not dependency-closed). `Level5.Versus.asmdef`: `autoReferenced: true`,
`overrideReferences: false`, references `["Level5.Core"]` only — no reference to any other Phase 2b
leaf. Headless Unity 6000.5.7f1 batch compile clean (`Level5.Versus.dll`, 144 defines, 296 references,
zero new `CS` errors; the only warnings in the log are the same pre-existing `CS0618`
`FindObjectsSortMode`/`FindFirstObjectByType` obsolete-API notices and one pre-existing `CS0414` in
`GameRules.cs` seen in prior slices' logs). Added `VersusCatalogTypesCompileIntoLevel5Versus` to
`Level5ProductionAssemblyBoundaryTests.cs` (mirrors `MatchControllerCompilesIntoLevel5Match`); focused
run of that fixture plus the existing `Level5VersusRulesetTests` fixture (ruleset ids, versions,
capabilities, comparison ordering, and `DefaultCompetitiveRulesets`/catalog fallback behavior) passed
with no changes needed to either file — `Level5VersusRulesetTests` already exercised
`DefaultCompetitiveRulesets.CreateAll()` and the shipped registry directly, so this slice is proof the
same code now compiles into a different assembly with identical behavior. Per this repository's
risk-based validation policy, the full EditMode/PlayMode suites were not re-run for an
assembly-boundary-only change with no behavior modification; PR CI owns that broader regression
coverage.

This does **not** close `AUD-012`/Phase 2, and does not unblock 2c on its own: `Level5GameplayPlayModeTests.cs`
now resolves `VersusCatalogs.Reset()` through `Level5.Versus` (`autoReferenced`) instead of
`Assembly-CSharp`, but the same file still reaches `VersusRuntime`, `VersusMatchReporter` and
`ActiveVersusAttempt` directly, all three still `Assembly-CSharp` — so it stays on the blocked list.
See the updated 2c status below.

**Slice 14 — `AtomicFile` into the existing `Level5.Utility` (2026-09-06), one type out of
`CharacterProgressStore.cs`.** `AtomicFile` was a second, unrelated top-level `public static class`
declared at the bottom of `Assets/Scripts/player/CharacterProgressStore.cs` — a dependency-closed
filesystem primitive (temp-file write, `File.Replace`, `.bak` fallback on read) with no player or
gameplay dependency, used by `CharacterProgressStore` itself and, directly, by
`PendingMatchPersistenceStore`, `ProgressionResultStore`, `PendingProgressionStore` and
`FileVersusSeriesRepository`. Its own source used only `System`/`System.IO`. Type-identity checked
and clean: no `[SerializeReference]`, `Type.GetType`, `Assembly.Load`/`LoadFrom`, or
`AssemblyQualifiedName` touches it anywhere in the repo, and no persisted data is keyed on its
assembly.

Moved the class body verbatim into a new `Assets/Scripts/Utility/Level5Utility/AtomicFile.cs` (new
`.meta`, fresh GUID — there was no prior GUID to preserve since it was never its own asset) and
deleted it from the bottom of `CharacterProgressStore.cs`, which otherwise keeps its own file, `.meta`,
and behavior untouched. Global type name and public API (`TryReadAllText` both overloads,
`WriteAllText`) are unchanged, so every consumer — inside and outside `Assembly-CSharp` — kept
compiling with no call-site edits; `Level5.Utility` is already `autoReferenced: true`, so this needed
no manifest change. Headless Unity 6000.5.7f1 batch compile clean (`Level5.Utility.dll`, 144 defines,
296 references, zero new `CS` errors; same pre-existing `CS0618`/`CS0414` warnings as prior slices'
logs). Added `AtomicFileCompilesIntoLevel5Utility` to `Level5ProductionAssemblyBoundaryTests.cs`
(mirrors `MatchControllerCompilesIntoLevel5Match`); focused run of that fixture (6/6) plus the existing
`Level5CoreTests` `AtomicFile` backup/recovery/malformed-primary coverage (2/2) passed unchanged. Per
this repository's risk-based validation policy, the full EditMode/PlayMode suites were not re-run for
an assembly-boundary-only change with no behavior modification; PR CI owns that broader regression
coverage.

Post-extraction remeasurement of `FileVersusSeriesRepository` and `VersusRuntime` (`Assets/Scripts/versus/`),
named in Slice 13's finding as blocked on `AtomicFile`: with the `AtomicFile` edge gone,
`FileVersusSeriesRepository`'s only remaining live identifiers are `System`/`System.IO`, `UnityEngine`,
and `Level5.Core.Versus`/`Level5.Core.Versus.Persistence` (`IVersusSeriesRepository`,
`VersusSeriesSerializer`, `VersusSeriesDocument`, `VersusLog`) — all already outside `Assembly-CSharp` —
making it dependency-closed on its own for the first time. `VersusRuntime` still reaches
`FileVersusSeriesRepository` itself (`Assembly-CSharp`) plus `VersusCatalogs` (`Level5.Versus`) and
`VersusMatchCoordinator`/`IVersusSeriesRepository` (`Level5.Core.Versus`, both already migrated), so it
remains blocked only on `FileVersusSeriesRepository`, not on anything this slice didn't already clear —
the two are now a closed pair that could migrate together in a future slice. This is recorded as a
finding only; neither file is moved in this slice.

This does not close `AUD-012`/Phase 2, and does not unblock 2c on its own — see the updated 2c status
below.

**Slice 15 — `Level5.Versus` (2026-09-06), two more files out of `versus`.** `FileVersusSeriesRepository.cs`
and `VersusRuntime.cs` (`Assets/Scripts/versus/`) are the pair Slice 14's remeasurement identified as
newly dependency-closed once `AtomicFile` moved to `Level5.Utility`. Re-read both in full before moving
them: `FileVersusSeriesRepository`'s only live identifiers are `System`, `System.Collections.Generic`,
`System.IO`, `UnityEngine`, `Level5.Core.Versus`/`Level5.Core.Versus.Persistence`
(`IVersusSeriesRepository`, `VersusSeries`, `SeriesId`, `SeriesSummary`, `VersusSeriesDocument`,
`VersusSeriesSerializer`, `VersusLog`) and `AtomicFile` — all already outside `Assembly-CSharp`.
`VersusRuntime` reaches only `Level5.Core.Versus` (`IVersusSeriesRepository`, `VersusMatchCoordinator`,
`CompetitiveRulesetCatalog`), `FileVersusSeriesRepository` itself, and `VersusCatalogs` — the last two
already confirmed in `Level5.Versus`/moving into it together. Both closed with no remaining
`Assembly-CSharp` edge; the expected graph (`Level5.Versus` -> `Level5.Core`, `Level5.Utility` -> `Level5.Core`,
no reverse edge) held, confirmed by re-reading `Level5.Utility.asmdef` showing only a `Level5.Core`
reference.

Consumer accessibility checked against every current production and test call site (`Assets/Scripts/versus/VersusLauncher.cs`,
`Assets/Scripts/versus/VersusMatchReporter.cs`, `Assets/Scripts/Dev/VersusDevConsole.cs`,
`Level5VersusArchitectureTests.cs`, `Level5VersusPersistenceTests.cs`, `Level5VersusIntegrationTests.cs`,
`Level5GameplayPlayModeTests.cs`): every reference is `VersusRuntime.Coordinator`/`.Override`/`.Reset`
or `new FileVersusSeriesRepository(...)`, all already public (`VersusRuntime.Repository` has no current
call site). No visibility change was needed.
Assembly-sensitive type identity checked and clean: no `[SerializeReference]`, `Type.GetType`,
`Assembly.Load`/`LoadFrom`, `AssemblyQualifiedName`, or `TypeNameHandling` touches either type anywhere
in the repo; stored versus JSON holds `VersusSeriesDocument` domain data only, never a repository or
runtime type name. `FileVersusSeriesRepository.cs.meta`'s GUID (`bea14fe6bcc6a6348a74a42ab8bffd44`) and
`VersusRuntime.cs.meta`'s GUID (`be5c8fefbb1960c4c922eae7c2c93a96`) were confirmed before the move and
unchanged after it, verified by reading both `.meta` files post-move.

Moved both files/`.meta`s together into the existing `Assets/Scripts/versus/Level5Versus/` sub-folder
(the same one Slice 13 created), source bodies unchanged. Added `Level5.Utility` to
`Level5.Versus.asmdef`'s `references` (now `["Level5.Core", "Level5.Utility"]`) since
`FileVersusSeriesRepository` calls `AtomicFile.WriteAllText`; confirmed no reverse reference from
`Level5.Utility.asmdef` back to `Level5.Versus`. Headless Unity 6000.5.7f1 batch compile clean
(`Level5.Utility.dll` unchanged at 144 defines/296 references; `Level5.Versus.dll` now 144 defines/297
references — the one new edge to `Level5.Utility` — zero new `CS` errors, same pre-existing
`CS0618`/`CS0414` warnings as prior slices' logs). Added `VersusRuntimeTypesCompileIntoLevel5Versus` to
`Level5ProductionAssemblyBoundaryTests.cs` (mirrors `VersusCatalogTypesCompileIntoLevel5Versus`); focused
EditMode runs passed unchanged: the boundary fixture (7/7, including the new test), `Level5VersusPersistenceTests`
(21/21, including `FileRepositoryWritesAndReloadsSerializedSeries` and
`FileRepositoryPreservesArchiveFlagAcrossLaterSave`), and `Level5VersusIntegrationTests` (8/8, including
`TwoRealMatchesResolveAGameThroughTheWholeStack`, which exercises `VersusRuntime.Override` with an
in-memory repository across the new assembly boundary). Per this repository's risk-based validation
policy, the full EditMode/PlayMode suites were not re-run for an assembly-boundary-only change with no
behavior modification; PR CI owns that broader regression coverage.

This does not close `AUD-012`/Phase 2, and does not unblock 2c on its own — see the updated 2c status
below.

**Slice 16 — `Level5.Versus` (2026-09-06), one more file out of `versus`.** `GameStatsAttemptResults.cs`
(`Assets/Scripts/versus/`) is the gameplay-to-versus result mapper: the one place that reads a finished
match's `GameStats` component and turns it into an `AttemptResult`. Re-read it in full before moving it:
its only live identifiers are `Level5.Core.Match` (`GameModeId`), `Level5.Core.Versus` (`AttemptResult`,
`AttemptMetric`, `RulesetId`) and `GameStats` itself — the last one still `Level5.Basketball` since Slice
11, not `Assembly-CSharp`. `Level5.Basketball.asmdef` was re-read and confirmed to hold no reference back
to `Level5.Versus`, so adding the new edge keeps the custom-assembly graph acyclic (`Level5.Versus ->
Level5.Basketball`, no reverse edge).

Consumer accessibility checked against the file's one production call site, `VersusMatchReporter.cs`
(`Assets/Scripts/versus/`, still `Assembly-CSharp`): it calls the public static `GameStatsAttemptResults.Build(...)`,
already public, so no visibility change was needed — `VersusMatchReporter` keeps compiling against the
moved mapper through the assembly boundary, `Level5.Versus` being `autoReferenced`. Assembly-sensitive
type identity checked and clean: no `[SerializeReference]`, `Type.GetType`, `Assembly.Load`/`LoadFrom`,
`AssemblyQualifiedName`, or `TypeNameHandling` touches the type anywhere in the repo, and no ruleset id,
ruleset version, or persisted versus data is keyed on its assembly. `GameStatsAttemptResults.cs.meta`'s
GUID (`7178539eef0d6f748b2d85607518dc34`) was confirmed before the move and unchanged after it, verified
by reading the `.meta` file post-move.

Moved the file/`.meta` together into the existing `Assets/Scripts/versus/Level5Versus/` sub-folder,
source body unchanged — no mapping, metric, shot-counter, completion-time-fallback, or null-stats logic
was touched. Added `Level5.Basketball` to `Level5.Versus.asmdef`'s `references` (now `["Level5.Core",
"Level5.Utility", "Level5.Basketball"]`) since the mapper's method signature takes a `GameStats`
parameter directly. Headless Unity 6000.5.7f1 batch compile clean, zero new `CS` errors. Added
`GameStatsAttemptResultsCompilesIntoLevel5Versus` to `Level5ProductionAssemblyBoundaryTests.cs` (mirrors
`VersusRuntimeTypesCompileIntoLevel5Versus`); focused EditMode runs passed unchanged: the boundary
fixture (8/8, including the new test) and `Level5VersusIntegrationTests` (10/10, including
`AMatchsStatsBecomeAComparableResult`, `AMakeCountModeIsMeasuredOnItsOwnShotRatherThanOnEveryShot`,
`AMatchWithNoStatsStillProducesAResultSoTheSeriesCanMoveOn`, and
`TwoRealMatchesResolveAGameThroughTheWholeStack`, which exercises the moved mapper end to end through
`VersusMatchReporter` across the new assembly boundary) — 18/18 combined in one filtered run confirming
both fixtures compile and pass across the new assembly boundary. Per this repository's risk-based
validation policy, the full EditMode/PlayMode suites were not re-run for an assembly-boundary-only
change with no
behavior modification; PR CI owns that broader regression coverage.

This does not close `AUD-012`/Phase 2, and does not unblock 2c on its own — see the updated 2c status
below. The earlier Phase 2 statement (Slice 13) describing `GameStatsAttemptResults` as staying in
`Assembly-CSharp` "unchanged from the Phase 2b0 finding" is now superseded by this slice; no other
current Phase 2 text describes it as structurally blocked.

Post-migration remeasurement of `VersusMatchReporter.cs` (`Assets/Scripts/versus/`), the mapper's one
consumer: with the `GameStatsAttemptResults` edge now resolving through `Level5.Versus`, its remaining
live identifiers are `System`, `Level5.Core.Match` (`GameModeId`), `Level5.Core.Versus` (`AttemptResult`,
`SeriesId`, `AttemptId`, `ParticipantId`, `SubmissionOperation`, `VersusValidationCode`), `UnityEngine`,
`VersusRuntime`/`GameStatsAttemptResults` (both already `Level5.Versus`), and `GameStats` (already
`Level5.Basketball`) — all outside `Assembly-CSharp` except one: `ActiveVersusAttempt`
(`Assets/Scripts/versus/`), read for `IsActive`/`SeriesId`/`AttemptId`/`ParticipantId` and cleared on
every exit path. `ActiveVersusAttempt` is `VersusMatchReporter`'s only remaining `Assembly-CSharp`
blocker, confirmed by re-reading the file rather than assumed. This is recorded as a finding only;
`VersusMatchReporter` and `ActiveVersusAttempt` are not moved in this slice.

**Slice 17 — `Level5.Match` (2026-09-07), `MatchSession` becomes the actual owner of the current match
result id and moves into `Level5.Match`.** `GameOptions.matchResultId` (`Assets/Scripts/menu_start/GameOptions.cs`)
was a documented-but-unmigrated ownership entry (`docs/game-options-inventory.md`: `matchResultId ->
MatchSession`) — `MatchSession` already mediated every read/write of the field, but the field itself
was still the backing store. Re-checked every production reference to `GameOptions.matchResultId`
before touching it: only `MatchSession.cs` read or wrote it (`Assets/Scripts/menu_progression/MatchSession.cs`,
before the move); the one other hit was a test (`Level5CoreTests.cs`) saving/restoring it around
`MatchSession.BeginNewMatch()` calls. No serialized asset, save payload, reflection path, or
assembly-qualified type string depends on `MatchSession` living in `Assembly-CSharp`.

`MatchSession` now holds a private `static string currentResultId` and generates ids itself via a new
`MatchSession.CreateResultId(string prefix)`, moved out of `ProgressionService.CreateResultId` — the
same `<prefix>-<Guid.NewGuid():N>` construction, same null/empty-prefix-becomes-`"match"` rule, unchanged.
`ProgressionService.CreateResultId` stays as a one-line compatibility delegate onto
`MatchSession.CreateResultId` rather than being deleted: it still has a live caller inside
`ProgressionService.ApplyMatchResult` itself (the null-result fallback path) and a direct test
(`Level5CoreTests.ResultIdsAreUniqueAndKeepTheirPrefix`). There is exactly one id-generation
implementation after this slice, on `MatchSession`; `ProgressionService`'s progression-application,
idempotency, pending-queue, repair, database-access, and JSON-projection behavior were not touched.

`BeginNewMatch()`/`EnsureCurrentMatch()` keep their exact prior contracts (always-fresh id / return-if-present-else-create),
now reading and writing `currentResultId` directly instead of `GameOptions.matchResultId`. No reset
method, setter, or other test-only hook was added to `MatchSession` — `Level5CoreTests.MatchSessionRotatesResultIdForEachGameplayLoad`
was simplified to drop its now-unnecessary save/restore of the deleted field rather than gaining a
replacement hook; static state persisting across test runs was already true of the pre-migration
field and no other test depends on a reset value (confirmed by re-checking every `MatchSession`/`EnsureCurrentMatch`/`BeginNewMatch`
reference under `Assets/Tests`).

`GameOptions.matchResultId` is deleted outright — it had exactly one production consumer and that
consumer no longer needs it. `MatchSession.cs` came off `Level5MatchArchitectureTests`'s
`LegacyGameOptionsConsumers` allowlist (it no longer reaches for `GameOptions` at all), and
`GameOptionsGrowsNoNewMatchFields`'s ratchet was lowered from 60 to 59 — measured directly off the
current file (71 raw `static public`/`public static` matches, minus 9 that are commented-out fields
already excluded by `StripComments`, minus 2 methods = 60 before this slice, 59 after removing the
one field), not assumed.

Moved `MatchSession.cs`/`.cs.meta` (`git mv`, preserving history) from `Assets/Scripts/menu_progression/`
into `Assets/Scripts/game manager/Level5Match/`, alongside `MatchController.cs`. GUID
(`3f26ad31a7a14f56a2c5e31f0ee8c5c1`) confirmed unchanged post-move by re-reading the `.meta` file.
Global namespace, type name, and public API (`BeginNewMatch`, `EnsureCurrentMatch`, the new
`CreateResultId`) preserved. `MatchSession` no longer references `GameOptions` or `ProgressionService`
at all — only `System` (`Guid`) — so the existing `Level5.Match.asmdef` (`references: ["Level5.Core"]`)
needed no change; confirmed by the headless compile showing `Level5.Match.dll` still at 144
defines/296 references, unchanged from Slice 12's boundary creation. Headless Unity 6000.5.7f1 batch
compile clean, zero new `CS` errors. Added `MatchSessionCompilesIntoLevel5Match` to
`Level5ProductionAssemblyBoundaryTests.cs` (mirrors `MatchControllerCompilesIntoLevel5Match`); focused
EditMode runs passed unchanged: the boundary fixture, `Level5MatchArchitectureTests` (including
`GameOptionsGrowsNoNewMatchFields`, `TheAllowlistHasNoStaleEntries`, `NoNewFileReachesForGameOptions`),
and the result-id coverage in `Level5CoreTests` (`ResultIdsAreUniqueAndKeepTheirPrefix`,
`MatchSessionRotatesResultIdForEachGameplayLoad`) — 37/37 assertions across the three fixtures, zero
failures. Per this repository's risk-based validation policy, the full EditMode/PlayMode suites were
not re-run for an ownership-migration-plus-assembly-boundary change with no externally observable
lifecycle behavior change; PR CI owns that broader regression coverage.

This does not close `AUD-012`/Phase 2, and does not unblock 2c on its own — see the updated 2c status
below.

Post-migration remeasurement of `ActiveMatch.cs` (`Assets/Scripts/menu_start/`), `MatchSession`'s
main production caller: re-read in full, its only live identifiers are `UnityEngine`,
`Level5.Core.Match` (`MatchConfiguration`, `LevelDefinition`, `MatchConfigurationBuilder`), and
`MatchSession` — all now outside `Assembly-CSharp`. `ActiveMatch` has zero remaining `Assembly-CSharp`
dependency after this slice and is eligible for the next Phase 2b slice. This is a diagnostic finding
only; `ActiveMatch` is not moved in this PR, and neither are `ActiveVersusAttempt`,
`VersusMatchReporter`, `MatchCatalogs`, or `PlayerController`.

**Slice 18 — `Level5.Match` (2026-09-07), `ActiveMatch` moves into the assembly whose eligibility
Slice 17's remeasurement found.** `ActiveMatch.cs` (`Assets/Scripts/menu_start/`) is the authoritative
current-match configuration: a launch source calls `Begin` immediately before the scene load, and every
gameplay/versus/menu consumer downstream reads it. Re-read it in full before moving it: its only live
identifiers are `UnityEngine`, `Level5.Core.Match` (`MatchConfiguration`, `LevelDefinition`,
`MatchConfigurationBuilder`), and `MatchSession` — the last one already `Level5.Match` since Slice 17.
No live dependency on a type remaining in `Assembly-CSharp` was found, confirming Slice 17's
remeasurement.

Consumer accessibility checked against every production call site (`VersusLauncher.cs`, `EndRoundMenuManager.cs`,
`StartManager.cs`, `Utility/LoadGame.cs`, `game manager/MatchRuntime.cs`, `versus/ActiveVersusAttempt.cs`):
all of them call only the existing public members (`Begin`, `Configuration`, `IsActive`, `ContinueInLevel`,
`Clear`), so no visibility change was needed — every consumer keeps compiling against the moved type
through the assembly boundary, `Level5.Match` being `autoReferenced`. Assembly-sensitive type identity
checked and clean: no `[SerializeReference]`, `Type.GetType`, `Assembly.Load`/`LoadFrom`,
`AssemblyQualifiedName`, or `TypeNameHandling` touches `ActiveMatch` anywhere in the repo, and — since
`ActiveMatch` is a static non-component type — no scene/prefab component serialization applies either.
`ActiveMatch.cs.meta`'s GUID (`153b6c6eb3113bf4080936a0c383776a`) was confirmed before the move and
unchanged after it, verified by reading the `.meta` file post-move.

Moved `ActiveMatch.cs`/`.cs.meta` (`git mv`, preserving history) from `Assets/Scripts/menu_start/` into
`Assets/Scripts/game manager/Level5Match/`, alongside `MatchController.cs`/`MatchSession.cs` — a pure
assembly-ownership move, source body byte-identical. `Configuration`'s exact stored-reference return,
`IsActive`'s `configuration != null` check, `Begin`'s null-logs/assignment-then-`MatchSession.BeginNewMatch()`
ordering, `ContinueInLevel`'s fresh-`MatchConfigurationBuilder.Resolve` reconstruction (mode/roster/modifiers/
cheerleader/source carried over, new level substituted in), and `Clear`'s configuration-only reset were all
left untouched. In particular, the reference-equality contract `ActiveVersusAttempt` depends on
(`ReferenceEquals(ActiveMatch.Configuration, launchedFor)`, protecting an abandoned competitive attempt from
capturing the next ordinary match) is unaffected: `Configuration` still returns the exact stored object, no
clone or rebuild was introduced. `Level5.Match.asmdef`'s `references` needed no change (`ActiveMatch`'s only
custom-assembly dependency, `Level5.Core.Match`, was already declared) — confirmed by the headless compile
showing `Level5.Match.dll` still at 144 defines/296 references, unchanged from Slice 17. Headless Unity
6000.5.7f1 batch compile clean, zero new `CS` errors. Added `ActiveMatchCompilesIntoLevel5Match` to
`Level5ProductionAssemblyBoundaryTests.cs` (mirrors `MatchSessionCompilesIntoLevel5Match`); focused EditMode
runs passed unchanged: the boundary fixture (10/10, including the new test) and
`Level5VersusIntegrationTests` (10/10, including `AnAbandonedTurnDoesNotCaptureTheNextOrdinaryMatch`, which
exercises the reference-identity protection directly across the new assembly boundary). Per this
repository's risk-based validation policy, the full EditMode/PlayMode suites were not re-run for an
assembly-boundary-only change with no behavior modification; PR CI owns that broader regression coverage.

This does not close `AUD-012`/Phase 2, and does not unblock 2c on its own — see the updated 2c status
below.

Post-migration remeasurement of `ActiveVersusAttempt.cs` (`Assets/Scripts/versus/`), the counterpart type
that reads `ActiveMatch.Configuration` for its reference-identity check: re-read in full, its only live
identifiers are `UnityEngine`, `Level5.Core.Match` (`MatchConfiguration`), `Level5.Core.Versus`
(`SeriesId`, `AttemptId`, `ParticipantId`, `RulesetId`, `Attempt`), and `ActiveMatch` — now `Level5.Match`
instead of `Assembly-CSharp` after this slice. `ActiveVersusAttempt` has zero remaining `Assembly-CSharp`
dependency and is eligible for a future Phase 2b slice into `Level5.Versus`, which would need
`Level5.Versus.asmdef` to add a `Level5.Match` reference (`Level5.Versus -> Level5.Match`, no reverse edge,
so the custom-assembly graph would stay acyclic). This is a diagnostic finding only; `ActiveVersusAttempt`
is not moved in this PR, and neither are `VersusMatchReporter`, `MatchCatalogs`, or `PlayerController`.

**Slice 19 — `Level5.Versus` (2026-09-07), the pair Slice 18's remeasurement found dependency-closed.**
`ActiveVersusAttempt.cs` and `VersusMatchReporter.cs` (`Assets/Scripts/versus/`) are the versus-side
counterpart to `ActiveMatch`: the former tracks which competitive attempt, if any, the loaded match is
playing (and the `ReferenceEquals(ActiveMatch.Configuration, launchedFor)` check protecting an
abandoned attempt from capturing the next ordinary match); the latter is the entire footprint versus
has inside gameplay, the one call `GameRules` makes at match end. Re-read both in full before moving
them: `ActiveVersusAttempt`'s only live identifiers are `UnityEngine`, `Level5.Core.Match`
(`MatchConfiguration`), `Level5.Core.Versus` (`SeriesId`, `AttemptId`, `ParticipantId`, `RulesetId`,
`Attempt`), and `ActiveMatch` — `Level5.Match` since Slice 18. `VersusMatchReporter`'s only live
identifiers are `System`, `UnityEngine`, `Level5.Core.Match`, `Level5.Core.Versus`, `GameStats`
(`Level5.Basketball`), `GameStatsAttemptResults`/`VersusRuntime` (`Level5.Versus`), and
`ActiveVersusAttempt`, moved in this same slice. Neither has a live dependency on a type remaining in
`Assembly-CSharp`, confirming Slice 18's finding.

Consumer accessibility checked against every production call site (`VersusLauncher.cs`'s
`ActiveVersusAttempt.Begin(...)`, `game manager/GameRules.cs`'s `VersusMatchReporter.TryReport(...)`):
both call only existing public members, so no visibility change was needed. Assembly-sensitive type
identity checked and clean: no `[SerializeReference]`, `Type.GetType`, `Assembly.Load`/`LoadFrom`,
`AssemblyQualifiedName`, or `TypeNameHandling` touches either type anywhere in the repo, and — since
both are static non-component types — no scene/prefab component serialization applies either.
`ActiveVersusAttempt.cs.meta`'s GUID (`33c21ba9a178bae4f8b12309f22a8f3c`) and
`VersusMatchReporter.cs.meta`'s GUID (`b14974f9aa1601140bc1b18fe10e35a3`) were confirmed before the
move and unchanged after it, verified by reading both `.meta` files post-move.

Moved both `.cs`/`.cs.meta` pairs (`git mv`, preserving history) from `Assets/Scripts/versus/` into
`Assets/Scripts/versus/Level5Versus/`, alongside the versus files already there — a pure
assembly-ownership move, source bodies byte-identical. `ActiveVersusAttempt`'s `SeriesId`/`AttemptId`/
`ParticipantId`/`RulesetId`/`RulesetVersion` properties, private `launchedFor`, `Begin`'s
invalid-input logging/no-op and valid-input capture, `IsActive`'s reference-equality check against
`ActiveMatch.Configuration`, and `Clear`'s full reset were all left untouched. `VersusMatchReporter`'s
no-active-attempt no-op, successful-submission clear-and-return-true, `PersistenceFailed`
leave-in-place-and-return-false retry path, other-refusal log-clear-and-return-true path, and
exception log-clear-and-return-true path were all left untouched — control flow, logging, and retry
policy unchanged. `Level5.Versus.asmdef` gained one new reference, `Level5.Match` (`ActiveVersusAttempt`'s
only custom-assembly dependency not already declared); `Level5.Match.asmdef` was re-checked and still
does not reference `Level5.Versus`, so the new edge (`Level5.Versus -> Level5.Match`) keeps the
custom-assembly graph acyclic. Headless Unity 6000.5.7f1 batch compile clean, zero new `CS` errors.
Added `VersusMatchStateTypesCompileIntoLevel5Versus` to `Level5ProductionAssemblyBoundaryTests.cs`
(mirrors `ActiveMatchCompilesIntoLevel5Match`); focused EditMode runs passed unchanged: the boundary
fixture plus `Level5VersusIntegrationTests` plus `Level5VersusArchitectureTests` together (31/31,
including the new test, and `TheReporterDoesNothingAtAllWhenTheMatchIsNotPartOfASeries`/
`AFailedSaveLeavesTheAttemptInPlaceSoTheMatchEndLoopRetriesIt`/
`ARefusedSubmissionReleasesTheAttemptRatherThanRetryingForever`/
`AnAbandonedTurnDoesNotCaptureTheNextOrdinaryMatch`, which exercise the no-op, retry, refusal-release,
and reference-identity contracts directly across the new assembly boundary). Per this repository's
risk-based validation policy, the full EditMode/PlayMode suites and
`TwoRealMatchesResolveAGameThroughTheWholeStack` were not re-run for an assembly-boundary-only change
with no behavior modification and no focused-test uncertainty; PR CI owns that broader regression
coverage.

This does not close `AUD-012`/Phase 2, and does not unblock 2c on its own — see the updated 2c status
below.

**Slice 20 — `Level5.Match` (2026-09-07), `MatchCatalogs` splits into a clean runtime owner and a
legacy composition seam, and the runtime owner moves.** Unlike slices 12-19, `MatchCatalogs.cs`
(`Assets/Scripts/menu_start/`) was not dependency-closed as it stood: its public `EnsureBuilt`
accepted `IReadOnlyList<StartScreenModeSelected>`/`IReadOnlyList<LevelSelected>` directly and called
`GameModeDefinitionFactory`/`LevelDefinitionFactory` itself, and an unused `EnsureBuiltFromLoadedData()`
depended on `LoadedData` — three `Assembly-CSharp` types a `Level5.Match` asmdef cannot legally
reference. Freshly searched every caller of `EnsureBuilt`, `EnsureBuiltFromLoadedData`, `Override`,
`Reset`, `Builder`, `Compatibility`, `IsReady`, `Modes`, and `Levels` repository-wide before touching
anything: `StartManager.cs`'s `getLoadedData()` was confirmed the only production caller of
`EnsureBuilt(modeSources, levelSources)`; `EnsureBuiltFromLoadedData()` had zero callers anywhere in
the repository (dead since it was written); `Override`/`Reset` are called only by
`Level5VersusIntegrationTests.cs` and `Level5GameplayPlayModeTests.cs`; `Builder`/`Compatibility`/
`IsReady` are called only by `StartManager.cs` and `VersusLauncher.cs`, all through types already in
`Level5.Core.Match`.

The split: `MatchCatalogs` keeps every runtime concern (authored-Resources precedence, the mode/level
catalogs, `GameModeCompatibility`, `MatchConfigurationBuilder`, the independent per-catalog reference
cache, `ConversionAnomalies`, problem reporting, `IsReady`/`Modes`/`Levels`/`Compatibility`/`Builder`/
`Override`/`Reset`) but its `EnsureBuilt` signature changes to
`EnsureBuilt(IReadOnlyList<GameModeDefinition> fallbackModes, IReadOnlyList<LevelDefinition>
fallbackLevels, IReadOnlyList<string> fallbackModeAnomalies = null)` — every parameter a
`Level5.Core.Match` type. `EnsureBuiltFromLoadedData()` was deleted rather than carried forward: no
repository evidence of a caller, so retaining an `Assembly-CSharp` `LoadedData` dependency for it would
have been dead weight, not a preserved contract. A new type, `LegacyMatchCatalogBootstrap`
(`Assembly-CSharp`, `Assets/Scripts/menu_start/`), is the only thing that still converts
`StartScreenModeSelected`/`LevelSelected` through the existing `GameModeDefinitionFactory`/
`LevelDefinitionFactory` — it duplicates none of that conversion logic, it just calls it — and hands
`MatchCatalogs` the resulting typed definitions. `StartManager.cs`'s single call site now reads
`LegacyMatchCatalogBootstrap.EnsureBuilt(modeSelectedData, levelSelectedData)`; every other
`MatchCatalogs` call site (`StartManager.cs`'s `Compatibility`/`IsReady`/`Builder`,
`VersusLauncher.cs`'s `Builder`, both test files' `Override`/`Reset`) was left untouched since none of
those members' signatures changed.

The source-reference cache that used to live inside `MatchCatalogs.EnsureModes`/`EnsureLevels` (skip
rebuilding when the exact same source-list object comes back) had to move with the conversion it was
guarding: `LegacyMatchCatalogBootstrap` now holds `lastModeSources`/`cachedFallbackModes`/
`cachedModeAnomalies` and `lastLevelSources`/`cachedFallbackLevels`, keyed by `ReferenceEquals` against
the legacy source lists exactly as `MatchCatalogs` used to key them — same identity semantics, in-place
mutation of the same list still does not force a reconversion, mode and level caches still invalidate
independently. `MatchCatalogs.EnsureModes`/`EnsureLevels` keep their own independent reference cache
underneath, now keyed by the fallback definition lists `LegacyMatchCatalogBootstrap` hands them; because
that bootstrap hands back the same cached list objects when its own legacy sources have not changed,
`MatchCatalogs`'s cache still sees "unchanged" the same way it used to. Critically, the bootstrap calls
`MatchCatalogs.EnsureBuilt` on every request regardless of its own cache hit/miss, so
`MatchCatalogs.Reset()` followed by the same legacy sources still rebuilds — `MatchCatalogs`'s own
`modeSourceKey`/`levelSourceKey` are cleared by `Reset()`/`Override()`, independent of whatever the
bootstrap cached. Anomaly clearing/reporting behavior is unchanged: `MatchCatalogs` only clears and
repopulates `conversionAnomalies` when the authored-Resources catalog is empty and the fallback path is
actually taken, exactly as before the split, and still reports it through `Debug.LogError` via
`ReportProblems`.

Authored-Resources precedence, the legacy prefab fallback, and every conversion rule in
`GameModeDefinitionFactory`/`LevelDefinitionFactory` are untouched — `MatchCatalogs` still decides
authored vs. fallback, `LegacyMatchCatalogBootstrap` still just converts, and `Assets/Resources/Match/
Modes`/`Levels` still do not exist on `dev`, so the fallback path remains the one actually live in
production. `MatchCatalogs.cs.meta`'s GUID (`d9db91f6a5878674f9d6d1d008150297`) was confirmed before the
move and unchanged after it (`git mv`, preserving history), verified by reading the `.meta` file
post-move. Moved `MatchCatalogs.cs`/`.cs.meta` from `Assets/Scripts/menu_start/` into `Assets/Scripts/
game manager/Level5Match/`, alongside `MatchController.cs`/`MatchSession.cs`/`ActiveMatch.cs`.
`Level5.Match.asmdef` needed no new reference — `MatchCatalogs`'s only custom-assembly dependency,
`Level5.Core.Match`, was already declared, and it references no menu/loading/player/versus type.
Assembly-sensitive type identity checked and clean: no `[SerializeReference]`, `Type.GetType`,
`Assembly.Load`/`LoadFrom`, `AssemblyQualifiedName`, or `TypeNameHandling` touches `MatchCatalogs`
anywhere in the repo, and — since it is a static non-component type — no scene/prefab component
serialization applies either.

A post-implementation review flagged one consistency gap: unlike every other static owner introduced
across this migration (`MatchCatalogs`, `ActiveMatch`, `MatchSession`, `VersusRuntime`, `VersusCatalogs`,
`ActiveVersusAttempt`), `LegacyMatchCatalogBootstrap` initially exposed no way to clear its own
conversion cache. Not a live bug — every production and test path that needed a hard reset already went
through fresh source-list references or `MatchCatalogs.Reset()` — but a testability/consistency gap
against the established pattern. Fixed by adding `LegacyMatchCatalogBootstrap.Reset()`, which clears its
five cache fields and leaves `MatchCatalogs` untouched (callers that also want the runtime catalogs
cleared still call `MatchCatalogs.Reset()` separately, as they already did).

Headless Unity 6000.5.7f1 batch compile clean, zero `CS` errors. Added
`MatchCatalogsCompilesIntoLevel5Match` to `Level5ProductionAssemblyBoundaryTests.cs` (mirrors
`ActiveMatchCompilesIntoLevel5Match`), and a new focused fixture,
`LegacyMatchCatalogBootstrapTests.cs`, proving the four behaviors this split most risked changing:
repeated bootstrap calls with the same legacy source-list reference reuse the built catalog rather than
rebuilding it, `MatchCatalogs.Reset()` followed by the same legacy input still repopulates the catalogs,
`LegacyMatchCatalogBootstrap.Reset()` clears its own conversion cache independently of `MatchCatalogs`,
and a fallback conversion anomaly is still visible through `MatchCatalogs.ConversionAnomalies`. Focused
EditMode runs passed unchanged: `Level5ProductionAssemblyBoundaryTests` (12/12, including the new test),
`Level5AuthoredMatchDataTests` (7/7 — the conversion-parity/anomaly/id/compatibility suite that
exercises `GameModeDefinitionFactory`/`LevelDefinitionFactory` directly and is therefore unaffected by
where `MatchCatalogs` itself lives), `Level5VersusIntegrationTests` (10/10, including
`TheLauncherBuildsAnOrdinaryMatchForTheSeriesFrozenMode`, which calls `MatchCatalogs.Override`/`Reset`
directly), and the new `LegacyMatchCatalogBootstrapTests` (4/4). Per this repository's risk-based
validation policy, the full EditMode/PlayMode suites were not re-run for a behavior-preserving
ownership/boundary split with focused parity coverage already in place; PR CI owns that broader
regression coverage.

This does not close `AUD-012`/Phase 2, and does not unblock 2c on its own — see the updated 2c status
below.

**Slice 21 — dependency cut only, no move (2026-09-07): `PlayerController`'s direct `GameLevelManager`
arena-state dependency is removed through explicit composition.** Unlike slices 1-20, this slice moves
no file — `PlayerController` was confirmed still not dependency-closed (it retains direct references to
`PlayerInputReader`, `MatchRuntime`, `SniperManager`, `PlayerIdentifier`, `CharacterProfile`,
`PlayerHealth`, `PlayerDunk`, `PlayerAttackQueue`, `PlayerSwapAttack`, `PlayerDamageReactions`,
`CallBallToPlayer`, `ShooterAttributesMapper` and `RigidbodyFreezeHelper` — all still `Assembly-CSharp`),
so it cannot move yet. Its `BasketBall`/`ShotMeter`/`IShooterActor`-style dependencies are *not* part of
that blocker set; they are already owned by custom assemblies and would be legal references from a future
player asmdef. As first written this sentence listed them as blockers — corrected 2026-09-08, see the
remeasured closure scan below. This slice narrows one edge in place: the two direct
`GameLevelManager` reads audited beforehand -
`bballRimVector = GameLevelManager.instance.BasketballRimVector` (in `Start()`) and
`GameLevelManager.instance.TerrainHeight` (the no-active-Terrain drop-shadow fallback in `Update()`) -
were the only live references found, matching the expected set exactly.

The replacement mirrors the shape `AUD-010` Phase 1c already established for `BasketBall`'s identical
no-Terrain fallback (`IGroundHeightProvider`, bound by composition instead of read from
`GameLevelManager.instance` directly). `PlayerController` gained `BindArenaContext(Vector3
basketballRimVector, IGroundHeightProvider groundHeightProvider)`, a narrow method that only retains
both references (the rim as a value snapshot, matching how this controller already treated it; the
ground-height provider as a live reference, since `GameLevelManager` keeps updating its own value after
spawning). The no-Terrain fallback was extracted into a private `ResolveDropShadowHeight()` — a pure,
behavior-preserving extraction mirroring `BasketBall.ResolveDropShadowHeight()` in shape — so it could
be driven directly in focused tests the same way. Unlike `BasketBall` (bound synchronously
before its own `Start()` ever runs, letting it dereference the provider unconditionally),
`PlayerController`'s context cannot be bound before its own `Start()` — see the timing constraint below
— so `ResolveDropShadowHeight()` guards the unbound case explicitly: `Debug.LogError` and return the
last-known `terrainYHeight` rather than throwing or inventing a fallback global. This narrow guard
exists only because the timing genuinely differs from `BasketBall`'s; it is not a general pattern this
slice introduces elsewhere. A same-day code review flagged that an unbound provider is re-entered every
`Update()` frame for an entire airborne arc, so an unbound-provider window would otherwise flood the
console once per frame rather than once; fixed with a single `groundHeightProviderMissingLogged` bool so
the error logs exactly once per controller, covered by
`ResolveDropShadowHeight_NoBoundProvider_LogsOnlyOnceAcrossRepeatedCalls`. The same review also noted
that `BindArenaContext` binds the rim vector with no equivalent unbound-state diagnostic — an
intentional asymmetry: the task's own composition contract for this method is "retain the supplied
dependencies" only, and the failure shape it flagged (silently defaulting to `Vector3.zero` if a
composition path never calls `BindArenaContext` at all) already existed identically before this slice
(the direct `GameLevelManager.instance.BasketballRimVector` read it replaced was never diagnosed either)
— left as a pre-existing, unchanged failure mode rather than expanded scope.

A second review round found two more, both fixed. First, the new `Start()` call was the only statement
in that method that could not survive the duplicate-manager path (`Awake`'s `instance != this` guard
returns before `_spawnCoordinator` is assigned): every other statement already tolerates it, since
`ArenaBootstrap.Apply` early-returns on the null `_rules` such an instance carries and `FindRimVector` is
a null-safe scene search. Made null-conditional to restore that parity; on the normal path the
coordinator is always assigned in `Awake` before spawning, so it can only skip where no level was built.
Second, the new guard test is scoped to `PlayerController.cs` alone, but sibling components on the same
player hierarchy still read the same singleton for the same arena values (`PlayerDunk` for the rim
vector; `AutoPlayerDefense`/`AutoPlayerController` for the rim vector and `TerrainHeight`) — all out of
scope here. The guard's own doc comment now says so explicitly, so a pass is not misread as "the player
prefab no longer depends on `GameLevelManager`", mirroring the caveat
`Level5BasketballGameManagerEdgeTests` already records for the unresolved basketball -> `GameRules` edge.

`SpawnCoordinator` gained `BindHumanArenaContext(Vector3 basketballRimVector, IGroundHeightProvider
groundHeightProvider)`, which forwards both already-resolved values to every registered non-CPU
participant's already-wired `PlayerController` (`PlayerIdentifier.playerController`, populated by the
existing `setPlayer()` — no new `GetComponent`/scene search). A registered human unexpectedly missing a
`PlayerController` logs and continues, the same composition-error shape `GiveBall` already uses for a
missing `BasketBallState`/`GameStats`, rather than throwing and aborting every other participant's
binding. CPU participants are skipped entirely — `AutoPlayerController` takes no arena context.

Timing was the hard constraint driving where the call lives: `GameLevelManager.Awake()` spawns players
(via `SpawnCoordinator.SpawnPlayers()`) before the rim is resolved — `ArenaBootstrap.Apply` and
`_basketballRimVector = ArenaBootstrap.FindRimVector()` only run in `GameLevelManager.Start()`. Binding
during `RegisterHuman()` (Awake-time) would therefore hand every human a stale/default rim, so
`GameLevelManager.Start()` calls `_spawnCoordinator.BindHumanArenaContext(_basketballRimVector, this)`
as the last step, immediately after `_basketballRimVector` is assigned. This is a direct method call, not
reliant on `PlayerController.Start()`/`GameLevelManager.Start()` running in a particular order relative
to each other (unlike the value this replaces, which depended on that ordering implicitly) — it sets
`PlayerController`'s fields synchronously regardless of whether that controller's own `Start()` has run
yet this frame, and it always runs before the first `Update()` of any object in the scene.

No serialized field, prefab contract, public player API, movement/jump/dunk/shoot/input/combat/health/
damage algorithm, or CPU behavior changed. `AutoPlayerController` is untouched.

Headless Unity 6000.5.7f1 batch compile clean, zero `CS` errors. Added
`Level5PlayerControllerDependencyGuardTests.PlayerControllerHasNoGameLevelManagerReference` (mirrors
`Level5BasketBallDependencyGuardTests`), and a new focused fixture,
`Level5PlayerControllerArenaContextTests.cs` (8 tests): `BindArenaContext` stores both values;
`ResolveDropShadowHeight` reads the bound provider live, not a value snapshotted at bind time (mirrors
`Level5BasketballGroundHeightProviderTests`'s identical proof for `BasketBall`); an unbound provider
logs and returns the last-known height without throwing, and logs only once across repeated calls;
`SpawnCoordinator.BindHumanArenaContext` binds a single human, binds multiple humans identically, leaves
a CPU participant untouched, and logs (without throwing) for a human missing `PlayerController`. Focused
EditMode run: 47/47, including the two new
fixtures and the existing `Level5BasketBallDependencyGuardTests`/`Level5BasketballGameManagerEdgeTests`/
`Level5RangeMeterOwnershipTests`/`Level5BasketballGroundHeightProviderTests` fixtures this slice's shape
borrows from. `PlayerMovementPhysicsTests.JumpingDoesNotCompoundHorizontalVelocity` (the menu -> gameplay
-> player bootstrap real-scene smoke test) passed unchanged, confirming the new
`GameLevelManager.Start()` -> `SpawnCoordinator.BindHumanArenaContext()` -> `PlayerController.BindArenaContext()`
composition chain wires correctly end to end in a real scene. Per this repository's risk-based validation
policy, the full EditMode/PlayMode suites were not re-run for a behavior-preserving, narrowly-scoped edge
cut with focused parity coverage already in place; PR CI owns that broader regression coverage.

**`PlayerController` dependency closure scan, remeasured from current declarations (2026-09-08).**
Re-read the file completely; `GameLevelManager` no longer appears in live code anywhere (the five
remaining occurrences are `///` doc comments, and the new guard strips comments before asserting).

The scan first published with this slice (2026-09-07) grouped the closure by *concern* only and labelled
every entry `Assembly-CSharp`. That was wrong for eight of them. Ownership was re-derived here by locating
each type's current declaration and walking up to its nearest enclosing `.asmdef`, which is authoritative
— not by folder name, subsystem name, or historical location. That distinction matters because four of
these subsystem folders carry a *nested* asmdef covering only part of the folder
(`Assets/Scripts/input/Level5Input/`, `Assets/Scripts/misc/Level5Misc/`,
`Assets/Scripts/Utility/Level5Utility/`, `Assets/Scripts/game manager/Level5Match/`), so inferring
ownership from the top-level folder gets it wrong in both directions.

**A. Already owned by custom assemblies — legal references, not blockers.** A future `Level5.Player`
asmdef could reference these directly:

- **`Level5.Input`** (`Assets/Scripts/input/Level5Input/`): `PlayerControls`, `PlayerControlsProvider`
- **`Level5.Core`** (`Assets/Level5/Core/`): `IShooterActor`, `ShooterAttributes`, `IGroundHeightProvider`
- **`Level5.Basketball`** (`Assets/Scripts/basketball/`): `BasketBall`, `ShotMeter`, `BasketBallState`
- **`Level5.Utility`** (`Assets/Scripts/Utility/Level5Utility/`): `SceneObjects`, `UtilityFunctions`

**B. Still compiled into `Assembly-CSharp` — the actual remaining blockers (13).** Only these prevent
`PlayerController` from moving into a production asmdef:

- **input:** `PlayerInputReader` (`Assets/Scripts/input/`, *outside* `Level5Input/`)
- **match/session runtime:** `MatchRuntime` (`Assets/Scripts/game manager/`, *outside* `Level5Match/`)
- **sniper/projectile:** `SniperManager` (`Assets/Scripts/projectile/`, no asmdef)
- **player-local components:** `PlayerIdentifier`, `CharacterProfile`, `PlayerHealth`, `PlayerDunk`,
  `PlayerAttackQueue`, `PlayerSwapAttack`, `PlayerDamageReactions` (all `Assets/Scripts/player/`)
- **gameplay adapters/helpers:** `CallBallToPlayer` (`Assets/Scripts/misc/callBallToPlayer.cs`, *outside*
  `Level5Misc/`), `ShooterAttributesMapper` (`Assets/Scripts/player/`), `RigidbodyFreezeHelper`
  (`Assets/Scripts/Utility/`, *outside* `Level5Utility/`)

Three corrections to closure *membership*, beyond ownership: `BasketBallState` and
`IGroundHeightProvider` are live dependencies the first scan omitted (both legal — `Level5.Basketball`
and `Level5.Core`), and `AutoPlayerController` is **not** a dependency at all — its only two appearances
in the file are a `Debug.LogError` string literal and a comment, so it never belonged in the graph.

This remeasurement is diagnostic only — none of these are addressed in this PR, `PlayerController` is
not moved, and Phase 2c's PlayMode test assemblies are not normalized here. `PlayerController` itself
remains in `Assembly-CSharp` (`Assets/Scripts/player/`); Slice 21 cut one outbound edge and moved no
file, so Phase 2c stays blocked on it.

**Running total after slices 1-21:** `Assets/Scripts/basketball`'s 12 production files (source left in
place, no `.meta` moved) plus `MatchController.cs`/`MatchSession.cs`/`ActiveMatch.cs`/`MatchCatalogs.cs`
(all four `Level5.Match`) plus
`VersusCatalogs.cs`/`DefaultCompetitiveRulesets.cs`/
`FileVersusSeriesRepository.cs`/`VersusRuntime.cs`/`GameStatsAttemptResults.cs`/`ActiveVersusAttempt.cs`/
`VersusMatchReporter.cs` (all seven moved, `.meta`s intact) plus `AtomicFile` (new file/`.meta` under
`Level5.Utility`, `CharacterProgressStore.cs` itself left in place) plus the 27 files across 10 leaf
assemblies from slices 1-10 (`Level5.Input`,
`Level5.Combat`, `Level5.Enemy`, `Level5.PlayerRacing`, `Level5.Vehicle`, `Level5.MenuProgression`,
`Level5.Utility`, `Level5.Misc`, `Level5.Models`, `Level5.MenuStart`) plus the 4 pre-existing ones
(`Level5.Core`, `Level5.Constants`, `Level5.Pooling`, `Level5.Audio`) — 17 production runtime assemblies
total (unchanged from Slice 18's count: Slice 20 added a file to an existing assembly, not a new one;
Slice 21 moved no file at all — it narrowed an edge in place), out of roughly 218 `.cs` files in
`Assets/Scripts` before this phase started. `LegacyMatchCatalogBootstrap.cs`, the composition seam Slice
20 added, stays in `Assembly-CSharp` (`Assets/Scripts/menu_start/`) and is not counted here. The
remainder is either `player`/`game manager` themselves (still mutually coupled, and still most of what
the asmdef-free gameplay PlayMode workaround needs), or reaches into that pair (directly or transitively)
and so is blocked the same way the rest of `versus`/`analytics`/`Models/HighScoreModel` were.

Prohibited in Phase 2: controller convergence, player/CPU behaviour cleanup, locomotion changes,
input ownership changes, scene-search removal, namespace restructuring, API redesign, new service
layers, DI/service locators, shader/material changes, URP configuration changes, and scene or
environment polishing. If a dependency must be inverted solely to make an intended boundary legal,
make the smallest possible behaviour-preserving change and protect it with a dependency regression
test.

#### 2c — Normalize gameplay PlayMode tests

The asmdef-free `Assets/Tests/PlayModeGameplay` workaround remains until the runtime code it tests is
available through proper asmdef references. Only once the required gameplay runtime assemblies
exist: give gameplay PlayMode tests explicit references to those assemblies; move/consolidate them
under the normal PlayMode test assembly structure as appropriate; remove the asmdef-free
`Level5GameplayPlayModeTests` workaround; run the entire PlayMode suite. The workaround disappearing
is an exit consequence of 2b, not an early migration step.

**Checked (2026-08-20): still blocked, correctly.** All four files in the workaround folder
(`Level5GameplayPlayModeTests.cs`, `BasketballVisibilityTests.cs`, `GameplayLevelUnpauseTests.cs`,
`PlayerMovementPhysicsTests.cs`) instantiate or look up `GameStats`, `MatchController`,
`PlayerController`, or `BasketBall` directly - all inside the blocked player/basketball/game-manager
triangle. None of slices 1-10 touched that triangle (2b0's gate forbids it), so none of this phase's
migrated assemblies are what these tests need. 2c cannot complete until the triangle itself is cut,
which is not this phase's work - see 2b0's remeasurement above.

**Still blocked after Slice 11 (2026-09-04).** `BasketBall` itself now lives in `Level5.Basketball`,
but every file in the workaround folder that references it also references `GameStats`,
`MatchController`, or `PlayerController` - all still `Assembly-CSharp`, since Slice 11 only cut
basketball's own outbound edges and did not touch `player` or `game manager`. 2c's exit condition is
unchanged: it needs `player`/`game manager` migrated too, not just basketball.

**Still blocked after Slice 12 (2026-09-05), re-verified against the current folder rather than the
2026-08-20 four-file count.** `Assets/Tests/PlayModeGameplay` now holds nine files, not four -
`Level5MenuScreenPlayModeTests.cs`, `Level5MoneyBallStateCompositionPlayModeTests.cs`,
`Level5BasketBallShotMadeCompositionPlayModeTests.cs`, `Level5ShotMarkerSessionCompositionPlayModeTests.cs`
and `Level5BasketBallShotTelemetryCompositionPlayModeTests.cs` were added since that count and were never
part of the blocked set (they exercise `BasketBall`/`GameRules` composition only, no `GameStats`/
`PlayerController`). `MatchController` moving to `Level5.Match` in this slice removes it from the
blocker list, but three files still reach a type that remains `Assembly-CSharp` directly:
`Level5GameplayPlayModeTests.cs` reaches `VersusRuntime`, `VersusMatchReporter` and `VersusCatalogs`
(`Assets/Scripts/versus/`) plus `ActiveMatch` and `MatchCatalogs` (`Assets/Scripts/menu_start/`) — not
`GameStats`, which Slice 11 already moved into `Level5.Basketball` and which this file's own
`host.AddComponent<GameStats>()` calls now resolve through, without issue, since `Level5.Basketball` is
`autoReferenced`; `BasketballVisibilityTests.cs` and `PlayerMovementPhysicsTests.cs` reach
`PlayerController` via `FindAnyObjectByType`/`GetComponent`. 2c's exit condition is unchanged: those
three files stay blocked until `versus`/`menu_start` (already known blocked via `AtomicFile`, see
Slices 3-9) and `PlayerController` themselves migrate, which needs the `player`/`game manager` cycle cut
first, not a continuation of leaf-picking.

**Still blocked after Slice 13 (2026-09-06), re-verified against the current folder.** The same nine
files as Slice 12's count; no files were added or removed. Slice 13 moved `VersusCatalogs` out of
`Assembly-CSharp`, and `Level5GameplayPlayModeTests.cs`'s `VersusCatalogs.Reset()` call now resolves
through `Level5.Versus` (`autoReferenced`) instead — but the same file still calls `VersusRuntime.Reset()`
and `ActiveVersusAttempt.Clear()` directly (`Assembly-CSharp`, `Assets/Scripts/versus/`) and
`ActiveMatch.Clear()`/`MatchCatalogs.Reset()` (`Assembly-CSharp`, `Assets/Scripts/menu_start/`), and its
own body still calls `VersusMatchReporter.TryReport(...)`. `BasketballVisibilityTests.cs` and
`PlayerMovementPhysicsTests.cs` are unaffected by this slice — neither references anything in `versus`.
2c's exit condition is unchanged: `VersusRuntime`, `VersusMatchReporter`, `ActiveVersusAttempt`
(blocked on `ActiveMatch`), `FileVersusSeriesRepository` (blocked on `AtomicFile`), `ActiveMatch`,
`MatchCatalogs`, and `PlayerController` all still need to migrate, which needs the `player`/`game
manager` cycle cut first, not a continuation of versus leaf-picking — `VersusCatalogs`/
`DefaultCompetitiveRulesets` were `versus`'s only dependency-closed pair (see Slice 13 above).

**Still blocked after Slice 14 (2026-09-06).** Same nine files as Slices 12-13, unchanged, for the
same reasons Slice 13 gave — `AtomicFile` moving to `Level5.Utility` touches none of the direct
`VersusRuntime`/`VersusMatchReporter`/`ActiveVersusAttempt`/`ActiveMatch`/`MatchCatalogs` calls in
`Level5GameplayPlayModeTests.cs` that keep it blocked. What changed is *why* `FileVersusSeriesRepository`
is blocked: no longer on `AtomicFile` (see Slice 14's remeasurement above — it is now dependency-closed
on its own), just on still living in `Assembly-CSharp` — a future-candidate leaf, not a cleared one.
2c's exit condition is unchanged.

**Still blocked after Slice 15 (2026-09-06), re-verified against the current folder.** Same nine files
as Slices 12-14, unchanged. `Level5GameplayPlayModeTests.cs`'s `VersusRuntime.Override(...)`,
`VersusRuntime.Reset()` and four separate `VersusRuntime.Coordinator` property accesses (`CreateSeries`,
`Load`, `IssueAttempt`, `StartAttempt`) now resolve through `Level5.Versus` (`autoReferenced`) instead
of `Assembly-CSharp`, closing the last `versus`-side gap this slice could close — but the same file
still calls `VersusMatchReporter.TryReport(...)` and
`ActiveVersusAttempt.Clear()` (`Assembly-CSharp`, `Assets/Scripts/versus/`) and `ActiveMatch.Clear()`/
`MatchCatalogs.Reset()` (`Assembly-CSharp`, `Assets/Scripts/menu_start/`) directly, and
`BasketballVisibilityTests.cs`/`PlayerMovementPhysicsTests.cs` are unaffected — neither references
anything in `versus`. 2c's exit condition is unchanged: `VersusMatchReporter`, `ActiveVersusAttempt`,
`ActiveMatch`, `MatchCatalogs`, and `PlayerController` all still need to migrate, which needs the
`player`/`game manager` cycle cut first, not a continuation of versus leaf-picking —
`FileVersusSeriesRepository`/`VersusRuntime` were `versus`'s only remaining dependency-closed pair after
Slice 14's remeasurement (see Slice 15 above).

**Still blocked after Slice 16 (2026-09-06), re-verified against the current folder.** Same nine files
as Slices 12-15, unchanged. Slice 16 moved `GameStatsAttemptResults` out of `Assembly-CSharp`, but
`Level5GameplayPlayModeTests.cs` never referenced that type directly — its `VersusMatchReporter.TryReport(...)`
call already ran on top of the mapper before this slice and still does, unaffected by which assembly the
mapper lives in. The same file still calls `VersusMatchReporter.TryReport(...)` and
`ActiveVersusAttempt.Clear()` directly (`Assembly-CSharp`, `Assets/Scripts/versus/`) and
`ActiveMatch.Clear()`/`MatchCatalogs.Reset()` (`Assembly-CSharp`, `Assets/Scripts/menu_start/`), and
`BasketballVisibilityTests.cs`/`PlayerMovementPhysicsTests.cs` are unaffected — neither references
anything in `versus`. 2c's exit condition is unchanged: `VersusMatchReporter`, `ActiveVersusAttempt`,
`ActiveMatch`, `MatchCatalogs`, and `PlayerController` all still need to migrate, which needs the
`player`/`game manager` cycle cut first, not a continuation of versus leaf-picking —
`GameStatsAttemptResults` was `versus`'s only remaining dependency-closed file after Slice 15 (see Slice
16 above); `VersusMatchReporter`'s one remaining blocker is now `ActiveVersusAttempt` alone (see Slice
16's remeasurement).

**Still blocked after Slice 17 (2026-09-07), re-verified against the current folder.** Same nine files
as Slices 12-16, unchanged. Slice 17 moved `MatchSession` out of `Assembly-CSharp`, but none of the
nine blocked files reference `MatchSession` directly — `Level5MenuScreenPlayModeTests.cs` only
mentions it in a doc comment explaining why its isolated scene load skips `GameLevelManager`/
`MatchSession` initialization, not in code. `Level5GameplayPlayModeTests.cs` still calls
`VersusMatchReporter.TryReport(...)`/`ActiveVersusAttempt.Clear()` (`Assembly-CSharp`,
`Assets/Scripts/versus/`) and `ActiveMatch.Clear()`/`MatchCatalogs.Reset()` (`Assembly-CSharp`,
`Assets/Scripts/menu_start/`) directly, unaffected by this slice. 2c's exit condition is unchanged:
`VersusMatchReporter`, `ActiveVersusAttempt`, `ActiveMatch`, `MatchCatalogs`, and `PlayerController`
all still need to migrate, which needs the `player`/`game manager` cycle cut first — this slice's
remeasurement found `ActiveMatch` itself now dependency-closed (see Slice 17 above), the first of
those five with no remaining `Assembly-CSharp` edge, but it is not moved in this PR.

**Still blocked after Slice 18 (2026-09-07), re-verified against the current folder.** Same nine files
as Slices 12-17, unchanged. Slice 18 moved `ActiveMatch` out of `Assembly-CSharp`, and
`Level5GameplayPlayModeTests.cs`'s `ActiveMatch.Clear()` call now resolves through `Level5.Match`
(`autoReferenced`) instead — but the same file still calls `VersusMatchReporter.TryReport(...)`/
`ActiveVersusAttempt.Clear()` (`Assembly-CSharp`, `Assets/Scripts/versus/`) and `MatchCatalogs.Reset()`
(`Assembly-CSharp`, `Assets/Scripts/menu_start/`) directly, and `BasketballVisibilityTests.cs`/
`PlayerMovementPhysicsTests.cs` are unaffected — neither references anything in `menu_start`. 2c's exit
condition is unchanged: `VersusMatchReporter`, `ActiveVersusAttempt`, `MatchCatalogs`, and
`PlayerController` all still need to migrate, which needs the `player`/`game manager` cycle cut first.
This slice's remeasurement found `ActiveVersusAttempt` itself now dependency-closed (see Slice 18 above),
the next candidate with no remaining `Assembly-CSharp` edge, but it is not moved in this PR.

**Still blocked after Slice 19 (2026-09-07), re-verified against the current folder.** Same nine files
as Slices 12-18, unchanged. Slice 19 moved `VersusMatchReporter` and `ActiveVersusAttempt` out of
`Assembly-CSharp`, and `Level5GameplayPlayModeTests.cs`'s `VersusMatchReporter.TryReport(...)`/
`ActiveVersusAttempt.Clear()` calls now resolve through `Level5.Versus` (`autoReferenced`) instead —
but the same file still calls `MatchCatalogs.Reset()` (`Assembly-CSharp`, `Assets/Scripts/menu_start/`)
directly, and `BasketballVisibilityTests.cs`/`PlayerMovementPhysicsTests.cs` each call
`PlayerController` directly (`Assembly-CSharp`, `Assets/Scripts/player/`), unaffected by this slice.
2c's exit condition is unchanged: `MatchCatalogs` and `PlayerController` are now the only two remaining
direct `Assembly-CSharp` dependencies the workaround needs migrated, which needs the `player`/
`game manager` cycle cut first — this is a diagnostic finding only, neither is moved in this PR.

**Still blocked after Slice 20 (2026-09-07), re-verified against the current folder.** Same nine files
as Slices 12-19, unchanged. Slice 20 moved `MatchCatalogs` out of `Assembly-CSharp`, and
`Level5GameplayPlayModeTests.cs`'s `MatchCatalogs.Reset()` call now resolves through `Level5.Match`
(`autoReferenced`) instead — closing the last direct `Assembly-CSharp` dependency that file had.
`Level5GameplayPlayModeTests.cs` itself now references no type remaining in `Assembly-CSharp` directly.
`PlayerMovementPhysicsTests.cs` and `BasketballVisibilityTests.cs` each still call
`PlayerController` directly (`Assembly-CSharp`, `Assets/Scripts/player/`) via `GetComponent`/
`FindAnyObjectByType`, unaffected by this slice, and the remaining six files
(`GameplayLevelUnpauseTests.cs`, `Level5MenuScreenPlayModeTests.cs`, and the four `*Composition
PlayModeTests.cs` files) are unaffected as in every prior remeasurement — none references anything in
`menu_start`, `versus`, or `player`. 2c's exit condition is unchanged in kind but narrower in scope:
`PlayerController` is now the *only* remaining direct `Assembly-CSharp` dependency across all nine
files in the workaround folder, still gated on cutting the `player`/`game manager` cycle first — this
is a diagnostic finding only, `PlayerController` is not moved in this PR.

**Still blocked after Slice 21 (2026-09-07), re-verified against the current folder.** Same nine files
as Slices 12-20, unchanged. Slice 21 removed `PlayerController`'s direct `GameLevelManager` dependency
through explicit composition, but moved no file — `PlayerMovementPhysicsTests.cs`/
`BasketballVisibilityTests.cs` still call `PlayerController` directly (`Assembly-CSharp`,
`Assets/Scripts/player/`) via `GetComponent`/`FindAnyObjectByType`, unaffected by this slice: removing
one of `PlayerController`'s own outbound edges does not change that these test files reach the type
itself, which remains `Assembly-CSharp` since it did not move. 2c's exit condition is unchanged in kind:
`PlayerController` is still the only remaining direct `Assembly-CSharp` dependency across all nine
workaround files, still gated on cutting the `player`/`game manager` cycle first, and now additionally
on `PlayerController`'s own remaining `Assembly-CSharp` dependency set (input, match/session runtime,
sniper/projectile, player-local components, gameplay adapters/helpers — the 13 types in group B of this
slice's remeasured closure scan above) before a move becomes possible. Its `Level5.Input`/`Level5.Core`/
`Level5.Basketball`/`Level5.Utility` dependencies are legal references and do not block the move — this
is a diagnostic finding only, `PlayerController` is not moved in this PR.

#### 2d — Architecture guards and exit verification

Add or extend tests asserting: intended runtime source no longer falls back into `Assembly-CSharp`;
no forbidden custom-assembly dependency cycle exists; runtime assemblies do not reference
Editor-only assemblies; new assemblies declare their required custom/package dependencies; assembly
allowlists cannot silently grow; the asmdef-free gameplay PlayMode workaround no longer exists.

**Landed (2026-08-20):** `Assets/Tests/Editor/Level5ProductionAssemblyBoundaryTests.cs`, asmdef-free
like `Level5GameManagerEdgeTests` so it can read every folder as text without joining the dependency
graph it checks. Two guards, covering every current and future production asmdef automatically
(discovered at run time from every non-test `.asmdef` under `Assets/Scripts` and `Assets/Level5`, not
a hand-maintained list):

- `NoMigratedProductionAssemblyReachesIntoAssemblyCSharp` - collects every top-level `public` type
  declared outside a production asmdef folder, then fails if any migrated file's identifiers hit that
  set.
- `NoProductionAssemblyReferencesAKnownEditorOnlyPackageAssembly` - fails if a production `.asmdef`
  lists `"Analytics"` (the 2a-canary-confirmed Editor-only package assembly).

Getting the first guard right took two real fixes, not just tuning: restricting the "Assembly-CSharp
declared types" collection to unindented `public` declarations only (a private nested
`StatsManager.mode` class was colliding with every unrelated local variable named `mode` across the
codebase - nested/non-public types can't be reached by a bare identifier from another assembly at
all, so they were never real hits); and stripping string/char literals **before** comments, not after
- `Constants.cs`'s `"https://localhost:44362/..."` API-address constants contain `//`, and stripping
comments first misread that as a comment start, truncating the string literal and desyncing every
quote-pairing for the rest of the file, which was *why* a `"CharacterProfile"` table-name string
survived stripping and read as a real reference. Both fixes are recorded in the test file's own
comments. Full EditMode 465/465 (463 + these 2) and PlayMode 9/9 both passed with the guards active;
`validate-repository.ps1` passed.

**Two further code-review rounds (2026-08-21)** found and fixed real gaps in the guard itself, and
one false alarm worth recording so it isn't re-litigated:

- The type-declaration scan required `public` at column zero, silently excluding every type
  declared inside a `namespace { }` block (most of this codebase's model/service types, e.g.
  `HighScoreModel`). Replaced with a brace-depth walk (`CollectTypesNotNestedInAnotherType`): a
  `public` type counts unless an *enclosing brace belongs to another type* - namespace nesting no
  longer excludes it, class/struct nesting still does.
- `EnumerateAssemblyCSharpScripts` only walked `Assets/Scripts` plus loose files directly under
  `Assets/`, missing three vendored third-party folders with no asmdef of their own
  (`Standard Assets`, `Joystick Pack`, `OmniSARTechnologies` - 50 files, genuinely part of
  Assembly-CSharp). Generalized to scan all of `Assets/` for any `.cs` file with no ancestor
  `.asmdef` and no `Editor`/`Tests` path segment, rather than hand-listing folders.
- `StripComments`/`Relative` were re-typed byte-for-byte identical in six test files (the five
  pre-existing architecture-guard tests plus this one) - extracted to a shared
  `Level5TestSourceText` so a future fix to either only has to land once.
- The Editor-only-assembly check switched from a raw substring-in-quotes regex over the whole
  `.asmdef` file to parsing it with `JsonUtility` and checking the `references` array itself -
  more precise, and the same `AsmdefInfo`/`JsonUtility` parse now backs the reference-declaration
  data other guards need.
- A speculative third guard (no production assembly reaches into a *different* production assembly
  it didn't declare) was tried and dropped: bare-identifier matching false-positived
  `GameModeCompatibility.cs`'s `public GameModeCatalog Modes => modes;` property against the
  unrelated `Modes` type in `Level5.Constants` - a property name colliding with a foreign type name.
  Nothing today needs this guard (no production assembly references another yet), so it wasn't
  worth chasing false positives to keep.
- **Rejected as a false alarm:** a review pass flagged `Level5.Misc.asmdef`/`Level5.MenuStart.asmdef`
  as missing an explicit `UnityEngine.UI` reference (for `Text`/`Button`/`Image`) and predicted a
  compile failure. Both packages' asmdefs have `"autoReferenced": true`, which - contrary to the
  review's claim - cascades to *any* consuming asmdef with `overrideReferences: false` (which all of
  this phase's asmdefs use), not only to Unity's predefined assemblies; the compiled
  `Level5.Misc.dll`/`Level5.MenuStart.dll` with zero `CS0246` errors, across every verification run
  in this phase, already proved this empirically before the claim was even checked against the
  package's own `.asmdef`.

Full EditMode 465/465 and PlayMode 9/9 both passed after every round; `validate-repository.ps1`
passed throughout.

**2d exit, checked against current state:** intended runtime source (the 10 slices) does not fall
back into `Assembly-CSharp` - guarded, green. No forbidden cycle among production assemblies -
trivially true today (none of the 10 new leaves reference each other, only `Level5.Core`/packages).
Runtime assemblies don't reference a known Editor-only assembly - guarded, green. New assemblies
declare required references explicitly - true by construction (`Level5.Utility` → `Level5.Core`,
`Level5.Input` → `Unity.InputSystem`, the rest need none). **Not reachable this phase:** the asmdef-
free gameplay PlayMode workaround is still present (2c, above) - this is the one 2d exit item that
depends on cutting the player/basketball/game-manager cycle, which 2b0 correctly keeps out of scope
here.

**Full Phase 2 exit is therefore not reached in this pass**, and that is the expected outcome given
2b0's gate, not a shortfall: 14 production runtime assemblies now exist (10 new + 4 pre-existing),
the migrated portion of the graph is acyclic and guarded against regrowth, and everything not moved
is either inside the blocked triangle or reaches into it. Finishing Phase 2 - removing the
`Level5GameplayPlayModeTests` workaround and migrating `player`/`basketball`/`game manager` themselves
- requires first inverting or cutting that remaining cycle, which is a Phase-1-scale slice of its own
(see Phase 1's own slicing for the shape that work took), not a continuation of leaf-picking.

**Exit:** production gameplay code targeted by this phase lives in proper referenced assemblies; the
runtime assembly graph is acyclic; no migrated assembly depends on `Assembly-CSharp`; package
references are explicit where Unity requires them; full repository validation, Unity batch
compilation, and the full EditMode/PlayMode suites pass; the asmdef-free gameplay PlayMode workaround
is gone; one representative gameplay mode and one representative menu flow pass manual Play Mode
verification; gameplay and visuals are observably unchanged.

### Phase 3 — Converge the human/CPU pairs

Not "one type". The pairs carry real, intended differences: the human path has an analytics call and
an `isCpu` swish gate the CPU path does not, and the CPU path has its own trigger resets, dunk
decisions and shot-meter wait behaviour.

**Exit:** one shared implementation for common behaviour, with documented role-specific adapters for
intentional differences, and a parity matrix covering human, CPU shooter, CPU defense and local
multiplayer.

### Phase 4 — One locomotion motor

Scoped to **actor locomotion**, not to velocity writes in general. Direct velocity stays correct for
jumps, dunks, projectiles and ball launch. The defect is driving a dynamic body's locomotion with
`MovePosition`.

**Exit:** no actor drives locomotion via `MovePosition`; impulse and launch remain available as
explicit APIs. Movement tested separately for human, CPU shooter, CPU defense, enemy, bodyguard,
racing vehicle and cinder block.

### Phase 5 — Input ownership

One owner per action map. Today a shared `PlayerControls` instance and per-player instances coexist,
consumers read maps nothing enables — which made every level unstartable — and the `PlayerTouch` map
has zero readers *and* zero enablers.

**Exit:** dead maps deleted, `activeInputHandler` dropped to Input System only, and a test asserting
every map a consumer reads is enabled by someone.

### Phase 6 — Remove the scene searches

Full removal of the 124 scene searches, on the contracts Phase 0 established.

**Exit:** the validator fails the build on a missing scene contract, not the play session.

## Play Mode verification matrix

Automation has not yet caught a feel regression and will not. Each phase is played, not only tested.

| Phase | Must be played |
| --- | --- |
| 0 | Nothing changes behaviour; smoke test one gameplay mode |
| 1 | Free play, marker contests, points by distance, In The Pocket, CPU shooter, local multiplayer |
| 2 | One gameplay mode and one menu flow — the split should change nothing |
| 3 | Human, CPU shooter, CPU defense, local multiplayer |
| 4 | Human, CPU shooter, CPU defense, enemy, bodyguard, racing |
| 5 | Desktop keyboard, controller, touch/mobile, every menu screen |
| 6 | Every mode touched by a migrated contract |

## Review pass

- **Why not the assembly split first?** It is blocked by the cycle. Attempting it first means drawing
  module boundaries around the tangle and cementing it.
- **Why not collapse the duplicates first?** Most visible, most tempting. But merging two 1,000-line
  controllers with no ability to test either is how a silent behaviour change ships. Phase 2 makes
  Phase 3 survivable.
- **Why is input late when it caused the worst bug?** The blocking bug is fixed; what remains is
  cleanup. Pull it forward if it bites again.
- **What could still make this wrong?** Phase 1b assumes a `GameStats` facade can hold the line while
  consumers migrate. If scoring or win conditions read through it in ways the facade cannot preserve,
  stop and re-plan rather than push through.

## Rejected

- ECS, a DI container, a service locator, a wholesale input rewrite.
- Interfaces or abstractions introduced before a second implementation exists.
- Any big-bang restructure.

## Appendix: how these were measured

Taken 2026-08-17 against `dev`, excluding `Legacy~` throughout. Third-party folders, `Assets/Tests`
and editor-only assemblies are excluded from the coupling table.

    # .cs files and asmdefs under Assets/Scripts
    find Assets/Scripts -name "*.cs" -not -path "*Legacy~*" | wc -l    # 218
    find Assets/Scripts -name "*.asmdef" | wc -l                       # 3, plus Level5.Core = 4

    # live scene searches, comment lines excluded
    grep -rn "GameObject.Find\|FindWithTag\|FindGameObjectsWithTag" \
      --include=*.cs Assets/Scripts | grep -v "Legacy~" | grep -vE ":\s*//" | wc -l   # 124

    # which package references are asmdef-based
    ls Library/ScriptAssemblies/*.dll     # asmdef-based; anything absent is precompiled or engine

The coupling table maps every declared type to its owning top-level folder under `Assets/Scripts`,
then counts foreign type mentions per file with comments stripped. It counts mentions rather than
unique symbols, so it indicates coupling weight rather than an exact dependency count.
