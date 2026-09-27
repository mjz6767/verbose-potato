using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private bool HasMeteorCrownStoryProgress()
        {
            return ContentSetCatalog.IsSewerSlice(activeContentSet)
                && MeteorCrownQuestRules.Accepted(state?.StoryFlags);
        }

        private string MeteorCrownObjectiveForProgress()
        {
            if (MeteorCrownQuestRules.Complete(state?.StoryFlags))
                return "Epilogue: The Last Dawn. Vhal Rakh is defeated, the ritual heart is broken, and the Old Road belongs to the living again. Midgaard's lamps will burn through the morning.";
            int depth = state?.Depth ?? 1;
            if (depth < 5) return "Chapter VI: The Last Dawn. Follow the known roads through the Red Gate and return to the surveyed descent at Salt Cistern Gate. Recall preserves the assault's progress.";
            if (depth == 5) return "Chapter VI: The Last Dawn. Open the surveyed descent at Salt Cistern Gate. Yara's return line is ready.";
            if (!MeteorCrownQuestRules.SentriesDefeated(state?.StoryFlags))
                return "Chapter VI: The Last Dawn. Break the crownwatch at the Red Gate Seal to secure the return road. The glass conduit leads onward from there.";
            if (!MeteorCrownQuestRules.HeartBroken(state?.StoryFlags))
                return "Chapter VI: The Last Dawn. Follow the crownwatch's conduit to the Glass Lore Library. Break the ritual heart feeding Vhal Rakh's crown.";
            return "Chapter VI: The Last Dawn. The heart is broken. Reach Salt Cistern Gate, confront Vhal Rakh, and bring the company home.";
        }

        private void RepairMeteorCrownStoryObjective(bool force = false)
        {
            if (state == null || !HasMeteorCrownStoryProgress()) return;
            state.ActiveStory = MeteorCrownObjectiveForProgress();
        }

        private string MeteorCrownCurrentSiteId() => MeteorCrownQuestRules.CurrentSite(state?.StoryFlags);

        private string MeteorCrownGuidanceTargetId()
        {
            if (!HasMeteorCrownStoryProgress() || MeteorCrownQuestRules.Complete(state.StoryFlags)) return "";
            switch (state.Depth)
            {
                case 1: return OldRoadDescentId;
                case 2: return BoneRoadPassageId;
                case 3: return GlassAndAshPassageId;
                case 4: return "regional-site:" + RedGateSealSiteId;
                case 5: return "regional-site:" + SaltCisternGateSiteId;
                case 6: return "regional-site:" + MeteorCrownCurrentSiteId();
                default: return "";
            }
        }

        private string MeteorCrownSiteDetail(string siteId)
        {
            if (!HasMeteorCrownStoryProgress()) return "";
            if (state.Depth == 5 && siteId == SaltCisternGateSiteId)
                return "The surveyed descent opens onto Meteor Crown country. Yara's return line is ready; enter when the company is supplied.";
            if (state.Depth != 6) return "";
            if (siteId == MeteorCrownQuestRules.SentriesSite)
                return MeteorCrownQuestRules.SentriesDefeated(state.StoryFlags)
                    ? "The crownwatch is gone. The return road stays open; follow its glass conduit to the Glass Lore Library."
                    : "The crownwatch holds the return road. Break its line before entering the mirrored library.";
            if (siteId == MeteorCrownQuestRules.HeartSite)
                return MeteorCrownQuestRules.HeartBroken(state.StoryFlags)
                    ? "The mirrors are dark and the ritual heart is broken. Vhal Rakh waits at Salt Cistern Gate."
                    : "The stolen ritual heart beats beneath the library. Secure the crownwatch conduit first, then destroy the heart and its keepers.";
            if (siteId == MeteorCrownQuestRules.SovereignSite)
                return MeteorCrownQuestRules.Complete(state.StoryFlags)
                    ? "Vhal Rakh's crown is cold. The Old Road belongs to the living."
                    : "Vhal Rakh waits beyond the last threshold. Break the ritual heart at the Glass Lore Library before confronting him.";
            return "";
        }

        private bool TryShowMeteorCrownConversation()
        {
            if (!ContentSetCatalog.IsSewerSlice(activeContentSet) || !MeteorCrownQuestRules.CanAccept(state?.StoryFlags)) return false;
            List<DialogueChoiceView> choices = new List<DialogueChoiceView>();
            string body;
            if (MeteorCrownQuestRules.Complete(state.StoryFlags))
            {
                body = "I kept a lamp beside the chart. Couldn't do much else. Then the red marks went out, one after another. Sit down, will you? I would rather hear your voices than read another report.";
                choices.Add(MakeDialogueChoice("home", "What happens to the road now?", "A quieter road, and work for those who come after."));
            }
            else if (MeteorCrownQuestRules.Accepted(state.StoryFlags))
            {
                body = MeteorCrownQuestRules.HeartBroken(state.StoryFlags)
                    ? "The glass on my chart has gone cold. You broke the heart. That leaves Vhal Rakh himself. Eat something while I check your return marks. He has waited this long; he can wait through supper."
                    : "Your return marks are holding. If you need a meal or fresh bandages, take them. I am not counting how often you come back; I am counting who comes back with you.";
                choices.Add(MakeDialogueChoice("route", "Walk us through the final road again.", "Crownwatch, heart, then Vhal Rakh. Camp or recall anytime."));
            }
            else
            {
                body = "Here is the final road. Your survey gave Brann somewhere to put his watch, and Sera has marked the return line. The descent stays sealed until you choose to go. I have been rehearsing a grand speech. All I can think of is: please come back.";
                choices.Add(MakePrimaryDialogueChoice("plan", "Show us the final assault plan.", "Review Chapter VI before accepting. You may still prepare or decline."));
            }
            choices.Add(MakeDialogueChoice("afraid", "Are you afraid?", "Ask Yara what this last road means to her."));
            choices.Add(MakeDialogueChoice("leave", "We'll speak again.", "Leave the conversation."));
            ShowDialogueChoices(MeteorCrownQuestRules.Title, "Yara", body, ObjectType.OldRoadScout, moss, choices.ToArray(), ResolveMeteorCrownConversation);
            return true;
        }

        private void ResolveMeteorCrownConversation(string choice)
        {
            if (choice == "leave") { CloseDialogue(); return; }
            if (choice == "plan") { ShowMeteorCrownAssaultReview(); return; }
            string body = choice == "afraid"
                ? "Yes. I know where every company went missing on this chart. I used to think knowing more would make it easier. It just means I know whose names to say. You have brought people back. I am trying to leave room for that, too."
                : choice == "home"
                    ? "First, people mend the bridges. Then someone complains about the toll. After that we will know we have won. I might walk to the shrine without counting the tracks ahead of me."
                    : "Enter beneath Salt Cistern Gate. Clear the crownwatch at the Red Gate Seal, follow its conduit to the Glass Lore Library, and break the heart beneath it. Then take the last road to Vhal Rakh at Salt Cistern Gate. Keep cold and light ready for him. If the company is hurt, camp or recall; every cleared position stays cleared.";
            ShowDialogueResponse(MeteorCrownQuestRules.Title, "Yara", body, ObjectType.OldRoadScout, moss, ShowYaraConversation);
        }

        private void ShowMeteorCrownAssaultReview()
        {
            if (!MeteorCrownQuestRules.CanAccept(state?.StoryFlags) || MeteorCrownQuestRules.Accepted(state.StoryFlags)) { ShowYaraConversation(); return; }
            ShowDialogueChoices(MeteorCrownQuestRules.Title, "Yara",
                "The final descent opens at Salt Cistern Gate. First clear the crownwatch at the Red Gate Seal. Their glass conduit leads to the library and the ritual heart. Break that before facing Vhal Rakh at the last Salt Cistern threshold. Three positions, with room to rest between them. This ends his hold on the road. It need not end your company.",
                ObjectType.OldRoadScout, blood, new[]
                {
                    MakePrimaryDialogueChoice("accept_meteor_crown", "We'll take the final road.", "Accept Chapter VI. The return route, camping, and recall stay available."),
                    MakeDialogueChoice("prepare", "We need to prepare first.", "Leave the descent sealed; return whenever ready.")
                }, ResolveMeteorCrownAssaultReview);
        }

        private void ResolveMeteorCrownAssaultReview(string choice)
        {
            if (state == null || state.Mode != GameMode.Explore || !ContentSetCatalog.IsSewerSlice(activeContentSet)) return;
            if (choice != "accept_meteor_crown" || !MeteorCrownQuestRules.TryAccept(state.StoryFlags)) { ShowYaraConversation(); return; }
            state.StoryChapter = Mathf.Max(state.StoryChapter, 6);
            state.ActiveStory = MeteorCrownObjectiveForProgress();
            DiscoverZoneHint(6, "red-gate");
            DiscoverZoneHint(6, "glass-warrens");
            DiscoverZoneHint(6, "salt-cisterns");
            EnsureOldRoadDescentMarker();
            PushLog("Yara opens the final assault chart. The company will secure the crownwatch road, break the ritual heart, and face Vhal Rakh. Recall remains available.", Tone.Good);
            AutosaveCheckpoint("Meteor Crown assault accepted");
            ShowDialogueResponse(MeteorCrownQuestRules.Title, "Yara", "Then I will keep the lamp lit. There is a cache at the descent: enough for a first camp, if the road takes more out of you than you expect. Come back and tell me my grand speech was terrible.", ObjectType.OldRoadScout, moss, ShowYaraConversation);
            ShowBanner("Chapter VI: The Last Dawn");
            PlaySfx("wayfind", 0.88f);
        }

        private bool TryResolveMeteorCrownStorySite(WorldMapSite site)
        {
            if (state == null || !ContentSetCatalog.IsSewerSlice(activeContentSet) || state.Mode != GameMode.Explore) return false;
            if (state.Depth == 5 && site.Id == SaltCisternGateSiteId && MeteorCrownQuestRules.CanAccept(state.StoryFlags))
            {
                if (!ContentSetCatalog.AllowMeteorCrownChapter(activeContentSet, state.StoryFlags)) return false;
                EnterMeteorCrownChapter();
                return true;
            }
            if (state.Depth != 6 || !ContentSetCatalog.AllowMeteorCrownChapter(activeContentSet, state.StoryFlags)) return false;
            string style = site.Id == MeteorCrownQuestRules.SentriesSite ? MeteorCrownQuestRules.SentriesStyle
                : site.Id == MeteorCrownQuestRules.HeartSite ? MeteorCrownQuestRules.HeartStyle
                : site.Id == MeteorCrownQuestRules.SovereignSite ? MeteorCrownQuestRules.SovereignStyle : "";
            if (style.Length == 0) return false;
            if (!MeteorCrownQuestRules.CanFight(state.StoryFlags, style))
            {
                PushLog(MeteorCrownSiteDetail(site.Id), Tone.Normal);
                ShowBanner(MeteorCrownQuestRules.CurrentSite(state.StoryFlags) == MeteorCrownQuestRules.SentriesSite ? "Secure the Return Road" : "The Last Dawn");
                return true;
            }
            state.ActiveStory = MeteorCrownObjectiveForProgress();
            if (style == MeteorCrownQuestRules.SovereignStyle)
            {
                ShowDialogueChoices("The Unfed Crown", "Vhal Rakh",
                    "I held that road while your little city slept. Broke the dead to my service. Made a kingdom from what your kings discarded. And now you come with a handful of tired names and a promise to go home.",
                    ObjectType.Obelisk, blood, new[]
                    {
                        MakePrimaryDialogueChoice("stand", "Those names are why we came.", "Confront Vhal Rakh and end the Meteor Crown."),
                        MakeDialogueChoice("rest", "We will return when everyone is ready.", "Step back. Camp or recall before the final battle.")
                    }, ResolveMeteorCrownConfrontation);
                return true;
            }
            StartCombat(style == MeteorCrownQuestRules.SentriesStyle ? EncounterId.MeteorCrownSentries : EncounterId.MeteorCrownHeart);
            return true;
        }

        private void ResolveMeteorCrownConfrontation(string choice)
        {
            CloseDialogue();
            if (choice != "stand" || state?.Depth != 6 || state.Mode != GameMode.Explore
                || !MeteorCrownQuestRules.CanFight(state.StoryFlags, MeteorCrownQuestRules.SovereignStyle)) return;
            PushLog("Vhal Rakh looks toward the silent glass conduit. For the first time, nothing answers him.", Tone.Warn);
            StartCombat(EncounterId.MeteorCrownSovereign);
        }

        private void EnterMeteorCrownChapter()
        {
            if (state?.Depth != 5 || state.Mode != GameMode.Explore || !ContentSetCatalog.AllowMeteorCrownChapter(activeContentSet, state.StoryFlags)) return;
            bool firstArrival = !HasStoryFlag(StoryFlags.MeteorCrownEntered);
            SetStoryFlag(StoryFlags.MeteorCrownEntered);
            state.Depth = 6;
            state.StoryChapter = Mathf.Max(state.StoryChapter, 6);
            state.ActiveRouteWaypointKey = "";
            state.Combat = null;
            InvalidateCombatController();
            state.ActiveStory = MeteorCrownObjectiveForProgress();
            state.Map = GenerateMap(state.Depth, state.Seed);
            InvalidateExplorationController();
            EnsureWorldLandmarks();
            PlacePlayerAtExplorationStart();
            lastExploreRegion = ExploreRegionName(state.PlayerX, state.PlayerY);
            DiscoverCurrentZone(true);
            if (firstArrival)
            {
                state.Supplies += 2;
                PushLog("Yara's cache waits beneath the last stair: supplies +2. The return marks burn steadily behind the company.", Tone.Good);
            }
            PushLog(state.ActiveStory, Tone.Good);
            ShowBanner("Chapter VI: The Last Dawn");
            PlaySfx("encounter", 0.86f);
            AutosaveCheckpoint(firstArrival ? "Meteor Crown entered" : "Meteor Crown revisited");
        }

        private void ApplyMeteorCrownStoryVictory(string style)
        {
            if (state?.Depth != 6 || !HasMeteorCrownStoryProgress() || style == MeteorCrownQuestRules.SovereignStyle
                || !MeteorCrownQuestRules.TryRecordVictory(state.StoryFlags, style)) return;
            state.Elixirs++;
            if (style == MeteorCrownQuestRules.HeartStyle)
            {
                state.Supplies += 2;
                PushLog("The stolen heart cracks. Its mirrors darken all the way to Vhal Rakh's throne. The keepers' cache holds two supplies and an elixir. Rest before taking the last road.", Tone.Good);
                ShowBanner("The Ritual Heart Is Broken");
            }
            else
            {
                PushLog("The crownwatch falls. An unbroken glass conduit leads to the Glass Lore Library, and an elixir is recovered from its field chest. The return road is secure.", Tone.Good);
            }
            state.ActiveStory = MeteorCrownObjectiveForProgress();
        }

        private bool IsMeteorCrownNonFinalCombat()
        {
            string style = state?.Combat?.EncounterStyle;
            return string.Equals(style, MeteorCrownQuestRules.SentriesStyle, StringComparison.OrdinalIgnoreCase)
                || string.Equals(style, MeteorCrownQuestRules.HeartStyle, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsAuthoredMeteorCrownFinalCombat()
        {
            return state?.Depth == 6 && HasMeteorCrownStoryProgress()
                && string.Equals(state.Combat?.EncounterStyle, MeteorCrownQuestRules.SovereignStyle, StringComparison.OrdinalIgnoreCase)
                && MeteorCrownQuestRules.CanFight(state.StoryFlags, MeteorCrownQuestRules.SovereignStyle)
                && state.Combat.Units.Any(unit => unit.Side == UnitSide.Enemy && unit.Role == "meteorlich");
        }

        private void FinishAuthoredMeteorCrownVictory()
        {
            if (state?.Depth != 6 || !HasMeteorCrownStoryProgress()
                || !MeteorCrownQuestRules.TryRecordVictory(state.StoryFlags, MeteorCrownQuestRules.SovereignStyle)) return;
            state.StoryChapter = 6;
            state.ActiveStory = MeteorCrownObjectiveForProgress();
            PushLog("By morning, Brann has opened the east gate. Sera leaves the temple lamps burning for those who did not come home. Yara sits beside a chart with no red marks left to count.", Tone.Good);
            if (HasStoryFlag(StoryFlags.RemembranceDelivered))
                PushLog("Edda's companions have their names back. Their names remain when the victory songs run out of verses.", Tone.Good);
            PushLog("Orren keeps the company's places by the hearth. For once, no one asks how soon you must leave.", Tone.Good);
        }

        private void AddMeteorCrownJournalRows(List<ArmoryRowView> rows)
        {
            if (rows == null || !ContentSetCatalog.IsSewerSlice(activeContentSet) || !MeteorCrownQuestRules.CanAccept(state?.StoryFlags)) return;
            string[] labels = { "Yara's Last Chart", "Crownwatch Return Road", "The Stolen Heart", "Vhal Rakh" };
            string[] hints =
            {
                "Debrief the Red Gate survey with Yara, then review and accept the final assault. The descent opens beneath Salt Cistern Gate.",
                "At depth VI, break the crownwatch at the Red Gate Seal. Its conduit names the path to the Glass Lore Library.",
                "Break the ritual heart and its keepers beneath the Glass Lore Library. Their cache provides two supplies and an elixir; rest before the finale.",
                "Reach Salt Cistern Gate after breaking the heart. Confront Vhal Rakh when ready. His fall ends the campaign."
            };
            bool[] done = { MeteorCrownQuestRules.Accepted(state.StoryFlags), MeteorCrownQuestRules.SentriesDefeated(state.StoryFlags), MeteorCrownQuestRules.HeartBroken(state.StoryFlags), MeteorCrownQuestRules.Complete(state.StoryFlags) };
            int active = Array.FindIndex(done, value => !value);
            for (int i = 0; i < labels.Length; i++) rows.Add(new ArmoryRowView
            {
                Key = -1, Title = "The Last Dawn - " + labels[i], Subtitle = done[i] ? "done" : i == active ? "current" : "pending",
                Detail = hints[i], AccentHex = ColorHtml(done[i] ? teal : i == active ? gold : line),
                Badge = done[i] ? "DONE" : i == active ? "NEXT" : "", BadgeAccentHex = ColorHtml(done[i] ? teal : i == active ? gold : line)
            });
        }
    }
}
