using System;

namespace AshenHalls
{
    public readonly struct CombatImpactProfile
    {
        public readonly string CastSfx;
        public readonly string ImpactSfx;
        public readonly string AftershockSfx;
        public readonly float CastVolume;
        public readonly float ImpactVolume;
        public readonly float AftershockVolume;
        public readonly float ImpactDelay;
        public readonly float AftershockDelay;
        public readonly int BurstCount;
        public readonly float BurstSpeed;
        public readonly float ShakeMagnitude;
        public readonly float ShakeDuration;
        public readonly float ResolutionDelay;
        public readonly int VisualTier;

        public CombatImpactProfile(
            string castSfx,
            string impactSfx,
            string aftershockSfx,
            float castVolume,
            float impactVolume,
            float aftershockVolume,
            float impactDelay,
            float aftershockDelay,
            int burstCount,
            float burstSpeed,
            float shakeMagnitude,
            float shakeDuration,
            float resolutionDelay,
            int visualTier = 0)
        {
            CastSfx = castSfx ?? "";
            ImpactSfx = impactSfx ?? "";
            AftershockSfx = aftershockSfx ?? "";
            CastVolume = Clamp(castVolume, 0f, 1.4f);
            ImpactVolume = Clamp(impactVolume, 0f, 1.4f);
            AftershockVolume = Clamp(aftershockVolume, 0f, 1.4f);
            ImpactDelay = Clamp(impactDelay, 0f, 0.55f);
            AftershockDelay = Math.Max(ImpactDelay, Clamp(aftershockDelay, 0f, 0.55f));
            BurstCount = Math.Max(0, Math.Min(32, burstCount));
            BurstSpeed = Clamp(burstSpeed, 0.4f, 2.2f);
            ShakeMagnitude = Clamp(shakeMagnitude, 0f, 10f);
            ShakeDuration = Clamp(shakeDuration, 0f, 0.45f);
            ResolutionDelay = Clamp(resolutionDelay, 0.06f, 0.60f);
            VisualTier = Math.Max(0, Math.Min(3, visualTier));
        }

        private static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }

    public static class CombatImpactRules
    {
        public static int VisualIntensity(CombatImpactProfile profile)
        {
            if (profile.VisualTier > 0) return profile.VisualTier;
            if (profile.ResolutionDelay >= 0.52f || profile.ShakeMagnitude >= 5f || profile.BurstCount >= 26) return 3;
            if (profile.ResolutionDelay >= 0.34f || profile.ShakeMagnitude >= 2f || profile.BurstCount >= 14) return 2;
            return 1;
        }

        public static int VisualIntensity(CombatImpactProfile profile, int reactionCount)
        {
            int reactionBonus = reactionCount > 0 ? 1 : 0;
            return Math.Min(3, VisualIntensity(profile) + reactionBonus);
        }

        public static float EchoDuration(CombatImpactProfile profile)
        {
            switch (VisualIntensity(profile))
            {
                case 3: return Math.Min(0.88f, 0.66f + profile.ShakeMagnitude * 0.022f);
                case 2: return 0.60f;
                default: return 0.42f;
            }
        }

        public static float EchoDuration(CombatImpactProfile profile, int reactionCount)
        {
            return Math.Min(0.94f, EchoDuration(profile) + (reactionCount > 0 ? 0.08f : 0f));
        }

        public static float CastAuraDuration(CombatImpactProfile profile)
        {
            return Math.Max(0.32f, Math.Min(0.80f, profile.ImpactDelay + 0.24f + VisualIntensity(profile) * 0.06f));
        }

        public static int PresentationBurstCount(CombatImpactProfile profile, int reactionCount)
        {
            return Math.Min(32, profile.BurstCount + Math.Min(2, Math.Max(0, reactionCount)) * 4);
        }

        public static float AftermathDelay(CombatImpactProfile profile)
        {
            return Math.Min(
                profile.ResolutionDelay,
                Math.Max(profile.ImpactDelay + 0.045f, profile.AftershockDelay - 0.025f));
        }

        public static float ImpactFrameDuration(CombatImpactProfile profile)
        {
            switch (VisualIntensity(profile))
            {
                case 3: return 0.14f;
                case 2: return 0.10f;
                default: return 0.07f;
            }
        }

        public static float SequenceImpactDelay(CombatImpactProfile profile, int index, float spacing = 0.055f)
        {
            int safeIndex = Math.Max(0, index);
            float safeSpacing = Math.Max(0f, Math.Min(0.12f, spacing));
            float latest = Math.Max(profile.ImpactDelay, profile.ResolutionDelay - 0.08f);
            return Math.Min(latest, profile.ImpactDelay + safeIndex * safeSpacing);
        }

