using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private void NewMuster()
        {
            CancelCombatResolutionBeat(false);
            int seed = Environment.TickCount;
            rng = new System.Random(seed);
            activeContentSet = ContentSetCatalog.SewerSlice;
            state = new GameState
            {
                SaveVersion = SaveVersion,
                ContentSetId = ContentSetCatalog.SewerSlice,
                Mode = GameMode.Tavern,
                Depth = 1,
                Seed = seed,
                Gold = 28,
                Supplies = 5,
                Elixirs = 3,
                StoryChapter = 1,
                ActiveStory = StoryObjectiveForDepth(1),
                DiscoveredZones = new List<string>(),
                ReducedMotion = false,
                SfxMuted = false,
                MusicMuted = false,
                SfxVolumePercent = 100,
                MusicVolumePercent = 65,
                Party = MakeDefaultParty(),
                Inventory = new List<InventoryItem>(),
                StoryFlags = new List<string>(),
                Log = new List<LogEntry>()
            };
            InvalidateControllerCaches();
            SetActiveContentSet(ContentSetCatalog.SewerSlice);
            NormalizeGameSettings();
            EnsurePartyCustomization();
            betaLabMode = false;
            labSaveBlocked = false;
            CloseTransientOverlays();
            selectedBuilderIndex = 0;
            PushLog("Four names gather at the tavern table.", Tone.Good);
        }

        private void BeginGame()
        {
            SetActiveContentSet(ContentSetCatalog.SewerSlice);
            EnsurePartyCustomization();
            if (!state.Party.All(p => p.Stats.Total == StatPointBudget))
            {
                PushLog($"Each character needs exactly {StatPointBudget} attribute points.", Tone.Warn);
                ShowBanner("Muster incomplete");
                PlaySfx("blocked", 0.55f);
                return;
            }

            CloseTransientOverlays();
            state.Mode = GameMode.Explore;
            betaLabMode = false;
            labSaveBlocked = false;
            exploreHudCollapsed = true;
            exploreWideView = false;
            state.Depth = 1;
            state.StoryChapter = 1;
            state.ActiveStory = StoryObjectiveForDepth(state.Depth);
            if (state.StoryFlags == null) state.StoryFlags = new List<string>();
            state.StoryFlags.Clear();
            if (state.DiscoveredZones == null) state.DiscoveredZones = new List<string>();
            state.DiscoveredZones.Clear();
            state.Map = GenerateMap(state.Depth, state.Seed);
            InvalidateControllerCaches();
            EnsureWorldLandmarks();
            PlacePlayerAtExplorationStart();
            lastExploreRegion = ExploreRegionName(state.PlayerX, state.PlayerY);
            DiscoverCurrentZone(true);
            PushLog($"The party leaves {HomeTownName}'s gate lamps and enters {GameTitle}: {GameSubtitle}.", Tone.Good);
            PushLog(state.ActiveStory, Tone.Normal);
            PushLog("First steps: follow the gold marker to King's Hall. Move with WASD or arrows; use Space or E beside a marked location.", Tone.Good);
            ShowBanner("First Steps: King's Hall");
            PlaySfx("uiconfirm", 0.55f);
            QueueSfx("shrine", 0.09f, 0.38f);
            AutosaveCheckpoint("new party reaches Midgaard");
        }

        private void QuickStart()
        {
            state.Party = MakeDefaultParty();
            BeginGame();
        }

        private void StartNewGame()
        {
            NewMuster();
            state.Mode = GameMode.Muster;
            showTavernSettings = false;
            showTavernTesting = false;
            PlaySfx("ui", 0.55f);
        }

        private void ContinueSavedGame()
        {
            if (!HasSavedGame())
            {
                PushLog("No saved oath is present.", Tone.Warn);
                ShowBanner("No save found");
                PlaySfx("ui", 0.45f);
                return;
            }

            showTavernSettings = false;
            showTavernTesting = false;
            LoadGame();
        }

        private bool HasSavedGame()
        {
            if (hasSavedGameCacheValid) return cachedHasSavedGame;
            try
            {
                cachedHasSavedGame = SaveService.SaveExists(SavePath());
            }
            catch
            {
                cachedHasSavedGame = false;
            }
            hasSavedGameCacheValid = true;
            return cachedHasSavedGame;
        }

        private static bool DeveloperTestingBuildEnabled()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return true;
#else
            return false;
