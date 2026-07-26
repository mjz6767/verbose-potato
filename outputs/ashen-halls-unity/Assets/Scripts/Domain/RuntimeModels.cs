using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    [Serializable]
    public sealed class GameState
    {
        public int SaveVersion;
        public string ContentSetId = ContentSetCatalog.SewerSlice;
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
        public bool MusicMuted;
        public int SfxVolumePercent = 100;
        public int MusicVolumePercent = 65;
        public int ExplorationSteps;
        public List<PartyMember> Party = new List<PartyMember>();
        public List<InventoryItem> Inventory = new List<InventoryItem>();
        public List<string> StoryFlags = new List<string>();
        public List<string> DiscoveredZones = new List<string>();
        public string ActiveRouteWaypointKey = "";
        public List<RoamingThreat> RoamingThreats = new List<RoamingThreat>();
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
        public string EquippedById;
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
        public string PermanentEnchantmentId;
        public string TemporaryEnchantmentId;
        public int TemporaryEnchantmentVictoriesRemaining;
        public bool EnchantmentBaseCaptured;
        public string EnchantmentBaseDisplayName;
        public string EnchantmentBaseTrait;
        public string EnchantmentBaseDamageType;
    }

    [Serializable]
    public enum ExplorationMaterial
    {
        NaturalGround = 0,
        PackedDirt = 1,
        CityPaving = 2,
        MarketCobbles = 3,
        TempleStone = 4,
        KeepStone = 5,
        SewerBrick = 6,
        QuarryStone = 7,
        GlassRubble = 8,
        FenMud = 9,
        RedAsh = 10,
        GloamStone = 11,
        CisternBrick = 12,
        Moss = 13,
        RuinedPaving = 14,
        ShallowWater = 15,
        DeepWater = 16,
        Forest = 17,
        Cliff = 18,
        RedBasalt = 19,
        RuinedWall = 20,
        CityWall = 21,
        BridgeDeck = 22
    }

    [Flags]
    public enum ExplorationCellRole
    {
        None = 0,
        Trail = 1 << 0,
        Road = 1 << 1,
        Room = 1 << 2,
        Plaza = 1 << 3,
        Threshold = 1 << 4,
        Water = 1 << 5,
        Bridge = 1 << 6,
        Hazard = 1 << 7,
        City = 1 << 8,
        Clearing = 1 << 9
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
        public List<int> SurfaceMaterials = new List<int>();
        public List<int> SurfaceRoles = new List<int>();
        public List<MapObject> Objects = new List<MapObject>();

        [NonSerialized] private Dictionary<long, MapObject> objectsByCell;
        [NonSerialized] private Dictionary<string, MapObject> objectsById;
        [NonSerialized] private List<MapObject> objectLookupSource;
        [NonSerialized] private int objectLookupSourceCount = -1;

        public MapObject FindObjectAt(int x, int y)
        {
            EnsureObjectLookup();
            MapObject obj;
            return objectsByCell.TryGetValue(CellKey(x, y), out obj) ? obj : null;
        }

        public MapObject FindObjectById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            EnsureObjectLookup();
            MapObject obj;
            return objectsById.TryGetValue(id, out obj) ? obj : null;
        }

        // Call after same-count list edits/reordering or changing an object's Id/X/Y.
        // List replacement and count changes are detected automatically.
        public void InvalidateObjectLookup()
        {
            objectsByCell = null;
            objectsById = null;
            objectLookupSource = null;
            objectLookupSourceCount = -1;
        }

        private void EnsureObjectLookup()
        {
            List<MapObject> source = Objects;
            int objectCount = source?.Count ?? 0;
            if (objectsByCell != null
                && objectsById != null
                && ReferenceEquals(objectLookupSource, source)
                && objectLookupSourceCount == objectCount)
            {
                return;
            }

            Dictionary<long, MapObject> cellLookup = new Dictionary<long, MapObject>(objectCount);
            Dictionary<string, MapObject> idLookup =
                new Dictionary<string, MapObject>(objectCount, StringComparer.Ordinal);

            if (source != null)
            {
                for (int i = 0; i < source.Count; i++)
                {
                    MapObject obj = source[i];
                    if (obj == null) continue;

                    long cell = CellKey(obj.X, obj.Y);
                    if (!cellLookup.ContainsKey(cell)) cellLookup.Add(cell, obj);
                    if (!string.IsNullOrWhiteSpace(obj.Id) && !idLookup.ContainsKey(obj.Id))
                    {
                        idLookup.Add(obj.Id, obj);
                    }
                }
            }

            objectsByCell = cellLookup;
            objectsById = idLookup;
            objectLookupSource = source;
            objectLookupSourceCount = source?.Count ?? 0;
        }

        private static long CellKey(int x, int y)
        {
            return ((long)x << 32) | (uint)y;
        }
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

    public sealed class RouteScaffoldDef
    {
        public string ZoneId;
        public ObjectType Type;
        public int MinDepth;
        public int ArtIndex;
        public string Name;
        public string Purpose;
        public string Summary;
        public string Icon;
        public Color Accent;

        public RouteScaffoldDef(string zoneId, ObjectType type, int minDepth, int artIndex, string name, string purpose, string summary, string icon, Color accent)
        {
            ZoneId = zoneId;
            Type = type;
            MinDepth = minDepth;
            ArtIndex = artIndex;
            Name = name;
            Purpose = purpose;
            Summary = summary;
            Icon = icon;
            Accent = accent;
        }
    }

    [Serializable]
    public sealed class MapObject
    {
        public string Id;
        public string TargetId;
        public int X;
        public int Y;
        public ObjectType Type;

        public MapObject()
        {
        }

        public MapObject(int x, int y, ObjectType type)
            : this(x, y, type, "", "")
        {
        }

        public MapObject(int x, int y, ObjectType type, string id, string targetId = "")
        {
            Id = id ?? "";
            TargetId = targetId ?? "";
            X = x;
            Y = y;
            Type = type;
        }
    }

    [Serializable]
    public sealed class RoamingThreat
    {
        public string Id;
        public string Name;
        public string Archetype;
        public int Depth;
        public int X;
        public int Y;
        public int HomeX;
        public int HomeY;
        public bool Active = true;
        public bool Alerted;
        public int GraceSteps;
        public int RespawnSteps;
    }

    [Serializable]
    public sealed class CombatState
    {
        public int Round;
        public string EncounterStyle;
        public string RoamingThreatId;
        public string ActiveId;
        public bool Moved;
        public bool Acted;
        public int MovePoints;
        public bool ActionAvailable;
        public CombatPhase Phase;
        public List<string> InitiativeQueue = new List<string>();
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
        public int Level;
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
        public int Stealthed;
        public int DemonFormTurns;
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
            int ritualIntegrity = CombatRitualRules.MaxIntegrity(kind);
            if (ritualIntegrity > 0) return ritualIntegrity;
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
        public int IconIndex = -1;
        public float Start;
        public float Duration;
        public int Lane;
        public float OffsetX;
        public float OffsetY;
        public int Serial;
    }

    public sealed class ParticleDot
    {
        public float X;
        public float Y;
        public float VX;
        public float VY;
        public string Color;
        public string Kind;
        public float Size;
        public float Gravity;
        public int Seed;
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

    public sealed class PowerImpactEcho
    {
        public int X;
        public int Y;
        public string Color;
        public string Kind;
        public int Intensity;
        public int ReactionCount;
        public float Start;
        public float ImpactAt;
        public float Duration;
    }

    public sealed class PowerCastAura
    {
        public int SourceX;
        public int SourceY;
        public int TargetX;
        public int TargetY;
        public string Color;
        public string Kind;
        public int Intensity;
        public bool Focused;
        public float Start;
        public float ImpactAt;
        public float Duration;
    }
}