        public static CombatImpactProfile ForFormula(FormulaDef formula)
        {
            if (formula == null) return FormulaProfile("spell", "spell", "", 1, 0.22f);

            switch (formula.Code)
            {
                case "GBH":
                    return Profile("castnature", "tree", "ward", 0.50f, 0.98f, 0.44f, 0.20f, 0.30f, 14, 0.90f, 2f, 0.20f, 0.42f);
                case "OIC":
                    return Profile("castmend", "heal", "light", 0.48f, 0.92f, 0.38f, 0.28f, 0.34f, 12, 0.82f, 0f, 0f, 0.40f);
                case "LBC":
                    return Profile("castmend", "heal", "light", 0.52f, 1.02f, 0.48f, 0.28f, 0.38f, 18, 1.04f, 1f, 0.18f, 0.46f);
                case "FBL":
                    return Profile("castember", "fireball", "fieldfire", 0.60f, 1.14f, 0.66f, CombatPowerVisualRules.FireballTravelDuration, 0.50f, 24, 1.72f, 6f, 0.34f, 0.58f, 3);
                case "MTR":
                    return Profile("castember", "meteor", "fieldfire", 0.58f, 1.20f, 0.78f, 0.46f, 0.54f, 32, 2.05f, 8f, 0.40f, 0.60f);
                case "RIG":
                    return Profile("castshock", "shock", "", 0.48f, 0.90f, 0f, CombatPowerVisualRules.BeamDuration("lightning"), 0.32f, 10, 1.14f, 0.5f, 0.10f, 0.34f, 1);
                case "RSG":
                    return Profile("castshock", "shock", "resonance", 0.58f, 1.08f, 0.48f, CombatPowerVisualRules.BeamDuration("thunderclap"), 0.34f, 22, 1.58f, 4f, 0.26f, 0.42f, 2);
                case "CLT":
                    return Profile("castshock", "shock", "resonance", 0.62f, 1.10f, 0.52f, CombatPowerVisualRules.BeamDuration("lightning"), 0.44f, 24, 1.74f, 3f, 0.24f, 0.52f, 2);
                case "RLM":
                    return Profile("castdeathburst", "deathburst", "fieldcurse", 0.56f, 1.14f, 0.54f, 0.10f, 0.28f, 26, 1.72f, 6f, 0.34f, 0.56f);
                case "IBD":
                    return Profile("castpact", "death", "encounter", 0.48f, 0.88f, 0.36f, 0.28f, 0.36f, 14, 1.02f, 2f, 0.20f, 0.42f);
                case "IBF":
                    return Profile("castpact", "death", "encounter", 0.54f, 1.00f, 0.46f, 0.28f, 0.39f, 20, 1.34f, 4f, 0.27f, 0.50f);
                case "IBG":
                    return Profile("castgreatersummon", "greatersummon", "encounter", 0.62f, 1.16f, 0.62f, 0.28f, 0.42f, 30, 1.86f, 7f, 0.38f, 0.60f);
                case "VST":
                    return Profile("castshock", "veilstep", "resonance", 0.60f, 1.04f, 0.48f, CombatPowerVisualRules.BeamDuration("arc"), 0.44f, 22, 1.62f, 3f, 0.24f, 0.50f, 2);
                case "AST":
                    return Profile("casttempest", "tempest", "resonance", 0.68f, 1.20f, 0.74f, 0.34f, 0.46f, 32, 2.04f, 8f, 0.40f, 0.60f, 3);
                case "DFA":
                    return Profile("castascendance", "ascendance", "resonance", 0.70f, 1.18f, 0.70f, 0.28f, 0.44f, 32, 1.92f, 7f, 0.38f, 0.60f);
                case "SRF":
                    return Profile("castseal", "riftseal", "resonance", 0.58f, 1.02f, 0.52f, 0.34f, 0.42f, 22, 1.46f, 3f, 0.24f, 0.50f);
            }

            int intensity = CombatPowerPresentationRules.FormulaIntensity(formula);
            string impact = FormulaImpactSfx(formula);
            string cast = FormulaCastSfx(formula);
            string aftershock = formula.Splash && impact != "spell" ? "spell" : "";
            return FormulaProfile(
                cast,
                impact,
                aftershock,
                intensity,
                BaseResolution(intensity),
                CombatPowerVisualRules.FormulaDeliveryDuration(formula));
        }

