using System;

namespace AshenHalls
{
    public enum CombatPowerVisualMotif
    {
        Generic,
        Fire,
        Frost,
        Shock,
        Holy,
        Nature,
        Void,
        Rift,
        Ascendance,
        Slash,
        Charge,
        Guard,
        Volley,
        Shadow,
        Smoke
    }

    public readonly struct CombatImpactArtPlan
    {
        public int PrimaryCell { get; }
        public float PrimaryScale { get; }
        public float PrimaryOpacity { get; }
        public int SecondaryCell { get; }
        public float SecondaryScale { get; }
        public float SecondaryOpacity { get; }

        public bool HasPrimary => PrimaryCell >= 0;
        public bool HasSecondary => SecondaryCell >= 0;

        public CombatImpactArtPlan(
            int primaryCell,
            float primaryScale,
            float primaryOpacity,
            int secondaryCell,
            float secondaryScale,
            float secondaryOpacity)
        {
            PrimaryCell = primaryCell;
            PrimaryScale = primaryScale;
            PrimaryOpacity = primaryOpacity;
            SecondaryCell = secondaryCell;
            SecondaryScale = secondaryScale;
            SecondaryOpacity = secondaryOpacity;
        }
    }

    public static class CombatPowerVisualRules
    {
        public const float FireballTravelDuration = 0.40f;

