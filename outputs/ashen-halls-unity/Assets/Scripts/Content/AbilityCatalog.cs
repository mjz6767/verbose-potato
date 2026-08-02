using System.Collections.Generic;
using System.Linq;

namespace AshenHalls
{
    public static class AbilityCatalog
    {
        private static readonly Dictionary<string, string[]> idsByClass = new Dictionary<string, string[]>
        {
            ["warrior"] = new[] { "charge", "rally", "shieldbash", "execute", "cleave", "whirlwind", "sunder" },
            ["rogue"] = new[] { "stealth", "ambush", "throwknife", "smokebomb", "hamstring", "eviscerate", "shadowstep" },
            ["ranger"] = new[] { "aimedshot", "pinningshot", "scoutmark", "volley", "broadheadshot", "disruptingshot", "quickshot" },
            ["demon"] = new[] { "riftpounce", "abyssalwhirl", "soulrend", "dreadroar" }
        };

        public static IEnumerable<string> IdsForClass(string classKey)
        {
            classKey = (classKey ?? "").ToLowerInvariant();
            return idsByClass.TryGetValue(classKey, out string[] ids) ? ids : Enumerable.Empty<string>();
        }

        public static List<MartialAbility> ForClass(string classKey)
        {
            return IdsForClass(classKey).Select(For).Where(a => a != null).ToList();
        }

