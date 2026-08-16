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
            Formula("FBL", "Fireball", CombatVfxShowcaseScenario.Projectile),
            Formula("MTR", "Meteor Shower", CombatVfxShowcaseScenario.AreaBombardment),
            Formula("RCL", "Cold Lance", CombatVfxShowcaseScenario.Projectile),
            Formula("AST", "Arcane Tempest", CombatVfxShowcaseScenario.AreaStorm),
            Formula("RBT", "Rift Bolt", CombatVfxShowcaseScenario.Projectile),
            Formula("IBD", "Summon Imp", CombatVfxShowcaseScenario.Summon),
            Formula("IBG", "Summon Greater Demon", CombatVfxShowcaseScenario.Summon),
            Formula("DFA", "Abyssal Ascendance", CombatVfxShowcaseScenario.Transformation),
            Formula("DMC", "Doom Circle", CombatVfxShowcaseScenario.GroundField),
            Formula("SLV", "Soul Veil", CombatVfxShowcaseScenario.SupportWard),
            Formula("PBR", "Pact Brand", CombatVfxShowcaseScenario.AreaHex),
            Formula("RLM", "Death Burst", CombatVfxShowcaseScenario.AreaBurst),
            Ability("charge", "Charge", CombatVfxShowcaseScenario.MovementStrike),
            Ability("whirlwind", "Whirlwind", CombatVfxShowcaseScenario.MeleeArea),
            Ability("shadowstep", "Shadowstep", CombatVfxShowcaseScenario.TeleportStrike),
            Ability("volley", "Volley", CombatVfxShowcaseScenario.RangedArea)
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

        private static CombatVfxShowcaseEntry Formula(string id, string name, CombatVfxShowcaseScenario scenario)
        {
            return new CombatVfxShowcaseEntry(id, name, CombatVfxShowcasePowerKind.Formula, scenario, SeedForId(id));
        }

        private static CombatVfxShowcaseEntry Ability(string id, string name, CombatVfxShowcaseScenario scenario)
        {
            return new CombatVfxShowcaseEntry(id, name, CombatVfxShowcasePowerKind.Ability, scenario, SeedForId(id));
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
