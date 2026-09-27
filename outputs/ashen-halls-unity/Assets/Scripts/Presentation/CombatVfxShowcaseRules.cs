using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public enum CombatVfxShowcasePowerKind
    {
        Formula,
        Ability
    }

    public enum CombatVfxShowcaseScenario
    {
        Projectile,
        AreaBombardment,
        AreaStorm,
        Summon,
        Transformation,
        GroundField,
        SupportWard,
        AreaHex,
        AreaBurst,
        MeleeStrike,
        SelfAura,
        MovementStrike,
        MeleeArea,
        TeleportStrike,
        RangedArea
    }

    public readonly struct CombatVfxShowcaseEntry
    {
        public string Id { get; }
        public string DisplayName { get; }
        public CombatVfxShowcasePowerKind Kind { get; }
        public CombatVfxShowcaseScenario Scenario { get; }
        public int StableSeed { get; }

        public bool Supported => !string.IsNullOrWhiteSpace(Id);

        public CombatVfxShowcaseEntry(
            string id,
            string displayName,
            CombatVfxShowcasePowerKind kind,
            CombatVfxShowcaseScenario scenario,
            int stableSeed)
        {
            Id = id ?? "";
            DisplayName = displayName ?? "";
            Kind = kind;
            Scenario = scenario;
            StableSeed = stableSeed > 0 ? stableSeed : 1;
        }
    }

    /// <summary>
    /// Pure, deterministic catalog used by the combat VFX beta showcase. The
    /// ordering deliberately alternates visual shapes instead of following
    /// progression, so Next provides a useful regression tour.
    /// </summary>
    public static class CombatVfxShowcaseRules
    {
        private static readonly CombatVfxShowcaseEntry[] Entries =
        {
            Formula("FBL", CombatVfxShowcaseScenario.Projectile),
            Formula("MTR", CombatVfxShowcaseScenario.AreaBombardment),
            Formula("RCL", CombatVfxShowcaseScenario.Projectile),
            Formula("OBL", CombatVfxShowcaseScenario.Projectile),
            Formula("AST", CombatVfxShowcaseScenario.AreaStorm),
            Formula("VST", CombatVfxShowcaseScenario.TeleportStrike),
            Formula("RBT", CombatVfxShowcaseScenario.Projectile),
            Formula("INH", CombatVfxShowcaseScenario.Projectile),
            Formula("IBD", CombatVfxShowcaseScenario.Summon),
            Formula("IBF", CombatVfxShowcaseScenario.Summon),
            Formula("IBG", CombatVfxShowcaseScenario.Summon),
            Formula("DFA", CombatVfxShowcaseScenario.Transformation),
            Formula("DMC", CombatVfxShowcaseScenario.GroundField),
            Formula("HLC", CombatVfxShowcaseScenario.GroundField),
            Formula("SLV", CombatVfxShowcaseScenario.SupportWard),
            Formula("PBR", CombatVfxShowcaseScenario.AreaHex),
            Formula("VRS", CombatVfxShowcaseScenario.TeleportStrike),
            Formula("RLM", CombatVfxShowcaseScenario.AreaBurst),
            Ability("charge", CombatVfxShowcaseScenario.MovementStrike),
            Ability("whirlwind", CombatVfxShowcaseScenario.MeleeArea),
            Ability("abyssalwhirl", CombatVfxShowcaseScenario.MeleeArea),
            Ability("rally", CombatVfxShowcaseScenario.SelfAura),
            Ability("dreadroar", CombatVfxShowcaseScenario.SelfAura),
            Ability("quickshot", CombatVfxShowcaseScenario.RangedArea),
            Ability("stealth", CombatVfxShowcaseScenario.SelfAura),
            Ability("smokebomb", CombatVfxShowcaseScenario.SelfAura),
            Ability("sunder", CombatVfxShowcaseScenario.MeleeStrike),
            Ability("execute", CombatVfxShowcaseScenario.MeleeStrike),
            Ability("shadowstep", CombatVfxShowcaseScenario.TeleportStrike),
            Ability("riftpounce", CombatVfxShowcaseScenario.TeleportStrike),
            Ability("volley", CombatVfxShowcaseScenario.RangedArea)
        };

        private static readonly IReadOnlyList<CombatVfxShowcaseEntry> ReadOnlyEntries = Array.AsReadOnly(Entries);

        public static IReadOnlyList<CombatVfxShowcaseEntry> Supported => ReadOnlyEntries;
        public static int Count => Entries.Length;

        public static CombatVfxShowcaseEntry At(int index)
        {
            return Entries[Wrap(index, Entries.Length)];
        }

        public static int IndexFor(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return -1;
            string candidate = id.Trim();
            for (int i = 0; i < Entries.Length; i++)
            {
                if (string.Equals(Entries[i].Id, candidate, StringComparison.OrdinalIgnoreCase)) return i;
            }
            return -1;
        }

        public static bool IsSupported(string id)
        {
            return IndexFor(id) >= 0;
        }

        public static bool TryGet(string id, out CombatVfxShowcaseEntry entry)
        {
            int index = IndexFor(id);
            if (index >= 0)
            {
                entry = Entries[index];
                return true;
            }

            entry = default;
            return false;
        }

        public static int NextIndex(int index)
        {
            return Wrap(index + 1, Entries.Length);
        }

        public static int NextIndex(string id)
        {
            return NextIndex(IndexFor(id));
        }

        public static int StableSeedFor(string id)
        {
            int index = IndexFor(id);
            return index >= 0 ? Entries[index].StableSeed : SeedForId(id);
        }

        private static CombatVfxShowcaseEntry Formula(string id, CombatVfxShowcaseScenario scenario)
        {
            return new CombatVfxShowcaseEntry(id, Array.Find(FormulaCatalog.All, formula => formula.Code == id)?.Name ?? id, CombatVfxShowcasePowerKind.Formula, scenario, SeedForId(id));
        }

        private static CombatVfxShowcaseEntry Ability(string id, CombatVfxShowcaseScenario scenario)
        {
            return new CombatVfxShowcaseEntry(id, AbilityCatalog.For(id)?.Name ?? id, CombatVfxShowcasePowerKind.Ability, scenario, SeedForId(id));
        }

        private static int SeedForId(string id)
        {
            // FNV-1a over invariant uppercase characters avoids runtime-randomized
            // string hashes while keeping each showcase replay reproducible.
            unchecked
            {
                uint hash = 2166136261u;
                string value = (id ?? "").Trim();
                for (int i = 0; i < value.Length; i++)
                {
                    hash ^= char.ToUpperInvariant(value[i]);
                    hash *= 16777619u;
                }

                int seed = (int)(hash & 0x7fffffffu);
                return seed > 0 ? seed : 1;
            }
        }

        private static int Wrap(int index, int count)
        {
            if (count <= 0) return 0;
            int wrapped = index % count;
            return wrapped < 0 ? wrapped + count : wrapped;
        }
    }
}
