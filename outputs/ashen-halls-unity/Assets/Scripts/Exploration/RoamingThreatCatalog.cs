using System;
using System.Collections.Generic;
using System.Linq;

namespace AshenHalls
{
    public enum RoamingThreatContentProfile
    {
        Any,
        SewerSlice,
        FullPrototype
    }

    public enum RoamingThreatFaction
    {
        Rats,
        Kobolds,
        Drow,
        Undead,
        Demons
    }

    public sealed class RoamingThreatDefinition
    {
        public readonly int Slot;
        public readonly string Id;
        public readonly string Name;
        public readonly string Archetype;
        public readonly int MinDepth;
        public readonly int MaxDepth;
        public readonly string PreferredZoneId;
        public readonly int TargetDistance;
        public readonly RoamingThreatContentProfile ContentProfile;
        public readonly RoamingThreatFaction Faction;
        private readonly string[] enemyIds;

        public IReadOnlyList<string> EnemyIds => enemyIds;
        public RoamingThreatBehaviorProfile BehaviorProfile => RoamingThreatRules.ProfileFor(Faction);

        public RoamingThreatDefinition(
            int slot,
            string id,
            string name,
            string archetype,
            int minDepth,
            int maxDepth,
            string preferredZoneId,
            int targetDistance,
            RoamingThreatContentProfile contentProfile,
            RoamingThreatFaction faction,
            params string[] enemyIds)
        {
            Slot = Math.Max(0, slot);
            Id = id ?? "";
            Name = name ?? "";
            Archetype = archetype ?? "";
            MinDepth = Math.Max(1, minDepth);
            MaxDepth = Math.Max(MinDepth, maxDepth);
            PreferredZoneId = preferredZoneId ?? "";
            TargetDistance = Math.Max(7, Math.Min(24, targetDistance));
            ContentProfile = contentProfile;
            Faction = faction;
            this.enemyIds = enemyIds?.Where(id => !string.IsNullOrEmpty(id)).ToArray() ?? Array.Empty<string>();
        }

        public bool AppliesTo(int depth, bool fullPrototype)
        {
            if (depth < MinDepth || depth > MaxDepth) return false;
            switch (ContentProfile)
            {
                case RoamingThreatContentProfile.SewerSlice: return !fullPrototype;
                case RoamingThreatContentProfile.FullPrototype: return fullPrototype;
                default: return true;
            }
        }
    }

