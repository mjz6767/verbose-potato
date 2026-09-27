using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AshenHalls.Editor
{
    // Native production adapters over disposable campaign states. No saves are
    // loaded, and no acceptance, encounter, reward or checkpoint is triggered.
    public static class MeteorCrownContentCapture
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Capture()
        {
            int exitCode = 1;
            Texture2D portraits = null;
            EventSystem previousSystem = EventSystem.current;
            EventSystem[] previousSystems = UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            EventSystem fixtureSystem = null;
            bool registered = false;
            try
            {
                fixtureSystem = UiRuntime.EnsureEventSystemReady();
                if (EventSystem.current == null)
                {
                    Invoke(fixtureSystem, "OnEnable");
                    registered = true;
                }
                EventSystem.current = fixtureSystem;
                portraits = LoadPortraits();
                string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "QA", "meteor-crown-content",
                    DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff")));
                Directory.CreateDirectory(directory);
                int captures = 0;
                foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1280, 720) })
                {
                    captures += CaptureConversation(directory, portraits, size, "yara-final-road", 0, "");
                    captures += CaptureConversation(directory, portraits, size, "yara-assault-plan", 0, "plan");
                    captures += CaptureConversation(directory, portraits, size, "yara-fear", 0, "afraid");
                    captures += CaptureConversation(directory, portraits, size, "yara-return-marks", 1, "");
                    captures += CaptureConversation(directory, portraits, size, "yara-route", 1, "route");
                    captures += CaptureConversation(directory, portraits, size, "yara-heart-broken", 3, "");
                    captures += CaptureConversation(directory, portraits, size, "yara-homecoming", 4, "");
                    captures += CaptureConversation(directory, portraits, size, "yara-road-after", 4, "home");
                    captures += CaptureConversation(directory, portraits, size, "vhal-rakh-confrontation", 3, "confront");
                    CaptureJournal(directory, size);
                    CaptureVictory(directory, size);
                    captures += 2;
                }
                Require(Directory.GetFiles(directory, "*.png").Length == captures, "every native finale content capture exists");
                File.WriteAllText(Path.Combine(directory, "README.txt"),
                    "Native production Meteor Crown dialogue, Journal, and Victory adapters over disposable fixtures.\n"
                    + "Every page of Yara's offer, assault plan, fear, return guidance, broken-heart response, homecoming, and Vhal Rakh's confrontation is captured.\n"
                    + "The Journal is scrolled to Vhal Rakh's current objective after the ritual heart is broken.\n"
                    + "The ending uses a completed campaign fixture and the actual EndState adapter.\n"
                    + "Both 960x600 and 1280x720 use native requested-size layout methods.\n"
                    + "No campaign saves were read or written; these captures are not proof of a full campaign playthrough. Visual review is required.\n");
                Debug.Log("METEOR CROWN CONTENT CAPTURES PASSED: " + directory);
                exitCode = 0;
            }
            catch (Exception exception) { Debug.LogError("METEOR CROWN CONTENT CAPTURES FAILED: " + exception); }
            finally
            {
                if (portraits != null) UnityEngine.Object.DestroyImmediate(portraits);
                if (fixtureSystem != null)
                {
                    if (!fixtureSystem.alreadySelecting) fixtureSystem.SetSelectedGameObject(null);
                    if (registered) Invoke(fixtureSystem, "OnDisable");
                    if (!previousSystems.Contains(fixtureSystem)) UnityEngine.Object.DestroyImmediate(fixtureSystem.gameObject);
                }
                if (previousSystem != null) EventSystem.current = previousSystem;
            }
            EditorApplication.Exit(exitCode);
        }

        private static int CaptureConversation(string directory, Texture2D portraits, Vector2Int size, string name, int progress, string response)
        {
            GameObject fixture = RemembranceItemSmoke.NewHost("Meteor Crown production conversation fixture");
            GameObject presentation = new GameObject("Meteor Crown native dialogue capture");
            try
            {
                AshenHallsGame game = fixture.AddComponent<AshenHallsGame>();
                GameState state = NewState(progress);
                BindFixture(game, state);
                Set(game, "npcPortraitAtlas", portraits);
                if (response == "confront")
                {
                    state.Depth = 6;
                    WorldMapSite site = new WorldMapSite(MeteorCrownQuestRules.SovereignSite, "salt-cisterns", "Salt Cistern Gate", "", 4, 4, 1, ObjectType.DungeonGate);
                    Require((bool)Invoke(game, "TryResolveMeteorCrownStorySite", site), "the real last threshold opens the confrontation before combat");
                    Require(state.Mode == GameMode.Explore && state.Combat == null, "capturing the confrontation does not start a battle");
                }
                else
                {
                    Require((bool)Invoke(game, "TryShowMeteorCrownConversation"), "production Yara finale conversation is available");
                    if (!string.IsNullOrEmpty(response)) Invoke(game, "ResolveMeteorCrownConversation", response);
                }
                DialogueScreen screen = presentation.AddComponent<DialogueScreen>();
                screen.Bind(new DialogueScreenBindings { View = () => (DialogueScreenView)Invoke(game, "BuildDialogueScreenView") });
                int pages = Field<string[]>(game, "dialoguePages").Length;
                Require(pages > 0, name + " has actual dialogue pages");
                for (int page = 0; page < pages; page++)
                {
                    DialogueScreenView view = (DialogueScreenView)Invoke(game, "BuildDialogueScreenView");
                    Require(response == "confront"
                        ? view.Speaker == "Vhal Rakh" && view.PortraitTexture == null
                        : view.PortraitTexture == portraits,
                        name + " uses the speaker's proper portrait identity rather than another NPC's face");
                    if (page < pages - 1) Require(view.Choices.Length == 0, name + " holds choices until its final page");
                    else Require(view.Choices.Length > 0 || !string.IsNullOrEmpty(view.ContinueLabel),
                        name + " provides its final choices or return action");
                    screen.SetVisible(true);
                    screen.Refresh();
                    Render(directory, name + "-page" + (page + 1), size, Field<Canvas>(screen, "canvas"), () =>
                    {
                        Layout(screen, new[] { typeof(int), typeof(float), typeof(float) }, view.Choices.Length, (float)size.x, (float)size.y);
                        Invoke(screen, "UpdateBodyContentLayout");
                    });
                    if (page + 1 < pages) Invoke(game, "AdvanceDialogue");
                }
                return pages;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(presentation);
                UnityEngine.Object.DestroyImmediate(fixture);
            }
        }

        private static void CaptureJournal(string directory, Vector2Int size)
        {
            GameObject fixture = RemembranceItemSmoke.NewHost("Meteor Crown production Journal fixture");
            GameObject presentation = new GameObject("Meteor Crown native Journal capture");
            try
            {
                AshenHallsGame game = fixture.AddComponent<AshenHallsGame>();
                GameState state = NewState(3);
                state.Depth = 6;
                BindFixture(game, state);
                Invoke(game, "RepairMeteorCrownStoryObjective", true);
                Set(game, "showArmory", true);
                Type tabType = typeof(AshenHallsGame).GetNestedType("ArmoryTab", BindingFlags.NonPublic);
                Require(tabType != null, "the production Journal tab exists");
                Set(game, "armoryTab", Convert.ToInt32(Enum.Parse(tabType, "Journal")));
                ArmoryOverlayView view = (ArmoryOverlayView)Invoke(game, "BuildArmoryOverlayView");
                int row = view.Rows.ToList().FindIndex(entry => entry.Title == "The Last Dawn - Vhal Rakh");
                Require(row >= 0 && view.Rows[row].Subtitle == "current", "the Journal exposes the actual last campaign objective");
                ArmoryOverlayScreen screen = presentation.AddComponent<ArmoryOverlayScreen>();
                screen.Bind(new ArmoryOverlayBindings { View = () => view });
                screen.SetVisible(true);
                screen.Refresh();
                Render(directory, "journal-last-dawn", size, Field<Canvas>(screen, "canvas"), () =>
                {
                    Layout(screen, new[] { typeof(bool), typeof(int), typeof(bool), typeof(int), typeof(float), typeof(float) },
                        view.Detail != null, view.Filters.Count, view.Detail?.ExtendedSummary ?? false, view.Detail?.Actions.Count ?? 0,
                        (float)size.x, (float)size.y);
                    screen.ScrollRowIntoViewForTest(row);
                });
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(presentation);
                UnityEngine.Object.DestroyImmediate(fixture);
            }
        }

        private static void CaptureVictory(string directory, Vector2Int size)
        {
            GameObject fixture = RemembranceItemSmoke.NewHost("Meteor Crown production ending fixture");
            GameObject presentation = new GameObject("Meteor Crown native ending capture");
            try
            {
                AshenHallsGame game = fixture.AddComponent<AshenHallsGame>();
                GameState state = NewState(4);
                state.Mode = GameMode.Victory;
                state.Depth = 6;
                BindFixture(game, state);
                EndStateView view = (EndStateView)Invoke(game, "BuildEndStateView");
                Require(view.Victory && view.RouteRows.Any(row => row.Contains("Meteor Crown")),
                    "the real ending adapter recognizes the completed campaign");
                EndStateScreen screen = presentation.AddComponent<EndStateScreen>();
                screen.Bind(new EndStateScreenBindings { View = () => view });
                screen.SetVisible(true);
                screen.Refresh();
                Render(directory, "meteor-crown-victory", size, Field<Canvas>(screen, "canvas"), () =>
                    Layout(screen, new[] { typeof(float), typeof(float) }, (float)size.x, (float)size.y));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(presentation);
                UnityEngine.Object.DestroyImmediate(fixture);
            }
        }

        private static GameState NewState(int progress)
        {
            GameState state = new GameState
            {
                Mode = GameMode.Explore, Depth = 1, StoryChapter = 6, Seed = 2330,
                Gold = 346, Supplies = 7, Elixirs = 3, ReducedMotion = true, SfxMuted = true, MusicMuted = true
            };
            state.StoryFlags.AddRange(new[]
            {
                StoryFlags.MidgaardKingMet, StoryFlags.SewerContractAccepted,
                StoryFlags.SewerBrokenSluiceCleared, StoryFlags.SewerFoulRunoffCleared, StoryFlags.SewerCisternDenCleared,
                StoryFlags.SewerRewardClaimed, StoryFlags.OldRoadTeaserUnlocked, StoryFlags.KoboldKingDefeated,
                StoryFlags.BoneRoadEntered, StoryFlags.BoneRoadWatchDefeated, StoryFlags.GloamRitualBroken,
                StoryFlags.GloamWardenDefeated, StoryFlags.RedGateWarningRecovered, StoryFlags.GlassAndAshFrontierSurveyed,
                StoryFlags.GlassAndAshExpeditionAccepted, StoryFlags.GlassAndAshEntered,
                StoryFlags.GlasswardAmbushDefeated, StoryFlags.GlassIndexRecovered, StoryFlags.EmberglassGateKeyRecovered,
                StoryFlags.GlassAndAshDebriefed, StoryFlags.RedGateAssaultAccepted, StoryFlags.RedGateEntered,
                StoryFlags.RedGateVanguardDefeated, StoryFlags.OssuaryRoadSealRecovered,
                StoryFlags.CrownroadMarshalDefeated, StoryFlags.MeteorCrownThresholdSurveyed, StoryFlags.RedGateDebriefed
            });
            if (progress >= 1) state.StoryFlags.AddRange(new[] { StoryFlags.MeteorCrownAssaultAccepted, StoryFlags.MeteorCrownEntered });
            if (progress >= 2) state.StoryFlags.Add(StoryFlags.MeteorCrownSentriesDefeated);
            if (progress >= 3) state.StoryFlags.Add(StoryFlags.MeteorCrownHeartBroken);
            if (progress >= 4) state.StoryFlags.Add(StoryFlags.MeteorCrownDefeated);
            foreach (StarterHeroDef hero in StarterPartyCatalog.All)
            {
                state.Party.Add(new PartyMember
                {
                    Id = "finale-capture-" + hero.ClassKey, Name = hero.Name, Race = hero.Race, ClassKey = hero.ClassKey,
                    Role = hero.Role, Stats = hero.Stats, Skills = hero.CreateSkills(), Spell = hero.Spell,
                    Level = 16, Hp = 54, MaxHp = 68, Mana = 12, MaxMana = 30, Range = hero.Range,
                    WeaponName = hero.Role == "bow" ? "plain longbow" : "plain steel sword",
                    ArmorName = "plain traveling coat", DamageMin = 8, DamageMax = 12, AttackSpeed = 8,
                    SpriteColor = "#82a081"
                });
            }
            return state;
        }

        private static void BindFixture(AshenHallsGame game, GameState state)
        {
            Set(game, "state", state);
            Set(game, "rng", new System.Random(state.Seed));
            Set(game, "activeContentSet", ContentSetCatalog.SewerSlice);
            Set(game, "visualSmokeSaveBlocked", true);
            Set(game, "launchError", "Meteor Crown content capture fixture presentation held");
        }

        private static Texture2D LoadPortraits()
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            Require(texture.LoadImage(File.ReadAllBytes(Path.Combine(Application.dataPath, "..", "Docs", "ArtReferences", RuntimeArtManifest.NpcPortraitAtlas))),
                "the actual NPC portrait atlas decodes");
            texture.filterMode = FilterMode.Bilinear;
            return texture;
        }

        private static void Render(string directory, string name, Vector2Int size, Canvas canvas, Action layout)
        {
            MethodInfo render = typeof(UiFinishCapture).GetMethod("Render", BindingFlags.Static | BindingFlags.NonPublic);
            Require(render != null, "shared native canvas renderer exists");
            render.Invoke(null, new object[] { directory, name, size, canvas, layout });
        }

        private static void Layout(object screen, Type[] signature, params object[] arguments)
        {
            MethodInfo method = screen.GetType().GetMethod("ApplyLayout", Private, null, signature, null);
            Require(method != null, "native layout overload exists for " + screen.GetType().Name);
            method.Invoke(screen, arguments);
        }

        private static T Field<T>(object owner, string name) => (T)owner.GetType().GetField(name, Private).GetValue(owner);
        private static object Invoke(object owner, string name, params object[] arguments) => RemembranceItemSmoke.Invoke(owner, name, arguments);
        private static void Set(object owner, string name, object value) => RemembranceItemSmoke.Set(owner, name, value);
        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Meteor Crown content capture: " + message);
        }
    }
}
