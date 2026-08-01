using System;
using System.Collections.Generic;
using System.Linq;

namespace AshenHalls
{
    public static class ContentSetCatalog
    {
        public const string SewerSlice = "sewer-slice";
        public const string FullPrototype = "full-prototype";

        private static readonly string[] sewerSliceFormulaCodes = { "OIC", "TBQ", "GBH", "NVC", "FIF", "WBI", "RIG", "RSG", "CLT", "VST", "AST", "FBL", "SRF", "RKW", "IBD", "RBT" };
        private static readonly string[] sewerSliceAbilityIds = { "charge", "execute", "shieldbash", "rally", "stealth", "ambush", "smokebomb", "aimedshot", "pinningshot", "scoutmark", "riftpounce", "abyssalwhirl", "soulrend", "dreadroar" };
        private static readonly string[] sewerSliceEnemyIds = { "sewerrat", "giantrat", "ratfolk", "ratcutthroat", "ratmage", "ratbrute" };

        private static readonly EncounterDefinition[] sewerSliceEncounters =
        {
            new EncounterDefinition
            {
                Id = EncounterId.MidgaardSewer,
                LegacyStyle = "sewer_broken_sluice",
                Banner = "Broken Sluice",
                Intro = "The first sluice chamber is all teeth and bad footing. Keep formation and clear the rats.",
                EnemyIds = new[] { "sewerrat", "sewerrat", "giantrat" },
                PartyPlacements = new[] { new Point(1, 2), new Point(1, 4), new Point(1, 1), new Point(1, 6) },
                EnemyPlacements = new[] { new Point(8, 2), new Point(9, 4), new Point(10, 5) },
                Obstacles = new[] { new Point(5, 3, "stone") }
            },
            new EncounterDefinition
            {
                Id = EncounterId.MidgaardSewer,
                LegacyStyle = "sewer_foul_runoff",
                Banner = "Foul Runoff",
                Intro = "Runoff fumes crawl over the bricks. Use warding, healing, and cover before the ratfolk close.",
                EnemyIds = new[] { "giantrat", "ratfolk", "ratcutthroat", "ratmage" },
                PartyPlacements = new[] { new Point(1, 2), new Point(1, 4), new Point(1, 1), new Point(1, 6) },
                EnemyPlacements = new[] { new Point(8, 1), new Point(9, 3), new Point(9, 5), new Point(10, 2) },
                Obstacles = new[] { new Point(5, 2, "stone"), new Point(6, 3, "gas", 6), new Point(7, 4, "web", 6) }
            },
            new EncounterDefinition
            {
                Id = EncounterId.MidgaardSewer,
                LegacyStyle = "sewer_cistern_den",
                Banner = "Cistern Den",
                Intro = "The den boss bellows behind a plague mage. Break the spell line, then finish the brute.",
                EnemyIds = new[] { "ratbrute", "ratmage", "ratcutthroat", "ratfolk", "giantrat" },
                PartyPlacements = new[] { new Point(1, 2), new Point(1, 4), new Point(1, 1), new Point(1, 6) },
                EnemyPlacements = new[] { new Point(9, 4), new Point(10, 2), new Point(8, 1), new Point(8, 6), new Point(10, 5) },
                Obstacles = new[] { new Point(5, 2, "stone"), new Point(5, 5, "stone"), new Point(7, 3, "gas", 7), new Point(7, 4, "web", 7) }
            }
        };

        public static IReadOnlyList<string> SewerSliceFormulaCodes => sewerSliceFormulaCodes;
        public static IReadOnlyList<string> SewerSliceAbilityIds => sewerSliceAbilityIds;
        public static IReadOnlyList<string> SewerSliceEnemyIds => sewerSliceEnemyIds;
        public static IReadOnlyList<EncounterDefinition> SewerSliceEncounters => sewerSliceEncounters;
        public static int SewerSliceRequiredProofCount => sewerSliceEncounters.Length;

