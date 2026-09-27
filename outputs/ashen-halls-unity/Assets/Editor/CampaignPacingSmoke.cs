using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    // A reward/pacing ledger, deliberately not a claim that simulated victories
    // prove a playable campaign. Enemy creation, ranks, XP, level-ups and camp
    // recovery use production code; route choices and travel awards are explicit.
    public static class CampaignPacingSmoke
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly int[] Seeds = { 0, 1, 7, 19, 42, 97, 227, 727, 1337, 2320 };
        private static readonly string[] SweepZones =
        {
            "midgaard-city", "midgaard-road", "old-quarry", "glass-warrens", "ash-fen",
            "red-gate", "gloam-courts", "salt-cisterns", "green-shrine-road", "dusk-market", "inner-ash-road"
        };

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " campaign pacing report complete (ledger, not tactical victory proof).");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError("Campaign pacing report failed: " + exception);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine("# Campaign pacing ledger");
            report.AppendLine();
            report.AppendLine("Generated from current production enemy creation, encounter placement, rank selection, reward, level-up, hero recalculation and camp methods. Four starter heroes, all survive, no repeated patrol farming, no purchased equipment or spent growth points. Victories are assumed; this does not measure win rate, tactics, route travel time or damage taken.");
            report.AppendLine();
            report.AppendLine("Direct route includes the 15 authored Chapter I–V battles plus chapter-entry XP (30/36/42/48), the Chapter III and V surveys (48/64), Yara's mandatory Glass/Red briefings (12/16), and first inspections of the two/ two/ three required regional sites in III/IV/V. It excludes zone discovery, optional errands, caches, shrines and optional patrols. These explicit world-award assumptions are reviewed against the runtime handlers; they are not dynamically extracted.");
            report.AppendLine();
            report.AppendLine("Sweep adds each current production patrol once and 11 regional zone discoveries per chapter. This is a generous comparison scenario: some regions may be revealed by hints before discovery and therefore award no XP. It is not an expected player itinerary or an upper bound on every possible optional reward.");
            report.AppendLine();
            report.AppendLine("## Seed 727 direct route before chapter graduation rewards");
            report.AppendLine();
            report.AppendLine("| Chapter/battle | Enemy HP / power | XP per survivor | Human level | Mage level | Accumulated base XP |");
            report.AppendLine("|---|---:|---:|---:|---:|---:|");
            Summary representative = BuildLedger(727, false, report);
            report.AppendLine();
            report.AppendLine("## Seed matrix at the surveyed Meteor Crown threshold");
            report.AppendLine();
            report.AppendLine("| Seed | Direct base XP | Direct human / mage | Sweep base XP | Sweep human / mage |");
            report.AppendLine("|---:|---:|---|---:|---|");
            foreach (int seed in Seeds)
            {
                Summary direct = seed == 727 ? representative : BuildLedger(seed, false, null);
                Summary sweep = BuildLedger(seed, true, null);
                report.AppendLine($"| {seed} | {direct.BaseXp} | {direct.HumanLevel} / {direct.MageLevel} | {sweep.BaseXp} | {sweep.HumanLevel} / {sweep.MageLevel} |");
                Require(sweep.BaseXp > direct.BaseXp, "optional patrol/zone sweep adds experience");
            }
            report.AppendLine();
            report.AppendLine("## Current milestone tuning and authored finale, seed 727 direct route");
            report.AppendLine();
            report.AppendLine("Milestone rewards below come from CampaignMilestoneRules and the real ApplyCampaignMilestoneRewards handler. Existing XP costs/unlocks and combat rewards remain unchanged. World rewards are inserted before the relevant site battle and chapter completions after the chapter's last victory. The three Chapter VI first inspections each award 22 XP; the dedicated Chapter VI entry currently grants supplies without separate arrival XP.");
            report.AppendLine();
            foreach (CampaignMilestone milestone in CampaignMilestoneRules.All)
                report.AppendLine($"- {milestone.Name}: {milestone.Experience} XP once.");
            report.AppendLine();
            report.AppendLine("| Chapter/battle | Enemy HP / power | XP per survivor | Human level | Mage level | Accumulated base XP |");
            report.AppendLine("|---|---:|---:|---:|---:|---:|");
            Summary tunedFinale = BuildLedger(727, false, report, true, 6);
            report.AppendLine();
            report.AppendLine("| Seed | Current direct threshold human / mage | Current direct ending human / mage | Current sweep ending human / mage |");
            report.AppendLine("|---:|---|---|---|");
            foreach (int seed in Seeds)
            {
                Summary threshold = BuildLedger(seed, false, null, true, 5);
                Summary ending = BuildLedger(seed, false, null, true, 6);
                Summary sweep = BuildLedger(seed, true, null, true, 6);
                report.AppendLine($"| {seed} | {threshold.HumanLevel} / {threshold.MageLevel} | {ending.HumanLevel} / {ending.MageLevel} | {sweep.HumanLevel} / {sweep.MageLevel} |");
                Require(ending.MageLevel >= 14 && ending.MageLevel <= 16,
                    "direct-route nonhuman reaches a bounded late-game level at seed " + seed);
                Require(ending.HumanLevel >= ending.MageLevel && ending.HumanLevel <= 16,
                    "human learning bonus remains bounded at seed " + seed);
            }
            report.AppendLine();
            report.AppendLine("## Previous unlock gap (seed 727 direct route before milestone tuning)");
            report.AppendLine();
            foreach (string classKey in new[] { "warrior", "ranger", "rogue" })
                report.AppendLine("- " + classKey + ": " + string.Join(", ", AbilityCatalog.IdsForClass(classKey).Select(AbilityCatalog.For)
                    .Where(ability => ability != null && ContentSetCatalog.AbilityActive(ContentSetCatalog.SewerSlice, ability.Id))
                    .Select(ability => ability.Name + " L" + ability.RequiredLevel + (ability.RequiredLevel > representative.HumanLevel ? " (unreached)" : ""))));
            report.AppendLine("- Active formulas above the mage's level: " + string.Join(", ", FormulaCatalog.All
                .Where(formula => ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, formula.Code)
                    && FormulaCatalog.RequiredLevel(formula) > representative.MageLevel)
                .Select(formula => formula.Name + " L" + FormulaCatalog.RequiredLevel(formula))) + ".");
            foreach (int level in new[] { 8, 12, 16, 20 })
                report.AppendLine($"- Level {level} requires {ProgressionRules.CumulativeExperienceToLevel(level)} cumulative XP; the direct-route non-human deficit is {Math.Max(0, ProgressionRules.CumulativeExperienceToLevel(level) - representative.BaseXp)} XP.");
            report.AppendLine($"- With current milestone tuning the direct ending reaches human L{tunedFinale.HumanLevel}, mage L{tunedFinale.MageLevel}. Optional exploration remains the route to level-16 capstones; level20 is not assumed for this campaign.");
            AppendRecoveryAndFinale(report, BuildLedger(727, false, null, true, 5));
            string path = Path.GetFullPath(Path.Combine(Application.dataPath, "../QA/campaign-pacing-report.md"));
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, report.ToString(), new UTF8Encoding(false));
            Debug.Log("Campaign pacing report: " + path + "; direct starter levels " + representative.HumanLevel + "/" + representative.MageLevel);
        }

        private static Summary BuildLedger(int seed, bool sweep, StringBuilder report, bool milestones = false, int finalDepth = 5)
        {
            using (Fixture fixture = new Fixture(seed))
            {
                int baseXp = 0;
                for (int depth = 1; depth <= finalDepth; depth++)
                {
                    fixture.State.Depth = depth;
                    if (milestones && depth == 6)
                        baseXp += fixture.ClaimMilestone(StoryFlags.MeteorCrownAssaultAccepted);
                    // Explicit one-time entry/briefing inputs, separate from encounter XP.
                    int[] worldAwards = depth == 2 ? new[] { 30 }
                        : depth == 3 ? new[] { 36 }
                        : depth == 4 ? new[] { 12, 42 }
                        : depth == 5 ? new[] { 16, 48 }
                        : Array.Empty<int>();
                    foreach (int award in worldAwards)
                    {
                        fixture.Call("AwardExperience", award);
                        baseXp += award;
                    }
                    if (sweep)
                    {
                        foreach (string zoneId in SweepZones)
                        {
                            WorldZone zone = WorldZoneCatalog.For(zoneId, depth);
                            int award = Math.Max(6, 10 + zone.Danger * 4 + depth * 2);
                            fixture.Call("AwardExperience", award);
                            baseXp += award;
                        }
                    }
                    int battleIndex = 0;
                    foreach (EncounterDefinition encounter in Route(depth))
                    {
                        int siteXp = depth == 3 && battleIndex == 0 ? 16
                            : depth == 4 && (battleIndex == 0 || battleIndex == 2) ? 18
                            : depth == 5 ? 20 : depth == 6 ? 22 : 0;
                        if (siteXp > 0) { fixture.Call("AwardExperience", siteXp); baseXp += siteXp; }
                        List<CombatUnit> enemies = fixture.Enemies(encounter);
                        int xp = CombatRewardRules.ExperienceForVictory(enemies, fixture.State.Party.Count);
                        Require(xp > 0, "authored battle pays experience: " + encounter.LegacyStyle);
                        fixture.Call("AwardExperience", xp);
                        baseXp += xp;
                        if (milestones && depth == 6 && battleIndex == 1)
                            baseXp += fixture.ClaimMilestone(StoryFlags.MeteorCrownHeartBroken);
                        report?.AppendLine($"| {depth}: {encounter.Banner} | {enemies.Sum(enemy => enemy.MaxHp)} / {enemies.Sum(enemy => enemy.Power)} | {xp} | {fixture.State.Party[0].Level} | {fixture.State.Party[2].Level} | {baseXp} |");
                        battleIndex++;
                    }
                    foreach (int award in depth == 3 ? new[] { 16, 48 } : depth == 5 ? new[] { 64 } : Array.Empty<int>())
                    { fixture.Call("AwardExperience", award); baseXp += award; }
                    if (milestones && depth <= 5)
                    {
                        string completed = depth == 1 ? StoryFlags.SewerRewardClaimed
                            : depth == 2 ? StoryFlags.KoboldKingDefeated
                            : depth == 3 ? StoryFlags.RedGateWarningRecovered
                            : depth == 4 ? StoryFlags.EmberglassGateKeyRecovered : StoryFlags.RedGateDebriefed;
                        int award = fixture.ClaimMilestone(completed);
                        baseXp += award;
                        report?.AppendLine($"| {depth}: chapter graduation | — | {award} | {fixture.State.Party[0].Level} | {fixture.State.Party[2].Level} | {baseXp} |");
                    }
                    if (sweep)
                    {
                        foreach (RoamingThreatDefinition threat in RoamingThreatCatalog.ForDepth(depth, false))
                        {
                            List<CombatUnit> enemies = fixture.Enemies(RoamingThreatCatalog.BuildEncounter(threat, depth));
                            int xp = CombatRewardRules.ExperienceForVictory(enemies, fixture.State.Party.Count);
                            fixture.Call("AwardExperience", xp);
                            baseXp += xp;
                        }
                    }
                }
                return new Summary
                {
                    BaseXp = baseXp,
                    HumanLevel = fixture.State.Party[0].Level,
                    MageLevel = fixture.State.Party[2].Level,
                    Party = JsonUtility.FromJson<GameState>(JsonUtility.ToJson(fixture.State)).Party
                };
            }
        }

        private static IEnumerable<EncounterDefinition> Route(int depth)
        {
            if (depth == 1) return ContentSetCatalog.SewerSliceEncounters;
            EncounterId[] ids = depth == 2 ? new[] { EncounterId.KoboldAmbush, EncounterId.KoboldCave, EncounterId.KoboldKing }
                : depth == 3 ? new[] { EncounterId.BoneRoadWatch, EncounterId.GloamCryptRitual, EncounterId.GloamWarden }
                : depth == 4 ? new[] { EncounterId.GlasswardAmbush, EncounterId.GlassIndexKeepers, EncounterId.AshenPactWarden }
                : depth == 6 ? new[] { EncounterId.MeteorCrownSentries, EncounterId.MeteorCrownHeart, EncounterId.MeteorCrownSovereign }
                : new[] { EncounterId.RedGateVanguard, EncounterId.OssuaryRoadSeal, EncounterId.CrownroadMarshal };
            return ids.Select(EncounterCatalog.For);
        }

        private static void AppendRecoveryAndFinale(StringBuilder report, Summary direct)
        {
            report.AppendLine();
            report.AppendLine("## Economy and recovery");
            report.AppendLine();
            report.AppendLine("Runtime FinishCombat gives 18–38 + 6 × depth gold and a 60% elixir chance. Across the 15 required fights: 540–840 gold (mean 690). Starting 28 + Borin 10 + Varkh 49 bring gross route gold to 627–927 (mean 777), before purchases, caches, sales or optional fights. Starting three + Varkh one + nine expected victory drops produce 13 expected elixirs; random drops are not guaranteed.");
            report.AppendLine("Guaranteed supplies: five starting + one Foul Runoff alcove + eight first chapter arrivals + two Varkh + one Ossuary Warden + one Mirror Index + two Ashen Pact + one road seal + two Marshal = 23 before use. Free Recall returns to Temple Square and RestorePartyFully also revives fallen heroes. Travel back to the current objective is the recovery cost. Camp heals living heroes only, spending one supply; it cannot recover a fallen hero. Dead heroes receive no XP at victory.");
            report.AppendLine();
            using (Fixture fixture = new Fixture(727))
            {
                fixture.State.Party = direct.Party;
                fixture.State.Supplies = 100;
                foreach (PartyMember member in fixture.State.Party)
                {
                    member.Hp = 1;
                    member.Mana = 0;
                }
                int[] hpBefore = fixture.State.Party.Select(member => member.Hp).ToArray();
                fixture.Call("Camp");
                Require(fixture.State.Supplies == 99, "production camp spends one supply");
                report.AppendLine("| Starter at threshold, gear/growth unspent | Level | Max HP / MP | One camp HP / MP restored | Camps from 1 HP / 0 MP to full |");
                report.AppendLine("|---|---:|---:|---:|---:|");
                for (int i = 0; i < fixture.State.Party.Count; i++)
                {
                    PartyMember member = fixture.State.Party[i];
                    int hpRestored = member.Hp - hpBefore[i];
                    int manaRestored = member.Mana;
                    int camps = 1;
                    while ((member.Hp < member.MaxHp || member.Mana < member.MaxMana) && camps < 100)
                    {
                        fixture.Call("Camp");
                        camps++;
                    }
                    report.AppendLine($"| {member.Name} | {member.Level} | {member.MaxHp} / {member.MaxMana} | {hpRestored} / {manaRestored} | {camps} |");
                    // Reset everyone so each row measures the same wounded condition.
                    foreach (PartyMember reset in fixture.State.Party) { reset.Hp = 1; reset.Mana = 0; }
                    fixture.State.Supplies = 100;
                    fixture.Call("Camp");
                }
                fixture.State.Party[0].Hp = 0;
                fixture.Call("Camp");
                Require(fixture.State.Party[0].Hp == 0, "camp cannot resurrect");
                fixture.Call("RestorePartyFully");
                Require(fixture.State.Party.All(member => member.Hp == member.MaxHp && member.Mana == member.MaxMana), "temple restoration includes fallen heroes");
                fixture.State.Depth = 6;
                List<CombatUnit> finale = fixture.Enemies(EncounterCatalog.For(EncounterId.MeteorCrownSovereign));
                report.AppendLine();
                report.AppendLine("## Authored final battle at depth 6, seed 727");
                report.AppendLine();
                report.AppendLine("| Enemy | HP | Power | Defense | Range | Magic resist | Weakness |");
                report.AppendLine("|---|---:|---:|---:|---:|---:|---|");
                foreach (CombatUnit enemy in finale)
                    report.AppendLine($"| {enemy.Name} | {enemy.MaxHp} | {enemy.Power} | {enemy.Defense} | {enemy.Range} | {enemy.MagicResist} | {(enemy.Weakness ?? "").Replace("|", " / ")} |");
                report.AppendLine($"Total {finale.Sum(enemy => enemy.MaxHp)} enemy HP; {CombatRewardRules.ExperienceForVictory(finale, 4)} base XP per survivor. This ledger does not establish a tactical win rate.");
            }
            report.AppendLine();
            report.AppendLine("Interpretation: current chapter rewards put level-12 tools in reach before the finale and level-14 tools before Vhal Rakh. Level-16/20 tools cannot be required on the direct route. Test tactical difficulty with these actual levels, earned quest gear and deliberately spent growth points. The generous exploration scenario reaches level18–19 and may substantially soften the finale; this is a separate playtest case, not proof of balanced combat. Aggregate enemy power is not damage per round.");
        }

        private sealed class Summary
        {
            public int BaseXp;
            public int HumanLevel;
            public int MageLevel;
            public List<PartyMember> Party;
        }

        private sealed class Fixture : IDisposable
        {
            private readonly GameObject host;
            private readonly AshenHallsGame game;
            public readonly GameState State;

            public Fixture(int seed)
            {
                host = new GameObject("Campaign pacing ledger");
                host.SetActive(false);
                host.hideFlags = HideFlags.HideAndDontSave;
                game = host.AddComponent<AshenHallsGame>();
                State = new GameState { ContentSetId = ContentSetCatalog.SewerSlice, Seed = seed, Depth = 1, Mode = GameMode.Explore,
                    ReducedMotion = true, SfxMuted = true, MusicMuted = true, Supplies = 5, Gold = 28, Elixirs = 3 };
                Set("state", State);
                Set("activeContentSet", ContentSetCatalog.SewerSlice);
                Set("rng", new System.Random(seed));
                Set("labSaveBlocked", true);
                State.Party = (List<PartyMember>)Call("MakeDefaultParty");
                Require(State.Party.Count == 4, "default party has four members");
            }

            public List<CombatUnit> Enemies(EncounterDefinition encounter)
            {
                Require(!encounter.UsesGeneratedEnemyPool, "ledger uses an authored roster");
                List<CombatUnit> enemies = new List<CombatUnit>();
                for (int index = 0; index < encounter.EnemyCountForDepth(State.Depth); index++)
                    enemies.Add((CombatUnit)Call("MakeEnemy", encounter.EnemyIds[index % encounter.EnemyIds.Length], index, null));
                CombatState combat = new CombatState { Units = enemies };
                Call("ApplyEncounterPlacements", combat, encounter);
                return enemies;
            }

            public int ClaimMilestone(string completedFlag)
            {
                if (!State.StoryFlags.Contains(completedFlag)) State.StoryFlags.Add(completedFlag);
                int award = CampaignMilestoneRules.Pending(State.StoryFlags).Sum(milestone => milestone.Experience);
                Call("ApplyCampaignMilestoneRewards");
                Require(!CampaignMilestoneRules.Pending(State.StoryFlags).Any(), "production milestone receipt is durable");
                return award;
            }

            public object Call(string name, params object[] arguments)
            {
                MethodInfo method = typeof(AshenHallsGame).GetMethods(Private).Single(candidate => candidate.Name == name && candidate.GetParameters().Length == arguments.Length);
                return method.Invoke(game, arguments);
            }
            private void Set(string name, object value) => typeof(AshenHallsGame).GetField(name, Private).SetValue(game, value);
            public void Dispose() => UnityEngine.Object.DestroyImmediate(host);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Campaign pacing ledger: " + message);
        }
    }
}
