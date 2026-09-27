# Beta Lab

Beta Lab is an opt-in testing mode for spells, skills, visual effects, sound cues, and Reduced Motion. It is intentionally absent from the normal retail player.

## Build the tester player

From the Unity project root, run:

```powershell
.\Tools\BuildBetaLabWindows.ps1
```

The script runs the same embedded rule, art, combat-UI, and runtime gates as the retail build, then invokes Unity's `Development` build option. The staged folder and ZIP it creates end in `-beta-dev`, so they cannot overwrite the retail package. It clean-extracts that ZIP and verifies the development-only title route from the packaged bytes.

## Enter the lab

Launch the Beta Development player and choose **Beta Lab** on the title screen. Lab combat is save-blocked and isolated from campaign progression.

The caster lab provides maximum-level **Mage**, **Warlock**, and **Priest** testers. Select a class to open its production Spellbook. Priest has all 16 Mend spells available; **Stage** keeps that priest selected and prepares wounded, afflicted allies, light-spell targets, and a hostile field for Rift Seal.

Choose **Skills Lab** on the combat toolbar, then **Warrior**, **Rogue**, or **Ranger** to open all seven skills for that class. Each class has a dedicated maximum-level tester. **Wound** prepares finishers and **Cluster** prepares area attacks. **Spells Lab** returns to caster testing. Reset restores defeated testers. The title-screen `T` (or `Ctrl+Shift+B`) testing panel still provides the separate Martial Lab entry.

Close the open book with Escape/controller B to reach the lab toolbar. Resource refill, hazards, enemy waves, reset, and the visual tour remain available.

In combat:

- Press `F10` or controller Back/Select to focus the lab controls.
- Use arrows, WASD, or left stick to choose an action.
- Press Enter, Space, or controller A to activate it.
- Press `F10` or Back/Select again to return input to the battlefield.
- Escape/controller B closes an open Visual-only Tour first, then releases the lab controls on the next press.

## Real casts and visual previews

Mage, Warlock, Priest, and martial test kits use the normal production spell/skill resolution paths. Damage, resources, targeting, summons, fields, movement, sound, and status effects behave as they do in campaign combat. The lab unlocks the complete prototype catalog; it does not change campaign progression or content selection.

Normal campaign progression exposes 33 of the 56 catalog formulas. The new promoted set is `OBL` Dawnlance (level 3), `RCL` Frost Lance (level 4), `INH` Siphon Life (level 5), `HLC` Hallowed Circle (level 8), `IBF` Summon Lesser Demon (level 8), and `DMC` Doom Circle (level 10).

The **31-entry Visual-only Tour** (18 formulas and 13 skills) is deliberately different: Replay/Next/Cue stages deterministic presentation without spending resources or changing combat outcomes. Use it for close VFX/SFX comparison, then validate mechanics through the production Spellbook or skill panel.

While Visual-only Tour is selected, use Q/E or the controller bumpers for previous/next, Enter/A to replay, C/X to audition the cue, and V/Y to switch Full/Reduced Motion.

Full Motion gives ten signature skills more distinct body language: Steel Tempest/Abyssal Whirl spin, Hold the Line/Dread Roar brace, Arrowstorm/Twinshot draw and recoil, Slip the Veil/Smoke Bomb vanish, and Shatterguard/Executioner's Cut commit to heavy strikes.

The Motion control switches between Full and Reduced Motion and immediately replays the selected preview. Reduced Motion keeps a readable static semantic impact while suppressing travel, lingering particles, and actor movement.