        public static CombatImpactProfile ForAbility(MartialAbility ability)
        {
            if (ability == null) return AbilityProfile("ui", "attack", "", 1, 0.22f);
            int intensity = CombatPowerPresentationRules.AbilityIntensity(ability.Id);
            switch ((ability.Id ?? "").ToLowerInvariant())
            {
                case "charge":
                    return Profile("charge", "chargeimpact", "crit", 0.72f, 0.98f, 0.44f, 0.18f, 0.28f, 16, 1.28f, 4f, 0.26f, 0.44f);
                case "whirlwind":
                    return Profile("whirlwind", "whirlwindimpact", "attack", 0.78f, 0.96f, 0.42f, 0.08f, 0.23f, 24, 1.68f, 4f, 0.30f, 0.56f);
                case "execute":
                    return Profile("execute", "executeimpact", "blade", 0.72f, 1.10f, 0.46f, 0.18f, 0.30f, 20, 1.58f, 5f, 0.30f, 0.50f);
                case "ambush":
                    return Profile("ambush", "ambushimpact", "crit", 0.66f, 0.92f, 0.38f, 0.18f, 0.29f, 16, 1.42f, 3f, 0.22f, 0.42f);
                case "eviscerate":
                    return Profile("eviscerate", "eviscerateimpact", "death", 0.70f, 1.00f, 0.34f, 0.18f, 0.30f, 22, 1.62f, 4f, 0.26f, 0.48f);
                case "volley":
                    return Profile("volley", "arrowrain", "bow", 0.70f, 0.98f, 0.40f, 0.34f, 0.44f, 24, 1.54f, 3f, 0.25f, 0.54f);
                case "shieldbash":
                    return Profile("guard", "counter", "attack", 0.58f, 0.92f, 0.30f, 0.18f, 0.28f, 14, 1.12f, 3f, 0.22f, 0.34f);
                case "rally":
                    return Profile("rally", "guard", "ward", 0.62f, 0.94f, 0.40f, 0.08f, 0.22f, 16, 1.08f, 1f, 0.16f, 0.22f);
                case "cleave":
                    return Profile("blade", "attack", "blade", 0.64f, 0.94f, 0.32f, 0.18f, 0.28f, 16, 1.32f, 3f, 0.22f, 0.34f);
                case "stealth":
                    return Profile("stealth", "status", "", 0.54f, 0.76f, 0f, 0.07f, 0.16f, 10, 0.82f, 0f, 0f, 0.22f);
                case "throwknife":
                    return Profile("blade", "blade", "", 0.52f, 0.82f, 0f, 0.20f, 0.26f, 10, 1.02f, 1f, 0.14f, 0.30f);
                case "smokebomb":
                    return Profile("smoke", "smoke", "status", 0.62f, 0.88f, 0.28f, 0.08f, 0.22f, 16, 1.14f, 1f, 0.16f, 0.36f);
                case "hamstring":
                    return Profile("eviscerate", "blade", "pinning", 0.58f, 0.88f, 0.24f, 0.18f, 0.28f, 13, 1.16f, 2f, 0.18f, 0.34f);
                case "aimedshot":
                    return Profile("aimedshot", "bow", "", 0.62f, 0.92f, 0f, 0.20f, 0.26f, 12, 1.20f, 2f, 0.18f, 0.30f);
                case "pinningshot":
                    return Profile("pinning", "bow", "status", 0.58f, 0.88f, 0.24f, 0.20f, 0.30f, 13, 1.18f, 2f, 0.18f, 0.36f);
                case "scoutmark":
                    return Profile("scoutmark", "mark", "status", 0.56f, 0.82f, 0.24f, 0.20f, 0.28f, 11, 0.92f, 0f, 0f, 0.34f);
                case "broadheadshot":
                    return Profile("bow", "blade", "", 0.58f, 0.92f, 0f, 0.20f, 0.27f, 14, 1.24f, 2f, 0.18f, 0.36f);
                case "disruptingshot":
                    return Profile("aimedshot", "counter", "shock", 0.62f, 0.94f, 0.30f, 0.20f, 0.30f, 16, 1.30f, 3f, 0.22f, 0.36f);
                default:
                    return AbilityProfile(AbilityCastSfx(ability.Id), AbilityImpactSfx(ability.Id), "", intensity, BaseResolution(intensity));
            }
        }

