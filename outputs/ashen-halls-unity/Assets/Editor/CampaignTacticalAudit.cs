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
    // A bounded policy experiment, not an assertion that human players win.
    // Every resolved move, hit, spell, enemy action and turn uses production code.
    public static class CampaignTacticalAudit
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly int[] Seeds = { 42, 727, 1337 };
        private static readonly EncounterId[] Targets = {
            EncounterId.KoboldKing, EncounterId.GloamWarden, EncounterId.AshenPactWarden,
            EncounterId.CrownroadMarshal, EncounterId.MeteorCrownSentries,
            EncounterId.MeteorCrownHeart, EncounterId.MeteorCrownSovereign };
        private static readonly string[] Builds = { "starter-unspent", "quest-gear-unspent", "quest-gear-spent" };
        private static readonly string[] SpellPolicy = {
            "FIF", "RIG", "RCL", "FBL", "RDF", "RBI", "FRB", "CLT", "MTR",
            "OBL", "OIC", "LBC", "DWP", "NVC", "TBQ", "SGW", "TBG", "TNC", "SWR", "SRF" };
        private static readonly Dictionary<string, string> AbilityDamageMethods = new Dictionary<string, string> {
            ["charge"] = "ChargeRawDamage", ["execute"] = "ExecuteRawDamage", ["shieldbash"] = "ShieldBashRawDamage",
            ["cleave"] = "CleaveRawDamage", ["aimedshot"] = "AimedShotRawDamage", ["pinningshot"] = "PinningShotRawDamage",
            ["volley"] = "VolleyRawDamage", ["broadheadshot"] = "BroadheadShotRawDamage", ["disruptingshot"] = "DisruptingShotRawDamage" };

        public static void Run()
        {
            try { RunOrThrow(); EditorApplication.Exit(0); }
            catch (Exception exception) { Debug.LogError("Campaign tactical audit failed: " + exception); EditorApplication.Exit(1); }
        }

        public static void RunOrThrow()
        {
            CampaignPacingSmoke.RunOrThrow();
            List<Result> results = new List<Result>();
            StringBuilder trace = new StringBuilder();
            foreach (EncounterId target in Targets)
            foreach (int seed in Seeds)
            foreach (string build in Builds)
            {
                using (Fixture fixture = new Fixture(seed))
                {
                    fixture.Prepare(target, build);
                    Result result = fixture.Fight(EncounterCatalog.For(target), build, trace);
                    results.Add(result);
                    Debug.Log("TACTICAL " + result.Row());
                }
            }
            StringBuilder report = new StringBuilder();
            report.AppendLine("# Campaign tactical policy audit");
            report.AppendLine();
            report.AppendLine("Generated " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm 'UTC'") + ". Actual production turns; victories are observed, never assumed within these fights. Seeds: " + string.Join(", ", Seeds) + ".");
            report.AppendLine();
            report.AppendLine("## Scope and reproducible method");
            report.AppendLine();
            report.AppendLine("Each row is an independent encounter, entered fully rested with the four default heroes and three elixirs. Prior route victories are assumed only to construct the entering XP/quest-gear baseline. XP uses actual MakeEnemy/rank/placement, CombatRewardRules and AwardExperience, plus production chapter milestones. The three actual SewerSliceEncounters are used for Chapter I. No optional patrols, zone XP, purchased items, prior combat skill practice, or bonus attribute points are invented. The one optional sewer safe-room choice is the Stormglass Conductor; the remaining gear is guaranteed quest rewards earned before this encounter. Historical world XP follows the pacing ledger's declared inputs.");
            report.AppendLine();
            report.AppendLine("Builds: starter-unspent keeps starter gear and all earned growth points. Quest-gear-unspent equips the Conductor on Luma, roadcoat on Cairn, Varkh sword and later Gloam mail on Maer, Mirrorweave mantle on Luma, and the Marshal warblade on Maer after Chapter V. Quest-gear-spent uses the same gear; two of every three attribute points go to Strength/Dexterity/Intelligence for warrior/ranger/casters, the rest to Health. Warrior talent spending alternates two Arms to one Guard; ranger Missile, mage Ember and priest Mend receive all their class talent points. Every point is validated and spent by PartyGrowthRules; every item is admitted/equipped by production inventory methods.");
            report.AppendLine();
            report.AppendLine("The deterministic one-turn policy ranks legal production attack forecasts, class strikes and damage spells (including resistance and nearby splash), healing, cure, wards, regeneration and ritual disruption/Rift Seal. It chooses a reachable move using actual path costs, penalizes hazards/exposure, then issues the selected action through CombatController.TryResolveAction. It forfeits unused movement after acting. EnemyAct, initiative, start/end-turn statuses, summoned reinforcements, hazards, victory/defeat and rewards all run normally. Only presentation wall-clock delays are fast-forwarded. Candidate previews use a detached clone of the acting unit; gameplay HP, damage, enemies and turn budgets are never changed to force an outcome.");
            report.AppendLine();
            report.AppendLine("Limits: this is a reproducible heuristic, not an optimal solver or estimated human win probability. The sample has only three deterministic seeds. No summons, terrain construction, out-of-combat enchantments, deliberate multi-turn combos, post-action repositioning or retreats. Ability hit probability and status value use simple utility estimates, although resolution itself is real. Fresh-rest rows do not prove consecutive campaign sustain or the ability to earn the assumed preceding wins. Defeat is evidence that this policy failed, not proof the encounter is impossible; easy victories are evidence that this build and tactic are available, not that every player will find them. Bounds: 35 rounds / 600 decisions; timeouts remain timeouts.");
            report.AppendLine("The action policy is deterministic for a given state. Production-generated GUIDs may break otherwise exact targeting/initiative ties differently between runs; the audit retains that behavior. Seeds therefore select representative rank/random-roll conditions, not a guarantee of bit-identical replay. Preceding quest flags are populated as legal completed history, Chapter VI admission is asserted, and a won Vhal fight must execute the real final-victory mode and completion flag.");
            report.AppendLine();
            report.AppendLine("## Observed outcomes");
            report.AppendLine();
            report.AppendLine("| Encounter | Build | Wins / fights | Mean rounds | Mean living heroes | Mean remaining HP / start HP | Mean elixirs used |");
            report.AppendLine("|---|---|---:|---:|---:|---:|---:|");
            foreach (var group in results.GroupBy(r => new { r.Encounter, r.Build }))
                report.AppendLine($"| {group.Key.Encounter} | {group.Key.Build} | {group.Count(r => r.Outcome == "victory")} / {group.Count()} | {group.Average(r => r.Rounds):0.0} | {group.Average(r => r.Survivors):0.0} | {group.Average(r => r.HpRatio):P0} | {group.Average(r => r.ElixirsUsed):0.0} |");
            report.AppendLine();
            report.AppendLine("## Per-seed evidence");
            report.AppendLine();
            report.AppendLine("HP/MP are the combat-unit values at the outcome, before victory level-ups can refill the persistent party. Enemy HP includes genuine ritual reinforcements. Actions are counted only when accepted by the production command controller.");
            report.AppendLine();
            report.AppendLine("| Encounter | Seed | Build | Entering levels | Outcome | Rounds / decisions | Living | HP remaining / initial | MP remaining / initial | Enemy HP left | Elixirs used | Actions |");
            report.AppendLine("|---|---:|---|---|---|---:|---:|---|---|---:|---:|---|");
            foreach (Result result in results) report.AppendLine(result.Row());
            report.AppendLine();
            report.AppendLine("## Entering builds, seed 727");
            report.AppendLine();
            foreach (Result result in results.Where(r => r.Seed == 727))
            {
                report.AppendLine("### " + result.Encounter + " / " + result.Build);
                report.AppendLine();
                report.AppendLine(result.StartingBuild);
                report.AppendLine();
            }
            string qa = Path.GetFullPath(Path.Combine(Application.dataPath, "../QA"));
            Directory.CreateDirectory(qa);
            File.WriteAllText(Path.Combine(qa, "campaign-tactical-report.md"), report.ToString(), new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(qa, "campaign-tactical-turns.txt"), trace.ToString(), new UTF8Encoding(false));
            Debug.Log("Campaign tactical audit complete: " + results.Count + " actual fights; report and turn trace in " + qa);
        }

        internal sealed class Result
        {
            public string Encounter, Build, Outcome, Levels, StartingBuild, Actions;
            public int Seed, Rounds, Decisions, Survivors, Hp, MaxHp, Mp, MaxMp, EnemyHp, ElixirsUsed, StartingHp, StartingMp;
            public float HpRatio => MaxHp == 0 ? 0 : (float)Hp / MaxHp;
            public string Row() => $"| {Encounter} | {Seed} | {Build} | {Levels} | {Outcome} | {Rounds} / {Decisions} | {Survivors} | {Hp} / {MaxHp} | {Mp} / {MaxMp} | {EnemyHp} | {ElixirsUsed} | {Actions} |";
        }

        private sealed class Choice
        {
            public string Kind, Code;
            public CombatUnit Target;
            public Point Ritual;
            public float Score;
            public string Label => Kind + (string.IsNullOrEmpty(Code) ? "" : ":" + Code);
        }

        internal sealed class Fixture : IDisposable
        {
            private readonly GameObject host;
            private readonly AshenHallsGame game;
            private readonly Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
            public readonly GameState State;
            private readonly Dictionary<string, int> actions = new Dictionary<string, int>();
            private readonly int[] spentAttributes = new int[4], spentTalents = new int[4];
            internal CampaignCombatTelemetry Telemetry;
            internal bool PreferMageArmsFallback;

            public Fixture(int seed)
            {
                host = new GameObject("Actual campaign tactical policy");
                host.SetActive(false);
                host.hideFlags = HideFlags.HideAndDontSave;
                game = host.AddComponent<AshenHallsGame>();
                State = new GameState { Seed = seed, Depth = 1, ContentSetId = ContentSetCatalog.SewerSlice,
                    Mode = GameMode.Explore, ReducedMotion = true, SfxMuted = true, MusicMuted = true,
                    Supplies = 5, Gold = 28, Elixirs = 3 };
                Set("state", State); Set("activeContentSet", ContentSetCatalog.SewerSlice);
                Set("rng", new System.Random(seed)); Set("labSaveBlocked", true);
                Set("launchError", "Tactical audit: inactive presentation host");
                State.Party = (List<PartyMember>)Call("MakeDefaultParty");
            }

            public void Prepare(EncounterId target, string build)
            {
                bool gear = build != "starter-unspent";
                for (int depth = 1; depth <= 6; depth++)
                {
                    State.Depth = depth;
                    if (depth == 6) Milestone(StoryFlags.MeteorCrownAssaultAccepted);
                    foreach (int xp in depth == 2 ? new[] { 30 } : depth == 3 ? new[] { 36 }
                        : depth == 4 ? new[] { 12, 42 } : depth == 5 ? new[] { 16, 48 } : Array.Empty<int>())
                        Call("AwardExperience", xp);
                    int index = 0;
                    foreach (EncounterDefinition encounter in Route(depth))
                    {
                        int site = depth == 3 && index == 0 ? 16 : depth == 4 && (index == 0 || index == 2) ? 18
                            : depth == 5 ? 20 : depth == 6 ? 22 : 0;
                        if (site > 0) Call("AwardExperience", site);
                        if (encounter.Id == target)
                        {
                            PrepareStoryHistory(target, depth);
                            if (build == "quest-gear-spent") SpendGrowth();
                            foreach (PartyMember member in State.Party) Call("RecalculateMember", member);
                            Call("RestorePartyFully");
                            State.Map = (MapData)Call("GenerateMap", depth, State.Seed);
                            // Reset only pre-fight RNG so builds face identical rolls until tactics diverge.
                            Set("rng", new System.Random(State.Seed));
                            return;
                        }
                        List<CombatUnit> enemies = new List<CombatUnit>();
                        for (int i = 0; i < encounter.EnemyCountForDepth(depth); i++)
                            enemies.Add((CombatUnit)Call("MakeEnemy", encounter.EnemyIds[i % encounter.EnemyIds.Length], i, null));
                        Call("ApplyEncounterPlacements", new CombatState { Units = enemies }, encounter);
                        Call("AwardExperience", CombatRewardRules.ExperienceForVictory(enemies, 4));
                        if (depth == 6 && index == 1) Milestone(StoryFlags.MeteorCrownHeartBroken);
                        index++;
                    }
                    foreach (int xp in depth == 3 ? new[] { 16, 48 } : depth == 5 ? new[] { 64 } : Array.Empty<int>()) Call("AwardExperience", xp);
                    if (depth <= 5) Milestone(depth == 1 ? StoryFlags.SewerRewardClaimed : depth == 2 ? StoryFlags.KoboldKingDefeated
                        : depth == 3 ? StoryFlags.RedGateWarningRecovered : depth == 4 ? StoryFlags.EmberglassGateKeyRecovered : StoryFlags.RedGateDebriefed);
                    if (gear)
                    {
                        if (depth == 1) { Equip(ContentSetCatalog.CreateSewerSafeRoomFocus(), 2); Equip(ContentSetCatalog.CreateSewerSliceReward(), 1); }
                        if (depth == 2) Equip(SignatureItemCatalog.CreateUnfathomableSword(), 0);
                        if (depth == 3) Equip(SignatureItemCatalog.CreateGloamReliquaryMail(), 0);
                        if (depth == 4) Equip(ContentSetCatalog.CreateAshglassRoadMantle(), 2);
                        if (depth == 5) Equip(ContentSetCatalog.CreateCrownwardEmberglassWarblade(), 0);
                    }
                }
                throw new InvalidOperationException("Unknown tactical target " + target);
            }

            private void PrepareStoryHistory(EncounterId target, int depth)
            {
                AddFlags(StoryFlags.SewerContractAccepted, StoryFlags.SewerBrokenSluiceCleared,
                    StoryFlags.SewerFoulRunoffCleared, StoryFlags.SewerCisternDenCleared, StoryFlags.OldRoadTeaserUnlocked,
                    StoryFlags.KoboldAmbushSprung, StoryFlags.KoboldAmbushSurvived, StoryFlags.KoboldCaveFound, StoryFlags.KoboldCaveCleared);
                if (depth >= 3) AddFlags(StoryFlags.BoneRoadEntered, StoryFlags.BoneRoadWatchSprung,
                    StoryFlags.BoneRoadWatchDefeated, StoryFlags.GloamRitualBroken);
                if (depth >= 4) AddFlags(StoryFlags.GloamWardenDefeated, StoryFlags.GlassAndAshFrontierSurveyed,
                    StoryFlags.GlassAndAshExpeditionAccepted, StoryFlags.GlassAndAshEntered,
                    StoryFlags.GlasswardAmbushDefeated, StoryFlags.GlassIndexRecovered);
                if (depth >= 5) AddFlags(StoryFlags.GlassAndAshDebriefed, StoryFlags.RedGateAssaultAccepted,
                    StoryFlags.RedGateEntered, StoryFlags.RedGateVanguardDefeated, StoryFlags.OssuaryRoadSealRecovered);
                if (depth >= 6)
                {
                    AddFlags(StoryFlags.CrownroadMarshalDefeated, StoryFlags.MeteorCrownThresholdSurveyed, StoryFlags.MeteorCrownEntered);
                    if (target != EncounterId.MeteorCrownSentries) AddFlags(StoryFlags.MeteorCrownSentriesDefeated);
                    Require(MeteorCrownQuestRules.CanFight(State.StoryFlags, EncounterCatalog.For(target).LegacyStyle), "legal Chapter VI entry history");
                }
                State.StoryChapter = depth;
            }

            private void AddFlags(params string[] flags)
            { foreach (string flag in flags) if (!State.StoryFlags.Contains(flag)) State.StoryFlags.Add(flag); }

            private void Milestone(string flag)
            {
                State.StoryFlags.Add(flag);
                Call("ApplyCampaignMilestoneRewards");
            }

            private void Equip(InventoryItem item, int hero)
            {
                Call("AddInventoryItem", item);
                Require((bool)Call("EquipInventoryItemToMember", item, State.Party[hero], null), "quest item equipped legally");
            }

            internal void SpendGrowth(bool continuous = false)
            {
                for (int i = 0; i < State.Party.Count; i++)
                {
                    PartyMember member = State.Party[i];
                    int n = continuous ? spentAttributes[i] : 0;
                    while (member.StatPoints > 0)
                    {
                        PartyGrowthChoice choice = n++ % 3 == 2 ? PartyGrowthChoice.Health : i == 0 ? PartyGrowthChoice.Strength
                            : i == 1 ? PartyGrowthChoice.Dexterity : PartyGrowthChoice.Intelligence;
                        Require(PartyGrowthRules.TrySpendAttributePoint(member, choice, out string note), note);
                    }
                    if (continuous) spentAttributes[i] = n;
                    n = continuous ? spentTalents[i] : 0;
                    while (member.SkillPoints > 0)
                    {
                        PartyGrowthChoice choice = i == 0 ? (n++ % 3 == 2 ? PartyGrowthChoice.Guard : PartyGrowthChoice.Arms)
                            : i == 1 ? PartyGrowthChoice.Missile : i == 2 ? PartyGrowthChoice.Ember : PartyGrowthChoice.Mend;
                        bool spent = PartyGrowthRules.TrySpendTalentPoint(member, choice, out string note);
                        if (!spent && continuous)
                        {
                            // Real practice can cap a talent before level rewards are spent.
                            // Spend an allowed secondary talent, or keep points for later review.
                            PartyGrowthChoice fallback = i == 0 ? (choice == PartyGrowthChoice.Arms ? PartyGrowthChoice.Guard : PartyGrowthChoice.Arms)
                                : i == 1 ? PartyGrowthChoice.Arms : i == 3 ? PartyGrowthChoice.Guard
                                : PreferMageArmsFallback ? PartyGrowthChoice.Arms : PartyGrowthChoice.Ember;
                            spent = PartyGrowthRules.TrySpendTalentPoint(member, fallback, out note);
                            if (!spent) break;
                        }
                        Require(spent, note);
                    }
                    if (continuous) spentTalents[i] = n;
                }
            }

            public Result Fight(EncounterDefinition encounter, string build, StringBuilder trace, bool alreadyStarted = false)
            {
                actions.Clear();
                Result result = new Result { Encounter = encounter.Banner, Build = build, Seed = State.Seed,
                    Levels = string.Join("/", State.Party.Select(p => p.Level)),
                    StartingBuild = string.Join("; ", State.Party.Select(p => $"{p.Name} L{p.Level}: HP{p.MaxHp}, MP{p.MaxMana}, damage{p.DamageMin}-{p.DamageMax}, defense{p.Defense}; STR/INT/DEX/HP {p.Stats.Strength}/{p.Stats.Intelligence}/{p.Stats.Dexterity}/{p.Stats.Health}; Arms/Missile/Ember/Mend/Guard {p.Skills.Arms}/{p.Skills.Missile}/{p.Skills.Ember}/{p.Skills.Mend}/{p.Skills.Guard}; {p.WeaponName}, {p.ArmorName}")) };
                if (!alreadyStarted) Call("StartCombat", encounter);
                CombatState combat = State.Combat;
                Require(combat != null, "production combat started");
                Require(combat.EncounterStyle == encounter.LegacyStyle, "admitted encounter matches policy target");
                var heroes = combat.Units.Where(u => u.Side == UnitSide.Party && u.PartyIndex >= 0).ToArray();
                result.StartingHp = heroes.Sum(u => u.Hp); result.StartingMp = heroes.Sum(u => u.Mana);
                result.MaxHp = heroes.Sum(u => u.MaxHp); result.MaxMp = heroes.Sum(u => u.MaxMana);
                Telemetry?.BeginFight(this, encounter, combat);
                int startingElixirs = State.Elixirs;
                trace.AppendLine("\n=== " + encounter.Banner + " seed=" + State.Seed + " " + build + " L" + result.Levels + " ===");
                for (int decision = 0; State.Combat != null && State.Mode == GameMode.Combat && decision < 600 && combat.Round <= 35; decision++)
                {
                    result.Decisions++;
                    if ((bool)Get("combatAdvancePending"))
                    {
                        Set("combatAdvanceAt", -1f);
                        Observe(null, "queued-lifecycle", () => Call("CompletePendingCombatAdvance")); continue;
                    }
                    CombatUnit active = (CombatUnit)Call("CurrentUnit");
                    Require(active != null && active.Hp > 0, "lifecycle selected a living actor");
                    int actionRound = combat.Round;
                    string label;
                    if (active.Side == UnitSide.Enemy)
                    {
                        Observe(active, "enemy-ai", () => Call("EnemyAct", active)); label = "enemy AI";
                        Observe(active, "enemy-lifecycle", () => Call("FinishEnemyCombatAction", active));
                    }
                    else label = TakePlayerTurn(active);
                    trace.AppendLine("r" + actionRound + " " + active.Name + " " + label + " | "
                        + string.Join("; ", combat.Units.Select(u => u.Name + "=" + Math.Max(0,u.Hp) + "hp/" + u.Mana + "mp@" + u.X + "," + u.Y)));
                }
                result.Rounds = combat.Round;
                result.Survivors = heroes.Count(u => u.Hp > 0);
                result.Hp = heroes.Sum(u => Math.Max(0, u.Hp)); result.Mp = heroes.Sum(u => Math.Max(0, u.Mana));
                result.EnemyHp = combat.Units.Where(u => u.Side == UnitSide.Enemy).Sum(u => Math.Max(0, u.Hp));
                result.Outcome = result.Survivors == 0 ? "defeat" : result.EnemyHp == 0 && !CombatRitualRules.KeepsCombatAlive(combat) ? "victory" : "timeout";
                if (result.Outcome == "victory" && encounter.Id == EncounterId.MeteorCrownSovereign)
                    Require(State.Mode == GameMode.Victory && MeteorCrownQuestRules.Complete(State.StoryFlags), "real campaign ending follows a tactical victory");
                result.ElixirsUsed = actions.TryGetValue("elixir", out int elixirs) ? elixirs : 0;
                result.Actions = string.Join(", ", actions.OrderBy(p => p.Key).Select(p => p.Key + " " + p.Value));
                Require(result.ElixirsUsed <= startingElixirs, "elixirs are a bounded real resource");
                Telemetry?.EndFight(this, result);
                return result;
            }

            private T Observe<T>(CombatUnit actor, string phase, Func<T> action, string choice = "", string targetId = "")
            {
                return Telemetry == null ? action() : Telemetry.Observe(this, actor, phase, action, choice, targetId);
            }

            private string TakePlayerTurn(CombatUnit active)
            {
                CombatController controller = (CombatController)Call("CombatLifecycle");
                if (!State.Combat.ActionAvailable)
                {
                    Observe(active, "party-lifecycle", () => Call("FinishCombatAction", active, true, 0f, "")); return "end movement";
                }
                Choice choice = BestChoice(active);
                int bx = active.X, by = active.Y;
                float best = choice.Score + PositionScore(active, active.X, active.Y);
                int[,] reachable = (int[,])Call("ReachableMoveCosts", active, State.Combat.MovePoints);
                CombatUnit preview = (CombatUnit)typeof(object).GetMethod("MemberwiseClone", Private).Invoke(active, null);
                for (int y = 0; y < reachable.GetLength(1); y++)
                for (int x = 0; x < reachable.GetLength(0); x++)
                {
                    if (reachable[x,y] > State.Combat.MovePoints || reachable[x,y] < 0 || (x == active.X && y == active.Y)) continue;
                    preview.X = x; preview.Y = y;
                    Choice candidate = BestChoice(preview);
                    float score = candidate.Score + PositionScore(preview, x, y) - reachable[x,y] * .08f;
                    if (score <= best + .01f) continue;
                    best = score; bx = x; by = y;
                }
                if (bx != active.X || by != active.Y)
                {
                    int oldX = active.X, oldY = active.Y;
                    Observe(active, "party-move", () => Call("MoveActiveTo", active, bx, by));
                    Require(active.X == bx && active.Y == by && (active.X != oldX || active.Y != oldY), "chosen move is accepted by real controller");
                }
                choice = BestChoice(active);
                CombatCommandResult result;
                result = Observe(active, "party-action", () => choice.Kind == "elixir" ? controller.TryUseItem(active, 18, 6)
                    : choice.Kind == "guard" ? controller.Guard(active, (int)Call("GuardActionBonus", active))
                    : controller.TryResolveAction(active, () => ResolveChoice(active, choice), true), choice.Label, choice.Target?.Id ?? "");
                Require(result.Success, "production accepted selected action " + choice.Label);
                if (choice.Kind == "guard") Call("ImproveSkill", active, "guard", 1);
                // Items/Guard use their own command entry points; close remaining movement through production lifecycle.
                controller.CompleteAction(active, true);
                if (!actions.ContainsKey(choice.Label)) actions[choice.Label] = 0;
                actions[choice.Label]++;
                Observe(active, "party-lifecycle", () => Call("AfterCombatAction", active, 0f, ""));
                return choice.Label + (choice.Target == null ? "" : " -> " + choice.Target.Name);
            }

            private bool ResolveChoice(CombatUnit active, Choice choice)
            {
                if (choice.Kind == "attack") return (bool)Call("Attack", active, choice.Target);
                if (choice.Kind == "ritual") return (bool)Call("AttackRitual", active, choice.Ritual);
                if (choice.Kind == "ability") return (bool)Call("UseTargetedAbility", active, choice.Code, choice.Target, choice.Target.X, choice.Target.Y);
                if (choice.Kind == "instant") return (bool)Call("UseInstantAbility", active, choice.Code);
                int x = choice.Target?.X ?? choice.Ritual.X, y = choice.Target?.Y ?? choice.Ritual.Y;
                return (bool)Call("CastFormula", active, choice.Code, choice.Target, x, y);
            }

            private Choice BestChoice(CombatUnit actor)
            {
                Choice best = new Choice { Kind = "guard", Score = .1f };
                Action<Choice> consider = candidate => { if (candidate.Score > best.Score) best = candidate; };
                CombatUnit[] enemies = State.Combat.Units.Where(u => u.Side == UnitSide.Enemy && u.Hp > 0).ToArray();
                CombatUnit[] allies = State.Combat.Units.Where(u => u.Side == UnitSide.Party && u.Hp > 0).ToArray();
                foreach (CombatUnit target in enemies)
                {
                    CombatAttackForecast forecast = (CombatAttackForecast)Call("AttackForecast", actor, target);
                    if (forecast.Legal) consider(new Choice { Kind = "attack", Target = target, Score = DamageValue(target, forecast.ExpectedDamage) });
                    foreach (var pair in AbilityDamageMethods)
                    {
                        MartialAbility ability = AbilityCatalog.For(pair.Key);
                        if (!(bool)Call("AbilityUsableNow", actor, ability, null)
                            || !(bool)Call("CanTargetAbility", actor, ability, target, target.X, target.Y, null)) continue;
                        int raw = (int)(pair.Key == "aimedshot" ? Call(pair.Value, actor, target) : Call(pair.Value, actor));
                        int damage = (int)Call("PreviewDamageAfterTraits", target, raw, "physical");
                        float score = DamageValue(target, damage * .85f);
                        if (pair.Key == "charge" && target.Stunned == 0) score += 7;
                        if (pair.Key == "disruptingshot" && target.Stunned == 0) score += 5;
                        if (pair.Key == "broadheadshot" && target.Bleeding == 0) score += 3;
                        if (pair.Key == "pinningshot" && target.Webbed == 0 && target.Range < 3) score += 3;
                        if (pair.Key == "volley") score += enemies.Where(u => u != target && Dist(u,target) <= 1).Sum(u => DamageValue(u, damage * .5f));
                        if (pair.Key == "cleave" && enemies.Count(u => Dist(u,actor) <= 1) > 1) score += damage * .4f;
                        consider(new Choice { Kind = "ability", Code = pair.Key, Target = target, Score = score });
                    }
                }
                foreach (string code in SpellPolicy)
                {
                    FormulaDef formula = FormulaCatalog.All.First(f => f.Code == code);
                    if (!(bool)Call("CanUseFormula", actor, code, null)) continue;
                    int cost = (int)Call("EffectiveFormulaMana", formula, actor);
                    if (cost > actor.Mana) continue;
                    if (code == "SRF")
                    {
                        foreach (Point ritual in State.Combat.Obstacles.Where(o => (bool)Call("IsDisruptableRitual", o)))
                            if (FormulaLegal(formula, actor, null, ritual.X, ritual.Y))
                                consider(new Choice { Kind = "spell", Code = code, Ritual = ritual, Score = 31 - Math.Min(10, ritual.Duration * 2) - cost * .15f });
                        continue;
                    }
                    foreach (CombatUnit target in formula.Target == "enemy" ? enemies : allies)
                    {
                        if (!FormulaLegal(formula, actor, target, target.X, target.Y)) continue;
                        float score = 0;
                        if (formula.Effect == "damage" || formula.Effect == "chain")
                        {
                            Vector2Int range = (Vector2Int)Call("FormulaDamagePreview", formula, actor, target);
                            score = DamageValue(target, (range.x + range.y) * .5f);
                            if (formula.Splash) score += enemies.Where(u => u != target && Dist(u,target) <= 1).Sum(u => DamageValue(u, (range.x + range.y) * .25f));
                            if (formula.Effect == "chain") score += enemies.Where(u => u != target && Dist(u,target) <= 3).Take(2).Sum(u => DamageValue(u, (range.x + range.y) * .2f));
                            if (formula.Status == "stun" && target.Stunned == 0) score += 4;
                            if (formula.Status == "web" && target.Webbed == 0 && target.Range <= 2) score += 3;
                        }
                        if (formula.Effect == "heal")
                        {
                            Vector2Int range = (Vector2Int)Call("FormulaHealPreview", formula, actor);
                            score = HealValue(target, (range.x + range.y) * .5f);
                            if (formula.Splash) score += allies.Where(u => u != target && Dist(u,target) <= 1).Sum(u => HealValue(u, (range.x + range.y) * .25f));
                        }
                        if (formula.Effect == "cure") score = target.Poisoned * 2 + target.Bleeding * 2 + target.Stunned * 13 + target.Sleeping * 13 + target.Hexed * 3 + (target.Range < 3 ? target.Webbed * 3 : 0);
                        if (formula.Status == "shield")
                        {
                            score = WardValue(target, enemies);
                            if (formula.Splash) score += allies.Where(u => u != target && Dist(u,target) <= 1).Sum(u => WardValue(u,enemies));
                        }
                        if (formula.Status == "regen")
                        {
                            score = RegenValue(target);
                            if (formula.Splash) score += allies.Where(u => u != target && Dist(u,target) <= 1).Sum(RegenValue);
                        }
                        consider(new Choice { Kind = "spell", Code = code, Target = target, Score = score - cost * .35f });
                    }
                }
                foreach (Point ritual in State.Combat.Obstacles.Where(o => (bool)Call("IsDisruptableRitual", o)))
                {
                    if (!(bool)Call("CanAttackCombatObstacle", actor, ritual)) continue;
                    int damage = (int)Call("RitualDisruptionDamage", actor, ritual), integrity = (int)Call("RitualIntegrity", ritual);
                    consider(new Choice { Kind = "ritual", Ritual = ritual, Score = (31 - Math.Min(10, ritual.Duration * 2)) * Math.Min(1f, (float)damage / Math.Max(1,integrity)) });
                }
                if (State.Elixirs > 0 && (actor.Hp < actor.MaxHp * .35f || (!string.IsNullOrEmpty(actor.Spell) && actor.Mana < 3)))
                    consider(new Choice { Kind = "elixir", Score = HealValue(actor, 18) + (!string.IsNullOrEmpty(actor.Spell) ? Math.Min(6, actor.MaxMana - actor.Mana) * 2 : 0) - 6 });
                return best;
            }

            private bool FormulaLegal(FormulaDef formula, CombatUnit actor, CombatUnit target, int x, int y) =>
                Math.Abs(actor.X - x) + Math.Abs(actor.Y - y) <= (int)Call("EffectiveFormulaRange", formula, actor)
                && (bool)Call("CanTargetFormula", formula, actor, target, x, y)
                && (bool)Call("HasFormulaLineOfSight", formula, actor, x, y);

            private float PositionScore(CombatUnit actor, int x, int y)
            {
                CombatUnit[] enemies = State.Combat.Units.Where(u => u.Side == UnitSide.Enemy && u.Hp > 0).ToArray();
                float score = enemies.Length == 0 ? 0 : -enemies.Min(u => Dist(u,actor)) * .3f;
                Point terrain = State.Combat.Obstacles.FirstOrDefault(o => o.X == x && o.Y == y);
                if (terrain != null && new[] { "fire", "gas", "web", "curse", "ice", "glyph", "demonrift" }.Contains(terrain.Kind)) score -= 12;
                if (actor.ClassKey != "warrior") score -= enemies.Count(u => Dist(u,actor) <= 2) * 2.5f;
                // Keep ranged heroes dispersed against real splash attacks.
                score -= State.Combat.Units.Count(u => u.Side == UnitSide.Party && u.Id != actor.Id && u.Hp > 0 && Dist(u,actor) <= 1) * .65f;
                return score;
            }
            private static float DamageValue(CombatUnit target, float damage) => Math.Min(target.Hp,damage) + (damage >= target.Hp ? 10 : 0);
            private static float HealValue(CombatUnit target, float heal) => Math.Min(target.MaxHp-target.Hp,heal) * (target.Hp < target.MaxHp*.4f ? 1.65f : target.Hp < target.MaxHp*.7f ? .95f : .3f);
            private static float RegenValue(CombatUnit target) => target.Regenerating > 0 ? 0 : Math.Min(target.MaxHp-target.Hp, 20) * .72f;
            private static float WardValue(CombatUnit target, CombatUnit[] enemies) => target.Shielded > 0 ? 0 : Math.Min(14, enemies.Count(u => Dist(u,target) <= u.Range+2) * 5);
            private static int Dist(CombatUnit a, CombatUnit b) => Math.Abs(a.X-b.X) + Math.Abs(a.Y-b.Y);

            public object Call(string name, params object[] args)
            {
                string key = name + ":" + string.Join(",", args.Select(a => a?.GetType().FullName ?? "null"));
                if (!methods.TryGetValue(key, out MethodInfo method))
                {
                    method = typeof(AshenHallsGame).GetMethods(Private).Single(m => m.Name == name && m.GetParameters().Length == args.Length
                        && m.GetParameters().Select((p,i) => args[i] == null || (p.ParameterType.IsByRef ? p.ParameterType.GetElementType() : p.ParameterType).IsInstanceOfType(args[i])).All(v => v));
                    methods[key] = method;
                }
                try { return method.Invoke(game, args); }
                catch (TargetInvocationException ex) { throw new InvalidOperationException("Production method " + name + " failed", ex.InnerException); }
            }
            internal object Get(string name) => typeof(AshenHallsGame).GetField(name, Private).GetValue(game);
            internal void Set(string name, object value) => typeof(AshenHallsGame).GetField(name, Private).SetValue(game,value);
            public void Dispose() => UnityEngine.Object.DestroyImmediate(host);
        }

        private static IEnumerable<EncounterDefinition> Route(int depth)
        {
            if (depth == 1) return ContentSetCatalog.SewerSliceEncounters;
            EncounterId[] ids = depth == 2 ? new[] { EncounterId.KoboldAmbush, EncounterId.KoboldCave, EncounterId.KoboldKing }
                : depth == 3 ? new[] { EncounterId.BoneRoadWatch, EncounterId.GloamCryptRitual, EncounterId.GloamWarden }
                : depth == 4 ? new[] { EncounterId.GlasswardAmbush, EncounterId.GlassIndexKeepers, EncounterId.AshenPactWarden }
                : depth == 5 ? new[] { EncounterId.RedGateVanguard, EncounterId.OssuaryRoadSeal, EncounterId.CrownroadMarshal }
                : new[] { EncounterId.MeteorCrownSentries, EncounterId.MeteorCrownHeart, EncounterId.MeteorCrownSovereign };
            return ids.Select(EncounterCatalog.For);
        }
        private static void Require(bool condition, string note)
        { if (!condition) throw new InvalidOperationException("Tactical audit: " + note); }
    }
}