        public static CombatPowerVisualMotif MotifFor(string kind)
        {
            string key = Normalize(kind);
            string supportSpellKey = SupportHexSpellVfxRules.NormalizeKey(kind);
            switch (supportSpellKey)
            {
                case "heal":
                case "ward":
                case "sun":
                case "cleanse":
                    return CombatPowerVisualMotif.Holy;
                case "nature":
                case "web":
                    return CombatPowerVisualMotif.Nature;
                case "poison":
                case "sleep":
                    return CombatPowerVisualMotif.Smoke;
                case "nightveil":
                    return CombatPowerVisualMotif.Shadow;
                case "gravehook":
                case "drainlife":
                case "mindbreak":
                    return CombatPowerVisualMotif.Void;
                case "ashencurse":
                    return CombatPowerVisualMotif.Fire;
            }

            string mageWarlockSpellKey = MageWarlockSpellVfxRules.NormalizeKey(kind);
            switch (mageWarlockSpellKey)
            {
                case "fireball":
                case "meteor":
                    return CombatPowerVisualMotif.Fire;
                case "frost":
                    return CombatPowerVisualMotif.Frost;
                case "tempest":
                    return CombatPowerVisualMotif.Shock;
                case "riftbolt":
                case "lessersummon":
                case "greatersummon":
                case "pactbrand":
                    return CombatPowerVisualMotif.Rift;
                case "ascendance":
                    return CombatPowerVisualMotif.Ascendance;
                case "doomcircle":
                case "soulveil":
                case "hex":
                    return CombatPowerVisualMotif.Void;
            }

            string classSkillKey = ClassSkillVfxRules.NormalizeKey(kind);
            switch (classSkillKey)
            {
                case "charge": return CombatPowerVisualMotif.Charge;
                case "shieldbash":
                case "rally":
                case "sunder": return CombatPowerVisualMotif.Guard;
                case "whirlwind":
                case "execute":
                case "throwknife": return CombatPowerVisualMotif.Slash;
                case "stealth":
                case "ambush":
                case "eviscerate":
                case "shadowstep": return CombatPowerVisualMotif.Shadow;
                case "smokebomb": return CombatPowerVisualMotif.Smoke;
                case "riftpounce": return CombatPowerVisualMotif.Rift;
                case "abyssalwhirl": return CombatPowerVisualMotif.Slash;
                case "soulrend": return CombatPowerVisualMotif.Void;
                case "dreadroar": return CombatPowerVisualMotif.Ascendance;
            }
            if (IsAbilityVisualKey(key, "riftpounce")) return CombatPowerVisualMotif.Rift;
            if (IsAbilityVisualKey(key, "abyssalwhirl")) return CombatPowerVisualMotif.Slash;
            if (IsAbilityVisualKey(key, "soulrend")) return CombatPowerVisualMotif.Void;
            if (IsAbilityVisualKey(key, "dreadroar")) return CombatPowerVisualMotif.Ascendance;
            if (IsAbilityVisualKey(key, "sunder")) return CombatPowerVisualMotif.Guard;
            if (IsAbilityVisualKey(key, "shadowstep")) return CombatPowerVisualMotif.Shadow;
            if (IsAbilityVisualKey(key, "quickshot")) return CombatPowerVisualMotif.Volley;
            if (key == "dawnpulse") return CombatPowerVisualMotif.Holy;
            if (key == "cinderstorm" || key == "ashencurse") return CombatPowerVisualMotif.Fire;
            if (key == "gravehook" || key == "soulveil") return CombatPowerVisualMotif.Void;
            if (ContainsAny(key, "doomcircle", "deathburst", "lifedrain")) return CombatPowerVisualMotif.Void;
            if (ContainsAny(key, "impsummon", "lessersummon", "pactbrand", "pactgate")) return CombatPowerVisualMotif.Rift;
            if (ContainsAny(key, "seal")) return CombatPowerVisualMotif.Holy;
            if (ContainsAny(key, "ascendance", "transform")) return CombatPowerVisualMotif.Ascendance;
            if (ContainsAny(key, "greatersummon", "castgreatersummon", "castpact", "rift", "encounter")) return CombatPowerVisualMotif.Rift;
            if (ContainsAny(key, "deathburst", "castdeathburst", "death", "curse", "casthex", "void")) return CombatPowerVisualMotif.Void;
            if (ContainsAny(key, "whirlwind", "execute", "blade", "crit", "swingheavy")) return CombatPowerVisualMotif.Slash;
            if (ContainsAny(key, "thunderstep", "veilstep")) return CombatPowerVisualMotif.Shock;
            if (ContainsAny(key, "ambush", "eviscerate", "stealth", "veil")) return CombatPowerVisualMotif.Shadow;
            if (ContainsAny(key, "volley", "arrow", "bow", "aimedshot", "pinningshot", "broadhead", "disruptingshot", "scoutmark")) return CombatPowerVisualMotif.Volley;
            if (ContainsAny(key, "rally", "shieldbash", "shield", "guard", "counter")) return CombatPowerVisualMotif.Guard;
            if (ContainsAny(key, "charge")) return CombatPowerVisualMotif.Charge;
            if (ContainsAny(key, "tempest", "shock", "lightning", "arc", "resonance")) return CombatPowerVisualMotif.Shock;
            if (ContainsAny(key, "fire", "ember", "meteor")) return CombatPowerVisualMotif.Fire;
            if (ContainsAny(key, "frost", "ice", "cold")) return CombatPowerVisualMotif.Frost;
            if (ContainsAny(key, "seal", "holy", "light", "heal", "ward", "mend")) return CombatPowerVisualMotif.Holy;
            if (ContainsAny(key, "nature", "tree", "stone", "web", "snare", "bind")) return CombatPowerVisualMotif.Nature;
            if (ContainsAny(key, "smoke", "sleep", "poison", "gas")) return CombatPowerVisualMotif.Smoke;
            return CombatPowerVisualMotif.Generic;
        }

        public static string ImpactKindForFormula(FormulaDef formula, string fallbackKind)
        {
            if (formula == null) return fallbackKind ?? "";
            string code = Normalize(formula.Code);
            string status = Normalize(formula.Status);
            string terrain = Normalize(formula.Terrain);
            string damage = Normalize(formula.DamageType);

            switch (code)
            {
                case "fbl": return "fireball";
                case "mtr": return "meteor";
                case "rcl": return "coldlance";
                case "rbi": return "frostburst";
                case "frb": return "frostbind";
                case "rig": return "arcspark";
                case "rsg": return "thunderclap";
                case "clt": return "chainlightning";
                case "vst": return "thunderstep";
                case "ast": return "tempest";
                case "rlm": return "deathburst";
                case "inh": return "lifedrain";
                case "dmc": return "doomcircle";
                case "ibd": return "lessersummon";
                case "ibf": return "lessersummon";
                case "pbr": return "pactbrand";
                case "ibg": return "greatersummon";
                case "dfa": return "ascendance";
                case "rbt": return "riftbolt";
                case "vrs": return "riftstep";
                case "dwp": return "dawnpulse";
                case "cns": return "cinderstorm";
                case "grh": return "gravehook";
                case "slv": return "soulveil";
                case "acr": return "ashencurse";
            }
            if (status == "sleep" || code == "rms" || code == "dsm") return "sleepmist";
            if (status == "stealth" || code == "nvl") return "shadowveil";
            if (terrain == "web" || status == "web" || code == "wbk" || code == "rkw") return "websnare";
            if (damage == "mind" || status == "hex" || code == "rnh" || code == "rmb") return "voidhex";
            return fallbackKind ?? "";
        }

