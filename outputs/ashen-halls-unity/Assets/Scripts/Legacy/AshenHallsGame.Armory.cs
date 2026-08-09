using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private const int RouteWaypointRowKeyBase = 10000;
        private const int InventoryTargetPickerKey = -100;
        private const int InventoryFilterThreshold = 9;
        private bool armoryInventoryTargetPickerOpen;

        private void EnsureArmoryOverlayScreen()
        {
            if (armoryOverlayScreen != null && armoryOverlayScreen.IsReady) return;
            if (armoryOverlayScreen != null)
            {
                Destroy(armoryOverlayScreen.gameObject);
                armoryOverlayScreen = null;
            }
            GameObject screen = new GameObject("Armory Overlay Screen");
            screen.transform.SetParent(transform, false);
            ArmoryOverlayScreen created = screen.AddComponent<ArmoryOverlayScreen>();
            try
            {
                created.Bind(new ArmoryOverlayBindings
                {
                    View = BuildArmoryOverlayView,
                    Close = CloseArmoryOverlay,
                    SelectTab = SelectArmoryTab,
                    SelectFilter = SelectArmoryFilter,
                    RunRowAction = RunArmoryRowAction,
                    RunDetailAction = RunArmoryDetailAction
                });
                created.SetVisible(false);
                armoryOverlayScreen = created;
            }
            catch
            {
                created.SetVisible(false);
                screen.SetActive(false);
                Destroy(screen);
                throw;
            }
        }

        private void SyncArmoryOverlayScreen()
        {
            bool visible = state != null && CurrentUiOverlay() == UiOverlay.Armory && !ShouldShowStartupSplash();
            if (visible && (armoryOverlayScreen == null || !armoryOverlayScreen.IsReady))
            {
                TryInitializePresentationScreen("Armory overlay recovery", EnsureArmoryOverlayScreen, false);
            }
            if (armoryOverlayScreen == null)
            {
                if (!visible) DiscardArmoryGrowthDrafts();
                if (visible) RecoverUnavailableOverlay(UiOverlay.Armory, "Armory");
                return;
            }
            if (!visible)
            {
                armoryInventoryTargetPickerOpen = false;
                DiscardArmoryGrowthDrafts();
                armoryOverlayScreen.SetVisible(false);
                return;
            }

            bool refresh = !armoryOverlayScreen.HasRenderableGeometry
                || ShouldRefreshPresentation(ref lastArmoryRefreshKey, ArmoryRefreshKey());
            if (refresh)
            {
                armoryOverlayScreen.SetVisible(false);
                try
                {
                    armoryOverlayScreen.Refresh();
                    armoryOverlayScreen.SetVisible(true);
                    Canvas.ForceUpdateCanvases();
                }
                catch (Exception ex)
                {
                    armoryOverlayScreen.SetVisible(false);
                    Debug.LogException(new InvalidOperationException(VersionInfo.ProductName + " armory refresh failed.", ex));
                }
            }
            if (!armoryOverlayScreen.HasRenderableGeometry)
            {
                armoryOverlayScreen.SetVisible(false);
                RecoverUnavailableOverlay(UiOverlay.Armory, "Armory");
            }
        }

        private string ArmoryRefreshKey()
        {
            if (state == null) return "empty";
            int hash = armoryTab;
            hash = unchecked(hash * 31 + armoryPackFilter);
            hash = unchecked(hash * 31 + armorySelectedInventoryIndex);
            hash = unchecked(hash * 31 + armorySelectedPartyIndex);
            hash = unchecked(hash * 31 + (armoryInventoryTargetPickerOpen ? 1 : 0));
            hash = unchecked(hash * 31 + ArmoryGrowthDraftHash());
            hash = unchecked(hash * 31 + (state.ActiveRouteWaypointKey ?? "").GetHashCode());
            hash = unchecked(hash * 31 + state.Depth);
            hash = unchecked(hash * 31 + state.PlayerX);
            hash = unchecked(hash * 31 + state.PlayerY);
            hash = unchecked(hash * 31 + (state.Gold));
            if (state.StoryFlags != null)
            {
                foreach (string flag in state.StoryFlags.OrderBy(flag => flag ?? "", StringComparer.Ordinal))
                {
                    hash = unchecked(hash * 31 + (flag ?? "").GetHashCode());
                }
            }
            hash = unchecked(hash * 31 + (state.Inventory == null ? 0 : state.Inventory.Count));
            if (state.Inventory != null)
            {
                foreach (InventoryItem item in state.Inventory)
                {
                    if (item == null) continue;
                    hash = unchecked(hash * 31 + (item.DisplayName ?? "").GetHashCode());
                    hash = unchecked(hash * 31 + (item.EquippedById ?? "").GetHashCode());
                    hash = unchecked(hash * 31 + item.Bonus);
                }
            }
            if (state.Party != null)
            {
                foreach (PartyMember member in state.Party)
                {
                    if (member == null) continue;
                    hash = unchecked(hash * 31 + (member.Name ?? "").GetHashCode());
                    hash = unchecked(hash * 31 + member.Level);
                    hash = unchecked(hash * 31 + member.Experience);
                    hash = unchecked(hash * 31 + member.StatPoints);
                    hash = unchecked(hash * 31 + member.SkillPoints);
                    hash = unchecked(hash * 31 + member.Stats.Strength);
                    hash = unchecked(hash * 31 + member.Stats.Intelligence);
                    hash = unchecked(hash * 31 + member.Stats.Dexterity);
                    hash = unchecked(hash * 31 + member.Stats.Health);
                    if (member.Skills != null)
                    {
                        hash = unchecked(hash * 31 + member.Skills.Arms);
                        hash = unchecked(hash * 31 + member.Skills.Missile);
                        hash = unchecked(hash * 31 + member.Skills.Mend);
                        hash = unchecked(hash * 31 + member.Skills.Ember);
                        hash = unchecked(hash * 31 + member.Skills.Hex);
                        hash = unchecked(hash * 31 + member.Skills.Guard);
                    }
                    hash = unchecked(hash * 31 + member.Hp);
                    hash = unchecked(hash * 31 + member.Mana);
                    hash = unchecked(hash * 31 + (member.WeaponName ?? "").GetHashCode());
                    hash = unchecked(hash * 31 + (member.ArmorName ?? "").GetHashCode());
                }
            }
            if (state.Log != null && state.Log.Count > 0) hash = unchecked(hash * 31 + (state.Log[0].Text ?? "").GetHashCode());
            return "armory=" + hash;
        }

        private ArmoryOverlayView BuildArmoryOverlayView()
        {
            EnsureInventoryEquipmentLinks();
            int tab = Mathf.Clamp(armoryTab, 0, ArmoryTabCount - 1);
            NormalizeArmorySelections(tab);
            bool inventoryFiltersVisible = tab == (int)ArmoryTab.Pack && InventoryCategoryFiltersVisible();
            bool growthFiltersVisible = tab == (int)ArmoryTab.Growth && state?.Party != null && state.Party.Count > 0;
            return new ArmoryOverlayView
            {
                Visible = state != null && CurrentUiOverlay() == UiOverlay.Armory,
                ActiveTab = tab,
                ActiveFilter = inventoryFiltersVisible
                    ? armoryPackFilter
                    : growthFiltersVisible ? armorySelectedPartyIndex : 0,
                Title = ArmoryTitle(tab),
                Subtitle = ArmorySubtitle(tab),
                Summary = ArmorySummaryLine(),
                Footer = ArmoryFooterLine(tab),
                CompactRows = tab == (int)ArmoryTab.Pack,
                Filters = inventoryFiltersVisible
                    ? new[] { "All", "Weapons", "Armor" }
                    : growthFiltersVisible ? ArmoryGrowthMemberFilters() : Array.Empty<string>(),
                Rows = BuildArmoryRows(tab),
                Detail = BuildArmoryDetail(tab)
            };
        }

        private void CloseArmoryOverlay()
        {
            if (!showArmory) return;
            SuppressBoardPointer();
            armoryInventoryTargetPickerOpen = false;
            DiscardArmoryGrowthDrafts();
            showArmory = false;
            MarkUiDirty();
            SyncArmoryOverlayScreen();
            PlaySfx("uiclose", 0.45f);
        }

        private void SelectArmoryTab(int tab)
        {
            int next = Mathf.Clamp(tab, 0, ArmoryTabCount - 1);
            if (armoryTab == next) return;
            if (armoryTab == (int)ArmoryTab.Growth) DiscardArmoryGrowthDrafts();
            armoryInventoryTargetPickerOpen = false;
            armoryTab = next;
            NormalizeArmorySelections(next);
            SyncArmoryOverlayScreen();
            PlaySfx("uitab", 0.45f);
        }

        private void SelectArmoryFilter(int filter)
        {
            if (armoryTab == (int)ArmoryTab.Growth)
            {
                SelectArmoryGrowthMember(filter);
                return;
            }
            int next = Mathf.Clamp(filter, 0, 2);
            if (armoryPackFilter == next) return;
            armoryInventoryTargetPickerOpen = false;
            armoryPackFilter = next;
            NormalizeArmorySelections((int)ArmoryTab.Pack);
            MarkUiDirty();
            SyncArmoryOverlayScreen();
            PlaySfx("uitab", 0.40f);
        }

        private void RunArmoryRowAction(int key)
        {
            if (armoryTab == (int)ArmoryTab.Growth)
            {
                RunArmoryGrowthRowAction(key);
                return;
            }
            if (armoryTab == (int)ArmoryTab.Party)
            {
                if (state?.Party == null || key < 0 || key >= state.Party.Count) return;
                armorySelectedPartyIndex = key;
            }
            else if (armoryTab == (int)ArmoryTab.Pack)
            {
                if (state?.Inventory == null || key < 0 || key >= state.Inventory.Count) return;
                if (armorySelectedInventoryIndex != key) armoryInventoryTargetPickerOpen = false;
                armorySelectedInventoryIndex = key;
            }
            else if (armoryTab == (int)ArmoryTab.Journal)
            {
                if (!TryJournalRouteWaypoint(key, out WorldMapJunction junction)) return;
                bool clearing = RouteChartRules.IsWaypoint(state.ActiveRouteWaypointKey, state.Depth, junction.Id);
                state.ActiveRouteWaypointKey = clearing
                    ? ""
                    : RouteChartRules.WaypointKey(state.Depth, junction.Id);
                InvalidateActiveRouteWaypointPath();
                if (clearing)
                {
                    PushLog($"Waypoint cleared: {junction.Name}.", Tone.Normal);
                    ShowBanner("Waypoint Cleared");
                }
                else
                {
                    IReadOnlyList<Point> path = ActiveRouteWaypointPath();
                    string route = path.Count > 1
                        ? ActiveRouteWaypointFirstDirection(path) + " " + RouteChartRules.DistanceLabel(path.Count - 1)
                        : path.Count == 1
                            ? "here"
                            : RouteChartRules.DirectionLabel(state.PlayerX, state.PlayerY, junction.X, junction.Y) + " / route blocked";
                    PushLog($"Waypoint set: {junction.Name}, {route}.", Tone.Good);
                    ShowBanner(junction.Name);
                }
                MarkUiDirty();
                SyncArmoryOverlayScreen();
                PlaySfx(clearing ? "uiclose" : "wayfind", clearing ? 0.38f : 0.62f);
                return;
            }
            else return;
            MarkUiDirty();
            SyncArmoryOverlayScreen();
            PlaySfx("uiconfirm", 0.38f);
        }

        private bool TryJournalRouteWaypoint(int key, out WorldMapJunction junction)
        {
            junction = default;
            if (state?.Map == null || key < RouteWaypointRowKeyBase) return false;
            WorldMapJunction[] junctions = WorldMapGenerationRules.RegionalJunctions(
                state.Map.Width,
                state.Map.Height,
                state.Map.StartX,
                state.Map.StartY);
            int index = key - RouteWaypointRowKeyBase;
            if (index < 0 || index >= junctions.Length) return false;
            WorldMapJunction candidate = junctions[index];
            if (!RouteChartRules.IsCharted(state.DiscoveredZones, state.Depth, candidate.Id)) return false;
            junction = candidate;
            return true;
        }

        private void RunArmoryDetailAction(int partyIndex)
        {
            if (armoryTab == (int)ArmoryTab.Growth)
            {
                RunArmoryGrowthDetailAction(partyIndex);
                return;
            }
            if (armoryTab != (int)ArmoryTab.Pack
                || state?.Inventory == null
                || armorySelectedInventoryIndex < 0
                || armorySelectedInventoryIndex >= state.Inventory.Count)
            {
                return;
            }

            InventoryItem item = state.Inventory[armorySelectedInventoryIndex];
            if (partyIndex == InventoryTargetPickerKey)
            {
                if (!InventoryEquipmentRules.IsEquippable(item)) return;
                armoryInventoryTargetPickerOpen = !armoryInventoryTargetPickerOpen;
                MarkUiDirty();
                SyncArmoryOverlayScreen();
                PlaySfx("uitab", 0.40f);
                return;
            }
            if (state.Party == null || partyIndex < 0 || partyIndex >= state.Party.Count) return;

            PartyMember target = state.Party[partyIndex];
            bool equipped = EquipInventoryItemToMember(item, target, out string result);
            if (equipped) armoryInventoryTargetPickerOpen = false;
            PushLog(result, equipped ? Tone.Good : Tone.Warn);
            MarkUiDirty();
            SyncArmoryOverlayScreen();
            PlaySfx(equipped ? "itemequip" : "blocked", 0.55f);
        }

        private IReadOnlyList<ArmoryRowView> BuildArmoryRows(int tab)
        {
            if (state == null) return Array.Empty<ArmoryRowView>();
            if (tab == (int)ArmoryTab.Pack) return BuildArmoryPackRows();
            if (tab == (int)ArmoryTab.Spells) return BuildArmoryFormulaRows();
            if (tab == (int)ArmoryTab.Journal) return BuildArmoryJournalRows();
            if (tab == (int)ArmoryTab.Growth) return BuildArmoryGrowthRows();
            return BuildArmoryPartyRows();
        }

        private IReadOnlyList<ArmoryRowView> BuildArmoryPartyRows()
        {
            List<ArmoryRowView> rows = new List<ArmoryRowView>();
            if (state?.Party == null) return rows;
            for (int i = 0; i < state.Party.Count; i++)
            {
                PartyMember member = state.Party[i];
                if (member == null) continue;
                rows.Add(new ArmoryRowView
                {
                    Key = i,
                    Title = member.Name,
                    Subtitle = $"L{member.Level} {DisplayRace(member.Race)} {DisplayClass(member.ClassKey)}  •  HP {member.Hp}/{member.MaxHp}  •  MP {member.Mana}/{member.MaxMana}",
                    Detail = $"Weapon  {TrimGearName(member.WeaponName)}\nArmor     {TrimGearName(member.ArmorName)}",
                    AccentHex = ColorHtml(MemberColor(member)),
                    Badge = DisplayClass(member.ClassKey),
                    BadgeAccentHex = ColorHtml(MemberColor(member)),
                    ActionLabel = i == armorySelectedPartyIndex ? "Viewing" : "View",
                    ActionEnabled = true,
                    Selected = i == armorySelectedPartyIndex,
                    IconLabel = MemberInitials(member)
                });
            }
            return rows;
        }

        private IReadOnlyList<ArmoryRowView> BuildArmoryPackRows()
        {
            List<ArmoryRowView> rows = new List<ArmoryRowView>();
            if (state?.Inventory == null) return rows;
            foreach (int i in SortedInventoryIndices(armoryPackFilter))
            {
                InventoryItem item = state.Inventory[i];
                if (item == null) continue;
                bool equippable = InventoryEquipmentRules.IsEquippable(item);
                bool weapon = InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form);
                Color accent = weapon ? DamageColor(string.IsNullOrEmpty(item.DamageType) ? "physical" : item.DamageType) : teal;
                PartyMember owner = equippable ? EquippedMember(item) : null;
                int comparisonScore = 0;
                PartyMember best = equippable ? BestInventoryFit(item, out _, out comparisonScore) : null;
                InventoryUpgradeGrade grade = InventoryEquipmentRules.Grade(comparisonScore);
                TryGetInventoryItemIcon(item, out Texture2D iconTexture, out Rect iconUv);
                ArmoryRowView row = new ArmoryRowView
                {
                    Key = i,
                    Title = item.DisplayName,
                    Subtitle = $"{InventoryEquipmentRules.RarityLabel(item.Rarity)} {InventoryEquipmentRules.SlotLabel(item.Slot, item.Form)}",
                    Detail = !equippable
                        ? "Quest material · stored"
                        : owner != null
                            ? $"Equipped by {owner.Name}"
                            : best == null
                                ? "Stored"
                                : $"{InventoryEquipmentRules.GradeLabel(grade)} for {best.Name} · {CompactInventoryComparisonLine(item, best)}",
                    AccentHex = ColorHtml(accent),
                    Badge = "",
                    BadgeAccentHex = "",
                    ActionLabel = "",
                    ActionEnabled = true,
                    Selected = i == armorySelectedInventoryIndex,
                    IconTexture = iconTexture,
                    IconUv = iconUv,
                    IconLabel = LootIconLabel(item)
                };
                rows.Add(row);
            }
            return rows;
        }

        private ArmoryDetailView BuildArmoryDetail(int tab)
        {
            if (tab == (int)ArmoryTab.Pack) return BuildInventoryItemDetail();
            if (tab == (int)ArmoryTab.Party) return BuildPartyEquipmentDetail();
            if (tab == (int)ArmoryTab.Growth) return BuildArmoryGrowthDetail();
            return null;
        }

        private ArmoryDetailView BuildPartyEquipmentDetail()
        {
            if (state?.Party == null || state.Party.Count == 0) return null;
            int index = Mathf.Clamp(armorySelectedPartyIndex, 0, state.Party.Count - 1);
            PartyMember member = state.Party[index];
            if (member == null) return null;
            return new ArmoryDetailView
            {
                Eyebrow = "EQUIPPED LOADOUT",
                Title = member.Name,
                Subtitle = $"L{member.Level} {DisplayRace(member.Race)} {DisplayClass(member.ClassKey)}",
                Summary = $"WEAPON  {TrimGearName(member.WeaponName)}\n{WeaponSummaryLine(member)}\n\nARMOR  {TrimGearName(member.ArmorName)}\n{ArmorSummaryLine(member)}",
                AccentHex = ColorHtml(MemberColor(member)),
                IconLabel = MemberInitials(member)
            };
        }

        private ArmoryDetailView BuildInventoryItemDetail()
        {
            if (state?.Inventory == null
                || armorySelectedInventoryIndex < 0
                || armorySelectedInventoryIndex >= state.Inventory.Count)
            {
                return null;
            }

            InventoryItem item = state.Inventory[armorySelectedInventoryIndex];
            if (item == null) return null;
            TryGetInventoryItemIcon(item, out Texture2D iconTexture, out Rect iconUv);
            bool equippable = InventoryEquipmentRules.IsEquippable(item);
            PartyMember owner = equippable ? EquippedMember(item) : null;
            List<ArmoryDetailActionView> actions = new List<ArmoryDetailActionView>();
            if (equippable && owner != null)
            {
                int ownerIndex = state.Party == null ? -1 : state.Party.IndexOf(owner);
                actions.Add(new ArmoryDetailActionView
                {
                    Key = ownerIndex,
                    Label = owner.Name,
                    Detail = "Currently equipped",
                    ButtonLabel = "Equipped",
                    AccentHex = ColorHtml(MemberColor(owner)),
                    Enabled = false
                });
            }
            else if (equippable && state.Party != null)
            {
                PartyMember best = BestInventoryFit(item, out int bestIndex, out _);
                if (!armoryInventoryTargetPickerOpen && best != null)
                {
                    InventoryUpgradeGrade grade = InventoryGradeFor(item, best);
                    actions.Add(new ArmoryDetailActionView
                    {
                        Key = bestIndex,
                        Label = $"Best match · {best.Name}",
                        Detail = $"{InventoryEquipmentRules.GradeLabel(grade)} · {CompactInventoryComparisonLine(item, best)}",
                        ButtonLabel = "Equip",
                        AccentHex = ColorHtml(MemberColor(best)),
                        Enabled = true
                    });
                    if (state.Party.Count(member => member != null) > 1)
                    {
                        actions.Add(new ArmoryDetailActionView
                        {
                            Key = InventoryTargetPickerKey,
                            Label = "Choose another adventurer",
                            Detail = "Show the full party",
                            ButtonLabel = "Choose",
                            AccentHex = ColorHtml(teal),
                            Enabled = true
                        });
                    }
                }
                else
                {
                    actions.Add(new ArmoryDetailActionView
                    {
                        Key = InventoryTargetPickerKey,
                        Label = "Back to best match",
                        Detail = "Hide the full party",
                        ButtonLabel = "Back",
                        AccentHex = ColorHtml(teal),
                        Enabled = true
                    });
                    foreach (int partyIndex in Enumerable.Range(0, state.Party.Count)
                        .Where(index => state.Party[index] != null)
                        .OrderByDescending(index => InventoryComparisonScore(item, state.Party[index])))
                    {
                        PartyMember member = state.Party[partyIndex];
                        InventoryUpgradeGrade grade = InventoryGradeFor(item, member);
                        actions.Add(new ArmoryDetailActionView
                        {
                            Key = partyIndex,
                            Label = partyIndex == bestIndex ? $"Recommended · {member.Name}" : member.Name,
                            Detail = $"{InventoryEquipmentRules.GradeLabel(grade)} · {CompactInventoryComparisonLine(item, member)}",
                            ButtonLabel = "Equip",
                            AccentHex = ColorHtml(MemberColor(member)),
                            Enabled = true
                        });
                    }
                }
            }

            return new ArmoryDetailView
            {
                Eyebrow = $"{InventoryEquipmentRules.RarityLabel(item.Rarity).ToUpperInvariant()} {InventoryEquipmentRules.SlotLabel(item.Slot, item.Form).ToUpperInvariant()}",
                Title = item.DisplayName,
                Subtitle = !equippable
                    ? "Stored · not equippable"
                    : owner == null ? "Stored" : "Equipped by " + owner.Name,
                Summary = equippable
                    ? CompactInventoryItemSummary(item)
                    : "Quest material\n" + InventoryItemIdentityLine(item),
                AccentHex = InventoryRarityAccent(item.Rarity),
                IconTexture = iconTexture,
                IconUv = iconUv,
                IconLabel = LootIconLabel(item),
                Actions = actions
            };
        }

        private void NormalizeArmorySelections(int tab)
        {
            if (state?.Party == null || state.Party.Count == 0) armorySelectedPartyIndex = 0;
            else armorySelectedPartyIndex = Mathf.Clamp(armorySelectedPartyIndex, 0, state.Party.Count - 1);

            if (tab != (int)ArmoryTab.Pack || state?.Inventory == null)
            {
                if (state?.Inventory == null || state.Inventory.Count == 0) armorySelectedInventoryIndex = -1;
                return;
            }

            if (!InventoryCategoryFiltersVisible()) armoryPackFilter = 0;
            int previousSelection = armorySelectedInventoryIndex;
            List<int> visible = SortedInventoryIndices(armoryPackFilter);
            if (!visible.Contains(armorySelectedInventoryIndex))
            {
                armorySelectedInventoryIndex = visible.Count > 0 ? visible[0] : -1;
            }
            if (armorySelectedInventoryIndex != previousSelection) armoryInventoryTargetPickerOpen = false;
        }

        private bool InventoryCategoryFiltersVisible()
        {
            return (state?.Inventory?.Count(item => item != null) ?? 0) >= InventoryFilterThreshold;
        }

        private List<int> SortedInventoryIndices(int filter)
        {
            if (state?.Inventory == null) return new List<int>();
            return state.Inventory
                .Select((item, index) => new
                {
                    Item = item,
                    Index = index,
                    Owner = InventoryEquipmentRules.IsEquippable(item) ? EquippedMember(item) : null,
                    BestScore = InventoryEquipmentRules.IsEquippable(item) ? BestInventoryComparisonScore(item) : int.MinValue / 4
                })
                .Where(entry => entry.Item != null
                    && InventoryEquipmentRules.MatchesFilter(
                        entry.Item,
                        filter,
                        InventoryEquipmentRules.Grade(entry.BestScore) == InventoryUpgradeGrade.Upgrade))
                .OrderByDescending(entry => InventoryEquipmentRules.SortScore(entry.Item, entry.Owner != null, entry.BestScore))
                .ThenByDescending(entry => entry.Index)
                .Select(entry => entry.Index)
                .ToList();
        }

        private static string MemberInitials(PartyMember member)
        {
            if (member == null || string.IsNullOrWhiteSpace(member.Name)) return "PC";
            string[] words = member.Name.Split(new[] { ' ', '-', '\'' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0) return "PC";
            if (words.Length == 1) return words[0].Substring(0, Mathf.Min(2, words[0].Length)).ToUpperInvariant();
            return (words[0][0].ToString() + words[words.Length - 1][0]).ToUpperInvariant();
        }

        private static string InventoryItemIdentityLine(InventoryItem item)
        {
            if (item == null) return "";
            List<string> parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(item.Material)) parts.Add(item.Material);
            if (!string.IsNullOrWhiteSpace(item.Form)) parts.Add(item.Form);
            if (!string.IsNullOrWhiteSpace(item.Trait)) parts.Add(item.Trait);
            return parts.Count == 0 ? "adventuring gear" : string.Join("  •  ", parts);
        }

        private string CompactInventoryComparisonLine(InventoryItem item, PartyMember member)
        {
            if (item == null || member == null) return "";
            List<string> changes = new List<string>();
            if (InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form))
            {
                int oldMin = Mathf.Max(1, member.WeaponDamageMin > 0 ? member.WeaponDamageMin : member.DamageMin);
                int oldMax = Mathf.Max(oldMin + 1, member.WeaponDamageMax > 0 ? member.WeaponDamageMax : member.DamageMax);
                int newMin = Mathf.Max(1, item.DamageMin);
                int newMax = Mathf.Max(newMin + 1, item.DamageMax);
                AddCompactDelta(changes, "Damage", (newMin + newMax) / 2, (oldMin + oldMax) / 2);
                AddCompactDelta(changes, "Speed", Mathf.Max(1, item.AttackSpeed), Mathf.Max(1, member.WeaponAttackSpeed > 0 ? member.WeaponAttackSpeed : member.AttackSpeed));
                AddCompactDelta(changes, "Range", WeaponRange(item, member), Mathf.Max(1, member.Range));
            }
            else if (InventoryEquipmentRules.IsArmorSlot(item.Slot, item.Form))
            {
                AddCompactDelta(changes, "Armor", ArmorDefenseBonus(item), member.ArmorBonus);
                AddCompactDelta(changes, "Agility", ArmorAgilityModifier(item.DisplayName), ArmorAgilityModifier(member.ArmorName));
            }
            return changes.Count == 0 ? "Core stats unchanged" : string.Join(" · ", changes.Take(2));
        }

        private string CompactInventoryItemSummary(InventoryItem item)
        {
            if (item == null) return "";
            List<string> core = new List<string>();
            List<string> traits = new List<string>();
            if (InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form))
            {
                PartyMember context = BestInventoryFit(item, out _, out _) ?? state?.Party?.FirstOrDefault();
                if (item.DamageMin > 0 && item.DamageMax > 0) core.Add($"{item.DamageMin}–{item.DamageMax} damage");
                if (item.AttackSpeed > 0) core.Add($"Speed {item.AttackSpeed}");
                int range = WeaponRange(item, context ?? new PartyMember { Role = "" });
                core.Add(range > 1 ? $"Range {range}" : "Melee");
                if (!string.IsNullOrWhiteSpace(item.DamageType) && !string.Equals(item.DamageType, "physical", StringComparison.OrdinalIgnoreCase))
                {
                    traits.Add(InventoryDisplayToken(item.DamageType));
                }
                string enchantment = WeaponEnchantmentRules.StatusText(item);
                if (!string.IsNullOrWhiteSpace(enchantment)) traits.Add(enchantment);
                if (!string.IsNullOrWhiteSpace(item.Trait)) traits.Add(InventoryDisplayToken(item.Trait));
                string status = GearOnHitStatus(item.DisplayName);
                if (!string.IsNullOrWhiteSpace(status)) traits.Add(InventoryDisplayToken(status) + " chance");
                if (GearLifeDrainAmount(item.DisplayName, Mathf.Max(1, item.DamageMax)) > 0) traits.Add("Life drain");
            }
            else
            {
                core.Add($"Armor {ArmorDefenseBonus(item)}");
                int agility = ArmorAgilityModifier(item.DisplayName);
                if (agility != 0) core.Add("Agility " + InventoryEquipmentRules.SignedDelta(agility));
                if (!string.IsNullOrWhiteSpace(item.Trait)) traits.Add(InventoryDisplayToken(item.Trait));
                if ((item.DisplayName ?? "").IndexOf("ward", StringComparison.OrdinalIgnoreCase) >= 0) traits.Add("Wards magic");
            }

            string stats = ItemStatBonusLine(item);
            if (!string.IsNullOrWhiteSpace(stats)) traits.Add(stats);
            string firstLine = core.Count == 0 ? "Adventuring gear" : string.Join(" · ", core);
            return traits.Count == 0 ? firstLine : firstLine + "\n" + string.Join(" · ", traits.Take(4));
        }

        private static string InventoryDisplayToken(string value)
        {
            string normalized = (value ?? "").Trim().Replace('-', ' ');
            return normalized.Length == 0
                ? ""
                : char.ToUpperInvariant(normalized[0]) + normalized.Substring(1);
        }

        private static void AddCompactDelta(List<string> changes, string label, int next, int current)
        {
            int delta = next - current;
            if (delta == 0) return;
            changes.Add($"{label} {InventoryEquipmentRules.SignedDelta(delta)}");
        }

        private string ArmorySummaryLine()
        {
            int partyCount = state?.Party?.Count(member => member != null) ?? 0;
            int inventoryCount = state?.Inventory?.Count(item => item != null) ?? 0;
            return $"{partyCount} adventurers  •  {inventoryCount} items  •  {state?.Gold ?? 0} gold";
        }

        private IReadOnlyList<ArmoryRowView> BuildArmoryFormulaRows()
        {
            List<ArmoryRowView> rows = new List<ArmoryRowView>();
            foreach (FormulaDef formula in ActiveFormulaBook())
            {
                if (formula == null) continue;
                rows.Add(new ArmoryRowView
                {
                    Key = -1,
                    Title = $"{formula.Code} - {formula.Name}",
                    Subtitle = $"{SpellCraftLabel(formula.School)} / {FormulaRuleLine(formula)}",
                    Detail = $"{formula.Hint}\n{FormulaEffectLine(formula)}",
                    AccentHex = ColorHtml(FormulaColor(formula))
                });
            }
            return rows;
        }

        private IReadOnlyList<ArmoryRowView> BuildArmoryJournalRows()
        {
            List<ArmoryRowView> rows = new List<ArmoryRowView>();
            if (state == null) return rows;
            if (!ContentSetCatalog.ShowPrototypeScaffold(activeContentSet))
            {
                BuildSewerSliceJournalRows(rows);
                if (ContentSetCatalog.AllowBoneRoadChapter(activeContentSet, state.StoryFlags))
                {
                    AddBoneRoadJournalRows(rows);
                }
                if (HasStoryFlag(StoryFlags.GlassAndAshFrontierSurveyed)
                    || HasGlassAndAshStoryProgress())
                {
                    AddGlassAndAshJournalRows(rows);
                }
                AddChartedRegionalSiteJournalRows(rows);
                return rows;
            }

            WorldZone zone = state.Map == null ? null : ZoneAt(state.PlayerX, state.PlayerY);
            rows.Add(new ArmoryRowView
            {
                Key = -1,
                Title = StoryChapterTitle(),
                Subtitle = $"Depth {state.Depth} / {(zone == null ? "Unknown road" : zone.Name)} / {RouteProgressSummary()}",
                Detail = state.ActiveStory ?? StoryObjectiveForDepth(state.Depth),
                AccentHex = ColorHtml(gold)
            });
            AddJournalRouteChartRows(rows);

            string[] chapters = { "I Cisterns", "II Smoke", "III Bone", "IV Glass", "V Gate", "VI Crown" };
            for (int i = 0; i < chapters.Length; i++)
            {
                int depth = i + 1;
                bool complete = ChapterComplete(depth);
                bool active = !complete && ChapterActive(depth);
                rows.Add(new ArmoryRowView
                {
                    Key = -1,
                    Title = chapters[i],
                    Subtitle = complete ? "complete" : active ? "current chapter" : "future scaffold",
                    Detail = StoryObjectiveForDepth(depth),
                    AccentHex = ColorHtml(complete ? teal : active ? gold : line)
                });
            }

            rows.Add(new ArmoryRowView
            {
                Key = -1,
                Title = "Mira's Lamp Round",
                Subtitle = LampRoundJournalStatus(),
                Detail = "Temple healer, market clerk, Kate's Diner, tavern keeper.",
                AccentHex = ColorHtml(teal)
            });
            rows.Add(new ArmoryRowView
            {
                Key = -1,
                Title = "Gate Survey",
                Subtitle = GateSurveyJournalStatus(),
                Detail = "Gate Captain Brann, West Gate, East Gate.",
                AccentHex = ColorHtml(gold)
            });

            AddKoboldRouteJournalRows(rows);

            AddJournalContactRows(rows);
            AddJournalScaffoldRows(rows);
            return rows;
        }

        private void AddJournalRouteChartRows(List<ArmoryRowView> rows)
        {
            if (rows == null || state?.Map == null) return;
            WorldMapJunction[] junctions = WorldMapGenerationRules.RegionalJunctions(
                state.Map.Width,
                state.Map.Height,
                state.Map.StartX,
                state.Map.StartY);
            int charted = RouteChartRules.CountCharted(junctions, state.DiscoveredZones, state.Depth);
            string nearestLine = "Walk the outer road and step onto marked clearings to chart its turns.";
            bool hasActiveWaypoint = RouteChartRules.TryResolveWaypoint(
                junctions,
                state.DiscoveredZones,
                state.Depth,
                state.ActiveRouteWaypointKey,
                out WorldMapJunction activeWaypoint);
            if (RouteChartRules.TryNearestCharted(junctions, state.DiscoveredZones, state.Depth, state.PlayerX, state.PlayerY, out RouteChartReading nearest))
            {
                nearestLine = hasActiveWaypoint
                    ? $"Waypoint: {activeWaypoint.Name}. The map now traces a walkable route from the party."
                    : nearest.Distance == 0
                        ? $"The party stands at {nearest.Junction.Name}. Mark any charted turn below."
                        : $"Nearest marker: {nearest.Junction.Name}, {nearest.Direction} {RouteChartRules.DistanceLabel(nearest.Distance)}. Mark a turn below.";
            }

            rows.Add(new ArmoryRowView
            {
                Key = -1,
                Title = "Outer Road Chart",
                Subtitle = hasActiveWaypoint
                    ? $"{charted}/{junctions.Length} junctions charted / {activeWaypoint.Name} marked"
                    : $"{charted}/{junctions.Length} junctions charted",
                Detail = nearestLine,
                AccentHex = ColorHtml(charted >= junctions.Length ? teal : charted > 0 ? gold : line),
                Badge = hasActiveWaypoint ? "WAYPOINT" : "",
                BadgeAccentHex = ColorHtml(gold)
            });

            for (int i = 0; i < junctions.Length; i++)
            {
                WorldMapJunction junction = junctions[i];
                if (!RouteChartRules.IsCharted(state.DiscoveredZones, state.Depth, junction.Id)) continue;
                int distance = Mathf.Abs(junction.X - state.PlayerX) + Mathf.Abs(junction.Y - state.PlayerY);
                string bearing = distance == 0
                    ? "current position"
                    : RouteChartRules.DirectionLabel(state.PlayerX, state.PlayerY, junction.X, junction.Y) + " / " + RouteChartRules.DistanceLabel(distance);
                WorldZone junctionZone = ZoneAt(junction.X, junction.Y);
                bool active = RouteChartRules.IsWaypoint(state.ActiveRouteWaypointKey, state.Depth, junction.Id);
                rows.Add(new ArmoryRowView
                {
                    Key = RouteWaypointRowKeyBase + i,
                    Title = junction.Name,
                    Subtitle = $"{junctionZone.Name} / {bearing}",
                    Detail = junction.Summary,
                    AccentHex = ColorHtml(active ? gold : ZoneDangerColor(junctionZone)),
                    Badge = active ? "WAYPOINT" : "CHARTED",
                    BadgeAccentHex = ColorHtml(active ? gold : ZoneDangerColor(junctionZone)),
                    ActionLabel = active ? "Clear" : "Mark",
                    ActionEnabled = true,
                    Selected = active
                });
            }
        }

        private void AddKoboldRouteJournalRows(List<ArmoryRowView> rows)
        {
            if (rows == null) return;
            string[] labels = { "Dusk Market Ambush", "Smoke Cave", "Varkh's Hall", "Bone Road" };
            string[] hints =
            {
                "Follow the Old Road east through Lanternless Cross into the ruined market and survive the scouts among the broken stalls.",
                "Find the cave mouth beyond the ambush, then break through smoke, webs, and shieldbearers.",
                "Enter the Kobold King's hall, outlast Varkh's rally magic, and claim the route reward.",
                "Take Varkh's east passage onto the Bone Road and break the grave-scout watch above Gloam Courts."
            };
            for (int i = 0; i < labels.Length; i++)
            {
                bool done = KoboldRouteStepComplete(i);
                bool active = i == KoboldRouteActiveStep();
                rows.Add(new ArmoryRowView
                {
                    Key = -1,
                    Title = "Kobold Smoke - " + labels[i],
                    Subtitle = done ? "complete" : active ? "current" : "pending",
                    Detail = hints[i],
                    AccentHex = ColorHtml(done ? teal : active ? gold : blood),
                    Badge = done ? "DONE" : active ? "NEXT" : "",
                    BadgeAccentHex = ColorHtml(done ? teal : active ? gold : line)
                });
            }
        }

        private void AddBoneRoadJournalRows(List<ArmoryRowView> rows)
        {
            if (rows == null || state == null) return;

            bool[] done =
            {
                HasStoryFlag(StoryFlags.BoneRoadWatchDefeated),
                HasStoryFlag(StoryFlags.GloamRitualBroken),
                HasStoryFlag(StoryFlags.GloamWardenDefeated),
                HasStoryFlag(StoryFlags.RedGateWarningRecovered)
            };
            int activeStep = Array.FindIndex(done, complete => !complete);
            string watchHint = HasStoryFlag(StoryFlags.BoneRoadWatchSprung)
                ? "The grave-scout watch has closed on the road. Hold formation and finish the ambush."
                : HasStoryFlag(StoryFlags.BoneRoadEntered)
                    ? "Advance along the exposed causeway and find the grave-scout watch before it cuts the road behind you."
                    : "Take Varkh's east passage onto the Bone Road and follow the old causeway toward Gloam Courts.";
            string[] labels =
            {
                "Bone Road Watch",
                "Gloam Ritual",
                "Ossuary Warden",
                "Red Gate Warning"
            };
            string[] hints =
            {
                watchHint,
                "Follow the courtward road to Gloam Deep Crypt and disrupt the marrow rite feeding its dead.",
                "Press into Gloam Deep Crypt after the ritual breaks and bring down the Ossuary Warden.",
                "Cross to the Red Gate Seal, read the outer ward, recover its warning, and chart the Glass-and-Ash frontier."
            };

            for (int i = 0; i < labels.Length; i++)
            {
                bool active = i == activeStep;
                rows.Add(new ArmoryRowView
                {
                    Key = -1,
                    Title = "Bone Road - " + labels[i],
                    Subtitle = done[i] ? "done" : active ? "current" : "pending",
                    Detail = hints[i],
                    AccentHex = ColorHtml(done[i] ? teal : active ? gold : line),
                    Badge = done[i] ? "DONE" : active ? "NEXT" : "",
                    BadgeAccentHex = ColorHtml(done[i] ? teal : active ? gold : line)
                });
            }
        }

        private void AddGlassAndAshJournalRows(List<ArmoryRowView> rows)
        {
            if (rows == null || state == null) return;

            bool[] done =
            {
                HasStoryFlag(StoryFlags.GlassAndAshExpeditionAccepted),
                HasStoryFlag(StoryFlags.GlasswardAmbushDefeated),
                HasStoryFlag(StoryFlags.GlassIndexRecovered),
                HasStoryFlag(StoryFlags.EmberglassGateKeyRecovered)
            };
            int activeStep = Array.FindIndex(done, complete => !complete);
            string[] labels =
            {
                "Yara's Briefing",
                "Glass-Warren Levy",
                "Mirror Index",
                "Ashen Pact"
            };
            string[] hints =
            {
                "Return the frontier survey to Yara in Midgaard. Review the library route, far seal, and recall rule before crossing.",
                "Cross at the Red Gate passage, reach the Glass Lore Library in the north-east warrens, and break the drow levy.",
                "Return to the library after the levy falls and defeat the reflected keepers guarding its true-road Index.",
                "Carry the Mirror Index south-east to the far seal, break its cinder pact, and recover the Emberglass key."
            };

            for (int i = 0; i < labels.Length; i++)
            {
                bool active = i == activeStep;
                rows.Add(new ArmoryRowView
                {
                    Key = -1,
                    Title = "Glass and Ash - " + labels[i],
                    Subtitle = done[i] ? "done" : active ? "current" : "pending",
                    Detail = hints[i],
                    AccentHex = ColorHtml(done[i] ? teal : active ? gold : line),
                    Badge = done[i] ? "DONE" : active ? "NEXT" : "",
                    BadgeAccentHex = ColorHtml(done[i] ? teal : active ? gold : line)
                });
            }
        }

        private void AddChartedRegionalSiteJournalRows(List<ArmoryRowView> rows)
        {
            if (rows == null || state?.Map == null) return;
            foreach (WorldMapSite site in WorldMapGenerationRules.RegionalSites(
                state.Map.Width,
                state.Map.Height,
                state.Map.StartX,
                state.Map.StartY))
            {
                if (!HasStoryFlag(WorldSiteInteractionRules.ChartFlag(state.Depth, site.Id))
                    || !WorldSiteInteractionRules.TryGet(site.Id, out WorldSiteInteractionProfile profile))
                {
                    continue;
                }

                int distance = Mathf.Abs(site.X - state.PlayerX) + Mathf.Abs(site.Y - state.PlayerY);
                string bearing = distance == 0
                    ? "current position"
                    : RouteChartRules.DirectionLabel(state.PlayerX, state.PlayerY, site.X, site.Y)
                        + " / " + RouteChartRules.DistanceLabel(distance);
                WorldZone zone = ZoneAt(site.X, site.Y);
                bool rewardClaimed = WorldSiteInteractionRules.RewardClaimed(
                    state.StoryFlags,
                    state.Depth,
                    site.Id);
                string rewardState = rewardClaimed ? "CLAIMED" : "READY";
                string repeatCost = WorldSiteRepeatCost(profile);
                rows.Add(new ArmoryRowView
                {
                    Key = -1,
                    Title = site.Name,
                    Subtitle = $"{(zone == null ? site.ZoneId : zone.Name)} / {bearing}",
                    Detail = $"{rewardState}: {profile.RewardSummary}\n{profile.ServiceName} repeat service: {profile.RepeatSummary} Cost: {repeatCost}.",
                    AccentHex = ColorHtml(rewardClaimed ? teal : gold),
                    Badge = rewardState,
                    BadgeAccentHex = ColorHtml(rewardClaimed ? teal : gold)
                });
            }
        }

        private static string WorldSiteRepeatCost(WorldSiteInteractionProfile profile)
        {
            if (profile == null) return "none";
            List<string> costs = new List<string>();
            if (profile.RepeatSupplyCost > 0)
            {
                costs.Add(profile.RepeatSupplyCost + (profile.RepeatSupplyCost == 1 ? " supply" : " supplies"));
            }
            if (profile.RepeatGoldCost > 0)
            {
                costs.Add(profile.RepeatGoldCost + " gold");
            }
            return costs.Count == 0 ? "none" : string.Join(" + ", costs);
        }

        private void BuildSewerSliceJournalRows(List<ArmoryRowView> rows)
        {
            int cleared = ContentSetCatalog.SewerSliceClearedCount(state.StoryFlags);
            bool accepted = HasStoryFlag(StoryFlags.SewerContractAccepted) || HasStoryFlag(StoryFlags.MidgaardRatQuestGiven);
            bool rewardReady = ContentSetCatalog.SewerSliceRewardReady(state.StoryFlags, state.Inventory);
            bool rewardClaimed = ContentSetCatalog.SewerSliceComplete(state.StoryFlags);
            bool safeRoomChoice = ContentSetCatalog.HasSewerSafeRoomChoice(state.StoryFlags);
            EncounterDefinition next = cleared < ContentSetCatalog.SewerSliceEncounters.Count
                ? ContentSetCatalog.SewerSliceEncounterForProgress(cleared)
                : null;

            rows.Add(new ArmoryRowView
            {
                Key = -1,
                Title = "Midgaard Sewer Contract",
                Subtitle = rewardClaimed ? "complete" : rewardReady ? "reward ready" : accepted ? "active" : "available",
                Detail = SewerSliceJournalObjective(accepted, cleared, rewardReady, rewardClaimed, next),
                AccentHex = ColorHtml(rewardClaimed ? teal : rewardReady ? gold : accepted ? poison : line)
            });

            rows.Add(new ArmoryRowView
            {
                Key = -1,
                Title = "Sewer Rooms",
                Subtitle = $"{cleared}/{ContentSetCatalog.SewerSliceEncounters.Count} cleared",
                Detail = next == null
                    ? "Broken Sluice, Foul Runoff, and the Cistern Den are clear."
                    : $"Next room: {next.Banner}. {next.Intro}",
                AccentHex = ColorHtml(cleared >= ContentSetCatalog.SewerSliceEncounters.Count ? teal : gold)
            });

            rows.Add(new ArmoryRowView
            {
                Key = -1,
                Title = "Dry Maintenance Alcove",
                Subtitle = safeRoomChoice
                    ? HasStoryFlag(StoryFlags.SewerSafeRoomFocusChosen) ? "stormglass focus chosen" : "sluicekeeper blade chosen"
                    : cleared >= 2 ? "choice ready" : "undiscovered",
                Detail = safeRoomChoice
                    ? "The chosen service weapon is in the party's pack or already equipped."
                    : cleared >= 2
                        ? "Return to the sewer grate and choose one tool before entering the Cistern Den."
                        : "A recovery cache lies beyond Foul Runoff.",
                AccentHex = ColorHtml(safeRoomChoice ? teal : cleared >= 2 ? gold : line)
            });

            int proof = ContentSetCatalog.CountSewerSliceProof(state.Inventory);
            rows.Add(new ArmoryRowView
            {
                Key = -1,
                Title = "Armorer's Reward",
                Subtitle = rewardClaimed ? "claimed" : rewardReady ? "ready" : $"proof {proof}/{ContentSetCatalog.SewerSliceRequiredProofCount}",
                Detail = rewardClaimed
                    ? "Rat-pelt armor is claimed. The East Gate now opens the Old Road toward Dusk Market."
                    : rewardReady
                        ? "Return to the Midgaard armorer to trade the sewer proof for stitched rat-pelt armor."
                        : "Each authored sewer room yields one proof bundle. Bring three to the armorer.",
                AccentHex = ColorHtml(rewardClaimed ? teal : rewardReady ? gold : line)
            });

            if (rewardClaimed)
            {
                rows.Add(new ArmoryRowView
                {
                    Key = -1,
                    Title = "Old Road Chapter II",
                    Subtitle = HasStoryFlag(StoryFlags.KoboldKingDefeated)
                        ? "Kobold Smoke complete"
                        : state.Depth >= 2 ? "route in progress" : "Eastbound Old Road open",
                    Detail = HasStoryFlag(StoryFlags.KoboldKingDefeated)
                        ? "Varkh is defeated and the road beyond his hall is the next chapter frontier."
                        : state.Depth >= 2
                            ? "Follow the live route through Dusk Market, Smoke Cave, and the Kobold King's hall."
                            : "Follow the Old Road east through Lanternless Cross. The bounded Chapter II route is now playable.",
                    AccentHex = ColorHtml(moss)
                });
                AddKoboldRouteJournalRows(rows);
                AddJournalRouteChartRows(rows);
            }
        }

        private string SewerSliceJournalObjective(bool accepted, int cleared, bool rewardReady, bool rewardClaimed, EncounterDefinition next)
        {
            if (rewardClaimed) return "First-play slice complete: sewer contract, proof, and starter reward are finished.";
            if (rewardReady) return "The sewer is clear. Return proof to the Midgaard armorer and claim the first equipment reward.";
            if (!accepted) return "Speak with King Halvard or the royal herald to accept the first sewer contract.";
            if (cleared >= 2 && !ContentSetCatalog.HasSewerSafeRoomChoice(state.StoryFlags)) return "Choose one service weapon from the dry maintenance alcove before entering the Cistern Den.";
            if (next == null) return "Return proof to the Midgaard armorer.";
            return $"Clear {next.Banner}: room {cleared + 1}/{ContentSetCatalog.SewerSliceEncounters.Count}.";
        }

        private void AddJournalContactRows(List<ArmoryRowView> rows)
        {
            rows.Add(JournalContactRow("Mira of Midgaard", "town healer", "Healing lamps, supplies, route recap.", true, teal));
            rows.Add(JournalContactRow("Dusk Market Scout", "route witness", "Ambush clues and cave charm rumors.", HasStoryFlag(StoryFlags.KoboldAmbushSurvived) || state.Depth >= 2, gold));
            rows.Add(JournalContactRow("Green Shrine Priest", "tree-cover tutor", "Priest formula lessons and shrine lore.", ZoneWasDiscovered("green-shrine-road"), moss));
            rows.Add(JournalContactRow("Old Quarry Mason", "gear workbench", "Stone blocks, bridges, and heavy armor notes.", ZoneWasDiscovered("old-quarry"), stone));
            rows.Add(JournalContactRow("Glass Warren Adept", "hostile archivist", "Mirror routes, false aisles, and caster pressure.", ZoneWasDiscovered("glass-warrens"), frost));
            rows.Add(JournalContactRow("Ashen Pact Warden", "far-seal keybearer", "Cinder troops, drow rent, and the Emberglass key.", ZoneWasDiscovered("red-gate") || state.Depth >= 4, blood));
        }

        private ArmoryRowView JournalContactRow(string name, string role, string note, bool available, Color accent)
        {
            return new ArmoryRowView
            {
                Key = -1,
                Title = available ? name : "Unknown Contact",
                Subtitle = role,
                Detail = available ? note : "Discover the matching zone or route beat to unlock this contact.",
                AccentHex = ColorHtml(available ? accent : line)
            };
        }

        private void AddJournalScaffoldRows(List<ArmoryRowView> rows)
        {
            foreach (RouteScaffoldDef def in RouteScaffoldDefs())
            {
                if (def == null) continue;
                bool unlocked = state != null && state.Depth >= def.MinDepth;
                bool seen = RouteScaffoldVisited(def) || ZoneWasDiscovered(def.ZoneId);
                string status = RouteScaffoldVisited(def) ? "tested" : seen ? "located" : unlocked ? "find on route" : "locked";
                rows.Add(new ArmoryRowView
                {
                    Key = -1,
                    Title = def.Name,
                    Subtitle = $"{def.Purpose} / depth {def.MinDepth}+ / {status}",
                    Detail = def.Summary,
                    AccentHex = ColorHtml(RouteScaffoldVisited(def) ? teal : unlocked && seen ? def.Accent : line)
                });
            }
        }

        private string ArmoryTitle(int tab)
        {
            if (tab == (int)ArmoryTab.Pack) return "Inventory";
            if (tab == (int)ArmoryTab.Spells) return "Spell Reference";
            if (tab == (int)ArmoryTab.Journal) return "Journal";
            if (tab == (int)ArmoryTab.Growth) return "Party Growth";
            return "Equipment";
        }

        private string ArmorySubtitle(int tab)
        {
            if (tab == (int)ArmoryTab.Pack) return "";
            if (tab == (int)ArmoryTab.Spells) return FormulaCasterSummary();
            if (tab == (int)ArmoryTab.Journal) return ContentSetCatalog.ShowPrototypeScaffold(activeContentSet)
                ? "Story beats, city errands, charted roads, selectable waypoints, and future scaffold hooks."
                : "Active road campaign: Midgaard Cisterns, Kobold Smoke, the Bone Road, and charted regional services.";
            if (tab == (int)ArmoryTab.Growth) return ArmoryGrowthSubtitle();
            return "Review every adventurer's current weapon, armor, and combat-facing stats.";
        }

        private string ArmoryFooterLine()
        {
            return ArmoryFooterLine(Mathf.Clamp(armoryTab, 0, ArmoryTabCount - 1));
        }

        private string ArmoryFooterLine(int tab)
        {
            if (tab == (int)ArmoryTab.Party) return "Equipment: select an adventurer for a complete loadout readout.";
            if (tab == (int)ArmoryTab.Pack) return "Select an item  •  Equip on the right  •  Esc closes";
            if (tab == (int)ArmoryTab.Spells) return "Spells tab: choose Ability in combat, select a formula, then click a highlighted target.";
            if (tab == (int)ArmoryTab.Growth) return ArmoryGrowthFooter();
            return ContentSetCatalog.ShowPrototypeScaffold(activeContentSet)
                ? "Journal tab: mark any charted road turn to draw a path and replace automatic guidance."
                : "Journal tab: follow the active road campaign; charted sites record one-time rewards and repeat services.";
        }

        private string LampRoundJournalStatus()
        {
            if (HasStoryFlag(StoryFlags.MidgaardLampRoundComplete)) return "Complete / elixir and provisions earned.";
            if (HasStoryFlag(StoryFlags.MidgaardLampRoundStarted)) return "Active / " + LampRoundStatusLine() + ".";
            return "Available / speak with Mira near Temple Square.";
        }

        private string GateSurveyJournalStatus()
        {
            if (HasStoryFlag(StoryFlags.MidgaardGateSurveyComplete)) return "Complete / gold and provisions earned.";
            if (HasStoryFlag(StoryFlags.MidgaardGateSurveyStarted)) return "Active / " + GateSurveyStatusLine() + ".";
            return "Available / speak with the gate captain by the West Gate.";
        }

        private bool ChapterComplete(int depth)
        {
            if (depth <= 1) return state != null && state.Depth > 1;
            if (depth == 2) return HasStoryFlag(StoryFlags.KoboldKingDefeated) || state != null && state.Depth > 2;
            if (depth == 3) return ContentSetCatalog.BoneRoadComplete(state?.StoryFlags) || state != null && state.Depth > 3;
            return state != null && state.Depth > depth;
        }

        private bool ChapterActive(int depth)
        {
            if (state == null) return false;
            if (depth == 2 && !HasStoryFlag(StoryFlags.KoboldKingDefeated)) return state.Depth == 2;
            if (depth == 3 && !ContentSetCatalog.BoneRoadComplete(state.StoryFlags)) return state.Depth == 3;
            return Mathf.Clamp(state.Depth, 1, FinalBossDepth) == depth;
        }

        private bool RouteScaffoldVisited(RouteScaffoldDef def)
        {
            if (def == null || state?.StoryFlags == null) return false;
            string type = SanitizeFlagPart(def.Type.ToString());
            string zone = SanitizeFlagPart(def.ZoneId);
            return state.StoryFlags.Any(flag => flag != null && flag.Contains("_" + zone + "_") && flag.Contains("_" + type + "_visited"));
        }

        private bool ZoneWasDiscovered(string zoneId)
        {
            if (state?.DiscoveredZones == null || string.IsNullOrEmpty(zoneId)) return false;
            if (state.DiscoveredZones.Contains(zoneId)) return true;
            if (state.DiscoveredZones.Contains(ZoneKey(state.Depth, zoneId))) return true;
            return state.DiscoveredZones.Any(z => z.EndsWith(":" + zoneId, StringComparison.OrdinalIgnoreCase));
        }

        private string RouteProgressSummary()
        {
            if (state == null) return "";
            if (HasStoryFlag(StoryFlags.KoboldKingDefeated)) return "Kobold Smoke complete";
            if (HasStoryFlag(StoryFlags.KoboldCaveCleared)) return "King hall open";
            if (HasStoryFlag(StoryFlags.KoboldAmbushSurvived)) return "Smoke cave revealed";
            if (HasStoryFlag(StoryFlags.KoboldAmbushSprung)) return "Ambush active";
            return state.Depth == 2 ? "Dusk Market route pending" : "Road scaffold";
        }

        private static string ColorHtml(Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }
    }
}