        public static MartialAbility For(string id)
        {
            switch ((id ?? "").ToLowerInvariant())
            {
                case "charge": return new MartialAbility { Id = "charge", Name = "Charge", Short = "CHG", ClassKey = "warrior", RequiredLevel = 1, Range = 5, Targeted = true, Summary = "Rush next to an enemy, hit, and stun for 1 turn.", Detail = "Charge uses path-aware movement up to 5 tiles and needs an open adjacent landing tile." };
                case "execute": return new MartialAbility { Id = "execute", Name = "Execute", Short = "EXE", ClassKey = "warrior", RequiredLevel = 5, Range = 1, Targeted = true, Summary = "Heavy finishing blow against badly wounded enemies.", Detail = "Execute is only valid when the enemy is at 35% HP or lower. Guard still reduces final damage." };
                case "shieldbash": return new MartialAbility { Id = "shieldbash", Name = "Shield Bash", Short = "BSH", ClassKey = "warrior", RequiredLevel = 3, Range = 1, Targeted = true, Summary = "Shove an adjacent enemy; blocked foes suffer a stunning collision.", Detail = "Shield Bash pushes the target one tile away when space is open. A wall, obstacle, unit, or board edge turns the shove into extra collision damage and a guaranteed stun." };
                case "rally": return new MartialAbility { Id = "rally", Name = "Rally", Short = "RLY", ClassKey = "warrior", RequiredLevel = 1, Range = 0, Targeted = false, Summary = "Brace the warrior and ward adjacent allies.", Detail = "Rally spends the action, gives the warrior a stronger ward, and gives adjacent allies a brief ward." };
                case "cleave": return new MartialAbility { Id = "cleave", Name = "Cleave", Short = "CLV", ClassKey = "warrior", RequiredLevel = 8, Range = 1, Targeted = true, Summary = "Heavy cut through one foe into another.", Detail = "Cleave hits the chosen adjacent enemy and clips one more adjacent enemy if the line is crowded." };
                case "whirlwind": return new MartialAbility { Id = "whirlwind", Name = "Whirlwind", Short = "WW", ClassKey = "warrior", RequiredLevel = 12, Range = 1, Targeted = false, Summary = "Strike every adjacent enemy.", Detail = "Whirlwind hits all enemies touching the warrior, trading single-target force for crowd control." };
                case "sunder": return new MartialAbility { Id = "sunder", Name = "Sunder", Short = "SUN", ClassKey = "warrior", RequiredLevel = 16, Range = 1, Targeted = true, Summary = "Measured strike that breaks Guard and tears away warding.", Detail = "Sunder deals moderate physical damage, ends Guard, and removes up to 2 turns of ward. It is reliable setup, not a finisher." };
                case "stealth": return new MartialAbility { Id = "stealth", Name = "Stealth", Short = "STL", ClassKey = "rogue", RequiredLevel = 1, Range = 0, Targeted = false, Summary = "Slip into shadow and become less likely to be targeted.", Detail = "Stealth lasts into the rogue's next turns and empowers Ambush." };
                case "ambush": return new MartialAbility { Id = "ambush", Name = "Ambush", Short = "AMB", ClassKey = "rogue", RequiredLevel = 1, Range = 1, Targeted = true, Summary = "High-accuracy strike; stronger from stealth.", Detail = "Ambush breaks stealth. From stealth it deals extra damage and can stun the target." };
                case "throwknife": return new MartialAbility { Id = "throwknife", Name = "Throw Knife", Short = "THR", ClassKey = "rogue", RequiredLevel = 3, Range = 3, Targeted = true, Summary = "Short ranged cut that can bleed.", Detail = "Throw Knife gives rogues a modest ranged option, but it still needs line of sight and does not replace ranger pressure." };
                case "smokebomb": return new MartialAbility { Id = "smokebomb", Name = "Smoke Bomb", Short = "SMK", ClassKey = "rogue", RequiredLevel = 5, Range = 0, Targeted = false, Summary = "Vanish and fill adjacent open tiles with sight-blocking smoke.", Detail = "Smoke Bomb grants stealth and places brief smoke fields nearby. Smoke does not block movement or poison units, but direct spells and missile attacks cannot see through it." };
                case "hamstring": return new MartialAbility { Id = "hamstring", Name = "Hamstring", Short = "HAM", ClassKey = "rogue", RequiredLevel = 8, Range = 1, Targeted = true, Summary = "Melee cut that slows and bleeds.", Detail = "Hamstring is less explosive than Ambush, but it can pin a foe in place and set up Eviscerate." };
                case "eviscerate": return new MartialAbility { Id = "eviscerate", Name = "Eviscerate", Short = "EVS", ClassKey = "rogue", RequiredLevel = 12, Range = 1, Targeted = true, Summary = "Deep cut that causes bleeding.", Detail = "Eviscerate deals more damage to already bleeding targets and rewards a prepared melee finish." };
                case "shadowstep": return new MartialAbility { Id = "shadowstep", Name = "Shadowstep", Short = "SHD", ClassKey = "rogue", RequiredLevel = 16, Range = 4, Targeted = true, Summary = "Slip through shadow, land beside an enemy, and cut once.", Detail = "Shadowstep ignores intervening terrain but needs an open landing tile beside the target. It trades control for precise repositioning and breaks stealth after the strike." };
                case "aimedshot": return new MartialAbility { Id = "aimedshot", Name = "Aimed Shot", Short = "AIM", ClassKey = "ranger", RequiredLevel = 1, Range = 6, Targeted = true, Summary = "High-accuracy arrow with stronger damage on clear sight lines.", Detail = "Aimed Shot uses missile skill, needs line of sight, and is strongest against marked or hexed targets." };
                case "pinningshot": return new MartialAbility { Id = "pinningshot", Name = "Pinning Shot", Short = "PIN", ClassKey = "ranger", RequiredLevel = 1, Range = 6, Targeted = true, Summary = "Arrow strike that webs/pins the target briefly.", Detail = "Pinning Shot deals lighter damage but can hold a dangerous enemy in place for one or two turns." };
                case "volley": return new MartialAbility { Id = "volley", Name = "Volley", Short = "VOL", ClassKey = "ranger", RequiredLevel = 5, Range = 6, Targeted = true, Summary = "Arc a small rain of arrows over a target and nearby foes.", Detail = "Volley can arc over cover and hits the chosen enemy plus adjacent enemies for reduced physical damage." };
                case "scoutmark": return new MartialAbility { Id = "scoutmark", Name = "Scout Mark", Short = "MRK", ClassKey = "ranger", RequiredLevel = 3, Range = 7, Targeted = true, Summary = "Break guard and mark one enemy for 2 turns.", Detail = "Scout Mark cancels guarding, strips one turn of ward, and exposes the target so the whole party can punish it." };
                case "broadheadshot": return new MartialAbility { Id = "broadheadshot", Name = "Broadhead Shot", Short = "BRD", ClassKey = "ranger", RequiredLevel = 8, Range = 6, Targeted = true, Summary = "Arrow strike that causes bleeding.", Detail = "Broadhead Shot rewards clear sight lines and sets up physical pressure from warriors and rogues." };
                case "disruptingshot": return new MartialAbility { Id = "disruptingshot", Name = "Disrupting Shot", Short = "DIS", ClassKey = "ranger", RequiredLevel = 12, Range = 6, Targeted = true, Summary = "Interrupt a dangerous enemy.", Detail = "Disrupting Shot deals modest damage but can stun a target. It is especially useful against casters and bosses' support turns." };
                case "quickshot": return new MartialAbility { Id = "quickshot", Name = "Quick Shot", Short = "QSH", ClassKey = "ranger", RequiredLevel = 16, Range = 6, Targeted = true, Summary = "Loose two lighter arrows in rapid succession.", Detail = "Each arrow rolls to hit separately and deals reduced damage. Quick Shot pressures lightly armored targets but loses efficiency against heavy defense." };
                case "riftpounce": return new MartialAbility { Id = "riftpounce", Name = "Rift Pounce", Short = "RPT", ClassKey = "demon", RequiredLevel = 1, Range = 5, Targeted = true, Summary = "Tear through the rift, land beside an enemy, and strike.", Detail = "Rift Pounce ignores intervening terrain and units, but still needs one open tile beside the target. It trades Charge's stun for unrestricted rift travel and death damage." };
                case "abyssalwhirl": return new MartialAbility { Id = "abyssalwhirl", Name = "Abyssal Whirl", Short = "AWH", ClassKey = "demon", RequiredLevel = 1, Range = 1, Targeted = false, Summary = "Borrow Whirlwind's fury and rake every adjacent enemy.", Detail = "Abyssal Whirl is the demon form's brutal answer to being surrounded. It deals sequenced death damage to every adjacent enemy." };
                case "soulrend": return new MartialAbility { Id = "soulrend", Name = "Soul Rend", Short = "SRD", ClassKey = "demon", RequiredLevel = 1, Range = 1, Targeted = true, Summary = "Rip life from an adjacent enemy and feed on the wound.", Detail = "Soul Rend deals a heavy death strike and heals the transformed warlock for half the actual damage dealt." };
                case "dreadroar": return new MartialAbility { Id = "dreadroar", Name = "Dread Roar", Short = "DRR", ClassKey = "demon", RequiredLevel = 1, Range = 1, Targeted = false, Summary = "Break nearby guards and hex adjacent enemies.", Detail = "Dread Roar strips guarding from every adjacent enemy, then tests each target's mind resistance against a three-turn hex. It deals no damage." };
                default: return null;
            }
        }
    }
}