        public static string CastKindForFormula(FormulaDef formula, string fallbackKind)
        {
            // Keep the production audio profile untouched while carrying a spell-specific
            // visual identity through the anticipation phase.
            return ImpactKindForFormula(formula, fallbackKind);
        }

        public static float BeamDuration(string kind)
        {
            string key = Normalize(kind);
            if (key == "meteor") return 0.46f;
            if (key == "meteorsmall") return 0.36f;
            if (key == "fireball") return FireballTravelDuration;
            if (key == "lightning") return 0.28f;
            if (key == "thunderclap") return 0.14f;
            if (key == "arc") return 0.34f;
            if (key == "death" || key == "hex" || key == "heal") return 0.28f;
            return 0.20f;
        }

        public static float FormulaDeliveryDuration(FormulaDef formula)
        {
            if (formula == null) return 0.20f;
            if (formula.Code == "MTR") return BeamDuration("meteor");
            if (formula.Code == "FBL") return BeamDuration("fireball");
            if (formula.Code == "CNS" || formula.Code == "ACR") return BeamDuration("fireball");
            if (formula.Code == "DWP" || formula.Code == "GRH" || formula.Code == "SLV") return BeamDuration("death");
            if (formula.Effect == "thunderclap") return BeamDuration("thunderclap");
            if (formula.Effect == "teleport") return BeamDuration("arc");
            if (formula.DamageType == "shock") return BeamDuration("lightning");
            if (formula.Effect == "summon"
                || formula.Effect == "transform"
                || formula.Effect == "heal"
                || formula.Effect == "cure"
                || formula.Status == "regen"
                || formula.Status == "shield"
                || formula.DamageType == "death"
                || formula.DamageType == "mind"
                || formula.DamageType == "poison"
                || formula.Status == "hex"
                || formula.Status == "sleep"
                || formula.Terrain == "web"
                || formula.Terrain == "gas"
                || formula.Terrain == "curse"
                || formula.Terrain == "sanctuary")
            {
                return BeamDuration("death");
            }
            return BeamDuration("spell");
        }

        public static float AbilityDeliveryDuration(string abilityId)
        {
            string key = Normalize(abilityId);
            if (key == "riftpounce") return 0.22f;
            if (key == "abyssalwhirl") return 0.10f;
            if (key == "soulrend") return 0.18f;
            if (key == "dreadroar") return 0.08f;
            if (key == "sunder") return 0.18f;
            if (key == "shadowstep") return 0.22f;
            if (key == "quickshot") return 0.24f;
            if (ContainsAny(key, "volley")) return BeamDuration("arc");
            if (ContainsAny(key, "aimedshot", "pinningshot", "broadheadshot", "disruptingshot", "scoutmark", "throwknife")) return BeamDuration("shot");
            if (ContainsAny(key, "charge", "execute", "ambush", "eviscerate", "cleave", "shieldbash", "hamstring")) return 0.18f;
            return 0.08f;
        }

        public static float ProjectileArcHeight(string kind)
        {
            string key = Normalize(kind);
            if (key == "fireball") return 0.42f;
            if (key == "meteor" || key == "meteorsmall") return 0.18f;
            if (key == "arc") return 0.20f;
            return 0f;
        }

        public static int ProjectileAtlasCell(string kind, float progress)
        {
            string key = Normalize(kind);
            float t = Clamp01(progress);
            if (key == "fireball") return t < 0.72f ? 1 : 2;
            if (key == "meteor" || key == "meteorsmall") return 5;
            if (key == "fire") return 2;
            if (key == "ice" || key == "frost") return 11;
            if (key == "shock" || key == "lightning" || key == "arc") return 10;
            if (key == "death" || key == "hex") return 15;
            return -1;
        }

