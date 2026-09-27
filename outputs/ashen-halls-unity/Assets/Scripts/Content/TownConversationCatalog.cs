namespace AshenHalls
{
    public enum TownStoryStage
    {
        Newcomers,
        CisternContract,
        CisternVictory,
        KoboldVictory,
        GloamVictory,
        GlassAndAshVictory,
        RedGateVictory
    }

    // Authored, local conversation: reading a line never changes story or service state.
    public static class TownConversationCatalog
    {
        public static TownStoryStage Stage(GameState state)
        {
            if (Has(state, StoryFlags.CrownroadMarshalDefeated)) return TownStoryStage.RedGateVictory;
            if (Has(state, StoryFlags.EmberglassGateKeyRecovered)) return TownStoryStage.GlassAndAshVictory;
            if (Has(state, StoryFlags.GloamWardenDefeated)) return TownStoryStage.GloamVictory;
            if (Has(state, StoryFlags.KoboldKingDefeated)) return TownStoryStage.KoboldVictory;
            if (Has(state, StoryFlags.SewerCisternDenCleared)
                || Has(state, StoryFlags.MidgaardRatPeltArmorMade)
                || Has(state, StoryFlags.SewerRewardClaimed)) return TownStoryStage.CisternVictory;
            if (Has(state, StoryFlags.SewerContractAccepted) || Has(state, StoryFlags.MidgaardRatQuestGiven))
                return TownStoryStage.CisternContract;
            return TownStoryStage.Newcomers;
        }

        public static string Greeting(NpcId speaker, GameState state)
        {
            TownStoryStage stage = Stage(state);
            int gold = state?.Gold ?? 0;
            switch (speaker)
            {
                case NpcId.Orren:
                    switch (stage)
                    {
                        case TownStoryStage.RedGateVictory: return "The tale from Red Gate got here before you. It grew a second dragon between the door and the bar. Sit down and tell me which bits to cross out.";
                        case TownStoryStage.GlassAndAshVictory: return "Glass country, was it? Leave the boots by the hearth; I don't want glitter in the floorboards. Glad to see the people who belong in them.";
                        case TownStoryStage.GloamVictory: return "They say the Warden is down. Good. I'd rather hear you complain about my beer than hear another song about the dead.";
                        case TownStoryStage.KoboldVictory: return "Pell came in grinning about the kobold king. Frightened the regulars. Your usual patch of hearth is free.";
                        case TownStoryStage.CisternVictory: return "Harl heard about the den. Wanted to buy you a drink, then remembered his tab. Sit by the fire; you've earned a dry chair.";
                        case TownStoryStage.CisternContract: return "So you've taken the cistern writ. Harl's over there, telling everyone he nearly had the brute. I can give you the shorter version.";
                        default: return "A clean table and a warm hearth. Start there. If you're thinking of taking work in this city, I've heard a few things you should hear first.";
                    }
                case NpcId.Kate:
                case NpcId.Lute:
                    bool lute = speaker == NpcId.Lute;
                    bool packed = Has(state, StoryFlags.MidgaardKateBundleBought) || Has(state, StoryFlags.MidgaardProvisionBundleBought);
                    string welcome = FoodWelcome(lute, stage);
                    if (packed)
                        return welcome + (lute
                            ? " Your first road order is accounted for. Packing advice is still free."
                            : " Your first road bundle's been packed. Stay a minute; the stove doesn't charge for company.");
                    return welcome + (lute
                        ? $" Three sealed provisions cost 10 gold. You've got {gold}."
                        : $" Four provisions cost 12 gold. You've got {gold}.");
                case NpcId.Borin:
                    if (Has(state, StoryFlags.MidgaardRatPeltArmorMade))
                        return "Back again. Good. I'd sooner see my work worn than hanging here. Keep that sewer-hide coat dry at the seams; tell me if a shoulder pulls.";
                    if (Has(state, StoryFlags.MidgaardBasicArmorBought))
                        return "How are the rings holding? The hauberk I made you is in your Inventory; compare the fit before choosing a wearer. I've a moment if you need advice.";
                    return (stage >= TownStoryStage.CisternVictory ? "Heard you came back from the den. Glad of it. " : "Mind the filings on that stool. ")
                        + $"A plain iron hauberk costs 28 gold. I'll put it in your Pack for you to choose the wearer. You've got {gold}.";
                case NpcId.Tessa:
                    if (Has(state, StoryFlags.MidgaardBasicWeaponBought))
                        return "Back with all your fingers? I'll count that as a recommendation. You have the weapon I fitted; if you're weighing a new find against it, I can talk you through the balance.";
                    return (stage >= TownStoryStage.CisternVictory ? "Word is you've given my trade a fair trial below the city. " : "Try the grip before you admire the blade. ")
                        + $"A weapon fitted to your lead fighter costs 32 gold. You've got {gold}.";
                case NpcId.Maud:
                    if (!HasWeapon(state)) return "Empty hands? Then you can keep me company. Bring a weapon or a focus when you want a rune; questions need neither.";
                    string opening = Has(state, StoryFlags.MidgaardWeaponEnchanted)
                        ? "Back at my anvil. How did the mark take? "
                        : "Set the weapon down gently. The anvil and I both have good hearing. ";
                    return opening + $"A temper costs {WeaponEnchantmentRules.TemporaryCost} gold for {WeaponEnchantmentRules.TemporaryVictories} victories; a permanent binding costs {WeaponEnchantmentRules.PermanentCost} gold. You've got {gold}.";
                default: return "";
            }
        }

        public static string OrrenCisternNews(GameState state)
        {
            if (Stage(state) >= TownStoryStage.CisternVictory)
                return "Harl asked whether the brute was really as big as he said. I told him you had other things to worry about. He's laughing about it now, which is an improvement. Clearing that den hasn't brought everyone home, but it has given the living something else to talk about.";
            return "Harl the mason came back with half a shield and no eyebrows. A brute kept his crew busy while a plague caster spoiled every safe patch of floor. His advice was 'Move the caster. Then mind the big one.' He says it with rather more swearing.";
        }

        public static string OrrenRoadNews(GameState state)
        {
            switch (Stage(state))
            {
                case TownStoryStage.RedGateVictory:
                    if (Has(state, StoryFlags.RedGateDebriefed))
                        return "Everyone wants to know what lies past the Meteor Crown threshold. I tell them there's plenty worth coming home to on this side. You've given Yara her report. You can give the rest of us a quiet evening, if you'd like.";
                    return Has(state, StoryFlags.MeteorCrownThresholdSurveyed)
                        ? "Everyone wants to know what lies past the Meteor Crown threshold. I tell them defeating the Marshal was enough work for one day. Take your survey back to Scout Yara before you take another road. She'll want the version without the extra dragon."
                        : "People are already singing about the Marshal. They leave the survey out; hard to rhyme. If you haven't charted the sealed descent at Salt Cistern Gate, finish that before reporting to Scout Yara. Look, mark it, come home. That's a fine ending to a song.";
                case TownStoryStage.GlassAndAshVictory:
                    return Has(state, StoryFlags.GlassAndAshDebriefed)
                        ? "The room is still divided about glass country. Half want to see it; half say glass belongs in a window. If Red Gate is next, plan the assault with Scout Yara and pack carefully. I'll keep the more excitable half here a while."
                        : "Your glass-country story has the room divided. Half want to see it; half say glass belongs in a window. Take the recovered key and your report to Scout Yara before chasing Red Gate rumors. I'll keep the more excitable half here a while.";
                case TownStoryStage.GloamVictory:
                    return "The Bone Road stories have changed since you brought down the Warden. People are asking what comes after it. If you've found word of Red Gate or the glass frontier, take it to Scout Yara. Mine is a good hearth for rumors and a poor place to plan an expedition.";
                case TownStoryStage.KoboldVictory:
                    return "The kobold king's fall is the talk of every cart that comes through. That doesn't make the road tame. Now the drivers are whispering about Bone Road and the Gloam Crypt. Show Scout Yara what you brought from Varkh's hall before you choose tomorrow's walk.";
                default:
                    return "Westbound travelers bring shrine moss and quarry dust. The eastern carts bring broken spokes and stories of kobold fires. Brann will have the fresher report. Mine usually arrives thirsty.";
            }
        }

        public static string PersonalStory(NpcId speaker)
        {
            switch (speaker)
            {
                case NpcId.Orren:
                    return "I used to drive a provision cart. Knew every rut between here and the east road; never knew who'd be at the next fire. When this place came up, I bought it. Now I get to be the person who's there. The accounts are worse, but I sleep better.";
                case NpcId.Borin:
                    return "My mother shod cart horses. She'd make me find the nail that would work loose before she'd let me touch the hammer. I wanted to forge crowns, of course. Turns out I like making things people can come home in. Less polishing, too.";
                case NpcId.Tessa:
                    return "I worked a caravan guard's rack. Everyone wanted the longest blade until they had to draw it between two wagons. After a season of bruised knuckles, I started fitting the weapon to the hand. People still ask for the longest one. I let them try the doorway first.";
                case NpcId.Maud:
                    return "My teacher kept a brass lamp that spoke once each winter. We polished it for years, waiting for some grand secret. It finally told us the roof leaked. I learned to listen to small, useful magic after that. Also to mend a roof before winter.";
                default: return "";
            }
        }

        public static string FoodForTheCisterns(bool provisionStall)
        {
            return provisionStall
                ? "My first cartload spoiled in a wet cellar. Kate bought what we could save and showed me how to wax the wrapping. I still check every knot twice. Hard bread, smoked meat, sealed parcels: keep them above the damp, and eat before the Cistern Den."
                : "My father repaired the old drains. He'd come home with his boots in one hand and a soggy lunch in the other. I learned to wrap a meal properly before I could lift his boots. Bread, smoked meat, apples, well wrapped. Eat before the last chamber, love. No prizes for carrying supper home untouched.";
        }

        private static string FoodWelcome(bool lute, TownStoryStage stage)
        {
            if (stage >= TownStoryStage.GlassAndAshVictory)
                return lute ? "Glass dust in the straps. Let's keep that out of your food." : "You've brought half a strange country in on your boots. Wipe them and come in, love.";
            if (stage >= TownStoryStage.KoboldVictory)
                return lute ? "The eastbound carters know your faces now. Mine too, for sending you out fed." : "Pell told me the road news. Lovely. Now sit down before you find another king to quarrel with.";
            if (stage >= TownStoryStage.CisternVictory)
                return lute ? "Back from the den. Good. Dry your pack before you put fresh food in it." : "There you are. I heard about the den. Come near the stove; you can tell me the parts fit for a kitchen.";
            return lute ? "Mind the twine. I was just tying the last parcel." : "Sit down, love. You can warm your hands while we talk.";
        }

        private static bool Has(GameState state, string flag)
        {
            return state?.StoryFlags != null && state.StoryFlags.Contains(flag);
        }

        private static bool HasWeapon(GameState state)
        {
            if (state?.Party == null) return false;
            foreach (PartyMember member in state.Party)
                if (member != null && !string.IsNullOrWhiteSpace(member.WeaponName)) return true;
            return false;
        }
    }
}
