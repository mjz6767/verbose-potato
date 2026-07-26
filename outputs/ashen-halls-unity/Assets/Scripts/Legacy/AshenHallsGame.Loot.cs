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
            PartyMember owner = EquippedMember(item);
            PartyMember best = BestInventoryFit(item, out _, out int comparisonScore);
            TryGetInventoryItemIcon(item, out Texture2D iconTexture, out Rect iconUv);
            bool canReview = item != null && state?.Inventory != null && state.Inventory.Contains(item);
            string comparison = owner != null
                ? $"{InventoryEquipmentRules.SlotLabel(item.Slot, item.Form)} is now active on {owner.Name}."
                : best == null
                    ? "Stored safely with the party's other gear."
                    : $"Best fit: {best.Name}  •  {InventoryEquipmentRules.GradeLabel(InventoryEquipmentRules.Grade(comparisonScore))}  •  {InventoryComparisonLine(item, best)}";
            return new LootPopupView
            {
                Visible = state != null
                    && item != null
                    && !string.IsNullOrEmpty(lootPanelBody)
                    && (lootPanelRequiresDismissal || remaining > 0f),
                CanReview = canReview,
                Title = lootPanelTitle,
                ItemName = item == null ? "" : item.DisplayName,
                ItemType = item == null ? "" : InventoryEquipmentRules.SlotLabel(item.Slot, item.Form),
                Rarity = item == null ? "" : InventoryEquipmentRules.RarityLabel(item.Rarity),
                TraitLine = lootPanelTraitLine,
                EquipNote = lootPanelEquipNote,
                Outcome = owner != null ? $"Equipped by {owner.Name}" : "Added to inventory",
                Comparison = comparison,
                IconLabel = LootIconLabel(item),
                AccentHex = item == null ? "#d7a84e" : InventoryRarityAccent(item.Rarity),
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

        private void ReviewLootInInventory()
        {
            if (state == null) return;
            int inventoryIndex = state.Inventory == null || lootPanelItem == null
                ? -1
                : state.Inventory.IndexOf(lootPanelItem);
            SuppressBoardPointer();
            DismissLootPopupSilently();
            if (inventoryIndex >= 0) armorySelectedInventoryIndex = inventoryIndex;
            armoryTab = (int)ArmoryTab.Pack;
            showArmory = true;
            MarkUiDirty();
            SyncLootPopupScreen();
            SyncArmoryOverlayScreen();
            PlaySfx("uitab", 0.48f);
        }

        private static string LootIconLabel(InventoryItem item)
        {
            if (item == null) return "LOOT";
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