        public static int EffectAtlasCell(string kind)
        {
            string key = Normalize(kind);
            if (IsAbilityVisualKey(key, "riftpounce")) return 13;
            if (IsAbilityVisualKey(key, "abyssalwhirl")) return 8;
            if (IsAbilityVisualKey(key, "soulrend")) return 15;
            if (IsAbilityVisualKey(key, "dreadroar")) return 9;
            if (key == "dawnpulse") return 12;
            if (key == "cinderstorm" || key == "ashencurse") return 3;
            if (key == "gravehook" || key == "soulveil") return 11;
            if (ContainsAny(key, "seal")) return 14;
            if (ContainsAny(key, "meteor")) return key.Contains("crater") ? 7 : 6;
            if (ContainsAny(key, "fireball")) return 2;
            if (ContainsAny(key, "fieldfire", "fire", "ember")) return 3;
            if (ContainsAny(key, "greatersummon", "castgreatersummon", "ascendance", "castascendance", "castpact", "rift", "encounter")) return 13;
            if (ContainsAny(key, "deathburst", "castdeathburst", "death")) return 15;
            if (ContainsAny(key, "hex", "curse")) return 8;
            if (ContainsAny(key, "poison", "gas", "sleep", "smoke", "web", "snare", "bind")) return 9;
            if (ContainsAny(key, "tempest", "shock", "lightning", "thunderclap", "thunderstep", "veilstep", "arc")) return 10;
            if (ContainsAny(key, "frost", "ice")) return 11;
            if (ContainsAny(key, "heal", "mend", "ward")) return 12;
            if (ContainsAny(key, "holy", "light")) return 14;
            return -1;
        }

        public static int ImpactAtlasCell(string kind, float progress)
        {
            string key = Normalize(kind);
            float t = Clamp01(progress);
            if (key.Contains("fireball"))
            {
                return t < 0.72f ? 3 : 4;
            }
            if (key.Contains("meteor")) return t < 0.62f ? 6 : 7;
            return EffectAtlasCell(kind);
        }

        public static float ImpactArtScale(string kind, int intensity, float progress)
        {
            string key = Normalize(kind);
            float t = Clamp01(progress);
            float tier = Math.Max(1, Math.Min(3, intensity));
            if (key.Contains("fireball"))
            {
                float strike = t < 0.18f ? t / 0.18f : 1f - (t - 0.18f) * 0.18f;
                return Clamp(0.78f + strike * 0.52f + tier * 0.05f, 0.78f, 1.45f);
            }
            if (key.Contains("meteor"))
            {
                return Clamp(0.84f + Math.Min(1f, t * 2.2f) * 0.45f + tier * 0.05f, 0.84f, 1.50f);
            }
            return Clamp(0.68f + Math.Min(1f, t * 2.4f) * (0.34f + tier * 0.04f), 0.68f, 1.32f);
        }

        public static int LayeredImpactAtlasCell(CombatPowerVisualMotif motif, float progress)
        {
            float t = Clamp01(progress);
            switch (motif)
            {
                case CombatPowerVisualMotif.Fire: return t < 0.72f ? 1 : 14;
                case CombatPowerVisualMotif.Frost: return 5;
                case CombatPowerVisualMotif.Shock: return 7;
                case CombatPowerVisualMotif.Holy: return 9;
                case CombatPowerVisualMotif.Nature: return 12;
                case CombatPowerVisualMotif.Void:
                case CombatPowerVisualMotif.Rift:
                case CombatPowerVisualMotif.Ascendance: return 11;
                case CombatPowerVisualMotif.Guard: return 9;
                case CombatPowerVisualMotif.Volley: return 15;
                case CombatPowerVisualMotif.Shadow:
                case CombatPowerVisualMotif.Smoke: return 14;
                default: return -1;
            }
        }