    public static class RoamingThreatCatalog
    {
        private static readonly RoamingThreatDefinition[] definitions =
        {
            // Chapter I is shared by both content sets. Preserve the two original
            // IDs so existing saves retain patrol defeat and respawn state.
            Threat(0, "midgaard-rat-patrol-west", "Gutter Rat Patrol", "rats", 1, 1, "salt-cisterns", 10, RoamingThreatFaction.Rats, "sewerrat", "sewerrat", "giantrat"),
            Threat(1, "midgaard-rat-patrol-east", "Cistern Ratfolk Patrol", "ratfolk", 1, 1, "green-shrine-road", 13, RoamingThreatFaction.Rats, "ratfolk", "ratcutthroat", "giantrat"),
            Threat(2, "midgaard-plague-patrol-north", "Plague-Bell Scavengers", "ratcleric", 1, 1, "gloam-courts", 17, RoamingThreatFaction.Rats, "ratmage", "giantrat", "sewerrat"),
            Threat(3, "midgaard-rat-captain-south", "Dusk-Market Rat Captain", "ratcaptain", 1, 1, "dusk-market", 20, RoamingThreatFaction.Rats, "ratbrute", "ratcutthroat", "ratfolk"),

            // A production save that reaches Chapter II meets the same kobold
            // pressure signposted by the Old Road story, plus one ratfolk remnant.
            Threat(2, "old-road-ratfolk-holdouts", "Old Road Ratfolk Holdouts", "ratfolk", 2, 2, "green-shrine-road", 19, RoamingThreatContentProfile.SewerSlice, RoamingThreatFaction.Rats, "ratfolk", "ratbrute", "ratcutthroat"),

            // Chapter II: kobold scouts lead the road pressure, with the first
            // drow and undead silhouettes appearing on their matching routes.
            Threat(0, "dusk-market-kobold-raiders", "Dusk Market Kobold Raiders", "kobolds", 2, 2, "dusk-market", 11, RoamingThreatFaction.Kobolds, "koboldraider", "koboldslinger", "koboldshield"),
            Threat(1, "quarry-kobold-hexers", "Quarry Kobold Hexers", "koboldshaman", 2, 2, "old-quarry", 15, RoamingThreatFaction.Kobolds, "koboldshaman", "koboldraider", "koboldslinger"),
            Threat(2, "gloam-drow-scouts", "Gloam Drow Scouts", "drowscout", 2, 2, "gloam-courts", 18, RoamingThreatContentProfile.FullPrototype, RoamingThreatFaction.Drow, "drowscout", "drowcrossbow", "drowblade"),
            Threat(3, "fen-restless-dead", "Fen Restless Dead", "undead", 2, 2, "ash-fen", 21, RoamingThreatContentProfile.FullPrototype, RoamingThreatFaction.Undead, "husk", "reaver", "shade"),

            // Chapter III's production route crosses occupied drow ground,
            // disturbed crypts, and the grave watch before the Red Gate warning.
            Threat(0, "bone-road-drow-watch", "Bone Road Drow Watch", "drowscout", 3, 3, "glass-warrens", 12, RoamingThreatContentProfile.SewerSlice, RoamingThreatFaction.Drow, "drowscout", "drowcrossbow", "drowmage", "drowpriest"),
            Threat(1, "gloam-crypt-procession", "Gloam Crypt Procession", "bonepriest", 3, 3, "gloam-courts", 17, RoamingThreatContentProfile.SewerSlice, RoamingThreatFaction.Undead, "bonepriest", "shade", "husk", "reaver"),
            Threat(2, "red-gate-grave-watch", "Red Gate Grave Watch", "revenant", 3, 3, "red-gate", 22, RoamingThreatContentProfile.SewerSlice, RoamingThreatFaction.Undead, "reaver", "bonepriest", "shade", "husk"),

            // Chapter IV keeps normal play bounded to three authored faction
            // bands, and every roster uses an explicitly active campaign enemy.
            Threat(0, "glassward-drow-levy", "Glassward Drow Levy", "drowmage", 4, 4, "glass-warrens", 12, RoamingThreatContentProfile.SewerSlice, RoamingThreatFaction.Drow, "drowscout", "drowcrossbow", "drowmage", "glassmage"),
            Threat(1, "ash-fen-bone-procession", "Ash Fen Bone Procession", "bonepriest", 4, 4, "ash-fen", 17, RoamingThreatContentProfile.SewerSlice, RoamingThreatFaction.Undead, "bonepriest", "shade", "husk", "reaver"),
            Threat(2, "red-gate-cinder-pact", "Red Gate Cinder Pact", "lesserdemon", 4, 4, "red-gate", 22, RoamingThreatContentProfile.SewerSlice, RoamingThreatFaction.Demons, "lesserdemon", "cinderling", "cinderling"),

            // Chapter V moves the production campaign inside the Red Gate with
            // one readable band from each faction holding the crownward road.
            Threat(0, "inner-gate-cinder-vanguard", "Inner Gate Cinder Vanguard", "lesserdemon", 5, 5, "red-gate", 12, RoamingThreatContentProfile.SewerSlice, RoamingThreatFaction.Demons, "lesserdemon", "cinderling", "cinderling", "cinderling"),
            Threat(1, "crownroad-ossuary-column", "Crownroad Ossuary Column", "gloamknight", 5, 5, "gloam-courts", 17, RoamingThreatContentProfile.SewerSlice, RoamingThreatFaction.Undead, "gloamknight", "bonepriest", "shade", "reaver"),
            Threat(2, "emberglass-drow-conclave", "Emberglass Drow Conclave", "drowmage", 5, 5, "glass-warrens", 22, RoamingThreatContentProfile.SewerSlice, RoamingThreatFaction.Drow, "drowpriest", "drowmage", "glassmage", "drowcrossbow"),

            // Chapter III: organized drow and death-cult patrols replace the
            // tentative scouts while kobolds remain on their established roads.
            Threat(0, "quarry-kobold-vanguard", "Quarry Kobold Vanguard", "kobolds", 3, 3, "old-quarry", 11, RoamingThreatContentProfile.FullPrototype, RoamingThreatFaction.Kobolds, "koboldshield", "koboldraider", "koboldslinger", "koboldshaman"),
            Threat(1, "glass-drow-magi", "Glass-Warren Drow Magi", "drowmage", 3, 3, "glass-warrens", 15, RoamingThreatContentProfile.FullPrototype, RoamingThreatFaction.Drow, "drowmage", "drowcrossbow", "drowscout", "drowpriest"),
            Threat(2, "gloam-reaver-patrol", "Gloam Reaver Patrol", "undead", 3, 3, "gloam-courts", 18, RoamingThreatContentProfile.FullPrototype, RoamingThreatFaction.Undead, "reaver", "husk", "shade", "bonepriest"),
            Threat(3, "red-mile-bone-priests", "Red Mile Bone Priests", "bonepriest", 3, 3, "red-gate", 21, RoamingThreatContentProfile.FullPrototype, RoamingThreatFaction.Undead, "bonepriest", "husk", "shade", "reaver"),

            // Chapters IV-V: five separated faction bands make the outer circuit
            // feel occupied without turning every travel step into a pursuit.
            Threat(0, "market-kobold-warband", "Market Kobold Warband", "koboldshaman", 4, 5, "dusk-market", 10, RoamingThreatContentProfile.FullPrototype, RoamingThreatFaction.Kobolds, "koboldshaman", "koboldshield", "koboldraider", "koboldslinger"),
            Threat(1, "glass-drow-warband", "Glass-Warren Drow Warband", "drow", 4, 5, "glass-warrens", 14, RoamingThreatContentProfile.FullPrototype, RoamingThreatFaction.Drow, "drowblade", "drowcrossbow", "drowmage", "drowpriest"),
            Threat(2, "gloam-dead-watch", "Gloam Dead Watch", "undead", 4, 5, "gloam-courts", 17, RoamingThreatContentProfile.FullPrototype, RoamingThreatFaction.Undead, "reaver", "husk", "shade", "bonepriest"),
            Threat(3, "fen-bone-procession", "Fen Bone Procession", "bonepriest", 4, 5, "ash-fen", 20, RoamingThreatContentProfile.FullPrototype, RoamingThreatFaction.Undead, "bonepriest", "husk", "shade", "reaver"),
            Threat(4, "red-gate-lesser-demons", "Red Gate Lesser Demons", "lesserdemon", 4, 5, "red-gate", 23, RoamingThreatContentProfile.FullPrototype, RoamingThreatFaction.Demons, "lesserdemon", "cinderling"),

            // Chapter VI keeps five readable threats but shifts the balance fully
            // toward the ritual factions already supported by the late-game pool.
            Threat(0, "crown-road-drow-host", "Crown Road Drow Host", "drow", 6, 6, "glass-warrens", 10, RoamingThreatContentProfile.FullPrototype, RoamingThreatFaction.Drow, "drowblade", "drowcrossbow", "drowmage", "drowpriest"),
            Threat(1, "gloam-revenant-watch", "Gloam Revenant Watch", "revenant", 6, 6, "gloam-courts", 14, RoamingThreatContentProfile.FullPrototype, RoamingThreatFaction.Undead, "reaver", "husk", "shade", "bonepriest"),
            Threat(2, "fen-bone-conclave", "Fen Bone Conclave", "bonepriest", 6, 6, "ash-fen", 17, RoamingThreatContentProfile.FullPrototype, RoamingThreatFaction.Undead, "bonepriest", "husk", "shade", "reaver"),
            Threat(3, "red-mile-demon-pack", "Red Mile Demon Pack", "lesserdemon", 6, 6, "red-gate", 20, RoamingThreatContentProfile.FullPrototype, RoamingThreatFaction.Demons, "lesserdemon", "cinderling"),
            Threat(4, "crown-road-demon-guard", "Crown Road Demon Guard", "redgatedemon", 6, 6, "inner-ash-road", 23, RoamingThreatContentProfile.FullPrototype, RoamingThreatFaction.Demons, "lesserdemon", "cinderling")
        };

