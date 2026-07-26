namespace AshenHalls
{
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
            return EnemyCatalog.For(kind);
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

    public sealed class EncounterDefinition
    {
        public EncounterId Id;
        public string LegacyStyle;
        public string Banner;
        public string Intro;
        public string[] EnemyIds;
        public int FixedEnemyCount;
        public int GeneratedCountBonus;
        public int RandomObstacleCount;
        public Point[] EnemyPlacements;
        public Point[] PartyPlacements;
        public Point[] Obstacles;
        public bool UsesGeneratedEnemyPool;
        public bool DevelopmentOnly;
        public bool BoostMartialLabParty;
        public bool WoundFirstEnemy;
        public bool NormalizeKoboldKing;

        public int EnemyCountForDepth(int depth)
        {
            if (!UsesGeneratedEnemyPool) return FixedEnemyCount > 0 ? FixedEnemyCount : (EnemyIds?.Length ?? 0);
            return UnityEngine.Mathf.Clamp(3 + depth / 2 + GeneratedCountBonus, 3, 7);
        }
    }
}
