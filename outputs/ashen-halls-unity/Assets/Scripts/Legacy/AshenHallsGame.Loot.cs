using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private void EnsureLootPopupScreen()
        {
            if (lootPopupScreen != null && lootPopupScreen.IsReady) return;
            if (lootPopupScreen != null)
            {
                Destroy(lootPopupScreen.gameObject);
                lootPopupScreen = null;
            }
            GameObject screen = new GameObject("Loot Popup Screen");
            screen.transform.SetParent(transform, false);
            LootPopupScreen created = screen.AddComponent<LootPopupScreen>();
            try
            {
                created.Bind(new LootPopupBindings
                {
                    View = BuildLootPopupView,
                    Dismiss = DismissLootPopup,
                    QuickEquip = EquipLootToBestFit,
                    ReviewInventory = ReviewLootInInventory
                });
                created.SetVisible(false);
                lootPopupScreen = created;
            }
            catch
            {
                created.SetVisible(false);
                screen.SetActive(false);
                Destroy(screen);
                throw;
            }
        }

        private void SyncLootPopupScreen()
        {
            bool visible = state != null
                && CurrentUiOverlay() == UiOverlay.Loot
                && !ShouldShowStartupSplash();
            if (visible && (lootPopupScreen == null || !lootPopupScreen.IsReady))
            {
                TryInitializePresentationScreen("Loot popup recovery", EnsureLootPopupScreen, false);
            }
            if (lootPopupScreen == null)
            {
                if (visible) RecoverUnavailableOverlay(UiOverlay.Loot, "Loot panel");
                return;
            }
            if (!visible)
            {
                lootPopupScreen.SetVisible(false);
                return;
            }

            bool refresh = !lootPopupScreen.HasRenderableGeometry
                || ShouldRefreshPresentation(ref lastLootPopupRefreshKey, LootPopupRefreshKey());
            if (refresh)
            {
                lootPopupScreen.SetVisible(false);
                try
                {
                    lootPopupScreen.Refresh();
                    lootPopupScreen.SetVisible(true);
                    Canvas.ForceUpdateCanvases();
                }
                catch (System.Exception ex)
                {
                    lootPopupScreen.SetVisible(false);
                    Debug.LogException(new System.InvalidOperationException(VersionInfo.ProductName + " loot popup refresh failed.", ex));
                }
            }
            if (!lootPopupScreen.HasRenderableGeometry)
            {
                lootPopupScreen.SetVisible(false);
                RecoverUnavailableOverlay(UiOverlay.Loot, "Loot panel");
            }
        }

        private string LootPopupRefreshKey()
        {
            int secondsRemaining = lootPanelRequiresDismissal
                ? -1
                : Mathf.CeilToInt(Mathf.Max(0f, lootPanelUntil - Time.time));
            return "loot=" + (lootPanelItem?.DisplayName ?? "").GetHashCode()
                + ":" + (lootPanelBody ?? "").GetHashCode()
                + ":" + (lootPanelEquipNote ?? "").GetHashCode()
                + ":" + (lootPanelItem?.EquippedById ?? "").GetHashCode()
                + ":" + lootPanelGold
                + ":" + lootPanelSupplies
                + ":" + lootPanelElixirs
                + ":" + secondsRemaining;
        }

        private LootPopupView BuildLootPopupView()
        {
            EnsureInventoryEquipmentLinks();
            float remaining = lootPanelRequiresDismissal ? 0f : Mathf.Max(0f, lootPanelUntil - Time.time);
            InventoryItem item = lootPanelItem;
            bool hasItem = item != null;
            bool equippable = InventoryEquipmentRules.IsEquippable(item);
            PartyMember owner = equippable ? EquippedMember(item) : null;
            PartyMember best = null;
            int bestComparisonScore = 0;
            if (equippable && owner == null)
            {
                best = BestLootInventoryFit(item, out _, out bestComparisonScore);
            }
            Texture2D iconTexture = null;
            Rect iconUv = default;
            if (hasItem) TryGetInventoryItemIcon(item, out iconTexture, out iconUv);
            bool canReview = hasItem && state?.Inventory != null && state.Inventory.Contains(item);
            bool canQuickEquip = canReview && equippable && owner == null && best != null;
            string comparison = !hasItem
                ? ""
                : !equippable
                    ? "A quest item, not equipment. It will not replace anyone's loadout."
                    : owner != null
                        ? $"{InventoryEquipmentRules.SlotLabel(item.Slot, item.Form)} is active on {owner.Name}."
                        : best == null
                            ? "Stored safely with the party's other gear."
                            : $"Best fit: {best.Name}  •  {InventoryGradeLabelFor(item, best)}  •  {InventoryComparisonLine(item, best)}";
            return new LootPopupView
            {
                Visible = state != null
                    && !string.IsNullOrEmpty(lootPanelBody)
                    && (lootPanelRequiresDismissal || remaining > 0f),
                HasItem = hasItem,
                CanReview = canReview,
                CanQuickEquip = canQuickEquip,
                Title = lootPanelTitle,
                ItemName = hasItem ? item.DisplayName : "Victory spoils",
                ItemType = hasItem ? InventoryEquipmentRules.SlotLabel(item.Slot, item.Form) : "",
                Rarity = hasItem ? InventoryEquipmentRules.RarityLabel(item.Rarity) : "",
                TraitLine = lootPanelTraitLine,
                EquipNote = lootPanelEquipNote,
                Outcome = !hasItem
                    ? "Added to company stores"
                    : !equippable
                        ? "Quest item secured"
                        : owner != null ? $"Equipped by {owner.Name}" : "Added to inventory",
                Comparison = comparison,
                ReviewActionLabel = !hasItem
                    ? ""
                    : !equippable
                    ? "View in inventory"
                    : owner != null ? "Review or reassign" : canQuickEquip ? "Compare others" : "Review inventory",
                QuickEquipActionLabel = canQuickEquip ? "Equip to " + best.Name : "",
                IconLabel = LootIconLabel(item),
                AccentHex = hasItem ? InventoryRarityAccent(item.Rarity) : "#d7a84e",
                IconTexture = iconTexture,
                IconUv = iconUv,
                Gold = lootPanelGold,
                Supplies = lootPanelSupplies,
                Elixirs = lootPanelElixirs,
                SecondsRemaining = remaining
            };
        }

        private void DismissLootPopup()
        {
            SuppressBoardPointer();
            DismissLootPopupSilently();
            SyncLootPopupScreen();
            PlaySfx("uiclose", 0.35f);
        }

        private void EquipLootToBestFit()
        {
            InventoryItem item = lootPanelItem;
            if (state == null
                || item == null
                || state.Inventory == null
                || !state.Inventory.Contains(item)
                || !InventoryEquipmentRules.IsEquippable(item))
            {
                return;
            }

            SuppressBoardPointer();
            EnsureInventoryEquipmentLinks();
            PartyMember owner = EquippedMember(item);
            PartyMember best = owner == null ? BestLootInventoryFit(item, out _, out _) : null;
            string result = string.Empty;
            bool equipped = best != null && EquipInventoryItemToMember(item, best, out result);
            if (best == null)
            {
                result = owner == null
                    ? "No adventurer can equip this item right now."
                    : $"{owner.Name} already has {item.DisplayName} equipped.";
            }

            lootPanelEquipNote = result;
            if (equipped) AutosaveCheckpoint("equipment changed");
            PushLog(result, equipped ? Tone.Good : Tone.Warn);
            MarkUiDirty();
            SyncLootPopupScreen();
            PlaySfx(equipped ? "itemequip" : "blocked", 0.55f);
        }

        private void ReviewLootInInventory()
        {
            if (state == null) return;
            int inventoryIndex = state.Inventory == null || lootPanelItem == null
                ? -1
                : state.Inventory.IndexOf(lootPanelItem);
            SuppressBoardPointer();
            DismissLootPopupSilently();
            if (inventoryIndex >= 0) armorySelectedInventoryIndex = inventoryIndex;
            armoryPackFilter = 0;
            armoryInventoryTargetPickerOpen = false;
            armoryTab = (int)ArmoryTab.Pack;
            showArmory = true;
            MarkUiDirty();
            SyncLootPopupScreen();
            SyncArmoryOverlayScreen();
            PlaySfx("uitab", 0.48f);
        }

        private static string LootIconLabel(InventoryItem item)
        {
            if (item == null) return "SPOILS";
            string slot = (item.Slot ?? "").Trim().ToLowerInvariant();
            string form = ((item.Form ?? "") + " " + (item.DisplayName ?? "")).Trim().ToLowerInvariant();
            if (InventoryEquipmentRules.IsWeaponSlot(slot, form)) return "WPN";
            if (form.Contains("shield")) return "SHD";
            if (form.Contains("focus") || form.Contains("scepter") || form.Contains("staff")) return "FOC";
            if (form.Contains("robe")) return "ROB";
            if (slot.Contains("armor")) return "ARM";
            return "ITEM";
        }
    }
}
