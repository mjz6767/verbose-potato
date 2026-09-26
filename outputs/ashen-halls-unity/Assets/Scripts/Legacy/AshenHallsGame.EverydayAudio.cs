namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        // Registered before imports, so missing masters retain complete feedback.
        private void BuildEverydaySoundClips()
        {
            soundClips["equipblade"] = MakeServiceSound("equipblade", "weapon");
            soundClips["equipbow"] = MakeSound("equipbow", 196f, 110f, 0.38f, 0.23f, "rustle");
            soundClips["equipstaff"] = MakeSound("equipstaff", 196f, 392f, 0.48f, 0.24f, "chime");
            soundClips["equipcloth"] = MakeSound("equipcloth", 170f, 96f, 0.34f, 0.23f, "rustle");
            soundClips["equipmail"] = MakeServiceSound("equipmail", "armor");
            soundClips["equipplate"] = MakeServiceSound("equipplate", "armor");
            soundClips["chestopen"] = MakeSound("chestopen", 146f, 62f, 0.62f, 0.28f, "thud");
            soundClips["lootrare"] = MakeSound("lootrare", 294f, 880f, 0.64f, 0.24f, "chime");
            soundClips["zonediscover"] = MakeSound("zonediscover", 196f, 587f, 0.72f, 0.23f, "chime");
            soundClips["questcomplete"] = MakeSound("questcomplete", 294f, 784f, 0.82f, 0.26f, "chime");
            foreach (string key in GameAudioCueRules.VariedFootstepKeys)
            {
                // Variants share the base fallback until their optional masters load.
                soundClips[key + "__v1"] = soundClips[key];
                soundClips[key + "__v2"] = soundClips[key];
            }
        }
    }
}
