using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls.Editor
{
    // Native production loot views over inactive, save-blocked disposable games.
    // No acquisition, equipment, or checkpoint callback is invoked by captures.
    public static class LootExperienceCapture
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        private const BindingFlags StaticPrivate = BindingFlags.Static | BindingFlags.NonPublic;

        public static void Capture()
        {
            int exit = 1;
            Texture2D remembrance = null, honest = null, ordinary = null;
            EventSystem previous = EventSystem.current;
            EventSystem[] previousSystems = UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            EventSystem system = null;
            bool registered = false;
            try
            {
                system = UiRuntime.EnsureEventSystemReady();
                if (EventSystem.current == null) { Invoke(system, "OnEnable"); registered = true; }
                EventSystem.current = system;
                remembrance = LoadAtlas(RuntimeArtManifest.RemembranceItemAtlas);
                honest = LoadAtlas(RuntimeArtManifest.HonestWeightItemAtlas);
                string artRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Docs", "ArtReferences"));
                string inventoryPath = Directory.GetFiles(artRoot, "item-inventory-atlas-runtime-*.png")
                    .OrderByDescending(path => Path.GetFileName(path), StringComparer.Ordinal).FirstOrDefault();
                Require(inventoryPath != null, "approved inventory atlas is present");
                ordinary = LoadAtlas(Path.GetFileName(inventoryPath));
                string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "QA", "loot-experience",
                    DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff")));
                Directory.CreateDirectory(directory);
                var report = new StringBuilder("Native loot receipt captures. Production BuildLootPopupView and original art.\n"
                    + "Inactive disposable fixtures; visualSmokeSaveBlocked=true; no live save reads or writes.\n"
                    + "Actual acquisition and checkpoint behavior are covered separately by real-player smoke.\n");
                foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
                foreach (string scenario in new[] { "ordinary-equipment", "travelers-knot", "measurekeeper-jack", "resources-only" })
                    CaptureReward(directory, scenario, size, ordinary, remembrance, honest, report);
                Require(Directory.GetFiles(directory, "*.png").Length == 12, "all 12 requested images exist");
                File.WriteAllText(Path.Combine(directory, "text-fit-and-validation.txt"), report.ToString());
                Debug.Log("LOOT EXPERIENCE CAPTURES PASSED: " + directory);
                exit = 0;
            }
            catch (Exception ex) { Debug.LogError("LOOT EXPERIENCE CAPTURES FAILED: " + ex); }
            finally
            {
                if (ordinary != null) UnityEngine.Object.DestroyImmediate(ordinary);
                if (remembrance != null) UnityEngine.Object.DestroyImmediate(remembrance);
                if (honest != null) UnityEngine.Object.DestroyImmediate(honest);
                if (system != null)
                {
                    if (!system.alreadySelecting) system.SetSelectedGameObject(null);
                    if (registered) Invoke(system, "OnDisable");
                    if (!previousSystems.Contains(system)) UnityEngine.Object.DestroyImmediate(system.gameObject);
                }
                if (previous != null) EventSystem.current = previous;
            }
            EditorApplication.Exit(exit);
        }

        private static void CaptureReward(string directory, string scenario, Vector2Int size,
            Texture2D ordinary, Texture2D remembrance, Texture2D honest, StringBuilder report)
        {
            GameObject host = RemembranceItemSmoke.NewHost("Loot capture save-blocked game");
            GameObject presentation = new GameObject("Loot receipt native capture");
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                GameState state = (GameState)typeof(RemembranceContentCapture).GetMethod("NewState", StaticPrivate).Invoke(null, null);
                Set(game, "state", state);
                Set(game, "rng", new System.Random(2370));
                Set(game, "visualSmokeSaveBlocked", true);
                Set(game, "launchError", "Loot capture fixture presentation held");
                Set(game, "itemIconAtlas", ordinary);
                Set(game, "itemIconAtlasUsesInventoryContract", true);
                Set(game, "remembranceItemAtlas", remembrance);
                Set(game, "honestWeightItemAtlas", honest);
                InventoryItem item = scenario == "travelers-knot" ? SignatureItemCatalog.CreateTravelersKnot()
                    : scenario == "measurekeeper-jack" ? SignatureItemCatalog.CreateMeasurekeeperJack()
                    : scenario == "ordinary-equipment" ? new InventoryItem
                    {
                        DisplayName = "+2 iron broadsword", Slot = "weapon", Form = "broadsword", Material = "iron",
                        Rarity = "fine", Bonus = 2, DamageMin = 4, DamageMax = 7, AttackSpeed = 8, DamageType = "physical"
                    } : null;
                if (item != null) Invoke(game, "AddInventoryItem", item);
                bool resources = item == null || scenario == "ordinary-equipment";
                Set(game, "lootPanelItem", item);
                Set(game, "lootPanelBody", item?.DisplayName ?? "Victory rewards");
                Set(game, "lootPanelTitle", scenario == "travelers-knot" ? RemembranceQuestRules.Title
                    : scenario == "measurekeeper-jack" ? HonestWeightQuestRules.Title : "Victory spoils");
                Set(game, "lootPanelRequiresDismissal", true);
                Set(game, "lootPanelGold", resources ? 127 : 0);
                Set(game, "lootPanelSupplies", resources ? 2 : 0);
                Set(game, "lootPanelElixirs", resources ? 1 : 0);
                Set(game, "lootPanelTraitLine", item == null ? "" : Invoke(game, "ItemTraitLine", item));
                Set(game, "lootPanelEquipNote", scenario == "travelers-knot" ? "Edda's keepsake. Added to the pack for deliberate equipping."
                    : scenario == "measurekeeper-jack" ? "Maud's workshop reward. Added to the pack for deliberate equipping."
                    : item == null ? "The company stores are updated." : Invoke(game, "AutoEquipItem", item));
                int gold = state.Gold, supplies = state.Supplies, elixirs = state.Elixirs, itemCount = state.Inventory.Count;
                LootPopupView view = (LootPopupView)Invoke(game, "BuildLootPopupView");
                Require(view.Visible && view.HasItem == (item != null), scenario + " uses the production reward adapter");
                Require(item == null || (view.IconTexture != null && view.CanReview), scenario + " uses actual artwork and exact-item review");
                Require(view.Gold == (resources ? 127 : 0) && view.Supplies == (resources ? 2 : 0) && view.Elixirs == (resources ? 1 : 0),
                    scenario + " preserves exact reward totals");
                LootPopupScreen screen = presentation.AddComponent<LootPopupScreen>();
                screen.Bind(new LootPopupBindings { View = () => view });
                screen.Refresh(); screen.SetVisible(true);
                Require(screen.HasDefaultFocusForTest, scenario + " defaults to safe Continue");
                Canvas canvas = Field<Canvas>(screen, "canvas");
                Action layout = () =>
                {
                    typeof(LootPopupScreen).GetMethod("ApplyLayout", Private, null, new[] { typeof(float), typeof(float) }, null)
                        .Invoke(screen, new object[] { (float)size.x, (float)size.y });
                    Canvas.ForceUpdateCanvases();
                    Require(LootPopupLayout.Calculate(size.x, size.y, view.HasItem, LootRewardPresentationRules.HasResources(view)).Fits(size.x, size.y),
                        scenario + " element geometry fits " + size);
                    ValidateTextFits(canvas, scenario + " " + size, report);
                };
                typeof(UiFinishCapture).GetMethod("Render", StaticPrivate).Invoke(null, new object[] { directory, scenario, size, canvas, layout });
                VerifyDimensions(Path.Combine(directory, scenario + "-" + size.x + "x" + size.y + ".png"), size);
                Require(state.Gold == gold && state.Supplies == supplies && state.Elixirs == elixirs && state.Inventory.Count == itemCount,
                    scenario + " presentation cannot mutate stored rewards");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(presentation);
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static void ValidateTextFits(Canvas canvas, string context, StringBuilder report)
        {
            foreach (Text text in canvas.GetComponentsInChildren<Text>(false))
            {
                if (!text.enabled || string.IsNullOrWhiteSpace(text.text)) continue;
                Rect area = text.rectTransform.rect;
                Require(area.width > 0f && area.height > 0f, context + " visible text has an allocated area: " + text.name);
                TextGenerationSettings settings = text.GetGenerationSettings(area.size);
                settings.resizeTextForBestFit = false;
                settings.fontSize = text.resizeTextForBestFit ? text.resizeTextMinSize : text.fontSize;
                settings.verticalOverflow = VerticalWrapMode.Overflow;
                float height = text.cachedTextGeneratorForLayout.GetPreferredHeight(text.text, settings) / text.pixelsPerUnit;
                Require(height <= area.height + .5f,
                    context + " text must fit without truncation: " + text.name + " needs=" + height + " available=" + area.height + " text=" + text.text);
                report.AppendLine(context + " | " + text.transform.parent.name + "/" + text.name + " | font=" + settings.fontSize
                    + " | textHeight=" + height.ToString("0.##") + " | available=" + area.height.ToString("0.##"));
            }
        }

        private static void VerifyDimensions(string path, Vector2Int expected)
        {
            Texture2D image = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            try { Require(image.LoadImage(File.ReadAllBytes(path)) && image.width == expected.x && image.height == expected.y, "PNG dimensions match " + path); }
            finally { UnityEngine.Object.DestroyImmediate(image); }
        }

        private static Texture2D LoadAtlas(string filename)
        {
            Texture2D image = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            Require(image.LoadImage(File.ReadAllBytes(Path.Combine(Application.dataPath, "..", "Docs", "ArtReferences", filename))), "atlas decodes: " + filename);
            image.filterMode = FilterMode.Bilinear;
            return image;
        }

        private static T Field<T>(object owner, string name) => (T)owner.GetType().GetField(name, Private).GetValue(owner);
        private static object Invoke(object owner, string name, params object[] args) => RemembranceItemSmoke.Invoke(owner, name, args);
        private static void Set(object owner, string name, object value) => RemembranceItemSmoke.Set(owner, name, value);
        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Loot experience capture: " + message);
        }
    }
}
