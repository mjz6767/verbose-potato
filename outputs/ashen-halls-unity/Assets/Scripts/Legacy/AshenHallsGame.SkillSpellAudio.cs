namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private void BuildSkillSpellSoundClips()
        {
            // School release fallbacks are already registered by BuildSoundClips.
            // Every new skill master also has a bounded local fallback if an asset is unavailable.
            soundClips["shieldbashhit"] = MakeMaterialImpact("shieldbashhit", "shield");
            soundClips["cleavehit"] = MakeSkillSound("cleavehit", "whirlwind-impact", 620f, 92f, .29f, .31f);
            soundClips["sunderhit"] = MakeMaterialImpact("sunderhit", "plate");
            soundClips["hamstringhit"] = MakeSkillSound("hamstringhit", "ambush-impact", 720f, 120f, .20f, .29f);
            soundClips["kniferelease"] = MakeSound("kniferelease", 840f, 330f, .17f, .19f, "slash");
            soundClips["smokeprimer"] = MakeSound("smokeprimer", 460f, 220f, .13f, .12f, "rustle");
            soundClips["smokeburst"] = MakeSkillSound("smokeburst", "smoke", 146f, 62f, .46f, .24f);
            soundClips["bowdraw"] = MakeSound("bowdraw", 114f, 210f, .18f, .12f, "rustle");
            soundClips["twinshotrelease"] = MakeSkillSound("twinshotrelease", "volley", 280f, 620f, .25f, .21f);
            soundClips["twinshothit"] = MakeSkillSound("twinshothit", "arrow-rain", 660f, 180f, .32f, .28f);
            soundClips["broadheadhit"] = MakeSkillSound("broadheadhit", "ambush-impact", 470f, 72f, .30f, .30f);
            soundClips["staggerhit"] = MakeMaterialImpact("staggerhit", "mail");
            soundClips["shieldbrace"] = MakeSound("shieldbrace", 146f, 220f, .20f, .12f, "rustle");
        }
    }
}
