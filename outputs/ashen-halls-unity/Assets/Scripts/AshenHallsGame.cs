using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace AshenHalls
{
    public sealed class AshenHallsGame : MonoBehaviour
    {
        private const int ExploreW = 46;
        private const int ExploreH = 30;
        private const int ExploreRevealRadius = 10;
        private const int CombatW = 12;
        private const int CombatH = 8;
        private const int SaveVersion = 17;
        private const int PartySize = 4;
        private const int StatPointBudget = 50;
        private const int CombatMoveAllowance = 3;
        private const int UnreachableMoveCost = 999;
        private const int SummonedTreeDuration = 8;
        private const int FinalBossDepth = 6;
        private const string PackageVersion = "v0.50.3";
        private const string GameTitle = "Ashen Halls";
        private const string GameSubtitle = "The Old Road";
        private const string BuildStage = "Beta RPG Scaffold";
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

        private Texture2D pixel;
        private Texture2D splashArt;
        private Texture2D betaCombatArt;
        private Texture2D formulaLabArt;
        private Texture2D combatSpriteSheet;
        private Texture2D classIconAtlas;
        private Texture2D worldObjectAtlas;
        private Texture2D itemIconAtlas;
        private Texture2D enemyRosterAtlas;
        private Texture2D combatUiAtlas;
        private Texture2D spellbookUiAtlas;
        private Texture2D emberSpellAtlas;
        private Texture2D epicSpellEffectsAtlas;
        private Texture2D combatSpellbookUiAtlas;
        private Texture2D bossEnemyAtlas;
        private Texture2D questWorldAtlas;
        private Texture2D characterInventoryUiAtlas;
        private Texture2D combatHudUiAtlas;
        private Texture2D combatSpellFloatAtlas;
        private Texture2D enemyWorldObjectAtlas;
        private Texture2D tavernBackdropArt;
        private Texture2D tavernUiAtlas;
        private Texture2D inventoryConsumableAtlas;
        private Texture2D combatCommandIconAtlas;
        private Texture2D creatureSpriteAtlas;
        private Texture2D combatTerrainAtlas;
        private GUIStyle titleStyle;
        private GUIStyle h2Style;
        private GUIStyle labelStyle;
        private GUIStyle mutedStyle;
        private GUIStyle buttonStyle;
        private GUIStyle smallButtonStyle;
        private GUIStyle logStyle;
        private GUIStyle fieldStyle;
        private readonly Dictionary<string, GUIStyle> centerStyleCache = new Dictionary<string, GUIStyle>();
        private readonly Dictionary<string, GUIStyle> centerLeftStyleCache = new Dictionary<string, GUIStyle>();
        private readonly Dictionary<string, GUIStyle> centerRightStyleCache = new Dictionary<string, GUIStyle>();
        private readonly Dictionary<int, SpriteCellMetrics> spriteCellMetrics = new Dictionary<int, SpriteCellMetrics>();
        private AudioSource audioSource;
        private AudioSource musicSource;
        private AudioClip tavernMusicClip;
        private readonly Dictionary<string, AudioClip> soundClips = new Dictionary<string, AudioClip>();

        private GameState state;
        private System.Random rng;
        private Rect boardRect;
        private Rect sideRect;
        private Vector2 logScroll;
        private Vector2 armoryScroll;
        private int selectedBuilderIndex;
        private int armoryTab;
        private ActionMode selectedAction = ActionMode.Attack;
        private string pendingFormulaCode = "";
        private float aiActAt = -1f;
        private string bannerText = "";
        private float bannerUntil;
        private float splashStartedAt;
        private bool splashClockStarted;
        private string launchStatus = "Lighting the old road...";
        private string launchError = "";
        private string lootPanelTitle = "";
        private string lootPanelBody = "";
        private InventoryItem lootPanelItem;
        private int lootPanelGold;
        private int lootPanelSupplies;
        private int lootPanelElixirs;
        private float lootPanelUntil;
        private string lastExploreRegion = "";
        private bool showArmory;
        private bool showTavernSettings;
        private bool showSpellbook;
        private bool betaLabMode;
        private int formulaChipPage;
        private string lastSfxKey = "";
        private float lastSfxAt = -10f;
        private float lastSfxVolume;
        private readonly List<Tween> tweens = new List<Tween>();
        private readonly List<FloatText> floatTexts = new List<FloatText>();
        private readonly List<ParticleDot> particles = new List<ParticleDot>();
        private readonly List<BeamEffect> beams = new List<BeamEffect>();
        private readonly List<CellFlash> flashes = new List<CellFlash>();
        private readonly List<CastGlyph> castGlyphs = new List<CastGlyph>();

        private static readonly string[] roleOrder =
        {
            "shield", "pike", "bow", "knife", "mender", "ember", "hex", "ward"
        };

        private static readonly string[] classOrder =
        {
            "rogue", "warrior", "ranger", "wizard", "mage", "warlock", "priest", "paladin"
        };

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

        private static readonly FormulaDef[] formulaBook =
        {
            new FormulaDef { Code = "GBH", Name = "Tree Cover", Hint = "grow breakable cover", School = "mend", Skill = "mend", Mana = 7, Range = 4, Target = "tile", Effect = "terrain", Terrain = "tree", Duration = SummonedTreeDuration, Arc = true },
            new FormulaDef { Code = "GBX", Name = "Stone Block", Hint = "raise stone cover", School = "mend", Skill = "mend", Mana = 6, Range = 4, Target = "tile", Effect = "terrain", Terrain = "stone", Duration = 0 },
            new FormulaDef { Code = "OIC", Name = "Heal", Hint = "heal ally", School = "mend", Skill = "mend", Mana = 5, Range = 4, Target = "ally", Effect = "heal", DamageType = "light", Power = 12 },
            new FormulaDef { Code = "NVC", Name = "Cleanse", Hint = "cure afflictions", School = "mend", Skill = "mend", Mana = 4, Range = 4, Target = "ally", Effect = "cure" },
            new FormulaDef { Code = "TBQ", Name = "Ward", Hint = "protect ally", School = "mend", Skill = "mend", Mana = 6, Range = 4, Target = "ally", Effect = "status", Status = "shield", Duration = 3 },
            new FormulaDef { Code = "TNC", Name = "Regenerate", Hint = "heal over time", School = "mend", Skill = "mend", Mana = 8, Range = 4, Target = "ally", Effect = "status", Status = "regen", Duration = 3 },
            new FormulaDef { Code = "LBC", Name = "Circle Heal", Hint = "heal nearby allies", School = "mend", Skill = "mend", Mana = 9, Range = 4, Target = "ally", Effect = "heal", DamageType = "light", Power = 8, Splash = true, Arc = true },
            new FormulaDef { Code = "TBG", Name = "Circle Ward", Hint = "ward nearby allies", School = "mend", Skill = "mend", Mana = 9, Range = 4, Target = "ally", Effect = "status", Status = "shield", Duration = 2, Splash = true, Arc = true },
            new FormulaDef { Code = "OBL", Name = "Light Bolt", Hint = "damage with light", School = "mend", Skill = "mend", Mana = 6, Range = 4, Target = "enemy", Effect = "damage", DamageType = "light", Power = 10 },
            new FormulaDef { Code = "LNH", Name = "Hold Sign", Hint = "briefly stun enemy", School = "mend", Skill = "mend", Mana = 7, Range = 4, Target = "enemy", Effect = "status", Status = "stun", DamageType = "light", Duration = 1 },

            new FormulaDef { Code = "FIF", Name = "Fire Spark", Hint = "small fire hit", School = "ember", Skill = "ember", Mana = 3, Range = 4, Target = "enemy", Effect = "damage", DamageType = "fire", Power = 9 },
            new FormulaDef { Code = "WBF", Name = "Fire Floor", Hint = "ignite floor", School = "ember", Skill = "ember", Mana = 6, Range = 4, Target = "tile", Effect = "terrain", Terrain = "fire", Duration = 12 },
            new FormulaDef { Code = "BTF", Name = "Burn Cover", Hint = "burn cover", School = "ember", Skill = "ember", Mana = 5, Range = 5, Target = "tile", Effect = "terrain", Terrain = "fire", Duration = 8, Arc = true },
            new FormulaDef { Code = "WBI", Name = "Ice Slick", Hint = "slick ice", School = "ember", Skill = "ember", Mana = 5, Range = 4, Target = "tile", Effect = "terrain", Terrain = "ice", Duration = 12 },
            new FormulaDef { Code = "RCL", Name = "Cold Lance", Hint = "long cold bolt", School = "ember", Skill = "ember", Mana = 6, Range = 6, Target = "enemy", Effect = "damage", DamageType = "cold", Power = 12 },
            new FormulaDef { Code = "RDF", Name = "Flame Jet", Hint = "fire and bleeding", School = "ember", Skill = "ember", Mana = 7, Range = 5, Target = "enemy", Effect = "damage", DamageType = "fire", Status = "bleed", Power = 13, Duration = 2 },
            new FormulaDef { Code = "RIG", Name = "Shock Bolt", Hint = "shock and stun", School = "ember", Skill = "ember", Mana = 5, Range = 4, Target = "enemy", Effect = "damage", DamageType = "shock", Status = "stun", Power = 10, Duration = 1 },
            new FormulaDef { Code = "FBL", Name = "Fireball", Hint = "classic splash fire", School = "ember", Skill = "ember", Mana = 7, Range = 5, Target = "enemy", Effect = "damage", DamageType = "fire", Power = 15, Splash = true, Arc = true },
            new FormulaDef { Code = "RLF", Name = "Fireburst", Hint = "splash fire", School = "ember", Skill = "ember", Mana = 8, Range = 5, Target = "enemy", Effect = "damage", DamageType = "fire", Power = 14, Splash = true },
            new FormulaDef { Code = "RSG", Name = "Shock Burst", Hint = "splash shock", School = "ember", Skill = "ember", Mana = 8, Range = 5, Target = "enemy", Effect = "damage", DamageType = "shock", Status = "stun", Power = 12, Duration = 1, Splash = true },
            new FormulaDef { Code = "RBI", Name = "Iceburst", Hint = "splash cold", School = "ember", Skill = "ember", Mana = 8, Range = 5, Target = "enemy", Effect = "damage", DamageType = "cold", Status = "stun", Power = 11, Duration = 1, Splash = true },
            new FormulaDef { Code = "MTR", Name = "Meteor Shower", Hint = "epic falling fire", School = "ember", Skill = "ember", Mana = 12, Range = 5, Target = "enemy", Effect = "damage", DamageType = "fire", Power = 18, Splash = true, Arc = true },

            new FormulaDef { Code = "WBK", Name = "Web Snare", Hint = "snare tile", School = "hex", Skill = "hex", Mana = 6, Range = 4, Target = "tile", Effect = "terrain", Terrain = "web", Duration = 12 },
            new FormulaDef { Code = "WBP", Name = "Poison Gas", Hint = "poison hazard", School = "hex", Skill = "hex", Mana = 7, Range = 4, Target = "tile", Effect = "terrain", Terrain = "gas", Duration = 12 },
            new FormulaDef { Code = "RMS", Name = "Sleep", Hint = "disable enemy", School = "hex", Skill = "hex", Mana = 6, Range = 5, Target = "enemy", Effect = "status", Status = "sleep", DamageType = "mind", Duration = 2 },
            new FormulaDef { Code = "RNH", Name = "Weaken", Hint = "lower defenses", School = "hex", Skill = "hex", Mana = 7, Range = 5, Target = "enemy", Effect = "status", Status = "hex", DamageType = "mind", Duration = 3 },
            new FormulaDef { Code = "RKW", Name = "Bind", Hint = "web enemy", School = "hex", Skill = "hex", Mana = 5, Range = 4, Target = "enemy", Effect = "status", Status = "web", DamageType = "mind", Duration = 2 },
            new FormulaDef { Code = "RPX", Name = "Poison Burst", Hint = "splash poison", School = "hex", Skill = "hex", Mana = 8, Range = 5, Target = "enemy", Effect = "damage", DamageType = "poison", Status = "poison", Power = 8, Duration = 2, Splash = true },
            new FormulaDef { Code = "INH", Name = "Drain Life", Hint = "damage and heal", School = "hex", Skill = "hex", Mana = 7, Range = 4, Target = "enemy", Effect = "drain", DamageType = "death", Power = 11 },
            new FormulaDef { Code = "RMB", Name = "Mind Break", Hint = "mind hit and hex", School = "hex", Skill = "hex", Mana = 7, Range = 5, Target = "enemy", Effect = "damage", DamageType = "mind", Status = "hex", Power = 10, Duration = 2 },
            new FormulaDef { Code = "RLM", Name = "Death Burst", Hint = "splash death", School = "ember|hex", Skill = "hex", Mana = 9, Range = 5, Target = "enemy", Effect = "damage", DamageType = "death", Power = 16, Splash = true },

            new FormulaDef { Code = "IBD", Name = "Bind Imp", Hint = "summon a fragile demon", School = "pact", Skill = "hex", Mana = 8, Range = 3, Target = "tile", Effect = "summon", SummonRole = "boundimp", DamageType = "death", Power = 6, Duration = 6, Arc = true },
            new FormulaDef { Code = "IBF", Name = "Bind Fiend", Hint = "summon a tougher demon", School = "pact", Skill = "hex", Mana = 12, Range = 3, Target = "tile", Effect = "summon", SummonRole = "lesserdemon", DamageType = "death", Power = 9, Duration = 4, Arc = true }
        };

        private sealed class SpriteCellMetrics
        {
            public Rect Source;
            public float AnchorX;
        }

        private struct StatusMark
        {
            public string Label;
            public Color Color;
            public int Turns;

            public StatusMark(string label, Color color, int turns)
            {
                Label = label;
                Color = color;
                Turns = turns;
            }
        }

        private void Awake()
        {
            splashStartedAt = Time.realtimeSinceStartup;
            Application.targetFrameRate = 60;
            Screen.fullScreen = false;
            Screen.fullScreenMode = FullScreenMode.Windowed;
            if (Screen.width < 1880 || Screen.height < 1040) Screen.SetResolution(1920, 1080, false);

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
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.playOnAwake = false;
                musicSource.loop = true;
                musicSource.spatialBlend = 0f;
                musicSource.ignoreListenerPause = true;
                musicSource.priority = 64;
                BuildSoundClips();
                tavernMusicClip = MakeTavernMusic();
                musicSource.clip = tavernMusicClip;
                NewMuster();
                ApplyAudioSettings();
                launchStatus = "Muster ready.";
            }
            catch (Exception ex)
            {
                launchError = ex.Message;
                Debug.LogException(ex);
            }
        }

        private void BuildSoundClips()
        {
            soundClips.Clear();
            soundClips["move"] = MakeSound("move", 96f, 54f, 0.08f, 0.34f, "thud");
            soundClips["blocked"] = MakeSound("blocked", 132f, 86f, 0.10f, 0.30f, "square");
            soundClips["attack"] = MakeSound("attack", 220f, 78f, 0.12f, 0.38f, "slash");
            soundClips["blade"] = MakeSound("blade", 330f, 120f, 0.10f, 0.34f, "slash");
            soundClips["bow"] = MakeSound("bow", 150f, 420f, 0.10f, 0.24f, "rustle");
            soundClips["miss"] = MakeSound("miss", 410f, 210f, 0.10f, 0.20f, "rustle");
            soundClips["crit"] = MakeSound("crit", 280f, 760f, 0.16f, 0.32f, "square");
            soundClips["guard"] = MakeSound("guard", 170f, 96f, 0.14f, 0.34f, "thud");
            soundClips["counter"] = MakeSound("counter", 260f, 150f, 0.12f, 0.30f, "slash");
            soundClips["resist"] = MakeSound("resist", 180f, 260f, 0.12f, 0.20f, "square");
            soundClips["status"] = MakeSound("status", 300f, 480f, 0.16f, 0.22f, "sine");
            soundClips["spell"] = MakeSound("spell", 410f, 820f, 0.24f, 0.30f, "sine");
            soundClips["heal"] = MakeSound("heal", 330f, 760f, 0.30f, 0.28f, "sine");
            soundClips["ward"] = MakeSound("ward", 260f, 520f, 0.22f, 0.30f, "sine");
            soundClips["light"] = MakeSound("light", 640f, 1180f, 0.20f, 0.28f, "sine");
            soundClips["shock"] = MakeSound("shock", 900f, 260f, 0.16f, 0.34f, "square");
            soundClips["arc"] = MakeSound("arc", 760f, 1120f, 0.18f, 0.30f, "square");
            soundClips["curse"] = MakeSound("curse", 260f, 82f, 0.32f, 0.28f, "square");
            soundClips["tree"] = MakeSound("tree", 122f, 190f, 0.38f, 0.34f, "rustle");
            soundClips["breakcover"] = MakeSound("breakcover", 110f, 62f, 0.20f, 0.38f, "thud");
            soundClips["stone"] = MakeSound("stone", 92f, 58f, 0.24f, 0.36f, "thud");
            soundClips["fire"] = MakeSound("fire", 260f, 640f, 0.26f, 0.32f, "rustle");
            soundClips["ice"] = MakeSound("ice", 720f, 420f, 0.20f, 0.28f, "sine");
            soundClips["web"] = MakeSound("web", 190f, 260f, 0.20f, 0.26f, "rustle");
            soundClips["poison"] = MakeSound("poison", 180f, 130f, 0.30f, 0.26f, "sine");
            soundClips["death"] = MakeSound("death", 180f, 54f, 0.40f, 0.36f, "square");
            soundClips["cache"] = MakeSound("cache", 560f, 960f, 0.16f, 0.30f, "square");
            soundClips["shrine"] = MakeSound("shrine", 260f, 520f, 0.36f, 0.26f, "sine");
            soundClips["encounter"] = MakeSound("encounter", 118f, 72f, 0.34f, 0.36f, "square");
            soundClips["victory"] = MakeSound("victory", 430f, 860f, 0.40f, 0.30f, "sine");
            soundClips["defeat"] = MakeSound("defeat", 150f, 64f, 0.48f, 0.34f, "thud");
            soundClips["save"] = MakeSound("save", 620f, 840f, 0.14f, 0.24f, "sine");
            soundClips["ui"] = MakeSound("ui", 480f, 610f, 0.06f, 0.20f, "square");
            soundClips["formula"] = MakeSound("formula", 520f, 880f, 0.12f, 0.24f, "sine");
            soundClips["turn"] = MakeSound("turn", 360f, 520f, 0.10f, 0.22f, "square");
        }

        private AudioClip MakeSound(string name, float startFrequency, float endFrequency, float duration, float volume, string shape)
        {
            const int sampleRate = 22050;
            int sampleCount = Mathf.Max(1, Mathf.CeilToInt(sampleRate * duration));
            float[] data = new float[sampleCount];
            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                float progress = sampleCount <= 1 ? 1f : i / (float)(sampleCount - 1);
                float frequency = Mathf.Lerp(startFrequency, endFrequency, progress);
                phase += Mathf.PI * 2f * frequency / sampleRate;
                float sine = Mathf.Sin(phase);
                float noise = PseudoNoise(i) * 2f - 1f;
                float sample;
                if (shape == "square")
                {
                    sample = sine >= 0f ? 1f : -1f;
                }
                else if (shape == "thud")
                {
                    sample = sine * 0.72f + noise * 0.28f;
                }
                else if (shape == "slash")
                {
                    sample = noise * 0.68f + Mathf.Sin(phase * 0.5f) * 0.32f;
                }
                else if (shape == "rustle")
                {
                    sample = noise * 0.62f + sine * 0.38f;
                }
                else
                {
                    sample = sine;
                }

                float attack = Mathf.Clamp01(t / 0.012f);
                float release = Mathf.Clamp01((duration - t) / Mathf.Max(0.001f, duration * 0.72f));
                float envelope = Mathf.Min(attack, release);
                data[i] = sample * envelope * envelope * volume;
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
            const float seconds = 16f;
            int sampleCount = Mathf.CeilToInt(sampleRate * seconds);
            float[] data = new float[sampleCount];
            float[] melody = { 392f, 440f, 494f, 440f, 392f, 330f, 349f, 392f, 294f, 330f, 392f, 330f, 262f, 294f, 330f, 392f };
            float[] harmony = { 196f, 247f, 220f, 262f, 174.6f, 220f, 196f, 247f };
            float[] flute = { 0f, 587f, 659f, 0f, 523f, 494f, 440f, 0f };
            float beat = 0.40f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                int m = Mathf.FloorToInt(t / beat) % melody.Length;
                int h = Mathf.FloorToInt(t / (beat * 2f)) % harmony.Length;
                int f = Mathf.FloorToInt(t / (beat * 4f)) % flute.Length;
                float beatT = (t % beat) / beat;
                float barT = (t % (beat * 4f)) / (beat * 4f);
                float pluckEnv = Mathf.Exp(-beatT * 6.6f);
                float bassEnv = Mathf.Exp(-((t % (beat * 2f)) / (beat * 2f)) * 2.7f);
                float swing = 0.76f + Mathf.Sin(t * Mathf.PI * 2f * 0.33f) * 0.08f;
                float lute = (Triangle(t * melody[m]) * 0.76f + Triangle(t * melody[m] * 2.01f) * 0.20f) * pluckEnv * 0.28f;
                float low = Mathf.Sin(t * harmony[h] * Mathf.PI * 2f) * bassEnv * 0.14f;
                float body = Triangle(t * (harmony[h] * 0.5f)) * 0.06f * (0.55f + Mathf.Sin(t * Mathf.PI * 2f * 0.125f) * 0.18f);
                float cup = Mathf.Sin(t * 1180f * Mathf.PI * 2f) * Mathf.Exp(-beatT * 13f) * (m % 4 == 0 ? 0.055f : 0.0f);
                float drum = (PseudoNoise(i) * 2f - 1f) * Mathf.Exp(-barT * 18f) * (m % 4 == 0 ? 0.050f : m % 4 == 2 ? 0.024f : 0f);
                float fluteTone = flute[f] <= 0f ? 0f : Mathf.Sin(t * flute[f] * Mathf.PI * 2f) * Mathf.Sin(Mathf.Clamp01(barT) * Mathf.PI) * 0.045f;
                float sample = (lute + low + body + cup + drum + fluteTone) * swing;
                data[i] = Mathf.Clamp(sample, -0.85f, 0.85f);
            }

            AudioClip clip = AudioClip.Create("tavern_hearth_lute_loop", sampleCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
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

        private void PlaySfx(string key, float volume = 1f)
        {
            if (audioSource == null || string.IsNullOrEmpty(key) || !soundClips.ContainsKey(key)) return;
            if (state != null && state.SfxMuted) return;
            try
            {
                ApplyAudioSettings();
                float clamped = Mathf.Clamp(volume, 0f, 1.4f);
                lastSfxKey = key;
                lastSfxAt = Time.realtimeSinceStartup;
                lastSfxVolume = clamped;
                audioSource.PlayOneShot(soundClips[key], clamped);
            }
            catch (Exception)
            {
                // Audio devices may be unavailable during headless smoke tests.
            }
        }

        private void ApplyAudioSettings()
        {
            int percent = state == null ? 100 : Mathf.Clamp(state.SfxVolumePercent <= 0 ? 100 : state.SfxVolumePercent, 25, 100);
            bool muted = state != null && state.SfxMuted;
            if (audioSource != null) audioSource.volume = muted ? 0f : 0.78f * (percent / 100f);
            if (musicSource != null) musicSource.volume = muted ? 0f : 0.20f * (percent / 100f);
        }

        private void UpdateTavernMusic()
        {
            if (musicSource == null || tavernMusicClip == null) return;
            bool shouldPlay = state != null && (state.Mode == GameMode.Tavern || state.Mode == GameMode.Muster) && !IsStartupSplashVisible() && !state.SfxMuted;
            ApplyAudioSettings();
            if (shouldPlay)
            {
                if (!musicSource.isPlaying)
                {
                    musicSource.clip = tavernMusicClip;
                    musicSource.Play();
                }
            }
            else if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        }

        private void TestSfx()
        {
            if (state == null) return;
            NormalizeGameSettings();
            state.SfxMuted = false;
            ApplyAudioSettings();
            PlaySfx("ui", 0.85f);
            PlaySfx("attack", 0.75f);
            PlaySfx("spell", 0.75f);
            ShowBanner("SFX test");
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

        private void CycleSfxVolume()
        {
            if (state == null) return;
            NormalizeGameSettings();
            if (state.SfxVolumePercent >= 100) state.SfxVolumePercent = 25;
            else state.SfxVolumePercent += 25;
            ApplyAudioSettings();
            PlaySfx("ui", 0.7f);
            ShowBanner($"SFX {state.SfxVolumePercent}%");
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

        private void ToggleArmory(int tab)
        {
            if (state == null) return;
            int nextTab = Mathf.Clamp(tab, 0, 2);
            if (showArmory && armoryTab == nextTab)
            {
                showArmory = false;
            }
            else
            {
                showArmory = true;
                armoryTab = nextTab;
                armoryScroll = Vector2.zero;
            }
            PlaySfx("ui", 0.55f);
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

            UpdateTavernMusic();
            if (IsStartupSplashVisible()) return;

            if (Input.GetKeyDown(KeyCode.F5)) SaveGame();
            if (Input.GetKeyDown(KeyCode.F9)) LoadGame();
            if (Input.GetKeyDown(KeyCode.F1)) ContextHelp();
            if (Input.GetKeyDown(KeyCode.M)) ToggleSfxMute();
            if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus)) AdjustSfxVolume(25);
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) AdjustSfxVolume(-25);
            bool gameplayMode = state != null && !showSpellbook && (state.Mode == GameMode.Explore || state.Mode == GameMode.Combat);
            if (gameplayMode && Input.GetKeyDown(KeyCode.I)) ToggleArmory(0);
            if (state != null && state.Mode == GameMode.Explore && !showSpellbook && Input.GetKeyDown(KeyCode.C)) ToggleArmory(2);
            if (showArmory && Input.GetKeyDown(KeyCode.Escape))
            {
                showArmory = false;
                PlaySfx("ui", 0.45f);
                return;
            }
            if (showArmory) return;

            if (state == null) return;
            if (showSpellbook && Input.GetKeyDown(KeyCode.Escape))
            {
                showSpellbook = false;
                PlaySfx("ui", 0.45f);
                return;
            }
            if (state.Mode == GameMode.Tavern)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) QuickStart();
                if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.C)) state.Mode = GameMode.Muster;
            }
            else if (state.Mode == GameMode.Muster)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) QuickStart();
                if (Input.GetKeyDown(KeyCode.B)) BeginGame();
                if (Input.GetKeyDown(KeyCode.Escape)) state.Mode = GameMode.Tavern;
            }
            if (state.Mode == GameMode.Explore) HandleExploreKeyboard();
            if (state.Mode == GameMode.Combat)
            {
                HandleCombatTimers();
                if (showSpellbook) return;
                HandleCombatHotkeys();
            }
        }

        private void OnGUI()
        {
            try
            {
                EnsureStyles();
                if (!splashClockStarted)
                {
                    splashStartedAt = Time.realtimeSinceStartup;
                    splashClockStarted = true;
                }

                DrawRect(new Rect(0, 0, Screen.width, Screen.height), bg);
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

                switch (state.Mode)
                {
                    case GameMode.Tavern:
                        DrawTavernMenu();
                        break;
                    case GameMode.Muster:
                        DrawMuster();
                        break;
                    case GameMode.Explore:
                        DrawExplore();
                        DrawSidePanels();
                        DrawCommandBar();
                        DrawGameChrome("Explore");
                        break;
                    case GameMode.Combat:
                        DrawCombat();
                        DrawSidePanels();
                        DrawCommandBar();
                        DrawGameChrome("Combat");
                        break;
                    case GameMode.Defeat:
                        DrawGameChrome("Defeat");
                        DrawDefeat();
                        DrawSidePanels();
                        break;
                    case GameMode.Victory:
                        DrawGameChrome("Victory");
                        DrawVictory();
                        break;
                }

                DrawBanner();
                DrawLootPanel();
                DrawArmoryOverlay();
            }
            catch (Exception ex)
            {
                launchError = ex.Message;
                Debug.LogException(ex);
                DrawLaunchError(ex);
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
            if (splashClockStarted && Event.current != null && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.KeyDown))
            {
                splashStartedAt = Time.realtimeSinceStartup - 6f;
                return false;
            }

            return IsStartupSplashVisible();
        }

        private bool IsStartupSplashVisible()
        {
            return !splashClockStarted || Time.realtimeSinceStartup - splashStartedAt < 6.0f;
        }

        private void DrawStartupSplash(string status, bool overlay)
        {
            EnsurePixel();
            float elapsed = splashClockStarted ? Mathf.Max(0f, Time.realtimeSinceStartup - splashStartedAt) : 0f;
            float alpha = overlay ? Mathf.Clamp01(1f - Mathf.InverseLerp(5.1f, 6.0f, elapsed)) : 1f;
            DrawRect(new Rect(0, 0, Screen.width, Screen.height), Hex("050708", overlay ? 0.94f * alpha : 1f));

            if (splashArt != null)
            {
                Color old = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, alpha);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), splashArt, ScaleMode.ScaleAndCrop);
                GUI.color = old;

                DrawRect(new Rect(0, 0, Screen.width, Screen.height), Hex("050708", 0.18f * alpha));
                Rect title = new Rect(Screen.width * 0.18f, Screen.height * 0.08f, Screen.width * 0.64f, Screen.height * 0.25f);
                DrawRect(new Rect(title.x + title.width * 0.10f, title.y + title.height * 0.02f, title.width * 0.80f, title.height * 0.78f), Hex("030405", 0.46f * alpha));
                DrawBorder(new Rect(title.x + title.width * 0.12f, title.y + title.height * 0.08f, title.width * 0.76f, title.height * 0.58f), Hex("d7a84e", 0.54f * alpha), 2);
                GUI.Label(new Rect(title.x, title.y + title.height * 0.10f, title.width, title.height * 0.30f), GameTitle.ToUpperInvariant(), CenterStyle(Mathf.RoundToInt(Mathf.Clamp(Screen.width / 34f, 32f, 54f)), Hex("f3ead7", alpha)));
                GUI.Label(new Rect(title.x, title.y + title.height * 0.42f, title.width, title.height * 0.18f), GameSubtitle, CenterStyle(Mathf.RoundToInt(Mathf.Clamp(Screen.width / 82f, 14f, 22f)), Hex("d7a84e", alpha)));
                Rect betaBadge = new Rect(title.x + title.width * 0.32f, title.y + title.height * 0.62f, title.width * 0.36f, 30f);
                DrawRect(betaBadge, Hex("2a1112", 0.88f * alpha));
                DrawBorder(betaBadge, Hex("c65c3b", 0.92f * alpha), 1);
                GUI.Label(betaBadge, BuildStage.ToUpperInvariant(), CenterStyle(15, Hex("f3ead7", alpha)));

                Rect splashBar = new Rect(Screen.width * 0.33f, Screen.height * 0.79f, Screen.width * 0.34f, Mathf.Max(10f, Screen.height * 0.014f));
                DrawRect(Pad(splashBar, -4), Hex("030405", 0.76f * alpha));
                DrawBorder(Pad(splashBar, -4), Hex("d7a84e", 0.42f * alpha), 1);
                float splashPulse = 0.25f + Mathf.PingPong(elapsed * 0.65f, 0.75f);
                DrawRect(new Rect(splashBar.x, splashBar.y, Mathf.Max(10f, splashBar.width * splashPulse), splashBar.height), Hex("58b7a5", 0.92f * alpha));
                GUI.Label(new Rect(0, splashBar.yMax + 10f, Screen.width, 24f), string.IsNullOrEmpty(status) ? "Loading..." : status, CenterStyle(13, Hex("f3ead7", alpha)));
                GUI.Label(new Rect(0, splashBar.yMax + 34f, Screen.width, 22f), "click, Enter, or any key to continue", CenterStyle(12, Hex("d7a84e", 0.88f * alpha)));
                if (!string.IsNullOrEmpty(launchError))
                {
                    GUI.Label(new Rect(Screen.width * 0.18f, splashBar.yMax + 58f, Screen.width * 0.64f, 24f), "Startup note: " + launchError, CenterStyle(11, Hex("b94b56", alpha)));
                }
                return;
            }

            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
            Rect art = new Rect(Screen.width * 0.5f - 310f * scale, Screen.height * 0.5f - 185f * scale, 620f * scale, 370f * scale);
            DrawRect(art, Hex("101619", 0.94f * alpha));
            DrawBorder(art, Hex("d7a84e", 0.70f * alpha), Mathf.Max(1, Mathf.RoundToInt(2 * scale)));

            Rect sky = new Rect(art.x + 16 * scale, art.y + 16 * scale, art.width - 32 * scale, art.height * 0.44f);
            DrawRect(sky, Hex("171c20", 0.95f * alpha));
            for (int i = 0; i < 18; i++)
            {
                float sx = sky.x + ((i * 53) % 560) * scale;
                float sy = sky.y + (18 + (i * 29) % 96) * scale;
                DrawRect(new Rect(sx, sy, Mathf.Max(2, 3 * scale), Mathf.Max(2, 3 * scale)), Hex(i % 3 == 0 ? "d7a84e" : "8d6dcc", 0.65f * alpha));
            }

            DrawRect(new Rect(art.x + 60 * scale, art.y + 138 * scale, 500 * scale, 24 * scale), Hex("3a3329", alpha));
            for (int i = 0; i < 7; i++)
            {
                float x = art.x + (96 + i * 66) * scale;
                float h = (86 + (i % 3) * 24) * scale;
                DrawRect(new Rect(x, art.y + 72 * scale + (120 * scale - h), 34 * scale, h), Hex(i % 2 == 0 ? "46504d" : "3c4544", alpha));
                DrawRect(new Rect(x - 8 * scale, art.y + 70 * scale + (120 * scale - h), 50 * scale, 10 * scale), Hex("6b756e", alpha));
                DrawRect(new Rect(x + 9 * scale, art.y + 92 * scale + (120 * scale - h), 8 * scale, 28 * scale), Hex("050708", 0.82f * alpha));
            }

            for (int i = 0; i < 8; i++)
            {
                float x = art.x + (78 + i * 59) * scale;
                float y = art.y + (222 + (i % 2) * 7) * scale;
                DrawRect(new Rect(x, y, 20 * scale, 34 * scale), RoleColor(roleOrder[i % roleOrder.Length]).WithAlpha(alpha));
                DrawRect(new Rect(x + 4 * scale, y - 10 * scale, 12 * scale, 12 * scale), Hex("d9a67b", alpha));
                DrawRect(new Rect(x - 4 * scale, y + 30 * scale, 28 * scale, 6 * scale), Hex("020303", 0.72f * alpha));
            }

            GUI.Label(new Rect(art.x, art.y + 188 * scale, art.width, 50 * scale), GameTitle.ToUpperInvariant(), CenterStyle(Mathf.RoundToInt(40 * scale), Hex("f3ead7", alpha)));
            GUI.Label(new Rect(art.x, art.y + 236 * scale, art.width, 28 * scale), GameSubtitle + " / modern-pixel tactical company RPG", CenterStyle(Mathf.RoundToInt(15 * scale), Hex("b7aa90", alpha)));
            Rect fallbackBadge = new Rect(art.x + art.width * 0.33f, art.y + 266 * scale, art.width * 0.34f, 24 * scale);
            DrawRect(fallbackBadge, Hex("2a1112", 0.88f * alpha));
            DrawBorder(fallbackBadge, Hex("c65c3b", 0.88f * alpha), Mathf.Max(1, Mathf.RoundToInt(scale)));
            GUI.Label(fallbackBadge, BuildStage.ToUpperInvariant(), CenterStyle(Mathf.RoundToInt(12 * scale), Hex("f3ead7", alpha)));

            Rect bar = new Rect(art.x + 126 * scale, art.y + 296 * scale, art.width - 252 * scale, 12 * scale);
            DrawRect(bar, Hex("050708", 0.90f * alpha));
            float pulse = 0.25f + Mathf.PingPong(elapsed * 0.65f, 0.75f);
            DrawRect(new Rect(bar.x + 2 * scale, bar.y + 2 * scale, Mathf.Max(8 * scale, (bar.width - 4 * scale) * pulse), bar.height - 4 * scale), Hex("58b7a5", 0.88f * alpha));
            GUI.Label(new Rect(art.x, art.y + 314 * scale, art.width, 22 * scale), string.IsNullOrEmpty(status) ? "Loading..." : status, CenterStyle(Mathf.RoundToInt(12 * scale), Hex("d7a84e", alpha)));
            GUI.Label(new Rect(art.x, art.y + 336 * scale, art.width, 20 * scale), "click, Enter, or any key to continue", CenterStyle(Mathf.RoundToInt(11 * scale), Hex("b7aa90", 0.85f * alpha)));
            if (!string.IsNullOrEmpty(launchError))
            {
                GUI.Label(new Rect(art.x + 36 * scale, art.y + 354 * scale, art.width - 72 * scale, 24 * scale), "Startup note: " + launchError, CenterStyle(Mathf.RoundToInt(11 * scale), Hex("b94b56", alpha)));
            }
        }

        private void DrawLaunchError(Exception ex)
        {
            EnsurePixel();
            DrawRect(new Rect(0, 0, Screen.width, Screen.height), Hex("050708"));
            Rect rect = new Rect(Screen.width / 2f - 320, Screen.height / 2f - 120, 640, 240);
            DrawRect(rect, Hex("171c20"));
            DrawBorder(rect, blood, 2);
            GUI.Label(new Rect(rect.x + 24, rect.y + 24, rect.width - 48, 36), "Ashen Halls startup recovered", CenterStyle(22, ink));
            GUI.Label(new Rect(rect.x + 32, rect.y + 76, rect.width - 64, 80), "The game caught a startup error instead of leaving a blank screen.\nTry closing and relaunching. The latest error was:", CenterStyle(14, muted));
            GUI.Label(new Rect(rect.x + 32, rect.y + 154, rect.width - 64, 42), ex.Message, CenterStyle(13, gold));
        }

        private void EnsureStyles()
        {
            EnsurePixel();

            if (titleStyle != null) return;

            centerStyleCache.Clear();
            centerLeftStyleCache.Clear();
            centerRightStyleCache.Clear();

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 31,
                fontStyle = FontStyle.Bold,
                normal = { textColor = ink }
            };
            h2Style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Hex("f7dfad") }
            };
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                normal = { textColor = ink },
                wordWrap = true
            };
            mutedStyle = new GUIStyle(labelStyle)
            {
                fontSize = 12,
                normal = { textColor = muted }
            };
            logStyle = new GUIStyle(labelStyle)
            {
                fontSize = 13,
                wordWrap = true,
                padding = new RectOffset(8, 8, 7, 7)
            };
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = ink },
                hover = { textColor = ink },
                active = { textColor = ink }
            };
            smallButtonStyle = new GUIStyle(buttonStyle) { fontSize = 12 };
            fieldStyle = new GUIStyle(GUI.skin.textField)
            {
                fontSize = 14,
                normal = { textColor = ink },
                focused = { textColor = ink }
            };
        }

        private void EnsurePixel()
        {
            if (pixel != null) return;
            pixel = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            pixel.SetPixel(0, 0, Color.white);
            pixel.Apply();
        }

        private void LoadExternalArt()
        {
            splashArt = LoadLatestExternalPng("splash-title-reference-", "splash-title-reference-v0.27.png");
            betaCombatArt = LoadLatestExternalPng("beta-combat-casting-ui-reference-", "beta-combat-casting-ui-reference-v0.28.png");
            formulaLabArt = LoadLatestExternalPng("magic-ui-atlas-runtime-", "magic-ui-atlas-runtime-v0.43.png") ?? LoadLatestExternalPng("spell-card-icons-reference-", "spell-card-icons-reference-v0.38.png") ?? LoadLatestExternalPng("formula-lab-effects-reference-", "formula-lab-effects-reference-v0.29.png");
            combatSpriteSheet = LoadLatestExternalPng("combat-sprite-sheet-alpha-", "combat-sprite-sheet-alpha-v0.43.png") ?? LoadExternalPng("combat-sprite-sheet-alpha-v0.29.png");
            classIconAtlas = LoadLatestExternalPng("class-icon-atlas-runtime-", "class-icon-atlas-runtime-v0.40.png");
            worldObjectAtlas = LoadLatestExternalPng("world-environment-atlas-runtime-", "world-environment-atlas-runtime-v0.43.png") ?? LoadLatestExternalPng("world-object-atlas-runtime-", "world-object-atlas-runtime-v0.41.png");
            itemIconAtlas = LoadLatestExternalPng("item-equipment-atlas-runtime-", "item-equipment-atlas-runtime-v0.47.png") ?? LoadLatestExternalPng("item-inventory-atlas-runtime-", "") ?? LoadLatestExternalPng("item-icon-atlas-runtime-", "item-icon-atlas-runtime-v0.43.png");
            enemyRosterAtlas = LoadLatestExternalPng("enemy-roster-atlas-runtime-", "enemy-roster-atlas-runtime-v0.43.png");
            combatUiAtlas = LoadLatestExternalPng("combat-ui-atlas-runtime-", "combat-ui-atlas-runtime-v0.44.png");
            spellbookUiAtlas = LoadLatestExternalPng("spellbook-combat-ui-atlas-runtime-", "spellbook-combat-ui-atlas-runtime-v0.46.png");
            emberSpellAtlas = LoadLatestExternalPng("ember-spell-effects-atlas-runtime-", "ember-spell-effects-atlas-runtime-v0.46.png");
            epicSpellEffectsAtlas = LoadLatestExternalPng("combat-spell-effects-atlas-runtime-", "") ?? LoadLatestExternalPng("epic-spell-effects-atlas-runtime-", "epic-spell-effects-atlas-runtime-v0.47.png");
            combatSpellbookUiAtlas = LoadLatestExternalPng("combat-spellbook-ui-atlas-runtime-", "combat-spellbook-ui-atlas-runtime-v0.47.png");
            bossEnemyAtlas = LoadLatestExternalPng("boss-enemy-atlas-runtime-", "boss-enemy-atlas-runtime-v0.47.png");
            questWorldAtlas = LoadLatestExternalPng("quest-world-object-atlas-runtime-", "quest-world-object-atlas-runtime-v0.47.png");
            characterInventoryUiAtlas = LoadLatestExternalPng("character-inventory-ui-atlas-runtime-", "character-inventory-ui-atlas-runtime-v0.47.png");
            combatHudUiAtlas = LoadLatestExternalPng("combat-hud-ui-atlas-runtime-", "combat-hud-ui-atlas-runtime-v0.49.png");
            combatSpellFloatAtlas = LoadLatestExternalPng("combat-spell-float-atlas-runtime-", "combat-spell-float-atlas-runtime-v0.49.png");
            enemyWorldObjectAtlas = LoadLatestExternalPng("enemy-world-object-atlas-runtime-", "enemy-world-object-atlas-runtime-v0.49.png");
            tavernBackdropArt = LoadLatestExternalPng("tavern-backdrop-runtime-", "tavern-backdrop-runtime-v0.49.png");
            tavernUiAtlas = LoadLatestExternalPng("tavern-ui-atlas-runtime-", "tavern-ui-atlas-runtime-v0.49.png");
            inventoryConsumableAtlas = LoadLatestExternalPng("inventory-consumable-atlas-runtime-", "inventory-consumable-atlas-runtime-v0.50.png");
            combatCommandIconAtlas = LoadLatestExternalPng("combat-command-icon-atlas-runtime-", "combat-command-icon-atlas-runtime-v0.50.png");
            creatureSpriteAtlas = LoadLatestExternalPng("combat-sprite-atlas-runtime-", "") ?? LoadLatestExternalPng("creature-sprite-atlas-runtime-", "creature-sprite-atlas-runtime-v0.50.png");
            combatTerrainAtlas = LoadLatestExternalPng("combat-terrain-atlas-runtime-", "combat-terrain-atlas-runtime-v0.50.1.png");
            spriteCellMetrics.Clear();
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
                foreach (string directory in ExternalArtDirectories())
                {
                    if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory)) continue;
                    foreach (string path in Directory.GetFiles(directory, filePrefix + "*.png").OrderByDescending(File.GetLastWriteTimeUtc).ThenByDescending(p => Path.GetFileName(p)))
                    {
                        Texture2D texture = TryLoadExternalPngPath(path);
                        if (texture != null) return texture;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Could not load latest external art for " + filePrefix + ": " + ex.Message);
            }
            return string.IsNullOrEmpty(fallbackFileName) ? null : LoadExternalPng(fallbackFileName);
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
            return texture;
        }

        private void NewMuster()
        {
            int seed = Environment.TickCount;
            rng = new System.Random(seed);
            state = new GameState
            {
                SaveVersion = SaveVersion,
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
                SfxVolumePercent = 100,
                Party = MakeDefaultParty(),
                Inventory = new List<InventoryItem>(),
                Log = new List<LogEntry>()
            };
            NormalizeGameSettings();
            betaLabMode = false;
            showSpellbook = false;
            showTavernSettings = false;
            selectedBuilderIndex = 0;
            PushLog("Four names gather at the tavern table.", Tone.Good);
        }

        private void BeginGame()
        {
            EnsurePartyCustomization();
            if (!state.Party.All(p => p.Stats.Total == StatPointBudget))
            {
                PushLog($"Each character needs exactly {StatPointBudget} attribute points.", Tone.Warn);
                ShowBanner("Muster incomplete");
                return;
            }

            state.Mode = GameMode.Explore;
            betaLabMode = false;
            state.Depth = 1;
            state.StoryChapter = 1;
            state.ActiveStory = StoryObjectiveForDepth(state.Depth);
            if (state.DiscoveredZones == null) state.DiscoveredZones = new List<string>();
            state.DiscoveredZones.Clear();
            state.Map = GenerateMap(state.Depth, state.Seed);
            EnsureWorldLandmarks();
            state.PlayerX = state.Map.StartX;
            state.PlayerY = state.Map.StartY;
            lastExploreRegion = ExploreRegionName(state.PlayerX, state.PlayerY);
            DiscoverCurrentZone(true);
            PushLog($"The company leaves {HomeTownName}'s gate lamps and enters {GameTitle}: {GameSubtitle}.", Tone.Good);
            PushLog(state.ActiveStory, Tone.Normal);
            ShowBanner(HomeTownName);
            PlaySfx("shrine", 0.55f);
        }

        private void QuickStart()
        {
            state.Party = MakeDefaultParty();
            BeginGame();
        }

        private void StartBetaCombatLab()
        {
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

            state.Map = GenerateMap(state.Depth, state.Seed);
            EnsureWorldLandmarks();
            state.PlayerX = state.Map.StartX;
            state.PlayerY = state.Map.StartY;
            state.StoryChapter = 3;
            state.ActiveStory = StoryObjectiveForDepth(state.Depth);
            if (state.DiscoveredZones == null) state.DiscoveredZones = new List<string>();
            betaLabMode = true;
            StartCombat("lab");
            PushLog("Beta Combat Lab: caster-heavy battle loaded for stress testing spells, hazards, enemy magic, and SFX.", Tone.Good);
            ShowBanner(BuildStage);
            TestSfx();
        }

        private List<PartyMember> MakeDefaultParty()
        {
            return new List<PartyMember>
            {
                MakeHero("Maer", "human", "paladin", "ward", new Stats(16, 10, 9, 15), "mend", 1, new SkillSet { Arms = 7, Guard = 8, Mend = 4 }),
                MakeHero("Selka", "dusk elf", "rogue", "knife", new Stats(11, 11, 19, 9), "", 1, new SkillSet { Arms = 6, Missile = 5 }),
                MakeHero("Vesh", "fenkin", "priest", "mender", new Stats(7, 20, 9, 14), "mend", 1, new SkillSet { Mend = 9, Guard = 3 }),
                MakeHero("Oryn", "ashling", "warlock", "hex", new Stats(8, 21, 12, 9), "hex|pact", 3, new SkillSet { Hex = 9, Guard = 1 })
            };
        }

        private void DrawTavernMenu()
        {
            DrawTavernBackdrop();
            Rect top = new Rect(22, 16, Screen.width - 44, 54);
            DrawRect(top, Hex("0a0f12", 0.58f));
            DrawBorder(top, line.WithAlpha(0.52f), 1);
            GUI.Label(new Rect(top.x + 16, top.y + 4, 320, 28), GameTitle, CenterLeftStyle(26, ink));
            GUI.Label(new Rect(top.x + 18, top.y + 31, 440, 18), $"{GameSubtitle} / {BuildStage}", CenterLeftStyle(11, gold));
            float prefW = Mathf.Clamp(Screen.width * 0.22f, 300f, 380f);
            DrawPreferenceControls(top.xMax - prefW - 16f, top.y + 17, prefW, false);

            float menuW = Mathf.Clamp(Screen.width * 0.24f, 300f, 390f);
            Rect menu = new Rect(42, 100, menuW, Mathf.Min(372f, Screen.height - 138f));
            DrawRect(menu, Hex("11171b", 0.76f));
            DrawBorder(menu, gold, 2);
            DrawCombatUiCornerTrim(menu, gold);
            GUI.Label(new Rect(menu.x + 22, menu.y + 18, menu.width - 44, 30), "Midgaard Tavern", h2Style);
            GUI.Label(new Rect(menu.x + 22, menu.y + 49, menu.width - 44, 32), "Gather the company, tune the rules, or jump into a beta combat test.", CenterLeftStyle(11, muted));

            float y = menu.y + 90;
            float buttonH = 42f;
            float buttonGap = 10f;
            if (DrawTavernMenuButton(new Rect(menu.x + 24, y, menu.width - 48, buttonH), "Start Game", 1)) BeginGame();
            y += buttonH + buttonGap;
            if (DrawTavernMenuButton(new Rect(menu.x + 24, y, menu.width - 48, buttonH), "Customize Party", 2)) state.Mode = GameMode.Muster;
            y += buttonH + buttonGap;
            if (DrawTavernMenuButton(new Rect(menu.x + 24, y, menu.width - 48, buttonH), "Beta Lab", 3)) StartBetaCombatLab();
            y += buttonH + buttonGap;
            if (DrawTavernMenuButton(new Rect(menu.x + 24, y, menu.width - 48, buttonH), "Settings", 4)) showTavernSettings = !showTavernSettings;
            y += buttonH + buttonGap;
            if (DrawTavernMenuButton(new Rect(menu.x + 24, y, menu.width - 48, buttonH), "Exit Game", 5))
            {
                PlaySfx("ui", 0.6f);
                Application.Quit();
                ShowBanner("Exit requested");
            }

            if (showTavernSettings) DrawTavernSettings(new Rect(Screen.width - 390, 86, 340, 230));
        }

        private bool DrawTavernMenuButton(Rect rect, string label, int iconIndex)
        {
            bool clicked = GUI.Button(rect, "", buttonStyle);
            Rect icon = new Rect(rect.x + 12f, rect.y + 8f, 32f, 32f);
            if (!TryDrawTavernUiAtlasIcon(icon, iconIndex, Color.white.WithAlpha(GUI.enabled ? 0.90f : 0.38f)))
            {
                DrawTinyUiIcon(icon, iconIndex == 4 ? "settings" : iconIndex == 3 ? "magic" : "door", gold);
            }
            GUI.Label(new Rect(rect.x + 54f, rect.y + 13f, rect.width - 66f, 20f), FitText(label, rect.width - 66f, CenterLeftStyle(14, ink)), CenterLeftStyle(14, ink));
            return clicked;
        }

        private void DrawTavernCompanyPreview(Rect rect)
        {
            DrawRect(rect, Hex("11171b", 0.90f));
            DrawBorder(rect, line, 1);
            GUI.Label(new Rect(rect.x + 20, rect.y + 16, rect.width - 40, 28), "Tonight's Table", h2Style);
            GUI.Label(new Rect(rect.x + 22, rect.y + 46, rect.width - 44, 22), "The current four-person company waits by the hearth.", mutedStyle);
            float cardW = Mathf.Max(168f, (rect.width - 70f) / 4f);
            float cardH = Mathf.Clamp(rect.height - 122f, 186f, 250f);
            float y = rect.y + 92f;
            for (int i = 0; i < state.Party.Count; i++)
            {
                PartyMember member = state.Party[i];
                Rect card = new Rect(rect.x + 20f + i * (cardW + 10f), y, cardW, cardH);
                DrawRect(card, Hex("151b20", 0.94f));
                DrawBorder(card, MemberColor(member), 1);
                float portraitH = Mathf.Clamp(card.height * 0.48f, 88f, 118f);
                DrawPixelPortrait(new Rect(card.x + 20f, card.y + 14f, card.width - 40f, portraitH), member);
                float textY = card.y + 24f + portraitH;
                GUI.Label(new Rect(card.x + 14f, textY, card.width - 28f, 20f), FitText(member.Name, card.width - 28f, CenterLeftStyle(15, gold)), CenterLeftStyle(15, gold));
                GUI.Label(new Rect(card.x + 14f, textY + 24f, card.width - 28f, 36f), $"{DisplayRace(member.Race)} / {DisplayClass(member.ClassKey)}\n{RoleIdentityLine(member)}", CenterLeftStyle(10, ink));
                if (card.height > 214f)
                {
                    GUI.Label(new Rect(card.x + 14f, textY + 68f, card.width - 28f, 28f), $"{TrimGearName(member.WeaponName)}\n{BestSkillLabel(member)} {BestSkillValue(member)}", CenterLeftStyle(10, muted));
                }
            }
        }

        private void DrawTavernSettings(Rect rect)
        {
            DrawRect(rect, Hex("080b0d", 0.97f));
            DrawBorder(rect, teal, 1);
            GUI.Label(new Rect(rect.x + 16, rect.y + 14, rect.width - 32, 24), "Settings", h2Style);
            GUI.Label(new Rect(rect.x + 18, rect.y + 48, rect.width - 36, 20), "Audio and motion settings apply immediately.", mutedStyle);
            DrawPreferenceControls(rect.x + 18, rect.y + 84, rect.width - 36);
            GUI.Label(new Rect(rect.x + 18, rect.y + 126, rect.width - 36, 42), $"Default window: {Screen.width} x {Screen.height}\nSFX volume: {state.SfxVolumePercent}% / Reduced motion: {(state.ReducedMotion ? "on" : "off")}", CenterLeftStyle(11, ink));
            if (GUI.Button(new Rect(rect.x + 18, rect.yMax - 48, rect.width - 36, 30), "Close", smallButtonStyle)) showTavernSettings = false;
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

        private void DrawMuster()
        {
            DrawTavernBackdrop();

            Rect top = new Rect(18, 16, Screen.width - 36, 68);
            DrawRect(top, Hex("10161b", 0.86f));
            DrawBorder(top, line.WithAlpha(0.70f), 1);
            GUI.Label(new Rect(top.x + 16, top.y + 5, 300, 27), GameTitle, CenterLeftStyle(24, ink));
            GUI.Label(new Rect(top.x + 18, top.y + 34, Mathf.Max(260, top.width - 760), 18), $"Tavern Muster / {PartySummaryLine()}", CenterLeftStyle(10, gold));
            DrawPreferenceControls(top.x + 330, top.y + 18, Mathf.Max(180f, top.width - 850f), false);
            float actionsY = top.y + 16;
            if (GUI.Button(new Rect(top.xMax - 438, actionsY, 82, 34), "Tavern", smallButtonStyle)) state.Mode = GameMode.Tavern;
            if (GUI.Button(new Rect(top.xMax - 348, actionsY, 86, 34), "Beta Lab", smallButtonStyle)) StartBetaCombatLab();
            if (GUI.Button(new Rect(top.xMax - 254, actionsY, 106, 34), "Quick Start", smallButtonStyle)) QuickStart();
            if (GUI.Button(new Rect(top.xMax - 140, actionsY, 104, 34), "Begin", smallButtonStyle)) BeginGame();

            float leftW = Mathf.Min(390, Screen.width * 0.34f);
            Rect roster = new Rect(18, 100, leftW, Screen.height - 118);
            DrawPanel(roster);
            GUI.Label(new Rect(roster.x + 14, roster.y + 12, roster.width - 28, 25), "Company", h2Style);

            float y = roster.y + 48;
            for (int i = 0; i < state.Party.Count; i++)
            {
                PartyMember member = state.Party[i];
                Rect row = new Rect(roster.x + 12, y, roster.width - 24, 54);
                DrawRect(row, i == selectedBuilderIndex ? Hex("2d3438") : panel2);
                DrawBorder(row, i == selectedBuilderIndex ? gold : line, 1);
                DrawClassIcon(new Rect(row.x + 10, row.y + 9, 36, 36), member.ClassKey, member.Role, MemberColor(member));
                GUI.Label(new Rect(row.x + 56, row.y + 8, row.width - 72, 20), member.Name, labelStyle);
                GUI.Label(new Rect(row.x + 56, row.y + 29, row.width - 72, 18), $"{DisplayRace(member.Race)} / {DisplayClass(member.ClassKey)} / {SkillAdjective(BestSkillValue(member))}", mutedStyle);
                if (GUI.Button(row, GUIContent.none, GUIStyle.none)) selectedBuilderIndex = i;
                y += 61;
            }

            Rect editor = new Rect(roster.xMax + 14, 100, Screen.width - roster.xMax - 32, Screen.height - 118);
            DrawPanel(editor);
            PartyMember selected = state.Party[Mathf.Clamp(selectedBuilderIndex, 0, state.Party.Count - 1)];
            GUI.Label(new Rect(editor.x + 18, editor.y + 14, 320, 28), "Tavern Muster", h2Style);
            GUI.Label(new Rect(editor.x + 18, editor.y + 45, editor.width - 36, 42), $"Four-person company scaffold: race, class, level, gear stats, and the classic {StatPointBudget}-point attribute split.", mutedStyle);

            GUI.Label(new Rect(editor.x + 18, editor.y + 98, 80, 24), "Name", labelStyle);
            selected.Name = GUI.TextField(new Rect(editor.x + 100, editor.y + 94, 210, 30), selected.Name, 16, fieldStyle);
            if (GUI.Button(new Rect(editor.x + 326, editor.y + 94, 76, 30), "Class", smallButtonStyle))
            {
                int idx = Array.IndexOf(classOrder, selected.ClassKey);
                ApplyClass(selected, classOrder[(idx + 1 + classOrder.Length) % classOrder.Length]);
            }
            if (GUI.Button(new Rect(editor.x + 410, editor.y + 94, 76, 30), "Race", smallButtonStyle)) CycleRace(selected);
            if (GUI.Button(new Rect(editor.x + 494, editor.y + 94, 76, 30), "Origin", smallButtonStyle)) CycleOrigin(selected);
            if (GUI.Button(new Rect(editor.x + 578, editor.y + 94, 76, 30), "Sigil", smallButtonStyle)) CycleSigil(selected);
            if (GUI.Button(new Rect(editor.x + 662, editor.y + 94, 86, 30), "Name", smallButtonStyle)) selected.Name = RandomName(selected.Role);
            if (GUI.Button(new Rect(editor.x + 326, editor.y + 128, 94, 28), "Reroll Gear", smallButtonStyle)) RerollGear(selected);
            if (GUI.Button(new Rect(editor.x + 430, editor.y + 128, 92, 28), "Reroll Look", smallButtonStyle)) RerollLook(selected);
            if (GUI.Button(new Rect(editor.x + 532, editor.y + 128, 78, 28), "Color", smallButtonStyle)) CycleColor(selected);
            GUI.Label(new Rect(editor.x + 100, editor.y + 128, 220, 24), $"{DisplayRace(selected.Race)} {DisplayClass(selected.ClassKey)} / {selected.Sigil}", mutedStyle);

            DrawStatEditor(editor, selected);
            DrawSkillFocusEditor(editor, selected);

            Rect preview = new Rect(editor.x + editor.width - 286, editor.y + 116, 250, 230);
            DrawRect(preview, Hex("11171b"));
            DrawBorder(preview, line, 1);
            DrawPixelPortrait(new Rect(preview.x + 42, preview.y + 22, 166, 118), selected);
            GUI.Label(new Rect(preview.x + 18, preview.y + 148, preview.width - 36, 24), selected.Name, h2Style);
            GUI.Label(new Rect(preview.x + 18, preview.y + 176, preview.width - 36, 46), $"{DisplayRace(selected.Race)} / {DisplayClass(selected.ClassKey)}\n{RoleIdentityLine(selected)}", mutedStyle);

            Rect note = new Rect(editor.x + 18, editor.yMax - 106, editor.width - 36, 78);
            DrawRect(note, Hex("12181c"));
            DrawBorder(note, line, 1);
            GUI.Label(new Rect(note.x + 12, note.y + 10, note.width - 24, note.height - 20),
                $"{PartyWeaknessLine()}\n{ProgressLine(selected)} / {EffectiveStatsLine(selected)}\n{GearShortLine(selected)} / {BestSkillLabel(selected)} {BestSkillValue(selected)}. All art and rules are original.",
                mutedStyle);
        }

        private void DrawTavernBackdrop()
        {
            float w = Screen.width;
            float h = Screen.height;
            if (tavernBackdropArt != null)
            {
                GUI.DrawTexture(new Rect(0, 0, w, h), tavernBackdropArt, ScaleMode.ScaleAndCrop);
                DrawRect(new Rect(0, 0, w, h), Hex("050708", 0.18f));
                DrawRect(new Rect(0, 0, w * 0.46f, h), Hex("050708", 0.24f));
                DrawRect(new Rect(0, 0, w, h), Hex("d7a84e", 0.025f + Mathf.PingPong(Time.time * 0.018f, 0.020f)));
                return;
            }
            DrawRect(new Rect(0, 0, w, h), Hex("08090a"));
            DrawRect(new Rect(0, 0, w, h * 0.62f), Hex("1a1411"));
            DrawRect(new Rect(0, h * 0.58f, w, h * 0.42f), Hex("201812"));

            for (int i = 0; i < 18; i++)
            {
                float x = i * (w / 17f);
                Color beam = Hex(i % 2 == 0 ? "2d2118" : "251b14", 0.88f);
                DrawRect(new Rect(x - 5f, 0, 10f, h), beam);
            }

            for (int y = 0; y < 6; y++)
            {
                float yy = h * 0.16f + y * 54f;
                DrawRect(new Rect(0, yy, w, 3f), Hex("3d2b1d", 0.58f));
            }

            Rect hearth = new Rect(w * 0.62f, h * 0.18f, w * 0.18f, h * 0.24f);
            DrawRect(hearth, Hex("15110e", 0.88f));
            DrawBorder(hearth, Hex("6a4a2b", 0.72f), 2);
            DrawRect(new Rect(hearth.x + hearth.width * 0.18f, hearth.y + hearth.height * 0.58f, hearth.width * 0.64f, hearth.height * 0.18f), Hex("5b2c20", 0.88f));
            for (int i = 0; i < 4; i++)
            {
                float fx = hearth.x + hearth.width * (0.28f + i * 0.12f);
                float flameH = hearth.height * (0.16f + 0.04f * Mathf.Sin(Time.time * 2f + i));
                DrawRect(new Rect(fx, hearth.y + hearth.height * 0.46f - flameH, hearth.width * 0.06f, flameH), Hex(i % 2 == 0 ? "d7a84e" : "c65c3b", 0.88f));
            }

            Rect bar = new Rect(w * 0.08f, h * 0.72f, w * 0.58f, h * 0.08f);
            DrawRect(bar, Hex("3a2418", 0.92f));
            DrawRect(new Rect(bar.x, bar.y, bar.width, 8f), Hex("6a4429", 0.92f));
            DrawBorder(bar, Hex("8a5c35", 0.46f), 1);
            for (int i = 0; i < 8; i++)
            {
                DrawTavernMug(bar.x + 28f + i * 56f, bar.y - 18f + (i % 2) * 4f, 0.72f);
            }

            DrawTavernPatron(w * 0.16f, h * 0.58f, Hex("58b7a5", 0.60f), "hat");
            DrawTavernPatron(w * 0.28f, h * 0.55f, Hex("b94b56", 0.56f), "cloak");
            DrawTavernPatron(w * 0.48f, h * 0.57f, Hex("d7a84e", 0.52f), "shield");
            DrawTavernPatron(w * 0.83f, h * 0.59f, Hex("9ad6e8", 0.46f), "staff");

            Rect stage = new Rect(w * 0.70f, h * 0.62f, w * 0.23f, h * 0.09f);
            DrawRect(stage, Hex("2f2018", 0.90f));
            DrawBorder(stage, Hex("8a5c35", 0.52f), 1);
            DrawTavernBand(new Rect(stage.x + 16f, stage.y - 46f, stage.width - 32f, 46f));

            DrawRect(new Rect(0, 0, w, h), Hex("050708", 0.34f));
            DrawRect(new Rect(0, 0, w, h), Hex("d7a84e", 0.03f + Mathf.PingPong(Time.time * 0.02f, 0.025f)));
        }

        private void DrawTavernMug(float x, float y, float alpha)
        {
            DrawRect(new Rect(x, y, 14f, 18f), Hex("c79a5d", alpha));
            DrawRect(new Rect(x + 2f, y + 3f, 10f, 4f), Hex("f3ead7", alpha * 0.72f));
            DrawBorder(new Rect(x - 2f, y + 4f, 18f, 12f), Hex("6a4429", alpha), 1);
            DrawRect(new Rect(x + 14f, y + 6f, 5f, 8f), Hex("c79a5d", alpha * 0.82f));
        }

        private void DrawTavernPatron(float x, float y, Color body, string prop)
        {
            DrawRect(new Rect(x - 22f, y + 32f, 44f, 8f), Hex("030405", 0.42f));
            DrawRect(new Rect(x - 10f, y - 2f, 20f, 30f), body);
            DrawRect(new Rect(x - 7f, y - 20f, 14f, 14f), Hex("d9a67b", 0.78f));
            DrawRect(new Rect(x - 16f, y + 8f, 9f, 23f), Color.Lerp(body, retroBlack, 0.25f));
            DrawRect(new Rect(x + 7f, y + 8f, 9f, 23f), Color.Lerp(body, retroBlack, 0.25f));
            if (prop == "hat")
            {
                DrawRect(new Rect(x - 14f, y - 24f, 28f, 5f), Hex("514538", 0.82f));
                DrawRect(new Rect(x - 8f, y - 31f, 16f, 8f), Hex("514538", 0.82f));
            }
            else if (prop == "cloak")
            {
                DrawRect(new Rect(x - 15f, y - 1f, 30f, 37f), Hex("4c2534", 0.42f));
            }
            else if (prop == "shield")
            {
                DrawRect(new Rect(x + 13f, y + 8f, 15f, 21f), Hex("8a5c35", 0.72f));
                DrawBorder(new Rect(x + 13f, y + 8f, 15f, 21f), Hex("d7a84e", 0.42f), 1);
            }
            else if (prop == "staff")
            {
                DrawRect(new Rect(x + 15f, y - 21f, 4f, 50f), Hex("8a5c35", 0.76f));
                DrawRect(new Rect(x + 12f, y - 28f, 10f, 10f), Hex("9ad6e8", 0.56f));
            }
        }

        private void DrawTavernBand(Rect area)
        {
            Color musician = Hex("b7aa90", 0.56f);
            for (int i = 0; i < 3; i++)
            {
                float x = area.x + area.width * (0.18f + i * 0.30f);
                float y = area.y + 20f + (i % 2) * 3f;
                DrawRect(new Rect(x - 6f, y - 14f, 12f, 12f), Hex("d9a67b", 0.62f));
                DrawRect(new Rect(x - 8f, y - 2f, 16f, 24f), musician);
                if (i == 0)
                {
                    DrawRect(new Rect(x - 21f, y + 3f, 21f, 5f), Hex("8a5c35", 0.72f));
                    DrawRect(new Rect(x - 25f, y - 1f, 8f, 12f), Hex("6a4429", 0.72f));
                }
                else if (i == 1)
                {
                    DrawRect(new Rect(x - 12f, y + 9f, 24f, 14f), Hex("6a4429", 0.72f));
                    DrawBorder(new Rect(x - 12f, y + 9f, 24f, 14f), Hex("d7a84e", 0.42f), 1);
                }
                else
                {
                    DrawRect(new Rect(x + 10f, y - 14f, 4f, 42f), Hex("8a5c35", 0.72f));
                    DrawRect(new Rect(x + 6f, y - 20f, 12f, 9f), Hex("6a4429", 0.72f));
                }
            }
        }

        private void DrawStatEditor(Rect editor, PartyMember member)
        {
            int total = member.Stats.Total;
            int cap = Mathf.Max(StatPointBudget, total + Mathf.Max(0, member.StatPoints));
            string pointNote = member.StatPoints > 0 ? $"  +{member.StatPoints} earned" : "";
            GUI.Label(new Rect(editor.x + 18, editor.y + 146, 340, 24), $"Attributes {total}/{cap}{pointNote}", h2Style);
            DrawSingleStat(editor.x + 18, editor.y + 186, "Strength", member.Stats.Strength, -1, member);
            DrawSingleStat(editor.x + 18, editor.y + 226, "Intelligence", member.Stats.Intelligence, -2, member);
            DrawSingleStat(editor.x + 18, editor.y + 266, "Agility", member.Stats.Dexterity, -3, member);
            DrawSingleStat(editor.x + 18, editor.y + 306, "Health", member.Stats.Health, -4, member);
        }

        private void DrawSingleStat(float x, float y, string label, int value, int code, PartyMember member)
        {
            GUI.Label(new Rect(x, y + 4, 116, 24), label, labelStyle);
            if (GUI.Button(new Rect(x + 122, y, 32, 30), "-", smallButtonStyle)) ChangeStat(member, code, -1);
            GUI.Label(new Rect(x + 164, y + 4, 42, 24), value.ToString(), labelStyle);
            if (GUI.Button(new Rect(x + 210, y, 32, 30), "+", smallButtonStyle)) ChangeStat(member, code, 1);
        }

        private void DrawSkillFocusEditor(Rect editor, PartyMember member)
        {
            GUI.Label(new Rect(editor.x + 18, editor.y + 352, 360, 24), $"Talents / best: {BestSkillLabel(member)} {BestSkillValue(member)} ({SkillAdjective(BestSkillValue(member))})", h2Style);
            string[] keys = { "arms", "missile", "mend", "ember", "hex", "guard" };
            for (int i = 0; i < keys.Length; i++)
            {
                Rect r = new Rect(editor.x + 18 + i * 78, editor.y + 386, 72, 30);
                if (GUI.Button(r, keys[i], smallButtonStyle))
                {
                    BoostTalent(member, keys[i]);
                }
            }
            GUI.Label(new Rect(editor.x + 18, editor.y + 422, 560, 34), "Talent buttons nudge starting skills upward; use in battle still matters most.", mutedStyle);
        }

        private void BoostTalent(PartyMember member, string key)
        {
            if (member.Skills == null) member.Skills = new SkillSet().Normalize();
            int current = SkillValue(member.Skills, key);
            SetSkill(member.Skills, key, Mathf.Clamp(Mathf.Max(10, current + 2), 1, 30));
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
            if (state?.Party == null) return $"Muster a {PartySize}-person tavern company.";
            int front = state.Party.Count(p => p.Role == "shield" || p.Role == "ward" || p.Role == "pike");
            int ranged = state.Party.Count(p => p.Role == "bow" || p.Range >= 3);
            int priests = state.Party.Count(p => CasterKnowsSchool(p.Spell, "mend") || p.Role == "mender");
            int arcanists = state.Party.Count(p => CasterKnowsSchool(p.Spell, "ember") || CasterKnowsSchool(p.Spell, "hex"));
            int level = state.Party.Count == 0 ? 1 : Mathf.Max(1, Mathf.RoundToInt((float)state.Party.Sum(p => Mathf.Max(1, p.Level)) / state.Party.Count));
            return $"{PartySize}-person company: front {front} / ranged {ranged} / priest {priests} / arcane {arcanists} / avg level {level}. {PartyWeaknessLine()}";
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
            member.Level = Mathf.Max(1, member.Level);
            int strength = EffectiveStrength(member);
            int intelligence = EffectiveIntelligence(member);
            int agility = EffectiveAgility(member);
            int health = EffectiveHealth(member);
            member.MaxHp = health + 16 + strength / 2 + (member.Level - 1) * 4;
            member.MaxMana = string.IsNullOrEmpty(member.Spell) ? 0 : intelligence + 8 + (member.Level - 1) * 3;
            member.Hp = Mathf.Clamp(member.Hp <= 0 ? member.MaxHp : member.Hp, 0, member.MaxHp);
            member.Mana = Mathf.Clamp(member.Mana, 0, member.MaxMana);
            int baseMin = member.WeaponDamageMin > 0 ? member.WeaponDamageMin : 2;
            int baseMax = member.WeaponDamageMax > 0 ? member.WeaponDamageMax : 5;
            member.DamageMin = Mathf.Max(1, baseMin + Mathf.Max(-1, member.WeaponBonus) + strength / 8);
            member.DamageMax = Mathf.Max(member.DamageMin, baseMax + Mathf.Max(-1, member.WeaponBonus) + strength / 5);
            member.Power = member.DamageMin + strength / 4 + Mathf.Max(-1, member.WeaponBonus) + WeaponPowerBonus(member.WeaponName);
            member.Defense = (health + strength) / 9 + Mathf.Max(0, member.ArmorBonus);
            member.AttackSpeed = Mathf.Clamp(member.WeaponAttackSpeed + agility / 4 + ArmorAgilityModifier(member.ArmorName), 3, 20);
            member.Agility = Mathf.Max(1, agility / 3 + 2 + ArmorAgilityModifier(member.ArmorName) + member.AttackSpeed / 8);
            member.Movement = Mathf.Clamp(CombatMoveAllowance + (agility >= 18 ? 1 : 0) + (member.AttackSpeed >= 15 ? 1 : 0) - (member.ArmorBonus >= 4 ? 1 : 0), 2, 5);
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

        private int ExperienceForNextLevel(int level)
        {
            level = Mathf.Max(1, level);
            return 60 + level * level * 40;
        }

        private int StartingRange(string role)
        {
            if (role == "bow") return 4;
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
                case "warlock": return "hex|pact";
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
            switch ((classKey ?? "").ToLowerInvariant())
            {
                case "wizard": return "ember|hex";
                case "mage": return "ember";
                case "warlock": return "hex";
                case "priest": return "mend";
                case "paladin": return "mend";
                default: return "";
            }
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

        private MapData GenerateMap(int depth, int seed)
        {
            System.Random mapRng = new System.Random(seed + depth * 6113);
            MapData map = new MapData
            {
                Width = ExploreW,
                Height = ExploreH,
                Depth = depth,
                Tiles = Enumerable.Repeat(0, ExploreW * ExploreH).ToList(),
                Objects = new List<MapObject>(),
                StartX = ExploreW / 2,
                StartY = ExploreH / 2
            };

            int x = map.StartX;
            int y = map.StartY;
            SetTile(map, x, y, 1);
            int districts = 7 + Mathf.Min(depth, 5);
            List<Point> anchors = new List<Point> { new Point(x, y) };
            for (int d = 0; d < districts; d++)
            {
                Point anchor = new Point(mapRng.Next(4, ExploreW - 4), mapRng.Next(3, ExploreH - 3));
                anchors.Add(anchor);
                CarveRoad(map, x, y, anchor.X, anchor.Y, mapRng);
                x = anchor.X;
                y = anchor.Y;
                int steps = 150 + depth * 12 + mapRng.Next(50);
                for (int i = 0; i < steps; i++)
                {
                    int dir = mapRng.Next(4);
                    if (dir == 0) x++; else if (dir == 1) x--; else if (dir == 2) y++; else y--;
                    x = Mathf.Clamp(x, 2, ExploreW - 3);
                    y = Mathf.Clamp(y, 2, ExploreH - 3);
                    SetTile(map, x, y, 1);
                    if (mapRng.NextDouble() < 0.27)
                    {
                        int radius = mapRng.NextDouble() < 0.3 ? 2 : 1;
                        CarveRoom(map, x, y, radius, mapRng);
                    }
                }
            }
            CarveRoad(map, map.StartX, map.StartY, anchors.Last().X, anchors.Last().Y, mapRng);
            int loopRoads = 3 + Mathf.Min(depth, 5);
            for (int i = 0; i < loopRoads && anchors.Count > 2; i++)
            {
                Point a = anchors[mapRng.Next(anchors.Count)];
                Point b = anchors[mapRng.Next(anchors.Count)];
                if (Distance(a.X, a.Y, b.X, b.Y) < 8) continue;
                CarveRoad(map, a.X, a.Y, b.X, b.Y, mapRng);
                if (mapRng.NextDouble() < 0.65)
                {
                    int cx = Mathf.Clamp((a.X + b.X) / 2 + mapRng.Next(-2, 3), 2, ExploreW - 3);
                    int cy = Mathf.Clamp((a.Y + b.Y) / 2 + mapRng.Next(-2, 3), 2, ExploreH - 3);
                    CarveRoom(map, cx, cy, mapRng.NextDouble() < 0.35 ? 3 : 2, mapRng);
                }
            }

            List<Point> open = new List<Point>();
            for (int yy = 1; yy < ExploreH - 1; yy++)
            for (int xx = 1; xx < ExploreW - 1; xx++)
            {
                if (TileAt(map, xx, yy) == 1 && Distance(xx, yy, map.StartX, map.StartY) > 3) open.Add(new Point(xx, yy));
            }

            PlaceObjectsInZone(map, open, mapRng, "salt-cisterns", ObjectType.Encounter, 2 + depth / 2);
            PlaceObjectsInZone(map, open, mapRng, "salt-cisterns", ObjectType.Stairs, depth == 1 ? 1 : 0);
            PlaceObjectsInZone(map, open, mapRng, "green-shrine-road", ObjectType.Shrine, 2);
            PlaceObjectsInZone(map, open, mapRng, "green-shrine-road", ObjectType.Camp, 1);
            PlaceObjectsInZone(map, open, mapRng, "old-quarry", ObjectType.Cache, 2);
            PlaceObjectsInZone(map, open, mapRng, "old-quarry", ObjectType.Ruin, 2);
            PlaceObjectsInZone(map, open, mapRng, "dusk-market", ObjectType.Cache, 2);
            PlaceObjectsInZone(map, open, mapRng, "dusk-market", ObjectType.Cave, 1);
            PlaceObjectsInZone(map, open, mapRng, "glass-warrens", ObjectType.Obelisk, 1);
            PlaceObjectsInZone(map, open, mapRng, "glass-warrens", ObjectType.Encounter, 2);
            PlaceObjectsInZone(map, open, mapRng, "ash-fen", ObjectType.Shrine, 1);
            PlaceObjectsInZone(map, open, mapRng, "ash-fen", ObjectType.Encounter, 2);
            PlaceObjectsInZone(map, open, mapRng, "red-gate", ObjectType.Encounter, 2 + depth / 2);
            PlaceObjectsInZone(map, open, mapRng, "red-gate", ObjectType.Ruin, 1 + depth / 2);
            PlaceObjectsInZone(map, open, mapRng, "red-gate", ObjectType.Obelisk, 1);

            PlaceObjects(map, open, mapRng, ObjectType.Cache, 6 + Mathf.Min(depth * 2, 7));
            PlaceObjects(map, open, mapRng, ObjectType.Shrine, 2 + depth / 2);
            PlaceObjects(map, open, mapRng, ObjectType.Encounter, 7 + depth * 2);
            PlaceObjects(map, open, mapRng, ObjectType.Stairs, 1);
            PlaceObjects(map, open, mapRng, ObjectType.Camp, 2 + depth / 2);
            PlaceObjects(map, open, mapRng, ObjectType.Ruin, 5 + Mathf.Min(depth, 4));
            PlaceObjects(map, open, mapRng, ObjectType.Obelisk, 2 + depth / 2);
            PlaceObjects(map, open, mapRng, ObjectType.Cave, 2 + Mathf.Min(depth / 2, 2));
            PlaceObjects(map, open, mapRng, ObjectType.Bridge, 2 + Mathf.Min(depth / 2, 2));
            map.Objects.Add(new MapObject(map.StartX, map.StartY, ObjectType.Camp));
            int townY = Mathf.Max(1, map.StartY - 4);
            for (int yy = townY; yy <= map.StartY; yy++) SetTile(map, map.StartX, yy, 1);
            map.Objects.Add(new MapObject(map.StartX, townY, ObjectType.Town));
            return map;
        }

        private void CarveRoad(MapData map, int ax, int ay, int bx, int by, System.Random mapRng)
        {
            int x = ax;
            int y = ay;
            int guard = 0;
            while ((x != bx || y != by) && guard++ < ExploreW + ExploreH)
            {
                SetTile(map, Mathf.Clamp(x, 1, ExploreW - 2), Mathf.Clamp(y, 1, ExploreH - 2), 1);
                if (x != bx && (y == by || mapRng.NextDouble() < 0.55)) x += Math.Sign(bx - x);
                else if (y != by) y += Math.Sign(by - y);
                SetTile(map, Mathf.Clamp(x, 1, ExploreW - 2), Mathf.Clamp(y, 1, ExploreH - 2), 1);
                if (mapRng.NextDouble() < 0.45)
                {
                    SetTile(map, Mathf.Clamp(x + 1, 1, ExploreW - 2), Mathf.Clamp(y, 1, ExploreH - 2), 1);
                    SetTile(map, Mathf.Clamp(x, 1, ExploreW - 2), Mathf.Clamp(y + 1, 1, ExploreH - 2), 1);
                }
            }
        }

        private void CarveRoom(MapData map, int cx, int cy, int radius, System.Random mapRng)
        {
            for (int yy = -radius; yy <= radius; yy++)
            for (int xx = -radius; xx <= radius; xx++)
            {
                if (Mathf.Abs(xx) + Mathf.Abs(yy) <= radius + 1 && mapRng.NextDouble() < 0.82)
                {
                    SetTile(map, Mathf.Clamp(cx + xx, 1, ExploreW - 2), Mathf.Clamp(cy + yy, 1, ExploreH - 2), 1);
                }
            }
        }

        private void PlaceObjects(MapData map, List<Point> open, System.Random mapRng, ObjectType type, int count)
        {
            for (int i = 0; i < count && open.Count > 0; i++)
            {
                int index = mapRng.Next(open.Count);
                Point point = open[index];
                open.RemoveAt(index);
                if (ObjectAt(map, point.X, point.Y) == null) map.Objects.Add(new MapObject(point.X, point.Y, type));
            }
        }

        private void PlaceObjectsInZone(MapData map, List<Point> open, System.Random mapRng, string zoneId, ObjectType type, int count)
        {
            if (count <= 0 || map == null || open == null || open.Count == 0) return;
            for (int i = 0; i < count; i++)
            {
                List<int> candidates = new List<int>();
                for (int n = 0; n < open.Count; n++)
                {
                    Point point = open[n];
                    WorldZone zone = ZoneFor(point.X, point.Y, map, map.Depth);
                    if (zone.Id == zoneId && ObjectAt(map, point.X, point.Y) == null) candidates.Add(n);
                }
                if (candidates.Count == 0) return;
                int openIndex = candidates[mapRng.Next(candidates.Count)];
                Point chosen = open[openIndex];
                open.RemoveAt(openIndex);
                map.Objects.Add(new MapObject(chosen.X, chosen.Y, type));
            }
        }

        private void EnsureWorldLandmarks()
        {
            if (state?.Map == null) return;
            if (state.Map.Objects == null) state.Map.Objects = new List<MapObject>();
            int landmarkCount = state.Map.Objects.Count(o => o.Type == ObjectType.Obelisk || o.Type == ObjectType.Ruin || o.Type == ObjectType.Bridge || o.Type == ObjectType.Cave);
            if (landmarkCount >= 5) return;

            List<Point> open = new List<Point>();
            for (int yy = 1; yy < state.Map.Height - 1; yy++)
            for (int xx = 1; xx < state.Map.Width - 1; xx++)
            {
                if (TileAt(state.Map, xx, yy) == 1 && Distance(xx, yy, state.Map.StartX, state.Map.StartY) > 5 && ObjectAt(state.Map, xx, yy) == null)
                {
                    open.Add(new Point(xx, yy));
                }
            }

            System.Random mapRng = new System.Random(state.Seed + state.Depth * 1709 + 27);
            PlaceObjects(state.Map, open, mapRng, ObjectType.Ruin, 3);
            PlaceObjects(state.Map, open, mapRng, ObjectType.Obelisk, 2);
            PlaceObjects(state.Map, open, mapRng, ObjectType.Bridge, 1);
            PlaceObjects(state.Map, open, mapRng, ObjectType.Cave, 1);
        }

        private void DrawGameChrome(string mode)
        {
            Rect top = new Rect(12, 10, Screen.width - 24, 58);
            DrawRect(top, Hex("10161b", 0.88f));
            DrawBorder(top, line.WithAlpha(0.72f), 1);
            DrawGameLogo(new Rect(top.x + 12, top.y + 9, 40, 40));
            float buttonW = Screen.width < 1180 ? 48f : 54f;
            float buttonGap = 6f;
            float actionsW = buttonW * 3f + buttonGap * 2f;
            float actionsX = top.xMax - actionsW - 10f;
            float resourceW = Screen.width < 1240 ? 62f : 76f;
            float resourceGap = Screen.width < 1240 ? 5f : 8f;
            float resourcesW = resourceW * 3f + resourceGap * 2f;
            float resourcesX = actionsX - resourcesW - 10f;
            float titleX = top.x + 62f;
            float titleW = Mathf.Max(180f, Mathf.Min(310f, resourcesX - titleX - 14f));
            GUI.Label(new Rect(titleX, top.y + 5, titleW, 28), GameTitle, CenterLeftStyle(24, ink));
            string modeLine = state.Mode == GameMode.Explore ? $"{StoryChapterTitle()} / Depth {state.Depth}" : $"{GameSubtitle} / Depth {state.Depth} / {mode}";
            GUI.Label(new Rect(titleX + 2, top.y + 34, titleW, 18), modeLine, CenterLeftStyle(10, muted));

            float preferenceX = titleX + titleW + 12f;
            float preferenceW = resourcesX - preferenceX - 12f;
            if (preferenceW >= 170f)
            {
                DrawPreferenceControls(preferenceX, top.y + 18, preferenceW, false);
            }

            DrawResource(new Rect(resourcesX, top.y + 8, resourceW, 42), "Gold", state.Gold.ToString());
            DrawResource(new Rect(resourcesX + resourceW + resourceGap, top.y + 8, resourceW, 42), "Supplies", state.Supplies.ToString());
            DrawResource(new Rect(resourcesX + (resourceW + resourceGap) * 2f, top.y + 8, resourceW, 42), "Elixirs", state.Elixirs.ToString());
            if (GUI.Button(new Rect(actionsX, top.y + 12, buttonW, 34), "Save", smallButtonStyle)) SaveGame();
            if (GUI.Button(new Rect(actionsX + buttonW + buttonGap, top.y + 12, buttonW, 34), "Load", smallButtonStyle)) LoadGame();
            if (GUI.Button(new Rect(actionsX + (buttonW + buttonGap) * 2f, top.y + 12, buttonW, 34), "New", smallButtonStyle)) NewMuster();
        }

        private void DrawPreferenceControls(float x, float y, float maxWidth = 300f, bool showPulse = true)
        {
            if (state == null) return;
            bool soundOn = !state.SfxMuted;
            bool compact = maxWidth < 340f;
            float sfxW = compact ? 54f : 70f;
            float volumeW = compact ? 54f : 64f;
            float testW = maxWidth >= 330f ? 52f : 0f;
            bool nextSoundOn = GUI.Toggle(new Rect(x, y, sfxW, 24), soundOn, " SFX");
            if (nextSoundOn != soundOn)
            {
                ToggleSfxMute();
            }

            if (GUI.Button(new Rect(x + sfxW + 6f, y - 3, volumeW, 28), $"{state.SfxVolumePercent}%", smallButtonStyle))
            {
                CycleSfxVolume();
            }

            if (testW > 0f && GUI.Button(new Rect(x + sfxW + volumeW + 12f, y - 3, testW, 28), "Test", smallButtonStyle))
            {
                TestSfx();
            }

            float motionX = x + sfxW + volumeW + (testW > 0f ? testW + 20f : 14f);
            float motionW = Mathf.Max(compact ? 76f : 118f, maxWidth - (motionX - x));
            string motionLabel = motionW < 116f ? " Motion" : " Reduced Motion";
            state.ReducedMotion = GUI.Toggle(new Rect(motionX, y, motionW, 24), state.ReducedMotion, motionLabel);
            if (showPulse) DrawSfxPulse(new Rect(x, y + 29f, Mathf.Min(maxWidth, 220f), 18f));
        }

        private void DrawSfxPulse(Rect rect)
        {
            if (rect.width < 120f) return;
            float age = Time.realtimeSinceStartup - lastSfxAt;
            bool live = !string.IsNullOrEmpty(lastSfxKey) && age >= 0f && age < 1.35f;
            Color color = live ? Color.Lerp(teal, gold, Mathf.Clamp01(1f - age / 1.35f)) : (state != null && state.SfxMuted ? blood : line);
            string label = state != null && state.SfxMuted ? "SFX muted" : live ? $"SFX: {lastSfxKey} {Mathf.RoundToInt(lastSfxVolume * 100f)}%" : "SFX ready";
            DrawRect(rect, Hex("080b0d", live ? 0.92f : 0.58f));
            DrawBorder(rect, color.WithAlpha(live ? 0.95f : 0.50f), 1);
            if (live)
            {
                DrawRect(new Rect(rect.x + 4, rect.y + 4, Mathf.Clamp(rect.width * (1f - age / 1.35f), 8f, rect.width - 8f), rect.height - 8), color.WithAlpha(0.28f));
            }
            GUI.Label(new Rect(rect.x + 7, rect.y + 1, rect.width - 14, rect.height), label, CenterLeftStyle(10, live ? cursorWhite : muted));
        }

        private void DrawResource(Rect rect, string label, string value)
        {
            DrawRect(rect, Hex("171c20"));
            DrawBorder(rect, line, 1);
            Color icon = label == "Gold" ? gold : label == "Supplies" ? moss : teal;
            Rect iconRect = new Rect(rect.x + 5, rect.y + 6, 15, 15);
            if (!TryDrawInventoryConsumableAtlasIcon(iconRect, ResourceConsumableIconIndex(label), Color.white.WithAlpha(0.94f)))
            {
                DrawRect(new Rect(rect.x + 8, rect.y + 10, 8, 8), icon);
            }
            GUI.Label(new Rect(rect.x + 23, rect.y + 4, rect.width - 27, 15), label == "Supplies" && rect.width < 78 ? "Sup" : label, CenterLeftStyle(9, muted));
            GUI.Label(new Rect(rect.x + 6, rect.y + 21, rect.width - 12, 18), value, CenterLeftStyle(13, ink));
        }

        private void DrawGameLogo(Rect rect)
        {
            DrawRect(rect, Hex("080b0d", 0.94f));
            DrawBorder(rect, teal, 2);
            DrawBorder(Pad(rect, 5f), gold.WithAlpha(0.74f), 1);
            Rect flame = new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.18f, rect.width * 0.32f, rect.height * 0.42f);
            DrawRect(new Rect(flame.x + flame.width * 0.34f, flame.y, flame.width * 0.32f, flame.height * 0.94f), ember);
            DrawRect(new Rect(flame.x + flame.width * 0.16f, flame.y + flame.height * 0.25f, flame.width * 0.26f, flame.height * 0.62f), gold);
            DrawRect(new Rect(flame.x + flame.width * 0.58f, flame.y + flame.height * 0.34f, flame.width * 0.28f, flame.height * 0.54f), blood);
            Rect hall = new Rect(rect.x + rect.width * 0.22f, rect.y + rect.height * 0.58f, rect.width * 0.56f, rect.height * 0.22f);
            DrawRect(hall, Hex("3c4544"));
            DrawRect(new Rect(hall.x + hall.width * 0.15f, hall.y - hall.height * 0.62f, hall.width * 0.16f, hall.height * 0.62f), Hex("a9b0a2"));
            DrawRect(new Rect(hall.x + hall.width * 0.68f, hall.y - hall.height * 0.62f, hall.width * 0.16f, hall.height * 0.62f), Hex("a9b0a2"));
            GUI.Label(new Rect(rect.x, rect.yMax - rect.height * 0.30f, rect.width, rect.height * 0.26f), "AH", CenterStyle(10, ink));
        }

        private void DrawExplore()
        {
            boardRect = GetBoardRect();
            DrawPanel(boardRect);
            Rect grid = BoardInnerRect(boardRect, ExploreW, ExploreH);
            float cell = Mathf.Min(grid.width / ExploreW, grid.height / ExploreH);
            grid.width = cell * ExploreW;
            grid.height = cell * ExploreH;

            for (int y = 0; y < ExploreH; y++)
            for (int x = 0; x < ExploreW; x++)
            {
                Rect c = new Rect(grid.x + x * cell, grid.y + y * cell, cell, cell);
                int distance = Distance(x, y, state.PlayerX, state.PlayerY);
                bool visible = distance <= ExploreRevealRadius;
                int tile = TileAt(state.Map, x, y);
                DrawRect(c, ExploreTileBaseColor(x, y, tile, visible));
                string tileKind = visible ? ExploreTileKind(x, y, tile) : "";
                if (visible) TryDrawExploreEnvironmentTile(c, x, y, tile, tileKind);
                DrawBorder(c, Hex("050708", visible ? 0.24f : 0.44f), 1);
                if (visible)
                {
                    DrawExploreTileMotif(c, x, y, tile);
                    DrawExploreTileEdges(c, x, y, tile);
                    DrawExploreDistanceShade(c, distance);
                }
                else
                {
                    DrawExploreFogMotif(c, x, y);
                }
            }

            foreach (MapObject obj in state.Map.Objects)
            {
                if (Distance(obj.X, obj.Y, state.PlayerX, state.PlayerY) > ExploreRevealRadius) continue;
                Rect objectCell = new Rect(grid.x + obj.X * cell, grid.y + obj.Y * cell, cell, cell);
                DrawExploreObject(Pad(objectCell, -cell * 0.18f), obj.Type);
            }

            DrawExploreMovementHints(grid, cell);
            PartyMember lead = state.Party.Count > 0 ? state.Party[0] : null;
            Color leadColor = lead != null ? MemberColor(lead) : teal;
            string leadSigil = lead != null ? lead.Sigil : "bar";
            string leadRole = lead != null ? lead.Role : "shield";
            Rect playerCell = new Rect(grid.x + state.PlayerX * cell, grid.y + state.PlayerY * cell, cell, cell);
            DrawToken(Pad(playerCell, -cell * 0.26f), leadRole, leadColor, true, "", leadSigil);
            DrawExploreRegionStrip(grid);
            DrawExploreHover(grid, cell);
            HandleExploreMouse(grid, cell);
        }

        private void DrawExploreMovementHints(Rect grid, float cell)
        {
            Point[] steps =
            {
                new Point(state.PlayerX, state.PlayerY - 1),
                new Point(state.PlayerX - 1, state.PlayerY),
                new Point(state.PlayerX + 1, state.PlayerY),
                new Point(state.PlayerX, state.PlayerY + 1)
            };

            foreach (Point step in steps)
            {
                if (step.X < 0 || step.X >= ExploreW || step.Y < 0 || step.Y >= ExploreH) continue;
                Rect tile = new Rect(grid.x + step.X * cell, grid.y + step.Y * cell, cell, cell);
                bool passable = TileAt(state.Map, step.X, step.Y) == 1;
                Color color = passable ? teal : Hex("8a5c35");
                DrawBorder(Pad(tile, cell * 0.16f), color.WithAlpha(passable ? 0.70f : 0.42f), 1);
                if (passable)
                {
                    MapObject obj = ObjectAt(state.Map, step.X, step.Y);
                    if (obj != null) DrawBorder(Pad(tile, cell * 0.06f), ObjectColor(obj.Type).WithAlpha(0.72f), 1);
                }
            }
        }

        private void DrawExploreRegionStrip(Rect grid)
        {
            Rect strip = new Rect(grid.x + 8, Mathf.Max(grid.y + 8, 76f), grid.width - 16, 28);
            DrawRect(strip, Hex("030405", 0.66f));
            DrawBorder(strip, Hex("253029", 0.82f), 1);
            WorldZone zone = ZoneAt(state.PlayerX, state.PlayerY);
            string region = zone.Name;
            string underfoot = ExploreUnderfootLine(state.PlayerX, state.PlayerY);
            GUI.Label(new Rect(strip.x + 10, strip.y + 4, strip.width * 0.25f, 18), region, CenterLeftStyle(13, gold));
            GUI.Label(new Rect(strip.x + strip.width * 0.25f + 10, strip.y + 5, strip.width * 0.13f, 16), ZoneDangerText(zone), CenterLeftStyle(10, ZoneDangerColor(zone)));
            GUI.Label(new Rect(strip.x + strip.width * 0.39f, strip.y + 5, strip.width * 0.34f, 16), underfoot, CenterLeftStyle(11, muted));
            GUI.Label(new Rect(strip.xMax - 206, strip.y + 5, 196, 16), StoryChapterTitle(), CenterLeftStyle(10, muted));
        }

        private void DrawExploreHover(Rect grid, float cell)
        {
            if (Event.current == null || !grid.Contains(Event.current.mousePosition)) return;
            int x = Mathf.FloorToInt((Event.current.mousePosition.x - grid.x) / cell);
            int y = Mathf.FloorToInt((Event.current.mousePosition.y - grid.y) / cell);
            if (x < 0 || x >= ExploreW || y < 0 || y >= ExploreH) return;

            bool visible = Distance(x, y, state.PlayerX, state.PlayerY) <= ExploreRevealRadius;
            Rect tile = new Rect(grid.x + x * cell, grid.y + y * cell, cell, cell);
            DrawBorder(Pad(tile, cell * 0.04f), visible ? cursorWhite : muted, 2);

            string text = visible ? ExploreLookLine(x, y) : "unseen dark";
            Rect box = new Rect(Mathf.Clamp(Event.current.mousePosition.x + 18, 8, Screen.width - 352), Mathf.Clamp(Event.current.mousePosition.y + 14, 8, Screen.height - 104), 334, 82);
            DrawRect(box, Hex("080b0d", 0.96f));
            DrawBorder(box, visible ? gold : line, 1);
            GUI.Label(new Rect(box.x + 9, box.y + 6, box.width - 18, box.height - 12), text, CenterLeftStyle(12, ink));
        }

        private void HandleExploreMouse(Rect grid, float cell)
        {
            Event e = Event.current;
            if (e.type != EventType.MouseDown || !grid.Contains(e.mousePosition)) return;
            int x = Mathf.FloorToInt((e.mousePosition.x - grid.x) / cell);
            int y = Mathf.FloorToInt((e.mousePosition.y - grid.y) / cell);
            int dx = x - state.PlayerX;
            int dy = y - state.PlayerY;
            if (Mathf.Abs(dx) + Mathf.Abs(dy) == 1)
            {
                TryMoveExplore(dx, dy);
                e.Use();
            }
        }

        private Color ExploreTileBaseColor(int x, int y, int tile, bool visible)
        {
            if (!visible)
            {
                return tile == 0 ? Hex("05080a") : Hex("0d1110");
            }

            string kind = ExploreTileKind(x, y, tile);
            Color color;
            switch (kind)
            {
                case "road": color = Hex("5a4027"); break;
                case "paved": color = Hex("50534b"); break;
                case "moss": color = Hex("365637"); break;
                case "mire": color = Hex("295653"); break;
                case "mud": color = Hex("453823"); break;
                case "quarry": color = Hex("565b54"); break;
                case "glass": color = Hex("34586a"); break;
                case "ash": color = Hex("4a2b28"); break;
                case "forestwall": color = Hex("1d3d28"); break;
                case "mirewall": color = Hex("14363b"); break;
                case "cliffwall": color = Hex("343c3f"); break;
                case "redwall": color = Hex("4a1d1d"); break;
                default: color = tile == 0 ? Hex("202829") : ((x + y) % 2 == 0 ? floorA : floorB); break;
            }

            float checker = ((x + y + state.Depth) & 1) == 0 ? 0.08f : -0.03f;
            return checker >= 0f ? Color.Lerp(color, cursorWhite, checker) : Color.Lerp(color, retroBlack, -checker);
        }

        private string ExploreTileKind(int x, int y, int tile)
        {
            string region = ExploreRegionName(x, y);
            bool roadSpine = state?.Map != null && Mathf.Abs(x - state.Map.StartX) <= 1 && y >= state.Map.StartY - 5 && y <= state.Map.StartY + 1;
            if (tile == 0)
            {
                if (region.Contains("Fen") || region.Contains("Cistern")) return "mirewall";
                if (region.Contains("Green") || region.Contains("Gloam")) return "forestwall";
                if (region.Contains("Quarry") || region.Contains("Glass")) return "cliffwall";
                if (region.Contains("Gate")) return "redwall";
                return "stonewall";
            }

            if (roadSpine || region.Contains("Road") || region.Contains("Outworks")) return "road";
            if (region.Contains("Fen") || region.Contains("Cistern")) return ExploreNoise(x, y, 7) % 4 == 0 ? "mire" : "mud";
            if (region.Contains("Quarry")) return "quarry";
            if (region.Contains("Glass")) return "glass";
            if (region.Contains("Green") || region.Contains("Gloam")) return "moss";
            if (region.Contains("Market") || region.Contains("Gate")) return "paved";
            if (state.Depth >= 3 && ExploreNoise(x, y, 11) % 5 == 0) return "ash";
            return "paved";
        }

        private int ExploreNoise(int x, int y, int salt)
        {
            unchecked
            {
                int seed = x * 92821 + y * 68917 + state.Depth * 8191 + salt * 19333;
                seed ^= seed << 13;
                seed ^= seed >> 17;
                seed ^= seed << 5;
                return seed & 0x7fffffff;
            }
        }

        private void DrawExploreFogMotif(Rect rect, int x, int y)
        {
            int n = ExploreNoise(x, y, 31) % 6;
            Color fog = Hex("a9b0a2", 0.06f);
            if (n <= 2) DrawRect(new Rect(rect.x + rect.width * 0.08f, rect.y + rect.height * (0.18f + n * 0.10f), rect.width * 0.72f, rect.height * 0.08f), fog);
            if (n == 3 || n == 5) DrawRect(new Rect(rect.x + rect.width * 0.32f, rect.y + rect.height * 0.10f, rect.width * 0.08f, rect.height * 0.70f), Hex("a9b0a2", 0.045f));
            if (Distance(x, y, state.PlayerX, state.PlayerY) == ExploreRevealRadius + 1)
            {
                DrawBorder(Pad(rect, rect.width * 0.08f), Hex("d9d3c4", 0.08f), 1);
            }
        }

        private void DrawExploreDistanceShade(Rect rect, int distance)
        {
            float edge = Mathf.Clamp01((distance - (ExploreRevealRadius - 3f)) / 3f);
            if (edge > 0f)
            {
                DrawRect(rect, Hex("050708", edge * 0.38f));
                if (distance >= ExploreRevealRadius) DrawBorder(Pad(rect, rect.width * 0.05f), Hex("d9d3c4", 0.10f), 1);
            }
        }

        private void DrawExploreTileEdges(Rect rect, int x, int y, int tile)
        {
            if (tile != 1) return;
            string kind = ExploreTileKind(x, y, tile);
            Color edge = kind == "mire" || kind == "mud" ? Hex("0f2728", 0.52f) : kind == "moss" ? Hex("17251c", 0.50f) : Hex("050708", 0.40f);
            float t = Mathf.Max(1f, rect.width * 0.08f);
            if (TileAt(state.Map, x, y - 1) == 0) DrawRect(new Rect(rect.x, rect.y, rect.width, t), edge);
            if (TileAt(state.Map, x, y + 1) == 0) DrawRect(new Rect(rect.x, rect.yMax - t, rect.width, t), edge);
            if (TileAt(state.Map, x - 1, y) == 0) DrawRect(new Rect(rect.x, rect.y, t, rect.height), edge);
            if (TileAt(state.Map, x + 1, y) == 0) DrawRect(new Rect(rect.xMax - t, rect.y, t, rect.height), edge);
        }

        private void DrawExploreTileMotif(Rect rect, int x, int y, int tile)
        {
            int n = ExploreNoise(x, y, 3) % 9;
            string kind = ExploreTileKind(x, y, tile);
            if (tile == 0)
            {
                DrawExploreBlockMotif(rect, kind, n);
                return;
            }

            if (kind == "road")
            {
                Color track = Hex("211912", 0.32f);
                bool vertical = TileAt(state.Map, x, y - 1) == 1 || TileAt(state.Map, x, y + 1) == 1;
                bool horizontal = TileAt(state.Map, x - 1, y) == 1 || TileAt(state.Map, x + 1, y) == 1;
                if (horizontal) DrawRect(new Rect(rect.x, rect.y + rect.height * 0.42f, rect.width, rect.height * 0.16f), track);
                if (vertical) DrawRect(new Rect(rect.x + rect.width * 0.42f, rect.y, rect.width * 0.16f, rect.height), track);
                DrawRect(new Rect(rect.x + rect.width * (0.16f + (n % 3) * 0.20f), rect.y + rect.height * 0.18f, rect.width * 0.12f, rect.height * 0.08f), Hex("8a7560", 0.20f));
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.72f, rect.width * 0.45f, rect.height * 0.05f), Hex("15110e", 0.24f));
                return;
            }

            if (kind == "mire" || kind == "mud")
            {
                Color puddle = kind == "mire" ? Hex("3b6b67", 0.36f) : Hex("1f2420", 0.26f);
                DrawRect(new Rect(rect.x + rect.width * 0.14f, rect.y + rect.height * 0.30f, rect.width * 0.58f, rect.height * 0.16f), puddle);
                if (n % 2 == 0) DrawRect(new Rect(rect.x + rect.width * 0.68f, rect.y + rect.height * 0.18f, rect.width * 0.06f, rect.height * 0.42f), Hex("9ca86e", 0.34f));
                if (n % 3 == 0) DrawRect(new Rect(rect.x + rect.width * 0.76f, rect.y + rect.height * 0.34f, rect.width * 0.05f, rect.height * 0.30f), Hex("7f9d5b", 0.28f));
                DrawRect(new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * 0.68f, rect.width * 0.38f, rect.height * 0.05f), Hex("050708", 0.22f));
                return;
            }

            if (kind == "moss")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.16f, rect.y + rect.height * 0.18f, rect.width * 0.40f, rect.height * 0.14f), Hex("7f9d5b", 0.24f));
                DrawRect(new Rect(rect.x + rect.width * 0.62f, rect.y + rect.height * 0.54f, rect.width * 0.22f, rect.height * 0.16f), Hex("465f39", 0.34f));
                if (n % 3 == 1) DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.20f, rect.width * 0.05f, rect.height * 0.58f), Hex("a8c06f", 0.20f));
            }
            else if (kind == "quarry")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.12f, rect.y + rect.height * 0.60f, rect.width * 0.52f, rect.height * 0.08f), Hex("15191b", 0.30f));
                DrawRect(new Rect(rect.x + rect.width * 0.64f, rect.y + rect.height * 0.24f, rect.width * 0.16f, rect.height * 0.12f), Hex("a9b0a2", 0.18f));
                if (n % 2 == 0) DrawRect(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.22f, rect.width * 0.12f, rect.height * 0.10f), Hex("6b756e", 0.20f));
            }
            else if (kind == "glass")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.22f, rect.y + rect.height * 0.18f, rect.width * 0.06f, rect.height * 0.42f), Hex("9ad6e8", 0.26f));
                DrawRect(new Rect(rect.x + rect.width * 0.58f, rect.y + rect.height * 0.34f, rect.width * 0.24f, rect.height * 0.05f), Hex("d6f4ff", 0.22f));
                DrawRect(new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.68f, rect.width * 0.34f, rect.height * 0.05f), Hex("15191b", 0.26f));
            }
            else if (kind == "ash")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.62f, rect.width * 0.54f, rect.height * 0.06f), Hex("050708", 0.34f));
                if (n % 3 == 0) DrawRect(new Rect(rect.x + rect.width * 0.58f, rect.y + rect.height * 0.18f, rect.width * 0.06f, rect.height * 0.36f), ember.WithAlpha(0.22f));
            }
            else
            {
                Color seam = Hex("15191b", 0.30f);
                DrawRect(new Rect(rect.x + rect.width * 0.10f, rect.y + rect.height * 0.48f, rect.width * 0.80f, rect.height * 0.05f), seam);
                if (n % 2 == 0) DrawRect(new Rect(rect.x + rect.width * 0.52f, rect.y + rect.height * 0.08f, rect.width * 0.05f, rect.height * 0.78f), seam);
                if (n % 4 == 0) DrawRect(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.20f, rect.width * 0.16f, rect.height * 0.10f), Hex("7f9d5b", 0.16f));
            }

            DrawRect(new Rect(rect.x + rect.width * 0.08f, rect.y + rect.height * 0.08f, rect.width * 0.12f, rect.height * 0.06f), Hex("ffffff", 0.035f));
        }

        private void DrawExploreBlockMotif(Rect rect, string kind, int n)
        {
            if (kind == "forestwall")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.44f, rect.y + rect.height * 0.26f, rect.width * 0.13f, rect.height * 0.54f), Hex("5b3a25", 0.62f));
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.14f, rect.width * 0.64f, rect.height * 0.28f), Hex("465f39", 0.60f));
                DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.04f, rect.width * 0.44f, rect.height * 0.22f), Hex("7f9d5b", 0.28f));
                if (n % 2 == 0) DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.66f, rect.width * 0.36f, rect.height * 0.06f), Hex("8a5c35", 0.42f));
                return;
            }

            if (kind == "mirewall")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.08f, rect.y + rect.height * 0.32f, rect.width * 0.78f, rect.height * 0.18f), Hex("3b6b67", 0.30f));
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.56f, rect.width * 0.54f, rect.height * 0.12f), Hex("101619", 0.34f));
                DrawRect(new Rect(rect.x + rect.width * 0.72f, rect.y + rect.height * 0.18f, rect.width * 0.06f, rect.height * 0.54f), Hex("7f9d5b", 0.34f));
                return;
            }

            Color shade = kind == "redwall" ? Hex("8f3733", 0.26f) : kind == "cliffwall" ? Hex("a9b0a2", 0.20f) : Hex("6b756e", 0.20f);
            DrawRect(new Rect(rect.x + rect.width * 0.12f, rect.y + rect.height * 0.20f, rect.width * 0.72f, rect.height * 0.10f), shade);
            DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.58f, rect.width * 0.55f, rect.height * 0.08f), shade);
            if (n % 2 == 0) DrawRect(new Rect(rect.x + rect.width * 0.64f, rect.y + rect.height * 0.30f, rect.width * 0.06f, rect.height * 0.42f), Hex("050708", 0.26f));
            if (kind == "redwall" && n % 3 == 0) DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.40f, rect.width * 0.34f, rect.height * 0.05f), ember.WithAlpha(0.24f));
        }

        private void DrawCombatTileMotif(Rect rect, int x, int y)
        {
            int depth = state != null ? state.Depth : 1;
            int n = Mathf.Abs(x * 31 + y * 47 + state.Combat.Round * 13 + depth * 19) % 12;
            Color crack = Hex("141719", 0.32f);
            Color warm = Hex("d7a84e", 0.08f);
            Color mossMark = Hex("7f9d5b", 0.10f);
            Color ash = Hex("c65c3b", 0.08f);
            Color rune = Hex("58b7a5", 0.12f);
            if (n <= 1) DrawRect(new Rect(rect.x + rect.width * 0.16f, rect.y + rect.height * 0.66f, rect.width * 0.58f, rect.height * 0.05f), crack);
            if (n == 2 || n == 4) DrawRect(new Rect(rect.x + rect.width * 0.70f, rect.y + rect.height * 0.20f, rect.width * 0.05f, rect.height * 0.46f), crack);
            if (n == 3) DrawRect(new Rect(rect.x + rect.width * 0.14f, rect.y + rect.height * 0.18f, rect.width * 0.16f, rect.height * 0.10f), warm);
            if (n == 5 || n == 8)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.20f, rect.width * 0.12f, rect.height * 0.05f), mossMark);
                DrawRect(new Rect(rect.x + rect.width * 0.26f, rect.y + rect.height * 0.26f, rect.width * 0.18f, rect.height * 0.04f), mossMark);
                DrawRect(new Rect(rect.x + rect.width * 0.12f, rect.y + rect.height * 0.74f, rect.width * 0.22f, rect.height * 0.04f), mossMark);
            }
            if (n == 6 && depth >= 2)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.35f, rect.y + rect.height * 0.35f, rect.width * 0.30f, rect.height * 0.06f), ash);
                DrawRect(new Rect(rect.x + rect.width * 0.43f, rect.y + rect.height * 0.46f, rect.width * 0.20f, rect.height * 0.05f), ash);
                DrawRect(new Rect(rect.x + rect.width * 0.29f, rect.y + rect.height * 0.52f, rect.width * 0.18f, rect.height * 0.04f), ash);
            }
            if (n == 7 && depth >= 3)
            {
                Rect grate = new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.31f, rect.width * 0.32f, rect.height * 0.32f);
                DrawRect(grate, Hex("050708", 0.22f));
                DrawBorder(grate, Hex("a9b0a2", 0.18f), 1);
                DrawRect(new Rect(grate.x + grate.width * 0.45f, grate.y, grate.width * 0.08f, grate.height), Hex("a9b0a2", 0.16f));
                DrawRect(new Rect(grate.x, grate.y + grate.height * 0.45f, grate.width, grate.height * 0.08f), Hex("a9b0a2", 0.16f));
            }
            if (n == 9)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.24f, rect.width * 0.10f, rect.height * 0.04f), Hex("d9d3c4", 0.12f));
                DrawRect(new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.29f, rect.width * 0.04f, rect.height * 0.10f), Hex("d9d3c4", 0.12f));
                DrawRect(new Rect(rect.x + rect.width * 0.63f, rect.y + rect.height * 0.67f, rect.width * 0.14f, rect.height * 0.04f), Hex("d9d3c4", 0.10f));
            }
            if (n == 10 && depth >= 2)
            {
                Rect r = new Rect(rect.x + rect.width * 0.40f, rect.y + rect.height * 0.33f, rect.width * 0.20f, rect.height * 0.20f);
                DrawBorder(r, rune, 1);
                DrawRect(new Rect(r.center.x - rect.width * 0.015f, r.y + r.height * 0.16f, rect.width * 0.03f, r.height * 0.68f), rune);
            }
        }

        private void DrawCombatObstacle(Rect rect, Point obstacle)
        {
            string kind = obstacle != null && !string.IsNullOrEmpty(obstacle.Kind) ? obstacle.Kind : "stone";
            int enemyWorldIndex = CombatObstacleEnemyWorldIndex(kind);
            if (enemyWorldIndex >= 0 && TryDrawEnemyWorldObjectAtlasIcon(Pad(rect, rect.width * 0.03f), enemyWorldIndex, Color.white.WithAlpha(0.92f)))
            {
                if (IsBreakableCover(obstacle)) DrawCoverIntegrityMarks(rect, obstacle);
                return;
            }
            int terrainIcon = TerrainMagicIconIndex(kind);
            if (terrainIcon >= 0 && TryDrawMagicUiAtlasIcon(Pad(rect, rect.width * 0.06f), terrainIcon, Color.white.WithAlpha(0.92f)))
            {
                if (IsBreakableCover(obstacle)) DrawCoverIntegrityMarks(rect, obstacle);
                return;
            }

            if (kind == "fire")
            {
                DrawRect(Pad(rect, rect.width * 0.18f), Hex("7c2f24", 0.40f));
                DrawRect(new Rect(rect.x + rect.width * 0.26f, rect.y + rect.height * 0.42f, rect.width * 0.16f, rect.height * 0.32f), ember);
                DrawRect(new Rect(rect.x + rect.width * 0.43f, rect.y + rect.height * 0.28f, rect.width * 0.14f, rect.height * 0.46f), gold);
                DrawRect(new Rect(rect.x + rect.width * 0.58f, rect.y + rect.height * 0.48f, rect.width * 0.14f, rect.height * 0.26f), Hex("d98b6a"));
                return;
            }
            if (kind == "ice")
            {
                Rect sheet = Pad(rect, rect.width * 0.14f);
                DrawRect(sheet, Hex("6baec7", 0.28f));
                DrawRect(new Rect(sheet.x + sheet.width * 0.08f, sheet.y + sheet.height * 0.22f, sheet.width * 0.70f, sheet.height * 0.08f), frost);
                DrawRect(new Rect(sheet.x + sheet.width * 0.25f, sheet.y + sheet.height * 0.55f, sheet.width * 0.62f, sheet.height * 0.07f), Hex("d6f4ff", 0.70f));
                DrawBorder(sheet, frost, 1);
                return;
            }
            if (kind == "web")
            {
                Rect web = Pad(rect, rect.width * 0.12f);
                Color thread = Hex("d9d3c4", 0.58f);
                DrawRect(new Rect(web.x, web.y + web.height * 0.46f, web.width, web.height * 0.06f), thread);
                DrawRect(new Rect(web.x + web.width * 0.47f, web.y, web.width * 0.06f, web.height), thread);
                DrawRect(new Rect(web.x + web.width * 0.18f, web.y + web.height * 0.18f, web.width * 0.64f, web.height * 0.06f), thread);
                DrawRect(new Rect(web.x + web.width * 0.18f, web.y + web.height * 0.76f, web.width * 0.64f, web.height * 0.06f), thread);
                DrawBorder(web, Hex("d9d3c4", 0.36f), 1);
                return;
            }
            if (kind == "gas")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.12f, rect.y + rect.height * 0.30f, rect.width * 0.76f, rect.height * 0.14f), Hex("7f9d5b", 0.38f));
                DrawRect(new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * 0.46f, rect.width * 0.56f, rect.height * 0.13f), Hex("b2cf72", 0.28f));
                DrawRect(new Rect(rect.x + rect.width * 0.16f, rect.y + rect.height * 0.61f, rect.width * 0.48f, rect.height * 0.10f), Hex("465f39", 0.34f));
                return;
            }
            if (kind == "tree")
            {
                Color trunk = Hex("5b3a25");
                Color trunkLight = Hex("8a5c35");
                Color leaf = moss;
                Color leafLight = Hex("a8c06f");
                Color leafDark = Hex("465f39");
                Rect canopy = Pad(rect, rect.width * 0.12f);
                DrawRect(new Rect(rect.x + rect.width * 0.44f, rect.y + rect.height * 0.43f, rect.width * 0.13f, rect.height * 0.36f), trunk);
                DrawRect(new Rect(rect.x + rect.width * 0.48f, rect.y + rect.height * 0.47f, rect.width * 0.05f, rect.height * 0.26f), trunkLight);
                DrawRect(new Rect(canopy.x + canopy.width * 0.18f, canopy.y + canopy.height * 0.20f, canopy.width * 0.64f, canopy.height * 0.28f), leaf);
                DrawRect(new Rect(canopy.x + canopy.width * 0.08f, canopy.y + canopy.height * 0.34f, canopy.width * 0.84f, canopy.height * 0.26f), leafDark);
                DrawRect(new Rect(canopy.x + canopy.width * 0.26f, canopy.y + canopy.height * 0.08f, canopy.width * 0.48f, canopy.height * 0.24f), leafLight);
                DrawRect(new Rect(canopy.x + canopy.width * 0.34f, canopy.y + canopy.height * 0.48f, canopy.width * 0.34f, canopy.height * 0.20f), leaf);
                DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.76f, rect.width * 0.44f, rect.height * 0.05f), trunkLight);
                DrawBorder(Pad(rect, rect.width * 0.16f), Hex("15191b", 0.5f), 1);
                DrawCoverIntegrityMarks(rect, obstacle);
                return;
            }

            Rect rock = Pad(rect, rect.width * 0.22f);
            DrawRect(new Rect(rock.x, rock.y + rock.height * 0.18f, rock.width, rock.height * 0.66f), stone);
            DrawRect(new Rect(rock.x + rock.width * 0.18f, rock.y, rock.width * 0.62f, rock.height * 0.32f), Color.Lerp(stone, ink, 0.12f));
            DrawRect(new Rect(rock.x + rock.width * 0.18f, rock.y + rock.height * 0.60f, rock.width * 0.16f, rock.height * 0.10f), Hex("20272e", 0.8f));
            DrawRect(new Rect(rock.x + rock.width * 0.58f, rock.y + rock.height * 0.36f, rock.width * 0.22f, rock.height * 0.08f), Hex("20272e", 0.8f));
            DrawCoverIntegrityMarks(rect, obstacle);
        }

        private int CombatObstacleEnemyWorldIndex(string kind)
        {
            switch ((kind ?? "").ToLowerInvariant())
            {
                case "tree": return 17;
                case "stone": return 18;
                default: return -1;
            }
        }

        private void DrawCoverIntegrityMarks(Rect rect, Point obstacle)
        {
            if (!IsBreakableCover(obstacle)) return;
            int max = CoverMaxIntegrity(obstacle.Kind);
            int current = CoverIntegrity(obstacle);
            DrawCoverIntegrityPips(rect, obstacle, current, max);
            if (max <= 0 || current >= max) return;
            Color mark = obstacle.Kind == "tree" ? Hex("f3ead7", 0.58f) : Hex("d7a84e", 0.54f);
            DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.20f, rect.width * 0.44f, rect.height * 0.055f), mark);
            DrawRect(new Rect(rect.x + rect.width * 0.54f, rect.y + rect.height * 0.26f, rect.width * 0.055f, rect.height * 0.26f), mark);
            if (current <= 1)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.32f, rect.y + rect.height * 0.58f, rect.width * 0.42f, rect.height * 0.055f), mark);
            }
        }

        private void DrawCoverIntegrityPips(Rect rect, Point obstacle, int current, int max)
        {
            if (max <= 0) return;
            Color filled = obstacle.Kind == "tree" ? Hex("a8c06f", 0.92f) : Hex("d9d3c4", 0.86f);
            Color empty = Hex("050708", 0.72f);
            float pip = Mathf.Max(4f, rect.width * 0.075f);
            float gap = Mathf.Max(2f, rect.width * 0.025f);
            float total = max * pip + (max - 1) * gap;
            float start = rect.center.x - total * 0.5f;
            float y = rect.yMax - pip - rect.height * 0.08f;
            for (int i = 0; i < max; i++)
            {
                Rect p = new Rect(start + i * (pip + gap), y, pip, pip);
                DrawRect(p, i < current ? filled : empty);
                DrawBorder(p, Hex("030405", 0.74f), 1);
            }
            if (obstacle.Duration > 0)
            {
                Rect timer = new Rect(rect.xMax - rect.width * 0.26f, rect.y + rect.height * 0.08f, rect.width * 0.20f, Mathf.Max(12f, rect.height * 0.16f));
                DrawRect(timer, Hex("050708", 0.82f));
                DrawBorder(timer, filled, 1);
                GUI.Label(new Rect(timer.x, timer.y - 1f, timer.width, timer.height + 2f), obstacle.Duration.ToString(), CenterStyle(8, cursorWhite));
            }
        }

        private void DrawExploreObject(Rect rect, ObjectType type)
        {
            Color objectColor = ObjectColor(type);
            float pulse = state != null && state.ReducedMotion ? 0.35f : 0.35f + Mathf.Sin(Time.time * 3.5f + (int)type) * 0.12f;
            DrawRect(Pad(rect, rect.width * 0.24f), objectColor.WithAlpha(0.16f + pulse * 0.18f));
            DrawBorder(Pad(rect, rect.width * 0.10f), objectColor.WithAlpha(0.62f + pulse * 0.24f), 2);
            if (TryDrawWorldObjectIcon(Pad(rect, rect.width * 0.08f), type))
            {
                DrawBorder(Pad(rect, rect.width * 0.07f), objectColor.WithAlpha(0.78f + pulse * 0.16f), 1);
                return;
            }

            Rect inner = Pad(rect, rect.width * 0.16f);
            switch (type)
            {
                case ObjectType.Cache:
                    DrawRect(new Rect(inner.x + inner.width * 0.08f, inner.y + inner.height * 0.72f, inner.width * 0.84f, inner.height * 0.08f), Hex("050708", 0.45f));
                    DrawRect(new Rect(inner.x + inner.width * 0.03f, inner.y + inner.height * 0.28f, inner.width * 0.94f, inner.height * 0.48f), Hex("5b331f"));
                    DrawRect(new Rect(inner.x + inner.width * 0.10f, inner.y + inner.height * 0.12f, inner.width * 0.80f, inner.height * 0.28f), Hex("9a6a3e"));
                    DrawRect(new Rect(inner.x + inner.width * 0.15f, inner.y + inner.height * 0.42f, inner.width * 0.70f, inner.height * 0.08f), gold);
                    DrawRect(new Rect(inner.x + inner.width * 0.26f, inner.y + inner.height * 0.22f, inner.width * 0.08f, inner.height * 0.52f), Hex("24150e", 0.70f));
                    DrawRect(new Rect(inner.x + inner.width * 0.66f, inner.y + inner.height * 0.22f, inner.width * 0.08f, inner.height * 0.52f), Hex("24150e", 0.70f));
                    DrawRect(new Rect(inner.x + inner.width * 0.44f, inner.y + inner.height * 0.45f, inner.width * 0.12f, inner.height * 0.18f), Hex("050708"));
                    DrawRect(new Rect(inner.x + inner.width * 0.62f, inner.y + inner.height * 0.17f, inner.width * 0.14f, inner.height * 0.07f), Hex("f3ead7", 0.50f));
                    DrawBorder(new Rect(inner.x + inner.width * 0.03f, inner.y + inner.height * 0.18f, inner.width * 0.94f, inner.height * 0.58f), Hex("d9d3c4", 0.48f), 1);
                    break;
                case ObjectType.Shrine:
                    DrawRect(new Rect(inner.x + inner.width * 0.08f, inner.y + inner.height * 0.72f, inner.width * 0.84f, inner.height * 0.12f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.56f, inner.width * 0.64f, inner.height * 0.18f), Hex("303936"));
                    DrawRect(new Rect(inner.x + inner.width * 0.28f, inner.y + inner.height * 0.16f, inner.width * 0.44f, inner.height * 0.48f), Hex("214b47"));
                    DrawRect(new Rect(inner.x + inner.width * 0.34f, inner.y + inner.height * 0.20f, inner.width * 0.32f, inner.height * 0.34f), teal.WithAlpha(0.80f));
                    DrawRect(new Rect(rect.center.x - rect.width * 0.028f, inner.y + inner.height * 0.08f, rect.width * 0.056f, inner.height * 0.60f), ink);
                    DrawRect(new Rect(inner.x + inner.width * 0.28f, rect.center.y - rect.height * 0.030f, inner.width * 0.44f, rect.height * 0.06f), ink);
                    DrawRect(new Rect(inner.x + inner.width * 0.08f, inner.y + inner.height * 0.82f, inner.width * 0.14f, inner.height * 0.08f), gold);
                    DrawRect(new Rect(inner.x + inner.width * 0.78f, inner.y + inner.height * 0.82f, inner.width * 0.14f, inner.height * 0.08f), gold);
                    break;
                case ObjectType.Encounter:
                    DrawRect(new Rect(inner.x + inner.width * 0.20f, inner.y + inner.height * 0.68f, inner.width * 0.60f, inner.height * 0.08f), Hex("050708", 0.45f));
                    DrawRect(new Rect(inner.x + inner.width * 0.12f, inner.y + inner.height * 0.22f, inner.width * 0.76f, inner.height * 0.42f), blood);
                    DrawRect(new Rect(inner.x + inner.width * 0.20f, inner.y + inner.height * 0.08f, inner.width * 0.18f, inner.height * 0.24f), Hex("d9d3c4"));
                    DrawRect(new Rect(inner.x + inner.width * 0.62f, inner.y + inner.height * 0.08f, inner.width * 0.18f, inner.height * 0.24f), Hex("d9d3c4"));
                    DrawRect(new Rect(inner.x + inner.width * 0.32f, inner.y + inner.height * 0.34f, inner.width * 0.09f, inner.height * 0.09f), Hex("050708"));
                    DrawRect(new Rect(inner.x + inner.width * 0.59f, inner.y + inner.height * 0.34f, inner.width * 0.09f, inner.height * 0.09f), Hex("050708"));
                    DrawRect(new Rect(inner.x + inner.width * 0.42f, inner.y + inner.height * 0.52f, inner.width * 0.16f, inner.height * 0.08f), Hex("050708"));
                    DrawRect(new Rect(inner.x + inner.width * 0.22f, inner.y + inner.height * 0.64f, inner.width * 0.13f, inner.height * 0.18f), Hex("d9d3c4", 0.65f));
                    DrawRect(new Rect(inner.x + inner.width * 0.65f, inner.y + inner.height * 0.64f, inner.width * 0.13f, inner.height * 0.18f), Hex("d9d3c4", 0.65f));
                    break;
                case ObjectType.Stairs:
                    DrawRect(new Rect(inner.x + inner.width * 0.08f, inner.y + inner.height * 0.08f, inner.width * 0.84f, inner.height * 0.78f), Hex("030405", 0.84f));
                    DrawBorder(new Rect(inner.x + inner.width * 0.08f, inner.y + inner.height * 0.08f, inner.width * 0.84f, inner.height * 0.78f), stone, 1);
                    for (int i = 0; i < 5; i++)
                    {
                        DrawRect(new Rect(inner.x + inner.width * (0.14f + i * 0.035f), inner.y + inner.height * (0.18f + i * 0.12f), inner.width * (0.72f - i * 0.04f), rect.height * 0.065f), Color.Lerp(stone, ink, i * 0.09f));
                    }
                    DrawRect(new Rect(inner.x + inner.width * 0.12f, inner.y + inner.height * 0.14f, inner.width * 0.10f, inner.height * 0.46f), gold.WithAlpha(0.55f));
                    DrawRect(new Rect(inner.x + inner.width * 0.78f, inner.y + inner.height * 0.14f, inner.width * 0.10f, inner.height * 0.46f), frost.WithAlpha(0.38f));
                    break;
                case ObjectType.Camp:
                    DrawRect(new Rect(inner.x + inner.width * 0.06f, inner.y + inner.height * 0.70f, inner.width * 0.88f, inner.height * 0.08f), Hex("050708", 0.42f));
                    DrawRect(new Rect(inner.x + inner.width * 0.10f, inner.y + inner.height * 0.42f, inner.width * 0.30f, inner.height * 0.28f), Hex("53646a"));
                    DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.24f, inner.width * 0.24f, inner.height * 0.22f), Hex("6f8376"));
                    DrawRect(new Rect(inner.x + inner.width * 0.52f, inner.y + inner.height * 0.64f, inner.width * 0.38f, inner.height * 0.08f), Hex("5b3a25"));
                    DrawRect(new Rect(inner.x + inner.width * 0.56f, inner.y + inner.height * 0.58f, inner.width * 0.30f, inner.height * 0.07f), Hex("8a5c35"));
                    DrawRect(new Rect(inner.x + inner.width * 0.60f, inner.y + inner.height * 0.38f, inner.width * 0.10f, inner.height * 0.26f), ember);
                    DrawRect(new Rect(inner.x + inner.width * 0.70f, inner.y + inner.height * 0.24f, inner.width * 0.10f, inner.height * 0.40f), gold);
                    DrawRect(new Rect(inner.x + inner.width * 0.80f, inner.y + inner.height * 0.45f, inner.width * 0.09f, inner.height * 0.18f), Hex("d98b6a"));
                    break;
                case ObjectType.Obelisk:
                    DrawRect(new Rect(inner.x + inner.width * 0.24f, inner.y + inner.height * 0.78f, inner.width * 0.52f, inner.height * 0.10f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.34f, inner.y + inner.height * 0.24f, inner.width * 0.32f, inner.height * 0.56f), Hex("4d5856"));
                    DrawRect(new Rect(inner.x + inner.width * 0.39f, inner.y + inner.height * 0.12f, inner.width * 0.22f, inner.height * 0.16f), Hex("6f7c76"));
                    DrawRect(new Rect(inner.x + inner.width * 0.47f, inner.y + inner.height * 0.34f, inner.width * 0.06f, inner.height * 0.26f), teal.WithAlpha(0.75f));
                    DrawRect(new Rect(inner.x + inner.width * 0.40f, inner.y + inner.height * 0.48f, inner.width * 0.20f, inner.height * 0.05f), teal.WithAlpha(0.62f));
                    break;
                case ObjectType.Ruin:
                    DrawRect(new Rect(inner.x + inner.width * 0.10f, inner.y + inner.height * 0.72f, inner.width * 0.80f, inner.height * 0.10f), Hex("050708", 0.40f));
                    DrawRect(new Rect(inner.x + inner.width * 0.16f, inner.y + inner.height * 0.34f, inner.width * 0.18f, inner.height * 0.42f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.42f, inner.y + inner.height * 0.20f, inner.width * 0.16f, inner.height * 0.56f), Hex("68706b"));
                    DrawRect(new Rect(inner.x + inner.width * 0.66f, inner.y + inner.height * 0.48f, inner.width * 0.16f, inner.height * 0.28f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.24f, inner.y + inner.height * 0.26f, inner.width * 0.32f, inner.height * 0.06f), Hex("a9b0a2", 0.30f));
                    DrawRect(new Rect(inner.x + inner.width * 0.58f, inner.y + inner.height * 0.70f, inner.width * 0.28f, inner.height * 0.06f), moss.WithAlpha(0.45f));
                    break;
                case ObjectType.Bridge:
                    DrawRect(new Rect(inner.x + inner.width * 0.04f, inner.y + inner.height * 0.38f, inner.width * 0.92f, inner.height * 0.30f), Hex("3b6b67", 0.35f));
                    DrawRect(new Rect(inner.x + inner.width * 0.08f, inner.y + inner.height * 0.44f, inner.width * 0.84f, inner.height * 0.18f), Hex("5b3a25"));
                    DrawRect(new Rect(inner.x + inner.width * 0.12f, inner.y + inner.height * 0.38f, inner.width * 0.76f, inner.height * 0.06f), Hex("8a5c35"));
                    for (int i = 0; i < 4; i++) DrawRect(new Rect(inner.x + inner.width * (0.18f + i * 0.16f), inner.y + inner.height * 0.39f, inner.width * 0.05f, inner.height * 0.27f), Hex("24150e", 0.54f));
                    DrawRect(new Rect(inner.x + inner.width * 0.06f, inner.y + inner.height * 0.28f, inner.width * 0.05f, inner.height * 0.48f), Hex("9b6b45"));
                    DrawRect(new Rect(inner.x + inner.width * 0.89f, inner.y + inner.height * 0.28f, inner.width * 0.05f, inner.height * 0.48f), Hex("9b6b45"));
                    break;
                case ObjectType.Cave:
                    DrawRect(new Rect(inner.x + inner.width * 0.10f, inner.y + inner.height * 0.70f, inner.width * 0.80f, inner.height * 0.10f), Hex("050708", 0.48f));
                    DrawRect(new Rect(inner.x + inner.width * 0.12f, inner.y + inner.height * 0.34f, inner.width * 0.76f, inner.height * 0.38f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.24f, inner.y + inner.height * 0.20f, inner.width * 0.52f, inner.height * 0.30f), Hex("68706b"));
                    DrawRect(new Rect(inner.x + inner.width * 0.30f, inner.y + inner.height * 0.38f, inner.width * 0.40f, inner.height * 0.36f), Hex("030405"));
                    DrawRect(new Rect(inner.x + inner.width * 0.37f, inner.y + inner.height * 0.48f, inner.width * 0.26f, inner.height * 0.22f), Hex("101619"));
                    DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.70f, inner.width * 0.18f, inner.height * 0.06f), moss.WithAlpha(0.46f));
                    break;
                case ObjectType.Town:
                    DrawRect(new Rect(inner.x + inner.width * 0.03f, inner.y + inner.height * 0.52f, inner.width * 0.94f, inner.height * 0.30f), moss);
                    DrawRect(new Rect(inner.x - rect.width * 0.04f, inner.y + inner.height * 0.32f, inner.width + rect.width * 0.08f, inner.height * 0.24f), gold);
                    DrawRect(new Rect(inner.x + inner.width * 0.12f, inner.y + inner.height * 0.22f, inner.width * 0.17f, inner.height * 0.36f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.41f, inner.y + inner.height * 0.18f, inner.width * 0.18f, inner.height * 0.40f), Hex("5f6b64"));
                    DrawRect(new Rect(inner.x + inner.width * 0.70f, inner.y + inner.height * 0.22f, inner.width * 0.17f, inner.height * 0.36f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.44f, inner.y + inner.height * 0.60f, inner.width * 0.12f, inner.height * 0.22f), Hex("050708"));
                    DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.65f, inner.width * 0.08f, inner.height * 0.08f), teal.WithAlpha(0.65f));
                    DrawRect(new Rect(inner.x + inner.width * 0.74f, inner.y + inner.height * 0.65f, inner.width * 0.08f, inner.height * 0.08f), teal.WithAlpha(0.65f));
                    break;
            }
        }

        private Color ObjectColor(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Cache: return gold;
                case ObjectType.Shrine: return teal;
                case ObjectType.Encounter: return blood;
                case ObjectType.Stairs: return frost;
                case ObjectType.Camp: return ember;
                case ObjectType.Town: return moss;
                case ObjectType.Obelisk: return teal;
                case ObjectType.Ruin: return stone;
                case ObjectType.Bridge: return Hex("9b6b45");
                case ObjectType.Cave: return Hex("a9b0a2");
                default: return muted;
            }
        }

        private WorldZone ZoneAt(int x, int y)
        {
            return ZoneFor(x, y, state?.Map, state?.Depth ?? 1);
        }

        private WorldZone ZoneFor(int x, int y, MapData map, int depth)
        {
            int sx = map?.StartX ?? ExploreW / 2;
            int sy = map?.StartY ?? ExploreH / 2;
            if (Distance(x, y, sx, sy) <= 4)
            {
                return new WorldZone
                {
                    Id = "midgaard-road",
                    Name = "Midgaard Road",
                    Title = "safe road",
                    Danger = 0,
                    Summary = $"{HomeTownName}'s lamp road, patrol stones, and last warm windows.",
                    Story = "This is the company's anchor: heal, regroup, then push outward."
                };
            }

            bool north = y < ExploreH * 0.35f;
            bool south = y > ExploreH * 0.65f;
            bool west = x < ExploreW * 0.34f;
            bool east = x > ExploreW * 0.66f;
            if (north && west) return MakeZone("old-quarry", "Old Quarry", "stone yards", 2, "broken quarries, hidden caches, and hard-footed raiders", "The quarry road should eventually feed stoneborn lore, bridge repairs, and heavy gear caches.");
            if (north && east) return MakeZone("glass-warrens", "Glass Warrens", "crystal maze", 3, "glass rubble, cold light, and caster spoor", "Glass shines where the Old Road cracked; later chapters can put mage factions and mirror puzzles here.");
            if (south && west) return MakeZone("ash-fen", "Ash Fen", "poison fen", 2, "mud banks, green fireflies, and sick water", "The fen is the first natural hazard zone: poison, mire movement, and shrine recovery pressure.");
            if (south && east) return MakeZone("red-gate", "Red Gate", "war gate", 4, "red basalt, old banners, and death-cult signs", "The Red Gate is the long arc's pressure point: bone wizards, pact risks, and gate keys.");
            if (north) return MakeZone("gloam-courts", "Gloam Courts", "upper ruins", 2, "fallen halls, court stones, and watchful dark", "These courts bridge rats and kobolds into organized ruin factions.");
            if (south) return MakeZone("salt-cisterns", "Salt Cisterns", "sewers", 1, "damp passages, rat nests, and old sluices", "This is the early story's first proving ground: rats, supplies, and sewer stairs.");
            if (west) return MakeZone("green-shrine-road", "Green Shrine Road", "pilgrim road", 1, "mossy paths, teal lamps, and old priest stones", "A recovery-oriented road for priest lore, shrines, and Tree Cover tutoring.");
            if (east) return MakeZone("dusk-market", "Dusk Market Ruins", "market ruins", 2, "collapsed stalls, thieves' marks, and kobold scouts", "The market is the rogue/ranger pressure zone: ambushes, caches, and cave mouths.");
            return MakeZone("inner-ash-road", depth <= 1 ? "Midgaard Outworks" : "Inner Ash Road", "central road", Mathf.Clamp(depth, 1, 4), "old paving, road shrines, and patrol signs", "The spine of the world map, used to connect chapter objectives.");
        }

        private WorldZone MakeZone(string id, string name, string title, int danger, string summary, string story)
        {
            return new WorldZone { Id = id, Name = name, Title = title, Danger = danger, Summary = summary, Story = story };
        }

        private string ZoneDangerText(WorldZone zone)
        {
            if (zone == null) return "unknown";
            if (zone.Danger <= 0) return "safe";
            if (zone.Danger == 1) return "low danger";
            if (zone.Danger == 2) return "danger";
            if (zone.Danger == 3) return "high danger";
            return "deadly";
        }

        private string StoryObjectiveForDepth(int depth)
        {
            if (depth <= 1) return "Chapter I: The Midgaard Cisterns. Find supplies, test the company against sewer rats, and locate the stair below the Old Road.";
            if (depth == 2) return "Chapter II: Kobold Smoke. Follow cave signs beyond the market ruins and learn why kobold shamans are gathering bone charms.";
            if (depth == 3) return "Chapter III: The Bone Road. Break caster pressure in the Gloam Courts and recover the first Red Gate warning.";
            if (depth == 4) return "Chapter IV: Glass and Ash. Cross the Glass Warrens, survive warlock bargains, and search for a gate key.";
            if (depth == 5) return "Chapter V: The Red Gate. Push through drow priests, bone wizards, and lesser demons to find the sealed descent.";
            if (depth >= FinalBossDepth) return "Chapter VI: Meteor Crown. Break the ritual heart and defeat Vhal Rakh before the Old Road burns open.";
            return "Chapter V: Below the Old Road. The scaffold now points toward bosses, factions, and deeper world-state choices.";
        }

        private string StoryChapterTitle()
        {
            string objective = string.IsNullOrEmpty(state?.ActiveStory) ? StoryObjectiveForDepth(state?.Depth ?? 1) : state.ActiveStory;
            int dot = objective.IndexOf('.');
            return dot > 0 ? objective.Substring(0, dot) : objective;
        }

        private string ExploreRegionName(int x, int y)
        {
            return ZoneAt(x, y).Name;
        }

        private string ExploreUnderfootLine(int x, int y)
        {
            MapObject obj = ObjectAt(state.Map, x, y);
            if (obj != null) return ObjectName(obj.Type) + ": " + ObjectHint(obj.Type);
            return TileAt(state.Map, x, y) == 1 ? $"{ExploreGroundName(x, y)} / patrol risk" : $"{ExploreGroundName(x, y)} / blocks movement";
        }

        private string ExploreLookLine(int x, int y)
        {
            string region = ExploreRegionName(x, y);
            WorldZone zone = ZoneAt(x, y);
            MapObject obj = ObjectAt(state.Map, x, y);
            int distance = Distance(x, y, state.PlayerX, state.PlayerY);
            if (obj != null)
            {
                string step = distance == 0 ? "underfoot" : distance == 1 ? "click to step onto it" : $"range {distance}";
                return $"{ObjectName(obj.Type)} / {region} / {ZoneDangerText(zone)}\n{ObjectHint(obj.Type)} / {step}";
            }

            if (TileAt(state.Map, x, y) == 0)
            {
                return $"{ExploreGroundName(x, y)} / {region}\nblocks movement / {zone.Title}";
            }

            string move = distance == 0 ? "current position" : distance == 1 ? "click to move" : $"range {distance}";
            return $"{ExploreGroundName(x, y)} / {region} / {ZoneDangerText(zone)}\n{move} / {zone.Summary}";
        }

        private string ExploreLegendLine()
        {
            return "gold cache / teal shrine / red danger / gray landmarks";
        }

        private Color ZoneDangerColor(WorldZone zone)
        {
            if (zone == null || zone.Danger <= 0) return teal;
            if (zone.Danger == 1) return moss;
            if (zone.Danger == 2) return gold;
            if (zone.Danger == 3) return ember;
            return blood;
        }

        private string ExploreGroundName(int x, int y)
        {
            int tile = TileAt(state.Map, x, y);
            string kind = ExploreTileKind(x, y, tile);
            switch (kind)
            {
                case "road": return "Old road";
                case "paved": return "Broken paving";
                case "moss": return "Mossy path";
                case "mire": return "Shallow mire";
                case "mud": return "Fen bank";
                case "quarry": return "Quarry stone";
                case "glass": return "Glass rubble";
                case "ash": return "Ash floor";
                case "forestwall": return "Tree wall";
                case "mirewall": return "Dark water";
                case "cliffwall": return "Cliff stone";
                case "redwall": return "Red basalt";
                default: return tile == 1 ? "Ruin floor" : "Stone wall";
            }
        }

        private string ObjectName(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Cache: return "Sealed cache";
                case ObjectType.Shrine: return "Old shrine";
                case ObjectType.Encounter: return "Enemy sign";
                case ObjectType.Stairs: return "Down stairs";
                case ObjectType.Camp: return "Camp mark";
                case ObjectType.Town: return HomeTownName;
                case ObjectType.Obelisk: return "Runed obelisk";
                case ObjectType.Ruin: return "Fallen ruin";
                case ObjectType.Bridge: return "Old bridge";
                case ObjectType.Cave: return "Cave mouth";
                default: return "Road mark";
            }
        }

        private string ObjectHint(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Cache: return "sealed loot and gear";
                case ObjectType.Shrine: return "teal recovery light";
                case ObjectType.Encounter: return "enemy tracks and bones";
                case ObjectType.Stairs: return "descend when standing here";
                case ObjectType.Camp: return "tent, coals, and rest";
                case ObjectType.Town: return $"{HomeTownName}'s healing lamps";
                case ObjectType.Obelisk: return "landmark and old ward";
                case ObjectType.Ruin: return "broken cover and memory";
                case ObjectType.Bridge: return "crossing marker";
                case ObjectType.Cave: return "cold side passage";
                default: return "something waits";
            }
        }

        private void DrawCombat()
        {
            boardRect = GetBoardRect();
            DrawPanel(boardRect);
            Rect grid = CombatBoardInnerRect(boardRect);
            float cell = Mathf.Min(grid.width / CombatW, grid.height / CombatH);
            grid.width = cell * CombatW;
            grid.height = cell * CombatH;
            DrawRect(new Rect(grid.x - 14, grid.y - 14, grid.width + 28, grid.height + 28), retroBlack);
            DrawRect(new Rect(grid.x - 14, grid.y - 14, grid.width * 0.24f, grid.height + 28), Hex("17352e", 0.30f));
            DrawRect(new Rect(grid.x + grid.width * 0.76f, grid.y - 14, grid.width * 0.24f + 28, grid.height + 28), Hex("43221f", 0.30f));

            CombatUnit active = CurrentUnit();
            for (int y = 0; y < CombatH; y++)
            for (int x = 0; x < CombatW; x++)
            {
                Rect c = new Rect(grid.x + x * cell, grid.y + y * cell, cell, cell);
                DrawRect(c, (x + y) % 2 == 0 ? Hex("101612") : Hex("0b110f"));
                Point obstacle = ObstacleAt(x, y);
                DrawCombatTerrainTexture(c, x, y, obstacle);
                DrawCombatSpeckles(c, x, y);
                DrawBorder(c, Hex("253029", 0.28f), 1);
                DrawCombatTileMotif(c, x, y);
                if (obstacle != null)
                {
                    DrawCombatObstacle(c, obstacle);
                }
                DrawCellFlash(c, x, y);
            }

            if (active != null && active.Side == UnitSide.Party)
            {
                DrawCombatHighlights(grid, cell, active);
                DrawHoverAim(grid, cell, active);
            }

            foreach (CombatUnit unit in state.Combat.Units.Where(u => u.Hp > 0))
            {
                Vector2 pos = UnitDrawPos(unit);
                Rect cellRect = new Rect(grid.x + pos.x * cell, grid.y + pos.y * cell, cell, cell);
                bool isActive = active != null && active.Id == unit.Id;
                if (isActive) DrawActiveCursor(cellRect, cell);
                DrawCombatStatusFrame(cellRect, unit, isActive, cell);
                DrawUnitBase(cellRect, unit, cell);
                DrawCombatUnitSprite(Pad(cellRect, cell * 0.01f), unit, isActive);
                Rect hp = new Rect(cellRect.x + cell * 0.15f, cellRect.yMax - cell * 0.14f, cell * 0.7f, cell * 0.07f);
                DrawRect(hp, Hex("111619"));
                DrawRect(new Rect(hp.x, hp.y, hp.width * Mathf.Clamp01((float)unit.Hp / unit.MaxHp), hp.height), unit.Side == UnitSide.Party ? ember : blood);
                DrawStatusPips(cellRect, unit, cell);
            }

            DrawCastGlyphs(grid, cell);
            DrawBeams(grid, cell);
            DrawParticles(grid, cell);
            DrawTurnQueue(grid, active);
            DrawHoverPreview(grid, cell, active);
            DrawCombatReadout(grid, active);
            HandleCombatMouse(grid, cell);
        }

        private void DrawCombatSpeckles(Rect rect, int x, int y)
        {
            float size = Mathf.Max(1f, rect.width * 0.025f);
            for (int i = 0; i < 8; i++)
            {
                int seed = Mathf.Abs(x * 92821 + y * 68917 + i * 19333 + state.Depth * 8191);
                float px = 0.10f + (seed % 79) / 98f;
                float py = 0.10f + ((seed / 97) % 79) / 98f;
                Color dot = i % 3 == 0 ? Hex("6f8d4d", 0.52f) : i % 3 == 1 ? Hex("37543a", 0.48f) : Hex("99a66a", 0.36f);
                DrawRect(new Rect(rect.x + rect.width * px, rect.y + rect.height * py, size, size), dot);
            }
        }

        private void DrawCombatTerrainTexture(Rect rect, int x, int y, Point obstacle)
        {
            int index = CombatTerrainTextureIndex(x, y, obstacle);
            if (index < 0) return;
            float alpha = obstacle == null ? 0.58f : 0.66f;
            if (TryDrawCombatTerrainAtlasIcon(Pad(rect, 1f), index, Color.white.WithAlpha(alpha)))
            {
                DrawRect(rect, Hex("030405", obstacle == null ? 0.18f : 0.10f));
            }
        }

        private int CombatTerrainTextureIndex(int x, int y, Point obstacle)
        {
            string kind = obstacle?.Kind ?? "";
            if (kind == "fire") return 14;
            if (kind == "ice") return 12;
            if (kind == "web") return 13;
            if (kind == "gas") return 0;
            if (kind == "stone") return 5;
            if (kind == "tree") return 0;

            int seed = Mathf.Abs(state.Seed + state.Depth * 193 + x * 37 + y * 71);
            int variant = seed % 5;
            if (state.Depth <= 1)
            {
                int[] road = { 0, 2, 3, 5, 15 };
                return road[variant];
            }
            if (state.Depth == 2)
            {
                int[] cistern = { 10, 5, 3, 11, 9 };
                return cistern[variant];
            }
            if (state.Depth == 3)
            {
                int[] cave = { 9, 11, 5, 3, 4 };
                return cave[variant];
            }
            if (state.Depth == 4)
            {
                int[] glass = { 1, 12, 11, 5, 4 };
                return glass[variant];
            }
            if (state.Depth == 5)
            {
                int[] red = { 6, 7, 14, 4, 15 };
                return red[variant];
            }
            int[] final = { 7, 14, 4, 15, 6 };
            return final[variant];
        }

        private void DrawActiveCursor(Rect cellRect, float cell)
        {
            Rect outer = Pad(cellRect, cell * 0.08f);
            float pulse = state.ReducedMotion ? 0.2f : 0.5f + Mathf.Sin(Time.time * 6f) * 0.5f;
            DrawBorder(outer, Color.Lerp(cursorWhite, gold, pulse * 0.22f), 2);
            float tick = cell * 0.20f;
            DrawRect(new Rect(outer.x, outer.y, tick, 3), cursorWhite);
            DrawRect(new Rect(outer.x, outer.y, 3, tick), cursorWhite);
            DrawRect(new Rect(outer.xMax - tick, outer.y, tick, 3), cursorWhite);
            DrawRect(new Rect(outer.xMax - 3, outer.y, 3, tick), cursorWhite);
            DrawRect(new Rect(outer.x, outer.yMax - 3, tick, 3), cursorWhite);
            DrawRect(new Rect(outer.x, outer.yMax - tick, 3, tick), cursorWhite);
            DrawRect(new Rect(outer.xMax - tick, outer.yMax - 3, tick, 3), cursorWhite);
            DrawRect(new Rect(outer.xMax - 3, outer.yMax - tick, 3, tick), cursorWhite);
        }

        private void DrawUnitBase(Rect cellRect, CombatUnit unit, float cell)
        {
            float hp = unit.MaxHp <= 0 ? 0f : Mathf.Clamp01((float)unit.Hp / unit.MaxHp);
            Color side = unit.Side == UnitSide.Party ? teal : blood;
            Rect baseRect = new Rect(cellRect.x + cell * 0.22f, cellRect.y + cell * 0.72f, cell * 0.56f, cell * 0.13f);
            DrawRect(baseRect, Hex("020303", 0.72f));
            DrawRect(new Rect(baseRect.x + cell * 0.05f, baseRect.y + cell * 0.035f, (baseRect.width - cell * 0.10f) * hp, baseRect.height * 0.34f), Color.Lerp(side, cursorWhite, 0.16f));
        }

        private void DrawCombatStatusFrame(Rect cellRect, CombatUnit unit, bool active, float cell)
        {
            Color status = CombatFrameColor(unit, active);
            Rect outer = Pad(cellRect, cell * 0.055f);
            DrawRect(outer, Hex("030405", 0.16f));
            DrawBorder(outer, status, active ? 3 : 2);
            float corner = Mathf.Max(5f, cell * 0.16f);
            DrawRect(new Rect(outer.x, outer.y, corner, 3), status);
            DrawRect(new Rect(outer.x, outer.y, 3, corner), status);
            DrawRect(new Rect(outer.xMax - corner, outer.y, corner, 3), status);
            DrawRect(new Rect(outer.xMax - 3, outer.y, 3, corner), status);
            DrawRect(new Rect(outer.x, outer.yMax - 3, corner, 3), status);
            DrawRect(new Rect(outer.x, outer.yMax - corner, 3, corner), status);
            DrawRect(new Rect(outer.xMax - corner, outer.yMax - 3, corner, 3), status);
            DrawRect(new Rect(outer.xMax - 3, outer.yMax - corner, 3, corner), status);

            string marker = CombatFrameMarker(unit, active);
            if (!string.IsNullOrEmpty(marker))
            {
                Rect tag = new Rect(outer.x + cell * 0.07f, outer.y + cell * 0.07f, cell * 0.42f, cell * 0.20f);
                DrawRect(tag, Hex("030405", 0.84f));
                DrawBorder(tag, status, 1);
                GUI.Label(tag, marker, CenterStyle(Mathf.RoundToInt(Mathf.Clamp(cell * 0.13f, 9f, 14f)), status));
            }
        }

        private Color CombatFrameColor(CombatUnit unit, bool active)
        {
            if (unit == null) return muted;
            if (active) return gold;
            if (unit.Poisoned > 0 || unit.Webbed > 0) return poison;
            if (unit.Hexed > 0 || unit.Sleeping > 0 || unit.Stunned > 0) return violet;
            if (unit.Shielded > 0 || unit.Regenerating > 0) return teal;
            if (unit.Guarding) return Hex("a9b0a2");
            if (unit.MaxHp > 0 && unit.Hp <= Mathf.CeilToInt(unit.MaxHp * 0.42f)) return blood;
            return unit.Side == UnitSide.Party ? Hex("58b7a5", 0.78f) : Hex("b94b56", 0.78f);
        }

        private string CombatFrameMarker(CombatUnit unit, bool active)
        {
            if (active) return "ACT";
            if (unit == null) return "";
            if (unit.Poisoned > 0) return "PSN";
            if (unit.Webbed > 0) return "WEB";
            if (unit.Hexed > 0) return "HEX";
            if (unit.Sleeping > 0) return "ZZ";
            if (unit.Stunned > 0) return "STN";
            if (unit.Shielded > 0) return "WRD";
            if (unit.Guarding) return "GRD";
            if (unit.MaxHp > 0 && unit.Hp <= Mathf.CeilToInt(unit.MaxHp * 0.42f)) return "HURT";
            return "";
        }

        private void DrawCombatReadout(Rect grid, CombatUnit active)
        {
            if (active == null) return;
            float y = Mathf.Min(grid.yMax + 8, boardRect.yMax - 52);
            Rect strip = new Rect(grid.x, y, grid.width, 48);
            DrawRect(strip, Hex("030405", 0.96f));
            DrawBorder(strip, active.Side == UnitSide.Party ? teal.WithAlpha(0.74f) : blood.WithAlpha(0.74f), 1);
            TryDrawCombatUiAtlasIcon(new Rect(strip.x + 6, strip.y + 5, 34, 34), 2, Color.white.WithAlpha(0.58f));
            string identity = CombatIdentityLine(active);
            string stat = $"{active.Name}  {identity}";
            string detail = CombatPhaseLabel() + " / " + ActiveReadoutDetail(active);
            Rect badge = new Rect(strip.xMax - 136, strip.y + 6, 124, 30);
            float statW = Mathf.Clamp(strip.width * 0.34f, 220f, 360f);
            float meterY = strip.y + 26f;
            float detailX = strip.x + 44f + statW + 18f;
            float detailW = Mathf.Max(120f, badge.x - detailX - 14f);
            DrawActionStateBadge(badge, state.Combat.ActionAvailable);
            GUI.Label(new Rect(strip.x + 44, strip.y + 3, statW, 20), FitText(stat, statW, CenterLeftStyle(15, cursorWhite)), CenterLeftStyle(15, cursorWhite));
            DrawMiniCombatMeter(new Rect(strip.x + 44, meterY, 132, 10), "HP", active.Hp, active.MaxHp, active.Side == UnitSide.Party ? ember : blood, 12);
            DrawMiniCombatMeter(new Rect(strip.x + 186, meterY, 108, 10), "MP", active.Mana, active.MaxMana, violet, 13);
            GUI.Label(new Rect(detailX, strip.y + 5, detailW, 17), FitText(detail, detailW, CenterLeftStyle(12, muted)), CenterLeftStyle(12, muted));
            GUI.Label(new Rect(detailX, strip.y + 24, detailW, 16), FitText(ActiveCommandPrompt(active), detailW, CenterLeftStyle(10, gold)), CenterLeftStyle(10, gold));
        }

        private void DrawMiniCombatMeter(Rect rect, string label, int value, int max, Color fill, int atlasIndex)
        {
            if (max <= 0) return;
            TryDrawCombatUiAtlasIcon(new Rect(rect.x, rect.y - 3f, 18f, 18f), atlasIndex, Color.white.WithAlpha(0.42f));
            GUI.Label(new Rect(rect.x + 18f, rect.y - 4f, 22f, 16f), label, CenterLeftStyle(9, muted));
            Rect bar = new Rect(rect.x + 38f, rect.y, Mathf.Max(10f, rect.width - 38f), rect.height);
            DrawMeter(bar, value, max, fill);
            DrawBorder(bar, Hex("030405", 0.76f), 1);
        }

        private void DrawActionStateBadge(Rect rect, bool ready)
        {
            Color accent = ready ? teal : ember;
            DrawRect(rect, ready ? Hex("14342e", 0.95f) : Hex("3d2523", 0.95f));
            Rect icon = new Rect(rect.x + 3f, rect.y + 2f, 26f, 26f);
            if (!TryDrawCombatHudUiAtlasIcon(icon, ready ? 6 : 7, Color.white.WithAlpha(0.74f)))
            {
                TryDrawCombatUiAtlasIcon(icon, ready ? 5 : 6, Color.white.WithAlpha(0.64f));
            }
            DrawBorder(rect, accent, 1);
            GUI.Label(new Rect(rect.x + 30f, rect.y + 5f, rect.width - 34f, 18f), ready ? "Action ready" : "Action used", CenterLeftStyle(11, cursorWhite));
        }

        private string CombatPhaseLabel()
        {
            if (state?.Combat == null) return "No combat";
            switch (state.Combat.Phase)
            {
                case CombatPhase.ChooseTarget: return "Choose target";
                case CombatPhase.Resolving: return "Resolving";
                case CombatPhase.EnemyThinking: return "Enemy thinking";
                default: return "Choose action";
            }
        }

        private void DrawTurnQueue(Rect grid, CombatUnit active)
        {
            if (state?.Combat?.Units == null) return;
            List<CombatUnit> queue = UpcomingUnits(7).ToList();
            Rect strip = new Rect(grid.x, Mathf.Max(grid.y - 42, 76f), grid.width, 34);
            DrawRect(strip, Hex("030405", 0.88f));
            DrawBorder(strip, Hex("253029"), 1);
            Rect queueIcon = new Rect(strip.x + 5, strip.y + 4, 24, 24);
            if (!TryDrawCombatHudUiAtlasIcon(queueIcon, 11, Color.white.WithAlpha(0.78f)))
            {
                TryDrawCombatUiAtlasIcon(queueIcon, 1, Color.white.WithAlpha(0.70f));
            }
            GUI.Label(new Rect(strip.x + 34, strip.y + 7, 92, 18), "Turn Order", CenterLeftStyle(12, muted));
            float x = strip.x + 130;
            foreach (CombatUnit unit in queue)
            {
                bool isActive = active != null && unit.Id == active.Id;
                bool partyIcon = unit.Side == UnitSide.Party && !unit.Summoned;
                string queueLabel = partyIcon ? unit.Name : $"{ClassShortLabel(unit)} {unit.Name}";
                float iconW = partyIcon ? 22f : 0f;
                Rect chip = new Rect(x, strip.y + 5, Mathf.Min(122, Mathf.Max(64, queueLabel.Length * 7 + 18 + iconW)), 22);
                DrawRect(chip, isActive ? Hex("2d3438") : Hex("151b20"));
                DrawBorder(chip, isActive ? gold : (unit.Side == UnitSide.Party ? teal : blood), 1);
                if (partyIcon)
                {
                    DrawClassIcon(new Rect(chip.x + 3, chip.y + 2, 18, 18), unit.ClassKey, unit.Role, RoleColor(unit.Role));
                }
                float labelX = chip.x + 5 + iconW;
                GUI.Label(new Rect(labelX, chip.y + 2, chip.width - (labelX - chip.x) - 5, 16), FitText(queueLabel, chip.width - (labelX - chip.x) - 5, CenterStyle(11, isActive ? cursorWhite : muted)), CenterStyle(11, isActive ? cursorWhite : muted));
                x += chip.width + 6;
                if (x > strip.xMax - 50) break;
            }
        }

        private void DrawHoverPreview(Rect grid, float cell, CombatUnit active)
        {
            if (active == null || active.Side != UnitSide.Party || Event.current == null || !grid.Contains(Event.current.mousePosition)) return;
            int x = Mathf.FloorToInt((Event.current.mousePosition.x - grid.x) / cell);
            int y = Mathf.FloorToInt((Event.current.mousePosition.y - grid.y) / cell);
            if (x < 0 || x >= CombatW || y < 0 || y >= CombatH) return;

            string text = "";
            CombatUnit target = UnitAt(x, y);
            if (selectedAction == ActionMode.Move)
            {
                int distance = Distance(x, y, active.X, active.Y);
                int moveCost = MoveCostTo(active, x, y);
                string terrain = TerrainPreviewLine(ObstacleAt(x, y));
                if (distance <= 0) text = "current tile";
                else if (!CanStandAt(x, y)) text = "blocked";
                else if (moveCost >= UnreachableMoveCost) text = $"path blocked{terrain}";
                else if (moveCost <= state.Combat.MovePoints) text = $"move {moveCost}, {state.Combat.MovePoints - moveCost} left{terrain}";
                else text = $"too far by {moveCost - state.Combat.MovePoints}{terrain}";
            }
            else if (selectedAction == ActionMode.Attack && target != null)
            {
                text = AttackPreview(active, target);
            }
            else if (selectedAction == ActionMode.Attack)
            {
                Point cover = ObstacleAt(x, y);
                if (IsBreakableCover(cover)) text = CoverAttackPreview(active, cover);
            }
            else if (selectedAction == ActionMode.Cast)
            {
                FormulaDef formula = GetFormula(pendingFormulaCode);
                if (formula != null) text = FormulaPreview(active, formula, target, x, y);
                else text = "choose a spell card first";
            }

            if (string.IsNullOrEmpty(text)) return;
            Point coverTarget = ObstacleAt(x, y);
            string title = HoverPreviewTitle(active, target, coverTarget);
            Color accent = HoverPreviewAccent(text, target, coverTarget);
            Rect box = PlaceCombatTooltip(Event.current.mousePosition, 470f, 174f);
            DrawRect(box, Hex("080b0d", 0.96f));
            DrawBorder(box, accent, 1);
            DrawCombatUiCornerTrim(box, accent);
            DrawActionButtonGlyph(new Rect(box.x + 10, box.y + 10, 46, 46), selectedAction, true, true);
            GUI.Label(new Rect(box.x + 66, box.y + 7, box.width - 214, 20), title, CenterLeftStyle(13, cursorWhite));
            string[] previewLines = text.Split(new[] { '\n' }, 2);
            string stateLine = $"{ActionName(selectedAction)} / {CombatPhaseLabel()} / Move {state.Combat.MovePoints} / {(state.Combat.ActionAvailable ? "Action ready" : "Action used")}";
            GUI.Label(new Rect(box.x + 66, box.y + 29, box.width - 214, 16), FitText(stateLine, box.width - 214, CenterLeftStyle(10, state.Combat.ActionAvailable ? teal : ember)), CenterLeftStyle(10, state.Combat.ActionAvailable ? teal : ember));
            GUI.Label(new Rect(box.x + 10, box.y + 62, box.width - 158, 17), FitText(previewLines[0], box.width - 158, CenterLeftStyle(12, ink)), CenterLeftStyle(12, ink));
            if (previewLines.Length > 1)
            {
                GUI.Label(new Rect(box.x + 10, box.y + 83, box.width - 158, 16), FitText(previewLines[1], box.width - 158, CenterLeftStyle(10, muted)), CenterLeftStyle(10, muted));
            }
            string tileLine = CombatHoverTileLine(x, y, coverTarget);
            GUI.Label(new Rect(box.x + 10, box.y + 105, box.width - 158, 16), FitText(tileLine, box.width - 158, CenterLeftStyle(10, gold)), CenterLeftStyle(10, gold));
            DrawHoverTargetMiniPanel(new Rect(box.xMax - 138, box.y + 10, 126, 104), target, coverTarget);
            DrawPreviewChip(new Rect(box.x + 10, box.yMax - 50, 96, 18), ActionHotkeyLabel(selectedAction) + " key", teal);
            DrawPreviewChip(new Rect(box.x + 112, box.yMax - 50, 104, 18), target != null ? ClassShortLabel(target) : IsBreakableCover(coverTarget) ? "Cover" : "Open tile", target != null ? CombatFrameColor(target, false) : gold);
            DrawPreviewChip(new Rect(box.x + 222, box.yMax - 50, 92, 18), state.Combat.ActionAvailable ? "Ready" : "Used", state.Combat.ActionAvailable ? teal : ember);
            GUI.Label(new Rect(box.x + 10, box.yMax - 27, box.width - 20, 16), FitText(HoverClickInstruction(active, target, coverTarget, x, y) + " / " + ActionRuleLine(active), box.width - 20, CenterLeftStyle(10, muted)), CenterLeftStyle(10, muted));
        }

        private string CombatHoverTileLine(int x, int y, Point cover)
        {
            if (IsBreakableCover(cover)) return $"{CoverName(cover)}: blocks movement and direct shots; arcing spells may pass.";
            if (cover != null)
            {
                string kind = string.IsNullOrEmpty(cover.Kind) ? "terrain" : char.ToUpperInvariant(cover.Kind[0]) + cover.Kind.Substring(1);
                return $"{kind}: {TerrainPreviewLine(cover).Trim()}";
            }
            int index = CombatTerrainTextureIndex(x, y, null);
            if (index == 0) return "Terrain: grass or overgrowth, normal movement.";
            if (index == 1) return "Terrain: snow, visual only for now.";
            if (index == 2 || index == 3) return "Terrain: dirt road, normal movement.";
            if (index == 9 || index == 11) return "Terrain: cave floor or rubble, normal movement.";
            if (index == 10) return "Terrain: sewer grates, normal movement.";
            if (index == 12) return "Terrain: ice texture. Spell-created ice adds move cost.";
            if (index == 14) return "Terrain: scorched ground. Spell fire is dangerous.";
            return "Terrain: old stone floor, normal movement.";
        }

        private Rect PlaceCombatTooltip(Vector2 mouse, float width, float height)
        {
            float rightLimit = sideRect.width > 0f ? sideRect.x - 10f : Screen.width - 8f;
            float bottomLimit = Mathf.Min(Screen.height - 102f, boardRect.yMax + 58f);
            float x = mouse.x + 18f;
            if (x + width > rightLimit) x = mouse.x - width - 18f;
            x = Mathf.Clamp(x, 8f, Mathf.Max(8f, rightLimit - width));

            float y = mouse.y + 14f;
            if (y + height > bottomLimit) y = mouse.y - height - 18f;
            y = Mathf.Clamp(y, 8f, Mathf.Max(8f, bottomLimit - height));
            return new Rect(x, y, width, height);
        }

        private string HoverPreviewTitle(CombatUnit active, CombatUnit target, Point cover)
        {
            if (selectedAction == ActionMode.Move) return "Movement Preview";
            if (selectedAction == ActionMode.Attack && target != null) return target.Side == UnitSide.Enemy ? "Attack Preview" : "Friendly Unit";
            if (selectedAction == ActionMode.Attack && IsBreakableCover(cover)) return "Break Cover";
            if (selectedAction == ActionMode.Cast)
            {
                FormulaDef formula = GetFormula(pendingFormulaCode);
                return formula == null ? "Choose Spell" : formula.Name;
            }
            return "Combat Preview";
        }

        private Color HoverPreviewAccent(string text, CombatUnit target, Point cover)
        {
            string lowered = (text ?? "").ToLowerInvariant();
            if (lowered.Contains("blocked") || lowered.Contains("out of") || lowered.Contains("needs") || lowered.Contains("too far") || lowered.Contains("friendly")) return ember;
            if (target != null) return target.Side == UnitSide.Enemy ? blood : teal;
            if (IsBreakableCover(cover)) return gold;
            if (selectedAction == ActionMode.Cast) return violet;
            if (selectedAction == ActionMode.Move) return teal;
            return gold;
        }

        private int HoverPreviewHudIconIndex()
        {
            if (selectedAction == ActionMode.Move) return 0;
            if (selectedAction == ActionMode.Attack) return 8;
            if (selectedAction == ActionMode.Cast) return 2;
            return 10;
        }

        private void DrawHoverTargetMiniPanel(Rect rect, CombatUnit target, Point cover)
        {
            DrawRect(rect, Hex("101619", 0.88f));
            DrawBorder(rect, target != null ? CombatFrameColor(target, false) : IsBreakableCover(cover) ? gold : line, 1);
            if (target != null)
            {
                GUI.Label(new Rect(rect.x + 6, rect.y + 4, rect.width - 12, 15), FitText(target.Name, rect.width - 12, CenterLeftStyle(10, cursorWhite)), CenterLeftStyle(10, cursorWhite));
                GUI.Label(new Rect(rect.x + 6, rect.y + 21, rect.width - 12, 14), FitText(CombatIdentityLine(target), rect.width - 12, CenterLeftStyle(9, muted)), CenterLeftStyle(9, muted));
                DrawMiniCombatMeter(new Rect(rect.x + 6, rect.y + 43, rect.width - 12, 8), "HP", target.Hp, target.MaxHp, target.Side == UnitSide.Party ? ember : blood, 12);
                string status = StatusCompactLine(target);
                GUI.Label(new Rect(rect.x + 6, rect.y + 63, rect.width - 12, 14), string.IsNullOrEmpty(status) ? "steady" : status, CenterLeftStyle(9, string.IsNullOrEmpty(status) ? muted : gold));
                GUI.Label(new Rect(rect.x + 6, rect.y + 81, rect.width - 12, 14), FitText(target.Side == UnitSide.Enemy ? EnemyThreatLine(target) : $"Range {target.Range} / {target.DamageType}", rect.width - 12, CenterLeftStyle(9, muted)), CenterLeftStyle(9, muted));
                return;
            }

            if (IsBreakableCover(cover))
            {
                GUI.Label(new Rect(rect.x + 6, rect.y + 5, rect.width - 12, 15), CoverName(cover), CenterLeftStyle(10, cursorWhite));
                GUI.Label(new Rect(rect.x + 6, rect.y + 28, rect.width - 12, 14), $"Integrity {CoverIntegrity(cover)}", CenterLeftStyle(9, gold));
                GUI.Label(new Rect(rect.x + 6, rect.y + 46, rect.width - 12, 14), cover.Duration > 0 ? $"{cover.Duration} turns" : "permanent", CenterLeftStyle(9, muted));
                GUI.Label(new Rect(rect.x + 6, rect.y + 66, rect.width - 12, 28), "Blocks movement and direct shots.", CenterLeftStyle(9, muted));
                return;
            }

            GUI.Label(new Rect(rect.x + 6, rect.y + 29, rect.width - 12, 16), "Open tile", CenterStyle(10, muted));
            GUI.Label(new Rect(rect.x + 6, rect.y + 52, rect.width - 12, 30), "Movement and targeting preview use the selected action.", CenterStyle(9, muted));
        }

        private void DrawPreviewChip(Rect rect, string text, Color accent)
        {
            DrawRect(rect, Hex("151b20", 0.92f));
            DrawBorder(rect, accent.WithAlpha(0.72f), 1);
            GUI.Label(new Rect(rect.x + 5, rect.y + 2, rect.width - 10, rect.height - 3), FitText(text, rect.width - 10, CenterStyle(9, cursorWhite)), CenterStyle(9, cursorWhite));
        }

        private string HoverClickInstruction(CombatUnit active, CombatUnit target, Point cover, int x, int y)
        {
            if (selectedAction == ActionMode.Move)
            {
                int moveCost = MoveCostTo(active, x, y);
                return CanStandAt(x, y) && moveCost < UnreachableMoveCost && moveCost <= state.Combat.MovePoints ? "Click to move" : "Cannot move there";
            }
            if (selectedAction == ActionMode.Attack)
            {
                if (target != null && target.Side == UnitSide.Enemy) return state.Combat.ActionAvailable ? "Click to attack" : "Action already used";
                if (IsBreakableCover(cover)) return state.Combat.ActionAvailable ? "Click to break cover" : "Action already used";
                return "Choose an enemy or cover";
            }
            if (selectedAction == ActionMode.Cast)
            {
                FormulaDef formula = GetFormula(pendingFormulaCode);
                if (formula == null) return "Choose a spell card";
                return IsFormulaActionable(formula, active, target, x, y) ? "Click to cast" : "Cannot cast there";
            }
            return "Choose a command";
        }

        private void DrawHoverAim(Rect grid, float cell, CombatUnit active)
        {
            if (active == null || Event.current == null || !grid.Contains(Event.current.mousePosition)) return;
            int x = Mathf.FloorToInt((Event.current.mousePosition.x - grid.x) / cell);
            int y = Mathf.FloorToInt((Event.current.mousePosition.y - grid.y) / cell);
            if (x < 0 || x >= CombatW || y < 0 || y >= CombatH) return;

            CombatUnit target = UnitAt(x, y);
            Rect tile = new Rect(grid.x + x * cell, grid.y + y * cell, cell, cell);
            Color color = gold;
            bool drawLine = false;

            if (selectedAction == ActionMode.Move)
            {
                int moveCost = MoveCostTo(active, x, y);
                bool reachable = CanStandAt(x, y) && moveCost < UnreachableMoveCost;
                bool valid = reachable && moveCost <= state.Combat.MovePoints;
                color = valid ? teal : Hex("8a5c35");
                DrawTargetBadge(tile, reachable ? moveCost.ToString() : "X", color, valid);
            }
            else if (selectedAction == ActionMode.Attack && target != null)
            {
                drawLine = target.Side == UnitSide.Enemy;
                bool blocked = AttackPreview(active, target).Contains("blocked");
                color = blocked ? Hex("8a5c35") : blood;
                DrawTargetReticle(tile, color, blocked ? "BLOCK" : $"{AttackHitChance(active, target)}%", !blocked);
            }
            else if (selectedAction == ActionMode.Attack)
            {
                Point cover = ObstacleAt(x, y);
                if (IsBreakableCover(cover))
                {
                    drawLine = true;
                    bool valid = Distance(active.X, active.Y, x, y) <= active.Range && (active.Range <= 1 || HasLineOfSight(active.X, active.Y, x, y, true));
                    color = valid ? gold : Hex("8a5c35");
                    DrawTargetReticle(tile, color, valid ? "BREAK" : "NO", valid);
                }
            }
            else if (selectedAction == ActionMode.Cast)
            {
                FormulaDef formula = GetFormula(pendingFormulaCode);
                bool actionable = formula != null && IsFormulaActionable(formula, active, target, x, y);
                drawLine = actionable;
                color = formula == null ? violet : actionable ? FormulaColor(formula) : Hex("8a5c35");
                if (formula != null)
                {
                    DrawFormulaAreaPreview(grid, cell, formula, active, target, x, y);
                    string tag = FormulaCanArcOverCover(formula, x, y) ? "ARC" : FormulaBaseRequiresLineOfSight(formula) ? "CAST" : FormulaPathLabel(formula).ToUpperInvariant();
                    DrawTargetReticle(tile, color, actionable ? tag : "NO", actionable);
                }
            }

            DrawBorder(Pad(tile, cell * 0.05f), color, 2);
            if (!drawLine) return;

            Vector2 from = new Vector2(grid.x + (active.X + 0.5f) * cell, grid.y + (active.Y + 0.5f) * cell);
            Vector2 to = new Vector2(grid.x + (x + 0.5f) * cell, grid.y + (y + 0.5f) * cell);
            DrawPixelLine(from, to, color.WithAlpha(0.62f), Mathf.Max(2f, cell * 0.035f));
            if (color == Hex("8a5c35"))
            {
                DrawBlockingCoverMarkers(grid, cell, active.X, active.Y, x, y);
            }
        }

        private void DrawBlockingCoverMarkers(Rect grid, float cell, int ax, int ay, int bx, int by)
        {
            foreach (Point cover in BlockingCoverAlongLine(ax, ay, bx, by))
            {
                Rect tile = new Rect(grid.x + cover.X * cell, grid.y + cover.Y * cell, cell, cell);
                DrawTargetReticle(tile, Hex("8a5c35", 0.82f), CoverIntegrity(cover).ToString(), false);
            }
        }

        private void DrawTargetReticle(Rect tile, Color color, string label, bool valid)
        {
            Rect ring = Pad(tile, tile.width * 0.13f);
            DrawRect(new Rect(ring.x, ring.y, ring.width * 0.26f, 3f), color);
            DrawRect(new Rect(ring.x, ring.y, 3f, ring.height * 0.26f), color);
            DrawRect(new Rect(ring.xMax - ring.width * 0.26f, ring.y, ring.width * 0.26f, 3f), color);
            DrawRect(new Rect(ring.xMax - 3f, ring.y, 3f, ring.height * 0.26f), color);
            DrawRect(new Rect(ring.x, ring.yMax - 3f, ring.width * 0.26f, 3f), color);
            DrawRect(new Rect(ring.x, ring.yMax - ring.height * 0.26f, 3f, ring.height * 0.26f), color);
            DrawRect(new Rect(ring.xMax - ring.width * 0.26f, ring.yMax - 3f, ring.width * 0.26f, 3f), color);
            DrawRect(new Rect(ring.xMax - 3f, ring.yMax - ring.height * 0.26f, 3f, ring.height * 0.26f), color);
            DrawTargetBadge(tile, label, color, valid);
        }

        private void DrawTargetBadge(Rect tile, string label, Color color, bool valid)
        {
            if (string.IsNullOrEmpty(label)) return;
            float w = Mathf.Clamp(label.Length * 7f + 14f, 24f, tile.width * 0.78f);
            Rect badge = new Rect(tile.center.x - w * 0.5f, tile.y + tile.height * 0.08f, w, 18f);
            DrawRect(badge, valid ? Hex("050708", 0.86f) : Hex("2b1714", 0.88f));
            DrawBorder(badge, color, 1);
            GUI.Label(new Rect(badge.x + 2, badge.y + 1, badge.width - 4, 14), label, CenterStyle(9, valid ? cursorWhite : Hex("d98b6a")));
        }

        private void DrawFormulaAreaPreview(Rect grid, float cell, FormulaDef formula, CombatUnit active, CombatUnit target, int x, int y)
        {
            if (formula == null || active == null) return;
            if (Distance(active.X, active.Y, x, y) > EffectiveFormulaRange(formula, active)) return;
            if (!CanTargetFormula(formula, active, target, x, y)) return;

            bool legal = HasFormulaLineOfSight(formula, active, x, y) && active.Mana >= EffectiveFormulaMana(formula, active);
            Color color = legal ? FormulaColor(formula, 0.72f) : Hex("8a5c35", 0.64f);
            if (formula.Splash)
            {
                foreach (Point point in SplashPreviewTiles(x, y))
                {
                    Rect tile = new Rect(grid.x + point.X * cell, grid.y + point.Y * cell, cell, cell);
                    DrawRect(Pad(tile, cell * 0.18f), FormulaColor(formula, point.X == x && point.Y == y ? 0.22f : 0.12f));
                    DrawBorder(Pad(tile, cell * 0.16f), color, point.X == x && point.Y == y ? 2 : 1);
                }
            }
            else if (formula.Effect == "terrain")
            {
                Rect tile = new Rect(grid.x + x * cell, grid.y + y * cell, cell, cell);
                DrawBorder(Pad(tile, cell * 0.16f), color, 2);
            }
            DrawFormulaAimTrace(grid, cell, active, formula, x, y, legal);
        }

        private IEnumerable<Point> SplashPreviewTiles(int x, int y)
        {
            Point[] tiles =
            {
                new Point(x, y),
                new Point(x + 1, y),
                new Point(x - 1, y),
                new Point(x, y + 1),
                new Point(x, y - 1)
            };
            foreach (Point tile in tiles)
            {
                if (tile.X < 0 || tile.X >= CombatW || tile.Y < 0 || tile.Y >= CombatH) continue;
                yield return tile;
            }
        }

        private void DrawFormulaAimTrace(Rect grid, float cell, CombatUnit active, FormulaDef formula, int x, int y, bool legal)
        {
            if (active == null || formula == null) return;
            Vector2 from = new Vector2(grid.x + (active.X + 0.5f) * cell, grid.y + (active.Y + 0.5f) * cell);
            Vector2 to = new Vector2(grid.x + (x + 0.5f) * cell, grid.y + (y + 0.5f) * cell);
            Color color = legal ? FormulaColor(formula, 0.72f) : Hex("8a5c35", 0.70f);
            float thickness = Mathf.Max(2f, cell * 0.028f);
            bool arcing = FormulaCanArcOverCover(formula, x, y);
            if (arcing)
            {
                Vector2 mid = Vector2.Lerp(from, to, 0.5f) + new Vector2(0f, -cell * 0.28f);
                DrawPixelLine(from, mid, color, thickness);
                DrawPixelLine(mid, to, Color.Lerp(color, cursorWhite, 0.20f), thickness);
                DrawFormulaPathGlyph(new Rect(mid.x - cell * 0.10f, mid.y - cell * 0.10f, cell * 0.20f, cell * 0.20f), formula, color, true);
            }
            else
            {
                DrawPixelLine(from, to, color, thickness);
                foreach (Point cover in BlockingCoverAlongLine(active.X, active.Y, x, y))
                {
                    Rect tile = new Rect(grid.x + cover.X * cell, grid.y + cover.Y * cell, cell, cell);
                    DrawBorder(Pad(tile, cell * 0.10f), Hex("8a5c35", 0.76f), 2);
                    DrawRect(new Rect(tile.center.x - cell * 0.04f, tile.y + cell * 0.24f, cell * 0.08f, cell * 0.52f), Hex("8a5c35", 0.62f));
                    DrawRect(new Rect(tile.x + cell * 0.24f, tile.center.y - cell * 0.04f, cell * 0.52f, cell * 0.08f), Hex("8a5c35", 0.62f));
                }
            }
            DrawFormulaPathGlyph(new Rect(to.x - cell * 0.08f, to.y - cell * 0.08f, cell * 0.16f, cell * 0.16f), formula, color, arcing);
        }

        private void DrawFormulaPathGlyph(Rect rect, FormulaDef formula, Color color, bool arcing)
        {
            DrawRect(rect, Hex("050708", 0.54f));
            DrawBorder(rect, color, 1);
            if (arcing)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.16f, rect.y + rect.height * 0.58f, rect.width * 0.24f, rect.height * 0.10f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.38f, rect.y + rect.height * 0.34f, rect.width * 0.24f, rect.height * 0.10f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.60f, rect.y + rect.height * 0.58f, rect.width * 0.24f, rect.height * 0.10f), color);
            }
            else if (FormulaBaseRequiresLineOfSight(formula))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.14f, rect.y + rect.height * 0.44f, rect.width * 0.72f, rect.height * 0.12f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.62f, rect.y + rect.height * 0.28f, rect.width * 0.18f, rect.height * 0.44f), color);
            }
            else
            {
                DrawPixelCross(Pad(rect, rect.width * 0.24f), color);
            }
        }

        private IEnumerable<Point> BlockingCoverAlongLine(int ax, int ay, int bx, int by)
        {
            int dx = bx - ax;
            int dy = by - ay;
            int steps = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
            if (steps <= 1) yield break;
            for (int i = 1; i < steps; i++)
            {
                float t = i / (float)steps;
                int x = Mathf.RoundToInt(ax + dx * t);
                int y = Mathf.RoundToInt(ay + dy * t);
                Point obstacle = ObstacleAt(x, y);
                if (IsBreakableCover(obstacle)) yield return obstacle;
            }
        }

        private void DrawCellFlash(Rect rect, int x, int y)
        {
            float now = Time.time;
            foreach (CellFlash flash in flashes)
            {
                if (flash.X != x || flash.Y != y) continue;
                float t = Mathf.Clamp01((now - flash.Start) / flash.Duration);
                Color c = flash.Color.ToColor();
                c.a = (1f - t) * 0.42f;
                DrawRect(Pad(rect, rect.width * Mathf.Lerp(0.05f, 0.22f, t)), c);
                DrawBorder(Pad(rect, rect.width * 0.08f), Color.Lerp(c, cursorWhite, 0.25f), 1);
            }
        }

        private void DrawBeams(Rect grid, float cell)
        {
            float now = Time.time;
            foreach (BeamEffect beam in beams)
            {
                float t = Mathf.Clamp01((now - beam.Start) / beam.Duration);
                Color c = beam.Color.ToColor();
                c.a = 1f - t * 0.55f;
                Vector2 from = new Vector2(grid.x + (beam.FromX + 0.5f) * cell, grid.y + (beam.FromY + 0.5f) * cell);
                Vector2 to = new Vector2(grid.x + (beam.ToX + 0.5f) * cell, grid.y + (beam.ToY + 0.5f) * cell);
                Vector2 head = Vector2.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
                if (beam.Kind == "fireball")
                {
                    DrawPixelLine(from, head, ember.WithAlpha(c.a * 0.70f), Mathf.Max(4f, cell * 0.085f));
                    DrawPixelLine(from, head, gold.WithAlpha(c.a), Mathf.Max(2f, cell * 0.045f));
                    DrawRect(new Rect(head.x - cell * 0.12f, head.y - cell * 0.12f, cell * 0.24f, cell * 0.24f), ember.WithAlpha(c.a));
                    DrawRect(new Rect(head.x - cell * 0.06f, head.y - cell * 0.06f, cell * 0.12f, cell * 0.12f), cursorWhite.WithAlpha(c.a * 0.82f));
                    continue;
                }
                if (beam.Kind == "meteor" || beam.Kind == "meteor-small")
                {
                    DrawJaggedPixelLine(from, head, ember.WithAlpha(c.a), Mathf.Max(3f, cell * (beam.Kind == "meteor" ? 0.070f : 0.045f)), cell * 0.04f);
                    DrawPixelLine(from + new Vector2(-cell * 0.10f, 0f), head, gold.WithAlpha(c.a * 0.55f), Mathf.Max(2f, cell * 0.030f));
                    float size = beam.Kind == "meteor" ? 0.26f : 0.16f;
                    DrawRect(new Rect(head.x - cell * size * 0.5f, head.y - cell * size * 0.5f, cell * size, cell * size), Color.Lerp(ember, cursorWhite, 0.20f).WithAlpha(c.a));
                    continue;
                }
                if (beam.Kind == "arc")
                {
                    Vector2 mid = Vector2.Lerp(from, head, 0.5f) + new Vector2(0f, -cell * 0.18f * Mathf.Sin(t * Mathf.PI));
                    DrawPixelLine(from, mid, c, Mathf.Max(2f, cell * 0.035f));
                    DrawPixelLine(mid, head, Color.Lerp(c, cursorWhite, 0.28f), Mathf.Max(2f, cell * 0.030f));
                    DrawRect(new Rect(head.x - cell * 0.05f, head.y - cell * 0.05f, cell * 0.10f, cell * 0.10f), c);
                    continue;
                }
                if (beam.Kind == "death" || beam.Kind == "hex")
                {
                    DrawJaggedPixelLine(from, head, c, Mathf.Max(2f, cell * 0.035f), cell * 0.09f);
                    DrawRect(new Rect(head.x - cell * 0.07f, head.y - cell * 0.07f, cell * 0.14f, cell * 0.14f), Color.Lerp(c, retroBlack, 0.22f));
                    continue;
                }
                DrawPixelLine(from, head, c, Mathf.Max(beam.Kind == "shot" ? 2f : 3f, cell * (beam.Kind == "shot" ? 0.030f : 0.055f)));
                if (beam.Kind == "shot")
                {
                    Vector2 dir = (head - from).normalized;
                    if (dir.sqrMagnitude < 0.01f) dir = Vector2.right;
                    Vector2 side = new Vector2(-dir.y, dir.x);
                    Vector2 tail = head - dir * cell * 0.12f;
                    DrawPixelLine(head, tail + side * cell * 0.06f, c, Mathf.Max(2f, cell * 0.025f));
                    DrawPixelLine(head, tail - side * cell * 0.06f, c, Mathf.Max(2f, cell * 0.025f));
                }
                else if (beam.Kind == "heal")
                {
                    DrawPixelCross(new Rect(head.x - cell * 0.07f, head.y - cell * 0.07f, cell * 0.14f, cell * 0.14f), Color.Lerp(c, cursorWhite, 0.20f));
                    DrawBorder(new Rect(head.x - cell * 0.11f, head.y - cell * 0.11f, cell * 0.22f, cell * 0.22f), c.WithAlpha(c.a * 0.70f), 1);
                }
                else if (beam.Kind == "fire")
                {
                    DrawRect(new Rect(head.x - cell * 0.07f, head.y - cell * 0.03f, cell * 0.14f, cell * 0.16f), ember.WithAlpha(c.a));
                    DrawRect(new Rect(head.x - cell * 0.035f, head.y - cell * 0.12f, cell * 0.07f, cell * 0.19f), gold.WithAlpha(c.a));
                }
                else if (beam.Kind == "ice")
                {
                    DrawRect(new Rect(head.x - cell * 0.11f, head.y - cell * 0.025f, cell * 0.22f, cell * 0.05f), frost.WithAlpha(c.a));
                    DrawRect(new Rect(head.x - cell * 0.025f, head.y - cell * 0.11f, cell * 0.05f, cell * 0.22f), Hex("d6f4ff", c.a));
                }
                else if (beam.Kind == "spell")
                {
                    DrawRect(new Rect(head.x - cell * 0.08f, head.y - cell * 0.08f, cell * 0.16f, cell * 0.16f), c);
                    DrawRect(new Rect(head.x - cell * 0.035f, head.y - cell * 0.16f, cell * 0.07f, cell * 0.32f), Color.Lerp(c, cursorWhite, 0.35f));
                }
            }
        }

        private void DrawJaggedPixelLine(Vector2 from, Vector2 to, Color color, float thickness, float amplitude)
        {
            Vector2 delta = to - from;
            int segments = Mathf.Max(2, Mathf.CeilToInt(delta.magnitude / Mathf.Max(8f, amplitude)));
            Vector2 prev = from;
            Vector2 normal = delta.sqrMagnitude < 0.001f ? Vector2.up : new Vector2(-delta.y, delta.x).normalized;
            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector2 next = Vector2.Lerp(from, to, t);
                if (i < segments) next += normal * (((i & 1) == 0 ? -1f : 1f) * amplitude);
                DrawPixelLine(prev, next, color, thickness);
                prev = next;
            }
        }

        private void DrawCastGlyphs(Rect grid, float cell)
        {
            float now = Time.time;
            foreach (CastGlyph glyph in castGlyphs)
            {
                float t = Mathf.Clamp01((now - glyph.Start) / glyph.Duration);
                Color c = glyph.Color.ToColor();
                c.a = 1f - t * 0.35f;
                Rect tile = new Rect(grid.x + glyph.X * cell, grid.y + glyph.Y * cell, cell, cell);
                Rect ring = Pad(tile, cell * Mathf.Lerp(0.24f, 0.06f, t));
                if (glyph.Kind == "impact")
                {
                    DrawRect(new Rect(tile.x + cell * 0.16f, tile.center.y - cell * 0.025f, cell * 0.68f, cell * 0.05f), Color.Lerp(c, cursorWhite, 0.22f));
                    DrawRect(new Rect(tile.center.x - cell * 0.025f, tile.y + cell * 0.16f, cell * 0.05f, cell * 0.68f), Color.Lerp(c, cursorWhite, 0.22f));
                    DrawBorder(Pad(tile, cell * Mathf.Lerp(0.34f, 0.12f, t)), c, 1);
                    continue;
                }
                if (glyph.Kind == "area")
                {
                    DrawBorder(Pad(tile, cell * Mathf.Lerp(0.42f, 0.08f, t)), c, 2);
                    DrawRect(new Rect(tile.x + cell * 0.26f, tile.y + cell * 0.26f, cell * 0.48f, cell * 0.07f), c.WithAlpha(c.a * 0.70f));
                    DrawRect(new Rect(tile.x + cell * 0.26f, tile.y + cell * 0.66f, cell * 0.48f, cell * 0.07f), c.WithAlpha(c.a * 0.70f));
                    continue;
                }
                if (glyph.Kind == "fireball")
                {
                    int frame = t < 0.38f ? 3 : 4;
                    if (TryDrawEpicSpellEffectsAtlasIcon(Pad(tile, cell * Mathf.Lerp(0.10f, -0.02f, t)), frame, Color.white.WithAlpha(c.a)))
                    {
                        DrawBorder(Pad(tile, cell * Mathf.Lerp(0.34f, 0.05f, t)), ember.WithAlpha(c.a * 0.80f), 2);
                        continue;
                    }
                    DrawBorder(Pad(tile, cell * Mathf.Lerp(0.32f, 0.05f, t)), ember.WithAlpha(c.a), 2);
                    DrawRect(new Rect(tile.center.x - cell * 0.16f, tile.center.y - cell * 0.03f, cell * 0.32f, cell * 0.06f), gold.WithAlpha(c.a));
                    DrawRect(new Rect(tile.center.x - cell * 0.03f, tile.center.y - cell * 0.16f, cell * 0.06f, cell * 0.32f), gold.WithAlpha(c.a));
                    DrawRect(Pad(tile, cell * Mathf.Lerp(0.44f, 0.30f, t)), cursorWhite.WithAlpha(c.a * 0.22f));
                    continue;
                }
                if (glyph.Kind == "meteor")
                {
                    int frame = t < 0.34f ? 13 : 14;
                    if (TryDrawEpicSpellEffectsAtlasIcon(Pad(tile, cell * Mathf.Lerp(0.05f, -0.06f, t)), frame, Color.white.WithAlpha(c.a)))
                    {
                        DrawBorder(Pad(tile, cell * Mathf.Lerp(0.44f, 0.06f, t)), Color.Lerp(ember, gold, 0.22f).WithAlpha(c.a), 3);
                        continue;
                    }
                    DrawBorder(Pad(tile, cell * Mathf.Lerp(0.44f, 0.06f, t)), Color.Lerp(ember, gold, 0.22f).WithAlpha(c.a), 3);
                    DrawRect(new Rect(tile.x + cell * 0.22f, tile.y + cell * 0.66f, cell * 0.56f, cell * 0.08f), ember.WithAlpha(c.a));
                    DrawRect(new Rect(tile.x + cell * 0.32f, tile.y + cell * 0.50f, cell * 0.36f, cell * 0.08f), gold.WithAlpha(c.a));
                    DrawRect(new Rect(tile.center.x - cell * 0.04f, tile.y + cell * 0.16f, cell * 0.08f, cell * 0.68f), cursorWhite.WithAlpha(c.a * 0.46f));
                    continue;
                }
                if (glyph.Kind == "status")
                {
                    DrawBorder(Pad(tile, cell * Mathf.Lerp(0.36f, 0.10f, t)), c, 2);
                    DrawRect(new Rect(tile.x + cell * 0.30f, tile.y + cell * 0.20f, cell * 0.40f, cell * 0.08f), c.WithAlpha(c.a * 0.76f));
                    DrawRect(new Rect(tile.x + cell * 0.30f, tile.y + cell * 0.72f, cell * 0.40f, cell * 0.08f), c.WithAlpha(c.a * 0.76f));
                    DrawRect(new Rect(tile.x + cell * 0.20f, tile.y + cell * 0.30f, cell * 0.08f, cell * 0.40f), c.WithAlpha(c.a * 0.76f));
                    DrawRect(new Rect(tile.x + cell * 0.72f, tile.y + cell * 0.30f, cell * 0.08f, cell * 0.40f), c.WithAlpha(c.a * 0.76f));
                    continue;
                }
                if (glyph.Kind == "priest")
                {
                    DrawBorder(ring, Color.Lerp(teal, cursorWhite, 0.25f), 2);
                    DrawRect(new Rect(tile.center.x - cell * 0.035f, tile.y + cell * 0.16f, cell * 0.07f, cell * 0.62f), c);
                    DrawRect(new Rect(tile.x + cell * 0.22f, tile.center.y - cell * 0.035f, cell * 0.56f, cell * 0.07f), c);
                    DrawRect(new Rect(tile.x + cell * 0.33f, tile.y + cell * 0.28f, cell * 0.34f, cell * 0.08f), Hex("d6f4ff", c.a));
                    DrawRect(new Rect(tile.x + cell * 0.33f, tile.y + cell * 0.64f, cell * 0.34f, cell * 0.08f), Hex("97dbc2", c.a));
                }
                else
                {
                    DrawBorder(ring, Color.Lerp(c, gold, 0.25f), 2);
                    DrawRect(new Rect(tile.x + cell * 0.18f, tile.y + cell * 0.30f, cell * 0.64f, cell * 0.07f), c);
                    DrawRect(new Rect(tile.x + cell * 0.28f, tile.y + cell * 0.58f, cell * 0.54f, cell * 0.07f), Color.Lerp(c, cursorWhite, 0.18f));
                    DrawRect(new Rect(tile.x + cell * 0.28f, tile.y + cell * 0.30f, cell * 0.07f, cell * 0.35f), c);
                    DrawRect(new Rect(tile.x + cell * 0.64f, tile.y + cell * 0.30f, cell * 0.07f, cell * 0.35f), c);
                    DrawRect(new Rect(tile.x + cell * 0.43f, tile.y + cell * 0.18f, cell * 0.14f, cell * 0.14f), Color.Lerp(c, cursorWhite, 0.35f));
                }
            }
        }

        private void DrawPixelLine(Vector2 from, Vector2 to, Color color, float thickness)
        {
            Vector2 delta = to - from;
            int steps = Mathf.Max(1, Mathf.CeilToInt(delta.magnitude / Mathf.Max(2f, thickness)));
            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;
                Vector2 p = Vector2.Lerp(from, to, t);
                DrawRect(new Rect(p.x - thickness * 0.5f, p.y - thickness * 0.5f, thickness, thickness), color);
            }
        }

        private string ActiveReadoutDetail(CombatUnit active)
        {
            string budget = $"Move {state.Combat.MovePoints} / {(state.Combat.ActionAvailable ? "Action Ready" : "Action Used")}";
            FormulaDef spell = GetFormula(pendingFormulaCode);
            if (spell != null) return $"{budget} / {spell.Name}";
            if (selectedAction == ActionMode.Cast && !string.IsNullOrEmpty(active.Spell)) return $"{budget} / choose spell";
            if (selectedAction == ActionMode.Move) return $"{budget} / move by distance";
            if (selectedAction == ActionMode.Guard) return $"{budget} / braced guard";
            if (selectedAction == ActionMode.Elixir) return $"{budget} / elixir ready";
            if (selectedAction == ActionMode.Wait) return $"{budget} / end turn";
            return $"{budget} / {ActiveWeaponLabel(active)} / {ArmorLabel(active)}";
        }

        private string ActionRuleLine(CombatUnit active)
        {
            if (active == null) return "";
            if (selectedAction == ActionMode.Move) return $"spend move points; {state.Combat.MovePoints} left";
            if (selectedAction == ActionMode.Attack) return state.Combat.ActionAvailable ? $"uses action; range {active.Range}" : "action already used";
            if (selectedAction == ActionMode.Cast)
            {
                FormulaDef formula = GetFormula(pendingFormulaCode);
                if (formula == null) return "choose a spell card";
                return $"{EffectiveFormulaMana(formula, active)} MP, R{EffectiveFormulaRange(formula, active)}, {FormulaPathLabel(formula)}";
            }
            if (selectedAction == ActionMode.Guard) return "uses action; stronger before moving";
            if (selectedAction == ActionMode.Elixir) return "uses action; restores health and mana";
            return "ends the active unit turn";
        }

        private string ActiveCommandPrompt(CombatUnit active)
        {
            if (active == null || active.Side != UnitSide.Party) return "Enemy turn resolving.";
            if (!state.Combat.ActionAvailable && state.Combat.MovePoints <= 0) return "Turn spent. End Turn advances.";
            if (selectedAction == ActionMode.Move) return state.Combat.MovePoints > 0 ? "Click a highlighted tile to move." : "No movement remains.";
            if (selectedAction == ActionMode.Attack) return state.Combat.ActionAvailable ? "Click an enemy, or click tree/stone cover to break it." : "Attack already spent.";
            if (selectedAction == ActionMode.Cast)
            {
                FormulaDef formula = GetFormula(pendingFormulaCode);
                if (formula == null) return "Choose a spell card from the panel.";
                return $"Click a highlighted {FormulaTargetLabel(formula)} to cast {formula.Name}.";
            }
            if (selectedAction == ActionMode.Guard) return state.Combat.ActionAvailable ? "Press Guard to brace until next turn." : "Guard already spent.";
            if (selectedAction == ActionMode.Elixir) return state.Elixirs > 0 ? "Use a shared elixir as this unit's action." : "No elixirs remain.";
            return "Press End Turn to pass.";
        }

        private string ActiveWeaponLabel(CombatUnit active)
        {
            if (active == null) return "";
            if (!string.IsNullOrEmpty(active.WeaponName)) return active.WeaponName;
            if (active.Role == "bow") return "ashwood bow";
            if (active.Role == "pike") return "long spear";
            if (active.Role == "knife") return "short blade";
            if (active.Role == "shield" || active.Role == "ward") return "shield and iron";
            if (active.Role == "mender") return "prayer focus";
            if (active.Role == "ember") return "ember focus";
            if (active.Role == "hex") return "hex focus";
            return active.Range > 1 ? "ranged attack" : "bare hands";
        }

        private string ArmorLabel(CombatUnit active)
        {
            if (active == null || string.IsNullOrEmpty(active.ArmorName)) return "no armor";
            return active.ArmorName;
        }

        private void DrawCombatHighlights(Rect grid, float cell, CombatUnit active)
        {
            if (selectedAction == ActionMode.Move && state.Combat.MovePoints > 0)
            {
                int[,] reachable = ReachableMoveCosts(active);
                for (int y = 0; y < CombatH; y++)
                for (int x = 0; x < CombatW; x++)
                {
                    int distance = Distance(x, y, active.X, active.Y);
                    int moveCost = reachable[x, y];
                    if (distance > 0 && moveCost < UnreachableMoveCost && moveCost <= state.Combat.MovePoints && CanStandAt(x, y))
                    {
                        float alpha = Mathf.Lerp(0.30f, 0.14f, moveCost / (float)(CombatMoveAllowance + 2));
                        Point terrain = ObstacleAt(x, y);
                        Rect mark = new Rect(grid.x + x * cell + 4, grid.y + y * cell + 4, cell - 8, cell - 8);
                        DrawRect(mark, TerrainHighlightColor(terrain, alpha));
                        if (TerrainMoveExtraCost(terrain) > 0)
                        {
                            DrawBorder(Pad(mark, cell * 0.08f), Hex("d7a84e", 0.58f), 1);
                        }
                    }
                }
            }

            if (selectedAction == ActionMode.Attack || selectedAction == ActionMode.Cast)
            {
                FormulaDef formula = selectedAction == ActionMode.Cast ? GetFormula(pendingFormulaCode) : null;
                int range = selectedAction == ActionMode.Cast ? formula != null ? EffectiveFormulaRange(formula, active) : 4 : active.Range;
                for (int y = 0; y < CombatH; y++)
                for (int x = 0; x < CombatW; x++)
                {
                    if (Distance(x, y, active.X, active.Y) <= range)
                    {
                        Color highlight = Hex("d7a84e", 0.15f);
                        if (selectedAction == ActionMode.Cast && formula != null)
                        {
                            CombatUnit unit = UnitAt(x, y);
                            if (CanTargetFormula(formula, active, unit, x, y))
                            {
                                if (!HasFormulaLineOfSight(formula, active, x, y)) highlight = Hex("8a5c35", 0.22f);
                                else highlight = FormulaColor(formula, 0.25f);
                            }
                            else continue;
                        }
                        if (selectedAction == ActionMode.Attack)
                        {
                            CombatUnit unit = UnitAt(x, y);
                            Point cover = ObstacleAt(x, y);
                            if (unit != null && unit.Side == UnitSide.Enemy)
                            {
                                if (active.Range > 1 && !HasLineOfSight(active.X, active.Y, x, y, true)) highlight = Hex("8a5c35", 0.22f);
                                else highlight = Hex("d7a84e", 0.22f);
                            }
                            else if (IsBreakableCover(cover))
                            {
                                highlight = Hex("d7a84e", 0.18f);
                            }
                            else continue;
                        }
                        DrawRect(new Rect(grid.x + x * cell + 6, grid.y + y * cell + 6, cell - 12, cell - 12), highlight);
                    }
                }
            }
        }

        private void HandleCombatMouse(Rect grid, float cell)
        {
            if (showArmory || showSpellbook) return;
            Event e = Event.current;
            if (e.type != EventType.MouseDown || !grid.Contains(e.mousePosition)) return;
            CombatUnit active = CurrentUnit();
            if (active == null || active.Side != UnitSide.Party) return;
            int x = Mathf.FloorToInt((e.mousePosition.x - grid.x) / cell);
            int y = Mathf.FloorToInt((e.mousePosition.y - grid.y) / cell);
            CombatUnit target = UnitAt(x, y);

            if (selectedAction == ActionMode.Move)
            {
                MoveActiveTo(active, x, y);
                e.Use();
                return;
            }
            if (selectedAction == ActionMode.Attack && state.Combat.ActionAvailable && target != null && target.Side == UnitSide.Enemy)
            {
                if (Attack(active, target))
                {
                    state.Combat.ActionAvailable = false;
                    state.Combat.Acted = true;
                    state.Combat.Phase = CombatPhase.Resolving;
                    SyncPartyFromCombat();
                    NextTurn();
                }
                e.Use();
                return;
            }
            if (selectedAction == ActionMode.Attack && state.Combat.ActionAvailable)
            {
                Point cover = ObstacleAt(x, y);
                if (IsBreakableCover(cover))
                {
                    if (AttackCover(active, cover))
                    {
                        state.Combat.ActionAvailable = false;
                        state.Combat.Acted = true;
                        state.Combat.Phase = CombatPhase.Resolving;
                        SyncPartyFromCombat();
                        NextTurn();
                    }
                    e.Use();
                    return;
                }
            }
            if (selectedAction == ActionMode.Cast && state.Combat.ActionAvailable)
            {
                if (string.IsNullOrEmpty(pendingFormulaCode))
                {
                    PushLog("Choose a spell card first.", Tone.Warn);
                    PlaySfx("blocked", 0.62f);
                    e.Use();
                    return;
                }
                bool castWorked = CastFormula(active, pendingFormulaCode, target, x, y);
                if (castWorked)
                {
                    ClearFormulaEntry();
                    state.Combat.ActionAvailable = false;
                    state.Combat.Acted = true;
                    state.Combat.Phase = CombatPhase.Resolving;
                    SyncPartyFromCombat();
                    NextTurn();
                }
                e.Use();
            }
        }

        private void DrawSidePanels()
        {
            float sideW = Mathf.Clamp(Screen.width * 0.24f, 390f, 470f);
            sideRect = new Rect(Screen.width - sideW - 12, 62, sideW, Screen.height - 74);
            float gap = 10f;
            float minLogH = Mathf.Min(190f, Mathf.Max(112f, sideRect.height * 0.20f));
            float companyH = state.Mode == GameMode.Combat
                ? Mathf.Clamp(sideRect.height * 0.32f, 220f, 330f)
                : Mathf.Clamp(sideRect.height * 0.42f, 250f, 390f);
            float enemiesH = state.Mode == GameMode.Combat ? Mathf.Clamp(sideRect.height * 0.20f, 132f, 210f) : Mathf.Clamp(sideRect.height * 0.18f, 110f, 150f);
            if (companyH + enemiesH + minLogH + gap * 2f > sideRect.height)
            {
                float usable = sideRect.height - minLogH - gap * 2f;
                companyH = Mathf.Max(200f, usable * 0.68f);
                enemiesH = Mathf.Max(104f, usable - companyH);
            }

            Rect companyRect = new Rect(sideRect.x, sideRect.y, sideRect.width, companyH);
            Rect enemiesRect = new Rect(sideRect.x, companyRect.yMax + gap, sideRect.width, enemiesH);
            Rect logRect = new Rect(sideRect.x, enemiesRect.yMax + gap, sideRect.width, Mathf.Max(96f, sideRect.yMax - enemiesRect.yMax - gap));

            DrawRpgPanel(companyRect, teal);
            DrawPanelHeader(companyRect, "Party", "party", teal, state.Party.Count + " sworn");
            float rosterTop = companyRect.y + 45f;
            float rosterH = Mathf.Max(30f, companyRect.yMax - rosterTop - 10f);
            int partyCount = Mathf.Max(1, state.Party.Count);
            float memberGap = 4f;
            float memberMinH = sideRect.height < 700f ? 25f : 32f;
            float memberH = Mathf.Clamp((rosterH - memberGap * Mathf.Max(0, partyCount - 1)) / partyCount, memberMinH, 78f);
            float y = rosterTop;
            CombatUnit active = CurrentUnit();
            foreach (PartyMember member in state.Party)
            {
                DrawMemberCard(new Rect(companyRect.x + 10, y, companyRect.width - 20, memberH), member, active?.PartyIndex == state.Party.IndexOf(member));
                y += memberH + memberGap;
                if (y > companyRect.yMax - memberH) break;
            }

            DrawRpgPanel(enemiesRect, blood);
            DrawPanelHeader(enemiesRect, "Enemy Combatants", "enemy", blood, state.Mode == GameMode.Combat ? $"Round {state.Combat.Round}" : "Round 0");
            float enemyTop = enemiesRect.y + 45f;
            float enemyAreaH = Mathf.Max(28f, enemiesRect.yMax - enemyTop - 10f);
            List<CombatUnit> enemies = state.Mode == GameMode.Combat ? state.Combat.Units.Where(u => u.Side == UnitSide.Enemy).ToList() : new List<CombatUnit>();
            if (enemies.Count == 0)
            {
                GUI.Label(new Rect(enemiesRect.x + 14, enemyTop, enemiesRect.width - 28, 24), "No active opposition.", mutedStyle);
            }
            int visibleEnemies = Mathf.Max(1, Mathf.Min(enemies.Count, 4));
            float enemyGap = 5f;
            float enemyH = Mathf.Clamp((enemyAreaH - enemyGap * Mathf.Max(0, visibleEnemies - 1)) / visibleEnemies, sideRect.height < 700f ? 42f : 52f, 76f);
            float ey = enemyTop;
            foreach (CombatUnit enemy in enemies)
            {
                DrawEnemyCard(new Rect(enemiesRect.x + 10, ey, enemiesRect.width - 20, enemyH), enemy);
                ey += enemyH + enemyGap;
                if (ey > enemiesRect.yMax - enemyH) break;
            }

            DrawRpgPanel(logRect, gold);
            DrawPanelHeader(logRect, "Timeline", "timeline", gold, "");
            Rect view = new Rect(logRect.x + 10, logRect.y + 44, logRect.width - 20, Mathf.Max(36f, logRect.height - 54));
            float contentW = view.width - 18;
            List<float> logHeights = new List<float>();
            float totalLogH = 0f;
            foreach (LogEntry entry in state.Log)
            {
                float rowH = Mathf.Clamp(logStyle.CalcHeight(new GUIContent(entry.Text), contentW - 18f) + 14f, 40f, 78f);
                logHeights.Add(rowH);
                totalLogH += rowH + 6f;
            }
            Rect content = new Rect(0, 0, contentW, Mathf.Max(view.height, totalLogH));
            logScroll = GUI.BeginScrollView(view, logScroll, content);
            float ly = 0;
            for (int i = 0; i < state.Log.Count; i++)
            {
                LogEntry entry = state.Log[i];
                float rowH = i < logHeights.Count ? logHeights[i] : 40f;
                Color stripe = entry.Tone == Tone.Warn ? ember : entry.Tone == Tone.Good ? teal : moss;
                Rect row = new Rect(0, ly, content.width, rowH);
                DrawRect(row, Hex("151b20"));
                DrawRect(new Rect(row.x, row.y, 4, row.height), stripe);
                GUI.Label(new Rect(row.x + 10, row.y + 6, row.width - 18, row.height - 10), entry.Text, logStyle);
                ly += rowH + 6f;
            }
            GUI.EndScrollView();
        }

        private void DrawMemberCard(Rect rect, PartyMember member, bool active)
        {
            bool compact = rect.height < 32f;
            Color accent = MemberColor(member);
            DrawRect(rect, active ? Hex("2d3438") : Hex("151b20"));
            DrawBorder(rect, active ? gold : line, active ? 2 : 1);
            DrawRect(new Rect(rect.x + 2, rect.y + 2, 3, rect.height - 4), accent);

            float portraitSize = Mathf.Clamp(rect.height - 6f, compact ? 22f : 40f, 68f);
            Rect portrait = new Rect(rect.x + 10, rect.y + (rect.height - portraitSize) * 0.5f, portraitSize, portraitSize);
            DrawMiniRolePortrait(portrait, member, accent);
            float classBadgeSize = Mathf.Clamp(portraitSize * 0.52f, 14f, 22f);
            DrawClassIcon(new Rect(portrait.xMax - classBadgeSize + 2f, portrait.y - 2f, classBadgeSize, classBadgeSize), member.ClassKey, member.Role, accent);

            float meterW = Mathf.Clamp(rect.width * 0.28f, compact ? 86f : 112f, compact ? 118f : 150f);
            float meterX = rect.xMax - meterW - 10f;
            float textX = portrait.xMax + 8f;
            float textW = Mathf.Max(90f, meterX - textX - 8f);
            GUIStyle nameStyle = CenterLeftStyle(compact ? 10 : rect.height < 54f ? 12 : 13, ink);
            GUIStyle metaStyle = CenterLeftStyle(compact ? 8 : rect.height < 54f ? 10 : 11, muted);
            GUI.Label(new Rect(textX, rect.y + (compact ? 2 : 4), textW, compact ? 12 : 16), FitText(member.Name, textW, nameStyle), nameStyle);
            string roleLine = $"L{member.Level} {DisplayRace(member.Race)} {DisplayClass(member.ClassKey)}";
            GUI.Label(new Rect(textX, rect.y + (compact ? 14 : rect.height < 42f ? 19 : 22), textW, compact ? 10 : 13), FitText(roleLine, textW, metaStyle), metaStyle);

            float meterH = compact ? 4f : rect.height < 42f ? 5f : 6f;
            float meterY = compact ? rect.y + 5f : rect.y + Mathf.Max(6f, (rect.height - (meterH * 3f + 7f)) * 0.5f);
            DrawLabeledMeter(new Rect(meterX, meterY, meterW, meterH), "H", member.Hp, member.MaxHp, blood);
            DrawLabeledMeter(new Rect(meterX, meterY + meterH + (compact ? 3f : 4f), meterW, meterH), "M", member.Mana, member.MaxMana, teal);
        }

        private string GearShortLine(PartyMember member)
        {
            string weapon = string.IsNullOrEmpty(member.WeaponName) ? StartingWeapon(member.Role) : member.WeaponName;
            string armor = string.IsNullOrEmpty(member.ArmorName) ? StartingArmor(member.Role) : member.ArmorName;
            return $"{TrimGearName(weapon)} / {TrimGearName(armor)}";
        }

        private string TrimGearName(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Length <= 28 ? text : text.Substring(0, 25) + "...";
        }

        private void DrawEnemyCard(Rect rect, CombatUnit enemy)
        {
            bool compact = rect.height < 44f;
            DrawRect(rect, Hex("151b20"));
            DrawBorder(rect, CombatFrameColor(enemy, false).WithAlpha(0.82f), 1);
            float portraitSize = Mathf.Clamp(rect.height - 10f, compact ? 30f : 42f, 58f);
            Rect portrait = new Rect(rect.x + 8, rect.y + (rect.height - portraitSize) * 0.5f, portraitSize, portraitSize);
            if (!TryDrawEnemyRosterPortrait(portrait, enemy) && !TryDrawAtlasCombatSprite(portrait, enemy))
            {
                DrawEnemyFigure(portrait, enemy.Role, enemy.Color.ToColor());
            }
            DrawBorder(portrait, CombatFrameColor(enemy, false), 1);
            DrawEnemyIntentSigil(new Rect(portrait.xMax - 10, portrait.yMax - 10, 14, 14), enemy);
            float meterW = Mathf.Clamp(rect.width * 0.34f, compact ? 108f : 132f, compact ? 144f : 176f);
            float meterX = rect.xMax - meterW - 10f;
            float textX = portrait.xMax + 8f;
            float textW = Mathf.Max(96f, meterX - textX - 8f);
            GUIStyle nameStyle = CenterLeftStyle(compact ? 10 : 13, ink);
            GUIStyle metaStyle = CenterLeftStyle(compact ? 8 : 11, muted);
            GUI.Label(new Rect(textX, rect.y + (compact ? 3 : 5), textW, compact ? 12 : 16), FitText(enemy.Name, textW, nameStyle), nameStyle);
            GUI.Label(new Rect(textX, rect.y + (compact ? 16 : 22), textW, compact ? 10 : 14), FitText(EnemyTraitLine(enemy), textW, metaStyle), metaStyle);
            if (!compact && rect.height >= 54f)
            {
                GUI.Label(new Rect(textX, rect.y + 38, textW, 12), FitText(EnemyTacticLine(enemy), textW, CenterLeftStyle(9, muted)), CenterLeftStyle(9, muted));
            }
            DrawLabeledMeter(new Rect(meterX, rect.y + (compact ? 8 : 12), meterW, compact ? 5 : 7), "H", enemy.Hp, enemy.MaxHp, blood);
            string status = StatusLine(enemy);
            GUIStyle statusStyle = CenterLeftStyle(compact ? 8 : 10, status == "steady" ? muted : gold);
            GUI.Label(new Rect(meterX, rect.y + (compact ? 17 : 23), meterW, compact ? 10 : 14), FitText(status == "steady" ? EnemyThreatLine(enemy) : status, meterW, statusStyle), statusStyle);
            if (!compact) DrawStatusPipRow(new Rect(meterX + 20, rect.y + rect.height - 12, meterW - 20, 8), enemy);
        }

        private void DrawMiniRolePortrait(Rect rect, PartyMember member, Color accent)
        {
            DrawRect(rect, Hex("050708", 0.82f));
            DrawBorder(rect, accent, 1);
            Rect inner = Pad(rect, rect.width * 0.18f);
            if (!TryDrawAtlasPartyPortrait(rect, member.Role))
            {
                DrawMiniRoleGlyph(inner, member.Role, accent);
            }
            Rect sigil = new Rect(rect.x + rect.width * 0.58f, rect.y + rect.height * 0.58f, rect.width * 0.28f, rect.height * 0.24f);
            DrawSigil(sigil, member.Sigil, ink);
        }

        private void DrawMiniRoleGlyph(Rect rect, string role, Color accent)
        {
            DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.18f, rect.width * 0.28f, rect.height * 0.68f), accent);
            DrawRect(new Rect(rect.x + rect.width * 0.40f, rect.y, rect.width * 0.20f, rect.height * 0.24f), Hex("d9a67b"));
            if (role == "shield" || role == "ward")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.06f, rect.y + rect.height * 0.38f, rect.width * 0.28f, rect.height * 0.38f), Hex("a9b0a2"));
            }
            else if (role == "pike")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.76f, rect.y - rect.height * 0.04f, rect.width * 0.08f, rect.height * 0.96f), ink);
            }
            else if (role == "bow")
            {
                DrawBorder(new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.16f, rect.width * 0.26f, rect.height * 0.62f), gold, 1);
            }
            else if (role == "knife")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.38f, rect.width * 0.26f, rect.height * 0.08f), ink);
            }
            else if (role == "mender")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.70f, rect.y + rect.height * 0.16f, rect.width * 0.08f, rect.height * 0.64f), teal);
                DrawRect(new Rect(rect.x + rect.width * 0.60f, rect.y + rect.height * 0.38f, rect.width * 0.28f, rect.height * 0.08f), teal);
            }
            else if (role == "ember" || role == "hex")
            {
                Color c = role == "ember" ? ember : violet;
                DrawRect(new Rect(rect.x + rect.width * 0.68f, rect.y + rect.height * 0.18f, rect.width * 0.12f, rect.height * 0.64f), c);
                DrawRect(new Rect(rect.x + rect.width * 0.62f, rect.y + rect.height * 0.04f, rect.width * 0.24f, rect.height * 0.18f), c);
            }
        }

        private void DrawEnemyIntentSigil(Rect rect, CombatUnit enemy)
        {
            Color color = DamageColor(enemy?.DamageType);
            DrawRect(rect, Hex("050708", 0.86f));
            DrawBorder(rect, color, 1);
            string glyph = IsCasterEnemy(enemy) ? "magic" : enemy != null && enemy.Range > 1 ? "bow" : "diamond";
            if (glyph == "magic") DrawTinyUiIcon(rect, "magic", color);
            else DrawSigil(Pad(rect, rect.width * 0.20f), glyph, color);
        }

        private void DrawStatusPipRow(Rect rect, CombatUnit unit)
        {
            List<StatusMark> pips = StatusMarks(unit);
            float size = Mathf.Max(4f, Mathf.Min(7f, rect.height));
            for (int i = 0; i < pips.Count && i < 6; i++)
            {
                Rect pip = new Rect(rect.x + i * (size + 3f), rect.y, size, size);
                DrawRect(pip, pips[i].Color);
                DrawBorder(pip, Hex("030405", 0.74f), 1);
            }
        }

        private void DrawStatusPips(Rect cellRect, CombatUnit unit, float cell)
        {
            List<StatusMark> pips = StatusMarks(unit);
            float size = Mathf.Max(6f, cell * 0.105f);
            for (int i = 0; i < pips.Count && i < 6; i++)
            {
                Rect pip = new Rect(cellRect.x + cell * 0.12f + i * (size + 2f), cellRect.y + cell * 0.09f, size, size);
                DrawRect(pip, pips[i].Color);
                DrawBorder(pip, Hex("030405", 0.74f), 1);
                if (size >= 7f)
                {
                    GUI.Label(new Rect(pip.x, pip.y - 1f, pip.width, pip.height + 2f), pips[i].Label, CenterStyle(7, retroBlack));
                }
            }
        }

        private void DrawStatusDurationBadges(Rect rect, CombatUnit unit)
        {
            List<StatusMark> marks = StatusMarks(unit);
            if (marks.Count == 0) return;
            float h = Mathf.Clamp(rect.height * 0.18f, 14f, 22f);
            float w = Mathf.Clamp(rect.width * 0.44f, 32f, 54f);
            float gap = Mathf.Max(2f, rect.height * 0.018f);
            float x = rect.xMax - w - rect.width * 0.04f;
            float y = rect.y + rect.height * 0.25f;
            for (int i = 0; i < marks.Count && i < 4; i++)
            {
                StatusMark mark = marks[i];
                Rect badge = new Rect(x, y + i * (h + gap), w, h);
                DrawRect(badge, Hex("050708", 0.82f));
                DrawRect(new Rect(badge.x, badge.y, Mathf.Min(badge.width, h * 0.55f), badge.height), mark.Color);
                DrawBorder(badge, mark.Color, 1);
                GUI.Label(new Rect(badge.x + h * 0.45f, badge.y - 1f, badge.width - h * 0.48f, badge.height + 2f), mark.Label + mark.Turns, CenterStyle(Mathf.RoundToInt(Mathf.Clamp(h * 0.62f, 8f, 11f)), cursorWhite));
            }
        }

        private List<StatusMark> StatusMarks(CombatUnit unit)
        {
            List<StatusMark> marks = new List<StatusMark>();
            if (unit == null) return marks;
            if (unit.Poisoned > 0) marks.Add(new StatusMark("P", poison, unit.Poisoned));
            if (unit.Bleeding > 0) marks.Add(new StatusMark("B", blood, unit.Bleeding));
            if (unit.Stunned > 0) marks.Add(new StatusMark("!", gold, unit.Stunned));
            if (unit.Sleeping > 0) marks.Add(new StatusMark("Z", violet, unit.Sleeping));
            if (unit.Webbed > 0) marks.Add(new StatusMark("W", Hex("d9d3c4"), unit.Webbed));
            if (unit.Shielded > 0) marks.Add(new StatusMark("S", teal, unit.Shielded));
            if (unit.Regenerating > 0) marks.Add(new StatusMark("+", Hex("97dbc2"), unit.Regenerating));
            if (unit.Hexed > 0) marks.Add(new StatusMark("H", violet, unit.Hexed));
            return marks;
        }

        private string StatusCompactLine(CombatUnit unit)
        {
            List<StatusMark> marks = StatusMarks(unit);
            if (marks.Count == 0) return "";
            return string.Join(" ", marks.Take(5).Select(m => m.Label + m.Turns).ToArray());
        }

        private string StatusName(StatusMark mark)
        {
            if (mark.Label == "P") return "poison";
            if (mark.Label == "B") return "bleed";
            if (mark.Label == "!") return "stun";
            if (mark.Label == "Z") return "sleep";
            if (mark.Label == "W") return "web";
            if (mark.Label == "S") return "ward";
            if (mark.Label == "+") return "regen";
            if (mark.Label == "H") return "hex";
            return "status";
        }

        private string EnemyTraitLine(CombatUnit enemy)
        {
            if (enemy == null) return "";
            List<string> parts = new List<string> { string.IsNullOrEmpty(enemy.Rank) ? enemy.Role : enemy.Rank + " " + enemy.Role };
            if (!string.IsNullOrEmpty(enemy.Resist)) parts.Add("res " + enemy.Resist.Replace("|", "/"));
            if (!string.IsNullOrEmpty(enemy.Weakness)) parts.Add("weak " + enemy.Weakness.Replace("|", "/"));
            if (!string.IsNullOrEmpty(enemy.StatusOnHit)) parts.Add(StatusLabel(enemy.StatusOnHit) + " hit");
            return string.Join(" / ", parts.Take(3).ToArray());
        }

        private string EnemyTacticLine(CombatUnit enemy)
        {
            if (enemy == null) return "";
            if (enemy.Role == "bonepriest") return "heals and wards";
            if (enemy.Role == "koboldraider") return "quick knife rush";
            if (enemy.Role == "koboldslinger") return "stone slinger";
            if (enemy.Role == "koboldshaman") return "hexing shaman";
            if (enemy.Role == "koboldwizard") return "death-ball wizard";
            if (enemy.Role == "koboldshield") return "small shield wall";
            if (enemy.Role == "sewerrat") return "fast sewer bite";
            if (enemy.Role == "giantrat") return "heavy rat swarm";
            if (enemy.Role == "ratfolk") return "scrap-tooth swarm";
            if (enemy.Role == "ratcutthroat") return "quick knife ambush";
            if (enemy.Role == "ratmage") return "plague caster";
            if (enemy.Role == "ratcleric") return "sewer healer";
            if (enemy.Role == "ratbrute") return "heavy ratfolk guard";
            if (enemy.Role == "drowscout") return "dark scout";
            if (enemy.Role == "drowblade") return "fast blade dancer";
            if (enemy.Role == "drowcrossbow") return "crossbow pressure";
            if (enemy.Role == "drowmage") return "dark-light caster";
            if (enemy.Role == "drowpriest") return "warding priest";
            if (enemy.Role == "lesserdemon") return "burning brute";
            if (enemy.Role == "mirearcher") return "poison archer";
            if (enemy.Role == "glassmage") return "cold hazard caster";
            if (enemy.Role == "adept") return "shock caster";
            if (enemy.Role == "spore") return "gas and poison";
            if (enemy.Role == "cinderling") return "leaves fire";
            if (enemy.Role == "shade") return "sleep and death";
            if (enemy.Role == "gloamknight") return "armored brute";
            if (enemy.Role == "thornbeast") return "bleeding brute";
            return enemy.Range > 1 ? "ranged pressure" : "front pressure";
        }

        private string EnemyThreatLine(CombatUnit enemy)
        {
            if (enemy == null) return "";
            string type = string.IsNullOrEmpty(enemy.DamageType) ? "physical" : enemy.DamageType;
            string rank = string.IsNullOrEmpty(enemy.Rank) ? "" : enemy.Rank + " / ";
            return $"{rank}{type} / range {enemy.Range}";
        }

        private string StatusLine(CombatUnit unit)
        {
            if (unit == null) return "";
            List<StatusMark> marks = StatusMarks(unit);
            List<string> parts = marks.Select(m => StatusName(m) + " " + m.Turns).ToList();
            return parts.Count == 0 ? "steady" : string.Join(", ", parts.Take(4).ToArray());
        }

        private void DrawCommandBar()
        {
            Rect baseRect = new Rect(12, Screen.height - 88, sideRect.x - 24, 74);
            if (baseRect.width < 480) return;
            if (state.Mode == GameMode.Explore)
            {
                if (GUI.Button(new Rect(baseRect.x + 58, baseRect.y, 56, 32), "N", buttonStyle)) TryMoveExplore(0, -1);
                if (GUI.Button(new Rect(baseRect.x, baseRect.y + 38, 56, 32), "W", buttonStyle)) TryMoveExplore(-1, 0);
                if (GUI.Button(new Rect(baseRect.x + 58, baseRect.y + 38, 56, 32), "S", buttonStyle)) TryMoveExplore(0, 1);
                if (GUI.Button(new Rect(baseRect.x + 116, baseRect.y + 38, 56, 32), "E", buttonStyle)) TryMoveExplore(1, 0);
                if (GUI.Button(new Rect(baseRect.x + 210, baseRect.y + 12, 86, 48), "Camp", buttonStyle)) Camp();
                GUI.enabled = CanDescend();
                if (GUI.Button(new Rect(baseRect.x + 306, baseRect.y + 12, 96, 48), "Descend", buttonStyle)) Descend();
                GUI.enabled = state.Elixirs > 0;
                if (GUI.Button(new Rect(baseRect.x + 412, baseRect.y + 12, 86, 48), "Elixir", buttonStyle)) UseElixir();
                GUI.enabled = true;
                if (baseRect.width > 610 && GUI.Button(new Rect(baseRect.x + 508, baseRect.y + 12, 92, 48), "Armory", buttonStyle)) ToggleArmory(0);
            }
            else if (state.Mode == GameMode.Combat)
            {
                CombatUnit active = CurrentUnit();
                bool playerTurn = active != null && active.Side == UnitSide.Party;
                ActionMode[] modes = { ActionMode.Move, ActionMode.Attack, ActionMode.Cast, ActionMode.Guard, ActionMode.Elixir, ActionMode.Wait };
                float gap = 10f;
                float reservedStatus = baseRect.width >= 820f ? 250f : 0f;
                float buttonW = Mathf.Clamp((baseRect.width - reservedStatus - gap * (modes.Length - 1)) / modes.Length, 62f, 88f);
                bool hasHoveredButton = false;
                ActionMode hoveredMode = ActionMode.Attack;
                Rect hoveredRect = Rect.zero;
                for (int i = 0; i < modes.Length; i++)
                {
                    GUI.enabled = playerTurn && ActionEnabled(modes[i], active);
                    Rect r = new Rect(baseRect.x + i * (buttonW + gap), baseRect.y + 5, buttonW, 64);
                    if (Event.current != null && r.Contains(Event.current.mousePosition))
                    {
                        hasHoveredButton = true;
                        hoveredMode = modes[i];
                        hoveredRect = r;
                    }
                    if (GUI.Button(r, "", buttonStyle))
                    {
                        SelectOrRunAction(modes[i], active);
                    }
                    DrawActionButtonGlyph(r, modes[i], playerTurn && ActionEnabled(modes[i], active), selectedAction == modes[i]);
                    DrawActionHotkeyBadge(r, ActionHotkeyLabel(modes[i]), playerTurn && ActionEnabled(modes[i], active), selectedAction == modes[i]);
                    if (selectedAction == modes[i]) DrawBorder(r, gold, 2);
                }
                GUI.enabled = true;
                float usedButtons = modes.Length * buttonW + (modes.Length - 1) * gap;
                if (baseRect.width - usedButtons >= 132f)
                {
                    DrawCommandStatus(new Rect(baseRect.x + usedButtons + 14f, baseRect.y + 5, baseRect.width - usedButtons - 14f, 64), active, playerTurn);
                }
                if (betaLabMode)
                {
                    float toolbarY = baseRect.y - 38;
                    DrawBetaLabToolbar(new Rect(baseRect.x, toolbarY, Mathf.Min(900, baseRect.width), 30), active, playerTurn);
                }
                if (showSpellbook) DrawSpellbookOverlay(active, playerTurn);
                if (hasHoveredButton) DrawActionButtonTooltip(hoveredRect, hoveredMode, active, playerTurn);
            }
        }

        private float FormulaPanelHeight(CombatUnit active, bool playerTurn)
        {
            bool casterReady = playerTurn && active != null && !string.IsNullOrEmpty(active.Spell);
            if (!casterReady) return 0f;
            return showSpellbook ? Mathf.Clamp(Screen.height * 0.64f, 360f, 620f) : 0f;
        }

        private void DrawSpellbookOverlay(CombatUnit active, bool playerTurn)
        {
            Rect shade = new Rect(0, 0, Screen.width, Screen.height);
            DrawRect(shade, Hex("020303", 0.62f));
            float w = Mathf.Clamp(Screen.width * 0.78f, 760f, 1180f);
            float h = Mathf.Clamp(Screen.height * 0.70f, 430f, 660f);
            Rect book = new Rect((Screen.width - w) * 0.5f, (Screen.height - h) * 0.5f, w, h);
            DrawRect(book, Hex("15100c", 0.98f));
            DrawBorder(book, gold, 2);
            DrawRect(new Rect(book.center.x - 2f, book.y + 18f, 4f, book.height - 36f), Hex("5b3a25", 0.88f));
            DrawRect(new Rect(book.x + 18f, book.y + 18f, book.width * 0.48f - 22f, book.height - 36f), Hex("171c20", 0.94f));
            DrawRect(new Rect(book.center.x + 8f, book.y + 18f, book.width * 0.48f - 26f, book.height - 36f), Hex("171c20", 0.94f));
            DrawCombatUiCornerTrim(book, violet);

            GUI.Label(new Rect(book.x + 28f, book.y + 18f, 360f, 28f), "Spellbook", h2Style);
            GUI.Label(new Rect(book.x + 30f, book.y + 50f, book.width - 180f, 22f), active == null ? "Waiting for an active caster." : $"{active.Name} / {SpellCraftLabel(active.Spell)} formulas", CenterLeftStyle(12, muted));
            if (GUI.Button(new Rect(book.xMax - 104f, book.y + 20f, 78f, 30f), "Close", smallButtonStyle))
            {
                showSpellbook = false;
                PlaySfx("ui", 0.45f);
            }

            Rect content = new Rect(book.x + 28f, book.y + 82f, book.width - 56f, book.height - 112f);
            DrawFormulaPanel(content, active, playerTurn);
        }

        private void DrawCommandStatus(Rect rect, CombatUnit active, bool playerTurn)
        {
            if (rect.width < 120 || active == null) return;
            DrawRect(rect, Hex("151a1f", 0.92f));
            DrawBorder(rect, playerTurn ? line : blood, 1);
            string focus = playerTurn && IsFocusedCaster(active) ? " / focused" : "";
            string top = playerTurn ? $"{state.Combat.MovePoints} move{focus}" : "enemy turn";
            string bottom = playerTurn ? CombatPhaseLabel() : active.Name;
            Rect icon = new Rect(rect.x + 5f, rect.y + 7f, 28f, 28f);
            if (!TryDrawCombatHudUiAtlasIcon(icon, state.Combat.ActionAvailable ? 6 : 7, Color.white.WithAlpha(playerTurn ? 0.74f : 0.36f)))
            {
                TryDrawCombatUiAtlasIcon(icon, state.Combat.ActionAvailable ? 5 : 6, Color.white.WithAlpha(playerTurn ? 0.62f : 0.36f));
            }
            GUI.Label(new Rect(rect.x + 38, rect.y + 5, rect.width - 46, 17), top, CenterLeftStyle(12, playerTurn ? cursorWhite : muted));
            GUI.Label(new Rect(rect.x + 38, rect.y + 24, rect.width - 46, 17), bottom, CenterLeftStyle(12, playerTurn && state.Combat.ActionAvailable ? teal : muted));
        }

        private void DrawActionButtonTooltip(Rect source, ActionMode mode, CombatUnit active, bool playerTurn)
        {
            if (active == null) return;
            bool enabled = playerTurn && ActionEnabled(mode, active);
            Color accent = enabled ? (selectedAction == mode ? gold : teal) : ember;
            Rect box = new Rect(source.x, Mathf.Max(112f, source.y - 102f), 286f, 88f);
            if (box.xMax > sideRect.x - 8f) box.x = Mathf.Max(8f, sideRect.x - 8f - box.width);
            DrawRect(box, Hex("080b0d", 0.97f));
            DrawBorder(box, accent, 1);
            DrawCombatUiCornerTrim(box, accent);
            DrawActionButtonGlyph(new Rect(box.x + 8f, box.y + 8f, 44f, 44f), mode, enabled, selectedAction == mode);
            GUI.Label(new Rect(box.x + 60f, box.y + 6f, box.width - 70f, 18f), ActionName(mode), CenterLeftStyle(12, cursorWhite));
            string stateLine = enabled ? $"{ActionHotkeyLabel(mode)} / {ActionButtonSubLabel(mode, active)} / Ready" : DisabledActionReason(mode, active, playerTurn);
            GUI.Label(new Rect(box.x + 60f, box.y + 26f, box.width - 70f, 15f), stateLine, CenterLeftStyle(10, enabled ? teal : ember));
            GUI.Label(new Rect(box.x + 10f, box.y + 50f, box.width - 20f, 28f), ActionTooltipLine(mode, active), CenterLeftStyle(10, muted));
        }

        private string ActionName(ActionMode mode)
        {
            switch (mode)
            {
                case ActionMode.Move: return "Move";
                case ActionMode.Attack: return "Attack";
                case ActionMode.Cast: return "Cast";
                case ActionMode.Guard: return "Guard";
                case ActionMode.Elixir: return "Elixir";
                case ActionMode.Wait: return "End Turn";
                default: return "Action";
            }
        }

        private string DisabledActionReason(ActionMode mode, CombatUnit active, bool playerTurn)
        {
            if (!playerTurn) return "Waiting for enemy turn";
            if (active == null) return "No active unit";
            if (mode == ActionMode.Move && state.Combat.MovePoints <= 0) return "No move points";
            if (mode == ActionMode.Move && active.Webbed > 0) return "Webbed";
            if (mode == ActionMode.Cast && string.IsNullOrEmpty(active.Spell)) return "No spell craft";
            if (mode == ActionMode.Elixir && state.Elixirs <= 0) return "No elixirs";
            if (mode != ActionMode.Move && mode != ActionMode.Wait && !state.Combat.ActionAvailable) return "Action already used";
            return "Unavailable";
        }

        private string ActionTooltipLine(ActionMode mode, CombatUnit active)
        {
            if (mode == ActionMode.Move) return "Click a highlighted tile. Distance and terrain spend move points.";
            if (mode == ActionMode.Attack) return $"Click an enemy or breakable cover. Current range {active.Range}.";
            if (mode == ActionMode.Cast)
            {
                if (string.IsNullOrEmpty(active.Spell)) return "Only trained spellcasters can open the spell panel.";
                FormulaDef formula = GetFormula(pendingFormulaCode);
                if (formula != null) return $"{formula.Name}: {EffectiveFormulaMana(formula, active)} MP, range {EffectiveFormulaRange(formula, active)}, {FormulaPathLabel(formula)}.";
                return "Open the Spellbook, choose one spell, then click a highlighted target.";
            }
            if (mode == ActionMode.Guard) return "Spend the action to reduce damage until next turn. Stronger before moving.";
            if (mode == ActionMode.Elixir) return "Spend the action to recover health and mana from the shared supply.";
            if (mode == ActionMode.Wait) return "End this unit's turn immediately.";
            return "";
        }

        private string ActionButtonSubLabel(ActionMode mode, CombatUnit active)
        {
            if (active == null) return "";
            switch (mode)
            {
                case ActionMode.Move: return state?.Combat == null ? "" : $"{state.Combat.MovePoints}";
                case ActionMode.Attack: return $"R{active.Range}";
                case ActionMode.Cast:
                    FormulaDef formula = GetFormula(pendingFormulaCode);
                    return formula == null ? "MP" : $"{EffectiveFormulaMana(formula, active)} MP";
                case ActionMode.Guard: return state?.Combat?.Moved == true ? "DEF" : "DEF+";
                case ActionMode.Elixir: return state == null ? "" : $"{state.Elixirs}";
                case ActionMode.Wait: return ">>";
                default: return "";
            }
        }

        private string ActionHotkeyLabel(ActionMode mode)
        {
            switch (mode)
            {
                case ActionMode.Move: return "1";
                case ActionMode.Attack: return "2";
                case ActionMode.Cast: return "3";
                case ActionMode.Guard: return "4";
                case ActionMode.Elixir: return "5";
                case ActionMode.Wait: return "6";
                default: return "";
            }
        }

        private void DrawActionButtonGlyph(Rect rect, ActionMode mode, bool enabled, bool selected)
        {
            Color color = enabled ? (selected ? gold : muted) : Hex("4f5558", 0.72f);
            float iconSize = Mathf.Clamp(Mathf.Min(rect.width, rect.height) * 0.80f, 44f, 62f);
            Rect icon = new Rect(rect.center.x - iconSize * 0.5f, rect.center.y - iconSize * 0.5f, iconSize, iconSize);
            DrawRect(icon, Hex("050708", selected ? 0.76f : 0.54f));
            DrawBorder(icon, color.WithAlpha(selected ? 0.95f : 0.70f), 1);
            Rect inner = Pad(icon, 4f);
            int commandIcon = -1;
            if (commandIcon >= 0 && TryDrawCombatCommandIconAtlasIcon(Pad(icon, 1f), commandIcon, Color.white.WithAlpha(enabled ? 0.96f : 0.34f)))
            {
                return;
            }
            int combatHudIcon = -1;
            if (combatHudIcon >= 0 && TryDrawCombatHudUiAtlasIcon(Pad(icon, 1f), combatHudIcon, Color.white.WithAlpha(enabled ? 0.94f : 0.36f)))
            {
                return;
            }
            int combatSpellbookIcon = -1;
            if (combatSpellbookIcon >= 0 && TryDrawCombatSpellbookUiAtlasIcon(Pad(icon, 1f), combatSpellbookIcon, Color.white.WithAlpha(enabled ? 0.90f : 0.36f)))
            {
                return;
            }
            int spellbookIcon = -1;
            if (spellbookIcon >= 0 && TryDrawSpellbookUiAtlasIcon(Pad(icon, 1f), spellbookIcon, Color.white.WithAlpha(enabled ? 0.88f : 0.36f)))
            {
                return;
            }
            int combatUiIndex = -1;
            if (combatUiIndex >= 0 && TryDrawCombatUiAtlasIcon(Pad(icon, 1f), combatUiIndex, Color.white.WithAlpha(enabled ? 0.84f : 0.36f)))
            {
                return;
            }
            int atlasIndex = -1;
            if (atlasIndex >= 0 && TryDrawMagicUiAtlasIcon(Pad(icon, 1f), atlasIndex, Color.white.WithAlpha(enabled ? 0.95f : 0.42f)))
            {
                return;
            }

            if (mode == ActionMode.Move)
            {
                DrawRect(new Rect(inner.x, inner.center.y - 1f, inner.width * 0.70f, 2f), color);
                DrawRect(new Rect(inner.x + inner.width * 0.58f, inner.y + inner.height * 0.18f, inner.width * 0.34f, inner.height * 0.22f), color);
                DrawRect(new Rect(inner.x + inner.width * 0.58f, inner.y + inner.height * 0.60f, inner.width * 0.34f, inner.height * 0.22f), color);
            }
            else if (mode == ActionMode.Attack)
            {
                DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.60f, inner.width * 0.66f, inner.height * 0.14f), color);
                DrawRect(new Rect(inner.x + inner.width * 0.58f, inner.y + inner.height * 0.18f, inner.width * 0.14f, inner.height * 0.54f), color);
                DrawRect(new Rect(inner.x + inner.width * 0.52f, inner.y + inner.height * 0.10f, inner.width * 0.34f, inner.height * 0.16f), cursorWhite.WithAlpha(color.a));
            }
            else if (mode == ActionMode.Cast)
            {
                DrawPixelCross(inner, color);
                DrawRect(new Rect(inner.center.x - 1f, inner.y, 2f, inner.height), color.WithAlpha(0.70f));
            }
            else if (mode == ActionMode.Guard)
            {
                DrawBorder(inner, color, 1);
                DrawRect(new Rect(inner.x + inner.width * 0.35f, inner.y + inner.height * 0.18f, inner.width * 0.30f, inner.height * 0.60f), color.WithAlpha(0.72f));
            }
            else if (mode == ActionMode.Elixir)
            {
                DrawRect(new Rect(inner.x + inner.width * 0.38f, inner.y, inner.width * 0.24f, inner.height * 0.25f), color);
                DrawRect(new Rect(inner.x + inner.width * 0.28f, inner.y + inner.height * 0.22f, inner.width * 0.44f, inner.height * 0.62f), teal.WithAlpha(color.a));
                DrawRect(new Rect(inner.x + inner.width * 0.34f, inner.y + inner.height * 0.52f, inner.width * 0.32f, inner.height * 0.08f), cursorWhite.WithAlpha(color.a));
            }
            else
            {
                DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.22f, inner.width * 0.18f, inner.height * 0.56f), color);
                DrawRect(new Rect(inner.x + inner.width * 0.58f, inner.y + inner.height * 0.22f, inner.width * 0.18f, inner.height * 0.56f), color);
            }
        }

        private void DrawActionHotkeyBadge(Rect rect, string hotkey, bool enabled, bool selected)
        {
            Rect badge = new Rect(rect.x + 7f, rect.y + 6f, 22f, 22f);
            Color accent = selected ? gold : enabled ? teal : line;
            DrawRect(badge, Hex("030405", 0.86f));
            DrawBorder(badge, accent.WithAlpha(0.82f), 1);
            GUI.Label(badge, hotkey, CenterStyle(11, enabled ? cursorWhite : muted));
        }

        private int ActionCombatSpellbookUiIconIndex(ActionMode mode)
        {
            switch (mode)
            {
                case ActionMode.Attack: return 0;
                case ActionMode.Guard: return 1;
                case ActionMode.Cast: return 2;
                case ActionMode.Wait: return 3;
                case ActionMode.Elixir: return 4;
                case ActionMode.Move: return 7;
                default: return -1;
            }
        }

        private int ActionCombatCommandIconIndex(ActionMode mode)
        {
            switch (mode)
            {
                case ActionMode.Move: return 0;
                case ActionMode.Attack: return 1;
                case ActionMode.Cast: return 2;
                case ActionMode.Guard: return 3;
                case ActionMode.Elixir: return 4;
                case ActionMode.Wait: return 5;
                default: return -1;
            }
        }

        private int ActionCombatHudIconIndex(ActionMode mode)
        {
            switch (mode)
            {
                case ActionMode.Move: return 0;
                case ActionMode.Attack: return 1;
                case ActionMode.Cast: return 2;
                case ActionMode.Guard: return 3;
                case ActionMode.Elixir: return 4;
                case ActionMode.Wait: return 5;
                default: return -1;
            }
        }

        private int ActionSpellbookIconIndex(ActionMode mode)
        {
            switch (mode)
            {
                case ActionMode.Attack: return 4;
                case ActionMode.Cast: return 5;
                case ActionMode.Wait: return 6;
                case ActionMode.Guard: return 7;
                case ActionMode.Move: return 8;
                case ActionMode.Elixir: return 9;
                default: return -1;
            }
        }

        private int ActionCombatUiIconIndex(ActionMode mode)
        {
            switch (mode)
            {
                case ActionMode.Move: return 4;
                case ActionMode.Attack: return 8;
                case ActionMode.Cast: return 7;
                case ActionMode.Guard: return 10;
                case ActionMode.Elixir: return 13;
                case ActionMode.Wait: return 6;
                default: return -1;
            }
        }

        private int ActionMagicIconIndex(ActionMode mode)
        {
            switch (mode)
            {
                case ActionMode.Move: return 12;
                case ActionMode.Attack: return 13;
                case ActionMode.Cast: return 11;
                case ActionMode.Guard: return 14;
                case ActionMode.Wait: return 15;
                default: return -1;
            }
        }

        private void DrawBetaLabToolbar(Rect rect, CombatUnit active, bool playerTurn)
        {
            DrawRect(rect, Hex("080b0d", 0.94f));
            DrawBorder(rect, Hex("c65c3b", 0.82f), 1);
            DrawFormulaLabRegion(new Rect(rect.x + 5, rect.y + 4, 24, 22), new Rect(1130, 718, 58, 58));
            GUI.Label(new Rect(rect.x + 34, rect.y + 5, 118, 18), "Beta Lab", CenterLeftStyle(12, gold));
            float x = rect.x + 154;
            float gap = 6f;
            string[] labels = { "Refill", "Reset", "Hazards", "Spawn", "SFX" };
            for (int i = 0; i < labels.Length; i++)
            {
                Rect button = new Rect(x + i * (74f + gap), rect.y + 3, 74, 24);
                if (GUI.Button(button, labels[i], smallButtonStyle))
                {
                    if (labels[i] == "Refill") RefillBetaLab();
                    else if (labels[i] == "Reset") StartBetaCombatLab();
                    else if (labels[i] == "Hazards") AddBetaLabHazards();
                    else if (labels[i] == "Spawn") SpawnBetaLabWave();
                    else TestSfx();
                }
            }

            if (active != null && rect.width > 650f)
            {
                string who = playerTurn ? $"{active.Name}: {CombatPhaseLabel()}" : $"{active.Name}: enemy test";
                GUI.Label(new Rect(rect.x + 558, rect.y + 5, rect.width - 568, 18), who, CenterLeftStyle(11, muted));
            }
        }

        private void RefillBetaLab()
        {
            if (state?.Combat?.Units == null) return;
            state.Elixirs = Mathf.Max(state.Elixirs, 9);
            foreach (CombatUnit unit in state.Combat.Units.Where(u => u.Side == UnitSide.Party))
            {
                unit.Hp = unit.MaxHp;
                unit.Mana = unit.MaxMana;
                unit.Poisoned = 0;
                unit.Bleeding = 0;
                unit.Stunned = 0;
                unit.Sleeping = 0;
                unit.Webbed = 0;
                unit.Hexed = 0;
                unit.Shielded = Mathf.Max(unit.Shielded, 1);
                AddFloat(unit.X, unit.Y, "refill", teal);
            }
            SyncPartyFromCombat();
            PushLog("Beta Lab refills party health, mana, elixirs, and clears afflictions.", Tone.Good);
            ShowBanner("Beta refill");
            PlaySfx("heal", 0.9f);
        }

        private void AddBetaLabHazards()
        {
            if (state?.Combat == null) return;
            AddOrRefreshObstacle(5, 1, "tree", SummonedTreeDuration);
            AddOrRefreshObstacle(6, 2, "stone", 0);
            AddOrRefreshObstacle(7, 3, "web", 9);
            AddOrRefreshObstacle(8, 4, "gas", 9);
            AddOrRefreshObstacle(6, 5, "fire", 7);
            AddOrRefreshObstacle(9, 5, "ice", 9);
            PushLog("Beta Lab refreshes tree, stone, web, gas, fire, and ice hazards.", Tone.Warn);
            ShowBanner("Hazards refreshed");
            PlaySfx("spell", 0.85f);
        }

        private void AddOrRefreshObstacle(int x, int y, string kind, int duration)
        {
            if (state?.Combat == null || UnitAt(x, y) != null) return;
            state.Combat.Obstacles.RemoveAll(o => o.X == x && o.Y == y);
            state.Combat.Obstacles.Add(new Point(x, y, kind, duration));
            AddFlash(x, y, TerrainHighlightColor(new Point(x, y, kind, duration), 0.8f));
        }

        private void SpawnBetaLabWave()
        {
            if (state?.Combat?.Units == null) return;
                string[] wave = { "koboldshaman", "koboldwizard", "bonepriest", "glassmage", "cinderling", "ratmage", "ratcleric", "drowmage", "drowpriest", "lesserdemon" };
            int spawned = 0;
            for (int i = 0; i < wave.Length && state.Combat.Units.Count(u => u.Hp > 0 && u.Side == UnitSide.Enemy) < 10; i++)
            {
                Point spot = FindBetaEnemySpawn();
                if (spot == null) break;
                CombatUnit enemy = MakeEnemy(wave[(state.Combat.Units.Count + i) % wave.Length], state.Combat.Units.Count + i);
                enemy.X = spot.X;
                enemy.Y = spot.Y;
                state.Combat.Units.Add(enemy);
                AddFlash(enemy.X, enemy.Y, blood);
                AddFloat(enemy.X, enemy.Y, "spawn", blood);
                spawned++;
            }

            if (spawned == 0)
            {
                PushLog("Beta Lab has no safe enemy spawn tile.", Tone.Warn);
                PlaySfx("blocked", 0.65f);
                return;
            }

            PushLog($"Beta Lab spawns {spawned} additional caster-pressure enemies.", Tone.Warn);
            ShowBanner($"Spawned {spawned}");
            PlaySfx("encounter", 0.8f);
        }

        private Point FindBetaEnemySpawn()
        {
            for (int x = CombatW - 2; x >= Mathf.Max(6, CombatW - 5); x--)
            for (int y = 0; y < CombatH; y++)
            {
                if (CanStandAt(x, y)) return new Point(x, y);
            }
            return null;
        }

        private void DrawFormulaPanel(Rect rect, CombatUnit active, bool playerTurn)
        {
            DrawRect(rect, Hex("151a1f", 0.96f));
            DrawBorder(rect, !string.IsNullOrEmpty(pendingFormulaCode) ? gold : line, 1);
            DrawCombatUiCornerTrim(rect, !string.IsNullOrEmpty(pendingFormulaCode) ? gold : violet);
            if (!TryDrawSpellbookUiAtlasIcon(new Rect(rect.xMax - 48f, rect.y + 6f, 42f, 42f), 0, Color.white.WithAlpha(0.34f)))
            {
                TryDrawCombatUiAtlasIcon(new Rect(rect.xMax - 42f, rect.y + 7f, 34f, 34f), 7, Color.white.WithAlpha(0.34f));
            }

            bool casterReady = playerTurn && active != null && !string.IsNullOrEmpty(active.Spell);
            if (!casterReady)
            {
                DrawFormulaLabIcon(new Rect(rect.x + 10, rect.y + 8, 24, 24), null, "");
                string text = active == null || active.Side != UnitSide.Party
                    ? "Spellbook: waiting for a party spellcaster turn."
                    : "No spell craft. Use clerics, ember mages, or hex mages for battle magic.";
                GUI.Label(new Rect(rect.x + 42, rect.y + 6, rect.width - 54, 18), text, CenterLeftStyle(12, muted));
                GUI.Label(new Rect(rect.x + 42, rect.y + 22, rect.width - 54, 14), "I opens Armory. C opens Spell Reference. Cast opens clickable spells for trained spellcasters.", CenterLeftStyle(10, muted));
                return;
            }

            if (rect.height < 70f)
            {
                FormulaDef compact = DefaultFormulaForCaster(active);
                DrawSpellbookHeaderIcon(new Rect(rect.x + 10, rect.y + 8, 24, 24), compact, active.Spell);
                GUI.Label(new Rect(rect.x + 42, rect.y + 5, rect.width - 54, 18), $"{active.Name} can cast {SpellCraftLabel(active.Spell)} spells.", CenterLeftStyle(12, ink));
                GUI.Label(new Rect(rect.x + 42, rect.y + 23, rect.width - 54, 16), "Press Cast to open the Spellbook. I and C remain normal hotkeys.", CenterLeftStyle(10, muted));
                return;
            }

            FormulaDef panelFormula = PanelFormula(active);
            string title = panelFormula != null ? panelFormula.Name : "Spellbook";
            string hint = panelFormula != null ? FormulaPanelHint(panelFormula, active) : FormulaCodexLine(active);
            Color titleColor = ink;

            FormulaDef iconFormula = panelFormula ?? DefaultFormulaForCaster(active);
            DrawSpellbookHeaderIcon(new Rect(rect.x + 10, rect.y + 9, 34, 34), iconFormula, active?.Spell);
            GUI.Label(new Rect(rect.x + 52, rect.y + 5, 210, 20), title, CenterLeftStyle(13, titleColor));
            float controlW = rect.width > 650f ? 78f : 0f;
            GUI.Label(new Rect(rect.x + 270, rect.y + 7, rect.width - 284 - controlW, 28), hint, CenterLeftStyle(12, muted));
            if (controlW > 0f)
            {
                Rect quick = new Rect(rect.xMax - controlW - 10f, rect.y + 10f, controlW, 24f);
                string quickLabel = panelFormula == null ? "Ready" : "Clear";
                if (GUI.Button(quick, quickLabel, smallButtonStyle))
                {
                    if (panelFormula == null)
                    {
                        FormulaDef first = DefaultFormulaForCaster(active) ?? KnownFormulasFor(active).FirstOrDefault();
                        if (first != null) PrepareFormulaCode(active, first.Code);
                    }
                    else
                    {
                        ClearFormulaEntry();
                        PlaySfx("ui", 0.5f);
                    }
                }
            }

            Rect detail = new Rect(rect.x + 52, rect.y + 35, rect.width - 64, 36);
            DrawRect(detail, Hex("080b0d", 0.74f));
            DrawBorder(detail, panelFormula == null ? line : FormulaColor(panelFormula, 0.72f), 1);
            TryDrawCombatUiAtlasIcon(new Rect(detail.xMax - 32f, detail.y + 3f, 26f, 26f), panelFormula == null ? 3 : (FormulaRequiresLineOfSight(panelFormula) ? 8 : 5), Color.white.WithAlpha(0.32f));
            GUI.Label(new Rect(detail.x + 8, detail.y + 3, detail.width - 16, 15), FormulaRuleSummary(panelFormula, active), CenterLeftStyle(10, panelFormula == null ? muted : ink));
            GUI.Label(new Rect(detail.x + 8, detail.y + 18, detail.width - 16, 15), FormulaEffectSummary(panelFormula, active), CenterLeftStyle(10, panelFormula == null ? muted : FormulaColor(panelFormula)));

            DrawFormulaChips(new Rect(rect.x + 12, rect.y + 78, rect.width - 24, rect.height - 102), active, playerTurn);
            DrawFormulaSchoolStrip(new Rect(rect.x + 12, rect.yMax - 20, rect.width - 24, 16), active);
        }

        private FormulaDef PanelFormula(CombatUnit active)
        {
            return GetFormula(pendingFormulaCode);
        }

        private string FormulaPanelHint(FormulaDef formula, CombatUnit active)
        {
            if (formula == null) return FormulaCodexLine(active);
            string armed = pendingFormulaCode == formula.Code ? "selected" : "preview";
            return $"{armed}: {formula.Hint}. Click a highlighted {FormulaTargetLabel(formula)}.";
        }

        private string FormulaRuleSummary(FormulaDef formula, CombatUnit active)
        {
            if (formula == null) return FormulaCodexLine(active);
            int mana = EffectiveFormulaMana(formula, active);
            int range = EffectiveFormulaRange(formula, active);
            string los = FormulaRequiresLineOfSight(formula) ? "direct sight" : "no direct sight needed";
            if (FormulaArcsOverCover(formula)) los = "arcs over trees";
            string focus = IsFocusedCaster(active) ? "focused" : state?.Combat?.Moved == true ? "moved" : "unfocused";
            return $"{SpellCraftLabel(formula.School)} {FormulaTierLabel(formula)} / {mana} MP / range {range} / {FormulaTargetLabel(formula)} / {los} / {focus}";
        }

        private string FormulaEffectSummary(FormulaDef formula, CombatUnit active)
        {
            if (formula == null) return "Choose a spell card.";
            if (formula.Effect == "terrain") return $"{TerrainDescription(formula.Terrain)} Duration {(formula.Duration <= 0 ? "permanent" : formula.Duration + " turns")}.";
            if (formula.Effect == "heal") return $"Heals about {FormulaHealPreview(formula, active).x}-{FormulaHealPreview(formula, active).y}; circle spells mend adjacent allies.";
            if (formula.Effect == "cure") return "Cleanses poison, bleed, web, stun, sleep, and hex.";
            if (formula.Effect == "status") return $"{StatusLabel(formula.Status)} for {Mathf.Max(1, formula.Duration)} turns; hostile targets roll against magic resistance.";
            if (formula.Effect == "drain") return $"Deals {formula.DamageType} damage and returns about half as healing.";
            if (formula.Effect == "summon") return $"Calls a fragile ally for {Mathf.Max(1, formula.Duration)} turns. It can move, block, and attack.";
            if (formula.Effect == "damage")
            {
                string splash = formula.Splash ? " with splash" : "";
                string status = string.IsNullOrEmpty(formula.Status) ? "" : $", may {StatusLabel(formula.Status)}";
                return $"{formula.DamageType} damage{splash}{status}; resistance and weakness apply.";
            }
            return formula.Hint;
        }

        private int FormulaTier(FormulaDef formula)
        {
            if (formula == null) return 1;
            if (formula.Code == "RLM" || formula.Code == "IBD" || formula.Code == "IBF" || (formula.Splash && formula.Mana >= 9)) return 4;
            if (formula.Splash || formula.Mana >= 8 || formula.Code == "TNC") return 3;
            if (formula.Mana >= 6 || formula.Effect == "status" || formula.Terrain == "stone" || formula.Terrain == "fire" || formula.Terrain == "gas") return 2;
            return 1;
        }

        private string FormulaTierLabel(FormulaDef formula)
        {
            switch (FormulaTier(formula))
            {
                case 4: return "elder";
                case 3: return "adept";
                case 2: return "apprentice";
                default: return "starter";
            }
        }

        private Color FormulaTierColor(FormulaDef formula)
        {
            switch (FormulaTier(formula))
            {
                case 4: return violet;
                case 3: return gold;
                case 2: return teal;
                default: return muted;
            }
        }

        private string FormulaTargetLabel(FormulaDef formula)
        {
            if (formula == null) return "target";
            if (formula.Effect == "summon") return "summon tile";
            if (formula.Target == "tile") return "open tile";
            if (formula.Target == "ally") return "ally";
            if (formula.Target == "enemy") return "enemy";
            if (formula.Target == "self") return "self";
            return "target";
        }

        private void DrawFormulaSchoolStrip(Rect rect, CombatUnit active)
        {
            if (active == null) return;
            string school = string.IsNullOrEmpty(active.Spell) ? "none" : SpellCraftLabel(active.Spell);
            string focus = IsFocusedCaster(active) ? "focused: -1 MP, +1 range, harder damage" : "move before casting removes focus bonus";
            string typed = selectedAction == ActionMode.Cast ? "choose a spell card" : "press Cast to open Spellbook";
            GUI.Label(rect, $"{school.ToUpperInvariant()} / {typed} / {focus}", CenterLeftStyle(10, gold));
        }

        private string SpellCraftLabel(string school)
        {
            if (string.IsNullOrEmpty(school)) return "spell";
            if (school.Contains("|")) return string.Join(" or ", school.Split('|').Select(SpellCraftLabel).ToArray());
            if (school.Equals("mend", StringComparison.OrdinalIgnoreCase)) return "cleric";
            if (school.Equals("ember", StringComparison.OrdinalIgnoreCase)) return "ember";
            if (school.Equals("hex", StringComparison.OrdinalIgnoreCase)) return "hex";
            if (school.Equals("pact", StringComparison.OrdinalIgnoreCase)) return "pact";
            return school;
        }

        private FormulaDef DefaultFormulaForCaster(CombatUnit active)
        {
            if (active == null || string.IsNullOrEmpty(active.Spell)) return null;
            if (CasterKnowsSchool(active.Spell, "mend")) return GetFormula("GBH");
            if (CasterKnowsSchool(active.Spell, "pact")) return GetFormula("IBD");
            if (CasterKnowsSchool(active.Spell, "ember")) return GetFormula("FBL") ?? GetFormula("FIF");
            if (CasterKnowsSchool(active.Spell, "hex")) return GetFormula("RLM");
            return KnownFormulasFor(active).FirstOrDefault();
        }

        private void DrawFormulaLabIcon(Rect rect, FormulaDef formula, string schoolFallback)
        {
            DrawRect(rect, Hex("050708", 0.82f));
            if (TryDrawSpellbookFormulaIcon(Pad(rect, 1f), formula, schoolFallback, Color.white.WithAlpha(0.94f)))
            {
                DrawBorder(rect, formula == null ? line : FormulaColor(formula), 1);
                return;
            }
            Rect source = FormulaLabIconRegion(formula, schoolFallback);
            if (!DrawFormulaLabRegion(rect, source))
            {
                Color color = formula == null ? muted : FormulaColor(formula);
                DrawBorder(rect, color, 1);
                DrawPixelCross(Pad(rect, rect.width * 0.24f), color);
                return;
            }
            DrawBorder(rect, formula == null ? line : FormulaColor(formula), 1);
        }

        private void DrawSpellbookHeaderIcon(Rect rect, FormulaDef formula, string schoolFallback)
        {
            DrawRect(rect, Hex("050708", 0.84f));
            bool drawn = TryDrawSpellbookUiAtlasIcon(Pad(rect, 1f), formula == null ? 0 : SpellbookFormulaIconIndex(formula, schoolFallback), Color.white.WithAlpha(0.94f));
            if (!drawn) DrawFormulaLabIcon(rect, formula, schoolFallback);
            else DrawBorder(rect, formula == null ? violet : FormulaColor(formula), 1);
        }

        private bool TryDrawSpellbookFormulaIcon(Rect rect, FormulaDef formula, string schoolFallback, Color tint)
        {
            int index = SpellbookFormulaIconIndex(formula, schoolFallback);
            if (index >= 0 && TryDrawSpellbookUiAtlasIcon(rect, index, tint)) return true;
            if (formula != null && (formula.Code == "FBL" || formula.Code == "MTR"))
            {
                int emberIndex = formula.Code == "MTR" ? 7 : 1;
                return TryDrawEmberSpellAtlasIcon(rect, emberIndex, tint);
            }
            return false;
        }

        private int SpellbookFormulaIconIndex(FormulaDef formula, string schoolFallback)
        {
            string code = formula?.Code ?? "";
            string type = formula?.DamageType ?? "";
            string terrain = formula?.Terrain ?? "";
            string effect = formula?.Effect ?? "";
            string school = formula?.School ?? schoolFallback ?? "";
            if (code == "FBL") return 2;
            if (code == "MTR") return 3;
            if (type == "fire" || terrain == "fire" || school.Contains("ember")) return 18;
            if (type == "cold" || terrain == "ice") return 19;
            if (type == "shock") return 20;
            if (type == "death" || type == "mind" || school.Contains("hex") || school.Contains("pact")) return 21;
            if (terrain == "tree") return 22;
            if (terrain == "stone") return 23;
            if (effect == "heal" || effect == "cure" || formula?.Status == "shield" || formula?.Status == "regen") return 0;
            if (effect == "summon") return 1;
            return -1;
        }

        private void DrawFormulaRuneCode(Rect rect, string code, FormulaDef formula)
        {
            Color runeColor = formula == null ? gold : FormulaColor(formula);
            DrawRect(rect, Hex("080b0d", 0.92f));
            DrawBorder(rect, runeColor, 1);
            string normalized = string.IsNullOrEmpty(code) ? "" : code.ToUpperInvariant();
            float gap = 3f;
            float slotW = (rect.width - gap * 4f) / 3f;
            for (int i = 0; i < 3; i++)
            {
                Rect slot = new Rect(rect.x + gap + i * (slotW + gap), rect.y + 4f, slotW, rect.height - 11f);
                bool filled = i < normalized.Length;
                DrawRect(slot, filled ? Color.Lerp(runeColor, Hex("101619"), 0.62f) : Hex("101619", 0.82f));
                DrawBorder(slot, filled ? runeColor : line, 1);
                string letter = filled ? normalized[i].ToString() : "_";
                GUI.Label(slot, letter, CenterStyle(14, filled ? cursorWhite : muted));
            }

            string path = FormulaPathLabel(formula);
            GUI.Label(new Rect(rect.x, rect.y + rect.height - 10f, rect.width, 10f), path, CenterStyle(8, runeColor));
        }

        private string FormulaPathLabel(FormulaDef formula)
        {
            if (formula == null) return "spell";
            if (FormulaArcsOverCover(formula)) return "arc";
            if (FormulaRequiresLineOfSight(formula)) return "sight";
            if (formula.Target == "ally") return "rite";
            return "open";
        }

        private Rect FormulaLabIconRegion(FormulaDef formula, string schoolFallback)
        {
            if (IsMagicUiAtlas())
            {
                return MagicUiAtlasCell(MagicUiIconIndex(formula, schoolFallback));
            }

            if (formulaLabArt != null && formulaLabArt.width == 1448 && formulaLabArt.height == 1086)
            {
                string effectName = formula?.Effect ?? "";
                string typeName = formula?.DamageType ?? "";
                string terrainName = formula?.Terrain ?? "";
                if (terrainName == "tree") return SpellIconCell(0, 0);
                if (terrainName == "stone") return SpellIconCell(1, 0);
                if (formula?.Status == "shield") return SpellIconCell(3, 0);
                if (effectName == "heal" || effectName == "cure" || formula?.Status == "regen") return SpellIconCell(2, 0);
                if (terrainName == "fire" && formula?.Arc == true) return SpellIconCell(2, 1);
                if (terrainName == "fire") return SpellIconCell(1, 1);
                if (terrainName == "ice" || typeName == "cold") return SpellIconCell(3, 1);
                if (typeName == "fire") return SpellIconCell(0, 1);
                if (typeName == "shock") return SpellIconCell(0, 2);
                if (terrainName == "web" || formula?.Status == "web") return SpellIconCell(1, 2);
                if (terrainName == "gas" || typeName == "poison") return SpellIconCell(2, 2);
                if (typeName == "death" || typeName == "mind" || formula?.Status == "hex" || formula?.Status == "sleep") return SpellIconCell(3, 2);
                return SpellIconCell(2, 0);
            }

            string school = formula?.School ?? schoolFallback ?? "";
            string effect = formula?.Effect ?? "";
            string type = formula?.DamageType ?? "";
            string terrain = formula?.Terrain ?? "";
            if (terrain == "tree") return new Rect(954, 42, 68, 68);
            if (terrain == "stone") return new Rect(1034, 42, 68, 68);
            if (type == "death" || formula?.Code == "RLM") return new Rect(870, 42, 68, 68);
            if (type == "cold" || terrain == "ice") return new Rect(622, 42, 68, 68);
            if (type == "shock") return new Rect(704, 42, 68, 68);
            if (type == "fire" || terrain == "fire" || school.Contains("ember")) return new Rect(540, 42, 68, 68);
            if (effect == "status" || terrain == "web" || terrain == "gas" || school.Contains("hex")) return new Rect(788, 42, 68, 68);
            if (school.Contains("mend")) return new Rect(458, 42, 68, 68);
            return new Rect(458, 42, 68, 68);
        }

        private bool IsMagicUiAtlas()
        {
            return formulaLabArt != null && Mathf.Abs(formulaLabArt.width - formulaLabArt.height) < 8 && formulaLabArt.width >= 1000;
        }

        private Rect MagicUiAtlasCell(int index)
        {
            return AtlasCell(formulaLabArt, index, 4, 4);
        }

        private int MagicUiIconIndex(FormulaDef formula, string schoolFallback)
        {
            string effectName = formula?.Effect ?? "";
            string typeName = formula?.DamageType ?? "";
            string terrainName = formula?.Terrain ?? "";
            string statusName = formula?.Status ?? "";
            string school = formula?.School ?? schoolFallback ?? "";
            if (terrainName == "tree") return 0;
            if (terrainName == "stone") return 1;
            if (effectName == "heal" || effectName == "cure" || statusName == "regen") return 2;
            if (statusName == "shield") return 3;
            if (terrainName == "fire" || typeName == "fire" || school.Contains("ember")) return 4;
            if (terrainName == "ice" || typeName == "cold") return 5;
            if (typeName == "shock") return 6;
            if (typeName == "death") return 7;
            if (terrainName == "web" || statusName == "web") return 8;
            if (terrainName == "gas" || typeName == "poison") return 9;
            if (typeName == "mind" || statusName == "hex" || statusName == "sleep" || school.Contains("hex")) return 10;
            if (effectName == "summon" || school.Contains("pact")) return 11;
            return 2;
        }

        private bool TryDrawMagicUiAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsMagicUiAtlas()) return false;
            return DrawTextureRegionTint(formulaLabArt, rect, MagicUiAtlasCell(index), tint);
        }

        private int TerrainMagicIconIndex(string kind)
        {
            switch (kind)
            {
                case "tree": return 0;
                case "stone": return 1;
                case "fire": return 4;
                case "ice": return 5;
                case "web": return 8;
                case "gas": return 9;
                default: return -1;
            }
        }

        private Rect SpellIconCell(int col, int row)
        {
            const float size = 362f;
            return new Rect(col * size, row * size, size, size);
        }

        private void DrawFormulaChips(Rect rect, CombatUnit active, bool playerTurn)
        {
            if (!playerTurn || active == null || string.IsNullOrEmpty(active.Spell) || !state.Combat.ActionAvailable)
            {
                GUI.Label(rect, FormulaCodexLine(active), CenterLeftStyle(12, muted));
                return;
            }

            FormulaDef[] allKnown = KnownFormulasFor(active).ToArray();
            if (allKnown.Length == 0)
            {
                GUI.Label(rect, "No spells available.", CenterLeftStyle(12, muted));
                return;
            }

            float gap = 6f;
            int rows = rect.height >= 300f ? 4 : rect.height >= 190f ? 3 : rect.height >= 68f ? 2 : 1;
            int columns = rect.width >= 1020f ? 5 : rect.width >= 760f ? 4 : 3;
            int slots = Mathf.Max(3, rows * columns);
            bool paged = allKnown.Length > slots;
            float moreW = paged ? 76f : 0f;
            int pageCount = Mathf.Max(1, Mathf.CeilToInt(allKnown.Length / (float)slots));
            formulaChipPage = ((formulaChipPage % pageCount) + pageCount) % pageCount;
            FormulaDef[] known = allKnown.Skip(formulaChipPage * slots).Take(slots).ToArray();
            float cardAreaW = rect.width - moreW - (paged ? gap : 0f);
            float cardW = (cardAreaW - gap * (columns - 1)) / columns;
            float cardH = Mathf.Max(26f, (rect.height - gap * (rows - 1)) / rows);
            for (int i = 0; i < known.Length; i++)
            {
                FormulaDef formula = known[i];
                int row = i / columns;
                int col = i % columns;
                Rect chip = new Rect(rect.x + col * (cardW + gap), rect.y + row * (cardH + gap), cardW, cardH);
                bool selected = pendingFormulaCode == formula.Code;
                DrawRect(chip, selected ? Hex("352316") : Hex("101619"));
                TryDrawCombatUiAtlasIcon(Pad(chip, Mathf.Min(10f, chip.height * 0.10f)), 7, Color.white.WithAlpha(selected ? 0.18f : 0.10f));
                DrawBorder(chip, selected ? gold : FormulaColor(formula, 0.70f), 1);
                if (selected) DrawBorder(Pad(chip, 3f), FormulaColor(formula, 0.78f), 1);
                DrawFormulaLabIcon(new Rect(chip.x + 5f, chip.y + 5f, Mathf.Min(28f, chip.height - 8f), Mathf.Min(28f, chip.height - 8f)), formula, active.Spell);
                GUI.enabled = ActionEnabled(ActionMode.Cast, active);
                if (GUI.Button(chip, "", GUIStyle.none))
                {
                    PrepareFormulaCode(active, formula.Code);
                }
                GUI.enabled = true;
                float textX = chip.x + Mathf.Min(38f, chip.height + 6f);
                GUI.Label(new Rect(textX, chip.y + 4f, chip.width - (textX - chip.x) - 8f, 15f), FitText(formula.Name, chip.width - (textX - chip.x) - 8f, CenterLeftStyle(11, selected ? cursorWhite : ink)), CenterLeftStyle(11, selected ? cursorWhite : ink));
                GUI.Label(new Rect(textX, chip.y + 20f, chip.width - (textX - chip.x) - 8f, 13f), FitText(formula.Hint, chip.width - (textX - chip.x) - 8f, CenterLeftStyle(9, muted)), CenterLeftStyle(9, muted));
                string rule = $"{FormulaTierLabel(formula)} / {EffectiveFormulaMana(formula, active)} MP / R{EffectiveFormulaRange(formula, active)} / {FormulaPathLabel(formula)}";
                GUI.Label(new Rect(textX, chip.yMax - 15f, chip.width - (textX - chip.x) - 8f, 12f), rule, CenterLeftStyle(9, FormulaTierColor(formula)));
            }

            if (paged)
            {
                Rect more = new Rect(rect.xMax - moreW, rect.y, moreW, rect.height);
                DrawRect(more, Hex("101619"));
                DrawBorder(more, gold, 1);
                if (GUI.Button(more, $"More\n{formulaChipPage + 1}/{pageCount}", smallButtonStyle))
                {
                    formulaChipPage = (formulaChipPage + 1) % pageCount;
                    PlaySfx("ui", 0.5f);
                }
            }
        }

        private bool ActionEnabled(ActionMode mode, CombatUnit active)
        {
            if (active == null) return false;
            if (active.Stunned > 0 || active.Sleeping > 0) return mode == ActionMode.Wait;
            if (mode == ActionMode.Move) return state.Combat.MovePoints > 0 && active.Webbed <= 0;
            if (mode == ActionMode.Attack) return state.Combat.ActionAvailable;
            if (mode == ActionMode.Cast) return state.Combat.ActionAvailable && !string.IsNullOrEmpty(active.Spell);
            if (mode == ActionMode.Elixir) return !active.Summoned && state.Combat.ActionAvailable && state.Elixirs > 0;
            if (mode == ActionMode.Guard) return state.Combat.ActionAvailable;
            return true;
        }

        private void SelectOrRunAction(ActionMode mode, CombatUnit active)
        {
            selectedAction = mode;
            if (mode == ActionMode.Cast) showSpellbook = ActionEnabled(ActionMode.Cast, active);
            else
            {
                showSpellbook = false;
                ClearFormulaEntry();
            }
            if (state?.Combat != null) state.Combat.Phase = mode == ActionMode.Move || mode == ActionMode.Attack || mode == ActionMode.Cast ? CombatPhase.ChooseTarget : CombatPhase.ChooseAction;
            if (mode == ActionMode.Guard)
            {
                active.Guarding = true;
                active.GuardBonus = (state.Combat.MovePoints >= UnitMoveAllowance(active) && !state.Combat.Moved ? 4 : 2) + GearGuardBonus(active);
                state.Combat.ActionAvailable = false;
                state.Combat.Acted = true;
                state.Combat.MovePoints = 0;
                state.Combat.Moved = true;
                state.Combat.Phase = CombatPhase.Resolving;
                ImproveSkill(active, "guard", 1);
                PushLog($"{active.Name} guards the line{(active.GuardBonus >= 4 ? " from a braced stance" : "")}.", Tone.Normal);
                PlaySfx("guard", 0.82f);
                NextTurn();
            }
            else if (mode == ActionMode.Elixir)
            {
                UseElixir();
            }
            else if (mode == ActionMode.Wait)
            {
                state.Combat.MovePoints = 0;
                state.Combat.ActionAvailable = false;
                state.Combat.Moved = true;
                state.Combat.Acted = true;
                state.Combat.Phase = CombatPhase.Resolving;
                PushLog($"{active.Name} ends the turn.", Tone.Normal);
                PlaySfx("ui", 0.55f);
                NextTurn();
            }
        }

        private void HandleExploreKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) TryMoveExplore(0, -1);
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) TryMoveExplore(0, 1);
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) TryMoveExplore(-1, 0);
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) TryMoveExplore(1, 0);
            if (Input.GetKeyDown(KeyCode.F)) ToggleArmory(0);
            if (Input.GetKeyDown(KeyCode.G)) ToggleArmory(2);
            if (Input.GetKeyDown(KeyCode.H)) UseElixir();
            if (Input.GetKeyDown(KeyCode.R)) Camp();
            if (Input.GetKeyDown(KeyCode.T) && CanDescend()) Descend();
        }

        private void TryMoveExplore(int dx, int dy)
        {
            int nx = state.PlayerX + dx;
            int ny = state.PlayerY + dy;
            if (TileAt(state.Map, nx, ny) != 1)
            {
                PushLog("Stone blocks the way.", Tone.Warn);
                PlaySfx("blocked");
                return;
            }
            int oldX = state.PlayerX;
            int oldY = state.PlayerY;
            string beforeRegion = ExploreRegionName(oldX, oldY);
            state.PlayerX = nx;
            state.PlayerY = ny;
            if (!state.ReducedMotion)
            {
                tweens.Add(new Tween("party", new Vector2(oldX, oldY), new Vector2(nx, ny), Time.time, 0.16f, TweenKind.Move));
            }
            PlaySfx("move", 0.65f);
            string afterRegion = ExploreRegionName(state.PlayerX, state.PlayerY);
            if (afterRegion != beforeRegion && afterRegion != lastExploreRegion)
            {
                lastExploreRegion = afterRegion;
                WorldZone zone = ZoneAt(state.PlayerX, state.PlayerY);
                PushLog($"The company reaches {afterRegion}: {ZoneDangerText(zone)}.", Tone.Normal);
                DiscoverCurrentZone(false);
                ShowBanner(afterRegion);
            }
            ResolveExploreTile();
        }

        private void DiscoverCurrentZone(bool force)
        {
            if (state?.Map == null) return;
            if (state.DiscoveredZones == null) state.DiscoveredZones = new List<string>();
            WorldZone zone = ZoneAt(state.PlayerX, state.PlayerY);
            if (zone == null || string.IsNullOrEmpty(zone.Id)) return;
            string key = $"{state.Depth}:{zone.Id}";
            if (!force && state.DiscoveredZones.Contains(key)) return;
            if (!state.DiscoveredZones.Contains(key)) state.DiscoveredZones.Add(key);
            PushLog($"{zone.Name}: {zone.Story}", zone.Danger >= 3 ? Tone.Warn : Tone.Normal);
        }

        private void ResolveExploreTile()
        {
            MapObject obj = ObjectAt(state.Map, state.PlayerX, state.PlayerY);
            if (obj == null)
            {
                WorldZone zone = ZoneAt(state.PlayerX, state.PlayerY);
                if (rng.NextDouble() < 0.015 + state.Depth * 0.003 + Mathf.Clamp(zone.Danger, 0, 4) * 0.004) StartCombat("patrol");
                return;
            }

            if (obj.Type == ObjectType.Cache)
            {
                int foundGold = rng.Next(12, 29) + state.Depth * 4;
                InventoryItem item = MakeItem();
                int foundElixirs = rng.NextDouble() < 0.5 ? 1 : 0;
                int foundSupplies = rng.NextDouble() < 0.55 ? 1 : 0;
                state.Gold += foundGold;
                state.Elixirs += foundElixirs;
                state.Supplies += foundSupplies;
                state.Inventory.Add(item);
                string equipNote = AutoEquipItem(item);
                ShowLootPanel(item, foundGold, foundSupplies, foundElixirs, equipNote);
                RemoveObject(obj);
                PushLog($"A sealed cache yields {foundGold} gold{CacheSupplyLine(foundSupplies, foundElixirs)} and {item.DisplayName}. {equipNote}", Tone.Good);
                ShowBanner("Cache opened");
                PlaySfx("cache");
                AddBurst(state.PlayerX, state.PlayerY, gold);
            }
            else if (obj.Type == ObjectType.Shrine)
            {
                foreach (PartyMember member in state.Party.Where(p => p.Hp > 0))
                {
                    member.Hp = Mathf.Min(member.MaxHp, member.Hp + 9 + state.Depth * 2);
                    member.Mana = Mathf.Min(member.MaxMana, member.Mana + 5);
                }
                RemoveObject(obj);
                PushLog("An old shrine steadies the company.", Tone.Good);
                ShowBanner("Shrine restored");
                PlaySfx("shrine");
                AddBurst(state.PlayerX, state.PlayerY, teal);
            }
            else if (obj.Type == ObjectType.Encounter)
            {
                RemoveObject(obj);
                StartCombat("guard");
            }
            else if (obj.Type == ObjectType.Stairs)
            {
                PushLog("A stairway sinks into a colder dark.", Tone.Normal);
                ShowBanner("Stairs found");
                PlaySfx("ui");
            }
            else if (obj.Type == ObjectType.Town)
            {
                foreach (PartyMember member in state.Party.Where(p => p.Hp > 0))
                {
                    member.Hp = Mathf.Min(member.MaxHp, member.Hp + 16);
                    member.Mana = Mathf.Min(member.MaxMana, member.Mana + 10);
                }
                PushLog($"{HomeTownName} opens its lamps to the company.", Tone.Good);
                PushLog(state.ActiveStory, Tone.Normal);
                ShowBanner(HomeTownName);
                PlaySfx("shrine", 0.75f);
            }
        }

        private void Camp()
        {
            if (state.Supplies <= 0)
            {
                PushLog("The packs hold no supplies.", Tone.Warn);
                return;
            }
            state.Supplies--;
            foreach (PartyMember member in state.Party.Where(p => p.Hp > 0))
            {
                member.Hp = Mathf.Min(member.MaxHp, member.Hp + 13);
                member.Mana = Mathf.Min(member.MaxMana, member.Mana + 8);
            }
            PushLog("A guarded campfire buys a little strength.", Tone.Good);
            PlaySfx("heal", 0.72f);
        }

        private bool CanDescend()
        {
            MapObject obj = ObjectAt(state.Map, state.PlayerX, state.PlayerY);
            return obj != null && obj.Type == ObjectType.Stairs;
        }

        private void Descend()
        {
            if (!CanDescend())
            {
                PushLog("No stairway lies underfoot.", Tone.Warn);
                return;
            }
            state.Depth++;
            state.StoryChapter = Mathf.Max(state.StoryChapter + 1, state.Depth);
            state.ActiveStory = StoryObjectiveForDepth(state.Depth);
            state.Supplies += 2;
            state.Map = GenerateMap(state.Depth, state.Seed);
            EnsureWorldLandmarks();
            state.PlayerX = state.Map.StartX;
            state.PlayerY = state.Map.StartY;
            lastExploreRegion = ExploreRegionName(state.PlayerX, state.PlayerY);
            DiscoverCurrentZone(true);
            PushLog($"The company descends to depth {state.Depth}. {state.ActiveStory}", Tone.Good);
            ShowBanner(StoryChapterTitle());
            PlaySfx("encounter", 0.8f);
            if (state.Depth >= FinalBossDepth)
            {
                PushLog("The final gate answers. A meteor-crowned will waits beyond it.", Tone.Warn);
                StartCombat("boss");
            }
        }

        private InventoryItem MakeItem()
        {
            bool weapon = rng.NextDouble() < 0.62;
            return MakeRoleItem("", weapon, false);
        }

        private InventoryItem MakeRoleItem(string role, bool weapon, bool starter)
        {
            string[] weaponForms = RoleWeaponForms(role, starter);
            string[] armorForms = RoleArmorForms(role, starter);
            string[] materials = starter
                ? new[] { "iron", "ashwood", "hidebound", "silvered", "moonstone" }
                : new[] { "iron", "fine steel", "ashwood", "silvered", "obsidian", "blackglass", "ironwood", "crystalline", "mithril", "adamantine", "stormglass", "moonstone", "bone", "silk" };
            string[] qualities = starter
                ? new[] { "plain", "serviceable", "balanced", "well-made" }
                : new[] { "crude", "serviceable", "fine", "balanced", "masterwork", "dwarven", "elven", "weightless", "vicious", "holy", "vampiric", "anti-magic" };
            string[] traits = starter
                ? new[] { "guarding", "haste", "focus", "warding", "keen" }
                : new[] { "flame", "frost", "storm", "venom", "terror", "mercy", "night", "warding", "haste", "echoes", "thorns", "silence", "bleeding", "stunning", "guarding", "focus", "death" };
            string form = weapon ? weaponForms[rng.Next(weaponForms.Length)] : armorForms[rng.Next(armorForms.Length)];
            string material = materials[rng.Next(materials.Length)];
            string quality = qualities[rng.Next(qualities.Length)];
            string trait = traits[rng.Next(traits.Length)];
            int bonus = starter ? Mathf.Clamp(RollItemBonus(quality, material), 0, 2) : RollItemBonus(quality, material);
            string damageType = weapon ? ItemDamageType(trait, material) : "";
            string rarity = ItemRarity(quality, material, trait, bonus, starter);
            Stats statBonuses = RollItemStatBonuses(weapon, form, trait, material, starter);
            Vector2Int damage = weapon ? ItemDamageRange(form, bonus, trait, material) : new Vector2Int(0, 0);
            int speed = weapon ? ItemAttackSpeed(form, quality, material, trait) : 0;
            string plus = bonus > 0 ? $"+{bonus} " : bonus < 0 ? $"{bonus} " : "";
            string display = starter && rng.NextDouble() < 0.48
                ? $"{plus}{quality} {form}"
                : $"{plus}{quality} {material} {form} of {trait}";
            return new InventoryItem
            {
                Mark = quality,
                Material = material,
                Form = form,
                Trait = trait,
                Slot = weapon ? "weapon" : "armor",
                Bonus = bonus,
                StrengthBonus = statBonuses.Strength,
                IntelligenceBonus = statBonuses.Intelligence,
                AgilityBonus = statBonuses.Dexterity,
                HealthBonus = statBonuses.Health,
                DamageMin = damage.x,
                DamageMax = damage.y,
                AttackSpeed = speed,
                Rarity = rarity,
                DamageType = damageType,
                DisplayName = display
            };
        }

        private string ItemRarity(string quality, string material, string trait, int bonus, bool starter)
        {
            if (starter) return "starter";
            int score = bonus;
            if (quality == "masterwork" || quality == "holy" || quality == "vampiric" || quality == "anti-magic") score += 2;
            if (material == "mithril" || material == "adamantine" || material == "stormglass" || material == "blackglass") score++;
            if (trait == "death" || trait == "vampiric" || trait == "silence") score++;
            if (score >= 6) return "relic";
            if (score >= 4) return "rare";
            if (score >= 2) return "uncommon";
            return "common";
        }

        private Stats RollItemStatBonuses(bool weapon, string form, string trait, string material, bool starter)
        {
            int str = 0;
            int intel = 0;
            int agi = 0;
            int hea = 0;
            string text = $"{form} {trait} {material}".ToLowerInvariant();
            if (text.Contains("war hammer") || text.Contains("broadsword") || text.Contains("adamantine") || text.Contains("guarding")) str++;
            if (text.Contains("focus") || text.Contains("orb") || text.Contains("scepter") || text.Contains("moonstone") || text.Contains("blackglass")) intel++;
            if (text.Contains("epee") || text.Contains("sabre") || text.Contains("bow") || text.Contains("silk") || text.Contains("haste") || text.Contains("weightless")) agi++;
            if (text.Contains("plate") || text.Contains("tower") || text.Contains("ironwood") || text.Contains("warding")) hea++;
            if (!starter && rng.NextDouble() < 0.22)
            {
                int pick = rng.Next(4);
                if (pick == 0) str++;
                else if (pick == 1) intel++;
                else if (pick == 2) agi++;
                else hea++;
            }
            if (starter)
            {
                str = Mathf.Min(str, 1);
                intel = Mathf.Min(intel, 1);
                agi = Mathf.Min(agi, 1);
                hea = Mathf.Min(hea, 1);
            }
            return new Stats(str, intel, agi, hea);
        }

        private Vector2Int ItemDamageRange(string form, int bonus, string trait, string material)
        {
            string text = $"{form} {trait} {material}".ToLowerInvariant();
            int min = 2;
            int max = 5;
            if (text.Contains("epee") || text.Contains("sabre") || text.Contains("knife")) { min = 1; max = 5; }
            else if (text.Contains("longbow") || text.Contains("crossbow")) { min = 2; max = 6; }
            else if (text.Contains("sling") || text.Contains("darts")) { min = 1; max = 4; }
            else if (text.Contains("spear") || text.Contains("pike") || text.Contains("glaive") || text.Contains("halberd")) { min = 2; max = 7; }
            else if (text.Contains("war hammer") || text.Contains("war flail")) { min = 3; max = 8; }
            else if (text.Contains("focus") || text.Contains("orb") || text.Contains("scepter") || text.Contains("staff")) { min = 1; max = 5; }
            min += Mathf.Max(0, bonus / 2);
            max += Mathf.Max(0, bonus);
            if (text.Contains("vicious") || text.Contains("death") || text.Contains("vampiric")) max += 2;
            return new Vector2Int(Mathf.Max(1, min), Mathf.Max(min + 1, max));
        }

        private int ItemAttackSpeed(string form, string quality, string material, string trait)
        {
            string text = $"{form} {quality} {material} {trait}".ToLowerInvariant();
            int speed = 7;
            if (text.Contains("epee") || text.Contains("sabre") || text.Contains("knife") || text.Contains("darts")) speed = 11;
            else if (text.Contains("bow") || text.Contains("sling")) speed = 9;
            else if (text.Contains("crossbow") || text.Contains("war hammer") || text.Contains("tower")) speed = 5;
            else if (text.Contains("spear") || text.Contains("pike") || text.Contains("halberd")) speed = 7;
            else if (text.Contains("focus") || text.Contains("orb") || text.Contains("scepter") || text.Contains("staff")) speed = 8;
            if (text.Contains("balanced") || text.Contains("elven") || text.Contains("haste") || text.Contains("weightless") || text.Contains("mithril")) speed += 2;
            if (text.Contains("crude") || text.Contains("adamantine")) speed -= 1;
            return Mathf.Clamp(speed, 3, 16);
        }

        private string[] RoleWeaponForms(string role, bool starter)
        {
            if (role == "bow") return new[] { "longbow", "crossbow", "sling", "throwing darts" };
            if (role == "pike") return new[] { "long spear", "pike", "glaive", "halberd" };
            if (role == "knife") return new[] { "epee", "sabre", "ritual knife", "throwing knives" };
            if (role == "mender") return new[] { "prayer focus", "ash staff", "scepter", "ritual bell" };
            if (role == "ember") return new[] { "ember focus", "ash staff", "stormglass orb", "scepter" };
            if (role == "hex") return new[] { "bone focus", "ritual knife", "blackglass orb", "ash staff" };
            if (role == "shield" || role == "ward") return new[] { "broadsword", "mace", "war hammer", "war flail", "arming sword" };
            return starter ? new[] { "short sword", "staff", "knife" } : new[] { "epee", "sabre", "broadsword", "mace", "war flail", "long spear", "halberd", "longbow", "crossbow", "throwing knives", "ash staff", "ritual knife", "scepter", "orb" };
        }

        private string[] RoleArmorForms(string role, bool starter)
        {
            if (role == "shield") return new[] { "chain hauberk", "plate cuirass", "kite shield", "scale shirt" };
            if (role == "ward") return new[] { "tower shield", "warding robe", "mail and tower shield", "plate cuirass" };
            if (role == "pike") return new[] { "scale shirt", "chain hauberk", "leather jack", "bone greaves" };
            if (role == "bow") return new[] { "scout leathers", "leather jack", "silk mantle", "shadow cloak" };
            if (role == "knife") return new[] { "dark leathers", "silk mantle", "buckler", "shadow cloak" };
            if (role == "mender") return new[] { "warding robe", "silk mantle", "prayer mantle", "moonstone circlet" };
            if (role == "ember" || role == "hex") return new[] { "spell robe", "silk mantle", "warding robe", "moonstone circlet" };
            return starter ? new[] { "leather jack", "robe", "buckler" } : new[] { "padded jack", "leather jack", "scout leathers", "dark leathers", "scale shirt", "chain hauberk", "plate cuirass", "buckler", "kite shield", "tower shield", "warding robe", "spell robe", "silk mantle", "bone greaves", "shadow cloak", "iron helm" };
        }

        private int RollItemBonus(string quality, string material)
        {
            int bonus = rng.NextDouble() < 0.18 ? -1 : rng.Next(0, 3);
            if (quality == "fine" || quality == "balanced" || quality == "dwarven" || quality == "elven") bonus++;
            if (quality == "masterwork" || quality == "holy" || quality == "vampiric" || quality == "anti-magic") bonus += 2;
            if (material == "mithril" || material == "adamantine" || material == "crystalline") bonus++;
            return Mathf.Clamp(bonus, -1, 5);
        }

        private string ItemDamageType(string trait, string material)
        {
            if (trait == "flame") return "fire";
            if (trait == "frost") return "cold";
            if (trait == "storm") return "shock";
            if (trait == "venom") return "poison";
            if (trait == "terror" || trait == "night" || trait == "death") return "death";
            if (material == "silvered" || trait == "holy") return "light";
            return "physical";
        }

        private string AutoEquipItem(InventoryItem item)
        {
            if (item == null || state?.Party == null) return "";
            IEnumerable<PartyMember> candidates = state.Party.Where(p => p.Hp > 0);
            if (item.Slot == "weapon")
            {
                candidates = candidates.OrderByDescending(p => WeaponRoleFit(item, p)).ThenBy(p => p.WeaponBonus);
                PartyMember target = candidates.FirstOrDefault(p => WeaponRoleFit(item, p) > 0 && item.Bonus >= p.WeaponBonus);
                if (target == null) target = state.Party.Where(p => p.Hp > 0).OrderBy(p => p.WeaponBonus).FirstOrDefault();
                if (target == null) return "";
                if (item.Bonus < target.WeaponBonus) return "No one claims it yet.";
                string old = string.IsNullOrEmpty(target.WeaponName) ? "old weapon" : target.WeaponName;
                target.WeaponName = item.DisplayName;
                target.WeaponBonus = item.Bonus;
                target.WeaponDamageType = string.IsNullOrEmpty(item.DamageType) ? "physical" : item.DamageType;
                target.WeaponDamageMin = Mathf.Max(1, item.DamageMin);
                target.WeaponDamageMax = Mathf.Max(target.WeaponDamageMin + 1, item.DamageMax);
                target.WeaponAttackSpeed = Mathf.Max(1, item.AttackSpeed);
                ApplyGearStatBonuses(target, item, true);
                target.Range = WeaponRange(item, target);
                RecalculateMember(target);
                return $"{target.Name} equips it over {old}. {ItemBehaviorLine(item, target)}";
            }
            else
            {
                PartyMember target = candidates.OrderBy(p => p.ArmorBonus + ArmorRolePenalty(item, p)).FirstOrDefault();
                if (target == null) return "";
                if (item.Bonus < target.ArmorBonus) return "It goes into the pack.";
                string old = string.IsNullOrEmpty(target.ArmorName) ? "plain armor" : target.ArmorName;
                target.ArmorName = item.DisplayName;
                target.ArmorBonus = ArmorDefenseBonus(item);
                ApplyGearStatBonuses(target, item, false);
                RecalculateMember(target);
                return $"{target.Name} wears it over {old}. {ItemBehaviorLine(item, target)}";
            }
        }

        private void ApplyGearStatBonuses(PartyMember member, InventoryItem item, bool weapon)
        {
            if (member == null || item == null) return;
            if (weapon)
            {
                member.WeaponStrengthBonus = item.StrengthBonus;
                member.WeaponIntelligenceBonus = item.IntelligenceBonus;
                member.WeaponAgilityBonus = item.AgilityBonus;
                member.WeaponHealthBonus = item.HealthBonus;
            }
            else
            {
                member.ArmorStrengthBonus = item.StrengthBonus;
                member.ArmorIntelligenceBonus = item.IntelligenceBonus;
                member.ArmorAgilityBonus = item.AgilityBonus;
                member.ArmorHealthBonus = item.HealthBonus;
            }
            member.GearStrength = member.WeaponStrengthBonus + member.ArmorStrengthBonus;
            member.GearIntelligence = member.WeaponIntelligenceBonus + member.ArmorIntelligenceBonus;
            member.GearAgility = member.WeaponAgilityBonus + member.ArmorAgilityBonus;
            member.GearHealth = member.WeaponHealthBonus + member.ArmorHealthBonus;
        }

        private int WeaponRoleFit(InventoryItem item, PartyMember member)
        {
            string form = item.Form ?? "";
            if (member.Role == "bow" && (form.Contains("bow") || form.Contains("crossbow"))) return 6;
            if (member.Role == "pike" && (form.Contains("spear") || form.Contains("halberd"))) return 6;
            if (member.Role == "knife" && (form.Contains("knife") || form.Contains("epee") || form.Contains("sabre"))) return 6;
            if ((member.Role == "ember" || member.Role == "hex" || member.Role == "mender") && (form.Contains("staff") || form.Contains("ritual"))) return 6;
            if (member.Role == "shield" || member.Role == "ward") return form.Contains("mace") || form.Contains("sword") || form.Contains("flail") ? 5 : 2;
            return 2;
        }

        private int WeaponRange(InventoryItem item, PartyMember member)
        {
            string form = item.Form ?? "";
            if (form.Contains("longbow") || form.Contains("crossbow")) return 4;
            if (form.Contains("throwing")) return 3;
            if (form.Contains("sling") || form.Contains("darts")) return 3;
            if (form.Contains("spear") || form.Contains("pike") || form.Contains("glaive") || form.Contains("halberd")) return 2;
            if ((form.Contains("focus") || form.Contains("orb") || form.Contains("scepter") || form.Contains("staff")) && (member.Role == "ember" || member.Role == "hex" || member.Role == "mender")) return 4;
            return member.Role == "bow" ? 4 : member.Role == "ember" || member.Role == "hex" ? 3 : 1;
        }

        private int ArmorRolePenalty(InventoryItem item, PartyMember member)
        {
            string form = item.Form ?? "";
            if ((member.Role == "ember" || member.Role == "hex" || member.Role == "mender") && (form.Contains("plate") || form.Contains("chain") || form.Contains("tower"))) return 3;
            if ((member.Role == "shield" || member.Role == "ward") && (form.Contains("plate") || form.Contains("shield"))) return -2;
            return 0;
        }

        private int ArmorDefenseBonus(InventoryItem item)
        {
            if (item == null) return 0;
            int bonus = Mathf.Max(-1, item.Bonus);
            string form = (item.Form ?? "").ToLowerInvariant();
            if (form.Contains("plate") || form.Contains("tower")) bonus += 2;
            else if (form.Contains("chain") || form.Contains("mail") || form.Contains("scale") || form.Contains("kite")) bonus += 1;
            if (form.Contains("robe") || form.Contains("mantle") || form.Contains("cloak")) bonus = Mathf.Max(0, bonus);
            return Mathf.Clamp(bonus, -1, 7);
        }

        private string ItemBehaviorLine(InventoryItem item, PartyMember target)
        {
            if (item == null) return "";
            List<string> notes = new List<string>();
            string text = ((item.DisplayName ?? "") + " " + (item.Form ?? "") + " " + (item.Trait ?? "")).ToLowerInvariant();
            if (item.Slot == "weapon")
            {
                int range = WeaponRange(item, target);
                if (range >= 4) notes.Add("long range");
                else if (range == 3) notes.Add("ranged");
                else if (range == 2) notes.Add("reach");
                string status = GearOnHitStatus(text);
                if (!string.IsNullOrEmpty(status)) notes.Add(status + " chance");
                if (!string.IsNullOrEmpty(item.DamageType) && item.DamageType != "physical") notes.Add(item.DamageType + " affinity");
                if (text.Contains("focus") || text.Contains("orb") || text.Contains("scepter")) notes.Add("spell focus");
            }
            else
            {
                if (text.Contains("plate") || text.Contains("tower") || text.Contains("chain") || text.Contains("mail")) notes.Add("heavy guard");
                if (text.Contains("leather") || text.Contains("cloak") || text.Contains("mantle")) notes.Add("light movement");
                if (text.Contains("warding") || text.Contains("anti-magic") || text.Contains("robe")) notes.Add("warding");
                if (text.Contains("thorns")) notes.Add("thorn guard");
            }
            return notes.Count == 0 ? "It is a clean upgrade." : "Why: " + string.Join(", ", notes) + ".";
        }

        private void StartCombat(string style)
        {
            state.Mode = GameMode.Combat;
            betaLabMode = style == "lab";
            CombatState combat = new CombatState
            {
                Round = 1,
                ActiveId = "",
                Moved = false,
                Acted = false,
                MovePoints = 0,
                ActionAvailable = false,
                Phase = CombatPhase.ChooseAction,
                Units = new List<CombatUnit>(),
                Obstacles = new List<Point>()
            };
            List<PartyMember> living = state.Party.Where(p => p.Hp > 0).ToList();
            for (int i = 0; i < living.Count; i++)
            {
                PartyMember p = living[i];
                combat.Units.Add(new CombatUnit
                {
                    Id = p.Id,
                    PartyIndex = state.Party.IndexOf(p),
                    Side = UnitSide.Party,
                    Name = p.Name,
                    Role = p.Role,
                    Race = p.Race,
                    ClassKey = p.ClassKey,
                    Origin = p.Origin,
                    Sigil = p.Sigil,
                    X = i < 4 ? 1 : 2,
                    Y = i < 4 ? i * 2 : (i - 4) * 2 + 1,
                    Hp = p.Hp,
                    MaxHp = p.MaxHp,
                    Mana = p.Mana,
                    MaxMana = p.MaxMana,
                    Movement = p.Movement,
                    Power = p.Power,
                    Defense = p.Defense,
                    Agility = p.Agility,
                    Range = p.Range,
                    AttackSpeed = p.AttackSpeed,
                    DamageMin = p.DamageMin,
                    DamageMax = p.DamageMax,
                    Spell = p.Spell,
                    Skills = p.Skills.Clone(),
                    Color = MemberColor(p).ToHex(),
                    DamageType = string.IsNullOrEmpty(p.WeaponDamageType) ? "physical" : p.WeaponDamageType,
                    WeaponName = p.WeaponName,
                    WeaponBonus = p.WeaponBonus,
                    ArmorName = p.ArmorName,
                    ArmorBonus = p.ArmorBonus
                });
            }
            string[] kinds = style == "boss"
                ? new[] { "meteorlich", "ritualheart", "drowpriest", "koboldwizard", "lesserdemon" }
                : style == "lab"
                ? new[] { "koboldshaman", "koboldwizard", "koboldshield", "koboldslinger", "bonepriest", "glassmage", "ratmage", "ratcleric", "drowmage", "drowpriest", "cinderling", "lesserdemon" }
                : EnemyPoolForDepth(state.Depth, ZoneAt(state.PlayerX, state.PlayerY)?.Id);
            int count = style == "boss" ? kinds.Length : style == "lab" ? 7 : Mathf.Clamp(3 + state.Depth / 2 + (style == "patrol" ? 0 : 1), 3, 7);
            for (int i = 0; i < count; i++)
            {
                string kind = style == "lab" || style == "boss" ? kinds[i % kinds.Length] : kinds[rng.Next(kinds.Length)];
                combat.Units.Add(MakeEnemy(kind, i));
            }
            if (style == "lab")
            {
                combat.Obstacles.Add(new Point(5, 1, "tree"));
                combat.Obstacles.Add(new Point(6, 2, "stone"));
                combat.Obstacles.Add(new Point(7, 3, "web", 9));
                combat.Obstacles.Add(new Point(8, 4, "gas", 9));
                combat.Obstacles.Add(new Point(6, 5, "fire", 7));
                combat.Obstacles.Add(new Point(9, 5, "ice", 9));
            }
            for (int i = 0; i < (style == "lab" ? 3 : 7); i++)
            {
                Point p = new Point(rng.Next(4, 9), rng.Next(1, CombatH - 1));
                if (!combat.Obstacles.Any(o => o.X == p.X && o.Y == p.Y)) combat.Obstacles.Add(p);
            }
            state.Combat = combat;
            selectedAction = ActionMode.Attack;
            PushLog(style == "boss" ? "The final gate breaks open. Meteor fire gathers above the hall." : style == "lab" ? "Beta combat lab opens: enemy casters are ready." : "Steel answers in the dark.", Tone.Warn);
            ShowBanner(style == "boss" ? "Final Gate" : style == "lab" ? BuildStage : "Encounter");
            PlaySfx("encounter");
            NextTurn();
        }

        private string[] EnemyPoolForDepth(int depth, string zoneId = "")
        {
            zoneId = zoneId ?? "";
            if (depth <= 1 && zoneId == "salt-cisterns") return new[] { "sewerrat", "sewerrat", "giantrat", "ratfolk", "ratcutthroat", "ratcleric", "spore", "koboldraider" };
            if (zoneId == "dusk-market") return new[] { "koboldraider", "koboldslinger", "koboldshield", "koboldshaman", "ratcutthroat", "drowscout", "drowcrossbow", "sentry", "mirearcher" };
            if (zoneId == "green-shrine-road") return new[] { "sewerrat", "giantrat", "ratcleric", "spore", "shade", "koboldshaman", "bonepriest", "drowpriest" };
            if (zoneId == "old-quarry") return new[] { "koboldshield", "sentry", "husk", "reaver", "koboldraider", "ratbrute", "thornbeast" };
            if (zoneId == "glass-warrens") return new[] { "adept", "glassmage", "drowmage", "drowcrossbow", "shade", "koboldwizard", "sentry", "cinderling" };
            if (zoneId == "ash-fen") return new[] { "spore", "mirearcher", "ratmage", "shade", "bonepriest", "koboldshaman", "giantrat" };
            if (zoneId == "red-gate") return new[] { "koboldwizard", "bonepriest", "drowblade", "drowpriest", "drowmage", "cinderling", "lesserdemon", "gloamknight", "reaver", "shade" };
            if (depth <= 1) return new[] { "sewerrat", "sewerrat", "giantrat", "ratfolk", "ratcutthroat", "koboldraider", "koboldslinger", "sentry" };
            if (depth == 2) return new[] { "koboldraider", "koboldslinger", "koboldshield", "koboldshaman", "koboldshaman", "ratmage", "ratcleric", "drowscout", "sentry", "adept", "husk", "reaver", "spore", "shade", "mirearcher", "bonepriest" };
            return new[] { "koboldraider", "koboldslinger", "koboldshield", "koboldshaman", "koboldwizard", "koboldwizard", "ratbrute", "drowscout", "drowblade", "drowcrossbow", "drowmage", "drowpriest", "lesserdemon", "sentry", "adept", "husk", "reaver", "spore", "shade", "glassmage", "thornbeast", "mirearcher", "bonepriest", "cinderling", "gloamknight" };
        }

        private CombatUnit MakeEnemy(string kind, int index)
        {
            EnemyTemplate t = EnemyTemplate.For(kind);
            string rank = EnemyRankFor(kind, index);
            int rankBonus = rank == "elite" ? 2 : rank == "veteran" ? 1 : 0;
            string displayName = RankEnemyName(t.Name, rank);
            return new CombatUnit
            {
                Id = Guid.NewGuid().ToString("N"),
                PartyIndex = -1,
                Side = UnitSide.Enemy,
                Name = displayName,
                Role = kind,
                Rank = rank,
                Origin = "ruins",
                Sigil = EnemySigil(kind),
                X = CombatW - 2 - (index % 2),
                Y = index % CombatH,
                Hp = t.Hp + state.Depth * 4 + rankBonus * 7,
                MaxHp = t.Hp + state.Depth * 4 + rankBonus * 7,
                Mana = 0,
                MaxMana = 0,
                Movement = CombatMoveAllowance,
                Power = t.Power + Mathf.FloorToInt(state.Depth * 1.4f) + rankBonus,
                Defense = t.Defense + state.Depth / 2 + (rank == "elite" ? 1 : 0),
                Agility = t.Agility + (rank == "veteran" && t.Range > 1 ? 1 : 0),
                Range = t.Range,
                AttackSpeed = Mathf.Clamp(8 + t.Agility + rankBonus, 5, 18),
                DamageMin = Mathf.Max(1, t.Power / 2 + rankBonus),
                DamageMax = Mathf.Max(2, t.Power + 3 + rankBonus * 2),
                Spell = "",
                Skills = new SkillSet().Normalize(),
                Color = RankColor(t.Color, rank),
                DamageType = t.DamageType,
                Resist = t.Resist,
                Weakness = t.Weakness,
                StatusOnHit = t.StatusOnHit,
                MagicResist = t.MagicResist + rankBonus,
                Fearless = t.Fearless || rank == "elite"
            };
        }

        private string EnemyRankFor(string kind, int index)
        {
            if (state.Depth < 2) return "";
            int roll = Mathf.Abs((state.Seed + state.Depth * 97 + index * 53 + StableSeed(kind)) % 100);
            int eliteChance = Mathf.Clamp(state.Depth * 4 - 3, 3, 18);
            int veteranChance = Mathf.Clamp(16 + state.Depth * 5, 18, 42);
            if (roll < eliteChance) return "elite";
            if (roll < veteranChance) return "veteran";
            return "";
        }

        private string RankEnemyName(string baseName, string rank)
        {
            if (rank == "elite") return "Marked " + baseName;
            if (rank == "veteran") return "Old " + baseName;
            return baseName;
        }

        private string RankColor(string baseColor, string rank)
        {
            Color color = baseColor.ToColor();
            if (rank == "elite") return Color.Lerp(color, gold, 0.30f).ToHex();
            if (rank == "veteran") return Color.Lerp(color, cursorWhite, 0.18f).ToHex();
            return baseColor;
        }

        private string EnemySigil(string kind)
        {
            switch (kind)
            {
                case "adept": return "eye";
                case "husk": return "diamond";
                case "reaver": return "flame";
                case "spore": return "leaf";
                case "shade": return "moon";
                case "glassmage": return "eye";
                case "thornbeast": return "chevron";
                case "mirearcher": return "bar";
                case "bonepriest": return "cross";
                case "cinderling": return "flame";
                case "gloamknight": return "diamond";
                case "koboldraider": return "chevron";
                case "koboldslinger": return "eye";
                case "koboldshaman": return "moon";
                case "koboldwizard": return "flame";
                case "koboldshield": return "diamond";
                case "sewerrat": return "bar";
                case "giantrat": return "chevron";
                case "ratfolk": return "bar";
                case "ratcutthroat": return "knife";
                case "ratmage": return "eye";
                case "ratcleric": return "cross";
                case "ratbrute": return "diamond";
                case "drowscout": return "eye";
                case "drowblade": return "moon";
                case "drowcrossbow": return "bar";
                case "drowmage": return "eye";
                case "drowpriest": return "cross";
                case "lesserdemon": return "flame";
                default: return "cross";
            }
        }

        private void HandleCombatTimers()
        {
            CombatUnit active = CurrentUnit();
            if (active == null) return;
            if (active.Side == UnitSide.Enemy && aiActAt > 0 && Time.time >= aiActAt)
            {
                aiActAt = -1f;
                EnemyAct(active);
                SyncPartyFromCombat();
                NextTurn();
            }
        }

        private void HandleCombatHotkeys()
        {
            CombatUnit active = CurrentUnit();
            if (active == null || active.Side != UnitSide.Party) return;

            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Z)) TryHotkeyAction(ActionMode.Move, active);
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.F)) TryHotkeyAction(ActionMode.Attack, active);
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.R)) TryHotkeyAction(ActionMode.Cast, active);
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.G)) TryHotkeyAction(ActionMode.Guard, active);
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.H)) TryHotkeyAction(ActionMode.Elixir, active);
            if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.T)) TryHotkeyAction(ActionMode.Wait, active);
        }

        private void TryHotkeyAction(ActionMode mode, CombatUnit active)
        {
            if (!ActionEnabled(mode, active))
            {
                PlaySfx("blocked", 0.6f);
                return;
            }

            SelectOrRunAction(mode, active);
        }

        private bool PrepareFormulaCode(CombatUnit active, string code)
        {
            if (active == null || string.IsNullOrWhiteSpace(code)) return false;
            string reason;
            if (!CanUseFormula(active, code, out reason))
            {
                PushLog(reason, Tone.Warn);
                PlaySfx("blocked", 0.7f);
                return false;
            }

            FormulaDef formula = GetFormula(code);
            pendingFormulaCode = formula.Code;
            selectedAction = ActionMode.Cast;
            showSpellbook = false;
            if (state?.Combat != null) state.Combat.Phase = CombatPhase.ChooseTarget;
            PushLog($"{active.Name} readies {formula.Name}. Choose the target.", Tone.Good);
            ShowBanner(formula.Name);
            PlaySfx("formula", 0.95f);
            return true;
        }

        private void ClearFormulaEntry()
        {
            pendingFormulaCode = "";
        }

        private void NextTurn()
        {
            if (state.Combat == null) return;
            aiActAt = -1f;
            ClearFormulaEntry();
            bool partyAlive = state.Combat.Units.Any(u => IsHeroUnit(u) && u.Hp > 0);
            bool enemiesAlive = state.Combat.Units.Any(u => u.Side == UnitSide.Enemy && u.Hp > 0);
            if (!partyAlive)
            {
                SyncPartyFromCombat();
                state.Mode = GameMode.Defeat;
                state.Combat = null;
                betaLabMode = false;
                PushLog("The company falls. A new oath may yet be sworn.", Tone.Warn);
                ShowBanner("Company defeated");
                PlaySfx("defeat");
                return;
            }
            if (!enemiesAlive)
            {
                FinishCombat();
                return;
            }

            List<CombatUnit> order = InitiativeOrder();
            int index = order.FindIndex(u => u.Id == state.Combat.ActiveId);
            if (index < 0 || index >= order.Count - 1)
            {
                index = 0;
                if (!string.IsNullOrEmpty(state.Combat.ActiveId)) state.Combat.Round++;
            }
            else index++;

            CombatUnit active = order[index];
            active.Guarding = false;
            active.GuardBonus = 0;
            state.Combat.ActiveId = active.Id;
            state.Combat.Moved = false;
            state.Combat.Acted = false;
            state.Combat.MovePoints = active.Webbed > 0 ? 0 : UnitMoveAllowance(active);
            state.Combat.ActionAvailable = true;
            state.Combat.Phase = active.Side == UnitSide.Enemy ? CombatPhase.EnemyThinking : CombatPhase.ChooseAction;
            selectedAction = ActionMode.Attack;
            ShowBanner(active.Side == UnitSide.Party ? active.Name + "'s turn" : active.Name + " moves");
            bool skipped = ApplyStartTurnEffects(active);
            if (active.Hp <= 0)
            {
                PushLog(active.Summoned ? $"{active.Name} loses its binding." : $"{active.Name} is down.", active.Side == UnitSide.Enemy ? Tone.Good : Tone.Warn);
                NextTurn();
                return;
            }
            if (skipped)
            {
                state.Combat.Moved = true;
                state.Combat.Acted = true;
                state.Combat.MovePoints = 0;
                state.Combat.ActionAvailable = false;
                state.Combat.Phase = CombatPhase.Resolving;
                NextTurn();
                return;
            }
            if (active.Side == UnitSide.Enemy)
            {
                aiActAt = Time.time + (state.ReducedMotion ? 0.05f : 0.45f);
            }
        }

        private void FinishCombat()
        {
            bool finalBattle = IsFinalBossCombat();
            int xp = CombatExperienceReward();
            SyncPartyFromCombat();
            int foundGold = rng.Next(18, 39) + state.Depth * 6;
            state.Gold += foundGold;
            if (rng.NextDouble() < 0.6) state.Elixirs++;
            AwardExperience(xp);
            if (finalBattle)
            {
                FinishCampaignVictory(foundGold, xp);
                return;
            }
            state.Mode = GameMode.Explore;
            state.Combat = null;
            betaLabMode = false;
            PushLog($"The field is won. {foundGold} gold and {xp} XP recovered.", Tone.Good);
            ShowBanner("Victory");
            PlaySfx("victory");
        }

        private bool IsFinalBossCombat()
        {
            if (state?.Combat?.Units == null) return false;
            if (state.Depth < FinalBossDepth) return false;
            return state.Combat.Units.Any(u => u.Side == UnitSide.Enemy && (u.Role == "meteorlich" || u.Role == "ritualheart"));
        }

        private void FinishCampaignVictory(int foundGold, int xp)
        {
            state.Mode = GameMode.Victory;
            state.Combat = null;
            betaLabMode = false;
            showSpellbook = false;
            state.ActiveStory = "Epilogue: The Old Road is sealed for now. Midgaard has one more dawn.";
            PushLog($"The final gate falls. {foundGold} gold and {xp} XP recovered.", Tone.Good);
            PushLog("Vhal Rakh's meteor crown breaks above the ritual heart. This beta route is complete.", Tone.Good);
            ShowBanner("The Old Road Is Sealed");
            PlaySfx("victory", 1.15f);
        }

        private int CombatExperienceReward()
        {
            if (state?.Combat?.Units == null) return 0;
            int reward = 0;
            foreach (CombatUnit enemy in state.Combat.Units.Where(u => u.Side == UnitSide.Enemy))
            {
                reward += Mathf.Max(6, enemy.MaxHp / 2 + enemy.Power * 2 + enemy.Defense * 3 + enemy.Range * 2);
                if (enemy.Rank == "veteran") reward += 12;
                if (enemy.Rank == "elite") reward += 28;
                if (IsCasterEnemy(enemy)) reward += 10;
            }
            return Mathf.Max(12, reward / Mathf.Max(1, state.Party.Count));
        }

        private void AwardExperience(int amount)
        {
            if (state?.Party == null || amount <= 0) return;
            foreach (PartyMember member in state.Party.Where(p => p.Hp > 0))
            {
                int gained = amount + RaceExperienceBonus(member, amount);
                member.Experience += gained;
                bool leveled = false;
                while (member.Experience >= ExperienceForNextLevel(member.Level))
                {
                    member.Experience -= ExperienceForNextLevel(member.Level);
                    member.Level++;
                    member.SkillPoints += 2;
                    member.StatPoints += member.Level % 2 == 0 ? 2 : 1;
                    leveled = true;
                }
                RecalculateMember(member);
                if (leveled)
                {
                    member.Hp = member.MaxHp;
                    member.Mana = member.MaxMana;
                    PushLog($"{member.Name} reaches level {member.Level}. Stat and skill points are waiting in the tavern scaffold.", Tone.Good);
                    AddFloat(Mathf.Clamp(state.PlayerX, 0, CombatW - 1), Mathf.Clamp(state.PlayerY, 0, CombatH - 1), "level", gold);
                }
            }
        }

        private int RaceExperienceBonus(PartyMember member, int amount)
        {
            return string.Equals(member?.Race, "human", StringComparison.OrdinalIgnoreCase) ? Mathf.Max(1, amount / 10) : 0;
        }

        private bool ApplyStartTurnEffects(CombatUnit active)
        {
            if (active.Summoned && active.SummonTurns > 0)
            {
                active.SummonTurns--;
                if (active.SummonTurns <= 0)
                {
                    active.Hp = 0;
                    AddFloat(active.X, active.Y, "unbound", violet);
                    AddBurst(active.X, active.Y, violet);
                    PlaySfx("death", 0.55f);
                    return true;
                }
            }

            bool skip = active.Stunned > 0 || active.Sleeping > 0;
            if (active.Poisoned > 0)
            {
                DealDamage(active, 3 + state.Depth / 2, "poison", poison);
                active.Poisoned = Mathf.Max(0, active.Poisoned - 1);
                PlaySfx("poison", 0.55f);
            }
            if (active.Bleeding > 0)
            {
                DealDamage(active, 2, "physical", blood);
                active.Bleeding = Mathf.Max(0, active.Bleeding - 1);
            }
            if (active.Regenerating > 0 && active.Hp > 0)
            {
                int heal = 4 + state.Depth;
                active.Hp = Mathf.Min(active.MaxHp, active.Hp + heal);
                AddFloat(active.X, active.Y, "+" + heal, teal);
                active.Regenerating = Mathf.Max(0, active.Regenerating - 1);
            }
            if (active.Hexed > 0)
            {
                active.Hexed = Mathf.Max(0, active.Hexed - 1);
                if (active.Hexed > 0) AddFloat(active.X, active.Y, "hexed", violet);
            }

            Point terrain = ObstacleAt(active.X, active.Y);
            if (terrain != null && active.Hp > 0)
            {
                if (terrain.Kind == "fire")
                {
                    DealDamage(active, 4 + state.Depth, "fire", ember);
                    PlaySfx("fire", 0.45f);
                }
                else if (terrain.Kind == "gas")
                {
                    DealDamage(active, 1 + state.Depth / 2, "poison", poison);
                    TryApplyStatus(active, "poison", 2, null, 1f, false);
                }
                else if (terrain.Kind == "web")
                {
                    active.Webbed = Mathf.Max(active.Webbed, 2);
                    AddFloat(active.X, active.Y, "webbed", poison);
                }
                else if (terrain.Kind == "ice" && rng.NextDouble() < 0.35)
                {
                    TryApplyStatus(active, "stun", 1, null, 1f, false);
                }
            }

            foreach (Point hazard in state.Combat.Obstacles.ToList())
            {
                if (hazard.Duration <= 0) continue;
                hazard.Duration--;
                if (hazard.Duration <= 0 && IsBlockingTerrain(hazard))
                {
                    state.Combat.Obstacles.Remove(hazard);
                    AddFloat(hazard.X, hazard.Y, hazard.Kind == "tree" ? "withers" : "crumbles", hazard.Kind == "tree" ? moss : stone);
                    AddFlash(hazard.X, hazard.Y, hazard.Kind == "tree" ? moss : stone);
                }
                else if (hazard.Duration <= 0)
                {
                    state.Combat.Obstacles.Remove(hazard);
                }
            }

            if (active.Shielded > 0) active.Shielded = Mathf.Max(0, active.Shielded - 1);
            if (active.Webbed > 0) active.Webbed = Mathf.Max(0, active.Webbed - 1);
            if (skip)
            {
                string status = active.Sleeping > 0 ? "sleeping" : "stunned";
                if (active.Sleeping > 0) active.Sleeping = Mathf.Max(0, active.Sleeping - 1);
                if (active.Stunned > 0) active.Stunned = Mathf.Max(0, active.Stunned - 1);
                PushLog($"{active.Name} is {status} and loses the turn.", Tone.Warn);
                AddFloat(active.X, active.Y, status, violet);
            }
            return skip;
        }

        private void EnemyAct(CombatUnit enemy)
        {
            state.Combat.Phase = CombatPhase.EnemyThinking;
            CombatUnit target = BestEnemyTarget(enemy);
            if (target == null) return;
            bool triedArcingSpecial = EnemySpecialArcsOverCover(enemy);
            if (triedArcingSpecial && TryEnemySpecial(enemy, target)) return;
            if (TryEnemyBreakCover(enemy, target, true)) return;
            if (!triedArcingSpecial && TryEnemySpecial(enemy, target)) return;
            int steps = enemy.Webbed > 0 ? 0 : UnitMoveAllowance(enemy);
            Vector2 start = new Vector2(enemy.X, enemy.Y);
            bool moved = false;
            while (steps > 0 && !CanEnemyAttack(enemy, target))
            {
                Point step = BestStepToward(enemy, target);
                if (step == null)
                {
                    if (TryEnemyBreakCover(enemy, target)) return;
                    break;
                }
                enemy.X = step.X;
                enemy.Y = step.Y;
                moved = true;
                steps--;
                target = BestEnemyTarget(enemy) ?? target;
            }
            if (moved)
            {
                AddTween(enemy.Id, start, new Vector2(enemy.X, enemy.Y), TweenKind.Move);
            }
            if (enemy.Webbed > 0)
            {
                PushLog($"{enemy.Name} strains against webbing.", Tone.Normal);
            }
            if (!moved && target != null && CanEnemyAttack(enemy, target))
            {
                Attack(enemy, target);
            }
            else if (target != null && TryEnemyBreakCover(enemy, target))
            {
                return;
            }
            else
            {
                PushLog($"{enemy.Name} advances.", Tone.Normal);
            }
        }

        private bool TryEnemyBreakCover(CombatUnit enemy, CombatUnit target, bool urgentOnly = false)
        {
            Point cover = BestCoverToBreak(enemy, target, urgentOnly);
            if (cover == null) return false;

            int damage = CoverBreakDamage(enemy, cover);
            int before = CoverIntegrity(cover);
            cover.Integrity = Mathf.Max(0, before - damage);
            Color color = cover.Kind == "tree" ? moss : stone;
            int distance = Distance(enemy.X, enemy.Y, cover.X, cover.Y);
            if (distance > 1 && enemy.Range > 1)
            {
                AddBeam(enemy.X, enemy.Y, cover.X, cover.Y, color, EnemySpecialArcsOverCover(enemy) ? "arc" : "shot");
            }
            else
            {
                AddTween(enemy.Id, new Vector2(enemy.X, enemy.Y), new Vector2(enemy.X + Mathf.Sign(cover.X - enemy.X) * 0.16f, enemy.Y + Mathf.Sign(cover.Y - enemy.Y) * 0.16f), TweenKind.Lunge);
            }
            AddFloat(cover.X, cover.Y, cover.Integrity <= 0 ? "broken" : $"-{damage}", color);
            AddFlash(cover.X, cover.Y, color);
            if (cover.Integrity <= 0)
            {
                state.Combat.Obstacles.Remove(cover);
                AddBurst(cover.X, cover.Y, color);
                PushLog($"{enemy.Name} breaks through the {CoverName(cover)}.", Tone.Warn);
                PlaySfx("breakcover", 0.82f);
            }
            else
            {
                string verb = distance > 1 && enemy.Range > 1 ? "pressures" : "batters";
                PushLog($"{enemy.Name} {verb} the {CoverName(cover)}. {cover.Integrity} integrity remains.", Tone.Warn);
                PlaySfx(cover.Kind == "tree" ? "attack" : "stone", 0.62f);
            }
            return true;
        }

        private Point BestCoverToBreak(CombatUnit enemy, CombatUnit target, bool urgentOnly = false)
        {
            if (enemy == null || target == null || state?.Combat?.Obstacles == null) return null;
            List<Point> candidates = new List<Point>
            {
                ObstacleAt(enemy.X + 1, enemy.Y),
                ObstacleAt(enemy.X - 1, enemy.Y),
                ObstacleAt(enemy.X, enemy.Y + 1),
                ObstacleAt(enemy.X, enemy.Y - 1)
            };

            if (enemy.Range > 1)
            {
                candidates.AddRange(BlockingCoverAlongLine(enemy.X, enemy.Y, target.X, target.Y)
                    .Where(o => Distance(enemy.X, enemy.Y, o.X, o.Y) <= enemy.Range && HasLineOfSight(enemy.X, enemy.Y, o.X, o.Y, true)));
            }

            IEnumerable<Point> covers = candidates
                .Where(IsBreakableCover)
                .GroupBy(o => $"{o.X},{o.Y}")
                .Select(g => g.First());
            if (urgentOnly) covers = covers.Where(o => CoverBlocksEnemyPressure(enemy, target, o));
            return covers
                .OrderBy(o => CoverBreakScore(enemy, target, o))
                .FirstOrDefault();
        }

        private bool CoverBlocksEnemyPressure(CombatUnit enemy, CombatUnit target, Point cover)
        {
            if (enemy == null || target == null || !IsBreakableCover(cover)) return false;
            if (BlockingCoverAlongLine(enemy.X, enemy.Y, target.X, target.Y).Any(o => o.X == cover.X && o.Y == cover.Y)) return true;
            int distance = Distance(enemy.X, enemy.Y, target.X, target.Y);
            int coverToTarget = Distance(cover.X, cover.Y, target.X, target.Y);
            if (enemy.Range > 1 && distance <= enemy.Range && !HasLineOfSight(enemy.X, enemy.Y, target.X, target.Y, true)) return true;
            if (coverToTarget <= 1 && distance <= Mathf.Max(2, enemy.Range + 2)) return true;
            if (enemy.Range <= 1 && coverToTarget < distance && !HasUsefulStepToward(enemy, target)) return true;
            return false;
        }

        private bool HasUsefulStepToward(CombatUnit mover, CombatUnit target)
        {
            if (mover == null || target == null) return false;
            int current = Distance(mover.X, mover.Y, target.X, target.Y);
            Point[] steps =
            {
                new Point(mover.X + 1, mover.Y),
                new Point(mover.X - 1, mover.Y),
                new Point(mover.X, mover.Y + 1),
                new Point(mover.X, mover.Y - 1)
            };
            return steps.Any(p => CanStandAt(p.X, p.Y) && Distance(p.X, p.Y, target.X, target.Y) < current);
        }

        private int CoverBreakScore(CombatUnit enemy, CombatUnit target, Point cover)
        {
            int score = Distance(cover.X, cover.Y, target.X, target.Y) * 10;
            score += Distance(enemy.X, enemy.Y, cover.X, cover.Y) * 4;
            if (CoverBlocksEnemyPressure(enemy, target, cover)) score -= 36;
            if (enemy.Range > 1 && !HasLineOfSight(enemy.X, enemy.Y, target.X, target.Y, true)) score -= 24;
            if (cover.Kind == "tree") score -= 8;
            score += CoverIntegrity(cover) * 2;
            return score;
        }

        private bool IsBreakableCover(Point cover)
        {
            return cover != null && (cover.Kind == "tree" || cover.Kind == "stone");
        }

        private int CoverBreakDamage(CombatUnit enemy, Point cover)
        {
            int damage = 1;
            if (IsBruteEnemy(enemy) || enemy.Power >= 9) damage++;
            if (enemy.Range <= 1 && enemy.WeaponName != null && enemy.WeaponName.ToLowerInvariant().Contains("axe")) damage++;
            if (enemy.Role == "cinderling" && cover?.Kind == "tree") damage++;
            if (enemy.Rank == "elite") damage++;
            return Mathf.Clamp(damage, 1, 4);
        }

        private int CoverIntegrity(Point cover)
        {
            if (cover == null) return 0;
            return cover.Integrity > 0 ? cover.Integrity : CoverMaxIntegrity(cover.Kind);
        }

        private int CoverMaxIntegrity(string kind)
        {
            if (kind == "tree") return 2;
            if (kind == "stone") return 3;
            return 0;
        }

        private string CoverName(Point cover)
        {
            if (cover == null) return "cover";
            if (cover.Kind == "tree") return "tree cover";
            if (cover.Kind == "stone") return "stone block";
            return cover.Kind;
        }

        private bool TryEnemySpecial(CombatUnit enemy, CombatUnit target)
        {
            if (enemy == null || target == null || rng.NextDouble() > EnemySpecialChance(enemy)) return false;

            if (enemy.Role == "bonepriest" || enemy.Role == "ratcleric" || enemy.Role == "drowpriest")
            {
                CombatUnit ally = state.Combat.Units
                    .Where(u => u.Side == UnitSide.Enemy && u.Hp > 0 && u.Hp < u.MaxHp && Distance(u.X, u.Y, enemy.X, enemy.Y) <= 4)
                    .OrderBy(u => (float)u.Hp / Mathf.Max(1, u.MaxHp))
                    .FirstOrDefault();
                if (ally != null)
                {
                    int heal = 7 + state.Depth + EnemyRankBonus(enemy) * 3;
                    ally.Hp = Mathf.Min(ally.MaxHp, ally.Hp + heal);
                    ally.Shielded = Mathf.Max(ally.Shielded, 2);
                    AddBeam(enemy.X, enemy.Y, ally.X, ally.Y, teal, "heal");
                    AddFloat(ally.X, ally.Y, "+" + heal, teal);
                    AddBurst(ally.X, ally.Y, teal);
                    PushLog($"{enemy.Name} rattles a ward over {ally.Name}.", Tone.Warn);
                PlaySfx("heal", 0.7f);
                return true;
            }
            }

            if (!CanEnemySpecialReach(enemy, target)) return false;

            if (enemy.Role == "koboldshaman")
            {
                AddEnemySpecialBeam(enemy, target, violet, "hex");
                DealDamage(target, Mathf.Max(3, enemy.Power - 1 + EnemyRankBonus(enemy)), "mind", violet);
                TryApplyStatus(target, "hex", 3, enemy, 0.66f + EnemyRankBonus(enemy) * 0.08f, true);
                Point web = BestHazardTileNear(target, "web");
                if (web != null && rng.NextDouble() < 0.58) state.Combat.Obstacles.Add(web);
                AddCastGlyph(enemy, null, violet);
                PushLog($"{enemy.Name} rattles a bone hex at {target.Name}.", Tone.Warn);
                PlaySfx("death", 0.62f);
                return true;
            }

            if (enemy.Role == "koboldwizard")
            {
                AddEnemySpecialBeam(enemy, target, blood, "death");
                int damage = Mathf.Max(5, enemy.Power + 1 + EnemyRankBonus(enemy) * 2);
                DealDamage(target, damage, "death", blood);
                foreach (CombatUnit unit in state.Combat.Units.Where(u => u.Side == UnitSide.Party && u.Hp > 0 && u.Id != target.Id && Distance(u.X, u.Y, target.X, target.Y) <= 1))
                {
                    DealDamage(unit, Mathf.Max(2, damage / 3), "death", blood);
                }
                TryApplyStatus(target, "hex", 2, enemy, 0.42f + EnemyRankBonus(enemy) * 0.08f, true);
                AddCastGlyph(enemy, null, blood);
                PushLog($"{enemy.Name} looses a red-black death ball.", Tone.Warn);
                PlaySfx("death", 0.88f);
                return true;
            }

            if (enemy.Role == "adept")
            {
                AddEnemySpecialBeam(enemy, target, gold, "arc");
                DealDamage(target, Mathf.Max(3, enemy.Power - 1), "shock", gold);
                TryApplyStatus(target, "stun", 1, enemy, 0.34f + EnemyRankBonus(enemy) * 0.08f, true);
                PushLog($"{enemy.Name} snaps a shock sign at {target.Name}.", Tone.Warn);
                PlaySfx("spell", 0.7f);
                return true;
            }

            if (enemy.Role == "glassmage")
            {
                AddEnemySpecialBeam(enemy, target, frost, "ice");
                DealDamage(target, Mathf.Max(3, enemy.Power), "cold", frost);
                Point ice = BestHazardTileNear(target, "ice");
                if (ice != null) state.Combat.Obstacles.Add(ice);
                PushLog($"{enemy.Name} splinters cold light across the floor.", Tone.Warn);
                PlaySfx("ice", 0.8f);
                return true;
            }

            if (enemy.Role == "ratmage")
            {
                AddEnemySpecialBeam(enemy, target, poison, "hex");
                DealDamage(target, Mathf.Max(3, enemy.Power - 1), "poison", poison);
                TryApplyStatus(target, "poison", 2, enemy, 0.58f + EnemyRankBonus(enemy) * 0.08f, true);
                Point gas = BestHazardTileNear(target, "gas");
                if (gas != null && rng.NextDouble() < 0.52) state.Combat.Obstacles.Add(gas);
                PushLog($"{enemy.Name} hisses plague signs through the cistern air.", Tone.Warn);
                PlaySfx("poison", 0.78f);
                return true;
            }

            if (enemy.Role == "drowmage" || enemy.Role == "drowpriest")
            {
                AddEnemySpecialBeam(enemy, target, violet, "hex");
                DealDamage(target, Mathf.Max(4, enemy.Power), "mind", violet);
                TryApplyStatus(target, "hex", 2, enemy, 0.48f + EnemyRankBonus(enemy) * 0.08f, true);
                PushLog($"{enemy.Name} bends dark light around {target.Name}.", Tone.Warn);
                PlaySfx("death", 0.66f);
                return true;
            }

            if (enemy.Role == "spore")
            {
                AddEnemySpecialBeam(enemy, target, poison, "hex");
                DealDamage(target, Mathf.Max(2, enemy.Power - 2), "poison", poison);
                TryApplyStatus(target, "poison", 2, enemy, 0.55f + EnemyRankBonus(enemy) * 0.08f, true);
                Point gas = BestHazardTileNear(target, "gas");
                if (gas != null && rng.NextDouble() < 0.45) state.Combat.Obstacles.Add(gas);
                PushLog($"{enemy.Name} coughs venom dust toward {target.Name}.", Tone.Warn);
                PlaySfx("poison", 0.75f);
                return true;
            }

            if (enemy.Role == "cinderling")
            {
                AddEnemySpecialBeam(enemy, target, ember, "fire");
                DealDamage(target, Mathf.Max(4, enemy.Power), "fire", ember);
                Point fire = BestHazardTileNear(target, "fire");
                if (fire != null) state.Combat.Obstacles.Add(fire);
                PushLog($"{enemy.Name} spits a low cinder trail.", Tone.Warn);
                PlaySfx("fire", 0.8f);
                return true;
            }

            if (enemy.Role == "lesserdemon")
            {
                AddEnemySpecialBeam(enemy, target, ember, "fire");
                DealDamage(target, Mathf.Max(5, enemy.Power + EnemyRankBonus(enemy)), "fire", ember);
                TryApplyStatus(target, "bleed", 2, enemy, 0.34f + EnemyRankBonus(enemy) * 0.08f, true);
                Point fire = BestHazardTileNear(target, "fire");
                if (fire != null && rng.NextDouble() < 0.45) state.Combat.Obstacles.Add(fire);
                PushLog($"{enemy.Name} claws a burning pact mark into the floor.", Tone.Warn);
                PlaySfx("fire", 0.86f);
                return true;
            }

            if (enemy.Role == "shade")
            {
                AddEnemySpecialBeam(enemy, target, violet, "hex");
                TryApplyStatus(target, "sleep", 1, enemy, 0.42f + EnemyRankBonus(enemy) * 0.08f, true);
                DealDamage(target, Mathf.Max(2, enemy.Power - 3), "mind", violet);
                PushLog($"{enemy.Name} dims the air around {target.Name}.", Tone.Warn);
                PlaySfx("death", 0.55f);
                return true;
            }

            return false;
        }

        private float EnemySpecialChance(CombatUnit enemy)
        {
            if (enemy == null) return 0f;
            float baseChance = 0f;
            if (enemy.Role == "bonepriest" || enemy.Role == "ratcleric" || enemy.Role == "drowpriest") baseChance = 0.62f;
            else if (enemy.Role == "koboldwizard") baseChance = 0.52f;
            else if (enemy.Role == "koboldshaman") baseChance = 0.46f;
            else if (enemy.Role == "adept" || enemy.Role == "glassmage" || enemy.Role == "ratmage" || enemy.Role == "drowmage" || enemy.Role == "spore" || enemy.Role == "cinderling" || enemy.Role == "lesserdemon" || enemy.Role == "shade") baseChance = 0.34f;
            return Mathf.Clamp01(baseChance + EnemyRankBonus(enemy) * 0.10f);
        }

        private int EnemyRankBonus(CombatUnit enemy)
        {
            if (enemy == null) return 0;
            if (enemy.Rank == "elite") return 2;
            if (enemy.Rank == "veteran") return 1;
            return 0;
        }

        private Point BestHazardTileNear(CombatUnit target, string kind)
        {
            if (target == null) return null;
            Point[] candidates =
            {
                new Point(target.X, target.Y, kind, 10),
                new Point(target.X + 1, target.Y, kind, 10),
                new Point(target.X - 1, target.Y, kind, 10),
                new Point(target.X, target.Y + 1, kind, 10),
                new Point(target.X, target.Y - 1, kind, 10)
            };
            return candidates.FirstOrDefault(p => p.X >= 0 && p.X < CombatW && p.Y >= 0 && p.Y < CombatH && ObstacleAt(p.X, p.Y) == null && !IsBlockingTerrain(p));
        }

        private CombatUnit BestEnemyTarget(CombatUnit enemy)
        {
            return state.Combat.Units.Where(u => u.Side == UnitSide.Party && u.Hp > 0)
                .OrderBy(u => EnemyTargetScore(enemy, u))
                .FirstOrDefault();
        }

        private int EnemyTargetScore(CombatUnit enemy, CombatUnit target)
        {
            int distance = Distance(target.X, target.Y, enemy.X, enemy.Y);
            int score = distance * 10;
            if (CanEnemyAttack(enemy, target)) score -= 45;
            if (enemy.Range > 1 && HasLineOfSight(enemy.X, enemy.Y, target.X, target.Y, true)) score -= 12;
            score += Mathf.RoundToInt((target.Hp / (float)Mathf.Max(1, target.MaxHp)) * 20f);
            if (target.Guarding) score += Mathf.Max(8, target.GuardBonus * 5);
            if (IsCasterEnemy(enemy) && !string.IsNullOrEmpty(target.Spell)) score -= 16;
            if (IsBruteEnemy(enemy) && (target.Role == "bow" || target.Role == "ember" || target.Role == "hex")) score -= 8;
            if (HasTag(target.Weakness, enemy.DamageType)) score -= 10;
            if (HasTag(target.Resist, enemy.DamageType)) score += 12;
            return score;
        }

        private bool CanEnemyAttack(CombatUnit enemy, CombatUnit target)
        {
            if (enemy == null || target == null || target.Hp <= 0) return false;
            if (Distance(enemy.X, enemy.Y, target.X, target.Y) > enemy.Range) return false;
            return enemy.Range <= 1 || HasLineOfSight(enemy.X, enemy.Y, target.X, target.Y, true);
        }

        private bool CanEnemySpecialReach(CombatUnit enemy, CombatUnit target)
        {
            if (enemy == null || target == null || target.Hp <= 0) return false;
            if (Distance(enemy.X, enemy.Y, target.X, target.Y) > enemy.Range) return false;
            return HasLineOfSight(enemy.X, enemy.Y, target.X, target.Y, true) || EnemySpecialArcsOverCover(enemy);
        }

        private bool EnemySpecialArcsOverCover(CombatUnit enemy)
        {
            if (enemy == null) return false;
            return enemy.Role == "koboldwizard"
                || enemy.Role == "koboldshaman"
                || enemy.Role == "adept"
                || enemy.Role == "glassmage"
                || enemy.Role == "ratmage"
                || enemy.Role == "ratcleric"
                || enemy.Role == "drowmage"
                || enemy.Role == "drowpriest"
                || enemy.Role == "shade"
                || enemy.DamageType == "death"
                || enemy.DamageType == "mind"
                || enemy.DamageType == "shock"
                || enemy.DamageType == "cold";
        }

        private void AddEnemySpecialBeam(CombatUnit enemy, CombatUnit target, Color color, string kind)
        {
            if (enemy == null || target == null) return;
            bool arcing = EnemySpecialArcsOverCover(enemy) && !HasLineOfSight(enemy.X, enemy.Y, target.X, target.Y, true);
            AddBeam(enemy.X, enemy.Y, target.X, target.Y, color, arcing ? "arc" : kind);
            if (arcing) AddFloat(target.X, target.Y, "over cover", color);
        }

        private Point BestStepToward(CombatUnit mover, CombatUnit target)
        {
            Point[] steps =
            {
                new Point(mover.X + 1, mover.Y),
                new Point(mover.X - 1, mover.Y),
                new Point(mover.X, mover.Y + 1),
                new Point(mover.X, mover.Y - 1)
            };
            return steps.Where(p => CanStandAt(p.X, p.Y)).OrderBy(p => EnemyStepScore(mover, target, p)).FirstOrDefault();
        }

        private int EnemyStepScore(CombatUnit mover, CombatUnit target, Point step)
        {
            int distance = Distance(step.X, step.Y, target.X, target.Y);
            int score = distance * 10;
            Point terrain = ObstacleAt(step.X, step.Y);
            if (terrain != null)
            {
                if (terrain.Kind == "fire" || terrain.Kind == "gas") score += IsBruteEnemy(mover) ? 16 : 30;
                if (terrain.Kind == "web" || terrain.Kind == "ice") score += IsBruteEnemy(mover) ? 10 : 18;
            }
            if (mover.Range > 1)
            {
                bool hasSight = HasLineOfSight(step.X, step.Y, target.X, target.Y, true);
                if (!hasSight) score += IsCasterEnemy(mover) ? 24 : 16;
                else score -= 10;
                if (distance < Mathf.Max(2, mover.Range - 1)) score += IsCasterEnemy(mover) ? 16 : 8;
                if (distance == mover.Range) score -= 6;
            }
            else if (IsBruteEnemy(mover) && distance <= 1)
            {
                score -= 8;
            }
            return score;
        }

        private bool IsCasterEnemy(CombatUnit enemy)
        {
            if (enemy == null) return false;
            return enemy.Role == "adept"
                || enemy.Role == "glassmage"
                || enemy.Role == "bonepriest"
                || enemy.Role == "ratmage"
                || enemy.Role == "ratcleric"
                || enemy.Role == "drowmage"
                || enemy.Role == "drowpriest"
                || enemy.Role == "cinderling"
                || enemy.Role == "spore"
                || enemy.Role == "koboldshaman"
                || enemy.Role == "koboldwizard"
                || enemy.DamageType == "shock"
                || enemy.DamageType == "cold"
                || enemy.DamageType == "mind"
                || enemy.DamageType == "death";
        }

        private bool IsBruteEnemy(CombatUnit enemy)
        {
            if (enemy == null) return false;
            return enemy.Role == "husk" || enemy.Role == "thornbeast" || enemy.Role == "gloamknight" || enemy.Role == "ratbrute" || enemy.Role == "lesserdemon" || enemy.Range <= 1 && enemy.Defense >= 4;
        }

        private void MoveActiveTo(CombatUnit active, int x, int y)
        {
            if (state.Combat.MovePoints <= 0) return;
            if (active.Webbed > 0)
            {
                PushLog($"{active.Name} is caught in webbing.", Tone.Warn);
                AddFloat(active.X, active.Y, "webbed", poison);
                PlaySfx("web");
                return;
            }
            int distance = Distance(x, y, active.X, active.Y);
            int moveCost = MoveCostTo(active, x, y);
            if (distance <= 0) return;
            if (!CanStandAt(x, y))
            {
                PlaySfx("blocked");
                return;
            }
            if (moveCost >= UnreachableMoveCost)
            {
                PushLog("No clear path to that tile.", Tone.Warn);
                PlaySfx("blocked");
                return;
            }
            if (moveCost > state.Combat.MovePoints)
            {
                PushLog("That move is too far.", Tone.Warn);
                PlaySfx("blocked");
                return;
            }
            Vector2 from = new Vector2(active.X, active.Y);
            active.X = x;
            active.Y = y;
            state.Combat.MovePoints = Mathf.Max(0, state.Combat.MovePoints - moveCost);
            state.Combat.Moved = state.Combat.MovePoints <= 0;
            state.Combat.Phase = CombatPhase.ChooseAction;
            AddTween(active.Id, from, new Vector2(x, y), TweenKind.Move);
            string warning = TerrainLogWarning(ObstacleAt(x, y));
            PushLog($"{active.Name} takes position. {state.Combat.MovePoints} move left.", Tone.Normal);
            if (!string.IsNullOrEmpty(warning)) PushLog(warning, Tone.Warn);
            PlaySfx("move", 0.72f);
        }

        private bool Attack(CombatUnit attacker, CombatUnit target)
        {
            if (Distance(attacker.X, attacker.Y, target.X, target.Y) > attacker.Range)
            {
                PushLog($"{target.Name} is out of reach.", Tone.Warn);
                return false;
            }
            if (attacker.Range > 1 && !HasLineOfSight(attacker.X, attacker.Y, target.X, target.Y, true))
            {
                PushLog("Cover breaks the shot line.", Tone.Warn);
                AddFloat(target.X, target.Y, "cover", moss);
                PlaySfx("blocked");
                return false;
            }
            string skill = attacker.Range > 1 ? "missile" : "arms";
            int skillValue = SkillValue(attacker.Skills, skill);
            int skillBonus = skillValue / 5;
            int guard = target.Guarding ? Mathf.Max(2, target.GuardBonus) : 0;
            string damageType = string.IsNullOrWhiteSpace(attacker.DamageType) ? "physical" : attacker.DamageType;
            int hitChance = AttackHitChance(attacker, target);
            int critFloor = target.Sleeping > 0 || target.Hexed > 0 ? 72 : 82;
            critFloor -= Mathf.Clamp(attacker.AttackSpeed / 4, 0, 4);
            bool critical = rng.Next(100) >= Mathf.Max(critFloor, 97 - skillValue / 3);
            if (rng.Next(100) >= hitChance)
            {
                AddTween(attacker.Id, new Vector2(attacker.X, attacker.Y), new Vector2(attacker.X + Mathf.Sign(target.X - attacker.X) * 0.16f, attacker.Y + Mathf.Sign(target.Y - attacker.Y) * 0.16f), TweenKind.Lunge);
                if (attacker.Range > 1) AddBeam(attacker.X, attacker.Y, target.X, target.Y, Hex("d9d3c4"), "shot");
                AddFloat(target.X, target.Y, "miss", muted);
                AddFlash(target.X, target.Y, Hex("d9d3c4", 0.65f));
                PushLog($"{attacker.Name} misses {target.Name}.", Tone.Normal);
                PlaySfx("miss", 0.64f);
                ImproveSkill(attacker, skill, 1);
                return true;
            }
            int hexShift = (target.Hexed > 0 ? 2 : 0) - (attacker.Hexed > 0 ? 2 : 0);
            int minDamage = attacker.DamageMin > 0 ? attacker.DamageMin : Mathf.Max(1, attacker.Power - 2);
            int maxDamage = attacker.DamageMax > 0 ? attacker.DamageMax : Mathf.Max(minDamage + 1, attacker.Power + 4);
            int rawDamage = Mathf.Max(1, rng.Next(minDamage, maxDamage + 1) + skillBonus + hexShift - target.Defense - target.ArmorBonus - guard);
            if (critical) rawDamage = Mathf.RoundToInt(rawDamage * 1.65f) + 2;
            int damage = DealDamage(target, rawDamage, damageType, DamageColor(damageType));
            if (attacker.Range > 1) AddBeam(attacker.X, attacker.Y, target.X, target.Y, DamageColor(damageType), "shot");
            AddTween(attacker.Id, new Vector2(attacker.X, attacker.Y), new Vector2(attacker.X + Mathf.Sign(target.X - attacker.X) * 0.25f, attacker.Y + Mathf.Sign(target.Y - attacker.Y) * 0.25f), TweenKind.Lunge);
            if (critical) AddFloat(target.X, target.Y, "crit", gold);
            PushLog($"{attacker.Name} {(critical ? "strikes hard" : "hits")} {target.Name} for {damage} {damageType}.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
            PlaySfx(AttackSfx(attacker, damageType, critical), critical ? 0.96f : 0.76f);
            ImproveSkill(attacker, skill, 1);
            if (target.Hp > 0 && target.Guarding && Distance(attacker.X, attacker.Y, target.X, target.Y) <= 1)
            {
                int counter = Mathf.Max(1, target.GuardBonus / 2);
                DealDamage(attacker, counter, "physical", teal);
                AddFloat(attacker.X, attacker.Y, "counter", teal);
                PushLog($"{target.Name}'s guard bites back.", target.Side == UnitSide.Party ? Tone.Good : Tone.Warn);
                PlaySfx("counter", 0.62f);
            }
            if (target.Hp > 0 && !string.IsNullOrEmpty(attacker.StatusOnHit))
            {
                TryApplyStatus(target, attacker.StatusOnHit, 2, attacker, 0.45f, attacker.Side != target.Side);
            }
            if (target.Hp > 0)
            {
                string gearStatus = GearOnHitStatus(attacker.WeaponName);
                if (!string.IsNullOrEmpty(gearStatus))
                {
                    TryApplyStatus(target, gearStatus, 2, attacker, GearOnHitChance(attacker.WeaponName), true);
                }
            }
            if (target.Hp <= 0)
            {
                PushLog($"{target.Name} is down.", Tone.Good);
                AddFloat(target.X, target.Y, "down", gold);
                AddFlash(target.X, target.Y, gold);
            }
            return true;
        }

        private bool AttackCover(CombatUnit attacker, Point cover)
        {
            if (attacker == null || !IsBreakableCover(cover)) return false;
            int distance = Distance(attacker.X, attacker.Y, cover.X, cover.Y);
            if (distance > attacker.Range)
            {
                PushLog($"{CoverName(cover)} is out of reach.", Tone.Warn);
                PlaySfx("blocked", 0.62f);
                return false;
            }
            if (attacker.Range > 1 && !HasLineOfSight(attacker.X, attacker.Y, cover.X, cover.Y, true))
            {
                PushLog("Other cover blocks the shot.", Tone.Warn);
                AddFloat(cover.X, cover.Y, "blocked", moss);
                PlaySfx("blocked", 0.62f);
                return false;
            }

            int damage = CoverBreakDamage(attacker, cover);
            cover.Integrity = Mathf.Max(0, CoverIntegrity(cover) - damage);
            Color color = cover.Kind == "tree" ? moss : stone;
            AddTween(attacker.Id, new Vector2(attacker.X, attacker.Y), new Vector2(attacker.X + Mathf.Sign(cover.X - attacker.X) * 0.18f, attacker.Y + Mathf.Sign(cover.Y - attacker.Y) * 0.18f), TweenKind.Lunge);
            if (attacker.Range > 1) AddBeam(attacker.X, attacker.Y, cover.X, cover.Y, Hex("d9d3c4"), "shot");
            AddFloat(cover.X, cover.Y, cover.Integrity <= 0 ? "broken" : "-" + damage, color);
            AddFlash(cover.X, cover.Y, color);
            ImproveSkill(attacker, attacker.Range > 1 ? "missile" : "arms", 1);
            if (cover.Integrity <= 0)
            {
                state.Combat.Obstacles.Remove(cover);
                AddBurst(cover.X, cover.Y, color);
                PushLog($"{attacker.Name} breaks the {CoverName(cover)}.", Tone.Good);
                PlaySfx("breakcover", 0.82f);
            }
            else
            {
                PushLog($"{attacker.Name} damages the {CoverName(cover)}. {cover.Integrity} integrity remains.", Tone.Normal);
                PlaySfx(cover.Kind == "tree" ? "attack" : "stone", 0.66f);
            }
            return true;
        }

        private string AttackSfx(CombatUnit attacker, string damageType, bool critical)
        {
            if (critical) return "crit";
            if (attacker != null && attacker.Range > 1) return "bow";
            if (damageType == "fire") return "fire";
            if (damageType == "cold") return "ice";
            if (damageType == "shock") return "shock";
            if (damageType == "poison") return "poison";
            if (damageType == "death" || damageType == "mind") return "death";
            return "blade";
        }

        private int AttackHitChance(CombatUnit attacker, CombatUnit target)
        {
            if (attacker == null || target == null) return 0;
            string skill = attacker.Range > 1 ? "missile" : "arms";
            int skillValue = SkillValue(attacker.Skills, skill);
            int guard = target.Guarding ? Mathf.Max(2, target.GuardBonus) : 0;
            int statusShift = 0;
            if (attacker.Hexed > 0) statusShift -= 12;
            if (target.Hexed > 0) statusShift += 10;
            if (target.Webbed > 0) statusShift += 8;
            if (target.Sleeping > 0) statusShift += 18;
            return Mathf.Clamp(62 + skillValue + attacker.Agility * 2 + attacker.AttackSpeed + attacker.WeaponBonus * 4 + WeaponHitBonus(attacker.WeaponName) + RaceHitBonus(attacker) + statusShift - target.Agility * 3 - (target.Defense + target.ArmorBonus) * 3 - guard * 8, 18, 95);
        }

        private bool CastSpell(CombatUnit caster, CombatUnit target)
        {
            if (string.IsNullOrEmpty(caster.Spell))
            {
                PushLog($"{caster.Name} knows no battle spell.", Tone.Warn);
                return false;
            }
            if (CasterKnowsSchool(caster.Spell, "mend"))
            {
                if (target.Side != UnitSide.Party)
                {
                    PushLog("Mend needs an ally.", Tone.Warn);
                    return false;
                }
                if (Distance(caster.X, caster.Y, target.X, target.Y) > 4)
                {
                    PushLog($"{target.Name} is beyond the chant.", Tone.Warn);
                    return false;
                }
                if (caster.Mana < 5)
                {
                    PushLog($"{caster.Name} lacks mana.", Tone.Warn);
                    return false;
                }
                int heal = 10 + SkillValue(caster.Skills, "mend") / 2;
                target.Hp = Mathf.Min(target.MaxHp, target.Hp + heal);
                caster.Mana -= 5;
                AddCastGlyph(caster, null, teal);
                ImproveSkill(caster, "mend", 2);
                AddFloat(target.X, target.Y, "+" + heal, teal);
                AddBurst(target.X, target.Y, teal);
                PushLog($"{caster.Name} mends {target.Name} for {heal}.", Tone.Good);
                PlaySfx("heal");
                return true;
            }

            if (target.Side != UnitSide.Enemy)
            {
                PushLog("That spell needs an enemy mark.", Tone.Warn);
                return false;
            }
            if (Distance(caster.X, caster.Y, target.X, target.Y) > 4)
            {
                PushLog($"{target.Name} is beyond the sigil.", Tone.Warn);
                return false;
            }
            if (!HasLineOfSight(caster.X, caster.Y, target.X, target.Y, true))
            {
                PushLog("Cover breaks the spell line.", Tone.Warn);
                AddFloat(target.X, target.Y, "cover", moss);
                PlaySfx("blocked");
                return false;
            }

            if (CasterKnowsSchool(caster.Spell, "ember"))
            {
                if (caster.Mana < 6)
                {
                    PushLog($"{caster.Name} lacks mana.", Tone.Warn);
                    return false;
                }
                int damage = 10 + SkillValue(caster.Skills, "ember") / 2 + rng.Next(0, 6);
                caster.Mana -= 6;
                AddCastGlyph(caster, null, ember);
                AddBeam(caster.X, caster.Y, target.X, target.Y, ember, "fire");
                int dealt = DealDamage(target, damage, "fire", ember);
                ImproveSkill(caster, "ember", 2);
                PushLog($"{caster.Name} casts ember for {dealt}.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
                PlaySfx("spell");
                return true;
            }

            if (caster.Mana < 5)
            {
                PushLog($"{caster.Name} lacks mana.", Tone.Warn);
                return false;
            }
            int hexDamage = 6 + SkillValue(caster.Skills, "hex") / 3 + rng.Next(0, 5);
            caster.Mana -= 5;
            AddCastGlyph(caster, null, gold);
            AddBeam(caster.X, caster.Y, target.X, target.Y, gold, "hex");
            DealDamage(target, hexDamage, "mind", gold);
            target.Hexed = 2;
            ImproveSkill(caster, "hex", 2);
            PushLog($"{caster.Name} hexes {target.Name}.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
            PlaySfx("spell", 0.85f);
            return true;
        }

        private bool CastFormula(CombatUnit caster, string code, CombatUnit target, int x, int y)
        {
            FormulaDef formula = GetFormula(code);
            string reason;
            if (!CanUseFormula(caster, code, out reason))
            {
                PushLog(reason, Tone.Warn);
                PlaySfx("blocked");
                return false;
            }
            if (formula == null)
            {
                PushLog(code + " has no stable shape.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }
            int range = EffectiveFormulaRange(formula, caster);
            if (Distance(caster.X, caster.Y, x, y) > range)
            {
                PushLog($"{formula.Name} cannot reach beyond range {range}.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }
            if (!CanTargetFormula(formula, caster, target, x, y))
            {
                PushLog(FormulaTargetPrompt(formula), Tone.Warn);
                PlaySfx("blocked");
                return false;
            }
            if (!HasFormulaLineOfSight(formula, caster, x, y))
            {
                PushLog(FormulaSightBlockText(formula), Tone.Warn);
                AddFloat(x, y, "cover", moss);
                PlaySfx("blocked");
                return false;
            }
            int manaCost = EffectiveFormulaMana(formula, caster);
            if (caster.Mana < manaCost)
            {
                PushLog($"{caster.Name} lacks mana.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }

            bool focused = IsFocusedCaster(caster);
            caster.Mana -= manaCost;
            string skill = FormulaSkill(formula, caster);
            AddCastGlyph(caster, formula, FormulaColor(formula));
            bool success = ResolveFormula(formula, caster, target, x, y);
            if (!success)
            {
                caster.Mana += manaCost;
                PlaySfx("blocked");
                return false;
            }
            ImproveSkill(caster, skill, formula.Splash ? 3 : 2);
            if (focused) AddFloat(caster.X, caster.Y, "focused", gold);
            PlayFormulaSfx(formula);
            return true;
        }

        private void PlayFormulaSfx(FormulaDef formula)
        {
            if (formula == null)
            {
                PlaySfx("spell", 0.95f);
                return;
            }

            if (formula.Code == "MTR")
            {
                PlaySfx("spell", 0.95f);
                PlaySfx("fire", 1.18f);
                PlaySfx("crit", 0.78f);
                return;
            }

            if (formula.Code == "FBL")
            {
                PlaySfx("spell", 0.85f);
                PlaySfx("fire", 1.08f);
                return;
            }

            PlaySfx(FormulaSfx(formula), formula.Code == "RLM" ? 1.15f : 0.95f);
        }

        private bool ResolveFormula(FormulaDef formula, CombatUnit caster, CombatUnit target, int x, int y)
        {
            if (formula.Effect == "summon")
            {
                if (!CanSummonAt(x, y)) return false;
                CombatUnit summon = MakeSummonedUnit(formula, caster, x, y);
                state.Combat.Units.Add(summon);
                AddBeam(caster.X, caster.Y, x, y, FormulaColor(formula), FormulaBeamKind(formula, caster, x, y));
                AddFloat(x, y, "bound", FormulaColor(formula));
                AddBurst(x, y, FormulaColor(formula));
                AddTileGlyph(x, y, formula, "impact", FormulaColor(formula));
                AddFlash(x, y, FormulaColor(formula));
                PushLog($"{caster.Name} binds {summon.Name} for {summon.SummonTurns} turns.", Tone.Good);
                return true;
            }

            if (formula.Effect == "terrain")
            {
                Point existing = ObstacleAt(x, y);
                string reaction = ApplyTerrainPlacementReaction(formula, caster, x, y, existing);
                state.Combat.Obstacles.RemoveAll(o => o.X == x && o.Y == y);
                state.Combat.Obstacles.Add(new Point(x, y, formula.Terrain, formula.Duration));
                AddBeam(caster.X, caster.Y, x, y, FormulaColor(formula), FormulaBeamKind(formula, caster, x, y));
                AddFloat(x, y, SpellFloatLabel(formula), FormulaColor(formula));
                AddBurst(x, y, FormulaColor(formula));
                AddTileGlyph(x, y, formula, "impact", FormulaColor(formula));
                AddFlash(x, y, FormulaColor(formula));
                PushLog($"{caster.Name} casts {formula.Name}. {TerrainDescription(formula.Terrain)}", Tone.Good);
                if (!string.IsNullOrEmpty(reaction)) PushLog(reaction, Tone.Good);
                return true;
            }

            if (formula.Effect == "heal")
            {
                int heal = formula.Power + SkillValue(caster.Skills, formula.Skill) / 2 + rng.Next(0, 5);
                target.Hp = Mathf.Min(target.MaxHp, target.Hp + heal);
                AddBeam(caster.X, caster.Y, target.X, target.Y, teal, FormulaBeamKind(formula, caster, target.X, target.Y));
                AddFloat(target.X, target.Y, "+" + heal, teal);
                AddBurst(target.X, target.Y, teal);
                AddTileGlyph(target.X, target.Y, formula, formula.Splash ? "area" : "impact", teal);
                AddFlash(target.X, target.Y, teal);
                int splashHeals = 0;
                if (formula.Splash)
                {
                    foreach (CombatUnit ally in state.Combat.Units.Where(u => u.Side == target.Side && u.Hp > 0 && u.Id != target.Id && Distance(u.X, u.Y, target.X, target.Y) <= 1))
                    {
                        int splashHeal = Mathf.Max(2, heal / 2);
                        ally.Hp = Mathf.Min(ally.MaxHp, ally.Hp + splashHeal);
                        AddFloat(ally.X, ally.Y, "+" + splashHeal, teal);
                        AddBurst(ally.X, ally.Y, teal);
                        AddTileGlyph(ally.X, ally.Y, formula, "impact", teal);
                        splashHeals++;
                    }
                }
                PushLog($"{caster.Name} casts {formula.Name}. {target.Name} recovers {heal}.", Tone.Good);
                if (splashHeals > 0) PushLog("The mend rings through nearby allies.", Tone.Good);
                return true;
            }

            if (formula.Effect == "cure")
            {
                target.Poisoned = 0;
                target.Bleeding = 0;
                target.Webbed = 0;
                target.Stunned = 0;
                target.Sleeping = 0;
                target.Hexed = 0;
                AddBeam(caster.X, caster.Y, target.X, target.Y, teal, FormulaBeamKind(formula, caster, target.X, target.Y));
                AddFloat(target.X, target.Y, "cleansed", teal);
                AddBurst(target.X, target.Y, teal);
                AddTileGlyph(target.X, target.Y, formula, "impact", teal);
                AddFlash(target.X, target.Y, teal);
                PushLog($"{caster.Name} casts {formula.Name}. {target.Name} is cleansed.", Tone.Good);
                return true;
            }

            if (formula.Effect == "status")
            {
                if (target != null) AddBeam(caster.X, caster.Y, target.X, target.Y, FormulaColor(formula), FormulaBeamKind(formula, caster, target.X, target.Y));
                if (target != null) AddTileGlyph(target.X, target.Y, formula, formula.Splash ? "area" : "impact", FormulaColor(formula));
                bool applied = TryApplyStatus(target, formula.Status, formula.Duration, caster, 0.86f, formula.Target == "enemy");
                int splashApplied = 0;
                if (formula.Splash && target != null)
                {
                    foreach (CombatUnit unit in state.Combat.Units.Where(u => u.Side == target.Side && u.Hp > 0 && u.Id != target.Id && Distance(u.X, u.Y, target.X, target.Y) <= 1))
                    {
                        if (TryApplyStatus(unit, formula.Status, Mathf.Max(1, formula.Duration - 1), caster, 0.72f, formula.Target == "enemy")) splashApplied++;
                        AddBeam(target.X, target.Y, unit.X, unit.Y, FormulaColor(formula), "arc");
                        AddTileGlyph(unit.X, unit.Y, formula, "impact", FormulaColor(formula));
                    }
                }
                string result = target == null ? "" : applied ? $"{target.Name} is {StatusLabel(formula.Status)}." : $"{target.Name} resists.";
                PushLog($"{caster.Name} casts {formula.Name}. {result}", applied ? Tone.Good : Tone.Warn);
                if (splashApplied > 0) PushLog($"The sign spreads to {splashApplied} nearby target{(splashApplied == 1 ? "" : "s")}.", Tone.Good);
                return true;
            }

            if (formula.Effect == "drain")
            {
                int damage = FormulaDamage(formula, caster);
                AddBeam(caster.X, caster.Y, target.X, target.Y, FormulaColor(formula), FormulaBeamKind(formula, caster, target.X, target.Y));
                AddTileGlyph(target.X, target.Y, formula, "impact", FormulaColor(formula));
                int dealt = DealDamage(target, damage, formula.DamageType, FormulaColor(formula));
                int heal = Mathf.Max(2, dealt / 2);
                caster.Hp = Mathf.Min(caster.MaxHp, caster.Hp + heal);
                AddFloat(caster.X, caster.Y, "+" + heal, violet);
                PushLog($"{caster.Name} casts {formula.Name}. Life pulls loose from {target.Name}.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
                if (target.Hp <= 0) PushLog($"{target.Name} is down.", Tone.Good);
                if (target.Hp <= 0) AddFlash(target.X, target.Y, gold);
                return true;
            }

            if (formula.Effect == "damage")
            {
                int damage = FormulaDamage(formula, caster);
                AddBeam(caster.X, caster.Y, target.X, target.Y, FormulaColor(formula), FormulaBeamKind(formula, caster, target.X, target.Y));
                AddTileGlyph(target.X, target.Y, formula, formula.Splash ? "area" : "impact", FormulaColor(formula));
                DealDamage(target, damage, formula.DamageType, FormulaColor(formula));
                string terrainReaction = ApplyFormulaHitTerrainReaction(formula, caster, target);
                bool statusApplied = false;
                if (!string.IsNullOrEmpty(formula.Status) && target.Hp > 0)
                {
                    statusApplied = TryApplyStatus(target, formula.Status, formula.Duration, caster, 0.42f, true);
                }

                int splashCount = 0;
                if (formula.Splash)
                {
                    foreach (CombatUnit enemy in state.Combat.Units.Where(u => u.Side == UnitSide.Enemy && u.Hp > 0 && u.Id != target.Id).ToList())
                    {
                        if (Distance(enemy.X, enemy.Y, target.X, target.Y) > 1) continue;
                        AddBeam(target.X, target.Y, enemy.X, enemy.Y, FormulaColor(formula), "arc");
                        AddTileGlyph(enemy.X, enemy.Y, formula, "impact", FormulaColor(formula));
                        DealDamage(enemy, Mathf.Max(3, damage / 3 + rng.Next(0, 3)), formula.DamageType, FormulaColor(formula));
                        splashCount++;
                    }
                }

                if (formula.Code == "MTR")
                {
                    AddMeteorShowerFlourish(caster, target, formula);
                }
                else if (formula.Code == "FBL")
                {
                    AddFireballFlourish(caster, target, formula);
                }

                PushLog($"{caster.Name} casts {formula.Name}. {target.Name} takes the mark.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
                if (!string.IsNullOrEmpty(formula.Status) && target.Hp > 0) PushLog(statusApplied ? $"{target.Name} is {StatusLabel(formula.Status)}." : $"{target.Name} resists the lingering sign.", statusApplied ? Tone.Good : Tone.Warn);
                if (!string.IsNullOrEmpty(terrainReaction)) PushLog(terrainReaction, Tone.Good);
                if (splashCount > 0) PushLog("The spell spills through nearby foes.", Tone.Good);
                if (target.Hp <= 0) PushLog($"{target.Name} is down.", Tone.Good);
                if (target.Hp <= 0) AddFlash(target.X, target.Y, gold);
                return true;
            }

            return false;
        }

        private void AddFireballFlourish(CombatUnit caster, CombatUnit target, FormulaDef formula)
        {
            if (caster == null || target == null || formula == null) return;
            Color color = FormulaColor(formula);
            AddBeam(caster.X, caster.Y, target.X, target.Y, color, "fireball");
            AddTileGlyph(target.X, target.Y, formula, "fireball", color);
            AddBurst(target.X, target.Y, color);
            AddEpicBurst(target.X, target.Y, Color.Lerp(color, gold, 0.45f), 18, 1.35f);
            AddFlash(target.X, target.Y, color);
            AddFloat(target.X, target.Y, "fireball", color);
        }

        private void AddMeteorShowerFlourish(CombatUnit caster, CombatUnit target, FormulaDef formula)
        {
            if (caster == null || target == null || formula == null) return;
            Color color = FormulaColor(formula);
            int[][] offsets =
            {
                new[] { 0, 0 },
                new[] { -1, 0 },
                new[] { 1, 0 },
                new[] { 0, -1 },
                new[] { 0, 1 }
            };

            for (int i = 0; i < offsets.Length; i++)
            {
                int tx = Mathf.Clamp(target.X + offsets[i][0], 0, CombatW - 1);
                int ty = Mathf.Clamp(target.Y + offsets[i][1], 0, CombatH - 1);
                AddBeam(Mathf.Clamp(tx - 2, 0, CombatW - 1), 0, tx, ty, color, i == 0 ? "meteor" : "meteor-small");
                AddTileGlyph(tx, ty, formula, i == 0 ? "meteor" : "impact", color);
                AddFlash(tx, ty, color);
                AddBurst(tx, ty, color);
                AddEpicBurst(tx, ty, Color.Lerp(color, gold, i == 0 ? 0.65f : 0.35f), i == 0 ? 24 : 12, i == 0 ? 1.65f : 1.05f);
            }
            AddFloat(target.X, target.Y, "meteor", color);
        }

        private CombatUnit MakeSummonedUnit(FormulaDef formula, CombatUnit caster, int x, int y)
        {
            string role = string.IsNullOrWhiteSpace(formula.SummonRole) ? "boundimp" : formula.SummonRole;
            int skill = Mathf.Max(1, SkillValue(caster.Skills, FormulaSkill(formula, caster)));
            int focus = IsFocusedCaster(caster) ? 1 : 0;
            int hp = 8 + state.Depth + skill / 3 + focus * 2;
            int power = Mathf.Max(3, formula.Power + skill / 5 + focus);
            return new CombatUnit
            {
                Id = Guid.NewGuid().ToString("N"),
                PartyIndex = -1,
                Side = UnitSide.Party,
                Name = role == "boundimp" ? "Bound Imp" : "Bound Demon",
                Role = role,
                Race = "demon",
                ClassKey = "summon",
                Rank = "summoned",
                Origin = "pact",
                Sigil = "flame",
                X = x,
                Y = y,
                Hp = hp,
                MaxHp = hp,
                Mana = 0,
                MaxMana = 0,
                Movement = 3,
                Power = power,
                Defense = 1 + skill / 12,
                Agility = 5 + skill / 8,
                Range = 1,
                AttackSpeed = 10 + skill / 6,
                DamageMin = Mathf.Max(2, power - 3),
                DamageMax = Mathf.Max(4, power + 1),
                Spell = "",
                Skills = new SkillSet { Arms = Mathf.Max(5, skill / 2), Guard = 2 }.Normalize(),
                Color = Hex("b94b56").ToHex(),
                DamageType = "death",
                WeaponName = "pact claws",
                WeaponBonus = 0,
                ArmorName = "bound hide",
                ArmorBonus = 0,
                Resist = "death|mind",
                Weakness = "light",
                StatusOnHit = "hex",
                MagicResist = 2,
                Fearless = true,
                Summoned = true,
                SummonTurns = Mathf.Max(1, formula.Duration + focus),
                SummonerId = caster.Id
            };
        }

        private int FormulaDamage(FormulaDef formula, CombatUnit caster)
        {
            string skill = FormulaSkill(formula, caster);
            int skillBonus = SkillValue(caster.Skills, skill) / 2;
            int focusBonus = IsFocusedCaster(caster) ? 3 : 0;
            return Mathf.Max(1, formula.Power + skillBonus + focusBonus + RaceFormulaPowerBonus(caster, formula) + rng.Next(0, 6));
        }

        private bool IsKnownFormula(string code)
        {
            return GetFormula(code) != null;
        }

        private string SpellFloatLabel(FormulaDef formula)
        {
            if (formula == null || string.IsNullOrEmpty(formula.Name)) return "spell";
            string first = formula.Name.Split(' ')[0];
            return first.Length <= 8 ? first : first.Substring(0, 8);
        }

        private bool IsFocusedCaster(CombatUnit caster)
        {
            return caster != null
                && state?.Combat != null
                && caster.Side == UnitSide.Party
                && state.Combat.ActionAvailable
                && !state.Combat.Moved
                && state.Combat.MovePoints >= UnitMoveAllowance(caster)
                && !string.IsNullOrEmpty(caster.Spell);
        }

        private int UnitMoveAllowance(CombatUnit unit)
        {
            if (unit == null) return CombatMoveAllowance;
            return Mathf.Clamp(unit.Movement > 0 ? unit.Movement : CombatMoveAllowance, 2, 5);
        }

        private int RaceHitBonus(CombatUnit unit)
        {
            if (unit == null || unit.Side != UnitSide.Party) return 0;
            if (string.Equals(unit.Race, "dusk elf", StringComparison.OrdinalIgnoreCase)) return 5;
            if (string.Equals(unit.Race, "fenkin", StringComparison.OrdinalIgnoreCase) && unit.Range > 1) return 2;
            return 0;
        }

        private int RaceDamageReduction(CombatUnit target, string damageType)
        {
            if (target == null || target.Side != UnitSide.Party) return 0;
            if (string.Equals(target.Race, "stoneborn", StringComparison.OrdinalIgnoreCase) && (string.IsNullOrEmpty(damageType) || damageType == "physical")) return 1;
            if (string.Equals(target.Race, "ashling", StringComparison.OrdinalIgnoreCase) && damageType == "fire") return 1;
            return 0;
        }

        private int RaceFormulaPowerBonus(CombatUnit caster, FormulaDef formula)
        {
            if (caster == null || formula == null || caster.Side != UnitSide.Party) return 0;
            if (string.Equals(caster.Race, "ashling", StringComparison.OrdinalIgnoreCase) && (formula.DamageType == "fire" || formula.Terrain == "fire")) return 2;
            if (string.Equals(caster.Race, "fenkin", StringComparison.OrdinalIgnoreCase) && (formula.Terrain == "web" || formula.Terrain == "gas" || formula.DamageType == "poison")) return 1;
            return 0;
        }

        private int EffectiveFormulaMana(FormulaDef formula, CombatUnit caster)
        {
            if (formula == null) return 0;
            return Mathf.Max(1, formula.Mana - (IsFocusedCaster(caster) ? 1 : 0));
        }

        private int EffectiveFormulaRange(FormulaDef formula, CombatUnit caster)
        {
            if (formula == null) return 0;
            return formula.Range + (IsFocusedCaster(caster) ? 1 : 0);
        }

        private string FormulaFocusNote(FormulaDef formula, CombatUnit caster)
        {
            if (!IsFocusedCaster(caster) || formula == null) return "";
            return $" / focused: {EffectiveFormulaMana(formula, caster)} MP, +1 range";
        }

        private bool CanUseFormula(CombatUnit caster, string code, out string reason)
        {
            reason = "";
            FormulaDef formula = GetFormula(code);
            if (caster == null)
            {
                reason = "No caster is ready.";
                return false;
            }
            if (string.IsNullOrEmpty(caster.Spell))
            {
                reason = $"{caster.Name} has no spell craft.";
                return false;
            }
            if (formula == null)
            {
                reason = code + " has no stable shape.";
                return false;
            }
            if (!SchoolMatches(formula, caster.Spell))
            {
                reason = $"{formula.Name} needs a different craft.";
                return false;
            }
            return true;
        }

        private bool CastEmptyTile(CombatUnit caster, int x, int y)
        {
            if (string.IsNullOrEmpty(caster.Spell))
            {
                PushLog($"{caster.Name} knows no battle spell.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }
            if (!CasterKnowsSchool(caster.Spell, "mend"))
            {
                PushLog("That spell needs a living mark.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }
            if (Distance(caster.X, caster.Y, x, y) > 4)
            {
                PushLog("Tree Cover cannot reach that far.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }
            if (!CanGrowTreeAt(x, y))
            {
                PushLog("Tree Cover needs an open tile.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }
            if (caster.Mana < 7)
            {
                PushLog($"{caster.Name} lacks mana.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }

            caster.Mana -= 7;
            state.Combat.Obstacles.RemoveAll(o => o.X == x && o.Y == y);
            state.Combat.Obstacles.Add(new Point(x, y, "tree", SummonedTreeDuration));
            ImproveSkill(caster, "mend", 2);
            AddFloat(x, y, "Tree", moss);
            AddBurst(x, y, moss);
            PushLog($"{caster.Name} casts Tree Cover. Cover rises for {SummonedTreeDuration} turns.", Tone.Good);
            PlaySfx("tree");
            return true;
        }

        private FormulaDef GetFormula(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;
            return formulaBook.FirstOrDefault(f => f.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<FormulaDef> KnownFormulasFor(CombatUnit caster)
        {
            if (caster == null || string.IsNullOrEmpty(caster.Spell)) return Enumerable.Empty<FormulaDef>();
            return formulaBook.Where(f => SchoolMatches(f, caster.Spell));
        }

        private bool SchoolMatches(FormulaDef formula, string school)
        {
            if (formula == null || string.IsNullOrEmpty(school)) return false;
            string[] formulaSchools = formula.School.Split('|');
            string[] casterSchools = school.Split('|');
            return formulaSchools.Any(f => casterSchools.Any(c => f.Equals(c, StringComparison.OrdinalIgnoreCase)));
        }

        private string FormulaSkill(FormulaDef formula, CombatUnit caster)
        {
            if (formula.Code == "RLM" && caster != null && CasterKnowsSchool(caster.Spell, "ember") && SkillValue(caster.Skills, "ember") >= SkillValue(caster.Skills, "hex")) return "ember";
            return string.IsNullOrEmpty(formula.Skill) ? caster?.Spell ?? "arms" : formula.Skill;
        }

        private string FormulaCodexLine(CombatUnit active)
        {
            if (active == null || string.IsNullOrEmpty(active.Spell)) return "No spell craft. Use Cast for trained spellcasters.";
            if (selectedAction != ActionMode.Cast) return "Press 3 or Cast to open the Spellbook. I opens Armory; C opens Spell Reference.";
            if (!string.IsNullOrEmpty(pendingFormulaCode)) return "Spell selected. Click a highlighted target, or Esc to clear.";
            return "Choose a spell card, then click a highlighted target.";
        }

        private bool CanTargetFormula(FormulaDef formula, CombatUnit caster, CombatUnit target, int x, int y)
        {
            if (formula == null || caster == null) return false;
            if (formula.Effect == "summon") return CanSummonAt(x, y);
            if (formula.Target == "tile") return CanPlaceTerrainAt(formula, x, y);
            if (formula.Target == "ally") return target != null && target.Side == UnitSide.Party;
            if (formula.Target == "enemy") return target != null && target.Side == UnitSide.Enemy;
            if (formula.Target == "self") return target != null && target.Id == caster.Id;
            return false;
        }

        private bool CanSummonAt(int x, int y)
        {
            return CanStandAt(x, y);
        }

        private bool IsFormulaActionable(FormulaDef formula, CombatUnit caster, CombatUnit target, int x, int y)
        {
            if (formula == null || caster == null) return false;
            if (Distance(caster.X, caster.Y, x, y) > EffectiveFormulaRange(formula, caster)) return false;
            if (!CanTargetFormula(formula, caster, target, x, y)) return false;
            if (!HasFormulaLineOfSight(formula, caster, x, y)) return false;
            return caster.Mana >= EffectiveFormulaMana(formula, caster);
        }

        private bool FormulaRequiresLineOfSight(FormulaDef formula)
        {
            if (formula == null || FormulaArcsOverCover(formula)) return false;
            return FormulaBaseRequiresLineOfSight(formula);
        }

        private bool FormulaBaseRequiresLineOfSight(FormulaDef formula)
        {
            if (formula == null) return false;
            if (formula.Target == "enemy") return true;
            if (formula.Target == "tile") return true;
            return false;
        }

        private bool FormulaArcsOverCover(FormulaDef formula)
        {
            return formula != null && (formula.Arc || (formula.Target == "enemy" && formula.Splash));
        }

        private bool HasFormulaLineOfSight(FormulaDef formula, CombatUnit caster, int x, int y)
        {
            if (caster == null) return true;
            if (FormulaCanArcOverCover(formula, x, y)) return true;
            if (!FormulaBaseRequiresLineOfSight(formula)) return true;
            return HasLineOfSight(caster.X, caster.Y, x, y, true);
        }

        private bool FormulaCanArcOverCover(FormulaDef formula, int x, int y)
        {
            if (!FormulaArcsOverCover(formula)) return false;
            if (formula.Code == "BTF")
            {
                Point existing = ObstacleAt(x, y);
                return IsBreakableCover(existing);
            }
            return true;
        }

        private string FormulaSightBlockText(FormulaDef formula)
        {
            if (formula != null && formula.Target == "tile") return "Cover hides that tile from the spell.";
            return "Cover breaks the spell line.";
        }

        private string FormulaTargetPrompt(FormulaDef formula)
        {
            if (formula == null) return "That spell has no target.";
            if (formula.Target == "tile") return $"{formula.Name} needs an open tile.";
            if (formula.Target == "ally") return $"{formula.Name} needs an ally.";
            if (formula.Target == "enemy") return $"{formula.Name} needs an enemy mark.";
            return $"{formula.Name} cannot find a target.";
        }

        private bool CanPlaceTerrainAt(FormulaDef formula, int x, int y)
        {
            if (formula == null || x < 0 || x >= CombatW || y < 0 || y >= CombatH) return false;
            if (UnitAt(x, y) != null) return false;
            Point existing = ObstacleAt(x, y);
            if (existing == null) return true;
            if (formula.Terrain == "fire" && existing.Kind == "tree") return true;
            return !IsBlockingTerrain(existing);
        }

        private string TerrainReactionPreview(FormulaDef formula, int x, int y)
        {
            string line = TerrainPreviewLine(new Point(x, y, formula?.Terrain)).Trim();
            Point existing = ObstacleAt(x, y);
            if (formula != null && formula.Terrain == "tree")
            {
                line = $"tree cover: {CoverMaxIntegrity("tree")} integrity, {SummonedTreeDuration} turns / arcs pass / foes break";
            }
            if (formula == null || existing == null) return line;
            string prefix = string.IsNullOrEmpty(line) ? "" : line + " / ";
            if (formula.Terrain == "fire" && existing.Kind == "tree") return prefix + "burns tree cover";
            if (formula.Terrain == "fire" && (existing.Kind == "gas" || existing.Kind == "web")) return prefix + "ignites hazard";
            if (formula.Terrain == "ice" && existing.Kind == "fire") return prefix + "quenches fire into steam";
            if (formula.Terrain == "fire" && existing.Kind == "ice") return prefix + "melts ice";
            return line;
        }

        private string ApplyTerrainPlacementReaction(FormulaDef formula, CombatUnit caster, int x, int y, Point existing)
        {
            if (formula == null || existing == null) return "";
            if (formula.Terrain == "fire" && existing.Kind == "tree")
            {
                DamageUnitsAround(x, y, 1, 3 + state.Depth / 2, "fire", FormulaColor(formula), caster);
                AddFloat(x, y, "burn cover", ember);
                AddFlash(x, y, ember);
                return "The cover burns, singeing anyone pressed close.";
            }
            if (formula.Terrain == "fire" && existing.Kind == "gas")
            {
                DamageUnitsAround(x, y, 1, 5 + state.Depth, "fire", FormulaColor(formula), caster);
                AddFloat(x, y, "flash", ember);
                AddBurst(x, y, ember);
                return "The gas catches in a sudden flash.";
            }
            if (formula.Terrain == "fire" && existing.Kind == "web")
            {
                DamageUnitsAround(x, y, 1, 4 + state.Depth / 2, "fire", FormulaColor(formula), caster);
                AddFloat(x, y, "web flare", ember);
                AddBurst(x, y, ember);
                return "The webbing flares and collapses into flame.";
            }
            if (formula.Terrain == "ice" && existing.Kind == "fire")
            {
                AddFloat(x, y, "steam", frost);
                AddBurst(x, y, frost);
                foreach (CombatUnit unit in UnitsAround(x, y, 1))
                {
                    TryApplyStatus(unit, "stun", 1, caster, 0.24f, unit.Side != caster.Side);
                }
                return "Ice quenches the fire into blinding steam.";
            }
            if (formula.Terrain == "fire" && existing.Kind == "ice")
            {
                AddFloat(x, y, "melt", ember);
                AddBurst(x, y, ember);
                return "The ice hisses away under the flame.";
            }
            return "";
        }

        private string ApplyFormulaHitTerrainReaction(FormulaDef formula, CombatUnit caster, CombatUnit target)
        {
            if (formula == null || target == null) return "";
            Point terrain = ObstacleAt(target.X, target.Y);
            if (terrain == null) return "";
            if (formula.DamageType == "fire" && (terrain.Kind == "gas" || terrain.Kind == "web"))
            {
                state.Combat.Obstacles.Remove(terrain);
                DamageUnitsAround(target.X, target.Y, 1, terrain.Kind == "gas" ? 5 + state.Depth : 3 + state.Depth / 2, "fire", FormulaColor(formula), caster);
                AddFloat(target.X, target.Y, terrain.Kind == "gas" ? "flash" : "flare", ember);
                AddBurst(target.X, target.Y, ember);
                return terrain.Kind == "gas" ? "The gas ignites around the mark." : "The webbing burns away around the mark.";
            }
            if (formula.DamageType == "cold" && terrain.Kind == "fire")
            {
                state.Combat.Obstacles.Remove(terrain);
                state.Combat.Obstacles.Add(new Point(target.X, target.Y, "ice", 8));
                TryApplyStatus(target, "stun", 1, caster, 0.28f, true);
                AddFloat(target.X, target.Y, "quenched", frost);
                return "Cold quenches the fire into a slick patch.";
            }
            if (formula.DamageType == "shock" && (terrain.Kind == "ice" || terrain.Kind == "gas" || terrain.Kind == "web"))
            {
                TryApplyStatus(target, "stun", 1, caster, 0.34f, true);
                foreach (CombatUnit unit in UnitsAround(target.X, target.Y, 1).Where(u => u.Id != target.Id))
                {
                    TryApplyStatus(unit, "stun", 1, caster, 0.18f, unit.Side != caster.Side);
                }
                AddFloat(target.X, target.Y, "arc", gold);
                AddBurst(target.X, target.Y, gold);
                return "The hazard carries the shock outward.";
            }
            return "";
        }

        private IEnumerable<CombatUnit> UnitsAround(int x, int y, int radius)
        {
            if (state?.Combat?.Units == null) yield break;
            foreach (CombatUnit unit in state.Combat.Units)
            {
                if (unit.Hp <= 0) continue;
                if (Distance(unit.X, unit.Y, x, y) <= radius) yield return unit;
            }
        }

        private void DamageUnitsAround(int x, int y, int radius, int amount, string damageType, Color color, CombatUnit source)
        {
            foreach (CombatUnit unit in UnitsAround(x, y, radius).ToList())
            {
                DealDamage(unit, amount, damageType, color);
            }
        }

        private string FormulaSfx(FormulaDef formula)
        {
            if (formula == null) return "spell";
            if (formula.Effect == "terrain")
            {
                if (formula.Terrain == "tree") return "tree";
                if (formula.Terrain == "stone") return "stone";
                if (formula.Terrain == "fire") return "fire";
                if (formula.Terrain == "ice") return "ice";
            if (formula.Terrain == "web") return "web";
            if (formula.Terrain == "gas") return "poison";
        }
            if (formula.Effect == "summon") return "death";
            if (formula.Code == "FBL" || formula.Code == "MTR") return "fire";
            if (formula.DamageType == "death") return "death";
            if (formula.DamageType == "shock") return formula.Splash ? "arc" : "shock";
            if (formula.DamageType == "light") return "light";
            if (formula.DamageType == "mind" || formula.Status == "hex" || formula.Status == "sleep") return "curse";
            if (formula.Splash) return "arc";
            if (formula.Effect == "heal" || formula.Effect == "cure") return "heal";
            if (formula.Status == "regen" || formula.Status == "shield") return "ward";
            return "spell";
        }

        private string FormulaBeamKind(FormulaDef formula, CombatUnit caster, int x, int y)
        {
            if (formula == null || caster == null) return "spell";
            if (formula.Code == "MTR") return "meteor";
            if (formula.Code == "FBL") return "fireball";
            if (formula.DamageType == "shock") return "arc";
            if (FormulaCanArcOverCover(formula, x, y) && !HasLineOfSight(caster.X, caster.Y, x, y, true)) return "arc";
            if (formula.Effect == "summon") return "death";
            if (formula.Effect == "heal" || formula.Effect == "cure" || formula.Status == "regen" || formula.Status == "shield") return "heal";
            if (formula.DamageType == "death") return "death";
            if (formula.DamageType == "fire" || formula.Terrain == "fire") return "fire";
            if (formula.DamageType == "cold" || formula.Terrain == "ice") return "ice";
            if (formula.DamageType == "mind" || formula.DamageType == "poison" || formula.Status == "hex" || formula.Status == "sleep" || formula.Terrain == "web" || formula.Terrain == "gas") return "hex";
            return "spell";
        }

        private Color FormulaColor(FormulaDef formula, float alpha = 1f)
        {
            if (formula == null) return Hex("d7a84e", alpha);
            if (formula.Effect == "summon") return Hex("b94b56", alpha);
            if (formula.Terrain == "tree") return Hex("7f9d5b", alpha);
            if (formula.Terrain == "stone") return Hex("9aa09a", alpha);
            if (formula.Terrain == "fire" || formula.DamageType == "fire") return Hex("c65c3b", alpha);
            if (formula.Terrain == "ice" || formula.DamageType == "cold") return Hex("9ad6e8", alpha);
            if (formula.Terrain == "web" || formula.Terrain == "gas" || formula.DamageType == "poison") return Hex("8fc27b", alpha);
            if (formula.DamageType == "death") return Hex("b94b56", alpha);
            if (formula.DamageType == "shock") return Hex("d7a84e", alpha);
            if (formula.DamageType == "light") return Hex("97dbc2", alpha);
            if (formula.Effect == "heal" || formula.Effect == "cure") return Hex("58b7a5", alpha);
            return Hex("8d6dcc", alpha);
        }

        private string TerrainDescription(string terrain)
        {
            if (terrain == "tree") return $"A breakable tree blocks movement and direct shots for {SummonedTreeDuration} turns.";
            if (terrain == "stone") return "A breakable stone block shoulders out of the floor.";
            if (terrain == "fire") return "Fire crawls across the stones.";
            if (terrain == "ice") return "A slick sheet of ice flashes into being.";
            if (terrain == "web") return "Sticky threads lace the floor.";
            if (terrain == "gas") return "A venom haze coils low to the ground.";
            return "The floor changes shape.";
        }

        private int DealDamage(CombatUnit target, int amount, string damageType, Color color)
        {
            if (target == null || target.Hp <= 0) return 0;
            string type = string.IsNullOrEmpty(damageType) ? "physical" : damageType;
            float multiplier = 1f;
            if (HasTag(target.Resist, type)) multiplier *= 0.55f;
            if (HasTag(target.Weakness, type)) multiplier *= 1.45f;
            if (target.Hexed > 0) multiplier *= 1.20f;
            int guard = target.Guarding ? Mathf.Max(2, target.GuardBonus) : 0;
            int shield = target.Shielded > 0 ? 3 : 0;
            int damage = Mathf.Max(1, Mathf.RoundToInt(amount * multiplier) - guard - shield - GearDamageReduction(target, type) - RaceDamageReduction(target, type));
            target.Hp = Mathf.Max(0, target.Hp - damage);
            if (damage > 0 && target.Sleeping > 0)
            {
                target.Sleeping = 0;
                AddFloat(target.X, target.Y, "wakes", muted);
            }
            if (multiplier < 0.9f) AddFloat(target.X, target.Y, "resist", muted);
            if (multiplier > 1.1f) AddFloat(target.X, target.Y, "weak", gold);
            AddFloat(target.X, target.Y, "-" + damage, color);
            AddBurst(target.X, target.Y, color);
            AddFlash(target.X, target.Y, color);
            return damage;
        }

        private Color DamageColor(string damageType)
        {
            if (damageType == "fire") return ember;
            if (damageType == "cold") return frost;
            if (damageType == "shock") return gold;
            if (damageType == "poison") return poison;
            if (damageType == "death") return blood;
            if (damageType == "mind") return violet;
            if (damageType == "light") return teal;
            return blood;
        }

        private int WeaponPowerBonus(string weaponName)
        {
            string text = (weaponName ?? "").ToLowerInvariant();
            int bonus = 0;
            if (text.Contains("broadsword") || text.Contains("war hammer") || text.Contains("war flail") || text.Contains("halberd")) bonus++;
            if (text.Contains("vicious") || text.Contains("vampiric") || text.Contains("death")) bonus++;
            if (text.Contains("crude")) bonus--;
            return Mathf.Clamp(bonus, -1, 3);
        }

        private int WeaponHitBonus(string weaponName)
        {
            string text = (weaponName ?? "").ToLowerInvariant();
            int bonus = 0;
            if (text.Contains("epee") || text.Contains("sabre") || text.Contains("balanced") || text.Contains("elven")) bonus += 5;
            if (text.Contains("crossbow")) bonus += 4;
            if (text.Contains("war hammer") || text.Contains("tower")) bonus -= 3;
            if (text.Contains("crude")) bonus -= 5;
            return bonus;
        }

        private int ArmorAgilityModifier(string armorName)
        {
            string text = (armorName ?? "").ToLowerInvariant();
            int mod = 0;
            if (text.Contains("plate") || text.Contains("tower") || text.Contains("chain") || text.Contains("mail")) mod -= 1;
            if (text.Contains("weightless") || text.Contains("silk") || text.Contains("mantle") || text.Contains("cloak") || text.Contains("leathers")) mod += 1;
            return Mathf.Clamp(mod, -2, 2);
        }

        private int GearGuardBonus(CombatUnit unit)
        {
            string armor = (unit?.ArmorName ?? "").ToLowerInvariant();
            string weapon = (unit?.WeaponName ?? "").ToLowerInvariant();
            int bonus = 0;
            if (armor.Contains("tower shield") || armor.Contains("kite shield")) bonus += 2;
            if (armor.Contains("buckler") || weapon.Contains("ward shield")) bonus++;
            if (armor.Contains("warding") || armor.Contains("guarding") || armor.Contains("anti-magic")) bonus++;
            if (armor.Contains("robe") && !string.IsNullOrEmpty(unit?.Spell)) bonus++;
            return Mathf.Clamp(bonus, 0, 4);
        }

        private int GearDamageReduction(CombatUnit target, string damageType)
        {
            string armor = (target?.ArmorName ?? "").ToLowerInvariant();
            string type = (damageType ?? "").ToLowerInvariant();
            int reduction = 0;
            if ((armor.Contains("warding") || armor.Contains("anti-magic") || armor.Contains("moonstone")) && type != "physical") reduction++;
            if ((armor.Contains("plate") || armor.Contains("tower shield")) && type == "physical") reduction++;
            if (armor.Contains("thorns") && type == "physical") reduction++;
            return Mathf.Clamp(reduction, 0, 3);
        }

        private string GearOnHitStatus(string weaponName)
        {
            string text = (weaponName ?? "").ToLowerInvariant();
            if (text.Contains("stunning") || text.Contains("storm") || text.Contains("war hammer")) return "stun";
            if (text.Contains("bleeding") || text.Contains("vicious") || text.Contains("epee") || text.Contains("sabre") || text.Contains("thorns")) return "bleed";
            if (text.Contains("venom")) return "poison";
            if (text.Contains("terror") || text.Contains("silence")) return "sleep";
            return "";
        }

        private float GearOnHitChance(string weaponName)
        {
            string text = (weaponName ?? "").ToLowerInvariant();
            float chance = 0.20f;
            if (text.Contains("masterwork") || text.Contains("vicious") || text.Contains("stormglass")) chance += 0.10f;
            if (text.Contains("crude")) chance -= 0.06f;
            return Mathf.Clamp(chance, 0.08f, 0.42f);
        }

        private bool TryApplyStatus(CombatUnit target, string status, int duration, CombatUnit source, float chance, bool hostile)
        {
            if (target == null || target.Hp <= 0 || string.IsNullOrEmpty(status)) return false;
            float rollChance = StatusApplyChance(target, status, source, chance, hostile);
            if (hostile && rng.NextDouble() > rollChance)
            {
                AddFloat(target.X, target.Y, "resist", muted);
                PlaySfx("resist", 0.34f);
                return false;
            }

            int turns = Mathf.Max(1, duration);
            Color color = violet;
            if (status == "poison")
            {
                target.Poisoned = Mathf.Max(target.Poisoned, turns);
                color = poison;
            }
            else if (status == "bleed")
            {
                target.Bleeding = Mathf.Max(target.Bleeding, turns);
                color = blood;
            }
            else if (status == "stun")
            {
                target.Stunned = Mathf.Max(target.Stunned, turns);
                color = gold;
            }
            else if (status == "sleep")
            {
                target.Sleeping = Mathf.Max(target.Sleeping, turns);
                color = violet;
            }
            else if (status == "shield")
            {
                target.Shielded = Mathf.Max(target.Shielded, turns);
                color = teal;
            }
            else if (status == "regen")
            {
                target.Regenerating = Mathf.Max(target.Regenerating, turns);
                color = teal;
            }
            else if (status == "web")
            {
                target.Webbed = Mathf.Max(target.Webbed, turns);
                color = poison;
            }
            else if (status == "hex")
            {
                target.Hexed = Mathf.Max(target.Hexed, turns);
                color = violet;
            }
            else return false;

            AddFloat(target.X, target.Y, StatusLabel(status), color);
            AddBurst(target.X, target.Y, color);
            AddFlash(target.X, target.Y, color);
            AddTileGlyph(target.X, target.Y, null, "status", color);
            PlaySfx(StatusSfx(status), 0.42f);
            return true;
        }

        private string StatusSfx(string status)
        {
            if (status == "poison") return "poison";
            if (status == "bleed") return "blade";
            if (status == "stun") return "shock";
            if (status == "sleep" || status == "hex") return "curse";
            if (status == "shield" || status == "regen") return "ward";
            if (status == "web") return "web";
            return "status";
        }

        private string StatusLabel(string status)
        {
            if (status == "poison") return "poisoned";
            if (status == "bleed") return "bleeding";
            if (status == "stun") return "stunned";
            if (status == "sleep") return "sleeping";
            if (status == "shield") return "warded";
            if (status == "regen") return "regen";
            if (status == "web") return "webbed";
            if (status == "hex") return "hexed";
            return status;
        }

        private bool HasTag(string list, string tag)
        {
            if (string.IsNullOrEmpty(list) || string.IsNullOrEmpty(tag)) return false;
            return list.Split('|').Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
        }

        private void UseElixir()
        {
            if (state.Elixirs <= 0)
            {
                PushLog("No elixirs remain.", Tone.Warn);
                PlaySfx("blocked");
                return;
            }
            if (state.Mode == GameMode.Combat)
            {
                CombatUnit active = CurrentUnit();
                if (active == null || active.Side != UnitSide.Party || !state.Combat.ActionAvailable) return;
                state.Elixirs--;
                active.Hp = Mathf.Min(active.MaxHp, active.Hp + 18);
                active.Mana = Mathf.Min(active.MaxMana, active.Mana + 6);
                AddFloat(active.X, active.Y, "elixir", teal);
                PushLog($"{active.Name} drinks an elixir.", Tone.Good);
                PlaySfx("heal", 0.8f);
                state.Combat.ActionAvailable = false;
                state.Combat.Acted = true;
                state.Combat.Phase = CombatPhase.Resolving;
                SyncPartyFromCombat();
                NextTurn();
                return;
            }
            PartyMember target = state.Party.Where(p => p.Hp > 0).OrderBy(p => (float)p.Hp / p.MaxHp).FirstOrDefault();
            if (target == null) return;
            state.Elixirs--;
            target.Hp = Mathf.Min(target.MaxHp, target.Hp + 18);
            target.Mana = Mathf.Min(target.MaxMana, target.Mana + 6);
            PushLog($"{target.Name} drinks an elixir.", Tone.Good);
            PlaySfx("heal", 0.8f);
        }

        private void SyncPartyFromCombat()
        {
            if (state.Combat == null) return;
            foreach (CombatUnit unit in state.Combat.Units.Where(u => u.Side == UnitSide.Party && u.PartyIndex >= 0))
            {
                PartyMember member = state.Party[unit.PartyIndex];
                member.Hp = Mathf.Clamp(unit.Hp, 0, member.MaxHp);
                member.Mana = Mathf.Clamp(unit.Mana, 0, member.MaxMana);
                member.Skills = unit.Skills.Clone();
            }
        }

        private void ImproveSkill(CombatUnit unit, string skill, int amount)
        {
            if (unit.Side != UnitSide.Party) return;
            int before = SkillValue(unit.Skills, skill);
            SetSkill(unit.Skills, skill, Mathf.Clamp(before + amount, 1, 99));
            if (unit.PartyIndex >= 0) state.Party[unit.PartyIndex].Skills = unit.Skills.Clone();
            int after = SkillValue(unit.Skills, skill);
            if (before < 15 && after >= 15) PushLog($"{unit.Name} is no longer lousy at {skill}.", Tone.Good);
            if (before < 30 && after >= 30) PushLog($"{unit.Name} becomes steady at {skill}.", Tone.Good);
        }

        private void SaveGame()
        {
            try
            {
                string path = SavePath();
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, JsonUtility.ToJson(state, true));
                PushLog("The current oath is saved.", Tone.Good);
                ShowBanner("Saved");
                PlaySfx("save");
            }
            catch (Exception ex)
            {
                PushLog("Save failed: " + ex.Message, Tone.Warn);
            }
        }

        private void LoadGame()
        {
            try
            {
                string path = SavePath();
                if (!File.Exists(path))
                {
                    PushLog("No saved oath is present.", Tone.Warn);
                    return;
                }
                GameState loaded = JsonUtility.FromJson<GameState>(File.ReadAllText(path));
                if (loaded == null || loaded.SaveVersion != SaveVersion) throw new InvalidDataException($"This beta scaffold needs a fresh v{SaveVersion} save.");
                state = loaded;
                state.SaveVersion = SaveVersion;
                NormalizeGameSettings();
                EnsurePartyCustomization();
                EnsureWorldState();
                EnsureCombatTurnState();
                if (state.Mode == GameMode.Explore && state.Map != null)
                {
                    EnsureWorldLandmarks();
                    lastExploreRegion = ExploreRegionName(state.PlayerX, state.PlayerY);
                }
                rng = new System.Random(state.Seed + state.Depth * 101);
                PushLog("The saved oath is restored.", Tone.Good);
                ShowBanner("Loaded");
                PlaySfx("save");
            }
            catch (Exception ex)
            {
                PushLog("Load failed: " + ex.Message, Tone.Warn);
            }
        }

        private void EnsureWorldState()
        {
            if (state == null) return;
            if (state.StoryChapter <= 0) state.StoryChapter = Mathf.Max(1, state.Depth);
            if (string.IsNullOrEmpty(state.ActiveStory)) state.ActiveStory = StoryObjectiveForDepth(Mathf.Max(1, state.Depth));
            if (state.DiscoveredZones == null) state.DiscoveredZones = new List<string>();
            if (state.Map != null && state.Map.Depth <= 0) state.Map.Depth = Mathf.Max(1, state.Depth);
        }

        private string SavePath()
        {
            return Path.Combine(Application.persistentDataPath, "AshenHallsSaveV2.json");
        }

        private void EnsureCombatTurnState()
        {
            if (state?.Combat == null) return;
            NormalizeCombatObstacles();
            CombatUnit active = CurrentUnit();
            if (active == null) return;
            state.Combat.ActionAvailable = !state.Combat.Acted;
            if (active.Stunned > 0 || active.Sleeping > 0) state.Combat.ActionAvailable = false;
            int allowance = UnitMoveAllowance(active);
            if (!state.Combat.Moved && state.Combat.MovePoints <= 0 && active.Webbed <= 0) state.Combat.MovePoints = allowance;
            state.Combat.MovePoints = Mathf.Clamp(state.Combat.MovePoints, 0, allowance);
            state.Combat.Phase = active.Side == UnitSide.Enemy ? CombatPhase.EnemyThinking : CombatPhase.ChooseAction;
            selectedAction = ActionMode.Attack;
            aiActAt = active.Side == UnitSide.Enemy ? Time.time + (state.ReducedMotion ? 0.05f : 0.45f) : -1f;
        }

        private void NormalizeCombatObstacles()
        {
            if (state?.Combat?.Obstacles == null) return;
            foreach (Point obstacle in state.Combat.Obstacles)
            {
                if (IsBreakableCover(obstacle) && obstacle.Integrity <= 0)
                {
                    obstacle.Integrity = CoverMaxIntegrity(obstacle.Kind);
                }
            }
        }

        private void ContextHelp()
        {
            if (state.Mode == GameMode.Combat)
            {
                PushLog("F1: each unit gets personal move and 1 action. Move can be spent in steps; attack, cast, guard, or elixir spends the action. Hover tiles for range, cover, and hit notes.", Tone.Good);
                PushLog("F1: press Cast to choose a spell card, then click a highlighted target. Standing still focuses casting: -1 MP, +1 range, and harder damage.", Tone.Good);
                PushLog("F1: priests shape trees/stone/wards/circle healing, mages shape fire/ice/shock, warlocks shape hex/death and bind fragile demons.", Tone.Good);
                PushLog($"F1: Tree Cover lasts {SummonedTreeDuration} turns, blocks arrows and direct bolts, but arcing spells pass over it. Enemies batter through cover that blocks their path or sight.", Tone.Good);
            }
            else if (state.Mode == GameMode.Explore)
            {
                PushLog($"F1: travel with arrows or WASD, visit {HomeTownName}, open caches, and descend by stairs.", Tone.Good);
                PushLog("F1: zones have danger ratings and story hooks. Hover map tiles to read zone purpose, object hints, and chapter context.", Tone.Good);
                PushLog("F1: press I for the Armory, or C for spell reference.", Tone.Good);
            }
            else
            {
                PushLog("F1: Quick Start begins immediately; custom muster keeps each hero at 50 points. Press I to inspect gear.", Tone.Good);
            }
            ShowBanner("Context help");
        }

        private void DrawDefeat()
        {
            boardRect = GetBoardRect();
            DrawPanel(boardRect);
            GUI.Label(new Rect(boardRect.x + 40, boardRect.y + 80, boardRect.width - 80, 50), "The company has fallen.", titleStyle);
            GUI.Label(new Rect(boardRect.x + 42, boardRect.y + 140, boardRect.width - 84, 60), "A new oath may yet be sworn. The old road waits.", labelStyle);
            if (GUI.Button(new Rect(boardRect.x + 42, boardRect.y + 220, 180, 48), "New Company", buttonStyle)) NewMuster();
        }

        private void DrawVictory()
        {
            boardRect = GetBoardRect();
            DrawRpgPanel(boardRect, gold);
            Rect hero = new Rect(boardRect.x + 46, boardRect.y + 52, boardRect.width - 92, 174);
            DrawRect(hero, Hex("080b0d", 0.86f));
            DrawBorder(hero, gold, 2);
            TryDrawQuestWorldAtlasIcon(new Rect(hero.xMax - 154, hero.y + 22, 116, 116), 19, Color.white.WithAlpha(0.88f));
            GUI.Label(new Rect(hero.x + 26, hero.y + 24, hero.width - 210, 42), "The Old Road Is Sealed", titleStyle);
            GUI.Label(new Rect(hero.x + 28, hero.y + 80, hero.width - 230, 54),
                "Vhal Rakh's meteor crown breaks above the ritual heart. Midgaard has one more dawn, and this beta route now has a complete ending.",
                labelStyle);

            Rect summary = new Rect(boardRect.x + 46, hero.yMax + 24, boardRect.width * 0.48f, 226);
            DrawRpgPanel(summary, teal);
            GUI.Label(new Rect(summary.x + 20, summary.y + 14, summary.width - 40, 24), "Company Ledger", h2Style);
            int living = state.Party.Count(p => p.Hp > 0);
            int level = state.Party.Count == 0 ? 1 : Mathf.RoundToInt((float)state.Party.Average(p => p.Level));
            string line = $"Survivors {living}/{state.Party.Count} / Avg level {level} / Gold {state.Gold} / Depth {state.Depth}";
            GUI.Label(new Rect(summary.x + 22, summary.y + 50, summary.width - 44, 24), line, CenterLeftStyle(13, gold));
            float y = summary.y + 84;
            foreach (PartyMember member in state.Party)
            {
                Rect row = new Rect(summary.x + 18, y, summary.width - 36, 30);
                DrawRect(row, Hex("151b20", 0.88f));
                DrawBorder(row, MemberColor(member).WithAlpha(0.66f), 1);
                GUI.Label(new Rect(row.x + 10, row.y + 4, row.width * 0.34f, 18), member.Name, CenterLeftStyle(12, ink));
                GUI.Label(new Rect(row.x + row.width * 0.36f, row.y + 4, row.width * 0.30f, 18), $"{DisplayClass(member.ClassKey)} L{member.Level}", CenterLeftStyle(11, muted));
                GUI.Label(new Rect(row.x + row.width * 0.66f, row.y + 4, row.width * 0.30f, 18), $"{BestSkillLabel(member)} {BestSkillValue(member)}", CenterLeftStyle(11, gold));
                y += 35;
            }

            Rect route = new Rect(summary.xMax + 22, hero.yMax + 24, boardRect.xMax - summary.xMax - 68, 226);
            DrawRpgPanel(route, violet);
            GUI.Label(new Rect(route.x + 20, route.y + 14, route.width - 40, 24), "Beta Route Complete", h2Style);
            string[] chapters =
            {
                "I  Midgaard Cisterns",
                "II Kobold Smoke",
                "III Bone Road",
                "IV Glass and Ash",
                "V  Red Gate",
                "VI Meteor Crown"
            };
            for (int i = 0; i < chapters.Length; i++)
            {
                Rect row = new Rect(route.x + 22, route.y + 50 + i * 24, route.width - 44, 20);
                DrawRect(new Rect(row.x, row.y + 8, 10, 4), i < chapters.Length - 1 ? teal : gold);
                GUI.Label(new Rect(row.x + 18, row.y, row.width - 18, row.height), chapters[i], CenterLeftStyle(11, i < chapters.Length - 1 ? muted : gold));
            }
            GUI.Label(new Rect(route.x + 22, route.yMax - 44, route.width - 44, 32), "Next passes can turn this scaffold into hand-authored dungeons, NPC quests, and multi-phase boss rules.", CenterLeftStyle(11, muted));

            Rect actions = new Rect(boardRect.x + 46, boardRect.yMax - 86, boardRect.width - 92, 56);
            if (GUI.Button(new Rect(actions.x, actions.y, 170, 46), "New Company", buttonStyle)) NewMuster();
            if (GUI.Button(new Rect(actions.x + 186, actions.y, 150, 46), "Tavern", buttonStyle))
            {
                state.Mode = GameMode.Tavern;
                PlaySfx("ui", 0.55f);
            }
            if (GUI.Button(new Rect(actions.x + 352, actions.y, 150, 46), "Beta Lab", buttonStyle)) StartBetaCombatLab();
        }

        private Rect GetBoardRect()
        {
            float sideW = Mathf.Clamp(Screen.width * 0.24f, 390f, 470f);
            return new Rect(12, 12, Screen.width - sideW - 36, Screen.height - 124);
        }

        private Rect BoardInnerRect(Rect outer, int w, int h)
        {
            Rect inner = new Rect(outer.x + 22, outer.y + 24, outer.width - 44, outer.height - 42);
            float aspect = (float)w / h;
            if (inner.width / inner.height > aspect)
            {
                float width = inner.height * aspect;
                inner.x += (inner.width - width) / 2f;
                inner.width = width;
            }
            else
            {
                float height = inner.width / aspect;
                inner.y += (inner.height - height) / 2f;
                inner.height = height;
            }
            return inner;
        }

        private Rect CombatBoardInnerRect(Rect outer)
        {
            Rect inner = new Rect(outer.x + 22, outer.y + 42, outer.width - 44, outer.height - 92);
            float aspect = (float)CombatW / CombatH;
            if (inner.width / inner.height > aspect)
            {
                float width = inner.height * aspect;
                inner.x += (inner.width - width) / 2f;
                inner.width = width;
            }
            else
            {
                float height = inner.width / aspect;
                inner.y += (inner.height - height) / 2f;
                inner.height = height;
            }
            return inner;
        }

        private void DrawPanel(Rect rect)
        {
            DrawRect(rect, panel);
            DrawRect(new Rect(rect.x, rect.y, rect.width, Mathf.Min(18f, rect.height * 0.18f)), Hex("2a3233", 0.34f));
            DrawRect(new Rect(rect.x + 1, rect.y + 1, rect.width - 2, 1), Hex("f3ead7", 0.10f));
            DrawBorder(rect, line, 1);
        }

        private void DrawRpgPanel(Rect rect, Color accent)
        {
            DrawPanel(rect);
            DrawRect(new Rect(rect.x + 2, rect.y + 2, rect.width - 4, 2), accent.WithAlpha(0.20f));
            DrawBorder(Pad(rect, 4), accent.WithAlpha(0.26f), 1);
            float c = Mathf.Min(20f, rect.width * 0.08f);
            DrawRect(new Rect(rect.x + 5, rect.y + 5, c, 2), accent.WithAlpha(0.62f));
            DrawRect(new Rect(rect.x + 5, rect.y + 5, 2, c), accent.WithAlpha(0.62f));
            DrawRect(new Rect(rect.xMax - 5 - c, rect.y + 5, c, 2), accent.WithAlpha(0.62f));
            DrawRect(new Rect(rect.xMax - 7, rect.y + 5, 2, c), accent.WithAlpha(0.62f));
            DrawRect(new Rect(rect.x + 5, rect.yMax - 7, c, 2), accent.WithAlpha(0.44f));
            DrawRect(new Rect(rect.x + 5, rect.yMax - 5 - c, 2, c), accent.WithAlpha(0.44f));
            DrawRect(new Rect(rect.xMax - 5 - c, rect.yMax - 7, c, 2), accent.WithAlpha(0.44f));
            DrawRect(new Rect(rect.xMax - 7, rect.yMax - 5 - c, 2, c), accent.WithAlpha(0.44f));
            DrawCombatUiCornerTrim(rect, accent);
        }

        private void DrawPanelHeader(Rect panelRect, string title, string icon, Color accent, string rightText)
        {
            Rect iconRect = new Rect(panelRect.x + 12, panelRect.y + 10, 22, 22);
            DrawTinyUiIcon(iconRect, icon, accent);
            float titleW = string.IsNullOrEmpty(rightText) ? panelRect.width - 58f : panelRect.width - 184f;
            GUI.Label(new Rect(panelRect.x + 42, panelRect.y + 9, titleW, 24), FitText(title, titleW, h2Style), h2Style);
            if (!string.IsNullOrEmpty(rightText))
            {
                GUI.Label(new Rect(panelRect.xMax - 126, panelRect.y + 13, 112, 18), rightText, CenterRightStyle(11, muted));
            }
            DrawRect(new Rect(panelRect.x + 12, panelRect.y + 38, panelRect.width - 24, 1), accent.WithAlpha(0.34f));
        }

        private void DrawTinyUiIcon(Rect rect, string icon, Color accent)
        {
            DrawRect(rect, Hex("050708", 0.74f));
            int hudIndex = CombatHudIconIndex(icon);
            if (hudIndex >= 0 && TryDrawCombatHudUiAtlasIcon(Pad(rect, 1f), hudIndex, Color.white.WithAlpha(0.94f)))
            {
                DrawBorder(rect, accent.WithAlpha(0.82f), 1);
                return;
            }
            int uiIndex = CombatUiIconIndex(icon);
            if (uiIndex >= 0 && TryDrawCombatUiAtlasIcon(Pad(rect, 1f), uiIndex, Color.white.WithAlpha(0.92f)))
            {
                DrawBorder(rect, accent.WithAlpha(0.82f), 1);
                return;
            }
            DrawBorder(rect, accent.WithAlpha(0.82f), 1);
            Rect inner = Pad(rect, rect.width * 0.22f);
            if (icon == "party")
            {
                DrawRect(new Rect(inner.x, inner.y + inner.height * 0.50f, inner.width, inner.height * 0.22f), accent);
                DrawRect(new Rect(inner.x + inner.width * 0.12f, inner.y, inner.width * 0.24f, inner.height * 0.44f), teal);
                DrawRect(new Rect(inner.x + inner.width * 0.60f, inner.y + inner.height * 0.08f, inner.width * 0.24f, inner.height * 0.40f), gold);
            }
            else if (icon == "enemy")
            {
                DrawRect(new Rect(inner.x + inner.width * 0.15f, inner.y + inner.height * 0.14f, inner.width * 0.70f, inner.height * 0.54f), blood);
                DrawRect(new Rect(inner.x + inner.width * 0.22f, inner.y, inner.width * 0.18f, inner.height * 0.28f), ink);
                DrawRect(new Rect(inner.x + inner.width * 0.60f, inner.y, inner.width * 0.18f, inner.height * 0.28f), ink);
                DrawRect(new Rect(inner.x + inner.width * 0.35f, inner.y + inner.height * 0.38f, inner.width * 0.10f, inner.height * 0.10f), retroBlack);
                DrawRect(new Rect(inner.x + inner.width * 0.56f, inner.y + inner.height * 0.38f, inner.width * 0.10f, inner.height * 0.10f), retroBlack);
            }
            else if (icon == "scroll")
            {
                DrawRect(new Rect(inner.x + inner.width * 0.10f, inner.y + inner.height * 0.10f, inner.width * 0.80f, inner.height * 0.72f), Hex("d9d3c4"));
                DrawRect(new Rect(inner.x + inner.width * 0.20f, inner.y + inner.height * 0.28f, inner.width * 0.58f, inner.height * 0.08f), accent);
                DrawRect(new Rect(inner.x + inner.width * 0.20f, inner.y + inner.height * 0.50f, inner.width * 0.42f, inner.height * 0.08f), accent.WithAlpha(0.75f));
            }
            else if (icon == "magic")
            {
                DrawPixelCross(inner, accent);
                DrawRect(new Rect(inner.center.x - inner.width * 0.10f, inner.y, inner.width * 0.20f, inner.height), accent.WithAlpha(0.70f));
            }
            else
            {
                DrawSigil(inner, icon, accent);
            }
        }

        private int CombatHudIconIndex(string icon)
        {
            switch ((icon ?? "").ToLowerInvariant())
            {
                case "queue": return 11;
                case "party":
                case "active": return 16;
                case "enemy": return 17;
                case "timeline":
                case "scroll": return 18;
                case "magic": return 2;
                default: return -1;
            }
        }

        private bool IsCombatUiAtlas()
        {
            return combatUiAtlas != null && Mathf.Abs(combatUiAtlas.width - combatUiAtlas.height) < 8 && combatUiAtlas.width >= 512;
        }

        private Rect CombatUiAtlasCell(int index)
        {
            return AtlasCell(combatUiAtlas, index, 4, 4);
        }

        private bool TryDrawCombatUiAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCombatUiAtlas()) return false;
            return DrawTextureRegionTint(combatUiAtlas, rect, CombatUiAtlasCell(index), tint);
        }

        private bool IsSpellbookUiAtlas()
        {
            return spellbookUiAtlas != null && Mathf.Abs(spellbookUiAtlas.width - spellbookUiAtlas.height) < 8 && spellbookUiAtlas.width >= 768;
        }

        private Rect SpellbookUiAtlasCell(int index)
        {
            return AtlasCell(spellbookUiAtlas, index, 5, 5);
        }

        private bool TryDrawSpellbookUiAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsSpellbookUiAtlas()) return false;
            return DrawTextureRegionTint(spellbookUiAtlas, rect, SpellbookUiAtlasCell(index), tint);
        }

        private bool IsCombatSpellbookUiAtlas()
        {
            return combatSpellbookUiAtlas != null && combatSpellbookUiAtlas.width >= 768 && combatSpellbookUiAtlas.height >= 600;
        }

        private Rect CombatSpellbookUiAtlasCell(int index)
        {
            return AtlasCell(combatSpellbookUiAtlas, index, 5, 4);
        }

        private bool TryDrawCombatSpellbookUiAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCombatSpellbookUiAtlas() || index < 0) return false;
            return DrawTextureRegionTint(combatSpellbookUiAtlas, rect, CombatSpellbookUiAtlasCell(index), tint);
        }

        private bool IsCombatHudUiAtlas()
        {
            return combatHudUiAtlas != null && combatHudUiAtlas.width >= 768 && combatHudUiAtlas.height >= 600;
        }

        private Rect CombatHudUiAtlasCell(int index)
        {
            return AtlasCell(combatHudUiAtlas, index, 5, 4);
        }

        private bool TryDrawCombatHudUiAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCombatHudUiAtlas() || index < 0) return false;
            return DrawTextureRegionTint(combatHudUiAtlas, rect, CombatHudUiAtlasCell(index), tint);
        }

        private bool IsCombatSpellFloatAtlas()
        {
            return combatSpellFloatAtlas != null && Mathf.Abs(combatSpellFloatAtlas.width - combatSpellFloatAtlas.height) < 8 && combatSpellFloatAtlas.width >= 768;
        }

        private Rect CombatSpellFloatAtlasCell(int index)
        {
            return AtlasCell(combatSpellFloatAtlas, index, 4, 4);
        }

        private bool TryDrawCombatSpellFloatAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCombatSpellFloatAtlas() || index < 0) return false;
            return DrawTextureRegionTint(combatSpellFloatAtlas, rect, CombatSpellFloatAtlasCell(index), tint);
        }

        private bool IsEmberSpellAtlas()
        {
            return emberSpellAtlas != null && Mathf.Abs(emberSpellAtlas.width - emberSpellAtlas.height) < 8 && emberSpellAtlas.width >= 768;
        }

        private Rect EmberSpellAtlasCell(int index)
        {
            return AtlasCell(emberSpellAtlas, index, 4, 4);
        }

        private bool TryDrawEmberSpellAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsEmberSpellAtlas()) return false;
            return DrawTextureRegionTint(emberSpellAtlas, rect, EmberSpellAtlasCell(index), tint);
        }

        private bool IsEpicSpellEffectsAtlas()
        {
            return epicSpellEffectsAtlas != null && epicSpellEffectsAtlas.width >= 768 && epicSpellEffectsAtlas.height >= 600;
        }

        private Rect EpicSpellEffectsAtlasCell(int index)
        {
            return AtlasCell(epicSpellEffectsAtlas, index, 5, 4);
        }

        private bool TryDrawEpicSpellEffectsAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsEpicSpellEffectsAtlas() || index < 0) return false;
            return DrawTextureRegionTint(epicSpellEffectsAtlas, rect, EpicSpellEffectsAtlasCell(index), tint);
        }

        private bool IsBossEnemyAtlas()
        {
            return bossEnemyAtlas != null && bossEnemyAtlas.width >= 768 && bossEnemyAtlas.height >= 600;
        }

        private Rect BossEnemyAtlasCell(int index)
        {
            return AtlasCell(bossEnemyAtlas, index, 5, 4);
        }

        private bool TryDrawBossEnemyAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsBossEnemyAtlas() || index < 0) return false;
            return DrawTextureRegionTint(bossEnemyAtlas, rect, BossEnemyAtlasCell(index), tint);
        }

        private bool IsQuestWorldAtlas()
        {
            return questWorldAtlas != null && questWorldAtlas.width >= 768 && questWorldAtlas.height >= 600;
        }

        private Rect QuestWorldAtlasCell(int index)
        {
            return AtlasCell(questWorldAtlas, index, 5, 4);
        }

        private bool TryDrawQuestWorldAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsQuestWorldAtlas() || index < 0) return false;
            return DrawTextureRegionTint(questWorldAtlas, rect, QuestWorldAtlasCell(index), tint);
        }

        private bool IsCharacterInventoryUiAtlas()
        {
            return characterInventoryUiAtlas != null && characterInventoryUiAtlas.width >= 768 && characterInventoryUiAtlas.height >= 600;
        }

        private Rect CharacterInventoryUiAtlasCell(int index)
        {
            return AtlasCell(characterInventoryUiAtlas, index, 5, 4);
        }

        private bool TryDrawCharacterInventoryUiAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCharacterInventoryUiAtlas() || index < 0) return false;
            return DrawTextureRegionTint(characterInventoryUiAtlas, rect, CharacterInventoryUiAtlasCell(index), tint);
        }

        private bool IsEnemyWorldObjectAtlas()
        {
            return enemyWorldObjectAtlas != null && enemyWorldObjectAtlas.width >= 768 && enemyWorldObjectAtlas.height >= 600;
        }

        private Rect EnemyWorldObjectAtlasCell(int index)
        {
            return AtlasCell(enemyWorldObjectAtlas, index, 5, 4);
        }

        private bool TryDrawEnemyWorldObjectAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsEnemyWorldObjectAtlas() || index < 0) return false;
            return DrawTextureRegionTint(enemyWorldObjectAtlas, rect, EnemyWorldObjectAtlasCell(index), tint);
        }

        private bool IsTavernUiAtlas()
        {
            return tavernUiAtlas != null && tavernUiAtlas.width >= 768 && tavernUiAtlas.height >= 600;
        }

        private Rect TavernUiAtlasCell(int index)
        {
            return AtlasCell(tavernUiAtlas, index, 5, 4);
        }

        private bool TryDrawTavernUiAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsTavernUiAtlas() || index < 0) return false;
            return DrawTextureRegionTint(tavernUiAtlas, rect, TavernUiAtlasCell(index), tint);
        }

        private bool IsInventoryConsumableAtlas()
        {
            return inventoryConsumableAtlas != null && inventoryConsumableAtlas.width >= 768 && inventoryConsumableAtlas.height >= 600;
        }

        private Rect InventoryConsumableAtlasCell(int index)
        {
            return AtlasCell(inventoryConsumableAtlas, index, 5, 4);
        }

        private bool TryDrawInventoryConsumableAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsInventoryConsumableAtlas() || index < 0) return false;
            return DrawTextureRegionTint(inventoryConsumableAtlas, rect, InventoryConsumableAtlasCell(index), tint);
        }

        private bool IsCombatCommandIconAtlas()
        {
            return combatCommandIconAtlas != null && combatCommandIconAtlas.width >= 768 && combatCommandIconAtlas.height >= 600;
        }

        private Rect CombatCommandIconAtlasCell(int index)
        {
            return AtlasCell(combatCommandIconAtlas, index, 5, 4);
        }

        private bool TryDrawCombatCommandIconAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCombatCommandIconAtlas() || index < 0) return false;
            return DrawTextureRegionTint(combatCommandIconAtlas, rect, CombatCommandIconAtlasCell(index), tint);
        }

        private bool IsCreatureSpriteAtlas()
        {
            return creatureSpriteAtlas != null && Mathf.Abs(creatureSpriteAtlas.width - creatureSpriteAtlas.height) < 8 && creatureSpriteAtlas.width >= 768;
        }

        private Rect CreatureSpriteAtlasCell(int index)
        {
            return AtlasCell(creatureSpriteAtlas, index, 4, 4);
        }

        private bool TryDrawCreatureSpriteAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCreatureSpriteAtlas() || index < 0) return false;
            return DrawTextureRegionTint(creatureSpriteAtlas, rect, CreatureSpriteAtlasCell(index), tint);
        }

        private bool IsCombatTerrainAtlas()
        {
            return combatTerrainAtlas != null && Mathf.Abs(combatTerrainAtlas.width - combatTerrainAtlas.height) < 8 && combatTerrainAtlas.width >= 768;
        }

        private Rect CombatTerrainAtlasCell(int index)
        {
            return AtlasCell(combatTerrainAtlas, index, 4, 4);
        }

        private bool TryDrawCombatTerrainAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCombatTerrainAtlas() || index < 0) return false;
            return DrawTextureRegionTint(combatTerrainAtlas, rect, CombatTerrainAtlasCell(index), tint);
        }

        private int CombatUiIconIndex(string icon)
        {
            switch ((icon ?? "").ToLowerInvariant())
            {
                case "timeline":
                case "scroll": return 0;
                case "queue": return 1;
                case "party":
                case "active": return 2;
                case "target": return 3;
                case "move": return 4;
                case "ready": return 5;
                case "spent": return 6;
                case "magic": return 7;
                case "range": return 8;
                case "blocked": return 9;
                case "guard": return 10;
                case "danger": return 11;
                case "hp": return 12;
                case "mp": return 13;
                case "status": return 14;
                case "trim": return 15;
                default: return -1;
            }
        }

        private void DrawCombatUiCornerTrim(Rect rect, Color accent)
        {
            if (!IsCombatUiAtlas()) return;
            float size = Mathf.Clamp(Mathf.Min(rect.width, rect.height) * 0.12f, 18f, 34f);
            Color tint = Color.Lerp(Color.white, accent, 0.28f).WithAlpha(0.36f);
            TryDrawCombatUiAtlasIcon(new Rect(rect.x + 5f, rect.y + 5f, size, size), 15, tint);
            TryDrawCombatUiAtlasIcon(new Rect(rect.xMax - size - 5f, rect.y + 5f, size, size), 15, tint);
        }

        private void DrawLabeledMeter(Rect rect, string label, int value, int max, Color color)
        {
            GUI.Label(new Rect(rect.x, rect.y - 2, 20, rect.height + 4), label, CenterLeftStyle(Mathf.RoundToInt(Mathf.Clamp(rect.height + 3, 8, 11)), muted));
            Rect bar = new Rect(rect.x + 20, rect.y, rect.width - 20, rect.height);
            DrawMeter(bar, value, max, color);
            DrawBorder(bar, Hex("030405", 0.70f), 1);
        }

        private string FitText(string text, float width, GUIStyle style)
        {
            if (string.IsNullOrEmpty(text) || width <= 12f || style == null) return "";
            if (style.CalcSize(new GUIContent(text)).x <= width) return text;
            const string ellipsis = "...";
            int lo = 0;
            int hi = text.Length;
            while (lo < hi)
            {
                int mid = (lo + hi + 1) / 2;
                string sample = text.Substring(0, mid) + ellipsis;
                if (style.CalcSize(new GUIContent(sample)).x <= width) lo = mid;
                else hi = mid - 1;
            }
            return text.Substring(0, Mathf.Clamp(lo, 0, text.Length)) + ellipsis;
        }

        private void DrawRect(Rect rect, Color color)
        {
            Color old = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, pixel);
            GUI.color = old;
        }

        private void DrawBorder(Rect rect, Color color, int thickness)
        {
            DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
            DrawRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
            DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
            DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color);
        }

        private void DrawMeter(Rect rect, int value, int max, Color color)
        {
            DrawRect(rect, Hex("111619"));
            DrawRect(new Rect(rect.x, rect.y, rect.width * Mathf.Clamp01(max <= 0 ? 0 : (float)value / max), rect.height), color);
        }

        private bool DrawAtlasRegion(Rect destination, Rect sourcePixels)
        {
            return DrawTextureRegion(betaCombatArt, destination, sourcePixels);
        }

        private bool DrawFormulaLabRegion(Rect destination, Rect sourcePixels)
        {
            return DrawTextureRegion(formulaLabArt, destination, sourcePixels);
        }

        private bool DrawClassIconRegion(Rect destination, Rect sourcePixels)
        {
            return DrawTextureRegion(classIconAtlas, destination, sourcePixels);
        }

        private bool DrawWorldObjectRegion(Rect destination, Rect sourcePixels)
        {
            return DrawTextureRegion(worldObjectAtlas, destination, sourcePixels);
        }

        private bool DrawCombatSpriteSheetRegion(Rect destination, Rect sourcePixels)
        {
            return DrawTextureRegion(combatSpriteSheet, destination, sourcePixels);
        }

        private bool DrawTextureRegion(Texture2D texture, Rect destination, Rect sourcePixels)
        {
            return DrawTextureRegionTint(texture, destination, sourcePixels, Color.white);
        }

        private bool DrawTextureRegionTint(Texture2D texture, Rect destination, Rect sourcePixels, Color tint)
        {
            if (texture == null || sourcePixels.width <= 0f || sourcePixels.height <= 0f) return false;
            Rect fit = AspectFit(destination, sourcePixels.width, sourcePixels.height);
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

        private bool TryDrawWorldObjectIcon(Rect rect, ObjectType type)
        {
            int enemyWorldIndex = EnemyWorldObjectIconIndex(type);
            if (enemyWorldIndex >= 0 && TryDrawEnemyWorldObjectAtlasIcon(rect, enemyWorldIndex, Color.white)) return true;
            int questIndex = QuestWorldObjectIconIndex(type);
            if (questIndex >= 0 && TryDrawQuestWorldAtlasIcon(rect, questIndex, Color.white)) return true;
            if (worldObjectAtlas == null) return false;
            Rect source = WorldObjectIconCell(type);
            if (source.width <= 0f || source.height <= 0f) return false;
            DrawWorldObjectRegion(rect, source);
            return true;
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

        private Rect WorldObjectIconCell(ObjectType type)
        {
            if (worldObjectAtlas == null) return Rect.zero;
            int index = Mathf.Clamp((int)type, 0, 9);
            return AtlasCell(worldObjectAtlas, index, 5, Mathf.Max(2, AtlasRows(worldObjectAtlas, 5)));
        }

        private bool TryDrawExploreEnvironmentTile(Rect rect, int x, int y, int tile, string kind)
        {
            if (worldObjectAtlas == null || AtlasRows(worldObjectAtlas, 5) < 4) return false;
            int index = EnvironmentTileAtlasIndex(tile, kind);
            if (index < 0) return false;
            float n = (ExploreNoise(x, y, 43) % 9) / 8f;
            float alpha = tile == 0 ? Mathf.Lerp(0.54f, 0.78f, n) : Mathf.Lerp(0.48f, 0.72f, n);
            DrawTextureRegionTint(worldObjectAtlas, rect, AtlasCell(worldObjectAtlas, index, 5, 4), Color.white.WithAlpha(alpha));
            return true;
        }

        private int EnvironmentTileAtlasIndex(int tile, string kind)
        {
            kind = kind ?? "";
            if (tile == 0)
            {
                if (kind == "mirewall") return 16;
                if (kind == "forestwall") return 15;
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

        private int ItemIconIndex(string name, string slot)
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
            if (text.Contains("leather") || text.Contains("hide")) return 9;
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

        private bool TryDrawAtlasCombatSprite(Rect rect, CombatUnit unit)
        {
            int creatureIndex = CreatureSpriteIndex(unit);
            if (creatureIndex >= 0 && TryDrawCreatureSpriteAtlasIcon(Pad(rect, -rect.width * 0.015f), creatureIndex, Color.white)) return true;
            if (unit != null && unit.Side == UnitSide.Enemy)
            {
                int enemyWorldIndex = EnemyWorldEnemyIndex(unit.Role);
                if (enemyWorldIndex >= 0 && TryDrawEnemyWorldObjectAtlasIcon(Pad(rect, rect.width * 0.04f), enemyWorldIndex, Color.white)) return true;
                int bossIndex = BossEnemyIndex(unit.Role);
                if (bossIndex >= 0 && TryDrawBossEnemyAtlasIcon(Pad(rect, rect.width * 0.04f), bossIndex, Color.white)) return true;
            }
            return TryDrawSpriteSheetCombatSprite(rect, unit);
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

        private bool TryDrawEnemyRosterPortrait(Rect rect, CombatUnit unit)
        {
            if (unit == null || unit.Side != UnitSide.Enemy) return false;
            int creatureIndex = CreatureSpriteIndex(unit);
            if (creatureIndex >= 0 && TryDrawCreatureSpriteAtlasIcon(Pad(rect, -rect.width * 0.01f), creatureIndex, Color.white)) return true;
            int enemyWorldIndex = EnemyWorldEnemyIndex(unit.Role);
            if (enemyWorldIndex >= 0 && TryDrawEnemyWorldObjectAtlasIcon(Pad(rect, rect.width * 0.03f), enemyWorldIndex, Color.white)) return true;
            int bossIndex = BossEnemyIndex(unit.Role);
            if (bossIndex >= 0 && TryDrawBossEnemyAtlasIcon(Pad(rect, rect.width * 0.03f), bossIndex, Color.white)) return true;
            if (enemyRosterAtlas == null) return false;
            int index = EnemyRosterIndex(unit.Role);
            if (index < 0) return false;
            DrawTextureRegionTint(enemyRosterAtlas, Pad(rect, rect.width * 0.03f), AtlasCell(enemyRosterAtlas, index, 5, 4), Color.white);
            return true;
        }

        private int EnemyWorldEnemyIndex(string role)
        {
            switch ((role ?? "").ToLowerInvariant())
            {
                case "koboldraider":
                case "koboldslinger":
                case "koboldshield": return 0;
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
                case "drowblade": return 4;
                case "drowmage":
                case "drowpriest": return 5;
                case "lesserdemon": return 6;
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

        private int EnemyRosterIndex(string role)
        {
            switch (role ?? "")
            {
                case "koboldraider": return 0;
                case "koboldslinger": return 1;
                case "koboldshaman": return 2;
                case "koboldwizard": return 3;
                case "koboldshield": return 4;
                case "sewerrat":
                case "giantrat": return 5;
                case "ratfolk":
                case "ratcutthroat": return 6;
                case "ratmage": return 7;
                case "ratcleric": return 8;
                case "ratbrute": return 9;
                case "drowscout": return 10;
                case "drowblade": return 11;
                case "drowpriest": return 12;
                case "drowmage":
                case "glassmage":
                case "adept": return 13;
                case "drowcrossbow":
                case "mirearcher": return 14;
                case "boundimp": return 15;
                case "cinderling": return 16;
                case "lesserdemon": return 17;
                case "shade": return 18;
                case "bonepriest": return 19;
                default: return -1;
            }
        }

        private int CreatureSpriteIndex(CombatUnit unit)
        {
            if (unit == null) return -1;
            if (unit.Summoned && (unit.Role ?? "").ToLowerInvariant().Contains("imp")) return 10;
            return CreatureSpriteIndexForRole(unit.Role, unit.Side);
        }

        private int CreatureSpriteIndexForRole(string role, UnitSide side)
        {
            string key = (role ?? "").ToLowerInvariant();
            if (side == UnitSide.Party)
            {
                switch (key)
                {
                    case "boundimp": return 10;
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
            if (role == "boundimp" || role == "lesserdemon") return 14;
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
                if (unit.Role != null && unit.Role.StartsWith("drow")) return 0.97f;
                if (unit.Role == "cinderling") return 1.00f;
                return 0.96f;
            }
            if (unit.Role == "mender" || unit.Role == "ember" || unit.Role == "hex") return 0.94f;
            return 0.96f;
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

        private void DrawCombatUnitSprite(Rect rect, CombatUnit unit, bool active)
        {
            Color color = VividColor(unit.Color.ToColor());
            Color frame = CombatFrameColor(unit, active);
            DrawSpriteFrameBackground(rect, frame, color, active);
            Rect spriteRect = rect;
            if (active)
            {
                float bob = state.ReducedMotion ? 0f : Mathf.Sin(Time.time * 7f) * rect.height * 0.018f;
                spriteRect.y += bob;
            }

            DrawSpriteGroundShadow(spriteRect, color, active);
            if (active) DrawActiveSpriteAccent(spriteRect, unit);
            bool atlasDrawn = TryDrawAtlasCombatSprite(spriteRect, unit);

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

            DrawCombatSpriteBadges(rect, unit, color);
            DrawCombatSpriteCasterMarks(spriteRect, unit);
            DrawWoundedSpriteMarks(spriteRect, unit);
            if (atlasDrawn && unit.Side == UnitSide.Party)
            {
                DrawEquipmentOverlay(spriteRect, unit.Role, unit.WeaponName, unit.ArmorName, unit.DamageType);
            }
            DrawStatusOverlay(rect, unit);
            DrawStatusDurationBadges(rect, unit);
            DrawSigil(new Rect(rect.x + rect.width * 0.37f, rect.y + rect.height * 0.80f, rect.width * 0.26f, rect.height * 0.14f), unit.Sigil, unit.Side == UnitSide.Party ? ink : Color.Lerp(color, ink, 0.35f));
        }

        private void DrawPartyPortraitSprite(Rect rect, PartyMember member, Color color)
        {
            color = VividColor(color);
            DrawSpriteFrameBackground(rect, color, color, false);
            DrawSpriteGroundShadow(rect, color, false);
            bool atlasDrawn = TryDrawAtlasPartyPortrait(rect, member.Role);
            if (!atlasDrawn)
            {
                DrawPartyFigure(rect, member.Role, color);
                DrawPartyVariantOverlay(rect, member.Role, member.Id + member.Name + member.Sigil, member.ArmorName, member.WeaponName, color);
                DrawEquipmentOverlay(rect, member.Role, member.WeaponName, member.ArmorName, member.WeaponDamageType);
            }
            else
            {
                DrawEquipmentOverlay(rect, member.Role, member.WeaponName, member.ArmorName, member.WeaponDamageType);
            }
            DrawSigil(new Rect(rect.x + rect.width * 0.37f, rect.y + rect.height * 0.80f, rect.width * 0.26f, rect.height * 0.14f), member.Sigil, ink);
        }

        private void DrawSpriteFrameBackground(Rect rect, Color frame, Color accent, bool active)
        {
            DrawRect(rect, Hex("101619"));
            DrawRect(Pad(rect, rect.width * 0.06f), Hex("050708", 0.40f));
            int heavy = Mathf.Max(2, Mathf.RoundToInt(rect.width * 0.045f));
            DrawBorder(rect, Hex("030405", 0.92f), heavy + 1);
            DrawBorder(Pad(rect, heavy), frame, Mathf.Max(2, heavy));
            DrawRect(new Rect(rect.x + rect.width * 0.08f, rect.y + rect.height * 0.08f, rect.width * 0.84f, rect.height * 0.10f), Color.Lerp(accent, ink, active ? 0.44f : 0.30f).WithAlpha(0.72f));
            DrawRect(new Rect(rect.x + rect.width * 0.16f, rect.y + rect.height * 0.73f, rect.width * 0.68f, Mathf.Max(2f, rect.height * 0.025f)), frame.WithAlpha(active ? 0.50f : 0.30f));
            DrawRect(new Rect(rect.center.x - 1f, rect.y + rect.height * 0.16f, 2f, rect.height * 0.58f), Hex("f3ead7", active ? 0.08f : 0.045f));
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
            float pulse = state != null && state.ReducedMotion ? 0.65f : 0.45f + Mathf.Sin(Time.time * 6f) * 0.20f;
            Color baseGlow = unit.Side == UnitSide.Party ? teal : blood;
            Color glow = Color.Lerp(baseGlow, cursorWhite, Mathf.Clamp01(pulse));
            DrawBorder(Pad(rect, -rect.width * 0.035f), glow, 2);
            DrawRect(new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * 0.91f, rect.width * 0.52f, rect.height * 0.035f), glow);
            if (unit.Side == UnitSide.Party)
            {
                string weapon = GearKind(unit.WeaponName, unit.Role, true);
                if (weapon.Contains("bow") || weapon.Contains("crossbow"))
                {
                    DrawRect(new Rect(rect.x + rect.width * 0.61f, rect.y + rect.height * 0.39f, rect.width * 0.26f, rect.height * 0.035f), glow);
                }
                else if (weapon.Contains("staff") || weapon.Contains("focus") || weapon.Contains("orb") || unit.Role == "mender" || unit.Role == "ember" || unit.Role == "hex")
                {
                    DrawPixelCross(new Rect(rect.x + rect.width * 0.65f, rect.y + rect.height * 0.24f, rect.width * 0.16f, rect.height * 0.16f), glow);
                }
                else
                {
                    DrawRect(new Rect(rect.x + rect.width * 0.68f, rect.y + rect.height * 0.27f, rect.width * 0.17f, rect.height * 0.035f), glow);
                }
            }
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
            if (unit.Rank == "veteran")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.16f, rect.width * 0.28f, rect.height * 0.035f), Color.Lerp(gold, color, 0.35f));
            }
            else if (unit.Rank == "elite")
            {
                DrawBorder(new Rect(rect.x + rect.width * 0.29f, rect.y + rect.height * 0.18f, rect.width * 0.42f, rect.height * 0.20f), Color.Lerp(gold, mark, 0.25f), 1);
                DrawPixelCross(new Rect(rect.x + rect.width * 0.43f, rect.y + rect.height * 0.12f, rect.width * 0.14f, rect.height * 0.10f), Color.Lerp(gold, cursorWhite, 0.18f));
            }

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
            return role == "sentry" || role == "adept" || role == "husk" || role == "reaver" || role == "spore" || role == "shade" || role == "glassmage" || role == "thornbeast" || role == "mirearcher" || role == "bonepriest" || role == "cinderling" || role == "gloamknight" || role == "koboldraider" || role == "koboldslinger" || role == "koboldshaman" || role == "koboldwizard" || role == "koboldshield" || role == "sewerrat" || role == "giantrat" || role == "ratfolk" || role == "ratcutthroat" || role == "ratmage" || role == "ratcleric" || role == "ratbrute" || role == "drowscout" || role == "drowblade" || role == "drowcrossbow" || role == "drowmage" || role == "drowpriest" || role == "lesserdemon";
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
            Color cloth = role == "koboldwizard" ? Hex("3a202b") : role == "koboldshaman" ? Hex("5f425e") : role == "koboldshield" ? Hex("7a4c35") : Hex("8a3d32");
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

            if (role == "koboldslinger")
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
            DrawRect(new Rect(rect.x + rect.width * 0.08f, rect.y + rect.height * 0.68f, rect.width * 0.84f, rect.height * 0.12f), Hex("080b0d", 0.7f));
            DrawPartyPortraitSprite(new Rect(rect.x + rect.width * 0.31f, rect.y + rect.height * 0.12f, rect.width * 0.38f, rect.height * 0.64f), member, color);
            DrawSigil(new Rect(rect.x + rect.width * 0.10f, rect.y + rect.height * 0.14f, rect.width * 0.18f, rect.height * 0.16f), member.Sigil, color);
            DrawClassIcon(new Rect(rect.xMax - rect.width * 0.28f, rect.y + rect.height * 0.10f, rect.width * 0.20f, rect.width * 0.20f), member.ClassKey, member.Role, color);
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

        private void DrawBanner()
        {
            if (Time.time > bannerUntil || string.IsNullOrEmpty(bannerText)) return;
            Rect rect = new Rect(Screen.width / 2f - 210, 114, 420, 48);
            DrawRect(rect, Hex("171c20", 0.96f));
            DrawBorder(rect, gold, 1);
            GUI.Label(rect, bannerText, CenterStyle(18, ink));
        }

        private void ShowLootPanel(InventoryItem item, int goldFound, int suppliesFound, int elixirsFound, string equipNote)
        {
            if (item == null) return;
            lootPanelTitle = "Cache opened";
            lootPanelBody = $"{item.DisplayName}\n{ItemTraitLine(item)}\n{equipNote}";
            lootPanelItem = item;
            lootPanelGold = goldFound;
            lootPanelSupplies = suppliesFound;
            lootPanelElixirs = elixirsFound;
            lootPanelUntil = Time.time + 6.4f;
        }

        private void DrawLootPanel()
        {
            if (Time.time > lootPanelUntil || string.IsNullOrEmpty(lootPanelBody)) return;
            Rect rect = new Rect(Screen.width / 2f - 286, Screen.height - 204, 572, 146);
            DrawRect(rect, Hex("11171b", 0.97f));
            DrawBorder(rect, gold, 1);
            DrawCombatUiCornerTrim(rect, gold);
            DrawItemIcon(new Rect(rect.x + 16, rect.y + 48, 66, 66), lootPanelItem);
            GUI.Label(new Rect(rect.x + 96, rect.y + 8, rect.width - 112, 22), lootPanelTitle, CenterLeftStyle(15, gold));
            float chipX = rect.x + 96;
            DrawLootResourceChip(new Rect(chipX, rect.y + 34, 104, 28), 10, $"+{lootPanelGold}", gold, "gold");
            if (lootPanelSupplies > 0) DrawLootResourceChip(new Rect(chipX + 112, rect.y + 34, 118, 28), 0, $"+{lootPanelSupplies}", moss, "supplies");
            if (lootPanelElixirs > 0) DrawLootResourceChip(new Rect(chipX + 238, rect.y + 34, 112, 28), 8, $"+{lootPanelElixirs}", teal, "elixir");
            GUI.Label(new Rect(rect.x + 96, rect.y + 68, rect.width - 112, rect.height - 78), lootPanelBody, CenterLeftStyle(12, ink));
        }

        private void DrawLootResourceChip(Rect rect, int iconIndex, string amount, Color accent, string label)
        {
            DrawRect(rect, Hex("080b0d", 0.82f));
            DrawBorder(rect, accent.WithAlpha(0.78f), 1);
            TryDrawInventoryConsumableAtlasIcon(new Rect(rect.x + 5, rect.y + 4, 20, 20), iconIndex, Color.white);
            GUI.Label(new Rect(rect.x + 30, rect.y + 3, rect.width - 34, 11), amount, CenterLeftStyle(10, cursorWhite));
            GUI.Label(new Rect(rect.x + 30, rect.y + 14, rect.width - 34, 11), label, CenterLeftStyle(9, muted));
        }

        private string CacheSupplyLine(int suppliesFound, int elixirsFound)
        {
            List<string> parts = new List<string>();
            if (suppliesFound > 0) parts.Add(suppliesFound == 1 ? "1 supply" : $"{suppliesFound} supplies");
            if (elixirsFound > 0) parts.Add(elixirsFound == 1 ? "1 elixir" : $"{elixirsFound} elixirs");
            return parts.Count == 0 ? "" : ", " + string.Join(", ", parts.ToArray());
        }

        private void DrawArmoryOverlay()
        {
            if (!showArmory || state == null) return;
            DrawRect(new Rect(0, 0, Screen.width, Screen.height), Hex("020303", 0.58f));
            Rect rect = ArmoryRect();
            GUI.ModalWindow(20020, rect, DrawArmoryWindow, "");
        }

        private Rect ArmoryRect()
        {
            float width = Mathf.Min(980, Screen.width - 64);
            float height = Mathf.Min(640, Screen.height - 64);
            return new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
        }

        private void DrawArmoryWindow(int id)
        {
            Rect outer = new Rect(0, 0, ArmoryRect().width, ArmoryRect().height);
            DrawRect(outer, Hex("11171b", 0.99f));
            DrawBorder(outer, gold, 2);
            GUI.Label(new Rect(18, 12, 360, 32), "Armory and Spell Reference", h2Style);
            GUI.Label(new Rect(20, 42, outer.width - 40, 22), "Inspect company gear, pack loot, and battle spells. Press I or C to toggle; Esc closes.", mutedStyle);
            if (GUI.Button(new Rect(outer.width - 82, 14, 58, 30), "Close", smallButtonStyle))
            {
                showArmory = false;
                PlaySfx("ui", 0.45f);
            }

            string[] tabs = { "Company", "Pack", "Spells" };
            for (int i = 0; i < tabs.Length; i++)
            {
                Rect tab = new Rect(20 + i * 116, 74, 106, 30);
                DrawRect(tab, armoryTab == i ? Hex("2d3438") : Hex("151b20"));
                DrawBorder(tab, armoryTab == i ? gold : line, 1);
                int tabIcon = i == 0 ? 0 : i == 1 ? 4 : 15;
                if (GUI.Button(tab, "", smallButtonStyle))
                {
                    armoryTab = i;
                    armoryScroll = Vector2.zero;
                    PlaySfx("ui", 0.45f);
                }
                if (!TryDrawCharacterInventoryUiAtlasIcon(new Rect(tab.x + 5, tab.y + 5, 20, 20), tabIcon, Color.white.WithAlpha(armoryTab == i ? 0.82f : 0.46f)))
                {
                    TryDrawSpellbookUiAtlasIcon(new Rect(tab.x + 5, tab.y + 5, 20, 20), i == 2 ? 0 : 12 + i, Color.white.WithAlpha(armoryTab == i ? 0.82f : 0.46f));
                }
                GUI.Label(new Rect(tab.x + 28, tab.y + 6, tab.width - 34, 18), tabs[i], CenterLeftStyle(11, armoryTab == i ? cursorWhite : muted));
            }

            Rect view = new Rect(20, 114, outer.width - 40, outer.height - 158);
            float contentHeight = armoryTab == 0 ? Mathf.Max(view.height, 76 + state.Party.Count * 82) :
                armoryTab == 1 ? Mathf.Max(view.height, 86 + Mathf.Max(1, state.Inventory.Count) * 76) :
                Mathf.Max(view.height, 92 + formulaBook.Length * 48);
            Rect content = new Rect(0, 0, view.width - 18, contentHeight);
            armoryScroll = GUI.BeginScrollView(view, armoryScroll, content);
            if (armoryTab == 0) DrawArmoryCompany(content.width);
            else if (armoryTab == 1) DrawArmoryPack(content.width);
            else DrawArmoryFormulas(content.width);
            GUI.EndScrollView();

            GUI.Label(new Rect(20, outer.height - 34, outer.width - 40, 18), ArmoryFooterLine(), CenterLeftStyle(11, muted));
        }

        private void DrawArmoryCompany(float width)
        {
            GUI.Label(new Rect(0, 0, width, 24), "Company Gear", CenterLeftStyle(16, gold));
            GUI.Label(new Rect(0, 26, width, 20), "Visible equipment affects range, damage type, guard, armor, and sprite overlays.", CenterLeftStyle(12, muted));
            float y = 56;
            for (int i = 0; i < state.Party.Count; i++)
            {
                PartyMember member = state.Party[i];
                Rect row = new Rect(0, y, width, 72);
                DrawRect(row, i % 2 == 0 ? Hex("151b20") : Hex("101619"));
                DrawBorder(row, line, 1);
                DrawClassIcon(new Rect(row.x + 10, row.y + 13, 44, 44), member.ClassKey, member.Role, MemberColor(member));
                GUI.Label(new Rect(row.x + 64, row.y + 7, 176, 20), $"{member.Name} / {DisplayClass(member.ClassKey)}", CenterLeftStyle(13, ink));
                GUI.Label(new Rect(row.x + 64, row.y + 28, 188, 16), $"HP {member.Hp}/{member.MaxHp}  MP {member.Mana}/{member.MaxMana}  Agi {member.Agility}", CenterLeftStyle(11, muted));
                GUI.Label(new Rect(row.x + 64, row.y + 46, 188, 16), $"{BestSkillLabel(member)} {BestSkillValue(member)} / {SkillAdjective(BestSkillValue(member))}", CenterLeftStyle(11, muted));
                float weaponX = row.x + 254;
                DrawGearIcon(new Rect(weaponX, row.y + 15, 36, 36), member.WeaponName, "weapon", member.WeaponDamageType);
                GUI.Label(new Rect(weaponX + 44, row.y + 7, width * 0.32f, 20), TrimGearName(member.WeaponName), CenterLeftStyle(12, gold));
                GUI.Label(new Rect(weaponX + 44, row.y + 29, width * 0.32f, 16), WeaponSummaryLine(member), CenterLeftStyle(11, muted));
                float armorX = row.x + width * 0.66f;
                DrawGearIcon(new Rect(armorX, row.y + 15, 36, 36), member.ArmorName, "armor", "");
                GUI.Label(new Rect(armorX + 44, row.y + 7, width * 0.24f, 20), TrimGearName(member.ArmorName), CenterLeftStyle(12, teal));
                GUI.Label(new Rect(armorX + 44, row.y + 29, width * 0.24f, 32), ArmorSummaryLine(member), CenterLeftStyle(11, muted));
                y += 82;
            }
        }

        private void DrawArmoryPack(float width)
        {
            GUI.Label(new Rect(0, 0, width, 24), $"Pack Loot ({state.Inventory.Count})", CenterLeftStyle(16, gold));
            GUI.Label(new Rect(0, 26, width, 20), "Caches auto-equip useful finds, but the pack keeps a history. Try Equip re-runs the fit check.", CenterLeftStyle(12, muted));
            if (state.Inventory.Count == 0)
            {
                GUI.Label(new Rect(0, 64, width, 24), "No cache loot yet.", CenterLeftStyle(13, muted));
                return;
            }

            float y = 56;
            for (int i = state.Inventory.Count - 1; i >= 0; i--)
            {
                InventoryItem item = state.Inventory[i];
                Rect row = new Rect(0, y, width, 66);
                DrawRect(row, i % 2 == 0 ? Hex("151b20") : Hex("101619"));
                DrawBorder(row, line, 1);
                Color mark = item.Slot == "weapon" ? DamageColor(string.IsNullOrEmpty(item.DamageType) ? "physical" : item.DamageType) : teal;
                DrawRect(new Rect(row.x + 9, row.y + 11, 4, row.height - 22), mark);
                DrawItemIcon(new Rect(row.x + 18, row.y + 10, 44, 44), item);
                GUI.Label(new Rect(row.x + 70, row.y + 7, width * 0.40f, 20), item.DisplayName, CenterLeftStyle(12, ink));
                GUI.Label(new Rect(row.x + 70, row.y + 29, width * 0.40f, 16), ItemTraitLine(item), CenterLeftStyle(10, muted));
                GUI.Label(new Rect(row.x + width * 0.48f, row.y + 11, width * 0.34f, 36), BestFitLine(item), CenterLeftStyle(11, muted));
                if (GUI.Button(new Rect(row.xMax - 86, row.y + 17, 72, 32), "Try Equip", smallButtonStyle))
                {
                    string result = AutoEquipItem(item);
                    PushLog(string.IsNullOrEmpty(result) ? "No one can use that item yet." : result, string.IsNullOrEmpty(result) ? Tone.Warn : Tone.Good);
                    PlaySfx(string.IsNullOrEmpty(result) ? "blocked" : "cache", 0.55f);
                }
                y += 76;
            }
        }

        private void DrawArmoryFormulas(float width)
        {
            GUI.Label(new Rect(0, 0, width, 24), "Spell Reference", CenterLeftStyle(16, gold));
            GUI.Label(new Rect(0, 26, width, 20), FormulaCasterSummary(), CenterLeftStyle(12, muted));
            float y = 56;
            string lastSchool = "";
            foreach (FormulaDef formula in formulaBook)
            {
                string school = SpellCraftLabel(formula.School);
                if (school != lastSchool)
                {
                    GUI.Label(new Rect(0, y, width, 18), school.ToUpperInvariant(), CenterLeftStyle(11, teal));
                    y += 22;
                    lastSchool = school;
                }

                Rect row = new Rect(0, y, width, 40);
                DrawRect(row, Hex("151b20"));
                DrawBorder(row, FormulaColor(formula, 0.72f), 1);
                DrawFormulaLabIcon(new Rect(row.x + 8, row.y + 6, 28, 28), formula, "");
                GUI.Label(new Rect(row.x + 46, row.y + 5, width * 0.32f, 18), formula.Name, CenterLeftStyle(12, ink));
                GUI.Label(new Rect(row.x + 46, row.y + 22, width * 0.32f, 14), formula.Hint, CenterLeftStyle(10, muted));
                GUI.Label(new Rect(row.x + width * 0.42f, row.y + 6, width * 0.24f, 18), FormulaRuleLine(formula), CenterLeftStyle(11, muted));
                GUI.Label(new Rect(row.x + width * 0.68f, row.y + 6, width * 0.30f, 18), FormulaEffectLine(formula), CenterLeftStyle(11, FormulaColor(formula)));
                y += 48;
            }
        }

        private string ArmoryFooterLine()
        {
            if (armoryTab == 0) return "Company tab: gear shown here is already reflected in combat sprites and stats.";
            if (armoryTab == 1) return "Pack tab: auto-equip is conservative; Try Equip only changes gear when the fit check accepts it.";
            return "Spells tab: choose Cast in combat, click a spell card, then click a highlighted target.";
        }

        private string ItemTraitLine(InventoryItem item)
        {
            if (item == null) return "";
            List<string> parts = new List<string>();
            if (item.Slot == "weapon")
            {
                int range = WeaponRange(item, state?.Party?.FirstOrDefault() ?? new PartyMember { Role = "" });
                parts.Add(range > 1 ? $"range {range}" : "melee");
                if (item.DamageMin > 0 && item.DamageMax > 0) parts.Add($"{item.DamageMin}-{item.DamageMax} dmg");
                if (item.AttackSpeed > 0) parts.Add($"spd {item.AttackSpeed}");
                if (!string.IsNullOrEmpty(item.DamageType) && item.DamageType != "physical") parts.Add(item.DamageType);
                string status = GearOnHitStatus(item.DisplayName);
                if (!string.IsNullOrEmpty(status)) parts.Add(status + " chance");
            }
            else
            {
                parts.Add($"armor {ArmorDefenseBonus(item)}");
                if (ArmorAgilityModifier(item.DisplayName) > 0) parts.Add("light");
                if (ArmorAgilityModifier(item.DisplayName) < 0) parts.Add("heavy");
                if ((item.DisplayName ?? "").ToLowerInvariant().Contains("ward")) parts.Add("warding");
            }
            string stats = ItemStatBonusLine(item);
            if (!string.IsNullOrEmpty(stats)) parts.Add(stats);
            if (!string.IsNullOrEmpty(item.Rarity) && item.Rarity != "starter") parts.Add(item.Rarity);
            return parts.Count == 0 ? "Plain but serviceable." : "Traits: " + string.Join(" / ", parts);
        }

        private string WeaponSummaryLine(PartyMember member)
        {
            if (member == null) return "";
            List<string> parts = new List<string>
            {
                $"range {member.Range}",
                $"{member.DamageMin}-{member.DamageMax} dmg",
                $"spd {member.AttackSpeed}",
                string.IsNullOrEmpty(member.WeaponDamageType) ? "physical" : member.WeaponDamageType
            };
            int hit = WeaponHitBonus(member.WeaponName);
            int power = WeaponPowerBonus(member.WeaponName);
            if (hit != 0) parts.Add("hit " + Signed(hit));
            if (power != 0) parts.Add("power " + Signed(power));
            string status = GearOnHitStatus(member.WeaponName);
            if (!string.IsNullOrEmpty(status)) parts.Add(status + " chance");
            return string.Join(" / ", parts);
        }

        private string ItemStatBonusLine(InventoryItem item)
        {
            if (item == null) return "";
            List<string> stats = new List<string>();
            if (item.StrengthBonus != 0) stats.Add("STR " + Signed(item.StrengthBonus));
            if (item.IntelligenceBonus != 0) stats.Add("INT " + Signed(item.IntelligenceBonus));
            if (item.AgilityBonus != 0) stats.Add("AGI " + Signed(item.AgilityBonus));
            if (item.HealthBonus != 0) stats.Add("HP " + Signed(item.HealthBonus));
            return string.Join(" ", stats);
        }

        private string ArmorSummaryLine(PartyMember member)
        {
            if (member == null) return "";
            int agility = ArmorAgilityModifier(member.ArmorName);
            int guard = GearGuardBonus(new CombatUnit { ArmorName = member.ArmorName, WeaponName = member.WeaponName, Spell = member.Spell });
            List<string> parts = new List<string> { $"armor {member.ArmorBonus}" };
            if (guard > 0) parts.Add("guard +" + guard);
            if (agility != 0) parts.Add("agi " + Signed(agility));
            string text = (member.ArmorName ?? "").ToLowerInvariant();
            if (text.Contains("ward") || text.Contains("anti-magic") || text.Contains("moonstone")) parts.Add("wards magic");
            return string.Join(" / ", parts);
        }

        private string BestFitLine(InventoryItem item)
        {
            if (item == null || state?.Party == null || state.Party.Count == 0) return "";
            if (item.Slot == "weapon")
            {
                PartyMember target = state.Party.OrderByDescending(p => WeaponRoleFit(item, p)).ThenBy(p => p.WeaponBonus).FirstOrDefault();
                if (target == null) return "";
                string type = string.IsNullOrEmpty(item.DamageType) ? "physical" : item.DamageType;
                return $"Best fit: {target.Name}\nrange {WeaponRange(item, target)} / {type} / bonus {Signed(item.Bonus)}";
            }
            else
            {
                PartyMember target = state.Party.OrderBy(p => ArmorRolePenalty(item, p)).ThenBy(p => p.ArmorBonus).FirstOrDefault();
                if (target == null) return "";
                int agility = ArmorAgilityModifier(item.DisplayName);
                string weight = agility > 0 ? "light" : agility < 0 ? "heavy" : "steady";
                return $"Best fit: {target.Name}\narmor {ArmorDefenseBonus(item)} / {weight} / bonus {Signed(item.Bonus)}";
            }
        }

        private string FormulaCasterSummary()
        {
            int mend = state?.Party?.Count(p => CasterKnowsSchool(p.Spell, "mend")) ?? 0;
            int emberCount = state?.Party?.Count(p => CasterKnowsSchool(p.Spell, "ember")) ?? 0;
            int hexCount = state?.Party?.Count(p => CasterKnowsSchool(p.Spell, "hex")) ?? 0;
            int pactCount = state?.Party?.Count(p => CasterKnowsSchool(p.Spell, "pact")) ?? 0;
            return $"Known by craft: priest {mend}, ember {emberCount}, hex {hexCount}, pact {pactCount}. Spells are selected from the combat Cast menu.";
        }

        private string FormulaRuleLine(FormulaDef formula)
        {
            if (formula == null) return "";
            string splash = formula.Splash ? " / splash" : "";
            string sight = FormulaRequiresLineOfSight(formula) ? " / sight" : FormulaArcsOverCover(formula) ? " / arc" : "";
            return $"{FormulaTierLabel(formula)} / {formula.Mana} MP / r{formula.Range} / {formula.Target}{splash}{sight}";
        }

        private string FormulaEffectLine(FormulaDef formula)
        {
            if (formula == null) return "";
            if (formula.Effect == "terrain") return formula.Terrain + (formula.Duration > 0 ? $" {formula.Duration}t" : " block");
            if (formula.Effect == "summon") return $"{SummonDisplayName(formula.SummonRole)} {Mathf.Max(1, formula.Duration)}t";
            if (formula.Effect == "damage" || formula.Effect == "drain")
            {
                string type = string.IsNullOrEmpty(formula.DamageType) ? "magic" : formula.DamageType;
                string extra = formula.Splash ? " splash" : "";
                if (!string.IsNullOrEmpty(formula.Status)) extra += " " + StatusLabel(formula.Status);
                return $"{type}{extra}";
            }
            if (formula.Effect == "status") return StatusLabel(formula.Status);
            return formula.Effect;
        }

        private string SummonDisplayName(string role)
        {
            if (string.Equals(role, "boundimp", StringComparison.OrdinalIgnoreCase)) return "Bound Imp";
            if (string.IsNullOrWhiteSpace(role)) return "Summon";
            return DisplayRace(role);
        }

        private string Signed(int value)
        {
            return value > 0 ? "+" + value : value.ToString();
        }

        private void ShowBanner(string text)
        {
            bannerText = text;
            bannerUntil = Time.time + (state != null && state.ReducedMotion ? 0.9f : 1.5f);
        }

        private void AddFloat(int x, int y, string text, Color color)
        {
            if (state != null && state.ReducedMotion) return;
            float now = Time.time;
            int lane = floatTexts.Count(t => t.X == x && t.Y == y && now <= t.Start + t.Duration + 0.18f) % 5;
            floatTexts.Add(new FloatText { X = x, Y = y, Text = text, Color = color.ToHex(), Start = now, Duration = 1.05f, Lane = lane });
        }

        private void AddBurst(int x, int y, Color color)
        {
            if (state.ReducedMotion) return;
            for (int i = 0; i < 10; i++)
            {
                particles.Add(new ParticleDot
                {
                    X = x + 0.5f,
                    Y = y + 0.5f,
                    VX = UnityEngine.Random.Range(-0.8f, 0.8f),
                    VY = UnityEngine.Random.Range(-0.8f, 0.8f),
                    Color = color.ToHex(),
                    Start = Time.time,
                    Duration = 0.55f
                });
            }
        }

        private void AddEpicBurst(int x, int y, Color color, int count, float speed)
        {
            if (state.ReducedMotion) return;
            count = Mathf.Clamp(count, 4, 32);
            speed = Mathf.Clamp(speed, 0.4f, 2.2f);
            for (int i = 0; i < count; i++)
            {
                float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                float velocity = UnityEngine.Random.Range(0.45f, speed);
                particles.Add(new ParticleDot
                {
                    X = x + 0.5f,
                    Y = y + 0.5f,
                    VX = Mathf.Cos(angle) * velocity,
                    VY = Mathf.Sin(angle) * velocity,
                    Color = color.ToHex(),
                    Start = Time.time,
                    Duration = UnityEngine.Random.Range(0.58f, 0.92f)
                });
            }
        }

        private void AddBeam(int fromX, int fromY, int toX, int toY, Color color, string kind = "shot")
        {
            if (state.ReducedMotion) return;
            float duration = kind == "meteor" ? 0.46f : kind == "meteor-small" ? 0.36f : kind == "fireball" ? 0.34f : kind == "arc" ? 0.34f : kind == "death" || kind == "hex" || kind == "heal" ? 0.28f : 0.20f;
            beams.Add(new BeamEffect { FromX = fromX, FromY = fromY, ToX = toX, ToY = toY, Color = color.ToHex(), Kind = kind, Start = Time.time, Duration = duration });
        }

        private void AddFlash(int x, int y, Color color)
        {
            if (state.ReducedMotion) return;
            flashes.Add(new CellFlash { X = x, Y = y, Color = color.ToHex(), Start = Time.time, Duration = 0.28f });
        }

        private void AddCastGlyph(CombatUnit caster, FormulaDef formula, Color color)
        {
            if (state.ReducedMotion || caster == null) return;
            string kind = CasterKnowsSchool(caster.Spell, "mend") || CasterKnowsSchool(formula?.School, "mend") ? "priest" : "wizard";
            castGlyphs.Add(new CastGlyph { X = caster.X, Y = caster.Y, Kind = kind, Color = color.ToHex(), Start = Time.time, Duration = 0.52f });
        }

        private void AddTileGlyph(int x, int y, FormulaDef formula, string kind, Color color)
        {
            if (state.ReducedMotion) return;
            castGlyphs.Add(new CastGlyph
            {
                X = x,
                Y = y,
                Kind = string.IsNullOrEmpty(kind) ? (formula != null && formula.Splash ? "area" : "impact") : kind,
                Color = color.ToHex(),
                Start = Time.time,
                Duration = formula != null && formula.Splash ? 0.62f : 0.46f
            });
        }

        private void DrawParticles(Rect grid, float cell)
        {
            float now = Time.time;
            foreach (ParticleDot p in particles)
            {
                float t = Mathf.Clamp01((now - p.Start) / p.Duration);
                Color c = p.Color.ToColor();
                c.a = 1f - t;
                DrawRect(new Rect(grid.x + (p.X + p.VX * t) * cell - 3, grid.y + (p.Y + p.VY * t) * cell - 3, 6, 6), c);
            }
            foreach (FloatText ft in floatTexts)
            {
                float t = Mathf.Clamp01((now - ft.Start) / ft.Duration);
                float w = Mathf.Clamp(cell * (1.04f + (ft.Text?.Length ?? 0) * 0.075f), cell * 1.16f, cell * 2.25f);
                float h = Mathf.Clamp(cell * 0.28f, 20f, 28f);
                float laneRise = ft.Lane * Mathf.Clamp(cell * 0.20f, 10f, 18f);
                float sway = ((ft.Lane % 3) - 1) * cell * 0.18f;
                Rect r = new Rect(grid.x + (ft.X + 0.5f) * cell - w * 0.5f + sway, grid.y + (ft.Y + 0.05f - t * 0.58f) * cell - laneRise, w, h);
                r.x = Mathf.Clamp(r.x, grid.x + 2f, grid.xMax - r.width - 2f);
                r.y = Mathf.Clamp(r.y, grid.y - 2f, grid.yMax - r.height - 2f);
                Color textColor = ft.Color.ToColor();
                DrawFloatBackplate(r, textColor.WithAlpha(1f - t * 0.18f));
                GUIStyle style = CenterStyle(Mathf.RoundToInt(Mathf.Clamp(cell * 0.18f, 12f, 16f)), textColor.WithAlpha(1f - t * 0.08f));
                GUI.Label(r, ft.Text, style);
            }
        }

        private void DrawFloatBackplate(Rect rect, Color accent)
        {
            Color tint = Color.white.WithAlpha(Mathf.Clamp(accent.a, 0.35f, 0.82f));
            if (TryDrawCombatSpellFloatAtlasIcon(Pad(rect, -Mathf.Max(2f, rect.height * 0.16f)), 11, tint))
            {
                DrawBorder(rect, accent.WithAlpha(0.50f), 1);
                return;
            }

            DrawRect(rect, Hex("030405", 0.72f));
            DrawBorder(rect, accent.WithAlpha(0.62f), 1);
        }

        private void AddTween(string id, Vector2 from, Vector2 to, TweenKind kind)
        {
            if (state.ReducedMotion) return;
            tweens.RemoveAll(t => t.Id == id);
            tweens.Add(new Tween(id, from, to, Time.time, kind == TweenKind.Lunge ? 0.14f : 0.18f, kind));
        }

        private Vector2 UnitDrawPos(CombatUnit unit)
        {
            Tween tween = tweens.LastOrDefault(t => t.Id == unit.Id);
            if (tween == null) return new Vector2(unit.X, unit.Y);
            float t = Mathf.Clamp01((Time.time - tween.Start) / tween.Duration);
            if (tween.Kind == TweenKind.Lunge) t = Mathf.Sin(t * Mathf.PI);
            return Vector2.Lerp(tween.From, tween.To, Mathf.SmoothStep(0, 1, t));
        }

        private CombatUnit CurrentUnit()
        {
            return state?.Combat?.Units?.FirstOrDefault(u => u.Id == state.Combat.ActiveId && u.Hp > 0);
        }

        private bool IsHeroUnit(CombatUnit unit)
        {
            return unit != null && unit.Side == UnitSide.Party && unit.PartyIndex >= 0 && !unit.Summoned;
        }

        private List<CombatUnit> InitiativeOrder()
        {
            if (state?.Combat?.Units == null) return new List<CombatUnit>();
            return state.Combat.Units.Where(u => u.Hp > 0).OrderByDescending(u => u.Agility + u.AttackSpeed / 3).ThenBy(u => u.Name).ToList();
        }

        private IEnumerable<CombatUnit> UpcomingUnits(int count)
        {
            List<CombatUnit> order = InitiativeOrder();
            if (order.Count == 0) yield break;
            int activeIndex = order.FindIndex(u => u.Id == state.Combat.ActiveId);
            if (activeIndex < 0) activeIndex = 0;
            for (int i = 0; i < count; i++)
            {
                yield return order[(activeIndex + i) % order.Count];
            }
        }

        private string AttackPreview(CombatUnit attacker, CombatUnit target)
        {
            if (attacker == null || target == null) return "";
            int distance = Distance(attacker.X, attacker.Y, target.X, target.Y);
            if (target.Side == attacker.Side) return "friendly target";
            if (distance > attacker.Range) return $"out of reach / range {attacker.Range}";
            if (attacker.Range > 1 && !HasLineOfSight(attacker.X, attacker.Y, target.X, target.Y, true)) return "covered\nline of sight blocked";
            int chance = AttackHitChance(attacker, target);
            string type = string.IsNullOrWhiteSpace(attacker.DamageType) ? "physical" : attacker.DamageType;
            string tag = DamageMatchNote(target, type);
            Vector2Int damage = AttackDamagePreview(attacker, target);
            string guard = target.Guarding ? $" / guarded -{Mathf.Max(2, target.GuardBonus)}" : "";
            string status = CombatStatusPreview(attacker, target);
            return $"{chance}% hit / {damage.x}-{damage.y} {type}\n{tag}{guard}{status}";
        }

        private string CoverAttackPreview(CombatUnit attacker, Point cover)
        {
            if (attacker == null || !IsBreakableCover(cover)) return "";
            int distance = Distance(attacker.X, attacker.Y, cover.X, cover.Y);
            if (distance > attacker.Range) return $"cover out of reach / range {attacker.Range}";
            if (attacker.Range > 1 && !HasLineOfSight(attacker.X, attacker.Y, cover.X, cover.Y, true)) return "cover blocked\nline of sight blocked";
            int damage = CoverBreakDamage(attacker, cover);
            int current = CoverIntegrity(cover);
            string time = cover.Duration > 0 ? $" / {cover.Duration} turns" : "";
            return $"break {CoverName(cover)} / {current} integrity{time}\nthis hit removes {damage}";
        }

        private string CombatStatusPreview(CombatUnit attacker, CombatUnit target)
        {
            List<string> notes = new List<string>();
            if (attacker != null && attacker.Hexed > 0) notes.Add($"attacker H{attacker.Hexed}");
            if (target != null)
            {
                string statuses = StatusCompactLine(target);
                if (!string.IsNullOrEmpty(statuses)) notes.Add("target " + statuses);
                if (target.Sleeping > 0) notes.Add("sleep wakes on hit");
            }
            return notes.Count == 0 ? "" : "\n" + string.Join(" / ", notes.Take(3).ToArray());
        }

        private string FormulaPreview(CombatUnit caster, FormulaDef formula, CombatUnit target, int x, int y)
        {
            if (formula == null || caster == null) return "";
            int range = EffectiveFormulaRange(formula, caster);
            int manaCost = EffectiveFormulaMana(formula, caster);
            if (Distance(caster.X, caster.Y, x, y) > range) return $"out of spell range {range}";
            if (!CanTargetFormula(formula, caster, target, x, y)) return FormulaTargetPrompt(formula);
            if (!HasFormulaLineOfSight(formula, caster, x, y)) return FormulaSightBlockText(formula) + "\nline of sight blocked";
            if (caster.Mana < manaCost) return $"needs {manaCost} mana";
            if (formula.Effect == "damage" || formula.Effect == "drain")
            {
                Vector2Int damage = FormulaDamagePreview(formula, caster, target);
                string type = string.IsNullOrEmpty(formula.DamageType) ? "magic" : formula.DamageType;
                string spill = formula.Splash ? $" / splash {SplashTargetCount(target, UnitSide.Enemy)}" : "";
                string status = string.IsNullOrEmpty(formula.Status) ? "" : $" / {StatusLabel(formula.Status)} {StatusChanceText(target, formula.Status, caster, 0.42f, true)}";
                string drain = formula.Effect == "drain" ? " / heals caster" : "";
                string arc = FormulaArcsOverCover(formula) && !HasLineOfSight(caster.X, caster.Y, x, y, true) ? " / arcs cover" : "";
                string current = target == null ? "" : StatusCompactLine(target);
                current = string.IsNullOrEmpty(current) ? "" : $" / target {current}";
                return $"{formula.Name}: {damage.x}-{damage.y} {type}{spill}{drain}{arc}\n{DamageMatchNote(target, type)}{status}{current} / {manaCost} MP{FormulaFocusNote(formula, caster)}";
            }
            if (formula.Effect == "terrain")
            {
                return $"{formula.Name}: {formula.Hint}\n{TerrainReactionPreview(formula, x, y)}{FormulaFocusNote(formula, caster)}";
            }
            if (formula.Effect == "summon")
            {
                int skill = SkillValue(caster.Skills, FormulaSkill(formula, caster));
                int turns = Mathf.Max(1, formula.Duration + (IsFocusedCaster(caster) ? 1 : 0));
                int power = Mathf.Max(3, formula.Power + skill / 5 + (IsFocusedCaster(caster) ? 1 : 0));
                string arc = FormulaArcsOverCover(formula) && !HasLineOfSight(caster.X, caster.Y, x, y, true) ? " / arcs cover" : "";
                return $"{formula.Name}: {SummonDisplayName(formula.SummonRole)} {turns} turns{arc}\nclaws {Mathf.Max(2, power - 3)}-{Mathf.Max(4, power + 1)} death / {manaCost} MP{FormulaFocusNote(formula, caster)}";
            }
            if (formula.Effect == "heal")
            {
                Vector2Int heal = FormulaHealPreview(formula, caster);
                string spread = formula.Splash ? $" / splash {SplashTargetCount(target, target?.Side ?? UnitSide.Party)}" : "";
                return $"{formula.Name}: +{heal.x}-{heal.y} HP{spread}\n{manaCost} MP{FormulaFocusNote(formula, caster)}";
            }
            if (formula.Effect == "cure")
            {
                return $"{formula.Name}: cleanse afflictions\npoison bleed web stun sleep hex / {manaCost} MP{FormulaFocusNote(formula, caster)}";
            }
            if (formula.Effect == "status")
            {
                bool hostile = formula.Target == "enemy";
                string spread = formula.Splash ? $" / splash {SplashTargetCount(target, target?.Side ?? UnitSide.Party)}" : "";
                string chance = StatusChanceText(target, formula.Status, caster, hostile ? 0.86f : 1f, hostile);
                return $"{formula.Name}: {StatusLabel(formula.Status)} {chance}{spread}\n{Mathf.Max(1, formula.Duration)} turns / {manaCost} MP{FormulaFocusNote(formula, caster)}";
            }
            return $"{formula.Name}: {formula.Hint}\n{manaCost} MP{FormulaFocusNote(formula, caster)}";
        }

        private int MoveCostTo(CombatUnit active, int x, int y)
        {
            if (active == null) return UnreachableMoveCost;
            if (x == active.X && y == active.Y) return 0;
            if (x < 0 || x >= CombatW || y < 0 || y >= CombatH) return UnreachableMoveCost;
            return ReachableMoveCosts(active)[x, y];
        }

        private int[,] ReachableMoveCosts(CombatUnit active)
        {
            int[,] costs = new int[CombatW, CombatH];
            for (int yy = 0; yy < CombatH; yy++)
            for (int xx = 0; xx < CombatW; xx++)
            {
                costs[xx, yy] = UnreachableMoveCost;
            }

            if (active == null || active.X < 0 || active.X >= CombatW || active.Y < 0 || active.Y >= CombatH) return costs;

            int maxCost = UnitMoveAllowance(active);
            Queue<Vector2Int> open = new Queue<Vector2Int>();
            costs[active.X, active.Y] = 0;
            open.Enqueue(new Vector2Int(active.X, active.Y));

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };
            while (open.Count > 0)
            {
                Vector2Int current = open.Dequeue();
                int currentCost = costs[current.x, current.y];
                for (int i = 0; i < 4; i++)
                {
                    int nx = current.x + dx[i];
                    int ny = current.y + dy[i];
                    if (!CanEnterMoveTile(active, nx, ny)) continue;
                    int stepCost = 1 + TerrainMoveExtraCost(ObstacleAt(nx, ny), active);
                    int nextCost = currentCost + stepCost;
                    if (nextCost > maxCost || nextCost >= costs[nx, ny]) continue;
                    costs[nx, ny] = nextCost;
                    open.Enqueue(new Vector2Int(nx, ny));
                }
            }

            return costs;
        }

        private bool CanEnterMoveTile(CombatUnit active, int x, int y)
        {
            if (x < 0 || x >= CombatW || y < 0 || y >= CombatH) return false;
            if (IsBlockingTerrain(ObstacleAt(x, y))) return false;
            CombatUnit blocker = UnitAt(x, y);
            return blocker == null || active != null && !string.IsNullOrEmpty(active.Id) && blocker.Id == active.Id;
        }

        private int TerrainMoveExtraCost(Point terrain, CombatUnit active = null)
        {
            if (terrain == null) return 0;
            int extra = 0;
            if (terrain.Kind == "web") extra = 2;
            if (terrain.Kind == "ice" || terrain.Kind == "gas") extra = 1;
            if (extra > 0 && active != null && string.Equals(active.Race, "fenkin", StringComparison.OrdinalIgnoreCase)) extra = Mathf.Max(0, extra - 1);
            return extra;
        }

        private string TerrainPreviewLine(Point terrain)
        {
            if (terrain == null) return "";
            if (terrain.Kind == "tree") return terrain.Duration > 0 ? $"\ntree cover: blocks shots, arcs pass, {CoverIntegrity(terrain)} integrity, {terrain.Duration} turns" : $"\ntree cover: blocks shots, arcs pass, {CoverIntegrity(terrain)} integrity";
            if (terrain.Kind == "stone") return $"\nstone block: blocks shots, {CoverIntegrity(terrain)} integrity";
            if (terrain.Kind == "fire") return "\nfire: hurts at turn start";
            if (terrain.Kind == "gas") return "\ngas: poison risk, +1 move";
            if (terrain.Kind == "web") return "\nweb: snare risk, +2 move";
            if (terrain.Kind == "ice") return "\nice: slip risk, +1 move";
            return "";
        }

        private string TerrainLogWarning(Point terrain)
        {
            if (terrain == null) return "";
            if (terrain.Kind == "fire") return "Fire will burn that tile at turn start.";
            if (terrain.Kind == "gas") return "Gas may poison anyone lingering there.";
            if (terrain.Kind == "web") return "Webbing may hold that position.";
            if (terrain.Kind == "ice") return "Ice may slip the next step.";
            return "";
        }

        private Color TerrainHighlightColor(Point terrain, float alpha)
        {
            if (terrain == null) return Hex("58b7a5", alpha);
            if (terrain.Kind == "fire") return Hex("c65c3b", Mathf.Max(alpha, 0.26f));
            if (terrain.Kind == "gas") return Hex("8fc27b", Mathf.Max(alpha, 0.24f));
            if (terrain.Kind == "web") return Hex("d9d3c4", Mathf.Max(alpha, 0.24f));
            if (terrain.Kind == "ice") return Hex("9ad6e8", Mathf.Max(alpha, 0.24f));
            return Hex("58b7a5", alpha);
        }

        private Vector2Int AttackDamagePreview(CombatUnit attacker, CombatUnit target)
        {
            if (attacker == null || target == null) return new Vector2Int(0, 0);
            string skill = attacker.Range > 1 ? "missile" : "arms";
            int skillBonus = SkillValue(attacker.Skills, skill) / 5;
            int guard = target.Guarding ? Mathf.Max(2, target.GuardBonus) : 0;
            int hexShift = (target.Hexed > 0 ? 2 : 0) - (attacker.Hexed > 0 ? 2 : 0);
            int minDamage = attacker.DamageMin > 0 ? attacker.DamageMin : Mathf.Max(1, attacker.Power - 2);
            int maxDamage = attacker.DamageMax > 0 ? attacker.DamageMax : Mathf.Max(minDamage + 1, attacker.Power + 4);
            int minRaw = Mathf.Max(1, minDamage + skillBonus + hexShift - target.Defense - target.ArmorBonus - guard);
            int maxRaw = Mathf.Max(1, maxDamage + skillBonus + hexShift - target.Defense - target.ArmorBonus - guard);
            string type = string.IsNullOrWhiteSpace(attacker.DamageType) ? "physical" : attacker.DamageType;
            return new Vector2Int(PreviewDamageAfterTraits(target, minRaw, type), PreviewDamageAfterTraits(target, maxRaw, type));
        }

        private Vector2Int FormulaDamagePreview(FormulaDef formula, CombatUnit caster, CombatUnit target)
        {
            if (formula == null || caster == null) return new Vector2Int(0, 0);
            string skill = FormulaSkill(formula, caster);
            int skillBonus = SkillValue(caster.Skills, skill) / 2;
            int focusBonus = IsFocusedCaster(caster) ? 3 : 0;
            int raceBonus = RaceFormulaPowerBonus(caster, formula);
            int minRaw = Mathf.Max(1, formula.Power + skillBonus + focusBonus + raceBonus);
            int maxRaw = Mathf.Max(1, formula.Power + skillBonus + focusBonus + raceBonus + 5);
            string type = string.IsNullOrEmpty(formula.DamageType) ? "magic" : formula.DamageType;
            return new Vector2Int(PreviewDamageAfterTraits(target, minRaw, type), PreviewDamageAfterTraits(target, maxRaw, type));
        }

        private Vector2Int FormulaHealPreview(FormulaDef formula, CombatUnit caster)
        {
            if (formula == null || caster == null) return new Vector2Int(0, 0);
            int skillBonus = SkillValue(caster.Skills, formula.Skill) / 2;
            int min = Mathf.Max(1, formula.Power + skillBonus);
            int max = Mathf.Max(min, formula.Power + skillBonus + 4);
            return new Vector2Int(min, max);
        }

        private int SplashTargetCount(CombatUnit target, UnitSide side)
        {
            if (target == null || state?.Combat?.Units == null) return 0;
            return state.Combat.Units.Count(u => u.Side == side && u.Hp > 0 && u.Id != target.Id && Distance(u.X, u.Y, target.X, target.Y) <= 1);
        }

        private float StatusApplyChance(CombatUnit target, string status, CombatUnit source, float chance, bool hostile)
        {
            if (!hostile) return 1f;
            float rollChance = chance - (target?.MagicResist ?? 0) * 0.07f;
            if (status == "sleep" && target != null && target.Fearless) rollChance *= 0.45f;
            return Mathf.Clamp01(rollChance);
        }

        private string StatusChanceText(CombatUnit target, string status, CombatUnit source, float chance, bool hostile)
        {
            if (target == null) return hostile ? "needs mark" : "100%";
            return $"{Mathf.RoundToInt(StatusApplyChance(target, status, source, chance, hostile) * 100f)}%";
        }

        private int PreviewDamageAfterTraits(CombatUnit target, int amount, string damageType)
        {
            if (target == null) return Mathf.Max(1, amount);
            string type = string.IsNullOrEmpty(damageType) ? "physical" : damageType;
            float multiplier = 1f;
            if (HasTag(target.Resist, type)) multiplier *= 0.55f;
            if (HasTag(target.Weakness, type)) multiplier *= 1.45f;
            if (target.Hexed > 0) multiplier *= 1.20f;
            int guard = target.Guarding ? Mathf.Max(2, target.GuardBonus) : 0;
            int shield = target.Shielded > 0 ? 3 : 0;
            return Mathf.Max(1, Mathf.RoundToInt(amount * multiplier) - guard - shield - GearDamageReduction(target, type) - RaceDamageReduction(target, type));
        }

        private string DamageMatchNote(CombatUnit target, string damageType)
        {
            if (target == null) return "no target";
            string type = string.IsNullOrEmpty(damageType) ? "physical" : damageType;
            if (HasTag(target.Resist, type)) return "resists";
            if (HasTag(target.Weakness, type)) return "weak";
            return "normal";
        }

        private CombatUnit UnitAt(int x, int y)
        {
            return state.Combat.Units.FirstOrDefault(u => u.Hp > 0 && u.X == x && u.Y == y);
        }

        private bool CanStandAt(int x, int y)
        {
            return x >= 0 && x < CombatW && y >= 0 && y < CombatH && !IsObstacle(x, y) && UnitAt(x, y) == null;
        }

        private bool CanGrowTreeAt(int x, int y)
        {
            if (x < 0 || x >= CombatW || y < 0 || y >= CombatH || UnitAt(x, y) != null) return false;
            Point existing = ObstacleAt(x, y);
            return existing == null || !IsBlockingTerrain(existing);
        }

        private Point ObstacleAt(int x, int y)
        {
            if (state?.Combat?.Obstacles == null) return null;
            return state.Combat.Obstacles.FirstOrDefault(o => o.X == x && o.Y == y);
        }

        private bool IsObstacle(int x, int y)
        {
            return IsBlockingTerrain(ObstacleAt(x, y));
        }

        private bool IsBlockingTerrain(Point point)
        {
            if (point == null) return false;
            return point.Kind == "tree" || point.Kind == "stone";
        }

        private bool HasLineOfSight(int ax, int ay, int bx, int by, bool missiles)
        {
            int dx = bx - ax;
            int dy = by - ay;
            int steps = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
            if (steps <= 1) return true;
            for (int i = 1; i < steps; i++)
            {
                float t = i / (float)steps;
                int x = Mathf.RoundToInt(ax + dx * t);
                int y = Mathf.RoundToInt(ay + dy * t);
                Point obstacle = ObstacleAt(x, y);
                if (obstacle == null) continue;
                if (missiles && IsBlockingTerrain(obstacle)) return false;
            }
            return true;
        }

        private int TileAt(MapData map, int x, int y)
        {
            if (map == null || x < 0 || y < 0 || x >= map.Width || y >= map.Height) return 0;
            return map.Tiles[y * map.Width + x];
        }

        private void SetTile(MapData map, int x, int y, int tile)
        {
            map.Tiles[y * map.Width + x] = tile;
        }

        private MapObject ObjectAt(MapData map, int x, int y)
        {
            return map.Objects.FirstOrDefault(o => o.X == x && o.Y == y);
        }

        private void RemoveObject(MapObject obj)
        {
            state.Map.Objects.Remove(obj);
        }

        private int Distance(int ax, int ay, int bx, int by)
        {
            return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
        }

        private string Initials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            string cleaned = name.Trim();
            return cleaned.Length == 1 ? cleaned.ToUpperInvariant() : cleaned.Substring(0, 2).ToUpperInvariant();
        }

        private void EnsurePartyCustomization()
        {
            if (state?.Party == null) return;
            if (state.Party.Count > PartySize)
            {
                state.Party = state.Party.Take(PartySize).ToList();
                selectedBuilderIndex = Mathf.Clamp(selectedBuilderIndex, 0, Mathf.Max(0, state.Party.Count - 1));
            }
            if (state.Party.Count == 0) state.Party = MakeDefaultParty();
            foreach (PartyMember member in state.Party)
            {
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

        private void PushLog(string text, Tone tone)
        {
            if (state?.Log == null) return;
            state.Log.Insert(0, new LogEntry { Text = text, Tone = tone });
            if (state.Log.Count > 50) state.Log.RemoveRange(50, state.Log.Count - 50);
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

    public enum GameMode { Muster, Explore, Combat, Defeat, Tavern, Victory }
    public enum ObjectType { Cache, Shrine, Encounter, Stairs, Camp, Town, Obelisk, Ruin, Bridge, Cave }
    public enum UnitSide { Party, Enemy }
    public enum Tone { Normal, Good, Warn }
    public enum ActionMode { Move, Attack, Cast, Guard, Elixir, Wait }
    public enum CombatPhase { ChooseAction, ChooseTarget, Resolving, EnemyThinking }
    public enum TweenKind { Move, Lunge }

    [Serializable]
    public sealed class GameState
    {
        public int SaveVersion;
        public GameMode Mode;
        public int Depth;
        public int Seed;
        public int Gold;
        public int Supplies;
        public int Elixirs;
        public int StoryChapter;
        public string ActiveStory;
        public int PlayerX;
        public int PlayerY;
        public bool ReducedMotion;
        public bool SfxMuted;
        public int SfxVolumePercent = 100;
        public List<PartyMember> Party = new List<PartyMember>();
        public List<InventoryItem> Inventory = new List<InventoryItem>();
        public List<string> DiscoveredZones = new List<string>();
        public MapData Map;
        public CombatState Combat;
        public List<LogEntry> Log = new List<LogEntry>();
    }

    [Serializable]
    public sealed class PartyMember
    {
        public string Id;
        public string Name;
        public string Role;
        public string Race;
        public string ClassKey;
        public string Origin;
        public string SpriteColor;
        public string Sigil;
        public Stats Stats;
        public int Level;
        public int Experience;
        public int SkillPoints;
        public int StatPoints;
        public int Hp;
        public int MaxHp;
        public int Mana;
        public int MaxMana;
        public int Movement;
        public int Power;
        public int Defense;
        public int Agility;
        public int Range;
        public int AttackSpeed;
        public int DamageMin;
        public int DamageMax;
        public int WeaponDamageMin;
        public int WeaponDamageMax;
        public int WeaponAttackSpeed;
        public string Spell;
        public SkillSet Skills;
        public string WeaponName;
        public int WeaponBonus;
        public string WeaponDamageType;
        public string ArmorName;
        public int ArmorBonus;
        public int WeaponStrengthBonus;
        public int WeaponIntelligenceBonus;
        public int WeaponAgilityBonus;
        public int WeaponHealthBonus;
        public int ArmorStrengthBonus;
        public int ArmorIntelligenceBonus;
        public int ArmorAgilityBonus;
        public int ArmorHealthBonus;
        public int GearStrength;
        public int GearIntelligence;
        public int GearAgility;
        public int GearHealth;
    }

    [Serializable]
    public struct Stats
    {
        public int Strength;
        public int Intelligence;
        public int Dexterity;
        public int Health;
        public int Total => Strength + Intelligence + Dexterity + Health;

        public Stats(int strength, int intelligence, int dexterity, int health)
        {
            Strength = strength;
            Intelligence = intelligence;
            Dexterity = dexterity;
            Health = health;
        }
    }

    [Serializable]
    public sealed class SkillSet
    {
        public int Arms;
        public int Missile;
        public int Mend;
        public int Ember;
        public int Hex;
        public int Guard;

        public SkillSet Normalize()
        {
            Arms = Mathf.Max(1, Arms);
            Missile = Mathf.Max(1, Missile);
            Mend = Mathf.Max(1, Mend);
            Ember = Mathf.Max(1, Ember);
            Hex = Mathf.Max(1, Hex);
            Guard = Mathf.Max(1, Guard);
            return this;
        }

        public SkillSet Clone()
        {
            return new SkillSet
            {
                Arms = Arms,
                Missile = Missile,
                Mend = Mend,
                Ember = Ember,
                Hex = Hex,
                Guard = Guard
            }.Normalize();
        }
    }

    [Serializable]
    public sealed class InventoryItem
    {
        public string Mark;
        public string Material;
        public string Form;
        public string Trait;
        public string Slot;
        public int Bonus;
        public int StrengthBonus;
        public int IntelligenceBonus;
        public int AgilityBonus;
        public int HealthBonus;
        public int DamageMin;
        public int DamageMax;
        public int AttackSpeed;
        public string Rarity;
        public string DamageType;
        public string DisplayName;
    }

    [Serializable]
    public sealed class MapData
    {
        public int Width;
        public int Height;
        public int Depth;
        public int StartX;
        public int StartY;
        public List<int> Tiles = new List<int>();
        public List<MapObject> Objects = new List<MapObject>();
    }

    public sealed class WorldZone
    {
        public string Id;
        public string Name;
        public string Title;
        public int Danger;
        public string Summary;
        public string Story;
    }

    [Serializable]
    public sealed class MapObject
    {
        public int X;
        public int Y;
        public ObjectType Type;

        public MapObject()
        {
        }

        public MapObject(int x, int y, ObjectType type)
        {
            X = x;
            Y = y;
            Type = type;
        }
    }

    [Serializable]
    public sealed class CombatState
    {
        public int Round;
        public string ActiveId;
        public bool Moved;
        public bool Acted;
        public int MovePoints;
        public bool ActionAvailable;
        public CombatPhase Phase;
        public List<CombatUnit> Units = new List<CombatUnit>();
        public List<Point> Obstacles = new List<Point>();
    }

    [Serializable]
    public sealed class CombatUnit
    {
        public string Id;
        public int PartyIndex;
        public UnitSide Side;
        public string Name;
        public string Role;
        public string Race;
        public string ClassKey;
        public string Rank;
        public string Origin;
        public string Sigil;
        public int X;
        public int Y;
        public int Hp;
        public int MaxHp;
        public int Mana;
        public int MaxMana;
        public int Movement;
        public int Power;
        public int Defense;
        public int Agility;
        public int Range;
        public int AttackSpeed;
        public int DamageMin;
        public int DamageMax;
        public string Spell;
        public SkillSet Skills;
        public string Color;
        public bool Guarding;
        public int GuardBonus;
        public int Hexed;
        public string DamageType;
        public string WeaponName;
        public int WeaponBonus;
        public string ArmorName;
        public int ArmorBonus;
        public string Resist;
        public string Weakness;
        public string StatusOnHit;
        public int MagicResist;
        public bool Fearless;
        public bool Summoned;
        public int SummonTurns;
        public string SummonerId;
        public int Poisoned;
        public int Bleeding;
        public int Stunned;
        public int Sleeping;
        public int Webbed;
        public int Shielded;
        public int Regenerating;
    }

    [Serializable]
    public sealed class Point
    {
            public int X;
            public int Y;
            public string Kind = "stone";
            public int Duration;
            public int Integrity;

        public Point()
        {
            Kind = "stone";
        }

        public Point(int x, int y)
            : this(x, y, "stone")
        {
        }

        public Point(int x, int y, string kind)
            : this(x, y, kind, 0)
        {
        }

        public Point(int x, int y, string kind, int duration)
        {
            X = x;
            Y = y;
            Kind = string.IsNullOrEmpty(kind) ? "stone" : kind;
            Duration = duration;
            Integrity = InitialIntegrity(Kind);
        }

        private static int InitialIntegrity(string kind)
        {
            if (kind == "tree") return 2;
            if (kind == "stone") return 3;
            return 0;
        }
    }

    [Serializable]
    public sealed class LogEntry
    {
        public string Text;
        public Tone Tone;
    }

    public sealed class Tween
    {
        public string Id;
        public Vector2 From;
        public Vector2 To;
        public float Start;
        public float Duration;
        public TweenKind Kind;

        public Tween(string id, Vector2 from, Vector2 to, float start, float duration, TweenKind kind)
        {
            Id = id;
            From = from;
            To = to;
            Start = start;
            Duration = duration;
            Kind = kind;
        }
    }

    public sealed class FloatText
    {
        public int X;
        public int Y;
        public string Text;
        public string Color;
        public float Start;
        public float Duration;
        public int Lane;
    }

    public sealed class ParticleDot
    {
        public float X;
        public float Y;
        public float VX;
        public float VY;
        public string Color;
        public float Start;
        public float Duration;
    }

    public sealed class BeamEffect
    {
        public int FromX;
        public int FromY;
        public int ToX;
        public int ToY;
        public string Color;
        public string Kind;
        public float Start;
        public float Duration;
    }

    public sealed class CellFlash
    {
        public int X;
        public int Y;
        public string Color;
        public float Start;
        public float Duration;
    }

    public sealed class CastGlyph
    {
        public int X;
        public int Y;
        public string Kind;
        public string Color;
        public float Start;
        public float Duration;
    }

    public sealed class EnemyTemplate
    {
        public string Name;
        public int Hp;
        public int Power;
        public int Defense;
        public int Agility;
        public int Range;
        public string Color;
        public string DamageType;
        public string Resist;
        public string Weakness;
        public string StatusOnHit;
        public int MagicResist;
        public bool Fearless;

        public static EnemyTemplate For(string kind)
        {
            switch (kind)
            {
                case "sewerrat": return new EnemyTemplate { Name = "Sewer Rat", Hp = 8, Power = 4, Defense = 0, Agility = 8, Range = 1, Color = "#7a6f5f", DamageType = "physical", Weakness = "fire" };
                case "giantrat": return new EnemyTemplate { Name = "Giant Rat", Hp = 13, Power = 5, Defense = 1, Agility = 7, Range = 1, Color = "#8f7b64", DamageType = "poison", Weakness = "fire", StatusOnHit = "poison" };
                case "adept": return new EnemyTemplate { Name = "Dust Adept", Hp = 15, Power = 7, Defense = 1, Agility = 6, Range = 3, Color = "#7b8c99", DamageType = "shock", Resist = "mind", Weakness = "physical", MagicResist = 2 };
                case "husk": return new EnemyTemplate { Name = "Iron Husk", Hp = 28, Power = 8, Defense = 5, Agility = 2, Range = 1, Color = "#8d9387", DamageType = "physical", Resist = "physical|poison", Weakness = "shock", MagicResist = 1, Fearless = true };
                case "reaver": return new EnemyTemplate { Name = "Grave Reaver", Hp = 22, Power = 9, Defense = 3, Agility = 5, Range = 1, Color = "#a34d52", DamageType = "physical", Weakness = "cold", StatusOnHit = "bleed" };
                case "spore": return new EnemyTemplate { Name = "Vermin Spore", Hp = 18, Power = 7, Defense = 2, Agility = 3, Range = 2, Color = "#7f9d5b", DamageType = "poison", Resist = "poison", Weakness = "fire", StatusOnHit = "poison" };
                case "shade": return new EnemyTemplate { Name = "Candle Shade", Hp = 20, Power = 8, Defense = 2, Agility = 7, Range = 1, Color = "#6f617e", DamageType = "death", Resist = "death|physical", Weakness = "fire", MagicResist = 3, Fearless = true };
                case "glassmage": return new EnemyTemplate { Name = "Glass Mage", Hp = 16, Power = 8, Defense = 1, Agility = 5, Range = 4, Color = "#9ad6e8", DamageType = "cold", Resist = "cold|shock", Weakness = "physical", MagicResist = 4 };
                case "thornbeast": return new EnemyTemplate { Name = "Thorn Beast", Hp = 30, Power = 10, Defense = 4, Agility = 3, Range = 1, Color = "#8f6f42", DamageType = "physical", Resist = "poison", Weakness = "fire", StatusOnHit = "bleed", Fearless = true };
                case "mirearcher": return new EnemyTemplate { Name = "Mire Archer", Hp = 17, Power = 7, Defense = 2, Agility = 6, Range = 4, Color = "#6f8f64", DamageType = "poison", Resist = "poison", Weakness = "fire", StatusOnHit = "poison" };
                case "bonepriest": return new EnemyTemplate { Name = "Bone Priest", Hp = 18, Power = 6, Defense = 2, Agility = 4, Range = 3, Color = "#d9d3c4", DamageType = "light", Resist = "death|mind", Weakness = "shock", MagicResist = 4 };
                case "cinderling": return new EnemyTemplate { Name = "Cinderling", Hp = 19, Power = 8, Defense = 2, Agility = 6, Range = 3, Color = "#c65c3b", DamageType = "fire", Resist = "fire", Weakness = "cold", StatusOnHit = "bleed" };
                case "gloamknight": return new EnemyTemplate { Name = "Gloam Knight", Hp = 32, Power = 10, Defense = 5, Agility = 3, Range = 1, Color = "#6f617e", DamageType = "death", Resist = "death|physical", Weakness = "light|shock", MagicResist = 2, Fearless = true };
                case "koboldraider": return new EnemyTemplate { Name = "Kobold Raider", Hp = 16, Power = 7, Defense = 2, Agility = 7, Range = 1, Color = "#7f9d5b", DamageType = "physical", Weakness = "cold", StatusOnHit = "bleed" };
                case "koboldslinger": return new EnemyTemplate { Name = "Kobold Slinger", Hp = 14, Power = 6, Defense = 1, Agility = 8, Range = 4, Color = "#8f8f58", DamageType = "physical", Weakness = "shock", StatusOnHit = "stun" };
                case "koboldshaman": return new EnemyTemplate { Name = "Kobold Shaman", Hp = 17, Power = 7, Defense = 1, Agility = 5, Range = 4, Color = "#6f8f64", DamageType = "mind", Resist = "poison|mind", Weakness = "physical", StatusOnHit = "hex", MagicResist = 3 };
                case "koboldwizard": return new EnemyTemplate { Name = "Kobold Bone Wizard", Hp = 18, Power = 9, Defense = 1, Agility = 6, Range = 4, Color = "#8d6dcc", DamageType = "death", Resist = "death|mind", Weakness = "physical|light", StatusOnHit = "hex", MagicResist = 4 };
                case "koboldshield": return new EnemyTemplate { Name = "Kobold Shieldbearer", Hp = 22, Power = 7, Defense = 4, Agility = 4, Range = 1, Color = "#7c6f45", DamageType = "physical", Resist = "physical", Weakness = "fire", MagicResist = 1 };
                case "ratfolk": return new EnemyTemplate { Name = "Ratfolk Scrapper", Hp = 14, Power = 6, Defense = 1, Agility = 7, Range = 1, Color = "#8f7b64", DamageType = "physical", Weakness = "fire", StatusOnHit = "bleed" };
                case "ratcutthroat": return new EnemyTemplate { Name = "Ratfolk Cutthroat", Hp = 15, Power = 7, Defense = 1, Agility = 9, Range = 1, Color = "#7a6f5f", DamageType = "poison", Weakness = "fire", StatusOnHit = "poison" };
                case "ratmage": return new EnemyTemplate { Name = "Ratfolk Plague Mage", Hp = 16, Power = 7, Defense = 1, Agility = 6, Range = 4, Color = "#8fc27b", DamageType = "poison", Resist = "poison|mind", Weakness = "fire", StatusOnHit = "poison", MagicResist = 3 };
                case "ratcleric": return new EnemyTemplate { Name = "Ratfolk Cistern Cleric", Hp = 17, Power = 5, Defense = 2, Agility = 4, Range = 3, Color = "#d9d3c4", DamageType = "light", Resist = "poison|mind", Weakness = "shock", MagicResist = 3 };
                case "ratbrute": return new EnemyTemplate { Name = "Ratfolk Brute", Hp = 28, Power = 9, Defense = 4, Agility = 3, Range = 1, Color = "#9b6b45", DamageType = "physical", Resist = "physical", Weakness = "fire", StatusOnHit = "stun" };
                case "drowscout": return new EnemyTemplate { Name = "Drow Scout", Hp = 18, Power = 7, Defense = 2, Agility = 9, Range = 1, Color = "#8d6dcc", DamageType = "physical", Resist = "mind", Weakness = "light" };
                case "drowblade": return new EnemyTemplate { Name = "Drow Blade Dancer", Hp = 22, Power = 9, Defense = 2, Agility = 10, Range = 1, Color = "#8d6dcc", DamageType = "physical", Resist = "mind", Weakness = "light", StatusOnHit = "bleed" };
                case "drowcrossbow": return new EnemyTemplate { Name = "Drow Crossbow", Hp = 18, Power = 8, Defense = 2, Agility = 8, Range = 5, Color = "#8d6dcc", DamageType = "physical", Resist = "mind", Weakness = "shock" };
                case "drowmage": return new EnemyTemplate { Name = "Drow Mage", Hp = 17, Power = 9, Defense = 1, Agility = 7, Range = 4, Color = "#8d6dcc", DamageType = "mind", Resist = "mind|death", Weakness = "light|physical", StatusOnHit = "hex", MagicResist = 4 };
                case "drowpriest": return new EnemyTemplate { Name = "Drow Priest", Hp = 20, Power = 7, Defense = 2, Agility = 5, Range = 4, Color = "#d9d3c4", DamageType = "death", Resist = "mind|death", Weakness = "light|shock", StatusOnHit = "hex", MagicResist = 5 };
                case "lesserdemon": return new EnemyTemplate { Name = "Lesser Demon", Hp = 30, Power = 10, Defense = 4, Agility = 5, Range = 1, Color = "#c65c3b", DamageType = "fire", Resist = "fire|death", Weakness = "cold|light", StatusOnHit = "bleed", MagicResist = 3, Fearless = true };
                case "meteorlich": return new EnemyTemplate { Name = "Vhal Rakh, Meteor Crown", Hp = 54, Power = 14, Defense = 4, Agility = 7, Range = 5, Color = "#d98b6a", DamageType = "fire", Resist = "fire|death|mind", Weakness = "cold|light", StatusOnHit = "hex", MagicResist = 7, Fearless = true };
                case "ritualheart": return new EnemyTemplate { Name = "Ritual Heart", Hp = 44, Power = 11, Defense = 6, Agility = 1, Range = 4, Color = "#b94b56", DamageType = "death", Resist = "death|poison|mind", Weakness = "light|shock", StatusOnHit = "bleed", MagicResist = 6, Fearless = true };
                default: return new EnemyTemplate { Name = "Fallen Sentry", Hp = 18, Power = 6, Defense = 2, Agility = 4, Range = 1, Color = "#9b6b45", DamageType = "physical", Weakness = "death" };
            }
        }
    }

    public sealed class FormulaDef
    {
        public string Code;
        public string Name;
        public string Hint;
        public string School;
        public string Skill;
        public int Mana;
        public int Range;
        public string Target;
        public string Effect;
        public string Terrain;
        public string DamageType;
        public string Status;
        public int Power;
        public int Duration;
        public bool Splash;
        public bool Arc;
        public string SummonRole;
    }

    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static string ToHex(this Color color)
        {
            Color32 c = color;
            return $"#{c.r:X2}{c.g:X2}{c.b:X2}";
        }

        public static Color ToColor(this string hex)
        {
            if (string.IsNullOrEmpty(hex)) return Color.white;
            if (hex.StartsWith("#")) hex = hex.Substring(1);
            byte r = Convert.ToByte(hex.Substring(0, 2), 16);
            byte g = Convert.ToByte(hex.Substring(2, 2), 16);
            byte b = Convert.ToByte(hex.Substring(4, 2), 16);
            return new Color32(r, g, b, 255);
        }
    }
}
