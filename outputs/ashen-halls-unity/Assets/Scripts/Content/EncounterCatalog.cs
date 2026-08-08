using System;
using System.Collections.Generic;
using System.Linq;

namespace AshenHalls
{
    public static class EncounterCatalog
    {
        public const string LegacySentryEnemyId = "sentry";

        private static readonly EncounterDefinition[] definitions =
        {
            new EncounterDefinition
            {
                Id = EncounterId.RandomEncounter,
                LegacyStyle = "",
                Banner = "Encounter",
                Intro = "Steel answers in the dark.",
                UsesGeneratedEnemyPool = true,
                GeneratedCountBonus = 1,
                RandomObstacleCount = 7
            },
            new EncounterDefinition
            {
                Id = EncounterId.Patrol,
                LegacyStyle = "patrol",
                Banner = "Encounter",
                Intro = "Steel answers in the dark.",
                UsesGeneratedEnemyPool = true,
                GeneratedCountBonus = 0,
                RandomObstacleCount = 7
            },
            new EncounterDefinition
            {
                Id = EncounterId.Guard,
                LegacyStyle = "guard",
                Banner = "Encounter",
                Intro = "Steel answers in the dark.",
                UsesGeneratedEnemyPool = true,
                GeneratedCountBonus = 1,
                RandomObstacleCount = 7
            },
            new EncounterDefinition
            {
                Id = EncounterId.FinalGate,
                LegacyStyle = "boss",
                Banner = "Final Gate",
                Intro = "The final gate breaks open. Meteor fire gathers above the hall.",
                EnemyIds = new[] { "meteorlich", "ritualheart", "drowpriest", "koboldwizard", "lesserdemon" }
            },
            new EncounterDefinition
            {
                Id = EncounterId.BetaLab,
                LegacyStyle = "lab",
                Banner = VersionInfo.BuildStage,
                Intro = "Beta combat lab opens: enemy casters are ready.",
                EnemyIds = new[] { "koboldshaman", "koboldwizard", "koboldshield", "koboldslinger", "bonepriest", "glassmage", "ratmage", "ratcleric", "drowmage", "drowpriest", "cinderling", "lesserdemon" },
                FixedEnemyCount = 7,
                RandomObstacleCount = 3,
                DevelopmentOnly = true,
                Obstacles = new[]
                {
                    new Point(5, 1, "tree"),
                    new Point(6, 2, "stone"),
                    new Point(7, 3, "web", 9),
                    new Point(8, 4, "gas", 9),
                    new Point(6, 5, "fire", 7),
                    // Keep the authored ice tile visible instead of placing it beneath
                    // the sixth default enemy at (9, 5).
                    new Point(8, 6, "ice", 9)
                }
            },
            new EncounterDefinition
            {
                Id = EncounterId.MartialLab,
                LegacyStyle = "martiallab",
                Banner = "Martial Lab",
                Intro = "Martial lab opens: melee targets crowd the line for skill testing.",
                EnemyIds = new[] { "koboldshield", "ratbrute", "koboldraider", "drowblade", "ratcutthroat", "reaver" },
                FixedEnemyCount = 6,
                DevelopmentOnly = true,
                BoostMartialLabParty = true,
                WoundFirstEnemy = true,
                PartyPlacements = new[]
                {
                    new Point(2, 3),
                    new Point(1, 4),
                    new Point(1, 1),
                    new Point(1, 6)
                },
                EnemyPlacements = new[]
                {
                    new Point(3, 3),
                    new Point(2, 2),
                    new Point(2, 4),
                    new Point(5, 3),
                    new Point(6, 5),
                    new Point(8, 2)
                },
                Obstacles = new[]
                {
                    new Point(5, 1, "tree", 8),
                    new Point(6, 2, "stone"),
                    new Point(7, 4, "web", 8)
                }
            },
            new EncounterDefinition
            {
                Id = EncounterId.KoboldKing,
                LegacyStyle = "koboldking",
                Banner = "Kobold King",
                Intro = "The Kobold King raises a stolen black blade. Shield charms, fire, and ice answer his bark.",
                EnemyIds = new[] { "koboldking", "koboldwizard", "koboldshaman", "koboldshield", "koboldslinger", "koboldraider" },
                NormalizeKoboldKing = true,
                EnemyPlacements = new[]
                {
                    new Point(10, 3),
                    new Point(9, 1),
                    new Point(9, 6),
                    new Point(8, 2),
                    new Point(10, 5),
                    new Point(8, 4)
                },
                Obstacles = new[]
                {
                    new Point(5, 1, "stone"),
                    new Point(6, 2, "stone"),
                    new Point(6, 5, "stone"),
                    new Point(5, 6, "stone"),
                    new Point(7, 3, "tree", 8),
                    new Point(7, 4, "tree", 8),
                    new Point(4, 3, "glyph", 6),
                    new Point(8, 5, "demonrift", 5)
                }
            },
            new EncounterDefinition
            {
                Id = EncounterId.KoboldCave,
                LegacyStyle = "koboldcave",
                Banner = "Smoke Cave",
                Intro = "The smoke cave tightens into shield work, slingers, and bone-sign magic.",
                EnemyIds = new[] { "koboldshield", "koboldshaman", "koboldslinger", "koboldraider", "koboldwizard" },
                FixedEnemyCount = 5,
                Obstacles = new[]
                {
                    new Point(4, 1, "stone"),
                    new Point(5, 3, "tree", 8),
                    new Point(6, 4, "web", 8),
                    new Point(7, 2, "stone"),
                    new Point(8, 6, "tree", 8),
                    new Point(7, 5, "glyph", 6)
                }
            },
            new EncounterDefinition
            {
                Id = EncounterId.KoboldAmbush,
                LegacyStyle = "koboldambush",
                Banner = "Kobold Ambush",
                Intro = "Kobolds hit from behind broken stalls, testing the line before the cave drums answer.",
                EnemyIds = new[] { "koboldraider", "koboldslinger", "koboldshield", "koboldshaman" },
                FixedEnemyCount = 4,
                Obstacles = new[]
                {
                    new Point(5, 1, "tree", 8),
                    new Point(6, 5, "tree", 8),
                    new Point(8, 3, "stone")
                }
            },
            new EncounterDefinition
            {
                Id = EncounterId.MidgaardSewer,
                LegacyStyle = "ratsewer",
                Banner = "Midgaard Sewer",
                Intro = "The Midgaard sewer churns with rats. Bring back enough pelts for the armorer.",
                EnemyIds = new[] { "sewerrat", "sewerrat", "giantrat", "ratfolk", "ratcutthroat" },
                FixedEnemyCount = 5,
                Obstacles = new[]
                {
                    new Point(5, 2, "stone"),
                    new Point(6, 4, "gas", 7),
                    new Point(7, 5, "web", 7)
                }
            },
            new EncounterDefinition
            {
                Id = EncounterId.BoneRoadWatch,
                LegacyStyle = "bone-road-watch",
                Banner = "Bone Road Watch",
                Intro = "A drow watch steps from the old mile stones while their dead auxiliaries close the road behind you.",
                EnemyIds = new[] { "drowscout", "drowcrossbow", "husk", "reaver" },
                FixedEnemyCount = 4,
                EnemyPlacements = new[]
                {
                    new Point(9, 2),
                    new Point(10, 5),
                    new Point(8, 4),
                    new Point(9, 6)
                },
                Obstacles = new[]
                {
                    new Point(5, 1, "stone"),
                    new Point(5, 6, "stone"),
                    new Point(6, 3, "tree", 8),
                    new Point(7, 5, "tree", 8)
                }
            },
            new EncounterDefinition
            {
                Id = EncounterId.GloamCryptRitual,
                LegacyStyle = "gloam-crypt-ritual",
                Banner = "Gloam Crypt Ritual",
                Intro = "A bone priest feeds the Gloam dead into a black rite. Break the circle before the crypt answers.",
                EnemyIds = new[] { "bonepriest", "shade", "husk", "reaver", "drowmage" },
                FixedEnemyCount = 5,
                EnemyPlacements = new[]
                {
                    new Point(10, 3),
                    new Point(9, 1),
                    new Point(8, 5),
                    new Point(10, 6),
                    new Point(9, 4)
                },
                Obstacles = new[]
                {
                    new Point(5, 2, "stone"),
                    new Point(5, 5, "stone"),
                    new Point(7, 2, "glyph", 7),
                    new Point(7, 4, "gas", 7),
                    new Point(6, 6, "web", 7)
                }
            },
            new EncounterDefinition
            {
                Id = EncounterId.GloamWarden,
                LegacyStyle = "gloam-warden-boss",
                Banner = "Warden of Gloam",
                Intro = "The Gloam Warden seals the Red Gate warning behind a wall of oath-bound dead and drow steel.",
                EnemyIds = new[] { "gloamknight", "bonepriest", "drowpriest", "reaver", "shade", "drowcrossbow" },
                FixedEnemyCount = 6,
                EnemyPlacements = new[]
                {
                    new Point(10, 3),
                    new Point(9, 1),
                    new Point(9, 6),
                    new Point(8, 2),
                    new Point(8, 5),
                    new Point(10, 5)
                },
                Obstacles = new[]
                {
                    new Point(5, 1, "stone"),
                    new Point(5, 6, "stone"),
                    new Point(6, 2, "glyph", 8),
                    new Point(6, 5, "glyph", 8),
                    new Point(7, 3, "demonrift", 6),
                    new Point(7, 4, "gas", 7)
                }
            }
        };

        public static IReadOnlyList<EncounterDefinition> All => definitions;

        public static EncounterDefinition For(EncounterId id)
        {
            EncounterDefinition definition = definitions.FirstOrDefault(d => d.Id == id);
            if (definition == null) throw new ArgumentException("Unknown encounter id: " + id, nameof(id));
            return definition;
        }

        public static EncounterId IdForLegacyStyle(string style)
        {
            style = style ?? "";
            EncounterDefinition definition = definitions.FirstOrDefault(d => string.Equals(d.LegacyStyle ?? "", style, StringComparison.OrdinalIgnoreCase));
            if (definition == null) throw new ArgumentException("Unknown encounter style: " + style, nameof(style));
            return definition.Id;
        }

        public static bool IsKnownEnemyId(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            if (string.Equals(id, LegacySentryEnemyId, StringComparison.OrdinalIgnoreCase)) return true;
            return EnemyCatalog.Ids.Any(existing => string.Equals(existing, id, StringComparison.OrdinalIgnoreCase));
        }
    }
}
