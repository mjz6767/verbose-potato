using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class RuleSmokeTests
    {
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " rule smoke tests passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " rule smoke tests failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunExplorationArtRules()
        {
            try
            {
                ExplorationArtRulesMapSemanticTilesAndScale();
                Debug.Log(VersionInfo.ProductName + " exploration-art rule smoke tests passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " exploration-art rule smoke tests failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunRoamingThreatRules()
        {
            try
            {
                RoamingThreatsTelegraphAndPathAroundTerrain();
                Debug.Log(VersionInfo.ProductName + " roaming-threat rule smoke tests passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " roaming-threat rule smoke tests failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            UiRuntimeDefaultFontResolves();
            AuthoredSfxAssetsMatchRuntimeCueContracts();
            OriginalMusicAssetsMatchRuntimeCueContracts();
            StandaloneBatchmodeQuitPredicateIsScopedToPlayerSmoke();
            VisualSmokeCaptureAcceptanceIsStrictAndDeterministic();
            StarterPartyCatalogDefinesFourIntentionalRoles();
            SelectableClassesHaveViableSewerSliceStarterKits();
            BannerToastScreenLayoutFitsSupportedResolutions();
            CombatRoundPresentationCopyAndTimingStayBounded();
            TavernMenuRulesKeepNormalOpeningPlayerFacing();
            CampaignCheckpointRulesProtectPlayerProgress();
            VisualSmokeLaunchRulesProtectPlayerProgress();
            CombatRetreatRequiresCampaignAndSupply();
            TavernScreenLayoutFitsSupportedResolutions();
            TavernStormRegionsStayOutsideTheRoom();
            TavernTitleAnimationIsReadableAndMotionSafe();
            GrandHearthTitlePresentationIsDeterministicAndMotionSafe();
            PartySetupScreenLayoutFitsSupportedResolutions();
            ExplorationHudScreenLayoutFitsSupportedResolutions();
            ExplorationGuidanceRulesKeepTheGoldenThreadActionable();
            WorldMapGenerationRulesDefineModestExpansion();
            WorldMapGenerationRulesDefineNamedJunctionCircuit();
            WorldMapGenerationRulesDefineRegionalSites();
            WorldAreaTemplateRulesDefineDistinctRegionalSites();
            WorldSiteInteractionRulesDefineDepthScopedServices();
            WorldSitePresentationRulesDefineDistinctAudioIdentity();
            MidgaardDistrictRulesDefineAuthoredWards();
            RouteChartRulesTrackDiscoveredJunctionsAndBearings();
            ExplorationReadabilityRulesKeepGroundBehindSprites();
            WorldMapSpriteCellCoverageRejectsPruningExtremes();
            WorldMapRegionLandmarkCatalogIsSemantic();
            WorldMapRegionMarkerCatalogIsSemantic();
            WorldAreaSetpieceCatalogIsSemantic();
            V24WorldMapCharacterArtRulesAreStable();
            ExplorationMiniMapPresentationRulesReserveSemanticMarkers();
            ApprovedV130WorldMapAtlasesMatchRuntimeContracts();
            ExplorationArtRulesMapSemanticTilesAndScale();
            ExplorationSurfaceRulesPreserveAuthoredMapStructure();
            CombatHudScreenLayoutFitsSupportedResolutions();
            CombatAbilityModalLayoutFitsSupportedResolutions();
            CombatAbilityModalNavigationRepeatsPredictably();
            CombatAbilityModalCardsExposeArtIdentity();
            CombatTargetHighlightsRequireAnArmedPower();
            AdvancedCasterProgressionPowersAreExplicit();
            DemonFormPackageDefinesCohesiveProgression();
            LightningPowerRulesDefineTacticalStormLadder();
            CombatIconCatalogDefinesUniqueActiveSkillArt();
            SignatureSpellIconCatalogDefinesUniqueFormulaArt();
            ApprovedPowerIconAtlasesMatchRuntimeContracts();
            CombatFeedbackRoutingUsesSemanticArt();
            CombatPowerPresentationMakesSignaturesDistinct();
            CombatPowerVisualMotifsStaySemanticAndBounded();
            CombatUnitPresentationBeatsStaySynchronizedAndBounded();
            CombatImpactProfilesStageSignaturePowers();
            CombatFieldsUseDistinctVisualAndAudioProfiles();
            CombatImpactPresentationHasReadableEchoAndSpatialMix();
            WeaponFeedbackProfilesStayDistinctAndBounded();
            GameAudioCuesMatchWorldSurfaces();
            AdaptiveMusicDirectorRoutesDistinctContexts();
            EnemyPowerPresentationIsDistinctAndBounded();
            CombatPowerResolutionTimingIsBriefAndScaled();
            CombatPowerOutcomeReportsActualChanges();
            CombatPowerOutcomeSurfacesReactions();
            CombatPowerTargetingPreviewsExposeRealFootprints();
            PauseMenuLayoutFitsSupportedResolutions();
            HelpOverlayLayoutFitsSupportedResolutions();
            HelpOverlayContentIsModeSpecific();
            EndStateScreenLayoutFitsSupportedResolutions();
            EndStateContentIsStateSpecific();
            DialogueScreenLayoutFitsSupportedResolutions();
            DialoguePagingAndPortraitCatalogAreReadable();
            LootPopupLayoutFitsSupportedResolutions();
            ArmoryOverlayLayoutFitsSupportedResolutions();
            InventoryEquipmentRulesMakeOwnershipAndComparisonsExplicit();
            WeaponEnchantmentRulesPreserveAffinityAndDuration();
            PresentationRefreshKeysAreStableUntilStateChanges();
            BoardPointerRoutingBlocksHudAndModalClicks();
            ExplorationUseTargetPriorityIsContextual();
            ExplorationControllerOwnsMovementAndContextualUse();
            MapDataObjectLookupCacheTracksMutations();
            ExplorationTraversalBlocksObjectOverlap();
            ExplorationTraversalCertifiesConnectedTargets();
            MidgaardInteriorPortalsRemainPaired();
            GrandHearthInteriorContractIsStable();
            RoamingThreatsTelegraphAndPathAroundTerrain();
            CombatCommandPresentationExposesSixCommands();
            CombatInputRoutingKeepsHudFocusAuthoritative();
            CombatTargetingCancellationIsExplicitAndSafe();
            EnemyTacticsProfilesAreRoleAware();
            CombatThreatForecastsAreSharedAndReadable();
            CombatControllerOwnsTurnAndActionLifecycle();
            CombatRitualsHaveCounterplayAndOutcomeWeight();
            RuntimeControllersAreCachedAccessors();
            EncounterCatalogDefinesExplicitValidEncounters();
            SewerSliceContentSetDefinesCompleteFirstPlayPath();
            SewerSliceEncountersHaveConciseGuidance();
            SewerSliceFirstPlayContractProgressionIsIdempotent();
            AttackDamageProfileIncludesSharedModifiers();
            NonPhysicalAttackDamageIgnoresWarriorEnrage();
            ReachableMoveCostsRespectBlockersAndTerrainCosts();
            SupercoverLineIncludesCornerTouchCells();
            LineOfSightUsesSupercoverBlockers();
            ContentSetNormalSaveRoundTrip();
            LegacyV17SaveMigratesToFullPrototype();
            UnknownContentSetRepairsToSewerSlice();
            LabSaveDoesNotWriteCampaignFile();
            SaveServiceWritesAndFallsBackToBackup();
        }

        private static void UiRuntimeDefaultFontResolves()
        {
            AssertEqual(true, UiRuntime.DefaultFont != null, "shared UI runtime font resolves");
            AssertEqual("Fonts/LibreBaskerville-Regular", UiRuntime.DialogueFontResource, "dialogue regular font resource remains explicitly pinned");
            AssertEqual("Fonts/LibreBaskerville-SemiBold", UiRuntime.DialogueEmphasisFontResource, "dialogue emphasis font resource remains explicitly pinned");
            AssertEqual(true, Resources.Load<Font>(UiRuntime.DialogueFontResource) != null, "bundled Libre Baskerville regular font loads");
            AssertEqual(true, Resources.Load<Font>(UiRuntime.DialogueEmphasisFontResource) != null, "bundled Libre Baskerville semibold font loads");
            AssertEqual(true, UiRuntime.DialogueFont != null, "dialogue body font resolves");
            AssertEqual(true, UiRuntime.DialogueEmphasisFont != null, "dialogue emphasis font resolves");
        }

        private static void AuthoredSfxAssetsMatchRuntimeCueContracts()
        {
            string[] expectedKeys =
            {
                "aimedshot", "ambush", "ambushimpact", "arrowcontact", "arrowrain",
                "ambbell", "ambcity", "ambdrip", "ambdrum", "ambforge", "ambgate",
                "ambhearth", "ambmarket", "ambrain", "ambstone", "ambtavern", "ambwind",
                "attack", "blade", "bladecontact", "blocked", "bow", "cache", "charge",
                "chargeimpact", "counter", "curse", "death", "defeat", "dialogueclose",
                "dialogueopen", "dialoguepage", "door", "doorroyal", "doorwood",
                "encounter", "eviscerate", "eviscerateimpact", "execute", "executeimpact",
                "fieldcurse", "fieldfire", "fieldgas", "fieldholy", "fieldice", "fieldsnare",
                "fireball", "footearth", "footstone", "footwater", "footwood", "formula",
                "gatebarred", "gateopen", "guard", "heal", "heavycontact", "hit", "ice",
                "impactflesh", "impactleather", "impactmail", "impactplate", "impactshield",
                "light", "meteor", "miss", "pinning", "poison", "save", "servicearmor",
                "servicecoin", "serviceenchant", "serviceweapon", "shock", "shopbell",
                "shrine", "spellrelease", "stonecontact", "swing", "swingheavy", "tempest",
                "thronechime", "thrustcontact", "tree", "turn", "ui", "victory", "volley",
                "ward", "wayfind", "web", "whirlwind", "whirlwindimpact", "woodcontact",
                "castmend", "castlight", "castember", "castfrost", "castshock", "castnature", "casthex", "castpact",
                "castdeathburst", "deathburst", "castgreatersummon", "greatersummon",
                "castascendance", "ascendance", "casttempest", "castveil", "veilstep", "castseal", "riftseal",
                "castshimmer", "impactlow", "resonance",
                "riftpounce", "riftpounceimpact", "abyssalwhirl", "abyssalwhirlimpact",
                "soulrend", "soulrendimpact", "dreadroar", "dreadroarimpact",
                "uiopen", "uiclose", "uiconfirm", "uitab", "itemequip", "itemtake", "elixir", "rest", "levelup",
                "ambgrove", "ambfen", "ambglass", "ambruin", "ambcave", "ambcamp",
                "footglass", "footmud", "footash", "footgravel"
            };
            string[] paths = AssetDatabase.FindAssets(
                    "t:AudioClip",
                    new[] { "Assets/Resources/Audio/Sfx" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();
            string[] keys = paths
                .Select(Path.GetFileNameWithoutExtension)
                .Select(key => key.ToLowerInvariant())
                .ToArray();

            AssertEqual(expectedKeys.Length, paths.Length, "authored SFX bank has the curated cue count");
            AssertEqual(expectedKeys.Length, keys.Distinct(StringComparer.Ordinal).Count(), "authored SFX filenames are unique cue keys");
            AssertEqual(
                true,
                expectedKeys.OrderBy(key => key, StringComparer.Ordinal).SequenceEqual(keys.OrderBy(key => key, StringComparer.Ordinal)),
                "authored SFX filenames match known runtime cue keys");

            foreach (string path in paths)
            {
                AssertEqual(".wav", Path.GetExtension(path).ToLowerInvariant(), path + " ships as a lossless short WAV");
                AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
                AssertEqual(true, importer != null, path + " has an audio importer");
                AssertEqual(true, importer.forceToMono, path + " is forced to mono");
                AssertEqual(false, importer.loadInBackground, path + " loads synchronously");
                AudioImporterSampleSettings settings = importer.defaultSampleSettings;
                AssertEqual(AudioClipLoadType.DecompressOnLoad, settings.loadType, path + " decompresses on load");
                AssertEqual(AudioCompressionFormat.PCM, settings.compressionFormat, path + " keeps transient detail as PCM");
                AssertEqual(AudioSampleRateSetting.PreserveSampleRate, settings.sampleRateSetting, path + " preserves its mastered sample rate");
                AssertEqual(true, settings.preloadAudioData, path + " preloads audio data");

                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                AssertEqual(true, clip != null, path + " loads as an AudioClip");
                AssertEqual(1, clip.channels, path + " is mono");
                AssertEqual(48000, clip.frequency, path + " is mastered at 48 kHz");
                AssertEqual(true, clip.length > 0.08f && clip.length <= 1.84f, path + " stays inside the short-cue duration budget");
            }
        }

        private static void OriginalMusicAssetsMatchRuntimeCueContracts()
        {
            string[] expectedKeys =
            {
                "ashes_on_the_road_defeat_loop",
                "a_fire_between_roads_loop",
                "ash_fen_haze_loop",
                "banners_before_the_crown_loop",
                "bells_over_temple_square_loop",
                "bones_beneath_stone_loop",
                "combat_battle_pulse_loop",
                "crooked_crown_kobold_king_loop",
                "crown_and_ashes_boss_loop",
                "drow_nightblades_loop",
                "dusk_market_ambush_loop",
                "embers_carry_home_victory_loop",
                "footsteps_behind_loop",
                "glass_and_quiet_stars_loop",
                "glass_warrens_shimmer_loop",
                "gloam_courts_echo_loop",
                "green_shrine_teal_loop",
                "kobold_hide_drums_loop",
                "lanterns_and_ledgers_loop",
                "last_lamps_east_loop",
                "midgaard_lamps_loop",
                "midgaard_merchant_hall_loop",
                "midgaard_throne_room_loop",
                "mouth_of_the_deep_loop",
                "muster_by_firelight_loop",
                "names_worn_away_loop",
                "old_green_prayer_loop",
                "old_quarry_stone_loop",
                "old_road_walk_loop",
                "one_more_turn_last_stand_loop",
                "ratfolk_plague_march_loop",
                "red_gate_omen_loop",
                "red_rift_war_loop",
                "roots_remember_loop",
                "salt_cistern_drips_loop",
                "sewer_hunt_combat_loop",
                "sigils_crossed_arcane_duel_loop",
                "smoke_across_the_road_loop",
                "steel_against_the_chosen_loop",
                "tavern_storm_hearth_ensemble_loop",
                "the_rift_walks_demon_lord_loop",
                "under_the_bellstone_loop",
                "watchfires_on_the_wall_loop",
                "wet_cobble_reel_loop"
            };
            string[] paths = AssetDatabase.FindAssets(
                    "t:AudioClip",
                    new[] { "Assets/Resources/Audio/Music" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();
            string[] keys = paths
                .Select(Path.GetFileNameWithoutExtension)
                .Select(key => key.ToLowerInvariant())
                .ToArray();

            AssertEqual(expectedKeys.Length, paths.Length, "original music bank covers every routed score context");
            AssertEqual(expectedKeys.Length, keys.Distinct(StringComparer.Ordinal).Count(), "original music filenames are unique clip names");
            AssertEqual(
                true,
                expectedKeys.OrderBy(key => key, StringComparer.Ordinal).SequenceEqual(keys.OrderBy(key => key, StringComparer.Ordinal)),
                "original music filenames match the runtime score names");

            foreach (string path in paths)
            {
                AssertEqual(".wav", Path.GetExtension(path).ToLowerInvariant(), path + " keeps a lossless source master");
                AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
                AssertEqual(true, importer != null, path + " has an audio importer");
                AssertEqual(false, importer.forceToMono, path + " preserves the stereo master");
                AssertEqual(true, importer.loadInBackground, path + " permits background music loading");
                AudioImporterSampleSettings settings = importer.defaultSampleSettings;
                AssertEqual(AudioClipLoadType.CompressedInMemory, settings.loadType, path + " stays compact in runtime memory");
                AssertEqual(AudioCompressionFormat.Vorbis, settings.compressionFormat, path + " uses music-appropriate compression");
                AssertEqual(AudioSampleRateSetting.PreserveSampleRate, settings.sampleRateSetting, path + " preserves the 32 kHz master");
                AssertEqual(true, settings.preloadAudioData, path + " is ready for immediate crossfades");

                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                AssertEqual(true, clip != null, path + " loads as an AudioClip");
                AssertEqual(2, clip.channels, path + " is stereo");
                AssertEqual(32000, clip.frequency, path + " is mastered at 32 kHz");
                float maximumDuration = string.Equals(
                    clip.name,
                    "tavern_storm_hearth_ensemble_loop",
                    StringComparison.Ordinal)
                    ? 40f
                    : 30.1f;
                AssertEqual(
                    true,
                    clip.length >= 15f && clip.length <= maximumDuration,
                    path + " stays inside its authored loop-duration budget");
            }
        }

        private static void CombatIconCatalogDefinesUniqueActiveSkillArt()
        {
            List<string> abilityIds = new[] { "warrior", "rogue", "ranger", "demon" }
                .SelectMany(AbilityCatalog.IdsForClass)
                .ToList();
            List<int> iconIndices = abilityIds.Select(CombatIconCatalog.AbilityIndex).ToList();

            AssertEqual(22, abilityIds.Count, "active martial and demon-form ability count");
            AssertEqual(true, iconIndices.All(index => index >= 0), "every active martial ability has icon art");
            AssertEqual(abilityIds.Count, iconIndices.Distinct().Count(), "active martial ability icons are unique");
            AssertEqual(19, CombatIconCatalog.AbilityIndex("smokebomb"), "expanded smoke bomb icon cell");
            AssertEqual("20,21,22,23", string.Join(",", AbilityCatalog.IdsForClass("demon").Select(CombatIconCatalog.AbilityIndex)), "demon-form art occupies the appended atlas row");
            AssertEqual(6, CombatIconCatalog.AbilityAtlasRows(1024, 1536), "expanded ability atlas rows");
            AssertEqual(true, CombatIconCatalog.IsAbilityAtlasDimensions(1024, 1536), "exact expanded ability atlas dimensions are accepted");
            AssertEqual(false, CombatIconCatalog.IsAbilityAtlasDimensions(1024, 1280), "pre-demon ability atlas dimensions are rejected");
            AssertEqual(false, CombatIconCatalog.IsAbilityAtlasDimensions(1448, 1086), "legacy irregular ability atlas dimensions are rejected");
            AssertEqual(false, CombatIconCatalog.IsAbilityAtlasDimensions(1024, 512), "legacy 4x2 ability atlas dimensions are rejected");

            AssertEqual(5, CombatIconCatalog.CombatCommandAtlasColumns, "combat command atlas columns");
            AssertEqual(4, CombatIconCatalog.CombatCommandAtlasRows, "combat command atlas rows");
            AssertEqual(256, CombatIconCatalog.CombatCommandAtlasCellSize, "combat command atlas cell size");
            AssertEqual(1280, CombatIconCatalog.CombatCommandAtlasWidth, "combat command atlas width");
            AssertEqual(1024, CombatIconCatalog.CombatCommandAtlasHeight, "combat command atlas height");
            AssertEqual(true, CombatIconCatalog.IsCombatCommandAtlasDimensions(1280, 1024), "exact combat command atlas dimensions are accepted");
            AssertEqual(false, CombatIconCatalog.IsCombatCommandAtlasDimensions(1448, 1086), "irregular v0.73 combat command atlas dimensions are rejected");
            AssertEqual(false, CombatIconCatalog.IsCombatCommandAtlasDimensions(1024, 1280), "transposed combat command atlas dimensions are rejected");
            AssertEqual(
                "0,1,2,7,3,4,5",
                string.Join(",", new[]
                {
                    CombatIconCatalog.CombatCommandMoveIndex,
                    CombatIconCatalog.CombatCommandAttackIndex,
                    CombatIconCatalog.CombatCommandCastIndex,
                    CombatIconCatalog.CombatCommandSkillsIndex,
                    CombatIconCatalog.CombatCommandGuardIndex,
                    CombatIconCatalog.CombatCommandElixirIndex,
                    CombatIconCatalog.CombatCommandEndTurnIndex
                }),
                "combat command semantics preserve the live row-major contract");

            AssertEqual(4, CombatIconCatalog.BookStateAtlasColumns, "power-book state atlas columns");
            AssertEqual(3, CombatIconCatalog.BookStateAtlasRows, "power-book state atlas rows");
            AssertEqual(64, CombatIconCatalog.BookStateAtlasCellSize, "power-book state atlas cell size");
            AssertEqual(256, CombatIconCatalog.BookStateAtlasWidth, "power-book state atlas width");
            AssertEqual(192, CombatIconCatalog.BookStateAtlasHeight, "power-book state atlas height");
            AssertEqual(
                "0,1,2,3,4,5,6,7,8,9,10,11",
                string.Join(",", new[]
                {
                    CombatIconCatalog.BookStateSelectionIndex,
                    CombatIconCatalog.BookStateTargetingIndex,
                    CombatIconCatalog.BookStateLockedIndex,
                    CombatIconCatalog.BookStateLowResourceIndex,
                    CombatIconCatalog.BookStateNoTargetIndex,
                    CombatIconCatalog.BookStateActionUsedIndex,
                    CombatIconCatalog.BookStateDisabledIndex,
                    CombatIconCatalog.BookStateBlockedIndex,
                    CombatIconCatalog.BookStateCostIndex,
                    CombatIconCatalog.BookStateReachIndex,
                    CombatIconCatalog.BookStateTargetIndex,
                    CombatIconCatalog.BookStatePreviewIndex
                }),
                "power-book state semantics preserve the authored row-major contract");
            AssertEqual(true, CombatIconCatalog.IsBookStateAtlasDimensions(256, 192), "exact power-book state atlas dimensions are accepted");
            AssertEqual(false, CombatIconCatalog.IsBookStateAtlasDimensions(192, 256), "transposed power-book state atlas dimensions are rejected");
        }

        private static void SignatureSpellIconCatalogDefinesUniqueFormulaArt()
        {
            string[] codes = FormulaCatalog.All.Select(formula => formula.Code).ToArray();
            List<int> indices = codes.Select(CombatIconCatalog.SignatureSpellIndex).ToList();
            AssertEqual(51, codes.Length, "full prototype formula count");
            AssertEqual(true, indices.All(index => index >= 0), "every spellbook formula has dedicated icon art");
            AssertEqual(codes.Length, indices.Distinct().Count(), "every spellbook formula icon is unique");
            AssertEqual(
                string.Join(",", Enumerable.Range(0, codes.Length)),
                string.Join(",", indices),
                "formula catalog order is the stable row-major spell icon contract");
            AssertEqual(7, CombatIconCatalog.SignatureSpellAtlasColumns, "expanded spell icon atlas columns");
            AssertEqual(8, CombatIconCatalog.SignatureSpellAtlasRows, "expanded spell icon atlas rows");
            AssertEqual("49,50", string.Join(",", new[] { CombatIconCatalog.SignatureSpellIndex("RBT"), CombatIconCatalog.SignatureSpellIndex("VRS") }), "pact gap spells occupy appended signature cells");
            AssertEqual(-1, CombatIconCatalog.SignatureSpellIndex("UNKNOWN"), "unknown spell has no signature cell");
            string[] lightningCodes = { "RIG", "CLT", "RSG", "AST", "VST" };
            int[] lightningIndices = lightningCodes
                .Select(LightningSpellIconCatalog.LightningIndex)
                .ToArray();
            AssertEqual("0,1,5,3,4", string.Join(",", lightningIndices), "lightning formulas own stable dedicated atlas cells");
            AssertEqual(5, lightningIndices.Distinct().Count(), "lightning spell icons are unique");
            AssertEqual(true, lightningCodes.All(code => FormulaCatalog.All.Any(formula => formula.Code == code)), "lightning art codes resolve to formulas");
            AssertEqual(-1, LightningSpellIconCatalog.LightningIndex("UNKNOWN"), "unknown spell has no lightning cell");
        }

        private static void ApprovedPowerIconAtlasesMatchRuntimeContracts()
        {
            Texture2D ability = null;
            Texture2D spell = null;
            Texture2D lightning = null;
            Texture2D powerBookState = null;
            Texture2D combatCommand = null;
            Texture2D magic = null;
            Texture2D effects = null;
            Texture2D epicEffects = null;
            try
            {
                ability = LoadApprovedRuntimeAtlas(RuntimeArtManifest.AbilityIconAtlas);
                spell = LoadApprovedRuntimeAtlas(RuntimeArtManifest.SignatureSpellIconAtlas);
                lightning = LoadApprovedRuntimeAtlas(RuntimeArtManifest.LightningSpellIconAtlas);
                powerBookState = LoadApprovedRuntimeAtlas(RuntimeArtManifest.PowerBookStateIconAtlas);
                combatCommand = LoadApprovedRuntimeAtlas(RuntimeArtManifest.CombatCommandIconAtlas);
                magic = LoadApprovedRuntimeAtlas(RuntimeArtManifest.MagicUiAtlas);
                effects = LoadApprovedRuntimeAtlas(RuntimeArtManifest.SpellAnimationAtlas);
                epicEffects = LoadApprovedRuntimeAtlas(RuntimeArtManifest.EpicSpellEffectsAtlas);
                AssertEqual(new Vector2Int(1024, 1536), new Vector2Int(ability.width, ability.height), "approved ability atlas dimensions");
                AssertEqual(new Vector2Int(1792, 2048), new Vector2Int(spell.width, spell.height), "approved signature spell atlas dimensions");
                AssertEqual(new Vector2Int(1024, 512), new Vector2Int(lightning.width, lightning.height), "approved lightning spell atlas dimensions");
                AssertEqual(
                    new Vector2Int(CombatIconCatalog.BookStateAtlasWidth, CombatIconCatalog.BookStateAtlasHeight),
                    new Vector2Int(powerBookState.width, powerBookState.height),
                    "approved power-book state atlas dimensions");
                AssertEqual(
                    new Vector2Int(CombatIconCatalog.CombatCommandAtlasWidth, CombatIconCatalog.CombatCommandAtlasHeight),
                    new Vector2Int(combatCommand.width, combatCommand.height),
                    "approved combat command atlas dimensions");
                AssertEqual(new Vector2Int(1024, 1024), new Vector2Int(magic.width, magic.height), "approved magic UI atlas dimensions");
                AssertEqual(new Vector2Int(1276, 1276), new Vector2Int(effects.width, effects.height), "approved spell animation atlas dimensions");
                AssertEqual(new Vector2Int(1254, 1254), new Vector2Int(epicEffects.width, epicEffects.height), "approved epic spell effects atlas dimensions");
                AssertAtlasCellCoverage(ability, 4, 6, Enumerable.Range(0, 24), 0.10f, 0.90f, "approved ability icon");
                AssertAtlasCellCoverage(
                    spell,
                    CombatIconCatalog.SignatureSpellAtlasColumns,
                    CombatIconCatalog.SignatureSpellAtlasRows,
                    Enumerable.Range(0, FormulaCatalog.All.Length),
                    0.10f,
                    0.90f,
                    "approved signature spell icon");
                AssertAtlasCellCoverage(lightning, LightningSpellIconCatalog.AtlasColumns, LightningSpellIconCatalog.AtlasRows, Enumerable.Range(0, 8), 0.10f, 0.90f, "approved lightning spell icon");
                AssertAtlasCellCoverage(
                    powerBookState,
                    CombatIconCatalog.BookStateAtlasColumns,
                    CombatIconCatalog.BookStateAtlasRows,
                    Enumerable.Range(0, CombatIconCatalog.BookStateAtlasColumns * CombatIconCatalog.BookStateAtlasRows),
                    0.10f,
                    0.90f,
                    "approved power-book state icon");
                AssertAtlasCellCoverage(magic, 4, 4, Enumerable.Range(0, 16), 0.10f, 0.90f, "approved magic UI icon");
                int[] commandCells =
                {
                    CombatIconCatalog.CombatCommandMoveIndex,
                    CombatIconCatalog.CombatCommandAttackIndex,
                    CombatIconCatalog.CombatCommandCastIndex,
                    CombatIconCatalog.CombatCommandGuardIndex,
                    CombatIconCatalog.CombatCommandElixirIndex,
                    CombatIconCatalog.CombatCommandEndTurnIndex,
                    CombatIconCatalog.CombatCommandSkillsIndex
                };
                AssertAtlasCellCoverage(
                    combatCommand,
                    CombatIconCatalog.CombatCommandAtlasColumns,
                    CombatIconCatalog.CombatCommandAtlasRows,
                    commandCells,
                    0.08f,
                    0.90f,
                    "approved combat command icon");
                AssertAtlasCellCoverage(effects, 4, 4, Enumerable.Range(0, 16), 0.02f, 0.92f, "approved spell animation");
                AssertAtlasCellSafeGutter(ability, 4, 6, Enumerable.Range(0, 24), 12, 8, 32, "approved ability icon");
                AssertAtlasCellSafeGutter(
                    spell,
                    CombatIconCatalog.SignatureSpellAtlasColumns,
                    CombatIconCatalog.SignatureSpellAtlasRows,
                    Enumerable.Range(0, FormulaCatalog.All.Length),
                    12,
                    8,
                    32,
                    "approved signature spell icon");
                AssertAtlasCellSafeGutter(lightning, LightningSpellIconCatalog.AtlasColumns, LightningSpellIconCatalog.AtlasRows, Enumerable.Range(0, 8), 12, 8, 32, "approved lightning spell icon");
                AssertAtlasCellSafeGutter(
                    powerBookState,
                    CombatIconCatalog.BookStateAtlasColumns,
                    CombatIconCatalog.BookStateAtlasRows,
                    Enumerable.Range(0, CombatIconCatalog.BookStateAtlasColumns * CombatIconCatalog.BookStateAtlasRows),
                    8,
                    8,
                    0,
                    "approved power-book state icon");
                AssertAtlasCellSafeGutter(magic, 4, 4, Enumerable.Range(0, 16), 12, 8, 32, "approved magic UI icon");
                AssertAtlasCellSafeGutter(
                    combatCommand,
                    CombatIconCatalog.CombatCommandAtlasColumns,
                    CombatIconCatalog.CombatCommandAtlasRows,
                    commandCells,
                    12,
                    8,
                    32,
                    "approved combat command icon");
            }
            finally
            {
                if (ability != null) UnityEngine.Object.DestroyImmediate(ability);
                if (spell != null) UnityEngine.Object.DestroyImmediate(spell);
                if (lightning != null) UnityEngine.Object.DestroyImmediate(lightning);
                if (powerBookState != null) UnityEngine.Object.DestroyImmediate(powerBookState);
                if (combatCommand != null) UnityEngine.Object.DestroyImmediate(combatCommand);
                if (magic != null) UnityEngine.Object.DestroyImmediate(magic);
                if (effects != null) UnityEngine.Object.DestroyImmediate(effects);
                if (epicEffects != null) UnityEngine.Object.DestroyImmediate(epicEffects);
            }
        }

        private static void CombatFeedbackRoutingUsesSemanticArt()
        {
            AssertEqual(10, CombatFeedbackRules.FloatIconIndex("-12", "fire"), "fire damage float icon");
            AssertEqual(11, CombatFeedbackRules.FloatIconIndex("-8", "cold"), "cold damage float icon");
            AssertEqual(2, CombatFeedbackRules.FloatIconIndex("+7"), "healing float icon");
            AssertEqual(6, CombatFeedbackRules.FloatIconIndex("bleed"), "bleed status float icon");
            AssertEqual(3, CombatFeedbackRules.FloatIconIndex("resist"), "resistance float icon");
            AssertEqual(17, CombatFeedbackRules.FloatIconIndex("spell broken"), "disruption float icon");
            AssertEqual(1f, CombatFeedbackRules.FloatAlpha(0.5f), "floating feedback holds full opacity");
            AssertEqual(0f, CombatFeedbackRules.FloatAlpha(1f), "floating feedback fades out at expiry");
            AssertEqual(4, CombatFeedbackRules.RangerImpactIndex("broadheadshot"), "broadhead uses arrow-impact art");
            AssertEqual(7, CombatFeedbackRules.RangerImpactIndex("disruptingshot"), "disrupting shot uses stun art");

            FormulaDef heal = new FormulaDef { Effect = "heal", School = "mend" };
            FormulaDef cold = new FormulaDef { DamageType = "cold" };
            FormulaDef shock = new FormulaDef { DamageType = "shock" };
            FormulaDef cure = new FormulaDef { Effect = "cure" };
            FormulaDef sanctuary = new FormulaDef { Terrain = "sanctuary" };
            FormulaDef light = new FormulaDef { DamageType = "light" };
            FormulaDef fireball = FormulaCatalog.All.First(formula => formula.Code == "FBL");
            FormulaDef basicHeal = FormulaCatalog.All.First(formula => formula.Code == "OIC");

            AssertEqual("spellanim:12", CombatFeedbackRules.SpellGlyphKind(heal, "caster"), "healing caster animation");
            AssertEqual("spellanim:11", CombatFeedbackRules.SpellGlyphKind(cold, "impact"), "cold impact animation");
            AssertEqual("spellanim:10", CombatFeedbackRules.SpellGlyphKind(shock, "impact"), "shock impact animation");
            AssertEqual("spellanim:0", CombatFeedbackRules.SpellGlyphKind(fireball, "caster"), "fireball caster begins on ember spark art");
            AssertEqual("fireball", CombatFeedbackRules.SpellGlyphKind(fireball, "fireball"), "fireball keeps staged animation");
            AssertEqual("signature:3", CombatFeedbackRules.SpellGlyphKind(basicHeal, "impact"), "signature heal uses dedicated art");
            AssertEqual(13, CombatFeedbackRules.MagicUiIconIndex(cure), "cleanse magic icon");
            AssertEqual(14, CombatFeedbackRules.MagicUiIconIndex(sanctuary), "sanctuary magic icon");
            AssertEqual(12, CombatFeedbackRules.MagicUiIconIndex(light), "light magic icon");
            AssertEqual(15, CombatFeedbackRules.MagicUiIconIndex(new FormulaDef()), "generic formula magic icon");
        }

        private static void StandaloneBatchmodeQuitPredicateIsScopedToPlayerSmoke()
        {
            MethodInfo method = typeof(AshenHallsGame).GetMethod("ShouldAutoQuitAfterBoot", BindingFlags.Static | BindingFlags.NonPublic);
            AssertEqual(true, method != null, "batchmode quit predicate exists");

            bool playerSmoke = (bool)method.Invoke(null, new object[] { new[] { "AshAndBrimstone.exe", "-batchmode", "-quit" }, true, false });
            bool editorSmoke = (bool)method.Invoke(null, new object[] { new[] { "Unity.exe", "-batchmode", "-quit" }, true, true });
            bool noQuitArg = (bool)method.Invoke(null, new object[] { new[] { "AshAndBrimstone.exe", "-batchmode" }, true, false });
            bool normalPlayer = (bool)method.Invoke(null, new object[] { new[] { "AshAndBrimstone.exe" }, false, false });

            AssertEqual(true, playerSmoke, "standalone batch smoke quits after boot");
            AssertEqual(false, editorSmoke, "editor smoke does not self-quit through runtime");
            AssertEqual(false, noQuitArg, "batch player without quit arg stays open");
            AssertEqual(false, normalPlayer, "normal player stays open");
        }

        private static void VisualSmokeCaptureAcceptanceIsStrictAndDeterministic()
        {
            CapturePixelSample[] visibleDarkScene =
            {
                new CapturePixelSample(0, 0, 0),
                new CapturePixelSample(3, 4, 5),
                new CapturePixelSample(24, 18, 12),
                new CapturePixelSample(182, 72, 36)
            };
            CaptureAcceptanceResult accepted = CaptureAcceptanceRules.Evaluate(
                1280,
                720,
                1280,
                720,
                1280,
                720,
                visibleDarkScene);
            AssertEqual(true, accepted.Accepted, "capture accepts exact dimensions with visible dark-scene variation");
            AssertEqual(CaptureAcceptanceFailure.None, accepted.Failure, "accepted capture has no rejection reason");

            CaptureAcceptanceResult wrongScreen = CaptureAcceptanceRules.Evaluate(
                1280,
                720,
                1080,
                720,
                1080,
                720,
                visibleDarkScene);
            AssertEqual(false, wrongScreen.Accepted, "capture rejects an actual window narrower than requested");
            AssertEqual(
                CaptureAcceptanceFailure.ScreenDimensionsDifferFromRequest,
                wrongScreen.Failure,
                "actual window mismatch rejection reason");

            CaptureAcceptanceResult wrongPng = CaptureAcceptanceRules.Evaluate(
                1280,
                720,
                1280,
                720,
                1080,
                720,
                visibleDarkScene);
            AssertEqual(false, wrongPng.Accepted, "capture rejects a PNG narrower than the actual window");
            AssertEqual(
                CaptureAcceptanceFailure.PngDimensionsDifferFromScreen,
                wrongPng.Failure,
                "PNG dimension mismatch rejection reason");

            CapturePixelSample[] blackFrame = Enumerable.Repeat(
                new CapturePixelSample(0, 0, 0),
                100).ToArray();
            CaptureAcceptanceResult black = CaptureAcceptanceRules.Evaluate(
                1280,
                720,
                1280,
                720,
                1280,
                720,
                blackFrame);
            AssertEqual(false, black.Accepted, "capture rejects a uniformly black frame");
            AssertEqual(CaptureAcceptanceFailure.NearUniformBlack, black.Failure, "black frame rejection reason");

            CapturePixelSample[] almostBlackFrame = Enumerable.Repeat(
                new CapturePixelSample(4, 5, 3),
                100).ToArray();
            almostBlackFrame[99] = new CapturePixelSample(20, 18, 17);
            CaptureAcceptanceResult almostBlack = CaptureAcceptanceRules.Evaluate(
                1920,
                1080,
                1920,
                1080,
                1920,
                1080,
                almostBlackFrame);
            AssertEqual(false, almostBlack.Accepted, "capture rejects a near-uniform almost-black frame");
            AssertEqual(CaptureAcceptanceFailure.NearUniformBlack, almostBlack.Failure, "almost-black rejection reason");

            CaptureAcceptanceResult noSamples = CaptureAcceptanceRules.Evaluate(
                1280,
                720,
                1280,
                720,
                1280,
                720,
                Array.Empty<CapturePixelSample>());
            AssertEqual(false, noSamples.Accepted, "capture rejects a PNG without readable pixel samples");
            AssertEqual(CaptureAcceptanceFailure.NoPixelSamples, noSamples.Failure, "empty sample rejection reason");
        }

        private static void StarterPartyCatalogDefinesFourIntentionalRoles()
        {
            List<StarterHeroDef> heroes = StarterPartyCatalog.All.ToList();

            AssertEqual(StarterPartyCatalog.ExpectedPartySize, heroes.Count, "starter party count");
            AssertEqual("warrior,ranger,mage,priest", string.Join(",", heroes.Select(hero => hero.ClassKey)), "starter class composition");
            AssertEqual(4, heroes.Select(hero => hero.Name).Distinct().Count(), "unique starter names");

            foreach (StarterHeroDef hero in heroes)
            {
                AssertEqual(50, hero.Stats.Total, hero.Name + " stat total");
            }

            StarterHeroDef ranger = heroes.First(hero => hero.ClassKey == "ranger");
            AssertEqual(true, ranger.Range >= 4, "ranger starts with ranged reach");
            AssertEqual(true, ranger.CreateSkills().Missile >= 10, "ranger missile skill");

            StarterHeroDef mage = heroes.First(hero => hero.ClassKey == "mage");
            AssertEqual("ember", mage.Spell, "mage starting spell school");
            AssertEqual(true, mage.CreateSkills().Ember >= 9, "mage ember skill");

            StarterHeroDef priest = heroes.First(hero => hero.ClassKey == "priest");
            AssertEqual("mend", priest.Spell, "priest starting spell school");
            AssertEqual(true, priest.CreateSkills().Mend >= 9, "priest mend skill");
        }

        private static void SelectableClassesHaveViableSewerSliceStarterKits()
        {
            AssertEqual(8, StarterPartyCatalog.SelectableClassKeys.Count, "selectable class count");
            foreach (string classKey in StarterPartyCatalog.SelectableClassKeys)
            {
                string spellSchool = StarterPartyCatalog.SpellSchoolForClass(classKey);
                int starterPowerCount;
                if (!string.IsNullOrWhiteSpace(spellSchool))
                {
                    starterPowerCount = FormulaCatalog.All.Count(formula =>
                        ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, formula.Code)
                        && FormulaCatalog.RequiredLevel(formula) <= 1
                        && SchoolsOverlap(formula.School, spellSchool));
                }
                else
                {
                    starterPowerCount = AbilityCatalog.IdsForClass(classKey)
                        .Select(AbilityCatalog.For)
                        .Count(ability => ability != null
                            && ability.RequiredLevel <= 1
                            && ContentSetCatalog.AbilityActive(ContentSetCatalog.SewerSlice, ability.Id));
                }

                AssertEqual(true, starterPowerCount >= 2, classKey + " has at least two sewer-slice starter powers");
            }
        }

        private static void BannerToastScreenLayoutFitsSupportedResolutions()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(1280, 720),
                new Vector2Int(1366, 768),
                new Vector2Int(1600, 900),
                new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152)
            };

            foreach (Vector2Int size in sizes)
            {
                BannerToastGeometry normal = BannerToastLayout.Calculate(size.x, size.y, false);
                AssertEqual(true, normal.Fits(size.x, size.y), $"normal banner toast layout fits {size.x}x{size.y}");
                AssertEqual(true, normal.Panel.height == 38f && normal.Panel.yMin >= 6f, $"normal banner remains compact {size.x}x{size.y}");

                BannerToastGeometry power = BannerToastLayout.Calculate(size.x, size.y, true);
                AssertEqual(true, power.Fits(size.x, size.y), $"power banner layout fits {size.x}x{size.y}");
                AssertEqual(true, power.Panel.yMin >= 6f && power.Panel.yMin <= 8.1f, $"power banner occupies top chrome {size.x}x{size.y}");
                AssertEqual(true, power.Panel.yMax <= 72.1f, $"power banner clears the top battlefield row {size.x}x{size.y}");
                AssertEqual(true, power.Phase.width >= 120f, $"power phase label remains readable {size.x}x{size.y}");
                AssertEqual(true, power.Outcome.height >= 16f, $"banner outcome line remains readable {size.x}x{size.y}");
            }

            BannerToastView timedPower = new BannerToastView
            {
                PowerCue = true,
                Intensity = 3,
                TotalSeconds = 1.50f,
                ImpactSeconds = 0.34f
            };
            timedPower.RemainingSeconds = 1.40f;
            AssertEqual("EPIC / INVOCATION", BannerToastLayout.PowerPhaseLabel(timedPower), "power strip begins with invocation");
            timedPower.RemainingSeconds = 1.25f;
            AssertEqual("EPIC / RELEASE", BannerToastLayout.PowerPhaseLabel(timedPower), "power strip names release before projectile arrival");
            timedPower.RemainingSeconds = 1.15f;
            AssertEqual("EPIC / IMPACT", BannerToastLayout.PowerPhaseLabel(timedPower), "power strip changes to impact on the canonical hit beat");
            timedPower.RemainingSeconds = 0.90f;
            AssertEqual("EPIC / AFTERMATH", BannerToastLayout.PowerPhaseLabel(timedPower), "power strip names the post-hit tail");
            AssertEqual(true, BannerToastLayout.BannerProgress(timedPower) > 0.35f, "power accent rail advances with elapsed presentation time");
        }

        private static void CombatRoundPresentationCopyAndTimingStayBounded()
        {
            CombatRoundPresentationSummary quiet =
                CombatRoundPresentationRules.Create(2, 0, 0, false);
            AssertEqual(2, quiet.Round, "round presentation preserves a valid round");
            AssertEqual("ROUND 2", quiet.BannerText, "quiet round transition stays concise");
            AssertEqual(false, quiet.HasFieldChanges, "quiet round reports no field changes");
            AssertEqual(false, quiet.HasRitualChanges, "quiet round reports no ritual changes");
            AssertEqual(
                0.56f,
                quiet.DurationSeconds,
                "normal round transition uses the standard readable hold");

            CombatRoundPresentationSummary singular =
                CombatRoundPresentationRules.Create(3, 1, 1, false);
            AssertEqual(
                "ROUND 3 \u2022 1 field fades \u2022 1 ritual opens",
                singular.BannerText,
                "round transition uses singular event grammar");
            AssertEqual(true, singular.HasFieldChanges, "field expiration is surfaced");
            AssertEqual(true, singular.HasRitualChanges, "ritual opening is surfaced");

            CombatRoundPresentationSummary plural =
                CombatRoundPresentationRules.Create(4, 2, 3, true);
            AssertEqual(
                "ROUND 4 \u2022 2 fields fade \u2022 3 rituals open",
                plural.BannerText,
                "round transition uses plural event grammar");
            AssertEqual(
                0.06f,
                plural.DurationSeconds,
                "Reduced Motion compresses the round transition");
            AssertEqual(
                true,
                plural.DurationSeconds < quiet.DurationSeconds,
                "Reduced Motion remains materially shorter than the standard hold");

            CombatRoundPresentationSummary bounded =
                CombatRoundPresentationRules.Create(int.MaxValue, int.MaxValue, int.MaxValue, false);
            AssertEqual(
                "ROUND 999+ \u2022 9+ fields fade \u2022 9+ rituals open",
                bounded.BannerText,
                "large round-transition counts use bounded display labels");
            AssertEqual(
                true,
                bounded.BannerText.Length <= CombatRoundPresentationRules.MaxBannerCharacters,
                "round-transition copy fits the compact banner budget");

            CombatRoundPresentationSummary repaired =
                CombatRoundPresentationRules.Create(-8, -3, -2, true);
            AssertEqual(1, repaired.Round, "invalid round repairs to round one");
            AssertEqual(0, repaired.ExpiredFields, "invalid field count repairs to zero");
            AssertEqual(0, repaired.OpenedRituals, "invalid ritual count repairs to zero");
            AssertEqual("ROUND 1", repaired.BannerText, "repaired transition remains player-facing");
        }

        private static void TavernMenuRulesKeepNormalOpeningPlayerFacing()
        {
            AssertEqual(3, TavernMenuRules.NormalChoiceCount(false), "tavern choices without save");
            AssertEqual("New Game,Settings,Exit Game", string.Join(",", TavernMenuRules.NormalChoiceLabels(false)), "tavern labels without save");
            AssertEqual(false, TavernMenuRules.ShowContinue(false), "continue hidden without save");

            AssertEqual(4, TavernMenuRules.NormalChoiceCount(true), "tavern choices with save");
            AssertEqual("Continue,New Game,Settings,Exit Game", string.Join(",", TavernMenuRules.NormalChoiceLabels(true)), "tavern labels with save");
            AssertEqual(true, TavernMenuRules.ShowContinue(true), "continue visible with save");

            AssertEqual(false, TavernMenuRules.ShowDeveloperTesting(false), "release build hides beta testing");
            AssertEqual(true, TavernMenuRules.ShowDeveloperTesting(true), "development build can show beta testing");
        }

        private static void CampaignCheckpointRulesProtectPlayerProgress()
        {
            GameState explore = new GameState { Mode = GameMode.Explore, Combat = null };
            AssertEqual(true, CampaignCheckpointRules.ShouldWrite(explore, false, false), "normal exploration can checkpoint");
            AssertEqual(false, CampaignCheckpointRules.ShouldWrite(null, false, false), "missing state cannot checkpoint");
            AssertEqual(false, CampaignCheckpointRules.ShouldWrite(explore, true, false), "lab state cannot checkpoint");
            AssertEqual(false, CampaignCheckpointRules.ShouldWrite(explore, false, true), "batch smoke cannot overwrite player save");

            explore.Mode = GameMode.Combat;
            AssertEqual(false, CampaignCheckpointRules.ShouldWrite(explore, false, false), "combat state cannot replace pre-fight checkpoint");
            explore.Mode = GameMode.Explore;
            explore.Combat = new CombatState();
            AssertEqual(false, CampaignCheckpointRules.ShouldWrite(explore, false, false), "dangling combat state cannot checkpoint");
        }

        private static void VisualSmokeLaunchRulesProtectPlayerProgress()
        {
            AssertEqual(
                true,
                VisualSmokeLaunchRules.BlockPersistence(new[] { "AshAndBrimstone.exe", "-ashen-capture", "frame.png" }),
                "visual capture blocks persistence");
            AssertEqual(
                true,
                VisualSmokeLaunchRules.BlockPersistence(new[] { "AshAndBrimstone.exe", "-ASHEN-CAPTURE", "frame.png" }),
                "visual capture persistence block is case insensitive");
            AssertEqual(
                true,
                VisualSmokeLaunchRules.BlockPersistence(new[] { "AshAndBrimstone.exe", "-ashen-explore-smoke" }),
                "visual smoke staging blocks persistence even without capture output");
            AssertEqual(
                false,
                VisualSmokeLaunchRules.BlockPersistence(new[] { "AshAndBrimstone.exe", "-ashen-seed", "12345" }),
                "non-smoke developer arguments do not block normal persistence");
            AssertEqual(false, VisualSmokeLaunchRules.BlockPersistence(null), "missing command line does not block persistence");
            AssertEqual(true, VisualSmokeLaunchRules.BlockLegacyImport(true, false), "visual smoke cannot import a legacy save");
            AssertEqual(true, VisualSmokeLaunchRules.BlockLegacyImport(false, true), "batch boot cannot import a legacy save");
            AssertEqual(false, VisualSmokeLaunchRules.BlockLegacyImport(false, false), "normal player launch can perform the one-time legacy import");
        }

        private static void CombatRetreatRequiresCampaignAndSupply()
        {
            GameState combat = new GameState
            {
                Mode = GameMode.Combat,
                Combat = new CombatState(),
                Supplies = CombatRetreatRules.SupplyCost
            };

            AssertEqual(true, CombatRetreatRules.CanOffer(combat, false, false), "normal campaign combat offers retreat");
            AssertEqual(true, CombatRetreatRules.CanAfford(combat), "one supply funds retreat");
            AssertEqual(false, CombatRetreatRules.CanOffer(combat, true, false), "lab-save block disables retreat");
            AssertEqual(false, CombatRetreatRules.CanOffer(combat, false, true), "beta lab disables retreat");

            combat.Supplies = 0;
            AssertEqual(false, CombatRetreatRules.CanAfford(combat), "empty supplies cannot fund retreat");
            combat.Mode = GameMode.Explore;
            AssertEqual(false, CombatRetreatRules.CanOffer(combat, false, false), "exploration does not offer combat retreat");
        }

        private static void TavernScreenLayoutFitsSupportedResolutions()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(1280, 720),
                new Vector2Int(1600, 900),
                new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152)
            };

            foreach (Vector2Int size in sizes)
            {
                foreach (bool saveExists in new[] { false, true })
                {
                    TavernScreenGeometry geometry = TavernScreenLayout.Calculate(size.x, size.y, saveExists);
                    AssertEqual(true, geometry.Fits(size.x, size.y), $"tavern layout fits {size.x}x{size.y} save={saveExists}");
                    AssertEqual(false, TitleScreenPresentationRules.Overlaps(geometry.Title, geometry.Menu, 10f), $"tavern title and menu keep a readable gutter {size.x}x{size.y} save={saveExists}");
                    AssertEqual(false, TitleScreenPresentationRules.Overlaps(geometry.Chronicle, geometry.Menu, 8f), $"tavern chronicle and menu keep a readable gutter {size.x}x{size.y} save={saveExists}");
                    AssertEqual(false, geometry.Testing.Overlaps(geometry.Menu), $"tavern testing panel does not overlap menu {size.x}x{size.y} save={saveExists}");
                    AssertEqual(TavernMenuRules.NormalChoiceCount(saveExists), TavernScreenLayout.ButtonRects(saveExists, geometry.Menu.width).Count, "tavern button count mirrors menu rules");
                    Rect testingButton = TavernScreenLayout.TestingButtonRect(geometry.Menu.width, geometry.Menu.height);
                    AssertEqual(true, testingButton.xMin >= 0f && testingButton.yMin >= 0f && testingButton.xMax <= geometry.Menu.width && testingButton.yMax <= geometry.Menu.height, $"tavern beta testing button fits menu {size.x}x{size.y} save={saveExists}");
                }
            }
        }

        private static void TavernStormRegionsStayOutsideTheRoom()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(1280, 720),
                new Vector2Int(1600, 900),
                new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152)
            };

            foreach (Vector2Int size in sizes)
            {
                IReadOnlyList<Rect> windows = TavernScreenLayout.StormWindowRects(size.x, size.y, 1672f, 941f);
                AssertEqual(4, windows.Count, $"tavern storm uses four painted-glass regions at {size.x}x{size.y}");
                for (int i = 0; i < windows.Count; i++)
                {
                    Rect window = windows[i];
                    AssertEqual(
                        true,
                        window.xMin >= size.x * 0.54f
                            && window.xMax <= size.x * 0.82f
                            && window.yMin >= 0f
                            && window.yMax <= size.y * 0.78f,
                        $"Grand Hearth weather region {i} remains within the painted storm doorway at {size.x}x{size.y}");
                    for (int other = i + 1; other < windows.Count; other++)
                    {
                        AssertEqual(
                            false,
                            window.Overlaps(windows[other]),
                            $"tavern weather regions {i} and {other} do not spill across interior timbers at {size.x}x{size.y}");
                    }
                }
            }
        }

        private static void TavernTitleAnimationIsReadableAndMotionSafe()
        {
            TavernTitleAnimationFrame opening = TavernTitleAnimationRules.Evaluate(0f, false);
            TavernTitleAnimationFrame strike = TavernTitleAnimationRules.Evaluate(0.78f, false);
            TavernTitleAnimationFrame settled = TavernTitleAnimationRules.Evaluate(4f, false);
            TavernTitleAnimationFrame reduced = TavernTitleAnimationRules.Evaluate(0f, true);

            AssertEqual(true, opening.FaceAlpha < 0.02f, "animated tavern title begins hidden");
            AssertEqual(true, opening.Scale > 1f, "animated tavern title begins slightly enlarged");
            AssertEqual(true, strike.FaceAlpha > 0.8f, "forge strike reveals the title face quickly");
            AssertEqual(true, strike.GlowAlpha > settled.GlowAlpha, "forge strike is brighter than settled title");
            AssertEqual(true, strike.UnderlineProgress > 0.5f, "forge line travels during reveal");
            AssertEqual(true, settled.FaceAlpha > 0.99f, "settled tavern title remains fully legible");
            AssertEqual(true, settled.GlowAlpha >= 0.15f && settled.GlowAlpha <= 0.22f, "settled title glow stays restrained");
            AssertEqual(true, settled.UnderlineProgress > 0.99f, "settled forge line remains complete");
            AssertEqual(1f, reduced.FaceAlpha, "reduced-motion title is immediately visible");
            AssertEqual(1f, reduced.Scale, "reduced-motion title does not scale");
            AssertEqual(1f, reduced.UnderlineProgress, "reduced-motion title uses the complete static line");
            AssertEqual(0f, reduced.EmberIntensity, "reduced-motion title disables drifting embers");

            for (int step = 0; step <= 80; step++)
            {
                TavernTitleAnimationFrame frame = TavernTitleAnimationRules.Evaluate(step * 0.1f, false);
                AssertEqual(true, frame.FaceAlpha >= 0f && frame.FaceAlpha <= 1f, "title face alpha remains bounded");
                AssertEqual(true, frame.GlowAlpha >= 0f && frame.GlowAlpha <= 0.55f, "title glow remains bounded");
                AssertEqual(true, frame.UnderlineProgress >= 0f && frame.UnderlineProgress <= 1f, "title line progress remains bounded");
                AssertEqual(true, frame.EmberIntensity >= 0f && frame.EmberIntensity <= 0.65f, "title ember intensity remains bounded");
            }
        }

        private static void GrandHearthTitlePresentationIsDeterministicAndMotionSafe()
        {
            TitleOpeningFrame opening = TitleScreenPresentationRules.Evaluate(0f, false);
            TitleOpeningFrame menuReveal = TitleScreenPresentationRules.Evaluate(1.05f, false);
            TitleOpeningFrame settled = TitleScreenPresentationRules.Evaluate(3f, false);
            TitleOpeningFrame reduced = TitleScreenPresentationRules.Evaluate(0f, true);

            AssertEqual(true, opening.BackdropReveal < 0.02f, "Grand Hearth opening begins beneath the shadow veil");
            AssertEqual(false, TitleScreenPresentationRules.MenuInteractive(opening), "hidden Grand Hearth menu cannot accept pointer or submit input");
            AssertEqual(true, menuReveal.MenuAlpha > 0.75f, "Grand Hearth menu arrives promptly after the title strike");
            AssertEqual(true, settled.ChronicleAlpha > 0.99f, "Grand Hearth chronicle settles fully legible");
            AssertEqual(true, TitleScreenPresentationRules.MenuInteractive(settled), "settled Grand Hearth menu accepts navigation input");
            AssertEqual(1f, reduced.BackdropReveal, "reduced motion reveals the Grand Hearth immediately");
            AssertEqual(1f, reduced.MenuAlpha, "reduced motion reveals the title menu immediately");
            AssertEqual(true, TitleScreenPresentationRules.MenuInteractive(reduced), "reduced motion keeps the title menu immediately usable");
            AssertEqual(0f, reduced.MenuRise, "reduced motion removes menu travel");
            AssertEqual(true, TitleScreenPresentationRules.CrossedCue(0.1f, 0.3f, TitleScreenPresentationRules.RevealStrikeAt, false, false), "Grand Hearth forge strike crosses once");
            AssertEqual(false, TitleScreenPresentationRules.CrossedCue(0.1f, 0.3f, TitleScreenPresentationRules.RevealStrikeAt, true, false), "reduced motion suppresses the title strike cue");
            AssertEqual(false, TitleScreenPresentationRules.CrossedCue(0.1f, 0.3f, TitleScreenPresentationRules.RevealStrikeAt, false, true), "played title strike cues do not repeat");
            AssertEqual(4, TitleScreenPresentationRules.ChronicleLines.Count, "Grand Hearth chronicle has four old-road verses");
            AssertEqual(2, TitleScreenPresentationRules.MenuIconIndex(TitleMenuChoiceKind.NewGame), "Grand Hearth new-game relic icon is pinned");

            TitleBackdropProjection cover = TitleScreenPresentationRules.ProjectBackdrop(1280f, 720f, 1672f, 941f);
            AssertEqual(true, cover.CoverRect.width >= 1280f && cover.CoverRect.height >= 720f, "Grand Hearth painting covers a 16:9 screen without letterboxing");
        }

        private static void PartySetupScreenLayoutFitsSupportedResolutions()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(1280, 720),
                new Vector2Int(1600, 900),
                new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152)
            };

            foreach (Vector2Int size in sizes)
            {
                PartySetupScreenGeometry geometry = PartySetupScreenLayout.Calculate(size.x, size.y);
                AssertEqual(true, geometry.Fits(size.x, size.y), $"party setup layout fits {size.x}x{size.y}");
                for (int i = 0; i < StarterPartyCatalog.ExpectedPartySize; i++)
                {
                    Rect row = PartySetupScreenLayout.RosterRow(geometry.Roster, i);
                    AssertEqual(true, row.xMin >= 0f && row.yMin >= 0f && row.xMax <= geometry.Roster.width && row.yMax <= geometry.Roster.height, $"party setup roster row {i} fits {size.x}x{size.y}");
                }
            }
        }

        private static void ExplorationHudScreenLayoutFitsSupportedResolutions()
        {
            const string routeCopy = "D / Right | Cave mouth | East | 1 step";
            const string outerRouteObjective = "Chapter II Route Lab: move once in the Dusk Market to trigger the ambush, then use the cave for the smoke-cave and king fight.";
            const string nearbyCopy = "Dusk Market Hideout N2\nSealed cache S4";
            const string hereCopy = "Ruin floor / patrol risk\nDusk Market Hideout nearby";
            const string growthCopy = "Party L1 / Maer 100 XP to L2";
            Vector2Int[] sizes =
            {
                new Vector2Int(1280, 720),
                new Vector2Int(1600, 900),
                new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152)
            };

            foreach (Vector2Int size in sizes)
            {
                float uiScale = ExplorationHudScreenLayout.InterfaceScale(size.x, size.y);
                AssertEqual(true, uiScale >= 1f && uiScale <= 1.25f, $"exploration UI scale stays bounded {size.x}x{size.y}");
                AssertEqual(true, ExplorationHudScreenLayout.FontSize(ExplorationHudScreenLayout.MinimumEyebrowFontSize, size.x, size.y) >= 11, $"exploration eyebrow type remains readable {size.x}x{size.y}");
                AssertEqual(true, ExplorationHudScreenLayout.FontSize(ExplorationHudScreenLayout.MinimumVitalFontSize, size.x, size.y) >= 9, $"exploration numeric vital type remains readable {size.x}x{size.y}");
                AssertEqual(true, ExplorationHudScreenLayout.FontSize(ExplorationHudScreenLayout.MinimumBodyFontSize, size.x, size.y) >= 12, $"exploration body type remains readable {size.x}x{size.y}");
                AssertEqual(true, ExplorationHudScreenLayout.FontSize(ExplorationHudScreenLayout.MinimumCommandFontSize, size.x, size.y) >= 13, $"exploration command type remains readable {size.x}x{size.y}");
                AssertEqual(true, ExplorationHudScreenLayout.FontSize(ExplorationHudScreenLayout.MinimumTitleFontSize, size.x, size.y) >= 18, $"exploration title type remains readable {size.x}x{size.y}");

                ExplorationHudGeometry commandGeometry = ExplorationHudScreenLayout.Calculate(size.x, size.y, false);
                ExplorationHudFallbackCommandLayout commandLayout = ExplorationHudFallbackLayoutRules.CalculateCommands(commandGeometry.Command, uiScale);
                AssertEqual(true, commandLayout.Fits(), $"exploration fallback command deck fits {size.x}x{size.y}");
                AssertEqual(9, commandLayout.Commands.Count, "exploration fallback preserves one context, four travel, and four utility commands");
                AssertEqual(true, commandLayout.Action.width >= commandLayout.Camp.width * 2f, $"exploration context action remains visually dominant {size.x}x{size.y}");
                AssertEqual(true, commandLayout.Action.xMax < commandLayout.ContextSeparatorX && commandLayout.ContextSeparatorX < commandLayout.Camp.xMin, $"exploration context and travel groups stay separated {size.x}x{size.y}");
                AssertEqual(true, commandLayout.Elixir.xMax < commandLayout.UtilitySeparatorX && commandLayout.UtilitySeparatorX < commandLayout.Map.xMin, $"exploration travel and utility groups stay separated {size.x}x{size.y}");
                for (int commandIndex = 0; commandIndex < commandLayout.Commands.Count; commandIndex++)
                {
                    Rect command = commandLayout.Commands[commandIndex];
                    AssertEqual(true, command.height >= 52f * uiScale - 0.01f, $"exploration fallback command {commandIndex} keeps readable height {size.x}x{size.y}");
                    if (commandIndex > 0)
                    {
                        AssertEqual(true, commandLayout.Commands[commandIndex - 1].xMax < command.xMin, $"exploration fallback command order does not overlap at slot {commandIndex} for {size.x}x{size.y}");
                    }
                }

                foreach (bool detailsOpen in new[] { false, true })
                {
                    ExplorationHudGeometry geometry = ExplorationHudScreenLayout.Calculate(size.x, size.y, detailsOpen);
                    AssertEqual(true, geometry.Fits(size.x, size.y), $"exploration HUD layout fits {size.x}x{size.y} details={detailsOpen}");
                    AssertEqual(true, geometry.Top.height >= 48f * uiScale - 0.01f, $"exploration top chrome supports readable type {size.x}x{size.y}");
                    AssertEqual(true, geometry.Command.height >= 68f * uiScale - 0.01f, $"exploration command deck supports readable controls {size.x}x{size.y}");
                    AssertEqual(true, geometry.Side.width >= (detailsOpen ? 330f : 270f) * uiScale - 0.01f, $"exploration information rail supports readable copy {size.x}x{size.y} details={detailsOpen}");
                    Rect[] buttons = ExplorationHudScreenLayout.CommandButtons(geometry.Command.width);
                    AssertEqual(2, buttons.Length, "exploration HUD keeps Interact and Menu as persistent commands");
                    foreach (Rect button in buttons)
                    {
                        AssertEqual(true, button.xMin >= 0f && button.yMin >= 0f && button.xMax <= geometry.Command.width && button.yMax <= geometry.Command.height, $"exploration command button fits {size.x}x{size.y}");
                        AssertEqual(true, button.height >= 52f * uiScale - 0.01f, $"exploration persistent command meets readable height {size.x}x{size.y}");
                    }

                    Rect fullBoard = new Rect(5f, 44f, size.x - 10f, size.y - 98f);
                    Rect reservedBoard = ExplorationHudScreenLayout.ReserveDetailsFromBoard(fullBoard, size.x, size.y, detailsOpen);
                    if (detailsOpen)
                    {
                        AssertEqual(true, reservedBoard.xMax <= geometry.Side.xMin - ExplorationHudScreenLayout.DetailsBoardGap + 0.01f, $"exploration Details reserves board space {size.x}x{size.y}");
                        AssertEqual(true, reservedBoard.width >= ExplorationHudScreenLayout.MinimumReservedBoardWidth - 0.01f, $"exploration reserved board remains usable {size.x}x{size.y}");
                    }
                    else
                    {
                        AssertEqual(true, reservedBoard.xMax <= geometry.Side.xMin - ExplorationHudScreenLayout.DetailsBoardGap + 0.01f, $"collapsed exploration tab does not cover the map grid {size.x}x{size.y}");
                        AssertEqual(true, reservedBoard.width >= ExplorationHudScreenLayout.MinimumReservedBoardWidth - 0.01f, $"collapsed exploration board remains usable {size.x}x{size.y}");
                    }

                    float sideInnerWidth = geometry.Side.width - 24f * uiScale;
                    float contentStart = geometry.Side.y + 66f * uiScale;
                    float contentBottom = geometry.Side.yMax - 48f * uiScale;
                    float availableHeight = Mathf.Max(0f, contentBottom - contentStart);
                    int bodyFontSize = ExplorationHudScreenLayout.FontSize(ExplorationHudScreenLayout.MinimumBodyFontSize, size.x, size.y);
                    string secondaryCopy = detailsOpen ? hereCopy : nearbyCopy;
                    foreach (bool hasAction in new[] { false, true })
                    {
                        ExplorationHudFallbackRailLayout rail = ExplorationHudFallbackLayoutRules.CalculateRail(
                            detailsOpen,
                            sideInnerWidth,
                            availableHeight,
                            uiScale,
                            bodyFontSize,
                            hasAction,
                            4,
                            routeCopy,
                            outerRouteObjective,
                            secondaryCopy,
                            growthCopy);
                        AssertEqual(true, rail.Fits(availableHeight), $"exploration fallback rail fits {size.x}x{size.y} details={detailsOpen} action={hasAction}");
                        AssertEqual(4, rail.PartyCount, $"exploration fallback keeps all four party rows visible {size.x}x{size.y} details={detailsOpen} action={hasAction}");
                        AssertEqual(true, contentStart + rail.UsedHeight <= contentBottom + 0.01f, $"exploration fallback party rows end above the details control {size.x}x{size.y} details={detailsOpen} action={hasAction}");
                        AssertEqual(true, rail.ObjectiveMaxLines >= 1, $"exploration fallback objective retains readable copy space {size.x}x{size.y} details={detailsOpen} action={hasAction}");

                        float copyWidth = Mathf.Max(40f * uiScale, sideInnerWidth - 18f * uiScale);
                        string boundedObjective = ExplorationHudFallbackLayoutRules.BoundedCopy(outerRouteObjective, copyWidth, bodyFontSize, rail.ObjectiveMaxLines);
                        AssertEqual(true, !string.IsNullOrWhiteSpace(boundedObjective), $"exploration fallback objective remains visible {size.x}x{size.y} details={detailsOpen} action={hasAction}");
                        AssertEqual(true, ExplorationHudFallbackLayoutRules.EstimatedWrappedLines(boundedObjective, copyWidth, bodyFontSize) <= rail.ObjectiveMaxLines, $"exploration fallback objective never clips {size.x}x{size.y} details={detailsOpen} action={hasAction}");
                        bool objectiveNeedsBounding = ExplorationHudFallbackLayoutRules.EstimatedWrappedLines(outerRouteObjective, copyWidth, bodyFontSize) > rail.ObjectiveMaxLines;
                        AssertEqual(objectiveNeedsBounding, boundedObjective.EndsWith("\u2026", StringComparison.Ordinal), $"exploration fallback makes bounded objective copy explicit {size.x}x{size.y} details={detailsOpen} action={hasAction}");
                        if (detailsOpen || !hasAction)
                        {
                            AssertEqual(outerRouteObjective, boundedObjective, $"exploration fallback expands to show the full outer-route objective when space is available {size.x}x{size.y} details={detailsOpen} action={hasAction}");
                        }
                    }
                }
            }
        }

        private static void ExplorationGuidanceRulesKeepTheGoldenThreadActionable()
        {
            AssertEqual(
                "E / Space | Enter King's Hall",
                ExplorationGuidanceRules.UseNow("King's Hall", "Enter"),
                "contextual guidance names the exact interaction input, verb, and target");
            AssertEqual(
                "E / Space | Speak with King Halvard",
                ExplorationGuidanceRules.UseNow("  King   Halvard  ", "  Speak   with  "),
                "contextual guidance normalizes authored whitespace");
            AssertEqual(
                "E / Space | Use Borin",
                ExplorationGuidanceRules.UseNow("Borin", ""),
                "missing contextual verbs receive a safe Use fallback");
            AssertEqual(
                "E / Space | Leave via Doors to Midgaard",
                ExplorationGuidanceRules.UseNow("Doors to Midgaard", "Enter", true),
                "interior exits override an unrelated contextual verb with explicit Leave copy");
            AssertEqual(
                "E / Space | Leave this interior",
                ExplorationGuidanceRules.UseNow(null, null, true),
                "missing interior-exit targets remain actionable and safe");
            AssertEqual(
                "E / Space | Use nearby objective",
                ExplorationGuidanceRules.UseNow("   ", null),
                "missing contextual targets remain actionable and safe");

            AssertEqual(
                "W / Up | King's Hall | North | 1 step",
                ExplorationGuidanceRules.Route("King's Hall", "n", 1),
                "one-step north guidance names the physical keyboard and arrow inputs");
            AssertEqual(
                "S / Down | Midgaard Sewer | South | 2 steps",
                ExplorationGuidanceRules.Route("Midgaard Sewer", "S", 2),
                "multi-step south guidance uses a plural distance");
            AssertEqual(
                "D / Right | Borin | East | 14 steps",
                ExplorationGuidanceRules.Route("Borin", " e ", 14),
                "east guidance maps the path direction to the correct physical input");
            AssertEqual(
                "A / Left | Marked: Green Shrine Turn | West | 12 steps",
                ExplorationGuidanceRules.Route("Green Shrine Turn", "W", 12, true),
                "west guidance never mislabels the northbound W key");
            AssertEqual(
                "J | Marked: Green Shrine Turn | Here - open Journal to Clear",
                ExplorationGuidanceRules.Route("Green Shrine Turn", "", 0, true),
                "a reached marked waypoint gives the exact action that resumes story guidance");
            AssertEqual(
                "WASD / arrows | King's Hall | Route blocked",
                ExplorationGuidanceRules.Route("King's Hall", "N", 9, false, true),
                "explicitly blocked routes do not promise a usable direction");
            AssertEqual(
                "WASD / arrows | King's Hall | Route blocked",
                ExplorationGuidanceRules.Route("King's Hall", "NE", 9),
                "non-cardinal first steps fail safely as a blocked route");
            AssertEqual(
                "WASD / arrows | King's Hall | Route blocked",
                ExplorationGuidanceRules.Route("King's Hall", "N", -1),
                "negative route lengths fail safely as a blocked route");
            AssertEqual(
                "WASD / arrows | No guided route is available",
                ExplorationGuidanceRules.Route(null, null, 0),
                "missing route targets produce a safe bounded fallback");

            ExplorationGuidanceRoute objectiveRoute = new ExplorationGuidanceRoute("King's Hall", "N", 7);
            ExplorationGuidanceRoute markedRoute = new ExplorationGuidanceRoute("Green Shrine Turn", "W", 12);
            string markedPreferred = ExplorationGuidanceRules.PreferredRoute(objectiveRoute, markedRoute);
            AssertEqual(
                "A / Left | Marked: Green Shrine Turn | West | 12 steps",
                markedPreferred,
                "an explicit marked waypoint takes precedence over automatic story guidance");
            AssertEqual(
                false,
                markedPreferred.Contains("King's Hall"),
                "marked precedence never mixes in the displaced automatic target");

            ExplorationGuidanceRoute blockedMarkedRoute = new ExplorationGuidanceRoute("Old Quarry Turn", "E", 8, true);
            AssertEqual(
                "WASD / arrows | Marked: Old Quarry Turn | Route blocked",
                ExplorationGuidanceRules.PreferredRoute(objectiveRoute, blockedMarkedRoute),
                "a blocked marked waypoint still retains explicit player-selected precedence");
            AssertEqual(
                "W / Up | King's Hall | North | 7 steps",
                ExplorationGuidanceRules.PreferredRoute(
                    objectiveRoute,
                    new ExplorationGuidanceRoute("  ", "W", 4)),
                "a missing marked target safely falls back to the automatic objective");
            AssertEqual(
                "WASD / arrows | No guided route is available",
                ExplorationGuidanceRules.PreferredRoute(default, default),
                "default route values remain null-safe");

            string[] directions = { "N", "S", "E", "W" };
            string[] inputs = { "W / Up", "S / Down", "D / Right", "A / Left" };
            string[] keys = { "W", "S", "D", "A" };
            string[] names = { "North", "South", "East", "West" };
            for (int i = 0; i < directions.Length; i++)
            {
                string cardinal = ExplorationGuidanceRules.Route("Road Marker", directions[i], 3);
                AssertEqual(
                    inputs[i] + " | Road Marker | " + names[i] + " | 3 steps",
                    cardinal,
                    directions[i] + " maps to one exact physical input and readable direction");
                AssertEqual(
                    keys[i],
                    ExplorationGuidanceRules.MovementKey(directions[i]),
                    directions[i] + " exposes the same WASD key used by the on-map next-step cue");
            }

            string longTarget = "The Extremely Long and Ceremonially Named Objective Beyond the Last Old Road Marker "
                + "with additional text that must never push inputs or distance outside the HUD";
            string boundedRoute = ExplorationGuidanceRules.Route(longTarget, "W", int.MaxValue, true);
            string boundedMarkedHere = ExplorationGuidanceRules.Route(longTarget, "", 0, true);
            string boundedUse = ExplorationGuidanceRules.UseNow(longTarget, "Deliberately overlong contextual interaction verb");
            AssertEqual(true, boundedRoute.Length <= ExplorationGuidanceRules.MaxHudLineLength, "long marked route copy stays inside the HUD bound");
            AssertEqual(true, boundedRoute.StartsWith("A / Left | Marked: ", StringComparison.Ordinal), "bounded route preserves the exact movement input");
            AssertEqual(true, boundedRoute.EndsWith("| West | 2147483647 steps", StringComparison.Ordinal), "bounded route preserves direction and distance after target truncation");
            AssertEqual(true, boundedMarkedHere.Length <= ExplorationGuidanceRules.MaxHudLineLength, "reached marked waypoint copy stays inside the HUD bound");
            AssertEqual(true, boundedMarkedHere.StartsWith("J | Marked: ", StringComparison.Ordinal), "reached marked waypoint preserves the exact Journal input");
            AssertEqual(true, boundedMarkedHere.EndsWith("| Here - open Journal to Clear", StringComparison.Ordinal), "reached marked waypoint preserves the resumable action after target truncation");
            AssertEqual(true, boundedUse.Length <= ExplorationGuidanceRules.MaxHudLineLength, "long contextual copy stays inside the HUD bound");
            AssertEqual(true, boundedUse.StartsWith("E / Space | ", StringComparison.Ordinal), "bounded contextual copy preserves the exact use input");
            AssertEqual(false, boundedRoute.Contains("\n") || boundedUse.Contains("\n"), "guidance always remains one HUD line");

            AssertEqual("King's Hall", objectiveRoute.TargetName, "formatting does not mutate the automatic route target");
            AssertEqual("N", objectiveRoute.FirstDirection, "formatting does not mutate the automatic route direction");
            AssertEqual(7, objectiveRoute.StepCount, "formatting does not mutate the automatic route distance");
            AssertEqual(false, objectiveRoute.RouteBlocked, "formatting does not mutate automatic route availability");
            AssertEqual(
                markedPreferred,
                ExplorationGuidanceRules.PreferredRoute(objectiveRoute, markedRoute),
                "guidance formatting is deterministic across repeated calls");

            AssertEqual(10, ExplorationMapGuidanceRules.VisiblePointLimit(false, false), "automatic Local Map thread remains restrained to nine segments");
            AssertEqual(18, ExplorationMapGuidanceRules.VisiblePointLimit(true, false), "automatic Region Map thread shows seventeen route segments");
            AssertEqual(14, ExplorationMapGuidanceRules.VisiblePointLimit(false, true), "marked Local Map route retains its stronger thirteen-segment reach");
            AssertEqual(25, ExplorationMapGuidanceRules.VisiblePointLimit(true, true), "marked Region Map route retains its stronger twenty-four-segment reach");

            Point[] eastExitPath =
            {
                new Point(2, 2),
                new Point(3, 2),
                new Point(4, 2),
                new Point(5, 2),
                new Point(6, 2)
            };
            AssertEqual(
                true,
                ExplorationMapGuidanceRules.TryFindViewportExit(
                    eastExitPath,
                    1,
                    1,
                    4,
                    4,
                    out ExplorationMapExitCue eastCue),
                "a guided path leaving the viewport produces one edge continuation cue");
            AssertEqual(ExplorationMapEdge.East, eastCue.Edge, "edge cue follows the path rather than a geometric target bearing");
            AssertEqual(4, eastCue.MapX, "edge cue anchors to the final visible route cell");
            AssertEqual(2, eastCue.MapY, "edge cue retains the path row");
            AssertEqual(2, eastCue.RemainingSteps, "edge cue reports the remaining walk from the final visible route cell");
            AssertEqual(2, eastCue.PathIndex, "edge cue exposes its route-prefix anchor for bounded rendering");
            AssertEqual(
                true,
                ExplorationMapGuidanceRules.IsExitCueWithinVisiblePrefix(
                    new ExplorationMapExitCue(ExplorationMapEdge.East, 0, 0, 1, 9),
                    10),
                "the last rendered route point may own an edge continuation cue");
            AssertEqual(
                false,
                ExplorationMapGuidanceRules.IsExitCueWithinVisiblePrefix(
                    new ExplorationMapExitCue(ExplorationMapEdge.East, 0, 0, 1, 10),
                    10),
                "the first omitted route point cannot own a disconnected edge cue");
            AssertEqual(
                false,
                ExplorationMapGuidanceRules.IsExitCueWithinVisiblePrefix(default, 10),
                "an empty edge cue never renders");
            AssertEqual(
                false,
                ExplorationMapGuidanceRules.IsExitCueWithinVisiblePrefix(eastCue, 0),
                "a zero-length route prefix never renders an edge cue");

            Point[] northExitPath =
            {
                new Point(3, 2),
                new Point(3, 1),
                new Point(3, 0)
            };
            AssertEqual(
                true,
                ExplorationMapGuidanceRules.TryFindViewportExit(
                    northExitPath,
                    1,
                    1,
                    4,
                    4,
                    out ExplorationMapExitCue northCue),
                "northbound routes receive a north edge cue");
            AssertEqual(ExplorationMapEdge.North, northCue.Edge, "north exit direction is exact");
            AssertEqual(1, northCue.RemainingSteps, "north exit remaining distance is exact");
            AssertEqual(1, northCue.PathIndex, "north edge cue anchors to the last visible path point");
            AssertEqual(
                true,
                ExplorationMapGuidanceRules.TryFindViewportExit(
                    new[]
                    {
                        new Point(2, 2),
                        new Point(1, 2),
                        new Point(0, 2)
                    },
                    1,
                    1,
                    4,
                    4,
                    out ExplorationMapExitCue westCue),
                "westbound routes receive a west edge cue");
            AssertEqual(ExplorationMapEdge.West, westCue.Edge, "west exit direction is exact");
            AssertEqual(1, westCue.MapX, "west cue anchors to the final visible column");
            AssertEqual(2, westCue.MapY, "west cue retains the path row");
            AssertEqual(1, westCue.RemainingSteps, "west cue distance is measured from its visible anchor");
            AssertEqual(1, westCue.PathIndex, "west cue exposes its visible path index");
            AssertEqual(
                true,
                ExplorationMapGuidanceRules.TryFindViewportExit(
                    new[]
                    {
                        new Point(2, 3),
                        new Point(2, 4),
                        new Point(2, 5)
                    },
                    1,
                    1,
                    4,
                    4,
                    out ExplorationMapExitCue southCue),
                "southbound routes receive a south edge cue");
            AssertEqual(ExplorationMapEdge.South, southCue.Edge, "south exit direction is exact");
            AssertEqual(2, southCue.MapX, "south cue retains the path column");
            AssertEqual(4, southCue.MapY, "south cue anchors to the final visible row");
            AssertEqual(1, southCue.RemainingSteps, "south cue distance is measured from its visible anchor");
            AssertEqual(1, southCue.PathIndex, "south cue exposes its visible path index");
            AssertEqual(
                false,
                ExplorationMapGuidanceRules.TryFindViewportExit(
                    new[] { new Point(2, 2), new Point(3, 2), new Point(4, 2) },
                    1,
                    1,
                    4,
                    4,
                    out _),
                "a route ending inside the viewport does not create a misleading edge cue");
            AssertEqual(
                false,
                ExplorationMapGuidanceRules.TryFindViewportExit(
                    new[] { new Point(2, 2), new Point(4, 2) },
                    1,
                    1,
                    4,
                    4,
                    out _),
                "a discontinuous path cannot produce a misleading continuation cue");
            AssertEqual(
                false,
                ExplorationMapGuidanceRules.TryFindViewportExit(
                    new[] { new Point(2, 2), null, new Point(2, 0) },
                    1,
                    1,
                    4,
                    4,
                    out _),
                "a malformed path remains safe and cue-free");
            AssertEqual(
                false,
                ExplorationMapGuidanceRules.TryFindViewportExit(
                    new[] { new Point(2, 2) },
                    1,
                    1,
                    4,
                    4,
                    out _),
                "a one-point route cannot claim to continue beyond the map");

            Point[] windingLocalPath =
            {
                new Point(2, 2),
                new Point(3, 2),
                new Point(3, 3),
                new Point(2, 3),
                new Point(1, 3),
                new Point(1, 2),
                new Point(1, 1),
                new Point(2, 1),
                new Point(3, 1),
                new Point(4, 1),
                new Point(4, 2),
                new Point(4, 3),
                new Point(5, 3)
            };
            AssertEqual(
                true,
                ExplorationMapGuidanceRules.TryFindViewportExit(
                    windingLocalPath,
                    0,
                    0,
                    5,
                    5,
                    out ExplorationMapExitCue lateExitCue),
                "a winding route still finds its true viewport crossing");
            AssertEqual(
                false,
                ExplorationMapGuidanceRules.IsExitCueWithinVisiblePrefix(
                    lateExitCue,
                    ExplorationMapGuidanceRules.VisiblePointLimit(false, false)),
                "an edge chip is suppressed when its anchor lies beyond the rendered Local Map thread");
        }

        private static void WorldMapGenerationRulesDefineModestExpansion()
        {
            int legacyArea = WorldMapGenerationRules.LegacyWidth * WorldMapGenerationRules.LegacyHeight;
            int previousArea = WorldMapGenerationRules.PreviousWidth * WorldMapGenerationRules.PreviousHeight;
            int expandedArea = WorldMapGenerationRules.Width * WorldMapGenerationRules.Height;

            AssertEqual(58, WorldMapGenerationRules.Width, "expanded world-map width");
            AssertEqual(46, WorldMapGenerationRules.Height, "expanded world-map height");
            AssertEqual(50, WorldMapGenerationRules.PreviousWidth, "previous world-map width remains documented");
            AssertEqual(32, WorldMapGenerationRules.PreviousHeight, "previous world-map height remains documented");
            AssertEqual(46, WorldMapGenerationRules.LegacyWidth, "legacy world-map width remains documented");
            AssertEqual(30, WorldMapGenerationRules.LegacyHeight, "legacy world-map height remains documented");
            AssertEqual(true, previousArea > legacyArea, "previous expansion remains larger than the legacy map");
            AssertEqual(true, expandedArea > previousArea, "new maps gain room for authored regional sites");
            AssertEqual(true, expandedArea <= previousArea * 7 / 4, "world expansion stays bounded around the authored road circuit");
            AssertEqual(29, WorldMapGenerationRules.StartX(WorldMapGenerationRules.Width), "fresh map start x is centered");
            AssertEqual(24, WorldMapGenerationRules.StartY(WorldMapGenerationRules.Height), "fresh map start y reserves northern room space");
            AssertEqual(9, WorldMapGenerationRules.DistrictCount(1), "depth-one map gains one additional procedural district");
            AssertEqual(5, WorldMapGenerationRules.LoopRoadCount(1), "depth-one map gains one additional random road loop");
            AssertEqual(168, WorldMapGenerationRules.WanderSteps(1, 0), "depth-one district carving scales with the larger map");
            AssertEqual(217, WorldMapGenerationRules.WanderSteps(1, 99), "district random walk clamps its random allowance");
        }

        private static void WorldMapGenerationRulesDefineNamedJunctionCircuit()
        {
            int startX = WorldMapGenerationRules.StartX(WorldMapGenerationRules.Width);
            int startY = WorldMapGenerationRules.StartY(WorldMapGenerationRules.Height);
            WorldMapJunction[] junctions = WorldMapGenerationRules.RegionalJunctions(
                WorldMapGenerationRules.Width,
                WorldMapGenerationRules.Height,
                startX,
                startY);

            AssertEqual(8, junctions.Length, "regional route circuit has eight authored junctions");
            AssertEqual(8, junctions.Select(junction => junction.Id).Distinct().Count(), "regional junction ids are unique");
            AssertEqual(8, junctions.Select(junction => junction.ZoneId).Distinct().Count(), "regional junctions identify all eight outer zones");
            AssertEqual(true, junctions.All(junction => !string.IsNullOrWhiteSpace(junction.Name) && !string.IsNullOrWhiteSpace(junction.Summary)), "regional junctions carry player-facing identity");
            AssertEqual(true, junctions.All(junction => junction.X >= 2 && junction.X < WorldMapGenerationRules.Width - 2 && junction.Y >= 2 && junction.Y < WorldMapGenerationRules.Height - 2), "regional junctions remain inside the traversable map frame");
            AssertEqual(true, ReferenceEquals(junctions, WorldMapGenerationRules.RegionalJunctions(WorldMapGenerationRules.Width, WorldMapGenerationRules.Height, startX, startY)), "regional junction arrays are cached for repeated map and HUD queries");

            MapData map = new MapData
            {
                Width = WorldMapGenerationRules.Width,
                Height = WorldMapGenerationRules.Height,
                Depth = 1,
                StartX = startX,
                StartY = startY
            };
            AssertEqual(true, junctions.All(junction => !MidgaardInteriorRules.IsReservedCell(map, junction.X, junction.Y)), "regional junctions never overlap Midgaard interior reservations");

            WorldMapJunction quarry = junctions.Single(junction => junction.Id == "quarry-turn");
            AssertEqual(true, WorldMapGenerationRules.TryFindRegionalJunction(WorldMapGenerationRules.Width, WorldMapGenerationRules.Height, startX, startY, quarry.X, quarry.Y, 0, out WorldMapJunction exact) && exact.Id == quarry.Id, "exact junction lookup resolves the authored landmark");
            AssertEqual(true, WorldMapGenerationRules.TryFindRegionalJunction(WorldMapGenerationRules.Width, WorldMapGenerationRules.Height, startX, startY, quarry.X + 1, quarry.Y, 1, out WorldMapJunction nearby) && nearby.Id == quarry.Id, "nearby junction lookup supports arrival discovery");

            int previousWidth = WorldMapGenerationRules.PreviousWidth;
            int previousHeight = WorldMapGenerationRules.PreviousHeight;
            int previousStartX = previousWidth / 2;
            int previousStartY = previousHeight / 2;
            MapData previousMap = new MapData
            {
                Width = previousWidth,
                Height = previousHeight,
                Depth = 1,
                StartX = previousStartX,
                StartY = previousStartY
            };
            WorldMapJunction[] previousJunctions = WorldMapGenerationRules.RegionalJunctions(
                previousWidth,
                previousHeight,
                previousStartX,
                previousStartY);
            AssertEqual(true, previousJunctions.All(junction => !MidgaardInteriorRules.IsReservedCell(previousMap, junction.X, junction.Y)), "previous-size save junctions stay outside embedded-room safety margins");
        }

        private static void WorldMapGenerationRulesDefineRegionalSites()
        {
            int width = WorldMapGenerationRules.Width;
            int height = WorldMapGenerationRules.Height;
            int startX = WorldMapGenerationRules.StartX(width);
            int startY = WorldMapGenerationRules.StartY(height);
            WorldMapSite[] sites = WorldMapGenerationRules.RegionalSites(width, height, startX, startY);
            MapData map = new MapData
            {
                Width = width,
                Height = height,
                Depth = 1,
                StartX = startX,
                StartY = startY
            };

            AssertEqual(8, sites.Length, "fresh world defines eight authored regional sites");
            AssertEqual(8, sites.Select(site => site.Id).Distinct().Count(), "regional site ids are unique");
            AssertEqual(8, sites.Select(site => site.ZoneId).Distinct().Count(), "regional sites give every outer zone one authored destination");
            AssertEqual(true, sites.All(site => !string.IsNullOrWhiteSpace(site.Name) && !string.IsNullOrWhiteSpace(site.Summary)), "regional sites carry player-facing names and summaries");
            AssertEqual(true, sites.All(site => site.Radius >= 2 && site.Radius <= 3), "regional sites use bounded room-sized footprints");
            AssertEqual(true, sites.All(site =>
                site.X - site.Radius >= 1
                && site.Y - site.Radius >= 1
                && site.X + site.Radius < width - 1
                && site.Y + site.Radius < height - 1), "regional site footprints remain inside the traversable frame");
            AssertEqual(true, sites.All(site => !MidgaardInteriorRules.IsReservedCell(map, site.X, site.Y)), "regional site centers remain clear of Midgaard interior reservations");
            AssertEqual(true, ReferenceEquals(sites, WorldMapGenerationRules.RegionalSites(width, height, startX, startY)), "regional site arrays are cached for generation and HUD reuse");
            AssertEqual(true, ReferenceEquals(WorldZoneCatalog.For("old-quarry", 1), WorldZoneCatalog.For("old-quarry", 1)), "world-zone metadata is shared instead of allocated per visible tile");
        }

        private static void WorldAreaTemplateRulesDefineDistinctRegionalSites()
        {
            int width = WorldMapGenerationRules.Width;
            int height = WorldMapGenerationRules.Height;
            int startX = WorldMapGenerationRules.StartX(width);
            int startY = WorldMapGenerationRules.StartY(height);
            WorldMapSite[] sites = WorldMapGenerationRules.RegionalSites(width, height, startX, startY);
            MapData map = new MapData
            {
                Width = width,
                Height = height,
                Depth = 1,
                StartX = startX,
                StartY = startY
            };

            AssertEqual(8, WorldAreaTemplateRules.All.Count, "regional area rules define eight authored compositions");
            AssertEqual(false, WorldAreaTemplateRules.TryGet(sites[0].Id, "wrong-zone", out _), "regional area lookup requires both stable site and zone identity");

            HashSet<string> signatures = new HashSet<string>(StringComparer.Ordinal);
            HashSet<string> silhouettes = new HashSet<string>(StringComparer.Ordinal);
            WorldMapJunction[] junctions = WorldMapGenerationRules.RegionalJunctions(width, height, startX, startY);
            foreach (WorldMapSite site in sites)
            {
                AssertEqual(
                    true,
                    WorldAreaTemplateRules.TryGet(site.Id, site.ZoneId, out WorldAreaTemplate template),
                    site.Id + " resolves an authored area template");
                AssertEqual(site.Radius, template.Radius, site.Id + " template preserves its established footprint radius");
                AssertEqual(
                    (site.Radius * 2 + 1) * (site.Radius * 2 + 1),
                    template.Cells.Count,
                    site.Id + " template accounts for every cell in its square footprint");

                HashSet<string> offsets = new HashSet<string>(StringComparer.Ordinal);
                foreach (WorldAreaCellTemplate cell in template.Cells)
                {
                    AssertEqual(
                        true,
                        Math.Abs(cell.OffsetX) <= site.Radius && Math.Abs(cell.OffsetY) <= site.Radius,
                        site.Id + " template cell stays inside its declared radius");
                    AssertEqual(
                        true,
                        offsets.Add(cell.OffsetX + ":" + cell.OffsetY),
                        site.Id + " template cell offsets are unique");

                    int x = site.X + cell.OffsetX;
                    int y = site.Y + cell.OffsetY;
                    AssertEqual(
                        true,
                        x >= 1 && y >= 1 && x < width - 1 && y < height - 1,
                        site.Id + " composed footprint stays inside the traversable world frame");
                    AssertEqual(
                        false,
                        MidgaardInteriorRules.IsReservedCell(map, x, y),
                        site.Id + " composed footprint avoids Midgaard reserved cells");
                }

                AssertEqual(
                    true,
                    template.Cells.Any(cell => cell.Open) && template.Cells.Any(cell => !cell.Open),
                    site.Id + " composition combines deliberate open and blocked space");
                AssertEqual(
                    true,
                    template.Cells.Select(cell => cell.Material).Distinct().Count() >= 2,
                    site.Id + " composition uses more than one authored surface material");
                AssertEqual(
                    true,
                    template.TryCell(0, 0, out WorldAreaCellTemplate center)
                    && center.Open
                    && (center.Roles & ExplorationCellRole.Threshold) != 0,
                    site.Id + " keeps its established centerpiece cell open and marked");
                AssertEqual(
                    true,
                    template.TryCell(template.ApproachOffsetX, template.ApproachOffsetY, out WorldAreaCellTemplate approach)
                    && approach.Open
                    && (approach.Roles & ExplorationCellRole.Road) != 0
                    && (approach.Roles & ExplorationCellRole.Threshold) != 0,
                    site.Id + " defines a road-marked boundary approach");
                AssertEqual(
                    1,
                    template.Cells.Count(cell =>
                        cell.Open
                        && (cell.Roles & ExplorationCellRole.Road) != 0
                        && (cell.Roles & ExplorationCellRole.Threshold) != 0),
                    site.Id + " has one unambiguous route approach");

                int approachSteps = Math.Max(Math.Abs(template.ApproachOffsetX), Math.Abs(template.ApproachOffsetY));
                int approachStepX = Math.Sign(template.ApproachOffsetX);
                int approachStepY = Math.Sign(template.ApproachOffsetY);
                AssertEqual(
                    true,
                    approachSteps == site.Radius
                    && (approachStepX == 0 || approachStepY == 0)
                    && approachStepX != approachStepY,
                    site.Id + " uses a cardinal boundary approach");
                for (int step = 1; step <= approachSteps; step++)
                {
                    AssertEqual(
                        true,
                        template.TryCell(approachStepX * step, approachStepY * step, out WorldAreaCellTemplate corridor)
                        && corridor.Open,
                        site.Id + " keeps its approach corridor open at step " + step);
                }

                AssertEqual(true, template.Objects.Count >= 3, site.Id + " has an intentional prop composition");
                HashSet<string> objectKeys = new HashSet<string>(StringComparer.Ordinal);
                HashSet<string> objectOffsets = new HashSet<string>(StringComparer.Ordinal);
                foreach (WorldAreaObjectTemplate decoration in template.Objects)
                {
                    AssertEqual(true, objectKeys.Add(decoration.Key), site.Id + " prop keys are stable and unique");
                    AssertEqual(
                        true,
                        objectOffsets.Add(decoration.OffsetX + ":" + decoration.OffsetY),
                        site.Id + " prop positions are unique");
                    AssertEqual(false, decoration.Type == ObjectType.Encounter, site.Id + " does not hide a static encounter blocker in its composition");
                    AssertEqual(
                        true,
                        template.TryCell(decoration.OffsetX, decoration.OffsetY, out WorldAreaCellTemplate propCell)
                        && propCell.Open,
                        site.Id + " props occupy authored open cells");
                    AssertEqual(
                        false,
                        decoration.OffsetX == 0 && decoration.OffsetY == 0,
                        site.Id + " props preserve the established centerpiece coordinate");
                    AssertEqual(
                        true,
                        ExplorationTraversalRules.CanStandOnObject(decoration.Type)
                        || decoration.OffsetX * approachStepY != decoration.OffsetY * approachStepX
                        || decoration.OffsetX * approachStepX + decoration.OffsetY * approachStepY <= 0,
                        site.Id + " blocking props stay off the approach corridor");
                }

                WorldMapJunction junction = junctions.Single(candidate =>
                    string.Equals(candidate.ZoneId, site.ZoneId, StringComparison.Ordinal));
                int junctionOffsetX = junction.X - site.X;
                int junctionOffsetY = junction.Y - site.Y;
                if (Math.Abs(junctionOffsetX) <= site.Radius && Math.Abs(junctionOffsetY) <= site.Radius)
                {
                    AssertEqual(
                        true,
                        template.TryCell(junctionOffsetX, junctionOffsetY, out WorldAreaCellTemplate junctionCell)
                        && junctionCell.Open
                        && (junctionCell.Roles & ExplorationCellRole.Road) != 0,
                        site.Id + " preserves its overlapping named route junction");
                    int openJunctionNeighbors = 0;
                    foreach (Point direction in new[]
                    {
                        new Point(0, -1),
                        new Point(-1, 0),
                        new Point(1, 0),
                        new Point(0, 1)
                    })
                    {
                        if (template.TryCell(
                                junctionOffsetX + direction.X,
                                junctionOffsetY + direction.Y,
                                out WorldAreaCellTemplate neighbor)
                            && neighbor.Open)
                        {
                            openJunctionNeighbors++;
                        }
                    }
                    AssertEqual(
                        true,
                        openJunctionNeighbors >= 2,
                        site.Id + " does not turn its overlapping route junction into a dead end");
                    AssertEqual(
                        true,
                        RegionalTemplateCellsConnect(
                            template,
                            junctionOffsetX,
                            junctionOffsetY,
                            template.ApproachOffsetX,
                            template.ApproachOffsetY),
                        site.Id + " connects its named junction to its authored approach");
                }

                signatures.Add(template.Signature);
                silhouettes.Add(string.Concat(template.Cells.Select(cell => cell.Open ? "1" : "0")));
            }

            AssertEqual(8, signatures.Count, "all eight regional compositions have distinct deterministic signatures");
            AssertEqual(8, silhouettes.Count, "all eight regional compositions have distinct open-space silhouettes");
        }

        private static bool RegionalTemplateCellsConnect(
            WorldAreaTemplate template,
            int startX,
            int startY,
            int goalX,
            int goalY)
        {
            if (template == null) return false;
            Queue<Point> frontier = new Queue<Point>();
            HashSet<string> visited = new HashSet<string>(StringComparer.Ordinal);
            frontier.Enqueue(new Point(startX, startY));
            visited.Add(startX + ":" + startY);
            Point[] directions =
            {
                new Point(0, -1),
                new Point(-1, 0),
                new Point(1, 0),
                new Point(0, 1)
            };
            while (frontier.Count > 0)
            {
                Point current = frontier.Dequeue();
                if (current.X == goalX && current.Y == goalY) return true;
                foreach (Point direction in directions)
                {
                    int x = current.X + direction.X;
                    int y = current.Y + direction.Y;
                    string key = x + ":" + y;
                    if (visited.Contains(key)
                        || !template.TryCell(x, y, out WorldAreaCellTemplate cell)
                        || !cell.Open)
                    {
                        continue;
                    }
                    visited.Add(key);
                    frontier.Enqueue(new Point(x, y));
                }
            }
            return false;
        }

        private static void WorldSiteInteractionRulesDefineDepthScopedServices()
        {
            int width = WorldMapGenerationRules.Width;
            int height = WorldMapGenerationRules.Height;
            int startX = WorldMapGenerationRules.StartX(width);
            int startY = WorldMapGenerationRules.StartY(height);
            WorldMapSite[] sites = WorldMapGenerationRules.RegionalSites(width, height, startX, startY);
            IReadOnlyList<WorldSiteInteractionProfile> profiles = WorldSiteInteractionRules.All;

            AssertEqual(8, profiles.Count, "regional interaction rules cover all eight authored sites");
            AssertEqual(
                string.Join("|", sites.Select(site => site.Id).OrderBy(id => id)),
                string.Join("|", profiles.Select(profile => profile.SiteId).OrderBy(id => id)),
                "regional interaction profiles match the stable authored site identities exactly");
            AssertEqual(8, profiles.Select(profile => profile.ServiceName).Distinct().Count(), "every regional site owns a distinct service name");
            AssertEqual(8, profiles.Select(profile => profile.ReadyStatus).Distinct().Count(), "every regional site owns distinct ready-state copy");
            AssertEqual(8, profiles.Select(profile => profile.ClaimedStatus).Distinct().Count(), "every regional site owns distinct completed-state copy");
            AssertEqual(8, profiles.Select(profile => profile.RewardSummary).Distinct().Count(), "every regional site explains a distinct first reward");
            AssertEqual(8, profiles.Select(profile => profile.RepeatSummary).Distinct().Count(), "every regional site explains a distinct repeat service");
            AssertEqual(8, profiles.Select(profile => profile.RewardKind).Distinct().Count(), "every regional site resolves one explicit reward implementation");

            HashSet<string> rewardFlags = new HashSet<string>(StringComparer.Ordinal);
            foreach (WorldSiteInteractionProfile profile in profiles)
            {
                AssertEqual(true, WorldSiteInteractionRules.TryGet(profile.SiteId, out WorldSiteInteractionProfile resolved), profile.SiteId + " resolves its interaction profile");
                AssertEqual(profile, resolved, profile.SiteId + " interaction lookup returns its canonical shared profile");
                AssertEqual(false, string.IsNullOrWhiteSpace(profile.ServiceName), profile.SiteId + " service name is player-facing");
                AssertEqual(false, string.IsNullOrWhiteSpace(profile.ReadyVerb), profile.SiteId + " publishes a reward-ready explore verb");
                AssertEqual(false, string.IsNullOrWhiteSpace(profile.RepeatVerb), profile.SiteId + " publishes a repeat-service explore verb");
                AssertEqual(false, profile.ReadyVerb == profile.RepeatVerb, profile.SiteId + " visibly distinguishes first reward from repeat service");
                AssertEqual(profile.ReadyVerb, WorldSiteInteractionRules.ContextVerb(profile, false), profile.SiteId + " resolves its ready explore verb");
                AssertEqual(profile.RepeatVerb, WorldSiteInteractionRules.ContextVerb(profile, true), profile.SiteId + " resolves its repeat explore verb");
                AssertEqual(
                    true,
                    profile.IsInformationalRepeat
                    || profile.RepeatSupplyCost > 0
                    || profile.RepeatGoldCost > 0,
                    profile.SiteId + " repeat mutation has an explicit resource cost");
                AssertEqual(
                    !profile.IsInformationalRepeat,
                    profile.RequiresExplicitRepeatUse,
                    profile.SiteId + " mutating repeat services require an explicit use action");
                AssertEqual(
                    true,
                    profile.RepeatSupplies == 0 || profile.RepeatGoldCost > 0,
                    profile.SiteId + " cannot generate repeat supplies without payment");

                string depthOneFlag = WorldSiteInteractionRules.RewardFlag(1, profile.SiteId);
                string depthTwoFlag = WorldSiteInteractionRules.RewardFlag(2, profile.SiteId);
                string chartFlag = WorldSiteInteractionRules.ChartFlag(1, profile.SiteId);
                AssertEqual(true, depthOneFlag.StartsWith(WorldSiteInteractionRules.RewardFlagPrefix, StringComparison.Ordinal), profile.SiteId + " uses the new regional reward namespace");
                AssertEqual(false, depthOneFlag.Contains("-"), profile.SiteId + " reward flag is serialization-safe");
                AssertEqual(false, depthOneFlag == depthTwoFlag, profile.SiteId + " reward state is scoped by map depth");
                AssertEqual(
                    "regional_site_1_" + profile.SiteId.Replace('-', '_') + "_charted",
                    chartFlag,
                    profile.SiteId + " retains its stable v2.3 chart identity for minimap discovery");
                AssertEqual(true, rewardFlags.Add(depthOneFlag), profile.SiteId + " depth-one reward flag is unique");
                AssertEqual(true, rewardFlags.Add(depthTwoFlag), profile.SiteId + " depth-two reward flag is unique");

                List<string> upgradedFlags = new List<string>
                {
                    chartFlag,
                    "route_scaffold_1_" + profile.SiteId.Replace('-', '_') + "_visited"
                };
                AssertEqual(
                    false,
                    WorldSiteInteractionRules.RewardClaimed(upgradedFlags, 1, profile.SiteId),
                    profile.SiteId + " legacy v2.3 chart/scaffold flags do not skip the new reward");
                upgradedFlags.Add(depthOneFlag);
                AssertEqual(
                    true,
                    WorldSiteInteractionRules.RewardClaimed(upgradedFlags, 1, profile.SiteId),
                    profile.SiteId + " recognizes only its new depth-scoped reward flag");
            }

            AssertEqual(false, WorldSiteInteractionRules.TryGet("not-an-authored-site", out _), "unknown regional sites do not inherit an unrelated service");
            AssertEqual("Use", WorldSiteInteractionRules.ContextVerb(null, false), "missing site metadata retains a safe generic verb");
            AssertEqual(
                WorldSiteInteractionRules.RewardFlag(1, "red-gate-seal"),
                WorldSiteInteractionRules.RewardFlag(0, "Red Gate Seal"),
                "reward flag generation clamps invalid depth and normalizes authored identity");
        }

        private static void MidgaardDistrictRulesDefineAuthoredWards()
        {
            AssertEqual(ExplorationMaterial.KeepStone, MidgaardDistrictRules.MaterialAtOffset(0, -7), "royal approach uses formal keep stone");
            AssertEqual(ExplorationMaterial.TempleStone, MidgaardDistrictRules.MaterialAtOffset(1, -4), "temple precinct uses pale temple stone");
            AssertEqual(ExplorationMaterial.MarketCobbles, MidgaardDistrictRules.MaterialAtOffset(3, 0), "trade ward uses warmer market cobbles");
            AssertEqual(ExplorationMaterial.MarketCobbles, MidgaardDistrictRules.MaterialAtOffset(-5, 2), "tavern ward uses active street cobbles");
            AssertEqual(ExplorationMaterial.SewerBrick, MidgaardDistrictRules.MaterialAtOffset(-4, 5), "cistern quarter uses damp sewer brick");
            AssertEqual(ExplorationMaterial.CityPaving, MidgaardDistrictRules.MaterialAtOffset(-8, -5), "civic edge retains quiet city paving");
            AssertEqual(5, new[]
            {
                MidgaardDistrictRules.MaterialAtOffset(0, -7),
                MidgaardDistrictRules.MaterialAtOffset(1, -4),
                MidgaardDistrictRules.MaterialAtOffset(3, 0),
                MidgaardDistrictRules.MaterialAtOffset(-4, 5),
                MidgaardDistrictRules.MaterialAtOffset(-8, -5)
            }.Distinct().Count(), "Midgaard exposes five readable district surfaces");
            AssertEqual(true, (MidgaardDistrictRules.RolesAtOffset(5, 2) & ExplorationCellRole.Road) != 0, "east trade lane is authored as a secondary road");
            AssertEqual(true, (MidgaardDistrictRules.RolesAtOffset(0, -6) & ExplorationCellRole.Plaza) != 0, "royal approach reads as a precinct");
            AssertEqual("Cistern Quarter", MidgaardDistrictRules.DistrictAtOffset(-4, 5), "HUD names the southern cistern quarter");
        }

        private static void RouteChartRulesTrackDiscoveredJunctionsAndBearings()
        {
            int width = WorldMapGenerationRules.Width;
            int height = WorldMapGenerationRules.Height;
            int startX = WorldMapGenerationRules.StartX(width);
            int startY = WorldMapGenerationRules.StartY(height);
            WorldMapJunction[] junctions = WorldMapGenerationRules.RegionalJunctions(width, height, startX, startY);
            WorldMapJunction quarry = junctions.Single(junction => junction.Id == "quarry-turn");
            WorldMapJunction market = junctions.Single(junction => junction.Id == "lanternless-cross");
            List<string> discoveries = new List<string>
            {
                RouteChartRules.DiscoveryKey(3, quarry.Id),
                RouteChartRules.DiscoveryKey(3, market.Id)
            };

            AssertEqual("3:junction:quarry-turn", RouteChartRules.DiscoveryKey(3, "Quarry-Turn"), "junction discovery keys normalize authored ids");
            AssertEqual(2, RouteChartRules.CountCharted(junctions, discoveries, 3), "route chart counts only current-depth junction discoveries");
            AssertEqual(0, RouteChartRules.CountCharted(junctions, discoveries, 2), "route chart does not leak discoveries across map depths");
            AssertEqual(true, RouteChartRules.TryNearestCharted(junctions, discoveries, 3, quarry.X + 1, quarry.Y, out RouteChartReading nearest), "route chart resolves a nearest charted marker");
            AssertEqual(quarry.Id, nearest.Junction.Id, "route chart selects the nearest discovered junction");
            AssertEqual("W", nearest.Direction, "route chart bearing uses map-readable cardinal directions");
            AssertEqual("1 step", RouteChartRules.DistanceLabel(nearest.Distance), "route chart uses singular step copy");
            AssertEqual("SE", RouteChartRules.DirectionLabel(2, 2, 5, 4), "route chart exposes diagonal bearings");

            string waypointKey = RouteChartRules.WaypointKey(3, market.Id);
            AssertEqual(RouteChartRules.DiscoveryKey(3, market.Id), waypointKey, "waypoint identity reuses the stable depth-scoped chart key");
            AssertEqual(true, RouteChartRules.IsWaypoint(waypointKey.ToUpperInvariant(), 3, market.Id), "waypoint matching is case-insensitive");
            AssertEqual(true, RouteChartRules.TryResolveWaypoint(junctions, discoveries, 3, waypointKey, out WorldMapJunction waypoint), "charted junction resolves as an active waypoint");
            AssertEqual(market.Id, waypoint.Id, "active waypoint resolves the intended authored junction");
            AssertEqual(false, RouteChartRules.TryResolveWaypoint(junctions, discoveries, 2, waypointKey, out _), "waypoint does not leak across map depths");
            AssertEqual(false, RouteChartRules.TryResolveWaypoint(junctions, new[] { RouteChartRules.DiscoveryKey(3, quarry.Id) }, 3, waypointKey, out _), "uncharted junction cannot be restored as a waypoint");
            AssertEqual("", RouteChartRules.RepairWaypointKey(junctions, discoveries, 4, waypointKey), "invalid saved waypoint repairs to an empty selection");

            GameState waypointState = new GameState { ActiveRouteWaypointKey = waypointKey };
            GameState restoredWaypointState = JsonUtility.FromJson<GameState>(JsonUtility.ToJson(waypointState));
            AssertEqual(waypointKey, restoredWaypointState.ActiveRouteWaypointKey, "save JSON roundtrip preserves the selected route waypoint");
        }

        private static void ExplorationReadabilityRulesKeepGroundBehindSprites()
        {
            float closeMidgaard = ExplorationReadabilityRules.TerrainArtAlpha(1, "midgaard-market", false, 1f);
            float wideMidgaard = ExplorationReadabilityRules.TerrainArtAlpha(1, "midgaard-market", true, 1f);
            float closeRoad = ExplorationReadabilityRules.TerrainArtAlpha(1, "road", false, 1f);
            float closeWall = ExplorationReadabilityRules.TerrainArtAlpha(0, "forestwall", false, 1f);

            AssertEqual(true, closeMidgaard >= 0.78f && closeMidgaard <= 0.84f, "close Midgaard terrain remains visible without overpowering actors");
            AssertEqual(true, wideMidgaard < closeMidgaard, "wide Midgaard view subdues terrain for overview readability");
            AssertEqual(true, closeRoad >= 0.79f && closeRoad <= 0.86f, "close road art remains readable behind interactive objects");
            AssertEqual(true, closeWall > closeRoad, "walls remain stronger than passable ground");
            AssertEqual(true, ExplorationReadabilityRules.InteractiveObjectBackdropAlpha(false, true) >= 0.20f, "framed targets receive a restrained contrast backing");
            AssertEqual(0f, ExplorationReadabilityRules.InteractiveObjectBackdropAlpha(false, false), "ordinary actors do not sit on rectangular UI plinths");
            AssertEqual(false, ExplorationReadabilityRules.ShouldUseStrongObjectFrame(true, false, false, false, false, true), "wide-map object types do not keep permanent strong frames");
            AssertEqual(true, ExplorationReadabilityRules.ShouldUseStrongObjectFrame(false, false, false, false, false, true), "local-map semantic objects retain persistent interaction frames");
            AssertEqual(true, ExplorationReadabilityRules.ShouldUseStrongObjectFrame(true, true, false, false, false, false), "wide-map objectives own strong frames");
            AssertEqual(true, ExplorationReadabilityRules.ShouldUseStrongObjectFrame(true, false, false, true, false, false), "wide-map hover owns a strong frame");
            AssertEqual(0f, ExplorationReadabilityRules.JunctionMarkerAlpha(false, false, false, false), "clearing cells without authored junctions draw no junction glyph");
            AssertEqual(true, ExplorationReadabilityRules.JunctionMarkerAlpha(true, true, true, false) > ExplorationReadabilityRules.JunctionMarkerAlpha(true, false, true, false), "active waypoint junctions outrank charted junctions");
            AssertEqual(true, ExplorationReadabilityRules.JunctionMarkerAlpha(true, false, false, true) > ExplorationReadabilityRules.JunctionMarkerAlpha(true, false, false, false), "nearby junctions outrank distant uncharted junctions");
            AssertEqual(true, ExplorationReadabilityRules.DecorativeDensityScale(true) < ExplorationReadabilityRules.DecorativeDensityScale(false), "region map uses fewer decorative props");
            AssertEqual(true, ExplorationReadabilityRules.MidgaardPropAlpha(false, 0f) >= 0.80f, "close-map ambient city props remain solid and visible");
            AssertEqual(true, ExplorationReadabilityRules.BiomePropAlpha(false, 0f) >= 0.82f, "close-map biome props remain solid and visible");
            AssertEqual(true, ExplorationReadabilityRules.MidgaardPropAlpha(false, 0f) >= 0.94f, "close-map city props no longer read as transparent ghosts");
            AssertEqual(true, ExplorationReadabilityRules.MidgaardPropAlpha(true, 1f) < 1f, "wide-map city props remain secondary to interactive targets");
            AssertEqual(true, ExplorationReadabilityRules.BiomePropAlpha(false, 1f) >= 0.98f, "close-map biome props remain solid enough to read");
            AssertEqual(true, ExplorationReadabilityRules.ShouldPreferCalmGroundUnderFocus(1, false), "near-party ground prefers calm variants");
            AssertEqual(true, ExplorationReadabilityRules.ShouldPreferCalmGroundUnderFocus(8, true), "object ground prefers calm variants");
            AssertEqual(false, ExplorationReadabilityRules.ShouldPreferCalmGroundUnderFocus(8, false), "distant empty ground can use textured variants");
            AssertEqual(false, ExplorationReadabilityRules.ShouldDrawProceduralGroundAccent(1, false, false, 0), "near-party ground does not receive procedural clutter");
            AssertEqual(false, ExplorationReadabilityRules.ShouldDrawProceduralGroundAccent(8, true, false, 0), "object ground does not receive procedural clutter");
            AssertEqual(true, ExplorationReadabilityRules.ShouldDrawProceduralGroundAccent(8, false, false, 2), "distant empty close-map ground can receive a sparse accent");
            AssertEqual(true, ExplorationReadabilityRules.ShouldDrawBiomeAmbientProp("moss", false, 0), "approved biome props can decorate the local map");
            AssertEqual(true, ExplorationReadabilityRules.ShouldDrawBiomeAmbientProp("ruins", false, 0), "route ruins can use approved environmental props");
            AssertEqual(true, ExplorationReadabilityRules.ShouldDrawBiomeAmbientProp("cistern", false, 0), "cistern routes can use approved environmental props");
            AssertEqual(true, ExplorationReadabilityRules.ShouldDrawBiomeAmbientProp("moss", true, 0), "region map retains restrained macro biome props");
            AssertEqual(false, ExplorationReadabilityRules.ShouldDrawBiomeAmbientProp("moss", true, 12), "region map caps macro prop density before local-map detail levels");
            AssertEqual(false, ExplorationReadabilityRules.ShouldDrawBiomeAmbientProp("midgaard-paved", false, 0), "city paving does not receive wilderness props");
            AssertEqual(true, ExplorationReadabilityRules.ShouldDrawMidgaardPavingDecal(false, 3, false, 0), "distant empty local-map paving can receive a city decal");
            AssertEqual(false, ExplorationReadabilityRules.ShouldDrawMidgaardPavingDecal(false, 1, false, 0), "party-adjacent paving stays clear");
            AssertEqual(false, ExplorationReadabilityRules.ShouldDrawMidgaardPavingDecal(false, 3, true, 0), "authored object cells stay clear of paving decals");
            AssertEqual(false, ExplorationReadabilityRules.ShouldDrawMidgaardPavingDecal(true, 8, false, 0), "region map suppresses paving decals");
            AssertEqual(true, ExplorationReadabilityRules.MidgaardPavingDecalAlpha(0f) >= 0.50f, "paving decals remain visible over city stone");
            AssertEqual(true, ExplorationReadabilityRules.MidgaardPavingDecalAlpha(1f) <= 0.68f, "paving decals remain subordinate to actors and props");
        }

        private static void WorldMapSpriteCellCoverageRejectsPruningExtremes()
        {
            GameObject host = new GameObject("Rule smoke world-map alpha host");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                MethodInfo method = typeof(AshenHallsGame).GetMethod("ExplorationAtlasCellLooksUsable", BindingFlags.Instance | BindingFlags.NonPublic);
                AssertEqual(true, method != null, "runtime world-map cell coverage predicate exists");

                int[] visiblePixelCounts = { 7, 8, 50, 92, 93 };
                bool[] expected = { false, true, true, true, false };
                string[] labels =
                {
                    "over-pruned cell below minimum",
                    "minimum coverage boundary",
                    "balanced sprite cell",
                    "maximum coverage boundary",
                    "under-pruned cell above maximum"
                };

                for (int i = 0; i < visiblePixelCounts.Length; i++)
                {
                    Texture2D texture = CreateAlphaCoverageTexture(visiblePixelCounts[i]);
                    try
                    {
                        bool actual = (bool)method.Invoke(game, new object[]
                        {
                            texture,
                            new Rect(0f, 0f, texture.width, texture.height),
                            i,
                            "rule smoke sprite",
                            0.08f,
                            0.92f
                        });
                        AssertEqual(expected[i], actual, labels[i]);
                    }
                    finally
                    {
                        UnityEngine.Object.DestroyImmediate(texture);
                    }
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static void WorldMapRegionLandmarkCatalogIsSemantic()
        {
            AssertEqual(5, WorldMapRegionLandmarkCatalog.Columns, "regional landmark atlas columns");
            AssertEqual(4, WorldMapRegionLandmarkCatalog.Rows, "regional landmark atlas rows");
            AssertEqual(20, WorldMapRegionLandmarkCatalog.CellCount, "regional landmark atlas cell count");

            int[] primaryMappings =
            {
                WorldMapRegionLandmarkCatalog.IconIndex("green-shrine-road", ObjectType.Shrine),
                WorldMapRegionLandmarkCatalog.IconIndex("green-shrine-road", ObjectType.AncientGrove),
                WorldMapRegionLandmarkCatalog.IconIndex("green-shrine-road", ObjectType.Camp),
                WorldMapRegionLandmarkCatalog.IconIndex("green-shrine-road", ObjectType.Waystone),
                WorldMapRegionLandmarkCatalog.IconIndex("green-shrine-road", ObjectType.Bridge),
                WorldMapRegionLandmarkCatalog.IconIndex("old-quarry", ObjectType.ForgeSite),
                WorldMapRegionLandmarkCatalog.IconIndex("old-quarry", ObjectType.Cache),
                WorldMapRegionLandmarkCatalog.IconIndex("old-quarry", ObjectType.Cave),
                WorldMapRegionLandmarkCatalog.IconIndex("old-quarry", ObjectType.Ruin),
                WorldMapRegionLandmarkCatalog.IconIndex("old-quarry", ObjectType.Bridge),
                WorldMapRegionLandmarkCatalog.IconIndex("glass-warrens", ObjectType.Obelisk),
                WorldMapRegionLandmarkCatalog.IconIndex("glass-warrens", ObjectType.Shrine),
                WorldMapRegionLandmarkCatalog.IconIndex("glass-warrens", ObjectType.Cave),
                WorldMapRegionLandmarkCatalog.IconIndex("glass-warrens", ObjectType.Ruin),
                WorldMapRegionLandmarkCatalog.IconIndex("glass-warrens", ObjectType.Cache),
                WorldMapRegionLandmarkCatalog.IconIndex("ash-fen", ObjectType.Camp),
                WorldMapRegionLandmarkCatalog.IconIndex("ash-fen", ObjectType.Shrine),
                WorldMapRegionLandmarkCatalog.IconIndex("ash-fen", ObjectType.Obelisk),
                WorldMapRegionLandmarkCatalog.IconIndex("ash-fen", ObjectType.Ruin),
                WorldMapRegionLandmarkCatalog.IconIndex("ash-fen", ObjectType.Cache)
            };
            AssertEqual(
                string.Join(",", Enumerable.Range(0, WorldMapRegionLandmarkCatalog.CellCount)),
                string.Join(",", primaryMappings.Distinct().OrderBy(index => index)),
                "regional landmark semantics cover every atlas cell exactly once");
            AssertEqual(1, WorldMapRegionLandmarkCatalog.IconIndex("green-shrine-road", ObjectType.TrainingGround), "Green Shrine training ground uses the root arch");
            AssertEqual(7, WorldMapRegionLandmarkCatalog.IconIndex("old-quarry", ObjectType.DungeonGate), "Old Quarry dungeon gate uses the supported cave");
            AssertEqual(11, WorldMapRegionLandmarkCatalog.IconIndex("glass-warrens", ObjectType.LoreLibrary), "Glass Warrens lore site uses the prism shrine");
            AssertEqual(18, WorldMapRegionLandmarkCatalog.IconIndex("red-gate", ObjectType.PortalSeal), "Red Gate portal seal uses infernal masonry");
            AssertEqual(-1, WorldMapRegionLandmarkCatalog.IconIndex("midgaard", ObjectType.Shrine), "Midgaard retains its dedicated town art");
            AssertEqual(-1, WorldMapRegionLandmarkCatalog.IconIndex("old-quarry", ObjectType.Camp), "unsupported regional objects retain generic fallbacks");
        }

        private static void WorldMapRegionMarkerCatalogIsSemantic()
        {
            AssertEqual(5, WorldMapRegionMarkerCatalog.Columns, "regional marker atlas columns");
            AssertEqual(4, WorldMapRegionMarkerCatalog.Rows, "regional marker atlas rows");
            AssertEqual(0, WorldMapRegionMarkerCatalog.ActorMarkerIndex(ObjectType.TownGuard), "guard uses the watch-banner marker");
            AssertEqual(1, WorldMapRegionMarkerCatalog.ActorMarkerIndex(ObjectType.CityCourier), "courier uses the messenger marker");
            AssertEqual(2, WorldMapRegionMarkerCatalog.ActorMarkerIndex(ObjectType.ArmorerNpc), "armorer uses the smith marker");
            AssertEqual(5, WorldMapRegionMarkerCatalog.ActorMarkerIndex(ObjectType.TavernKeeper), "tavern keeper uses the tavern marker");
            AssertEqual(10, WorldMapRegionMarkerCatalog.ActorMarkerIndex(ObjectType.MarketClerk), "market clerk uses the trade marker");
            AssertEqual(11, WorldMapRegionMarkerCatalog.ActorMarkerIndex(ObjectType.TempleHealer), "healer uses the shrine marker");
            AssertEqual(19, WorldMapRegionMarkerCatalog.ActorMarkerIndex(ObjectType.EnchanterNpc), "enchanter uses the arcane marker");
            AssertEqual(true, WorldMapRegionMarkerCatalog.ShouldShowActor(ObjectType.WoundedTraveler, 2, false), "nearby story contacts remain visible");
            AssertEqual(false, WorldMapRegionMarkerCatalog.ShouldShowActor(ObjectType.WoundedTraveler, 6, false), "distant secondary contacts no longer crowd the region map");
            AssertEqual(true, WorldMapRegionMarkerCatalog.ShouldShowActor(ObjectType.MarketClerk, 6, false), "important services remain visible at district range");
            AssertEqual(false, WorldMapRegionMarkerCatalog.ShouldShowActor(ObjectType.MarketClerk, 8, false), "even service markers yield beyond readable range");
            AssertEqual(true, WorldMapRegionMarkerCatalog.ShouldShowActor(ObjectType.WoundedTraveler, 12, true), "active objectives override distance suppression");
        }

        private static void WorldAreaSetpieceCatalogIsSemantic()
        {
            AssertEqual(4, WorldAreaSetpiecePresentationRules.Columns, "world-area set-piece atlas columns");
            AssertEqual(2, WorldAreaSetpiecePresentationRules.Rows, "world-area set-piece atlas rows");
            AssertEqual(8, WorldAreaSetpiecePresentationRules.CellCount, "world-area set-piece atlas cell count");

            Dictionary<string, int> expected = new Dictionary<string, int>(StringComparer.Ordinal)
            {
                { WorldSitePresentationRules.GreenShrineTrainingRing, 0 },
                { WorldSitePresentationRules.OldQuarryForge, 1 },
                { WorldSitePresentationRules.GloamDeepCrypt, 2 },
                { WorldSitePresentationRules.GlassLoreLibrary, 3 },
                { WorldSitePresentationRules.DuskMarketHideout, 4 },
                { WorldSitePresentationRules.RedGateSeal, 5 },
                { WorldSitePresentationRules.SaltCisternGate, 6 },
                { WorldSitePresentationRules.AshFenAncientGrove, 7 }
            };
            foreach (KeyValuePair<string, int> mapping in expected)
            {
                AssertEqual(mapping.Value, WorldAreaSetpiecePresentationRules.IconIndex(mapping.Key), mapping.Key + " uses its authored set-piece cell");
            }

            AssertEqual(
                string.Join(",", Enumerable.Range(0, WorldAreaSetpiecePresentationRules.CellCount)),
                string.Join(",", expected.Values.Distinct().OrderBy(index => index)),
                "all eight authored sites cover the set-piece atlas exactly once");
            AssertEqual(-1, WorldAreaSetpiecePresentationRules.IconIndex("unknown-site"), "unknown sites retain the generic landmark fallback");
            AssertEqual(-1, WorldAreaSetpiecePresentationRules.IconIndex(null), "missing site identity retains the generic landmark fallback");
            AssertEqual(true, WorldAreaSetpiecePresentationRules.MapScale(false) > WorldAreaSetpiecePresentationRules.MapScale(true), "local view gives authored set-pieces more visual weight");
            AssertEqual(true, WorldAreaSetpiecePresentationRules.MapScale(true) >= 2f, "region-map set-pieces remain prominent landmarks");
            AssertEqual(true, WorldAreaSetpiecePresentationRules.BaselineFraction(false) > WorldAreaSetpiecePresentationRules.BaselineFraction(true), "local set-pieces keep their authored ground baseline");
            AssertEqual(true, WorldAreaSetpiecePresentationRules.BaselineFraction(true) > 0.70f && WorldAreaSetpiecePresentationRules.BaselineFraction(false) < 0.90f, "set-piece baselines stay inside their map cells");
            AssertEqual(true, WorldAreaSetpiecePresentationRules.FitsViewport(12f, 12f, 28f, 28f, 0f, 0f, 40f, 40f, 2f), "fully visible authored set-pieces keep their large presentation");
            AssertEqual(false, WorldAreaSetpiecePresentationRules.FitsViewport(-2f, 12f, 14f, 28f, 0f, 0f, 40f, 40f, 2f), "edge-crossing authored set-pieces use the compact fallback");
            AssertEqual(false, WorldAreaSetpiecePresentationRules.FitsViewport(2f, 12f, 18f, 28f, 0f, 0f, 40f, 40f, 3f), "set-piece safe inset prevents near-edge clipping");
        }

        private static void V24WorldMapCharacterArtRulesAreStable()
        {
            AssertEqual(4, WorldThreatHabitatPresentationRules.Columns, "v2.4 habitat atlas columns");
            AssertEqual(2, WorldThreatHabitatPresentationRules.Rows, "v2.4 habitat atlas rows");
            AssertEqual(8, WorldThreatHabitatPresentationRules.CellCount, "v2.4 habitat atlas cell count");
            AssertEqual(0, WorldThreatHabitatPresentationRules.ArchetypeIndex("rats"), "rat patrols use the warren habitat");
            AssertEqual(1, WorldThreatHabitatPresentationRules.ArchetypeIndex("ratcleric"), "plague patrols use the bell midden habitat");
            AssertEqual(2, WorldThreatHabitatPresentationRules.ArchetypeIndex("kobolds"), "kobold patrols use the ambush camp habitat");
            AssertEqual(3, WorldThreatHabitatPresentationRules.ArchetypeIndex("koboldshaman"), "kobold shamans use the totem-yard habitat");
            AssertEqual(4, WorldThreatHabitatPresentationRules.FactionIndex(RoamingThreatFaction.Drow), "drow patrols use the watchpost habitat");
            AssertEqual(5, WorldThreatHabitatPresentationRules.FactionIndex(RoamingThreatFaction.Undead), "undead patrols use the ossuary habitat");
            AssertEqual(6, WorldThreatHabitatPresentationRules.FactionIndex(RoamingThreatFaction.Demons), "demon patrols use the breach habitat");
            AssertEqual(7, WorldThreatHabitatPresentationRules.ArchetypeIndex("waystation"), "neutral aftermath uses the ruined waystation habitat");
            AssertEqual(true, WorldThreatHabitatPresentationRules.DrawsBeneathRoamingThreatToken, "habitats remain below mobile patrol tokens");
            AssertEqual(true, WorldThreatHabitatPresentationRules.BottomCenterPivotY == 1f, "habitats keep a GUI bottom-center anchor");
            AssertEqual(false, WorldThreatHabitatPresentationRules.ShouldDrawAtHome(true, true), "habitats stay off certified safe roads");
            AssertEqual(true, WorldThreatHabitatPresentationRules.ShouldDrawAtHome(true, false), "valid threat homes receive habitat art");

            string[] roles = { "shield", "pike", "bow", "knife", "mender", "ember", "hex", "ward" };
            for (int index = 0; index < roles.Length; index++)
            {
                AssertEqual(index, ExplorationCharacterArtCatalog.PlayerRoleIndex(roles[index]), roles[index] + " exploration-role atlas cell");
                AssertEqual(index, ExplorationCharacterArtCatalog.PlayerTokenIndex(1, roles[index]), "single " + roles[index] + " uses the dedicated role atlas");
            }
            AssertEqual(-1, ExplorationCharacterArtCatalog.PlayerRoleIndex("party"), "party group never impersonates a solo role");
            AssertEqual(-1, ExplorationCharacterArtCatalog.PlayerTokenIndex(4, "shield"), "multi-member party bypasses the solo role atlas");
            AssertEqual(true, ExplorationCharacterArtCatalog.UsesPartyGroupToken(4), "multi-member party preserves the existing group token");

            for (int index = 0; index < ExplorationCharacterArtCatalog.CitizenCellCount; index++)
            {
                AmbientCitizenProfession profession = ExplorationCharacterArtCatalog.CitizenProfessionAt(index);
                AssertEqual(index, ExplorationCharacterArtCatalog.CitizenAtlasIndex(profession), "ambient citizen profession round-trips cell " + index);
            }
            AssertEqual(
                ExplorationCharacterArtCatalog.AmbientCitizenIndex("Wharf Market", 9471, 23, 17),
                ExplorationCharacterArtCatalog.AmbientCitizenIndex("Wharf Market", 9471, 23, 17),
                "ambient citizen placement is deterministic");
            AssertEqual(false, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(true, false, false, false, false), "ambient citizens stay off the tutorial lane");
            AssertEqual(false, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(false, true, false, false, false), "ambient citizens stay off certified safe roads");
            AssertEqual(false, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(false, false, true, false, false), "ambient citizens stay off guidance routes");
            AssertEqual(false, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(false, false, false, true, false), "ambient citizens keep entrances clear");
            AssertEqual(false, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(false, false, false, false, true), "ambient citizens never become interactable-cell impostors");
            AssertEqual(true, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(false, false, false, false, false), "ordinary non-interactive ambience remains eligible");
        }

        private static void ExplorationMiniMapPresentationRulesReserveSemanticMarkers()
        {
            AssertEqual(false, ExplorationMiniMapPresentationRules.ShouldShowAuthoredSite(false, false, 12, 8), "unknown distant authored sites stay hidden on the minimap");
            AssertEqual(true, ExplorationMiniMapPresentationRules.ShouldShowAuthoredSite(true, false, 12, 8), "discovered authored sites remain useful minimap anchors");
            AssertEqual(true, ExplorationMiniMapPresentationRules.ShouldShowAuthoredSite(false, false, 8, 8), "nearby authored sites appear at the reveal boundary");
            AssertEqual(true, ExplorationMiniMapPresentationRules.ShouldShowAuthoredSite(false, true, 20, 8), "the current authored site is always represented");

            AssertEqual(false, ExplorationMiniMapPresentationRules.ShouldShowPatrol(false, 2, 2, true, 1, 8), "inactive patrols never receive minimap markers");
            AssertEqual(false, ExplorationMiniMapPresentationRules.ShouldShowPatrol(true, 3, 2, true, 1, 8), "patrols on another depth stay off the minimap");
            AssertEqual(true, ExplorationMiniMapPresentationRules.ShouldShowPatrol(true, 2, 2, false, 8, 8), "active nearby patrols appear at the reveal boundary");
            AssertEqual(false, ExplorationMiniMapPresentationRules.ShouldShowPatrol(true, 2, 2, false, 9, 8), "unalerted patrols beyond reveal remain hidden");
            AssertEqual(true, ExplorationMiniMapPresentationRules.ShouldShowPatrol(true, 2, 2, true, 14, 8), "alerted pursuing patrols remain visible beyond reveal");

            AssertEqual(true, ExplorationMiniMapPresentationRules.MarkerPixels(ExplorationMiniMapMarkerKind.CurrentSite) > ExplorationMiniMapPresentationRules.MarkerPixels(ExplorationMiniMapMarkerKind.AuthoredSite), "current sites receive stronger minimap markers");
            AssertEqual(true, ExplorationMiniMapPresentationRules.MarkerPixels(ExplorationMiniMapMarkerKind.AlertedPatrol) > ExplorationMiniMapPresentationRules.MarkerPixels(ExplorationMiniMapMarkerKind.Patrol), "alerted patrols receive stronger minimap markers");
        }

        private static void ApprovedV130WorldMapAtlasesMatchRuntimeContracts()
        {
            AssertEqual("Ash & Brimstone", VersionInfo.ProductName, "player-facing product name");
            AssertEqual("AshAndBrimstone", VersionInfo.ExecutableBaseName, "Windows executable base name");
            AssertEqual("Ashen Halls", VersionInfo.LegacyProductName, "legacy product name remains available for save import");
            AssertEqual("v2.4.0", VersionInfo.PackageVersion, "package version matches the v2.4 release");
            BuildWindows.ValidateApprovedRuntimeArtIsLatest(Directory.GetParent(Application.dataPath).FullName);
            AssertEqual("ability-icon-atlas-runtime-v2.0.0.png", RuntimeArtManifest.AbilityIconAtlas, "approved v2.0 ability atlas pin");
            AssertEqual("signature-spell-icon-atlas-runtime-v2.0.0.png", RuntimeArtManifest.SignatureSpellIconAtlas, "approved v2.0 signature spell atlas pin");
            AssertEqual("lightning-spell-icon-atlas-runtime-v1.97.0.png", RuntimeArtManifest.LightningSpellIconAtlas, "approved v1.97 lightning spell atlas pin");
            AssertEqual("power-book-state-icon-atlas-runtime-v1.97.0.png", RuntimeArtManifest.PowerBookStateIconAtlas, "approved v1.97 power-book state atlas pin");
            AssertEqual("combat-command-icon-atlas-runtime-v1.99.0.png", RuntimeArtManifest.CombatCommandIconAtlas, "approved v1.99 combat command atlas pin");
            AssertEqual("magic-ui-atlas-runtime-v1.31.0.png", RuntimeArtManifest.MagicUiAtlas, "approved v1.31 magic UI atlas pin");
            AssertEqual("spell-animation-atlas-runtime-v1.49.0.png", RuntimeArtManifest.SpellAnimationAtlas, "approved v1.49 spell animation atlas pin");
            AssertEqual("combat-spell-effects-atlas-runtime-v0.73.png", RuntimeArtManifest.EpicSpellEffectsAtlas, "approved epic spell effects atlas pin");
            AssertEqual("title-backdrop-runtime-v2.4.0.png", RuntimeArtManifest.TavernBackdrop, "approved v2.4 Grand Hearth title backdrop pin");
            AssertEqual("tavern-ui-atlas-runtime-v1.5.9.png", RuntimeArtManifest.TavernUiAtlas, "approved v1.5.9 Grand Hearth relic atlas pin");
            AssertEqual("midgaard-gate-atlas-runtime-v1.93.0.png", RuntimeArtManifest.MidgaardGateAtlas, "approved v1.93 gate atlas pin");
            AssertEqual("midgaard-wall-atlas-runtime-v1.91.0.png", RuntimeArtManifest.MidgaardWallAtlas, "approved v1.91 wall atlas pin");
            AssertEqual("world-map-exploration-tile-atlas-runtime-v1.68.0.png", RuntimeArtManifest.WorldMapExplorationTileAtlas, "approved v1.68 exploration terrain pin");
            AssertEqual("world-map-material-atlas-runtime-v1.92.0.png", RuntimeArtManifest.WorldMapMaterialAtlas, "approved v1.92 material atlas pin");
            AssertEqual("world-map-overlay-atlas-runtime-v0.80.png", RuntimeArtManifest.WorldMapOverlayAtlas, "approved v0.80 world overlay pin");
            AssertEqual("world-map-progression-overlay-atlas-runtime-v0.63.png", RuntimeArtManifest.WorldMapProgressionOverlayAtlas, "approved v0.63 progression overlay pin");
            AssertEqual("world-map-ui-atlas-runtime-v1.6.0.png", RuntimeArtManifest.WorldMapUiAtlas, "approved v1.6 world UI pin");
            AssertEqual("world-map-token-sprite-atlas-runtime-v1.91.0.png", RuntimeArtManifest.WorldMapTokenSpriteAtlas, "approved v1.91 party token atlas pin");
            AssertEqual("world-map-prop-atlas-runtime-v1.29.0.png", RuntimeArtManifest.WorldMapPropAtlas, "approved v1.29 world prop atlas pin");
            AssertEqual("world-map-biome-prop-atlas-runtime-v1.29.0.png", RuntimeArtManifest.WorldMapBiomePropAtlas, "approved v1.29 biome prop atlas pin");
            AssertEqual("world-map-landmark-atlas-runtime-v1.29.0.png", RuntimeArtManifest.WorldMapLandmarkAtlas, "approved v1.29 landmark atlas pin");
            AssertEqual("world-map-region-landmark-atlas-runtime-v1.65.0.png", RuntimeArtManifest.WorldMapRegionLandmarkAtlas, "approved v1.65 regional landmark atlas pin");
            AssertEqual("world-map-region-marker-atlas-runtime-v2.2.0.png", RuntimeArtManifest.WorldMapRegionMarkerAtlas, "approved v2.2 regional marker atlas pin");
            AssertEqual("world-area-setpiece-atlas-runtime-v2.3.0.png", RuntimeArtManifest.WorldAreaSetpieceAtlas, "approved v2.3 world-area set-piece atlas pin");
            AssertEqual("world-threat-habitat-atlas-runtime-v2.4.0.png", RuntimeArtManifest.WorldThreatHabitatAtlas, "approved v2.4 roaming-threat habitat atlas pin");
            AssertEqual("player-exploration-role-atlas-runtime-v2.4.0.png", RuntimeArtManifest.PlayerExplorationRoleAtlas, "approved v2.4 player exploration-role atlas pin");
            AssertEqual("midgaard-town-atlas-runtime-v1.29.0.png", RuntimeArtManifest.MidgaardTownAtlas, "approved v1.29 town atlas pin");
            AssertEqual("midgaard-tile-atlas-runtime-v1.6.3.png", RuntimeArtManifest.MidgaardTileAtlas, "approved v1.6.3 Midgaard terrain pin");
            AssertEqual("midgaard-city-prop-atlas-runtime-v1.29.0.png", RuntimeArtManifest.MidgaardCityPropAtlas, "approved v1.29 city prop atlas pin");
            AssertEqual("midgaard-street-life-atlas-runtime-v1.50.0.png", RuntimeArtManifest.MidgaardStreetLifeAtlas, "approved v1.50 street-life atlas pin");
            AssertEqual("midgaard-paving-decal-atlas-runtime-v1.50.0.png", RuntimeArtManifest.MidgaardPavingDecalAtlas, "approved v1.50 paving-decal atlas pin");
            AssertEqual("midgaard-npc-atlas-runtime-v1.93.0.png", RuntimeArtManifest.MidgaardNpcAtlas, "approved v1.93 NPC atlas pin");
            AssertEqual("world-npc-citizen-atlas-runtime-v2.4.0.png", RuntimeArtManifest.WorldNpcCitizenAtlas, "approved v2.4 ambient-citizen atlas pin");
            AssertEqual("route-scaffold-atlas-runtime-v1.30.0.png", RuntimeArtManifest.RouteScaffoldAtlas, "approved v1.30 route scaffold atlas pin");
            AssertEqual("kobold-route-atlas-runtime-v1.30.0.png", RuntimeArtManifest.KoboldRouteAtlas, "approved v1.30 kobold route atlas pin");
            AssertEqual("midgaard-sewer-atlas-runtime-v1.30.0.png", RuntimeArtManifest.MidgaardSewerAtlas, "approved v1.30 sewer atlas pin");
            AssertEqual("npc-portrait-atlas-runtime-v1.60.0.png", RuntimeArtManifest.NpcPortraitAtlas, "approved v1.60 NPC portrait atlas pin");
            AssertEqual("character-combat-atlas-runtime-v1.93.0.png", RuntimeArtManifest.CharacterCombatAtlas, "approved v1.93 character sprite atlas pin");
            AssertEqual("enemy-sprite-atlas-runtime-v1.77.0.png", RuntimeArtManifest.EnemySpriteAtlas, "approved v1.77 enemy sprite atlas pin");
            AssertEqual("demon-summon-atlas-runtime-v1.4.0.png", RuntimeArtManifest.DemonSummonAtlas, "approved demon summon and transformation atlas pin");
            AssertEqual("midgaard-interior-prop-atlas-runtime-v1.61.0.png", RuntimeArtManifest.MidgaardInteriorPropAtlas, "approved v1.61 interior prop atlas pin");
            AssertEqual("midgaard-interior-tile-atlas-runtime-v1.61.0.png", RuntimeArtManifest.MidgaardInteriorTileAtlas, "approved v1.61 interior tile atlas pin");
            AssertEqual("ash-and-brimstone-title-card-runtime-v1.64.0.png", RuntimeArtManifest.TitleCard, "approved v1.64 title-card pin");
            AssertEqual("ash-and-brimstone-icon-runtime-v1.61.0.png", RuntimeArtManifest.GameIcon, "approved v1.61 game-icon pin");
            AssertEqual("roaming-threat-atlas-runtime-v1.62.0.png", RuntimeArtManifest.RoamingThreatAtlas, "approved v1.62 roaming-threat atlas pin");
            AssertEqual(
                "ability-icon-atlas-runtime-v2.0.0.png|signature-spell-icon-atlas-runtime-v2.0.0.png|lightning-spell-icon-atlas-runtime-v1.97.0.png|power-book-state-icon-atlas-runtime-v1.97.0.png|combat-command-icon-atlas-runtime-v1.99.0.png|magic-ui-atlas-runtime-v1.31.0.png|spell-animation-atlas-runtime-v1.49.0.png|combat-spell-effects-atlas-runtime-v0.73.png|title-backdrop-runtime-v2.4.0.png|tavern-ui-atlas-runtime-v1.5.9.png|midgaard-gate-atlas-runtime-v1.93.0.png|midgaard-wall-atlas-runtime-v1.91.0.png|world-map-exploration-tile-atlas-runtime-v1.68.0.png|world-map-material-atlas-runtime-v1.92.0.png|world-map-overlay-atlas-runtime-v0.80.png|world-map-progression-overlay-atlas-runtime-v0.63.png|world-map-ui-atlas-runtime-v1.6.0.png|world-map-token-sprite-atlas-runtime-v1.91.0.png|world-map-prop-atlas-runtime-v1.29.0.png|world-map-biome-prop-atlas-runtime-v1.29.0.png|world-map-landmark-atlas-runtime-v1.29.0.png|world-map-region-landmark-atlas-runtime-v1.65.0.png|world-map-region-marker-atlas-runtime-v2.2.0.png|world-area-setpiece-atlas-runtime-v2.3.0.png|world-threat-habitat-atlas-runtime-v2.4.0.png|player-exploration-role-atlas-runtime-v2.4.0.png|midgaard-town-atlas-runtime-v1.29.0.png|midgaard-tile-atlas-runtime-v1.6.3.png|midgaard-city-prop-atlas-runtime-v1.29.0.png|midgaard-street-life-atlas-runtime-v1.50.0.png|midgaard-paving-decal-atlas-runtime-v1.50.0.png|midgaard-npc-atlas-runtime-v1.93.0.png|world-npc-citizen-atlas-runtime-v2.4.0.png|route-scaffold-atlas-runtime-v1.30.0.png|kobold-route-atlas-runtime-v1.30.0.png|midgaard-sewer-atlas-runtime-v1.30.0.png|npc-portrait-atlas-runtime-v1.60.0.png|character-combat-atlas-runtime-v1.93.0.png|enemy-sprite-atlas-runtime-v1.77.0.png|demon-summon-atlas-runtime-v1.4.0.png|midgaard-interior-prop-atlas-runtime-v1.61.0.png|midgaard-interior-tile-atlas-runtime-v1.61.0.png|ash-and-brimstone-title-card-runtime-v1.64.0.png|ash-and-brimstone-icon-runtime-v1.61.0.png|roaming-threat-atlas-runtime-v1.62.0.png",
                string.Join("|", RuntimeArtManifest.ApprovedRuntimeFiles),
                "approved runtime atlas manifest");
            AssertEqual(45, RuntimeArtManifest.ApprovedRuntimeFiles.Distinct().Count(), "approved runtime atlas pins are unique");

            Dictionary<ExplorationMaterial, int> materialIndices = new Dictionary<ExplorationMaterial, int>
            {
                { ExplorationMaterial.NaturalGround, 4 },
                { ExplorationMaterial.PackedDirt, 7 },
                { ExplorationMaterial.CityPaving, 0 },
                { ExplorationMaterial.MarketCobbles, 1 },
                { ExplorationMaterial.TempleStone, 2 },
                { ExplorationMaterial.KeepStone, 3 },
                { ExplorationMaterial.SewerBrick, 15 },
                { ExplorationMaterial.QuarryStone, 10 },
                { ExplorationMaterial.GlassRubble, 11 },
                { ExplorationMaterial.FenMud, 8 },
                { ExplorationMaterial.RedAsh, 12 },
                { ExplorationMaterial.GloamStone, 13 },
                { ExplorationMaterial.CisternBrick, 14 },
                { ExplorationMaterial.Moss, 5 },
                { ExplorationMaterial.RuinedPaving, 6 },
                { ExplorationMaterial.ShallowWater, 9 },
                { ExplorationMaterial.DeepWater, 9 },
                { ExplorationMaterial.Forest, 4 },
                { ExplorationMaterial.Cliff, 10 },
                { ExplorationMaterial.RedBasalt, 12 },
                { ExplorationMaterial.RuinedWall, 6 },
                { ExplorationMaterial.CityWall, -1 },
                { ExplorationMaterial.BridgeDeck, 7 }
            };
            AssertEqual(Enum.GetValues(typeof(ExplorationMaterial)).Length, materialIndices.Count, "every exploration material has an approved atlas mapping decision");
            foreach (KeyValuePair<ExplorationMaterial, int> mapping in materialIndices)
            {
                AssertEqual(mapping.Value, ExplorationArtRules.MaterialAtlasIndex(mapping.Key), mapping.Key + " approved material atlas cell");
            }
            AssertEqual(
                string.Join(",", Enumerable.Range(0, 16)),
                string.Join(",", materialIndices.Values.Where(index => index >= 0).Distinct().OrderBy(index => index)),
                "approved material mappings cover all sixteen atlas cells");
            AssertEqual(
                string.Join(",", Enumerable.Range(0, 64)),
                string.Join(
                    ",",
                    materialIndices.Keys
                        .Where(material => materialIndices[material] >= 0)
                        .SelectMany(material => Enumerable.Range(0, 4)
                            .Select(variant => ExplorationArtRules.MaterialAtlasVariantIndex(material, variant)))
                        .Distinct()
                        .OrderBy(index => index)),
                "expanded material mappings cover all sixty-four atlas cells");

            GameObject host = new GameObject("Rule smoke v1.28 mapping host");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            Texture2D gateAtlas = null;
            Texture2D wallAtlas = null;
            Texture2D materialAtlas = null;
            List<Texture2D> normalizedAtlases = new List<Texture2D>();
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                AssertApprovedV128GateAndWallMappings(game);
                AssertApprovedSpriteMappings(game);

                gateAtlas = LoadApprovedRuntimeAtlas(RuntimeArtManifest.MidgaardGateAtlas);
                wallAtlas = LoadApprovedRuntimeAtlas(RuntimeArtManifest.MidgaardWallAtlas);
                materialAtlas = LoadApprovedRuntimeAtlas(RuntimeArtManifest.WorldMapMaterialAtlas);

                AssertEqual(new Vector2Int(1280, 1024), new Vector2Int(gateAtlas.width, gateAtlas.height), "approved gate atlas dimensions");
                AssertEqual(new Vector2Int(1280, 1024), new Vector2Int(wallAtlas.width, wallAtlas.height), "approved wall atlas dimensions");
                AssertEqual(new Vector2Int(2048, 2048), new Vector2Int(materialAtlas.width, materialAtlas.height), "approved material atlas dimensions");
                AssertAtlasCellCoverage(gateAtlas, 5, 4, new[] { 0, 1, 6, 7 }, 0.08f, 0.92f, "approved gate sprite");
                AssertAtlasCellSafeGutter(gateAtlas, 5, 4, new[] { 0, 1, 6, 7 }, 16, 8, 8, "approved gate sprite");
                RectInt closedGateBounds = AtlasCellVisibleBounds(gateAtlas, 5, 4, 0, 8);
                RectInt westGateBounds = AtlasCellVisibleBounds(gateAtlas, 5, 4, 6, 8);
                RectInt eastGateBounds = AtlasCellVisibleBounds(gateAtlas, 5, 4, 7, 8);
                AssertEqual(true, closedGateBounds.width >= 216 && closedGateBounds.width <= 228, "sealed gate keeps a coherent two-tower width");
                AssertEqual(true, closedGateBounds.height >= 178 && closedGateBounds.height <= 188, "sealed gate keeps a coherent wall-scale height");
                AssertEqual(true, eastGateBounds.width >= 58 && eastGateBounds.width <= 66, "east gate keeps its compact wall-aligned width");
                AssertEqual(true, westGateBounds.width >= 58 && westGateBounds.width <= 66, "west gate keeps its compact wall-aligned width");
                AssertEqual(true, eastGateBounds.height >= 220 && eastGateBounds.height <= 228, "east gate keeps its portrait side-view height");
                AssertEqual(true, westGateBounds.height >= 220 && westGateBounds.height <= 228, "west gate keeps its portrait side-view height");
                AssertAtlasCellClearHorizontalPassage(
                    gateAtlas,
                    5,
                    4,
                    new[] { 6, 7 },
                    104,
                    48,
                    1,
                    0f,
                    "approved side gate");
                AssertAtlasCellsHorizontalMirrors(gateAtlas, 5, 4, 6, 7, "west/east gate directional pair");
                float westGateMassX = AtlasCellVisibleCentroidX(gateAtlas, 5, 4, 6, 32);
                float eastGateMassX = AtlasCellVisibleCentroidX(gateAtlas, 5, 4, 7, 32);
                AssertEqual(true, westGateMassX > 127.5f, "west gate cell 6 keeps its authored town-side mass bias");
                AssertEqual(true, eastGateMassX < 127.5f, "east gate cell 7 keeps its authored town-side mass bias");
                AssertEqual(true, Mathf.Abs(westGateMassX + eastGateMassX - 255f) < 0.01f, "east/west directional mass remains an exact mirror pair");
                RectInt westWallBounds = AtlasCellVisibleBounds(wallAtlas, 5, 4, 2, 8);
                RectInt eastWallBounds = AtlasCellVisibleBounds(wallAtlas, 5, 4, 3, 8);
                AssertEqual(true, westWallBounds.width >= 53 && westWallBounds.width <= 61, "west straight wall contains one connected masonry run");
                AssertEqual(true, eastWallBounds.width >= 53 && eastWallBounds.width <= 61, "east straight wall contains one connected masonry run");
                AssertEqual(true, westWallBounds.height >= 171 && westWallBounds.height <= 179, "west straight wall keeps its authored height");
                AssertEqual(true, eastWallBounds.height >= 171 && eastWallBounds.height <= 179, "east straight wall keeps its authored height");
                AssertAtlasCellCoverage(wallAtlas, 5, 4, Enumerable.Range(0, 10), 0.04f, 0.92f, "approved wall sprite");
                AssertAtlasCellCoverage(materialAtlas, 8, 8, Enumerable.Range(0, 64), 0.99f, 1f, "approved material ground");

                Texture2D explorationTerrain = LoadApprovedRuntimeAtlas(RuntimeArtManifest.WorldMapExplorationTileAtlas);
                normalizedAtlases.Add(explorationTerrain);
                AssertEqual(new Vector2Int(1280, 2048), new Vector2Int(explorationTerrain.width, explorationTerrain.height), "v1.68 exploration terrain dimensions");
                AssertAtlasCellCoverage(explorationTerrain, 5, 8, Enumerable.Range(0, 40), 0.99f, 1f, "v1.68 exploration terrain");

                Texture2D worldOverlay = LoadApprovedRuntimeAtlas(RuntimeArtManifest.WorldMapOverlayAtlas);
                Texture2D progressionOverlay = LoadApprovedRuntimeAtlas(RuntimeArtManifest.WorldMapProgressionOverlayAtlas);
                Texture2D worldUi = LoadApprovedRuntimeAtlas(RuntimeArtManifest.WorldMapUiAtlas);
                Texture2D midgaardTiles = LoadApprovedRuntimeAtlas(RuntimeArtManifest.MidgaardTileAtlas);
                normalizedAtlases.Add(worldOverlay);
                normalizedAtlases.Add(progressionOverlay);
                normalizedAtlases.Add(worldUi);
                normalizedAtlases.Add(midgaardTiles);
                AssertEqual(new Vector2Int(1280, 1024), new Vector2Int(worldOverlay.width, worldOverlay.height), "approved world overlay dimensions");
                AssertEqual(new Vector2Int(1280, 1024), new Vector2Int(progressionOverlay.width, progressionOverlay.height), "approved progression overlay dimensions");
                AssertEqual(new Vector2Int(1402, 1122), new Vector2Int(worldUi.width, worldUi.height), "approved world UI dimensions");
                AssertEqual(new Vector2Int(1400, 1120), new Vector2Int(midgaardTiles.width, midgaardTiles.height), "approved Midgaard terrain dimensions");
                AssertAtlasCellCoverage(worldOverlay, 5, 4, Enumerable.Range(0, 19), 0.02f, 0.92f, "approved world overlay");
                AssertAtlasCellCoverage(progressionOverlay, 5, 4, Enumerable.Range(0, 20), 0.02f, 0.92f, "approved progression overlay");
                AssertAtlasCellCoverage(midgaardTiles, 5, 4, Enumerable.Range(0, 20), 0.99f, 1f, "approved Midgaard terrain");

                string[] normalizedFiles =
                {
                    RuntimeArtManifest.WorldMapTokenSpriteAtlas,
                    RuntimeArtManifest.WorldMapPropAtlas,
                    RuntimeArtManifest.WorldMapBiomePropAtlas,
                    RuntimeArtManifest.WorldMapLandmarkAtlas,
                    RuntimeArtManifest.MidgaardTownAtlas,
                    RuntimeArtManifest.MidgaardCityPropAtlas,
                    RuntimeArtManifest.MidgaardNpcAtlas
                };
                foreach (string fileName in normalizedFiles)
                {
                    Texture2D atlas = LoadApprovedRuntimeAtlas(fileName);
                    normalizedAtlases.Add(atlas);
                    AssertEqual(new Vector2Int(1280, 1024), new Vector2Int(atlas.width, atlas.height), fileName + " normalized dimensions");
                    AssertAtlasCellCoverage(atlas, 5, 4, Enumerable.Range(0, 20), 0.04f, 0.92f, fileName + " cell");
                    AssertAtlasCellSafeGutter(atlas, 5, 4, Enumerable.Range(0, 20), 18, 8, 8, fileName + " cell");
                    if (string.Equals(fileName, RuntimeArtManifest.WorldMapTokenSpriteAtlas, StringComparison.Ordinal))
                    {
                        RectInt partyBounds = AtlasCellVisibleBounds(atlas, 5, 4, 0, 8);
                        float partyAspect = partyBounds.width / (float)Mathf.Max(1, partyBounds.height);
                        AssertEqual(true, partyBounds.width >= 164 && partyBounds.width <= 174, "party marker is compact rather than a horizontal lineup");
                        AssertEqual(true, partyBounds.height >= 216 && partyBounds.height <= 224, "party marker uses the vertical space needed at region scale");
                        AssertEqual(true, partyAspect >= 0.74f && partyAspect <= 0.80f, "party marker keeps a readable near-portrait group silhouette");
                    }
                }

                Texture2D npcPortraits = LoadApprovedRuntimeAtlas(RuntimeArtManifest.NpcPortraitAtlas);
                Texture2D characterSprites = LoadApprovedRuntimeAtlas(RuntimeArtManifest.CharacterCombatAtlas);
                Texture2D enemySprites = LoadApprovedRuntimeAtlas(RuntimeArtManifest.EnemySpriteAtlas);
                normalizedAtlases.Add(npcPortraits);
                normalizedAtlases.Add(characterSprites);
                normalizedAtlases.Add(enemySprites);
                AssertEqual(new Vector2Int(1400, 1120), new Vector2Int(npcPortraits.width, npcPortraits.height), "approved NPC portrait dimensions");
                AssertEqual(new Vector2Int(1280, 1792), new Vector2Int(characterSprites.width, characterSprites.height), "approved character sprite dimensions");
                AssertEqual(new Vector2Int(1024, 1024), new Vector2Int(enemySprites.width, enemySprites.height), "approved enemy sprite dimensions");
                AssertAtlasCellCoverage(npcPortraits, 5, 4, Enumerable.Range(0, 20), 0.04f, 0.92f, "approved NPC portrait");
                AssertAtlasCellCoverage(
                    characterSprites,
                    PlayerSpriteCatalog.Columns,
                    PlayerSpriteCatalog.Rows,
                    Enumerable.Range(0, PlayerSpriteCatalog.Columns * PlayerSpriteCatalog.Rows),
                    0.02f,
                    0.92f,
                    "approved character sprite");
                AssertAtlasCellCoverage(enemySprites, 4, 4, Enumerable.Range(0, 16), 0.02f, 0.92f, "approved enemy sprite");
                AssertAtlasCellSafeGutter(
                    characterSprites,
                    PlayerSpriteCatalog.Columns,
                    PlayerSpriteCatalog.Rows,
                    Enumerable.Range(0, PlayerSpriteCatalog.Columns * PlayerSpriteCatalog.Rows),
                    18,
                    8,
                    8,
                    "approved character sprite");
                AssertAtlasCellSafeGutter(enemySprites, 4, 4, Enumerable.Range(0, 16), 18, 8, 8, "approved enemy sprite");

                Texture2D roamingThreats = LoadApprovedRuntimeAtlas(RuntimeArtManifest.RoamingThreatAtlas);
                normalizedAtlases.Add(roamingThreats);
                AssertEqual(new Vector2Int(1400, 1120), new Vector2Int(roamingThreats.width, roamingThreats.height), "v1.62 roaming-threat dimensions");
                AssertAtlasCellCoverage(roamingThreats, 5, 4, Enumerable.Range(0, 20), 0.08f, 0.92f, "v1.62 roaming threat");
                AssertAtlasCellSafeGutter(roamingThreats, 5, 4, Enumerable.Range(0, 20), 18, 8, 8, "v1.62 roaming threat");

                Texture2D regionalLandmarks = LoadApprovedRuntimeAtlas(RuntimeArtManifest.WorldMapRegionLandmarkAtlas);
                Texture2D regionalMarkers = LoadApprovedRuntimeAtlas(RuntimeArtManifest.WorldMapRegionMarkerAtlas);
                Texture2D areaSetpieces = LoadApprovedRuntimeAtlas(RuntimeArtManifest.WorldAreaSetpieceAtlas);
                normalizedAtlases.Add(regionalLandmarks);
                normalizedAtlases.Add(regionalMarkers);
                normalizedAtlases.Add(areaSetpieces);
                AssertEqual(new Vector2Int(1400, 1120), new Vector2Int(regionalLandmarks.width, regionalLandmarks.height), "v1.65 regional-landmark dimensions");
                AssertEqual(new Vector2Int(1400, 1120), new Vector2Int(regionalMarkers.width, regionalMarkers.height), "v2.2 regional-marker dimensions");
                AssertEqual(new Vector2Int(1536, 768), new Vector2Int(areaSetpieces.width, areaSetpieces.height), "v2.3 world-area set-piece dimensions");
                AssertAtlasCellCoverage(regionalLandmarks, 5, 4, Enumerable.Range(0, 20), 0.08f, 0.92f, "v1.65 regional landmark");
                AssertAtlasCellCoverage(regionalMarkers, 5, 4, Enumerable.Range(0, 20), 0.08f, 0.92f, "v2.2 regional marker");
                AssertAtlasCellCoverage(areaSetpieces, 4, 2, Enumerable.Range(0, 8), 0.40f, 0.60f, "v2.3 world-area set-piece");
                AssertAtlasCellSolidCore(areaSetpieces, 4, 2, Enumerable.Range(0, 8), 0.90f, "v2.3 world-area set-piece");
                AssertAtlasCellSafeGutter(regionalLandmarks, 5, 4, Enumerable.Range(0, 20), 18, 8, 8, "v1.65 regional landmark");
                AssertAtlasCellSafeGutter(regionalMarkers, 5, 4, Enumerable.Range(0, 20), 18, 8, 8, "v2.2 regional marker");
                AssertAtlasCellSafeGutter(areaSetpieces, 4, 2, Enumerable.Range(0, 8), 20, 8, 0, "v2.3 world-area set-piece");
                for (int cell = 0; cell < WorldAreaSetpiecePresentationRules.CellCount; cell++)
                {
                    RectInt bounds = AtlasCellVisibleBounds(areaSetpieces, 4, 2, cell, 8);
                    AssertEqual(20, bounds.x, "v2.3 world-area set-piece cell " + cell + " keeps exact horizontal padding");
                    AssertEqual(344, bounds.width, "v2.3 world-area set-piece cell " + cell + " uses the normalized landmark width");
                    AssertEqual(true, bounds.y >= 30 && bounds.y <= 39, "v2.3 world-area set-piece cell " + cell + " keeps bounded top padding");
                    AssertEqual(true, bounds.height >= 306 && bounds.height <= 324, "v2.3 world-area set-piece cell " + cell + " keeps a substantial landmark silhouette");
                }

                Texture2D threatHabitats = LoadApprovedRuntimeAtlas(RuntimeArtManifest.WorldThreatHabitatAtlas);
                Texture2D ambientCitizens = LoadApprovedRuntimeAtlas(RuntimeArtManifest.WorldNpcCitizenAtlas);
                Texture2D playerRoles = LoadApprovedRuntimeAtlas(RuntimeArtManifest.PlayerExplorationRoleAtlas);
                normalizedAtlases.Add(threatHabitats);
                normalizedAtlases.Add(ambientCitizens);
                normalizedAtlases.Add(playerRoles);
                AssertEqual(new Vector2Int(1536, 768), new Vector2Int(threatHabitats.width, threatHabitats.height), "v2.4 threat-habitat dimensions");
                AssertEqual(new Vector2Int(1536, 768), new Vector2Int(ambientCitizens.width, ambientCitizens.height), "v2.4 ambient-citizen dimensions");
                AssertEqual(new Vector2Int(1536, 768), new Vector2Int(playerRoles.width, playerRoles.height), "v2.4 player-role dimensions");
                AssertAtlasCellCoverageAtAlpha(threatHabitats, 4, 2, Enumerable.Range(0, 8), 0.46f, 0.51f, 8, "v2.4 threat habitat");
                AssertAtlasCellCoverageAtAlpha(ambientCitizens, 4, 2, Enumerable.Range(0, 8), 0.26f, 0.34f, 8, "v2.4 ambient citizen");
                AssertAtlasCellCoverageAtAlpha(playerRoles, 4, 2, Enumerable.Range(0, 8), 0.22f, 0.43f, 8, "v2.4 player role");
                AssertAtlasCellSafeGutter(threatHabitats, 4, 2, Enumerable.Range(0, 8), 20, 8, 0, "v2.4 threat habitat");
                AssertAtlasCellSafeGutter(ambientCitizens, 4, 2, Enumerable.Range(0, 8), 20, 8, 0, "v2.4 ambient citizen");
                AssertAtlasCellSafeGutter(playerRoles, 4, 2, Enumerable.Range(0, 8), 20, 8, 0, "v2.4 player role");
                AssertAtlasHasNoVisibleBrightMagenta(threatHabitats, 8, "v2.4 threat habitat");
                AssertAtlasHasNoVisibleBrightMagenta(ambientCitizens, 8, "v2.4 ambient citizen");
                AssertAtlasHasNoVisibleBrightMagenta(playerRoles, 8, "v2.4 player role");

                Texture2D streetLife = LoadApprovedRuntimeAtlas(RuntimeArtManifest.MidgaardStreetLifeAtlas);
                Texture2D pavingDecals = LoadApprovedRuntimeAtlas(RuntimeArtManifest.MidgaardPavingDecalAtlas);
                normalizedAtlases.Add(streetLife);
                normalizedAtlases.Add(pavingDecals);
                AssertEqual(new Vector2Int(1400, 1120), new Vector2Int(streetLife.width, streetLife.height), "v1.50 Midgaard street-life dimensions");
                AssertEqual(new Vector2Int(1252, 1252), new Vector2Int(pavingDecals.width, pavingDecals.height), "v1.50 Midgaard paving-decal dimensions");
                AssertAtlasCellCoverage(streetLife, 5, 4, Enumerable.Range(0, 20), 0.10f, 0.92f, "v1.50 street-life prop");
                AssertAtlasCellCoverage(pavingDecals, 4, 4, Enumerable.Range(0, 16), 0.08f, 0.92f, "v1.50 paving decal");

                Texture2D interiorProps = LoadApprovedRuntimeAtlas(RuntimeArtManifest.MidgaardInteriorPropAtlas);
                Texture2D interiorTiles = LoadApprovedRuntimeAtlas(RuntimeArtManifest.MidgaardInteriorTileAtlas);
                Texture2D titleCard = LoadApprovedRuntimeAtlas(RuntimeArtManifest.TitleCard);
                Texture2D gameIcon = LoadApprovedRuntimeAtlas(RuntimeArtManifest.GameIcon);
                normalizedAtlases.Add(interiorProps);
                normalizedAtlases.Add(interiorTiles);
                normalizedAtlases.Add(titleCard);
                normalizedAtlases.Add(gameIcon);
                AssertEqual(new Vector2Int(1400, 1120), new Vector2Int(interiorProps.width, interiorProps.height), "v1.61 Midgaard interior-prop dimensions");
                AssertEqual(new Vector2Int(1400, 1120), new Vector2Int(interiorTiles.width, interiorTiles.height), "v1.61 Midgaard interior-tile dimensions");
                AssertAtlasCellCoverage(interiorProps, 5, 4, Enumerable.Range(0, 20), 0.04f, 0.92f, "v1.61 interior prop");
                AssertAtlasCellCoverage(interiorTiles, 5, 4, Enumerable.Range(0, 20), 0.99f, 1f, "v1.61 interior terrain");
                AssertEqual(new Vector2Int(1800, 600), new Vector2Int(titleCard.width, titleCard.height), "v1.64 Ash & Brimstone title-card dimensions");
                AssertAtlasCellCoverage(titleCard, 1, 1, new[] { 0 }, 0.99f, 1f, "v1.64 title card");
                AssertEqual(new Vector2Int(1254, 1254), new Vector2Int(gameIcon.width, gameIcon.height), "v1.61 Ash & Brimstone game-icon dimensions");
                AssertAtlasCellCoverage(gameIcon, 1, 1, new[] { 0 }, 0.99f, 1f, "v1.61 game icon");

                Texture2D routeScaffold = LoadApprovedRuntimeAtlas(RuntimeArtManifest.RouteScaffoldAtlas);
                Texture2D koboldRoute = LoadApprovedRuntimeAtlas(RuntimeArtManifest.KoboldRouteAtlas);
                Texture2D midgaardSewer = LoadApprovedRuntimeAtlas(RuntimeArtManifest.MidgaardSewerAtlas);
                normalizedAtlases.Add(routeScaffold);
                normalizedAtlases.Add(koboldRoute);
                normalizedAtlases.Add(midgaardSewer);
                AssertEqual(new Vector2Int(1402, 1122), new Vector2Int(routeScaffold.width, routeScaffold.height), "v1.30 route scaffold dimensions");
                AssertEqual(new Vector2Int(1024, 1024), new Vector2Int(koboldRoute.width, koboldRoute.height), "v1.30 kobold route dimensions");
                AssertEqual(new Vector2Int(1280, 1024), new Vector2Int(midgaardSewer.width, midgaardSewer.height), "v1.30 Midgaard sewer dimensions");
                AssertAtlasCellSolidCore(routeScaffold, 5, 4, new[] { 1, 7 }, 0.90f, "v1.30 route scaffold");
                AssertAtlasCellSolidCore(koboldRoute, 4, 4, new[] { 0, 1, 2, 7 }, 0.85f, "v1.30 kobold route");
                AssertAtlasCellSolidCore(midgaardSewer, 5, 4, new[] { 0, 12 }, 0.95f, "v1.30 Midgaard sewer");
            }
            finally
            {
                if (gateAtlas != null) UnityEngine.Object.DestroyImmediate(gateAtlas);
                if (wallAtlas != null) UnityEngine.Object.DestroyImmediate(wallAtlas);
                if (materialAtlas != null) UnityEngine.Object.DestroyImmediate(materialAtlas);
                foreach (Texture2D atlas in normalizedAtlases)
                {
                    if (atlas != null) UnityEngine.Object.DestroyImmediate(atlas);
                }
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static void AssertApprovedSpriteMappings(AshenHallsGame game)
        {
            MethodInfo characterMethod = typeof(AshenHallsGame).GetMethod("CharacterCombatAtlasIndex", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo enemyMethod = typeof(AshenHallsGame).GetMethod("EnemySpriteIndex", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo tokenMethod = typeof(AshenHallsGame).GetMethod("WorldMapTokenSpriteIndex", BindingFlags.Instance | BindingFlags.NonPublic);
            AssertEqual(true, characterMethod != null, "character sprite mapping exists");
            AssertEqual(true, enemyMethod != null, "enemy sprite mapping exists");
            AssertEqual(true, tokenMethod != null, "world-map token mapping exists");

            string[] characterClasses =
            {
                "warrior", "rogue", "ranger", "priest", "warlock", "wizard", "paladin"
            };
            string[] characterRoles =
            {
                "shield", "knife", "bow", "mender", "hex", "ember", "ward"
            };
            string[] characterRaces =
            {
                "human", "dusk elf", "stoneborn", "fenkin", "ashling"
            };
            for (int classRow = 0; classRow < characterClasses.Length; classRow++)
            {
                for (int raceColumn = 0; raceColumn < characterRaces.Length; raceColumn++)
                {
                    int expected = classRow * PlayerSpriteCatalog.Columns + raceColumn;
                    int actual = (int)characterMethod.Invoke(
                        game,
                        new object[] { characterClasses[classRow], characterRaces[raceColumn], characterRoles[classRow] });
                    AssertEqual(
                        expected,
                        actual,
                        characterRaces[raceColumn] + " " + characterClasses[classRow] + " character sprite cell");
                }
            }
            AssertEqual(29, (int)characterMethod.Invoke(game, new object[] { "mage", "ashling", "ember" }), "mage alias uses the ashling wizard sprite");
            AssertEqual(0, (int)characterMethod.Invoke(game, new object[] { " ", null, "shield" }), "blank legacy class falls back to its role sprite");

            Dictionary<string, int> enemyMappings = new Dictionary<string, int>
            {
                { "koboldraider", 0 },
                { "koboldslinger", 2 },
                { "koboldshaman", 3 },
                { "koboldwizard", 4 },
                { "koboldking", 5 },
                { "koboldshield", 6 },
                { "ratfolk", 8 },
                { "ratcleric", 10 },
                { "ratmage", 11 },
                { "drowblade", 12 },
                { "drowmage", 13 },
                { "boundimp", 14 },
                { "lesserdemon", 15 }
            };
            foreach (KeyValuePair<string, int> mapping in enemyMappings)
            {
                AssertEqual(mapping.Value, (int)enemyMethod.Invoke(game, new object[] { mapping.Key }), mapping.Key + " enemy sprite cell");
            }
            AssertEqual(-1, (int)enemyMethod.Invoke(game, new object[] { "sewerrat" }), "sewer rat retains its dedicated creature art");

            Dictionary<string, int> tokenMappings = new Dictionary<string, int>
            {
                { "party", 0 },
                { "shield", 1 },
                { "bow", 2 },
                { "knife", 3 },
                { "mender", 4 },
                { "ember", 5 },
                { "hex", 6 },
                { "ward", 7 },
                { "pike", 8 }
            };
            foreach (KeyValuePair<string, int> mapping in tokenMappings)
            {
                AssertEqual(mapping.Value, (int)tokenMethod.Invoke(game, new object[] { mapping.Key }), mapping.Key + " world-map token cell");
            }
            AssertEqual("party", ExplorationArtRules.PartyTokenRole(4, "shield"), "multi-member party uses the group token");
            AssertEqual("shield", ExplorationArtRules.PartyTokenRole(1, "shield"), "single-member party uses its role token");
            AssertEqual("shield", ExplorationArtRules.PartyTokenRole(1, ""), "single-member legacy party gets a safe shield token");
        }

        private static void AssertApprovedV128GateAndWallMappings(AshenHallsGame game)
        {
            Type orientationType = typeof(AshenHallsGame).GetNestedType("GateOrientation", BindingFlags.NonPublic);
            MethodInfo orientationMethod = typeof(AshenHallsGame).GetMethod("GetGateOrientation", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo gateMethod = typeof(AshenHallsGame).GetMethod("GateAtlasIndex", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo fallbackMethod = typeof(AshenHallsGame).GetMethod("GateTownFallbackAtlasIndex", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo stateField = typeof(AshenHallsGame).GetField("state", BindingFlags.Instance | BindingFlags.NonPublic);
            AssertEqual(true, orientationType != null, "runtime gate orientation contract exists");
            AssertEqual(true, orientationMethod != null, "runtime gate object-to-orientation bridge exists");
            AssertEqual(true, gateMethod != null, "runtime gate atlas mapping exists");
            AssertEqual(true, fallbackMethod != null, "runtime gate fallback atlas mapping exists");
            AssertEqual(true, stateField != null, "runtime state field exists for gate and wall mapping contracts");
            AssertEqual("North,South,East,West", string.Join(",", Enum.GetNames(orientationType)), "runtime gate orientations remain exhaustive and ordered");
            stateField.SetValue(game, new GameState
            {
                Depth = 1,
                Map = new MapData { Width = 41, Height = 33, StartX = 20, StartY = 16 }
            });

            Dictionary<string, int> gateIndices = new Dictionary<string, int>
            {
                { "North", 0 },
                { "South", 0 },
                { "East", 7 },
                { "West", 6 }
            };
            Dictionary<string, int> fallbackIndices = new Dictionary<string, int>
            {
                { "North", 8 },
                { "South", 9 },
                { "East", -1 },
                { "West", -1 }
            };
            AssertEqual(gateIndices["North"], gateIndices["South"], "sealed north and south gates share sealed gate art");
            AssertEqual(true, gateIndices["East"] != gateIndices["West"], "open east and west gates retain directional art");
            foreach (KeyValuePair<string, int> mapping in gateIndices)
            {
                object orientation = Enum.Parse(orientationType, mapping.Key);
                int actual = (int)gateMethod.Invoke(game, new[] { orientation });
                AssertEqual(mapping.Value, actual, mapping.Key + " approved gate atlas cell");
                int fallback = (int)fallbackMethod.Invoke(game, new[] { orientation });
                AssertEqual(fallbackIndices[mapping.Key], fallback, mapping.Key + " approved town fallback atlas cell");
            }
            Dictionary<ObjectType, string> gateObjectOrientations = new Dictionary<ObjectType, string>
            {
                { ObjectType.NorthGate, "North" },
                { ObjectType.SouthGate, "South" },
                { ObjectType.EastGate, "East" },
                { ObjectType.WestGate, "West" }
            };
            foreach (KeyValuePair<ObjectType, string> mapping in gateObjectOrientations)
            {
                object markerOrientation = orientationMethod.Invoke(
                    game,
                    new object[] { new MapObject { Type = mapping.Key }, ObjectType.Town });
                AssertEqual(mapping.Value, markerOrientation?.ToString(), mapping.Key + " map object resolves to the intended orientation");
                AssertEqual(
                    gateIndices[mapping.Value],
                    (int)gateMethod.Invoke(game, new[] { markerOrientation }),
                    mapping.Key + " map object reaches the intended runtime atlas cell");

                object fallbackOrientation = orientationMethod.Invoke(game, new object[] { null, mapping.Key });
                AssertEqual(mapping.Value, fallbackOrientation?.ToString(), mapping.Key + " fallback type resolves to the intended orientation");
                AssertEqual(
                    gateIndices[mapping.Value],
                    (int)gateMethod.Invoke(game, new[] { fallbackOrientation }),
                    mapping.Key + " fallback type reaches the intended runtime atlas cell");
            }

            MethodInfo objectRectMethod = typeof(AshenHallsGame).GetMethod("ExploreObjectRect", BindingFlags.Instance | BindingFlags.NonPublic);
            AssertEqual(true, objectRectMethod != null, "gate composition rect exists");
            Rect eastGateRect = (Rect)objectRectMethod.Invoke(game, new object[] { new Rect(100f, 100f, 80f, 80f), new MapObject { Type = ObjectType.EastGate } });
            Rect southGateRect = (Rect)objectRectMethod.Invoke(game, new object[] { new Rect(100f, 100f, 80f, 80f), new MapObject { Type = ObjectType.SouthGate } });
            AssertEqual(true, eastGateRect.width >= 61f && eastGateRect.width <= 63f, "local side gate keeps a compact wall-aligned objective frame");
            AssertEqual(true, eastGateRect.height >= 135f && eastGateRect.height <= 137f, "local side gate has a portrait landmark height");
            AssertEqual(true, Mathf.Abs(eastGateRect.center.y - 140f) < 0.01f, "local side gate is centered symmetrically on its wall opening");
            AssertEqual(true, southGateRect.width >= 160f && southGateRect.width <= 163f, "local sealed gate remains wider than the side gate");
            AssertEqual(true, southGateRect.height >= 125f && southGateRect.height <= 128f, "local sealed gate remains wall-scale instead of engulfing the map");

            MethodInfo wallMethod = typeof(AshenHallsGame).GetMethod("MidgaardWallAtlasIndexForCoordinate", BindingFlags.Instance | BindingFlags.NonPublic);
            AssertEqual(true, wallMethod != null, "runtime wall atlas mapping exists");

            AssertEqual(0, (int)wallMethod.Invoke(game, new object[] { 20, 8, false }), "north wall approved atlas cell");
            AssertEqual(1, (int)wallMethod.Invoke(game, new object[] { 20, 23, false }), "south wall approved atlas cell");
            AssertEqual(2, (int)wallMethod.Invoke(game, new object[] { 10, 16, false }), "west wall approved atlas cell");
            AssertEqual(3, (int)wallMethod.Invoke(game, new object[] { 30, 16, false }), "east wall approved atlas cell");
            AssertEqual(5, (int)wallMethod.Invoke(game, new object[] { 10, 8, false }), "northwest wall corner points into the perimeter");
            AssertEqual(4, (int)wallMethod.Invoke(game, new object[] { 30, 8, false }), "northeast wall corner points into the perimeter");
            AssertEqual(6, (int)wallMethod.Invoke(game, new object[] { 10, 23, false }), "southwest wall corner approved atlas cell");
            AssertEqual(7, (int)wallMethod.Invoke(game, new object[] { 30, 23, false }), "southeast wall corner approved atlas cell");
            AssertEqual(8, (int)wallMethod.Invoke(game, new object[] { 16, 8, true }), "horizontal wall accent approved atlas cell");
            AssertEqual(8, (int)wallMethod.Invoke(game, new object[] { 16, 23, true }), "south horizontal wall accent approved atlas cell");
            AssertEqual(9, (int)wallMethod.Invoke(game, new object[] { 10, 14, true }), "west vertical wall accent follows town-relative cadence");
            AssertEqual(9, (int)wallMethod.Invoke(game, new object[] { 30, 22, true }), "east vertical wall accent follows town-relative cadence");
            AssertEqual(0, (int)wallMethod.Invoke(game, new object[] { 16, 8, false }), "north terrain wall does not use object accent art");
            AssertEqual(2, (int)wallMethod.Invoke(game, new object[] { 10, 16, false }), "west terrain wall does not use object accent art");
            AssertEqual(-1, (int)wallMethod.Invoke(game, new object[] { 20, 16, false }), "interior cell has no wall atlas mapping");

            GameState state = (GameState)stateField.GetValue(game);
            state.Depth = 2;
            AssertEqual(-1, (int)wallMethod.Invoke(game, new object[] { 20, 8, false }), "wall atlas mapping remains scoped to Midgaard");
        }

        private static Texture2D CreateAlphaCoverageTexture(int visiblePixelCount)
        {
            const int size = 10;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = "rule-smoke-alpha-" + visiblePixelCount;
            Color32[] pixels = new Color32[size * size];
            int visible = Mathf.Clamp(visiblePixelCount, 0, pixels.Length);
            for (int i = 0; i < visible; i++) pixels[i] = new Color32(255, 255, 255, 255);
            texture.SetPixels32(pixels);
            texture.Apply(false, false);
            return texture;
        }

        private static Texture2D LoadApprovedRuntimeAtlas(string fileName)
        {
            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
            string[] candidates =
            {
                string.IsNullOrEmpty(projectRoot) ? "" : Path.Combine(projectRoot, "Docs", "ArtReferences", fileName),
                Path.Combine(Application.dataPath, "Docs", "ArtReferences", fileName)
            };
            string path = candidates.FirstOrDefault(candidate => !string.IsNullOrEmpty(candidate) && File.Exists(candidate));
            if (string.IsNullOrEmpty(path)) throw new InvalidOperationException("approved runtime atlas is missing: " + fileName);

            Type imageConversion = Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule");
            MethodInfo loadImage = imageConversion?.GetMethod("LoadImage", new[] { typeof(Texture2D), typeof(byte[]) });
            AssertEqual(true, loadImage != null, "Unity image loader is available for " + fileName);

            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            try
            {
                bool loaded = (bool)loadImage.Invoke(null, new object[] { texture, File.ReadAllBytes(path) });
                if (!loaded) throw new InvalidOperationException("could not decode approved runtime atlas: " + fileName);
                texture.name = fileName;
                return texture;
            }
            catch
            {
                UnityEngine.Object.DestroyImmediate(texture);
                throw;
            }
        }

        private static void AssertAtlasCellCoverage(Texture2D texture, int columns, int rows, IEnumerable<int> cells, float minimum, float maximum, string label)
        {
            AssertEqual(true, texture != null, label + " atlas loads");
            AssertEqual(0, texture.width % columns, label + " atlas width divides into whole cells");
            AssertEqual(0, texture.height % rows, label + " atlas height divides into whole cells");
            Color32[] pixels = texture.GetPixels32();
            AssertEqual(texture.width * texture.height, pixels.Length, label + " atlas pixels are readable");
            int cellWidth = texture.width / columns;
            int cellHeight = texture.height / rows;

            foreach (int cell in cells)
            {
                AssertEqual(true, cell >= 0 && cell < columns * rows, label + " cell index " + cell + " is in range");
                int column = cell % columns;
                int row = cell / columns;
                int visible = 0;
                for (int topY = row * cellHeight; topY < (row + 1) * cellHeight; topY++)
                {
                    int pixelY = texture.height - 1 - topY;
                    int offset = pixelY * texture.width + column * cellWidth;
                    for (int x = 0; x < cellWidth; x++)
                    {
                        if (pixels[offset + x].a >= 32) visible++;
                    }
                }

                float fraction = visible / (float)(cellWidth * cellHeight);
                if (fraction < minimum || fraction > maximum)
                {
                    throw new InvalidOperationException($"{label} cell {cell} coverage: expected {minimum:P0}-{maximum:P0}, got {fraction:P1}");
                }
            }
        }

        private static void AssertAtlasCellCoverageAtAlpha(
            Texture2D texture,
            int columns,
            int rows,
            IEnumerable<int> cells,
            float minimum,
            float maximum,
            byte visibleAlphaThreshold,
            string label)
        {
            AssertEqual(true, texture != null, label + " atlas loads");
            AssertEqual(true, visibleAlphaThreshold > 0, label + " visibility threshold is positive");
            AssertEqual(0, texture.width % columns, label + " atlas width divides into whole cells");
            AssertEqual(0, texture.height % rows, label + " atlas height divides into whole cells");
            Color32[] pixels = texture.GetPixels32();
            int cellWidth = texture.width / columns;
            int cellHeight = texture.height / rows;
            foreach (int cell in cells)
            {
                AssertEqual(true, cell >= 0 && cell < columns * rows, label + " cell index " + cell + " is in range");
                int column = cell % columns;
                int row = cell / columns;
                int visible = 0;
                for (int localY = 0; localY < cellHeight; localY++)
                {
                    int topY = row * cellHeight + localY;
                    int pixelY = texture.height - 1 - topY;
                    int offset = pixelY * texture.width + column * cellWidth;
                    for (int localX = 0; localX < cellWidth; localX++)
                    {
                        if (pixels[offset + localX].a >= visibleAlphaThreshold) visible++;
                    }
                }

                float fraction = visible / (float)(cellWidth * cellHeight);
                if (fraction < minimum || fraction > maximum)
                {
                    throw new InvalidOperationException(
                        $"{label} cell {cell} coverage at alpha {visibleAlphaThreshold}+: expected {minimum:P0}-{maximum:P0}, got {fraction:P1}");
                }
            }
        }

        private static void AssertAtlasHasNoVisibleBrightMagenta(
            Texture2D texture,
            byte visibleAlphaThreshold,
            string label)
        {
            AssertEqual(true, texture != null, label + " atlas loads for chroma inspection");
            int residue = texture.GetPixels32().Count(pixel =>
                pixel.a >= visibleAlphaThreshold
                && pixel.r >= 248
                && pixel.g <= 8
                && pixel.b >= 248);
            AssertEqual(0, residue, label + " has no visible bright-magenta residue");
        }

        private static void AssertAtlasCellSolidCore(Texture2D texture, int columns, int rows, IEnumerable<int> cells, float minimumSolidFraction, string label)
        {
            AssertEqual(true, texture != null, label + " atlas loads");
            Color32[] pixels = texture.GetPixels32();
            int cellWidth = texture.width / columns;
            int cellHeight = texture.height / rows;
            foreach (int cell in cells)
            {
                AssertEqual(true, cell >= 0 && cell < columns * rows, label + " core cell index is in range");
                int column = cell % columns;
                int row = cell / columns;
                int visible = 0;
                int solid = 0;
                for (int localY = 0; localY < cellHeight; localY++)
                {
                    int topY = row * cellHeight + localY;
                    int pixelY = texture.height - 1 - topY;
                    int offset = pixelY * texture.width + column * cellWidth;
                    for (int localX = 0; localX < cellWidth; localX++)
                    {
                        byte alpha = pixels[offset + localX].a;
                        if (alpha < 32) continue;
                        visible++;
                        if (alpha >= 224) solid++;
                    }
                }

                float solidFraction = solid / (float)Mathf.Max(1, visible);
                if (solidFraction < minimumSolidFraction)
                {
                    throw new InvalidOperationException($"{label} cell {cell} solid-alpha core: expected at least {minimumSolidFraction:P0}, got {solidFraction:P1}");
                }
            }
        }

        private static void AssertAtlasCellClearHorizontalPassage(
            Texture2D texture,
            int columns,
            int rows,
            IEnumerable<int> cells,
            int passageTop,
            int passageHeight,
            byte visibleAlphaThreshold,
            float maximumVisibleFraction,
            string label)
        {
            AssertEqual(true, texture != null, label + " atlas loads for passage inspection");
            AssertEqual(true, columns > 0 && rows > 0, label + " passage atlas grid is positive");
            AssertEqual(0, texture.width % columns, label + " atlas width divides into whole passage cells");
            AssertEqual(0, texture.height % rows, label + " atlas height divides into whole passage cells");
            AssertEqual(true, cells != null, label + " passage cells are provided");
            int cellWidth = texture.width / columns;
            int cellHeight = texture.height / rows;
            AssertEqual(true, passageTop >= 0 && passageHeight > 0 && passageTop + passageHeight <= cellHeight, label + " passage band is inside the atlas cell");
            AssertEqual(true, visibleAlphaThreshold > 0, label + " passage alpha threshold is positive");
            AssertEqual(true, maximumVisibleFraction >= 0f && maximumVisibleFraction <= 1f, label + " passage coverage limit is normalized");
            Color32[] pixels = texture.GetPixels32();

            foreach (int cell in cells)
            {
                AssertEqual(true, cell >= 0 && cell < columns * rows, label + " passage cell index " + cell + " is in range");
                int column = cell % columns;
                int row = cell / columns;
                int visible = 0;
                for (int localY = passageTop; localY < passageTop + passageHeight; localY++)
                {
                    int topY = row * cellHeight + localY;
                    int pixelY = texture.height - 1 - topY;
                    int offset = pixelY * texture.width + column * cellWidth;
                    for (int localX = 0; localX < cellWidth; localX++)
                    {
                        if (pixels[offset + localX].a >= visibleAlphaThreshold) visible++;
                    }
                }

                float visibleFraction = visible / (float)(cellWidth * passageHeight);
                if (visibleFraction > maximumVisibleFraction)
                {
                    throw new InvalidOperationException(
                        $"{label} cell {cell} horizontal passage: expected at most {maximumVisibleFraction:P0} visible pixels, got {visibleFraction:P1}");
                }
            }
        }

        private static void AssertAtlasCellsHorizontalMirrors(
            Texture2D texture,
            int columns,
            int rows,
            int sourceCell,
            int mirroredCell,
            string label)
        {
            AssertEqual(true, texture != null, label + " atlas loads for mirror inspection");
            AssertEqual(true, columns > 0 && rows > 0, label + " mirror atlas grid is positive");
            AssertEqual(0, texture.width % columns, label + " atlas width divides into whole mirror cells");
            AssertEqual(0, texture.height % rows, label + " atlas height divides into whole mirror cells");
            AssertEqual(true, sourceCell >= 0 && sourceCell < columns * rows, label + " source cell is in range");
            AssertEqual(true, mirroredCell >= 0 && mirroredCell < columns * rows, label + " mirrored cell is in range");
            AssertEqual(true, sourceCell != mirroredCell, label + " uses distinct directional cells");
            int cellWidth = texture.width / columns;
            int cellHeight = texture.height / rows;
            int sourceColumn = sourceCell % columns;
            int sourceRow = sourceCell / columns;
            int mirroredColumn = mirroredCell % columns;
            int mirroredRow = mirroredCell / columns;
            Color32[] pixels = texture.GetPixels32();
            int mismatchedPixels = 0;

            for (int localY = 0; localY < cellHeight; localY++)
            {
                int sourceTopY = sourceRow * cellHeight + localY;
                int sourcePixelY = texture.height - 1 - sourceTopY;
                int sourceOffset = sourcePixelY * texture.width + sourceColumn * cellWidth;
                int mirroredTopY = mirroredRow * cellHeight + localY;
                int mirroredPixelY = texture.height - 1 - mirroredTopY;
                int mirroredOffset = mirroredPixelY * texture.width + mirroredColumn * cellWidth;
                for (int localX = 0; localX < cellWidth; localX++)
                {
                    Color32 sourcePixel = pixels[sourceOffset + localX];
                    Color32 mirroredPixel = pixels[mirroredOffset + cellWidth - 1 - localX];
                    if (sourcePixel.a != mirroredPixel.a
                        || (sourcePixel.a > 0
                            && (sourcePixel.r != mirroredPixel.r
                                || sourcePixel.g != mirroredPixel.g
                                || sourcePixel.b != mirroredPixel.b)))
                    {
                        mismatchedPixels++;
                    }
                }
            }

            if (mismatchedPixels > 0)
            {
                throw new InvalidOperationException($"{label}: expected exact horizontal mirrors, found {mismatchedPixels} mismatched pixels");
            }
        }

        private static float AtlasCellVisibleCentroidX(
            Texture2D texture,
            int columns,
            int rows,
            int cell,
            byte visibleAlphaThreshold)
        {
            AssertEqual(true, texture != null, "atlas loads for visible-centroid inspection");
            AssertEqual(true, columns > 0 && rows > 0, "visible-centroid atlas grid is positive");
            AssertEqual(0, texture.width % columns, "visible-centroid atlas width divides into whole cells");
            AssertEqual(0, texture.height % rows, "visible-centroid atlas height divides into whole cells");
            AssertEqual(true, cell >= 0 && cell < columns * rows, "visible-centroid cell index is in range");
            AssertEqual(true, visibleAlphaThreshold > 0, "visible-centroid alpha threshold is positive");
            int cellWidth = texture.width / columns;
            int cellHeight = texture.height / rows;
            int column = cell % columns;
            int row = cell / columns;
            Color32[] pixels = texture.GetPixels32();
            long visible = 0;
            long weightedX = 0;

            for (int localY = 0; localY < cellHeight; localY++)
            {
                int topY = row * cellHeight + localY;
                int pixelY = texture.height - 1 - topY;
                int offset = pixelY * texture.width + column * cellWidth;
                for (int localX = 0; localX < cellWidth; localX++)
                {
                    if (pixels[offset + localX].a < visibleAlphaThreshold) continue;
                    visible++;
                    weightedX += localX;
                }
            }

            AssertEqual(true, visible > 0, "visible-centroid cell contains visible pixels");
            return weightedX / (float)visible;
        }

        private static RectInt AtlasCellVisibleBounds(
            Texture2D texture,
            int columns,
            int rows,
            int cell,
            byte visibleAlphaThreshold)
        {
            AssertEqual(true, texture != null, "atlas loads for visible-bounds inspection");
            AssertEqual(true, columns > 0 && rows > 0, "visible-bounds atlas grid is positive");
            AssertEqual(0, texture.width % columns, "visible-bounds atlas width divides into whole cells");
            AssertEqual(0, texture.height % rows, "visible-bounds atlas height divides into whole cells");
            AssertEqual(true, cell >= 0 && cell < columns * rows, "visible-bounds cell index is in range");

            int cellWidth = texture.width / columns;
            int cellHeight = texture.height / rows;
            int column = cell % columns;
            int row = cell / columns;
            int minimumX = cellWidth;
            int minimumY = cellHeight;
            int maximumX = -1;
            int maximumY = -1;
            Color32[] pixels = texture.GetPixels32();

            for (int localY = 0; localY < cellHeight; localY++)
            {
                int topY = row * cellHeight + localY;
                int pixelY = texture.height - 1 - topY;
                int offset = pixelY * texture.width + column * cellWidth;
                for (int localX = 0; localX < cellWidth; localX++)
                {
                    if (pixels[offset + localX].a < visibleAlphaThreshold) continue;
                    minimumX = Mathf.Min(minimumX, localX);
                    minimumY = Mathf.Min(minimumY, localY);
                    maximumX = Mathf.Max(maximumX, localX);
                    maximumY = Mathf.Max(maximumY, localY);
                }
            }

            return maximumX < minimumX || maximumY < minimumY
                ? new RectInt(0, 0, 0, 0)
                : new RectInt(
                    minimumX,
                    minimumY,
                    maximumX - minimumX + 1,
                    maximumY - minimumY + 1);
        }

        private static void AssertAtlasCellSafeGutter(
            Texture2D texture,
            int columns,
            int rows,
            IEnumerable<int> cells,
            int gutterPixels,
            byte visibleAlphaThreshold,
            int maximumSoftGutterPixels,
            string label)
        {
            AssertEqual(true, texture != null, label + " atlas loads for gutter inspection");
            AssertEqual(0, texture.width % columns, label + " atlas width divides into whole gutter cells");
            AssertEqual(0, texture.height % rows, label + " atlas height divides into whole gutter cells");
            AssertEqual(true, visibleAlphaThreshold > 0, label + " gutter visibility threshold is positive");
            AssertEqual(true, maximumSoftGutterPixels >= 0, label + " soft gutter tolerance is non-negative");

            int cellWidth = texture.width / columns;
            int cellHeight = texture.height / rows;
            AssertEqual(true, gutterPixels > 0 && gutterPixels * 2 < cellWidth && gutterPixels * 2 < cellHeight, label + " safe gutter fits each cell");

            Color32[] pixels = texture.GetPixels32();
            AssertEqual(texture.width * texture.height, pixels.Length, label + " gutter pixels are readable");
            foreach (int cell in cells)
            {
                AssertEqual(true, cell >= 0 && cell < columns * rows, label + " gutter cell index " + cell + " is in range");
                int column = cell % columns;
                int row = cell / columns;
                int visibleBoundaryPixels = 0;
                int visibleGutterPixels = 0;
                int softGutterPixels = 0;

                for (int localY = 0; localY < cellHeight; localY++)
                {
                    int topY = row * cellHeight + localY;
                    int pixelY = texture.height - 1 - topY;
                    int offset = pixelY * texture.width + column * cellWidth;
                    for (int localX = 0; localX < cellWidth; localX++)
                    {
                        bool inSafeGutter = localX < gutterPixels
                            || localX >= cellWidth - gutterPixels
                            || localY < gutterPixels
                            || localY >= cellHeight - gutterPixels;
                        if (!inSafeGutter) continue;

                        byte alpha = pixels[offset + localX].a;
                        if (alpha >= visibleAlphaThreshold)
                        {
                            visibleGutterPixels++;
                            bool touchesBoundary = localX == 0
                                || localX == cellWidth - 1
                                || localY == 0
                                || localY == cellHeight - 1;
                            if (touchesBoundary) visibleBoundaryPixels++;
                        }
                        else if (alpha > 0)
                        {
                            softGutterPixels++;
                        }
                    }
                }

                if (visibleBoundaryPixels > 0)
                {
                    throw new InvalidOperationException($"{label} {cell} has {visibleBoundaryPixels} visible pixels touching a cell boundary");
                }
                if (visibleGutterPixels > 0)
                {
                    throw new InvalidOperationException($"{label} {cell} violates its {gutterPixels}px safe gutter with {visibleGutterPixels} pixels at alpha {visibleAlphaThreshold}+");
                }
                if (softGutterPixels > maximumSoftGutterPixels)
                {
                    throw new InvalidOperationException($"{label} {cell} has {softGutterPixels} soft-antialias gutter pixels; allowed at most {maximumSoftGutterPixels} below alpha {visibleAlphaThreshold}");
                }
            }
        }

        private static void ExplorationArtRulesMapSemanticTilesAndScale()
        {
            AssertEqual(18, ExplorationArtRules.MidgaardTileIndex(1, "midgaard-tavern", 50, true), "tavern landmark rests on calm warm paving");
            AssertEqual(16, ExplorationArtRules.MidgaardTileIndex(1, "midgaard-armorer", 10, true), "armorer rests on calm hard paving");
            AssertEqual(16, ExplorationArtRules.MidgaardTileIndex(1, "midgaard-weapons", 10, true), "weapon vendor rests on calm hard paving");
            AssertEqual(16, ExplorationArtRules.MidgaardTileIndex(1, "midgaard-enchanter", 90, true), "enchanter rests on calm cool paving");
            AssertEqual(16, ExplorationArtRules.MidgaardTileIndex(1, "midgaard-king", 50, true), "king hall rests on formal paving");
            AssertEqual(18, ExplorationArtRules.MidgaardTileIndex(1, "midgaard-sewer", 50, true), "sewer rests on worn paving");
            AssertEqual(17, ExplorationArtRules.MidgaardTileIndex(1, "midgaard-recall", 50, true), "recall circle rests on cool paving");
            AssertEqual(true, ExplorationArtRules.MidgaardTileIndex(1, "midgaard-market", 40, false) >= 15, "ordinary district ground avoids repeated landmark art");

            int[] districtIndices =
            {
                ExplorationArtRules.MidgaardTileIndex(1, "midgaard-market", 4, true),
                ExplorationArtRules.MidgaardTileIndex(1, "midgaard-temple", 40, true),
                ExplorationArtRules.MidgaardTileIndex(1, "midgaard-fountain", 90, true),
                ExplorationArtRules.MidgaardTileIndex(1, "midgaard-diner", 80, true),
                ExplorationArtRules.MidgaardTileIndex(1, "midgaard-ratquest", 80, true),
                ExplorationArtRules.MidgaardTileIndex(1, "midgaard-paved", 20, false),
                ExplorationArtRules.MidgaardTileIndex(1, "midgaard-paved", 60, false),
                ExplorationArtRules.MidgaardTileIndex(1, "midgaard-paved", 84, false)
            };
            AssertEqual(true, districtIndices.Distinct().Count() >= 4, "Midgaard districts expose a restrained set of paving materials");
            AssertEqual(true, districtIndices.All(index => index >= 0 && index < 20), "Midgaard art indices fit the 5x4 atlas");
            AssertEqual(true, districtIndices.All(index => index >= 15), "landmark sprites never duplicate semantic art in the ground layer");

            AssertEqual(11, ExplorationArtRules.ViewportWidth(false, 0), "small local viewport width");
            AssertEqual(13, ExplorationArtRules.ViewportWidth(false, 1), "medium local viewport width");
            AssertEqual(13, ExplorationArtRules.ViewportWidth(false, 2), "large local viewport preserves actor scale");
            AssertEqual(21, ExplorationArtRules.ViewportWidth(true, 2), "large region viewport width");
            AssertEqual(7, ExplorationArtRules.ViewportHeight(false, 2), "large local viewport preserves actor scale vertically");
            AssertEqual(11, ExplorationArtRules.ViewportHeight(true, 2), "large region viewport height");
            AssertEqual(0, ExplorationArtRules.AdaptiveViewportTier(false, 920f, 560f), "small exploration surface keeps large readable cells");
            AssertEqual(true, ExplorationArtRules.AdaptiveViewportTier(false, 1500f, 920f) >= 1, "larger local map surface reveals more context");
            AssertEqual(true, ExplorationArtRules.AdaptiveViewportTier(true, 1900f, 1040f) >= 1, "large region map can expand without shrinking cells below readability threshold");

            float localFill = 1f - 2f * ExplorationArtRules.PartyTokenPadding(false);
            float wideFill = 1f - 2f * ExplorationArtRules.PartyTokenPadding(true);
            AssertEqual(true, localFill >= 0.78f && localFill <= 0.82f, "local party token occupies a strong readable tile area");
            AssertEqual(true, wideFill >= 0.80f && wideFill <= 0.84f, "region party token uses the compact silhouette's safe tile area");
            AssertEqual(true, ExplorationArtRules.PartyRegionMarkerScale() >= 0.80f && ExplorationArtRules.PartyRegionMarkerScale() <= 0.84f, "region party marker remains legible at map scale");
            AssertEqual(true, ExplorationArtRules.PartyRegionMarkerMinimumPixels() >= 22f, "region party marker retains a readable small-window floor");

            AssertEqual(true, ExplorationArtRules.IsMidgaardBuilding(ObjectType.Temple), "temple uses the dedicated city-building footprint");
            AssertEqual(true, ExplorationArtRules.IsMidgaardBuilding(ObjectType.Armorer), "armorer shop no longer falls through to generic prop sizing");
            AssertEqual(true, ExplorationArtRules.IsMidgaardBuilding(ObjectType.Provisions), "provisions shop no longer falls through to generic prop sizing");
            AssertEqual(true, ExplorationArtRules.IsMidgaardBuilding(ObjectType.WeaponVendor), "weapon shop no longer falls through to generic prop sizing");
            AssertEqual(true, ExplorationArtRules.IsMidgaardBuilding(ObjectType.Enchanter), "enchanter shop no longer falls through to generic prop sizing");
            AssertEqual(false, ExplorationArtRules.IsMidgaardBuilding(ObjectType.Fountain), "plaza fixtures retain their compact footprint");
            float buildingArtFill = 1f - 2f * ExplorationArtRules.MidgaardBuildingArtPadding();
            float localBuildingFill = (1f - 2f * ExplorationArtRules.MidgaardBuildingPadding(false))
                * buildingArtFill
                * ExplorationArtRules.MidgaardBuildingSpriteScale(false);
            float regionBuildingFill = (1f - 2f * ExplorationArtRules.MidgaardBuildingPadding(true))
                * buildingArtFill
                * ExplorationArtRules.MidgaardBuildingSpriteScale(true);
            AssertEqual(true, localBuildingFill >= 1.30f && localBuildingFill <= 1.33f, "local Midgaard buildings rise beyond one cell for landmark readability");
            AssertEqual(true, regionBuildingFill >= 0.99f && regionBuildingFill <= 1.02f, "region Midgaard buildings remain cell-filling silhouettes");
            AssertEqual(true, ExplorationArtRules.MidgaardBuildingVerticalOffset(false) < ExplorationArtRules.MidgaardBuildingVerticalOffset(true), "local building growth favors the roofline over the street");

            AssertEqual(true, ExplorationArtRules.GateArtWidthInCells(false, true) >= 0.76f && ExplorationArtRules.GateArtWidthInCells(false, true) <= 0.80f, "local side gate keeps a compact wall-aligned frame");
            AssertEqual(true, ExplorationArtRules.GateArtWidthInCells(false, false) >= 1.98f && ExplorationArtRules.GateArtWidthInCells(false, false) <= 2.06f, "local sealed gate width is bounded");
            AssertEqual(true, ExplorationArtRules.GateArtHeightInCells(false, true) >= 1.66f && ExplorationArtRules.GateArtHeightInCells(false, true) <= 1.74f, "local side gate keeps its portrait height");
            AssertEqual(true, ExplorationArtRules.GateArtHeightInCells(false, false) >= 1.54f && ExplorationArtRules.GateArtHeightInCells(false, false) <= 1.62f, "local sealed gate height is bounded");
            AssertEqual(true, ExplorationArtRules.GateArtWidthInCells(true, true) < ExplorationArtRules.GateArtWidthInCells(false, true), "region side gate footprint steps down cleanly");
            AssertEqual(true, ExplorationArtRules.GateArtWidthInCells(true, false) < ExplorationArtRules.GateArtWidthInCells(false, false), "region sealed gate footprint steps down cleanly");
            AssertEqual(true, ExplorationArtRules.MidgaardWallBandThickness(false) >= 0.54f && ExplorationArtRules.MidgaardWallBandThickness(false) <= 0.58f, "local wall foundation remains substantial");
            AssertEqual(true, ExplorationArtRules.MidgaardWallBandThickness(true) >= 0.50f && ExplorationArtRules.MidgaardWallBandThickness(true) <= 0.54f, "region wall foundation remains continuous");
            AssertEqual(true, ExplorationArtRules.MidgaardWallVerticalBandThickness(false) >= 0.34f && ExplorationArtRules.MidgaardWallVerticalBandThickness(false) <= 0.38f, "local vertical wall foundation hugs the authored masonry");
            AssertEqual(true, ExplorationArtRules.MidgaardWallVerticalBandThickness(true) >= 0.32f && ExplorationArtRules.MidgaardWallVerticalBandThickness(true) <= 0.36f, "region vertical wall foundation stays narrow and continuous");
            AssertEqual(true, ExplorationArtRules.MidgaardWallVerticalBandThickness(false) < ExplorationArtRules.MidgaardWallBandThickness(false), "vertical wall foundation avoids broad side rails");
            int[] wallConnections = { 10, 10, 5, 5, 12, 6, 3, 9, 10, 5 };
            for (int wallCell = 0; wallCell < wallConnections.Length; wallCell++)
            {
                AssertEqual(
                    wallConnections[wallCell],
                    ExplorationArtRules.MidgaardWallConnectionMask(wallCell),
                    "wall cell " + wallCell + " has the approved perimeter connection mask");
            }

            AssertEqual(0, ExplorationArtRules.MaterialAtlasIndex(ExplorationMaterial.CityPaving), "material atlas city paving cell");
            AssertEqual(3, ExplorationArtRules.MaterialAtlasIndex(ExplorationMaterial.KeepStone), "material atlas keep stone cell");
            AssertEqual(5, ExplorationArtRules.MaterialAtlasIndex(ExplorationMaterial.Moss), "material atlas moss cell");
            AssertEqual(7, ExplorationArtRules.MaterialAtlasIndex(ExplorationMaterial.PackedDirt), "material atlas packed dirt cell");
            AssertEqual(9, ExplorationArtRules.MaterialAtlasIndex(ExplorationMaterial.ShallowWater), "material atlas shallow water cell");
            AssertEqual(15, ExplorationArtRules.MaterialAtlasIndex(ExplorationMaterial.SewerBrick), "material atlas sewer brick cell");
            AssertEqual(0, ExplorationArtRules.MaterialAtlasVariantIndex(ExplorationMaterial.CityPaving, 0), "approved city paving is the first expanded cell");
            AssertEqual(3, ExplorationArtRules.MaterialAtlasVariantIndex(ExplorationMaterial.CityPaving, 3), "city paving exposes three additional variants");
            AssertEqual(60, ExplorationArtRules.MaterialAtlasVariantIndex(ExplorationMaterial.SewerBrick, 0), "approved sewer brick begins the final variant bank");
            AssertEqual(63, ExplorationArtRules.MaterialAtlasVariantIndex(ExplorationMaterial.SewerBrick, 3), "expanded material atlas ends at cell sixty-three");
            AssertEqual(0, ExplorationArtRules.MaterialAtlasVariant(ExplorationMaterial.CityPaving, 24), "first v1.92 city variant owns one quarter of the distribution");
            AssertEqual(1, ExplorationArtRules.MaterialAtlasVariant(ExplorationMaterial.CityPaving, 25), "second v1.92 city variant begins at the next quarter");
            AssertEqual(2, ExplorationArtRules.MaterialAtlasVariant(ExplorationMaterial.CityPaving, 50), "third v1.92 city variant is evenly weighted");
            AssertEqual(3, ExplorationArtRules.MaterialAtlasVariant(ExplorationMaterial.CityPaving, 75), "fourth v1.92 city variant is evenly weighted");
            AssertEqual(0, ExplorationArtRules.MaterialAtlasVariant(ExplorationMaterial.NaturalGround, 24), "open natural ground retains its grassy foundation");
            AssertEqual(1, ExplorationArtRules.MaterialAtlasVariant(ExplorationMaterial.NaturalGround, 25), "new natural-ground variants own most visible cells");
            AssertEqual(1, ExplorationArtRules.MaterialAtlasVariant(ExplorationMaterial.Forest, 0), "forest avoids the bright open-ground variant");
            AssertEqual(3, ExplorationArtRules.MaterialAtlasVariant(ExplorationMaterial.Forest, 2), "all three forest-loam variants are reachable");
            AssertEqual(
                true,
                ExplorationArtRules.ShouldBlendMaterialEdge(
                    ExplorationMaterial.CityPaving,
                    ExplorationMaterial.MarketCobbles,
                    true,
                    true,
                    false),
                "different passable material banks receive a feathered join");
            AssertEqual(
                false,
                ExplorationArtRules.ShouldBlendMaterialEdge(
                    ExplorationMaterial.CityPaving,
                    ExplorationMaterial.CityPaving,
                    true,
                    true,
                    false),
                "one material bank never feathers against itself");
            AssertEqual(
                false,
                ExplorationArtRules.ShouldBlendMaterialEdge(
                    ExplorationMaterial.CityPaving,
                    ExplorationMaterial.MarketCobbles,
                    true,
                    true,
                    true),
                "gate and threshold cells keep a crisp protected join");
            AssertEqual(
                false,
                ExplorationArtRules.ShouldBlendMaterialEdge(
                    ExplorationMaterial.CityPaving,
                    ExplorationMaterial.MarketCobbles,
                    true,
                    false,
                    false),
                "passable material feathering never crosses a blocked boundary");
            AssertEqual(3, ExplorationArtRules.MaterialBlendBandCount(), "material feather uses three restrained inward bands");
            AssertEqual(
                true,
                ExplorationArtRules.MaterialBlendBandFraction(true) > ExplorationArtRules.MaterialBlendBandFraction(false),
                "Region view gives material joins a slightly wider visual feather");
            AssertEqual(
                true,
                ExplorationArtRules.MaterialBlendBandAlpha(0, true) > ExplorationArtRules.MaterialBlendBandAlpha(1, true)
                    && ExplorationArtRules.MaterialBlendBandAlpha(1, true) > ExplorationArtRules.MaterialBlendBandAlpha(2, true),
                "material feather opacity falls off toward the cell interior");
            float blendFraction = ExplorationArtRules.MaterialBlendBandFraction(false);
            AssertEqual(
                true,
                Math.Abs(
                    ExplorationArtRules.MaterialBlendSourceStart(true, 0, blendFraction, false)
                    - (1f - blendFraction)) < 0.0001f,
                "an unflipped negative-axis neighbor samples its boundary-side source strip");
            AssertEqual(
                true,
                Math.Abs(
                    ExplorationArtRules.MaterialBlendSourceStart(false, 0, blendFraction, false))
                    < 0.0001f,
                "an unflipped positive-axis neighbor samples its boundary-side source strip");
            AssertEqual(
                true,
                Math.Abs(
                    ExplorationArtRules.MaterialBlendSourceStart(true, 0, blendFraction, true))
                    < 0.0001f,
                "a flipped negative-axis neighbor converts its logical boundary into source coordinates");
            AssertEqual(
                true,
                Math.Abs(
                    ExplorationArtRules.MaterialBlendSourceStart(false, 0, blendFraction, true)
                    - (1f - blendFraction)) < 0.0001f,
                "a flipped positive-axis neighbor converts its logical boundary into source coordinates");
            AssertEqual(true, ExplorationArtRules.MaterialBlendDrawFlip(false), "an unflipped source strip reverses toward the shared edge");
            AssertEqual(false, ExplorationArtRules.MaterialBlendDrawFlip(true), "an already flipped source strip reverses back toward the shared edge");
            AssertEqual(
                false,
                ExplorationArtRules.ShouldDrawMaterialPathStroke(
                    ExplorationMaterial.PackedDirt,
                    ExplorationCellRole.Road | ExplorationCellRole.Plaza),
                "packed-dirt approach plazas suppress their inherited per-cell road grid");
            AssertEqual(
                true,
                ExplorationArtRules.ShouldDrawMaterialPathStroke(
                    ExplorationMaterial.PackedDirt,
                    ExplorationCellRole.Road),
                "ordinary packed-dirt road cells retain their route stroke");
            AssertEqual(
                true,
                ExplorationArtRules.ShouldDrawMaterialPathStroke(
                    ExplorationMaterial.CityPaving,
                    ExplorationCellRole.Road | ExplorationCellRole.Plaza),
                "city plaza roads retain their deliberate street connection");

            AssertEqual(0, ExplorationArtRules.WorldMapTileIndex(1, "road", 80, false), "road maps to detailed road tile");
            AssertEqual(13, ExplorationArtRules.WorldMapTileIndex(1, "road", 80, true), "connected road junction maps to intersection art");
            AssertEqual(0, ExplorationArtRules.WorldMapTileIndex(1, "moss", 80, false), "passable moss uses quiet road ground, not baked shrine art");
            AssertEqual(3, ExplorationArtRules.WorldMapTileIndex(1, "mire", 80, true), "mire uses coherent wet ground, not a random scenic cell");
            AssertEqual(12, ExplorationArtRules.WorldMapTileIndex(1, "quarry", 80, false), "quarry defaults to repeatable rubble ground");
            AssertEqual(12, ExplorationArtRules.WorldMapTileIndex(1, "glass", 80, false), "glass reaches default to calm tinted rubble");
            AssertEqual(5, ExplorationArtRules.WorldMapTileIndex(1, "glass", 99, false), "glass crystal painting remains a rare accent");
            AssertEqual(12, ExplorationArtRules.WorldMapTileIndex(1, "ash", 80, false), "ash reaches use traversable rubble instead of barred-gate art");
            AssertEqual(1, ExplorationArtRules.WorldMapTileIndex(1, "ruins", 80, false), "Dusk Market defaults to calm repeatable paving");
            AssertEqual(21, ExplorationArtRules.WorldMapTileIndex(0, "forestwall", 80, false), "forest wall selects a deterministic dedicated blocked variant");
            AssertEqual(26, ExplorationArtRules.WorldMapTileIndex(0, "mirewall", 80, false), "mire wall selects a deterministic flooded variant");
            AssertEqual(31, ExplorationArtRules.WorldMapTileIndex(0, "cliffwall", 80, false), "cliff wall selects a deterministic escarpment variant");
            AssertEqual(10, ExplorationArtRules.WorldMapTileIndex(0, "redwall", 80, false), "red wall uses the dedicated blocked cell");
            AssertEqual(
                "7,20,21,22,23,24",
                string.Join(",", Enumerable.Range(0, 100).Select(roll => ExplorationArtRules.WorldMapTileIndex(0, "forestwall", roll, false)).Distinct().OrderBy(index => index)),
                "forest walls expose the approved cell plus five new variants");
            AssertEqual(
                "8,25,26,27,28,29",
                string.Join(",", Enumerable.Range(0, 100).Select(roll => ExplorationArtRules.WorldMapTileIndex(0, "mirewall", roll, false)).Distinct().OrderBy(index => index)),
                "mire walls expose the approved cell plus five new variants");
            AssertEqual(
                "9,30,31,32,33,34",
                string.Join(",", Enumerable.Range(0, 100).Select(roll => ExplorationArtRules.WorldMapTileIndex(0, "cliffwall", roll, false)).Distinct().OrderBy(index => index)),
                "cliff walls expose the approved cell plus five new variants");
            AssertEqual(
                "10,35,36,37,38",
                string.Join(",", Enumerable.Range(0, 100).Select(roll => ExplorationArtRules.WorldMapTileIndex(0, "redwall", roll, false)).Distinct().OrderBy(index => index)),
                "red walls expose the approved cell plus four new variants");
            AssertEqual(
                "9,30,31,32,33,34,39",
                string.Join(",", Enumerable.Range(0, 100).Select(roll => ExplorationArtRules.WorldMapTileIndex(0, "ruinswall", roll, false)).Distinct().OrderBy(index => index)),
                "ruined walls include their dedicated overgrown variant and compatible cliffs");
            AssertEqual(true, ExplorationArtRules.CanMirrorTerrain(1, "ruins"), "calm terrain can mirror to break repetition");
            AssertEqual(false, ExplorationArtRules.CanMirrorTerrain(1, "road"), "directional road art is not mirrored");
            AssertEqual(false, ExplorationArtRules.CanMirrorTerrain(0, "forestwall"), "blocked silhouettes are not mirrored");
            AssertEqual(1, ExplorationArtRules.TerrainMacroSize(1, "ruins"), "terrain art never expands into a 2x2 poster block");
            AssertEqual(1, ExplorationArtRules.TerrainMacroSize(1, "midgaard-paved"), "city paving stays one authored tile per map cell");
            AssertEqual(15, ExplorationArtRules.MidgaardTileIndex(1, "midgaard-road", 40, false), "city roads use calm paving beneath their topology overlay");
            AssertEqual(17, ExplorationArtRules.MidgaardTileIndex(1, "midgaard-plaza", 40, false), "city plazas use authored square paving");
            AssertEqual(13, ExplorationArtRules.WorldMapTileIndex(1, "road", 40, 15, false), "four-way semantic road uses junction art");
            AssertEqual(0, ExplorationArtRules.WorldMapTileIndex(1, "road", 40, 5, false), "straight semantic road avoids junction art");
            AssertEqual(12, ExplorationArtRules.WorldMapTileIndex(1, "glass", 99, 0, true), "object footprint forces calm compatible glass ground");
        }

        private static void ExplorationSurfaceRulesPreserveAuthoredMapStructure()
        {
            MapData map = new MapData { Width = 4, Height = 3, Depth = 2, StartX = 1, StartY = 1 };
            map.Tiles = new List<int>
            {
                0, 0, 0, 0,
                0, 1, 1, 1,
                0, 0, 0, 0
            };

            AssertEqual(true, ExplorationSurfaceRules.IsLoadableMap(map, false), "v18 topology is loadable without semantic surfaces");
            AssertEqual(false, ExplorationSurfaceRules.IsLoadableMap(map, true), "v19 map rejects a missing semantic surface grid");
            AssertEqual(true, ExplorationSurfaceRules.EnsureGrid(map), "missing semantic surface grid is reconstructed");
            AssertEqual(12, map.SurfaceMaterials.Count, "semantic material grid matches map dimensions");
            AssertEqual(12, map.SurfaceRoles.Count, "semantic role grid matches map dimensions");
            AssertEqual(ExplorationMaterial.NaturalGround, ExplorationSurfaceRules.MaterialAt(map, 0, 0), "blocked tile receives neutral material until migration classifies it");
            AssertEqual(ExplorationCellRole.None, ExplorationSurfaceRules.RolesAt(map, 1, 1), "open legacy tile receives no invented role");
            AssertEqual(true, ExplorationSurfaceRules.IsLoadableMap(map, true), "reconstructed semantic map satisfies v19 load contract");

            ExplorationSurfaceRules.SetMaterial(map, 1, 1, ExplorationMaterial.Moss);
            ExplorationSurfaceRules.SetRoles(map, 1, 1, ExplorationCellRole.Road);
            ExplorationSurfaceRules.SetRoles(map, 2, 1, ExplorationCellRole.Road);
            ExplorationSurfaceRules.SetRoles(map, 3, 1, ExplorationCellRole.Trail);
            AssertEqual(2, ExplorationSurfaceRules.PathNeighborMask(map, 1, 1), "road mask connects east");
            AssertEqual(8, ExplorationSurfaceRules.PathNeighborMask(map, 2, 1), "road-width mask does not expand through an adjacent trail cell");
            AssertEqual(10, ExplorationSurfaceRules.PathConnectorNeighborMask(map, 2, 1), "thin connector mask lets a trail terminate cleanly at the road");
            AssertEqual(8, ExplorationSurfaceRules.PathNeighborMask(map, 3, 1), "trail mask connects west");
            AssertEqual(8, ExplorationSurfaceRules.PathConnectorNeighborMask(map, 3, 1), "trail connector retains its road meeting");

            map.Tiles[6] = 0;
            ExplorationSurfaceRules.RepairConsistency(map);
            AssertEqual(ExplorationCellRole.Road, ExplorationSurfaceRules.RolesAt(map, 2, 1), "passability does not erase authored path role");
            AssertEqual(0, ExplorationSurfaceRules.PathNeighborMask(map, 1, 1), "blocked authored path is excluded from visible path topology");
            ExplorationSurfaceRules.SetMaterial(map, 2, 1, ExplorationMaterial.Forest);
            AssertEqual(ExplorationMaterial.Forest, ExplorationSurfaceRules.MaterialAt(map, 2, 1), "blocked geometry retains authored biome material");
            float offset = ExplorationSurfaceRules.SmoothBoundaryOffset(9, 17, 2.4f);
            AssertEqual(true, offset >= -2.4f && offset <= 2.4f, "visual zone boundary offset remains bounded");
            AssertEqual(offset, ExplorationSurfaceRules.SmoothBoundaryOffset(9, 17, 2.4f), "visual zone boundary offset is deterministic");
        }

        private static void CombatHudScreenLayoutFitsSupportedResolutions()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(1280, 720),
                new Vector2Int(1600, 900),
                new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152)
            };
            float[] minimumTileSizes = { 70f, 96f, 118f, 128f };

            for (int sizeIndex = 0; sizeIndex < sizes.Length; sizeIndex++)
            {
                Vector2Int size = sizes[sizeIndex];
                CombatHudGeometry geometry = CombatHudScreenLayout.Calculate(size.x, size.y);
                AssertEqual(true, geometry.Fits(size.x, size.y), $"combat HUD layout fits {size.x}x{size.y}");
                AssertEqual(true, geometry.Top.height >= 50f && geometry.Top.height <= 56f, $"combat top ribbon stays compact and readable at {size.x}x{size.y}");
                AssertEqual(true, geometry.Command.width >= 96f && geometry.Command.width <= 112f, $"combat commands use the compact vertical palette at {size.x}x{size.y}");
                AssertEqual(true, geometry.Side.width <= size.x * 0.22f, $"combat dossier leaves the battlefield as the visual hero at {size.x}x{size.y}");
                Rect grid = CombatHudScreenLayout.BoardInner(geometry.Board, 12, 8);
                float tileSize = Mathf.Min(grid.width / 12f, grid.height / 8f);
                AssertEqual(true, tileSize >= minimumTileSizes[sizeIndex], $"combat battlefield preserves large tiles at {size.x}x{size.y} ({tileSize:0.0}px)");
                AssertEqual(true, grid.width * grid.height >= size.x * size.y * 0.50f, $"combat battlefield owns at least half the frame at {size.x}x{size.y}");
                foreach (bool promoteEndTurn in new[] { false, true })
                {
                    Rect[] buttons = CombatHudScreenLayout.CommandButtons(geometry.Command.width, geometry.Command.height, promoteEndTurn);
                    AssertEqual(6, buttons.Length, "combat HUD keeps all six combat commands visible");
                    foreach (Rect button in buttons)
                    {
                        AssertEqual(true, button.xMin >= 0f && button.yMin >= 0f && button.xMax <= geometry.Command.width && button.yMax <= geometry.Command.height, $"combat command button fits {size.x}x{size.y} promoted={promoteEndTurn}");
                        AssertEqual(true, button.width >= 56f && button.height >= 80f, $"combat command keeps a generous pointer and controller target at {size.x}x{size.y}");
                        float iconSize = CombatHudScreenLayout.CommandIconSize(button);
                        AssertEqual(true, iconSize >= 52f, $"combat command art remains readable at {size.x}x{size.y}");
                        AssertEqual(true, 6f + iconSize + 27f <= button.height, $"combat command art clears its compact label stack at {size.x}x{size.y}");
                    }
                    AssertEqual(true, buttons[3].yMin - buttons[2].yMax > buttons[2].yMin - buttons[1].yMax, $"targeted and instant command groups stay visually separated at {size.x}x{size.y}");
                    AssertEqual(promoteEndTurn, buttons[5].height > buttons[4].height, $"end turn height promotion is intentional at {size.x}x{size.y}");
                }
                foreach (int commandCount in new[] { 0, 1, 4, 6 })
                {
                    Rect[] buttons = CombatHudScreenLayout.CommandButtons(geometry.Command.width, geometry.Command.height, commandCount, false);
                    AssertEqual(commandCount, buttons.Length, $"combat command geometry follows the rendered model count ({commandCount})");
                    foreach (Rect button in buttons)
                    {
                        AssertEqual(
                            true,
                            button.xMin >= 0f && button.yMin >= 0f && button.xMax <= geometry.Command.width && button.yMax <= geometry.Command.height,
                            $"data-driven combat command button fits {size.x}x{size.y} count={commandCount}");
                    }
                }
                float contextWidth = Mathf.Max(420f, geometry.Top.width * 0.45f);
                foreach (bool showUndoMove in new[] { false, true })
                foreach (bool showCancelTarget in new[] { false, true })
                {
                    Rect prompt = CombatHudScreenLayout.CommandPrompt(contextWidth, showUndoMove, showCancelTarget);
                    Rect undo = CombatHudScreenLayout.UndoMoveButton(contextWidth, showCancelTarget);
                    Rect cancel = CombatHudScreenLayout.CancelTargetButton(contextWidth);
                    AssertEqual(true, prompt.xMin >= 0f && prompt.xMax <= contextWidth && prompt.yMin >= 0f && prompt.yMax <= geometry.Top.height, $"combat prompt fits the top ribbon at {size.x}x{size.y} undo={showUndoMove} cancel={showCancelTarget}");
                    if (showUndoMove)
                    {
                        AssertEqual(true, undo.xMin >= 0f && undo.xMax <= contextWidth && undo.yMin >= 0f && undo.yMax <= geometry.Top.height, $"undo control fits {size.x}x{size.y}");
                        AssertEqual(true, prompt.xMax <= undo.xMin, $"undo control does not overlap the combat prompt at {size.x}x{size.y}");
                    }
                    if (showCancelTarget)
                    {
                        AssertEqual(true, cancel.xMin >= 0f && cancel.xMax <= contextWidth && cancel.yMin >= 0f && cancel.yMax <= geometry.Top.height, $"cancel control fits {size.x}x{size.y}");
                        Rect leftmost = showUndoMove ? undo : cancel;
                        AssertEqual(true, prompt.xMax <= leftmost.xMin, $"target cancel controls do not overlap the combat prompt at {size.x}x{size.y}");
                    }
                    if (showUndoMove && showCancelTarget)
                    {
                        AssertEqual(true, undo.xMax <= cancel.xMin, $"undo and target cancel controls do not overlap at {size.x}x{size.y}");
                    }
                }

                CombatHudScreenLayout.SidePanels(geometry.Side, false, out Rect active, out Rect target, out Rect timeline);
                AssertEqual(true, active.yMin >= 0f && target.yMin >= active.yMax && timeline.yMin >= target.yMax && timeline.yMax <= geometry.Side.height, $"combat side collapsed fits {size.x}x{size.y}");
                float collapsedTimelineHeight = timeline.height;
                AssertEqual(true, geometry.Side.height - timeline.yMax >= 100f, $"collapsed combat dossier leaves deliberate breathing room at {size.x}x{size.y}");
                AssertEqual(true, collapsedTimelineHeight >= 126f && collapsedTimelineHeight <= 154f, $"collapsed combat timeline shows initiative without reserving event-log space at {size.x}x{size.y}");
                foreach (bool showMana in new[] { false, true })
                {
                    CombatHudUnitCardGeometry activeCard = CombatHudScreenLayout.UnitCard(active.width, active.height, showMana);
                    CombatHudUnitCardGeometry targetCard = CombatHudScreenLayout.UnitCard(target.width, target.height, showMana);
                    AssertEqual(true, activeCard.Fits(active.width, active.height), $"active card rows do not overlap at {size.x}x{size.y} mana={showMana}");
                    AssertEqual(true, targetCard.Fits(target.width, target.height), $"target card rows do not overlap at {size.x}x{size.y} mana={showMana}");
                    foreach (CombatHudUnitCardGeometry card in new[] { activeCard, targetCard })
                    {
                        AssertEqual(true, card.Portrait.width >= 36f && Mathf.Abs(card.Portrait.width - card.Portrait.height) < 0.01f, $"combat portrait remains square and readable at {size.x}x{size.y} mana={showMana}");
                        AssertEqual(true, card.Portrait.xMax <= card.Name.xMin && card.Portrait.yMax <= card.Hp.yMin, $"combat portrait clears unit copy and meters at {size.x}x{size.y} mana={showMana}");
                    }
                }
                CombatHudScreenLayout.SidePanels(geometry.Side, true, out active, out target, out timeline);
                AssertEqual(true, active.yMin >= 0f && target.yMin >= active.yMax && timeline.yMin >= target.yMax && timeline.yMax <= geometry.Side.height, $"combat side expanded fits {size.x}x{size.y}");
                AssertEqual(true, timeline.height >= 284f && timeline.height >= collapsedTimelineHeight + 120f, $"expanded combat timeline becomes the intentional information drawer at {size.x}x{size.y}");
                foreach (bool showMana in new[] { false, true })
                {
                    AssertEqual(true, CombatHudScreenLayout.UnitCard(active.width, active.height, showMana).Fits(active.width, active.height), $"expanded active card rows do not overlap at {size.x}x{size.y} mana={showMana}");
                    AssertEqual(true, CombatHudScreenLayout.UnitCard(target.width, target.height, showMana).Fits(target.width, target.height), $"expanded target card rows do not overlap at {size.x}x{size.y} mana={showMana}");
                }
            }

            CombatHudGeometry compactGeometry = CombatHudScreenLayout.Calculate(960f, 600f);
            AssertEqual(true, compactGeometry.Fits(960f, 600f), "combat HUD remains bounded at the project's compact 960x600 window size");
            Rect[] compactButtons = CombatHudScreenLayout.CommandButtons(
                compactGeometry.Command.width,
                compactGeometry.Command.height,
                6,
                false);
            foreach (Rect button in compactButtons)
            {
                AssertEqual(true, CombatHudScreenLayout.UsesCompactCommandLayout(button), "compact combat window switches commands to the single-label layout");
                float iconSize = CombatHudScreenLayout.CommandIconSize(button);
                float labelBottom = 4f + iconSize + 1f + 15f;
                AssertEqual(true, labelBottom <= button.height - 3f, "compact combat command art and label stay inside their hit target");
            }
            CombatHudScreenLayout.SidePanels(compactGeometry.Side, false, out Rect compactActive, out Rect compactTarget, out Rect compactTimeline);
            AssertEqual(true, CombatHudScreenLayout.UnitCard(compactActive.width, compactActive.height, true).Fits(compactActive.width, compactActive.height), "compact fallback active card keeps portrait, text, and meters separated");
            AssertEqual(true, CombatHudScreenLayout.UnitCard(compactTarget.width, compactTarget.height, true).Fits(compactTarget.width, compactTarget.height), "compact fallback target card keeps portrait, text, and meters separated");
            AssertEqual(true, compactTimeline.yMax <= compactGeometry.Side.height, "compact dossier remains bounded beneath the adaptive unit cards");
        }

        private static void PauseMenuLayoutFitsSupportedResolutions()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(1280, 720),
                new Vector2Int(1600, 900),
                new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152)
            };

            foreach (Vector2Int size in sizes)
            {
                foreach (bool settingsOpen in new[] { false, true })
                {
                    PauseMenuGeometry geometry = PauseMenuScreenLayout.Calculate(size.x, size.y, settingsOpen);
                    AssertEqual(true, geometry.Fits(size.x, size.y), $"pause menu layout fits {size.x}x{size.y} settings={settingsOpen}");
                    for (int i = 0; i < 6; i++)
                    {
                        Rect button = PauseMenuScreenLayout.ButtonRect(geometry.Panel.width, i);
                        AssertEqual(true, button.xMin >= 0f && button.yMin >= 0f && button.xMax <= geometry.Panel.width && button.yMax <= geometry.Panel.height, $"pause menu button {i} fits {size.x}x{size.y}");
                    }
                }
            }
        }

        private static void CombatAbilityModalLayoutFitsSupportedResolutions()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(1280, 720),
                new Vector2Int(1600, 900),
                new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152)
            };

            foreach (Vector2Int size in sizes)
            {
                CombatAbilityModalGeometry geometry = CombatAbilityModalLayout.Calculate(size.x, size.y);
                AssertEqual(true, geometry.Fits(size.x, size.y), $"combat ability modal layout fits {size.x}x{size.y}");
                AssertEqual(true, geometry.CloseButton.width >= 200f && geometry.CloseButton.height >= 40f, $"combat ability modal Back/Esc control is readable at {size.x}x{size.y}");
                AssertEqual(true, geometry.Filters.height >= 40f && geometry.Filters.width == geometry.List.width, $"combat ability modal filters align to the list with a full control-height target at {size.x}x{size.y}");
                AssertEqual(true, geometry.List.width >= 540f && geometry.Detail.width >= 400f, $"combat ability modal keeps useful list and detail measures at {size.x}x{size.y}");
                AssertEqual(true, geometry.Detail.width >= geometry.List.width * 0.70f, $"combat ability modal gives the selected power a useful reading measure at {size.x}x{size.y}");
                AssertEqual(true, geometry.CloseButton.height >= 40f, $"combat ability modal close control keeps a full-height target at {size.x}x{size.y}");
                AssertEqual(true, geometry.Footer.yMin >= geometry.List.yMax && geometry.Footer.yMin >= geometry.Detail.yMax, $"combat ability modal footer stays below both columns at {size.x}x{size.y}");
                AssertEqual(true, geometry.Footer.width >= geometry.List.width + geometry.Detail.width, $"combat ability modal footer spans the complete book at {size.x}x{size.y}");

                float rowHeight = CombatAbilityModalListRules.RowHeight(size.y);
                float rowGap = CombatAbilityModalListRules.RowGap(size.y);
                float viewportHeight = geometry.List.height - 14f;
                float sixRowContent = CombatAbilityModalListRules.ContentHeight(
                    6,
                    rowHeight,
                    rowGap,
                    viewportHeight);
                AssertEqual(
                    true,
                    6f * (rowHeight + rowGap) + CombatAbilityModalListRules.ContentPadding <= viewportHeight,
                    $"combat ability modal fully fits six rows without clipping at {size.x}x{size.y}");
                AssertEqual(
                    "1–6 OF 6",
                    CombatAbilityModalListRules.ScrollLabel(
                        6,
                        rowHeight,
                        rowGap,
                        viewportHeight,
                        sixRowContent,
                        0f),
                    $"combat ability modal reports a truthful unscrolled six-row range at {size.x}x{size.y}");
            }

            CombatAbilityModalGeometry minimumGeometry = CombatAbilityModalLayout.Calculate(1280, 720);
            float minimumViewport = minimumGeometry.List.height - 14f;
            float minimumRowHeight = CombatAbilityModalListRules.RowHeight(720f);
            float minimumRowGap = CombatAbilityModalListRules.RowGap(720f);
            float longContent = CombatAbilityModalListRules.ContentHeight(
                18,
                minimumRowHeight,
                minimumRowGap,
                minimumViewport);
            AssertEqual(
                "1–6 OF 18  ↓",
                CombatAbilityModalListRules.ScrollLabel(
                    18,
                    minimumRowHeight,
                    minimumRowGap,
                    minimumViewport,
                    longContent,
                    0f),
                "combat ability modal top range counts only fully visible 720p rows and advertises content below");
            AssertEqual(
                "13–18 OF 18  ↑",
                CombatAbilityModalListRules.ScrollLabel(
                    18,
                    minimumRowHeight,
                    minimumRowGap,
                    minimumViewport,
                    longContent,
                    longContent - minimumViewport),
                "combat ability modal bottom range counts only fully visible 720p rows and advertises content above");
        }

        private static void CombatAbilityModalCardsExposeArtIdentity()
        {
            Type type = typeof(CombatAbilityModalCardView);
            AssertEqual(true, type.GetField("IconTexture") != null, "ability modal card icon texture contract");
            AssertEqual(true, type.GetField("IconSource") != null, "ability modal card icon source contract");
            AssertEqual(true, type.GetField("Sigil") != null, "ability modal card sigil contract");
            AssertEqual(true, type.GetField("AccentHex") != null, "ability modal card accent contract");
            AssertEqual(true, type.GetField("Impact") != null, "ability modal card impact contract");
            AssertEqual(true, type.GetField("UnlockLevel") != null, "ability modal card unlock-level contract");
            AssertEqual(true, type.GetField("Locked") != null, "ability modal card locked-state contract");
            AssertEqual(true, type.GetField("Focused") != null, "ability modal card focused-cast contract");
            AssertEqual(true, type.GetField("ResourceAfter") != null, "ability modal card projected-resource contract");
            AssertEqual(true, type.GetField("ValidTargetCount") != null, "ability modal card legal-target count contract");
            AssertEqual(true, type.GetField("TargetCountKnown") != null, "ability modal card target-enumeration state contract");
            AssertEqual(true, type.GetField("TacticalNote") != null, "ability modal card tactical requirement contract");
            AssertEqual(true, typeof(CombatAbilityModalView).GetField("ContextKey") != null, "ability modal scroll context distinguishes picker and combatant");
            AssertEqual(true, typeof(CombatAbilityModalView).GetField("Actor") != null, "ability modal structured actor header contract");
            AssertEqual(true, typeof(CombatAbilityModalView).GetField("Resource") != null, "ability modal structured resource header contract");
            AssertEqual(true, typeof(CombatAbilityModalView).GetField("StateIconTexture") != null, "ability modal exposes the optional power-book state atlas");
            AssertEqual(true, typeof(CombatAbilityModalBindings).GetField("PreviewCard") != null, "ability modal hover/focus preview binding contract");

            CombatAbilityModalCardView targeted = new CombatAbilityModalCardView
            {
                Id = "targeted",
                Kind = "Ember formula",
                Tier = "starter",
                Impact = "Single target",
                ResourceAfter = "7 MP after",
                Target = "enemy",
                Targeted = true,
                TargetCountKnown = true,
                ValidTargetCount = 2,
                Usable = true,
                RowSummary = "Shock one enemy.",
                CurrentEffect = "12-16 shock damage.",
                Detail = "CASTING RULES\nRequires sight.\n\nFORMULA NOTE\nDo not waste the opening."
            };
            CombatAbilityModalCardView skillDetail = new CombatAbilityModalCardView
            {
                Detail = "CURRENT EFFECT\n12 physical damage.\n\nTACTICS\nUse after the target is marked."
            };
            CombatAbilityModalCardView instant = new CombatAbilityModalCardView { Id = "instant", Targeted = false, Usable = true };
            CombatAbilityModalCardView whirlwind = new CombatAbilityModalCardView
            {
                Id = "whirlwind",
                Targeted = false,
                Usable = true,
                Impact = "Adjacent enemies"
            };
            CombatAbilityModalCardView ready = new CombatAbilityModalCardView
            {
                Id = "ready",
                Targeted = true,
                TargetCountKnown = true,
                ValidTargetCount = 1,
                Usable = true,
                Ready = true
            };
            CombatAbilityModalCardView locked = new CombatAbilityModalCardView
            {
                Id = "locked",
                Locked = true,
                UnlockLevel = 4,
                Cost = "13 MP",
                Range = "Range 7"
            };
            CombatAbilityModalCardView noTarget = new CombatAbilityModalCardView
            {
                Id = "no-target",
                Targeted = true,
                TargetCountKnown = true,
                ValidTargetCount = 0,
                Usable = true,
                TacticalNote = "Move closer."
            };
            CombatAbilityModalCardView lowMana = new CombatAbilityModalCardView
            {
                Id = "low-mana",
                Usable = false,
                DisabledReason = "Needs 3 more MP (2/5)."
            };
            CombatAbilityModalCardView staleReady = new CombatAbilityModalCardView
            {
                Id = "stale-ready",
                Ready = true,
                Usable = false,
                DisabledReason = "Needs 1 more MP (4/5)."
            };
            CombatAbilityModalCardView actionUsed = new CombatAbilityModalCardView
            {
                Id = "action-used",
                Usable = false,
                DisabledReason = "Action already used."
            };
            CombatAbilityModalCardView disabled = new CombatAbilityModalCardView
            {
                Id = "disabled",
                Ready = true,
                Targeted = true,
                TargetCountKnown = true,
                ValidTargetCount = 0,
                Usable = false,
                DisabledReason = "Stunned for 2 more turns."
            };
            CombatAbilityModalCardView blocked = new CombatAbilityModalCardView
            {
                Id = "blocked",
                Usable = false,
                DisabledReason = "Combat resolution pending."
            };
            CombatAbilityModalCardView readyWithoutTarget = new CombatAbilityModalCardView
            {
                Id = "ready-without-target",
                Ready = true,
                Targeted = true,
                TargetCountKnown = true,
                ValidTargetCount = 0,
                Usable = true,
                TacticalNote = "Step back into line of sight."
            };
            AssertEqual("Choose Target", CombatAbilityModalPresentationRules.CardActionLabel(targeted), "targeted modal card uses an explicit action verb");
            AssertEqual("Use Now", CombatAbilityModalPresentationRules.CardActionLabel(instant), "instant modal card uses an explicit action verb");
            AssertEqual("Resume Targeting", CombatAbilityModalPresentationRules.CardActionLabel(ready), "armed modal card resumes battlefield targeting");
            AssertEqual("Unlocks at Level 4", CombatAbilityModalPresentationRules.CardActionLabel(locked), "locked modal card names its unlock level");
            AssertEqual("LOCKED", CombatAbilityModalPresentationRules.AvailabilityLabel(locked), "locked cards use familiar plain language");
            AssertEqual("No Legal Target", CombatAbilityModalPresentationRules.CardActionLabel(noTarget), "zero-target card blocks preparation without assuming range is the cause");
            AssertEqual("NO TARGET", CombatAbilityModalPresentationRules.AvailabilityLabel(noTarget), "zero-target badge avoids prescribing the wrong remedy");
            AssertEqual("Needs 3 more MP (2/5)", CombatAbilityModalPresentationRules.CardActionLabel(lowMana), "short resource deficit remains explicit on the primary action");
            AssertEqual(CombatAbilityModalBookState.Unavailable, CombatAbilityModalPresentationRules.ResolveBookState(null), "missing cards resolve to the typed unavailable state");
            AssertEqual(CombatAbilityModalBookState.ReadyNow, CombatAbilityModalPresentationRules.ResolveBookState(targeted), "ordinary legal powers resolve to ready-now");
            AssertEqual(CombatAbilityModalBookState.Targeting, CombatAbilityModalPresentationRules.ResolveBookState(ready), "armed powers resolve to targeting");
            AssertEqual(CombatAbilityModalBookState.Locked, CombatAbilityModalPresentationRules.ResolveBookState(locked), "future powers resolve to locked");
            AssertEqual(CombatAbilityModalBookState.LowResource, CombatAbilityModalPresentationRules.ResolveBookState(lowMana), "mana deficits resolve to low-resource");
            AssertEqual(CombatAbilityModalBookState.NoTarget, CombatAbilityModalPresentationRules.ResolveBookState(noTarget), "empty target sets resolve to no-target");
            AssertEqual(CombatAbilityModalBookState.ActionUsed, CombatAbilityModalPresentationRules.ResolveBookState(actionUsed), "spent turns resolve to action-used");
            AssertEqual(CombatAbilityModalBookState.Disabled, CombatAbilityModalPresentationRules.ResolveBookState(disabled), "incapacitation resolves to disabled before stale targeting");
            AssertEqual(CombatAbilityModalBookState.Blocked, CombatAbilityModalPresentationRules.ResolveBookState(blocked), "other global gates resolve to blocked");
            AssertEqual(CombatIconCatalog.BookStateBlockedIndex, CombatAbilityModalPresentationRules.BookStateIconIndex(CombatAbilityModalBookState.Unavailable), "unavailable state uses the blocked fallback microicon");
            AssertEqual(CombatIconCatalog.BookStateSelectionIndex, CombatAbilityModalPresentationRules.BookStateIconIndex(CombatAbilityModalBookState.ReadyNow), "ready-now selection uses the committed selector microicon");
            AssertEqual(CombatIconCatalog.BookStateTargetingIndex, CombatAbilityModalPresentationRules.BookStateIconIndex(CombatAbilityModalBookState.Targeting), "targeting uses the targeting microicon");
            AssertEqual(CombatIconCatalog.BookStateLockedIndex, CombatAbilityModalPresentationRules.BookStateIconIndex(CombatAbilityModalBookState.Locked), "locked uses the locked microicon");
            AssertEqual(CombatIconCatalog.BookStateLowResourceIndex, CombatAbilityModalPresentationRules.BookStateIconIndex(CombatAbilityModalBookState.LowResource), "low-resource uses the low-resource microicon");
            AssertEqual(CombatIconCatalog.BookStateNoTargetIndex, CombatAbilityModalPresentationRules.BookStateIconIndex(CombatAbilityModalBookState.NoTarget), "no-target uses the no-target microicon");
            AssertEqual(CombatIconCatalog.BookStateActionUsedIndex, CombatAbilityModalPresentationRules.BookStateIconIndex(CombatAbilityModalBookState.ActionUsed), "action-used uses the action-used microicon");
            AssertEqual(CombatIconCatalog.BookStateDisabledIndex, CombatAbilityModalPresentationRules.BookStateIconIndex(CombatAbilityModalBookState.Disabled), "disabled uses the disabled microicon");
            AssertEqual(CombatIconCatalog.BookStateBlockedIndex, CombatAbilityModalPresentationRules.BookStateIconIndex(CombatAbilityModalBookState.Blocked), "blocked uses the blocked microicon");
            AssertEqual("Back to Battle  [Esc]", CombatAbilityModalPresentationRules.BackButtonLabel(new[] { targeted }), "ordinary modal back control names the destination and shortcut");
            AssertEqual("Back to Battle  [Esc]", CombatAbilityModalPresentationRules.BackButtonLabel(new[] { ready }), "armed and ordinary views keep one invariant exit");
            AssertEqual("", CombatAbilityModalPresentationRules.DetailPrompt(targeted), "Choose Target CTA does not repeat itself in the detail pane");
            AssertEqual("Move closer.", CombatAbilityModalPresentationRules.DetailPrompt(noTarget), "zero-target detail publishes its tactical remedy");
            AssertEqual("Shock one enemy.", targeted.RowSummary, "power row keeps one concise tactical summary");
            AssertEqual("12-16 shock damage.", targeted.CurrentEffect, "selected power exposes only its live resolved effect");
            AssertEqual(
                "Ember formula  •  Starter tier  •  Single target  •  7 MP after",
                CombatAbilityModalPresentationRules.DetailMeta(targeted),
                "selected power detail exposes identity, tier, footprint, and projected resource");
            AssertEqual(
                "Do not waste the opening.",
                CombatAbilityModalPresentationRules.DetailNotes(targeted),
                "formula detail keeps authored guidance without duplicating the structured casting profile");
            AssertEqual(
                "Use after the target is marked.",
                CombatAbilityModalPresentationRules.DetailNotes(skillDetail),
                "skill detail removes the repeated live-effect section");
            AssertEqual(true, CombatAbilityModalPresentationRules.CanActivate(targeted), "legal targeted power can activate");
            AssertEqual(true, CombatAbilityModalPresentationRules.CanActivate(instant), "usable instant power can activate");
            AssertEqual(false, CombatAbilityModalPresentationRules.CanActivate(noTarget), "targeted power with no current mark cannot activate");
            AssertEqual(false, CombatAbilityModalPresentationRules.CanActivate(lowMana), "unaffordable power cannot activate");
            AssertEqual(false, CombatAbilityModalPresentationRules.CanActivate(locked), "future power cannot activate");
            AssertEqual(true, CombatAbilityModalPresentationRules.CanActivate(ready), "already armed power with a live target can return to its target step");
            AssertEqual(false, CombatAbilityModalPresentationRules.CanActivate(staleReady), "an armed power that lost affordability cannot masquerade as actionable targeting");
            AssertEqual("Needs 1 more MP (4/5)", CombatAbilityModalPresentationRules.CardActionLabel(staleReady), "stale armed power exposes its new blocking requirement");
            AssertEqual(false, CombatAbilityModalPresentationRules.CanActivate(readyWithoutTarget), "armed targeting cannot bypass a newly empty legal-target set");
            AssertEqual("No Legal Target", CombatAbilityModalPresentationRules.CardActionLabel(readyWithoutTarget), "armed zero-target card exposes the actual blocker");
            AssertEqual("NO TARGET", CombatAbilityModalPresentationRules.AvailabilityLabel(readyWithoutTarget), "armed zero-target card does not retain a false targeting badge");
            AssertEqual("Step back into line of sight.", CombatAbilityModalPresentationRules.DetailPrompt(readyWithoutTarget), "armed zero-target detail publishes its tactical remedy");
            AssertEqual("Back to Battle  [Esc]", CombatAbilityModalPresentationRules.BackButtonLabel(new[] { readyWithoutTarget }), "dead armed targeting keeps the standard exit");
            AssertEqual("2 legal enemies", CombatAbilityModalPresentationRules.TargetCountLabel(targeted), "target count names the legal target type");
            AssertEqual("Immediate", CombatAbilityModalPresentationRules.TargetCountLabel(instant), "instant power avoids a fake target count");
            AssertEqual("Adjacent enemies", CombatAbilityModalPresentationRules.TargetCountLabel(whirlwind), "untargeted area skills describe their real footprint");
            AssertEqual(
                "Unlocks L4  •  13 MP  •  Range 7",
                CombatAbilityModalPresentationRules.RowMeta(locked),
                "locked modal rows expose their distinct unlock level beside ordinary cost and reach facts");
            AssertEqual(
                "",
                CombatAbilityModalPresentationRules.RowMeta(null),
                "missing modal rows do not publish placeholder progression metadata");
            AssertEqual(false, CombatAbilityModalPresentationRules.ShouldShowRowBadge(targeted, CombatAbilityModalFilter.All, "ACTION READY"), "ordinary usable rows stay visually quiet");
            AssertEqual(true, CombatAbilityModalPresentationRules.ShouldShowRowBadge(ready, CombatAbilityModalFilter.Ready, "ACTION READY"), "the one armed power keeps its targeting badge");
            AssertEqual(true, CombatAbilityModalPresentationRules.ShouldShowRowBadge(noTarget, CombatAbilityModalFilter.Learned, "ACTION READY"), "card-specific no-target state remains visible");
            AssertEqual(true, CombatAbilityModalPresentationRules.ShouldShowRowBadge(lowMana, CombatAbilityModalFilter.Learned, "ACTION READY"), "card-specific low-resource state remains visible");
            AssertEqual(true, CombatAbilityModalPresentationRules.ShouldShowRowBadge(locked, CombatAbilityModalFilter.All, "ACTION READY"), "All view labels locked cards");
            AssertEqual(false, CombatAbilityModalPresentationRules.ShouldShowRowBadge(locked, CombatAbilityModalFilter.Future, "ACTION READY"), "Locked view does not repeat LOCKED on every row");
            AssertEqual(false, CombatAbilityModalPresentationRules.ShouldShowRowBadge(actionUsed, CombatAbilityModalFilter.Learned, "ACTION USED"), "global action-used state does not repeat on every row");

            CombatAbilityModalCardView[] cards = { targeted, instant, ready, locked, noTarget, lowMana };
            AssertEqual(6, CombatAbilityModalPresentationRules.Count(cards, CombatAbilityModalFilter.All), "All filter retains complete book");
            AssertEqual(3, CombatAbilityModalPresentationRules.Count(cards, CombatAbilityModalFilter.Ready), "Ready filter keeps only actionable powers");
            AssertEqual(5, CombatAbilityModalPresentationRules.Count(cards, CombatAbilityModalFilter.Learned), "Learned filter excludes future unlocks");
            AssertEqual(1, CombatAbilityModalPresentationRules.Count(cards, CombatAbilityModalFilter.Future), "Future filter isolates progression");
            AssertEqual(CombatAbilityModalFilter.Ready, CombatAbilityModalPresentationRules.InitialFilter(cards), "combat picker opens on action-ready choices");
            AssertEqual(false, ready.Selected, "armed state remains separate from browse selection");
            AssertEqual("[1]  READY  3", CombatAbilityModalPresentationRules.FilterLabel(CombatAbilityModalFilter.Ready, cards), "Ready filter advertises key 1 and its count");
            AssertEqual("[2]  KNOWN  5", CombatAbilityModalPresentationRules.FilterLabel(CombatAbilityModalFilter.Learned, cards), "Known filter advertises key 2 and its count");
            AssertEqual("[3]  LOCKED  1", CombatAbilityModalPresentationRules.FilterLabel(CombatAbilityModalFilter.Future, cards), "Locked filter advertises key 3 and its count");
            AssertEqual("[4]  ALL  6", CombatAbilityModalPresentationRules.FilterLabel(CombatAbilityModalFilter.All, cards), "All filter advertises key 4 and its count");
            AssertEqual(
                "Back to Battle  [Esc]",
                CombatAbilityModalPresentationRules.BackButtonLabel(new[] { staleReady }),
                "stale armed power keeps the standard battle exit");
            AssertEqual(true, CombatAbilityModalDetailScrollRules.ApplyAxis(0.5f, 1f, 0.1f) > 0.5f, "right-stick detail input scrolls without changing row focus");
            AssertEqual(0.5f, CombatAbilityModalDetailScrollRules.ApplyAxis(0.5f, 0.1f, 0.1f), "right-stick dead zone prevents detail drift");
            AssertEqual(1f, CombatAbilityModalDetailScrollRules.ApplyPage(0.9f, 1), "detail Page Up clamps at the top");
            AssertEqual(0f, CombatAbilityModalDetailScrollRules.ApplyPage(0.1f, -1), "detail Page Down clamps at the bottom");
        }

        private static void CombatAbilityModalNavigationRepeatsPredictably()
        {
            CombatAbilityModalNavigationStep neutral =
                CombatAbilityModalNavigationRules.SeedVertical(0f, 10f);
            AssertEqual(0, neutral.Direction, "neutral book input does not move");
            AssertEqual(0, neutral.HeldDirection, "neutral book input does not retain a direction");
            AssertEqual(0f, neutral.NextRepeatAt, "neutral book input has no repeat deadline");

            CombatAbilityModalNavigationStep heldAtOpen =
                CombatAbilityModalNavigationRules.SeedVertical(1f, 10f);
            AssertEqual(0, heldAtOpen.Direction, "held input at book open does not jump the initial selection");
            AssertEqual(-1, heldAtOpen.HeldDirection, "positive vertical input maps to the previous row");
            AssertEqual(
                10f + CombatAbilityModalNavigationRules.InitialRepeatDelay,
                heldAtOpen.NextRepeatAt,
                "held input at book open receives the normal initial repeat delay");

            CombatAbilityModalNavigationStep beforeDelay =
                CombatAbilityModalNavigationRules.ResolveVertical(
                    1f,
                    heldAtOpen.HeldDirection,
                    heldAtOpen.NextRepeatAt,
                    10.2f);
            AssertEqual(0, beforeDelay.Direction, "held input waits through the initial repeat delay");

            CombatAbilityModalNavigationStep repeated =
                CombatAbilityModalNavigationRules.ResolveVertical(
                    1f,
                    beforeDelay.HeldDirection,
                    beforeDelay.NextRepeatAt,
                    heldAtOpen.NextRepeatAt);
            AssertEqual(-1, repeated.Direction, "held input repeats toward the previous row");
            AssertEqual(
                heldAtOpen.NextRepeatAt + CombatAbilityModalNavigationRules.RepeatInterval,
                repeated.NextRepeatAt,
                "held input switches to the faster repeat interval");

            CombatAbilityModalNavigationStep reversed =
                CombatAbilityModalNavigationRules.ResolveVertical(
                    -1f,
                    repeated.HeldDirection,
                    repeated.NextRepeatAt,
                    11f);
            AssertEqual(1, reversed.Direction, "reversing the stick immediately moves toward the next row");
            AssertEqual(1, reversed.HeldDirection, "reversing the stick replaces the held direction");

            CombatAbilityModalNavigationStep released =
                CombatAbilityModalNavigationRules.ResolveVertical(
                    0f,
                    reversed.HeldDirection,
                    reversed.NextRepeatAt,
                    11.1f);
            AssertEqual(0, released.Direction, "releasing the stick does not move");
            AssertEqual(0, released.HeldDirection, "releasing the stick resets repeat state");
            AssertEqual(0f, released.NextRepeatAt, "releasing the stick clears the repeat deadline");

            CombatAbilityModalNavigationStep freshPress =
                CombatAbilityModalNavigationRules.ResolveVertical(1f, 0, 0f, 12f);
            AssertEqual(-1, freshPress.Direction, "a fresh press still moves exactly once immediately");
        }

        private static void CombatTargetHighlightsRequireAnArmedPower()
        {
            AssertEqual(
                true,
                CombatTargetingRules.ShouldDrawTargetHighlights(ActionMode.Attack, false, false),
                "ordinary weapon targeting keeps its board highlights");
            AssertEqual(
                false,
                CombatTargetingRules.ShouldDrawTargetHighlights(ActionMode.Cast, false, false),
                "opening the Spellbook cannot paint ghost cast targets before a formula is armed");
            AssertEqual(
                true,
                CombatTargetingRules.ShouldDrawTargetHighlights(ActionMode.Cast, true, false),
                "an armed resolved formula owns cast target highlights");
            AssertEqual(
                false,
                CombatTargetingRules.ShouldDrawTargetHighlights(ActionMode.Ability, false, false),
                "opening the Skillbook cannot paint ghost skill targets before a skill is armed");
            AssertEqual(
                true,
                CombatTargetingRules.ShouldDrawTargetHighlights(ActionMode.Ability, false, true),
                "an armed resolved skill owns ability target highlights");
            AssertEqual(
                false,
                CombatTargetingRules.ShouldDrawTargetHighlights(ActionMode.Move, true, true),
                "non-targeting modes ignore stale power references");
            AssertEqual(
                true,
                CombatTargetingRules.RoutesUnitPreviewToSideRail(ActionMode.Attack, true),
                "unit attack previews use the persistent target rail instead of covering the board");
            AssertEqual(
                true,
                CombatTargetingRules.RoutesUnitPreviewToSideRail(ActionMode.Cast, true),
                "unit spell previews use the persistent target rail instead of covering the board");
            AssertEqual(
                false,
                CombatTargetingRules.RoutesUnitPreviewToSideRail(ActionMode.Move, true),
                "movement keeps its destination tooltip");
            AssertEqual(
                false,
                CombatTargetingRules.RoutesUnitPreviewToSideRail(ActionMode.Cast, false),
                "tile-targeted powers keep their necessary board tooltip");
            Rect tooltipBoard = new Rect(0f, 0f, 800f, 480f);
            Rect tooltipDecision = new Rect(350f, 200f, 80f, 80f);
            List<Rect> tooltipBlockers = new List<Rect>
            {
                new Rect(8f, 8f, 320f, 100f),
                new Rect(8f, 372f, 320f, 100f),
                new Rect(472f, 372f, 320f, 100f)
            };
            Rect placedTooltip = CombatTargetingRules.PlaceBoardTooltip(
                tooltipBoard,
                tooltipDecision,
                tooltipDecision.center,
                320f,
                100f,
                tooltipBlockers);
            AssertEqual(true, placedTooltip.xMin >= tooltipBoard.xMin
                && placedTooltip.yMin >= tooltipBoard.yMin
                && placedTooltip.xMax <= tooltipBoard.xMax
                && placedTooltip.yMax <= tooltipBoard.yMax, "combat board tooltip remains inside the tactical map");
            AssertEqual(true, placedTooltip.xMin > tooltipDecision.xMax
                && placedTooltip.yMin < tooltipDecision.yMin, "combat board tooltip selects the unobstructed corner away from the decision and combatants");
        }

        private static void AdvancedCasterProgressionPowersAreExplicit()
        {
            FormulaDef thunderStep = FormulaCatalog.All.First(formula => formula.Code == "VST");
            FormulaDef tempest = FormulaCatalog.All.First(formula => formula.Code == "AST");
            FormulaDef ascendance = FormulaCatalog.All.First(formula => formula.Code == "DFA");
            FormulaDef riftSeal = FormulaCatalog.All.First(formula => formula.Code == "SRF");
            HashSet<string> validTargets = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase) { "ally", "enemy", "self", "tile" };
            HashSet<string> validEffects = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase) { "chain", "cure", "damage", "dispel", "drain", "heal", "status", "summon", "tempest", "terrain", "teleport", "thunderclap", "transform" };

            AssertEqual(4, FormulaCatalog.RequiredLevel(thunderStep), "Thunder Step unlock level");
            AssertEqual(6, FormulaCatalog.RequiredLevel(tempest), "Arcane Tempest unlock level");
            AssertEqual(6, FormulaCatalog.RequiredLevel(ascendance), "Abyssal Ascendance unlock level");
            AssertEqual(3, FormulaCatalog.RequiredLevel(riftSeal), "Rift Seal unlock level");
            AssertEqual("teleport", thunderStep.Effect, "Thunder Step has an explicit teleport effect");
            AssertEqual("transform", ascendance.Effect, "Abyssal Ascendance has an explicit transform effect");
            AssertEqual("dispel", riftSeal.Effect, "Rift Seal has an explicit dispel effect");
            AssertEqual(true, tempest.Effect == "tempest" && tempest.Power >= 18, "Arcane Tempest is an elder radius-area spell");
            AssertEqual(CombatPowerFootprintKind.Placement, CombatPowerTargetingRules.ForFormula(thunderStep).Kind, "Thunder Step previews a destination tile");
            AssertEqual(CombatPowerFootprintKind.RadiusArea, CombatPowerTargetingRules.ForFormula(tempest).Kind, "Arcane Tempest previews its radius-two storm");
            AssertEqual(CombatPowerFootprintKind.SelfArea, CombatPowerTargetingRules.ForFormula(ascendance).Kind, "Abyssal Ascendance previews self transformation");
            AssertEqual(3, CombatPowerPresentationRules.FormulaIntensity(tempest), "Arcane Tempest uses epic presentation");
            AssertEqual(3, CombatPowerPresentationRules.FormulaIntensity(ascendance), "Abyssal Ascendance uses epic presentation");
            AssertEqual(2, CombatPowerPresentationRules.FormulaIntensity(riftSeal), "Rift Seal uses a strong readable presentation");
            AssertEqual(true, CombatImpactRules.ForFormula(tempest).BurstCount >= 30, "Arcane Tempest owns a full elder impact profile");
            AssertEqual(true, CombatImpactRules.ForFormula(ascendance).AftershockVolume >= 0.60f, "Abyssal Ascendance owns a layered transformation aftershock");
            AssertEqual("resonance", CombatImpactRules.ForFormula(riftSeal).AftershockSfx, "Rift Seal owns a sealing resonance layer");
            AssertEqual(true, CombatIconCatalog.SignatureSpellIndex("VST") >= 0 && CombatIconCatalog.SignatureSpellIndex("DFA") >= 0, "advanced powers have runtime icon placeholders");
            AssertEqual(false, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, "DFA"), "late pact transformation stays outside the first sewer slice");
            AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.FullPrototype, "DFA"), "late pact transformation is available in prototype testing");
            AssertEqual(true, FormulaCatalog.All.All(formula => validTargets.Contains(formula.Target ?? "")), "every formula target passes the runtime catalog contract");
            AssertEqual(true, FormulaCatalog.All.All(formula => validEffects.Contains(formula.Effect ?? "")), "every formula effect passes the runtime catalog contract");
        }

        private static void DemonFormPackageDefinesCohesiveProgression()
        {
            FormulaDef riftBolt = FormulaCatalog.All.Single(formula => formula.Code == "RBT");
            FormulaDef riftStep = FormulaCatalog.All.Single(formula => formula.Code == "VRS");
            AssertEqual(true,
                riftBolt.Name == "Rift Bolt"
                && FormulaCatalog.RequiredLevel(riftBolt) == 1
                && riftBolt.School == "pact"
                && riftBolt.Skill == "hex"
                && riftBolt.Mana == 4
                && riftBolt.Range == 5
                && riftBolt.Target == "enemy"
                && riftBolt.Effect == "damage"
                && riftBolt.DamageType == "death"
                && riftBolt.Power == 8,
                "Rift Bolt fills the pact starter-damage gap");
            AssertEqual(true,
                riftStep.Name == "Rift Step"
                && FormulaCatalog.RequiredLevel(riftStep) == 4
                && riftStep.School == "pact"
                && riftStep.Skill == "hex"
                && riftStep.Mana == 6
                && riftStep.Range == 5
                && riftStep.Target == "tile"
                && riftStep.Effect == "teleport"
                && riftStep.DamageType == "death"
                && riftStep.Arc,
                "Rift Step fills the level-four pact mobility gap");
            AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, "RBT"), "Rift Bolt is available to the sewer-slice warlock");
            AssertEqual(false, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, "VRS"), "Rift Step remains a later progression power");
            AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.FullPrototype, "VRS"), "Rift Step is available in the full prototype");
            AssertEqual(CombatPowerFootprintKind.Single, CombatPowerTargetingRules.ForFormula(riftBolt).Kind, "Rift Bolt is a direct target spell");
            AssertEqual("STEP|Teleport destination", CombatPowerTargetingRules.ForFormula(riftStep).BoardLabel + "|" + CombatPowerTargetingRules.ForFormula(riftStep).ModalLabel, "Rift Step previews its open destination");
            AssertEqual(2, CombatPowerPresentationRules.FormulaIntensity(riftStep), "Rift Step receives a strong mobility presentation");
            AssertEqual("castpact|veilstep|resonance", CombatImpactRules.ForFormula(riftStep).CastSfx + "|" + CombatImpactRules.ForFormula(riftStep).ImpactSfx + "|" + CombatImpactRules.ForFormula(riftStep).AftershockSfx, "Rift Step owns a pact-rift audio identity");

            string[] demonIds = AbilityCatalog.IdsForClass("demon").ToArray();
            AssertEqual("riftpounce,abyssalwhirl,soulrend,dreadroar", string.Join(",", demonIds), "demon form exposes four stable Demon Arts");
            MartialAbility riftPounce = AbilityCatalog.For("riftpounce");
            MartialAbility abyssalWhirl = AbilityCatalog.For("abyssalwhirl");
            MartialAbility soulRend = AbilityCatalog.For("soulrend");
            MartialAbility dreadRoar = AbilityCatalog.For("dreadroar");
            AssertEqual(true, demonIds.Select(AbilityCatalog.For).All(ability => ability != null && ability.ClassKey == "demon" && ability.RequiredLevel == 1), "every Demon Art is derived from the active transformation rather than permanent class progression");
            AssertEqual(true, riftPounce.Targeted && riftPounce.Range == 5, "Rift Pounce is the ranged landing art");
            AssertEqual(true, !abyssalWhirl.Targeted && abyssalWhirl.Range == 1, "Abyssal Whirl is an adjacent self-area art");
            AssertEqual(true, soulRend.Targeted && soulRend.Range == 1, "Soul Rend is an adjacent single-target art");
            AssertEqual(true, !dreadRoar.Targeted && dreadRoar.Range == 1, "Dread Roar is an adjacent control art");
            AssertEqual("RIFT|Rift + landing", CombatPowerTargetingRules.ForAbility(riftPounce).BoardLabel + "|" + CombatPowerTargetingRules.ForAbility(riftPounce).ModalLabel, "Rift Pounce previews its landing tile");
            AssertEqual(CombatPowerFootprintKind.SelfArea, CombatPowerTargetingRules.ForAbility(abyssalWhirl).Kind, "Abyssal Whirl previews all adjacent foes");
            AssertEqual(CombatPowerFootprintKind.Single, CombatPowerTargetingRules.ForAbility(soulRend).Kind, "Soul Rend previews one adjacent foe");
            AssertEqual("DREAD|Adjacent enemies", CombatPowerTargetingRules.ForAbility(dreadRoar).BoardLabel + "|" + CombatPowerTargetingRules.ForAbility(dreadRoar).ModalLabel, "Dread Roar previews its adjacent control field");
            AssertEqual(true, demonIds.All(id => CombatPowerPresentationRules.AbilityIntensity(id) == 3), "Demon Arts receive transformation-scale presentation");
            AssertEqual("c6576d", CombatPowerPresentationRules.AbilityAccent("demon"), "Demon Arts own a distinct blood-rift accent");
            AssertEqual(
                "Rift,Slash,Void,Ascendance",
                string.Join(",", demonIds.Select(id => CombatPowerVisualRules.MotifFor(id))),
                "Demon Arts keep four readable visual motifs");
            AssertEqual(
                "13,8,15,9",
                string.Join(",", demonIds.Select(CombatPowerVisualRules.EffectAtlasCell)),
                "Demon Arts route to distinct primary effect cells");
            AssertEqual(true, demonIds.All(CombatPowerVisualRules.UsesRitualCastPresentation), "Demon Arts receive a brief transformation-aware anticipation beat");
            AssertEqual(true, demonIds.All(id =>
            {
                CombatImpactArtPlan plan = CombatPowerVisualRules.ImpactArtPlan(id, 3, 0.5f);
                return plan.HasPrimary && plan.HasSecondary && plan.SecondaryScale > plan.PrimaryScale;
            }), "every Demon Art layers bounded epic impact art around its primary silhouette");

            CombatImpactProfile[] demonImpacts = demonIds
                .Select(id => CombatImpactRules.ForAbility(AbilityCatalog.For(id)))
                .ToArray();
            AssertEqual(
                "riftpounce|abyssalwhirl|soulrend|dreadroar",
                string.Join("|", demonImpacts.Select(profile => profile.CastSfx)),
                "each Demon Art owns its authored release cue");
            AssertEqual(
                "riftpounceimpact|abyssalwhirlimpact|soulrendimpact|dreadroarimpact",
                string.Join("|", demonImpacts.Select(profile => profile.ImpactSfx)),
                "each Demon Art owns its authored impact cue");
            AssertEqual(true, demonImpacts.All(profile => profile.VisualTier == 3 && profile.BurstCount >= 26), "Demon Arts keep a bounded but emphatic impact profile");

            CombatUnit warlock = new CombatUnit { Name = "Pact Tester", Role = "hex", ClassKey = "warlock", Spell = "hex|pact" };
            IReadOnlyList<CombatCommandEntry> mortalCommands = CombatCommandPresentationRules.PrimaryCommandsFor(warlock);
            AssertEqual(true, mortalCommands[2].Mode == ActionMode.Cast && mortalCommands[2].Label == "Spells", "mortal warlock keeps the Spellbook command");
            warlock.DemonFormTurns = 4;
            IReadOnlyList<CombatCommandEntry> demonCommands = CombatCommandPresentationRules.PrimaryCommandsFor(warlock);
            AssertEqual(true, demonCommands[2].Mode == ActionMode.Ability && demonCommands[2].Label == "Demon Arts", "transformation replaces Spellbook with Demon Arts");
            AssertEqual("demon", CreatureAudioRules.FactionFor(warlock), "transformed warlock adopts demon audio identity");
            AssertEqual("demonattack", CreatureAudioRules.CueFor(warlock, "attack"), "transformed warlock routes demon attack voice");
            warlock.DemonFormTurns = 0;
            AssertEqual(true, CombatCommandPresentationRules.PrimaryCommandsFor(warlock)[2].Mode == ActionMode.Cast, "expiring the form restores the Spellbook command");
        }

        private static void LightningPowerRulesDefineTacticalStormLadder()
        {
            FormulaDef arcSpark = FormulaCatalog.All.Single(formula => formula.Code == "RIG");
            FormulaDef thunderclap = FormulaCatalog.All.Single(formula => formula.Code == "RSG");
            FormulaDef chainLightning = FormulaCatalog.All.Single(formula => formula.Code == "CLT");
            FormulaDef thunderStep = FormulaCatalog.All.Single(formula => formula.Code == "VST");
            FormulaDef tempest = FormulaCatalog.All.Single(formula => formula.Code == "AST");

            AssertEqual("Arc Spark", arcSpark.Name, "starter lightning spell name");
            AssertEqual(true, FormulaCatalog.RequiredLevel(arcSpark) == 1
                && arcSpark.Mana == 3
                && arcSpark.Range == 5
                && arcSpark.Target == "enemy"
                && arcSpark.Effect == "damage"
                && arcSpark.DamageType == "shock"
                && arcSpark.Power == 8
                && !arcSpark.Arc
                && !arcSpark.Splash, "Arc Spark is the reliable direct starter shock");
            AssertEqual("Thunderclap", thunderclap.Name, "self-area lightning spell name");
            AssertEqual(true, FormulaCatalog.RequiredLevel(thunderclap) == 2
                && thunderclap.Mana == 6
                && thunderclap.Range == 0
                && thunderclap.Target == "self"
                && thunderclap.Effect == "thunderclap"
                && thunderclap.DamageType == "shock"
                && thunderclap.Power == 8, "Thunderclap is the adjacent push-and-collision spell");
            AssertEqual("Chain Lightning", chainLightning.Name, "chain lightning spell name");
            AssertEqual(true, FormulaCatalog.RequiredLevel(chainLightning) == 3
                && chainLightning.Mana == 9
                && chainLightning.Range == 6
                && chainLightning.Target == "enemy"
                && chainLightning.Effect == "chain"
                && chainLightning.DamageType == "shock"
                && chainLightning.Power == 14
                && chainLightning.Arc, "Chain Lightning is the four-target formation spell");
            AssertEqual("Thunder Step", thunderStep.Name, "teleport lightning spell name");
            AssertEqual(true, FormulaCatalog.RequiredLevel(thunderStep) == 4
                && thunderStep.Mana == 8
                && thunderStep.Range == 6
                && thunderStep.Target == "tile"
                && thunderStep.Effect == "teleport"
                && thunderStep.DamageType == "shock"
                && thunderStep.Power == 8
                && thunderStep.Arc, "Thunder Step is the damaging arrival teleport");
            AssertEqual("Arcane Tempest", tempest.Name, "elder lightning spell name");
            AssertEqual(true, FormulaCatalog.RequiredLevel(tempest) == 6
                && tempest.Mana == 14
                && tempest.Range == 6
                && tempest.Target == "enemy"
                && tempest.Effect == "tempest"
                && tempest.DamageType == "shock"
                && tempest.Power == 18
                && tempest.Arc
                && !tempest.Splash, "Arcane Tempest owns its custom radius resolver");
            AssertEqual(true, new[] { arcSpark, thunderclap, chainLightning, thunderStep, tempest }
                .All(formula => formula.School == "ember" && formula.Skill == "ember"), "the storm ladder advances one coherent mage craft");

            AssertEqual(4, LightningPowerRules.MaximumChainTargets, "Chain Lightning target cap");
            AssertEqual(2, LightningPowerRules.NormalJumpRange, "normal lightning jump range");
            AssertEqual(3, LightningPowerRules.ConductiveJumpRange, "conductive lightning jump range");
            AssertEqual(2, LightningPowerRules.TempestRadius, "Arcane Tempest radius");
            AssertEqual(20, LightningPowerRules.ChainDamage(20, 0), "chain primary damage");
            AssertEqual(15, LightningPowerRules.ChainDamage(20, 1), "chain second-target falloff");
            AssertEqual(11, LightningPowerRules.ChainDamage(20, 2), "chain third-target falloff");
            AssertEqual(8, LightningPowerRules.ChainDamage(20, 3), "chain fourth-target falloff");
            AssertEqual(8, LightningPowerRules.ChainDamage(20, 99), "chain damage remains capped at fourth-target falloff");
            AssertEqual(20, LightningPowerRules.ChainDamage(20, -1), "negative chain indices safely resolve as the primary");
            AssertEqual(3, LightningPowerRules.ChainDamage(1, 1), "chain jumps preserve a readable minimum");
            AssertEqual(8, LightningPowerRules.ThunderclapDamage(16), "Thunderclap uses half base damage");
            AssertEqual(3, LightningPowerRules.ThunderclapDamage(1), "Thunderclap minimum damage");
            AssertEqual(8, LightningPowerRules.ThunderStepDamage(16), "Thunder Step arrival uses half base damage");
            AssertEqual(20, LightningPowerRules.TempestDamage(20, true), "Tempest center keeps full damage");
            AssertEqual(12, LightningPowerRules.TempestDamage(20, false), "Tempest outer radius uses sixty-percent damage");
            AssertEqual(4, LightningPowerRules.TempestDamage(1, false), "Tempest outer radius minimum damage");
            AssertEqual(8, LightningPowerRules.CollisionDamage(16), "blocked lightning push collision damage");
            AssertEqual(4, LightningPowerRules.CollisionDamage(1), "collision minimum damage");
            AssertEqual(true, LightningPowerRules.IsConductiveTerrain("ice"), "ice extends lightning chains");
            AssertEqual(true, LightningPowerRules.IsConductiveTerrain("GAS"), "gas extends lightning chains case-insensitively");
            AssertEqual(true, LightningPowerRules.IsConductiveTerrain("web"), "web extends lightning chains");
            AssertEqual(false, LightningPowerRules.IsConductiveTerrain("smoke"), "smoke obscures but does not conduct lightning");
            AssertEqual(false, LightningPowerRules.IsConductiveTerrain(null), "empty terrain does not conduct lightning");

            AssertEqual(true, CombatTerrainRules.BlocksMovement("tree") && CombatTerrainRules.BlocksSight("tree"), "tree cover blocks movement and sight");
            AssertEqual(true, CombatTerrainRules.BlocksMovement("STONE") && CombatTerrainRules.BlocksSight("STONE"), "stone cover blocks movement and sight case-insensitively");
            AssertEqual(false, CombatTerrainRules.BlocksMovement("smoke"), "smoke leaves movement open");
            AssertEqual(true, CombatTerrainRules.BlocksSight(" smoke "), "smoke blocks sight after normalization");
            AssertEqual(false, CombatTerrainRules.BlocksMovement("gas") || CombatTerrainRules.BlocksSight("gas"), "poison gas remains a hazard rather than hard cover");
            AssertEqual(false, CombatTerrainRules.BlocksMovement(null) || CombatTerrainRules.BlocksSight(null), "empty terrain blocks neither movement nor sight");
        }

        private static void CombatPowerPresentationMakesSignaturesDistinct()
        {
            FormulaDef spark = FormulaCatalog.All.First(formula => formula.Code == "FIF");
            FormulaDef fireball = FormulaCatalog.All.First(formula => formula.Code == "FBL");
            FormulaDef meteor = FormulaCatalog.All.First(formula => formula.Code == "MTR");
            FormulaDef greaterDemon = FormulaCatalog.All.First(formula => formula.Code == "IBG");
            FormulaDef[] lightning = new[] { "RIG", "RSG", "CLT", "VST", "AST" }
                .Select(code => FormulaCatalog.All.First(formula => formula.Code == code))
                .ToArray();

            AssertEqual(1, CombatPowerPresentationRules.FormulaIntensity(spark), "starter spell impact intensity");
            AssertEqual(3, CombatPowerPresentationRules.FormulaIntensity(fireball), "fireball receives signature spell intensity");
            AssertEqual(3, CombatPowerPresentationRules.FormulaIntensity(meteor), "meteor impact intensity");
            AssertEqual(3, CombatPowerPresentationRules.FormulaIntensity(greaterDemon), "greater demon impact intensity");
            AssertEqual(false, CombatPowerPresentationRules.FormulaAccent(spark) == CombatPowerPresentationRules.FormulaAccent(greaterDemon), "ember and pact spell accents differ");
            AssertEqual("1,2,2,2,3", string.Join(",", lightning.Select(CombatPowerPresentationRules.FormulaIntensity)), "storm presentation intensity escalates by spell tier");
            AssertEqual(true, lightning.All(formula => CombatPowerPresentationRules.FormulaAccent(formula) == "d7b94e"), "storm spells share a readable lightning accent");

            CombatPowerIdentity formulaIdentity = CombatPowerPresentationRules.ForFormula(fireball, "Luma", "Ratfolk Brute");
            AssertEqual("Fireball", formulaIdentity.Title, "formula cue title");
            AssertEqual("FBL", formulaIdentity.Sigil, "formula cue sigil");
            AssertEqual(true, formulaIdentity.Subtitle.Contains("Luma") && formulaIdentity.Subtitle.Contains("Ratfolk Brute"), "formula cue names actor and target");
            CombatPowerIdentity focusedFormulaIdentity = CombatPowerPresentationRules.ForFormula(fireball, "Luma", "Ratfolk Brute", true);
            AssertEqual(true, focusedFormulaIdentity.Subtitle.Contains("FOCUSED"), "focused casting is named in the power cue");
            AssertEqual(true, focusedFormulaIdentity.Duration > formulaIdentity.Duration, "focused casting receives a slightly longer readable cue");

            MartialAbility aimed = AbilityCatalog.For("aimedshot");
            MartialAbility charge = AbilityCatalog.For("charge");
            MartialAbility whirlwind = AbilityCatalog.For("whirlwind");
            AssertEqual(1, CombatPowerPresentationRules.AbilityIntensity(aimed.Id), "aimed shot impact intensity");
            AssertEqual(2, CombatPowerPresentationRules.AbilityIntensity(charge.Id), "charge impact intensity");
            AssertEqual(3, CombatPowerPresentationRules.AbilityIntensity(whirlwind.Id), "whirlwind impact intensity");

            CombatPowerIdentity abilityIdentity = CombatPowerPresentationRules.ForAbility(charge, "Maer", "Ratfolk Brute");
            AssertEqual("Charge", abilityIdentity.Title, "ability cue title");
            AssertEqual("CHG", abilityIdentity.Sigil, "ability cue sigil");
            AssertEqual(true, abilityIdentity.Subtitle.Contains("Maer") && abilityIdentity.Subtitle.Contains("Ratfolk Brute"), "ability cue names actor and target");

            foreach (string code in ContentSetCatalog.SewerSliceFormulaCodes)
            {
                CombatPowerIdentity identity = CombatPowerPresentationRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == code), "Caster", "Target");
                AssertEqual(true, !string.IsNullOrWhiteSpace(identity.Title) && !string.IsNullOrWhiteSpace(identity.Sigil) && !string.IsNullOrWhiteSpace(identity.AccentHex), "slice formula identity " + code);
                AssertEqual(true, identity.Intensity >= 1 && identity.Intensity <= 3, "slice formula intensity " + code);
            }

            foreach (string id in ContentSetCatalog.SewerSliceAbilityIds)
            {
                CombatPowerIdentity identity = CombatPowerPresentationRules.ForAbility(AbilityCatalog.For(id), "Hero", "Target");
                AssertEqual(true, !string.IsNullOrWhiteSpace(identity.Title) && !string.IsNullOrWhiteSpace(identity.Sigil) && !string.IsNullOrWhiteSpace(identity.AccentHex), "slice ability identity " + id);
                AssertEqual(true, identity.Intensity >= 1 && identity.Intensity <= 3, "slice ability intensity " + id);
            }
        }

        private static void CombatPowerVisualMotifsStaySemanticAndBounded()
        {
            AssertEqual(CombatPowerVisualMotif.Fire, CombatPowerVisualRules.MotifFor("fireball"), "fireball uses fire motif");
            AssertEqual(CombatPowerVisualMotif.Void, CombatPowerVisualRules.MotifFor("deathburst"), "death burst uses void motif");
            AssertEqual(CombatPowerVisualMotif.Rift, CombatPowerVisualRules.MotifFor("greatersummon"), "greater summon uses rift motif");
            AssertEqual(CombatPowerVisualMotif.Ascendance, CombatPowerVisualRules.MotifFor("ascendance"), "ascendance uses transformation motif");
            AssertEqual(CombatPowerVisualMotif.Shock, CombatPowerVisualRules.MotifFor("tempest"), "tempest uses shock motif");
            AssertEqual(CombatPowerVisualMotif.Holy, CombatPowerVisualRules.MotifFor("riftseal"), "rift seal uses holy motif");
            AssertEqual(CombatPowerVisualMotif.Slash, CombatPowerVisualRules.MotifFor("whirlwindimpact"), "whirlwind uses slash motif");
            AssertEqual(CombatPowerVisualMotif.Shadow, CombatPowerVisualRules.MotifFor("ambushimpact"), "ambush uses shadow motif");
            AssertEqual(CombatPowerVisualMotif.Charge, CombatPowerVisualRules.MotifFor("chargeimpact"), "charge uses charge motif");
            AssertEqual(CombatPowerVisualMotif.Guard, CombatPowerVisualRules.MotifFor("shieldbash"), "shield skills use guard motif");
            AssertEqual(CombatPowerVisualMotif.Guard, CombatPowerVisualRules.MotifFor("counter"), "shield counters use guard motif");
            AssertEqual(CombatPowerVisualMotif.Volley, CombatPowerVisualRules.MotifFor("arrowrain"), "ranger barrages use volley motif");
            AssertEqual(CombatPowerVisualMotif.Nature, CombatPowerVisualRules.MotifFor("websnare"), "web binding uses a nature motif");
            AssertEqual(CombatPowerVisualMotif.Smoke, CombatPowerVisualRules.MotifFor("sleepmist"), "sleep magic uses a smoke motif");
            AssertEqual(CombatPowerVisualMotif.Void, CombatPowerVisualRules.MotifFor("voidhex"), "mind hexes use a void motif");
            AssertEqual(CombatPowerVisualMotif.Shadow, CombatPowerVisualRules.MotifFor("shadowveil"), "veil magic uses a shadow motif");
            AssertEqual(2, CombatPowerVisualRules.EffectAtlasCell("fireball"), "fireball art cell");
            AssertEqual(1, CombatPowerVisualRules.ProjectileAtlasCell("fireball", 0.25f), "fireball travel uses flame projectile art");
            AssertEqual(2, CombatPowerVisualRules.ProjectileAtlasCell("fireball", 0.90f), "fireball arrival resolves into its orb art");
            AssertEqual(3, CombatPowerVisualRules.ImpactAtlasCell("fireball", 0f), "fireball arrival opens on explosion art");
            AssertEqual(3, CombatPowerVisualRules.ImpactAtlasCell("fireball", 0.25f), "fireball impact uses explosion art");
            AssertEqual(4, CombatPowerVisualRules.ImpactAtlasCell("fireball", 0.90f), "fireball aftermath uses smoke art");
            AssertEqual(1, CombatPowerVisualRules.LayeredImpactAtlasCell(CombatPowerVisualMotif.Fire, 0.25f), "fire impact layers the dedicated explosion sheet");
            AssertEqual(14, CombatPowerVisualRules.LayeredImpactAtlasCell(CombatPowerVisualMotif.Fire, 0.90f), "fire aftermath layers smoke");
            AssertEqual(CombatPowerVisualRules.FireballTravelDuration, CombatPowerVisualRules.BeamDuration("fireball"), "fireball travel duration has one canonical rule");
            AssertEqual(true, CombatPowerVisualRules.ProjectileArcHeight("fireball") > 0.35f, "fireball travels on a readable arc");
            AssertEqual(15, CombatPowerVisualRules.EffectAtlasCell("deathburst"), "death burst art cell");
            AssertEqual(13, CombatPowerVisualRules.EffectAtlasCell("greatersummon"), "greater summon art cell");
            AssertEqual(10, CombatPowerVisualRules.EffectAtlasCell("tempest"), "tempest art cell");
            AssertEqual(14, CombatPowerVisualRules.EffectAtlasCell("riftseal"), "rift seal art cell");
            AssertEqual(true, CombatPowerVisualRules.UsesRitualCastPresentation("castember"), "spell casts retain ritual anticipation");
            AssertEqual(true, CombatPowerVisualRules.UsesRitualCastPresentation("casthex"), "dark spell casts retain ritual anticipation");
            AssertEqual(false, CombatPowerVisualRules.UsesRitualCastPresentation("whirlwind"), "martial skills omit ritual anticipation");
            AssertEqual(false, CombatPowerVisualRules.UsesRitualCastPresentation("rally"), "martial support skills omit ritual anticipation");
            AssertEqual(1, CombatPowerVisualRules.AnticipationRingCount(CombatPowerVisualMotif.Fire, 3), "epic magic keeps one readable contracting telegraph");
            AssertEqual(1, CombatPowerVisualRules.AnticipationRingCount(CombatPowerVisualMotif.Volley, 3), "epic martial telegraphs stay directional and restrained");
            AssertEqual(true, CombatPowerVisualRules.ImpactArtScale("fireball", 3, 0.25f) <= 1.45f, "fireball art stays inside the adjacent-unit readability cap");
            AssertEqual(true, CombatPowerVisualRules.ImpactArtScale("meteor", 3, 0.75f) <= 1.50f, "meteor art stays inside the adjacent-unit readability cap");
            AssertEqual(true, CombatPowerVisualRules.ImpactArtScale("tempest", 3, 0.75f) <= 1.32f, "ordinary impact art remains compact");
            AssertEqual(false, CombatPowerVisualRules.UsesLayeredImpactArt(CombatPowerVisualMotif.Fire, 2), "ordinary magic uses one effect-art layer");
            AssertEqual(true, CombatPowerVisualRules.UsesLayeredImpactArt(CombatPowerVisualMotif.Fire, 3), "signature magic may use the epic secondary atlas");
            AssertEqual(false, CombatPowerVisualRules.UsesLayeredImpactArt(CombatPowerVisualMotif.Slash, 3), "epic martial skills do not inherit spell atlas overlays");
            AssertEqual(false, CombatPowerVisualRules.UsesLayeredImpactArt(CombatPowerVisualMotif.Smoke, 3), "smoke effects remain a single readable art layer");
            CombatImpactArtPlan fireballPlan = CombatPowerVisualRules.ImpactArtPlan("fireball", 3, 0.25f);
            AssertEqual(true, fireballPlan.HasPrimary && fireballPlan.HasSecondary, "signature fireball render plan draws both atlases");
            AssertEqual(3, fireballPlan.PrimaryCell, "signature fireball keeps the crisp primary impact frame");
            AssertEqual(1, fireballPlan.SecondaryCell, "signature fireball adds its epic explosion layer");
            AssertEqual(true, fireballPlan.SecondaryScale > fireballPlan.PrimaryScale, "epic aura art sits behind and beyond the primary impact");
            AssertEqual(true, fireballPlan.SecondaryScale >= 1.54f && fireballPlan.SecondaryScale <= 2.18f, "epic secondary art stays within battlefield readability bounds");
            AssertEqual(true, fireballPlan.SecondaryOpacity > 0.30f && fireballPlan.SecondaryOpacity <= 0.58f, "epic secondary art remains visible without washing out sprites");
            AssertEqual(false, CombatPowerVisualRules.ImpactArtPlan("fireball", 2, 0.25f).HasSecondary, "ordinary fireball impact remains single-layered");
            AssertEqual(false, CombatPowerVisualRules.ImpactArtPlan("whirlwindimpact", 3, 0.25f).HasSecondary, "martial signature render plans never borrow spell overlays");

            FormulaDef webSnare = FormulaCatalog.All.First(formula => formula.Code == "WBK");
            FormulaDef sleep = FormulaCatalog.All.First(formula => formula.Code == "RMS");
            FormulaDef weaken = FormulaCatalog.All.First(formula => formula.Code == "RNH");
            FormulaDef nightVeil = FormulaCatalog.All.First(formula => formula.Code == "NVL");
            FormulaDef bind = FormulaCatalog.All.First(formula => formula.Code == "RKW");
            FormulaDef mindBreak = FormulaCatalog.All.First(formula => formula.Code == "RMB");
            FormulaDef dreamSmoke = FormulaCatalog.All.First(formula => formula.Code == "DSM");
            AssertEqual("websnare", CombatPowerVisualRules.ImpactKindForFormula(webSnare, "fieldsnare"), "Web Snare keeps semantic impact art even when audio is generic");
            AssertEqual("sleepmist", CombatPowerVisualRules.ImpactKindForFormula(sleep, "spell"), "Sleep keeps semantic impact art");
            AssertEqual("voidhex", CombatPowerVisualRules.ImpactKindForFormula(weaken, "spell"), "Weaken keeps semantic impact art");
            AssertEqual("shadowveil", CombatPowerVisualRules.ImpactKindForFormula(nightVeil, "spell"), "Night Veil keeps semantic impact art");
            AssertEqual("websnare", CombatPowerVisualRules.ImpactKindForFormula(bind, "spell"), "Bind keeps semantic impact art");
            AssertEqual("voidhex", CombatPowerVisualRules.ImpactKindForFormula(mindBreak, "spell"), "Mind Break keeps semantic impact art");
            AssertEqual("sleepmist", CombatPowerVisualRules.ImpactKindForFormula(dreamSmoke, "spell"), "Dream Smoke keeps semantic impact art");
            AssertEqual("ember", CombatPowerVisualRules.AftermathParticleKind(CombatPowerVisualMotif.Fire), "fire aftermath sheds embers");
            AssertEqual("shard", CombatPowerVisualRules.AftermathParticleKind(CombatPowerVisualMotif.Frost), "frost aftermath sheds shards");
            AssertEqual("streak", CombatPowerVisualRules.AftermathParticleKind(CombatPowerVisualMotif.Volley), "volley aftermath remains martial");
            foreach (CombatPowerVisualMotif motif in Enum.GetValues(typeof(CombatPowerVisualMotif)))
            {
                AssertEqual(true, CombatPowerVisualRules.AftermathParticleCount(motif, 3) >= 5 && CombatPowerVisualRules.AftermathParticleCount(motif, 3) <= 13, "combat aftermath remains bounded for " + motif);
                AssertEqual(true, CombatPowerVisualRules.AftermathParticleSpeed(motif, 3) >= 0.30f && CombatPowerVisualRules.AftermathParticleSpeed(motif, 3) <= 1.40f, "combat aftermath speed remains bounded for " + motif);
            }
            foreach (CombatPowerVisualMotif motif in Enum.GetValues(typeof(CombatPowerVisualMotif)))
            {
                float opacity = CombatPowerVisualRules.EffectOpacity(motif);
                AssertEqual(true, opacity >= 0.35f && opacity <= 0.80f, "combat effect opacity remains art-first for " + motif);
            }
        }

        private static void CombatUnitPresentationBeatsStaySynchronizedAndBounded()
        {
            CombatUnitPresentationBeat hit = CombatUnitPresentationRules.Create(
                "target",
                CombatUnitPresentationBeatKind.Hit,
                2f,
                1f);
            AssertEqual(true, CombatUnitPresentationRules.ShouldRenderActor(true, hit, 1.9f), "living target remains visible before staged impact");
            CombatUnitPresentationPose hitPose = CombatUnitPresentationRules.PoseFor(hit, 2.06f, false);
            AssertEqual(true, hitPose.OffsetX > 0f && hitPose.Scale > 1f, "target recoils and punches up on impact");
            AssertEqual(true, CombatUnitPresentationRules.PoseFor(hit, 2.06f, true).Scale == 1f, "Reduced Motion keeps a neutral combatant pose");

            CombatUnitPresentationBeat defeat = CombatUnitPresentationRules.Create(
                "defeated",
                CombatUnitPresentationBeatKind.Defeat,
                3f,
                -1f);
            AssertEqual(true, CombatUnitPresentationRules.ShouldRenderActor(false, defeat, 2.9f), "lethal target is held visibly until impact");
            AssertEqual(false, CombatUnitPresentationRules.ShouldRenderTacticalOverlay(false, defeat, 2.9f), "dead target does not retain tactical HP/status chrome");
            CombatUnitPresentationPose defeatPose = CombatUnitPresentationRules.PoseFor(defeat, 3.135f, false);
            AssertEqual(true, defeatPose.OffsetX < 0f && defeatPose.OffsetY > 0f && defeatPose.Alpha < 1f, "defeat beat recoils, falls, and fades");
            AssertEqual(false, CombatUnitPresentationRules.ShouldRenderActor(false, defeat, defeat.Until + 0.01f), "defeated sprite clears after its bounded fall");

            CombatUnitPresentationBeat reveal = CombatUnitPresentationRules.Create(
                "summon",
                CombatUnitPresentationBeatKind.Reveal,
                4f);
            AssertEqual(false, CombatUnitPresentationRules.ShouldRenderActor(true, reveal, 3.9f), "summon exists in state but remains hidden before ritual impact");
            AssertEqual(false, CombatUnitPresentationRules.ShouldRenderTacticalOverlay(true, reveal, 3.9f), "summon tactical chrome waits for materialization");
            AssertEqual(true, CombatUnitPresentationRules.ShouldRenderActor(true, reveal, 4f), "summon becomes visible on the ritual beat");
            AssertEqual(true, CombatUnitPresentationRules.PoseFor(reveal, 4.03f, false).Scale < 1f, "summon scales into the battlefield after impact");

            List<CombatUnitPresentationBeat> beats = new List<CombatUnitPresentationBeat>();
            CombatUnitPresentationRules.AddBounded(beats, hit, 0f);
            CombatUnitPresentationRules.AddBounded(beats, defeat, 0f);
            CombatUnitPresentationRules.AddBounded(beats, reveal, 0f);
            AssertEqual(true, CombatUnitPresentationRules.RemainingHoldDuration(beats, 3.95f) > 0f, "combat advance can honor the final visible presentation hold");
            List<CombatUnitPresentationBeat> lateSequence = new List<CombatUnitPresentationBeat>
            {
                CombatUnitPresentationRules.Create("late-defeat", CombatUnitPresentationBeatKind.Defeat, 0.50f)
            };
            AssertEqual(true, CombatUnitPresentationRules.RemainingHoldDuration(lateSequence, 0f) > 0.80f, "late chain defeat keeps its full impact and fall beat");
            for (int i = 0; i < 40; i++)
            {
                CombatUnitPresentationRules.AddBounded(
                    beats,
                    CombatUnitPresentationRules.Create("bounded-" + i, CombatUnitPresentationBeatKind.Hit, 10f + i * 0.01f),
                    0f);
            }
            AssertEqual(true, beats.Count <= CombatUnitPresentationRules.MaxActiveBeats, "combatant presentation beats remain globally bounded");
            CombatUnitPresentationRules.PruneAndBound(beats, 20f);
            AssertEqual(0, beats.Count, "expired combatant presentation beats prune cleanly");
        }

        private static void CombatPowerResolutionTimingIsBriefAndScaled()
        {
            FormulaDef spark = FormulaCatalog.All.First(formula => formula.Code == "FIF");
            FormulaDef fireball = FormulaCatalog.All.First(formula => formula.Code == "FBL");
            FormulaDef meteor = FormulaCatalog.All.First(formula => formula.Code == "MTR");
            MartialAbility aimed = AbilityCatalog.For("aimedshot");
            MartialAbility charge = AbilityCatalog.For("charge");
            MartialAbility whirlwind = AbilityCatalog.For("whirlwind");

            AssertEqual(true, Math.Abs(CombatPowerResolutionRules.DelayForFormula(spark, false) - 0.30f) < 0.0001f, "starter spell resolution holds through its aligned delivery and aftershock");
            AssertEqual(0.58f, CombatPowerResolutionRules.DelayForFormula(fireball, false), "fireball resolution holds through impact and aftershock");
            AssertEqual(0.60f, CombatPowerResolutionRules.DelayForFormula(meteor, false), "meteor resolution beat");
            AssertEqual(true, Math.Abs(CombatPowerResolutionRules.DelayForAbility(aimed, false) - 0.30f) < 0.0001f, "aimed shot resolution holds through projectile arrival");
            AssertEqual(0.44f, CombatPowerResolutionRules.DelayForAbility(charge, false), "charge resolution beat");
            AssertEqual(0.56f, CombatPowerResolutionRules.DelayForAbility(whirlwind, false), "whirlwind resolution beat");
            AssertEqual(0.06f, CombatPowerResolutionRules.DelayForFormula(meteor, true), "reduced motion spell resolution beat");
            AssertEqual(0.06f, CombatPowerResolutionRules.DelayForAbility(whirlwind, true), "reduced motion ability resolution beat");
            AssertEqual(true, CombatPowerResolutionRules.DelayForIntensity(1, false) < CombatPowerResolutionRules.DelayForIntensity(2, false), "power resolution tier one before tier two");
            AssertEqual(true, CombatPowerResolutionRules.DelayForIntensity(2, false) < CombatPowerResolutionRules.DelayForIntensity(3, false), "power resolution tier two before tier three");
            AssertEqual(true, CombatPowerResolutionRules.DelayForIntensity(3, false) <= 0.60f, "power resolution remains brief");
        }

        private static void CombatImpactProfilesStageSignaturePowers()
        {
            AssertEqual(true,
                typeof(PowerImpactEcho).GetField("StaticStamp")?.FieldType == typeof(bool)
                && typeof(PowerImpactEcho).GetField("ImpactAt")?.FieldType == typeof(float)
                && typeof(PowerImpactEcho).GetField("Duration")?.FieldType == typeof(float),
                "power impact echoes expose an explicit static-stamp timing contract for Reduced Motion");
            PowerImpactEcho staticStamp = new PowerImpactEcho
            {
                Kind = "fireball",
                StaticStamp = true,
                Start = 2f,
                ImpactAt = 2f,
                Duration = 0.16f
            };
            AssertEqual(true,
                staticStamp.StaticStamp
                && staticStamp.ImpactAt == staticStamp.Start
                && staticStamp.Duration > 0f,
                "a static impact stamp is immediate, finite, and structurally distinct from an animated echo");
            CombatImpactProfile heal = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "OIC"));
            CombatImpactProfile tree = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "GBH"));
            CombatImpactProfile fireball = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "FBL"));
            CombatImpactProfile meteor = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "MTR"));
            CombatImpactProfile deathBurst = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "RLM"));
            CombatImpactProfile imp = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "IBD"));
            CombatImpactProfile greaterDemon = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "IBG"));
            CombatImpactProfile arcSpark = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "RIG"));
            CombatImpactProfile thunderclap = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "RSG"));
            CombatImpactProfile chainLightning = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "CLT"));
            CombatImpactProfile thunderStep = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "VST"));
            CombatImpactProfile tempest = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "AST"));
            CombatImpactProfile ascendance = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "DFA"));
            CombatImpactProfile riftSeal = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "SRF"));

            AssertEqual("heal", heal.ImpactSfx, "heal impact sound identity");
            AssertEqual(0f, heal.ShakeMagnitude, "heal keeps board steady");
            AssertEqual("tree", tree.ImpactSfx, "tree cover impact sound identity");
            AssertEqual("fireball", fireball.ImpactSfx, "fireball impact sound identity");
            AssertEqual(3, fireball.VisualTier, "fireball explicitly owns signature visual scale");
            AssertEqual(CombatPowerVisualRules.BeamDuration("fireball"), fireball.ImpactDelay, "fireball impact aligns with projectile arrival");
            AssertEqual("meteor", meteor.ImpactSfx, "meteor impact sound identity");
            AssertEqual("fieldfire", meteor.AftershockSfx, "meteor aftershock sound identity");
            AssertEqual(CombatPowerVisualRules.BeamDuration("heal"), heal.ImpactDelay, "healing arc arrives on its impact beat");
            AssertEqual(CombatPowerVisualRules.BeamDuration("meteor"), meteor.ImpactDelay, "meteor arrives on its impact beat");
            AssertEqual("deathburst", deathBurst.ImpactSfx, "death burst impact sound identity");
            AssertEqual("greatersummon", greaterDemon.ImpactSfx, "greater demon impact sound identity");
            AssertEqual(true, new[] { arcSpark, thunderclap, chainLightning, thunderStep }
                .All(profile => profile.CastSfx == "castshock"), "lightning progression shares a coherent storm casting voice");
            AssertEqual("shock", arcSpark.ImpactSfx, "Arc Spark shock impact identity");
            AssertEqual("shock", thunderclap.ImpactSfx, "Thunderclap shock impact identity");
            AssertEqual("shock", chainLightning.ImpactSfx, "Chain Lightning shock impact identity");
            AssertEqual("veilstep", thunderStep.ImpactSfx, "Thunder Step travel impact identity");
            AssertEqual("tempest", tempest.ImpactSfx, "arcane tempest impact sound identity");
            AssertEqual("1,2,2,2,3", string.Join(",", new[] { arcSpark.VisualTier, thunderclap.VisualTier, chainLightning.VisualTier, thunderStep.VisualTier, tempest.VisualTier }), "lightning VFX scale climbs from spark to elder storm");
            AssertEqual(CombatPowerVisualRules.BeamDuration("lightning"), arcSpark.ImpactDelay, "Arc Spark impact aligns with its lightning beam");
            AssertEqual(CombatPowerVisualRules.BeamDuration("thunderclap"), thunderclap.ImpactDelay, "Thunderclap impact aligns with its radial burst");
            AssertEqual(CombatPowerVisualRules.BeamDuration("lightning"), chainLightning.ImpactDelay, "Chain Lightning first impact aligns with its beam");
            AssertEqual(CombatPowerVisualRules.BeamDuration("arc"), thunderStep.ImpactDelay, "Thunder Step arrival aligns with its travel arc");
            AssertEqual(true, arcSpark.BurstCount < thunderclap.BurstCount
                && thunderclap.BurstCount <= chainLightning.BurstCount
                && chainLightning.BurstCount < tempest.BurstCount, "lightning impact volume escalates with tactical scope");
            AssertEqual("ascendance", ascendance.ImpactSfx, "abyssal ascendance impact sound identity");
            AssertEqual("riftseal", riftSeal.ImpactSfx, "rift seal impact sound identity");
            AssertEqual(true, meteor.BurstCount > fireball.BurstCount && meteor.ShakeMagnitude > fireball.ShakeMagnitude, "meteor exceeds fireball impact scale");
            AssertEqual(true, greaterDemon.BurstCount > imp.BurstCount && greaterDemon.ResolutionDelay > imp.ResolutionDelay, "greater demon exceeds imp summoning impact");
            AssertEqual(fireball.ImpactDelay, CombatImpactRules.SequenceImpactDelay(fireball, 0), "sequence starts at primary impact");
            AssertEqual(true, CombatImpactRules.SequenceImpactDelay(meteor, 1) > CombatImpactRules.SequenceImpactDelay(meteor, 0), "meteor impacts advance in sequence");
            AssertEqual(true, CombatImpactRules.SequenceImpactDelay(meteor, 8) <= meteor.ResolutionDelay - 0.08f + 0.0001f, "meteor sequence stays inside resolution beat");

            CombatImpactProfile charge = CombatImpactRules.ForAbility(AbilityCatalog.For("charge"));
            CombatImpactProfile whirlwind = CombatImpactRules.ForAbility(AbilityCatalog.For("whirlwind"));
            CombatImpactProfile execute = CombatImpactRules.ForAbility(AbilityCatalog.For("execute"));
            CombatImpactProfile ambush = CombatImpactRules.ForAbility(AbilityCatalog.For("ambush"));
            CombatImpactProfile eviscerate = CombatImpactRules.ForAbility(AbilityCatalog.For("eviscerate"));
            CombatImpactProfile volley = CombatImpactRules.ForAbility(AbilityCatalog.For("volley"));
            AssertEqual("charge", charge.CastSfx, "charge cast sound identity");
            AssertEqual("whirlwind", whirlwind.CastSfx, "whirlwind cast sound identity");
            AssertEqual("chargeimpact", charge.ImpactSfx, "charge impact sound identity");
            AssertEqual("whirlwindimpact", whirlwind.ImpactSfx, "whirlwind impact sound identity");
            AssertEqual("executeimpact", execute.ImpactSfx, "execute impact sound identity");
            AssertEqual("ambushimpact", ambush.ImpactSfx, "ambush impact sound identity");
            AssertEqual("eviscerateimpact", eviscerate.ImpactSfx, "eviscerate impact sound identity");
            AssertEqual("arrowrain", volley.ImpactSfx, "volley impact sound identity");
            AssertEqual(false, charge.ImpactSfx == whirlwind.ImpactSfx, "signature martial impacts stay distinct");
            AssertEqual(CombatPowerVisualRules.AbilityDeliveryDuration("charge"), charge.ImpactDelay, "charge arrives on its impact beat");
            AssertEqual(CombatPowerVisualRules.AbilityDeliveryDuration("volley"), volley.ImpactDelay, "volley arrows arrive on their impact beat");
            AssertEqual(true, CombatImpactRules.AftermathDelay(meteor) >= meteor.ImpactDelay, "aftermath begins after the impact beat");
            AssertEqual(true, CombatImpactRules.ImpactFrameDuration(meteor) > 0f && CombatImpactRules.ImpactFrameDuration(meteor) <= 0.14f, "impact frame remains brief");

            foreach (FormulaDef formula in FormulaCatalog.All)
            {
                CombatImpactProfile profile = CombatImpactRules.ForFormula(formula);
                AssertEqual(true, !string.IsNullOrWhiteSpace(profile.ImpactSfx), "formula impact cue " + formula.Code);
                AssertEqual(true, profile.ImpactDelay >= 0f && profile.AftershockDelay >= profile.ImpactDelay, "formula staged cue order " + formula.Code);
                AssertEqual(true, profile.BurstCount >= 0 && profile.BurstCount <= 32, "formula impact burst bounds " + formula.Code);
                AssertEqual(true, profile.ResolutionDelay >= 0.06f && profile.ResolutionDelay <= 0.60f, "formula resolution bounds " + formula.Code);
            }
        }

        private static void CombatFieldsUseDistinctVisualAndAudioProfiles()
        {
            string[] fields = { "fire", "ice", "gas", "web", "sanctuary", "curse" };
            List<CombatFieldPresentationProfile> profiles = fields.Select(CombatFieldPresentationRules.For).ToList();
            AssertEqual(true, fields.All(CombatFieldPresentationRules.IsPersistentField), "all tactical fields use persistent terrain identity");
            AssertEqual(true, fields.All(CombatFieldPresentationRules.UsesDedicatedGroundSprite), "all tactical fields restore one authored ground sprite");
            AssertEqual(false, CombatFieldPresentationRules.IsPersistentField("tree"), "breakable cover remains outside persistent field rendering");
            AssertEqual(true, CombatFieldPresentationRules.UsesBaseTileDecoration(null), "empty combat cells keep quiet floor decoration");
            AssertEqual(false, CombatFieldPresentationRules.UsesBaseTileDecoration("fire"), "persistent fields suppress duplicate floor decoration");
            AssertEqual(false, CombatFieldPresentationRules.UsesBaseTileDecoration("tree"), "cover suppresses duplicate floor decoration");
            AssertEqual(true, CombatFieldPresentationRules.UsesDedicatedGroundSprite("fire"), "persistent fields restore their authored terrain sprite");
            AssertEqual(false, CombatFieldPresentationRules.UsesDedicatedGroundSprite("stone"), "cover uses the zone floor beneath one foreground sprite");
            AssertEqual(true, CombatFieldPresentationRules.UsesDedicatedGroundSprite("glyph"), "ritual marks retain their dedicated ground identity");
            AssertEqual(false, CombatFieldPresentationRules.UsesPropSprite("gas"), "persistent fields do not request a second center prop");
            AssertEqual(true, CombatFieldPresentationRules.UsesPropSprite("tree"), "cover retains one foreground prop");
            AssertEqual(false, CombatFieldPresentationRules.UsesAlwaysOnTacticalFrame("stone"), "cover no longer carries an always-on tactical card");
            AssertEqual(false, CombatFieldPresentationRules.UsesAlwaysOnTacticalFrame("demonrift"), "ritual ground art replaces the old always-on tactical frame");
            AssertEqual("3R", CombatFieldPresentationRules.DurationBadgeLabel(3), "persistent field countdown names its round unit");
            AssertEqual("", CombatFieldPresentationRules.DurationBadgeLabel(0), "permanent terrain has no countdown badge");
            AssertEqual(true, CombatFieldPresentationRules.DurationBadgeUrgent(1), "last-round field countdown receives urgency treatment");
            AssertEqual(false, CombatFieldPresentationRules.DurationBadgeUrgent(2), "multi-round field countdown stays informational");
            AssertEqual(fields.Length, profiles.Select(profile => profile.ActivationSfx).Distinct().Count(), "each tactical field has a distinct activation cue");
            foreach (CombatFieldPresentationProfile profile in profiles)
            {
                AssertEqual(true, profile.PlacementBurstCount >= 4 && profile.PlacementBurstCount <= 24, "field placement burst remains bounded " + profile.Kind);

                FormulaDef formula = FormulaCatalog.All.First(value => value.Terrain == profile.Kind);
                CombatImpactProfile impact = CombatImpactRules.ForFormula(formula);
                AssertEqual(profile.ActivationSfx, impact.ImpactSfx, "field spell uses its matching impact cue " + profile.Kind);
            }

            CombatFieldPresentationProfile smoke = CombatFieldPresentationRules.For("smoke");
            CombatImpactProfile smokeBomb = CombatImpactRules.ForAbility(AbilityCatalog.For("smokebomb"));
            AssertEqual(true, CombatFieldPresentationRules.IsPersistentField("smoke"), "Smoke Bomb clouds retain a short-lived field identity");
            AssertEqual(true, CombatFieldPresentationRules.UsesDedicatedGroundSprite("smoke"), "Smoke Bomb clouds use dedicated ground art");
            AssertEqual("smoke", smoke.Kind, "smoke field profile kind");
            AssertEqual("smoke", smoke.ActivationSfx, "smoke field activation cue");
            AssertEqual("smoke", smokeBomb.CastSfx, "Smoke Bomb uses its authored throw cue");
            AssertEqual("smoke", smokeBomb.ImpactSfx, "Smoke Bomb impact uses its authored cloud cue");
            AssertEqual(true, smoke.PlacementBurstCount >= 4 && smoke.PlacementBurstCount <= 24, "smoke placement burst remains bounded");
            AssertEqual(false, CombatTerrainRules.BlocksMovement("smoke"), "Smoke Bomb fields do not block movement");
            AssertEqual(true, CombatTerrainRules.BlocksSight("smoke"), "Smoke Bomb fields block direct sight");

            AssertEqual("castmend", CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "OIC")).CastSfx, "mend casting voice");
            AssertEqual("castnature", CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "GBH")).CastSfx, "tree shaping casting voice");
            AssertEqual("castlight", CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "OBL")).CastSfx, "light casting voice");
            AssertEqual("castember", CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "FIF")).CastSfx, "ember casting voice");
            AssertEqual("castfrost", CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "RCL")).CastSfx, "frost casting voice");
            AssertEqual("castshock", CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "RIG")).CastSfx, "shock casting voice");
            AssertEqual("casthex", CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "RNH")).CastSfx, "hex casting voice");
            AssertEqual("castpact", CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "IBD")).CastSfx, "pact casting voice");
        }

        private static void WorldSitePresentationRulesDefineDistinctAudioIdentity()
        {
            WorldSitePresentationProfile[] expected =
            {
                new WorldSitePresentationProfile(
                    WorldSitePresentationRules.GreenShrineTrainingRing,
                    "green-shrine-road",
                    ObjectType.TrainingGround,
                    "ambforge",
                    "ambgrove",
                    "green-shrine-road",
                    "guard"),
                new WorldSitePresentationProfile(
                    WorldSitePresentationRules.OldQuarryForge,
                    "old-quarry",
                    ObjectType.ForgeSite,
                    "ambforge",
                    "ambstone",
                    "old-quarry",
                    "servicearmor"),
                new WorldSitePresentationProfile(
                    WorldSitePresentationRules.GloamDeepCrypt,
                    "gloam-courts",
                    ObjectType.DeepCrypt,
                    "ambruin",
                    "ambcave",
                    MusicDirectorRules.ForgottenRuins,
                    "door"),
                new WorldSitePresentationProfile(
                    WorldSitePresentationRules.GlassLoreLibrary,
                    "glass-warrens",
                    ObjectType.LoreLibrary,
                    "ambglass",
                    "ambruin",
                    MusicDirectorRules.ArcaneThreshold,
                    "formula"),
                new WorldSitePresentationProfile(
                    WorldSitePresentationRules.DuskMarketHideout,
                    "dusk-market",
                    ObjectType.FactionCamp,
                    "ambdrum",
                    "ambcamp",
                    MusicDirectorRules.FactionCamp,
                    "ambush"),
                new WorldSitePresentationProfile(
                    WorldSitePresentationRules.RedGateSeal,
                    "red-gate",
                    ObjectType.PortalSeal,
                    "ambgate",
                    "ambglass",
                    "red-gate",
                    "riftseal"),
                new WorldSitePresentationProfile(
                    WorldSitePresentationRules.SaltCisternGate,
                    "salt-cisterns",
                    ObjectType.DungeonGate,
                    "ambdrip",
                    "ambcave",
                    MusicDirectorRules.UnderstoneThreshold,
                    "gateopen"),
                new WorldSitePresentationProfile(
                    WorldSitePresentationRules.AshFenAncientGrove,
                    "ash-fen",
                    ObjectType.AncientGrove,
                    "ambgrove",
                    "ambfen",
                    MusicDirectorRules.AncientGrove,
                    "castnature")
            };

            AssertEqual(8, WorldSitePresentationRules.All.Count, "all eight authored sites own presentation profiles");
            AssertEqual(WorldAreaTemplateRules.All.Count, WorldSitePresentationRules.All.Count, "every authored area template owns an audio profile");
            HashSet<string> ambientFingerprints = new HashSet<string>(StringComparer.Ordinal);
            HashSet<string> inspectCues = new HashSet<string>(StringComparer.Ordinal);
            HashSet<string> musicKeys = new HashSet<string>(StringComparer.Ordinal);
            foreach (WorldSitePresentationProfile expectation in expected)
            {
                AssertEqual(true, WorldSitePresentationRules.TryGet(expectation.SiteId, out WorldSitePresentationProfile profile), expectation.SiteId + " profile resolves by stable ID");
                AssertEqual(expectation.ZoneId, profile.ZoneId, expectation.SiteId + " profile keeps its authored zone");
                AssertEqual(expectation.LandmarkType, profile.LandmarkType, expectation.SiteId + " profile keeps its landmark type");
                AssertEqual(expectation.PrimaryAmbientCue, profile.PrimaryAmbientCue, expectation.SiteId + " primary ambience");
                AssertEqual(expectation.SecondaryAmbientCue, profile.SecondaryAmbientCue, expectation.SiteId + " secondary ambience");
                AssertEqual(expectation.MusicKey, profile.MusicKey, expectation.SiteId + " exploration score");
                AssertEqual(expectation.InspectCue, profile.InspectCue, expectation.SiteId + " inspect cue");
                AssertEqual(expectation.PrimaryAmbientCue, profile.AmbientCueFor(0), expectation.SiteId + " starts with its primary ambience");
                AssertEqual(expectation.SecondaryAmbientCue, profile.AmbientCueFor(1), expectation.SiteId + " alternates to its secondary ambience");
                AssertEqual(true, profile.UsesAmbientCue(profile.AmbientCueFor(8)), expectation.SiteId + " emits only its authored ambient pair");
                AssertEqual(true, WorldSitePresentationRules.TryGetForLandmarkObjectId(
                    WorldSitePresentationRules.LandmarkObjectIdPrefix + expectation.SiteId,
                    out WorldSitePresentationProfile objectProfile), expectation.SiteId + " profile resolves from its live landmark ID");
                AssertEqual(expectation.SiteId, objectProfile.SiteId, expectation.SiteId + " live landmark ID cannot cross-route");
                AssertEqual(expectation.MusicKey, WorldSitePresentationRules.ExploreMusicKey(expectation.SiteId, expectation.ZoneId, false), expectation.SiteId + " uses its authored score while calm");
                AssertEqual(MusicDirectorRules.HuntedRoad, WorldSitePresentationRules.ExploreMusicKey(expectation.SiteId, expectation.ZoneId, true), expectation.SiteId + " yields to alerted-patrol music");
                AssertEqual(expectation.InspectCue, WorldSitePresentationRules.InspectCueFor(expectation.SiteId), expectation.SiteId + " resolves a semantic interaction cue");
                ambientFingerprints.Add(profile.PrimaryAmbientCue + ">" + profile.SecondaryAmbientCue);
                inspectCues.Add(profile.InspectCue);
                musicKeys.Add(profile.MusicKey);
            }

            AssertEqual(expected.Length, ambientFingerprints.Count, "all eight sites own distinct two-cue ambience fingerprints");
            AssertEqual(expected.Length, inspectCues.Count, "all eight sites own distinct inspect cues");
            AssertEqual(expected.Length, musicKeys.Count, "all eight sites own distinct calm music routes");
            AssertEqual("ui", WorldSitePresentationRules.InspectCueFor("unknown-site"), "unknown sites retain safe UI feedback");
            AssertEqual("old-quarry", WorldSitePresentationRules.ExploreMusicKey("unknown-site", "OLD-QUARRY", false), "unknown sites retain normalized zone music");
            AssertEqual("midgaard-city", WorldSitePresentationRules.ExploreMusicKey("unknown-site", "midgaard-city", true), "safe Midgaard still ignores pursuit music");

            foreach (WorldAreaTemplate template in WorldAreaTemplateRules.All)
            {
                AssertEqual(true, WorldSitePresentationRules.TryGet(template.SiteId, out _), template.SiteId + " template has presentation data");
                foreach (WorldAreaObjectTemplate decoration in template.Objects)
                {
                    string decorationId = WorldSitePresentationRules.DecorationObjectIdPrefix
                        + template.SiteId
                        + ":"
                        + decoration.Key;
                    AssertEqual(true, WorldSitePresentationRules.IsDecorationObjectId(decorationId), decorationId + " is excluded from audio landmark routing");
                    AssertEqual(false, WorldSitePresentationRules.TryGetForLandmarkObjectId(decorationId, out _), decorationId + " cannot impersonate its parent site");
                }
            }
            AssertEqual(false, WorldSitePresentationRules.IsDecorationObjectId(
                WorldSitePresentationRules.LandmarkObjectIdPrefix + WorldSitePresentationRules.RedGateSeal), "site centers remain eligible audio landmarks");
        }

        private static void GameAudioCuesMatchWorldSurfaces()
        {
            AssertEqual("footstone", GameAudioCueRules.FootstepFor(ExplorationMaterial.CityPaving), "city paving footstep");
            AssertEqual("footearth", GameAudioCueRules.FootstepFor(ExplorationMaterial.PackedDirt), "road earth footstep");
            AssertEqual("footwood", GameAudioCueRules.FootstepFor(ExplorationMaterial.BridgeDeck), "bridge footstep");
            AssertEqual("footwater", GameAudioCueRules.FootstepFor(ExplorationMaterial.ShallowWater), "water footstep");
            AssertEqual("footglass", GameAudioCueRules.FootstepFor(ExplorationMaterial.GlassRubble), "glass rubble footstep");
            AssertEqual("footmud", GameAudioCueRules.FootstepFor(ExplorationMaterial.FenMud), "fen mud footstep");
            AssertEqual("footash", GameAudioCueRules.FootstepFor(ExplorationMaterial.RedAsh), "red ash footstep");
            AssertEqual("footgravel", GameAudioCueRules.FootstepFor(ExplorationMaterial.QuarryStone), "quarry gravel footstep");
            AssertEqual("footgravel", GameAudioCueRules.FootstepFor(ExplorationMaterial.RuinedPaving), "ruined paving footstep");
            AssertEqual(true, GameAudioCueRules.FootstepVolume(ExplorationMaterial.CityPaving) >= 0.78f, "quiet city stone remains audible");
            AssertEqual(true, GameAudioCueRules.FootstepVolume(ExplorationMaterial.PackedDirt) >= 0.80f, "quiet road earth remains audible");
            AssertEqual(true, GameAudioCueRules.FootstepVolume(ExplorationMaterial.ShallowWater) <= 0.36f, "water steps remain restrained");
            AssertEqual(true, GameAudioCueRules.FootstepVolume(ExplorationMaterial.GlassRubble) <= 0.32f, "glass steps remain restrained");
            AssertEqual(true, GameAudioCueRules.FootstepVolume(ExplorationMaterial.FenMud) <= 0.34f, "mud steps remain restrained");
            AssertEqual(true, GameAudioCueRules.FootstepVolume(ExplorationMaterial.RedAsh) <= 0.32f, "ash steps remain restrained");
            foreach (ExplorationMaterial material in Enum.GetValues(typeof(ExplorationMaterial)))
            {
                float volume = GameAudioCueRules.FootstepVolume(material);
                AssertEqual(true, volume >= 0.30f && volume <= 0.88f, material + " footstep gain stays inside the world mix");
                float threatVolume = GameAudioCueRules.RoamingThreatFootstepVolume(material);
                AssertEqual(true, threatVolume >= 0.18f && threatVolume <= 0.32f, material + " roaming-threat step stays beneath the party");
            }
            foreach (int x in new[] { -20, 0, 4, 1000 })
            {
                float pitch = GameAudioCueRules.FootstepPitch(x, x - 3);
                AssertEqual(true, pitch >= 0.96f && pitch <= 1.04f, "footstep pitch stays restrained at " + x);
            }
            AssertEqual(false, GameAudioCueRules.SuppressesExplorationAmbience("footstone"), "party footsteps do not starve world ambience");
            AssertEqual(false, GameAudioCueRules.SuppressesExplorationAmbience("footmud"), "wet footsteps do not starve world ambience");
            AssertEqual(false, GameAudioCueRules.SuppressesExplorationAmbience("koboldstep"), "roaming movement does not starve world ambience");
            AssertEqual(false, GameAudioCueRules.SuppressesExplorationAmbience("ambwind"), "ambient one-shots do not suppress their own schedule");
            AssertEqual(true, GameAudioCueRules.SuppressesExplorationAmbience("ratchitter"), "rat alerts preserve foreground breathing room");
            AssertEqual(true, GameAudioCueRules.SuppressesExplorationAmbience("wayfind"), "discovery stingers preserve foreground breathing room");
            AssertEqual(true, GameAudioCueRules.SuppressesExplorationAmbience("gateopen"), "landmark interactions preserve foreground breathing room");
            AssertEqual(8, GameAudioCueRules.RoamingThreatHearingRadius, "roaming patrol audio has a bounded hearing radius");
            AssertEqual(false, GameAudioCueRules.CanHearRoamingThreat(-1), "invalid patrol distances stay silent");
            AssertEqual(true, GameAudioCueRules.CanHearRoamingThreat(0), "overlapping patrol audio remains audible");
            AssertEqual(true, GameAudioCueRules.CanHearRoamingThreat(GameAudioCueRules.RoamingThreatHearingRadius), "patrol steps remain faintly audible at the hearing edge");
            AssertEqual(false, GameAudioCueRules.CanHearRoamingThreat(GameAudioCueRules.RoamingThreatHearingRadius + 1), "remote patrol steps are inaudible beyond the hearing edge");
            AssertEqual(0f, GameAudioCueRules.RoamingThreatRelativePan(6, 6), "patrol audio directly north or south stays centered");
            AssertEqual(true, GameAudioCueRules.RoamingThreatRelativePan(7, 6) > 0f, "a patrol east of the party sounds right");
            AssertEqual(true, GameAudioCueRules.RoamingThreatRelativePan(5, 6) < 0f, "a patrol west of the party sounds left");
            AssertEqual(
                -GameAudioCueRules.RoamingThreatRelativePan(3, 8),
                GameAudioCueRules.RoamingThreatRelativePan(13, 8),
                "party-relative patrol stereo placement is symmetric");
            AssertEqual(true, GameAudioCueRules.RoamingThreatRelativePan(100, 0) <= 0.68f, "distant horizontal patrol placement stays inside the world mix");
            float closePatrolGain = GameAudioCueRules.RoamingThreatDistanceGain(1);
            float middlePatrolGain = GameAudioCueRules.RoamingThreatDistanceGain(4);
            float edgePatrolGain = GameAudioCueRules.RoamingThreatDistanceGain(GameAudioCueRules.RoamingThreatHearingRadius);
            AssertEqual(1f, closePatrolGain, "adjacent patrol movement keeps its calibrated close gain");
            AssertEqual(true, closePatrolGain > middlePatrolGain && middlePatrolGain > edgePatrolGain, "patrol movement fades monotonically with distance");
            AssertEqual(true, edgePatrolGain > 0f && edgePatrolGain <= 0.18f, "hearing-edge patrol movement is present but faint");
            AssertEqual(0f, GameAudioCueRules.RoamingThreatDistanceGain(GameAudioCueRules.RoamingThreatHearingRadius + 1), "inaudible patrol movement has zero gain");
            AssertEqual(0.28f, GameAudioCueRules.RoamingThreatMovementVolume(0.28f, 1), "close faction steps preserve their authored gain");
            AssertEqual(0f, GameAudioCueRules.RoamingThreatMovementVolume(0.28f, GameAudioCueRules.RoamingThreatHearingRadius + 1), "remote faction steps cannot enter the SFX mix");

            AssertEqual("ambbell", GameAudioCueRules.AmbientFor("midgaard-city", ObjectType.TempleHealer), "temple district uses bell ambience");
            AssertEqual("ambforge", GameAudioCueRules.AmbientFor("midgaard-city", ObjectType.Armorer), "armorer district uses forge ambience");
            AssertEqual("ambgate", GameAudioCueRules.AmbientFor("midgaard-city", ObjectType.EastGate), "city gate uses chain ambience");
            AssertEqual("ambmarket", GameAudioCueRules.AmbientFor("midgaard-city", ObjectType.MarketClerk), "market district uses crowd ambience");
            AssertEqual("ambmarket", GameAudioCueRules.AmbientFor("midgaard-city", ObjectType.DinerCook), "Kate retains warm market-lane ambience");
            AssertEqual("ambmarket", GameAudioCueRules.AmbientFor("midgaard-city", ObjectType.Provisioner), "Lute retains provision-market ambience");
            AssertEqual("ambdrip", GameAudioCueRules.AmbientFor("midgaard-city", ObjectType.DockWorker), "south-quarter worker carries the wet works ambience");
            AssertEqual("ambcity", GameAudioCueRules.AmbientFor("midgaard-city", ObjectType.Scholar), "keep scholar uses restrained city ambience instead of market crowds");
            AssertEqual("ambdrip", GameAudioCueRules.AmbientFor("salt-cisterns", null), "cisterns use water ambience");
            AssertEqual("ambdrum", GameAudioCueRules.AmbientFor("dusk-market", null), "Dusk Market uses distant drums");
            AssertEqual("ambstone", GameAudioCueRules.AmbientFor("old-quarry", null), "Old Quarry uses stone ambience");
            AssertEqual("ambgrove", GameAudioCueRules.AmbientFor("green-shrine-road", null), "Green Shrine Road uses a living grove ambience");
            AssertEqual("ambglass", GameAudioCueRules.AmbientFor("glass-warrens", null), "Glass Warrens uses resonant glass ambience");
            AssertEqual("ambfen", GameAudioCueRules.AmbientFor("ash-fen", null), "Ash Fen uses mire ambience");
            AssertEqual("ambruin", GameAudioCueRules.AmbientFor("gloam-courts", null), "Gloam Courts uses ruin ambience");
            AssertEqual("ambcamp", GameAudioCueRules.AmbientFor("old-quarry", ObjectType.Camp), "outer camps use ember ambience");
            AssertEqual("ambcave", GameAudioCueRules.AmbientFor("dusk-market", ObjectType.Cave), "cave mouths use underground ambience");
            AssertEqual("ambglass", GameAudioCueRules.AmbientFor("green-shrine-road", ObjectType.Obelisk), "arcane landmarks use glass resonance");
            AssertEqual("ambbell", GameAudioCueRules.AmbientFor("midgaard-throne-room", ObjectType.KingHalvard), "throne room keeps a restrained ceremonial ambience");
            AssertEqual("ambforge", GameAudioCueRules.AmbientFor("midgaard-merchant-hall", ObjectType.ArmorerNpc), "merchant hall follows the active craft bay");
            AssertEqual("ambmarket", GameAudioCueRules.AmbientFor("midgaard-merchant-hall", null), "merchant hall defaults to quiet trade ambience");
            AssertEqual(true, GameAudioCueRules.IsAmbientLandmark(ObjectType.SouthGate), "gates can steer exploration ambience");
            AssertEqual(false, GameAudioCueRules.IsAmbientLandmark(ObjectType.Cache), "ordinary loot does not steer exploration ambience");
            for (int sequence = 0; sequence < 12; sequence++)
            {
                float interval = GameAudioCueRules.AmbientInterval(173, sequence);
                float pan = GameAudioCueRules.AmbientPan(173, sequence);
                float ambientPitch = GameAudioCueRules.AmbientPitch(173, sequence);
                AssertEqual(true, interval >= 9f && interval <= 15f, "ambient one-shots remain sparse");
                AssertEqual(true, pan >= -0.24f && pan <= 0.24f, "ambient pan remains subtle");
                AssertEqual(true, ambientPitch >= 0.96f && ambientPitch <= 1.04f, "ambient pitch remains natural");
            }
        }

        private static void AdaptiveMusicDirectorRoutesDistinctContexts()
        {
            AssertEqual(MusicDirectorRules.MidgaardTemple, MusicDirectorRules.ExploreTrackKey("midgaard-city", ObjectType.Temple, true, false), "Temple Square receives its own score");
            AssertEqual(MusicDirectorRules.MidgaardMarket, MusicDirectorRules.ExploreTrackKey("midgaard-city", ObjectType.MarketClerk, true, false), "Market Square receives its own score");
            AssertEqual(MusicDirectorRules.MidgaardTavernLane, MusicDirectorRules.ExploreTrackKey("midgaard-city", ObjectType.Tavern, true, false), "Tavern Lane receives its own score");
            AssertEqual(MusicDirectorRules.MidgaardGateWatch, MusicDirectorRules.ExploreTrackKey("midgaard-city", ObjectType.EastGate, true, false), "Midgaard gates receive a watch score");
            AssertEqual(MusicDirectorRules.MidgaardCisternMouth, MusicDirectorRules.ExploreTrackKey("midgaard-city", ObjectType.Sewer, true, false), "sewer approach receives a threshold score");
            AssertEqual(MusicDirectorRules.MidgaardRoyalApproach, MusicDirectorRules.ExploreTrackKey("midgaard-city", ObjectType.KingHall, true, false), "King's Hall approach receives a processional score");
            AssertEqual(MusicDirectorRules.MidgaardTavernLane, MusicDirectorRules.ExploreTrackKey("midgaard-city", ObjectType.DinerCook, true, false), "Kate's live contact retains Tavern Lane music");
            AssertEqual(MusicDirectorRules.MidgaardMarket, MusicDirectorRules.ExploreTrackKey("midgaard-city", ObjectType.Provisioner, true, false), "Lute's live contact retains market music");
            AssertEqual(MusicDirectorRules.MidgaardGateWatch, MusicDirectorRules.ExploreTrackKey("midgaard-city", ObjectType.DockWorker, true, false), "south-quarter worker follows the gate-watch score");
            AssertEqual(MusicDirectorRules.MidgaardRoyalApproach, MusicDirectorRules.ExploreTrackKey("midgaard-city", ObjectType.Scholar, true, false), "keep scholar follows the royal-approach score");
            AssertEqual("midgaard-throne-room", MusicDirectorRules.ExploreTrackKey("midgaard-throne-room", ObjectType.KingHalvard, true, false), "throne-room score is not overridden by its NPC");
            AssertEqual(MusicDirectorRules.MidgaardRoad, MusicDirectorRules.ExploreTrackKey("midgaard-road", ObjectType.Waystone, true, false), "safe approach retains its own road score");
            AssertEqual(MusicDirectorRules.RoadsideRest, MusicDirectorRules.ExploreTrackKey("old-quarry", ObjectType.Camp, true, false), "roadside camp receives a rest score");
            AssertEqual(MusicDirectorRules.SacredGround, MusicDirectorRules.ExploreTrackKey("green-shrine-road", ObjectType.Shrine, true, false), "outer shrine receives a sacred score");
            AssertEqual(MusicDirectorRules.UnderstoneThreshold, MusicDirectorRules.ExploreTrackKey("dusk-market", ObjectType.Cave, true, false), "cave mouth receives a threshold score");
            AssertEqual(MusicDirectorRules.ForgottenRuins, MusicDirectorRules.ExploreTrackKey("gloam-courts", ObjectType.DeepCrypt, true, false), "crypt receives a ruin score");
            AssertEqual(MusicDirectorRules.ArcaneThreshold, MusicDirectorRules.ExploreTrackKey("glass-warrens", ObjectType.Obelisk, true, false), "obelisk receives an arcane score");
            AssertEqual(MusicDirectorRules.HuntedRoad, MusicDirectorRules.ExploreTrackKey("dusk-market", ObjectType.Cache, false, true), "alerted patrol overrides an outer-road score");
            AssertEqual("midgaard-city", MusicDirectorRules.ExploreTrackKey("midgaard-city", ObjectType.Cache, false, true), "safe Midgaard ignores pursuit music");

            AssertEqual(MusicDirectorRules.CombatKoboldKing, MusicDirectorRules.CombatTrackKey("koboldking", "kobold", false, true, true, false, true), "Kobold King owns the highest-priority battle score");
            AssertEqual(MusicDirectorRules.CombatDemonLord, MusicDirectorRules.CombatTrackKey("finalgate", "demon", false, true, true, true, false), "greater demon finale owns a titan score");
            AssertEqual(MusicDirectorRules.CombatBoss, MusicDirectorRules.CombatTrackKey("boss", "undead", false, true, true, false, false), "ordinary bosses retain the crown-and-ashes score");
            AssertEqual(MusicDirectorRules.CombatLastStand, MusicDirectorRules.CombatTrackKey("patrol", "kobold", false, false, false, false, true), "critical party health invokes last-stand music");
            AssertEqual(MusicDirectorRules.CombatRatfolk, MusicDirectorRules.CombatTrackKey("midgaardsewer", "rat", true, true, false, false, false), "ratfolk encounter receives plague-march music");
            AssertEqual(MusicDirectorRules.CombatSewer, MusicDirectorRules.CombatTrackKey("midgaardsewer", "rat", false, false, false, false, false), "ordinary sewer rats retain sewer combat music");
            AssertEqual(MusicDirectorRules.CombatDrow, MusicDirectorRules.CombatTrackKey("patrol", "drow", false, true, true, false, false), "recognized faction outranks generic elite music");
            AssertEqual(MusicDirectorRules.CombatElite, MusicDirectorRules.CombatTrackKey("patrol", "", false, false, true, false, false), "unaffiliated elite encounter receives a chosen-foe score");
            AssertEqual(MusicDirectorRules.CombatArcaneDuel, MusicDirectorRules.CombatTrackKey("patrol", "", false, true, false, false, false), "unaffiliated caster encounter receives arcane-duel music");
            AssertEqual(MusicDirectorRules.CombatGeneric, MusicDirectorRules.CombatTrackKey("patrol", "", false, false, false, false, false), "ordinary encounter retains generic battle music");
            AssertEqual(true, MusicDirectorRules.IsCriticalPartyHealth(10, 40), "party at one quarter health is critical");
            AssertEqual(false, MusicDirectorRules.IsCriticalPartyHealth(20, 40), "party at half health is not critical");
            AssertEqual(true, MusicDirectorRules.IsMusicLandmark(ObjectType.PortalSeal), "portal seals can steer local music");
            AssertEqual(false, MusicDirectorRules.IsMusicLandmark(ObjectType.Cache), "ordinary caches do not churn the soundtrack");
        }

        private static void CombatImpactPresentationHasReadableEchoAndSpatialMix()
        {
            CombatImpactProfile spark = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "FIF"));
            CombatImpactProfile fireball = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "FBL"));
            CombatImpactProfile meteor = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "MTR"));

            AssertEqual(1, CombatImpactRules.VisualIntensity(spark), "small power uses restrained impact echo");
            AssertEqual(3, CombatImpactRules.VisualIntensity(fireball), "fireball uses an epic impact echo");
            AssertEqual(3, CombatImpactRules.VisualIntensity(meteor), "meteor uses epic impact echo");
            AssertEqual(3, CombatImpactRules.VisualIntensity(fireball, 1), "terrain reaction preserves Fireball's epic visual ceiling");
            AssertEqual(true, CombatImpactRules.EchoDuration(spark) < CombatImpactRules.EchoDuration(fireball), "epic Fireball echo outlasts a small spell");
            AssertEqual(true, CombatImpactRules.EchoDuration(fireball) < CombatImpactRules.EchoDuration(meteor), "Meteor retains the longer epic echo");
            AssertEqual(true, CombatImpactRules.EchoDuration(fireball, 1) > CombatImpactRules.EchoDuration(fireball), "reaction echo receives a longer readable tail");
            AssertEqual(true, CombatImpactRules.CastAuraDuration(fireball) >= fireball.ImpactDelay + 0.20f, "cast aura survives through Fireball impact");
            AssertEqual(true, CombatImpactRules.PresentationBurstCount(fireball, 2) > fireball.BurstCount, "reaction crescendo adds bounded impact particles");

            List<CombatImpactProfile> abilityProfiles = new[] { "warrior", "rogue", "ranger", "demon" }
                .SelectMany(AbilityCatalog.IdsForClass)
                .Select(id => CombatImpactRules.ForAbility(AbilityCatalog.For(id)))
                .ToList();
            AssertEqual(22, abilityProfiles.Count, "all martial and demon-form abilities enter the shared impact pipeline");
            AssertEqual(true, abilityProfiles.All(profile => profile.CastSfx != "ui" && !string.IsNullOrWhiteSpace(profile.ImpactSfx)), "every martial ability has intentional cast and impact audio");
            AssertEqual(22, abilityProfiles.Select(profile => profile.CastSfx + "|" + profile.ImpactSfx + "|" + profile.AftershockSfx).Distinct().Count(), "martial and demon-form abilities keep distinct audiovisual signatures");

            float leftPan = CombatAudioMixRules.StereoPanForColumn(0, 12);
            float centerPan = CombatAudioMixRules.StereoPanForColumn(5, 12);
            float rightPan = CombatAudioMixRules.StereoPanForColumn(11, 12);
            AssertEqual(true, leftPan < -0.60f, "left battlefield impact pans left");
            AssertEqual(true, Math.Abs(centerPan) < 0.10f, "center battlefield impact stays near center");
            AssertEqual(true, rightPan > 0.60f, "right battlefield impact pans right");
            float travelPan = CombatAudioMixRules.StereoPanMidpoint(leftPan, rightPan);
            AssertEqual(true, Math.Abs(travelPan) < 0.001f, "spell release travels through the caster-target stereo midpoint");
            AssertEqual(CombatAudioMixRules.SfxVoiceCount, 8, "layered combat audio owns eight reusable voices");

            foreach (int column in new[] { int.MinValue, -1, 0, 4, 11, int.MaxValue })
            {
                float pitch = CombatAudioMixRules.PitchForCue("fireball", column);
                AssertEqual(true, pitch >= 0.95f && pitch <= 1.05f, "deterministic combat pitch remains bounded at column " + column);
            }

            AssertEqual(0f, CombatAudioMixRules.MusicDuckDepth(spark), "small power does not suppress music");
            AssertEqual(true, CombatAudioMixRules.MusicDuckDepth(fireball) > 0f, "fireball briefly clears space in the mix");
            AssertEqual(true, CombatAudioMixRules.MusicDuckDepth(meteor) > CombatAudioMixRules.MusicDuckDepth(fireball), "meteor ducks music more than fireball");
            AssertEqual(true, CombatAudioMixRules.MusicDuckDepth(spark, 1) > CombatAudioMixRules.MusicDuckDepth(spark), "small reaction receives a readable mix crescendo");
            AssertEqual(true, CombatAudioMixRules.MusicDuckDuration(fireball, 1) > CombatAudioMixRules.MusicDuckDuration(fireball), "reaction mix leaves a slightly longer recovery tail");
            float duckDepth = CombatAudioMixRules.MusicDuckDepth(fireball, 1);
            float attackStartedAt = 10f;
            float fullDepthAt = attackStartedAt + CombatAudioMixRules.MusicDuckAttackDuration(fireball, 1);
            float holdUntil = fullDepthAt + CombatAudioMixRules.MusicDuckHoldDuration(fireball, 1);
            float releaseUntil = holdUntil + CombatAudioMixRules.MusicDuckReleaseDuration(fireball, 1);
            float duckFloor = 1f - duckDepth;
            AssertEqual(1f, CombatAudioMixRules.MusicDuckEnvelopeMultiplier(attackStartedAt - 0.01f, attackStartedAt, fullDepthAt, holdUntil, releaseUntil, duckDepth), "music remains open before the impact attack");
            AssertEqual(true, CombatAudioMixRules.MusicDuckEnvelopeMultiplier((attackStartedAt + fullDepthAt) * 0.5f, attackStartedAt, fullDepthAt, holdUntil, releaseUntil, duckDepth) < 1f, "music duck attacks immediately before impact");
            AssertEqual(true, Math.Abs(CombatAudioMixRules.MusicDuckEnvelopeMultiplier(fullDepthAt, attackStartedAt, fullDepthAt, holdUntil, releaseUntil, duckDepth) - duckFloor) < 0.0001f, "music reaches full duck exactly on impact");
            AssertEqual(true, Math.Abs(CombatAudioMixRules.MusicDuckEnvelopeMultiplier(holdUntil, attackStartedAt, fullDepthAt, holdUntil, releaseUntil, duckDepth) - duckFloor) < 0.0001f, "music holds impact headroom briefly");
            float releaseMidpoint = (holdUntil + releaseUntil) * 0.5f;
            float releaseMultiplier = CombatAudioMixRules.MusicDuckEnvelopeMultiplier(releaseMidpoint, attackStartedAt, fullDepthAt, holdUntil, releaseUntil, duckDepth);
            AssertEqual(true, releaseMultiplier > duckFloor && releaseMultiplier < 1f, "music recovers smoothly after the hit");
            AssertEqual(1f, CombatAudioMixRules.MusicDuckEnvelopeMultiplier(releaseUntil, attackStartedAt, fullDepthAt, holdUntil, releaseUntil, duckDepth), "music fully recovers after the bounded release");
            AssertEqual(true, CombatAudioMixRules.ShouldLayerEpicImpact(meteor), "epic powers receive a low impact layer");
            AssertEqual(true, CombatAudioMixRules.ShouldLayerEpicImpact(fireball), "Fireball earns a restrained low-frequency impact layer");
            AssertEqual(true, CombatAudioMixRules.ShouldLayerSpellRelease(fireball), "formula casts receive a quiet release articulation");
            CombatImpactProfile charge = CombatImpactRules.ForAbility(AbilityCatalog.For("charge"));
            CombatImpactProfile whirlwind = CombatImpactRules.ForAbility(AbilityCatalog.For("whirlwind"));
            CombatImpactProfile volley = CombatImpactRules.ForAbility(AbilityCatalog.For("volley"));
            AssertEqual(false, CombatAudioMixRules.ShouldLayerSpellRelease(charge), "martial powers do not receive a spell release cue");
            AssertEqual(true, CombatAudioMixRules.ShouldLayerCastShimmer(fireball), "signature spells retain their epic release shimmer");
            AssertEqual(true, CombatAudioMixRules.ShouldLayerCastShimmer(meteor), "Meteor retains its epic release shimmer");
            AssertEqual(false, CombatAudioMixRules.ShouldLayerCastShimmer(whirlwind), "epic martial skills do not sound like ritual magic");
            AssertEqual(false, CombatAudioMixRules.ShouldLayerCastShimmer(volley), "epic ranger skills do not sound like ritual magic");
            AssertEqual(true, CombatAudioMixRules.ShouldLayerEpicImpact(whirlwind), "epic martial skills retain physical low-impact weight");
            AssertEqual(true, CombatAudioMixRules.AuxiliaryLayerVolume(0.60f) < 0.60f, "auxiliary layers leave headroom for the primary impact");
            AssertEqual(true, CombatAudioMixRules.AuxiliaryLayerVolume(-1f) >= 0f, "auxiliary layer gain remains non-negative");
            AssertEqual(true, CombatAudioMixRules.AuxiliaryLayerVolume(2f) <= 0.72f, "auxiliary layer gain remains bounded");
            AssertEqual(true, CombatAudioMixRules.ShouldLayerReaction(1), "terrain reaction receives a resonance layer");
            AssertEqual(true, CombatAudioMixRules.MusicDuckDuration(fireball) >= 0.36f, "fireball duck has a readable minimum duration");
            AssertEqual(true, CombatAudioMixRules.MusicDuckDuration(meteor) <= 0.82f, "epic music duck remains brief");
        }

        private static void WeaponFeedbackProfilesStayDistinctAndBounded()
        {
            WeaponFeedbackProfile sword = WeaponFeedbackRules.For("fine longsword", false);
            WeaponFeedbackProfile axe = WeaponFeedbackRules.For("vicious greataxe", false);
            WeaponFeedbackProfile epee = WeaponFeedbackRules.For("fine epee", false);
            WeaponFeedbackProfile bow = WeaponFeedbackRules.For("ashwood longbow", true);

            AssertEqual(WeaponFeedbackKind.Slash, sword.Kind, "sword uses slash contact feedback");
            AssertEqual(WeaponFeedbackKind.Heavy, axe.Kind, "greataxe uses heavy contact feedback");
            AssertEqual(WeaponFeedbackKind.Thrust, epee.Kind, "epee uses thrust contact feedback");
            AssertEqual(WeaponFeedbackKind.Projectile, bow.Kind, "longbow uses projectile contact feedback");
            AssertEqual(4, new[] { sword.ContactCue, axe.ContactCue, epee.ContactCue, bow.ContactCue }.Distinct().Count(), "weapon families use distinct contact sounds");
            AssertEqual(4, new[] { sword.VisualKind, axe.VisualKind, epee.VisualKind, bow.VisualKind }.Distinct().Count(), "weapon families use distinct impact marks");
            AssertEqual(true, WeaponFeedbackRules.ContactVolume(axe, true, true) > WeaponFeedbackRules.ContactVolume(axe, false, false), "critical guarded contact receives a stronger bounded layer");
            AssertEqual(true, WeaponFeedbackRules.ContactVolume(axe, true, true) <= 0.82f, "weapon contact layering remains bounded");
            AssertEqual(true, WeaponFeedbackRules.PresentationBurstCount(sword, true) > WeaponFeedbackRules.PresentationBurstCount(sword, false), "critical weapon hit adds a small visual crescendo");
            AssertEqual("woodcontact", WeaponFeedbackRules.CoverContactCue("tree"), "generated tree cover receives a woody material response");
            AssertEqual("stonecontact", WeaponFeedbackRules.CoverContactCue("stone"), "stone cover receives a masonry response");
            AssertEqual("weapon-splinter", WeaponFeedbackRules.CoverBreakVisualKind("tree"), "tree destruction receives a splinter motif");
            AssertEqual("weapon-rubble", WeaponFeedbackRules.CoverBreakVisualKind("stone"), "stone destruction receives a rubble motif");
            AssertEqual(true, WeaponFeedbackRules.CoverContactVolume(axe, true) > WeaponFeedbackRules.CoverContactVolume(axe, false), "breaking cover has a stronger bounded material layer");
            AssertEqual(true, WeaponFeedbackRules.CoverContactVolume(axe, true) <= 0.78f, "cover material layering remains bounded");
        }

        private static void EnemyPowerPresentationIsDistinctAndBounded()
        {
            string[] powerKeys =
            {
                "graveward", "bonehex", "deathball", "shocksign", "coldsplinter",
                "plaguesigns", "darklight", "venomdust", "cindertrail", "burningpact",
                "dreamveil", "royalrally", "royalcharge", "royalfireball", "royalicelance"
            };
            HashSet<string> titles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string key in powerKeys)
            {
                CombatPowerIdentity identity = CombatPowerPresentationRules.ForEnemyPower(key, "Enemy", "Hero");
                CombatImpactProfile profile = CombatImpactRules.ForEnemyPower(key);
                AssertEqual(true, !string.IsNullOrWhiteSpace(identity.Title) && !string.IsNullOrWhiteSpace(identity.Sigil), "enemy power identity " + key);
                AssertEqual(true, identity.Intensity >= 1 && identity.Intensity <= 3, "enemy power intensity " + key);
                AssertEqual(true, profile.ImpactDelay >= 0f && profile.AftershockDelay >= profile.ImpactDelay, "enemy power staged cue order " + key);
                AssertEqual(true, profile.ResolutionDelay >= 0.06f && profile.ResolutionDelay <= 0.60f, "enemy power resolution bounds " + key);
                AssertEqual(true, titles.Add(identity.Title), "enemy power title is distinct " + key);
                AssertEqual(true,
                    !string.IsNullOrWhiteSpace(CombatPowerPresentationRules.EnemyPowerFormulaArtCode(key))
                    || !string.IsNullOrWhiteSpace(CombatPowerPresentationRules.EnemyPowerAbilityArtId(key)),
                    "enemy power reuses a supported power-art cell " + key);
            }

            CombatImpactProfile deathBall = CombatImpactRules.ForEnemyPower("deathball");
            CombatImpactProfile royalFireball = CombatImpactRules.ForEnemyPower("royalfireball");
            CombatImpactProfile graveWard = CombatImpactRules.ForEnemyPower("graveward");
            AssertEqual("death", deathBall.ImpactSfx, "enemy death ball impact identity");
            AssertEqual("fireball", royalFireball.ImpactSfx, "Kobold King fireball impact identity");
            AssertEqual(0f, graveWard.ShakeMagnitude, "enemy healing ward keeps board steady");
            AssertEqual(true, royalFireball.BurstCount > deathBall.BurstCount && royalFireball.ShakeMagnitude > deathBall.ShakeMagnitude, "boss fireball exceeds normal caster impact");
            AssertEqual(0.06f, CombatPowerResolutionRules.DelayForEnemyPower("royalfireball", true), "reduced motion enemy power beat");
        }

        private static void CombatPowerOutcomeReportsActualChanges()
        {
            GameState state = TestCombatState(out CombatUnit hero, out CombatUnit enemy);
            CombatUnit secondEnemy = new CombatUnit
            {
                Id = "enemy-two",
                Side = UnitSide.Enemy,
                Name = "Second Target",
                X = 4,
                Y = 2,
                Hp = 9,
                MaxHp = 9
            };
            state.Combat.Units.Add(secondEnemy);
            state.Combat.Obstacles = new List<Point> { new Point(3, 3, "tree", 3) };
            hero.Hp = 10;
            hero.Poisoned = 2;

            CombatPowerOutcomeSnapshot before = CombatPowerOutcomeRules.Capture(state.Combat);
            hero.Hp = 16;
            hero.Poisoned = 0;
            enemy.Hp = 0;
            enemy.Stunned = 1;
            secondEnemy.Hp = 4;
            state.Combat.Units.Add(new CombatUnit
            {
                Id = "bound-imp",
                Side = UnitSide.Party,
                Name = "Bound Imp",
                Hp = 8,
                MaxHp = 8,
                Summoned = true
            });
            state.Combat.Obstacles = new List<Point> { new Point(3, 3, "fire", 2) };

            CombatPowerOutcome outcome = CombatPowerOutcomeRules.Compare(before, state.Combat);
            AssertEqual(17, outcome.Damage, "power outcome actual damage");
            AssertEqual(6, outcome.Healing, "power outcome actual healing");
            AssertEqual(3, outcome.AffectedUnits, "power outcome affected units");
            AssertEqual(1, outcome.DefeatedUnits, "power outcome defeated units");
            AssertEqual(1, outcome.StatusesApplied, "power outcome applied statuses");
            AssertEqual(1, outcome.AilmentsCleared, "power outcome cleared ailments");
            AssertEqual(1, outcome.SummonsBound, "power outcome summons");
            AssertEqual(1, outcome.TerrainChanges, "power outcome terrain changes");

            CombatPowerOutcome damageOnly = new CombatPowerOutcome(17, 0, 2, 1, 0, 0, 0, 0);
            AssertEqual("17 damage / 2 targets / 1 defeated", damageOnly.Summary, "damage outcome summary");
            CombatPowerOutcome support = new CombatPowerOutcome(0, 6, 1, 0, 0, 1, 0, 0);
            AssertEqual("6 restored / 1 ailment cleared", support.Summary, "support outcome summary");
            CombatPowerOutcome field = new CombatPowerOutcome(0, 0, 0, 0, 0, 0, 1, 1);
            AssertEqual("1 summon bound / field shaped", field.Summary, "summon and field outcome summary");
        }

        private static void CombatPowerOutcomeSurfacesReactions()
        {
            CombatPowerOutcome outcome = new CombatPowerOutcome(18, 0, 2, 0, 1, 0, 0, 1);
            string summary = CombatPowerOutcomeRules.FormatWithReactions(
                outcome,
                new[] { "Gas ignition", "gas ignition", "Shock conduction", "Third reaction" });
            AssertEqual(true, summary.StartsWith("Gas ignition + Shock conduction", StringComparison.Ordinal), "reaction summary keeps two distinct reactions first");
            AssertEqual(true, summary.Contains("18 damage") && summary.Contains("field shaped"), "reaction summary retains combat outcome");
            AssertEqual("Power resolved", CombatPowerOutcomeRules.FormatWithReactions(default, null), "empty reaction summary remains stable");
        }

        private static void CombatPowerTargetingPreviewsExposeRealFootprints()
        {
            FormulaDef heal = FormulaCatalog.All.First(formula => formula.Code == "OIC");
            FormulaDef treeCover = FormulaCatalog.All.First(formula => formula.Code == "GBH");
            FormulaDef fireball = FormulaCatalog.All.First(formula => formula.Code == "FBL");
            FormulaDef arcSpark = FormulaCatalog.All.First(formula => formula.Code == "RIG");
            FormulaDef thunderclap = FormulaCatalog.All.First(formula => formula.Code == "RSG");
            FormulaDef chainLightning = FormulaCatalog.All.First(formula => formula.Code == "CLT");
            FormulaDef thunderStep = FormulaCatalog.All.First(formula => formula.Code == "VST");
            FormulaDef tempest = FormulaCatalog.All.First(formula => formula.Code == "AST");
            AssertEqual(CombatPowerFootprintKind.Single, CombatPowerTargetingRules.ForFormula(heal).Kind, "heal single-target footprint");
            AssertEqual(CombatPowerFootprintKind.Placement, CombatPowerTargetingRules.ForFormula(treeCover).Kind, "tree cover placement footprint");
            AssertEqual(CombatPowerFootprintKind.CrossArea, CombatPowerTargetingRules.ForFormula(fireball).Kind, "fireball cross-area footprint");
            AssertEqual(CombatPowerFootprintKind.Single, CombatPowerTargetingRules.ForFormula(arcSpark).Kind, "Arc Spark direct-target footprint");
            AssertEqual(CombatPowerFootprintKind.SelfArea, CombatPowerTargetingRules.ForFormula(thunderclap).Kind, "Thunderclap adjacent self-area footprint");
            AssertEqual("PUSH|Adjacent enemies", CombatPowerTargetingRules.ForFormula(thunderclap).BoardLabel + "|" + CombatPowerTargetingRules.ForFormula(thunderclap).ModalLabel, "Thunderclap targeting copy");
            AssertEqual(CombatPowerFootprintKind.Chain, CombatPowerTargetingRules.ForFormula(chainLightning).Kind, "Chain Lightning linked-target footprint");
            AssertEqual("CHAIN|Up to 4 jumps", CombatPowerTargetingRules.ForFormula(chainLightning).BoardLabel + "|" + CombatPowerTargetingRules.ForFormula(chainLightning).ModalLabel, "Chain Lightning targeting copy");
            AssertEqual(CombatPowerFootprintKind.Placement, CombatPowerTargetingRules.ForFormula(thunderStep).Kind, "Thunder Step landing footprint");
            AssertEqual("STEP|Step + landing arc", CombatPowerTargetingRules.ForFormula(thunderStep).BoardLabel + "|" + CombatPowerTargetingRules.ForFormula(thunderStep).ModalLabel, "Thunder Step targeting copy");
            AssertEqual(CombatPowerFootprintKind.RadiusArea, CombatPowerTargetingRules.ForFormula(tempest).Kind, "Arcane Tempest radius footprint");
            AssertEqual("STORM|Radius 2", CombatPowerTargetingRules.ForFormula(tempest).BoardLabel + "|" + CombatPowerTargetingRules.ForFormula(tempest).ModalLabel, "Arcane Tempest targeting copy");

            AssertEqual(CombatPowerFootprintKind.ChargeLanding, CombatPowerTargetingRules.ForAbility(AbilityCatalog.For("charge")).Kind, "charge landing footprint");
            AssertEqual(CombatPowerFootprintKind.SecondaryStrike, CombatPowerTargetingRules.ForAbility(AbilityCatalog.For("cleave")).Kind, "cleave secondary footprint");
            AssertEqual(CombatPowerFootprintKind.CrossArea, CombatPowerTargetingRules.ForAbility(AbilityCatalog.For("volley")).Kind, "volley cross-area footprint");
            AssertEqual(CombatPowerFootprintKind.SelfArea, CombatPowerTargetingRules.ForAbility(AbilityCatalog.For("whirlwind")).Kind, "whirlwind adjacent footprint");
            AssertEqual(CombatPowerFootprintKind.Single, CombatPowerTargetingRules.ForAbility(AbilityCatalog.For("aimedshot")).Kind, "aimed shot single footprint");
            AssertEqual("SMOKE|Adjacent field", CombatPowerTargetingRules.ForAbility(AbilityCatalog.For("smokebomb")).BoardLabel + "|" + CombatPowerTargetingRules.ForAbility(AbilityCatalog.For("smokebomb")).ModalLabel, "Smoke Bomb targeting copy");

            foreach (string code in ContentSetCatalog.SewerSliceFormulaCodes)
            {
                CombatPowerTargetingProfile profile = CombatPowerTargetingRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == code));
                AssertEqual(true, !string.IsNullOrWhiteSpace(profile.BoardLabel) && !string.IsNullOrWhiteSpace(profile.ModalLabel), "slice formula targeting profile " + code);
            }

            foreach (string id in ContentSetCatalog.SewerSliceAbilityIds)
            {
                CombatPowerTargetingProfile profile = CombatPowerTargetingRules.ForAbility(AbilityCatalog.For(id));
                AssertEqual(true, !string.IsNullOrWhiteSpace(profile.BoardLabel) && !string.IsNullOrWhiteSpace(profile.ModalLabel), "slice ability targeting profile " + id);
            }
        }

        private static void BoardPointerRoutingBlocksHudAndModalClicks()
        {
            AssertEqual(true, ScreenInputRules.ShouldRouteBoardPointer(UiOverlay.None, false, true, false, false), "plain board click routes");
            AssertEqual(false, ScreenInputRules.ShouldRouteBoardPointer(UiOverlay.Pause, false, true, false, false), "pause menu blocks board click");
            AssertEqual(false, ScreenInputRules.ShouldRouteBoardPointer(UiOverlay.Help, false, true, false, false), "help overlay blocks board click");
            AssertEqual(false, ScreenInputRules.ShouldRouteBoardPointer(UiOverlay.Dialogue, false, true, false, false), "dialogue blocks board click");
            AssertEqual(false, ScreenInputRules.ShouldRouteBoardPointer(UiOverlay.Armory, false, true, false, false), "armory blocks board click");
            AssertEqual(false, ScreenInputRules.ShouldRouteBoardPointer(UiOverlay.Loot, false, true, false, false), "loot popup blocks board click");
            AssertEqual(false, ScreenInputRules.ShouldRouteBoardPointer(UiOverlay.AbilityPicker, false, true, false, false), "ability picker blocks board click");
            AssertEqual(false, ScreenInputRules.ShouldRouteBoardPointer(UiOverlay.None, true, true, false, false), "uGUI blocks board click");
            AssertEqual(false, ScreenInputRules.ShouldRouteBoardPointer(UiOverlay.None, false, false, false, false), "outside board blocked");
            AssertEqual(false, ScreenInputRules.ShouldRouteBoardPointer(UiOverlay.None, false, true, true, false), "legacy side panel blocks exploration board click");
            AssertEqual(false, ScreenInputRules.ShouldRouteBoardPointer(UiOverlay.None, false, true, false, true), "legacy command bar blocks exploration board click");
            AssertEqual(true, ScreenInputRules.ShouldSuppressBoardPointer(100, 101), "overlay close suppresses the current board click");
            AssertEqual(true, ScreenInputRules.ShouldSuppressBoardPointer(101, 101), "overlay close suppression survives one late frame");
            AssertEqual(false, ScreenInputRules.ShouldSuppressBoardPointer(102, 101), "board pointer suppression expires promptly");
            AssertEqual(true, ScreenInputRules.CanAcceptGameplayInput(ScreenInputRules.TopOverlay(false, false, false, false, false)), "no overlay accepts gameplay input");
            AssertEqual(false, ScreenInputRules.CanAcceptGameplayInput(ScreenInputRules.TopOverlay(true, false, false, false, false)), "pause menu blocks keyboard gameplay");
            AssertEqual(false, ScreenInputRules.CanAcceptGameplayInput(ScreenInputRules.TopOverlay(false, true, false, false, false, false)), "help overlay blocks keyboard gameplay");
            AssertEqual(false, ScreenInputRules.CanAcceptGameplayInput(ScreenInputRules.TopOverlay(false, false, true, false, false)), "dialogue blocks keyboard gameplay");
            AssertEqual(false, ScreenInputRules.CanAcceptGameplayInput(ScreenInputRules.TopOverlay(false, true, false, false, false)), "armory blocks keyboard gameplay");
            AssertEqual(false, ScreenInputRules.CanAcceptGameplayInput(ScreenInputRules.TopOverlay(false, false, false, true, false)), "loot blocks keyboard gameplay");
            AssertEqual(false, ScreenInputRules.CanAcceptGameplayInput(ScreenInputRules.TopOverlay(false, false, false, false, true)), "ability picker blocks keyboard gameplay");
            AssertEqual(UiOverlay.Dialogue, ScreenInputRules.TopOverlay(false, true, true, true, true), "dialogue owns focus before other gameplay overlays");
            AssertEqual(UiOverlay.Pause, ScreenInputRules.TopOverlay(true, true, true, true, true), "pause owns focus before other overlays");
            AssertEqual(UiOverlay.Help, ScreenInputRules.TopOverlay(true, true, true, true, true, true), "help owns focus when explicitly open");
        }

        private static void HelpOverlayLayoutFitsSupportedResolutions()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(1280, 720),
                new Vector2Int(1600, 900),
                new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152)
            };

            foreach (Vector2Int size in sizes)
            {
                HelpOverlayGeometry geometry = HelpOverlayLayout.Calculate(size.x, size.y);
                AssertEqual(true, geometry.Fits(size.x, size.y), $"help overlay layout fits {size.x}x{size.y}");
            }
        }

        private static void HelpOverlayContentIsModeSpecific()
        {
            HelpOverlayView tavern = HelpOverlayContent.Build(GameMode.Tavern, true, 6, "Midgaard");
            HelpOverlayView explore = HelpOverlayContent.Build(GameMode.Explore, false, 6, "Midgaard");
            HelpOverlayView combat = HelpOverlayContent.Build(GameMode.Combat, false, 6, "Midgaard");
            HelpOverlayView muster = HelpOverlayContent.Build(GameMode.Muster, false, 6, "Midgaard");
            HelpOverlayView victory = HelpOverlayContent.Build(GameMode.Victory, false, 6, "Midgaard");
            HelpOverlayView defeat = HelpOverlayContent.Build(GameMode.Defeat, false, 6, "Midgaard");

            AssertEqual(true, tavern.Title.Contains("Tavern"), "tavern help title");
            AssertEqual(true, tavern.Lines.Any(line => line.Contains("Beta Testing")), "developer tavern help mentions beta testing");
            AssertEqual(true, explore.Lines.Any(line => line.Contains("Space / E")), "exploration help mentions contextual use");
            AssertEqual(true, explore.Lines.Any(line => line.IndexOf("east and west gates", StringComparison.OrdinalIgnoreCase) >= 0), "exploration help mentions pass-through gates");
            AssertEqual(true, combat.Lines.Any(line => line.Contains("Tree Cover")), "combat help mentions tree cover");
            AssertEqual(true, combat.Lines.Any(line => line.Contains("undo this turn's movement")), "combat help explains pre-action movement undo");
            AssertEqual(true, combat.Lines.Any(line => line.Contains("cancels an armed target")), "combat help explains non-destructive target cancellation");
            AssertEqual(true, combat.Lines.Any(line => line.Contains("retreat for one supply")), "combat help explains the retreat safety valve");
            AssertEqual(true, muster.Lines.Any(line => line.Contains("50-point")), "muster help mentions stat budget");
            AssertEqual(true, victory.Title.Contains("Victory"), "victory help title");
            AssertEqual(true, defeat.Title.Contains("Defeat"), "defeat help title");
        }

        private static void EndStateScreenLayoutFitsSupportedResolutions()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(1280, 720),
                new Vector2Int(1600, 900),
                new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152)
            };

            foreach (Vector2Int size in sizes)
            {
                EndStateGeometry geometry = EndStateScreenLayout.Calculate(size.x, size.y);
                AssertEqual(true, geometry.Fits(size.x, size.y), $"end-state layout fits {size.x}x{size.y}");
            }
        }

        private static void EndStateContentIsStateSpecific()
        {
            string[] rows = { "Maer / L3 Warrior / HP 31/31 / arms 9" };
            EndStateView victory = EndStateContent.BuildVictory("Midgaard", 1, 1, 3, 160, 6, rows, true);
            EndStateView defeat = EndStateContent.BuildDefeat("Midgaard", rows);

            AssertEqual(true, victory.Victory, "victory view marks victory");
            AssertEqual(true, victory.ShowTavernButton, "victory view exposes tavern return");
            AssertEqual(true, victory.ShowBetaLabButton, "victory view exposes beta lab when requested");
            AssertEqual(true, victory.Title.Contains("Old Road"), "victory title names route ending");
            AssertEqual(false, defeat.Victory, "defeat view marks defeat");
            AssertEqual(true, defeat.ShowTavernButton, "defeat exposes tavern checkpoint recovery");
            AssertEqual(true, defeat.Title.Contains("Fallen"), "defeat title names fall state");
            AssertEqual(true, defeat.RouteRows.Any(row => row.IndexOf("checkpoint", StringComparison.OrdinalIgnoreCase) >= 0), "defeat route gives checkpoint guidance");
        }

        private static void DialogueScreenLayoutFitsSupportedResolutions()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(1280, 720),
                new Vector2Int(1600, 900),
                new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152)
            };

            foreach (Vector2Int size in sizes)
            {
                DialogueScreenGeometry geometry = DialogueScreenLayout.Calculate(size.x, size.y);
                AssertEqual(true, geometry.Fits(size.x, size.y), $"dialogue layout fits {size.x}x{size.y}");
                for (int choices = 0; choices <= 4; choices++)
                {
                    DialogueScreenGeometry choiceGeometry = DialogueScreenLayout.Calculate(size.x, size.y, choices);
                    AssertEqual(true, choiceGeometry.Fits(size.x, size.y), $"dialogue layout with {choices} choices fits {size.x}x{size.y}");
                }
                AssertEqual(true, BannerToastLayout.Calculate(size.x, size.y, false).Fits(size.x, size.y), $"compact world toast fits {size.x}x{size.y}");
                AssertEqual(true, BannerToastLayout.Calculate(size.x, size.y, true).Fits(size.x, size.y), $"power banner fits {size.x}x{size.y}");
            }

            DialogueScreenGeometry desktopTopics = DialogueScreenLayout.Calculate(1920f, 1080f, 3);
            AssertEqual(true, desktopTopics.Panel.height >= 490f && desktopTopics.Panel.height <= 510f, "desktop topic conversation reserves comfortable single-column choices");
            AssertEqual(true, desktopTopics.Panel.width >= 1100f && desktopTopics.Panel.width <= 1160f, "desktop conversation uses a readable measure without reaching the screen edges");
            AssertEqual(true, desktopTopics.Body.height >= 128f, "desktop conversation reserves a readable body measure");
            AssertEqual(true, desktopTopics.CloseButton.width >= 148f && desktopTopics.CloseButton.height >= 38f, "dialogue continuation target remains comfortably clickable");

            Color readableStone = DialoguePresentationRules.ReadableAccent(new Color(0.27f, 0.31f, 0.30f, 1f));
            Color.RGBToHSV(readableStone, out _, out _, out float readableValue);
            AssertEqual(true, readableValue >= 0.58f, "dark stone dialogue accents are lifted into a readable title and border range");

            Color dialoguePanel = new Color32(0x1a, 0x20, 0x26, 0xff);
            Color readableBlood = DialoguePresentationRules.ReadableTextAccent(new Color(0.34f, 0.08f, 0.08f, 1f));
            AssertEqual(true, DialoguePresentationRules.ContrastRatio(readableBlood, dialoguePanel) >= 4.5f, "dark character accents produce accessible title contrast");
        }

        private static void DialoguePagingAndPortraitCatalogAreReadable()
        {
            string[] shortPages = DialoguePagingRules.Paginate("One short road warning.");
            AssertEqual(1, shortPages.Length, "short NPC line stays on one dialogue page");

            string longSpeech = "The west road is open, but the shrine stones only mark the first safe mile. "
                + "Carry food, keep one elixir, and turn back when the market drums answer from beyond the wall. "
                + "If the quarry lights go dark, do not prove anything to the dark. Return to Midgaard and report it.";
            string[] pages = DialoguePagingRules.Paginate(longSpeech, 120, 180);
            AssertEqual(true, pages.Length >= 2, "long NPC speech becomes readable conversation beats");
            AssertEqual(true, pages.All(page => !string.IsNullOrWhiteSpace(page) && page.Length <= 180), "dialogue beats stay within the hard reading limit");
            AssertEqual(longSpeech, string.Join(" ", pages), "dialogue paging preserves authored text");

            AssertEqual(62, (int)ObjectType.MerchantCounter, "serialized object enum preserves the pre-contact append point");
            AssertEqual(63, (int)ObjectType.DinerCook, "serialized Kate contact id is append-only");
            AssertEqual(64, (int)ObjectType.Provisioner, "serialized Lute contact id is append-only");
            AssertEqual(65, (int)ObjectType.DockWorker, "serialized dock-worker contact id is append-only");
            AssertEqual(66, (int)ObjectType.Scholar, "serialized scholar contact id is append-only");

            AssertEqual(0, NpcPortraitCatalog.PortraitIndex(ObjectType.KingHall, "King Halvard"), "king uses royal portrait");
            AssertEqual(1, NpcPortraitCatalog.PortraitIndex(ObjectType.RoyalHerald, "Herald Vann"), "herald is visually distinct from the king");
            AssertEqual(3, NpcPortraitCatalog.PortraitIndex(ObjectType.TownGuard, "Watchman Rusk"), "west watch uses Rusk portrait");
            AssertEqual(4, NpcPortraitCatalog.PortraitIndex(ObjectType.TownGuard, "Watchwoman Ilyra"), "east watch uses Ilyra portrait");
            AssertEqual(13, NpcPortraitCatalog.PortraitIndex(ObjectType.Armorer, "Borin"), "armorer uses smith portrait");
            AssertEqual(15, NpcPortraitCatalog.PortraitIndex(ObjectType.Enchanter, "Maud"), "enchanter uses mage portrait");
            AssertEqual(9, NpcPortraitCatalog.PortraitIndex(ObjectType.WoundedTraveler, "Edna"), "common Edna spelling still resolves to Edda's portrait");
            AssertEqual(11, NpcPortraitCatalog.PortraitIndex(ObjectType.GateCaptain, "Brann"), "gate captain uses officer portrait");
            AssertEqual(17, NpcPortraitCatalog.PortraitIndex(ObjectType.Provisions, "Lute"), "provision seller has a stable identity");
            AssertEqual(-1, NpcPortraitCatalog.PortraitIndex(ObjectType.QuestBoard, "Midgaard Notices"), "non-speaker notice keeps emblem fallback");

            Dictionary<string, int> namedSpeakers = new Dictionary<string, int>
            {
                { "King Halvard", 0 },
                { "Herald Vann", 1 },
                { "Nessa", 2 },
                { "Watchman Rusk", 3 },
                { "Watchwoman Ilyra", 4 },
                { "Tovan", 5 },
                { "Mira", 6 },
                { "Sera", 7 },
                { "Orren", 8 },
                { "Edda", 9 },
                { "Pell", 10 },
                { "Brann", 11 },
                { "Kate", 12 },
                { "Borin", 13 },
                { "Tessa", 14 },
                { "Maud", 15 },
                { "Yara", 16 },
                { "Lute", 17 },
                { "Dock Worker", 18 },
                { "Midgaard Scholar", 19 }
            };
            foreach (KeyValuePair<string, int> speaker in namedSpeakers)
            {
                AssertEqual(speaker.Value, NpcPortraitCatalog.PortraitIndex(ObjectType.Town, speaker.Key), speaker.Key + " portrait identity");
            }
            AssertEqual(namedSpeakers.Count, namedSpeakers
                .Select(speaker => NpcPortraitCatalog.PortraitIndex(ObjectType.Town, speaker.Key))
                .Distinct()
                .Count(), "named Midgaard contacts use unique portrait identities");

            AssertEqual(0, NpcPortraitCatalog.WorldSpriteIndex(ObjectType.TownGuard, false), "Rusk uses the west-watch sprite");
            AssertEqual(1, NpcPortraitCatalog.WorldSpriteIndex(ObjectType.TownGuard, true), "Ilyra uses the east-watch sprite");
            Dictionary<ObjectType, int> worldSprites = new Dictionary<ObjectType, int>
            {
                { ObjectType.KingHalvard, 2 },
                { ObjectType.MarketClerk, 3 },
                { ObjectType.TempleHealer, 4 },
                { ObjectType.TavernKeeper, 5 },
                { ObjectType.ArmorerNpc, 6 },
                { ObjectType.WeaponMerchantNpc, 7 },
                { ObjectType.GateCaptain, 8 },
                { ObjectType.EnchanterNpc, 9 },
                { ObjectType.CityCourier, 12 },
                { ObjectType.WoundedTraveler, 13 },
                { ObjectType.StableHand, 15 },
                { ObjectType.RoyalHerald, 16 },
                { ObjectType.NoviceHealer, 17 },
                { ObjectType.OldRoadScout, 18 },
                { ObjectType.DinerCook, 10 },
                { ObjectType.Provisioner, 11 },
                { ObjectType.DockWorker, 14 },
                { ObjectType.Scholar, 19 }
            };
            foreach (KeyValuePair<ObjectType, int> sprite in worldSprites)
            {
                AssertEqual(sprite.Value, NpcPortraitCatalog.WorldSpriteIndex(sprite.Key, false), sprite.Key + " world-sprite identity");
            }
            Dictionary<NpcId, int> namedWorldSprites = new Dictionary<NpcId, int>
            {
                { NpcId.WatchmanRusk, 0 },
                { NpcId.WatchwomanIlyra, 1 },
                { NpcId.KingHalvard, 2 },
                { NpcId.Nessa, 3 },
                { NpcId.Mira, 4 },
                { NpcId.Orren, 5 },
                { NpcId.Borin, 6 },
                { NpcId.Tessa, 7 },
                { NpcId.CaptainBrann, 8 },
                { NpcId.Maud, 9 },
                { NpcId.Kate, 10 },
                { NpcId.Lute, 11 },
                { NpcId.Tovan, 12 },
                { NpcId.Edda, 13 },
                { NpcId.DockWorker, 14 },
                { NpcId.Pell, 15 },
                { NpcId.HeraldVann, 16 },
                { NpcId.Sera, 17 },
                { NpcId.Yara, 18 },
                { NpcId.Scholar, 19 }
            };
            foreach (KeyValuePair<NpcId, int> sprite in namedWorldSprites)
            {
                AssertEqual(
                    sprite.Value,
                    NpcPortraitCatalog.WorldSpriteIndex(sprite.Key),
                    sprite.Key + " named world-sprite identity");
            }
            AssertEqual(
                NpcPortraitCatalog.Columns * NpcPortraitCatalog.Rows,
                namedWorldSprites.Values.Distinct().Count(),
                "all named NPC world-sprite identities are unique");
            foreach (ObjectType type in new[]
            {
                ObjectType.DinerCook,
                ObjectType.Provisioner,
                ObjectType.DockWorker,
                ObjectType.Scholar
            })
            {
                AssertEqual(true, ExplorationInteractionRules.IsUseObject(type), type + " is a reachable Midgaard contact");
                AssertEqual(true, ExplorationTraversalRules.CanUseFromAdjacent(type), type + " supports adjacent Talk interaction");
            }
        }

        private static void LootPopupLayoutFitsSupportedResolutions()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(1280, 720),
                new Vector2Int(1600, 900),
                new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152)
            };

            foreach (Vector2Int size in sizes)
            {
                LootPopupGeometry geometry = LootPopupLayout.Calculate(size.x, size.y);
                AssertEqual(true, geometry.Fits(size.x, size.y), $"loot popup layout fits {size.x}x{size.y}");
            }
        }

        private static void ArmoryOverlayLayoutFitsSupportedResolutions()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(1280, 720),
                new Vector2Int(1600, 900),
                new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152)
            };

            foreach (Vector2Int size in sizes)
            {
                ArmoryOverlayGeometry geometry = ArmoryOverlayLayout.Calculate(size.x, size.y);
                AssertEqual(true, geometry.Fits(size.x, size.y), $"armory overlay layout fits {size.x}x{size.y}");
                Rect[] tabs = ArmoryOverlayLayout.TabRects(geometry.Tabs.width);
                foreach (Rect tab in tabs)
                {
                    AssertEqual(true, tab.xMin >= 0f && tab.yMin >= 0f && tab.xMax <= geometry.Tabs.width && tab.yMax <= geometry.Tabs.height, $"armory tab fits {size.x}x{size.y}");
                }
                AssertEqual(true, geometry.ListContent.width >= 500f, $"inventory list remains readable at {size.x}x{size.y}");
                AssertEqual(true, geometry.Detail.width >= 320f, $"equipment comparison pane remains readable at {size.x}x{size.y}");
            }
        }

        private static void InventoryEquipmentRulesMakeOwnershipAndComparisonsExplicit()
        {
            InventoryItem focus = new InventoryItem
            {
                Slot = "focus",
                Form = "stormglass orb",
                Rarity = "rare",
                Bonus = 3
            };
            InventoryItem armor = new InventoryItem
            {
                Slot = "armor",
                Form = "chain hauberk",
                Rarity = "uncommon",
                Bonus = 2
            };
            InventoryItem questMaterial = new InventoryItem
            {
                Slot = "quest",
                Form = "pelt bundle",
                Rarity = "common",
                Bonus = 0
            };

            AssertEqual(true, InventoryEquipmentRules.IsWeaponSlot(focus.Slot, focus.Form), "focus inventory slot is presented and equipped as a weapon");
            AssertEqual(false, InventoryEquipmentRules.IsWeaponSlot(armor.Slot, armor.Form), "armor inventory slot remains armor");
            AssertEqual(true, InventoryEquipmentRules.IsArmorSlot(armor.Slot, armor.Form), "armor has an explicit equipment classification");
            AssertEqual(true, InventoryEquipmentRules.IsEquippable(focus), "weapon inventory items are equippable");
            AssertEqual(true, InventoryEquipmentRules.IsEquippable(armor), "armor inventory items are equippable");
            AssertEqual(false, InventoryEquipmentRules.IsEquippable(questMaterial), "quest materials cannot enter equipment actions");
            AssertEqual("Quest item", InventoryEquipmentRules.SlotLabel(questMaterial.Slot, questMaterial.Form), "quest materials have an honest inventory label");
            AssertEqual("Rare", InventoryEquipmentRules.RarityLabel(focus.Rarity), "rarity label is player-facing");
            AssertEqual(InventoryUpgradeGrade.Upgrade, InventoryEquipmentRules.Grade(4), "meaningful positive score is an upgrade");
            AssertEqual(InventoryUpgradeGrade.Sidegrade, InventoryEquipmentRules.Grade(2), "small score change is a sidegrade");
            AssertEqual(InventoryUpgradeGrade.Downgrade, InventoryEquipmentRules.Grade(-4), "meaningful negative score is a tradeoff");
            AssertEqual(true, InventoryEquipmentRules.MatchesFilter(focus, 1, false), "weapon filter includes a focus");
            AssertEqual(false, InventoryEquipmentRules.MatchesFilter(focus, 2, false), "armor filter excludes a focus");
            AssertEqual(true, InventoryEquipmentRules.MatchesFilter(focus, 3, true), "upgrade filter uses the calculated comparison");
            AssertEqual(false, InventoryEquipmentRules.MatchesFilter(questMaterial, 2, false), "armor filter excludes quest materials");
            AssertEqual(false, InventoryEquipmentRules.MatchesFilter(questMaterial, 3, true), "upgrade filter excludes non-equipment");
            AssertEqual("+3", InventoryEquipmentRules.SignedDelta(3), "positive comparison delta is explicit");
            AssertEqual(true,
                InventoryEquipmentRules.SortScore(focus, false, 8) > InventoryEquipmentRules.SortScore(armor, true, -2),
                "clear unequipped upgrade sorts ahead of equipped tradeoff");
        }

        private static void WeaponEnchantmentRulesPreserveAffinityAndDuration()
        {
            WeaponEnchantmentDefinition[] definitions = WeaponEnchantmentRules.All.ToArray();
            AssertEqual(4, definitions.Length, "weapon enchanter exposes all four affinities");
            AssertEqual(
                "fire:Fire:fire:fiery:flamebound|ice:Ice:cold:icy:frostbound|storm:Storm:shock:stormcharged:stormbound|radiance:Radiance:light:radiant:sunbound",
                string.Join("|", definitions.Select(definition =>
                    $"{definition.Id}:{definition.MenuLabel}:{definition.DamageType}:{definition.TemporaryPrefix}:{definition.PermanentPrefix}")),
                "weapon enchantment definitions keep their authored text and damage identities");

            InventoryItem temporaryFire = new InventoryItem
            {
                DisplayName = "iron broadsword",
                Trait = "steady",
                DamageType = "physical",
                Slot = "weapon",
                Form = "broadsword"
            };
            AssertEqual(true, WeaponEnchantmentRules.ApplyTemporary(temporaryFire, "fire"), "temporary fire enchantment changes the weapon");
            AssertEqual("fiery iron broadsword", temporaryFire.DisplayName, "temporary fire changes item name text");
            AssertEqual("fiery steady", temporaryFire.Trait, "temporary fire changes item trait text");
            AssertEqual("fire", temporaryFire.DamageType, "temporary fire changes weapon damage type");
            AssertEqual("fire", temporaryFire.TemporaryEnchantmentId, "temporary fire records its affinity");
            AssertEqual(3, temporaryFire.TemporaryEnchantmentVictoriesRemaining, "temporary fire starts with three victories");
            AssertEqual("Fire (temporary, 3 victories remaining)", WeaponEnchantmentRules.StatusText(temporaryFire), "temporary fire publishes its duration");
            AssertEqual(true, WeaponEnchantmentRules.AdvanceAfterVictory(temporaryFire), "first victory advances temporary fire");
            AssertEqual(2, temporaryFire.TemporaryEnchantmentVictoriesRemaining, "temporary fire has two victories remaining");
            AssertEqual(true, WeaponEnchantmentRules.AdvanceAfterVictory(temporaryFire), "second victory advances temporary fire");
            AssertEqual(1, temporaryFire.TemporaryEnchantmentVictoriesRemaining, "temporary fire has one victory remaining");
            AssertEqual(true, WeaponEnchantmentRules.AdvanceAfterVictory(temporaryFire), "third victory expires temporary fire");
            AssertEqual("iron broadsword", temporaryFire.DisplayName, "expired temporary fire restores base item name");
            AssertEqual("steady", temporaryFire.Trait, "expired temporary fire restores base trait");
            AssertEqual("physical", temporaryFire.DamageType, "expired temporary fire restores base damage type");
            AssertEqual(0, temporaryFire.TemporaryEnchantmentVictoriesRemaining, "expired temporary fire clears its countdown");

            InventoryItem layered = new InventoryItem
            {
                DisplayName = "iron broadsword",
                Trait = "steady",
                DamageType = "physical",
                Slot = "weapon",
                Form = "broadsword"
            };
            AssertEqual(true, WeaponEnchantmentRules.ApplyPermanent(layered, "ice"), "permanent ice enchantment changes the weapon");
            AssertEqual("frostbound iron broadsword", layered.DisplayName, "permanent ice changes item name text");
            AssertEqual("frostbound steady", layered.Trait, "permanent ice changes item trait text");
            AssertEqual("cold", layered.DamageType, "permanent ice changes weapon damage type");
            AssertEqual("Ice (permanent)", WeaponEnchantmentRules.StatusText(layered), "permanent ice publishes permanent status");
            AssertEqual(false, WeaponEnchantmentRules.AdvanceAfterVictory(layered), "victories do not consume permanent ice");
            AssertEqual("frostbound iron broadsword", layered.DisplayName, "permanent ice persists after victory");

            AssertEqual(true, WeaponEnchantmentRules.ApplyTemporary(layered, "storm"), "temporary storm layers over permanent ice");
            AssertEqual("stormcharged frostbound iron broadsword", layered.DisplayName, "temporary storm text layers over permanent ice text");
            AssertEqual("shock", layered.DamageType, "temporary storm affinity overrides permanent ice while active");
            AssertEqual("Storm (temporary, 3 victories remaining) over Ice (permanent)", WeaponEnchantmentRules.StatusText(layered), "layered enchantments publish both durations");
            AssertEqual(true, WeaponEnchantmentRules.AdvanceAfterVictory(layered), "layered storm advances after first victory");
            AssertEqual(true, WeaponEnchantmentRules.AdvanceAfterVictory(layered), "layered storm advances after second victory");
            AssertEqual(true, WeaponEnchantmentRules.AdvanceAfterVictory(layered), "layered storm expires after third victory");
            AssertEqual("frostbound iron broadsword", layered.DisplayName, "expired storm restores permanent ice text");
            AssertEqual("frostbound steady", layered.Trait, "expired storm restores permanent ice trait");
            AssertEqual("cold", layered.DamageType, "expired storm restores permanent ice damage type");
            AssertEqual("ice", layered.PermanentEnchantmentId, "temporary expiry preserves permanent ice metadata");
            AssertEqual("Ice (permanent)", WeaponEnchantmentRules.StatusText(layered), "temporary expiry restores permanent ice status");

            InventoryItem replacement = new InventoryItem
            {
                DisplayName = "ash staff",
                Trait = "balanced",
                DamageType = "physical",
                Slot = "focus",
                Form = "staff"
            };
            WeaponEnchantmentRules.ApplyPermanent(replacement, "fire");
            WeaponEnchantmentRules.ApplyPermanent(replacement, "ice");
            WeaponEnchantmentRules.ApplyPermanent(replacement, "ice");
            WeaponEnchantmentRules.ApplyTemporary(replacement, "fire");
            WeaponEnchantmentRules.ApplyTemporary(replacement, "radiance");
            WeaponEnchantmentRules.ApplyTemporary(replacement, "radiance");
            AssertEqual("radiant frostbound ash staff", replacement.DisplayName, "affinity replacement rebuilds text from one clean base");
            AssertEqual("radiant frostbound balanced", replacement.Trait, "affinity replacement rebuilds trait text from one clean base");
            AssertEqual(1, CountOccurrences(replacement.DisplayName, "radiant "), "temporary replacement never duplicates its prefix");
            AssertEqual(1, CountOccurrences(replacement.DisplayName, "frostbound "), "permanent replacement never duplicates its prefix");
            AssertEqual(false, replacement.DisplayName.Contains("fiery ") || replacement.DisplayName.Contains("flamebound "), "replaced affinities leave no stale prefixes");

            InventoryItem armor = new InventoryItem
            {
                DisplayName = "iron hauberk",
                Trait = "guarded",
                DamageType = "physical",
                Slot = "armor",
                Form = "chain"
            };
            AssertThrows<ArgumentException>(
                () => WeaponEnchantmentRules.ApplyTemporary(armor, "fire"),
                "non-weapon items reject weapon enchantments");

            WeaponEnchantmentRules.AdvanceAfterVictory(replacement);
            string json = JsonUtility.ToJson(replacement);
            InventoryItem restored = JsonUtility.FromJson<InventoryItem>(json);
            AssertEqual(replacement.PermanentEnchantmentId, restored.PermanentEnchantmentId, "JSON roundtrip preserves permanent affinity metadata");
            AssertEqual(replacement.TemporaryEnchantmentId, restored.TemporaryEnchantmentId, "JSON roundtrip preserves temporary affinity metadata");
            AssertEqual(replacement.TemporaryEnchantmentVictoriesRemaining, restored.TemporaryEnchantmentVictoriesRemaining, "JSON roundtrip preserves temporary countdown metadata");
            AssertEqual(replacement.EnchantmentBaseCaptured, restored.EnchantmentBaseCaptured, "JSON roundtrip preserves base-capture state");
            AssertEqual(replacement.EnchantmentBaseDisplayName, restored.EnchantmentBaseDisplayName, "JSON roundtrip preserves base item text");
            AssertEqual(replacement.EnchantmentBaseTrait, restored.EnchantmentBaseTrait, "JSON roundtrip preserves base trait text");
            AssertEqual(replacement.EnchantmentBaseDamageType, restored.EnchantmentBaseDamageType, "JSON roundtrip preserves base damage type");
            AssertEqual(replacement.DisplayName, restored.DisplayName, "JSON roundtrip preserves enchanted display text");
            AssertEqual(replacement.DamageType, restored.DamageType, "JSON roundtrip preserves active affinity");
            WeaponEnchantmentRules.Rebuild(restored);
            AssertEqual(replacement.DisplayName, restored.DisplayName, "roundtripped metadata deterministically rebuilds enchanted text");
        }

        private static void PresentationRefreshKeysAreStableUntilStateChanges()
        {
            string first = PresentationRefreshRules.ComposeKey(1600, 900, 12, "mode=explore|x=1|y=2");
            string same = PresentationRefreshRules.ComposeKey(1600, 900, 12, "mode=explore|x=1|y=2");
            string resized = PresentationRefreshRules.ComposeKey(1920, 1080, 12, "mode=explore|x=1|y=2");
            string revised = PresentationRefreshRules.ComposeKey(1600, 900, 13, "mode=explore|x=1|y=2");
            string hoverChanged = PresentationRefreshRules.ComposeKey(1600, 900, 12, "mode=explore|x=1|y=3");

            AssertEqual(false, PresentationRefreshRules.KeyChanged(first, same), "idle refresh key remains stable");
            AssertEqual(true, PresentationRefreshRules.KeyChanged(first, resized), "screen size changes refresh key");
            AssertEqual(true, PresentationRefreshRules.KeyChanged(first, revised), "ui revision changes refresh key");
            AssertEqual(true, PresentationRefreshRules.KeyChanged(first, hoverChanged), "local view state changes refresh key");
        }

        private static void ExplorationUseTargetPriorityIsContextual()
        {
            MapData stairsMap = OpenTestMap(3, 3);
            MapObject stairs = new MapObject(1, 1, ObjectType.Stairs);
            MapObject healer = new MapObject(1, 0, ObjectType.TempleHealer);
            stairsMap.Objects.Add(stairs);
            stairsMap.Objects.Add(healer);

            bool found = ExplorationInteractionRules.TryFindUseTarget(stairsMap, 1, 1, obj => obj == healer, AlwaysPassable, ExplorationTraversalRules.CanUseFromAdjacent, out ExplorationUseTarget selection);
            AssertEqual(true, found, "underfoot stairs target found");
            AssertEqual(ObjectType.Stairs, selection.Target.Type, "underfoot stairs beat adjacent objective");
            AssertEqual(0, selection.StepX, "underfoot stairs step x");
            AssertEqual(0, selection.StepY, "underfoot stairs step y");

            MapData adjacentMap = OpenTestMap(3, 3);
            adjacentMap.Objects.Add(new MapObject(1, 0, ObjectType.Shrine));
            adjacentMap.Objects.Add(new MapObject(2, 1, ObjectType.Cache));
            found = ExplorationInteractionRules.TryFindUseTarget(adjacentMap, 1, 1, obj => false, AlwaysPassable, ExplorationTraversalRules.CanUseFromAdjacent, out selection);
            AssertEqual(true, found, "adjacent target found");
            AssertEqual(ObjectType.Cache, selection.Target.Type, "best adjacent target by priority");
            AssertEqual(1, selection.StepX, "adjacent target step x");
            AssertEqual(0, selection.StepY, "adjacent target step y");

            MapData facedMap = OpenTestMap(3, 3);
            facedMap.Objects.Add(new MapObject(1, 0, ObjectType.TempleHealer));
            facedMap.Objects.Add(new MapObject(2, 1, ObjectType.GateCaptain));
            found = ExplorationInteractionRules.TryFindUseTarget(facedMap, 1, 1, obj => false, AlwaysPassable, ExplorationTraversalRules.CanUseFromAdjacent, 1, 0, out selection);
            AssertEqual(true, found, "faced adjacent target found");
            AssertEqual(ObjectType.GateCaptain, selection.Target.Type, "facing breaks ties within one semantic priority");
            AssertEqual(1, selection.StepX, "faced target step x");
            AssertEqual(0, selection.StepY, "faced target step y");

            MapData emptyMap = OpenTestMap(3, 3);
            emptyMap.Objects.Add(new MapObject(1, 0, ObjectType.CityWall));
            found = ExplorationInteractionRules.TryFindUseTarget(emptyMap, 1, 1, obj => false, AlwaysPassable, ExplorationTraversalRules.CanUseFromAdjacent, out selection);
            AssertEqual(false, found, "no contextual target when none usable");

            MapData guardMap = OpenTestMap(3, 3);
            guardMap.Objects.Add(new MapObject(1, 0, ObjectType.TownGuard));
            guardMap.Objects.Add(new MapObject(2, 1, ObjectType.EastGate));
            found = ExplorationInteractionRules.TryFindUseTarget(guardMap, 1, 1, obj => false, AlwaysPassable, ExplorationTraversalRules.CanUseFromAdjacent, out selection);
            AssertEqual(true, found, "guard and gate contextual target found");
            AssertEqual(ObjectType.TownGuard, selection.Target.Type, "talking to a nearby guard outranks inspecting a gate");

            found = ExplorationInteractionRules.TryFindUseTarget(guardMap, 1, 1, obj => obj.Type == ObjectType.EastGate, AlwaysPassable, ExplorationTraversalRules.CanUseFromAdjacent, 0, -1, out selection);
            AssertEqual(true, found, "current gate objective target found");
            AssertEqual(ObjectType.EastGate, selection.Target.Type, "facing remains a tie-breaker and cannot override the current objective");
        }

        private static void ExplorationControllerOwnsMovementAndContextualUse()
        {
            MapData map = OpenTestMap(4, 3);
            MapObject cache = new MapObject(2, 1, ObjectType.Cache);
            map.Objects.Add(cache);
            GameState state = new GameState { Map = map, PlayerX = 1, PlayerY = 1 };
            ExplorationController controller = TestExplorationController(state);

            ExplorationInteraction interaction = controller.CurrentInteraction();
            AssertEqual(true, interaction.HasTarget, "controller contextual target found");
            AssertEqual(ObjectType.Cache, interaction.Target.Type, "controller contextual target type");
            AssertEqual("Loot", interaction.Verb, "controller contextual verb");
            AssertEqual("Cache", interaction.TargetName, "controller target label");
            AssertEqual(1, interaction.StepX, "controller target step x");
            AssertEqual(0, interaction.StepY, "controller target step y");

            bool used = controller.TryUseContextualTarget(out ExplorationCommandResult command);
            AssertEqual(true, used, "controller contextual use resolves target");
            AssertEqual(ExplorationCommandKind.ResolveTarget, command.Kind, "controller contextual use kind");
            AssertEqual(1, command.Move.OldX, "controller use old x");
            AssertEqual(2, command.Move.NewX, "controller target x");
            AssertEqual(1, state.PlayerX, "controller leaves player beside target");
            AssertEqual(1, state.PlayerY, "controller keeps player y");

            state.PlayerX = 1;
            state.PlayerY = 1;
            SetTestTile(map, 2, 1, 0);
            bool moved = controller.TryMove(1, 0, out ExplorationMoveResult blocked);
            AssertEqual(false, moved, "controller blocked move rejected");
            AssertEqual(false, blocked.Moved, "controller blocked result");
            AssertEqual(1, state.PlayerX, "blocked move leaves x");

            MapData stairsMap = OpenTestMap(3, 3);
            stairsMap.Objects.Add(new MapObject(1, 1, ObjectType.Stairs));
            GameState stairsState = new GameState { Map = stairsMap, PlayerX = 1, PlayerY = 1 };
            controller = TestExplorationController(stairsState);
            used = controller.TryUseContextualTarget(out command);
            AssertEqual(true, used, "controller underfoot stairs use");
            AssertEqual(ExplorationCommandKind.Descend, command.Kind, "controller underfoot stairs command");
            AssertEqual(1, stairsState.PlayerX, "descend command does not move x");
        }

        private static void ExplorationTraversalBlocksObjectOverlap()
        {
            AssertEqual(true, ExplorationTraversalRules.CanStandOnObject((MapObject)null), "empty floor remains standable");
            AssertEqual(true, ExplorationTraversalRules.CanStandOnObject(ObjectType.Stairs), "stairs remain standable");
            AssertEqual(true, ExplorationTraversalRules.CanStandOnObject(ObjectType.EastGate), "east gate remains pass-through");
            AssertEqual(true, ExplorationTraversalRules.CanStandOnObject(ObjectType.WestGate), "west gate remains pass-through");
            AssertEqual(true, ExplorationTraversalRules.CanStandOnObject(ObjectType.Camp), "camp markers do not sever travel routes");
            AssertEqual(true, ExplorationTraversalRules.CanStandOnObject(ObjectType.Bridge), "bridge landmarks remain pass-through");
            AssertEqual(false, ExplorationTraversalRules.CanStandOnObject(ObjectType.NorthGate), "sealed north gate blocks standing overlap");
            AssertEqual(false, ExplorationTraversalRules.CanStandOnObject(ObjectType.SouthGate), "sealed south gate blocks standing overlap");
            AssertEqual(false, ExplorationTraversalRules.CanStandOnObject(ObjectType.TempleHealer), "npc blocks standing overlap");
            AssertEqual(false, ExplorationTraversalRules.CanStandOnObject(ObjectType.Cache), "cache blocks standing overlap");
            AssertEqual(false, ExplorationTraversalRules.CanStandOnObject(ObjectType.CityWall), "city wall blocks standing overlap");
            AssertEqual(true, ExplorationTraversalRules.CanUseFromAdjacent(ObjectType.TempleHealer), "npc can be used from adjacent tile");
            AssertEqual(true, ExplorationTraversalRules.CanUseFromAdjacent(ObjectType.Cache), "cache can be used from adjacent tile");
            AssertEqual(false, ExplorationTraversalRules.CanUseFromAdjacent(ObjectType.Stairs), "stairs still require standing on the tile");

            MapData map = OpenTestMap(4, 3);
            MapObject healer = new MapObject(2, 1, ObjectType.TempleHealer);
            map.Objects.Add(healer);
            GameState state = new GameState { Map = map, PlayerX = 1, PlayerY = 1 };
            ExplorationController controller = TestExplorationController(state);

            bool moved = controller.TryMove(1, 0, out ExplorationMoveResult move);
            AssertEqual(false, moved, "controller rejects moving onto npc");
            AssertEqual(false, move.Moved, "npc-blocked move result");
            AssertEqual(1, state.PlayerX, "npc-blocked move leaves x");

            bool used = controller.TryUseContextualTarget(out ExplorationCommandResult command);
            AssertEqual(true, used, "adjacent npc use succeeds");
            AssertEqual(ExplorationCommandKind.ResolveTarget, command.Kind, "adjacent npc resolves directly");
            AssertEqual(ObjectType.TempleHealer, command.Interaction.Target.Type, "adjacent npc command target");
            AssertEqual(1, state.PlayerX, "adjacent npc use does not overlap sprite");
        }

        private static void MapDataObjectLookupCacheTracksMutations()
        {
            MapData map = OpenTestMap(4, 3);
            MapObject first = new MapObject(1, 1, ObjectType.Cache, "lookup-first");
            map.Objects.Add(first);

            AssertEqual(first, map.FindObjectAt(1, 1), "cell lookup resolves the initial object");
            AssertEqual(first, map.FindObjectById("lookup-first"), "id lookup resolves the initial object");
            AssertEqual<MapObject>(null, map.FindObjectAt(3, 2), "cell lookup reports an initial miss");
            AssertEqual<MapObject>(null, map.FindObjectById("lookup-missing"), "id lookup reports an initial miss");

            MapObject second = new MapObject(2, 1, ObjectType.Shrine, "lookup-second");
            map.Objects.Add(second);
            AssertEqual(second, map.FindObjectAt(2, 1), "count change rebuilds the cell lookup");
            AssertEqual(second, map.FindObjectById("lookup-second"), "count change rebuilds the id lookup");

            second.X = 3;
            second.Y = 2;
            second.Id = "lookup-moved";
            map.InvalidateObjectLookup();
            AssertEqual<MapObject>(null, map.FindObjectAt(2, 1), "explicit invalidation removes a moved object's old cell");
            AssertEqual<MapObject>(null, map.FindObjectById("lookup-second"), "explicit invalidation removes an object's old id");
            AssertEqual(second, map.FindObjectAt(3, 2), "explicit invalidation exposes a same-count cell move");
            AssertEqual(second, map.FindObjectById("lookup-moved"), "explicit invalidation exposes a same-count id change");

            map.Objects.Remove(second);
            AssertEqual<MapObject>(null, map.FindObjectAt(3, 2), "removal rebuilds the cell lookup");
            AssertEqual<MapObject>(null, map.FindObjectById("lookup-moved"), "removal rebuilds the id lookup");
            AssertEqual(first, map.FindObjectAt(1, 1), "removal preserves surviving cell entries");
            AssertEqual(first, map.FindObjectById("lookup-first"), "removal preserves surviving id entries");
        }

        private static void ExplorationTraversalCertifiesConnectedTargets()
        {
            MapData map = new MapData { Width = 7, Height = 5, Depth = 2, StartX = 1, StartY = 2 };
            for (int i = 0; i < map.Width * map.Height; i++) map.Tiles.Add(0);
            for (int y = 1; y <= 3; y++)
            {
                SetTestTile(map, 1, y, 1);
                SetTestTile(map, 2, y, 1);
                SetTestTile(map, 4, y, 1);
                SetTestTile(map, 5, y, 1);
            }
            MapObject stair = new MapObject(5, 2, ObjectType.Stairs);
            map.Objects.Add(stair);

            bool[,] reachable = ExplorationTraversalRules.ReachableMask(map, 1, 2);
            AssertEqual(6, ExplorationTraversalRules.ReachableCount(reachable), "flood fill remains in the connected component");
            AssertEqual(false, ExplorationTraversalRules.CanReachObject(reachable, map, stair), "disconnected stair fails the route certificate");

            SetTestTile(map, 3, 2, 1);
            reachable = ExplorationTraversalRules.ReachableMask(map, 1, 2);
            AssertEqual(true, ExplorationTraversalRules.CanReachObject(reachable, map, stair), "carved connector certifies the stair route");
            List<Point> stairPath = ExplorationTraversalRules.FindPathToObject(map, 1, 2, stair);
            AssertEqual(5, stairPath.Count, "shortest route includes start and reachable stair");
            AssertEqual(2, stairPath[1].X, "path guidance chooses the first open east step");
            AssertEqual(2, stairPath[1].Y, "path guidance does not point into a wall");

            MapObject gate = new MapObject(2, 2, ObjectType.EastGate);
            map.Objects.Add(gate);
            reachable = ExplorationTraversalRules.ReachableMask(map, 1, 2);
            AssertEqual(true, reachable[4, 2], "pass-through east gate remains inside the reachable component");

            MapObject cache = new MapObject(5, 1, ObjectType.Cache);
            map.Objects.Add(cache);
            reachable = ExplorationTraversalRules.ReachableMask(map, 1, 2);
            AssertEqual(true, ExplorationTraversalRules.CanReachObject(reachable, map, cache), "blocking interaction is certified through a reachable adjacent tile");
            List<Point> cachePath = ExplorationTraversalRules.FindPathToObject(map, 1, 2, cache);
            AssertEqual(true, cachePath.Count > 0, "path guidance reaches an adjacent interaction tile");
            Point cacheApproach = cachePath[cachePath.Count - 1];
            AssertEqual(1, Mathf.Abs(cacheApproach.X - cache.X) + Mathf.Abs(cacheApproach.Y - cache.Y), "blocking target route ends adjacent");
        }

        private static void MidgaardInteriorPortalsRemainPaired()
        {
            MapData map = new MapData
            {
                Width = 8,
                Height = 6,
                Tiles = Enumerable.Repeat(1, 48).ToList(),
                SurfaceMaterials = Enumerable.Repeat((int)ExplorationMaterial.KeepStone, 48).ToList(),
                SurfaceRoles = Enumerable.Repeat((int)ExplorationCellRole.Room, 48).ToList(),
                Objects = new List<MapObject>()
            };
            MapObject exterior = new MapObject(
                2,
                2,
                ObjectType.KingHall,
                MidgaardInteriorRules.KingHallDoorId,
                MidgaardInteriorRules.ThroneRoomExitId);
            MapObject interior = new MapObject(
                5,
                3,
                ObjectType.InteriorDoor,
                MidgaardInteriorRules.ThroneRoomExitId,
                MidgaardInteriorRules.KingHallDoorId);
            map.Objects.Add(exterior);
            map.Objects.Add(interior);

            AssertEqual(true, MidgaardInteriorRules.IsPortal(exterior), "named exterior doorway is a portal");
            AssertEqual(true, MidgaardInteriorRules.HasValidTarget(map, exterior), "exterior doorway resolves its interior target");
            AssertEqual(true, MidgaardInteriorRules.HasValidTarget(map, interior), "interior doorway resolves its exterior target");
            AssertEqual(0, MidgaardInteriorRules.BrokenPortalIds(map).Count, "paired interior portals report no broken links");
            AssertEqual(true, MidgaardInteriorRules.TryFindArrival(map, interior, out Point arrival), "portal finds a safe adjacent arrival");
            AssertEqual(1, Math.Abs(arrival.X - interior.X) + Math.Abs(arrival.Y - interior.Y), "portal arrival is exactly one tile from the destination");

            interior.TargetId = "missing-door";
            AssertEqual(false, MidgaardInteriorRules.HasValidTarget(map, interior), "missing portal target is rejected");
            AssertEqual(MidgaardInteriorRules.ThroneRoomExitId, MidgaardInteriorRules.BrokenPortalIds(map).Single(), "broken portal reports its stable identity");
        }

        private static void GrandHearthInteriorContractIsStable()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(58, 46),
                new Vector2Int(50, 32),
                new Vector2Int(46, 30)
            };
            foreach (Vector2Int size in sizes)
            {
                MapData map = new MapData
                {
                    Width = size.x,
                    Height = size.y,
                    Depth = 1,
                    StartX = WorldMapGenerationRules.StartX(size.x),
                    StartY = WorldMapGenerationRules.StartY(size.y)
                };
                RectInt room = MidgaardInteriorRules.GrandHearthBounds(map);
                Point spawn = MidgaardInteriorRules.GrandHearthSpawn(map);
                Point exit = MidgaardInteriorRules.GrandHearthExit(map);
                AssertEqual(true, room.xMin >= 1 && room.yMin >= 1 && room.xMax < size.x && room.yMax < size.y, $"Grand Hearth room fits supported map {size.x}x{size.y}");
                AssertEqual(true, room.Contains(new Vector2Int(spawn.X, spawn.Y)), $"Grand Hearth spawn stays inside room {size.x}x{size.y}");
                AssertEqual(true, room.Contains(new Vector2Int(exit.X, exit.Y)), $"Grand Hearth exit stays inside room {size.x}x{size.y}");
                AssertEqual(room.xMax - 1, exit.X, $"Grand Hearth storm doors remain on the east wall {size.x}x{size.y}");
                AssertEqual(false, room.Overlaps(MidgaardInteriorRules.ThroneRoomBounds(map)), $"Grand Hearth remains separate from the throne room {size.x}x{size.y}");
                AssertEqual(false, room.Overlaps(MidgaardInteriorRules.MerchantHallBounds(map)), $"Grand Hearth remains separate from the merchant hall {size.x}x{size.y}");
                AssertEqual(true, MidgaardInteriorRules.IsReservedCell(map, spawn.X, spawn.Y), $"Grand Hearth spawn is protected from procedural landmarks {size.x}x{size.y}");
            }

            MapData portalMap = new MapData
            {
                Width = 8,
                Height = 6,
                Depth = 1,
                Tiles = Enumerable.Repeat(1, 48).ToList(),
                SurfaceMaterials = Enumerable.Repeat((int)ExplorationMaterial.KeepStone, 48).ToList(),
                SurfaceRoles = Enumerable.Repeat((int)ExplorationCellRole.Room, 48).ToList(),
                Objects = new List<MapObject>()
            };
            MapObject exterior = new MapObject(2, 2, ObjectType.Tavern, MidgaardInteriorRules.GrandHearthDoorId, MidgaardInteriorRules.GrandHearthExitId);
            MapObject interior = new MapObject(5, 3, ObjectType.InteriorDoor, MidgaardInteriorRules.GrandHearthExitId, MidgaardInteriorRules.GrandHearthDoorId);
            portalMap.Objects.Add(exterior);
            portalMap.Objects.Add(interior);
            AssertEqual(true, MidgaardInteriorRules.HasValidTarget(portalMap, exterior), "Grand Hearth exterior storm doors resolve the interior exit");
            AssertEqual(true, MidgaardInteriorRules.HasValidTarget(portalMap, interior), "Grand Hearth interior storm doors resolve the Midgaard doorway");
            AssertEqual(0, MidgaardInteriorRules.BrokenPortalIds(portalMap).Count, "Grand Hearth portal pair is complete");
            AssertEqual("Grand Hearth", WorldZoneCatalog.For("midgaard-grand-hearth", 1).Name, "Grand Hearth has a dedicated player-facing zone");
            AssertEqual(MusicDirectorRules.Tavern, MusicDirectorRules.ExploreTrackKey("midgaard-grand-hearth", ObjectType.Tavern, true, false), "Grand Hearth continues the title overture");
            AssertEqual("ambhearth", GameAudioCueRules.AmbientFor("midgaard-grand-hearth", null), "Grand Hearth uses the hearth ambience bed");
        }

        private static void RoamingThreatsTelegraphAndPathAroundTerrain()
        {
            AssertEqual(
                RoamingThreatCatalog.All.Count,
                RoamingThreatCatalog.All.Select(definition => definition.Id).Distinct().Count(),
                "roaming-threat catalog identities are globally unique for save repair");
            IReadOnlyList<RoamingThreatDefinition> chapterOne = RoamingThreatCatalog.ForDepth(1, false);
            IReadOnlyList<RoamingThreatDefinition> chapterOnePrototype = RoamingThreatCatalog.ForDepth(1, true);
            AssertRoamingThreatRoster(chapterOne, 1, 4, "shared chapter-one patrol roster");
            AssertEqual(
                string.Join("|", chapterOne.Select(definition => definition.Id)),
                string.Join("|", chapterOnePrototype.Select(definition => definition.Id)),
                "chapter-one patrol identities stay stable across content profiles");
            AssertEqual(true, chapterOne.Any(definition => definition.Id == "midgaard-rat-patrol-west"), "west patrol keeps its save-compatible identity");
            AssertEqual(true, chapterOne.Any(definition => definition.Id == "midgaard-rat-patrol-east"), "east patrol keeps its save-compatible identity");

            for (int depth = 2; depth <= 6; depth++)
            {
                IReadOnlyList<RoamingThreatDefinition> safeRoster = RoamingThreatCatalog.ForDepth(depth, false);
                AssertRoamingThreatRoster(safeRoster, depth, 3, "sewer-slice fallback patrol roster");
                AssertEqual(
                    true,
                    safeRoster.All(definition =>
                        definition.Archetype == "rats"
                        || definition.Archetype == "ratfolk"
                        || definition.Archetype == "ratcleric"),
                    "sewer-slice later-depth patrols stay aligned with its rat-only combat pool at depth " + depth);
            }

            int[] prototypeCounts = { 0, 4, 4, 5, 5, 5 };
            for (int depth = 2; depth <= 6; depth++)
            {
                AssertRoamingThreatRoster(
                    RoamingThreatCatalog.ForDepth(depth, true),
                    depth,
                    prototypeCounts[depth - 1],
                    "full-prototype patrol progression");
            }

            IReadOnlyList<RoamingThreatDefinition> chapterTwo = RoamingThreatCatalog.ForDepth(2, true);
            IReadOnlyList<RoamingThreatDefinition> chapterThree = RoamingThreatCatalog.ForDepth(3, true);
            IReadOnlyList<RoamingThreatDefinition> chapterFour = RoamingThreatCatalog.ForDepth(4, true);
            AssertEqual(true, chapterTwo.Any(definition => definition.Archetype == "kobolds"), "chapter two introduces a visible kobold patrol");
            AssertEqual(true, chapterTwo.Any(definition => definition.Archetype == "drowscout"), "chapter two telegraphs the first drow scouts");
            AssertEqual(true, chapterTwo.Any(definition => definition.Archetype == "undead"), "chapter two telegraphs the first restless dead");
            AssertEqual(true, chapterThree.Any(definition => definition.Archetype == "drowmage"), "chapter three escalates the drow patrol silhouette");
            AssertEqual(true, chapterThree.Any(definition => definition.Archetype == "bonepriest"), "chapter three introduces the death-cult patrol");
            AssertEqual(true, chapterFour.Any(definition => definition.Archetype == "lesserdemon"), "chapter four introduces a supported demon patrol");
            AssertEqual(
                string.Join("|", chapterFour.Select(definition => definition.Id)),
                string.Join("|", RoamingThreatCatalog.ForDepth(4, true).Select(definition => definition.Id)),
                "patrol selection is deterministic for the same depth and content profile");

            Dictionary<RoamingThreatFaction, int[]> behaviorExpectations = new Dictionary<RoamingThreatFaction, int[]>
            {
                { RoamingThreatFaction.Rats, new[] { 4, 2, 2, 6 } },
                { RoamingThreatFaction.Kobolds, new[] { 5, 2, 3, 8 } },
                { RoamingThreatFaction.Drow, new[] { 6, 2, 4, 10 } },
                { RoamingThreatFaction.Undead, new[] { 4, 3, 5, 11 } },
                { RoamingThreatFaction.Demons, new[] { 6, 1, 2, 12 } }
            };
            HashSet<string> behaviorSignatures = new HashSet<string>(StringComparer.Ordinal);
            foreach (KeyValuePair<RoamingThreatFaction, int[]> expected in behaviorExpectations)
            {
                RoamingThreatBehaviorProfile behavior = RoamingThreatRules.ProfileFor(expected.Key);
                AssertEqual(expected.Value[0], behavior.AlertRadius, expected.Key + " patrol alert radius");
                AssertEqual(expected.Value[1], behavior.PursuitCadence, expected.Key + " patrol pursuit cadence");
                AssertEqual(expected.Value[2], behavior.ReturnCadence, expected.Key + " patrol return cadence");
                AssertEqual(expected.Value[3], behavior.LeashRadius, expected.Key + " patrol leash radius");
                AssertEqual(true, behavior.LeashRadius > behavior.AlertRadius, expected.Key + " patrol can disengage before overrunning its home range");
                AssertEqual(false, RoamingThreatRules.ShouldAlert(behavior.AlertRadius, true, 0, behavior), expected.Key + " patrol respects safe roads");
                AssertEqual(false, RoamingThreatRules.ShouldAlert(behavior.AlertRadius, false, 1, behavior), expected.Key + " patrol respects retreat grace");
                AssertEqual(true, RoamingThreatRules.ShouldAlert(behavior.AlertRadius, false, 0, behavior), expected.Key + " patrol alerts at its faction boundary");
                AssertEqual(false, RoamingThreatRules.ShouldAlert(behavior.AlertRadius + 1, false, 0, behavior), expected.Key + " patrol stays quiet beyond its faction boundary");
                behaviorSignatures.Add(behavior.AlertRadius + ":" + behavior.PursuitCadence + ":" + behavior.ReturnCadence + ":" + behavior.LeashRadius);
            }
            AssertEqual(behaviorExpectations.Count, behaviorSignatures.Count, "every roaming faction owns a distinct travel behavior signature");

            RoamingThreatBehaviorProfile rats = RoamingThreatRules.ProfileFor(RoamingThreatFaction.Rats);
            RoamingThreatBehaviorProfile drow = RoamingThreatRules.ProfileFor(RoamingThreatFaction.Drow);
            RoamingThreatBehaviorProfile undead = RoamingThreatRules.ProfileFor(RoamingThreatFaction.Undead);
            RoamingThreatBehaviorProfile demons = RoamingThreatRules.ProfileFor(RoamingThreatFaction.Demons);
            AssertEqual(false, RoamingThreatRules.ShouldAlert(5, false, 0, rats), "rats are skittish instead of long-range sentries");
            AssertEqual(true, RoamingThreatRules.ShouldAlert(6, false, 0, drow), "drow keep the longest watch line");
            AssertEqual(true, RoamingThreatRules.ShouldPursue(1, demons), "demons advance on every alerted travel step");
            AssertEqual(false, RoamingThreatRules.ShouldPursue(2, undead), "undead advance more slowly than living patrols");
            AssertEqual(true, RoamingThreatRules.ShouldPursue(3, undead), "undead advance on their deliberate third step");
            AssertEqual(true, RoamingThreatRules.ShouldReturnHome(2, rats), "rats quickly fall back toward their nest");
            AssertEqual(false, RoamingThreatRules.ShouldReturnHome(2, drow), "drow hold their extended watch before returning");
            AssertEqual(true, RoamingThreatRules.ShouldReturnHome(4, drow), "drow return on their fourth travel step");
            AssertEqual(true, RoamingThreatRules.ShouldLeash(6, rats), "rats keep a tight home range");
            AssertEqual(false, RoamingThreatRules.ShouldLeash(6, drow), "drow can stalk beyond the rat home range");
            AssertEqual(8, RoamingThreatRules.DisengageRadius(drow), "drow threat guidance includes a two-cell readable disengage buffer");

            AssertEqual(false, RoamingThreatRules.ShouldAlert(3, true, 0), "safe roads suppress roaming threat alerts");
            AssertEqual(false, RoamingThreatRules.ShouldAlert(3, false, 1), "retreat grace suppresses roaming threat alerts");
            AssertEqual(true, RoamingThreatRules.ShouldAlert(3, false, 0), "danger-zone patrols alert inside the telegraph radius");
            AssertEqual(false, RoamingThreatRules.ShouldPursue(1), "patrols do not move on every travel step");
            AssertEqual(true, RoamingThreatRules.ShouldPursue(2), "patrols move on their second travel step");
            AssertEqual(RoamingThreatRules.DefeatRespawnSteps, 36, "defeated patrol respawn cadence");
            AssertEqual(RoamingThreatRules.RetreatGraceSteps, 6, "retreat grants a readable patrol grace window");
            AssertEqual(0, RoamingThreatPresentationRules.SpriteIndex("rats"), "rat patrol uses the sewer scout cell");
            AssertEqual(1, RoamingThreatPresentationRules.SpriteIndex("ratfolk"), "ratfolk patrol uses the armored brute cell");
            AssertEqual(5, RoamingThreatPresentationRules.SpriteIndex("kobolds"), "kobold patrol uses the raider-band cell");
            AssertEqual(11, RoamingThreatPresentationRules.SpriteIndex("drow"), "drow patrol uses the warband cell");
            AssertEqual(8, RoamingThreatPresentationRules.SpriteIndex("demon"), "demon patrol uses the greater-demon cell");
            AssertEqual("koboldalert", CreatureAudioRules.CueForArchetype("kobolds", "alert"), "kobold patrol owns a faction alert cue");
            AssertEqual("drowstep", CreatureAudioRules.CueForArchetype("drow", "step"), "drow patrol owns a quiet movement cue");
            AssertEqual("demondeath", CreatureAudioRules.CueForArchetype("greaterdemon", "death"), "greater demons own a defeat voice");
            AssertEqual("undeadcast", CreatureAudioRules.CueForArchetype("bonepriest", "cast"), "bone priests own an undead casting voice");
            AssertEqual("ratchitter", CreatureAudioRules.CueForArchetype("ratcaptain", "alert"), "rat captains retain rat-family alert audio");
            AssertEqual("ratattack", CreatureAudioRules.CueForArchetype("ratswarm", "attack"), "rat swarms retain rat-family attack audio");
            AssertEqual("ratdeath", CreatureAudioRules.CueForArchetype("plaguerats", "death"), "plague rats retain rat-family defeat audio");

            bool found = RoamingThreatRules.TryNextStep(
                7,
                5,
                1,
                2,
                5,
                2,
                (x, y) => x > 0 && x < 6 && y > 0 && y < 4 && !(x == 2 && y == 2),
                (x, y) => false,
                true,
                out Point next);
            AssertEqual(true, found, "roaming threat BFS finds a route around terrain");
            AssertEqual(true, next != null && !(next.X == 2 && next.Y == 2), "roaming threat does not choose the blocked direct step");
            AssertEqual(1, Math.Abs(next.X - 1) + Math.Abs(next.Y - 2), "roaming threat advances exactly one orthogonal tile");

            bool blocked = RoamingThreatRules.TryNextStep(
                7,
                5,
                1,
                2,
                5,
                2,
                (x, y) => x > 0 && x < 6 && y > 0 && y < 4 && x != 3,
                (x, y) => false,
                true,
                out _);
            AssertEqual(false, blocked, "roaming threats cannot cross an impassable boundary");
        }

        private static void AssertRoamingThreatRoster(
            IReadOnlyList<RoamingThreatDefinition> roster,
            int depth,
            int expectedCount,
            string label)
        {
            AssertEqual(expectedCount, roster.Count, label + " count at depth " + depth);
            AssertEqual(expectedCount, roster.Select(definition => definition.Slot).Distinct().Count(), label + " uses distinct deterministic slots at depth " + depth);
            AssertEqual(expectedCount, roster.Select(definition => definition.Id).Distinct().Count(), label + " uses distinct stable IDs at depth " + depth);
            AssertEqual(expectedCount, roster.Select(definition => definition.Archetype).Distinct().Count(), label + " uses distinct art archetypes at depth " + depth);
            AssertEqual(expectedCount, roster.Select(definition => definition.PreferredZoneId).Distinct().Count(), label + " uses distinct zone preferences at depth " + depth);
            foreach (RoamingThreatDefinition definition in roster)
            {
                AssertEqual(true, definition.MinDepth <= depth && definition.MaxDepth >= depth, definition.Id + " includes its selected depth");
                AssertEqual(true, !string.IsNullOrEmpty(definition.Name), definition.Id + " has a player-facing name");
                RoamingThreatBehaviorProfile behavior = definition.BehaviorProfile;
                RoamingThreatBehaviorProfile expectedBehavior = RoamingThreatRules.ProfileFor(definition.Faction);
                AssertEqual(expectedBehavior.Id, behavior.Id, definition.Id + " resolves its faction behavior identity");
                AssertEqual(expectedBehavior.AlertRadius, behavior.AlertRadius, definition.Id + " resolves its faction alert radius");
                AssertEqual(expectedBehavior.PursuitCadence, behavior.PursuitCadence, definition.Id + " resolves its faction pursuit cadence");
                AssertEqual(expectedBehavior.ReturnCadence, behavior.ReturnCadence, definition.Id + " resolves its faction return cadence");
                AssertEqual(expectedBehavior.LeashRadius, behavior.LeashRadius, definition.Id + " resolves its faction leash radius");
                AssertEqual(true, !string.IsNullOrEmpty(behavior.Id), definition.Id + " has a stable behavior identity");
                AssertEqual(true, behavior.PursuitCadence > 0 && behavior.ReturnCadence > 0, definition.Id + " has valid travel cadences");
                AssertEqual(true, behavior.LeashRadius > behavior.AlertRadius, definition.Id + " has a readable home-range boundary");
                AssertEqual(true, RoamingThreatPresentationRules.SpriteIndex(definition.Archetype) != 19, definition.Id + " resolves approved roaming-threat art");
                AssertEqual(definition.Faction, RoamingThreatCatalog.FactionForArchetype(definition.Archetype), definition.Id + " art archetype matches its combat faction");
                AssertEqual(true, WorldZoneCatalog.For(definition.PreferredZoneId, depth).Danger > 0, definition.Id + " prefers a hostile zone instead of Midgaard or a safe road");
                AssertEqual(true, definition.TargetDistance >= 7 && definition.TargetDistance <= 24, definition.Id + " keeps the established spawn-distance envelope");
                AssertEqual(true, ReferenceEquals(definition, RoamingThreatCatalog.Find(definition.Id, depth, definition.ContentProfile != RoamingThreatContentProfile.SewerSlice)), definition.Id + " resolves deterministically by stable identity");
                AssertEqual(true, definition.EnemyIds.Count >= 2, definition.Id + " has an explicit combat roster");
                foreach (string enemyId in definition.EnemyIds)
                {
                    AssertEqual(true, EncounterCatalog.IsKnownEnemyId(enemyId), definition.Id + " combat roster references known enemy " + enemyId);
                    AssertEqual(definition.Faction, RoamingThreatCatalog.FactionForEnemy(enemyId), definition.Id + " combat enemy " + enemyId + " matches the visible faction");
                    if (definition.ContentProfile != RoamingThreatContentProfile.FullPrototype)
                    {
                        AssertEqual(true, ContentSetCatalog.EnemyActive(ContentSetCatalog.SewerSlice, enemyId), definition.Id + " remains combat-safe in the sewer slice");
                    }
                }

                EncounterDefinition encounter = RoamingThreatCatalog.BuildEncounter(definition, depth);
                AssertEqual(EncounterId.Patrol, encounter.Id, definition.Id + " retains the patrol encounter lifecycle");
                AssertEqual("patrol", encounter.LegacyStyle, definition.Id + " retains patrol rewards and victory routing");
                AssertEqual(false, encounter.UsesGeneratedEnemyPool, definition.Id + " cannot drift back to an unrelated zone pool");
                AssertEqual(
                    EncounterCatalog.For(EncounterId.Patrol).EnemyCountForDepth(depth),
                    encounter.EnemyCountForDepth(depth),
                    definition.Id + " retains standard patrol difficulty count");
                AssertEqual(
                    string.Join("|", definition.EnemyIds),
                    string.Join("|", encounter.EnemyIds),
                    definition.Id + " copies its explicit combat roster without mutation");
            }
        }

        private static MapData OpenTestMap(int width, int height)
        {
            MapData map = new MapData { Width = width, Height = height, Depth = 1, StartX = 1, StartY = 1 };
            for (int i = 0; i < width * height; i++) map.Tiles.Add(1);
            return map;
        }

        private static bool AlwaysPassable(int x, int y)
        {
            return true;
        }

        private static ExplorationController TestExplorationController(GameState state)
        {
            return new ExplorationController(
                state,
                obj => false,
                (x, y) => x >= 0
                    && y >= 0
                    && x < state.Map.Width
                    && y < state.Map.Height
                    && state.Map.Tiles[y * state.Map.Width + x] == 1
                    && ExplorationTraversalRules.CanStandOnObject(ObjectAtTest(state.Map, x, y)),
                ExplorationTraversalRules.CanUseFromAdjacent,
                (obj, dx, dy) => obj.Type == ObjectType.Cache ? "Loot" : obj.Type == ObjectType.Stairs ? (dx == 0 && dy == 0 ? "Descend" : "Enter") : "Use",
                obj => obj.Type == ObjectType.Cache ? "quest" : "target",
                obj => obj.Type == ObjectType.Cache ? "Cache" : obj.Type.ToString());
        }

        private static MapObject ObjectAtTest(MapData map, int x, int y)
        {
            return map.Objects.FirstOrDefault(obj => obj != null && obj.X == x && obj.Y == y);
        }

        private static void SetTestTile(MapData map, int x, int y, int tile)
        {
            map.Tiles[y * map.Width + x] = tile;
        }

        private static void CombatCommandPresentationExposesSixCommands()
        {
            Type commandViewType = typeof(CombatHudCommandView);
            AssertEqual(true, commandViewType.GetField("IconTexture") != null, "combat command icon texture contract");
            AssertEqual(true, commandViewType.GetField("IconSource") != null, "combat command icon source contract");
            AssertEqual(true, typeof(CombatHudView).GetField("CommandPrompt") != null, "combat command prompt contract");
            AssertEqual(true, typeof(CombatHudView).GetField("TargetTitle") != null, "combat target card publishes its actual interaction title");
            AssertEqual(true, typeof(CombatHudView).GetField("TargetSourceLabel")?.FieldType == typeof(string), "combat target card publishes hover, nearest, suggested, or intent provenance");
            AssertEqual(true,
                typeof(CombatHudUnitView).GetField("PortraitTexture")?.FieldType == typeof(Texture2D)
                && typeof(CombatHudUnitView).GetField("PortraitSource")?.FieldType == typeof(Rect),
                "combat unit cards expose authored portrait atlas geometry");
            AssertEqual(true,
                typeof(CombatHudTurnView).GetField("PortraitTexture")?.FieldType == typeof(Texture2D)
                && typeof(CombatHudTurnView).GetField("PortraitSource")?.FieldType == typeof(Rect),
                "combat initiative chips expose authored portrait atlas geometry");

            CombatUnit ranger = new CombatUnit { ClassKey = "ranger", Role = "bow" };
            List<CombatCommandEntry> rangerCommands = CombatCommandPresentationRules.PrimaryCommandsFor(ranger).ToList();
            AssertEqual(6, rangerCommands.Count, "combat command count");
            AssertEqual("Move,Attack,Skills,Guard,Elixir,End Turn", string.Join(",", rangerCommands.Select(command => command.Label)), "martial combat command labels");
            AssertEqual("WASD,F,C,G,H,Space", string.Join(",", rangerCommands.Select(command => command.Hotkey)), "combat command hotkeys");
            AssertEqual(ActionMode.Ability, rangerCommands[2].Mode, "ranger ability entry opens martial skills");
            AssertEqual(ActionMode.Guard, rangerCommands[3].Mode, "guard remains a direct combat command");
            AssertEqual(ActionMode.Elixir, rangerCommands[4].Mode, "elixir remains a direct combat command");
            AssertEqual(ActionMode.Wait, rangerCommands[5].Mode, "end turn remains a direct combat command");

            CombatUnit mage = new CombatUnit { ClassKey = "mage", Role = "ember", Spell = "ember" };
            List<CombatCommandEntry> mageCommands = CombatCommandPresentationRules.PrimaryCommandsFor(mage).ToList();
            AssertEqual(ActionMode.Cast, mageCommands[2].Mode, "caster ability entry opens spellbook");
            AssertEqual("Spells", mageCommands[2].Label, "caster command names the spellbook directly");

            AssertEqual(false, CombatCommandPresentationRules.ShouldPromoteEndTurn(false, 0, false, false, false), "enemy turn does not promote player end turn");
            AssertEqual(false, CombatCommandPresentationRules.ShouldPromoteEndTurn(true, 3, true, false, false), "fresh turn does not promote end turn");
            AssertEqual(false, CombatCommandPresentationRules.ShouldPromoteEndTurn(true, 0, true, false, false), "available action prevents premature end-turn promotion");
            AssertEqual(false, CombatCommandPresentationRules.ShouldPromoteEndTurn(true, 2, false, false, false), "remaining movement prevents premature end-turn promotion");
            AssertEqual(true, CombatCommandPresentationRules.ShouldPromoteEndTurn(true, 0, false, false, false), "spent movement and action promote end turn");
            AssertEqual(true, CombatCommandPresentationRules.ShouldPromoteEndTurn(true, 3, true, true, false), "stunned promotes end turn");
        }

        private static void CombatInputRoutingKeepsHudFocusAuthoritative()
        {
            AssertEqual(
                false,
                CombatInputRoutingRules.ShouldRouteToWorld(true, CombatHotkeyKind.Navigation),
                "focused combat HUD owns WASD and arrow navigation");
            AssertEqual(
                false,
                CombatInputRoutingRules.ShouldRouteToWorld(true, CombatHotkeyKind.Submit),
                "focused combat HUD owns keyboard and controller submit");
            AssertEqual(
                true,
                CombatInputRoutingRules.ShouldRouteToWorld(true, CombatHotkeyKind.Dedicated),
                "focused combat HUD preserves dedicated combat hotkeys");
            AssertEqual(
                true,
                CombatInputRoutingRules.ShouldRouteToWorld(false, CombatHotkeyKind.Navigation),
                "unfocused combat HUD leaves world movement available");
            AssertEqual(
                true,
                CombatInputRoutingRules.ShouldRouteToWorld(false, CombatHotkeyKind.Submit),
                "unfocused combat HUD leaves the End Turn shortcut available");
        }

        private static void CombatTargetingCancellationIsExplicitAndSafe()
        {
            CombatState combat = new CombatState
            {
                ActionAvailable = true,
                Acted = false,
                Phase = CombatPhase.ChooseTarget
            };

            AssertEqual(true, CombatTargetingRules.CanCancel(combat, ActionMode.Cast, "FBL", ""), "armed spell targeting can cancel");
            AssertEqual(true, CombatTargetingRules.CanCancel(combat, ActionMode.Ability, "", "charge"), "armed skill targeting can cancel");
            AssertEqual(false, CombatTargetingRules.CanCancel(combat, ActionMode.Cast, "", ""), "empty spell selection has nothing to cancel");
            AssertEqual(false, CombatTargetingRules.CanCancel(combat, ActionMode.Attack, "", ""), "ordinary attack selection leaves Esc available for Menu");
            AssertEqual("Cancel Spell", CombatTargetingRules.CancelLabel(ActionMode.Cast), "spell cancellation label");
            AssertEqual("Cancel Skill", CombatTargetingRules.CancelLabel(ActionMode.Ability), "skill cancellation label");

            combat.ActionAvailable = false;
            AssertEqual(false, CombatTargetingRules.CanCancel(combat, ActionMode.Cast, "FBL", ""), "spent action cannot cancel stale targeting");
            combat.ActionAvailable = true;
            combat.Acted = true;
            AssertEqual(false, CombatTargetingRules.CanCancel(combat, ActionMode.Ability, "", "charge"), "acted unit cannot cancel stale targeting");
            combat.Acted = false;
            combat.Phase = CombatPhase.Resolving;
            AssertEqual(false, CombatTargetingRules.CanCancel(combat, ActionMode.Cast, "FBL", ""), "resolving power cannot be canceled");
        }

        private static void EnemyTacticsProfilesAreRoleAware()
        {
            foreach (string id in EnemyCatalog.Ids.Concat(new[] { "sentry" }))
            {
                CombatUnit enemy = TacticalEnemy(id);
                EnemyTacticsProfile profile = EnemyTacticsRules.For(enemy);
                AssertEqual(true, Enum.IsDefined(typeof(EnemyTacticsArchetype), profile.Archetype), id + " tactical archetype");
                AssertEqual(true, profile.PreferredRange >= 1, id + " preferred range");
                AssertEqual(true, !string.IsNullOrWhiteSpace(EnemyTacticsRules.StyleLabel(enemy)), id + " tactical label");
            }

            CombatUnit brute = TacticalEnemy("ratbrute");
            CombatUnit marksman = TacticalEnemy("drowcrossbow");
            CombatUnit caster = TacticalEnemy("glassmage");
            CombatUnit support = TacticalEnemy("ratcleric");
            CombatUnit boss = TacticalEnemy("koboldking");
            CombatUnit skirmisher = TacticalEnemy("koboldraider");

            AssertEqual(EnemyTacticsArchetype.Brute, EnemyTacticsRules.For(brute).Archetype, "rat brute tactical identity");
            AssertEqual(EnemyTacticsArchetype.Marksman, EnemyTacticsRules.For(marksman).Archetype, "crossbow tactical identity");
            AssertEqual(EnemyTacticsArchetype.Caster, EnemyTacticsRules.For(caster).Archetype, "glass mage tactical identity");
            AssertEqual(EnemyTacticsArchetype.Support, EnemyTacticsRules.For(support).Archetype, "rat cleric tactical identity");
            AssertEqual(EnemyTacticsArchetype.Boss, EnemyTacticsRules.For(boss).Archetype, "kobold king tactical identity");
            AssertEqual(EnemyTacticsArchetype.Skirmisher, EnemyTacticsRules.For(skirmisher).Archetype, "raider tactical identity");

            AssertEqual(true, EnemyTacticsRules.CanAttackAfterMove(brute, 1), "brute can step and strike");
            AssertEqual(true, EnemyTacticsRules.CanAttackAfterMove(marksman, 1), "marksman can step and shoot");
            AssertEqual(false, EnemyTacticsRules.CanAttackAfterMove(skirmisher, 1), "ordinary skirmisher must commit its move");
            skirmisher.Rank = "veteran";
            AssertEqual(true, EnemyTacticsRules.CanAttackAfterMove(skirmisher, 1), "veteran skirmisher can step and strike");
            AssertEqual(false, EnemyTacticsRules.CanAttackAfterMove(boss, 2), "no enemy attacks after a long move");

            int firingLane = EnemyTacticsRules.PositionAdjustment(marksman, marksman.Range, 0, true, false);
            int adjacentLane = EnemyTacticsRules.PositionAdjustment(marksman, 1, 0, true, false);
            AssertEqual(true, firingLane < adjacentLane, "marksman prefers a clear distant firing lane");
            AssertEqual(true, EnemyTacticsRules.TerrainRisk(brute, "fire", 0) < EnemyTacticsRules.TerrainRisk(marksman, "fire", 0), "brute accepts more hazard pressure than marksman");

            CombatUnit exposedMage = new CombatUnit { Hp = 20, MaxHp = 20, Defense = 1, ClassKey = "mage", Role = "ember", Spell = "ember" };
            CombatUnit armoredWarrior = new CombatUnit { Hp = 20, MaxHp = 20, Defense = 5, ArmorBonus = 3, ClassKey = "warrior", Role = "arms" };
            AssertEqual(true,
                EnemyTacticsRules.TargetPriorityAdjustment(marksman, exposedMage) < EnemyTacticsRules.TargetPriorityAdjustment(marksman, armoredWarrior),
                "marksman prefers the exposed caster");
            int visiblePriority = EnemyTacticsRules.TargetPriorityAdjustment(caster, exposedMage);
            exposedMage.Stealthed = 2;
            AssertEqual(true, EnemyTacticsRules.TargetPriorityAdjustment(caster, exposedMage) > visiblePriority, "stealth lowers enemy target priority");
            AssertEqual(25, VersionInfo.SaveVersion, "current save schema persists weapon enchantments and the route waypoint");
        }

        private static CombatUnit TacticalEnemy(string id, string rank = "")
        {
            EnemyTemplate template = EnemyCatalog.For(id);
            return new CombatUnit
            {
                Id = id,
                Side = UnitSide.Enemy,
                Name = template.Name,
                Role = id,
                Rank = rank,
                Hp = template.Hp,
                MaxHp = template.Hp,
                Power = template.Power,
                Defense = template.Defense,
                Agility = template.Agility,
                Range = template.Range,
                DamageType = template.DamageType,
                Resist = template.Resist,
                Weakness = template.Weakness
            };
        }

        private static void CombatThreatForecastsAreSharedAndReadable()
        {
            CombatAttackForecast blocked = CombatThreatRules.Create(
                AttackForecastBlockReason.OutOfRange,
                true,
                true,
                true,
                6,
                4,
                75,
                5,
                9,
                "physical",
                "normal",
                false,
                24,
                24);
            AssertEqual(false, blocked.Legal, "out-of-range forecast is illegal");
            AssertEqual(false, blocked.HasOutcome, "blocked forecast does not publish fake damage");
            AssertEqual(CombatThreatLevel.None, blocked.ThreatLevel, "blocked forecast has no direct threat level");
            AssertEqual("out of range", CombatThreatRules.BlockLabel(blocked.BlockReason), "blocked forecast has readable reason");

            CombatAttackForecast direct = CombatThreatRules.Create(
                AttackForecastBlockReason.None,
                true,
                false,
                true,
                1,
                1,
                70,
                4,
                8,
                "physical",
                "normal",
                false,
                30,
                30);
            AssertEqual(true, direct.Legal && direct.HasOutcome, "legal forecast exposes an outcome");
            AssertEqual(4, direct.ExpectedDamage, "forecast expected damage uses hit chance and average damage");
            AssertEqual(CombatThreatLevel.Direct, direct.ThreatLevel, "ordinary attack is direct threat");

            CombatAttackForecast severe = CombatThreatRules.Create(
                AttackForecastBlockReason.None,
                true,
                true,
                true,
                4,
                5,
                80,
                6,
                10,
                "cold",
                "weak",
                true,
                18,
                30);
            AssertEqual(CombatThreatLevel.Severe, severe.ThreatLevel, "half-health damage forecast is severe");
            AssertEqual("HIGH THREAT", CombatThreatRules.SeverityLabel(severe.ThreatLevel), "severe threat label");

            CombatAttackForecast lethal = CombatThreatRules.Create(
                AttackForecastBlockReason.None,
                true,
                false,
                true,
                1,
                1,
                65,
                5,
                8,
                "death",
                "normal",
                false,
                7,
                30);
            AssertEqual(CombatThreatLevel.Lethal, lethal.ThreatLevel, "maximum lethal damage is telegraphed");
            AssertEqual("LETHAL", CombatThreatRules.SeverityLabel(lethal.ThreatLevel), "lethal threat label");

            CombatAttackForecast legalityOnly = CombatThreatRules.Create(
                AttackForecastBlockReason.None,
                false,
                false,
                true,
                1,
                1,
                0,
                0,
                0,
                "physical",
                "normal",
                false,
                20,
                20);
            AssertEqual(true, legalityOnly.Legal && !legalityOnly.HasOutcome, "AI legality probe avoids outcome work");
            AssertEqual(CombatThreatLevel.Direct, legalityOnly.ThreatLevel, "legal probe still reports direct reach");
            AssertEqual("safe", CombatThreatRules.MovementDestinationLabel(0, 0), "safe movement destination is explicit");
            AssertEqual("threat: 1 can hit", CombatThreatRules.MovementDestinationLabel(1, 0), "one direct movement threat is explicit");
            AssertEqual("threat: 2 can reach", CombatThreatRules.MovementDestinationLabel(0, 2), "closing movement threats are distinguished from direct attacks");
            AssertEqual("threat: 1 can hit + 2 can reach", CombatThreatRules.MovementDestinationLabel(1, 2), "mixed destination threats retain both responsible groups");
            AssertEqual(25, VersionInfo.SaveVersion, "current save schema persists weapon enchantments and the route waypoint");
        }

        private static void CombatControllerOwnsTurnAndActionLifecycle()
        {
            GameState state = TestCombatState(out CombatUnit hero, out CombatUnit enemy);
            CombatController controller = TestCombatController(state);

            controller.BeginTurn(hero, false);
            AssertEqual(hero.Id, state.Combat.ActiveId, "begin turn active id");
            AssertEqual(3, state.Combat.MovePoints, "begin turn movement");
            AssertEqual(true, state.Combat.ActionAvailable, "begin turn action");

            state.Combat.Phase = CombatPhase.Resolving;
            AssertEqual(false, controller.ActionEnabled(ActionMode.Move, hero, false, false, 0), "resolving phase blocks commands");
            state.Elixirs = 2;
            int resolvingResolverCalls = 0;
            int resolvingHeroHp = hero.Hp;
            int resolvingEnemyHp = enemy.Hp;
            int resolvingMovePoints = state.Combat.MovePoints;
            CombatCommandResult resolvingMove = controller.TryMove(hero, 2, 1);
            AssertEqual(false, resolvingMove.Success, "resolving phase blocks movement");
            AssertEqual(CombatCommandFailure.ActionUnavailable, resolvingMove.Failure, "resolving movement failure");
            CombatCommandResult resolvingUndo = controller.TryUndoMove(hero);
            AssertEqual(false, resolvingUndo.Success, "resolving phase blocks movement undo");
            AssertEqual(CombatCommandFailure.ActionUnavailable, resolvingUndo.Failure, "resolving undo failure");
            CombatCommandResult resolvingAttack = controller.TryAttack(hero, enemy, (actor, target) =>
            {
                resolvingResolverCalls++;
                return true;
            });
            CombatCommandResult resolvingAbility = controller.TryUseAbility(hero, () =>
            {
                resolvingResolverCalls++;
                return true;
            });
            CombatCommandResult resolvingAction = controller.TryResolveAction(hero, () =>
            {
                resolvingResolverCalls++;
                return true;
            });
            CombatCommandResult resolvingGuard = controller.Guard(hero, 4);
            CombatCommandResult resolvingItem = controller.TryUseItem(hero, 18, 6);
            CombatCommandResult resolvingEndTurn = controller.EndTurn(hero);
            AssertEqual(false, resolvingAttack.Success, "resolving phase blocks attacks");
            AssertEqual(false, resolvingAbility.Success, "resolving phase blocks abilities");
            AssertEqual(false, resolvingAction.Success, "resolving phase blocks generic actions");
            AssertEqual(false, resolvingGuard.Success, "resolving phase blocks Guard");
            AssertEqual(false, resolvingItem.Success, "resolving phase blocks elixirs");
            AssertEqual(false, resolvingEndTurn.Success, "resolving phase blocks End Turn");
            AssertEqual(0, resolvingResolverCalls, "resolving command gate never invokes action resolvers");
            AssertEqual(resolvingHeroHp, hero.Hp, "resolving command gate preserves actor health");
            AssertEqual(resolvingEnemyHp, enemy.Hp, "resolving command gate preserves target health");
            AssertEqual(2, state.Elixirs, "resolving command gate preserves resources");
            AssertEqual(resolvingMovePoints, state.Combat.MovePoints, "resolving command gate preserves movement");
            AssertEqual(false, state.Combat.Acted, "resolving command gate preserves acted state");
            AssertEqual(true, state.Combat.ActionAvailable, "resolving command gate preserves action state");
            AssertEqual(CombatPhase.Resolving, state.Combat.Phase, "resolving command gate preserves the owning phase");
            state.Combat.Phase = CombatPhase.ChooseAction;

            hero.Hp = 0;
            int defeatedResolverCalls = 0;
            CombatCommandResult defeatedAttack = controller.TryAttack(hero, enemy, (actor, target) =>
            {
                defeatedResolverCalls++;
                return true;
            });
            AssertEqual(false, controller.ActionEnabled(ActionMode.Attack, hero, false, false, state.Elixirs), "defeated active unit has no enabled commands");
            AssertEqual(false, defeatedAttack.Success, "defeated active unit cannot attack");
            AssertEqual(0, defeatedResolverCalls, "defeated active unit cannot invoke a resolver");
            AssertEqual(false, controller.TryMove(hero, 2, 1).Success, "defeated active unit cannot move");
            AssertEqual(false, controller.EndTurn(hero).Success, "defeated active unit cannot end the turn");
            hero.Hp = hero.MaxHp;

            CombatCommandResult move = controller.TryMove(hero, 2, 1);
            AssertEqual(true, move.Success, "move succeeds");
            AssertEqual(2, hero.X, "move updates x");
            AssertEqual(1, hero.Y, "move updates y");
            AssertEqual(2, state.Combat.MovePoints, "move spends distance");
            AssertEqual(true, state.Combat.ActionAvailable, "move keeps action");
            AssertEqual(true, controller.CanUndoMove(hero), "uncommitted movement can be undone");

            CombatCommandResult undoMove = controller.TryUndoMove(hero);
            AssertEqual(true, undoMove.Success, "movement undo succeeds");
            AssertEqual(1, hero.X, "movement undo restores x");
            AssertEqual(1, hero.Y, "movement undo restores y");
            AssertEqual(3, state.Combat.MovePoints, "movement undo restores the turn budget");
            AssertEqual(false, state.Combat.Moved, "movement undo restores the unmoved stance");
            AssertEqual(true, state.Combat.ActionAvailable, "movement undo preserves the action");
            AssertEqual(false, controller.CanUndoMove(hero), "fresh origin has nothing left to undo");

            move = controller.TryMove(hero, 2, 1);
            AssertEqual(true, move.Success, "unit can move again after undo");

            CombatCommandResult attack = controller.TryAttack(hero, enemy, (actor, target) =>
            {
                target.Hp -= 3;
                return true;
            });
            AssertEqual(true, attack.Success, "attack succeeds");
            AssertEqual(9, enemy.Hp, "attack resolver ran");
            AssertEqual(false, state.Combat.ActionAvailable, "attack spends action");
            AssertEqual(true, state.Combat.Acted, "attack marks acted");
            AssertEqual(CombatPhase.Resolving, state.Combat.Phase, "attack resolving phase");
            AssertEqual(false, controller.CanUndoMove(hero), "spent action locks movement undo");
            AssertEqual(CombatCommandFailure.ActionUnavailable, controller.TryUndoMove(hero).Failure, "spent action reports the authoritative resolving-phase rejection");

            CombatCommandResult rejected = controller.TryAttack(hero, enemy, (actor, target) => true);
            AssertEqual(false, rejected.Success, "second attack rejected");
            AssertEqual(CombatCommandFailure.ActionUnavailable, rejected.Failure, "second attack failure");

            controller.BeginTurn(hero, false);
            int resolverCalls = 0;
            CombatCommandResult rejectedResolution = controller.TryResolveAction(hero, () =>
            {
                resolverCalls++;
                return false;
            });
            AssertEqual(false, rejectedResolution.Success, "rejected generic action reports failure");
            AssertEqual(1, resolverCalls, "legal generic action invokes its resolver once");
            AssertEqual(true, state.Combat.ActionAvailable, "rejected resolver preserves the action");
            AssertEqual(false, state.Combat.Acted, "rejected resolver preserves unspent action state");
            AssertEqual(CombatPhase.ChooseAction, state.Combat.Phase, "rejected resolver preserves the command phase");

            CombatCommandResult acceptedResolution = controller.TryResolveAction(hero, () => true);
            AssertEqual(true, acceptedResolution.Success, "accepted generic action succeeds");
            AssertEqual(false, state.Combat.ActionAvailable, "accepted generic action spends the action once");
            AssertEqual(true, state.Combat.Acted, "accepted generic action marks acted");
            AssertEqual(CombatPhase.Resolving, state.Combat.Phase, "accepted generic action enters resolution");

            controller.BeginTurn(hero, false);
            CombatCommandResult wrongUnit = controller.TryAttack(enemy, hero, (actor, target) => true);
            AssertEqual(false, wrongUnit.Success, "non-active unit rejected");
            AssertEqual(CombatCommandFailure.NotActiveUnit, wrongUnit.Failure, "non-active unit failure");

            CombatCommandResult guard = controller.Guard(hero, 4);
            AssertEqual(true, guard.Success, "guard succeeds");
            AssertEqual(true, hero.Guarding, "guard flag");
            AssertEqual(4, hero.GuardBonus, "guard bonus");
            AssertEqual(0, state.Combat.MovePoints, "guard ends movement");
            AssertEqual(false, state.Combat.ActionAvailable, "guard spends action");

            controller.BeginTurn(hero, false);
            state.Elixirs = 1;
            hero.Hp = 5;
            hero.Mana = 1;
            CombatCommandResult item = controller.TryUseItem(hero, 18, 6);
            AssertEqual(true, item.Success, "elixir succeeds");
            AssertEqual(0, state.Elixirs, "elixir consumed");
            AssertEqual(20, hero.Hp, "elixir heals to cap");
            AssertEqual(7, hero.Mana, "elixir restores mana");
            AssertEqual(false, state.Combat.ActionAvailable, "elixir spends action");

            hero.Stunned = 1;
            controller.BeginTurn(hero, false);
            AssertEqual(false, state.Combat.ActionAvailable, "stunned cannot act");
            AssertEqual(true, controller.ActionEnabled(ActionMode.Wait, hero, false, false, 0), "stunned can wait");
            CombatCommandResult stunnedAttack = controller.TryAttack(hero, enemy, (actor, target) => true);
            AssertEqual(false, stunnedAttack.Success, "stunned attack rejected");
            AssertEqual(CombatCommandFailure.ActionUnavailable, stunnedAttack.Failure, "stunned failure");
            AssertEqual(true, controller.EndTurn(hero).Success, "stunned unit can end turn");
            hero.Stunned = 0;

            hero.Sleeping = 1;
            controller.BeginTurn(hero, false);
            state.Combat.Phase = CombatPhase.ChooseTarget;
            AssertEqual(true, controller.EndTurn(hero).Success, "sleeping unit can end turn from a player command phase");
            hero.Sleeping = 0;

            controller.BeginTurn(hero, false);
            state.Combat.Acted = true;
            state.Combat.ActionAvailable = false;
            state.Combat.Phase = CombatPhase.ChooseTarget;
            AssertEqual(true, controller.EndTurn(hero).Success, "action-spent living unit can end turn from a player command phase");

            state.Combat.Acted = false;
            state.Combat.Moved = false;
            state.Combat.MovePoints = 0;
            state.Combat.ActionAvailable = false;
            state.Combat.Phase = CombatPhase.Resolving;
            controller.RepairActiveTurnState(hero, false);
            AssertEqual(3, state.Combat.MovePoints, "turn repair refills unspent movement");
            AssertEqual(true, state.Combat.ActionAvailable, "turn repair restores unspent action");
            AssertEqual(CombatPhase.ChooseAction, state.Combat.Phase, "turn repair chooses player action phase");

            state.Combat.Acted = true;
            state.Combat.Moved = false;
            state.Combat.MovePoints = 9;
            state.Combat.ActionAvailable = true;
            controller.RepairActiveTurnState(hero, false);
            AssertEqual(3, state.Combat.MovePoints, "turn repair clamps movement to allowance");
            AssertEqual(false, state.Combat.ActionAvailable, "turn repair preserves spent action");

            state.Combat.Acted = false;
            state.Combat.Moved = true;
            state.Combat.MovePoints = 0;
            controller.RepairActiveTurnState(hero, false);
            AssertEqual(0, state.Combat.MovePoints, "turn repair does not refill moved unit");

            hero.Webbed = 2;
            controller.BeginTurn(hero, false);
            AssertEqual(0, state.Combat.MovePoints, "webbed turn snapshots zero movement before automatic effects");
            hero.Webbed = 0;
            controller.RepairActiveTurnState(hero, false);
            AssertEqual(3, state.Combat.MovePoints, "turn repair restores unspent movement when start-turn effects clear web");
            int repairedOriginX = hero.X;
            int repairedOriginY = hero.Y;
            int repairedDestinationX = hero.X + 1;
            CombatCommandResult repairedMove = controller.TryMove(hero, repairedDestinationX, hero.Y);
            AssertEqual(true, repairedMove.Success, "unit can move after start-turn web recovery");
            CombatCommandResult repairedUndo = controller.TryUndoMove(hero);
            AssertEqual(true, repairedUndo.Success, "web-recovered movement can be undone");
            AssertEqual(repairedOriginX, hero.X, "web-recovered undo restores origin x");
            AssertEqual(repairedOriginY, hero.Y, "web-recovered undo restores origin y");
            AssertEqual(3, state.Combat.MovePoints, "web-recovered undo restores the refreshed full budget");
            AssertEqual(true, controller.TryMove(hero, repairedDestinationX, hero.Y).Success, "unit can move again after web-recovered undo");
            AssertEqual(true, controller.TryUndoMove(hero).Success, "second web-recovered move remains reversible");

            controller.BeginTurn(enemy, true);
            controller.RepairActiveTurnState(enemy, true);
            AssertEqual(CombatPhase.EnemyThinking, state.Combat.Phase, "turn repair chooses enemy thinking phase");

            CombatUnit summon = new CombatUnit { Id = "summon", Side = UnitSide.Party, Name = "Imp", X = 3, Y = 3, Hp = 5, MaxHp = 5, Summoned = true, SummonTurns = 1 };
            state.Combat.Units.Add(summon);
            SummonBindingResult summonResult = controller.TickSummonBindingEndOfTurn(summon);
            AssertEqual(true, summonResult.Ticked, "summon binding ticked");
            AssertEqual(true, summonResult.Expired, "summon binding expired");
            AssertEqual(0, summon.Hp, "expired summon removed");

            enemy.Hp = 0;
            hero.Hp = 20;
            AssertEqual(CombatOutcome.Victory, controller.CurrentOutcome(), "victory outcome");
            enemy.Hp = 12;
            hero.Hp = 0;
            AssertEqual(CombatOutcome.Defeat, controller.CurrentOutcome(), "defeat outcome");
        }

        private static void CombatRitualsHaveCounterplayAndOutcomeWeight()
        {
            Point glyph = new Point(4, 2, "glyph", 3);
            Point rift = new Point(5, 2, "demonrift", 4);
            AssertEqual(true, CombatRitualRules.IsRitual(glyph), "glyph is a combat ritual");
            AssertEqual(2, glyph.Integrity, "glyph initializes ritual integrity");
            AssertEqual(3, rift.Integrity, "demon rift initializes ritual integrity");
            AssertEqual("koboldraider", CombatRitualRules.SpawnRole(glyph.Kind), "glyph reinforcement role");
            AssertEqual("lesserdemon", CombatRitualRules.SpawnRole(rift.Kind), "demon rift reinforcement role");
            AssertEqual(true, CombatRitualRules.IsDispelableField(new Point(2, 2, "gas", 2)), "Rift Seal accepts hostile fields");

            CombatUnit axeFighter = new CombatUnit { Power = 10, WeaponName = "heavy iron axe", DamageType = "physical" };
            CombatUnit archer = new CombatUnit { Power = 10, WeaponName = "ashwood longbow", DamageType = "physical" };
            AssertEqual(true, CombatRitualRules.PhysicalDisruptionDamage(axeFighter, false) > CombatRitualRules.PhysicalDisruptionDamage(archer, true), "heavy melee breaks rituals faster than ranged attacks");

            GameState state = TestCombatState(out CombatUnit hero, out CombatUnit enemy);
            CombatController controller = TestCombatController(state);
            enemy.Hp = 0;
            state.Combat.Obstacles.Add(glyph);
            AssertEqual(CombatOutcome.Ongoing, controller.CurrentOutcome(), "unresolved ritual prevents premature victory");
            state.Combat.Obstacles.Clear();
            AssertEqual(CombatOutcome.Victory, controller.CurrentOutcome(), "victory returns after ritual is removed");

            FormulaDef seal = FormulaCatalog.All.First(formula => formula.Code == "SRF");
            CombatPowerTargetingProfile targeting = CombatPowerTargetingRules.ForFormula(seal);
            AssertEqual(CombatPowerFootprintKind.Placement, targeting.Kind, "Rift Seal targets a field tile");
            AssertEqual("SEAL", targeting.BoardLabel, "Rift Seal board label");
            AssertEqual(true, CombatIconCatalog.SignatureSpellIndex(seal.Code) >= 0, "Rift Seal has spellbook icon art");
            AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, seal.Code), "Rift Seal is learnable in the first campaign slice");
        }

        private static void RuntimeControllersAreCachedAccessors()
        {
            string legacyRoot = Path.Combine(Application.dataPath, "Scripts", "Legacy");
            string exploreSource = File.ReadAllText(Path.Combine(legacyRoot, "AshenHallsGame.Explore.cs"));
            string combatSource = File.ReadAllText(Path.Combine(legacyRoot, "AshenHallsGame.Combat.cs"));
            string coreSource = File.ReadAllText(Path.Combine(legacyRoot, "AshenHallsGame.Core.cs"));
            string tavernSource = File.ReadAllText(Path.Combine(legacyRoot, "AshenHallsGame.Tavern.cs"));

            AssertEqual(false, exploreSource.Contains("CreateExplorationController"), "exploration factory path removed");
            AssertEqual(1, CountOccurrences(exploreSource, "new ExplorationController("), "exploration controller is constructed by one cached accessor");
            AssertEqual(true, exploreSource.Contains("ReferenceEquals(explorationControllerState, state)"), "exploration controller cache keys current state");
            AssertEqual(true, exploreSource.Contains("ReferenceEquals(explorationControllerMap, map)"), "exploration controller cache keys current map");

            AssertEqual(1, CountOccurrences(combatSource, "new CombatController("), "combat controller is constructed by one cached accessor");
            AssertEqual(true, combatSource.Contains("ReferenceEquals(combatControllerState, state)"), "combat controller cache keys current state");
            AssertEqual(true, combatSource.Contains("ReferenceEquals(combatControllerCombat, combat)"), "combat controller cache keys combat session");

            AssertEqual(true, coreSource.Contains("private ExplorationController explorationController;"), "exploration controller cache field exists");
            AssertEqual(true, coreSource.Contains("private CombatController combatController;"), "combat controller cache field exists");
            AssertEqual(true, CountOccurrences(coreSource + exploreSource + combatSource + tavernSource, "InvalidateControllerCaches(") >= 5, "state/map transitions invalidate controller caches");
        }

        private static GameState TestCombatState(out CombatUnit hero, out CombatUnit enemy)
        {
            hero = new CombatUnit
            {
                Id = "hero",
                Side = UnitSide.Party,
                Name = "Tester",
                X = 1,
                Y = 1,
                Hp = 20,
                MaxHp = 20,
                Mana = 1,
                MaxMana = 10,
                Movement = 3
            };
            enemy = new CombatUnit
            {
                Id = "enemy",
                Side = UnitSide.Enemy,
                Name = "Target",
                X = 4,
                Y = 1,
                Hp = 12,
                MaxHp = 12,
                Movement = 3
            };

            GameState state = new GameState
            {
                Combat = new CombatState
                {
                    ActiveId = hero.Id,
                    MovePoints = 3,
                    ActionAvailable = true,
                    Units = new List<CombatUnit> { hero, enemy }
                }
            };
            return state;
        }

        private static CombatController TestCombatController(GameState state)
        {
            return new CombatController(
                state,
                999,
                unit => unit?.Movement > 0 ? unit.Movement : 3,
                (unit, x, y) => TestCombatTileOpen(state, unit, x, y),
                (unit, x, y) => unit == null ? 999 : Math.Abs(unit.X - x) + Math.Abs(unit.Y - y),
                unit => unit != null && unit.Side == UnitSide.Party);
        }

        private static bool TestCombatTileOpen(GameState state, CombatUnit active, int x, int y)
        {
            if (state?.Combat?.Units == null) return false;
            if (x < 0 || x >= 12 || y < 0 || y >= 8) return false;
            return !state.Combat.Units.Any(unit => unit != null && unit != active && unit.Hp > 0 && unit.X == x && unit.Y == y);
        }

        private static void EncounterCatalogDefinesExplicitValidEncounters()
        {
            List<EncounterDefinition> encounters = EncounterCatalog.All.ToList();
            AssertEqual(Enum.GetValues(typeof(EncounterId)).Length, encounters.Count, "encounter definition count");
            AssertEqual(encounters.Count, encounters.Select(encounter => encounter.Id).Distinct().Count(), "unique encounter ids");
            AssertEqual(encounters.Count, encounters.Select(encounter => encounter.LegacyStyle ?? "").Distinct().Count(), "unique encounter styles");

            foreach (EncounterDefinition encounter in encounters)
            {
                AssertEqual(encounter.Id, EncounterCatalog.For(encounter.Id).Id, encounter.Id + " lookup");
                AssertEqual(encounter.Id, EncounterCatalog.IdForLegacyStyle(encounter.LegacyStyle), encounter.Id + " legacy style lookup");
                AssertEqual(false, string.IsNullOrWhiteSpace(encounter.Banner), encounter.Id + " banner");
                AssertEqual(false, string.IsNullOrWhiteSpace(encounter.Intro), encounter.Id + " intro");
                AssertEqual(true, encounter.EnemyCountForDepth(1) >= 3 || !encounter.UsesGeneratedEnemyPool && encounter.EnemyCountForDepth(1) == encounter.EnemyIds.Length, encounter.Id + " enemy count");

                if (encounter.EnemyIds != null)
                {
                    foreach (string enemyId in encounter.EnemyIds)
                    {
                        AssertEqual(true, EncounterCatalog.IsKnownEnemyId(enemyId), encounter.Id + " known enemy " + enemyId);
                    }
                }

                AssertPointsOnBoard(encounter.Id + " enemy placement", encounter.EnemyPlacements);
                AssertPointsOnBoard(encounter.Id + " party placement", encounter.PartyPlacements);
                AssertPointsOnBoard(encounter.Id + " obstacle", encounter.Obstacles);
                AssertNoDuplicatePoints(encounter.Id + " enemy placement", encounter.EnemyPlacements);
                AssertNoDuplicatePoints(encounter.Id + " party placement", encounter.PartyPlacements);
                AssertNoDuplicatePoints(encounter.Id + " obstacle", encounter.Obstacles);
                AssertNoFixedPlacementOverlap(encounter);
            }

            AssertThrows<ArgumentException>(() => EncounterCatalog.For((EncounterId)999), "unknown encounter id throws");
            AssertThrows<ArgumentException>(() => EncounterCatalog.IdForLegacyStyle("not-a-real-encounter"), "unknown encounter style throws");
        }

        private static void SewerSliceContentSetDefinesCompleteFirstPlayPath()
        {
            AssertEqual(16, ContentSetCatalog.SewerSliceFormulaCodes.Count, "sewer slice formula count");
            AssertEqual(14, ContentSetCatalog.SewerSliceAbilityIds.Count, "sewer slice permanent and derived ability count");
            AssertEqual(6, ContentSetCatalog.SewerSliceEnemyIds.Count, "sewer slice enemy count");
            AssertEqual(3, ContentSetCatalog.SewerSliceEncounters.Count, "sewer slice encounter count");

            foreach (string code in ContentSetCatalog.SewerSliceFormulaCodes)
            {
                AssertEqual(true, FormulaCatalog.All.Any(formula => formula.Code == code), "slice formula exists " + code);
                AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, code), "slice formula active " + code);
            }
            AssertEqual(false, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, "MTR"), "meteor shower hidden in sewer slice");
            AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.FullPrototype, "MTR"), "meteor shower available in prototype");
            AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, "RKW"), "warlock bind active in sewer slice");
            AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, "IBD"), "warlock summon imp active in sewer slice");
            AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, "RBT"), "warlock Rift Bolt active in sewer slice");
            AssertEqual(false, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, "VRS"), "later Rift Step remains outside the sewer slice");
            AssertEqual(
                "RIG,RSG,CLT,VST,AST",
                string.Join(",", ContentSetCatalog.SewerSliceFormulaCodes.Where(code => new[] { "RIG", "RSG", "CLT", "VST", "AST" }.Contains(code))),
                "sewer slice exposes the complete lightning progression in tier order");

            foreach (string id in ContentSetCatalog.SewerSliceAbilityIds)
            {
                AssertEqual(true, AbilityCatalog.For(id) != null, "slice ability exists " + id);
                AssertEqual(true, ContentSetCatalog.AbilityActive(ContentSetCatalog.SewerSlice, id), "slice ability active " + id);
            }
            AssertEqual(false, ContentSetCatalog.AbilityActive(ContentSetCatalog.SewerSlice, "whirlwind"), "whirlwind hidden in sewer slice");
            AssertEqual(true, ContentSetCatalog.AbilityActive(ContentSetCatalog.FullPrototype, "whirlwind"), "whirlwind available in prototype");
            AssertEqual(true, ContentSetCatalog.AbilityActive(ContentSetCatalog.SewerSlice, "execute"), "warrior execute active in sewer slice");
            AssertEqual(true, ContentSetCatalog.AbilityActive(ContentSetCatalog.SewerSlice, "stealth"), "rogue stealth active in sewer slice");
            AssertEqual(true, ContentSetCatalog.AbilityActive(ContentSetCatalog.SewerSlice, "ambush"), "rogue ambush active in sewer slice");
            AssertEqual(true, ContentSetCatalog.AbilityActive(ContentSetCatalog.SewerSlice, "shieldbash"), "Shield Bash tactical push active in sewer slice");
            AssertEqual(true, ContentSetCatalog.AbilityActive(ContentSetCatalog.SewerSlice, "smokebomb"), "Smoke Bomb sight-control field active in sewer slice");
            AssertEqual(true, ContentSetCatalog.AbilityActive(ContentSetCatalog.SewerSlice, "scoutmark"), "Scout Mark guard break active in sewer slice");
            AssertEqual(true, AbilityCatalog.IdsForClass("demon").All(id => ContentSetCatalog.AbilityActive(ContentSetCatalog.SewerSlice, id)), "derived Demon Arts remain available when a sewer-slice warlock transforms");
            MartialAbility smokeBomb = AbilityCatalog.For("smokebomb");
            AssertEqual(true, smokeBomb.Summary.Contains("sight-blocking")
                && smokeBomb.Detail.Contains("does not block movement or poison units")
                && smokeBomb.Detail.Contains("cannot see through"), "Smoke Bomb catalog copy states its immediate sight-control rules");
            AssertEqual(false, smokeBomb.Summary.Contains("poison") || smokeBomb.Detail.Contains("poison hazard"), "Smoke Bomb never presents itself as poison gas");

            foreach (string id in ContentSetCatalog.SewerSliceEnemyIds)
            {
                AssertEqual(true, EncounterCatalog.IsKnownEnemyId(id), "slice enemy known " + id);
                AssertEqual(true, !string.IsNullOrWhiteSpace(EnemyCatalog.For(id).Name), "slice enemy has name " + id);
            }

            foreach (EncounterDefinition encounter in ContentSetCatalog.SewerSliceEncounters)
            {
                AssertEqual(false, encounter.UsesGeneratedEnemyPool, encounter.LegacyStyle + " is authored");
                AssertEqual(false, string.IsNullOrWhiteSpace(encounter.Banner), encounter.LegacyStyle + " banner");
                AssertEqual(false, string.IsNullOrWhiteSpace(encounter.Intro), encounter.LegacyStyle + " intro");
                AssertEqual(true, encounter.EnemyIds.All(id => ContentSetCatalog.EnemyActive(ContentSetCatalog.SewerSlice, id)), encounter.LegacyStyle + " uses slice enemies");
                AssertPointsOnBoard(encounter.LegacyStyle + " enemy placement", encounter.EnemyPlacements);
                AssertPointsOnBoard(encounter.LegacyStyle + " party placement", encounter.PartyPlacements);
                AssertPointsOnBoard(encounter.LegacyStyle + " obstacle", encounter.Obstacles);
                AssertNoDuplicatePoints(encounter.LegacyStyle + " enemy placement", encounter.EnemyPlacements);
                AssertNoDuplicatePoints(encounter.LegacyStyle + " party placement", encounter.PartyPlacements);
                AssertNoDuplicatePoints(encounter.LegacyStyle + " obstacle", encounter.Obstacles);
                AssertNoFixedPlacementOverlap(encounter);
            }

            List<string> flags = new List<string>();
            AssertEqual(false, ContentSetCatalog.AllowKoboldChapter(ContentSetCatalog.SewerSlice, flags), "sewer slice keeps the kobold chapter locked before the old road teaser");
            AssertEqual(true, ContentSetCatalog.AllowKoboldChapter(ContentSetCatalog.FullPrototype, null), "full prototype keeps the kobold chapter available without campaign flags");
            ContentSetCatalog.MarkSewerSliceContractAccepted(flags);
            AssertEqual(true, flags.Contains(StoryFlags.MidgaardRatQuestGiven), "sewer contract sets legacy quest flag");
            AssertEqual(true, flags.Contains(StoryFlags.SewerContractAccepted), "sewer contract flag");
            AssertEqual("sewer_broken_sluice", ContentSetCatalog.SewerSliceEncounterForProgress(ContentSetCatalog.SewerSliceClearedCount(flags)).LegacyStyle, "first sewer room");

            ContentSetCatalog.MarkSewerSliceEncounterCleared(flags, "sewer_broken_sluice");
            AssertEqual(1, ContentSetCatalog.SewerSliceClearedCount(flags), "broken sluice cleared count");
            AssertEqual("sewer_foul_runoff", ContentSetCatalog.SewerSliceEncounterForProgress(ContentSetCatalog.SewerSliceClearedCount(flags)).LegacyStyle, "second sewer room");

            ContentSetCatalog.MarkSewerSliceEncounterCleared(flags, "sewer_foul_runoff");
            AssertEqual(2, ContentSetCatalog.SewerSliceClearedCount(flags), "foul runoff cleared count");
            AssertEqual("sewer_cistern_den", ContentSetCatalog.SewerSliceEncounterForProgress(ContentSetCatalog.SewerSliceClearedCount(flags)).LegacyStyle, "third sewer room");

            ContentSetCatalog.MarkSewerSliceEncounterCleared(flags, "sewer_cistern_den");
            AssertEqual(3, ContentSetCatalog.SewerSliceClearedCount(flags), "cistern den cleared count");
            AssertEqual(true, flags.Contains(StoryFlags.MidgaardRatPeltsCollected), "final sewer room creates proof");
            AssertEqual(true, ContentSetCatalog.SewerSliceRewardReady(flags), "reward ready after final room");
            AssertEqual(false, ContentSetCatalog.AllowKoboldChapter(ContentSetCatalog.SewerSlice, flags), "clearing the sewer alone does not unlock the kobold chapter");

            InventoryItem renamedProof = ContentSetCatalog.CreateSewerSliceProof();
            renamedProof.DisplayName = "proof token with tester-facing display text";
            List<InventoryItem> inventory = new List<InventoryItem>
            {
                renamedProof,
                ContentSetCatalog.CreateSewerSliceProof(),
                new InventoryItem { Slot = "quest", Trait = "quest", Material = "rat pelt", DisplayName = "rat pelt" }
            };
            AssertEqual(2, ContentSetCatalog.CountSewerSliceProof(inventory), "sewer proof count ignores display names");
            ContentSetCatalog.RemoveSewerSliceProof(inventory, 1);
            AssertEqual(1, ContentSetCatalog.CountSewerSliceProof(inventory), "sewer proof removal uses stable fields");

            InventoryItem reward = ContentSetCatalog.CreateSewerSliceReward();
            AssertEqual("armor", reward.Slot, "sewer reward slot");
            AssertEqual(3, reward.Bonus, "sewer reward armor bonus");
            AssertEqual(1, reward.AgilityBonus, "sewer reward agility bonus");
            AssertEqual(1, reward.HealthBonus, "sewer reward health bonus");
            AssertEqual("quest", reward.Rarity, "sewer reward rarity");

            InventoryItem safeBlade = ContentSetCatalog.CreateSewerSafeRoomBlade();
            InventoryItem safeFocus = ContentSetCatalog.CreateSewerSafeRoomFocus();
            AssertEqual("weapon", safeBlade.Slot, "safe-room blade is equipment");
            AssertEqual(2, safeBlade.Bonus, "safe-room blade bonus");
            AssertEqual(1, safeBlade.StrengthBonus, "safe-room blade supports front line");
            AssertEqual("physical", safeBlade.DamageType, "safe-room blade damage type");
            AssertEqual("weapon", safeFocus.Slot, "safe-room focus is equipment");
            AssertEqual(2, safeFocus.Bonus, "safe-room focus bonus");
            AssertEqual(1, safeFocus.IntelligenceBonus, "safe-room focus supports casters");
            AssertEqual("shock", safeFocus.DamageType, "safe-room focus damage type");
            AssertEqual(false, safeBlade.DisplayName == safeFocus.DisplayName, "safe-room choices have distinct identities");

            ContentSetCatalog.MarkSewerSafeRoomChoice(flags, "focus");
            AssertEqual(true, ContentSetCatalog.HasSewerSafeRoomChoice(flags), "safe-room choice recorded");
            AssertEqual(true, flags.Contains(StoryFlags.SewerSafeRoomFocusChosen), "safe-room focus choice recorded");
            ContentSetCatalog.MarkSewerSafeRoomChoice(flags, "blade");
            AssertEqual(false, flags.Contains(StoryFlags.SewerSafeRoomBladeChosen), "safe-room choice cannot be claimed twice");

            ContentSetCatalog.MarkSewerSliceRewardClaimed(flags);
            AssertEqual(true, flags.Contains(StoryFlags.SewerRewardClaimed), "reward claimed");
            AssertEqual(true, flags.Contains(StoryFlags.OldRoadTeaserUnlocked), "old road teaser flag");
            AssertEqual(true, ContentSetCatalog.AllowKoboldChapter(ContentSetCatalog.SewerSlice, flags), "old road teaser unlocks the kobold chapter in the sewer slice");
            AssertEqual(false, flags.Contains(StoryFlags.MidgaardSecondQuestGiven), "normal sewer slice does not unlock second quest scaffold");
            AssertEqual(false, ContentSetCatalog.SewerSliceRewardReady(flags), "reward no longer pending");
        }

        private static void SewerSliceEncountersHaveConciseGuidance()
        {
            foreach (EncounterDefinition encounter in ContentSetCatalog.SewerSliceEncounters)
            {
                AssertEqual(true, EncounterGuidanceCatalog.TryFor(encounter.LegacyStyle, out EncounterGuidance guidance), encounter.LegacyStyle + " has guidance");
                AssertEqual(true, guidance.IsValid, encounter.LegacyStyle + " guidance is valid");
                AssertEqual(true, guidance.Priority.Length <= 48, encounter.LegacyStyle + " priority fits the card");
                AssertEqual(true, guidance.Plan.Length <= 58, encounter.LegacyStyle + " plan fits the card");
                AssertEqual(true, guidance.Reminder.Length <= 62, encounter.LegacyStyle + " reminder fits the card");
            }

            AssertEqual(false, EncounterGuidanceCatalog.TryFor("ratsewer", out _), "prototype encounter does not impersonate sewer-slice guidance");
        }

        private static void SewerSliceFirstPlayContractProgressionIsIdempotent()
        {
            List<string> flags = new List<string>();
            List<InventoryItem> inventory = new List<InventoryItem>();

            AssertEqual(false, ContentSetCatalog.ShowPrototypeScaffold(ContentSetCatalog.SewerSlice), "normal slice hides prototype journal scaffold");
            AssertEqual(false, ContentSetCatalog.AllowPrototypeRouteTriggers(ContentSetCatalog.SewerSlice, flags), "normal slice disables prototype route triggers");
            AssertEqual(true, ContentSetCatalog.ShowPrototypeScaffold(ContentSetCatalog.FullPrototype), "full prototype shows scaffold");
            AssertEqual(true, ContentSetCatalog.AllowPrototypeRouteTriggers(ContentSetCatalog.FullPrototype, flags), "full prototype route triggers enabled");

            ContentSetCatalog.MarkSewerSliceContractAccepted(flags);
            AssertEqual(true, flags.Contains(StoryFlags.SewerContractAccepted), "contract accepted");
            AssertEqual(0, ContentSetCatalog.CountSewerSliceProof(inventory), "no proof before room victories");

            for (int i = 0; i < ContentSetCatalog.SewerSliceEncounters.Count; i++)
            {
                EncounterDefinition encounter = ContentSetCatalog.SewerSliceEncounterForProgress(ContentSetCatalog.SewerSliceClearedCount(flags));
                string clearFlag = ContentSetCatalog.ClearedFlagForEncounterStyle(encounter.LegacyStyle);
                bool firstClear = !flags.Contains(clearFlag);
                ContentSetCatalog.MarkSewerSliceEncounterCleared(flags, encounter.LegacyStyle);
                if (firstClear) inventory.Add(ContentSetCatalog.CreateSewerSliceProof());

                AssertEqual(i + 1, ContentSetCatalog.SewerSliceClearedCount(flags), "sewer cleared count after " + encounter.LegacyStyle);
                AssertEqual(i + 1, ContentSetCatalog.CountSewerSliceProof(inventory), "one proof per authored sewer victory");
                if (i == 1)
                {
                    ContentSetCatalog.MarkSewerSafeRoomChoice(flags, "blade");
                    AssertEqual(true, ContentSetCatalog.HasSewerSafeRoomChoice(flags), "safe-room decision sits between the second and third rooms");
                }
            }

            AssertEqual(ContentSetCatalog.SewerSliceRequiredProofCount, ContentSetCatalog.CountSewerSliceProof(inventory), "exact sewer proof requirement");
            AssertEqual(true, ContentSetCatalog.SewerSliceRewardReady(flags, inventory), "reward ready with three rooms and proof");

            bool claimed = ContentSetCatalog.TryClaimSewerSliceReward(flags, inventory, out InventoryItem reward, out string claimNote);
            AssertEqual(true, claimed, "first reward claim succeeds: " + claimNote);
            AssertEqual(false, reward == null, "sewer reward created");
            inventory.Add(reward);
            AssertEqual(0, ContentSetCatalog.CountSewerSliceProof(inventory), "claim consumes proof");
            AssertEqual(true, ContentSetCatalog.SewerSliceComplete(flags), "reward claim unlocks old road teaser");
            AssertEqual(false, flags.Contains(StoryFlags.MidgaardSecondQuestGiven), "reward claim does not unlock future scaffold");

            bool secondClaim = ContentSetCatalog.TryClaimSewerSliceReward(flags, inventory, out InventoryItem secondReward, out string secondNote);
            AssertEqual(false, secondClaim, "second reward claim blocked: " + secondNote);
            AssertEqual(true, secondReward == null, "second reward omitted");
            AssertEqual(1, inventory.Count(item => item != null && item.Slot == "armor" && item.Trait == "nimble" && item.Material == "rat pelt"), "reward exists once");
        }

        private static void AttackDamageProfileIncludesSharedModifiers()
        {
            CombatUnit attacker = new CombatUnit
            {
                DamageMin = 4,
                DamageMax = 7,
                DamageType = "",
                Stealthed = 1
            };
            CombatUnit target = new CombatUnit
            {
                Defense = 2,
                ArmorBonus = 1,
                Hexed = 1
            };

            AttackDamageProfile profile = AttackRules.BuildDamageProfile(attacker, target, 13, 2, 4);

            AssertEqual("physical", profile.DamageType, "default damage type");
            AssertEqual(2, profile.SkillBonus, "skill bonus");
            AssertEqual(2, profile.HexShift, "hex shift");
            AssertEqual(2, profile.EnrageBonus, "physical enrage");
            AssertEqual(4, profile.FlatPowerBonus, "flat transformation power");
            AssertEqual(3, profile.StealthBonus, "stealth bonus");
            AssertEqual(14, profile.MinRawDamage, "minimum raw attack damage");
            AssertEqual(17, profile.MaxRawDamage, "maximum raw attack damage");
        }

        private static void NonPhysicalAttackDamageIgnoresWarriorEnrage()
        {
            CombatUnit attacker = new CombatUnit
            {
                DamageMin = 5,
                DamageMax = 5,
                DamageType = "fire"
            };
            CombatUnit target = new CombatUnit
            {
                Defense = 1
            };

            AttackDamageProfile profile = AttackRules.BuildDamageProfile(attacker, target, 10, 8, 4);

            AssertEqual("fire", profile.DamageType, "explicit damage type");
            AssertEqual(0, profile.EnrageBonus, "non-physical enrage");
            AssertEqual(4, profile.FlatPowerBonus, "non-physical attacks retain flat transformation power");
            AssertEqual(10, profile.MinRawDamage, "non-physical minimum raw damage");
            AssertEqual(10, profile.MaxRawDamage, "non-physical maximum raw damage");
        }

        private static void ReachableMoveCostsRespectBlockersAndTerrainCosts()
        {
            CombatUnit active = new CombatUnit { Id = "actor", X = 0, Y = 1 };
            HashSet<string> blockers = new HashSet<string> { "1,1" };
            int[,] costs = CombatGridRules.ReachableMoveCosts(
                active,
                5,
                3,
                4,
                999,
                (unit, x, y) => x >= 0 && y >= 0 && x < 5 && y < 3 && !blockers.Contains(x + "," + y),
                (unit, x, y) => x == 2 && y == 0 ? 2 : 1);

            AssertEqual(0, costs[0, 1], "origin move cost");
            AssertEqual(999, costs[1, 1], "blocked center tile");
            AssertEqual(4, costs[2, 0], "terrain-cost detour tile");
            AssertEqual(999, costs[3, 0], "beyond move budget");

            IReadOnlyList<Vector2Int> path = CombatGridRules.ShortestReachablePath(
                active,
                costs,
                2,
                0,
                999,
                (unit, x, y) => x == 2 && y == 0 ? 2 : 1);
            AssertEqual(4, path.Count, "movement preview path includes origin, weighted steps, and destination");
            AssertEqual(new Vector2Int(0, 1), path[0], "movement preview begins at actor");
            AssertEqual(new Vector2Int(2, 0), path[path.Count - 1], "movement preview ends at staged destination");
            AssertEqual(false, path.Contains(new Vector2Int(1, 1)), "movement preview never crosses a blocking cell");
            int pathCost = path
                .Skip(1)
                .Sum(cell => cell.x == 2 && cell.y == 0 ? 2 : 1);
            AssertEqual(costs[2, 0], pathCost, "movement preview weighted cost matches reachable-cost truth");
            AssertEqual(
                true,
                path.SequenceEqual(CombatGridRules.ShortestReachablePath(
                    active,
                    costs,
                    2,
                    0,
                    999,
                    (unit, x, y) => x == 2 && y == 0 ? 2 : 1)),
                "movement preview path is deterministic");
            AssertEqual(
                0,
                CombatGridRules.ShortestReachablePath(
                    active,
                    costs,
                    3,
                    0,
                    999,
                    (unit, x, y) => 1).Count,
                "movement preview returns no route for an unreachable destination");

            CombatUnit detourActor = new CombatUnit { Id = "detour", X = 0, Y = 1 };
            int[,] detourCosts = CombatGridRules.ReachableMoveCosts(
                detourActor,
                5,
                3,
                8,
                999,
                (unit, x, y) => x >= 0 && y >= 0 && x < 5 && y < 3,
                (unit, x, y) => x == 2 && y == 1 ? 4 : 1);
            IReadOnlyList<Vector2Int> cheaperDetour = CombatGridRules.ShortestReachablePath(
                detourActor,
                detourCosts,
                4,
                1,
                999,
                (unit, x, y) => x == 2 && y == 1 ? 4 : 1);
            AssertEqual(6, detourCosts[4, 1], "weighted movement chooses the cheaper route around costly terrain");
            AssertEqual(false, cheaperDetour.Contains(new Vector2Int(2, 1)), "movement preview follows the cheaper detour instead of the costly shortcut");
            AssertEqual(new Vector2Int(4, 1), cheaperDetour[cheaperDetour.Count - 1], "weighted detour reaches its intended destination");
        }

        private static void SupercoverLineIncludesCornerTouchCells()
        {
            List<Vector2Int> cells = CombatGridRules.SupercoverLine(0, 0, 2, 2, 4, 4).ToList();

            AssertContains(cells, new Vector2Int(0, 0), "supercover start");
            AssertContains(cells, new Vector2Int(1, 0), "supercover corner x cell");
            AssertContains(cells, new Vector2Int(0, 1), "supercover corner y cell");
            AssertContains(cells, new Vector2Int(1, 1), "supercover center diagonal cell");
            AssertContains(cells, new Vector2Int(2, 2), "supercover end");
        }

        private static void LineOfSightUsesSupercoverBlockers()
        {
            bool blocked = CombatGridRules.HasLineOfSight(0, 0, 2, 2, 4, 4, true, (x, y) => x == 1 && y == 0);
            bool openForArcs = CombatGridRules.HasLineOfSight(0, 0, 2, 2, 4, 4, false, (x, y) => true);

            AssertEqual(false, blocked, "missile line of sight through supercover blocker");
            AssertEqual(true, openForArcs, "non-missile line of sight bypass");
        }

        private static void ContentSetNormalSaveRoundTrip()
        {
            string root = Path.Combine(Path.GetTempPath(), "AshenHallsContentSetRoundTrip-" + Guid.NewGuid().ToString("N"));
            try
            {
                string path = SaveService.SavePath(root);
                GameState state = new GameState
                {
                    SaveVersion = VersionInfo.SaveVersion,
                    ContentSetId = ContentSetCatalog.SewerSlice,
                    Seed = 303,
                    Depth = 1,
                    Gold = 12
                };

                bool saved = SaveService.TrySaveCampaignState(path, state, false, out string blockedReason);
                AssertEqual(true, saved, "normal campaign save allowed");
                AssertEqual("", blockedReason, "normal campaign save block reason");

                GameState loaded = SaveService.LoadGameState(path, out bool usedBackup);
                AssertEqual(false, usedBackup, "normal content-set primary save used");
                AssertEqual(VersionInfo.SaveVersion, loaded.SaveVersion, "normal content-set save version");
                AssertEqual(ContentSetCatalog.SewerSlice, loaded.ContentSetId, "normal content-set round trip");

                string repaired = ContentSetCatalog.RepairLoadedContentSetId(loaded, out bool wasRepaired, out string note);
                AssertEqual(false, wasRepaired, "normal content-set does not repair");
                AssertEqual("", note, "normal content-set repair note");
                AssertEqual(ContentSetCatalog.SewerSlice, repaired, "normal content-set remains sewer slice");
            }
            finally
            {
                if (Directory.Exists(root)) Directory.Delete(root, true);
            }
        }

        private static void LegacyV17SaveMigratesToFullPrototype()
        {
            GameState legacy = new GameState
            {
                SaveVersion = 17,
                ContentSetId = ContentSetCatalog.SewerSlice,
                Seed = 404,
                Depth = 3
            };

            string repaired = ContentSetCatalog.RepairLoadedContentSetId(legacy, out bool wasRepaired, out string note);
            AssertEqual(true, wasRepaired, "v17 content-set migration repairs");
            AssertEqual(ContentSetCatalog.FullPrototype, repaired, "v17 save migrates to full prototype");
            AssertEqual(true, note.Contains("v17"), "v17 migration note mentions version");
        }

        private static void UnknownContentSetRepairsToSewerSlice()
        {
            GameState unknown = new GameState
            {
                SaveVersion = VersionInfo.SaveVersion,
                ContentSetId = "strange-future-campaign",
                Seed = 505,
                Depth = 1
            };

            string repaired = ContentSetCatalog.RepairLoadedContentSetId(unknown, out bool wasRepaired, out string note);
            AssertEqual(true, wasRepaired, "unknown content-set repairs");
            AssertEqual(ContentSetCatalog.SewerSlice, repaired, "unknown content-set falls back to sewer slice");
            AssertEqual(true, note.Contains("Unknown content set"), "unknown content-set repair note");
        }

        private static void LabSaveDoesNotWriteCampaignFile()
        {
            string root = Path.Combine(Path.GetTempPath(), "AshenHallsLabSaveBlocked-" + Guid.NewGuid().ToString("N"));
            try
            {
                string path = SaveService.SavePath(root);
                GameState labState = new GameState
                {
                    SaveVersion = VersionInfo.SaveVersion,
                    ContentSetId = ContentSetCatalog.FullPrototype,
                    Seed = 606,
                    Depth = 3,
                    Gold = 99
                };

                bool saved = SaveService.TrySaveCampaignState(path, labState, true, out string blockedReason);
                AssertEqual(false, saved, "lab campaign save blocked");
                AssertEqual(true, blockedReason.Contains("Lab runs are not saved"), "lab save block reason");
                AssertEqual(false, File.Exists(path), "lab save does not create campaign file");
            }
            finally
            {
                if (Directory.Exists(root)) Directory.Delete(root, true);
            }
        }

        private static void SaveServiceWritesAndFallsBackToBackup()
        {
            string root = Path.Combine(Path.GetTempPath(), "AshenHallsSaveServiceSmoke-" + Guid.NewGuid().ToString("N"));
            try
            {
                string path = SaveService.SavePath(root);
                SaveService.SaveGameState(path, new GameState { SaveVersion = VersionInfo.SaveVersion, Seed = 101, Depth = 1, Gold = 7 });
                AssertEqual(true, SaveService.SaveExists(path), "save service created primary file");

                GameState primary = SaveService.LoadGameState(path, out bool usedBackup);
                AssertEqual(false, usedBackup, "primary load backup flag");
                AssertEqual(101, primary.Seed, "primary load seed");

                SaveService.SaveGameState(path, new GameState { SaveVersion = VersionInfo.SaveVersion, Seed = 202, Depth = 2, Gold = 11 });
                File.Delete(path);

                GameState backup = SaveService.LoadGameState(path, out usedBackup);
                AssertEqual(true, usedBackup, "backup load flag");
                AssertEqual(101, backup.Seed, "backup load seed");

                string mapPath = Path.Combine(root, "map-save.json");
                MapData validMap = OpenTestMap(3, 3);
                ExplorationSurfaceRules.EnsureGrid(validMap);
                SaveService.SaveGameState(mapPath, new GameState { SaveVersion = VersionInfo.SaveVersion, Seed = 303, Depth = 1, Map = validMap });
                MapData malformedMap = OpenTestMap(3, 3);
                malformedMap.SurfaceRoles.Clear();
                SaveService.SaveGameState(mapPath, new GameState { SaveVersion = VersionInfo.SaveVersion, Seed = 404, Depth = 1, Map = malformedMap });
                GameState repairedFromBackup = SaveService.LoadGameState(
                    mapPath,
                    candidate => candidate != null
                        && candidate.SaveVersion == VersionInfo.SaveVersion
                        && candidate.Map != null
                        && ExplorationSurfaceRules.IsLoadableMap(candidate.Map, true),
                    out usedBackup);
                AssertEqual(true, usedBackup, "malformed v19 primary falls back to structurally valid backup");
                AssertEqual(303, repairedFromBackup.Seed, "map validator restored valid backup state");

                string partyPath = Path.Combine(root, "party-save.json");
                GameState validPartyState = new GameState
                {
                    SaveVersion = VersionInfo.SaveVersion,
                    Mode = GameMode.Muster,
                    Seed = 505,
                    Party = new List<PartyMember> { new PartyMember { Name = "Maer" } }
                };
                SaveService.SaveGameState(partyPath, validPartyState);
                SaveService.SaveGameState(partyPath, new GameState
                {
                    SaveVersion = VersionInfo.SaveVersion,
                    Mode = GameMode.Muster,
                    Seed = 606,
                    Party = new List<PartyMember>()
                });
                GameState repairedParty = SaveService.LoadGameState(
                    partyPath,
                    candidate => SaveCandidateRules.IsLoadable(candidate, VersionInfo.SaveVersion),
                    out usedBackup);
                AssertEqual(true, usedBackup, "empty-party primary falls back to deeply valid backup");
                AssertEqual(505, repairedParty.Seed, "deep candidate validation restores valid party backup");

                string legacyRoot = Path.Combine(root, VersionInfo.LegacyProductName);
                string renamedRoot = Path.Combine(root, VersionInfo.ProductName);
                string legacyPath = SaveService.LegacySavePath(legacyRoot);
                string renamedPath = SaveService.SavePath(renamedRoot);
                SaveService.SaveGameState(legacyPath, new GameState { SaveVersion = VersionInfo.SaveVersion, Seed = 707, Depth = 1, Gold = 33 });
                AssertEqual(true, SaveService.TryImportLegacySave(renamedPath, legacyPath), "renamed product imports the legacy campaign once");
                AssertEqual(false, SaveService.TryImportLegacySave(renamedPath, legacyPath), "legacy import never overwrites an existing renamed save");
                GameState renamedSave = SaveService.LoadGameState(renamedPath, out usedBackup);
                AssertEqual(false, usedBackup, "renamed save imports as a normal primary");
                AssertEqual(707, renamedSave.Seed, "renamed save preserves the legacy campaign payload");
            }
            finally
            {
                if (Directory.Exists(root)) Directory.Delete(root, true);
            }
        }

        private static void AssertEqual<T>(T expected, T actual, string label)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new InvalidOperationException($"{label}: expected {expected}, got {actual}");
            }
        }

        private static int CountOccurrences(string text, string value)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(value)) return 0;
            int count = 0;
            int index = 0;
            while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += value.Length;
            }
            return count;
        }

        private static bool SchoolsOverlap(string left, string right)
        {
            string[] leftTokens = (left ?? "").Split('|');
            string[] rightTokens = (right ?? "").Split('|');
            return leftTokens.Any(a => rightTokens.Any(b => string.Equals(a.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase)));
        }

        private static void AssertContains(List<Vector2Int> cells, Vector2Int expected, string label)
        {
            if (!cells.Contains(expected))
            {
                throw new InvalidOperationException($"{label}: expected line to include {expected}, got {string.Join(", ", cells)}");
            }
        }

        private static void AssertPointsOnBoard(string label, Point[] points)
        {
            if (points == null) return;
            foreach (Point point in points)
            {
                AssertEqual(true, point.X >= 0 && point.X < 12 && point.Y >= 0 && point.Y < 8, label + " " + PointKey(point));
            }
        }

        private static void AssertNoDuplicatePoints(string label, Point[] points)
        {
            if (points == null) return;
            HashSet<string> seen = new HashSet<string>();
            foreach (Point point in points)
            {
                string key = PointKey(point);
                if (!seen.Add(key)) throw new InvalidOperationException(label + ": duplicate point " + key);
            }
        }

        private static void AssertNoFixedPlacementOverlap(EncounterDefinition encounter)
        {
            HashSet<string> occupied = new HashSet<string>();
            AddFixedPoints(encounter.Id + " party placement", occupied, encounter.PartyPlacements);
            AddFixedPoints(encounter.Id + " enemy placement", occupied, encounter.EnemyPlacements);
            AddFixedPoints(encounter.Id + " obstacle", occupied, encounter.Obstacles);
        }

        private static void AddFixedPoints(string label, HashSet<string> occupied, Point[] points)
        {
            if (points == null) return;
            foreach (Point point in points)
            {
                string key = PointKey(point);
                if (!occupied.Add(key)) throw new InvalidOperationException(label + ": overlaps fixed point " + key);
            }
        }

        private static string PointKey(Point point)
        {
            return point == null ? "null" : point.X + "," + point.Y;
        }

        private static void AssertThrows<T>(Action action, string label) where T : Exception
        {
            try
            {
                action();
            }
            catch (T)
            {
                return;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(label + ": expected " + typeof(T).Name + ", got " + ex.GetType().Name);
            }
            throw new InvalidOperationException(label + ": expected " + typeof(T).Name);
        }
    }
}
