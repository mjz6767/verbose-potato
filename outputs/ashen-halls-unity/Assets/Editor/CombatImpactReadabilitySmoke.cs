using System;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class CombatImpactReadabilitySmoke
    {
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " combat impact readability smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " combat impact readability smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            DemonicRitualsStayDistinctAndBounded();
            string[] skills = { "charge", "shieldbash", "rally", "whirlwind", "execute", "sunder", "stealth", "ambush", "smokebomb", "throwknife", "eviscerate", "shadowstep", "riftpounce", "abyssalwhirl", "soulrend", "dreadroar" };
            string[] spells = { "fireball", "meteor", "frost", "tempest", "riftbolt", "lessersummon", "greatersummon", "ascendance", "pactbrand", "doomcircle", "soulveil", "hex" };
            for (int intensity = 1; intensity <= 3; intensity++)
            {
                foreach (string key in skills)
                {
                    ClassSkillVfxArtPlan early = ClassSkillVfxRules.ImpactPlan(key, intensity, 0.18f);
                    ClassSkillVfxArtPlan recovery = ClassSkillVfxRules.ImpactPlan(key, intensity, 0.65f);
                    Require(early.HasPrimary && early.PrimaryScale > recovery.PrimaryScale, key + " skill impact crests near contact");
                    Require(!early.HasSecondary || early.SecondaryOpacity <= early.PrimaryOpacity * 0.25f, key + " ghost leaves the primary strike legible");
                    ClassSkillVfxArtPlan stillA = ClassSkillVfxRules.ImpactPlan(key, intensity, 0f, true);
                    ClassSkillVfxArtPlan stillB = ClassSkillVfxRules.ImpactPlan(key, intensity, 1f, true);
                    Require(stillA.PrimaryScale == stillB.PrimaryScale && stillA.PrimaryOpacity == stillB.PrimaryOpacity && !stillA.HasSecondary,
                        key + " reduced motion holds a single stable impact stamp");
                }
                foreach (string key in spells)
                {
                    MageWarlockSpellVfxArtPlan early = MageWarlockSpellVfxRules.ImpactPlan(key, intensity, 0.18f);
                    MageWarlockSpellVfxArtPlan recovery = MageWarlockSpellVfxRules.ImpactPlan(key, intensity, 0.65f);
                    Require(early.HasPrimary && early.PrimaryScale > recovery.PrimaryScale, key + " spell impact crests near contact");
                    MageWarlockSpellVfxArtPlan stillA = MageWarlockSpellVfxRules.ImpactPlan(key, intensity, 0f, true);
                    MageWarlockSpellVfxArtPlan stillB = MageWarlockSpellVfxRules.ImpactPlan(key, intensity, 1f, true);
                    Require(stillA.PrimaryScale == stillB.PrimaryScale && stillA.PrimaryOpacity == stillB.PrimaryOpacity && !stillA.HasSecondary,
                        key + " reduced motion holds a single stable spell stamp");
                }
            }
            for (int frame = 0; frame <= 100; frame++)
            {
                float snap = CombatPowerVisualRules.ImpactSnap(frame / 100f);
                Require(snap >= 0f && snap <= 1f, "impact crest stays bounded across its full lifetime");
            }
            Require(CombatPowerVisualRules.ImpactSnap(-1f) == 0f, "negative impact time is clamped");
            Require(CombatPowerVisualRules.ImpactSnap(2f) == CombatPowerVisualRules.ImpactSnap(1f), "late impact time is clamped");
        }

        private static void DemonicRitualsStayDistinctAndBounded()
        {
            Require(MageWarlockSpellVfxRules.DemonicStyleFor("IBD") == DemonicSpellVfxStyle.LesserSummon, "imp spell keeps a lesser gate");
            Require(MageWarlockSpellVfxRules.DemonicStyleFor("IBG") == DemonicSpellVfxStyle.GreaterSummon, "greater summon keeps a crowned gate");
            Require(MageWarlockSpellVfxRules.DemonicStyleFor("DFA") == DemonicSpellVfxStyle.Ascendance, "transformation keeps its winged signature");
            Require(MageWarlockSpellVfxRules.DemonicStyleFor("deathburst") == DemonicSpellVfxStyle.Soul, "deathburst uses the soul implosion");
            Require(MageWarlockSpellVfxRules.DemonicStyleFor("FBL") == DemonicSpellVfxStyle.None, "elemental spell never borrows a demonic ritual");
            for (int intensity = 1; intensity <= 3; intensity++)
            {
                float lesser = MageWarlockSpellVfxRules.DemonicGateRadiusCells("IBD", intensity);
                float greater = MageWarlockSpellVfxRules.DemonicGateRadiusCells("IBG", intensity);
                float ascendance = MageWarlockSpellVfxRules.DemonicGateRadiusCells("DFA", intensity);
                Require(lesser < greater && greater < ascendance && ascendance <= 1.02f, "demonic gate tiers remain distinct and board bounded");
            }
            Require(MageWarlockSpellVfxRules.DemonicEmergence(0f) == 0f && MageWarlockSpellVfxRules.DemonicEmergence(0.30f) == 1f,
                "summoned art emerges during the opening impact beat");
            Require(MageWarlockSpellVfxRules.DemonicEmergence(0f, true) == MageWarlockSpellVfxRules.DemonicEmergence(1f, true),
                "reduced-motion emergence stays static");
        }

        private static void Require(bool condition, string label)
        {
            if (!condition) throw new InvalidOperationException(label);
        }
    }
}
