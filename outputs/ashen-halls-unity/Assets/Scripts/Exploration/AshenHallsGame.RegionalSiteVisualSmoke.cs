using System;
using System.Collections;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private const string RegionalSiteVisualSmokeArgument = "-ashen-site-smoke";

        private IEnumerator Start()
        {
            string[] args = Environment.GetCommandLineArgs();
            int siteOption = FindCommandLineOption(args, RegionalSiteVisualSmokeArgument);
            if (siteOption < 0) yield break;

            // Awake owns the normal visual-smoke Quick Start and begins a capture
            // coroutine with a two-second hold. Stage on the next frame so all
            // exploration state exists while still beating that capture window.
            yield return null;

            try
            {
                StageRegionalSiteVisualSmoke(args, siteOption);
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    VersionInfo.ProductName
                    + " regional-site visual smoke failed: "
                    + ex);
                Application.Quit(2);
            }
        }

        private void StageRegionalSiteVisualSmoke(string[] args, int siteOption)
        {
            if (FindCommandLineOption(args, "-ashen-explore-smoke") < 0)
            {
                throw new InvalidOperationException(
                    RegionalSiteVisualSmokeArgument
                    + " requires -ashen-explore-smoke so the authored world map is staged first.");
            }
            if (siteOption + 1 >= args.Length
                || string.IsNullOrWhiteSpace(args[siteOption + 1])
                || args[siteOption + 1].StartsWith("-", StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    RegionalSiteVisualSmokeArgument
                    + " requires one authored regional site id.");
            }
            if (FindCommandLineOption(args, RegionalSiteVisualSmokeArgument, siteOption + 1) >= 0)
            {
                throw new InvalidOperationException(
                    RegionalSiteVisualSmokeArgument
                    + " may be supplied only once per capture.");
            }
            if (state == null || state.Mode != GameMode.Explore || state.Map == null)
            {
                throw new InvalidOperationException(
                    "Regional-site visual smoke did not reach a live exploration map.");
            }

            string requestedSiteId = args[siteOption + 1].Trim();
            WorldMapSite requestedSite = default;
            int matchCount = 0;
            foreach (WorldMapSite site in WorldMapGenerationRules.RegionalSites(
                state.Map.Width,
                state.Map.Height,
                state.Map.StartX,
                state.Map.StartY))
            {
                if (!string.Equals(site.Id, requestedSiteId, StringComparison.OrdinalIgnoreCase)) continue;
                requestedSite = site;
                matchCount++;
            }
            if (matchCount != 1)
            {
                throw new InvalidOperationException(
                    $"Regional-site visual smoke expected one authored site '{requestedSiteId}', found {matchCount}.");
            }

            MapObject landmark = state.Map.FindObjectById(RegionalSiteObjectId(requestedSite));
            if (landmark == null
                || !TryRegionalSite(state.Map, landmark, out WorldMapSite landmarkSite)
                || !string.Equals(landmarkSite.Id, requestedSite.Id, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Regional-site visual smoke could not resolve the live landmark for '{requestedSite.Id}'.");
            }

            if (!TryPositionRegionalSiteVisualSmoke(requestedSite, landmark, out ExplorationInteraction interaction))
            {
                throw new InvalidOperationException(
                    $"Regional-site visual smoke found no walkable interaction approach for '{requestedSite.Id}'.");
            }

            if (!TryRegionalSiteAt(
                    state.Map,
                    state.PlayerX,
                    state.PlayerY,
                    out WorldMapSite stagedSite)
                || !string.Equals(stagedSite.Id, requestedSite.Id, StringComparison.Ordinal)
                || !interaction.HasTarget
                || !ReferenceEquals(interaction.Target, landmark))
            {
                throw new InvalidOperationException(
                    $"Regional-site visual smoke interaction validation failed for '{requestedSite.Id}'.");
            }
            if (!WorldSiteInteractionRules.TryGet(
                    requestedSite.Id,
                    out WorldSiteInteractionProfile profile))
            {
                throw new InvalidOperationException(
                    $"Regional-site visual smoke found no interaction profile for '{requestedSite.Id}'.");
            }

            lastExploreRegion = ExploreRegionName(state.PlayerX, state.PlayerY);
            bannerText = "";
            bannerUntil = 0f;
            InvalidateExplorationController();
            MarkUiDirty();
            ApplyVisualSmokeExploreView(args);

            bool rewardClaimed = WorldSiteInteractionRules.RewardClaimed(
                state.StoryFlags,
                state.Depth,
                requestedSite.Id);
            string expectedVerb = WorldSiteInteractionRules.ContextVerb(profile, rewardClaimed);
            ExplorationInteraction stagedInteraction = CurrentExploreInteraction();
            if (!stagedInteraction.HasTarget
                || !ReferenceEquals(stagedInteraction.Target, landmark)
                || !string.Equals(stagedInteraction.Verb, expectedVerb, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Regional-site visual smoke expected {expectedVerb} for '{requestedSite.Id}', "
                    + $"but staged '{stagedInteraction.Verb}'.");
            }

            Debug.Log(
                $"{VersionInfo.ProductName} regional-site visual smoke: "
                + $"site={requestedSite.Id}, center={requestedSite.X},{requestedSite.Y}, "
                + $"stand={state.PlayerX},{state.PlayerY}, view={(exploreWideView ? "Region Map" : "Local Map")}, "
                + $"details={(exploreHudCollapsed ? "closed" : "open")}, "
                + $"rewardClaimed={rewardClaimed}, verb={stagedInteraction.Verb}, "
                + $"service={profile.ServiceName}.");
        }

        private bool TryPositionRegionalSiteVisualSmoke(
            WorldMapSite site,
            MapObject landmark,
            out ExplorationInteraction interaction)
        {
            interaction = ExplorationInteraction.None;
            Point preferred = RegionalSiteApproach(state.Map, site);
            Point[] candidates =
            {
                preferred,
                new Point(site.X, site.Y),
                new Point(site.X, site.Y + 1),
                new Point(site.X - 1, site.Y),
                new Point(site.X + 1, site.Y),
                new Point(site.X, site.Y - 1)
            };

            for (int i = 0; i < candidates.Length; i++)
            {
                Point candidate = candidates[i];
                if (candidate == null || !CanStepExplore(candidate.X, candidate.Y)) continue;

                bool duplicate = false;
                for (int earlier = 0; earlier < i; earlier++)
                {
                    Point previous = candidates[earlier];
                    if (previous != null
                        && previous.X == candidate.X
                        && previous.Y == candidate.Y)
                    {
                        duplicate = true;
                        break;
                    }
                }
                if (duplicate) continue;

                state.PlayerX = candidate.X;
                state.PlayerY = candidate.Y;
                int facingX = Math.Sign(site.X - candidate.X);
                int facingY = Math.Sign(site.Y - candidate.Y);
                if (Math.Abs(facingX) + Math.Abs(facingY) != 1)
                {
                    facingX = 0;
                    facingY = -1;
                }
                exploreFacingX = facingX;
                exploreFacingY = facingY;
                InvalidateExplorationController();
                interaction = CurrentExploreInteraction();
                if (interaction.HasTarget && ReferenceEquals(interaction.Target, landmark)) return true;
            }

            interaction = ExplorationInteraction.None;
            return false;
        }

        private static int FindCommandLineOption(string[] args, string option, int startIndex = 0)
        {
            if (args == null || string.IsNullOrEmpty(option)) return -1;
            for (int i = Math.Max(0, startIndex); i < args.Length; i++)
            {
                if (string.Equals(args[i], option, StringComparison.OrdinalIgnoreCase)) return i;
            }
            return -1;
        }
    }
}
