using System;
using System.Reflection;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class WorldMapSpritePolishSmoke
    {
        private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void RunOrThrow()
        {
            GroundedCitizensShareTheNamedFootline();
            ContactCuesLeaveFacesAndNeighboringCellsClear();
            ProductionSpritesKeepOneAuthoredShadow();
        }

        private static void GroundedCitizensShareTheNamedFootline()
        {
            foreach (float size in new[] { 24f, 32f, 44f, 64f, 96f, 128f })
            foreach (bool wide in new[] { false, true })
            foreach (bool patron in new[] { false, true })
            {
                Rect cell = new Rect(37f, 91f, size, size);
                Rect sprite = ExplorationNpcPresentationRules.AmbientSpriteRect(cell, wide, patron, Vector2.zero);
                float expected = cell.y + size * ExplorationNpcPresentationRules.NamedFootlineInCells(wide);
                Near(sprite.yMax, expected, "citizens and named contacts share the same ground baseline");
                Require(sprite.yMin >= cell.yMin && sprite.yMax <= cell.yMax, "grounded citizens remain within their terrain row");

                Rect yielding = ExplorationNpcPresentationRules.AmbientSpriteRect(cell, wide, patron, new Vector2(0.16f, 0.05f));
                Require(yielding.yMax <= cell.yMax, "yielding toward the lower row never sinks into a neighboring tile");
                Near(yielding.width, sprite.width, "stepping aside preserves body scale");
                Near(yielding.x - sprite.x, size * 0.16f, "existing deterministic lateral yielding stays intact");
            }
        }

        private static void ContactCuesLeaveFacesAndNeighboringCellsClear()
        {
            foreach (float size in new[] { 24f, 32f, 44f, 64f, 96f, 128f })
            foreach (bool wide in new[] { false, true })
            {
                Rect cell = new Rect(37f, 91f, size, size);
                Rect feet = ExplorationNpcPresentationRules.ContactFootprint(cell, wide);
                Inside(feet, cell, "foot-level focus brackets stay inside the actor tile");
                Require(feet.yMin >= cell.y + size * 0.75f, "focus decoration stays below the body");
                Rect key = ExplorationNpcPresentationRules.ContactUseKey(cell);
                Inside(key, cell, "interaction key remains within the actor tile even at compact resolutions");
                Require(key.height >= 11f && key.width >= 12f, "interaction key has a readable minimum area");

                bool badgeVisible = ExplorationNpcPresentationRules.ShouldShowContactBadge(wide, size, true);
                Require(badgeVisible == (!wide && size >= 44f), "role badges only appear on readable focused Local sprites");
                Require(!ExplorationNpcPresentationRules.ShouldShowContactBadge(wide, size, false), "unfocused contacts do not grow permanent map UI");
                if (!badgeVisible) continue;
                Rect badge = ExplorationNpcPresentationRules.ContactBadge(cell);
                Inside(badge, cell, "service badge never overlaps an adjacent cell");
                Require(badge.xMin >= cell.center.x + size * 0.22f, "badge remains outside the central silhouette");
                Require(!badge.Overlaps(key) && badge.yMin >= cell.center.y, "service and E badges are separate and leave the face clear");
            }
            Require(!ExplorationNpcPresentationRules.ShouldShowContactBadge(false, float.NaN, true), "invalid scale cannot draw a badge");
            Require(!ExplorationNpcPresentationRules.ShouldShowContactBadge(false, float.PositiveInfinity, true), "infinite scale cannot draw a badge");
        }

        private static void ProductionSpritesKeepOneAuthoredShadow()
        {
            GameObject host = new GameObject("World map sprite polish audit");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                foreach (ObjectType type in Enum.GetValues(typeof(ObjectType)))
                {
                    if (!(bool)Call(game, "UsesNamedNpcPresentation", type)) continue;
                    Require(!(bool)Call(game, "ShouldReinforceExploreObjectAlpha", type),
                        type + " uses the atlas shadow once, without a double-painted fringe");
                }
                Require((bool)Call(game, "ShouldReinforceExploreObjectAlpha", ObjectType.Cache), "blocking chest art retains its existing solidity treatment");
                Require(!(bool)Call(game, "ShouldReinforceExploreObjectAlpha", ObjectType.Camp), "walkable camp does not acquire a solid overlay");

                foreach (bool wide in new[] { false, true })
                {
                    typeof(AshenHallsGame).GetField("exploreWideView", PrivateInstance).SetValue(game, wide);
                    Rect cell = new Rect(37f, 91f, 80f, 80f);
                    MapObject contact = new MapObject(4, 4, ObjectType.MarketClerk, "sprite-audit-clerk");
                    Rect bounds = (Rect)Call(game, "ExploreObjectRect", cell, contact);
                    float padding = (float)Call(game, "ExploreObjectArtPadding", contact.Type, true);
                    float inset = bounds.width * padding;
                    float artHeight = bounds.height - inset * 2f;
                    object spec = Call(game, "WorldMapArtSpecFor", contact.Type, contact);
                    const BindingFlags specFields = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                    float scale = (float)spec.GetType().GetField("Scale", specFields).GetValue(spec);
                    Vector2 offset = (Vector2)spec.GetType().GetField("Offset", specFields).GetValue(spec);
                    float actualFootline = bounds.yMax - inset + artHeight * ((scale - 1f) * 0.5f + offset.y);
                    float expectedFootline = cell.y + cell.height * ExplorationNpcPresentationRules.NamedFootlineInCells(wide);
                    Near(actualFootline, expectedFootline, "shared footline matches the production atlas transform in both views");
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static object Call(AshenHallsGame game, string method, params object[] args)
        {
            return typeof(AshenHallsGame).GetMethod(method, PrivateInstance).Invoke(game, args);
        }

        private static void Inside(Rect inner, Rect outer, string message)
        {
            Require(inner.width > 0f && inner.height > 0f && inner.xMin >= outer.xMin - 0.001f
                && inner.xMax <= outer.xMax + 0.001f && inner.yMin >= outer.yMin - 0.001f
                && inner.yMax <= outer.yMax + 0.001f, message);
        }

        private static void Near(float actual, float expected, string message)
        {
            Require(Mathf.Abs(actual - expected) < 0.001f, message + ": " + actual + " vs " + expected);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("World map sprite polish: " + message);
        }
    }
}