        public static IReadOnlyList<RoamingThreatDefinition> All => definitions;

        public static IReadOnlyList<RoamingThreatDefinition> ForDepth(int depth, bool fullPrototype)
        {
            return definitions
                .Where(definition => definition.AppliesTo(Math.Max(1, depth), fullPrototype))
                .OrderBy(definition => definition.Slot)
                .ToArray();
        }

        public static RoamingThreatDefinition Find(string id, int depth, bool fullPrototype)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return definitions.FirstOrDefault(definition =>
                string.Equals(definition.Id, id, StringComparison.Ordinal)
                && definition.AppliesTo(Math.Max(1, depth), fullPrototype));
        }

        public static EncounterDefinition BuildEncounter(RoamingThreatDefinition definition, int depth)
        {
            if (definition == null || definition.EnemyIds.Count == 0) return null;
            EncounterDefinition patrol = EncounterCatalog.For(EncounterId.Patrol);
            return new EncounterDefinition
            {
                Id = EncounterId.Patrol,
                LegacyStyle = patrol.LegacyStyle,
                Banner = definition.Name,
                Intro = definition.Name + " closes the road. Steel answers in the dark.",
                EnemyIds = definition.EnemyIds.ToArray(),
                FixedEnemyCount = patrol.EnemyCountForDepth(Math.Max(1, depth)),
                RandomObstacleCount = patrol.RandomObstacleCount
            };
        }

