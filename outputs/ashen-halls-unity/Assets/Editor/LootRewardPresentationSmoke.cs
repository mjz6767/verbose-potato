using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AshenHalls.Editor
{
    public static class LootRewardPresentationSmoke
    {
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " loot reward presentation smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " loot reward presentation smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            RewardLayoutsHaveDistinctNonOverlappingLanes();
            SummariesKeepStrategicWarningsAndExactValues();
            ComparisonHintsDiscloseOnlyOmittedChanges();
            StoredStatusRemovesOnlyRepeatedInstructions();
            RewardKindsRefreshWithoutStaleEquipment();
            DefaultSubmitUsesProductionDismissalWithoutMutatingRewards();
        }

        private static void RewardLayoutsHaveDistinctNonOverlappingLanes()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(960, 600), new Vector2Int(1280, 720),
                new Vector2Int(1600, 900), new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152), new Vector2Int(3440, 1440)
            };
            foreach (Vector2Int size in sizes)
            foreach (bool item in new[] { false, true })
            foreach (bool resources in new[] { false, true })
            {
                LootPopupGeometry g = LootPopupLayout.Calculate(size.x, size.y, item, resources);
                string context = $"{size.x}x{size.y}, item={item}, resources={resources}";
                Require(g.Fits(size.x, size.y), "all geometry remains on-screen: " + context);
                Require(g.Outcome.yMax < g.DismissButton.yMin, "stored status never overlaps actions: " + context);
                Require(g.DismissButton.width >= 144f && g.DismissButton.height >= 40f, "safe action retains usable target: " + context);
                if (!item)
                {
                    Require(g.Icon == Rect.zero && g.Body == Rect.zero && g.Comparison == Rect.zero,
                        "resource-only rewards have no empty equipment wells: " + context);
                    Require(g.Panel.height <= 244f, "resource-only receipt is compact: " + context);
                    Require(g.ItemTitle.yMax < g.ResourceRow.yMin && g.ResourceRow.yMax < g.Outcome.yMin,
                        "resource-only hierarchy does not overlap: " + context);
                    continue;
                }
                Require(g.Eyebrow.yMax < g.ItemTitle.yMin && g.ItemTitle.yMax < g.Body.yMin,
                    "rarity, item name and traits occupy separate lanes: " + context);
                Require(g.Icon.xMax < g.ItemTitle.xMin && g.Body.xMax < g.Comparison.xMin,
                    "item artwork, traits and comparison do not overlap: " + context);
                Require(g.Body.yMax < g.Outcome.yMin && g.Comparison.yMax < g.Outcome.yMin,
                    "item and comparison remain above saved status: " + context);
                Require(!resources || g.ResourceRow.yMax < g.Eyebrow.yMin,
                    "reward totals precede the item card: " + context);
                Require(g.QuickEquipButton.xMax < g.ReviewButton.xMin && g.ReviewButton.xMax < g.DismissButton.xMin,
                    "all three explicit actions remain separate: " + context);
            }
        }

        private static void SummariesKeepStrategicWarningsAndExactValues()
        {
            const string canonical = "Sword to bow / Lose life drain / 3-6 dmg vs 2-4 / BON 2 (+2) / SPD 9 (+1) / RNG 4 (+3) / ATTR 1 (+1)";
            string summary = LootRewardPresentationRules.ComparisonPreview(canonical);
            Require(summary.StartsWith("Sword to bow\nLose life drain\n3-6 dmg vs 2-4", StringComparison.Ordinal),
                "form and strategic losses stay above raw numbers");
            Require(summary.Contains("Bonus 2 (+2)") && summary.Contains("Speed 9 (+1)"),
                "concise stat labels preserve exact values and signs");
            Require(summary.Split('\n').Length == 5, "preview has a bounded readable line count");
            Require(canonical.Contains("RNG 4 (+3) / ATTR 1 (+1)"), "preview never mutates the complete comparison");
            Require(LootRewardPresentationRules.ComparisonPreview("ARM 2 (=) / AGI 1 (=)") == "Same core stats as current equipment.",
                "identical stats become one honest sentence");
            Require(LootRewardPresentationRules.ComparisonPreview("ARM 0 (-3) / AGI 1 (=)") == "Armor 0 (-3)",
                "negative and zero values are retained; equal stats do not add noise");
            Require(LootRewardPresentationRules.ComparisonPreview("4-7 dmg vs 2-4 / BON 2 (+2) / SPD 9 (+1) / RNG 1 (-3)")
                    == "4-7 dmg vs 2-4\nRange 1 (-3)\nBonus 2 (+2)\nSpeed 9 (+1)",
                "numerical sacrifices precede gains instead of falling off the end of a short preview");
            Require(LootRewardPresentationRules.ComparisonPreview(null) == "", "missing comparison is safe");
            string details = LootRewardPresentationRules.ItemDetails("Signature · Steady Hand\nTraits: range 4 / 3-6 dmg / spd 9 / quest", "Quest");
            Require(details == "Signature · Steady Hand\nrange 4  ·  3-6 dmg  ·  spd 9",
                "item detail removes only repeated label and rarity, preserving signature and stats");
            Require(LootRewardPresentationRules.ItemDetails("Details: proof / quest-bound", "quest") == "proof  ·  quest-bound",
                "rarity cleanup does not remove a partial trait match");
        }

        private static void ComparisonHintsDiscloseOnlyOmittedChanges()
        {
            const string five = "Sword to bow / Lose life drain / 3-6 dmg vs 2-4 / BON 2 (+2) / SPD 9 (+1)";
            Require(LootRewardPresentationRules.ComparisonHint(five, true) == "Full details in Inventory",
                "a complete five-change preview does not invent omitted changes");
            Require(LootRewardPresentationRules.ComparisonHint(five + " / RNG 4 (+3)", true) == "1 more change in Inventory",
                "one omitted change is disclosed with a singular label");
            string full = five + " / RNG 4 (+3) / ATTR 1 (+1) / ARM 2 (=) / AGI 1 (=) /   ";
            Require(LootRewardPresentationRules.ComparisonHint(full, true) == "2 more changes in Inventory",
                "only meaningful changes omitted by the preview are counted; equal and blank tokens do not inflate the hint");
            Require(LootRewardPresentationRules.ComparisonPreview(full).Split('\n').Length == 5,
                "disclosing overflow preserves the compact five-entry comparison");
            Require(LootRewardPresentationRules.ComparisonHint(null, true) == "Full details in Inventory"
                && LootRewardPresentationRules.ComparisonHint("ARM 2 (=) / AGI 1 (=)", true) == "Full details in Inventory",
                "absent or identical comparisons do not claim hidden changes");
            Require(LootRewardPresentationRules.ComparisonHint(full, false) == "",
                "a receipt with no review action does not advertise unavailable Inventory navigation");
            Require(LootRewardPresentationRules.ComparisonHint(full, true, "Authored equipment summary.") == "Full details in Inventory",
                "an independently authored summary has no assumed five-entry truncation contract");
            Require(LootRewardPresentationRules.ComparisonHint(full, true, LootRewardPresentationRules.ComparisonPreview(full))
                    == "2 more changes in Inventory",
                "a matching bounded summary discloses its exact remaining entries");
        }

        private static void StoredStatusRemovesOnlyRepeatedInstructions()
        {
            Require(LootRewardPresentationRules.StorageDetail("Added to inventory", "Added to inventory") == "",
                "stored outcome is shown once");
            Require(LootRewardPresentationRules.StorageDetail("Clear upgrade for Cairn — 4-7 dmg vs 2-4. Kept in the pack for your decision.", "Added to inventory") == "",
                "automatic recommendation is not duplicated beneath the comparison");
            Require(LootRewardPresentationRules.StorageDetail("Review for Vesh — Lose life drain. Kept in the pack.", "Added to inventory") == "",
                "review recommendation is not duplicated beneath the comparison");
            Require(LootRewardPresentationRules.StorageDetail("Edda's keepsake. Added to the pack for deliberate equipping.", "Added to inventory") == "Edda's keepsake.",
                "authored reward provenance survives removal of routine instructions");
            Require(LootRewardPresentationRules.StorageDetail("No adventurer can equip this item right now.", "Added to inventory") == "No adventurer can equip this item right now.",
                "action failures remain visible");
            Require(LootRewardPresentationRules.StorageDetail(null, null) == "", "missing status is safe");
        }

        private static void RewardKindsRefreshWithoutStaleEquipment()
        {
            GameObject host = new GameObject("Loot Reward Presentation Smoke");
            try
            {
                int equipped = 0, reviewed = 0, dismissed = 0;
                var view = new LootPopupView
                {
                    Visible = true, HasItem = true, CanQuickEquip = true, CanReview = true,
                    ItemName = "Roadwarden's Longsword", Rarity = "Rare", ItemType = "Sword",
                    TraitLine = "Traits: 4-7 dmg / spd 2 / rare", Outcome = "Added to inventory",
                    Comparison = "Best fit: Cairn · Upgrade · 4-7 dmg vs 2-4 / BON 2 (+2)",
                    ComparisonTitle = "Best fit: Cairn · Upgrade", ComparisonSummary = "4-7 dmg vs 2-4\nBonus 2 (+2)",
                    QuickEquipActionLabel = "Equip to Cairn", ReviewActionLabel = "Compare others",
                    Gold = int.MaxValue, Supplies = 1, Elixirs = 1
                };
                LootPopupScreen screen = host.AddComponent<LootPopupScreen>();
                screen.Bind(new LootPopupBindings
                {
                    View = () => view, QuickEquip = () => equipped++,
                    ReviewInventory = () => reviewed++, Dismiss = () => dismissed++
                });
                screen.Refresh();
                screen.SetVisible(true);
                Require(screen.HasItemCardForTest && screen.HasQuickEquipActionForTest && screen.HasReviewActionForTest,
                    "equipment receipt exposes existing deliberate actions and fallback icon");
                Require(screen.HasDefaultFocusForTest && screen.PrimaryActionLabelForTest == "Continue",
                    "already-collected rewards default to safe Continue, never an invented Take transaction");
                Require(screen.ItemDetailsForTest == "4-7 dmg  ·  spd 2" && screen.ComparisonPreviewForTest == view.ComparisonSummary,
                    "item stats and comparison are displayed separately");
                Require(screen.ComparisonHintForTest == "Full details in Inventory",
                    "a short comparison keeps the quiet full-details hint");
                const string detailedComparison = "Sword to bow / Lose life drain / 3-6 dmg vs 2-4 / BON 2 (+2) / SPD 9 (+1) / RNG 4 (+3) / ATTR 1 (+1)";
                view.Comparison = "Best fit: Cairn  •  Review  •  " + detailedComparison;
                view.ComparisonStats = detailedComparison;
                view.ComparisonSummary = LootRewardPresentationRules.ComparisonPreview(detailedComparison);
                screen.Refresh();
                Require(screen.ComparisonHintForTest == "2 more changes in Inventory",
                    "the visible receipt discloses that additional changes require review");
                view.Comparison = "ARM 2 (+1)";
                view.ComparisonStats = view.Comparison;
                view.ComparisonSummary = LootRewardPresentationRules.ComparisonPreview(view.Comparison);
                screen.Refresh();
                Require(screen.ComparisonHintForTest == "Full details in Inventory",
                    "refreshing a shorter comparison clears an old hidden-change count");
                view.Comparison = "Best fit: A / B  •  Review  •  " + detailedComparison;
                view.ComparisonStats = null;
                view.ComparisonSummary = "Authored comparison without a known preview limit.";
                screen.Refresh();
                Require(screen.ComparisonHintForTest == "Full details in Inventory",
                    "a custom summary without raw comparison data does not manufacture an omitted-change count from a name or heading");
                Require(equipped == 0 && reviewed == 0 && dismissed == 0, "refresh does not mutate rewards or equipment");
                screen.InvokeQuickEquipForTest(); screen.InvokeReviewForTest(); screen.InvokeDismissForTest();
                Require(equipped == 1 && reviewed == 1 && dismissed == 1, "actions still invoke exactly once");
                view = new LootPopupView { Visible = true, HasItem = false, Gold = 7, Supplies = 2, Elixirs = 1, Outcome = "Added to company stores" };
                screen.Refresh();
                Require(!screen.HasItemCardForTest && !screen.HasReviewActionForTest && !screen.HasQuickEquipActionForTest,
                    "resource-only refresh removes stale artwork and equipment actions");
                Require(screen.ItemDetailsForTest == "" && screen.ComparisonPreviewForTest == "" && screen.ComparisonHintForTest == "" && screen.HasDefaultFocusForTest,
                    "resource-only refresh clears old details without losing safe focus");
                screen.InvokeQuickEquipForTest(); screen.InvokeReviewForTest();
                Require(equipped == 1 && reviewed == 1, "hidden actions cannot invoke equipment mutations");
                Require(!LootRewardPresentationRules.HasResources(new LootPopupView { Gold = -1 })
                    && LootRewardPresentationRules.HasResources(new LootPopupView { Elixirs = 1 }),
                    "only positive actual resource rewards reserve summary space");
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static void DefaultSubmitUsesProductionDismissalWithoutMutatingRewards()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            EventSystem navigation = UiRuntime.EnsureEventSystemReady();
            GameObject previousSelection = navigation.currentSelectedGameObject;
            GameObject host = RemembranceItemSmoke.NewHost("Loot production default-submit smoke");
            LootPopupScreen screen = null;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                var state = new GameState
                {
                    Mode = GameMode.Explore, Seed = 2370, Depth = 1,
                    SaveVersion = VersionInfo.SaveVersion, ContentSetId = ContentSetCatalog.SewerSlice,
                    Gold = 164, Supplies = 7, Elixirs = 3, SfxMuted = true, MusicMuted = true, ReducedMotion = true
                };
                RemembranceItemSmoke.Set(game, "state", state);
                RemembranceItemSmoke.Set(game, "activeContentSet", ContentSetCatalog.SewerSlice);
                RemembranceItemSmoke.Set(game, "rng", new System.Random(state.Seed));
                RemembranceItemSmoke.Set(game, "visualSmokeSaveBlocked", true);
                RemembranceItemSmoke.Set(game, "launchError", "");
                string role = (string)RemembranceItemSmoke.Invoke(game, "RoleForClass", "warrior");
                PartyMember member = (PartyMember)RemembranceItemSmoke.Invoke(game, "MakeHero", "Continue guard", "human", "warrior", role,
                    new Stats(18, 18, 18, 18), StarterPartyCatalog.SpellSchoolForClass("warrior"), 99,
                    new SkillSet { Arms = 8 }.Normalize());
                state.Party.Add(member);
                RemembranceItemSmoke.Invoke(game, "RecalculateMember", member);
                member.Hp = member.MaxHp;
                InventoryItem current = new InventoryItem
                {
                    DisplayName = "plain iron broadsword", Slot = "weapon", Form = "broadsword", Material = "iron",
                    DamageMin = 2, DamageMax = 4, AttackSpeed = 8, DamageType = "physical", Rarity = "common"
                };
                InventoryItem reward = new InventoryItem
                {
                    DisplayName = "+4 iron broadsword", Slot = "weapon", Form = "broadsword", Material = "iron",
                    DamageMin = 12, DamageMax = 20, Bonus = 4, AttackSpeed = 12, DamageType = "physical", Rarity = "fine"
                };
                RemembranceItemSmoke.Invoke(game, "AddInventoryItem", current);
                object[] equipArguments = { current, member, "" };
                Require((bool)RemembranceItemSmoke.Invoke(game, "EquipInventoryItemToMember", equipArguments),
                    "production Continue fixture has real canonical equipment");
                RemembranceItemSmoke.Invoke(game, "AddInventoryItem", reward);

                // Use the real adapter bindings, not a replacement callback which
                // could accidentally make a dangerous production Submit pass.
                RemembranceItemSmoke.Invoke(game, "EnsureLootPopupScreen");
                screen = (LootPopupScreen)typeof(AshenHallsGame).GetField("lootPopupScreen", flags).GetValue(game);
                screen.transform.SetParent(null, false);
                screen.gameObject.SetActive(true);
                ProductionComparisonHintsIgnoreEditableNames(game, screen, member, reward);
                foreach (bool withItem in new[] { true, false })
                {
                    RemembranceItemSmoke.Invoke(game, "ShowLootPanel", withItem ? reward : null, 48, 2, 1, "", "Continue safety receipt");
                    Require(screen.IsVisible && screen.HasDefaultFocusForTest && screen.PrimaryActionLabelForTest == "Continue",
                        "production receipt opens with Continue owning default Submit");
                    Require(screen.HasQuickEquipActionForTest == withItem,
                        "the equipment case has a real available quick-equip action that default Submit must not take");
                    Require(string.IsNullOrEmpty(reward.EquippedById) && member.WeaponItemId == current.InstanceId,
                        "reward is still unequipped before default Submit");
                    string before = JsonUtility.ToJson(state);
                    GameObject defaultButton = navigation.currentSelectedGameObject;
                    Require(defaultButton != null && ExecuteEvents.Execute(defaultButton, new BaseEventData(navigation), ExecuteEvents.submitHandler),
                        "actual selected Unity button receives Submit");
                    Require(!(bool)RemembranceItemSmoke.Invoke(game, "IsLootPopupOpen") && !screen.IsVisible
                        && (UiOverlay)RemembranceItemSmoke.Invoke(game, "CurrentUiOverlay") == UiOverlay.None,
                        "production default Submit dismisses the receipt without opening equipment review");
                    Require(JsonUtility.ToJson(state) == before && state.Inventory.Contains(reward)
                        && string.IsNullOrEmpty(reward.EquippedById) && member.WeaponItemId == current.InstanceId,
                        "Continue preserves exact stored rewards, inventory identity, and every loadout value");
                    Require(navigation.currentSelectedGameObject != defaultButton,
                        "dismissal releases the receipt's selected button");
                    // A queued second submit must not collect twice or equip after
                    // the first callback cleared transient reward presentation.
                    ExecuteEvents.Execute(defaultButton, new BaseEventData(navigation), ExecuteEvents.submitHandler);
                    Require(JsonUtility.ToJson(state) == before && !(bool)RemembranceItemSmoke.Invoke(game, "IsLootPopupOpen"),
                        "a repeated stale Submit cannot repeat acquisition or equip the stored reward");
                }
            }
            finally
            {
                if (screen != null) UnityEngine.Object.DestroyImmediate(screen.gameObject);
                UnityEngine.Object.DestroyImmediate(host);
                navigation.SetSelectedGameObject(previousSelection);
            }
        }

        private static void ProductionComparisonHintsIgnoreEditableNames(AshenHallsGame game, LootPopupScreen screen,
            PartyMember member, InventoryItem reward)
        {
            string originalName = member.Name, originalDisplayName = reward.DisplayName, originalForm = reward.Form;
            int originalStrength = reward.StrengthBonus;
            try
            {
                member.Name = "A / B";
                reward.StrengthBonus = 2;
                foreach (bool sixthChange in new[] { false, true })
                {
                    // Life drain, damage, bonus, speed and attributes make five
                    // changes. A same-reach knife adds the sixth: lower role fit.
                    reward.Form = sixthChange ? "knife" : "broadsword";
                    reward.DisplayName = "+4 vampiric iron " + reward.Form;
                    RemembranceItemSmoke.Invoke(game, "ShowLootPanel", reward, 48, 2, 1, "", "Comparison count safety receipt");
                    LootPopupView view = (LootPopupView)RemembranceItemSmoke.Invoke(game, "BuildLootPopupView");
                    string raw = (string)RemembranceItemSmoke.Invoke(game, "InventoryComparisonLine", reward, member);
                    int changed = 0;
                    foreach (string entry in raw.Split(new[] { " / " }, StringSplitOptions.RemoveEmptyEntries))
                        if (!entry.Trim().EndsWith("(=)", StringComparison.Ordinal)) changed++;
                    Require(changed == (sixthChange ? 6 : 5), "production slash-name fixture has the exact changed-entry boundary");
                    Require(view.Comparison.Contains("A / B") && view.ComparisonStats == raw
                        && view.ComparisonSummary == LootRewardPresentationRules.ComparisonPreview(raw),
                        "production summary and overflow source use the same raw stats independently of the editable name");
                    Require(screen.ComparisonHintForTest == (sixthChange ? "1 more change in Inventory" : "Full details in Inventory"),
                        "a slash in a hero name never turns five real changes into a false overflow or inflates a six-change count");
                    Require(screen.HasDefaultFocusForTest && !screen.HasQuickEquipActionForTest,
                        "comparison disclosure preserves safe Continue and deliberate life-drain review");
                }
            }
            finally
            {
                member.Name = originalName;
                reward.DisplayName = originalDisplayName;
                reward.Form = originalForm;
                reward.StrengthBonus = originalStrength;
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Loot reward presentation smoke failed: " + message);
        }
    }
}
