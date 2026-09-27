using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AshenHalls.Editor
{
    // Disposable fixtures render the real adapters and native screens, without reading or writing a campaign save.
    public static class SteelAndStudyContentCapture
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly StringBuilder Evidence = new StringBuilder();

        public static void Capture()
        {
            int exit = 1;
            Texture2D portraits = null, unique = null, honest = null;
            EventSystem previous = EventSystem.current;
            EventSystem[] systems = UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            EventSystem system = null;
            bool registered = false;
            try
            {
                Evidence.Clear();
                system = UiRuntime.EnsureEventSystemReady();
                if (EventSystem.current == null) { Invoke(system, "OnEnable"); registered = true; }
                EventSystem.current = system;
                portraits = LoadAtlas(RuntimeArtManifest.NpcPortraitAtlas);
                unique = LoadAtlas(RuntimeArtManifest.UniqueItemAtlas);
                honest = LoadAtlas(RuntimeArtManifest.HonestWeightItemAtlas);
                string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "QA", "steel-and-study-content", DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff")));
                Directory.CreateDirectory(directory);
                int count = 0;
                foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1280, 720) })
                {
                    foreach (string scenario in new[] { "mage-fallback", "mage-cap-finish", "priest-before-threshold", "priest-threshold", "paladin-maximum", "warrior-mixed" })
                    { CaptureGrowth(directory, size, scenario); count++; }
                    foreach (string scenario in new[] { "mage-knife", "mage-focus", "priest-focus" })
                    { CaptureWeapon(directory, size, scenario, unique, honest); count++; }
                    foreach (bool bow in new[] { false, true }) { CaptureRanger(directory, size, bow); count++; }
                    foreach (bool restoreBow in new[] { false, true }) { CaptureRangerComparison(directory, size, restoreBow, unique); count++; }
                    foreach (string scenario in new[] { "root", "forms", "range" }) count += CaptureTessa(directory, size, scenario, portraits);
                }
                Require(Directory.GetFiles(directory, "*.png").Length == count, "every requested native image exists");
                File.WriteAllText(Path.Combine(directory, "RENDERED_TEXT.txt"), Evidence.ToString());
                File.WriteAllText(Path.Combine(directory, "README.txt"),
                    "Native production Growth, Pack, Skillbook and Tessa views at 960x600 and 1280x720.\n"
                    + "Six Growth states: capped Ember mage with Arms available; exact 98-to-99 one-point finish; Priest Guard16-to18; Priest Guard28-to30; Paladin Guard88-to90; Warrior mixed Strength/Arms preview. Staging uses the production row action and never applies to a live save.\n"
                    + "Actual equipped items: mage Counterweight Knife (melee), mage Stormglass Conductor (range4), priest starter prayer focus (range4). Their actual basic attack forecasts, skill and blocked-line-of-sight result are recorded in RENDERED_TEXT.txt; Pack renders equipment text and art.\n"
                    + "Ranger Skillbook selects Broadhead with a sword (blocked) and longbow (ready); Tessa root, forms and range guidance include every page and the final choice footer.\n"
                    + "Ranger Pack target lists compare a real equipped longbow with unowned Crownward Warblade, then compare that equipped sword with the previous bow, exposing actual per-member range changes before confirmation.\n"
                    + "These are disposable rendering fixtures, not connected playthrough evidence. Manual visual review remains required.\n");
                Debug.Log("STEEL AND STUDY CONTENT CAPTURES PASSED: " + directory); exit = 0;
            }
            catch (Exception exception) { Debug.LogError("STEEL AND STUDY CONTENT CAPTURES FAILED: " + exception); }
            finally
            {
                foreach (Texture2D texture in new[] { portraits, unique, honest }) if (texture != null) UnityEngine.Object.DestroyImmediate(texture);
                if (system != null)
                {
                    if (!system.alreadySelecting) system.SetSelectedGameObject(null);
                    if (registered) Invoke(system, "OnDisable");
                    if (!systems.Contains(system)) UnityEngine.Object.DestroyImmediate(system.gameObject);
                }
                if (previous != null) EventSystem.current = previous;
            }
            EditorApplication.Exit(exit);
        }

        private static void CaptureGrowth(string directory, Vector2Int size, string scenario)
        {
            string cls = scenario.StartsWith("mage", StringComparison.Ordinal) ? "mage"
                : scenario.StartsWith("priest", StringComparison.Ordinal) ? "priest"
                : scenario.StartsWith("paladin", StringComparison.Ordinal) ? "paladin" : "warrior";
            using (Fixture f = new Fixture(cls))
            {
                PartyMember member = f.Member;
                member.SkillPoints = cls == "mage" ? 12 : 4; member.StatPoints = 3;
                PartyGrowthChoice selected = cls == "mage" ? PartyGrowthChoice.Arms
                    : cls == "warrior" ? PartyGrowthChoice.Strength : PartyGrowthChoice.Guard;
                if (cls == "mage") { member.Skills.Ember = 99; member.Skills.Arms = scenario == "mage-cap-finish" ? 98 : 8; }
                if (cls == "priest") member.Skills.Guard = scenario == "priest-threshold" ? 28 : 16;
                if (cls == "paladin") member.Skills.Guard = 88;
                if (cls == "warrior") member.Skills.Arms = 42;
                f.Call("RecalculateMember", member);
                f.Set("armorySelectedPartyIndex", f.MemberIndex);
                f.OpenNativeGrowth();
                if (scenario != "mage-fallback")
                    f.Call("RunArmoryGrowthRowAction", 20000 + (int)selected);
                if (cls == "warrior") f.Call("RunArmoryGrowthRowAction", 20000 + (int)PartyGrowthChoice.Arms);
                f.Set("armoryGrowthSelectedChoiceKey", 20000 + (int)selected);
                ArmoryOverlayView view = (ArmoryOverlayView)f.Call("BuildArmoryOverlayView");
                Require(view.Detail != null && view.Rows.Any(row => row.Key == 20000 + (int)selected), "Growth has the selected legal class choice");
                if (scenario == "mage-fallback") Require(view.Rows.Single(row => row.Title == "Arms").ActionEnabled && !view.Rows.Single(row => row.Title == "Ember").ActionEnabled, "capped mage exposes actual Arms training");
                if (scenario == "mage-cap-finish") Require(view.Detail.Summary.Contains("Arms +1") && view.Detail.Summary.Contains("98 -> 99"), "cap preview discloses one rank");
                if (scenario == "priest-before-threshold") Require(view.Detail.Summary.Contains("Guard action +0 -> +0") && view.Detail.Summary.Contains("next at 30 (12 ranks)"), "pre-threshold Guard preview stays truthful");
                if (scenario == "priest-threshold") Require(view.Detail.Summary.Contains("Guard action +0 -> +1"), "Guard tier crossing is visible");
                if (scenario == "paladin-maximum") Require(view.Detail.Summary.Contains("Guard action +2 -> +3"), "maximum Guard tier crossing is visible");
                CaptureArmory(directory, "growth-" + scenario, size, view, 20000 + (int)selected);
            }
        }

        private static void CaptureWeapon(string directory, Vector2Int size, string scenario, Texture2D unique, Texture2D honest)
        {
            using (Fixture f = new Fixture(scenario == "priest-focus" ? "priest" : "mage"))
            {
                f.Set("uniqueItemAtlas", unique); f.Set("honestWeightItemAtlas", honest);
                InventoryItem item = scenario == "mage-knife" ? SignatureItemCatalog.CreateCounterweightKnife()
                    : scenario == "mage-focus" ? SignatureItemCatalog.CreateStormglassConductor()
                    : new InventoryItem { Slot = "weapon", Form = "prayer focus", DisplayName = "plain prayer focus", Material = "wood", Trait = "plain",
                        DamageMin = 2, DamageMax = 4, AttackSpeed = 8, DamageType = "light" };
                f.Equip(item);
                Require(f.Member.Range == (scenario == "mage-knife" ? 1 : 4), "equipped weapon has the real form range");
                f.MakeCombat();
                f.Enemy.X = scenario == "mage-knife" ? 3 : 6;
                string skill = (string)f.Call("AttackSkillName", f.Hero, f.Enemy);
                Require(skill == "arms", "caster physical/focus basic attack practices Arms");
                CombatAttackForecast forecast = (CombatAttackForecast)f.Call("AttackForecast", f.Hero, f.Enemy);
                Require(forecast.Legal && !forecast.Ranged, "basic attack is legal with Arms at the actual item range");
                Evidence.AppendLine(scenario + " " + size + " actual basic attack: " + f.Call("AttackPreview", f.Hero, f.Enemy) + " / skill " + skill);
                if (scenario != "mage-knife")
                {
                    f.State.Combat.Obstacles.Add(new Point(3, 2, "stone", 0) { Integrity = 3 });
                    forecast = (CombatAttackForecast)f.Call("AttackForecast", f.Hero, f.Enemy);
                    Require(!forecast.Legal && forecast.BlockReason == AttackForecastBlockReason.LineOfSight, "focus basic attack needs clear sight");
                    Evidence.AppendLine("Blocked sight: " + f.Call("AttackPreview", f.Hero, f.Enemy));
                }
                f.State.Combat = null; f.State.Mode = GameMode.Explore;
                f.Set("showArmory", true); SetTab(f.Game, "Pack"); f.Call("SelectArmoryInventoryItem", item);
                ArmoryOverlayView view = (ArmoryOverlayView)f.Call("BuildArmoryOverlayView");
                Require(view.Detail != null, "equipped item has a native detail view");
                if (scenario != "priest-focus") Require(view.Detail.IconTexture == (scenario == "mage-knife" ? honest : unique), "signature item uses its approved actual atlas");
                CaptureArmory(directory, scenario + "-pack", size, view, null);
            }
        }

        private static void CaptureRanger(string directory, Vector2Int size, bool bow)
        {
            using (Fixture f = new Fixture("ranger"))
            {
                f.Equip(new InventoryItem { Slot = "weapon", Form = bow ? "longbow" : "broadsword", DisplayName = bow ? "plain longbow" : "plain broadsword",
                    Material = "iron", Trait = "plain", DamageMin = 3, DamageMax = 7, AttackSpeed = 8, DamageType = "physical" });
                f.MakeCombat(); f.Enemy.X = 5;
                f.Set("showAbilityPanel", true); f.Set("showSpellbook", false); f.Call("SelectCombatAbilityModalCard", "broadheadshot");
                CombatAbilityModalView view = (CombatAbilityModalView)f.Call("BuildCombatAbilityModalView");
                CombatAbilityModalCardView card = view.Cards.Single(value => value.Id == "broadheadshot");
                Require(card.Usable == bow, "ranger skillbook gates Broadhead on a missile weapon");
                if (!bow) Require(card.DisabledReason.Contains("Equip a bow"), "blocked card explains the required weapon");
                GameObject presentation = new GameObject("Steel and study native Skillbook");
                try
                {
                    CombatAbilityModalScreen screen = presentation.AddComponent<CombatAbilityModalScreen>();
                    screen.Bind(new CombatAbilityModalBindings { View = () => (CombatAbilityModalView)f.Call("BuildCombatAbilityModalView") });
                    screen.SetVisible(true); screen.Refresh();
                    screen.SetFilterForTest(CombatAbilityModalFilter.All);
                    f.Call("SelectCombatAbilityModalCard", "broadheadshot"); screen.Refresh();
                    Require(screen.DetailIdForTest == "broadheadshot" && screen.DetailActionInteractableForTest == bow,
                        "native Broadhead detail and activation match the live equipment requirement");
                    string name = "ranger-broadhead-" + (bow ? "longbow-ready" : "sword-blocked");
                    Evidence.AppendLine(name + " " + size + "\n" + JsonUtility.ToJson(card, true));
                    Render(directory, name, size, Field<Canvas>(screen, "canvas"), () => Layout(screen, new[] { typeof(float), typeof(float) }, (float)size.x, (float)size.y));
                }
                finally { UnityEngine.Object.DestroyImmediate(presentation); }
            }
        }

        private static int CaptureTessa(string directory, Vector2Int size, string scenario, Texture2D portraits)
        {
            using (Fixture f = new Fixture("warrior"))
            {
                f.Set("npcPortraitAtlas", portraits);
                f.Call("ShowTessaConversation", new object[] { null });
                if (scenario != "root") { Choose(f.Game, "advice"); Choose(f.Game, scenario); }
                f.Set("dialoguePageIndex", 0);
                GameObject presentation = new GameObject("Steel and study native Tessa dialogue");
                try
                {
                    DialogueScreen screen = presentation.AddComponent<DialogueScreen>();
                    screen.Bind(new DialogueScreenBindings { View = () => (DialogueScreenView)f.Call("BuildDialogueScreenView") });
                    int pages = f.Get<string[]>("dialoguePages").Length;
                    Require(pages > 0, "Tessa guidance has authored pages");
                    for (int page = 0; page < pages; page++)
                    {
                        DialogueScreenView view = (DialogueScreenView)f.Call("BuildDialogueScreenView");
                        Require(view.PortraitTexture == portraits && view.Choices.Length <= 4, "Tessa resolves her real portrait and all choices");
                        screen.SetVisible(true); screen.Refresh();
                        string name = "tessa-" + scenario + "-page" + (page + 1);
                        Evidence.AppendLine(name + " " + size + "\n" + JsonUtility.ToJson(view, true));
                        Render(directory, name, size, Field<Canvas>(screen, "canvas"), () =>
                        {
                            Layout(screen, new[] { typeof(int), typeof(float), typeof(float) }, view.Choices.Length, (float)size.x, (float)size.y);
                            Invoke(screen, "UpdateBodyContentLayout");
                        });
                        if (page + 1 < pages) f.Call("AdvanceDialogue");
                    }
                    return pages;
                }
                finally { UnityEngine.Object.DestroyImmediate(presentation); }
            }
        }

        private static void CaptureRangerComparison(string directory, Vector2Int size, bool restoreBow, Texture2D unique)
        {
            using (Fixture f = new Fixture("ranger"))
            {
                f.Set("uniqueItemAtlas", unique);
                InventoryItem bow = new InventoryItem { Slot = "weapon", Form = "longbow", DisplayName = "plain longbow",
                    Material = "ashwood", Trait = "plain", DamageMin = 3, DamageMax = 7, AttackSpeed = 8, DamageType = "physical" };
                f.Equip(bow);
                InventoryItem sword = SignatureItemCatalog.CreateCrownwardWarblade();
                f.Call("AddInventoryItem", sword);
                if (restoreBow)
                {
                    object[] args = { sword, f.Member, "" };
                    Require((bool)f.Call("EquipInventoryItemToMember", args), "Cairn actually equips the sword before reviewing his previous bow: " + args[2]);
                }
                InventoryItem selected = restoreBow ? bow : sword;
                Require(f.Member.Range == (restoreBow ? 1 : 5), "comparison starts from the actual current range");
                f.Set("showArmory", true); SetTab(f.Game, "Pack"); f.Call("SelectArmoryInventoryItem", selected);
                f.Set("armoryInventoryTargetPickerOpen", true);
                ArmoryOverlayView view = (ArmoryOverlayView)f.Call("BuildArmoryOverlayView");
                ArmoryDetailActionView cairn = view.Detail.Actions.Single(action => action.Key == f.MemberIndex);
                Require(cairn.Detail.Contains(restoreBow ? "Range 1" : "Range 5"), "Cairn's member-specific range transition is visible before equipping: " + cairn.Detail);
                Require(cairn.Detail.Contains(restoreBow ? "shots enabled" : "shots disabled"), "the compact comparison discloses the current shooting capability");
                CaptureArmory(directory, restoreBow ? "ranger-restore-longbow-pack-comparison" : "ranger-crownward-pack-comparison", size, view, null);
            }
        }

        private static void CaptureArmory(string directory, string name, Vector2Int size, ArmoryOverlayView view, int? selectedKey)
        {
            GameObject presentation = new GameObject("Steel and study native Armory");
            try
            {
                ArmoryOverlayScreen screen = presentation.AddComponent<ArmoryOverlayScreen>();
                screen.Bind(new ArmoryOverlayBindings { View = () => view }); screen.SetVisible(true); screen.Refresh();
                Evidence.AppendLine(name + " " + size + "\n" + view.Detail.Title + "\n" + view.Detail.Summary);
                foreach (ArmoryRowView row in view.Rows) Evidence.AppendLine(row.Title + " | " + row.Subtitle + " | " + row.Detail);
                foreach (ArmoryDetailActionView action in view.Detail.Actions) Evidence.AppendLine(action.Label + " | " + action.Detail);
                Render(directory, name, size, Field<Canvas>(screen, "canvas"), () =>
                {
                    Layout(screen, new[] { typeof(bool), typeof(int), typeof(bool), typeof(int), typeof(float), typeof(float) },
                        view.Detail != null, view.Filters.Count, view.Detail?.ExtendedSummary ?? false, view.Detail?.Actions.Count ?? 0, (float)size.x, (float)size.y);
                    if (selectedKey.HasValue) screen.ScrollRowIntoViewForTest(view.Rows.ToList().FindIndex(row => row.Key == selectedKey.Value));
                });
            }
            finally { UnityEngine.Object.DestroyImmediate(presentation); }
        }

        private sealed class Fixture : IDisposable
        {
            private readonly GameObject host = RemembranceItemSmoke.NewHost("Steel and study fixture");
            public readonly AshenHallsGame Game;
            private ArmoryOverlayScreen detachedArmory;
            public readonly int MemberIndex;
            public GameState State => Get<GameState>("state");
            public PartyMember Member => State.Party[MemberIndex];
            public CombatUnit Hero => State.Combat.Units[0];
            public CombatUnit Enemy => State.Combat.Units[1];

            public Fixture(string cls)
            {
                Game = host.AddComponent<AshenHallsGame>();
                GameState state = new GameState { SaveVersion = VersionInfo.SaveVersion, ContentSetId = ContentSetCatalog.SewerSlice, Mode = GameMode.Explore,
                    Depth = 1, StoryChapter = 3, Seed = 2360, Gold = 168, Supplies = 5, Elixirs = 2, ReducedMotion = true, SfxMuted = true, MusicMuted = true };
                Set("state", state); Set("activeContentSet", ContentSetCatalog.SewerSlice); Set("rng", new System.Random(state.Seed));
                Set("visualSmokeSaveBlocked", true); Set("launchError", "Steel and study fixture presentation held");
                state.Party = (List<PartyMember>)Call("MakeDefaultParty");
                MemberIndex = state.Party.FindIndex(member => member.ClassKey == cls);
                if (MemberIndex < 0)
                {
                    MemberIndex = 0;
                    state.Party[0] = (PartyMember)Call("MakeHero", "Maer", "human", cls, (string)Call("RoleForClass", cls),
                        new Stats(18, 18, 18, 18), StarterPartyCatalog.SpellSchoolForClass(cls), 1,
                        new SkillSet { Arms = 18, Missile = 8, Ember = 8, Mend = 18, Hex = 8, Guard = 16 }.Normalize());
                }
                foreach (PartyMember member in state.Party)
                {
                    member.Level = 15; member.Stats = new Stats(18, 18, 18, 18);
                    Call("RecalculateMember", member); member.Hp = member.MaxHp; member.Mana = member.MaxMana;
                }
                state.StoryFlags.AddRange(new[] { StoryFlags.MidgaardGrandHearthDeparted, StoryFlags.MidgaardKingMet, StoryFlags.MidgaardRatQuestGiven,
                    StoryFlags.SewerContractAccepted, StoryFlags.SewerBrokenSluiceCleared, StoryFlags.SewerFoulRunoffCleared, StoryFlags.SewerCisternDenCleared,
                    StoryFlags.SewerSafeRoomChoiceClaimed, StoryFlags.SewerSafeRoomFocusChosen, StoryFlags.SewerRewardClaimed,
                    StoryFlags.MidgaardRatPeltArmorMade, StoryFlags.OldRoadTeaserUnlocked, StoryFlags.KoboldAmbushSurvived, StoryFlags.KoboldCaveCleared, StoryFlags.KoboldKingDefeated });
                state.Map = (MapData)Call("GenerateMap", 1, state.Seed);
                Call("EnsureWorldLandmarks"); Call("PlacePlayerAtExplorationStart");
            }

            public void Equip(InventoryItem item)
            {
                Call("AddInventoryItem", item);
                object[] args = { item, Member, "" };
                Require((bool)Call("EquipInventoryItemToMember", args), "actual item equipment assignment: " + args[2]);
            }

            public void OpenNativeGrowth()
            {
                // Real bindings and staging callbacks; only the native screen is activated, so the game never boots or loads a save.
                Set("launchError", ""); Call("EnsureArmoryOverlayScreen");
                detachedArmory = Get<ArmoryOverlayScreen>("armoryOverlayScreen");
                detachedArmory.transform.SetParent(null, false); detachedArmory.gameObject.SetActive(true);
                typeof(AshenHallsGame).GetMethod("ToggleArmory", Private, null, new[] { typeof(int) }, null).Invoke(Game, new object[] { 4 });
                Require(detachedArmory.IsVisible && detachedArmory.HasRenderableGeometry, "real native Growth owns its visible canvas before staging");
            }

            public void MakeCombat()
            {
                PartyMember member = Member;
                CombatUnit hero = new CombatUnit { Id = member.Id, Name = member.Name, Side = UnitSide.Party, PartyIndex = MemberIndex,
                    Role = member.Role, ClassKey = member.ClassKey, Race = member.Race, Level = member.Level, X = 2, Y = 2,
                    Hp = member.Hp, MaxHp = member.MaxHp, Mana = member.Mana, MaxMana = member.MaxMana, Spell = member.Spell, Skills = member.Skills.Clone(),
                    WeaponName = member.WeaponName, Range = member.Range, WeaponBonus = member.WeaponBonus, DamageType = member.WeaponDamageType,
                    DamageMin = member.DamageMin, DamageMax = member.DamageMax, AttackSpeed = member.AttackSpeed, Agility = member.Agility,
                    Movement = member.Movement, Power = member.Power };
                CombatUnit enemy = new CombatUnit { Id = "steel-target", Name = "Road marauder", Side = UnitSide.Enemy, PartyIndex = -1,
                    Role = "raider", Level = 8, Hp = 500, MaxHp = 500, X = 3, Y = 2, Range = 1, Movement = 3, Skills = new SkillSet().Normalize() };
                State.Mode = GameMode.Combat;
                State.Combat = new CombatState { Round = 1, EncounterStyle = "rat-sluice", ActiveId = hero.Id, ActionAvailable = true,
                    Phase = CombatPhase.ChooseAction, MovePoints = hero.Movement, Units = new List<CombatUnit> { hero, enemy }, InitiativeQueue = new List<string> { hero.Id, enemy.Id } };
            }

            public object Call(string name, params object[] args) => Invoke(Game, name, args);
            public T Get<T>(string name) => Field<T>(Game, name);
            public void Set(string name, object value) => SteelAndStudyContentCapture.Set(Game, name, value);
            public void Dispose()
            {
                if (detachedArmory != null) UnityEngine.Object.DestroyImmediate(detachedArmory.gameObject);
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static void Choose(AshenHallsGame game, string id)
        {
            while (Field<int>(game, "dialoguePageIndex") < (Field<string[]>(game, "dialoguePages")?.Length ?? 0) - 1) Invoke(game, "AdvanceDialogue");
            DialogueScreenView view = (DialogueScreenView)Invoke(game, "BuildDialogueScreenView");
            Require(view.Choices.Length <= 4 && view.Choices.Any(choice => choice.Id == id && choice.Enabled), "visible enabled guidance choice " + id);
            typeof(AshenHallsGame).GetMethod("ChooseDialogueChoice", Private, null, new[] { typeof(string) }, null).Invoke(game, new object[] { id });
        }
        private static void SetTab(AshenHallsGame game, string name) => Set(game, "armoryTab", Convert.ToInt32(Enum.Parse(typeof(AshenHallsGame).GetNestedType("ArmoryTab", BindingFlags.NonPublic), name)));
        private static Texture2D LoadAtlas(string filename)
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            Require(texture.LoadImage(File.ReadAllBytes(Path.Combine(Application.dataPath, "..", "Docs", "ArtReferences", filename))), filename + " decodes");
            texture.filterMode = FilterMode.Bilinear; return texture;
        }
        private static void Render(string directory, string name, Vector2Int size, Canvas canvas, Action layout) => typeof(UiFinishCapture).GetMethod("Render", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { directory, name, size, canvas, layout });
        private static void Layout(object screen, Type[] signature, params object[] args) => screen.GetType().GetMethod("ApplyLayout", Private, null, signature, null).Invoke(screen, args);
        private static T Field<T>(object owner, string name) => (T)owner.GetType().GetField(name, Private).GetValue(owner);
        private static object Invoke(object owner, string name, params object[] args) => RemembranceItemSmoke.Invoke(owner, name, args);
        private static void Set(object owner, string name, object value) => RemembranceItemSmoke.Set(owner, name, value);
        private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException("Steel and study content: " + message); }
    }
}
