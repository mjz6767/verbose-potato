using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    // An observation harness: failures are evidence, not permission to repair the party or world.
    public static class CampaignJourneyAudit
    {
        private static readonly int[] Seeds = { 42, 727, 1337 };
        private static readonly string[] Policies = { "prudent", "conserve" };
        private static readonly EncounterDefinition[] Mandatory = ContentSetCatalog.SewerSliceEncounters.Concat(new[] {
            EncounterId.KoboldAmbush, EncounterId.KoboldCave, EncounterId.KoboldKing,
            EncounterId.BoneRoadWatch, EncounterId.GloamCryptRitual, EncounterId.GloamWarden,
            EncounterId.GlasswardAmbush, EncounterId.GlassIndexKeepers, EncounterId.AshenPactWarden,
            EncounterId.RedGateVanguard, EncounterId.OssuaryRoadSeal, EncounterId.CrownroadMarshal,
            EncounterId.MeteorCrownSentries, EncounterId.MeteorCrownHeart, EncounterId.MeteorCrownSovereign
        }.Select(EncounterCatalog.For)).ToArray();

        public static void Run()
        {
            try { RunOrThrow(); EditorApplication.Exit(0); }
            catch (Exception ex) { Debug.LogException(ex); EditorApplication.Exit(1); }
        }

        public static void RunOrThrow()
        {
            RunSelection(Seeds, "campaign-journey");
        }

        public static void RunSeed1337()
        {
            try { RunSelection(new[] { 1337 }, "campaign-journey-1337-probe"); EditorApplication.Exit(0); }
            catch (Exception ex) { Debug.LogException(ex); EditorApplication.Exit(1); }
        }

        public static void RunTelemetryV236()
        {
            try { RunSelection(Seeds, "campaign-journey-v236-baseline", true); EditorApplication.Exit(0); }
            catch (Exception ex) { Debug.LogException(ex); EditorApplication.Exit(1); }
        }

        public static void RunTelemetryV236Candidate()
        {
            try { RunSelection(Seeds, "campaign-journey-v236-candidate", true, true); EditorApplication.Exit(0); }
            catch (Exception ex) { Debug.LogException(ex); EditorApplication.Exit(1); }
        }

        private static void RunSelection(int[] seeds, string outputStem, bool telemetry = false, bool candidateGrowth = false)
        {
            string qa = Path.GetFullPath(Path.Combine(Application.dataPath, "../QA"));
            Directory.CreateDirectory(qa);
            var report = new StringBuilder();
            var trace = new StringBuilder();
            var telemetrySink = telemetry ? new CampaignCombatTelemetry.Sink() : null;
            report.AppendLine("# Connected campaign journey audit\n");
            if (candidateGrowth) report.AppendLine("Candidate policy difference: after Ember reaches its cap, Luma attempts production-legal Arms spending; rejected purchases retain the point. Baseline retains mage points at the Ember cap.\n");
            report.AppendLine("Production quest admissions, cardinal exploration steps/portals, combat lifecycle, rewards, Growth, inventory equipment, camps and Recall. One persistent default party per run; no seeded story flags, synthetic XP/gear, HP/mana edits, assumed wins or RNG resets. Extra roaming fights count separately. Defeat, turn bound or navigation/admission failure ends that run and is retained. This is a policy experiment, not a player completion guarantee.\n");
            report.AppendLine("At each main planned-waypoint decision boundary, prudent recovers if any living hero is below 80% HP or any living caster below 60% mana; conserve uses 40% HP / 20% caster mana. Neither interrupts a return leg to rest immediately after an unexpected roaming battle. Both spend real camps until thresholds, Recall if someone is dead or supplies run out below threshold, and walk back through chapter gates. Required Yara visits also use real Recall (free full heal/revival). Growth is spent continuously 2 primary attributes:1 Health; warrior 2 Arms:1 Guard, ranger Missile, mage Ember, priest Mend. Capped talents fall back to an allowed secondary (warrior Arms/Guard, ranger Arms, priest Guard); a fully capped mage retains points. Each newly earned item is compared with production BestInventoryFit and equipped only when its score is positive. No purchases, optional quest planning, retries, save reloads, out-of-combat spellcasting, or manual item manufacture.\n");
            report.AppendLine("Navigation knows map geometry and authored waypoint identities, chooses a shortest legal path including real interior portals, and avoids optional Camp/Town/RecallCircle healing tiles. Each cardinal step invokes production movement: discovery, roaming movement/engagement and authored approach triggers remain active. Choice callbacks require the enabled choice on the current final dialogue page and at most four choices; rendered input/hit testing is not simulated. No elapsed-time exploration update, fog-limited route search or wall-clock walking delay is simulated. Combat policy is shared with CampaignTacticalAudit; it skips presentation waits, forecasts legal actions greedily, and forgoes post-action movement. GUID tie ordering prevents a bit-for-bit determinism claim.\n");
            report.AppendLine("| Seed | Policy | Outcome | Mandatory wins | Extra fights | Camps | Recovery / quest Recalls | Steps | Portals / waypoints | End levels | End HP / MP | Gold / supplies / elixirs |\n|---|---|---|---:|---:|---:|---|---:|---|---|---|---|");
            var details = new StringBuilder();
            foreach (int seed in seeds)
            foreach (string policy in Policies)
            {
                using (var journey = new Journey(seed, policy, trace, details, outputStem, telemetrySink, candidateGrowth))
                {
                    journey.Run();
                    report.AppendLine(journey.Summary());
                }
                // Preserve every completed/failed run if a later run crashes externally.
                File.WriteAllText(Path.Combine(qa, outputStem + "-report.md"), report.ToString() + details, new UTF8Encoding(false));
                File.WriteAllText(Path.Combine(qa, outputStem + "-turns.txt"), trace.ToString(), new UTF8Encoding(false));
                telemetrySink?.Write(qa, outputStem);
            }
            Debug.Log("Campaign journey audit complete: " + (seeds.Length * Policies.Length) + " connected runs; outcomes (including failures) in " + qa);
        }

        private sealed class Journey : IDisposable
        {
            private readonly CampaignTacticalAudit.Fixture f;
            private GameState S => f.State;
            private readonly int seed;
            private readonly string policy;
            private readonly string outputStem;
            private readonly StringBuilder trace, details, rows = new StringBuilder();
            private readonly HashSet<string> consideredItems = new HashSet<string>();
            private int stage, extras, camps, recoveryRecalls, questRecalls, steps, portals, waypoints, battles;
            private string outcome = "not started";
            private bool returning;

            public Journey(int seed, string policy, StringBuilder trace, StringBuilder details, string outputStem, CampaignCombatTelemetry.Sink telemetrySink, bool candidateGrowth)
            {
                this.seed = seed; this.policy = policy; this.trace = trace; this.details = details;
                this.outputStem = outputStem;
                f = new CampaignTacticalAudit.Fixture(seed);
                f.PreferMageArmsFallback = candidateGrowth;
                if (telemetrySink != null) f.Telemetry = new CampaignCombatTelemetry(seed, policy, telemetrySink);
                f.Set("visualSmokeSaveBlocked", true); // BeginGame clears the lab flag; this independently blocks persistence.
            }

            public void Run()
            {
                trace.AppendLine("\n######## JOURNEY " + seed + " " + policy + " ########");
                rows.AppendLine("\n## " + seed + " / " + policy + "\n");
                rows.AppendLine("| Battle | Kind | Entry levels | Entry HP / MP | Outcome; rounds | End combat HP / MP | Post-reward XP | Gold / supply / elixir delta |\n|---|---|---|---|---|---|---|---|");
                try
                {
                    f.Call("BeginGame");
                    Require(S.Mode == GameMode.Explore && S.Depth == 1 && S.Party.All(p => p.Level == 1), "real new party began");
                    ValidateRecoveryClassification();
                    Event("BEGIN", Snapshot());
                    for (int decisions = 0; stage < 18 && decisions < 160; decisions++)
                    {
                        Require(S.Mode == GameMode.Explore, "journey is ready for exploration");
                        Dismiss(); GrowAndEquip(); Recover();
                        int before = stage;
                        if (stage < 3)
                        {
                            ReturnToDepth(1);
                            UseType(ObjectType.Sewer);
                            if (stage == 2 && (bool)f.Get("showDialogue")) Choose("focus");
                        }
                        else if (stage < 6)
                        {
                            if (!Has(StoryFlags.SewerRewardClaimed))
                            {
                                ReturnToDepth(1); UseType(ObjectType.RatPeltQuest); Dismiss();
                                Require(Has(StoryFlags.SewerRewardClaimed), "Borin actually claimed earned sewer proof");
                                GrowAndEquip();
                            }
                            ReturnToDepth(2);
                            UseId("dusk-market-smoke-cave");
                        }
                        else if (stage < 9)
                        {
                            ReturnToDepth(3); UseSite("gloam-deep-crypt");
                        }
                        else if (stage < 12)
                        {
                            if (!Has(StoryFlags.GlassAndAshExpeditionAccepted))
                            {
                                ReturnToDepth(3);
                                if (!Has(StoryFlags.RedGateWarningRecovered)) UseSite("red-gate-seal");
                                if (!Has(StoryFlags.GlassAndAshFrontierSurveyed)) TakeStairs("glass-and-ash-passage-red-gate", 3);
                                Require(Has(StoryFlags.GlassAndAshFrontierSurveyed), "frontier surveyed through underfoot stairs");
                                Brief("expedition", "accept_expedition", StoryFlags.GlassAndAshExpeditionAccepted);
                            }
                            ReturnToDepth(4);
                            UseSite(stage < 11 ? "glass-lore-library" : "red-gate-seal");
                        }
                        else if (stage < 15)
                        {
                            if (!Has(StoryFlags.RedGateAssaultAccepted)) Brief("redgateplan", "accept_red_gate", StoryFlags.RedGateAssaultAccepted);
                            ReturnToDepth(5);
                            UseSite(stage == 12 ? "red-gate-seal" : stage == 13 ? "gloam-deep-crypt" : "salt-cistern-gate");
                        }
                        else
                        {
                            if (!Has(StoryFlags.MeteorCrownAssaultAccepted))
                            {
                                ReturnToDepth(5);
                                if (!Has(StoryFlags.MeteorCrownThresholdSurveyed)) UseSite("salt-cistern-gate");
                                Require(Has(StoryFlags.MeteorCrownThresholdSurveyed), "threshold surveyed after actual marshal victory");
                                Brief("plan", "accept_meteor_crown", StoryFlags.MeteorCrownAssaultAccepted);
                            }
                            ReturnToDepth(6);
                            UseSite(stage == 15 ? "red-gate-seal" : stage == 16 ? "glass-lore-library" : "salt-cistern-gate");
                            if (stage == 17 && (bool)f.Get("showDialogue")) Choose("stand");
                        }
                        ResolveBattle();
                        Dismiss();
                        Event("DECISION", "stage " + before + " -> " + stage + "; " + Snapshot());
                    }
                    Require(stage == 18, "bounded journey decision budget reached");
                    Require(S.Mode == GameMode.Victory && MeteorCrownQuestRules.Complete(S.StoryFlags), "real final victory state");
                    outcome = "campaign victory";
                }
                catch (Exception ex)
                {
                    outcome = ex.Message.Replace("|", "/").Replace("\n", " ");
                    Event("STOP", ex + "\nRecent game log: " + string.Join("\n", S.Log.Skip(Math.Max(0,S.Log.Count-12)).Select(e=>JsonUtility.ToJson(e))));
                    string stopPath = Path.GetFullPath(Path.Combine(Application.dataPath,"../QA/" + outputStem + "-stop-" + seed + "-" + policy + ".json"));
                    File.WriteAllText(stopPath, JsonUtility.ToJson(S,true),new UTF8Encoding(false));
                }
                rows.AppendLine("\nOutcome: **" + outcome + "**. Final persistent party (includes actual skill practice):\n```json\n" + string.Join("\n", S.Party.Select(p => JsonUtility.ToJson(p))) + "\n```\n");
                details.Append(rows);
            }

            private void Brief(string root, string accept, string expected)
            {
                Recall(true); UseType(ObjectType.OldRoadScout); Choose(root); Choose(accept); Dismiss();
                Require(Has(expected), "explicit Yara acceptance: " + expected);
                GrowAndEquip();
            }

            private void Recover()
            {
                if (S.Party.Any(p => p.Hp <= 0)) { Recall(false); return; }
                int bounded = 0;
                while (NeedsRecovery() && S.Supplies > 0 && bounded++ < 30)
                {
                    string before = Snapshot(); int old = S.Supplies;
                    f.Call("Camp"); Require(S.Supplies == old - 1, "camp spent one actual supply");
                    camps++; Event("CAMP", before + " -> " + Snapshot());
                }
                if (NeedsRecovery()) Recall(false);
            }

            private bool NeedsRecovery()
            {
                float hp = policy == "prudent" ? .8f : .4f;
                float mp = policy == "prudent" ? .6f : .2f;
                return S.Party.Any(p => p.Hp > 0 && (p.Hp < p.MaxHp * hp
                    || CasterManaBelow(p, p.Mana, mp)));
            }

            private static bool CasterManaBelow(PartyMember member, int availableMana, float cutoff)
            {
                return !string.IsNullOrEmpty(StarterPartyCatalog.SpellSchoolForClass(member.ClassKey))
                    && member.MaxMana > 0 && availableMana < member.MaxMana * cutoff;
            }

            private void ValidateRecoveryClassification()
            {
                PartyMember[] casters = S.Party.Where(p => !string.IsNullOrEmpty(StarterPartyCatalog.SpellSchoolForClass(p.ClassKey))).ToArray();
                Require(casters.Length == 2 && casters.Any(p=>p.ClassKey=="mage") && casters.Any(p=>p.ClassKey=="priest"), "default mage and priest participate in mana recovery");
                foreach (PartyMember caster in casters)
                foreach (float cutoff in new[] { .6f, .2f })
                {
                    int threshold = Mathf.CeilToInt(caster.MaxMana * cutoff);
                    // Probe the exact policy predicate without changing the live party's mana.
                    Require(CasterManaBelow(caster, threshold-1, cutoff), caster.Name + " mana below threshold triggers recovery");
                    Require(!CasterManaBelow(caster, threshold, cutoff), caster.Name + " mana at/above threshold does not trigger recovery");
                }
            }

            private void Recall(bool quest)
            {
                Dismiss(); string before = Snapshot(); int gold = S.Gold, supplies = S.Supplies;
                f.Call("RecallToTempleSquare");
                Require(S.Depth == 1 && S.Party.All(p => p.Hp == p.MaxHp && p.Mana == p.MaxMana), "production Recall restores party in Midgaard");
                Require(S.Gold == gold && S.Supplies == supplies, "production Recall is free of gold/supplies");
                if (quest) questRecalls++; else recoveryRecalls++;
                Event(quest ? "QUEST RECALL" : "RECOVERY RECALL", before + " -> " + Snapshot());
            }

            private void ReturnToDepth(int target)
            {
                if (S.Depth > target) Recall(true);
                returning = true;
                try
                {
                    for (int i = 0; S.Depth < target && i < 7; i++)
                    {
                        int depth = S.Depth;
                        if (depth == 1) TakeStairs("old-road-descent-sluice-steps", 2);
                        else if (depth == 2) TakeStairs("bone-road-passage-varkh-hall", 3);
                        else if (depth == 3) TakeStairs("glass-and-ash-passage-red-gate", 4);
                        else if (depth == 4) UseSite("red-gate-seal");
                        else if (depth == 5) UseSite("salt-cistern-gate");
                        Require(S.Depth == depth + 1, "return route admitted depth " + (depth + 1));
                    }
                    Require(S.Depth == target, "requested chapter depth reached");
                }
                finally { returning = false; }
            }

            private void TakeStairs(string id, int targetDepth)
            {
                MapObject stairs = FindId(id);
                Require(WalkTo(stairs), "stairs reached without changing mandatory stage");
                string before = Snapshot(); f.Call("Descend");
                Event("DESCEND", id + "; " + before + " -> " + Snapshot());
                Require(S.Depth == targetDepth, "real stair admission to " + targetDepth);
            }

            private bool UseSite(string id) => UseId("regional-site:" + id);
            private bool UseId(string id) => Use(FindId(id));
            private bool UseType(ObjectType type)
            {
                MapObject obj = S.Map.Objects.FirstOrDefault(o => o != null && o.Type == type);
                Require(obj != null, "map has interaction " + type);
                return Use(obj);
            }
            private MapObject FindId(string id)
            {
                MapObject obj = S.Map.FindObjectById(id);
                Require(obj != null, "map has waypoint " + id + " at depth " + S.Depth);
                return obj;
            }
            private bool Use(MapObject obj)
            {
                if (!WalkTo(obj)) return false;
                string before = Snapshot(); f.Call("ResolveExploreObject", obj);
                Event("USE", obj.Id + " / " + obj.Type + "; " + before + " -> " + Snapshot());
                return true;
            }

            private void Choose(string choice)
            {
                for (int i = 0; (bool)f.Get("showDialogue") && !(bool)f.Call("IsDialogueChoicePage") && i < 30; i++) f.Call("AdvanceDialogue");
                var choices = (DialogueChoiceView[])f.Get("dialogueChoices");
                Require(choices.Length <= 4, "target dialogue fits the four-choice native interaction capacity");
                Require(choices.Any(c => c.Id == choice && c.Enabled), "visible enabled dialogue choice " + choice);
                f.Call("ChooseDialogueChoice", choice); Event("CHOOSE", choice + "; " + Snapshot());
            }

            private void ResolveBattle()
            {
                if (S.Mode != GameMode.Combat) return;
                Require(++battles <= 120, "120-fight bound reached");
                string style = S.Combat.EncounterStyle;
                int expected = Array.FindIndex(Mandatory, e => e.LegacyStyle == style);
                Require(expected < 0 || expected == stage, "authored encounter order: got " + style + ", expected stage " + stage);
                EncounterDefinition encounter = expected >= 0 ? Mandatory[expected] : EncounterCatalog.For(EncounterCatalog.IdForLegacyStyle(style));
                int gold = S.Gold, supply = S.Supplies, elixir = S.Elixirs;
                Event("PRE-COMBAT", Snapshot());
                var result = f.Fight(encounter, policy + "-connected", trace, true);
                rows.AppendLine($"| {encounter.Banner} | {(expected >= 0 ? "mandatory " + (stage+1) : "roaming")} | {result.Levels} | {result.StartingHp}/{result.MaxHp}; {result.StartingMp}/{result.MaxMp} | {result.Outcome}; {result.Rounds} | {result.Hp}/{result.MaxHp}; {result.Mp}/{result.MaxMp} | {string.Join("/", S.Party.Select(p => p.Experience))} | {S.Gold-gold}/{S.Supplies-supply}/{S.Elixirs-elixir} |");
                Event("POST-COMBAT", result.Outcome + "; " + Snapshot());
                Require(result.Outcome == "victory", "combat " + result.Outcome + " at " + encounter.Banner);
                if (expected >= 0) stage++; else extras++;
                Dismiss();
                if (S.Mode == GameMode.Explore) GrowAndEquip();
            }

            private void GrowAndEquip()
            {
                f.SpendGrowth(true);
                foreach (PartyMember member in S.Party) f.Call("RecalculateMember", member);
                foreach (InventoryItem item in S.Inventory.ToArray())
                {
                    if (!InventoryEquipmentRules.IsEquippable(item) || !consideredItems.Add(item.InstanceId)) continue;
                    object[] args = { item, 0, 0 };
                    var best = (PartyMember)f.Call("BestInventoryFit", args);
                    int score = (int)args[2];
                    if (best == null || score <= 0) continue;
                    Require((bool)f.Call("EquipInventoryItemToMember", item, best, null), "earned inventory item equips legally");
                    Event("EQUIP", item.DisplayName + " -> " + best.Name + "; comparison " + score);
                }
            }

            private void Dismiss()
            {
                f.Call("CloseDialogue");
                if ((bool)f.Call("IsLootPopupOpen")) f.Call("DismissLootPopup");
            }

            private sealed class Edge
            {
                public int From, To;
                public MapObject Portal;
            }

            // Full map knowledge is a policy advantage, but every resulting step/portal is real.
            private bool WalkTo(MapObject target)
            {
                Dismiss(); waypoints++;
                Event("WAYPOINT", "depth " + S.Depth + " " + target.Id + "/" + target.Type + " at " + target.X + "," + target.Y);
                int originalStage = stage, originalDepth = S.Depth;
                for (int replans = 0; replans < 24; replans++)
                {
                    List<Edge> path = FindPath(target);
                    if (path == null)
                    {
                        path = FindPath(target, true);
                        if (path != null) Event("BLOCKED-PASSAGE PLAN", "No avoidance route; approach a live patrol and use production directional engagement, then replan. " + target.Id);
                    }
                    if (path == null) DiagnoseBlockedRoute(target);
                    Require(path != null, "no walkable route to " + target.Id + "/" + target.Type + " from " + S.PlayerX + "," + S.PlayerY);
                    bool moved = false;
                    foreach (Edge edge in path)
                    {
                        Require(steps < 30000, "30000-step journey bound reached");
                        if (edge.Portal != null)
                        {
                            Require(Math.Abs(S.PlayerX-edge.Portal.X) + Math.Abs(S.PlayerY-edge.Portal.Y) == 1, "portal used from adjacent tile");
                            f.Call("ResolveExploreObject", edge.Portal); portals++;
                            Event("PORTAL", edge.Portal.Id + " -> " + S.PlayerX + "," + S.PlayerY);
                            Require(S.PlayerX + S.PlayerY*S.Map.Width == edge.To, "native portal arrival matches route");
                        }
                        else
                        {
                            int nx = edge.To % S.Map.Width, ny = edge.To / S.Map.Width;
                            if (!(bool)f.Call("CanStepExplore", nx, ny))
                            {
                                if (CanPlanPatrolEngagement(nx,ny))
                                {
                                    Require(Math.Abs(nx-S.PlayerX)+Math.Abs(ny-S.PlayerY)==1,"blocked patrol approached from adjacent tile");
                                    Event("CLEAR BLOCKED PASSAGE", "engage patrol at " + nx + "," + ny + " through TryMoveOrUseExplore");
                                    f.Call("TryMoveOrUseExplore",nx-S.PlayerX,ny-S.PlayerY);
                                    Require(S.Mode==GameMode.Combat,"production directional input admitted blocking patrol fight");
                                    ResolveBattle(); moved=true; // Progress is the real victory; no walking step is fabricated.
                                    if (stage!=originalStage) return false;
                                }
                                break; // Replan after a moved threat or a real passage-clearing battle.
                            }
                            int oldX = S.PlayerX, oldY = S.PlayerY;
                            f.Call("TryMoveOrUseExplore", nx-oldX, ny-oldY);
                            if (S.PlayerX == oldX && S.PlayerY == oldY && S.Mode != GameMode.Combat) break;
                            if (S.PlayerX != oldX || S.PlayerY != oldY) { steps++; moved = true; }
                            ResolveBattle();
                            if (stage != originalStage) return false;
                            if (S.Party.Any(p => p.Hp <= 0) && !returning) return false;
                            Dismiss();
                            Require(S.Depth == originalDepth, "walking did not unexpectedly change chapter");
                        }
                    }
                    if (AtTarget(target)) return true;
                    Require(moved || replans < 3, "route repeatedly blocked by moving threats");
                }
                throw new InvalidOperationException("24 navigation replans exhausted at " + target.Id);
            }

            private bool AtTarget(MapObject target)
            {
                int distance = Math.Abs(S.PlayerX-target.X) + Math.Abs(S.PlayerY-target.Y);
                return ExplorationTraversalRules.CanStandOnObject(target) ? distance == 0 : distance == 1;
            }

            private bool CanPlanPatrolEngagement(int x,int y)
            {
                return ExplorationTraversalRules.IsStandable(S.Map,x,y)
                    && !(bool)f.Call("IsActiveRoamingThreatHabitatCell",x,y)
                    && f.Call("RoamingThreatAt",x,y,"")!=null;
            }

            private List<Edge> FindPath(MapObject target, bool allowPatrolEngagement = false)
            {
                MapData map = S.Map; int width = map.Width;
                int start = S.PlayerX + S.PlayerY*width;
                var parents = new Dictionary<int, Edge>();
                var queue = new Queue<int>(); var seen = new HashSet<int> { start };
                var walkable = new Dictionary<int, bool>();
                queue.Enqueue(start);
                int found = -1;
                int[] dx = {0,-1,1,0}, dy = {-1,0,0,1};
                while (queue.Count > 0)
                {
                    int cell = queue.Dequeue(), x = cell % width, y = cell / width;
                    int distance = Math.Abs(x-target.X) + Math.Abs(y-target.Y);
                    if (ExplorationTraversalRules.CanStandOnObject(target) ? distance == 0 : distance == 1) { found = cell; break; }
                    for (int d = 0; d < 4; d++)
                    {
                        int nx=x+dx[d], ny=y+dy[d];
                        if (nx<0 || ny<0 || nx>=map.Width || ny>=map.Height) continue;
                        int next=nx+ny*width;
                        MapObject obj=map.FindObjectAt(nx,ny);
                        if (MidgaardInteriorRules.IsPortal(obj))
                        {
                            MapObject exit=map.FindObjectById(obj.TargetId);
                            if (exit != null && MidgaardInteriorRules.TryFindArrival(map,exit,out Point arrival))
                            {
                                int end=arrival.X+arrival.Y*width;
                                if (seen.Add(end)) { parents[end]=new Edge{From=cell,To=end,Portal=obj}; queue.Enqueue(end); }
                            }
                        }
                        if (seen.Contains(next)) continue;
                        if (!walkable.TryGetValue(next,out bool open))
                        {
                            open=((bool)f.Call("CanStepExplore",nx,ny) || allowPatrolEngagement && CanPlanPatrolEngagement(nx,ny))
                                && (obj == null || obj == target || (obj.Type != ObjectType.Camp && obj.Type != ObjectType.Town && obj.Type != ObjectType.RecallCircle));
                            walkable[next]=open;
                        }
                        if (!open) continue;
                        seen.Add(next); parents[next]=new Edge{From=cell,To=next}; queue.Enqueue(next);
                    }
                }
                if (found<0) return null;
                var path = new List<Edge>();
                while (found!=start) { Edge edge=parents[found]; path.Add(edge); found=edge.From; }
                path.Reverse(); return path;
            }

            private void DiagnoseBlockedRoute(MapObject target)
            {
                MapData map = S.Map;
                var goals = ExplorationTraversalRules.CanStandOnObject(target) ? new[] { new Point(target.X,target.Y) }
                    : new[] { new Point(target.X-1,target.Y),new Point(target.X+1,target.Y),new Point(target.X,target.Y-1),new Point(target.X,target.Y+1) };
                foreach (string category in new[] { "static-all", "static-without-healing", "dynamic-all", "dynamic-without-healing" })
                {
                    bool dynamic = category.StartsWith("dynamic",StringComparison.Ordinal), exclude = category.EndsWith("without-healing",StringComparison.Ordinal);
                    Func<int,int,bool> stand = (x,y) =>
                    {
                        if (!(dynamic ? (bool)f.Call("CanStepExplore",x,y) : ExplorationTraversalRules.IsStandable(map,x,y))) return false;
                        MapObject obj=map.FindObjectAt(x,y);
                        return !exclude || obj==null || obj.Type!=ObjectType.Camp && obj.Type!=ObjectType.Town && obj.Type!=ObjectType.RecallCircle;
                    };
                    List<Point> best = goals.Select(g=>ExplorationTraversalRules.FindPath(map,S.PlayerX,S.PlayerY,g.X,g.Y,stand))
                        .Where(p=>p.Count>0).OrderBy(p=>p.Count).FirstOrDefault();
                    Event("ROUTE-DIAGNOSTIC", category + ": " + (best==null ? "NO PATH" : best.Count + " cells: " + string.Join(" ",best.Select(p=>p.X+","+p.Y))));
                    if (best!=null)
                    {
                        Event("ROUTE-OBJECTS", string.Join("; ",best.Select(p=>map.FindObjectAt(p.X,p.Y)).Where(o=>o!=null).Select(o=>o.Id+"/"+o.Type+"@"+o.X+","+o.Y)));
                        Event("ROUTE-DYNAMIC-BLOCKERS", string.Join("; ",best.Where(p=>!(bool)f.Call("CanStepExplore",p.X,p.Y)).Select(p=>p.X+","+p.Y)));
                    }
                }
                Event("ROAMING-STATE", string.Join("\n",S.RoamingThreats.Where(t=>t.Depth==S.Depth).Select(t=>JsonUtility.ToJson(t))));
            }

            private bool Has(string flag) => S.StoryFlags.Contains(flag);
            private string Snapshot() => "D" + S.Depth + "@" + S.PlayerX + "," + S.PlayerY + " mode=" + S.Mode
                + "; L=" + string.Join("/",S.Party.Select(p=>p.Level)) + "; XP=" + string.Join("/",S.Party.Select(p=>p.Experience))
                + "; HP=" + string.Join("/",S.Party.Select(p=>p.Hp+":"+p.MaxHp)) + "; MP=" + string.Join("/",S.Party.Select(p=>p.Mana+":"+p.MaxMana))
                + "; gold/supply/elixir=" + S.Gold + "/" + S.Supplies + "/" + S.Elixirs;
            private void Event(string kind,string text) => trace.AppendLine("J " + seed + " " + policy + " " + kind + " " + text);
            public string Summary() => $"| {seed} | {policy} | {outcome} | {stage}/18 | {extras} | {camps} | {recoveryRecalls} / {questRecalls} | {steps} | {portals} / {waypoints} | {string.Join("/",S.Party.Select(p=>p.Level))} | {S.Party.Sum(p=>p.Hp)}/{S.Party.Sum(p=>p.MaxHp)}; {S.Party.Sum(p=>p.Mana)}/{S.Party.Sum(p=>p.MaxMana)} | {S.Gold}/{S.Supplies}/{S.Elixirs} |";
            public void Dispose() => f.Dispose();
        }

        private static void Require(bool condition, string message)
        { if (!condition) throw new InvalidOperationException(message); }
    }
}
