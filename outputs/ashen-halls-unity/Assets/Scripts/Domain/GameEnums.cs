namespace AshenHalls
{
    public enum GameMode { Muster, Explore, Combat, Defeat, Tavern, Victory }

    public enum ObjectType
    {
        Cache,
        Shrine,
        Encounter,
        Stairs,
        Camp,
        Town,
        Obelisk,
        Ruin,
        Bridge,
        Cave,
        Market,
        Temple,
        Fountain,
        Diner,
        Tavern,
        Armorer,
        WeaponVendor,
        Enchanter,
        EastGate,
        WestGate,
        TownGuard,
        KingHall,
        Sewer,
        CityWall,
        Provisions,
        RatPeltQuest,
        RecallCircle,
        QuestBoard,
        Waystone,
        TrainingGround,
        LoreLibrary,
        ForgeSite,
        FactionCamp,
        DungeonGate,
        DeepCrypt,
        AncientGrove,
        PortalSeal,
        MarketClerk,
        TempleHealer,
        TavernKeeper,
        GateCaptain,
        CityCourier,
        WoundedTraveler,
        StableHand,
        RoyalHerald,
        NoviceHealer,
        OldRoadScout,
        NorthGate,
        SouthGate,
        InteriorDoor,
        KingHalvard,
        ArmorerNpc,
        WeaponMerchantNpc,
        EnchanterNpc,
        RoyalThrone,
        RoyalBanner,
        RoyalLectern,
        RoyalBrazier,
        ArmorDisplay,
        WeaponDisplay,
        EnchantmentTable,
        ProvisionShelf,
        MerchantCounter
    }

    public enum UnitSide { Party, Enemy }
    public enum Tone { Normal, Good, Warn }
    public enum ActionMode { Move, Attack, Cast, Ability, Guard, Elixir, Wait }
    public enum CombatPhase { ChooseAction, ChooseTarget, Resolving, EnemyThinking }
    public enum TweenKind { Move, Lunge }
    public enum EncounterId { RandomEncounter, Patrol, Guard, FinalGate, BetaLab, MartialLab, KoboldKing, KoboldCave, KoboldAmbush, MidgaardSewer }
}
