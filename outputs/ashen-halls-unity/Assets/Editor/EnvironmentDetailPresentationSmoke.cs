using System;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class EnvironmentDetailPresentationSmoke
    {
        public static void RunOrThrow()
        {
            DetailEligibilityProtectsGameplayAndFog();
            SelectionIsStableSparseAndSeparated();
            GeometryAndAtlasContractRemainBounded();
        }

        private static void DetailEligibilityProtectsGameplayAndFog()
        {
            for (int flags = 0; flags < 64; flags++)
            {
                bool charted = (flags & 1) != 0;
                bool interior = (flags & 2) != 0;
                bool occupied = (flags & 4) != 0;
                bool focus = (flags & 8) != 0;
                bool guidance = (flags & 16) != 0;
                int tile = (flags & 32) != 0 ? 0 : 1;
                bool expected = charted && !interior && !occupied && !focus && !guidance && tile == 1;
                Require(ExplorationEnvironmentDetailRules.Eligible(charted, interior, tile,
                    ExplorationCellRole.None, occupied, focus, guidance, ExplorationMaterial.Moss) == expected,
                    "details require revealed exterior open ground, clear of actors and guidance");
            }
            foreach (ExplorationCellRole role in new[] { ExplorationCellRole.Road, ExplorationCellRole.Trail,
                ExplorationCellRole.Plaza, ExplorationCellRole.Threshold })
                Require(!ExplorationEnvironmentDetailRules.Eligible(true, false, 1, role, false, false, false, ExplorationMaterial.Moss),
                    "paths, plazas and door approaches remain clear of cosmetic clusters");
            foreach (ExplorationMaterial material in Enum.GetValues(typeof(ExplorationMaterial)))
            for (int variant = -3; variant <= 3; variant++)
            {
                int index = ExplorationEnvironmentDetailRules.CellFor(material, variant);
                Require(index >= -1 && index < ExplorationEnvironmentDetailRules.CellCount, "all materials resolve only authored atlas cells");
            }
            foreach (ExplorationMaterial material in new[] { ExplorationMaterial.DeepWater, ExplorationMaterial.ShallowWater,
                ExplorationMaterial.CityWall, ExplorationMaterial.Cliff, ExplorationMaterial.BridgeDeck })
                Require(!ExplorationEnvironmentDetailRules.Eligible(true, false, 1, ExplorationCellRole.None, false, false, false, material),
                    "unsupported water, walls and bridges cannot acquire misleading ground scenery");
        }

        private static void SelectionIsStableSparseAndSeparated()
        {
            foreach (int seed in new[] { 0, 9173, -42, int.MaxValue })
            foreach (int depth in new[] { 1, 2, 4 })
            {
                int selected = 0;
                for (int y = 0; y < 80; y++)
                for (int x = 0; x < 80; x++)
                {
                    bool pick = ExplorationEnvironmentDetailRules.Selected(x, y, seed, depth, ExplorationMaterial.Moss);
                    Require(pick == ExplorationEnvironmentDetailRules.Selected(x, y, seed, depth, ExplorationMaterial.Moss),
                        "selection is repeatable without frame, viewport, or mutable random state");
                    if (!pick) continue;
                    selected++;
                    Require(!ExplorationEnvironmentDetailRules.Selected(x + 1, y, seed, depth, ExplorationMaterial.Moss)
                        && !ExplorationEnvironmentDetailRules.Selected(x, y + 1, seed, depth, ExplorationMaterial.Moss),
                        "adjacent details cannot turn into repetitive clutter rows");
                }
                Require(selected >= 220 && selected <= 520, "large fields retain a sparse 3.4-8.2 percent scenery density");
            }
        }

        private static void GeometryAndAtlasContractRemainBounded()
        {
            Require(ExplorationEnvironmentDetailRules.Width / (float)ExplorationEnvironmentDetailRules.Columns
                == ExplorationEnvironmentDetailRules.Height / (float)ExplorationEnvironmentDetailRules.Rows, "fractional atlas cells remain exactly square without resampling");
            foreach (float size in new[] { 8f, 18f, 32f, 64f, 128f })
            for (uint variant = 0; variant < 1000; variant++)
            {
                Rect cell = new Rect(17f, 29f, size, size);
                Rect detail = ExplorationEnvironmentDetailRules.DetailRect(cell, variant);
                Require(detail.xMin >= cell.xMin && detail.xMax <= cell.xMax
                    && detail.yMin >= cell.yMin && detail.yMax <= cell.yMax,
                    "cosmetic art cannot bleed over neighbors, roads, fog or interactable footprints");
            }
            Require(ExplorationEnvironmentDetailRules.Opacity(true) < ExplorationEnvironmentDetailRules.Opacity(false)
                && ExplorationEnvironmentDetailRules.Opacity(false) <= 1f, "Region overview keeps quieter cosmetic details");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Environment detail presentation smoke: " + message);
        }
    }
}
