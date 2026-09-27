using System.Collections.Generic;

namespace AshenHalls
{
    public static class SkillSpellAudioRules
    {
        // Authored masters complement, rather than replace, the existing impact bank.
        public static readonly IReadOnlyList<string> CueKeys = new[]
        {
            "releaseember", "releasefrost", "releaseshock", "releasenature",
            "releaseholy", "releasehex", "releasepact", "releaseveil",
            "shieldbashhit", "cleavehit", "sunderhit", "hamstringhit", "kniferelease",
            "smokeprimer", "smokeburst", "bowdraw", "twinshotrelease", "twinshothit",
            "broadheadhit", "staggerhit", "shieldbrace"
        };
    }
}