        public static RoamingThreatFaction FactionForArchetype(string archetype)
        {
            switch ((archetype ?? "").Trim().ToLowerInvariant())
            {
                case "rats":
                case "rat":
                case "ratfolk":
                case "ratbrute":
                case "ratcleric":
                case "plaguerats":
                case "ratswarm":
                case "ratcaptain": return RoamingThreatFaction.Rats;
                case "kobold":
                case "kobolds":
                case "koboldshaman":
                case "koboldking": return RoamingThreatFaction.Kobolds;
                case "drowscout":
                case "drowmage":
                case "drow": return RoamingThreatFaction.Drow;
                case "reaver":
                case "undead":
                case "bonepriest":
                case "gloamknight":
                case "revenant": return RoamingThreatFaction.Undead;
                case "imp":
                case "boundimp":
                case "lesserdemon":
                case "greaterdemon":
                case "demon":
                case "redgatedemon": return RoamingThreatFaction.Demons;
                default: throw new ArgumentException("Unknown roaming-threat archetype: " + (archetype ?? ""), nameof(archetype));
            }
        }

        public static RoamingThreatFaction FactionForEnemy(string enemyId)
        {
            switch ((enemyId ?? "").Trim().ToLowerInvariant())
            {
                case "sewerrat":
                case "giantrat":
                case "ratfolk":
                case "ratcutthroat":
                case "ratmage":
                case "ratcleric":
                case "ratbrute": return RoamingThreatFaction.Rats;
                case "koboldraider":
                case "koboldslinger":
                case "koboldshaman":
                case "koboldwizard":
                case "koboldshield":
                case "koboldking": return RoamingThreatFaction.Kobolds;
                case "drowscout":
                case "drowblade":
                case "drowcrossbow":
                case "drowmage":
                case "drowpriest":
                case "glassmage": return RoamingThreatFaction.Drow;
                case "husk":
                case "reaver":
                case "shade":
                case "bonepriest":
                case "gloamknight":
                    return RoamingThreatFaction.Undead;
                case "cinderling":
                case "lesserdemon": return RoamingThreatFaction.Demons;
                default: throw new ArgumentException("Enemy is not assigned to a roaming-threat faction: " + (enemyId ?? ""), nameof(enemyId));
            }
        }

        private static RoamingThreatDefinition Threat(
            int slot,
            string id,
            string name,
            string archetype,
            int minDepth,
            int maxDepth,
            string preferredZoneId,
            int targetDistance,
            RoamingThreatFaction faction,
            params string[] enemyIds)
        {
            return Threat(
                slot,
                id,
                name,
                archetype,
                minDepth,
                maxDepth,
                preferredZoneId,
                targetDistance,
                RoamingThreatContentProfile.Any,
                faction,
                enemyIds);
        }

        private static RoamingThreatDefinition Threat(
            int slot,
            string id,
            string name,
            string archetype,
            int minDepth,
            int maxDepth,
            string preferredZoneId,
            int targetDistance,
            RoamingThreatContentProfile contentProfile,
            RoamingThreatFaction faction,
            params string[] enemyIds)
        {
            return new RoamingThreatDefinition(
                slot,
                id,
                name,
                archetype,
                minDepth,
                maxDepth,
                preferredZoneId,
                targetDistance,
                contentProfile,
                faction,
                enemyIds);
        }
    }
}
