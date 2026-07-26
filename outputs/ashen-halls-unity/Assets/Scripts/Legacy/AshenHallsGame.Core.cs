using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private const int ExploreW = WorldMapGenerationRules.Width;

        private const int ExploreH = WorldMapGenerationRules.Height;

        private const int ExploreViewportW = 11;

        private const int ExploreViewportH = 7;

        private const int ExploreWideViewportW = 17;

        private const int ExploreWideViewportH = 9;

        private const int ExploreRevealRadius = 8;

        private const int CombatW = 12;

        private const int CombatH = 8;

        private const int SaveVersion = VersionInfo.SaveVersion;

        private const int PartySize = 4;

        private const int StatPointBudget = 50;

        private const int CombatMoveAllowance = 3;

        private const int UnreachableMoveCost = 999;

        private const int SummonedTreeDuration = FormulaCatalog.SummonedTreeDuration;

        private const int BasePactSummonBurden = 3;

        private const int FinalBossDepth = 6;

        private const string PackageVersion = VersionInfo.PackageVersion;

        private const string GameTitle = VersionInfo.ProductName;

        private const string GameSubtitle = "The Old Road";

        private const string BuildStage = VersionInfo.BuildStage;

        private const string HomeTownName = "Midgaard";

        private readonly Color bg = Hex("0e1114");

        private readonly Color panel = Hex("1a2026");

        private readonly Color panel2 = Hex("20272e");

        private readonly Color ink = Hex("f3ead7");

        private readonly Color muted = Hex("b7aa90");

        private readonly Color line = Hex("3c4544");

        private readonly Color gold = Hex("d7a84e");

        private readonly Color ember = Hex("c65c3b");

        private readonly Color teal = Hex("58b7a5");

        private readonly Color blood = Hex("b94b56");

        private readonly Color moss = Hex("7f9d5b");

        private readonly Color frost = Hex("9ad6e8");

        private readonly Color poison = Hex("8fc27b");

        private readonly Color violet = Hex("8d6dcc");

        private readonly Color stone = Hex("46504d");

        private readonly Color floorA = Hex("3a3329");

        private readonly Color floorB = Hex("463828");

        private readonly Color retroBlack = Hex("050708");

        private readonly Color cursorWhite = Hex("f5f1df");

        private GameState state;

        private System.Random rng;

        private ExplorationController explorationController;

        private GameState explorationControllerState;

        private MapData explorationControllerMap;

        private CombatController combatController;

        private GameState combatControllerState;

        private CombatState combatControllerCombat;

        private Rect boardRect;

        private Rect sideRect;

        private Vector2 logScroll;

        private int selectedBuilderIndex;

        private int armoryTab;

        private ActionMode selectedAction = ActionMode.Attack;

        private string pendingFormulaCode = "";

        private string pendingAbilityId = "";

        private string spellbookSelectedCode = "";

        private string abilitySelectedId = "";

        private readonly Dictionary<string, string> combatAbilityBrowseSelections =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private float aiActAt = -1f;

        private string bannerText = "";

        private float bannerUntil;

        private CombatPowerIdentity combatPowerCue;

        private float combatPowerCueStarted;

        private float combatPowerCueUntil;

        private float combatPowerCueImpactAt;

        private Texture2D combatPowerCueTexture;

        private Rect combatPowerCueSource;

        private string combatPowerOutcomeText = "";

        private float combatPowerOutcomeVisibleAt;

        private float combatPowerPulseUntil;

        private bool combatAdvancePending;

        private float combatAdvanceAt = -1f;

        private string combatAdvanceUnitId = "";

        private bool combatAdvanceStartsReservedTurn;

        private string combatResolutionLabel = "";

        private float splashStartedAt;

        private bool splashClockStarted;

        private string launchStatus = "Lighting the old road...";

        private string launchError = "";

        private string lootPanelTitle = "";

        private string lootPanelBody = "";

        private string lootPanelTraitLine = "";

        private string lootPanelEquipNote = "";

        private InventoryItem lootPanelItem;

        private int lootPanelGold;

        private int lootPanelSupplies;

        private int lootPanelElixirs;

        private float lootPanelUntil;

        private bool lootPanelRequiresDismissal;

        private InventoryItem queuedDialogueLootItem;

        private string queuedDialogueLootTitle = "";

        private string queuedDialogueLootEquipNote = "";

        private int queuedDialogueLootGold;

        private int queuedDialogueLootSupplies;

        private int queuedDialogueLootElixirs;

        private string lastExploreRegion = "";

        private bool showArmory;

        private bool showPauseMenu;

        private bool showHelpOverlay;

        private bool pauseSettingsOpen;

        private string pauseConfirmAction = "";

        private bool showTavernSettings;

        private bool showTavernTesting;

        private bool showSpellbook;

        private bool showAbilityPanel;

        private Vector2 combatModalFallbackScroll;

        private bool combatTimelineExpanded;

        private bool showDialogue;

        private Vector2 dialogueFallbackScroll;

        private int dialogueOpenedFrame = -1;

        // uGUI closes overlays during EventSystem.Update, before the legacy IMGUI
        // board sees the same mouse event. Hold board input briefly so that click
        // cannot leak through to movement or targeting underneath the overlay.
        private int suppressBoardPointerThroughFrame = -1;

        private bool exploreWideView;

        private bool exploreHudCollapsed = true;

        private bool showExploreArtDebug;

        private string exploreHoverLookLine = "";

        private int exploreFacingX;

        private int exploreFacingY = -1;

        private string dialogueTitle = "";

        private string dialogueSpeaker = "";

        private string dialogueBody = "";

        private string[] dialoguePages = Array.Empty<string>();

        private int dialoguePageIndex;

        private ObjectType dialogueFocus = ObjectType.Town;

        private Color dialogueAccentColor;

        private DialogueChoiceView[] dialogueChoices = Array.Empty<DialogueChoiceView>();

        private Action<string> dialogueChoiceHandler;

        private DialogueChoiceView[] dialogueTopicChoices = Array.Empty<DialogueChoiceView>();

        private Action<string> dialogueTopicChoiceHandler;

        private Action dialogueReturnToTopics;

        private bool dialogueShowingResponse;

        private bool betaLabMode;

        private bool labSaveBlocked;

        private string activeContentSet = ContentSetCatalog.SewerSlice;

        private int uiRevision = 1;

        private bool cachedHasSavedGame;

        private bool hasSavedGameCacheValid;

        private string lastTavernRefreshKey = "";

        private string lastPartySetupRefreshKey = "";

        private string lastExplorationHudRefreshKey = "";

        private string lastCombatHudRefreshKey = "";

        private string lastCombatAbilityModalRefreshKey = "";

        private string lastDialogueRefreshKey = "";

        private string lastLootPopupRefreshKey = "";

        private string lastArmoryRefreshKey = "";

        private string lastPauseMenuRefreshKey = "";

        private string lastHelpOverlayRefreshKey = "";

        private string lastEndStateRefreshKey = "";

        private readonly List<ExplorationHudPartyMemberView> explorationHudPartyBuffer = new List<ExplorationHudPartyMemberView>(4);

        private readonly List<ExplorationHudLogView> explorationHudLogBuffer = new List<ExplorationHudLogView>(3);

        private readonly List<CombatHudCommandView> combatHudCommandBuffer = new List<CombatHudCommandView>(6);

        private readonly List<CombatHudLogView> combatHudLogBuffer = new List<CombatHudLogView>(5);

        private readonly List<CombatHudTurnView> combatHudTurnBuffer = new List<CombatHudTurnView>(6);

        private string lastSfxKey = "";

        private float lastSfxAt = -10f;

        private float lastSfxVolume;

        private bool contentValidated;

        private int floatTextSerial;

        private TavernScreen tavernScreen;

        private PartySetupScreen partySetupScreen;

        private ExplorationHudScreen explorationHudScreen;

        private CombatHudScreen combatHudScreen;

        private CombatAbilityModalScreen combatAbilityModalScreen;

        private DialogueScreen dialogueScreen;

        private LootPopupScreen lootPopupScreen;

        private ArmoryOverlayScreen armoryOverlayScreen;

        private PauseMenuScreen pauseMenuScreen;

        private HelpOverlayScreen helpOverlayScreen;

        private EndStateScreen endStateScreen;

        private BannerToastScreen bannerToastScreen;

        private static readonly string[] roleOrder =
        {
            "shield", "pike", "bow", "knife", "mender", "ember", "hex", "ward"
        };

        private enum ArmoryTab
        {
            Party = 0,
            Pack = 1,
            Spells = 2,
            Journal = 3
        }

        private void InvalidateControllerCaches()
        {
            InvalidateExplorationController();
            InvalidateCombatController();
        }

        private void InvalidateExplorationController()
        {
            explorationController = null;
            explorationControllerState = null;
            explorationControllerMap = null;
        }

        private void InvalidateCombatController()
        {
            combatController = null;
            combatControllerState = null;
            combatControllerCombat = null;
        }

        private void Awake()
        {
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            bool visualCaptureRequested = commandLineArgs.Any(arg => string.Equals(arg, "-ashen-capture", StringComparison.OrdinalIgnoreCase));
            if (visualCaptureRequested)
            {
                Application.runInBackground = true;
            }
            Debug.Log(VersionInfo.ProductName + " boot start " + VersionInfo.PackageVersion + " / save " + VersionInfo.SaveVersion + ".");
            splashStartedAt = Time.realtimeSinceStartup;
            Application.targetFrameRate = 60;
            Screen.fullScreen = false;
            Screen.fullScreenMode = FullScreenMode.Windowed;
            int displayW = Screen.currentResolution.width > 0 ? Screen.currentResolution.width : 1920;
            int displayH = Screen.currentResolution.height > 0 ? Screen.currentResolution.height : 1080;
            int targetW = displayW >= 2048 && displayH >= 1152 ? 2048 : displayW >= 1920 && displayH >= 1080 ? 1920 : displayW >= 1280 ? Mathf.Min(displayW, 1600) : displayW;
            int targetH = displayW >= 2048 && displayH >= 1152 ? 1152 : displayW >= 1920 && displayH >= 1080 ? 1080 : displayH >= 720 ? Mathf.Min(displayH, 900) : displayH;
            int minimumW = Mathf.Min(1280, displayW);
            int minimumH = Mathf.Min(720, displayH);
            int maximumW = visualCaptureRequested ? Mathf.Max(displayW, 4096) : displayW;
            int maximumH = visualCaptureRequested ? Mathf.Max(displayH, 2160) : displayH;
            targetW = CommandLineDimension(commandLineArgs, "-screen-width", targetW, minimumW, maximumW);
            targetH = CommandLineDimension(commandLineArgs, "-screen-height", targetH, minimumH, maximumH);
            if (Mathf.Abs(Screen.width - targetW) > 24 || Mathf.Abs(Screen.height - targetH) > 24) Screen.SetResolution(targetW, targetH, false);

            try
            {
                EnsurePixel();
                LoadExternalArt();
                EnsureAudioListener();
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0f;
                audioSource.ignoreListenerPause = true;
                audioSource.priority = 32;
                EnsureSfxVoicePool();
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.playOnAwake = false;
                musicSource.loop = true;
                musicSource.spatialBlend = 0f;
                musicSource.ignoreListenerPause = true;
                musicSource.priority = 64;
                musicFadeSource = gameObject.AddComponent<AudioSource>();
                musicFadeSource.playOnAwake = false;
                musicFadeSource.loop = true;
                musicFadeSource.spatialBlend = 0f;
                musicFadeSource.ignoreListenerPause = true;
                musicFadeSource.priority = 65;
                BuildSoundClips();
                BuildMusicClips();
                ValidateContentCatalogs();
                musicSource.clip = tavernMusicClip;
                NewMuster();
                ApplyAudioSettings();
                InitializePresentationScreens();
                ApplyVisualSmokeLaunchMode(commandLineArgs);
                RequestVisualSmokeCaptureIfNeeded(commandLineArgs);
                launchStatus = "Muster ready.";
                Debug.Log(VersionInfo.ProductName + " boot complete: " + launchStatus);
                RequestBatchmodeQuitAfterBootIfNeeded();
            }
            catch (Exception ex)
            {
                launchError = ex.Message;
                Debug.LogException(ex);
                RequestBatchmodeQuitAfterBootIfNeeded();
            }
        }

        private void RequestBatchmodeQuitAfterBootIfNeeded()
        {
            if (!ShouldAutoQuitAfterBoot(Environment.GetCommandLineArgs(), Application.isBatchMode, Application.isEditor))
            {
                return;
            }

            Debug.Log(VersionInfo.ProductName + " batchmode quit requested after boot.");
            Application.Quit();
        }

        private void ApplyVisualSmokeLaunchMode(string[] args)
        {
            if (args == null || args.Length == 0) return;
            ApplyVisualSmokeSeed(args);
            if (args.Any(arg => string.Equals(arg, "-ashen-gate-smoke", StringComparison.OrdinalIgnoreCase)))
            {
                QuickStart();
                ApplyVisualSmokeExploreView(args);
                if (!PositionVisualSmokeAtGate(args))
                {
                    throw new InvalidOperationException("Visual smoke could not stage a Midgaard gate close-up.");
                }
                bannerText = "";
                bannerUntil = 0f;
                Debug.Log(VersionInfo.ProductName + " visual smoke mode: Midgaard gate close-up.");
                return;
            }
            bool dialogueResponseSmoke = args.Any(arg => string.Equals(arg, "-ashen-dialogue-response-smoke", StringComparison.OrdinalIgnoreCase));
            if (dialogueResponseSmoke || args.Any(arg => string.Equals(arg, "-ashen-dialogue-smoke", StringComparison.OrdinalIgnoreCase)))
            {
                QuickStart();
                if (!TryOpenVisualSmokeNpcDialogue(args, dialogueResponseSmoke))
                {
                    throw new InvalidOperationException("Visual smoke could not open a Midgaard NPC dialogue through the exploration interaction path.");
                }
                if (dialogueResponseSmoke)
                {
                    int responseChoice = Array.FindIndex(
                        dialogueChoices,
                        choice => choice != null
                            && choice.Enabled
                            && !string.Equals(choice.Id, "buy", StringComparison.OrdinalIgnoreCase));
                    ChooseDialogueChoice(responseChoice >= 0 ? responseChoice : 0);
                    if (CurrentUiOverlay() != UiOverlay.Dialogue)
                    {
                        throw new InvalidOperationException("Visual smoke could not advance to an NPC dialogue response.");
                    }
                }
                ValidateDialogueSmokeState(dialogueResponseSmoke);
                Debug.Log(VersionInfo.ProductName + (dialogueResponseSmoke
                    ? " visual smoke mode: NPC dialogue response."
                    : " visual smoke mode: NPC dialogue interaction."));
                return;
            }

            bool lootSmoke = args.Any(arg => string.Equals(arg, "-ashen-loot-smoke", StringComparison.OrdinalIgnoreCase));
            if (lootSmoke || args.Any(arg => string.Equals(arg, "-ashen-inventory-smoke", StringComparison.OrdinalIgnoreCase)))
            {
                StageInventoryVisualSmoke(lootSmoke);
                Debug.Log(VersionInfo.ProductName + (lootSmoke
                    ? " visual smoke mode: acquired loot."
                    : " visual smoke mode: inventory and equipment."));
                return;
            }

            if (args.Any(arg => string.Equals(arg, "-ashen-explore-smoke", StringComparison.OrdinalIgnoreCase)))
            {
                QuickStart();
                ApplyVisualSmokeExploreView(args);
                ValidateRoamingThreatSmokeState();
                Debug.Log(VersionInfo.ProductName + " visual smoke mode: exploration.");
                return;
            }

            if (args.Any(arg => string.Equals(arg, "-ashen-route-smoke", StringComparison.OrdinalIgnoreCase)))
            {
                StartKoboldRouteLab();
                ApplyVisualSmokeExploreView(args);
                Debug.Log(VersionInfo.ProductName + " visual smoke mode: route exploration.");
                return;
            }

            if (args.Any(arg => string.Equals(arg, "-ashen-feedback-smoke", StringComparison.OrdinalIgnoreCase)))
            {
                StartBetaCombatLab();
                StageVisualSmokeCombatFeedback();
                Debug.Log(VersionInfo.ProductName + " visual smoke mode: combat feedback.");
                return;
            }

            if (args.Any(arg => string.Equals(arg, "-ashen-spellbook-smoke", StringComparison.OrdinalIgnoreCase)))
            {
                StartBetaCombatLab();
                CombatUnit tester = CurrentUnit();
                PromoteMageTester(tester);
                ApplyVisualSmokeCombatBookState(args, tester, true);
                Debug.Log(VersionInfo.ProductName + " visual smoke mode: spellbook.");
                return;
            }

            if (args.Any(arg => string.Equals(arg, "-ashen-skills-smoke", StringComparison.OrdinalIgnoreCase)))
            {
                StartMartialCombatLab();
                CombatUnit tester = state?.Combat?.Units?
                    .FirstOrDefault(unit => unit.Side == UnitSide.Party
                        && unit.Hp > 0
                        && (unit.ClassKey == "warrior" || unit.ClassKey == "rogue" || unit.ClassKey == "ranger"));
                if (tester != null)
                {
                    state.Combat.ActiveId = tester.Id;
                    state.Combat.ActionAvailable = true;
                    state.Combat.Phase = CombatPhase.ChooseAction;
                    SelectOrRunAction(ActionMode.Ability, tester);
                    ApplyVisualSmokeCombatBookState(args, tester, false);
                }
                Debug.Log(VersionInfo.ProductName + " visual smoke mode: combat skills.");
                return;
            }

            if (args.Any(arg => string.Equals(arg, "-ashen-combat-smoke", StringComparison.OrdinalIgnoreCase)))
            {
                StartBetaCombatLab();
                Debug.Log(VersionInfo.ProductName + " visual smoke mode: combat.");
            }
        }

        private void ApplyVisualSmokeCombatBookState(string[] args, CombatUnit tester, bool spellbook)
        {
            if (args == null || tester == null || state?.Combat == null) return;
            bool future = args.Any(arg => string.Equals(arg, "-ashen-book-future", StringComparison.OrdinalIgnoreCase));
            bool bottom = args.Any(arg => string.Equals(arg, "-ashen-book-bottom", StringComparison.OrdinalIgnoreCase));
            bool unavailable = args.Any(arg => string.Equals(arg, "-ashen-book-unavailable", StringComparison.OrdinalIgnoreCase));
            bool armed = args.Any(arg => string.Equals(arg, "-ashen-book-armed", StringComparison.OrdinalIgnoreCase));

            if (future) tester.Level = 1;
            if (unavailable) state.Combat.ActionAvailable = false;
            if (armed)
            {
                state.Combat.ActionAvailable = true;
                if (spellbook)
                {
                    CombatAbilityModalCardView candidate = BuildFormulaModalCards(tester, true)
                        .Where(card => card != null
                            && card.Targeted
                            && CombatAbilityModalPresentationRules.CanActivate(card))
                        .OrderBy(card => string.Equals(card.Id, "FBL", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                        .FirstOrDefault();
                    if (candidate == null || !PrepareFormulaCode(tester, candidate.Id))
                    {
                        throw new InvalidOperationException("Visual smoke could not arm a legal targeted spell.");
                    }
                    SelectOrRunAction(ActionMode.Cast, tester);
                }
                else
                {
                    CombatAbilityModalCardView candidate = BuildSkillModalCards(tester, true)
                        .FirstOrDefault(card => card != null
                            && card.Targeted
                            && CombatAbilityModalPresentationRules.CanActivate(card));
                    if (candidate == null || !PrepareAbility(tester, candidate.Id))
                    {
                        throw new InvalidOperationException("Visual smoke could not arm a legal targeted skill.");
                    }
                    SelectOrRunAction(ActionMode.Ability, tester);
                }
            }

            SyncCombatAbilityModalScreen();
            CombatAbilityModalView view = BuildCombatAbilityModalView();
            if (armed && !(view.Cards ?? Array.Empty<CombatAbilityModalCardView>()).Any(card => card != null && card.Ready && card.Usable))
            {
                throw new InvalidOperationException("Visual smoke armed state did not reach the combat power book.");
            }
            CombatAbilityModalFilter filter = armed
                ? CombatAbilityModalFilter.Ready
                : future
                    ? CombatAbilityModalFilter.Future
                    : unavailable
                        ? CombatAbilityModalFilter.Learned
                        : bottom
                            ? CombatAbilityModalFilter.All
                            : CombatAbilityModalPresentationRules.InitialFilter(view.Cards);
            combatAbilityModalScreen?.SetFilterForTest(filter);

            IEnumerable<CombatAbilityModalCardView> visible = (view.Cards ?? Array.Empty<CombatAbilityModalCardView>())
                .Where(card => CombatAbilityModalPresentationRules.MatchesFilter(card, filter));
            CombatAbilityModalCardView selected = bottom || future
                ? visible.LastOrDefault()
                : visible.FirstOrDefault(card => card.Ready) ?? visible.FirstOrDefault();
            if (selected != null) PreviewCombatAbilityModalCard(selected.Id);
            MarkUiDirty();
        }

        private void ValidateDialogueSmokeState(bool responseMode)
        {
            if (CurrentUiOverlay() != UiOverlay.Dialogue || !showDialogue)
            {
                throw new InvalidOperationException("Dialogue smoke did not retain modal ownership.");
            }
            DialogueChoiceView[] topics = responseMode ? dialogueTopicChoices : dialogueChoices;
            if (topics == null || topics.Length < 3)
            {
                throw new InvalidOperationException("Dialogue smoke did not expose a complete topic list.");
            }
            if (responseMode && !dialogueShowingResponse)
            {
                throw new InvalidOperationException("Dialogue response smoke did not retain response state.");
            }
            if (responseMode && dialogueChoices != null && dialogueChoices.Length > 0)
            {
                throw new InvalidOperationException("Dialogue response smoke found topic buttons intruding into the answer view.");
            }
            Debug.Log($"{VersionInfo.ProductName} dialogue smoke: speaker={dialogueSpeaker}, topics={topics.Length}, response={dialogueShowingResponse}.");
        }

        private void ValidateRoamingThreatSmokeState()
        {
            List<RoamingThreat> threats = state?.RoamingThreats?
                .Where(threat => threat != null && threat.Active && threat.Depth == state.Depth)
                .ToList();
            if (threats == null || threats.Count != 2)
            {
                throw new InvalidOperationException($"Exploration smoke expected two active Midgaard patrols, found {threats?.Count ?? 0}.");
            }
            if (threats.Select(threat => threat.Id).Distinct().Count() != threats.Count)
            {
                throw new InvalidOperationException("Exploration smoke found duplicate roaming patrol identities.");
            }
            foreach (RoamingThreat threat in threats)
            {
                if (ZoneAt(threat.X, threat.Y)?.Danger <= 0)
                {
                    throw new InvalidOperationException($"{threat.Name} spawned inside a safe zone.");
                }
                if (ObjectAt(state.Map, threat.X, threat.Y) != null)
                {
                    throw new InvalidOperationException($"{threat.Name} overlaps a map object.");
                }
            }
            Debug.Log(VersionInfo.ProductName + " roaming threat smoke: " + string.Join(", ", threats.Select(threat => $"{threat.Name}@{threat.X},{threat.Y}")));
        }

        private void ApplyVisualSmokeSeed(string[] args)
        {
            if (state == null || args == null) return;
            int option = Array.FindIndex(args, arg => string.Equals(arg, "-ashen-seed", StringComparison.OrdinalIgnoreCase));
            if (option < 0 || option + 1 >= args.Length) return;
            if (int.TryParse(args[option + 1], out int seed)) state.Seed = seed;
        }

        private void ApplyVisualSmokeExploreView(string[] args)
        {
            exploreWideView = args != null && args.Any(arg => string.Equals(arg, "-ashen-region-smoke", StringComparison.OrdinalIgnoreCase));
        }

        private bool PositionVisualSmokeAtGate(string[] args)
        {
            if (state?.Map?.Objects == null) return false;
            string requested = "east";
            int option = Array.FindIndex(args, arg => string.Equals(arg, "-ashen-gate-smoke", StringComparison.OrdinalIgnoreCase));
            if (option >= 0 && option + 1 < args.Length && !args[option + 1].StartsWith("-", StringComparison.Ordinal))
            {
                requested = args[option + 1].Trim().ToLowerInvariant();
            }

            ObjectType type = requested == "west" ? ObjectType.WestGate
                : requested == "north" ? ObjectType.NorthGate
                : requested == "south" ? ObjectType.SouthGate
                : ObjectType.EastGate;
            MapObject gate = state.Map.Objects.FirstOrDefault(obj => obj != null && obj.Type == type);
            if (gate == null) return false;

            int inwardX = type == ObjectType.EastGate ? -1 : type == ObjectType.WestGate ? 1 : 0;
            int inwardY = type == ObjectType.NorthGate ? 1 : type == ObjectType.SouthGate ? -1 : 0;
            int[] preferredDistances = { 2, 1, 3 };
            for (int i = 0; i < preferredDistances.Length; i++)
            {
                int distance = preferredDistances[i];
                int x = gate.X + inwardX * distance;
                int y = gate.Y + inwardY * distance;
                if (!CanStepExplore(x, y)) continue;
                state.PlayerX = x;
                state.PlayerY = y;
                exploreFacingX = -inwardX;
                exploreFacingY = -inwardY;
                lastExploreRegion = ExploreRegionName(x, y);
                InvalidateExplorationController();
                MarkUiDirty();
                return true;
            }

            return false;
        }

        private bool TryOpenVisualSmokeNpcDialogue(string[] args, bool responseMode)
        {
            if (state?.Map?.Objects == null || state.Mode != GameMode.Explore) return false;
            string optionName = responseMode ? "-ashen-dialogue-response-smoke" : "-ashen-dialogue-smoke";
            string requested = "mira";
            int option = Array.FindIndex(args, arg => string.Equals(arg, optionName, StringComparison.OrdinalIgnoreCase));
            if (option >= 0 && option + 1 < args.Length && !args[option + 1].StartsWith("-", StringComparison.Ordinal))
            {
                requested = args[option + 1].Trim().ToLowerInvariant();
            }

            ObjectType requestedType = requested == "courier" || requested == "tovan" ? ObjectType.CityCourier
                : requested == "novice" || requested == "sera" ? ObjectType.NoviceHealer
                : requested == "traveler" || requested == "edda" ? ObjectType.WoundedTraveler
                : requested == "stable" || requested == "pell" ? ObjectType.StableHand
                : requested == "diner" || requested == "kate" ? ObjectType.Diner
                : requested == "herald" || requested == "vann" ? ObjectType.RoyalHerald
                : requested == "nessa" || requested == "market" ? ObjectType.MarketClerk
                : requested == "brann" || requested == "captain" ? ObjectType.GateCaptain
                : requested == "guard" || requested == "rusk" ? ObjectType.TownGuard
                : requested == "armorer" || requested == "borin" ? ObjectType.ArmorerNpc
                : requested == "vendor" || requested == "tessa" ? ObjectType.WeaponMerchantNpc
                : requested == "enchanter" || requested == "maud" ? ObjectType.EnchanterNpc
                : ObjectType.TempleHealer;
            MapObject npc = state.Map.Objects.FirstOrDefault(obj => obj != null && obj.Type == requestedType)
                ?? state.Map.Objects.FirstOrDefault(obj => obj != null && obj.Type == ObjectType.TempleHealer)
                ?? state.Map.Objects.FirstOrDefault(obj => obj != null && obj.Type == ObjectType.QuestBoard)
                ?? state.Map.Objects.FirstOrDefault(obj => obj != null && ShouldResolveExploreObjectFromAdjacent(obj));
            if (npc == null) return false;

            int[] dx = { 0, -1, 1, 0 };
            int[] dy = { -1, 0, 0, 1 };
            for (int i = 0; i < dx.Length; i++)
            {
                int standX = npc.X - dx[i];
                int standY = npc.Y - dy[i];
                if (!CanStepExplore(standX, standY)) continue;
                state.PlayerX = standX;
                state.PlayerY = standY;
                exploreFacingX = dx[i];
                exploreFacingY = dy[i];
                InvalidateExplorationController();
                UseNearbyExploreObject();
                return CurrentUiOverlay() == UiOverlay.Dialogue;
            }

            return false;
        }

        private void RequestVisualSmokeCaptureIfNeeded(string[] args)
        {
            if (args == null) return;
            int option = Array.FindIndex(args, arg => string.Equals(arg, "-ashen-capture", StringComparison.OrdinalIgnoreCase));
            if (option < 0 || option + 1 >= args.Length || string.IsNullOrWhiteSpace(args[option + 1])) return;
            string capturePath = Path.GetFullPath(args[option + 1]);
            bool quitAfterCapture = args.Any(arg => string.Equals(arg, "-ashen-capture-quit", StringComparison.OrdinalIgnoreCase));
            StartCoroutine(CaptureVisualSmoke(capturePath, quitAfterCapture));
        }

        private static int CommandLineDimension(string[] args, string option, int fallback, int minimum, int maximum)
        {
            if (args == null) return fallback;
            for (int i = 0; i + 1 < args.Length; i++)
            {
                if (!string.Equals(args[i], option, StringComparison.OrdinalIgnoreCase)) continue;
                return int.TryParse(args[i + 1], out int requested)
                    ? Mathf.Clamp(requested, minimum, Mathf.Max(minimum, maximum))
                    : fallback;
            }
            return fallback;
        }

        private IEnumerator CaptureVisualSmoke(string capturePath, bool quitAfterCapture)
        {
            string[] captureArgs = Environment.GetCommandLineArgs();
            bool feedbackCapture = captureArgs.Any(arg => string.Equals(arg, "-ashen-feedback-smoke", StringComparison.OrdinalIgnoreCase));
            if (feedbackCapture)
            {
                PowerImpactEcho showcase = powerImpactEchoes
                    .Where(echo => echo != null)
                    .OrderByDescending(echo => echo.ImpactAt)
                    .FirstOrDefault();
                float captureAt = showcase == null ? Time.time + 0.16f : showcase.ImpactAt + 0.12f;
                float waitDeadline = Time.realtimeSinceStartup + 4f;
                while (Time.time < captureAt && Time.realtimeSinceStartup < waitDeadline) yield return null;
            }
            else
            {
                yield return new WaitForSecondsRealtime(2f);
            }

            bool combatPresentationCapture = feedbackCapture
                || captureArgs.Any(arg => string.Equals(arg, "-ashen-combat-smoke", StringComparison.OrdinalIgnoreCase));
            if (combatPresentationCapture)
            {
                bannerText = "";
                bannerUntil = 0f;
                MarkUiDirty();
                yield return null;
            }
            yield return new WaitForEndOfFrame();

            string directory = Path.GetDirectoryName(capturePath);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            if (File.Exists(capturePath)) File.Delete(capturePath);

            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            int requestedWidth = RequestedCaptureDimension(captureArgs, "-screen-width", screenWidth);
            int requestedHeight = RequestedCaptureDimension(captureArgs, "-screen-height", screenHeight);
            ScreenCapture.CaptureScreenshot(capturePath, 1);

            bool captureEvaluated = false;
            int pngWidth = 0;
            int pngHeight = 0;
            string readFailure = "PNG was not written before the capture deadline";
            CaptureAcceptanceResult acceptance = new CaptureAcceptanceResult(CaptureAcceptanceFailure.NoPixelSamples);
            float deadline = Time.realtimeSinceStartup + 10f;
            while (Time.realtimeSinceStartup < deadline)
            {
                yield return null;
                if (!File.Exists(capturePath) || new FileInfo(capturePath).Length <= 256L) continue;
                if (!TryEvaluateVisualSmokeCapture(
                    capturePath,
                    requestedWidth,
                    requestedHeight,
                    screenWidth,
                    screenHeight,
                    out acceptance,
                    out pngWidth,
                    out pngHeight,
                    out readFailure))
                {
                    continue;
                }

                captureEvaluated = true;
                break;
            }

            bool captured = captureEvaluated && acceptance.Accepted;
            string acceptanceDetail = captureEvaluated
                ? $"failure={acceptance.Failure}, png={pngWidth}x{pngHeight}, samples={acceptance.SampleCount}, nearBlack={acceptance.NearBlackSampleCount}, brightness={acceptance.MinimumBrightness}-{acceptance.MaximumBrightness}"
                : $"failure={readFailure}, png=unreadable";
            string captureSummary = $"{VersionInfo.ProductName} visual smoke capture: path={capturePath}, complete={captured}, requested={requestedWidth}x{requestedHeight}, screen={screenWidth}x{screenHeight}, {acceptanceDetail}.";
            if (captured) Debug.Log(captureSummary);
            else Debug.LogError(captureSummary);
            if (quitAfterCapture) Application.Quit(captured ? 0 : 2);
        }

        private static int RequestedCaptureDimension(string[] args, string option, int fallback)
        {
            if (args == null) return fallback;
            for (int i = 0; i + 1 < args.Length; i++)
            {
                if (!string.Equals(args[i], option, StringComparison.OrdinalIgnoreCase)) continue;
                return int.TryParse(args[i + 1], out int requested) && requested > 0
                    ? requested
                    : fallback;
            }
            return fallback;
        }

        private static bool TryEvaluateVisualSmokeCapture(
            string capturePath,
            int requestedWidth,
            int requestedHeight,
            int screenWidth,
            int screenHeight,
            out CaptureAcceptanceResult acceptance,
            out int pngWidth,
            out int pngHeight,
            out string readFailure)
        {
            acceptance = new CaptureAcceptanceResult(CaptureAcceptanceFailure.NoPixelSamples);
            pngWidth = 0;
            pngHeight = 0;
            readFailure = "";
            Texture2D texture = null;
            try
            {
                byte[] pngBytes = File.ReadAllBytes(capturePath);
                texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (!texture.LoadImage(pngBytes, false))
                {
                    readFailure = "PNG could not be decoded";
                    return false;
                }

                pngWidth = texture.width;
                pngHeight = texture.height;
                Color32[] pixels = texture.GetPixels32();
                CapturePixelSample[] samples = SampleCapturePixels(pixels, pngWidth, pngHeight);
                acceptance = CaptureAcceptanceRules.Evaluate(
                    requestedWidth,
                    requestedHeight,
                    screenWidth,
                    screenHeight,
                    pngWidth,
                    pngHeight,
                    samples);
                return true;
            }
            catch (IOException ex)
            {
                readFailure = "PNG read failed: " + ex.Message;
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                readFailure = "PNG read failed: " + ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                readFailure = "PNG validation failed: " + ex.Message;
                return false;
            }
            finally
            {
                if (texture != null) UnityEngine.Object.Destroy(texture);
            }
        }

        private static CapturePixelSample[] SampleCapturePixels(Color32[] pixels, int width, int height)
        {
            if (pixels == null || width <= 0 || height <= 0 || pixels.Length < width * height)
            {
                return Array.Empty<CapturePixelSample>();
            }

            int columns = Math.Min(32, width);
            int rows = Math.Min(18, height);
            CapturePixelSample[] samples = new CapturePixelSample[columns * rows];
            int sampleIndex = 0;
            for (int row = 0; row < rows; row++)
            {
                int y = ((row * 2 + 1) * height) / (rows * 2);
                for (int column = 0; column < columns; column++)
                {
                    int x = ((column * 2 + 1) * width) / (columns * 2);
                    Color32 pixel = pixels[y * width + x];
                    samples[sampleIndex++] = new CapturePixelSample(pixel.r, pixel.g, pixel.b);
                }
            }
            return samples;
        }

        private static bool ShouldAutoQuitAfterBoot(string[] args, bool isBatchMode, bool isEditor)
        {
            if (!isBatchMode || isEditor || args == null) return false;
            return args.Any(arg => string.Equals(arg, "-quit", StringComparison.OrdinalIgnoreCase));
        }

        private void InitializePresentationScreens()
        {
            TryInitializePresentationScreen("Tavern", EnsureTavernScreen, true);
            TryInitializePresentationScreen("Party setup", EnsurePartySetupScreen, false);
            TryInitializePresentationScreen("Exploration HUD", EnsureExplorationHudScreen, false);
            TryInitializePresentationScreen("Combat HUD", EnsureCombatHudScreen, false);
            TryInitializePresentationScreen("Combat ability modal", EnsureCombatAbilityModalScreen, false);
            TryInitializePresentationScreen("Dialogue", EnsureDialogueScreen, false);
            TryInitializePresentationScreen("Loot popup", EnsureLootPopupScreen, false);
            TryInitializePresentationScreen("Armory overlay", EnsureArmoryOverlayScreen, false);
            TryInitializePresentationScreen("Pause menu", EnsurePauseMenuScreen, false);
            TryInitializePresentationScreen("Help overlay", EnsureHelpOverlayScreen, false);
            TryInitializePresentationScreen("End state", EnsureEndStateScreen, false);
            TryInitializePresentationScreen("Banner toast", EnsureBannerToastScreen, false);

            SyncTavernScreen();
            SyncPartySetupScreen();
            SyncExplorationHudScreen();
            SyncCombatHudScreen();
            SyncCombatAbilityModalScreen();
            SyncDialogueScreen();
            SyncLootPopupScreen();
            SyncArmoryOverlayScreen();
            SyncPauseMenuScreen();
            SyncHelpOverlayScreen();
            SyncEndStateScreen();
            SyncBannerToastScreen();
        }

        private void TryInitializePresentationScreen(string screenName, Action initializer, bool required)
        {
            try
            {
                initializer();
            }
            catch (Exception ex)
            {
                Debug.LogException(new InvalidOperationException($"{VersionInfo.ProductName} {screenName} UI startup failed.", ex));
                if (required && string.IsNullOrEmpty(launchError)) launchError = $"{screenName} UI failed: {ex.Message}";
            }
        }

        private void ValidateContentCatalogs()
        {
            if (contentValidated) return;
            contentValidated = true;

            List<string> warnings = new List<string>();
            ValidateFormulaCatalog(warnings);
            ValidateAbilityCatalog(warnings);
            ValidateEnemyCatalog(warnings);

            if (warnings.Count == 0) return;
            foreach (string warning in warnings)
            {
                Debug.LogWarning($"{VersionInfo.ProductName} content validation: {warning}");
            }
        }

        private void ValidateFormulaCatalog(List<string> warnings)
        {
            HashSet<string> seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> validTargets = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "ally", "enemy", "self", "tile" };
            HashSet<string> validEffects = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "chain", "cure", "damage", "dispel", "drain", "heal", "status", "summon", "tempest", "terrain", "teleport", "thunderclap", "transform" };
            HashSet<string> validSchools = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "mend", "ember", "hex", "pact" };
            HashSet<string> validSkills = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "mend", "ember", "hex" };

            foreach (FormulaDef formula in formulaBook)
            {
                if (formula == null)
                {
                    warnings.Add("formulaBook contains a null row.");
                    continue;
                }

                string code = (formula.Code ?? "").Trim();
                string label = string.IsNullOrEmpty(code) ? formula.Name ?? "(unnamed formula)" : code;
                if (code.Length != 3 || code.Any(c => c < 'A' || c > 'Z')) warnings.Add($"{label} formula code must be exactly three uppercase letters.");
                if (!seenCodes.Add(code)) warnings.Add($"{label} formula code is duplicated.");
                if (string.IsNullOrWhiteSpace(formula.Name)) warnings.Add($"{label} is missing a name.");
                if (string.IsNullOrWhiteSpace(formula.Hint)) warnings.Add($"{label} is missing a hint.");
                if (formula.Mana < 0) warnings.Add($"{label} has negative mana cost.");
                if (formula.Range < 0) warnings.Add($"{label} has negative range.");
                if (!validTargets.Contains(formula.Target ?? "")) warnings.Add($"{label} has invalid target '{formula.Target}'.");
                if (!validEffects.Contains(formula.Effect ?? "")) warnings.Add($"{label} has invalid effect '{formula.Effect}'.");
                if (!validSkills.Contains(formula.Skill ?? "")) warnings.Add($"{label} has invalid skill '{formula.Skill}'.");

                foreach (string school in SplitContentTokens(formula.School))
                {
                    if (!validSchools.Contains(school)) warnings.Add($"{label} has invalid school '{school}'.");
                }

                if (StringEquals(formula.Effect, "terrain") && string.IsNullOrWhiteSpace(formula.Terrain)) warnings.Add($"{label} terrain formula is missing Terrain.");
                if (StringEquals(formula.Effect, "summon") && string.IsNullOrWhiteSpace(formula.SummonRole)) warnings.Add($"{label} summon formula is missing SummonRole.");
                if ((StringEquals(formula.Effect, "damage") || StringEquals(formula.Effect, "drain") || StringEquals(formula.Effect, "heal")) && formula.Power <= 0) warnings.Add($"{label} {formula.Effect} formula needs positive Power.");
            }
        }

        private void ValidateAbilityCatalog(List<string> warnings)
        {
            HashSet<string> seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string[] classes = { "warrior", "rogue", "ranger" };
            foreach (string classKey in classes)
            {
                foreach (string id in AbilityIdsForClass(classKey))
                {
                    MartialAbility ability = AbilityDef(id);
                    if (ability == null)
                    {
                        warnings.Add($"{classKey} ability id '{id}' has no AbilityDef row.");
                        continue;
                    }

                    if (!seenIds.Add(ability.Id ?? "")) warnings.Add($"{ability.Id} ability id is duplicated.");
                    if (!StringEquals(ability.ClassKey, classKey)) warnings.Add($"{ability.Id} ability class '{ability.ClassKey}' does not match '{classKey}'.");
                    if (string.IsNullOrWhiteSpace(ability.Name)) warnings.Add($"{id} ability is missing a name.");
                    if (string.IsNullOrWhiteSpace(ability.Short)) warnings.Add($"{id} ability is missing a short label.");
                    if (ability.RequiredLevel < 1) warnings.Add($"{id} ability has invalid required level {ability.RequiredLevel}.");
                    if (ability.Range < 0) warnings.Add($"{id} ability has negative range.");
                    if (string.IsNullOrWhiteSpace(ability.Summary)) warnings.Add($"{id} ability is missing a summary.");
                    if (string.IsNullOrWhiteSpace(ability.Detail)) warnings.Add($"{id} ability is missing detail text.");
                }
            }
        }

        private void ValidateEnemyCatalog(List<string> warnings)
        {
            HashSet<string> seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string id in enemyTemplateIds)
            {
                if (!seenIds.Add(id)) warnings.Add($"{id} enemy template id is duplicated.");
                EnemyTemplate template = EnemyTemplate.For(id);
                if (template == null)
                {
                    warnings.Add($"{id} enemy template is missing.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(template.Name)) warnings.Add($"{id} enemy is missing a display name.");
                if (template.Hp <= 0) warnings.Add($"{id} enemy has non-positive HP.");
                if (template.Power <= 0) warnings.Add($"{id} enemy has non-positive Power.");
                if (template.Defense < 0) warnings.Add($"{id} enemy has negative Defense.");
                if (template.Agility < 0) warnings.Add($"{id} enemy has negative Agility.");
                if (template.Range <= 0) warnings.Add($"{id} enemy has invalid Range.");
                if (string.IsNullOrWhiteSpace(template.DamageType)) warnings.Add($"{id} enemy is missing DamageType.");
            }
        }

        private IEnumerable<string> SplitContentTokens(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) yield break;
            string[] tokens = value.Split('|');
            foreach (string token in tokens)
            {
                string trimmed = token.Trim();
                if (!string.IsNullOrEmpty(trimmed)) yield return trimmed;
            }
        }

        private static bool StringEquals(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        private void ToggleArmory(ArmoryTab tab)
        {
            ToggleArmory((int)tab);
        }

        private void ToggleArmory(int tab)
        {
            if (state == null) return;
            int nextTab = Mathf.Clamp(tab, 0, ArmoryTabCount - 1);
            if (showArmory && armoryTab == nextTab)
            {
                armoryInventoryTargetPickerOpen = false;
                showArmory = false;
            }
            else
            {
                armoryInventoryTargetPickerOpen = false;
                showArmory = true;
                armoryTab = nextTab;
                showSpellbook = false;
                showAbilityPanel = false;
                showDialogue = false;
                DismissLootPopupSilently();
            }
            PlaySfx(showArmory ? "uiopen" : "uiclose", 0.55f);
            MarkUiDirty();
            SyncArmoryOverlayScreen();
        }

        private UiOverlay CurrentUiOverlay()
        {
            return ScreenInputRules.TopOverlay(
                showPauseMenu,
                showHelpOverlay,
                showArmory,
                showDialogue,
                IsLootPopupOpen(),
                state != null && state.Mode == GameMode.Combat && (showSpellbook || showAbilityPanel));
        }

        private bool CanAcceptGameplayInput()
        {
            return state != null && !ShouldShowStartupSplash() && ScreenInputRules.CanAcceptGameplayInput(CurrentUiOverlay());
        }

        private void SuppressBoardPointer()
        {
            suppressBoardPointerThroughFrame = Mathf.Max(suppressBoardPointerThroughFrame, Time.frameCount + 1);
        }

        private bool IsBoardPointerSuppressed()
        {
            return ScreenInputRules.ShouldSuppressBoardPointer(Time.frameCount, suppressBoardPointerThroughFrame);
        }

        private void ConsumeSuppressedPointerEvent()
        {
            Event current = Event.current;
            if (!IsBoardPointerSuppressed() || current == null) return;
            switch (current.type)
            {
                case EventType.MouseDown:
                case EventType.MouseUp:
                case EventType.MouseDrag:
                case EventType.ContextClick:
                case EventType.ScrollWheel:
                    current.Use();
                    break;
            }
        }

        private bool IsLootPopupOpen()
        {
            return lootPanelItem != null
                && !string.IsNullOrEmpty(lootPanelBody)
                && (lootPanelRequiresDismissal || Time.time <= lootPanelUntil);
        }

        private void DismissLootPopupSilently()
        {
            lootPanelUntil = 0f;
            lootPanelItem = null;
            lootPanelBody = "";
            lootPanelTitle = "";
            lootPanelTraitLine = "";
            lootPanelEquipNote = "";
            lootPanelGold = 0;
            lootPanelSupplies = 0;
            lootPanelElixirs = 0;
            lootPanelRequiresDismissal = false;
            MarkUiDirty();
        }

        private bool CloseTopOverlay()
        {
            UiOverlay overlay = CurrentUiOverlay();
            if (overlay != UiOverlay.None) SuppressBoardPointer();
            switch (overlay)
            {
                case UiOverlay.Help:
                    CloseHelpOverlay();
                    return true;
                case UiOverlay.Pause:
                    ClosePauseMenu();
                    return true;
                case UiOverlay.Dialogue:
                    CloseDialogue();
                    return true;
                case UiOverlay.Armory:
                    showArmory = false;
                    SyncArmoryOverlayScreen();
                    PlaySfx("uiclose", 0.45f);
                    MarkUiDirty();
                    return true;
                case UiOverlay.AbilityPicker:
                    CloseCombatAbilityModal();
                    return true;
                case UiOverlay.Loot:
                    DismissLootPopup();
                    return true;
                default:
                    return false;
            }
        }

        private void RecoverUnavailableOverlay(UiOverlay overlay, string label)
        {
            SuppressBoardPointer();
            switch (overlay)
            {
                case UiOverlay.Pause:
                    showPauseMenu = false;
                    pauseSettingsOpen = false;
                    pauseConfirmAction = "";
                    break;
                case UiOverlay.Help:
                    showHelpOverlay = false;
                    break;
                case UiOverlay.Armory:
                    showArmory = false;
                    break;
                case UiOverlay.Loot:
                    DismissLootPopupSilently();
                    break;
            }

            string screenName = string.IsNullOrWhiteSpace(label) ? "Panel" : label;
            PushLog($"{screenName} could not open. Gameplay input was restored.", Tone.Warn);
            ShowBanner(screenName + " recovered");
            PlaySfx("blocked", 0.45f);
            MarkUiDirty();
        }

        private void EnsurePauseMenuScreen()
        {
            if (pauseMenuScreen != null && pauseMenuScreen.IsReady) return;
            if (pauseMenuScreen != null)
            {
                Destroy(pauseMenuScreen.gameObject);
                pauseMenuScreen = null;
            }
            GameObject screen = new GameObject("Pause Menu Screen");
            screen.transform.SetParent(transform, false);
            PauseMenuScreen created = screen.AddComponent<PauseMenuScreen>();
            try
            {
                created.Bind(new PauseMenuScreenBindings
                {
                    View = BuildPauseMenuView,
                    Continue = ClosePauseMenu,
                    Save = SaveGame,
                    Load = LoadGame,
                    ToggleSettings = TogglePauseSettings,
                    ToggleAudio = ToggleSfxMute,
                    ToggleMusic = ToggleMusicMute,
                    VolumeDown = () => AdjustSfxVolume(-25),
                    VolumeUp = () => AdjustSfxVolume(25),
                    MusicVolumeDown = () => AdjustMusicVolume(-25),
                    MusicVolumeUp = () => AdjustMusicVolume(25),
                    ToggleReducedMotion = ToggleReducedMotionSetting,
                    RequestRetreat = RequestPauseRetreat,
                    ConfirmRetreat = ConfirmPauseRetreat,
                    RequestReturnToTavern = RequestPauseReturnToTavern,
                    ConfirmReturnToTavern = ConfirmPauseReturnToTavern,
                    RequestNewGame = RequestPauseNewGame,
                    ConfirmNewGame = ConfirmPauseNewGame
                });
                created.SetVisible(false);
                pauseMenuScreen = created;
            }
            catch
            {
                created.SetVisible(false);
                screen.SetActive(false);
                Destroy(screen);
                throw;
            }
        }

        private void SyncPauseMenuScreen()
        {
            bool visible = state != null
                && CurrentUiOverlay() == UiOverlay.Pause
                && !ShouldShowStartupSplash();
            if (visible && (pauseMenuScreen == null || !pauseMenuScreen.IsReady))
            {
                TryInitializePresentationScreen("Pause menu recovery", EnsurePauseMenuScreen, false);
            }
            if (pauseMenuScreen == null)
            {
                if (visible) RecoverUnavailableOverlay(UiOverlay.Pause, "Menu");
                return;
            }
            if (!visible)
            {
                pauseMenuScreen.SetVisible(false);
                return;
            }
            bool refresh = !pauseMenuScreen.HasRenderableGeometry
                || ShouldRefreshPresentation(ref lastPauseMenuRefreshKey, PauseMenuRefreshKey());
            if (refresh)
            {
                pauseMenuScreen.SetVisible(false);
                try
                {
                    pauseMenuScreen.Refresh();
                    pauseMenuScreen.SetVisible(true);
                    Canvas.ForceUpdateCanvases();
                }
                catch (Exception ex)
                {
                    pauseMenuScreen.SetVisible(false);
                    Debug.LogException(new InvalidOperationException(VersionInfo.ProductName + " pause menu refresh failed.", ex));
                }
            }
            if (!pauseMenuScreen.HasRenderableGeometry)
            {
                pauseMenuScreen.SetVisible(false);
                RecoverUnavailableOverlay(UiOverlay.Pause, "Menu");
            }
        }

        private PauseMenuView BuildPauseMenuView()
        {
            string route = state == null ? "" : state.Mode == GameMode.Combat ? $"{GameSubtitle} / Combat" : $"{StoryChapterTitle()} / D{state.Depth}";
            string saved = HasSavedGame() ? "Campaign save found." : "No campaign save yet.";
            string audio = state == null ? "SFX" : state.SfxMuted ? "Enable SFX" : "Mute SFX";
            bool showRetreat = CombatRetreatRules.CanOffer(state, labSaveBlocked, betaLabMode);
            return new PauseMenuView
            {
                Title = "Menu",
                RouteLine = route,
                SaveLine = saved,
                AudioLine = audio,
                SfxLine = state == null ? "SFX 100%" : $"SFX {state.SfxVolumePercent}%",
                MusicLine = state == null
                    ? "Music 65%"
                    : state.MusicMuted
                        ? "Music Muted"
                        : $"Music {state.MusicVolumePercent}%",
                SettingsOpen = pauseSettingsOpen,
                ShowRetreat = showRetreat,
                RetreatEnabled = showRetreat && CombatRetreatRules.CanAfford(state),
                ConfirmRetreat = string.Equals(pauseConfirmAction, "retreat", StringComparison.Ordinal),
                ConfirmReturnToTavern = string.Equals(pauseConfirmAction, "return", StringComparison.Ordinal),
                ConfirmNewGame = string.Equals(pauseConfirmAction, "new", StringComparison.Ordinal)
            };
        }

        private string PauseMenuRefreshKey()
        {
            int hash = 29;
            hash = unchecked(hash * 31 + (showPauseMenu ? 1 : 0));
            hash = unchecked(hash * 31 + (pauseSettingsOpen ? 1 : 0));
            hash = unchecked(hash * 31 + (pauseConfirmAction ?? "").GetHashCode());
            hash = unchecked(hash * 31 + (HasSavedGame() ? 1 : 0));
            if (state != null)
            {
                hash = unchecked(hash * 31 + (int)state.Mode);
                hash = unchecked(hash * 31 + state.Depth);
                hash = unchecked(hash * 31 + state.Gold);
                hash = unchecked(hash * 31 + state.Supplies);
                hash = unchecked(hash * 31 + state.Elixirs);
                hash = unchecked(hash * 31 + (state.SfxMuted ? 1 : 0));
                hash = unchecked(hash * 31 + (state.MusicMuted ? 1 : 0));
                hash = unchecked(hash * 31 + state.SfxVolumePercent);
                hash = unchecked(hash * 31 + state.MusicVolumePercent);
                hash = unchecked(hash * 31 + (state.ReducedMotion ? 1 : 0));
            }
            return "pause=" + hash;
        }

        private bool CanOpenPauseMenu()
        {
            return state != null
                && !ShouldShowStartupSplash()
                && (state.Mode == GameMode.Explore || state.Mode == GameMode.Combat);
        }

        private void OpenPauseMenu()
        {
            if (!CanOpenPauseMenu()) return;
            showArmory = false;
            showDialogue = false;
            showSpellbook = false;
            showAbilityPanel = false;
            DismissLootPopupSilently();
            showPauseMenu = true;
            pauseSettingsOpen = false;
            pauseConfirmAction = "";
            PlaySfx("uiopen", 0.45f);
            MarkUiDirty();
            SyncPauseMenuScreen();
        }

        private void ClosePauseMenu()
        {
            SuppressBoardPointer();
            showPauseMenu = false;
            pauseSettingsOpen = false;
            pauseConfirmAction = "";
            PlaySfx("uiclose", 0.35f);
            MarkUiDirty();
            SyncPauseMenuScreen();
        }

        private void EnsureHelpOverlayScreen()
        {
            if (helpOverlayScreen != null && helpOverlayScreen.IsReady) return;
            if (helpOverlayScreen != null)
            {
                Destroy(helpOverlayScreen.gameObject);
                helpOverlayScreen = null;
            }
            GameObject screen = new GameObject("Help Overlay Screen");
            screen.transform.SetParent(transform, false);
            HelpOverlayScreen created = screen.AddComponent<HelpOverlayScreen>();
            try
            {
                created.Bind(new HelpOverlayBindings
                {
                    View = BuildHelpOverlayView,
                    Close = CloseHelpOverlay
                });
                created.SetVisible(false);
                helpOverlayScreen = created;
            }
            catch
            {
                created.SetVisible(false);
                screen.SetActive(false);
                Destroy(screen);
                throw;
            }
        }

        private void SyncHelpOverlayScreen()
        {
            bool visible = state != null
                && CurrentUiOverlay() == UiOverlay.Help
                && !ShouldShowStartupSplash();
            if (visible && (helpOverlayScreen == null || !helpOverlayScreen.IsReady))
            {
                TryInitializePresentationScreen("Help overlay recovery", EnsureHelpOverlayScreen, false);
            }
            if (helpOverlayScreen == null)
            {
                if (visible) RecoverUnavailableOverlay(UiOverlay.Help, "Help");
                return;
            }
            if (!visible)
            {
                helpOverlayScreen.SetVisible(false);
                return;
            }
            bool refresh = !helpOverlayScreen.HasRenderableGeometry
                || ShouldRefreshPresentation(ref lastHelpOverlayRefreshKey, HelpOverlayRefreshKey());
            if (refresh)
            {
                helpOverlayScreen.SetVisible(false);
                try
                {
                    helpOverlayScreen.Refresh();
                    helpOverlayScreen.SetVisible(true);
                    Canvas.ForceUpdateCanvases();
                }
                catch (Exception ex)
                {
                    helpOverlayScreen.SetVisible(false);
                    Debug.LogException(new InvalidOperationException(VersionInfo.ProductName + " help overlay refresh failed.", ex));
                }
            }
            if (!helpOverlayScreen.HasRenderableGeometry)
            {
                helpOverlayScreen.SetVisible(false);
                RecoverUnavailableOverlay(UiOverlay.Help, "Help");
            }
        }

        private HelpOverlayView BuildHelpOverlayView()
        {
            GameMode mode = state == null ? GameMode.Tavern : state.Mode;
            return HelpOverlayContent.Build(
                mode,
                TavernMenuRules.ShowDeveloperTesting(DeveloperTestingBuildEnabled()),
                SummonedTreeDuration,
                HomeTownName);
        }

        private string HelpOverlayRefreshKey()
        {
            int hash = 41;
            hash = unchecked(hash * 31 + (showHelpOverlay ? 1 : 0));
            hash = unchecked(hash * 31 + (state == null ? -1 : (int)state.Mode));
            hash = unchecked(hash * 31 + (TavernMenuRules.ShowDeveloperTesting(DeveloperTestingBuildEnabled()) ? 1 : 0));
            return "help=" + hash;
        }

        private void OpenHelpOverlay()
        {
            if (state == null || ShouldShowStartupSplash()) return;
            showHelpOverlay = true;
            PlaySfx("uiopen", 0.45f);
            MarkUiDirty();
            SyncHelpOverlayScreen();
        }

        private void CloseHelpOverlay()
        {
            SuppressBoardPointer();
            showHelpOverlay = false;
            PlaySfx("uiclose", 0.35f);
            MarkUiDirty();
            SyncHelpOverlayScreen();
        }

        private void EnsureEndStateScreen()
        {
            if (endStateScreen != null) return;
            GameObject screen = new GameObject("End State Screen");
            screen.transform.SetParent(transform, false);
            EndStateScreen created = screen.AddComponent<EndStateScreen>();
            created.Bind(new EndStateScreenBindings
            {
                View = BuildEndStateView,
                NewParty = NewMuster,
                ReturnToTavern = ReturnEndStateToTavern,
                BetaLab = StartBetaCombatLab
            });
            created.SetVisible(false);
            endStateScreen = created;
        }

        private void SyncEndStateScreen()
        {
            if (endStateScreen == null) return;
            bool visible = state != null
                && (state.Mode == GameMode.Defeat || state.Mode == GameMode.Victory)
                && !ShouldShowStartupSplash();
            endStateScreen.SetVisible(visible);
            if (visible && ShouldRefreshPresentation(ref lastEndStateRefreshKey, EndStateRefreshKey())) endStateScreen.Refresh();
        }

        private EndStateView BuildEndStateView()
        {
            if (state == null) return null;
            string[] partyRows = state.Party == null
                ? Array.Empty<string>()
                : state.Party.Select(EndStatePartyRow).ToArray();

            if (state.Mode == GameMode.Victory)
            {
                int count = state.Party == null ? 0 : state.Party.Count;
                int living = state.Party == null ? 0 : state.Party.Count(p => p.Hp > 0);
                int averageLevel = count == 0 ? 1 : Mathf.RoundToInt((float)state.Party.Average(p => p.Level));
                return EndStateContent.BuildVictory(
                    HomeTownName,
                    living,
                    count,
                    averageLevel,
                    state.Gold,
                    state.Depth,
                    partyRows,
                    TavernMenuRules.ShowDeveloperTesting(DeveloperTestingBuildEnabled()));
            }

            if (state.Mode == GameMode.Defeat)
            {
                return EndStateContent.BuildDefeat(HomeTownName, partyRows);
            }

            return null;
        }

        private string EndStatePartyRow(PartyMember member)
        {
            if (member == null) return "";
            string condition = member.Hp > 0 ? $"HP {member.Hp}/{member.MaxHp}" : "fallen";
            return $"{member.Name} / L{member.Level} {DisplayClass(member.ClassKey)} / {condition} / {BestSkillLabel(member)} {BestSkillValue(member)}";
        }

        private string EndStateRefreshKey()
        {
            int hash = 47;
            if (state != null)
            {
                hash = unchecked(hash * 31 + (int)state.Mode);
                hash = unchecked(hash * 31 + state.Depth);
                hash = unchecked(hash * 31 + state.Gold);
                hash = unchecked(hash * 31 + (TavernMenuRules.ShowDeveloperTesting(DeveloperTestingBuildEnabled()) ? 1 : 0));
                if (state.Party != null)
                {
                    foreach (PartyMember member in state.Party)
                    {
                        hash = unchecked(hash * 31 + (member.Name ?? "").GetHashCode());
                        hash = unchecked(hash * 31 + (member.ClassKey ?? "").GetHashCode());
                        hash = unchecked(hash * 31 + member.Level);
                        hash = unchecked(hash * 31 + member.Hp);
                        hash = unchecked(hash * 31 + member.MaxHp);
                    }
                }
            }
            return "end=" + hash;
        }

        private void ReturnEndStateToTavern()
        {
            if (state == null) return;
            CloseTransientOverlays();
            state.Mode = GameMode.Tavern;
            ShowBanner("Returned to tavern");
            PlaySfx("ui", 0.55f);
            MarkUiDirty();
        }

        private void TogglePauseSettings()
        {
            pauseSettingsOpen = !pauseSettingsOpen;
            pauseConfirmAction = "";
            PlaySfx("ui", 0.45f);
            MarkUiDirty();
        }

        private void RequestPauseReturnToTavern()
        {
            pauseConfirmAction = "return";
            pauseSettingsOpen = false;
            ShowBanner("Confirm return");
            PlaySfx("ui", 0.45f);
            MarkUiDirty();
        }

        private void RequestPauseRetreat()
        {
            if (!CombatRetreatRules.CanOffer(state, labSaveBlocked, betaLabMode))
            {
                PushLog("This fight cannot use the campaign retreat route.", Tone.Warn);
                PlaySfx("blocked", 0.45f);
                return;
            }
            if (!CombatRetreatRules.CanAfford(state))
            {
                PushLog("Retreat needs one supply.", Tone.Warn);
                ShowBanner("No retreat supplies");
                PlaySfx("blocked", 0.45f);
                return;
            }

            pauseConfirmAction = "retreat";
            pauseSettingsOpen = false;
            ShowBanner("Confirm retreat");
            PlaySfx("ui", 0.45f);
            MarkUiDirty();
        }

        private void ConfirmPauseRetreat()
        {
            if (!CombatRetreatRules.CanOffer(state, labSaveBlocked, betaLabMode)
                || !CombatRetreatRules.CanAfford(state))
            {
                RequestPauseRetreat();
                return;
            }

            CancelCombatResolutionBeat(false);
            SyncPartyFromCombat();
            state.Supplies -= CombatRetreatRules.SupplyCost;
            string roamingThreatId = state.Combat?.RoamingThreatId ?? "";
            state.Combat = null;
            InvalidateCombatController();
            state.Mode = GameMode.Explore;
            betaLabMode = false;
            CloseTransientOverlays();
            ResolveRoamingThreatRetreat(roamingThreatId);
            ReturnPartyToTempleSquare(
                "The party spends one supply, abandons the fight and regroups beneath Temple Square's bells.",
                "Retreat: Temple Square",
                0.78f);
            AutosaveCheckpoint("combat retreat");
            MarkUiDirty();
        }

        private void ConfirmPauseReturnToTavern()
        {
            NewMuster();
            state.Mode = GameMode.Tavern;
            ShowBanner("Returned to tavern");
            PlaySfx("ui", 0.55f);
            MarkUiDirty();
        }

        private void RequestPauseNewGame()
        {
            pauseConfirmAction = "new";
            pauseSettingsOpen = false;
            ShowBanner("Confirm new game");
            PlaySfx("ui", 0.45f);
            MarkUiDirty();
        }

        private void ConfirmPauseNewGame()
        {
            StartNewGame();
            ShowBanner("New game");
            MarkUiDirty();
        }

        private void Update()
        {
            float now = Time.time;
            tweens.RemoveAll(t => now > t.Start + t.Duration + 0.1f);
            floatTexts.RemoveAll(t => now > t.Start + t.Duration);
            particles.RemoveAll(p => now > p.Start + p.Duration);
            beams.RemoveAll(b => now > b.Start + b.Duration);
            flashes.RemoveAll(f => now > f.Start + f.Duration);
            castGlyphs.RemoveAll(g => now > g.Start + g.Duration);
            powerImpactEchoes.RemoveAll(e => now > e.ImpactAt + e.Duration);
            powerCastAuras.RemoveAll(a => now > a.Start + a.Duration);

            UpdateTavernMusic();
            UpdateTavernAmbience();
            UpdateExplorationAmbience();
            UpdateScheduledSfx();
            if (IsStartupSplashVisible()) return;

            if (Input.GetKeyDown(KeyCode.F5)) SaveGame();
            if (Input.GetKeyDown(KeyCode.F9)) LoadGame();
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ContextHelp();
                return;
            }
            if (Input.GetKeyDown(KeyCode.M)) ToggleSfxMute();
            if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus)) AdjustSfxVolume(25);
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) AdjustSfxVolume(-25);
            if (state != null && state.Mode == GameMode.Explore && Input.GetKeyDown(KeyCode.F8)) ToggleExploreArtDebug();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (CurrentUiOverlay() == UiOverlay.Dialogue)
                {
                    if (ReturnDialogueToTopics()) return;
                    CloseDialogue();
                    return;
                }
                if (CloseTopOverlay()) return;
                if (state != null && state.Mode == GameMode.Combat && CancelCombatTargeting()) return;
                if (CanOpenPauseMenu())
                {
                    OpenPauseMenu();
                    return;
                }
            }
            if (CurrentUiOverlay() == UiOverlay.Dialogue)
            {
                if (Time.frameCount > dialogueOpenedFrame)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) ChooseDialogueChoice(0);
                    else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) ChooseDialogueChoice(1);
                    else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) ChooseDialogueChoice(2);
                    else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) ChooseDialogueChoice(3);
                    else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) dialogueScreen?.MoveChoiceSelection(-1);
                    else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) dialogueScreen?.MoveChoiceSelection(1);
                }
                if (Time.frameCount > dialogueOpenedFrame
                    && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space)))
                {
                    if (IsDialogueChoicePage()) dialogueScreen?.InvokeSelectedChoice();
                    else AdvanceDialogue();
                }
                return;
            }
            bool gameplayMode = state != null && CanAcceptGameplayInput() && (state.Mode == GameMode.Explore || state.Mode == GameMode.Combat);
            if (gameplayMode && Input.GetKeyDown(KeyCode.P))
            {
                OpenPauseMenu();
                return;
            }
            if (gameplayMode && Input.GetKeyDown(KeyCode.I)) ToggleArmory(ArmoryTab.Pack);
            if (gameplayMode && Input.GetKeyDown(KeyCode.J)) ToggleArmory(ArmoryTab.Journal);
            if (state != null && state.Mode == GameMode.Explore && CanAcceptGameplayInput() && Input.GetKeyDown(KeyCode.C)) ToggleArmory(ArmoryTab.Spells);

            if (state == null) return;
            if (state.Mode == GameMode.Tavern)
            {
                if (DeveloperTestingHotkeyPressed())
                {
                    OpenDeveloperTestingShortcut();
                    return;
                }
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    if (HasSavedGame()) ContinueSavedGame();
                    else QuickStart();
                }
                if (Input.GetKeyDown(KeyCode.B)) QuickStart();
                if (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown(KeyCode.C)) StartNewGame();
                if (Input.GetKeyDown(KeyCode.S)) ToggleTavernSettings();
                if (Input.GetKeyDown(KeyCode.T)) ToggleTavernTesting();
            }
            else if (state.Mode == GameMode.Muster)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) QuickStart();
                if (Input.GetKeyDown(KeyCode.B)) BeginGame();
                if (Input.GetKeyDown(KeyCode.Escape)) state.Mode = GameMode.Tavern;
            }
            GameMode inputMode = state.Mode;
            if (inputMode == GameMode.Explore && CanAcceptGameplayInput())
            {
                HandleExploreKeyboard();
                return;
            }
            if (inputMode == GameMode.Combat)
            {
                if (CurrentUiOverlay() != UiOverlay.Pause) HandleCombatTimers();
                if (state.Mode != GameMode.Combat) return;
                if (CanAcceptGameplayInput()) HandleCombatHotkeys();
            }
        }

        private void LateUpdate()
        {
            SyncCurrentMajorScreen();
            SyncCurrentOverlayScreens();
        }

        private void SyncCurrentMajorScreen()
        {
            bool splashVisible = ShouldShowStartupSplash();
            bool tavernVisible = state != null && state.Mode == GameMode.Tavern && !splashVisible;
            bool musterVisible = state != null && state.Mode == GameMode.Muster && !splashVisible;
            bool exploreVisible = state != null && state.Mode == GameMode.Explore && !splashVisible;
            bool combatVisible = state != null && state.Mode == GameMode.Combat && !splashVisible;
            bool endStateVisible = state != null && (state.Mode == GameMode.Defeat || state.Mode == GameMode.Victory) && !splashVisible;

            if (tavernVisible) SyncTavernScreen();
            else tavernScreen?.SetVisible(false);

            if (musterVisible) SyncPartySetupScreen();
            else partySetupScreen?.SetVisible(false);

            if (exploreVisible) SyncExplorationHudScreen();
            else explorationHudScreen?.SetVisible(false);

            if (combatVisible) SyncCombatHudScreen();
            else combatHudScreen?.SetVisible(false);

            if (endStateVisible) SyncEndStateScreen();
            else endStateScreen?.SetVisible(false);
        }

        private void SyncCurrentOverlayScreens()
        {
            bool splashVisible = ShouldShowStartupSplash();
            UiOverlay overlay = CurrentUiOverlay();
            bool dialogueVisible = state != null && overlay == UiOverlay.Dialogue && !splashVisible;
            bool armoryVisible = state != null && overlay == UiOverlay.Armory && !splashVisible;
            bool combatModalVisible = state != null && overlay == UiOverlay.AbilityPicker && !splashVisible;
            bool lootVisible = state != null && overlay == UiOverlay.Loot && !splashVisible;
            bool pauseVisible = state != null && overlay == UiOverlay.Pause && !splashVisible;
            bool helpVisible = state != null && overlay == UiOverlay.Help && !splashVisible;

            if (dialogueVisible) SyncDialogueScreen();
            else dialogueScreen?.SetVisible(false);

            if (armoryVisible) SyncArmoryOverlayScreen();
            else armoryOverlayScreen?.SetVisible(false);

            if (combatModalVisible) SyncCombatAbilityModalScreen();
            else combatAbilityModalScreen?.SetVisible(false);

            if (lootVisible) SyncLootPopupScreen();
            else lootPopupScreen?.SetVisible(false);

            if (pauseVisible) SyncPauseMenuScreen();
            else pauseMenuScreen?.SetVisible(false);

            if (helpVisible) SyncHelpOverlayScreen();
            else helpOverlayScreen?.SetVisible(false);

            SyncBannerToastScreen();
        }

        private void EnsureBannerToastScreen()
        {
            if (bannerToastScreen != null) return;
            GameObject screen = new GameObject("Banner Toast Screen");
            screen.transform.SetParent(transform, false);
            BannerToastScreen created = screen.AddComponent<BannerToastScreen>();
            created.Bind(new BannerToastBindings
            {
                View = BuildBannerToastView
            });
            created.SetVisible(false);
            bannerToastScreen = created;
        }

        private BannerToastView BuildBannerToastView()
        {
            bool modalOpen = state != null && CurrentUiOverlay() != UiOverlay.None;
            bool powerVisible = state != null
                && state.Mode == GameMode.Combat
                && !modalOpen
                && Time.time < combatPowerCueUntil
                && !string.IsNullOrWhiteSpace(combatPowerCue.Title);
            if (powerVisible)
            {
                return new BannerToastView
                {
                    Visible = !ShouldShowStartupSplash(),
                    Text = combatPowerCue.Title,
                    Subtitle = combatPowerCue.Subtitle,
                    AccentHex = combatPowerCue.AccentHex,
                    IconTexture = combatPowerCueTexture,
                    IconSource = combatPowerCueSource,
                    Sigil = combatPowerCue.Sigil,
                    Outcome = Time.time >= combatPowerOutcomeVisibleAt ? combatPowerOutcomeText : "",
                    PowerCue = true,
                    Intensity = combatPowerCue.Intensity,
                    RemainingSeconds = Mathf.Max(0f, combatPowerCueUntil - Time.time),
                    TotalSeconds = Mathf.Max(0.01f, combatPowerCueUntil - combatPowerCueStarted),
                    ImpactSeconds = Mathf.Max(0f, combatPowerCueImpactAt - combatPowerCueStarted),
                    ReducedMotion = state.ReducedMotion
                };
            }

            float remaining = Mathf.Max(0f, bannerUntil - Time.time);
            float total = state != null && state.ReducedMotion ? 0.9f : 1.5f;
            return new BannerToastView
            {
                Visible = state != null && !modalOpen && !ShouldShowStartupSplash() && remaining > 0f && !string.IsNullOrWhiteSpace(bannerText),
                Text = bannerText,
                AccentHex = "d7a84e",
                RemainingSeconds = remaining,
                TotalSeconds = total,
                ReducedMotion = state != null && state.ReducedMotion
            };
        }

        private void SyncBannerToastScreen()
        {
            if (bannerToastScreen == null) return;
            bannerToastScreen.Refresh();
        }

        private bool ShouldRefreshPresentation(ref string lastKey, string localKey)
        {
            string nextKey = PresentationRefreshRules.ComposeKey(Screen.width, Screen.height, uiRevision, localKey);
            if (!PresentationRefreshRules.KeyChanged(lastKey, nextKey)) return false;
            lastKey = nextKey;
            return true;
        }

        private void MarkUiDirty()
        {
            unchecked
            {
                uiRevision++;
                if (uiRevision == 0) uiRevision = 1;
            }
        }

        private void InvalidateSavedGameCache()
        {
            hasSavedGameCacheValid = false;
            MarkUiDirty();
        }

        private void OnGUI()
        {
            try
            {
                EnsureStyles();
                ConsumeSuppressedPointerEvent();
                if (!splashClockStarted)
                {
                    splashStartedAt = Time.realtimeSinceStartup;
                    splashClockStarted = true;
                }

                UiOverlay activeOverlay = state == null ? UiOverlay.None : CurrentUiOverlay();
                bool gameplayOverlayHandledByUnityUi =
                    state != null &&
                    !ShouldShowStartupSplash() &&
                    (state.Mode == GameMode.Explore || state.Mode == GameMode.Combat) &&
                    HasRenderableGameplayOverlay(activeOverlay);
                bool combatHudHandledByUnityUi =
                    state != null &&
                    state.Mode == GameMode.Combat &&
                    activeOverlay == UiOverlay.None &&
                    combatHudScreen != null &&
                    combatHudScreen.HasUsableCommandBar;
                bool handledByUnityUi =
                    state != null &&
                    !ShouldShowStartupSplash() &&
                    ((state.Mode == GameMode.Tavern && tavernScreen != null) ||
                    (state.Mode == GameMode.Muster && partySetupScreen != null) ||
                    ((state.Mode == GameMode.Defeat || state.Mode == GameMode.Victory) && endStateScreen != null) ||
                    gameplayOverlayHandledByUnityUi ||
                    combatHudHandledByUnityUi);
                if (!handledByUnityUi)
                {
                    DrawRect(new Rect(0, 0, Screen.width, Screen.height), bg);
                }
                if (state == null)
                {
                    TryRecoverMuster();
                }

                if (state == null)
                {
                    DrawStartupSplash(string.IsNullOrEmpty(launchError) ? "Recovering the muster..." : "Startup needs attention", false);
                    return;
                }

                if (ShouldShowStartupSplash())
                {
                    DrawStartupSplash(launchStatus, false);
                    return;
                }

                // Gameplay boards still use IMGUI, which Unity paints over root uGUI
                // canvases. A migrated modal therefore takes full-frame ownership;
                // otherwise a healthy popup can exist and receive input while being
                // completely hidden behind the board.
                if (gameplayOverlayHandledByUnityUi) return;

                switch (state.Mode)
                {
                    case GameMode.Tavern:
                        if (tavernScreen == null) DrawStartupSplash("Tavern UI is recovering...", false);
                        if (bannerToastScreen == null) DrawBanner();
                        return;
                    case GameMode.Muster:
                        if (partySetupScreen == null) DrawStartupSplash("Party setup UI is recovering...", false);
                        if (bannerToastScreen == null) DrawBanner();
                        return;
                    case GameMode.Explore:
                        DrawExplore();
                        DrawEmergencyExplorationHudFallback();
                        break;
                    case GameMode.Combat:
                        bool combatModalOpen = IsCombatModalOpen();
                        DrawCombat();
                        if (!combatModalOpen) DrawCombatDebugOverlay();
                        DrawEmergencyCombatHudFallback();
                        DrawEmergencyCombatAbilityModalFallback();
                        break;
                    case GameMode.Defeat:
                        if (endStateScreen != null)
                        {
                            if (bannerToastScreen == null) DrawBanner();
                            return;
                        }
                        DrawGameChrome("Defeat");
                        DrawDefeat();
                        DrawSidePanels();
                        break;
                    case GameMode.Victory:
                        if (endStateScreen != null)
                        {
                            if (bannerToastScreen == null) DrawBanner();
                            return;
                        }
                        DrawGameChrome("Victory");
                        DrawVictory();
                        break;
                }

                DrawEmergencyDialogueFallback();
                if (bannerToastScreen == null) DrawBanner();
            }
            catch (Exception ex)
            {
                launchError = ex.Message;
                Debug.LogException(ex);
                DrawLaunchError(ex);
            }
        }

        private bool IsCombatModalOpen()
        {
            return CurrentUiOverlay() == UiOverlay.AbilityPicker;
        }

        private bool HasRenderableGameplayOverlay(UiOverlay overlay)
        {
            switch (overlay)
            {
                case UiOverlay.Dialogue:
                    return dialogueScreen != null && dialogueScreen.CanOwnModal;
                case UiOverlay.AbilityPicker:
                    return combatAbilityModalScreen != null && combatAbilityModalScreen.CanOwnModal;
                case UiOverlay.Armory:
                    return armoryOverlayScreen != null
                        && armoryOverlayScreen.IsVisible
                        && armoryOverlayScreen.HasRenderableGeometry;
                case UiOverlay.Loot:
                    return lootPopupScreen != null
                        && lootPopupScreen.IsVisible
                        && lootPopupScreen.HasRenderableGeometry;
                case UiOverlay.Pause:
                    return pauseMenuScreen != null
                        && pauseMenuScreen.IsVisible
                        && pauseMenuScreen.HasRenderableGeometry;
                case UiOverlay.Help:
                    return helpOverlayScreen != null
                        && helpOverlayScreen.IsVisible
                        && helpOverlayScreen.HasRenderableGeometry;
                default:
                    return false;
            }
        }

        private void TryRecoverMuster()
        {
            try
            {
                launchStatus = "Rebuilding the muster...";
                NewMuster();
                launchStatus = "Muster ready.";
            }
            catch (Exception ex)
            {
                launchError = ex.Message;
                Debug.LogException(ex);
            }
        }

        private bool ShouldShowStartupSplash()
        {
            if (string.IsNullOrEmpty(launchError)) return false;
            if (splashClockStarted && Event.current != null && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.KeyDown))
            {
                splashStartedAt = Time.realtimeSinceStartup - 6f;
                return false;
            }

            return IsStartupSplashVisible();
        }

        private bool IsStartupSplashVisible()
        {
            if (string.IsNullOrEmpty(launchError)) return false;
            return !splashClockStarted || Time.realtimeSinceStartup - splashStartedAt < 6.0f;
        }

        private void SaveGame()
        {
            if (BlockPersistenceDuringCombatResolution()) return;
            try
            {
                PrepareStateForPersistence();
                if (!SaveService.TrySaveCampaignState(SavePath(), state, labSaveBlocked, out string blockedReason))
                {
                    PushLog(blockedReason, Tone.Warn);
                    ShowBanner("Lab not saved");
                    PlaySfx("ui", 0.45f);
                    return;
                }

                PushLog("The current oath is saved.", Tone.Good);
                InvalidateSavedGameCache();
                ShowBanner("Saved");
                PlaySfx("save");
            }
            catch (Exception ex)
            {
                PushLog("Save failed: " + ex.Message, Tone.Warn);
            }
        }

        private void PrepareStateForPersistence()
        {
            if (state == null) return;
            state.ContentSetId = ContentSetCatalog.NormalizeContentSetId(activeContentSet);
            NormalizeWeaponEnchantments();
            EnsureInventoryEquipmentLinks();
            if (state.Map != null)
            {
                EnsureExploreSurfaceData(state.Map, state.SaveVersion);
                if (!ExplorationSurfaceRules.HasValidGrid(state.Map)) throw new InvalidDataException("Exploration surface data is inconsistent.");
            }
            state.SaveVersion = SaveVersion;
        }

        private void AutosaveCheckpoint(string reason)
        {
            if (!CampaignCheckpointRules.ShouldWrite(state, labSaveBlocked, Application.isBatchMode)) return;
            try
            {
                PrepareStateForPersistence();
                if (!SaveService.TrySaveCampaignState(SavePath(), state, false, out string blockedReason))
                {
                    Debug.LogWarning($"{VersionInfo.ProductName} checkpoint skipped ({reason}): {blockedReason}");
                    return;
                }

                InvalidateSavedGameCache();
                Debug.Log($"{VersionInfo.ProductName} checkpoint saved: {reason}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"{VersionInfo.ProductName} checkpoint failed ({reason}): {ex.Message}");
            }
        }

        private void LoadGame()
        {
            if (BlockPersistenceDuringCombatResolution()) return;
            try
            {
                string path = SavePath();
                if (!SaveService.SaveExists(path))
                {
                    PushLog("No saved oath is present.", Tone.Warn);
                    ShowBanner("No save found");
                    PlaySfx("ui", 0.45f);
                    return;
                }
                GameState loaded = ReadSavedGameState(path);
                if (loaded == null) throw new InvalidDataException("Saved oath is unreadable.");
                if (!IsSupportedSaveVersion(loaded.SaveVersion)) throw new InvalidDataException($"This beta scaffold needs a supported v17-v{SaveVersion} save.");
                int sourceSaveVersion = loaded.SaveVersion;
                string repairedContentSet = ContentSetCatalog.RepairLoadedContentSetId(loaded, out bool contentSetRepaired, out string contentSetRepairNote);
                GameState previous = state;
                string previousContentSet = activeContentSet;
                bool previousLabSaveBlocked = labSaveBlocked;
                bool previousBetaLabMode = betaLabMode;
                try
                {
                    activeContentSet = repairedContentSet;
                    loaded.ContentSetId = repairedContentSet;
                    CloseTransientOverlays();
                    state = loaded;
                    InvalidateControllerCaches();
                    NormalizeGameSettings();
                    EnsurePartyCustomization();
                    EnsureWorldState(sourceSaveVersion);
                    if (state.Party == null || state.Party.Count == 0) throw new InvalidDataException("Saved party is empty.");
                    if (state.Mode == GameMode.Explore && state.Map == null) throw new InvalidDataException("Saved exploration map is missing.");
                    EnsureCombatTurnState();
                    if (state.Mode == GameMode.Explore && state.Map != null)
                    {
                        EnsureWorldLandmarks();
                        EnsureExploreSurfaceData(state.Map, sourceSaveVersion);
                        if (!ExplorationSurfaceRules.HasValidGrid(state.Map)) throw new InvalidDataException("Saved exploration surface data could not be repaired.");
                        RepairPlayerExplorationPosition();
                        lastExploreRegion = ExploreRegionName(state.PlayerX, state.PlayerY);
                    }
                    rng = new System.Random(state.Seed + state.Depth * 101);
                    state.SaveVersion = SaveVersion;
                    betaLabMode = false;
                    labSaveBlocked = false;
                }
                catch
                {
                    state = previous;
                    InvalidateControllerCaches();
                    activeContentSet = previousContentSet;
                    labSaveBlocked = previousLabSaveBlocked;
                    betaLabMode = previousBetaLabMode;
                    throw;
                }
                if (contentSetRepaired && !string.IsNullOrWhiteSpace(contentSetRepairNote)) PushLog(contentSetRepairNote, Tone.Warn);
                PushLog("The saved oath is restored.", Tone.Good);
                InvalidateSavedGameCache();
                ShowBanner("Loaded");
                PlaySfx("save");
            }
            catch (Exception ex)
            {
                PushLog("Load failed: " + ex.Message, Tone.Warn);
                ShowBanner("Load failed");
                PlaySfx("hit", 0.45f);
            }
        }

        private bool BlockPersistenceDuringCombatResolution()
        {
            if (!IsCombatResolutionPending()) return false;
            PushLog("Let the current power finish resolving.", Tone.Warn);
            ShowBanner("Power resolving");
            PlaySfx("ui", 0.35f);
            return true;
        }

        private GameState ReadSavedGameState(string path)
        {
            GameState loaded = SaveService.LoadGameState(
                path,
                IsLoadCandidateValid,
                out bool usedBackup);
            if (usedBackup) PushLog("Primary save was unavailable or invalid; restored backup.", Tone.Warn);
            return loaded;
        }

        private bool IsLoadCandidateValid(GameState candidate)
        {
            return SaveCandidateRules.IsLoadable(candidate, SaveVersion);
        }

        private static bool IsSupportedSaveVersion(int version)
        {
            return version >= 17 && version <= SaveVersion;
        }

        private void SetActiveContentSet(string contentSet)
        {
            activeContentSet = ContentSetCatalog.NormalizeContentSetId(contentSet);
            if (state != null) state.ContentSetId = activeContentSet;
            MarkUiDirty();
        }

        private void CloseTransientOverlays()
        {
            showArmory = false;
            showPauseMenu = false;
            showHelpOverlay = false;
            pauseSettingsOpen = false;
            pauseConfirmAction = "";
            showTavernSettings = false;
            showTavernTesting = false;
            showSpellbook = false;
            showAbilityPanel = false;
            ClearFormulaEntry();
            ClearAbilityEntry();
            showDialogue = false;
            lootPanelTitle = "";
            lootPanelBody = "";
            lootPanelTraitLine = "";
            lootPanelEquipNote = "";
            lootPanelItem = null;
            lootPanelGold = 0;
            lootPanelSupplies = 0;
            lootPanelElixirs = 0;
            lootPanelUntil = 0f;
            lootPanelRequiresDismissal = false;
            ClearQueuedDialogueLoot();
            MarkUiDirty();
        }

        private void EnsureWorldState(int sourceSaveVersion = SaveVersion)
        {
            if (state == null) return;
            if (state.StoryChapter <= 0) state.StoryChapter = Mathf.Max(1, state.Depth);
            if (string.IsNullOrEmpty(state.ActiveStory)) state.ActiveStory = StoryObjectiveForDepth(Mathf.Max(1, state.Depth));
            if (state.StoryFlags == null) state.StoryFlags = new List<string>();
            if (state.DiscoveredZones == null) state.DiscoveredZones = new List<string>();
            if (state.RoamingThreats == null) state.RoamingThreats = new List<RoamingThreat>();
            if (state.Inventory == null) state.Inventory = new List<InventoryItem>();
            NormalizeWeaponEnchantments(sourceSaveVersion);
            EnsureInventoryEquipmentLinks();
            state.ExplorationSteps = Mathf.Max(0, state.ExplorationSteps);
            if (state.Map != null && state.Map.Depth <= 0) state.Map.Depth = Mathf.Max(1, state.Depth);
            if (state.Map != null)
            {
                EnsureExploreSurfaceData(state.Map, sourceSaveVersion);
                state.ActiveRouteWaypointKey = RouteChartRules.RepairWaypointKey(
                    WorldMapGenerationRules.RegionalJunctions(
                        state.Map.Width,
                        state.Map.Height,
                        state.Map.StartX,
                        state.Map.StartY),
                    state.DiscoveredZones,
                    state.Depth,
                    state.ActiveRouteWaypointKey);
            }
            else
            {
                state.ActiveRouteWaypointKey = "";
            }
            if (state.Depth == 2 && state.Map != null && !HasStoryFlag(StoryFlags.KoboldKingDefeated)) EnsureKoboldKingCaveMarker();
        }

        private string SavePath()
        {
            string currentPath = SaveService.SavePath(Application.persistentDataPath);
            try
            {
                DirectoryInfo currentProductDirectory = Directory.GetParent(Application.persistentDataPath);
                if (currentProductDirectory != null)
                {
                    string legacyDirectory = Path.Combine(currentProductDirectory.FullName, VersionInfo.LegacyProductName);
                    string legacyPath = SaveService.LegacySavePath(legacyDirectory);
                    if (SaveService.TryImportLegacySave(currentPath, legacyPath))
                    {
                        Debug.Log($"{VersionInfo.ProductName} imported a save from {VersionInfo.LegacyProductName}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"{VersionInfo.ProductName} could not inspect the legacy save location: {ex.Message}");
            }
            return currentPath;
        }

        private void ContextHelp()
        {
            if (state == null) return;
            if (showHelpOverlay)
            {
                CloseHelpOverlay();
                return;
            }

            OpenHelpOverlay();
            ShowBanner("Help");
        }

        private string Initials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            string cleaned = name.Trim();
            return cleaned.Length == 1 ? cleaned.ToUpperInvariant() : cleaned.Substring(0, 2).ToUpperInvariant();
        }

        private void PushLog(string text, Tone tone)
        {
            if (state?.Log == null) return;
            state.Log.Insert(0, new LogEntry { Text = text, Tone = tone });
            if (state.Log.Count > 50) state.Log.RemoveRange(50, state.Log.Count - 50);
            MarkUiDirty();
        }

        private static Color Hex(string hex, float alpha = 1f)
        {
            if (hex.StartsWith("#")) hex = hex.Substring(1);
            byte r = Convert.ToByte(hex.Substring(0, 2), 16);
            byte g = Convert.ToByte(hex.Substring(2, 2), 16);
            byte b = Convert.ToByte(hex.Substring(4, 2), 16);
            return new Color32(r, g, b, (byte)Mathf.RoundToInt(alpha * 255f));
        }

        private static Color VividColor(Color color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            Color vivid = Color.HSVToRGB(h, Mathf.Clamp01(s * 1.28f + 0.08f), Mathf.Clamp01(v * 1.14f + 0.06f));
            vivid.a = color.a;
            return vivid;
        }
    }

    public static class PresentationRefreshRules
    {
        public static string ComposeKey(int screenWidth, int screenHeight, int revision, string localKey)
        {
            return screenWidth.ToString() + "x" + screenHeight.ToString() + "|" + revision.ToString() + "|" + (localKey ?? "");
        }

        public static bool KeyChanged(string previousKey, string nextKey)
        {
            return !string.Equals(previousKey ?? "", nextKey ?? "", StringComparison.Ordinal);
        }
    }
}
