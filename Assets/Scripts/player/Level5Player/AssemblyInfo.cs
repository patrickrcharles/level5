using System.Runtime.CompilerServices;

// AUD-012 Phase 2b Slice 28 deviation: Level5CharacterProfileDependencyGuardTests / the Slice 28
// audit both assumed CharacterProfileStatMapper was the only live writer of
// CharacterProfile.IsLocked (get; internal set;). LoadManager.loadPlayerSelectDataList
// (Assets/Scripts/menu_loading/LoadManager.cs) also writes it - the live, documented primary-roster
// SQLite unlock path (docs/persistence-boundaries.md, "Unlock authority") - and stays in
// Assembly-CSharp; it is not dependency-closed and moving it is out of this slice's scope. This
// attribute preserves IsLocked's internal setter for every assembly except the one that already had
// unrestricted access to everything else on this type, rather than widening the setter to public or
// changing LoadManager's persistence-authority behavior in an ownership-only slice.
[assembly: InternalsVisibleTo("Assembly-CSharp")]
