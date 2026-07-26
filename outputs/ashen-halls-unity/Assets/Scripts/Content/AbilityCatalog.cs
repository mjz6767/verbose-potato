using System.Collections.Generic;
using System.Linq;

namespace AshenHalls
{
    public static class AbilityCatalog
    {
        private static readonly Dictionary<string, string[]> idsByClass = new Dictionary<string, string[]>
        {
            ["warrior"] = new[] { "charge", "execute", "shieldbash", "rally", "cleave", "whirlwind" },
            ["rogue"] = new[] { "stealth", "ambush", "throwknife", "smokebomb", "hamstring", "eviscerate" },
            ["ranger"] = new[] { "aimedshot", "pinningshot", "volley", "scoutmark", "broadheadshot", "disruptingshot" }
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
                case "execute": return new MartialAbility { Id = "execute", Name = "Execute", Short = "EXE", ClassKey = "warrior", RequiredLevel = 1, Range = 1, Targeted = true, Summary = "Heavy finishing blow against badly wounded enemies.", Detail = "Execute is only valid when the enemy is at 35% HP or lower. Guard still reduces final damage." };
                case "shieldbash": return new MartialAbility { Id = "shieldbash", Name = "Shield Bash", Short = "BSH", ClassKey = "warrior", RequiredLevel = 2, Range = 1, Targeted = true, Summary = "Shove an adjacent enemy; blocked foes suffer a stunning collision.", Detail = "Shield Bash pushes the target one tile away when space is open. A wall, obstacle, unit, or board edge turns the shove into extra collision damage and a guaranteed stun." };
                case "rally": return new MartialAbility { Id = "rally", Name = "Rally", Short = "RLY", ClassKey = "warrior", RequiredLevel = 2, Range = 0, Targeted = false, Summary = "Brace the warrior and ward adjacent allies.", Detail = "Rally spends the action, gives the warrior a stronger ward, and gives adjacent allies a brief ward." };
                case "cleave": return new MartialAbility { Id = "cleave", Name = "Cleave", Short = "CLV", ClassKey = "warrior", RequiredLevel = 3, Range = 1, Targeted = true, Summary = "Heavy cut through one foe into another.", Detail = "Cleave hits the chosen adjacent enemy and clips one more adjacent enemy if the line is crowded." };
                case "whirlwind": return new MartialAbility { Id = "whirlwind", Name = "Whirlwind", Short = "WW", ClassKey = "warrior", RequiredLevel = 3, Range = 1, Targeted = false, Summary = "Strike every adjacent enemy.", Detail = "Whirlwind unlocks at level 3 and hits all enemies touching the warrior." };
                case "stealth": return new MartialAbility { Id = "stealth", Name = "Stealth", Short = "STL", ClassKey = "rogue", RequiredLevel = 1, Range = 0, Targeted = false, Summary = "Slip into shadow and become less likely to be targeted.", Detail = "Stealth lasts into the rogue's next turns and empowers Ambush." };
                case "ambush": return new MartialAbility { Id = "ambush", Name = "Ambush", Short = "AMB", ClassKey = "rogue", RequiredLevel = 1, Range = 1, Targeted = true, Summary = "High-accuracy strike; stronger from stealth.", Detail = "Ambush breaks stealth. From stealth it deals extra damage and can stun the target." };
                case "throwknife": return new MartialAbility { Id = "throwknife", Name = "Throw Knife", Short = "THR", ClassKey = "rogue", RequiredLevel = 2, Range = 3, Targeted = true, Summary = "Short ranged cut that can bleed.", Detail = "Throw Knife gives rogues a modest ranged option, but it still needs line of sight and does not replace ranger pressure." };
                case "smokebomb": return new MartialAbility { Id = "smokebomb", Name = "Smoke Bomb", Short = "SMK", ClassKey = "rogue", RequiredLevel = 2, Range = 0, Targeted = false, Summary = "Vanish and fill adjacent open tiles with sight-blocking smoke.", Detail = "Smoke Bomb grants stealth and places brief smoke fields nearby. Smoke does not block movement or poison units, but direct spells and missile attacks cannot see through it." };
                case "hamstring": return new MartialAbility { Id = "hamstring", Name = "Hamstring", Short = "HAM", ClassKey = "rogue", RequiredLevel = 3, Range = 1, Targeted = true, Summary = "Melee cut that slows and bleeds.", Detail = "Hamstring is less explosive than Ambush, but it can pin a foe in place and set up Eviscerate." };
                case "eviscerate": return new MartialAbility { Id = "eviscerate", Name = "Eviscerate", Short = "EVS", ClassKey = "rogue", RequiredLevel = 2, Range = 1, Targeted = true, Summary = "Deep cut that causes bleeding.", Detail = "Eviscerate unlocks at level 2 and deals more damage to already bleeding targets." };
                case "aimedshot": return new MartialAbility { Id = "aimedshot", Name = "Aimed Shot", Short = "AIM", ClassKey = "ranger", RequiredLevel = 1, Range = 6, Targeted = true, Summary = "High-accuracy arrow with stronger damage on clear sight lines.", Detail = "Aimed Shot uses missile skill, needs line of sight, and is strongest against marked or hexed targets." };
                case "pinningshot": return new MartialAbility { Id = "pinningshot", Name = "Pinning Shot", Short = "PIN", ClassKey = "ranger", RequiredLevel = 1, Range = 6, Targeted = true, Summary = "Arrow strike that webs/pins the target briefly.", Detail = "Pinning Shot deals lighter damage but can hold a dangerous enemy in place for one or two turns." };
                case "volley": return new MartialAbility { Id = "volley", Name = "Volley", Short = "VOL", ClassKey = "ranger", RequiredLevel = 2, Range = 6, Targeted = true, Summary = "Arc a small rain of arrows over a target and nearby foes.", Detail = "Volley can arc over cover and hits the chosen enemy plus adjacent enemies for reduced physical damage." };
                case "scoutmark": return new MartialAbility { Id = "scoutmark", Name = "Scout Mark", Short = "MRK", ClassKey = "ranger", RequiredLevel = 2, Range = 7, Targeted = true, Summary = "Break guard and mark one enemy for 2 turns.", Detail = "Scout Mark cancels guarding, strips one turn of ward, and exposes the target so the whole party can punish it." };
                case "broadheadshot": return new MartialAbility { Id = "broadheadshot", Name = "Broadhead Shot", Short = "BRD", ClassKey = "ranger", RequiredLevel = 3, Range = 6, Targeted = true, Summary = "Arrow strike that causes bleeding.", Detail = "Broadhead Shot rewards clear sight lines and sets up physical pressure from warriors and rogues." };
                case "disruptingshot": return new MartialAbility { Id = "disruptingshot", Name = "Disrupting Shot", Short = "DIS", ClassKey = "ranger", RequiredLevel = 4, Range = 6, Targeted = true, Summary = "Interrupt a dangerous enemy.", Detail = "Disrupting Shot deals modest damage but can stun a target. It is especially useful against casters and bosses' support turns." };
                default: return null;
            }
        }
    }
}