        public static CombatImpactArtPlan ImpactArtPlan(string kind, int intensity, float progress)
        {
            string key = Normalize(kind);
            CombatPowerVisualMotif motif = MotifFor(kind);
            int tier = Math.Max(1, Math.Min(3, intensity));
            float t = Clamp01(progress);
            int primaryCell = ImpactAtlasCell(kind, t);
            float primaryScale = ImpactArtScale(kind, tier, t);
            float primaryOpacity = EffectOpacity(motif);
            bool demonAbility = IsDemonAbilityVisualKey(key);
            bool layered = demonAbility ? tier >= 3 : UsesLayeredImpactArt(motif, tier);
            int secondaryCell = layered
                ? demonAbility ? DemonAbilityLayeredImpactAtlasCell(key, t) : LayeredImpactAtlasCell(motif, t)
                : -1;
            layered = secondaryCell >= 0;
            float secondaryScale = layered
                ? Clamp(
                    LayeredImpactBaseScale(motif) + tier * 0.09f + (float)Math.Sin(t * Math.PI) * LayeredImpactPulseScale(motif),
                    1.54f,
                    2.18f)
                : 0f;
            float secondaryOpacity = layered
                ? Clamp(primaryOpacity * LayeredImpactOpacityMultiplier(motif, tier), 0.34f, 0.58f)
                : 0f;
            return new CombatImpactArtPlan(
                primaryCell,
                primaryScale,
                primaryOpacity,
                secondaryCell,
                secondaryScale,
                secondaryOpacity);
        }

        public static CombatImpactArtPlan ReducedMotionImpactArtPlan(string kind, int intensity)
        {
            // Reduced Motion is a single local stamp, not a shortened animation.
            return ImpactArtPlan(kind, intensity, 0f);
        }

        public static int AnticipationAtlasCell(CombatPowerVisualMotif motif)
        {
            switch (motif)
            {
                case CombatPowerVisualMotif.Fire: return 0;
                case CombatPowerVisualMotif.Frost: return 4;
                case CombatPowerVisualMotif.Shock: return 6;
                case CombatPowerVisualMotif.Holy: return 8;
                case CombatPowerVisualMotif.Nature: return 12;
                case CombatPowerVisualMotif.Void:
                case CombatPowerVisualMotif.Rift: return 10;
                case CombatPowerVisualMotif.Ascendance: return 11;
                case CombatPowerVisualMotif.Slash: return 14;
                case CombatPowerVisualMotif.Guard: return 9;
                case CombatPowerVisualMotif.Volley: return 15;
                case CombatPowerVisualMotif.Shadow:
                case CombatPowerVisualMotif.Smoke: return 14;
                default: return -1;
            }
        }

        public static float AnticipationArtScale(CombatPowerVisualMotif motif, int intensity, float progress)
        {
            float tier = Math.Max(1, Math.Min(3, intensity));
            float t = Clamp01(progress);
            float baseScale = motif == CombatPowerVisualMotif.Ascendance || motif == CombatPowerVisualMotif.Rift
                ? 0.78f
                : motif == CombatPowerVisualMotif.Volley ? 0.54f : 0.64f;
            return Clamp(baseScale + tier * 0.06f + t * 0.24f, 0.54f, 1.18f);
        }

        public static float AnticipationOpacity(CombatPowerVisualMotif motif, int intensity, float progress)
        {
            if (motif == CombatPowerVisualMotif.Generic) return 0f;
            float tier = Math.Max(1, Math.Min(3, intensity));
            float t = Clamp01(progress);
            float pulse = 0.74f + (float)Math.Sin(t * Math.PI * (2f + tier)) * 0.12f;
            return Clamp((0.16f + tier * 0.055f + t * 0.10f) * pulse, 0.12f, 0.46f);
        }

        public static int AnticipationRingCount(int intensity)
        {
            return 1;
        }

        public static int AnticipationRingCount(CombatPowerVisualMotif motif, int intensity)
        {
            if (IsMartialMotif(motif)) return 1;
            return AnticipationRingCount(intensity);
        }

        public static bool IsMartialMotif(CombatPowerVisualMotif motif)
        {
            return motif == CombatPowerVisualMotif.Slash
                || motif == CombatPowerVisualMotif.Charge
                || motif == CombatPowerVisualMotif.Guard
                || motif == CombatPowerVisualMotif.Volley
                || motif == CombatPowerVisualMotif.Shadow;
        }

        public static bool UsesRitualCastPresentation(string castKind)
        {
            string key = Normalize(castKind);
            return IsDemonAbilityVisualKey(key)
                || key.StartsWith("cast", StringComparison.Ordinal)
                || key == "formula"
                || key == "spell";
        }