        public static CombatImpactProfile ForEnemyPower(string powerKey)
        {
            string key = (powerKey ?? "").Replace("_", "").Replace("-", "").Trim().ToLowerInvariant();
            switch (key)
            {
                case "graveward":
                    return Profile("casthex", "heal", "ward", 0.46f, 0.88f, 0.34f, 0.08f, 0.22f, 12, 0.86f, 0f, 0f, 0.34f);
                case "bonehex":
                    return Profile("casthex", "death", "fieldsnare", 0.52f, 0.92f, 0.34f, 0.09f, 0.24f, 16, 1.18f, 2f, 0.18f, 0.42f);
                case "deathball":
                    return Profile("casthex", "death", "fieldcurse", 0.58f, 1.12f, 0.54f, 0.10f, 0.29f, 27, 1.78f, 6f, 0.34f, 0.56f);
                case "shocksign":
                    return Profile("castshock", "shock", "", 0.42f, 0.82f, 0f, 0.07f, 0.18f, 10, 0.88f, 1f, 0.14f, 0.30f);
                case "coldsplinter":
                    return Profile("castfrost", "ice", "fieldice", 0.46f, 0.90f, 0.30f, 0.08f, 0.20f, 15, 1.12f, 2f, 0.18f, 0.38f);
                case "plaguesigns":
                    return Profile("casthex", "poison", "fieldgas", 0.48f, 0.90f, 0.30f, 0.09f, 0.22f, 15, 1.06f, 2f, 0.18f, 0.40f);
                case "darklight":
                    return Profile("casthex", "death", "", 0.48f, 0.90f, 0f, 0.09f, 0.22f, 15, 1.12f, 2f, 0.18f, 0.40f);
                case "venomdust":
                    return Profile("casthex", "poison", "fieldgas", 0.40f, 0.78f, 0.26f, 0.07f, 0.18f, 10, 0.82f, 0f, 0f, 0.30f);
                case "cindertrail":
                    return Profile("castember", "fieldfire", "", 0.42f, 0.84f, 0f, 0.08f, 0.20f, 12, 1.02f, 1f, 0.15f, 0.34f);
                case "burningpact":
                    return Profile("castpact", "fieldfire", "fieldcurse", 0.50f, 0.96f, 0.34f, 0.09f, 0.25f, 18, 1.30f, 3f, 0.22f, 0.44f);
                case "dreamveil":
                    return Profile("casthex", "sleep", "fieldcurse", 0.46f, 0.82f, 0.26f, 0.09f, 0.22f, 14, 0.96f, 1f, 0.16f, 0.38f);
                case "royalrally":
                    return Profile("rally", "heal", "guard", 0.62f, 1.00f, 0.42f, 0.09f, 0.24f, 22, 1.32f, 3f, 0.22f, 0.50f);
                case "royalcharge":
                    return Profile("charge", "guard", "crit", 0.72f, 1.08f, 0.46f, 0.08f, 0.22f, 24, 1.62f, 6f, 0.34f, 0.56f);
                case "royalfireball":
                    return Profile("castember", "fireball", "fieldfire", 0.64f, 1.18f, 0.66f, CombatPowerVisualRules.FireballTravelDuration, 0.52f, 30, 1.92f, 8f, 0.40f, 0.60f, 3);
                case "royalicelance":
                    return Profile("castfrost", "ice", "fieldice", 0.56f, 1.04f, 0.38f, 0.09f, 0.25f, 22, 1.48f, 5f, 0.28f, 0.50f);
                default:
                    return FormulaProfile("casthex", "spell", "", 1, 0.22f);
            }
        }

        private static CombatImpactProfile FormulaProfile(
            string cast,
            string impact,
            string aftershock,
            int intensity,
            float resolution,
            float deliveryDuration = 0f)
        {
            float impactDelay = Math.Max(0.07f + intensity * 0.015f, deliveryDuration);
            float aftershockDelay = Math.Max(0.18f + intensity * 0.04f, impactDelay + 0.06f);
            float resolutionDelay = Math.Max(resolution, Math.Min(0.60f, aftershockDelay + 0.04f));
            return Profile(
                cast,
                impact,
                aftershock,
                0.44f + intensity * 0.06f,
                0.76f + intensity * 0.10f,
                0.34f + intensity * 0.06f,
                impactDelay,
                aftershockDelay,
                7 + intensity * 5,
                0.68f + intensity * 0.22f,
                intensity <= 1 ? 0f : intensity * 1.2f,
                intensity <= 1 ? 0f : 0.14f + intensity * 0.04f,
                resolutionDelay);
        }