        public static bool IsSewerSlice(string contentSet)
        {
            return string.Equals(contentSet, SewerSlice, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsFullPrototype(string contentSet)
        {
            return string.Equals(contentSet, FullPrototype, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ShowPrototypeScaffold(string contentSet)
        {
            return IsFullPrototype(contentSet);
        }

        public static bool AllowPrototypeRouteTriggers(string contentSet, IReadOnlyCollection<string> flags)
        {
            return IsFullPrototype(contentSet);
        }

        public static bool AllowKoboldChapter(string contentSet, IReadOnlyCollection<string> flags)
        {
            if (IsFullPrototype(contentSet)) return true;
            return IsSewerSlice(contentSet)
                && flags != null
                && flags.Contains(StoryFlags.OldRoadTeaserUnlocked);
        }

        public static bool IsKnown(string contentSet)
        {
            return string.Equals(contentSet, SewerSlice, StringComparison.OrdinalIgnoreCase)
                || string.Equals(contentSet, FullPrototype, StringComparison.OrdinalIgnoreCase);
        }

        public static string NormalizeContentSetId(string contentSet)
        {
            if (string.Equals(contentSet, FullPrototype, StringComparison.OrdinalIgnoreCase)) return FullPrototype;
            if (string.Equals(contentSet, SewerSlice, StringComparison.OrdinalIgnoreCase)) return SewerSlice;
            return SewerSlice;
        }

        public static string RepairLoadedContentSetId(GameState loaded, out bool repaired, out string note)
        {
            repaired = false;
            note = "";
            if (loaded == null)
            {
                repaired = true;
                note = "Missing save state; using sewer slice rules.";
                return SewerSlice;
            }

            if (loaded.SaveVersion == 17)
            {
                repaired = true;
                note = "Older v17 save migrated to full prototype rules.";
                return FullPrototype;
            }

            if (IsKnown(loaded.ContentSetId))
            {
                return NormalizeContentSetId(loaded.ContentSetId);
            }

            repaired = true;

            note = $"Unknown content set '{loaded.ContentSetId ?? "(blank)"}' repaired to sewer slice rules.";
            return SewerSlice;
        }

        public static bool FormulaActive(string contentSet, string code)
        {
            return IsFullPrototype(contentSet) || sewerSliceFormulaCodes.Any(id => string.Equals(id, code, StringComparison.OrdinalIgnoreCase));
        }

        public static bool AbilityActive(string contentSet, string id)
        {
            return IsFullPrototype(contentSet) || sewerSliceAbilityIds.Any(active => string.Equals(active, id, StringComparison.OrdinalIgnoreCase));
        }

        public static bool EnemyActive(string contentSet, string id)
        {
            return IsFullPrototype(contentSet) || sewerSliceEnemyIds.Any(active => string.Equals(active, id, StringComparison.OrdinalIgnoreCase));
        }

        public static EncounterDefinition SewerSliceEncounterForProgress(int clearedCount)
        {
            int index = Math.Max(0, Math.Min(clearedCount, sewerSliceEncounters.Length - 1));
            return sewerSliceEncounters[index];
        }

        public static void MarkSewerSliceContractAccepted(ICollection<string> flags)
        {
            AddFlag(flags, StoryFlags.MidgaardRatQuestGiven);
            AddFlag(flags, StoryFlags.SewerContractAccepted);
        }

        public static void MarkSewerSliceEncounterCleared(ICollection<string> flags, string style)
        {
            AddFlag(flags, ClearedFlagForEncounterStyle(style));
            if (SewerSliceClearedCount(flags as IReadOnlyCollection<string> ?? flags?.ToList()) >= sewerSliceEncounters.Length)
            {
                AddFlag(flags, StoryFlags.MidgaardRatPeltsCollected);
            }
        }

        public static bool SewerSliceRewardReady(IReadOnlyCollection<string> flags)
        {
            return SewerSliceClearedCount(flags) >= sewerSliceEncounters.Length && flags != null && !flags.Contains(StoryFlags.SewerRewardClaimed);
        }

        public static bool SewerSliceRewardReady(IReadOnlyCollection<string> flags, IEnumerable<InventoryItem> inventory)
        {
            return SewerSliceRewardReady(flags) && CountSewerSliceProof(inventory) >= SewerSliceRequiredProofCount;
        }

        public static bool SewerSliceComplete(IReadOnlyCollection<string> flags)
        {
            return flags != null && flags.Contains(StoryFlags.SewerRewardClaimed) && flags.Contains(StoryFlags.OldRoadTeaserUnlocked);
        }

        public static void MarkSewerSliceRewardClaimed(ICollection<string> flags)
        {
            AddFlag(flags, StoryFlags.SewerRewardClaimed);
            AddFlag(flags, StoryFlags.OldRoadTeaserUnlocked);
        }

        public static bool TryClaimSewerSliceReward(ICollection<string> flags, IList<InventoryItem> inventory, out InventoryItem reward, out string note)
        {
            reward = null;
            note = "";
            if (flags == null)
            {
                note = "No sewer contract state is available.";
                return false;
            }
            if (flags.Contains(StoryFlags.SewerRewardClaimed))
            {
                note = "The sewer reward has already been claimed.";
                return false;
            }

            int cleared = SewerSliceClearedCount(flags as IReadOnlyCollection<string> ?? flags.ToList());
            if (cleared < sewerSliceEncounters.Length)
            {
                note = $"Clear all three sewer rooms first. Rooms cleared: {cleared}/{sewerSliceEncounters.Length}.";
                return false;
            }
            if (inventory == null)
            {
                note = "No pack is available for the sewer proof.";
                return false;
            }

            int proof = CountSewerSliceProof(inventory);
            if (proof < SewerSliceRequiredProofCount)
            {
                note = $"Bring {SewerSliceRequiredProofCount} sewer proof bundles to the armorer. Current proof: {proof}/{SewerSliceRequiredProofCount}.";
                return false;
            }

            RemoveSewerSliceProof(inventory, SewerSliceRequiredProofCount);
            reward = CreateSewerSliceReward();
            MarkSewerSliceRewardClaimed(flags);
            note = "The armorer accepts the sewer proof and prepares the first reward.";
            return true;
        }

        public static InventoryItem CreateSewerSliceProof()
        {
            return new InventoryItem
            {
                Mark = "sewer",
                Material = "rat pelt",
                Form = "pelt bundle",
                Trait = "sewer-proof",
                Slot = "quest",
                Rarity = "quest",
                DisplayName = "rat pelt"
            };
        }

        public static InventoryItem CreateSewerSliceReward()
        {
            return new InventoryItem
            {
                Mark = "stitched",
                Material = "rat pelt",
                Form = "rat pelt armor",
                Trait = "nimble",
                Slot = "armor",
                Bonus = 3,
                AgilityBonus = 1,
                HealthBonus = 1,
                Rarity = "quest",
                DisplayName = "+3 stitched rat pelt armor"
            };
        }

        public static InventoryItem CreateSewerSafeRoomBlade()
        {
            return new InventoryItem
            {
                Mark = "sluicekeeper",
                Material = "fine steel",
                Form = "broadsword",
                Trait = "guarding",
                Slot = "weapon",
                Bonus = 2,
                StrengthBonus = 1,
                DamageMin = 4,
                DamageMax = 7,
                AttackSpeed = 3,
                Rarity = "quest",
                DamageType = "physical",
                DisplayName = "+2 sluicekeeper fine steel broadsword"
            };
        }

        public static InventoryItem CreateSewerSafeRoomFocus()
        {
            return new InventoryItem
            {
                Mark = "etched",
                Material = "stormglass",
                Form = "ritual staff",
                Trait = "storm",
                Slot = "weapon",
                Bonus = 2,
                IntelligenceBonus = 1,
                DamageMin = 3,
                DamageMax = 6,
                AttackSpeed = 3,
                Rarity = "quest",
                DamageType = "shock",
                DisplayName = "+2 etched stormglass ritual staff"
            };
        }

        public static bool HasSewerSafeRoomChoice(IReadOnlyCollection<string> flags)
        {
            return flags != null && flags.Contains(StoryFlags.SewerSafeRoomChoiceClaimed);
        }

        public static void MarkSewerSafeRoomChoice(ICollection<string> flags, string choice)
        {
            if (flags == null || HasSewerSafeRoomChoice(flags as IReadOnlyCollection<string> ?? flags.ToList())) return;
            AddFlag(flags, StoryFlags.SewerSafeRoomChoiceClaimed);
            AddFlag(
                flags,
                string.Equals(choice, "focus", StringComparison.OrdinalIgnoreCase)
                    ? StoryFlags.SewerSafeRoomFocusChosen
                    : StoryFlags.SewerSafeRoomBladeChosen);
        }

        public static bool IsSewerSliceProof(InventoryItem item)
        {
            return item != null
                && string.Equals(item.Slot, "quest", StringComparison.OrdinalIgnoreCase)
                && string.Equals(item.Trait, "sewer-proof", StringComparison.OrdinalIgnoreCase)
                && string.Equals(item.Material, "rat pelt", StringComparison.OrdinalIgnoreCase);
        }

        public static int CountSewerSliceProof(IEnumerable<InventoryItem> inventory)
        {
            return inventory == null ? 0 : inventory.Count(IsSewerSliceProof);
        }

        public static void RemoveSewerSliceProof(IList<InventoryItem> inventory, int count)
        {
            if (inventory == null || count <= 0) return;
            for (int i = inventory.Count - 1; i >= 0 && count > 0; i--)
            {
                if (!IsSewerSliceProof(inventory[i])) continue;
                inventory.RemoveAt(i);
                count--;
            }
        }

        public static int SewerSliceClearedCount(IReadOnlyCollection<string> flags)
        {
            if (flags == null) return 0;
            if (flags.Contains(StoryFlags.SewerCisternDenCleared)) return 3;
            if (flags.Contains(StoryFlags.SewerFoulRunoffCleared)) return 2;
            if (flags.Contains(StoryFlags.SewerBrokenSluiceCleared)) return 1;
            return 0;
        }

        public static bool IsSewerSliceEncounterStyle(string style)
        {
            return sewerSliceEncounters.Any(encounter => string.Equals(encounter.LegacyStyle, style, StringComparison.OrdinalIgnoreCase));
        }

        public static string ClearedFlagForEncounterStyle(string style)
        {
            if (string.Equals(style, "sewer_broken_sluice", StringComparison.OrdinalIgnoreCase)) return StoryFlags.SewerBrokenSluiceCleared;
            if (string.Equals(style, "sewer_foul_runoff", StringComparison.OrdinalIgnoreCase)) return StoryFlags.SewerFoulRunoffCleared;
            if (string.Equals(style, "sewer_cistern_den", StringComparison.OrdinalIgnoreCase)) return StoryFlags.SewerCisternDenCleared;
            return "";
        }

        private static void AddFlag(ICollection<string> flags, string flag)
        {
            if (flags == null || string.IsNullOrEmpty(flag) || flags.Contains(flag)) return;
            flags.Add(flag);
        }
    }
}