        public static bool UsesLayeredImpactArt(CombatPowerVisualMotif motif, int intensity)
        {
            return intensity >= 3
                && motif != CombatPowerVisualMotif.Generic
                && motif != CombatPowerVisualMotif.Smoke
                && !IsMartialMotif(motif);
        }

        public static string AftermathParticleKind(CombatPowerVisualMotif motif)
        {
            switch (motif)
            {
                case CombatPowerVisualMotif.Fire: return "ember";
                case CombatPowerVisualMotif.Frost: return "shard";
                case CombatPowerVisualMotif.Shock: return "spark";
                case CombatPowerVisualMotif.Holy: return "glint";
                case CombatPowerVisualMotif.Nature: return "leaf";
                case CombatPowerVisualMotif.Void:
                case CombatPowerVisualMotif.Rift:
                case CombatPowerVisualMotif.Ascendance:
                case CombatPowerVisualMotif.Shadow:
                case CombatPowerVisualMotif.Smoke: return "smoke";
                case CombatPowerVisualMotif.Slash:
                case CombatPowerVisualMotif.Charge:
                case CombatPowerVisualMotif.Guard:
                case CombatPowerVisualMotif.Volley: return "streak";
                default: return "mote";
            }
        }

        public static int AftermathParticleCount(CombatPowerVisualMotif motif, int intensity)
        {
            int tier = Math.Max(1, Math.Min(3, intensity));
            if (motif == CombatPowerVisualMotif.Generic) return tier + 2;
            if (motif == CombatPowerVisualMotif.Fire || motif == CombatPowerVisualMotif.Shock) return 4 + tier * 3;
            return 3 + tier * 2;
        }

        public static float AftermathParticleSpeed(CombatPowerVisualMotif motif, int intensity)
        {
            int tier = Math.Max(1, Math.Min(3, intensity));
            switch (motif)
            {
                case CombatPowerVisualMotif.Shock:
                case CombatPowerVisualMotif.Slash:
                case CombatPowerVisualMotif.Volley: return 0.82f + tier * 0.16f;
                case CombatPowerVisualMotif.Charge: return 0.68f + tier * 0.14f;
                case CombatPowerVisualMotif.Holy:
                case CombatPowerVisualMotif.Nature: return 0.34f + tier * 0.08f;
                default: return 0.44f + tier * 0.10f;
            }
        }

        public static float ImpactFlashOpacity(int intensity)
        {
            switch (Math.Max(1, Math.Min(3, intensity)))
            {
                case 3: return 0.10f;
                case 2: return 0.055f;
                default: return 0.025f;
            }
        }

        public static float EffectOpacity(CombatPowerVisualMotif motif)
        {
            switch (motif)
            {
                case CombatPowerVisualMotif.Fire:
                case CombatPowerVisualMotif.Frost:
                case CombatPowerVisualMotif.Shock:
                case CombatPowerVisualMotif.Holy:
                    return 0.78f;
                case CombatPowerVisualMotif.Void:
                case CombatPowerVisualMotif.Rift:
                case CombatPowerVisualMotif.Ascendance:
                    return 0.72f;
                case CombatPowerVisualMotif.Guard:
                case CombatPowerVisualMotif.Volley:
                    return 0.70f;
                default:
                    return 0.62f;
            }
        }

        public static float SemanticImpactOverlayOpacity(
            CombatPowerVisualMotif motif,
            int intensity,
            bool impactArtDrawn,
            bool reducedMotion)
        {
            if (motif == CombatPowerVisualMotif.Generic) return 0f;
            if (reducedMotion) return IsMartialMotif(motif) ? 0.82f : 0.62f;
            if (!impactArtDrawn || IsMartialMotif(motif)) return 1f;

            int tier = Math.Max(1, Math.Min(3, intensity));
            if (tier < 2) return 0f;
            switch (motif)
            {
                case CombatPowerVisualMotif.Shock: return 0.46f;
                case CombatPowerVisualMotif.Fire:
                case CombatPowerVisualMotif.Frost: return 0.38f;
                case CombatPowerVisualMotif.Void:
                case CombatPowerVisualMotif.Rift:
                case CombatPowerVisualMotif.Ascendance: return 0.42f;
                case CombatPowerVisualMotif.Holy:
                case CombatPowerVisualMotif.Nature: return 0.32f;
                default: return 0.28f;
            }
        }