        private static CombatImpactProfile AbilityProfile(string cast, string impact, string aftershock, int intensity, float resolution)
        {
            return Profile(
                cast,
                impact,
                aftershock,
                0.48f + intensity * 0.06f,
                0.76f + intensity * 0.08f,
                0.32f + intensity * 0.05f,
                0.06f + intensity * 0.01f,
                0.17f + intensity * 0.035f,
                6 + intensity * 5,
                0.72f + intensity * 0.20f,
                intensity <= 1 ? 0f : intensity,
                intensity <= 1 ? 0f : 0.12f + intensity * 0.04f,
                resolution);
        }

        private static CombatImpactProfile Profile(
            string cast,
            string impact,
            string aftershock,
            float castVolume,
            float impactVolume,
            float aftershockVolume,
            float impactDelay,
            float aftershockDelay,
            int burstCount,
            float burstSpeed,
            float shakeMagnitude,
            float shakeDuration,
            float resolutionDelay,
            int visualTier = 0)
        {
            return new CombatImpactProfile(
                cast,
                impact,
                aftershock,
                castVolume,
                impactVolume,
                aftershockVolume,
                impactDelay,
                aftershockDelay,
                burstCount,
                burstSpeed,
                shakeMagnitude,
                shakeDuration,
                resolutionDelay,
                visualTier);
        }

        private static float BaseResolution(int intensity)
        {
            return intensity >= 3 ? 0.52f : intensity == 2 ? 0.36f : 0.22f;
        }

        private static string FormulaImpactSfx(FormulaDef formula)
        {
            if (formula == null) return "spell";
            if (formula.Effect == "heal") return "heal";
            if (formula.Effect == "cure" || formula.Status == "shield" || formula.Status == "regen") return "ward";
            if (formula.Effect == "dispel") return "light";
            if (formula.Effect == "summon") return "death";
            if (formula.Terrain == "tree") return "tree";
            if (formula.Terrain == "stone") return "stone";
            if (formula.Terrain == "fire") return "fieldfire";
            if (formula.Terrain == "ice") return "fieldice";
            if (formula.Terrain == "web") return "fieldsnare";
            if (formula.Terrain == "gas") return "fieldgas";
            if (formula.Terrain == "sanctuary") return "fieldholy";
            if (formula.Terrain == "curse") return "fieldcurse";
            switch (formula.DamageType)
            {
                case "fire": return "fire";
                case "cold": return "ice";
                case "shock": return "shock";
                case "light": return "light";
                case "death": return "death";
                case "poison": return "poison";
                case "mind": return "curse";
                default: return "spell";
            }
        }

        private static string FormulaCastSfx(FormulaDef formula)
        {
            if (formula == null) return "formula";
            string school = (formula.School ?? "").ToLowerInvariant();
            if (formula.Effect == "summon" || school.Contains("pact")) return "castpact";
            if (school.Contains("hex") || formula.DamageType == "death" || formula.DamageType == "mind" || formula.Terrain == "curse") return "casthex";
            if (formula.Terrain == "tree" || formula.Terrain == "stone") return "castnature";
            if (formula.DamageType == "light" || formula.Terrain == "sanctuary" || formula.Effect == "dispel") return "castlight";
            if (formula.Effect == "heal" || formula.Effect == "cure" || school.Contains("mend")) return "castmend";
            if (formula.DamageType == "cold" || formula.Terrain == "ice") return "castfrost";
            if (formula.DamageType == "shock") return "castshock";
            if (school.Contains("ember") || formula.DamageType == "fire" || formula.Terrain == "fire") return "castember";
            return "formula";
        }

        private static string AbilityCastSfx(string abilityId)
        {
            switch ((abilityId ?? "").ToLowerInvariant())
            {
                case "stealth": return "stealth";
                case "smokebomb": return "smoke";
                case "rally": return "rally";
                case "aimedshot": return "aimedshot";
                case "pinningshot": return "pinning";
                case "scoutmark": return "scoutmark";
                default: return "ui";
            }
        }

        private static string AbilityImpactSfx(string abilityId)
        {
            switch ((abilityId ?? "").ToLowerInvariant())
            {
                case "stealth": return "status";
                case "smokebomb": return "smoke";
                case "rally": return "guard";
                case "aimedshot": return "bow";
                case "pinningshot": return "bow";
                case "scoutmark": return "mark";
                case "broadheadshot": return "bow";
                case "disruptingshot": return "counter";
                default: return "attack";
            }
        }
    }
}
