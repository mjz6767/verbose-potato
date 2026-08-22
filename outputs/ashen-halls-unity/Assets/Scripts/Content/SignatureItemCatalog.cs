using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public sealed class SignatureItemDefinition
    {
        private readonly IReadOnlyList<string> legacyDisplayNames;

        public string Id { get; }
        public int IconIndex { get; }
        public string DisplayName { get; }
        public string Lore { get; }
        public string IntrinsicName { get; }
        public string IntrinsicSummary { get; }
        public string Mark { get; }
        public string Material { get; }
        public string Form { get; }
        public string Trait { get; }
        public string Slot { get; }
        public int Bonus { get; }
        public int StrengthBonus { get; }
        public int IntelligenceBonus { get; }
        public int AgilityBonus { get; }
        public int HealthBonus { get; }
        public int DamageMin { get; }
        public int DamageMax { get; }
        public int AttackSpeed { get; }
        public string Rarity { get; }
        public string DamageType { get; }
        public IReadOnlyList<string> LegacyDisplayNames => legacyDisplayNames;

        public SignatureItemDefinition(
            string id,
            int iconIndex,
            string displayName,
            string lore,
            string intrinsicName,
            string intrinsicSummary,
            string mark,
            string material,
            string form,
            string trait,
            string slot,
            int bonus,
            int strengthBonus,
            int intelligenceBonus,
            int agilityBonus,
            int healthBonus,
            int damageMin,
            int damageMax,
            int attackSpeed,
            string rarity,
            string damageType,
            params string[] legacyDisplayNames)
        {
            Id = id ?? "";
            IconIndex = iconIndex;
            DisplayName = displayName ?? "";
            Lore = lore ?? "";
            IntrinsicName = intrinsicName ?? "";
            IntrinsicSummary = intrinsicSummary ?? "";
            Mark = mark ?? "";
            Material = material ?? "";
            Form = form ?? "";
            Trait = trait ?? "";
            Slot = slot ?? "";
            Bonus = bonus;
            StrengthBonus = strengthBonus;
            IntelligenceBonus = intelligenceBonus;
            AgilityBonus = agilityBonus;
            HealthBonus = healthBonus;
            DamageMin = damageMin;
            DamageMax = damageMax;
            AttackSpeed = attackSpeed;
            Rarity = rarity ?? "";
            DamageType = damageType ?? "";
            this.legacyDisplayNames = Array.AsReadOnly(legacyDisplayNames ?? Array.Empty<string>());
        }

        public InventoryItem Create()
        {
            return new InventoryItem
            {
                SignatureId = Id,
                Mark = Mark,
                Material = Material,
                Form = Form,
                Trait = Trait,
                Slot = Slot,
                Bonus = Bonus,
                StrengthBonus = StrengthBonus,
                IntelligenceBonus = IntelligenceBonus,
                AgilityBonus = AgilityBonus,
                HealthBonus = HealthBonus,
                DamageMin = DamageMin,
                DamageMax = DamageMax,
                AttackSpeed = AttackSpeed,
                Rarity = Rarity,
                DamageType = DamageType,
                DisplayName = DisplayName
            };
        }

        internal bool MatchesStructure(InventoryItem item)
        {
            return item != null
                && Same(item.Mark, Mark)
                && Same(item.Material, Material)
                && Same(item.Form, Form)
                && Same(item.Trait, Trait);
        }

        internal bool MatchesKnownName(string displayName)
        {
            if (MatchesKnownNameExact(displayName)) return true;
            if (HasKnownNameSuffix(displayName, DisplayName)) return true;
            foreach (string legacyName in legacyDisplayNames)
            {
                if (HasKnownNameSuffix(displayName, legacyName)) return true;
            }
            return false;
        }

        internal bool MatchesKnownNameExact(string displayName)
        {
            if (Same(displayName, DisplayName)) return true;
            foreach (string legacyName in legacyDisplayNames)
            {
                if (Same(displayName, legacyName)) return true;
            }
            return false;
        }

        private static bool HasKnownNameSuffix(string displayName, string knownName)
        {
            string normalizedDisplayName = (displayName ?? "").Trim();
            string normalizedKnownName = (knownName ?? "").Trim();
            return normalizedDisplayName.Length > normalizedKnownName.Length
                && normalizedDisplayName.EndsWith(
                    " " + normalizedKnownName,
                    StringComparison.OrdinalIgnoreCase);
        }

        private static bool Same(string left, string right)
        {
            return string.Equals(
                (left ?? "").Trim(),
                (right ?? "").Trim(),
                StringComparison.OrdinalIgnoreCase);
        }
    }

    public static class SignatureItemCatalog
    {
        public const string UnfathomableSwordId = "unfathomable_sword";
        public const string SluicekeeperBladeId = "sluicekeeper_blade";
        public const string StormglassConductorId = "stormglass_conductor";
        public const string RatcatcherRoadcoatId = "ratcatcher_roadcoat";
        public const string GloamReliquaryMailId = "gloam_reliquary_mail";
        public const string MirrorweaveRoadMantleId = "mirrorweave_road_mantle";
        public const string CrownwardWarbladeId = "crownward_warblade";

        private static readonly IReadOnlyList<SignatureItemDefinition> Definitions =
            Array.AsReadOnly(new[]
            {
                new SignatureItemDefinition(
                    UnfathomableSwordId,
                    0,
                    "Sword of Unfathomable Darkness",
                    "A blackglass blade stolen from a fallen adventurer and reclaimed from Varkh's hoard.",
                    "Life Drinker",
                    "Gain +3 hit and +2 weapon power; successful hits restore 1-3 health based on damage dealt.",
                    "stolen", "blackglass", "broadsword", "unfathomable darkness", "weapon",
                    4, 1, 0, 1, 1, 5, 11, 8, "epic", "death"),
                new SignatureItemDefinition(
                    SluicekeeperBladeId,
                    1,
                    "+2 Sluicekeeper Blade",
                    "A steady fine-steel blade kept above the flood line for whoever must hold the sluice.",
                    "Sluicekeeper’s Brace",
                    "Gain +1 Guard only when taking the Guard action.",
                    "sluicekeeper", "fine steel", "broadsword", "guarding", "weapon",
                    2, 1, 0, 0, 0, 4, 7, 3, "quest", "physical",
                    "+2 sluicekeeper fine steel broadsword"),
                new SignatureItemDefinition(
                    StormglassConductorId,
                    2,
                    "+2 Stormglass Conductor",
                    "Etched stormglass carries the old sewer current without surrendering it to the dark.",
                    "Conduction",
                    "Basic weapon hits have a 30% base chance to stun before resistance.",
                    "etched", "stormglass", "ritual staff", "storm", "weapon",
                    2, 0, 1, 0, 0, 3, 6, 3, "quest", "shock",
                    "+2 etched stormglass ritual staff"),
                new SignatureItemDefinition(
                    RatcatcherRoadcoatId,
                    3,
                    "+3 Ratcatcher’s Roadcoat",
                    "Borin's close stitching turns clean sewer proof into a light coat made for the road beyond Midgaard.",
                    "Sewer-Step",
                    "Reduce poison damage taken by 1.",
                    "stitched", "rat pelt", "rat pelt armor", "nimble", "armor",
                    3, 0, 0, 1, 1, 0, 0, 0, "quest", "",
                    "+3 stitched rat pelt armor",
                    "+3 Ratcatcher's Roadcoat"),
                new SignatureItemDefinition(
                    GloamReliquaryMailId,
                    4,
                    "+4 Gloam Reliquary Mail",
                    "Consecrated scales from the Ossuary Warden's reliquary still carry the names they guarded.",
                    "Reliquary Ward",
                    "Reduce death or mind damage taken by 1.",
                    "gloamward", "reliquary scale", "scale mail", "warding", "armor",
                    4, 0, 1, 0, 2, 0, 0, 0, "quest", "",
                    "+4 gloamward reliquary scale mail"),
                new SignatureItemDefinition(
                    MirrorweaveRoadMantleId,
                    5,
                    "+5 Mirrorweave Road Mantle",
                    "Ashglass threads remember the one true road reflected by the recovered Mirror Index.",
                    "Mirrorweave",
                    "Reduce any nonphysical damage taken by 1.",
                    "ashglass", "mirrorweave", "road mantle", "warding", "armor",
                    5, 0, 2, 1, 1, 0, 0, 0, "quest", "",
                    "+5 ashglass mirrorweave road mantle"),
                new SignatureItemDefinition(
                    CrownwardWarbladeId,
                    6,
                    "+6 Crownward Emberglass Warblade",
                    "The Crownroad Marshal's emberglass command blade burns with the last order of the Red Gate.",
                    "Crownfire",
                    "A successful basic weapon hit removes one Ward turn from the target.",
                    "crownward", "emberglass", "broadsword", "warding", "weapon",
                    6, 2, 1, 0, 0, 8, 13, 3, "quest", "fire",
                    "+6 crownward emberglass warblade",
                    "+6 Crownward Warblade")
            });

        public static IReadOnlyList<SignatureItemDefinition> All => Definitions;

        public static SignatureItemDefinition Find(string id)
        {
            string normalized = (id ?? "").Trim();
            foreach (SignatureItemDefinition definition in Definitions)
            {
                if (string.Equals(definition.Id, normalized, StringComparison.OrdinalIgnoreCase))
                {
                    return definition;
                }
            }
            return null;
        }

        public static SignatureItemDefinition Find(InventoryItem item)
        {
            return Identify(item);
        }

        public static SignatureItemDefinition Identify(InventoryItem item)
        {
            if (item == null) return null;
            SignatureItemDefinition explicitIdentity = Find(item.SignatureId);
            if (explicitIdentity != null) return explicitIdentity;

            foreach (SignatureItemDefinition definition in Definitions)
            {
                if (definition.MatchesStructure(item)) return definition;
            }
            foreach (SignatureItemDefinition definition in Definitions)
            {
                if (definition.MatchesKnownName(item.DisplayName)) return definition;
            }
            return null;
        }

        public static SignatureItemDefinition Identify(string signatureIdOrDisplayName)
        {
            SignatureItemDefinition explicitIdentity = Find(signatureIdOrDisplayName);
            if (explicitIdentity != null) return explicitIdentity;

            foreach (SignatureItemDefinition definition in Definitions)
            {
                if (definition.MatchesKnownName(signatureIdOrDisplayName)) return definition;
            }
            return null;
        }

        public static int IconIndex(string signatureIdOrDisplayName)
        {
            SignatureItemDefinition definition = Identify(signatureIdOrDisplayName);
            return definition == null ? -1 : definition.IconIndex;
        }

        public static int IconIndex(InventoryItem item)
        {
            SignatureItemDefinition definition = Identify(item);
            return definition == null ? -1 : definition.IconIndex;
        }

        public static bool RepairIdentity(InventoryItem item)
        {
            SignatureItemDefinition definition = Identify(item);
            if (item == null || definition == null) return false;

            bool changed = !string.Equals(item.SignatureId, definition.Id, StringComparison.Ordinal);
            item.SignatureId = definition.Id;
            if (string.IsNullOrWhiteSpace(item.DisplayName) || definition.MatchesKnownNameExact(item.DisplayName))
            {
                if (!string.Equals(item.DisplayName, definition.DisplayName, StringComparison.Ordinal)) changed = true;
                item.DisplayName = definition.DisplayName;
            }
            if (item.EnchantmentBaseCaptured
                && definition.MatchesKnownNameExact(item.EnchantmentBaseDisplayName))
            {
                if (!string.Equals(
                    item.EnchantmentBaseDisplayName,
                    definition.DisplayName,
                    StringComparison.Ordinal))
                {
                    changed = true;
                }
                item.EnchantmentBaseDisplayName = definition.DisplayName;
            }
            return changed;
        }

        public static InventoryItem Create(string signatureId)
        {
            SignatureItemDefinition definition = Find(signatureId);
            return definition?.Create();
        }

        public static InventoryItem CreateUnfathomableSword() => Create(UnfathomableSwordId);
        public static InventoryItem CreateSluicekeeperBlade() => Create(SluicekeeperBladeId);
        public static InventoryItem CreateStormglassConductor() => Create(StormglassConductorId);
        public static InventoryItem CreateRatcatcherRoadcoat() => Create(RatcatcherRoadcoatId);
        public static InventoryItem CreateGloamReliquaryMail() => Create(GloamReliquaryMailId);
        public static InventoryItem CreateMirrorweaveRoadMantle() => Create(MirrorweaveRoadMantleId);
        public static InventoryItem CreateCrownwardWarblade() => Create(CrownwardWarbladeId);
    }
}
