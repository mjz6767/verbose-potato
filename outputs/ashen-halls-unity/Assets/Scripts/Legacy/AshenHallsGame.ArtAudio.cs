using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private Texture2D pixel;

        private Texture2D splashArt;

        private Texture2D titleCardArt;

        private Texture2D gameIconArt;

        private Texture2D betaCombatArt;

        private Texture2D formulaLabArt;

        private Texture2D combatSpriteSheet;

        private Texture2D classIconAtlas;

        private Texture2D worldObjectAtlas;

        private Texture2D itemIconAtlas;

        private Texture2D enemyRosterAtlas;

        private Texture2D combatUiAtlas;

        private Texture2D combatUiPanelAtlas;

        private Texture2D spellbookUiAtlas;

        private Texture2D signatureSpellIconAtlas;

        private Texture2D lightningSpellIconAtlas;

        private Texture2D powerBookStateIconAtlas;

        private Texture2D emberSpellAtlas;

        private Texture2D epicSpellEffectsAtlas;

        private Texture2D mageWarlockSpellVfxAtlas;

        private Texture2D supportHexSpellVfxAtlas;

        private Texture2D classSkillVfxAtlas;

        private Texture2D combatPowerTravelVfxAtlas;

        private Texture2D combatPowerAftermathVfxAtlas;

        private Texture2D spellAnimationAtlas;

        private Texture2D combatSpellbookUiAtlas;

        private Texture2D pactSpellbookAtlas;

        private Texture2D bossEnemyAtlas;

        private Texture2D questWorldAtlas;

        private Texture2D worldMapPropAtlas;

        private Texture2D worldMapBiomePropAtlas;

        private Texture2D worldMapExplorationTileAtlas;
        private Texture2D worldMapMaterialAtlas;

        private Texture2D worldMapLandmarkAtlas;

        private Texture2D worldMapRegionLandmarkAtlas;

        private Texture2D worldMapRegionMarkerAtlas;

        private Texture2D worldAreaSetpieceAtlas;

        private Texture2D worldMapOverlayAtlas;

        private Texture2D worldMapProgressionOverlayAtlas;

        private Texture2D worldMapUiAtlas;

        private Texture2D worldMapTokenSpriteAtlas;

        private Texture2D storyCardAtlas;

        private Texture2D npcPortraitAtlas;

        private Texture2D routeScaffoldAtlas;

        private Texture2D dungeonScaffoldAtlas;

        private Texture2D factionBannerAtlas;

        private Texture2D serviceScaffoldAtlas;

        private Texture2D characterInventoryUiAtlas;

        private Texture2D uniqueItemAtlas;

        private Texture2D combatHudUiAtlas;

        private Texture2D combatSpellFloatAtlas;

        private Texture2D enemyWorldObjectAtlas;

        private Texture2D roamingThreatAtlas;

        private Texture2D tavernBackdropArt;

        private Texture2D tavernUiAtlas;

        private Texture2D titleMenuScrollArt;

        private Texture2D titleMenuFocusArt;

        private Texture2D titleMenuIconAtlas;

        private Texture2D inventoryConsumableAtlas;

        private Texture2D combatCommandIconAtlas;

        private Texture2D abilityIconAtlas;

        private Texture2D rangerAbilityEffectAtlas;

        private Texture2D enemySpriteAtlas;

        private Texture2D characterCombatAtlas;

        private Texture2D creatureSpriteAtlas;

        private Texture2D demonSummonAtlas;

        private Texture2D combatTerrainAtlas;

        private Texture2D koboldCombatTerrainAtlas;

        private Texture2D koboldRouteAtlas;

        private Texture2D koboldBossAtlas;

        private Texture2D koboldCavePropAtlas;

        private Texture2D midgaardTownAtlas;

        private Texture2D midgaardTileAtlas;

        private Texture2D midgaardWallAtlas;

        private Texture2D midgaardGateAtlas;

        private Texture2D midgaardCityPropAtlas;

        private Texture2D midgaardStreetLifeAtlas;

        private Texture2D midgaardPavingDecalAtlas;

        private Texture2D midgaardNpcAtlas;

        private Texture2D midgaardSewerAtlas;

        private Texture2D midgaardInteriorPropAtlas;
        private Texture2D midgaardInteriorTileAtlas;

        private bool itemIconAtlasUsesInventoryContract;

        private readonly Dictionary<string, bool> explorationAtlasCellUsable = new Dictionary<string, bool>();

        private readonly Dictionary<string, ExploreArtMetrics> exploreArtMetrics = new Dictionary<string, ExploreArtMetrics>();

        private readonly Dictionary<int, SpriteCellMetrics> spriteCellMetrics = new Dictionary<int, SpriteCellMetrics>();

        private AudioSource audioSource;

        private AudioSource musicSource;

        private AudioSource musicFadeSource;

        private bool musicTransitionActive;

        private float musicTransitionStartedAt = -1f;

        private float activeMusicTransitionDuration = MusicTransitionRules.ExploreTransitionDuration;

        private bool musicIntroFadeActive;

        private float musicIntroFadeStartedAt = -1f;

        private float activeMusicIntroFadeDuration = MusicTransitionRules.ExploreIntroFadeDuration;

        private AudioClip musicIntroFadeClip;

        private string explorationMusicSelectedKey = "";

        private string explorationMusicCandidateKey = "";

        private float explorationMusicSelectedAt = -1f;

        private float explorationMusicCandidateAt = -1f;

        private readonly List<AudioSource> sfxVoices = new List<AudioSource>();

        private int nextSfxVoice;

        private int sfxPlaybackSerial;

        private float combatMusicDuckStartedAt = -1f;

        private float combatMusicDuckFullDepthAt = -1f;

        private float combatMusicDuckHoldUntil = -1f;

        private float combatMusicDuckUntil = -1f;

        private float combatMusicDuckDepth;

        private CombatState combatMusicEncounter;

        private string combatMusicBaseKey = "";

        private string combatMusicSelectedKey = "";

        private float combatMusicSelectedAt = -1f;

        private float combatMusicCriticalStartedAt = -1f;

        private float combatMusicRecoveredStartedAt = -1f;

        private bool combatMusicLastStandActive;

        private AudioClip tavernMusicClip;

        private AudioClip combatMusicClip;

        private AudioClip sewerCombatMusicClip;

        private AudioClip bossCombatMusicClip;

        private AudioClip koboldCombatMusicClip;

        private AudioClip drowCombatMusicClip;

        private AudioClip demonCombatMusicClip;

        private AudioClip undeadCombatMusicClip;

        private readonly Dictionary<string, AudioClip> zoneMusicClips = new Dictionary<string, AudioClip>();

        private readonly Dictionary<string, AudioClip> adaptiveMusicClips = new Dictionary<string, AudioClip>();

        private readonly Dictionary<string, Func<AudioClip>> adaptiveMusicFactories = new Dictionary<string, Func<AudioClip>>();

        private readonly Dictionary<string, AudioClip> soundClips = new Dictionary<string, AudioClip>();

        private const string ImportedSfxResourcePath = "Audio/Sfx";

        private readonly HashSet<string> importedSfxKeys = new HashSet<string>(StringComparer.Ordinal);

        private const string ImportedMusicResourcePath = "Audio/Music";

        private readonly Dictionary<string, AudioClip> importedMusicClips =
            new Dictionary<string, AudioClip>(StringComparer.Ordinal);

        private readonly Dictionary<string, string> importedMusicRouteNames =
            new Dictionary<string, string>(StringComparer.Ordinal);

        private readonly HashSet<string> importedMusicKeys =
            new HashSet<string>(StringComparer.Ordinal);

        private float nextExplorationAmbienceAt = -1f;

        private string lastExplorationAmbienceContext = "";

        private int explorationAmbienceSequence;

        private float lastExplorationForegroundSfxAt = -10f;

        private float nextTavernAmbienceAt = -1f;

        private int tavernAmbienceSequence;

        private float nextCombatAmbienceAt = -1f;

        private int combatAmbienceSequence;

        private float lastCombatForegroundSfxAt = -10f;

        private CombatState combatAmbienceEncounter;

        private readonly List<ScheduledSfxCue> scheduledSfx = new List<ScheduledSfxCue>();

        private int scheduledSfxSerial;

        private struct ScheduledSfxCue
        {
            public string Key;
            public float Volume;
            public float PlayAt;
            public float Pan;
            public float Pitch;
            public int Priority;
            public int Serial;
        }

        private enum GateOrientation
        {
            North,
            South,
            East,
            West
        }

        private const int ArmoryTabCount = 5;

        private static readonly float[] floatTextLaneOffsets =
        {
            0f, -0.30f, 0.30f, -0.15f, 0.15f, -0.45f, 0.45f, -0.58f, 0.58f
        };

        private static readonly string[] classOrder = StarterPartyCatalog.SelectableClassKeys.ToArray();

        private static readonly string[] raceOrder =
        {
            "human", "dusk elf", "stoneborn", "fenkin", "ashling"
        };

        private static readonly string[] originOrder =
        {
            "Midgaard", "Ash Road", "Salt Fen", "Glass Hill", "Dusk Market", "Old Quarry", "Green Shrine", "Red Gate"
        };

        private static readonly string[] sigilOrder =
        {
            "bar", "chevron", "moon", "cross", "diamond", "flame", "leaf", "eye"
        };

        private static readonly string[] accentPalette =
        {
            "#58B7A5", "#D7A84E", "#C65C3B", "#B94B56", "#7F9D5B", "#8D93C7", "#D98B6A", "#A9B0A2", "#97DBC2", "#9B6B45"
        };

        private static readonly string[] enemyTemplateIds = EnemyCatalog.Ids;

        private static readonly FormulaDef[] formulaBook = FormulaCatalog.All;

        private sealed class SpriteCellMetrics
        {
            public Rect Source;
            public float AnchorX;
        }

        private sealed class ExploreArtMetrics
        {
            public Rect Source;
        }

        private struct WorldMapArtSpec
        {
            public float Scale;
            public Vector2 Pivot;
            public Vector2 Offset;
            public bool AllowOverflow;

            public WorldMapArtSpec(float scale, Vector2 pivot, Vector2 offset, bool allowOverflow)
            {
                Scale = scale;
                Pivot = pivot;
                Offset = offset;
                AllowOverflow = allowOverflow;
            }
        }

        private void BuildSoundClips()
        {
            soundClips.Clear();
            scheduledSfx.Clear();
            soundClips["move"] = MakeSound("move", 96f, 54f, 0.08f, 0.34f, "thud");
            soundClips["blocked"] = MakeSound("blocked", 132f, 86f, 0.10f, 0.30f, "square");
            soundClips["attack"] = MakeSound("attack", 220f, 78f, 0.12f, 0.38f, "slash");
            soundClips["hit"] = MakeSound("hit", 148f, 82f, 0.11f, 0.34f, "thud");
            soundClips["blade"] = MakeSound("blade", 330f, 120f, 0.10f, 0.34f, "slash");
            soundClips["bow"] = MakeSound("bow", 150f, 420f, 0.10f, 0.24f, "rustle");
            soundClips["miss"] = MakeSound("miss", 410f, 210f, 0.10f, 0.20f, "rustle");
            soundClips["crit"] = MakeSound("crit", 280f, 760f, 0.16f, 0.32f, "square");
            soundClips["guard"] = MakeSound("guard", 170f, 96f, 0.14f, 0.34f, "thud");
            soundClips["counter"] = MakeSound("counter", 260f, 150f, 0.12f, 0.30f, "slash");
            soundClips["resist"] = MakeSound("resist", 180f, 260f, 0.12f, 0.20f, "square");
            soundClips["status"] = MakeSound("status", 300f, 480f, 0.16f, 0.22f, "sine");
            soundClips["spell"] = MakeSound("spell", 410f, 820f, 0.24f, 0.30f, "sine");
            soundClips["fireball"] = MakeSpellSound("fireball", "fire", 180f, 820f, 0.32f, 0.34f);
            soundClips["meteor"] = MakeSpellSound("meteor", "meteor", 72f, 520f, 0.48f, 0.38f);
            soundClips["heal"] = MakeSpellSound("heal", "holy", 310f, 860f, 0.32f, 0.26f);
            soundClips["ward"] = MakeSound("ward", 260f, 520f, 0.22f, 0.30f, "sine");
            soundClips["light"] = MakeSpellSound("light", "holy", 620f, 1320f, 0.22f, 0.27f);
            soundClips["shock"] = MakeSpellSound("shock", "shock", 1120f, 210f, 0.18f, 0.31f);
            soundClips["arc"] = MakeSound("arc", 760f, 1120f, 0.18f, 0.30f, "square");
            soundClips["curse"] = MakeSound("curse", 260f, 82f, 0.32f, 0.28f, "square");
            soundClips["tree"] = MakeSpellSound("tree", "nature", 104f, 240f, 0.40f, 0.31f);
            soundClips["breakcover"] = MakeSound("breakcover", 110f, 62f, 0.20f, 0.38f, "thud");
            soundClips["stone"] = MakeSound("stone", 92f, 58f, 0.24f, 0.36f, "thud");
            soundClips["fire"] = MakeSound("fire", 260f, 640f, 0.26f, 0.32f, "rustle");
            soundClips["ice"] = MakeSpellSound("ice", "frost", 980f, 390f, 0.24f, 0.27f);
            soundClips["web"] = MakeSound("web", 190f, 260f, 0.20f, 0.26f, "rustle");
            soundClips["poison"] = MakeSound("poison", 180f, 130f, 0.30f, 0.26f, "sine");
            soundClips["death"] = MakeSpellSound("death", "death", 210f, 48f, 0.42f, 0.33f);
            soundClips["cache"] = MakeSound("cache", 560f, 960f, 0.16f, 0.30f, "square");
            soundClips["shrine"] = MakeSound("shrine", 260f, 520f, 0.36f, 0.26f, "sine");
            soundClips["encounter"] = MakeSound("encounter", 118f, 72f, 0.34f, 0.36f, "square");
            soundClips["victory"] = MakeSound("victory", 430f, 860f, 0.40f, 0.30f, "sine");
            soundClips["defeat"] = MakeSound("defeat", 150f, 64f, 0.48f, 0.34f, "thud");
            soundClips["save"] = MakeSound("save", 620f, 840f, 0.14f, 0.24f, "sine");
            soundClips["ui"] = MakeSound("ui", 480f, 610f, 0.06f, 0.20f, "square");
            soundClips["uiopen"] = MakeSound("uiopen", 360f, 680f, 0.18f, 0.20f, "chime");
            soundClips["uiclose"] = MakeSound("uiclose", 620f, 310f, 0.16f, 0.18f, "rustle");
            soundClips["uiconfirm"] = MakeSound("uiconfirm", 390f, 780f, 0.24f, 0.24f, "chime");
            soundClips["uitab"] = MakeSound("uitab", 560f, 720f, 0.10f, 0.16f, "rustle");
            soundClips[TitleAudioRules.RevealStrikeKey] = MakeSound(TitleAudioRules.RevealStrikeKey, 820f, 92f, 0.42f, 0.34f, "boom");
            soundClips[TitleAudioRules.RevealChimeKey] = MakeSound(TitleAudioRules.RevealChimeKey, 294f, 880f, 0.40f, 0.26f, "chime");
            soundClips[TitleAudioRules.FocusKey] = MakeSound(TitleAudioRules.FocusKey, 680f, 860f, 0.10f, 0.18f, "chime");
            soundClips[TitleAudioRules.ConfirmKey] = MakeSound(TitleAudioRules.ConfirmKey, 294f, 1175f, 0.34f, 0.28f, "chime");
            soundClips[TitleAudioRules.OpenKey] = MakeSound(TitleAudioRules.OpenKey, 260f, 720f, 0.28f, 0.22f, "rustle");
            soundClips[TitleAudioRules.CloseKey] = MakeSound(TitleAudioRules.CloseKey, 620f, 180f, 0.24f, 0.22f, "rustle");
            soundClips[CombatAudioMixRules.StepCue] = MakeSound(CombatAudioMixRules.StepCue, 138f, 74f, 0.16f, 0.26f, "thud");
            soundClips[CombatAudioMixRules.GuardCue] = MakeSound(CombatAudioMixRules.GuardCue, 184f, 680f, 0.28f, 0.30f, "chime");
            soundClips[CombatAudioMixRules.TurnCue] = MakeSound(CombatAudioMixRules.TurnCue, 420f, 168f, 0.24f, 0.24f, "square");
            soundClips[CombatAudioMixRules.CriticalCue] = MakeSound(CombatAudioMixRules.CriticalCue, 940f, 76f, 0.42f, 0.38f, "boom");
            soundClips[CombatAudioMixRules.SteelAmbienceCue] = MakeAmbientSound(CombatAudioMixRules.SteelAmbienceCue, "forge");
            soundClips[CombatAudioMixRules.SewerAmbienceCue] = MakeAmbientSound(CombatAudioMixRules.SewerAmbienceCue, "drip");
            soundClips[CombatAudioMixRules.ArcaneAmbienceCue] = MakeAmbientSound(CombatAudioMixRules.ArcaneAmbienceCue, "wind");
            soundClips["itemequip"] = MakeServiceSound("itemequip", "armor");
            soundClips["itemtake"] = MakeSound("itemtake", 210f, 460f, 0.24f, 0.22f, "rustle");
            soundClips["elixir"] = MakeSpellSound("elixir", "holy", 260f, 740f, 0.42f, 0.24f);
            soundClips["rest"] = MakeAmbientSound("rest", "hearth");
            soundClips["levelup"] = MakeSound("levelup", 290f, 980f, 0.62f, 0.28f, "chime");
            soundClips["formula"] = MakeSound("formula", 520f, 880f, 0.12f, 0.24f, "sine");
            soundClips["castmend"] = MakeSpellSound("castmend", "holy", 280f, 920f, 0.25f, 0.24f);
            soundClips["castlight"] = MakeSpellSound("castlight", "holy", 520f, 1260f, 0.23f, 0.25f);
            soundClips["castember"] = MakeSpellSound("castember", "fire", 150f, 760f, 0.24f, 0.28f);
            soundClips["castfrost"] = MakeSpellSound("castfrost", "frost", 1060f, 360f, 0.25f, 0.25f);
            soundClips["castshock"] = MakeSpellSound("castshock", "shock", 1240f, 280f, 0.20f, 0.27f);
            soundClips["castnature"] = MakeSpellSound("castnature", "nature", 92f, 330f, 0.31f, 0.27f);
            soundClips["casthex"] = MakeSpellSound("casthex", "death", 390f, 72f, 0.29f, 0.25f);
            soundClips["castpact"] = MakeSpellSound("castpact", "rift", 96f, 310f, 0.34f, 0.29f);
            soundClips["castdeathburst"] = MakeSpellSound("castdeathburst", "death", 320f, 42f, 0.42f, 0.36f);
            soundClips["deathburst"] = MakeSpellSound("deathburst", "death", 190f, 34f, 0.54f, 0.40f);
            soundClips["castgreatersummon"] = MakeSpellSound("castgreatersummon", "rift", 84f, 360f, 0.48f, 0.36f);
            soundClips["greatersummon"] = MakeSpellSound("greatersummon", "rift", 62f, 220f, 0.58f, 0.42f);
            soundClips["castascendance"] = MakeSpellSound("castascendance", "rift", 112f, 510f, 0.50f, 0.38f);
            soundClips["ascendance"] = MakeSpellSound("ascendance", "death", 88f, 640f, 0.56f, 0.42f);
            soundClips["casttempest"] = MakeSpellSound("casttempest", "shock", 820f, 1660f, 0.38f, 0.36f);
            soundClips["tempest"] = MakeSpellSound("tempest", "shock", 1480f, 94f, 0.46f, 0.42f);
            soundClips["castveil"] = MakeSpellSound("castveil", "rift", 760f, 180f, 0.30f, 0.30f);
            soundClips["veilstep"] = MakeSpellSound("veilstep", "shock", 1240f, 260f, 0.26f, 0.30f);
            soundClips["castseal"] = MakeSpellSound("castseal", "holy", 360f, 1180f, 0.34f, 0.32f);
            soundClips["riftseal"] = MakeSpellSound("riftseal", "holy", 960f, 280f, 0.40f, 0.36f);
            soundClips["fieldfire"] = MakeSound("fieldfire", 310f, 150f, 0.20f, 0.25f, "rustle");
            soundClips["fieldice"] = MakeSound("fieldice", 880f, 510f, 0.18f, 0.23f, "chime");
            soundClips["fieldgas"] = MakeSound("fieldgas", 118f, 174f, 0.26f, 0.22f, "smoke");
            soundClips["fieldsnare"] = MakeSound("fieldsnare", 260f, 150f, 0.18f, 0.22f, "rustle");
            soundClips["fieldholy"] = MakeSound("fieldholy", 420f, 980f, 0.22f, 0.24f, "chime");
            soundClips["fieldcurse"] = MakeSound("fieldcurse", 210f, 72f, 0.28f, 0.25f, "square");
            soundClips["turn"] = MakeSound("turn", 360f, 520f, 0.10f, 0.22f, "square");
            soundClips["charge"] = MakeSkillSound("charge", "charge", 150f, 520f, 0.18f, 0.34f);
            soundClips["whirlwind"] = MakeSkillSound("whirlwind", "whirlwind", 560f, 160f, 0.22f, 0.31f);
            soundClips["execute"] = MakeSkillSound("execute", "execute", 180f, 860f, 0.19f, 0.38f);
            soundClips["ambush"] = MakeSkillSound("ambush", "ambush", 240f, 700f, 0.14f, 0.30f);
            soundClips["eviscerate"] = MakeSkillSound("eviscerate", "eviscerate", 620f, 180f, 0.16f, 0.36f);
            soundClips["chargeimpact"] = MakeSkillSound("chargeimpact", "charge-impact", 128f, 52f, 0.24f, 0.42f);
            soundClips["whirlwindimpact"] = MakeSkillSound("whirlwindimpact", "whirlwind-impact", 840f, 92f, 0.28f, 0.39f);
            soundClips["executeimpact"] = MakeSkillSound("executeimpact", "execute-impact", 760f, 58f, 0.24f, 0.44f);
            soundClips["ambushimpact"] = MakeSkillSound("ambushimpact", "ambush-impact", 980f, 140f, 0.20f, 0.38f);
            soundClips["eviscerateimpact"] = MakeSkillSound("eviscerateimpact", "eviscerate-impact", 720f, 82f, 0.23f, 0.42f);
            soundClips["sleep"] = MakeSpellSound("sleep", "death", 420f, 92f, 0.32f, 0.26f);
            soundClips["stealth"] = MakeSkillSound("stealth", "stealth", 720f, 360f, 0.24f, 0.20f);
            soundClips["smoke"] = MakeSkillSound("smoke", "smoke", 110f, 180f, 0.34f, 0.24f);
            soundClips["rally"] = MakeSkillSound("rally", "rally", 320f, 680f, 0.25f, 0.30f);
            soundClips["aimedshot"] = MakeSkillSound("aimedshot", "focus", 220f, 980f, 0.14f, 0.32f);
            soundClips["pinning"] = MakeSkillSound("pinning", "pinning", 180f, 620f, 0.16f, 0.30f);
            soundClips["volley"] = MakeSkillSound("volley", "volley", 310f, 760f, 0.24f, 0.28f);
            soundClips["scoutmark"] = MakeSkillSound("scoutmark", "mark", 540f, 840f, 0.20f, 0.24f);
            soundClips["arrowrain"] = MakeSkillSound("arrowrain", "arrow-rain", 440f, 210f, 0.28f, 0.24f);
            soundClips["mark"] = MakeSkillSound("mark", "mark", 680f, 920f, 0.12f, 0.20f);
            soundClips["castshimmer"] = MakeSound("castshimmer", 760f, 1320f, 0.16f, 0.20f, "chime");
            soundClips["impactlow"] = MakeSound("impactlow", 88f, 46f, 0.22f, 0.34f, "boom");
            soundClips["resonance"] = MakeSound("resonance", 460f, 170f, 0.24f, 0.26f, "sine");
            soundClips["riftpounce"] = MakeSkillSound("riftpounce", "charge", 118f, 940f, 0.30f, 0.32f);
            soundClips["riftpounceimpact"] = MakeSkillSound("riftpounceimpact", "charge-impact", 136f, 42f, 0.40f, 0.40f);
            soundClips["abyssalwhirl"] = MakeSkillSound("abyssalwhirl", "whirlwind", 430f, 116f, 0.42f, 0.31f);
            soundClips["abyssalwhirlimpact"] = MakeSkillSound("abyssalwhirlimpact", "whirlwind-impact", 780f, 68f, 0.46f, 0.39f);
            soundClips["soulrend"] = MakeSkillSound("soulrend", "eviscerate", 760f, 110f, 0.32f, 0.32f);
            soundClips["soulrendimpact"] = MakeSkillSound("soulrendimpact", "eviscerate-impact", 920f, 44f, 0.44f, 0.41f);
            soundClips["dreadroar"] = MakeSpellSound("dreadroar", "rift", 62f, 420f, 0.54f, 0.34f);
            soundClips["dreadroarimpact"] = MakeSpellSound("dreadroarimpact", "death", 188f, 38f, 0.58f, 0.42f);
            soundClips["footstone"] = MakeSound("footstone", 126f, 72f, 0.075f, 0.24f, "thud");
            soundClips["footearth"] = MakeSound("footearth", 92f, 58f, 0.085f, 0.22f, "rustle");
            soundClips["footwood"] = MakeSound("footwood", 172f, 104f, 0.075f, 0.22f, "thud");
            soundClips["footwater"] = MakeSound("footwater", 210f, 118f, 0.12f, 0.18f, "smoke");
            soundClips["footglass"] = MakeSound("footglass", 1280f, 360f, 0.12f, 0.18f, "chime");
            soundClips["footmud"] = MakeSound("footmud", 78f, 46f, 0.14f, 0.20f, "smoke");
            soundClips["footash"] = MakeSound("footash", 118f, 62f, 0.11f, 0.18f, "rustle");
            soundClips["footgravel"] = MakeSound("footgravel", 220f, 82f, 0.11f, 0.18f, "thud");
            soundClips["wayfind"] = MakeSound("wayfind", 420f, 940f, 0.22f, 0.24f, "chime");
            soundClips["dialogue"] = MakeSound("dialogue", 390f, 540f, 0.10f, 0.18f, "chime");
            soundClips["door"] = MakeSound("door", 104f, 62f, 0.24f, 0.28f, "thud");
            soundClips["dialogueopen"] = MakeSound("dialogueopen", 246f, 520f, 0.18f, 0.22f, "chime");
            soundClips["dialoguepage"] = MakeSound("dialoguepage", 320f, 190f, 0.12f, 0.18f, "rustle");
            soundClips["dialogueclose"] = MakeSound("dialogueclose", 430f, 260f, 0.12f, 0.17f, "chime");
            soundClips["gateopen"] = MakeSound("gateopen", 92f, 48f, 0.38f, 0.32f, "thud");
            soundClips["gatebarred"] = MakeSound("gatebarred", 138f, 62f, 0.28f, 0.36f, "thud");
            soundClips["doorwood"] = MakeSound("doorwood", 148f, 58f, 0.34f, 0.30f, "rustle");
            soundClips["doorroyal"] = MakeSound("doorroyal", 106f, 42f, 0.48f, 0.38f, "thud");
            soundClips["thronechime"] = MakeSound("thronechime", 392f, 784f, 0.44f, 0.27f, "chime");
            soundClips["shopbell"] = MakeSound("shopbell", 660f, 990f, 0.24f, 0.24f, "chime");
            soundClips["swing"] = MakeSound("swing", 460f, 118f, 0.13f, 0.27f, "slash");
            soundClips["swingheavy"] = MakeSound("swingheavy", 270f, 72f, 0.18f, 0.34f, "slash");
            soundClips["thrust"] = MakeSound("thrust", 620f, 170f, 0.11f, 0.25f, "slash");
            soundClips["arrowrelease"] = MakeSound("arrowrelease", 182f, 760f, 0.13f, 0.25f, "rustle");
            soundClips["bladecontact"] = MakeSound("bladecontact", 920f, 190f, 0.10f, 0.27f, "slash");
            soundClips["thrustcontact"] = MakeSound("thrustcontact", 1180f, 240f, 0.085f, 0.25f, "slash");
            soundClips["heavycontact"] = MakeSound("heavycontact", 164f, 52f, 0.18f, 0.34f, "boom");
            soundClips["arrowcontact"] = MakeSound("arrowcontact", 760f, 130f, 0.09f, 0.24f, "square");
            soundClips["woodcontact"] = MakeSound("woodcontact", 236f, 72f, 0.14f, 0.28f, "thud");
            soundClips["stonecontact"] = MakeSound("stonecontact", 148f, 48f, 0.17f, 0.32f, "thud");
            soundClips["spellrelease"] = MakeSound("spellrelease", 520f, 1260f, 0.14f, 0.22f, "chime");
            soundClips["impactflesh"] = MakeMaterialImpact("impactflesh", "flesh");
            soundClips["impactleather"] = MakeMaterialImpact("impactleather", "leather");
            soundClips["impactmail"] = MakeMaterialImpact("impactmail", "mail");
            soundClips["impactplate"] = MakeMaterialImpact("impactplate", "plate");
            soundClips["impactshield"] = MakeMaterialImpact("impactshield", "shield");
            soundClips["ratchitter"] = MakeRatVoice("ratchitter", "chitter");
            soundClips["ratattack"] = MakeRatVoice("ratattack", "attack");
            soundClips["ratcast"] = MakeRatVoice("ratcast", "cast");
            soundClips["ratimpact"] = MakeRatVoice("ratimpact", "hurt");
            soundClips["ratdeath"] = MakeRatVoice("ratdeath", "death");
            foreach (string faction in new[] { "kobold", "drow", "demon", "undead" })
            {
                foreach (string action in new[] { "alert", "step", "attack", "cast", "hurt", "death" })
                {
                    string key = faction + action;
                    soundClips[key] = MakeCreatureVoice(key, faction, action);
                }
            }
            soundClips["servicecoin"] = MakeServiceSound("servicecoin", "coin");
            soundClips["servicearmor"] = MakeServiceSound("servicearmor", "armor");
            soundClips["serviceweapon"] = MakeServiceSound("serviceweapon", "weapon");
            soundClips["serviceenchant"] = MakeServiceSound("serviceenchant", "rune");
            soundClips["ambcity"] = MakeAmbientSound("ambcity", "city");
            soundClips["ambbell"] = MakeAmbientSound("ambbell", "bell");
            soundClips["ambmarket"] = MakeAmbientSound("ambmarket", "market");
            soundClips["ambforge"] = MakeAmbientSound("ambforge", "forge");
            soundClips["ambgate"] = MakeAmbientSound("ambgate", "gate");
            soundClips["ambdrip"] = MakeAmbientSound("ambdrip", "drip");
            soundClips["ambwind"] = MakeAmbientSound("ambwind", "wind");
            soundClips["ambdrum"] = MakeAmbientSound("ambdrum", "drum");
            soundClips["ambstone"] = MakeAmbientSound("ambstone", "stone");
            soundClips["ambrain"] = MakeAmbientSound("ambrain", "rain");
            soundClips["ambtavern"] = MakeAmbientSound("ambtavern", "tavern");
            soundClips["ambhearth"] = MakeAmbientSound("ambhearth", "hearth");
            soundClips["ambgrove"] = MakeAmbientSound("ambgrove", "wind");
            soundClips["ambfen"] = MakeAmbientSound("ambfen", "rain");
            soundClips["ambglass"] = MakeAmbientSound("ambglass", "bell");
            soundClips["ambruin"] = MakeAmbientSound("ambruin", "stone");
            soundClips["ambcave"] = MakeAmbientSound("ambcave", "drip");
            soundClips["ambcamp"] = MakeAmbientSound("ambcamp", "hearth");
            ApplyImportedSfxOverrides();
        }

        private void ApplyImportedSfxOverrides()
        {
            importedSfxKeys.Clear();
            AudioClip[] clips;
            try
            {
                clips = Resources.LoadAll<AudioClip>(ImportedSfxResourcePath);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Authored SFX bank unavailable; procedural fallbacks retained. " + ex.Message);
                return;
            }

            Array.Sort(clips, (left, right) => string.CompareOrdinal(
                left == null ? "" : left.name,
                right == null ? "" : right.name));
            foreach (AudioClip clip in clips)
            {
                if (clip == null) continue;
                string key = (clip.name ?? "").Trim().ToLowerInvariant();
                if (string.IsNullOrEmpty(key) || !soundClips.ContainsKey(key))
                {
                    Debug.LogWarning("Ignoring authored SFX with unknown cue key: " + clip.name);
                    continue;
                }

                if (!importedSfxKeys.Add(key))
                {
                    Debug.LogWarning("Ignoring duplicate authored SFX cue: " + key);
                    continue;
                }

                if (clip.loadState == AudioDataLoadState.Unloaded && !clip.LoadAudioData())
                {
                    importedSfxKeys.Remove(key);
                    Debug.LogWarning("Authored SFX failed to begin loading; procedural fallback retained: " + key);
                    continue;
                }

                if (clip.loadState == AudioDataLoadState.Failed)
                {
                    importedSfxKeys.Remove(key);
                    Debug.LogWarning("Authored SFX failed to load; procedural fallback retained: " + key);
                    continue;
                }

                soundClips[key] = clip;
            }

            if (importedSfxKeys.Count > 0)
            {
                Debug.Log("Authored SFX overrides active: " + importedSfxKeys.Count);
            }
        }

        private void BuildMusicClips()
        {
            zoneMusicClips.Clear();
            adaptiveMusicClips.Clear();
            adaptiveMusicFactories.Clear();
            importedMusicRouteNames.Clear();
            tavernMusicClip = MakeTavernMusic();
            combatMusicClip = MakePatternMusic("combat_battle_pulse_loop", 18f, 0.34f, new[] { 196f, 220f, 247f, 220f, 174.6f, 196f, 220f, 247f }, new[] { 98f, 110f, 87.3f, 98f }, 0.74f, "combat");
            sewerCombatMusicClip = MakePatternMusic("sewer_hunt_combat_loop", 18f, 0.36f, new[] { 174.6f, 196f, 146.8f, 174.6f, 164.8f, 146.8f, 130.8f, 164.8f }, new[] { 65.4f, 73.4f, 82.4f, 61.7f }, 0.68f, "dripcombat");
            bossCombatMusicClip = MakePatternMusic("crown_and_ashes_boss_loop", 19.2f, 0.40f, new[] { 196f, 233f, 261.6f, 311f, 261.6f, 233f, 185f, 196f }, new[] { 49f, 61.7f, 55f, 73.4f }, 0.90f, "boss");
            koboldCombatMusicClip = MakePatternMusic("kobold_hide_drums_loop", 18f, 0.29f, new[] { 294f, 349f, 330f, 392f, 294f, 440f, 349f, 330f }, new[] { 73.4f, 87.3f, 65.4f, 98f }, 0.78f, "skirmish");
            drowCombatMusicClip = MakePatternMusic("drow_nightblades_loop", 20f, 0.42f, new[] { 311f, 370f, 415.3f, 370f, 277f, 311f, 233f, 277f }, new[] { 77.8f, 92.5f, 69.3f, 103.8f }, 0.66f, "shadowcombat");
            demonCombatMusicClip = MakePatternMusic("red_rift_war_loop", 20f, 0.48f, new[] { 185f, 220f, 233f, 185f, 164.8f, 196f, 146.8f, 164.8f }, new[] { 46.2f, 55f, 41.2f, 61.7f }, 0.86f, "riftcombat");
            undeadCombatMusicClip = MakePatternMusic("bones_beneath_stone_loop", 19.2f, 0.40f, new[] { 246.9f, 277.2f, 220f, 196f, 233.1f, 185f, 207.7f, 174.6f }, new[] { 61.7f, 69.3f, 55f, 77.8f }, 0.70f, "bonecombat");
            zoneMusicClips["midgaard-city"] = MakePatternMusic("midgaard_lamps_loop", 20f, 0.48f, new[] { 330f, 392f, 440f, 392f, 330f, 294f, 330f, 392f }, new[] { 165f, 196f, 147f, 174.6f }, 0.38f, "bells");
            zoneMusicClips["midgaard-throne-room"] = MakePatternMusic("midgaard_throne_room_loop", 19.2f, 0.60f, new[] { 261.6f, 329.6f, 392f, 329.6f, 293.7f, 261.6f, 220f, 246.9f }, new[] { 65.4f, 82.4f, 73.4f, 98f }, 0.34f, "royal");
            zoneMusicClips["midgaard-merchant-hall"] = MakePatternMusic("midgaard_merchant_hall_loop", 19.2f, 0.48f, new[] { 329.6f, 392f, 440f, 493.9f, 440f, 392f, 349.2f, 392f }, new[] { 130.8f, 164.8f, 146.8f, 196f }, 0.32f, "market");
            zoneMusicClips["inner-ash-road"] = MakePatternMusic("old_road_walk_loop", 20f, 0.52f, new[] { 294f, 330f, 349f, 392f, 349f, 330f, 294f, 262f }, new[] { 147f, 165f, 130.8f, 147f }, 0.34f, "road");
            zoneMusicClips["salt-cisterns"] = MakePatternMusic("salt_cistern_drips_loop", 18f, 0.56f, new[] { 220f, 247f, 196f, 174.6f, 196f, 220f, 164.8f, 196f }, new[] { 82.4f, 98f, 73.4f, 87.3f }, 0.32f, "drip");
            zoneMusicClips["green-shrine-road"] = MakePatternMusic("green_shrine_teal_loop", 22f, 0.60f, new[] { 392f, 440f, 523f, 440f, 392f, 349f, 392f, 330f }, new[] { 196f, 174.6f, 220f, 196f }, 0.34f, "chime");
            zoneMusicClips["dusk-market"] = MakePatternMusic("dusk_market_ambush_loop", 18f, 0.42f, new[] { 294f, 330f, 370f, 330f, 277f, 294f, 247f, 294f }, new[] { 110f, 123.5f, 98f, 110f }, 0.46f, "stealth");
            zoneMusicClips["old-quarry"] = MakePatternMusic("old_quarry_stone_loop", 18f, 0.50f, new[] { 247f, 294f, 330f, 294f, 247f, 220f, 247f, 196f }, new[] { 98f, 123.5f, 87.3f, 98f }, 0.42f, "stone");
            zoneMusicClips["glass-warrens"] = MakePatternMusic("glass_warrens_shimmer_loop", 20f, 0.46f, new[] { 523f, 587f, 659f, 587f, 494f, 523f, 440f, 494f }, new[] { 130.8f, 164.8f, 146.8f, 196f }, 0.36f, "glass");
            zoneMusicClips["ash-fen"] = MakePatternMusic("ash_fen_haze_loop", 20f, 0.58f, new[] { 220f, 196f, 174.6f, 196f, 164.8f, 174.6f, 196f, 220f }, new[] { 82.4f, 73.4f, 98f, 87.3f }, 0.34f, "haze");
            zoneMusicClips["red-gate"] = MakePatternMusic("red_gate_omen_loop", 18f, 0.38f, new[] { 196f, 233f, 247f, 220f, 185f, 196f, 164.8f, 185f }, new[] { 65.4f, 73.4f, 82.4f, 98f }, 0.50f, "omen");
            zoneMusicClips["gloam-courts"] = MakePatternMusic("gloam_courts_echo_loop", 20f, 0.54f, new[] { 262f, 311f, 349f, 311f, 262f, 233f, 262f, 220f }, new[] { 87.3f, 110f, 98f, 130.8f }, 0.36f, "echo");
            zoneMusicClips["road"] = zoneMusicClips["inner-ash-road"];

            RegisterAdaptiveMusic(MusicDirectorRules.Muster, () => MakePatternMusic(
                "muster_by_firelight_loop", 24f, 0.60f,
                new[] { 329.6f, 392f, 493.9f, 440f, 392f, 349.2f, 329.6f, 293.7f, 329.6f, 369.9f, 440f, 392f, 349.2f, 329.6f, 293.7f, 246.9f },
                new[] { 82.4f, 98f, 73.4f, 110f, 82.4f, 65.4f, 73.4f, 98f }, 0.34f, "muster"));
            RegisterAdaptiveMusic(MusicDirectorRules.Victory, () => MakePatternMusic(
                "embers_carry_home_victory_loop", 16f, 0.40f,
                new[] { 261.6f, 329.6f, 392f, 523.3f, 493.9f, 440f, 392f, 329.6f, 349.2f, 440f, 523.3f, 659.3f, 587.3f, 523.3f, 440f, 392f },
                new[] { 130.8f, 164.8f, 196f, 146.8f, 174.6f, 220f, 196f, 164.8f }, 0.60f, "victory"));
            RegisterAdaptiveMusic(MusicDirectorRules.Defeat, () => MakePatternMusic(
                "ashes_on_the_road_defeat_loop", 22f, 0.72f,
                new[] { 293.7f, 261.6f, 220f, 196f, 174.6f, 196f, 220f, 164.8f, 196f, 174.6f, 146.8f, 130.8f, 146.8f, 164.8f, 130.8f, 110f },
                new[] { 73.4f, 65.4f, 55f, 49f, 61.7f, 55f, 41.2f, 49f }, 0.30f, "lament"));

            RegisterAdaptiveMusic(MusicDirectorRules.GrandHearth, () => MakePatternMusic(
                "four_names_by_the_fire_loop", 26f, 0.72f,
                new[] { 293.7f, 349.2f, 440f, 392f, 349.2f, 293.7f, 261.6f, 220f, 293.7f, 392f, 440f, 493.9f, 440f, 392f, 349.2f, 293.7f },
                new[] { 73.4f, 98f, 82.4f, 110f, 73.4f, 65.4f, 82.4f, 98f }, 0.28f, "camp"));
            RegisterAdaptiveMusic(MusicDirectorRules.WorldMapOverview, () => MakePatternMusic(
                "ashen_atlas_overview_loop", 25.2f, 0.78f,
                new[] { 293.7f, 440f, 349.2f, 392f, 293.7f, 493.9f, 392f, 349.2f, 293.7f, 392f, 440f, 523.3f, 440f, 392f, 349.2f, 293.7f },
                new[] { 73.4f, 110f, 87.3f, 98f, 73.4f, 123.5f, 98f, 82.4f }, 0.30f, "road"));
            RegisterAdaptiveMusic(MusicDirectorRules.GreenShrineTrainingRing, () => MakePatternMusic(
                "sparks_on_the_oathring_loop", 22f, 0.42f,
                new[] { 392f, 466.2f, 523.3f, 587.3f, 523.3f, 440f, 392f, 349.2f, 392f, 493.9f, 587.3f, 659.3f, 587.3f, 523.3f, 440f, 392f },
                new[] { 98f, 123.5f, 110f, 146.8f, 98f, 130.8f, 110f, 82.4f }, 0.50f, "watch"));
            RegisterAdaptiveMusic(MusicDirectorRules.OldQuarryForge, () => MakePatternMusic(
                "anvil_echoes_in_old_stone_loop", 24f, 0.56f,
                new[] { 220f, 261.6f, 293.7f, 329.6f, 293.7f, 261.6f, 246.9f, 220f, 196f, 246.9f, 293.7f, 349.2f, 329.6f, 293.7f, 261.6f, 220f },
                new[] { 55f, 73.4f, 65.4f, 82.4f, 55f, 87.3f, 73.4f, 49f }, 0.45f, "stone"));
            RegisterAdaptiveMusic(MusicDirectorRules.GloamDeepCrypt, () => MakePatternMusic(
                "the_crypt_keeps_its_names_loop", 28f, 0.88f,
                new[] { 246.9f, 261.6f, 392f, 246.9f, 349.2f, 261.6f, 440f, 246.9f, 196f, 207.7f, 329.6f, 196f, 293.7f, 207.7f, 369.9f, 196f },
                new[] { 61.7f, 65.4f, 98f, 61.7f, 87.3f, 65.4f, 110f, 61.7f }, 0.25f, "ruins"));
            RegisterAdaptiveMusic(MusicDirectorRules.GlassLoreLibrary, () => MakePatternMusic(
                "starlight_in_the_glass_index_loop", 24f, 0.52f,
                new[] { 493.9f, 587.3f, 698.5f, 784f, 698.5f, 659.3f, 587.3f, 523.3f, 587.3f, 698.5f, 880f, 784f, 698.5f, 659.3f, 587.3f, 493.9f },
                new[] { 123.5f, 146.8f, 174.6f, 110f, 130.8f, 164.8f, 146.8f, 98f }, 0.39f, "arcane"));
            RegisterAdaptiveMusic(MusicDirectorRules.DuskMarketHideout, () => MakePatternMusic(
                "lanterns_under_false_names_loop", 22f, 0.38f,
                new[] { 293.7f, 311.1f, 349.2f, 392f, 349.2f, 329.6f, 311.1f, 293.7f, 277.2f, 329.6f, 369.9f, 440f, 392f, 349.2f, 311.1f, 277.2f },
                new[] { 73.4f, 77.8f, 98f, 82.4f, 69.3f, 92.5f, 77.8f, 65.4f }, 0.60f, "stealth"));
            RegisterAdaptiveMusic(MusicDirectorRules.RedGateSeal, () => MakePatternMusic(
                "embers_at_the_broken_seal_loop", 24f, 0.50f,
                new[] { 220f, 329.6f, 246.9f, 415.3f, 220f, 349.2f, 293.7f, 246.9f, 440f, 659.3f, 493.9f, 830.6f, 440f, 698.5f, 587.3f, 493.9f },
                new[] { 55f, 82.4f, 61.7f, 103.8f, 55f, 87.3f, 73.4f, 61.7f }, 0.62f, "omen"));
            RegisterAdaptiveMusic(MusicDirectorRules.SaltCisternGate, () => MakePatternMusic(
                "chains_below_bellstone_loop", 26f, 0.68f,
                new[] { 164.8f, 196f, 174.6f, 146.8f, 130.8f, 146.8f, 123.5f, 110f, 146.8f, 174.6f, 164.8f, 130.8f, 123.5f, 110f, 98f, 82.4f },
                new[] { 41.2f, 49f, 46.2f, 36.7f, 41.2f, 32.7f, 36.7f, 30.9f }, 0.34f, "threshold"));
            RegisterAdaptiveMusic(MusicDirectorRules.AshFenAncientGrove, () => MakePatternMusic(
                "old_sap_under_ash_loop", 28f, 0.76f,
                new[] { 329.6f, 554.4f, 440f, 587.3f, 329.6f, 493.9f, 392f, 554.4f, 261.6f, 440f, 349.2f, 493.9f, 261.6f, 392f, 311.1f, 440f },
                new[] { 82.4f, 138.6f, 110f, 146.8f, 82.4f, 123.5f, 98f, 138.6f }, 0.29f, "grove"));

            RegisterAdaptiveMusic(MusicDirectorRules.MidgaardTemple, () => MakePatternMusic(
                "bells_over_temple_square_loop", 24f, 0.68f,
                new[] { 392f, 523.3f, 659.3f, 587.3f, 523.3f, 440f, 392f, 349.2f, 392f, 493.9f, 587.3f, 523.3f, 440f, 392f, 329.6f, 349.2f },
                new[] { 98f, 130.8f, 110f, 146.8f, 98f, 123.5f, 110f, 82.4f }, 0.30f, "sanctuary"));
            RegisterAdaptiveMusic(MusicDirectorRules.MidgaardMarket, () => MakePatternMusic(
                "lanterns_and_ledgers_loop", 22f, 0.42f,
                new[] { 329.6f, 392f, 440f, 493.9f, 440f, 523.3f, 493.9f, 392f, 349.2f, 440f, 493.9f, 587.3f, 523.3f, 493.9f, 440f, 392f },
                new[] { 130.8f, 164.8f, 146.8f, 196f, 130.8f, 174.6f, 146.8f, 164.8f }, 0.42f, "marketdance"));
            RegisterAdaptiveMusic(MusicDirectorRules.MidgaardTavernLane, () => MakePatternMusic(
                "wet_cobble_reel_loop", 22f, 0.375f,
                new[] { 392f, 440f, 493.9f, 587.3f, 523.3f, 493.9f, 440f, 392f, 329.6f, 392f, 440f, 523.3f, 493.9f, 440f, 392f, 349.2f },
                new[] { 98f, 123.5f, 110f, 146.8f, 82.4f, 110f, 98f, 123.5f }, 0.44f, "street"));
            RegisterAdaptiveMusic(MusicDirectorRules.MidgaardGateWatch, () => MakePatternMusic(
                "watchfires_on_the_wall_loop", 22f, 0.50f,
                new[] { 220f, 293.7f, 329.6f, 293.7f, 246.9f, 329.6f, 369.9f, 329.6f, 220f, 261.6f, 329.6f, 392f, 369.9f, 329.6f, 293.7f, 246.9f },
                new[] { 73.4f, 98f, 82.4f, 110f, 73.4f, 92.5f, 82.4f, 98f }, 0.40f, "watch"));
            RegisterAdaptiveMusic(MusicDirectorRules.MidgaardCisternMouth, () => MakePatternMusic(
                "under_the_bellstone_loop", 21f, 0.58f,
                new[] { 220f, 196f, 164.8f, 174.6f, 146.8f, 164.8f, 130.8f, 146.8f, 196f, 174.6f, 146.8f, 130.8f, 123.5f, 146.8f, 164.8f, 110f },
                new[] { 55f, 65.4f, 49f, 61.7f, 55f, 41.2f, 49f, 46.2f }, 0.35f, "threshold"));
            RegisterAdaptiveMusic(MusicDirectorRules.MidgaardRoyalApproach, () => MakePatternMusic(
                "banners_before_the_crown_loop", 22f, 0.60f,
                new[] { 261.6f, 329.6f, 392f, 493.9f, 440f, 392f, 329.6f, 293.7f, 329.6f, 392f, 523.3f, 493.9f, 440f, 392f, 349.2f, 329.6f },
                new[] { 65.4f, 82.4f, 98f, 73.4f, 82.4f, 110f, 98f, 73.4f }, 0.38f, "processional"));
            RegisterAdaptiveMusic(MusicDirectorRules.MidgaardRoad, () => MakePatternMusic(
                "last_lamps_east_loop", 22f, 0.52f,
                new[] { 293.7f, 329.6f, 392f, 440f, 392f, 349.2f, 329.6f, 293.7f, 261.6f, 329.6f, 369.9f, 392f, 349.2f, 329.6f, 293.7f, 246.9f },
                new[] { 98f, 110f, 82.4f, 98f, 73.4f, 82.4f, 65.4f, 73.4f }, 0.34f, "roadwatch"));

            RegisterAdaptiveMusic(MusicDirectorRules.RoadsideRest, () => MakePatternMusic(
                "a_fire_between_roads_loop", 24f, 0.64f,
                new[] { 329.6f, 392f, 440f, 392f, 349.2f, 329.6f, 293.7f, 329.6f, 392f, 493.9f, 440f, 392f, 349.2f, 293.7f, 329.6f, 261.6f },
                new[] { 82.4f, 98f, 110f, 73.4f, 82.4f, 65.4f, 73.4f, 98f }, 0.28f, "camp"));
            RegisterAdaptiveMusic(MusicDirectorRules.SacredGround, () => MakePatternMusic(
                "old_green_prayer_loop", 24f, 0.72f,
                new[] { 349.2f, 440f, 523.3f, 659.3f, 587.3f, 523.3f, 440f, 392f, 440f, 523.3f, 698.5f, 659.3f, 587.3f, 523.3f, 440f, 349.2f },
                new[] { 87.3f, 110f, 130.8f, 98f, 110f, 146.8f, 130.8f, 87.3f }, 0.30f, "sanctuary"));
            RegisterAdaptiveMusic(MusicDirectorRules.UnderstoneThreshold, () => MakePatternMusic(
                "mouth_of_the_deep_loop", 22f, 0.66f,
                new[] { 164.8f, 196f, 174.6f, 146.8f, 130.8f, 146.8f, 123.5f, 110f, 146.8f, 174.6f, 164.8f, 130.8f, 123.5f, 110f, 98f, 82.4f },
                new[] { 41.2f, 49f, 46.2f, 36.7f, 41.2f, 32.7f, 36.7f, 30.9f }, 0.38f, "threshold"));
            RegisterAdaptiveMusic(MusicDirectorRules.ForgottenRuins, () => MakePatternMusic(
                "names_worn_away_loop", 24f, 0.74f,
                new[] { 246.9f, 293.7f, 261.6f, 220f, 196f, 220f, 174.6f, 196f, 233.1f, 277.2f, 246.9f, 207.7f, 185f, 207.7f, 164.8f, 146.8f },
                new[] { 61.7f, 73.4f, 65.4f, 55f, 49f, 61.7f, 46.2f, 55f }, 0.27f, "ruins"));
            RegisterAdaptiveMusic(MusicDirectorRules.ArcaneThreshold, () => MakePatternMusic(
                "glass_and_quiet_stars_loop", 22f, 0.46f,
                new[] { 493.9f, 587.3f, 698.5f, 784f, 659.3f, 587.3f, 523.3f, 440f, 523.3f, 659.3f, 880f, 784f, 698.5f, 659.3f, 587.3f, 493.9f },
                new[] { 123.5f, 146.8f, 174.6f, 110f, 130.8f, 164.8f, 146.8f, 98f }, 0.34f, "arcane"));
            RegisterAdaptiveMusic(MusicDirectorRules.HuntedRoad, () => MakePatternMusic(
                "footsteps_behind_loop", 18f, 0.30f,
                new[] { 220f, 246.9f, 261.6f, 293.7f, 261.6f, 246.9f, 233.1f, 220f, 246.9f, 277.2f, 311.1f, 293.7f, 277.2f, 261.6f, 246.9f, 220f },
                new[] { 55f, 61.7f, 65.4f, 49f, 55f, 69.3f, 61.7f, 46.2f }, 0.63f, "pursuit"));
            RegisterAdaptiveMusic(MusicDirectorRules.AncientGrove, () => MakePatternMusic(
                "roots_remember_loop", 24f, 0.72f,
                new[] { 293.7f, 392f, 440f, 523.3f, 440f, 392f, 349.2f, 293.7f, 329.6f, 440f, 493.9f, 587.3f, 493.9f, 440f, 392f, 329.6f },
                new[] { 73.4f, 98f, 110f, 82.4f, 73.4f, 110f, 98f, 65.4f }, 0.29f, "grove"));
            RegisterAdaptiveMusic(MusicDirectorRules.FactionCamp, () => MakePatternMusic(
                "smoke_across_the_road_loop", 20f, 0.40f,
                new[] { 196f, 246.9f, 293.7f, 329.6f, 293.7f, 261.6f, 246.9f, 220f, 196f, 233.1f, 277.2f, 311.1f, 293.7f, 261.6f, 220f, 196f },
                new[] { 49f, 61.7f, 73.4f, 55f, 49f, 69.3f, 61.7f, 46.2f }, 0.52f, "warcamp"));

            RegisterAdaptiveMusic(MusicDirectorRules.CombatRatfolk, () => MakePatternMusic(
                "ratfolk_plague_march_loop", 19.2f, 0.32f,
                new[] { 174.6f, 207.7f, 233.1f, 261.6f, 233.1f, 196f, 185f, 174.6f, 207.7f, 246.9f, 277.2f, 261.6f, 233.1f, 207.7f, 196f, 164.8f },
                new[] { 43.7f, 55f, 61.7f, 49f, 43.7f, 65.4f, 55f, 41.2f }, 0.74f, "plaguecombat"));
            RegisterAdaptiveMusic(MusicDirectorRules.CombatArcaneDuel, () => MakePatternMusic(
                "sigils_crossed_arcane_duel_loop", 20f, 0.31f,
                new[] { 392f, 493.9f, 587.3f, 698.5f, 659.3f, 523.3f, 440f, 493.9f, 587.3f, 784f, 698.5f, 659.3f, 587.3f, 493.9f, 440f, 392f },
                new[] { 98f, 123.5f, 146.8f, 110f, 130.8f, 164.8f, 123.5f, 98f }, 0.72f, "duel"));
            RegisterAdaptiveMusic(MusicDirectorRules.CombatElite, () => MakePatternMusic(
                "steel_against_the_chosen_loop", 19.2f, 0.30f,
                new[] { 220f, 329.6f, 415.3f, 293.7f, 220f, 349.2f, 261.6f, 415.3f, 440f, 659.3f, 830.6f, 587.3f, 440f, 698.5f, 523.3f, 830.6f },
                new[] { 55f, 82.4f, 103.8f, 73.4f, 55f, 87.3f, 65.4f, 103.8f }, 0.82f, "elitecombat"));
            RegisterAdaptiveMusic(MusicDirectorRules.CombatLastStand, () => MakePatternMusic(
                "one_more_turn_last_stand_loop", 18f, 0.28f,
                new[] { 196f, 220f, 261.6f, 293.7f, 329.6f, 349.2f, 392f, 440f, 220f, 261.6f, 293.7f, 349.2f, 392f, 440f, 493.9f, 523.3f },
                new[] { 49f, 55f, 65.4f, 73.4f, 49f, 61.7f, 69.3f, 82.4f }, 0.86f, "laststand"));
            RegisterAdaptiveMusic(MusicDirectorRules.CombatKoboldKing, () => MakePatternMusic(
                "crooked_crown_kobold_king_loop", 21.6f, 0.27f,
                new[] { 293.7f, 349.2f, 392f, 466.2f, 440f, 392f, 349.2f, 293.7f, 329.6f, 392f, 493.9f, 587.3f, 523.3f, 466.2f, 392f, 349.2f },
                new[] { 55f, 73.4f, 65.4f, 87.3f, 55f, 82.4f, 73.4f, 49f }, 0.93f, "kingcombat"));
            RegisterAdaptiveMusic(MusicDirectorRules.CombatDemonLord, () => MakePatternMusic(
                "the_rift_walks_demon_lord_loop", 22f, 0.38f,
                new[] { 164.8f, 196f, 220f, 233.1f, 196f, 174.6f, 146.8f, 164.8f, 185f, 220f, 261.6f, 246.9f, 220f, 196f, 174.6f, 146.8f },
                new[] { 32.7f, 41.2f, 46.2f, 36.7f, 32.7f, 49f, 41.2f, 30.9f }, 0.96f, "titancombat"));

            RegisterImportedMusicRoute(MusicDirectorRules.Muster, "muster_by_firelight_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.Victory, "embers_carry_home_victory_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.Defeat, "ashes_on_the_road_defeat_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.GrandHearth, "four_names_by_the_fire_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.WorldMapOverview, "ashen_atlas_overview_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.GreenShrineTrainingRing, "sparks_on_the_oathring_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.OldQuarryForge, "anvil_echoes_in_old_stone_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.GloamDeepCrypt, "the_crypt_keeps_its_names_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.GlassLoreLibrary, "starlight_in_the_glass_index_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.DuskMarketHideout, "lanterns_under_false_names_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.RedGateSeal, "embers_at_the_broken_seal_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.SaltCisternGate, "chains_below_bellstone_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.AshFenAncientGrove, "old_sap_under_ash_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.MidgaardTemple, "bells_over_temple_square_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.MidgaardMarket, "lanterns_and_ledgers_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.MidgaardTavernLane, "wet_cobble_reel_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.MidgaardGateWatch, "watchfires_on_the_wall_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.MidgaardCisternMouth, "under_the_bellstone_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.MidgaardRoyalApproach, "banners_before_the_crown_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.MidgaardRoad, "last_lamps_east_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.RoadsideRest, "a_fire_between_roads_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.SacredGround, "old_green_prayer_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.UnderstoneThreshold, "mouth_of_the_deep_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.ForgottenRuins, "names_worn_away_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.ArcaneThreshold, "glass_and_quiet_stars_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.HuntedRoad, "footsteps_behind_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.AncientGrove, "roots_remember_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.FactionCamp, "smoke_across_the_road_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.CombatRatfolk, "ratfolk_plague_march_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.CombatArcaneDuel, "sigils_crossed_arcane_duel_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.CombatElite, "steel_against_the_chosen_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.CombatLastStand, "one_more_turn_last_stand_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.CombatKoboldKing, "crooked_crown_kobold_king_loop");
            RegisterImportedMusicRoute(MusicDirectorRules.CombatDemonLord, "the_rift_walks_demon_lord_loop");
            LoadImportedMusicOverrides();
            ApplyImportedMusicOverrides();
        }

        private void RegisterImportedMusicRoute(string routeKey, string clipName)
        {
            if (string.IsNullOrWhiteSpace(routeKey) || string.IsNullOrWhiteSpace(clipName)) return;
            importedMusicRouteNames[routeKey] = clipName;
        }

        private void LoadImportedMusicOverrides()
        {
            importedMusicClips.Clear();
            importedMusicKeys.Clear();
            AudioClip[] clips;
            try
            {
                clips = Resources.LoadAll<AudioClip>(ImportedMusicResourcePath);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Original music bank unavailable; procedural score retained. " + ex.Message);
                return;
            }

            Array.Sort(clips, (left, right) => string.CompareOrdinal(
                left == null ? "" : left.name,
                right == null ? "" : right.name));
            foreach (AudioClip clip in clips)
            {
                if (clip == null) continue;
                string key = (clip.name ?? "").Trim().ToLowerInvariant();
                if (string.IsNullOrEmpty(key))
                {
                    Debug.LogWarning("Ignoring original music asset without a clip name.");
                    continue;
                }

                if (!importedMusicKeys.Add(key))
                {
                    Debug.LogWarning("Ignoring duplicate original music master: " + key);
                    continue;
                }

                if (clip.frequency < 22050 || clip.channels < 1 || clip.channels > 2 || clip.length < 8f)
                {
                    importedMusicKeys.Remove(key);
                    Debug.LogWarning("Ignoring invalid original music master: " + key);
                    continue;
                }

                if (clip.loadState == AudioDataLoadState.Failed)
                {
                    importedMusicKeys.Remove(key);
                    Debug.LogWarning("Original music master failed to import: " + key);
                    continue;
                }

                importedMusicClips[key] = clip;
            }

            if (importedMusicKeys.Count > 0)
            {
                Debug.Log("Original music masters available: " + importedMusicKeys.Count);
            }
        }

        private void ApplyImportedMusicOverrides()
        {
            tavernMusicClip = ImportedMusicOrFallback(tavernMusicClip);
            combatMusicClip = ImportedMusicOrFallback(combatMusicClip);
            sewerCombatMusicClip = ImportedMusicOrFallback(sewerCombatMusicClip);
            bossCombatMusicClip = ImportedMusicOrFallback(bossCombatMusicClip);
            koboldCombatMusicClip = ImportedMusicOrFallback(koboldCombatMusicClip);
            drowCombatMusicClip = ImportedMusicOrFallback(drowCombatMusicClip);
            demonCombatMusicClip = ImportedMusicOrFallback(demonCombatMusicClip);
            undeadCombatMusicClip = ImportedMusicOrFallback(undeadCombatMusicClip);

            string[] zoneKeys = zoneMusicClips.Keys.ToArray();
            foreach (string zoneKey in zoneKeys)
            {
                zoneMusicClips[zoneKey] = ImportedMusicOrFallback(zoneMusicClips[zoneKey]);
            }
        }

        private AudioClip ImportedMusicOrFallback(AudioClip fallback)
        {
            if (fallback == null) return null;
            return TryImportedMusic(fallback.name, out AudioClip imported) ? imported : fallback;
        }

        private bool TryImportedMusic(string clipName, out AudioClip imported)
        {
            imported = null;
            string key = (clipName ?? "").Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(key) || !importedMusicClips.TryGetValue(key, out AudioClip clip) || clip == null)
            {
                return false;
            }

            if (clip.loadState == AudioDataLoadState.Unloaded && !clip.LoadAudioData())
            {
                Debug.LogWarning("Original music master failed to begin loading; procedural fallback retained: " + key);
                return false;
            }

            if (clip.loadState == AudioDataLoadState.Failed)
            {
                Debug.LogWarning("Original music master failed to load; procedural fallback retained: " + key);
                return false;
            }

            imported = clip;
            return true;
        }

        private void RegisterAdaptiveMusic(string key, Func<AudioClip> factory)
        {
            if (string.IsNullOrWhiteSpace(key) || factory == null) return;
            adaptiveMusicFactories[key] = factory;
        }

        private AudioClip AdaptiveMusicClip(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            if (adaptiveMusicClips.TryGetValue(key, out AudioClip cached)) return cached;
            if (!adaptiveMusicFactories.TryGetValue(key, out Func<AudioClip> factory)) return null;
            try
            {
                if (importedMusicRouteNames.TryGetValue(key, out string clipName)
                    && TryImportedMusic(clipName, out AudioClip imported))
                {
                    adaptiveMusicClips[key] = imported;
                    return imported;
                }

                AudioClip clip = factory();
                adaptiveMusicClips[key] = clip;
                return clip;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Could not compose music track '{key}': {ex.Message}");
                adaptiveMusicClips[key] = null;
                return null;
            }
        }

        private AudioClip MakeSound(string name, float startFrequency, float endFrequency, float duration, float volume, string shape)
        {
            const int sampleRate = 32000;
            float safeDuration = Mathf.Max(0.035f, duration);
            int sampleCount = Mathf.Max(1, Mathf.CeilToInt(sampleRate * safeDuration));
            float[] data = new float[sampleCount];
            string cue = (name ?? "sound").Trim().ToLowerInvariant();
            string voice = (shape ?? "sine").Trim().ToLowerInvariant();
            uint noiseState = StableAudioSeed(cue + "|" + voice);
            float phase = 0f;
            float subPhase = 0f;
            float overtonePhase = 0f;
            float smoothNoise = 0f;
            float deepNoise = 0f;
            bool impactLike = voice == "thud"
                || voice == "boom"
                || cue.Contains("contact")
                || cue.Contains("impact")
                || cue.Contains("blocked")
                || cue.Contains("guard");
            bool uiLike = cue == "ui"
                || cue == "turn"
                || cue == "save"
                || cue == "cache"
                || cue.Contains("dialogue");
            bool wooden = cue.Contains("wood") || cue.Contains("door") || cue.Contains("tree");
            bool stoneLike = cue.Contains("stone") || cue.Contains("rock");
            bool fieldLike = cue.StartsWith("field", StringComparison.Ordinal);

            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                float progress = sampleCount <= 1 ? 1f : i / (float)(sampleCount - 1);
                float contour = Mathf.SmoothStep(0f, 1f, progress);
                float rawNoise = NextAudioNoise(ref noiseState);
                float smoothRate = voice == "smoke" ? 0.014f : voice == "rustle" || voice == "slash" ? 0.055f : 0.09f;
                smoothNoise += (rawNoise - smoothNoise) * smoothRate;
                deepNoise += (rawNoise - deepNoise) * 0.006f;
                float brightNoise = rawNoise - smoothNoise;
                float bandNoise = smoothNoise - deepNoise;
                float frequency = Mathf.Max(24f, Mathf.Lerp(startFrequency, endFrequency, contour));
                frequency *= 1f + deepNoise * (uiLike ? 0.0025f : 0.008f);
                phase += Mathf.PI * 2f * frequency / sampleRate;
                subPhase += Mathf.PI * 2f * Mathf.Max(22f, frequency * (wooden ? 0.38f : 0.50f)) / sampleRate;
                overtonePhase += Mathf.PI * 2f * Mathf.Min(sampleRate * 0.42f, frequency * (stoneLike ? 3.12f : 2.03f)) / sampleRate;

                float sine = Mathf.Sin(phase);
                float sub = Mathf.Sin(subPhase);
                float overtone = Mathf.Sin(overtonePhase);
                float body;
                if (voice == "square")
                {
                    // Odd harmonics keep the old assertive pulse without a hard, aliased square edge.
                    body = sine * 0.66f
                        + Mathf.Sin(phase * 3f) * 0.22f
                        + Mathf.Sin(phase * 5f) * 0.09f
                        + bandNoise * 0.06f;
                }
                else if (voice == "thud")
                {
                    body = sub * 0.48f + sine * 0.25f + smoothNoise * 0.18f + brightNoise * 0.09f;
                }
                else if (voice == "slash")
                {
                    float sweep = Mathf.Sin(phase * 0.52f + progress * Mathf.PI * 1.5f);
                    body = brightNoise * 0.42f + bandNoise * 0.25f + sweep * 0.24f + sine * 0.09f;
                }
                else if (voice == "rustle")
                {
                    body = brightNoise * 0.34f + bandNoise * 0.42f + sine * 0.16f + sub * 0.08f;
                }
                else if (voice == "boom")
                {
                    body = sub * 0.50f + Mathf.Sin(subPhase * 0.51f) * 0.20f + sine * 0.17f + smoothNoise * 0.13f;
                }
                else if (voice == "chime")
                {
                    body = sine * 0.48f + overtone * 0.25f + Mathf.Sin(phase * 3.01f) * 0.15f + sub * 0.08f + bandNoise * 0.04f;
                }
                else if (voice == "smoke")
                {
                    float breath = 0.72f + 0.28f * Mathf.Sin(progress * Mathf.PI * 3f + deepNoise);
                    body = deepNoise * 0.42f + smoothNoise * 0.34f + bandNoise * 0.16f + sub * 0.08f;
                    body *= breath;
                }
                else
                {
                    body = sine * 0.62f + sub * 0.16f + overtone * 0.14f + bandNoise * 0.08f;
                }

                float transientDecay = uiLike ? 150f : impactLike ? 92f : 58f;
                float transient = Mathf.Exp(-t * transientDecay)
                    * (brightNoise * (impactLike ? 0.44f : 0.24f) + overtone * 0.24f + sub * (impactLike ? 0.20f : 0.06f));
                float secondary = 0f;
                if (cue.Contains("break") || cue.Contains("gate") || cue.Contains("door"))
                {
                    secondary = AudioDecayPulse(t, safeDuration * 0.52f, wooden ? 30f : 42f)
                        * (smoothNoise * 0.25f + sub * 0.22f + overtone * 0.18f);
                }
                else if (fieldLike)
                {
                    secondary = AudioDecayPulse(t, safeDuration * 0.38f, 24f)
                        * (bandNoise * 0.18f + overtone * 0.12f);
                }

                if (cue.Contains("fire"))
                {
                    float crackle = Mathf.Max(0f, Mathf.Abs(brightNoise) - 0.42f) * (0.38f + progress * 0.42f);
                    body += crackle * (rawNoise >= 0f ? 1f : -1f);
                }
                if (cue.Contains("ice") || cue.Contains("holy") || cue.Contains("shrine"))
                {
                    body += overtone * Mathf.Exp(-progress * 3.2f) * 0.12f;
                }
                if (wooden) body += smoothNoise * Mathf.Sin(progress * Mathf.PI) * 0.10f;
                if (stoneLike) body += sub * Mathf.Exp(-progress * 4.5f) * 0.14f;

                float attackSeconds = uiLike ? 0.0015f : impactLike ? 0.0025f : voice == "smoke" ? 0.018f : 0.006f;
                float attack = Mathf.Clamp01(t / attackSeconds);
                float releaseEdge = Mathf.Clamp01((safeDuration - t) / (uiLike ? 0.005f : 0.012f));
                float envelope;
                if (voice == "chime" || voice == "sine")
                {
                    envelope = attack * Mathf.Exp(-progress * (uiLike ? 4.6f : 2.25f));
                }
                else if (voice == "smoke")
                {
                    envelope = attack * Mathf.Pow(Mathf.Sin(progress * Mathf.PI), 0.62f);
                }
                else if (voice == "rustle" || voice == "slash")
                {
                    envelope = attack * Mathf.Pow(1f - progress, 0.52f);
                }
                else
                {
                    envelope = attack * Mathf.Pow(1f - progress, impactLike ? 0.82f : 0.66f);
                }

                float tail = (sine * 0.10f + overtone * 0.07f)
                    * Mathf.Exp(-progress * (voice == "chime" ? 2.2f : 4.8f));
                float mixed = (body * envelope + transient + secondary + tail) * releaseEdge * volume * 1.32f;
                data[i] = SoftLimitAudio(mixed);
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip MakeSkillSound(string name, string style, float startFrequency, float endFrequency, float duration, float volume)
        {
            const int sampleRate = 32000;
            float safeDuration = Mathf.Max(0.08f, duration);
            int sampleCount = Mathf.Max(1, Mathf.CeilToInt(sampleRate * safeDuration));
            float[] data = new float[sampleCount];
            string motion = (style ?? "skill").Trim().ToLowerInvariant();
            uint noiseState = StableAudioSeed((name ?? "skill") + "|skill|" + motion);
            float phase = 0f;
            float subPhase = 0f;
            float edgePhase = 0f;
            float airNoise = 0f;
            float lowNoise = 0f;
            bool impact = motion.EndsWith("-impact", StringComparison.Ordinal);

            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                float progress = sampleCount <= 1 ? 1f : i / (float)(sampleCount - 1);
                float contour = Mathf.SmoothStep(0f, 1f, progress);
                float rawNoise = NextAudioNoise(ref noiseState);
                airNoise += (rawNoise - airNoise) * 0.075f;
                lowNoise += (rawNoise - lowNoise) * 0.012f;
                float brightNoise = rawNoise - airNoise;
                float bandNoise = airNoise - lowNoise;
                float frequency = Mathf.Max(28f, Mathf.Lerp(startFrequency, endFrequency, contour));
                phase += Mathf.PI * 2f * frequency / sampleRate;
                subPhase += Mathf.PI * 2f * Mathf.Max(24f, frequency * 0.43f) / sampleRate;
                edgePhase += Mathf.PI * 2f * Mathf.Min(sampleRate * 0.40f, frequency * 2.17f) / sampleRate;

                float tone = Mathf.Sin(phase);
                float sub = Mathf.Sin(subPhase);
                float edge = Mathf.Sin(edgePhase);
                float body = 0f;
                float transient = 0f;
                float tail = 0f;
                float envelope;

                float pulseCount = motion == "arrow-rain" ? 5f : motion == "volley" ? 3f : motion == "whirlwind-impact" ? 3f : 2f;
                float pulsePosition = progress * pulseCount;
                float pulseLocal = pulsePosition - Mathf.Floor(pulsePosition);
                float impulseTrain = Mathf.Pow(Mathf.Sin(pulseLocal * Mathf.PI), 7f);

                switch (motion)
                {
                    case "charge":
                        body = sub * 0.33f + tone * 0.18f + bandNoise * (0.18f + progress * 0.38f) + brightNoise * progress * 0.20f;
                        transient = AudioDecayPulse(t, safeDuration * 0.66f, 42f) * (sub * 0.28f + brightNoise * 0.22f);
                        envelope = Mathf.Clamp01(t / 0.008f) * Mathf.Pow(1f - progress, 0.28f);
                        break;
                    case "whirlwind":
                        body = (brightNoise * 0.36f + bandNoise * 0.24f + tone * 0.17f)
                            * (0.70f + 0.30f * Mathf.Sin(progress * Mathf.PI * 6f));
                        tail = edge * Mathf.Exp(-progress * 3.6f) * 0.10f;
                        envelope = Mathf.Clamp01(t / 0.005f) * Mathf.Pow(Mathf.Sin(progress * Mathf.PI), 0.48f);
                        break;
                    case "execute":
                        body = sub * (0.24f + progress * 0.16f) + edge * progress * 0.22f + bandNoise * 0.20f;
                        transient = AudioDecayPulse(t, safeDuration * 0.70f, 58f) * (edge * 0.48f + brightNoise * 0.22f);
                        envelope = Mathf.Clamp01(t / 0.006f) * Mathf.Pow(1f - progress, 0.24f);
                        break;
                    case "ambush":
                        body = brightNoise * 0.38f + bandNoise * 0.22f + edge * 0.20f + tone * 0.10f;
                        transient = Mathf.Exp(-t * 105f) * (brightNoise * 0.34f + edge * 0.28f);
                        envelope = Mathf.Clamp01(t / 0.002f) * Mathf.Pow(1f - progress, 0.48f);
                        break;
                    case "eviscerate":
                        body = brightNoise * 0.35f + bandNoise * 0.30f + tone * 0.14f + sub * 0.12f;
                        transient = (Mathf.Exp(-t * 82f) + AudioDecayPulse(t, safeDuration * 0.42f, 74f) * 0.72f)
                            * (brightNoise * 0.30f + edge * 0.20f);
                        envelope = Mathf.Clamp01(t / 0.003f) * Mathf.Pow(1f - progress, 0.42f);
                        break;
                    case "charge-impact":
                        body = sub * 0.52f + tone * 0.17f + lowNoise * 0.19f + bandNoise * 0.12f;
                        transient = Mathf.Exp(-t * 72f) * (brightNoise * 0.38f + edge * 0.20f + sub * 0.24f);
                        tail = sub * Mathf.Exp(-progress * 4.2f) * 0.18f;
                        envelope = Mathf.Clamp01(t / 0.0018f) * Mathf.Pow(1f - progress, 0.92f);
                        break;
                    case "whirlwind-impact":
                        body = (brightNoise * 0.36f + bandNoise * 0.26f + edge * 0.18f) * (0.46f + impulseTrain * 0.54f);
                        transient = impulseTrain * (brightNoise * 0.26f + tone * 0.14f);
                        tail = sub * Mathf.Exp(-progress * 4.8f) * 0.12f;
                        envelope = Mathf.Clamp01(t / 0.002f) * Mathf.Pow(1f - progress, 0.55f);
                        break;
                    case "execute-impact":
                        body = sub * 0.38f + tone * 0.18f + bandNoise * 0.22f + edge * 0.15f;
                        transient = Mathf.Exp(-t * 90f) * (brightNoise * 0.34f + edge * 0.35f + sub * 0.16f);
                        tail = edge * Mathf.Exp(-progress * 3.8f) * 0.12f;
                        envelope = Mathf.Clamp01(t / 0.0015f) * Mathf.Pow(1f - progress, 0.78f);
                        break;
                    case "ambush-impact":
                        body = brightNoise * 0.40f + edge * 0.24f + bandNoise * 0.20f + tone * 0.08f;
                        transient = Mathf.Exp(-t * 128f) * (edge * 0.42f + brightNoise * 0.34f);
                        tail = tone * Mathf.Exp(-progress * 6f) * 0.08f;
                        envelope = Mathf.Clamp01(t / 0.0012f) * Mathf.Pow(1f - progress, 0.62f);
                        break;
                    case "eviscerate-impact":
                        body = brightNoise * 0.34f + bandNoise * 0.32f + sub * 0.17f + edge * 0.12f;
                        transient = (Mathf.Exp(-t * 96f) + AudioDecayPulse(t, safeDuration * 0.28f, 88f) * 0.78f)
                            * (brightNoise * 0.32f + edge * 0.24f);
                        tail = lowNoise * Mathf.Exp(-progress * 3.3f) * 0.12f;
                        envelope = Mathf.Clamp01(t / 0.0018f) * Mathf.Pow(1f - progress, 0.60f);
                        break;
                    case "stealth":
                        body = bandNoise * 0.30f + lowNoise * 0.23f + tone * 0.15f + edge * 0.12f;
                        tail = edge * Mathf.Exp(-progress * 2.6f) * 0.16f;
                        envelope = Mathf.Pow(Mathf.Sin(progress * Mathf.PI), 0.72f);
                        break;
                    case "smoke":
                        body = lowNoise * 0.46f + airNoise * 0.28f + bandNoise * 0.16f + sub * 0.08f;
                        envelope = Mathf.Clamp01(t / 0.018f) * Mathf.Pow(Mathf.Sin(progress * Mathf.PI), 0.58f);
                        break;
                    case "rally":
                        body = tone * 0.38f + Mathf.Sin(phase * 2.01f) * 0.22f + Mathf.Sin(phase * 3.02f) * 0.12f + sub * 0.14f;
                        transient = AudioDecayPulse(t, safeDuration * 0.42f, 32f) * edge * 0.18f;
                        tail = edge * Mathf.Exp(-progress * 2.2f) * 0.13f;
                        envelope = Mathf.Clamp01(t / 0.008f) * Mathf.Pow(1f - progress, 0.34f);
                        break;
                    case "focus":
                        body = tone * 0.30f + edge * progress * 0.29f + bandNoise * 0.14f + sub * 0.09f;
                        transient = AudioDecayPulse(t, safeDuration * 0.70f, 78f) * (edge * 0.38f + brightNoise * 0.14f);
                        envelope = Mathf.Clamp01(t / 0.004f) * Mathf.Pow(1f - progress, 0.24f);
                        break;
                    case "pinning":
                        body = tone * 0.24f + edge * progress * 0.24f + bandNoise * 0.28f + brightNoise * 0.12f;
                        transient = AudioDecayPulse(t, safeDuration * 0.62f, 72f) * (edge * 0.30f + brightNoise * 0.16f);
                        envelope = Mathf.Clamp01(t / 0.004f) * Mathf.Pow(1f - progress, 0.36f);
                        break;
                    case "volley":
                        body = bandNoise * 0.30f + brightNoise * 0.20f + tone * 0.14f;
                        transient = impulseTrain * (edge * 0.26f + brightNoise * 0.22f);
                        tail = lowNoise * Mathf.Exp(-progress * 3.4f) * 0.10f;
                        envelope = Mathf.Clamp01(t / 0.003f) * Mathf.Pow(1f - progress, 0.46f);
                        break;
                    case "arrow-rain":
                        body = bandNoise * 0.34f + brightNoise * 0.24f + tone * 0.10f;
                        transient = impulseTrain * (brightNoise * 0.30f + edge * 0.18f);
                        tail = lowNoise * Mathf.Exp(-progress * 2.8f) * 0.12f;
                        envelope = Mathf.Clamp01(t / 0.004f) * Mathf.Pow(1f - progress, 0.38f);
                        break;
                    case "mark":
                        body = tone * 0.40f + edge * 0.25f + Mathf.Sin(phase * 3.01f) * 0.12f + bandNoise * 0.06f;
                        transient = AudioDecayPulse(t, safeDuration * 0.46f, 48f) * edge * 0.18f;
                        tail = edge * Mathf.Exp(-progress * 2.6f) * 0.13f;
                        envelope = Mathf.Clamp01(t / 0.004f) * Mathf.Pow(1f - progress, 0.54f);
                        break;
                    default:
                        body = tone * 0.34f + sub * 0.18f + bandNoise * 0.26f + edge * 0.12f;
                        transient = Mathf.Exp(-t * 68f) * (brightNoise * 0.22f + edge * 0.18f);
                        envelope = Mathf.Clamp01(t / 0.004f) * Mathf.Pow(1f - progress, impact ? 0.72f : 0.48f);
                        break;
                }

                float releaseEdge = Mathf.Clamp01((safeDuration - t) / 0.010f);
                float mixed = (body * envelope + transient + tail) * releaseEdge * volume * (impact ? 1.48f : 1.34f);
                data[i] = SoftLimitAudio(mixed);
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private uint StableAudioSeed(string value)
        {
            unchecked
            {
                uint hash = 2166136261u;
                string text = value ?? "";
                for (int i = 0; i < text.Length; i++)
                {
                    hash ^= text[i];
                    hash *= 16777619u;
                }
                return hash == 0u ? 0x6D2B79F5u : hash;
            }
        }

        private float NextAudioNoise(ref uint state)
        {
            if (state == 0u) state = 0x6D2B79F5u;
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;
            return (state & 0x00FFFFFFu) / 8388607.5f - 1f;
        }

        private float AudioDecayPulse(float time, float start, float decay)
        {
            if (time < start) return 0f;
            return Mathf.Exp(-(time - start) * Mathf.Max(1f, decay));
        }

        private float SoftLimitAudio(float sample)
        {
            float shaped = sample / (1f + Mathf.Abs(sample) * 0.28f);
            return Mathf.Clamp(shaped, -0.92f, 0.92f);
        }

        private AudioClip MakeMaterialImpact(string name, string material)
        {
            const int sampleRate = 22050;
            float duration = material == "shield" ? 0.20f : material == "plate" ? 0.18f : 0.14f;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                float noise = PseudoNoise(i * 5 + name.Length * 97) * 2f - 1f;
                float impulse = Mathf.Exp(-t * (material == "flesh" ? 34f : 48f));
                float sample;
                switch (material)
                {
                    case "plate":
                        sample = noise * impulse * 0.26f
                            + Mathf.Sin(t * Mathf.PI * 2f * 410f) * Mathf.Exp(-t * 15f) * 0.46f
                            + Mathf.Sin(t * Mathf.PI * 2f * 890f) * Mathf.Exp(-t * 23f) * 0.20f;
                        break;
                    case "mail":
                        sample = noise * impulse * 0.34f
                            + Mathf.Sin(t * Mathf.PI * 2f * 305f) * Mathf.Exp(-t * 22f) * 0.32f
                            + Mathf.Sin(t * Mathf.PI * 2f * (720f + 90f * Mathf.Sin(t * 76f))) * Mathf.Exp(-t * 29f) * 0.22f;
                        break;
                    case "shield":
                        sample = noise * impulse * 0.20f
                            + Mathf.Sin(t * Mathf.PI * 2f * 118f) * Mathf.Exp(-t * 20f) * 0.38f
                            + Mathf.Sin(t * Mathf.PI * 2f * 540f) * Mathf.Exp(-t * 13f) * 0.42f;
                        break;
                    case "leather":
                        sample = noise * impulse * 0.45f
                            + Mathf.Sin(t * Mathf.PI * 2f * 150f) * Mathf.Exp(-t * 30f) * 0.32f;
                        break;
                    default:
                        sample = noise * impulse * 0.22f
                            + Mathf.Sin(t * Mathf.PI * 2f * 92f) * Mathf.Exp(-t * 27f) * 0.58f;
                        break;
                }

                float attack = Mathf.Clamp01(t / 0.0015f);
                float release = Mathf.Clamp01((duration - t) / 0.025f);
                data[i] = Mathf.Clamp(sample * attack * release * 0.72f, -0.86f, 0.86f);
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip MakeRatVoice(string name, string style)
        {
            const int sampleRate = 22050;
            int chirps = style == "chitter" ? 4 : style == "cast" ? 3 : style == "death" ? 2 : 1;
            float duration = style == "cast" ? 0.28f : style == "death" ? 0.34f : style == "chitter" ? 0.18f : 0.16f;
            float start = style == "attack" ? 430f : style == "death" ? 760f : style == "hurt" ? 880f : 720f;
            float end = style == "attack" ? 1040f : style == "cast" ? 190f : style == "death" ? 105f : style == "hurt" ? 280f : 470f;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[sampleCount];
            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                float progress = Mathf.Clamp01(t / duration);
                float chirpPosition = progress * chirps;
                float local = chirpPosition - Mathf.Floor(chirpPosition);
                int chirp = Mathf.Min(chirps - 1, Mathf.FloorToInt(chirpPosition));
                float direction = style == "chitter" && (chirp & 1) == 1 ? 1f - local : local;
                float frequency = Mathf.Lerp(start, end, Mathf.Clamp01((chirp + direction) / chirps));
                frequency *= 1f + Mathf.Sin(t * Mathf.PI * 2f * 31f) * 0.045f;
                phase += Mathf.PI * 2f * frequency / sampleRate;
                float chirpEnvelope = Mathf.Sin(local * Mathf.PI);
                chirpEnvelope *= chirpEnvelope;
                float noise = PseudoNoise(i * 7 + name.Length * 43) * 2f - 1f;
                float voice = Mathf.Sin(phase) * 0.58f + (Mathf.Sin(phase) >= 0f ? 1f : -1f) * 0.22f + noise * 0.20f;
                if (style == "cast") voice += Mathf.Sin(t * Mathf.PI * 2f * 92f) * 0.22f;
                float release = Mathf.Clamp01((duration - t) / Mathf.Max(0.025f, duration * 0.28f));
                data[i] = Mathf.Clamp(voice * chirpEnvelope * release * 0.34f, -0.72f, 0.72f);
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip MakeCreatureVoice(string name, string faction, string style)
        {
            const int sampleRate = 22050;
            faction = (faction ?? "").ToLowerInvariant();
            style = (style ?? "").ToLowerInvariant();
            float duration = style == "death" ? (faction == "demon" ? 0.58f : 0.44f)
                : style == "cast" ? 0.38f
                : style == "alert" ? 0.34f
                : style == "attack" ? 0.24f
                : style == "hurt" ? 0.20f
                : 0.14f;
            float baseFrequency = faction == "kobold" ? 470f
                : faction == "drow" ? 230f
                : faction == "demon" ? 82f
                : 128f;
            float direction = style == "death" ? 0.28f
                : style == "cast" ? 1.55f
                : style == "attack" || style == "alert" ? 1.28f
                : style == "hurt" ? 0.70f
                : 0.86f;
            int sampleCount = Mathf.Max(1, Mathf.CeilToInt(sampleRate * duration));
            float[] data = new float[sampleCount];
            float phase = 0f;
            float secondaryPhase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                float progress = Mathf.Clamp01(t / duration);
                float contour = Mathf.SmoothStep(0f, 1f, progress);
                float frequency = Mathf.Lerp(baseFrequency, baseFrequency * direction, contour);
                if (faction == "kobold") frequency *= 1f + Mathf.Sin(t * Mathf.PI * 2f * 24f) * 0.065f;
                if (faction == "drow") frequency *= 1f + Mathf.Sin(t * Mathf.PI * 2f * 5.5f) * 0.025f;
                if (faction == "demon") frequency *= 1f + Mathf.Sin(t * Mathf.PI * 2f * 17f) * 0.035f;
                phase += Mathf.PI * 2f * frequency / sampleRate;
                secondaryPhase += Mathf.PI * 2f * (frequency * (faction == "undead" ? 2.65f : 1.49f)) / sampleRate;
                float noise = PseudoNoise(i * 19 + name.Length * 83) * 2f - 1f;
                float sample;
                if (faction == "kobold")
                {
                    float yip = Mathf.Sin(phase) * 0.42f
                        + (Mathf.Sin(phase * 0.52f) >= 0f ? 1f : -1f) * 0.20f
                        + noise * 0.20f;
                    float chatter = style == "alert"
                        ? 0.62f + 0.38f * (Mathf.Sin(t * Mathf.PI * 2f * 13f) >= 0f ? 1f : 0f)
                        : 1f;
                    sample = yip * chatter;
                }
                else if (faction == "drow")
                {
                    float whisper = noise * (0.26f + 0.12f * Mathf.Sin(t * 37f));
                    float tone = Mathf.Sin(phase) * 0.34f + Mathf.Sin(secondaryPhase) * 0.15f;
                    float shimmer = style == "cast" ? Mathf.Sin(phase * 3.01f) * 0.20f : 0f;
                    sample = tone + whisper + shimmer;
                }
                else if (faction == "demon")
                {
                    float sub = Mathf.Sin(phase * 0.50f) * 0.42f;
                    float growl = Mathf.Sin(phase) * 0.24f + noise * 0.24f;
                    float rasp = (Mathf.Sin(secondaryPhase) >= 0f ? 1f : -1f) * 0.12f;
                    sample = (sub + growl + rasp) * (0.82f + Mathf.Sin(t * 31f) * 0.18f);
                }
                else
                {
                    float moan = Mathf.Sin(phase) * 0.34f + Mathf.Sin(phase * 0.51f) * 0.26f;
                    float rattlePulse = Mathf.Exp(-((t * 10f) % 1f) * 8f);
                    float rattle = (noise * 0.28f + Mathf.Sin(secondaryPhase) * 0.16f) * rattlePulse;
                    sample = moan + rattle;
                }

                if (style == "step")
                {
                    sample = noise * 0.26f
                        + Mathf.Sin(phase * (faction == "demon" ? 0.45f : 0.72f)) * 0.34f;
                }
                else if (style == "cast")
                {
                    sample += Mathf.Sin(t * Mathf.PI * 2f * (faction == "drow" ? 930f : 310f)) * progress * 0.15f;
                }

                float attack = Mathf.Clamp01(t / (style == "step" ? 0.002f : 0.012f));
                float release = Mathf.Clamp01((duration - t) / Mathf.Max(0.025f, duration * 0.38f));
                float body = style == "death" ? Mathf.Pow(1f - progress, 0.35f) : Mathf.Sin(progress * Mathf.PI);
                float amplitude = faction == "demon" ? 0.46f : faction == "kobold" ? 0.34f : 0.38f;
                data[i] = Mathf.Clamp(sample * attack * release * body * amplitude, -0.82f, 0.82f);
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip MakeServiceSound(string name, string style)
        {
            const int sampleRate = 22050;
            float duration = style == "rune" ? 0.48f : style == "weapon" ? 0.38f : style == "armor" ? 0.32f : 0.24f;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[sampleCount];
            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                float progress = Mathf.Clamp01(t / duration);
                float noise = PseudoNoise(i * 17 + name.Length * 89) * 2f - 1f;
                float sample;
                if (style == "coin")
                {
                    float a = t >= 0.010f ? Mathf.Exp(-(t - 0.010f) * 34f) : 0f;
                    float b = t >= 0.078f ? Mathf.Exp(-(t - 0.078f) * 39f) * 0.78f : 0f;
                    float c = t >= 0.142f ? Mathf.Exp(-(t - 0.142f) * 44f) * 0.52f : 0f;
                    float ring = a + b + c;
                    sample = Mathf.Sin(t * Mathf.PI * 2f * 1040f) * ring * 0.48f
                        + Mathf.Sin(t * Mathf.PI * 2f * 1565f) * ring * 0.27f
                        + noise * ring * 0.08f;
                }
                else if (style == "armor")
                {
                    float a = t >= 0.018f ? Mathf.Exp(-(t - 0.018f) * 30f) : 0f;
                    float b = t >= 0.126f ? Mathf.Exp(-(t - 0.126f) * 34f) * 0.74f : 0f;
                    float c = t >= 0.232f ? Mathf.Exp(-(t - 0.232f) * 42f) * 0.48f : 0f;
                    float strike = a + b + c;
                    sample = noise * strike * 0.19f
                        + Mathf.Sin(t * Mathf.PI * 2f * 286f) * strike * 0.34f
                        + Mathf.Sin(t * Mathf.PI * 2f * 735f) * strike * 0.29f
                        + Mathf.Sin(t * Mathf.PI * 2f * 1110f) * strike * 0.12f;
                }
                else if (style == "weapon")
                {
                    float frequency = Mathf.Lerp(1380f, 310f, progress);
                    phase += Mathf.PI * 2f * frequency / sampleRate;
                    float scrape = Mathf.Sin(progress * Mathf.PI) * Mathf.Clamp01((0.30f - t) / 0.06f);
                    float ringT = t - 0.245f;
                    float ring = ringT >= 0f ? Mathf.Exp(-ringT * 17f) : 0f;
                    sample = (noise * 0.42f + Mathf.Sin(phase) * 0.20f) * scrape
                        + Mathf.Sin(t * Mathf.PI * 2f * 612f) * ring * 0.44f
                        + Mathf.Sin(t * Mathf.PI * 2f * 1224f) * ring * 0.16f;
                }
                else
                {
                    float frequency = Mathf.Lerp(170f, 930f, Mathf.SmoothStep(0f, 1f, progress));
                    phase += Mathf.PI * 2f * frequency / sampleRate;
                    float gather = Mathf.Sin(progress * Mathf.PI);
                    float sealT = t - 0.34f;
                    float seal = sealT >= 0f ? Mathf.Exp(-sealT * 16f) : 0f;
                    sample = Mathf.Sin(phase) * gather * 0.34f
                        + Mathf.Sin(phase * 1.503f) * gather * 0.22f
                        + (Mathf.Sin(phase * 3.01f) * 0.18f + noise * 0.10f) * seal;
                }

                float edge = Mathf.Min(Mathf.Clamp01(t / 0.003f), Mathf.Clamp01((duration - t) / 0.018f));
                data[i] = Mathf.Clamp(sample * edge * 0.76f, -0.82f, 0.82f);
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip MakeSpellSound(string name, string texture, float startFrequency, float endFrequency, float duration, float volume)
        {
            const int sampleRate = 32000;
            float safeDuration = Mathf.Max(0.08f, duration);
            int sampleCount = Mathf.Max(1, Mathf.CeilToInt(sampleRate * safeDuration));
            float[] data = new float[sampleCount];
            string cue = (name ?? "spell").Trim().ToLowerInvariant();
            string element = (texture ?? "death").Trim().ToLowerInvariant();
            uint noiseState = StableAudioSeed(cue + "|spell|" + element);
            float phase = 0f;
            float subPhase = 0f;
            float shimmerPhase = 0f;
            float detunePhase = 0f;
            float lowNoise = 0f;
            float midNoise = 0f;
            float airNoise = 0f;
            bool isCast = cue.StartsWith("cast", StringComparison.Ordinal);
            bool isDeathBurst = cue.Contains("deathburst");
            bool isGreaterSummon = cue.Contains("greatersummon");
            bool isAscendance = cue.Contains("ascendance");
            bool isTempest = cue.Contains("tempest");
            bool isSeal = cue.Contains("seal");
            bool isVeil = cue.Contains("veil");

            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                float progress = sampleCount <= 1 ? 1f : i / (float)(sampleCount - 1);
                float contour = Mathf.SmoothStep(0f, 1f, progress);
                float rawNoise = NextAudioNoise(ref noiseState);
                lowNoise += (rawNoise - lowNoise) * 0.008f;
                midNoise += (rawNoise - midNoise) * 0.052f;
                airNoise += (rawNoise - airNoise) * 0.19f;
                float bodyNoise = midNoise - lowNoise;
                float brightNoise = rawNoise - airNoise;
                float airyNoise = airNoise - midNoise;

                float frequency = Mathf.Clamp(Mathf.Lerp(startFrequency, endFrequency, contour), 24f, sampleRate * 0.38f);
                float pitchWander = element == "shock"
                    ? 1f + bodyNoise * 0.034f
                    : element == "rift" || element == "death"
                        ? 1f + lowNoise * 0.018f
                        : 1f + lowNoise * 0.006f;
                frequency *= pitchWander;
                phase += Mathf.PI * 2f * frequency / sampleRate;
                subPhase += Mathf.PI * 2f * Mathf.Max(22f, frequency * (element == "holy" ? 0.50f : 0.42f)) / sampleRate;
                shimmerPhase += Mathf.PI * 2f * Mathf.Min(sampleRate * 0.41f, frequency * (element == "frost" ? 3.07f : 2.03f)) / sampleRate;
                detunePhase += Mathf.PI * 2f * Mathf.Min(sampleRate * 0.38f, frequency * 1.013f + 3.7f) / sampleRate;

                float core = Mathf.Sin(phase);
                float sub = Mathf.Sin(subPhase);
                float shimmer = Mathf.Sin(shimmerPhase);
                float detuned = Mathf.Sin(detunePhase);
                float upper = Mathf.Sin(phase * 2.01f);
                float sparkPosition = progress * (isTempest ? 5f : element == "shock" ? 3f : 2f);
                float sparkLocal = sparkPosition - Mathf.Floor(sparkPosition);
                float sparkPulse = Mathf.Pow(Mathf.Sin(sparkLocal * Mathf.PI), 9f);
                float body = 0f;
                float transient = 0f;
                float tail = 0f;

                switch (element)
                {
                    case "fire":
                        float crackleGate = Mathf.Max(0f, Mathf.Abs(brightNoise) - 0.34f) * 1.65f;
                        float crackle = crackleGate * (rawNoise >= 0f ? 1f : -1f);
                        body = core * 0.25f
                            + sub * 0.19f
                            + bodyNoise * (0.28f + progress * 0.18f)
                            + airyNoise * 0.12f
                            + crackle * (isCast ? 0.14f + progress * 0.24f : 0.31f);
                        transient = isCast
                            ? AudioDecayPulse(t, safeDuration * 0.76f, 48f) * (brightNoise * 0.31f + shimmer * 0.16f)
                            : Mathf.Exp(-t * 76f) * (brightNoise * 0.38f + sub * 0.19f);
                        tail = (sub * 0.11f + bodyNoise * 0.08f) * Mathf.Exp(-progress * 3.2f);
                        break;
                    case "meteor":
                        float debrisA = AudioDecayPulse(t, safeDuration * 0.23f, 26f);
                        float debrisB = AudioDecayPulse(t, safeDuration * 0.49f, 31f);
                        body = Mathf.Sin(subPhase * 0.52f) * 0.40f
                            + sub * 0.24f
                            + core * 0.14f
                            + lowNoise * 0.18f
                            + bodyNoise * (0.10f + progress * 0.26f);
                        transient = Mathf.Exp(-t * 46f) * (sub * 0.34f + brightNoise * 0.31f)
                            + (debrisA + debrisB * 0.72f) * (bodyNoise * 0.24f + shimmer * 0.12f);
                        tail = (sub * 0.17f + lowNoise * 0.11f) * Mathf.Exp(-progress * 2.4f);
                        break;
                    case "frost":
                        body = core * 0.35f
                            + upper * 0.21f
                            + shimmer * 0.20f
                            + detuned * 0.13f
                            + airyNoise * 0.08f;
                        transient = isCast
                            ? AudioDecayPulse(t, safeDuration * 0.80f, 66f) * (shimmer * 0.31f + brightNoise * 0.15f)
                            : Mathf.Exp(-t * 105f) * (shimmer * 0.38f + brightNoise * 0.21f);
                        tail = (shimmer * 0.17f + detuned * 0.10f) * Mathf.Exp(-progress * 2.1f);
                        break;
                    case "shock":
                        float electricEdge = core * 0.52f
                            + Mathf.Sin(phase * 3f) * 0.18f
                            + Mathf.Sin(phase * 5f) * 0.07f;
                        body = electricEdge * 0.39f
                            + shimmer * 0.16f
                            + brightNoise * (0.16f + sparkPulse * 0.21f)
                            + bodyNoise * 0.16f;
                        transient = sparkPulse * (brightNoise * 0.36f + shimmer * 0.24f)
                            + (isCast
                                ? AudioDecayPulse(t, safeDuration * 0.78f, 74f)
                                : Mathf.Exp(-t * 118f)) * (brightNoise * 0.34f + upper * 0.17f);
                        tail = detuned * Mathf.Exp(-progress * 3.8f) * 0.10f;
                        break;
                    case "holy":
                        body = core * 0.39f
                            + upper * 0.23f
                            + Mathf.Sin(phase * 3.01f) * 0.12f
                            + detuned * 0.13f
                            + shimmer * 0.08f;
                        transient = isCast
                            ? AudioDecayPulse(t, safeDuration * 0.72f, 42f) * (shimmer * 0.24f + upper * 0.15f)
                            : Mathf.Exp(-t * 62f) * (shimmer * 0.27f + upper * 0.17f);
                        tail = (shimmer * 0.19f + detuned * 0.12f + core * 0.08f) * Mathf.Exp(-progress * 1.9f);
                        break;
                    case "nature":
                        body = sub * 0.30f
                            + core * 0.19f
                            + lowNoise * 0.24f
                            + bodyNoise * 0.22f
                            + airyNoise * 0.07f;
                        transient = isCast
                            ? AudioDecayPulse(t, safeDuration * 0.67f, 31f) * (bodyNoise * 0.29f + sub * 0.19f)
                            : Mathf.Exp(-t * 52f) * (lowNoise * 0.31f + sub * 0.23f);
                        tail = (lowNoise * 0.14f + sub * 0.10f) * Mathf.Exp(-progress * 2.8f);
                        break;
                    case "rift":
                        float portalPulse = 0.72f + 0.28f * Mathf.Sin(progress * Mathf.PI * (isGreaterSummon ? 7f : 4f));
                        body = (sub * 0.30f
                            + core * 0.18f
                            + detuned * 0.20f
                            + Mathf.Sin(phase * 1.51f) * 0.12f
                            + bodyNoise * 0.17f
                            + lowNoise * 0.10f) * portalPulse;
                        transient = isCast
                            ? AudioDecayPulse(t, safeDuration * 0.79f, 35f) * (detuned * 0.28f + brightNoise * 0.18f)
                            : Mathf.Exp(-t * 48f) * (sub * 0.27f + brightNoise * 0.23f);
                        tail = (detuned * 0.15f + sub * 0.13f) * Mathf.Exp(-progress * 2.2f);
                        break;
                    default:
                        float rasp = core * 0.46f
                            + Mathf.Sin(phase * 3f) * 0.15f
                            + Mathf.Sin(phase * 5f) * 0.06f;
                        body = sub * 0.32f
                            + rasp * 0.26f
                            + detuned * 0.13f
                            + lowNoise * 0.15f
                            + bodyNoise * 0.16f;
                        transient = isCast
                            ? AudioDecayPulse(t, safeDuration * 0.75f, 30f) * (bodyNoise * 0.28f + sub * 0.23f)
                            : Mathf.Exp(-t * 43f) * (sub * 0.31f + brightNoise * 0.22f);
                        tail = (sub * 0.17f + detuned * 0.11f + lowNoise * 0.08f) * Mathf.Exp(-progress * 2.0f);
                        break;
                }

                // Signature cues get distinct event timing instead of merely receiving a different pitch.
                float signature = 0f;
                if (isDeathBurst)
                {
                    float burstStart = isCast ? safeDuration * 0.72f : 0f;
                    float burst = AudioDecayPulse(t, burstStart, 18f)
                        + AudioDecayPulse(t, burstStart + safeDuration * 0.16f, 28f) * 0.60f;
                    signature += burst * (sub * 0.32f + bodyNoise * 0.25f + brightNoise * 0.16f);
                }
                if (isGreaterSummon)
                {
                    float gate = Mathf.Pow(Mathf.Sin(progress * Mathf.PI * 3f), 8f);
                    signature += gate * (detuned * 0.23f + sub * 0.18f + bodyNoise * 0.13f);
                }
                if (isAscendance)
                {
                    signature += (shimmer * 0.20f + upper * 0.13f) * progress * progress;
                }
                if (isTempest)
                {
                    signature += sparkPulse * (brightNoise * 0.28f + shimmer * 0.24f + upper * 0.12f);
                }
                if (isSeal)
                {
                    float sealTime = isCast ? safeDuration * 0.73f : safeDuration * 0.08f;
                    signature += AudioDecayPulse(t, sealTime, 44f)
                        * (shimmer * 0.29f + upper * 0.19f + sub * 0.12f);
                }
                if (isVeil)
                {
                    signature += (airyNoise * 0.19f + detuned * 0.12f)
                        * Mathf.Pow(Mathf.Sin(progress * Mathf.PI), 0.58f);
                }

                float attackSeconds = isCast
                    ? Mathf.Max(0.010f, safeDuration * 0.12f)
                    : element == "shock" || element == "frost" ? 0.0012f : 0.0022f;
                float attack = Mathf.Clamp01(t / attackSeconds);
                float envelope;
                if (isCast)
                {
                    float gather = 0.34f + 0.66f * Mathf.SmoothStep(0f, 1f, progress);
                    envelope = attack * gather * Mathf.Pow(1f - progress, 0.12f);
                }
                else
                {
                    float decayPower = element == "holy" || element == "frost" ? 0.48f
                        : element == "meteor" || element == "death" ? 0.72f
                        : 0.60f;
                    envelope = attack * Mathf.Pow(1f - progress, decayPower);
                }

                float releaseEdge = Mathf.Clamp01((safeDuration - t) / (isCast ? 0.014f : 0.010f));
                float mixed = (body * envelope + transient + signature + tail)
                    * releaseEdge
                    * volume
                    * (isCast ? 1.48f : 1.58f);
                data[i] = SoftLimitAudio(mixed);
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip MakeAmbientSound(string name, string style)
        {
            const int sampleRate = 22050;
            float duration = style == "rain" ? 1.80f
                : style == "tavern" ? 1.65f
                : style == "hearth" ? 1.35f
                : style == "wind" || style == "market" ? 1.40f
                : style == "city" ? 1.20f
                : style == "bell" ? 1.10f
                : style == "drum" ? 1.05f
                : 0.86f;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[sampleCount];
            float smoothNoise = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                float progress = Mathf.Clamp01(t / duration);
                float noise = PseudoNoise(i * 13 + name.Length * 71) * 2f - 1f;
                smoothNoise = Mathf.Lerp(smoothNoise, noise, style == "wind" || style == "rain" ? 0.006f : 0.035f);
                float sample;
                if (style == "bell")
                {
                    float first = Mathf.Exp(-t * 3.8f);
                    float secondT = t - 0.52f;
                    float second = secondT >= 0f ? Mathf.Exp(-secondT * 5.2f) * 0.44f : 0f;
                    float envelope = first + second;
                    sample = (Mathf.Sin(t * Mathf.PI * 2f * 523f) * 0.48f
                        + Mathf.Sin(t * Mathf.PI * 2f * 784f) * 0.29f
                        + Mathf.Sin(t * Mathf.PI * 2f * 1047f) * 0.16f) * envelope;
                }
                else if (style == "forge")
                {
                    float hitA = t >= 0.04f ? Mathf.Exp(-(t - 0.04f) * 30f) : 0f;
                    float hitB = t >= 0.34f ? Mathf.Exp(-(t - 0.34f) * 32f) : 0f;
                    float hitC = t >= 0.62f ? Mathf.Exp(-(t - 0.62f) * 35f) : 0f;
                    float hit = hitA + hitB * 0.76f + hitC * 0.54f;
                    sample = noise * hit * 0.25f
                        + Mathf.Sin(t * Mathf.PI * 2f * 390f) * hit * 0.42f
                        + Mathf.Sin(t * Mathf.PI * 2f * 930f) * hit * 0.22f;
                }
                else if (style == "gate")
                {
                    float clankA = t >= 0.05f ? Mathf.Exp(-(t - 0.05f) * 24f) : 0f;
                    float clankB = t >= 0.30f ? Mathf.Exp(-(t - 0.30f) * 27f) : 0f;
                    float chain = clankA + clankB * 0.72f;
                    float groan = Mathf.Sin(t * Mathf.PI * 2f * (82f - progress * 24f)) * Mathf.Sin(progress * Mathf.PI);
                    sample = noise * chain * 0.22f
                        + Mathf.Sin(t * Mathf.PI * 2f * 470f) * chain * 0.34f
                        + groan * 0.24f;
                }
                else if (style == "drip")
                {
                    float dropA = t >= 0.07f ? Mathf.Exp(-(t - 0.07f) * 19f) : 0f;
                    float dropB = t >= 0.48f ? Mathf.Exp(-(t - 0.48f) * 23f) * 0.62f : 0f;
                    float drop = dropA + dropB;
                    sample = Mathf.Sin(t * Mathf.PI * 2f * (980f - progress * 610f)) * drop * 0.54f
                        + Mathf.Sin(t * Mathf.PI * 2f * 142f) * drop * 0.18f
                        + smoothNoise * drop * 0.15f;
                }
                else if (style == "drum")
                {
                    float beatA = t >= 0.08f ? Mathf.Exp(-(t - 0.08f) * 14f) : 0f;
                    float beatB = t >= 0.48f ? Mathf.Exp(-(t - 0.48f) * 16f) * 0.78f : 0f;
                    float beatC = t >= 0.82f ? Mathf.Exp(-(t - 0.82f) * 18f) * 0.54f : 0f;
                    float beat = beatA + beatB + beatC;
                    sample = Mathf.Sin(t * Mathf.PI * 2f * 74f) * beat * 0.56f
                        + noise * beat * 0.14f;
                }
                else if (style == "stone")
                {
                    float fall = t >= 0.12f ? Mathf.Exp(-(t - 0.12f) * 8f) : 0f;
                    float chip = t >= 0.50f ? Mathf.Exp(-(t - 0.50f) * 22f) * 0.48f : 0f;
                    sample = smoothNoise * fall * 0.28f
                        + noise * chip * 0.24f
                        + Mathf.Sin(t * Mathf.PI * 2f * 106f) * (fall + chip) * 0.26f;
                }
                else if (style == "market")
                {
                    float crowd = 0.58f + Mathf.Sin(t * Mathf.PI * 2f * 0.72f) * 0.18f;
                    float cartT = t - 0.76f;
                    float cart = cartT >= 0f ? Mathf.Exp(-cartT * 24f) : 0f;
                    sample = smoothNoise * crowd * 0.34f
                        + Mathf.Sin(t * Mathf.PI * 2f * 96f) * (0.10f + 0.04f * Mathf.Sin(t * 5.1f))
                        + (noise * 0.18f + Mathf.Sin(t * Mathf.PI * 2f * 310f) * 0.10f) * cart;
                }
                else if (style == "rain")
                {
                    float wash = 0.68f + Mathf.Sin(t * Mathf.PI * 2f * 0.43f) * 0.12f;
                    float dropA = t >= 0.28f ? Mathf.Exp(-(t - 0.28f) * 24f) : 0f;
                    float dropB = t >= 0.91f ? Mathf.Exp(-(t - 0.91f) * 27f) * 0.74f : 0f;
                    float dropC = t >= 1.42f ? Mathf.Exp(-(t - 1.42f) * 30f) * 0.48f : 0f;
                    float drops = dropA + dropB + dropC;
                    sample = smoothNoise * wash * 0.48f
                        + noise * 0.10f
                        + Mathf.Sin(t * Mathf.PI * 2f * 1280f) * drops * 0.09f;
                }
                else if (style == "tavern")
                {
                    float crowd = 0.56f + Mathf.Sin(t * Mathf.PI * 2f * 0.61f) * 0.16f;
                    float clinkA = t >= 0.46f ? Mathf.Exp(-(t - 0.46f) * 36f) : 0f;
                    float clinkB = t >= 1.18f ? Mathf.Exp(-(t - 1.18f) * 42f) * 0.66f : 0f;
                    float clink = clinkA + clinkB;
                    sample = smoothNoise * crowd * 0.31f
                        + Mathf.Sin(t * Mathf.PI * 2f * 108f) * (0.08f + crowd * 0.035f)
                        + (Mathf.Sin(t * Mathf.PI * 2f * 780f) * 0.20f + noise * 0.10f) * clink;
                }
                else if (style == "hearth")
                {
                    float crackleA = t >= 0.16f ? Mathf.Exp(-(t - 0.16f) * 38f) : 0f;
                    float crackleB = t >= 0.58f ? Mathf.Exp(-(t - 0.58f) * 46f) * 0.80f : 0f;
                    float crackleC = t >= 1.02f ? Mathf.Exp(-(t - 1.02f) * 52f) * 0.56f : 0f;
                    float crackle = crackleA + crackleB + crackleC;
                    sample = smoothNoise * 0.18f
                        + noise * crackle * 0.42f
                        + Mathf.Sin(t * Mathf.PI * 2f * 74f) * Mathf.Sin(progress * Mathf.PI) * 0.10f;
                }
                else if (style == "city")
                {
                    float distant = Mathf.Exp(-t * 2.6f);
                    sample = smoothNoise * Mathf.Sin(progress * Mathf.PI) * 0.20f
                        + (Mathf.Sin(t * Mathf.PI * 2f * 392f) * 0.20f
                            + Mathf.Sin(t * Mathf.PI * 2f * 588f) * 0.10f) * distant;
                }
                else
                {
                    float gust = Mathf.Sin(progress * Mathf.PI);
                    float sway = 0.68f + Mathf.Sin(t * Mathf.PI * 2f * 0.43f) * 0.24f;
                    sample = smoothNoise * gust * sway * 0.54f
                        + Mathf.Sin(t * Mathf.PI * 2f * 71f) * gust * 0.07f;
                }

                float edge = Mathf.Min(Mathf.Clamp01(t / 0.018f), Mathf.Clamp01((duration - t) / 0.035f));
                data[i] = Mathf.Clamp(sample * edge * 0.56f, -0.72f, 0.72f);
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private float PseudoNoise(int index)
        {
            float v = Mathf.Sin(index * 12.9898f + 78.233f) * 43758.5453f;
            return v - Mathf.Floor(v);
        }

        private AudioClip MakeTavernMusic()
        {
            const int sampleRate = 22050;
            const float seconds = 24f;
            const float beat = 0.375f;
            int sampleCount = Mathf.CeilToInt(sampleRate * seconds);
            float[] data = new float[sampleCount];
            float[] melody =
            {
                392f, 440f, 494f, 440f, 392f, 330f, 349f, 392f,
                294f, 330f, 392f, 330f, 262f, 294f, 330f, 392f,
                440f, 494f, 523f, 494f, 440f, 392f, 349f, 330f,
                294f, 330f, 349f, 392f, 330f, 294f, 262f, 294f
            };
            float[] harmony = { 196f, 247f, 220f, 262f, 174.6f, 220f, 196f, 247f };
            float[] response = { 0f, 587f, 659f, 587f, 0f, 523f, 494f, 440f };
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                int globalBeat = Mathf.FloorToInt(t / beat);
                int m = globalBeat % melody.Length;
                int h = Mathf.FloorToInt(t / (beat * 4f)) % harmony.Length;
                int phraseBeat = globalBeat % 16;
                int responseIndex = Mathf.FloorToInt(t / (beat * 2f)) % response.Length;
                float beatT = (t % beat) / beat;
                float barT = (t % (beat * 4f)) / (beat * 4f);
                float halfBarT = (t % (beat * 2f)) / (beat * 2f);
                float pluckEnv = Mathf.Exp(-beatT * 6.2f);
                float bassEnv = Mathf.Exp(-halfBarT * 2.35f);
                float phraseLift = globalBeat >= 32 ? 1.05f : 0.94f;
                float sway = 0.79f + Mathf.Sin(t * Mathf.PI * 2f / 6f) * 0.07f;
                float lute = (Triangle(t * melody[m]) * 0.74f + Triangle(t * melody[m] * 2.005f) * 0.18f) * pluckEnv * 0.255f;
                float low = Mathf.Sin(t * harmony[h] * Mathf.PI * 2f) * bassEnv * 0.125f;
                float drone = Triangle(t * harmony[h] * 0.5f) * (0.042f + Mathf.Sin(t * Mathf.PI * 2f / 12f) * 0.010f);
                float cup = Mathf.Sin(t * 1210f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 14f) * (phraseBeat == 6 || phraseBeat == 14 ? 0.040f : 0f);
                float drumAccent = phraseBeat % 4 == 0 ? 0.044f : phraseBeat % 4 == 2 ? 0.021f : 0f;
                float drum = (PseudoNoise(i) * 2f - 1f) * Mathf.Exp(-beatT * 17f) * drumAccent;
                bool responsePhrase = phraseBeat >= 8 && phraseBeat <= 14 && response[responseIndex] > 0f;
                float responseEnv = Mathf.Sin(Mathf.Clamp01(halfBarT) * Mathf.PI);
                float fiddle = responsePhrase
                    ? (Mathf.Sin(t * response[responseIndex] * Mathf.PI * 2f)
                        + Triangle(t * response[responseIndex] * 0.5f) * 0.20f) * responseEnv * 0.039f
                    : 0f;
                float room = (PseudoNoise(i + 137) * 2f - 1f) * 0.0035f;
                float loopEdge = Mathf.Min(Mathf.Clamp01(t / 0.075f), Mathf.Clamp01((seconds - t) / 0.075f));
                float sample = (lute * phraseLift + low + drone + cup + drum + fiddle + room) * sway * loopEdge;
                data[i] = Mathf.Clamp(sample, -0.78f, 0.78f);
            }

            AudioClip clip = AudioClip.Create("tavern_storm_hearth_ensemble_loop", sampleCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip MakePatternMusic(string name, float seconds, float beat, float[] melody, float[] bass, float energy, string texture)
        {
            const int sampleRate = 22050;
            int sampleCount = Mathf.CeilToInt(sampleRate * Mathf.Max(4f, seconds));
            float[] data = new float[sampleCount];
            if (melody == null || melody.Length == 0) melody = new[] { 294f, 330f, 392f, 330f };
            if (bass == null || bass.Length == 0) bass = new[] { 98f, 110f };
            beat = Mathf.Max(0.22f, beat);
            energy = Mathf.Clamp01(energy);
            texture = (texture ?? "").Trim().ToLowerInvariant();
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                int m = Mathf.FloorToInt(t / beat) % melody.Length;
                int b = Mathf.FloorToInt(t / (beat * 2f)) % bass.Length;
                float beatT = (t % beat) / beat;
                float barT = (t % (beat * 4f)) / (beat * 4f);
                float phrase = 0.78f + Mathf.Sin(t * Mathf.PI * 2f / Mathf.Max(3f, seconds * 0.5f)) * 0.12f;
                bool ringingLead = texture == "glass"
                    || texture == "chime"
                    || texture == "bells"
                    || texture == "sanctuary"
                    || texture == "arcane"
                    || texture == "duel"
                    || texture == "victory";
                float leadEnv = ringingLead ? Mathf.Exp(-beatT * 4.4f) : Mathf.Exp(-beatT * 5.8f);
                float lowEnv = Mathf.Exp(-((t % (beat * 2f)) / (beat * 2f)) * 2.4f);
                float lead = (Triangle(t * melody[m]) * 0.54f + Mathf.Sin(t * melody[m] * Mathf.PI * 2f) * 0.24f) * leadEnv;
                float low = Mathf.Sin(t * bass[b] * Mathf.PI * 2f) * lowEnv * 0.54f;
                float air = 0f;
                float tick = 0f;
                float counter = 0f;
                float noise = PseudoNoise(i) * 2f - 1f;
                if (texture == "combat")
                {
                    tick = noise * Mathf.Exp(-barT * 22f) * 0.58f + (m % 2 == 0 ? Mathf.Sin(t * 88f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 10f) * 0.42f : 0f);
                    lead += Mathf.Sin(t * melody[m] * 0.5f * Mathf.PI * 2f) * 0.26f;
                }
                else if (texture == "dripcombat")
                {
                    tick = noise * Mathf.Exp(-barT * 18f) * 0.34f
                        + Mathf.Sin(t * 1180f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 15f) * (m % 3 == 0 ? 0.18f : 0f);
                    lead += Mathf.Sin(t * melody[m] * 0.5f * Mathf.PI * 2f) * 0.20f;
                    air = noise * 0.026f;
                }
                else if (texture == "boss")
                {
                    tick = noise * Mathf.Exp(-barT * 24f) * 0.66f
                        + Mathf.Sin(t * 73.4f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 9f) * 0.48f;
                    lead += Triangle(t * melody[m] * 0.5f) * 0.34f;
                    low *= 1.18f;
                }
                else if (texture == "skirmish")
                {
                    float offBeat = ((t + beat * 0.5f) % beat) / beat;
                    tick = noise * Mathf.Exp(-barT * 28f) * 0.42f
                        + Mathf.Sin(t * 118f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 13f) * 0.34f
                        + Triangle(t * 760f) * Mathf.Exp(-offBeat * 17f) * (m % 2 == 1 ? 0.12f : 0.04f);
                    lead += Triangle(t * melody[m] * 1.5f) * leadEnv * 0.14f;
                    low *= 0.92f;
                }
                else if (texture == "shadowcombat")
                {
                    float offBeat = ((t + beat * 0.5f) % beat) / beat;
                    tick = noise * Mathf.Exp(-offBeat * 18f) * (m % 2 == 1 ? 0.20f : 0.06f);
                    air = noise * 0.042f
                        + Mathf.Sin(t * melody[m] * 0.25f * Mathf.PI * 2f) * 0.075f;
                    lead *= 0.76f;
                    low *= 0.88f;
                }
                else if (texture == "riftcombat")
                {
                    tick = noise * Mathf.Exp(-barT * 20f) * 0.42f
                        + Mathf.Sin(t * 55f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 7f) * 0.46f;
                    lead = Triangle(t * melody[m] * 0.5f) * leadEnv * 0.48f
                        + Mathf.Sin(t * melody[m] * Mathf.PI * 2f) * leadEnv * 0.18f;
                    low = Mathf.Sin(t * bass[b] * 0.5f * Mathf.PI * 2f) * lowEnv * 0.78f;
                    air = noise * 0.036f + Mathf.Sin(t * 31f * Mathf.PI * 2f) * 0.035f;
                }
                else if (texture == "bonecombat")
                {
                    float rattleBeat = ((t + beat * 0.25f) % (beat * 0.5f)) / (beat * 0.5f);
                    tick = noise * Mathf.Exp(-rattleBeat * 23f) * (m % 4 == 3 ? 0.28f : 0.12f)
                        + Mathf.Sin(t * 112f * Mathf.PI * 2f) * Mathf.Exp(-barT * 10f) * 0.24f;
                    lead *= 0.66f;
                    low *= 1.08f;
                    air = Mathf.Sin(t * 46f * Mathf.PI * 2f) * 0.045f;
                }
                else if (texture == "drip")
                {
                    tick = (m % 3 == 0 ? Mathf.Sin(t * 1320f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 16f) * 0.26f : 0f);
                    air = noise * 0.035f;
                }
                else if (texture == "glass")
                {
                    lead += Mathf.Sin(t * melody[m] * 2.01f * Mathf.PI * 2f) * leadEnv * 0.22f;
                    tick = Mathf.Sin(t * 1460f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 12f) * (m % 4 == 0 ? 0.16f : 0f);
                }
                else if (texture == "haze" || texture == "stealth")
                {
                    air = noise * (texture == "stealth" ? 0.052f : 0.040f);
                    lead *= texture == "stealth" ? 0.78f : 0.62f;
                }
                else if (texture == "stone" || texture == "omen")
                {
                    tick = Mathf.Sin(t * (texture == "omen" ? 72f : 96f) * Mathf.PI * 2f) * Mathf.Exp(-barT * 8f) * 0.28f;
                }
                else if (texture == "chime" || texture == "bells")
                {
                    tick = Mathf.Sin(t * (texture == "bells" ? 880f : 1160f) * Mathf.PI * 2f) * Mathf.Exp(-beatT * 9f) * (m % 4 == 0 ? 0.18f : 0.06f);
                }
                else if (texture == "royal")
                {
                    lead *= 0.72f;
                    tick = Mathf.Sin(t * 784f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 7f) * (m % 4 == 0 ? 0.20f : 0.04f);
                    air = Mathf.Sin(t * 49f * Mathf.PI * 2f) * 0.045f;
                }
                else if (texture == "market")
                {
                    lead *= 0.78f;
                    tick = Triangle(t * 988f) * Mathf.Exp(-beatT * 11f) * (m % 2 == 0 ? 0.08f : 0.025f);
                    air = noise * 0.018f;
                }
                else if (texture == "muster")
                {
                    lead *= 0.72f;
                    counter = Triangle(t * melody[(m + melody.Length - 3) % melody.Length] * 0.5f)
                        * Mathf.Sin(barT * Mathf.PI) * 0.18f;
                    tick = noise * Mathf.Exp(-beatT * 15f) * (m % 4 == 0 ? 0.07f : 0.018f);
                    air = Mathf.Sin(t * bass[b] * 0.5f * Mathf.PI * 2f) * 0.038f;
                }
                else if (texture == "victory")
                {
                    float rise = Mathf.SmoothStep(0.25f, 1f, barT);
                    lead += Triangle(t * melody[m] * 0.5f) * leadEnv * 0.30f;
                    counter = Mathf.Sin(t * melody[(m + 4) % melody.Length] * 0.5f * Mathf.PI * 2f) * rise * 0.18f;
                    tick = noise * Mathf.Exp(-barT * 26f) * 0.42f
                        + Mathf.Sin(t * 1046.5f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 10f) * (m % 4 == 0 ? 0.18f : 0.03f);
                    low *= 1.08f;
                }
                else if (texture == "lament")
                {
                    lead *= 0.48f;
                    low *= 0.78f;
                    counter = Mathf.Sin(t * melody[(m + 5) % melody.Length] * 0.25f * Mathf.PI * 2f) * 0.10f;
                    air = noise * 0.022f + Mathf.Sin(t * 36.7f * Mathf.PI * 2f) * 0.030f;
                }
                else if (texture == "sanctuary")
                {
                    lead *= 0.62f;
                    counter = Mathf.Sin(t * melody[(m + 5) % melody.Length] * 2f * Mathf.PI * 2f)
                        * Mathf.Exp(-beatT * 7f) * (m % 4 == 0 ? 0.14f : 0.035f);
                    tick = Mathf.Sin(t * 1320f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 11f) * (m % 8 == 0 ? 0.13f : 0f);
                    air = Mathf.Sin(t * bass[b] * 0.5f * Mathf.PI * 2f) * 0.045f + noise * 0.010f;
                }
                else if (texture == "marketdance" || texture == "street")
                {
                    float offBeat = ((t + beat * 0.5f) % beat) / beat;
                    lead *= texture == "street" ? 0.88f : 0.76f;
                    counter = Triangle(t * melody[(m + 5) % melody.Length] * 0.5f)
                        * Mathf.Exp(-offBeat * 5f) * 0.16f;
                    tick = Triangle(t * (texture == "street" ? 720f : 940f))
                        * Mathf.Exp(-offBeat * 15f) * (m % 2 == 1 ? 0.10f : 0.035f);
                    air = noise * 0.014f;
                }
                else if (texture == "watch" || texture == "processional" || texture == "roadwatch")
                {
                    float marchBeat = ((t + beat * 0.25f) % (beat * 0.5f)) / (beat * 0.5f);
                    lead += Triangle(t * melody[m] * 0.5f) * leadEnv * (texture == "processional" ? 0.28f : 0.18f);
                    counter = Mathf.Sin(t * melody[(m + 4) % melody.Length] * 0.25f * Mathf.PI * 2f) * 0.10f;
                    tick = noise * Mathf.Exp(-marchBeat * 20f) * (m % 4 == 0 ? 0.17f : 0.052f);
                    low *= texture == "watch" ? 1.02f : 0.92f;
                }
                else if (texture == "threshold")
                {
                    lead *= 0.42f;
                    low *= 1.04f;
                    counter = Triangle(t * melody[(m + 7) % melody.Length] * 0.25f) * 0.10f;
                    tick = Mathf.Sin(t * 58f * Mathf.PI * 2f) * Mathf.Exp(-barT * 7f) * 0.18f;
                    air = noise * 0.032f + Mathf.Sin(t * 29f * Mathf.PI * 2f) * 0.042f;
                }
                else if (texture == "camp")
                {
                    lead *= 0.68f;
                    counter = Triangle(t * melody[(m + 4) % melody.Length] * 0.5f) * Mathf.Exp(-barT * 2.8f) * 0.12f;
                    tick = noise * Mathf.Exp(-beatT * 19f) * (m % 8 == 0 ? 0.055f : 0.012f);
                    air = (PseudoNoise(i / 11 + 431) * 2f - 1f) * 0.020f;
                }
                else if (texture == "ruins")
                {
                    lead *= 0.44f;
                    counter = Mathf.Sin(t * melody[(m + 6) % melody.Length] * 0.25f * Mathf.PI * 2f) * 0.12f;
                    tick = Mathf.Sin(t * 760f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 13f) * (m % 8 == 0 ? 0.08f : 0f);
                    air = noise * 0.027f;
                }
                else if (texture == "arcane" || texture == "duel")
                {
                    float arpeggio = ((t + beat * 0.33f) % beat) / beat;
                    lead *= texture == "duel" ? 0.92f : 0.64f;
                    counter = Mathf.Sin(t * melody[(m + 5) % melody.Length] * 1.5f * Mathf.PI * 2f)
                        * Mathf.Exp(-arpeggio * 5.5f) * (texture == "duel" ? 0.19f : 0.12f);
                    tick = Mathf.Sin(t * 1510f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 12f)
                        * (m % (texture == "duel" ? 2 : 4) == 0 ? 0.15f : 0.025f);
                    air = Mathf.Sin(t * 43.7f * Mathf.PI * 2f) * 0.030f;
                }
                else if (texture == "pursuit" || texture == "warcamp")
                {
                    float doubleBeat = (t % (beat * 0.5f)) / (beat * 0.5f);
                    lead *= texture == "pursuit" ? 0.72f : 0.66f;
                    counter = Triangle(t * melody[(m + 3) % melody.Length] * 0.5f) * 0.14f;
                    tick = noise * Mathf.Exp(-doubleBeat * 21f) * (texture == "pursuit" ? 0.25f : 0.32f)
                        + Mathf.Sin(t * 82.4f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 9f) * 0.20f;
                    low *= 1.05f;
                }
                else if (texture == "grove")
                {
                    lead *= 0.54f;
                    counter = Mathf.Sin(t * melody[(m + 4) % melody.Length] * 0.5f * Mathf.PI * 2f)
                        * (0.08f + Mathf.Sin(t * Mathf.PI * 2f / 7f) * 0.025f);
                    tick = Mathf.Sin(t * 1180f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 12f) * (m % 8 == 0 ? 0.09f : 0f);
                    air = noise * 0.018f + Mathf.Sin(t * 31f * Mathf.PI * 2f) * 0.030f;
                }
                else if (texture == "plaguecombat")
                {
                    float rattleBeat = (t % (beat * 0.5f)) / (beat * 0.5f);
                    lead = Triangle(t * melody[m]) * leadEnv * 0.62f
                        + Triangle(t * melody[m] * 0.992f) * leadEnv * 0.20f;
                    counter = Mathf.Sin(t * melody[(m + 5) % melody.Length] * 0.5f * Mathf.PI * 2f) * 0.15f;
                    tick = noise * Mathf.Exp(-rattleBeat * 22f) * 0.28f
                        + Mathf.Sin(t * 103f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 8f) * 0.22f;
                    air = noise * 0.032f;
                }
                else if (texture == "elitecombat")
                {
                    float doubleBeat = (t % (beat * 0.5f)) / (beat * 0.5f);
                    lead += Triangle(t * melody[m] * 0.5f) * leadEnv * 0.28f;
                    counter = Mathf.Sin(t * melody[(m + 4) % melody.Length] * 0.5f * Mathf.PI * 2f) * 0.18f;
                    tick = noise * Mathf.Exp(-doubleBeat * 24f) * 0.38f
                        + Mathf.Sin(t * 92.5f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 9f) * 0.28f;
                    low *= 1.12f;
                }
                else if (texture == "laststand")
                {
                    float heartBeat = (t % (beat * 2f)) / (beat * 2f);
                    float rise = 0.35f + Mathf.SmoothStep(0f, 1f, (m % 8) / 7f) * 0.65f;
                    lead *= rise;
                    counter = Triangle(t * melody[(m + 5) % melody.Length] * 0.5f) * rise * 0.22f;
                    tick = Mathf.Sin(t * 58f * Mathf.PI * 2f) * Mathf.Exp(-heartBeat * 18f) * 0.48f
                        + noise * Mathf.Exp(-beatT * 18f) * 0.16f;
                    low *= 1.08f;
                }
                else if (texture == "kingcombat")
                {
                    float doubleBeat = (t % (beat * 0.5f)) / (beat * 0.5f);
                    lead += Triangle(t * melody[m] * 0.5f) * leadEnv * 0.34f;
                    counter = Triangle(t * melody[(m + 5) % melody.Length] * 1.5f) * leadEnv * 0.16f;
                    tick = noise * Mathf.Exp(-doubleBeat * 26f) * 0.44f
                        + Mathf.Sin(t * 110f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 10f) * 0.34f;
                    low *= 1.20f;
                }
                else if (texture == "titancombat")
                {
                    float impactBeat = (t % (beat * 2f)) / (beat * 2f);
                    lead = Triangle(t * melody[m] * 0.5f) * leadEnv * 0.58f
                        + Mathf.Sin(t * melody[m] * Mathf.PI * 2f) * leadEnv * 0.14f;
                    low = Mathf.Sin(t * bass[b] * 0.5f * Mathf.PI * 2f) * lowEnv * 0.88f;
                    counter = Triangle(t * melody[(m + 6) % melody.Length] * 0.25f) * 0.17f;
                    tick = noise * Mathf.Exp(-impactBeat * 25f) * 0.52f
                        + Mathf.Sin(t * 41.2f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 6f) * 0.46f;
                    air = noise * 0.040f + Mathf.Sin(t * 24.5f * Mathf.PI * 2f) * 0.045f;
                }
                else if (texture == "echo")
                {
                    air = Mathf.Sin(t * melody[m] * 0.25f * Mathf.PI * 2f) * 0.07f;
                }

                float sample = (lead * 0.30f + low * 0.25f + tick * 0.18f + counter * 0.18f + air)
                    * Mathf.Lerp(0.30f, 0.72f, energy)
                    * phrase;
                data[i] = Mathf.Clamp(sample, -0.82f, 0.82f);
            }

            ApplyLoopEdgeFade(data, sampleRate);
            AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static void ApplyLoopEdgeFade(float[] data, int sampleRate)
        {
            if (data == null || data.Length < 4 || sampleRate <= 0) return;
            int edgeSamples = Mathf.Clamp(Mathf.RoundToInt(sampleRate * 0.045f), 32, data.Length / 4);
            for (int i = 0; i < edgeSamples; i++)
            {
                float shaped = Mathf.SmoothStep(0f, 1f, (i + 1f) / edgeSamples);
                data[i] *= shaped;
                data[data.Length - 1 - i] *= shaped;
            }
        }

        private float Triangle(float cycles)
        {
            float v = cycles - Mathf.Floor(cycles);
            return 4f * Mathf.Abs(v - 0.5f) - 1f;
        }

        private void EnsureAudioListener()
        {
            try
            {
                if (FindAnyObjectByType<AudioListener>() == null)
                {
                    gameObject.AddComponent<AudioListener>();
                }
            }
            catch (Exception)
            {
                // Some smoke-test environments do not expose audio devices cleanly.
            }
        }

        private void EnsureSfxVoicePool()
        {
            sfxVoices.Clear();
            if (audioSource != null) sfxVoices.Add(audioSource);
            for (int i = sfxVoices.Count; i < CombatAudioMixRules.SfxVoiceCount; i++)
            {
                AudioSource voice = gameObject.AddComponent<AudioSource>();
                voice.playOnAwake = false;
                voice.spatialBlend = 0f;
                voice.ignoreListenerPause = true;
                voice.priority = 32 + i;
                sfxVoices.Add(voice);
            }
            nextSfxVoice = 0;
        }

        private void PlaySfx(string key, float volume = 1f)
        {
            if (state != null && state.Mode == GameMode.Combat)
            {
                CombatAudioCueProfile combatCue = CombatAudioMixRules.DirectCue(key, volume);
                if (!string.Equals(combatCue.Key, key, StringComparison.OrdinalIgnoreCase)
                    && soundClips.ContainsKey(combatCue.Key))
                {
                    CombatUnit active = state.Combat == null ? null : CurrentUnit();
                    int column = active?.X ?? CombatW / 2;
                    float pan = CombatAudioMixRules.StereoPanForColumn(column, CombatW);
                    float pitch = CombatAudioMixRules.PitchForCue(combatCue.Key, column);
                    PlaySfxSpatial(combatCue.Key, combatCue.Volume, pan * 0.58f, pitch);
                    return;
                }
            }
            PlaySfxSpatial(key, volume, 0f, 1f);
        }

        private void PlaySfxSpatial(string key, float volume, float pan, float pitch)
        {
            if (audioSource == null || string.IsNullOrEmpty(key) || !soundClips.ContainsKey(key)) return;
            if (state != null && state.SfxMuted) return;
            if (key == "ui"
                && lastSfxKey == "dialogueopen"
                && Time.realtimeSinceStartup - lastSfxAt < 0.10f)
            {
                return;
            }
            try
            {
                ApplyAudioSettings();
                float clamped = Mathf.Clamp(volume, 0f, 1.4f);
                AudioSource voice = AcquireSfxVoice();
                if (voice == null) return;
                voice.panStereo = Mathf.Clamp(pan, -0.85f, 0.85f);
                voice.pitch = Mathf.Clamp(pitch * SfxPlaybackPitchVariation(key, sfxPlaybackSerial++), 0.90f, 1.10f);
                lastSfxKey = key;
                lastSfxAt = Time.realtimeSinceStartup;
                lastSfxVolume = clamped;
                if (state != null
                    && state.Mode == GameMode.Explore
                    && GameAudioCueRules.SuppressesExplorationAmbience(key))
                {
                    lastExplorationForegroundSfxAt = Time.unscaledTime;
                }
                if (state != null
                    && state.Mode == GameMode.Combat
                    && !CombatAudioMixRules.IsCombatAmbienceCue(key))
                {
                    lastCombatForegroundSfxAt = Time.time;
                }
                voice.PlayOneShot(soundClips[key], clamped);
            }
            catch (Exception)
            {
                // Audio devices may be unavailable during headless smoke tests.
            }
        }

        private float SfxPlaybackPitchVariation(string key, int serial)
        {
            if (TitleAudioRules.LocksPitch(key)) return 1f;
            unchecked
            {
                uint seed = StableAudioSeed((key ?? "") + "|playback");
                uint mixed = seed ^ ((uint)Mathf.Max(0, serial) * 0x9E3779B9u);
                int step = (int)(mixed & 3u);
                return 0.988f + step * 0.008f;
            }
        }

        private AudioSource AcquireSfxVoice()
        {
            if (sfxVoices.Count == 0) return audioSource;
            for (int offset = 0; offset < sfxVoices.Count; offset++)
            {
                int index = (nextSfxVoice + offset) % sfxVoices.Count;
                AudioSource voice = sfxVoices[index];
                if (voice == null || voice.isPlaying) continue;
                nextSfxVoice = (index + 1) % sfxVoices.Count;
                return voice;
            }

            AudioSource fallback = sfxVoices[nextSfxVoice % sfxVoices.Count];
            nextSfxVoice = (nextSfxVoice + 1) % sfxVoices.Count;
            return fallback;
        }

        private void ApplyAudioSettings()
        {
            int percent = state == null ? 100 : Mathf.Clamp(state.SfxVolumePercent <= 0 ? 100 : state.SfxVolumePercent, 25, 100);
            bool muted = state != null && state.SfxMuted;
            float sfxVolume = muted ? 0f : 0.78f * (percent / 100f);
            if (sfxVoices.Count == 0)
            {
                if (audioSource != null) audioSource.volume = sfxVolume;
            }
            else
            {
                foreach (AudioSource voice in sfxVoices)
                {
                    if (voice != null) voice.volume = sfxVolume;
                }
            }
            int musicPercent = state == null ? 65 : Mathf.Clamp(state.MusicVolumePercent <= 0 ? 65 : state.MusicVolumePercent, 25, 100);
            bool musicMuted = state != null && state.MusicMuted;
            GameMode musicMode = state == null ? GameMode.Tavern : state.Mode;
            bool worldMapMix = musicMode == GameMode.Explore
                && exploreWideView
                && !string.Equals(
                    explorationMusicSelectedKey,
                    MusicDirectorRules.HuntedRoad,
                    StringComparison.Ordinal);
            float musicVolume = musicMuted
                ? 0f
                : TitleAudioRules.MusicSourceGain(musicMode, worldMapMix)
                    * (musicPercent / 100f)
                    * CurrentCombatMusicDuckMultiplier();
            if (musicTransitionActive && musicFadeSource != null)
            {
                float duration = Mathf.Max(0.01f, activeMusicTransitionDuration);
                float progress = Mathf.Clamp01((Time.unscaledTime - musicTransitionStartedAt) / duration);
                MusicCrossfadeGains gains = MusicTransitionRules.EqualPowerCrossfade(progress);
                if (musicSource != null) musicSource.volume = musicVolume * gains.Outgoing;
                musicFadeSource.volume = musicVolume * gains.Incoming;
            }
            else
            {
                float introGain = 1f;
                if (musicIntroFadeActive)
                {
                    float duration = Mathf.Max(0.01f, activeMusicIntroFadeDuration);
                    float progress = Mathf.Clamp01((Time.unscaledTime - musicIntroFadeStartedAt) / duration);
                    introGain = Mathf.SmoothStep(0f, 1f, progress);
                    if (progress >= 1f) musicIntroFadeActive = false;
                }
                if (musicSource != null) musicSource.volume = musicVolume * introGain;
                if (musicFadeSource != null) musicFadeSource.volume = 0f;
            }
        }

        private float CurrentCombatMusicDuckMultiplier()
        {
            if (state == null || state.Mode != GameMode.Combat || combatMusicDuckDepth <= 0f || Time.time >= combatMusicDuckUntil) return 1f;
            return CombatAudioMixRules.MusicDuckEnvelopeMultiplier(
                Time.time,
                combatMusicDuckStartedAt,
                combatMusicDuckFullDepthAt,
                combatMusicDuckHoldUntil,
                combatMusicDuckUntil,
                combatMusicDuckDepth);
        }

        private void UpdateExplorationAmbience()
        {
            float now = Time.unscaledTime;
            if (state == null || state.Mode != GameMode.Explore || IsStartupSplashVisible())
            {
                lastExplorationAmbienceContext = "";
                nextExplorationAmbienceAt = -1f;
                return;
            }

            UiOverlay overlay = CurrentUiOverlay();
            if (state.SfxMuted || (overlay != UiOverlay.None && overlay != UiOverlay.Dialogue))
            {
                nextExplorationAmbienceAt = Mathf.Max(nextExplorationAmbienceAt, now + 1.5f);
                return;
            }

            string cue = CurrentExplorationAmbientCue(out string ambienceContext);
            if (string.IsNullOrEmpty(cue) || !soundClips.ContainsKey(cue)) return;
            if (!string.Equals(lastExplorationAmbienceContext, ambienceContext, StringComparison.Ordinal))
            {
                lastExplorationAmbienceContext = ambienceContext;
                nextExplorationAmbienceAt = now + 2.8f;
                return;
            }

            if (nextExplorationAmbienceAt < 0f) nextExplorationAmbienceAt = now + 2.8f;
            if (now < nextExplorationAmbienceAt) return;
            if (now - lastExplorationForegroundSfxAt < 1.25f)
            {
                nextExplorationAmbienceAt = now + 1.5f;
                return;
            }

            int seed = state.Seed ^ state.PlayerX * 397 ^ state.PlayerY * 911;
            float pan = GameAudioCueRules.AmbientPan(seed, explorationAmbienceSequence);
            float pitch = GameAudioCueRules.AmbientPitch(seed, explorationAmbienceSequence);
            PlaySfxSpatial(cue, GameAudioCueRules.AmbientVolume(cue), pan, pitch);
            nextExplorationAmbienceAt = now + GameAudioCueRules.AmbientInterval(seed, explorationAmbienceSequence);
            explorationAmbienceSequence++;
        }

        private void UpdateTavernAmbience()
        {
            float now = Time.unscaledTime;
            bool tavernMode = state != null && (state.Mode == GameMode.Tavern || state.Mode == GameMode.Muster);
            if (!tavernMode || IsStartupSplashVisible())
            {
                nextTavernAmbienceAt = -1f;
                tavernAmbienceSequence = 0;
                return;
            }

            if (state.SfxMuted)
            {
                nextTavernAmbienceAt = Mathf.Max(nextTavernAmbienceAt, now + 1.5f);
                return;
            }

            bool musicAudible = !state.MusicMuted;
            if (nextTavernAmbienceAt < 0f)
            {
                nextTavernAmbienceAt = now + TitleAudioRules.InitialAmbienceDelay(state.Mode, musicAudible);
            }
            if (now < nextTavernAmbienceAt || now - lastSfxAt < 0.75f) return;

            TitleAmbienceProfile ambience = TitleAudioRules.Ambience(state.Mode, musicAudible, tavernAmbienceSequence);
            PlaySfxSpatial(ambience.Key, ambience.Volume, ambience.Pan, ambience.Pitch);
            nextTavernAmbienceAt = now + TitleAudioRules.AmbienceInterval(state.Mode, musicAudible, tavernAmbienceSequence);
            tavernAmbienceSequence++;
        }

        private void UpdateCombatAmbience()
        {
            float now = Time.time;
            if (state == null
                || state.Mode != GameMode.Combat
                || state.Combat == null
                || IsStartupSplashVisible())
            {
                nextCombatAmbienceAt = -1f;
                combatAmbienceSequence = 0;
                combatAmbienceEncounter = null;
                return;
            }

            if (!ReferenceEquals(combatAmbienceEncounter, state.Combat))
            {
                combatAmbienceEncounter = state.Combat;
                combatAmbienceSequence = 0;
                nextCombatAmbienceAt = -1f;
            }

            if (state.SfxMuted || CurrentUiOverlay() == UiOverlay.Pause)
            {
                nextCombatAmbienceAt = Mathf.Max(nextCombatAmbienceAt, now + 1.5f);
                return;
            }

            bool musicAudible = !state.MusicMuted;
            if (nextCombatAmbienceAt < 0f)
            {
                nextCombatAmbienceAt = now + CombatAudioMixRules.InitialAmbienceDelay(musicAudible);
            }
            if (scheduledSfx.Count > 0
                || combatMusicDuckDepth > 0f && now < combatMusicDuckUntil
                || now - lastCombatForegroundSfxAt < CombatAudioMixRules.CombatAmbienceForegroundQuietWindow)
            {
                nextCombatAmbienceAt = Mathf.Max(
                    nextCombatAmbienceAt,
                    now + CombatAudioMixRules.CombatAmbienceForegroundQuietWindow);
                return;
            }
            if (now < nextCombatAmbienceAt) return;

            string route = !string.IsNullOrEmpty(combatMusicBaseKey)
                ? combatMusicBaseKey
                : !string.IsNullOrEmpty(combatMusicSelectedKey)
                    ? combatMusicSelectedKey
                    : MusicDirectorRules.CombatGeneric;
            CombatAmbienceProfile ambience = CombatAudioMixRules.Ambience(route, musicAudible, combatAmbienceSequence);
            if (!soundClips.ContainsKey(ambience.Key))
            {
                nextCombatAmbienceAt = now + 2f;
                return;
            }
            PlaySfxSpatial(ambience.Key, ambience.Volume, ambience.Pan, ambience.Pitch);
            nextCombatAmbienceAt = now + CombatAudioMixRules.AmbienceInterval(musicAudible, combatAmbienceSequence);
            combatAmbienceSequence++;
        }

        private string CurrentExplorationAmbientCue()
        {
            return CurrentExplorationAmbientCue(out _);
        }

        private string CurrentExplorationAmbientCue(out string ambienceContext)
        {
            string zoneId = "road";
            try
            {
                zoneId = ZoneAt(state.PlayerX, state.PlayerY)?.Id ?? "road";
            }
            catch (Exception)
            {
                zoneId = "road";
            }

            if (TryRegionalSiteAt(
                    state?.Map,
                    state == null ? 0 : state.PlayerX,
                    state == null ? 0 : state.PlayerY,
                    out WorldMapSite currentSite)
                && WorldSitePresentationRules.TryGet(
                    currentSite.Id,
                    out WorldSitePresentationProfile currentSiteProfile))
            {
                ambienceContext = WorldSitePresentationRules.LandmarkObjectIdPrefix + currentSite.Id;
                return currentSiteProfile.AmbientCueFor(explorationAmbienceSequence);
            }

            ObjectType? nearest = null;
            WorldSitePresentationProfile nearestSite = default;
            bool hasNearestSite = false;
            int nearestDistance = int.MaxValue;
            int nearestPriority = -1;
            if (state?.Map?.Objects != null)
            {
                foreach (MapObject obj in state.Map.Objects)
                {
                    if (obj == null || WorldSitePresentationRules.IsDecorationObjectId(obj.Id)) continue;
                    bool isAuthoredSite = WorldSitePresentationRules.TryGetForLandmarkObjectId(
                        obj.Id,
                        out WorldSitePresentationProfile site)
                        && obj.Type == site.LandmarkType;
                    if (!isAuthoredSite && !GameAudioCueRules.IsAmbientLandmark(obj.Type)) continue;
                    int distance = Mathf.Abs(obj.X - state.PlayerX) + Mathf.Abs(obj.Y - state.PlayerY);
                    int priority = isAuthoredSite ? 1 : 0;
                    if (distance > 6
                        || distance > nearestDistance
                        || distance == nearestDistance && priority <= nearestPriority)
                    {
                        continue;
                    }
                    nearestDistance = distance;
                    nearestPriority = priority;
                    nearest = obj.Type;
                    nearestSite = site;
                    hasNearestSite = isAuthoredSite;
                }
            }

            if (hasNearestSite)
            {
                ambienceContext = WorldSitePresentationRules.LandmarkObjectIdPrefix + nearestSite.SiteId;
                return nearestSite.AmbientCueFor(explorationAmbienceSequence);
            }

            string cue = GameAudioCueRules.AmbientFor(zoneId, nearest);
            ambienceContext = cue;
            return cue;
        }

        private void UpdateTavernMusic()
        {
            if (state == null || state.Mode != GameMode.Combat) ResetCombatMusicPresentationState();
            if (state == null || state.Mode != GameMode.Explore) ResetExplorationMusicPresentationState();
            if (musicSource == null) return;
            AudioClip desired = DesiredMusicClip();
            bool shouldRunTransport = MusicTransitionRules.ShouldKeepTransportAlive(
                state != null,
                desired != null,
                IsStartupSplashVisible(),
                state != null && state.MusicMuted);
            if (!shouldRunTransport)
            {
                musicSource.Stop();
                if (musicFadeSource != null) musicFadeSource.Stop();
                musicTransitionActive = false;
                musicTransitionStartedAt = -1f;
                musicIntroFadeActive = false;
                musicIntroFadeStartedAt = -1f;
                musicIntroFadeClip = null;
                ApplyAudioSettings();
                return;
            }

            if (musicTransitionActive)
            {
                if (musicFadeSource == null || musicFadeSource.clip != desired)
                {
                    SettleInterruptedMusicTransition();
                }
                else if (Time.unscaledTime - musicTransitionStartedAt >= activeMusicTransitionDuration)
                {
                    CompleteMusicTransition();
                }
            }

            if (!musicTransitionActive && musicSource.clip != desired)
            {
                if (musicSource.isPlaying && musicFadeSource != null)
                {
                    musicFadeSource.Stop();
                    musicFadeSource.clip = desired;
                    musicFadeSource.time = 0f;
                    musicFadeSource.volume = 0f;
                    musicFadeSource.Play();
                    musicTransitionStartedAt = Time.unscaledTime;
                    activeMusicTransitionDuration = MusicTransitionRules.TransitionDurationFor(CurrentMusicTransitionContext());
                    musicTransitionActive = true;
                    musicIntroFadeActive = false;
                    musicIntroFadeClip = desired;
                }
                else
                {
                    musicSource.clip = desired;
                    musicSource.time = 0f;
                    musicIntroFadeClip = null;
                }
            }

            if (!musicTransitionActive && !musicSource.isPlaying)
            {
                musicSource.Play();
                if (musicIntroFadeClip != musicSource.clip)
                {
                    MusicTransitionTiming timing = MusicTransitionRules.TimingFor(CurrentMusicTransitionContext());
                    musicIntroFadeClip = musicSource.clip;
                    musicIntroFadeStartedAt = Time.unscaledTime;
                    activeMusicIntroFadeDuration = timing.IntroFadeDuration;
                    musicIntroFadeActive = activeMusicIntroFadeDuration > 0f;
                }
            }
            ApplyAudioSettings();
        }

        private MusicTransitionContext CurrentMusicTransitionContext()
        {
            if (state == null || state.Mode == GameMode.Tavern || state.Mode == GameMode.Muster)
            {
                return MusicTransitionContext.Title;
            }
            if (state.Mode == GameMode.Combat) return MusicTransitionContext.Combat;
            if (state.Mode == GameMode.Victory) return MusicTransitionContext.Victory;
            if (state.Mode == GameMode.Defeat) return MusicTransitionContext.Defeat;
            if (state.Mode == GameMode.Explore
                && exploreWideView
                && !string.Equals(
                    explorationMusicSelectedKey,
                    MusicDirectorRules.HuntedRoad,
                    StringComparison.Ordinal))
            {
                return MusicTransitionContext.WorldMapExplore;
            }
            return MusicTransitionContext.Explore;
        }

        private void SettleInterruptedMusicTransition()
        {
            if (!musicTransitionActive || musicFadeSource == null) return;
            float duration = Mathf.Max(0.01f, activeMusicTransitionDuration);
            float progress = Mathf.Clamp01((Time.unscaledTime - musicTransitionStartedAt) / duration);
            MusicCrossfadeGains gains = MusicTransitionRules.EqualPowerCrossfade(progress);
            if (gains.Incoming >= gains.Outgoing)
            {
                CompleteMusicTransition();
                return;
            }

            musicFadeSource.Stop();
            musicFadeSource.clip = null;
            musicFadeSource.volume = 0f;
            musicTransitionActive = false;
            musicTransitionStartedAt = -1f;
        }

        private void CompleteMusicTransition()
        {
            if (!musicTransitionActive || musicFadeSource == null) return;
            musicSource.Stop();
            AudioSource oldSource = musicSource;
            musicSource = musicFadeSource;
            musicFadeSource = oldSource;
            musicFadeSource.Stop();
            musicFadeSource.clip = null;
            musicFadeSource.volume = 0f;
            musicTransitionActive = false;
            musicTransitionStartedAt = -1f;
            musicIntroFadeClip = musicSource.clip;
        }

        private AudioClip DesiredMusicClip()
        {
            if (state == null)
            {
                ResetCombatMusicPresentationState();
                ResetExplorationMusicPresentationState();
                return null;
            }
            if (state.Mode != GameMode.Combat) ResetCombatMusicPresentationState();
            if (state.Mode != GameMode.Explore) ResetExplorationMusicPresentationState();
            if (state.Mode == GameMode.Tavern) return MusicClipForKey(MusicDirectorRules.Tavern);
            if (state.Mode == GameMode.Muster)
            {
                return MusicClipForKey(MusicDirectorRules.Muster)
                    ?? MusicClipForKey(MusicDirectorRules.Tavern);
            }
            if (state.Mode == GameMode.Victory)
            {
                return MusicClipForKey(MusicDirectorRules.Victory)
                    ?? MusicClipForKey(MusicDirectorRules.Tavern);
            }
            if (state.Mode == GameMode.Defeat)
            {
                return MusicClipForKey(MusicDirectorRules.Defeat)
                    ?? MusicClipForKey(MusicDirectorRules.Tavern);
            }
            if (state.Mode == GameMode.Combat)
            {
                string key = CurrentCombatMusicKey();
                return MusicClipForKey(key)
                    ?? MusicClipForKey(MusicDirectorRules.CombatGeneric)
                    ?? MusicClipForKey(MusicDirectorRules.Tavern);
            }
            if (state.Mode == GameMode.Explore)
            {
                string zoneId = "road";
                try
                {
                    zoneId = ZoneAt(state.PlayerX, state.PlayerY)?.Id ?? "road";
                }
                catch (Exception)
                {
                    zoneId = "road";
                }

                bool threatAlerted = IsAlertedRoamingThreatNear();
                bool hasLandmark;
                ObjectType landmark;
                string siteId;
                if (TryRegionalSiteAt(state.Map, state.PlayerX, state.PlayerY, out WorldMapSite currentSite)
                    && WorldSitePresentationRules.TryGet(currentSite.Id, out _))
                {
                    hasLandmark = true;
                    landmark = currentSite.Type;
                    siteId = currentSite.Id;
                }
                else
                {
                    hasLandmark = TryNearestMusicLandmark(out landmark, out siteId);
                }
                string candidateKey = string.IsNullOrEmpty(siteId)
                    ? MusicDirectorRules.ExploreTrackKey(zoneId, landmark, hasLandmark, threatAlerted)
                    : WorldSitePresentationRules.ExploreMusicKey(siteId, zoneId, threatAlerted);
                if (exploreWideView
                    && !string.Equals(candidateKey, MusicDirectorRules.HuntedRoad, StringComparison.Ordinal))
                {
                    candidateKey = MusicDirectorRules.WorldMapOverview;
                }
                string key = ResolveExplorationMusicPresentationKey(candidateKey, Time.unscaledTime);
                return MusicClipForKey(key)
                    ?? MusicClipForKey(zoneId)
                    ?? MusicClipForKey("road")
                    ?? MusicClipForKey(MusicDirectorRules.Tavern);
            }
            return null;
        }

        private string ResolveExplorationMusicPresentationKey(string candidateKey, float now)
        {
            string candidate = MusicTransitionRules.NormalizeRouteKey(candidateKey);
            if (string.IsNullOrEmpty(candidate)) return explorationMusicSelectedKey;

            if (string.Equals(candidate, explorationMusicSelectedKey, StringComparison.Ordinal))
            {
                explorationMusicCandidateKey = "";
                explorationMusicCandidateAt = -1f;
                return explorationMusicSelectedKey;
            }

            if (!string.Equals(candidate, explorationMusicCandidateKey, StringComparison.Ordinal))
            {
                explorationMusicCandidateKey = candidate;
                explorationMusicCandidateAt = now;
            }

            float candidateHeld = explorationMusicCandidateAt < 0f
                ? 0f
                : Mathf.Max(0f, now - explorationMusicCandidateAt);
            float selectedDwell = explorationMusicSelectedAt < 0f
                ? 0f
                : Mathf.Max(0f, now - explorationMusicSelectedAt);
            bool explicitWorldMapViewChange = string.Equals(
                    explorationMusicSelectedKey,
                    MusicDirectorRules.WorldMapOverview,
                    StringComparison.Ordinal)
                || string.Equals(candidate, MusicDirectorRules.WorldMapOverview, StringComparison.Ordinal);
            ExplorationMusicSwitchDecision decision = MusicTransitionRules.EvaluateExplorationSwitch(
                explorationMusicSelectedKey,
                candidate,
                candidateHeld,
                selectedDwell,
                candidateHeld,
                explicitWorldMapViewChange);
            if (decision.ShouldSwitch)
            {
                explorationMusicSelectedKey = candidate;
                explorationMusicSelectedAt = now;
                explorationMusicCandidateKey = "";
                explorationMusicCandidateAt = -1f;
            }

            return string.IsNullOrEmpty(explorationMusicSelectedKey)
                ? candidate
                : explorationMusicSelectedKey;
        }

        private void ResetExplorationMusicPresentationState()
        {
            if (string.IsNullOrEmpty(explorationMusicSelectedKey)
                && string.IsNullOrEmpty(explorationMusicCandidateKey))
            {
                return;
            }

            explorationMusicSelectedKey = "";
            explorationMusicCandidateKey = "";
            explorationMusicSelectedAt = -1f;
            explorationMusicCandidateAt = -1f;
        }

        private AudioClip MusicClipForKey(string key)
        {
            switch ((key ?? "").Trim().ToLowerInvariant())
            {
                case MusicDirectorRules.Tavern: return tavernMusicClip;
                case MusicDirectorRules.CombatGeneric: return combatMusicClip;
                case MusicDirectorRules.CombatSewer: return sewerCombatMusicClip;
                case MusicDirectorRules.CombatBoss: return bossCombatMusicClip;
                case MusicDirectorRules.CombatKobold: return koboldCombatMusicClip;
                case MusicDirectorRules.CombatDrow: return drowCombatMusicClip;
                case MusicDirectorRules.CombatDemon: return demonCombatMusicClip;
                case MusicDirectorRules.CombatUndead: return undeadCombatMusicClip;
            }

            if (zoneMusicClips.TryGetValue(key ?? "", out AudioClip zoneClip)) return zoneClip;
            return AdaptiveMusicClip(key);
        }

        private string CurrentCombatMusicKey()
        {
            if (state?.Combat?.Units == null)
            {
                ResetCombatMusicPresentationState();
                return MusicDirectorRules.CombatGeneric;
            }

            int rats = 0;
            int kobolds = 0;
            int drow = 0;
            int demons = 0;
            int undead = 0;
            int partyHp = 0;
            int partyMaxHp = 0;
            bool hasRatfolk = false;
            bool hasCaster = false;
            bool hasElite = false;
            bool hasGreaterDemon = false;
            foreach (CombatUnit unit in state.Combat.Units)
            {
                if (unit == null) continue;
                if (unit.Side == UnitSide.Party)
                {
                    partyHp += Mathf.Max(0, unit.Hp);
                    partyMaxHp += Mathf.Max(1, unit.MaxHp);
                    continue;
                }
                if (unit.Hp <= 0) continue;

                string identity = ((unit.Name ?? "") + " "
                    + (unit.Role ?? "") + " "
                    + (unit.Race ?? "") + " "
                    + (unit.ClassKey ?? "") + " "
                    + (unit.Rank ?? "")).ToLowerInvariant();
                string faction = CreatureAudioRules.FactionFor(unit);
                if (faction == "rat") rats++;
                else if (faction == "kobold") kobolds++;
                else if (faction == "drow") drow++;
                else if (faction == "demon") demons++;
                else if (faction == "undead") undead++;

                hasRatfolk |= identity.Contains("ratfolk")
                    || identity.Contains("rat mage")
                    || identity.Contains("rat cleric")
                    || identity.Contains("rat brute");
                hasCaster |= unit.Mana > 0
                    || !string.IsNullOrWhiteSpace(unit.Spell)
                    || identity.Contains("mage")
                    || identity.Contains("wizard")
                    || identity.Contains("priest")
                    || identity.Contains("cleric")
                    || identity.Contains("shaman")
                    || identity.Contains("warlock");
                hasElite |= identity.Contains("elite")
                    || identity.Contains("veteran")
                    || identity.Contains("champion")
                    || identity.Contains("captain");
                hasGreaterDemon |= identity.Contains("greater demon")
                    || identity.Contains("demon lord")
                    || identity.Contains("archdemon");
            }

            string factionKey = "";
            int largest = 0;
            SetDominantMusicFaction("demon", demons, ref factionKey, ref largest);
            SetDominantMusicFaction("drow", drow, ref factionKey, ref largest);
            SetDominantMusicFaction("kobold", kobolds, ref factionKey, ref largest);
            SetDominantMusicFaction("undead", undead, ref factionKey, ref largest);
            SetDominantMusicFaction("rat", rats, ref factionKey, ref largest);
            string candidateBaseKey = MusicDirectorRules.CombatTrackKey(
                state.Combat.EncounterStyle,
                factionKey,
                hasRatfolk,
                hasCaster,
                hasElite,
                hasGreaterDemon,
                false);
            return ResolveCombatMusicPresentationKey(
                state.Combat,
                candidateBaseKey,
                partyHp,
                partyMaxHp,
                Time.unscaledTime);
        }

        private string ResolveCombatMusicPresentationKey(
            CombatState encounter,
            string candidateBaseKey,
            int partyHp,
            int partyMaxHp,
            float now)
        {
            if (!ReferenceEquals(combatMusicEncounter, encounter))
            {
                ResetCombatMusicPresentationState();
                combatMusicEncounter = encounter;
                combatMusicBaseKey = CombatMusicPresentationRules.StableBaseTrack("", candidateBaseKey);
                combatMusicSelectedKey = combatMusicBaseKey;
                combatMusicSelectedAt = now;
            }

            string stableBase = CombatMusicPresentationRules.StableBaseTrack(combatMusicBaseKey, candidateBaseKey);
            if (!string.Equals(stableBase, combatMusicBaseKey, StringComparison.Ordinal))
            {
                combatMusicBaseKey = stableBase;
                combatMusicSelectedKey = stableBase;
                combatMusicSelectedAt = now;
                combatMusicLastStandActive = false;
                combatMusicCriticalStartedAt = -1f;
                combatMusicRecoveredStartedAt = -1f;
            }

            bool critical = CombatMusicPresentationRules.IsCriticalPartyHealth(partyHp, partyMaxHp);
            bool recovered = CombatMusicPresentationRules.IsRecoveredPartyHealth(partyHp, partyMaxHp);
            if (critical)
            {
                if (combatMusicCriticalStartedAt < 0f) combatMusicCriticalStartedAt = now;
                combatMusicRecoveredStartedAt = -1f;
            }
            else
            {
                combatMusicCriticalStartedAt = -1f;
                if (recovered)
                {
                    if (combatMusicRecoveredStartedAt < 0f) combatMusicRecoveredStartedAt = now;
                }
                else
                {
                    combatMusicRecoveredStartedAt = -1f;
                }
            }

            float selectedDwell = combatMusicSelectedAt < 0f ? 0f : Mathf.Max(0f, now - combatMusicSelectedAt);
            float criticalHeld = combatMusicCriticalStartedAt < 0f ? 0f : Mathf.Max(0f, now - combatMusicCriticalStartedAt);
            float recoveredHeld = combatMusicRecoveredStartedAt < 0f ? 0f : Mathf.Max(0f, now - combatMusicRecoveredStartedAt);

            if (CombatMusicPresentationRules.IsBossCombatTrack(combatMusicBaseKey))
            {
                combatMusicLastStandActive = false;
                combatMusicSelectedKey = combatMusicBaseKey;
            }
            else if (CombatMusicPresentationRules.ShouldEnterLastStand(
                combatMusicBaseKey,
                combatMusicLastStandActive,
                partyHp,
                partyMaxHp,
                criticalHeld,
                selectedDwell))
            {
                combatMusicLastStandActive = true;
                combatMusicSelectedKey = MusicDirectorRules.CombatLastStand;
                combatMusicSelectedAt = now;
                combatMusicRecoveredStartedAt = -1f;
            }
            else if (CombatMusicPresentationRules.ShouldExitLastStand(
                combatMusicLastStandActive,
                partyHp,
                partyMaxHp,
                recoveredHeld,
                selectedDwell))
            {
                combatMusicLastStandActive = false;
                combatMusicSelectedKey = combatMusicBaseKey;
                combatMusicSelectedAt = now;
                combatMusicCriticalStartedAt = -1f;
            }

            return string.IsNullOrEmpty(combatMusicSelectedKey)
                ? MusicDirectorRules.CombatGeneric
                : combatMusicSelectedKey;
        }

        private void ResetCombatMusicPresentationState()
        {
            if (combatMusicEncounter == null
                && string.IsNullOrEmpty(combatMusicBaseKey)
                && combatMusicDuckDepth <= 0f)
            {
                return;
            }

            combatMusicEncounter = null;
            combatMusicBaseKey = "";
            combatMusicSelectedKey = "";
            combatMusicSelectedAt = -1f;
            combatMusicCriticalStartedAt = -1f;
            combatMusicRecoveredStartedAt = -1f;
            combatMusicLastStandActive = false;
            combatMusicDuckStartedAt = -1f;
            combatMusicDuckFullDepthAt = -1f;
            combatMusicDuckHoldUntil = -1f;
            combatMusicDuckUntil = -1f;
            combatMusicDuckDepth = 0f;
        }

        private static void SetDominantMusicFaction(
            string candidate,
            int count,
            ref string current,
            ref int largest)
        {
            if (count <= largest) return;
            current = candidate;
            largest = count;
        }

        private bool TryNearestMusicLandmark(out ObjectType landmark, out string siteId)
        {
            landmark = default;
            siteId = "";
            if (state?.Map?.Objects == null) return false;
            int bestDistance = int.MaxValue;
            int bestPriority = -1;
            bool found = false;
            foreach (MapObject obj in state.Map.Objects)
            {
                if (obj == null || WorldSitePresentationRules.IsDecorationObjectId(obj.Id)) continue;
                bool isAuthoredSite = WorldSitePresentationRules.TryGetForLandmarkObjectId(
                    obj.Id,
                    out WorldSitePresentationProfile site)
                    && obj.Type == site.LandmarkType;
                if (!isAuthoredSite && !MusicDirectorRules.IsMusicLandmark(obj.Type)) continue;
                int distance = Mathf.Abs(obj.X - state.PlayerX) + Mathf.Abs(obj.Y - state.PlayerY);
                if (distance > 3) continue;
                int priority = isAuthoredSite ? 100 : MusicDirectorRules.LandmarkPriority(obj.Type);
                if (distance > bestDistance || distance == bestDistance && priority <= bestPriority) continue;
                landmark = obj.Type;
                siteId = isAuthoredSite ? site.SiteId : "";
                bestDistance = distance;
                bestPriority = priority;
                found = true;
            }
            return found;
        }

        private bool IsAlertedRoamingThreatNear()
        {
            if (state?.RoamingThreats == null) return false;
            return state.RoamingThreats.Any(threat =>
                threat != null
                && threat.Active
                && threat.Alerted
                && threat.Depth == state.Depth
                && Mathf.Abs(threat.X - state.PlayerX) + Mathf.Abs(threat.Y - state.PlayerY)
                    <= RoamingThreatRules.AlertRadius + 2);
        }

        private void TestSfx()
        {
            if (state == null) return;
            NormalizeGameSettings();
            state.SfxMuted = false;
            ApplyAudioSettings();
            PlaySfx("ui", 0.85f);
            PlaySfx("attack", 0.75f);
            PlaySfx("castember", 0.72f);
            PlaySfx("fieldholy", 0.62f);
            ShowBanner("Audio test");
        }

        private void ToggleSfxMute()
        {
            if (state == null) return;
            NormalizeGameSettings();
            state.SfxMuted = !state.SfxMuted;
            ApplyAudioSettings();
            if (!state.SfxMuted) PlaySfx("ui", 0.7f);
            ShowBanner(state.SfxMuted ? "SFX muted" : "SFX on");
        }

        private void ToggleMusicMute()
        {
            if (state == null) return;
            NormalizeGameSettings();
            state.MusicMuted = !state.MusicMuted;
            ApplyAudioSettings();
            if (!state.MusicMuted) PlaySfx("ui", 0.55f);
            ShowBanner(state.MusicMuted ? "Music muted" : "Music on");
        }

        private void CycleSfxVolume()
        {
            if (state == null) return;
            NormalizeGameSettings();
            if (state.SfxVolumePercent >= 100) state.SfxVolumePercent = 25;
            else state.SfxVolumePercent += 25;
            ApplyAudioSettings();
            PlaySfx("ui", 0.7f);
            ShowBanner($"Audio {state.SfxVolumePercent}%");
        }

        private void AdjustSfxVolume(int delta)
        {
            if (state == null) return;
            NormalizeGameSettings();
            state.SfxMuted = false;
            state.SfxVolumePercent = Mathf.Clamp(state.SfxVolumePercent + delta, 25, 100);
            ApplyAudioSettings();
            PlaySfx("ui", 0.7f);
            ShowBanner($"SFX {state.SfxVolumePercent}%");
        }

        private void AdjustMusicVolume(int delta)
        {
            if (state == null) return;
            NormalizeGameSettings();
            state.MusicMuted = false;
            state.MusicVolumePercent = Mathf.Clamp(state.MusicVolumePercent + delta, 25, 100);
            ApplyAudioSettings();
            PlaySfx("ui", 0.55f);
            ShowBanner($"Music {state.MusicVolumePercent}%");
        }

        private void LoadExternalArt()
        {
            titleCardArt = LoadExternalPng(RuntimeArtManifest.TitleCard)
                ?? LoadLatestExternalPng("ash-and-brimstone-title-card-runtime-", "")
                ?? LoadLatestExternalPng("ashen-halls-title-card-runtime-", "");
            gameIconArt = LoadExternalPng(RuntimeArtManifest.GameIcon)
                ?? LoadLatestExternalPng("ash-and-brimstone-icon-runtime-", "")
                ?? LoadLatestExternalPng("ashen-halls-icon-runtime-", "");
            betaCombatArt = LoadLatestExternalPng("beta-combat-casting-ui-reference-", "beta-combat-casting-ui-reference-v0.28.png");
            formulaLabArt = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.MagicUiAtlas, 0.20f, "magic UI icons", 0.10f)
                ?? LoadLatestExternalPngWithAlpha("magic-ui-atlas-runtime-", "magic-ui-atlas-runtime-v0.43.png", 0.20f, "magic UI icons", 0.10f)
                ?? LoadLatestExternalPng("spell-card-icons-reference-", "spell-card-icons-reference-v0.38.png")
                ?? LoadLatestExternalPng("formula-lab-effects-reference-", "formula-lab-effects-reference-v0.29.png");
            combatSpriteSheet = LoadLatestExternalPng("combat-sprite-sheet-alpha-", "combat-sprite-sheet-alpha-v0.43.png") ?? LoadExternalPng("combat-sprite-sheet-alpha-v0.29.png");
            classIconAtlas = LoadLatestExternalPng("class-icon-atlas-runtime-", "class-icon-atlas-runtime-v0.40.png");
            worldObjectAtlas = LoadLatestExternalPng("world-environment-atlas-runtime-", "world-environment-atlas-runtime-v0.43.png") ?? LoadLatestExternalPng("world-object-atlas-runtime-", "world-object-atlas-runtime-v0.41.png");
            Texture2D inventoryItemAtlas = LoadLatestExternalPng("item-inventory-atlas-runtime-", "");
            itemIconAtlasUsesInventoryContract = inventoryItemAtlas != null;
            itemIconAtlas = inventoryItemAtlas ?? LoadLatestExternalPng("item-equipment-atlas-runtime-", "item-equipment-atlas-runtime-v0.47.png") ?? LoadLatestExternalPng("item-icon-atlas-runtime-", "item-icon-atlas-runtime-v0.43.png");
            enemyRosterAtlas = LoadLatestExternalPng("enemy-roster-atlas-runtime-", "enemy-roster-atlas-runtime-v0.43.png");
            combatUiAtlas = LoadLatestExternalPng("combat-ui-atlas-runtime-", "combat-ui-atlas-runtime-v0.44.png");
            combatUiPanelAtlas = LoadLatestExternalPng("combat-ui-panel-atlas-runtime-", "");
            spellbookUiAtlas = LoadLatestExternalPng("spellbook-combat-ui-atlas-runtime-", "spellbook-combat-ui-atlas-runtime-v0.46.png");
            signatureSpellIconAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.SignatureSpellIconAtlas, 0.20f, "signature spell icons", 0.10f)
                ?? LoadLatestExternalPngWithAlpha("signature-spell-icon-atlas-runtime-", "", 0.20f, "signature spell icons", 0.10f);
            lightningSpellIconAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.LightningSpellIconAtlas, 0.20f, "lightning spell icons", 0.10f);
            powerBookStateIconAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.PowerBookStateIconAtlas, 0.08f, "power book state icons", 0.06f);
            emberSpellAtlas = LoadLatestExternalPng("ember-spell-effects-atlas-runtime-", "ember-spell-effects-atlas-runtime-v0.46.png");
            epicSpellEffectsAtlas = LoadExternalPng(RuntimeArtManifest.EpicSpellEffectsAtlas)
                ?? LoadLatestExternalPng("combat-spell-effects-atlas-runtime-", "")
                ?? LoadLatestExternalPng("epic-spell-effects-atlas-runtime-", "epic-spell-effects-atlas-runtime-v0.47.png");
            mageWarlockSpellVfxAtlas = LoadApprovedExternalPngWithAlpha(
                RuntimeArtManifest.MageWarlockSpellVfxAtlas,
                0.20f,
                "mage and warlock spell VFX",
                0.10f);
            supportHexSpellVfxAtlas = LoadApprovedExternalPngWithAlpha(
                RuntimeArtManifest.SupportHexSpellVfxAtlas,
                0.20f,
                "support and hex spell VFX",
                0.10f);
            classSkillVfxAtlas = LoadApprovedExternalPngWithAlpha(
                RuntimeArtManifest.ClassSkillVfxAtlas,
                0.20f,
                "class skill VFX",
                0.10f);
            combatPowerTravelVfxAtlas = LoadApprovedExternalPngWithAlpha(
                RuntimeArtManifest.CombatPowerTravelVfxAtlas,
                0.20f,
                "combat power travel VFX",
                0.10f);
            combatPowerAftermathVfxAtlas = LoadApprovedExternalPngWithAlpha(
                RuntimeArtManifest.CombatPowerAftermathVfxAtlas,
                0.20f,
                "combat power aftermath VFX",
                0.10f);
            spellAnimationAtlas = LoadExternalPng(RuntimeArtManifest.SpellAnimationAtlas)
                ?? LoadLatestExternalPng("spell-animation-atlas-runtime-", "");
            combatSpellbookUiAtlas = LoadLatestExternalPng("combat-spellbook-ui-atlas-runtime-", "combat-spellbook-ui-atlas-runtime-v0.47.png");
            pactSpellbookAtlas = LoadLatestExternalPngWithAlpha("pact-spellbook-atlas-runtime-", "", 0.20f, "pact spellbook icons");
            bossEnemyAtlas = LoadLatestExternalPng("boss-enemy-atlas-runtime-", "boss-enemy-atlas-runtime-v0.47.png");
            questWorldAtlas = LoadLatestExternalPng("quest-world-object-atlas-runtime-", "quest-world-object-atlas-runtime-v0.47.png");
            worldMapPropAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.WorldMapPropAtlas, 0.20f, "world map props", 0.16f)
                ?? LoadLatestExternalPngWithAlpha("world-map-prop-atlas-runtime-", "", 0.20f, "world map props", 0.16f);
            worldMapBiomePropAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.WorldMapBiomePropAtlas, 0.20f, "world map biome props", 0.08f)
                ?? LoadLatestExternalPngWithAlpha("world-map-biome-prop-atlas-runtime-", "", 0.20f, "world map biome props", 0.08f);
            worldMapExplorationTileAtlas = LoadExternalPng(RuntimeArtManifest.WorldMapExplorationTileAtlas)
                ?? LoadLatestExternalPng("world-map-exploration-tile-atlas-runtime-", "");
            worldMapMaterialAtlas = LoadExternalPng(RuntimeArtManifest.WorldMapMaterialAtlas)
                ?? LoadLatestExternalPng("world-map-material-atlas-runtime-", "");
            worldMapLandmarkAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.WorldMapLandmarkAtlas, 0.12f, "world map landmarks", 0.16f)
                ?? LoadLatestExternalPngWithAlpha("world-map-landmark-atlas-runtime-", "", 0.12f, "world map landmarks", 0.16f);
            worldMapRegionLandmarkAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.WorldMapRegionLandmarkAtlas, 0.20f, "world map regional landmarks", 0.16f);
            worldMapRegionMarkerAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.WorldMapRegionMarkerAtlas, 0.20f, "world map regional markers", 0.12f);
            worldAreaSetpieceAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.WorldAreaSetpieceAtlas, 0.20f, "world area set-pieces", 0.12f);
            LoadV24WorldMapArtAtlases();
            worldMapOverlayAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.WorldMapOverlayAtlas, 0.20f, "world map overlays", 0.04f)
                ?? LoadLatestExternalPngWithAlpha("world-map-overlay-atlas-runtime-", "", 0.20f, "world map overlays", 0.04f);
            worldMapProgressionOverlayAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.WorldMapProgressionOverlayAtlas, 0.20f, "world map progression overlays", 0.04f)
                ?? LoadLatestExternalPngWithAlpha("world-map-progression-overlay-atlas-runtime-", "", 0.20f, "world map progression overlays", 0.04f);
            worldMapUiAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.WorldMapUiAtlas, 0.20f, "world map UI icons", 0.10f)
                ?? LoadLatestExternalPngWithAlpha("world-map-ui-atlas-runtime-", "", 0.20f, "world map UI icons", 0.10f);
            worldMapTokenSpriteAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.WorldMapTokenSpriteAtlas, 0.20f, "world map tokens", 0.18f)
                ?? LoadLatestExternalPngWithAlpha("world-map-token-sprite-atlas-runtime-", "", 0.20f, "world map tokens", 0.18f);
            storyCardAtlas = LoadLatestExternalPng("story-card-atlas-runtime-", "");
            npcPortraitAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.NpcPortraitAtlas, 0.20f, "NPC portraits", 0.08f)
                ?? LoadLatestExternalPngWithAlpha("npc-portrait-atlas-runtime-", "", 0.20f, "NPC portraits", 0.08f);
            routeScaffoldAtlas = LoadExternalPng(RuntimeArtManifest.RouteScaffoldAtlas);
            dungeonScaffoldAtlas = LoadLatestExternalPng("dungeon-scaffold-atlas-runtime-", "");
            factionBannerAtlas = LoadLatestExternalPng("faction-banner-atlas-runtime-", "");
            serviceScaffoldAtlas = LoadLatestExternalPng("service-scaffold-atlas-runtime-", "");
            characterInventoryUiAtlas = LoadLatestExternalPng("character-inventory-ui-atlas-runtime-", "character-inventory-ui-atlas-runtime-v0.47.png");
            uniqueItemAtlas = LoadLatestExternalPng("unique-item-atlas-runtime-", "");
            combatHudUiAtlas = LoadLatestExternalPng("combat-hud-ui-atlas-runtime-", "combat-hud-ui-atlas-runtime-v0.49.png");
            combatSpellFloatAtlas = LoadLatestExternalPng("combat-spell-float-atlas-runtime-", "combat-spell-float-atlas-runtime-v0.49.png");
            enemyWorldObjectAtlas = LoadLatestExternalPng("enemy-world-object-atlas-runtime-", "enemy-world-object-atlas-runtime-v0.49.png");
            roamingThreatAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.RoamingThreatAtlas, 0.20f, "roaming threat sprites", 0.08f);
            tavernBackdropArt = LoadExternalPng(RuntimeArtManifest.TavernBackdrop)
                ?? LoadLatestExternalPng("tavern-backdrop-runtime-", "tavern-backdrop-runtime-v0.49.png");
            splashArt = tavernBackdropArt
                ?? titleCardArt
                ?? LoadLatestExternalPng("splash-title-reference-", "splash-title-reference-v0.27.png");
            tavernUiAtlas = LoadExternalPng(RuntimeArtManifest.TavernUiAtlas)
                ?? LoadLatestExternalPng("tavern-ui-atlas-runtime-", "tavern-ui-atlas-runtime-v0.49.png");
            titleMenuScrollArt = LoadApprovedExternalPngWithAlpha(
                RuntimeArtManifest.TitleMenuScroll,
                0.20f,
                "title menu scroll",
                0.52f);
            if (!TitleScreenPresentationRules.SupportsMenuScrollArt(titleMenuScrollArt))
            {
                if (titleMenuScrollArt != null)
                {
                    Debug.LogWarning("Ignoring title menu scroll with non-approved dimensions.");
                }
                titleMenuScrollArt = null;
            }
            else
            {
                titleMenuScrollArt.filterMode = FilterMode.Bilinear;
                titleMenuScrollArt.wrapMode = TextureWrapMode.Clamp;
            }
            titleMenuFocusArt = LoadApprovedExternalPngWithAlpha(
                RuntimeArtManifest.TitleMenuFocus,
                0.45f,
                "title menu focus ribbon",
                0.20f);
            if (!TitleScreenPresentationRules.SupportsMenuFocusArt(titleMenuFocusArt))
            {
                if (titleMenuFocusArt != null)
                {
                    Debug.LogWarning("Ignoring title menu focus ribbon with non-approved dimensions.");
                }
                titleMenuFocusArt = null;
            }
            else
            {
                titleMenuFocusArt.filterMode = FilterMode.Bilinear;
                titleMenuFocusArt.wrapMode = TextureWrapMode.Clamp;
            }
            titleMenuIconAtlas = LoadApprovedExternalPngWithAlpha(
                RuntimeArtManifest.TitleMenuIconAtlas,
                0.20f,
                "title menu icons",
                0.10f);
            if (!TitleScreenPresentationRules.SupportsMenuIconArt(titleMenuIconAtlas))
            {
                if (titleMenuIconAtlas != null)
                {
                    Debug.LogWarning("Ignoring title menu icon atlas with non-approved dimensions.");
                }
                titleMenuIconAtlas = null;
            }
            else
            {
                titleMenuIconAtlas.filterMode = FilterMode.Bilinear;
                titleMenuIconAtlas.wrapMode = TextureWrapMode.Clamp;
            }
            inventoryConsumableAtlas = LoadLatestExternalPng("inventory-consumable-atlas-runtime-", "inventory-consumable-atlas-runtime-v0.50.png");
            combatCommandIconAtlas = LoadApprovedExternalPngWithAlpha(
                    RuntimeArtManifest.CombatCommandIconAtlas,
                    0.08f,
                    "combat command icons",
                    0.08f)
                ?? LoadExternalPng("combat-command-icon-atlas-runtime-v0.61.png");
            abilityIconAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.AbilityIconAtlas, 0.20f, "ability icons", 0.10f)
                ?? LoadLatestExternalPngWithAlpha("ability-icon-atlas-runtime-", "", 0.20f, "ability icons", 0.10f);
            rangerAbilityEffectAtlas = LoadLatestExternalPng("ranger-ability-effect-atlas-runtime-", "");
            enemySpriteAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.EnemySpriteAtlas, 0.20f, "enemy combat sprites", 0.08f)
                ?? LoadLatestExternalPngWithAlpha("enemy-sprite-atlas-runtime-", "", 0.20f, "enemy combat sprites", 0.08f);
            characterCombatAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.CharacterCombatAtlas, 0.20f, "character combat sprites", 0.08f)
                ?? LoadLatestExternalPngWithAlpha("character-combat-atlas-runtime-", "", 0.20f, "character combat sprites", 0.08f);
            creatureSpriteAtlas = LoadLatestExternalPng("combat-sprite-atlas-runtime-", "") ?? LoadLatestExternalPng("creature-sprite-atlas-runtime-", "creature-sprite-atlas-runtime-v0.50.png");
            demonSummonAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.DemonSummonAtlas, 0.20f, "demon summon sprites")
                ?? LoadLatestExternalPngWithAlpha("demon-summon-atlas-runtime-", "", 0.20f, "demon summon sprites");
            combatTerrainAtlas = LoadLatestExternalPng("combat-terrain-atlas-runtime-", "combat-terrain-atlas-runtime-v0.50.1.png");
            koboldCombatTerrainAtlas = LoadLatestExternalPng("kobold-combat-terrain-atlas-runtime-", "");
            koboldRouteAtlas = LoadExternalPng(RuntimeArtManifest.KoboldRouteAtlas);
            koboldBossAtlas = LoadLatestExternalPng("kobold-boss-atlas-runtime-", "");
            koboldCavePropAtlas = LoadLatestExternalPng("kobold-cave-prop-atlas-runtime-", "");
            midgaardTownAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.MidgaardTownAtlas, 0.12f, "Midgaard town objects", 0.16f)
                ?? LoadLatestExternalPngWithAlpha("midgaard-town-atlas-runtime-", "", 0.12f, "Midgaard town objects", 0.16f);
            midgaardTileAtlas = LoadExternalPng(RuntimeArtManifest.MidgaardTileAtlas)
                ?? LoadLatestExternalPng("midgaard-tile-atlas-runtime-", "");
            midgaardWallAtlas = LoadExternalPng(RuntimeArtManifest.MidgaardWallAtlas)
                ?? LoadLatestExternalPng("midgaard-wall-atlas-runtime-", "");
            // v1.93 side gates deliberately trade the old opaque facade footprint
            // for a clear road lane. The active four cells still validate
            // individually at draw time, so the family-level floor can stay low.
            midgaardGateAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.MidgaardGateAtlas, 0.20f, "Midgaard gates", 0.05f)
                ?? LoadLatestExternalPngWithAlpha("midgaard-gate-atlas-runtime-", "", 0.20f, "Midgaard gates", 0.05f);
            midgaardCityPropAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.MidgaardCityPropAtlas, 0.20f, "Midgaard props", 0.16f)
                ?? LoadLatestExternalPngWithAlpha("midgaard-city-prop-atlas-runtime-", "", 0.20f, "Midgaard props", 0.16f);
            midgaardStreetLifeAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.MidgaardStreetLifeAtlas, 0.20f, "Midgaard street life", 0.10f);
            midgaardPavingDecalAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.MidgaardPavingDecalAtlas, 0.20f, "Midgaard paving decals", 0.08f);
            // NPC cells are semantic identities, not interchangeable art slots.
            // Older 5x4 sheets used several v1.93 cells as empty reserves, so a
            // geometry-compatible family fallback would silently miscast actors.
            midgaardNpcAtlas = LoadApprovedExternalPngWithAlpha(
                RuntimeArtManifest.MidgaardNpcAtlas,
                0.20f,
                "Midgaard NPCs",
                0.16f);
            midgaardSewerAtlas = LoadExternalPng(RuntimeArtManifest.MidgaardSewerAtlas);
            midgaardInteriorPropAtlas = LoadApprovedExternalPngWithAlpha(RuntimeArtManifest.MidgaardInteriorPropAtlas, 0.20f, "Midgaard interior props", 0.10f)
                ?? LoadLatestExternalPngWithAlpha("midgaard-interior-prop-atlas-runtime-", "", 0.20f, "Midgaard interior props", 0.10f);
            midgaardInteriorTileAtlas = LoadExternalPng(RuntimeArtManifest.MidgaardInteriorTileAtlas)
                ?? LoadLatestExternalPng("midgaard-interior-tile-atlas-runtime-", "");
            LoadGrandHearthArt();
            ConfigureExplorationTerrainTextures();
            explorationAtlasCellUsable.Clear();
            exploreArtMetrics.Clear();
            spriteCellMetrics.Clear();
            ValidateExplorationArtAlpha();
        }

        private void ConfigureExplorationTerrainTextures()
        {
            ConfigureExplorationTerrainTexture(worldMapExplorationTileAtlas);
            ConfigureExplorationTerrainTexture(worldMapMaterialAtlas);
            ConfigureExplorationTerrainTexture(midgaardTileAtlas);
            ConfigureExplorationTerrainTexture(midgaardInteriorTileAtlas);
            ConfigureExplorationTerrainTexture(grandHearthFloorAtlas);
        }

        private void ConfigureExplorationTerrainTexture(Texture2D texture)
        {
            if (texture == null) return;
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.anisoLevel = 0;
        }

        private void ValidateExplorationArtAlpha()
        {
            ValidateSpriteAtlasAlpha(worldMapTokenSpriteAtlas, "world map tokens", 0.20f, 0.18f);
            ValidateSpriteAtlasAlpha(worldMapPropAtlas, "world map props", 0.20f, 0.16f);
            ValidateSpriteAtlasAlpha(worldMapBiomePropAtlas, "world map biome props", 0.20f, 0.08f);
            ValidateSpriteAtlasAlpha(worldMapLandmarkAtlas, "world map landmarks", 0.12f, 0.16f);
            ValidateSpriteAtlasAlpha(worldMapRegionLandmarkAtlas, "world map regional landmarks", 0.20f, 0.16f);
            ValidateSpriteAtlasAlpha(worldMapRegionMarkerAtlas, "world map regional markers", 0.20f, 0.12f);
            ValidateSpriteAtlasAlpha(worldAreaSetpieceAtlas, "world area set-pieces", 0.20f, 0.12f);
            ValidateV24WorldMapArtContracts();
            ValidateSpriteAtlasAlpha(worldMapOverlayAtlas, "world map overlays", 0.20f, 0.04f);
            ValidateSpriteAtlasAlpha(worldMapProgressionOverlayAtlas, "world map progression overlays", 0.20f, 0.04f);
            ValidateSpriteAtlasAlpha(worldMapUiAtlas, "world map UI icons", 0.20f, 0.10f);
            ValidateSpriteAtlasAlpha(midgaardTownAtlas, "Midgaard town objects", 0.12f, 0.16f);
            ValidateSpriteAtlasAlpha(midgaardCityPropAtlas, "Midgaard props", 0.20f, 0.16f);
            ValidateSpriteAtlasAlpha(midgaardStreetLifeAtlas, "Midgaard street life", 0.20f, 0.10f);
            ValidateSpriteAtlasAlpha(midgaardPavingDecalAtlas, "Midgaard paving decals", 0.20f, 0.08f);
            ValidateSpriteAtlasAlpha(midgaardNpcAtlas, "Midgaard NPCs", 0.20f, 0.16f);
            ValidateSpriteAtlasAlpha(npcPortraitAtlas, "NPC portraits", 0.20f, 0.08f);
            ValidateSpriteAtlasAlpha(characterCombatAtlas, "character combat sprites", 0.20f, 0.08f);
            ValidateSpriteAtlasAlpha(enemySpriteAtlas, "enemy combat sprites", 0.20f, 0.08f);
            ValidateSpriteAtlasAlpha(midgaardInteriorPropAtlas, "Midgaard interior props", 0.20f, 0.10f);
            ValidateWorldMapArtContracts();
        }

        private void ValidateWorldMapArtContracts()
        {
            ValidateAtlasCells(
                abilityIconAtlas,
                "ability icon",
                CombatIconCatalog.AbilityAtlasColumns,
                CombatIconCatalog.ExpandedAbilityAtlasRows,
                true,
                CombatIconCatalog.MappedAbilityIndices.OrderBy(index => index).ToArray());
            ValidateAtlasSquareCells(
                abilityIconAtlas,
                "ability icon",
                CombatIconCatalog.AbilityAtlasColumns,
                CombatIconCatalog.ExpandedAbilityAtlasRows,
                1f);
            ValidateAtlasCells(
                signatureSpellIconAtlas,
                "signature spell icon",
                CombatIconCatalog.SignatureSpellAtlasColumns,
                CombatIconCatalog.SignatureSpellAtlasRows,
                true,
                Enumerable.Range(0, FormulaCatalog.All.Length).ToArray());
            ValidateAtlasSquareCells(
                signatureSpellIconAtlas,
                "signature spell icon",
                CombatIconCatalog.SignatureSpellAtlasColumns,
                CombatIconCatalog.SignatureSpellAtlasRows,
                1f);
            ValidateAtlasCells(lightningSpellIconAtlas, "lightning spell icon", LightningSpellIconCatalog.AtlasColumns, LightningSpellIconCatalog.AtlasRows, true, 0, 1, 2, 3, 4, 5, 6, 7);
            ValidateAtlasSquareCells(lightningSpellIconAtlas, "lightning spell icon", LightningSpellIconCatalog.AtlasColumns, LightningSpellIconCatalog.AtlasRows, 1f);
            ValidateAtlasCells(
                powerBookStateIconAtlas,
                "power book state icon",
                CombatIconCatalog.BookStateAtlasColumns,
                CombatIconCatalog.BookStateAtlasRows,
                true,
                Enumerable.Range(0, CombatIconCatalog.BookStateAtlasColumns * CombatIconCatalog.BookStateAtlasRows).ToArray());
            ValidateAtlasSquareCells(
                powerBookStateIconAtlas,
                "power book state icon",
                CombatIconCatalog.BookStateAtlasColumns,
                CombatIconCatalog.BookStateAtlasRows,
                1f);
            ValidateAtlasCells(
                mageWarlockSpellVfxAtlas,
                "mage and warlock spell VFX",
                4,
                4,
                true,
                Enumerable.Range(0, 16).ToArray());
            ValidateAtlasSquareCells(mageWarlockSpellVfxAtlas, "mage and warlock spell VFX", 4, 4, 1f);
            ValidateAtlasCells(
                supportHexSpellVfxAtlas,
                "support and hex spell VFX",
                4,
                4,
                true,
                Enumerable.Range(0, 16).ToArray());
            ValidateAtlasSquareCells(supportHexSpellVfxAtlas, "support and hex spell VFX", 4, 4, 1f);
            ValidateAtlasCells(
                classSkillVfxAtlas,
                "class skill VFX",
                4,
                4,
                true,
                Enumerable.Range(0, 16).ToArray());
            ValidateAtlasSquareCells(classSkillVfxAtlas, "class skill VFX", 4, 4, 1f);
            ValidateAtlasCells(
                combatPowerTravelVfxAtlas,
                "combat power travel VFX",
                4,
                4,
                true,
                Enumerable.Range(0, 16).ToArray());
            ValidateAtlasSquareCells(combatPowerTravelVfxAtlas, "combat power travel VFX", 4, 4, 1f);
            ValidateAtlasCells(
                combatPowerAftermathVfxAtlas,
                "combat power aftermath VFX",
                4,
                4,
                true,
                Enumerable.Range(0, 16).ToArray());
            ValidateAtlasSquareCells(combatPowerAftermathVfxAtlas, "combat power aftermath VFX", 4, 4, 1f);
            ValidateAtlasCells(spellAnimationAtlas, "spell animation", 4, 4, true, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
            ValidateAtlasSquareCells(spellAnimationAtlas, "spell animation", 4, 4, 1f);
            ValidateAtlasCells(midgaardGateAtlas, "Midgaard gate", 5, 4, false, 0, 1, 6, 7);
            if (midgaardGateAtlas == null) ValidateAtlasCells(midgaardTownAtlas, "Midgaard gate fallback", 5, 4, false, 8, 9);
            ValidateAtlasCells(midgaardWallAtlas, "Midgaard wall", 5, 4, false, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
            ValidateAtlasSquareCells(midgaardWallAtlas, "Midgaard wall", 5, 4, 3f);
            ValidateAtlasSquareCells(midgaardGateAtlas, "Midgaard gate", 5, 4, 3f);
            ValidateAtlasCells(midgaardTownAtlas, "Midgaard town object", 5, 4, false, 0, 1, 2, 3, 4, 5, 6, 7, 11, 12, 14, 15, 17);
            ValidateAtlasCells(
                midgaardNpcAtlas,
                "Midgaard NPC",
                NpcPortraitCatalog.Columns,
                NpcPortraitCatalog.Rows,
                false,
                Enumerable.Range(0, NpcPortraitCatalog.Columns * NpcPortraitCatalog.Rows).ToArray());
            ValidateAtlasSquareCells(
                midgaardNpcAtlas,
                "Midgaard NPC",
                NpcPortraitCatalog.Columns,
                NpcPortraitCatalog.Rows,
                1f);
            ValidateAtlasCells(npcPortraitAtlas, "NPC portrait", NpcPortraitCatalog.Columns, NpcPortraitCatalog.Rows, true, Enumerable.Range(0, NpcPortraitCatalog.Columns * NpcPortraitCatalog.Rows).ToArray());
            ValidateAtlasSquareCells(npcPortraitAtlas, "NPC portrait", NpcPortraitCatalog.Columns, NpcPortraitCatalog.Rows, 1f);
            ValidateAtlasCells(
                characterCombatAtlas,
                "character combat sprite",
                PlayerSpriteCatalog.Columns,
                PlayerSpriteCatalog.Rows,
                true,
                Enumerable.Range(0, PlayerSpriteCatalog.Columns * PlayerSpriteCatalog.Rows).ToArray());
            ValidateAtlasSquareCells(
                characterCombatAtlas,
                "character combat sprite",
                PlayerSpriteCatalog.Columns,
                PlayerSpriteCatalog.Rows,
                1f);
            ValidateAtlasCells(enemySpriteAtlas, "enemy combat sprite", 4, 4, true, Enumerable.Range(0, 16).ToArray());
            ValidateAtlasSquareCells(enemySpriteAtlas, "enemy combat sprite", 4, 4, 1f);
            ValidateAtlasCells(midgaardInteriorPropAtlas, "Midgaard interior prop", 5, 4, false, 0, 1, 2, 3, 4, 10, 11, 12, 13, 14, 15, 16);
            ValidateAtlasSquareCells(midgaardInteriorPropAtlas, "Midgaard interior prop", 5, 4, 3f);
            ValidateAtlasCells(midgaardCityPropAtlas, "Midgaard ambient prop", 5, 4, false, 0, 1, 2, 3, 4, 6, 8, 11, 12, 14, 16, 17, 18, 19);
            ValidateAtlasCells(midgaardStreetLifeAtlas, "Midgaard street-life prop", 5, 4, false, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19);
            ValidateAtlasSquareCells(midgaardStreetLifeAtlas, "Midgaard street-life prop", 5, 4, 3f);
            ValidateAtlasCells(midgaardPavingDecalAtlas, "Midgaard paving decal", 4, 4, false, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
            ValidateAtlasSquareCells(midgaardPavingDecalAtlas, "Midgaard paving decal", 4, 4, 3f);
            ValidateAtlasCells(worldMapTokenSpriteAtlas, "world map token", 5, 4, false, 0, 2, 3, 4, 5, 6, 7, 8);
            ValidateAtlasCells(worldMapPropAtlas, "world map prop", 5, 4, false, 1, 2, 6, 8, 9, 10, 11, 17);
            ValidateAtlasCells(worldMapBiomePropAtlas, "world map biome prop", 5, 4, false, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19);
            ValidateAtlasSquareCells(worldMapBiomePropAtlas, "world map biome prop", 5, 4, 3f);
            ValidateAtlasCells(worldMapLandmarkAtlas, "world map landmark", 5, 4, false, 1, 3, 4, 5, 6, 8, 10, 11, 13, 15, 17);
            ValidateAtlasCells(worldMapRegionLandmarkAtlas, "world map regional landmark", 5, 4, false, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19);
            ValidateAtlasSquareCells(worldMapRegionLandmarkAtlas, "world map regional landmark", 5, 4, 3f);
            ValidateAtlasCells(worldMapRegionMarkerAtlas, "world map regional marker", 5, 4, false, Enumerable.Range(0, 20).ToArray());
            ValidateAtlasSquareCells(worldMapRegionMarkerAtlas, "world map regional marker", 5, 4, 3f);
            ValidateAtlasCells(worldAreaSetpieceAtlas, "world area set-piece", 4, 2, false, Enumerable.Range(0, 8).ToArray());
            ValidateAtlasSquareCells(worldAreaSetpieceAtlas, "world area set-piece", 4, 2, 3f);
            ValidateAtlasCells(worldMapOverlayAtlas, "world map overlay", 5, 4, false, Enumerable.Range(0, 19).ToArray());
            ValidateAtlasCells(worldMapProgressionOverlayAtlas, "world map progression overlay", 5, 4, false, Enumerable.Range(0, 20).ToArray());
            int explorationRows = WorldMapExplorationTileAtlasRows();
            ValidateAtlasCells(
                worldMapExplorationTileAtlas,
                "world map exploration terrain",
                5,
                explorationRows,
                false,
                Enumerable.Range(0, 5 * explorationRows).ToArray());
            ValidateAtlasSquareCells(worldMapExplorationTileAtlas, "world map exploration terrain", 5, explorationRows, 3f);
            int materialColumns = WorldMapMaterialAtlasColumns();
            ValidateAtlasCells(
                worldMapMaterialAtlas,
                "world map material",
                materialColumns,
                materialColumns,
                false,
                Enumerable.Range(0, materialColumns * materialColumns).ToArray());
            ValidateAtlasSquareCells(worldMapMaterialAtlas, "world map material", materialColumns, materialColumns, 3f);
        }

        private void ValidateAtlasSquareCells(Texture2D texture, string label, int columns, int rows, float tolerance)
        {
            if (texture == null) return;
            if (!AtlasHasSquareCells(texture, columns, rows, tolerance))
            {
                float cellW = texture.width / (float)Mathf.Max(1, columns);
                float cellH = texture.height / (float)Mathf.Max(1, rows);
                Debug.LogWarning($"{label} atlas '{texture.name}' has non-square {columns}x{rows} cells ({Mathf.RoundToInt(cellW)}x{Mathf.RoundToInt(cellH)}). Re-export as square cells to avoid map scaling artifacts.");
            }
        }

        private bool AtlasHasSquareCells(Texture2D texture, int columns, int rows, float tolerance = 2f)
        {
            if (texture == null || columns <= 0 || rows <= 0) return false;
            float cellW = texture.width / (float)columns;
            float cellH = texture.height / (float)rows;
            return Mathf.Abs(cellW - cellH) <= Mathf.Max(0f, tolerance);
        }

        private void ValidateAtlasCells(Texture2D texture, string label, int columns, int rows, bool warnIfMissing, params int[] requiredCells)
        {
            if (columns <= 0 || rows <= 0) return;
            if (texture == null)
            {
                if (warnIfMissing) Debug.LogWarning($"{label} atlas is missing.");
                return;
            }

            int cellCount = columns * rows;
            if (texture.width < columns || texture.height < rows)
            {
                Debug.LogWarning($"{label} atlas '{texture.name}' is too small for a {columns}x{rows} contract.");
                return;
            }

            Color32[] pixels;
            int textureWidth;
            int textureHeight;
            bool hasPixelSnapshot = TrySnapshotAtlasPixels(texture, out pixels, out textureWidth, out textureHeight);
            foreach (int cell in requiredCells)
            {
                if (cell < 0 || cell >= cellCount)
                {
                    Debug.LogWarning($"{label} atlas contract asks for invalid cell {cell}; valid range is 0-{cellCount - 1}.");
                    continue;
                }

                Rect source = AtlasCell(texture, cell, columns, rows);
                float visible = hasPixelSnapshot
                    ? VisiblePixelFractionFromSnapshot(pixels, textureWidth, textureHeight, source)
                    : -1f;
                if (visible >= 0f && visible < 0.04f)
                {
                    Debug.LogWarning($"{label} atlas '{texture.name}' cell {cell} is nearly empty ({Mathf.RoundToInt(visible * 100f)}% visible pixels).");
                }
            }
        }

        private void ValidateSpriteAtlasAlpha(Texture2D texture, string label, float minimumTransparentFraction, float minimumVisibleFraction = 0f)
        {
            if (texture == null) return;
            float fraction;
            float visible;
            MeasureAtlasAlpha(texture, out fraction, out visible);
            if (fraction < 0f)
            {
                Debug.LogWarning($"Could not validate {label} alpha for '{texture.name}'.");
                return;
            }

            if (fraction < minimumTransparentFraction)
            {
                Debug.LogWarning($"{label} atlas '{texture.name}' is mostly opaque ({Mathf.RoundToInt(fraction * 100f)}% transparent); map sprites may blend into floor tiles.");
            }

            if (minimumVisibleFraction > 0f && visible >= 0f && visible < minimumVisibleFraction)
            {
                Debug.LogWarning($"{label} atlas '{texture.name}' may be over-pruned ({Mathf.RoundToInt(visible * 100f)}% visible pixels); sprites may appear broken or incomplete.");
            }
        }

        private IEnumerable<string> ExternalArtDirectories()
        {
            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
            if (!string.IsNullOrEmpty(projectRoot)) yield return Path.Combine(projectRoot, "Docs", "ArtReferences");
            yield return Path.Combine(Application.dataPath, "Docs", "ArtReferences");
        }

        private Texture2D LoadLatestExternalPng(string filePrefix, string fallbackFileName)
        {
            try
            {
                IEnumerable<string> candidates = ExternalArtDirectories()
                    .Where(directory => !string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                    .SelectMany(directory => Directory.GetFiles(directory, filePrefix + "*.png"))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderByDescending(ArtVersionSortKey)
                    .ThenByDescending(File.GetLastWriteTimeUtc)
                    .ThenByDescending(path => Path.GetFileName(path));
                foreach (string path in candidates)
                {
                    Texture2D texture = TryLoadExternalPngPath(path);
                    if (texture != null) return texture;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Could not load latest external art for " + filePrefix + ": " + ex.Message);
            }
            return string.IsNullOrEmpty(fallbackFileName) ? null : LoadExternalPng(fallbackFileName);
        }

        private Texture2D LoadLatestExternalPngWithAlpha(string filePrefix, string fallbackFileName, float minimumTransparentFraction, string label, float minimumVisibleFraction = 0f)
        {
            try
            {
                IEnumerable<string> candidates = ExternalArtDirectories()
                    .Where(directory => !string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                    .SelectMany(directory => Directory.GetFiles(directory, filePrefix + "*.png"))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderByDescending(ArtVersionSortKey)
                    .ThenByDescending(File.GetLastWriteTimeUtc)
                    .ThenByDescending(path => Path.GetFileName(path));
                foreach (string path in candidates)
                {
                    Texture2D texture = TryLoadExternalPngPath(path);
                    if (texture == null) continue;
                    float transparent;
                    float visible;
                    MeasureAtlasAlpha(texture, out transparent, out visible);
                    bool transparentEnough = transparent < 0f || transparent >= minimumTransparentFraction;
                    bool visibleEnough = minimumVisibleFraction <= 0f || visible < 0f || visible >= minimumVisibleFraction;
                    if (transparentEnough && visibleEnough) return texture;
                    if (!transparentEnough)
                    {
                        Debug.LogWarning($"Skipped {label} atlas '{Path.GetFileName(path)}' because it is only {Mathf.RoundToInt(transparent * 100f)}% transparent.");
                    }
                    else
                    {
                        Debug.LogWarning($"Skipped {label} atlas '{Path.GetFileName(path)}' because it appears over-pruned ({Mathf.RoundToInt(visible * 100f)}% visible pixels).");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Could not load latest alpha external art for " + filePrefix + ": " + ex.Message);
            }

            Texture2D fallback = string.IsNullOrEmpty(fallbackFileName) ? null : LoadExternalPng(fallbackFileName);
            if (fallback != null && TransparentPixelFraction(fallback) < minimumTransparentFraction)
            {
                Debug.LogWarning($"{label} fallback atlas '{fallback.name}' is below the transparency target; using it only because no better atlas loaded.");
            }
            return fallback;
        }

        private Texture2D LoadApprovedExternalPngWithAlpha(string fileName, float minimumTransparentFraction, string label, float minimumVisibleFraction = 0f)
        {
            Texture2D texture = LoadExternalPng(fileName);
            if (texture == null) return null;

            float transparent;
            float visible;
            MeasureAtlasAlpha(texture, out transparent, out visible);
            bool transparentEnough = transparent < 0f || transparent >= minimumTransparentFraction;
            bool visibleEnough = minimumVisibleFraction <= 0f || visible < 0f || visible >= minimumVisibleFraction;
            if (transparentEnough && visibleEnough) return texture;

            string reason = !transparentEnough
                ? $"only {Mathf.RoundToInt(transparent * 100f)}% transparent"
                : $"only {Mathf.RoundToInt(visible * 100f)}% visible";
            Debug.LogWarning($"Rejected approved {label} atlas '{fileName}' because it is {reason}; trying the development fallback chain.");
            return null;
        }

        private float TransparentPixelFraction(Texture2D texture)
        {
            if (texture == null) return -1f;
            Color32[] pixels;
            int textureWidth;
            int textureHeight;
            if (!TrySnapshotAtlasPixels(texture, out pixels, out textureWidth, out textureHeight)) return -1f;
            return TransparentPixelFractionFromSnapshot(pixels, textureWidth, textureHeight);
        }

        private float VisiblePixelFraction(Texture2D texture)
        {
            if (texture == null) return -1f;
            Color32[] pixels;
            int textureWidth;
            int textureHeight;
            if (!TrySnapshotAtlasPixels(texture, out pixels, out textureWidth, out textureHeight)) return -1f;
            return VisiblePixelFractionFromSnapshot(pixels, textureWidth, textureHeight);
        }

        private float VisiblePixelFraction(Texture2D texture, Rect sourcePixels)
        {
            if (texture == null || sourcePixels.width <= 0f || sourcePixels.height <= 0f) return -1f;
            Color32[] pixels;
            int textureWidth;
            int textureHeight;
            if (!TrySnapshotAtlasPixels(texture, out pixels, out textureWidth, out textureHeight)) return -1f;
            return VisiblePixelFractionFromSnapshot(pixels, textureWidth, textureHeight, sourcePixels);
        }

        private void MeasureAtlasAlpha(Texture2D texture, out float transparentFraction, out float visibleFraction)
        {
            transparentFraction = -1f;
            visibleFraction = -1f;

            Color32[] pixels;
            int textureWidth;
            int textureHeight;
            if (!TrySnapshotAtlasPixels(texture, out pixels, out textureWidth, out textureHeight)) return;

            transparentFraction = TransparentPixelFractionFromSnapshot(pixels, textureWidth, textureHeight);
            visibleFraction = VisiblePixelFractionFromSnapshot(pixels, textureWidth, textureHeight);
        }

        private bool TrySnapshotAtlasPixels(Texture2D texture, out Color32[] pixels, out int textureWidth, out int textureHeight)
        {
            Exception error;
            return TrySnapshotAtlasPixels(texture, out pixels, out textureWidth, out textureHeight, out error);
        }

        private bool TrySnapshotAtlasPixels(Texture2D texture, out Color32[] pixels, out int textureWidth, out int textureHeight, out Exception error)
        {
            pixels = null;
            textureWidth = texture == null ? 0 : texture.width;
            textureHeight = texture == null ? 0 : texture.height;
            error = null;
            if (texture == null) return false;

            try
            {
                pixels = texture.GetPixels32();
                return AtlasPixelSnapshotIsUsable(pixels, textureWidth, textureHeight);
            }
            catch (Exception ex)
            {
                pixels = null;
                error = ex;
                return false;
            }
        }

        private float TransparentPixelFractionFromSnapshot(Color32[] pixels, int textureWidth, int textureHeight)
        {
            if (!AtlasPixelSnapshotIsUsable(pixels, textureWidth, textureHeight)) return -1f;
            int transparent = 0;
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a < 32) transparent++;
            }
            return transparent / (float)pixels.Length;
        }

        private float VisiblePixelFractionFromSnapshot(Color32[] pixels, int textureWidth, int textureHeight)
        {
            if (!AtlasPixelSnapshotIsUsable(pixels, textureWidth, textureHeight)) return -1f;
            int visible = 0;
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a >= 32) visible++;
            }
            return visible / (float)pixels.Length;
        }

        private float VisiblePixelFractionFromSnapshot(Color32[] pixels, int textureWidth, int textureHeight, Rect sourcePixels)
        {
            if (!AtlasPixelSnapshotIsUsable(pixels, textureWidth, textureHeight)
                || sourcePixels.width <= 0f
                || sourcePixels.height <= 0f
                || (long)textureWidth * textureHeight > pixels.Length)
            {
                return -1f;
            }

            int x0 = Mathf.Clamp(Mathf.FloorToInt(sourcePixels.x), 0, textureWidth - 1);
            int x1 = Mathf.Clamp(Mathf.CeilToInt(sourcePixels.x + sourcePixels.width), x0 + 1, textureWidth);

            // Atlas rects are authored and rendered from a top-left origin, while
            // Texture2D.GetPixels32 is laid out from the bottom row upward.
            int topY0 = Mathf.Clamp(Mathf.FloorToInt(sourcePixels.y), 0, textureHeight - 1);
            int topY1 = Mathf.Clamp(Mathf.CeilToInt(sourcePixels.y + sourcePixels.height), topY0 + 1, textureHeight);
            int y0 = Mathf.Clamp(textureHeight - topY1, 0, textureHeight - 1);
            int y1 = Mathf.Clamp(textureHeight - topY0, y0 + 1, textureHeight);
            int total = Mathf.Max(1, (x1 - x0) * (y1 - y0));
            int visible = 0;

            for (int y = y0; y < y1; y++)
            {
                int row = y * textureWidth;
                for (int x = x0; x < x1; x++)
                {
                    if (pixels[row + x].a >= 32) visible++;
                }
            }

            return visible / (float)total;
        }

        private bool AtlasPixelSnapshotIsUsable(Color32[] pixels, int textureWidth, int textureHeight)
        {
            return pixels != null
                && textureWidth > 0
                && textureHeight > 0
                && (long)textureWidth * textureHeight == pixels.Length;
        }

        private Texture2D LoadExternalPng(string fileName)
        {
            try
            {
                foreach (string directory in ExternalArtDirectories())
                {
                    if (string.IsNullOrEmpty(directory)) continue;
                    Texture2D texture = TryLoadExternalPngPath(Path.Combine(directory, fileName));
                    if (texture != null) return texture;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Could not load external art: " + ex.Message);
            }
            return null;
        }

        private Texture2D TryLoadExternalPngPath(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return null;
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            Type imageConversion = Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule");
            System.Reflection.MethodInfo loadImage = imageConversion?.GetMethod("LoadImage", new[] { typeof(Texture2D), typeof(byte[]) });
            if (loadImage == null || !(bool)loadImage.Invoke(null, new object[] { texture, File.ReadAllBytes(path) })) return null;
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.name = Path.GetFileName(path);
            return texture;
        }

        private long ArtVersionSortKey(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path) ?? "";
            int versionStart = name.LastIndexOf("-v", StringComparison.OrdinalIgnoreCase);
            if (versionStart < 0) return -1;
            string version = name.Substring(versionStart + 2);
            long key = 0;
            int segmentCount = 0;
            foreach (string rawPart in version.Split('.'))
            {
                if (segmentCount >= 4) break;
                int value = 0;
                int digitCount = 0;
                foreach (char ch in rawPart)
                {
                    if (!char.IsDigit(ch)) break;
                    value = Mathf.Min(999, value * 10 + (ch - '0'));
                    digitCount++;
                }
                key = key * 1000 + (digitCount > 0 ? value : 0);
                segmentCount++;
            }
            while (segmentCount++ < 4) key *= 1000;
            return key;
        }

        private bool TryDrawValidatedExplorationAtlasCell(Texture2D texture, Rect destination, int index, int columns, int rows, Color tint, string label, float minimumVisibleFraction, float maximumVisibleFraction = 0.92f)
        {
            if (texture == null || index < 0 || columns <= 0 || rows <= 0) return false;
            int cellCount = columns * rows;
            if (index >= cellCount)
            {
                Debug.LogWarning($"{label} atlas '{texture.name}' does not have cell {index}; expected 0-{cellCount - 1}.");
                return false;
            }

            Rect source = AtlasCell(texture, index, columns, rows);
            if (!ExplorationAtlasCellLooksUsable(texture, source, index, label, minimumVisibleFraction, maximumVisibleFraction)) return false;
            return DrawTextureRegionTint(texture, destination, source, tint);
        }

        private bool TryDrawTrimmedExplorationAtlasCell(Texture2D texture, Rect destination, int index, int columns, int rows, Color tint, string label, float minimumVisibleFraction, float maximumVisibleFraction = 0.92f)
        {
            return TryDrawTrimmedExplorationAtlasCell(texture, destination, index, columns, rows, tint, label, minimumVisibleFraction, maximumVisibleFraction, DefaultWorldMapArtSpec());
        }

        private bool TryDrawTrimmedExplorationAtlasCell(Texture2D texture, Rect destination, int index, int columns, int rows, Color tint, string label, float minimumVisibleFraction, float maximumVisibleFraction, WorldMapArtSpec spec)
        {
            if (texture == null || index < 0 || columns <= 0 || rows <= 0) return false;
            int cellCount = columns * rows;
            if (index >= cellCount)
            {
                Debug.LogWarning($"{label} atlas '{texture.name}' does not have cell {index}; expected 0-{cellCount - 1}.");
                return false;
            }

            Rect source = AtlasCell(texture, index, columns, rows);
            ExploreArtMetrics metrics;
            if (!TryResolveExploreArtMetrics(
                    texture,
                    source,
                    index,
                    label,
                    minimumVisibleFraction,
                    maximumVisibleFraction,
                    out metrics))
            {
                return false;
            }
            return DrawTextureRegionTintAnchored(texture, destination, metrics.Source, tint, spec);
        }

        private bool TryResolveExploreArtMetrics(
            Texture2D texture,
            Rect sourcePixels,
            int index,
            string label,
            float minimumVisibleFraction,
            float maximumVisibleFraction,
            out ExploreArtMetrics metrics)
        {
            metrics = null;
            if (texture == null) return false;

            string usabilityKey = ExplorationAtlasCellUsabilityKey(texture, sourcePixels, minimumVisibleFraction, maximumVisibleFraction);
            bool usable;
            bool hasUsability = explorationAtlasCellUsable.TryGetValue(usabilityKey, out usable);
            if (hasUsability && !usable) return false;

            string metricsKey = ExploreArtMetricsKey(texture, sourcePixels, index);
            bool hasMetrics = exploreArtMetrics.TryGetValue(metricsKey, out metrics);
            if (hasUsability && hasMetrics) return true;

            Color32[] pixels;
            int textureWidth;
            int textureHeight;
            Exception snapshotError;
            bool hasPixelSnapshot = TrySnapshotAtlasPixels(texture, out pixels, out textureWidth, out textureHeight, out snapshotError);
            if (!hasMetrics && snapshotError != null)
            {
                Debug.LogWarning($"Could not trim {label} cell {index}: {snapshotError.Message}");
            }
            if (!hasUsability)
            {
                float visible = hasPixelSnapshot
                    ? VisiblePixelFractionFromSnapshot(pixels, textureWidth, textureHeight, sourcePixels)
                    : -1f;
                usable = CacheExplorationAtlasCellUsability(
                    texture,
                    usabilityKey,
                    index,
                    label,
                    minimumVisibleFraction,
                    maximumVisibleFraction,
                    visible);
                if (!usable) return false;
            }

            if (!hasMetrics)
            {
                Rect trimmedSource = hasPixelSnapshot
                    ? TrimVisibleSourceFromSnapshot(pixels, textureWidth, textureHeight, sourcePixels)
                    : sourcePixels;
                metrics = new ExploreArtMetrics { Source = trimmedSource };
                exploreArtMetrics[metricsKey] = metrics;
            }
            return true;
        }

        private ExploreArtMetrics ExploreArtMetricsFor(Texture2D texture, Rect sourcePixels, int index, string label)
        {
            string key = ExploreArtMetricsKey(texture, sourcePixels, index);
            ExploreArtMetrics cached;
            if (exploreArtMetrics.TryGetValue(key, out cached)) return cached;

            ExploreArtMetrics metrics = new ExploreArtMetrics { Source = TrimVisibleSource(texture, sourcePixels, label, index) };
            exploreArtMetrics[key] = metrics;
            return metrics;
        }

        private string ExploreArtMetricsKey(Texture2D texture, Rect sourcePixels, int index)
        {
            return texture.GetInstanceID() + ":" + index + ":" + Mathf.RoundToInt(sourcePixels.x) + ":" + Mathf.RoundToInt(sourcePixels.y) + ":" + Mathf.RoundToInt(sourcePixels.width) + ":" + Mathf.RoundToInt(sourcePixels.height);
        }

        private Rect TrimVisibleSource(Texture2D texture, Rect sourcePixels, string label, int index)
        {
            if (texture == null || sourcePixels.width <= 0f || sourcePixels.height <= 0f) return sourcePixels;
            try
            {
                Color32[] pixels;
                int textureWidth;
                int textureHeight;
                Exception snapshotError;
                if (!TrySnapshotAtlasPixels(texture, out pixels, out textureWidth, out textureHeight, out snapshotError))
                {
                    if (snapshotError != null)
                    {
                        Debug.LogWarning($"Could not trim {label} cell {index}: {snapshotError.Message}");
                    }
                    return sourcePixels;
                }
                return TrimVisibleSourceFromSnapshot(pixels, textureWidth, textureHeight, sourcePixels);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Could not trim {label} cell {index}: {ex.Message}");
                return sourcePixels;
            }
        }

        private Rect TrimVisibleSourceFromSnapshot(Color32[] pixels, int textureWidth, int textureHeight, Rect sourcePixels)
        {
            if (!AtlasPixelSnapshotIsUsable(pixels, textureWidth, textureHeight)
                || sourcePixels.width <= 0f
                || sourcePixels.height <= 0f
                || (long)textureWidth * textureHeight > pixels.Length)
            {
                return sourcePixels;
            }

            int x0 = Mathf.Clamp(Mathf.FloorToInt(sourcePixels.x), 0, textureWidth - 1);
            int y0 = Mathf.Clamp(Mathf.FloorToInt(sourcePixels.y), 0, textureHeight - 1);
            int x1 = Mathf.Clamp(Mathf.CeilToInt(sourcePixels.x + sourcePixels.width), x0 + 1, textureWidth);
            int y1 = Mathf.Clamp(Mathf.CeilToInt(sourcePixels.y + sourcePixels.height), y0 + 1, textureHeight);
            int minX = x1;
            int minY = y1;
            int maxX = x0;
            int maxY = y0;

            for (int topY = y0; topY < y1; topY++)
            {
                int pixelY = textureHeight - 1 - topY;
                if (pixelY < 0 || pixelY >= textureHeight) continue;
                int row = pixelY * textureWidth;
                for (int x = x0; x < x1; x++)
                {
                    if (pixels[row + x].a < 32) continue;
                    minX = Mathf.Min(minX, x);
                    minY = Mathf.Min(minY, topY);
                    maxX = Mathf.Max(maxX, x + 1);
                    maxY = Mathf.Max(maxY, topY + 1);
                }
            }

            if (minX >= maxX || minY >= maxY) return sourcePixels;
            int pad = Mathf.Max(1, Mathf.RoundToInt(Mathf.Min(sourcePixels.width, sourcePixels.height) * 0.015f));
            minX = Mathf.Max(x0, minX - pad);
            minY = Mathf.Max(y0, minY - pad);
            maxX = Mathf.Min(x1, maxX + pad);
            maxY = Mathf.Min(y1, maxY + pad);
            return new Rect(minX, minY, Mathf.Max(1, maxX - minX), Mathf.Max(1, maxY - minY));
        }

        private bool DrawTextureRegionTintAnchored(Texture2D texture, Rect destination, Rect sourcePixels, Color tint, WorldMapArtSpec spec)
        {
            if (texture == null || sourcePixels.width <= 0f || sourcePixels.height <= 0f) return false;
            float scale = Mathf.Max(0.10f, spec.Scale <= 0f ? 1f : spec.Scale);
            if (!spec.AllowOverflow) scale = Mathf.Min(1f, scale);
            Rect box = new Rect(
                destination.center.x - destination.width * scale * 0.5f + spec.Offset.x * destination.width,
                destination.center.y - destination.height * scale * 0.5f + spec.Offset.y * destination.height,
                destination.width * scale,
                destination.height * scale);

            float sourceAspect = sourcePixels.width / Mathf.Max(1f, sourcePixels.height);
            float targetAspect = box.width / Mathf.Max(1f, box.height);
            Rect fit;
            if (targetAspect > sourceAspect)
            {
                float width = box.height * sourceAspect;
                fit = new Rect(box.x + (box.width - width) * Mathf.Clamp01(spec.Pivot.x), box.y, width, box.height);
            }
            else
            {
                float height = box.width / sourceAspect;
                fit = new Rect(box.x, box.y + (box.height - height) * Mathf.Clamp01(spec.Pivot.y), box.width, height);
            }

            Rect tex = new Rect(
                sourcePixels.x / texture.width,
                1f - (sourcePixels.y + sourcePixels.height) / texture.height,
                sourcePixels.width / texture.width,
                sourcePixels.height / texture.height);

            Color old = GUI.color;
            GUI.color = tint;
            GUI.DrawTextureWithTexCoords(fit, texture, tex, true);
            GUI.color = old;
            return true;
        }

        private WorldMapArtSpec DefaultWorldMapArtSpec()
        {
            return new WorldMapArtSpec(0.98f, new Vector2(0.5f, 1f), Vector2.zero, false);
        }

        private bool ExplorationAtlasCellLooksUsable(Texture2D texture, Rect sourcePixels, int index, string label, float minimumVisibleFraction, float maximumVisibleFraction)
        {
            if (texture == null) return false;
            string key = ExplorationAtlasCellUsabilityKey(texture, sourcePixels, minimumVisibleFraction, maximumVisibleFraction);
            bool cached;
            if (explorationAtlasCellUsable.TryGetValue(key, out cached)) return cached;

            float visible = VisiblePixelFraction(texture, sourcePixels);
            return CacheExplorationAtlasCellUsability(texture, key, index, label, minimumVisibleFraction, maximumVisibleFraction, visible);
        }

        private string ExplorationAtlasCellUsabilityKey(Texture2D texture, Rect sourcePixels, float minimumVisibleFraction, float maximumVisibleFraction)
        {
            return texture.GetInstanceID() + ":" + Mathf.RoundToInt(sourcePixels.x) + ":" + Mathf.RoundToInt(sourcePixels.y) + ":" + Mathf.RoundToInt(sourcePixels.width) + ":" + Mathf.RoundToInt(sourcePixels.height) + ":" + Mathf.RoundToInt(minimumVisibleFraction * 10000f) + ":" + Mathf.RoundToInt(maximumVisibleFraction * 10000f);
        }

        private bool CacheExplorationAtlasCellUsability(
            Texture2D texture,
            string key,
            int index,
            string label,
            float minimumVisibleFraction,
            float maximumVisibleFraction,
            float visible)
        {
            bool usable = visible < 0f || (visible >= minimumVisibleFraction && visible <= maximumVisibleFraction);
            explorationAtlasCellUsable[key] = usable;
            if (!usable)
            {
                string reason = visible < minimumVisibleFraction ? "over-pruned" : "mostly opaque";
                Debug.LogWarning($"Skipped {label} cell {index} in '{texture.name}' because it appears {reason} ({Mathf.RoundToInt(visible * 100f)}% visible pixels).");
            }
            return usable;
        }

        private Rect AtlasCell(Texture2D texture, int index, int columns, int rows)
        {
            if (texture == null || index < 0 || columns <= 0 || rows <= 0) return Rect.zero;
            int max = columns * rows - 1;
            index = Mathf.Clamp(index, 0, max);
            float cellW = texture.width / (float)columns;
            float cellH = texture.height / (float)rows;
            int col = index % columns;
            int row = index / columns;
            return new Rect(col * cellW, row * cellH, cellW, cellH);
        }

        private int AtlasRows(Texture2D texture, int columns)
        {
            if (texture == null || columns <= 0) return 0;
            float cellW = texture.width / (float)columns;
            return Mathf.Max(1, Mathf.RoundToInt(texture.height / Mathf.Max(1f, cellW)));
        }

        private Rect AspectFit(Rect outer, float sourceWidth, float sourceHeight)
        {
            if (sourceWidth <= 0f || sourceHeight <= 0f) return outer;
            float sourceAspect = sourceWidth / sourceHeight;
            float targetAspect = outer.width / Mathf.Max(1f, outer.height);
            if (targetAspect > sourceAspect)
            {
                float width = outer.height * sourceAspect;
                return new Rect(outer.x + (outer.width - width) * 0.5f, outer.y, width, outer.height);
            }

            float height = outer.width / sourceAspect;
            return new Rect(outer.x, outer.y + (outer.height - height) * 0.5f, outer.width, height);
        }

        private bool DrawClassIcon(Rect rect, string classKey, string role, Color accent)
        {
            DrawRect(rect, Hex("050708", 0.86f));
            int index = ClassIconIndex(classKey, role);
            bool drawn = index >= 0 && classIconAtlas != null && DrawClassIconRegion(Pad(rect, rect.width * 0.08f), ClassIconCell(index));
            DrawBorder(rect, accent, 1);
            if (!drawn)
            {
                Rect inner = Pad(rect, rect.width * 0.20f);
                DrawMiniRoleGlyph(inner, role, accent);
                GUI.Label(new Rect(rect.x, rect.yMax - rect.height * 0.36f, rect.width, rect.height * 0.34f), ClassShortLabel(classKey, role), CenterStyle(Mathf.RoundToInt(Mathf.Clamp(rect.height * 0.22f, 7f, 11f)), cursorWhite));
            }
            return drawn;
        }

        private Rect ClassIconCell(int index)
        {
            if (classIconAtlas == null) return Rect.zero;
            float cellW = classIconAtlas.width / 4f;
            float cellH = classIconAtlas.height / 2f;
            int col = Mathf.Clamp(index % 4, 0, 3);
            int row = Mathf.Clamp(index / 4, 0, 1);
            return new Rect(col * cellW, row * cellH, cellW, cellH);
        }

        private int ClassIconIndex(string classKey, string role)
        {
            switch ((classKey ?? "").ToLowerInvariant())
            {
                case "warrior": return 0;
                case "ranger": return 1;
                case "rogue": return 2;
                case "wizard": return 3;
                case "mage": return 4;
                case "warlock": return 5;
                case "priest": return 6;
                case "paladin": return 7;
            }

            switch ((role ?? "").ToLowerInvariant())
            {
                case "shield":
                case "pike": return 0;
                case "bow": return 1;
                case "knife": return 2;
                case "ember": return 4;
                case "hex": return 5;
                case "mender": return 6;
                case "ward": return 7;
                default: return -1;
            }
        }

        private bool TryDrawWorldObjectIcon(Rect rect, ObjectType type, MapObject obj = null)
        {
            return TryDrawWorldObjectIcon(rect, type, obj, Color.white);
        }

        private bool TryDrawWorldObjectIcon(Rect rect, ObjectType type, MapObject obj, Color tint)
        {
            if (TryDrawGrandHearthSetpieceAtlasIcon(rect, obj, tint)) return true;
            if (obj != null
                && string.Equals(obj.Id, MidgaardInteriorRules.GrandHearthFireId, StringComparison.Ordinal)
                && TryDrawTavernUiAtlasIcon(rect, 0, tint))
            {
                return true;
            }
            WorldMapArtSpec spec = WorldMapArtSpecFor(type, obj);
            GateOrientation? gate = GetGateOrientation(obj, type);
            if (gate.HasValue)
            {
                int gateIndex = GateAtlasIndex(gate.Value);
                if (gateIndex >= 0 && TryDrawMidgaardGateAtlasIcon(rect, gateIndex, tint, spec)) return true;
                int fallbackGateIndex = GateTownFallbackAtlasIndex(gate.Value);
                if (fallbackGateIndex >= 0 && TryDrawMidgaardTownAtlasIcon(rect, fallbackGateIndex, tint, spec)) return true;
                return false;
            }
            // The opaque wall atlas belongs to the terrain layer. Reusing it here creates square wall overlays.
            if (type == ObjectType.CityWall) return false;
            int interiorPropIndex = MidgaardInteriorPropIconIndex(type, obj);
            if (interiorPropIndex >= 0 && TryDrawMidgaardInteriorPropAtlasIcon(rect, interiorPropIndex, tint, spec)) return true;
            int midgaardSewerIndex = MidgaardSewerObjectIconIndex(type);
            if (midgaardSewerIndex >= 0 && TryDrawMidgaardSewerAtlasIcon(rect, midgaardSewerIndex, tint, spec)) return true;
            int midgaardNpcIndex = MidgaardNpcObjectIconIndex(type, obj);
            if (midgaardNpcIndex >= 0)
            {
                // A missing semantic NPC sheet must reach the role-aware
                // procedural silhouette, never a clamped generic atlas cell.
                return TryDrawMidgaardNpcAtlasIcon(rect, midgaardNpcIndex, tint, spec);
            }
            int midgaardTownIndex = MidgaardTownObjectIconIndexFor(type, obj);
            if (midgaardTownIndex >= 0 && TryDrawMidgaardTownAtlasIcon(rect, midgaardTownIndex, tint, spec)) return true;
            int regionalLandmarkIndex = WorldMapRegionLandmarkIconIndex(type, obj);
            if (regionalLandmarkIndex >= 0 && TryDrawWorldMapRegionLandmarkAtlasIcon(rect, regionalLandmarkIndex, tint, spec)) return true;
            int routeScaffoldIndex = RouteScaffoldObjectIconIndex(type);
            if (routeScaffoldIndex >= 0 && TryDrawRouteScaffoldAtlasIcon(rect, routeScaffoldIndex, tint)) return true;
            int dungeonScaffoldIndex = DungeonScaffoldObjectIconIndex(type);
            if (dungeonScaffoldIndex >= 0 && TryDrawDungeonScaffoldAtlasIcon(rect, dungeonScaffoldIndex, tint)) return true;
            int serviceScaffoldIndex = ServiceScaffoldObjectIconIndex(type);
            if (serviceScaffoldIndex >= 0 && TryDrawServiceScaffoldAtlasIcon(rect, serviceScaffoldIndex, tint)) return true;
            int landmarkIndex = WorldMapLandmarkIconIndex(type);
            if (landmarkIndex >= 0 && TryDrawWorldMapLandmarkAtlasIcon(rect, landmarkIndex, tint, spec)) return true;
            int propIndex = WorldMapPropIconIndex(type);
            if (propIndex >= 0 && TryDrawWorldMapPropAtlasIcon(rect, propIndex, tint, spec)) return true;
            int enemyWorldIndex = EnemyWorldObjectIconIndex(type);
            if (enemyWorldIndex >= 0 && TryDrawEnemyWorldObjectAtlasIcon(rect, enemyWorldIndex, tint)) return true;
            int questIndex = QuestWorldObjectIconIndex(type);
            if (questIndex >= 0 && TryDrawQuestWorldAtlasIcon(rect, questIndex, tint)) return true;
            if (IsRouteScaffoldObject(type)) return false;
            if (worldObjectAtlas == null) return false;
            Rect source = WorldObjectIconCell(type);
            if (source.width <= 0f || source.height <= 0f) return false;
            DrawTextureRegionTint(worldObjectAtlas, rect, source, tint);
            return true;
        }

        private WorldMapArtSpec WorldMapArtSpecFor(ObjectType type, MapObject obj)
        {
            if (GetGateOrientation(obj, type).HasValue)
            {
                return new WorldMapArtSpec(1.02f, new Vector2(0.5f, 1f), Vector2.zero, true);
            }

            if (IsMidgaardNpcObject(type) || type == ObjectType.TownGuard || type == ObjectType.GateCaptain)
            {
                return new WorldMapArtSpec(exploreWideView ? 0.94f : 1.04f, new Vector2(0.5f, 1f), new Vector2(0f, 0.02f), false);
            }

            if (ExplorationArtRules.IsMidgaardBuilding(type))
            {
                return new WorldMapArtSpec(
                    ExplorationArtRules.MidgaardBuildingSpriteScale(exploreWideView),
                    new Vector2(0.5f, 1f),
                    new Vector2(0f, ExplorationArtRules.MidgaardBuildingVerticalOffset(exploreWideView)),
                    true);
            }

            switch (type)
            {
                case ObjectType.CityWall:
                    return new WorldMapArtSpec(1.00f, new Vector2(0.5f, 1f), Vector2.zero, false);
                case ObjectType.Fountain:
                case ObjectType.RecallCircle:
                    return new WorldMapArtSpec(0.86f, new Vector2(0.5f, 0.62f), Vector2.zero, false);
                case ObjectType.Sewer:
                case ObjectType.RatPeltQuest:
                    return new WorldMapArtSpec(0.84f, new Vector2(0.5f, 1f), new Vector2(0f, 0.03f), false);
                case ObjectType.Cache:
                case ObjectType.Camp:
                case ObjectType.Shrine:
                case ObjectType.Stairs:
                case ObjectType.Cave:
                case ObjectType.Ruin:
                case ObjectType.Obelisk:
                case ObjectType.Bridge:
                case ObjectType.Encounter:
                    return new WorldMapArtSpec(0.94f, new Vector2(0.5f, 1f), Vector2.zero, false);
                case ObjectType.QuestBoard:
                case ObjectType.TrainingGround:
                case ObjectType.LoreLibrary:
                case ObjectType.ForgeSite:
                case ObjectType.FactionCamp:
                case ObjectType.DungeonGate:
                case ObjectType.DeepCrypt:
                case ObjectType.AncientGrove:
                case ObjectType.PortalSeal:
                case ObjectType.Waystone:
                    return new WorldMapArtSpec(0.96f, new Vector2(0.5f, 1f), Vector2.zero, false);
                default:
                    return DefaultWorldMapArtSpec();
            }
        }

        private int MidgaardTownObjectIconIndex(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Market: return 0;
                case ObjectType.Temple: return 1;
                case ObjectType.Fountain: return 2;
                case ObjectType.Tavern: return 3;
                case ObjectType.Armorer: return 4;
                case ObjectType.Provisions: return 5;
                case ObjectType.WeaponVendor: return 6;
                case ObjectType.Enchanter: return 7;
                case ObjectType.KingHall: return 11;
                case ObjectType.Sewer: return 12;
                case ObjectType.CityWall: return 13;
                case ObjectType.Diner: return 14;
                case ObjectType.RatPeltQuest: return 15;
                case ObjectType.RecallCircle: return 17;
                default: return -1;
            }
        }

        private int MidgaardNpcObjectIconIndex(ObjectType type, MapObject obj = null)
        {
            bool eastSideGuard = obj != null && state?.Map != null && obj.X > state.Map.StartX;
            return NpcPortraitCatalog.WorldSpriteIndex(type, eastSideGuard);
        }

        private int MidgaardInteriorPropIconIndex(ObjectType type, MapObject obj)
        {
            switch (type)
            {
                case ObjectType.InteriorDoor:
                    return obj != null && obj.Id == MidgaardInteriorRules.ThroneRoomExitId ? 1 : 16;
                case ObjectType.RoyalThrone: return 0;
                case ObjectType.RoyalBanner:
                    return obj != null && obj.Id == MidgaardInteriorRules.GrandHearthWindowId ? 9 : 3;
                case ObjectType.RoyalLectern:
                    return obj != null && obj.Id == MidgaardInteriorRules.GrandHearthMapTableId ? 7 : 2;
                case ObjectType.RoyalBrazier: return 4;
                case ObjectType.ArmorDisplay: return 11;
                case ObjectType.WeaponDisplay: return 12;
                case ObjectType.EnchantmentTable: return 14;
                case ObjectType.ProvisionShelf:
                    if (obj != null && obj.Id == MidgaardInteriorRules.GrandHearthCargoId) return 19;
                    return obj != null && obj.Id == MidgaardInteriorRules.GrandHearthRoadChestId ? 17 : 15;
                case ObjectType.MerchantCounter: return 13;
                default: return -1;
            }
        }

        private int MidgaardTownObjectIconIndexFor(ObjectType type, MapObject obj)
        {
            // The exterior portal remains semantically a Tavern for established
            // quest routing, but its stable identity now presents Town Hall with
            // the approved civic-hall silhouette.
            if (obj != null
                && string.Equals(obj.Id, MidgaardInteriorRules.GrandHearthDoorId, StringComparison.Ordinal))
            {
                return MidgaardTownObjectIconIndex(ObjectType.KingHall);
            }
            return MidgaardTownObjectIconIndex(type);
        }

        private int MidgaardSewerObjectIconIndex(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Sewer: return 0;
                case ObjectType.RatPeltQuest: return 12;
                default: return -1;
            }
        }

        private int MidgaardSewerEnemySpriteIndex(string role)
        {
            switch ((role ?? "").ToLowerInvariant())
            {
                case "sewerrat": return 6;
                case "giantrat": return 7;
                case "ratfolk":
                case "ratcutthroat": return 8;
                case "ratmage": return 9;
                case "ratcleric": return 10;
                case "ratbrute": return 11;
                default: return -1;
            }
        }

        private int WorldMapLandmarkIconIndex(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Town: return 0;
                case ObjectType.Camp: return 1;
                case ObjectType.Bridge: return 2;
                case ObjectType.Cave: return 3;
                case ObjectType.Ruin: return 4;
                case ObjectType.Obelisk: return 5;
                case ObjectType.Stairs: return state != null && state.Depth >= FinalBossDepth - 1 ? 7 : 6;
                case ObjectType.Shrine: return 8;
                case ObjectType.Cache: return 9;
                case ObjectType.Encounter: return 10;
                default: return -1;
            }
        }

        private int WorldMapPropIconIndex(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Town: return 0;
                case ObjectType.Cache: return 2;
                case ObjectType.Shrine: return 10;
                case ObjectType.Encounter: return 17;
                case ObjectType.Stairs: return state != null && state.Depth >= FinalBossDepth - 1 ? 15 : 13;
                case ObjectType.Camp: return 1;
                case ObjectType.Obelisk: return 11;
                case ObjectType.Ruin: return 9;
                case ObjectType.Bridge: return 6;
                case ObjectType.Cave: return 8;
                default: return -1;
            }
        }

        private int EnemyWorldObjectIconIndex(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Cache: return 15;
                case ObjectType.Shrine: return 14;
                case ObjectType.Stairs: return state != null && state.Depth >= FinalBossDepth - 1 ? 16 : -1;
                case ObjectType.Cave: return 10;
                case ObjectType.Town: return 13;
                default: return -1;
            }
        }

        private int QuestWorldObjectIconIndex(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Town: return 2;
                case ObjectType.Cache: return 1;
                case ObjectType.Shrine: return 11;
                case ObjectType.Stairs: return state != null && state.Depth >= FinalBossDepth - 1 ? 5 : 4;
                case ObjectType.Encounter: return 0;
                case ObjectType.Camp: return 3;
                case ObjectType.Obelisk: return 12;
                case ObjectType.Ruin: return 10;
                case ObjectType.Bridge: return 8;
                case ObjectType.Cave: return 9;
                default: return -1;
            }
        }

        private int RouteScaffoldObjectIconIndex(ObjectType type)
        {
            RouteScaffoldDef def = RouteScaffoldDefs().FirstOrDefault(d => d.Type == type);
            return def == null ? -1 : def.ArtIndex;
        }

        private int DungeonScaffoldObjectIconIndex(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.DungeonGate: return 0;
                case ObjectType.DeepCrypt: return 1;
                case ObjectType.AncientGrove: return 2;
                case ObjectType.PortalSeal: return 3;
                case ObjectType.Waystone: return 4;
                default: return -1;
            }
        }

        private int ServiceScaffoldObjectIconIndex(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.QuestBoard: return 0;
                case ObjectType.TrainingGround: return 1;
                case ObjectType.LoreLibrary: return 2;
                case ObjectType.ForgeSite: return 3;
                case ObjectType.FactionCamp: return 4;
                default: return -1;
            }
        }

        private Rect WorldObjectIconCell(ObjectType type)
        {
            if (worldObjectAtlas == null) return Rect.zero;
            int index = Mathf.Clamp((int)type, 0, 9);
            return AtlasCell(worldObjectAtlas, index, 5, Mathf.Max(2, AtlasRows(worldObjectAtlas, 5)));
        }

        private bool TryDrawExploreEnvironmentTile(Rect rect, int x, int y, int tile, string kind)
        {
            if (TryDrawGrandHearthFloorTile(rect, x, y, tile)) return true;
            int interiorTileIndex = MidgaardInteriorTileAtlasIndex(x, y, tile);
            if (interiorTileIndex >= 0
                && TryDrawMidgaardInteriorTileAtlasIcon(
                    rect,
                    interiorTileIndex,
                    Color.white.WithAlpha(exploreWideView ? 0.90f : 0.98f)))
            {
                return true;
            }

            int index = EnvironmentTileAtlasIndex(tile, kind);
            if (index < 0) return false;
            int macroSize = ExploreTerrainMacroSize(x, y, tile, kind);
            float n = (ExploreNoise(x, y, 43) % 9) / 8f;
            float alpha = ExplorationReadabilityRules.TerrainArtAlpha(tile, kind, exploreWideView, n);
            bool canMirror = macroSize == 1 && ExplorationArtRules.CanMirrorTerrain(tile, kind);
            bool flipX = canMirror && (ExploreNoise(x, y, 47) & 1) != 0;
            bool flipY = false;
            Color tint = ExploreTerrainTextureTint(kind, alpha);
            int midgaardWallIndex = MidgaardWallTileAtlasIndex(x, y, tile, kind);
            if (midgaardWallIndex >= 0)
            {
                MapObject wallMarker = ObjectAt(state?.Map, x, y);
                bool openSideGate = wallMarker != null
                    && (wallMarker.Type == ObjectType.EastGate || wallMarker.Type == ObjectType.WestGate);
                if (openSideGate)
                {
                    // The side-gate object owns the two interrupted wall ends.
                    // Drawing a complete straight wall cell underneath it would
                    // seal the transparent left-right passage in the new art.
                    DrawMidgaardWallTerrainUnderlay(rect, x, y);
                    return true;
                }
                DrawMidgaardWallFoundation(rect, midgaardWallIndex, x, y);
                TryDrawMidgaardWallAtlasIcon(rect, midgaardWallIndex, tint);
                return true;
            }
            if (tile == 1)
            {
                ExplorationMaterial material = ExploreMaterialAt(x, y);
                if (TryDrawWorldMapMaterialAtlasTile(rect, material, Color.white.WithAlpha(alpha), x, y)) return true;
            }
            int midgaardTileIndex = MidgaardTileAtlasIndex(x, y, tile, kind, macroSize);
            if (midgaardTileIndex >= 0 && TryDrawMidgaardTileAtlasIcon(rect, midgaardTileIndex, tint, flipX, flipY, macroSize, x, y)) return true;
            int worldMapTileIndex = WorldMapExplorationTileIndex(x, y, tile, kind, macroSize);
            if (worldMapTileIndex >= 0 && TryDrawWorldMapExplorationTileAtlasIcon(rect, worldMapTileIndex, tint, flipX, flipY, macroSize, x, y)) return true;
            if (worldObjectAtlas == null || AtlasRows(worldObjectAtlas, 5) < 4) return false;
            DrawTextureRegionTint(worldObjectAtlas, rect, AtlasCell(worldObjectAtlas, index, 5, 4), tint);
            return true;
        }

        private int WorldMapRegionLandmarkIconIndex(ObjectType type, MapObject obj)
        {
            if (obj == null || state?.Map == null) return -1;
            WorldZone zone = ZoneFor(obj.X, obj.Y, state.Map, state.Depth);
            return WorldMapRegionLandmarkCatalog.IconIndex(zone?.Id, type);
        }

        private Color ExploreTerrainTextureTint(string kind, float alpha)
        {
            if (kind == "ash" || kind == "red") return Hex("d28678", alpha);
            if (kind == "moss" || kind == "forestwall") return Hex("d8ead4", alpha);
            if (kind == "mire" || kind == "mud" || kind == "cistern" || kind == "mirewall") return Hex("cce4df", alpha);
            return Color.white.WithAlpha(alpha);
        }

        private int ExploreTerrainMacroSize(int x, int y, int tile, string kind)
        {
            int requested = ExplorationArtRules.TerrainMacroSize(tile, kind);
            if (requested <= 1 || state?.Map == null) return 1;
            int anchorX = x - PositiveModulo(x, requested);
            int anchorY = y - PositiveModulo(y, requested);
            for (int yy = anchorY; yy < anchorY + requested; yy++)
            for (int xx = anchorX; xx < anchorX + requested; xx++)
            {
                if (xx < 0 || yy < 0 || xx >= state.Map.Width || yy >= state.Map.Height) return 1;
                if (TileAt(state.Map, xx, yy) != tile) return 1;
                if (!string.Equals(ExploreTileKind(xx, yy, tile), kind, StringComparison.Ordinal)) return 1;
            }
            return requested;
        }

        private int PositiveModulo(int value, int divisor)
        {
            int result = value % divisor;
            return result < 0 ? result + divisor : result;
        }

        private int MidgaardTileAtlasIndex(int x, int y, int tile, string kind, int macroSize)
        {
            MapObject obj = state?.Map == null ? null : ObjectAt(state.Map, x, y);
            int sampleX = macroSize > 1 ? x - PositiveModulo(x, macroSize) : x;
            int sampleY = macroSize > 1 ? y - PositiveModulo(y, macroSize) : y;
            return ExplorationArtRules.MidgaardTileIndex(
                tile,
                kind,
                ExploreNoise(sampleX, sampleY, 113),
                UsesSemanticMidgaardGround(obj));
        }

        private bool UsesSemanticMidgaardGround(MapObject obj)
        {
            if (obj == null) return false;
            switch (obj.Type)
            {
                case ObjectType.Market:
                case ObjectType.Temple:
                case ObjectType.Fountain:
                case ObjectType.Diner:
                case ObjectType.Tavern:
                case ObjectType.Armorer:
                case ObjectType.WeaponVendor:
                case ObjectType.Enchanter:
                case ObjectType.NorthGate:
                case ObjectType.SouthGate:
                case ObjectType.EastGate:
                case ObjectType.WestGate:
                case ObjectType.KingHall:
                case ObjectType.Sewer:
                case ObjectType.Provisions:
                case ObjectType.RatPeltQuest:
                case ObjectType.RecallCircle:
                    return true;
                default:
                    return false;
            }
        }

        private void PlayCombatImpactSfx(
            CombatImpactProfile profile,
            int impactX,
            int impactY,
            int reactionCount,
            string exactPowerKey = "")
        {
            if (TryPlayCombatPowerSfxPlan(profile, impactX, impactY, reactionCount, exactPowerKey)) return;

            bool reduced = state != null && state.ReducedMotion;
            CombatUnit caster = CurrentUnit();
            float impactPan = CombatAudioMixRules.StereoPanForColumn(impactX, CombatW);
            float impactPitch = CombatAudioMixRules.PitchForCue(profile.ImpactSfx, impactX);
            if (reduced)
            {
                string compactKey = !string.IsNullOrEmpty(profile.ImpactSfx) ? profile.ImpactSfx : profile.CastSfx;
                PlaySfxSpatial(compactKey, Mathf.Min(0.88f, profile.ImpactVolume), impactPan, impactPitch);
                BeginCombatMusicDuck(profile, reactionCount, 0f);
                return;
            }

            PowerCastAura stagedCast = powerCastAuras.LastOrDefault(aura =>
                aura != null
                && aura.TargetX == impactX
                && aura.TargetY == impactY
                && string.Equals(aura.Kind, profile.CastSfx, StringComparison.OrdinalIgnoreCase));
            int casterColumn = stagedCast?.SourceX ?? caster?.X ?? impactX;
            float casterPan = CombatAudioMixRules.StereoPanForColumn(casterColumn, CombatW);
            float releasePan = CombatAudioMixRules.StereoPanMidpoint(casterPan, impactPan);
            float castPitch = CombatAudioMixRules.PitchForCue(profile.CastSfx, casterColumn);
            float releasePitch = Mathf.Lerp(castPitch, impactPitch, 0.50f);

            PlaySfxSpatial(profile.CastSfx, profile.CastVolume, casterPan, castPitch);
            string casterVoice = CreatureAudioRules.CueFor(caster, "cast");
            if (!string.IsNullOrEmpty(casterVoice))
            {
                float voicePitch = CombatAudioMixRules.PitchForCue(casterVoice, casterColumn);
                PlaySfxSpatial(casterVoice, CombatAudioMixRules.AuxiliaryLayerVolume(0.24f), casterPan, Mathf.Clamp(voicePitch * 1.04f, 0.94f, 1.10f));
            }
            bool epicImpact = CombatAudioMixRules.ShouldLayerEpicImpact(profile);
            if (CombatAudioMixRules.ShouldLayerCastShimmer(profile))
            {
                PlaySfxSpatial("castshimmer", CombatAudioMixRules.AuxiliaryLayerVolume(0.28f), casterPan, Mathf.Clamp(castPitch * 1.04f, 0.94f, 1.10f));
            }
            if (CombatAudioMixRules.ShouldLayerSpellRelease(profile))
            {
                QueueSfx("spellrelease", Mathf.Max(0.035f, profile.ImpactDelay * 0.48f), CombatAudioMixRules.AuxiliaryLayerVolume(epicImpact ? 0.30f : 0.20f), releasePan, Mathf.Clamp(releasePitch * 1.03f, 0.94f, 1.10f), CombatAudioMixRules.ScheduledSfxPriorityAuxiliary);
            }
            QueueSfx(profile.ImpactSfx, profile.ImpactDelay, profile.ImpactVolume, impactPan, impactPitch, CombatAudioMixRules.ScheduledSfxPriorityPrimaryImpact);
            int secondaryBeatCount = CombatAudioMixRules.SecondaryImpactBeatCount(profile, reactionCount);
            for (int i = 0; i < secondaryBeatCount; i++)
            {
                QueueSfx(
                    CombatAudioMixRules.SecondaryImpactCue(profile, i),
                    CombatAudioMixRules.SecondaryImpactDelay(profile, i),
                    CombatAudioMixRules.SecondaryImpactVolume(profile, i),
                    CombatAudioMixRules.SecondaryImpactPan(impactPan, i),
                    CombatAudioMixRules.SecondaryImpactPitch(impactPitch, i),
                    CombatAudioMixRules.ScheduledSfxPrioritySecondaryImpact);
            }
            QueueSfx(profile.AftershockSfx, profile.AftershockDelay, profile.AftershockVolume, impactPan * 0.72f, Mathf.Clamp(impactPitch * 0.96f, 0.90f, 1.10f), CombatAudioMixRules.ScheduledSfxPrioritySupporting);
            CombatUnit impactTarget = state?.Combat?.Units?.FirstOrDefault(unit =>
                unit != null
                && unit.X == impactX
                && unit.Y == impactY
                && unit.Id != caster?.Id);
            string reactionCue = CreatureAudioRules.CueFor(impactTarget, impactTarget != null && impactTarget.Hp <= 0 ? "death" : "hurt");
            if (!string.IsNullOrEmpty(reactionCue))
            {
                QueueSfx(
                    reactionCue,
                    profile.ImpactDelay + 0.035f,
                    CombatAudioMixRules.AuxiliaryLayerVolume(impactTarget.Hp <= 0 ? 0.60f : 0.34f),
                    impactPan,
                    Mathf.Clamp(impactPitch * 1.02f, 0.94f, 1.10f),
                    CombatAudioMixRules.ScheduledSfxPriorityAuxiliary);
            }
            if (epicImpact)
            {
                QueueSfx("impactlow", profile.ImpactDelay + 0.015f, CombatAudioMixRules.AuxiliaryLayerVolume(0.38f), impactPan * 0.48f, Mathf.Clamp(impactPitch * 0.82f, 0.90f, 1.02f), CombatAudioMixRules.ScheduledSfxPrioritySupporting);
            }
            if (CombatAudioMixRules.ShouldLayerReaction(reactionCount)
                && !string.Equals(profile.AftershockSfx, "resonance", StringComparison.OrdinalIgnoreCase))
            {
                QueueSfx("resonance", profile.ImpactDelay + 0.055f, CombatAudioMixRules.AuxiliaryLayerVolume(0.40f + Mathf.Min(2, reactionCount) * 0.06f), impactPan * 0.62f, Mathf.Clamp(impactPitch * 1.02f, 0.94f, 1.10f), CombatAudioMixRules.ScheduledSfxPrioritySupporting);
            }
            BeginCombatMusicDuck(profile, reactionCount, profile.ImpactDelay);
        }

        private bool TryPlayCombatPowerSfxPlan(
            CombatImpactProfile impactProfile,
            int impactX,
            int impactY,
            int reactionCount,
            string exactPowerKey)
        {
            if (!TryResolveCombatPowerSfxPlan(impactProfile, exactPowerKey, reactionCount, out CombatPowerSfxPlan plan)) return false;

            CombatUnit caster = CurrentUnit();
            float impactPan = CombatAudioMixRules.StereoPanForColumn(impactX, CombatW);
            int sampleIndex = (impactX + 1) * 31 + (impactY + 1) * 17 + Mathf.Max(0, reactionCount) * 13;
            float impactPitch = CombatPowerSfxRules.StablePitch(plan.Impact, plan.ProfileKey, sampleIndex, 3);
            if (plan.ReducedAudio)
            {
                ScheduleCombatPowerSfxCue(
                    plan.Impact,
                    plan.ProfileKey,
                    sampleIndex,
                    3,
                    impactPan,
                    CombatAudioMixRules.ScheduledSfxPriorityPrimaryImpact);
                BeginCombatMusicDuck(impactProfile, reactionCount, 0f);
                return true;
            }

            PowerCastAura stagedCast = powerCastAuras.LastOrDefault(aura =>
                aura != null
                && aura.TargetX == impactX
                && aura.TargetY == impactY
                && string.Equals(aura.PowerKey, plan.ProfileKey, StringComparison.OrdinalIgnoreCase));
            int casterColumn = stagedCast?.SourceX ?? caster?.X ?? impactX;
            float casterPan = CombatAudioMixRules.StereoPanForColumn(casterColumn, CombatW);
            float releasePan = CombatAudioMixRules.StereoPanMidpoint(casterPan, impactPan);

            ScheduleCombatPowerSfxCue(
                plan.Cast,
                plan.ProfileKey,
                sampleIndex,
                1,
                casterPan,
                CombatAudioMixRules.ScheduledSfxPrioritySupporting);
            string casterVoice = CreatureAudioRules.CueFor(caster, "cast");
            if (!string.IsNullOrEmpty(casterVoice))
            {
                float voicePitch = CombatAudioMixRules.PitchForCue(casterVoice, casterColumn);
                PlaySfxSpatial(
                    casterVoice,
                    CombatAudioMixRules.AuxiliaryLayerVolume(0.24f),
                    casterPan,
                    Mathf.Clamp(voicePitch * 1.04f, 0.94f, 1.10f));
            }

            ScheduleCombatPowerSfxCue(
                plan.Shimmer,
                plan.ProfileKey,
                sampleIndex,
                7,
                casterPan,
                CombatAudioMixRules.ScheduledSfxPriorityAuxiliary);
            ScheduleCombatPowerSfxCue(
                plan.Release,
                plan.ProfileKey,
                sampleIndex,
                2,
                releasePan,
                CombatAudioMixRules.ScheduledSfxPriorityAuxiliary);
            ScheduleCombatPowerSfxCue(
                plan.Impact,
                plan.ProfileKey,
                sampleIndex,
                3,
                impactPan,
                CombatAudioMixRules.ScheduledSfxPriorityPrimaryImpact);
            ScheduleCombatPowerSfxCue(
                plan.Aftershock,
                plan.ProfileKey,
                sampleIndex,
                4,
                impactPan * 0.72f,
                CombatAudioMixRules.ScheduledSfxPrioritySupporting);
            ScheduleCombatPowerSfxCue(
                plan.LowHit,
                plan.ProfileKey,
                sampleIndex,
                5,
                impactPan * 0.48f,
                CombatAudioMixRules.ScheduledSfxPrioritySupporting);
            ScheduleCombatPowerSfxCue(
                plan.Rumble,
                plan.ProfileKey,
                sampleIndex,
                6,
                impactPan * 0.36f,
                CombatAudioMixRules.ScheduledSfxPrioritySupporting);

            CombatUnit impactTarget = state?.Combat?.Units?.FirstOrDefault(unit =>
                unit != null
                && unit.X == impactX
                && unit.Y == impactY
                && unit.Id != caster?.Id);
            bool beneficialImpact = string.Equals(plan.Impact.Key, "heal", StringComparison.OrdinalIgnoreCase)
                || string.Equals(plan.Impact.Key, "ward", StringComparison.OrdinalIgnoreCase)
                || string.Equals(plan.Impact.Key, "guard", StringComparison.OrdinalIgnoreCase)
                || string.Equals(plan.Impact.Key, "fieldholy", StringComparison.OrdinalIgnoreCase)
                || CombatPowerSfxProfileTargetsBeneficiary(plan.ProfileKey);
            string reactionCue = beneficialImpact
                ? ""
                : CreatureAudioRules.CueFor(impactTarget, impactTarget != null && impactTarget.Hp <= 0 ? "death" : "hurt");
            if (!string.IsNullOrEmpty(reactionCue))
            {
                QueueSfx(
                    reactionCue,
                    plan.Impact.Delay + 0.035f,
                    CombatAudioMixRules.AuxiliaryLayerVolume(impactTarget.Hp <= 0 ? 0.60f : 0.34f),
                    impactPan,
                    Mathf.Clamp(impactPitch * 1.02f, 0.94f, 1.10f),
                    CombatAudioMixRules.ScheduledSfxPriorityAuxiliary);
            }
            if (CombatAudioMixRules.ShouldLayerReaction(reactionCount)
                && !string.Equals(plan.Aftershock.Key, "resonance", StringComparison.OrdinalIgnoreCase))
            {
                QueueSfx(
                    "resonance",
                    plan.Impact.Delay + 0.055f,
                    CombatAudioMixRules.AuxiliaryLayerVolume(0.40f + Mathf.Min(2, reactionCount) * 0.06f),
                    impactPan * 0.62f,
                    Mathf.Clamp(impactPitch * 1.02f, 0.94f, 1.10f),
                    CombatAudioMixRules.ScheduledSfxPrioritySupporting);
            }
            BeginCombatMusicDuck(impactProfile, reactionCount, plan.Impact.Delay);
            return true;
        }

        private bool CombatPowerSfxProfileTargetsBeneficiary(string profileKey)
        {
            switch ((profileKey ?? "").Trim().ToLowerInvariant())
            {
                case "oic":
                case "nvc":
                case "tbq":
                case "sgw":
                case "tnc":
                case "lbc":
                case "tbg":
                case "nvl":
                case "swr":
                case "dwp":
                case "slv":
                case "rally":
                    return true;
                default:
                    return false;
            }
        }

        private bool TryResolveCombatPowerSfxPlan(
            CombatImpactProfile impactProfile,
            string exactPowerKey,
            int reactionCount,
            out CombatPowerSfxPlan plan)
        {
            plan = default;
            if (string.IsNullOrWhiteSpace(exactPowerKey)) return false;

            int intensity = CombatImpactRules.VisualIntensity(impactProfile, reactionCount);
            bool reducedAudio = state != null && state.ReducedMotion;
            if (CombatPowerSfxRules.IsSupportedFormula(exactPowerKey))
            {
                plan = CombatPowerSfxRules.PlanForFormula(exactPowerKey, intensity, reducedAudio);
                return true;
            }
            if (CombatPowerSfxRules.IsSupportedAbility(exactPowerKey))
            {
                plan = CombatPowerSfxRules.PlanForAbility(exactPowerKey, intensity, reducedAudio);
                return true;
            }
            return false;
        }

        private float ResolvedCombatPowerSfxImpactDelay(CombatImpactProfile profile, string exactPowerKey)
        {
            return TryResolveCombatPowerSfxPlan(profile, exactPowerKey, 0, out CombatPowerSfxPlan plan)
                && plan.Impact.Enabled
                ? plan.Impact.Delay
                : profile.ImpactDelay;
        }

        private void ScheduleCombatPowerSfxCue(
            CombatPowerSfxCuePlan cue,
            string profileKey,
            int sampleIndex,
            int channel,
            float pan,
            int priority)
        {
            if (!cue.Enabled) return;
            QueueSfx(
                cue.Key,
                cue.Delay,
                cue.Gain,
                pan,
                CombatPowerSfxRules.StablePitch(cue, profileKey, sampleIndex, channel),
                priority);
        }

        private void PlayCombatMissSfx(CombatImpactProfile profile, int targetX)
        {
            if (string.IsNullOrEmpty(profile.CastSfx)) return;
            CombatUnit caster = CurrentUnit();
            int sourceX = caster?.X ?? targetX;
            float pan = CombatAudioMixRules.StereoPanForColumn(sourceX, CombatW);
            float pitch = CombatAudioMixRules.PitchForCue(profile.CastSfx, sourceX);
            PlaySfxSpatial(
                profile.CastSfx,
                Mathf.Min(0.78f, profile.CastVolume),
                pan,
                Mathf.Lerp(1f, pitch, 0.45f));
        }

        private void PlayWeaponAttackSequence(CombatUnit attacker, CombatUnit target, string damageType, bool critical, bool hit, bool ranged)
        {
            int sourceX = attacker?.X ?? target?.X ?? 0;
            int targetX = target == null ? attacker?.X ?? 0 : target.X;
            float sourcePan = CombatAudioMixRules.StereoPanForColumn(sourceX, CombatW);
            float impactPan = CombatAudioMixRules.StereoPanForColumn(targetX, CombatW);
            WeaponFeedbackProfile feedback = WeaponFeedbackRules.For(attacker?.WeaponName, ranged);

            float releasePitch = CombatAudioMixRules.PitchForCue(feedback.ReleaseCue, sourceX);
            float impactPitch = CombatAudioMixRules.PitchForCue(feedback.ContactCue, targetX);
            string attackerVoice = CreatureAudioRules.CueFor(attacker, "attack");
            if (!string.IsNullOrEmpty(attackerVoice))
            {
                PlaySfxSpatial(attackerVoice, 0.26f, sourcePan, Mathf.Clamp(releasePitch * 1.05f, 0.96f, 1.10f));
            }
            PlaySfxSpatial(feedback.ReleaseCue, feedback.ReleaseVolume, sourcePan, releasePitch);
            if (hit)
            {
                QueueSfx(
                    feedback.ContactCue,
                    feedback.ImpactDelay,
                    WeaponFeedbackRules.ContactVolume(feedback, critical, target != null && target.Guarding),
                    impactPan * 0.82f,
                    Mathf.Clamp(impactPitch * (critical ? 0.95f : 1.02f), 0.90f, 1.10f));
                QueueSfx(
                    WeaponImpactSfx(attacker, target, damageType),
                    feedback.ImpactDelay + 0.012f,
                    critical ? 0.82f : 0.62f,
                    impactPan,
                    Mathf.Clamp(impactPitch * (critical ? 0.96f : 1f), 0.90f, 1.10f),
                    CombatAudioMixRules.ScheduledSfxPriorityPrimaryImpact);
            }
            else
            {
                QueueSfx("miss", feedback.ImpactDelay, 0.62f, impactPan, impactPitch, CombatAudioMixRules.ScheduledSfxPriorityPrimaryImpact);
            }
            if (hit)
            {
                string reaction = CreatureAudioRules.CueFor(target, target.Hp <= 0 ? "death" : "hurt");
                if (!string.IsNullOrEmpty(reaction))
                {
                    QueueSfx(reaction, feedback.ImpactDelay + 0.032f, target.Hp <= 0 ? 0.66f : 0.40f, impactPan, Mathf.Clamp(impactPitch * 1.03f, 0.94f, 1.10f), CombatAudioMixRules.ScheduledSfxPriorityAuxiliary);
                }
            }
            if (hit && critical)
            {
                QueueSfx(
                    CombatAudioMixRules.CriticalCue,
                    feedback.ImpactDelay + 0.022f,
                    0.78f,
                    impactPan,
                    CombatAudioMixRules.PitchForCue(CombatAudioMixRules.CriticalCue, targetX),
                    CombatAudioMixRules.ScheduledSfxPrioritySecondaryImpact);
                QueueSfx("impactlow", feedback.ImpactDelay + 0.030f, 0.26f, impactPan * 0.48f, 0.92f, CombatAudioMixRules.ScheduledSfxPrioritySupporting);
            }
        }

        private void PlayCoverAttackSequence(CombatUnit attacker, Point cover, bool ranged, bool broken, bool arcing = false)
        {
            if (attacker == null || cover == null) return;
            WeaponFeedbackProfile feedback = WeaponFeedbackRules.For(attacker.WeaponName, ranged);
            float sourcePan = CombatAudioMixRules.StereoPanForColumn(attacker.X, CombatW);
            float impactPan = CombatAudioMixRules.StereoPanForColumn(cover.X, CombatW);
            string releaseCue = arcing ? CoverArcReleaseCue(attacker.DamageType) : feedback.ReleaseCue;
            float impactDelay = arcing ? 0.09f : feedback.ImpactDelay;
            float releasePitch = CombatAudioMixRules.PitchForCue(releaseCue, attacker.X);
            float impactPitch = CombatAudioMixRules.PitchForCue(feedback.ContactCue, cover.X);
            PlaySfxSpatial(releaseCue, arcing ? 0.46f : feedback.ReleaseVolume, sourcePan, releasePitch);
            if (!arcing)
            {
                QueueSfx(
                    feedback.ContactCue,
                    impactDelay,
                    WeaponFeedbackRules.ContactVolume(feedback, broken, false) * 0.72f,
                    impactPan * 0.78f,
                    Mathf.Clamp(impactPitch * (broken ? 0.95f : 1.02f), 0.90f, 1.10f));
            }
            QueueSfx(
                WeaponFeedbackRules.CoverContactCue(cover.Kind),
                impactDelay + 0.014f,
                WeaponFeedbackRules.CoverContactVolume(feedback, broken),
                impactPan,
                Mathf.Clamp(impactPitch * (cover.Kind == "tree" ? 1.03f : 0.92f), 0.88f, 1.10f),
                CombatAudioMixRules.ScheduledSfxPriorityPrimaryImpact);
            if (broken)
            {
                QueueSfx("breakcover", impactDelay + 0.050f, 0.76f, impactPan * 0.88f, cover.Kind == "tree" ? 1.04f : 0.91f);
            }
        }

        private string CoverArcReleaseCue(string damageType)
        {
            switch ((damageType ?? "").ToLowerInvariant())
            {
                case "fire": return "castember";
                case "cold": return "castfrost";
                case "shock": return "castshock";
                case "poison": return "castnature";
                case "death":
                case "mind": return "casthex";
                default: return "spellrelease";
            }
        }

        private string WeaponImpactSfx(CombatUnit attacker, CombatUnit target, string damageType)
        {
            if (damageType == "fire") return "fire";
            if (damageType == "cold") return "ice";
            if (damageType == "shock") return "shock";
            if (damageType == "poison") return "poison";
            if (damageType == "death" || damageType == "mind") return "death";
            if (target != null && target.Guarding) return "impactshield";

            string armor = (target?.ArmorName ?? "").ToLowerInvariant();
            if (armor.Contains("plate") || armor.Contains("cuirass") || armor.Contains("brigandine")) return "impactplate";
            if (armor.Contains("chain") || armor.Contains("mail") || armor.Contains("hauberk") || armor.Contains("scale")) return "impactmail";
            if (armor.Contains("leather") || armor.Contains("hide") || armor.Contains("jerkin")) return "impactleather";
            return "impactflesh";
        }

        private void BeginCombatMusicDuck(CombatImpactProfile profile, int reactionCount, float impactDelay)
        {
            float depth = CombatAudioMixRules.MusicDuckDepth(profile, reactionCount);
            float attackDuration = CombatAudioMixRules.MusicDuckAttackDuration(profile, reactionCount);
            float holdDuration = CombatAudioMixRules.MusicDuckHoldDuration(profile, reactionCount);
            float releaseDuration = CombatAudioMixRules.MusicDuckReleaseDuration(profile, reactionCount);
            if (depth <= 0f || releaseDuration <= 0f) return;

            float now = Time.time;
            if (!CombatAudioMixRules.ShouldReplaceActiveMusicDuck(
                    now,
                    combatMusicDuckUntil,
                    combatMusicDuckDepth,
                    depth))
            {
                return;
            }
            float safeImpactDelay = Mathf.Clamp(impactDelay, 0f, 0.60f);
            combatMusicDuckFullDepthAt = now + safeImpactDelay;
            combatMusicDuckStartedAt = combatMusicDuckFullDepthAt - Mathf.Min(safeImpactDelay, attackDuration);
            combatMusicDuckHoldUntil = combatMusicDuckFullDepthAt + holdDuration;
            combatMusicDuckUntil = combatMusicDuckHoldUntil + releaseDuration;
            combatMusicDuckDepth = depth;
            ApplyAudioSettings();
        }

        private void ClearCombatAudioForReducedMotion()
        {
            scheduledSfx.Clear();
            combatMusicDuckStartedAt = -1f;
            combatMusicDuckFullDepthAt = -1f;
            combatMusicDuckHoldUntil = -1f;
            combatMusicDuckUntil = -1f;
            combatMusicDuckDepth = 0f;
            lastCombatForegroundSfxAt = Time.time;
            nextCombatAmbienceAt = Mathf.Max(
                nextCombatAmbienceAt,
                lastCombatForegroundSfxAt + CombatAudioMixRules.CombatAmbienceForegroundQuietWindow);
            ApplyAudioSettings();
        }

        private void QueueSfx(
            string key,
            float delay,
            float volume,
            float pan = 0f,
            float pitch = 1f,
            int priority = CombatAudioMixRules.ScheduledSfxPrioritySupporting)
        {
            if (string.IsNullOrEmpty(key) || !soundClips.ContainsKey(key)) return;
            if (delay <= 0.005f)
            {
                PlaySfxSpatial(key, volume, pan, pitch);
                return;
            }

            ScheduledSfxCue incoming = new ScheduledSfxCue
            {
                Key = key,
                Volume = Mathf.Clamp(volume, 0f, 1.4f),
                PlayAt = Time.time + Mathf.Clamp(delay, 0.01f, 0.60f),
                Pan = Mathf.Clamp(pan, -0.85f, 0.85f),
                Pitch = Mathf.Clamp(pitch, 0.90f, 1.10f),
                Priority = Mathf.Clamp(priority, CombatAudioMixRules.ScheduledSfxPriorityAuxiliary, CombatAudioMixRules.ScheduledSfxPriorityPrimaryImpact),
                Serial = scheduledSfxSerial++
            };

            for (int i = 0; i < scheduledSfx.Count; i++)
            {
                ScheduledSfxCue existing = scheduledSfx[i];
                if (!CombatAudioMixRules.ShouldCoalesceScheduledCue(
                    existing.Key,
                    existing.PlayAt,
                    existing.Pan,
                    existing.Priority,
                    incoming.Key,
                    incoming.PlayAt,
                    incoming.Pan,
                    incoming.Priority))
                {
                    continue;
                }

                existing.Volume = Mathf.Max(existing.Volume, incoming.Volume);
                existing.PlayAt = Mathf.Min(existing.PlayAt, incoming.PlayAt);
                existing.Pan = Mathf.Lerp(existing.Pan, incoming.Pan, 0.5f);
                existing.Pitch = Mathf.Lerp(existing.Pitch, incoming.Pitch, 0.5f);
                existing.Priority = Mathf.Max(existing.Priority, incoming.Priority);
                scheduledSfx[i] = existing;
                return;
            }

            if (scheduledSfx.Count >= CombatAudioMixRules.ScheduledSfxCapacity)
            {
                int replaceIndex = ScheduledSfxReplacementIndex(incoming);
                if (replaceIndex < 0) return;
                scheduledSfx.RemoveAt(replaceIndex);
            }
            scheduledSfx.Add(incoming);
        }

        private int ScheduledSfxReplacementIndex(ScheduledSfxCue incoming)
        {
            int replaceIndex = -1;
            for (int i = 0; i < scheduledSfx.Count; i++)
            {
                ScheduledSfxCue candidate = scheduledSfx[i];
                if (candidate.Priority >= incoming.Priority) continue;
                if (replaceIndex < 0
                    || candidate.Priority < scheduledSfx[replaceIndex].Priority
                    || candidate.Priority == scheduledSfx[replaceIndex].Priority
                        && candidate.PlayAt > scheduledSfx[replaceIndex].PlayAt)
                {
                    replaceIndex = i;
                }
            }
            if (replaceIndex >= 0) return replaceIndex;
            if (incoming.Priority < CombatAudioMixRules.ScheduledSfxPriorityPrimaryImpact) return -1;

            // A fully saturated primary-only queue stays bounded. Prefer the cue that
            // lands first so a just-scheduled impact cannot be lost behind a later one.
            int latestIndex = -1;
            for (int i = 0; i < scheduledSfx.Count; i++)
            {
                if (scheduledSfx[i].Priority != incoming.Priority) continue;
                if (latestIndex < 0 || scheduledSfx[i].PlayAt > scheduledSfx[latestIndex].PlayAt) latestIndex = i;
            }
            return latestIndex >= 0 && incoming.PlayAt < scheduledSfx[latestIndex].PlayAt ? latestIndex : -1;
        }

        private void UpdateScheduledSfx()
        {
            if (scheduledSfx.Count == 0) return;
            if (state == null || state.Mode != GameMode.Combat)
            {
                scheduledSfx.Clear();
                return;
            }

            float now = Time.time;
            List<ScheduledSfxCue> due = null;
            for (int i = scheduledSfx.Count - 1; i >= 0; i--)
            {
                ScheduledSfxCue cue = scheduledSfx[i];
                if (now < cue.PlayAt) continue;
                scheduledSfx.RemoveAt(i);
                if (due == null) due = new List<ScheduledSfxCue>();
                due.Add(cue);
            }
            if (due == null) return;
            due.Sort((left, right) =>
            {
                int priorityOrder = left.Priority.CompareTo(right.Priority);
                if (priorityOrder != 0) return priorityOrder;
                int timeOrder = left.PlayAt.CompareTo(right.PlayAt);
                return timeOrder != 0 ? timeOrder : left.Serial.CompareTo(right.Serial);
            });
            foreach (ScheduledSfxCue cue in due)
            {
                // Supporting layers play first; primary impacts are the final voices
                // requested in a saturated frame and therefore survive voice stealing.
                PlaySfxSpatial(cue.Key, cue.Volume, cue.Pan, cue.Pitch);
            }
        }

        private int MidgaardWallTileAtlasIndex(int x, int y, int tile, string kind)
        {
            if (tile != 0 || kind != "midgaardwall") return -1;
            MapObject marker = ObjectAt(state?.Map, x, y);
            bool structuralAccent = marker != null && marker.Type == ObjectType.CityWall;
            return MidgaardWallAtlasIndexForCoordinate(x, y, structuralAccent);
        }

        private int MidgaardWallObjectAtlasIndex(MapObject obj)
        {
            if (obj == null) return -1;
            if (obj.Type != ObjectType.CityWall) return -1;
            return MidgaardWallAtlasIndexForCoordinate(obj.X, obj.Y, true);
        }

        private GateOrientation? GetGateOrientation(MapObject obj, ObjectType fallbackType)
        {
            if (state?.Map == null || state.Depth != 1) return null;
            ObjectType type = obj != null ? obj.Type : fallbackType;
            switch (type)
            {
                case ObjectType.NorthGate: return GateOrientation.North;
                case ObjectType.SouthGate: return GateOrientation.South;
                case ObjectType.EastGate: return GateOrientation.East;
                case ObjectType.WestGate: return GateOrientation.West;
                default: return null;
            }
        }

        private int GateAtlasIndex(GateOrientation orientation)
        {
            switch (orientation)
            {
                case GateOrientation.North: return 0;
                case GateOrientation.South: return 0;
                case GateOrientation.East: return 7;
                case GateOrientation.West: return 6;
                default: return -1;
            }
        }

        private int GateTownFallbackAtlasIndex(GateOrientation orientation)
        {
            switch (orientation)
            {
                case GateOrientation.North: return 8;
                case GateOrientation.South: return 9;
                case GateOrientation.East:
                case GateOrientation.West:
                    return -1;
                default: return -1;
            }
        }

        private int MidgaardWallAtlasIndexForCoordinate(int x, int y, bool objectArt)
        {
            if (state?.Map == null || state.Depth != 1) return -1;
            int left = MidgaardLeft(state.Map);
            int right = MidgaardRight(state.Map);
            int top = MidgaardTop(state.Map);
            int bottom = MidgaardBottom(state.Map);
            // Cells 4 and 5 are authored as top-right and top-left respectively.
            // The older mapping crossed them, leaving both top joins pointing out
            // of town instead of into the perimeter.
            if (x == left && y == top) return 5;
            if (x == right && y == top) return 4;
            if (x == left && y == bottom) return 6;
            if (x == right && y == bottom) return 7;
            if (y == top) return objectArt && PositiveModulo(x - left - 6, 8) == 0 ? 8 : 0;
            if (y == bottom) return objectArt && PositiveModulo(x - left - 6, 8) == 0 ? 8 : 1;
            if (x == left) return objectArt && PositiveModulo(y - top - 6, 8) == 0 ? 9 : 2;
            if (x == right) return objectArt && PositiveModulo(y - top - 6, 8) == 0 ? 9 : 3;
            return -1;
        }

        private int WorldMapExplorationTileIndex(int x, int y, int tile, string kind, int macroSize)
        {
            kind = kind ?? "";
            int sampleX = macroSize > 1 ? x - PositiveModulo(x, macroSize) : x;
            int sampleY = macroSize > 1 ? y - PositiveModulo(y, macroSize) : y;
            int roll = ExploreNoise(sampleX, sampleY, 131) % 100;
            if (tile == 0)
            {
                if (kind == "forestwall") return ForestWallExplorationTileIndex(x, y, roll);
            }
            int pathMask = kind == "road" ? ExplorationSurfaceRules.PathNeighborMask(state?.Map, x, y) : 0;
            bool quietCell = HasStaticExploreObjectFootprint(x, y);
            return ExplorationArtRules.WorldMapTileIndex(tile, kind, roll, pathMask, quietCell);
        }

        private bool HasStaticExploreObjectFootprint(int x, int y)
        {
            MapData map = state?.Map;
            if (map == null) return false;
            // Distance <= 1 is the center plus four cardinal neighbors. Use the
            // map's coordinate lookup rather than allocating a capturing LINQ
            // predicate and scanning every authored object for every drawn band.
            return ObjectAt(map, x, y) != null
                || ObjectAt(map, x, y - 1) != null
                || ObjectAt(map, x + 1, y) != null
                || ObjectAt(map, x, y + 1) != null
                || ObjectAt(map, x - 1, y) != null;
        }

        private int CountRoadExploreNeighbors(int x, int y)
        {
            if (state?.Map == null) return 0;
            int count = 0;
            if (IsRoadExploreTile(x, y - 1)) count++;
            if (IsRoadExploreTile(x - 1, y)) count++;
            if (IsRoadExploreTile(x + 1, y)) count++;
            if (IsRoadExploreTile(x, y + 1)) count++;
            return count;
        }

        private bool IsRoadExploreTile(int x, int y)
        {
            if (state?.Map == null || x < 0 || y < 0 || x >= state.Map.Width || y >= state.Map.Height) return false;
            int tile = TileAt(state.Map, x, y);
            return tile == 1 && ExploreTileKind(x, y, tile) == "road";
        }

        private int ForestWallExplorationTileIndex(int x, int y, int roll)
        {
            // The first bank keeps the repaired semantic contract; v1.68 appends
            // five forest-wall alternatives in cells 20-24. Selection remains a
            // pure coordinate-derived rule, so saves and replays render identically.
            return ExplorationArtRules.WorldMapTileIndex(0, "forestwall", roll, 0, false);
        }

        private bool IsForestWallAt(int x, int y)
        {
            if (state?.Map == null) return false;
            if (x < 0 || y < 0 || x >= state.Map.Width || y >= state.Map.Height) return false;
            return TileAt(state.Map, x, y) == 0 && ExploreTileKind(x, y, 0) == "forestwall";
        }

        private int EnvironmentTileAtlasIndex(int tile, string kind)
        {
            kind = kind ?? "";
            if (tile == 0)
            {
                if (kind == "mirewall") return 16;
                if (kind == "forestwall") return 15;
                if (kind == "cliffwall") return 14;
                if (kind == "redwall") return 19;
                return 14;
            }

            if (kind == "road") return 11;
            if (kind == "paved") return 12;
            if (kind == "moss") return 17;
            if (kind == "mire" || kind == "mud") return 10;
            if (kind == "quarry") return 13;
            if (kind == "glass") return 18;
            if (kind == "ash") return 19;
            return 12;
        }

        private void DrawItemIcon(Rect rect, InventoryItem item)
        {
            string name = item?.DisplayName ?? "";
            string slot = item?.Slot ?? "";
            string type = item?.DamageType ?? "";
            DrawGearIcon(rect, name, slot, type);
        }

        private void DrawGearIcon(Rect rect, string name, string slot, string damageType)
        {
            Color accent = slot == "armor" ? teal : DamageColor(string.IsNullOrEmpty(damageType) ? "physical" : damageType);
            DrawRect(rect, Hex("050708", 0.86f));
            DrawBorder(rect, accent.WithAlpha(0.85f), 1);
            int uniqueIndex = UniqueItemIconIndex(name);
            if (uniqueIndex >= 0 && TryDrawUniqueItemAtlasIcon(Pad(rect, rect.width * 0.06f), uniqueIndex, Color.white))
            {
                return;
            }
            int consumableIndex = InventoryConsumableIconIndex(name, slot);
            if (consumableIndex >= 0 && TryDrawInventoryConsumableAtlasIcon(Pad(rect, rect.width * 0.06f), consumableIndex, Color.white))
            {
                return;
            }
            int index = ItemIconIndex(name, slot);
            if (index >= 0 && TryDrawItemAtlasCell(Pad(rect, rect.width * 0.06f), index, Color.white))
            {
                return;
            }

            Rect inner = Pad(rect, rect.width * 0.24f);
            if (slot == "armor")
            {
                DrawBorder(inner, accent, 1);
                DrawRect(new Rect(inner.x + inner.width * 0.24f, inner.y + inner.height * 0.18f, inner.width * 0.52f, inner.height * 0.64f), accent.WithAlpha(0.50f));
            }
            else
            {
                DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.66f, inner.width * 0.62f, inner.height * 0.10f), accent);
                DrawRect(new Rect(inner.x + inner.width * 0.58f, inner.y + inner.height * 0.14f, inner.width * 0.12f, inner.height * 0.58f), cursorWhite.WithAlpha(0.85f));
            }
        }

        private bool TryDrawItemAtlasCell(Rect rect, int index, Color tint)
        {
            if (itemIconAtlas == null || index < 0) return false;
            return DrawTextureRegionTint(itemIconAtlas, rect, AtlasCell(itemIconAtlas, index, 5, 4), tint);
        }

        private bool TryDrawUniqueItemAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsUniqueItemAtlas() || index < 0) return false;
            return DrawTextureRegionTint(uniqueItemAtlas, rect, AtlasCell(uniqueItemAtlas, index, 5, 4), tint);
        }

        private bool IsUniqueItemAtlas()
        {
            return uniqueItemAtlas != null && uniqueItemAtlas.width >= 768 && uniqueItemAtlas.height >= 600;
        }

        private int UniqueItemIconIndex(string name)
        {
            string text = (name ?? "").ToLowerInvariant();
            if (text.Contains("unfathomable darkness")) return 0;
            return -1;
        }

        private int ItemIconIndex(string name, string slot)
        {
            if (itemIconAtlasUsesInventoryContract) return InventoryItemIconIndex(name, slot);
            return LegacyEquipmentItemIconIndex(name, slot);
        }

        private int InventoryItemIconIndex(string name, string slot)
        {
            string text = (name ?? "").ToLowerInvariant();
            if (text.Contains("epee") || text.Contains("rapier")) return 1;
            if (text.Contains("dagger") || text.Contains("knife")) return 2;
            if (text.Contains("axe") || text.Contains("hatchet")) return 3;
            if (text.Contains("spear") || text.Contains("pike") || text.Contains("lance") || text.Contains("halberd")) return 4;
            if (text.Contains("crossbow") || text.Contains("sling")) return 6;
            if (text.Contains("bow")) return 5;
            if (text.Contains("staff") || text.Contains("wand") || text.Contains("focus") || text.Contains("crystal")) return 7;
            if (text.Contains("orb")) return 8;
            if (text.Contains("shield") || text.Contains("buckler")) return 9;
            if (text.Contains("leather") || text.Contains("hide") || text.Contains("pelt")) return 10;
            if (text.Contains("chain") || text.Contains("mail")) return 11;
            if (text.Contains("plate") || text.Contains("adamant") || text.Contains("mithril") || text.Contains("helm")) return 12;
            if (text.Contains("robe") || text.Contains("cloak") || text.Contains("mantle")) return 13;
            if (text.Contains("potion")) return 14;
            if (text.Contains("elixir") || text.Contains("mana")) return 15;
            if (text.Contains("ration") || text.Contains("supply") || text.Contains("bread")) return 16;
            if (text.Contains("scroll")) return 17;
            if (text.Contains("coin") || text.Contains("gold")) return 18;
            if (text.Contains("ring") || text.Contains("gem") || text.Contains("jewel")) return 19;
            if (slot == "armor") return 12;
            if (slot == "weapon") return 0;
            return -1;
        }

        private int LegacyEquipmentItemIconIndex(string name, string slot)
        {
            string text = (name ?? "").ToLowerInvariant();
            if (text.Contains("axe") || text.Contains("hatchet")) return 1;
            if (text.Contains("dagger") || text.Contains("knife") || text.Contains("epee") || text.Contains("rapier")) return 2;
            if (text.Contains("spear") || text.Contains("pike") || text.Contains("lance") || text.Contains("halberd")) return 3;
            if (text.Contains("bow") || text.Contains("crossbow") || text.Contains("sling")) return 4;
            if (text.Contains("staff") || text.Contains("wand")) return 5;
            if (text.Contains("orb") || text.Contains("focus") || text.Contains("crystal")) return 6;
            if (text.Contains("buckler") || text.Contains("round shield")) return 7;
            if (text.Contains("shield")) return 8;
            if (text.Contains("leather") || text.Contains("hide") || text.Contains("pelt")) return 9;
            if (text.Contains("chain") || text.Contains("mail")) return 10;
            if (text.Contains("plate") || text.Contains("adamant") || text.Contains("mithril")) return 11;
            if (text.Contains("robe") || text.Contains("cloak") || text.Contains("mantle")) return 12;
            if (text.Contains("potion")) return 13;
            if (text.Contains("elixir")) return 14;
            if (text.Contains("coin") || text.Contains("gold")) return 15;
            if (text.Contains("scroll")) return 16;
            if (text.Contains("ring")) return 17;
            if (text.Contains("boot") || text.Contains("greave")) return 18;
            if (text.Contains("helm")) return 19;
            if (slot == "armor") return 11;
            if (slot == "weapon") return 0;
            return -1;
        }

        private int InventoryConsumableIconIndex(string name, string slot)
        {
            string text = ((name ?? "") + " " + (slot ?? "")).ToLowerInvariant();
            if (text.Contains("ration") || text.Contains("supply")) return 0;
            if (text.Contains("bread")) return 1;
            if (text.Contains("cheese")) return 2;
            if (text.Contains("meat") || text.Contains("roast")) return 3;
            if (text.Contains("berry") || text.Contains("berries")) return 4;
            if (text.Contains("water")) return 5;
            if (text.Contains("healing") || text.Contains("red potion")) return 6;
            if (text.Contains("mana") || text.Contains("blue potion")) return 7;
            if (text.Contains("elixir")) return 8;
            if (text.Contains("antidote") || text.Contains("amber")) return 9;
            if (text.Contains("gold") || text.Contains("coin")) return 10;
            if (text.Contains("gem") || text.Contains("jewel")) return 11;
            if (text.Contains("scroll")) return 12;
            if (text.Contains("key")) return 13;
            if (text.Contains("torch")) return 14;
            if (text.Contains("pack") || text.Contains("satchel")) return 15;
            if (text.Contains("cache") || text.Contains("chest")) return 16;
            if (text.Contains("herb")) return 17;
            if (text.Contains("mushroom") || text.Contains("fungus")) return 18;
            if (text.Contains("crate")) return 19;
            return -1;
        }

        private int ResourceConsumableIconIndex(string label)
        {
            switch ((label ?? "").ToLowerInvariant())
            {
                case "gold": return 10;
                case "supplies": return 0;
                case "elixirs": return 8;
                default: return -1;
            }
        }

        private Rect AtlasCombatRegion(CombatUnit unit)
        {
            if (unit == null) return Rect.zero;
            if (unit.Side == UnitSide.Party) return AtlasPartyFigureRegion(unit.Role);
            return AtlasEnemyRegion(unit.Role);
        }

        private Rect AtlasPartyFigureRegion(string role)
        {
            switch (role)
            {
                case "bow":
                case "knife":
                    return new Rect(482, 248, 94, 126);
                case "mender":
                    return new Rect(620, 282, 104, 138);
                case "ember":
                case "hex":
                    return new Rect(492, 430, 104, 136);
                case "shield":
                case "pike":
                case "ward":
                default:
                    return new Rect(552, 64, 96, 128);
            }
        }

        private Rect AtlasPartyPortraitRegion(string role)
        {
            switch (role)
            {
                case "bow":
                case "knife":
                    return new Rect(128, 384, 98, 142);
                case "mender":
                    return new Rect(238, 384, 100, 142);
                case "ember":
                case "hex":
                    return new Rect(354, 384, 100, 142);
                case "shield":
                case "pike":
                case "ward":
                default:
                    return new Rect(34, 384, 96, 142);
            }
        }

        private Rect AtlasEnemyRegion(string role)
        {
            switch (role)
            {
                case "koboldshaman":
                    return new Rect(1282, 48, 238, 286);
                case "koboldwizard":
                case "shade":
                case "glassmage":
                case "adept":
                    return new Rect(1282, 402, 238, 286);
                case "koboldking":
                    return new Rect(902, 92, 138, 196);
                case "koboldshield":
                    return new Rect(1010, 268, 100, 130);
                case "koboldslinger":
                    return new Rect(985, 252, 102, 128);
                case "koboldraider":
                    return new Rect(820, 88, 112, 128);
                case "bonepriest":
                    return new Rect(900, 96, 126, 190);
                case "cinderling":
                case "gloamknight":
                case "reaver":
                    return new Rect(902, 92, 138, 196);
                default:
                    return new Rect(812, 86, 118, 132);
            }
        }

        private bool TryDrawAtlasCombatSprite(Rect rect, CombatUnit unit, bool allowDemonFormArt = true)
        {
            int demonIndex = allowDemonFormArt ? DemonSummonSpriteIndex(unit) : -1;
            if (demonIndex >= 0 && TryDrawDemonSummonAtlasIcon(Pad(rect, -rect.width * 0.02f), demonIndex, Color.white)) return true;
            if (unit != null && unit.Side == UnitSide.Enemy)
            {
                int koboldBossIndex = KoboldBossSpriteIndex(unit, false);
                if (koboldBossIndex >= 0 && TryDrawKoboldBossAtlasIcon(Pad(rect, -rect.width * 0.01f), koboldBossIndex, Color.white)) return true;
                int sewerEnemyIndex = MidgaardSewerEnemySpriteIndex(unit.Role);
                if (sewerEnemyIndex >= 0 && TryDrawMidgaardSewerAtlasIcon(Pad(rect, -rect.width * 0.01f), sewerEnemyIndex, Color.white)) return true;
                int enemySpriteIndex = EnemySpriteIndex(unit.Role);
                if (enemySpriteIndex >= 0 && TryDrawEnemySpriteAtlasIcon(Pad(rect, -rect.width * 0.015f), enemySpriteIndex, Color.white)) return true;
            }
            if (unit != null && unit.Side == UnitSide.Party)
            {
                int characterIndex = CharacterCombatAtlasIndex(unit.ClassKey, unit.Race, unit.Role);
                if (characterIndex >= 0 && TryDrawCharacterCombatAtlasIcon(Pad(rect, -rect.width * 0.06f), characterIndex, Color.white)) return true;
            }
            int creatureIndex = CreatureSpriteIndex(unit);
            if (creatureIndex >= 0)
            {
                float pad = unit != null && unit.Side == UnitSide.Party ? -rect.width * 0.04f : -rect.width * 0.015f;
                if (TryDrawCreatureSpriteAtlasIcon(Pad(rect, pad), creatureIndex, Color.white)) return true;
            }
            if (unit != null && unit.Side == UnitSide.Enemy)
            {
                int enemyWorldIndex = EnemyWorldEnemyIndex(unit.Role);
                if (enemyWorldIndex >= 0 && TryDrawEnemyWorldObjectAtlasIcon(Pad(rect, rect.width * 0.04f), enemyWorldIndex, Color.white)) return true;
                int bossIndex = BossEnemyIndex(unit.Role);
                if (bossIndex >= 0 && TryDrawBossEnemyAtlasIcon(Pad(rect, rect.width * 0.04f), bossIndex, Color.white)) return true;
            }
            return TryDrawSpriteSheetCombatSprite(rect, unit);
        }

        private bool TryDrawAtlasPartyPortrait(Rect rect, PartyMember member)
        {
            int characterIndex = CharacterCombatAtlasIndex(member?.ClassKey, member?.Race, member?.Role);
            if (characterIndex >= 0 && TryDrawCharacterCombatAtlasIcon(Pad(rect, -rect.width * 0.08f), characterIndex, Color.white)) return true;
            return TryDrawAtlasPartyPortrait(rect, member?.Role);
        }

        private bool TryDrawAtlasPartyPortrait(Rect rect, string role)
        {
            int creatureIndex = CreatureSpriteIndexForRole(role, UnitSide.Party);
            if (creatureIndex >= 0 && TryDrawCreatureSpriteAtlasIcon(Pad(rect, -rect.width * 0.01f), creatureIndex, Color.white)) return true;
            int index = SpriteSheetIndexForRole(role, UnitSide.Party);
            if (index < 0) return false;
            DrawRect(Pad(rect, rect.width * 0.05f), Hex("050708", 0.65f));
            DrawSpriteSheetCell(rect, index, 0.98f);
            return true;
        }

        private int KoboldBossSpriteIndex(CombatUnit unit, bool portrait)
        {
            if (unit == null || unit.Role != "koboldking") return -1;
            if (portrait) return 3;
            if (unit.Hp <= 0) return 2;
            if (unit.MaxHp > 0 && unit.Hp * 4 <= unit.MaxHp) return 8;
            if (unit.MaxHp > 0 && unit.Hp * 2 <= unit.MaxHp) return 1;
            return 0;
        }

        private int EnemySpriteIndex(string role)
        {
            switch ((role ?? "").ToLowerInvariant())
            {
                case "sewerrat":
                case "giantrat": return -1;
                case "ratfolk":
                case "ratcutthroat": return 8;
                case "ratmage": return 11;
                case "ratcleric": return 10;
                case "ratbrute": return 8;
                case "koboldraider": return 0;
                case "koboldshield": return 6;
                case "koboldslinger": return 2;
                case "koboldshaman": return 3;
                case "koboldwizard": return 4;
                case "koboldking": return 5;
                case "drowscout":
                case "drowblade": return 12;
                case "drowcrossbow": return 12;
                case "drowmage":
                case "drowpriest": return 13;
                case "boundimp": return 14;
                case "lesserdemon":
                case "greaterdemon":
                case "cinderling": return 15;
                case "gloamknight":
                case "reaver": return 15;
                case "bonepriest": return 13;
                case "shade":
                case "adept":
                case "glassmage": return 13;
                case "thornbeast": return -1;
                case "sentry": return 6;
                case "mirearcher": return 12;
                default: return -1;
            }
        }

        private int EnemyWorldEnemyIndex(string role)
        {
            switch ((role ?? "").ToLowerInvariant())
            {
                case "koboldraider":
                case "koboldslinger":
                case "koboldshield": return 0;
                case "koboldking": return 2;
                case "koboldshaman": return 1;
                case "koboldwizard": return 2;
                case "ratfolk":
                case "ratcutthroat":
                case "ratbrute":
                case "sewerrat":
                case "giantrat": return 3;
                case "ratmage":
                case "ratcleric": return 4;
                case "drowblade":
                case "drowscout": return 5;
                case "drowmage":
                case "drowpriest":
                case "glassmage":
                case "adept": return 6;
                case "drowcrossbow":
                case "mirearcher": return 7;
                case "lesserdemon":
                case "greaterdemon":
                case "cinderling":
                case "gloamknight": return 8;
                case "boundimp": return 9;
                default: return -1;
            }
        }

        private int BossEnemyIndex(string role)
        {
            switch (role ?? "")
            {
                case "ratbrute": return 0;
                case "ratmage":
                case "ratcleric": return 1;
                case "koboldshield": return 2;
                case "koboldshaman":
                case "koboldwizard": return 3;
                case "koboldking": return 16;
                case "drowblade": return 4;
                case "drowmage":
                case "drowpriest": return 5;
                case "lesserdemon": return 6;
                case "greaterdemon": return 6;
                case "boundimp": return 7;
                case "bonepriest": return 8;
                case "cinderling":
                case "gloamknight": return 9;
                case "adept":
                case "sentry": return 10;
                case "glassmage": return 11;
                case "giantrat":
                case "spore": return 12;
                case "reaver": return 13;
                case "husk": return 14;
                case "drowcrossbow":
                case "mirearcher": return 15;
                case "meteorlich": return 17;
                case "ritualheart": return 18;
                default: return -1;
            }
        }

        private int CreatureSpriteIndex(CombatUnit unit)
        {
            if (unit == null) return -1;
            if (unit.Summoned && (unit.Role ?? "").ToLowerInvariant().Contains("imp")) return 10;
            if (unit.Summoned && (unit.Role ?? "").ToLowerInvariant().Contains("demon")) return 11;
            return CreatureSpriteIndexForRole(unit.Role, unit.Side);
        }

        private int CharacterCombatAtlasIndex(string classKey, string race, string role)
        {
            string cls = (string.IsNullOrWhiteSpace(classKey) ? ClassForRole(role) : classKey).Trim().ToLowerInvariant();
            return PlayerSpriteCatalog.AtlasIndex(cls, race);
        }

        private int CreatureSpriteIndexForRole(string role, UnitSide side)
        {
            string key = (role ?? "").ToLowerInvariant();
            if (side == UnitSide.Party)
            {
                switch (key)
                {
                    case "boundimp": return 10;
                    case "lesserdemon":
                    case "greaterdemon": return 11;
                    case "knife":
                    case "bow": return 1;
                    case "mender":
                    case "ward": return 2;
                    case "ember":
                    case "hex": return 3;
                    case "shield":
                    case "pike":
                    default: return 0;
                }
            }

            switch (key)
            {
                case "koboldraider":
                case "koboldslinger":
                case "koboldshield": return 4;
                case "koboldshaman":
                case "koboldwizard": return 5;
                case "ratfolk":
                case "ratcutthroat":
                case "ratbrute":
                case "sewerrat":
                case "giantrat": return 6;
                case "ratmage":
                case "ratcleric": return 7;
                case "drowblade":
                case "drowscout":
                case "drowcrossbow":
                case "mirearcher": return 8;
                case "drowmage":
                case "drowpriest":
                case "glassmage":
                case "adept": return 9;
                case "boundimp": return 10;
                case "lesserdemon":
                case "greaterdemon":
                case "cinderling":
                case "gloamknight": return 11;
                case "reaver":
                case "husk":
                case "shade": return 12;
                case "bonepriest":
                case "meteorlich":
                case "ritualheart": return 13;
                case "sentry": return 14;
                case "spore": return 15;
                default: return -1;
            }
        }

        private bool TryDrawSpriteSheetCombatSprite(Rect rect, CombatUnit unit)
        {
            if (unit == null || combatSpriteSheet == null) return false;
            int index = SpriteSheetIndexForRole(unit.Role, unit.Side);
            if (index < 0) return false;
            DrawRect(Pad(rect, rect.width * 0.08f), Hex("050708", 0.34f));
            DrawSpriteSheetCell(rect, index, SpriteSheetScaleFor(unit));
            return true;
        }

        private void DrawSpriteSheetCell(Rect rect, int index, float scale)
        {
            if (combatSpriteSheet == null || index < 0) return;
            int columns = 4;
            int row = index / columns;
            int col = index % columns;
            float cellW = combatSpriteSheet.width / 4f;
            float cellH = combatSpriteSheet.height / 4f;
            Rect source = new Rect(col * cellW, row * cellH, cellW, cellH);
            SpriteCellMetrics metrics = GetSpriteCellMetrics(index, source);
            source = metrics.Source;
            float aspect = source.width / Mathf.Max(1f, source.height);
            Rect safe = Pad(rect, rect.width * 0.12f);
            float destH = safe.height * Mathf.Clamp(scale, 0.65f, 1.08f);
            float destW = destH * aspect;
            float maxW = safe.width * 0.96f;
            if (destW > maxW)
            {
                destW = maxW;
                destH = destW / Mathf.Max(0.1f, aspect);
            }

            float baseline = rect.y + rect.height * 0.79f;
            float x = rect.center.x - destW * Mathf.Clamp01(metrics.AnchorX);
            x = Mathf.Clamp(x, safe.x, safe.xMax - destW);
            Rect dest = new Rect(x, baseline - destH, destW, destH);
            DrawCombatSpriteSheetRegion(dest, source);
        }

        private SpriteCellMetrics GetSpriteCellMetrics(int index, Rect cell)
        {
            SpriteCellMetrics cached;
            if (spriteCellMetrics.TryGetValue(index, out cached)) return cached;

            SpriteCellMetrics metrics = new SpriteCellMetrics { Source = cell, AnchorX = 0.5f };
            if (combatSpriteSheet == null)
            {
                spriteCellMetrics[index] = metrics;
                return metrics;
            }

            try
            {
                Color32[] pixels = combatSpriteSheet.GetPixels32();
                int texW = combatSpriteSheet.width;
                int texH = combatSpriteSheet.height;
                int minX = Mathf.FloorToInt(cell.xMax);
                int maxX = Mathf.FloorToInt(cell.x);
                int minY = Mathf.FloorToInt(cell.yMax);
                int maxY = Mathf.FloorToInt(cell.y);
                int startX = Mathf.Clamp(Mathf.FloorToInt(cell.x), 0, texW - 1);
                int endX = Mathf.Clamp(Mathf.CeilToInt(cell.xMax), 0, texW);
                int startY = Mathf.Clamp(Mathf.FloorToInt(cell.y), 0, texH - 1);
                int endY = Mathf.Clamp(Mathf.CeilToInt(cell.yMax), 0, texH);

                for (int sy = startY; sy < endY; sy++)
                for (int sx = startX; sx < endX; sx++)
                {
                    int bottomY = texH - 1 - sy;
                    if (bottomY < 0 || bottomY >= texH) continue;
                    Color32 pixelColor = pixels[bottomY * texW + sx];
                    if (pixelColor.a <= 24) continue;
                    minX = Mathf.Min(minX, sx);
                    maxX = Mathf.Max(maxX, sx);
                    minY = Mathf.Min(minY, sy);
                    maxY = Mathf.Max(maxY, sy);
                }

                if (maxX > minX && maxY > minY)
                {
                    Rect trimmed = new Rect(minX, minY, maxX - minX + 1, maxY - minY + 1);
                    metrics.Source = ExpandSourceRect(trimmed, cell, 3f);
                    metrics.AnchorX = SpriteLowerBodyAnchorX(metrics.Source, pixels, texW, texH);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Sprite trim fallback for cell " + index + ": " + ex.Message);
            }

            spriteCellMetrics[index] = metrics;
            return metrics;
        }

        private Rect ExpandSourceRect(Rect rect, Rect limit, float pad)
        {
            float xMin = Mathf.Max(limit.xMin, rect.xMin - pad);
            float yMin = Mathf.Max(limit.yMin, rect.yMin - pad);
            float xMax = Mathf.Min(limit.xMax, rect.xMax + pad);
            float yMax = Mathf.Min(limit.yMax, rect.yMax + pad);
            return new Rect(xMin, yMin, Mathf.Max(1f, xMax - xMin), Mathf.Max(1f, yMax - yMin));
        }

        private float SpriteLowerBodyAnchorX(Rect source, Color32[] pixels, int texW, int texH)
        {
            float lowerStart = source.y + source.height * 0.58f;
            float weighted = 0f;
            float weight = 0f;
            int startX = Mathf.Clamp(Mathf.FloorToInt(source.x), 0, texW - 1);
            int endX = Mathf.Clamp(Mathf.CeilToInt(source.xMax), 0, texW);
            int startY = Mathf.Clamp(Mathf.FloorToInt(lowerStart), 0, texH - 1);
            int endY = Mathf.Clamp(Mathf.CeilToInt(source.yMax), 0, texH);

            for (int sy = startY; sy < endY; sy++)
            for (int sx = startX; sx < endX; sx++)
            {
                int bottomY = texH - 1 - sy;
                if (bottomY < 0 || bottomY >= texH) continue;
                byte alpha = pixels[bottomY * texW + sx].a;
                if (alpha <= 24) continue;
                float rowWeight = Mathf.Lerp(1f, 2.5f, Mathf.InverseLerp(startY, Mathf.Max(startY + 1, endY - 1), sy));
                weighted += (sx + 0.5f) * alpha * rowWeight;
                weight += alpha * rowWeight;
            }

            if (weight <= 0f) return 0.5f;
            return Mathf.Clamp01((weighted / weight - source.x) / Mathf.Max(1f, source.width));
        }

        private int SpriteSheetIndexForRole(string role, UnitSide side)
        {
            role = role ?? "";
            if (role == "boundimp" || role == "lesserdemon" || role == "greaterdemon") return 14;
            if (side == UnitSide.Party)
            {
                switch (role)
                {
                    case "shield": return 0;
                    case "pike": return 1;
                    case "bow": return 2;
                    case "knife": return 3;
                    case "mender": return 4;
                    case "ember": return 5;
                    case "hex": return 6;
                    case "ward": return 7;
                    default: return -1;
                }
            }

            switch (role)
            {
                case "koboldraider": return 8;
                case "koboldslinger": return 9;
                case "koboldshaman": return 10;
                case "koboldwizard": return 11;
                case "bonepriest": return 12;
                case "glassmage": return 13;
                case "cinderling": return 14;
                case "koboldshield": return 15;
                case "ratfolk":
                case "ratcutthroat":
                case "ratmage":
                case "ratcleric":
                case "ratbrute": return 12;
                case "drowscout":
                case "drowblade":
                case "drowcrossbow":
                case "drowmage":
                case "drowpriest": return 13;
                default: return -1;
            }
        }

        private float SpriteSheetScaleFor(CombatUnit unit)
        {
            if (unit == null) return 0.92f;
            if (unit.Side == UnitSide.Enemy)
            {
                if (unit.Role == "koboldraider" || unit.Role == "koboldslinger" || unit.Role == "koboldshield") return 1.02f;
                if (unit.Role == "ratfolk" || unit.Role == "ratcutthroat" || unit.Role == "ratmage" || unit.Role == "ratcleric") return 0.96f;
                if (unit.Role == "ratbrute" || unit.Role == "lesserdemon") return 1.03f;
                if (unit.Role == "greaterdemon") return 1.08f;
                if (unit.Role != null && unit.Role.StartsWith("drow")) return 0.97f;
                if (unit.Role == "cinderling") return 1.00f;
                return 0.96f;
            }
            if (unit.Role == "mender" || unit.Role == "ember" || unit.Role == "hex") return 0.94f;
            return 0.96f;
        }

        private bool TryDrawExplorePartyToken(Rect rect, string role, Color color, string sigil)
        {
            int roleIndex = ExplorationCharacterArtCatalog.PlayerRoleIndex(role);
            int legacyIndex = WorldMapTokenSpriteIndex(role);
            bool canUseDedicatedRole = roleIndex >= 0 && IsPlayerExplorationRoleAtlas();
            bool canUseLegacyToken = legacyIndex >= 0 && IsWorldMapTokenSpriteAtlas();
            if (!canUseDedicatedRole && !canUseLegacyToken) return false;
            float shadowHeight = Mathf.Max(3f, rect.height * 0.075f);
            Rect shadow = new Rect(rect.x + rect.width * 0.18f, rect.yMax - shadowHeight - rect.height * 0.02f, rect.width * 0.64f, shadowHeight);
            DrawRect(shadow, Hex("020303", 0.86f));
            WorldMapArtSpec spec = new WorldMapArtSpec(1.02f, new Vector2(0.5f, 1f), new Vector2(0f, 0.01f), true);
            if (canUseDedicatedRole && TryDrawPlayerExplorationRoleAtlasIcon(rect, roleIndex, Color.white, spec)) return true;
            return canUseLegacyToken && TryDrawWorldMapTokenSpriteAtlasIcon(rect, legacyIndex, Color.white, spec);
        }

        private int WorldMapTokenSpriteIndex(string role)
        {
            switch ((role ?? "").ToLowerInvariant())
            {
                case "party": return 0;
                case "bow": return 2;
                case "knife": return 3;
                case "mender": return 4;
                case "ember": return 5;
                case "hex": return 6;
                case "ward": return 7;
                case "pike": return 8;
                case "shield": return 1;
                default: return 0;
            }
        }

        private void DrawToken(Rect rect, string role, Color color, bool active, string text, string sigil)
        {
            if (active)
            {
                float pulse = 0.5f + Mathf.Sin(Time.time * 5f) * 0.5f;
                DrawBorder(new Rect(rect.x - 4, rect.y - 4, rect.width + 8, rect.height + 8), Color.Lerp(gold, teal, pulse), 3);
            }
            DrawRect(rect, Hex("101619"));
            DrawBorder(rect, color, Mathf.Max(2, Mathf.RoundToInt(rect.width * 0.07f)));
            DrawRect(new Rect(rect.x + rect.width * 0.08f, rect.y + rect.height * 0.08f, rect.width * 0.84f, rect.height * 0.12f), Color.Lerp(color, ink, 0.25f));

            if (IsEnemyRole(role))
            {
                DrawEnemyFigure(rect, role, color);
                DrawSigil(new Rect(rect.x + rect.width * 0.37f, rect.y + rect.height * 0.80f, rect.width * 0.26f, rect.height * 0.14f), sigil, Color.Lerp(color, ink, 0.35f));
                return;
            }

            DrawPartyFigure(rect, role, color);
            DrawSigil(new Rect(rect.x + rect.width * 0.37f, rect.y + rect.height * 0.80f, rect.width * 0.26f, rect.height * 0.14f), sigil, ink);
            if (!string.IsNullOrEmpty(text)) GUI.Label(rect, text, CenterStyle(Mathf.RoundToInt(rect.height * 0.21f), ink));
        }

        private void DrawCombatUnitSprite(
            Rect rect,
            Rect anchoredRect,
            CombatUnit unit,
            bool active,
            float figureAlpha,
            bool allowDemonFormArt = true)
        {
            Color color = VividColor(unit.Color.ToColor());
            Color frame = CombatFrameColor(unit, active);
            DrawCombatSpritePedestal(anchoredRect, frame, color, active);
            DrawCombatSpriteFootlight(anchoredRect, frame, color, active);
            Rect spriteRect = rect;
            if (active)
            {
                float bob = state.ReducedMotion ? 0f : Mathf.Sin(Time.time * 7f) * rect.height * 0.018f;
                spriteRect.y += bob;
            }

            DrawCombatSpriteGroundShadow(anchoredRect, color, active);
            Color previousGuiColor = GUI.color;
            try
            {
                GUI.color = new Color(
                    previousGuiColor.r,
                    previousGuiColor.g,
                    previousGuiColor.b,
                    previousGuiColor.a * Mathf.Clamp01(figureAlpha));
                bool atlasDrawn = TryDrawAtlasCombatSprite(spriteRect, unit, allowDemonFormArt);

                if (!atlasDrawn && unit.Summoned)
                {
                    DrawSummonFigure(spriteRect, unit.Role, color);
                }
                else if (!atlasDrawn && unit.Side == UnitSide.Enemy)
                {
                    DrawEnemyFigure(spriteRect, unit.Role, color);
                    DrawEnemyCombatDetails(spriteRect, unit);
                    DrawEnemyDepthVariant(spriteRect, unit);
                    DrawEnemyVariantOverlay(spriteRect, unit);
                }
                else if (!atlasDrawn)
                {
                    DrawPartyFigure(spriteRect, unit.Role, color);
                    DrawPartyVariantOverlay(spriteRect, unit.Role, unit.Id + unit.Name + unit.Sigil, unit.ArmorName, unit.WeaponName, color);
                    DrawEquipmentOverlay(spriteRect, unit.Role, unit.WeaponName, unit.ArmorName, unit.DamageType);
                }
            }
            finally
            {
                GUI.color = previousGuiColor;
            }

            DrawCombatSpriteSideRim(anchoredRect, unit, frame, active, figureAlpha);
            DrawEnemyRankMarker(anchoredRect, unit, figureAlpha);
            if (active && figureAlpha > 0.05f) DrawActiveSpriteAccent(anchoredRect, unit);

            // Keep combat sprites art-first; tactical details live in HP bars, edge pips, hover cards, and side panels.
        }

        private void DrawCombatSpritePedestal(Rect rect, Color frame, Color accent, bool active)
        {
            CombatSpriteStageGeometry stage = CombatSpriteStageRules.GeometryFor(rect);
            DrawRect(stage.Footprint, Hex("030405", active ? 0.54f : 0.38f));
            DrawRect(
                new Rect(stage.Footprint.x + stage.Footprint.width * 0.08f, stage.Footprint.yMax - 2f, stage.Footprint.width * 0.84f, 2f),
                Color.Lerp(frame, accent, 0.28f).WithAlpha(active ? 0.90f : 0.58f));
        }

        private void DrawCombatSpriteFootlight(Rect rect, Color frame, Color accent, bool active)
        {
            CombatSpriteStageGeometry stage = CombatSpriteStageRules.GeometryFor(rect);
            Color light = Color.Lerp(frame, accent, 0.46f);
            DrawRect(stage.Footlight, light.WithAlpha(active ? 0.085f : 0.045f));
            DrawRect(stage.FootlightCore, Color.Lerp(light, cursorWhite, 0.12f).WithAlpha(active ? 0.11f : 0.055f));
        }

        private void DrawCombatSpriteGroundShadow(Rect rect, Color tint, bool active)
        {
            CombatSpriteStageGeometry stage = CombatSpriteStageRules.GeometryFor(rect);
            Color shadow = Hex("050708", active ? 0.80f : 0.61f);
            DrawRect(stage.Footprint, shadow);
            DrawRect(stage.FootprintCore, Color.Lerp(shadow, tint, active ? 0.23f : 0.16f));
        }

        private void DrawCombatSpriteSideRim(
            Rect rect,
            CombatUnit unit,
            Color frame,
            bool active,
            float figureAlpha)
        {
            if (unit == null || figureAlpha <= 0.05f) return;
            CombatSpriteStageGeometry stage = CombatSpriteStageRules.GeometryFor(rect);
            Color faction = unit.Side == UnitSide.Party ? teal : blood;
            Color rim = Color.Lerp(frame, faction, 0.44f).WithAlpha(
                Mathf.Clamp01(figureAlpha) * (active ? 0.72f : 0.36f));
            DrawRect(stage.LeftRim, rim);
            DrawRect(stage.RightRim, rim.WithAlpha(rim.a * 0.72f));
        }

        private void DrawEnemyRankMarker(Rect rect, CombatUnit unit, float figureAlpha)
        {
            if (unit == null || unit.Side != UnitSide.Enemy || figureAlpha <= 0.05f) return;

            int chevronCount;
            Color accent;
            if (string.Equals(unit.Rank, "veteran", StringComparison.OrdinalIgnoreCase))
            {
                chevronCount = 1;
                accent = Color.Lerp(Hex("d0c5ae"), cursorWhite, 0.18f);
            }
            else if (string.Equals(unit.Rank, "elite", StringComparison.OrdinalIgnoreCase))
            {
                chevronCount = 2;
                accent = Color.Lerp(gold, cursorWhite, 0.12f);
            }
            else
            {
                return;
            }

            float alpha = Mathf.Clamp01(figureAlpha);
            float width = Mathf.Max(14f, rect.width * 0.18f);
            float height = Mathf.Max(11f, rect.height * (chevronCount == 1 ? 0.12f : 0.17f));
            Rect badge = new Rect(
                rect.xMax - width - rect.width * 0.07f,
                rect.y + rect.height * 0.09f,
                width,
                height);
            DrawRect(badge, Hex("030405", 0.76f * alpha));
            DrawBorder(badge, accent.WithAlpha(0.70f * alpha), 1);

            float inset = Mathf.Max(3f, width * 0.20f);
            float chevronHeight = Mathf.Max(3f, height * 0.25f);
            float gap = Mathf.Max(2f, height * 0.11f);
            float stroke = Mathf.Max(1f, rect.width * 0.018f);
            Color shadow = Hex("030405", 0.90f * alpha);
            Color mark = accent.WithAlpha(0.96f * alpha);
            float stackHeight = chevronCount * chevronHeight + (chevronCount - 1) * gap;
            float startY = badge.center.y - stackHeight * 0.5f;
            for (int i = 0; i < chevronCount; i++)
            {
                float y = startY + i * (chevronHeight + gap);
                Vector2 left = new Vector2(badge.x + inset, y + chevronHeight);
                Vector2 peak = new Vector2(badge.center.x, y);
                Vector2 right = new Vector2(badge.xMax - inset, y + chevronHeight);
                DrawPixelLine(left, peak, shadow, stroke + 2f);
                DrawPixelLine(peak, right, shadow, stroke + 2f);
                DrawPixelLine(left, peak, mark, stroke);
                DrawPixelLine(peak, right, mark, stroke);
            }
        }

        private void DrawPartyPortraitSprite(Rect rect, PartyMember member, Color color)
        {
            color = VividColor(color);
            DrawSpriteFrameBackground(rect, color, color, false);
            DrawSpriteGroundShadow(rect, color, false);
            bool characterAtlasDrawn = IsCharacterCombatAtlas() && CharacterCombatAtlasIndex(member?.ClassKey, member?.Race, member?.Role) >= 0;
            bool atlasDrawn = TryDrawAtlasPartyPortrait(rect, member);
            if (!atlasDrawn)
            {
                DrawPartyFigure(rect, member.Role, color);
                DrawPartyVariantOverlay(rect, member.Role, member.Id + member.Name + member.Sigil, member.ArmorName, member.WeaponName, color);
                DrawEquipmentOverlay(rect, member.Role, member.WeaponName, member.ArmorName, member.WeaponDamageType);
            }
            else if (!characterAtlasDrawn)
            {
                DrawEquipmentOverlay(rect, member.Role, member.WeaponName, member.ArmorName, member.WeaponDamageType);
            }
            DrawSigil(new Rect(rect.x + rect.width * 0.37f, rect.y + rect.height * 0.80f, rect.width * 0.26f, rect.height * 0.14f), member.Sigil, ink);
        }

        private void DrawSpriteFrameBackground(Rect rect, Color frame, Color accent, bool active)
        {
            DrawRect(rect, Hex("101619"));
            int heavy = Mathf.Max(2, Mathf.RoundToInt(rect.width * 0.045f));
            DrawBorder(rect, Hex("030405", 0.92f), heavy + 1);
            DrawBorder(Pad(rect, heavy), frame, Mathf.Max(2, heavy));
            DrawCombatUiCornerTrim(rect, frame);
            if (active)
            {
                float pulse = state.ReducedMotion ? 0.45f : 0.5f + Mathf.Sin(Time.time * 6f) * 0.5f;
                DrawBorder(Pad(rect, -2f), Color.Lerp(frame, cursorWhite, pulse * 0.35f), 2);
            }
        }

        private void DrawPartyVariantOverlay(Rect rect, string role, string seedKey, string armorName, string weaponName, Color accent)
        {
            int seed = StableSeed(seedKey + role);
            Color skin = VariantSkin(seed);
            Color hair = VariantHair(seed);
            Color cloak = Color.Lerp(accent, Color.black, 0.22f);
            string armor = GearKind(armorName, role, false);
            string weapon = GearKind(weaponName, role, true);
            bool helm = armor.Contains("helm") || armor.Contains("plate") || armor.Contains("chain");
            float lean = (seed % 3 - 1) * rect.width * 0.018f;
            Rect head = new Rect(rect.x + rect.width * 0.36f + lean, rect.y + rect.height * 0.14f, rect.width * 0.28f, rect.height * 0.20f);
            Rect torso = new Rect(rect.x + rect.width * 0.29f + lean * 0.5f, rect.y + rect.height * 0.37f, rect.width * 0.42f, rect.height * 0.34f);

            DrawRect(new Rect(rect.x + rect.width * 0.25f + lean * 0.3f, rect.y + rect.height * 0.50f, rect.width * 0.50f, rect.height * 0.25f), Color.Lerp(cloak, Hex("101619"), 0.12f));
            DrawRect(head, skin);
            if (helm)
            {
                Color metal = MaterialTint(armor, Hex("a9b0a2"));
                DrawRect(new Rect(head.x - head.width * 0.06f, head.y, head.width * 1.12f, head.height * 0.42f), metal);
                DrawRect(new Rect(head.x + head.width * 0.10f, head.y + head.height * 0.32f, head.width * 0.80f, head.height * 0.10f), Color.Lerp(metal, cursorWhite, 0.20f));
            }
            else
            {
                int hairStyle = seed % 4;
                DrawRect(new Rect(head.x, head.y, head.width, head.height * 0.25f), hair);
                if (hairStyle == 1) DrawRect(new Rect(head.x - head.width * 0.12f, head.y + head.height * 0.14f, head.width * 0.20f, head.height * 0.50f), hair);
                if (hairStyle == 2) DrawRect(new Rect(head.x + head.width * 0.72f, head.y + head.height * 0.10f, head.width * 0.20f, head.height * 0.58f), hair);
                if (hairStyle == 3) DrawRect(new Rect(head.x + head.width * 0.18f, head.y - head.height * 0.10f, head.width * 0.64f, head.height * 0.18f), hair);
            }

            DrawRect(new Rect(head.x + head.width * 0.24f, head.y + head.height * 0.52f, head.width * 0.12f, head.height * 0.10f), Hex("101619", 0.72f));
            DrawRect(new Rect(head.x + head.width * 0.64f, head.y + head.height * 0.52f, head.width * 0.12f, head.height * 0.10f), Hex("101619", 0.72f));
            DrawRect(new Rect(torso.x + torso.width * 0.20f, torso.y + torso.height * 0.42f, torso.width * 0.60f, torso.height * 0.08f), Color.Lerp(accent, cursorWhite, 0.22f));
            DrawRect(new Rect(rect.x + rect.width * (seed % 2 == 0 ? 0.30f : 0.33f), rect.y + rect.height * 0.73f, rect.width * 0.13f, rect.height * 0.05f), Color.Lerp(accent, Color.black, 0.36f));
            DrawRect(new Rect(rect.x + rect.width * (seed % 2 == 0 ? 0.58f : 0.55f), rect.y + rect.height * 0.73f, rect.width * 0.13f, rect.height * 0.05f), Color.Lerp(accent, Color.black, 0.36f));
            DrawPersonalSpriteDetails(rect, seed, role, armor, weapon, accent, hair, skin);

            if (role == "bow")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.78f, rect.y + rect.height * 0.30f, rect.width * 0.08f, rect.height * 0.36f), Color.Lerp(hair, Hex("9b6b45"), 0.35f));
                DrawRect(new Rect(rect.x + rect.width * 0.80f, rect.y + rect.height * 0.24f, rect.width * 0.05f, rect.height * 0.10f), cursorWhite);
            }
            else if (role == "knife")
            {
                DrawRect(new Rect(head.x - head.width * 0.12f, head.y + head.height * 0.02f, head.width * 1.24f, head.height * 0.22f), Color.Lerp(cloak, Color.black, 0.20f));
            }
            else if (role == "mender")
            {
                DrawBorder(new Rect(rect.x + rect.width * 0.41f, rect.y + rect.height * 0.25f, rect.width * 0.18f, rect.height * 0.12f), teal, 1);
            }
            else if (role == "ember" || role == "hex")
            {
                Color magic = role == "ember" ? ember : violet;
                DrawRect(new Rect(rect.x + rect.width * 0.40f, rect.y + rect.height * 0.22f, rect.width * 0.20f, rect.height * 0.05f), magic);
                DrawRect(new Rect(rect.x + rect.width * 0.49f, rect.y + rect.height * 0.17f, rect.width * 0.04f, rect.height * 0.15f), magic);
            }
            else if (role == "shield" || role == "ward")
            {
                DrawSigil(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.51f, rect.width * 0.10f, rect.height * 0.10f), weapon.Contains("mace") ? "diamond" : "bar", gold);
            }
            DrawRoleAccessoryFlair(rect, role, seed, armor, weapon, accent);
        }

        private void DrawSpriteGroundShadow(Rect rect, Color tint, bool active)
        {
            Color shadow = Hex("050708", active ? 0.78f : 0.58f);
            DrawRect(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.83f, rect.width * 0.60f, rect.height * 0.07f), shadow);
            DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.80f, rect.width * 0.44f, rect.height * 0.04f), Color.Lerp(shadow, tint, 0.18f));
        }

        private void DrawActiveSpriteAccent(Rect rect, CombatUnit unit)
        {
            CombatSpriteStageGeometry stage = CombatSpriteStageRules.GeometryFor(rect);
            float pulse = CombatSpriteStageRules.ActivePulse(Time.time, state != null && state.ReducedMotion);
            Color baseGlow = unit.Side == UnitSide.Party ? teal : blood;
            Color glow = Color.Lerp(baseGlow, cursorWhite, Mathf.Clamp01(pulse));
            DrawRect(stage.ActiveTick, glow.WithAlpha(0.54f + pulse * 0.24f));
            DrawRect(stage.ActiveTickCore, Color.Lerp(glow, cursorWhite, 0.26f).WithAlpha(0.90f));
            DrawRect(new Rect(stage.ActiveTick.x, stage.ActiveTick.y - 2f, 2f, stage.ActiveTick.height + 4f), glow.WithAlpha(0.78f));
            DrawRect(new Rect(stage.ActiveTick.xMax - 2f, stage.ActiveTick.y - 2f, 2f, stage.ActiveTick.height + 4f), glow.WithAlpha(0.62f));
        }

        private void DrawPartyCombatSpriteAccents(Rect rect, CombatUnit unit, Color frame)
        {
            if (unit == null) return;
            Color role = RoleColor(unit.Role);
            Color damage = DamageColor(string.IsNullOrWhiteSpace(unit.DamageType) ? "physical" : unit.DamageType);
            float pip = Mathf.Max(5f, rect.width * 0.065f);
            Rect rolePip = new Rect(rect.x + rect.width * 0.08f, rect.y + rect.height * 0.12f, pip, pip);
            DrawRect(rolePip, Hex("030405", 0.78f));
            DrawBorder(rolePip, role.WithAlpha(0.86f), 1);
            DrawRect(Pad(rolePip, 2f), role.WithAlpha(0.72f));
            if (unit.MaxMana > 0)
            {
                Rect manaPip = new Rect(rolePip.x, rolePip.yMax + 3f, rolePip.width, Mathf.Max(3f, pip * 0.35f));
                DrawRect(manaPip, Hex("030405", 0.74f));
                DrawRect(new Rect(manaPip.x, manaPip.y, manaPip.width * Mathf.Clamp01(unit.Mana / (float)Mathf.Max(1, unit.MaxMana)), manaPip.height), teal.WithAlpha(0.72f));
            }

            Rect sigil = new Rect(rect.xMax - rect.width * 0.22f, rect.y + rect.height * 0.075f, rect.width * 0.14f, rect.height * 0.14f);
            DrawRect(sigil, Hex("030405", 0.70f));
            DrawBorder(sigil, frame.WithAlpha(0.78f), 1);
            DrawSigil(Pad(sigil, sigil.width * 0.24f), unit.Sigil, role);

            string weapon = GearKind(unit.WeaponName, unit.Role, true);
            Rect weaponNotch = new Rect(rect.xMax - rect.width * 0.16f, rect.yMax - rect.height * 0.16f, pip * 1.35f, pip * 0.58f);
            if (weapon.Contains("bow") || weapon.Contains("crossbow"))
            {
                DrawRect(weaponNotch, moss.WithAlpha(0.80f));
            }
            else if (weapon.Contains("staff") || weapon.Contains("focus") || weapon.Contains("orb") || unit.Role == "mender" || unit.Role == "ember" || unit.Role == "hex")
            {
                DrawRect(weaponNotch, violet.WithAlpha(0.78f));
                DrawPixelCross(new Rect(weaponNotch.x + weaponNotch.width * 0.36f, weaponNotch.y - weaponNotch.height * 0.72f, weaponNotch.width * 0.26f, weaponNotch.height * 1.45f), damage.WithAlpha(0.82f));
            }
            else
            {
                DrawRect(weaponNotch, damage.WithAlpha(0.76f));
            }
        }

        private void DrawPartyCombatSpriteCornerSigil(Rect rect, CombatUnit unit, Color frame)
        {
            if (unit == null) return;
            float size = Mathf.Clamp(rect.width * 0.17f, 10f, 18f);
            Rect sigil = new Rect(rect.xMax - size - rect.width * 0.07f, rect.y + rect.height * 0.07f, size, size);
            Color role = RoleColor(unit.Role);
            DrawRect(sigil, Hex("030405", 0.74f));
            DrawBorder(sigil, frame.WithAlpha(0.72f), 1);
            DrawSigil(Pad(sigil, Mathf.Max(2f, size * 0.22f)), unit.Sigil, role.WithAlpha(0.88f));
        }

        private void DrawCombatSpriteBadges(Rect rect, CombatUnit unit, Color accent)
        {
            if (unit == null) return;
            float badge = Mathf.Clamp(rect.width * 0.28f, 20f, 42f);
            Rect topLeft = new Rect(rect.x + rect.width * 0.08f, rect.y + rect.height * 0.08f, badge, badge);
            Rect topRight = new Rect(rect.xMax - rect.width * 0.08f - badge, rect.y + rect.height * 0.08f, badge, badge);
            Rect bottomLeft = new Rect(rect.x + rect.width * 0.08f, rect.yMax - rect.height * 0.11f - badge, badge, badge);

            if (unit.Side == UnitSide.Party)
            {
                Color roleColor = RoleColor(unit.Role);
                DrawCombatBadgeLabel(topLeft, ClassShortLabel(unit), roleColor);
            }
            else
            {
                Color threat = DamageColor(string.IsNullOrEmpty(unit.DamageType) ? "physical" : unit.DamageType);
                DrawCombatBadgeLabel(topLeft, ClassShortLabel(unit), threat);
            }

            if (unit.Range > 1 || !string.IsNullOrEmpty(unit.Spell) || IsCasterEnemy(unit))
            {
                Color mark = !string.IsNullOrEmpty(unit.Spell) ? SpellSchoolColor(unit.Spell) : DamageColor(unit.DamageType);
                string label = unit.Range > 1 ? "R" + unit.Range : "CST";
                DrawCombatBadgeLabel(topRight, label, mark);
            }

            if (unit.Guarding || unit.Shielded > 0 || unit.Webbed > 0)
            {
                Color stateColor = unit.Webbed > 0 ? Hex("d9d3c4") : unit.Shielded > 0 ? teal : gold;
                DrawCombatBadgeLabel(bottomLeft, unit.Webbed > 0 ? "WEB" : unit.Shielded > 0 ? "WRD" : "GRD", stateColor);
            }
        }

        private void DrawCombatBadgeLabel(Rect rect, string label, Color accent)
        {
            DrawRect(rect, Hex("030405", 0.82f));
            DrawBorder(rect, accent.WithAlpha(0.92f), 1);
            DrawRect(new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f, Mathf.Max(2f, rect.height * 0.13f)), accent.WithAlpha(0.34f));
            GUIStyle style = CenterStyle(Mathf.RoundToInt(Mathf.Clamp(rect.height * 0.32f, 8f, 13f)), cursorWhite);
            GUI.Label(Pad(rect, 2f), FitText(label, rect.width - 4f, style), style);
        }

        private void DrawCombatSpriteCasterMarks(Rect rect, CombatUnit unit)
        {
            if (unit == null) return;
            bool caster = !string.IsNullOrEmpty(unit.Spell) || IsCasterEnemy(unit);
            if (!caster) return;
            Color mark = unit.Side == UnitSide.Party
                ? SpellSchoolColor(unit.Spell)
                : DamageColor(string.IsNullOrEmpty(unit.DamageType) ? "mind" : unit.DamageType);
            if (unit.Mana <= 0 && unit.Side == UnitSide.Party) mark = muted;
            float pulse = state != null && state.ReducedMotion ? 0.72f : 0.58f + Mathf.Sin(Time.time * 4.5f + rect.x * 0.03f) * 0.16f;
            mark.a = Mathf.Clamp01(pulse);
            float s = Mathf.Max(3f, rect.width * 0.045f);
            for (int i = 0; i < 3; i++)
            {
                float x = rect.x + rect.width * (0.28f + i * 0.18f);
                DrawRect(new Rect(x, rect.y + rect.height * 0.055f, s, s), mark);
                if (i == 1) DrawRect(new Rect(x - s * 0.7f, rect.y + rect.height * 0.055f + s * 0.35f, s * 2.4f, s * 0.35f), mark.WithAlpha(mark.a * 0.65f));
            }
            if (unit.Side == UnitSide.Enemy && IsCasterEnemy(unit))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.17f, rect.y + rect.height * 0.19f, rect.width * 0.12f, rect.height * 0.035f), mark);
                DrawRect(new Rect(rect.x + rect.width * 0.71f, rect.y + rect.height * 0.19f, rect.width * 0.12f, rect.height * 0.035f), mark);
            }
        }

        private void DrawWoundedSpriteMarks(Rect rect, CombatUnit unit)
        {
            if (unit == null || unit.MaxHp <= 0) return;
            float ratio = (float)unit.Hp / unit.MaxHp;
            if (ratio > 0.55f) return;
            Color hurt = unit.Side == UnitSide.Party ? blood : Color.Lerp(blood, unit.Color.ToColor(), 0.25f);
            DrawRect(new Rect(rect.x + rect.width * 0.23f, rect.y + rect.height * 0.63f, rect.width * 0.11f, rect.height * 0.035f), hurt);
            DrawRect(new Rect(rect.x + rect.width * 0.63f, rect.y + rect.height * 0.49f, rect.width * 0.10f, rect.height * 0.035f), hurt);
            if (ratio <= 0.25f)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.21f, rect.width * 0.64f, rect.height * 0.035f), Hex("080b0d", 0.62f));
                DrawRect(new Rect(rect.x + rect.width * 0.32f, rect.y + rect.height * 0.76f, rect.width * 0.36f, rect.height * 0.035f), hurt);
            }
        }

        private void DrawPixelCross(Rect rect, Color color)
        {
            DrawRect(new Rect(rect.x + rect.width * 0.40f, rect.y, rect.width * 0.20f, rect.height), color);
            DrawRect(new Rect(rect.x, rect.y + rect.height * 0.40f, rect.width, rect.height * 0.20f), color);
        }

        private void DrawPersonalSpriteDetails(Rect rect, int seed, string role, string armor, string weapon, Color accent, Color hair, Color skin)
        {
            Color strap = Color.Lerp(Hex("2f211b"), accent, 0.18f);
            Color boot = Color.Lerp(Hex("101619"), accent, 0.14f);
            float sway = (seed % 5 - 2) * rect.width * 0.006f;
            DrawRect(new Rect(rect.x + rect.width * 0.24f + sway, rect.y + rect.height * 0.42f, rect.width * 0.08f, rect.height * 0.22f), skin);
            DrawRect(new Rect(rect.x + rect.width * 0.68f + sway, rect.y + rect.height * 0.42f, rect.width * 0.08f, rect.height * 0.22f), skin);
            DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.67f, rect.width * 0.44f, rect.height * 0.055f), strap);
            DrawRect(new Rect(rect.x + rect.width * 0.47f, rect.y + rect.height * 0.66f, rect.width * 0.07f, rect.height * 0.08f), Color.Lerp(gold, accent, 0.20f));
            DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.83f, rect.width * 0.19f, rect.height * 0.05f), boot);
            DrawRect(new Rect(rect.x + rect.width * 0.54f, rect.y + rect.height * 0.83f, rect.width * 0.19f, rect.height * 0.05f), boot);

            if (seed % 3 == 0)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.46f, rect.y + rect.height * 0.38f, rect.width * 0.08f, rect.height * 0.29f), Color.Lerp(accent, cursorWhite, 0.18f));
            }
            else if (seed % 3 == 1)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.31f, rect.y + rect.height * 0.38f, rect.width * 0.12f, rect.height * 0.08f), Color.Lerp(accent, gold, 0.16f));
                DrawRect(new Rect(rect.x + rect.width * 0.57f, rect.y + rect.height * 0.38f, rect.width * 0.12f, rect.height * 0.08f), Color.Lerp(accent, gold, 0.16f));
            }
            else
            {
                DrawRect(new Rect(rect.x + rect.width * 0.33f, rect.y + rect.height * 0.36f, rect.width * 0.34f, rect.height * 0.05f), Color.Lerp(accent, Color.black, 0.24f));
            }

            if (seed % 7 == 0 && !armor.Contains("helm"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.42f, rect.y + rect.height * 0.27f, rect.width * 0.16f, rect.height * 0.10f), Color.Lerp(hair, Hex("101619"), 0.12f));
            }
            if (armor.Contains("cloak") || armor.Contains("mantle"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.45f, rect.width * 0.09f, rect.height * 0.30f), Color.Lerp(accent, Color.black, 0.34f));
                DrawRect(new Rect(rect.x + rect.width * 0.71f, rect.y + rect.height * 0.45f, rect.width * 0.09f, rect.height * 0.30f), Color.Lerp(accent, Color.black, 0.34f));
            }
            if (weapon.Contains("throwing"))
            {
                for (int i = 0; i < 3; i++) DrawRect(new Rect(rect.x + rect.width * (0.18f + i * 0.055f), rect.y + rect.height * 0.36f, rect.width * 0.035f, rect.height * 0.16f), cursorWhite);
            }
        }

        private void DrawRoleAccessoryFlair(Rect rect, string role, int seed, string armor, string weapon, Color accent)
        {
            Color detail = Color.Lerp(accent, cursorWhite, 0.20f);
            if (role == "bow")
            {
                for (int i = 0; i < 3; i++) DrawRect(new Rect(rect.x + rect.width * (0.70f + i * 0.035f), rect.y + rect.height * 0.25f, rect.width * 0.025f, rect.height * 0.16f), detail);
            }
            else if (role == "knife")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.62f, rect.width * 0.28f, rect.height * 0.04f), Hex("101619", 0.72f));
                DrawRect(new Rect(rect.x + rect.width * 0.58f, rect.y + rect.height * 0.58f, rect.width * 0.10f, rect.height * 0.035f), cursorWhite);
            }
            else if (role == "mender")
            {
                DrawPixelCross(new Rect(rect.x + rect.width * 0.43f, rect.y + rect.height * 0.43f, rect.width * 0.14f, rect.height * 0.14f), Color.Lerp(teal, cursorWhite, 0.16f));
            }
            else if (role == "ember")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.61f, rect.y + rect.height * 0.28f, rect.width * 0.08f, rect.height * 0.12f), ember);
                DrawRect(new Rect(rect.x + rect.width * 0.64f, rect.y + rect.height * 0.22f, rect.width * 0.04f, rect.height * 0.08f), gold);
            }
            else if (role == "hex")
            {
                DrawBorder(new Rect(rect.x + rect.width * 0.61f, rect.y + rect.height * 0.26f, rect.width * 0.12f, rect.height * 0.12f), violet, 1);
                DrawRect(new Rect(rect.x + rect.width * 0.65f, rect.y + rect.height * 0.31f, rect.width * 0.04f, rect.height * 0.04f), blood);
            }
            else if (role == "pike")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.63f, rect.y + rect.height * 0.59f, rect.width * 0.20f, rect.height * 0.04f), detail);
            }

            if (armor.Contains("circlet"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.38f, rect.y + rect.height * 0.17f, rect.width * 0.24f, rect.height * 0.035f), MaterialTint(armor, gold));
            }
            if (seed % 11 == 0)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.59f, rect.y + rect.height * 0.26f, rect.width * 0.04f, rect.height * 0.04f), detail);
            }
        }

        private void DrawEquipmentOverlay(Rect rect, string role, string weaponName, string armorName, string damageType)
        {
            string armor = GearKind(armorName, role, false);
            string weapon = GearKind(weaponName, role, true);
            Color steel = MaterialTint(weapon + " " + armor, Hex("c8c8b8"));
            Color haft = MaterialTint(weapon, Hex("9b6b45"));
            Color leather = MaterialTint(armor, Hex("7c5a3d"));
            Color plate = MaterialTint(armor, Hex("a9b0a2"));
            Color magic = DamageColor(string.IsNullOrEmpty(damageType) ? "physical" : damageType);

            if (armor.Contains("plate"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.31f, rect.y + rect.height * 0.39f, rect.width * 0.38f, rect.height * 0.25f), plate);
                DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.18f, rect.width * 0.28f, rect.height * 0.08f), plate);
            }
            else if (armor.Contains("chain"))
            {
                for (int i = 0; i < 4; i++) DrawRect(new Rect(rect.x + rect.width * (0.32f + i * 0.09f), rect.y + rect.height * 0.43f, rect.width * 0.05f, rect.height * 0.23f), plate);
            }
            else if (armor.Contains("leather"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.30f, rect.y + rect.height * 0.51f, rect.width * 0.40f, rect.height * 0.09f), leather);
                DrawRect(new Rect(rect.x + rect.width * 0.46f, rect.y + rect.height * 0.37f, rect.width * 0.08f, rect.height * 0.34f), leather);
            }
            else if (armor.Contains("robe"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.60f, rect.width * 0.44f, rect.height * 0.12f), Color.Lerp(magic, Color.black, 0.12f));
            }
            else if (armor.Contains("mantle"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.22f, rect.y + rect.height * 0.35f, rect.width * 0.56f, rect.height * 0.12f), MaterialTint(armor, Hex("d9d3c4")));
            }
            else if (armor.Contains("greaves"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.33f, rect.y + rect.height * 0.69f, rect.width * 0.13f, rect.height * 0.18f), plate);
                DrawRect(new Rect(rect.x + rect.width * 0.55f, rect.y + rect.height * 0.69f, rect.width * 0.13f, rect.height * 0.18f), plate);
            }
            if (armor.Contains("shield") || weapon.Contains("shield") || role == "shield" || role == "ward")
            {
                Rect shield = new Rect(rect.x + rect.width * 0.12f, rect.y + rect.height * 0.42f, rect.width * 0.22f, rect.height * 0.34f);
                DrawRect(shield, Color.Lerp(plate, magic, 0.16f));
                DrawBorder(shield, gold, 1);
                DrawRect(new Rect(shield.x + shield.width * 0.40f, shield.y + shield.height * 0.14f, shield.width * 0.18f, shield.height * 0.66f), Hex("101619", 0.45f));
            }

            if (weapon.Contains("bow") || weapon.Contains("crossbow"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.15f, rect.y + rect.height * 0.25f, rect.width * 0.06f, rect.height * 0.55f), haft);
                DrawRect(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.30f, rect.width * 0.04f, rect.height * 0.45f), steel);
                if (weapon.Contains("crossbow")) DrawRect(new Rect(rect.x + rect.width * 0.63f, rect.y + rect.height * 0.43f, rect.width * 0.25f, rect.height * 0.06f), steel);
            }
            else if (weapon.Contains("spear") || weapon.Contains("halberd") || role == "pike")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.76f, rect.y + rect.height * 0.12f, rect.width * 0.05f, rect.height * 0.72f), haft);
                DrawRect(new Rect(rect.x + rect.width * 0.70f, rect.y + rect.height * 0.11f, rect.width * 0.17f, rect.height * 0.08f), steel);
                if (weapon.Contains("halberd")) DrawRect(new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.18f, rect.width * 0.12f, rect.height * 0.12f), steel);
            }
            else if (weapon.Contains("staff") || weapon.Contains("ritual") || role == "mender" || role == "ember" || role == "hex")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.24f, rect.width * 0.06f, rect.height * 0.57f), haft);
                DrawRect(new Rect(rect.x + rect.width * 0.13f, rect.y + rect.height * 0.18f, rect.width * 0.16f, rect.height * 0.13f), magic);
                DrawRect(new Rect(rect.x + rect.width * 0.68f, rect.y + rect.height * 0.43f, rect.width * 0.18f, rect.height * 0.08f), magic);
            }
            else if (weapon.Contains("axe") || weapon.Contains("flail") || weapon.Contains("mace"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.72f, rect.y + rect.height * 0.28f, rect.width * 0.06f, rect.height * 0.46f), haft);
                DrawRect(new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.25f, rect.width * 0.18f, rect.height * 0.13f), steel);
                if (weapon.Contains("flail")) DrawRect(new Rect(rect.x + rect.width * 0.62f, rect.y + rect.height * 0.52f, rect.width * 0.12f, rect.height * 0.12f), steel);
            }
            else
            {
                float blade = weapon.Contains("knife") || weapon.Contains("epee") || weapon.Contains("sabre") ? 0.24f : 0.42f;
                DrawRect(new Rect(rect.x + rect.width * 0.72f, rect.y + rect.height * 0.31f, rect.width * 0.06f, rect.height * blade), steel);
                DrawRect(new Rect(rect.x + rect.width * 0.68f, rect.y + rect.height * 0.58f, rect.width * 0.14f, rect.height * 0.05f), gold);
                if (weapon.Contains("epee")) DrawRect(new Rect(rect.x + rect.width * 0.75f, rect.y + rect.height * 0.28f, rect.width * 0.03f, rect.height * 0.10f), steel);
            }
            DrawArmorPolish(rect, armor, plate, leather, magic);
            DrawWeaponPolish(rect, weapon, steel, haft, magic);
            DrawGearTraitMarks(rect, weapon + " " + armor, magic);
        }

        private void DrawArmorPolish(Rect rect, string armor, Color plate, Color leather, Color magic)
        {
            if (armor.Contains("scale"))
            {
                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        DrawRect(new Rect(rect.x + rect.width * (0.34f + col * 0.10f + row * 0.018f), rect.y + rect.height * (0.43f + row * 0.07f), rect.width * 0.055f, rect.height * 0.035f), Color.Lerp(plate, Hex("101619"), row * 0.08f));
                    }
                }
            }
            if (armor.Contains("mail") || armor.Contains("chain"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.32f, rect.y + rect.height * 0.63f, rect.width * 0.36f, rect.height * 0.035f), Color.Lerp(plate, cursorWhite, 0.15f));
                DrawRect(new Rect(rect.x + rect.width * 0.35f, rect.y + rect.height * 0.48f, rect.width * 0.30f, rect.height * 0.035f), Hex("101619", 0.40f));
            }
            if (armor.Contains("padded"))
            {
                for (int i = 0; i < 3; i++) DrawRect(new Rect(rect.x + rect.width * (0.34f + i * 0.10f), rect.y + rect.height * 0.42f, rect.width * 0.035f, rect.height * 0.25f), Color.Lerp(leather, cursorWhite, 0.12f));
            }
            if (armor.Contains("scout") || armor.Contains("dark leathers") || armor.Contains("leather jack"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.33f, rect.y + rect.height * 0.40f, rect.width * 0.06f, rect.height * 0.28f), Color.Lerp(leather, Hex("101619"), 0.20f));
                DrawRect(new Rect(rect.x + rect.width * 0.61f, rect.y + rect.height * 0.40f, rect.width * 0.06f, rect.height * 0.28f), Color.Lerp(leather, Hex("101619"), 0.20f));
            }
            if (armor.Contains("robe") || armor.Contains("mantle") || armor.Contains("cloak"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.33f, rect.y + rect.height * 0.69f, rect.width * 0.34f, rect.height * 0.045f), Color.Lerp(magic, cursorWhite, 0.12f));
                DrawRect(new Rect(rect.x + rect.width * 0.46f, rect.y + rect.height * 0.43f, rect.width * 0.08f, rect.height * 0.25f), Color.Lerp(magic, Hex("101619"), 0.08f));
            }
            if (armor.Contains("helm"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.38f, rect.y + rect.height * 0.14f, rect.width * 0.24f, rect.height * 0.07f), plate);
                DrawRect(new Rect(rect.x + rect.width * 0.45f, rect.y + rect.height * 0.19f, rect.width * 0.10f, rect.height * 0.10f), Color.Lerp(plate, Hex("101619"), 0.16f));
            }
            if (armor.Contains("tower shield"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.15f, rect.y + rect.height * 0.48f, rect.width * 0.16f, rect.height * 0.24f), Color.Lerp(plate, magic, 0.10f));
                DrawRect(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.51f, rect.width * 0.05f, rect.height * 0.17f), Hex("101619", 0.45f));
            }
            else if (armor.Contains("buckler") || armor.Contains("kite shield"))
            {
                Rect smallShield = new Rect(rect.x + rect.width * 0.16f, rect.y + rect.height * 0.49f, rect.width * 0.15f, rect.height * 0.20f);
                DrawRect(smallShield, Color.Lerp(plate, leather, armor.Contains("buckler") ? 0.42f : 0.12f));
                DrawBorder(smallShield, Color.Lerp(gold, magic, 0.20f), 1);
            }
        }

        private void DrawWeaponPolish(Rect rect, string weapon, Color steel, Color haft, Color magic)
        {
            if (weapon.Contains("longbow") || weapon.Contains("crossbow") || weapon.Contains("sling"))
            {
                for (int i = 0; i < 3; i++) DrawRect(new Rect(rect.x + rect.width * (0.51f + i * 0.055f), rect.y + rect.height * 0.36f, rect.width * 0.035f, rect.height * 0.13f), steel);
                if (weapon.Contains("sling")) DrawRect(new Rect(rect.x + rect.width * 0.17f, rect.y + rect.height * 0.61f, rect.width * 0.18f, rect.height * 0.035f), haft);
            }
            if (weapon.Contains("glaive") || weapon.Contains("halberd"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.68f, rect.y + rect.height * 0.12f, rect.width * 0.08f, rect.height * 0.20f), steel);
                DrawRect(new Rect(rect.x + rect.width * 0.80f, rect.y + rect.height * 0.20f, rect.width * 0.05f, rect.height * 0.13f), steel);
            }
            if (weapon.Contains("war hammer"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.62f, rect.y + rect.height * 0.22f, rect.width * 0.25f, rect.height * 0.08f), steel);
                DrawRect(new Rect(rect.x + rect.width * 0.71f, rect.y + rect.height * 0.18f, rect.width * 0.06f, rect.height * 0.18f), steel);
            }
            if (weapon.Contains("broadsword") || weapon.Contains("arming sword") || weapon.Contains("short sword"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.735f, rect.y + rect.height * 0.28f, rect.width * 0.025f, rect.height * 0.30f), Color.Lerp(steel, cursorWhite, 0.18f));
            }
            if (weapon.Contains("sabre"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.79f, rect.y + rect.height * 0.33f, rect.width * 0.05f, rect.height * 0.22f), steel);
                DrawRect(new Rect(rect.x + rect.width * 0.65f, rect.y + rect.height * 0.56f, rect.width * 0.14f, rect.height * 0.06f), gold);
            }
            if (weapon.Contains("orb") || weapon.Contains("focus") || weapon.Contains("scepter") || weapon.Contains("bell"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.69f, rect.y + rect.height * 0.32f, rect.width * 0.11f, rect.height * 0.11f), Color.Lerp(magic, cursorWhite, 0.10f));
                DrawPixelCross(new Rect(rect.x + rect.width * 0.71f, rect.y + rect.height * 0.34f, rect.width * 0.07f, rect.height * 0.07f), Hex("101619", 0.52f));
                if (weapon.Contains("bell")) DrawRect(new Rect(rect.x + rect.width * 0.15f, rect.y + rect.height * 0.21f, rect.width * 0.14f, rect.height * 0.08f), gold);
            }
            if (weapon.Contains("throwing knives") || weapon.Contains("throwing darts"))
            {
                for (int i = 0; i < 3; i++) DrawRect(new Rect(rect.x + rect.width * (0.64f + i * 0.055f), rect.y + rect.height * (0.29f + i * 0.045f), rect.width * 0.08f, rect.height * 0.028f), steel);
            }
        }

        private void DrawEnemyCombatDetails(Rect rect, CombatUnit unit)
        {
            Color color = unit.Color.ToColor();
            if (unit.Range > 1)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.14f, rect.y + rect.height * 0.24f, rect.width * 0.05f, rect.height * 0.54f), Color.Lerp(color, ink, 0.18f));
                DrawRect(new Rect(rect.x + rect.width * 0.12f, rect.y + rect.height * 0.22f, rect.width * 0.12f, rect.height * 0.07f), DamageColor(unit.DamageType));
            }
            if (!string.IsNullOrEmpty(unit.StatusOnHit))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.68f, rect.y + rect.height * 0.22f, rect.width * 0.16f, rect.height * 0.11f), DamageColor(unit.DamageType));
            }
            if (!string.IsNullOrEmpty(unit.Resist))
            {
                DrawBorder(new Rect(rect.x + rect.width * 0.22f, rect.y + rect.height * 0.37f, rect.width * 0.56f, rect.height * 0.34f), frost, 1);
            }
            DrawEnemyTraitMarks(rect, unit, color);
        }

        private void DrawEnemyDepthVariant(Rect rect, CombatUnit unit)
        {
            int tier = Mathf.Clamp(state == null ? 0 : state.Depth / 2, 0, 4);
            if (unit.Rank == "veteran") tier = Mathf.Max(tier, 2);
            if (unit.Rank == "elite") tier = Mathf.Max(tier, 3);
            if (tier <= 0) return;
            Color mark = Color.Lerp(unit.Color.ToColor(), DamageColor(unit.DamageType), 0.45f);
            for (int i = 0; i < tier; i++)
            {
                DrawRect(new Rect(rect.x + rect.width * (0.18f + i * 0.14f), rect.y + rect.height * 0.12f, rect.width * 0.08f, rect.height * 0.05f), mark);
            }
            if (tier >= 2)
            {
                DrawBorder(new Rect(rect.x + rect.width * 0.25f, rect.y + rect.height * 0.39f, rect.width * 0.50f, rect.height * 0.30f), Color.Lerp(mark, cursorWhite, 0.18f), 1);
            }
            if (tier >= 3)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.11f, rect.y + rect.height * 0.70f, rect.width * 0.78f, rect.height * 0.05f), Hex("101619", 0.70f));
                DrawRect(new Rect(rect.x + rect.width * 0.44f, rect.y + rect.height * 0.18f, rect.width * 0.12f, rect.height * 0.08f), mark);
            }
        }

        private void DrawEnemyVariantOverlay(Rect rect, CombatUnit unit)
        {
            int seed = StableSeed(unit.Id + unit.Name + unit.Role + unit.Rank);
            Color color = unit.Color.ToColor();
            Color mark = Color.Lerp(DamageColor(unit.DamageType), cursorWhite, 0.08f);
            if (seed % 2 == 0)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.31f, rect.y + rect.height * 0.57f, rect.width * 0.38f, rect.height * 0.032f), Hex("101619", 0.55f));
            }
            else
            {
                DrawRect(new Rect(rect.x + rect.width * 0.43f, rect.y + rect.height * 0.27f, rect.width * 0.15f, rect.height * 0.032f), mark);
            }

            if (unit.Role == "husk")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.63f, rect.width * 0.44f, rect.height * 0.04f), Color.Lerp(Hex("a9b0a2"), color, 0.40f));
            }
            else if (unit.Role == "reaver")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.70f, rect.width * 0.16f, rect.height * 0.04f), blood);
                DrawRect(new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.70f, rect.width * 0.16f, rect.height * 0.04f), blood);
            }
            else if (unit.Role == "spore")
            {
                for (int i = 0; i < 4; i++) DrawRect(new Rect(rect.x + rect.width * (0.28f + i * 0.12f), rect.y + rect.height * (0.29f + (i % 2) * 0.09f), rect.width * 0.055f, rect.height * 0.055f), Color.Lerp(poison, cursorWhite, 0.10f));
            }
            else if (unit.Role == "shade")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.23f, rect.y + rect.height * 0.73f, rect.width * 0.54f, rect.height * 0.04f), Hex("080b0d", 0.78f));
                DrawRect(new Rect(rect.x + rect.width * 0.30f, rect.y + rect.height * 0.78f, rect.width * 0.40f, rect.height * 0.035f), violet);
            }
            else if (unit.Role == "glassmage")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.35f, rect.y + rect.height * 0.48f, rect.width * 0.30f, rect.height * 0.035f), Hex("d6f4ff"));
                DrawRect(new Rect(rect.x + rect.width * 0.48f, rect.y + rect.height * 0.22f, rect.width * 0.04f, rect.height * 0.19f), Hex("d6f4ff"));
            }
            else if (unit.Role == "thornbeast")
            {
                for (int i = 0; i < 4; i++) DrawRect(new Rect(rect.x + rect.width * (0.20f + i * 0.16f), rect.y + rect.height * 0.31f, rect.width * 0.04f, rect.height * 0.16f), Color.Lerp(gold, color, 0.25f));
            }
            else if (unit.Role == "mirearcher")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.70f, rect.y + rect.height * 0.35f, rect.width * 0.14f, rect.height * 0.04f), poison);
                DrawRect(new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.58f, rect.width * 0.18f, rect.height * 0.035f), poison);
            }
            else if (unit.Role == "bonepriest")
            {
                for (int i = 0; i < 3; i++) DrawRect(new Rect(rect.x + rect.width * (0.39f + i * 0.085f), rect.y + rect.height * 0.51f, rect.width * 0.04f, rect.height * 0.13f), Hex("d9d3c4"));
            }
            else if (unit.Role == "cinderling")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.17f, rect.width * 0.07f, rect.height * 0.10f), gold);
                DrawRect(new Rect(rect.x + rect.width * 0.57f, rect.y + rect.height * 0.17f, rect.width * 0.07f, rect.height * 0.10f), gold);
            }
            else if (unit.Role == "gloamknight")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.45f, rect.width * 0.32f, rect.height * 0.055f), Hex("a9b0a2"));
                DrawRect(new Rect(rect.x + rect.width * 0.40f, rect.y + rect.height * 0.26f, rect.width * 0.20f, rect.height * 0.04f), blood);
            }
        }

        private void DrawEnemyTraitMarks(Rect rect, CombatUnit unit, Color color)
        {
            Color type = DamageColor(unit.DamageType);
            if (!string.IsNullOrEmpty(unit.Weakness))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.76f, rect.y + rect.height * 0.72f, rect.width * 0.10f, rect.height * 0.035f), Color.Lerp(type, Hex("101619"), 0.25f));
            }
            if (unit.MagicResist >= 3)
            {
                DrawBorder(new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.42f, rect.width * 0.32f, rect.height * 0.22f), Color.Lerp(violet, cursorWhite, 0.12f), 1);
            }
            if (unit.Fearless)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.40f, rect.y + rect.height * 0.18f, rect.width * 0.20f, rect.height * 0.035f), Color.Lerp(blood, color, 0.15f));
            }
            if (unit.StatusOnHit == "bleed")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.70f, rect.y + rect.height * 0.66f, rect.width * 0.16f, rect.height * 0.035f), blood);
            }
            else if (unit.StatusOnHit == "poison")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.70f, rect.y + rect.height * 0.66f, rect.width * 0.16f, rect.height * 0.035f), poison);
            }
        }

        private void DrawStatusOverlay(Rect rect, CombatUnit unit)
        {
            if (unit.Guarding) DrawBorder(Pad(rect, rect.width * 0.05f), teal, 2);
            if (unit.Poisoned > 0) DrawRect(new Rect(rect.x, rect.yMax - rect.height * 0.08f, rect.width, rect.height * 0.04f), poison);
            if (unit.Bleeding > 0) DrawRect(new Rect(rect.x, rect.y + rect.height * 0.22f, rect.width, rect.height * 0.035f), blood);
            if (unit.Webbed > 0)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.34f, rect.width * 0.64f, rect.height * 0.04f), Hex("d9d3c4", 0.82f));
                DrawRect(new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * 0.62f, rect.width * 0.52f, rect.height * 0.04f), Hex("d9d3c4", 0.72f));
            }
            if (unit.Shielded > 0) DrawBorder(Pad(rect, rect.width * 0.12f), frost, 1);
            if (unit.Regenerating > 0) DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.18f, rect.width * 0.10f, rect.height * 0.10f), Color.Lerp(teal, cursorWhite, 0.20f));
            if (unit.Hexed > 0) DrawBorder(Pad(rect, rect.width * 0.18f), violet, 1);
            if (unit.Stunned > 0 || unit.Sleeping > 0) DrawRect(new Rect(rect.x + rect.width * 0.70f, rect.y + rect.height * 0.12f, rect.width * 0.12f, rect.height * 0.12f), unit.Stunned > 0 ? gold : violet);
        }

        private string GearKind(string name, string role, bool weapon)
        {
            string text = (name ?? "").ToLowerInvariant();
            if (string.IsNullOrEmpty(text))
            {
                if (!weapon) return role == "shield" || role == "ward" ? "shield leather" : role == "ember" || role == "hex" || role == "mender" ? "robe" : "leather";
                if (role == "bow") return "bow";
                if (role == "pike") return "spear";
                if (role == "knife") return "knife";
                if (role == "ember" || role == "hex" || role == "mender") return "staff";
                return role == "shield" || role == "ward" ? "sword shield" : "sword";
            }
            return text;
        }

        private int StableSeed(string text)
        {
            unchecked
            {
                int hash = 23;
                string source = string.IsNullOrEmpty(text) ? "ashen" : text;
                for (int i = 0; i < source.Length; i++) hash = hash * 31 + source[i];
                return hash & 0x7fffffff;
            }
        }

        private Color VariantSkin(int seed)
        {
            Color[] tones = { Hex("d9a67b"), Hex("f0c18e"), Hex("b9815d"), Hex("8f5f45"), Hex("c99a72"), Hex("e1b89b") };
            return tones[seed % tones.Length];
        }

        private Color VariantHair(int seed)
        {
            Color[] tones = { Hex("2f211b"), Hex("5e3f2b"), Hex("9b6b45"), Hex("d7a84e"), Hex("a9b0a2"), Hex("101619") };
            return tones[(seed / 7) % tones.Length];
        }

        private Color MaterialTint(string text, Color fallback)
        {
            string value = (text ?? "").ToLowerInvariant();
            if (value.Contains("mithril")) return Hex("d6f4ff");
            if (value.Contains("adamantine")) return Hex("7f9d5b");
            if (value.Contains("blackglass") || value.Contains("obsidian")) return Hex("382c46");
            if (value.Contains("stormglass")) return Hex("9ad6e8");
            if (value.Contains("moonstone") || value.Contains("silver")) return Hex("d9d3c4");
            if (value.Contains("crystalline")) return Hex("b8e4e6");
            if (value.Contains("ashwood") || value.Contains("ironwood")) return Hex("8a5c35");
            if (value.Contains("bone")) return Hex("d8c7a3");
            if (value.Contains("silk")) return Hex("8d6dcc");
            if (value.Contains("fine steel")) return Hex("c8c8b8");
            return fallback;
        }

        private void DrawGearTraitMarks(Rect rect, string text, Color magic)
        {
            string value = (text ?? "").ToLowerInvariant();
            if (value.Contains("flame") || value.Contains("frost") || value.Contains("storm") || value.Contains("venom") || value.Contains("terror") || value.Contains("night"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.80f, rect.y + rect.height * 0.22f, rect.width * 0.08f, rect.height * 0.08f), magic);
                DrawRect(new Rect(rect.x + rect.width * 0.82f, rect.y + rect.height * 0.31f, rect.width * 0.04f, rect.height * 0.08f), magic);
            }
            if (value.Contains("warding") || value.Contains("holy") || value.Contains("anti-magic"))
            {
                DrawBorder(new Rect(rect.x + rect.width * 0.35f, rect.y + rect.height * 0.40f, rect.width * 0.30f, rect.height * 0.22f), Color.Lerp(teal, cursorWhite, 0.18f), 1);
            }
            if (value.Contains("vampiric") || value.Contains("thorns"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.16f, rect.y + rect.height * 0.72f, rect.width * 0.68f, rect.height * 0.04f), blood);
            }
            if (value.Contains("bleeding") || value.Contains("vicious") || value.Contains("epee") || value.Contains("sabre"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.72f, rect.y + rect.height * 0.47f, rect.width * 0.11f, rect.height * 0.035f), blood);
            }
            if (value.Contains("stunning") || value.Contains("storm"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.76f, rect.y + rect.height * 0.18f, rect.width * 0.05f, rect.height * 0.05f), gold);
                DrawRect(new Rect(rect.x + rect.width * 0.83f, rect.y + rect.height * 0.24f, rect.width * 0.05f, rect.height * 0.05f), gold);
            }
            if (value.Contains("focus") || value.Contains("echoes") || value.Contains("silence"))
            {
                DrawBorder(new Rect(rect.x + rect.width * 0.62f, rect.y + rect.height * 0.22f, rect.width * 0.16f, rect.height * 0.16f), Color.Lerp(magic, cursorWhite, 0.20f), 1);
            }
            if (value.Contains("haste") || value.Contains("keen") || value.Contains("weightless"))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.19f, rect.y + rect.height * 0.79f, rect.width * 0.20f, rect.height * 0.035f), Color.Lerp(teal, cursorWhite, 0.20f));
                DrawRect(new Rect(rect.x + rect.width * 0.61f, rect.y + rect.height * 0.79f, rect.width * 0.20f, rect.height * 0.035f), Color.Lerp(teal, cursorWhite, 0.20f));
            }
        }

        private void DrawPartyFigure(Rect rect, string role, Color color)
        {
            Color dark = Color.Lerp(color, Color.black, 0.45f);
            Color light = Color.Lerp(color, ink, 0.22f);
            Color trim = Color.Lerp(gold, color, 0.18f);
            Color skin = Hex("d9a67b");
            Rect head = new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.14f, rect.width * 0.28f, rect.height * 0.20f);
            Rect torso = new Rect(rect.x + rect.width * 0.29f, rect.y + rect.height * 0.37f, rect.width * 0.42f, rect.height * 0.34f);
            DrawRect(new Rect(rect.x + rect.width * 0.25f, rect.y + rect.height * 0.34f, rect.width * 0.50f, rect.height * 0.42f), Hex("030405", 0.70f));
            DrawRect(Pad(head, -rect.width * 0.025f), Hex("030405", 0.82f));
            DrawRect(Pad(torso, -rect.width * 0.025f), Hex("030405", 0.82f));
            DrawRect(new Rect(rect.x + rect.width * 0.35f, rect.y + rect.height * 0.72f, rect.width * 0.10f, rect.height * 0.16f), dark);
            DrawRect(new Rect(rect.x + rect.width * 0.56f, rect.y + rect.height * 0.72f, rect.width * 0.10f, rect.height * 0.16f), dark);
            DrawRect(head, skin);
            DrawRect(new Rect(head.x, head.y, head.width, head.height * 0.32f), role == "hex" ? violet : role == "mender" ? teal : dark);
            DrawRect(torso, color);
            DrawRect(new Rect(torso.x + torso.width * 0.16f, torso.y + torso.height * 0.18f, torso.width * 0.68f, torso.height * 0.14f), light);
            DrawRect(new Rect(torso.x + torso.width * 0.16f, torso.y + torso.height * 0.72f, torso.width * 0.68f, torso.height * 0.08f), Color.Lerp(color, cursorWhite, 0.30f));
            DrawRect(new Rect(head.x + head.width * 0.58f, head.y + head.height * 0.42f, head.width * 0.12f, head.height * 0.10f), Hex("050708"));

            if (role == "shield" || role == "ward")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.15f, rect.y + rect.height * 0.41f, rect.width * 0.22f, rect.height * 0.34f), dark);
                DrawBorder(new Rect(rect.x + rect.width * 0.15f, rect.y + rect.height * 0.41f, rect.width * 0.22f, rect.height * 0.34f), trim, 1);
                DrawRect(new Rect(rect.x + rect.width * 0.69f, rect.y + rect.height * 0.28f, rect.width * 0.07f, rect.height * 0.50f), trim);
                if (role == "ward") DrawRect(new Rect(rect.x + rect.width * 0.39f, rect.y + rect.height * 0.47f, rect.width * 0.22f, rect.height * 0.08f), cursorWhite);
            }
            else if (role == "pike")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.73f, rect.y + rect.height * 0.12f, rect.width * 0.06f, rect.height * 0.72f), trim);
                DrawRect(new Rect(rect.x + rect.width * 0.68f, rect.y + rect.height * 0.12f, rect.width * 0.16f, rect.height * 0.08f), cursorWhite);
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.49f, rect.width * 0.18f, rect.height * 0.08f), dark);
            }
            else if (role == "bow")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.17f, rect.y + rect.height * 0.28f, rect.width * 0.07f, rect.height * 0.52f), trim);
                DrawRect(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.28f, rect.width * 0.07f, rect.height * 0.10f), cursorWhite);
                DrawRect(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.70f, rect.width * 0.07f, rect.height * 0.10f), cursorWhite);
                DrawRect(new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.38f, rect.width * 0.18f, rect.height * 0.05f), trim);
                DrawRect(new Rect(rect.x + rect.width * 0.69f, rect.y + rect.height * 0.46f, rect.width * 0.18f, rect.height * 0.05f), trim);
            }
            else if (role == "knife")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.46f, rect.width * 0.20f, rect.height * 0.07f), cursorWhite);
                DrawRect(new Rect(rect.x + rect.width * 0.65f, rect.y + rect.height * 0.43f, rect.width * 0.19f, rect.height * 0.07f), cursorWhite);
                DrawRect(new Rect(torso.x + torso.width * 0.18f, torso.y + torso.height * 0.52f, torso.width * 0.64f, torso.height * 0.10f), dark);
            }
            else if (role == "mender")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.28f, rect.width * 0.07f, rect.height * 0.52f), teal);
                DrawRect(new Rect(rect.x + rect.width * 0.13f, rect.y + rect.height * 0.30f, rect.width * 0.21f, rect.height * 0.06f), cursorWhite);
                DrawRect(new Rect(torso.x + torso.width * 0.42f, torso.y + torso.height * 0.05f, torso.width * 0.16f, torso.height * 0.82f), cursorWhite);
                DrawRect(new Rect(torso.x + torso.width * 0.25f, torso.y + torso.height * 0.33f, torso.width * 0.50f, torso.height * 0.12f), cursorWhite);
            }
            else if (role == "ember" || role == "hex")
            {
                Color magic = role == "ember" ? ember : violet;
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.26f, rect.width * 0.08f, rect.height * 0.55f), trim);
                DrawRect(new Rect(rect.x + rect.width * 0.14f, rect.y + rect.height * 0.20f, rect.width * 0.16f, rect.height * 0.13f), magic);
                DrawRect(new Rect(rect.x + rect.width * 0.67f, rect.y + rect.height * 0.44f, rect.width * 0.18f, rect.height * 0.08f), magic);
                DrawRect(new Rect(torso.x + torso.width * 0.18f, torso.y + torso.height * 0.10f, torso.width * 0.64f, torso.height * 0.10f), dark);
            }
            else
            {
                DrawRect(new Rect(rect.x + rect.width * 0.69f, rect.y + rect.height * 0.30f, rect.width * 0.08f, rect.height * 0.44f), trim);
            }
        }

        private void DrawSummonFigure(Rect rect, string role, Color color)
        {
            Color dark = Color.Lerp(color, Color.black, 0.55f);
            Color hot = Color.Lerp(color, ember, 0.35f);
            Color glow = Color.Lerp(gold, color, 0.45f);
            DrawRect(new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.24f, rect.width * 0.32f, rect.height * 0.20f), dark);
            DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.42f, rect.width * 0.44f, rect.height * 0.30f), color);
            DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.36f, rect.width * 0.18f, rect.height * 0.08f), hot);
            DrawRect(new Rect(rect.x + rect.width * 0.64f, rect.y + rect.height * 0.36f, rect.width * 0.18f, rect.height * 0.08f), hot);
            DrawRect(new Rect(rect.x + rect.width * 0.22f, rect.y + rect.height * 0.56f, rect.width * 0.16f, rect.height * 0.07f), dark);
            DrawRect(new Rect(rect.x + rect.width * 0.62f, rect.y + rect.height * 0.56f, rect.width * 0.16f, rect.height * 0.07f), dark);
            DrawRect(new Rect(rect.x + rect.width * 0.32f, rect.y + rect.height * 0.16f, rect.width * 0.10f, rect.height * 0.12f), glow);
            DrawRect(new Rect(rect.x + rect.width * 0.58f, rect.y + rect.height * 0.16f, rect.width * 0.10f, rect.height * 0.12f), glow);
            DrawRect(new Rect(rect.x + rect.width * 0.42f, rect.y + rect.height * 0.31f, rect.width * 0.05f, rect.height * 0.06f), gold);
            DrawRect(new Rect(rect.x + rect.width * 0.54f, rect.y + rect.height * 0.31f, rect.width * 0.05f, rect.height * 0.06f), gold);
            DrawRect(new Rect(rect.x + rect.width * 0.38f, rect.y + rect.height * 0.72f, rect.width * 0.24f, rect.height * 0.08f), Hex("050708", 0.72f));
        }

        private bool IsEnemyRole(string role)
        {
            return role == "sentry" || role == "adept" || role == "husk" || role == "reaver" || role == "spore" || role == "shade" || role == "glassmage" || role == "thornbeast" || role == "mirearcher" || role == "bonepriest" || role == "cinderling" || role == "gloamknight" || role == "koboldraider" || role == "koboldslinger" || role == "koboldshaman" || role == "koboldwizard" || role == "koboldshield" || role == "koboldking" || role == "sewerrat" || role == "giantrat" || role == "ratfolk" || role == "ratcutthroat" || role == "ratmage" || role == "ratcleric" || role == "ratbrute" || role == "drowscout" || role == "drowblade" || role == "drowcrossbow" || role == "drowmage" || role == "drowpriest" || role == "lesserdemon" || role == "greaterdemon";
        }

        private void DrawEnemyFigure(Rect rect, string role, Color color)
        {
            Color dark = Color.Lerp(color, Color.black, 0.48f);
            Color light = Color.Lerp(color, ink, 0.22f);
            Color glow = Color.Lerp(gold, color, 0.28f);

            if (role == "sewerrat" || role == "giantrat")
            {
                DrawRatFigure(rect, role, color, dark, light);
            }
            else if (role.StartsWith("kobold"))
            {
                DrawKoboldFigure(rect, role, color, dark, light, glow);
            }
            else if (role == "husk")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.40f, rect.width * 0.64f, rect.height * 0.34f), dark);
                DrawRect(new Rect(rect.x + rect.width * 0.27f, rect.y + rect.height * 0.26f, rect.width * 0.46f, rect.height * 0.22f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.10f, rect.y + rect.height * 0.44f, rect.width * 0.20f, rect.height * 0.18f), light);
                DrawRect(new Rect(rect.x + rect.width * 0.70f, rect.y + rect.height * 0.44f, rect.width * 0.20f, rect.height * 0.18f), light);
                DrawRect(new Rect(rect.x + rect.width * 0.40f, rect.y + rect.height * 0.34f, rect.width * 0.20f, rect.height * 0.06f), Hex("101619"));
                DrawRect(new Rect(rect.x + rect.width * 0.32f, rect.y + rect.height * 0.55f, rect.width * 0.08f, rect.height * 0.16f), glow);
                DrawRect(new Rect(rect.x + rect.width * 0.58f, rect.y + rect.height * 0.50f, rect.width * 0.06f, rect.height * 0.20f), Hex("101619", 0.65f));
            }
            else if (role == "adept")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.30f, rect.y + rect.height * 0.23f, rect.width * 0.40f, rect.height * 0.23f), dark);
                DrawRect(new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * 0.42f, rect.width * 0.52f, rect.height * 0.34f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.38f, rect.y + rect.height * 0.31f, rect.width * 0.24f, rect.height * 0.10f), glow);
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.22f, rect.width * 0.07f, rect.height * 0.56f), light);
                DrawRect(new Rect(rect.x + rect.width * 0.13f, rect.y + rect.height * 0.18f, rect.width * 0.17f, rect.height * 0.08f), glow);
                DrawRect(new Rect(rect.x + rect.width * 0.64f, rect.y + rect.height * 0.50f, rect.width * 0.18f, rect.height * 0.06f), Hex("101619", 0.72f));
                DrawRect(new Rect(rect.x + rect.width * 0.30f, rect.y + rect.height * 0.70f, rect.width * 0.40f, rect.height * 0.06f), dark);
            }
            else if (role == "reaver")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.38f, rect.y + rect.height * 0.24f, rect.width * 0.24f, rect.height * 0.18f), light);
                DrawRect(new Rect(rect.x + rect.width * 0.31f, rect.y + rect.height * 0.42f, rect.width * 0.38f, rect.height * 0.34f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.23f, rect.y + rect.height * 0.48f, rect.width * 0.10f, rect.height * 0.22f), dark);
                DrawRect(new Rect(rect.x + rect.width * 0.67f, rect.y + rect.height * 0.48f, rect.width * 0.10f, rect.height * 0.22f), dark);
                DrawRect(new Rect(rect.x + rect.width * 0.13f, rect.y + rect.height * 0.66f, rect.width * 0.18f, rect.height * 0.06f), glow);
                DrawRect(new Rect(rect.x + rect.width * 0.69f, rect.y + rect.height * 0.66f, rect.width * 0.18f, rect.height * 0.06f), glow);
                DrawRect(new Rect(rect.x + rect.width * 0.42f, rect.y + rect.height * 0.31f, rect.width * 0.06f, rect.height * 0.06f), Hex("101619"));
                DrawRect(new Rect(rect.x + rect.width * 0.53f, rect.y + rect.height * 0.31f, rect.width * 0.06f, rect.height * 0.06f), Hex("101619"));
            }
            else if (role == "spore")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.32f, rect.y + rect.height * 0.42f, rect.width * 0.36f, rect.height * 0.30f), dark);
                DrawRect(new Rect(rect.x + rect.width * 0.23f, rect.y + rect.height * 0.26f, rect.width * 0.54f, rect.height * 0.24f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.31f, rect.y + rect.height * 0.18f, rect.width * 0.38f, rect.height * 0.16f), light);
                DrawRect(new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * 0.55f, rect.width * 0.12f, rect.height * 0.18f), glow);
                DrawRect(new Rect(rect.x + rect.width * 0.64f, rect.y + rect.height * 0.52f, rect.width * 0.12f, rect.height * 0.20f), glow);
                DrawRect(new Rect(rect.x + rect.width * 0.46f, rect.y + rect.height * 0.34f, rect.width * 0.08f, rect.height * 0.08f), Hex("101619"));
            }
            else if (role == "shade")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.18f, rect.width * 0.28f, rect.height * 0.16f), light);
                DrawRect(new Rect(rect.x + rect.width * 0.27f, rect.y + rect.height * 0.33f, rect.width * 0.46f, rect.height * 0.42f), dark);
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.50f, rect.width * 0.18f, rect.height * 0.08f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.64f, rect.y + rect.height * 0.50f, rect.width * 0.18f, rect.height * 0.08f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.41f, rect.y + rect.height * 0.26f, rect.width * 0.06f, rect.height * 0.06f), glow);
                DrawRect(new Rect(rect.x + rect.width * 0.54f, rect.y + rect.height * 0.26f, rect.width * 0.06f, rect.height * 0.06f), glow);
                DrawRect(new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.72f, rect.width * 0.32f, rect.height * 0.08f), Hex("101619", 0.55f));
            }
            else if (role == "glassmage")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.38f, rect.y + rect.height * 0.18f, rect.width * 0.24f, rect.height * 0.22f), light);
                DrawRect(new Rect(rect.x + rect.width * 0.29f, rect.y + rect.height * 0.41f, rect.width * 0.42f, rect.height * 0.33f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.22f, rect.width * 0.07f, rect.height * 0.56f), frost);
                DrawRect(new Rect(rect.x + rect.width * 0.15f, rect.y + rect.height * 0.18f, rect.width * 0.14f, rect.height * 0.10f), Hex("d6f4ff"));
                DrawRect(new Rect(rect.x + rect.width * 0.70f, rect.y + rect.height * 0.43f, rect.width * 0.12f, rect.height * 0.10f), glow);
                DrawRect(new Rect(rect.x + rect.width * 0.40f, rect.y + rect.height * 0.30f, rect.width * 0.20f, rect.height * 0.06f), Hex("101619"));
            }
            else if (role == "thornbeast")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * 0.42f, rect.width * 0.52f, rect.height * 0.28f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.25f, rect.width * 0.32f, rect.height * 0.22f), dark);
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.36f, rect.width * 0.14f, rect.height * 0.10f), light);
                DrawRect(new Rect(rect.x + rect.width * 0.68f, rect.y + rect.height * 0.36f, rect.width * 0.14f, rect.height * 0.10f), light);
                DrawRect(new Rect(rect.x + rect.width * 0.22f, rect.y + rect.height * 0.28f, rect.width * 0.08f, rect.height * 0.20f), glow);
                DrawRect(new Rect(rect.x + rect.width * 0.70f, rect.y + rect.height * 0.28f, rect.width * 0.08f, rect.height * 0.20f), glow);
                DrawRect(new Rect(rect.x + rect.width * 0.38f, rect.y + rect.height * 0.32f, rect.width * 0.06f, rect.height * 0.06f), Hex("101619"));
                DrawRect(new Rect(rect.x + rect.width * 0.56f, rect.y + rect.height * 0.32f, rect.width * 0.06f, rect.height * 0.06f), Hex("101619"));
            }
            else if (role == "mirearcher")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.25f, rect.width * 0.32f, rect.height * 0.18f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.30f, rect.y + rect.height * 0.43f, rect.width * 0.38f, rect.height * 0.31f), dark);
                DrawRect(new Rect(rect.x + rect.width * 0.16f, rect.y + rect.height * 0.22f, rect.width * 0.07f, rect.height * 0.58f), poison);
                DrawRect(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.24f, rect.width * 0.06f, rect.height * 0.50f), glow);
                DrawRect(new Rect(rect.x + rect.width * 0.62f, rect.y + rect.height * 0.48f, rect.width * 0.23f, rect.height * 0.05f), poison);
                DrawRect(new Rect(rect.x + rect.width * 0.42f, rect.y + rect.height * 0.32f, rect.width * 0.06f, rect.height * 0.06f), Hex("101619"));
                DrawRect(new Rect(rect.x + rect.width * 0.54f, rect.y + rect.height * 0.32f, rect.width * 0.06f, rect.height * 0.06f), Hex("101619"));
            }
            else if (role == "bonepriest")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.18f, rect.width * 0.28f, rect.height * 0.22f), Hex("d9d3c4"));
                DrawRect(new Rect(rect.x + rect.width * 0.29f, rect.y + rect.height * 0.42f, rect.width * 0.42f, rect.height * 0.34f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.24f, rect.width * 0.06f, rect.height * 0.56f), light);
                DrawRect(new Rect(rect.x + rect.width * 0.13f, rect.y + rect.height * 0.19f, rect.width * 0.16f, rect.height * 0.12f), teal);
                DrawRect(new Rect(rect.x + rect.width * 0.46f, rect.y + rect.height * 0.26f, rect.width * 0.08f, rect.height * 0.22f), Hex("101619", 0.65f));
                DrawRect(new Rect(rect.x + rect.width * 0.39f, rect.y + rect.height * 0.34f, rect.width * 0.22f, rect.height * 0.05f), Hex("101619"));
                DrawBorder(new Rect(rect.x + rect.width * 0.33f, rect.y + rect.height * 0.48f, rect.width * 0.34f, rect.height * 0.18f), teal, 1);
            }
            else if (role == "cinderling")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.31f, rect.y + rect.height * 0.32f, rect.width * 0.38f, rect.height * 0.36f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.18f, rect.width * 0.28f, rect.height * 0.23f), ember);
                DrawRect(new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * 0.54f, rect.width * 0.14f, rect.height * 0.10f), gold);
                DrawRect(new Rect(rect.x + rect.width * 0.62f, rect.y + rect.height * 0.54f, rect.width * 0.14f, rect.height * 0.10f), gold);
                DrawRect(new Rect(rect.x + rect.width * 0.44f, rect.y + rect.height * 0.26f, rect.width * 0.05f, rect.height * 0.06f), Hex("101619"));
                DrawRect(new Rect(rect.x + rect.width * 0.55f, rect.y + rect.height * 0.26f, rect.width * 0.05f, rect.height * 0.06f), Hex("101619"));
                DrawRect(new Rect(rect.x + rect.width * 0.38f, rect.y + rect.height * 0.70f, rect.width * 0.24f, rect.height * 0.08f), ember);
            }
            else if (role == "gloamknight")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * 0.38f, rect.width * 0.52f, rect.height * 0.36f), dark);
                DrawRect(new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.20f, rect.width * 0.32f, rect.height * 0.22f), color);
                DrawBorder(new Rect(rect.x + rect.width * 0.30f, rect.y + rect.height * 0.42f, rect.width * 0.40f, rect.height * 0.24f), Hex("a9b0a2"), 1);
                DrawRect(new Rect(rect.x + rect.width * 0.14f, rect.y + rect.height * 0.47f, rect.width * 0.18f, rect.height * 0.26f), Hex("46504d"));
                DrawRect(new Rect(rect.x + rect.width * 0.72f, rect.y + rect.height * 0.23f, rect.width * 0.06f, rect.height * 0.56f), blood);
                DrawRect(new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.24f, rect.width * 0.18f, rect.height * 0.07f), blood);
                DrawRect(new Rect(rect.x + rect.width * 0.40f, rect.y + rect.height * 0.31f, rect.width * 0.20f, rect.height * 0.05f), Hex("101619"));
            }
            else
            {
                DrawRect(new Rect(rect.x + rect.width * 0.32f, rect.y + rect.height * 0.24f, rect.width * 0.36f, rect.height * 0.20f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.26f, rect.y + rect.height * 0.45f, rect.width * 0.46f, rect.height * 0.30f), dark);
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.49f, rect.width * 0.18f, rect.height * 0.25f), Color.Lerp(color, ink, 0.08f));
                DrawBorder(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.49f, rect.width * 0.18f, rect.height * 0.25f), glow, 1);
                DrawRect(new Rect(rect.x + rect.width * 0.72f, rect.y + rect.height * 0.20f, rect.width * 0.06f, rect.height * 0.58f), glow);
                DrawRect(new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.20f, rect.width * 0.18f, rect.height * 0.06f), glow);
                DrawRect(new Rect(rect.x + rect.width * 0.39f, rect.y + rect.height * 0.34f, rect.width * 0.22f, rect.height * 0.05f), Hex("101619"));
            }
        }

        private void DrawRatFigure(Rect rect, string role, Color color, Color dark, Color light)
        {
            bool giant = role == "giantrat";
            float y = giant ? 0.48f : 0.54f;
            float bodyW = giant ? 0.58f : 0.48f;
            float bodyH = giant ? 0.28f : 0.22f;
            DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * y, rect.width * bodyW, rect.height * bodyH), dark);
            DrawRect(new Rect(rect.x + rect.width * 0.52f, rect.y + rect.height * (y - 0.11f), rect.width * 0.25f, rect.height * 0.18f), color);
            DrawRect(new Rect(rect.x + rect.width * 0.72f, rect.y + rect.height * (y - 0.04f), rect.width * 0.12f, rect.height * 0.055f), light);
            DrawRect(new Rect(rect.x + rect.width * 0.60f, rect.y + rect.height * (y - 0.16f), rect.width * 0.06f, rect.height * 0.09f), light);
            DrawRect(new Rect(rect.x + rect.width * 0.68f, rect.y + rect.height * (y - 0.15f), rect.width * 0.06f, rect.height * 0.08f), light);
            DrawRect(new Rect(rect.x + rect.width * 0.64f, rect.y + rect.height * (y - 0.03f), rect.width * 0.045f, rect.height * 0.045f), gold);
            DrawRect(new Rect(rect.x + rect.width * 0.08f, rect.y + rect.height * (y + 0.09f), rect.width * 0.20f, rect.height * 0.05f), color);
            DrawRect(new Rect(rect.x + rect.width * 0.03f, rect.y + rect.height * (y + 0.04f), rect.width * 0.09f, rect.height * 0.045f), color);
            DrawRect(new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * (y + bodyH), rect.width * 0.14f, rect.height * 0.045f), light);
            DrawRect(new Rect(rect.x + rect.width * 0.52f, rect.y + rect.height * (y + bodyH), rect.width * 0.13f, rect.height * 0.045f), light);
            if (giant)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.49f, rect.width * 0.12f, rect.height * 0.04f), poison);
                DrawRect(new Rect(rect.x + rect.width * 0.46f, rect.y + rect.height * 0.61f, rect.width * 0.10f, rect.height * 0.04f), blood);
            }
        }

        private void DrawKoboldFigure(Rect rect, string role, Color color, Color dark, Color light, Color glow)
        {
            Color scaleDark = Color.Lerp(color, Hex("1b2218"), 0.38f);
            Color belly = Color.Lerp(Hex("d8c7a3"), color, 0.26f);
            Color cloth = role == "koboldking" ? Hex("6a3f1d") : role == "koboldwizard" ? Hex("3a202b") : role == "koboldshaman" ? Hex("5f425e") : role == "koboldshield" ? Hex("7a4c35") : Hex("8a3d32");
            Color bone = Hex("d9d3c4");
            Color magic = role == "koboldwizard" ? blood : violet;

            DrawRect(new Rect(rect.x + rect.width * 0.25f, rect.y + rect.height * 0.47f, rect.width * 0.42f, rect.height * 0.27f), scaleDark);
            DrawRect(new Rect(rect.x + rect.width * 0.30f, rect.y + rect.height * 0.51f, rect.width * 0.28f, rect.height * 0.18f), belly);
            DrawRect(new Rect(rect.x + rect.width * 0.31f, rect.y + rect.height * 0.25f, rect.width * 0.35f, rect.height * 0.22f), color);
            DrawRect(new Rect(rect.x + rect.width * 0.55f, rect.y + rect.height * 0.30f, rect.width * 0.22f, rect.height * 0.12f), color);
            DrawRect(new Rect(rect.x + rect.width * 0.70f, rect.y + rect.height * 0.35f, rect.width * 0.10f, rect.height * 0.055f), scaleDark);
            DrawRect(new Rect(rect.x + rect.width * 0.38f, rect.y + rect.height * 0.22f, rect.width * 0.07f, rect.height * 0.13f), bone);
            DrawRect(new Rect(rect.x + rect.width * 0.55f, rect.y + rect.height * 0.21f, rect.width * 0.07f, rect.height * 0.13f), bone);
            DrawRect(new Rect(rect.x + rect.width * 0.52f, rect.y + rect.height * 0.32f, rect.width * 0.06f, rect.height * 0.055f), gold);
            DrawRect(new Rect(rect.x + rect.width * 0.62f, rect.y + rect.height * 0.32f, rect.width * 0.055f, rect.height * 0.05f), Hex("101619"));
            DrawRect(new Rect(rect.x + rect.width * 0.29f, rect.y + rect.height * 0.60f, rect.width * 0.38f, rect.height * 0.12f), cloth);
            DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.68f, rect.width * 0.23f, rect.height * 0.07f), scaleDark);
            DrawRect(new Rect(rect.x + rect.width * 0.54f, rect.y + rect.height * 0.69f, rect.width * 0.22f, rect.height * 0.07f), scaleDark);
            DrawRect(new Rect(rect.x + rect.width * 0.09f, rect.y + rect.height * 0.57f, rect.width * 0.20f, rect.height * 0.075f), scaleDark);
            DrawRect(new Rect(rect.x + rect.width * 0.04f, rect.y + rect.height * 0.52f, rect.width * 0.11f, rect.height * 0.06f), color);
            DrawRect(new Rect(rect.x + rect.width * 0.25f, rect.y + rect.height * 0.77f, rect.width * 0.15f, rect.height * 0.06f), bone);
            DrawRect(new Rect(rect.x + rect.width * 0.56f, rect.y + rect.height * 0.77f, rect.width * 0.15f, rect.height * 0.06f), bone);

            if (role == "koboldking")
            {
                Rect crown = new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.15f, rect.width * 0.31f, rect.height * 0.09f);
                DrawRect(crown, gold);
                DrawRect(new Rect(crown.x + crown.width * 0.10f, crown.y - crown.height * 0.55f, crown.width * 0.16f, crown.height * 0.70f), gold);
                DrawRect(new Rect(crown.x + crown.width * 0.42f, crown.y - crown.height * 0.80f, crown.width * 0.16f, crown.height * 0.95f), gold);
                DrawRect(new Rect(crown.x + crown.width * 0.74f, crown.y - crown.height * 0.55f, crown.width * 0.16f, crown.height * 0.70f), gold);
                Rect shield = new Rect(rect.x + rect.width * 0.63f, rect.y + rect.height * 0.37f, rect.width * 0.26f, rect.height * 0.34f);
                DrawRect(shield, Hex("5b3a25"));
                DrawBorder(shield, gold, 1);
                DrawRect(new Rect(shield.x + shield.width * 0.42f, shield.y + shield.height * 0.10f, shield.width * 0.16f, shield.height * 0.78f), bone);
                DrawRect(new Rect(rect.x + rect.width * 0.14f, rect.y + rect.height * 0.45f, rect.width * 0.21f, rect.height * 0.065f), bone);
                DrawRect(new Rect(rect.x + rect.width * 0.09f, rect.y + rect.height * 0.40f, rect.width * 0.12f, rect.height * 0.065f), blood);
            }
            else if (role == "koboldslinger")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.31f, rect.width * 0.055f, rect.height * 0.46f), Hex("9b6b45"));
                DrawRect(new Rect(rect.x + rect.width * 0.14f, rect.y + rect.height * 0.26f, rect.width * 0.13f, rect.height * 0.09f), bone);
                DrawRect(new Rect(rect.x + rect.width * 0.67f, rect.y + rect.height * 0.47f, rect.width * 0.18f, rect.height * 0.05f), glow);
            }
            else if (role == "koboldshaman" || role == "koboldwizard")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.20f, rect.width * 0.07f, rect.height * 0.58f), bone);
                DrawRect(new Rect(rect.x + rect.width * 0.12f, rect.y + rect.height * 0.14f, rect.width * 0.20f, rect.height * 0.13f), magic);
                DrawRect(new Rect(rect.x + rect.width * 0.43f, rect.y + rect.height * 0.24f, rect.width * 0.17f, rect.height * 0.13f), bone);
                DrawRect(new Rect(rect.x + rect.width * 0.47f, rect.y + rect.height * 0.29f, rect.width * 0.04f, rect.height * 0.04f), Hex("101619"));
                DrawRect(new Rect(rect.x + rect.width * 0.14f, rect.y + rect.height * 0.08f, rect.width * 0.16f, rect.height * 0.05f), bone);
                DrawRect(new Rect(rect.x + rect.width * 0.72f, rect.y + rect.height * 0.18f, rect.width * 0.08f, rect.height * 0.36f), magic);
                DrawRect(new Rect(rect.x + rect.width * 0.68f, rect.y + rect.height * 0.14f, rect.width * 0.16f, rect.height * 0.08f), Color.Lerp(magic, cursorWhite, 0.18f));
                if (role == "koboldwizard")
                {
                    DrawRect(new Rect(rect.x + rect.width * 0.33f, rect.y + rect.height * 0.56f, rect.width * 0.34f, rect.height * 0.05f), blood);
                    DrawRect(new Rect(rect.x + rect.width * 0.68f, rect.y + rect.height * 0.26f, rect.width * 0.18f, rect.height * 0.05f), gold);
                    DrawBorder(new Rect(rect.x + rect.width * 0.63f, rect.y + rect.height * 0.09f, rect.width * 0.26f, rect.height * 0.18f), blood, 1);
                }
                else
                {
                    DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.56f, rect.width * 0.40f, rect.height * 0.05f), poison.WithAlpha(0.72f));
                    DrawBorder(new Rect(rect.x + rect.width * 0.67f, rect.y + rect.height * 0.12f, rect.width * 0.20f, rect.height * 0.16f), violet, 1);
                }
            }
            else if (role == "koboldshield")
            {
                Rect shield = new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.42f, rect.width * 0.22f, rect.height * 0.30f);
                DrawRect(shield, Hex("5b3a25"));
                DrawBorder(shield, bone, 1);
                DrawRect(new Rect(shield.x + shield.width * 0.43f, shield.y + shield.height * 0.10f, shield.width * 0.14f, shield.height * 0.78f), Hex("101619", 0.46f));
                DrawRect(new Rect(rect.x + rect.width * 0.16f, rect.y + rect.height * 0.48f, rect.width * 0.20f, rect.height * 0.055f), bone);
            }
            else
            {
                DrawRect(new Rect(rect.x + rect.width * 0.68f, rect.y + rect.height * 0.46f, rect.width * 0.08f, rect.height * 0.28f), bone);
                DrawRect(new Rect(rect.x + rect.width * 0.64f, rect.y + rect.height * 0.42f, rect.width * 0.16f, rect.height * 0.055f), glow);
            }
        }

        private void DrawPixelPortrait(Rect rect, PartyMember member)
        {
            Color color = MemberColor(member);
            DrawRect(rect, Hex("101619"));
            DrawBorder(rect, Color.Lerp(color, gold, 0.25f), 2);
            DrawRect(new Rect(rect.x + rect.width * 0.08f, rect.y + rect.height * 0.69f, rect.width * 0.84f, rect.height * 0.12f), Hex("080b0d", 0.7f));
            DrawPartyPortraitSprite(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.04f, rect.width * 0.64f, rect.height * 0.78f), member, color);
            DrawSigil(new Rect(rect.x + rect.width * 0.08f, rect.y + rect.height * 0.12f, rect.width * 0.17f, rect.height * 0.16f), member.Sigil, color);
            DrawClassIcon(new Rect(rect.xMax - rect.width * 0.26f, rect.y + rect.height * 0.08f, rect.width * 0.20f, rect.width * 0.20f), member.ClassKey, member.Role, color);
            DrawRect(new Rect(rect.x + rect.width * 0.08f, rect.y + rect.height * 0.84f, rect.width * 0.84f, rect.height * 0.05f), color);
            GUI.Label(new Rect(rect.x, rect.y + rect.height * 0.76f, rect.width, 22), member.Origin, CenterStyle(13, muted));
        }

        private void DrawSigil(Rect rect, string sigil, Color color)
        {
            if (string.IsNullOrEmpty(sigil)) sigil = "bar";
            if (sigil == "chevron")
            {
                DrawRect(new Rect(rect.x, rect.y + rect.height * 0.45f, rect.width * 0.46f, rect.height * 0.22f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.54f, rect.y + rect.height * 0.45f, rect.width * 0.46f, rect.height * 0.22f), color);
            }
            else if (sigil == "moon")
            {
                DrawRect(new Rect(rect.x, rect.y, rect.width * 0.7f, rect.height), color);
                DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y, rect.width * 0.65f, rect.height), Hex("101619"));
            }
            else if (sigil == "cross")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.43f, rect.y, rect.width * 0.14f, rect.height), color);
                DrawRect(new Rect(rect.x, rect.y + rect.height * 0.42f, rect.width, rect.height * 0.18f), color);
            }
            else if (sigil == "diamond")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.35f, rect.y, rect.width * 0.3f, rect.height), color);
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.28f, rect.width * 0.64f, rect.height * 0.44f), color);
            }
            else if (sigil == "flame")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.42f, rect.y, rect.width * 0.18f, rect.height), color);
                DrawRect(new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * 0.42f, rect.width * 0.52f, rect.height * 0.48f), color);
            }
            else if (sigil == "leaf")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.12f, rect.y + rect.height * 0.38f, rect.width * 0.76f, rect.height * 0.28f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.45f, rect.y, rect.width * 0.12f, rect.height), color);
            }
            else if (sigil == "eye")
            {
                DrawRect(new Rect(rect.x, rect.y + rect.height * 0.32f, rect.width, rect.height * 0.36f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.38f, rect.y, rect.width * 0.24f, rect.height), color);
            }
            else
            {
                DrawRect(new Rect(rect.x, rect.y + rect.height * 0.35f, rect.width, rect.height * 0.3f), color);
            }
        }

        private GUIStyle CenterStyle(int size, Color color)
        {
            string key = StyleKey(size, color);
            GUIStyle style;
            if (centerStyleCache.TryGetValue(key, out style)) return style;
            style = new GUIStyle(GUI.skin.label)
            {
                fontSize = size,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = color }
            };
            centerStyleCache[key] = style;
            return style;
        }

        private GUIStyle CenterLeftStyle(int size, Color color)
        {
            string key = StyleKey(size, color);
            GUIStyle style;
            if (centerLeftStyleCache.TryGetValue(key, out style)) return style;
            style = new GUIStyle(GUI.skin.label)
            {
                fontSize = size,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = color }
            };
            centerLeftStyleCache[key] = style;
            return style;
        }

        private GUIStyle CenterRightStyle(int size, Color color)
        {
            string key = StyleKey(size, color);
            GUIStyle style;
            if (centerRightStyleCache.TryGetValue(key, out style)) return style;
            style = new GUIStyle(GUI.skin.label)
            {
                fontSize = size,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleRight,
                normal = { textColor = color }
            };
            centerRightStyleCache[key] = style;
            return style;
        }

        private string StyleKey(int size, Color color)
        {
            Color32 c = color;
            return $"{size}|{c.r:X2}{c.g:X2}{c.b:X2}{c.a:X2}";
        }

        private Rect Pad(Rect rect, float pad)
        {
            return new Rect(rect.x + pad, rect.y + pad, rect.width - pad * 2f, rect.height - pad * 2f);
        }
    }
}
