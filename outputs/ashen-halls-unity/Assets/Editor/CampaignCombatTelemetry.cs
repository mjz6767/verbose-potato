using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace AshenHalls.Editor
{
    // Editor-only observation. Never intercepts, replaces or repeats a gameplay action.
    internal sealed class CampaignCombatTelemetry
    {
        internal sealed class Sink
        {
            internal readonly StringBuilder Events = new StringBuilder();
            internal readonly List<BattleSummary> Battles = new List<BattleSummary>();
            internal void Write(string qa, string stem)
            {
                File.WriteAllText(Path.Combine(qa,stem+"-combat.jsonl"),Events.ToString(),new UTF8Encoding(false));
                var report=new StringBuilder("# Production combat telemetry\n\n");
                report.AppendLine("Editor-only snapshots, existing floating feedback and game logs; action resolution is separated from movement and automatic lifecycle. Detailed phases cover depths IV–VI; every battle retains entry/end skill vectors. Every sampling operation asserts unchanged serialized game state and RNG internals. No RNG method is called by observation.\n");
                report.AppendLine("Damage feedback is production post-mitigation damage before the HP cap, so includes overkill. HP losses/restoration below are net per unit within each observed phase, not per-hit raw damage. Positive float text can report offered healing. Damage/defense fields are read-only forecasts; exact raw RNG rolls, special chance failures and every hidden ImproveSkill attempt are unavailable. Reach means geometric eligibility, not proof a special was attempted. Skill deltas hide attempts at a cap.\n");
                report.AppendLine("| Seed / policy | Battle | Rounds | Party / enemy action windows | Enemy move-only windows | Enemy basic blocked after movement | Party / enemy damage feedback | Party HP lost / restored | Enemy HP lost / restored | Miss feedback | Skipped turns |\n|---|---|---:|---|---:|---:|---|---|---|---:|---:|");
                foreach(var b in Battles.Where(b=>b.Depth>=4)) report.AppendLine($"| {b.Seed} / {b.Policy} | {b.Encounter} | {b.Rounds} | {b.PartyActions} / {b.EnemyActions} | {b.EnemyMoveOnly} | {b.EnemyPostMoveDenied} | {b.PartyDamageFeedback} / {b.EnemyDamageFeedback} | {b.PartyHpLost} / {b.PartyHpRestored} | {b.EnemyHpLost} / {b.EnemyHpRestored} | {b.Misses} | {b.Skips} |");
                report.AppendLine("\nDamage feedback is attributed to the action owner only for action phases; counterattacks share that phase and must be identified in the event log. Lifecycle damage remains separately labeled in JSONL. Move-only means an enemy moved with no numeric damage feedback and no hit/miss/special attack log; support/cover actions may require manual classification.\n");
                foreach(var b in Battles)
                {
                    report.AppendLine("\n## "+b.Seed+" / "+b.Policy+" / "+b.Encounter+"\n");
                    report.AppendLine("Outcome: "+b.Outcome+". Actions: "+string.Join(", ",b.Actions.OrderByDescending(p=>p.Value).Select(p=>p.Key+"="+p.Value))+".\n");
                    report.AppendLine("Entry skills: "+string.Join("; ",b.EntrySkills)+".\nEnd skills: "+string.Join("; ",b.EndSkills)+".\n");
                }
                File.WriteAllText(Path.Combine(qa,stem+"-combat.md"),report.ToString(),new UTF8Encoding(false));
            }
        }

        internal sealed class BattleSummary
        {
            internal int Seed,Depth,Rounds,PartyActions,EnemyActions,EnemyMoveOnly,EnemyPostMoveDenied,
                PartyDamageFeedback,EnemyDamageFeedback,PartyHpLost,PartyHpRestored,EnemyHpLost,EnemyHpRestored,Misses,Skips;
            internal string Policy,Encounter,Outcome;
            internal string[] EntrySkills,EndSkills;
            internal readonly Dictionary<string,int> Actions=new Dictionary<string,int>();
        }

        [Serializable] internal sealed class Reduction
        {
            public string UnitId,Type;
            public int Guard,Ward,Gear,Race,Demon;
            public string Resist,Weakness;
            public bool Hexed;
        }
        [Serializable] internal sealed class TargetForecast
        {
            public string TargetId,TargetName,BlockReason,DamageType;
            public bool BasicLegal,Ranged,LineOfSight,StrictLineOfSight,SpecialReach;
            public int Distance,Range,HitChance,MinDamage,MaxDamage,ExpectedDamage;
        }
        [Serializable] internal sealed class Snapshot
        {
            public string ActiveActorId,SelectedEnemyTargetId,Mode,EnemyPresentationLabel;
            public int ActorMoveAllowance;
            public float EnemySpecialChance;
            public bool EnemyAttackAfterZeroMove,EnemyAttackAfterOneMove,EnemyAttackAfterTwoMove,EnemyAttackAfterThreeMove;
            public bool[] EnemyAttackAfterMoveBudgets;
            public CombatState Board;
            public TargetForecast[] Targets;
            public Reduction[] Reductions;
        }
        [Serializable] internal sealed class Feedback
        {
            public int Serial,X,Y,IconIndex;
            public string Text;
            public string[] CandidateUnitIds,DamageTypeCandidates;
        }
        [Serializable] internal sealed class Event
        {
            public int Seed,Depth,Sequence;
            public string Policy,Encounter,Phase,ActorId,ActorName,ActorSide,Choice,TargetId;
            public Snapshot Before,After;
            public Feedback[] Feedback;
            public string[] Logs;
            public int EnemySpentMoveBudget=-1;
            public bool EnemyBasicAllowedAfterObservedMove;
        }

        private static readonly string[] DamageTypes={"physical","fire","cold","shock","poison","death","mind","light"};
        private readonly int seed;
        private readonly string policy;
        private readonly Sink sink;
        private CombatState combat;
        private BattleSummary battle;
        private int sequence;
        internal CampaignCombatTelemetry(int seed,string policy,Sink sink) { this.seed=seed;this.policy=policy;this.sink=sink; }

        internal void BeginFight(CampaignTacticalAudit.Fixture f,EncounterDefinition encounter,CombatState board)
        {
            combat=board;
            battle=new BattleSummary { Seed=seed,Policy=policy,Depth=f.State.Depth,Encounter=encounter.Banner,
                EntrySkills=Skills(board),EndSkills=Array.Empty<string>() };
            if(battle.Depth>=4) Write(new Event { Phase="battle-entry",After=Read(f,null) });
        }

        internal T Observe<T>(CampaignTacticalAudit.Fixture f,CombatUnit actor,string phase,Func<T> action,string choice,string targetId)
        {
            if(battle==null || battle.Depth<4) return action();
            Snapshot before=Read(f,actor);
            int firstSerial=(int)f.Get("floatTextSerial");
            var oldLogs=new HashSet<LogEntry>(f.State.Log);
            T result=action();
            Snapshot after=Read(f,actor);
            var feedback=((List<FloatText>)f.Get("floatTexts")).Where(t=>t.Serial>=firstSerial).OrderBy(t=>t.Serial).Select(t=>new Feedback {
                Serial=t.Serial,X=t.X,Y=t.Y,IconIndex=t.IconIndex,Text=t.Text,
                CandidateUnitIds=before.Board.Units.Concat(after.Board.Units).Where(u=>u.X==t.X && u.Y==t.Y).Select(u=>u.Id).Distinct().ToArray(),
                DamageTypeCandidates=t.Text.StartsWith("-",StringComparison.Ordinal) ? DamageTypes.Where(kind=>CombatFeedbackRules.FloatIconIndex("-1",kind)==t.IconIndex).ToArray() : Array.Empty<string>()
            }).ToArray();
            var e=new Event { Phase=phase,ActorId=actor?.Id,ActorName=actor?.Name,ActorSide=actor?.Side.ToString(),Choice=choice,TargetId=targetId,
                Before=before,After=after,Feedback=feedback,Logs=f.State.Log.Where(l=>!oldLogs.Contains(l)).Select(l=>l.Text).ToArray() };
            if(phase=="enemy-ai" && after.Board.Moved)
            {
                e.EnemySpentMoveBudget=Math.Max(0,before.ActorMoveAllowance-after.Board.MovePoints);
                e.EnemyBasicAllowedAfterObservedMove=e.EnemySpentMoveBudget<after.EnemyAttackAfterMoveBudgets.Length
                    && after.EnemyAttackAfterMoveBudgets[e.EnemySpentMoveBudget];
            }
            Accumulate(e); Write(e); return result;
        }

        internal void EndFight(CampaignTacticalAudit.Fixture f,CampaignTacticalAudit.Result result)
        {
            if(battle==null) return;
            battle.Rounds=result.Rounds;battle.Outcome=result.Outcome;battle.EndSkills=Skills(combat);
            if(battle.Depth>=4) Write(new Event { Phase="battle-end",After=Read(f,null) });
            sink.Battles.Add(battle);battle=null;
        }

        private Snapshot Read(CampaignTacticalAudit.Fixture f,CombatUnit actor)
        {
            string stateBefore=JsonUtility.ToJson(f.State),rngBefore=RngFingerprint(f.Get("rng"));
            var snapshot=new Snapshot { ActiveActorId=combat.ActiveId,Mode=f.State.Mode.ToString(),
                Board=JsonUtility.FromJson<CombatState>(JsonUtility.ToJson(combat)),Targets=Array.Empty<TargetForecast>(),
                EnemyAttackAfterMoveBudgets=Array.Empty<bool>(),
                EnemyPresentationLabel=(string)f.Get("enemyActionResolutionLabel") };
            snapshot.Reductions=combat.Units.SelectMany(u=>DamageTypes.Select(type=>new Reduction {
                UnitId=u.Id,Type=type,Guard=u.Guarding?Math.Max(2,u.GuardBonus):0,Ward=u.Shielded>0?3:0,
                Gear=(int)f.Call("GearDamageReduction",u,type),Race=(int)f.Call("RaceDamageReduction",u,type),Demon=(int)f.Call("DemonFormDamageReduction",u),
                Resist=u.Resist,Weakness=u.Weakness,Hexed=u.Hexed>0
            })).ToArray();
            if(actor!=null && ReferenceEquals(f.State.Combat,combat))
            {
                snapshot.ActorMoveAllowance=actor.Webbed>0 || actor.Role=="ritualheart"?0:(int)f.Call("UnitMoveAllowance",actor);
                if(actor.Side==UnitSide.Enemy && actor.Hp>0)
                {
                    snapshot.SelectedEnemyTargetId=((CombatUnit)f.Call("BestEnemyTarget",actor))?.Id;
                    snapshot.EnemySpecialChance=(float)f.Call("EnemySpecialChance",actor);
                    snapshot.EnemyAttackAfterZeroMove=(bool)f.Call("EnemyCanAttackAfterMove",actor,0);
                    snapshot.EnemyAttackAfterOneMove=(bool)f.Call("EnemyCanAttackAfterMove",actor,1);
                    snapshot.EnemyAttackAfterTwoMove=(bool)f.Call("EnemyCanAttackAfterMove",actor,2);
                    snapshot.EnemyAttackAfterThreeMove=(bool)f.Call("EnemyCanAttackAfterMove",actor,3);
                    snapshot.EnemyAttackAfterMoveBudgets=Enumerable.Range(0,Math.Max(5,snapshot.ActorMoveAllowance)+1)
                        .Select(cost=>(bool)f.Call("EnemyCanAttackAfterMove",actor,cost)).ToArray();
                }
                snapshot.Targets=combat.Units.Where(t=>t.Hp>0 && t.Side!=actor.Side).Select(t=> {
                    var forecast=(CombatAttackForecast)f.Call("AttackForecast",actor,t);
                    return new TargetForecast { TargetId=t.Id,TargetName=t.Name,BasicLegal=forecast.Legal,BlockReason=forecast.BlockReason.ToString(),
                        Ranged=forecast.Ranged,LineOfSight=forecast.HasLineOfSight,StrictLineOfSight=(bool)f.Call("HasLineOfSight",actor.X,actor.Y,t.X,t.Y,true),
                        Distance=forecast.Distance,Range=forecast.Range,HitChance=forecast.HitChance,MinDamage=forecast.MinDamage,MaxDamage=forecast.MaxDamage,
                        ExpectedDamage=forecast.ExpectedDamage,DamageType=forecast.DamageType,SpecialReach=actor.Side==UnitSide.Enemy && (bool)f.Call("CanEnemySpecialReach",actor,t) };
                }).ToArray();
            }
            if(stateBefore!=JsonUtility.ToJson(f.State) || rngBefore!=RngFingerprint(f.Get("rng")))
                throw new InvalidOperationException("Combat telemetry observation mutated production state or RNG.");
            return snapshot;
        }

        private void Accumulate(Event e)
        {
            if(e.Phase=="party-action")
            {
                battle.PartyActions++;
                string key=e.ActorName+":"+e.Choice;
                battle.Actions[key]=battle.Actions.TryGetValue(key,out int n)?n+1:1;
            }
            if(e.Phase=="enemy-ai")
            {
                battle.EnemyActions++;
                bool numericDamage=e.Feedback.Any(t=>t.Text.StartsWith("-",StringComparison.Ordinal) && int.TryParse(t.Text.Substring(1),out _));
                bool attackLog=e.Logs.Any(l=>l.Contains(" misses ") || l.Contains(" hits ") || l.Contains(" shoots ") || l.Contains(" casts ") || l.Contains("strikes hard"));
                if(e.After.Board.Moved && !numericDamage && !attackLog) battle.EnemyMoveOnly++;
                if(e.EnemySpentMoveBudget>0 && !e.EnemyBasicAllowedAfterObservedMove) battle.EnemyPostMoveDenied++;
            }
            foreach(var u in e.Before.Board.Units)
            {
                CombatUnit after=e.After.Board.Units.FirstOrDefault(a=>a.Id==u.Id);
                if(after==null) continue;
                int loss=Math.Max(0,u.Hp-after.Hp),heal=Math.Max(0,after.Hp-u.Hp);
                if(u.Side==UnitSide.Party) { battle.PartyHpLost+=loss;battle.PartyHpRestored+=heal; }
                else { battle.EnemyHpLost+=loss;battle.EnemyHpRestored+=heal; }
            }
            foreach(var t in e.Feedback)
            {
                if(t.Text=="miss") battle.Misses++;
                if(t.Text.StartsWith("-",StringComparison.Ordinal) && int.TryParse(t.Text.Substring(1),out int damage))
                {
                    if(e.Phase=="enemy-ai") battle.EnemyDamageFeedback+=damage;
                    if(e.Phase=="party-action") battle.PartyDamageFeedback+=damage;
                }
            }
            battle.Skips+=e.Logs.Count(l=>l.Contains("and loses the turn"));
        }

        private void Write(Event e)
        {
            e.Seed=seed;e.Policy=policy;e.Depth=battle.Depth;e.Encounter=battle.Encounter;e.Sequence=++sequence;
            sink.Events.AppendLine(JsonUtility.ToJson(e));
        }
        private static string[] Skills(CombatState board) => board.Units.Where(u=>u.Side==UnitSide.Party && u.PartyIndex>=0)
            .OrderBy(u=>u.PartyIndex).Select(u=>u.Name+" L"+u.Level+" "+JsonUtility.ToJson(u.Skills)).ToArray();

        private static string RngFingerprint(object rng)
        {
            var text=new StringBuilder();Fingerprint(rng,text,new HashSet<object>(),0);return text.ToString();
        }
        private static void Fingerprint(object value,StringBuilder text,HashSet<object> visited,int depth)
        {
            if(value==null) { text.Append("null;");return; }
            Type type=value.GetType();
            if(type.IsPrimitive || type.IsEnum || value is string) { text.Append(value).Append(';');return; }
            if(depth>6 || !visited.Add(value)) { text.Append(type.FullName).Append(';');return; }
            if(value is IEnumerable array) { foreach(object element in array) Fingerprint(element,text,visited,depth+1);return; }
            foreach(FieldInfo field in type.GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic).OrderBy(fi=>fi.Name))
            { text.Append(field.Name).Append('=');Fingerprint(field.GetValue(value),text,visited,depth+1); }
        }
    }
}