        public static float ReducedMotionStampScale(CombatPowerVisualMotif motif, int intensity)
        {
            int tier = Math.Max(1, Math.Min(3, intensity));
            float motifScale;
            switch (motif)
            {
                case CombatPowerVisualMotif.Rift:
                case CombatPowerVisualMotif.Ascendance: motifScale = 1.02f; break;
                case CombatPowerVisualMotif.Fire:
                case CombatPowerVisualMotif.Shock: motifScale = 0.96f; break;
                case CombatPowerVisualMotif.Smoke:
                case CombatPowerVisualMotif.Shadow: motifScale = 0.86f; break;
                default: motifScale = 0.90f; break;
            }
            return Clamp(motifScale + (tier - 1) * 0.035f, 0.86f, 1.10f);
        }

        private static float LayeredImpactBaseScale(CombatPowerVisualMotif motif)
        {
            switch (motif)
            {
                case CombatPowerVisualMotif.Shock: return 1.66f;
                case CombatPowerVisualMotif.Void:
                case CombatPowerVisualMotif.Rift:
                case CombatPowerVisualMotif.Ascendance: return 1.72f;
                case CombatPowerVisualMotif.Holy: return 1.60f;
                case CombatPowerVisualMotif.Nature:
                case CombatPowerVisualMotif.Frost: return 1.56f;
                default: return 1.62f;
            }
        }

        private static float LayeredImpactPulseScale(CombatPowerVisualMotif motif)
        {
            switch (motif)
            {
                case CombatPowerVisualMotif.Shock: return 0.28f;
                case CombatPowerVisualMotif.Void:
                case CombatPowerVisualMotif.Rift: return 0.22f;
                case CombatPowerVisualMotif.Holy:
                case CombatPowerVisualMotif.Nature: return 0.14f;
                default: return 0.18f;
            }
        }

        private static float LayeredImpactOpacityMultiplier(CombatPowerVisualMotif motif, int tier)
        {
            float baseMultiplier;
            switch (motif)
            {
                case CombatPowerVisualMotif.Fire:
                case CombatPowerVisualMotif.Shock: baseMultiplier = 0.62f; break;
                case CombatPowerVisualMotif.Void:
                case CombatPowerVisualMotif.Rift:
                case CombatPowerVisualMotif.Ascendance: baseMultiplier = 0.66f; break;
                case CombatPowerVisualMotif.Holy:
                case CombatPowerVisualMotif.Nature: baseMultiplier = 0.52f; break;
                default: baseMultiplier = 0.56f; break;
            }
            return baseMultiplier + tier * 0.018f;
        }

        private static string Normalize(string value)
        {
            return (value ?? "").Trim().Replace("_", "").Replace("-", "").ToLowerInvariant();
        }

        private static float Clamp01(float value)
        {
            return Math.Max(0f, Math.Min(1f, value));
        }

        private static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        private static bool ContainsAny(string value, params string[] fragments)
        {
            foreach (string fragment in fragments)
            {
                if (value.IndexOf(fragment, StringComparison.Ordinal) >= 0) return true;
            }
            return false;
        }

        private static bool IsDemonAbilityVisualKey(string key)
        {
            return IsAbilityVisualKey(key, "riftpounce")
                || IsAbilityVisualKey(key, "abyssalwhirl")
                || IsAbilityVisualKey(key, "soulrend")
                || IsAbilityVisualKey(key, "dreadroar");
        }

        private static bool IsAbilityVisualKey(string key, string abilityId)
        {
            return key == abilityId || key == abilityId + "impact";
        }

        private static int DemonAbilityLayeredImpactAtlasCell(string key, float progress)
        {
            if (IsAbilityVisualKey(key, "riftpounce")) return 11;
            if (IsAbilityVisualKey(key, "abyssalwhirl")) return 14;
            if (IsAbilityVisualKey(key, "soulrend")) return 10;
            if (IsAbilityVisualKey(key, "dreadroar")) return progress < 0.58f ? 11 : 14;
            return -1;
        }
    }
}