#endif
        }

        private static bool DeveloperTestingHotkeyPressed()
        {
            bool control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            return control && shift && Input.GetKeyDown(KeyCode.B);
        }

        private void OpenDeveloperTestingShortcut()
        {
            if (TavernMenuRules.ShowDeveloperTesting(DeveloperTestingBuildEnabled()))
            {
                ToggleTavernTesting();
                return;
            }

            StartBetaCombatLab();
        }

        private void StartBetaCombatLab()
        {
            SetActiveContentSet(ContentSetCatalog.FullPrototype);
            state.Party = MakeDefaultParty();
            EnsurePartyCustomization();
            state.Depth = 3;
            state.Gold = Mathf.Max(state.Gold, 160);
            state.Supplies = Mathf.Max(state.Supplies, 8);
            state.Elixirs = Mathf.Max(state.Elixirs, 8);
            foreach (PartyMember member in state.Party)
            {
                member.Hp = member.MaxHp;
                member.Mana = member.MaxMana;
                if (!string.IsNullOrEmpty(member.Spell))
                {
                    BoostTalent(member, PrimarySpellSchool(member.Spell));
                    member.Mana = member.MaxMana + 6;
                    member.MaxMana = member.Mana;
                }
            }
            PrepareBetaSpellLabParty();

            state.Map = GenerateMap(state.Depth, state.Seed);
            InvalidateControllerCaches();
            EnsureWorldLandmarks();
            PlacePlayerAtExplorationStart();
            state.StoryChapter = 3;
            state.ActiveStory = StoryObjectiveForDepth(state.Depth);
            if (state.DiscoveredZones == null) state.DiscoveredZones = new List<string>();
            betaLabMode = true;
            labSaveBlocked = true;
            StartCombat("lab");
            PushLog("Beta Combat Lab: caster-heavy battle loaded for stress testing spells, hazards, enemy magic, and audio.", Tone.Good);
            PushLog("Spell Lab craft is enabled here: use Mage for ember/fire/cold/shock testing, Craft for all casters, and Vesh for priest wards and mending.", Tone.Good);
            ShowBanner(BuildStage);
            TestSfx();
        }

        private void StartMartialCombatLab()
        {
            SetActiveContentSet(ContentSetCatalog.FullPrototype);
            state.Party = MakeDefaultParty();
            EnsurePartyCustomization();
            state.Depth = 3;
            state.Gold = Mathf.Max(state.Gold, 160);
            state.Supplies = Mathf.Max(state.Supplies, 8);
            state.Elixirs = Mathf.Max(state.Elixirs, 8);
            foreach (PartyMember member in state.Party)
            {
                if (member.ClassKey == "warrior" || member.ClassKey == "rogue" || member.ClassKey == "ranger")
                {
                    PromoteMemberForMartialTesting(member);
                }
                member.Hp = member.MaxHp;
                member.Mana = member.MaxMana;
            }

            state.Map = GenerateMap(state.Depth, state.Seed);
            InvalidateControllerCaches();
            EnsureWorldLandmarks();
            PlacePlayerAtExplorationStart();
            state.StoryChapter = 3;
            state.ActiveStory = "Martial Lab: test warrior, rogue, and ranger skills against staged melee and ranged targets.";
            if (state.DiscoveredZones == null) state.DiscoveredZones = new List<string>();
            betaLabMode = true;
            labSaveBlocked = true;
            StartCombat("martiallab");
            PushLog("Martial Lab: warrior, rogue, and ranger skills are unlocked and nearby targets are staged for stress testing.", Tone.Good);
            ShowBanner("Martial Lab");
            TestSfx();
        }

        private void StartKoboldRouteLab()
        {
            SetActiveContentSet(ContentSetCatalog.FullPrototype);
            state.Party = MakeDefaultParty();
            EnsurePartyCustomization();
            state.Depth = 2;
            state.StoryChapter = 2;
            state.Gold = Mathf.Max(state.Gold, 120);
            state.Supplies = Mathf.Max(state.Supplies, 7);
            state.Elixirs = Mathf.Max(state.Elixirs, 6);
            state.StoryFlags = new List<string>();
            foreach (PartyMember member in state.Party)
            {
                member.Hp = member.MaxHp;
                member.Mana = member.MaxMana;
            }

            state.Map = GenerateMap(state.Depth, state.Seed);
            InvalidateControllerCaches();
            EnsureWorldLandmarks();
            MapObject cave = FindKoboldStoryCave();
            Point routeAnchor = FindCriticalRouteAnchor(state.Map);
            bool caveReachable = cave != null
                && routeAnchor != null
                && ExplorationTraversalRules.CanReachObject(
                    ExplorationTraversalRules.ReachableMask(state.Map, routeAnchor.X, routeAnchor.Y),
                    state.Map,
                    cave);
            Debug.Log($"{VersionInfo.ProductName} route smoke: Dusk Market cave present={cave != null}, reachable={caveReachable}.");
            Point start = cave == null ? null : BestOpenNeighbor(cave.X, cave.Y);
            if (start != null)
            {
                state.PlayerX = start.X;
                state.PlayerY = start.Y;
            }
            else
            {
                PlacePlayerAtExplorationStart();
            }
            if (state.DiscoveredZones == null) state.DiscoveredZones = new List<string>();
            state.DiscoveredZones.Clear();
            lastExploreRegion = ExploreRegionName(state.PlayerX, state.PlayerY);
            DiscoverCurrentZone(true);
            state.Mode = GameMode.Explore;
            exploreHudCollapsed = true;
            exploreWideView = false;
            betaLabMode = false;
            labSaveBlocked = true;
            CloseTransientOverlays();
            state.ActiveStory = "Chapter II Route Lab: move once in the Dusk Market to trigger the ambush, then use the cave for the smoke-cave and king fights.";
            PushLog(state.ActiveStory, Tone.Good);
            ShowBanner("Kobold Route Lab");
            PlaySfx("encounter", 0.72f);
        }

        private List<PartyMember> MakeDefaultParty()
        {
            return StarterPartyCatalog.All
                .Select(hero => MakeHero(hero.Name, hero.Race, hero.ClassKey, hero.Role, hero.Stats, hero.Spell, hero.Range, hero.CreateSkills()))
                .ToList();
        }

        private void EnsureTavernScreen()
        {
            if (tavernScreen != null) return;
            GameObject screen = new GameObject("Tavern Screen");
            screen.transform.SetParent(transform, false);
            TavernScreen created = screen.AddComponent<TavernScreen>();
            created.Bind(new TavernScreenBindings
            {
                Title = GameTitle,
                Subtitle = "A warm door before the Old Road",
                VersionLine = () => $"{PackageVersion} / {BuildStage}",
                BackdropArt = tavernBackdropArt,
                TitleArt = titleCardArt,
                HasSavedGame = HasSavedGame,
                SettingsVisible = () => showTavernSettings,
                TestingVisible = () => showTavernTesting,
                DeveloperTestingVisible = () => TavernMenuRules.ShowDeveloperTesting(DeveloperTestingBuildEnabled()),
                AudioMuted = () => state != null && state.SfxMuted,
                MusicMuted = () => state != null && state.MusicMuted,
                VolumePercent = () => state == null ? 100 : state.SfxVolumePercent,
                MusicVolumePercent = () => state == null ? 65 : state.MusicVolumePercent,
                ReducedMotion = () => state != null && state.ReducedMotion,
                Continue = ContinueSavedGame,
                NewGame = StartNewGame,
                ToggleSettings = ToggleTavernSettings,
                ToggleTesting = ToggleTavernTesting,
                Quit = RequestQuitFromTavern,
                CloseSettings = CloseTavernSettings,
                ToggleAudio = ToggleSfxMute,
                ToggleMusic = ToggleMusicMute,
                VolumeDown = () => AdjustSfxVolume(-25),
                VolumeUp = () => AdjustSfxVolume(25),
                MusicVolumeDown = () => AdjustMusicVolume(-25),
                MusicVolumeUp = () => AdjustMusicVolume(25),
                ToggleReducedMotion = ToggleReducedMotionSetting,
                BetaLab = StartBetaCombatLab,
                MartialLab = StartMartialCombatLab,
                KoboldLab = StartKoboldRouteLab
            });
            created.SetVisible(false);
            tavernScreen = created;
        }

        private void SyncTavernScreen()
        {
            if (tavernScreen == null) return;
            bool visible = state != null && state.Mode == GameMode.Tavern && !ShouldShowStartupSplash();
            tavernScreen.SetVisible(visible);
            if (visible && ShouldRefreshPresentation(ref lastTavernRefreshKey, TavernRefreshKey())) tavernScreen.Refresh();
        }

        private string TavernRefreshKey()
        {
            bool saveExists = HasSavedGame();
            bool muted = state != null && state.SfxMuted;
            bool musicMuted = state != null && state.MusicMuted;
            int volume = state == null ? 100 : state.SfxVolumePercent;
            int musicVolume = state == null ? 65 : state.MusicVolumePercent;
            bool reducedMotion = state != null && state.ReducedMotion;
            return "save=" + saveExists
                + "|settings=" + showTavernSettings
                + "|testing=" + showTavernTesting
                + "|dev=" + TavernMenuRules.ShowDeveloperTesting(DeveloperTestingBuildEnabled())
                + "|audio=" + muted + ":" + musicMuted + ":" + volume + ":" + musicVolume + ":" + reducedMotion;
        }

        private void ToggleTavernSettings()
        {
            showTavernSettings = !showTavernSettings;
            showTavernTesting = false;
            PlaySfx("ui", 0.45f);
            MarkUiDirty();
        }

        private void CloseTavernSettings()
        {
            showTavernSettings = false;
            PlaySfx("ui", 0.45f);
            MarkUiDirty();
        }

        private void ToggleTavernTesting()
        {
            if (!TavernMenuRules.ShowDeveloperTesting(DeveloperTestingBuildEnabled())) return;
            showTavernTesting = !showTavernTesting;
            showTavernSettings = false;
            ShowBanner(showTavernTesting ? "Testing doors" : "Tavern");
            PlaySfx("uiopen", 0.55f);
            MarkUiDirty();
        }

        private void ToggleReducedMotionSetting()
        {
            if (state == null) return;
            state.ReducedMotion = !state.ReducedMotion;
            if (state.ReducedMotion) ClearCombatMotionForReducedMotion();
            ShowBanner(state.ReducedMotion ? "Reduced motion on" : "Reduced motion off");
            PlaySfx("ui", 0.45f);
        }

        private void RequestQuitFromTavern()
        {
            PlaySfx("ui", 0.6f);
            Application.Quit();
            ShowBanner("Exit requested");
        }

        private PartyMember MakeHero(string name, string race, string classKey, string role, Stats stats, string spell, int range, SkillSet skills)
        {
            PartyMember member = new PartyMember
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = name,
                Role = role,
                Race = race,
                ClassKey = classKey,
                Origin = DefaultOrigin(name),
                SpriteColor = RoleColor(role).ToHex(),
                Sigil = DefaultSigil(role),
                Stats = stats,
                Level = 1,
                Experience = 0,
                SkillPoints = 0,
                StatPoints = 0,
                Range = range,
                Spell = spell,
                Skills = skills.Normalize(),
                WeaponName = StartingWeapon(role),
                WeaponBonus = 0,
                WeaponDamageType = StartingWeaponDamageType(role),
                ArmorName = StartingArmor(role),
                ArmorBonus = StartingArmorBonus(role)
            };
            ApplyStarterGearStats(member);
            RecalculateMember(member);
            member.Hp = member.MaxHp;
            member.Mana = member.MaxMana;
            return member;
        }

        private void PromoteMemberForMartialTesting(PartyMember member)
        {
            if (member == null) return;
            member.Level = Mathf.Max(member.Level, 3);
            if (member.Skills == null) member.Skills = StartingSkills(member.ClassKey).Normalize();
            if (member.ClassKey == "warrior")
            {
                member.Skills.Arms = Mathf.Max(member.Skills.Arms, 18);
                member.Skills.Guard = Mathf.Max(member.Skills.Guard, 12);
            }
            else if (member.ClassKey == "rogue")
            {
                member.Skills.Arms = Mathf.Max(member.Skills.Arms, 18);
                member.Skills.Missile = Mathf.Max(member.Skills.Missile, 10);
            }
            else if (member.ClassKey == "ranger")
            {
                member.Skills.Missile = Mathf.Max(member.Skills.Missile, 20);
                member.Skills.Arms = Mathf.Max(member.Skills.Arms, 8);
            }
            RecalculateMember(member);
        }

        private void EnsurePartySetupScreen()
        {
            if (partySetupScreen != null) return;
            GameObject screen = new GameObject("Party Setup Screen");
            screen.transform.SetParent(transform, false);
            PartySetupScreen created = screen.AddComponent<PartySetupScreen>();
            created.Bind(new PartySetupScreenBindings
            {
                Title = GameTitle,
                Subtitle = GameSubtitle,
                BackdropArt = tavernBackdropArt,
                SummaryLine = () => $"Tavern Muster / {PartySummaryLine()}",
                WeaknessLine = PartyWeaknessLine,
                Members = PartySetupMemberViews,
                SelectedIndex = () => selectedBuilderIndex,
                SelectedMember = SelectedPartySetupMemberView,
                SelectMember = SelectPartySetupMember,
                Begin = BeginGame,
                QuickStart = QuickStart,
                BackToTavern = ReturnToTavernFromMuster,
                SetName = SetSelectedMemberName,
                CycleClass = CycleSelectedClass,
                CycleRace = () => CycleRace(SelectedBuilderMember()),
                CycleOrigin = () => CycleOrigin(SelectedBuilderMember()),
                CycleSigil = () => CycleSigil(SelectedBuilderMember()),
                RandomName = RandomizeSelectedMemberName,
                RerollGear = () => RerollGear(SelectedBuilderMember()),
                RerollLook = () => RerollLook(SelectedBuilderMember()),
                CycleColor = () => CycleColor(SelectedBuilderMember()),
                ChangeStat = ChangeSelectedStat,
                BoostTalent = key => BoostTalent(SelectedBuilderMember(), key)
            });
            created.SetVisible(false);
            partySetupScreen = created;
        }

        private void SyncPartySetupScreen()
        {
            if (partySetupScreen == null) return;
            bool visible = state != null && state.Mode == GameMode.Muster && !ShouldShowStartupSplash();
            partySetupScreen.SetVisible(visible);
            if (visible && ShouldRefreshPresentation(ref lastPartySetupRefreshKey, PartySetupRefreshKey())) partySetupScreen.Refresh();
        }

        private IReadOnlyList<PartySetupMemberView> PartySetupMemberViews()
        {
            if (state?.Party == null) return Array.Empty<PartySetupMemberView>();
            return state.Party.Select(MakePartySetupMemberView).ToList();
        }

        private string PartySetupRefreshKey()
        {
            if (state?.Party == null) return "empty|" + selectedBuilderIndex;
            int hash = selectedBuilderIndex;
            foreach (PartyMember member in state.Party)
            {
                if (member == null) continue;
                hash = unchecked(hash * 31 + (member.Name ?? "").GetHashCode());
                hash = unchecked(hash * 31 + (member.Race ?? "").GetHashCode());
                hash = unchecked(hash * 31 + (member.ClassKey ?? "").GetHashCode());
                hash = unchecked(hash * 31 + member.Stats.Strength);
                hash = unchecked(hash * 31 + member.Stats.Intelligence);
                hash = unchecked(hash * 31 + member.Stats.Dexterity);
                hash = unchecked(hash * 31 + member.Stats.Health);
                hash = unchecked(hash * 31 + member.StatPoints);
                hash = unchecked(hash * 31 + (member.SpriteColor ?? "").GetHashCode());
                hash = unchecked(hash * 31 + (member.WeaponName ?? "").GetHashCode());
                hash = unchecked(hash * 31 + (member.ArmorName ?? "").GetHashCode());
            }
            return "partySetup=" + hash;
        }

        private PartySetupMemberView SelectedPartySetupMemberView()
        {
            PartyMember member = SelectedBuilderMember();
            return member == null ? null : MakePartySetupMemberView(member);
        }

        private PartySetupMemberView MakePartySetupMemberView(PartyMember member)
        {
            int total = member.Stats.Total;
            int cap = Mathf.Max(StatPointBudget, total + Mathf.Max(0, member.StatPoints));
            return new PartySetupMemberView
            {
                Name = member.Name,
                RaceClassLine = $"{DisplayRace(member.Race)} / {DisplayClass(member.ClassKey)}",
                RoleLine = RoleIdentityLine(member),
                GearLine = GearShortLine(member),
                ProgressLine = ProgressLine(member) + " / " + EffectiveStatsLine(member),
                UnlockLine = ProgressionUnlockLine(member),
                BestSkillLine = $"{BestSkillLabel(member)} {BestSkillValue(member)} ({SkillAdjective(BestSkillValue(member))})",
                ColorHex = member.SpriteColor,
                Strength = member.Stats.Strength,
                Intelligence = member.Stats.Intelligence,
                Agility = member.Stats.Dexterity,
                Health = member.Stats.Health,
                StatTotal = total,
                StatCap = cap
            };
        }

        private PartyMember SelectedBuilderMember()
        {
            if (state?.Party == null || state.Party.Count == 0) return null;
            selectedBuilderIndex = Mathf.Clamp(selectedBuilderIndex, 0, state.Party.Count - 1);
            return state.Party[selectedBuilderIndex];
        }

        private void SelectPartySetupMember(int index)
        {
            if (state?.Party == null || state.Party.Count == 0) return;
            selectedBuilderIndex = Mathf.Clamp(index, 0, state.Party.Count - 1);
            PlaySfx("uitab", 0.35f);
        }

        private void ReturnToTavernFromMuster()
        {
            if (state == null) return;
            CloseTransientOverlays();
            state.Mode = GameMode.Tavern;
            PlaySfx("uiclose", 0.45f);
        }

        private void SetSelectedMemberName(string name)
        {
            PartyMember member = SelectedBuilderMember();
            if (member == null) return;
            string trimmed = string.IsNullOrWhiteSpace(name) ? RandomName(member.Role) : name.Trim();
            member.Name = trimmed.Length > 16 ? trimmed.Substring(0, 16) : trimmed;
        }

        private void CycleSelectedClass()
        {
            PartyMember member = SelectedBuilderMember();
            if (member == null) return;
            int idx = Array.IndexOf(classOrder, member.ClassKey);
            ApplyClass(member, classOrder[(idx + 1 + classOrder.Length) % classOrder.Length]);
            PlaySfx("ui", 0.45f);
        }

        private void RandomizeSelectedMemberName()
        {
            PartyMember member = SelectedBuilderMember();
            if (member == null) return;
            member.Name = RandomName(member.Role);
            PlaySfx("ui", 0.35f);
        }

        private void ChangeSelectedStat(int code, int delta)
        {
            ChangeStat(SelectedBuilderMember(), code, delta);
            PlaySfx("ui", 0.25f);
        }

        private bool CanBoostTalent(PartyMember member, string key)
        {
            if (member == null) return false;
            if (member.Skills == null) member.Skills = new SkillSet().Normalize();
            if (member.SkillPoints > 0) return SkillValue(member.Skills, key) < 99;
            bool freshRecruit = member.Level <= 1 && member.Experience <= 0 && member.StatPoints <= 0;
            return freshRecruit && SkillValue(member.Skills, key) < 12;
        }

        private void BoostTalent(PartyMember member, string key)
        {
            if (member == null) return;
            if (member.Skills == null) member.Skills = new SkillSet().Normalize();
            int current = SkillValue(member.Skills, key);
            if (member.SkillPoints > 0)
            {
                SetSkill(member.Skills, key, Mathf.Clamp(current + 2, 1, 99));
                member.SkillPoints = Mathf.Max(0, member.SkillPoints - 1);
                PushLog($"{member.Name} trains {key} to {SkillValue(member.Skills, key)}.", Tone.Good);
                PlaySfx("ui", 0.45f);
                return;
            }

            if (!CanBoostTalent(member, key)) return;
            SetSkill(member.Skills, key, Mathf.Clamp(Mathf.Max(10, current + 2), 1, 12));
            PlaySfx("ui", 0.35f);
        }

        private void RerollGear(PartyMember member)
        {
            if (member == null) return;
            InventoryItem weapon = MakeRoleItem(member.Role, true, true);
            InventoryItem armor = MakeRoleItem(member.Role, false, true);
            member.WeaponName = weapon.DisplayName;
            member.WeaponBonus = weapon.Bonus;
            member.WeaponDamageType = string.IsNullOrEmpty(weapon.DamageType) ? "physical" : weapon.DamageType;
            member.WeaponDamageMin = Mathf.Max(1, weapon.DamageMin);
            member.WeaponDamageMax = Mathf.Max(member.WeaponDamageMin + 1, weapon.DamageMax);
            member.WeaponAttackSpeed = Mathf.Max(1, weapon.AttackSpeed);
            ApplyGearStatBonuses(member, weapon, true);
            member.Range = WeaponRange(weapon, member);
            member.ArmorName = armor.DisplayName;
            member.ArmorBonus = ArmorDefenseBonus(armor);
            ApplyGearStatBonuses(member, armor, false);
            RecalculateMember(member);
            PushLog($"{member.Name} tries a new kit: {TrimGearName(member.WeaponName)} / {TrimGearName(member.ArmorName)}.", Tone.Normal);
            PlaySfx("cache", 0.45f);
        }

        private void RerollLook(PartyMember member)
        {
            if (member == null) return;
            member.Origin = originOrder[rng.Next(originOrder.Length)];
            member.Sigil = sigilOrder[rng.Next(sigilOrder.Length)];
            member.SpriteColor = accentPalette[rng.Next(accentPalette.Length)];
            PushLog($"{member.Name} changes colors and sigil.", Tone.Normal);
            PlaySfx("ui", 0.55f);
        }

        private string PartySummaryLine()
        {
            if (state?.Party == null) return $"Muster a {PartySize}-person tavern party.";
            int front = state.Party.Count(p => p.Role == "shield" || p.Role == "ward" || p.Role == "pike");
            int ranged = state.Party.Count(p => p.Role == "bow" || p.Range >= 3);
            int priests = state.Party.Count(p => CasterKnowsSchool(p.Spell, "mend") || p.Role == "mender");
            int arcanists = state.Party.Count(p => CasterKnowsSchool(p.Spell, "ember") || CasterKnowsSchool(p.Spell, "hex"));
            int level = state.Party.Count == 0 ? 1 : Mathf.Max(1, Mathf.RoundToInt((float)state.Party.Sum(p => Mathf.Max(1, p.Level)) / state.Party.Count));
            return $"{PartySize}-person party: front {front} / ranged {ranged} / priest {priests} / arcane {arcanists} / avg level {level}. {PartyWeaknessLine()}";
        }

        private string PartyWeaknessLine()
        {
            if (state?.Party == null) return "";
            List<string> gaps = new List<string>();
            if (state.Party.Count(p => p.Role == "shield" || p.Role == "ward" || p.Role == "pike") < 1) gaps.Add("thin front");
            if (!state.Party.Any(p => p.Role == "bow" || p.Range >= 3)) gaps.Add("no ranged pressure");
            if (!state.Party.Any(p => CasterKnowsSchool(p.Spell, "mend") || p.Role == "mender")) gaps.Add("no priest");
            if (!state.Party.Any(p => CasterKnowsSchool(p.Spell, "ember") || CasterKnowsSchool(p.Spell, "hex"))) gaps.Add("no arcane caster");
            if (state.Party.Count(p => EffectiveHealth(p) <= 9) >= 2) gaps.Add("fragile");
            return gaps.Count == 0 ? "No obvious gaps." : "Watch: " + string.Join(", ", gaps) + ".";
        }

        private string RoleIdentityLine(PartyMember member)
        {
            if (member == null) return "";
            string role = member.Role ?? "";
            string header = $"L{Mathf.Max(1, member.Level)} {DisplayClass(member.ClassKey)}";
            if (role == "shield") return $"{header} / front guard / HP {member.MaxHp} / guard {member.Skills.Guard}";
            if (role == "ward") return $"{header} / oath guard / armor {member.ArmorBonus} / guard {member.Skills.Guard}";
            if (role == "pike") return $"reach line / range {member.Range} / arms {member.Skills.Arms}";
            if (role == "bow") return $"ranged pressure / range {member.Range} / missile {member.Skills.Missile}";
            if (role == "knife") return $"{header} / light striker / speed {member.AttackSpeed} / arms {member.Skills.Arms}";
            if (role == "mender") return $"{header} / priest spells / MP {member.MaxMana} / mend {member.Skills.Mend}";
            if (role == "ember") return $"{header} / arcane spells / MP {member.MaxMana} / ember {member.Skills.Ember}";
            if (role == "hex" && member.ClassKey == "warlock") return $"{header} / dark arts / MP {member.MaxMana} / summon+hex {member.Skills.Hex}";
            if (role == "hex") return $"{header} / hex spells / MP {member.MaxMana} / hex {member.Skills.Hex}";
            return $"{header} / HP {member.MaxHp} / MP {member.MaxMana}";
        }

        private void ChangeStat(PartyMember member, int code, int delta)
        {
            if (member == null) return;
            bool spendsEarnedPoint = delta > 0 && member.Stats.Total >= StatPointBudget;
            if (spendsEarnedPoint && member.StatPoints <= 0) return;
            if (delta < 0)
            {
                int current = GetStat(member.Stats, code);
                if (current <= 3) return;
            }

            switch (code)
            {
                case -1: member.Stats.Strength += delta; break;
                case -2: member.Stats.Intelligence += delta; break;
                case -3: member.Stats.Dexterity += delta; break;
                case -4: member.Stats.Health += delta; break;
            }
            if (spendsEarnedPoint) member.StatPoints = Mathf.Max(0, member.StatPoints - 1);
            if (delta < 0 && member.Stats.Total >= StatPointBudget) member.StatPoints++;
            RecalculateMember(member);
        }

        private int GetStat(Stats stats, int code)
        {
            switch (code)
            {
                case -1: return stats.Strength;
                case -2: return stats.Intelligence;
                case -3: return stats.Dexterity;
                default: return stats.Health;
            }
        }

        private void ApplyRole(PartyMember member, string role)
        {
            if (member == null) return;
            string classKey = ClassForRole(role);
            ApplyClass(member, classKey);
        }

        private void ApplyClass(PartyMember member, string classKey)
        {
            if (member == null) return;
            member.ClassKey = string.IsNullOrWhiteSpace(classKey) ? "warrior" : classKey;
            member.Role = RoleForClass(member.ClassKey);
            member.Spell = SpellForClass(member.ClassKey);
            member.Range = StartingRange(member.Role);
            member.Skills = StartingSkills(member.ClassKey).Normalize();
            member.WeaponName = StartingWeapon(member.Role);
            member.WeaponBonus = 0;
            member.WeaponDamageType = StartingWeaponDamageType(member.Role);
            member.ArmorName = StartingArmor(member.Role);
            member.ArmorBonus = StartingArmorBonus(member.Role);
            ApplyStarterGearStats(member);
            RecalculateMember(member);
        }

        private void RecalculateMember(PartyMember member)
        {
            if (member == null) return;
            if (member.Skills == null) member.Skills = new SkillSet().Normalize();
            bool missingVitalBaseline = member.MaxHp <= 0;
            member.Level = Mathf.Max(1, member.Level);
            int strength = EffectiveStrength(member);
            int intelligence = EffectiveIntelligence(member);
            int agility = EffectiveAgility(member);
            int health = EffectiveHealth(member);
            member.MaxHp = health + 16 + strength / 2 + (member.Level - 1) * 4;
            member.MaxMana = string.IsNullOrEmpty(member.Spell) ? 0 : intelligence + 8 + (member.Level - 1) * 3;
            member.Hp = missingVitalBaseline && member.Hp <= 0
                ? member.MaxHp
                : Mathf.Clamp(member.Hp, 0, member.MaxHp);
            member.Mana = Mathf.Clamp(member.Mana, 0, member.MaxMana);
            int baseMin = member.WeaponDamageMin > 0 ? member.WeaponDamageMin : 2;
            int baseMax = member.WeaponDamageMax > 0 ? member.WeaponDamageMax : 5;
            int weaponStat = WeaponPrimaryStat(member, strength, intelligence, agility);
            int weaponPowerStat = WeaponPowerStat(member, strength, intelligence, agility);
            member.DamageMin = Mathf.Max(1, baseMin + Mathf.Max(-1, member.WeaponBonus) + weaponStat / 8);
            member.DamageMax = Mathf.Max(member.DamageMin, baseMax + Mathf.Max(-1, member.WeaponBonus) + weaponStat / 5);
            member.Power = member.DamageMin + weaponPowerStat / 4 + Mathf.Max(-1, member.WeaponBonus) + WeaponPowerBonus(member.WeaponName);
            member.Defense = (health + strength) / 9 + Mathf.Max(0, member.ArmorBonus);
            member.AttackSpeed = Mathf.Clamp(member.WeaponAttackSpeed + agility / 4 + ArmorAgilityModifier(member.ArmorName), 3, 20);
            member.Agility = Mathf.Max(1, agility / 3 + 2 + ArmorAgilityModifier(member.ArmorName) + member.AttackSpeed / 8);
            member.Movement = Mathf.Clamp(CombatMoveAllowance + (agility >= 18 ? 1 : 0) + (member.AttackSpeed >= 15 ? 1 : 0) - (member.ArmorBonus >= 4 ? 1 : 0), 2, 5);
            member.Range = Mathf.Max(member.Range, StartingRange(member.Role));
        }

        private int EffectiveStrength(PartyMember member)
        {
            return Mathf.Max(1, member.Stats.Strength + RaceStatBonus(member.Race, "str") + member.GearStrength);
        }

        private int EffectiveIntelligence(PartyMember member)
        {
            return Mathf.Max(1, member.Stats.Intelligence + RaceStatBonus(member.Race, "int") + member.GearIntelligence);
        }

        private int EffectiveAgility(PartyMember member)
        {
            return Mathf.Max(1, member.Stats.Dexterity + RaceStatBonus(member.Race, "agi") + member.GearAgility);
        }

        private int EffectiveHealth(PartyMember member)
        {
            return Mathf.Max(1, member.Stats.Health + RaceStatBonus(member.Race, "hea") + member.GearHealth);
        }

        private int WeaponPrimaryStat(PartyMember member, int strength, int intelligence, int agility)
        {
            string key = WeaponPrimaryStatKey(member);
            if (key == "agi") return agility;
            if (key == "int") return intelligence;
            if (key == "finesse") return Mathf.Max(agility, (strength + agility) / 2);
            if (key == "reach") return Mathf.Max(strength, (strength + agility) / 2);
            return strength;
        }

        private int WeaponPowerStat(PartyMember member, int strength, int intelligence, int agility)
        {
            string key = WeaponPrimaryStatKey(member);
            if (key == "int") return intelligence;
            if (key == "agi") return Mathf.Max(agility, strength / 2);
            if (key == "finesse") return Mathf.Max(agility, strength);
            if (key == "reach") return Mathf.Max(strength, agility);
            return strength;
        }

        private string WeaponPrimaryStatKey(PartyMember member)
        {
            if (member == null) return "str";
            string weapon = (member.WeaponName ?? "").ToLowerInvariant();
            string role = (member.Role ?? "").ToLowerInvariant();
            string classKey = (member.ClassKey ?? "").ToLowerInvariant();
            if (role == "bow" || classKey == "ranger" || ContainsAny(weapon, "bow", "crossbow", "sling", "dart")) return "agi";
            if (role == "knife" || classKey == "rogue" || ContainsAny(weapon, "epee", "rapier", "sabre", "knife", "dagger")) return "finesse";
            if (ContainsAny(weapon, "focus", "orb", "scepter", "staff", "bell") || role == "mender" || role == "ember" || role == "hex") return "int";
            if (role == "pike" || ContainsAny(weapon, "spear", "pike", "glaive", "halberd", "lance")) return "reach";
            return "str";
        }

        private string WeaponPrimaryStatLabel(PartyMember member)
        {
            switch (WeaponPrimaryStatKey(member))
            {
                case "agi": return "AGI";
                case "int": return "INT";
                case "finesse": return "AGI/STR";
                case "reach": return "STR/AGI";
                default: return "STR";
            }
        }

        private bool ContainsAny(string text, params string[] terms)
        {
            if (string.IsNullOrEmpty(text) || terms == null) return false;
            for (int i = 0; i < terms.Length; i++)
            {
                if (!string.IsNullOrEmpty(terms[i]) && text.Contains(terms[i])) return true;
            }
            return false;
        }

        private int RaceStatBonus(string race, string stat)
        {
            race = (race ?? "human").ToLowerInvariant();
            if (race == "human") return stat == "hea" ? 1 : 0;
            if (race == "dusk elf") return stat == "agi" ? 2 : stat == "hea" ? -1 : 0;
            if (race == "stoneborn") return stat == "str" || stat == "hea" ? 2 : stat == "agi" ? -1 : 0;
            if (race == "fenkin") return stat == "int" || stat == "agi" ? 1 : 0;
            if (race == "ashling") return stat == "int" ? 2 : stat == "hea" ? -1 : 0;
            return 0;
        }

        private string EffectiveStatsLine(PartyMember member)
        {
            if (member == null) return "";
            return $"Stats: STR {EffectiveStrength(member)} / INT {EffectiveIntelligence(member)} / AGI {EffectiveAgility(member)} / HP {EffectiveHealth(member)}";
        }

        private string ProgressLine(PartyMember member)
        {
            if (member == null) return "";
            int next = ExperienceForNextLevel(member.Level);
            return $"Level {member.Level} / XP {member.Experience}/{next} / unspent stat {member.StatPoints} skill {member.SkillPoints}";
        }

        private string ProgressionUnlockLine(PartyMember member)
        {
            if (member == null) return "";
            List<string> parts = new List<string>();
            int abilities = AbilityIdsForClass(member.ClassKey).Select(AbilityDef).Count(a => a != null && member.Level >= a.RequiredLevel);
            int spells = string.IsNullOrEmpty(member.Spell) ? 0 : ActiveFormulaBook().Count(f => SchoolMatches(f, member.Spell) && member.Level >= FormulaRequiredLevel(f));
            if (abilities > 0) parts.Add($"{abilities} combat skill{(abilities == 1 ? "" : "s")}");
            if (spells > 0) parts.Add($"{spells} spell{(spells == 1 ? "" : "s")}");
            string current = parts.Count == 0 ? "No learned powers yet" : "Learned: " + string.Join(" / ", parts);
            string next = NextProgressionUnlock(member);
            return string.IsNullOrEmpty(next) ? current : $"{current} / Next: {next}";
        }

        private string NextProgressionUnlock(PartyMember member)
        {
            if (member == null) return "";
            List<Tuple<int, string>> unlocks = new List<Tuple<int, string>>();
            foreach (string id in AbilityIdsForClass(member.ClassKey))
            {
                MartialAbility ability = AbilityDef(id);
                if (ability != null && ability.RequiredLevel > member.Level) unlocks.Add(Tuple.Create(ability.RequiredLevel, ability.Name));
            }
            if (!string.IsNullOrEmpty(member.Spell))
            {
                foreach (FormulaDef formula in ActiveFormulaBook())
                {
                    int required = FormulaRequiredLevel(formula);
                    if (required > member.Level && SchoolMatches(formula, member.Spell)) unlocks.Add(Tuple.Create(required, formula.Name));
                }
            }
            Tuple<int, string> next = unlocks.OrderBy(u => u.Item1).ThenBy(u => u.Item2).FirstOrDefault();
            return next == null ? "deeper training" : $"L{next.Item1} {next.Item2}";
        }

        private int ExperienceForNextLevel(int level)
        {
            level = Mathf.Max(1, level);
            return 60 + level * level * 40;
        }

        private int StartingRange(string role)
        {
            if (role == "bow") return 5;
            if (role == "ember" || role == "hex") return 3;
            if (role == "pike") return 2;
            return 1;
        }

        private string RoleForClass(string classKey)
        {
            switch ((classKey ?? "").ToLowerInvariant())
            {
                case "rogue": return "knife";
                case "ranger": return "bow";
                case "wizard": return "ember";
                case "mage": return "ember";
                case "warlock": return "hex";
                case "priest": return "mender";
                case "paladin": return "ward";
                case "warrior":
                default: return "shield";
            }
        }

        private string ClassForRole(string role)
        {
            switch ((role ?? "").ToLowerInvariant())
            {
                case "knife": return "rogue";
                case "bow": return "ranger";
                case "ember": return "wizard";
                case "hex": return "warlock";
                case "mender": return "priest";
                case "ward": return "paladin";
                case "pike":
                case "shield":
                default: return "warrior";
            }
        }

        private string SpellForClass(string classKey)
        {
            return StarterPartyCatalog.SpellSchoolForClass(classKey);
        }

        private SkillSet StartingSkills(string classKey)
        {
            switch ((classKey ?? "").ToLowerInvariant())
            {
                case "rogue": return new SkillSet { Arms = 7, Missile = 5, Guard = 2 };
                case "ranger": return new SkillSet { Arms = 4, Missile = 8, Guard = 2 };
                case "wizard": return new SkillSet { Ember = 8, Hex = 5, Guard = 1 };
                case "mage": return new SkillSet { Ember = 9, Guard = 1 };
                case "warlock": return new SkillSet { Hex = 9, Arms = 3 };
                case "priest": return new SkillSet { Mend = 9, Guard = 3 };
                case "paladin": return new SkillSet { Arms = 6, Guard = 8, Mend = 4 };
                case "warrior":
                default: return new SkillSet { Arms = 8, Guard = 6 };
            }
        }

        private string DisplayClass(string classKey)
        {
            classKey = string.IsNullOrWhiteSpace(classKey) ? "warrior" : classKey;
            return char.ToUpperInvariant(classKey[0]) + classKey.Substring(1);
        }

        private string CombatIdentityLine(CombatUnit unit)
        {
            if (unit == null) return "";
            if (unit.Summoned) return $"{ClassShortLabel(unit)} Summoned";
            if (unit.Side == UnitSide.Enemy) return EnemyTacticLine(unit);
            string race = DisplayRace(unit.Race);
            string cls = DisplayClass(unit.ClassKey);
            return $"{ClassShortLabel(unit)} {race} {cls}";
        }

        private string ClassShortLabel(CombatUnit unit)
        {
            if (unit == null) return "??";
            if (unit.Summoned) return "SMN";
            if (unit.Side == UnitSide.Enemy) return EnemyShortLabel(unit.Role);
            return ClassShortLabel(unit.ClassKey, unit.Role);
        }

        private string EnemyShortLabel(string role)
        {
            role = (role ?? "").ToLowerInvariant();
            if (role.Contains("shaman") || role.Contains("wizard") || role.Contains("mage") || role.Contains("priest") || role.Contains("cleric") || role == "adept" || role == "glassmage" || role == "bonepriest") return "CST";
            if (role.Contains("slinger") || role.Contains("archer") || role.Contains("crossbow")) return "RNG";
            if (role.Contains("shield") || role.Contains("knight") || role.Contains("brute") || role.Contains("demon") || role == "husk") return "BRU";
            if (role.Contains("rat")) return "RAT";
            if (role.Contains("drow")) return "DRW";
            return "FOE";
        }

        private string ClassShortLabel(PartyMember member)
        {
            if (member == null) return "??";
            return ClassShortLabel(member.ClassKey, member.Role);
        }

        private string ClassShortLabel(string classKey, string role)
        {
            switch ((classKey ?? "").ToLowerInvariant())
            {
                case "warrior": return "WAR";
                case "ranger": return "RNG";
                case "rogue": return "ROG";
                case "wizard": return "WIZ";
                case "mage": return "MAG";
                case "warlock": return "WLK";
                case "priest": return "PRS";
                case "paladin": return "PAL";
            }

            switch ((role ?? "").ToLowerInvariant())
            {
                case "shield": return "WAR";
                case "pike": return "WAR";
                case "bow": return "RNG";
                case "knife": return "ROG";
                case "mender": return "PRS";
                case "ember": return "MAG";
                case "hex": return "WLK";
                case "ward": return "PAL";
                default: return "??";
            }
        }

        private string DisplayRace(string race)
        {
            race = string.IsNullOrWhiteSpace(race) ? "human" : race;
            return string.Join(" ", race.Split(' ').Select(part => string.IsNullOrEmpty(part) ? part : char.ToUpperInvariant(part[0]) + part.Substring(1)).ToArray());
        }

        private void CycleRace(PartyMember member)
        {
            if (member == null) return;
            int index = Array.IndexOf(raceOrder, string.IsNullOrWhiteSpace(member.Race) ? "human" : member.Race);
            member.Race = raceOrder[(index + 1 + raceOrder.Length) % raceOrder.Length];
            RecalculateMember(member);
        }

        private bool CasterKnowsSchool(string casterSchool, string school)
        {
            if (string.IsNullOrWhiteSpace(casterSchool) || string.IsNullOrWhiteSpace(school)) return false;
            return casterSchool.Split('|').Any(s => s.Equals(school, StringComparison.OrdinalIgnoreCase));
        }

        private string PrimarySpellSchool(string casterSchool)
        {
            if (string.IsNullOrWhiteSpace(casterSchool)) return "arms";
            return casterSchool.Split('|').FirstOrDefault(s => !string.IsNullOrWhiteSpace(s)) ?? "arms";
        }

        private void ApplyStarterGearStats(PartyMember member)
        {
            if (member == null) return;
            member.WeaponStrengthBonus = 0;
            member.WeaponIntelligenceBonus = 0;
            member.WeaponAgilityBonus = 0;
            member.WeaponHealthBonus = 0;
            member.ArmorStrengthBonus = 0;
            member.ArmorIntelligenceBonus = 0;
            member.ArmorAgilityBonus = 0;
            member.ArmorHealthBonus = 0;
            member.GearStrength = 0;
            member.GearIntelligence = 0;
            member.GearAgility = 0;
            member.GearHealth = 0;
            member.WeaponDamageMin = StartingWeaponMin(member.Role);
            member.WeaponDamageMax = StartingWeaponMax(member.Role);
            member.WeaponAttackSpeed = StartingWeaponSpeed(member.Role);
        }

        private string StartingWeapon(string role)
        {
            if (role == "bow") return "plain ashwood longbow";
            if (role == "pike") return "plain long spear";
            if (role == "knife") return "plain epee";
            if (role == "mender") return "plain prayer focus";
            if (role == "ember") return "plain ember focus";
            if (role == "hex") return "plain bone focus";
            if (role == "ward") return "plain mace and ward shield";
            if (role == "shield") return "plain iron broadsword";
            return "plain weapon";
        }

        private string StartingArmor(string role)
        {
            if (role == "shield") return "plain chain hauberk";
            if (role == "ward") return "plain mail and tower shield";
            if (role == "pike") return "plain scale shirt";
            if (role == "bow") return "plain scout leathers";
            if (role == "knife") return "plain dark leathers";
            if (role == "mender") return "plain warding robe";
            if (role == "ember" || role == "hex") return "plain spell robe";
            return "plain leather";
        }

        private int StartingArmorBonus(string role)
        {
            if (role == "ward") return 2;
            if (role == "shield" || role == "pike") return 1;
            return 0;
        }

        private string StartingWeaponDamageType(string role)
        {
            if (role == "ember") return "fire";
            if (role == "hex") return "death";
            return "physical";
        }

        private int StartingWeaponMin(string role)
        {
            if (role == "bow") return 2;
            if (role == "pike") return 2;
            if (role == "knife") return 1;
            if (role == "mender" || role == "ember" || role == "hex") return 1;
            if (role == "ward") return 2;
            return 2;
        }

        private int StartingWeaponMax(string role)
        {
            if (role == "bow") return 6;
            if (role == "pike") return 7;
            if (role == "knife") return 5;
            if (role == "mender") return 4;
            if (role == "ember" || role == "hex") return 5;
            if (role == "ward") return 6;
            return 7;
        }

        private int StartingWeaponSpeed(string role)
        {
            if (role == "knife") return 11;
            if (role == "bow") return 9;
            if (role == "pike") return 7;
            if (role == "mender" || role == "ember" || role == "hex") return 8;
            if (role == "ward") return 6;
            return 7;
        }

        private void EnsurePartyCustomization()
        {
            if (state?.Party == null) return;
            if (state.Party.Count == 0) state.Party = MakeDefaultParty();
            if (state.Party.Count > PartySize)
            {
                state.Party = state.Party.Take(PartySize).ToList();
                selectedBuilderIndex = Mathf.Clamp(selectedBuilderIndex, 0, Mathf.Max(0, state.Party.Count - 1));
            }
            foreach (PartyMember member in state.Party)
            {
                if ((member.Role ?? "").Equals("hex|pact", StringComparison.OrdinalIgnoreCase)) member.Role = "hex";
                if (string.IsNullOrWhiteSpace(member.Role)) member.Role = "shield";
                if (string.IsNullOrWhiteSpace(member.ClassKey)) member.ClassKey = ClassForRole(member.Role);
                if (string.IsNullOrWhiteSpace(member.Race)) member.Race = "human";
                if (string.IsNullOrWhiteSpace(member.Origin)) member.Origin = DefaultOrigin(member.Name);
                if (string.IsNullOrWhiteSpace(member.Sigil)) member.Sigil = DefaultSigil(member.Role);
                if (string.IsNullOrWhiteSpace(member.SpriteColor)) member.SpriteColor = RoleColor(member.Role).ToHex();
                if (member.Level <= 0) member.Level = 1;
                member.Experience = Mathf.Max(0, member.Experience);
                member.SkillPoints = Mathf.Max(0, member.SkillPoints);
                member.StatPoints = Mathf.Max(0, member.StatPoints);
                if (member.Skills == null) member.Skills = new SkillSet().Normalize();
                else member.Skills.Normalize();
                if (string.IsNullOrWhiteSpace(member.Spell)) member.Spell = SpellForClass(member.ClassKey);
                if ((member.ClassKey ?? "").Equals("warlock", StringComparison.OrdinalIgnoreCase) && !CasterKnowsSchool(member.Spell, "pact")) member.Spell = string.IsNullOrWhiteSpace(member.Spell) ? "hex|pact" : member.Spell + "|pact";
                if (string.IsNullOrWhiteSpace(member.WeaponName)) member.WeaponName = StartingWeapon(member.Role);
                if (string.IsNullOrWhiteSpace(member.WeaponDamageType)) member.WeaponDamageType = "physical";
                if (member.WeaponDamageMin <= 0 || member.WeaponDamageMax <= 0 || member.WeaponAttackSpeed <= 0)
                {
                    member.WeaponDamageMin = StartingWeaponMin(member.Role);
                    member.WeaponDamageMax = StartingWeaponMax(member.Role);
                    member.WeaponAttackSpeed = StartingWeaponSpeed(member.Role);
                }
                if (string.IsNullOrWhiteSpace(member.ArmorName)) member.ArmorName = StartingArmor(member.Role);
                member.GearStrength = member.WeaponStrengthBonus + member.ArmorStrengthBonus;
                member.GearIntelligence = member.WeaponIntelligenceBonus + member.ArmorIntelligenceBonus;
                member.GearAgility = member.WeaponAgilityBonus + member.ArmorAgilityBonus;
                member.GearHealth = member.WeaponHealthBonus + member.ArmorHealthBonus;
                RecalculateMember(member);
            }
        }

        private void NormalizeGameSettings()
        {
            if (state == null) return;
            if (state.SfxVolumePercent <= 0) state.SfxVolumePercent = 100;
            state.SfxVolumePercent = Mathf.Clamp(state.SfxVolumePercent, 25, 100);
            if (state.MusicVolumePercent <= 0) state.MusicVolumePercent = 65;
            state.MusicVolumePercent = Mathf.Clamp(state.MusicVolumePercent, 25, 100);
            ApplyAudioSettings();
        }

        private Color MemberColor(PartyMember member)
        {
            if (member == null || string.IsNullOrWhiteSpace(member.SpriteColor)) return RoleColor(member?.Role);
            try { return member.SpriteColor.ToColor(); }
            catch { return RoleColor(member.Role); }
        }

        private string RoleFallback(PartyMember member)
        {
            return DisplayRole(member?.Role);
        }

        private string DisplayRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return "wanderer";
            if (role == "mender") return "cleric";
            if (role == "ember") return "ember mage";
            if (role == "hex") return "hex mage";
            if (role == "ward") return "warder";
            return role;
        }

        private string DefaultOrigin(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return originOrder[0];
            int index = Mathf.Abs(name.GetHashCode()) % originOrder.Length;
            return originOrder[index];
        }

        private string DefaultSigil(string role)
        {
            int index = Mathf.Abs((role ?? "shield").GetHashCode()) % sigilOrder.Length;
            return sigilOrder[index];
        }

        private void CycleOrigin(PartyMember member)
        {
            int index = Array.IndexOf(originOrder, member.Origin);
            member.Origin = originOrder[(index + 1 + originOrder.Length) % originOrder.Length];
        }

        private void CycleSigil(PartyMember member)
        {
            int index = Array.IndexOf(sigilOrder, member.Sigil);
            member.Sigil = sigilOrder[(index + 1 + sigilOrder.Length) % sigilOrder.Length];
        }

        private void CycleColor(PartyMember member)
        {
            string current = string.IsNullOrWhiteSpace(member.SpriteColor) ? RoleColor(member.Role).ToHex() : member.SpriteColor.ToUpperInvariant();
            int index = Array.FindIndex(accentPalette, c => c.Equals(current, StringComparison.OrdinalIgnoreCase));
            member.SpriteColor = accentPalette[(index + 1 + accentPalette.Length) % accentPalette.Length];
        }

        private string RandomName(string role)
        {
            string[] hard = { "Maer", "Cairn", "Rusk", "Brann", "Korr", "Daven", "Harl", "Tor" };
            string[] quick = { "Selka", "Jory", "Tala", "Neris", "Venn", "Ilya", "Sable", "Kesh" };
            string[] mystic = { "Vesh", "Oryn", "Luma", "Sareth", "Edrin", "Mira", "Ithe", "Vaul" };
            string[] source = role == "mender" || role == "ember" || role == "hex" ? mystic : role == "bow" || role == "knife" ? quick : hard;
            return source[rng.Next(source.Length)];
        }

        private Color RoleColor(string role)
        {
            switch (role)
            {
                case "shield": return Hex("58b7a5");
                case "pike": return Hex("8fc27b");
                case "bow": return Hex("d7a84e");
                case "knife": return Hex("d98b6a");
                case "mender": return Hex("97dbc2");
                case "ember": return Hex("c65c3b");
                case "hex": return Hex("b94b56");
                case "ward": return Hex("a9b0a2");
                default: return teal;
            }
        }

        private Color SpellSchoolColor(string school)
        {
            if (CasterKnowsSchool(school, "mend")) return RoleColor("mender");
            if (CasterKnowsSchool(school, "pact")) return RoleColor("hex");
            if (CasterKnowsSchool(school, "ember")) return RoleColor("ember");
            if (CasterKnowsSchool(school, "hex")) return RoleColor("hex");
            return gold;
        }

        private string BestSkillLabel(PartyMember member)
        {
            Dictionary<string, int> skills = SkillPairs(member.Skills);
            return skills.OrderByDescending(kv => kv.Value).First().Key;
        }

        private int BestSkillValue(PartyMember member)
        {
            return SkillPairs(member.Skills).Values.Max();
        }

        private Dictionary<string, int> SkillPairs(SkillSet skills)
        {
            return new Dictionary<string, int>
            {
                { "arms", skills.Arms },
                { "missile", skills.Missile },
                { "mend", skills.Mend },
                { "ember", skills.Ember },
                { "hex", skills.Hex },
                { "guard", skills.Guard }
            };
        }

        private string SkillAdjective(int value)
        {
            if (value < 8) return "lousy";
            if (value < 15) return "feeble";
            if (value < 30) return "steady";
            if (value < 50) return "deft";
            return "masterful";
        }

        private int SkillValue(SkillSet skills, string key)
        {
            switch (key)
            {
                case "arms": return skills.Arms;
                case "missile": return skills.Missile;
                case "mend": return skills.Mend;
                case "ember": return skills.Ember;
                case "hex": return skills.Hex;
                case "guard": return skills.Guard;
                default: return 1;
            }
        }

        private void SetSkill(SkillSet skills, string key, int value)
        {
            switch (key)
            {
                case "arms": skills.Arms = value; break;
                case "missile": skills.Missile = value; break;
                case "mend": skills.Mend = value; break;
                case "ember": skills.Ember = value; break;
                case "hex": skills.Hex = value; break;
                case "guard": skills.Guard = value; break;
            }
        }
    }
}
