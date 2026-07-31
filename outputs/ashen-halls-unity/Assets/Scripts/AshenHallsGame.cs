using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace AshenHalls
{
    public static class TavernMenuRules
    {
        private static readonly string[] choicesWithoutSave = { "New Game", "Settings", "Exit Game" };
        private static readonly string[] choicesWithSave = { "Continue", "New Game", "Settings", "Exit Game" };

        public static IReadOnlyList<string> NormalChoiceLabels(bool saveExists)
        {
            return saveExists ? choicesWithSave : choicesWithoutSave;
        }

        public static int NormalChoiceCount(bool saveExists)
        {
            return NormalChoiceLabels(saveExists).Count;
        }

        public static bool ShowContinue(bool saveExists)
        {
            return saveExists;
        }

        public static bool ShowDeveloperTesting(bool developmentBuild)
        {
            return developmentBuild;
        }
    }

    public static class CampaignCheckpointRules
    {
        public static bool ShouldWrite(GameState gameState, bool labSaveBlocked, bool batchMode)
        {
            return gameState != null
                && !labSaveBlocked
                && !batchMode
                && gameState.Mode == GameMode.Explore
                && gameState.Combat == null;
        }
    }

    public static class VisualSmokeLaunchRules
    {
        public static bool BlockPersistence(IEnumerable<string> commandLineArgs)
        {
            return commandLineArgs != null
                && commandLineArgs.Any(arg =>
                    string.Equals(arg, "-ashen-capture", StringComparison.OrdinalIgnoreCase)
                    || !string.IsNullOrWhiteSpace(arg)
                    && arg.StartsWith("-ashen-", StringComparison.OrdinalIgnoreCase)
                    && arg.EndsWith("-smoke", StringComparison.OrdinalIgnoreCase));
        }

        public static bool BlockLegacyImport(bool visualSmokeSaveBlocked, bool batchMode)
        {
            return visualSmokeSaveBlocked || batchMode;
        }
    }

    public readonly struct ExplorationUseTarget
    {
        public readonly MapObject Target;
        public readonly int StepX;
        public readonly int StepY;

        public ExplorationUseTarget(MapObject target, int stepX, int stepY)
        {
            Target = target;
            StepX = stepX;
            StepY = stepY;
        }

        public bool IsUnderfoot => StepX == 0 && StepY == 0;
    }

    public static class ExplorationInteractionRules
    {
        public static bool TryFindUseTarget(
            MapData map,
            int playerX,
            int playerY,
            Func<MapObject, bool> isCurrentObjective,
            Func<int, int, bool> canStepTo,
            Func<MapObject, bool> canUseAdjacentWithoutStanding,
            out ExplorationUseTarget selection)
        {
            return TryFindUseTarget(
                map,
                playerX,
                playerY,
                isCurrentObjective,
                canStepTo,
                canUseAdjacentWithoutStanding,
                0,
                0,
                out selection);
        }

        public static bool TryFindUseTarget(
            MapData map,
            int playerX,
            int playerY,
            Func<MapObject, bool> isCurrentObjective,
            Func<int, int, bool> canStepTo,
            Func<MapObject, bool> canUseAdjacentWithoutStanding,
            int preferredStepX,
            int preferredStepY,
            out ExplorationUseTarget selection)
        {
            selection = new ExplorationUseTarget();
            if (map?.Objects == null) return false;

            MapObject underfoot = ObjectAt(map, playerX, playerY);
            if (underfoot != null && underfoot.Type == ObjectType.Stairs)
            {
                selection = new ExplorationUseTarget(underfoot, 0, 0);
                return true;
            }

            int bestScore = int.MaxValue;
            ExplorationUseTarget best = new ExplorationUseTarget();
            bool found = false;

            void Consider(int tx, int ty, int stepX, int stepY)
            {
                if (tx < 0 || ty < 0 || tx >= map.Width || ty >= map.Height) return;
                MapObject obj = ObjectAt(map, tx, ty);
                if (!IsUseObject(obj)) return;
                if ((stepX != 0 || stepY != 0)
                    && canStepTo != null
                    && !canStepTo(tx, ty)
                    && (canUseAdjacentWithoutStanding == null || !canUseAdjacentWithoutStanding(obj)))
                {
                    return;
                }
                int score = UsePriority(obj.Type, isCurrentObjective != null && isCurrentObjective(obj)) * 10;
                if ((preferredStepX != 0 || preferredStepY != 0)
                    && stepX == preferredStepX
                    && stepY == preferredStepY)
                {
                    score -= 1;
                }
                if (score >= bestScore) return;
                bestScore = score;
                best = new ExplorationUseTarget(obj, stepX, stepY);
                found = true;
            }

            Consider(playerX, playerY, 0, 0);
            Consider(playerX, playerY - 1, 0, -1);
            Consider(playerX - 1, playerY, -1, 0);
            Consider(playerX + 1, playerY, 1, 0);
            Consider(playerX, playerY + 1, 0, 1);

            selection = best;
            return found;
        }

        public static bool IsUseObject(MapObject obj)
        {
            return obj != null && IsUseObject(obj.Type);
        }

        public static bool IsUseObject(ObjectType type)
        {
            if (type == ObjectType.CityWall) return false;
            return type == ObjectType.Cache
                || type == ObjectType.Shrine
                || type == ObjectType.Encounter
                || type == ObjectType.Stairs
                || type == ObjectType.Cave
                || type == ObjectType.Camp
                || type == ObjectType.Town
                || IsRouteScaffoldObject(type)
                || IsMidgaardTownObject(type);
        }

        public static int UsePriority(ObjectType type, bool currentObjective)
        {
            if (currentObjective) return 0;
            switch (type)
            {
                case ObjectType.Stairs: return 10;
                case ObjectType.Cave: return 14;
                case ObjectType.Encounter: return 16;
                case ObjectType.Cache: return 20;
                case ObjectType.Shrine: return 22;
                case ObjectType.Sewer: return 24;
                case ObjectType.KingHall: return 26;
                case ObjectType.InteriorDoor: return 27;
                case ObjectType.RecallCircle: return 28;
                case ObjectType.TempleHealer:
                case ObjectType.MarketClerk:
                case ObjectType.TavernKeeper:
                case ObjectType.GateCaptain:
                case ObjectType.CityCourier:
                case ObjectType.WoundedTraveler:
                case ObjectType.StableHand:
                case ObjectType.RoyalHerald:
                case ObjectType.NoviceHealer:
                case ObjectType.OldRoadScout:
                case ObjectType.TownGuard:
                case ObjectType.KingHalvard:
                case ObjectType.DinerCook:
                case ObjectType.Provisioner:
                case ObjectType.DockWorker:
                case ObjectType.Scholar:
                    return 30;
                case ObjectType.QuestBoard:
                case ObjectType.Waystone:
                case ObjectType.TrainingGround:
                case ObjectType.LoreLibrary:
                case ObjectType.ForgeSite:
                case ObjectType.FactionCamp:
                case ObjectType.DungeonGate:
                case ObjectType.DeepCrypt:
                case ObjectType.AncientGrove:
                case ObjectType.PortalSeal:
                    return 36;
                case ObjectType.Armorer:
                case ObjectType.RatPeltQuest:
                case ObjectType.WeaponVendor:
                case ObjectType.Enchanter:
                case ObjectType.Provisions:
                case ObjectType.Diner:
                case ObjectType.Tavern:
                case ObjectType.Temple:
                case ObjectType.Market:
                case ObjectType.ArmorerNpc:
                case ObjectType.WeaponMerchantNpc:
                case ObjectType.EnchanterNpc:
                    return 40;
                case ObjectType.EastGate:
                case ObjectType.WestGate:
                case ObjectType.NorthGate:
                case ObjectType.SouthGate:
                    return 46;
                default:
                    return 80;
            }
        }

        private static bool IsRouteScaffoldObject(ObjectType type)
        {
            return type == ObjectType.QuestBoard
                || type == ObjectType.Waystone
                || type == ObjectType.TrainingGround
                || type == ObjectType.LoreLibrary
                || type == ObjectType.ForgeSite
                || type == ObjectType.FactionCamp
                || type == ObjectType.DungeonGate
                || type == ObjectType.DeepCrypt
                || type == ObjectType.AncientGrove
                || type == ObjectType.PortalSeal;
        }

        private static bool IsMidgaardTownObject(ObjectType type)
        {
            switch (type)
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
                case ObjectType.TownGuard:
                case ObjectType.KingHall:
                case ObjectType.Sewer:
                case ObjectType.Provisions:
                case ObjectType.RatPeltQuest:
                case ObjectType.RecallCircle:
                case ObjectType.MarketClerk:
                case ObjectType.TempleHealer:
                case ObjectType.TavernKeeper:
                case ObjectType.GateCaptain:
                case ObjectType.CityCourier:
                case ObjectType.WoundedTraveler:
                case ObjectType.StableHand:
                case ObjectType.RoyalHerald:
                case ObjectType.NoviceHealer:
                case ObjectType.OldRoadScout:
                case ObjectType.InteriorDoor:
                case ObjectType.KingHalvard:
                case ObjectType.ArmorerNpc:
                case ObjectType.WeaponMerchantNpc:
                case ObjectType.EnchanterNpc:
                case ObjectType.DinerCook:
                case ObjectType.Provisioner:
                case ObjectType.DockWorker:
                case ObjectType.Scholar:
                    return true;
                default:
                    return false;
            }
        }

        private static MapObject ObjectAt(MapData map, int x, int y)
        {
            return map.Objects.FirstOrDefault(o => o != null && o.X == x && o.Y == y);
        }
    }

    public readonly struct CombatCommandEntry
    {
        public readonly ActionMode Mode;
        public readonly string Label;
        public readonly string Hotkey;

        public CombatCommandEntry(ActionMode mode, string label, string hotkey)
        {
            Mode = mode;
            Label = label;
            Hotkey = hotkey;
        }
    }

    public static class CombatCommandPresentationRules
    {
        public static IReadOnlyList<CombatCommandEntry> PrimaryCommandsFor(CombatUnit active)
        {
            ActionMode abilityMode = HasMartialAbilities(active) ? ActionMode.Ability : ActionMode.Cast;
            string abilityLabel = abilityMode == ActionMode.Ability ? "Skills" : "Spells";
            return new[]
            {
                new CombatCommandEntry(ActionMode.Move, "Move", "WASD"),
                new CombatCommandEntry(ActionMode.Attack, "Attack", "F"),
                new CombatCommandEntry(abilityMode, abilityLabel, "C"),
                new CombatCommandEntry(ActionMode.Guard, "Guard", "G"),
                new CombatCommandEntry(ActionMode.Elixir, "Elixir", "H"),
                new CombatCommandEntry(ActionMode.Wait, "End Turn", "Space")
            };
        }

        public static bool ShouldPromoteEndTurn(bool playerTurn, int movePoints, bool actionAvailable, bool stunned, bool sleeping)
        {
            if (!playerTurn) return false;
            if (stunned || sleeping) return true;
            return movePoints <= 0 && !actionAvailable;
        }

        public static bool HasMartialAbilities(CombatUnit unit)
        {
            if (unit == null || unit.Summoned) return false;
            string cls = (unit.ClassKey ?? "").ToLowerInvariant();
            if (string.IsNullOrEmpty(cls)) cls = ClassForRole(unit.Role);
            return cls == "warrior" || cls == "rogue" || cls == "ranger";
        }

        private static string ClassForRole(string role)
        {
            switch ((role ?? "").ToLowerInvariant())
            {
                case "shield":
                case "pike": return "warrior";
                case "bow": return "ranger";
                case "knife": return "rogue";
                case "mender": return "priest";
                case "ember": return "mage";
                case "hex": return "warlock";
                case "ward": return "paladin";
                default: return "";
            }
        }
    }

    public enum CombatHotkeyKind
    {
        Dedicated,
        Navigation,
        Submit
    }

    public static class CombatInputRoutingRules
    {
        public static bool ShouldRouteToWorld(bool combatHudOwnsSelection, CombatHotkeyKind kind)
        {
            return !combatHudOwnsSelection || kind == CombatHotkeyKind.Dedicated;
        }
    }

    public sealed partial class AshenHallsGame : MonoBehaviour
    {
    }
}
