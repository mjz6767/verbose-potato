using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    public enum BetaLabKind
    {
        Caster,
        Martial
    }

    public enum BetaLabToolbarActionId
    {
        Refill,
        Mage,
        Warlock,
        Craft,
        Stage,
        Hazards,
        Spawn,
        Reset,
        VisualTour,
        Promote,
        Wound,
        Cluster
    }

    public enum BetaLabToolbarNavigation
    {
        Left,
        Right,
        Up,
        Down,
        Previous,
        Next,
        First,
        Last
    }

    public enum BetaLabBuildFlavor
    {
        Retail,
        BetaDevelopment
    }

    public readonly struct BetaLabToolbarActionDefinition
    {
        public readonly BetaLabToolbarActionId Id;
        public readonly string Label;
        public readonly string CompactLabel;
        public readonly string Description;
        public readonly bool VisualOnly;

        public BetaLabToolbarActionDefinition(
            BetaLabToolbarActionId id,
            string label,
            string compactLabel,
            string description,
            bool visualOnly = false)
        {
            Id = id;
            Label = label ?? "";
            CompactLabel = string.IsNullOrWhiteSpace(compactLabel) ? Label : compactLabel;
            Description = description ?? "";
            VisualOnly = visualOnly;
        }

        public string LabelForWidth(float width)
        {
            return width < BetaLabToolbarRules.CompactLabelWidth ? CompactLabel : Label;
        }
    }

    public readonly struct BetaLabToolbarGeometry
    {
        public readonly Rect Bounds;
        public readonly Rect Header;
        public readonly Rect Status;
        public readonly IReadOnlyList<Rect> ActionRects;
        public readonly int FirstRowCount;

        public BetaLabToolbarGeometry(
            Rect bounds,
            Rect header,
            Rect status,
            Rect[] actionRects,
            int firstRowCount)
        {
            Bounds = bounds;
            Header = header;
            Status = status;
            ActionRects = Array.AsReadOnly(actionRects ?? Array.Empty<Rect>());
            FirstRowCount = Math.Max(0, firstRowCount);
        }

        public int ActionCount => ActionRects.Count;
        public int RowCount => ActionCount == 0 ? 0 : ActionCount <= FirstRowCount ? 1 : 2;

        public bool Fits()
        {
            if (Bounds.width < BetaLabToolbarRules.MinimumToolbarWidth
                || Bounds.height < BetaLabToolbarRules.ToolbarHeight
                || !Contains(Bounds, Header)
                || !Contains(Bounds, Status)
                || Header.Overlaps(Status))
            {
                return false;
            }

            for (int i = 0; i < ActionRects.Count; i++)
            {
                Rect action = ActionRects[i];
                if (!Contains(Bounds, action)
                    || action.width < BetaLabToolbarRules.MinimumActionWidth
                    || action.height < BetaLabToolbarRules.ActionHeight)
                {
                    return false;
                }

                for (int other = i + 1; other < ActionRects.Count; other++)
                {
                    if (action.Overlaps(ActionRects[other])) return false;
                }
            }

            return true;
        }

        private static bool Contains(Rect outer, Rect inner)
        {
            const float tolerance = 0.01f;
            return inner.width >= 0f
                && inner.height >= 0f
                && inner.xMin >= outer.xMin - tolerance
                && inner.yMin >= outer.yMin - tolerance
                && inner.xMax <= outer.xMax + tolerance
                && inner.yMax <= outer.yMax + tolerance;
        }
    }

    public readonly struct BetaLabBuildFlavorProfile
    {
        public readonly BetaLabBuildFlavor Flavor;
        public readonly string DisplayName;
        public readonly string ArtifactSuffix;
        public readonly bool ShowsTitleBetaLab;
        public readonly bool RequiresUnityDevelopmentBuild;
        public readonly bool IsRetailRelease;

        public BetaLabBuildFlavorProfile(
            BetaLabBuildFlavor flavor,
            string displayName,
            string artifactSuffix,
            bool showsTitleBetaLab,
            bool requiresUnityDevelopmentBuild,
            bool isRetailRelease)
        {
            Flavor = flavor;
            DisplayName = displayName ?? "";
            ArtifactSuffix = artifactSuffix ?? "";
            ShowsTitleBetaLab = showsTitleBetaLab;
            RequiresUnityDevelopmentBuild = requiresUnityDevelopmentBuild;
            IsRetailRelease = isRetailRelease;
        }
    }

    /// <summary>
    /// Pure geometry, catalog, and navigation rules for the in-combat Beta Lab.
    /// The header occupies a fixed left rail while actions wrap into two rows,
    /// keeping all controls inside the combat board at the supported minimum.
    /// </summary>
    public static class BetaLabToolbarRules
    {
        public const float MinimumToolbarWidth = 448f;
        public const float ToolbarHeight = 60f;
        public const float OuterPadding = 4f;
        public const float HeaderGap = 6f;
        public const float ActionGap = 4f;
        public const float ActionHeight = 24f;
        public const float RowGap = 4f;
        public const float MinimumActionWidth = 64f;
        public const float CompactLabelWidth = 88f;

        public const string KeyboardNavigationHint =
            "Arrows: choose lab action · Enter/Space: use · Tab/Shift+Tab: next/previous";

        public const string ControllerNavigationHint =
            "Left stick: choose lab action · A: use · bumpers: next/previous";

        private static readonly BetaLabToolbarActionDefinition[] CasterActions =
        {
            Action(BetaLabToolbarActionId.Refill, "Refill", "Refill", "Restore party health, mana, elixirs, and clear afflictions."),
            Action(BetaLabToolbarActionId.Mage, "Mage", "Mage", "Prepare a maximum-level Mage and open the production Spellbook."),
            Action(BetaLabToolbarActionId.Warlock, "Warlock", "Warlock", "Prepare a maximum-level Warlock and open the production Spellbook."),
            Action(BetaLabToolbarActionId.Craft, "Craft", "Craft", "Unlock the appropriate spell schools for every test caster."),
            Action(BetaLabToolbarActionId.Stage, "Stage", "Stage", "Arrange legal Mage or Warlock targets and reopen the production Spellbook."),
            Action(BetaLabToolbarActionId.Hazards, "Hazards", "Fields", "Refresh tree, stone, web, gas, fire, and ice test fields."),
            Action(BetaLabToolbarActionId.Spawn, "Spawn", "Spawn", "Add caster-pressure enemies on safe open cells."),
            Action(BetaLabToolbarActionId.Reset, "Reset", "Reset", "Rebuild the save-blocked caster lab encounter."),
            Action(
                BetaLabToolbarActionId.VisualTour,
                "Visual-only Tour",
                "Visual-only",
                "Preview deterministic VFX and SFX without casting, spending resources, or changing combat state.",
                true)
        };

        private static readonly BetaLabToolbarActionDefinition[] MartialActions =
        {
            Action(BetaLabToolbarActionId.Refill, "Refill", "Refill", "Restore party health, resources, and clear afflictions."),
            Action(BetaLabToolbarActionId.Promote, "Promote", "Promote", "Unlock the production skill kits for martial testers."),
            Action(BetaLabToolbarActionId.Wound, "Wound", "Wound", "Prepare one bleeding enemy below execute range."),
            Action(BetaLabToolbarActionId.Cluster, "Cluster", "Cluster", "Arrange adjacent enemies for melee-area skill testing."),
            Action(BetaLabToolbarActionId.Reset, "Reset", "Reset", "Rebuild the save-blocked martial lab encounter."),
            Action(BetaLabToolbarActionId.Spawn, "Spawn", "Spawn", "Add melee and ranged test enemies on safe open cells."),
            Action(
                BetaLabToolbarActionId.VisualTour,
                "Visual-only Tour",
                "Visual-only",
                "Preview deterministic VFX and SFX without using a skill or changing combat state.",
                true)
        };

        private static readonly IReadOnlyList<BetaLabToolbarActionDefinition> ReadOnlyCasterActions =
            Array.AsReadOnly(CasterActions);

        private static readonly IReadOnlyList<BetaLabToolbarActionDefinition> ReadOnlyMartialActions =
            Array.AsReadOnly(MartialActions);

        public static IReadOnlyList<BetaLabToolbarActionDefinition> Actions(BetaLabKind kind)
        {
            return kind == BetaLabKind.Martial ? ReadOnlyMartialActions : ReadOnlyCasterActions;
        }

        public static BetaLabToolbarActionDefinition ActionAt(BetaLabKind kind, int index)
        {
            IReadOnlyList<BetaLabToolbarActionDefinition> actions = Actions(kind);
            return actions[NormalizeIndex(index, actions.Count)];
        }

        public static int IndexOf(BetaLabKind kind, BetaLabToolbarActionId id)
        {
            IReadOnlyList<BetaLabToolbarActionDefinition> actions = Actions(kind);
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].Id == id) return i;
            }
            return -1;
        }

        public static int FirstRowCount(BetaLabKind kind)
        {
            return (Actions(kind).Count + 1) / 2;
        }

        public static int RowForIndex(BetaLabKind kind, int index)
        {
            int safe = NormalizeIndex(index, Actions(kind).Count);
            return safe < FirstRowCount(kind) ? 0 : 1;
        }

        public static int ColumnForIndex(BetaLabKind kind, int index)
        {
            int safe = NormalizeIndex(index, Actions(kind).Count);
            int firstRow = FirstRowCount(kind);
            return safe < firstRow ? safe : safe - firstRow;
        }

        public static int NextIndex(BetaLabKind kind, int index)
        {
            int count = Actions(kind).Count;
            return index < 0 || index >= count ? 0 : NormalizeIndex(index + 1, count);
        }

        public static int PreviousIndex(BetaLabKind kind, int index)
        {
            int count = Actions(kind).Count;
            return index < 0 || index >= count ? Math.Max(0, count - 1) : NormalizeIndex(index - 1, count);
        }

        public static int Navigate(BetaLabKind kind, int index, BetaLabToolbarNavigation direction)
        {
            int count = Actions(kind).Count;
            if (count <= 0) return -1;
            if (direction == BetaLabToolbarNavigation.First) return 0;
            if (direction == BetaLabToolbarNavigation.Last) return count - 1;
            if (direction == BetaLabToolbarNavigation.Next) return NextIndex(kind, index);
            if (direction == BetaLabToolbarNavigation.Previous) return PreviousIndex(kind, index);

            int safe = index < 0 || index >= count ? 0 : index;
            int firstRow = FirstRowCount(kind);
            int row = safe < firstRow ? 0 : 1;
            int rowStart = row == 0 ? 0 : firstRow;
            int rowCount = row == 0 ? firstRow : count - firstRow;
            int column = safe - rowStart;

            if (direction == BetaLabToolbarNavigation.Left)
            {
                return rowStart + NormalizeIndex(column - 1, rowCount);
            }
            if (direction == BetaLabToolbarNavigation.Right)
            {
                return rowStart + NormalizeIndex(column + 1, rowCount);
            }

            int targetRow = row == 0 ? 1 : 0;
            int targetStart = targetRow == 0 ? 0 : firstRow;
            int targetCount = targetRow == 0 ? firstRow : count - firstRow;
            if (targetCount <= 0) return safe;
            int targetColumn = MapColumn(column, rowCount, targetCount);
            return targetStart + targetColumn;
        }

        public static BetaLabToolbarGeometry Calculate(Rect anchor, BetaLabKind kind)
        {
            float x = Finite(anchor.x) ? anchor.x : 0f;
            float y = Finite(anchor.y) ? anchor.y : 0f;
            float width = Finite(anchor.width) ? Math.Max(0f, anchor.width) : 0f;
            Rect bounds = new Rect(x, y, width, ToolbarHeight);
            float headerWidth = HeaderWidth(width);
            Rect header = new Rect(
                bounds.x + OuterPadding,
                bounds.y + OuterPadding,
                headerWidth,
                ActionHeight);
            Rect status = new Rect(
                bounds.x + OuterPadding,
                bounds.y + OuterPadding + ActionHeight + RowGap,
                headerWidth,
                ActionHeight);

            IReadOnlyList<BetaLabToolbarActionDefinition> actions = Actions(kind);
            int firstRowCount = FirstRowCount(kind);
            Rect[] actionRects = new Rect[actions.Count];
            float actionsX = header.xMax + HeaderGap;
            float actionsWidth = Math.Max(0f, bounds.xMax - OuterPadding - actionsX);
            for (int i = 0; i < actionRects.Length; i++)
            {
                int row = i < firstRowCount ? 0 : 1;
                int rowStart = row == 0 ? 0 : firstRowCount;
                int rowCount = row == 0 ? firstRowCount : actionRects.Length - firstRowCount;
                int column = i - rowStart;
                float buttonWidth = rowCount <= 0
                    ? 0f
                    : Math.Max(0f, (actionsWidth - ActionGap * Math.Max(0, rowCount - 1)) / rowCount);
                actionRects[i] = new Rect(
                    actionsX + column * (buttonWidth + ActionGap),
                    bounds.y + OuterPadding + row * (ActionHeight + RowGap),
                    buttonWidth,
                    ActionHeight);
            }

            return new BetaLabToolbarGeometry(bounds, header, status, actionRects, firstRowCount);
        }

        private static BetaLabToolbarActionDefinition Action(
            BetaLabToolbarActionId id,
            string label,
            string compactLabel,
            string description,
            bool visualOnly = false)
        {
            return new BetaLabToolbarActionDefinition(
                id,
                label,
                compactLabel,
                description,
                visualOnly);
        }

        private static float HeaderWidth(float width)
        {
            if (width < 540f) return 96f;
            if (width < 700f) return 112f;
            return 136f;
        }

        private static int MapColumn(int column, int sourceCount, int targetCount)
        {
            if (sourceCount <= 0 || targetCount <= 0) return 0;
            float normalizedCenter = (Math.Max(0, column) + 0.5f) / sourceCount;
            int mapped = (int)(normalizedCenter * targetCount);
            return Math.Max(0, Math.Min(targetCount - 1, mapped));
        }

        private static int NormalizeIndex(int index, int count)
        {
            if (count <= 0) return 0;
            int wrapped = index % count;
            return wrapped < 0 ? wrapped + count : wrapped;
        }

        private static bool Finite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }

    /// <summary>
    /// Keeps an ordinary retail artifact distinct from the opt-in Unity
    /// Development build that exposes the title-screen Beta Lab.
    /// </summary>
    public static class BetaLabBuildFlavorRules
    {
        public const string BetaArtifactSuffix = "-beta-dev";

        public static BetaLabBuildFlavorProfile ProfileFor(BetaLabBuildFlavor flavor)
        {
            if (flavor == BetaLabBuildFlavor.BetaDevelopment)
            {
                return new BetaLabBuildFlavorProfile(
                    flavor,
                    "Beta Development",
                    BetaArtifactSuffix,
                    true,
                    true,
                    false);
            }

            return new BetaLabBuildFlavorProfile(
                BetaLabBuildFlavor.Retail,
                "Retail",
                "",
                false,
                false,
                true);
        }

        public static bool MatchesUnityBuild(BetaLabBuildFlavor flavor, bool isUnityDevelopmentBuild)
        {
            return ProfileFor(flavor).RequiresUnityDevelopmentBuild == isUnityDevelopmentBuild;
        }

        public static string WindowsArtifactName(
            string executableBaseName,
            string packageVersion,
            BetaLabBuildFlavor flavor)
        {
            string executable = string.IsNullOrWhiteSpace(executableBaseName)
                ? "Game"
                : executableBaseName.Trim();
            string version = string.IsNullOrWhiteSpace(packageVersion)
                ? "unversioned"
                : packageVersion.Trim();
            BetaLabBuildFlavorProfile profile = ProfileFor(flavor);
            return executable + "-Windows-" + version + profile.ArtifactSuffix;
        }

        public static string WindowsZipFileName(
            string executableBaseName,
            string packageVersion,
            BetaLabBuildFlavor flavor)
        {
            return WindowsArtifactName(executableBaseName, packageVersion, flavor) + ".zip";
        }
    }
}
