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

        public static void RunTitleScreenRules()
        {
            try
            {
                TavernMenuRulesKeepNormalOpeningPlayerFacing();
                TavernScreenLayoutFitsSupportedResolutions();
                TavernStormRegionsStayOutsideTheRoom();
                TavernTitleAnimationIsReadableAndMotionSafe();
                GrandHearthTitlePresentationIsDeterministicAndMotionSafe();
                TitleAudioRulesKeepMusicAndMenuFeedbackLegible();
                Debug.Log(VersionInfo.ProductName + " title-screen rule smoke tests passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " title-screen rule smoke tests failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunTitleAudioRules()
        {
            try
            {
                TitleAudioRulesKeepMusicAndMenuFeedbackLegible();
                AuthoredSfxAssetsMatchRuntimeCueContracts();
                OriginalMusicAssetsMatchRuntimeCueContracts();
                Debug.Log(VersionInfo.ProductName + " title-audio rule smoke tests passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " title-audio rule smoke tests failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunCombatAudioRules()
        {
            try
            {
                AuthoredSfxAssetsMatchRuntimeCueContracts();
                CombatImpactPresentationHasReadableEchoAndSpatialMix();
                CombatAudioCuesAndAmbienceStaySemanticAndMixSafe();
                WeaponFeedbackProfilesStayDistinctAndBounded();
                EnemyPowerPresentationIsDistinctAndBounded();
                AdaptiveMusicDirectorRoutesDistinctContexts();
                Debug.Log(VersionInfo.ProductName + " combat-audio rule smoke tests passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " combat-audio rule smoke tests failed: " + ex);
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
            TitleAudioRulesKeepMusicAndMenuFeedbackLegible();
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
            ExplorationChartRulesTrackDurableTerrainDiscovery();
            ExplorationReadabilityRulesKeepGroundBehindSprites();
            WorldMapAtlasPixelMetricsShareTopDownSnapshots();
            WorldMapSpriteCellCoverageRejectsPruningExtremes();
            WorldMapRegionLandmarkCatalogIsSemantic();
            WorldMapRegionMarkerCatalogIsSemantic();
            WorldAreaSetpieceCatalogIsSemantic();
            V24WorldMapCharacterArtRulesAreStable();
            ExplorationMiniMapPresentationRulesReserveSemanticMarkers();
            ExplorationMiniMapTerrainCacheIsStableAndTopDown();
            WorldMapProgressionPresentationRulesTrackChapterFiveState();
            ApprovedV130WorldMapAtlasesMatchRuntimeContracts();
            ExplorationArtRulesMapSemanticTilesAndScale();
            ExplorationSurfaceRulesPreserveAuthoredMapStructure();
            ExplorationRoadPresentationRulesKeepRoutesReadable();
            CombatHudScreenLayoutFitsSupportedResolutions();
            CombatAbilityModalLayoutFitsSupportedResolutions();
            CombatAbilityModalNavigationRepeatsPredictably();
            V29CombatPresentationPolishRulesStayStable();
            CombatAbilityModalCardsExposeArtIdentity();
            CombatTargetHighlightsRequireAnArmedPower();
            CombatDecisionClarityRulesKeepTurnsActionable();
            AdvancedCasterProgressionPowersAreExplicit();
            EarlyProgressionRuleSmoke.Run();
            DemonFormPackageDefinesCohesiveProgression();
            LightningPowerRulesDefineTacticalStormLadder();
            CombatIconCatalogDefinesUniqueActiveSkillArt();
            SignatureSpellIconCatalogDefinesUniqueFormulaArt();
            ApprovedPowerIconAtlasesMatchRuntimeContracts();
            CombatFeedbackRoutingUsesSemanticArt();
            CombatPowerPresentationMakesSignaturesDistinct();
            CombatPowerVisualMotifsStaySemanticAndBounded();
            MageWarlockSpellVfxProfilesStayDistinctAndMotionSafe();
            SupportHexSpellVfxProfilesStayDistinctAndMotionSafe();
            ClassSkillVfxProfilesStayDistinctAndMotionSafe();
            CombatPowerTravelVfxProfilesStayDistinctAndMotionSafe();
            CombatPowerAftermathAndTimelineStayDeterministic();
            CombatPowerActorChoreographyStaysSynchronizedAndBounded();
            BetaLabToolbarRulesStayResponsiveAndAccessible();
            CombatVfxShowcaseCatalogIsStableAndReplayable();
            CombatPowerSfxProfilesStayDistinctAndMixSafe();
            CombatUnitPresentationBeatsStaySynchronizedAndBounded();
            CombatImpactProfilesStageSignaturePowers();
            CombatFieldsUseDistinctVisualAndAudioProfiles();
            CombatImpactPresentationHasReadableEchoAndSpatialMix();
            CombatAudioCuesAndAmbienceStaySemanticAndMixSafe();
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
            SignatureItemCatalogAndAtlasMatchRuntimeContract();
            PartyGrowthRulesStageAndApplyCampaignPointsSafely();
            InventoryEquipmentRulesMakeOwnershipAndComparisonsExplicit();
            InventoryItemIdentityRulesAreStableAndConservative();
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
            BoneRoadProductionArcDefinitionsAreStable();
            GlassAndAshProductionArcDefinitionsAreStable();
            RedGateProductionArcDefinitionsAreStable();
            SewerSliceContentSetDefinesCompleteFirstPlayPath();
            SewerSliceEncountersHaveConciseGuidance();
            SewerSliceFirstPlayContractProgressionIsIdempotent();
            AttackDamageProfileIncludesSharedModifiers();
            NonPhysicalAttackDamageIgnoresWarriorEnrage();
            ReachableMoveCostsRespectBlockersAndTerrainCosts();
            SupercoverLineIncludesCornerTouchCells();
            LineOfSightUsesSupercoverBlockers();
            ContentSetNormalSaveRoundTrip();
            InventoryIdentitySaveRoundTrip();
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
                "titleforge", "titlereveal", "titlefocus", "titleconfirm", "titleopen", "titleclose",
                "combatstep", "combatguard", "combatturn", "combatcrit",
                "arrowrelease", "thrust", "spell", "fire",
                "combatambsteel", "combatambsewer", "combatambarcane",
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
                "ashen_atlas_overview_loop",
                "a_fire_between_roads_loop",
                "anvil_echoes_in_old_stone_loop",
                "ash_fen_haze_loop",
                "banners_before_the_crown_loop",
                "bells_over_temple_square_loop",
                "bones_beneath_stone_loop",
                "chains_below_bellstone_loop",
                "combat_battle_pulse_loop",
                "crooked_crown_kobold_king_loop",
                "crown_and_ashes_boss_loop",
                "drow_nightblades_loop",
                "dusk_market_ambush_loop",
                "embers_at_the_broken_seal_loop",
                "embers_carry_home_victory_loop",
                "footsteps_behind_loop",
                "four_names_by_the_fire_loop",
                "glass_and_quiet_stars_loop",
                "glass_warrens_shimmer_loop",
                "gloam_courts_echo_loop",
                "green_shrine_teal_loop",
                "kobold_hide_drums_loop",
                "lanterns_and_ledgers_loop",
                "lanterns_under_false_names_loop",
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
                "old_sap_under_ash_loop",
                "one_more_turn_last_stand_loop",
                "ratfolk_plague_march_loop",
                "red_gate_omen_loop",
                "red_rift_war_loop",
                "roots_remember_loop",
                "salt_cistern_drips_loop",
                "sewer_hunt_combat_loop",
                "sigils_crossed_arcane_duel_loop",
                "smoke_across_the_road_loop",
                "sparks_on_the_oathring_loop",
                "starlight_in_the_glass_index_loop",
                "steel_against_the_chosen_loop",
                "tavern_storm_hearth_ensemble_loop",
                "the_crypt_keeps_its_names_loop",
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
                    ? 60.1f
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

            AssertEqual(25, abilityIds.Count, "active martial and demon-form ability count");
            AssertEqual(true, iconIndices.All(index => index >= 0), "every active martial ability has icon art");
            AssertEqual(abilityIds.Count, iconIndices.Distinct().Count(), "active martial ability icons are unique");
            AssertEqual(19, CombatIconCatalog.AbilityIndex("smokebomb"), "expanded smoke bomb icon cell");
            AssertEqual("20,21,22,23", string.Join(",", AbilityCatalog.IdsForClass("demon").Select(CombatIconCatalog.AbilityIndex)), "demon-form art occupies the appended atlas row");
            AssertEqual("24,25,26", string.Join(",", new[] { "sunder", "shadowstep", "quickshot" }.Select(CombatIconCatalog.AbilityIndex)), "level 16 martial capstones occupy the appended atlas row");
            AssertEqual(7, CombatIconCatalog.AbilityAtlasRows(1024, 1792), "expanded ability atlas rows");
            AssertEqual(true, CombatIconCatalog.IsAbilityAtlasDimensions(1024, 1792), "exact expanded ability atlas dimensions are accepted");
            AssertEqual(false, CombatIconCatalog.IsAbilityAtlasDimensions(1024, 1536), "pre-capstone ability atlas dimensions are rejected");
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
            AssertEqual(56, codes.Length, "full prototype formula count");
            AssertEqual(true, indices.All(index => index >= 0), "every spellbook formula has dedicated icon art");
            AssertEqual(codes.Length, indices.Distinct().Count(), "every spellbook formula icon is unique");
            AssertEqual(
                string.Join(",", Enumerable.Range(0, codes.Length)),
                string.Join(",", indices),
                "formula catalog order is the stable row-major spell icon contract");
            AssertEqual(7, CombatIconCatalog.SignatureSpellAtlasColumns, "expanded spell icon atlas columns");
            AssertEqual(8, CombatIconCatalog.SignatureSpellAtlasRows, "expanded spell icon atlas rows");
            AssertEqual("49,50", string.Join(",", new[] { CombatIconCatalog.SignatureSpellIndex("RBT"), CombatIconCatalog.SignatureSpellIndex("VRS") }), "pact gap spells occupy appended signature cells");
            AssertEqual("51,52,53,54,55", string.Join(",", new[] { "DWP", "CNS", "GRH", "SLV", "ACR" }.Select(CombatIconCatalog.SignatureSpellIndex)), "new gradual-progression spells fill the reserved signature cells");
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
            Texture2D mageWarlockEffects = null;
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
                mageWarlockEffects = LoadApprovedRuntimeAtlas(RuntimeArtManifest.MageWarlockSpellVfxAtlas);
                AssertEqual(new Vector2Int(CombatIconCatalog.AbilityAtlasWidth, CombatIconCatalog.AbilityAtlasHeight), new Vector2Int(ability.width, ability.height), "approved ability atlas dimensions");
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
                AssertEqual(new Vector2Int(1280, 1280), new Vector2Int(epicEffects.width, epicEffects.height), "approved 4x4 epic spell effects atlas dimensions");
                AssertEqual(new Vector2Int(1254, 1254), new Vector2Int(mageWarlockEffects.width, mageWarlockEffects.height), "approved 4x4 mage and warlock spell VFX atlas dimensions");
                Color32[] mageWarlockPixels = mageWarlockEffects.GetPixels32();
                int mageWarlockTransparentPixels = mageWarlockPixels.Count(pixel => pixel.a < 8);
                int mageWarlockVisiblePixels = mageWarlockPixels.Count(pixel => pixel.a >= 32);
                AssertEqual(mageWarlockEffects.width * mageWarlockEffects.height, mageWarlockPixels.Length, "approved mage and warlock spell VFX pixels are readable");
                AssertEqual(true, mageWarlockTransparentPixels >= mageWarlockPixels.Length * 0.45f, "approved mage and warlock VFX preserve substantial transparent framing");
                AssertEqual(true, mageWarlockVisiblePixels >= mageWarlockPixels.Length * 0.20f, "approved mage and warlock VFX preserve substantial visible spell silhouettes");
                AssertAtlasCellCoverage(
                    ability,
                    CombatIconCatalog.AbilityAtlasColumns,
                    CombatIconCatalog.ExpandedAbilityAtlasRows,
                    CombatIconCatalog.MappedAbilityIndices,
                    0.10f,
                    0.90f,
                    "approved ability icon");
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
                AssertAtlasCellSafeGutter(
                    ability,
                    CombatIconCatalog.AbilityAtlasColumns,
                    CombatIconCatalog.ExpandedAbilityAtlasRows,
                    CombatIconCatalog.MappedAbilityIndices,
                    12,
                    8,
                    32,
                    "approved ability icon");
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
                if (mageWarlockEffects != null) UnityEngine.Object.DestroyImmediate(mageWarlockEffects);
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
            AssertEqual(0, CombatFeedbackRules.RangerImpactIndex("quickshot"), "quick shot uses the canonical arrow-contact art");

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
            AssertEqual("spellvfx:0", CombatFeedbackRules.SpellGlyphKind(fireball, "caster"), "Fireball caster begins on its authored compression rune");
            AssertEqual("spellvfx:2", CombatFeedbackRules.SpellGlyphKind(fireball, "fireball"), "Fireball impact uses its authored explosive crown");
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
                new Vector2Int(960, 600),
                new Vector2Int(1024, 768),
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
            AssertEqual(4, TavernMenuRules.NormalChoiceCount(false), "tavern choices without save keep a stable scroll");
            AssertEqual("Continue,New Game,Settings,Exit Game", string.Join(",", TavernMenuRules.NormalChoiceLabels(false)), "tavern labels without save");
            AssertEqual(true, TavernMenuRules.ShowContinue(false), "continue remains visible without save");
            AssertEqual(false, TavernMenuRules.EnableContinue(false), "continue is visibly unavailable without save");

            AssertEqual(4, TavernMenuRules.NormalChoiceCount(true), "tavern choices with save");
            AssertEqual("Continue,New Game,Settings,Exit Game", string.Join(",", TavernMenuRules.NormalChoiceLabels(true)), "tavern labels with save");
            AssertEqual(true, TavernMenuRules.ShowContinue(true), "continue visible with save");
            AssertEqual(true, TavernMenuRules.EnableContinue(true), "continue becomes actionable with save");

            AssertEqual(false, TavernMenuRules.ShowDeveloperTesting(false), "release build hides beta testing");
            AssertEqual(true, TavernMenuRules.ShowDeveloperTesting(true), "development build can show beta testing");
            AssertEqual(false, TavernMenuRules.CanDispatchDeveloperTestingShortcut(false), "retail title shortcut cannot dispatch a hidden Beta Lab route");
            AssertEqual(true, TavernMenuRules.CanDispatchDeveloperTestingShortcut(true), "development title shortcut can open its testing tools");

            foreach (bool saveExists in new[] { false, true })
            {
                IReadOnlyList<string> labels = TavernMenuRules.NormalChoiceLabels(saveExists);
                for (int i = 0; i < labels.Count; i++)
                {
                    AssertEqual(true, !string.IsNullOrWhiteSpace(labels[i]) && labels[i].Length <= 10, $"tavern choice {i} stays concise and player-facing");
                    for (int other = i + 1; other < labels.Count; other++)
                    {
                        AssertEqual(false, string.Equals(labels[i], labels[other], StringComparison.Ordinal), $"tavern choices {i} and {other} remain distinct");
                    }
                }
            }
        }

        private static void CampaignCheckpointRulesProtectPlayerProgress()
        {
            GameState explore = new GameState { Mode = GameMode.Explore, Combat = null };
            AssertEqual(true, CampaignCheckpointRules.ShouldWrite(explore, false, false), "normal exploration can checkpoint");
            GameState victory = new GameState { Mode = GameMode.Victory, Combat = null };
            AssertEqual(true, CampaignCheckpointRules.ShouldWrite(victory, false, false), "final victory can preserve its relic and completion state");
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
                new Vector2Int(960, 600),
                new Vector2Int(1024, 768),
                new Vector2Int(1280, 720),
                new Vector2Int(1600, 900),
                new Vector2Int(1920, 1200),
                new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152),
                new Vector2Int(2560, 1080),
                new Vector2Int(3440, 1440)
            };

            foreach (Vector2Int size in sizes)
            {
                foreach (bool saveExists in new[] { false, true })
                {
                    foreach (bool developerTestingVisible in new[] { false, true })
                    {
                        TavernScreenGeometry geometry = TavernScreenLayout.Calculate(size.x, size.y, saveExists, developerTestingVisible);
                        TitleBackdropProjection projection = TitleScreenPresentationRules.ProjectBackdrop(size.x, size.y, 1672f, 941f);
                        Rect logoSafeZone = projection.ProjectNormalized(TitleScreenPresentationRules.LogoSafeZoneNormalized);
                        Rect menuSafeZone = projection.ProjectNormalized(TitleScreenPresentationRules.MenuSafeZoneNormalized);
                        AssertEqual(true, geometry.Fits(size.x, size.y), $"tavern layout fits {size.x}x{size.y} save={saveExists} dev={developerTestingVisible}");
                        AssertEqual(false, TitleScreenPresentationRules.Overlaps(geometry.Title, geometry.Menu, 10f), $"tavern title and menu keep a readable gutter {size.x}x{size.y} save={saveExists}");
                        AssertEqual(false, geometry.Testing.Overlaps(geometry.Menu), $"tavern testing panel does not overlap menu {size.x}x{size.y} save={saveExists}");
                        AssertEqual(true, geometry.Title.center.x < geometry.Menu.center.x, $"title leads the right-side menu at {size.x}x{size.y}");
                        AssertEqual(true, geometry.Title.xMin >= logoSafeZone.xMin - 0.01f && geometry.Title.xMax <= logoSafeZone.xMax + 0.01f, $"title plaque stays inside its painted quiet zone at {size.x}x{size.y}");
                        AssertEqual(true, geometry.Menu.xMin >= menuSafeZone.xMin - 0.01f && geometry.Menu.xMax <= size.x + 0.01f, $"menu stays inside its painted right-side zone at {size.x}x{size.y}");
                        AssertEqual(true, Mathf.Approximately(geometry.Settings.xMax, geometry.Menu.xMax) && geometry.Settings.width >= geometry.Menu.width, $"settings stays right-aligned and never narrows the menu at {size.x}x{size.y}");

                        IReadOnlyList<Rect> buttons = TavernScreenLayout.ButtonRects(saveExists, geometry.Menu.width);
                        AssertEqual(TavernMenuRules.NormalChoiceCount(saveExists), buttons.Count, "tavern button count mirrors menu rules");
                        TavernMenuScrollGeometry scroll = TavernScreenLayout.ScrollGeometry(geometry.Menu.width, geometry.Menu.height);
                        AssertEqual(true, scroll.Fits(geometry.Menu.width, geometry.Menu.height), $"title scroll fits menu bounds at {size.x}x{size.y}");
                        AssertEqual(true, scroll.Sheet.width > 0f && scroll.Content.width > 0f, $"title scroll keeps a visible parchment sheet and content well at {size.x}x{size.y}");
                        AssertEqual(true, Mathf.Approximately(scroll.TopRoll.width, scroll.BottomRoll.width) && Mathf.Approximately(scroll.TopRoll.height, scroll.BottomRoll.height), $"title scroll rolls stay symmetric at {size.x}x{size.y}");
                        AssertEqual(true, Mathf.Approximately(scroll.TopRoll.center.x, scroll.Sheet.center.x) && Mathf.Approximately(scroll.BottomRoll.center.x, scroll.Sheet.center.x), $"title scroll rolls remain centered on the sheet at {size.x}x{size.y}");
                        AssertEqual(true, scroll.TopRoll.width >= scroll.Sheet.width + 12f && scroll.BottomRoll.width >= scroll.Sheet.width + 12f, $"title scroll rolls extend beyond the parchment sheet at {size.x}x{size.y}");
                        AssertEqual(true, scroll.TopRoll.yMax > scroll.Sheet.yMin && scroll.BottomRoll.yMin < scroll.Sheet.yMax, $"title scroll sheet tucks continuously beneath both rolls at {size.x}x{size.y}");
                        AssertEqual(true, scroll.Content.xMin - scroll.Bounds.xMin >= TitleScreenPresentationRules.MenuScrollSideInset && scroll.Bounds.xMax - scroll.Content.xMax >= TitleScreenPresentationRules.MenuScrollSideInset, $"title scroll content clears its authored side rails at {size.x}x{size.y}");
                        AssertEqual(true, scroll.Content.width >= 96f, $"title scroll retains a readable center between its authored rails at {size.x}x{size.y}");
                        AssertEqual(true, !TavernScreenLayout.IsCompactMenu(geometry.Menu.width) || scroll.Content.width - 49f >= 68f, $"compact title choices retain icon and single-line label wells at {size.x}x{size.y}");
                        AssertEqual(true, scroll.Header.yMin >= TitleScreenPresentationRules.MenuScrollTopInset, $"title scroll heading clears its authored upper roller at {size.x}x{size.y}");
                        AssertEqual(true, buttons.Count > 0 && buttons[0].yMin >= scroll.TopRoll.yMax + 44f, $"title scroll keeps heading space above its first action at {size.x}x{size.y}");
                        Rect testingButton = TavernScreenLayout.TestingButtonRect(
                            saveExists,
                            geometry.Menu.width,
                            developerTestingVisible);
                        AssertEqual(
                            developerTestingVisible,
                            testingButton.width > 0f && testingButton.height > 0f,
                            $"tavern beta testing geometry mirrors developer visibility at {size.x}x{size.y} save={saveExists}");
                        if (developerTestingVisible)
                        {
                            AssertEqual(true, testingButton.xMin >= 0f && testingButton.yMin >= 0f && testingButton.xMax <= geometry.Menu.width && testingButton.yMax <= geometry.Menu.height, $"tavern beta testing button fits menu {size.x}x{size.y} save={saveExists}");
                            AssertEqual(true, testingButton.height >= 44f, $"development Beta Lab row keeps the same readable hit target at {size.x}x{size.y}");
                        }
                        for (int i = 0; i < buttons.Count; i++)
                        {
                            Rect button = buttons[i];
                            AssertEqual(true, button.xMin >= 0f && button.yMin >= 0f && button.xMax <= geometry.Menu.width && button.yMax <= geometry.Menu.height, $"tavern menu choice {i} fits at {size.x}x{size.y}");
                            AssertEqual(true, scroll.ContainsContent(button), $"tavern menu choice {i} stays on the parchment sheet at {size.x}x{size.y}");
                            AssertEqual(true, button.height >= 44f, $"tavern menu choice {i} keeps a readable hit target at {size.x}x{size.y}");
                            if (developerTestingVisible)
                            {
                                AssertEqual(false, button.Overlaps(testingButton), $"tavern menu choice {i} stays clear of developer testing at {size.x}x{size.y}");
                            }
                            for (int other = i + 1; other < buttons.Count; other++)
                            {
                                AssertEqual(false, button.Overlaps(buttons[other]), $"tavern choices {i} and {other} remain separated at {size.x}x{size.y}");
                            }
                        }
                        Rect lastButton = buttons[buttons.Count - 1];
                        if (developerTestingVisible)
                        {
                            AssertEqual(true, scroll.ContainsContent(testingButton), $"developer testing stays on the parchment sheet at {size.x}x{size.y}");
                            AssertEqual(true, lastButton.yMax + 8f <= testingButton.yMin, $"tavern choices keep a clear developer-testing gutter at {size.x}x{size.y}");
                            AssertEqual(true, testingButton.yMax + 6f <= scroll.BottomRoll.yMin, $"developer testing stays clear of the lower scroll roll at {size.x}x{size.y}");
                            AssertEqual(true, testingButton.yMax + 6f <= scroll.Bounds.yMax - TitleScreenPresentationRules.MenuScrollBottomInset, $"developer testing clears the authored lower roller at {size.x}x{size.y}");
                        }
                        else
                        {
                            AssertEqual(true, lastButton.yMax + 6f <= scroll.BottomRoll.yMin, $"tavern choices stay clear of the lower scroll roll at {size.x}x{size.y}");
                            AssertEqual(true, lastButton.yMax + 6f <= scroll.Bounds.yMax - TitleScreenPresentationRules.MenuScrollBottomInset, $"tavern choices clear the authored lower roller at {size.x}x{size.y}");
                        }
                    }
                }
            }
        }

        private static void TavernStormRegionsStayOutsideTheRoom()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(960, 600),
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
                            window.xMin >= size.x * 0.52f
                                && window.xMax <= size.x * 0.84f
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
            AssertEqual(true, settled.MenuAlpha > 0.99f && settled.MenuRise < 0.01f, "Grand Hearth menu settles fully legible and at rest");
            AssertEqual(true, TitleScreenPresentationRules.MenuInteractive(settled), "settled Grand Hearth menu accepts navigation input");
            AssertEqual(1f, reduced.BackdropReveal, "reduced motion reveals the Grand Hearth immediately");
            AssertEqual(1f, reduced.MenuAlpha, "reduced motion reveals the title menu immediately");
            AssertEqual(true, TitleScreenPresentationRules.MenuInteractive(reduced), "reduced motion keeps the title menu immediately usable");
            AssertEqual(0f, reduced.MenuRise, "reduced motion removes menu travel");
            AssertEqual(true, TitleScreenPresentationRules.CrossedCue(0.1f, 0.3f, TitleScreenPresentationRules.RevealStrikeAt, false, false), "Grand Hearth forge strike crosses once");
            AssertEqual(false, TitleScreenPresentationRules.CrossedCue(0.1f, 0.3f, TitleScreenPresentationRules.RevealStrikeAt, true, false), "reduced motion suppresses the title strike cue");
            AssertEqual(false, TitleScreenPresentationRules.CrossedCue(0.1f, 0.3f, TitleScreenPresentationRules.RevealStrikeAt, false, true), "played title strike cues do not repeat");
            TitleHearthFlickerFrame firstFlicker = TitleScreenPresentationRules.EvaluateHearthFlicker(0.5f, false);
            TitleHearthFlickerFrame repeatedFlicker = TitleScreenPresentationRules.EvaluateHearthFlicker(0.5f, false);
            TitleHearthFlickerFrame laterFlicker = TitleScreenPresentationRules.EvaluateHearthFlicker(0.75f, false);
            TitleHearthFlickerFrame reducedFlicker = TitleScreenPresentationRules.EvaluateHearthFlicker(0f, true);
            TitleHearthFlickerFrame reducedLater = TitleScreenPresentationRules.EvaluateHearthFlicker(30f, true);
            AssertEqual(firstFlicker.RoomGlow, repeatedFlicker.RoomGlow, "title hearth flicker is deterministic at the same instant");
            AssertEqual(firstFlicker.FireboxGlow, repeatedFlicker.FireboxGlow, "title firebox flicker is deterministic at the same instant");
            AssertEqual(true, Mathf.Abs(firstFlicker.FireboxGlow - laterFlicker.FireboxGlow) >= 0.08f, "title firebox visibly flickers between nearby frames");
            AssertEqual(true, firstFlicker.RoomGlow >= 0.42f && firstFlicker.RoomGlow <= 0.50f, "title hearth room bloom stays restrained");
            AssertEqual(true, firstFlicker.FireboxGlow >= 0.44f && firstFlicker.FireboxGlow <= 0.74f, "title firebox flicker stays warmly bounded");
            AssertEqual(reducedFlicker.RoomGlow, reducedLater.RoomGlow, "reduced motion freezes the room bloom");
            AssertEqual(reducedFlicker.FireboxGlow, reducedLater.FireboxGlow, "reduced motion freezes the firebox flicker");
            TitleMenuFocusHonorsReducedMotion();
            TitleMenuScrollStyle scrollStyle = TitleScreenPresentationRules.MenuScrollStyle;
            AssertEqual(true, scrollStyle.Paper.a >= 0.90f && scrollStyle.Roll.a >= 0.90f, "title scroll parchment and rolls remain opaque");
            AssertEqual(true, scrollStyle.Paper.r > scrollStyle.Paper.b, "title scroll parchment remains visibly warm");
            AssertEqual(true, Mathf.Abs(TitleScreenPresentationRules.RelativeLuminance(scrollStyle.Paper) - TitleScreenPresentationRules.RelativeLuminance(scrollStyle.Roll)) >= 0.04f, "title scroll rolls remain distinct from the sheet");
            AssertEqual(true, TitleScreenPresentationRules.ContrastRatio(scrollStyle.Ink, scrollStyle.Paper) >= 4.5f, "title scroll ink remains readable on parchment");
            AssertEqual(true, TitleScreenPresentationRules.ContrastRatio(scrollStyle.Selection, scrollStyle.Paper) >= 3f, "title scroll selection remains distinct from parchment");
            AssertEqual(true, TitleScreenPresentationRules.ContrastRatio(scrollStyle.SelectionInk, scrollStyle.Selection) >= 4.5f, "title scroll focused copy remains readable on its leather ribbon");
            float scrollPixelsPerCanvasPixel = TitleScreenPresentationRules.MenuScrollPixelsPerUnit
                / TitleScreenPresentationRules.MenuScrollReferencePixelsPerUnit;
            float focusPixelsPerCanvasPixel = TitleScreenPresentationRules.MenuFocusPixelsPerUnit
                / TitleScreenPresentationRules.MenuScrollReferencePixelsPerUnit;
            float scrollSideCap = TitleScreenPresentationRules.MenuScrollSpriteBorder.x / scrollPixelsPerCanvasPixel;
            float focusSideCaps = (TitleScreenPresentationRules.MenuFocusSpriteBorder.x + TitleScreenPresentationRules.MenuFocusSpriteBorder.z)
                / focusPixelsPerCanvasPixel;
            TavernScreenGeometry narrowMenu = TavernScreenLayout.Calculate(1024f, 768f, true);
            TavernMenuScrollGeometry narrowScroll = TavernScreenLayout.ScrollGeometry(narrowMenu.Menu.width, narrowMenu.Menu.height);
            AssertEqual(true, scrollSideCap * 2f + 96f <= narrowMenu.Menu.width, "authored charter keeps a stretchable center at the narrowest supported menu");
            AssertEqual(true, focusSideCaps + 48f <= narrowScroll.Content.width, "authored focus ribbon keeps readable center space at the narrowest supported menu");
            TitleMenuArtMatchesRuntimeContract();
            int[] menuIcons = { 0, 1, 2, 3, 4 };
            int[] legacyMenuIcons = { 7, 2, 4, 5, 8 };
            TitleMenuChoiceKind[] menuKinds =
            {
                TitleMenuChoiceKind.Continue,
                TitleMenuChoiceKind.NewGame,
                TitleMenuChoiceKind.Settings,
                TitleMenuChoiceKind.Exit,
                TitleMenuChoiceKind.BetaLab
            };
            for (int i = 0; i < menuKinds.Length; i++)
            {
                AssertEqual(menuIcons[i], TitleScreenPresentationRules.MenuIconIndex(menuKinds[i]), $"Grand Hearth {menuKinds[i]} relic icon is pinned");
                AssertEqual(true, menuIcons[i] >= 0 && menuIcons[i] < TitleScreenPresentationRules.MenuIconColumns, $"Grand Hearth {menuKinds[i]} glyph fits the dedicated 5x1 atlas");
                AssertEqual(legacyMenuIcons[i], TitleScreenPresentationRules.LegacyMenuIconIndex(menuKinds[i]), $"Grand Hearth {menuKinds[i]} keeps its legacy fallback crop");
                for (int other = i + 1; other < menuIcons.Length; other++)
                {
                    AssertEqual(false, menuIcons[i] == menuIcons[other], $"Grand Hearth {menuKinds[i]} and {menuKinds[other]} keep distinct relic icons");
                }
            }

            foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
            {
                TitleBackdropProjection cover = TitleScreenPresentationRules.ProjectBackdrop(size.x, size.y, 1672f, 941f);
                AssertEqual(true, cover.CoverRect.xMin <= 0f && cover.CoverRect.yMin <= 0f && cover.CoverRect.xMax >= size.x && cover.CoverRect.yMax >= size.y, $"Grand Hearth painting covers {size.x}x{size.y} without letterboxing");
                AssertEqual(true, Mathf.Abs(cover.CoverRect.center.x - size.x * 0.5f) < 0.01f && Mathf.Abs(cover.CoverRect.center.y - size.y * 0.5f) < 0.01f, $"Grand Hearth crop stays centered at {size.x}x{size.y}");
                AssertEqual(true, Mathf.Abs(cover.CoverRect.width / cover.CoverRect.height - 1672f / 941f) < 0.001f, $"Grand Hearth painting preserves aspect at {size.x}x{size.y}");
            }
        }

        private static void TitleMenuFocusHonorsReducedMotion()
        {
            TitleMenuFocusFrame opening = TitleScreenPresentationRules.EvaluateMenuFocus(0f, false);
            TitleMenuFocusFrame later = TitleScreenPresentationRules.EvaluateMenuFocus(0.5f, false);
            TitleMenuFocusFrame reduced = TitleScreenPresentationRules.EvaluateMenuFocus(0f, true);
            TitleMenuFocusFrame reducedLater = TitleScreenPresentationRules.EvaluateMenuFocus(30f, true);

            AssertEqual(true, Mathf.Abs(opening.CursorAlpha - later.CursorAlpha) > 0.05f, "normal title focus keeps its visible alpha pulse");
            AssertEqual(true, Mathf.Abs(opening.CursorScale - later.CursorScale) > 0.004f, "normal title focus keeps its subtle scale pulse");
            AssertEqual(reduced.CursorAlpha, reducedLater.CursorAlpha, "Reduced Motion freezes title focus alpha");
            AssertEqual(reduced.CursorScale, reducedLater.CursorScale, "Reduced Motion freezes title focus scale");
            AssertEqual(true, reduced.CursorAlpha >= 0.72f && reduced.CursorAlpha <= 0.90f, "static Reduced Motion focus remains clearly visible");
            AssertEqual(true, reduced.CursorScale >= 0.98f && reduced.CursorScale <= 1.02f, "static Reduced Motion focus retains its settled scale");
        }

        private static void TitleMenuArtMatchesRuntimeContract()
        {
            Texture2D scroll = null;
            Texture2D focus = null;
            Texture2D icons = null;
            try
            {
                scroll = LoadApprovedRuntimeAtlas(RuntimeArtManifest.TitleMenuScroll);
                focus = LoadApprovedRuntimeAtlas(RuntimeArtManifest.TitleMenuFocus);
                icons = LoadApprovedRuntimeAtlas(RuntimeArtManifest.TitleMenuIconAtlas);
                AssertEqual(true, TitleScreenPresentationRules.SupportsMenuScrollArt(scroll), "authored title charter uses its exact 1280-square source contract");
                AssertEqual(true, TitleScreenPresentationRules.SupportsMenuFocusArt(focus), "authored title focus ribbon uses its exact 2048x768 source contract");
                AssertEqual(true, TitleScreenPresentationRules.SupportsMenuIconArt(icons), "title choices use the exact dedicated 1280x256 glyph strip");
                AssertEqual(new Vector4(360f, 360f, 360f, 360f), TitleScreenPresentationRules.MenuScrollSpriteBorder, "authored title charter keeps its quiet-center nine-slice");
                AssertEqual(new Rect(0f, 192f, 2048f, 384f), TitleScreenPresentationRules.MenuFocusSpriteRect, "authored title focus ribbon crops away transparent vertical authoring space");
                AssertEqual(new Vector4(360f, 96f, 360f, 96f), TitleScreenPresentationRules.MenuFocusSpriteBorder, "authored title focus ribbon keeps ornamental ends outside its stretchable center");

                Color32[] scrollPixels = scroll.GetPixels32();
                int scrollTransparent = 0;
                int scrollVisible = 0;
                int chromaResidue = 0;
                for (int i = 0; i < scrollPixels.Length; i++)
                {
                    Color32 pixel = scrollPixels[i];
                    if (pixel.a < 32) scrollTransparent++;
                    else scrollVisible++;
                    if (pixel.a >= 8 && pixel.g >= 245 && pixel.r <= 24 && pixel.b <= 24) chromaResidue++;
                }
                AssertEqual(true, scrollTransparent >= scrollPixels.Length * 0.20f && scrollVisible >= scrollPixels.Length * 0.52f, "authored title charter keeps a transparent silhouette and substantial readable vellum");
                AssertEqual(0, chromaResidue, "authored title charter contains no visible generation-key residue");

                int gutterSamples = 0;
                int transparentGutterSamples = 0;
                const int gutter = 8;
                for (int y = 0; y < scroll.height; y++)
                {
                    for (int x = 0; x < scroll.width; x++)
                    {
                        if (x >= gutter && x < scroll.width - gutter && y >= gutter && y < scroll.height - gutter) continue;
                        gutterSamples++;
                        if (scrollPixels[y * scroll.width + x].a < 8) transparentGutterSamples++;
                    }
                }
                AssertEqual(true, transparentGutterSamples >= gutterSamples * 0.995f, "authored title charter keeps a clean transparent outer gutter");

                int centerVisible = 0;
                int centerSamples = 0;
                for (int y = 420; y < 860; y++)
                {
                    for (int x = 420; x < 860; x++)
                    {
                        centerSamples++;
                        if (scrollPixels[y * scroll.width + x].a >= 220) centerVisible++;
                    }
                }
                AssertEqual(true, centerVisible >= centerSamples * 0.98f, "authored title charter keeps an opaque quiet writing field");

                Color32[] focusPixels = focus.GetPixels32();
                int focusTransparent = focusPixels.Count(pixel => pixel.a < 32);
                int focusVisible = focusPixels.Count(pixel => pixel.a >= 32);
                AssertEqual(true, focusTransparent >= focusPixels.Length * 0.45f && focusVisible >= focusPixels.Length * 0.20f, "authored focus ribbon keeps clean alpha around a substantial plaque silhouette");

                Color32[] iconPixels = icons.GetPixels32();
                int cellWidth = icons.width / TitleScreenPresentationRules.MenuIconColumns;
                HashSet<ulong> iconFingerprints = new HashSet<ulong>();
                for (int cell = 0; cell < TitleScreenPresentationRules.MenuIconColumns; cell++)
                {
                    int visible = 0;
                    int iconGutterSamples = 0;
                    int transparentIconGutter = 0;
                    ulong fingerprint = 1469598103934665603UL;
                    for (int y = 0; y < icons.height; y++)
                    {
                        for (int localX = 0; localX < cellWidth; localX++)
                        {
                            Color32 pixel = iconPixels[y * icons.width + cell * cellWidth + localX];
                            if (pixel.a >= 32) visible++;
                            if (localX < 16 || localX >= cellWidth - 16 || y < 16 || y >= icons.height - 16)
                            {
                                iconGutterSamples++;
                                if (pixel.a < 8) transparentIconGutter++;
                            }
                            unchecked
                            {
                                fingerprint ^= pixel.r;
                                fingerprint *= 1099511628211UL;
                                fingerprint ^= pixel.g;
                                fingerprint *= 1099511628211UL;
                                fingerprint ^= pixel.b;
                                fingerprint *= 1099511628211UL;
                                fingerprint ^= pixel.a;
                                fingerprint *= 1099511628211UL;
                            }
                        }
                    }

                    int cellPixels = cellWidth * icons.height;
                    AssertEqual(true, visible >= cellPixels * 0.30f && visible <= cellPixels * 0.50f, $"title glyph {cell} keeps a bold but uncrowded silhouette");
                    AssertEqual(true, transparentIconGutter >= iconGutterSamples * 0.995f, $"title glyph {cell} keeps a clean transparent scaling gutter");
                    AssertEqual(true, iconFingerprints.Add(fingerprint), $"title glyph {cell} is visually distinct from the other scroll actions");
                }
            }
            finally
            {
                if (scroll != null) UnityEngine.Object.DestroyImmediate(scroll);
                if (focus != null) UnityEngine.Object.DestroyImmediate(focus);
                if (icons != null) UnityEngine.Object.DestroyImmediate(icons);
            }
        }

        private static void TitleAudioRulesKeepMusicAndMenuFeedbackLegible()
        {
            TitleAudioCueProfile strike = TitleAudioRules.PresentationCue("impactlow", 0.14f);
            TitleAudioCueProfile reveal = TitleAudioRules.PresentationCue("uiconfirm", 0.16f);
            TitleAudioCueProfile focus = TitleAudioRules.PresentationCue("uitab", 0.18f);
            AssertEqual(TitleAudioRules.RevealStrikeKey, strike.Key, "Grand Hearth reveal uses its laptop-readable forge strike");
            AssertEqual(0.28f, strike.Volume, "Grand Hearth forge strike keeps its authored mix gain");
            AssertEqual(TitleAudioRules.RevealChimeKey, reveal.Key, "Grand Hearth reveal chime is distinct from generic confirmation");
            AssertEqual(0.22f, reveal.Volume, "Grand Hearth reveal chime keeps its authored mix gain");
            AssertEqual(TitleAudioRules.FocusKey, focus.Key, "Grand Hearth focus movement uses its dedicated dry tick");
            AssertEqual(0.20f, focus.Volume, "Grand Hearth focus tick stays restrained");

            AssertEqual(TitleAudioRules.ConfirmKey, TitleAudioRules.MenuCue(TitleMenuAudioAction.Confirm).Key, "title activation has a semantic confirmation cue");
            AssertEqual(TitleAudioRules.OpenKey, TitleAudioRules.MenuCue(TitleMenuAudioAction.Open).Key, "title panel opening has a semantic cue");
            AssertEqual(TitleAudioRules.CloseKey, TitleAudioRules.MenuCue(TitleMenuAudioAction.Close).Key, "title panel closing has a semantic cue");
            AssertEqual("blocked", TitleAudioRules.MenuCue(TitleMenuAudioAction.Blocked).Key, "unavailable title actions retain blocked feedback");

            AssertEqual(true, TitleAudioRules.MusicSourceGain(GameMode.Tavern) > TitleAudioRules.MusicSourceGain(GameMode.Muster), "title overture opens above the muster score gain");
            AssertEqual(true, TitleAudioRules.MusicSourceGain(GameMode.Muster) > TitleAudioRules.MusicSourceGain(GameMode.Explore), "muster transition steps down smoothly toward gameplay gain");
            AssertEqual(true, TitleAudioRules.MusicSourceGain(GameMode.Tavern) <= 0.32f, "title overture gain retains mix headroom");
            AssertEqual(true, TitleAudioRules.MusicSourceGain(GameMode.Combat) <= 0.22f, "combat score keeps headroom for impact layers");
            AssertEqual(true, TitleAudioRules.MusicSourceGain(GameMode.Explore, true) < TitleAudioRules.MusicSourceGain(GameMode.Explore), "World Map score sits beneath route and ambience feedback");
            AssertEqual("ambhearth", TitleAudioRules.HearthAmbienceKey, "Grand Hearth moments cannot alias the dry combat fire impact");

            float audibleDelay = TitleAudioRules.InitialAmbienceDelay(GameMode.Tavern, true);
            float mutedDelay = TitleAudioRules.InitialAmbienceDelay(GameMode.Tavern, false);
            TitleAmbienceProfile audibleBed = TitleAudioRules.Ambience(GameMode.Tavern, true, 0);
            TitleAmbienceProfile audibleHearth = TitleAudioRules.Ambience(GameMode.Tavern, true, 1);
            TitleAmbienceProfile mutedBed = TitleAudioRules.Ambience(GameMode.Tavern, false, 0);
            AssertEqual(true, audibleDelay >= 7.5f && mutedDelay < audibleDelay, "authored title opening receives an undoubled reveal window");
            AssertEqual(true, TitleAudioRules.AmbienceInterval(GameMode.Tavern, true, 0) >= 10.5f, "audible title music keeps secondary ambience sparse");
            AssertEqual("ambtavern", audibleBed.Key, "audible title music avoids a duplicate rain layer");
            AssertEqual("ambhearth", audibleHearth.Key, "audible title music retains a quiet hearth detail");
            AssertEqual(true, audibleBed.Volume <= 0.075f && audibleHearth.Volume <= 0.075f, "secondary title ambience stays beneath the score");
            AssertEqual(true, mutedBed.Volume > audibleBed.Volume, "muting music restores a fuller environmental bed");

            AssertEqual(true, TitleAudioRules.LocksPitch(TitleAudioRules.RevealStrikeKey), "title reveal strike is pitch locked to its master");
            AssertEqual(true, TitleAudioRules.LocksPitch(TitleAudioRules.ConfirmKey), "title confirmation is pitch locked to the overture");
            AssertEqual(false, TitleAudioRules.LocksPitch("ui"), "generic UI feedback retains its normal variation");
        }

        private static void PartySetupScreenLayoutFitsSupportedResolutions()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(960, 600),
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
                new Vector2Int(960, 600),
                new Vector2Int(1024, 768),
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
                AssertEqual(5, commandLayout.Commands.Count, "exploration fallback keeps one contextual action and four core navigation commands");
                AssertEqual(true, commandLayout.Action.width >= commandLayout.Map.width * 2f, $"exploration context action remains visually dominant {size.x}x{size.y}");
                AssertEqual(true, commandLayout.Action.xMax < commandLayout.SeparatorX && commandLayout.SeparatorX < commandLayout.Map.xMin, $"exploration context and navigation commands stay separated {size.x}x{size.y}");
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
                    AssertEqual(5, buttons.Length, "exploration HUD keeps Interact, Map, Journal, Party, and Menu visible");
                    foreach (Rect button in buttons)
                    {
                        AssertEqual(true, button.xMin >= 0f && button.yMin >= 0f && button.xMax <= geometry.Command.width && button.yMax <= geometry.Command.height, $"exploration command button fits {size.x}x{size.y}");
                        AssertEqual(true, button.height >= 52f * uiScale - 0.01f, $"exploration persistent command meets readable height {size.x}x{size.y}");
                    }
                    Rect detailsButton = ExplorationHudScreenLayout.DetailsButton(geometry.Side.width, geometry.Side.height, uiScale);
                    Rect[] partyRows = ExplorationHudScreenLayout.PartyRows(geometry.Side.width, uiScale, detailsOpen, 4);
                    AssertEqual(true, detailsButton.yMax <= geometry.Side.height + 0.01f, $"exploration details control stays inside the rail {size.x}x{size.y} details={detailsOpen}");
                    AssertEqual(true, partyRows.All(row => row.xMin >= 0f && row.xMax <= geometry.Side.width && row.yMin >= 0f && row.yMax <= detailsButton.yMin - 4f * uiScale + 0.01f), $"exploration party readiness stays above Details at {size.x}x{size.y} details={detailsOpen}");

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
                        AssertEqual(true, rail.PartyCount >= 1 && rail.PartyCount <= 4, $"exploration fallback keeps a bounded party-readiness sample {size.x}x{size.y} details={detailsOpen} action={hasAction}");
                        AssertEqual(true, contentStart + rail.UsedHeight <= contentBottom + 0.01f, $"exploration fallback party rows end above the details control {size.x}x{size.y} details={detailsOpen} action={hasAction}");
                        AssertEqual(true, rail.ObjectiveMaxLines >= 1, $"exploration fallback objective retains readable copy space {size.x}x{size.y} details={detailsOpen} action={hasAction}");

                        float copyWidth = Mathf.Max(40f * uiScale, sideInnerWidth - 18f * uiScale);
                        string boundedObjective = ExplorationHudFallbackLayoutRules.BoundedCopy(outerRouteObjective, copyWidth, bodyFontSize, rail.ObjectiveMaxLines);
                        AssertEqual(true, !string.IsNullOrWhiteSpace(boundedObjective), $"exploration fallback objective remains visible {size.x}x{size.y} details={detailsOpen} action={hasAction}");
                        AssertEqual(true, ExplorationHudFallbackLayoutRules.EstimatedWrappedLines(boundedObjective, copyWidth, bodyFontSize) <= rail.ObjectiveMaxLines, $"exploration fallback objective never clips {size.x}x{size.y} details={detailsOpen} action={hasAction}");
                        bool objectiveNeedsBounding = ExplorationHudFallbackLayoutRules.EstimatedWrappedLines(outerRouteObjective, copyWidth, bodyFontSize) > rail.ObjectiveMaxLines;
                        AssertEqual(objectiveNeedsBounding, boundedObjective.EndsWith("\u2026", StringComparison.Ordinal), $"exploration fallback makes bounded objective copy explicit {size.x}x{size.y} details={detailsOpen} action={hasAction}");
                    }
                }
            }
            RegionMapNavigationRulesKeepBrowsingBounded();
            ExplorationMovementRepeatRulesKeepTravelCardinalAndBounded();
        }

        private static void RegionMapNavigationRulesKeepBrowsingBounded()
        {
            const int mapWidth = 58;
            const int mapHeight = 46;
            const int viewWidth = 17;
            const int viewHeight = 9;

            Point center = RegionMapNavigationRules.ClampFocus(29, 24, mapWidth, mapHeight);
            AssertEqual(29, center.X, "Region Map keeps an in-bounds horizontal focus");
            AssertEqual(24, center.Y, "Region Map keeps an in-bounds vertical focus");
            Point clamped = RegionMapNavigationRules.ClampFocus(-12, 99, mapWidth, mapHeight);
            AssertEqual(0, clamped.X, "Region Map focus clamps at the west edge");
            AssertEqual(mapHeight - 1, clamped.Y, "Region Map focus clamps at the south edge");

            Point centeredOrigin = RegionMapNavigationRules.ViewportOrigin(
                center.X,
                center.Y,
                viewWidth,
                viewHeight,
                mapWidth,
                mapHeight);
            AssertEqual(21, centeredOrigin.X, "Region Map viewport centers horizontally on browse focus");
            AssertEqual(20, centeredOrigin.Y, "Region Map viewport centers vertically on browse focus");
            Point northwestOrigin = RegionMapNavigationRules.ViewportOrigin(0, 0, viewWidth, viewHeight, mapWidth, mapHeight);
            Point southeastOrigin = RegionMapNavigationRules.ViewportOrigin(99, 99, viewWidth, viewHeight, mapWidth, mapHeight);
            AssertEqual(0, northwestOrigin.X, "Region Map viewport stops at the west edge");
            AssertEqual(0, northwestOrigin.Y, "Region Map viewport stops at the north edge");
            AssertEqual(mapWidth - viewWidth, southeastOrigin.X, "Region Map viewport stops at the east edge");
            AssertEqual(mapHeight - viewHeight, southeastOrigin.Y, "Region Map viewport stops at the south edge");

            RegionMapNavigationStep initial = RegionMapNavigationRules.ResolveAxes(1f, 1f, 0, 0, 0f, 10f);
            AssertEqual(1, initial.DeltaX, "rightward left-stick axis intent pans Region Map east once");
            AssertEqual(-1, initial.DeltaY, "up stick-axis intent pans Region Map north once");
            AssertEqual(true, Mathf.Approximately(10f + RegionMapNavigationRules.InitialRepeatDelay, initial.NextRepeatAt), "Region Map initial axis repeat delay is deterministic");
            RegionMapNavigationStep held = RegionMapNavigationRules.ResolveAxes(
                1f,
                1f,
                initial.HeldX,
                initial.HeldY,
                initial.NextRepeatAt,
                initial.NextRepeatAt - 0.01f);
            AssertEqual(false, held.Moved, "held Region Map axis waits through its initial repeat delay");
            RegionMapNavigationStep repeated = RegionMapNavigationRules.ResolveAxes(
                1f,
                1f,
                held.HeldX,
                held.HeldY,
                held.NextRepeatAt,
                held.NextRepeatAt);
            AssertEqual(true, repeated.Moved, "held Region Map axis repeats after its deterministic delay");
            AssertEqual(true, Mathf.Approximately(held.NextRepeatAt + RegionMapNavigationRules.RepeatInterval, repeated.NextRepeatAt), "Region Map repeat cadence stays deterministic");
            RegionMapNavigationStep released = RegionMapNavigationRules.ResolveAxes(0f, 0f, repeated.HeldX, repeated.HeldY, repeated.NextRepeatAt, 11f);
            AssertEqual(0, released.HeldX, "released horizontal Region Map axis clears held state");
            AssertEqual(0, released.HeldY, "released vertical Region Map axis clears held state");

            RegionMapPointerPanStep drag = RegionMapNavigationRules.ResolvePointerDrag(-40f, 20f, 20f, 0f, 0f);
            AssertEqual(2, drag.DeltaX, "dragging the Region Map left pans focus two cells east under grab-map semantics");
            AssertEqual(-1, drag.DeltaY, "dragging the Region Map down pans focus one cell north under grab-map semantics");
            RegionMapPointerPanStep partialDrag = RegionMapNavigationRules.ResolvePointerDrag(-7f, 5f, 20f, 0f, 0f);
            AssertEqual(0, partialDrag.DeltaX, "sub-cell pointer drag waits instead of jittering the Region Map");
            AssertEqual(0, partialDrag.DeltaY, "sub-cell vertical pointer drag waits instead of jittering the Region Map");
            RegionMapPointerPanStep completedDrag = RegionMapNavigationRules.ResolvePointerDrag(-13f, 15f, 20f, partialDrag.RemainderX, partialDrag.RemainderY);
            AssertEqual(1, completedDrag.DeltaX, "accumulated pointer drag advances exactly one horizontal cell");
            AssertEqual(-1, completedDrag.DeltaY, "accumulated pointer drag advances exactly one vertical cell");
            Point verticalWheel = RegionMapNavigationRules.ScrollDelta(1f, false);
            Point horizontalWheel = RegionMapNavigationRules.ScrollDelta(-1f, true);
            AssertEqual(1, verticalWheel.Y, "wheel down pans Region Map south");
            AssertEqual(-1, horizontalWheel.X, "Shift-wheel up pans Region Map west");

            HelpOverlayView help = HelpOverlayContent.Build(GameMode.Explore, false, 6, "Midgaard");
            AssertEqual(true, help.Lines.Any(line => line.IndexOf("left stick", StringComparison.OrdinalIgnoreCase) >= 0), "exploration help names the configured Region Map controller axis");
            AssertEqual(true, help.Lines.Any(line => line.IndexOf("drag", StringComparison.OrdinalIgnoreCase) >= 0 && line.IndexOf("wheel", StringComparison.OrdinalIgnoreCase) >= 0), "exploration help names Region Map pointer panning");
            AssertEqual(true, help.Lines.Any(line => line.IndexOf("Home", StringComparison.OrdinalIgnoreCase) >= 0 && line.IndexOf("gamepad X", StringComparison.OrdinalIgnoreCase) >= 0), "exploration help names both Region Map recenter controls");
        }

        private static void ExplorationMovementRepeatRulesKeepTravelCardinalAndBounded()
        {
            ExplorationMovementRepeatStep belowThreshold = ExplorationMovementRepeatRules.ResolveAxes(
                ExplorationMovementRepeatRules.AxisThreshold - 0.001f,
                -(ExplorationMovementRepeatRules.AxisThreshold - 0.001f),
                0,
                0,
                0f,
                10f);
            AssertEqual(false, belowThreshold.HasAction, "sub-threshold exploration axes remain neutral");
            AssertEqual(0, belowThreshold.HeldX, "sub-threshold horizontal intent leaves no held travel direction");
            AssertEqual(0, belowThreshold.HeldY, "sub-threshold vertical intent leaves no held travel direction");

            ExplorationMovementRepeatStep exactHorizontal = ExplorationMovementRepeatRules.ResolveAxes(
                ExplorationMovementRepeatRules.AxisThreshold,
                0f,
                0,
                0,
                0f,
                10f);
            AssertEqual(1, exactHorizontal.DeltaX, "the exact positive exploration threshold travels east");
            AssertEqual(0, exactHorizontal.DeltaY, "horizontal threshold travel remains cardinal");
            AssertEqual(true, exactHorizontal.IsInitialOrDirectionChange, "a threshold crossing is an initial travel action");
            AssertEqual(false, exactHorizontal.IsHeldRepeat, "a threshold crossing is not mislabeled as a held repeat");
            AssertEqual(
                true,
                Mathf.Approximately(10f + ExplorationMovementRepeatRules.InitialRepeatDelay, exactHorizontal.NextRepeatAt),
                "initial exploration travel arms its deterministic repeat delay");

            ExplorationMovementRepeatStep exactVertical = ExplorationMovementRepeatRules.ResolveAxes(
                0f,
                -ExplorationMovementRepeatRules.AxisThreshold,
                0,
                0,
                0f,
                10f);
            AssertEqual(0, exactVertical.DeltaX, "vertical threshold travel remains cardinal");
            AssertEqual(1, exactVertical.DeltaY, "the exact negative vertical threshold travels south");

            ExplorationMovementRepeatStep horizontalDominant = ExplorationMovementRepeatRules.ResolveAxes(
                0.91f,
                0.72f,
                0,
                0,
                0f,
                20f);
            AssertEqual(1, horizontalDominant.DeltaX, "a stronger horizontal diagonal resolves east");
            AssertEqual(0, horizontalDominant.DeltaY, "a stronger horizontal diagonal never produces a second travel axis");
            ExplorationMovementRepeatStep verticalDominant = ExplorationMovementRepeatRules.ResolveAxes(
                -0.72f,
                0.91f,
                0,
                0,
                0f,
                20f);
            AssertEqual(0, verticalDominant.DeltaX, "a stronger vertical diagonal never produces a second travel axis");
            AssertEqual(-1, verticalDominant.DeltaY, "a stronger upward diagonal resolves north");

            ExplorationMovementRepeatStep tiedInitial = ExplorationMovementRepeatRules.ResolveAxes(
                1f,
                1f,
                0,
                0,
                0f,
                30f);
            AssertEqual(0, tiedInitial.DeltaX, "an exact new diagonal tie uses the stable vertical priority");
            AssertEqual(-1, tiedInitial.DeltaY, "an exact new diagonal tie remains one northbound cardinal step");
            ExplorationMovementRepeatStep tiedHeldHorizontal = ExplorationMovementRepeatRules.ResolveAxes(
                1f,
                1f,
                1,
                0,
                31f,
                30f);
            AssertEqual(0, tiedHeldHorizontal.DeltaX, "a held horizontal tie waits until its existing deadline");
            AssertEqual(0, tiedHeldHorizontal.DeltaY, "a held horizontal tie cannot leak into vertical travel");
            AssertEqual(1, tiedHeldHorizontal.HeldX, "an exact diagonal tie preserves its matching held horizontal direction");
            AssertEqual(0, tiedHeldHorizontal.HeldY, "a preserved horizontal tie remains cardinal");

            ExplorationMovementRepeatStep noisyDiagonalStart = ExplorationMovementRepeatRules.ResolveAxes(
                0.72f,
                0.70f,
                0,
                0,
                0f,
                32f);
            AssertEqual(1, noisyDiagonalStart.DeltaX, "a slightly east-dominant stick begins eastbound travel");
            AssertEqual(0, noisyDiagonalStart.DeltaY, "a slightly east-dominant stick remains cardinal");
            ExplorationMovementRepeatStep noisyDiagonalFlip = ExplorationMovementRepeatRules.ResolveAxes(
                0.70f,
                0.72f,
                noisyDiagonalStart.HeldX,
                noisyDiagonalStart.HeldY,
                noisyDiagonalStart.NextRepeatAt,
                32.05f);
            AssertEqual(false, noisyDiagonalFlip.HasAction, "near-diagonal stick noise cannot bypass the held deadline");
            AssertEqual(1, noisyDiagonalFlip.HeldX, "near-diagonal stick noise preserves the established horizontal direction");
            AssertEqual(0, noisyDiagonalFlip.HeldY, "near-diagonal stick noise cannot alternate travel axes");
            AssertEqual(
                true,
                Mathf.Approximately(noisyDiagonalStart.NextRepeatAt, noisyDiagonalFlip.NextRepeatAt),
                "near-diagonal stick noise preserves the existing repeat deadline");
            ExplorationMovementRepeatStep noisyDiagonalReturn = ExplorationMovementRepeatRules.ResolveAxes(
                0.72f,
                0.70f,
                noisyDiagonalFlip.HeldX,
                noisyDiagonalFlip.HeldY,
                noisyDiagonalFlip.NextRepeatAt,
                32.10f);
            AssertEqual(false, noisyDiagonalReturn.HasAction, "alternating near-diagonal samples remain cadence-bound");
            AssertEqual(1, noisyDiagonalReturn.HeldX, "alternating near-diagonal samples retain horizontal ownership");
            ExplorationMovementRepeatStep decisiveDiagonalChange = ExplorationMovementRepeatRules.ResolveAxes(
                0.62f,
                0.75f,
                noisyDiagonalReturn.HeldX,
                noisyDiagonalReturn.HeldY,
                noisyDiagonalReturn.NextRepeatAt,
                32.15f);
            AssertEqual(0, decisiveDiagonalChange.DeltaX, "a decisive analog turn releases the previous horizontal direction");
            AssertEqual(-1, decisiveDiagonalChange.DeltaY, "a decisive analog turn changes north immediately");
            AssertEqual(true, decisiveDiagonalChange.IsInitialOrDirectionChange, "a decisive analog turn re-arms the initial cadence");

            ExplorationMovementRepeatStep beforeDeadline = ExplorationMovementRepeatRules.ResolveAxes(
                1f,
                0f,
                exactHorizontal.HeldX,
                exactHorizontal.HeldY,
                exactHorizontal.NextRepeatAt,
                exactHorizontal.NextRepeatAt - 0.001f);
            AssertEqual(false, beforeDeadline.HasAction, "held exploration travel waits through its initial delay");
            AssertEqual(false, beforeDeadline.IsHeldRepeat, "a waiting held direction is not reported as a repeat action");
            AssertEqual(false, beforeDeadline.IsInitialOrDirectionChange, "a waiting held direction carries no initial-action flag");
            ExplorationMovementRepeatStep atDeadline = ExplorationMovementRepeatRules.ResolveAxes(
                1f,
                0f,
                beforeDeadline.HeldX,
                beforeDeadline.HeldY,
                beforeDeadline.NextRepeatAt,
                beforeDeadline.NextRepeatAt);
            AssertEqual(1, atDeadline.DeltaX, "held exploration travel repeats exactly at its deadline");
            AssertEqual(0, atDeadline.DeltaY, "held exploration repeat remains cardinal");
            AssertEqual(true, atDeadline.IsHeldRepeat, "a deadline action carries the held-repeat flag");
            AssertEqual(false, atDeadline.IsInitialOrDirectionChange, "a held repeat is distinct from an initial action");
            AssertEqual(
                true,
                Mathf.Approximately(beforeDeadline.NextRepeatAt + ExplorationMovementRepeatRules.RepeatInterval, atDeadline.NextRepeatAt),
                "held exploration travel advances by the exact repeat interval");

            ExplorationMovementRepeatStep directionChange = ExplorationMovementRepeatRules.ResolveAxes(
                0f,
                1f,
                atDeadline.HeldX,
                atDeadline.HeldY,
                atDeadline.NextRepeatAt,
                40f);
            AssertEqual(0, directionChange.DeltaX, "changing held direction drops the previous horizontal travel axis");
            AssertEqual(-1, directionChange.DeltaY, "changing held direction acts north immediately");
            AssertEqual(true, directionChange.IsInitialOrDirectionChange, "a direction change receives the initial-action flag");
            AssertEqual(false, directionChange.IsHeldRepeat, "a direction change is not mislabeled as a repeat");
            AssertEqual(
                true,
                Mathf.Approximately(40f + ExplorationMovementRepeatRules.InitialRepeatDelay, directionChange.NextRepeatAt),
                "a direction change re-arms the full initial delay");

            ExplorationMovementRepeatStep longFrame = ExplorationMovementRepeatRules.ResolveAxes(
                0f,
                1f,
                directionChange.HeldX,
                directionChange.HeldY,
                directionChange.NextRepeatAt,
                400f);
            AssertEqual(0, longFrame.DeltaX, "a late frame still emits no horizontal catch-up travel");
            AssertEqual(-1, longFrame.DeltaY, "a late frame emits exactly one cardinal repeat");
            AssertEqual(
                true,
                Mathf.Approximately(400f + ExplorationMovementRepeatRules.RepeatInterval, longFrame.NextRepeatAt),
                "a late frame schedules from now instead of replaying missed deadlines");

            ExplorationMovementRepeatStep released = ExplorationMovementRepeatRules.ResolveAxes(
                0f,
                0f,
                longFrame.HeldX,
                longFrame.HeldY,
                longFrame.NextRepeatAt,
                401f);
            AssertEqual(false, released.HasAction, "neutral exploration axes emit no travel action");
            AssertEqual(0, released.HeldX, "neutral exploration axes clear held horizontal state");
            AssertEqual(0, released.HeldY, "neutral exploration axes clear held vertical state");
            AssertEqual(0f, released.NextRepeatAt, "neutral exploration axes clear the repeat deadline");
            AssertEqual(false, released.IsHeldRepeat, "neutral exploration axes clear repeat flags");
            AssertEqual(false, released.IsInitialOrDirectionChange, "neutral exploration axes clear initial-action flags");

            ExplorationMovementRepeatStep nonFiniteAxes = ExplorationMovementRepeatRules.ResolveAxes(
                float.NaN,
                float.PositiveInfinity,
                1,
                0,
                12f,
                13f);
            AssertEqual(false, nonFiniteAxes.HasAction, "non-finite exploration axes fail safely to neutral");
            AssertEqual(0, nonFiniteAxes.HeldX, "non-finite exploration axes clear held horizontal state");
            AssertEqual(0, nonFiniteAxes.HeldY, "non-finite exploration axes clear held vertical state");
            AssertEqual(0f, nonFiniteAxes.NextRepeatAt, "non-finite exploration axes clear their repeat deadline");

            ExplorationMovementRepeatStep nonFiniteNow = ExplorationMovementRepeatRules.ResolveAxes(
                1f,
                0f,
                0,
                0,
                0f,
                float.NaN);
            AssertEqual(1, nonFiniteNow.DeltaX, "a non-finite clock still permits one bounded initial cardinal action");
            AssertEqual(
                true,
                Mathf.Approximately(ExplorationMovementRepeatRules.InitialRepeatDelay, nonFiniteNow.NextRepeatAt),
                "a non-finite clock falls back to a finite initial deadline");
            ExplorationMovementRepeatStep nonFiniteDeadline = ExplorationMovementRepeatRules.ResolveAxes(
                1f,
                0f,
                nonFiniteNow.HeldX,
                nonFiniteNow.HeldY,
                float.PositiveInfinity,
                50f);
            AssertEqual(false, nonFiniteDeadline.HasAction, "a non-finite held deadline cannot trigger an unbounded repeat");
            AssertEqual(1, nonFiniteDeadline.HeldX, "a non-finite deadline preserves the finite held direction");
            AssertEqual(
                true,
                Mathf.Approximately(50f + ExplorationMovementRepeatRules.InitialRepeatDelay, nonFiniteDeadline.NextRepeatAt),
                "a non-finite held deadline is repaired with the safe initial delay");
            AssertEqual(false, float.IsNaN(nonFiniteDeadline.NextRepeatAt) || float.IsInfinity(nonFiniteDeadline.NextRepeatAt), "repaired exploration deadlines remain finite");
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

            WorldMapSite[] sites = WorldMapGenerationRules.RegionalSites(width, height, startX, startY);
            WorldMapSite library = sites.Single(site => site.Id == "glass-lore-library");
            List<string> siteFlags = new List<string>
            {
                WorldSiteInteractionRules.ChartFlag(3, library.Id).ToUpperInvariant()
            };
            string siteWaypointKey = RouteChartRules.SiteWaypointKey(3, "  Glass-Lore-Library  ");
            AssertEqual("3:site:glass-lore-library", siteWaypointKey, "site waypoint keys are distinct and normalize authored ids");
            AssertEqual(true, RouteChartRules.IsSiteWaypoint(siteWaypointKey.ToUpperInvariant(), 3, library.Id), "site waypoint matching is case-insensitive");
            AssertEqual(true, RouteChartRules.IsSiteCharted(siteFlags, 3, library.Id), "site waypoint eligibility uses the depth-scoped chart flag");
            AssertEqual(false, RouteChartRules.IsSiteCharted(siteFlags, 2, library.Id), "site chart eligibility does not leak across map depths");
            AssertEqual(false, RouteChartRules.TryResolveTarget(junctions, sites, discoveries, Array.Empty<string>(), 3, siteWaypointKey, out _), "uncharted site cannot be restored as a waypoint");
            AssertEqual(true, RouteChartRules.TryResolveTarget(junctions, sites, discoveries, siteFlags, 3, siteWaypointKey.ToUpperInvariant(), out RouteChartTarget siteTarget), "charted site resolves as a typed active waypoint");
            AssertEqual(RouteChartTargetKind.Site, siteTarget.Kind, "typed waypoint identifies a regional site");
            AssertEqual(library.Name, siteTarget.Name, "typed site waypoint keeps its authored name");
            AssertEqual(library.X, siteTarget.X, "typed site waypoint keeps its authored x coordinate");
            AssertEqual(library.Y, siteTarget.Y, "typed site waypoint keeps its authored y coordinate");
            AssertEqual(siteWaypointKey, RouteChartRules.RepairWaypointKey(junctions, sites, discoveries, siteFlags, 3, siteWaypointKey.ToUpperInvariant()), "valid site waypoint repair returns the canonical key");
            AssertEqual("", RouteChartRules.RepairWaypointKey(junctions, sites, discoveries, siteFlags, 3, "3:site:missing-site"), "invalid site waypoint repairs to an empty selection");

            AssertEqual(true, RouteChartRules.TryResolveTarget(junctions, sites, discoveries, siteFlags, 3, waypointKey, out RouteChartTarget junctionTarget), "typed waypoint resolver preserves legacy junction behavior");
            AssertEqual(RouteChartTargetKind.Junction, junctionTarget.Kind, "typed waypoint identifies a legacy junction");
            AssertEqual(market.Name, junctionTarget.Name, "typed junction waypoint keeps its authored name");
            AssertEqual(market.X, junctionTarget.X, "typed junction waypoint keeps its authored x coordinate");
            AssertEqual(market.Y, junctionTarget.Y, "typed junction waypoint keeps its authored y coordinate");

            GameState waypointState = new GameState { ActiveRouteWaypointKey = waypointKey };
            GameState restoredWaypointState = JsonUtility.FromJson<GameState>(JsonUtility.ToJson(waypointState));
            AssertEqual(waypointKey, restoredWaypointState.ActiveRouteWaypointKey, "save JSON roundtrip preserves the selected route waypoint");
        }

        private static void ExplorationChartRulesTrackDurableTerrainDiscovery()
        {
            AssertEqual("1:cell:4:7", ExplorationChartRules.CellKey(0, 4, 7), "terrain chart keys clamp map depth to one");
            AssertEqual("", ExplorationChartRules.CellKey(2, -1, 7), "terrain chart keys reject negative x coordinates");
            AssertEqual("", ExplorationChartRules.CellKey(2, 4, -1), "terrain chart keys reject negative y coordinates");
            AssertEqual(true, ExplorationChartRules.IsCellKey("3:CELL:4:7"), "terrain chart key recognition is case-insensitive");
            AssertEqual(false, ExplorationChartRules.IsCellKey("3:junction:quarry-turn"), "junction discoveries are not terrain cell keys");
            AssertEqual(false, ExplorationChartRules.IsCellKey("3:cell:-1:7"), "negative terrain coordinates are malformed");
            AssertEqual(false, ExplorationChartRules.IsCellKey("3:cell:4"), "truncated terrain discoveries are malformed");

            IReadOnlyList<string> radiusZero = ExplorationChartRules.RevealKeys(0, 2, 3, 0, 8, 8);
            AssertEqual(1, radiusZero.Count, "radius-zero reveal contains only the center cell");
            AssertEqual("1:cell:2:3", radiusZero[0], "radius-zero reveal uses the normalized depth and center");

            IReadOnlyList<string> clippedCorner = ExplorationChartRules.RevealKeys(2, 0, 0, 2, 6, 6);
            AssertEqual(
                "2:cell:0:0|2:cell:1:0|2:cell:2:0|2:cell:0:1|2:cell:1:1|2:cell:0:2",
                string.Join("|", clippedCorner),
                "corner reveal clips the Manhattan diamond to in-bounds cells in row-major order");

            IReadOnlyList<string> radiusTwo = ExplorationChartRules.RevealKeys(4, 3, 3, 2, 8, 8);
            IReadOnlyList<string> repeatedRadiusTwo = ExplorationChartRules.RevealKeys(4, 3, 3, 2, 8, 8);
            AssertEqual(13, radiusTwo.Count, "radius-two reveal contains the complete thirteen-cell Manhattan diamond away from edges");
            AssertEqual(13, radiusTwo.Distinct(StringComparer.OrdinalIgnoreCase).Count(), "terrain reveal keys are unique");
            AssertEqual(true, radiusTwo.SequenceEqual(repeatedRadiusTwo), "terrain reveal order is deterministic");
            AssertEqual(
                "4:cell:3:1|4:cell:2:2|4:cell:3:2|4:cell:4:2|4:cell:1:3|4:cell:2:3|4:cell:3:3|4:cell:4:3|4:cell:5:3|4:cell:2:4|4:cell:3:4|4:cell:4:4|4:cell:3:5",
                string.Join("|", radiusTwo),
                "terrain reveal scans the diamond in stable row-major order");

            List<string> discoveries = new List<string>
            {
                ExplorationChartRules.CellKey(4, 3, 3).ToUpperInvariant(),
                ExplorationChartRules.CellKey(4, 3, 3),
                ExplorationChartRules.CellKey(4, 4, 3),
                ExplorationChartRules.CellKey(5, 3, 3),
                RouteChartRules.DiscoveryKey(4, "quarry-turn"),
                WorldSiteInteractionRules.ChartFlag(4, "glass-lore-library"),
                "4:green-shrine-road",
                "4:cell:-1:0",
                "4:cell:3",
                "0:cell:1:1",
                "malformed"
            };
            AssertEqual(true, ExplorationChartRules.IsCharted(discoveries, 4, 3, 3), "charted terrain lookup is case-insensitive");
            AssertEqual(false, ExplorationChartRules.IsCharted(discoveries, 3, 3, 3), "charted terrain lookup remains depth-scoped");
            AssertEqual(2, ExplorationChartRules.CountChartedCells(discoveries, 4), "terrain chart count deduplicates cells and ignores zones, junctions, sites, and malformed keys");
            AssertEqual(1, ExplorationChartRules.CountChartedCells(discoveries, 5), "terrain chart count isolates another map depth");
            AssertEqual(
                2,
                ExplorationChartRules.CountChartedCells(discoveries.Concat(new[] { "4:cell:99:99" }), 4, 8, 8),
                "bounded terrain chart counts reject numerically valid cells outside the live map");
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
            AssertEqual(true, ExplorationReadabilityRules.MidgaardPropAlpha(false, 0f) >= 0.76f, "close-map ambient city props remain legible");
            AssertEqual(true, ExplorationReadabilityRules.MidgaardPropAlpha(false, 1f) <= 0.86f, "close-map ambient city props remain subordinate to blockers");
            AssertEqual(true, ExplorationReadabilityRules.BiomePropAlpha(false, 0f) >= 0.94f, "approved close-map soft scenery keeps an opaque readable core");
            AssertEqual(true, ExplorationReadabilityRules.BiomePropAlpha(false, 1f) <= 0.98f, "approved close-map soft-scenery opacity remains bounded");
            AssertEqual(true, ExplorationReadabilityRules.MidgaardPropAlpha(true, 1f) < ExplorationReadabilityRules.MidgaardPropAlpha(false, 1f), "region-map city props yield to semantic targets");
            AssertEqual(true, ExplorationReadabilityRules.BiomePropAlpha(true, 1f) < ExplorationReadabilityRules.BiomePropAlpha(false, 1f), "region-map biome props yield to semantic targets");

            MapObject stairs = new MapObject(1, 1, ObjectType.Stairs);
            MapObject healer = new MapObject(1, 1, ObjectType.TempleHealer);
            MapObject wall = new MapObject(1, 1, ObjectType.CityWall);
            MapObject northGate = new MapObject(1, 1, ObjectType.NorthGate);
            MapObject southGate = new MapObject(1, 1, ObjectType.SouthGate);
            AssertEqual(WorldMapCellAccessKind.OpenGround, ExplorationReadabilityRules.ClassifyCellAccess(1, null, false, false), "empty floor reads as open ground");
            AssertEqual(WorldMapCellAccessKind.SoftScenery, ExplorationReadabilityRules.ClassifyCellAccess(1, null, true, false), "presentation-only art reads as soft scenery");
            AssertEqual(WorldMapCellAccessKind.WalkableFeature, ExplorationReadabilityRules.ClassifyCellAccess(1, stairs, false, false), "stairs retain their walkable interaction footprint");
            AssertEqual(WorldMapCellAccessKind.UseFromBeside, ExplorationReadabilityRules.ClassifyCellAccess(1, healer, false, false), "named NPCs advertise adjacent use instead of overlap");
            AssertEqual(WorldMapCellAccessKind.SolidObstacle, ExplorationReadabilityRules.ClassifyCellAccess(1, wall, false, false), "solid objects advertise their blocking footprint");
            AssertEqual(WorldMapCellAccessKind.BlockedTerrain, ExplorationReadabilityRules.ClassifyCellAccess(0, null, false, false), "closed terrain advertises its blocked footprint");
            AssertEqual(WorldMapCellAccessKind.UseFromBeside, ExplorationReadabilityRules.ClassifyCellAccess(0, northGate, false, false), "barred north gate advertises adjacent use despite its wall tile");
            AssertEqual(WorldMapCellAccessKind.UseFromBeside, ExplorationReadabilityRules.ClassifyCellAccess(0, southGate, false, false), "barred south gate advertises adjacent use despite its wall tile");
            AssertEqual(WorldMapCellAccessKind.EnemyOccupied, ExplorationReadabilityRules.ClassifyCellAccess(1, null, false, true), "mobile threats outrank ground presentation");
            AssertEqual(true, ExplorationReadabilityRules.IsWalkableAccess(WorldMapCellAccessKind.SoftScenery), "soft scenery explicitly yields the path");
            AssertEqual(false, ExplorationReadabilityRules.IsWalkableAccess(WorldMapCellAccessKind.UseFromBeside), "adjacent-use objects never imply overlap");
            AssertEqual(WorldMapMovementCueKind.OpenTick, ExplorationReadabilityRules.MovementCueKind(WorldMapCellAccessKind.WalkableFeature), "walkable features use an open cue shape");
            AssertEqual(WorldMapMovementCueKind.UseBracket, ExplorationReadabilityRules.MovementCueKind(WorldMapCellAccessKind.UseFromBeside), "adjacent-use objects use a bracket cue shape");
            AssertEqual(WorldMapMovementCueKind.StopBar, ExplorationReadabilityRules.MovementCueKind(WorldMapCellAccessKind.SolidObstacle), "solid blockers use a stop-bar cue shape");
            AssertEqual(WorldMapMovementCueKind.ThreatDoubleBar, ExplorationReadabilityRules.MovementCueKind(WorldMapCellAccessKind.EnemyOccupied), "enemy-occupied cells use a distinct double cue shape");
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

        private static void WorldMapAtlasPixelMetricsShareTopDownSnapshots()
        {
            GameObject host = new GameObject("Rule smoke world-map snapshot host");
            Texture2D snapshotAtlas = null;
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                MethodInfo transparent = typeof(AshenHallsGame).GetMethod("TransparentPixelFractionFromSnapshot", flags);
                MethodInfo visible = typeof(AshenHallsGame).GetMethod(
                    "VisiblePixelFractionFromSnapshot",
                    flags,
                    null,
                     new[] { typeof(Color32[]), typeof(int), typeof(int), typeof(Rect) },
                     null);
                MethodInfo fullVisible = typeof(AshenHallsGame).GetMethod(
                    "VisiblePixelFractionFromSnapshot",
                    flags,
                    null,
                    new[] { typeof(Color32[]), typeof(int), typeof(int) },
                    null);
                MethodInfo trim = typeof(AshenHallsGame).GetMethod("TrimVisibleSourceFromSnapshot", flags);
                MethodInfo resolve = typeof(AshenHallsGame).GetMethod("TryResolveExploreArtMetrics", flags);
                FieldInfo usabilityCacheField = typeof(AshenHallsGame).GetField("explorationAtlasCellUsable", flags);
                FieldInfo metricsCacheField = typeof(AshenHallsGame).GetField("exploreArtMetrics", flags);
                AssertEqual(
                    true,
                    transparent != null
                    && visible != null
                    && fullVisible != null
                    && trim != null
                    && resolve != null
                    && usabilityCacheField != null
                    && metricsCacheField != null,
                    "runtime world-map snapshot metrics and shared caches remain available");

                Color32 clear = new Color32(0, 0, 0, 0);
                Color32 opaque = new Color32(255, 255, 255, 255);
                Color32[] pixels =
                {
                    clear, clear, clear, clear,
                    clear, clear, opaque, clear,
                    clear, opaque, opaque, clear,
                    clear, clear, clear, clear
                };
                float transparentFraction = (float)transparent.Invoke(game, new object[] { pixels, 4, 4 });
                float topHalfVisible = (float)visible.Invoke(game, new object[] { pixels, 4, 4, new Rect(0f, 0f, 4f, 2f) });
                float bottomHalfVisible = (float)visible.Invoke(game, new object[] { pixels, 4, 4, new Rect(0f, 2f, 4f, 2f) });
                AssertEqual(13f / 16f, transparentFraction, "snapshot transparency uses the exact alpha threshold over one pixel buffer");
                AssertEqual(2f / 8f, topHalfVisible, "top-left atlas coordinates sample Unity's upper rows");
                AssertEqual(1f / 8f, bottomHalfVisible, "top-left atlas coordinates invert correctly into Unity's lower pixel rows");

                Rect trimmed = (Rect)trim.Invoke(game, new object[] { pixels, 4, 4, new Rect(0f, 0f, 4f, 4f) });
                AssertEqual(new Rect(0f, 0f, 4f, 4f), trimmed, "tiny snapshot trim padding remains clamped to the atlas cell");
                Color32[] thresholdPixels =
                {
                    new Color32(255, 255, 255, 31),
                    new Color32(255, 255, 255, 32)
                };
                AssertEqual(0.5f, (float)transparent.Invoke(game, new object[] { thresholdPixels, 2, 1 }), "alpha 31 remains transparent while alpha 32 remains visible");
                AssertEqual(0.5f, (float)fullVisible.Invoke(game, new object[] { thresholdPixels, 2, 1 }), "snapshot visibility preserves the exact alpha-32 boundary");
                AssertEqual(-1f, (float)transparent.Invoke(game, new object[] { new Color32[0], 0, 0 }), "invalid pixel snapshots retain the validation fallback");
                AssertEqual(-1f, (float)transparent.Invoke(game, new object[] { new Color32[3], 2, 2 }), "short pixel snapshots retain the validation fallback");
                AssertEqual(-1f, (float)transparent.Invoke(game, new object[] { new Color32[5], 2, 2 }), "oversized pixel snapshots retain the validation fallback");
                AssertEqual(-1f, (float)visible.Invoke(game, new object[] { new Color32[3], 2, 2, new Rect(0f, 0f, 2f, 2f) }), "regional metrics reject mismatched snapshot geometry");
                Rect malformedSource = new Rect(0f, 0f, 2f, 2f);
                AssertEqual(malformedSource, (Rect)trim.Invoke(game, new object[] { new Color32[5], 2, 2, malformedSource }), "trim metrics preserve their source when snapshot geometry is mismatched");

                snapshotAtlas = new Texture2D(4, 4, TextureFormat.RGBA32, false);
                snapshotAtlas.name = "rule-smoke-shared-snapshot";
                snapshotAtlas.SetPixels32(pixels);
                snapshotAtlas.Apply(false, false);
                System.Collections.IDictionary usabilityCache = (System.Collections.IDictionary)usabilityCacheField.GetValue(game);
                System.Collections.IDictionary metricsCache = (System.Collections.IDictionary)metricsCacheField.GetValue(game);
                int usabilityBefore = usabilityCache.Count;
                int metricsBefore = metricsCache.Count;
                object[] firstResolveArgs = { snapshotAtlas, new Rect(0f, 0f, 4f, 4f), 0, "rule smoke snapshot", 0f, 1f, null };
                AssertEqual(true, (bool)resolve.Invoke(game, firstResolveArgs), "first trimmed-art lookup accepts a valid shared snapshot");
                AssertEqual(usabilityBefore + 1, usabilityCache.Count, "first trimmed-art lookup caches one usability result");
                AssertEqual(metricsBefore + 1, metricsCache.Count, "first trimmed-art lookup caches one bounds result");
                object firstMetrics = firstResolveArgs[6];
                object[] secondResolveArgs = { snapshotAtlas, new Rect(0f, 0f, 4f, 4f), 0, "rule smoke snapshot", 0f, 1f, null };
                AssertEqual(true, (bool)resolve.Invoke(game, secondResolveArgs), "repeat trimmed-art lookup reuses the accepted snapshot metrics");
                AssertEqual(usabilityBefore + 1, usabilityCache.Count, "repeat trimmed-art lookup does not duplicate usability cache entries");
                AssertEqual(metricsBefore + 1, metricsCache.Count, "repeat trimmed-art lookup does not duplicate bounds cache entries");
                AssertEqual(true, object.ReferenceEquals(firstMetrics, secondResolveArgs[6]), "repeat trimmed-art lookup returns the cached metrics object");
            }
            finally
            {
                if (snapshotAtlas != null) UnityEngine.Object.DestroyImmediate(snapshotAtlas);
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
            AssertEqual(true,
                WorldAreaSetpiecePresentationRules.MapScale(true) >= 1.25f
                && WorldAreaSetpiecePresentationRules.MapScale(true) <= 1.60f,
                "region-map set-pieces remain prominent without implying a multi-cell collision platform");
            AssertEqual(true, WorldAreaSetpiecePresentationRules.BaselineFraction(false) > WorldAreaSetpiecePresentationRules.BaselineFraction(true), "local set-pieces keep their authored ground baseline");
            AssertEqual(true, WorldAreaSetpiecePresentationRules.BaselineFraction(true) > 0.70f && WorldAreaSetpiecePresentationRules.BaselineFraction(false) < 0.90f, "set-piece baselines stay inside their map cells");
            AssertEqual(true, WorldAreaSetpiecePresentationRules.PreserveScaleAtViewportEdge, "edge-crossing set-pieces preserve world-space scale and clip to the map");
            AssertEqual(true, WorldAreaSetpiecePresentationRules.FitsViewport(12f, 12f, 28f, 28f, 0f, 0f, 40f, 40f, 2f), "fully visible authored set-pieces keep their large presentation");
            AssertEqual(false, WorldAreaSetpiecePresentationRules.FitsViewport(-2f, 12f, 14f, 28f, 0f, 0f, 40f, 40f, 2f), "edge-crossing authored set-pieces are recognized for map clipping");
            AssertEqual(false, WorldAreaSetpiecePresentationRules.FitsViewport(2f, 12f, 18f, 28f, 0f, 0f, 40f, 40f, 3f), "set-piece safe inset recognizes near-edge clipping");
        }

        private static void V24WorldMapCharacterArtRulesAreStable()
        {
            AssertEqual(4, WorldThreatHabitatPresentationRules.Columns, "v2.4 habitat atlas columns");
            AssertEqual(2, WorldThreatHabitatPresentationRules.Rows, "v2.4 habitat atlas rows");
            AssertEqual(8, WorldThreatHabitatPresentationRules.CellCount, "v2.4 habitat atlas cell count");
            int[] passableBiomePropIndices = { 3, 5, 6, 7, 14, 16, 19 };
            foreach (int index in passableBiomePropIndices)
            {
                AssertEqual(true, ExplorationReadabilityRules.IsPassableBiomePropIndex(index), $"low-profile biome prop {index} remains eligible for passable scenery");
            }
            int[] solidBiomePropIndices = { 0, 1, 2, 4, 8, 9, 10, 11, 12, 13, 15, 17, 18 };
            foreach (int index in solidBiomePropIndices)
            {
                AssertEqual(false, ExplorationReadabilityRules.IsPassableBiomePropIndex(index), $"solid biome prop {index} cannot masquerade as passable scenery");
            }
            AssertEqual(false, ExplorationReadabilityRules.IsPassableBiomePropIndex(-1), "invalid biome prop indices remain ineligible for passable scenery");
            AssertEqual(false, ExplorationReadabilityRules.IsPassableBiomePropIndex(20), "out-of-range biome prop indices remain ineligible for passable scenery");
            AssertEqual(false, ExplorationReadabilityRules.AllowPassableMidgaardFixtures, "procedural Midgaard fixtures cannot occupy walkable cells");
            AssertEqual(0, WorldThreatHabitatPresentationRules.ArchetypeIndex("rats"), "rat patrols use the warren habitat");
            AssertEqual(1, WorldThreatHabitatPresentationRules.ArchetypeIndex("ratcleric"), "plague patrols use the bell midden habitat");
            AssertEqual(2, WorldThreatHabitatPresentationRules.ArchetypeIndex("kobolds"), "kobold patrols use the ambush camp habitat");
            AssertEqual(3, WorldThreatHabitatPresentationRules.ArchetypeIndex("koboldshaman"), "kobold shamans use the totem-yard habitat");
            AssertEqual(4, WorldThreatHabitatPresentationRules.FactionIndex(RoamingThreatFaction.Drow), "drow patrols use the watchpost habitat");
            AssertEqual(5, WorldThreatHabitatPresentationRules.FactionIndex(RoamingThreatFaction.Undead), "undead patrols use the ossuary habitat");
            AssertEqual(6, WorldThreatHabitatPresentationRules.FactionIndex(RoamingThreatFaction.Demons), "demon patrols use the breach habitat");
            AssertEqual(7, WorldThreatHabitatPresentationRules.ArchetypeIndex("waystation"), "neutral aftermath uses the ruined waystation habitat");
            RoamingThreatDefinition activeKobold = RoamingThreatCatalog.Find("dusk-market-kobold-raiders", 2, true);
            AssertEqual(2, WorldThreatHabitatPresentationRules.PresentationIndex(true, activeKobold, "demon"), "active threats retain their authored habitat");
            AssertEqual(7, WorldThreatHabitatPresentationRules.PresentationIndex(false, activeKobold, "demon"), "inactive threats leave neutral ruined-road aftermath art");
            AssertEqual(6, WorldThreatHabitatPresentationRules.PresentationIndex(true, null, "demon"), "active legacy threats retain their archetype habitat fallback");
            AssertEqual(true, WorldThreatHabitatPresentationRules.DrawsBeneathRoamingThreatToken, "habitats remain below mobile patrol tokens");
            AssertEqual(true, WorldThreatHabitatPresentationRules.BottomCenterPivotY == 1f, "habitats keep a GUI bottom-center anchor");
            AssertEqual(true, WorldThreatHabitatPresentationRules.PreserveScaleAtViewportEdge, "edge-crossing habitats preserve world-space scale and clip to the map");
            AssertEqual(false, WorldThreatHabitatPresentationRules.ShouldDrawAtHome(true, true), "habitats stay off certified safe roads");
            AssertEqual(true, WorldThreatHabitatPresentationRules.ShouldDrawAtHome(true, false), "valid threat homes receive habitat art");
            AssertEqual(true, WorldThreatHabitatPresentationRules.ShouldDrawFallback(false, true), "a missing habitat atlas receives a visible fallback on active collision cells");
            AssertEqual(false, WorldThreatHabitatPresentationRules.ShouldDrawFallback(true, true), "the approved habitat atlas remains the preferred active presentation");
            AssertEqual(false, WorldThreatHabitatPresentationRules.ShouldDrawFallback(false, false), "walkable cleared aftermath stays low even when the atlas is unavailable");
            AssertEqual(true, WorldThreatHabitatPresentationRules.MapScale(false, false) < WorldThreatHabitatPresentationRules.MapScale(false, true), "cleared habitats shrink into walkable aftermath");
            AssertEqual(true, WorldThreatHabitatPresentationRules.MapScale(true, false) < WorldThreatHabitatPresentationRules.MapScale(true, true), "region-map aftermath stays subordinate to active lairs");
            AssertEqual(WorldThreatHabitatPresentationRules.MapScale(false, true, true), WorldThreatHabitatPresentationRules.MapScale(false, true, false), "active local-map lair scale remains physical while its patrol roams");
            AssertEqual(WorldThreatHabitatPresentationRules.MapScale(true, true, true), WorldThreatHabitatPresentationRules.MapScale(true, true, false), "active region-map lair scale remains physical while its patrol roams");
            AssertEqual(WorldThreatHabitatPresentationRules.HabitatAlpha(false, true, true), WorldThreatHabitatPresentationRules.HabitatAlpha(false, true, false), "active lair opacity no longer changes with patrol occupancy");
            AssertEqual(true, WorldThreatHabitatPresentationRules.HabitatAlpha(false, true, false) >= 0.90f, "active lairs retain a strongly opaque physical silhouette");
            AssertEqual(true, WorldThreatHabitatPresentationRules.HabitatAlpha(false, false, false) < WorldThreatHabitatPresentationRules.HabitatAlpha(false, true, false), "cleared aftermath remains visibly subordinate to an active lair");
            AssertEqual(true, ExplorationArtRules.MidgaardBuildingFoundationWidthInCells(false) <= 1f, "local-view building ground contact matches its one-cell collision footprint");
            AssertEqual(true, ExplorationArtRules.MidgaardBuildingFoundationWidthInCells(true) <= 1f, "region-view building ground contact matches its one-cell collision footprint");
            AssertEqual(true, ExplorationArtRules.MidgaardBuildingSpriteScale(false) > 1f, "local-view building art may still rise beyond its collision footprint");
            AssertEqual(true, ExplorationArtRules.MidgaardBuildingSpriteScale(true) > 1f, "region-view building silhouettes remain readable");

            AssertEqual("The Old Road", WorldMapGenerationRules.OldRoadName, "Midgaard's east-west artery keeps its authored name");
            AssertEqual("pilgrim-fork", WorldMapGenerationRules.OldRoadWestJunctionId, "the Old Road retains its western adventure endpoint");
            AssertEqual("lanternless-cross", WorldMapGenerationRules.OldRoadEastJunctionId, "the Old Road retains its eastern adventure endpoint");
            AssertEqual(true, MidgaardDistrictRules.IsOldRoadOffset(-10, 0), "the Old Road begins at Midgaard's western gate centerline");
            AssertEqual(true, MidgaardDistrictRules.IsOldRoadOffset(10, 0), "the Old Road reaches Midgaard's eastern gate centerline");
            AssertEqual(false, MidgaardDistrictRules.IsOldRoadOffset(-11, 0), "the Old Road district contract stops outside Midgaard's west wall");
            AssertEqual(false, MidgaardDistrictRules.IsOldRoadOffset(11, 0), "the Old Road district contract stops outside Midgaard's east wall");
            AssertEqual(false, MidgaardDistrictRules.IsOldRoadOffset(0, 1), "parallel civic paving is not mislabeled as the Old Road");
            for (int dx = -10; dx <= 10; dx++)
            {
                AssertEqual(true, MidgaardDistrictRules.IsOldRoadOffset(dx, 0), $"Old Road centerline includes Midgaard offset {dx},0");
                ExplorationCellRole centerlineRoles = MidgaardDistrictRules.RolesAtOffset(dx, 0);
                AssertEqual(true, (centerlineRoles & ExplorationCellRole.Road) != 0, $"Old Road centerline offset {dx},0 carries the Road role");
                AssertEqual(
                    ExplorationCellRole.None,
                    centerlineRoles & (ExplorationCellRole.Room | ExplorationCellRole.Water | ExplorationCellRole.Hazard),
                    $"Old Road centerline offset {dx},0 is free of room, water, and hazard conflicts");
                AssertEqual(WorldMapGenerationRules.OldRoadName, MidgaardDistrictRules.DistrictAtOffset(dx, 0), $"Old Road centerline offset {dx},0 carries the named district identity");
            }

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
            AssertEqual(true, ExplorationCharacterArtCatalog.IsNewGameTutorialLane(1, false, true), "the opening Grand Hearth objective route is the tutorial lane");
            AssertEqual(false, ExplorationCharacterArtCatalog.IsNewGameTutorialLane(1, false, false), "ordinary chapter-one roads are not mislabeled as tutorial lanes");
            AssertEqual(false, ExplorationCharacterArtCatalog.IsNewGameTutorialLane(1, true, true), "the tutorial lane retires after the first contract is accepted");
            AssertEqual(false, ExplorationCharacterArtCatalog.IsNewGameTutorialLane(2, false, true), "later-depth guidance is not mislabeled as the New Game tutorial lane");

            AssertEqual(false, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(ExplorationCellRole.Road, true, false, false, false, false), "ambient citizens stay off the actual tutorial lane");
            AssertEqual(false, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(ExplorationCellRole.Road, false, true, false, false, false), "ambient citizens stay off certified safe roads");
            AssertEqual(false, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(ExplorationCellRole.Road, false, false, true, false, false), "ambient citizens stay off guidance routes");
            AssertEqual(false, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(ExplorationCellRole.Room, false, false, false, false, false), "ambient citizens stay out of room-reserved cells");
            AssertEqual(false, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(ExplorationCellRole.Water, false, false, false, false, false), "ambient citizens stay out of water cells");
            AssertEqual(false, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(ExplorationCellRole.Hazard, false, false, false, false, false), "ambient citizens stay out of hazard cells");
            AssertEqual(false, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(ExplorationCellRole.Threshold, false, false, false, false, false), "ambient citizens keep entrances clear");
            AssertEqual(false, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(ExplorationCellRole.Road, false, false, false, true, false), "ambient citizens never become interactable-cell impostors");
            AssertEqual(false, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(ExplorationCellRole.Road, false, false, false, false, true), "ambient citizens stay out of authored regional-site reservations");
            AssertEqual(true, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(ExplorationCellRole.Road, false, false, false, false, false), "edge-offset passersby can inhabit ordinary non-guidance roads");
            AssertEqual(false, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(ExplorationCellRole.Trail, false, false, false, false, false), "ambient citizens keep trails visually clear");
            AssertEqual(false, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(ExplorationCellRole.Bridge, false, false, false, false, false), "ambient citizens keep bridges visually clear");
            AssertEqual(true, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(ExplorationCellRole.Plaza, false, false, false, false, false), "edge-offset passersby keep ordinary plazas inhabited");
            AssertEqual(true, ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(ExplorationCellRole.None, false, false, false, false, false), "off-lane civic ground remains eligible for ambient life");
            AssertEqual("Lamplighter / passing townsfolk", ExplorationCharacterArtCatalog.AmbientCitizenDisplayLabel(AmbientCitizenProfession.Lamplighter), "passersby receive natural non-interactive hover labels");
            AssertEqual(true, ExplorationCharacterArtCatalog.ExteriorCitizenPadding(true) > ExplorationCharacterArtCatalog.ExteriorCitizenPadding(false), "region-map passersby occupy less of their cell");
            AssertEqual(true, ExplorationCharacterArtCatalog.ExteriorCitizenAlpha(false, false) >= 0.80f && ExplorationCharacterArtCatalog.ExteriorCitizenAlpha(false, false) < 0.90f, "local passersby stay readable while remaining behind named actors");
            AssertEqual(true, ExplorationCharacterArtCatalog.ExteriorCitizenAlpha(false, true) < ExplorationCharacterArtCatalog.ExteriorCitizenAlpha(false, false), "nearby passersby fade as they yield the path");
            AssertEqual(true, ExplorationNpcPresentationRules.ShouldDrawExteriorAmbientCitizen(false), "Local Map keeps readable ambient citizens");
            AssertEqual(false, ExplorationNpcPresentationRules.ShouldDrawExteriorAmbientCitizen(true), "Region Map replaces ambient full-body figures with strategic map clarity");
            AssertEqual(ExplorationNpcPresentationRules.ExteriorAmbientPadding(false), ExplorationCharacterArtCatalog.ExteriorCitizenPadding(false), "ambient-citizen catalog delegates Local sizing to the shared v2.21 presentation rules");
            AssertEqual(ExplorationNpcPresentationRules.ExteriorAmbientAlpha(false, false), ExplorationCharacterArtCatalog.ExteriorCitizenAlpha(false, false), "ambient-citizen catalog delegates Local opacity to the shared v2.21 presentation rules");
            AssertEqual(true, ExplorationNpcPresentationRules.ExteriorAmbientAlpha(false, true) < ExplorationNpcPresentationRules.ExteriorAmbientAlpha(false, false), "v2.21 citizens still yield visually beside the party");
            float namedNpcLocalOccupancy = (1f - 2f * ExplorationNpcPresentationRules.NamedObjectPadding(false))
                * (1f - 2f * ExplorationNpcPresentationRules.NamedArtPadding())
                * ExplorationNpcPresentationRules.NamedArtScale(false);
            float ambientNpcLocalOccupancy = (1f - 2f * ExplorationNpcPresentationRules.ExteriorAmbientPadding(false)) * 0.98f;
            float patronNpcLocalOccupancy = (1f - 2f * ExplorationNpcPresentationRules.GrandHearthPatronPadding(false)) * 0.98f;
            AssertEqual(true, namedNpcLocalOccupancy >= 0.90f && namedNpcLocalOccupancy <= 1.05f, "named Local Map NPCs occupy a readable but bounded share of their cell");
            AssertEqual(true, Math.Abs(namedNpcLocalOccupancy - ambientNpcLocalOccupancy) <= 0.08f, "ambient Local Map citizens stay within eight percent of named-NPC height");
            AssertEqual(true, Math.Abs(namedNpcLocalOccupancy - patronNpcLocalOccupancy) <= 0.08f, "Grand Hearth patrons stay within eight percent of named-NPC height");
            float namedNpcWideOccupancy = (1f - 2f * ExplorationNpcPresentationRules.NamedObjectPadding(true))
                * (1f - 2f * ExplorationNpcPresentationRules.NamedArtPadding())
                * ExplorationNpcPresentationRules.NamedArtScale(true);
            float patronNpcWideOccupancy = (1f - 2f * ExplorationNpcPresentationRules.GrandHearthPatronPadding(true)) * 0.98f;
            AssertEqual(true, Math.Abs(namedNpcWideOccupancy - patronNpcWideOccupancy) <= 0.03f, "Region named contacts and Grand Hearth patrons stay optically aligned");
            AssertEqual(true, ExplorationNpcPresentationRules.NamedObjectPadding(true) > ExplorationNpcPresentationRules.NamedObjectPadding(false), "named Region Map NPCs remain subordinate to landmarks");
            AssertEqual(true, ExplorationCharacterArtCatalog.ExteriorCitizenYieldsToParty(5, 4, 4, 4), "orthogonally adjacent passersby yield to the party");
            AssertEqual(true, ExplorationCharacterArtCatalog.ExteriorCitizenYieldsToParty(4, 4, 4, 4), "overlapped passersby finish stepping aside instead of popping out");
            AssertEqual(false, ExplorationCharacterArtCatalog.ExteriorCitizenYieldsToParty(5, 5, 4, 4), "diagonal passersby do not pretend to occupy a movement target");
            float citizenOffset = ExplorationCharacterArtCatalog.ExteriorCitizenHorizontalOffsetInCells("Wharf Market", 9471, 23, 17, 12, 12);
            AssertEqual(citizenOffset, ExplorationCharacterArtCatalog.ExteriorCitizenHorizontalOffsetInCells("Wharf Market", 9471, 23, 17, 12, 12), "passerby edge placement is deterministic");
            AssertEqual(true, Math.Abs(citizenOffset) >= 0.12f && Math.Abs(citizenOffset) <= 0.18f, "passersby stay near a cell edge without leaving their walkable cell");
            float overlapOffset = ExplorationCharacterArtCatalog.ExteriorCitizenHorizontalOffsetInCells("Wharf Market", 9471, 23, 17, 23, 17);
            AssertEqual(true, Math.Abs(overlapOffset) >= 0.15f && Math.Abs(overlapOffset) <= 0.18f, "an overlapped passerby completes a bounded visible side-step");
        }

        private static void ExplorationMiniMapPresentationRulesReserveSemanticMarkers()
        {
            Color32 uncharted = ExplorationMiniMapPresentationRules.UnchartedTerrainPixel();
            AssertEqual(true, uncharted.r == 3 && uncharted.g == 6 && uncharted.b == 7 && uncharted.a == 255,
                "uncharted minimap terrain uses one opaque topology-neutral pixel");
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

        private static void ExplorationMiniMapTerrainCacheIsStableAndTopDown()
        {
            AssertEqual(6, ExplorationMiniMapPresentationRules.PixelIndexForTopDownMapCell(0, 0, 3, 3), "map north row uploads to Unity's top texture row");
            AssertEqual(2, ExplorationMiniMapPresentationRules.PixelIndexForTopDownMapCell(2, 2, 3, 3), "map south row uploads to Unity's bottom texture row");
            AssertEqual(-1, ExplorationMiniMapPresentationRules.PixelIndexForTopDownMapCell(3, 0, 3, 3), "out-of-bounds minimap cells have no texture index");
            AssertEqual(2610, ExplorationMiniMapPresentationRules.PixelIndexForTopDownMapCell(0, 0, 58, 46), "production map north-west corner maps to the top texture row");
            AssertEqual(2667, ExplorationMiniMapPresentationRules.PixelIndexForTopDownMapCell(57, 0, 58, 46), "production map north-east corner maps to the top texture row");
            AssertEqual(0, ExplorationMiniMapPresentationRules.PixelIndexForTopDownMapCell(0, 45, 58, 46), "production map south-west corner maps to the bottom texture row");
            AssertEqual(57, ExplorationMiniMapPresentationRules.PixelIndexForTopDownMapCell(57, 45, 58, 46), "production map south-east corner maps to the bottom texture row");
            AssertEqual(1247, ExplorationMiniMapPresentationRules.PixelIndexForTopDownMapCell(29, 24, 58, 46), "production map interior cells preserve top-down row projection");
            AssertEqual(-1, ExplorationMiniMapPresentationRules.PixelIndexForTopDownMapCell(-1, 0, 58, 46), "negative minimap cells have no texture index");
            AssertEqual(-1, ExplorationMiniMapPresentationRules.PixelIndexForTopDownMapCell(0, 46, 58, 46), "south out-of-bounds minimap cells have no texture index");
            AssertEqual(-1, ExplorationMiniMapPresentationRules.PixelIndexForTopDownMapCell(0, 0, 0, 46), "nonpositive minimap dimensions have no texture index");
            HashSet<int> projectedCells = new HashSet<int>();
            for (int y = 0; y < 46; y++)
            for (int x = 0; x < 58; x++)
            {
                int pixel = ExplorationMiniMapPresentationRules.PixelIndexForTopDownMapCell(x, y, 58, 46);
                AssertEqual(true, pixel >= 0 && pixel < 58 * 46, "every production minimap cell projects inside the terrain texture");
                AssertEqual(true, projectedCells.Add(pixel), "production minimap projection is bijective");
            }
            AssertEqual(58 * 46, projectedCells.Count, "production minimap projection covers every raster pixel once");

            long baseKey = ExplorationMiniMapPresentationRules.TerrainCacheKey(9, 58, 46, 2, 10, 11, 71);
            AssertEqual(baseKey, ExplorationMiniMapPresentationRules.TerrainCacheKey(9, 58, 46, 2, 10, 11, 71), "identical minimap terrain state has a stable cache key");
            AssertEqual(false, baseKey == ExplorationMiniMapPresentationRules.TerrainCacheKey(10, 58, 46, 2, 10, 11, 71), "map replacement invalidates the terrain raster");
            AssertEqual(false, baseKey == ExplorationMiniMapPresentationRules.TerrainCacheKey(9, 57, 46, 2, 10, 11, 71), "map width changes invalidate the terrain raster");
            AssertEqual(false, baseKey == ExplorationMiniMapPresentationRules.TerrainCacheKey(9, 58, 45, 2, 10, 11, 71), "map height changes invalidate the terrain raster");
            AssertEqual(false, baseKey == ExplorationMiniMapPresentationRules.TerrainCacheKey(9, 58, 46, 2, 11, 11, 71), "party movement invalidates proximity-shaded terrain");
            AssertEqual(false, baseKey == ExplorationMiniMapPresentationRules.TerrainCacheKey(9, 58, 46, 2, 10, 12, 71), "north-south party movement invalidates proximity-shaded terrain");
            AssertEqual(false, baseKey == ExplorationMiniMapPresentationRules.TerrainCacheKey(9, 58, 46, 3, 10, 11, 71), "depth changes invalidate terrain colors");
            AssertEqual(false, baseKey == ExplorationMiniMapPresentationRules.TerrainCacheKey(9, 58, 46, 2, 10, 11, 72), "topology changes invalidate the terrain raster");

            ExplorationMiniMapTerrainCache cache = new ExplorationMiniMapTerrainCache();
            Texture2D first = null;
            Texture2D resized = null;
            int providerCalls = 0;
            try
            {
                first = cache.GetOrBuild(baseKey, 3, 2, (x, y) =>
                {
                    providerCalls++;
                    return new Color32((byte)(x * 40), (byte)(y * 80), 20, 255);
                });
                AssertEqual(6, providerCalls, "a minimap cache miss rasterizes each cell once");
                AssertEqual(1, cache.RebuildCount, "the first minimap raster increments the rebuild counter");
                AssertEqual(FilterMode.Point, first.filterMode, "cached minimap terrain keeps hard pixel edges");
                AssertEqual(TextureWrapMode.Clamp, first.wrapMode, "cached minimap terrain cannot bleed across map edges");

                Texture2D hit = cache.GetOrBuild(baseKey, 3, 2, (x, y) =>
                {
                    providerCalls++;
                    return new Color32(255, 0, 0, 255);
                });
                AssertEqual(true, ReferenceEquals(first, hit), "an unchanged minimap state reuses its texture");
                AssertEqual(6, providerCalls, "a cache hit never invokes the terrain provider");
                AssertEqual(1, cache.RebuildCount, "a cache hit does not count as a rebuild");

                Color32 north = new Color32(24, 48, 72, 191);
                Color32 south = new Color32(12, 18, 24, 47);
                Color32[] topDownPixels = new Color32[6];
                topDownPixels[ExplorationMiniMapPresentationRules.PixelIndexForTopDownMapCell(0, 0, 3, 2)] = north;
                topDownPixels[ExplorationMiniMapPresentationRules.PixelIndexForTopDownMapCell(0, 1, 3, 2)] = south;
                Texture2D recolored = cache.GetOrBuild(baseKey + 1, 3, 2, topDownPixels);
                AssertEqual(true, ReferenceEquals(first, recolored), "a same-size state change updates the existing texture");
                AssertEqual(2, cache.RebuildCount, "a changed terrain key rebuilds once");
                Color32[] uploaded = recolored.GetPixels32();
                AssertEqual(north, uploaded[3], "map north remains in Unity's upper texture row after upload");
                AssertEqual(south, uploaded[0], "map south remains in Unity's lower texture row after upload");
                AssertEqual((byte)191, uploaded[3].a, "cached terrain preserves reveal alpha for IMGUI blending");
                AssertEqual((byte)47, uploaded[0].a, "cached terrain preserves distant-cell alpha for IMGUI blending");

                resized = cache.GetOrBuild(baseKey + 2, 2, 2, new Color32[4]);
                AssertEqual(false, ReferenceEquals(first, resized), "a minimap dimension change replaces the texture");
                AssertEqual(true, cache.IsCurrent(baseKey + 2, 2, 2), "the replacement texture advertises its exact key and dimensions");
                cache.Clear();
                AssertEqual(false, cache.HasCachedRaster, "clearing the minimap cache drops the raster identity");
                AssertEqual<Texture2D>(null, cache.Texture, "clearing the minimap cache releases its texture reference");
                AssertEqual(3, cache.RebuildCount, "clearing a minimap raster preserves historical rebuild diagnostics");
                AssertThrows<ArgumentOutOfRangeException>(() => cache.GetOrBuild(1, 0, 2, new Color32[0]), "zero-width minimap rasters are rejected");
                AssertThrows<OverflowException>(() => cache.GetOrBuild(1, int.MaxValue, 2, new Color32[0]), "overflowing minimap dimensions are rejected");
                AssertThrows<ArgumentException>(() => cache.GetOrBuild(1, 2, 2, new Color32[3]), "wrong-sized minimap pixel arrays are rejected");
            }
            finally
            {
                cache.Dispose();
            }
            cache.Dispose();
            AssertThrows<ObjectDisposedException>(() => cache.GetOrBuild(1, 1, 1, new Color32[1]), "disposed minimap caches cannot be rebuilt");
        }

        private static void WorldMapProgressionPresentationRulesTrackChapterFiveState()
        {
            string redGate = WorldSitePresentationRules.LandmarkObjectIdPrefix + WorldSitePresentationRules.RedGateSeal;
            string crypt = WorldSitePresentationRules.LandmarkObjectIdPrefix + WorldSitePresentationRules.GloamDeepCrypt;
            string cistern = WorldSitePresentationRules.LandmarkObjectIdPrefix + WorldSitePresentationRules.SaltCisternGate;
            List<string> flags = new List<string>();

            AssertEqual(-1, WorldMapProgressionPresentationRules.ChapterFiveSiteIcon(null, flags), "a missing landmark ID has no Chapter V overlay");
            AssertEqual(-1, WorldMapProgressionPresentationRules.ChapterFiveSiteIcon("", flags), "a blank landmark ID has no Chapter V overlay");
            AssertEqual(-1, WorldMapProgressionPresentationRules.ChapterFiveSiteIcon(redGate.ToUpperInvariant(), flags), "landmark IDs remain case-sensitive save identities");
            AssertEqual(WorldMapProgressionPresentationRules.RedGateSealIcon, WorldMapProgressionPresentationRules.ChapterFiveSiteIcon(redGate, flags), "the uncleared Red Gate uses its authored seal graphic");
            AssertEqual(WorldMapProgressionPresentationRules.LockedGateIcon, WorldMapProgressionPresentationRules.ChapterFiveSiteIcon(crypt, flags), "the unrecovered ossuary road seal remains visibly locked");
            AssertEqual(WorldMapProgressionPresentationRules.LockedGateIcon, WorldMapProgressionPresentationRules.ChapterFiveSiteIcon(cistern, flags), "Salt Cistern Gate stays locked before the marshal route is opened");
            flags.Add(StoryFlags.RedGateVanguardDefeated);
            AssertEqual(WorldMapProgressionPresentationRules.ClearedSiteIcon, WorldMapProgressionPresentationRules.ChapterFiveSiteIcon(redGate, flags), "the defeated vanguard changes the Red Gate marker to cleared");
            flags.Add(StoryFlags.OssuaryRoadSealRecovered);
            AssertEqual(WorldMapProgressionPresentationRules.ClearedSiteIcon, WorldMapProgressionPresentationRules.ChapterFiveSiteIcon(crypt, flags), "recovering the road seal changes the crypt marker to cleared");
            flags.Add(StoryFlags.CrownroadMarshalDefeated);
            AssertEqual(WorldMapProgressionPresentationRules.MeteorCrownThresholdIcon, WorldMapProgressionPresentationRules.ChapterFiveSiteIcon(cistern, flags), "marshal victory reveals the Meteor Crown threshold graphic");
            flags.Add(StoryFlags.MeteorCrownThresholdSurveyed);
            AssertEqual(WorldMapProgressionPresentationRules.ClearedSiteIcon, WorldMapProgressionPresentationRules.ChapterFiveSiteIcon(cistern, flags), "surveying the threshold changes Salt Cistern Gate to cleared");
            AssertEqual(-1, WorldMapProgressionPresentationRules.ChapterFiveSiteIcon("regional-site:unrelated", flags), "unrelated landmarks keep their existing progression mapping");
            int[] iconCells =
            {
                WorldMapProgressionPresentationRules.LockedGateIcon,
                WorldMapProgressionPresentationRules.ClearedSiteIcon,
                WorldMapProgressionPresentationRules.RedGateSealIcon,
                WorldMapProgressionPresentationRules.MeteorCrownThresholdIcon
            };
            AssertEqual(4, iconCells.Distinct().Count(), "Chapter V progression states use four distinct semantic graphics");
            AssertEqual(true, iconCells.All(cell => cell >= 0 && cell < 20), "Chapter V progression graphics stay inside the approved 5x4 atlas");
        }

        private static void ApprovedV130WorldMapAtlasesMatchRuntimeContracts()
        {
            AssertEqual("Ash & Brimstone", VersionInfo.ProductName, "player-facing product name");
            AssertEqual("AshAndBrimstone", VersionInfo.ExecutableBaseName, "Windows executable base name");
            AssertEqual("Ashen Halls", VersionInfo.LegacyProductName, "legacy product name remains available for save import");
            AssertEqual("v2.22.0", VersionInfo.PackageVersion, "package version marks the stable inventory identity release");
            BuildWindows.ValidateApprovedRuntimeArtIsLatest(Directory.GetParent(Application.dataPath).FullName);
            AssertEqual("ability-icon-atlas-runtime-v2.9.0.png", RuntimeArtManifest.AbilityIconAtlas, "approved v2.9 ability atlas pin");
            AssertEqual("signature-spell-icon-atlas-runtime-v2.9.0.png", RuntimeArtManifest.SignatureSpellIconAtlas, "approved v2.9 signature spell atlas pin");
            AssertEqual("lightning-spell-icon-atlas-runtime-v1.97.0.png", RuntimeArtManifest.LightningSpellIconAtlas, "approved v1.97 lightning spell atlas pin");
            AssertEqual("power-book-state-icon-atlas-runtime-v1.97.0.png", RuntimeArtManifest.PowerBookStateIconAtlas, "approved v1.97 power-book state atlas pin");
            AssertEqual("combat-command-icon-atlas-runtime-v1.99.0.png", RuntimeArtManifest.CombatCommandIconAtlas, "approved v1.99 combat command atlas pin");
            AssertEqual("magic-ui-atlas-runtime-v1.31.0.png", RuntimeArtManifest.MagicUiAtlas, "approved v1.31 magic UI atlas pin");
            AssertEqual("spell-animation-atlas-runtime-v1.49.0.png", RuntimeArtManifest.SpellAnimationAtlas, "approved v1.49 spell animation atlas pin");
            AssertEqual("combat-spell-effects-atlas-runtime-v2.9.0.png", RuntimeArtManifest.EpicSpellEffectsAtlas, "approved v2.9 epic spell effects atlas pin");
            AssertEqual("mage-warlock-spell-vfx-atlas-runtime-v2.13.0.png", RuntimeArtManifest.MageWarlockSpellVfxAtlas, "approved v2.13 mage and warlock spell VFX pin");
            AssertEqual("support-hex-spell-vfx-atlas-runtime-v2.14.0.png", RuntimeArtManifest.SupportHexSpellVfxAtlas, "approved v2.14 support and hex spell VFX pin");
            AssertEqual("class-skill-vfx-atlas-runtime-v2.14.0.png", RuntimeArtManifest.ClassSkillVfxAtlas, "approved v2.14 class skill VFX pin");
            AssertEqual("combat-power-travel-vfx-atlas-runtime-v2.15.0.png", RuntimeArtManifest.CombatPowerTravelVfxAtlas, "approved v2.15 combat power travel VFX pin");
            AssertEqual("combat-power-aftermath-vfx-atlas-runtime-v2.17.0.png", RuntimeArtManifest.CombatPowerAftermathVfxAtlas, "approved v2.17 combat power aftermath VFX pin");
            AssertEqual("unique-item-atlas-runtime-v2.20.0.png", RuntimeArtManifest.UniqueItemAtlas, "approved v2.20 signature-item atlas pin");
            AssertEqual("title-backdrop-runtime-v2.4.0.png", RuntimeArtManifest.TavernBackdrop, "approved v2.4 Grand Hearth title backdrop pin");
            AssertEqual("tavern-ui-atlas-runtime-v1.5.9.png", RuntimeArtManifest.TavernUiAtlas, "approved v1.5.9 Grand Hearth relic atlas pin");
            AssertEqual("title-menu-scroll-runtime-v2.12.1.png", RuntimeArtManifest.TitleMenuScroll, "approved v2.12.1 Ashen Road charter pin");
            AssertEqual("title-menu-focus-runtime-v2.12.1.png", RuntimeArtManifest.TitleMenuFocus, "approved v2.12.1 title focus-ribbon pin");
            AssertEqual("title-menu-icon-atlas-runtime-v2.16.0.png", RuntimeArtManifest.TitleMenuIconAtlas, "approved v2.16 title menu glyph pin");
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
            AssertEqual("midgaard-town-atlas-runtime-v2.21.0.png", RuntimeArtManifest.MidgaardTownAtlas, "approved v2.21 architectural town atlas pin");
            AssertEqual("midgaard-tile-atlas-runtime-v1.6.3.png", RuntimeArtManifest.MidgaardTileAtlas, "approved v1.6.3 Midgaard terrain pin");
            AssertEqual("midgaard-city-prop-atlas-runtime-v1.29.0.png", RuntimeArtManifest.MidgaardCityPropAtlas, "approved v1.29 city prop atlas pin");
            AssertEqual("midgaard-street-life-atlas-runtime-v1.50.0.png", RuntimeArtManifest.MidgaardStreetLifeAtlas, "approved v1.50 street-life atlas pin");
            AssertEqual("midgaard-paving-decal-atlas-runtime-v1.50.0.png", RuntimeArtManifest.MidgaardPavingDecalAtlas, "approved v1.50 paving-decal atlas pin");
            AssertEqual("midgaard-road-surface-atlas-runtime-v2.21.0.png", RuntimeArtManifest.MidgaardRoadSurfaceAtlas, "approved v2.21 road-surface atlas pin");
            AssertEqual("midgaard-npc-atlas-runtime-v2.21.0.png", RuntimeArtManifest.MidgaardNpcAtlas, "approved v2.21 coherent named-NPC atlas pin");
            AssertEqual("world-npc-citizen-atlas-runtime-v2.21.0.png", RuntimeArtManifest.WorldNpcCitizenAtlas, "approved v2.21 coherent ambient-citizen atlas pin");
            AssertEqual("route-scaffold-atlas-runtime-v1.30.0.png", RuntimeArtManifest.RouteScaffoldAtlas, "approved v1.30 route scaffold atlas pin");
            AssertEqual("kobold-route-atlas-runtime-v1.30.0.png", RuntimeArtManifest.KoboldRouteAtlas, "approved v1.30 kobold route atlas pin");
            AssertEqual("midgaard-sewer-atlas-runtime-v1.30.0.png", RuntimeArtManifest.MidgaardSewerAtlas, "approved v1.30 sewer atlas pin");
            AssertEqual("npc-portrait-atlas-runtime-v1.60.0.png", RuntimeArtManifest.NpcPortraitAtlas, "approved v1.60 NPC portrait atlas pin");
            AssertEqual("character-combat-atlas-runtime-v1.93.0.png", RuntimeArtManifest.CharacterCombatAtlas, "approved v1.93 character sprite atlas pin");
            AssertEqual("enemy-sprite-atlas-runtime-v1.77.0.png", RuntimeArtManifest.EnemySpriteAtlas, "approved v1.77 enemy sprite atlas pin");
            AssertEqual("demon-summon-atlas-runtime-v1.4.0.png", RuntimeArtManifest.DemonSummonAtlas, "approved demon summon and transformation atlas pin");
            AssertEqual("midgaard-interior-prop-atlas-runtime-v1.61.0.png", RuntimeArtManifest.MidgaardInteriorPropAtlas, "approved v1.61 interior prop atlas pin");
            AssertEqual("midgaard-interior-tile-atlas-runtime-v1.61.0.png", RuntimeArtManifest.MidgaardInteriorTileAtlas, "approved v1.61 interior tile atlas pin");
            AssertEqual("grand-hearth-floor-atlas-runtime-v2.7.0.png", RuntimeArtManifest.GrandHearthFloorAtlas, "approved v2.7 Grand Hearth floor atlas pin");
            AssertEqual("grand-hearth-setpiece-atlas-runtime-v2.7.0.png", RuntimeArtManifest.GrandHearthSetpieceAtlas, "approved v2.7 Grand Hearth set-piece atlas pin");
            AssertEqual("grand-hearth-ambience-atlas-runtime-v2.8.0.png", RuntimeArtManifest.GrandHearthAmbienceAtlas, "approved v2.8 Grand Hearth ambience atlas pin");
            AssertEqual("ash-and-brimstone-title-card-runtime-v1.64.0.png", RuntimeArtManifest.TitleCard, "approved v1.64 title-card pin");
            AssertEqual("ash-and-brimstone-icon-runtime-v1.61.0.png", RuntimeArtManifest.GameIcon, "approved v1.61 game-icon pin");
            AssertEqual("roaming-threat-atlas-runtime-v1.62.0.png", RuntimeArtManifest.RoamingThreatAtlas, "approved v1.62 roaming-threat atlas pin");
            AssertEqual(
                "ability-icon-atlas-runtime-v2.9.0.png|signature-spell-icon-atlas-runtime-v2.9.0.png|lightning-spell-icon-atlas-runtime-v1.97.0.png|power-book-state-icon-atlas-runtime-v1.97.0.png|combat-command-icon-atlas-runtime-v1.99.0.png|magic-ui-atlas-runtime-v1.31.0.png|spell-animation-atlas-runtime-v1.49.0.png|combat-spell-effects-atlas-runtime-v2.9.0.png|mage-warlock-spell-vfx-atlas-runtime-v2.13.0.png|support-hex-spell-vfx-atlas-runtime-v2.14.0.png|class-skill-vfx-atlas-runtime-v2.14.0.png|combat-power-travel-vfx-atlas-runtime-v2.15.0.png|combat-power-aftermath-vfx-atlas-runtime-v2.17.0.png|unique-item-atlas-runtime-v2.20.0.png|title-backdrop-runtime-v2.4.0.png|tavern-ui-atlas-runtime-v1.5.9.png|title-menu-scroll-runtime-v2.12.1.png|title-menu-focus-runtime-v2.12.1.png|title-menu-icon-atlas-runtime-v2.16.0.png|midgaard-gate-atlas-runtime-v1.93.0.png|midgaard-wall-atlas-runtime-v1.91.0.png|world-map-exploration-tile-atlas-runtime-v1.68.0.png|world-map-material-atlas-runtime-v1.92.0.png|world-map-overlay-atlas-runtime-v0.80.png|world-map-progression-overlay-atlas-runtime-v0.63.png|world-map-ui-atlas-runtime-v1.6.0.png|world-map-token-sprite-atlas-runtime-v1.91.0.png|world-map-prop-atlas-runtime-v1.29.0.png|world-map-biome-prop-atlas-runtime-v1.29.0.png|world-map-landmark-atlas-runtime-v1.29.0.png|world-map-region-landmark-atlas-runtime-v1.65.0.png|world-map-region-marker-atlas-runtime-v2.2.0.png|world-area-setpiece-atlas-runtime-v2.3.0.png|world-threat-habitat-atlas-runtime-v2.4.0.png|player-exploration-role-atlas-runtime-v2.4.0.png|midgaard-town-atlas-runtime-v2.21.0.png|midgaard-tile-atlas-runtime-v1.6.3.png|midgaard-city-prop-atlas-runtime-v1.29.0.png|midgaard-street-life-atlas-runtime-v1.50.0.png|midgaard-paving-decal-atlas-runtime-v1.50.0.png|midgaard-road-surface-atlas-runtime-v2.21.0.png|midgaard-npc-atlas-runtime-v2.21.0.png|world-npc-citizen-atlas-runtime-v2.21.0.png|route-scaffold-atlas-runtime-v1.30.0.png|kobold-route-atlas-runtime-v1.30.0.png|midgaard-sewer-atlas-runtime-v1.30.0.png|npc-portrait-atlas-runtime-v1.60.0.png|character-combat-atlas-runtime-v1.93.0.png|enemy-sprite-atlas-runtime-v1.77.0.png|demon-summon-atlas-runtime-v1.4.0.png|midgaard-interior-prop-atlas-runtime-v1.61.0.png|midgaard-interior-tile-atlas-runtime-v1.61.0.png|grand-hearth-floor-atlas-runtime-v2.7.0.png|grand-hearth-setpiece-atlas-runtime-v2.7.0.png|grand-hearth-ambience-atlas-runtime-v2.8.0.png|ash-and-brimstone-title-card-runtime-v1.64.0.png|ash-and-brimstone-icon-runtime-v1.61.0.png|roaming-threat-atlas-runtime-v1.62.0.png",
                string.Join("|", RuntimeArtManifest.ApprovedRuntimeFiles),
                "approved runtime atlas manifest");
            AssertEqual(58, RuntimeArtManifest.ApprovedRuntimeFiles.Distinct().Count(), "approved runtime atlas pins are unique");

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
                    if (string.Equals(fileName, RuntimeArtManifest.MidgaardTownAtlas, StringComparison.Ordinal))
                    {
                        int[] architectureCells = { 0, 1, 3, 4, 5, 6, 7, 11, 14 };
                        AssertAtlasCellCoverageAtAlpha(atlas, 5, 4, architectureCells, 0.34f, 0.55f, 24, "v2.21 Midgaard architecture");
                        foreach (int cell in architectureCells)
                        {
                            RectInt bounds = AtlasCellVisibleBounds(atlas, 5, 4, cell, 24);
                            AssertEqual(true, bounds.width >= 170 && bounds.width <= 220, "v2.21 Midgaard building cell " + cell + " has substantial facade width");
                            AssertEqual(true, bounds.height >= 188 && bounds.height <= 220, "v2.21 Midgaard building cell " + cell + " has roof-led vertical mass");
                        }
                    }
                    if (string.Equals(fileName, RuntimeArtManifest.MidgaardNpcAtlas, StringComparison.Ordinal))
                    {
                        AssertAtlasCellCoverageAtAlpha(atlas, 5, 4, Enumerable.Range(0, 20), 0.18f, 0.40f, 24, "v2.21 named Midgaard NPC");
                        foreach (int cell in Enumerable.Range(0, 20))
                        {
                            RectInt bounds = AtlasCellVisibleBounds(atlas, 5, 4, cell, 24);
                            AssertEqual(true, bounds.height >= 216 && bounds.height <= 220, "v2.21 named NPC cell " + cell + " shares the normalized baseline height");
                            AssertEqual(true, bounds.width >= 90 && bounds.width <= 170, "v2.21 named NPC cell " + cell + " keeps a readable bounded silhouette");
                        }
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
                AssertEqual(new Vector2Int(1536, 768), new Vector2Int(ambientCitizens.width, ambientCitizens.height), "v2.21 ambient-citizen dimensions");
                AssertEqual(new Vector2Int(1536, 768), new Vector2Int(playerRoles.width, playerRoles.height), "v2.4 player-role dimensions");
                AssertAtlasCellCoverageAtAlpha(threatHabitats, 4, 2, Enumerable.Range(0, 8), 0.46f, 0.51f, 8, "v2.4 threat habitat");
                AssertAtlasCellCoverageAtAlpha(ambientCitizens, 4, 2, Enumerable.Range(0, 8), 0.25f, 0.38f, 8, "v2.21 ambient citizen");
                AssertAtlasCellCoverageAtAlpha(playerRoles, 4, 2, Enumerable.Range(0, 8), 0.22f, 0.43f, 8, "v2.4 player role");
                AssertAtlasCellSafeGutter(threatHabitats, 4, 2, Enumerable.Range(0, 8), 20, 8, 0, "v2.4 threat habitat");
                AssertAtlasCellSafeGutter(ambientCitizens, 4, 2, Enumerable.Range(0, 8), 20, 8, 0, "v2.21 ambient citizen");
                AssertAtlasCellSafeGutter(playerRoles, 4, 2, Enumerable.Range(0, 8), 20, 8, 0, "v2.4 player role");
                AssertAtlasHasNoVisibleBrightMagenta(threatHabitats, 8, "v2.4 threat habitat");
                AssertAtlasHasNoVisibleBrightMagenta(ambientCitizens, 8, "v2.21 ambient citizen");
                AssertAtlasHasNoVisibleBrightMagenta(playerRoles, 8, "v2.4 player role");
                for (int cell = 0; cell < ExplorationCharacterArtCatalog.CitizenCellCount; cell++)
                {
                    RectInt bounds = AtlasCellVisibleBounds(ambientCitizens, 4, 2, cell, 8);
                    AssertEqual(20, bounds.y, "v2.21 ambient citizen cell " + cell + " keeps the exact top gutter");
                    AssertEqual(344, bounds.height, "v2.21 ambient citizen cell " + cell + " uses the shared figure height");
                    AssertEqual(20, 384 - bounds.yMax, "v2.21 ambient citizen cell " + cell + " keeps the shared baseline gutter");
                }

                Texture2D streetLife = LoadApprovedRuntimeAtlas(RuntimeArtManifest.MidgaardStreetLifeAtlas);
                Texture2D pavingDecals = LoadApprovedRuntimeAtlas(RuntimeArtManifest.MidgaardPavingDecalAtlas);
                Texture2D roadSurfaces = LoadApprovedRuntimeAtlas(RuntimeArtManifest.MidgaardRoadSurfaceAtlas);
                normalizedAtlases.Add(streetLife);
                normalizedAtlases.Add(pavingDecals);
                normalizedAtlases.Add(roadSurfaces);
                AssertEqual(new Vector2Int(1400, 1120), new Vector2Int(streetLife.width, streetLife.height), "v1.50 Midgaard street-life dimensions");
                AssertEqual(new Vector2Int(1252, 1252), new Vector2Int(pavingDecals.width, pavingDecals.height), "v1.50 Midgaard paving-decal dimensions");
                AssertEqual(new Vector2Int(512, 512), new Vector2Int(roadSurfaces.width, roadSurfaces.height), "v2.21 Midgaard road-surface dimensions");
                AssertAtlasCellCoverage(streetLife, 5, 4, Enumerable.Range(0, 20), 0.10f, 0.92f, "v1.50 street-life prop");
                AssertAtlasCellCoverage(pavingDecals, 4, 4, Enumerable.Range(0, 16), 0.08f, 0.92f, "v1.50 paving decal");
                AssertAtlasCellCoverage(roadSurfaces, 2, 2, Enumerable.Range(0, 4), 0.99f, 1f, "v2.21 seamless road surface");

                Texture2D interiorProps = LoadApprovedRuntimeAtlas(RuntimeArtManifest.MidgaardInteriorPropAtlas);
                Texture2D interiorTiles = LoadApprovedRuntimeAtlas(RuntimeArtManifest.MidgaardInteriorTileAtlas);
                Texture2D grandHearthFloor = LoadApprovedRuntimeAtlas(RuntimeArtManifest.GrandHearthFloorAtlas);
                Texture2D grandHearthSetpieces = LoadApprovedRuntimeAtlas(RuntimeArtManifest.GrandHearthSetpieceAtlas);
                Texture2D grandHearthAmbience = LoadApprovedRuntimeAtlas(RuntimeArtManifest.GrandHearthAmbienceAtlas);
                Texture2D titleCard = LoadApprovedRuntimeAtlas(RuntimeArtManifest.TitleCard);
                Texture2D gameIcon = LoadApprovedRuntimeAtlas(RuntimeArtManifest.GameIcon);
                normalizedAtlases.Add(interiorProps);
                normalizedAtlases.Add(interiorTiles);
                normalizedAtlases.Add(grandHearthFloor);
                normalizedAtlases.Add(grandHearthSetpieces);
                normalizedAtlases.Add(grandHearthAmbience);
                normalizedAtlases.Add(titleCard);
                normalizedAtlases.Add(gameIcon);
                AssertEqual(new Vector2Int(1400, 1120), new Vector2Int(interiorProps.width, interiorProps.height), "v1.61 Midgaard interior-prop dimensions");
                AssertEqual(new Vector2Int(1400, 1120), new Vector2Int(interiorTiles.width, interiorTiles.height), "v1.61 Midgaard interior-tile dimensions");
                AssertAtlasCellCoverage(interiorProps, 5, 4, Enumerable.Range(0, 20), 0.04f, 0.92f, "v1.61 interior prop");
                AssertAtlasCellCoverage(interiorTiles, 5, 4, Enumerable.Range(0, 20), 0.99f, 1f, "v1.61 interior terrain");
                AssertEqual(new Vector2Int(1536, 1024), new Vector2Int(grandHearthFloor.width, grandHearthFloor.height), "v2.7 Grand Hearth floor dimensions");
                AssertEqual(new Vector2Int(1536, 1024), new Vector2Int(grandHearthSetpieces.width, grandHearthSetpieces.height), "v2.7 Grand Hearth set-piece dimensions");
                AssertEqual(new Vector2Int(1536, 1024), new Vector2Int(grandHearthAmbience.width, grandHearthAmbience.height), "v2.8 Grand Hearth ambience dimensions");
                AssertAtlasCellCoverageAtAlpha(
                    grandHearthFloor,
                    GrandHearthArtCatalog.FloorAtlasColumns,
                    GrandHearthArtCatalog.FloorAtlasRows,
                    Enumerable.Range(0, GrandHearthArtCatalog.FloorAtlasCellCount),
                    1f,
                    1f,
                    byte.MaxValue,
                    "v2.7 Grand Hearth floor");
                AssertAtlasCellCoverageAtAlpha(
                    grandHearthSetpieces,
                    GrandHearthArtCatalog.SetpieceAtlasColumns,
                    GrandHearthArtCatalog.SetpieceAtlasRows,
                    Enumerable.Range(0, GrandHearthArtCatalog.SetpieceAtlasCellCount),
                    1f / (512f * 512f),
                    1f,
                    8,
                    "v2.7 Grand Hearth set-piece");
                AssertAtlasCellSafeGutter(
                    grandHearthSetpieces,
                    GrandHearthArtCatalog.SetpieceAtlasColumns,
                    GrandHearthArtCatalog.SetpieceAtlasRows,
                    Enumerable.Range(0, GrandHearthArtCatalog.SetpieceAtlasCellCount),
                    32,
                    8,
                    0,
                    "v2.7 Grand Hearth set-piece");
                AssertAtlasHasNoVisibleBrightMagenta(grandHearthSetpieces, 8, "v2.7 Grand Hearth set-piece");
                AssertAtlasCellCoverageAtAlpha(
                    grandHearthAmbience,
                    GrandHearthArtCatalog.AmbienceAtlasColumns,
                    GrandHearthArtCatalog.AmbienceAtlasRows,
                    Enumerable.Range(0, GrandHearthArtCatalog.AmbienceAtlasCellCount),
                    1f / (512f * 512f),
                    0.90f,
                    8,
                    "v2.8 Grand Hearth ambience");
                AssertAtlasCellSafeGutter(
                    grandHearthAmbience,
                    GrandHearthArtCatalog.AmbienceAtlasColumns,
                    GrandHearthArtCatalog.AmbienceAtlasRows,
                    Enumerable.Range(0, GrandHearthArtCatalog.AmbienceAtlasCellCount),
                    24,
                    8,
                    0,
                    "v2.8 Grand Hearth ambience");
                AssertAtlasHasNoVisibleBrightMagenta(grandHearthAmbience, 8, "v2.8 Grand Hearth ambience");
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
            int residue = CountVisibleBrightMagenta(texture, visibleAlphaThreshold);
            AssertEqual(0, residue, label + " has no visible bright-magenta residue");
        }

        private static void AssertAtlasHasNoBrightMagentaKeyField(
            Texture2D texture,
            byte visibleAlphaThreshold,
            int maximumHighlightPixels,
            string label)
        {
            AssertEqual(true, texture != null, label + " atlas loads for chroma inspection");
            int residue = CountVisibleBrightMagenta(texture, visibleAlphaThreshold);
            AssertEqual(
                true,
                residue <= maximumHighlightPixels,
                label + " retains only bounded authored magenta highlights, never a chroma-key field");
        }

        private static int CountVisibleBrightMagenta(Texture2D texture, byte visibleAlphaThreshold)
        {
            if (texture == null) return 0;
            return texture.GetPixels32().Count(pixel =>
                pixel.a >= visibleAlphaThreshold
                && pixel.r >= 248
                && pixel.g <= 8
                && pixel.b >= 248);
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
            AssertEqual(true, localBuildingFill >= 1.44f && localBuildingFill <= 1.46f, "v2.21 local Midgaard buildings carry a roof-led architectural silhouette");
            AssertEqual(true, regionBuildingFill >= 1.05f && regionBuildingFill <= 1.07f, "v2.21 region Midgaard buildings remain landmark-readable silhouettes");
            AssertEqual(true, ExplorationArtRules.MidgaardBuildingSpriteScale(ObjectType.KingHall, false) > ExplorationArtRules.MidgaardBuildingSpriteScale(ObjectType.Market, false), "Town Hall owns the strongest local civic skyline weight");
            AssertEqual(true, ExplorationArtRules.MidgaardBuildingSpriteScale(ObjectType.Temple, true) > ExplorationArtRules.MidgaardBuildingSpriteScale(ObjectType.Provisions, true), "Temple remains distinct from ordinary Region Map shopfronts");
            AssertEqual(true, ExplorationArtRules.MidgaardBuildingVerticalOffset(false) < ExplorationArtRules.MidgaardBuildingVerticalOffset(true), "local building growth favors the roofline over the street");
            foreach (ObjectType buildingType in new[] { ObjectType.Market, ObjectType.Temple, ObjectType.Tavern, ObjectType.KingHall })
            {
                foreach (bool wideView in new[] { false, true })
                {
                    float scale = ExplorationArtRules.MidgaardBuildingSpriteScale(buildingType, wideView);
                    float offset = ExplorationArtRules.MidgaardBuildingVerticalOffset(buildingType, wideView);
                    AssertEqual(true, Mathf.Abs(0.5f + offset + scale * 0.5f - 1f) < 0.001f, buildingType + " keeps its doorway on the cell baseline");
                }
            }

            Dictionary<ObjectType, int> townAtlasCells = new Dictionary<ObjectType, int>
            {
                { ObjectType.Market, 0 },
                { ObjectType.Temple, 1 },
                { ObjectType.Fountain, 2 },
                { ObjectType.Tavern, 3 },
                { ObjectType.Armorer, 4 },
                { ObjectType.Provisions, 5 },
                { ObjectType.WeaponVendor, 6 },
                { ObjectType.Enchanter, 7 },
                { ObjectType.KingHall, 11 },
                { ObjectType.Sewer, 12 },
                { ObjectType.CityWall, 13 },
                { ObjectType.Diner, 14 },
                { ObjectType.RatPeltQuest, 15 },
                { ObjectType.RecallCircle, 17 }
            };
            foreach (KeyValuePair<ObjectType, int> entry in townAtlasCells)
            {
                AssertEqual(entry.Value, MidgaardTownArtCatalog.AtlasIndex(entry.Key), entry.Key + " keeps its semantic Midgaard town-atlas cell");
                AssertEqual(ExplorationArtRules.IsMidgaardBuilding(entry.Key), MidgaardTownArtCatalog.IsArchitectureCell(entry.Value), entry.Key + " atlas cell agrees with its architectural footprint role");
            }
            AssertEqual(-1, MidgaardTownArtCatalog.AtlasIndex(ObjectType.Encounter), "non-city encounters never borrow a Midgaard building cell");
            AssertEqual(ObjectType.KingHall, MidgaardTownArtCatalog.PresentationType(ObjectType.Tavern, true), "Grand Hearth exterior uses Town Hall art and skyline weight");
            AssertEqual(ObjectType.Tavern, MidgaardTownArtCatalog.PresentationType(ObjectType.Tavern, false), "ordinary taverns retain their own building presentation");

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
            AssertEqual(
                false,
                ExplorationArtRules.ShouldDrawMaterialPathStroke(
                    ExplorationMaterial.PackedDirt,
                    ExplorationCellRole.Road | ExplorationCellRole.Room | ExplorationCellRole.Clearing,
                    true),
                "authored regional rooms rely on painted route material instead of a circuit-board stroke");
            AssertEqual(
                true,
                ExplorationArtRules.ShouldDrawMaterialPathStroke(
                    ExplorationMaterial.PackedDirt,
                    ExplorationCellRole.Road | ExplorationCellRole.Room | ExplorationCellRole.Threshold,
                    true),
                "authored regional thresholds keep a visible outside approach stroke");
            AssertEqual(
                true,
                ExplorationArtRules.ShouldDrawMaterialPathStroke(
                    ExplorationMaterial.PackedDirt,
                    ExplorationCellRole.Road | ExplorationCellRole.Room,
                    false),
                "procedural roads crossing ordinary rooms keep their visible route stroke");

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

        private static void ExplorationRoadPresentationRulesKeepRoutesReadable()
        {
            ExplorationRoadJoin[] expectedJoins =
            {
                ExplorationRoadJoin.None,
                ExplorationRoadJoin.Endpoint,
                ExplorationRoadJoin.Endpoint,
                ExplorationRoadJoin.Corner,
                ExplorationRoadJoin.Endpoint,
                ExplorationRoadJoin.Straight,
                ExplorationRoadJoin.Corner,
                ExplorationRoadJoin.Tee,
                ExplorationRoadJoin.Endpoint,
                ExplorationRoadJoin.Corner,
                ExplorationRoadJoin.Straight,
                ExplorationRoadJoin.Tee,
                ExplorationRoadJoin.Corner,
                ExplorationRoadJoin.Tee,
                ExplorationRoadJoin.Tee,
                ExplorationRoadJoin.Cross
            };
            for (int mask = 0; mask <= ExplorationRoadPresentationRules.CardinalMask; mask++)
            {
                AssertEqual(
                    expectedJoins[mask],
                    ExplorationRoadPresentationRules.JoinForMask(mask),
                    $"road presentation classifies cardinal join mask {mask}");
                AssertEqual(
                    expectedJoins[mask],
                    ExplorationRoadPresentationRules.JoinForMask(mask | 0x70),
                    $"road presentation ignores non-cardinal bits for join mask {mask}");
                AssertEqual(
                    expectedJoins[mask] != ExplorationRoadJoin.Straight,
                    ExplorationRoadPresentationRules.ShouldDrawJunctionApron(expectedJoins[mask]),
                    $"road join mask {mask} reserves the central apron for actual endpoints and junctions");
            }

            ExplorationCellRole trail = ExplorationCellRole.Trail;
            AssertEqual(
                false,
                ExplorationRoadPresentationRules.ShouldDrawTrail(
                    true,
                    trail,
                    0,
                    ExplorationRoadPresentationRules.East | ExplorationRoadPresentationRules.West),
                "Region Map suppresses pure trail strokes beneath its strategic road read");
            ExplorationRoadVisualPlan regionTrail = ExplorationRoadPresentationRules.Resolve(
                trail,
                true,
                false,
                0,
                ExplorationRoadPresentationRules.East | ExplorationRoadPresentationRules.West,
                ExplorationRoadPresentationRules.East | ExplorationRoadPresentationRules.West);
            AssertEqual(false, regionTrail.Draw, "Region Map pure trails resolve to no visual plan");

            AssertEqual(
                false,
                ExplorationRoadPresentationRules.ShouldDrawTrail(
                    false,
                    trail,
                    ExplorationRoadPresentationRules.North,
                    0),
                "a Local Map trail dangling from a road receives no decorative sidecar stroke");
            AssertEqual(
                false,
                ExplorationRoadPresentationRules.ShouldDrawTrail(
                    false,
                    trail,
                    ExplorationRoadPresentationRules.North,
                    ExplorationRoadPresentationRules.East | ExplorationRoadPresentationRules.West),
                "a Local Map trail parallel to a road receives no ladder-like sidecar stroke");
            AssertEqual(
                true,
                ExplorationRoadPresentationRules.ShouldDrawTrail(
                    false,
                    trail,
                    ExplorationRoadPresentationRules.North,
                    ExplorationRoadPresentationRules.South),
                "a Local Map trail continuing directly away from a road retains its real departure");
            AssertEqual(
                true,
                ExplorationRoadPresentationRules.ShouldDrawTrail(
                    false,
                    trail | ExplorationCellRole.Threshold,
                    ExplorationRoadPresentationRules.North,
                    0),
                "a Local Map threshold anchors its trail even without a continuation");
            AssertEqual(
                true,
                ExplorationRoadPresentationRules.ShouldDrawTrail(
                    false,
                    trail | ExplorationCellRole.Clearing,
                    ExplorationRoadPresentationRules.East,
                    ExplorationRoadPresentationRules.North),
                "a Local Map clearing anchors its trail through a non-collinear meeting");

            foreach (bool wideView in new[] { false, true })
            {
                ExplorationRoadVisualPlan city = ExplorationRoadPresentationRules.Resolve(
                    ExplorationCellRole.Road | ExplorationCellRole.City,
                    wideView,
                    false,
                    ExplorationRoadPresentationRules.East | ExplorationRoadPresentationRules.West,
                    0,
                    0);
                ExplorationRoadVisualPlan road = ExplorationRoadPresentationRules.Resolve(
                    ExplorationCellRole.Road,
                    wideView,
                    false,
                    ExplorationRoadPresentationRules.East | ExplorationRoadPresentationRules.West,
                    0,
                    0);
                ExplorationRoadVisualPlan bridge = ExplorationRoadPresentationRules.Resolve(
                    ExplorationCellRole.Road | ExplorationCellRole.Bridge,
                    wideView,
                    false,
                    ExplorationRoadPresentationRules.East | ExplorationRoadPresentationRules.West,
                    0,
                    0);
                ExplorationRoadVisualPlan oldRoad = ExplorationRoadPresentationRules.Resolve(
                    ExplorationCellRole.Road,
                    wideView,
                    true,
                    ExplorationRoadPresentationRules.East | ExplorationRoadPresentationRules.West,
                    0,
                    0);
                ExplorationRoadVisualPlan civicOldRoad = ExplorationRoadPresentationRules.Resolve(
                    ExplorationCellRole.Road | ExplorationCellRole.City,
                    wideView,
                    true,
                    ExplorationRoadPresentationRules.East | ExplorationRoadPresentationRules.West,
                    0,
                    0);

                AssertEqual(ExplorationRoadVisualTier.CityStreet, city.Tier, $"{(wideView ? "Region" : "Local")} Map resolves the city-street visual tier");
                AssertEqual(ExplorationRoadVisualTier.Road, road.Tier, $"{(wideView ? "Region" : "Local")} Map resolves the ordinary-road visual tier");
                AssertEqual(ExplorationRoadVisualTier.Bridge, bridge.Tier, $"{(wideView ? "Region" : "Local")} Map resolves the bridge visual tier");
                AssertEqual(ExplorationRoadVisualTier.OldRoad, oldRoad.Tier, $"{(wideView ? "Region" : "Local")} Map resolves the Old Road visual tier");
                AssertEqual(true, city.CivicSurface, $"{(wideView ? "Region" : "Local")} Map gives city streets a civic cobble surface");
                AssertEqual(false, road.CivicSurface, $"{(wideView ? "Region" : "Local")} Map keeps wilderness roads earthen");
                AssertEqual(false, oldRoad.CivicSurface, $"{(wideView ? "Region" : "Local")} Map keeps the Old Road earthen outside Midgaard");
                AssertEqual(true, civicOldRoad.CivicSurface, $"{(wideView ? "Region" : "Local")} Map dresses the Old Road as restrained cobble inside Midgaard");
                AssertEqual(
                    true,
                    oldRoad.ShoulderFraction > bridge.ShoulderFraction
                        && bridge.ShoulderFraction > road.ShoulderFraction
                        && road.ShoulderFraction > city.ShoulderFraction,
                    $"{(wideView ? "Region" : "Local")} Map keeps Old Road, bridge, road, and city shoulders in readable hierarchy");
                AssertEqual(
                    true,
                    oldRoad.CoreFraction > bridge.CoreFraction
                        && bridge.CoreFraction > road.CoreFraction
                        && road.CoreFraction > city.CoreFraction,
                    $"{(wideView ? "Region" : "Local")} Map keeps Old Road, bridge, road, and city cores in readable hierarchy");
                foreach (ExplorationRoadVisualPlan plan in new[] { city, road, bridge, oldRoad })
                {
                    AssertEqual(true, plan.Draw, $"{(wideView ? "Region" : "Local")} Map draws {plan.Tier}");
                    AssertEqual(true, plan.ShoulderFraction > plan.CoreFraction && plan.CoreFraction > 0f, $"{(wideView ? "Region" : "Local")} Map bounds {plan.Tier} core inside its shoulder");
                    AssertEqual(true, plan.ShoulderFraction < 1f, $"{(wideView ? "Region" : "Local")} Map keeps {plan.Tier} inside one map cell");
                    AssertEqual(false, plan.DrawJunctionApron, $"{(wideView ? "Region" : "Local")} Map renders straight {plan.Tier} as one uninterrupted strip");
                }
            }

            int fullCross = ExplorationRoadPresentationRules.CardinalMask;
            ExplorationRoadVisualPlan oldRoadCross = ExplorationRoadPresentationRules.Resolve(
                ExplorationCellRole.Road,
                false,
                true,
                fullCross,
                0,
                0);
            AssertEqual(
                ExplorationRoadPresentationRules.East | ExplorationRoadPresentationRules.West,
                oldRoadCross.MainMask,
                "Old Road keeps its broad main carriage stroke east-west at a four-way meeting");
            AssertEqual(
                ExplorationRoadPresentationRules.North | ExplorationRoadPresentationRules.South,
                oldRoadCross.ConnectorMask,
                "Old Road retains north-south road branches as subordinate connectors");
            AssertEqual(ExplorationRoadJoin.Straight, oldRoadCross.Join, "Old Road join identity follows its east-west main carriage stroke");
            AssertEqual(true, oldRoadCross.DrawCenterWear, "Local straight Old Road retains its authored center wear");

            ExplorationRoadVisualPlan regionOldRoadCross = ExplorationRoadPresentationRules.Resolve(
                ExplorationCellRole.Road,
                true,
                true,
                fullCross,
                0,
                0);
            AssertEqual(oldRoadCross.MainMask, regionOldRoadCross.MainMask, "Region Old Road preserves the same east-west main topology");
            AssertEqual(oldRoadCross.ConnectorMask, regionOldRoadCross.ConnectorMask, "Region Old Road preserves real north-south road branches");
            AssertEqual(false, regionOldRoadCross.DrawCenterWear, "Region Old Road suppresses Local-only center wear");

            ExplorationRoadVisualPlan civicEndpoint = ExplorationRoadPresentationRules.Resolve(
                ExplorationCellRole.Road | ExplorationCellRole.City,
                false,
                false,
                ExplorationRoadPresentationRules.East,
                0,
                0);
            ExplorationRoadVisualPlan earthenEndpoint = ExplorationRoadPresentationRules.Resolve(
                ExplorationCellRole.Road,
                false,
                false,
                ExplorationRoadPresentationRules.East,
                0,
                0);
            AssertEqual(
                false,
                ExplorationRoadPresentationRules.ShouldDrawGenericSurfaceChip(civicEndpoint, false, 1),
                "a civic endpoint cannot place a stray generic chip on its absent half");
            AssertEqual(
                true,
                ExplorationRoadPresentationRules.ShouldDrawGenericSurfaceChip(earthenEndpoint, false, 1),
                "a Local earthen endpoint retains sparse material variation");
            AssertEqual(
                false,
                ExplorationRoadPresentationRules.ShouldDrawGenericSurfaceChip(earthenEndpoint, true, 1),
                "Region roads suppress cell-scale material chips");
        }

        private static void CombatHudScreenLayoutFitsSupportedResolutions()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(960, 600),
                new Vector2Int(1280, 720),
                new Vector2Int(1600, 900),
                new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152)
            };
            float[] minimumTileSizes = { 49f, 70f, 96f, 118f, 128f };

            for (int sizeIndex = 0; sizeIndex < sizes.Length; sizeIndex++)
            {
                Vector2Int size = sizes[sizeIndex];
                CombatHudGeometry geometry = CombatHudScreenLayout.Calculate(size.x, size.y);
                bool compactViewport = size.x <= 1024;
                AssertEqual(true, geometry.Fits(size.x, size.y), $"combat HUD layout fits {size.x}x{size.y}");
                AssertEqual(true, geometry.Top.height >= 50f && geometry.Top.height <= 56f, $"combat top ribbon stays compact and readable at {size.x}x{size.y}");
                AssertEqual(true, compactViewport ? Mathf.Approximately(geometry.Command.width, 84f) : geometry.Command.width >= 96f && geometry.Command.width <= 112f, $"combat commands use the adaptive vertical palette at {size.x}x{size.y}");
                AssertEqual(true, !compactViewport || Mathf.Approximately(geometry.Side.width, 240f), $"compact combat dossier returns width to the battlefield at {size.x}x{size.y}");
                float maximumDossierShare = compactViewport ? 0.25f : 0.22f;
                AssertEqual(true, geometry.Side.width <= size.x * maximumDossierShare, $"combat dossier leaves the battlefield as the visual hero at {size.x}x{size.y}");
                Rect grid = CombatHudScreenLayout.BoardInner(geometry.Board, 12, 8);
                float tileSize = Mathf.Min(grid.width / 12f, grid.height / 8f);
                AssertEqual(true, tileSize >= minimumTileSizes[sizeIndex], $"combat battlefield preserves large tiles at {size.x}x{size.y} ({tileSize:0.0}px)");
                float minimumBattlefieldShare = compactViewport ? 0.40f : 0.50f;
                AssertEqual(true, grid.width * grid.height >= size.x * size.y * minimumBattlefieldShare, $"combat battlefield owns its intended frame share at {size.x}x{size.y}");
                foreach (bool promoteEndTurn in new[] { false, true })
                {
                    Rect[] buttons = CombatHudScreenLayout.CommandButtons(geometry.Command.width, geometry.Command.height, promoteEndTurn);
                    AssertEqual(6, buttons.Length, "combat HUD keeps all six combat commands visible");
                    foreach (Rect button in buttons)
                    {
                        AssertEqual(true, button.xMin >= 0f && button.yMin >= 0f && button.xMax <= geometry.Command.width && button.yMax <= geometry.Command.height, $"combat command button fits {size.x}x{size.y} promoted={promoteEndTurn}");
                        AssertEqual(true, button.width >= 72f && button.height >= 80f, $"combat command keeps a generous pointer and controller target at {size.x}x{size.y}");
                        float iconSize = CombatHudScreenLayout.CommandIconSize(button);
                        AssertEqual(true, iconSize >= 52f, $"combat command art remains readable at {size.x}x{size.y}");
                        float iconTop = CombatHudScreenLayout.UsesCompactCommandLayout(button) ? 4f : 6f;
                        float labelHeight = CombatHudScreenLayout.UsesCompactCommandLayout(button) ? 15f : 17f;
                        AssertEqual(true, iconTop + iconSize + 1f + labelHeight <= button.height, $"combat command art clears its visible label at {size.x}x{size.y}");
                        Rect stateTag = CombatHudScreenLayout.CommandStateTagRect(button);
                        Rect hotkey = CombatHudScreenLayout.CommandHotkeyRect(button);
                        AssertEqual(true, stateTag.xMax <= hotkey.xMin, $"combat command state and hotkey badges never overlap at {size.x}x{size.y}");
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
                AssertEqual(compactViewport ? 4 : 6, CombatHudScreenLayout.TurnChipCapacity(geometry.Side.width), $"combat turn order uses the readable capacity at {size.x}x{size.y}");
                AssertEqual(compactViewport ? 2 : 3, CombatHudScreenLayout.TurnChipColumns(geometry.Side.width), $"combat turn order uses readable columns at {size.x}x{size.y}");
                AssertEqual(true, CombatHudScreenLayout.TurnChipNameWidth(geometry.Side.width) >= (compactViewport ? 60f : 34f), $"combat turn names retain a readable well at {size.x}x{size.y}");
                AssertEqual(true, CombatHudScreenLayout.TurnChipNameWidth(geometry.Side.width, true) >= (compactViewport ? 47f : 24f), $"next-round markers reserve a non-overlapping turn-name well at {size.x}x{size.y}");
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
                float minimumExpandedTimelineHeight = compactViewport ? 244f : 284f;
                AssertEqual(true, timeline.height >= minimumExpandedTimelineHeight && timeline.height >= collapsedTimelineHeight + 120f, $"expanded combat timeline becomes the intentional information drawer at {size.x}x{size.y}");
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

        private static void V29CombatPresentationPolishRulesStayStable()
        {
            AssertEqual(
                false,
                CombatAbilityModalPointerRules.ShouldCommitPreview("fireball", "spark", true, 10f, 10.05f),
                "pointer preview waits through its hover dwell");
            AssertEqual(
                true,
                CombatAbilityModalPointerRules.ShouldCommitPreview("fireball", "spark", true, 10f, 10.20f),
                "pointer preview commits after a stable hover");
            AssertEqual(
                false,
                CombatAbilityModalPointerRules.ShouldCommitPreview("fireball", "spark", false, 10f, 10.20f),
                "pointer exit cancels a pending preview");
            AssertEqual(
                false,
                CombatAbilityModalPointerRules.ShouldCommitPreview("spark", "spark", true, 10f, 10.20f),
                "hovering the selected power cannot create a second selection rail");

            AssertEqual(
                CombatHudCommandStyleRules.ShootCommandAtlasIndex,
                CombatHudCommandStyleRules.AttackCommandAtlasIndex(true, false),
                "an unengaged ranged attack uses the bow command icon");
            AssertEqual(
                CombatIconCatalog.CombatCommandAttackIndex,
                CombatHudCommandStyleRules.AttackCommandAtlasIndex(true, true),
                "an engaged ranged combatant uses the melee attack icon");
            AssertEqual(
                CombatIconCatalog.CombatCommandAttackIndex,
                CombatHudCommandStyleRules.AttackCommandAtlasIndex(false, false),
                "a melee combatant keeps the sword command icon");

            CombatHudCommandView blocked = new CombatHudCommandView
            {
                Enabled = false,
                Blocked = true,
                DisabledReason = "Out of range."
            };
            CombatHudCommandView armed = new CombatHudCommandView
            {
                Enabled = true,
                Armed = true,
                SubLabel = "Choose a target"
            };
            CombatHudCommandView promoted = new CombatHudCommandView
            {
                Enabled = true,
                Promoted = true
            };
            CombatHudCommandView available = new CombatHudCommandView { Enabled = true, Mode = ActionMode.Move };
            CombatHudCommandView guard = new CombatHudCommandView { Enabled = true, Mode = ActionMode.Guard, SubLabel = "Guard +4" };
            CombatHudCommandView elixir = new CombatHudCommandView { Enabled = true, Mode = ActionMode.Elixir, SubLabel = "2 left" };
            AssertEqual(CombatHudCommandVisualState.Blocked, CombatHudCommandStyleRules.Resolve(blocked), "disabled combat commands resolve to blocked styling");
            AssertEqual("", CombatHudCommandStyleRules.StateTag(CombatHudCommandStyleRules.Resolve(blocked)), "blocked commands avoid repeating their state in a badge");
            AssertEqual("Out of range", CombatHudCommandStyleRules.SecondaryLine(blocked), "blocked commands explain the reason without a redundant prefix");
            AssertEqual("Out of range.", CombatHudCommandStyleRules.PromptDetail(blocked), "blocked command focus repeats the actionable reason in the canonical prompt");
            AssertEqual(CombatHudCommandVisualState.Armed, CombatHudCommandStyleRules.Resolve(armed), "armed targeting owns the strongest command state");
            AssertEqual("ARMED", CombatHudCommandStyleRules.StateTag(CombatHudCommandStyleRules.Resolve(armed)), "armed command receives a targeting tag");
            AssertEqual("Choose a target", CombatHudCommandStyleRules.SecondaryLine(armed), "armed commands do not repeat their state tag in secondary copy");
            AssertEqual(true, CombatHudCommandStyleRules.ShowsPersistentSecondary(armed, false), "armed targeting keeps its useful target detail in the full command deck");
            AssertEqual(false, CombatHudCommandStyleRules.ShowsPersistentSecondary(blocked, false), "blocked commands expose their actionable reason through focus instead of crowding the full command deck");
            AssertEqual(false, CombatHudCommandStyleRules.ShowsPersistentSecondary(available, false), "ordinary commands defer explanatory copy to the contextual prompt");
            AssertEqual(true, CombatHudCommandStyleRules.ShowsPersistentSecondary(guard, false), "Guard keeps its decision-changing bonus in the full command deck");
            AssertEqual(true, CombatHudCommandStyleRules.ShowsPersistentSecondary(elixir, false), "Elixir keeps its shared stock count in the full command deck");
            AssertEqual(true, CombatHudCommandStyleRules.PromptDetail(elixir).Contains("2 left") && CombatHudCommandStyleRules.PromptDetail(elixir).Contains("Recover"), "Elixir focus keeps stock and purpose available when compact copy is hidden");
            AssertEqual(false, CombatHudCommandStyleRules.ShowsPersistentSecondary(armed, true), "compact commands defer secondary copy to the contextual prompt");
            AssertEqual(false, CombatHudCommandStyleRules.ShowsPersistentSecondary(elixir, true), "compact Elixir exposes stock through focus instead of crowding its button");
            AssertEqual("NEXT", CombatHudCommandStyleRules.StateTag(CombatHudCommandStyleRules.Resolve(promoted)), "promoted end-turn command advertises the next combatant");
            AssertEqual("Finish turn", CombatHudCommandStyleRules.SecondaryLine(promoted), "promoted end-turn copy avoids a second readiness label");

            AssertEqual(
                MusicDirectorRules.CombatDrow,
                CombatMusicPresentationRules.StableBaseTrack("", MusicDirectorRules.CombatDrow),
                "combat establishes one encounter music identity");
            AssertEqual(
                MusicDirectorRules.CombatBoss,
                CombatMusicPresentationRules.StableBaseTrack(MusicDirectorRules.CombatGeneric, MusicDirectorRules.CombatBoss),
                "a boss reveal may escalate an ordinary encounter score once");
            AssertEqual(
                MusicDirectorRules.CombatDrow,
                CombatMusicPresentationRules.StableBaseTrack(MusicDirectorRules.CombatGeneric, MusicDirectorRules.CombatDrow),
                "the first authored faction score may replace a bootstrap generic cue");
            AssertEqual(
                MusicDirectorRules.CombatDrow,
                CombatMusicPresentationRules.StableBaseTrack(MusicDirectorRules.CombatDrow, MusicDirectorRules.CombatKobold),
                "later roster changes cannot churn an established faction score");
            AssertEqual(
                MusicDirectorRules.CombatBoss,
                CombatMusicPresentationRules.StableBaseTrack(MusicDirectorRules.CombatBoss, MusicDirectorRules.CombatGeneric),
                "boss music cannot be downgraded by later roster changes");
            AssertEqual(false, CombatMusicPresentationRules.IsRecoveredPartyHealth(0, 40), "a defeated party cannot be misclassified as recovered");
            AssertEqual(false, CombatMusicPresentationRules.ShouldEnterLastStand(MusicDirectorRules.CombatGeneric, false, 10, 40, 0.50f, 8f), "last-stand music waits for sustained danger");
            AssertEqual(false, CombatMusicPresentationRules.ShouldEnterLastStand(MusicDirectorRules.CombatGeneric, false, 10, 40, 1.20f, 5f), "last-stand music respects the track dwell floor");
            AssertEqual(true, CombatMusicPresentationRules.ShouldEnterLastStand(MusicDirectorRules.CombatGeneric, false, 10, 40, 1.20f, 7f), "sustained critical health enters last-stand music after dwell");
            AssertEqual(false, CombatMusicPresentationRules.ShouldEnterLastStand(MusicDirectorRules.CombatBoss, false, 10, 40, 2f, 8f), "boss encounters retain their authored score at critical health");
            AssertEqual(false, CombatMusicPresentationRules.ShouldExitLastStand(true, 22, 40, 1f, 8f), "last-stand recovery uses exit hysteresis");
            AssertEqual(true, CombatMusicPresentationRules.ShouldExitLastStand(true, 22, 40, 2.5f, 8f), "sustained recovery exits last-stand music cleanly");

            Rect actor = new Rect(100f, 50f, 80f, 100f);
            CombatSpriteStageGeometry stage = CombatSpriteStageRules.GeometryFor(actor);
            AssertEqual(true, stage.Footprint.xMin >= actor.xMin && stage.Footprint.xMax <= actor.xMax, "sprite footprint remains inside its tactical cell");
            AssertEqual(true, stage.Footlight.xMin >= actor.xMin && stage.Footlight.xMax <= actor.xMax, "sprite footlight remains local to the combatant");
            AssertEqual(true, stage.LeftRim.xMin >= actor.xMin && stage.RightRim.xMax <= actor.xMax, "faction rim lighting remains inside the sprite stage");
            AssertEqual(true, stage.ActiveTick.width > stage.ActiveTickCore.width && stage.ActiveTick.height > 0f, "active-turn tick has a readable outer and inner mark");
            AssertEqual(0.64f, CombatSpriteStageRules.ActivePulse(0f, true), "Reduced Motion keeps a stable active-unit cue");
            AssertEqual(true, CombatSpriteStageRules.ActivePulse(0f, false) != CombatSpriteStageRules.ActivePulse(0.5f, false), "standard active-unit cue pulses subtly over time");
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

        private static void CombatDecisionClarityRulesKeepTurnsActionable()
        {
            AssertEqual(
                true,
                CombatTargetingRules.ShouldDrawPassiveTargetState(CombatTargetHighlightState.Legal),
                "legal targets retain a passive board marker");
            AssertEqual(
                false,
                CombatTargetingRules.ShouldDrawPassiveTargetState(CombatTargetHighlightState.Blocked),
                "blocked targets stay visually quiet until the player inspects them");
            AssertEqual(
                false,
                CombatTargetingRules.ShouldDrawPassiveTargetState(CombatTargetHighlightState.None),
                "unrelated cells never receive passive target state art");
            AssertEqual("RANGE", CombatTargetingRules.BlockedBadge(AttackForecastBlockReason.OutOfRange), "out-of-range inspection badge");
            AssertEqual("LOS", CombatTargetingRules.BlockedBadge(AttackForecastBlockReason.LineOfSight), "line-of-sight inspection badge");
            AssertEqual("ALLY", CombatTargetingRules.BlockedBadge(AttackForecastBlockReason.FriendlyTarget), "friendly-target inspection badge");
            AssertEqual("DOWN", CombatTargetingRules.BlockedBadge(AttackForecastBlockReason.DefeatedTarget), "defeated-target inspection badge");
            AssertEqual("REQ", CombatTargetingRules.BlockedBadge(AttackForecastBlockReason.None), "unknown attack requirement keeps a concise fallback badge");
            AssertEqual("LOS", CombatTargetingRules.BlockedBadge("line of sight blocked"), "power inspection recognizes line-of-sight copy");
            AssertEqual("RANGE", CombatTargetingRules.BlockedBadge("outside spell range"), "power inspection recognizes range copy");
            AssertEqual("MANA", CombatTargetingRules.BlockedBadge("not enough MP"), "power inspection recognizes mana copy");
            AssertEqual("TARGET", CombatTargetingRules.BlockedBadge("choose another target"), "power inspection recognizes target copy");
            AssertEqual("REQ", CombatTargetingRules.BlockedBadge(""), "power inspection keeps an explicit fallback badge");

            AssertEqual(
                ActionMode.Wait,
                CombatTurnFlowRules.DefaultAction(true, true, true, true, true, ActionMode.Cast, true),
                "incapacitated units default directly to End Turn");
            AssertEqual(
                ActionMode.Attack,
                CombatTurnFlowRules.DefaultAction(false, true, true, true, true, ActionMode.Cast, true),
                "a legal basic attack remains the strongest immediate default");
            AssertEqual(
                ActionMode.Move,
                CombatTurnFlowRules.DefaultAction(false, true, false, true, true, ActionMode.Cast, true),
                "movement replaces Attack when no enemy is currently legal");
            AssertEqual(
                ActionMode.Move,
                CombatTurnFlowRules.DefaultAction(false, false, true, true, false, ActionMode.Wait, false),
                "remaining movement stays available after the action is spent");
            AssertEqual(
                ActionMode.Cast,
                CombatTurnFlowRules.DefaultAction(false, true, false, false, true, ActionMode.Cast, true),
                "an actionable spell is selected when neither attacking nor movement helps");
            AssertEqual(
                ActionMode.Ability,
                CombatTurnFlowRules.DefaultAction(false, true, false, false, true, ActionMode.Ability, true),
                "an actionable martial skill receives the same smart fallback");
            AssertEqual(
                ActionMode.Guard,
                CombatTurnFlowRules.DefaultAction(false, true, false, false, false, ActionMode.Wait, true),
                "Guard is the safe fallback for an otherwise action-ready unit");
            AssertEqual(
                ActionMode.Wait,
                CombatTurnFlowRules.DefaultAction(false, false, false, false, false, ActionMode.Wait, false),
                "a unit with no remaining choice defaults to End Turn");

            AssertEqual(true, CombatTurnFlowRules.ShouldResumePostActionMovement(true, true, false, 2, true), "a living hero can spend movement left after acting");
            AssertEqual(false, CombatTurnFlowRules.ShouldResumePostActionMovement(false, true, false, 2, true), "enemy turns never enter player post-action movement");
            AssertEqual(false, CombatTurnFlowRules.ShouldResumePostActionMovement(true, false, false, 2, true), "defeated heroes cannot resume movement");
            AssertEqual(false, CombatTurnFlowRules.ShouldResumePostActionMovement(true, true, true, 2, true), "an unspent action does not trigger post-action movement");
            AssertEqual(false, CombatTurnFlowRules.ShouldResumePostActionMovement(true, true, false, 0, true), "zero movement points proceed to turn completion");
            AssertEqual(false, CombatTurnFlowRules.ShouldResumePostActionMovement(true, true, false, 2, false), "combat resolution prevents a stale movement resume");
            AssertEqual(false, CombatTurnFlowRules.ShouldResumePostActionMovement(true, true, false, 2, true, false), "a boxed-in hero advances instead of entering an empty movement step");
        }

        private static void AdvancedCasterProgressionPowersAreExplicit()
        {
            FormulaDef thunderStep = FormulaCatalog.All.First(formula => formula.Code == "VST");
            FormulaDef tempest = FormulaCatalog.All.First(formula => formula.Code == "AST");
            FormulaDef ascendance = FormulaCatalog.All.First(formula => formula.Code == "DFA");
            FormulaDef riftSeal = FormulaCatalog.All.First(formula => formula.Code == "SRF");
            HashSet<string> validTargets = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase) { "ally", "enemy", "self", "tile" };
            HashSet<string> validEffects = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase) { "chain", "cure", "damage", "dispel", "drain", "heal", "status", "summon", "tempest", "terrain", "teleport", "thunderclap", "transform" };

            AssertEqual(16, FormulaCatalog.RequiredLevel(thunderStep), "Thunder Step unlock level");
            AssertEqual(20, FormulaCatalog.RequiredLevel(tempest), "Arcane Tempest unlock level");
            AssertEqual(18, FormulaCatalog.RequiredLevel(ascendance), "Abyssal Ascendance unlock level");
            AssertEqual(10, FormulaCatalog.RequiredLevel(riftSeal), "Rift Seal unlock level");
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
            AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, "DFA"), "late pact transformation remains in the level-20 progression slice");
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
                && FormulaCatalog.RequiredLevel(riftStep) == 10
                && riftStep.School == "pact"
                && riftStep.Skill == "hex"
                && riftStep.Mana == 6
                && riftStep.Range == 5
                && riftStep.Target == "tile"
                && riftStep.Effect == "teleport"
                && riftStep.DamageType == "death"
                && riftStep.Arc,
                "Rift Step fills the level-ten pact mobility tier");
            AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, "RBT"), "Rift Bolt is available to the sewer-slice warlock");
            AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, "VRS"), "Rift Step remains a later level-20 progression power");
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
            AssertEqual(true, FormulaCatalog.RequiredLevel(thunderclap) == 8
                && thunderclap.Mana == 6
                && thunderclap.Range == 0
                && thunderclap.Target == "self"
                && thunderclap.Effect == "thunderclap"
                && thunderclap.DamageType == "shock"
                && thunderclap.Power == 8, "Thunderclap is the adjacent push-and-collision spell");
            AssertEqual("Chain Lightning", chainLightning.Name, "chain lightning spell name");
            AssertEqual(true, FormulaCatalog.RequiredLevel(chainLightning) == 12
                && chainLightning.Mana == 9
                && chainLightning.Range == 6
                && chainLightning.Target == "enemy"
                && chainLightning.Effect == "chain"
                && chainLightning.DamageType == "shock"
                && chainLightning.Power == 14
                && chainLightning.Arc, "Chain Lightning is the four-target formation spell");
            AssertEqual("Thunder Step", thunderStep.Name, "teleport lightning spell name");
            AssertEqual(true, FormulaCatalog.RequiredLevel(thunderStep) == 16
                && thunderStep.Mana == 8
                && thunderStep.Range == 6
                && thunderStep.Target == "tile"
                && thunderStep.Effect == "teleport"
                && thunderStep.DamageType == "shock"
                && thunderStep.Power == 8
                && thunderStep.Arc, "Thunder Step is the damaging arrival teleport");
            AssertEqual("Arcane Tempest", tempest.Name, "elder lightning spell name");
            AssertEqual(true, FormulaCatalog.RequiredLevel(tempest) == 20
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
            CombatImpactArtPlan reducedFireballPlan = CombatPowerVisualRules.ReducedMotionImpactArtPlan("fireball", 3);
            AssertEqual(3, reducedFireballPlan.PrimaryCell, "Reduced Motion pins Fireball to one impact frame");
            AssertEqual(
                reducedFireballPlan.PrimaryCell,
                CombatPowerVisualRules.ReducedMotionImpactArtPlan("fireball", 3).PrimaryCell,
                "Reduced Motion spell stamps remain frame-stable");

            FormulaDef webSnare = FormulaCatalog.All.First(formula => formula.Code == "WBK");
            FormulaDef sleep = FormulaCatalog.All.First(formula => formula.Code == "RMS");
            FormulaDef weaken = FormulaCatalog.All.First(formula => formula.Code == "RNH");
            FormulaDef nightVeil = FormulaCatalog.All.First(formula => formula.Code == "NVL");
            FormulaDef bind = FormulaCatalog.All.First(formula => formula.Code == "RKW");
            FormulaDef mindBreak = FormulaCatalog.All.First(formula => formula.Code == "RMB");
            FormulaDef dreamSmoke = FormulaCatalog.All.First(formula => formula.Code == "DSM");
            FormulaDef dawnPulse = FormulaCatalog.All.First(formula => formula.Code == "DWP");
            FormulaDef cinderstorm = FormulaCatalog.All.First(formula => formula.Code == "CNS");
            FormulaDef graveHook = FormulaCatalog.All.First(formula => formula.Code == "GRH");
            FormulaDef soulVeil = FormulaCatalog.All.First(formula => formula.Code == "SLV");
            FormulaDef ashenCurse = FormulaCatalog.All.First(formula => formula.Code == "ACR");
            AssertEqual("websnare", CombatPowerVisualRules.ImpactKindForFormula(webSnare, "fieldsnare"), "Web Snare keeps semantic impact art even when audio is generic");
            AssertEqual("sleepmist", CombatPowerVisualRules.ImpactKindForFormula(sleep, "spell"), "Sleep keeps semantic impact art");
            AssertEqual("voidhex", CombatPowerVisualRules.ImpactKindForFormula(weaken, "spell"), "Weaken keeps semantic impact art");
            AssertEqual("shadowveil", CombatPowerVisualRules.ImpactKindForFormula(nightVeil, "spell"), "Night Veil keeps semantic impact art");
            AssertEqual("websnare", CombatPowerVisualRules.ImpactKindForFormula(bind, "spell"), "Bind keeps semantic impact art");
            AssertEqual("voidhex", CombatPowerVisualRules.ImpactKindForFormula(mindBreak, "spell"), "Mind Break keeps semantic impact art");
            AssertEqual("sleepmist", CombatPowerVisualRules.ImpactKindForFormula(dreamSmoke, "spell"), "Dream Smoke keeps semantic impact art");
            AssertEqual("dawnpulse", CombatPowerVisualRules.ImpactKindForFormula(dawnPulse, "spell"), "Dawn Pulse keeps its holy wave identity");
            AssertEqual("cinderstorm", CombatPowerVisualRules.ImpactKindForFormula(cinderstorm, "spell"), "Cinderstorm keeps its fire-field identity");
            AssertEqual("gravehook", CombatPowerVisualRules.ImpactKindForFormula(graveHook, "spell"), "Grave Hook keeps its void tether identity");
            AssertEqual("soulveil", CombatPowerVisualRules.ImpactKindForFormula(soulVeil, "spell"), "Soul Veil keeps its protective void identity");
            AssertEqual("ashencurse", CombatPowerVisualRules.ImpactKindForFormula(ashenCurse, "spell"), "Ashen Curse keeps its cursed-fire identity");
            AssertEqual(CombatPowerVisualMotif.Holy, CombatPowerVisualRules.MotifFor("dawnpulse"), "Dawn Pulse uses the holy motif");
            AssertEqual(CombatPowerVisualMotif.Fire, CombatPowerVisualRules.MotifFor("cinderstorm"), "Cinderstorm uses the fire motif");
            AssertEqual(CombatPowerVisualMotif.Void, CombatPowerVisualRules.MotifFor("gravehook"), "Grave Hook uses the void motif");
            AssertEqual(CombatPowerVisualMotif.Void, CombatPowerVisualRules.MotifFor("soulveil"), "Soul Veil uses the void motif");
            AssertEqual(CombatPowerVisualMotif.Fire, CombatPowerVisualRules.MotifFor("ashencurse"), "Ashen Curse uses the fire motif");
            AssertEqual("12,3,11,11,3", string.Join(",", new[] { "dawnpulse", "cinderstorm", "gravehook", "soulveil", "ashencurse" }.Select(CombatPowerVisualRules.EffectAtlasCell)), "new early spells use their approved effect-atlas cells");
            AssertEqual(CombatPowerVisualMotif.Guard, CombatPowerVisualRules.MotifFor("sunder"), "Sunder uses a guard-breaking motif");
            AssertEqual(CombatPowerVisualMotif.Shadow, CombatPowerVisualRules.MotifFor("shadowstep"), "Shadowstep uses a shadow motif");
            AssertEqual(CombatPowerVisualMotif.Volley, CombatPowerVisualRules.MotifFor("quickshot"), "Quick Shot uses a projectile motif");
            AssertEqual(CombatPowerVisualMotif.Guard, CombatPowerVisualRules.MotifFor(CombatImpactRules.ForAbility(AbilityCatalog.For("sunder")).ImpactSfx), "Sunder production impact routes to the guard-breaking motif");
            AssertEqual(CombatPowerVisualMotif.Shadow, CombatPowerVisualRules.MotifFor(CombatImpactRules.ForAbility(AbilityCatalog.For("shadowstep")).ImpactSfx), "Shadowstep production impact routes to the shadow motif");
            AssertEqual(CombatPowerVisualMotif.Volley, CombatPowerVisualRules.MotifFor(CombatImpactRules.ForAbility(AbilityCatalog.For("quickshot")).ImpactSfx), "Quick Shot production impact routes to the projectile motif");
            AssertEqual("2,2,2", string.Join(",", new[] { "sunder", "shadowstep", "quickshot" }.Select(CombatPowerPresentationRules.AbilityIntensity)), "new early skills receive strong but bounded presentation");
            AssertEqual(true, CombatPowerVisualRules.ReducedMotionStampScale(CombatPowerVisualMotif.Fire, 3) <= 1.10f, "Reduced Motion fire stamp stays inside its tactical cell");
            AssertEqual(true, CombatPowerVisualRules.SemanticImpactOverlayOpacity(CombatPowerVisualMotif.Shadow, 2, true, true) >= 0.80f, "Reduced Motion preserves a readable martial impact mark");
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
                float reducedOpacity = CombatPowerVisualRules.SemanticImpactOverlayOpacity(motif, 3, true, true);
                AssertEqual(motif == CombatPowerVisualMotif.Generic, reducedOpacity == 0f, "Reduced Motion semantic overlay contract remains explicit for " + motif);
                for (int intensity = 1; intensity <= 3; intensity++)
                {
                    float reducedScale = CombatPowerVisualRules.ReducedMotionStampScale(motif, intensity);
                    AssertEqual(true, reducedScale >= 0.86f && reducedScale <= 1.10f, "Reduced Motion stamp stays local for " + motif + " tier " + intensity);
                }
            }
        }

        private static void MageWarlockSpellVfxProfilesStayDistinctAndMotionSafe()
        {
            AssertEqual(4, MageWarlockSpellVfxRules.AtlasColumns, "mage and warlock VFX atlas columns");
            AssertEqual(4, MageWarlockSpellVfxRules.AtlasRows, "mage and warlock VFX atlas rows");
            AssertEqual(16, MageWarlockSpellVfxRules.AtlasCellCount, "mage and warlock VFX atlas cell count");
            int[] authoredCells =
            {
                MageWarlockSpellVfxRules.FireCastRuneCell,
                MageWarlockSpellVfxRules.FireballProjectileCell,
                MageWarlockSpellVfxRules.FireballImpactCell,
                MageWarlockSpellVfxRules.MeteorCell,
                MageWarlockSpellVfxRules.FrostLanceCell,
                MageWarlockSpellVfxRules.FrostBurstCell,
                MageWarlockSpellVfxRules.LightningCastRuneCell,
                MageWarlockSpellVfxRules.TempestImpactCell,
                MageWarlockSpellVfxRules.HexRuneCell,
                MageWarlockSpellVfxRules.SoulProjectileCell,
                MageWarlockSpellVfxRules.PactGateCell,
                MageWarlockSpellVfxRules.AbyssalImpactCell,
                MageWarlockSpellVfxRules.LesserSummonCell,
                MageWarlockSpellVfxRules.GreaterSummonCell,
                MageWarlockSpellVfxRules.AscendanceCell,
                MageWarlockSpellVfxRules.DoomCircleCell
            };
            AssertEqual(
                string.Join(",", Enumerable.Range(0, 16)),
                string.Join(",", authoredCells),
                "mage and warlock VFX semantics occupy the strict row-major 4x4 contract");
            AssertEqual(16, authoredCells.Distinct().Count(), "every mage and warlock VFX cell has one stable semantic identity");
            AssertEqual(true, authoredCells.All(MageWarlockSpellVfxRules.IsAtlasCell), "every authored mage and warlock VFX cell is in range");
            AssertEqual(false, MageWarlockSpellVfxRules.IsAtlasCell(-1), "negative mage and warlock VFX cells are rejected");
            AssertEqual(false, MageWarlockSpellVfxRules.IsAtlasCell(16), "mage and warlock VFX cells beyond the 4x4 sheet are rejected");

            string[] formulaCodes =
            {
                "FBL", "MTR", "RCL", "AST", "RBT", "IBD",
                "IBG", "DFA", "DMC", "SLV", "PBR", "RLM"
            };
            MageWarlockSpellVfxProfile[] profiles = formulaCodes
                .Select(MageWarlockSpellVfxRules.ProfileFor)
                .ToArray();
            AssertEqual(
                "fireball|meteor|frost|tempest|riftbolt|lessersummon|greatersummon|ascendance|doomcircle|soulveil|pactbrand|hex",
                string.Join("|", profiles.Select(profile => profile.Key)),
                "canonical mage and warlock formulas retain distinct visual families");
            AssertEqual(12, profiles.Select(profile => profile.Key).Distinct().Count(), "canonical mage and warlock VFX families do not collapse into generic spell art");
            AssertEqual(
                "fireball:0/1/2/0|meteor:0/3/3/2|frost:4/4/5/4|tempest:6/6/7/6|riftbolt:10/9/11/10|lessersummon:10/-1/12/10|greatersummon:10/-1/13/10|ascendance:10/-1/14/10|doomcircle:8/-1/15/8|soulveil:8/9/10/8|pactbrand:10/9/15/8|hex:8/9/11/8",
                string.Join("|", profiles.Select(profile => $"{profile.Key}:{profile.CastCell}/{profile.ProjectileCell}/{profile.ImpactCell}/{profile.ImpactAccentCell}")),
                "canonical mage and warlock formulas map to their authored cast, travel, impact, and accent cells");

            MageWarlockSpellVfxArtPlan fireballCast = MageWarlockSpellVfxRules.CastPlan("FBL", 3, 0.50f);
            MageWarlockSpellVfxArtPlan fireballProjectile = MageWarlockSpellVfxRules.ProjectilePlan("FBL", 3, 0.50f);
            MageWarlockSpellVfxArtPlan fireballImpact = MageWarlockSpellVfxRules.ImpactPlan("FBL", 3, 0.50f);
            AssertEqual(MageWarlockSpellVfxRules.FireCastRuneCell, fireballCast.PrimaryCell, "Fireball opens on its canonical cast rune");
            AssertEqual(MageWarlockSpellVfxRules.FireballProjectileCell, fireballProjectile.PrimaryCell, "Fireball travels as its canonical projectile");
            AssertEqual(MageWarlockSpellVfxRules.FireballImpactCell, fireballImpact.PrimaryCell, "Fireball lands on its canonical impact art");
            AssertEqual(true, fireballProjectile.HasSecondary && fireballImpact.HasSecondary, "epic Fireball layers authored cast and impact accents");
            AssertEqual(false, MageWarlockSpellVfxRules.ProjectilePlan("IBG", 3, 0.50f).HasPrimary, "greater summons use a ritual arrival instead of a misleading projectile");

            MageWarlockSpellVfxArtPlan reducedFireball = MageWarlockSpellVfxRules.ImpactPlan("FBL", 3, 0.12f, true);
            MageWarlockSpellVfxArtPlan reducedFireballLate = MageWarlockSpellVfxRules.ImpactPlan("FBL", 3, 0.88f, true);
            AssertEqual(MageWarlockSpellVfxRules.FireballImpactCell, reducedFireball.PrimaryCell, "Reduced Motion preserves Fireball's semantic impact stamp");
            AssertEqual(false, reducedFireball.HasSecondary, "Reduced Motion removes layered Fireball animation");
            AssertEqual(true, reducedFireball.DurationSeconds <= 0.10f, "Reduced Motion collapses Fireball to a brief static beat");
            AssertEqual(reducedFireball.PrimaryScale, reducedFireballLate.PrimaryScale, "Reduced Motion Fireball scale is frame-stable");
            AssertEqual(reducedFireball.PrimaryOpacity, reducedFireballLate.PrimaryOpacity, "Reduced Motion Fireball opacity is frame-stable");
            AssertEqual(true, reducedFireball.BurstCount < fireballImpact.BurstCount, "Reduced Motion lowers Fireball burst density");

            foreach (MageWarlockSpellVfxProfile profile in profiles)
            {
                foreach (MageWarlockSpellVfxPhase phase in Enum.GetValues(typeof(MageWarlockSpellVfxPhase)))
                {
                    foreach (int intensity in new[] { 0, 1, 2, 3, 4 })
                    {
                        foreach (float progress in new[] { -0.5f, 0f, 0.5f, 1f, 1.5f })
                        {
                            MageWarlockSpellVfxArtPlan plan = phase == MageWarlockSpellVfxPhase.Cast
                                ? MageWarlockSpellVfxRules.CastPlan(profile.Key, intensity, progress)
                                : phase == MageWarlockSpellVfxPhase.Projectile
                                    ? MageWarlockSpellVfxRules.ProjectilePlan(profile.Key, intensity, progress)
                                    : MageWarlockSpellVfxRules.ImpactPlan(profile.Key, intensity, progress);
                            if (!plan.HasPrimary)
                            {
                                AssertEqual(false, plan.HasSecondary, profile.Key + " empty phase has no orphaned accent art");
                                AssertEqual(0f, plan.DurationSeconds, profile.Key + " empty phase has no animation duration");
                                AssertEqual(0, plan.BurstCount, profile.Key + " empty phase has no particle burst");
                                continue;
                            }

                            AssertEqual(true, MageWarlockSpellVfxRules.IsAtlasCell(plan.PrimaryCell), profile.Key + " primary VFX cell stays in range");
                            AssertEqual(true, plan.PrimaryScale >= 0.42f && plan.PrimaryScale <= 2.40f, profile.Key + " primary VFX scale stays bounded");
                            AssertEqual(true, plan.PrimaryOpacity >= 0f && plan.PrimaryOpacity <= 1f, profile.Key + " primary VFX opacity stays bounded");
                            AssertEqual(true, plan.DurationSeconds >= 0f && plan.DurationSeconds <= 1f, profile.Key + " VFX duration stays brief");
                            AssertEqual(true, plan.BurstCount >= 0 && plan.BurstCount <= 64, profile.Key + " VFX burst count stays bounded");
                            if (plan.HasSecondary)
                            {
                                AssertEqual(true, MageWarlockSpellVfxRules.IsAtlasCell(plan.SecondaryCell), profile.Key + " accent VFX cell stays in range");
                                AssertEqual(true, plan.SecondaryScale >= 0.50f && plan.SecondaryScale <= 2.55f, profile.Key + " accent VFX scale stays bounded");
                                AssertEqual(true, plan.SecondaryOpacity >= 0f && plan.SecondaryOpacity <= 1f, profile.Key + " accent VFX opacity stays bounded");
                            }
                        }
                    }
                }
            }

            int fireballHash = MageWarlockSpellVfxRules.StableVisualHash("FBL", 7, 2);
            AssertEqual(fireballHash, MageWarlockSpellVfxRules.StableVisualHash("fireball", 7, 2), "Fireball aliases share deterministic VFX sampling");
            AssertEqual(true, fireballHash != MageWarlockSpellVfxRules.StableVisualHash("FBL", 7, 3), "deterministic VFX channels remain decorrelated");
            for (int sampleIndex = 0; sampleIndex < 16; sampleIndex++)
            {
                float sample = MageWarlockSpellVfxRules.StableVisualSample("DFA", sampleIndex, 1);
                float signed = MageWarlockSpellVfxRules.StableVisualSignedSample("DFA", sampleIndex, 1);
                AssertEqual(sample, MageWarlockSpellVfxRules.StableVisualSample("DFA", sampleIndex, 1), "ascendance VFX samples repeat exactly");
                AssertEqual(true, sample >= 0f && sample < 1f, "deterministic VFX samples stay normalized");
                AssertEqual(true, signed >= -1f && signed < 1f, "deterministic signed VFX samples stay normalized");
            }

            string[] routedFormulaCodes =
            {
                "FBL", "MTR", "RCL", "RBI", "FRB", "RIG", "RSG", "CLT", "VST", "AST",
                "RLM", "INH", "DMC", "IBD", "IBF", "PBR", "IBG", "DFA", "RBT", "VRS", "SLV"
            };
            FormulaDef[] routedFormulas = routedFormulaCodes
                .Select(code => FormulaCatalog.All.First(formula => formula.Code == code))
                .ToArray();
            const string expectedVisualKinds = "fireball|meteor|coldlance|frostburst|frostbind|arcspark|thunderclap|chainlightning|thunderstep|tempest|deathburst|lifedrain|doomcircle|lessersummon|lessersummon|pactbrand|greatersummon|ascendance|riftbolt|riftstep|soulveil";
            AssertEqual(
                expectedVisualKinds,
                string.Join("|", routedFormulas.Select(formula => CombatPowerVisualRules.ImpactKindForFormula(formula, "spell"))),
                "mage and warlock formulas route to semantic impact identities");
            AssertEqual(
                expectedVisualKinds,
                string.Join("|", routedFormulas.Select(formula => CombatPowerVisualRules.CastKindForFormula(formula, "spell"))),
                "mage and warlock formulas carry the same semantic identity into cast anticipation");
        }

        private static void SupportHexSpellVfxProfilesStayDistinctAndMotionSafe()
        {
            AssertEqual(4, SupportHexSpellVfxRules.AtlasColumns, "support and hex VFX atlas columns");
            AssertEqual(4, SupportHexSpellVfxRules.AtlasRows, "support and hex VFX atlas rows");
            AssertEqual(16, SupportHexSpellVfxRules.AtlasCellCount, "support and hex VFX atlas cell count");
            int[] authoredCells =
            {
                SupportHexSpellVfxRules.HolyCastRuneCell,
                SupportHexSpellVfxRules.MendingWispCell,
                SupportHexSpellVfxRules.HealDawnBloomCell,
                SupportHexSpellVfxRules.WardDomeCell,
                SupportHexSpellVfxRules.SunLanceCell,
                SupportHexSpellVfxRules.SunBrandHoldImpactCell,
                SupportHexSpellVfxRules.CleanseRiftSealCell,
                SupportHexSpellVfxRules.NatureStoneCreationCell,
                SupportHexSpellVfxRules.WebSnapCell,
                SupportHexSpellVfxRules.PoisonCloudBurstCell,
                SupportHexSpellVfxRules.SleepDreamMistCell,
                SupportHexSpellVfxRules.NightVeilCell,
                SupportHexSpellVfxRules.GraveHookCell,
                SupportHexSpellVfxRules.DrainLifeCell,
                SupportHexSpellVfxRules.MindBreakWitherCell,
                SupportHexSpellVfxRules.AshenCurseCell
            };
            AssertEqual(
                string.Join(",", Enumerable.Range(0, 16)),
                string.Join(",", authoredCells),
                "support and hex VFX semantics occupy the strict row-major 4x4 contract");
            AssertEqual(16, authoredCells.Distinct().Count(), "every support and hex VFX cell has one stable semantic identity");
            AssertEqual(true, authoredCells.All(SupportHexSpellVfxRules.IsAtlasCell), "every authored support and hex VFX cell is in range");
            AssertEqual(false, SupportHexSpellVfxRules.IsAtlasCell(-1), "negative support and hex VFX cells are rejected");
            AssertEqual(false, SupportHexSpellVfxRules.IsAtlasCell(16), "support and hex VFX cells beyond the 4x4 sheet are rejected");

            string[] formulaCodes =
            {
                "OIC", "HLC", "OBL", "NVC", "GBH", "WBK", "WBP",
                "RMS", "NVL", "GRH", "INH", "RMB", "ACR"
            };
            string[] semanticAliases =
            {
                "mending wisp", "sanctuary-ward", "light bolt", "Rift Seal", "stone block", "bind", "poison gas",
                "dream smoke", "shadow veil", "grave hook", "life drain", "weaken", "Ashen Curse"
            };
            const string expectedFamilies = "heal|ward|sun|cleanse|nature|web|poison|sleep|nightveil|gravehook|drainlife|mindbreak|ashencurse";
            AssertEqual(
                expectedFamilies,
                string.Join("|", formulaCodes.Select(SupportHexSpellVfxRules.NormalizeKey)),
                "representative support and hex formula codes route to every authored VFX family");
            AssertEqual(
                expectedFamilies,
                string.Join("|", semanticAliases.Select(SupportHexSpellVfxRules.NormalizeKey)),
                "production semantic aliases route to every authored support and hex VFX family");
            AssertEqual(true, formulaCodes.All(SupportHexSpellVfxRules.IsSupported), "every representative support and hex formula has authored VFX");

            SupportHexSpellVfxProfile[] profiles = formulaCodes
                .Select(SupportHexSpellVfxRules.ProfileFor)
                .ToArray();
            AssertEqual(expectedFamilies, string.Join("|", profiles.Select(profile => profile.Key)), "support and hex profile keys remain exact");
            AssertEqual(13, profiles.Select(profile => profile.Key).Distinct().Count(), "support and hex spell families do not collapse into generic art");
            AssertEqual(
                "heal:0/1/2/0|ward:0/1/3/0|sun:0/4/5/0|cleanse:0/4/6/0|nature:0/-1/7/0|web:8/8/8/14|poison:9/9/9/-1|sleep:10/10/10/11|nightveil:11/11/11/10|gravehook:12/12/12/14|drainlife:13/13/13/11|mindbreak:14/14/14/10|ashencurse:15/15/15/14",
                string.Join("|", profiles.Select(profile => $"{profile.Key}:{profile.CastCell}/{profile.ProjectileCell}/{profile.ImpactCell}/{profile.ImpactAccentCell}")),
                "support and hex families map to exact authored cast, travel, impact, and accent cells");

            foreach (SupportHexSpellVfxProfile profile in profiles)
            {
                foreach (SupportHexSpellVfxPhase phase in Enum.GetValues(typeof(SupportHexSpellVfxPhase)))
                {
                    SupportHexSpellVfxArtPlan fullPlan = phase == SupportHexSpellVfxPhase.Cast
                        ? SupportHexSpellVfxRules.CastPlan(profile.Key, 3, 0.50f)
                        : phase == SupportHexSpellVfxPhase.Projectile
                            ? SupportHexSpellVfxRules.ProjectilePlan(profile.Key, 3, 0.50f)
                            : SupportHexSpellVfxRules.ImpactPlan(profile.Key, 3, 0.50f);
                    SupportHexSpellVfxArtPlan reducedEarly = phase == SupportHexSpellVfxPhase.Cast
                        ? SupportHexSpellVfxRules.CastPlan(profile.Key, 3, 0.12f, true)
                        : phase == SupportHexSpellVfxPhase.Projectile
                            ? SupportHexSpellVfxRules.ProjectilePlan(profile.Key, 3, 0.12f, true)
                            : SupportHexSpellVfxRules.ImpactPlan(profile.Key, 3, 0.12f, true);
                    SupportHexSpellVfxArtPlan reducedLate = phase == SupportHexSpellVfxPhase.Cast
                        ? SupportHexSpellVfxRules.CastPlan(profile.Key, 3, 0.88f, true)
                        : phase == SupportHexSpellVfxPhase.Projectile
                            ? SupportHexSpellVfxRules.ProjectilePlan(profile.Key, 3, 0.88f, true)
                            : SupportHexSpellVfxRules.ImpactPlan(profile.Key, 3, 0.88f, true);

                    if (!fullPlan.HasPrimary)
                    {
                        AssertEqual(SupportHexSpellVfxPhase.Projectile, phase, profile.Key + " only omits an authored projectile phase");
                        AssertEqual(false, profile.HasProjectile, profile.Key + " omitted projectile matches its profile contract");
                        AssertEqual(false, reducedEarly.HasPrimary || reducedEarly.HasSecondary, profile.Key + " reduced empty phase has no orphaned art");
                        AssertEqual(0f, reducedEarly.DurationSeconds, profile.Key + " reduced empty phase has no duration");
                        AssertEqual(0, reducedEarly.BurstCount, profile.Key + " reduced empty phase has no burst");
                        continue;
                    }

                    AssertEqual(true, SupportHexSpellVfxRules.IsAtlasCell(fullPlan.PrimaryCell), profile.Key + " primary support VFX cell stays in range");
                    AssertEqual(true, fullPlan.PrimaryScale >= 0.42f && fullPlan.PrimaryScale <= 2.45f, profile.Key + " primary support VFX scale stays bounded");
                    AssertEqual(true, fullPlan.PrimaryOpacity >= 0f && fullPlan.PrimaryOpacity <= 1f, profile.Key + " primary support VFX opacity stays bounded");
                    AssertEqual(true, fullPlan.DurationSeconds > 0f && fullPlan.DurationSeconds <= 1f, profile.Key + " support VFX duration stays brief");
                    AssertEqual(true, fullPlan.BurstCount > 0 && fullPlan.BurstCount <= 64, profile.Key + " support VFX burst count stays bounded");
                    if (fullPlan.HasSecondary)
                    {
                        AssertEqual(true, SupportHexSpellVfxRules.IsAtlasCell(fullPlan.SecondaryCell), profile.Key + " support accent VFX cell stays in range");
                        AssertEqual(true, fullPlan.SecondaryScale >= 0.50f && fullPlan.SecondaryScale <= 2.55f, profile.Key + " support accent VFX scale stays bounded");
                        AssertEqual(true, fullPlan.SecondaryOpacity >= 0f && fullPlan.SecondaryOpacity <= 1f, profile.Key + " support accent VFX opacity stays bounded");
                    }

                    AssertEqual(true, reducedEarly.HasPrimary, profile.Key + " Reduced Motion preserves semantic phase art");
                    AssertEqual(false, reducedEarly.HasSecondary, profile.Key + " Reduced Motion collapses support VFX to one art layer");
                    AssertEqual(true, reducedEarly.DurationSeconds <= 0.10f, profile.Key + " Reduced Motion support VFX stays a brief static beat");
                    AssertEqual(true, reducedEarly.PrimaryScale >= 0.42f && reducedEarly.PrimaryScale <= 2.45f, profile.Key + " Reduced Motion support scale stays bounded");
                    AssertEqual(true, reducedEarly.PrimaryOpacity >= 0f && reducedEarly.PrimaryOpacity <= 1f, profile.Key + " Reduced Motion support opacity stays bounded");
                    AssertEqual(true, reducedEarly.BurstCount > 0 && reducedEarly.BurstCount < fullPlan.BurstCount, profile.Key + " Reduced Motion lowers support burst density");
                    AssertEqual(reducedEarly.PrimaryCell, reducedLate.PrimaryCell, profile.Key + " Reduced Motion support art is frame-stable");
                    AssertEqual(reducedEarly.PrimaryScale, reducedLate.PrimaryScale, profile.Key + " Reduced Motion support scale is frame-stable");
                    AssertEqual(reducedEarly.PrimaryOpacity, reducedLate.PrimaryOpacity, profile.Key + " Reduced Motion support opacity is frame-stable");
                }
            }

            for (int familyIndex = 0; familyIndex < formulaCodes.Length; familyIndex++)
            {
                int hash = SupportHexSpellVfxRules.StableVisualHash(formulaCodes[familyIndex], 7, 2);
                AssertEqual(hash, SupportHexSpellVfxRules.StableVisualHash(semanticAliases[familyIndex], 7, 2), profiles[familyIndex].Key + " aliases share deterministic VFX sampling");
                for (int sampleIndex = 0; sampleIndex < 4; sampleIndex++)
                {
                    float sample = SupportHexSpellVfxRules.StableVisualSample(formulaCodes[familyIndex], sampleIndex, 1);
                    float signed = SupportHexSpellVfxRules.StableVisualSignedSample(formulaCodes[familyIndex], sampleIndex, 1);
                    AssertEqual(sample, SupportHexSpellVfxRules.StableVisualSample(formulaCodes[familyIndex], sampleIndex, 1), profiles[familyIndex].Key + " VFX samples repeat exactly");
                    AssertEqual(true, sample >= 0f && sample < 1f, profiles[familyIndex].Key + " deterministic VFX samples stay normalized");
                    AssertEqual(true, signed >= -1f && signed < 1f, profiles[familyIndex].Key + " deterministic signed VFX samples stay normalized");
                }
            }
        }

        private static void ClassSkillVfxProfilesStayDistinctAndMotionSafe()
        {
            AssertEqual(4, ClassSkillVfxRules.AtlasColumns, "class skill VFX atlas columns");
            AssertEqual(4, ClassSkillVfxRules.AtlasRows, "class skill VFX atlas rows");
            AssertEqual(16, ClassSkillVfxRules.AtlasCellCount, "class skill VFX atlas cell count");
            int[] authoredCells =
            {
                ClassSkillVfxRules.ChargeCell,
                ClassSkillVfxRules.ShieldBashCell,
                ClassSkillVfxRules.RallyCell,
                ClassSkillVfxRules.WhirlwindCell,
                ClassSkillVfxRules.ExecuteCell,
                ClassSkillVfxRules.SunderCell,
                ClassSkillVfxRules.StealthCell,
                ClassSkillVfxRules.AmbushCell,
                ClassSkillVfxRules.SmokeBombCell,
                ClassSkillVfxRules.ThrowKnifeCell,
                ClassSkillVfxRules.EviscerateHamstringCell,
                ClassSkillVfxRules.ShadowstepCell,
                ClassSkillVfxRules.RiftPounceCell,
                ClassSkillVfxRules.AbyssalWhirlCell,
                ClassSkillVfxRules.SoulRendCell,
                ClassSkillVfxRules.DreadRoarCell
            };
            AssertEqual(
                string.Join(",", Enumerable.Range(0, 16)),
                string.Join(",", authoredCells),
                "class skill VFX semantics occupy the strict row-major 4x4 contract");
            AssertEqual(16, authoredCells.Distinct().Count(), "every class skill VFX cell has one stable semantic identity");
            AssertEqual(true, authoredCells.All(ClassSkillVfxRules.IsAtlasCell), "every authored class skill VFX cell is in range");
            AssertEqual(false, ClassSkillVfxRules.IsAtlasCell(-1), "negative class skill VFX cells are rejected");
            AssertEqual(false, ClassSkillVfxRules.IsAtlasCell(16), "class skill VFX cells beyond the 4x4 sheet are rejected");
            AssertEqual(true, ClassSkillVfxRules.SupportsClass("warrior"), "warrior skills use the class VFX atlas");
            AssertEqual(true, ClassSkillVfxRules.SupportsClass("rogue"), "rogue skills use the class VFX atlas");
            AssertEqual(true, ClassSkillVfxRules.SupportsClass("demon"), "demon skills use the class VFX atlas");

            string[] abilityIds =
            {
                "charge", "shieldbash", "rally", "whirlwind", "execute", "sunder", "stealth", "ambush",
                "smokebomb", "throwknife", "eviscerate", "shadowstep", "riftpounce", "abyssalwhirl", "soulrend", "dreadroar"
            };
            string[] semanticAliases =
            {
                "charge impact", "shield slam", "battle cry", "cleave", "execution", "guard break", "vanish", "backstab",
                "smoke cloud", "thrown knife", "hamstring", "shadow strike", "demon pounce", "abyssal whirlwind", "life rip", "demon roar"
            };
            const string expectedFamilies = "charge|shieldbash|rally|whirlwind|execute|sunder|stealth|ambush|smokebomb|throwknife|eviscerate|shadowstep|riftpounce|abyssalwhirl|soulrend|dreadroar";
            AssertEqual(expectedFamilies, string.Join("|", abilityIds.Select(ClassSkillVfxRules.NormalizeKey)), "class skill IDs route to every authored VFX family");
            AssertEqual(expectedFamilies, string.Join("|", semanticAliases.Select(ClassSkillVfxRules.NormalizeKey)), "production semantic aliases route to every authored class skill VFX family");
            AssertEqual(true, abilityIds.All(ClassSkillVfxRules.IsSupported), "every warrior, rogue, and demon skill family has authored VFX");

            ClassSkillVfxProfile[] profiles = abilityIds
                .Select(ClassSkillVfxRules.ProfileFor)
                .ToArray();
            AssertEqual(expectedFamilies, string.Join("|", profiles.Select(profile => profile.Key)), "class skill profile keys remain exact");
            AssertEqual(16, profiles.Select(profile => profile.Key).Distinct().Count(), "class skill families do not collapse into generic art");
            AssertEqual(true, profiles.All(profile => profile.Supported), "every class skill profile has authored cast and impact art");
            AssertEqual(
                "charge:0/0/0|shieldbash:1/-1/1|rally:2/-1/2|whirlwind:3/-1/3|execute:4/-1/4|sunder:5/-1/5|stealth:6/-1/6|ambush:7/-1/7|smokebomb:8/-1/8|throwknife:9/9/9|eviscerate:10/-1/10|shadowstep:11/11/11|riftpounce:12/12/12|abyssalwhirl:13/-1/13|soulrend:14/14/14|dreadroar:15/-1/15",
                string.Join("|", profiles.Select(profile => $"{profile.Key}:{profile.CastCell}/{profile.TravelCell}/{profile.ImpactCell}")),
                "class skill families map to exact authored cast, travel, and impact cells");

            foreach (ClassSkillVfxProfile profile in profiles)
            {
                foreach (ClassSkillVfxPhase phase in Enum.GetValues(typeof(ClassSkillVfxPhase)))
                {
                    ClassSkillVfxArtPlan fullPlan = phase == ClassSkillVfxPhase.Cast
                        ? ClassSkillVfxRules.CastPlan(profile.Key, 3, 0.50f)
                        : phase == ClassSkillVfxPhase.Travel
                            ? ClassSkillVfxRules.TravelPlan(profile.Key, 3, 0.50f)
                            : ClassSkillVfxRules.ImpactPlan(profile.Key, 3, 0.50f);
                    ClassSkillVfxArtPlan reducedEarly = phase == ClassSkillVfxPhase.Cast
                        ? ClassSkillVfxRules.CastPlan(profile.Key, 3, 0.12f, true)
                        : phase == ClassSkillVfxPhase.Travel
                            ? ClassSkillVfxRules.TravelPlan(profile.Key, 3, 0.12f, true)
                            : ClassSkillVfxRules.ImpactPlan(profile.Key, 3, 0.12f, true);
                    ClassSkillVfxArtPlan reducedLate = phase == ClassSkillVfxPhase.Cast
                        ? ClassSkillVfxRules.CastPlan(profile.Key, 3, 0.88f, true)
                        : phase == ClassSkillVfxPhase.Travel
                            ? ClassSkillVfxRules.TravelPlan(profile.Key, 3, 0.88f, true)
                            : ClassSkillVfxRules.ImpactPlan(profile.Key, 3, 0.88f, true);

                    if (!fullPlan.HasPrimary)
                    {
                        AssertEqual(ClassSkillVfxPhase.Travel, phase, profile.Key + " only omits an authored travel phase");
                        AssertEqual(false, profile.HasTravel, profile.Key + " omitted travel matches its profile contract");
                        AssertEqual(false, reducedEarly.HasPrimary || reducedEarly.HasSecondary, profile.Key + " reduced empty phase has no orphaned art");
                        AssertEqual(0f, reducedEarly.DurationSeconds, profile.Key + " reduced empty phase has no duration");
                        AssertEqual(0, reducedEarly.BurstCount, profile.Key + " reduced empty phase has no burst");
                        continue;
                    }

                    AssertEqual(true, ClassSkillVfxRules.IsAtlasCell(fullPlan.PrimaryCell), profile.Key + " primary class skill VFX cell stays in range");
                    AssertEqual(true, fullPlan.PrimaryScale >= 0.42f && fullPlan.PrimaryScale <= 2.40f, profile.Key + " primary class skill VFX scale stays bounded");
                    AssertEqual(true, fullPlan.PrimaryOpacity >= 0f && fullPlan.PrimaryOpacity <= 1f, profile.Key + " primary class skill VFX opacity stays bounded");
                    AssertEqual(true, fullPlan.DurationSeconds > 0f && fullPlan.DurationSeconds <= 1f, profile.Key + " class skill VFX duration stays brief");
                    AssertEqual(true, fullPlan.BurstCount > 0 && fullPlan.BurstCount <= 64, profile.Key + " class skill VFX burst count stays bounded");
                    if (fullPlan.HasSecondary)
                    {
                        AssertEqual(true, ClassSkillVfxRules.IsAtlasCell(fullPlan.SecondaryCell), profile.Key + " class skill accent VFX cell stays in range");
                        AssertEqual(true, fullPlan.SecondaryScale >= 0.50f && fullPlan.SecondaryScale <= 2.55f, profile.Key + " class skill accent VFX scale stays bounded");
                        AssertEqual(true, fullPlan.SecondaryOpacity >= 0f && fullPlan.SecondaryOpacity <= 1f, profile.Key + " class skill accent VFX opacity stays bounded");
                    }

                    AssertEqual(true, reducedEarly.HasPrimary, profile.Key + " Reduced Motion preserves semantic phase art");
                    AssertEqual(false, reducedEarly.HasSecondary, profile.Key + " Reduced Motion collapses class skill VFX to one art layer");
                    AssertEqual(true, reducedEarly.DurationSeconds <= 0.10f, profile.Key + " Reduced Motion class skill VFX stays a brief static beat");
                    AssertEqual(true, reducedEarly.PrimaryScale >= 0.42f && reducedEarly.PrimaryScale <= 2.40f, profile.Key + " Reduced Motion class skill scale stays bounded");
                    AssertEqual(true, reducedEarly.PrimaryOpacity >= 0f && reducedEarly.PrimaryOpacity <= 1f, profile.Key + " Reduced Motion class skill opacity stays bounded");
                    AssertEqual(true, reducedEarly.BurstCount > 0 && reducedEarly.BurstCount < fullPlan.BurstCount, profile.Key + " Reduced Motion lowers class skill burst density");
                    AssertEqual(reducedEarly.PrimaryCell, reducedLate.PrimaryCell, profile.Key + " Reduced Motion class skill art is frame-stable");
                    AssertEqual(reducedEarly.PrimaryScale, reducedLate.PrimaryScale, profile.Key + " Reduced Motion class skill scale is frame-stable");
                    AssertEqual(reducedEarly.PrimaryOpacity, reducedLate.PrimaryOpacity, profile.Key + " Reduced Motion class skill opacity is frame-stable");
                }
            }

            for (int familyIndex = 0; familyIndex < abilityIds.Length; familyIndex++)
            {
                int hash = ClassSkillVfxRules.StableVisualHash(abilityIds[familyIndex], 7, 2);
                AssertEqual(hash, ClassSkillVfxRules.StableVisualHash(semanticAliases[familyIndex], 7, 2), profiles[familyIndex].Key + " aliases share deterministic VFX sampling");
                for (int sampleIndex = 0; sampleIndex < 4; sampleIndex++)
                {
                    float sample = ClassSkillVfxRules.StableVisualSample(abilityIds[familyIndex], sampleIndex, 1);
                    float signed = ClassSkillVfxRules.StableVisualSignedSample(abilityIds[familyIndex], sampleIndex, 1);
                    AssertEqual(sample, ClassSkillVfxRules.StableVisualSample(abilityIds[familyIndex], sampleIndex, 1), profiles[familyIndex].Key + " VFX samples repeat exactly");
                    AssertEqual(true, sample >= 0f && sample < 1f, profiles[familyIndex].Key + " deterministic VFX samples stay normalized");
                    AssertEqual(true, signed >= -1f && signed < 1f, profiles[familyIndex].Key + " deterministic signed VFX samples stay normalized");
                }
            }

            AssertEqual(false, ClassSkillVfxRules.SupportsClass("ranger"), "Ranger keeps its dedicated directional skill atlas");
            string[] rangerAbilityIds = { "aimedshot", "pinningshot", "scoutmark", "volley", "broadheadshot", "disruptingshot", "quickshot" };
            foreach (string rangerAbilityId in rangerAbilityIds)
            {
                AssertEqual(rangerAbilityId, ClassSkillVfxRules.NormalizeKey(rangerAbilityId), rangerAbilityId + " remains an exact Ranger atlas identity");
                AssertEqual(false, ClassSkillVfxRules.IsSupported(rangerAbilityId), rangerAbilityId + " does not borrow class skill VFX art");
                ClassSkillVfxProfile rangerProfile = ClassSkillVfxRules.ProfileFor(rangerAbilityId);
                AssertEqual(false, rangerProfile.Supported, rangerAbilityId + " returns an unsupported class skill profile");
                AssertEqual("skill", rangerProfile.Key, rangerAbilityId + " falls back without changing its Ranger identity");
                ClassSkillVfxArtPlan rangerImpact = ClassSkillVfxRules.ImpactPlan(rangerAbilityId, 3, 0.50f);
                AssertEqual(false, rangerImpact.HasPrimary || rangerImpact.HasSecondary, rangerAbilityId + " class skill impact plan remains empty");
                AssertEqual(0f, rangerImpact.DurationSeconds, rangerAbilityId + " empty class skill impact has no duration");
                AssertEqual(0, rangerImpact.BurstCount, rangerAbilityId + " empty class skill impact has no burst");
            }
        }

        private static void CombatPowerTravelVfxProfilesStayDistinctAndMotionSafe()
        {
            AssertEqual(4, CombatPowerTravelVfxRules.AtlasColumns, "combat power travel VFX atlas columns");
            AssertEqual(4, CombatPowerTravelVfxRules.AtlasRows, "combat power travel VFX atlas rows");
            AssertEqual(16, CombatPowerTravelVfxRules.AtlasCellCount, "combat power travel VFX atlas cell count");
            int[] authoredCells =
            {
                CombatPowerTravelVfxRules.FireballCometCell,
                CombatPowerTravelVfxRules.MeteorVerticalStreakCell,
                CombatPowerTravelVfxRules.FrostLanceCell,
                CombatPowerTravelVfxRules.LightningLeaderCell,
                CombatPowerTravelVfxRules.MendingWispCell,
                CombatPowerTravelVfxRules.SunLanceCell,
                CombatPowerTravelVfxRules.RiftBoltCell,
                CombatPowerTravelVfxRules.SoulDrainTetherCell,
                CombatPowerTravelVfxRules.WebBolaNetCell,
                CombatPowerTravelVfxRules.PoisonVialVaporCell,
                CombatPowerTravelVfxRules.SleepCrescentMistCell,
                CombatPowerTravelVfxRules.GraveHookChainCell,
                CombatPowerTravelVfxRules.ChargeDashCell,
                CombatPowerTravelVfxRules.ThrownKnifeCell,
                CombatPowerTravelVfxRules.ShadowstepTeleportTrailCell,
                CombatPowerTravelVfxRules.RangerArrowsVolleyCell
            };
            AssertEqual(
                string.Join(",", Enumerable.Range(0, CombatPowerTravelVfxRules.AtlasCellCount)),
                string.Join(",", authoredCells),
                "combat power travel VFX cells preserve the strict row-major 4x4 contract");
            AssertEqual(true, authoredCells.All(CombatPowerTravelVfxRules.IsAtlasCell), "every authored combat power travel VFX cell is in range");
            AssertEqual(false, CombatPowerTravelVfxRules.IsAtlasCell(-1), "negative combat power travel VFX cells are rejected");
            AssertEqual(false, CombatPowerTravelVfxRules.IsAtlasCell(CombatPowerTravelVfxRules.AtlasCellCount), "combat power travel VFX cells beyond the 4x4 sheet are rejected");

            string[] semanticPowerKeys =
            {
                "FBL", "MTR", "RCL", "RIG",
                "OIC", "OBL", "RBT", "INH",
                "RKW", "RPX", "RMS", "GRH",
                "charge", "throwknife", "shadowstep", "volley"
            };
            CombatPowerTravelPath[] semanticPaths =
            {
                CombatPowerTravelPath.Arc,
                CombatPowerTravelPath.Rain,
                CombatPowerTravelPath.Straight,
                CombatPowerTravelPath.Straight,
                CombatPowerTravelPath.Straight,
                CombatPowerTravelPath.Straight,
                CombatPowerTravelPath.Straight,
                CombatPowerTravelPath.Tether,
                CombatPowerTravelPath.Arc,
                CombatPowerTravelPath.Arc,
                CombatPowerTravelPath.Arc,
                CombatPowerTravelPath.Chain,
                CombatPowerTravelPath.Dash,
                CombatPowerTravelPath.Straight,
                CombatPowerTravelPath.Teleport,
                CombatPowerTravelPath.Rain
            };
            CombatPowerTravelVfxProfile[] semanticProfiles = semanticPowerKeys
                .Select(CombatPowerTravelVfxRules.ProfileFor)
                .ToArray();
            AssertEqual(
                string.Join(",", authoredCells),
                string.Join(",", semanticProfiles.Select(profile => profile.AtlasCell)),
                "every combat power travel atlas cell has one canonical semantic probe");
            AssertEqual(
                string.Join("|", semanticPaths.Select(path => path.ToString())),
                string.Join("|", semanticProfiles.Select(profile => profile.Path.ToString())),
                "canonical combat power travel probes preserve projectile geometry");
            AssertEqual(16, semanticProfiles.Select(profile => profile.AtlasCell).Distinct().Count(), "canonical travel probes cover all sixteen authored cells exactly once");
            AssertEqual(true, semanticProfiles.All(profile => profile.HasTravel), "canonical travel probes all produce visible travel");

            Texture2D travelAtlas = null;
            try
            {
                travelAtlas = LoadApprovedRuntimeAtlas(RuntimeArtManifest.CombatPowerTravelVfxAtlas);
                AssertEqual(new Vector2Int(1280, 1280), new Vector2Int(travelAtlas.width, travelAtlas.height), "approved combat power travel VFX atlas dimensions");
                AssertEqual(320, travelAtlas.width / CombatPowerTravelVfxRules.AtlasColumns, "approved combat power travel VFX atlas cell width");
                AssertEqual(320, travelAtlas.height / CombatPowerTravelVfxRules.AtlasRows, "approved combat power travel VFX atlas cell height");
                AssertAtlasCellCoverage(
                    travelAtlas,
                    CombatPowerTravelVfxRules.AtlasColumns,
                    CombatPowerTravelVfxRules.AtlasRows,
                    authoredCells,
                    0.08f,
                    0.36f,
                    "approved combat power travel VFX");
                AssertAtlasCellSafeGutter(
                    travelAtlas,
                    CombatPowerTravelVfxRules.AtlasColumns,
                    CombatPowerTravelVfxRules.AtlasRows,
                    authoredCells,
                    24,
                    12,
                    0,
                    "approved combat power travel VFX");
                AssertAtlasHasNoBrightMagentaKeyField(travelAtlas, 12, 32, "approved combat power travel VFX");
            }
            finally
            {
                if (travelAtlas != null) UnityEngine.Object.DestroyImmediate(travelAtlas);
            }

            HashSet<string> noTravelFormulaCodes = new HashSet<string>(StringComparer.Ordinal)
            {
                "GBH", "GBX", "HLC", "SRF",
                "WBF", "BTF", "WBI", "RSG",
                "WBK", "WBP", "DMC",
                "IBD", "IBF", "IBG", "DFA"
            };
            AssertEqual(56, FormulaCatalog.All.Length, "travel VFX test catalog covers all formulas");
            AssertEqual(15, noTravelFormulaCodes.Count, "intentional impact-only formula travel count");
            AssertEqual(true, noTravelFormulaCodes.All(code => FormulaCatalog.All.Any(formula => formula.Code == code)), "every impact-only formula belongs to FormulaCatalog");
            AssertEqual(41, FormulaCatalog.All.Count(formula => !noTravelFormulaCodes.Contains(formula.Code)), "authored formula travel profile count");

            foreach (FormulaDef formula in FormulaCatalog.All)
            {
                bool expectsTravel = !noTravelFormulaCodes.Contains(formula.Code);
                AssertEqual(true, CombatPowerTravelVfxRules.IsKnownFormula(formula.Code), formula.Code + " is a known travel VFX formula code");
                AssertEqual(true, CombatPowerTravelVfxRules.IsKnownFormula(formula.Name), formula.Code + " full name is a known travel VFX formula alias");
                AssertEqual(expectsTravel, CombatPowerTravelVfxRules.IsSupportedFormula(formula.Code), formula.Code + " formula travel support is intentional");
                AssertEqual(expectsTravel, CombatPowerTravelVfxRules.IsSupported(formula.Name), formula.Code + " generic travel lookup preserves formula support");

                CombatPowerTravelVfxProfile profile = CombatPowerTravelVfxRules.ProfileForFormula(formula.Code);
                CombatPowerTravelVfxProfile namedProfile = CombatPowerTravelVfxRules.ProfileForFormula(formula.Name);
                AssertEqual(formula.Code, profile.Key, formula.Code + " formula travel profile keeps its canonical key");
                AssertEqual(profile.Key, namedProfile.Key, formula.Code + " full-name travel profile keeps its canonical key");
                AssertEqual(profile.AtlasCell, namedProfile.AtlasCell, formula.Code + " full-name travel profile keeps its atlas cell");
                AssertEqual(profile.Path, namedProfile.Path, formula.Code + " full-name travel profile keeps its path");
                AssertEqual(expectsTravel, profile.Supported, formula.Code + " formula travel profile support matches its delivery semantics");
                AssertEqual(expectsTravel, profile.HasTravel, formula.Code + " formula travel profile visibility matches its delivery semantics");

                CombatPowerTravelVfxPlan plan = CombatPowerTravelVfxRules.PlanForFormula(formula.Code, 3, 0.45f, false, 7);
                CombatPowerTravelVfxPlan repeated = CombatPowerTravelVfxRules.PlanForFormula(formula.Name, 3, 0.45f, false, 7);
                CombatPowerTravelVfxPlan generic = CombatPowerTravelVfxRules.PlanFor(formula.Code, 3, 0.45f, false, 7);
                AssertCombatPowerTravelPlansEquivalentAndBounded(plan, repeated, formula.Code + " formula travel aliases");
                AssertCombatPowerTravelPlansEquivalentAndBounded(plan, generic, formula.Code + " generic formula travel lookup");
                AssertEqual(expectsTravel, plan.HasTravel, formula.Code + " formula travel plan support matches its delivery semantics");

                if (expectsTravel)
                {
                    AssertEqual(true, CombatPowerTravelVfxRules.IsAtlasCell(profile.AtlasCell), formula.Code + " formula travel cell stays in range");
                    AssertEqual(true, profile.Path != CombatPowerTravelPath.None, formula.Code + " formula travel has a meaningful path");
                    AssertEqual(true, profile.BaseScale >= CombatPowerTravelVfxRules.MinimumScale && profile.BaseScale <= CombatPowerTravelVfxRules.MaximumScale, formula.Code + " formula travel profile scale stays bounded");
                    AssertEqual(true, profile.BaseOpacity >= CombatPowerTravelVfxRules.MinimumOpacity && profile.BaseOpacity <= CombatPowerTravelVfxRules.MaximumOpacity, formula.Code + " formula travel profile opacity stays bounded");
                    AssertEqual(true, profile.DurationSeconds >= CombatPowerTravelVfxRules.MinimumDurationSeconds && profile.DurationSeconds <= CombatPowerTravelVfxRules.MaximumDurationSeconds, formula.Code + " formula travel duration stays bounded");
                    AssertEqual(true, profile.TrailSampleCount >= CombatPowerTravelVfxRules.MinimumTrailSamples && profile.TrailSampleCount <= CombatPowerTravelVfxRules.MaximumTrailSamples, formula.Code + " formula trail sample count stays bounded");
                    AssertEqual(profile.AtlasCell, plan.AtlasCell, formula.Code + " formula travel plan preserves its atlas cell");
                    AssertEqual(profile.Path, plan.Path, formula.Code + " formula travel plan preserves its path");
                    AssertEqual(profile.DurationSeconds, plan.DurationSeconds, formula.Code + " formula travel plan preserves its authored duration");
                    AssertEqual(0.45f, plan.Progress, formula.Code + " formula travel plan preserves normalized progress");
                }
                else
                {
                    AssertEqual(-1, profile.AtlasCell, formula.Code + " impact-only formula has no travel atlas cell");
                    AssertEqual(CombatPowerTravelPath.None, profile.Path, formula.Code + " impact-only formula has no travel path");
                    AssertEqual(0f, profile.BaseScale, formula.Code + " impact-only formula has no travel scale");
                    AssertEqual(0f, profile.BaseOpacity, formula.Code + " impact-only formula has no travel opacity");
                    AssertEqual(0f, profile.DurationSeconds, formula.Code + " impact-only formula has no travel duration");
                    AssertEqual(0, profile.TrailSampleCount, formula.Code + " impact-only formula has no trail samples");
                }

                CombatPowerTravelVfxPlan reduced = CombatPowerTravelVfxRules.PlanForFormula(formula.Code, 3, 0.45f, true, 7);
                AssertEqual(false, reduced.Supported || reduced.HasTravel, formula.Code + " Reduced Motion formula plan suppresses travel");
                AssertEqual(-1, reduced.AtlasCell, formula.Code + " Reduced Motion formula plan has no atlas cell");
                AssertEqual(CombatPowerTravelPath.None, reduced.Path, formula.Code + " Reduced Motion formula plan has no path");
                AssertEqual(0f, reduced.DurationSeconds, formula.Code + " Reduced Motion formula plan has no duration");
                AssertEqual(0, reduced.TrailSampleCount, formula.Code + " Reduced Motion formula plan has no trail samples");

                int hash = CombatPowerTravelVfxRules.StableTravelHash(formula.Code, 7, 2);
                AssertEqual(hash, CombatPowerTravelVfxRules.StableTravelHash(formula.Name, 7, 2), formula.Code + " travel hash is deterministic across formula aliases");
                float sample = CombatPowerTravelVfxRules.StableTravelSample(formula.Code, 7, 2);
                float signed = CombatPowerTravelVfxRules.StableTravelSignedSample(formula.Code, 7, 2);
                AssertEqual(sample, CombatPowerTravelVfxRules.StableTravelSample(formula.Code, 7, 2), formula.Code + " travel samples repeat exactly");
                AssertEqual(true, sample >= 0f && sample < 1f, formula.Code + " travel samples stay normalized");
                AssertEqual(true, signed >= -1f && signed < 1f, formula.Code + " signed travel samples stay normalized");
            }

            string[] abilityIds = new[] { "warrior", "rogue", "ranger", "demon" }
                .SelectMany(AbilityCatalog.IdsForClass)
                .ToArray();
            HashSet<string> noTravelAbilityIds = new HashSet<string>(StringComparer.Ordinal)
            {
                "execute", "shieldbash", "rally", "cleave", "whirlwind", "sunder",
                "stealth", "ambush", "smokebomb", "hamstring", "eviscerate",
                "abyssalwhirl", "dreadroar"
            };
            AssertEqual(25, abilityIds.Length, "travel VFX test catalog covers all abilities");
            AssertEqual(25, abilityIds.Distinct(StringComparer.Ordinal).Count(), "travel VFX ability catalog is unique");
            AssertEqual(13, noTravelAbilityIds.Count, "intentional impact-only ability travel count");
            AssertEqual(true, noTravelAbilityIds.All(id => abilityIds.Contains(id)), "every impact-only ability belongs to AbilityCatalog");
            AssertEqual(12, abilityIds.Count(id => !noTravelAbilityIds.Contains(id)), "authored ability travel profile count");

            foreach (string abilityId in abilityIds)
            {
                MartialAbility ability = AbilityCatalog.For(abilityId);
                bool expectsTravel = !noTravelAbilityIds.Contains(abilityId);
                AssertEqual(true, ability != null, abilityId + " resolves through AbilityCatalog for travel VFX");
                AssertEqual(true, CombatPowerTravelVfxRules.IsKnownAbility(abilityId), abilityId + " is a known travel VFX ability ID");
                AssertEqual(true, CombatPowerTravelVfxRules.IsKnownAbility(ability.Name), abilityId + " full name is a known travel VFX ability alias");
                AssertEqual(expectsTravel, CombatPowerTravelVfxRules.IsSupportedAbility(abilityId), abilityId + " ability travel support is intentional");
                AssertEqual(expectsTravel, CombatPowerTravelVfxRules.IsSupported(ability.Name), abilityId + " generic travel lookup preserves ability support");

                CombatPowerTravelVfxProfile profile = CombatPowerTravelVfxRules.ProfileForAbility(abilityId);
                CombatPowerTravelVfxProfile namedProfile = CombatPowerTravelVfxRules.ProfileForAbility(ability.Name);
                AssertEqual(abilityId, profile.Key, abilityId + " ability travel profile keeps its canonical key");
                AssertEqual(profile.Key, namedProfile.Key, abilityId + " full-name ability travel profile keeps its canonical key");
                AssertEqual(profile.AtlasCell, namedProfile.AtlasCell, abilityId + " full-name ability travel profile keeps its atlas cell");
                AssertEqual(profile.Path, namedProfile.Path, abilityId + " full-name ability travel profile keeps its path");
                AssertEqual(expectsTravel, profile.Supported, abilityId + " ability travel profile support matches its delivery semantics");
                AssertEqual(expectsTravel, profile.HasTravel, abilityId + " ability travel profile visibility matches its delivery semantics");

                CombatPowerTravelVfxPlan plan = CombatPowerTravelVfxRules.PlanForAbility(abilityId, 3, 0.55f, false, 11);
                CombatPowerTravelVfxPlan repeated = CombatPowerTravelVfxRules.PlanForAbility(ability.Name, 3, 0.55f, false, 11);
                CombatPowerTravelVfxPlan generic = CombatPowerTravelVfxRules.PlanFor(abilityId, 3, 0.55f, false, 11);
                AssertCombatPowerTravelPlansEquivalentAndBounded(plan, repeated, abilityId + " ability travel aliases");
                AssertCombatPowerTravelPlansEquivalentAndBounded(plan, generic, abilityId + " generic ability travel lookup");
                AssertEqual(expectsTravel, plan.HasTravel, abilityId + " ability travel plan support matches its delivery semantics");

                if (expectsTravel)
                {
                    AssertEqual(true, CombatPowerTravelVfxRules.IsAtlasCell(profile.AtlasCell), abilityId + " ability travel cell stays in range");
                    AssertEqual(true, profile.Path != CombatPowerTravelPath.None, abilityId + " ability travel has a meaningful path");
                    AssertEqual(true, profile.BaseScale >= CombatPowerTravelVfxRules.MinimumScale && profile.BaseScale <= CombatPowerTravelVfxRules.MaximumScale, abilityId + " ability travel profile scale stays bounded");
                    AssertEqual(true, profile.BaseOpacity >= CombatPowerTravelVfxRules.MinimumOpacity && profile.BaseOpacity <= CombatPowerTravelVfxRules.MaximumOpacity, abilityId + " ability travel profile opacity stays bounded");
                    AssertEqual(true, profile.DurationSeconds >= CombatPowerTravelVfxRules.MinimumDurationSeconds && profile.DurationSeconds <= CombatPowerTravelVfxRules.MaximumDurationSeconds, abilityId + " ability travel duration stays bounded");
                    AssertEqual(true, profile.TrailSampleCount >= CombatPowerTravelVfxRules.MinimumTrailSamples && profile.TrailSampleCount <= CombatPowerTravelVfxRules.MaximumTrailSamples, abilityId + " ability trail sample count stays bounded");
                }
                else
                {
                    AssertEqual(-1, profile.AtlasCell, abilityId + " impact-only ability has no travel atlas cell");
                    AssertEqual(CombatPowerTravelPath.None, profile.Path, abilityId + " impact-only ability has no travel path");
                    AssertEqual(0f, profile.DurationSeconds, abilityId + " impact-only ability has no travel duration");
                    AssertEqual(0, profile.TrailSampleCount, abilityId + " impact-only ability has no trail samples");
                }

                CombatPowerTravelVfxPlan reduced = CombatPowerTravelVfxRules.PlanForAbility(abilityId, 3, 0.55f, true, 11);
                AssertEqual(false, reduced.Supported || reduced.HasTravel, abilityId + " Reduced Motion ability plan suppresses travel");
                AssertEqual(-1, reduced.AtlasCell, abilityId + " Reduced Motion ability plan has no atlas cell");
                AssertEqual(0f, reduced.DurationSeconds, abilityId + " Reduced Motion ability plan has no duration");
                AssertEqual(0, reduced.TrailSampleCount, abilityId + " Reduced Motion ability plan has no trail samples");

                int hash = CombatPowerTravelVfxRules.StableTravelHash(abilityId, 11, 3);
                AssertEqual(hash, CombatPowerTravelVfxRules.StableTravelHash(ability.Name, 11, 3), abilityId + " travel hash is deterministic across ability aliases");
                float sample = CombatPowerTravelVfxRules.StableTravelSample(abilityId, 11, 3);
                AssertEqual(true, sample >= 0f && sample < 1f, abilityId + " travel samples stay normalized");
            }

            CombatPowerTravelVfxPlan fireball = CombatPowerTravelVfxRules.PlanForFormula("FBL", 3, 0.75f, false, 4);
            AssertEqual(0f, CombatPowerTravelVfxRules.TravelProgress(-0.5f, 1f), "negative travel time clamps to the beginning");
            AssertEqual(0.5f, CombatPowerTravelVfxRules.TravelProgress(0.5f, 1f), "travel time preserves mid-flight progress");
            AssertEqual(1f, CombatPowerTravelVfxRules.TravelProgress(2f, 1f), "travel time clamps after arrival");
            AssertEqual(1f, CombatPowerTravelVfxRules.TravelProgress(0f, 0f), "zero-duration travel resolves immediately");
            AssertEqual(0.75f, CombatPowerTravelVfxRules.TrailSampleProgress(fireball, 0), "Fireball trail head matches current travel progress");
            AssertEqual(0.75f, CombatPowerTravelVfxRules.TrailSampleProgress(fireball, -1), "negative trail indices clamp to the head");
            float previousTrailProgress = 1f;
            for (int trailIndex = 0; trailIndex < fireball.TrailSampleCount; trailIndex++)
            {
                float trailProgress = CombatPowerTravelVfxRules.TrailSampleProgress(fireball, trailIndex);
                AssertEqual(true, trailProgress >= 0f && trailProgress <= fireball.Progress, "Fireball trail sample " + trailIndex + " stays behind its head");
                AssertEqual(true, trailProgress <= previousTrailProgress, "Fireball trail samples remain ordered from head to tail");
                previousTrailProgress = trailProgress;
            }
            AssertEqual(
                previousTrailProgress,
                CombatPowerTravelVfxRules.TrailSampleProgress(fireball, fireball.TrailSampleCount + 20),
                "trail indices past the tail clamp deterministically");

            AssertEqual(true, CombatPowerTravelVfxRules.IsKnownFormula("Fireball projectile"), "decorated Fireball projectile aliases remain known");
            AssertEqual(CombatPowerTravelVfxRules.FireballCometCell, CombatPowerTravelVfxRules.ProfileFor("Fireball projectile").AtlasCell, "decorated Fireball aliases keep their comet art");
            AssertEqual(false, CombatPowerTravelVfxRules.IsKnownFormula("unknown formula"), "unknown formula travel identities are rejected");
            AssertEqual(false, CombatPowerTravelVfxRules.IsKnownAbility("unknown ability"), "unknown ability travel identities are rejected");
            AssertEqual(false, CombatPowerTravelVfxRules.IsSupported("unknown power"), "unknown power travel identities remain unsupported");
            CombatPowerTravelVfxPlan unknown = CombatPowerTravelVfxRules.PlanFor("unknown power", 99, 2f, false, 99);
            AssertEqual(false, unknown.Supported || unknown.HasTravel, "unknown power travel plan remains empty");
            AssertEqual(-1, unknown.AtlasCell, "unknown power travel plan has no atlas cell");
            AssertEqual(CombatPowerTravelPath.None, unknown.Path, "unknown power travel plan has no path");

            CombatPowerTravelVfxPlan clampedLow = CombatPowerTravelVfxRules.PlanForFormula("FBL", 0, -1f, false, 2);
            CombatPowerTravelVfxPlan clampedHigh = CombatPowerTravelVfxRules.PlanForFormula("FBL", 99, 2f, false, 2);
            AssertEqual(0f, clampedLow.Progress, "travel plan clamps progress below zero");
            AssertEqual(1f, clampedHigh.Progress, "travel plan clamps progress above one");
            AssertEqual(true, clampedLow.Scale >= CombatPowerTravelVfxRules.MinimumScale && clampedLow.Scale <= CombatPowerTravelVfxRules.MaximumPlanScale, "low-intensity travel plan scale stays bounded");
            AssertEqual(true, clampedHigh.Scale >= CombatPowerTravelVfxRules.MinimumScale && clampedHigh.Scale <= CombatPowerTravelVfxRules.MaximumPlanScale, "high-intensity travel plan scale stays bounded");
            AssertEqual(true, clampedHigh.TrailSampleCount <= CombatPowerTravelVfxRules.MaximumPlanTrailSamples, "high-intensity travel trail count stays bounded");
            AssertEqual(
                CombatPowerTravelVfxRules.StableTravelHash("FBL", 4, 2),
                CombatPowerTravelVfxRules.StableTravelHash("fireball", 4, 2),
                "Fireball aliases share deterministic travel hashing");
            AssertEqual(true, CombatPowerTravelVfxRules.StableTravelHash("FBL", 4, 2) != CombatPowerTravelVfxRules.StableTravelHash("FBL", 4, 3), "travel sampling channels remain decorrelated");
        }

        private static void AssertCombatPowerTravelPlansEquivalentAndBounded(
            CombatPowerTravelVfxPlan expected,
            CombatPowerTravelVfxPlan actual,
            string label)
        {
            AssertEqual(expected.Key, actual.Key, label + " key");
            AssertEqual(expected.AtlasCell, actual.AtlasCell, label + " atlas cell");
            AssertEqual(expected.Path, actual.Path, label + " path");
            AssertEqual(expected.Scale, actual.Scale, label + " scale");
            AssertEqual(expected.Opacity, actual.Opacity, label + " opacity");
            AssertEqual(expected.DurationSeconds, actual.DurationSeconds, label + " duration");
            AssertEqual(expected.TrailSampleCount, actual.TrailSampleCount, label + " trail sample count");
            AssertEqual(expected.Progress, actual.Progress, label + " progress");
            AssertEqual(expected.StableSeed, actual.StableSeed, label + " stable seed");
            AssertEqual(expected.LateralJitter, actual.LateralJitter, label + " lateral jitter");
            AssertEqual(expected.SpinDegrees, actual.SpinDegrees, label + " spin");
            if (!actual.HasTravel) return;
            AssertEqual(true, CombatPowerTravelVfxRules.IsAtlasCell(actual.AtlasCell), label + " atlas cell stays in range");
            AssertEqual(true, actual.Scale >= CombatPowerTravelVfxRules.MinimumScale && actual.Scale <= CombatPowerTravelVfxRules.MaximumPlanScale, label + " scale stays bounded");
            AssertEqual(true, actual.Opacity >= CombatPowerTravelVfxRules.MinimumOpacity && actual.Opacity <= CombatPowerTravelVfxRules.MaximumOpacity, label + " opacity stays bounded");
            AssertEqual(true, actual.DurationSeconds >= CombatPowerTravelVfxRules.MinimumDurationSeconds && actual.DurationSeconds <= CombatPowerTravelVfxRules.MaximumDurationSeconds, label + " duration stays bounded");
            AssertEqual(true, actual.TrailSampleCount >= CombatPowerTravelVfxRules.MinimumTrailSamples && actual.TrailSampleCount <= CombatPowerTravelVfxRules.MaximumPlanTrailSamples, label + " trail sample count stays bounded");
            AssertEqual(true, actual.Progress >= 0f && actual.Progress <= 1f, label + " progress stays normalized");
            AssertEqual(true, actual.StableSeed >= 0, label + " stable seed stays non-negative");
            AssertEqual(true, actual.LateralJitter >= -1f && actual.LateralJitter <= 1f, label + " lateral jitter stays normalized");
            AssertEqual(true, actual.SpinDegrees >= -180f && actual.SpinDegrees <= 180f, label + " spin stays bounded");
        }

        private static void CombatPowerAftermathAndTimelineStayDeterministic()
        {
            AssertEqual(4, CombatPowerAftermathVfxRules.AtlasColumns, "combat power aftermath VFX atlas columns");
            AssertEqual(4, CombatPowerAftermathVfxRules.AtlasRows, "combat power aftermath VFX atlas rows");
            AssertEqual(16, CombatPowerAftermathVfxRules.AtlasCellCount, "combat power aftermath VFX atlas cell count");
            string[] familyKeys =
            {
                "FBL", "MTR", "RCL", "RIG",
                "OIC", "TBQ", "GBH", "SBN",
                "RBT", "INH", "PBR", "DSM",
                "RKW", "IBG", "DFA", "whirlwind"
            };
            CombatPowerAftermathVfxProfile[] familyProfiles = familyKeys
                .Select(CombatPowerAftermathVfxRules.ProfileFor)
                .ToArray();
            AssertEqual(
                string.Join(",", Enumerable.Range(0, CombatPowerAftermathVfxRules.AtlasCellCount)),
                string.Join(",", familyProfiles.Select(profile => profile.AtlasCell)),
                "combat power aftermath families preserve the strict row-major 4x4 contract");
            AssertEqual(true, familyProfiles.All(profile => profile.HasAftermath), "every canonical aftermath family has visible art");

            Texture2D aftermathAtlas = null;
            try
            {
                aftermathAtlas = LoadApprovedRuntimeAtlas(RuntimeArtManifest.CombatPowerAftermathVfxAtlas);
                AssertEqual(new Vector2Int(1280, 1280), new Vector2Int(aftermathAtlas.width, aftermathAtlas.height), "approved combat power aftermath VFX atlas dimensions");
                AssertAtlasCellCoverage(
                    aftermathAtlas,
                    CombatPowerAftermathVfxRules.AtlasColumns,
                    CombatPowerAftermathVfxRules.AtlasRows,
                    Enumerable.Range(0, CombatPowerAftermathVfxRules.AtlasCellCount),
                    0.08f,
                    0.72f,
                    "approved combat power aftermath VFX");
                AssertAtlasCellSafeGutter(
                    aftermathAtlas,
                    CombatPowerAftermathVfxRules.AtlasColumns,
                    CombatPowerAftermathVfxRules.AtlasRows,
                    Enumerable.Range(0, CombatPowerAftermathVfxRules.AtlasCellCount),
                    24,
                    12,
                    0,
                    "approved combat power aftermath VFX");
                AssertAtlasHasNoBrightMagentaKeyField(aftermathAtlas, 12, 32, "approved combat power aftermath VFX");
            }
            finally
            {
                if (aftermathAtlas != null) UnityEngine.Object.DestroyImmediate(aftermathAtlas);
            }

            AssertEqual(FormulaCatalog.All.Length, FormulaCatalog.All.Count(formula => CombatPowerAftermathVfxRules.ProfileForFormula(formula.Code).HasAftermath), "all formulas have deterministic aftermath profiles");
            string[] abilityIds = new[] { "warrior", "rogue", "ranger", "demon" }
                .SelectMany(AbilityCatalog.IdsForClass)
                .ToArray();
            AssertEqual(25, abilityIds.Length, "aftermath ability probe covers the complete active skill catalog");
            AssertEqual(abilityIds.Length, abilityIds.Count(id => CombatPowerAftermathVfxRules.ProfileForAbility(id).HasAftermath), "all active skills have deterministic aftermath profiles");

            CombatPowerAftermathVfxPlan fireballPlan = CombatPowerAftermathVfxRules.PlanForFormula("FBL", 3, 0.42f, false, 9182);
            CombatPowerAftermathVfxPlan repeatedFireballPlan = CombatPowerAftermathVfxRules.PlanForFormula("fireball", 3, 0.42f, false, 9182);
            AssertEqual(fireballPlan.AtlasCell, repeatedFireballPlan.AtlasCell, "Fireball aftermath aliases preserve art identity");
            AssertEqual(fireballPlan.StableSeed, repeatedFireballPlan.StableSeed, "Fireball aftermath aliases preserve deterministic seeds");
            AssertEqual(fireballPlan.Scale, repeatedFireballPlan.Scale, "Fireball aftermath replay scale repeats exactly");
            AssertEqual(fireballPlan.Drift, repeatedFireballPlan.Drift, "Fireball aftermath replay drift repeats exactly");
            AssertEqual(true, fireballPlan.Scale >= CombatPowerAftermathVfxRules.MinimumScale && fireballPlan.Scale <= CombatPowerAftermathVfxRules.MaximumPlanScale, "Fireball aftermath scale stays bounded");
            AssertEqual(true, fireballPlan.Opacity > 0f && fireballPlan.Opacity <= CombatPowerAftermathVfxRules.MaximumOpacity, "Fireball aftermath opacity stays bounded");
            AssertEqual(true, fireballPlan.LayerCount >= 1 && fireballPlan.LayerCount <= CombatPowerAftermathVfxRules.MaximumLayerCount, "Fireball aftermath layers stay bounded");
            AssertEqual(true, fireballPlan.ParticleCount <= CombatPowerAftermathVfxRules.MaximumParticleCount, "Fireball aftermath particle recipe stays bounded");
            CombatPowerAftermathVfxPlan reducedAftermath = CombatPowerAftermathVfxRules.PlanForFormula("FBL", 3, 0f, true, 9182);
            AssertEqual(false, reducedAftermath.HasAftermath, "Reduced Motion suppresses lingering aftermath art");
            AssertEqual(0f, reducedAftermath.DurationSeconds, "Reduced Motion aftermath has no delayed duration");

            const int stableSeed = 77193;
            CombatPowerAnimationTimeline fireball = CombatPowerAnimationTimelineRules.ForFormula("FBL", stableSeed, 3, false);
            CombatPowerAnimationTimeline repeatedFireball = CombatPowerAnimationTimelineRules.ForFormula("fireball", stableSeed, 3, false);
            AssertEqual(true, fireball.Supported && fireball.HasTravel && fireball.HasAftermath, "Fireball owns the full cast, travel, impact, and aftermath timeline");
            AssertEqual(fireball.ReleaseAt, repeatedFireball.ReleaseAt, "Fireball replay release beat repeats exactly");
            AssertEqual(fireball.ImpactAt, repeatedFireball.ImpactAt, "Fireball replay impact beat repeats exactly");
            AssertEqual(fireball.AftermathAt, repeatedFireball.AftermathAt, "Fireball replay aftermath beat repeats exactly");
            AssertEqual(fireball.CompleteAt, repeatedFireball.CompleteAt, "Fireball replay completion beat repeats exactly");
            AssertEqual(true, fireball.CastAt <= fireball.ReleaseAt && fireball.ReleaseAt < fireball.ImpactAt && fireball.ImpactAt <= fireball.AftermathAt && fireball.AftermathAt < fireball.CompleteAt, "Fireball phases stay strictly ordered around travel and aftermath");
            AssertEqual(CombatPowerAnimationPhase.Cast, fireball.FrameAt(Mathf.Max(0f, fireball.ReleaseAt - 0.001f)).Phase, "Fireball remains in cast anticipation before release");
            AssertEqual(CombatPowerAnimationPhase.ReleaseTravel, fireball.FrameAt(fireball.ReleaseAt).Phase, "Fireball release boundary owns projectile travel");
            AssertEqual(CombatPowerAnimationPhase.Impact, fireball.FrameAt(fireball.ImpactAt).Phase, "Fireball impact boundary owns contact");
            AssertEqual(CombatPowerAnimationPhase.Aftermath, fireball.FrameAt(fireball.AftermathAt).Phase, "Fireball aftermath boundary owns its scorch finish");
            AssertEqual(CombatPowerAnimationPhase.Complete, fireball.FrameAt(fireball.CompleteAt).Phase, "Fireball completion boundary retires the animation");

            CombatPowerAnimationTimeline summon = CombatPowerAnimationTimelineRules.ForFormula("IBG", stableSeed, 3, false);
            AssertEqual(false, summon.HasTravel, "Greater Demon summon skips projectile travel");
            AssertEqual(CombatPowerAnimationPhase.Cast, summon.FrameAt(Mathf.Max(0f, summon.ImpactAt - 0.001f)).Phase, "no-travel summons stay in ritual anticipation until contact");
            AssertEqual(CombatPowerAnimationPhase.Impact, summon.FrameAt(summon.ImpactAt).Phase, "no-travel summons enter impact without a zero-duration travel phase");

            foreach (FormulaDef formula in FormulaCatalog.All)
            {
                CombatPowerAnimationTimeline timeline = CombatPowerAnimationTimelineRules.ForFormula(formula, stableSeed, 2, false);
                AssertEqual(true, timeline.Supported, formula.Code + " animation timeline is supported");
                AssertEqual(true, timeline.CastAt <= timeline.ReleaseAt && timeline.ReleaseAt <= timeline.ImpactAt && timeline.ImpactAt <= timeline.AftermathAt && timeline.AftermathAt <= timeline.CompleteAt, formula.Code + " animation phases are finite and monotonic");
                AssertEqual(CombatPowerAnimationPhase.Complete, timeline.FrameAt(timeline.CompleteAt + 1f).Phase, formula.Code + " animation reaches Complete");
            }
            foreach (string abilityId in abilityIds)
            {
                CombatPowerAnimationTimeline timeline = CombatPowerAnimationTimelineRules.ForAbility(abilityId, stableSeed, 2, false);
                AssertEqual(true, timeline.Supported, abilityId + " animation timeline is supported");
                AssertEqual(true, timeline.CastAt <= timeline.ReleaseAt && timeline.ReleaseAt <= timeline.ImpactAt && timeline.ImpactAt <= timeline.AftermathAt && timeline.AftermathAt <= timeline.CompleteAt, abilityId + " animation phases are finite and monotonic");
            }

            CombatPowerAnimationTimeline reduced = CombatPowerAnimationTimelineRules.ForFormula("FBL", stableSeed, 3, true);
            AssertEqual(false, reduced.HasTravel || reduced.HasAftermath, "Reduced Motion removes travel and lingering aftermath phases");
            AssertEqual(CombatPowerAnimationPhase.Impact, reduced.FrameAt(0f).Phase, "Reduced Motion starts on a compact static impact");
            AssertEqual(true, reduced.FrameAt(0f).StaticImpact, "Reduced Motion impact frame is explicitly static");
            AssertEqual(CombatPowerAnimationPhase.Complete, reduced.FrameAt(CombatPowerAnimationTimelineRules.ReducedMotionImpactHoldSeconds).Phase, "Reduced Motion completes after its finite static hold");
            CombatPowerAnimationTimeline unknown = CombatPowerAnimationTimelineRules.For("unknown power", stableSeed, 3, false);
            AssertEqual(false, unknown.Supported, "unknown animation powers remain unsupported");
            AssertEqual(CombatPowerAnimationPhase.Complete, unknown.FrameAt(0f).Phase, "unknown animation powers resolve directly to Complete");
            CombatTransientPresentationBoundariesAreExplicit();
        }

        private static void CombatPowerActorChoreographyStaysSynchronizedAndBounded()
        {
            const int stableSeed = 48271;
            const int sourceX = 1;
            const int sourceY = 2;
            const int landingX = 6;
            const int landingY = 4;
            const float epsilon = 0.0001f;

            string[] abilityIds = new[] { "warrior", "rogue", "ranger", "demon" }
                .SelectMany(AbilityCatalog.IdsForClass)
                .ToArray();
            AssertEqual(56, FormulaCatalog.All.Length, "actor choreography covers every canonical formula");
            AssertEqual(25, abilityIds.Length, "actor choreography covers every active skill");
            AssertEqual(25, abilityIds.Distinct(StringComparer.Ordinal).Count(), "actor choreography active-skill catalog is unique");

            List<CombatPowerActorPosePlan> plans = new List<CombatPowerActorPosePlan>();
            foreach (FormulaDef formula in FormulaCatalog.All)
            {
                CombatPowerActorPosePlan plan = CombatPowerActorPoseRules.ForFormula(
                    formula,
                    stableSeed,
                    sourceX,
                    sourceY,
                    landingX,
                    landingY,
                    3,
                    false);
                CombatPowerActorPosePlan alias = CombatPowerActorPoseRules.ForFormula(
                    formula.Name,
                    stableSeed,
                    sourceX,
                    sourceY,
                    landingX,
                    landingY,
                    3,
                    false);
                CombatPowerActorPosePlan generic = CombatPowerActorPoseRules.For(
                    formula.Code,
                    stableSeed,
                    sourceX,
                    sourceY,
                    landingX,
                    landingY,
                    3,
                    false);
                AssertEqual(true, plan.Supported, formula.Code + " actor choreography is supported");
                AssertEqual(CombatPowerAnimationSourceKind.Formula, plan.SourceKind, formula.Code + " actor choreography retains formula identity");
                AssertEqual(formula.Code, plan.PowerKey, formula.Code + " actor choreography retains its canonical key");
                AssertEqual(plan.PowerKey, alias.PowerKey, formula.Code + " full-name actor choreography keeps its canonical key");
                AssertEqual(plan.Choreography, alias.Choreography, formula.Code + " full-name actor choreography keeps its semantic kind");
                AssertEqual(plan.ReleaseAt, alias.ReleaseAt, formula.Code + " full-name actor choreography keeps its release boundary");
                AssertEqual(plan.ImpactAt, alias.ImpactAt, formula.Code + " full-name actor choreography keeps its impact boundary");
                AssertEqual(plan.DurationSeconds, alias.DurationSeconds, formula.Code + " full-name actor choreography keeps its duration");
                AssertEqual(plan.Choreography, generic.Choreography, formula.Code + " generic actor lookup keeps its semantic kind");
                AssertEqual(true, plan.ReleaseAt <= plan.ImpactAt && plan.ImpactAt <= plan.DurationSeconds, formula.Code + " actor choreography boundaries are monotonic");
                plans.Add(plan);
            }

            foreach (string abilityId in abilityIds)
            {
                MartialAbility ability = AbilityCatalog.For(abilityId);
                CombatPowerActorPosePlan plan = CombatPowerActorPoseRules.ForAbility(
                    ability,
                    stableSeed,
                    sourceX,
                    sourceY,
                    landingX,
                    landingY,
                    3,
                    false);
                CombatPowerActorPosePlan alias = CombatPowerActorPoseRules.ForAbility(
                    ability.Name,
                    stableSeed,
                    sourceX,
                    sourceY,
                    landingX,
                    landingY,
                    3,
                    false);
                CombatPowerActorPosePlan generic = CombatPowerActorPoseRules.For(
                    abilityId,
                    stableSeed,
                    sourceX,
                    sourceY,
                    landingX,
                    landingY,
                    3,
                    false);
                AssertEqual(true, plan.Supported, abilityId + " actor choreography is supported");
                AssertEqual(CombatPowerAnimationSourceKind.Ability, plan.SourceKind, abilityId + " actor choreography retains ability identity");
                AssertEqual(abilityId, plan.PowerKey, abilityId + " actor choreography retains its canonical key");
                AssertEqual(plan.PowerKey, alias.PowerKey, abilityId + " full-name actor choreography keeps its canonical key");
                AssertEqual(plan.Choreography, alias.Choreography, abilityId + " full-name actor choreography keeps its semantic kind");
                AssertEqual(plan.ReleaseAt, alias.ReleaseAt, abilityId + " full-name actor choreography keeps its release boundary");
                AssertEqual(plan.ImpactAt, alias.ImpactAt, abilityId + " full-name actor choreography keeps its impact boundary");
                AssertEqual(plan.DurationSeconds, alias.DurationSeconds, abilityId + " full-name actor choreography keeps its duration");
                AssertEqual(plan.Choreography, generic.Choreography, abilityId + " generic actor lookup keeps its semantic kind");
                AssertEqual(true, plan.ReleaseAt <= plan.ImpactAt && plan.ImpactAt <= plan.DurationSeconds, abilityId + " actor choreography boundaries are monotonic");
                plans.Add(plan);
            }

            AssertEqual(81, plans.Count, "actor choreography test matrix includes all formulas and active skills");
            foreach (CombatPowerActorPosePlan plan in plans)
            {
                float[] sampleTimes =
                {
                    -1f,
                    0f,
                    Mathf.Max(0f, plan.ReleaseAt - 0.001f),
                    plan.ReleaseAt,
                    (plan.ReleaseAt + plan.ImpactAt) * 0.5f,
                    Mathf.Max(0f, plan.ImpactAt - 0.001f),
                    plan.ImpactAt,
                    (plan.ImpactAt + plan.DurationSeconds) * 0.5f,
                    plan.DurationSeconds,
                    float.PositiveInfinity,
                    float.NaN
                };
                foreach (CombatPowerActorPoseRole role in Enum.GetValues(typeof(CombatPowerActorPoseRole)))
                {
                    foreach (float sampleTime in sampleTimes)
                    {
                        CombatPowerActorPoseFrame frame = plan.FrameAt(role, sampleTime);
                        string label = plan.PowerKey + " " + role + " actor frame at " + sampleTime;
                        AssertEqual(true, !float.IsNaN(frame.PositionX) && !float.IsInfinity(frame.PositionX), label + " has a finite X coordinate");
                        AssertEqual(true, !float.IsNaN(frame.PositionY) && !float.IsInfinity(frame.PositionY), label + " has a finite Y coordinate");
                        AssertEqual(true, frame.PositionX >= Math.Min(sourceX, landingX) - epsilon && frame.PositionX <= Math.Max(sourceX, landingX) + epsilon, label + " X coordinate stays between source and landing");
                        AssertEqual(true, frame.PositionY >= Math.Min(sourceY, landingY) - epsilon && frame.PositionY <= Math.Max(sourceY, landingY) + epsilon, label + " Y coordinate stays between source and landing");
                        AssertEqual(true, frame.OffsetX >= -CombatPowerActorPoseRules.MaximumOffset && frame.OffsetX <= CombatPowerActorPoseRules.MaximumOffset, label + " horizontal pose offset stays bounded");
                        AssertEqual(true, frame.OffsetY >= -CombatPowerActorPoseRules.MaximumOffset && frame.OffsetY <= CombatPowerActorPoseRules.MaximumOffset, label + " vertical pose offset stays bounded");
                        AssertEqual(true, frame.Scale >= CombatPowerActorPoseRules.MinimumScale && frame.Scale <= CombatPowerActorPoseRules.MaximumScale, label + " scale stays bounded");
                        AssertEqual(true, frame.Opacity >= 0f && frame.Opacity <= 1f, label + " opacity stays normalized");
                        AssertEqual(true, frame.LocalProgress >= 0f && frame.LocalProgress <= 1f, label + " phase progress stays normalized");
                    }
                }
            }

            CombatPowerActorPosePlan fireball = CombatPowerActorPoseRules.ForFormula("FBL", stableSeed, sourceX, sourceY, landingX, landingY, 3, false);
            CombatPowerActorPoseFrame fireballBeforeRelease = fireball.SourceFrameAt(Mathf.Max(0f, fireball.ReleaseAt - 0.001f));
            CombatPowerActorPoseFrame fireballRelease = fireball.SourceFrameAt(fireball.ReleaseAt);
            AssertEqual(CombatPowerActorPosePhase.CastWindup, fireballBeforeRelease.Phase, "Fireball actor remains in windup immediately before release");
            AssertEqual(CombatPowerActorPosePhase.Release, fireballRelease.Phase, "Fireball actor enters release on the exact projectile boundary");
            AssertEqual(fireball.ReleaseAt, fireballRelease.PhaseStartAt, "Fireball release pose begins on the authored release boundary");

            CombatPowerActorPosePlan charge = CombatPowerActorPoseRules.ForAbility("charge", stableSeed, sourceX, sourceY, landingX, landingY, 3, false);
            AssertEqual(CombatPowerActorChoreographyKind.Dash, charge.Choreography, "Charge uses continuous dash choreography");
            AssertEqual(true, charge.HasMovement && charge.ReleaseAt < charge.ImpactAt, "Charge moves only through a finite release-to-impact window");
            AssertEqual(CombatPowerActorPosePhase.CastWindup, charge.SourceFrameAt(charge.ReleaseAt - 0.001f).Phase, "Charge braces immediately before release");
            CombatPowerActorPoseFrame chargeRelease = charge.SourceFrameAt(charge.ReleaseAt);
            CombatPowerActorPoseFrame chargeMidDash = charge.SourceFrameAt((charge.ReleaseAt + charge.ImpactAt) * 0.5f);
            CombatPowerActorPoseFrame chargeImpact = charge.SourceFrameAt(charge.ImpactAt);
            AssertEqual(CombatPowerActorPosePhase.Dash, chargeRelease.Phase, "Charge begins its dash on ReleaseAt");
            AssertEqual((float)sourceX, chargeRelease.PositionX, "Charge begins its dash on the source cell");
            AssertEqual(true, chargeMidDash.Phase == CombatPowerActorPosePhase.Dash && chargeMidDash.PositionX > sourceX && chargeMidDash.PositionX < landingX, "Charge crosses the battlefield during its authored delivery window");
            AssertEqual(CombatPowerActorPosePhase.Recovery, chargeImpact.Phase, "Charge begins recovery on ImpactAt");
            AssertEqual((float)landingX, chargeImpact.PositionX, "Charge reaches its landing cell exactly on impact");
            AssertEqual(CombatPowerActorPosePhase.Complete, charge.TargetFrameAt(charge.ImpactAt - 0.001f).Phase, "Charge target recoil does not begin early");
            AssertEqual(CombatPowerActorPosePhase.TargetHit, charge.TargetFrameAt(charge.ImpactAt).Phase, "Charge target recoil begins on impact");

            Dictionary<string, CombatPowerActorChoreographyKind> signatureSkillChoreography =
                new Dictionary<string, CombatPowerActorChoreographyKind>(StringComparer.OrdinalIgnoreCase)
                {
                    ["whirlwind"] = CombatPowerActorChoreographyKind.Whirl,
                    ["abyssalwhirl"] = CombatPowerActorChoreographyKind.Whirl,
                    ["rally"] = CombatPowerActorChoreographyKind.Brace,
                    ["dreadroar"] = CombatPowerActorChoreographyKind.Brace,
                    ["volley"] = CombatPowerActorChoreographyKind.Bow,
                    ["quickshot"] = CombatPowerActorChoreographyKind.Bow,
                    ["stealth"] = CombatPowerActorChoreographyKind.Vanish,
                    ["smokebomb"] = CombatPowerActorChoreographyKind.Vanish,
                    ["sunder"] = CombatPowerActorChoreographyKind.HeavyStrike,
                    ["execute"] = CombatPowerActorChoreographyKind.HeavyStrike
                };
            foreach (KeyValuePair<string, CombatPowerActorChoreographyKind> expected in signatureSkillChoreography)
            {
                CombatPowerActorPosePlan signature = CombatPowerActorPoseRules.ForAbility(
                    expected.Key,
                    stableSeed,
                    sourceX,
                    sourceY,
                    landingX,
                    landingY,
                    3,
                    false);
                AssertEqual(expected.Value, signature.Choreography, expected.Key + " keeps its signature actor body language");
                CombatPowerActorPoseFrame windup = signature.SourceFrameAt(Mathf.Max(0f, signature.ReleaseAt - 0.001f));
                CombatPowerActorPoseFrame release = signature.SourceFrameAt((signature.ReleaseAt + signature.ReleaseEndAt) * 0.5f);
                AssertEqual(CombatPowerActorPosePhase.CastWindup, windup.Phase, expected.Key + " anticipates before release");
                AssertEqual(CombatPowerActorPosePhase.Release, release.Phase, expected.Key + " performs its signature release pose");
                AssertEqual(true, Math.Abs(release.OffsetX) > 0.001f || Math.Abs(release.OffsetY) > 0.001f || Math.Abs(release.Scale - 1f) > 0.001f || release.Opacity < 0.99f,
                    expected.Key + " release is visually distinct from a neutral actor frame");

                CombatPowerActorPosePlan reducedSignature = CombatPowerActorPoseRules.ForAbility(
                    expected.Key,
                    stableSeed,
                    sourceX,
                    sourceY,
                    landingX,
                    landingY,
                    3,
                    true);
                CombatPowerActorPoseFrame reducedFrame = reducedSignature.SourceFrameAt(0f);
                AssertEqual(true, reducedFrame.IsStaticFallback, expected.Key + " Reduced Motion uses a static semantic pose");
                AssertEqual(0f, reducedFrame.OffsetX, expected.Key + " Reduced Motion removes horizontal actor motion");
                AssertEqual(0f, reducedFrame.OffsetY, expected.Key + " Reduced Motion removes vertical actor motion");
                AssertEqual(1f, reducedFrame.Scale, expected.Key + " Reduced Motion keeps neutral actor scale");
            }

            string[] teleportKeys = { "VST", "VRS", "shadowstep", "riftpounce" };
            foreach (string teleportKey in teleportKeys)
            {
                CombatPowerActorPosePlan teleport = CombatPowerActorPoseRules.For(teleportKey, stableSeed, sourceX, sourceY, landingX, landingY, 3, false);
                AssertEqual(CombatPowerActorChoreographyKind.Teleport, teleport.Choreography, teleportKey + " uses vanish-and-arrive choreography");
                AssertEqual(true, teleport.ReleaseAt < teleport.TeleportSplitAt && teleport.TeleportSplitAt < teleport.ImpactAt, teleportKey + " owns distinct vanish and arrival windows");
                CombatPowerActorPoseFrame teleportRelease = teleport.SourceFrameAt(teleport.ReleaseAt);
                CombatPowerActorPoseFrame teleportArrival = teleport.SourceFrameAt(teleport.TeleportSplitAt);
                CombatPowerActorPoseFrame teleportImpact = teleport.SourceFrameAt(teleport.ImpactAt);
                AssertEqual(CombatPowerActorPosePhase.TeleportOut, teleportRelease.Phase, teleportKey + " begins vanishing on ReleaseAt");
                AssertEqual((float)sourceX, teleportRelease.PositionX, teleportKey + " vanishes from its source cell");
                AssertEqual(CombatPowerActorPosePhase.TeleportIn, teleportArrival.Phase, teleportKey + " begins appearing at the authored split boundary");
                AssertEqual((float)landingX, teleportArrival.PositionX, teleportKey + " reappears on its landing cell instead of sliding between cells");
                AssertEqual(CombatPowerActorPosePhase.Recovery, teleportImpact.Phase, teleportKey + " settles on ImpactAt");
                AssertEqual((float)landingX, teleportImpact.PositionX, teleportKey + " remains on its landing cell after contact");
            }

            string[] summonKeys = { "IBD", "IBF", "IBG" };
            foreach (string summonKey in summonKeys)
            {
                CombatPowerActorPosePlan summon = CombatPowerActorPoseRules.ForFormula(summonKey, stableSeed, sourceX, sourceY, landingX, landingY, 3, false);
                AssertEqual(CombatPowerActorChoreographyKind.Summon, summon.Choreography, summonKey + " uses landing-reveal choreography");
                AssertEqual(true, summon.HasLandingReveal && summon.SummonRevealEndAt > summon.ImpactAt, summonKey + " owns a finite post-impact summon reveal");
                CombatPowerActorPoseFrame hiddenSummon = summon.LandingFrameAt(summon.ImpactAt - 0.001f);
                CombatPowerActorPoseFrame summonImpact = summon.LandingFrameAt(summon.ImpactAt);
                CombatPowerActorPoseFrame summonReveal = summon.LandingFrameAt((summon.ImpactAt + summon.SummonRevealEndAt) * 0.5f);
                AssertEqual(false, hiddenSummon.IsVisible, summonKey + " summon remains hidden immediately before impact");
                AssertEqual(CombatPowerActorPosePhase.SummonReveal, summonImpact.Phase, summonKey + " summon reveal begins on ImpactAt");
                AssertEqual(0f, summonImpact.Opacity, summonKey + " summon reveal begins from zero opacity");
                AssertEqual(true, summonReveal.IsVisible && summonReveal.Scale != 1f, summonKey + " summon visibly rises into its landing cell");
                AssertEqual(CombatPowerActorPosePhase.Complete, summon.LandingFrameAt(summon.SummonRevealEndAt).Phase, summonKey + " summon reveal retires at its exact end boundary");
            }

            CombatPowerActorPosePlan morph = CombatPowerActorPoseRules.ForFormula("DFA", stableSeed, sourceX, sourceY, landingX, landingY, 3, false);
            AssertEqual(CombatPowerActorChoreographyKind.Morph, morph.Choreography, "Abyssal Ascendance uses transformation choreography");
            AssertEqual(true, morph.HasMorph && morph.MorphOutStartAt < morph.ImpactAt && morph.ImpactAt < morph.MorphInEndAt, "Abyssal Ascendance owns distinct morph-out and morph-in windows");
            AssertEqual(CombatPowerActorPosePhase.CastWindup, morph.SourceFrameAt(morph.MorphOutStartAt - 0.001f).Phase, "Abyssal Ascendance gathers before the transformation");
            AssertEqual(CombatPowerActorPosePhase.MorphOut, morph.SourceFrameAt(morph.MorphOutStartAt).Phase, "Abyssal Ascendance begins dissolving at its morph-out boundary");
            CombatPowerActorPoseFrame morphImpact = morph.SourceFrameAt(morph.ImpactAt);
            AssertEqual(CombatPowerActorPosePhase.MorphIn, morphImpact.Phase, "Abyssal Ascendance reveals the new form on ImpactAt");
            AssertEqual((float)landingX, morphImpact.PositionX, "Abyssal Ascendance reveals the transformed actor at its resolved cell");
            AssertEqual(CombatPowerActorPosePhase.Recovery, morph.SourceFrameAt(morph.MorphInEndAt).Phase, "Abyssal Ascendance settles after its reveal window");

            CombatPowerActorPoseFrame deterministicFrame = fireball.SourceFrameAt((fireball.ReleaseAt + fireball.ReleaseEndAt) * 0.5f);
            CombatPowerActorPoseFrame repeatedFrame = CombatPowerActorPoseRules.ForFormula("fireball", stableSeed, sourceX, sourceY, landingX, landingY, 3, false)
                .SourceFrameAt((fireball.ReleaseAt + fireball.ReleaseEndAt) * 0.5f);
            AssertEqual(deterministicFrame.Phase, repeatedFrame.Phase, "Fireball actor frame phase repeats across aliases");
            AssertEqual(deterministicFrame.PositionX, repeatedFrame.PositionX, "Fireball actor frame position repeats across aliases");
            AssertEqual(deterministicFrame.OffsetX, repeatedFrame.OffsetX, "Fireball actor frame horizontal pose repeats across aliases");
            AssertEqual(deterministicFrame.OffsetY, repeatedFrame.OffsetY, "Fireball actor frame vertical pose repeats across aliases");
            AssertEqual(deterministicFrame.Scale, repeatedFrame.Scale, "Fireball actor frame scale repeats across aliases");
            AssertEqual(deterministicFrame.Opacity, repeatedFrame.Opacity, "Fireball actor frame opacity repeats across aliases");
            int actorHash = CombatPowerActorPoseRules.StableActorHash("FBL", stableSeed, CombatPowerActorPoseRole.Source, CombatPowerActorPosePhase.Release, 2);
            AssertEqual(actorHash, CombatPowerActorPoseRules.StableActorHash("fireball", stableSeed, CombatPowerActorPoseRole.Source, CombatPowerActorPosePhase.Release, 2), "Fireball aliases share deterministic actor hashing");
            AssertEqual(true, actorHash != CombatPowerActorPoseRules.StableActorHash("FBL", stableSeed, CombatPowerActorPoseRole.Source, CombatPowerActorPosePhase.Release, 3), "actor pose sampling channels remain decorrelated");
            float actorSample = CombatPowerActorPoseRules.StableActorSample("FBL", stableSeed, CombatPowerActorPoseRole.Source, CombatPowerActorPosePhase.Release, 2);
            float signedActorSample = CombatPowerActorPoseRules.StableActorSignedSample("FBL", stableSeed, CombatPowerActorPoseRole.Source, CombatPowerActorPosePhase.Release, 2);
            AssertEqual(true, actorSample >= 0f && actorSample < 1f, "actor pose sampling stays normalized");
            AssertEqual(true, signedActorSample >= -1f && signedActorSample < 1f, "signed actor pose sampling stays normalized");

            CombatPowerActorPosePlan reducedCharge = CombatPowerActorPoseRules.ForAbility("charge", stableSeed, sourceX, sourceY, landingX, landingY, 3, true);
            CombatPowerActorPoseFrame reducedChargeFrame = reducedCharge.SourceFrameAt(0f);
            AssertEqual(true, reducedCharge.Supported && reducedCharge.ReducedMotion, "Reduced Motion keeps Charge choreography semantically supported");
            AssertEqual(true, reducedChargeFrame.IsStaticFallback, "Reduced Motion replaces Charge motion with a static semantic frame");
            AssertEqual(CombatPowerActorPosePhase.Dash, reducedChargeFrame.Phase, "Reduced Motion retains Charge's dash identity");
            AssertEqual((float)landingX, reducedChargeFrame.PositionX, "Reduced Motion places Charge directly on its resolved landing cell");
            AssertEqual(0f, reducedChargeFrame.OffsetX, "Reduced Motion removes Charge horizontal pose motion");
            AssertEqual(0f, reducedChargeFrame.OffsetY, "Reduced Motion removes Charge vertical pose motion");
            AssertEqual(1f, reducedChargeFrame.Scale, "Reduced Motion keeps Charge at neutral scale");
            AssertEqual(1f, reducedChargeFrame.Opacity, "Reduced Motion keeps Charge fully legible");
            AssertEqual(CombatPowerActorPosePhase.Complete, reducedCharge.SourceFrameAt(reducedCharge.DurationSeconds).Phase, "Reduced Motion Charge completes after its finite static hold");

            CombatPowerActorPosePlan reducedTeleport = CombatPowerActorPoseRules.ForAbility("shadowstep", stableSeed, sourceX, sourceY, landingX, landingY, 3, true);
            CombatPowerActorPoseFrame reducedTeleportFrame = reducedTeleport.SourceFrameAt(0f);
            AssertEqual(true, reducedTeleportFrame.IsStaticFallback, "Reduced Motion replaces Shadowstep with a static arrival");
            AssertEqual(CombatPowerActorPosePhase.TeleportIn, reducedTeleportFrame.Phase, "Reduced Motion retains Shadowstep's arrival identity");
            AssertEqual((float)landingX, reducedTeleportFrame.PositionX, "Reduced Motion snaps Shadowstep to its landing cell");

            CombatPowerActorPosePlan reducedSummon = CombatPowerActorPoseRules.ForFormula("IBG", stableSeed, sourceX, sourceY, landingX, landingY, 3, true);
            CombatPowerActorPoseFrame reducedSummonFrame = reducedSummon.LandingFrameAt(0f);
            AssertEqual(true, reducedSummonFrame.IsStaticFallback, "Reduced Motion replaces summon materialization with a static reveal");
            AssertEqual(CombatPowerActorPosePhase.SummonReveal, reducedSummonFrame.Phase, "Reduced Motion retains summon reveal identity");

            CombatPowerActorPosePlan reducedMorph = CombatPowerActorPoseRules.ForFormula("DFA", stableSeed, sourceX, sourceY, landingX, landingY, 3, true);
            CombatPowerActorPoseFrame reducedMorphFrame = reducedMorph.SourceFrameAt(0f);
            AssertEqual(true, reducedMorphFrame.IsStaticFallback, "Reduced Motion replaces transformation motion with a static reveal");
            AssertEqual(CombatPowerActorPosePhase.MorphIn, reducedMorphFrame.Phase, "Reduced Motion retains transformation reveal identity");
            AssertEqual((float)landingX, reducedMorphFrame.PositionX, "Reduced Motion resolves the transformed actor at its landing cell");

            CombatPowerActorPosePlan unknown = CombatPowerActorPoseRules.For("unknown power", stableSeed, sourceX, sourceY, landingX, landingY, 3, false);
            CombatPowerActorPoseFrame unknownFrame = unknown.SourceFrameAt(0f);
            AssertEqual(false, unknown.Supported, "unknown powers do not create actor choreography");
            AssertEqual(true, unknown.IsEmpty, "unknown actor choreography remains empty");
            AssertEqual(CombatPowerActorPosePhase.Complete, unknownFrame.Phase, "unknown actor choreography resolves directly to Complete");
            AssertEqual(false, unknownFrame.HasPose || unknownFrame.IsVisible, "unknown actor choreography never exposes a visible pose");
            AssertEqual((float)sourceX, unknownFrame.PositionX, "unknown actor choreography retains the source coordinate fallback");
        }

        private static void BetaLabToolbarRulesStayResponsiveAndAccessible()
        {
            AssertEqual(
                "Refill|Mage|Warlock|Craft|Stage|Hazards|Spawn|Reset|VisualTour",
                string.Join("|", BetaLabToolbarRules.Actions(BetaLabKind.Caster).Select(action => action.Id.ToString())),
                "caster Beta Lab keeps its exact action order");
            AssertEqual(
                "Refill|Promote|Wound|Cluster|Reset|Spawn|VisualTour",
                string.Join("|", BetaLabToolbarRules.Actions(BetaLabKind.Martial).Select(action => action.Id.ToString())),
                "martial Beta Lab keeps its exact action order");
            BetaLabToolbarActionDefinition visualTour = BetaLabToolbarRules.Actions(BetaLabKind.Caster).Last();
            AssertEqual(true, visualTour.VisualOnly, "Beta VFX/SFX tour is explicitly nonmutating");
            AssertEqual("Visual-only Tour", visualTour.Label, "Beta tour labels its presentation-only contract");
            AssertEqual(true, visualTour.Description.Contains("without casting"), "Beta tour describes its non-casting behavior");

            float[] widths = { 448f, 588f, 652f, 864f, 900f };
            foreach (BetaLabKind kind in Enum.GetValues(typeof(BetaLabKind)))
            {
                foreach (float width in widths)
                {
                    BetaLabToolbarGeometry geometry = BetaLabToolbarRules.Calculate(
                        new Rect(12f, 18f, width, BetaLabToolbarRules.ToolbarHeight),
                        kind);
                    AssertEqual(true, geometry.Fits(), $"{kind} Beta toolbar fits {width}px without overlap");
                    AssertEqual(2, geometry.RowCount, $"{kind} Beta toolbar uses two readable rows at {width}px");
                    AssertEqual(BetaLabToolbarRules.Actions(kind).Count, geometry.ActionCount, $"{kind} Beta toolbar exposes every action at {width}px");
                    AssertEqual(true, geometry.ActionRects.All(rect => rect.width >= BetaLabToolbarRules.MinimumActionWidth), $"{kind} Beta controls retain minimum hit width at {width}px");
                }
            }

            AssertEqual(0, BetaLabToolbarRules.NextIndex(BetaLabKind.Caster, -1), "unset Beta selection recovers to first action");
            AssertEqual(0, BetaLabToolbarRules.NextIndex(BetaLabKind.Caster, 8), "caster Beta next wraps after Visual-only Tour");
            AssertEqual(8, BetaLabToolbarRules.PreviousIndex(BetaLabKind.Caster, 0), "caster Beta previous wraps before Refill");
            AssertEqual(4, BetaLabToolbarRules.Navigate(BetaLabKind.Caster, 0, BetaLabToolbarNavigation.Left), "caster horizontal navigation wraps within its first row");
            AssertEqual(5, BetaLabToolbarRules.Navigate(BetaLabKind.Caster, 0, BetaLabToolbarNavigation.Down), "caster vertical navigation maps by button center into row two");
            AssertEqual(0, BetaLabToolbarRules.Navigate(BetaLabKind.Caster, 5, BetaLabToolbarNavigation.Up), "caster vertical navigation returns to the aligned first-row action");
            AssertEqual(6, BetaLabToolbarRules.Navigate(BetaLabKind.Martial, 0, BetaLabToolbarNavigation.Previous), "martial bumper navigation wraps linearly");
            AssertEqual(true, BetaLabToolbarRules.KeyboardNavigationHint.Contains("Enter/Space"), "Beta toolbar publishes keyboard activation guidance");
            AssertEqual(true, BetaLabToolbarRules.ControllerNavigationHint.Contains("A: use"), "Beta toolbar publishes controller activation guidance");
            AssertEqual(false, BetaLabToolbarRules.ControllerNavigationHint.Contains("D-pad"), "Beta toolbar does not advertise an unconfigured controller D-pad axis");

            BetaLabBuildFlavorProfile retail = BetaLabBuildFlavorRules.ProfileFor(BetaLabBuildFlavor.Retail);
            BetaLabBuildFlavorProfile beta = BetaLabBuildFlavorRules.ProfileFor(BetaLabBuildFlavor.BetaDevelopment);
            AssertEqual(true, retail.IsRetailRelease && !retail.ShowsTitleBetaLab && !retail.RequiresUnityDevelopmentBuild, "retail build keeps developer tools hidden");
            AssertEqual(true, !beta.IsRetailRelease && beta.ShowsTitleBetaLab && beta.RequiresUnityDevelopmentBuild, "Beta artifact requires a Unity Development build and exposes the title lab");
            AssertEqual(true, BetaLabBuildFlavorRules.MatchesUnityBuild(BetaLabBuildFlavor.Retail, false), "retail flavor matches a non-development Unity player");
            AssertEqual(true, BetaLabBuildFlavorRules.MatchesUnityBuild(BetaLabBuildFlavor.BetaDevelopment, true), "Beta flavor matches a Unity Development player");
            AssertEqual(false, BetaLabBuildFlavorRules.MatchesUnityBuild(BetaLabBuildFlavor.BetaDevelopment, false), "Beta flavor rejects an accidentally retail-compiled player");
            AssertEqual(
                "AshAndBrimstone-Windows-v2.21.0-beta-dev",
                BetaLabBuildFlavorRules.WindowsArtifactName("AshAndBrimstone", "v2.21.0", BetaLabBuildFlavor.BetaDevelopment),
                "Beta Development artifact remains distinct from retail");
            AssertEqual(
                "AshAndBrimstone-Windows-v2.21.0-beta-dev.zip",
                BetaLabBuildFlavorRules.WindowsZipFileName("AshAndBrimstone", "v2.21.0", BetaLabBuildFlavor.BetaDevelopment),
                "Beta Development zip remains distinct from retail");

            string combatSource = File.ReadAllText(Path.Combine(Application.dataPath, "Scripts", "Legacy", "AshenHallsGame.Combat.cs"));
            string coreSource = File.ReadAllText(Path.Combine(Application.dataPath, "Scripts", "Legacy", "AshenHallsGame.Core.cs"));
            string buildSource = File.ReadAllText(Path.Combine(Application.dataPath, "Editor", "BuildWindows.cs"));
            string betaBuildScript = File.ReadAllText(Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Tools", "BuildBetaLabWindows.ps1"));
            AssertEqual(true,
                new[]
                {
                    "TryHandleBetaVfxShowcaseInput()", "KeyCode.Q", "KeyCode.E", "KeyCode.C", "KeyCode.V",
                    "JoystickButton4", "JoystickButton5", "JoystickButton2", "JoystickButton3",
                    "ReplayBetaVfxShowcase();", "CueBetaVfxShowcaseAudio();", "ToggleReducedMotionSetting();"
                }.All(token => combatSource.Contains(token)),
                "focused Beta tour routes keyboard and controller replay, next/previous, cue, and motion controls");
            int executeLabAt = combatSource.IndexOf("private void ExecuteBetaLabToolbarAction", StringComparison.Ordinal);
            int executeLabGuardAt = combatSource.IndexOf("if (IsCombatResolutionPending())", executeLabAt, StringComparison.Ordinal);
            int executeLabSwitchAt = combatSource.IndexOf("switch (actionId)", executeLabAt, StringComparison.Ordinal);
            int showcaseDrawAt = combatSource.IndexOf("private void DrawBetaVfxShowcaseToolbar", StringComparison.Ordinal);
            int showcaseDrawEndAt = combatSource.IndexOf("private void ReplayBetaVfxShowcase", showcaseDrawAt, StringComparison.Ordinal);
            string showcaseDrawBody = showcaseDrawAt >= 0 && showcaseDrawEndAt > showcaseDrawAt
                ? combatSource.Substring(showcaseDrawAt, showcaseDrawEndAt - showcaseDrawAt)
                : "";
            AssertEqual(true,
                combatSource.Contains("GUI.enabled = guiEnabled && !actionsLocked;")
                && executeLabAt >= 0
                && executeLabGuardAt > executeLabAt
                && executeLabSwitchAt > executeLabGuardAt
                && showcaseDrawBody.Contains("actionsLocked = IsCombatResolutionPending()")
                && showcaseDrawBody.Contains("GUI.enabled = guiEnabled && !actionsLocked")
                && combatSource.Contains("if (IsCombatResolutionPending() || !betaLabMode || state?.Combat == null) return;"),
                "Beta mouse, keyboard, controller, replay, and cue actions stay locked while production combat presentation resolves");
            int closeOverlayAt = coreSource.IndexOf("if (CloseTopOverlay()) return true;", StringComparison.Ordinal);
            int cancelLabAt = coreSource.IndexOf("if (CancelBetaLabToolbarFocus()) return true;", StringComparison.Ordinal);
            int cancelTargetAt = coreSource.IndexOf("CancelCombatTargeting()", cancelLabAt, StringComparison.Ordinal);
            AssertEqual(true, closeOverlayAt >= 0 && cancelLabAt > closeOverlayAt && cancelTargetAt > cancelLabAt,
                "Cancel closes modal overlays first, then the focused Beta preview/rail before combat targeting or Pause");
            AssertEqual(true,
                buildSource.Contains("public static void BuildBeta()")
                && buildSource.Contains("BuildOptions.Development")
                && buildSource.Contains("WriteBetaLabPackageNote")
                && buildSource.Contains("press T (or Ctrl+Shift+B)"),
                "BuildWindows exposes a distinct Unity Development Beta artifact without weakening retail");
            AssertEqual(true,
                betaBuildScript.Contains("-ashen-beta-title-smoke")
                && betaBuildScript.Contains("development title exposes Beta Lab")
                && betaBuildScript.Contains("Compress-Archive")
                && betaBuildScript.Contains("-LiteralPath $outputRoot")
                && betaBuildScript.Contains("Expand-Archive")
                && betaBuildScript.Contains("$extractedPlayerExe")
                && betaBuildScript.Contains("QA\\beta-development")
                && !betaBuildScript.Contains("Join-Path $outputRoot \"beta-title-smoke.log\"")
                && betaBuildScript.Contains("Get-FileHash"),
                "Beta build wrapper archives the named folder, clean-extracts the packaged player, keeps machine logs outside it, and reports a hash");
        }

        private static void CombatTransientPresentationBoundariesAreExplicit()
        {
            string combatSource = File.ReadAllText(Path.Combine(Application.dataPath, "Scripts", "Legacy", "AshenHallsGame.Combat.cs"));
            string coreSource = File.ReadAllText(Path.Combine(Application.dataPath, "Scripts", "Legacy", "AshenHallsGame.Core.cs"));
            int resetAt = combatSource.IndexOf("private void ClearTransientCombatPresentation()", StringComparison.Ordinal);
            int resetEndAt = combatSource.IndexOf("private void RefillBetaLab()", resetAt, StringComparison.Ordinal);
            int startCombatAt = combatSource.IndexOf("private void StartCombat(EncounterDefinition encounter)", StringComparison.Ordinal);
            int startCombatEndAt = combatSource.IndexOf("private void ApplyEncounterPlacements", startCombatAt, StringComparison.Ordinal);
            int startCombatResetAt = combatSource.IndexOf("ClearTransientCombatPresentation();", startCombatAt, StringComparison.Ordinal);
            int adoptAt = coreSource.IndexOf("private void AdoptLoadedGameState", StringComparison.Ordinal);
            int adoptEndAt = coreSource.IndexOf("private bool BlockPersistenceDuringCombatResolution", adoptAt, StringComparison.Ordinal);
            int successfulAdoptionAt = coreSource.IndexOf("labSaveBlocked = false;", adoptAt, StringComparison.Ordinal);
            int adoptedResetAt = coreSource.IndexOf("ClearTransientCombatPresentation();", adoptAt, StringComparison.Ordinal);
            string resetBody = resetAt >= 0 && resetEndAt > resetAt
                ? combatSource.Substring(resetAt, resetEndAt - resetAt)
                : "";

            AssertEqual(true, resetAt >= 0 && resetEndAt > resetAt, "combat owns one explicit transient-presentation reset");
            AssertEqual(true,
                new[]
                {
                    "tweens.Clear();", "floatTexts.Clear();", "particles.Clear();", "beams.Clear();", "flashes.Clear();", "castGlyphs.Clear();",
                    "powerCastAuras.Clear();", "powerImpactEchoes.Clear();", "powerTravelVfx.Clear();", "powerAftermathVfx.Clear();",
                    "combatPowerCue = default;", "combatPowerOutcomeText = \"\";", "ClearCombatAudioForReducedMotion();"
                }.All(token => resetBody.IndexOf(token, StringComparison.Ordinal) >= 0),
                "combat transient reset covers legacy motion, authored timeline, cue, outcome, and scheduled audio layers");
            AssertEqual(true, startCombatResetAt > startCombatAt && startCombatResetAt < startCombatEndAt,
                "every new encounter clears prior transient combat presentation before staging the encounter");
            AssertEqual(true, adoptAt >= 0 && successfulAdoptionAt > adoptAt && adoptedResetAt > successfulAdoptionAt && adoptedResetAt < adoptEndAt,
                "load adoption clears prior transient combat presentation only after validation and state adoption succeed");
        }

        private static void CombatVfxShowcaseCatalogIsStableAndReplayable()
        {
            IReadOnlyList<CombatVfxShowcaseEntry> entries = CombatVfxShowcaseRules.Supported;
            AssertEqual(31, CombatVfxShowcaseRules.Count, "combat VFX showcase entry count");
            AssertEqual(CombatVfxShowcaseRules.Count, entries.Count, "combat VFX showcase count matches its read-only catalog");
            AssertEqual(
                "FBL|MTR|RCL|OBL|AST|VST|RBT|INH|IBD|IBF|IBG|DFA|DMC|HLC|SLV|PBR|VRS|RLM|charge|whirlwind|abyssalwhirl|rally|dreadroar|quickshot|stealth|smokebomb|sunder|execute|shadowstep|riftpounce|volley",
                string.Join("|", entries.Select(entry => entry.Id)),
                "combat VFX showcase keeps its exact regression-tour order");
            AssertEqual(
                "Fireball|Meteor Shower|Cold Lance|Light Bolt|Arcane Tempest|Thunder Step|Rift Bolt|Drain Life|Summon Imp|Summon Lesser Demon|Summon Greater Demon|Abyssal Ascendance|Doom Circle|Hallowed Circle|Soul Veil|Pact Brand|Rift Step|Death Burst|Charge|Whirlwind|Abyssal Whirl|Rally|Dread Roar|Quick Shot|Stealth|Smoke Bomb|Sunder|Execute|Shadowstep|Rift Pounce|Volley",
                string.Join("|", entries.Select(entry => entry.DisplayName)),
                "combat VFX showcase keeps exact player-facing power names");
            AssertEqual(
                "Formula|Formula|Formula|Formula|Formula|Formula|Formula|Formula|Formula|Formula|Formula|Formula|Formula|Formula|Formula|Formula|Formula|Formula|Ability|Ability|Ability|Ability|Ability|Ability|Ability|Ability|Ability|Ability|Ability|Ability|Ability",
                string.Join("|", entries.Select(entry => entry.Kind.ToString())),
                "combat VFX showcase distinguishes its eighteen formulas and thirteen abilities");
            AssertEqual(
                "Projectile|AreaBombardment|Projectile|Projectile|AreaStorm|TeleportStrike|Projectile|Projectile|Summon|Summon|Summon|Transformation|GroundField|GroundField|SupportWard|AreaHex|TeleportStrike|AreaBurst|MovementStrike|MeleeArea|MeleeArea|SelfAura|SelfAura|RangedArea|SelfAura|SelfAura|MeleeStrike|MeleeStrike|TeleportStrike|TeleportStrike|RangedArea",
                string.Join("|", entries.Select(entry => entry.Scenario.ToString())),
                "combat VFX showcase scenarios preserve every intended battlefield shape");
            CombatVfxShowcaseEntry[] movementEntries = entries
                .Where(entry => entry.Scenario == CombatVfxShowcaseScenario.MovementStrike || entry.Scenario == CombatVfxShowcaseScenario.TeleportStrike)
                .ToArray();
            AssertEqual(
                "VST|VRS|charge|shadowstep|riftpounce",
                string.Join("|", movementEntries.Select(entry => entry.Id)),
                "combat VFX showcase explicitly covers every authored actor-movement family");
            AssertEqual(
                "Teleport|Teleport|Dash|Teleport|Teleport",
                string.Join("|", movementEntries.Select(entry => CombatPowerActorPoseRules.For(entry.Id, entry.StableSeed, 1, 1, 4, 2, 3, false).Choreography.ToString())),
                "combat VFX showcase movement entries route through their exact actor choreography");
            AssertEqual(
                true,
                movementEntries.All(entry => CombatPowerActorPoseRules.For(entry.Id, entry.StableSeed, 1, 1, 4, 2, 3, false).HasMovement),
                "combat VFX showcase movement entries all stage a source-to-landing actor plan");
            AssertEqual(true, entries.All(entry => entry.Supported), "every combat VFX showcase entry is actionable");
            AssertEqual(true, entries.All(entry => entry.StableSeed > 0), "every combat VFX showcase replay seed is positive");
            AssertEqual(31, entries.Select(entry => entry.StableSeed).Distinct().Count(), "combat VFX showcase entries use distinct replay seeds");

            for (int index = 0; index < entries.Count; index++)
            {
                CombatVfxShowcaseEntry entry = entries[index];
                AssertEqual(index, CombatVfxShowcaseRules.IndexFor(entry.Id.ToLowerInvariant()), entry.Id + " showcase lookup is case-insensitive");
                AssertEqual(index, CombatVfxShowcaseRules.IndexFor("  " + entry.Id.ToLowerInvariant() + "  "), entry.Id + " showcase lookup trims surrounding whitespace");
                AssertEqual(true, CombatVfxShowcaseRules.IsSupported(entry.Id.ToLowerInvariant()), entry.Id + " showcase support lookup is case-insensitive");
                AssertEqual(entry.Id, CombatVfxShowcaseRules.At(index).Id, entry.Id + " showcase indexed lookup is stable");
                AssertEqual(entry.StableSeed, CombatVfxShowcaseRules.StableSeedFor(entry.Id.ToLowerInvariant()), entry.Id + " showcase seed lookup is case-insensitive");
                AssertEqual(entry.StableSeed, CombatVfxShowcaseRules.StableSeedFor(entry.Id), entry.Id + " showcase seed repeats exactly");
            }

            AssertEqual("volley", CombatVfxShowcaseRules.At(-1).Id, "combat VFX showcase wraps backward to its final entry");
            AssertEqual("FBL", CombatVfxShowcaseRules.At(31).Id, "combat VFX showcase wraps forward to its first entry");
            AssertEqual(0, CombatVfxShowcaseRules.NextIndex(30), "combat VFX showcase Next wraps after Volley");
            AssertEqual(0, CombatVfxShowcaseRules.NextIndex(-1), "combat VFX showcase starts at Fireball from an unset index");
            AssertEqual(1, CombatVfxShowcaseRules.NextIndex("fbl"), "combat VFX showcase Next advances from a case-insensitive ID");
            AssertEqual(0, CombatVfxShowcaseRules.NextIndex("unknown"), "combat VFX showcase unknown selection recovers to Fireball");
            AssertEqual(-1, CombatVfxShowcaseRules.IndexFor("unknown"), "unknown powers are absent from the combat VFX showcase");
            AssertEqual(false, CombatVfxShowcaseRules.IsSupported("unknown"), "unknown powers are not showcase-supported");
            AssertEqual(false, CombatVfxShowcaseRules.TryGet("unknown", out CombatVfxShowcaseEntry missing), "unknown showcase lookup fails explicitly");
            AssertEqual(false, missing.Supported, "failed showcase lookup returns an unsupported empty entry");
            AssertEqual(true, CombatVfxShowcaseRules.TryGet("ChArGe", out CombatVfxShowcaseEntry charge), "showcase TryGet is case-insensitive");
            AssertEqual("charge", charge.Id, "showcase TryGet returns the canonical ID");
            int unknownSeed = CombatVfxShowcaseRules.StableSeedFor("unknown beta power");
            AssertEqual(true, unknownSeed > 0, "unknown beta showcase IDs still receive a safe positive seed");
            AssertEqual(unknownSeed, CombatVfxShowcaseRules.StableSeedFor("UNKNOWN BETA POWER"), "fallback showcase seeds are deterministic and case-insensitive");
        }

        private static void CombatPowerSfxProfilesStayDistinctAndMixSafe()
        {
            AssertEqual(16, CombatPowerSfxRules.MendFormulaProfileCount, "Mend formula SFX profile count");
            AssertEqual(17, CombatPowerSfxRules.MageFormulaProfileCount, "mage formula SFX profile count");
            AssertEqual(22, CombatPowerSfxRules.WarlockFormulaProfileCount, "warlock formula SFX profile count");
            AssertEqual(1, CombatPowerSfxRules.CrossSchoolFormulaProfileCount, "cross-school formula SFX profile count");
            AssertEqual(56, CombatPowerSfxRules.FormulaProfileCount, "canonical formula SFX profile total");
            AssertEqual(
                CombatPowerSfxRules.FormulaProfileCount,
                CombatPowerSfxRules.MendFormulaProfileCount + CombatPowerSfxRules.MageFormulaProfileCount + CombatPowerSfxRules.WarlockFormulaProfileCount + CombatPowerSfxRules.CrossSchoolFormulaProfileCount,
                "formula SFX school profile counts reconcile");
            AssertEqual(25, CombatPowerSfxRules.AbilityProfileCount, "ability SFX profile count");

            string[] formulaCodes =
            {
                "GBH", "GBX", "HLC", "OIC", "NVC", "SRF", "TBQ", "SGW", "TNC", "LBC", "TBG", "OBL", "LNH", "SWR", "SBN", "DWP",
                "FIF", "RIG", "WBI", "WBF", "RCL", "BTF", "FBL", "RDF", "RSG", "FRB", "RBI", "RLF", "CLT", "MTR", "CNS", "VST", "AST",
                "RKW", "RNH", "WBK", "NVL", "RMS", "INH", "RMB", "WBP", "GRH", "RPX", "DMC", "WTR", "DSM", "RLM", "RBT", "IBD", "SLV", "PBR", "IBF", "VRS", "IBG", "DFA",
                "ACR"
            };
            AssertEqual(56, formulaCodes.Length, "canonical formula SFX test catalog count");
            AssertEqual(56, formulaCodes.Distinct(StringComparer.Ordinal).Count(), "canonical formula SFX test catalog is unique");
            AssertEqual(FormulaCatalog.All.Length, formulaCodes.Length, "formula SFX test catalog covers every FormulaCatalog entry");
            AssertEqual(true, formulaCodes.All(code => FormulaCatalog.All.Any(formula => formula.Code == code)), "every canonical SFX formula belongs to FormulaCatalog");
            AssertEqual(true, formulaCodes.All(CombatPowerSfxRules.IsSupportedFormula), "all fifty-six canonical formulas have authored SFX profiles");
            AssertEqual(true, FormulaCatalog.All.All(formula => CombatPowerSfxRules.IsSupportedFormula(formula.Name)), "every formula full name resolves to its authored SFX profile");

            List<CombatPowerSfxProfile> formulaProfiles = new List<CombatPowerSfxProfile>();
            foreach (string formulaCode in formulaCodes)
            {
                CombatPowerSfxProfile profile = CombatPowerSfxRules.ProfileForFormula(formulaCode);
                CombatPowerSfxPlan plan = CombatPowerSfxRules.PlanForFormula(formulaCode);
                CombatPowerSfxPlan repeated = CombatPowerSfxRules.PlanForFormula(formulaCode.ToLowerInvariant());
                formulaProfiles.Add(profile);
                AssertEqual(formulaCode.ToLowerInvariant(), profile.Key, formulaCode + " formula SFX profile keeps its canonical key");
                AssertEqual(profile.Key, plan.ProfileKey, formulaCode + " formula SFX plan keeps its profile key");
                AssertEqual(profile.Intensity, plan.Intensity, formulaCode + " formula SFX plan keeps its authored intensity");
                AssertEqual(true, plan.Impact.Enabled, formulaCode + " formula SFX plan has a semantic impact cue");
                AssertEqual(true, plan.CoreCueCount >= 3 && plan.CoreCueCount <= 4, formulaCode + " formula SFX core cue count stays staged and bounded");
                AssertEqual(true, plan.CueCount >= plan.CoreCueCount && plan.CueCount <= 7, formulaCode + " formula SFX total cue count stays bounded");
                AssertCombatPowerSfxPlansEquivalentAndBounded(plan, repeated, formulaCode + " formula SFX plan");

                int audioHash = CombatPowerSfxRules.StableAudioHash(formulaCode, 5, 2);
                AssertEqual(audioHash, CombatPowerSfxRules.StableAudioHash(formulaCode.ToLowerInvariant(), 5, 2), formulaCode + " formula audio hash is case-insensitive and deterministic");
                float sample = CombatPowerSfxRules.StableAudioSample(formulaCode, 5, 2);
                float signed = CombatPowerSfxRules.StableAudioSignedSample(formulaCode, 5, 2);
                AssertEqual(sample, CombatPowerSfxRules.StableSfxSample(formulaCode, 5, 2), formulaCode + " stable SFX sample aliases stable audio sampling");
                AssertEqual(true, sample >= 0f && sample < 1f, formulaCode + " stable audio sample stays normalized");
                AssertEqual(true, signed >= -1f && signed < 1f, formulaCode + " signed audio sample stays normalized");

                CombatPowerSfxPlan reduced = CombatPowerSfxRules.PlanForFormula(formulaCode, 0, true);
                AssertReducedCombatPowerSfxPlan(reduced, formulaCode + " Reduced Audio formula plan");
            }
            AssertEqual(56, formulaProfiles.Select(profile => profile.Key).Distinct().Count(), "all canonical formula SFX profiles remain distinct");

            string[] abilityIds = new[] { "warrior", "rogue", "ranger", "demon" }
                .SelectMany(AbilityCatalog.IdsForClass)
                .ToArray();
            AssertEqual(
                "charge|rally|shieldbash|execute|cleave|whirlwind|sunder|stealth|ambush|throwknife|smokebomb|hamstring|eviscerate|shadowstep|aimedshot|pinningshot|scoutmark|volley|broadheadshot|disruptingshot|quickshot|riftpounce|abyssalwhirl|soulrend|dreadroar",
                string.Join("|", abilityIds),
                "AbilityCatalog exposes the exact twenty-five SFX power IDs");
            AssertEqual(CombatPowerSfxRules.AbilityProfileCount, abilityIds.Length, "AbilityCatalog and ability SFX profile counts agree");
            AssertEqual(abilityIds.Length, abilityIds.Distinct(StringComparer.Ordinal).Count(), "AbilityCatalog SFX IDs are unique");
            AssertEqual(true, abilityIds.All(id => AbilityCatalog.For(id) != null), "every ability SFX ID resolves through AbilityCatalog");
            AssertEqual(true, abilityIds.All(CombatPowerSfxRules.IsSupportedAbility), "all twenty-five AbilityCatalog powers have authored SFX profiles");

            List<CombatPowerSfxProfile> abilityProfiles = new List<CombatPowerSfxProfile>();
            foreach (string abilityId in abilityIds)
            {
                CombatPowerSfxProfile profile = CombatPowerSfxRules.ProfileForAbility(abilityId);
                CombatPowerSfxPlan plan = CombatPowerSfxRules.PlanForAbility(abilityId);
                CombatPowerSfxPlan repeated = CombatPowerSfxRules.PlanForAbility(abilityId.ToUpperInvariant());
                abilityProfiles.Add(profile);
                AssertEqual(abilityId, profile.Key, abilityId + " ability SFX profile keeps its canonical key");
                AssertEqual(profile.Key, plan.ProfileKey, abilityId + " ability SFX plan keeps its profile key");
                AssertEqual(profile.Intensity, plan.Intensity, abilityId + " ability SFX plan keeps its authored intensity");
                AssertEqual(true, plan.Impact.Enabled, abilityId + " ability SFX plan has a semantic impact cue");
                AssertEqual(true, plan.CoreCueCount >= 2 && plan.CoreCueCount <= 4, abilityId + " ability SFX core cue count stays staged and bounded");
                AssertEqual(true, plan.CueCount >= plan.CoreCueCount && plan.CueCount <= 7, abilityId + " ability SFX total cue count stays bounded");
                AssertCombatPowerSfxPlansEquivalentAndBounded(plan, repeated, abilityId + " ability SFX plan");

                int audioHash = CombatPowerSfxRules.StableAudioHash(abilityId, 5, 2);
                AssertEqual(audioHash, CombatPowerSfxRules.StableSfxHash(abilityId.ToUpperInvariant(), 5, 2), abilityId + " ability audio hash aliases stable SFX hashing");
                float sample = CombatPowerSfxRules.StableAudioSample(abilityId, 5, 2);
                AssertEqual(true, sample >= 0f && sample < 1f, abilityId + " stable audio sample stays normalized");

                CombatPowerSfxPlan reduced = CombatPowerSfxRules.PlanForAbility(abilityId, 0, true);
                AssertReducedCombatPowerSfxPlan(reduced, abilityId + " Reduced Audio ability plan");
            }
            AssertEqual(25, abilityProfiles.Select(profile => profile.Key).Distinct().Count(), "all AbilityCatalog SFX profiles remain distinct");

            CombatPowerSfxPlan canonicalFireball = CombatPowerSfxRules.PlanForFormula("FBL", 3);
            CombatPowerSfxPlan namedFireball = CombatPowerSfxRules.PlanForFormula("Fireball", 3);
            AssertCombatPowerSfxPlansEquivalentAndBounded(canonicalFireball, namedFireball, "Fireball formula aliases");

            CombatPowerSfxProfile natureSupport = CombatPowerSfxRules.ProfileForFormula("GBH");
            AssertEqual("castnature", natureSupport.Cast.Key, "Tree Cover begins with the living-nature cast family");
            AssertEqual("tree", natureSupport.Impact.Key, "Tree Cover lands with its authored growth impact");
            AssertEqual("ward", natureSupport.Aftershock.Key, "Tree Cover settles into a protective ward resonance");
            AssertCombatPowerSfxPlansEquivalentAndBounded(CombatPowerSfxRules.PlanForFormula("GBH"), CombatPowerSfxRules.PlanForFormula("Tree Cover"), "Tree Cover formula aliases");

            CombatPowerSfxProfile healingSupport = CombatPowerSfxRules.ProfileForFormula("OIC");
            AssertEqual("castmend", healingSupport.Cast.Key, "Heal begins with the restorative Mend cast family");
            AssertEqual("heal", healingSupport.Impact.Key, "Heal lands with its authored restorative impact");
            AssertCombatPowerSfxPlansEquivalentAndBounded(CombatPowerSfxRules.PlanForFormula("OIC"), CombatPowerSfxRules.PlanForFormula("Heal"), "Heal formula aliases");

            CombatPowerSfxProfile wardSupport = CombatPowerSfxRules.ProfileForFormula("TBQ");
            AssertEqual("ward", wardSupport.Impact.Key, "Ward lands with its authored shield resonance");
            AssertEqual("fieldholy", wardSupport.Aftershock.Key, "Ward leaves a holy protective tail");
            AssertCombatPowerSfxPlansEquivalentAndBounded(CombatPowerSfxRules.PlanForFormula("TBQ"), CombatPowerSfxRules.PlanForFormula("Ward"), "Ward formula aliases");

            CombatPowerSfxProfile sunSupport = CombatPowerSfxRules.ProfileForFormula("SBN");
            AssertEqual("castlight", sunSupport.Cast.Key, "Sun Brand begins with the high-radiance cast family");
            AssertEqual("light", sunSupport.Impact.Key, "Sun Brand lands with its authored light impact");
            AssertEqual("fieldholy", sunSupport.Aftershock.Key, "Sun Brand leaves a broad holy aftershock");
            AssertCombatPowerSfxPlansEquivalentAndBounded(CombatPowerSfxRules.PlanForFormula("SBN"), CombatPowerSfxRules.PlanForFormula("Sun Brand"), "Sun Brand formula aliases");

            CombatPowerSfxProfile cleanseSupport = CombatPowerSfxRules.ProfileForFormula("NVC");
            AssertEqual("status", cleanseSupport.Aftershock.Key, "Cleanse resolves with its condition-clearing status tail");
            AssertCombatPowerSfxPlansEquivalentAndBounded(CombatPowerSfxRules.PlanForFormula("NVC"), CombatPowerSfxRules.PlanForFormula("Purify"), "Cleanse formula aliases");

            CombatPowerSfxProfile sealSupport = CombatPowerSfxRules.ProfileForFormula("SRF");
            AssertEqual("castseal", sealSupport.Cast.Key, "Rift Seal begins with its dedicated seal invocation");
            AssertEqual("riftseal", sealSupport.Impact.Key, "Rift Seal lands with its dedicated closure impact");
            AssertCombatPowerSfxPlansEquivalentAndBounded(CombatPowerSfxRules.PlanForFormula("SRF"), CombatPowerSfxRules.PlanForFormula("Seal Rift"), "Rift Seal formula aliases");

            CombatPowerSfxPlan canonicalWhirlwind = CombatPowerSfxRules.PlanForAbility("whirlwind", 3);
            CombatPowerSfxPlan shortWhirlwind = CombatPowerSfxRules.PlanForAbility("WW", 3);
            AssertCombatPowerSfxPlansEquivalentAndBounded(canonicalWhirlwind, shortWhirlwind, "Whirlwind ability aliases");

            CombatPowerSfxPlan fullVolume = CombatPowerSfxRules.PlanForFormula("FBL", 3, false, false, 100);
            CombatPowerSfxPlan halfVolume = CombatPowerSfxRules.PlanForFormula("FBL", 3, false, false, 50);
            CombatPowerSfxCuePlan[] fullCues = { fullVolume.Cast, fullVolume.Release, fullVolume.Impact, fullVolume.Aftershock, fullVolume.LowHit, fullVolume.Rumble, fullVolume.Shimmer };
            CombatPowerSfxCuePlan[] halfCues = { halfVolume.Cast, halfVolume.Release, halfVolume.Impact, halfVolume.Aftershock, halfVolume.LowHit, halfVolume.Rumble, halfVolume.Shimmer };
            for (int cueIndex = 0; cueIndex < fullCues.Length; cueIndex++)
            {
                AssertEqual(fullCues[cueIndex].Enabled, halfCues[cueIndex].Enabled, "master SFX volume preserves Fireball cue topology " + cueIndex);
                if (!fullCues[cueIndex].Enabled) continue;
                AssertEqual(true, Math.Abs(halfCues[cueIndex].Gain - fullCues[cueIndex].Gain * 0.50f) < 0.0001f, "half master SFX volume halves Fireball cue gain " + cueIndex);
            }
            CombatPowerSfxPlan overVolume = CombatPowerSfxRules.PlanForFormula("FBL", 3, false, false, 150);
            AssertCombatPowerSfxPlansEquivalentAndBounded(fullVolume, overVolume, "master SFX volume clamps at one hundred percent");
            AssertEqual(0, CombatPowerSfxRules.PlanForFormula("FBL", 3, false, false, 0).CueCount, "zero master SFX volume disables every cue");
            AssertEqual(0, CombatPowerSfxRules.PlanForFormula("FBL", 3, false, false, -25).CueCount, "negative master SFX volume clamps to silence");
            AssertEqual(0, CombatPowerSfxRules.PlanForFormula("FBL", 3, false, true, 100).CueCount, "mute disables every formula SFX cue");
            AssertEqual(0, CombatPowerSfxRules.PlanForAbility("whirlwind", 3, false, true, 100).CueCount, "mute disables every ability SFX cue");
            AssertEqual(0, CombatPowerSfxRules.PlanForFormula("FBL", 3, true, true, 100).CueCount, "mute also silences the compact Reduced Audio cue");

            AssertEqual(false, CombatPowerSfxRules.IsSupportedFormula("unknown formula"), "unknown formulas are not marked as authored SFX profiles");
            AssertEqual(false, CombatPowerSfxRules.IsSupportedAbility("unknown skill"), "unknown abilities are not marked as authored SFX profiles");
            CombatPowerSfxProfile fallbackSpell = CombatPowerSfxRules.ProfileForFormula("unknown formula");
            CombatPowerSfxProfile fallbackSkill = CombatPowerSfxRules.ProfileForAbility("unknown skill");
            AssertEqual("spell", fallbackSpell.Key, "unknown formulas use the explicit generic spell SFX profile");
            AssertEqual("skill", fallbackSkill.Key, "unknown abilities use the explicit generic skill SFX profile");
            CombatPowerSfxPlan fallbackSpellPlan = CombatPowerSfxRules.PlanForFormula("unknown formula");
            CombatPowerSfxPlan fallbackSkillPlan = CombatPowerSfxRules.PlanForAbility("unknown skill");
            AssertEqual("spell", fallbackSpellPlan.ProfileKey, "unknown formula SFX plan preserves its fallback identity");
            AssertEqual("spell", fallbackSpellPlan.Impact.Key, "unknown formula SFX plan uses the generic spell impact");
            AssertEqual(3, fallbackSpellPlan.CoreCueCount, "unknown formula fallback remains a compact cast-release-impact sequence");
            AssertEqual(0, fallbackSpellPlan.AccentCueCount, "unknown formula fallback does not invent signature accents");
            AssertEqual("skill", fallbackSkillPlan.ProfileKey, "unknown ability SFX plan preserves its fallback identity");
            AssertEqual("attack", fallbackSkillPlan.Impact.Key, "unknown ability SFX plan uses the generic attack impact");
            AssertEqual(3, fallbackSkillPlan.CoreCueCount, "unknown ability fallback remains a compact cast-release-impact sequence");
            AssertEqual(0, fallbackSkillPlan.AccentCueCount, "unknown ability fallback does not invent signature accents");
            AssertCombatPowerSfxPlansEquivalentAndBounded(fallbackSpellPlan, CombatPowerSfxRules.PlanForFormula("UNKNOWN FORMULA"), "unknown formula fallback");
            AssertCombatPowerSfxPlansEquivalentAndBounded(fallbackSkillPlan, CombatPowerSfxRules.PlanForAbility("UNKNOWN SKILL"), "unknown ability fallback");
            AssertReducedCombatPowerSfxPlan(CombatPowerSfxRules.PlanForFormula("unknown formula", 0, true), "unknown formula Reduced Audio fallback");
            AssertReducedCombatPowerSfxPlan(CombatPowerSfxRules.PlanForAbility("unknown skill", 0, true), "unknown ability Reduced Audio fallback");
        }

        private static void AssertCombatPowerSfxPlansEquivalentAndBounded(
            CombatPowerSfxPlan expected,
            CombatPowerSfxPlan actual,
            string label)
        {
            AssertEqual(expected.ProfileKey, actual.ProfileKey, label + " profile key is deterministic");
            AssertEqual(expected.Intensity, actual.Intensity, label + " intensity is deterministic");
            AssertEqual(expected.ReducedAudio, actual.ReducedAudio, label + " Reduced Audio flag is deterministic");
            AssertEqual(expected.CueCount, actual.CueCount, label + " cue count is deterministic");
            CombatPowerSfxCuePlan[] expectedCues = { expected.Cast, expected.Release, expected.Impact, expected.Aftershock, expected.LowHit, expected.Rumble, expected.Shimmer };
            CombatPowerSfxCuePlan[] actualCues = { actual.Cast, actual.Release, actual.Impact, actual.Aftershock, actual.LowHit, actual.Rumble, actual.Shimmer };
            for (int cueIndex = 0; cueIndex < expectedCues.Length; cueIndex++)
            {
                CombatPowerSfxCuePlan expectedCue = expectedCues[cueIndex];
                CombatPowerSfxCuePlan actualCue = actualCues[cueIndex];
                AssertEqual(expectedCue.Phase, actualCue.Phase, label + " cue phase is deterministic " + cueIndex);
                AssertEqual(expectedCue.Key, actualCue.Key, label + " cue key is deterministic " + cueIndex);
                AssertEqual(expectedCue.Delay, actualCue.Delay, label + " cue delay is deterministic " + cueIndex);
                AssertEqual(expectedCue.Gain, actualCue.Gain, label + " cue gain is deterministic " + cueIndex);
                AssertEqual(expectedCue.Pitch, actualCue.Pitch, label + " cue pitch is deterministic " + cueIndex);
                AssertEqual(expectedCue.Enabled, actualCue.Enabled, label + " cue enablement is deterministic " + cueIndex);
                AssertEqual(true, actualCue.Delay >= 0f && actualCue.Delay <= 0.60f, label + " cue delay stays bounded " + cueIndex);
                AssertEqual(true, actualCue.Gain >= 0f && actualCue.Gain <= 1.40f, label + " cue gain stays bounded " + cueIndex);
                AssertEqual(true, actualCue.Pitch >= 0.90f && actualCue.Pitch <= 1.10f, label + " cue pitch stays bounded " + cueIndex);
                if (!actualCue.Enabled) continue;
                float stablePitch = CombatPowerSfxRules.StablePitch(actualCue, actual.ProfileKey, 7, cueIndex);
                AssertEqual(stablePitch, CombatPowerSfxRules.StablePitch(actualCue, actual.ProfileKey, 7, cueIndex), label + " stable cue pitch repeats exactly " + cueIndex);
                AssertEqual(true, stablePitch >= 0.90f && stablePitch <= 1.10f, label + " stable cue pitch stays bounded " + cueIndex);
            }
        }

        private static void AssertReducedCombatPowerSfxPlan(CombatPowerSfxPlan plan, string label)
        {
            AssertEqual(true, plan.ReducedAudio, label + " is marked Reduced Audio");
            AssertEqual(1, plan.CueCount, label + " collapses to one cue");
            AssertEqual(1, plan.CoreCueCount, label + " keeps one core impact cue");
            AssertEqual(0, plan.AccentCueCount, label + " removes all accent layers");
            AssertEqual(false, plan.Cast.Enabled, label + " removes the cast layer");
            AssertEqual(false, plan.Release.Enabled, label + " removes the release layer");
            AssertEqual(true, plan.Impact.Enabled, label + " preserves one semantic impact");
            AssertEqual(CombatPowerSfxPhase.Impact, plan.Impact.Phase, label + " compact cue is staged as impact");
            AssertEqual(0f, plan.Impact.Delay, label + " compact impact is immediate");
            AssertEqual(true, plan.Impact.Gain > 0f && plan.Impact.Gain <= 0.82f, label + " compact impact respects the Reduced Audio gain cap");
            AssertEqual(true, plan.Impact.Pitch >= 0.90f && plan.Impact.Pitch <= 1.10f, label + " compact impact pitch stays bounded");
            AssertEqual(false, plan.Aftershock.Enabled, label + " removes the aftershock layer");
            AssertEqual(false, plan.UsesLowHit || plan.UsesRumble || plan.UsesShimmer, label + " removes all signature accent layers");
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

            CombatUnitPresentationBeat unbind = CombatUnitPresentationRules.Create(
                "summon",
                CombatUnitPresentationBeatKind.Unbind,
                5f,
                -1f);
            AssertEqual(true, CombatUnitPresentationRules.ShouldRenderActor(false, unbind, 4.9f), "dead summon remains visible until its staged unbinding impact");
            AssertEqual(false, CombatUnitPresentationRules.ShouldRenderTacticalOverlay(false, unbind, 4.9f), "unbound summon immediately loses tactical HP and status chrome");
            CombatUnitPresentationPose unbindPose = CombatUnitPresentationRules.PoseFor(unbind, 5.14f, false);
            AssertEqual(true, unbindPose.OffsetX < 0f && unbindPose.OffsetY < 0f, "unbinding summon twists and lifts toward its dismissal direction");
            AssertEqual(true, unbindPose.Scale < 1f && unbindPose.Alpha < 1f, "unbinding summon visibly shrinks and fades");
            AssertEqual(false, CombatUnitPresentationRules.ShouldRenderActor(false, unbind, unbind.Until + 0.01f), "unbound summon clears after its bounded dismissal beat");

            List<CombatUnitPresentationBeat> replacementBeats = new List<CombatUnitPresentationBeat>();
            CombatUnitPresentationRules.AddBounded(
                replacementBeats,
                CombatUnitPresentationRules.Create("summon", CombatUnitPresentationBeatKind.Reveal, 5f),
                0f);
            CombatUnitPresentationRules.AddBounded(
                replacementBeats,
                CombatUnitPresentationRules.Create("summon", CombatUnitPresentationBeatKind.Hit, 5.1f),
                0f);
            CombatUnitPresentationRules.AddBounded(
                replacementBeats,
                CombatUnitPresentationRules.Create("other-unit", CombatUnitPresentationBeatKind.Hit, 5.1f),
                0f);
            CombatUnitPresentationRules.AddBounded(replacementBeats, unbind, 0f);
            AssertEqual(1, replacementBeats.Count(beat => beat.UnitId == "summon" && beat.Kind == CombatUnitPresentationBeatKind.Unbind), "unbinding inserts one bounded dismissal beat");
            AssertEqual(1, replacementBeats.Count(beat => beat.UnitId == "other-unit"), "unbinding preserves unrelated combatant presentation beats");
            CombatUnitPresentationBeat replacementUnbind = CombatUnitPresentationRules.Create(
                "summon",
                CombatUnitPresentationBeatKind.Unbind,
                5.2f,
                1f);
            CombatUnitPresentationRules.AddBounded(replacementBeats, replacementUnbind, 0f);
            AssertEqual(1, replacementBeats.Count(beat => beat.UnitId == "summon"), "a repeated unbinding replaces the summon beat instead of accumulating duplicates");
            AssertEqual(true, CombatUnitPresentationRules.TryGetBeat(replacementBeats, "summon", 5.1f, out CombatUnitPresentationBeat selectedUnbind), "the replacement unbinding remains queryable before impact");
            AssertEqual(replacementUnbind, selectedUnbind, "the newest unbinding owns presentation priority for its summon");

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
                    MusicDirectorRules.GreenShrineTrainingRing,
                    "guard"),
                new WorldSitePresentationProfile(
                    WorldSitePresentationRules.OldQuarryForge,
                    "old-quarry",
                    ObjectType.ForgeSite,
                    "ambforge",
                    "ambstone",
                    MusicDirectorRules.OldQuarryForge,
                    "servicearmor"),
                new WorldSitePresentationProfile(
                    WorldSitePresentationRules.GloamDeepCrypt,
                    "gloam-courts",
                    ObjectType.DeepCrypt,
                    "ambruin",
                    "ambcave",
                    MusicDirectorRules.GloamDeepCrypt,
                    "door"),
                new WorldSitePresentationProfile(
                    WorldSitePresentationRules.GlassLoreLibrary,
                    "glass-warrens",
                    ObjectType.LoreLibrary,
                    "ambglass",
                    "ambruin",
                    MusicDirectorRules.GlassLoreLibrary,
                    "formula"),
                new WorldSitePresentationProfile(
                    WorldSitePresentationRules.DuskMarketHideout,
                    "dusk-market",
                    ObjectType.FactionCamp,
                    "ambdrum",
                    "ambcamp",
                    MusicDirectorRules.DuskMarketHideout,
                    "ambush"),
                new WorldSitePresentationProfile(
                    WorldSitePresentationRules.RedGateSeal,
                    "red-gate",
                    ObjectType.PortalSeal,
                    "ambgate",
                    "ambglass",
                    MusicDirectorRules.RedGateSeal,
                    "riftseal"),
                new WorldSitePresentationProfile(
                    WorldSitePresentationRules.SaltCisternGate,
                    "salt-cisterns",
                    ObjectType.DungeonGate,
                    "ambdrip",
                    "ambcave",
                    MusicDirectorRules.SaltCisternGate,
                    "gateopen"),
                new WorldSitePresentationProfile(
                    WorldSitePresentationRules.AshFenAncientGrove,
                    "ash-fen",
                    ObjectType.AncientGrove,
                    "ambgrove",
                    "ambfen",
                    MusicDirectorRules.AshFenAncientGrove,
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
            AssertEqual(MusicDirectorRules.Title, MusicDirectorRules.Tavern, "title mode retains its save-stable Tavern route key");
            AssertEqual(MusicDirectorRules.GrandHearth, MusicDirectorRules.ExploreTrackKey("midgaard-grand-hearth", ObjectType.Tavern, true, false), "Grand Hearth receives the quieter title-theme reprise");
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

            MusicCrossfadeGains crossfadeStart = MusicTransitionRules.EqualPowerCrossfade(0f);
            MusicCrossfadeGains crossfadeMid = MusicTransitionRules.EqualPowerCrossfade(0.5f);
            MusicCrossfadeGains crossfadeEnd = MusicTransitionRules.EqualPowerCrossfade(1f);
            AssertEqual(true, Mathf.Abs(crossfadeStart.Outgoing - 1f) < 0.0001f && crossfadeStart.Incoming < 0.0001f, "equal-power music fade starts entirely on the outgoing score");
            AssertEqual(true, Mathf.Abs(crossfadeMid.Outgoing - 0.7071068f) < 0.0001f && Mathf.Abs(crossfadeMid.Incoming - 0.7071068f) < 0.0001f, "equal-power music fade keeps both scores audible at midpoint");
            AssertEqual(true, crossfadeEnd.Outgoing < 0.0001f && Mathf.Abs(crossfadeEnd.Incoming - 1f) < 0.0001f, "equal-power music fade ends entirely on the incoming score");
            AssertEqual(true, Mathf.Abs(crossfadeMid.Outgoing * crossfadeMid.Outgoing + crossfadeMid.Incoming * crossfadeMid.Incoming - 1f) < 0.0001f, "equal-power music fade avoids the old midpoint energy dip");
            AssertEqual(true, MusicTransitionRules.TransitionDurationFor(MusicTransitionContext.Combat) < MusicTransitionRules.TransitionDurationFor(MusicTransitionContext.Explore), "combat score enters faster than calm exploration music");
            AssertEqual(true, MusicTransitionRules.TransitionDurationFor(MusicTransitionContext.Explore) < MusicTransitionRules.TransitionDurationFor(MusicTransitionContext.WorldMapExplore), "World Map score receives a more spacious transition");
            AssertEqual(true, MusicTransitionRules.IntroFadeDurationFor(MusicTransitionContext.Title) > MusicTransitionRules.IntroFadeDurationFor(MusicTransitionContext.Combat), "title overture receives a gentler opening than combat");
            AssertEqual(true, MusicTransitionRules.ShouldKeepTransportAlive(true, true, false, true), "muting music preserves its transport and playhead");
            AssertEqual(false, MusicTransitionRules.ShouldKeepTransportAlive(true, true, true, false), "startup splash still suspends music transport");
            AssertEqual(false, MusicTransitionRules.ShouldKeepTransportAlive(true, false, false, false), "missing score clips do not keep an empty transport alive");

            ExplorationMusicSwitchDecision initialRoute = MusicTransitionRules.EvaluateExplorationSwitch("", MusicDirectorRules.MidgaardRoad, 0f, 0f, 0f, false);
            ExplorationMusicSwitchDecision calmCandidateHolding = MusicTransitionRules.EvaluateExplorationSwitch(MusicDirectorRules.MidgaardRoad, MusicDirectorRules.MidgaardTemple, 1.49f, 9f, 0f, false);
            ExplorationMusicSwitchDecision calmDwellHolding = MusicTransitionRules.EvaluateExplorationSwitch(MusicDirectorRules.MidgaardRoad, MusicDirectorRules.MidgaardTemple, 1.50f, 7.99f, 0f, false);
            ExplorationMusicSwitchDecision calmStable = MusicTransitionRules.EvaluateExplorationSwitch(MusicDirectorRules.MidgaardRoad, MusicDirectorRules.MidgaardTemple, 1.50f, 8f, 0f, false);
            ExplorationMusicSwitchDecision pursuitEntered = MusicTransitionRules.EvaluateExplorationSwitch(MusicDirectorRules.MidgaardRoad, MusicDirectorRules.HuntedRoad, 0f, 0f, 0f, false);
            ExplorationMusicSwitchDecision pursuitMapHold = MusicTransitionRules.EvaluateExplorationSwitch(MusicDirectorRules.HuntedRoad, MusicDirectorRules.WorldMapOverview, 2.99f, 10f, 2.99f, true);
            ExplorationMusicSwitchDecision pursuitReleased = MusicTransitionRules.EvaluateExplorationSwitch(MusicDirectorRules.HuntedRoad, MusicDirectorRules.WorldMapOverview, 3f, 10f, 3f, true);
            ExplorationMusicSwitchDecision explicitOverview = MusicTransitionRules.EvaluateExplorationSwitch(MusicDirectorRules.MidgaardRoad, MusicDirectorRules.WorldMapOverview, 0f, 0f, 0f, true);
            AssertEqual(true, initialRoute.ShouldSwitch && initialRoute.Reason == ExplorationMusicSwitchReason.InitialRoute, "exploration music establishes its first route immediately");
            AssertEqual(false, calmCandidateHolding.ShouldSwitch, "calm landmark music waits for a stable candidate");
            AssertEqual(false, calmDwellHolding.ShouldSwitch, "calm landmark music respects the current route dwell");
            AssertEqual(true, calmStable.ShouldSwitch && calmStable.Reason == ExplorationMusicSwitchReason.CalmRouteStable, "calm landmark music changes only after both stability gates");
            AssertEqual(true, pursuitEntered.ShouldSwitch && pursuitEntered.Reason == ExplorationMusicSwitchReason.PursuitEntered, "nearby pursuit music preempts calm exploration immediately");
            AssertEqual(true, !pursuitMapHold.ShouldSwitch && pursuitMapHold.Reason == ExplorationMusicSwitchReason.PursuitReleaseHolding, "opening the World Map cannot prematurely clear pursuit music");
            AssertEqual(true, pursuitReleased.ShouldSwitch && pursuitReleased.Reason == ExplorationMusicSwitchReason.PursuitReleased, "pursuit music releases after a sustained calm candidate");
            AssertEqual(true, explicitOverview.ShouldSwitch && explicitOverview.Reason == ExplorationMusicSwitchReason.ExplicitWorldMapViewChange, "explicit World Map changes switch calm routes immediately");
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
            AssertEqual(25, abilityProfiles.Count, "all martial and demon-form abilities enter the shared impact pipeline");
            AssertEqual(true, abilityProfiles.All(profile => profile.CastSfx != "ui" && !string.IsNullOrWhiteSpace(profile.ImpactSfx)), "every martial ability has intentional cast and impact audio");
            AssertEqual(25, abilityProfiles.Select(profile => profile.CastSfx + "|" + profile.ImpactSfx + "|" + profile.AftershockSfx).Distinct().Count(), "martial and demon-form abilities keep distinct audiovisual signatures");

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
            AssertEqual(false, CombatAudioMixRules.ShouldReplaceActiveMusicDuck(10.2f, 10.8f, 0.48f, 0.28f), "a weaker overlapping hit cannot prolong a stronger music duck");
            AssertEqual(true, CombatAudioMixRules.ShouldReplaceActiveMusicDuck(10.2f, 10.8f, 0.28f, 0.48f), "a stronger overlapping hit can claim clearer mix space");
            AssertEqual(true, CombatAudioMixRules.ShouldReplaceActiveMusicDuck(10.9f, 10.8f, 0.48f, 0.28f), "an expired music duck accepts the next readable impact");
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
            AssertEqual(16, CombatAudioMixRules.ScheduledSfxCapacity, "scheduled combat audio owns a bounded sixteen-cue queue");
            AssertEqual(
                true,
                CombatAudioMixRules.ShouldCoalesceScheduledCue("shock", 2f, 0.10f, CombatAudioMixRules.ScheduledSfxPrioritySupporting, "SHOCK", 2.02f, 0.20f, CombatAudioMixRules.ScheduledSfxPrioritySupporting),
                "near-identical supporting cues coalesce before crowding the mix");
            AssertEqual(
                false,
                CombatAudioMixRules.ShouldCoalesceScheduledCue("impact", 2f, 0f, CombatAudioMixRules.ScheduledSfxPriorityPrimaryImpact, "impact", 2.01f, 0f, CombatAudioMixRules.ScheduledSfxPriorityPrimaryImpact),
                "primary impacts never disappear into coalescing");
            AssertEqual(
                false,
                CombatAudioMixRules.ShouldCoalesceScheduledCue("shock", 2f, -0.50f, CombatAudioMixRules.ScheduledSfxPrioritySupporting, "shock", 2.02f, 0.50f, CombatAudioMixRules.ScheduledSfxPrioritySupporting),
                "spatially separate impacts remain audible");

            CombatImpactProfile tempest = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "AST"));
            CombatImpactProfile chainLightning = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "CLT"));
            CombatImpactProfile thunderclap = CombatImpactRules.ForFormula(FormulaCatalog.All.First(formula => formula.Code == "RSG"));
            AssertEqual(3, CombatAudioMixRules.SecondaryImpactBeatCount(tempest, 0), "Arcane Tempest receives three bounded secondary thunder beats");
            AssertEqual(2, CombatAudioMixRules.SecondaryImpactBeatCount(meteor, 0), "Meteor receives two bounded secondary crash beats");
            AssertEqual(2, CombatAudioMixRules.SecondaryImpactBeatCount(chainLightning, 0), "Chain Lightning receives two bounded secondary shock beats");
            AssertEqual(2, CombatAudioMixRules.SecondaryImpactBeatCount(thunderclap, 0), "Thunderclap receives a short rolling shock tail");
            AssertEqual(1, CombatAudioMixRules.SecondaryImpactBeatCount(fireball, 1), "an epic terrain reaction receives one supporting impact echo");
            AssertEqual(true, CombatAudioMixRules.SecondaryImpactDelay(tempest, 0) < CombatAudioMixRules.SecondaryImpactDelay(tempest, 2), "secondary impact beats remain ordered");
            AssertEqual(true, CombatAudioMixRules.SecondaryImpactDelay(tempest, 2) <= 0.60f, "secondary impact beats finish inside the combat mix window");
            AssertEqual(true, CombatAudioMixRules.SecondaryImpactVolume(tempest, 2) < CombatAudioMixRules.SecondaryImpactVolume(tempest, 0), "secondary impacts taper beneath the primary hit");
            AssertEqual(true, CombatAudioMixRules.SecondaryImpactVolume(tempest, 0) <= 0.40f, "secondary spell beats retain summed-mix headroom");
            AssertEqual("resonance", CombatAudioMixRules.SecondaryImpactCue(tempest, 0), "Arcane Tempest echoes with a compact resonance instead of repeating its long master");
            AssertEqual("impactlow", CombatAudioMixRules.SecondaryImpactCue(tempest, 1), "Arcane Tempest alternates a restrained low thunder beat");
            AssertEqual("impactlow", CombatAudioMixRules.SecondaryImpactCue(meteor, 0), "Meteor secondary crashes reinforce the body without replaying the long primary master");
            AssertEqual(true, Math.Abs(CombatAudioMixRules.SecondaryImpactPan(0f, 0) - CombatAudioMixRules.SecondaryImpactPan(0f, 1)) > 0.20f, "secondary impacts spread across the battlefield mix");
        }

        private static void CombatAudioCuesAndAmbienceStaySemanticAndMixSafe()
        {
            AssertEqual(CombatAudioMixRules.StepCue, CombatAudioMixRules.DirectCue("move", 1f).Key, "combat movement uses a grounded step cue");
            AssertEqual(CombatAudioMixRules.GuardCue, CombatAudioMixRules.DirectCue("guard", 1f).Key, "Guard uses a dedicated brace cue");
            AssertEqual(CombatAudioMixRules.TurnCue, CombatAudioMixRules.DirectCue("turn", 1f).Key, "turn handoff uses a compact combat cue");
            AssertEqual(CombatAudioMixRules.CriticalCue, CombatAudioMixRules.DirectCue("crit", 1f).Key, "critical hits use a dedicated forged accent");
            AssertEqual("victory", CombatAudioMixRules.DirectCue("victory", 0.63f).Key, "non-combat semantic cues remain untouched");
            AssertEqual(true, CombatAudioMixRules.DirectCue("move", 1f).Volume < CombatAudioMixRules.DirectCue("crit", 1f).Volume, "critical accents sit above ordinary footwork");

            CombatAmbienceProfile sewer = CombatAudioMixRules.Ambience(MusicDirectorRules.CombatSewer, true, 0);
            CombatAmbienceProfile ratfolk = CombatAudioMixRules.Ambience(MusicDirectorRules.CombatRatfolk, true, 0);
            CombatAmbienceProfile kobold = CombatAudioMixRules.Ambience(MusicDirectorRules.CombatKobold, true, 0);
            CombatAmbienceProfile arcane = CombatAudioMixRules.Ambience(MusicDirectorRules.CombatArcaneDuel, true, 0);
            CombatAmbienceProfile mutedMusic = CombatAudioMixRules.Ambience(MusicDirectorRules.CombatGeneric, false, 0);
            AssertEqual(CombatAudioMixRules.SewerAmbienceCue, sewer.Key, "sewer fights retain wet enclosed ambience");
            AssertEqual(CombatAudioMixRules.SewerAmbienceCue, ratfolk.Key, "ratfolk sewer battles inherit the sewer room tone");
            AssertEqual(CombatAudioMixRules.SteelAmbienceCue, kobold.Key, "kobold fights do not inherit unrelated sewer water");
            AssertEqual(CombatAudioMixRules.ArcaneAmbienceCue, arcane.Key, "arcane duels receive a restrained unstable field");
            AssertEqual(true, sewer.Volume <= 0.10f && mutedMusic.Volume > sewer.Volume, "combat ambience stays behind music but remains present when music is muted");
            AssertEqual(true, CombatAudioMixRules.InitialAmbienceDelay(true) >= 5f, "foreground combat receives a clean opening window");
            AssertEqual(true, CombatAudioMixRules.AmbienceInterval(true, 0) >= 12f, "combat ambience remains sparse beneath the score");
            AssertEqual(true, CombatAudioMixRules.AmbienceInterval(false, 0) < CombatAudioMixRules.AmbienceInterval(true, 0), "music mute allows a slightly fuller environmental bed");
            AssertEqual(true, CombatAudioMixRules.CombatAmbienceForegroundQuietWindow >= 1.5f, "foreground attacks suppress ambience through their audible tail");
            AssertEqual(true, CombatAudioMixRules.IsCombatAmbienceCue(arcane.Key), "combat ambience keys are recognized for foreground suppression");
            AssertEqual(false, CombatAudioMixRules.IsCombatAmbienceCue("combatcrit"), "critical feedback remains a foreground cue");
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
            HelpOverlayView retailTavern = HelpOverlayContent.Build(GameMode.Tavern, false, 6, "Midgaard");
            HelpOverlayView explore = HelpOverlayContent.Build(GameMode.Explore, false, 6, "Midgaard");
            HelpOverlayView combat = HelpOverlayContent.Build(GameMode.Combat, false, 6, "Midgaard");
            HelpOverlayView muster = HelpOverlayContent.Build(GameMode.Muster, false, 6, "Midgaard");
            HelpOverlayView victory = HelpOverlayContent.Build(GameMode.Victory, false, 6, "Midgaard");
            HelpOverlayView defeat = HelpOverlayContent.Build(GameMode.Defeat, false, 6, "Midgaard");

            AssertEqual(true, tavern.Title.Contains("Tavern"), "tavern help title");
            AssertEqual(true, tavern.Lines.Any(line => line.Contains("Beta Lab")), "developer tavern help names the direct Beta Lab row");
            AssertEqual(true, tavern.Lines.Any(line => line.Contains("broader combat, martial, and route testing panel")), "developer tavern help retains the broader testing-panel route");
            AssertEqual(false, retailTavern.Lines.Any(line => line.Contains("Beta Lab") || line.Contains("testing panel")), "retail tavern help does not advertise hidden developer routes");
            AssertEqual(true, explore.Lines.Any(line => line.Contains("Space / E")), "exploration help mentions contextual use");
            AssertEqual(true, explore.Lines.Any(line => line.Contains("Growth tab")), "exploration help points earned progression to I > Growth");
            AssertEqual(true, explore.Lines.Any(line => line.IndexOf("east and west gates", StringComparison.OrdinalIgnoreCase) >= 0), "exploration help mentions pass-through gates");
            AssertEqual(
                true,
                explore.Lines.Any(line =>
                    line.IndexOf("patrons", StringComparison.OrdinalIgnoreCase) >= 0
                    && line.IndexOf("Town Hall's Grand Hearth", StringComparison.OrdinalIgnoreCase) >= 0
                    && line.IndexOf("storm doors", StringComparison.OrdinalIgnoreCase) >= 0
                    && line.IndexOf("begin the journey", StringComparison.OrdinalIgnoreCase) >= 0),
                "exploration help explains the Town Hall gathering and required first departure");
            AssertEqual(true, combat.Lines.Any(line => line.Contains("Tree Cover")), "combat help mentions tree cover");
            AssertEqual(true, combat.Lines.Any(line => line.Contains("undo this turn's movement")), "combat help explains pre-action movement undo");
            AssertEqual(true, combat.Lines.Any(line => line.Contains("cancels an armed target")), "combat help explains non-destructive target cancellation");
            AssertEqual(true, combat.Lines.Any(line => line.Contains("retreat for one supply")), "combat help explains the retreat safety valve");
            AssertEqual(true, combat.Lines.Any(line => line.Contains("review-only")), "combat help explains that growth spending waits until combat ends");
            AssertEqual(true, muster.Lines.Any(line => line.Contains("50-point")), "muster help mentions stat budget");
            AssertEqual(true, tavern.Lines.Any(line => line.IndexOf("muster", StringComparison.OrdinalIgnoreCase) >= 0)
                && !tavern.Lines.Any(line => line.Contains("Customize Party")), "title help names the current new-company path without a removed choice");
            AssertEqual(
                true,
                tavern.Lines.Any(line => line.IndexOf("Town Hall's Grand Hearth", StringComparison.OrdinalIgnoreCase) >= 0),
                "title help sends the new company to Town Hall's Grand Hearth");
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
                new Vector2Int(960, 600),
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
                new Vector2Int(960, 600),
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
                AssertEqual(5, tabs.Length, $"armory exposes five tabs at {size.x}x{size.y}");
                foreach (Rect tab in tabs)
                {
                    AssertEqual(true, tab.xMin >= 0f && tab.yMin >= 0f && tab.xMax <= geometry.Tabs.width && tab.yMax <= geometry.Tabs.height, $"armory tab fits {size.x}x{size.y}");
                }
                AssertEqual(true, geometry.ListContent.width >= 500f, $"inventory list remains readable at {size.x}x{size.y}");
                AssertEqual(true, geometry.Detail.width >= 320f, $"equipment comparison pane remains readable at {size.x}x{size.y}");
                Rect actionArea = ArmoryOverlayLayout.DetailActionsArea(geometry.Detail.width, geometry.Detail.height, false, 5);
                Rect[] actions = ArmoryOverlayLayout.DetailActionRects(actionArea.width, actionArea.height, 5);
                AssertEqual(5, actions.Length, $"four-person inventory picker and Back action are laid out at {size.x}x{size.y}");
                foreach (Rect action in actions)
                {
                    AssertEqual(true, action.height >= 40f, $"inventory target action remains at least 40px tall at {size.x}x{size.y}");
                    AssertEqual(true, action.yMin >= 0f && action.yMax <= actionArea.height + 0.5f, $"inventory target action stays inside the detail pane at {size.x}x{size.y}");
                }
            }
        }

        private static void PartyGrowthRulesStageAndApplyCampaignPointsSafely()
        {
            var expectedTalents = new Dictionary<string, PartyGrowthChoice[]>
            {
                { "warrior", new[] { PartyGrowthChoice.Arms, PartyGrowthChoice.Guard } },
                { "rogue", new[] { PartyGrowthChoice.Arms, PartyGrowthChoice.Missile } },
                { "ranger", new[] { PartyGrowthChoice.Missile, PartyGrowthChoice.Arms } },
                { "wizard", new[] { PartyGrowthChoice.Ember, PartyGrowthChoice.Hex } },
                { "mage", new[] { PartyGrowthChoice.Ember } },
                { "warlock", new[] { PartyGrowthChoice.Hex, PartyGrowthChoice.Arms } },
                { "priest", new[] { PartyGrowthChoice.Mend, PartyGrowthChoice.Guard } },
                { "paladin", new[] { PartyGrowthChoice.Guard, PartyGrowthChoice.Arms, PartyGrowthChoice.Mend } }
            };

            foreach (KeyValuePair<string, PartyGrowthChoice[]> expected in expectedTalents)
            {
                PartyMember member = PartyGrowthTestMember(expected.Key, 0, 0);
                AssertEqual(
                    true,
                    PartyGrowthRules.RelevantTalents(member).SequenceEqual(expected.Value),
                    expected.Key + " growth exposes only class-relevant talents");
            }
            AssertEqual(
                0,
                PartyGrowthRules.RelevantTalents(PartyGrowthTestMember("unknown", 0, 0)).Count,
                "unknown classes expose no talent spending");
            AssertEqual(true, PartyGrowthRules.IsAttribute(PartyGrowthChoice.Strength), "Strength is an attribute growth choice");
            AssertEqual(false, PartyGrowthRules.IsAttribute(PartyGrowthChoice.Arms), "Arms is a talent growth choice");
            AssertEqual("Strength", PartyGrowthRules.Label(PartyGrowthChoice.Strength), "growth choice has a player-facing label");
            AssertEqual("+1 Strength", PartyGrowthRules.Effect(PartyGrowthChoice.Strength), "attribute growth advertises its exact gain");
            AssertEqual("+2 Arms", PartyGrowthRules.Effect(PartyGrowthChoice.Arms), "talent growth advertises its exact gain");

            PartyMember direct = PartyGrowthTestMember("warrior", 2, 2);
            AssertEqual(
                true,
                PartyGrowthRules.TrySpendAttributePoint(direct, PartyGrowthChoice.Strength, out _),
                "one stat point can be spent directly on an attribute");
            AssertEqual(11, direct.Stats.Strength, "one stat point adds exactly one Strength");
            AssertEqual(1, direct.StatPoints, "direct attribute spending consumes exactly one stat point");
            AssertEqual(
                true,
                PartyGrowthRules.TrySpendTalentPoint(direct, PartyGrowthChoice.Arms, out _),
                "one skill point can be spent directly on a relevant talent");
            AssertEqual(12, direct.Skills.Arms, "one skill point adds exactly two Arms");
            AssertEqual(1, direct.SkillPoints, "direct talent spending consumes exactly one skill point");

            PartyMember balanced = PartyGrowthTestMember("warrior", 2, 2);
            PartyGrowthPlan balancedPlan = new PartyGrowthPlan();
            AssertEqual(true, PartyGrowthRules.TryStage(balanced, balancedPlan, PartyGrowthChoice.Strength, out _), "first attribute point stages");
            AssertEqual(true, PartyGrowthRules.TryStage(balanced, balancedPlan, PartyGrowthChoice.Health, out _), "second attribute point stages");
            AssertEqual(true, PartyGrowthRules.TryStage(balanced, balancedPlan, PartyGrowthChoice.Arms, out _), "first talent point stages");
            AssertEqual(true, PartyGrowthRules.TryStage(balanced, balancedPlan, PartyGrowthChoice.Guard, out _), "second talent point stages");
            AssertEqual(2, balancedPlan.SpentStatPoints, "staged plan counts every stat point exactly once");
            AssertEqual(2, balancedPlan.SpentSkillPoints, "staged plan counts every skill point exactly once");
            AssertEqual(true, PartyGrowthRules.Validate(balanced, balancedPlan, out _), "balanced growth plan validates");
            AssertEqual(11, PartyGrowthRules.ProjectedAttribute(balanced, balancedPlan, PartyGrowthChoice.Strength), "attribute preview is exact");
            AssertEqual(12, PartyGrowthRules.ProjectedSkill(balanced, balancedPlan, PartyGrowthChoice.Arms), "talent preview is exact");
            AssertEqual(true, PartyGrowthRules.TryApply(balanced, balancedPlan, out _), "balanced growth plan applies");
            AssertEqual(11, balanced.Stats.Strength, "applied plan adds staged Strength");
            AssertEqual(11, balanced.Stats.Health, "applied plan adds staged Health");
            AssertEqual(12, balanced.Skills.Arms, "applied plan adds staged Arms");
            AssertEqual(12, balanced.Skills.Guard, "applied plan adds staged Guard");
            AssertEqual(0, balanced.StatPoints, "applied plan conserves all stat points");
            AssertEqual(0, balanced.SkillPoints, "applied plan conserves all skill points");

            PartyMember limited = PartyGrowthTestMember("warrior", 1, 1);
            PartyGrowthPlan limitedPlan = new PartyGrowthPlan();
            AssertEqual(true, PartyGrowthRules.TryStage(limited, limitedPlan, PartyGrowthChoice.Strength, out _), "available stat point stages");
            AssertEqual(false, PartyGrowthRules.CanStage(limited, limitedPlan, PartyGrowthChoice.Health, out _), "stat overspend cannot stage");
            AssertEqual(false, PartyGrowthRules.TryStage(limited, limitedPlan, PartyGrowthChoice.Health, out _), "stat overspend is rejected atomically");
            AssertEqual(0, limitedPlan.Get(PartyGrowthChoice.Health), "rejected stat overspend does not alter the plan");
            AssertEqual(true, PartyGrowthRules.TryStage(limited, limitedPlan, PartyGrowthChoice.Arms, out _), "available skill point stages");
            AssertEqual(false, PartyGrowthRules.TryStage(limited, limitedPlan, PartyGrowthChoice.Guard, out _), "skill overspend is rejected");
            AssertEqual(0, limitedPlan.Get(PartyGrowthChoice.Guard), "rejected skill overspend does not alter the plan");

            PartyMember irrelevant = PartyGrowthTestMember("warrior", 0, 1);
            PartyGrowthPlan irrelevantPlan = new PartyGrowthPlan();
            AssertEqual(false, PartyGrowthRules.CanStage(irrelevant, irrelevantPlan, PartyGrowthChoice.Ember, out _), "irrelevant talent cannot stage");
            AssertEqual(false, PartyGrowthRules.TryStage(irrelevant, irrelevantPlan, PartyGrowthChoice.Ember, out _), "irrelevant talent is rejected");
            AssertEqual(true, irrelevantPlan.IsEmpty, "irrelevant-talent rejection leaves the plan empty");

            PartyMember cappedAttribute = PartyGrowthTestMember("warrior", 1, 0);
            cappedAttribute.Stats = new Stats(PartyGrowthRules.MaximumValue, 10, 10, 10);
            AssertEqual(
                false,
                PartyGrowthRules.TrySpendAttributePoint(cappedAttribute, PartyGrowthChoice.Strength, out _),
                "an attribute at 99 rejects another point");
            AssertEqual(PartyGrowthRules.MaximumValue, cappedAttribute.Stats.Strength, "rejected attribute cap does not mutate Strength");
            AssertEqual(1, cappedAttribute.StatPoints, "rejected attribute cap does not spend a point");

            PartyMember cappedTalent = PartyGrowthTestMember("warrior", 0, 1);
            cappedTalent.Skills.Arms = PartyGrowthRules.MaximumValue - 1;
            AssertEqual(
                false,
                PartyGrowthRules.TrySpendTalentPoint(cappedTalent, PartyGrowthChoice.Arms, out _),
                "a talent that would exceed 99 rejects another point");
            AssertEqual(PartyGrowthRules.MaximumValue - 1, cappedTalent.Skills.Arms, "rejected talent cap does not mutate Arms");
            AssertEqual(1, cappedTalent.SkillPoints, "rejected talent cap does not spend a point");

            PartyMember atomic = PartyGrowthTestMember("warrior", 1, 1);
            PartyGrowthPlan invalidPlan = new PartyGrowthPlan();
            invalidPlan.Increment(PartyGrowthChoice.Strength);
            invalidPlan.Increment(PartyGrowthChoice.Health);
            invalidPlan.Increment(PartyGrowthChoice.Arms);
            Stats atomicStats = atomic.Stats;
            int atomicArms = atomic.Skills.Arms;
            AssertEqual(false, PartyGrowthRules.Validate(atomic, invalidPlan, out _), "overspent manual plan fails validation");
            AssertEqual(false, PartyGrowthRules.TryApply(atomic, invalidPlan, out _), "invalid plan cannot partially apply");
            AssertEqual(atomicStats.Strength, atomic.Stats.Strength, "invalid-plan rejection preserves Strength");
            AssertEqual(atomicStats.Health, atomic.Stats.Health, "invalid-plan rejection preserves Health");
            AssertEqual(atomicArms, atomic.Skills.Arms, "invalid-plan rejection preserves talents");
            AssertEqual(1, atomic.StatPoints, "invalid-plan rejection preserves stat points");
            AssertEqual(1, atomic.SkillPoints, "invalid-plan rejection preserves skill points");
            invalidPlan.Reset();
            AssertEqual(true, invalidPlan.IsEmpty, "reset clears an invalid plan");
            AssertEqual(0, invalidPlan.SpentStatPoints, "reset clears staged stat spending");
            AssertEqual(0, invalidPlan.SpentSkillPoints, "reset clears staged skill spending");

            PartyMember preview = PartyGrowthTestMember("warrior", 3, 3);
            PartyGrowthPlan previewPlan = new PartyGrowthPlan();
            AssertEqual(true, PartyGrowthRules.TryStage(preview, previewPlan, PartyGrowthChoice.Strength, out _), "preview attribute stages");
            AssertEqual(true, PartyGrowthRules.TryStage(preview, previewPlan, PartyGrowthChoice.Arms, out _), "preview talent stages");
            PartyGrowthPlan previewClone = previewPlan.Clone();
            previewClone.Increment(PartyGrowthChoice.Health);
            AssertEqual(0, previewPlan.Get(PartyGrowthChoice.Health), "preview clone is independent from the active draft");
            AssertEqual(1, previewClone.Get(PartyGrowthChoice.Health), "preview clone owns its additional choice");
            AssertEqual(11, PartyGrowthRules.ProjectedAttribute(preview, previewPlan, PartyGrowthChoice.Strength), "preview reads staged attribute without applying it");
            AssertEqual(12, PartyGrowthRules.ProjectedSkill(preview, previewPlan, PartyGrowthChoice.Arms), "preview reads staged talent without applying it");
            AssertEqual(10, preview.Stats.Strength, "preview does not mutate member attributes");
            AssertEqual(10, preview.Skills.Arms, "preview does not mutate member talents");
            AssertEqual(3, preview.StatPoints, "preview does not spend stat points");
            AssertEqual(3, preview.SkillPoints, "preview does not spend skill points");
            previewClone.Reset();
            previewPlan.Reset();
            AssertEqual(true, previewClone.IsEmpty, "cancel clears the preview clone");
            AssertEqual(true, previewPlan.IsEmpty, "cancel clears the active growth draft");
            AssertEqual(10, preview.Stats.Strength, "cancel preserves member attributes");
            AssertEqual(10, preview.Skills.Arms, "cancel preserves member talents");
            AssertEqual(3, preview.StatPoints, "cancel preserves stat points");
            AssertEqual(3, preview.SkillPoints, "cancel preserves skill points");
        }

        private static PartyMember PartyGrowthTestMember(string classKey, int statPoints, int skillPoints)
        {
            return new PartyMember
            {
                Name = "Growth Tester",
                ClassKey = classKey,
                Stats = new Stats(10, 10, 10, 10),
                StatPoints = statPoints,
                SkillPoints = skillPoints,
                Skills = new SkillSet
                {
                    Arms = 10,
                    Missile = 10,
                    Mend = 10,
                    Ember = 10,
                    Hex = 10,
                    Guard = 10
                }
            };
        }

        private static void SignatureItemCatalogAndAtlasMatchRuntimeContract()
        {
            string[] expectedIds =
            {
                SignatureItemCatalog.UnfathomableSwordId,
                SignatureItemCatalog.SluicekeeperBladeId,
                SignatureItemCatalog.StormglassConductorId,
                SignatureItemCatalog.RatcatcherRoadcoatId,
                SignatureItemCatalog.GloamReliquaryMailId,
                SignatureItemCatalog.MirrorweaveRoadMantleId,
                SignatureItemCatalog.CrownwardWarbladeId
            };
            string[] expectedNames =
            {
                "Sword of Unfathomable Darkness",
                "+2 Sluicekeeper Blade",
                "+2 Stormglass Conductor",
                "+3 Ratcatcher’s Roadcoat",
                "+4 Gloam Reliquary Mail",
                "+5 Mirrorweave Road Mantle",
                "+6 Crownward Emberglass Warblade"
            };
            string[] expectedIntrinsicNames =
            {
                "Life Drinker",
                "Sluicekeeper’s Brace",
                "Conduction",
                "Sewer-Step",
                "Reliquary Ward",
                "Mirrorweave",
                "Crownfire"
            };
            Func<InventoryItem>[] factories =
            {
                SignatureItemCatalog.CreateUnfathomableSword,
                SignatureItemCatalog.CreateSluicekeeperBlade,
                SignatureItemCatalog.CreateStormglassConductor,
                SignatureItemCatalog.CreateRatcatcherRoadcoat,
                SignatureItemCatalog.CreateGloamReliquaryMail,
                SignatureItemCatalog.CreateMirrorweaveRoadMantle,
                SignatureItemCatalog.CreateCrownwardWarblade
            };

            IReadOnlyList<SignatureItemDefinition> definitions = SignatureItemCatalog.All;
            AssertEqual(7, definitions.Count, "signature-item catalog contains the seven authored rewards");
            AssertEqual(7, definitions.Select(definition => definition.Id).Distinct(StringComparer.Ordinal).Count(), "signature-item IDs are unique");
            AssertEqual(7, definitions.Select(definition => definition.IconIndex).Distinct().Count(), "signature-item icon cells are unique");
            AssertEqual(7, definitions.Select(definition => definition.DisplayName).Distinct(StringComparer.Ordinal).Count(), "signature-item canonical names are unique");

            for (int index = 0; index < definitions.Count; index++)
            {
                SignatureItemDefinition definition = definitions[index];
                AssertEqual(expectedIds[index], definition.Id, "signature-item definition " + index + " stable ID");
                AssertEqual(index, definition.IconIndex, "signature-item definition " + index + " atlas cell");
                AssertEqual(expectedNames[index], definition.DisplayName, "signature-item definition " + index + " canonical name");
                AssertEqual(expectedIntrinsicNames[index], definition.IntrinsicName, "signature-item definition " + index + " canonical intrinsic");
                AssertEqual(true, !string.IsNullOrWhiteSpace(definition.Lore), "signature-item definition " + index + " has authored lore");
                AssertEqual(true, !string.IsNullOrWhiteSpace(definition.IntrinsicName), "signature-item definition " + index + " names its intrinsic");
                AssertEqual(true, !string.IsNullOrWhiteSpace(definition.IntrinsicSummary), "signature-item definition " + index + " explains its intrinsic");
                AssertEqual(definition, SignatureItemCatalog.Find(expectedIds[index]), "signature-item definition " + index + " resolves by ID");
                AssertEqual(definition, SignatureItemCatalog.Identify(expectedNames[index]), "signature-item definition " + index + " resolves by canonical name");
                AssertEqual(index, SignatureItemCatalog.IconIndex(expectedIds[index]), "signature-item definition " + index + " maps its ID to art");
                AssertEqual(index, SignatureItemCatalog.IconIndex(expectedNames[index]), "signature-item definition " + index + " maps its canonical name to art");
                AssertEqual(index, SignatureItemCatalog.IconIndex("enchanted " + expectedNames[index]), "signature-item definition " + index + " keeps art after an enchantment prefix");
                foreach (string legacyName in definition.LegacyDisplayNames)
                {
                    AssertEqual(index, SignatureItemCatalog.IconIndex(legacyName), "signature-item definition " + index + " maps legacy name '" + legacyName + "'");
                }

                InventoryItem factoryItem = factories[index]();
                AssertEqual(definition, SignatureItemCatalog.Find(factoryItem), "signature-item factory " + index + " preserves catalog identity");
                AssertEqual(index, SignatureItemCatalog.IconIndex(factoryItem), "signature-item factory " + index + " preserves atlas cell");
            }

            AssertSignatureItemFactory(
                SignatureItemCatalog.CreateUnfathomableSword(),
                SignatureItemCatalog.UnfathomableSwordId,
                "Sword of Unfathomable Darkness", "stolen", "blackglass", "broadsword", "unfathomable darkness", "weapon",
                4, 1, 0, 1, 1, 5, 11, 8, "epic", "death");
            AssertSignatureItemFactory(
                SignatureItemCatalog.CreateSluicekeeperBlade(),
                SignatureItemCatalog.SluicekeeperBladeId,
                "+2 Sluicekeeper Blade", "sluicekeeper", "fine steel", "broadsword", "guarding", "weapon",
                2, 1, 0, 0, 0, 4, 7, 3, "quest", "physical");
            AssertSignatureItemFactory(
                SignatureItemCatalog.CreateStormglassConductor(),
                SignatureItemCatalog.StormglassConductorId,
                "+2 Stormglass Conductor", "etched", "stormglass", "ritual staff", "storm", "weapon",
                2, 0, 1, 0, 0, 3, 6, 3, "quest", "shock");
            AssertSignatureItemFactory(
                SignatureItemCatalog.CreateRatcatcherRoadcoat(),
                SignatureItemCatalog.RatcatcherRoadcoatId,
                "+3 Ratcatcher’s Roadcoat", "stitched", "rat pelt", "rat pelt armor", "nimble", "armor",
                3, 0, 0, 1, 1, 0, 0, 0, "quest", "");
            AssertSignatureItemFactory(
                SignatureItemCatalog.CreateGloamReliquaryMail(),
                SignatureItemCatalog.GloamReliquaryMailId,
                "+4 Gloam Reliquary Mail", "gloamward", "reliquary scale", "scale mail", "warding", "armor",
                4, 0, 1, 0, 2, 0, 0, 0, "quest", "");
            AssertSignatureItemFactory(
                SignatureItemCatalog.CreateMirrorweaveRoadMantle(),
                SignatureItemCatalog.MirrorweaveRoadMantleId,
                "+5 Mirrorweave Road Mantle", "ashglass", "mirrorweave", "road mantle", "warding", "armor",
                5, 0, 2, 1, 1, 0, 0, 0, "quest", "");
            AssertSignatureItemFactory(
                SignatureItemCatalog.CreateCrownwardWarblade(),
                SignatureItemCatalog.CrownwardWarbladeId,
                "+6 Crownward Emberglass Warblade", "crownward", "emberglass", "broadsword", "warding", "weapon",
                6, 2, 1, 0, 0, 8, 13, 3, "quest", "fire");

            InventoryItem sword = SignatureItemCatalog.CreateUnfathomableSword();
            InventoryItem sluicekeeper = SignatureItemCatalog.CreateSluicekeeperBlade();
            InventoryItem conductor = SignatureItemCatalog.CreateStormglassConductor();
            InventoryItem roadcoat = SignatureItemCatalog.CreateRatcatcherRoadcoat();
            InventoryItem reliquaryMail = SignatureItemCatalog.CreateGloamReliquaryMail();
            InventoryItem mirrorweave = SignatureItemCatalog.CreateMirrorweaveRoadMantle();
            InventoryItem crownward = SignatureItemCatalog.CreateCrownwardWarblade();
            AssertEqual(1, SignatureItemRules.GuardActionBonus(sluicekeeper), "Sluicekeeper’s Brace adds one Guard to the Guard action");
            AssertEqual(0, SignatureItemRules.GuardActionBonus(conductor), "non-Sluicekeeper weapons do not inherit its Guard intrinsic");
            AssertEqual("stun", SignatureItemRules.BasicHitStatus(conductor), "Conduction applies the authored stun status");
            AssertEqual(true, Mathf.Approximately(0.30f, SignatureItemRules.BasicHitStatusChance(conductor)), "Conduction keeps an exact 30% base stun chance");
            AssertEqual(1, SignatureItemRules.DamageReduction(roadcoat, "poison"), "Sewer-Step reduces poison damage by one");
            AssertEqual(0, SignatureItemRules.DamageReduction(roadcoat, "physical"), "Sewer-Step does not reduce physical damage");
            AssertEqual(1, SignatureItemRules.DamageReduction(reliquaryMail, "death"), "Reliquary Ward reduces death damage by one");
            AssertEqual(1, SignatureItemRules.DamageReduction(reliquaryMail, "mind"), "Reliquary Ward reduces mind damage by one");
            AssertEqual(0, SignatureItemRules.DamageReduction(reliquaryMail, "fire"), "Reliquary Ward does not reduce unrelated damage");
            AssertEqual(0, SignatureItemRules.DamageReduction(mirrorweave, "physical"), "Mirrorweave leaves physical damage unchanged");
            AssertEqual(1, SignatureItemRules.DamageReduction(mirrorweave, "fire"), "Mirrorweave reduces nonphysical damage by one");
            AssertEqual(3, SignatureItemRules.WeaponHitBonus(sword), "Life Drinker preserves the sword's exact hit edge");
            AssertEqual(2, SignatureItemRules.WeaponPowerBonus(sword), "Life Drinker preserves the sword's exact power edge");
            AssertEqual(2, SignatureItemRules.LifeDrainAmount(sword, 11), "Life Drinker scales its bounded healing from dealt damage");
            AssertEqual(1, SignatureItemRules.WardTurnsRemovedOnBasicHit(crownward, 3, true), "Crownfire strips one Ward turn on a successful basic hit");
            AssertEqual(0, SignatureItemRules.WardTurnsRemovedOnBasicHit(crownward, 3, false), "Crownfire does not strip Ward on a miss");

            InventoryItem enchantedLegacyConductor = SignatureItemCatalog.CreateStormglassConductor();
            enchantedLegacyConductor.SignatureId = "";
            enchantedLegacyConductor.DisplayName = "+2 etched stormglass ritual staff";
            AssertEqual(true, WeaponEnchantmentRules.ApplyPermanent(enchantedLegacyConductor, "fire"), "legacy signature weapon accepts Maud's permanent binding");
            AssertEqual(true, SignatureItemCatalog.RepairIdentity(enchantedLegacyConductor), "legacy enchanted signature weapon gains stable identity");
            WeaponEnchantmentRules.Rebuild(enchantedLegacyConductor);
            AssertEqual(SignatureItemCatalog.StormglassConductorId, enchantedLegacyConductor.SignatureId, "Maud binding preserves signature ID");
            AssertEqual("+2 Stormglass Conductor", enchantedLegacyConductor.EnchantmentBaseDisplayName, "migration canonicalizes the captured signature base name");
            AssertEqual("flamebound +2 Stormglass Conductor", enchantedLegacyConductor.DisplayName, "Maud prefix survives canonical signature repair");
            AssertEqual(2, SignatureItemCatalog.IconIndex(enchantedLegacyConductor), "Maud binding preserves signature art cell");

            InventoryItem proceduralLookalike = new InventoryItem
            {
                DisplayName = "+2 stormglass conductor blade",
                Mark = "road-marked",
                Material = "stormglass",
                Form = "sabre",
                Trait = "steady",
                Slot = "weapon"
            };
            AssertEqual(null, SignatureItemCatalog.Identify(proceduralLookalike), "similarly named procedural gear is not misclassified as a signature reward");
            AssertEqual(false, SignatureItemCatalog.RepairIdentity(proceduralLookalike), "procedural lookalike remains mutation-free during repair");

            AssertEqual("unique-item-atlas-runtime-v2.20.0.png", RuntimeArtManifest.UniqueItemAtlas, "signature-item art uses the exact approved v2.20 atlas");
            AssertEqual(58, RuntimeArtManifest.ApprovedRuntimeFiles.Length, "approved runtime manifest includes all 58 exact pins");
            AssertEqual(58, RuntimeArtManifest.ApprovedRuntimeFiles.Distinct(StringComparer.Ordinal).Count(), "approved runtime manifest has no duplicate pins");
            AssertEqual(1, RuntimeArtManifest.ApprovedRuntimeFiles.Count(file => file == RuntimeArtManifest.UniqueItemAtlas), "signature-item atlas appears once in the approved runtime manifest");

            Texture2D atlas = null;
            try
            {
                atlas = LoadApprovedRuntimeAtlas(RuntimeArtManifest.UniqueItemAtlas);
                AssertEqual(new Vector2Int(1280, 1024), new Vector2Int(atlas.width, atlas.height), "approved signature-item atlas uses exact 5x4 geometry");
                Color32[] pixels = atlas.GetPixels32();
                float transparentFraction = pixels.Count(pixel => pixel.a < 32) / (float)pixels.Length;
                float visibleFraction = pixels.Count(pixel => pixel.a >= 32) / (float)pixels.Length;
                AssertEqual(true, transparentFraction >= 0.70f, "approved signature-item atlas preserves at least 70% transparency");
                AssertEqual(true, visibleFraction >= 0.04f, "approved signature-item atlas preserves at least 4% visible art");
                AssertAtlasCellCoverageAtAlpha(atlas, 5, 4, Enumerable.Range(0, 7), 0.12f, 0.45f, 8, "approved signature item");
                AssertAtlasCellCoverageAtAlpha(atlas, 5, 4, Enumerable.Range(7, 13), 0f, 0f, 1, "reserved signature item");
            }
            finally
            {
                if (atlas != null) UnityEngine.Object.DestroyImmediate(atlas);
            }
        }

        private static void AssertSignatureItemFactory(
            InventoryItem item,
            string signatureId,
            string displayName,
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
            string damageType)
        {
            AssertEqual(true, item != null, signatureId + " factory creates an item");
            AssertEqual(signatureId, item.SignatureId, signatureId + " factory signature ID");
            AssertEqual(displayName, item.DisplayName, signatureId + " factory canonical name");
            AssertEqual(mark, item.Mark, signatureId + " factory mark");
            AssertEqual(material, item.Material, signatureId + " factory material");
            AssertEqual(form, item.Form, signatureId + " factory form");
            AssertEqual(trait, item.Trait, signatureId + " factory trait");
            AssertEqual(slot, item.Slot, signatureId + " factory slot");
            AssertEqual(bonus, item.Bonus, signatureId + " factory bonus");
            AssertEqual(strengthBonus, item.StrengthBonus, signatureId + " factory Strength");
            AssertEqual(intelligenceBonus, item.IntelligenceBonus, signatureId + " factory Intelligence");
            AssertEqual(agilityBonus, item.AgilityBonus, signatureId + " factory Agility");
            AssertEqual(healthBonus, item.HealthBonus, signatureId + " factory Health");
            AssertEqual(damageMin, item.DamageMin, signatureId + " factory minimum damage");
            AssertEqual(damageMax, item.DamageMax, signatureId + " factory maximum damage");
            AssertEqual(attackSpeed, item.AttackSpeed, signatureId + " factory attack speed");
            AssertEqual(rarity, item.Rarity, signatureId + " factory rarity");
            AssertEqual(damageType, item.DamageType, signatureId + " factory damage type");
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

        private static void InventoryItemIdentityRulesAreStableAndConservative()
        {
            InventoryItem firstSignature = SignatureItemCatalog.CreateStormglassConductor();
            InventoryItem secondSignature = SignatureItemCatalog.CreateStormglassConductor();
            InventoryItem duplicateId = new InventoryItem
            {
                InstanceId = " keep-me ",
                Slot = "weapon",
                Form = "broadsword",
                DisplayName = "duplicate ID probe"
            };
            InventoryItem questItem = new InventoryItem
            {
                Slot = "quest",
                Form = "pelt bundle",
                DisplayName = "proof bundle"
            };
            firstSignature.InstanceId = "keep-me";
            List<InventoryItem> inventory = new List<InventoryItem>
            {
                firstSignature,
                secondSignature,
                duplicateId,
                questItem
            };

            AssertEqual("keep-me", string.Join(",", InventoryItemIdentityRules.DuplicateInstanceIds(inventory)), "identity rules expose ambiguous canonical IDs before repairing them");
            int repaired = InventoryItemIdentityRules.NormalizeInstanceIds(inventory, 123);
            AssertEqual(3, repaired, "identity normalization repairs blanks and the later duplicate without touching the first valid ID");
            AssertEqual("keep-me", firstSignature.InstanceId, "identity normalization preserves the first valid ID");
            AssertEqual("legacy-item-0000007b-0002", secondSignature.InstanceId, "legacy identity is deterministic from campaign seed and original position");
            AssertEqual("legacy-item-0000007b-0003", duplicateId.InstanceId, "duplicate identity receives its own deterministic replacement");
            AssertEqual("legacy-item-0000007b-0004", questItem.InstanceId, "non-equipment inventory receives the same durable identity contract");
            AssertEqual(true, InventoryItemIdentityRules.HasUniqueInstanceIds(inventory), "all non-null inventory entries have unique nonblank IDs");
            AssertEqual(firstSignature.SignatureId, secondSignature.SignatureId, "signature identity remains shared definition metadata");
            AssertEqual(false, string.Equals(firstSignature.InstanceId, secondSignature.InstanceId, StringComparison.Ordinal), "identical signature copies remain distinct physical items");

            string stableIds = string.Join("|", inventory.Select(item => item.InstanceId));
            AssertEqual(0, InventoryItemIdentityRules.NormalizeInstanceIds(inventory, 123), "identity normalization is idempotent");
            AssertEqual(stableIds, string.Join("|", inventory.Select(item => item.InstanceId)), "a second normalization does not churn IDs");
            AssertEqual(secondSignature, InventoryItemIdentityRules.FindById(inventory, secondSignature.InstanceId), "identity lookup returns the exact duplicate instance");

            string signatureIdBeforeRepair = secondSignature.InstanceId;
            SignatureItemCatalog.RepairIdentity(secondSignature);
            WeaponEnchantmentRules.ApplyPermanent(secondSignature, "storm");
            AssertEqual(signatureIdBeforeRepair, secondSignature.InstanceId, "signature repair and enchantment naming never change physical identity");

            InventoryItem admitted = new InventoryItem { Slot = "armor", Form = "mail", DisplayName = "new admission" };
            string admissionId = InventoryItemIdentityRules.EnsureAdmissionId(admitted, inventory);
            AssertEqual(true, admissionId.StartsWith("item-", StringComparison.Ordinal), "new admissions receive opaque IDs rather than content-derived IDs");
            AssertEqual(false, inventory.Any(item => item.InstanceId == admissionId), "new admission ID does not collide with existing inventory");
            AssertEqual(InventoryItemIdentityRules.SchemaVersion, VersionInfo.SaveVersion, "save schema tracks the canonical inventory identity migration");
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
            AssertEqual("midgaard-grand-hearth", MidgaardInteriorRules.GrandHearthZoneId, "Town Hall interior keeps its save-stable zone identity");
            AssertEqual("Town Hall / Grand Hearth", MidgaardInteriorRules.GrandHearthDisplayName, "Town Hall interior has the intended player-facing name");
            AssertEqual("midgaard-grand-hearth-door", MidgaardInteriorRules.GrandHearthDoorId, "Town Hall exterior door identity remains stable");
            AssertEqual("midgaard-grand-hearth-exit", MidgaardInteriorRules.GrandHearthExitId, "Town Hall interior exit identity remains stable");
            AssertEqual("midgaard-grand-hearth-fire", MidgaardInteriorRules.GrandHearthFireId, "Grand Hearth fixture identity remains stable");
            AssertEqual("midgaard-grand-hearth-cargo", MidgaardInteriorRules.GrandHearthCargoId, "Town Hall cargo identity remains stable");
            AssertEqual("midgaard-grand-hearth-window", MidgaardInteriorRules.GrandHearthWindowId, "Town Hall window identity remains stable");
            AssertEqual("midgaard-grand-hearth-map-table", MidgaardInteriorRules.GrandHearthMapTableId, "Town Hall map-table identity remains stable");
            AssertEqual("midgaard-grand-hearth-road-chest", MidgaardInteriorRules.GrandHearthRoadChestId, "Town Hall road-chest identity remains stable");
            AssertEqual("midgaard-grand-hearth-register", MidgaardInteriorRules.GrandHearthRegisterId, "Town Hall register identity remains stable");
            AssertEqual("midgaard-grand-hearth-banner", MidgaardInteriorRules.GrandHearthBannerId, "Town Hall banner identity remains stable");
            AssertEqual("midgaard-grand-hearth-shelves", MidgaardInteriorRules.GrandHearthShelvesId, "Town Hall shelves identity remains stable");
            AssertEqual(3, GrandHearthArtCatalog.FloorAtlasColumns, "Grand Hearth floor atlas has three columns");
            AssertEqual(2, GrandHearthArtCatalog.FloorAtlasRows, "Grand Hearth floor atlas has two rows");
            AssertEqual(6, GrandHearthArtCatalog.FloorAtlasCellCount, "Grand Hearth floor atlas has six semantic cells");
            AssertEqual(3, GrandHearthArtCatalog.SetpieceAtlasColumns, "Grand Hearth set-piece atlas has three columns");
            AssertEqual(2, GrandHearthArtCatalog.SetpieceAtlasRows, "Grand Hearth set-piece atlas has two rows");
            AssertEqual(6, GrandHearthArtCatalog.SetpieceAtlasCellCount, "Grand Hearth set-piece atlas has six semantic cells");
            AssertEqual(3, GrandHearthArtCatalog.AmbienceAtlasColumns, "Grand Hearth ambience atlas has three columns");
            AssertEqual(2, GrandHearthArtCatalog.AmbienceAtlasRows, "Grand Hearth ambience atlas has two rows");
            AssertEqual(6, GrandHearthArtCatalog.AmbienceAtlasCellCount, "Grand Hearth ambience atlas has six semantic cells");
            AssertEqual(0, GrandHearthArtCatalog.HearthLightCell, "Grand Hearth warm light owns ambience cell 0");
            AssertEqual(1, GrandHearthArtCatalog.StormDoorLightCell, "Grand Hearth storm-door spill owns ambience cell 1");
            AssertEqual(2, GrandHearthArtCatalog.WallSconceCell, "Grand Hearth wall sconce owns ambience cell 2");
            AssertEqual(3, GrandHearthArtCatalog.EmberMotesCell, "Grand Hearth embers own ambience cell 3");
            AssertEqual(4, GrandHearthArtCatalog.WindowReflectionCell, "Grand Hearth rain reflection owns ambience cell 4");
            AssertEqual(5, GrandHearthArtCatalog.PatronShadowCell, "Grand Hearth patron shadow owns ambience cell 5");
            AssertEqual(0, GrandHearthArtCatalog.SetpieceIndex(MidgaardInteriorRules.GrandHearthFireId), "Grand Hearth fire owns set-piece cell 0");
            AssertEqual(1, GrandHearthArtCatalog.SetpieceIndex(MidgaardInteriorRules.GrandHearthExitId), "Grand Hearth storm doors own set-piece cell 1");
            AssertEqual(2, GrandHearthArtCatalog.SetpieceIndex(MidgaardInteriorRules.GrandHearthRegisterId), "Grand Hearth register owns set-piece cell 2");
            AssertEqual(3, GrandHearthArtCatalog.SetpieceIndex(MidgaardInteriorRules.GrandHearthBannerId), "Grand Hearth banner owns set-piece cell 3");
            AssertEqual(4, GrandHearthArtCatalog.SetpieceIndex(MidgaardInteriorRules.GrandHearthWindowId), "Grand Hearth window owns set-piece cell 4");
            AssertEqual(5, GrandHearthArtCatalog.SetpieceIndex(MidgaardInteriorRules.GrandHearthCargoId), "Grand Hearth cargo owns set-piece cell 5");
            AssertEqual(5, GrandHearthArtCatalog.SetpieceIndex(MidgaardInteriorRules.GrandHearthShelvesId), "Grand Hearth shelves share set-piece cell 5");
            AssertEqual(-1, GrandHearthArtCatalog.SetpieceIndex(MidgaardInteriorRules.GrandHearthRoadChestId), "company road chest keeps its established prop art");
            AssertEqual(-1, GrandHearthArtCatalog.SetpieceIndex("unknown-grand-hearth-fixture"), "unknown fixtures cannot borrow Grand Hearth set-piece art");
            AssertEqual(6, MidgaardInteriorRules.GrandHearthPatrons.Count, "Town Hall gathering has six authored patrons");
            GrandHearthPatronPlacement[] expectedPatrons =
            {
                new GrandHearthPatronPlacement(1, 1, AmbientCitizenProfession.Tailor),
                new GrandHearthPatronPlacement(6, 1, AmbientCitizenProfession.Mason),
                new GrandHearthPatronPlacement(8, 2, AmbientCitizenProfession.Lamplighter),
                new GrandHearthPatronPlacement(1, 5, AmbientCitizenProfession.CaravanGuide),
                new GrandHearthPatronPlacement(6, 6, AmbientCitizenProfession.RoadPilgrim),
                new GrandHearthPatronPlacement(2, 7, AmbientCitizenProfession.Fishmonger)
            };
            HashSet<int> patronAtlasCells = new HashSet<int>();
            for (int index = 0; index < expectedPatrons.Length; index++)
            {
                GrandHearthPatronPlacement actual = MidgaardInteriorRules.GrandHearthPatrons[index];
                GrandHearthPatronPlacement expected = expectedPatrons[index];
                AssertEqual(expected.OffsetX, actual.OffsetX, $"Town Hall patron {index} keeps its audited horizontal placement");
                AssertEqual(expected.OffsetY, actual.OffsetY, $"Town Hall patron {index} keeps its audited vertical placement");
                AssertEqual(expected.Profession, actual.Profession, $"Town Hall patron {index} keeps its authored citizen role");
                AssertEqual(
                    true,
                    patronAtlasCells.Add(ExplorationCharacterArtCatalog.CitizenAtlasIndex(actual.Profession)),
                    $"Town Hall patron {index} uses a distinct approved citizen sprite");
            }

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
                AssertEqual(1, room.xMin, $"Town Hall uses the safe westward expansion on {size.x}x{size.y}");
                AssertEqual(Math.Max(1, size.y - 10), room.yMin, $"Town Hall keeps its stable north/south reservation on {size.x}x{size.y}");
                AssertEqual(10, room.width, $"Town Hall preserves its largest safe width on {size.x}x{size.y}");
                AssertEqual(9, room.height, $"Town Hall preserves its largest safe height on {size.x}x{size.y}");
                AssertEqual(56, (room.width - 2) * (room.height - 2), $"Town Hall provides an open 8x7 gathering floor on {size.x}x{size.y}");
                AssertEqual(true, room.xMin >= 1 && room.yMin >= 1 && room.xMax < size.x && room.yMax < size.y, $"Grand Hearth room fits supported map {size.x}x{size.y}");
                AssertEqual(true, room.Contains(new Vector2Int(spawn.X, spawn.Y)), $"Grand Hearth spawn stays inside room {size.x}x{size.y}");
                AssertEqual(true, room.Contains(new Vector2Int(exit.X, exit.Y)), $"Grand Hearth exit stays inside room {size.x}x{size.y}");
                AssertEqual(room.xMin + 5, spawn.X, $"Town Hall spawn keeps its stable horizontal coordinate on {size.x}x{size.y}");
                AssertEqual(room.yMin + 4, spawn.Y, $"Town Hall spawn keeps its stable company-runner row on {size.x}x{size.y}");
                AssertEqual(room.xMax - 1, exit.X, $"Grand Hearth storm doors remain on the east wall {size.x}x{size.y}");
                AssertEqual(spawn.Y, exit.Y, $"Town Hall spawn and storm doors remain joined by the company runner on {size.x}x{size.y}");
                AssertEqual(true, MidgaardInteriorRules.IsGrandHearthCompanyRunner(map, room.xMin + 3, spawn.Y), $"Town Hall runner keeps its stable west endpoint on {size.x}x{size.y}");
                AssertEqual(true, MidgaardInteriorRules.IsGrandHearthCompanyRunner(map, room.xMax - 2, spawn.Y), $"Town Hall runner keeps its stable east endpoint on {size.x}x{size.y}");
                AssertEqual(false, MidgaardInteriorRules.IsGrandHearthCompanyRunner(map, room.xMin + 2, spawn.Y), $"Town Hall expansion does not extend the company runner west on {size.x}x{size.y}");
                AssertEqual(
                    true,
                    GrandHearthArtCatalog.TryFloorChoice(map, spawn.X, spawn.Y, 1, out GrandHearthFloorChoice spawnFloor),
                    $"Town Hall spawn resolves authored floor art on {size.x}x{size.y}");
                AssertEqual(3, spawnFloor.AtlasIndex, $"Town Hall spawn uses the company medallion on {size.x}x{size.y}");
                AssertEqual(false, spawnFloor.FlipX || spawnFloor.FlipY, $"Town Hall company medallion keeps its authored orientation on {size.x}x{size.y}");
                AssertEqual(
                    true,
                    GrandHearthArtCatalog.TryFloorChoice(map, room.xMin + 3, spawn.Y, 1, out GrandHearthFloorChoice runnerFloor),
                    $"Town Hall company runner resolves authored floor art on {size.x}x{size.y}");
                AssertEqual(2, runnerFloor.AtlasIndex, $"Town Hall company runner uses its connecting floor cell on {size.x}x{size.y}");
                AssertEqual(false, runnerFloor.FlipX || runnerFloor.FlipY, $"Town Hall company runner keeps its authored orientation on {size.x}x{size.y}");
                AssertEqual(
                    true,
                    GrandHearthArtCatalog.TryFloorChoice(map, exit.X, exit.Y, 1, out GrandHearthFloorChoice thresholdFloor),
                    $"Town Hall storm threshold resolves authored floor art on {size.x}x{size.y}");
                AssertEqual(5, thresholdFloor.AtlasIndex, $"Town Hall storm threshold uses its dedicated floor cell on {size.x}x{size.y}");
                AssertEqual(false, thresholdFloor.FlipX || thresholdFloor.FlipY, $"Town Hall storm threshold keeps its authored orientation on {size.x}x{size.y}");
                foreach (Vector2Int apron in new[]
                {
                    new Vector2Int(room.xMin + 2, room.yMin + 2),
                    new Vector2Int(room.xMin + 2, room.yMin + 3)
                })
                {
                    AssertEqual(
                        true,
                        GrandHearthArtCatalog.TryFloorChoice(map, apron.x, apron.y, 1, out GrandHearthFloorChoice apronFloor),
                        $"Grand Hearth apron resolves authored floor art at {apron.x},{apron.y} on {size.x}x{size.y}");
                    AssertEqual(4, apronFloor.AtlasIndex, $"Grand Hearth apron uses its dedicated floor cell at {apron.x},{apron.y} on {size.x}x{size.y}");
                    AssertEqual(false, apronFloor.FlipX || apronFloor.FlipY, $"Grand Hearth apron keeps its authored orientation at {apron.x},{apron.y} on {size.x}x{size.y}");
                }

                HashSet<int> timberVariants = new HashSet<int>();
                for (int floorY = room.yMin + 1; floorY < room.yMax - 1; floorY++)
                for (int floorX = room.xMin + 1; floorX < room.xMax - 1; floorX++)
                {
                    if (MidgaardInteriorRules.IsGrandHearthCompanyRunner(map, floorX, floorY)
                        || floorX == room.xMin + 2 && (floorY == room.yMin + 2 || floorY == room.yMin + 3))
                    {
                        continue;
                    }

                    AssertEqual(
                        true,
                        GrandHearthArtCatalog.TryFloorChoice(map, floorX, floorY, 1, out GrandHearthFloorChoice timberFloor),
                        $"Town Hall open floor resolves deterministic hearthwood at {floorX},{floorY} on {size.x}x{size.y}");
                    AssertEqual(true, timberFloor.AtlasIndex == 0 || timberFloor.AtlasIndex == 1, $"Town Hall open floor uses only its two hearthwood variants at {floorX},{floorY} on {size.x}x{size.y}");
                    AssertEqual(
                        true,
                        GrandHearthArtCatalog.TryFloorChoice(map, floorX, floorY, 1, out GrandHearthFloorChoice repeatedTimber)
                            && repeatedTimber.AtlasIndex == timberFloor.AtlasIndex
                            && repeatedTimber.FlipX == timberFloor.FlipX
                            && repeatedTimber.FlipY == timberFloor.FlipY,
                        $"Town Hall hearthwood choice is deterministic at {floorX},{floorY} on {size.x}x{size.y}");
                    timberVariants.Add(timberFloor.AtlasIndex);
                }
                AssertEqual("0,1", string.Join(",", timberVariants.OrderBy(index => index)), $"Town Hall open floor uses both restrained hearthwood variants on {size.x}x{size.y}");
                AssertEqual(false, GrandHearthArtCatalog.TryFloorChoice(map, room.xMin, room.yMin, 0, out _), $"Town Hall walls never resolve walkable floor art on {size.x}x{size.y}");
                AssertEqual(false, GrandHearthArtCatalog.TryFloorChoice(map, room.xMax, room.yMin, 1, out _), $"cells outside Town Hall never resolve its floor art on {size.x}x{size.y}");
                MapData otherDepthMap = new MapData { Width = size.x, Height = size.y, Depth = 2 };
                AssertEqual(false, GrandHearthArtCatalog.TryFloorChoice(otherDepthMap, spawn.X, spawn.Y, 1, out _), $"other depths never borrow Town Hall floor art on {size.x}x{size.y}");
                AssertEqual(false, room.Overlaps(MidgaardInteriorRules.ThroneRoomBounds(map)), $"Grand Hearth remains separate from the throne room {size.x}x{size.y}");
                AssertEqual(false, room.Overlaps(MidgaardInteriorRules.MerchantHallBounds(map)), $"Grand Hearth remains separate from the merchant hall {size.x}x{size.y}");
                AssertEqual(true, MidgaardInteriorRules.IsReservedCell(map, spawn.X, spawn.Y), $"Grand Hearth spawn is protected from procedural landmarks {size.x}x{size.y}");

                HashSet<Vector2Int> fixtureCells = new HashSet<Vector2Int>
                {
                    new Vector2Int(exit.X, exit.Y),
                    new Vector2Int(room.xMin + 2, room.yMin + 2),
                    new Vector2Int(room.xMin + 3, room.yMin + 2),
                    new Vector2Int(room.xMin + 4, room.yMin + 1),
                    new Vector2Int(room.xMin + 3, room.yMin),
                    new Vector2Int(room.xMax - 3, room.yMin),
                    new Vector2Int(room.xMax - 3, room.yMin + 1),
                    new Vector2Int(room.xMax - 2, room.yMin + 1),
                    new Vector2Int(room.xMax - 3, room.yMax - 4),
                    new Vector2Int(room.xMin + 2, room.yMax - 4),
                    new Vector2Int(room.xMin + 4, room.yMax - 3),
                    new Vector2Int(room.xMax - 2, room.yMax - 3)
                };
                HashSet<Vector2Int> patronCells = new HashSet<Vector2Int>();
                foreach (GrandHearthPatronPlacement patron in MidgaardInteriorRules.GrandHearthPatrons)
                {
                    Vector2Int cell = new Vector2Int(room.xMin + patron.OffsetX, room.yMin + patron.OffsetY);
                    AssertEqual(true, patronCells.Add(cell), $"Town Hall patron placements are unique on {size.x}x{size.y}");
                    AssertEqual(
                        true,
                        cell.x > room.xMin && cell.x < room.xMax - 1
                            && cell.y > room.yMin && cell.y < room.yMax - 1,
                        $"Town Hall patron remains on the open interior floor at {cell.x},{cell.y} on {size.x}x{size.y}");
                    AssertEqual(false, MidgaardInteriorRules.IsGrandHearthCompanyRunner(map, cell.x, cell.y), $"Town Hall patron stays off the company runner at {cell.x},{cell.y} on {size.x}x{size.y}");
                    AssertEqual(false, cell == new Vector2Int(spawn.X, spawn.Y), $"Town Hall patron keeps the first-spawn tile clear on {size.x}x{size.y}");
                    AssertEqual(false, cell == new Vector2Int(exit.X, exit.Y), $"Town Hall patron keeps the storm doors clear on {size.x}x{size.y}");
                    AssertEqual(false, fixtureCells.Contains(cell), $"Town Hall patron avoids stable fixture and NPC cells at {cell.x},{cell.y} on {size.x}x{size.y}");
                    AssertEqual(
                        true,
                        ExplorationCharacterArtCatalog.CitizenAtlasIndex(patron.Profession) >= 0
                            && ExplorationCharacterArtCatalog.CitizenAtlasIndex(patron.Profession) < ExplorationCharacterArtCatalog.CitizenCellCount,
                        $"Town Hall patron {patron.Profession} maps to the approved citizen atlas");
                    AssertEqual(
                        true,
                        MidgaardInteriorRules.TryGrandHearthPatron(map, cell.x, cell.y, out AmbientCitizenProfession resolved)
                            && resolved == patron.Profession,
                        $"Town Hall patron lookup round-trips {cell.x},{cell.y} on {size.x}x{size.y}");
                }
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
            WorldZone zone = WorldZoneCatalog.For(MidgaardInteriorRules.GrandHearthZoneId, 1);
            AssertEqual(MidgaardInteriorRules.GrandHearthDisplayName, zone.Name, "Town Hall / Grand Hearth has a dedicated player-facing zone");
            AssertEqual(true, zone.Summary.IndexOf("patrons", StringComparison.OrdinalIgnoreCase) >= 0, "Town Hall zone copy describes the gathering patrons");
            AssertEqual(
                true,
                zone.Story.IndexOf("leave through the storm doors", StringComparison.OrdinalIgnoreCase) >= 0
                    && zone.Story.IndexOf("begin the journey", StringComparison.OrdinalIgnoreCase) >= 0,
                "Town Hall zone copy makes departure the first journey step");
            string explorationHudSource = File.ReadAllText(Path.Combine(Application.dataPath, "Scripts", "Legacy", "AshenHallsGame.ExplorationHud.cs"));
            AssertEqual(
                true,
                explorationHudSource.Contains("\"Leave Town Hall through the storm doors to begin the journey.\""),
                "opening objective explicitly requires leaving Town Hall");
            AssertEqual(MusicDirectorRules.GrandHearth, MusicDirectorRules.ExploreTrackKey(MidgaardInteriorRules.GrandHearthZoneId, ObjectType.Tavern, true, false), "Grand Hearth continues the title theme with its quieter in-world reprise");
            AssertEqual("ambhearth", GameAudioCueRules.AmbientFor(MidgaardInteriorRules.GrandHearthZoneId, null), "Grand Hearth uses the hearth ambience bed");
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

            IReadOnlyList<RoamingThreatDefinition> productionChapterTwo = RoamingThreatCatalog.ForDepth(2, false);
            AssertRoamingThreatRoster(productionChapterTwo, 2, 3, "production Chapter II patrol roster");
            AssertEqual(
                "dusk-market-kobold-raiders|quarry-kobold-hexers|old-road-ratfolk-holdouts",
                string.Join("|", productionChapterTwo.Select(definition => definition.Id)),
                "production Chapter II patrol identities stay deterministic");
            AssertEqual(2, productionChapterTwo.Count(definition => definition.Faction == RoamingThreatFaction.Kobolds), "production Chapter II owns two kobold patrols");
            AssertEqual(1, productionChapterTwo.Count(definition => definition.Faction == RoamingThreatFaction.Rats), "production Chapter II keeps one ratfolk holdout patrol");
            AssertEqual(true, productionChapterTwo.SelectMany(definition => definition.EnemyIds)
                .All(id => ContentSetCatalog.EnemyActive(ContentSetCatalog.SewerSlice, id)), "production Chapter II patrol enemies are active in its content set");

            IReadOnlyList<RoamingThreatDefinition> productionChapterThree = RoamingThreatCatalog.ForDepth(3, false);
            AssertRoamingThreatRoster(productionChapterThree, 3, 3, "production Chapter III patrol roster");
            AssertEqual(
                "bone-road-drow-watch|gloam-crypt-procession|red-gate-grave-watch",
                string.Join("|", productionChapterThree.Select(definition => definition.Id)),
                "production Chapter III patrol identities stay deterministic");
            AssertEqual(true, productionChapterThree.All(definition =>
                definition.Faction == RoamingThreatFaction.Drow
                || definition.Faction == RoamingThreatFaction.Undead), "production Chapter III patrols stay drow-or-undead only");
            AssertEqual(true, productionChapterThree.SelectMany(definition => definition.EnemyIds)
                .All(id => ContentSetCatalog.EnemyActive(ContentSetCatalog.SewerSlice, id)), "production Chapter III patrol enemies are active in its content set");

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
            AssertEqual(18, RoamingThreatPresentationRules.SpriteIndex("gloamknight"), "Gloam Knight patrol uses the ash-road revenant-knight cell");
            AssertEqual(RoamingThreatFaction.Undead, RoamingThreatCatalog.FactionForArchetype("gloamknight"), "Gloam Knight patrol art remains aligned with its undead roster");
            AssertEqual("undeadalert", CreatureAudioRules.CueForArchetype("gloamknight", "alert"), "Gloam Knight patrol audio remains aligned with its undead roster");
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
            CombatBoardNavigationRulesKeepTargetingDeterministic();
        }

        private static void CombatBoardNavigationRulesKeepTargetingDeterministic()
        {
            Vector2Int northwest = CombatBoardNavigationRules.Step(
                new Vector2Int(0, 0),
                -1,
                -1,
                12,
                8);
            AssertEqual(new Vector2Int(0, 0), northwest, "combat board cursor clamps at the northwest edge");
            Vector2Int southeast = CombatBoardNavigationRules.Step(
                new Vector2Int(11, 7),
                4,
                9,
                12,
                8);
            AssertEqual(new Vector2Int(11, 7), southeast, "combat board cursor clamps at the southeast edge");
            AssertEqual(
                new Vector2Int(4, 2),
                CombatBoardNavigationRules.Step(new Vector2Int(3, 3), 6, -8, 12, 8),
                "combat board cursor advances one deterministic cell per navigation step");

            Vector2Int origin = new Vector2Int(2, 2);
            Vector2Int[] unordered =
            {
                new Vector2Int(4, 2),
                new Vector2Int(2, 3),
                new Vector2Int(1, 2),
                new Vector2Int(2, 1),
                new Vector2Int(2, 1),
                new Vector2Int(-1, 2)
            };
            List<Vector2Int> ordered = CombatBoardNavigationRules.OrderCandidates(origin, unordered, 12, 8);
            AssertEqual(4, ordered.Count, "combat target cycle removes duplicates and out-of-board cells");
            AssertEqual(new Vector2Int(2, 1), ordered[0], "combat target cycle sorts nearest candidates in stable row order");
            AssertEqual(new Vector2Int(1, 2), ordered[1], "combat target cycle resolves same-distance candidates by row then column");
            AssertEqual(new Vector2Int(2, 3), ordered[2], "combat target cycle keeps the final nearest candidate stable");
            AssertEqual(new Vector2Int(4, 2), ordered[3], "combat target cycle places farther candidates after nearer ones");

            AssertEqual(
                ordered[0],
                CombatBoardNavigationRules.Cycle(origin, origin, unordered, 1, 12, 8),
                "forward target cycle enters at the first legal target when the cursor is elsewhere");
            AssertEqual(
                ordered[ordered.Count - 1],
                CombatBoardNavigationRules.Cycle(origin, origin, unordered, -1, 12, 8),
                "reverse target cycle enters at the last legal target when the cursor is elsewhere");
            AssertEqual(
                ordered[0],
                CombatBoardNavigationRules.Cycle(origin, ordered[ordered.Count - 1], unordered, 1, 12, 8),
                "forward target cycle wraps deterministically");
            AssertEqual(
                ordered[ordered.Count - 1],
                CombatBoardNavigationRules.Cycle(origin, ordered[0], unordered, -1, 12, 8),
                "reverse target cycle wraps deterministically");

            AssertEqual(
                false,
                CombatBoardNavigationRules.PointerMovementOwnsInspection(Vector2.zero, new Vector2(1f, 1f)),
                "stationary pointer jitter never clears controller board focus");
            AssertEqual(
                true,
                CombatBoardNavigationRules.PointerMovementOwnsInspection(Vector2.zero, new Vector2(3f, 0f)),
                "deliberate pointer movement restores pointer inspection");
            AssertEqual(
                false,
                CombatBoardNavigationRules.NavigationIsNeutral(0.55f, 0f, false),
                "held stick at the routing threshold is not neutral after pointer takeover");
            AssertEqual(
                false,
                CombatBoardNavigationRules.NavigationIsNeutral(0f, 0f, true),
                "held digital navigation is not neutral after pointer takeover");
            AssertEqual(
                true,
                CombatBoardNavigationRules.NavigationIsNeutral(0.54f, -0.54f, false),
                "released navigation inside the configured dead zone clears pointer-takeover suppression");
            string combatInputSource = File.ReadAllText(Path.Combine(
                Application.dataPath,
                "Scripts",
                "Legacy",
                "AshenHallsGame.Combat.cs"));
            AssertEqual(
                true,
                combatInputSource.Contains("if (TrackCombatBoardPointerOwnership() || ConsumeCombatBoardNavigationAfterPointerTakeover()) return;"),
                "production HandleCombatHotkeys short-circuits both pointer takeover and held navigation suppression");

            HelpOverlayView help = HelpOverlayContent.Build(GameMode.Combat, false, 6, "Midgaard");
            AssertEqual(
                true,
                help.Lines.Any(line => line.IndexOf("left stick", StringComparison.OrdinalIgnoreCase) >= 0),
                "combat help names the configured left-stick board cursor");
            AssertEqual(
                true,
                help.Lines.Any(line => line.IndexOf("bumper", StringComparison.OrdinalIgnoreCase) >= 0),
                "combat help names deterministic controller target cycling");
            AssertEqual(
                true,
                help.Lines.Any(line => line.IndexOf("Submit", StringComparison.OrdinalIgnoreCase) >= 0),
                "combat help names controller cursor confirmation");
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
            AssertEqual(27, VersionInfo.SaveVersion, "current save schema persists exact inventory identity and equipment links");
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
            AssertEqual(27, VersionInfo.SaveVersion, "current save schema persists exact inventory identity and equipment links");
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

            controller.BeginTurn(hero, false);
            state.Elixirs = 2;
            hero.Hp = hero.MaxHp;
            hero.Mana = hero.MaxMana;
            int fullRecoveryMovePoints = state.Combat.MovePoints;
            AssertEqual(false, CombatController.HasElixirRecoveryBenefit(hero, 18, 6), "full health and mana expose no elixir benefit");
            AssertEqual(
                false,
                controller.ActionEnabled(ActionMode.Elixir, hero, false, false, state.Elixirs, false),
                "the combat command disables an elixir with no recovery benefit");
            CombatCommandResult unnecessaryItem = controller.TryUseItem(hero, 18, 6);
            AssertEqual(false, unnecessaryItem.Success, "full health and mana reject an unnecessary elixir");
            AssertEqual(CombatCommandFailure.NoRecoveryNeeded, unnecessaryItem.Failure, "unnecessary elixir reports its specific rejection");
            AssertEqual(2, state.Elixirs, "rejected elixir preserves the shared resource");
            AssertEqual(hero.MaxHp, hero.Hp, "rejected elixir preserves full health");
            AssertEqual(hero.MaxMana, hero.Mana, "rejected elixir preserves full mana");
            AssertEqual(fullRecoveryMovePoints, state.Combat.MovePoints, "rejected elixir preserves movement");
            AssertEqual(true, state.Combat.ActionAvailable, "rejected elixir preserves the action");
            AssertEqual(false, state.Combat.Acted, "rejected elixir does not mark the unit acted");
            AssertEqual(CombatPhase.ChooseAction, state.Combat.Phase, "rejected elixir stays in the command phase");

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

        private static void BoneRoadProductionArcDefinitionsAreStable()
        {
            List<string> flags = new List<string>();
            AssertEqual(false, ContentSetCatalog.AllowBoneRoadChapter(ContentSetCatalog.SewerSlice, flags), "production Bone Road stays locked before Varkh falls");
            flags.Add(StoryFlags.KoboldKingDefeated);
            AssertEqual(true, ContentSetCatalog.AllowBoneRoadChapter(ContentSetCatalog.SewerSlice, flags), "Varkh's defeat opens the production Bone Road chapter");
            AssertEqual(true, ContentSetCatalog.AllowBoneRoadChapter(ContentSetCatalog.FullPrototype, null), "full prototype keeps the Bone Road available without production flags");

            EncounterDefinition watch = EncounterCatalog.For(EncounterId.BoneRoadWatch);
            EncounterDefinition ritual = EncounterCatalog.For(EncounterId.GloamCryptRitual);
            EncounterDefinition warden = EncounterCatalog.For(EncounterId.GloamWarden);
            EncounterDefinition[] chapterEncounters = { watch, ritual, warden };
            AssertEqual("bone-road-watch", watch.LegacyStyle, "Bone Road watch owns its stable encounter style");
            AssertEqual("gloam-crypt-ritual", ritual.LegacyStyle, "Gloam crypt ritual owns its stable encounter style");
            AssertEqual("gloam-warden-boss", warden.LegacyStyle, "Gloam Warden owns its stable encounter style");
            AssertEqual("drowscout|drowcrossbow|husk|reaver", string.Join("|", watch.EnemyIds), "Bone Road watch has the exact drow-and-undead roster");
            AssertEqual("bonepriest|shade|husk|reaver|drowmage", string.Join("|", ritual.EnemyIds), "Gloam ritual has the exact caster-pressure roster");
            AssertEqual("gloamknight|bonepriest|drowpriest|reaver|shade|drowcrossbow", string.Join("|", warden.EnemyIds), "Ossuary Warden encounter has the exact boss roster");

            foreach (EncounterDefinition encounter in chapterEncounters)
            {
                AssertEqual(false, encounter.UsesGeneratedEnemyPool, encounter.LegacyStyle + " remains an authored encounter");
                AssertEqual(encounter.EnemyIds.Length, encounter.FixedEnemyCount, encounter.LegacyStyle + " spawns its exact roster once");
                AssertEqual(true, ContentSetCatalog.IsBoneRoadEncounterStyle(encounter.LegacyStyle), encounter.LegacyStyle + " is recognized as Chapter III combat");
                AssertEqual(true, encounter.EnemyIds.All(enemyId => ContentSetCatalog.EnemyActive(ContentSetCatalog.SewerSlice, enemyId)), encounter.LegacyStyle + " uses production-active enemies");
                AssertEqual(true, encounter.EnemyIds.All(enemyId =>
                {
                    RoamingThreatFaction faction = RoamingThreatCatalog.FactionForEnemy(enemyId);
                    return faction == RoamingThreatFaction.Drow || faction == RoamingThreatFaction.Undead;
                }), encounter.LegacyStyle + " cannot drift outside its drow-and-undead faction contract");
                foreach (Point obstacle in encounter.Obstacles.Where(CombatRitualRules.IsRitual))
                {
                    string spawnRole = ContentSetCatalog.RitualSpawnRoleForEncounter(
                        encounter.LegacyStyle,
                        obstacle.Kind);
                    AssertEqual(true,
                        ContentSetCatalog.EnemyActive(ContentSetCatalog.SewerSlice, spawnRole),
                        encounter.LegacyStyle + " ritual resolves to a production-active enemy");
                    RoamingThreatFaction spawnFaction = RoamingThreatCatalog.FactionForEnemy(spawnRole);
                    AssertEqual(true,
                        spawnFaction == RoamingThreatFaction.Drow || spawnFaction == RoamingThreatFaction.Undead,
                        encounter.LegacyStyle + " ritual reinforcement stays drow-or-undead");
                }
            }

            AssertEqual(false, ContentSetCatalog.IsBoneRoadEncounterStyle("koboldking"), "Varkh remains Chapter II combat");
            AssertEqual(false, ContentSetCatalog.IsBoneRoadEncounterStyle("guard"), "generic guard combat cannot advance Chapter III");
            AssertEqual("bonepriest", ContentSetCatalog.RitualSpawnRoleForEncounter(ritual.LegacyStyle, "glyph"), "crypt glyph opens into a production Bone Priest");
            AssertEqual("shade", ContentSetCatalog.RitualSpawnRoleForEncounter(warden.LegacyStyle, "glyph"), "Warden glyph opens into an Ossuary Shade");
            AssertEqual("gloamknight", ContentSetCatalog.RitualSpawnRoleForEncounter(warden.LegacyStyle, "demonrift"), "Warden breach opens into a Gloam Knight");
            AssertEqual("koboldraider", ContentSetCatalog.RitualSpawnRoleForEncounter("koboldking", "glyph"), "non-Chapter-III glyph behavior remains stable");
            AssertEqual(2, warden.Obstacles.Count(point => point.Kind == "glyph" && point.Duration == 8), "Ossuary Warden board owns two long ritual glyphs");
            AssertEqual(true, warden.Obstacles.Any(point => point.X == 7 && point.Y == 3 && point.Kind == "demonrift" && point.Duration == 6), "Ossuary Warden board owns its central breach obstacle");
            AssertEqual(
                "5,1:stone:0|5,6:stone:0|6,2:glyph:8|6,5:glyph:8|7,3:demonrift:6|7,4:gas:7",
                string.Join("|", warden.Obstacles.Select(point => $"{point.X},{point.Y}:{point.Kind}:{point.Duration}")),
                "Ossuary Warden obstacle pattern remains deterministic");

            List<string> completionFlags = new List<string> { StoryFlags.GloamWardenDefeated };
            AssertEqual(false, ContentSetCatalog.BoneRoadComplete(completionFlags), "defeating the Warden alone does not skip the Red Gate warning");
            completionFlags.Add(StoryFlags.RedGateWarningRecovered);
            AssertEqual(true, ContentSetCatalog.BoneRoadComplete(completionFlags), "Warden victory plus the recovered warning completes Chapter III");
        }

        private static void GlassAndAshProductionArcDefinitionsAreStable()
        {
            List<string> flags = new List<string>();
            AssertEqual(false, ContentSetCatalog.AllowGlassAndAshChapter(ContentSetCatalog.SewerSlice, flags), "production Glass and Ash stays locked before the frontier survey");
            flags.Add(StoryFlags.GlassAndAshFrontierSurveyed);
            AssertEqual(false, ContentSetCatalog.AllowGlassAndAshChapter(ContentSetCatalog.SewerSlice, flags), "surveying the frontier alone does not skip Yara's expedition briefing");
            flags.Add(StoryFlags.GlassAndAshExpeditionAccepted);
            AssertEqual(true, ContentSetCatalog.AllowGlassAndAshChapter(ContentSetCatalog.SewerSlice, flags), "the surveyed and accepted expedition opens production Chapter IV");
            AssertEqual(true, ContentSetCatalog.AllowGlassAndAshChapter(ContentSetCatalog.FullPrototype, null), "full prototype keeps Glass and Ash available without production flags");

            EncounterDefinition ambush = EncounterCatalog.For(EncounterId.GlasswardAmbush);
            EncounterDefinition keepers = EncounterCatalog.For(EncounterId.GlassIndexKeepers);
            EncounterDefinition warden = EncounterCatalog.For(EncounterId.AshenPactWarden);
            EncounterDefinition[] chapterEncounters = { ambush, keepers, warden };
            AssertEqual("glassward-ambush", ambush.LegacyStyle, "Glassward Ambush owns its stable encounter style");
            AssertEqual("glass-index-keepers", keepers.LegacyStyle, "Glass Index Keepers own their stable encounter style");
            AssertEqual("ashen-pact-warden-boss", warden.LegacyStyle, "Ashen Pact Warden owns its stable encounter style");
            AssertEqual("drowscout|drowcrossbow|drowmage|glassmage", string.Join("|", ambush.EnemyIds), "Glassward Ambush has the exact scout-and-caster roster");
            AssertEqual("glassmage|glassmage|drowpriest|drowcrossbow|shade", string.Join("|", keepers.EnemyIds), "Glass Index Keepers have the exact reflected-caster roster");
            AssertEqual("lesserdemon|cinderling|cinderling|glassmage|drowpriest|drowscout", string.Join("|", warden.EnemyIds), "Ashen Pact Warden has the exact far-seal roster");

            foreach (EncounterDefinition encounter in chapterEncounters)
            {
                AssertEqual(false, encounter.UsesGeneratedEnemyPool, encounter.LegacyStyle + " remains an authored encounter");
                AssertEqual(encounter.EnemyIds.Length, encounter.FixedEnemyCount, encounter.LegacyStyle + " spawns its exact roster once");
                AssertEqual(true, ContentSetCatalog.IsGlassAndAshEncounterStyle(encounter.LegacyStyle), encounter.LegacyStyle + " is recognized as Chapter IV combat");
                AssertEqual(true, encounter.EnemyIds.All(enemyId => ContentSetCatalog.EnemyActive(ContentSetCatalog.SewerSlice, enemyId)), encounter.LegacyStyle + " uses production-active enemies");
                AssertEqual(false, encounter.Obstacles.Any(CombatRitualRules.IsRitual), encounter.LegacyStyle + " cannot open an unrelated prototype reinforcement");
            }

            AssertEqual(false, ContentSetCatalog.IsGlassAndAshEncounterStyle("gloam-warden-boss"), "the Ossuary Warden remains Chapter III combat");
            AssertEqual(false, ContentSetCatalog.IsGlassAndAshEncounterStyle("guard"), "generic guard combat cannot advance Chapter IV");
            AssertEqual(2, ambush.Obstacles.Count(point => point.Kind == "ice" && point.Duration == 6), "Glassward Ambush owns two readable cold lanes");
            AssertEqual(2, keepers.EnemyIds.Count(id => id == "glassmage"), "Glass Index Keepers center their pressure on two Glass Mages");
            AssertEqual(2, warden.EnemyIds.Count(id => id == "cinderling"), "Ashen Pact Warden owns two cinderling outriders");
            AssertEqual(true, warden.EnemyIds.First() == "lesserdemon", "the first Ashen Pact enemy remains the named boss role for presentation wiring");

            List<string> completionFlags = new List<string> { StoryFlags.GlassIndexRecovered };
            AssertEqual(false, ContentSetCatalog.GlassAndAshComplete(completionFlags), "recovering the Glass Index does not skip the far-seal fight");
            completionFlags.Add(StoryFlags.EmberglassGateKeyRecovered);
            AssertEqual(true, ContentSetCatalog.GlassAndAshComplete(completionFlags), "the recovered Emberglass gate key completes Chapter IV");
            AssertEqual("glass_and_ash_debriefed", StoryFlags.GlassAndAshDebriefed, "the post-key Yara debrief has a stable save flag");

            InventoryItem mantle = ContentSetCatalog.CreateAshglassRoadMantle();
            AssertEqual("+5 Mirrorweave Road Mantle", mantle.DisplayName, "Chapter IV reward has a stable player-facing identity");
            AssertEqual("armor", mantle.Slot, "Chapter IV reward is equippable armor rather than a sellable gate key");
            AssertEqual(5, mantle.Bonus, "Chapter IV reward advances beyond the Gloam reliquary reward");
            AssertEqual(2, mantle.IntelligenceBonus, "Chapter IV reward carries its exact mirror ward bonus");
            AssertEqual(1, mantle.AgilityBonus, "Chapter IV reward carries its exact road movement bonus");
            AssertEqual("quest", mantle.Rarity, "Chapter IV reward remains a unique quest item");

            IReadOnlyList<RoamingThreatDefinition> productionChapterFour = RoamingThreatCatalog.ForDepth(4, false);
            AssertRoamingThreatRoster(productionChapterFour, 4, 3, "production Chapter IV patrol roster");
            AssertEqual(
                "glassward-drow-levy|ash-fen-bone-procession|red-gate-cinder-pact",
                string.Join("|", productionChapterFour.Select(definition => definition.Id)),
                "production Chapter IV patrol identities stay deterministic");
            AssertEqual(
                "drowscout|drowcrossbow|drowmage|glassmage",
                string.Join("|", productionChapterFour.Single(definition => definition.Id == "glassward-drow-levy").EnemyIds),
                "Glassward patrol has the exact drow-and-glass roster");
            AssertEqual(
                "bonepriest|shade|husk|reaver",
                string.Join("|", productionChapterFour.Single(definition => definition.Id == "ash-fen-bone-procession").EnemyIds),
                "Ash Fen patrol has the exact undead roster");
            AssertEqual(
                "lesserdemon|cinderling|cinderling",
                string.Join("|", productionChapterFour.Single(definition => definition.Id == "red-gate-cinder-pact").EnemyIds),
                "Red Gate patrol has the exact demon roster");
            AssertEqual(true, productionChapterFour.SelectMany(definition => definition.EnemyIds)
                .All(id => ContentSetCatalog.EnemyActive(ContentSetCatalog.SewerSlice, id)), "production Chapter IV patrol enemies are active in its content set");
            AssertEqual(3, RoamingThreatCatalog.ForDepth(5, false).Count, "production patrol catalog exposes exactly three authored Chapter V bands");
        }

        private static void RedGateProductionArcDefinitionsAreStable()
        {
            List<string> flags = new List<string>();
            AssertEqual(false, ContentSetCatalog.AllowRedGateChapter(ContentSetCatalog.SewerSlice, flags), "production Red Gate stays locked before the Emberglass key");
            flags.Add(StoryFlags.EmberglassGateKeyRecovered);
            AssertEqual(false, ContentSetCatalog.AllowRedGateChapter(ContentSetCatalog.SewerSlice, flags), "the key alone does not skip Yara's debrief");
            flags.Add(StoryFlags.GlassAndAshDebriefed);
            AssertEqual(false, ContentSetCatalog.AllowRedGateChapter(ContentSetCatalog.SewerSlice, flags), "the debrief alone does not accept the inner-road assault");
            flags.Add(StoryFlags.RedGateAssaultAccepted);
            AssertEqual(true, ContentSetCatalog.AllowRedGateChapter(ContentSetCatalog.SewerSlice, flags), "explicit Red Gate acceptance opens production Chapter V");
            AssertEqual(true, ContentSetCatalog.AllowRedGateChapter(ContentSetCatalog.FullPrototype, null), "full prototype keeps the Red Gate available without production flags");

            EncounterDefinition vanguard = EncounterCatalog.For(EncounterId.RedGateVanguard);
            EncounterDefinition ossuary = EncounterCatalog.For(EncounterId.OssuaryRoadSeal);
            EncounterDefinition marshal = EncounterCatalog.For(EncounterId.CrownroadMarshal);
            EncounterDefinition[] chapterEncounters = { vanguard, ossuary, marshal };
            AssertEqual("red-gate-vanguard", vanguard.LegacyStyle, "Red Gate Vanguard owns its stable encounter style");
            AssertEqual("ossuary-road-seal", ossuary.LegacyStyle, "Ossuary Road Seal owns its stable encounter style");
            AssertEqual("crownroad-marshal-boss", marshal.LegacyStyle, "Crownroad Marshal owns its stable encounter style");
            AssertEqual("drowpriest|drowmage|glassmage|drowcrossbow|lesserdemon", string.Join("|", vanguard.EnemyIds), "Red Gate Vanguard has the exact inner-gate roster");
            AssertEqual("bonepriest|bonepriest|gloamknight|shade|reaver|cinderling", string.Join("|", ossuary.EnemyIds), "Ossuary Road Seal has the exact reliquary roster");
            AssertEqual("lesserdemon|gloamknight|drowpriest|bonepriest|cinderling|glassmage", string.Join("|", marshal.EnemyIds), "Crownroad Marshal has the exact three-faction boss roster");

            foreach (EncounterDefinition encounter in chapterEncounters)
            {
                AssertEqual(false, encounter.UsesGeneratedEnemyPool, encounter.LegacyStyle + " remains an authored encounter");
                AssertEqual(encounter.EnemyIds.Length, encounter.FixedEnemyCount, encounter.LegacyStyle + " spawns its exact roster once");
                AssertEqual(true, ContentSetCatalog.IsRedGateEncounterStyle(encounter.LegacyStyle), encounter.LegacyStyle + " is recognized as Chapter V combat");
                AssertEqual(true, encounter.EnemyIds.All(enemyId => ContentSetCatalog.EnemyActive(ContentSetCatalog.SewerSlice, enemyId)), encounter.LegacyStyle + " uses production-active enemies");
                AssertEqual(false, encounter.Obstacles.Any(CombatRitualRules.IsRitual), encounter.LegacyStyle + " cannot open an unrelated prototype reinforcement");
            }

            AssertEqual(false, ContentSetCatalog.IsRedGateEncounterStyle("ashen-pact-warden-boss"), "the Ashen Pact Warden remains Chapter IV combat");
            AssertEqual(false, ContentSetCatalog.IsRedGateEncounterStyle("boss"), "the final prototype boss cannot advance Chapter V");
            AssertEqual(2, ossuary.EnemyIds.Count(id => id == "bonepriest"), "the Ossuary Road Seal centers its pressure on two Bone Priests");
            AssertEqual(true, marshal.EnemyIds.First() == "lesserdemon", "the first marshal enemy remains the named boss role for presentation wiring");
            AssertEqual(2, marshal.Obstacles.Count(point => point.Kind == "fire" && point.Duration == 8), "the Crownroad Marshal owns two long fire lanes");

            List<string> completionFlags = new List<string> { StoryFlags.CrownroadMarshalDefeated };
            AssertEqual(false, ContentSetCatalog.RedGateComplete(completionFlags), "defeating the marshal alone does not skip the threshold survey");
            completionFlags.Add(StoryFlags.MeteorCrownThresholdSurveyed);
            AssertEqual(true, ContentSetCatalog.RedGateComplete(completionFlags), "marshal victory plus the surveyed threshold completes Chapter V");
            AssertEqual("red_gate_debriefed", StoryFlags.RedGateDebriefed, "the post-threshold Yara debrief has a stable save flag");

            InventoryItem warblade = ContentSetCatalog.CreateCrownwardEmberglassWarblade();
            AssertEqual("+6 Crownward Emberglass Warblade", warblade.DisplayName, "Chapter V reward has a stable player-facing identity");
            AssertEqual("weapon", warblade.Slot, "Chapter V reward is an equippable weapon rather than a hidden story token");
            AssertEqual(6, warblade.Bonus, "Chapter V reward advances beyond the Ashglass mantle tier");
            AssertEqual(8, warblade.DamageMin, "Chapter V reward keeps its exact minimum damage");
            AssertEqual(13, warblade.DamageMax, "Chapter V reward keeps its exact maximum damage");
            AssertEqual("fire", warblade.DamageType, "Chapter V reward carries its Emberglass affinity");
            AssertEqual("quest", warblade.Rarity, "Chapter V reward remains unique quest gear");

            IReadOnlyList<RoamingThreatDefinition> productionChapterFive = RoamingThreatCatalog.ForDepth(5, false);
            AssertRoamingThreatRoster(productionChapterFive, 5, 3, "production Chapter V patrol roster");
            AssertEqual(
                "inner-gate-cinder-vanguard|crownroad-ossuary-column|emberglass-drow-conclave",
                string.Join("|", productionChapterFive.Select(definition => definition.Id)),
                "production Chapter V patrol identities stay deterministic");
            AssertEqual(true, productionChapterFive.SelectMany(definition => definition.EnemyIds)
                .All(id => ContentSetCatalog.EnemyActive(ContentSetCatalog.SewerSlice, id)), "production Chapter V patrol enemies are active in its content set");
            AssertEqual(0, RoamingThreatCatalog.ForDepth(6, false).Count, "production patrol catalog keeps unfinished Chapter VI sealed");
        }

        private static void SewerSliceContentSetDefinesCompleteFirstPlayPath()
        {
            AssertEqual(33, ContentSetCatalog.SewerSliceFormulaCodes.Count, "sewer slice formula count");
            AssertEqual(25, ContentSetCatalog.SewerSliceAbilityIds.Count, "sewer slice permanent and derived ability count");
            AssertEqual(22, ContentSetCatalog.SewerSliceEnemyIds.Count, "production campaign enemy count through the Red Gate");
            AssertEqual(3, ContentSetCatalog.SewerSliceEncounters.Count, "sewer slice encounter count");
            AssertEqual(
                "OIC,TBQ,NVC,OBL,GBH,TNC,HLC,SRF,DWP,SBN,FIF,RIG,WBI,RCL,FBL,RSG,CLT,CNS,VST,ACR,AST,RKW,RNH,RBT,IBD,SLV,INH,PBR,GRH,IBF,DMC,VRS,DFA",
                string.Join(",", ContentSetCatalog.SewerSliceFormulaCodes),
                "sewer slice formulas stay grouped by spellcraft and ordered by unlock tier");

            foreach (string code in ContentSetCatalog.SewerSliceFormulaCodes)
            {
                AssertEqual(true, FormulaCatalog.All.Any(formula => formula.Code == code), "slice formula exists " + code);
                AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, code), "slice formula active " + code);
            }
            string[] campaignFormulaExpansion = { "OBL", "RCL", "INH", "HLC", "IBF", "DMC" };
            AssertEqual(true, campaignFormulaExpansion.All(FormulaCatalog.HasExplicitRequiredLevel), "expanded campaign formulas have explicit unlock levels");
            AssertEqual(true, campaignFormulaExpansion.All(code => ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, code.ToLowerInvariant())), "expanded campaign formulas resolve case-insensitively as known sewer-slice powers");
            AssertEqual(false, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, "MTR"), "meteor shower hidden in sewer slice");
            AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.FullPrototype, "MTR"), "meteor shower available in prototype");
            AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, "RKW"), "warlock bind active in sewer slice");
            AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, "IBD"), "warlock summon imp active in sewer slice");
            AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, "RBT"), "warlock Rift Bolt active in sewer slice");
            AssertEqual(true, ContentSetCatalog.FormulaActive(ContentSetCatalog.SewerSlice, "VRS"), "later Rift Step remains in the level-20 progression slice");
            AssertEqual(
                "RIG,RSG,CLT,VST,AST",
                string.Join(",", ContentSetCatalog.SewerSliceFormulaCodes.Where(code => new[] { "RIG", "RSG", "CLT", "VST", "AST" }.Contains(code))),
                "sewer slice exposes the complete lightning progression in tier order");

            foreach (string id in ContentSetCatalog.SewerSliceAbilityIds)
            {
                AssertEqual(true, AbilityCatalog.For(id) != null, "slice ability exists " + id);
                AssertEqual(true, ContentSetCatalog.AbilityActive(ContentSetCatalog.SewerSlice, id), "slice ability active " + id);
            }
            AssertEqual(true, ContentSetCatalog.AbilityActive(ContentSetCatalog.SewerSlice, "whirlwind"), "whirlwind remains in the level-20 progression slice");
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

        private static void InventoryIdentitySaveRoundTrip()
        {
            string root = Path.Combine(Path.GetTempPath(), "AshenHallsInventoryIdentityRoundTrip-" + Guid.NewGuid().ToString("N"));
            try
            {
                string path = SaveService.SavePath(root);
                InventoryItem exactWeapon = new InventoryItem
                {
                    InstanceId = "item-roundtrip-weapon",
                    EquippedById = "party-roundtrip",
                    Slot = "weapon",
                    Form = "broadsword",
                    DisplayName = "stormbound round-trip blade",
                    PermanentEnchantmentId = "storm",
                    DamageMin = 4,
                    DamageMax = 9,
                    AttackSpeed = 7
                };
                InventoryItem identicalSibling = new InventoryItem
                {
                    InstanceId = "item-roundtrip-sibling",
                    Slot = "weapon",
                    Form = exactWeapon.Form,
                    DisplayName = exactWeapon.DisplayName,
                    PermanentEnchantmentId = exactWeapon.PermanentEnchantmentId,
                    DamageMin = exactWeapon.DamageMin,
                    DamageMax = exactWeapon.DamageMax,
                    AttackSpeed = exactWeapon.AttackSpeed
                };
                InventoryItem exactArmor = new InventoryItem
                {
                    InstanceId = "item-roundtrip-armor",
                    EquippedById = "party-roundtrip",
                    Slot = "armor",
                    Form = "mail",
                    DisplayName = "round-trip road mail",
                    Bonus = 3,
                    HealthBonus = 5
                };
                GameState state = new GameState
                {
                    SaveVersion = VersionInfo.SaveVersion,
                    ContentSetId = ContentSetCatalog.SewerSlice,
                    Seed = 727,
                    Party = new List<PartyMember>
                    {
                        new PartyMember
                        {
                            Id = "party-roundtrip",
                            Name = "Round Trip",
                            WeaponItemId = exactWeapon.InstanceId,
                            WeaponName = exactWeapon.DisplayName,
                            ArmorItemId = exactArmor.InstanceId,
                            ArmorName = exactArmor.DisplayName
                        }
                    },
                    Inventory = new List<InventoryItem> { identicalSibling, exactWeapon, exactArmor }
                };

                SaveService.SaveGameState(path, state);
                GameState loaded = SaveService.LoadGameState(path, out bool usedBackup);
                AssertEqual(false, usedBackup, "inventory identity round trip uses the primary save");
                AssertEqual(3, loaded.Inventory.Count, "inventory identity round trip preserves duplicate-looking items and exact armor");
                AssertEqual("item-roundtrip-weapon", loaded.Party[0].WeaponItemId, "member weapon reference survives save round trip");
                AssertEqual("item-roundtrip-armor", loaded.Party[0].ArmorItemId, "member armor reference survives save round trip");
                InventoryItem loadedWeapon = InventoryItemIdentityRules.FindById(loaded.Inventory, loaded.Party[0].WeaponItemId);
                InventoryItem loadedArmor = InventoryItemIdentityRules.FindById(loaded.Inventory, loaded.Party[0].ArmorItemId);
                AssertEqual(true, loadedWeapon != null, "member reference resolves after save round trip");
                AssertEqual(true, loadedArmor != null && loadedArmor.EquippedById == "party-roundtrip", "member armor reference resolves to the exact reciprocal item after save round trip");
                AssertEqual("party-roundtrip", loadedWeapon.EquippedById, "compatibility owner mirror survives save round trip");
                AssertEqual("storm", loadedWeapon.PermanentEnchantmentId, "enchantment identity stays attached to the exact physical item");
                AssertEqual("", InventoryItemIdentityRules.FindById(loaded.Inventory, "item-roundtrip-sibling").EquippedById ?? "", "identical sibling remains unowned after save round trip");
                AssertEqual(true, InventoryItemIdentityRules.HasUniqueInstanceIds(loaded.Inventory), "save round trip keeps IDs unique");

                GameState v26Candidate = new GameState
                {
                    SaveVersion = 26,
                    Party = new List<PartyMember> { new PartyMember { Id = "legacy-party", Name = "Legacy" } },
                    Inventory = new List<InventoryItem> { new InventoryItem { DisplayName = "legacy item" } }
                };
                AssertEqual(true, SaveCandidateRules.IsLoadable(v26Candidate, VersionInfo.SaveVersion), "v26 saves remain eligible for identity migration before strict validation");
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
