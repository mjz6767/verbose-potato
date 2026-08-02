using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private bool maudPermanentEnchantment;

        private int maudEnchantmentTargetIndex = -1;

        private readonly List<Tween> tweens = new List<Tween>();

        private readonly List<FloatText> floatTexts = new List<FloatText>();

        private readonly List<ParticleDot> particles = new List<ParticleDot>();

        private readonly List<BeamEffect> beams = new List<BeamEffect>();

        private readonly List<CellFlash> flashes = new List<CellFlash>();

        private readonly List<CastGlyph> castGlyphs = new List<CastGlyph>();

        private readonly List<PowerImpactEcho> powerImpactEchoes = new List<PowerImpactEcho>();

        private readonly List<PowerCastAura> powerCastAuras = new List<PowerCastAura>();

        private readonly List<CombatUnitPresentationBeat> combatUnitPresentationBeats = new List<CombatUnitPresentationBeat>();

        private readonly List<Rect> combatTooltipBlockers = new List<Rect>(32);

        private float combatShakeStarted;

        private float combatShakeUntil;

        private float combatShakeMagnitude;

        private float combatImpactFrameStarted;

        private float combatImpactFrameUntil;

        private string combatImpactFrameColor = "";

        private int combatImpactFrameIntensity;

        private float combatVfxImpactDelay;

        private bool targetedMartialHitConnected = true;

        private readonly List<string> combatPowerReactions = new List<string>(2);

        private float pendingCombatPowerOutcomeDelay;

        private float enemyActionResolutionDelay;

        private string enemyActionResolutionLabel = "";

        private struct EnemyPowerBeat
        {
            public CombatImpactProfile Profile;
            public CombatPowerOutcomeSnapshot Before;
            public float PreviousVfxDelay;
            public int ImpactX;
            public int ImpactY;
            public Color Color;
        }

        private struct StatusMark
        {
            public string Label;
            public Color Color;
            public int Turns;

            public StatusMark(string label, Color color, int turns)
            {
                Label = label;
                Color = color;
                Turns = turns;
            }
        }

        private sealed class CombatActionCard
        {
            public string Id;
            public string Name;
            public string Kind;
            public string Cost;
            public string Range;
            public string Target;
            public string Path;
            public string Impact;
            public string Summary;
            public string CurrentEffect;
            public string Detail;
            public bool Targeted;
            public bool Ready;
            public bool Usable;
            public string DisabledReason;
        }

        private void DrawCombat()
        {
            float presentationNow = Time.time;
            CombatUnitPresentationRules.PruneAndBound(combatUnitPresentationBeats, presentationNow);
            sideRect = SidePanelRect();
            boardRect = GetBoardRect();
            DrawPanel(boardRect);
            Rect grid = CombatBoardInnerRect(boardRect);
            float cell = Mathf.Min(grid.width / CombatW, grid.height / CombatH);
            grid.width = cell * CombatW;
            grid.height = cell * CombatH;
            grid = ApplyCombatImpactShake(grid);
            DrawRect(new Rect(grid.x - 14, grid.y - 14, grid.width + 28, grid.height + 28), retroBlack);
            DrawRect(new Rect(grid.x - 14, grid.y - 14, grid.width * 0.24f, grid.height + 28), Hex("17352e", 0.30f));
            DrawRect(new Rect(grid.x + grid.width * 0.76f, grid.y - 14, grid.width * 0.24f + 28, grid.height + 28), Hex("43221f", 0.30f));

            CombatUnit active = CurrentUnit();
            for (int y = 0; y < CombatH; y++)
            for (int x = 0; x < CombatW; x++)
            {
                Rect c = new Rect(grid.x + x * cell, grid.y + y * cell, cell, cell);
                DrawRect(c, (x + y) % 2 == 0 ? Hex("101612") : Hex("0b110f"));
                Point obstacle = ObstacleAt(x, y);
                DrawCombatTerrainTexture(c, x, y, obstacle);
                DrawBorder(c, Hex("253029", 0.28f), 1);
                if (obstacle != null)
                {
                    DrawCombatObstacle(c, obstacle);
                }
                DrawCellFlash(c, x, y);
            }

            bool resolvingPower = IsCombatResolutionPending();
            if (active != null && active.Side == UnitSide.Party && !resolvingPower)
            {
                NormalizeCombatSelection(active);
                DrawCombatHighlights(grid, cell, active);
            }

            // Ground and cast art stays below combatants so silhouettes remain readable.
            DrawCastGlyphs(grid, cell);
            DrawPowerCastAuras(grid, cell);
            DrawPowerImpactEchoes(grid, cell);

            foreach (CombatUnit unit in state.Combat.Units)
            {
                CombatUnitPresentationRules.TryGetBeat(
                    combatUnitPresentationBeats,
                    unit.Id,
                    presentationNow,
                    out CombatUnitPresentationBeat beat);
                if (!CombatUnitPresentationRules.ShouldRenderActor(unit.Hp > 0, beat, presentationNow)) continue;

                Vector2 pos = UnitDrawPos(unit);
                Rect cellRect = new Rect(grid.x + pos.x * cell, grid.y + pos.y * cell, cell, cell);
                bool isActive = active != null && active.Id == unit.Id;
                CombatUnitPresentationPose pose = CombatUnitPresentationRules.PoseFor(
                    beat,
                    presentationNow,
                    state.ReducedMotion);
                Rect anchoredRect = Pad(cellRect, cell * 0.01f);
                Rect spriteRect = CombatUnitPresentationRules.ApplyPose(anchoredRect, pose);
                DrawCombatUnitSprite(spriteRect, anchoredRect, unit, isActive, pose.Alpha);
            }

            // Foreground action art may cross unit cells, but never the tactical readout.
            DrawBeams(grid, cell);
            DrawParticles(grid, cell);
            DrawCombatPowerPulse(grid);

            if (active != null && active.Side == UnitSide.Party && !resolvingPower)
            {
                DrawCombatTargetStateShapes(grid, cell, active);
                DrawEnemyThreatCues(grid, cell, active);
                DrawHoverAim(grid, cell, active);
            }

            if (!resolvingPower && active != null && active.Side == UnitSide.Enemy && state.Combat.Phase == CombatPhase.EnemyThinking)
            {
                DrawEnemyIntentCue(grid, cell, active);
            }

            // Selection, status and the single HP rail are always the top board layer.
            foreach (CombatUnit unit in state.Combat.Units)
            {
                CombatUnitPresentationRules.TryGetBeat(
                    combatUnitPresentationBeats,
                    unit.Id,
                    presentationNow,
                    out CombatUnitPresentationBeat beat);
                if (!CombatUnitPresentationRules.ShouldRenderTacticalOverlay(
                    unit.Hp > 0,
                    beat,
                    presentationNow))
                {
                    continue;
                }

                Vector2 pos = UnitDrawPos(unit);
                Rect cellRect = new Rect(grid.x + pos.x * cell, grid.y + pos.y * cell, cell, cell);
                bool isActive = active != null && active.Id == unit.Id;
                DrawCombatStatusFrame(cellRect, unit, isActive, cell);
                if (isActive) DrawActiveCursor(cellRect, cell);
                float hpHeight = Mathf.Clamp(cell * 0.095f, 6f, 10f);
                Rect hp = new Rect(
                    cellRect.x + cell * 0.10f,
                    cellRect.yMax - hpHeight - cell * 0.055f,
                    cell * 0.80f,
                    hpHeight);
                float hpRatio = unit.MaxHp <= 0 ? 0f : Mathf.Clamp01((float)unit.Hp / unit.MaxHp);
                Color hpColor = unit.Side == UnitSide.Party
                    ? hpRatio <= 0.30f ? gold : Hex("58b7a5")
                    : blood;
                DrawRect(hp, Hex("050708", 0.94f));
                DrawRect(new Rect(hp.x + 1f, hp.y + 1f, Mathf.Max(0f, (hp.width - 2f) * hpRatio), Mathf.Max(1f, hp.height - 2f)), hpColor);
                DrawBorder(hp, unit.Side == UnitSide.Party ? Hex("58b7a5", 0.72f) : Hex("b94b56", 0.72f), 1);
                DrawStatusPips(cellRect, unit, cell);
            }

            DrawFloatingTextLayer(grid, cell, Time.time);
            if (!resolvingPower) DrawHoverPreview(grid, cell, active);
            HandleCombatMouse(grid, cell);
        }

        private void DrawCombatSpeckles(Rect rect, int x, int y)
        {
            float size = Mathf.Max(1f, rect.width * 0.025f);
            for (int i = 0; i < 8; i++)
            {
                int seed = Mathf.Abs(x * 92821 + y * 68917 + i * 19333 + state.Depth * 8191);
                float px = 0.10f + (seed % 79) / 98f;
                float py = 0.10f + ((seed / 97) % 79) / 98f;
                Color dot = i % 3 == 0 ? Hex("6f8d4d", 0.52f) : i % 3 == 1 ? Hex("37543a", 0.48f) : Hex("99a66a", 0.36f);
                DrawRect(new Rect(rect.x + rect.width * px, rect.y + rect.height * py, size, size), dot);
            }
        }

        private void DrawCombatTerrainTexture(Rect rect, int x, int y, Point obstacle)
        {
            Point groundObstacle = obstacle != null && CombatFieldPresentationRules.UsesDedicatedGroundSprite(obstacle.Kind)
                ? obstacle
                : null;
            int index = CombatTerrainTextureIndex(x, y, groundObstacle);
            if (index < 0) return;
            float alpha = groundObstacle == null ? 0.86f : 0.98f;
            bool drewAtlas = false;
            bool drewKoboldAtlas = false;
            int koboldIndex = KoboldCombatTerrainTextureIndex(x, y, groundObstacle, index);
            string groundKind = groundObstacle?.Kind ?? "";
            if (groundKind == "smoke")
            {
                DrawCombatTerrainFallback(rect, index, x, y, groundObstacle);
                DrawCombatTileVignette(rect, index, groundObstacle, false);
                return;
            }
            bool requiresSemanticKoboldCell = groundKind == "gas"
                || groundKind == "sanctuary"
                || groundKind == "curse"
                || groundKind == "glyph"
                || groundKind == "demonrift";
            if (koboldIndex >= 0 && TryDrawKoboldCombatTerrainAtlasIcon(Pad(rect, 1f), koboldIndex, Color.white.WithAlpha(Mathf.Min(1f, alpha + 0.04f))))
            {
                DrawRect(rect, Hex("030405", groundObstacle == null ? 0.045f : 0.02f));
                drewAtlas = true;
                drewKoboldAtlas = true;
            }
            else if (!requiresSemanticKoboldCell && TryDrawCombatTerrainAtlasIcon(Pad(rect, 1f), index, Color.white.WithAlpha(alpha)))
            {
                DrawRect(rect, Hex("030405", groundObstacle == null ? 0.06f : 0.025f));
                drewAtlas = true;
            }
            if (!drewAtlas) DrawCombatTerrainFallback(rect, index, x, y, groundObstacle);
            DrawCombatTileVignette(rect, index, groundObstacle, drewKoboldAtlas);
        }

        private void DrawCombatTerrainFallback(Rect rect, int index, int x, int y, Point obstacle)
        {
            if (obstacle != null && CombatFieldPresentationRules.IsPersistentField(obstacle.Kind))
            {
                // Procedural field art is a fail-safe only. In normal play the authored
                // terrain atlas owns the tile and this branch is never reached.
                DrawPersistentFieldSurface(rect, obstacle);
                return;
            }
            if (obstacle != null && CombatRitualRules.IsRitual(obstacle))
            {
                Color ritualAccent = ObstacleAccent(obstacle.Kind);
                DrawRect(rect, Color.Lerp(Hex("050708"), ritualAccent, 0.24f).WithAlpha(0.90f));
                int ritualIcon = TerrainMagicIconIndex(obstacle.Kind);
                if (ritualIcon >= 0
                    && TryDrawMagicUiAtlasIcon(Pad(rect, rect.width * 0.08f), ritualIcon, Color.white.WithAlpha(0.92f)))
                {
                    return;
                }
                DrawBorder(Pad(rect, rect.width * 0.12f), ritualAccent.WithAlpha(0.74f), 2);
                return;
            }

            int noise = CombatTileNoise(x, y, index);
            int depth = state?.Depth ?? 1;
            Color baseTint = depth <= 1 ? Hex("24342a") : depth == 2 ? Hex("243137") : depth == 3 ? Hex("2a2c2b") : depth == 4 ? Hex("1d3038") : Hex("332321");
            if (index == 12) baseTint = Hex("244254");
            if (index == 13) baseTint = Hex("2b2b32");
            if (index == 14) baseTint = Hex("3b211d");
            if (index == 0 && obstacle != null && obstacle.Kind == "gas") baseTint = Hex("263a26");
            if (index == 0 && obstacle != null && obstacle.Kind == "smoke") baseTint = Hex("283333");

            DrawRect(rect, baseTint.WithAlpha(0.44f));
            Rect inset = Pad(rect, rect.width * 0.07f);
            DrawRect(inset, Color.Lerp(baseTint, retroBlack, 0.35f).WithAlpha(0.38f));

            Color seam = depth == 2 ? Hex("53696b", 0.18f) : depth >= 5 ? Hex("d98b6a", 0.16f) : Hex("8aa071", 0.14f);
            if (index == 12) seam = frost.WithAlpha(0.28f);
            if (index == 13) seam = Hex("d9d3c4", 0.22f);
            if (index == 14) seam = ember.WithAlpha(0.26f);

            if (index == 12)
            {
                DrawRect(new Rect(inset.x + inset.width * 0.08f, inset.y + inset.height * 0.25f, inset.width * 0.70f, Mathf.Max(1f, inset.height * 0.035f)), frost.WithAlpha(0.50f));
                DrawRect(new Rect(inset.x + inset.width * 0.26f, inset.y + inset.height * 0.58f, inset.width * 0.62f, Mathf.Max(1f, inset.height * 0.035f)), Hex("d6f4ff", 0.36f));
                DrawBorder(inset, frost.WithAlpha(0.24f), 1);
                return;
            }
            if (index == 13)
            {
                DrawRect(new Rect(inset.x, inset.y + inset.height * 0.48f, inset.width, Mathf.Max(1f, inset.height * 0.035f)), seam);
                DrawRect(new Rect(inset.x + inset.width * 0.48f, inset.y, Mathf.Max(1f, inset.width * 0.035f), inset.height), seam);
                DrawRect(new Rect(inset.x + inset.width * 0.18f, inset.y + inset.height * 0.20f, inset.width * 0.64f, Mathf.Max(1f, inset.height * 0.025f)), seam.WithAlpha(0.20f));
                DrawRect(new Rect(inset.x + inset.width * 0.18f, inset.y + inset.height * 0.76f, inset.width * 0.64f, Mathf.Max(1f, inset.height * 0.025f)), seam.WithAlpha(0.18f));
                return;
            }
            if (index == 14)
            {
                DrawRect(new Rect(inset.x + inset.width * 0.20f, inset.y + inset.height * 0.62f, inset.width * 0.58f, inset.height * 0.06f), ember.WithAlpha(0.34f));
                DrawRect(new Rect(inset.x + inset.width * 0.36f, inset.y + inset.height * 0.36f, inset.width * 0.28f, inset.height * 0.05f), gold.WithAlpha(0.24f));
                DrawRect(new Rect(inset.x + inset.width * 0.56f, inset.y + inset.height * 0.48f, inset.width * 0.18f, inset.height * 0.04f), blood.WithAlpha(0.30f));
                return;
            }

            int cracks = 2 + noise % 3;
            for (int i = 0; i < cracks; i++)
            {
                int n = CombatTileNoise(x, y, index + i * 97);
                float px = 0.14f + (n % 64) / 90f;
                float py = 0.16f + ((n / 73) % 62) / 92f;
                float w = rect.width * (0.12f + (n % 5) * 0.015f);
                float h = Mathf.Max(1f, rect.height * (0.018f + (n % 3) * 0.006f));
                DrawRect(new Rect(rect.x + rect.width * px, rect.y + rect.height * py, w, h), seam);
            }

            if (depth <= 1)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.10f, rect.y + rect.height * 0.47f, rect.width * 0.80f, Mathf.Max(2f, rect.height * 0.035f)), Hex("a08355", 0.11f));
            }
            else if (depth == 2)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.12f, Mathf.Max(1f, rect.width * 0.025f), rect.height * 0.70f), Hex("7ea0a0", 0.10f));
                DrawRect(new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.22f, Mathf.Max(1f, rect.width * 0.025f), rect.height * 0.58f), Hex("7ea0a0", 0.09f));
            }
        }

        private void DrawCombatTileVignette(Rect rect, int index, Point obstacle, bool drewKoboldAtlas)
        {
            DrawRect(new Rect(rect.x, rect.y, rect.width, Mathf.Max(1f, rect.height * 0.035f)), Hex("f3ead7", obstacle == null ? 0.035f : 0.055f));
            DrawRect(new Rect(rect.x, rect.yMax - Mathf.Max(1f, rect.height * 0.040f), rect.width, Mathf.Max(1f, rect.height * 0.040f)), Hex("030405", 0.18f));
            if (!drewKoboldAtlas && index == 0 && obstacle != null && obstacle.Kind == "gas")
            {
                DrawRect(Pad(rect, rect.width * 0.18f), poison.WithAlpha(0.12f));
            }
            if (index == 0 && obstacle != null && obstacle.Kind == "smoke")
            {
                DrawRect(Pad(rect, rect.width * 0.12f), Hex("8fa7a2", 0.10f));
            }
        }

        private int CombatTileNoise(int x, int y, int salt)
        {
            unchecked
            {
                int seed = (state?.Seed ?? 0) + (state?.Depth ?? 0) * 73856093 + x * 19349663 + y * 83492791 + salt * 265443576;
                seed ^= seed << 13;
                seed ^= seed >> 17;
                seed ^= seed << 5;
                return seed & 0x7fffffff;
            }
        }

        private int CombatTerrainTextureIndex(int x, int y, Point obstacle)
        {
            string kind = obstacle?.Kind ?? "";
            if (kind == "fire") return 11;
            if (kind == "ice") return 9;
            if (kind == "web") return 10;
            if (kind == "gas") return 0;
            if (kind == "smoke") return 0;
            if (kind == "sanctuary") return 4;
            if (kind == "curse") return 12;
            if (kind == "glyph") return 8;
            if (kind == "demonrift") return 12;
            if (kind == "stone") return 14;
            if (kind == "tree") return 0;

            string zoneId = CombatTerrainZoneId();
            int variant = CombatTerrainPatchVariant(x, y, zoneId);
            return ZoneCombatTerrainTextureIndex(zoneId, variant);
        }

        private string CombatTerrainZoneId()
        {
            string encounter = state?.Combat?.EncounterStyle ?? "";
            if (encounter == "ratsewer") return "salt-cisterns";
            if (encounter == "koboldambush" || encounter == "koboldcave" || encounter == "koboldking") return "dusk-market";
            if (encounter == "lab" || encounter == "martiallab") return "inner-ash-road";
            if (state?.Map != null) return ZoneAt(state.PlayerX, state.PlayerY)?.Id ?? "inner-ash-road";
            int depth = state?.Depth ?? 1;
            if (depth <= 1) return "midgaard-road";
            if (depth == 2) return "salt-cisterns";
            if (depth == 3) return "gloam-courts";
            if (depth == 4) return "glass-warrens";
            return "red-gate";
        }

        private int CombatTerrainPatchVariant(int x, int y, string zoneId)
        {
            int seed = (state?.Seed ?? 0) + StableSeed(zoneId ?? "") * 3 + (x / 3) * 97 + (y / 2) * 131;
            return Mathf.Abs(seed) % 5;
        }

        private int ZoneCombatTerrainTextureIndex(string zoneId, int variant)
        {
            switch (zoneId ?? "")
            {
                case "midgaard-city":
                    return new[] { 4, 5, 14, 4, 15 }[variant];
                case "midgaard-road":
                case "inner-ash-road":
                    return new[] { 2, 3, 4, 14, 15 }[variant];
                case "green-shrine-road":
                    return new[] { 0, 14, 5, 4, 2 }[variant];
                case "salt-cisterns":
                    return new[] { 7, 13, 4, 6, 3 }[variant];
                case "old-quarry":
                    return new[] { 6, 14, 4, 8, 3 }[variant];
                case "glass-warrens":
                    return new[] { 8, 6, 9, 4, 13 }[variant];
                case "ash-fen":
                    return new[] { 0, 3, 13, 10, 11 }[variant];
                case "dusk-market":
                    return new[] { 2, 3, 5, 15, 4 }[variant];
                case "gloam-courts":
                    return new[] { 4, 5, 10, 14, 6 }[variant];
                case "red-gate":
                    return new[] { 11, 12, 3, 4, 6 }[variant];
            }
            int depth = state?.Depth ?? 1;
            if (depth <= 1)
            {
                return new[] { 2, 3, 4, 14, 15 }[variant];
            }
            if (depth == 2)
            {
                return new[] { 7, 13, 4, 6, 3 }[variant];
            }
            if (depth == 3)
            {
                return new[] { 6, 14, 4, 8, 3 }[variant];
            }
            if (depth == 4)
            {
                return new[] { 8, 6, 9, 4, 13 }[variant];
            }
            return new[] { 11, 12, 3, 4, 6 }[variant];
        }

        private int KoboldCombatTerrainTextureIndex(int x, int y, Point obstacle, int fallbackIndex)
        {
            string kind = obstacle?.Kind ?? "";
            if (!IsKoboldRouteCombat())
            {
                // The generic terrain atlas has strong fire, ice, and web cells but no
                // semantic gas, sanctuary, curse, glyph, or rift cells. Borrow the
                // authored full-tile cave effects instead of presenting ordinary floor.
                if (kind == "gas") return 6;
                if (kind == "sanctuary") return 14;
                if (kind == "curse") return 7;
                if (kind == "glyph") return 4;
                if (kind == "demonrift") return 7;
                return -1;
            }
            if (kind == "fire") return 12;
            if (kind == "ice") return 8;
            if (kind == "web") return 9;
            if (kind == "gas") return 6;
            if (kind == "sanctuary") return 14;
            if (kind == "curse") return 7;
            if (kind == "glyph") return 4;
            if (kind == "demonrift") return 7;
            if (kind == "stone") return 10;
            if (kind == "tree") return 0;

            int variant = CombatTerrainPatchVariant(x, y, "kobold-" + (state?.Combat?.EncounterStyle ?? ""));
            string encounter = state?.Combat?.EncounterStyle ?? "";
            if (encounter == "koboldking")
            {
                int[] hall = { 10, 10, 2, 11, 5 };
                return hall[variant];
            }
            if (encounter == "koboldambush")
            {
                int[] market = { 3, 3, 0, 10, 11 };
                return market[variant];
            }
            int[] cave = { 0, 0, 1, 10, 15 };
            return cave[variant];
        }

        private void DrawActiveCursor(Rect cellRect, float cell)
        {
            Rect outer = Pad(cellRect, cell * 0.08f);
            float pulse = state.ReducedMotion ? 0.2f : 0.5f + Mathf.Sin(Time.time * 6f) * 0.5f;
            Color cursor = Color.Lerp(cursorWhite, gold, pulse * 0.22f);
            float tick = cell * 0.20f;
            float stroke = 2f;
            DrawRect(new Rect(outer.x, outer.y, tick, stroke), cursor);
            DrawRect(new Rect(outer.x, outer.y, stroke, tick), cursor);
            DrawRect(new Rect(outer.xMax - tick, outer.y, tick, stroke), cursor);
            DrawRect(new Rect(outer.xMax - stroke, outer.y, stroke, tick), cursor);
            DrawRect(new Rect(outer.x, outer.yMax - stroke, tick, stroke), cursor);
            DrawRect(new Rect(outer.x, outer.yMax - tick, stroke, tick), cursor);
            DrawRect(new Rect(outer.xMax - tick, outer.yMax - stroke, tick, stroke), cursor);
            DrawRect(new Rect(outer.xMax - stroke, outer.yMax - tick, stroke, tick), cursor);
        }

        private void DrawUnitBase(Rect cellRect, CombatUnit unit, float cell)
        {
            float hp = unit.MaxHp <= 0 ? 0f : Mathf.Clamp01((float)unit.Hp / unit.MaxHp);
            Color side = unit.Side == UnitSide.Party ? teal : blood;
            Rect baseRect = new Rect(cellRect.x + cell * 0.22f, cellRect.y + cell * 0.72f, cell * 0.56f, cell * 0.13f);
            DrawRect(baseRect, Hex("020303", 0.72f));
            DrawRect(new Rect(baseRect.x + cell * 0.05f, baseRect.y + cell * 0.035f, (baseRect.width - cell * 0.10f) * hp, baseRect.height * 0.34f), Color.Lerp(side, cursorWhite, 0.16f));
        }

        private void DrawCombatStatusFrame(Rect cellRect, CombatUnit unit, bool active, float cell)
        {
            Color status = CombatFrameColor(unit, active);
            Rect outer = Pad(cellRect, cell * 0.055f);
            float stroke = active ? 3f : 2f;
            float corner = Mathf.Max(5f, cell * (active ? 0.18f : 0.12f));
            Color edge = status.WithAlpha(active ? 0.96f : 0.78f);
            if (active) DrawBorder(outer, edge, 2);
            else DrawRect(new Rect(outer.x, outer.y + corner, stroke, Mathf.Max(2f, outer.height - corner * 2f)), edge);
            DrawRect(new Rect(outer.x, outer.y, corner, stroke), edge);
            DrawRect(new Rect(outer.x, outer.y, stroke, corner), edge);
            DrawRect(new Rect(outer.xMax - corner, outer.y, corner, stroke), edge);
            DrawRect(new Rect(outer.xMax - stroke, outer.y, stroke, corner), edge);
            DrawRect(new Rect(outer.x, outer.yMax - stroke, corner, stroke), edge);
            DrawRect(new Rect(outer.xMax - corner, outer.yMax - stroke, corner, stroke), edge);

            // Persistent status text now lives in hover cards and side panels so the sprite stays readable.
        }

        private Color CombatFrameColor(CombatUnit unit, bool active)
        {
            if (unit == null) return muted;
            if (active) return gold;
            if (unit.Poisoned > 0 || unit.Webbed > 0) return poison;
            if (unit.Hexed > 0 || unit.Sleeping > 0 || unit.Stunned > 0) return violet;
            if (unit.DemonFormTurns > 0) return violet;
            if (unit.Shielded > 0 || unit.Regenerating > 0) return teal;
            if (unit.Guarding) return Hex("a9b0a2");
            if (unit.MaxHp > 0 && unit.Hp <= Mathf.CeilToInt(unit.MaxHp * 0.42f)) return blood;
            return unit.Side == UnitSide.Party ? Hex("58b7a5", 0.78f) : Hex("b94b56", 0.78f);
        }

        private string CombatPhaseLabel()
        {
            if (state?.Combat == null) return "No combat";
            if (IsCombatResolutionPending())
            {
                return string.IsNullOrWhiteSpace(combatResolutionLabel)
                    ? "Resolving power"
                    : "Resolving " + combatResolutionLabel;
            }
            switch (state.Combat.Phase)
            {
                case CombatPhase.ChooseTarget: return "Choose target";
                case CombatPhase.Resolving: return "Resolving";
                case CombatPhase.EnemyThinking: return "Enemy thinking";
                default: return "Choose action";
            }
        }

        private bool TryCombatHoverCell(
            Rect grid,
            float cell,
            out int x,
            out int y,
            out Vector2 pointer)
        {
            x = -1;
            y = -1;
            pointer = Vector2.zero;
            if (cell <= 0f) return false;

            if (visualSmokeCombatHoverCell.HasValue)
            {
                Vector2Int staged = visualSmokeCombatHoverCell.Value;
                if (staged.x < 0 || staged.x >= CombatW || staged.y < 0 || staged.y >= CombatH) return false;
                x = staged.x;
                y = staged.y;
                pointer = new Vector2(
                    grid.x + (x + 0.5f) * cell,
                    grid.y + (y + 0.5f) * cell);
                return true;
            }

            Event current = Event.current;
            if (current == null || !grid.Contains(current.mousePosition)) return false;
            pointer = current.mousePosition;
            x = Mathf.FloorToInt((pointer.x - grid.x) / cell);
            y = Mathf.FloorToInt((pointer.y - grid.y) / cell);
            return x >= 0 && x < CombatW && y >= 0 && y < CombatH;
        }

        private void DrawHoverPreview(Rect grid, float cell, CombatUnit active)
        {
            if (active == null
                || active.Side != UnitSide.Party
                || !TryCombatHoverCell(grid, cell, out int x, out int y, out Vector2 pointer))
            {
                return;
            }

            string text = "";
            CombatUnit target = UnitAt(x, y);
            if (selectedAction == ActionMode.Move)
            {
                int distance = Distance(x, y, active.X, active.Y);
                int moveCost = MoveCostTo(active, x, y);
                string terrain = TerrainPreviewLine(ObstacleAt(x, y));
                if (distance <= 0) text = "current tile";
                else if (!CanStandAt(x, y)) text = "blocked";
                else if (moveCost >= UnreachableMoveCost) text = $"path blocked{terrain}";
                else if (moveCost <= state.Combat.MovePoints)
                {
                    string threat = ProjectedMoveThreatSummary(active, x, y);
                    text = $"move {moveCost}, {state.Combat.MovePoints - moveCost} left / {threat}{terrain}";
                }
                else text = $"too far by {moveCost - state.Combat.MovePoints}{terrain}";
            }
            else if (selectedAction == ActionMode.Attack && target != null)
            {
                text = AttackPreview(active, target);
            }
            else if (selectedAction == ActionMode.Attack)
            {
                Point cover = ObstacleAt(x, y);
                if (IsDisruptableRitual(cover)) text = RitualAttackPreview(active, cover);
                else if (IsBreakableCover(cover)) text = CoverAttackPreview(active, cover);
            }
            else if (selectedAction == ActionMode.Cast)
            {
                FormulaDef formula = GetFormula(pendingFormulaCode);
                if (formula != null) text = FormulaPreview(active, formula, target, x, y);
                else text = "choose a spell card first";
            }
            else if (selectedAction == ActionMode.Ability)
            {
                text = AbilityPreview(active, target, x, y);
            }

            if (string.IsNullOrEmpty(text)) return;
            if (CombatTargetingRules.RoutesUnitPreviewToSideRail(
                    selectedAction,
                    target != null))
            {
                return;
            }
            Point coverTarget = ObstacleAt(x, y);
            string title = HoverPreviewTitle(active, target, coverTarget);
            Color accent = HoverPreviewAccent(text, target, coverTarget);
            string[] previewLines = text.Split(new[] { '\n' }, 2);
            float tooltipWidth = Mathf.Clamp(grid.width * 0.42f, 280f, 330f);
            float tooltipHeight = previewLines.Length > 1 ? 108f : 92f;
            Rect box = PlaceCombatBoardTooltip(
                grid,
                cell,
                active,
                x,
                y,
                pointer,
                tooltipWidth,
                tooltipHeight);
            DrawCombatTooltipBackplate(box, accent, 1);
            DrawActionButtonGlyph(new Rect(box.x + 9, box.y + 8, 36, 36), selectedAction, true, true);
            GUI.Label(new Rect(box.x + 55, box.y + 5, box.width - 67, 20), title, CenterLeftStyle(13, cursorWhite));
            GUI.Label(new Rect(box.x + 55, box.y + 27, box.width - 67, 17), FitText(previewLines[0], box.width - 67, CenterLeftStyle(12, ink)), CenterLeftStyle(12, ink));
            if (previewLines.Length > 1)
            {
                GUI.Label(new Rect(box.x + 9, box.y + 50, box.width - 18, 16), FitText(previewLines[1], box.width - 18, CenterLeftStyle(10, muted)), CenterLeftStyle(10, muted));
            }
            string tileLine = CombatHoverTileLine(x, y, coverTarget);
            float tileY = previewLines.Length > 1 ? 68f : 49f;
            GUI.Label(new Rect(box.x + 9, box.y + tileY, box.width - 18, 16), FitText(tileLine, box.width - 18, CenterLeftStyle(10, gold)), CenterLeftStyle(10, gold));
            GUI.Label(new Rect(box.x + 9, box.yMax - 22f, box.width - 18, 16), FitText(HoverClickInstruction(active, target, coverTarget, x, y), box.width - 18, CenterLeftStyle(10, muted)), CenterLeftStyle(10, muted));
        }

        private Rect PlaceCombatBoardTooltip(
            Rect grid,
            float cell,
            CombatUnit active,
            int targetX,
            int targetY,
            Vector2 pointer,
            float width,
            float height)
        {
            combatTooltipBlockers.Clear();
            if (state?.Combat?.Units != null)
            {
                foreach (CombatUnit unit in state.Combat.Units)
                {
                    if (unit == null || unit.Hp <= 0) continue;
                    combatTooltipBlockers.Add(new Rect(
                        grid.x + unit.X * cell,
                        grid.y + unit.Y * cell,
                        cell,
                        cell));
                }
            }
            if (selectedAction == ActionMode.Move && active != null)
            {
                int moveCost = MoveCostTo(active, targetX, targetY);
                if (CanStandAt(targetX, targetY)
                    && moveCost < UnreachableMoveCost
                    && moveCost <= state.Combat.MovePoints)
                {
                    IReadOnlyList<Vector2Int> path = ReachableMovePath(
                        active,
                        targetX,
                        targetY,
                        state.Combat.MovePoints);
                    if (path != null)
                    {
                        for (int i = 0; i < path.Count; i++)
                        {
                            Vector2Int point = path[i];
                            combatTooltipBlockers.Add(new Rect(
                                grid.x + point.x * cell,
                                grid.y + point.y * cell,
                                cell,
                                cell));
                        }
                    }
                }
            }
            Rect decision = new Rect(
                grid.x + targetX * cell,
                grid.y + targetY * cell,
                cell,
                cell);
            return CombatTargetingRules.PlaceBoardTooltip(
                grid,
                decision,
                pointer,
                width,
                height,
                combatTooltipBlockers);
        }

        private string CombatHoverTileLine(int x, int y, Point cover)
        {
            if (IsDisruptableRitual(cover)) return $"{RitualName(cover)}: opens in {Mathf.Max(1, cover.Duration)} round{(cover.Duration == 1 ? "" : "s")}; disrupt or seal it.";
            if (IsBreakableCover(cover))
            {
                int current = CoverIntegrity(cover);
                int maximum = CoverMaxIntegrity(cover.Kind);
                string duration = cover.Duration > 0
                    ? $"; withers in {cover.Duration} round{(cover.Duration == 1 ? "" : "s")}"
                    : "";
                return $"{CoverName(cover)}: {current}/{maximum} integrity; blocks movement/direct shots; arcs pass{duration}.";
            }
            if (cover != null)
            {
                string kind = string.IsNullOrEmpty(cover.Kind) ? "terrain" : char.ToUpperInvariant(cover.Kind[0]) + cover.Kind.Substring(1);
                return $"{kind}: {TerrainPreviewLine(cover).Trim()}";
            }
            int index = CombatTerrainTextureIndex(x, y, null);
            if (index == 0) return "Terrain: grass or overgrowth, normal movement.";
            if (index == 1) return "Terrain: snow, visual only for now.";
            if (index == 2 || index == 3) return "Terrain: dirt road, normal movement.";
            if (index == 9 || index == 11) return "Terrain: cave floor or rubble, normal movement.";
            if (index == 10) return "Terrain: sewer grates, normal movement.";
            if (index == 12) return "Terrain: ice texture. Spell-created ice adds move cost.";
            if (index == 14) return "Terrain: scorched ground. Spell fire is dangerous.";
            return "Terrain: old stone floor, normal movement.";
        }

        private Rect PlaceCombatTooltip(Vector2 mouse, float width, float height)
        {
            float rightLimit = CombatTooltipRightLimit();
            float bottomLimit = CombatTooltipBottomLimit();
            float x = mouse.x + 18f;
            if (x + width > rightLimit) x = mouse.x - width - 18f;
            float y = mouse.y + 14f;
            if (y + height > bottomLimit) y = mouse.y - height - 18f;
            return ClampTooltipRect(new Rect(x, y, width, height), rightLimit, bottomLimit);
        }

        private Rect PlaceActionTooltip(Rect source, float width, float height)
        {
            float rightLimit = CombatTooltipRightLimit();
            Rect box = new Rect(source.x, source.y - height - 12f, width, height);
            if (box.y < 86f) box.y = source.yMax + 8f;
            if (box.xMax > rightLimit) box.x = rightLimit - box.width;
            return ClampTooltipRect(box, rightLimit, Screen.height - 8f);
        }

        private Rect ClampTooltipRect(Rect rect, float rightLimit, float bottomLimit)
        {
            float left = 8f;
            float top = 8f;
            rightLimit = Mathf.Clamp(rightLimit, left + 80f, Screen.width - 8f);
            bottomLimit = Mathf.Clamp(bottomLimit, top + 60f, Screen.height - 8f);
            rect.width = Mathf.Min(rect.width, Mathf.Max(80f, rightLimit - left));
            rect.height = Mathf.Min(rect.height, Mathf.Max(60f, bottomLimit - top));
            rect.x = Mathf.Clamp(rect.x, left, Mathf.Max(left, rightLimit - rect.width));
            rect.y = Mathf.Clamp(rect.y, top, Mathf.Max(top, bottomLimit - rect.height));
            return rect;
        }

        private float CombatTooltipRightLimit()
        {
            Rect currentSide = SidePanelRect();
            return currentSide.width > 0f ? currentSide.x - 10f : Screen.width - 8f;
        }

        private float CombatTooltipBottomLimit()
        {
            float boardBottom = boardRect.height > 0f ? boardRect.yMax + 52f : Screen.height - 8f;
            return Mathf.Min(Screen.height - 96f, boardBottom);
        }

        private void DrawCombatTooltipBackplate(Rect rect, Color accent, int artIndex)
        {
            DrawRect(rect, Hex("080b0d", 0.97f));
            if (TryDrawCombatUiPanelAtlasIcon(Pad(rect, 1f), artIndex, Color.white.WithAlpha(0.17f)))
            {
                DrawRect(rect, Hex("030405", 0.28f));
            }
            DrawBorder(rect, accent, 1);
            DrawCombatUiCornerTrim(rect, accent);
        }

        private string HoverPreviewTitle(CombatUnit active, CombatUnit target, Point cover)
        {
            if (selectedAction == ActionMode.Move) return "Movement Preview";
            if (selectedAction == ActionMode.Attack && target != null) return target.Side == UnitSide.Enemy ? "Attack Preview" : "Friendly Unit";
            if (selectedAction == ActionMode.Attack && IsDisruptableRitual(cover)) return "Disrupt Ritual";
            if (selectedAction == ActionMode.Attack && IsBreakableCover(cover)) return "Break Cover";
            if (selectedAction == ActionMode.Cast)
            {
                FormulaDef formula = GetFormula(pendingFormulaCode);
                return formula == null ? "Choose Spell" : formula.Name;
            }
            if (selectedAction == ActionMode.Ability)
            {
                MartialAbility ability = AbilityDef(pendingAbilityId);
                return ability == null ? "Choose Skill" : ability.Name;
            }
            return "Combat Preview";
        }

        private Color HoverPreviewAccent(string text, CombatUnit target, Point cover)
        {
            string lowered = (text ?? "").ToLowerInvariant();
            if (lowered.Contains("blocked") || lowered.Contains("out of") || lowered.Contains("needs") || lowered.Contains("too far") || lowered.Contains("friendly")) return ember;
            if (target != null) return target.Side == UnitSide.Enemy ? blood : teal;
            if (IsDisruptableRitual(cover)) return ObstacleAccent(cover.Kind);
            if (IsBreakableCover(cover)) return gold;
            if (selectedAction == ActionMode.Cast) return violet;
            if (selectedAction == ActionMode.Ability) return gold;
            if (selectedAction == ActionMode.Move) return teal;
            return gold;
        }

        private int HoverPreviewHudIconIndex()
        {
            if (selectedAction == ActionMode.Move) return 0;
            if (selectedAction == ActionMode.Attack) return 8;
            if (selectedAction == ActionMode.Cast) return 2;
            if (selectedAction == ActionMode.Ability) return 8;
            return 10;
        }

        private void DrawPreviewChip(Rect rect, string text, Color accent)
        {
            DrawRect(rect, Hex("151b20", 0.92f));
            DrawBorder(rect, accent.WithAlpha(0.72f), 1);
            GUI.Label(new Rect(rect.x + 5, rect.y + 2, rect.width - 10, rect.height - 3), FitText(text, rect.width - 10, CenterStyle(9, cursorWhite)), CenterStyle(9, cursorWhite));
        }

        private string HoverClickInstruction(CombatUnit active, CombatUnit target, Point cover, int x, int y)
        {
            if (selectedAction == ActionMode.Move)
            {
                int moveCost = MoveCostTo(active, x, y);
                return CanStandAt(x, y) && moveCost < UnreachableMoveCost && moveCost <= state.Combat.MovePoints ? "Click to move" : "Cannot move there";
            }
            if (selectedAction == ActionMode.Attack)
            {
                if (!state.Combat.ActionAvailable) return "Action already used";
                if (target != null && target.Side == UnitSide.Enemy)
                {
                    CombatAttackForecast forecast = AttackForecast(active, target);
                    return forecast.Legal
                        ? "Click to attack"
                        : CombatThreatRules.BlockLabel(forecast.BlockReason);
                }
                if (IsDisruptableRitual(cover))
                {
                    return CanAttackCombatObstacle(active, cover)
                        ? "Click to disrupt ritual"
                        : CombatObstacleAttackBlockReason(active, cover);
                }
                if (IsBreakableCover(cover))
                {
                    return CanAttackCombatObstacle(active, cover)
                        ? "Click to break cover"
                        : CombatObstacleAttackBlockReason(active, cover);
                }
                return "Choose an enemy, ritual, or cover";
            }
            if (selectedAction == ActionMode.Cast)
            {
                FormulaDef formula = GetFormula(pendingFormulaCode);
                if (formula == null) return "Choose a spell card";
                return IsFormulaActionable(formula, active, target, x, y) ? "Click to cast" : "Cannot cast there";
            }
            if (selectedAction == ActionMode.Ability)
            {
                MartialAbility ability = AbilityDef(pendingAbilityId);
                string reason;
                if (ability == null) return "Choose a skill";
                return CanTargetAbility(active, ability, target, x, y, out reason) ? $"Click to use {ability.Name}" : reason;
            }
            return "Choose a command";
        }

        private bool CanAttackCombatObstacle(CombatUnit active, Point obstacle)
        {
            if (active == null || obstacle == null) return false;
            if (!IsDisruptableRitual(obstacle) && !IsBreakableCover(obstacle)) return false;
            int range = EffectiveAttackRangeTo(active, obstacle.X, obstacle.Y);
            if (Distance(active.X, active.Y, obstacle.X, obstacle.Y) > range) return false;
            bool ranged = UsesRangedAttackAt(active, active.X, active.Y, obstacle.X, obstacle.Y);
            return !ranged || HasLineOfSight(active.X, active.Y, obstacle.X, obstacle.Y, true);
        }

        private string CombatObstacleAttackBlockReason(CombatUnit active, Point obstacle)
        {
            if (active == null || obstacle == null) return "No valid target";
            int range = EffectiveAttackRangeTo(active, obstacle.X, obstacle.Y);
            if (Distance(active.X, active.Y, obstacle.X, obstacle.Y) > range) return "Out of range";
            bool ranged = UsesRangedAttackAt(active, active.X, active.Y, obstacle.X, obstacle.Y);
            if (ranged && !HasLineOfSight(active.X, active.Y, obstacle.X, obstacle.Y, true))
            {
                return "Line of sight blocked";
            }
            return "Cannot attack that";
        }

        private void DrawHoverAim(Rect grid, float cell, CombatUnit active)
        {
            if (active == null
                || !TryCombatHoverCell(grid, cell, out int x, out int y, out _))
            {
                return;
            }

            CombatUnit target = UnitAt(x, y);
            Rect tile = new Rect(grid.x + x * cell, grid.y + y * cell, cell, cell);
            Color color = gold;
            bool drawLine = false;

            if (selectedAction == ActionMode.Move)
            {
                int moveCost = MoveCostTo(active, x, y);
                bool reachable = CanStandAt(x, y) && moveCost < UnreachableMoveCost;
                bool valid = reachable && moveCost <= state.Combat.MovePoints;
                color = valid ? teal : Hex("8a5c35");
                if (valid)
                {
                    IReadOnlyList<Vector2Int> path = ReachableMovePath(active, x, y, state.Combat.MovePoints);
                    DrawMovementPathPreview(grid, cell, path, color);
                }
                DrawTargetBadge(tile, reachable ? moveCost.ToString() : "X", color, valid);
            }
            else if (selectedAction == ActionMode.Attack && target != null)
            {
                CombatAttackForecast forecast = AttackForecast(active, target);
                drawLine = target.Side != active.Side;
                bool invalid = !forecast.Legal;
                color = invalid ? Hex("8a5c35") : blood;
                string label = forecast.BlockReason == AttackForecastBlockReason.LineOfSight ? "BLOCK" : invalid ? "NO" : $"{forecast.HitChance}%";
                DrawTargetReticle(tile, color, label, !invalid);
            }
            else if (selectedAction == ActionMode.Attack)
            {
                Point cover = ObstacleAt(x, y);
                if (IsDisruptableRitual(cover))
                {
                    drawLine = true;
                    bool valid = CanAttackCombatObstacle(active, cover);
                    color = valid ? ObstacleAccent(cover.Kind) : Hex("8a5c35");
                    DrawTargetReticle(tile, color, valid ? "BREAK" : "NO", valid);
                }
                else if (IsBreakableCover(cover))
                {
                    drawLine = true;
                    bool valid = CanAttackCombatObstacle(active, cover);
                    color = valid ? gold : Hex("8a5c35");
                    DrawTargetReticle(tile, color, valid ? "BREAK" : "NO", valid);
                }
            }
            else if (selectedAction == ActionMode.Cast)
            {
                FormulaDef formula = GetFormula(pendingFormulaCode);
                bool actionable = formula != null && IsFormulaActionable(formula, active, target, x, y);
                // Formula footprint preview owns its path trace.
                drawLine = false;
                color = formula == null ? violet : actionable ? FormulaColor(formula) : Hex("8a5c35");
                if (formula != null)
                {
                    DrawFormulaAreaPreview(grid, cell, formula, active, target, x, y);
                    CombatPowerTargetingProfile profile = CombatPowerTargetingRules.ForFormula(formula);
                    string tag = profile.Kind == CombatPowerFootprintKind.Single
                        ? FormulaCanArcOverCover(formula, x, y) ? "ARC" : FormulaBaseRequiresLineOfSight(formula) ? "CAST" : FormulaPathLabel(formula).ToUpperInvariant()
                        : profile.BoardLabel;
                    DrawTargetReticle(tile, color, actionable ? tag : "NO", actionable);
                }
            }
            else if (selectedAction == ActionMode.Ability)
            {
                MartialAbility ability = AbilityDef(pendingAbilityId);
                string reason;
                bool actionable = ability != null && CanTargetAbility(active, ability, target, x, y, out reason);
                CombatPowerTargetingProfile profile = CombatPowerTargetingRules.ForAbility(ability);
                drawLine = actionable && profile.Kind != CombatPowerFootprintKind.ChargeLanding && profile.Kind != CombatPowerFootprintKind.CrossArea;
                color = actionable && ability != null
                    ? CombatPowerPresentationRules.AbilityAccent(ability.ClassKey).ToColor()
                    : Hex("8a5c35");
                if (ability != null) DrawAbilityFootprintPreview(grid, cell, ability, active, target, x, y, actionable);
                DrawTargetReticle(tile, color, actionable ? ability.Short : "NO", actionable);
            }

            if (selectedAction == ActionMode.Move) DrawBorder(Pad(tile, cell * 0.05f), color, 2);
            if (!drawLine) return;

            Vector2 from = new Vector2(grid.x + (active.X + 0.5f) * cell, grid.y + (active.Y + 0.5f) * cell);
            Vector2 to = new Vector2(grid.x + (x + 0.5f) * cell, grid.y + (y + 0.5f) * cell);
            DrawPixelLine(from, to, color.WithAlpha(0.62f), Mathf.Max(2f, cell * 0.035f));
            if (color == Hex("8a5c35"))
            {
                DrawBlockingCoverMarkers(grid, cell, active.X, active.Y, x, y);
            }
        }

        private void DrawMovementPathPreview(
            Rect grid,
            float cell,
            IReadOnlyList<Vector2Int> path,
            Color color)
        {
            if (path == null || path.Count < 2) return;
            float thickness = Mathf.Max(2f, cell * 0.025f);
            Color shadow = Hex("030405", 0.66f);
            Color trace = color.WithAlpha(0.62f);
            for (int i = 1; i < path.Count; i++)
            {
                Vector2Int previous = path[i - 1];
                Vector2Int current = path[i];
                Vector2 from = new Vector2(
                    grid.x + (previous.x + 0.5f) * cell,
                    grid.y + (previous.y + 0.5f) * cell);
                Vector2 to = new Vector2(
                    grid.x + (current.x + 0.5f) * cell,
                    grid.y + (current.y + 0.5f) * cell);
                DrawPixelLine(from, to, shadow, thickness + 3f);
                DrawPixelLine(from, to, trace, thickness);

                float dotSize = Mathf.Clamp(cell * 0.10f, 5f, 10f);
                Rect dot = new Rect(
                    to.x - dotSize * 0.5f,
                    to.y - dotSize * 0.5f,
                    dotSize,
                    dotSize);
                DrawRect(Pad(dot, -2f), shadow);
                DrawRect(dot, Color.Lerp(trace, cursorWhite, i == path.Count - 1 ? 0.30f : 0.12f));
            }
        }

        private void DrawBlockingCoverMarkers(Rect grid, float cell, int ax, int ay, int bx, int by)
        {
            foreach (Point cover in BlockingCoverAlongLine(ax, ay, bx, by))
            {
                Rect tile = new Rect(grid.x + cover.X * cell, grid.y + cover.Y * cell, cell, cell);
                Color blocked = Hex("8a5c35", 0.82f);
                DrawTargetBadge(tile, CoverIntegrity(cover).ToString(), blocked, false);
                DrawRect(new Rect(tile.xMax - Mathf.Max(2f, cell * 0.035f), tile.y + cell * 0.24f, Mathf.Max(2f, cell * 0.035f), cell * 0.52f), blocked);
            }
        }

        private void DrawTargetReticle(Rect tile, Color color, string label, bool valid)
        {
            DrawTargetStateShape(tile, color, valid);
            DrawTargetBadge(tile, label, color, valid);
        }

        private void DrawTargetStateShape(Rect tile, Color color, bool valid)
        {
            Rect ring = Pad(tile, tile.width * 0.13f);
            float stroke = Mathf.Max(2f, tile.width * 0.035f);
            if (!valid)
            {
                float inset = tile.width * 0.10f;
                Vector2 topLeft = new Vector2(ring.x + inset, ring.y + inset);
                Vector2 topRight = new Vector2(ring.xMax - inset, ring.y + inset);
                Vector2 bottomLeft = new Vector2(ring.x + inset, ring.yMax - inset);
                Vector2 bottomRight = new Vector2(ring.xMax - inset, ring.yMax - inset);
                DrawPixelLine(topLeft, bottomRight, Hex("030405", 0.72f), stroke + 3f);
                DrawPixelLine(topRight, bottomLeft, Hex("030405", 0.72f), stroke + 3f);
                DrawPixelLine(topLeft, bottomRight, color.WithAlpha(0.90f), stroke);
                DrawPixelLine(topRight, bottomLeft, color.WithAlpha(0.90f), stroke);
                return;
            }

            float corner = ring.width * 0.26f;
            DrawRect(new Rect(ring.x, ring.y, corner, stroke), color);
            DrawRect(new Rect(ring.x, ring.y, stroke, ring.height * 0.26f), color);
            DrawRect(new Rect(ring.xMax - corner, ring.y, corner, stroke), color);
            DrawRect(new Rect(ring.xMax - stroke, ring.y, stroke, ring.height * 0.26f), color);
            DrawRect(new Rect(ring.x, ring.yMax - stroke, corner, stroke), color);
            DrawRect(new Rect(ring.x, ring.yMax - ring.height * 0.26f, stroke, ring.height * 0.26f), color);
            DrawRect(new Rect(ring.xMax - corner, ring.yMax - stroke, corner, stroke), color);
            DrawRect(new Rect(ring.xMax - stroke, ring.yMax - ring.height * 0.26f, stroke, ring.height * 0.26f), color);
        }

        private void DrawTargetBadge(Rect tile, string label, Color color, bool valid)
        {
            if (string.IsNullOrEmpty(label)) return;
            float w = Mathf.Clamp(label.Length * 6f + 12f, 20f, tile.width * 0.62f);
            float h = Mathf.Clamp(tile.height * 0.18f, 13f, 16f);
            Rect badge = new Rect(tile.xMax - w - tile.width * 0.09f, tile.y + tile.height * 0.09f, w, h);
            DrawRect(badge, valid ? Hex("050708", 0.68f) : Hex("2b1714", 0.76f));
            DrawBorder(badge, color.WithAlpha(valid ? 0.72f : 0.86f), 1);
            GUI.Label(new Rect(badge.x + 2, badge.y, badge.width - 4, badge.height), label, CenterStyle(8, valid ? cursorWhite : Hex("d98b6a")));
        }

        private void DrawFormulaAreaPreview(Rect grid, float cell, FormulaDef formula, CombatUnit active, CombatUnit target, int x, int y)
        {
            if (formula == null || active == null) return;
            if (Distance(active.X, active.Y, x, y) > EffectiveFormulaRange(formula, active)) return;
            if (!CanTargetFormula(formula, active, target, x, y)) return;

            bool legal = HasFormulaLineOfSight(formula, active, x, y) && active.Mana >= EffectiveFormulaMana(formula, active);
            Color color = legal ? FormulaColor(formula, 0.72f) : Hex("8a5c35", 0.64f);
            CombatPowerTargetingProfile profile = CombatPowerTargetingRules.ForFormula(formula);
            if (profile.Kind == CombatPowerFootprintKind.Chain && target != null)
            {
                List<CombatUnit> chain = BuildLightningChain(target);
                CombatUnit previous = null;
                for (int i = 0; i < chain.Count; i++)
                {
                    CombatUnit node = chain[i];
                    Rect nodeTile = new Rect(grid.x + node.X * cell, grid.y + node.Y * cell, cell, cell);
                    DrawRect(Pad(nodeTile, cell * 0.18f), FormulaColor(formula, i == 0 ? 0.22f : 0.13f));
                    DrawBorder(Pad(nodeTile, cell * 0.14f), color.WithAlpha(i == 0 ? 0.90f : 0.68f), i == 0 ? 2 : 1);
                    DrawTargetBadge(nodeTile, (i + 1).ToString(), color, legal);
                    if (previous != null)
                    {
                        Vector2 from = new Vector2(grid.x + (previous.X + 0.5f) * cell, grid.y + (previous.Y + 0.5f) * cell);
                        Vector2 to = new Vector2(grid.x + (node.X + 0.5f) * cell, grid.y + (node.Y + 0.5f) * cell);
                        DrawJaggedPixelLine(from, to, color.WithAlpha(0.72f), Mathf.Max(2f, cell * 0.028f), cell * 0.07f);
                    }
                    previous = node;
                }
            }
            else if (profile.Kind == CombatPowerFootprintKind.RadiusArea)
            {
                foreach (Point point in RadiusPreviewTiles(x, y, LightningPowerRules.TempestRadius))
                {
                    Rect areaTile = new Rect(grid.x + point.X * cell, grid.y + point.Y * cell, cell, cell);
                    bool center = point.X == x && point.Y == y;
                    bool occupied = UnitAt(point.X, point.Y)?.Side == UnitSide.Enemy;
                    DrawRect(Pad(areaTile, cell * 0.18f), FormulaColor(formula, center ? 0.22f : occupied ? 0.14f : 0.08f));
                    DrawBorder(Pad(areaTile, cell * 0.16f), color.WithAlpha(center || occupied ? 0.76f : 0.42f), center ? 2 : 1);
                }
            }
            else if (formula.Code == "VST")
            {
                foreach (Point point in SplashPreviewTiles(x, y))
                {
                    Rect areaTile = new Rect(grid.x + point.X * cell, grid.y + point.Y * cell, cell, cell);
                    bool center = point.X == x && point.Y == y;
                    bool occupied = UnitAt(point.X, point.Y)?.Side == UnitSide.Enemy;
                    DrawRect(Pad(areaTile, cell * 0.19f), FormulaColor(formula, center ? 0.20f : occupied ? 0.14f : 0.06f));
                    DrawBorder(Pad(areaTile, cell * 0.17f), color.WithAlpha(center || occupied ? 0.74f : 0.36f), center ? 2 : 1);
                }
            }
            else if (formula.Splash)
            {
                foreach (Point point in SplashPreviewTiles(x, y))
                {
                    Rect tile = new Rect(grid.x + point.X * cell, grid.y + point.Y * cell, cell, cell);
                    DrawRect(Pad(tile, cell * 0.18f), FormulaColor(formula, point.X == x && point.Y == y ? 0.22f : 0.12f));
                    DrawBorder(Pad(tile, cell * 0.16f), color, point.X == x && point.Y == y ? 2 : 1);
                }
            }
            else if (formula.Effect == "terrain")
            {
                Rect tile = new Rect(grid.x + x * cell, grid.y + y * cell, cell, cell);
                DrawBorder(Pad(tile, cell * 0.16f), color, 2);
            }
            DrawFormulaAimTrace(grid, cell, active, formula, x, y, legal);
        }

        private IEnumerable<Point> RadiusPreviewTiles(int x, int y, int radius)
        {
            for (int offsetY = -radius; offsetY <= radius; offsetY++)
            for (int offsetX = -radius; offsetX <= radius; offsetX++)
            {
                if (Mathf.Abs(offsetX) + Mathf.Abs(offsetY) > radius) continue;
                int tileX = x + offsetX;
                int tileY = y + offsetY;
                if (tileX < 0 || tileX >= CombatW || tileY < 0 || tileY >= CombatH) continue;
                yield return new Point(tileX, tileY);
            }
        }

        private IEnumerable<Point> SplashPreviewTiles(int x, int y)
        {
            Point[] tiles =
            {
                new Point(x, y),
                new Point(x + 1, y),
                new Point(x - 1, y),
                new Point(x, y + 1),
                new Point(x, y - 1)
            };
            foreach (Point tile in tiles)
            {
                if (tile.X < 0 || tile.X >= CombatW || tile.Y < 0 || tile.Y >= CombatH) continue;
                yield return tile;
            }
        }

        private void DrawAbilityFootprintPreview(Rect grid, float cell, MartialAbility ability, CombatUnit active, CombatUnit target, int x, int y, bool legal)
        {
            if (ability == null || active == null || target == null) return;
            CombatPowerTargetingProfile profile = CombatPowerTargetingRules.ForAbility(ability);
            Color accent = legal
                ? CombatPowerPresentationRules.AbilityAccent(ability.ClassKey).ToColor()
                : Hex("8a5c35");

            if (profile.Kind == CombatPowerFootprintKind.CrossArea)
            {
                foreach (Point point in SplashPreviewTiles(x, y))
                {
                    Rect areaTile = new Rect(grid.x + point.X * cell, grid.y + point.Y * cell, cell, cell);
                    bool center = point.X == x && point.Y == y;
                    bool occupied = UnitAt(point.X, point.Y)?.Side == UnitSide.Enemy;
                    DrawRect(Pad(areaTile, cell * 0.18f), accent.WithAlpha(center ? 0.20f : occupied ? 0.15f : 0.09f));
                    DrawBorder(Pad(areaTile, cell * 0.16f), accent.WithAlpha(center || occupied ? 0.76f : 0.46f), center || occupied ? 2 : 1);
                }
                DrawAbilityArcTrace(grid, cell, active, x, y, accent, legal);
                return;
            }

            if (profile.Kind == CombatPowerFootprintKind.SecondaryStrike)
            {
                CombatUnit secondary = CleaveSecondaryTarget(active, target);
                if (secondary == null) return;
                Rect secondaryTile = new Rect(grid.x + secondary.X * cell, grid.y + secondary.Y * cell, cell, cell);
                DrawRect(Pad(secondaryTile, cell * 0.18f), accent.WithAlpha(0.14f));
                DrawBorder(Pad(secondaryTile, cell * 0.13f), accent.WithAlpha(0.82f), 2);
                DrawTargetBadge(secondaryTile, profile.BoardLabel, accent, legal);
                return;
            }

            if (profile.Kind != CombatPowerFootprintKind.ChargeLanding) return;
            Point landing = BestChargeLanding(active, target);
            if (landing == null) return;
            Rect landingTile = new Rect(grid.x + landing.X * cell, grid.y + landing.Y * cell, cell, cell);
            DrawRect(Pad(landingTile, cell * 0.16f), accent.WithAlpha(0.16f));
            DrawBorder(Pad(landingTile, cell * 0.10f), accent.WithAlpha(0.90f), 2);
            DrawTargetBadge(landingTile, profile.BoardLabel, accent, legal);
        }

        private void DrawAbilityArcTrace(Rect grid, float cell, CombatUnit active, int x, int y, Color color, bool legal)
        {
            if (active == null) return;
            Vector2 from = new Vector2(grid.x + (active.X + 0.5f) * cell, grid.y + (active.Y + 0.5f) * cell);
            Vector2 to = new Vector2(grid.x + (x + 0.5f) * cell, grid.y + (y + 0.5f) * cell);
            Vector2 mid = Vector2.Lerp(from, to, 0.5f) + new Vector2(0f, -cell * 0.30f);
            float thickness = Mathf.Max(2f, cell * 0.028f);
            Color trace = color.WithAlpha(legal ? 0.74f : 0.62f);
            DrawPixelLine(from, mid, trace, thickness);
            DrawPixelLine(mid, to, Color.Lerp(trace, cursorWhite, 0.18f), thickness);
            Rect crest = new Rect(mid.x - cell * 0.08f, mid.y - cell * 0.08f, cell * 0.16f, cell * 0.16f);
            DrawRect(crest, Hex("050708", 0.52f));
            DrawBorder(crest, trace, 1);
            DrawRect(new Rect(crest.x + crest.width * 0.18f, crest.y + crest.height * 0.58f, crest.width * 0.24f, crest.height * 0.10f), trace);
            DrawRect(new Rect(crest.x + crest.width * 0.40f, crest.y + crest.height * 0.34f, crest.width * 0.24f, crest.height * 0.10f), trace);
            DrawRect(new Rect(crest.x + crest.width * 0.62f, crest.y + crest.height * 0.58f, crest.width * 0.20f, crest.height * 0.10f), trace);
        }

        private void DrawFormulaAimTrace(Rect grid, float cell, CombatUnit active, FormulaDef formula, int x, int y, bool legal)
        {
            if (active == null || formula == null) return;
            Vector2 from = new Vector2(grid.x + (active.X + 0.5f) * cell, grid.y + (active.Y + 0.5f) * cell);
            Vector2 to = new Vector2(grid.x + (x + 0.5f) * cell, grid.y + (y + 0.5f) * cell);
            Color color = legal ? FormulaColor(formula, 0.72f) : Hex("8a5c35", 0.70f);
            float thickness = Mathf.Max(2f, cell * 0.028f);
            bool arcing = FormulaCanArcOverCover(formula, x, y);
            if (formula.DamageType == "shock" && formula.Target == "enemy")
            {
                DrawJaggedPixelLine(from, to, color, Mathf.Max(2f, cell * 0.034f), cell * 0.08f);
                DrawFormulaPathGlyph(new Rect(to.x - cell * 0.08f, to.y - cell * 0.08f, cell * 0.16f, cell * 0.16f), formula, color, true);
                return;
            }
            if (arcing)
            {
                Vector2 mid = Vector2.Lerp(from, to, 0.5f) + new Vector2(0f, -cell * 0.28f);
                DrawPixelLine(from, mid, color, thickness);
                DrawPixelLine(mid, to, Color.Lerp(color, cursorWhite, 0.20f), thickness);
                DrawFormulaPathGlyph(new Rect(mid.x - cell * 0.10f, mid.y - cell * 0.10f, cell * 0.20f, cell * 0.20f), formula, color, true);
            }
            else
            {
                DrawPixelLine(from, to, color, thickness);
                foreach (Point cover in BlockingCoverAlongLine(active.X, active.Y, x, y))
                {
                    Rect tile = new Rect(grid.x + cover.X * cell, grid.y + cover.Y * cell, cell, cell);
                    DrawBorder(Pad(tile, cell * 0.10f), Hex("8a5c35", 0.76f), 2);
                    DrawRect(new Rect(tile.center.x - cell * 0.04f, tile.y + cell * 0.24f, cell * 0.08f, cell * 0.52f), Hex("8a5c35", 0.62f));
                    DrawRect(new Rect(tile.x + cell * 0.24f, tile.center.y - cell * 0.04f, cell * 0.52f, cell * 0.08f), Hex("8a5c35", 0.62f));
                }
            }
            DrawFormulaPathGlyph(new Rect(to.x - cell * 0.08f, to.y - cell * 0.08f, cell * 0.16f, cell * 0.16f), formula, color, arcing);
        }

        private void DrawFormulaPathGlyph(Rect rect, FormulaDef formula, Color color, bool arcing)
        {
            DrawRect(rect, Hex("050708", 0.54f));
            DrawBorder(rect, color, 1);
            if (arcing)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.16f, rect.y + rect.height * 0.58f, rect.width * 0.24f, rect.height * 0.10f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.38f, rect.y + rect.height * 0.34f, rect.width * 0.24f, rect.height * 0.10f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.60f, rect.y + rect.height * 0.58f, rect.width * 0.24f, rect.height * 0.10f), color);
            }
            else if (FormulaBaseRequiresLineOfSight(formula))
            {
                DrawRect(new Rect(rect.x + rect.width * 0.14f, rect.y + rect.height * 0.44f, rect.width * 0.72f, rect.height * 0.12f), color);
                DrawRect(new Rect(rect.x + rect.width * 0.62f, rect.y + rect.height * 0.28f, rect.width * 0.18f, rect.height * 0.44f), color);
            }
            else
            {
                DrawPixelCross(Pad(rect, rect.width * 0.24f), color);
            }
        }

        private IEnumerable<Point> BlockingCoverAlongLine(int ax, int ay, int bx, int by)
        {
            int dx = bx - ax;
            int dy = by - ay;
            int steps = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
            if (steps <= 1) yield break;
            for (int i = 1; i < steps; i++)
            {
                float t = i / (float)steps;
                int x = Mathf.RoundToInt(ax + dx * t);
                int y = Mathf.RoundToInt(ay + dy * t);
                Point obstacle = ObstacleAt(x, y);
                if (IsBreakableCover(obstacle)) yield return obstacle;
            }
        }

        private void DrawCellFlash(Rect rect, int x, int y)
        {
            float now = Time.time;
            foreach (CellFlash flash in flashes)
            {
                if (flash.X != x || flash.Y != y) continue;
                if (now < flash.Start) continue;
                float t = Mathf.Clamp01((now - flash.Start) / flash.Duration);
                Color c = flash.Color.ToColor();
                c.a = (1f - t) * 0.42f;
                DrawRect(Pad(rect, rect.width * Mathf.Lerp(0.05f, 0.22f, t)), c);
                DrawBorder(Pad(rect, rect.width * 0.08f), Color.Lerp(c, cursorWhite, 0.25f), 1);
            }
        }

        private void DrawBeams(Rect grid, float cell)
        {
            float now = Time.time;
            foreach (BeamEffect beam in beams)
            {
                if (now < beam.Start) continue;
                float t = Mathf.Clamp01((now - beam.Start) / beam.Duration);
                Color c = beam.Color.ToColor();
                c.a = 1f - t * 0.55f;
                Vector2 from = new Vector2(grid.x + (beam.FromX + 0.5f) * cell, grid.y + (beam.FromY + 0.5f) * cell);
                Vector2 to = new Vector2(grid.x + (beam.ToX + 0.5f) * cell, grid.y + (beam.ToY + 0.5f) * cell);
                Vector2 head = Vector2.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
                if ((beam.Kind ?? "").StartsWith("weapon-", StringComparison.Ordinal))
                {
                    DrawWeaponContactMark(beam.Kind, from, to, cell, t, c);
                    continue;
                }
                if (beam.Kind == "fireball")
                {
                    DrawFireballProjectile(from, to, cell, t, c);
                    continue;
                }
                if (beam.Kind == "meteor" || beam.Kind == "meteor-small")
                {
                    DrawJaggedPixelLine(from, head, ember.WithAlpha(c.a), Mathf.Max(3f, cell * (beam.Kind == "meteor" ? 0.070f : 0.045f)), cell * 0.04f);
                    DrawPixelLine(from + new Vector2(-cell * 0.10f, 0f), head, gold.WithAlpha(c.a * 0.55f), Mathf.Max(2f, cell * 0.030f));
                    float size = beam.Kind == "meteor" ? 0.26f : 0.16f;
                    DrawRect(new Rect(head.x - cell * size * 0.5f, head.y - cell * size * 0.5f, cell * size, cell * size), Color.Lerp(ember, cursorWhite, 0.20f).WithAlpha(c.a));
                    continue;
                }
                if (beam.Kind == "lightning")
                {
                    Vector2 direction = (head - from).normalized;
                    if (direction.sqrMagnitude < 0.01f) direction = Vector2.right;
                    Vector2 side = new Vector2(-direction.y, direction.x);
                    Color outer = Color.Lerp(c, gold, 0.30f).WithAlpha(c.a * 0.88f);
                    Color core = Hex("d6f4ff", c.a);
                    DrawJaggedPixelLine(from, head, outer, Mathf.Max(3f, cell * 0.070f), cell * 0.105f);
                    DrawJaggedPixelLine(from, head, core, Mathf.Max(1f, cell * 0.026f), cell * 0.050f);

                    int polarity = ((beam.FromX * 31 + beam.FromY * 17 + beam.ToX * 13 + beam.ToY * 7) & 1) == 0 ? 1 : -1;
                    Vector2 branchOne = Vector2.Lerp(from, head, 0.43f);
                    Vector2 branchTwo = Vector2.Lerp(from, head, 0.68f);
                    DrawJaggedPixelLine(
                        branchOne,
                        branchOne + (side * polarity - direction * 0.18f) * cell * 0.28f,
                        outer.WithAlpha(c.a * 0.68f),
                        Mathf.Max(1f, cell * 0.024f),
                        cell * 0.032f);
                    DrawJaggedPixelLine(
                        branchTwo,
                        branchTwo + (-side * polarity - direction * 0.12f) * cell * 0.22f,
                        core.WithAlpha(c.a * 0.58f),
                        Mathf.Max(1f, cell * 0.018f),
                        cell * 0.026f);
                    DrawPixelCross(
                        new Rect(head.x - cell * 0.070f, head.y - cell * 0.070f, cell * 0.14f, cell * 0.14f),
                        core);
                    continue;
                }
                if (beam.Kind == "arc")
                {
                    Vector2 mid = Vector2.Lerp(from, head, 0.5f) + new Vector2(0f, -cell * 0.18f * Mathf.Sin(t * Mathf.PI));
                    DrawPixelLine(from, mid, c, Mathf.Max(2f, cell * 0.035f));
                    DrawPixelLine(mid, head, Color.Lerp(c, cursorWhite, 0.28f), Mathf.Max(2f, cell * 0.030f));
                    DrawRect(new Rect(head.x - cell * 0.05f, head.y - cell * 0.05f, cell * 0.10f, cell * 0.10f), c);
                    continue;
                }
                if (beam.Kind == "death" || beam.Kind == "hex")
                {
                    DrawJaggedPixelLine(from, head, c, Mathf.Max(2f, cell * 0.035f), cell * 0.09f);
                    DrawRect(new Rect(head.x - cell * 0.07f, head.y - cell * 0.07f, cell * 0.14f, cell * 0.14f), Color.Lerp(c, retroBlack, 0.22f));
                    continue;
                }
                DrawPixelLine(from, head, c, Mathf.Max(beam.Kind == "shot" ? 2f : 3f, cell * (beam.Kind == "shot" ? 0.030f : 0.055f)));
                if (beam.Kind == "shot")
                {
                    Vector2 dir = (head - from).normalized;
                    if (dir.sqrMagnitude < 0.01f) dir = Vector2.right;
                    Vector2 side = new Vector2(-dir.y, dir.x);
                    Vector2 tail = head - dir * cell * 0.12f;
                    DrawPixelLine(head, tail + side * cell * 0.06f, c, Mathf.Max(2f, cell * 0.025f));
                    DrawPixelLine(head, tail - side * cell * 0.06f, c, Mathf.Max(2f, cell * 0.025f));
                }
                else if (beam.Kind == "heal")
                {
                    DrawPixelCross(new Rect(head.x - cell * 0.07f, head.y - cell * 0.07f, cell * 0.14f, cell * 0.14f), Color.Lerp(c, cursorWhite, 0.20f));
                    DrawBorder(new Rect(head.x - cell * 0.11f, head.y - cell * 0.11f, cell * 0.22f, cell * 0.22f), c.WithAlpha(c.a * 0.70f), 1);
                }
                else if (beam.Kind == "fire")
                {
                    DrawRect(new Rect(head.x - cell * 0.07f, head.y - cell * 0.03f, cell * 0.14f, cell * 0.16f), ember.WithAlpha(c.a));
                    DrawRect(new Rect(head.x - cell * 0.035f, head.y - cell * 0.12f, cell * 0.07f, cell * 0.19f), gold.WithAlpha(c.a));
                }
                else if (beam.Kind == "ice")
                {
                    DrawRect(new Rect(head.x - cell * 0.11f, head.y - cell * 0.025f, cell * 0.22f, cell * 0.05f), frost.WithAlpha(c.a));
                    DrawRect(new Rect(head.x - cell * 0.025f, head.y - cell * 0.11f, cell * 0.05f, cell * 0.22f), Hex("d6f4ff", c.a));
                }
                else if (beam.Kind == "spell")
                {
                    DrawRect(new Rect(head.x - cell * 0.08f, head.y - cell * 0.08f, cell * 0.16f, cell * 0.16f), c);
                    DrawRect(new Rect(head.x - cell * 0.035f, head.y - cell * 0.16f, cell * 0.07f, cell * 0.32f), Color.Lerp(c, cursorWhite, 0.35f));
                }
            }
        }

        private void DrawFireballProjectile(Vector2 from, Vector2 to, float cell, float progress, Color tint)
        {
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(progress));
            Vector2 head = ProjectileArcPoint(from, to, t, cell, "fireball");
            float trailStart = Mathf.Max(0f, t - Mathf.Lerp(0.16f, 0.32f, t));
            int trailSegments = 7;
            Vector2 previous = ProjectileArcPoint(from, to, trailStart, cell, "fireball");
            for (int i = 1; i <= trailSegments; i++)
            {
                float sample = Mathf.Lerp(trailStart, t, i / (float)trailSegments);
                Vector2 next = ProjectileArcPoint(from, to, sample, cell, "fireball");
                float segmentFade = i / (float)trailSegments;
                float width = Mathf.Max(2f, cell * Mathf.Lerp(0.030f, 0.105f, segmentFade));
                DrawPixelLine(previous, next, ember.WithAlpha(Mathf.Lerp(0.20f, 0.82f, segmentFade)), width);
                DrawPixelLine(previous, next, gold.WithAlpha(Mathf.Lerp(0.08f, 0.68f, segmentFade)), Mathf.Max(1f, width * 0.46f));
                previous = next;
            }

            for (int i = 1; i <= 3; i++)
            {
                float echoT = Mathf.Max(0f, t - i * 0.055f);
                Vector2 echo = ProjectileArcPoint(from, to, echoT, cell, "fireball");
                float size = cell * (0.10f - i * 0.016f);
                DrawRect(new Rect(echo.x - size * 0.5f, echo.y - size * 0.5f, size, size), Color.Lerp(ember, gold, 0.35f).WithAlpha(0.54f - i * 0.11f));
            }

            float flicker = 0.86f + Mathf.Sin((t * 19f + from.x * 0.013f + to.y * 0.017f) * Mathf.PI) * 0.08f;
            int atlasCell = CombatPowerVisualRules.ProjectileAtlasCell("fireball", t);
            float artSize = cell * (atlasCell == 1 ? 0.82f : 0.68f) * flicker;
            Rect art = new Rect(head.x - artSize * 0.5f, head.y - artSize * 0.5f, artSize, artSize);
            Vector2 tangent = ProjectileArcPoint(from, to, Mathf.Min(1f, t + 0.025f), cell, "fireball")
                - ProjectileArcPoint(from, to, Mathf.Max(0f, t - 0.025f), cell, "fireball");
            float angle = tangent.sqrMagnitude < 0.001f ? 0f : Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
            Matrix4x4 oldMatrix = GUI.matrix;
            bool drewArt = false;
            try
            {
                if (atlasCell == 1) GUIUtility.RotateAroundPivot(angle - 135f, head);
                drewArt = TryDrawSpellAnimationAtlasIcon(art, atlasCell, Color.white.WithAlpha(Mathf.Max(0.76f, tint.a)));
            }
            finally
            {
                GUI.matrix = oldMatrix;
            }

            if (!drewArt)
            {
                DrawRect(new Rect(head.x - cell * 0.15f, head.y - cell * 0.15f, cell * 0.30f, cell * 0.30f), ember.WithAlpha(Mathf.Max(0.78f, tint.a)));
                DrawRect(new Rect(head.x - cell * 0.07f, head.y - cell * 0.07f, cell * 0.14f, cell * 0.14f), cursorWhite.WithAlpha(0.94f));
            }

            float halo = cell * (0.34f + Mathf.Sin(t * Mathf.PI * 7f) * 0.025f);
            DrawBorder(new Rect(head.x - halo * 0.5f, head.y - halo * 0.5f, halo, halo), gold.WithAlpha(0.54f), Mathf.Max(1, Mathf.RoundToInt(cell * 0.018f)));
        }

        private Vector2 ProjectileArcPoint(Vector2 from, Vector2 to, float progress, float cell, string kind)
        {
            float t = Mathf.Clamp01(progress);
            Vector2 point = Vector2.Lerp(from, to, t);
            point.y -= cell * CombatPowerVisualRules.ProjectileArcHeight(kind) * Mathf.Sin(t * Mathf.PI);
            return point;
        }

        private void DrawWeaponContactMark(string kind, Vector2 from, Vector2 to, float cell, float progress, Color color)
        {
            Vector2 direction = (to - from).normalized;
            if (direction.sqrMagnitude < 0.01f) direction = Vector2.right;
            Vector2 side = new Vector2(-direction.y, direction.x);
            float reveal = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(progress * 3.2f));
            float fade = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.42f, 1f, progress));
            float thickness = Mathf.Max(2f, cell * 0.034f);
            Color bright = Color.Lerp(color, cursorWhite, 0.32f).WithAlpha(color.a * fade);
            Color core = color.WithAlpha(color.a * fade * 0.88f);

            if (kind == "weapon-miss")
            {
                Vector2 offset = side * cell * 0.22f;
                float length = cell * 0.24f * reveal;
                DrawPixelLine(to + offset - direction * length, to + offset + direction * length, bright.WithAlpha(fade * 0.58f), thickness);
                DrawPixelLine(to - offset * 0.35f - direction * length * 0.55f, to - offset * 0.35f + direction * length * 0.55f, core.WithAlpha(fade * 0.42f), Mathf.Max(1f, thickness - 1f));
                return;
            }

            if (kind == "weapon-guard")
            {
                float halfW = cell * 0.20f * reveal;
                float halfH = cell * 0.25f * reveal;
                Vector2 top = to - Vector2.up * halfH;
                Vector2 right = to + Vector2.right * halfW;
                Vector2 bottom = to + Vector2.up * halfH;
                Vector2 left = to - Vector2.right * halfW;
                DrawPixelLine(top, right, bright, thickness);
                DrawPixelLine(right, bottom, core, thickness);
                DrawPixelLine(bottom, left, core, thickness);
                DrawPixelLine(left, top, bright, thickness);
                DrawPixelLine(to - side * cell * 0.10f, to + side * cell * 0.10f, bright.WithAlpha(fade * 0.70f), Mathf.Max(1f, thickness - 1f));
                return;
            }

            if (kind == "weapon-defeat")
            {
                float length = cell * 0.30f * reveal;
                Vector2 diagonal = (Vector2.right + Vector2.up).normalized;
                Vector2 counter = (Vector2.right - Vector2.up).normalized;
                DrawPixelLine(to - diagonal * length, to + diagonal * length, bright, thickness + 1f);
                DrawPixelLine(to - counter * length, to + counter * length, core, thickness);
                DrawPixelLine(to + Vector2.up * cell * 0.22f, to + Vector2.up * cell * 0.34f * reveal, bright.WithAlpha(fade * 0.62f), Mathf.Max(1f, thickness - 1f));
                return;
            }

            if (kind == "weapon-splinter")
            {
                float length = cell * 0.30f * reveal;
                DrawPixelLine(to - direction * cell * 0.04f, to + direction * length, bright, thickness);
                DrawPixelLine(to, to + (direction + side * 0.82f).normalized * length * 0.88f, core, Mathf.Max(1f, thickness - 1f));
                DrawPixelLine(to, to + (direction - side * 0.82f).normalized * length * 0.72f, bright.WithAlpha(fade * 0.70f), Mathf.Max(1f, thickness - 1f));
                return;
            }

            if (kind == "weapon-rubble")
            {
                float spread = cell * 0.22f * reveal;
                DrawPixelLine(to - side * spread, to + direction * spread * 0.70f, bright, thickness + 1f);
                DrawPixelLine(to + side * spread, to + direction * spread * 0.46f, core, thickness);
                float chip = Mathf.Max(3f, cell * 0.07f * reveal);
                DrawRect(new Rect(to.x - spread - chip * 0.5f, to.y + spread * 0.28f, chip, chip), bright.WithAlpha(fade * 0.76f));
                DrawRect(new Rect(to.x + spread * 0.62f, to.y - spread * 0.34f, chip * 0.78f, chip * 0.78f), core.WithAlpha(fade * 0.68f));
                return;
            }

            if (kind == "weapon-arrow")
            {
                float length = cell * 0.34f * reveal;
                Vector2 start = to - direction * length;
                Vector2 tip = to + direction * cell * 0.08f * reveal;
                DrawPixelLine(start, tip, bright, thickness);
                DrawPixelLine(start, start + direction * cell * 0.11f + side * cell * 0.09f, core, Mathf.Max(1f, thickness - 1f));
                DrawPixelLine(start, start + direction * cell * 0.11f - side * cell * 0.09f, core, Mathf.Max(1f, thickness - 1f));
                return;
            }

            if (kind == "weapon-thrust")
            {
                float length = cell * 0.42f * reveal;
                Vector2 tip = to + direction * cell * 0.12f * reveal;
                DrawPixelLine(to - direction * length, tip, bright, thickness);
                DrawPixelLine(tip - direction * cell * 0.08f + side * cell * 0.07f, tip, core, Mathf.Max(1f, thickness - 1f));
                DrawPixelLine(tip - direction * cell * 0.08f - side * cell * 0.07f, tip, core, Mathf.Max(1f, thickness - 1f));
                return;
            }

            if (kind == "weapon-heavy")
            {
                float crack = cell * 0.30f * reveal;
                DrawPixelLine(to - direction * cell * 0.12f, to + side * crack + direction * crack * 0.45f, bright, thickness + 1f);
                DrawPixelLine(to - direction * cell * 0.12f, to - side * crack + direction * crack * 0.52f, core, thickness);
                DrawPixelLine(to, to + direction * cell * 0.24f * reveal, bright.WithAlpha(fade * 0.74f), thickness);
                return;
            }

            Vector2 slashDirection = (side + direction * 0.34f).normalized;
            float slashLength = cell * 0.36f * reveal;
            DrawPixelLine(to - slashDirection * slashLength, to + slashDirection * slashLength, bright, thickness);
            Vector2 echoOffset = direction * cell * 0.10f;
            DrawPixelLine(to - slashDirection * slashLength * 0.62f + echoOffset, to + slashDirection * slashLength * 0.62f + echoOffset, core, Mathf.Max(1f, thickness - 1f));
        }

        private void DrawJaggedPixelLine(Vector2 from, Vector2 to, Color color, float thickness, float amplitude)
        {
            Vector2 delta = to - from;
            int segments = Mathf.Max(2, Mathf.CeilToInt(delta.magnitude / Mathf.Max(8f, amplitude)));
            Vector2 prev = from;
            Vector2 normal = delta.sqrMagnitude < 0.001f ? Vector2.up : new Vector2(-delta.y, delta.x).normalized;
            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector2 next = Vector2.Lerp(from, to, t);
                if (i < segments) next += normal * (((i & 1) == 0 ? -1f : 1f) * amplitude);
                DrawPixelLine(prev, next, color, thickness);
                prev = next;
            }
        }

        private void DrawCastGlyphs(Rect grid, float cell)
        {
            float now = Time.time;
            foreach (CastGlyph glyph in castGlyphs)
            {
                if (now < glyph.Start) continue;
                float t = Mathf.Clamp01((now - glyph.Start) / glyph.Duration);
                Color c = glyph.Color.ToColor();
                c.a = 1f - t * 0.35f;
                Rect tile = new Rect(grid.x + glyph.X * cell, grid.y + glyph.Y * cell, cell, cell);
                Rect ring = Pad(tile, cell * Mathf.Lerp(0.24f, 0.06f, t));
                if (!string.IsNullOrEmpty(glyph.Kind) && glyph.Kind.StartsWith("ranger:", StringComparison.OrdinalIgnoreCase))
                {
                    int index = 0;
                    int.TryParse(glyph.Kind.Substring("ranger:".Length), out index);
                    if (TryDrawRangerAbilityEffectAtlasIcon(Pad(tile, cell * Mathf.Lerp(0.22f, 0.12f, t)), index, Color.white.WithAlpha(c.a)))
                    {
                        DrawBorder(Pad(tile, cell * Mathf.Lerp(0.34f, 0.06f, t)), c.WithAlpha(c.a * 0.86f), 2);
                        continue;
                    }
                    DrawBorder(ring, Color.Lerp(c, gold, 0.20f), 2);
                    DrawPixelLine(new Vector2(tile.x + cell * 0.20f, tile.center.y), new Vector2(tile.xMax - cell * 0.18f, tile.center.y - cell * 0.12f), c, Mathf.Max(2f, cell * 0.04f));
                    continue;
                }
                if (!string.IsNullOrEmpty(glyph.Kind) && glyph.Kind.StartsWith("ability:", StringComparison.OrdinalIgnoreCase))
                {
                    int index = 0;
                    int.TryParse(glyph.Kind.Substring("ability:".Length), out index);
                    Rect stamp = Pad(tile, cell * Mathf.Lerp(0.24f, 0.10f, Mathf.Min(1f, t * 1.6f)));
                    if (TryDrawAbilityIconAtlasIcon(stamp, index, Color.white.WithAlpha(c.a * 0.88f)))
                    {
                        DrawBorder(Pad(tile, cell * Mathf.Lerp(0.38f, 0.07f, t)), c.WithAlpha(c.a * 0.84f), 2);
                        DrawImpactBrackets(Pad(tile, cell * Mathf.Lerp(0.28f, 0.14f, t)), Color.Lerp(c, cursorWhite, 0.28f).WithAlpha(c.a * 0.72f), Mathf.Max(2f, cell * 0.026f));
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(glyph.Kind) && glyph.Kind.StartsWith("spellanim:", StringComparison.OrdinalIgnoreCase))
                {
                    int index = 0;
                    int.TryParse(glyph.Kind.Substring("spellanim:".Length), out index);
                    int stagedIndex = index == 0 && t >= 0.58f ? 2 : index;
                    float chargePulse = index == 0 ? Mathf.Sin(Mathf.Min(1f, t / 0.58f) * Mathf.PI) * 0.06f : 0f;
                    if (TryDrawSpellAnimationAtlasIcon(Pad(tile, cell * (Mathf.Lerp(0.08f, -0.05f, t) - chargePulse)), stagedIndex, Color.white.WithAlpha(c.a)))
                    {
                        DrawBorder(Pad(tile, cell * Mathf.Lerp(0.36f, 0.07f, t)), c.WithAlpha(c.a * 0.78f), 2);
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(glyph.Kind) && glyph.Kind.StartsWith("signature:", StringComparison.OrdinalIgnoreCase))
                {
                    int index = 0;
                    int.TryParse(glyph.Kind.Substring("signature:".Length), out index);
                    if (TryDrawSignatureSpellIconAtlasIcon(Pad(tile, cell * Mathf.Lerp(0.26f, 0.16f, t)), index, Color.white.WithAlpha(c.a)))
                    {
                        DrawBorder(Pad(tile, cell * Mathf.Lerp(0.38f, 0.07f, t)), c.WithAlpha(c.a * 0.82f), 2);
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(glyph.Kind) && glyph.Kind.StartsWith("magicui:", StringComparison.OrdinalIgnoreCase))
                {
                    int index = 0;
                    int.TryParse(glyph.Kind.Substring("magicui:".Length), out index);
                    if (TryDrawMagicUiAtlasIcon(Pad(tile, cell * Mathf.Lerp(0.26f, 0.16f, t)), index, Color.white.WithAlpha(c.a)))
                    {
                        DrawBorder(Pad(tile, cell * Mathf.Lerp(0.36f, 0.08f, t)), c.WithAlpha(c.a * 0.78f), 2);
                        continue;
                    }
                }
                if (glyph.Kind == "impact")
                {
                    DrawRect(new Rect(tile.x + cell * 0.16f, tile.center.y - cell * 0.025f, cell * 0.68f, cell * 0.05f), Color.Lerp(c, cursorWhite, 0.22f));
                    DrawRect(new Rect(tile.center.x - cell * 0.025f, tile.y + cell * 0.16f, cell * 0.05f, cell * 0.68f), Color.Lerp(c, cursorWhite, 0.22f));
                    DrawBorder(Pad(tile, cell * Mathf.Lerp(0.34f, 0.12f, t)), c, 1);
                    continue;
                }
                if (glyph.Kind == "area")
                {
                    DrawBorder(Pad(tile, cell * Mathf.Lerp(0.42f, 0.08f, t)), c, 2);
                    DrawRect(new Rect(tile.x + cell * 0.26f, tile.y + cell * 0.26f, cell * 0.48f, cell * 0.07f), c.WithAlpha(c.a * 0.70f));
                    DrawRect(new Rect(tile.x + cell * 0.26f, tile.y + cell * 0.66f, cell * 0.48f, cell * 0.07f), c.WithAlpha(c.a * 0.70f));
                    continue;
                }
                if (glyph.Kind == "fireball")
                {
                    int spellFrame = t < 0.68f ? 3 : 4;
                    int layeredFrame = t < 0.68f ? 1 : 14;
                    bool drewLayer = TryDrawEpicSpellEffectsAtlasIcon(
                        Pad(tile, cell * Mathf.Lerp(0.03f, -0.18f, Mathf.Min(1f, t * 1.8f))),
                        layeredFrame,
                        Color.white.WithAlpha(c.a * (t < 0.68f ? 0.78f : 0.42f)));
                    bool drewSpell = TryDrawSpellAnimationAtlasIcon(
                        Pad(tile, cell * Mathf.Lerp(0.06f, -0.10f, Mathf.Min(1f, t * 2.2f))),
                        spellFrame,
                        Color.white.WithAlpha(c.a));
                    if (drewLayer || drewSpell)
                    {
                        DrawBorder(Pad(tile, cell * Mathf.Lerp(0.34f, 0.05f, t)), ember.WithAlpha(c.a * 0.80f), 2);
                        continue;
                    }
                    DrawBorder(Pad(tile, cell * Mathf.Lerp(0.32f, 0.05f, t)), ember.WithAlpha(c.a), 2);
                    DrawRect(new Rect(tile.center.x - cell * 0.16f, tile.center.y - cell * 0.03f, cell * 0.32f, cell * 0.06f), gold.WithAlpha(c.a));
                    DrawRect(new Rect(tile.center.x - cell * 0.03f, tile.center.y - cell * 0.16f, cell * 0.06f, cell * 0.32f), gold.WithAlpha(c.a));
                    DrawRect(Pad(tile, cell * Mathf.Lerp(0.44f, 0.30f, t)), cursorWhite.WithAlpha(c.a * 0.22f));
                    continue;
                }
                if (glyph.Kind == "meteor")
                {
                    int spellFrame = t < 0.68f ? 6 : 7;
                    int layeredFrame = t < 0.68f ? 3 : 14;
                    bool drewLayer = TryDrawEpicSpellEffectsAtlasIcon(
                        Pad(tile, cell * Mathf.Lerp(-0.02f, -0.16f, Mathf.Min(1f, t * 1.8f))),
                        layeredFrame,
                        Color.white.WithAlpha(c.a * (t < 0.68f ? 0.72f : 0.36f)));
                    bool drewSpell = TryDrawSpellAnimationAtlasIcon(
                        Pad(tile, cell * Mathf.Lerp(0.02f, -0.10f, Mathf.Min(1f, t * 2.0f))),
                        spellFrame,
                        Color.white.WithAlpha(c.a));
                    if (drewLayer || drewSpell)
                    {
                        DrawBorder(Pad(tile, cell * Mathf.Lerp(0.44f, 0.06f, t)), Color.Lerp(ember, gold, 0.22f).WithAlpha(c.a), 3);
                        continue;
                    }
                    DrawBorder(Pad(tile, cell * Mathf.Lerp(0.44f, 0.06f, t)), Color.Lerp(ember, gold, 0.22f).WithAlpha(c.a), 3);
                    DrawRect(new Rect(tile.x + cell * 0.22f, tile.y + cell * 0.66f, cell * 0.56f, cell * 0.08f), ember.WithAlpha(c.a));
                    DrawRect(new Rect(tile.x + cell * 0.32f, tile.y + cell * 0.50f, cell * 0.36f, cell * 0.08f), gold.WithAlpha(c.a));
                    DrawRect(new Rect(tile.center.x - cell * 0.04f, tile.y + cell * 0.16f, cell * 0.08f, cell * 0.68f), cursorWhite.WithAlpha(c.a * 0.46f));
                    continue;
                }
                if (glyph.Kind == "status")
                {
                    DrawBorder(Pad(tile, cell * Mathf.Lerp(0.36f, 0.10f, t)), c, 2);
                    DrawRect(new Rect(tile.x + cell * 0.30f, tile.y + cell * 0.20f, cell * 0.40f, cell * 0.08f), c.WithAlpha(c.a * 0.76f));
                    DrawRect(new Rect(tile.x + cell * 0.30f, tile.y + cell * 0.72f, cell * 0.40f, cell * 0.08f), c.WithAlpha(c.a * 0.76f));
                    DrawRect(new Rect(tile.x + cell * 0.20f, tile.y + cell * 0.30f, cell * 0.08f, cell * 0.40f), c.WithAlpha(c.a * 0.76f));
                    DrawRect(new Rect(tile.x + cell * 0.72f, tile.y + cell * 0.30f, cell * 0.08f, cell * 0.40f), c.WithAlpha(c.a * 0.76f));
                    continue;
                }
                if (glyph.Kind == "priest")
                {
                    DrawBorder(ring, Color.Lerp(teal, cursorWhite, 0.25f), 2);
                    DrawRect(new Rect(tile.center.x - cell * 0.035f, tile.y + cell * 0.16f, cell * 0.07f, cell * 0.62f), c);
                    DrawRect(new Rect(tile.x + cell * 0.22f, tile.center.y - cell * 0.035f, cell * 0.56f, cell * 0.07f), c);
                    DrawRect(new Rect(tile.x + cell * 0.33f, tile.y + cell * 0.28f, cell * 0.34f, cell * 0.08f), Hex("d6f4ff", c.a));
                    DrawRect(new Rect(tile.x + cell * 0.33f, tile.y + cell * 0.64f, cell * 0.34f, cell * 0.08f), Hex("97dbc2", c.a));
                }
                else
                {
                    DrawBorder(ring, Color.Lerp(c, gold, 0.25f), 2);
                    DrawRect(new Rect(tile.x + cell * 0.18f, tile.y + cell * 0.30f, cell * 0.64f, cell * 0.07f), c);
                    DrawRect(new Rect(tile.x + cell * 0.28f, tile.y + cell * 0.58f, cell * 0.54f, cell * 0.07f), Color.Lerp(c, cursorWhite, 0.18f));
                    DrawRect(new Rect(tile.x + cell * 0.28f, tile.y + cell * 0.30f, cell * 0.07f, cell * 0.35f), c);
                    DrawRect(new Rect(tile.x + cell * 0.64f, tile.y + cell * 0.30f, cell * 0.07f, cell * 0.35f), c);
                    DrawRect(new Rect(tile.x + cell * 0.43f, tile.y + cell * 0.18f, cell * 0.14f, cell * 0.14f), Color.Lerp(c, cursorWhite, 0.35f));
                }
            }
        }

        private void DrawPixelLine(Vector2 from, Vector2 to, Color color, float thickness)
        {
            Vector2 delta = to - from;
            int steps = Mathf.Max(1, Mathf.CeilToInt(delta.magnitude / Mathf.Max(2f, thickness)));
            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;
                Vector2 p = Vector2.Lerp(from, to, t);
                DrawRect(new Rect(p.x - thickness * 0.5f, p.y - thickness * 0.5f, thickness, thickness), color);
            }
        }

        private string ActiveReadoutDetail(CombatUnit active)
        {
            string budget = $"Move {state.Combat.MovePoints}";
            if (active != null && active.Summoned)
            {
                string turns = active.SummonTurns > 0 ? $" / bound {active.SummonTurns}t" : "";
                return $"{budget} / {ActiveWeaponLabel(active)}{turns}";
            }
            FormulaDef spell = GetFormula(pendingFormulaCode);
            if (spell != null) return $"{budget} / {spell.Name}";
            MartialAbility ability = AbilityDef(pendingAbilityId);
            if (ability != null) return $"{budget} / {ability.Name}";
            if (selectedAction == ActionMode.Cast && !string.IsNullOrEmpty(active.Spell)) return $"{budget} / choose spell";
            if (selectedAction == ActionMode.Ability && HasMartialAbilities(active)) return $"{budget} / choose skill";
            if (selectedAction == ActionMode.Move) return $"{budget} / move by distance";
            if (selectedAction == ActionMode.Guard) return $"{budget} / braced guard";
            if (selectedAction == ActionMode.Elixir) return $"{budget} / elixir ready";
            if (selectedAction == ActionMode.Wait) return $"{budget} / end turn";
            return $"{budget} / {ActiveWeaponLabel(active)} / {ArmorLabel(active)}";
        }

        private string ActiveCommandPrompt(CombatUnit active)
        {
            if (IsCombatResolutionPending())
            {
                return string.IsNullOrWhiteSpace(combatResolutionLabel)
                    ? "Power resolving..."
                    : combatResolutionLabel + " resolving...";
            }
            if (active == null || active.Side != UnitSide.Party) return "Enemy turn resolving.";
            if (active.Summoned) return $"{active.Name} is a controlled pact ally. Move, attack, guard, or press Space to end.";
            if (!state.Combat.ActionAvailable && state.Combat.MovePoints <= 0) return "Turn spent. Press Space or End Turn.";
            if (!state.Combat.ActionAvailable) return "Action spent. Move if you can, or press Space to end.";
            if (state.Combat.MovePoints <= 0) return "No movement left. Attack, Spells/Skills, Guard, or press Space.";
            if (selectedAction == ActionMode.Move) return state.Combat.MovePoints > 0 ? "Click a highlighted tile to move." : "No movement remains.";
            if (selectedAction == ActionMode.Attack)
            {
                if (!state.Combat.ActionAvailable) return "Attack already spent.";
                int legalTargets = CountLegalAttackTargets(active);
                if (legalTargets <= 0)
                {
                    return state.Combat.MovePoints > 0
                        ? "No attack target from here. Move closer, use a power, Guard, or end the turn."
                        : "No attack target from here. Use a power, Guard, or end the turn.";
                }
                return legalTargets == 1
                    ? "1 attack target. Click its bracket to attack."
                    : $"{legalTargets} attack targets. Click a bracket to attack.";
            }
            if (selectedAction == ActionMode.Cast)
            {
                FormulaDef formula = GetFormula(pendingFormulaCode);
                if (formula == null) return showSpellbook
                    ? "Choose a spell card from the panel."
                    : "Press C or Spells to reopen the spellbook.";
                return $"Click a highlighted {FormulaTargetLabel(formula)} to cast {formula.Name}. Esc/right-click cancels.";
            }
            if (selectedAction == ActionMode.Ability)
            {
                MartialAbility ability = AbilityDef(pendingAbilityId);
                if (ability == null) return showAbilityPanel
                    ? "Choose a combat skill from the panel."
                    : "Press C or Skills to reopen combat skills.";
                return $"Click a highlighted enemy to use {ability.Name}. Esc/right-click cancels.";
            }
            if (selectedAction == ActionMode.Guard) return state.Combat.ActionAvailable ? "Press Guard to brace until next turn." : "Guard already spent.";
            if (selectedAction == ActionMode.Elixir) return state.Elixirs > 0 ? "Use a shared elixir as this unit's action." : "No elixirs remain.";
            return "Press End Turn to pass.";
        }

        private void NormalizeCombatSelection(CombatUnit active)
        {
            if (state?.Combat == null || active == null || active.Side != UnitSide.Party) return;
            if (showSpellbook || showAbilityPanel) return;
            if (active.Stunned > 0 || active.Sleeping > 0)
            {
                selectedAction = ActionMode.Wait;
                return;
            }
            if (state.Combat.MovePoints <= 0 && !state.Combat.ActionAvailable)
            {
                selectedAction = ActionMode.Wait;
                return;
            }
            if (!ActionEnabled(selectedAction, active))
            {
                if (state.Combat.ActionAvailable)
                {
                    if (ActionEnabled(ActionMode.Attack, active)) selectedAction = ActionMode.Attack;
                    else if (ActionEnabled(PreferredThirdAction(active), active)) selectedAction = PreferredThirdAction(active);
                    else if (ActionEnabled(ActionMode.Guard, active)) selectedAction = ActionMode.Guard;
                    else selectedAction = ActionMode.Wait;
                }
                else if (ActionEnabled(ActionMode.Move, active))
                {
                    selectedAction = ActionMode.Move;
                }
                else
                {
                    selectedAction = ActionMode.Wait;
                }
            }
        }

        private string ActiveThreatSummary(CombatUnit active)
        {
            int direct = DirectThreatCount(active);
            int pressure = PressureThreatCount(active) - direct;
            if (direct > 0) return direct == 1 ? "1 enemy can hit" : $"{direct} enemies can hit";
            if (pressure > 0) return pressure == 1 ? "1 enemy threatening" : $"{pressure} enemies threatening";
            return "no direct threat";
        }

        private Vector2Int ProjectedMoveThreatCounts(CombatUnit active, int destinationX, int destinationY)
        {
            if (active == null || state?.Combat?.Units == null) return Vector2Int.zero;
            int direct = 0;
            int pressure = 0;
            foreach (CombatUnit enemy in state.Combat.Units.Where(unit =>
                         unit != null
                         && unit.Side == UnitSide.Enemy
                         && unit.Hp > 0))
            {
                bool canHit = CanEnemyAttackAt(enemy, active, destinationX, destinationY)
                    || CanEnemySpecialReachAt(enemy, active, destinationX, destinationY);
                if (canHit)
                {
                    direct++;
                }
                else if (IsEnemyPressureThreatAt(enemy, active, destinationX, destinationY))
                {
                    pressure++;
                }
            }
            return new Vector2Int(direct, pressure);
        }

        private string ProjectedMoveThreatSummary(CombatUnit active, int destinationX, int destinationY)
        {
            Vector2Int counts = ProjectedMoveThreatCounts(active, destinationX, destinationY);
            return CombatThreatRules.MovementDestinationLabel(counts.x, counts.y);
        }

        private int DirectThreatCount(CombatUnit active)
        {
            if (active == null || state?.Combat?.Units == null) return 0;
            return state.Combat.Units.Count(u => u.Side == UnitSide.Enemy && u.Hp > 0 && CanEnemyReachNow(u, active));
        }

        private int PressureThreatCount(CombatUnit active)
        {
            if (active == null || state?.Combat?.Units == null) return 0;
            return state.Combat.Units.Count(u => u.Side == UnitSide.Enemy && u.Hp > 0 && IsEnemyPressureThreat(u, active));
        }

        private bool ShouldPromoteEndTurn(CombatUnit active)
        {
            return state?.Combat != null
                && active != null
                && !IsCombatResolutionPending()
                && CombatCommandPresentationRules.ShouldPromoteEndTurn(
                    active.Side == UnitSide.Party,
                    state.Combat.MovePoints,
                    state.Combat.ActionAvailable,
                    active.Stunned > 0,
                    active.Sleeping > 0);
        }

        private string ActiveWeaponLabel(CombatUnit active)
        {
            if (active == null) return "";
            if (active.DemonFormTurns > 0) return "abyssal claws";
            if (!string.IsNullOrEmpty(active.WeaponName)) return active.WeaponName;
            if (active.Role == "bow") return "ashwood bow";
            if (active.Role == "pike") return "long spear";
            if (active.Role == "knife") return "short blade";
            if (active.Role == "shield" || active.Role == "ward") return "shield and iron";
            if (active.Role == "mender") return "prayer focus";
            if (active.Role == "ember") return "ember focus";
            if (active.Role == "hex") return "hex focus";
            return active.Range > 1 ? "ranged attack" : "bare hands";
        }

        private string ArmorLabel(CombatUnit active)
        {
            if (active == null || string.IsNullOrEmpty(active.ArmorName)) return "no armor";
            return active.ArmorName;
        }

        private bool IsRangedAttackProfile(CombatUnit unit)
        {
            if (unit == null) return false;
            string role = (unit.Role ?? "").ToLowerInvariant();
            string cls = (unit.ClassKey ?? "").ToLowerInvariant();
            string weapon = (unit.WeaponName ?? "").ToLowerInvariant();
            if (cls == "ranger" || role == "bow") return true;
            if (weapon.Contains("bow") || weapon.Contains("crossbow") || weapon.Contains("sling") || weapon.Contains("throwing") || weapon.Contains("dart")) return true;
            return unit.Side == UnitSide.Enemy && unit.Range > 1 && role != "pike";
        }

        private int BaseAttackRange(CombatUnit unit)
        {
            if (unit == null) return 1;
            if (unit.DemonFormTurns > 0) return 1;
            int range = Mathf.Max(1, unit.Range);
            string role = (unit.Role ?? "").ToLowerInvariant();
            string cls = (unit.ClassKey ?? "").ToLowerInvariant();
            string weapon = (unit.WeaponName ?? "").ToLowerInvariant();
            if (cls == "ranger" || role == "bow") range = Mathf.Max(range, 5);
            if (weapon.Contains("longbow")) range = Mathf.Max(range, 5);
            else if (weapon.Contains("bow") || weapon.Contains("crossbow")) range = Mathf.Max(range, 4);
            if (weapon.Contains("sling") || weapon.Contains("throwing") || weapon.Contains("dart")) range = Mathf.Max(range, 3);
            return range;
        }

        private bool IsEngagedByHostileAt(CombatUnit unit, int x, int y)
        {
            if (unit == null || state?.Combat?.Units == null) return false;
            return state.Combat.Units.Any(other => other.Hp > 0 && other.Side != unit.Side && Distance(x, y, other.X, other.Y) <= 1);
        }

        private bool IsEngagedByHostile(CombatUnit unit)
        {
            return unit != null && IsEngagedByHostileAt(unit, unit.X, unit.Y);
        }

        private int EffectiveAttackRangeFrom(CombatUnit unit, int attackerX, int attackerY)
        {
            if (unit == null) return 1;
            if (IsRangedAttackProfile(unit) && IsEngagedByHostileAt(unit, attackerX, attackerY)) return 1;
            return BaseAttackRange(unit);
        }

        private int EffectiveAttackRangeTo(CombatUnit unit, int targetX, int targetY)
        {
            if (unit == null) return 1;
            return EffectiveAttackRangeFrom(unit, unit.X, unit.Y);
        }

        private bool UsesRangedAttackAt(CombatUnit unit, int attackerX, int attackerY, int targetX, int targetY)
        {
            return unit != null
                && IsRangedAttackProfile(unit)
                && EffectiveAttackRangeFrom(unit, attackerX, attackerY) > 1
                && Distance(attackerX, attackerY, targetX, targetY) > 1;
        }

        private bool UsesRangedAttack(CombatUnit unit, CombatUnit target)
        {
            return unit != null && target != null && UsesRangedAttackAt(unit, unit.X, unit.Y, target.X, target.Y);
        }

        private string AttackSkillName(CombatUnit attacker, CombatUnit target)
        {
            if (attacker == null || target == null) return "arms";
            return AttackSkillNameAt(attacker, target, attacker.X, attacker.Y);
        }

        private string AttackSkillNameAt(CombatUnit attacker, CombatUnit target, int attackerX, int attackerY)
        {
            if (attacker != null && attacker.DemonFormTurns > 0) return "hex";
            return attacker != null && target != null && UsesRangedAttackAt(attacker, attackerX, attackerY, target.X, target.Y)
                ? "missile"
                : "arms";
        }

        private string AttackModeLabel(CombatUnit active)
        {
            if (active == null) return "Attack";
            if (active.DemonFormTurns > 0) return "Claw";
            if (!IsRangedAttackProfile(active)) return "Attack";
            return IsEngagedByHostile(active) ? "Melee" : "Shoot";
        }

        private CombatAttackForecast AttackForecast(CombatUnit attacker, CombatUnit target)
        {
            int x = attacker?.X ?? 0;
            int y = attacker?.Y ?? 0;
            return BuildAttackForecastFrom(attacker, target, x, y, true);
        }

        private CombatAttackForecast AttackLegalityForecast(CombatUnit attacker, CombatUnit target)
        {
            int x = attacker?.X ?? 0;
            int y = attacker?.Y ?? 0;
            return BuildAttackForecastFrom(attacker, target, x, y, false);
        }

        private CombatAttackForecast AttackLegalityForecastFrom(CombatUnit attacker, CombatUnit target, int attackerX, int attackerY)
        {
            return BuildAttackForecastFrom(attacker, target, attackerX, attackerY, false);
        }

        private CombatAttackForecast BuildAttackForecastFrom(CombatUnit attacker, CombatUnit target, int attackerX, int attackerY, bool includeOutcome)
        {
            int range = attacker == null ? 1 : EffectiveAttackRangeFrom(attacker, attackerX, attackerY);
            int distance = attacker == null || target == null ? 0 : Distance(attackerX, attackerY, target.X, target.Y);
            bool ranged = attacker != null && target != null && UsesRangedAttackAt(attacker, attackerX, attackerY, target.X, target.Y);
            bool hasSight = attacker != null && target != null && (!ranged || HasLineOfSight(attackerX, attackerY, target.X, target.Y, true));
            AttackForecastBlockReason blockReason = AttackForecastBlockReason.None;
            if (attacker == null || target == null) blockReason = AttackForecastBlockReason.InvalidCombatants;
            else if (attacker.Side == target.Side) blockReason = AttackForecastBlockReason.FriendlyTarget;
            else if (target.Hp <= 0) blockReason = AttackForecastBlockReason.DefeatedTarget;
            else if (distance > range) blockReason = AttackForecastBlockReason.OutOfRange;
            else if (!hasSight) blockReason = AttackForecastBlockReason.LineOfSight;

            int hitChance = 0;
            Vector2Int damage = Vector2Int.zero;
            string damageType = attacker == null || string.IsNullOrWhiteSpace(attacker.DamageType) ? "physical" : attacker.DamageType;
            string damageMatch = target == null ? "normal" : DamageMatchNote(target, damageType);
            if (blockReason == AttackForecastBlockReason.None && includeOutcome)
            {
                hitChance = AttackHitChanceAt(attacker, target, attackerX, attackerY);
                damage = AttackDamagePreviewAt(attacker, target, attackerX, attackerY);
            }

            return CombatThreatRules.Create(
                blockReason,
                includeOutcome,
                ranged,
                hasSight,
                distance,
                range,
                hitChance,
                damage.x,
                damage.y,
                damageType,
                damageMatch,
                target != null && target.Guarding,
                target?.Hp ?? 0,
                target?.MaxHp ?? 0);
        }

        private void DrawCombatHighlights(Rect grid, float cell, CombatUnit active)
        {
            if (selectedAction == ActionMode.Move && state.Combat.MovePoints > 0)
            {
                int[,] reachable = ReachableMoveCosts(active);
                for (int y = 0; y < CombatH; y++)
                for (int x = 0; x < CombatW; x++)
                {
                    int distance = Distance(x, y, active.X, active.Y);
                    int moveCost = reachable[x, y];
                    if (distance > 0 && moveCost < UnreachableMoveCost && moveCost <= state.Combat.MovePoints && CanStandAt(x, y))
                    {
                        float alpha = Mathf.Lerp(0.30f, 0.14f, moveCost / (float)(CombatMoveAllowance + 2));
                        Point terrain = ObstacleAt(x, y);
                        Rect mark = new Rect(grid.x + x * cell + 4, grid.y + y * cell + 4, cell - 8, cell - 8);
                        DrawRect(mark, TerrainHighlightColor(terrain, alpha));
                        if (TerrainMoveExtraCost(terrain) > 0)
                        {
                            DrawBorder(Pad(mark, cell * 0.08f), Hex("d7a84e", 0.58f), 1);
                        }
                    }
                }
            }

            if (selectedAction == ActionMode.Attack || selectedAction == ActionMode.Cast || selectedAction == ActionMode.Ability)
            {
                FormulaDef formula = selectedAction == ActionMode.Cast ? GetFormula(pendingFormulaCode) : null;
                MartialAbility ability = selectedAction == ActionMode.Ability ? AbilityDef(pendingAbilityId) : null;
                if (!CombatTargetingRules.ShouldDrawTargetHighlights(
                    selectedAction,
                    formula != null,
                    ability != null))
                {
                    return;
                }
                for (int y = 0; y < CombatH; y++)
                for (int x = 0; x < CombatW; x++)
                {
                    CombatTargetHighlightState highlight = CombatTargetHighlightStateAt(
                        active,
                        formula,
                        ability,
                        x,
                        y);
                    if (highlight == CombatTargetHighlightState.None) continue;
                    bool legal = highlight == CombatTargetHighlightState.Legal;
                    Color accent = Hex("8a5c35");
                    if (legal && selectedAction == ActionMode.Cast)
                    {
                        accent = FormulaColor(formula);
                    }
                    else if (legal && selectedAction == ActionMode.Ability)
                    {
                        accent = CombatPowerPresentationRules.AbilityAccent(ability.ClassKey).ToColor();
                    }
                    else if (legal)
                    {
                        Point cover = ObstacleAt(x, y);
                        accent = IsDisruptableRitual(cover)
                            ? ObstacleAccent(cover.Kind)
                            : gold;
                    }
                    Rect tile = new Rect(grid.x + x * cell, grid.y + y * cell, cell, cell);
                    Rect mark = new Rect(tile.x + 6f, tile.y + 6f, tile.width - 12f, tile.height - 12f);
                    float candidateAlpha = !legal
                        ? 0.05f
                        : selectedAction == ActionMode.Attack ? 0.12f : 0.07f;
                    DrawRect(mark, accent.WithAlpha(candidateAlpha));
                }
            }
        }

        private void DrawCombatTargetStateShapes(Rect grid, float cell, CombatUnit active)
        {
            FormulaDef formula = selectedAction == ActionMode.Cast ? GetFormula(pendingFormulaCode) : null;
            MartialAbility ability = selectedAction == ActionMode.Ability ? AbilityDef(pendingAbilityId) : null;
            if (!CombatTargetingRules.ShouldDrawTargetHighlights(
                selectedAction,
                formula != null,
                ability != null))
            {
                return;
            }

            for (int y = 0; y < CombatH; y++)
            for (int x = 0; x < CombatW; x++)
            {
                CombatTargetHighlightState highlight = CombatTargetHighlightStateAt(
                    active,
                    formula,
                    ability,
                    x,
                    y);
                if (highlight == CombatTargetHighlightState.None) continue;
                bool legal = highlight == CombatTargetHighlightState.Legal;
                Color accent = Hex("8a5c35");
                if (legal && selectedAction == ActionMode.Cast)
                {
                    accent = FormulaColor(formula);
                }
                else if (legal && selectedAction == ActionMode.Ability)
                {
                    accent = CombatPowerPresentationRules.AbilityAccent(ability.ClassKey).ToColor();
                }
                else if (legal)
                {
                    Point cover = ObstacleAt(x, y);
                    accent = IsDisruptableRitual(cover)
                        ? ObstacleAccent(cover.Kind)
                        : gold;
                }
                Rect tile = new Rect(grid.x + x * cell, grid.y + y * cell, cell, cell);
                DrawTargetStateShape(tile, accent.WithAlpha(legal ? 0.82f : 0.74f), legal);
            }
        }

        private CombatTargetHighlightState CombatTargetHighlightStateAt(
            CombatUnit active,
            FormulaDef formula,
            MartialAbility ability,
            int x,
            int y)
        {
            if (active == null) return CombatTargetHighlightState.None;
            CombatUnit unit = UnitAt(x, y);
            Point cover = ObstacleAt(x, y);
            if (selectedAction == ActionMode.Cast && formula != null)
            {
                bool candidate = CanTargetFormula(formula, active, unit, x, y);
                if (!candidate) return CombatTargetHighlightState.None;
                bool inRange = Distance(x, y, active.X, active.Y) <= EffectiveFormulaRange(formula, active);
                if (!inRange && unit == null && cover == null)
                {
                    return CombatTargetHighlightState.None;
                }
                return IsFormulaActionable(formula, active, unit, x, y)
                    ? CombatTargetHighlightState.Legal
                    : CombatTargetHighlightState.Blocked;
            }
            if (selectedAction == ActionMode.Attack)
            {
                if (unit != null && unit.Side == UnitSide.Enemy)
                {
                    return AttackForecast(active, unit).Legal
                        ? CombatTargetHighlightState.Legal
                        : CombatTargetHighlightState.Blocked;
                }
                if (IsDisruptableRitual(cover) || IsBreakableCover(cover))
                {
                    return CanAttackCombatObstacle(active, cover)
                        ? CombatTargetHighlightState.Legal
                        : CombatTargetHighlightState.Blocked;
                }
                return CombatTargetHighlightState.None;
            }
            if (selectedAction == ActionMode.Ability && ability != null)
            {
                if (unit == null || unit.Side != UnitSide.Enemy)
                {
                    return CombatTargetHighlightState.None;
                }
                return CanTargetAbility(active, ability, unit, x, y, out _)
                    ? CombatTargetHighlightState.Legal
                    : CombatTargetHighlightState.Blocked;
            }
            return CombatTargetHighlightState.None;
        }

        private void DrawEnemyThreatCues(Rect grid, float cell, CombatUnit active)
        {
            if (active == null || state?.Combat?.Units == null) return;
            bool previewsDestination = false;
            int previewX = active.X;
            int previewY = active.Y;
            if (selectedAction == ActionMode.Move
                && TryCombatHoverCell(grid, cell, out int hoverX, out int hoverY, out _))
            {
                int moveCost = MoveCostTo(active, hoverX, hoverY);
                previewsDestination = CanStandAt(hoverX, hoverY)
                    && moveCost < UnreachableMoveCost
                    && moveCost <= state.Combat.MovePoints;
                if (previewsDestination)
                {
                    previewX = hoverX;
                    previewY = hoverY;
                }
            }

            foreach (CombatUnit enemy in state.Combat.Units.Where(u => u.Side == UnitSide.Enemy && u.Hp > 0))
            {
                CombatAttackForecast forecast = AttackForecast(enemy, active);
                bool standard = previewsDestination
                    ? CanEnemyAttackAt(enemy, active, previewX, previewY)
                    : forecast.Legal;
                bool special = !standard && (previewsDestination
                    ? CanEnemySpecialReachAt(enemy, active, previewX, previewY)
                    : CanEnemySpecialReach(enemy, active));
                bool direct = standard || special;
                bool pressure = !direct && (previewsDestination
                    ? IsEnemyPressureThreatAt(enemy, active, previewX, previewY)
                    : IsEnemyPressureThreat(enemy, active));
                if (!direct && !pressure) continue;
                if (!direct && selectedAction != ActionMode.Move) continue;
                Rect tile = new Rect(grid.x + enemy.X * cell + cell * 0.08f, grid.y + enemy.Y * cell + cell * 0.08f, cell * 0.84f, cell * 0.84f);
                CombatThreatLevel level = !previewsDestination && forecast.Legal
                    ? forecast.ThreatLevel
                    : direct ? CombatThreatLevel.Direct : CombatThreatLevel.Pressure;
                Color accent = CombatThreatAccent(level);
                float pulse = state.ReducedMotion ? 0.45f : 0.5f + Mathf.Sin(Time.time * (direct ? 7.5f : 4.5f)) * 0.5f;
                float railW = Mathf.Max(direct ? 3f : 2f, cell * (direct ? 0.035f : 0.025f));
                Rect rail = new Rect(tile.xMax - railW, tile.y + tile.height * 0.20f, railW, tile.height * 0.60f);
                DrawRect(rail, accent.WithAlpha((direct ? 0.72f : 0.42f) + pulse * 0.12f));
                if (previewsDestination)
                {
                    DrawBorder(tile, accent.WithAlpha(direct ? 0.90f : 0.68f), direct ? 2 : 1);
                    DrawTargetBadge(tile, direct ? "HIT" : "MOVE", accent, false);
                }
            }
        }

        private Color CombatThreatAccent(CombatThreatLevel level)
        {
            switch (level)
            {
                case CombatThreatLevel.Lethal: return Hex("f06a5c");
                case CombatThreatLevel.Severe: return Hex("e08a45");
                case CombatThreatLevel.Direct: return blood;
                case CombatThreatLevel.Pressure: return gold;
                default: return muted;
            }
        }

        private bool CanEnemyReachNow(CombatUnit enemy, CombatUnit active)
        {
            if (enemy == null || active == null || active.Hp <= 0) return false;
            return CanEnemyAttack(enemy, active) || CanEnemySpecialReach(enemy, active);
        }

        private bool CanEnemyAttackAt(CombatUnit enemy, CombatUnit active, int targetX, int targetY)
        {
            if (enemy == null || active == null || active.Hp <= 0 || enemy.Side == active.Side) return false;
            int range = EffectiveAttackRangeFrom(enemy, enemy.X, enemy.Y);
            if (Distance(enemy.X, enemy.Y, targetX, targetY) > range) return false;
            bool ranged = UsesRangedAttackAt(enemy, enemy.X, enemy.Y, targetX, targetY);
            return !ranged || HasLineOfSight(enemy.X, enemy.Y, targetX, targetY, true);
        }

        private bool CanEnemySpecialReachAt(CombatUnit enemy, CombatUnit active, int targetX, int targetY)
        {
            if (enemy == null || active == null || active.Hp <= 0) return false;
            if (Distance(enemy.X, enemy.Y, targetX, targetY) > enemy.Range) return false;
            return HasLineOfSight(enemy.X, enemy.Y, targetX, targetY, true) || EnemySpecialArcsOverCover(enemy);
        }

        private bool IsEnemyPressureThreat(CombatUnit enemy, CombatUnit active)
        {
            return active != null && IsEnemyPressureThreatAt(enemy, active, active.X, active.Y);
        }

        private bool IsEnemyPressureThreatAt(CombatUnit enemy, CombatUnit active, int targetX, int targetY)
        {
            if (enemy == null || active == null || active.Hp <= 0) return false;
            int distance = Distance(enemy.X, enemy.Y, targetX, targetY);
            int reach = Mathf.Max(1, enemy.Range) + UnitMoveAllowance(enemy);
            if (distance > reach) return false;
            if (enemy.Range > 1) return HasLineOfSight(enemy.X, enemy.Y, targetX, targetY, true) || EnemySpecialArcsOverCover(enemy);
            return true;
        }

        private void HandleCombatMouse(Rect grid, float cell)
        {
            Event e = Event.current;
            if (e == null || e.type != EventType.MouseDown) return;
            if (IsBoardPointerSuppressed()) return;
            if (IsCombatResolutionPending()) return;
            if (!ScreenInputRules.ShouldRouteBoardPointer(
                    CurrentUiOverlay(),
                    UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(),
                    grid.Contains(e.mousePosition),
                    false,
                    false)) return;
            CombatUnit active = CurrentUnit();
            if (active == null || active.Side != UnitSide.Party) return;
            if (e.button == 1)
            {
                if (CancelCombatTargeting()) e.Use();
                return;
            }
            if (e.button != 0) return;
            int x = Mathf.FloorToInt((e.mousePosition.x - grid.x) / cell);
            int y = Mathf.FloorToInt((e.mousePosition.y - grid.y) / cell);
            CombatUnit target = UnitAt(x, y);

            if (selectedAction == ActionMode.Move)
            {
                MoveActiveTo(active, x, y);
                e.Use();
                return;
            }
            if (selectedAction == ActionMode.Attack && state.Combat.ActionAvailable && target != null && target.Side == UnitSide.Enemy)
            {
                CombatCommandResult result = CombatLifecycle().TryAttack(active, target, Attack);
                if (result.Success)
                {
                    AfterCombatAction(active);
                }
                e.Use();
                return;
            }
            if (selectedAction == ActionMode.Attack && state.Combat.ActionAvailable)
            {
                Point cover = ObstacleAt(x, y);
                if (IsDisruptableRitual(cover))
                {
                    CombatCommandResult result = CombatLifecycle().TryResolveAction(active, () => AttackRitual(active, cover));
                    if (result.Success)
                    {
                        AfterCombatAction(active);
                    }
                    e.Use();
                    return;
                }
                if (IsBreakableCover(cover))
                {
                    CombatCommandResult result = CombatLifecycle().TryResolveAction(active, () => AttackCover(active, cover));
                    if (result.Success)
                    {
                        AfterCombatAction(active);
                    }
                    e.Use();
                    return;
                }
            }
            if (selectedAction == ActionMode.Cast && state.Combat.ActionAvailable)
            {
                if (string.IsNullOrEmpty(pendingFormulaCode))
                {
                    PushLog("Choose a spell card first.", Tone.Warn);
                    PlaySfx("blocked", 0.62f);
                    e.Use();
                    return;
                }
                FormulaDef resolvedFormula = GetFormula(pendingFormulaCode);
                CombatPowerOutcomeSnapshot outcomeBefore = CombatPowerOutcomeRules.Capture(state.Combat);
                CombatCommandResult result = CombatLifecycle().TryResolveAction(active, () => CastFormula(active, pendingFormulaCode, target, x, y));
                if (result.Success)
                {
                    SetCombatPowerOutcome(outcomeBefore);
                    FinishResolvedPlayerFormulaAction(active, resolvedFormula);
                }
                e.Use();
            }
            if (selectedAction == ActionMode.Ability && state.Combat.ActionAvailable)
            {
                if (string.IsNullOrEmpty(pendingAbilityId))
                {
                    PushLog("Choose a combat skill first.", Tone.Warn);
                    PlaySfx("blocked", 0.62f);
                    e.Use();
                    return;
                }
                MartialAbility resolvedAbility = AbilityDef(pendingAbilityId);
                CombatPowerOutcomeSnapshot outcomeBefore = CombatPowerOutcomeRules.Capture(state.Combat);
                CombatCommandResult result = CombatLifecycle().TryResolveAction(active, () => UseTargetedAbility(active, pendingAbilityId, target, x, y));
                if (result.Success)
                {
                    SetCombatPowerOutcome(outcomeBefore);
                    FinishResolvedPlayerAbilityAction(active, resolvedAbility);
                }
                e.Use();
            }
        }

        private void DrawSidePanels()
        {
            sideRect = SidePanelRect();
            if (state.Mode == GameMode.Combat)
            {
                return;
            }

            float gap = 10f;
            float minLogH;
            float partyH;
            float enemiesH;
            minLogH = Mathf.Min(174f, Mathf.Max(118f, sideRect.height * 0.18f));
            partyH = state.Mode == GameMode.Combat
                ? Mathf.Clamp(sideRect.height * 0.30f, 238f, 340f)
                : Mathf.Clamp(sideRect.height * 0.33f, 220f, 330f);
            enemiesH = state.Mode == GameMode.Combat ? Mathf.Clamp(sideRect.height * 0.24f, 168f, 270f) : Mathf.Clamp(sideRect.height * 0.34f, 205f, 360f);
            if (partyH + enemiesH + minLogH + gap * 2f > sideRect.height)
            {
                float usable = sideRect.height - minLogH - gap * 2f;
                partyH = Mathf.Max(220f, usable * 0.58f);
                enemiesH = Mathf.Max(140f, usable - partyH);
            }

            Rect partyRect = new Rect(sideRect.x, sideRect.y, sideRect.width, partyH);
            Rect enemiesRect = new Rect(sideRect.x, partyRect.yMax + gap, sideRect.width, enemiesH);
            Rect logRect = new Rect(sideRect.x, enemiesRect.yMax + gap, sideRect.width, Mathf.Max(96f, sideRect.yMax - enemiesRect.yMax - gap));

            DrawRpgPanel(partyRect, teal);
            DrawPanelHeader(partyRect, "Party", "party", teal, state.Party.Count + " sworn");
            float rosterTop = partyRect.y + 45f;
            float rosterH = Mathf.Max(30f, partyRect.yMax - rosterTop - 10f);
            int partyCount = Mathf.Max(1, state.Party.Count);
            float memberGap = 4f;
            float memberMinH = sideRect.height < 700f ? 34f : 40f;
            float memberH = Mathf.Clamp((rosterH - memberGap * Mathf.Max(0, partyCount - 1)) / partyCount, memberMinH, 78f);
            float y = rosterTop;
            CombatUnit active = CurrentUnit();
            foreach (PartyMember member in state.Party)
            {
                DrawMemberCard(new Rect(partyRect.x + 10, y, partyRect.width - 20, memberH), member, active?.PartyIndex == state.Party.IndexOf(member));
                y += memberH + memberGap;
                if (y > partyRect.yMax - memberH) break;
            }

            DrawLocationPanel(enemiesRect);

            DrawRpgPanel(logRect, gold);
            DrawPanelHeader(logRect, "Timeline", "timeline", gold, "");
            Rect view = new Rect(logRect.x + 10, logRect.y + 44, logRect.width - 20, Mathf.Max(36f, logRect.height - 54));
            float contentW = view.width - 18;
            List<float> logHeights = new List<float>();
            float totalLogH = 0f;
            foreach (LogEntry entry in state.Log)
            {
                float rowH = Mathf.Clamp(logStyle.CalcHeight(new GUIContent(entry.Text), contentW - 18f) + 14f, 40f, 78f);
                logHeights.Add(rowH);
                totalLogH += rowH + 6f;
            }
            Rect content = new Rect(0, 0, contentW, Mathf.Max(view.height, totalLogH));
            logScroll = GUI.BeginScrollView(view, logScroll, content);
            float ly = 0;
            for (int i = 0; i < state.Log.Count; i++)
            {
                LogEntry entry = state.Log[i];
                float rowH = i < logHeights.Count ? logHeights[i] : 40f;
                Color stripe = entry.Tone == Tone.Warn ? ember : entry.Tone == Tone.Good ? teal : moss;
                Rect row = new Rect(0, ly, content.width, rowH);
                DrawRect(row, Hex("151b20"));
                DrawRect(new Rect(row.x, row.y, 4, row.height), stripe);
                GUI.Label(new Rect(row.x + 10, row.y + 6, row.width - 18, row.height - 10), entry.Text, logStyle);
                ly += rowH + 6f;
            }
            GUI.EndScrollView();
        }

        private string CombatFocusStateLine(CombatUnit unit, bool activeCard)
        {
            if (unit == null) return "";
            if (activeCard && state?.Combat != null)
            {
                string action = state.Combat.ActionAvailable ? "Action ready" : "Action used";
                return $"Move {state.Combat.MovePoints} / {action}";
            }
            CombatUnit active = CurrentUnit();
            if (active != null && unit.Id != active.Id) return $"Distance {Distance(active.X, active.Y, unit.X, unit.Y)}";
            return unit.Side == UnitSide.Party ? "Party unit" : "Enemy unit";
        }

        private void DrawLocationPanel(Rect rect)
        {
            DrawRpgPanel(rect, moss);
            WorldZone zone = ZoneAt(state.PlayerX, state.PlayerY);
            MapObject obj = ObjectAt(state.Map, state.PlayerX, state.PlayerY);
            DrawPanelHeader(rect, "Location", "scroll", moss, TravelDangerLabel(zone));
            float y = rect.y + 46f;
            GUI.Label(new Rect(rect.x + 14f, y, rect.width - 28f, 20f), FitText(zone.Name, rect.width - 28f, CenterLeftStyle(14, gold)), CenterLeftStyle(14, gold));
            y += 22f;
            GUI.Label(new Rect(rect.x + 14f, y, rect.width - 28f, 18f), FitText(zone.Title + " / " + ExploreGroundName(state.PlayerX, state.PlayerY), rect.width - 28f, CenterLeftStyle(10, muted)), CenterLeftStyle(10, muted));
            y += 24f;
            string nearbyAction = ExploreNearbyActionLine();
            string routeChart = RegionalRouteChartCompactLine();
            string anchor = !string.IsNullOrEmpty(exploreHoverLookLine)
                ? "Look: " + exploreHoverLookLine.Replace("\n", " / ")
                : !string.IsNullOrEmpty(nearbyAction) ? nearbyAction
                : obj == null && ShouldShowMidgaardTracker() ? MidgaardWayfindingCompactLine() : obj == null ? "Nearby: roads, fog, and old markers." : $"{ObjectName(obj.Type)}: {ObjectHint(obj)}.";
            if (string.IsNullOrEmpty(exploreHoverLookLine) && string.IsNullOrEmpty(nearbyAction) && obj == null && !ShouldShowMidgaardTracker() && !string.IsNullOrEmpty(routeChart))
            {
                anchor = routeChart;
            }
            float anchorH = rect.height < 230f ? 24f : 30f;
            Rect anchorRect = new Rect(rect.x + 12f, y - 2f, rect.width - 24f, anchorH + 6f);
            if (!string.IsNullOrEmpty(exploreHoverLookLine))
            {
                DrawRect(anchorRect, Hex("080b0d", 0.76f));
                DrawBorder(anchorRect, teal.WithAlpha(0.50f), 1);
            }
            GUI.Label(new Rect(rect.x + 16f, y, rect.width - 32f, anchorH), FitText(anchor, rect.width - 32f, CenterLeftStyle(11, ink)), CenterLeftStyle(11, ink));
            y += anchorH + 6f;
            bool showMidgaardTracker = ShouldShowMidgaardTracker();
            bool showKoboldTracker = !showMidgaardTracker && ShouldShowKoboldRouteTracker();
            bool showRouteTracker = showMidgaardTracker || showKoboldTracker;
            float trackerTarget = showMidgaardTracker ? 112f : showKoboldTracker ? 92f : 30f;
            float reservedBottom = showRouteTracker ? trackerTarget + 10f : 38f;
            if (rect.height > 172f)
            {
                float mapSpace = rect.yMax - y - 12f - reservedBottom;
                if (mapSpace >= 50f)
                {
                    float mapH = Mathf.Clamp(Mathf.Min(rect.height * (showRouteTracker ? 0.24f : 0.32f), mapSpace), 50f, showRouteTracker ? 86f : 104f);
                    DrawExploreMiniMap(new Rect(rect.x + 12f, y, rect.width - 24f, mapH));
                    y += mapH + 8f;
                }
            }
            if (!showRouteTracker && rect.height > 282f && rect.yMax - y > 118f)
            {
                GUI.Label(new Rect(rect.x + 14f, y, rect.width - 28f, 30f), FitText(zone.Summary, rect.width - 28f, CenterLeftStyle(10, muted)), CenterLeftStyle(10, muted));
                y += 36f;
            }
            float trackerH = rect.yMax - y - 12f;
            if (showMidgaardTracker && trackerH >= 86f)
            {
                DrawMidgaardRouteTracker(new Rect(rect.x + 12f, y, rect.width - 24f, Mathf.Min(124f, trackerH)));
            }
            else if (showKoboldTracker && trackerH >= 64f)
            {
                DrawKoboldRouteTracker(new Rect(rect.x + 12f, y, rect.width - 24f, Mathf.Min(92f, trackerH)));
            }
            else if (trackerH >= 30f)
            {
                DrawPartyGrowthChip(new Rect(rect.x + 12f, y, rect.width - 24f, 30f));
            }
        }

        private void DrawExploreMiniMap(Rect rect)
        {
            if (state?.Map == null) return;
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            DrawRect(rect, Hex("080b0d", 0.78f));
            DrawBorder(rect, line.WithAlpha(0.62f), 1);

            float labelH = Mathf.Min(22f * scale, rect.height * 0.26f);
            int labelSize = ExplorationHudScreenLayout.FontSize(11, Screen.width, Screen.height);
            GUI.Label(new Rect(rect.x + 9f * scale, rect.y + 3f * scale, rect.width * 0.58f, labelH), ExploreViewLabel(), CenterLeftStyle(labelSize, exploreWideView ? frost : teal));
            GUI.Label(new Rect(rect.xMax - 88f * scale, rect.y + 3f * scale, 78f * scale, labelH), "TAB", CenterRightStyle(labelSize, Hex("d0c5ae")));

            Rect map = new Rect(rect.x + 9f * scale, rect.y + labelH + 7f * scale, rect.width - 18f * scale, rect.height - labelH - 16f * scale);
            if (map.width <= 12f || map.height <= 12f) return;
            float aspect = (float)state.Map.Width / Mathf.Max(1, state.Map.Height);
            if (map.width / map.height > aspect)
            {
                float w = map.height * aspect;
                map.x += (map.width - w) * 0.5f;
                map.width = w;
            }
            else
            {
                float h = map.width / aspect;
                map.y += (map.height - h) * 0.5f;
                map.height = h;
            }

            DrawRect(map, Hex("020303", 0.94f));
            float sx = map.width / state.Map.Width;
            float sy = map.height / state.Map.Height;
            for (int y = 0; y < state.Map.Height; y++)
            for (int x = 0; x < state.Map.Width; x++)
            {
                int tile = TileAt(state.Map, x, y);
                bool near = Distance(x, y, state.PlayerX, state.PlayerY) <= ExploreRevealRadius;
                Color c = tile == 0
                    ? Hex("050708", near ? 0.74f : 0.36f)
                    : Color.Lerp(ZoneDangerColor(ZoneFor(x, y, state.Map, state.Depth)), Hex("151b20"), near ? 0.36f : 0.68f).WithAlpha(near ? 0.86f : 0.38f);
                DrawRect(new Rect(map.x + x * sx, map.y + y * sy, Mathf.Max(1f, sx + 0.35f), Mathf.Max(1f, sy + 0.35f)), c);
            }

            foreach (MapObject mapObject in state.Map.Objects)
            {
                if (mapObject == null) continue;
                bool near = Distance(mapObject.X, mapObject.Y, state.PlayerX, state.PlayerY) <= ExploreRevealRadius;
                bool important = IsCurrentMidgaardObjective(mapObject) || mapObject.Type == ObjectType.Stairs || mapObject.Type == ObjectType.Encounter || IsRouteScaffoldObject(mapObject.Type);
                if (!near && !important) continue;
                float dot = important ? 4f : 3f;
                Rect dotRect = new Rect(map.x + mapObject.X * sx + sx * 0.5f - dot * 0.5f, map.y + mapObject.Y * sy + sy * 0.5f - dot * 0.5f, dot, dot);
                DrawRect(dotRect, ObjectColor(mapObject.Type).WithAlpha(important ? 0.96f : 0.70f));
            }

            int viewW = Mathf.Min(ExploreViewportWidth(), state.Map.Width);
            int viewH = Mathf.Min(ExploreViewportHeight(), state.Map.Height);
            Point origin = ExploreViewportOrigin(viewW, viewH);
            Rect view = new Rect(map.x + origin.X * sx, map.y + origin.Y * sy, viewW * sx, viewH * sy);
            DrawBorder(view, gold.WithAlpha(0.94f), 1);
            DrawBorder(Pad(view, -2f), cursorWhite.WithAlpha(0.36f), 1);

            Rect party = new Rect(map.x + state.PlayerX * sx + sx * 0.5f - 3f, map.y + state.PlayerY * sy + sy * 0.5f - 3f, 6f, 6f);
            DrawRect(party, teal);
            DrawBorder(Pad(party, -1f), cursorWhite.WithAlpha(0.86f), 1);
        }

        private void DrawPartyGrowthChip(Rect rect)
        {
            DrawRect(rect, Hex("080b0d", 0.58f));
            DrawBorder(rect, gold.WithAlpha(0.36f), 1);
            Rect icon = new Rect(rect.x + 7f, rect.y + 5f, 20f, 20f);
            if (!TryDrawWorldMapProgressionOverlayAtlasIcon(icon, 9, Color.white.WithAlpha(0.78f)))
            {
                DrawTinyUiIcon(icon, "scroll", gold);
            }
            GUI.Label(new Rect(rect.x + 34f, rect.y + 6f, rect.width - 42f, 18f), FitText(PartyGrowthLine(), rect.width - 42f, CenterLeftStyle(10, muted)), CenterLeftStyle(10, muted));
        }

        private string PartyGrowthLine()
        {
            if (state?.Party == null || state.Party.Count == 0) return "No party growth data yet.";
            int avg = Mathf.Max(1, Mathf.RoundToInt((float)state.Party.Average(p => Mathf.Max(1, p.Level))));
            PartyMember closest = state.Party.OrderBy(p => Mathf.Max(0, ExperienceForNextLevel(p.Level) - p.Experience)).FirstOrDefault();
            int unspentStats = state.Party.Sum(p => Mathf.Max(0, p.StatPoints));
            int unspentSkills = state.Party.Sum(p => Mathf.Max(0, p.SkillPoints));
            string next = closest == null ? "" : $"{closest.Name} {Mathf.Max(0, ExperienceForNextLevel(closest.Level) - closest.Experience)} XP to L{closest.Level + 1}";
            string ready = unspentStats + unspentSkills > 0 ? $" / spend {unspentStats} stat {unspentSkills} skill" : "";
            return $"Party L{avg} / {next}{ready}";
        }

        private bool ShouldShowMidgaardTracker()
        {
            return ShouldUseMidgaardWayfinding();
        }

        private bool ShouldUseMidgaardWayfinding()
        {
            if (state?.Map == null || state.Depth != 1) return false;
            const int horizontalApproach = 6;
            const int verticalApproach = 4;
            return state.PlayerX >= MidgaardLeft(state.Map) - horizontalApproach
                && state.PlayerX <= MidgaardRight(state.Map) + horizontalApproach
                && state.PlayerY >= MidgaardTop(state.Map) - verticalApproach
                && state.PlayerY <= MidgaardBottom(state.Map) + verticalApproach;
        }

        private void DrawMidgaardRouteTracker(Rect rect)
        {
            if (rect.height < 84f) return;
            DrawRect(rect, Hex("080b0d", 0.62f));
            DrawBorder(rect, MidgaardObjectiveColor().WithAlpha(0.62f), 1);
            Rect icon = new Rect(rect.x + 8f, rect.y + 8f, 30f, 30f);
            if (!TryDrawWorldMapProgressionOverlayAtlasIcon(icon, MidgaardRouteProgressionIcon(), Color.white.WithAlpha(0.88f)))
            {
                DrawTinyUiIcon(icon, MidgaardRouteActiveStep() == 1 ? "enemy" : MidgaardRouteActiveStep() == 2 ? "scroll" : "party", MidgaardObjectiveColor());
            }
            float chipH = 22f;
            float chipY = rect.yMax - chipH - 8f;
            float textMaxY = chipY - 5f;
            GUI.Label(new Rect(rect.x + 46f, rect.y + 7f, rect.width - 56f, 16f), "Midgaard Work", CenterLeftStyle(12, gold));
            GUI.Label(new Rect(rect.x + 46f, rect.y + 23f, rect.width - 56f, 15f), FitText(MidgaardRouteStatusLine(), rect.width - 56f, CenterLeftStyle(9, muted)), CenterLeftStyle(9, muted));
            if (textMaxY - (rect.y + 42f) >= 13f)
            {
                Rect wayfinding = new Rect(rect.x + 10f, rect.y + 42f, rect.width - 20f, Mathf.Min(30f, textMaxY - (rect.y + 42f)));
                GUI.Label(wayfinding, FitText(MidgaardWayfindingCompactLine(), wayfinding.width, CenterLeftStyle(8, muted)), CenterLeftStyle(8, muted));
            }

            string[] labels = rect.width < 300f ? new[] { "King", "Rat", "Gear", "Road" } : new[] { "King", "Sewer", "Armor", "Road" };
            float chipGap = 4f;
            float chipW = Mathf.Max(44f, (rect.width - 16f - chipGap * 3f) / 4f);
            int active = MidgaardRouteActiveStep();
            for (int i = 0; i < labels.Length; i++)
            {
                bool done = MidgaardRouteStepComplete(i);
                Rect chip = new Rect(rect.x + 8f + i * (chipW + chipGap), chipY, chipW, chipH);
                Color accent = done ? teal : i == active ? MidgaardObjectiveColor() : line;
                DrawRect(chip, Hex("151b20", done ? 0.86f : 0.62f));
                DrawBorder(chip, accent.WithAlpha(i == active ? 0.95f : 0.62f), i == active ? 2 : 1);
                GUI.Label(new Rect(chip.x + 4f, chip.y + 2f, chip.width - 8f, chip.height - 2f), FitText(done ? labels[i] + " ok" : labels[i], chip.width - 8f, CenterStyle(8, done ? teal : ink)), CenterStyle(8, done ? teal : ink));
            }
        }

        private string MidgaardRouteStatusLine()
        {
            if (state == null) return "";
            if (state.Depth > 1) return "Midgaard work started the road below.";
            if (ContentSetCatalog.ShowPrototypeScaffold(activeContentSet) && HasStoryFlag(StoryFlags.MidgaardLampRoundStarted) && !HasStoryFlag(StoryFlags.MidgaardLampRoundComplete)) return "Lamp Round: " + LampRoundStatusLine() + ".";
            if (ContentSetCatalog.ShowPrototypeScaffold(activeContentSet) && HasStoryFlag(StoryFlags.MidgaardGateSurveyStarted) && !HasStoryFlag(StoryFlags.MidgaardGateSurveyComplete)) return "Gate Survey: " + GateSurveyStatusLine() + ".";
            if (!HasStoryFlag(StoryFlags.MidgaardRatQuestGiven)) return "Speak with the King of Midgaard.";
            if (!ContentSetCatalog.IsFullPrototype(activeContentSet) && !HasStoryFlag(StoryFlags.SewerCisternDenCleared)) return $"Clear the Midgaard sewer rooms. Rooms {ContentSetCatalog.SewerSliceClearedCount(state.StoryFlags)}/{ContentSetCatalog.SewerSliceEncounters.Count}.";
            if (ContentSetCatalog.IsFullPrototype(activeContentSet) && !HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade) && !MidgaardRatPeltsReady()) return $"Enter the sewer and collect four pelts. Pelts {RatPeltCount()}/4.";
            if (!ContentSetCatalog.IsFullPrototype(activeContentSet) && !HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade)) return $"Bring sewer proof to the armorer. Proof {RatPeltCount()}/{ContentSetCatalog.SewerSliceRequiredProofCount}.";
            if (!HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade)) return $"Bring pelts to the armorer. Pelts {RatPeltCount()}/4.";
            if (!HasStoryFlag(StoryFlags.OldRoadTeaserUnlocked)) return "Speak with the Old Road scout.";
            return "Old Road open: descend at Sluice Steps toward Dusk Market.";
        }

        private void DrawPowerImpactEchoes(Rect grid, float cell)
        {
            if (powerImpactEchoes.Count == 0) return;
            float now = Time.time;
            GUI.BeginClip(grid);
            try
            {
                foreach (PowerImpactEcho echo in powerImpactEchoes)
                {
                    if (now < echo.Start) continue;
                    Color accent = echo.Color.ToColor();
                    Vector2 center = new Vector2((echo.X + 0.5f) * cell, (echo.Y + 0.5f) * cell);
                    int intensity = Mathf.Clamp(echo.Intensity, 1, 3);
                    CombatPowerVisualMotif motif = CombatPowerVisualRules.MotifFor(echo.Kind);

                    if (now < echo.ImpactAt)
                    {
                        float travel = Mathf.Max(0.01f, echo.ImpactAt - echo.Start);
                        float t = Mathf.Clamp01((now - echo.Start) / travel);
                        float eased = Mathf.SmoothStep(0f, 1f, t);
                        float size = cell * Mathf.Lerp(1.08f + intensity * 0.08f, 0.30f, eased);
                        float pulse = 0.46f + Mathf.Sin(t * Mathf.PI * (2f + intensity)) * 0.12f;
                        Rect incoming = new Rect(center.x - size * 0.5f, center.y - size * 0.5f, size, size);
                        DrawBorder(incoming, accent.WithAlpha(pulse * 0.72f), intensity >= 3 ? 2 : 1);
                        if (motif == CombatPowerVisualMotif.Generic)
                        {
                            DrawImpactBrackets(incoming, Color.Lerp(accent, cursorWhite, 0.20f).WithAlpha(Mathf.Min(0.84f, pulse + 0.18f)), Mathf.Max(2f, cell * 0.035f));
                        }
                        DrawAnticipationMotif(motif, center, cell, t, accent, intensity);
                        continue;
                    }

                    float duration = Mathf.Max(0.08f, echo.Duration);
                    float tImpact = Mathf.Clamp01((now - echo.ImpactAt) / duration);
                    float fade = 1f - Mathf.SmoothStep(0f, 1f, tImpact);
                    CombatImpactArtPlan artPlan = CombatPowerVisualRules.ImpactArtPlan(echo.Kind, intensity, tImpact);
                    if (echo.StaticStamp)
                    {
                        float stampSize = cell * 0.90f;
                        Rect stamp = new Rect(
                            center.x - stampSize * 0.5f,
                            center.y - stampSize * 0.5f,
                            stampSize,
                            stampSize);
                        bool stampDrawn = artPlan.HasPrimary
                            && TryDrawSpellAnimationAtlasIcon(
                                stamp,
                                artPlan.PrimaryCell,
                                Color.white.WithAlpha(fade * Mathf.Min(0.86f, artPlan.PrimaryOpacity)));
                        if (!stampDrawn)
                        {
                            DrawImpactBrackets(stamp, accent.WithAlpha(fade * 0.88f), Mathf.Max(2f, cell * 0.04f));
                        }
                        continue;
                    }
                    bool secondaryArtDrawn = false;
                    if (artPlan.HasSecondary)
                    {
                        float secondaryFade = 1f - Mathf.SmoothStep(0f, 0.52f, tImpact);
                        float secondarySize = cell * artPlan.SecondaryScale;
                        Rect secondaryArt = new Rect(
                            center.x - secondarySize * 0.5f,
                            center.y - secondarySize * 0.5f,
                            secondarySize,
                            secondarySize);
                        secondaryArtDrawn = TryDrawEpicSpellEffectsAtlasIcon(
                            secondaryArt,
                            artPlan.SecondaryCell,
                            Color.white.WithAlpha(secondaryFade * artPlan.SecondaryOpacity));
                    }

                    bool primaryArtDrawn = false;
                    if (artPlan.HasPrimary)
                    {
                        float primarySize = cell * artPlan.PrimaryScale;
                        Rect primaryArt = new Rect(
                            center.x - primarySize * 0.5f,
                            center.y - primarySize * 0.5f,
                            primarySize,
                            primarySize);
                        primaryArtDrawn = TryDrawSpellAnimationAtlasIcon(
                            primaryArt,
                            artPlan.PrimaryCell,
                            Color.white.WithAlpha(fade * artPlan.PrimaryOpacity));
                    }
                    bool impactArtDrawn = secondaryArtDrawn || primaryArtDrawn;
                    if (!impactArtDrawn)
                    {
                        if (tImpact < 0.16f)
                        {
                            float strike = 1f - Mathf.SmoothStep(0f, 0.16f, tImpact);
                            float flashSize = cell * (0.58f + intensity * 0.20f);
                            Rect flash = new Rect(center.x - flashSize * 0.5f, center.y - flashSize * 0.5f, flashSize, flashSize);
                            DrawRect(flash, Color.Lerp(accent, cursorWhite, 0.70f).WithAlpha(strike * (0.18f + intensity * 0.06f)));
                        }
                        float coreSize = cell * Mathf.Lerp(0.18f, 0.44f, Mathf.Min(1f, tImpact * 2.6f));
                        Rect core = new Rect(center.x - coreSize * 0.5f, center.y - coreSize * 0.5f, coreSize, coreSize);
                        DrawRect(core, Color.Lerp(accent, cursorWhite, 0.42f).WithAlpha(fade * 0.42f));

                        float ringT = Mathf.Clamp01(tImpact * 1.08f);
                        float ringSize = cell * Mathf.Lerp(0.28f, 1.18f + intensity * 0.24f, ringT);
                        Rect ring = new Rect(center.x - ringSize * 0.5f, center.y - ringSize * 0.5f, ringSize, ringSize);
                        Color ringColor = Color.Lerp(accent, cursorWhite, 0.24f);
                        DrawBorder(ring, ringColor.WithAlpha(fade * 0.68f), intensity >= 3 ? 2 : 1);
                    }

                    if (CombatPowerVisualRules.IsMartialMotif(motif) || !impactArtDrawn)
                    {
                        DrawSemanticImpactMotif(motif, center, cell, tImpact, fade, accent, intensity);
                    }
                }
            }
            finally
            {
                GUI.EndClip();
            }
        }

        private void DrawAnticipationMotif(
            CombatPowerVisualMotif motif,
            Vector2 center,
            float cell,
            float progress,
            Color accent,
            int intensity)
        {
            if (motif == CombatPowerVisualMotif.Generic) return;
            float radius = cell * Mathf.Lerp(0.44f + intensity * 0.05f, 0.16f, Mathf.SmoothStep(0f, 1f, progress));
            float thick = Mathf.Max(2f, cell * 0.024f);
            float pulse = 0.50f + Mathf.Sin(progress * Mathf.PI * (3f + intensity)) * 0.16f;
            Color bright = Color.Lerp(accent, cursorWhite, 0.30f).WithAlpha(Mathf.Clamp01(pulse));
            Color shadow = Color.Lerp(accent, retroBlack, 0.64f).WithAlpha(Mathf.Clamp01(pulse * 0.72f));

            switch (motif)
            {
                case CombatPowerVisualMotif.Fire:
                    for (int i = 0; i < 4; i++)
                    {
                        float angle = Mathf.PI * 0.25f + i * Mathf.PI * 0.5f;
                        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                        DrawJaggedPixelLine(center + direction * radius, center + direction * radius * 0.20f, i % 2 == 0 ? bright : shadow, thick, cell * 0.025f);
                    }
                    break;
                case CombatPowerVisualMotif.Frost:
                    DrawPixelLine(center + Vector2.left * radius, center + Vector2.right * radius, bright, thick);
                    DrawPixelLine(center + Vector2.up * radius, center + Vector2.down * radius, bright, thick);
                    DrawPixelLine(center + new Vector2(-radius, -radius) * 0.68f, center + new Vector2(radius, radius) * 0.68f, shadow, Mathf.Max(1f, thick * 0.72f));
                    DrawPixelLine(center + new Vector2(radius, -radius) * 0.68f, center + new Vector2(-radius, radius) * 0.68f, shadow, Mathf.Max(1f, thick * 0.72f));
                    break;
                case CombatPowerVisualMotif.Shock:
                    DrawJaggedPixelLine(center + new Vector2(-radius, -radius * 0.48f), center, bright, thick, cell * 0.045f);
                    DrawJaggedPixelLine(center + new Vector2(radius, radius * 0.48f), center, bright, thick, cell * 0.045f);
                    break;
                case CombatPowerVisualMotif.Holy:
                    DrawPixelLine(center + Vector2.left * radius, center + Vector2.right * radius, bright, thick);
                    DrawPixelLine(center + Vector2.up * radius, center + Vector2.down * radius, bright, thick);
                    break;
                case CombatPowerVisualMotif.Nature:
                    DrawJaggedPixelLine(center + new Vector2(-radius, radius), center, bright, thick, cell * 0.035f);
                    DrawJaggedPixelLine(center + new Vector2(radius, radius), center, bright, thick, cell * 0.035f);
                    break;
                case CombatPowerVisualMotif.Void:
                case CombatPowerVisualMotif.Rift:
                case CombatPowerVisualMotif.Ascendance:
                    DrawRect(new Rect(center.x - thick * 0.55f, center.y - radius, thick * 1.10f, radius * 2f), retroBlack.WithAlpha(0.72f));
                    DrawJaggedPixelLine(center + new Vector2(-thick, -radius), center + new Vector2(thick, radius), bright, thick, cell * 0.035f);
                    break;
                case CombatPowerVisualMotif.Slash:
                case CombatPowerVisualMotif.Shadow:
                    DrawPixelLine(center + new Vector2(-radius, radius * 0.72f), center + new Vector2(radius, -radius * 0.72f), bright, thick * 1.15f);
                    break;
                case CombatPowerVisualMotif.Charge:
                    DrawPixelLine(center + Vector2.left * radius, center + Vector2.right * radius, bright, thick * 1.20f);
                    DrawPixelLine(center + new Vector2(-radius * 0.15f, -radius * 0.34f), center + new Vector2(radius * 0.18f, 0f), bright, thick);
                    DrawPixelLine(center + new Vector2(-radius * 0.15f, radius * 0.34f), center + new Vector2(radius * 0.18f, 0f), bright, thick);
                    break;
                case CombatPowerVisualMotif.Guard:
                    {
                        Vector2 top = center + Vector2.up * radius;
                        Vector2 right = center + Vector2.right * radius * 0.78f;
                        Vector2 bottom = center + Vector2.down * radius;
                        Vector2 left = center + Vector2.left * radius * 0.78f;
                        DrawPixelLine(top, right, bright, thick);
                        DrawPixelLine(right, bottom, shadow, thick);
                        DrawPixelLine(bottom, left, shadow, thick);
                        DrawPixelLine(left, top, bright, thick);
                        break;
                    }
                case CombatPowerVisualMotif.Volley:
                    for (int i = -1; i <= 1; i++)
                    {
                        float offset = i * radius * 0.38f;
                        Vector2 start = center + new Vector2(offset - radius * 0.22f, -radius);
                        Vector2 end = center + new Vector2(offset + radius * 0.22f, radius * 0.56f);
                        DrawPixelLine(start, end, i == 0 ? bright : shadow, i == 0 ? thick * 1.15f : thick);
                    }
                    break;
                case CombatPowerVisualMotif.Smoke:
                    DrawBorder(new Rect(center.x - radius, center.y - radius * 0.60f, radius * 2f, radius * 1.20f), shadow.WithAlpha(0.48f), 1);
                    break;
            }
        }

        private void DrawSemanticImpactMotif(CombatPowerVisualMotif motif, Vector2 center, float cell, float progress, float fade, Color accent, int intensity)
        {
            if (motif == CombatPowerVisualMotif.Generic || fade <= 0f) return;
            float radius = cell * Mathf.Lerp(0.18f, 0.48f + intensity * 0.07f, progress);
            float thick = Mathf.Max(2f, cell * 0.028f);
            Color bright = Color.Lerp(accent, cursorWhite, 0.30f).WithAlpha(fade * 0.72f);
            Color dark = Color.Lerp(accent, retroBlack, 0.72f).WithAlpha(fade * 0.70f);

            switch (motif)
            {
                case CombatPowerVisualMotif.Fire:
                    {
                        int rays = 6 + intensity * 2;
                        float burst = Mathf.Sin(Mathf.Min(1f, progress * 1.28f) * Mathf.PI);
                        for (int i = 0; i < rays; i++)
                        {
                            float angle = i * Mathf.PI * 2f / rays + (i % 2 == 0 ? 0.10f : -0.08f);
                            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                            float rayLength = radius * (0.72f + (i % 3) * 0.14f) * (0.76f + burst * 0.38f);
                            Vector2 start = center + direction * radius * 0.16f;
                            Vector2 end = center + direction * rayLength;
                            DrawPixelLine(start, end, i % 2 == 0 ? bright : Color.Lerp(bright, gold, 0.52f), i % 3 == 0 ? thick * 1.25f : thick);
                        }
                        float lift = radius * (0.42f + progress * 0.62f);
                        for (int i = -1; i <= 1; i++)
                        {
                            float offset = i * radius * 0.32f;
                            Vector2 root = center + new Vector2(offset, radius * 0.28f);
                            Vector2 tip = center + new Vector2(offset * 0.42f + Mathf.Sin(progress * 11f + i) * thick, -lift * (i == 0 ? 1.12f : 0.82f));
                            DrawJaggedPixelLine(root, tip, i == 0 ? bright : dark, i == 0 ? thick * 1.18f : thick, cell * 0.035f);
                        }
                        if (progress > 0.38f)
                        {
                            float scorch = radius * Mathf.Lerp(0.74f, 1.22f, Mathf.InverseLerp(0.38f, 1f, progress));
                            DrawBorder(new Rect(center.x - scorch, center.y - scorch * 0.38f, scorch * 2f, scorch * 0.76f), dark.WithAlpha(fade * 0.44f), 2);
                        }
                        break;
                    }
                case CombatPowerVisualMotif.Frost:
                    {
                        int spokes = 8;
                        for (int i = 0; i < spokes; i++)
                        {
                            float angle = i * Mathf.PI * 2f / spokes;
                            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                            Vector2 side = new Vector2(-direction.y, direction.x);
                            Vector2 tip = center + direction * radius;
                            DrawPixelLine(center, tip, i % 2 == 0 ? bright : dark, i % 2 == 0 ? thick * 1.12f : thick);
                            Vector2 branch = center + direction * radius * 0.62f;
                            DrawPixelLine(branch, branch - direction * radius * 0.18f + side * radius * 0.17f, bright.WithAlpha(fade * 0.58f), Mathf.Max(1f, thick * 0.72f));
                            DrawPixelLine(branch, branch - direction * radius * 0.18f - side * radius * 0.17f, bright.WithAlpha(fade * 0.58f), Mathf.Max(1f, thick * 0.72f));
                        }
                        break;
                    }
                case CombatPowerVisualMotif.Void:
                    {
                        float collapse = cell * Mathf.Lerp(0.54f, 0.18f, progress);
                        Rect rim = new Rect(center.x - collapse * 0.5f, center.y - collapse * 0.5f, collapse, collapse);
                        DrawRect(Pad(rim, collapse * 0.22f), retroBlack.WithAlpha(fade * 0.78f));
                        DrawBorder(rim, bright, intensity >= 3 ? 3 : 2);
                        break;
                    }
                case CombatPowerVisualMotif.Rift:
                case CombatPowerVisualMotif.Ascendance:
                    {
                        float slit = radius * (motif == CombatPowerVisualMotif.Ascendance ? 1.34f : 1.08f);
                        DrawRect(new Rect(center.x - thick * 0.65f, center.y - slit, thick * 1.3f, slit * 2f), dark);
                        DrawPixelLine(new Vector2(center.x - thick * 2.2f, center.y - slit), new Vector2(center.x + thick * 1.2f, center.y - slit * 0.45f), bright, thick);
                        DrawPixelLine(new Vector2(center.x + thick * 2.0f, center.y + slit), new Vector2(center.x - thick * 1.0f, center.y + slit * 0.42f), bright, thick);
                        if (motif == CombatPowerVisualMotif.Ascendance)
                        {
                            DrawPixelLine(new Vector2(center.x - radius * 0.52f, center.y + radius * 0.32f), new Vector2(center.x, center.y - radius), bright, thick);
                            DrawPixelLine(new Vector2(center.x + radius * 0.52f, center.y + radius * 0.32f), new Vector2(center.x, center.y - radius), bright, thick);
                        }
                        break;
                    }
                case CombatPowerVisualMotif.Shock:
                    DrawJaggedPixelLine(center, center + new Vector2(radius, -radius * 0.72f), bright, thick, cell * 0.055f);
                    DrawJaggedPixelLine(center, center + new Vector2(-radius, radius * 0.66f), bright, thick, cell * 0.055f);
                    DrawJaggedPixelLine(center, center + new Vector2(radius * 0.62f, radius), bright, thick, cell * 0.055f);
                    break;
                case CombatPowerVisualMotif.Holy:
                    DrawPixelLine(new Vector2(center.x - radius, center.y), new Vector2(center.x + radius, center.y), bright, thick);
                    DrawPixelLine(new Vector2(center.x, center.y - radius), new Vector2(center.x, center.y + radius), bright, thick);
                    DrawBorder(new Rect(center.x - radius * 0.58f, center.y - radius * 0.58f, radius * 1.16f, radius * 1.16f), bright.WithAlpha(fade * 0.48f), 1);
                    break;
                case CombatPowerVisualMotif.Nature:
                    DrawJaggedPixelLine(center, center + new Vector2(-radius, radius * 0.72f), bright, thick, cell * 0.045f);
                    DrawJaggedPixelLine(center, center + new Vector2(radius, radius * 0.68f), bright, thick, cell * 0.045f);
                    DrawJaggedPixelLine(center, center + new Vector2(-radius * 0.72f, -radius * 0.42f), dark, thick, cell * 0.045f);
                    break;
                case CombatPowerVisualMotif.Slash:
                    DrawPixelLine(center + new Vector2(-radius, radius * 0.72f), center + new Vector2(radius, -radius * 0.72f), bright, thick * 1.15f);
                    DrawPixelLine(center + new Vector2(-radius * 0.72f, -radius), center + new Vector2(radius * 0.72f, radius), bright.WithAlpha(fade * 0.56f), thick);
                    break;
                case CombatPowerVisualMotif.Charge:
                    DrawPixelLine(new Vector2(center.x - radius, center.y), new Vector2(center.x + radius, center.y), bright, thick * 1.25f);
                    DrawPixelLine(new Vector2(center.x - radius * 0.18f, center.y - radius * 0.30f), new Vector2(center.x + radius * 0.18f, center.y), bright, thick);
                    DrawPixelLine(new Vector2(center.x - radius * 0.18f, center.y + radius * 0.30f), new Vector2(center.x + radius * 0.18f, center.y), bright, thick);
                    break;
                case CombatPowerVisualMotif.Guard:
                    {
                        Vector2 top = center + Vector2.up * radius;
                        Vector2 right = center + Vector2.right * radius * 0.76f;
                        Vector2 bottom = center + Vector2.down * radius;
                        Vector2 left = center + Vector2.left * radius * 0.76f;
                        DrawPixelLine(top, right, bright, thick * 1.15f);
                        DrawPixelLine(right, bottom, dark, thick * 1.15f);
                        DrawPixelLine(bottom, left, dark, thick * 1.15f);
                        DrawPixelLine(left, top, bright, thick * 1.15f);
                        DrawPixelLine(center + Vector2.left * radius * 0.42f, center + Vector2.right * radius * 0.42f, bright.WithAlpha(fade * 0.58f), thick);
                        break;
                    }
                case CombatPowerVisualMotif.Volley:
                    for (int i = -1; i <= 1; i++)
                    {
                        float offset = i * radius * 0.34f;
                        Vector2 from = center + new Vector2(offset - radius * 0.34f, -radius);
                        Vector2 to = center + new Vector2(offset + radius * 0.20f, radius);
                        DrawPixelLine(from, to, i == 0 ? bright : dark, i == 0 ? thick * 1.15f : thick);
                        Vector2 direction = (to - from).normalized;
                        Vector2 side = new Vector2(-direction.y, direction.x);
                        DrawPixelLine(to, to - direction * radius * 0.22f + side * radius * 0.12f, bright.WithAlpha(fade * 0.68f), Mathf.Max(1f, thick * 0.76f));
                        DrawPixelLine(to, to - direction * radius * 0.22f - side * radius * 0.12f, bright.WithAlpha(fade * 0.68f), Mathf.Max(1f, thick * 0.76f));
                    }
                    break;
                case CombatPowerVisualMotif.Shadow:
                    DrawPixelLine(center + new Vector2(-radius, radius * 0.52f), center + new Vector2(radius, -radius * 0.62f), dark, thick * 1.5f);
                    DrawPixelLine(center + new Vector2(-radius * 0.72f, radius), center + new Vector2(radius * 0.56f, -radius), bright, thick);
                    break;
                case CombatPowerVisualMotif.Smoke:
                    DrawBorder(new Rect(center.x - radius, center.y - radius * 0.62f, radius * 2f, radius * 1.24f), dark.WithAlpha(fade * 0.46f), 2);
                    break;
            }
        }

        private void DrawEnemyIntentCue(Rect grid, float cell, CombatUnit enemy)
        {
            CombatUnit focus = EnemyIntentFocus(enemy);
            if (focus == null || focus.Hp <= 0) return;

            Rect tile = new Rect(grid.x + focus.X * cell, grid.y + focus.Y * cell, cell, cell);
            Rect edge = Pad(tile, cell * 0.035f);
            bool supportFocus = focus.Side == enemy.Side;
            Color accent = supportFocus ? teal : gold;
            float pulse = state.ReducedMotion ? 0.48f : 0.5f + Mathf.Sin(Time.time * 6f) * 0.5f;
            float corner = Mathf.Max(7f, cell * 0.18f);
            float thick = Mathf.Max(2f, cell * 0.025f);

            DrawRect(new Rect(edge.x, edge.y, corner, thick), accent);
            DrawRect(new Rect(edge.x, edge.y, thick, corner), accent);
            DrawRect(new Rect(edge.xMax - corner, edge.y, corner, thick), accent);
            DrawRect(new Rect(edge.xMax - thick, edge.y, thick, corner), accent);
            DrawRect(new Rect(edge.x, edge.yMax - thick, corner, thick), accent);
            DrawRect(new Rect(edge.x, edge.yMax - corner, thick, corner), accent);
            DrawRect(new Rect(edge.xMax - corner, edge.yMax - thick, corner, thick), accent);
            DrawRect(new Rect(edge.xMax - thick, edge.yMax - corner, thick, corner), accent);
        }

        private void DrawPowerCastAuras(Rect grid, float cell)
        {
            if (powerCastAuras.Count == 0) return;
            float now = Time.time;
            GUI.BeginClip(grid);
            try
            {
                foreach (PowerCastAura aura in powerCastAuras)
                {
                    if (now < aura.Start || now > aura.Start + aura.Duration) continue;
                    Color accent = aura.Color.ToColor();
                    Color focusColor = aura.Focused ? Color.Lerp(accent, gold, 0.58f) : accent;
                    CombatPowerVisualMotif motif = CombatPowerVisualRules.MotifFor(aura.Kind);
                    bool ritualPresentation = CombatPowerVisualRules.UsesRitualCastPresentation(aura.Kind);
                    Vector2 source = new Vector2((aura.SourceX + 0.5f) * cell, (aura.SourceY + 0.5f) * cell);
                    Vector2 target = new Vector2((aura.TargetX + 0.5f) * cell, (aura.TargetY + 0.5f) * cell);
                    float chargeSpan = Mathf.Max(0.05f, aura.ImpactAt - aura.Start);
                    float charge = Mathf.Clamp01((now - aura.Start) / chargeSpan);
                    float fade = now <= aura.ImpactAt
                        ? 1f
                        : 1f - Mathf.Clamp01((now - aura.ImpactAt) / Mathf.Max(0.08f, aura.Start + aura.Duration - aura.ImpactAt));
                    float wave = 0.70f + Mathf.Sin((now - aura.Start) * (12f + aura.Intensity * 2f)) * 0.22f;

                    int anticipationCell = CombatPowerVisualRules.AnticipationAtlasCell(motif);
                    bool usesAnticipationArt = ritualPresentation && anticipationCell >= 0 && (aura.Intensity >= 2 || aura.Focused);
                    if (!usesAnticipationArt)
                    {
                        float outerSize = cell * Mathf.Lerp(0.92f, 0.52f, Mathf.SmoothStep(0f, 1f, charge));
                        float innerSize = cell * Mathf.Lerp(0.24f, 0.70f, Mathf.SmoothStep(0f, 1f, charge));
                        Rect outer = new Rect(source.x - outerSize * 0.5f, source.y - outerSize * 0.5f, outerSize, outerSize);
                        Rect inner = new Rect(source.x - innerSize * 0.5f, source.y - innerSize * 0.5f, innerSize, innerSize);
                        DrawBorder(outer, focusColor.WithAlpha(fade * wave * 0.72f), aura.Intensity >= 3 ? 2 : 1);
                        DrawImpactBrackets(inner, focusColor.WithAlpha(fade * 0.82f), Mathf.Max(2f, cell * 0.03f));
                    }

                    if (usesAnticipationArt)
                    {
                        float artScale = CombatPowerVisualRules.AnticipationArtScale(motif, aura.Intensity, charge);
                        float artSize = cell * artScale;
                        Rect art = new Rect(source.x - artSize * 0.5f, source.y - artSize * 0.5f, artSize, artSize);
                        float artAlpha = fade * CombatPowerVisualRules.AnticipationOpacity(motif, aura.Intensity, charge);
                        TryDrawEpicSpellEffectsAtlasIcon(art, anticipationCell, Color.white.WithAlpha(artAlpha));
                    }

                    if (!usesAnticipationArt)
                    {
                        DrawSemanticCastMotif(motif, source, target, cell, charge, fade, focusColor, aura.Intensity);
                    }
                }
            }
            finally
            {
                GUI.EndClip();
            }
        }

        private void DrawSemanticCastMotif(CombatPowerVisualMotif motif, Vector2 source, Vector2 target, float cell, float charge, float fade, Color accent, int intensity)
        {
            if (motif == CombatPowerVisualMotif.Generic || fade <= 0f) return;
            float radius = cell * Mathf.Lerp(0.20f, 0.39f + intensity * 0.035f, charge);
            float thick = Mathf.Max(2f, cell * 0.026f);
            Color bright = Color.Lerp(accent, cursorWhite, 0.34f).WithAlpha(fade * 0.68f);
            Color dark = Color.Lerp(accent, retroBlack, 0.66f).WithAlpha(fade * 0.62f);

            switch (motif)
            {
                case CombatPowerVisualMotif.Fire:
                    {
                        int embers = 5 + intensity;
                        float rotation = charge * Mathf.PI * 1.65f;
                        for (int i = 0; i < embers; i++)
                        {
                            float angle = rotation + i * Mathf.PI * 2f / embers;
                            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                            Vector2 emberPoint = source + direction * radius;
                            float size = Mathf.Max(2f, cell * (i % 2 == 0 ? 0.050f : 0.034f));
                            DrawRect(new Rect(emberPoint.x - size * 0.5f, emberPoint.y - size * 0.5f, size, size), (i % 2 == 0 ? bright : Color.Lerp(bright, gold, 0.58f)).WithAlpha(fade * 0.78f));
                            DrawPixelLine(emberPoint, source + direction * radius * 0.42f, dark.WithAlpha(fade * 0.42f), Mathf.Max(1f, thick * 0.62f));
                        }
                        float crown = radius * Mathf.Lerp(0.48f, 1.08f, charge);
                        DrawJaggedPixelLine(source + new Vector2(-crown * 0.62f, crown * 0.42f), source + new Vector2(0f, -crown), bright, thick, cell * 0.035f);
                        DrawJaggedPixelLine(source + new Vector2(crown * 0.62f, crown * 0.42f), source + new Vector2(0f, -crown), Color.Lerp(bright, gold, 0.42f), thick, cell * 0.035f);
                        break;
                    }
                case CombatPowerVisualMotif.Frost:
                    {
                        int spokes = 6;
                        for (int i = 0; i < spokes; i++)
                        {
                            float angle = i * Mathf.PI * 2f / spokes + charge * 0.42f;
                            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                            Vector2 side = new Vector2(-direction.y, direction.x);
                            Vector2 tip = source + direction * radius;
                            DrawPixelLine(source, tip, bright, thick);
                            Vector2 branch = source + direction * radius * 0.64f;
                            DrawPixelLine(branch, branch - direction * radius * 0.18f + side * radius * 0.14f, dark, Mathf.Max(1f, thick * 0.70f));
                            DrawPixelLine(branch, branch - direction * radius * 0.18f - side * radius * 0.14f, dark, Mathf.Max(1f, thick * 0.70f));
                        }
                        break;
                    }
                case CombatPowerVisualMotif.Void:
                    {
                        float inward = radius * Mathf.Lerp(1.25f, 0.54f, charge);
                        Rect rim = new Rect(source.x - inward, source.y - inward, inward * 2f, inward * 2f);
                        DrawBorder(rim, bright, intensity >= 3 ? 2 : 1);
                        DrawRect(Pad(rim, inward * 0.72f), retroBlack.WithAlpha(fade * charge * 0.62f));
                        break;
                    }
                case CombatPowerVisualMotif.Rift:
                case CombatPowerVisualMotif.Ascendance:
                    {
                        float height = radius * (motif == CombatPowerVisualMotif.Ascendance ? 1.80f : 1.42f);
                        DrawRect(new Rect(source.x - thick * 0.55f, source.y - height, thick * 1.1f, height * 2f), dark);
                        DrawJaggedPixelLine(new Vector2(source.x - thick, source.y - height), new Vector2(source.x + thick, source.y + height), bright, thick, cell * 0.045f);
                        if (motif == CombatPowerVisualMotif.Ascendance)
                        {
                            DrawPixelLine(source + new Vector2(-radius, radius * 0.52f), source + new Vector2(0f, -radius), bright, thick);
                            DrawPixelLine(source + new Vector2(radius, radius * 0.52f), source + new Vector2(0f, -radius), bright, thick);
                        }
                        break;
                    }
                case CombatPowerVisualMotif.Shock:
                    DrawJaggedPixelLine(source + new Vector2(-radius, 0f), source + new Vector2(radius, 0f), bright, thick, cell * 0.05f);
                    DrawJaggedPixelLine(source + new Vector2(0f, -radius), source + new Vector2(0f, radius), bright, thick, cell * 0.05f);
                    break;
                case CombatPowerVisualMotif.Holy:
                    DrawPixelLine(source + new Vector2(-radius, 0f), source + new Vector2(radius, 0f), bright, thick);
                    DrawPixelLine(source + new Vector2(0f, -radius), source + new Vector2(0f, radius), bright, thick);
                    break;
                case CombatPowerVisualMotif.Nature:
                    DrawJaggedPixelLine(source, source + new Vector2(-radius, radius), bright, thick, cell * 0.04f);
                    DrawJaggedPixelLine(source, source + new Vector2(radius, radius), bright, thick, cell * 0.04f);
                    DrawJaggedPixelLine(source, source + new Vector2(0f, -radius), dark, thick, cell * 0.04f);
                    break;
                case CombatPowerVisualMotif.Slash:
                    DrawPixelLine(source + new Vector2(-radius, radius * 0.62f), source + new Vector2(radius, -radius * 0.62f), bright, thick * 1.15f);
                    break;
                case CombatPowerVisualMotif.Charge:
                    DrawPixelLine(source, target, bright, thick * 1.3f);
                    DrawImpactBrackets(new Rect(target.x - radius * 0.52f, target.y - radius * 0.52f, radius * 1.04f, radius * 1.04f), bright, thick);
                    break;
                case CombatPowerVisualMotif.Guard:
                    {
                        Vector2 top = source + Vector2.up * radius;
                        Vector2 right = source + Vector2.right * radius * 0.72f;
                        Vector2 bottom = source + Vector2.down * radius;
                        Vector2 left = source + Vector2.left * radius * 0.72f;
                        DrawPixelLine(top, right, bright, thick);
                        DrawPixelLine(right, bottom, dark, thick);
                        DrawPixelLine(bottom, left, dark, thick);
                        DrawPixelLine(left, top, bright, thick);
                        break;
                    }
                case CombatPowerVisualMotif.Volley:
                    {
                        Vector2 direction = (target - source).normalized;
                        if (direction.sqrMagnitude < 0.01f) direction = Vector2.right;
                        Vector2 side = new Vector2(-direction.y, direction.x);
                        for (int i = -1; i <= 1; i++)
                        {
                            Vector2 offset = side * radius * i * 0.34f;
                            DrawPixelLine(source + offset - direction * radius * 0.42f, source + offset + direction * radius, i == 0 ? bright : dark, i == 0 ? thick * 1.12f : thick);
                        }
                        break;
                    }
                case CombatPowerVisualMotif.Shadow:
                    DrawPixelLine(source + new Vector2(-radius, radius * 0.74f), source + new Vector2(radius, -radius * 0.74f), dark, thick * 1.45f);
                    DrawPixelLine(source + new Vector2(-radius * 0.65f, -radius), source + new Vector2(radius * 0.58f, radius), bright, thick);
                    break;
                case CombatPowerVisualMotif.Smoke:
                    DrawBorder(new Rect(source.x - radius, source.y - radius * 0.62f, radius * 2f, radius * 1.24f), dark.WithAlpha(fade * 0.42f), 1);
                    break;
            }
        }

        private void DrawImpactBrackets(Rect rect, Color color, float thickness)
        {
            float length = Mathf.Min(rect.width, rect.height) * 0.24f;
            DrawRect(new Rect(rect.x, rect.y, length, thickness), color);
            DrawRect(new Rect(rect.x, rect.y, thickness, length), color);
            DrawRect(new Rect(rect.xMax - length, rect.y, length, thickness), color);
            DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, length), color);
            DrawRect(new Rect(rect.x, rect.yMax - thickness, length, thickness), color);
            DrawRect(new Rect(rect.x, rect.yMax - length, thickness, length), color);
            DrawRect(new Rect(rect.xMax - length, rect.yMax - thickness, length, thickness), color);
            DrawRect(new Rect(rect.xMax - thickness, rect.yMax - length, thickness, length), color);
        }

        private int MidgaardRouteActiveStep()
        {
            if (state == null || state.Depth > 1) return 3;
            if (!HasStoryFlag(StoryFlags.MidgaardRatQuestGiven)) return 0;
            if (!ContentSetCatalog.IsFullPrototype(activeContentSet) && !HasStoryFlag(StoryFlags.SewerCisternDenCleared)) return 1;
            if (ContentSetCatalog.IsFullPrototype(activeContentSet) && !HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade) && !MidgaardRatPeltsReady()) return 1;
            if (!HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade)) return 2;
            return 3;
        }

        private int MidgaardRouteProgressionIcon()
        {
            int step = MidgaardRouteActiveStep();
            if (step == 0) return 4;
            if (step == 1) return 13;
            if (step == 2) return 8;
            return 16;
        }

        private bool MidgaardRouteStepComplete(int index)
        {
            switch (index)
            {
                case 0: return HasStoryFlag(StoryFlags.MidgaardRatQuestGiven);
                case 1: return HasStoryFlag(StoryFlags.MidgaardRatQuestGiven) && (HasStoryFlag(StoryFlags.SewerCisternDenCleared) || MidgaardRatPeltsReady());
                case 2: return HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade);
                case 3: return HasStoryFlag(StoryFlags.OldRoadTeaserUnlocked) || state != null && state.Depth > 1;
                default: return false;
            }
        }

        private string MidgaardWayfindingLine()
        {
            if (state?.Map == null || state.Depth != 1) return "Waypoints wake after the party reaches Midgaard.";
            string next = TryCurrentMidgaardObjectiveType(out ObjectType objective)
                ? $"Next {MidgaardObjectiveLabel(objective)} {MidgaardDirectionTo(objective)}. "
                : "";
            string diner = MidgaardDirectionTo(ObjectType.Diner);
            string sewer = MidgaardDirectionTo(ObjectType.Sewer);
            string mira = MidgaardDirectionTo(ObjectType.TempleHealer);
            string captain = MidgaardDirectionTo(ObjectType.GateCaptain);
            string gates = $"{MidgaardDirectionTo(ObjectType.WestGate)} W gate / {MidgaardDirectionTo(ObjectType.EastGate)} E gate";
            return $"{next}Mira {mira}, Captain {captain}, Diner {diner}, Sewer {sewer}, {gates}.";
        }

        private string MidgaardWayfindingCompactLine()
        {
            if (state?.Map == null || state.Depth != 1) return "Midgaard waypoints wake after arrival.";
            if (TryCurrentMidgaardObjectiveType(out ObjectType objective))
            {
                return $"Next: {MidgaardObjectiveLabel(objective)} {MidgaardDirectionTo(objective)}.";
            }

            return $"Mira {MidgaardDirectionTo(ObjectType.TempleHealer)} / Sewer {MidgaardDirectionTo(ObjectType.Sewer)} / West {MidgaardDirectionTo(ObjectType.WestGate)} / East {MidgaardDirectionTo(ObjectType.EastGate)}";
        }

        private bool TryCurrentMidgaardObjectiveType(out ObjectType type)
        {
            type = ObjectType.Market;
            if (state == null || state.Depth != 1) return false;
            if (ContentSetCatalog.ShowPrototypeScaffold(activeContentSet) && HasStoryFlag(StoryFlags.MidgaardLampRoundStarted) && !HasStoryFlag(StoryFlags.MidgaardLampRoundComplete))
            {
                if (!HasStoryFlag(StoryFlags.MidgaardLampRoundMarket)) type = ObjectType.MarketClerk;
                else if (!HasStoryFlag(StoryFlags.MidgaardLampRoundDiner)) type = ObjectType.Diner;
                else if (!HasStoryFlag(StoryFlags.MidgaardLampRoundTavern)) type = ObjectType.TavernKeeper;
                else type = ObjectType.TempleHealer;
                return true;
            }
            if (ContentSetCatalog.ShowPrototypeScaffold(activeContentSet) && HasStoryFlag(StoryFlags.MidgaardGateSurveyStarted) && !HasStoryFlag(StoryFlags.MidgaardGateSurveyComplete))
            {
                if (!HasStoryFlag(StoryFlags.MidgaardGateSurveyWest)) type = ObjectType.WestGate;
                else if (!HasStoryFlag(StoryFlags.MidgaardGateSurveyEast)) type = ObjectType.EastGate;
                else type = ObjectType.GateCaptain;
                return true;
            }
            if (!HasStoryFlag(StoryFlags.MidgaardRatQuestGiven))
            {
                type = ObjectType.KingHall;
                return true;
            }
            if (!ContentSetCatalog.IsFullPrototype(activeContentSet) && !HasStoryFlag(StoryFlags.SewerCisternDenCleared))
            {
                type = ObjectType.Sewer;
                return true;
            }
            if (ContentSetCatalog.IsFullPrototype(activeContentSet) && !HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade) && !MidgaardRatPeltsReady())
            {
                type = ObjectType.Sewer;
                return true;
            }
            if (!HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade) || !HasStoryFlag(StoryFlags.SewerRewardClaimed))
            {
                type = ObjectType.Armorer;
                return true;
            }
            if (!HasStoryFlag(StoryFlags.OldRoadTeaserUnlocked))
            {
                type = ObjectType.OldRoadScout;
                return true;
            }
            if (!HasStoryFlag(StoryFlags.KoboldKingDefeated)
                && ContentSetCatalog.AllowKoboldChapter(activeContentSet, state.StoryFlags)
                && state.Map?.FindObjectById(OldRoadDescentId) != null)
            {
                type = ObjectType.Stairs;
                return true;
            }
            return false;
        }

        private string MidgaardObjectiveLabel(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.KingHall: return "King's Hall";
                case ObjectType.Sewer: return "Sewer";
                case ObjectType.Armorer: return "Armorer";
                case ObjectType.OldRoadScout: return "Old Road Scout";
                case ObjectType.Stairs: return "Sluice Steps";
                case ObjectType.MarketClerk: return "Market Clerk";
                case ObjectType.TempleHealer: return "Mira";
                case ObjectType.TavernKeeper: return "Tavern Keeper";
                case ObjectType.GateCaptain: return "Gate Captain";
                default: return ObjectName(type);
            }
        }

        private string MidgaardDirectionTo(ObjectType type)
        {
            MapObject target = type == ObjectType.Stairs
                ? state?.Map?.FindObjectById(OldRoadDescentId)
                : state?.Map?.Objects?.FirstOrDefault(o => o.Type == type);
            if (target == null) return "?";
            int dx = target.X - state.PlayerX;
            int dy = target.Y - state.PlayerY;
            int distance = Mathf.Abs(dx) + Mathf.Abs(dy);
            if (distance == 0) return "here";
            string ns = dy < 0 ? "N" : dy > 0 ? "S" : "";
            string ew = dx < 0 ? "W" : dx > 0 ? "E" : "";
            return $"{ns}{ew}{distance}";
        }

        private bool ShouldShowKoboldRouteTracker()
        {
            if (state == null) return false;
            if (state.Depth == 2) return true;
            return HasStoryFlag(StoryFlags.KoboldAmbushSprung)
                || HasStoryFlag(StoryFlags.KoboldAmbushSurvived)
                || HasStoryFlag(StoryFlags.KoboldCaveFound)
                || HasStoryFlag(StoryFlags.KoboldCaveCleared)
                || HasStoryFlag(StoryFlags.KoboldKingDefeated);
        }

        private void DrawKoboldRouteTracker(Rect rect)
        {
            if (rect.height < 54f) return;
            DrawRect(rect, Hex("080b0d", 0.62f));
            DrawBorder(rect, gold.WithAlpha(0.54f), 1);
            Rect icon = new Rect(rect.x + 8f, rect.y + 8f, 30f, 30f);
            if (!TryDrawWorldMapProgressionOverlayAtlasIcon(icon, KoboldRouteProgressionIcon(), Color.white.WithAlpha(0.90f))
                && !TryDrawKoboldRouteArtIcon(icon, KoboldRouteMarkerIndex(), Color.white.WithAlpha(0.92f)))
            {
                DrawTinyUiIcon(icon, "enemy", gold);
            }
            GUI.Label(new Rect(rect.x + 46f, rect.y + 7f, rect.width - 56f, 16f), "Kobold Smoke", CenterLeftStyle(12, gold));
            GUI.Label(new Rect(rect.x + 46f, rect.y + 23f, rect.width - 56f, 15f), FitText(KoboldRouteStatusLine(), rect.width - 56f, CenterLeftStyle(9, muted)), CenterLeftStyle(9, muted));

            string[] labels = { "Ambush", "Cave", "King", "Road" };
            float chipGap = 4f;
            float chipW = Mathf.Max(44f, (rect.width - 16f - chipGap * 3f) / 4f);
            float chipY = rect.y + Mathf.Min(52f, rect.height - 28f);
            int active = KoboldRouteActiveStep();
            for (int i = 0; i < labels.Length; i++)
            {
                bool done = KoboldRouteStepComplete(i);
                Rect chip = new Rect(rect.x + 8f + i * (chipW + chipGap), chipY, chipW, 22f);
                Color accent = done ? teal : i == active ? gold : line;
                DrawRect(chip, Hex("151b20", done ? 0.86f : 0.62f));
                DrawBorder(chip, accent.WithAlpha(i == active ? 0.92f : 0.62f), i == active ? 2 : 1);
                GUI.Label(new Rect(chip.x + 4f, chip.y + 2f, chip.width - 8f, chip.height - 2f), FitText(done ? labels[i] + " ok" : labels[i], chip.width - 8f, CenterStyle(8, done ? teal : ink)), CenterStyle(8, done ? teal : ink));
            }
        }

        private string KoboldRouteStatusLine()
        {
            if (state == null) return "";
            if (state.Depth > 2) return "Route complete; the road points deeper.";
            if (HasStoryFlag(StoryFlags.KoboldKingDefeated)) return "King defeated. Find stairs toward the Bone Road.";
            if (HasStoryFlag(StoryFlags.KoboldCaveCleared)) return "Return to the cave mouth for the king's hall.";
            if (HasStoryFlag(StoryFlags.KoboldAmbushSurvived)) return "Find the smoke cave behind the market charms.";
            if (HasStoryFlag(StoryFlags.KoboldAmbushSprung)) return "Ambush sprung. Hold formation.";
            return "Reach Dusk Market and watch for bone whistles.";
        }

        private int KoboldRouteActiveStep()
        {
            if (state == null || state.Depth > 2 || HasStoryFlag(StoryFlags.KoboldKingDefeated)) return 3;
            if (HasStoryFlag(StoryFlags.KoboldCaveCleared)) return 2;
            if (HasStoryFlag(StoryFlags.KoboldAmbushSurvived)) return 1;
            return 0;
        }

        private int KoboldRouteProgressionIcon()
        {
            int step = KoboldRouteActiveStep();
            if (step == 0) return 17;
            if (step == 1) return 17;
            if (step == 2) return 7;
            return 16;
        }

        private bool KoboldRouteStepComplete(int index)
        {
            switch (index)
            {
                case 0: return HasStoryFlag(StoryFlags.KoboldAmbushSurvived);
                case 1: return HasStoryFlag(StoryFlags.KoboldCaveCleared);
                case 2: return HasStoryFlag(StoryFlags.KoboldKingDefeated);
                case 3: return state != null && state.Depth > 2;
                default: return false;
            }
        }

        private void DrawMemberCard(Rect rect, PartyMember member, bool active)
        {
            bool compact = rect.height < 40f;
            Color accent = MemberColor(member);
            DrawRect(rect, active ? Hex("223238") : Hex("151b20"));
            DrawBorder(rect, active ? gold : line, active ? 2 : 1);
            DrawRect(new Rect(rect.x + 2, rect.y + 2, 3, rect.height - 4), accent);

            float portraitSize = Mathf.Clamp(rect.height - 8f, compact ? 30f : 42f, 70f);
            Rect portrait = new Rect(rect.x + 10, rect.y + (rect.height - portraitSize) * 0.5f, portraitSize, portraitSize);
            DrawMiniRolePortrait(portrait, member, accent);
            float classBadgeSize = Mathf.Clamp(portraitSize * 0.52f, 14f, 22f);
            DrawClassIcon(new Rect(portrait.xMax - classBadgeSize + 2f, portrait.y - 2f, classBadgeSize, classBadgeSize), member.ClassKey, member.Role, accent);

            float meterW = Mathf.Clamp(rect.width * 0.28f, compact ? 86f : 112f, compact ? 118f : 150f);
            float meterX = rect.xMax - meterW - 10f;
            float textX = portrait.xMax + 8f;
            float textW = Mathf.Max(90f, meterX - textX - 8f);
            GUIStyle nameStyle = CenterLeftStyle(compact ? 10 : rect.height < 54f ? 12 : 13, ink);
            GUIStyle metaStyle = CenterLeftStyle(compact ? 8 : rect.height < 54f ? 10 : 11, muted);
            GUI.Label(new Rect(textX, rect.y + (compact ? 2 : 4), textW, compact ? 12 : 16), FitText(member.Name, textW, nameStyle), nameStyle);
            string roleLine = $"L{member.Level} {DisplayRace(member.Race)} {DisplayClass(member.ClassKey)}";
            GUI.Label(new Rect(textX, rect.y + (compact ? 14 : rect.height < 42f ? 19 : 22), textW, compact ? 10 : 13), FitText(roleLine, textW, metaStyle), metaStyle);

            float meterH = compact ? 5f : rect.height < 52f ? 6f : 7f;
            float meterY = compact ? rect.y + 5f : rect.y + Mathf.Max(6f, (rect.height - (meterH * 3f + 7f)) * 0.5f);
            DrawLabeledMeter(new Rect(meterX, meterY, meterW, meterH), "H", member.Hp, member.MaxHp, blood);
            DrawLabeledMeter(new Rect(meterX, meterY + meterH + (compact ? 3f : 4f), meterW, meterH), "M", member.Mana, member.MaxMana, teal);
            if (!compact && rect.height >= 58f)
            {
                GUI.Label(new Rect(textX, rect.y + 39f, textW, 13f), FitText(PartyCombatCardLine(member), textW, CenterLeftStyle(9, muted)), CenterLeftStyle(9, muted));
            }
        }

        private string PartyCombatCardLine(PartyMember member)
        {
            if (member == null) return "";
            List<string> parts = new List<string>();
            if (member.Hp <= 0) parts.Add("down");
            else if (member.Hp * 2 <= Mathf.Max(1, member.MaxHp)) parts.Add("hurt");
            if (!string.IsNullOrEmpty(member.Spell) && member.MaxMana > 0)
            {
                parts.Add($"{SpellCraftLabel(member.Spell)} MP {member.Mana}");
            }
            int unspent = Mathf.Max(0, member.StatPoints) + Mathf.Max(0, member.SkillPoints);
            if (unspent > 0) parts.Add($"unspent {unspent}");
            if (parts.Count == 0) parts.Add($"{BestSkillLabel(member)} {BestSkillValue(member)}");
            return string.Join(" / ", parts.Take(2).ToArray());
        }

        private string GearShortLine(PartyMember member)
        {
            string weapon = string.IsNullOrEmpty(member.WeaponName) ? StartingWeapon(member.Role) : member.WeaponName;
            string armor = string.IsNullOrEmpty(member.ArmorName) ? StartingArmor(member.Role) : member.ArmorName;
            return $"{TrimGearName(weapon)} / {TrimGearName(armor)}";
        }

        private string TrimGearName(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Length <= 28 ? text : text.Substring(0, 25) + "...";
        }

        private void DrawMiniRolePortrait(Rect rect, PartyMember member, Color accent)
        {
            DrawRect(rect, Hex("050708", 0.82f));
            DrawBorder(rect, accent, 1);
            Rect inner = Pad(rect, rect.width * 0.18f);
            if (!TryDrawAtlasPartyPortrait(rect, member))
            {
                DrawMiniRoleGlyph(inner, member.Role, accent);
            }
            Rect sigil = new Rect(rect.x + rect.width * 0.58f, rect.y + rect.height * 0.58f, rect.width * 0.28f, rect.height * 0.24f);
            DrawSigil(sigil, member.Sigil, ink);
        }

        private void DrawMiniRoleGlyph(Rect rect, string role, Color accent)
        {
            DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.18f, rect.width * 0.28f, rect.height * 0.68f), accent);
            DrawRect(new Rect(rect.x + rect.width * 0.40f, rect.y, rect.width * 0.20f, rect.height * 0.24f), Hex("d9a67b"));
            if (role == "shield" || role == "ward")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.06f, rect.y + rect.height * 0.38f, rect.width * 0.28f, rect.height * 0.38f), Hex("a9b0a2"));
            }
            else if (role == "pike")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.76f, rect.y - rect.height * 0.04f, rect.width * 0.08f, rect.height * 0.96f), ink);
            }
            else if (role == "bow")
            {
                DrawBorder(new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.16f, rect.width * 0.26f, rect.height * 0.62f), gold, 1);
            }
            else if (role == "knife")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.38f, rect.width * 0.26f, rect.height * 0.08f), ink);
            }
            else if (role == "mender")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.70f, rect.y + rect.height * 0.16f, rect.width * 0.08f, rect.height * 0.64f), teal);
                DrawRect(new Rect(rect.x + rect.width * 0.60f, rect.y + rect.height * 0.38f, rect.width * 0.28f, rect.height * 0.08f), teal);
            }
            else if (role == "ember" || role == "hex")
            {
                Color c = role == "ember" ? ember : violet;
                DrawRect(new Rect(rect.x + rect.width * 0.68f, rect.y + rect.height * 0.18f, rect.width * 0.12f, rect.height * 0.64f), c);
                DrawRect(new Rect(rect.x + rect.width * 0.62f, rect.y + rect.height * 0.04f, rect.width * 0.24f, rect.height * 0.18f), c);
            }
        }

        private void DrawStatusPipRow(Rect rect, CombatUnit unit)
        {
            List<StatusMark> pips = StatusMarks(unit);
            float size = Mathf.Max(4f, Mathf.Min(7f, rect.height));
            for (int i = 0; i < pips.Count && i < 6; i++)
            {
                Rect pip = new Rect(rect.x + i * (size + 3f), rect.y, size, size);
                DrawRect(pip, pips[i].Color);
                DrawBorder(pip, Hex("030405", 0.74f), 1);
            }
        }

        private void DrawStatusPips(Rect cellRect, CombatUnit unit, float cell)
        {
            List<StatusMark> pips = StatusMarks(unit);
            float badgeH = Mathf.Clamp(cell * 0.17f, 10f, 14f);
            float badgeW = badgeH * 1.58f;
            float gap = Mathf.Max(2f, cell * 0.025f);
            int columns = Mathf.Max(1, Mathf.Min(3, Mathf.FloorToInt((cell - cell * 0.08f + gap) / (badgeW + gap))));
            for (int i = 0; i < pips.Count && i < 6; i++)
            {
                int column = i % columns;
                int row = i / columns;
                StatusMark mark = pips[i];
                Rect badge = new Rect(
                    cellRect.x + cell * 0.04f + column * (badgeW + gap),
                    cellRect.y + cell * 0.035f + row * (badgeH + gap),
                    badgeW,
                    badgeH);
                DrawRect(badge, Hex("050708", 0.90f));
                DrawRect(new Rect(badge.x, badge.y, Mathf.Max(3f, badgeH * 0.28f), badge.height), mark.Color);
                DrawBorder(badge, mark.Color.WithAlpha(0.82f), 1);
                GUI.Label(
                    new Rect(badge.x + badgeH * 0.24f, badge.y - 1f, badge.width - badgeH * 0.20f, badge.height + 2f),
                    mark.Label + mark.Turns,
                    CenterStyle(Mathf.RoundToInt(Mathf.Clamp(badgeH * 0.62f, 8f, 10f)), cursorWhite));
            }
        }

        private void DrawStatusDurationBadges(Rect rect, CombatUnit unit)
        {
            List<StatusMark> marks = StatusMarks(unit);
            if (marks.Count == 0) return;
            float h = Mathf.Clamp(rect.height * 0.18f, 14f, 22f);
            float w = Mathf.Clamp(rect.width * 0.44f, 32f, 54f);
            float gap = Mathf.Max(2f, rect.height * 0.018f);
            float x = rect.xMax - w - rect.width * 0.04f;
            float y = rect.y + rect.height * 0.25f;
            for (int i = 0; i < marks.Count && i < 4; i++)
            {
                StatusMark mark = marks[i];
                Rect badge = new Rect(x, y + i * (h + gap), w, h);
                DrawRect(badge, Hex("050708", 0.82f));
                DrawRect(new Rect(badge.x, badge.y, Mathf.Min(badge.width, h * 0.55f), badge.height), mark.Color);
                DrawBorder(badge, mark.Color, 1);
                GUI.Label(new Rect(badge.x + h * 0.45f, badge.y - 1f, badge.width - h * 0.48f, badge.height + 2f), mark.Label + mark.Turns, CenterStyle(Mathf.RoundToInt(Mathf.Clamp(h * 0.62f, 8f, 11f)), cursorWhite));
            }
        }

        private List<StatusMark> StatusMarks(CombatUnit unit)
        {
            List<StatusMark> marks = new List<StatusMark>();
            if (unit == null) return marks;
            if (unit.Poisoned > 0) marks.Add(new StatusMark("P", poison, unit.Poisoned));
            if (unit.Bleeding > 0) marks.Add(new StatusMark("B", blood, unit.Bleeding));
            if (unit.Stunned > 0) marks.Add(new StatusMark("!", gold, unit.Stunned));
            if (unit.Sleeping > 0) marks.Add(new StatusMark("Z", violet, unit.Sleeping));
            if (unit.Webbed > 0) marks.Add(new StatusMark("W", Hex("d9d3c4"), unit.Webbed));
            if (unit.Shielded > 0) marks.Add(new StatusMark("S", teal, unit.Shielded));
            if (unit.Regenerating > 0) marks.Add(new StatusMark("+", Hex("97dbc2"), unit.Regenerating));
            if (unit.Hexed > 0) marks.Add(new StatusMark("H", violet, unit.Hexed));
            if (unit.Stealthed > 0) marks.Add(new StatusMark("T", Hex("7bd3c3"), unit.Stealthed));
            if (unit.DemonFormTurns > 0) marks.Add(new StatusMark("D", Hex("b94b56"), DisplayedDemonFormTurns(unit)));
            return marks;
        }

        private string StatusCompactLine(CombatUnit unit)
        {
            List<StatusMark> marks = StatusMarks(unit);
            if (marks.Count == 0) return "";
            return string.Join(" ", marks.Take(5).Select(m => m.Label + m.Turns).ToArray());
        }

        private string StatusName(StatusMark mark)
        {
            if (mark.Label == "P") return "poison";
            if (mark.Label == "B") return "bleed";
            if (mark.Label == "!") return "stun";
            if (mark.Label == "Z") return "sleep";
            if (mark.Label == "W") return "web";
            if (mark.Label == "S") return "ward";
            if (mark.Label == "+") return "regen";
            if (mark.Label == "H") return "hex";
            if (mark.Label == "T") return "stealth";
            if (mark.Label == "D") return "demon form";
            return "status";
        }

        private string EnemyTraitLine(CombatUnit enemy)
        {
            if (enemy == null) return "";
            List<string> parts = new List<string> { string.IsNullOrEmpty(enemy.Rank) ? enemy.Role : enemy.Rank + " " + enemy.Role };
            if (!string.IsNullOrEmpty(enemy.Resist)) parts.Add("res " + enemy.Resist.Replace("|", "/"));
            if (!string.IsNullOrEmpty(enemy.Weakness)) parts.Add("weak " + enemy.Weakness.Replace("|", "/"));
            if (!string.IsNullOrEmpty(enemy.StatusOnHit)) parts.Add(StatusLabel(enemy.StatusOnHit) + " hit");
            return string.Join(" / ", parts.Take(3).ToArray());
        }

        private string EnemyTacticLine(CombatUnit enemy)
        {
            if (enemy == null) return "";
            if (enemy.Role == "bonepriest") return "heals and wards";
            if (enemy.Role == "koboldraider") return "quick knife rush";
            if (enemy.Role == "koboldslinger") return "stone slinger";
            if (enemy.Role == "koboldshaman") return "hexing shaman";
            if (enemy.Role == "koboldwizard") return "death-ball wizard";
            if (enemy.Role == "koboldshield") return "small shield wall";
            if (enemy.Role == "koboldking") return "boss: warrior-mage king";
            if (enemy.Role == "sewerrat") return "fast sewer bite";
            if (enemy.Role == "giantrat") return "heavy rat swarm";
            if (enemy.Role == "ratfolk") return "scrap-tooth swarm";
            if (enemy.Role == "ratcutthroat") return "quick knife ambush";
            if (enemy.Role == "ratmage") return "plague caster";
            if (enemy.Role == "ratcleric") return "sewer healer";
            if (enemy.Role == "ratbrute") return "heavy ratfolk guard";
            if (enemy.Role == "drowscout") return "dark scout";
            if (enemy.Role == "drowblade") return "fast blade dancer";
            if (enemy.Role == "drowcrossbow") return "crossbow pressure";
            if (enemy.Role == "drowmage") return "dark-light caster";
            if (enemy.Role == "drowpriest") return "warding priest";
            if (enemy.Role == "lesserdemon") return "burning brute";
            if (enemy.Role == "mirearcher") return "poison archer";
            if (enemy.Role == "glassmage") return "cold hazard caster";
            if (enemy.Role == "adept") return "shock caster";
            if (enemy.Role == "spore") return "gas and poison";
            if (enemy.Role == "cinderling") return "leaves fire";
            if (enemy.Role == "shade") return "sleep and death";
            if (enemy.Role == "gloamknight") return "armored brute";
            if (enemy.Role == "thornbeast") return "bleeding brute";
            return enemy.Range > 1 ? "ranged pressure" : "front pressure";
        }

        private string EnemyThreatLine(CombatUnit enemy)
        {
            if (enemy == null) return "";
            string type = string.IsNullOrEmpty(enemy.DamageType) ? "physical" : enemy.DamageType;
            string rank = string.IsNullOrEmpty(enemy.Rank) ? "" : enemy.Rank + " / ";
            return $"{rank}{type} / range {enemy.Range}";
        }

        private string StatusLine(CombatUnit unit)
        {
            if (unit == null) return "";
            List<StatusMark> marks = StatusMarks(unit);
            List<string> parts = marks.Select(m => StatusName(m) + " " + m.Turns).ToList();
            CombatUnit active = CurrentUnit();
            if (active != null
                && unit.Side == UnitSide.Party
                && !string.IsNullOrEmpty(active.Id)
                && active.Id == unit.Id)
            {
                string conditions = parts.Count == 0
                    ? "No conditions"
                    : string.Join(", ", parts.Take(2).ToArray());
                return conditions + " / " + ActiveThreatSummary(unit);
            }
            return parts.Count == 0 ? "steady" : string.Join(", ", parts.Take(4).ToArray());
        }

        private Rect ExploreCommandBarRect()
        {
            return ExplorationHudScreenLayout.Calculate(Screen.width, Screen.height, !exploreHudCollapsed).Command;
        }

        private ActionMode PreferredThirdAction(CombatUnit active)
        {
            return HasMartialAbilities(active) ? ActionMode.Ability : ActionMode.Cast;
        }

        private bool HasMartialAbilities(CombatUnit unit)
        {
            return unit != null && MartialAbilitiesFor(unit).Count > 0;
        }

        private string MartialClassKey(CombatUnit unit)
        {
            string classKey = (unit?.ClassKey ?? ClassForRole(unit?.Role)).ToLowerInvariant();
            return classKey == "warlock" && unit?.DemonFormTurns > 0 ? "demon" : classKey;
        }

        private int DisplayedDemonFormTurns(CombatUnit unit)
        {
            if (unit == null || unit.DemonFormTurns <= 0) return 0;
            // The stored counter includes one bookkeeping tick so the cast does
            // not consume the first promised turn. Keep that internal tick out
            // of player-facing status copy.
            int advertisedDuration = 4 + (IsFocusedCaster(unit) ? 1 : 0);
            return Mathf.Min(unit.DemonFormTurns, advertisedDuration);
        }

        private string AbilityHeaderLine(CombatUnit active)
        {
            if (active == null) return "No active combatant.";
            if (!HasMartialAbilities(active)) return $"{active.Name} has no martial skill deck.";
            string cls = MartialClassKey(active);
            string passive = WarriorEnrageBonus(active) > 0
                ? "Enrage active: physical damage increased."
                : cls == "warrior"
                    ? "Enrage triggers under half HP."
                    : cls == "ranger"
                        ? "Ranger skills reward sight lines and marked targets."
                        : cls == "demon"
                            ? $"Abyssal form: {DisplayedDemonFormTurns(active)} turn{(DisplayedDemonFormTurns(active) == 1 ? "" : "s")} remain. Demon Arts deal death damage."
                        : active.Stealthed > 0
                            ? $"Stealthed {active.Stealthed}: enemies are less likely to focus this rogue."
                            : "Rogue skills reward stealth and bleeding targets.";
            string action = state?.Combat?.ActionAvailable == true ? "Action Ready" : "Action Used";
            string move = state?.Combat == null ? "" : $"Move {state.Combat.MovePoints}";
            return $"{active.Name} / {DisplayClass(active.ClassKey)} / {move} / {action} / {passive}";
        }

        private int AbilityIconIndex(string abilityId)
        {
            return CombatIconCatalog.AbilityIndex(abilityId);
        }

        private List<MartialAbility> MartialAbilitiesFor(CombatUnit active)
        {
            return AbilityIdsForClass(MartialClassKey(active)).Select(AbilityDef).Where(ability => ability != null).ToList();
        }

        private MartialAbility AbilityDef(string id)
        {
            return AbilityCatalog.For(id);
        }

        private string CombatPowerActionBlockReason(CombatUnit active)
        {
            if (active == null) return "No active unit.";
            if (IsCombatResolutionPending()) return "A power is still resolving.";
            if (active.Stunned > 0) return $"Stunned for {active.Stunned} more turn{(active.Stunned == 1 ? "" : "s")}.";
            if (active.Sleeping > 0) return $"Sleeping for {active.Sleeping} more turn{(active.Sleeping == 1 ? "" : "s")}.";
            if (state?.Combat?.ActionAvailable != true) return "Action already used.";
            return "";
        }

        private CombatActionCard FormulaActionCard(FormulaDef formula, CombatUnit active)
        {
            if (formula == null) return null;
            int mana = active == null ? Mathf.Max(1, formula.Mana) : EffectiveFormulaMana(formula, active);
            int range = active == null ? Mathf.Max(1, formula.Range) : EffectiveFormulaRange(formula, active);
            string reason = "";
            bool usable = active != null && CanUseFormula(active, formula.Code, out reason);
            if (usable)
            {
                reason = CombatPowerActionBlockReason(active);
                usable = string.IsNullOrEmpty(reason);
            }
            if (active != null && usable && active.Mana < mana)
            {
                usable = false;
                reason = $"Needs {mana - active.Mana} more MP ({active.Mana}/{mana}).";
            }
            if (active != null && usable && formula.Code == "RSG" && !AdjacentEnemies(active).Any())
            {
                usable = false;
                reason = "No adjacent foes";
            }
            bool selfCast = string.Equals(formula.Target, "self", StringComparison.OrdinalIgnoreCase);
            return new CombatActionCard
            {
                Id = formula.Code,
                Name = formula.Name,
                Kind = $"{SpellCraftLabel(formula.School)} formula",
                Cost = $"{mana} MP",
                Range = selfCast ? "Self" : $"Range {range}",
                Target = FormulaTargetLabel(formula),
                Path = FormulaPathLabel(formula),
                Impact = CombatPowerTargetingRules.ForFormula(formula).ModalLabel,
                Summary = FormulaEffectSummary(formula, active),
                CurrentEffect = FormulaCurrentEffectLine(formula, active),
                Detail = $"CASTING RULES\n{FormulaRuleSummary(formula, active)}\n\nFORMULA NOTE\n{formula.Hint}.",
                Targeted = !selfCast,
                Ready = pendingFormulaCode == formula.Code,
                Usable = usable,
                DisabledReason = reason
            };
        }

        private CombatActionCard AbilityActionCard(MartialAbility ability, CombatUnit active)
        {
            if (ability == null) return null;
            string reason;
            bool usable = AbilityUsableNow(active, ability, out reason);
            return new CombatActionCard
            {
                Id = ability.Id,
                Name = ability.Name,
                Kind = $"{DisplayClass(ability.ClassKey)} skill",
                Cost = "1 Action",
                Range = ability.Targeted ? $"Range {ability.Range}" : "Self",
                Target = ability.Targeted ? "Enemy" : "Instant",
                Path = AbilityPathLabel(ability),
                Impact = CombatPowerTargetingRules.ForAbility(ability).ModalLabel,
                Summary = ability.Summary,
                CurrentEffect = AbilityCurrentEffectLine(ability, active),
                Detail = $"CURRENT EFFECT\n{AbilityCurrentEffectLine(ability, active)}\n\nTACTICS\n{ability.Detail}",
                Targeted = ability.Targeted,
                Ready = pendingAbilityId == ability.Id,
                Usable = usable,
                DisabledReason = reason
            };
        }

        private string AbilityPathLabel(MartialAbility ability)
        {
            if (ability == null) return "skill";
            if (!ability.Targeted) return "instant";
            if (ability.Id == "charge") return "rush";
            if (ability.Id == "riftpounce") return "rift";
            if (ability.Id == "volley") return "arc";
            if (IsRangerAbility(ability.Id)) return "sight";
            return "adjacent";
        }

        private bool AbilityUsableNow(CombatUnit active, MartialAbility ability, out string reason)
        {
            reason = "";
            if (active == null || ability == null)
            {
                reason = "No active unit";
                return false;
            }
            if (MartialClassKey(active) != ability.ClassKey)
            {
                reason = "Wrong class";
                return false;
            }
            if (active.Level < ability.RequiredLevel)
            {
                reason = $"Level {ability.RequiredLevel}";
                return false;
            }
            reason = CombatPowerActionBlockReason(active);
            if (!string.IsNullOrEmpty(reason)) return false;
            if (ability.Id == "charge" && active.Webbed > 0)
            {
                reason = "Webbed: Charge needs free movement.";
                return false;
            }
            if (ability.Id == "whirlwind" && !AdjacentEnemies(active).Any())
            {
                reason = "No adjacent foes";
                return false;
            }
            if ((ability.Id == "abyssalwhirl" || ability.Id == "dreadroar") && !AdjacentEnemies(active).Any())
            {
                reason = "No adjacent foes";
                return false;
            }
            return true;
        }

        private string AbilityCurrentEffectLine(MartialAbility ability, CombatUnit active)
        {
            if (ability == null || active == null) return "No active combatant.";
            switch (ability.Id)
            {
                case "charge":
                    return $"{ChargeRawDamage(active)} raw physical damage{AbilityStatNote(active, ability.Id)} and stun 1; rushes through an open lane.";
                case "execute":
                    return $"{ExecuteRawDamage(active)} raw physical damage{AbilityStatNote(active, ability.Id)} against an enemy at 35% HP or lower.";
                case "shieldbash":
                    return $"{ShieldBashRawDamage(active)} raw physical damage{AbilityStatNote(active, ability.Id)} and a 1-tile push; blocked pushes collide and stun.";
                case "cleave":
                    return $"{CleaveRawDamage(active)} raw physical damage{AbilityStatNote(active, ability.Id)} and may clip a second adjacent enemy.";
                case "rally":
                    return $"Brace with guard {3 + GearGuardBonus(active)}, gain ward 2, and ward adjacent allies.";
                case "whirlwind":
                    return $"{WhirlwindRawDamage(active)} raw physical damage{AbilityStatNote(active, ability.Id)} to every adjacent enemy.";
                case "ambush":
                    return $"{AmbushRawDamage(active, active.Stealthed > 0)} raw physical damage{AbilityStatNote(active, ability.Id)}{(active.Stealthed > 0 ? " and stun 1 from stealth" : "")}.";
                case "throwknife":
                    return $"{ThrowKnifeRawDamage(active)} raw physical damage{AbilityStatNote(active, ability.Id)} and bleed 2.";
                case "eviscerate":
                    return $"{EviscerateRawDamage(active, null, active.Stealthed > 0)} base raw physical damage{AbilityStatNote(active, ability.Id)}; +4 against bleeding and +3 from stealth.";
                case "hamstring":
                    return $"{HamstringRawDamage(active)} raw physical damage{AbilityStatNote(active, ability.Id)}, bleed, and hobble 2.";
                case "stealth":
                    return "Gain stealth 2. Enemies deprioritize this rogue and opening attacks improve.";
                case "smokebomb":
                    return "Gain stealth 2 and fill adjacent open tiles with 3-round smoke that blocks direct sight but not movement.";
                case "aimedshot":
                    return $"{AimedShotRawDamage(active, null)} base raw physical damage{AbilityStatNote(active, ability.Id)}; +4 against a marked target.";
                case "pinningshot":
                    return $"{PinningShotRawDamage(active)} raw physical damage{AbilityStatNote(active, ability.Id)} and pin 1–2.";
                case "volley":
                    return $"{VolleyRawDamage(active)} raw physical splash damage{AbilityStatNote(active, ability.Id)}; arcs over cover.";
                case "scoutmark":
                    return "Break guard, strip 1 ward, and mark one enemy for 2 turns.";
                case "broadheadshot":
                    return $"{BroadheadShotRawDamage(active)} raw physical damage{AbilityStatNote(active, ability.Id)} and bleed 3.";
                case "disruptingshot":
                    return $"{DisruptingShotRawDamage(active)} raw physical damage{AbilityStatNote(active, ability.Id)} and stun 1; more accurate against casters.";
                case "riftpounce":
                    return $"{RiftPounceRawDamage(active)} raw death damage; ignores intervening terrain and lands beside the target.";
                case "abyssalwhirl":
                    return $"{AbyssalWhirlRawDamage(active)} raw death damage to every adjacent enemy.";
                case "soulrend":
                    return $"{SoulRendRawDamage(active)} raw death damage; heal for half the actual damage dealt.";
                case "dreadroar":
                    return "Strip Guard from every adjacent enemy and attempt a 3-turn mind-resisted hex.";
                default:
                    return ability.Summary ?? "Combat skill.";
            }
        }

        private string ActionName(ActionMode mode, CombatUnit active)
        {
            switch (mode)
            {
                case ActionMode.Move: return "Move";
                case ActionMode.Attack: return AttackModeLabel(active);
                case ActionMode.Cast: return "Spells";
                case ActionMode.Ability: return MartialClassKey(active) == "demon" ? "Demon Arts" : "Skills";
                case ActionMode.Guard: return "Guard";
                case ActionMode.Elixir: return "Elixir";
                case ActionMode.Wait: return "End Turn";
                default: return "Action";
            }
        }

        private string DisabledActionReason(ActionMode mode, CombatUnit active, bool playerTurn)
        {
            if (!playerTurn) return "Waiting for enemy turn";
            if (active == null) return "No active unit";
            if (IsCombatResolutionPending()) return "Power resolving";
            if (mode == ActionMode.Move && state.Combat.MovePoints <= 0) return "No move points";
            if (mode == ActionMode.Move && active.Webbed > 0) return "Webbed";
            if (mode == ActionMode.Cast && string.IsNullOrEmpty(active.Spell)) return "No spell craft";
            if (mode == ActionMode.Ability && !HasMartialAbilities(active)) return "No martial skills";
            if (mode == ActionMode.Elixir && state.Elixirs <= 0) return "No elixirs";
            if (mode != ActionMode.Move && mode != ActionMode.Wait && !state.Combat.ActionAvailable) return "Action already used";
            return "Unavailable";
        }

        private string ActionTooltipLine(ActionMode mode, CombatUnit active)
        {
            if (active == null) return "";
            if (mode == ActionMode.Move) return "Click a highlighted tile. Distance and terrain spend move points.";
            if (mode == ActionMode.Attack) return IsRangedAttackProfile(active)
                ? IsEngagedByHostile(active)
                    ? "Engaged: bow fire is suppressed. Click an adjacent enemy or cover to fight in melee."
                    : $"Shoot an enemy or breakable cover. Range {EffectiveAttackRangeFrom(active, active.X, active.Y)} with clear line of sight."
                : $"Click an enemy or breakable cover. Current range {EffectiveAttackRangeFrom(active, active.X, active.Y)}.";
            if (mode == ActionMode.Cast)
            {
                if (string.IsNullOrEmpty(active.Spell)) return "Only trained spellcasters can open the spell panel.";
                FormulaDef formula = GetFormula(pendingFormulaCode);
                if (formula != null) return $"{formula.Name}: {EffectiveFormulaMana(formula, active)} MP, range {EffectiveFormulaRange(formula, active)}, {FormulaPathLabel(formula)}.";
                return "Open the Spellbook, choose one spell, then click a highlighted target.";
            }
            if (mode == ActionMode.Ability)
            {
                MartialAbility ability = AbilityDef(pendingAbilityId);
                if (ability != null) return $"{ability.Name}: {ability.Summary}";
                return "Open Combat Skills, choose one martial skill, then click a highlighted target if needed.";
            }
            if (mode == ActionMode.Guard) return $"Instant: spend the action to gain Guard +{GuardActionBonus(active)} until next turn. Stronger before moving.";
            if (mode == ActionMode.Elixir) return "Instant: spend the action to recover health and mana from the shared supply.";
            if (mode == ActionMode.Wait) return "Instant: end this unit's turn immediately.";
            return "";
        }

        private string ActionButtonSubLabel(ActionMode mode, CombatUnit active)
        {
            if (active == null) return "";
            switch (mode)
            {
                case ActionMode.Move: return state?.Combat == null ? "" : $"{state.Combat.MovePoints} move";
                case ActionMode.Attack: return IsRangedAttackProfile(active) && IsEngagedByHostile(active) ? "Melee" : $"Range {EffectiveAttackRangeFrom(active, active.X, active.Y)}";
                case ActionMode.Cast:
                    FormulaDef formula = GetFormula(pendingFormulaCode);
                    return formula == null ? "Open spellbook" : $"{EffectiveFormulaMana(formula, active)} MP";
                case ActionMode.Ability:
                    MartialAbility ability = AbilityDef(pendingAbilityId);
                    return ability == null ? "Open skills" : ability.Short;
                case ActionMode.Guard: return $"Guard +{GuardActionBonus(active)}";
                case ActionMode.Elixir: return state == null ? "" : $"{state.Elixirs} left";
                case ActionMode.Wait: return "Finish turn";
                default: return "";
            }
        }

        private string SelectedActionButtonSubLabel(ActionMode mode, CombatUnit active)
        {
            if (active == null) return "";
            switch (mode)
            {
                case ActionMode.Move:
                    return LegalChoiceCountLabel(CountReachableMoveDestinations(active), "tile");
                case ActionMode.Attack:
                    return LegalChoiceCountLabel(CountLegalAttackTargets(active), "target");
                case ActionMode.Cast:
                    FormulaDef formula = GetFormula(pendingFormulaCode);
                    if (formula == null) return "Choose spell";
                    return string.Equals(formula.Target, "self", StringComparison.OrdinalIgnoreCase)
                        ? "Use now"
                        : LegalChoiceCountLabel(CountLegalFormulaTargets(formula, active), "target");
                case ActionMode.Ability:
                    MartialAbility ability = AbilityDef(pendingAbilityId);
                    if (ability == null) return "Choose skill";
                    return !ability.Targeted
                        ? "Use now"
                        : LegalChoiceCountLabel(CountLegalAbilityTargets(ability, active), "target");
                default: return ActionButtonSubLabel(mode, active);
            }
        }

        private int CountReachableMoveDestinations(CombatUnit active)
        {
            if (active == null || state?.Combat == null || state.Combat.MovePoints <= 0) return 0;
            int[,] reachable = ReachableMoveCosts(active, state.Combat.MovePoints);
            int count = 0;
            for (int y = 0; y < CombatH; y++)
            for (int x = 0; x < CombatW; x++)
            {
                if (x == active.X && y == active.Y) continue;
                if (reachable[x, y] < UnreachableMoveCost && CanStandAt(x, y)) count++;
            }
            return count;
        }

        private int CountLegalAttackTargets(CombatUnit active)
        {
            if (active == null || state?.Combat == null || !state.Combat.ActionAvailable) return 0;
            int count = state.Combat.Units.Count(unit =>
                unit != null
                && unit.Hp > 0
                && unit.Side == UnitSide.Enemy
                && AttackForecast(active, unit).Legal);
            count += state.Combat.Obstacles.Count(obstacle =>
                obstacle != null
                && (IsDisruptableRitual(obstacle) || IsBreakableCover(obstacle))
                && CanAttackCombatObstacle(active, obstacle));
            return count;
        }

        private static string LegalChoiceCountLabel(int count, string singular)
        {
            if (count <= 0) return "No " + singular + "s";
            return count == 1 ? "1 " + singular : count + " " + singular + "s";
        }

        private int GuardActionBonus(CombatUnit active)
        {
            if (active == null || state?.Combat == null) return 0;
            bool braced = state.Combat.MovePoints >= UnitMoveAllowance(active) && !state.Combat.Moved;
            return (braced ? 4 : 2) + GearGuardBonus(active);
        }

        private void DrawActionButtonGlyph(Rect rect, ActionMode mode, bool enabled, bool selected)
        {
            Color color = enabled ? (selected ? gold : muted) : Hex("4f5558", 0.72f);
            float iconSize = Mathf.Clamp(Mathf.Min(rect.width, rect.height) * 0.98f, 28f, 88f);
            Rect icon = new Rect(rect.center.x - iconSize * 0.5f, rect.center.y - iconSize * 0.5f, iconSize, iconSize);
            DrawRect(icon, Hex("050708", selected ? 0.76f : 0.54f));
            DrawBorder(icon, color.WithAlpha(selected ? 0.95f : 0.70f), 1);
            Rect inner = Pad(icon, Mathf.Max(2f, iconSize * 0.07f));
            if (mode == ActionMode.Cast
                && selectedAction == ActionMode.Cast
                && !string.IsNullOrEmpty(pendingFormulaCode))
            {
                FormulaDef formula = GetFormula(pendingFormulaCode);
                if (formula != null
                    && TryGetFormulaPowerArt(formula, out Texture2D formulaTexture, out Rect formulaSource)
                    && DrawTextureRegionTint(
                        formulaTexture,
                        Pad(icon, -2f),
                        formulaSource,
                        Color.white.WithAlpha(enabled ? 0.94f : 0.36f)))
                {
                    return;
                }
            }
            if (mode == ActionMode.Ability
                && selectedAction == ActionMode.Ability
                && !string.IsNullOrEmpty(pendingAbilityId))
            {
                int abilityIcon = AbilityIconIndex(pendingAbilityId);
                if (abilityIcon >= 0 && TryDrawAbilityIconAtlasIcon(Pad(icon, -2f), abilityIcon, Color.white.WithAlpha(enabled ? 0.94f : 0.36f)))
                {
                    return;
                }
            }
            int commandIcon = ActionCombatCommandIconIndex(mode);
            if (commandIcon >= 0 && TryDrawCombatCommandIconAtlasIcon(Pad(icon, -2f), commandIcon, Color.white.WithAlpha(enabled ? 0.96f : 0.34f)))
            {
                return;
            }
            int combatHudIcon = ActionCombatHudIconIndex(mode);
            if (combatHudIcon >= 0 && TryDrawCombatHudUiAtlasIcon(Pad(icon, -1f), combatHudIcon, Color.white.WithAlpha(enabled ? 0.94f : 0.36f)))
            {
                return;
            }
            int combatSpellbookIcon = ActionCombatSpellbookUiIconIndex(mode);
            if (combatSpellbookIcon >= 0 && TryDrawCombatSpellbookUiAtlasIcon(Pad(icon, -1f), combatSpellbookIcon, Color.white.WithAlpha(enabled ? 0.90f : 0.36f)))
            {
                return;
            }
            int spellbookIcon = ActionSpellbookIconIndex(mode);
            if (spellbookIcon >= 0 && TryDrawSpellbookUiAtlasIcon(Pad(icon, -1f), spellbookIcon, Color.white.WithAlpha(enabled ? 0.88f : 0.36f)))
            {
                return;
            }
            int combatUiIndex = ActionCombatUiIconIndex(mode);
            if (combatUiIndex >= 0 && TryDrawCombatUiAtlasIcon(Pad(icon, -1f), combatUiIndex, Color.white.WithAlpha(enabled ? 0.84f : 0.36f)))
            {
                return;
            }
            int atlasIndex = ActionMagicIconIndex(mode);
            if (atlasIndex >= 0 && TryDrawMagicUiAtlasIcon(Pad(icon, -1f), atlasIndex, Color.white.WithAlpha(enabled ? 0.95f : 0.42f)))
            {
                return;
            }

            if (mode == ActionMode.Move)
            {
                DrawRect(new Rect(inner.x, inner.center.y - 1f, inner.width * 0.70f, 2f), color);
                DrawRect(new Rect(inner.x + inner.width * 0.58f, inner.y + inner.height * 0.18f, inner.width * 0.34f, inner.height * 0.22f), color);
                DrawRect(new Rect(inner.x + inner.width * 0.58f, inner.y + inner.height * 0.60f, inner.width * 0.34f, inner.height * 0.22f), color);
            }
            else if (mode == ActionMode.Attack)
            {
                DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.60f, inner.width * 0.66f, inner.height * 0.14f), color);
                DrawRect(new Rect(inner.x + inner.width * 0.58f, inner.y + inner.height * 0.18f, inner.width * 0.14f, inner.height * 0.54f), color);
                DrawRect(new Rect(inner.x + inner.width * 0.52f, inner.y + inner.height * 0.10f, inner.width * 0.34f, inner.height * 0.16f), cursorWhite.WithAlpha(color.a));
            }
            else if (mode == ActionMode.Cast)
            {
                DrawPixelCross(inner, color);
                DrawRect(new Rect(inner.center.x - 1f, inner.y, 2f, inner.height), color.WithAlpha(0.70f));
            }
            else if (mode == ActionMode.Ability)
            {
                DrawRect(new Rect(inner.x + inner.width * 0.16f, inner.y + inner.height * 0.58f, inner.width * 0.68f, inner.height * 0.12f), color);
                DrawRect(new Rect(inner.x + inner.width * 0.52f, inner.y + inner.height * 0.16f, inner.width * 0.12f, inner.height * 0.60f), color);
                DrawSigil(new Rect(inner.x + inner.width * 0.30f, inner.y + inner.height * 0.22f, inner.width * 0.34f, inner.height * 0.34f), "diamond", cursorWhite.WithAlpha(color.a));
            }
            else if (mode == ActionMode.Guard)
            {
                DrawBorder(inner, color, 1);
                DrawRect(new Rect(inner.x + inner.width * 0.35f, inner.y + inner.height * 0.18f, inner.width * 0.30f, inner.height * 0.60f), color.WithAlpha(0.72f));
            }
            else if (mode == ActionMode.Elixir)
            {
                DrawRect(new Rect(inner.x + inner.width * 0.38f, inner.y, inner.width * 0.24f, inner.height * 0.25f), color);
                DrawRect(new Rect(inner.x + inner.width * 0.28f, inner.y + inner.height * 0.22f, inner.width * 0.44f, inner.height * 0.62f), teal.WithAlpha(color.a));
                DrawRect(new Rect(inner.x + inner.width * 0.34f, inner.y + inner.height * 0.52f, inner.width * 0.32f, inner.height * 0.08f), cursorWhite.WithAlpha(color.a));
            }
            else
            {
                DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.22f, inner.width * 0.18f, inner.height * 0.56f), color);
                DrawRect(new Rect(inner.x + inner.width * 0.58f, inner.y + inner.height * 0.22f, inner.width * 0.18f, inner.height * 0.56f), color);
            }
        }

        private int ActionCombatSpellbookUiIconIndex(ActionMode mode)
        {
            switch (mode)
            {
                case ActionMode.Attack: return 0;
                case ActionMode.Guard: return 1;
                case ActionMode.Cast: return 2;
                case ActionMode.Ability: return 0;
                case ActionMode.Wait: return 3;
                case ActionMode.Elixir: return 4;
                case ActionMode.Move: return 7;
                default: return -1;
            }
        }

        private int ActionCombatCommandIconIndex(ActionMode mode)
        {
            switch (mode)
            {
                case ActionMode.Move: return CombatIconCatalog.CombatCommandMoveIndex;
                case ActionMode.Attack: return CombatIconCatalog.CombatCommandAttackIndex;
                case ActionMode.Cast: return CombatIconCatalog.CombatCommandCastIndex;
                case ActionMode.Ability: return CombatIconCatalog.CombatCommandSkillsIndex;
                case ActionMode.Guard: return CombatIconCatalog.CombatCommandGuardIndex;
                case ActionMode.Elixir: return CombatIconCatalog.CombatCommandElixirIndex;
                case ActionMode.Wait: return CombatIconCatalog.CombatCommandEndTurnIndex;
                default: return -1;
            }
        }

        private int ActionCombatHudIconIndex(ActionMode mode)
        {
            switch (mode)
            {
                case ActionMode.Move: return 9;
                case ActionMode.Attack: return 10;
                case ActionMode.Cast: return 11;
                case ActionMode.Ability: return 5;
                case ActionMode.Guard: return 14;
                case ActionMode.Wait: return 17;
                default: return -1;
            }
        }

        private int ActionSpellbookIconIndex(ActionMode mode)
        {
            switch (mode)
            {
                case ActionMode.Attack: return 4;
                case ActionMode.Cast: return 5;
                case ActionMode.Ability: return 4;
                case ActionMode.Wait: return 6;
                case ActionMode.Guard: return 7;
                case ActionMode.Move: return 8;
                case ActionMode.Elixir: return 9;
                default: return -1;
            }
        }

        private int ActionCombatUiIconIndex(ActionMode mode)
        {
            switch (mode)
            {
                case ActionMode.Move: return 4;
                case ActionMode.Attack: return 8;
                case ActionMode.Cast: return 7;
                case ActionMode.Ability: return 8;
                case ActionMode.Guard: return 10;
                case ActionMode.Elixir: return 13;
                case ActionMode.Wait: return 6;
                default: return -1;
            }
        }

        private int ActionMagicIconIndex(ActionMode mode)
        {
            switch (mode)
            {
                case ActionMode.Move: return 12;
                case ActionMode.Attack: return 13;
                case ActionMode.Cast: return 11;
                case ActionMode.Ability: return 13;
                case ActionMode.Guard: return 14;
                case ActionMode.Wait: return 15;
                default: return -1;
            }
        }

        private void DrawBetaLabToolbar(Rect rect, CombatUnit active, bool playerTurn)
        {
            DrawRect(rect, Hex("080b0d", 0.94f));
            DrawBorder(rect, Hex("c65c3b", 0.82f), 1);
            DrawFormulaLabRegion(new Rect(rect.x + 5, rect.y + 4, 24, 22), new Rect(1130, 718, 58, 58));
            bool martial = state?.Combat?.EncounterStyle == "martiallab";
            GUI.Label(new Rect(rect.x + 34, rect.y + 5, 118, 18), martial ? "Martial Lab" : "Beta Lab", CenterLeftStyle(12, gold));
            float x = rect.x + 154;
            float gap = 6f;
            string[] labels = martial
                ? new[] { "Refill", "Promote", "Wound", "Cluster", "Reset", "Spawn", "Audio" }
                : new[] { "Refill", "Mage", "Pact", "Craft", "Stage", "Hazards", "Spawn", "Reset", "Audio" };
            float buttonW = martial ? 68f : 62f;
            if (!martial) gap = 4f;
            for (int i = 0; i < labels.Length; i++)
            {
                Rect button = new Rect(x + i * (buttonW + gap), rect.y + 3, buttonW, 24);
                if (GUI.Button(button, labels[i], smallButtonStyle))
                {
                    if (labels[i] == "Refill") RefillBetaLab();
                    else if (labels[i] == "Mage") PromoteMageTester(active);
                    else if (labels[i] == "Pact") PromoteWarlockTester(active);
                    else if (labels[i] == "Craft") EmpowerSpellLabCasters();
                    else if (labels[i] == "Stage") StageSpellLabTargets(active);
                    else if (labels[i] == "Promote") PromoteMartialLabUnits();
                    else if (labels[i] == "Wound") StageWoundedEnemyForMartialLab(active);
                    else if (labels[i] == "Cluster") ClusterEnemiesForMartialLab(active);
                    else if (labels[i] == "Reset")
                    {
                        if (martial) StartMartialCombatLab();
                        else StartBetaCombatLab();
                    }
                    else if (labels[i] == "Hazards") AddBetaLabHazards();
                    else if (labels[i] == "Spawn") SpawnBetaLabWave();
                    else TestSfx();
                }
            }

            if (active != null && rect.width > 650f)
            {
                string who = playerTurn ? $"{active.Name}: {CombatPhaseLabel()}" : $"{active.Name}: enemy test";
                float statusX = x + labels.Length * (buttonW + gap) + 8f;
                float statusW = rect.xMax - statusX - 8f;
                if (statusW >= 150f)
                {
                    GUI.Label(new Rect(statusX, rect.y + 5, statusW, 18), FitText(who, statusW, CenterLeftStyle(11, muted)), CenterLeftStyle(11, muted));
                }
            }
        }

        private void RefillBetaLab()
        {
            if (state?.Combat?.Units == null) return;
            state.Elixirs = Mathf.Max(state.Elixirs, 9);
            foreach (CombatUnit unit in state.Combat.Units.Where(u => u.Side == UnitSide.Party))
            {
                unit.Hp = unit.MaxHp;
                unit.Mana = unit.MaxMana;
                unit.Poisoned = 0;
                unit.Bleeding = 0;
                unit.Stunned = 0;
                unit.Sleeping = 0;
                unit.Webbed = 0;
                unit.Hexed = 0;
                unit.DemonFormTurns = 0;
                unit.Shielded = Mathf.Max(unit.Shielded, 1);
                AddFloat(unit.X, unit.Y, "refill", teal);
            }
            SyncPartyFromCombat();
            PushLog("Beta Lab refills party health, mana, elixirs, and clears afflictions.", Tone.Good);
            ShowBanner("Beta refill");
            PlaySfx("heal", 0.9f);
        }

        private void PromoteMageTester(CombatUnit active)
        {
            if (state == null) return;
            CombatUnit unit = active != null && active.Side == UnitSide.Party
                ? active
                : state.Combat?.Units?.FirstOrDefault(u => u.Side == UnitSide.Party && u.Hp > 0);
            if (unit != null)
            {
                ApplyMageTesterKit(unit);
                if (unit.PartyIndex >= 0 && unit.PartyIndex < state.Party.Count) ApplyMageTesterKit(state.Party[unit.PartyIndex]);
                state.Combat.ActiveId = unit.Id;
                state.Combat.ActionAvailable = true;
                selectedAction = ActionMode.Cast;
                showSpellbook = true;
                showAbilityPanel = false;
                ClearFormulaEntry();
                AddFloat(unit.X, unit.Y, "mage lab", ember);
            }
            else if (state.Party != null && state.Party.Count > 0)
            {
                ApplyMageTesterKit(state.Party[0]);
            }
            PushLog("Beta Lab Mage kit ready: focused casting, Veil Step, Meteor Shower, and Arcane Tempest are available for testing.", Tone.Good);
            ShowBanner("Mage test ready");
            PlaySfx("spell", 0.92f);
        }

        private void ApplyMageTesterKit(PartyMember member)
        {
            if (member == null) return;
            if (member.Skills == null) member.Skills = new SkillSet();
            member.ClassKey = "mage";
            member.Role = "ember";
            member.Spell = "ember";
            member.Level = Mathf.Max(member.Level, 6);
            member.Skills.Ember = Mathf.Max(member.Skills.Ember, 34);
            member.Skills.Hex = Mathf.Max(member.Skills.Hex, 6);
            member.MaxMana = Mathf.Max(member.MaxMana, 58);
            member.Mana = member.MaxMana;
            member.Power = Mathf.Max(member.Power, 9);
            member.Range = Mathf.Max(member.Range, 4);
            member.WeaponName = "lab ember focus";
            member.WeaponDamageType = "fire";
        }

        private void ApplyMageTesterKit(CombatUnit unit)
        {
            if (unit == null) return;
            if (unit.Skills == null) unit.Skills = new SkillSet();
            unit.ClassKey = "mage";
            unit.Role = "ember";
            unit.Spell = "ember";
            unit.Level = Mathf.Max(unit.Level, 6);
            unit.Skills.Ember = Mathf.Max(unit.Skills.Ember, 34);
            unit.Skills.Hex = Mathf.Max(unit.Skills.Hex, 6);
            unit.MaxMana = Mathf.Max(unit.MaxMana, 58);
            unit.Mana = unit.MaxMana;
            unit.Power = Mathf.Max(unit.Power, 9);
            unit.Range = Mathf.Max(unit.Range, 4);
            unit.WeaponName = "lab ember focus";
            unit.DamageType = "fire";
            unit.Color = RoleColor("ember").ToHex();
        }

        private void PromoteWarlockTester(CombatUnit active)
        {
            if (state == null) return;
            CombatUnit unit = active != null && active.Side == UnitSide.Party
                ? active
                : state.Combat?.Units?.FirstOrDefault(u => u.Side == UnitSide.Party && u.Hp > 0);
            if (unit != null)
            {
                ApplyWarlockTesterKit(unit);
                if (unit.PartyIndex >= 0 && unit.PartyIndex < state.Party.Count) ApplyWarlockTesterKit(state.Party[unit.PartyIndex]);
                state.Combat.ActiveId = unit.Id;
                state.Combat.ActionAvailable = true;
                selectedAction = ActionMode.Cast;
                showSpellbook = true;
                showAbilityPanel = false;
                ClearFormulaEntry();
                AddFloat(unit.X, unit.Y, "pact lab", violet);
            }
            PushLog("Beta Lab Pact kit ready: all three summons, Pact Brand, and Abyssal Ascendance are available for testing.", Tone.Good);
            ShowBanner("Pact test ready");
            PlaySfx("curse", 0.92f);
        }

        private void ApplyWarlockTesterKit(PartyMember member)
        {
            if (member == null) return;
            if (member.Skills == null) member.Skills = new SkillSet();
            member.ClassKey = "warlock";
            member.Role = "hex";
            member.Spell = "hex|pact";
            member.Level = Mathf.Max(member.Level, 6);
            member.Skills.Hex = Mathf.Max(member.Skills.Hex, 36);
            member.MaxMana = Mathf.Max(member.MaxMana, 64);
            member.Mana = member.MaxMana;
            member.Power = Mathf.Max(member.Power, 10);
            member.Range = Mathf.Max(member.Range, 4);
            member.WeaponName = "lab abyssal focus";
            member.WeaponDamageType = "death";
        }

        private void ApplyWarlockTesterKit(CombatUnit unit)
        {
            if (unit == null) return;
            if (unit.Skills == null) unit.Skills = new SkillSet();
            unit.ClassKey = "warlock";
            unit.Role = "hex";
            unit.Spell = "hex|pact";
            unit.Level = Mathf.Max(unit.Level, 6);
            unit.Skills.Hex = Mathf.Max(unit.Skills.Hex, 36);
            unit.MaxMana = Mathf.Max(unit.MaxMana, 64);
            unit.Mana = unit.MaxMana;
            unit.Power = Mathf.Max(unit.Power, 10);
            unit.Range = Mathf.Max(unit.Range, 4);
            unit.WeaponName = "lab abyssal focus";
            unit.DamageType = "death";
            unit.Color = RoleColor("hex").ToHex();
        }

        private void PrepareBetaSpellLabParty()
        {
            if (state?.Party == null) return;
            foreach (PartyMember member in state.Party)
            {
                ApplySpellLabCraft(member);
            }
        }

        private void ApplySpellLabCraft(PartyMember member)
        {
            if (member == null) return;
            if (member.Skills == null) member.Skills = StartingSkills(member.ClassKey).Normalize();
            string cls = (member.ClassKey ?? "").ToLowerInvariant();
            if (cls == "warlock" || cls == "wizard" || cls == "mage" || member.Role == "hex" || member.Role == "ember")
            {
                member.Spell = MergeSpellSchools(member.Spell, "ember", "hex", "pact");
                member.Level = Mathf.Max(member.Level, 3);
                member.Skills.Ember = Mathf.Max(member.Skills.Ember, 22);
                member.Skills.Hex = Mathf.Max(member.Skills.Hex, 22);
                member.MaxMana = Mathf.Max(member.MaxMana, 42);
                member.Mana = member.MaxMana;
            }
            else if (cls == "priest" || cls == "paladin" || member.Role == "mender" || member.Role == "ward")
            {
                member.Spell = MergeSpellSchools(member.Spell, "mend");
                member.Level = Mathf.Max(member.Level, 2);
                member.Skills.Mend = Mathf.Max(member.Skills.Mend, 20);
                member.Skills.Guard = Mathf.Max(member.Skills.Guard, 5);
                member.MaxMana = Mathf.Max(member.MaxMana, 36);
                member.Mana = member.MaxMana;
            }
        }

        private void ApplySpellLabCraft(CombatUnit unit)
        {
            if (unit == null || unit.Side != UnitSide.Party || unit.Summoned) return;
            if (unit.Skills == null) unit.Skills = new SkillSet().Normalize();
            string cls = (unit.ClassKey ?? "").ToLowerInvariant();
            if (cls == "warlock" || cls == "wizard" || cls == "mage" || unit.Role == "hex" || unit.Role == "ember")
            {
                unit.Spell = MergeSpellSchools(unit.Spell, "ember", "hex", "pact");
                unit.Level = Mathf.Max(unit.Level, 3);
                unit.Skills.Ember = Mathf.Max(unit.Skills.Ember, 22);
                unit.Skills.Hex = Mathf.Max(unit.Skills.Hex, 22);
                unit.MaxMana = Mathf.Max(unit.MaxMana, 42);
                unit.Mana = unit.MaxMana;
                AddFloat(unit.X, unit.Y, "all craft", violet);
            }
            else if (cls == "priest" || cls == "paladin" || unit.Role == "mender" || unit.Role == "ward")
            {
                unit.Spell = MergeSpellSchools(unit.Spell, "mend");
                unit.Level = Mathf.Max(unit.Level, 2);
                unit.Skills.Mend = Mathf.Max(unit.Skills.Mend, 20);
                unit.Skills.Guard = Mathf.Max(unit.Skills.Guard, 5);
                unit.MaxMana = Mathf.Max(unit.MaxMana, 36);
                unit.Mana = unit.MaxMana;
                AddFloat(unit.X, unit.Y, "mend craft", teal);
            }
        }

        private string MergeSpellSchools(string existing, params string[] schools)
        {
            List<string> merged = new List<string>();
            if (!string.IsNullOrWhiteSpace(existing))
            {
                merged.AddRange(existing.Split('|').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim().ToLowerInvariant()));
            }
            foreach (string school in schools)
            {
                if (!string.IsNullOrWhiteSpace(school)) merged.Add(school.Trim().ToLowerInvariant());
            }
            return string.Join("|", merged.Distinct().ToArray());
        }

        private void EmpowerSpellLabCasters()
        {
            if (state == null) return;
            int partyCount = 0;
            if (state.Party != null)
            {
                foreach (PartyMember member in state.Party)
                {
                    string before = member.Spell;
                    ApplySpellLabCraft(member);
                    if (before != member.Spell || !string.IsNullOrEmpty(member.Spell)) partyCount++;
                }
            }
            if (state.Combat?.Units != null)
            {
                foreach (CombatUnit unit in state.Combat.Units.Where(u => u.Side == UnitSide.Party))
                {
                    ApplySpellLabCraft(unit);
                }
            }
            PushLog($"Spell Lab empowers {partyCount} caster test kit{(partyCount == 1 ? "" : "s")}: ember, hex, pact, and mend formulas are ready.", Tone.Good);
            ShowBanner("Spellcraft ready");
            PlaySfx("shrine", 0.76f);
        }

        private void StageSpellLabTargets(CombatUnit active)
        {
            if (state?.Combat?.Units == null) return;
            Point[] enemySpots =
            {
                new Point(8, 3),
                new Point(8, 2),
                new Point(8, 4),
                new Point(9, 3)
            };
            string[] kinds = { "koboldshield", "cinderling", "ratmage", "drowpriest" };
            List<CombatUnit> enemies = state.Combat.Units.Where(u => u.Side == UnitSide.Enemy && u.Hp > 0).ToList();
            int staged = 0;
            for (int i = 0; i < enemySpots.Length; i++)
            {
                Point spot = enemySpots[i];
                if (UnitAt(spot.X, spot.Y)?.Side == UnitSide.Party) continue;
                CombatUnit enemy = i < enemies.Count ? enemies[i] : null;
                if (enemy == null)
                {
                    enemy = MakeEnemy(kinds[i % kinds.Length], state.Combat.Units.Count + i);
                    state.Combat.Units.Add(enemy);
                }
                state.Combat.Obstacles.RemoveAll(o => o.X == spot.X && o.Y == spot.Y);
                Vector2 from = new Vector2(enemy.X, enemy.Y);
                enemy.X = spot.X;
                enemy.Y = spot.Y;
                enemy.Hp = Mathf.Max(1, enemy.MaxHp);
                AddTween(enemy.Id, from, new Vector2(spot.X, spot.Y), TweenKind.Move);
                AddFloat(enemy.X, enemy.Y, "mark", blood);
                AddFlash(enemy.X, enemy.Y, blood);
                staged++;
            }

            AddOrRefreshObstacle(7, 2, "tree", SummonedTreeDuration);
            AddOrRefreshObstacle(7, 3, "gas", 5);
            AddOrRefreshObstacle(9, 2, "web", 5);
            AddOrRefreshObstacle(9, 4, "ice", 5);
            AddOrRefreshObstacle(6, 4, "stone", 0);
            AddOrRefreshObstacle(5, 5, "sanctuary", 8);
            AddOrRefreshObstacle(8, 5, "curse", 8);
            PushLog($"Spell Lab stages {staged} marked targets and reactive hazards for Fireball, Meteor, Shock, Cold, Burn Cover, Tree Cover, Hallowed Circle, and Doom Circle testing.", Tone.Warn);
            ShowBanner("Spell targets staged");
            PlaySfx("spell", 0.88f);
        }

        private void PromoteMartialLabUnits()
        {
            if (state?.Combat?.Units == null) return;
            int promoted = 0;
            foreach (CombatUnit unit in state.Combat.Units.Where(u => u.Side == UnitSide.Party && (u.ClassKey == "warrior" || u.ClassKey == "rogue" || u.ClassKey == "ranger")))
            {
                unit.Level = Mathf.Max(unit.Level, 3);
                if (unit.Skills == null) unit.Skills = new SkillSet().Normalize();
                unit.Skills.Arms = Mathf.Max(unit.Skills.Arms, 18);
                if (unit.ClassKey == "warrior") unit.Skills.Guard = Mathf.Max(unit.Skills.Guard, 12);
                if (unit.ClassKey == "rogue") unit.Skills.Missile = Mathf.Max(unit.Skills.Missile, 10);
                if (unit.ClassKey == "ranger") unit.Skills.Missile = Mathf.Max(unit.Skills.Missile, 20);
                unit.Hp = unit.MaxHp;
                AddFloat(unit.X, unit.Y, "promote", gold);
                promoted++;
            }
            foreach (PartyMember member in state.Party.Where(p => p.ClassKey == "warrior" || p.ClassKey == "rogue" || p.ClassKey == "ranger"))
            {
                PromoteMemberForMartialTesting(member);
                member.Hp = member.MaxHp;
                member.Mana = member.MaxMana;
            }
            SyncPartyFromCombat();
            PushLog($"Martial Lab promotes {promoted} warrior/rogue/ranger testers to unlock skill gates.", Tone.Good);
            ShowBanner("Skills unlocked");
            PlaySfx("shrine", 0.72f);
        }

        private void StageWoundedEnemyForMartialLab(CombatUnit active)
        {
            if (state?.Combat?.Units == null) return;
            CombatUnit target = state.Combat.Units
                .Where(u => u.Side == UnitSide.Enemy && u.Hp > 0)
                .OrderBy(u => active == null ? 0 : Distance(active.X, active.Y, u.X, u.Y))
                .FirstOrDefault();
            if (target == null)
            {
                PushLog("No enemy remains to wound.", Tone.Warn);
                PlaySfx("blocked", 0.62f);
                return;
            }
            target.Hp = Mathf.Max(1, Mathf.CeilToInt(target.MaxHp * 0.30f));
            target.Bleeding = Mathf.Max(target.Bleeding, 2);
            AddFloat(target.X, target.Y, "wounded", blood);
            AddFlash(target.X, target.Y, blood);
            PushLog($"{target.Name} is staged below execute range and bleeding for martial testing.", Tone.Warn);
            ShowBanner("Target wounded");
            PlaySfx("blade", 0.72f);
        }

        private void ClusterEnemiesForMartialLab(CombatUnit active)
        {
            if (state?.Combat?.Units == null || active == null)
            {
                PlaySfx("blocked", 0.62f);
                return;
            }
            Point[] spots =
            {
                new Point(active.X + 1, active.Y),
                new Point(active.X - 1, active.Y),
                new Point(active.X, active.Y + 1),
                new Point(active.X, active.Y - 1)
            };
            List<CombatUnit> enemies = state.Combat.Units.Where(u => u.Side == UnitSide.Enemy && u.Hp > 0).Take(4).ToList();
            int moved = 0;
            for (int i = 0; i < enemies.Count && i < spots.Length; i++)
            {
                Point spot = spots[i];
                if (spot.X < 0 || spot.X >= CombatW || spot.Y < 0 || spot.Y >= CombatH) continue;
                CombatUnit occupant = UnitAt(spot.X, spot.Y);
                if (occupant != null && occupant.Id != enemies[i].Id) continue;
                state.Combat.Obstacles.RemoveAll(o => o.X == spot.X && o.Y == spot.Y);
                if (IsBlockingTerrain(ObstacleAt(spot.X, spot.Y))) continue;
                Vector2 from = new Vector2(enemies[i].X, enemies[i].Y);
                enemies[i].X = spot.X;
                enemies[i].Y = spot.Y;
                AddTween(enemies[i].Id, from, new Vector2(spot.X, spot.Y), TweenKind.Move);
                AddFlash(spot.X, spot.Y, blood);
                moved++;
            }
            if (moved == 0)
            {
                PushLog("No open adjacent tiles for clustering.", Tone.Warn);
                PlaySfx("blocked", 0.62f);
                return;
            }
            PushLog($"Martial Lab clusters {moved} enemies around {active.Name}.", Tone.Warn);
            ShowBanner("Enemies clustered");
            PlaySfx("encounter", 0.72f);
        }

        private void AddBetaLabHazards()
        {
            if (state?.Combat == null) return;
            AddOrRefreshObstacle(5, 1, "tree", SummonedTreeDuration);
            AddOrRefreshObstacle(6, 2, "stone", 0);
            AddOrRefreshObstacle(7, 3, "web", 9);
            AddOrRefreshObstacle(8, 4, "gas", 9);
            AddOrRefreshObstacle(6, 5, "fire", 7);
            AddOrRefreshObstacle(8, 6, "ice", 9);
            PushLog("Beta Lab refreshes tree, stone, web, gas, fire, and ice hazards.", Tone.Warn);
            ShowBanner("Hazards refreshed");
            PlaySfx("spell", 0.85f);
        }

        private void AddOrRefreshObstacle(int x, int y, string kind, int duration)
        {
            if (state?.Combat == null || UnitAt(x, y) != null) return;
            state.Combat.Obstacles.RemoveAll(o => o.X == x && o.Y == y);
            int rounds = FieldDurationRounds(kind, duration);
            state.Combat.Obstacles.Add(new Point(x, y, kind, rounds));
            AddFlash(x, y, TerrainHighlightColor(new Point(x, y, kind, rounds), 0.8f));
        }

        private void SpawnBetaLabWave()
        {
            if (state?.Combat?.Units == null) return;
            bool martial = state.Combat.EncounterStyle == "martiallab";
            string[] wave = martial
                ? new[] { "koboldshield", "ratbrute", "koboldraider", "drowblade", "ratcutthroat", "reaver" }
                : new[] { "koboldshaman", "koboldwizard", "bonepriest", "glassmage", "cinderling", "ratmage", "ratcleric", "drowmage", "drowpriest", "lesserdemon" };
            int spawned = 0;
            for (int i = 0; i < wave.Length && state.Combat.Units.Count(u => u.Hp > 0 && u.Side == UnitSide.Enemy) < 10; i++)
            {
                Point spot = FindBetaEnemySpawn();
                if (spot == null) break;
                CombatUnit enemy = MakeEnemy(wave[(state.Combat.Units.Count + i) % wave.Length], state.Combat.Units.Count + i);
                enemy.X = spot.X;
                enemy.Y = spot.Y;
                state.Combat.Units.Add(enemy);
                AddFlash(enemy.X, enemy.Y, blood);
                AddFloat(enemy.X, enemy.Y, "spawn", blood);
                spawned++;
            }

            if (spawned == 0)
            {
                PushLog("Beta Lab has no safe enemy spawn tile.", Tone.Warn);
                PlaySfx("blocked", 0.65f);
                return;
            }

            PushLog($"{(martial ? "Martial Lab" : "Beta Lab")} spawns {spawned} additional {(martial ? "melee test" : "caster-pressure")} enemies.", Tone.Warn);
            ShowBanner($"Spawned {spawned}");
            PlaySfx("encounter", 0.8f);
        }

        private Point FindBetaEnemySpawn()
        {
            for (int x = CombatW - 2; x >= Mathf.Max(6, CombatW - 5); x--)
            for (int y = 0; y < CombatH; y++)
            {
                if (CanStandAt(x, y)) return new Point(x, y);
            }
            return null;
        }

        private string FormulaRuleSummary(FormulaDef formula, CombatUnit active)
        {
            if (formula == null) return FormulaCodexLine(active);
            int mana = EffectiveFormulaMana(formula, active);
            int range = EffectiveFormulaRange(formula, active);
            string los = FormulaRequiresLineOfSight(formula) ? "direct sight" : "no direct sight needed";
            if (FormulaArcsOverCover(formula)) los = "arcs over trees";
            string focus = IsFocusedCaster(active) ? "focused" : state?.Combat?.Moved == true ? "moved" : "unfocused";
            return $"L{FormulaRequiredLevel(formula)} {SpellCraftLabel(formula.School)} {FormulaTierLabel(formula)} / {mana} MP / range {range} / {FormulaTargetLabel(formula)} / {los} / {focus}";
        }

        private string FormulaEffectSummary(FormulaDef formula, CombatUnit active)
        {
            if (formula == null) return "Choose a spell card.";
            switch (formula.Code ?? "")
            {
                case "RIG": return "Reliable shock against one enemy; conductive hazards carry a small arc.";
                case "RSG": return "Shock and push every adjacent enemy; blocked pushes collide and stun.";
                case "CLT": return "Shock up to four linked enemies, losing power at each jump.";
                case "VST": return "Teleport to an open tile and shock enemies beside the landing.";
                case "AST": return "Call a radius-2 storm: heavy center damage, lighter outer bolts, and possible stuns.";
            }
            if (formula.Effect == "dispel") return "Seals summoning rituals instantly and unravels fire, ice, gas, smoke, web, or curse fields.";
            if (formula.Effect == "terrain")
            {
                int duration = FieldDurationRounds(formula.Terrain, formula.Duration);
                return $"{TerrainDescription(formula.Terrain)} Duration {(duration <= 0 ? "permanent" : duration + " rounds")}.";
            }
            if (formula.Effect == "heal") return $"Heals about {FormulaHealPreview(formula, active).x}-{FormulaHealPreview(formula, active).y}; Intelligence improves the mend.";
            if (formula.Effect == "cure") return "Cleanses poison, bleed, web, stun, sleep, and hex.";
            if (formula.Effect == "status") return $"{StatusLabel(formula.Status)} for {Mathf.Max(1, formula.Duration)} turns; Intelligence helps beat magic resistance.";
            if (formula.Effect == "drain") return $"Deals {formula.DamageType} damage and returns about half as healing.";
            if (formula.Effect == "summon") return $"{SummonDisplayName(formula.SummonRole)} joins for {Mathf.Max(1, formula.Duration)} turns. Intelligence strengthens the binding.";
            if (formula.Effect == "teleport") return $"Teleport to an open tile within range {EffectiveFormulaRange(formula, active)}. Cover and occupied paths do not interfere.";
            if (formula.Effect == "transform") return $"Become a greater demon for {Mathf.Max(1, formula.Duration)} turns: +4 physical and pact power, 2 damage reduction, ward, regeneration, and an immediate heal.";
            if (formula.Effect == "damage")
            {
                string splash = formula.Splash ? " with splash" : "";
                string status = string.IsNullOrEmpty(formula.Status) ? "" : $", may {StatusLabel(formula.Status)}";
                return $"{formula.DamageType} damage{splash}{status}; Intelligence and status setup raise power.";
            }
            return formula.Hint;
        }

        private string FormulaCurrentEffectLine(FormulaDef formula, CombatUnit active)
        {
            if (formula == null || active == null) return FormulaEffectSummary(formula, active);
            Vector2Int damage = FormulaDamagePreview(formula, active, null);
            switch (formula.Code ?? "")
            {
                case "RIG":
                    return $"{damage.x}-{damage.y} shock to one enemy; ice, gas, and webs conduct to nearby enemies.";
                case "RSG":
                    return $"{LightningPowerRules.ThunderclapDamage(damage.x)}-{LightningPowerRules.ThunderclapDamage(damage.y)} shock to adjacent enemies, then push 1; collisions add damage and stun.";
                case "CLT":
                    return $"{damage.x}-{damage.y} shock to the first enemy, then 75%, 55%, and 40% through up to three nearby jumps.";
                case "VST":
                    return $"Teleport up to {EffectiveFormulaRange(formula, active)} tiles; adjacent enemies take {LightningPowerRules.ThunderStepDamage(damage.x)}-{LightningPowerRules.ThunderStepDamage(damage.y)} shock.";
                case "AST":
                    return $"{damage.x}-{damage.y} shock at the center and 60% in radius 2; each surviving enemy may be stunned.";
            }
            if (formula.Effect == "damage" || formula.Effect == "drain")
            {
                string area = formula.Splash ? " with adjacent splash" : "";
                string drain = formula.Effect == "drain" ? "; heals for half damage dealt" : "";
                return $"{damage.x}-{damage.y} {formula.DamageType} damage{area}{drain}.";
            }
            return FormulaEffectSummary(formula, active);
        }

        private int FormulaTier(FormulaDef formula)
        {
            if (formula == null) return 1;
            if (formula.Code == "RLM" || formula.Code == "IBG" || formula.Code == "AST" || formula.Code == "DFA" || (formula.Splash && formula.Mana >= 9)) return 4;
            if (formula.Code == "IBF") return 3;
            if (formula.Splash || formula.Mana >= 8 || formula.Code == "TNC") return 3;
            if (formula.Mana >= 6 || formula.Effect == "status" || formula.Terrain == "stone" || formula.Terrain == "fire" || formula.Terrain == "gas") return 2;
            return 1;
        }

        private int FormulaRequiredLevel(FormulaDef formula)
        {
            return FormulaCatalog.RequiredLevel(formula);
        }

        private string FormulaTierLabel(FormulaDef formula)
        {
            switch (FormulaTier(formula))
            {
                case 4: return "elder";
                case 3: return "adept";
                case 2: return "apprentice";
                default: return "starter";
            }
        }

        private Color FormulaTierColor(FormulaDef formula)
        {
            switch (FormulaTier(formula))
            {
                case 4: return violet;
                case 3: return gold;
                case 2: return teal;
                default: return muted;
            }
        }

        private string FormulaTargetLabel(FormulaDef formula)
        {
            if (formula == null) return "target";
            if (formula.Effect == "summon") return "summon tile";
            if (formula.Effect == "dispel") return "ritual or hostile field";
            if (formula.Target == "tile") return "open tile";
            if (formula.Target == "ally") return "ally";
            if (formula.Target == "enemy") return "enemy";
            if (formula.Target == "self") return "self";
            return "target";
        }

        private string SpellCraftLabel(string school)
        {
            if (string.IsNullOrEmpty(school)) return "spell";
            if (school.Contains("|")) return string.Join(" or ", school.Split('|').Select(SpellCraftLabel).ToArray());
            if (school.Equals("mend", StringComparison.OrdinalIgnoreCase)) return "cleric";
            if (school.Equals("ember", StringComparison.OrdinalIgnoreCase)) return "ember";
            if (school.Equals("hex", StringComparison.OrdinalIgnoreCase)) return "hex";
            if (school.Equals("pact", StringComparison.OrdinalIgnoreCase)) return "pact";
            return school;
        }

        private FormulaDef DefaultFormulaForCaster(CombatUnit active)
        {
            if (active == null || string.IsNullOrEmpty(active.Spell)) return null;
            if (string.Equals(active.ClassKey, "warlock", StringComparison.OrdinalIgnoreCase) && CasterKnowsSchool(active.Spell, "pact")) return KnownFormulaByCode(active, "IBD") ?? KnownFormulasFor(active).FirstOrDefault();
            if (CasterKnowsSchool(active.Spell, "mend")) return KnownFormulaByCode(active, "GBH") ?? KnownFormulasFor(active).FirstOrDefault();
            if (CasterKnowsSchool(active.Spell, "ember")) return KnownFormulaByCode(active, "FBL") ?? KnownFormulaByCode(active, "FIF") ?? KnownFormulasFor(active).FirstOrDefault();
            if (CasterKnowsSchool(active.Spell, "hex")) return KnownFormulaByCode(active, "RLM") ?? KnownFormulaByCode(active, "RKW") ?? KnownFormulasFor(active).FirstOrDefault();
            if (CasterKnowsSchool(active.Spell, "pact")) return KnownFormulaByCode(active, "IBD") ?? KnownFormulasFor(active).FirstOrDefault();
            return KnownFormulasFor(active).FirstOrDefault();
        }

        private FormulaDef KnownFormulaByCode(CombatUnit active, string code)
        {
            return KnownFormulasFor(active).FirstOrDefault(f => f.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        private void DrawFormulaLabIcon(Rect rect, FormulaDef formula, string schoolFallback)
        {
            DrawRect(rect, Hex("050708", 0.82f));
            if (TryDrawSpellbookFormulaIcon(Pad(rect, 1f), formula, schoolFallback, Color.white.WithAlpha(0.94f)))
            {
                DrawBorder(rect, formula == null ? line : FormulaColor(formula), 1);
                return;
            }
            if (formula != null)
            {
                DrawFormulaRuneCode(rect, formula.Code, formula);
                return;
            }
            Rect source = FormulaLabIconRegion(formula, schoolFallback);
            if (!DrawFormulaLabRegion(rect, source))
            {
                Color color = formula == null ? muted : FormulaColor(formula);
                DrawBorder(rect, color, 1);
                DrawPixelCross(Pad(rect, rect.width * 0.24f), color);
                return;
            }
            DrawBorder(rect, formula == null ? line : FormulaColor(formula), 1);
        }

        private bool TryDrawSpellbookFormulaIcon(Rect rect, FormulaDef formula, string schoolFallback, Color tint)
        {
            if (formula != null)
            {
                int signatureIndex = CombatIconCatalog.SignatureSpellIndex(formula.Code);
                return signatureIndex >= 0
                    && TryDrawSignatureSpellIconAtlasIcon(rect, signatureIndex, tint);
            }

            int pactIndex = PactFormulaIconIndex(formula);
            if (pactIndex >= 0 && TryDrawPactSpellbookAtlasIcon(rect, pactIndex, tint)) return true;
            int index = SpellbookFormulaIconIndex(formula, schoolFallback);
            if (index >= 0 && TryDrawSpellbookUiAtlasIcon(rect, index, tint)) return true;
            return false;
        }

        private int PactFormulaIconIndex(FormulaDef formula)
        {
            switch (formula?.Code ?? "")
            {
                case "IBD": return 0;
                case "IBF": return 1;
                case "IBG": return 2;
                case "DFA": return 3;
                case "PBR": return 4;
                default:
                    if (formula != null && formula.Effect == "summon") return 8;
                    if (formula != null && SchoolMatches(formula, "pact")) return 11;
                    return -1;
            }
        }

        private int SpellbookFormulaIconIndex(FormulaDef formula, string schoolFallback)
        {
            string code = formula?.Code ?? "";
            string type = formula?.DamageType ?? "";
            string terrain = formula?.Terrain ?? "";
            string effect = formula?.Effect ?? "";
            string school = formula?.School ?? schoolFallback ?? "";
            if (code == "FBL") return 2;
            if (code == "MTR") return 3;
            if (terrain == "tree") return 12;
            if (terrain == "stone") return 13;
            if (terrain == "sanctuary") return 14;
            if (terrain == "curse") return 15;
            if (terrain == "web") return 16;
            if (terrain == "gas") return 17;
            if (terrain == "fire") return 18;
            if (terrain == "ice") return 19;
            if (type == "fire") return 2;
            if (type == "cold") return 8;
            if (type == "shock") return 9;
            if (type == "death") return 10;
            if (type == "mind") return 11;
            if (effect == "heal") return 0;
            if (effect == "cure") return 5;
            if (formula?.Status == "shield") return 4;
            if (formula?.Status == "regen") return 6;
            if (effect == "summon") return 1;
            if (school.Contains("ember")) return 20;
            if (school.Contains("hex")) return 21;
            if (school.Contains("mend")) return 22;
            if (school.Contains("pact")) return 23;
            return 24;
        }

        private void DrawFormulaRuneCode(Rect rect, string code, FormulaDef formula)
        {
            Color runeColor = formula == null ? gold : FormulaColor(formula);
            DrawRect(rect, Hex("080b0d", 0.92f));
            DrawBorder(rect, runeColor, 1);
            string normalized = string.IsNullOrEmpty(code) ? "" : code.ToUpperInvariant();
            float gap = 3f;
            float slotW = (rect.width - gap * 4f) / 3f;
            for (int i = 0; i < 3; i++)
            {
                Rect slot = new Rect(rect.x + gap + i * (slotW + gap), rect.y + 4f, slotW, rect.height - 11f);
                bool filled = i < normalized.Length;
                DrawRect(slot, filled ? Color.Lerp(runeColor, Hex("101619"), 0.62f) : Hex("101619", 0.82f));
                DrawBorder(slot, filled ? runeColor : line, 1);
                string letter = filled ? normalized[i].ToString() : "_";
                GUI.Label(slot, letter, CenterStyle(14, filled ? cursorWhite : muted));
            }

            string path = FormulaPathLabel(formula);
            GUI.Label(new Rect(rect.x, rect.y + rect.height - 10f, rect.width, 10f), path, CenterStyle(8, runeColor));
        }

        private string FormulaPathLabel(FormulaDef formula)
        {
            if (formula == null) return "spell";
            if (FormulaArcsOverCover(formula)) return "arc";
            if (FormulaRequiresLineOfSight(formula)) return "sight";
            if (formula.Target == "ally") return "rite";
            return "open";
        }

        private Rect FormulaLabIconRegion(FormulaDef formula, string schoolFallback)
        {
            if (IsMagicUiAtlas())
            {
                return MagicUiAtlasCell(MagicUiIconIndex(formula, schoolFallback));
            }

            if (formulaLabArt != null && formulaLabArt.width == 1448 && formulaLabArt.height == 1086)
            {
                string effectName = formula?.Effect ?? "";
                string typeName = formula?.DamageType ?? "";
                string terrainName = formula?.Terrain ?? "";
                if (terrainName == "tree") return SpellIconCell(0, 0);
                if (terrainName == "stone") return SpellIconCell(1, 0);
                if (formula?.Status == "shield") return SpellIconCell(3, 0);
                if (effectName == "heal" || effectName == "cure" || formula?.Status == "regen") return SpellIconCell(2, 0);
                if (terrainName == "fire" && formula?.Arc == true) return SpellIconCell(2, 1);
                if (terrainName == "fire") return SpellIconCell(1, 1);
                if (terrainName == "ice" || typeName == "cold") return SpellIconCell(3, 1);
                if (typeName == "fire") return SpellIconCell(0, 1);
                if (typeName == "shock") return SpellIconCell(0, 2);
                if (terrainName == "web" || formula?.Status == "web") return SpellIconCell(1, 2);
                if (terrainName == "gas" || typeName == "poison") return SpellIconCell(2, 2);
                if (typeName == "death" || typeName == "mind" || formula?.Status == "hex" || formula?.Status == "sleep") return SpellIconCell(3, 2);
                return SpellIconCell(2, 0);
            }

            string school = formula?.School ?? schoolFallback ?? "";
            string effect = formula?.Effect ?? "";
            string type = formula?.DamageType ?? "";
            string terrain = formula?.Terrain ?? "";
            if (terrain == "tree") return new Rect(954, 42, 68, 68);
            if (terrain == "stone") return new Rect(1034, 42, 68, 68);
            if (type == "death" || formula?.Code == "RLM") return new Rect(870, 42, 68, 68);
            if (type == "cold" || terrain == "ice") return new Rect(622, 42, 68, 68);
            if (type == "shock") return new Rect(704, 42, 68, 68);
            if (type == "fire" || terrain == "fire" || school.Contains("ember")) return new Rect(540, 42, 68, 68);
            if (effect == "status" || terrain == "web" || terrain == "gas" || school.Contains("hex")) return new Rect(788, 42, 68, 68);
            if (school.Contains("mend")) return new Rect(458, 42, 68, 68);
            return new Rect(458, 42, 68, 68);
        }

        private bool IsMagicUiAtlas()
        {
            return formulaLabArt != null && Mathf.Abs(formulaLabArt.width - formulaLabArt.height) < 8 && formulaLabArt.width >= 1000;
        }

        private Rect MagicUiAtlasCell(int index)
        {
            return AtlasCell(formulaLabArt, index, 4, 4);
        }

        private int MagicUiIconIndex(FormulaDef formula, string schoolFallback)
        {
            return CombatFeedbackRules.MagicUiIconIndex(formula, schoolFallback);
        }

        private bool TryDrawMagicUiAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsMagicUiAtlas()) return false;
            return DrawTextureRegionTint(formulaLabArt, rect, MagicUiAtlasCell(index), tint);
        }

        private int TerrainMagicIconIndex(string kind)
        {
            switch (kind)
            {
                case "tree": return 0;
                case "stone": return 1;
                case "fire": return 4;
                case "ice": return 5;
                case "web": return 8;
                case "gas": return 9;
                case "sanctuary": return 14;
                case "curse": return 10;
                case "glyph": return 11;
                case "demonrift": return 7;
                default: return -1;
            }
        }

        private Rect SpellIconCell(int col, int row)
        {
            const float size = 362f;
            return new Rect(col * size, row * size, size, size);
        }

        private bool ActionEnabled(ActionMode mode, CombatUnit active)
        {
            return CombatLifecycle().ActionEnabled(
                mode,
                active,
                active != null && !string.IsNullOrEmpty(active.Spell),
                HasMartialAbilities(active),
                state?.Elixirs ?? 0);
        }

        private bool CanInspectCombatPowerBook(ActionMode mode, CombatUnit active)
        {
            if (active == null || active.Side != UnitSide.Party) return false;
            if (mode == ActionMode.Cast)
            {
                return !string.IsNullOrEmpty(active.Spell)
                    && ActiveFormulaBook().Any(formula => formula != null && SchoolMatches(formula, active.Spell));
            }
            return mode == ActionMode.Ability && HasMartialAbilities(active);
        }

        private bool CombatCommandEnabled(ActionMode mode, CombatUnit active)
        {
            if (IsCombatResolutionPending()) return false;
            return ActionEnabled(mode, active) || CanInspectCombatPowerBook(mode, active);
        }

        private void SelectOrRunAction(ActionMode mode, CombatUnit active)
        {
            if (IsCombatResolutionPending()) return;
            bool changedMode = selectedAction != mode;
            selectedAction = mode;
            if (mode == ActionMode.Cast)
            {
                showSpellbook = CanInspectCombatPowerBook(ActionMode.Cast, active);
                showAbilityPanel = false;
                if (showSpellbook)
                {
                    showArmory = false;
                    showDialogue = false;
                    DismissLootPopupSilently();
                }
                ClearAbilityEntry();
                if (showSpellbook && string.IsNullOrEmpty(spellbookSelectedCode))
                {
                    FormulaDef first = GetFormula(pendingFormulaCode) ?? DefaultFormulaForCaster(active) ?? KnownFormulasFor(active).FirstOrDefault();
                    if (first != null) spellbookSelectedCode = first.Code;
                }
            }
            else if (mode == ActionMode.Ability)
            {
                showAbilityPanel = CanInspectCombatPowerBook(ActionMode.Ability, active);
                showSpellbook = false;
                if (showAbilityPanel)
                {
                    showArmory = false;
                    showDialogue = false;
                    DismissLootPopupSilently();
                }
                ClearFormulaEntry();
                if (showAbilityPanel && string.IsNullOrEmpty(abilitySelectedId))
                {
                    MartialAbility first = AbilityDef(pendingAbilityId) ?? MartialAbilitiesFor(active).FirstOrDefault();
                    if (first != null) abilitySelectedId = first.Id;
                }
            }
            else
            {
                showSpellbook = false;
                showAbilityPanel = false;
                ClearFormulaEntry();
                ClearAbilityEntry();
            }
            if (state?.Combat != null)
            {
                bool choosingTarget = mode == ActionMode.Move
                    || mode == ActionMode.Attack
                    || (mode == ActionMode.Cast && !string.IsNullOrEmpty(pendingFormulaCode))
                    || (mode == ActionMode.Ability && !string.IsNullOrEmpty(pendingAbilityId));
                state.Combat.Phase = choosingTarget ? CombatPhase.ChooseTarget : CombatPhase.ChooseAction;
            }
            if (changedMode && (mode == ActionMode.Move || mode == ActionMode.Attack))
            {
                PlaySfx("uitab", 0.28f);
            }
            else if (changedMode && ((mode == ActionMode.Cast && showSpellbook) || (mode == ActionMode.Ability && showAbilityPanel)))
            {
                PlaySfx("uiopen", 0.38f);
            }
            if (mode == ActionMode.Guard)
            {
                int guardBonus = GuardActionBonus(active);
                CombatCommandResult result = CombatLifecycle().Guard(active, guardBonus);
                if (!result.Success) return;
                ImproveSkill(active, "guard", 1);
                PushLog($"{active.Name} guards the line{(active.GuardBonus >= 4 ? " from a braced stance" : "")}.", Tone.Normal);
                PlaySfx("guard", 0.82f);
                AfterCombatAction(active);
            }
            else if (mode == ActionMode.Elixir)
            {
                UseElixir();
            }
            else if (mode == ActionMode.Wait)
            {
                CombatCommandResult result = CombatLifecycle().EndTurn(active);
                if (!result.Success) return;
                PushLog($"{active.Name} ends the turn.", Tone.Normal);
                PlaySfx("ui", 0.55f);
                AfterCombatAction(active);
            }
        }

        private void DiscoverCurrentZone(bool force)
        {
            if (state?.Map == null) return;
            if (state.DiscoveredZones == null) state.DiscoveredZones = new List<string>();
            WorldZone zone = ZoneAt(state.PlayerX, state.PlayerY);
            if (zone == null || string.IsNullOrEmpty(zone.Id)) return;
            string key = ZoneKey(state.Depth, zone.Id);
            if (!force && state.DiscoveredZones.Contains(key)) return;
            bool fresh = !state.DiscoveredZones.Contains(key);
            if (fresh) state.DiscoveredZones.Add(key);
            PushLog($"{zone.Name}: {zone.Story}", zone.Danger >= 3 ? Tone.Warn : Tone.Normal);
            if (fresh && !force)
            {
                int xp = Mathf.Max(6, 10 + zone.Danger * 4 + state.Depth * 2);
                AwardWorldExperience(xp, $"{zone.Name} discovered");
                AddBurst(state.PlayerX, state.PlayerY, ZoneDangerColor(zone));
            }
        }

        private string ZoneKey(int depth, string zoneId)
        {
            return $"{Mathf.Max(1, depth)}:{zoneId ?? ""}";
        }

        private bool HasStoryFlag(string flag)
        {
            return state?.StoryFlags != null && state.StoryFlags.Contains(flag);
        }

        private void SetStoryFlag(string flag)
        {
            if (state == null || string.IsNullOrEmpty(flag)) return;
            if (state.StoryFlags == null) state.StoryFlags = new List<string>();
            if (!state.StoryFlags.Contains(flag)) state.StoryFlags.Add(flag);
        }

        private const string KoboldStoryCaveId = "dusk-market-smoke-cave";

        private bool IsKoboldStoryCave(MapObject cave)
        {
            if (state?.Map == null || state.Depth != 2 || cave == null || cave.Type != ObjectType.Cave) return false;
            return string.Equals(cave.Id, KoboldStoryCaveId, StringComparison.Ordinal)
                && ZoneIdFor(cave.X, cave.Y, state.Map, state.Depth) == "dusk-market";
        }

        private bool MaybeTriggerKoboldAmbush()
        {
            if (!ContentSetCatalog.AllowKoboldChapter(activeContentSet, state?.StoryFlags)) return false;
            if (state?.Map == null || state.Mode != GameMode.Explore) return false;
            if (state.Depth != 2 || HasStoryFlag(StoryFlags.KoboldAmbushSurvived) || HasStoryFlag(StoryFlags.KoboldKingDefeated)) return false;
            if (ZoneAt(state.PlayerX, state.PlayerY)?.Id != "dusk-market") return false;

            SetStoryFlag(StoryFlags.KoboldAmbushSprung);
            state.ActiveStory = "Chapter II: Kobold Smoke. Survive the Dusk Market ambush and find the cave mouth behind the bone charms.";
            PushLog("Bone whistles snap from the broken stalls. Kobolds pour out before the party can form a clean line.", Tone.Warn);
            ShowBanner("Kobold Ambush");
            StartCombat(EncounterId.KoboldAmbush);
            return true;
        }

        private void EnsureKoboldKingCaveMarker()
        {
            if (!ContentSetCatalog.AllowKoboldChapter(activeContentSet, state?.StoryFlags)) return;
            if (state?.Map?.Objects == null || state.Depth != 2 || HasStoryFlag(StoryFlags.KoboldKingDefeated)) return;
            if (state.Map.FindObjectById(KoboldStoryCaveId) != null) return;
            MapObject existingDuskCave = state.Map.Objects
                .Where(o => o != null
                    && o.Type == ObjectType.Cave
                    && ZoneIdFor(o.X, o.Y, state.Map, state.Depth) == "dusk-market")
                .OrderByDescending(o => Distance(o.X, o.Y, state.Map.StartX, state.Map.StartY))
                .FirstOrDefault();
            if (existingDuskCave != null)
            {
                existingDuskCave.Id = KoboldStoryCaveId;
                state.Map.InvalidateObjectLookup();
                return;
            }

            List<Point> candidates = new List<Point>();
            for (int y = 1; y < state.Map.Height - 1; y++)
            for (int x = 1; x < state.Map.Width - 1; x++)
            {
                if (TileAt(state.Map, x, y) == 1
                    && ObjectAt(state.Map, x, y) == null
                    && CanPlaceGeneratedExploreObject(state.Map, x, y, ObjectType.Cave)
                    && ZoneIdFor(x, y, state.Map, state.Depth) == "dusk-market")
                {
                    candidates.Add(new Point(x, y));
                }
            }

            Point chosen = candidates
                .OrderByDescending(p => Distance(p.X, p.Y, state.Map.StartX, state.Map.StartY))
                .FirstOrDefault();
            if (chosen != null)
            {
                state.Map.Objects.Add(new MapObject(chosen.X, chosen.Y, ObjectType.Cave, KoboldStoryCaveId));
                state.Map.InvalidateObjectLookup();
            }
        }

        private MapObject FindKoboldStoryCave()
        {
            if (!ContentSetCatalog.AllowKoboldChapter(activeContentSet, state?.StoryFlags)) return null;
            if (state?.Map?.Objects == null || state.Depth != 2) return null;
            EnsureKoboldKingCaveMarker();
            return state.Map.FindObjectById(KoboldStoryCaveId);
        }

        private Point BestOpenNeighbor(int x, int y)
        {
            if (state?.Map == null) return null;
            Point[] choices =
            {
                new Point(x - 1, y),
                new Point(x + 1, y),
                new Point(x, y - 1),
                new Point(x, y + 1),
                new Point(x - 1, y - 1),
                new Point(x + 1, y + 1),
                new Point(x - 1, y + 1),
                new Point(x + 1, y - 1)
            };
            return choices
                .Where(p => TileAt(state.Map, p.X, p.Y) == 1 && ExplorationTraversalRules.CanStandOnObject(ObjectAt(state.Map, p.X, p.Y)))
                .OrderBy(p => Distance(p.X, p.Y, state.Map.StartX, state.Map.StartY))
                .FirstOrDefault();
        }

        private void ResolveExploreTile()
        {
            MapObject obj = ObjectAt(state.Map, state.PlayerX, state.PlayerY);
            if (obj == null)
            {
                WorldZone zone = ZoneAt(state.PlayerX, state.PlayerY);
                if (zone == null || zone.Danger <= 0) return;
                if (!ContentSetCatalog.AllowPrototypeRouteTriggers(activeContentSet, state.StoryFlags)) return;
                if (rng.NextDouble() < 0.015 + state.Depth * 0.003 + Mathf.Clamp(zone.Danger, 0, 4) * 0.004) StartCombat(EncounterId.Patrol);
                return;
            }

            ResolveExploreObject(obj);
        }

        private void ResolveExploreObject(MapObject obj)
        {
            if (obj == null) return;
            if (TryResolveRegionalSite(obj)) return;

            if (obj.Type == ObjectType.Cache)
            {
                int foundGold = rng.Next(12, 29) + state.Depth * 4;
                InventoryItem item = MakeItem();
                int foundElixirs = rng.NextDouble() < 0.5 ? 1 : 0;
                int foundSupplies = rng.NextDouble() < 0.55 ? 1 : 0;
                state.Gold += foundGold;
                state.Elixirs += foundElixirs;
                state.Supplies += foundSupplies;
                state.Inventory.Add(item);
                string equipNote = AutoEquipItem(item);
                ShowLootPanel(item, foundGold, foundSupplies, foundElixirs, equipNote);
                RemoveObject(obj);
                PushLog($"A sealed cache yields {foundGold} gold{CacheSupplyLine(foundSupplies, foundElixirs)} and {item.DisplayName}. {equipNote}", Tone.Good);
                ShowBanner("Cache opened");
                PlaySfx("cache");
                AddBurst(state.PlayerX, state.PlayerY, gold);
                AwardWorldExperience(8 + state.Depth * 3, "Cache charted");
            }
            else if (obj.Type == ObjectType.Shrine)
            {
                foreach (PartyMember member in state.Party.Where(p => p.Hp > 0))
                {
                    member.Hp = Mathf.Min(member.MaxHp, member.Hp + 9 + state.Depth * 2);
                    member.Mana = Mathf.Min(member.MaxMana, member.Mana + 5);
                }
                RemoveObject(obj);
                PushLog("An old shrine steadies the party.", Tone.Good);
                ShowBanner("Shrine restored");
                PlaySfx("shrine");
                AddBurst(state.PlayerX, state.PlayerY, teal);
                AwardWorldExperience(6 + state.Depth * 2, "Shrine restored");
            }
            else if (obj.Type == ObjectType.Camp)
            {
                int suppliesBefore = state.Supplies;
                Camp();
                ShowBanner(state.Supplies < suppliesBefore ? "Camp Rest" : "Campfire");
            }
            else if (obj.Type == ObjectType.Encounter)
            {
                if (!ContentSetCatalog.AllowPrototypeRouteTriggers(activeContentSet, state.StoryFlags))
                {
                    PushLog("This patrol marker is disabled during the sewer-slice test path.", Tone.Normal);
                    ShowBanner("Patrol Disabled");
                    PlaySfx("blocked", 0.52f);
                    return;
                }
                RemoveObject(obj);
                StartCombat(EncounterId.Guard);
            }
            else if (obj.Type == ObjectType.Cave)
            {
                if (ContentSetCatalog.AllowKoboldChapter(activeContentSet, state.StoryFlags)
                    && HasStoryFlag(StoryFlags.KoboldAmbushSurvived)
                    && IsKoboldStoryCave(obj)
                    && !HasStoryFlag(StoryFlags.KoboldKingDefeated))
                {
                    SetStoryFlag(StoryFlags.KoboldCaveFound);
                    if (!HasStoryFlag(StoryFlags.KoboldCaveCleared))
                    {
                        state.ActiveStory = "Chapter II: Kobold Smoke. Clear the smoke caves, then return to the cave mouth for the king's hall.";
                        PushLog("Drums answer from the cave mouth. Kobold shields scrape over stone.", Tone.Warn);
                        ShowBanner("Kobold Cave");
                        StartCombat(EncounterId.KoboldCave);
                    }
                    else
                    {
                        state.ActiveStory = "Chapter II: Kobold Smoke. The cave mouth opens onto the Kobold King's shield hall.";
                        PushLog("The cleared cave drops into a shield-lined hall. A crowned kobold voice barks for torches.", Tone.Warn);
                        ShowBanner("Kobold King");
                        StartCombat(EncounterId.KoboldKing);
                    }
                }
                else
                {
                    PushLog("The cave mouth exhales cold air. This side passage is marked for a later route.", Tone.Normal);
                    ShowBanner("Cave Mouth");
                    PlaySfx("ui", 0.62f);
                }
            }
            else if (obj.Type == ObjectType.Stairs)
            {
                PushLog("A stairway sinks into a colder dark.", Tone.Normal);
                ShowBanner("Stairs found");
                PlaySfx("door", 0.62f);
            }
            else if (obj.Type == ObjectType.Town)
            {
                foreach (PartyMember member in state.Party.Where(p => p.Hp > 0))
                {
                    member.Hp = Mathf.Min(member.MaxHp, member.Hp + 16);
                    member.Mana = Mathf.Min(member.MaxMana, member.Mana + 10);
                }
                PushLog($"{HomeTownName} opens its lamps to the party.", Tone.Good);
                PushLog(state.ActiveStory, Tone.Normal);
                ShowBanner(HomeTownName);
                PlaySfx("shrine", 0.75f);
            }
            else if (obj.Type == ObjectType.QuestBoard)
            {
                // The Midgaard board is part of the normal sewer-slice city, even
                // though QuestBoard is also reused by the optional route scaffold.
                // Dispatch it before the prototype-content gate so the advertised
                // Talk action always opens the city notices dialogue.
                VisitMidgaardQuestBoard(!HasStoryFlag(StoryFlags.MidgaardCityBoardSeen));
            }
            else if (IsRouteScaffoldObject(obj.Type))
            {
                if (ContentSetCatalog.AllowPrototypeRouteTriggers(activeContentSet, state.StoryFlags)) ResolveRouteScaffoldTile(obj);
                else
                {
                    PushLog("This route marker belongs to the prototype scaffold and is disabled for normal sewer-slice play.", Tone.Normal);
                    ShowBanner("Prototype Route");
                    PlaySfx("blocked", 0.52f);
                }
            }
            else if (IsMidgaardTownObject(obj.Type))
            {
                ResolveMidgaardTile(obj);
            }
        }

        private bool TryResolveRegionalSite(MapObject obj)
        {
            if (!TryRegionalSite(state?.Map, obj, out WorldMapSite site)) return false;
            string visitedFlag = $"regional_site_{Mathf.Max(1, state.Depth)}_{SanitizeFlagPart(site.Id)}_charted";
            bool firstVisit = !HasStoryFlag(visitedFlag);
            if (firstVisit)
            {
                SetStoryFlag(visitedFlag);
                SetStoryFlag(RouteScaffoldFlag(obj));
                AwardWorldExperience(10 + state.Depth * 2, site.Name + " charted");
                PushLog(site.Name + " is added to the party's road chart. " + site.Summary, Tone.Good);
            }
            else
            {
                PushLog(site.Summary, Tone.Normal);
            }
            ShowBanner(site.Name);
            PlaySfx(WorldSitePresentationRules.InspectCueFor(site.Id), 0.68f);
            return true;
        }

        private void ResolveRouteScaffoldTile(MapObject obj)
        {
            if (!ContentSetCatalog.AllowPrototypeRouteTriggers(activeContentSet, state?.StoryFlags))
            {
                PushLog("Prototype route scaffolds are disabled in the sewer-slice content set.", Tone.Normal);
                PlaySfx("blocked", 0.52f);
                return;
            }
            RouteScaffoldDef def = RouteScaffoldFor(obj);
            if (def == null)
            {
                PushLog("This road marker is reserved for future content.", Tone.Normal);
                PlaySfx("ui", 0.52f);
                return;
            }

            string flag = RouteScaffoldFlag(obj);
            bool firstVisit = !HasStoryFlag(flag);
            if (firstVisit) SetStoryFlag(flag);
            SetStoryFlag("route_scaffold_seen_" + SanitizeFlagPart(def.ZoneId));
            PushLog($"{def.Name}: {def.Summary}", firstVisit ? Tone.Good : Tone.Normal);
            ShowBanner(def.Name);

            switch (obj.Type)
            {
                case ObjectType.QuestBoard:
                    VisitMidgaardQuestBoard(firstVisit);
                    break;
                case ObjectType.Waystone:
                    RestoreLivingParty(6 + state.Depth, 4 + state.Depth);
                    if (firstVisit) AwardWorldExperience(6 + state.Depth * 2, "Waystone mapped");
                    AddBurst(state.PlayerX, state.PlayerY, teal);
                    PlaySfx("shrine", 0.72f);
                    break;
                case ObjectType.TrainingGround:
                    if (firstVisit)
                    {
                        foreach (PartyMember member in state.Party.Where(p => p.Hp > 0)) member.SkillPoints += 1;
                        PushLog("Everyone gains 1 unspent skill point for future class-training UI tests.", Tone.Good);
                        AwardWorldExperience(8 + state.Depth * 2, "Training ring tested");
                    }
                    PlaySfx("guard", 0.74f);
                    break;
                case ObjectType.LoreLibrary:
                    if (firstVisit)
                    {
                        foreach (PartyMember member in state.Party.Where(p => p.Hp > 0 && !string.IsNullOrEmpty(p.Spell))) member.SkillPoints += 1;
                        PushLog("Spellcasters gain 1 unspent skill point. This is the future home of formula lessons and library events.", Tone.Good);
                        AwardWorldExperience(10 + state.Depth * 2, "Lore shelf translated");
                    }
                    AddBurst(state.PlayerX, state.PlayerY, frost);
                    PlaySfx("spell", 0.72f);
                    break;
                case ObjectType.ForgeSite:
                    if (firstVisit)
                    {
                        EnsureInventoryList();
                        InventoryItem item = MakeRouteScaffoldItem(def);
                        state.Inventory.Add(item);
                        string equipNote = AutoEquipItem(item);
                        ShowLootPanel(item, 0, 0, 0, equipNote);
                        PushLog($"The forge scaffold yields {item.DisplayName}. {equipNote}", Tone.Good);
                        AwardWorldExperience(8 + state.Depth * 2, "Forge site catalogued");
                    }
                    PlaySfx("cache", 0.62f);
                    break;
                case ObjectType.FactionCamp:
                    if (firstVisit)
                    {
                        state.Supplies += 2;
                        SetStoryFlag("route_contact_" + SanitizeFlagPart(def.ZoneId));
                        PushLog("A future faction contact is now marked in the Journal. Supplies +2 for the scouting cache.", Tone.Good);
                        AwardWorldExperience(8 + state.Depth * 2, "Faction camp found");
                    }
                    PlaySfx("ui", 0.62f);
                    break;
                case ObjectType.DungeonGate:
                case ObjectType.DeepCrypt:
                    if (firstVisit)
                    {
                        PushLog("This scaffold starts a placeholder fight now; later it can become an authored multi-room dungeon.", Tone.Warn);
                        PlaySfx("encounter", 0.76f);
                        StartCombat(EncounterId.Guard);
                    }
                    else
                    {
                        PushLog("The entrance has been scouted. A later build can attach rooms, locks, keys, and boss tables here.", Tone.Normal);
                        PlaySfx("ui", 0.54f);
                    }
                    break;
                case ObjectType.AncientGrove:
                    RestoreLivingParty(4, 3);
                    if (firstVisit)
                    {
                        state.Elixirs += 1;
                        PushLog("The grove stores a future hazard/puzzle route. For now it grants an elixir and a little recovery.", Tone.Good);
                        AwardWorldExperience(8 + state.Depth * 2, "Ancient grove marked");
                    }
                    AddBurst(state.PlayerX, state.PlayerY, poison);
                    PlaySfx("shrine", 0.68f);
                    break;
                case ObjectType.PortalSeal:
                    if (firstVisit)
                    {
                        state.ActiveStory = "The Red Gate seal is now marked. Future builds can attach keys, faction choices, and late-route boss gates here.";
                        PushLog(state.ActiveStory, Tone.Warn);
                        AwardWorldExperience(12 + state.Depth * 2, "Portal seal studied");
                    }
                    AddBurst(state.PlayerX, state.PlayerY, blood);
                    PlaySfx("spell", 0.82f);
                    break;
            }
        }

        private InventoryItem MakeRouteScaffoldItem(RouteScaffoldDef def)
        {
            InventoryItem item = MakeItem();
            if (def != null && def.Type == ObjectType.ForgeSite)
            {
                item.Mark = "quarry";
                item.Material = "worked iron";
                item.Trait = "guarding";
                item.Slot = "armor";
                item.Form = "ring mail";
                item.Bonus = Mathf.Max(2, item.Bonus);
                item.HealthBonus = Mathf.Max(1, item.HealthBonus);
                item.Rarity = "scaffold";
                item.DisplayName = "+2 quarry worked iron ring mail";
            }
            else
            {
                item.Mark = "road-marked";
                item.Rarity = "scaffold";
                item.DisplayName = string.IsNullOrEmpty(item.DisplayName) ? "road-marked cache item" : "road-marked " + item.DisplayName;
            }
            return item;
        }

        private bool IsMidgaardTownObject(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Market:
                case ObjectType.Temple:
                case ObjectType.Fountain:
                case ObjectType.Diner:
                case ObjectType.Tavern:
                case ObjectType.Armorer:
                case ObjectType.WeaponVendor:
                case ObjectType.Enchanter:
                case ObjectType.NorthGate:
                case ObjectType.SouthGate:
                case ObjectType.EastGate:
                case ObjectType.WestGate:
                case ObjectType.TownGuard:
                case ObjectType.KingHall:
                case ObjectType.Sewer:
                case ObjectType.CityWall:
                case ObjectType.Provisions:
                case ObjectType.RatPeltQuest:
                case ObjectType.RecallCircle:
                case ObjectType.MarketClerk:
                case ObjectType.TempleHealer:
                case ObjectType.TavernKeeper:
                case ObjectType.GateCaptain:
                case ObjectType.CityCourier:
                case ObjectType.WoundedTraveler:
                case ObjectType.StableHand:
                case ObjectType.RoyalHerald:
                case ObjectType.NoviceHealer:
                case ObjectType.OldRoadScout:
                case ObjectType.InteriorDoor:
                case ObjectType.KingHalvard:
                case ObjectType.ArmorerNpc:
                case ObjectType.WeaponMerchantNpc:
                case ObjectType.EnchanterNpc:
                case ObjectType.DinerCook:
                case ObjectType.Provisioner:
                case ObjectType.DockWorker:
                case ObjectType.Scholar:
                    return true;
                default:
                    return false;
            }
        }

        private void ResolveMidgaardTile(MapObject obj)
        {
            if (obj == null) return;
            if (TryUseMidgaardPortal(obj)) return;
            switch (obj.Type)
            {
                case ObjectType.Market:
                    SetStoryFlag(StoryFlags.MidgaardMarketFound);
                    PushLog("Market Square holds the roads together: temple north, gates east and west, vendors along the stones.", Tone.Normal);
                    ShowBanner("Market Square");
                    PlaySfx("ui", 0.55f);
                    break;
                case ObjectType.MarketClerk:
                    VisitMarketClerk();
                    break;
                case ObjectType.CityCourier:
                    VisitCityCourier();
                    break;
                case ObjectType.Temple:
                case ObjectType.Fountain:
                case ObjectType.RecallCircle:
                    RestorePartyFully();
                    SetStoryFlag(StoryFlags.MidgaardTempleFound);
                    PushLog("Temple Square quiets the party. You feel at peace here.", Tone.Good);
                    ShowBanner("Temple Square");
                    PlaySfx("shrine", 0.88f);
                    AddBurst(state.PlayerX, state.PlayerY, teal);
                    break;
                case ObjectType.TempleHealer:
                    VisitTempleHealer();
                    break;
                case ObjectType.NoviceHealer:
                    VisitNoviceHealer();
                    break;
                case ObjectType.Diner:
                case ObjectType.Provisions:
                    VisitKatesDiner(obj.Type == ObjectType.Provisions);
                    break;
                case ObjectType.DinerCook:
                    VisitKatesDiner(false, ObjectType.DinerCook);
                    break;
                case ObjectType.Provisioner:
                    VisitKatesDiner(true, ObjectType.Provisioner);
                    break;
                case ObjectType.DockWorker:
                    VisitDockWorker();
                    break;
                case ObjectType.Scholar:
                    VisitMidgaardScholar();
                    break;
                case ObjectType.Tavern:
                    VisitMidgaardTavern();
                    break;
                case ObjectType.TavernKeeper:
                    VisitTavernKeeper();
                    break;
                case ObjectType.WoundedTraveler:
                    VisitWoundedTraveler();
                    break;
                case ObjectType.StableHand:
                    VisitStableHand();
                    break;
                case ObjectType.Armorer:
                case ObjectType.RatPeltQuest:
                    if (!TryCompleteRatPeltArmor()) VisitMidgaardArmorer();
                    break;
                case ObjectType.ArmorerNpc:
                    if (!TryCompleteRatPeltArmor()) VisitMidgaardArmorer();
                    break;
                case ObjectType.WeaponVendor:
                    VisitWeaponVendor();
                    break;
                case ObjectType.WeaponMerchantNpc:
                    VisitWeaponVendor();
                    break;
                case ObjectType.Enchanter:
                    VisitWeaponEnchanter();
                    break;
                case ObjectType.EnchanterNpc:
                    VisitWeaponEnchanter();
                    break;
                case ObjectType.KingHall:
                case ObjectType.KingHalvard:
                    VisitKingHall();
                    break;
                case ObjectType.RoyalHerald:
                    VisitRoyalHerald();
                    break;
                case ObjectType.Sewer:
                    EnterMidgaardSewer();
                    break;
                case ObjectType.NorthGate:
                    PushLog("The North Gate is barred for city watch work. The east and west roads remain Midgaard's open exits.", Tone.Warn);
                    ShowBanner("North Gate sealed");
                    PlaySfx("gatebarred", 0.68f);
                    break;
                case ObjectType.SouthGate:
                    PushLog("The South Gate is sealed against the cistern works below. Use the sewer grate or the east and west roads.", Tone.Warn);
                    ShowBanner("South Gate sealed");
                    PlaySfx("gatebarred", 0.68f);
                    break;
                case ObjectType.EastGate:
                    MarkGateSurveyVisit("east");
                    PushLog("The East Gate opens onto the Dusk Market road. The guard keeps the north and south walls sealed.", Tone.Normal);
                    ShowBanner("East Gate");
                    PlaySfx("gateopen", 0.66f);
                    break;
                case ObjectType.WestGate:
                    MarkGateSurveyVisit("west");
                    PushLog("The West Gate leads toward the green shrine road and old quarry. There is no north or south gate.", Tone.Normal);
                    ShowBanner("West Gate");
                    PlaySfx("gateopen", 0.66f);
                    break;
                case ObjectType.GateCaptain:
                    VisitGateCaptain();
                    break;
                case ObjectType.OldRoadScout:
                    VisitOldRoadScout();
                    break;
                case ObjectType.TownGuard:
                    VisitTownGuardAt(obj);
                    break;
                case ObjectType.CityWall:
                    PushLog("Midgaard's wall is unbroken here. Only the east and west gates are open.", Tone.Warn);
                    PlaySfx("blocked", 0.62f);
                    break;
            }
        }

        private void RestorePartyFully()
        {
            if (state?.Party == null) return;
            foreach (PartyMember member in state.Party)
            {
                member.Hp = member.MaxHp;
                member.Mana = member.MaxMana;
            }
        }

        private void RestoreLivingParty(int hp, int mana)
        {
            if (state?.Party == null) return;
            foreach (PartyMember member in state.Party.Where(p => p.Hp > 0))
            {
                member.Hp = Mathf.Min(member.MaxHp, member.Hp + hp);
                member.Mana = Mathf.Min(member.MaxMana, member.Mana + mana);
            }
        }

        private void VisitMidgaardQuestBoard(bool firstVisit)
        {
            SetStoryFlag(StoryFlags.MidgaardCityBoardSeen);
            if (firstVisit)
            {
                state.Supplies += 1;
                AwardWorldExperience(4, "Quest board checked");
            }

            bool showCityErrands = ContentSetCatalog.ShowPrototypeScaffold(activeContentSet);
            string lamp = showCityErrands
                ? HasStoryFlag(StoryFlags.MidgaardLampRoundComplete)
                    ? "Mira's Lamp Round complete"
                    : HasStoryFlag(StoryFlags.MidgaardLampRoundStarted)
                        ? "Mira's Lamp Round: " + LampRoundStatusLine()
                        : "Mira by Temple Square needs a lamp round through market, diner, and tavern."
                : !HasStoryFlag(StoryFlags.MidgaardRatQuestGiven)
                    ? "Royal notice: King Halvard is hearing parties for work in the old cisterns."
                    : MidgaardRatPeltsReady()
                        ? "Cistern writ complete: take the recovered proof to Borin's forge."
                        : "Cistern writ: " + ContentSetCatalog.SewerSliceClearedCount(state.StoryFlags) + " of 3 chambers cleared.";
            string gate = showCityErrands
                ? HasStoryFlag(StoryFlags.MidgaardGateSurveyComplete)
                    ? "Gate Survey complete"
                    : HasStoryFlag(StoryFlags.MidgaardGateSurveyStarted)
                        ? "Gate Survey: " + GateSurveyStatusLine()
                        : "The gate captain wants both city gates checked."
                : "City services: healing at Temple Square, provisions at Kate's or Lute's, and road reports at either open gate.";

            PushLog($"Quest board: {lamp} {gate}", firstVisit ? Tone.Good : Tone.Normal);
            ShowBanner("Midgaard Quest Board");
            ShowDialogue(
                "Midgaard Quest Board",
                "City Notices",
                lamp + "\n\n" + gate,
                ObjectType.QuestBoard,
                gold);
            PlaySfx("ui", 0.62f);
        }

        private void VisitMarketClerk()
        {
            SetStoryFlag(StoryFlags.MidgaardMarketFound);
            MarkLampRoundVisit("market", "Market Clerk Nessa");
            PushLog("Nessa the market clerk stamps the city ledger and points out the safest lane to Kate's Diner.", Tone.Normal);
            ShowBanner("Market Clerk");
            ShowNessaConversation();
            PlaySfx("ui", 0.54f);
        }

        private void ShowNessaConversation(string greeting = null)
        {
            ShowDialogueChoices(
                "Market Ledger",
                "Nessa",
                string.IsNullOrWhiteSpace(greeting)
                    ? "You're new. I'd remember you. Need the safe way across town, the honest shops, or today's rumor?"
                    : greeting,
                ObjectType.MarketClerk,
                gold,
                new[]
                {
                    MakeDialogueChoice("lanes", "Which streets are safe?", ""),
                    MakeDialogueChoice("vendors", "Where should we shop?", "Armor, weapons, food, and enchantment."),
                    MakeDialogueChoice("rumors", "What are people saying?", "The trouble below the city and beyond its walls.")
                },
                ResolveNessaDialogueChoice);
        }

        private void ResolveNessaDialogueChoice(string choice)
        {
            switch (choice)
            {
                case "vendors":
                    ShowDialogueResponse("Market Ledger", "Nessa", "Borin works armor west of the square. Tessa keeps the weapon rack nearby, and Maud has the rune anvil. All three know their work. Just see Kate or Lute before you spend your last coin on steel. An empty pack makes poor company in the cisterns.", ObjectType.MarketClerk, gold, () => ShowNessaConversation("What else? The ledger can wait a moment."));
                    break;
                case "rumors":
                    ShowDialogueResponse("Market Ledger", "Nessa", MidgaardRatPeltsReady()
                        ? "Your cistern work reached the market before you did. Now the wagoners have moved on to kobold drums east of the wall and odd lights along Green Shrine Road. Cheerful lot, wagoners."
                        : "Three names keep coming back: Broken Sluice, Foul Runoff, and the Cistern Den. They say the last one holds a plague caster behind a wall of brutes. I'd call it tavern fog, but too many sober people tell the same story.", ObjectType.MarketClerk, gold, () => ShowNessaConversation("That's the rumor. Was there something else?"));
                    break;
                default:
                    ShowDialogueResponse("Market Ledger", "Nessa", "Take the pale paving to Temple Square, then follow the south lamps to Kate's and Orren's tavern. The watch walks that whole loop. If the paving runs out or the houses go dark, you've gone too far. Turn back.", ObjectType.MarketClerk, gold, () => ShowNessaConversation("Got the route? Good. Anything else?"));
                    break;
            }
        }

        private DialogueChoiceView MakeDialogueChoice(string id, string label, string hint = "", bool enabled = true)
        {
            return new DialogueChoiceView
            {
                Id = id,
                Label = label,
                Hint = hint,
                Enabled = enabled
            };
        }

        private void VisitTownGuard()
        {
            VisitTownGuardAt(null);
        }

        private void VisitTownGuardAt(MapObject guard)
        {
            if (IsEastMidgaardGuard(guard))
            {
                PushLog("Watchwoman Ilyra lowers her spear just enough to talk without surrendering the road.", Tone.Normal);
                ShowBanner("East Gate Watch");
                ShowIlyraConversation();
                PlaySfx("guard", 0.52f);
                return;
            }

            PushLog("Watchman Rusk taps the spear butt twice and gives the party his full attention.", Tone.Normal);
            ShowBanner("West Gate Watch");
            ShowRuskConversation();
            PlaySfx("guard", 0.52f);
        }

        private bool IsEastMidgaardGuard(MapObject guard)
        {
            return guard != null && state?.Map != null && guard.X > state.Map.StartX;
        }

        private void ShowRuskConversation()
        {
            ShowDialogueChoices(
                "Midgaard Watch",
                "Watchman Rusk",
                "West Gate is quiet for the moment. I can tell you which streets the watch still walks, what we learned from the cisterns, or why only two gates are open.",
                ObjectType.TownGuard,
                stone,
                new[]
                {
                    MakeDialogueChoice("lamps", "Which streets do you patrol?", "The safest route between Midgaard's main landmarks."),
                    MakeDialogueChoice("sewer", "What did the cisterns teach you?", "How ratfolk brutes protect a plague mage."),
                    MakeDialogueChoice("gates", "Why are only two gates open?", "The watch's reason for sealing the north and south exits.")
                },
                ResolveRuskDialogueChoice);
        }

        private void ResolveRuskDialogueChoice(string choice)
        {
            switch (choice)
            {
                case "sewer":
                    ShowDialogueResponse("Midgaard Watch", "Watchman Rusk", MidgaardRatPeltsReady()
                        ? "You saw the trick for yourself. The brutes make enough noise to keep every eye on them while the plague mage poisons the open ground. You came back because you did not let the loudest enemy choose your target."
                        : "The big ratfolk are there to hold your attention. The plague mage behind them poisons whoever it can see and fouls the floor with gas. Step out of the cloud, use a pillar to break the ordinary shot, and close on the mage before you spend all your strength on the brute.", ObjectType.TownGuard, stone, ShowRuskConversation);
                    break;
                case "gates":
                    ShowDialogueResponse("Midgaard Watch", "Watchman Rusk", "We can see the approaches to the east and west gates, so those remain open. The north gate is under repair, and the south gate stands over the old cistern works. Until both are secure, no captain here will pretend otherwise.", ObjectType.TownGuard, stone, ShowRuskConversation);
                    break;
                default:
                    ShowDialogueResponse("Midgaard Watch", "Watchman Rusk", "Stay on the pale paving between Market Square, the temple, Kate's, and Orren's tavern. We check those lamps every watch. If you find an unlit stretch or a whole row of shuttered houses, come back and tell us. Do not go looking for the reason alone.", ObjectType.TownGuard, stone, ShowRuskConversation);
                    break;
            }
        }

        private void ShowIlyraConversation()
        {
            ShowDialogueChoices(
                "East Gate Watch",
                "Watchwoman Ilyra",
                "Keep one eye on the east road while we talk. Kobolds like a distracted traveler. What do you need to know?",
                ObjectType.TownGuard,
                teal,
                new[]
                {
                    MakeDialogueChoice("signals", "What signs do you watch?", "Tracks, bone charms, and sudden silence on the road."),
                    MakeDialogueChoice("shamans", "How do we stop a shaman?", "Surviving the bone hex and closing with steel."),
                    MakeDialogueChoice("retreat", "When should we turn back?", "The east watch's rule for bringing everyone home.")
                },
                ResolveIlyraDialogueChoice);
        }

        private void ResolveIlyraDialogueChoice(string choice)
        {
            switch (choice)
            {
                case "shamans":
                    ShowDialogueResponse(
                        "East Gate Watch",
                        "Watchwoman Ilyra",
                        "A shaman's bone hex can arc over cover, so do not huddle behind one stone and wait it out. Spread your people, close quickly, and use steel. Their charms can cloud a mind and web the ground, but they do not turn a blade.",
                        ObjectType.TownGuard,
                        teal,
                        ShowIlyraConversation);
                    break;
                case "retreat":
                    ShowDialogueResponse(
                        "East Gate Watch",
                        "Watchwoman Ilyra",
                        "Leave while you still have food for the walk home and enough strength to help one another. If two people are doing the work of four, the road has already given you its answer.",
                        ObjectType.TownGuard,
                        teal,
                        ShowIlyraConversation);
                    break;
                default:
                    ShowDialogueResponse(
                        "East Gate Watch",
                        "Watchwoman Ilyra",
                        "Fresh sling stones on the road mean a patrol is close. Bone strips or black cord tied above the brush mean a shaman has marked the way. Most of all, watch the birds. If the whole verge empties at once, stop walking and find cover.",
                        ObjectType.TownGuard,
                        teal,
                        ShowIlyraConversation);
                    break;
            }
        }

        private void VisitCityCourier()
        {
            bool first = !HasStoryFlag(StoryFlags.MidgaardCityCourierMet);
            SetStoryFlag(StoryFlags.MidgaardCityCourierMet);
            if (first)
            {
                state.Gold += 4;
                state.Supplies += 1;
                AwardWorldExperience(4, "Courier route copied");
                PushLog("The city courier lets the party copy a safe-lane note and slips over a wrapped ration.", Tone.Good);
            }
            else
            {
                PushLog("The city courier repeats the shortest safe loop through Midgaard's lamps.", Tone.Normal);
            }
            ShowBanner("City Courier");
            ShowTovanConversation();
        }

        private void ShowTovanConversation()
        {
            ShowDialogueChoices(
                "City Courier",
                "Tovan",
                "I run Market Square, Temple Square, the south lamps, and then the gates. If you need a route or the latest dispatch, ask now. I am already late.",
                ObjectType.CityCourier,
                gold,
                new[]
                {
                    MakeDialogueChoice("lanes", "What is your safest route?", "The shortest reliable loop through Midgaard."),
                    MakeDialogueChoice("gates", "What are the gates reporting?", "The latest word from the west and east watches."),
                    MakeDialogueChoice("signals", "How do you keep your bearings?", "Landmarks that remain useful after dark.")
                },
                ResolveTovanDialogueChoice);
        }

        private void ResolveTovanDialogueChoice(string choice)
        {
            switch (choice)
            {
                case "gates":
                    ShowDialogueResponse("City Courier", "Tovan", "West Gate reports washed-out ground near Green Shrine Road and late quarry carts. East Gate has heard kobold drums after dusk and found sling stones by the old milestones. Brann has both exits watched, but neither road is quiet.", ObjectType.CityCourier, gold, ShowTovanConversation);
                    break;
                case "signals":
                    ShowDialogueResponse("City Courier", "Tovan", "I use landmarks, not shortcuts: the temple bell, Kate's stove, Orren's hearth, and the gate towers. At night I keep two lit buildings in sight. If I cannot name the last doorway I passed, I turn around. A map is no help once you stop knowing where you are on it.", ObjectType.CityCourier, gold, ShowTovanConversation);
                    break;
                default:
                    ShowDialogueResponse("City Courier", "Tovan", "From Market Square, take the pale paving to Temple Square. Go south past Kate's Diner and Orren's tavern, then follow the lamps back toward the hall or either open gate. It is not the shortest walk on paper, but every part of it has light and people nearby.", ObjectType.CityCourier, gold, ShowTovanConversation);
                    break;
            }
        }

        private void VisitTempleHealer()
        {
            RestoreLivingParty(10, 8);
            SetStoryFlag(StoryFlags.MidgaardTempleFound);
            if (!ContentSetCatalog.ShowPrototypeScaffold(activeContentSet))
            {
                PushLog("Mira steadies the party and points them back toward the sewer contract.", Tone.Good);
                ShowBanner("Mira of Midgaard");
                AddBurst(state.PlayerX, state.PlayerY, teal);
                ShowMiraConversation("Hold still... there. You're fit to walk. If you're still going below, ask me what you need before you leave the square.");
                PlaySfx("heal", 0.72f);
                return;
            }
            if (!HasStoryFlag(StoryFlags.MidgaardLampRoundStarted))
            {
                SetStoryFlag(StoryFlags.MidgaardLampRoundStarted);
                PushLog("Mira asks for a simple lamp round: market clerk, Kate's Diner, tavern keeper, then back to Temple Square.", Tone.Good);
                ShowBanner("Mira's Lamp Round");
            }
            else if (LampRoundReady() && !HasStoryFlag(StoryFlags.MidgaardLampRoundComplete))
            {
                SetStoryFlag(StoryFlags.MidgaardLampRoundComplete);
                state.Elixirs += 1;
                state.Supplies += 2;
                AwardWorldExperience(10, "Mira's Lamp Round complete");
                PushLog("Mira seals the lamp route and sends the party out with an elixir and two provisions.", Tone.Good);
                ShowBanner("Lamp Round Complete");
            }
            else if (!HasStoryFlag(StoryFlags.MidgaardLampRoundComplete))
            {
                PushLog("Mira's lamp list is still open: " + LampRoundStatusLine(), Tone.Normal);
                ShowBanner("Mira of Midgaard");
            }
            else
            {
                PushLog("Mira keeps Temple Square calm. Her lamp route is marked in the party journal.", Tone.Normal);
                ShowBanner("Mira of Midgaard");
            }
            AddBurst(state.PlayerX, state.PlayerY, teal);
            ShowMiraConversation(HasStoryFlag(StoryFlags.MidgaardLampRoundComplete)
                ? "You found the lamps and found your way back. Good. What do you need before you travel?"
                : "Better. Now, before the pain comes back—what do you need to know?");
            PlaySfx("heal", 0.72f);
        }

        private void ShowMiraConversation(string greeting)
        {
            bool offersLampRound = ContentSetCatalog.ShowPrototypeScaffold(activeContentSet);
            ShowDialogueChoices(
                "Temple Square",
                "Mira",
                greeting,
                ObjectType.TempleHealer,
                teal,
                new[]
                {
                    MakeDialogueChoice(
                        offersLampRound ? "lamp" : "temple",
                        offersLampRound ? "How goes the lamp round?" : "What can the temple mend?",
                        offersLampRound ? "Review the city errand and remaining stops." : "What Mira restores when the party returns to Temple Square."),
                    MakeDialogueChoice("healing", "When should we heal?", "Keeping the healer safe and acting before a crisis."),
                    MakeDialogueChoice("cistern", "What waits in the cisterns?", "How ratfolk brutes shelter a poison-working mage.")
                },
                ResolveMiraDialogueChoice);
        }

        private void ResolveMiraDialogueChoice(string choice)
        {
            Action returnToMira = () => ShowMiraConversation("Anything else? Better to ask me here than wonder about it below.");
            switch (choice)
            {
                case "healing":
                    ShowDialogueResponse("Temple Square", "Mira", "Don't wait for someone to fall. Keep your healer behind cover and tend the wound most likely to put everyone else in danger. Set a Hallowed Circle where your people can hold their ground. A blessing works better as part of the plan than as an apology afterward.", ObjectType.TempleHealer, teal, returnToMira);
                    break;
                case "cistern":
                    ShowDialogueResponse("Temple Square", "Mira", "The ratfolk below work together. Their brutes pin you while a plague mage poisons your people and fills the floor with gas. Keep a cure ready, but move out of the cloud before you use it. And don't let the brute rush you somewhere worse.", ObjectType.TempleHealer, teal, returnToMira);
                    break;
                case "lamp":
                    ShowDialogueResponse("Temple Square", "Mira", HasStoryFlag(StoryFlags.MidgaardLampRoundComplete)
                        ? "Market, diner, tavern, temple—you know the round now. If you come home hurt or after dark, your feet will remember where the light, food, and help are."
                        : "You still need to " + LampRoundStatusLine() + ". Go by the market, Kate's, and Orren's, then come back. The signatures are only proof. I care that you can find help in the dark.", ObjectType.TempleHealer, teal, returnToMira);
                    break;
                default:
                    ShowDialogueResponse("Temple Square", "Mira", "Bring anyone still on their feet to Temple Square and I'll bind what I can. But don't make me your whole plan. Come back before the last of you has to carry the others.", ObjectType.TempleHealer, teal, returnToMira);
                    break;
            }
        }

        private void VisitNoviceHealer()
        {
            RestoreLivingParty(6, 5);
            SetStoryFlag(StoryFlags.MidgaardTempleFound);
            bool first = !HasStoryFlag(StoryFlags.MidgaardNoviceHealerMet);
            SetStoryFlag(StoryFlags.MidgaardNoviceHealerMet);
            if (first)
            {
                state.Supplies += 1;
                AwardWorldExperience(4, "Novice healer lesson");
                PushLog("A novice healer practices a steadying ward and packs one temple ration for the party.", Tone.Good);
            }
            else
            {
                PushLog("The novice healer repeats the warding pattern: step, breathe, shield, mend.", Tone.Normal);
            }
            AddBurst(state.PlayerX, state.PlayerY, teal);
            ShowBanner("Novice Healer");
            ShowSeraConversation();
            PlaySfx("heal", 0.56f);
        }

        private void ShowSeraConversation()
        {
            ShowDialogueChoices(
                "Novice Healer",
                "Sera",
                "Mira makes me explain each lesson aloud. She says that if I cannot say it plainly, I do not understand it yet. What should I try?",
                ObjectType.NoviceHealer,
                teal,
                new[]
                {
                    MakeDialogueChoice("circle", "How do you use Hallowed Circle?", "Claiming safe ground before wounds deepen."),
                    MakeDialogueChoice("order", "Who do you heal first?", "Sera's rule for choosing under pressure."),
                    MakeDialogueChoice("plague", "How do you handle plague magic?", "Poison, gas, and the mage behind them.")
                },
                ResolveSeraDialogueChoice);
        }

        private void ResolveSeraDialogueChoice(string choice)
        {
            switch (choice)
            {
                case "order":
                    ShowDialogueResponse("Novice Healer", "Sera", "First, keep the healer standing. Then help whoever would leave the others exposed if they fell. The worst wound is not always the first one to mend. I still have to remind myself to look at the enemy before I choose.", ObjectType.NoviceHealer, teal, ShowSeraConversation);
                    break;
                case "plague":
                    ShowDialogueResponse("Novice Healer", "Sera", "I was taught to cure the poison and forget the caster. Mira corrected me. Move the victim out of the gas first, then use the cure. While you tend them, someone else must press the plague mage or the next cloud will undo your work.", ObjectType.NoviceHealer, teal, ShowSeraConversation);
                    break;
                default:
                    ShowDialogueResponse("Novice Healer", "Sera", "Cast Hallowed Circle where you mean to hold, not wherever the first wound happens. Keep the priest behind cover and let the front line fight along the circle's edge. If you wait until everyone has scattered, the blessing cannot gather them for you.", ObjectType.NoviceHealer, teal, ShowSeraConversation);
                    break;
            }
        }

        private void VisitTavernKeeper()
        {
            RestoreLivingParty(8, 5);
            MarkLampRoundVisit("tavern", "Tavern Keeper Orren");
            if (ClaimOrrenIntroduction())
            {
                PushLog("Orren the tavern keeper marks road rumors in charcoal and gives the party travel bread.", Tone.Good);
            }
            else
            {
                PushLog("Orren keeps the common room quiet and repeats the best advice: learn the gates before chasing glory.", Tone.Normal);
            }
            ShowBanner("Tavern Keeper");
            ShowOrrenConversation();
            PlaySfx("ui", 0.62f);
        }

        private bool ClaimOrrenIntroduction()
        {
            bool alreadyMet = HasStoryFlag(StoryFlags.MidgaardTavernKeeperMet)
                || HasStoryFlag(StoryFlags.MidgaardTavernHeard);
            SetStoryFlag(StoryFlags.MidgaardTavernKeeperMet);
            SetStoryFlag(StoryFlags.MidgaardTavernHeard);
            if (alreadyMet) return false;
            state.Supplies += 1;
            return true;
        }

        private void ShowOrrenConversation(string greeting = null)
        {
            ShowDialogueChoices(
                "Tavern Keeper",
                "Orren",
                string.IsNullOrWhiteSpace(greeting)
                    ? "Heading out? I can tell by the boots. Want packing advice, cistern gossip, or the latest from the roads?"
                    : greeting,
                ObjectType.TavernKeeper,
                Hex("d98b6a"),
                new[]
                {
                    MakeDialogueChoice("supplies", "What should we pack?", "Food, elixirs, and enough reserve to come home."),
                    MakeDialogueChoice("cistern", "What did the survivors say?", "What returning parties learned about the ratfolk."),
                    MakeDialogueChoice("roads", "What lies beyond the gates?", "The latest tavern talk from the western and eastern roads.")
                },
                ResolveOrrenDialogueChoice);
        }

        private void ResolveOrrenDialogueChoice(string choice)
        {
            switch (choice)
            {
                case "cistern":
                    ShowDialogueResponse("Tavern Keeper", "Orren", "Harl the mason came back last week with half a shield and no eyebrows. A brute kept his crew busy while a plague caster spoiled every safe patch of floor. His exact words? 'Move the caster. Then mind the big one.'", ObjectType.TavernKeeper, Hex("d98b6a"), () => ShowOrrenConversation("Harl survived, if you're wondering. What else?"));
                    break;
                case "roads":
                    ShowDialogueResponse("Tavern Keeper", "Orren", "Westbound travelers come in wearing shrine moss and quarry dust. The eastern carts bring broken spokes, sling stones, and stories of kobold fires. Brann will have the fresher report. Mine usually arrives thirsty.", ObjectType.TavernKeeper, Hex("d98b6a"), () => ShowOrrenConversation("That's what I've heard. Need anything else?"));
                    break;
                default:
                    ShowDialogueResponse("Tavern Keeper", "Orren", "Food for the walk out, food for the walk home, and one ration for bad luck. Take an elixir you'll actually drink, too. If half the pack is gone before you reach the job, turn around. The road will still be there after supper.", ObjectType.TavernKeeper, Hex("d98b6a"), () => ShowOrrenConversation("That's my sermon. Shorter than most. What else?"));
                    break;
            }
        }

        private void VisitWoundedTraveler()
        {
            RestoreLivingParty(5, 2);
            bool first = !HasStoryFlag(StoryFlags.MidgaardWoundedTravelerHelped);
            SetStoryFlag(StoryFlags.MidgaardWoundedTravelerHelped);
            if (first)
            {
                state.Elixirs += 1;
                AwardWorldExperience(5, "Traveler warning heard");
                PushLog("The party rests beside the wounded traveler while a temple acolyte changes her bandage. Edda leaves one emergency elixir with them.", Tone.Good);
            }
            else
            {
                PushLog("The wounded traveler keeps repeating the same warning: the first enemy is panic.", Tone.Normal);
            }
            ShowBanner("Wounded Traveler");
            ShowEddaConversation();
        }

        private void ShowEddaConversation()
        {
            ShowDialogueChoices(
                "Roadside Warning",
                "Edda",
                "Five of us went into the cisterns. I'm the one who came back. If you're going anyway, ask me what they did—or what we did wrong.",
                ObjectType.WoundedTraveler,
                blood,
                new[]
                {
                    MakeDialogueChoice("rats", "How did the ratfolk fight?", "How the brutes protected a plague caster."),
                    MakeDialogueChoice("failure", "What went wrong?", "The choice that split Edda's group."),
                    MakeDialogueChoice("escape", "How did you get out?", "A broken pillar, an elixir, and a timely retreat.")
                },
                ResolveEddaDialogueChoice);
        }

        private void ResolveEddaDialogueChoice(string choice)
        {
            switch (choice)
            {
                case "failure":
                    ShowDialogueResponse("Roadside Warning", "Edda", "One of the small ones ran, and we chased it. That split our front. The plague caster could see our healer, and our archer stepped too close trying to get another shot. It happened quickly after that. Do not let a fleeing enemy pull you out of place.", ObjectType.WoundedTraveler, blood, ShowEddaConversation);
                    break;
                case "escape":
                    ShowDialogueResponse("Roadside Warning", "Edda", "I put a broken pillar between me and the brute, drank the elixir we had been saving, and ran while the caster turned toward the others. I am not proud of that last part. I am alive to tell you because I stopped waiting for a better moment.", ObjectType.WoundedTraveler, blood, ShowEddaConversation);
                    break;
                default:
                    ShowDialogueResponse("Roadside Warning", "Edda", "Two brutes blocked the middle. The plague mage stayed behind them, poisoning us and laying gas across the only clear ground. The smaller ones crowded every way around. We kept hitting what stood nearest until there was nowhere safe left to stand.", ObjectType.WoundedTraveler, blood, ShowEddaConversation);
                    break;
            }
        }

        private void VisitStableHand()
        {
            bool first = !HasStoryFlag(StoryFlags.MidgaardStableHandMet);
            SetStoryFlag(StoryFlags.MidgaardStableHandMet);
            if (first)
            {
                state.Supplies += 1;
                AwardWorldExperience(4, "Stable road advice");
                PushLog("The stable hand marks a dry east-road cut and adds one travel feed bundle to the pack.", Tone.Good);
            }
            else
            {
                PushLog("The stable hand watches the road and names which stones still look safe.", Tone.Normal);
            }
            ShowBanner("Stable Hand");
            ShowPellConversation();
        }

        private void ShowPellConversation()
        {
            ShowDialogueChoices(
                "East Road Stable",
                "Pell",
                "The mare has refused the east road twice this week. She is usually right before I am. Ask about the road, the kobolds, or what your packs ought to weigh.",
                ObjectType.StableHand,
                moss,
                new[]
                {
                    MakeDialogueChoice("east", "What is the east road like?", "Fast ground, little shelter, and the way toward Dusk Market."),
                    MakeDialogueChoice("signs", "How do you spot kobolds?", "Tracks, sling stones, bone charms, and skittish animals."),
                    MakeDialogueChoice("pack", "How much should we carry?", "A stable hand's rule for provisions and the return trip.")
                },
                ResolvePellDialogueChoice);
        }

        private void ResolvePellDialogueChoice(string choice)
        {
            switch (choice)
            {
                case "signs":
                    ShowDialogueResponse("East Road Stable", "Pell", "A kobold carrying sling stones leaves one track deeper than the other. Shamans hang little bone strips where the road narrows, and sometimes stretch cord low through the brush. The horses notice both before I do. If their ears go flat, I stop the cart.", ObjectType.StableHand, moss, ShowPellConversation);
                    break;
                case "pack":
                    ShowDialogueResponse("East Road Stable", "Pell", "Count one provision for the way out, one for the way home, and one for delay. Put the water where anyone can reach it. When you open the food meant for your return, that is your sign to turn around, not a reason to walk faster.", ObjectType.StableHand, moss, ShowPellConversation);
                    break;
                default:
                    ShowDialogueResponse("East Road Stable", "Pell", "The east road is level and quick, but there is little shelter between here and Dusk Market. Use the broken stalls and wagons when sling stones start flying. If you see bone charms, keep your people apart so one web cannot snare the whole group.", ObjectType.StableHand, moss, ShowPellConversation);
                    break;
            }
        }

        private void VisitGateCaptain()
        {
            SetStoryFlag(StoryFlags.MidgaardGateCaptainMet);
            if (!ContentSetCatalog.ShowPrototypeScaffold(activeContentSet))
            {
                PushLog("Brann keeps the gates quiet and points the party back toward the sewer contract.", Tone.Normal);
                ShowBanner("Gate Captain");
                ShowBrannConversation("The roads can wait until the cistern contract is settled. If you need the watch report before you go below, ask.");
                PlaySfx("guard", 0.66f);
                return;
            }
            if (!HasStoryFlag(StoryFlags.MidgaardGateSurveyStarted))
            {
                SetStoryFlag(StoryFlags.MidgaardGateSurveyStarted);
                PushLog("Gate Captain Brann wants a beginner's survey: stand at both West Gate and East Gate, then report back.", Tone.Good);
                ShowBanner("Gate Survey");
            }
            else if (GateSurveyReady() && !HasStoryFlag(StoryFlags.MidgaardGateSurveyComplete))
            {
                SetStoryFlag(StoryFlags.MidgaardGateSurveyComplete);
                state.Gold += 18;
                state.Supplies += 1;
                AwardWorldExperience(10, "Gate Survey complete");
                PushLog("Brann pays for the survey and notes the party can find its way around Midgaard's walls.", Tone.Good);
                ShowBanner("Gate Survey Complete");
            }
            else if (!HasStoryFlag(StoryFlags.MidgaardGateSurveyComplete))
            {
                PushLog("Brann taps the city map: " + GateSurveyStatusLine(), Tone.Normal);
                ShowBanner("Gate Captain");
            }
            else
            {
                PushLog("Brann has the gates covered for now. He asks the party to report fresh tracks, broken lamps, or organized raiders.", Tone.Normal);
                ShowBanner("Gate Captain");
            }
            ShowBrannConversation(HasStoryFlag(StoryFlags.MidgaardGateSurveyComplete)
                    ? "Your survey is filed and paid. What else do you need from the watch?"
                    : "Ask now. Once you pass a gate, the report will already be out of date.");
            PlaySfx("guard", 0.66f);
        }

        private void ShowBrannConversation(string greeting = null)
        {
            bool offersGateSurvey = ContentSetCatalog.ShowPrototypeScaffold(activeContentSet);
            ShowDialogueChoices(
                "Gate Captain",
                "Brann",
                string.IsNullOrWhiteSpace(greeting)
                    ? "West and east are open. North and south are sealed. What do you need clarified?"
                    : greeting,
                ObjectType.GateCaptain,
                stone,
                new[]
                {
                    MakeDialogueChoice(
                        offersGateSurvey ? "survey" : "watch",
                        offersGateSurvey ? "What remains on the survey?" : "What are the current orders?",
                        offersGateSurvey ? "Review the two open exits and your progress." : "The watch's priority while the cistern writ is active."),
                    MakeDialogueChoice("roads", "How do the roads differ?", "Compare the western shrine road with the kobold country east."),
                    MakeDialogueChoice("sealed", "Why are two gates sealed?", "The condition of the north and south gatehouses.")
                },
                ResolveBrannDialogueChoice);
        }

        private void ResolveBrannDialogueChoice(string choice)
        {
            switch (choice)
            {
                case "roads":
                    ShowDialogueResponse("Gate Captain", "Brann", "West Gate leads to Green Shrine Road and the old quarry. The ground narrows there, with roots and broken stone for cover. East Gate leads toward Dusk Market. Kobolds use slings, webs, and bone signs on the open road. Pick the route your group is equipped to handle.", ObjectType.GateCaptain, stone, () => ShowBrannConversation());
                    break;
                case "sealed":
                    ShowDialogueResponse("Gate Captain", "Brann", "The north gate stays barred until its defenses are repaired. The south gate stands over the cistern works; opening it would give anything below a direct way into the city. Until both are secure, use east or west.", ObjectType.GateCaptain, stone, () => ShowBrannConversation());
                    break;
                case "survey":
                    ShowDialogueResponse("Gate Captain", "Brann", HasStoryFlag(StoryFlags.MidgaardGateSurveyComplete)
                        ? "The survey is complete and your payment is recorded. You checked both open approaches and returned with a useful report."
                        : "Your survey still requires you to " + GateSurveyStatusLine() + ". Stand at each open gate, look beyond the wall, and report back to me.", ObjectType.GateCaptain, stone, () => ShowBrannConversation());
                    break;
                default:
                    ShowDialogueResponse("Gate Captain", "Brann", MidgaardRatPeltsReady()
                        ? "The cistern chambers are clear. Take the proof to Borin before you leave the city; earned armor is worth more than another road rumor. East and west remain open."
                        : "King Halvard's cistern writ is the current priority. The threat is inside the walls, below the south quarter. Clear the contracted chambers before you commit to either outer road.", ObjectType.GateCaptain, stone, () => ShowBrannConversation());
                    break;
            }
        }

        private void MarkLampRoundVisit(string stop, string label)
        {
            if (!ContentSetCatalog.ShowPrototypeScaffold(activeContentSet)) return;
            if (!HasStoryFlag(StoryFlags.MidgaardLampRoundStarted) || HasStoryFlag(StoryFlags.MidgaardLampRoundComplete)) return;
            string flag = "midgaard_lamp_round_" + stop;
            if (HasStoryFlag(flag)) return;
            SetStoryFlag(flag);
            PushLog($"{label} signs Mira's lamp route. {LampRoundStatusLine()}", Tone.Good);
            if (LampRoundReady()) PushLog("Mira's lamp round is ready to turn in at Temple Square.", Tone.Good);
        }

        private bool LampRoundReady()
        {
            return HasStoryFlag(StoryFlags.MidgaardLampRoundMarket)
                && HasStoryFlag(StoryFlags.MidgaardLampRoundDiner)
                && HasStoryFlag(StoryFlags.MidgaardLampRoundTavern);
        }

        private string LampRoundStatusLine()
        {
            List<string> remaining = new List<string>();
            if (!HasStoryFlag(StoryFlags.MidgaardLampRoundMarket)) remaining.Add("market clerk");
            if (!HasStoryFlag(StoryFlags.MidgaardLampRoundDiner)) remaining.Add("Kate's Diner");
            if (!HasStoryFlag(StoryFlags.MidgaardLampRoundTavern)) remaining.Add("tavern keeper");
            return remaining.Count == 0 ? "return to Mira" : "visit " + string.Join(", ", remaining);
        }

        private void MarkGateSurveyVisit(string gate)
        {
            if (!ContentSetCatalog.ShowPrototypeScaffold(activeContentSet)) return;
            if (!HasStoryFlag(StoryFlags.MidgaardGateSurveyStarted) || HasStoryFlag(StoryFlags.MidgaardGateSurveyComplete)) return;
            string flag = "midgaard_gate_survey_" + gate;
            if (HasStoryFlag(flag)) return;
            SetStoryFlag(flag);
            PushLog($"{(gate == "east" ? "East" : "West")} Gate logged for Brann's survey. {GateSurveyStatusLine()}", Tone.Good);
            if (GateSurveyReady()) PushLog("Both gates are checked. Report back to Gate Captain Brann.", Tone.Good);
        }

        private bool GateSurveyReady()
        {
            return HasStoryFlag(StoryFlags.MidgaardGateSurveyWest) && HasStoryFlag(StoryFlags.MidgaardGateSurveyEast);
        }

        private string GateSurveyStatusLine()
        {
            List<string> remaining = new List<string>();
            if (!HasStoryFlag(StoryFlags.MidgaardGateSurveyWest)) remaining.Add("West Gate");
            if (!HasStoryFlag(StoryFlags.MidgaardGateSurveyEast)) remaining.Add("East Gate");
            return remaining.Count == 0 ? "return to Gate Captain Brann" : "check " + string.Join(" and ", remaining);
        }

        private void VisitKatesDiner(bool provisionStall)
        {
            VisitKatesDiner(
                provisionStall,
                provisionStall ? ObjectType.Provisions : ObjectType.Diner);
        }

        private void VisitKatesDiner(bool provisionStall, ObjectType interactionFocus)
        {
            RestoreLivingParty(10, 6);
            MarkLampRoundVisit("diner", provisionStall ? "Lute's Provision Stall" : "Kate's Diner");
            PushLog(provisionStall
                ? "Lute checks the party's packs and names the price before touching a single coin."
                : "Kate clears a place near the stove and lets the party choose food before advice.", Tone.Normal);
            ShowBanner(provisionStall ? "Provision Stall" : "Kate's Diner");
            ShowKateConversation(provisionStall, null, interactionFocus);
            PlaySfx("heal", 0.66f);
        }

        private void VisitDockWorker()
        {
            const string warning =
                "River landing's shut until they trust the south gate and cistern works again, "
                + "so I'm hauling rope instead of cargo. Going below? Keep off any stone that "
                + "shines wet, and test a handrail before you give it your weight. Half of them are only pretending.";
            PushLog("A dock worker coils a tarred line beside the south-quarter works and points out the slick approach.", Tone.Normal);
            ShowBanner("South-Quarter Worker");
            ShowDialogue("South-Quarter Works", "Dock Worker", warning, ObjectType.DockWorker, stone);
            PlaySfx("ui", 0.52f);
        }

        private void VisitMidgaardScholar()
        {
            const string note =
                "Look here. The old records call the cisterns a second road beneath Midgaard. "
                + "Masons marked safe junctions with two neat cuts. The ratfolk marks are rougher, "
                + "and just as likely to lead you into an ambush as toward an exit. Formula scribes "
                + "borrowed the paired cuts for warding diagrams. Same measured rhythm: give lightning "
                + "a safe channel, or it finds one through the caster.";
            PushLog("A city scholar compares a water-stained cistern plan with the paving around the keep.", Tone.Normal);
            ShowBanner("Midgaard Scholar");
            ShowDialogue("Keep Records", "Midgaard Scholar", note, ObjectType.Scholar, frost);
            PlaySfx("ui", 0.52f);
        }

        private static ObjectType KateConversationFocus(bool provisionStall, ObjectType interactionFocus)
        {
            if (interactionFocus == ObjectType.DinerCook || interactionFocus == ObjectType.Provisioner)
            {
                return interactionFocus;
            }

            return provisionStall ? ObjectType.Provisions : ObjectType.Diner;
        }

        private void ShowKateConversation(
            bool provisionStall,
            string greeting = null,
            ObjectType interactionFocus = ObjectType.Town)
        {
            string speaker = provisionStall ? "Lute" : "Kate";
            ObjectType focus = KateConversationFocus(provisionStall, interactionFocus);
            int cost = provisionStall ? 10 : 12;
            int bundle = provisionStall ? 3 : 4;
            bool bought = HasKateStarterBundle();
            bool canBuy = !bought && state.Gold >= cost;
            string serviceLabel = bought ? "Road bundle packed" : $"Buy {bundle} provisions - {cost} gold";
            string serviceHint = bought
                ? (provisionStall ? "Lute has already packed this order." : "Kate has already packed this order.")
                : canBuy
                    ? "Confirm the purchase and add the food to the party pack."
                    : $"Need {Mathf.Max(0, cost - state.Gold)} more gold.";
            ShowDialogueChoices(
                provisionStall ? "Provision Stall" : "Kate's Diner",
                speaker,
                string.IsNullOrWhiteSpace(greeting)
                    ? provisionStall
                        ? $"Three sealed portions, ten gold. Dry, weighed, and counted twice. You've got {state.Gold}; want them?"
                        : $"Sit down, love. The stove's warm. If you're stocking the road, four portions are twelve gold. You've got {state.Gold}."
                    : greeting,
                focus,
                Hex("d98b6a"),
                new[]
                {
                    MakeDialogueChoice("buy", serviceLabel, serviceHint, canBuy),
                    MakeDialogueChoice("packing", "How much food should we carry?", "A practical rule for provisions, water, and the return trip."),
                    MakeDialogueChoice("safe", "Where can we rest safely?", "Reliable places for light, water, and help in Midgaard."),
                    MakeDialogueChoice("sewer", "What food keeps in the cisterns?", "Packing a meal that survives damp and a long fight.")
                },
                choice => ResolveKateDialogueChoice(choice, provisionStall, focus));
        }

        private void ResolveKateDialogueChoice(string choice, bool provisionStall, ObjectType interactionFocus)
        {
            string speaker = provisionStall ? "Lute" : "Kate";
            ObjectType focus = KateConversationFocus(provisionStall, interactionFocus);
            Action returnToKate = () => ShowKateConversation(
                provisionStall,
                provisionStall ? "Anything else? I can talk while I pack." : "While you're here, love—anything else?",
                focus);
            switch (choice)
            {
                case "buy":
                    ShowKatePurchaseReview(provisionStall, focus);
                    break;
                case "safe":
                    ShowDialogueResponse(
                        provisionStall ? "Provision Stall" : "Kate's Diner",
                        speaker,
                        provisionStall
                            ? "Temple Square has healers and clean water. Kate keeps her stove lit late, and Orren hardly ever closes the common room. Learn those three doors now. Tired feet are poor mapmakers."
                            : "Come back hurt, go straight to Mira in Temple Square. If you only need food, warmth, and someone to notice you made it home, try me or Orren. Learn the walk now, while your legs are sound.",
                        focus,
                        Hex("d98b6a"),
                        returnToKate);
                    break;
                case "sewer":
                    ShowDialogueResponse(
                        provisionStall ? "Provision Stall" : "Kate's Diner",
                        speaker,
                        provisionStall
                            ? "Hard bread, smoked meat, waxed wrapping. Keep it above the damp and open only what you'll finish. Eat before the Cistern Den. Food carried home unopened helped no one."
                            : "Bread, smoked meat, apples—wrap them well and they'll keep. Eat before the last chamber, not after it. I've seen too many people save supper until they were too sick to touch it.",
                        focus,
                        Hex("d98b6a"),
                        returnToKate);
                    break;
                default:
                    ShowDialogueResponse(
                        provisionStall ? "Provision Stall" : "Kate's Diner",
                        speaker,
                        provisionStall
                            ? "One portion out, one back, one for delay. Water goes on top, not under the armor. When you reach the bundle marked for home, go home. That's why I marked it."
                            : "A meal going out, another coming home, and one more for bad luck. Keep the water where everyone can reach it. When only the homeward meal remains, come home.",
                        focus,
                        Hex("d98b6a"),
                        returnToKate);
                    break;
            }
        }

        private bool HasKateStarterBundle()
        {
            return HasStoryFlag(StoryFlags.MidgaardKateBundleBought)
                || HasStoryFlag(StoryFlags.MidgaardProvisionBundleBought);
        }

        private void PurchaseKateStarterBundle(bool provisionStall, ObjectType interactionFocus)
        {
            int cost = provisionStall ? 10 : 12;
            int bundle = provisionStall ? 3 : 4;
            if (HasKateStarterBundle() || state.Gold < cost)
            {
                ShowKateConversation(provisionStall, null, interactionFocus);
                return;
            }

            state.Gold -= cost;
            state.Supplies += bundle;
            SetStoryFlag(StoryFlags.MidgaardKateBundleBought);
            SetStoryFlag(StoryFlags.MidgaardProvisionBundleBought);
            string speaker = provisionStall ? "Lute" : "Kate";
            string greeting = provisionStall
                ? $"There. Three dry portions, and the weight's on the cord. You've got {state.Gold} gold left."
                : $"There you are—four portions. Eat before hunger starts making decisions for you. You've got {state.Gold} gold left.";
            PushLog($"{speaker} packs {bundle} provisions for {cost} gold.", Tone.Good);
            ShowBanner("Provisions Packed");
            PlaySfx("servicecoin", 0.48f);
            QueueSfx("cache", 0.08f, 0.54f);
            ShowKateConversation(provisionStall, greeting, interactionFocus);
        }

        private void VisitMidgaardTavern()
        {
            RestoreLivingParty(8, 5);
            MarkLampRoundVisit("tavern", "Midgaard Tavern");
            if (ClaimOrrenIntroduction())
            {
                PushLog("The tavern keeper marks the sewer grate on your map and slides over travel bread.", Tone.Good);
            }
            else
            {
                PushLog("The tavern settles the party's nerves. Rumor points back to the king's first sewer work.", Tone.Normal);
            }
            ShowBanner("Midgaard Tavern");
            ShowOrrenConversation();
            PlaySfx("ui", 0.62f);
        }

        private void VisitMidgaardArmorer()
        {
            PushLog("Borin sets aside his rivet hammer and waits for the party to choose a service.", Tone.Normal);
            ShowBanner("Midgaard Armorer");
            ShowBorinConversation();
            PlaySfx("ui", 0.56f);
        }

        private void ShowBorinConversation(string greeting = null)
        {
            bool bought = HasStoryFlag(StoryFlags.MidgaardBasicArmorBought);
            bool canBuy = !bought && state.Gold >= 28;
            string serviceLabel = bought ? "Mail already fitted" : "Fit a hauberk - 28 gold";
            string serviceHint = bought
                ? "Borin has already fitted this serviceable hauberk."
                : canBuy
                    ? "Buy one serviceable hauberk and fit it to the best wearer."
                    : $"Need {Mathf.Max(0, 28 - state.Gold)} more gold.";
            ShowDialogueChoices(
                "Midgaard Armorer",
                "Borin",
                string.IsNullOrWhiteSpace(greeting)
                    ? bought
                        ? "The rings are settling well. Bring it back if a strap bites or a rivet lifts."
                        : $"Plain iron, good rings, fitted to the wearer—twenty-eight gold. You've got {state.Gold}."
                    : greeting,
                ObjectType.Armorer,
                stone,
                new[]
                {
                    MakeDialogueChoice("buy", serviceLabel, serviceHint, canBuy),
                    MakeDialogueChoice("pelts", "What can you make from rat hide?", "The armor Borin can craft from clean sewer proof."),
                    MakeDialogueChoice("guard", "How should we use heavy armor?", "Why armor works best with Guard and a stable front.")
                },
                ResolveBorinDialogueChoice);
        }

        private void ResolveBorinDialogueChoice(string choice)
        {
            Action returnToBorin = () => ShowBorinConversation("Anything else? I've a few minutes before the forge is hot again.");
            switch (choice)
            {
                case "buy":
                    ShowBorinPurchaseReview();
                    break;
                case "pelts":
                    ShowDialogueResponse("Midgaard Armorer", "Borin", HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade)
                        ? "The sewer-hide coat is holding. It gives where city mail would catch. Keep the seams dry when you can, and bring it back if the shoulder starts to pull."
                        : MidgaardRatPeltsReady()
                            ? "That's enough clean hide for the sewer pattern. Take it to the rat-pelt bench and I'll cut you a coat lighter than city mail."
                            : "Bring me clean hide from each contracted cistern chamber. Miserable stuff to work, but tough, flexible, and made for damp tunnels.", ObjectType.Armorer, stone, returnToBorin);
                    break;
                default:
                    ShowDialogueResponse("Midgaard Armorer", "Borin", "Mail buys you a heartbeat, not a miracle. Set your feet before the blow, keep the shield toward it, and make the enemy come through your reach. Good rings won't mend bad footing—or an exposed back.", ObjectType.Armorer, stone, returnToBorin);
                    break;
            }
        }

        private void PurchaseBorinHauberk()
        {
            if (HasStoryFlag(StoryFlags.MidgaardBasicArmorBought) || state.Gold < 28)
            {
                ShowBorinConversation();
                return;
            }
            state.Gold -= 28;
            EnsureInventoryList();
            InventoryItem item = MakeTownArmor();
            state.Inventory.Add(item);
            string equipNote = AutoEquipItem(item);
            SetStoryFlag(StoryFlags.MidgaardBasicArmorBought);
            ShowDialogueThenLoot(
                "Midgaard Armorer",
                "Borin",
                "Walk the square in it before you go below. Lift your arms, kneel, and check the straps. You should find a bad fit here, not when something is trying to kill you.",
                ObjectType.Armorer,
                stone,
                item,
                0,
                0,
                0,
                equipNote,
                "Borin's Armor Fitting");
            PushLog($"The armorer fits {item.DisplayName}. {equipNote}", Tone.Good);
            ShowBanner("Armor Fitted");
            PlaySfx("servicecoin", 0.50f);
            QueueSfx("servicearmor", 0.08f, 0.70f);
        }

        private void VisitWeaponVendor()
        {
            PushLog("Tessa turns the weapon rack so every grip is visible before naming a price.", Tone.Normal);
            ShowBanner("Weapon Vendor");
            ShowTessaConversation();
            PlaySfx("ui", 0.56f);
        }

        private void ShowTessaConversation(string greeting = null)
        {
            bool bought = HasStoryFlag(StoryFlags.MidgaardBasicWeaponBought);
            bool canBuy = !bought && state.Gold >= 32;
            string serviceLabel = bought ? "Weapon already purchased" : "Buy a town-forged weapon - 32 gold";
            string serviceHint = bought
                ? "Tessa has already fitted one town-forged weapon to the party."
                : canBuy
                    ? "Buy a weapon suited to the lead adventurer and equip it."
                    : $"Need {Mathf.Max(0, 32 - state.Gold)} more gold.";
            ShowDialogueChoices(
                "Weapon Vendor",
                "Tessa",
                string.IsNullOrWhiteSpace(greeting)
                    ? bought
                        ? "Still sound. Use it long enough and it'll tell you what the next weapon should do better."
                        : $"I can match a sound weapon to your lead fighter for thirty-two gold. Tell me how they move; I'll choose the balance. You've got {state.Gold}."
                    : greeting,
                ObjectType.WeaponVendor,
                gold,
                new[]
                {
                    MakeDialogueChoice("buy", serviceLabel, serviceHint, canBuy),
                    MakeDialogueChoice("forms", "Which weapon suits which fighter?", "The tradeoffs between blades, axes, spears, bows, and staves."),
                    MakeDialogueChoice("range", "When should an archer draw steel?", "Using distance and reach without getting trapped by them.")
                },
                ResolveTessaDialogueChoice);
        }

        private void ResolveTessaDialogueChoice(string choice)
        {
            switch (choice)
            {
                case "buy":
                    ShowTessaPurchaseReview();
                    break;
                case "forms":
                    ShowDialogueResponse("Weapon Vendor", "Tessa", "Quick hands like daggers and epees. Strong shoulders can make room for an axe or heavy blade. Spears buy you a step; bows need a lane; a staff should earn its keep as a focus. Look at the wielder first. The rack comes second.", ObjectType.WeaponVendor, gold, () => ShowTessaConversation("That's the shape of it. Need another answer?"));
                    break;
                default:
                    ShowDialogueResponse("Weapon Vendor", "Tessa", "Shoot while the lane is clear. The moment an enemy can touch the bow, draw steel; string won't stop a blade. Spears are the same lesson backward: enjoy the reach, then shorten your grip or give ground when the gap closes.", ObjectType.WeaponVendor, gold, () => ShowTessaConversation("Need another answer, or a weapon?"));
                    break;
            }
        }

        private void PurchaseTessaWeapon()
        {
            if (HasStoryFlag(StoryFlags.MidgaardBasicWeaponBought) || state.Gold < 32)
            {
                ShowTessaConversation();
                return;
            }
            state.Gold -= 32;
            PartyMember quotedLead = state.Party != null && state.Party.Count > 0 ? state.Party[0] : null;
            string role = quotedLead?.Role ?? "shield";
            EnsureInventoryList();
            InventoryItem item = TakeTessaWeaponQuote(role);
            state.Inventory.Add(item);
            string equipNote;
            if (quotedLead != null && EquipInventoryItemToMember(item, quotedLead, out string quotedEquipNote))
            {
                equipNote = quotedEquipNote;
            }
            else
            {
                equipNote = AutoEquipItem(item);
            }
            SetStoryFlag(StoryFlags.MidgaardBasicWeaponBought);
            ShowDialogueThenLoot(
                "Weapon Vendor",
                "Tessa",
                "Try the grip. Good. It will not fight the cisterns for you, but it will answer cleanly when you ask it to.",
                ObjectType.WeaponVendor,
                gold,
                item,
                0,
                0,
                0,
                equipNote,
                "Tessa's Weapon Rack");
            PushLog($"The weapon vendor sells {item.DisplayName}. {equipNote}", Tone.Good);
            ShowBanner("Weapon Bought");
            PlaySfx("servicecoin", 0.50f);
            QueueSfx("serviceweapon", 0.08f, 0.70f);
        }

        private void VisitWeaponEnchanter()
        {
            PartyMember target = state.Party?.FirstOrDefault(p => p != null && !string.IsNullOrWhiteSpace(p.WeaponName));
            maudEnchantmentTargetIndex = -1;
            PushLog(target == null
                ? "Maud lets the rune anvil fall silent; the party has no weapon she can mark."
                : "Maud lays out four rune-stones and asks which weapon should take the mark.", target == null ? Tone.Warn : Tone.Normal);
            ShowBanner("Rune Anvil");
            ShowMaudConversation();
            PlaySfx("ui", 0.56f);
        }

        private void ShowMaudConversation()
        {
            bool hasWeapon = state.Party != null
                && state.Party.Any(member => member != null && !string.IsNullOrWhiteSpace(member.WeaponName));
            bool canTemper = hasWeapon && state.Gold >= WeaponEnchantmentRules.TemporaryCost;
            bool canBind = hasWeapon && state.Gold >= WeaponEnchantmentRules.PermanentCost;
            ShowDialogueChoices(
                "Weapon Enchanter",
                "Maud",
                hasWeapon
                    ? $"Two sorts of work: a quick temper for the next {WeaponEnchantmentRules.TemporaryVictories} victories, or a true binding that stays with the weapon. You have {state.Gold} gold."
                    : "Bring me a weapon with a name and an edge, or a focus with a memory. Then we'll see what the runes are willing to hold.",
                ObjectType.Enchanter,
                violet,
                new[]
                {
                    MakeDialogueChoice(
                        "temporary",
                        $"Temporary temper - {WeaponEnchantmentRules.TemporaryCost} gold",
                        canTemper
                            ? $"Choose one party weapon; the mark lasts {WeaponEnchantmentRules.TemporaryVictories} victories."
                            : !hasWeapon
                                ? "The party has no weapon Maud can mark."
                                : $"Need {Mathf.Max(0, WeaponEnchantmentRules.TemporaryCost - state.Gold)} more gold.",
                        canTemper),
                    MakeDialogueChoice(
                        "permanent",
                        $"Permanent binding - {WeaponEnchantmentRules.PermanentCost} gold",
                        canBind
                            ? "Choose one party weapon; the binding remains until Maud replaces it."
                            : !hasWeapon
                                ? "The party has no weapon Maud can bind."
                                : $"Need {Mathf.Max(0, WeaponEnchantmentRules.PermanentCost - state.Gold)} more gold.",
                        canBind),
                    MakeDialogueChoice("affinity", "What can the four runes do?", "Fire, ice, storm, and radiant affinities.")
                },
                ResolveMaudDialogueChoice);
        }

        private void ResolveMaudDialogueChoice(string choice)
        {
            switch (choice)
            {
                case "temporary":
                    ShowMaudWeaponChoices(false);
                    break;
                case "permanent":
                    ShowMaudWeaponChoices(true);
                    break;
                default:
                    ShowDialogueResponse(
                        "Weapon Enchanter",
                        "Maud",
                        "Fire catches webbing and gas. Ice punishes anything that fears the cold. Storm worries at wards and may stun on a clean strike. Radiance is the sure choice against things from graves and rifts. The mark changes what kind of harm the weapon deals, so choose for the road ahead.",
                        ObjectType.Enchanter,
                        violet,
                        ShowMaudConversation);
                    break;
            }
        }

        private void ShowMaudWeaponChoices(bool permanent)
        {
            maudPermanentEnchantment = permanent;
            maudEnchantmentTargetIndex = -1;
            int price = permanent ? WeaponEnchantmentRules.PermanentCost : WeaponEnchantmentRules.TemporaryCost;
            List<DialogueChoiceView> choices = new List<DialogueChoiceView>();
            for (int i = 0; i < Mathf.Min(4, state.Party?.Count ?? 0); i++)
            {
                PartyMember member = state.Party[i];
                bool hasWeapon = member != null && !string.IsNullOrWhiteSpace(member.WeaponName);
                choices.Add(MakeDialogueChoice(
                    i.ToString(),
                    member == null ? "Empty place" : $"{member.Name}: {TrimGearName(member.WeaponName)}",
                    !hasWeapon
                        ? "There is no weapon in this place."
                        : state.Gold < price
                            ? $"Need {Mathf.Max(0, price - state.Gold)} more gold."
                            : permanent
                                ? "Bind a lasting affinity to this exact weapon."
                                : $"Temper this exact weapon for {WeaponEnchantmentRules.TemporaryVictories} victories.",
                    hasWeapon && state.Gold >= price));
            }

            ShowDialogueChoices(
                "Weapon Enchanter",
                "Maud",
                permanent
                    ? $"Which weapon am I binding? The work is permanent and costs {price} gold."
                    : $"Which weapon gets the quick temper? {price} gold, and it holds for {WeaponEnchantmentRules.TemporaryVictories} victories.",
                ObjectType.Enchanter,
                violet,
                choices.ToArray(),
                ResolveMaudWeaponChoice);
        }

        private void ResolveMaudWeaponChoice(string choice)
        {
            if (!int.TryParse(choice, out int partyIndex)
                || state.Party == null
                || partyIndex < 0
                || partyIndex >= state.Party.Count
                || state.Party[partyIndex] == null
                || string.IsNullOrWhiteSpace(state.Party[partyIndex].WeaponName))
            {
                ShowMaudConversation();
                return;
            }

            maudEnchantmentTargetIndex = partyIndex;
            ShowMaudAffinityChoices();
        }

        private void ShowMaudAffinityChoices()
        {
            if (state.Party == null
                || maudEnchantmentTargetIndex < 0
                || maudEnchantmentTargetIndex >= state.Party.Count)
            {
                ShowMaudConversation();
                return;
            }

            PartyMember target = state.Party[maudEnchantmentTargetIndex];
            int price = maudPermanentEnchantment ? WeaponEnchantmentRules.PermanentCost : WeaponEnchantmentRules.TemporaryCost;
            DialogueChoiceView[] choices = WeaponEnchantmentRules.All
                .Take(4)
                .Select(definition => MakeDialogueChoice(
                    definition.Id,
                    $"{definition.MenuLabel} - {price} gold",
                    definition.EffectHint,
                    state.Gold >= price))
                .ToArray();
            ShowDialogueChoices(
                "Weapon Enchanter",
                "Maud",
                $"For {target.Name}'s {TrimGearName(target.WeaponName)}—which mark? The {(maudPermanentEnchantment ? "binding is permanent" : $"temper holds for {WeaponEnchantmentRules.TemporaryVictories} victories")}.",
                ObjectType.Enchanter,
                violet,
                choices,
                ResolveMaudAffinityChoice);
        }

        private void ResolveMaudAffinityChoice(string affinityId)
        {
            PurchaseMaudEnchantment(maudEnchantmentTargetIndex, affinityId, maudPermanentEnchantment);
        }

        private void PurchaseMaudEnchantment(int partyIndex, string affinityId, bool permanent)
        {
            int price = permanent ? WeaponEnchantmentRules.PermanentCost : WeaponEnchantmentRules.TemporaryCost;
            WeaponEnchantmentDefinition definition = WeaponEnchantmentRules.Find(affinityId);
            if (definition == null
                || state.Party == null
                || partyIndex < 0
                || partyIndex >= state.Party.Count
                || state.Party[partyIndex] == null
                || string.IsNullOrWhiteSpace(state.Party[partyIndex].WeaponName)
                || state.Gold < price)
            {
                ShowMaudConversation();
                return;
            }

            PartyMember target = state.Party[partyIndex];
            InventoryItem item = EnsureEnchantmentWeaponItem(target);
            if (item == null)
            {
                ShowMaudConversation();
                return;
            }

            string old = item.DisplayName;
            if (permanent) WeaponEnchantmentRules.ApplyPermanent(item, affinityId);
            else WeaponEnchantmentRules.ApplyTemporary(item, affinityId);
            state.Gold -= price;
            SyncEnchantedWeaponToOwner(item);
            SetStoryFlag(StoryFlags.MidgaardWeaponEnchanted);
            string duration = permanent
                ? "The binding is permanent and will follow the weapon between wielders."
                : $"The temper will hold through {WeaponEnchantmentRules.TemporaryVictories} victories.";
            PushLog($"Maud changes {target.Name}'s {old} into {item.DisplayName}. {duration}", Tone.Good);
            ShowBanner(permanent ? "Weapon Bound" : "Weapon Tempered");
            ShowDialogue(
                "Weapon Enchanter",
                "Maud",
                $"Done. {definition.ResultLine} {target.Name}'s weapon is now {item.DisplayName}. {duration}",
                ObjectType.Enchanter,
                violet);
            PlaySfx("servicecoin", 0.48f);
            QueueSfx("serviceenchant", 0.10f, 0.74f);
        }

        private InventoryItem EnsureEnchantmentWeaponItem(PartyMember target)
        {
            if (target == null || string.IsNullOrWhiteSpace(target.WeaponName)) return null;
            EnsureInventoryList();
            EnsurePartyInventoryIds();
            EnsureInventoryEquipmentLinks();

            InventoryItem item = EquippedInventoryItem(target, true);
            if (item != null) return item;

            item = CreateEnchantmentWeaponItem(
                target,
                target.WeaponName,
                target.WeaponBonus,
                target.WeaponDamageMin,
                target.WeaponDamageMax,
                target.WeaponDamageType);
            state.Inventory.Add(item);
            return item;
        }

        private InventoryItem CreateEnchantmentWeaponItem(
            PartyMember target,
            string displayName,
            int bonus,
            int damageMin,
            int damageMax,
            string damageType)
        {
            string name = string.IsNullOrWhiteSpace(displayName) ? "plain weapon" : displayName.Trim();
            return new InventoryItem
            {
                Mark = "party-carried",
                EquippedById = target?.Id ?? "",
                Material = EnchantmentWeaponMaterial(name),
                Form = EnchantmentWeaponForm(name),
                Trait = EnchantmentWeaponTrait(name),
                Slot = "weapon",
                Bonus = bonus,
                StrengthBonus = target?.WeaponStrengthBonus ?? 0,
                IntelligenceBonus = target?.WeaponIntelligenceBonus ?? 0,
                AgilityBonus = target?.WeaponAgilityBonus ?? 0,
                HealthBonus = target?.WeaponHealthBonus ?? 0,
                DamageMin = Mathf.Max(1, damageMin),
                DamageMax = Mathf.Max(Mathf.Max(1, damageMin) + 1, damageMax),
                AttackSpeed = Mathf.Max(1, target?.WeaponAttackSpeed ?? 1),
                Rarity = bonus > 2 ? "rare" : bonus > 0 ? "common" : "starter",
                DamageType = string.IsNullOrWhiteSpace(damageType) ? "physical" : damageType,
                DisplayName = name
            };
        }

        private static string EnchantmentWeaponForm(string weaponName)
        {
            string text = (weaponName ?? "").ToLowerInvariant();
            string[] forms =
            {
                "mace and ward shield",
                "throwing knives",
                "throwing darts",
                "prayer focus",
                "ember focus",
                "bone focus",
                "stormglass orb",
                "blackglass orb",
                "ritual knife",
                "arming sword",
                "iron broadsword",
                "war hammer",
                "war flail",
                "long spear",
                "ash staff",
                "longbow",
                "crossbow",
                "broadsword",
                "halberd",
                "glaive",
                "scepter",
                "sabre",
                "epee",
                "focus",
                "staff",
                "mace",
                "sword",
                "orb",
                "weapon"
            };
            string match = forms.FirstOrDefault(text.Contains);
            if (match == "iron broadsword") return "broadsword";
            return string.IsNullOrEmpty(match) ? "weapon" : match;
        }

        private static string EnchantmentWeaponMaterial(string weaponName)
        {
            string text = (weaponName ?? "").ToLowerInvariant();
            string[] materials =
            {
                "fine steel",
                "stormglass",
                "blackglass",
                "adamantine",
                "mithril",
                "moonstone",
                "crystalline",
                "silvered",
                "ironwood",
                "ashwood",
                "obsidian",
                "steel",
                "iron",
                "bone",
                "wood"
            };
            return materials.FirstOrDefault(text.Contains) ?? "";
        }

        private static string EnchantmentWeaponTrait(string weaponName)
        {
            string text = (weaponName ?? "").Trim();
            int traitAt = text.LastIndexOf(" of ", StringComparison.OrdinalIgnoreCase);
            return traitAt >= 0 && traitAt + 4 < text.Length ? text.Substring(traitAt + 4).Trim() : "";
        }

        private void SyncEnchantedWeaponToOwner(InventoryItem item)
        {
            if (item == null
                || state?.Party == null
                || string.IsNullOrWhiteSpace(item.EquippedById)) return;
            PartyMember owner = state.Party.FirstOrDefault(member =>
                member != null && string.Equals(member.Id, item.EquippedById, StringComparison.Ordinal));
            if (owner == null) return;

            owner.WeaponName = item.DisplayName;
            owner.WeaponBonus = item.Bonus;
            owner.WeaponDamageType = string.IsNullOrWhiteSpace(item.DamageType) ? "physical" : item.DamageType;
            owner.WeaponDamageMin = Mathf.Max(1, item.DamageMin);
            owner.WeaponDamageMax = Mathf.Max(owner.WeaponDamageMin + 1, item.DamageMax);
            owner.WeaponAttackSpeed = Mathf.Max(1, item.AttackSpeed);
            ApplyGearStatBonuses(owner, item, true);
            owner.Range = WeaponRange(item, owner);
            RecalculateMember(owner);
        }

        private void NormalizeWeaponEnchantments(int sourceSaveVersion = SaveVersion)
        {
            if (state == null) return;
            EnsureInventoryList();
            EnsurePartyInventoryIds();
            if (sourceSaveVersion < 24) MigrateLegacyMaudEnchantment();

            foreach (InventoryItem item in state.Inventory.Where(item =>
                item != null && InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form)))
            {
                WeaponEnchantmentRules.Rebuild(item);
                SyncEnchantedWeaponToOwner(item);
            }
        }

        private void MigrateLegacyMaudEnchantment()
        {
            if (!HasStoryFlag(StoryFlags.MidgaardWeaponEnchanted) || state?.Party == null) return;
            PartyMember target = state.Party.FirstOrDefault(member =>
                member != null
                && !string.IsNullOrWhiteSpace(member.WeaponName)
                && member.WeaponName.StartsWith("enchanted ", StringComparison.OrdinalIgnoreCase));
            if (target == null) return;

            string baseName = target.WeaponName.Substring("enchanted ".Length).Trim();
            InventoryItem item = state.Inventory.LastOrDefault(candidate =>
                candidate != null
                && InventoryEquipmentRules.IsWeaponSlot(candidate.Slot, candidate.Form)
                && string.Equals(candidate.DisplayName, baseName, StringComparison.Ordinal));
            if (item == null)
            {
                string baseType = StartingWeaponDamageType(target.Role);
                item = CreateEnchantmentWeaponItem(
                    target,
                    baseName,
                    target.WeaponBonus,
                    target.WeaponDamageMin,
                    target.WeaponDamageMax,
                    baseType);
                state.Inventory.Add(item);
            }
            else
            {
                item.Bonus = target.WeaponBonus;
                item.DamageMin = Mathf.Max(1, target.WeaponDamageMin);
                item.DamageMax = Mathf.Max(item.DamageMin + 1, target.WeaponDamageMax);
                item.AttackSpeed = Mathf.Max(1, target.WeaponAttackSpeed);
            }
            item.EquippedById = target.Id;

            string affinityId;
            switch ((target.WeaponDamageType ?? "").ToLowerInvariant())
            {
                case "fire": affinityId = "fire"; break;
                case "cold": affinityId = "ice"; break;
                case "light": affinityId = "radiance"; break;
                default: affinityId = "storm"; break;
            }
            WeaponEnchantmentRules.ApplyPermanent(item, affinityId);
            SyncEnchantedWeaponToOwner(item);
        }

        private void AdvanceTemporaryWeaponEnchantmentsAfterVictory()
        {
            if (state?.Inventory == null) return;
            int updated = 0;
            int expired = 0;
            foreach (InventoryItem item in state.Inventory.Where(item =>
                item != null && InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form)))
            {
                string before = WeaponEnchantmentRules.StatusText(item);
                if (!WeaponEnchantmentRules.AdvanceAfterVictory(item)) continue;
                string after = WeaponEnchantmentRules.StatusText(item);
                updated++;
                if (before.IndexOf("temporary", StringComparison.OrdinalIgnoreCase) >= 0
                    && after.IndexOf("temporary", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    expired++;
                }
                SyncEnchantedWeaponToOwner(item);
            }

            if (expired > 0)
            {
                PushLog(expired == 1
                    ? "A temporary weapon temper spends its final spark and fades."
                    : $"{expired} temporary weapon tempers spend their final sparks and fade.", Tone.Normal);
            }
            else if (updated > 0)
            {
                PushLog("The temporary weapon tempers settle after the victory; their remaining duration is updated in Inventory.", Tone.Normal);
            }
            if (updated > 0) MarkUiDirty();
        }

        private void VisitKingHall()
        {
            SetStoryFlag(StoryFlags.MidgaardKingMet);
            bool contractAccepted = HasStoryFlag(StoryFlags.MidgaardRatQuestGiven);
            bool armorEarned = HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade);
            bool fullPrototype = ContentSetCatalog.IsFullPrototype(activeContentSet);
            if (!contractAccepted)
            {
                PushLog("King Halvard leaves the cistern writ sealed while the party hears its terms.", Tone.Normal);
                ShowBanner("Royal Audience");
            }
            else if (!armorEarned)
            {
                PushLog(fullPrototype
                    ? "The king waits for proof from the sewers. Four rat pelts will earn the armorer's upgrade."
                    : "The king waits for proof from the three sewer rooms. The armorer will trade it for the first reward.",
                    Tone.Normal);
            }
            else if (fullPrototype && !HasStoryFlag(StoryFlags.MidgaardSecondQuestGiven))
            {
                SetStoryFlag(StoryFlags.MidgaardSecondQuestGiven);
                state.ActiveStory = "Chapter I: The Midgaard Cisterns. Rat-pelt armor earned, find the stair below the Salt Cisterns and prove the party can descend.";
                PushLog("The king nods at the rat-pelt armor. Next work: find the stair below the cisterns and open the deeper road.", Tone.Good);
                ShowBanner("Next Royal Work");
            }
            else if (!fullPrototype)
            {
                PushLog("The king marks the sewer work complete. Sluice Steps now opens the Old Road toward Dusk Market.", Tone.Good);
                ShowBanner("Sewer Contract Complete");
            }
            else
            {
                PushLog("The king's hall is ready for later quest chains. For now, the Old Road still waits below.", Tone.Normal);
            }
            ShowHalvardConversation();
            PlaySfx("ui", 0.64f);
        }

        private void ShowHalvardConversation()
        {
            bool contractAccepted = HasStoryFlag(StoryFlags.MidgaardRatQuestGiven);
            ShowDialogueChoices(
                "King's Hall",
                "King Halvard",
                !contractAccepted
                    ? "The writ stays sealed until you have heard it whole. Ask what you need, then decide whether the work is yours."
                    : MidgaardRatPeltsReady()
                        ? "Three chambers cleared, and proof brought home. Good. Ask what comes next."
                        : "The cistern writ is still open. If any part of it is unclear, ask before you descend.",
                ObjectType.KingHalvard,
                gold,
                contractAccepted
                    ? new[]
                    {
                        MakeDialogueChoice("duty", "What is our current duty?", "Review the writ, its progress, and the proof still required."),
                        MakeDialogueChoice("city", "What are we protecting here?", "Why the cistern work matters to Midgaard."),
                        MakeDialogueChoice("road", "What is the Old Road?", "What lies beyond the first contract.")
                    }
                    : new[]
                    {
                        MakeDialogueChoice("accept", "We accept the writ.", "Take responsibility for clearing the three cistern chambers."),
                        MakeDialogueChoice("duty", "What are the terms?", "Hear the objective, proof, and payment before deciding."),
                        MakeDialogueChoice("city", "Why does this work matter?", "Ask what the cisterns threaten."),
                        MakeDialogueChoice("decline", "We are not ready.", "Leave the writ sealed for now.")
                    },
                ResolveHalvardDialogueChoice);
        }

        private void ResolveHalvardDialogueChoice(string choice)
        {
            switch (choice)
            {
                case "accept":
                    if (!HasStoryFlag(StoryFlags.MidgaardRatQuestGiven))
                    {
                        AcceptSewerContract();
                        PushLog("King Halvard entrusts the party with a sealed writ: clear Broken Sluice, Foul Runoff, and the Cistern Den, then bring proof to Borin.", Tone.Good);
                        ShowBanner("Royal Writ Accepted");
                        PlaySfx("thronechime", 0.62f);
                    }
                    ShowDialogueResponse("King's Hall", "King Halvard", "Then take the writ. Clear Broken Sluice, Foul Runoff, and the Cistern Den. Bring proof from all three to Borin; he will make the armor named as payment. Bring your people home as well. Then we will speak of the Old Road.", ObjectType.KingHalvard, gold, ShowHalvardConversation);
                    break;
                case "decline":
                    ShowDialogueResponse("King's Hall", "King Halvard", "Then leave it sealed. Speak with the watch, visit the temple, and prepare properly. The work will still be here when you are ready to answer for it.", ObjectType.KingHalvard, gold, ShowHalvardConversation);
                    break;
                case "city":
                    ShowDialogueResponse("King's Hall", "King Halvard", "Those cisterns once carried clean water beneath the south quarter. Now ratfolk use them to reach our stores and foundations. Leave the tunnels to them and we have not lost old stone—we have handed an enemy a road beneath our walls.", ObjectType.KingHalvard, gold, ShowHalvardConversation);
                    break;
                case "road":
                    ShowDialogueResponse("King's Hall", "King Halvard", HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade)
                        ? "The Old Road runs beyond the cistern stair toward Green Shrine Road, the old quarry, and country that no longer answers to Midgaard. Yara can mark the first western routes. Beyond them, even my reports grow thin."
                        : "Finish the cistern writ first. The stair below it opens onto roads we cannot patrol or quickly reinforce. Curiosity is not enough reason to send an untested company there.", ObjectType.KingHalvard, gold, ShowHalvardConversation);
                    break;
                default:
                    ShowDialogueResponse("King's Hall", "King Halvard", !HasStoryFlag(StoryFlags.MidgaardRatQuestGiven)
                        ? "Three chambers block the old cistern route: Broken Sluice, Foul Runoff, and the Cistern Den. Clear them and bring proof to Borin. He will make your payment in armor. Dangerous work, plainly stated—and still your choice."
                        : HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade)
                            ? "Your proof is accepted and Borin's payment is complete. The next route begins at the stair below the Salt Cisterns. Locate it, confirm what lies beyond, and return before committing to a deeper expedition."
                            : MidgaardRatPeltsReady()
                                ? "The three chambers are clear. Take the proof to Borin and claim the armor named in your writ."
                                : "Clear Broken Sluice, Foul Runoff, and the Cistern Den. Bring proof from all three to Borin. Do not pursue unmarked tunnels beyond the contract.", ObjectType.KingHalvard, gold, ShowHalvardConversation);
                    break;
            }
        }

        private void AcceptSewerContract()
        {
            if (state.StoryFlags == null) state.StoryFlags = new List<string>();
            ContentSetCatalog.MarkSewerSliceContractAccepted(state.StoryFlags);
            state.ActiveStory = "Chapter I: The Midgaard Cisterns. Enter the sewer, clear three chambers, then return proof to the armorer.";
        }

        private void VisitRoyalHerald()
        {
            bool first = !HasStoryFlag(StoryFlags.MidgaardRoyalHeraldMet);
            SetStoryFlag(StoryFlags.MidgaardRoyalHeraldMet);
            if (first)
            {
                AwardWorldExperience(4, "Royal notice read");
                PushLog("The royal herald explains the first writ: sewer proof before deeper road charters.", Tone.Good);
            }
            else
            {
                PushLog("The royal herald keeps the same notices pinned in a perfect row.", Tone.Normal);
            }
            ShowBanner("Royal Herald");
            ShowVannConversation();
        }

        private void ShowVannConversation()
        {
            ShowDialogueChoices(
                "Royal Notice",
                "Herald Vann",
                "The cistern writ is brief, which has not prevented people from misreading it. Which clause requires explanation?",
                ObjectType.RoyalHerald,
                gold,
                new[]
                {
                    MakeDialogueChoice("writ", "What exactly does the writ require?", "The contracted chambers and acceptable proof."),
                    MakeDialogueChoice("reward", "What is the payment?", "What Borin and the crown owe for completed work."),
                    MakeDialogueChoice("charter", "Why are deeper routes restricted?", "The requirements for an Old Road charter.")
                },
                ResolveVannDialogueChoice);
        }

        private void ResolveVannDialogueChoice(string choice)
        {
            switch (choice)
            {
                case "reward":
                    ShowDialogueResponse("Royal Notice", "Herald Vann", HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade)
                        ? "Your proof is recorded, and Borin's armor satisfies the payment named in the writ. Further payment requires a further charter. The crown does not reimburse repeated presentation of the same teeth."
                        : "Clear all three contracted chambers and deliver the proof to Borin. He will turn the recovered hide into armor, and the crown adds ten gold when the claim is settled. Partial clearance earns no partial coat.", ObjectType.RoyalHerald, gold, ShowVannConversation);
                    break;
                case "charter":
                    ShowDialogueResponse("Royal Notice", "Herald Vann", "An Old Road charter requires a completed city writ, proof of return, and a report the watch can use. Midgaard cannot rescue every expedition that treats an unmarked stair as permission. Finish the bounded work first.", ObjectType.RoyalHerald, gold, ShowVannConversation);
                    break;
                default:
                    ShowDialogueResponse("Royal Notice", "Herald Vann", HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade)
                        ? "The cistern writ is satisfied. Any deeper stair, organized force, or damaged city work must be reported before you proceed beyond the marked route."
                        : "By order of King Halvard: clear Broken Sluice, Foul Runoff, and the Cistern Den. Deliver proof from all three to Borin. Unmarked tunnels are outside the writ and outside the watch's planned rescue route.", ObjectType.RoyalHerald, gold, ShowVannConversation);
                    break;
            }
        }

        private void VisitOldRoadScout()
        {
            bool first = !HasStoryFlag(StoryFlags.MidgaardOldRoadScoutMet);
            SetStoryFlag(StoryFlags.MidgaardOldRoadScoutMet);
            if (first)
            {
                DiscoverZoneHint(1, "green-shrine-road");
                DiscoverZoneHint(1, "old-quarry");
                state.Supplies += 1;
                AwardWorldExperience(5, "Old Road scout notes");
                PushLog("The old-road scout marks Green Shrine Road and Old Quarry in the journal margins.", Tone.Good);
            }
            else
            {
                PushLog("The old-road scout repeats the same road rule: return before pride empties the pack.", Tone.Normal);
            }
            ShowBanner("Old Road Scout");
            ShowYaraConversation();
        }

        private void ShowYaraConversation()
        {
            ShowDialogueChoices(
                "Old Road Scout",
                "Yara",
                "I have current notes on Green Shrine Road and the old quarry. I can also show you how the cover changes along the western route. Which do you need?",
                ObjectType.OldRoadScout,
                moss,
                new[]
                {
                    MakeDialogueChoice("shrine", "What is Green Shrine Road like?", "Roots, old shrines, and narrow approaches."),
                    MakeDialogueChoice("quarry", "What should we expect at the quarry?", "Open sight lines, broken levels, and heavy enemies."),
                    MakeDialogueChoice("cover", "How reliable is the cover?", "Using roots and stone without becoming trapped behind them.")
                },
                ResolveYaraDialogueChoice);
        }

        private void ResolveYaraDialogueChoice(string choice)
        {
            switch (choice)
            {
                case "quarry":
                    ShowDialogueResponse("Old Road Scout", "Yara", "The quarry floor is open, but the old cuts split it into ledges. You will see heavy tracks near the ramps and loose stone below them. Bring ranged weapons, keep the group from bunching at the climbs, and leave yourself a way off any ledge you take.", ObjectType.OldRoadScout, moss, ShowYaraConversation);
                    break;
                case "cover":
                    ShowDialogueResponse("Old Road Scout", "Yara", "Roots and low stone will stop a charge or spoil a shot, but they will not hold forever. Use them while you fire, then move before the enemy works around them. Keep a blade ready for anything that closes past your bow.", ObjectType.OldRoadScout, moss, ShowYaraConversation);
                    break;
                default:
                    ShowDialogueResponse("Old Road Scout", "Yara", "Green Shrine Road narrows between old stones and thick roots. The western bend is wet, and the shrines hide anyone standing downhill from you. Keep your healer in the middle, check both sides before approaching a light, and remember that enemies use the same cover you do.", ObjectType.OldRoadScout, moss, ShowYaraConversation);
                    break;
            }
        }

        private void DiscoverZoneHint(int depth, string zoneId)
        {
            if (state == null || string.IsNullOrEmpty(zoneId)) return;
            if (state.DiscoveredZones == null) state.DiscoveredZones = new List<string>();
            string key = ZoneKey(depth, zoneId);
            if (!state.DiscoveredZones.Contains(key)) state.DiscoveredZones.Add(key);
        }

        private void EnterMidgaardSewer()
        {
            if (!HasStoryFlag(StoryFlags.MidgaardRatQuestGiven))
            {
                AcceptSewerContract();
                PushLog("The sewer watchman checks the writ: 'Broken Sluice, Foul Runoff, Cistern Den. Bring proof from all three to Borin.'", Tone.Good);
            }
            if (!ContentSetCatalog.IsFullPrototype(activeContentSet))
            {
                int cleared = ContentSetCatalog.SewerSliceClearedCount(state.StoryFlags);
                if (cleared >= ContentSetCatalog.SewerSliceEncounters.Count)
                {
                    PushLog("The sewer contract is clear. Return to the armorer with proof from the den.", Tone.Good);
                    ShowBanner("Sewer Cleared");
                    PlaySfx("ui", 0.62f);
                    return;
                }

                if (cleared >= 2 && !ContentSetCatalog.HasSewerSafeRoomChoice(state.StoryFlags))
                {
                    ShowSewerSafeRoomChoice();
                    return;
                }

                EncounterDefinition encounter = ContentSetCatalog.SewerSliceEncounterForProgress(cleared);
                state.ActiveStory = SewerSliceObjectiveLine(cleared);
                PushLog("The sewer grate drops into damp brick and rat scratchings.", Tone.Warn);
                ShowBanner(encounter.Banner);
                PlaySfx("encounter", 0.74f);
                AutosaveCheckpoint("before " + encounter.Banner);
                StartCombat(encounter);
                return;
            }

            PushLog("The sewer grate drops into damp brick and rat scratchings.", Tone.Warn);
            ShowBanner("Midgaard Sewer");
            PlaySfx("encounter", 0.74f);
            AutosaveCheckpoint("before Midgaard Sewer");
            StartCombat(EncounterId.MidgaardSewer);
        }

        private string SewerSliceObjectiveLine(int clearedCount)
        {
            switch (Mathf.Clamp(clearedCount, 0, 3))
            {
                case 0: return "Chapter I: The Midgaard Cisterns. Clear Broken Sluice and learn the turn order.";
                case 1: return "Chapter I: The Midgaard Cisterns. Press into Foul Runoff and use healing, warding, and cover.";
                case 2: return "Chapter I: The Midgaard Cisterns. Break the Cistern Den: stop the Plague Mage, then the Brute.";
                default: return "Chapter I: The Midgaard Cisterns. Return proof to the Midgaard armorer.";
            }
        }

        private bool ShowSewerSafeRoomChoice()
        {
            if (state == null || ContentSetCatalog.HasSewerSafeRoomChoice(state.StoryFlags)) return false;
            ShowDialogueChoices(
                "Dry Maintenance Alcove",
                "Sewer Cache",
                "Behind a rusted sluice wheel, two serviceable relics remain dry. The party can carry only one before entering the Cistern Den.",
                ObjectType.Sewer,
                teal,
                new[]
                {
                    new DialogueChoiceView
                    {
                        Id = "blade",
                        Label = "Take the sluicekeeper blade",
                        Hint = "+2 physical broadsword / Strength +1 / better front-line pressure.",
                        Enabled = true
                    },
                    new DialogueChoiceView
                    {
                        Id = "focus",
                        Label = "Take the stormglass focus",
                        Hint = "+2 shock ritual staff / Intelligence +1 / favors Mage or Priest.",
                        Enabled = true
                    }
                },
                ResolveSewerSafeRoomChoice);
            return true;
        }

        private void ResolveSewerSafeRoomChoice(string choice)
        {
            if (state == null)
            {
                CloseDialogue();
                return;
            }
            if (ContentSetCatalog.HasSewerSafeRoomChoice(state.StoryFlags))
            {
                CloseDialogue();
                return;
            }

            bool takeFocus = string.Equals(choice, "focus", StringComparison.OrdinalIgnoreCase);
            InventoryItem item = takeFocus
                ? ContentSetCatalog.CreateSewerSafeRoomFocus()
                : ContentSetCatalog.CreateSewerSafeRoomBlade();
            EnsureInventoryList();
            if (state.StoryFlags == null) state.StoryFlags = new List<string>();
            ContentSetCatalog.MarkSewerSafeRoomChoice(state.StoryFlags, takeFocus ? "focus" : "blade");
            state.Inventory.Add(item);
            string equipNote = AutoEquipItem(item);
            state.ActiveStory = SewerSliceObjectiveLine(2);

            CloseDialogue();
            ShowLootPanel(item, 0, 0, 0, equipNote, "Safe-Room Choice");
            PushLog($"The party chooses {item.DisplayName}. {equipNote}", Tone.Good);
            ShowBanner(takeFocus ? "Stormglass Focus" : "Sluicekeeper Blade");
            PlaySfx("cache", 0.76f);
            AutosaveCheckpoint("sewer safe-room choice");
        }

        private InventoryItem MakeTownArmor()
        {
            return new InventoryItem
            {
                Mark = "serviceable",
                Material = "iron",
                Form = "chain hauberk",
                Trait = "guarding",
                Slot = "armor",
                Bonus = 2,
                HealthBonus = 1,
                Rarity = "common",
                DisplayName = "+1 serviceable iron chain hauberk"
            };
        }

        private InventoryItem MakeTownWeapon(string role)
        {
            InventoryItem item = MakeRoleItem(role, true, true);
            item.Mark = "town-forged";
            item.Material = "fine steel";
            item.Trait = "keen";
            item.Bonus = Mathf.Max(1, item.Bonus);
            Vector2Int damage = ItemDamageRange(item.Form, item.Bonus, item.Trait, item.Material);
            item.DamageMin = damage.x;
            item.DamageMax = damage.y;
            item.AttackSpeed = Mathf.Max(1, ItemAttackSpeed(item.Form, item.Mark, item.Material, item.Trait));
            item.DamageType = "physical";
            item.Rarity = "common";
            item.DisplayName = $"+{item.Bonus} town-forged {item.Form}";
            return item;
        }

        private InventoryItem MakeRatPelt()
        {
            return ContentSetCatalog.CreateSewerSliceProof();
        }

        private void EnsureInventoryList()
        {
            if (state != null && state.Inventory == null) state.Inventory = new List<InventoryItem>();
        }

        private InventoryItem MakeRatPeltArmor()
        {
            return ContentSetCatalog.CreateSewerSliceReward();
        }

        private int RatPeltCount()
        {
            return ContentSetCatalog.CountSewerSliceProof(state?.Inventory);
        }

        private bool MidgaardRatPeltsReady()
        {
            if (!ContentSetCatalog.IsFullPrototype(activeContentSet))
            {
                return ContentSetCatalog.SewerSliceRewardReady(state?.StoryFlags, state?.Inventory)
                    || HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade);
            }
            return HasStoryFlag(StoryFlags.MidgaardRatPeltsCollected)
                || HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade)
                || RatPeltCount() >= 4;
        }

        private void RemoveRatPelts(int count)
        {
            ContentSetCatalog.RemoveSewerSliceProof(state?.Inventory, count);
        }

        private bool TryCompleteRatPeltArmor()
        {
                if (HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade))
                {
                if (!HasStoryFlag(StoryFlags.SewerRewardClaimed))
                {
                    if (state.StoryFlags == null) state.StoryFlags = new List<string>();
                    ContentSetCatalog.MarkSewerSliceRewardClaimed(state.StoryFlags);
                    EnsureOldRoadDescentMarker();
                    DiscoverZoneHint(1, "green-shrine-road");
                    DiscoverZoneHint(1, "old-quarry");
                    state.ActiveStory = ContentSetCatalog.IsSewerSlice(activeContentSet)
                        ? "Chapter II unlocked: follow the Salt Cisterns road to Sluice Steps and descend toward Dusk Market."
                        : "Chapter I complete: the Old Road scout marks Green Shrine Road and Old Quarry as the next routes.";
                    AutosaveCheckpoint("chapter reward repaired");
                }
                PushLog("The rat-pelt armor pattern is finished. The armorer can grow into a proper shop later.", Tone.Normal);
                ShowBorinConversation("The sewer-hide coat is holding at the shoulder and the seams are still dry. What do you need?");
                return true;
            }
            if (!ContentSetCatalog.IsFullPrototype(activeContentSet))
            {
                EnsureInventoryList();
                if (state.StoryFlags == null) state.StoryFlags = new List<string>();
                if (!ContentSetCatalog.TryClaimSewerSliceReward(state.StoryFlags, state.Inventory, out InventoryItem sewerReward, out string sewerNote))
                {
                    PushLog(sewerNote, Tone.Normal);
                    return false;
                }

                state.Inventory.Add(sewerReward);
                string sewerEquipNote = AutoEquipItem(sewerReward);
                state.Gold += 10;
                SetStoryFlag(StoryFlags.MidgaardRatPeltArmorMade);
                EnsureOldRoadDescentMarker();
                DiscoverZoneHint(1, "green-shrine-road");
                DiscoverZoneHint(1, "old-quarry");
                state.ActiveStory = "Chapter II unlocked: follow the Salt Cisterns road to Sluice Steps and descend toward Dusk Market.";
                ShowDialogueThenLoot(
                    "Rat-Pelt Workbench",
                    "Borin",
                    "I cleaned the hide, doubled the weak seams, and left room at the shoulders. It is lighter than mail and better suited to wet tunnels. Keep it dry when you can.",
                    ObjectType.Armorer,
                    stone,
                    sewerReward,
                    10,
                    0,
                    0,
                    sewerEquipNote,
                    "Rat-Pelt Armor");
                PushLog($"The armorer stitches the proof into {sewerReward.DisplayName}. {sewerEquipNote}", Tone.Good);
                PushLog("The Old Road opens: Sluice Steps now descends toward Dusk Market and the kobold smoke route.", Tone.Good);
                ShowBanner("Rat-Pelt Armor");
                PlaySfx("cache", 0.78f);
                AutosaveCheckpoint("chapter reward claimed");
                return true;
            }
            int pelts = RatPeltCount();
            if (pelts < 4 && !HasStoryFlag(StoryFlags.MidgaardRatPeltsCollected))
            {
                PushLog($"The armorer needs four rat pelts. Current pelts: {pelts}/4.", Tone.Normal);
                return false;
            }

            RemoveRatPelts(Mathf.Min(4, pelts));
            EnsureInventoryList();
            InventoryItem item = MakeRatPeltArmor();
            state.Inventory.Add(item);
            string equipNote = AutoEquipItem(item);
            state.Gold += 10;
            SetStoryFlag(StoryFlags.MidgaardRatPeltArmorMade);
            if (state.StoryFlags == null) state.StoryFlags = new List<string>();
            ContentSetCatalog.MarkSewerSliceRewardClaimed(state.StoryFlags);
            EnsureOldRoadDescentMarker();
            DiscoverZoneHint(1, "green-shrine-road");
            DiscoverZoneHint(1, "old-quarry");
            state.ActiveStory = "Chapter I complete: rat-pelt armor earned. The Old Road scout marks Green Shrine Road and Old Quarry as the next routes.";
            ShowDialogueThenLoot(
                "Rat-Pelt Workbench",
                "Borin",
                "I cleaned the hide, doubled the weak seams, and left room at the shoulders. It is lighter than mail and better suited to wet tunnels. Keep it dry when you can.",
                ObjectType.Armorer,
                stone,
                item,
                10,
                0,
                0,
                equipNote,
                "Rat-Pelt Armor");
            PushLog($"The armorer stitches the pelts into {item.DisplayName}. {equipNote}", Tone.Good);
            PushLog("Old Road teaser unlocked: Green Shrine Road and Old Quarry are marked in the journal.", Tone.Good);
            ShowBanner("Rat-Pelt Armor");
            PlaySfx("cache", 0.78f);
            AutosaveCheckpoint("chapter reward claimed");
            return true;
        }

        private void ApplyMidgaardStoryVictory(string encounterStyle)
        {
            if (ContentSetCatalog.IsSewerSliceEncounterStyle(encounterStyle))
            {
                if (state.StoryFlags == null) state.StoryFlags = new List<string>();
                bool firstClear = !HasStoryFlag(ContentSetCatalog.ClearedFlagForEncounterStyle(encounterStyle));
                ContentSetCatalog.MarkSewerSliceEncounterCleared(state.StoryFlags, encounterStyle);

                int cleared = ContentSetCatalog.SewerSliceClearedCount(state.StoryFlags);
                if (encounterStyle == "sewer_foul_runoff")
                {
                    RestoreLivingParty(10, 6);
                    state.Supplies += 1;
                    PushLog("A dry maintenance alcove restores the party and offers one piece of service gear. Supplies +1.", Tone.Good);
                    ShowSewerSafeRoomChoice();
                }

                if (cleared >= ContentSetCatalog.SewerSliceEncounters.Count)
                {
                    EnsureInventoryList();
                    if (firstClear) state.Inventory.Add(MakeRatPelt());
                    state.ActiveStory = "Chapter I: The Midgaard Cisterns. Return sewer proof to the Midgaard armorer for the first equipment reward.";
                    PushLog("The Cistern Den is broken. The party has three proof bundles for the armorer's reward.", Tone.Good);
                }
                else
                {
                    EnsureInventoryList();
                    if (firstClear) state.Inventory.Add(MakeRatPelt());
                    state.ActiveStory = SewerSliceObjectiveLine(cleared);
                    PushLog(state.ActiveStory, Tone.Good);
                }
                return;
            }

            if (encounterStyle != "ratsewer") return;
            int pelts = HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade) ? 2 : 4;
            EnsureInventoryList();
            for (int i = 0; i < pelts; i++) state.Inventory.Add(MakeRatPelt());
            SetStoryFlag(StoryFlags.MidgaardRatPeltsCollected);
            if (!HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade))
            {
                state.ActiveStory = "Chapter I: The Midgaard Cisterns. Return four rat pelts to the Midgaard armorer for starter armor.";
            }
            PushLog($"The party gathers {pelts} rat pelt{(pelts == 1 ? "" : "s")} from the sewer fight.", Tone.Good);
        }

        private void RecallToTempleSquare()
        {
            if (state == null || state.Mode != GameMode.Explore) return;
            ReturnPartyToTempleSquare(
                "Recall pulls the party back to Temple Square. The fountain steadies every breath.",
                "Recall: Temple Square",
                0.90f);
        }

        private bool ReturnPartyToTempleSquare(string logText, string bannerText, float sfxVolume)
        {
            if (state == null) return false;
            if (state.Depth != 1 || state.Map == null || state.Map.Depth != 1)
            {
                state.Depth = 1;
                state.Map = GenerateMap(state.Depth, state.Seed);
                InvalidateExplorationController();
            }
            EnsureWorldLandmarks();
            MapObject recall = state.Map.Objects.FirstOrDefault(o => o.Type == ObjectType.RecallCircle)
                ?? state.Map.Objects.FirstOrDefault(o => o.Type == ObjectType.Fountain)
                ?? state.Map.Objects.FirstOrDefault(o => o.Type == ObjectType.Temple);
            int oldX = state.PlayerX;
            int oldY = state.PlayerY;
            if (recall == null)
            {
                PlacePlayerAtExplorationStart();
                PushLog("Temple Square's anchor was repaired during the return.", Tone.Warn);
            }
            else
            {
                state.PlayerX = recall.X;
                state.PlayerY = recall.Y;
            }
            lastExploreRegion = ExploreRegionName(state.PlayerX, state.PlayerY);
            if (!state.ReducedMotion) tweens.Add(new Tween("party", new Vector2(oldX, oldY), new Vector2(state.PlayerX, state.PlayerY), Time.time, 0.20f, TweenKind.Move));
            RestorePartyFully();
            SetStoryFlag(StoryFlags.MidgaardTempleFound);
            PushLog(logText, Tone.Good);
            ShowBanner(bannerText);
            PlaySfx("shrine", sfxVolume);
            AddBurst(state.PlayerX, state.PlayerY, teal);
            return true;
        }

        private void Camp()
        {
            if (state.Supplies <= 0)
            {
                PushLog("The packs hold no supplies.", Tone.Warn);
                PlaySfx("blocked", 0.48f);
                return;
            }
            state.Supplies--;
            foreach (PartyMember member in state.Party.Where(p => p.Hp > 0))
            {
                member.Hp = Mathf.Min(member.MaxHp, member.Hp + 13);
                member.Mana = Mathf.Min(member.MaxMana, member.Mana + 8);
            }
            PushLog("A guarded campfire buys a little strength.", Tone.Good);
            PlaySfx("rest", 0.72f);
        }

        private const string OldRoadDescentId = "old-road-descent-sluice-steps";

        private void EnsureOldRoadDescentMarker()
        {
            if (!ContentSetCatalog.IsSewerSlice(activeContentSet)
                || !ContentSetCatalog.AllowKoboldChapter(activeContentSet, state?.StoryFlags)
                || state?.Map?.Objects == null
                || state.Depth != 1)
            {
                return;
            }

            MapObject existing = state.Map.FindObjectById(OldRoadDescentId);
            if (existing != null)
            {
                existing.Type = ObjectType.Stairs;
                return;
            }

            WorldMapJunction junction = WorldMapGenerationRules
                .RegionalJunctions(state.Map.Width, state.Map.Height, state.Map.StartX, state.Map.StartY)
                .FirstOrDefault(candidate => candidate.Id == "sluice-steps");
            if (string.IsNullOrEmpty(junction.Id)) return;

            MapObject occupant = ObjectAt(state.Map, junction.X, junction.Y);
            if (occupant != null)
            {
                if (occupant.Type != ObjectType.Stairs) return;
                occupant.Id = OldRoadDescentId;
                state.Map.InvalidateObjectLookup();
                return;
            }

            state.Map.Objects.Add(new MapObject(junction.X, junction.Y, ObjectType.Stairs, OldRoadDescentId));
            state.Map.InvalidateObjectLookup();
            ExplorationSurfaceRules.AddRoles(
                state.Map,
                junction.X,
                junction.Y,
                ExplorationCellRole.Road | ExplorationCellRole.Clearing | ExplorationCellRole.Threshold);
        }

        private bool CanDescend()
        {
            if (state?.Map == null) return false;
            MapObject obj = ObjectAt(state.Map, state.PlayerX, state.PlayerY);
            if (obj == null || obj.Type != ObjectType.Stairs) return false;
            if (ContentSetCatalog.IsFullPrototype(activeContentSet)) return true;
            return state.Depth == 1
                && ContentSetCatalog.AllowKoboldChapter(activeContentSet, state.StoryFlags)
                && string.Equals(obj.Id, OldRoadDescentId, StringComparison.Ordinal);
        }

        private void Descend()
        {
            if (ContentSetCatalog.IsSewerSlice(activeContentSet)
                && !ContentSetCatalog.AllowKoboldChapter(activeContentSet, state?.StoryFlags))
            {
                PushLog("The Old Road descent opens after the sewer contract and Borin's reward.", Tone.Normal);
                ShowBanner("Old Road Locked");
                return;
            }
            if (!CanDescend())
            {
                PushLog("No stairway lies underfoot.", Tone.Warn);
                return;
            }
            state.ActiveRouteWaypointKey = "";
            state.Depth++;
            state.StoryChapter = Mathf.Max(state.StoryChapter + 1, state.Depth);
            state.ActiveStory = StoryObjectiveForDepth(state.Depth);
            state.Supplies += 2;
            state.Map = GenerateMap(state.Depth, state.Seed);
            InvalidateExplorationController();
            EnsureWorldLandmarks();
            PlacePlayerAtExplorationStart();
            lastExploreRegion = ExploreRegionName(state.PlayerX, state.PlayerY);
            DiscoverCurrentZone(true);
            PushLog($"The party descends to depth {state.Depth}. {state.ActiveStory}", Tone.Good);
            AwardWorldExperience(18 + state.Depth * 6, $"Chapter {state.Depth} reached");
            ShowBanner(StoryChapterTitle());
            PlaySfx("encounter", 0.8f);
            if (state.Depth >= FinalBossDepth)
            {
                PushLog("The final gate answers. A meteor-crowned will waits beyond it.", Tone.Warn);
                StartCombat(EncounterId.FinalGate);
            }
        }

        private InventoryItem MakeItem()
        {
            bool weapon = rng.NextDouble() < 0.62;
            return MakeRoleItem("", weapon, false);
        }

        private InventoryItem MakeCombatLootItem(string encounterStyle, bool bossLoot)
        {
            if (state?.Combat?.Units == null) return null;
            List<CombatUnit> enemies = state.Combat.Units.Where(u => u.Side == UnitSide.Enemy).ToList();
            if (enemies.Count == 0) return null;
            if (bossLoot)
            {
                return MakeBossLoot(encounterStyle);
            }

            float dropChance = Mathf.Clamp(0.18f + state.Depth * 0.045f + enemies.Count * 0.018f, 0.18f, 0.62f);
            foreach (CombatUnit enemy in enemies)
            {
                if (enemy.Rank == "veteran") dropChance += 0.08f;
                if (enemy.Rank == "elite") dropChance += 0.20f;
                if (EnemyIsGearCarrier(enemy)) dropChance += 0.035f;
            }
            dropChance = Mathf.Clamp(dropChance, 0.12f, 0.82f);
            if (rng.NextDouble() > dropChance) return null;

            CombatUnit source = enemies
                .OrderByDescending(LootSourceScore)
                .ThenBy(_ => rng.Next())
                .FirstOrDefault();
            if (source == null) return null;

            string role = LootRoleForEnemy(source);
            bool weapon = LootWeaponChance(source) >= rng.NextDouble();
            InventoryItem item = MakeRoleItem(role, weapon, false);
            ImproveLootItemFromEnemy(item, source);
            return item;
        }

        private InventoryItem MakeBossLoot(string encounterStyle)
        {
            if (encounterStyle == "koboldking" || IsKoboldKingCombat()) return MakeSwordOfUnfathomableDarkness();
            InventoryItem item = MakeRoleItem("ember", true, false);
            item.Mark = "legendary";
            item.Material = "meteor iron";
            item.Trait = "fallen star";
            item.Bonus = Mathf.Max(item.Bonus, 5);
            item.Rarity = "legendary";
            item.DamageType = "fire";
            item.DisplayName = "+5 legendary meteor iron scepter of fallen stars";
            item.DamageMin = Mathf.Max(item.DamageMin, 4);
            item.DamageMax = Mathf.Max(item.DamageMax, 12);
            item.AttackSpeed = Mathf.Max(item.AttackSpeed, 8);
            item.IntelligenceBonus = Mathf.Max(item.IntelligenceBonus, 2);
            return item;
        }

        private InventoryItem MakeSwordOfUnfathomableDarkness()
        {
            return new InventoryItem
            {
                Mark = "stolen",
                Material = "blackglass",
                Form = "broadsword",
                Trait = "unfathomable darkness",
                Slot = "weapon",
                Bonus = 4,
                StrengthBonus = 1,
                IntelligenceBonus = 0,
                AgilityBonus = 1,
                HealthBonus = 1,
                DamageMin = 5,
                DamageMax = 11,
                AttackSpeed = 8,
                Rarity = "epic",
                DamageType = "death",
                DisplayName = "Sword of Unfathomable Darkness"
            };
        }

        private int LootSourceScore(CombatUnit enemy)
        {
            if (enemy == null) return 0;
            int score = enemy.MaxHp + enemy.Power * 3 + enemy.Defense * 2 + enemy.Range;
            if (enemy.Rank == "veteran") score += 24;
            if (enemy.Rank == "elite") score += 52;
            if (enemy.Fearless) score += 10;
            if (EnemyIsGearCarrier(enemy)) score += 12;
            return score;
        }

        private bool EnemyIsGearCarrier(CombatUnit enemy)
        {
            string role = (enemy?.Role ?? "").ToLowerInvariant();
            return role.Contains("kobold") || role.Contains("rat") || role.Contains("drow") || role.Contains("sentry") || role.Contains("knight") || role.Contains("reaver") || role.Contains("adept") || role.Contains("priest") || role.Contains("mage");
        }

        private string LootRoleForEnemy(CombatUnit enemy)
        {
            string role = (enemy?.Role ?? "").ToLowerInvariant();
            if (role.Contains("slinger") || role.Contains("archer") || role.Contains("crossbow")) return "bow";
            if (role.Contains("shield") || role.Contains("husk") || role.Contains("brute") || role.Contains("knight") || role.Contains("king")) return "shield";
            if (role.Contains("shaman") || role.Contains("priest") || role.Contains("cleric")) return "mender";
            if (role.Contains("wizard") || role.Contains("mage") || role.Contains("adept")) return rng.NextDouble() < 0.5 ? "ember" : "hex";
            if (role.Contains("cutthroat") || role.Contains("scout") || role.Contains("blade")) return "knife";
            if (role.Contains("reaver") || role.Contains("thorn")) return "pike";
            if (role.Contains("demon") || role.Contains("shade") || role.Contains("lich")) return "hex";
            return enemy != null && enemy.Range > 1 ? "bow" : "shield";
        }

        private float LootWeaponChance(CombatUnit enemy)
        {
            string role = (enemy?.Role ?? "").ToLowerInvariant();
            if (role.Contains("priest") || role.Contains("cleric") || role.Contains("mage") || role.Contains("wizard") || role.Contains("shaman")) return 0.64f;
            if (role.Contains("shield") || role.Contains("husk") || role.Contains("brute") || role.Contains("knight")) return 0.46f;
            if (role.Contains("spore") || role.Contains("beast") || role.Contains("rat")) return 0.38f;
            return 0.66f;
        }

        private void ImproveLootItemFromEnemy(InventoryItem item, CombatUnit enemy)
        {
            if (item == null || enemy == null) return;
            int floorBonus = Mathf.Clamp(state.Depth / 2, 0, 3);
            if (enemy.Rank == "veteran") floorBonus = Mathf.Max(floorBonus, 2);
            if (enemy.Rank == "elite") floorBonus = Mathf.Max(floorBonus, 3);
            item.Bonus = Mathf.Max(item.Bonus, floorBonus);
            if (InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form))
            {
                item.DamageMin = Mathf.Max(item.DamageMin, Mathf.Max(1, enemy.Power / 2));
                item.DamageMax = Mathf.Max(item.DamageMax, enemy.Power + (enemy.Rank == "elite" ? 4 : enemy.Rank == "veteran" ? 3 : 2));
                item.AttackSpeed = Mathf.Max(3, item.AttackSpeed);
                if (!string.IsNullOrEmpty(enemy.DamageType) && enemy.DamageType != "physical" && rng.NextDouble() < 0.45)
                {
                    item.DamageType = enemy.DamageType;
                    item.Trait = DamageTraitName(enemy.DamageType);
                }
            }
            else
            {
                item.HealthBonus = Mathf.Max(item.HealthBonus, enemy.Rank == "elite" ? 1 : item.HealthBonus);
            }

            if (enemy.Rank == "elite")
            {
                item.Rarity = RarityAtLeast(item.Rarity, "rare");
                if (!item.DisplayName.StartsWith("+") && item.Bonus > 0) item.DisplayName = "+" + item.Bonus + " " + item.DisplayName;
                item.DisplayName = item.DisplayName.Contains("marked") ? item.DisplayName : "marked " + item.DisplayName;
            }
            else if (enemy.Rank == "veteran")
            {
                item.Rarity = RarityAtLeast(item.Rarity, "uncommon");
                item.DisplayName = item.DisplayName.Contains("old-road") ? item.DisplayName : "old-road " + item.DisplayName;
            }
            else if (state.Depth >= 4)
            {
                item.Rarity = RarityAtLeast(item.Rarity, "uncommon");
            }
        }

        private string DamageTraitName(string damageType)
        {
            switch ((damageType ?? "").ToLowerInvariant())
            {
                case "fire": return "flame";
                case "cold": return "frost";
                case "shock": return "storm";
                case "poison": return "venom";
                case "death": return "death";
                case "mind": return "night";
                case "light": return "holy";
                default: return "keen";
            }
        }

        private string RarityAtLeast(string current, string floor)
        {
            string[] order = { "starter", "common", "uncommon", "rare", "epic", "legendary", "relic" };
            int currentIndex = Array.IndexOf(order, string.IsNullOrEmpty(current) ? "common" : current);
            int floorIndex = Array.IndexOf(order, string.IsNullOrEmpty(floor) ? "common" : floor);
            if (currentIndex < 0) currentIndex = 1;
            if (floorIndex < 0) floorIndex = 1;
            return order[Mathf.Max(currentIndex, floorIndex)];
        }

        private InventoryItem MakeRoleItem(string role, bool weapon, bool starter)
        {
            string[] weaponForms = RoleWeaponForms(role, starter);
            string[] armorForms = RoleArmorForms(role, starter);
            string[] materials = starter
                ? new[] { "iron", "ashwood", "hidebound", "silvered", "moonstone" }
                : new[] { "iron", "fine steel", "ashwood", "silvered", "obsidian", "blackglass", "ironwood", "crystalline", "mithril", "adamantine", "stormglass", "moonstone", "bone", "silk" };
            string[] qualities = starter
                ? new[] { "plain", "serviceable", "balanced", "well-made" }
                : new[] { "crude", "serviceable", "fine", "balanced", "masterwork", "dwarven", "elven", "weightless", "vicious", "holy", "vampiric", "anti-magic" };
            string[] traits = starter
                ? new[] { "guarding", "haste", "focus", "warding", "keen" }
                : new[] { "flame", "frost", "storm", "venom", "terror", "mercy", "night", "warding", "haste", "echoes", "thorns", "silence", "bleeding", "stunning", "guarding", "focus", "death" };
            string form = weapon ? weaponForms[rng.Next(weaponForms.Length)] : armorForms[rng.Next(armorForms.Length)];
            string material = materials[rng.Next(materials.Length)];
            string quality = qualities[rng.Next(qualities.Length)];
            string trait = traits[rng.Next(traits.Length)];
            int bonus = starter ? Mathf.Clamp(RollItemBonus(quality, material), 0, 2) : RollItemBonus(quality, material);
            string damageType = weapon ? ItemDamageType(trait, material) : "";
            string rarity = ItemRarity(quality, material, trait, bonus, starter);
            Stats statBonuses = RollItemStatBonuses(weapon, form, trait, material, starter);
            Vector2Int damage = weapon ? ItemDamageRange(form, bonus, trait, material) : new Vector2Int(0, 0);
            int speed = weapon ? ItemAttackSpeed(form, quality, material, trait) : 0;
            string plus = bonus > 0 ? $"+{bonus} " : bonus < 0 ? $"{bonus} " : "";
            string display = starter && rng.NextDouble() < 0.48
                ? $"{plus}{quality} {form}"
                : $"{plus}{quality} {material} {form} of {trait}";
            return new InventoryItem
            {
                Mark = quality,
                Material = material,
                Form = form,
                Trait = trait,
                Slot = weapon ? "weapon" : "armor",
                Bonus = bonus,
                StrengthBonus = statBonuses.Strength,
                IntelligenceBonus = statBonuses.Intelligence,
                AgilityBonus = statBonuses.Dexterity,
                HealthBonus = statBonuses.Health,
                DamageMin = damage.x,
                DamageMax = damage.y,
                AttackSpeed = speed,
                Rarity = rarity,
                DamageType = damageType,
                DisplayName = display
            };
        }

        private string ItemRarity(string quality, string material, string trait, int bonus, bool starter)
        {
            if (starter) return "starter";
            int score = bonus;
            if (quality == "masterwork" || quality == "holy" || quality == "vampiric" || quality == "anti-magic") score += 2;
            if (material == "mithril" || material == "adamantine" || material == "stormglass" || material == "blackglass") score++;
            if (trait == "death" || trait == "vampiric" || trait == "silence") score++;
            if (score >= 6) return "relic";
            if (score >= 4) return "rare";
            if (score >= 2) return "uncommon";
            return "common";
        }

        private Stats RollItemStatBonuses(bool weapon, string form, string trait, string material, bool starter)
        {
            int str = 0;
            int intel = 0;
            int agi = 0;
            int hea = 0;
            string text = $"{form} {trait} {material}".ToLowerInvariant();
            if (text.Contains("war hammer") || text.Contains("broadsword") || text.Contains("adamantine") || text.Contains("guarding")) str++;
            if (text.Contains("focus") || text.Contains("orb") || text.Contains("scepter") || text.Contains("moonstone") || text.Contains("blackglass")) intel++;
            if (text.Contains("epee") || text.Contains("sabre") || text.Contains("bow") || text.Contains("silk") || text.Contains("haste") || text.Contains("weightless")) agi++;
            if (text.Contains("plate") || text.Contains("tower") || text.Contains("ironwood") || text.Contains("warding")) hea++;
            if (!starter && rng.NextDouble() < 0.22)
            {
                int pick = rng.Next(4);
                if (pick == 0) str++;
                else if (pick == 1) intel++;
                else if (pick == 2) agi++;
                else hea++;
            }
            if (starter)
            {
                str = Mathf.Min(str, 1);
                intel = Mathf.Min(intel, 1);
                agi = Mathf.Min(agi, 1);
                hea = Mathf.Min(hea, 1);
            }
            return new Stats(str, intel, agi, hea);
        }

        private Vector2Int ItemDamageRange(string form, int bonus, string trait, string material)
        {
            string text = $"{form} {trait} {material}".ToLowerInvariant();
            int min = 2;
            int max = 5;
            if (text.Contains("epee") || text.Contains("sabre") || text.Contains("knife")) { min = 1; max = 5; }
            else if (text.Contains("longbow") || text.Contains("crossbow")) { min = 2; max = 6; }
            else if (text.Contains("sling") || text.Contains("darts")) { min = 1; max = 4; }
            else if (text.Contains("spear") || text.Contains("pike") || text.Contains("glaive") || text.Contains("halberd")) { min = 2; max = 7; }
            else if (text.Contains("war hammer") || text.Contains("war flail")) { min = 3; max = 8; }
            else if (text.Contains("focus") || text.Contains("orb") || text.Contains("scepter") || text.Contains("staff")) { min = 1; max = 5; }
            min += Mathf.Max(0, bonus / 2);
            max += Mathf.Max(0, bonus);
            if (text.Contains("vicious") || text.Contains("death") || text.Contains("vampiric")) max += 2;
            return new Vector2Int(Mathf.Max(1, min), Mathf.Max(min + 1, max));
        }

        private int ItemAttackSpeed(string form, string quality, string material, string trait)
        {
            string text = $"{form} {quality} {material} {trait}".ToLowerInvariant();
            int speed = 7;
            if (text.Contains("epee") || text.Contains("sabre") || text.Contains("knife") || text.Contains("darts")) speed = 11;
            else if (text.Contains("bow") || text.Contains("sling")) speed = 9;
            else if (text.Contains("crossbow") || text.Contains("war hammer") || text.Contains("tower")) speed = 5;
            else if (text.Contains("spear") || text.Contains("pike") || text.Contains("halberd")) speed = 7;
            else if (text.Contains("focus") || text.Contains("orb") || text.Contains("scepter") || text.Contains("staff")) speed = 8;
            if (text.Contains("balanced") || text.Contains("elven") || text.Contains("haste") || text.Contains("weightless") || text.Contains("mithril")) speed += 2;
            if (text.Contains("crude") || text.Contains("adamantine")) speed -= 1;
            return Mathf.Clamp(speed, 3, 16);
        }

        private string[] RoleWeaponForms(string role, bool starter)
        {
            if (role == "bow") return new[] { "longbow", "crossbow", "sling", "throwing darts" };
            if (role == "pike") return new[] { "long spear", "pike", "glaive", "halberd" };
            if (role == "knife") return new[] { "epee", "sabre", "ritual knife", "throwing knives" };
            if (role == "mender") return new[] { "prayer focus", "ash staff", "scepter", "ritual bell" };
            if (role == "ember") return new[] { "ember focus", "ash staff", "stormglass orb", "scepter" };
            if (role == "hex") return new[] { "bone focus", "ritual knife", "blackglass orb", "ash staff" };
            if (role == "shield" || role == "ward") return new[] { "broadsword", "mace", "war hammer", "war flail", "arming sword" };
            return starter ? new[] { "short sword", "staff", "knife" } : new[] { "epee", "sabre", "broadsword", "mace", "war flail", "long spear", "halberd", "longbow", "crossbow", "throwing knives", "ash staff", "ritual knife", "scepter", "orb" };
        }

        private string[] RoleArmorForms(string role, bool starter)
        {
            if (role == "shield") return new[] { "chain hauberk", "plate cuirass", "kite shield", "scale shirt" };
            if (role == "ward") return new[] { "tower shield", "warding robe", "mail and tower shield", "plate cuirass" };
            if (role == "pike") return new[] { "scale shirt", "chain hauberk", "leather jack", "bone greaves" };
            if (role == "bow") return new[] { "scout leathers", "leather jack", "silk mantle", "shadow cloak" };
            if (role == "knife") return new[] { "dark leathers", "silk mantle", "buckler", "shadow cloak" };
            if (role == "mender") return new[] { "warding robe", "silk mantle", "prayer mantle", "moonstone circlet" };
            if (role == "ember" || role == "hex") return new[] { "spell robe", "silk mantle", "warding robe", "moonstone circlet" };
            return starter ? new[] { "leather jack", "robe", "buckler" } : new[] { "padded jack", "leather jack", "scout leathers", "dark leathers", "scale shirt", "chain hauberk", "plate cuirass", "buckler", "kite shield", "tower shield", "warding robe", "spell robe", "silk mantle", "bone greaves", "shadow cloak", "iron helm" };
        }

        private int RollItemBonus(string quality, string material)
        {
            int bonus = rng.NextDouble() < 0.18 ? -1 : rng.Next(0, 3);
            if (quality == "fine" || quality == "balanced" || quality == "dwarven" || quality == "elven") bonus++;
            if (quality == "masterwork" || quality == "holy" || quality == "vampiric" || quality == "anti-magic") bonus += 2;
            if (material == "mithril" || material == "adamantine" || material == "crystalline") bonus++;
            return Mathf.Clamp(bonus, -1, 5);
        }

        private string ItemDamageType(string trait, string material)
        {
            if (trait == "flame") return "fire";
            if (trait == "frost") return "cold";
            if (trait == "storm") return "shock";
            if (trait == "venom") return "poison";
            if (trait == "terror" || trait == "night" || trait == "death") return "death";
            if (material == "silvered" || trait == "holy") return "light";
            return "physical";
        }

        private string AutoEquipItem(InventoryItem item)
        {
            if (item == null || state?.Party == null) return "";
            EnsureInventoryEquipmentLinks();
            IEnumerable<PartyMember> candidates = state.Party.Where(p => p.Hp > 0);
            if (InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form))
            {
                candidates = candidates.OrderByDescending(p => WeaponRoleFit(item, p)).ThenBy(p => p.WeaponBonus);
                PartyMember target = candidates.FirstOrDefault(p => WeaponRoleFit(item, p) > 0 && item.Bonus >= p.WeaponBonus);
                if (target == null) target = state.Party.Where(p => p.Hp > 0).OrderBy(p => p.WeaponBonus).FirstOrDefault();
                if (target == null) return "";
                if (item.Bonus < target.WeaponBonus) return "No one claims it yet.";
                EquipInventoryItemToMember(item, target, out string result);
                return result;
            }
            else
            {
                PartyMember target = candidates.OrderBy(p => p.ArmorBonus + ArmorRolePenalty(item, p)).FirstOrDefault();
                if (target == null) return "";
                if (item.Bonus < target.ArmorBonus) return "It goes into the pack.";
                EquipInventoryItemToMember(item, target, out string result);
                return result;
            }
        }

        private void ApplyGearStatBonuses(PartyMember member, InventoryItem item, bool weapon)
        {
            if (member == null || item == null) return;
            if (weapon)
            {
                member.WeaponStrengthBonus = item.StrengthBonus;
                member.WeaponIntelligenceBonus = item.IntelligenceBonus;
                member.WeaponAgilityBonus = item.AgilityBonus;
                member.WeaponHealthBonus = item.HealthBonus;
            }
            else
            {
                member.ArmorStrengthBonus = item.StrengthBonus;
                member.ArmorIntelligenceBonus = item.IntelligenceBonus;
                member.ArmorAgilityBonus = item.AgilityBonus;
                member.ArmorHealthBonus = item.HealthBonus;
            }
            member.GearStrength = member.WeaponStrengthBonus + member.ArmorStrengthBonus;
            member.GearIntelligence = member.WeaponIntelligenceBonus + member.ArmorIntelligenceBonus;
            member.GearAgility = member.WeaponAgilityBonus + member.ArmorAgilityBonus;
            member.GearHealth = member.WeaponHealthBonus + member.ArmorHealthBonus;
        }

        private int WeaponRoleFit(InventoryItem item, PartyMember member)
        {
            string form = item.Form ?? "";
            if (member.Role == "bow" && (form.Contains("bow") || form.Contains("crossbow"))) return 6;
            if (member.Role == "pike" && (form.Contains("spear") || form.Contains("halberd"))) return 6;
            if (member.Role == "knife" && (form.Contains("knife") || form.Contains("epee") || form.Contains("sabre"))) return 6;
            if ((member.Role == "ember" || member.Role == "hex" || member.Role == "mender") && (form.Contains("staff") || form.Contains("ritual"))) return 6;
            if (member.Role == "shield" || member.Role == "ward") return form.Contains("mace") || form.Contains("sword") || form.Contains("flail") ? 5 : 2;
            return 2;
        }

        private int WeaponRange(InventoryItem item, PartyMember member)
        {
            string form = item.Form ?? "";
            if (form.Contains("longbow") || form.Contains("crossbow")) return 4;
            if (form.Contains("throwing")) return 3;
            if (form.Contains("sling") || form.Contains("darts")) return 3;
            if (form.Contains("spear") || form.Contains("pike") || form.Contains("glaive") || form.Contains("halberd")) return 2;
            if ((form.Contains("focus") || form.Contains("orb") || form.Contains("scepter") || form.Contains("staff")) && (member.Role == "ember" || member.Role == "hex" || member.Role == "mender")) return 4;
            return member.Role == "bow" ? 4 : member.Role == "ember" || member.Role == "hex" ? 3 : 1;
        }

        private int ArmorRolePenalty(InventoryItem item, PartyMember member)
        {
            string form = item.Form ?? "";
            if ((member.Role == "ember" || member.Role == "hex" || member.Role == "mender") && (form.Contains("plate") || form.Contains("chain") || form.Contains("tower"))) return 3;
            if ((member.Role == "shield" || member.Role == "ward") && (form.Contains("plate") || form.Contains("shield"))) return -2;
            return 0;
        }

        private int ArmorDefenseBonus(InventoryItem item)
        {
            if (item == null) return 0;
            int bonus = Mathf.Max(-1, item.Bonus);
            string form = (item.Form ?? "").ToLowerInvariant();
            if (form.Contains("plate") || form.Contains("tower")) bonus += 2;
            else if (form.Contains("chain") || form.Contains("mail") || form.Contains("scale") || form.Contains("kite")) bonus += 1;
            if (form.Contains("robe") || form.Contains("mantle") || form.Contains("cloak")) bonus = Mathf.Max(0, bonus);
            return Mathf.Clamp(bonus, -1, 7);
        }

        private string ItemBehaviorLine(InventoryItem item, PartyMember target)
        {
            if (item == null) return "";
            List<string> notes = new List<string>();
            string text = ((item.DisplayName ?? "") + " " + (item.Form ?? "") + " " + (item.Trait ?? "")).ToLowerInvariant();
            if (InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form))
            {
                int range = WeaponRange(item, target);
                if (range >= 4) notes.Add("long range");
                else if (range == 3) notes.Add("ranged");
                else if (range == 2) notes.Add("reach");
                string status = GearOnHitStatus(text);
                if (!string.IsNullOrEmpty(status)) notes.Add(status + " chance");
                if (GearLifeDrainAmount(item.DisplayName, Mathf.Max(1, item.DamageMax)) > 0) notes.Add("life drain");
                if (!string.IsNullOrEmpty(item.DamageType) && item.DamageType != "physical") notes.Add(item.DamageType + " affinity");
                if (text.Contains("focus") || text.Contains("orb") || text.Contains("scepter")) notes.Add("spell focus");
                if (text.Contains("unfathomable darkness")) notes.Add("mild vorpal edge");
            }
            else
            {
                if (text.Contains("plate") || text.Contains("tower") || text.Contains("chain") || text.Contains("mail")) notes.Add("heavy guard");
                if (text.Contains("leather") || text.Contains("cloak") || text.Contains("mantle")) notes.Add("light movement");
                if (text.Contains("warding") || text.Contains("anti-magic") || text.Contains("robe")) notes.Add("warding");
                if (text.Contains("thorns")) notes.Add("thorn guard");
            }
            return notes.Count == 0 ? "It is a clean upgrade." : "Why: " + string.Join(", ", notes) + ".";
        }

        private void StartCombat(string style)
        {
            StartCombat(EncounterCatalog.IdForLegacyStyle(style));
        }

        private void StartCombat(EncounterId id)
        {
            StartCombat(EncounterCatalog.For(id));
        }

        private void StartCombat(EncounterDefinition encounter)
        {
            if (encounter == null) throw new ArgumentNullException(nameof(encounter));
            CancelCombatResolutionBeat(false);
            combatUnitPresentationBeats.Clear();
            CloseTransientOverlays();
            state.Mode = GameMode.Combat;
            betaLabMode = encounter.DevelopmentOnly;
            CombatState combat = new CombatState
            {
                Round = 1,
                EncounterStyle = encounter.LegacyStyle ?? "",
                ActiveId = "",
                Moved = false,
                Acted = false,
                MovePoints = 0,
                ActionAvailable = false,
                Phase = CombatPhase.ChooseAction,
                Units = new List<CombatUnit>(),
                Obstacles = new List<Point>()
            };
            List<PartyMember> living = state.Party.Where(p => p.Hp > 0).ToList();
            for (int i = 0; i < living.Count; i++)
            {
                PartyMember p = living[i];
                combat.Units.Add(new CombatUnit
                {
                    Id = p.Id,
                    PartyIndex = state.Party.IndexOf(p),
                    Side = UnitSide.Party,
                    Name = p.Name,
                    Role = p.Role,
                    Race = p.Race,
                    ClassKey = p.ClassKey,
                    Origin = p.Origin,
                    Sigil = p.Sigil,
                    X = i < 4 ? 1 : 2,
                    Y = i < 4 ? i * 2 : (i - 4) * 2 + 1,
                    Hp = p.Hp,
                    MaxHp = p.MaxHp,
                    Level = p.Level,
                    Mana = p.Mana,
                    MaxMana = p.MaxMana,
                    Movement = p.Movement,
                    Power = p.Power,
                    Defense = p.Defense,
                    Agility = p.Agility,
                    Range = p.Range,
                    AttackSpeed = p.AttackSpeed,
                    DamageMin = p.DamageMin,
                    DamageMax = p.DamageMax,
                    Spell = p.Spell,
                    Skills = p.Skills.Clone(),
                    Color = MemberColor(p).ToHex(),
                    DamageType = string.IsNullOrEmpty(p.WeaponDamageType) ? "physical" : p.WeaponDamageType,
                    WeaponName = p.WeaponName,
                    WeaponBonus = p.WeaponBonus,
                    ArmorName = p.ArmorName,
                    ArmorBonus = p.ArmorBonus
                });
            }
            string[] kinds = encounter.UsesGeneratedEnemyPool
                ? EnemyPoolForDepth(state.Depth, ZoneAt(state.PlayerX, state.PlayerY)?.Id)
                : encounter.EnemyIds ?? Array.Empty<string>();
            int count = encounter.EnemyCountForDepth(state.Depth);
            for (int i = 0; i < count; i++)
            {
                string kind = encounter.UsesGeneratedEnemyPool ? kinds[rng.Next(kinds.Length)] : kinds[i % kinds.Length];
                combat.Units.Add(MakeEnemy(kind, i));
            }
            ApplyEncounterPlacements(combat, encounter);
            ApplyEncounterObstacles(combat, encounter);
            for (int i = 0; i < encounter.RandomObstacleCount; i++)
            {
                Point p = new Point(rng.Next(4, 9), rng.Next(1, CombatH - 1));
                if (!combat.Obstacles.Any(o => o.X == p.X && o.Y == p.Y)) combat.Obstacles.Add(p);
            }
            RebuildInitiativeQueue(combat);
            state.Combat = combat;
            InvalidateCombatController();
            selectedAction = ActionMode.Attack;
            PushLog(encounter.Intro, Tone.Warn);
            ShowBanner(encounter.Banner);
            PlaySfx("encounter");
            NextTurn();
        }

        private void ApplyEncounterPlacements(CombatState combat, EncounterDefinition encounter)
        {
            if (combat?.Units == null || encounter == null) return;
            if (encounter.BoostMartialLabParty) ApplyMartialLabPartySetup(combat, encounter.PartyPlacements);

            List<CombatUnit> enemies = combat.Units.Where(u => u.Side == UnitSide.Enemy).ToList();
            if (encounter.EnemyPlacements != null)
            {
                for (int i = 0; i < enemies.Count && i < encounter.EnemyPlacements.Length; i++)
                {
                    Point spot = encounter.EnemyPlacements[i];
                    enemies[i].X = spot.X;
                    enemies[i].Y = spot.Y;
                }
            }

            if (encounter.NormalizeKoboldKing)
            {
                CombatUnit king = enemies.FirstOrDefault(enemy => enemy.Role == "koboldking");
                if (king != null)
                {
                    king.Rank = "";
                    king.Name = "Varkh, Kobold King";
                }
            }

            if (encounter.WoundFirstEnemy && enemies.Count > 0)
            {
                CombatUnit enemy = enemies[0];
                enemy.Hp = Mathf.Max(1, Mathf.CeilToInt(enemy.MaxHp * 0.30f));
                enemy.Bleeding = Mathf.Max(enemy.Bleeding, 2);
                enemy.Name = "Wounded " + enemy.Name;
            }
        }

        private void ApplyMartialLabPartySetup(CombatState combat, Point[] placements)
        {
            if (combat?.Units == null) return;
            CombatUnit warrior = combat.Units.FirstOrDefault(u => u.Side == UnitSide.Party && u.ClassKey == "warrior");
            CombatUnit rogue = combat.Units.FirstOrDefault(u => u.Side == UnitSide.Party && u.ClassKey == "rogue");
            CombatUnit priest = combat.Units.FirstOrDefault(u => u.Side == UnitSide.Party && u.Role == "mender");
            CombatUnit caster = combat.Units.FirstOrDefault(u => u.Side == UnitSide.Party && u != warrior && u != rogue && u != priest);

            PlaceUnit(warrior, placements, 0);
            if (warrior != null)
            {
                warrior.Level = Mathf.Max(warrior.Level, 3);
                warrior.Skills.Arms = Mathf.Max(warrior.Skills.Arms, 18);
                warrior.Skills.Guard = Mathf.Max(warrior.Skills.Guard, 12);
            }

            PlaceUnit(rogue, placements, 1);
            if (rogue != null)
            {
                rogue.Level = Mathf.Max(rogue.Level, 3);
                rogue.Skills.Arms = Mathf.Max(rogue.Skills.Arms, 18);
                rogue.Skills.Missile = Mathf.Max(rogue.Skills.Missile, 10);
                rogue.Stealthed = Mathf.Max(rogue.Stealthed, 1);
            }

            PlaceUnit(priest, placements, 2);
            PlaceUnit(caster, placements, 3);
        }

        private void PlaceUnit(CombatUnit unit, Point[] placements, int index)
        {
            if (unit == null || placements == null || index < 0 || index >= placements.Length) return;
            unit.X = placements[index].X;
            unit.Y = placements[index].Y;
        }

        private void ApplyEncounterObstacles(CombatState combat, EncounterDefinition encounter)
        {
            if (combat?.Obstacles == null || encounter?.Obstacles == null) return;
            foreach (Point point in encounter.Obstacles)
            {
                if (point == null) continue;
                combat.Obstacles.Add(new Point(point.X, point.Y, point.Kind, point.Duration));
            }
        }

        private string[] EnemyPoolForDepth(int depth, string zoneId = "")
        {
            if (!ContentSetCatalog.IsFullPrototype(activeContentSet))
            {
                return ContentSetCatalog.SewerSliceEnemyIds.ToArray();
            }

            zoneId = zoneId ?? "";
            if (depth <= 1 && zoneId == "salt-cisterns") return new[] { "sewerrat", "sewerrat", "giantrat", "ratfolk", "ratcutthroat", "ratcleric", "spore", "koboldraider" };
            if (zoneId == "dusk-market") return new[] { "koboldraider", "koboldslinger", "koboldshield", "koboldshaman", "ratcutthroat", "drowscout", "drowcrossbow", "sentry", "mirearcher" };
            if (zoneId == "green-shrine-road") return new[] { "sewerrat", "giantrat", "ratcleric", "spore", "shade", "koboldshaman", "bonepriest", "drowpriest" };
            if (zoneId == "old-quarry") return new[] { "koboldshield", "sentry", "husk", "reaver", "koboldraider", "ratbrute", "thornbeast" };
            if (zoneId == "glass-warrens") return new[] { "adept", "glassmage", "drowmage", "drowcrossbow", "shade", "koboldwizard", "sentry", "cinderling" };
            if (zoneId == "ash-fen") return new[] { "spore", "mirearcher", "ratmage", "shade", "bonepriest", "koboldshaman", "giantrat" };
            if (zoneId == "red-gate") return new[] { "koboldwizard", "bonepriest", "drowblade", "drowpriest", "drowmage", "cinderling", "lesserdemon", "gloamknight", "reaver", "shade" };
            if (depth <= 1) return new[] { "sewerrat", "sewerrat", "giantrat", "ratfolk", "ratcutthroat", "koboldraider", "koboldslinger", "sentry" };
            if (depth == 2) return new[] { "koboldraider", "koboldslinger", "koboldshield", "koboldshaman", "koboldshaman", "ratmage", "ratcleric", "drowscout", "sentry", "adept", "husk", "reaver", "spore", "shade", "mirearcher", "bonepriest" };
            return new[] { "koboldraider", "koboldslinger", "koboldshield", "koboldshaman", "koboldwizard", "koboldwizard", "ratbrute", "drowscout", "drowblade", "drowcrossbow", "drowmage", "drowpriest", "lesserdemon", "sentry", "adept", "husk", "reaver", "spore", "shade", "glassmage", "thornbeast", "mirearcher", "bonepriest", "cinderling", "gloamknight" };
        }

        private CombatUnit MakeEnemy(string kind, int index, string forcedRank = null)
        {
            EnemyTemplate t = EnemyTemplate.For(kind);
            string rank = forcedRank ?? EnemyRankFor(kind, index);
            int rankBonus = rank == "elite" ? 2 : rank == "veteran" ? 1 : 0;
            string displayName = rank == "ritual" ? t.Name : RankEnemyName(t.Name, rank);
            return new CombatUnit
            {
                Id = Guid.NewGuid().ToString("N"),
                PartyIndex = -1,
                Side = UnitSide.Enemy,
                Name = displayName,
                Role = kind,
                Rank = rank,
                Origin = "ruins",
                Sigil = EnemySigil(kind),
                X = CombatW - 2 - (index % 2),
                Y = index % CombatH,
                Hp = t.Hp + state.Depth * 4 + rankBonus * 7,
                MaxHp = t.Hp + state.Depth * 4 + rankBonus * 7,
                Mana = 0,
                MaxMana = 0,
                Movement = CombatMoveAllowance,
                Power = t.Power + Mathf.FloorToInt(state.Depth * 1.4f) + rankBonus,
                Defense = t.Defense + state.Depth / 2 + (rank == "elite" ? 1 : 0),
                Agility = t.Agility + (rank == "veteran" && t.Range > 1 ? 1 : 0),
                Range = t.Range,
                AttackSpeed = Mathf.Clamp(8 + t.Agility + rankBonus, 5, 18),
                DamageMin = Mathf.Max(1, t.Power / 2 + rankBonus),
                DamageMax = Mathf.Max(2, t.Power + 3 + rankBonus * 2),
                Spell = "",
                Skills = new SkillSet().Normalize(),
                Color = RankColor(t.Color, rank),
                DamageType = t.DamageType,
                Resist = t.Resist,
                Weakness = t.Weakness,
                StatusOnHit = t.StatusOnHit,
                MagicResist = t.MagicResist + rankBonus,
                Fearless = t.Fearless || rank == "elite"
            };
        }

        private string EnemyRankFor(string kind, int index)
        {
            if (kind == "koboldking" || kind == "meteorlich" || kind == "ritualheart") return "";
            if (state.Depth < 2) return "";
            int roll = Mathf.Abs((state.Seed + state.Depth * 97 + index * 53 + StableSeed(kind)) % 100);
            int eliteChance = Mathf.Clamp(state.Depth * 4 - 3, 3, 18);
            int veteranChance = Mathf.Clamp(16 + state.Depth * 5, 18, 42);
            if (roll < eliteChance) return "elite";
            if (roll < veteranChance) return "veteran";
            return "";
        }

        private string RankEnemyName(string baseName, string rank)
        {
            if (rank == "elite") return "Marked " + baseName;
            if (rank == "veteran") return "Old " + baseName;
            return baseName;
        }

        private string RankColor(string baseColor, string rank)
        {
            Color color = baseColor.ToColor();
            if (rank == "elite") return Color.Lerp(color, gold, 0.30f).ToHex();
            if (rank == "veteran") return Color.Lerp(color, cursorWhite, 0.18f).ToHex();
            return baseColor;
        }

        private string EnemySigil(string kind)
        {
            switch (kind)
            {
                case "adept": return "eye";
                case "husk": return "diamond";
                case "reaver": return "flame";
                case "spore": return "leaf";
                case "shade": return "moon";
                case "glassmage": return "eye";
                case "thornbeast": return "chevron";
                case "mirearcher": return "bar";
                case "bonepriest": return "cross";
                case "cinderling": return "flame";
                case "gloamknight": return "diamond";
                case "koboldraider": return "chevron";
                case "koboldslinger": return "eye";
                case "koboldshaman": return "moon";
                case "koboldwizard": return "flame";
                case "koboldshield": return "diamond";
                case "koboldking": return "flame";
                case "sewerrat": return "bar";
                case "giantrat": return "chevron";
                case "ratfolk": return "bar";
                case "ratcutthroat": return "knife";
                case "ratmage": return "eye";
                case "ratcleric": return "cross";
                case "ratbrute": return "diamond";
                case "drowscout": return "eye";
                case "drowblade": return "moon";
                case "drowcrossbow": return "bar";
                case "drowmage": return "eye";
                case "drowpriest": return "cross";
                case "lesserdemon": return "flame";
                default: return "cross";
            }
        }

        private void HandleCombatTimers()
        {
            if (combatAdvancePending)
            {
                CompletePendingCombatAdvance();
                return;
            }
            CombatUnit active = CurrentUnit();
            if (active == null) return;
            if (active.Side == UnitSide.Enemy && aiActAt > 0 && Time.time >= aiActAt)
            {
                aiActAt = -1f;
                EnemyAct(active);
                FinishEnemyCombatAction(active);
            }
        }

        private void HandleCombatHotkeys()
        {
            if (IsCombatResolutionPending()) return;
            CombatUnit active = CurrentUnit();
            if (active == null || active.Side != UnitSide.Party) return;

            NormalizeCombatSelection(active);
            bool combatHudOwnsSelection = CombatHudOwnsCurrentSelection();
            if (Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.Backspace))
            {
                UndoActiveMovement();
                return;
            }
            if (CombatInputRoutingRules.ShouldRouteToWorld(combatHudOwnsSelection, CombatHotkeyKind.Navigation)
                && TryCombatDirectionalHotkey(active)) return;
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Z)) TryHotkeyAction(ActionMode.Move, active);
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.F)) TryHotkeyAction(ActionMode.Attack, active);
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.C)) TryHotkeyAction(PreferredThirdAction(active), active);
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) TryHotkeyAction(ActionMode.Wait, active);
            if (CombatInputRoutingRules.ShouldRouteToWorld(combatHudOwnsSelection, CombatHotkeyKind.Submit)
                && (Input.GetKeyDown(KeyCode.Space)
                    || Input.GetKeyDown(KeyCode.Return)
                    || Input.GetKeyDown(KeyCode.KeypadEnter)))
            {
                TryHotkeyAction(ActionMode.Wait, active);
            }
            if (Input.GetKeyDown(KeyCode.G)) TryHotkeyAction(ActionMode.Guard, active);
            if (Input.GetKeyDown(KeyCode.H)) TryHotkeyAction(ActionMode.Elixir, active);
        }

        private bool CombatHudOwnsCurrentSelection()
        {
            UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.EventSystems.EventSystem.current;
            return combatHudScreen != null
                && eventSystem != null
                && combatHudScreen.OwnsSelection(eventSystem.currentSelectedGameObject);
        }

        private bool TryCombatDirectionalHotkey(CombatUnit active)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) return TryCombatStep(active, 0, -1);
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) return TryCombatStep(active, 0, 1);
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) return TryCombatStep(active, -1, 0);
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) return TryCombatStep(active, 1, 0);
            return false;
        }

        private bool TryCombatStep(CombatUnit active, int dx, int dy)
        {
            if (active == null) return false;
            ActionMode armedMode = !string.IsNullOrEmpty(pendingFormulaCode)
                ? ActionMode.Cast
                : !string.IsNullOrEmpty(pendingAbilityId)
                    ? ActionMode.Ability
                    : ActionMode.Move;
            selectedAction = ActionMode.Move;
            if (!ActionEnabled(ActionMode.Move, active))
            {
                selectedAction = armedMode;
                PushLog(active.Webbed > 0 ? $"{active.Name} is webbed and cannot move." : "No movement remains.", Tone.Warn);
                PlaySfx("blocked", 0.62f);
                return true;
            }

            int beforeX = active.X;
            int beforeY = active.Y;
            MoveActiveTo(active, active.X + dx, active.Y + dy);
            bool moved = active.X != beforeX || active.Y != beforeY;
            if (moved && armedMode == ActionMode.Cast && !string.IsNullOrEmpty(pendingFormulaCode))
            {
                FormulaDef armedFormula = GetFormula(pendingFormulaCode);
                CombatActionCard movedFormulaCard = FormulaActionCard(armedFormula, active);
                if (movedFormulaCard == null || !movedFormulaCard.Usable)
                {
                    string formulaName = armedFormula?.Name ?? "The spell";
                    string reason = movedFormulaCard?.DisabledReason;
                    if (string.IsNullOrWhiteSpace(reason)) reason = "its requirements changed";
                    ClearFormulaEntry();
                    selectedAction = ActionMode.Move;
                    if (state?.Combat != null) state.Combat.Phase = CombatPhase.ChooseTarget;
                    PushLog($"{formulaName} is no longer armed after moving: {reason} The action and mana remain ready.", Tone.Warn);
                    PlaySfx("blocked", 0.62f);
                    MarkUiDirty();
                    return true;
                }
            }
            if ((armedMode == ActionMode.Cast && !string.IsNullOrEmpty(pendingFormulaCode))
                || (armedMode == ActionMode.Ability && !string.IsNullOrEmpty(pendingAbilityId)))
            {
                selectedAction = armedMode;
                if (state?.Combat != null) state.Combat.Phase = CombatPhase.ChooseTarget;
            }
            return true;
        }

        private void TryHotkeyAction(ActionMode mode, CombatUnit active)
        {
            if (!CombatCommandEnabled(mode, active))
            {
                PushLog(DisabledActionReason(mode, active, true), Tone.Warn);
                PlaySfx("blocked", 0.6f);
                return;
            }

            SelectOrRunAction(mode, active);
        }

        private bool PrepareFormulaCode(CombatUnit active, string code)
        {
            if (active == null || string.IsNullOrWhiteSpace(code)) return false;
            FormulaDef formula = GetFormula(code);
            CombatActionCard card = FormulaActionCard(formula, active);
            if (card == null || !card.Usable)
            {
                string reason = card?.DisabledReason;
                if (string.IsNullOrWhiteSpace(reason)) reason = $"{code} is unavailable.";
                PushLog(reason, Tone.Warn);
                PlaySfx("blocked", 0.7f);
                return false;
            }
            if (card.Targeted && CountLegalFormulaTargets(formula, active) <= 0)
            {
                PushLog(FormulaNoTargetReason(formula, active), Tone.Warn);
                PlaySfx("blocked", 0.7f);
                return false;
            }
            pendingFormulaCode = formula.Code;
            spellbookSelectedCode = formula.Code;
            RememberCombatAbilityBrowseSelection(active, true, formula.Code);
            selectedAction = ActionMode.Cast;
            showSpellbook = false;
            SuppressBoardPointer();
            if (state?.Combat != null) state.Combat.Phase = CombatPhase.ChooseTarget;
            PushLog($"{active.Name} readies {formula.Name}. Choose the target.", Tone.Good);
            ShowBanner(formula.Name);
            PlaySfx("formula", 0.95f);
            return true;
        }

        private bool PrepareAbility(CombatUnit active, string abilityId)
        {
            MartialAbility ability = AbilityDef(abilityId);
            CombatActionCard card = AbilityActionCard(ability, active);
            if (card == null || !card.Usable)
            {
                string reason = card?.DisabledReason;
                if (string.IsNullOrWhiteSpace(reason)) reason = $"{abilityId} is unavailable.";
                PushLog(reason, Tone.Warn);
                PlaySfx("blocked", 0.62f);
                return false;
            }
            if (ability.Targeted && CountLegalAbilityTargets(ability, active) <= 0)
            {
                PushLog(AbilityNoTargetReason(ability, active), Tone.Warn);
                PlaySfx("blocked", 0.62f);
                return false;
            }
            pendingAbilityId = ability.Id;
            abilitySelectedId = ability.Id;
            RememberCombatAbilityBrowseSelection(active, false, ability.Id);
            selectedAction = ActionMode.Ability;
            showAbilityPanel = false;
            showSpellbook = false;
            SuppressBoardPointer();
            if (state?.Combat != null) state.Combat.Phase = CombatPhase.ChooseTarget;
            PushLog($"{active.Name} readies {ability.Name}. Choose the target.", Tone.Good);
            ShowBanner(ability.Name);
            PlaySfx("ui", 0.62f);
            return true;
        }

        private void ClearFormulaEntry()
        {
            pendingFormulaCode = "";
        }

        private void ClearAbilityEntry()
        {
            pendingAbilityId = "";
        }

        private bool CanCancelCombatTargeting()
        {
            return CombatTargetingRules.CanCancel(
                state?.Combat,
                selectedAction,
                pendingFormulaCode,
                pendingAbilityId);
        }

        private bool CancelCombatTargeting()
        {
            if (!CanCancelCombatTargeting()) return false;

            string targetName = selectedAction == ActionMode.Cast
                ? GetFormula(pendingFormulaCode)?.Name
                : AbilityDef(pendingAbilityId)?.Name;
            ClearFormulaEntry();
            ClearAbilityEntry();
            showSpellbook = false;
            showAbilityPanel = false;
            selectedAction = ActionMode.Attack;
            state.Combat.Phase = CombatPhase.ChooseAction;
            PushLog($"{(string.IsNullOrWhiteSpace(targetName) ? "Targeting" : targetName)} canceled. The action remains ready.", Tone.Normal);
            PlaySfx("ui", 0.45f);
            SuppressBoardPointer();
            MarkUiDirty();
            return true;
        }

        private void NextTurn()
        {
            CancelCombatResolutionBeat(false);
            if (state.Combat == null) return;
            aiActAt = -1f;
            ClearFormulaEntry();
            ClearAbilityEntry();
            showAbilityPanel = false;
            RepairCombatSummons(false);
            ApplySummonerDeathToPactSummons();
            CombatOutcome outcome = CombatLifecycle().CurrentOutcome();
            if (outcome == CombatOutcome.Defeat)
            {
                SyncPartyFromCombat();
                state.Mode = GameMode.Defeat;
                combatUnitPresentationBeats.Clear();
                state.Combat = null;
                InvalidateCombatController();
                betaLabMode = false;
                PushLog("The party falls. A new oath may yet be sworn.", Tone.Warn);
                ShowBanner("Party defeated");
                PlaySfx("defeat");
                return;
            }
            if (outcome == CombatOutcome.Victory)
            {
                FinishCombat();
                return;
            }

            bool newRound = false;
            CombatUnit active = NextQueuedCombatUnit(out newRound);
            if (active == null) return;
            if (newRound)
            {
                CombatRoundPresentationSummary summary = TickCombatRoundState();
                QueueCombatRoundTransition(active, summary);
                return;
            }

            BeginQueuedCombatTurn(active);
        }

        private void BeginQueuedCombatTurn(CombatUnit active)
        {
            if (state?.Combat == null || active == null || active.Hp <= 0)
            {
                NextTurn();
                return;
            }
            CombatLifecycle().BeginTurn(active, active.Side == UnitSide.Enemy);
            selectedAction = ActionMode.Attack;
            ShowBanner(active.Side == UnitSide.Party ? active.Name + "'s turn" : active.Name + " moves");
            bool skipped = ApplyStartTurnEffects(active);
            if (active.Hp <= 0)
            {
                PushLog(active.Summoned ? $"{active.Name} loses its binding." : $"{active.Name} is down.", active.Side == UnitSide.Enemy ? Tone.Good : Tone.Warn);
                float defeatHold = state != null && !state.ReducedMotion
                    ? CombatUnitPresentationRules.RemainingHoldDuration(combatUnitPresentationBeats, Time.time)
                    : 0f;
                if (defeatHold > 0f)
                {
                    QueueCombatAdvance(active, defeatHold, "fall");
                    return;
                }
                NextTurn();
                return;
            }
            if (skipped)
            {
                FinishCombatAction(active, true);
                return;
            }
            if (active.Side == UnitSide.Party) PlaySfx("turn", 0.48f);
            if (active.Side == UnitSide.Enemy)
            {
                aiActAt = Time.time + (state.ReducedMotion ? 0.05f : 0.45f);
            }
        }

        private void QueueCombatRoundTransition(
            CombatUnit reservedUnit,
            CombatRoundPresentationSummary summary)
        {
            if (state?.Combat == null || reservedUnit == null)
            {
                BeginQueuedCombatTurn(reservedUnit);
                return;
            }

            combatAdvancePending = true;
            combatAdvanceAt = Time.time + Mathf.Clamp(summary.DurationSeconds, 0.01f, 0.85f);
            combatAdvanceUnitId = reservedUnit.Id ?? "";
            combatAdvanceStartsReservedTurn = true;
            combatResolutionLabel = "Round " + summary.Round;
            state.Combat.ActiveId = reservedUnit.Id;
            state.Combat.Moved = false;
            state.Combat.Acted = false;
            state.Combat.MovePoints = 0;
            state.Combat.ActionAvailable = false;
            state.Combat.Phase = CombatPhase.Resolving;
            aiActAt = -1f;
            ShowBanner(summary.BannerText);
            MarkUiDirty();
        }

        private void FinishCombat()
        {
            combatUnitPresentationBeats.Clear();
            string encounterStyle = state.Combat?.EncounterStyle ?? "";
            string roamingThreatId = state.Combat?.RoamingThreatId ?? "";
            bool finalBattle = IsFinalBossCombat();
            bool koboldKingBattle = encounterStyle == "koboldking" || IsKoboldKingCombat();
            int xp = CombatExperienceReward();
            SyncPartyFromCombat();
            AdvanceTemporaryWeaponEnchantmentsAfterVictory();
            int foundGold = rng.Next(18, 39) + state.Depth * 6;
            int foundElixirs = 0;
            state.Gold += foundGold;
            if (rng.NextDouble() < 0.6)
            {
                state.Elixirs++;
                foundElixirs = 1;
            }
            AwardExperience(xp);
            if (finalBattle)
            {
                FinishCampaignVictory(foundGold, xp);
                return;
            }
            if (koboldKingBattle)
            {
                FinishKoboldKingVictory(foundGold, xp);
                return;
            }
            InventoryItem battleLoot = ContentSetCatalog.IsSewerSliceEncounterStyle(encounterStyle) ? null : MakeCombatLootItem(encounterStyle, false);
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvalidateCombatController();
            betaLabMode = false;
            showSpellbook = false;
            showAbilityPanel = false;
            ResolveRoamingThreatVictory(roamingThreatId);
            ApplyKoboldStoryVictory(encounterStyle);
            ApplyMidgaardStoryVictory(encounterStyle);
            if (battleLoot != null)
            {
                EnsureInventoryList();
                state.Inventory.Add(battleLoot);
                string equipNote = AutoEquipItem(battleLoot);
                ShowLootPanel(battleLoot, foundGold, 0, foundElixirs, string.IsNullOrEmpty(equipNote) ? "Recovered from the defeated mob." : equipNote, "Battle loot");
                PushLog($"The field is won. {foundGold} gold, {xp} XP, and {battleLoot.DisplayName} recovered.", Tone.Good);
            }
            else
            {
                PushLog($"The field is won. {foundGold} gold and {xp} XP recovered{(foundElixirs > 0 ? ", plus an elixir" : "")}.", Tone.Good);
            }
            AutosaveCheckpoint(string.IsNullOrWhiteSpace(encounterStyle) ? "combat victory" : encounterStyle + " cleared");
            ShowBanner("Victory");
            PlaySfx("victory");
        }

        private bool IsFinalBossCombat()
        {
            if (state?.Combat?.Units == null) return false;
            if (state.Depth < FinalBossDepth) return false;
            return state.Combat.Units.Any(u => u.Side == UnitSide.Enemy && (u.Role == "meteorlich" || u.Role == "ritualheart"));
        }

        private bool IsKoboldKingCombat()
        {
            return state?.Combat?.Units != null && state.Combat.Units.Any(u => u.Side == UnitSide.Enemy && u.Role == "koboldking");
        }

        private bool IsKoboldRouteCombat()
        {
            string encounterStyle = state?.Combat?.EncounterStyle ?? "";
            return encounterStyle == "koboldambush" || encounterStyle == "koboldcave" || encounterStyle == "koboldking" || IsKoboldKingCombat();
        }

        private void ApplyKoboldStoryVictory(string encounterStyle)
        {
            if (!ContentSetCatalog.AllowKoboldChapter(activeContentSet, state?.StoryFlags)) return;
            if (encounterStyle == "koboldambush")
            {
                SetStoryFlag(StoryFlags.KoboldAmbushSurvived);
                state.ActiveStory = "Chapter II: Kobold Smoke. Find the cave mouth behind the Dusk Market bone charms.";
                PushLog("The ambush breaks. One fleeing kobold drops a cave charm wrapped in smoke-stained cord.", Tone.Good);
                EnsureKoboldKingCaveMarker();
            }
            else if (encounterStyle == "koboldcave")
            {
                SetStoryFlag(StoryFlags.KoboldCaveCleared);
                state.ActiveStory = "Chapter II: Kobold Smoke. Return to the cave mouth and challenge the Kobold King's shield hall.";
                PushLog("The smoke cave falls quiet. Deeper drums mark the king's hall beyond the same cave mouth.", Tone.Good);
                EnsureKoboldKingCaveMarker();
            }
        }

        private void FinishKoboldKingVictory(int foundGold, int xp)
        {
            SetStoryFlag(StoryFlags.KoboldCaveFound);
            SetStoryFlag(StoryFlags.KoboldCaveCleared);
            SetStoryFlag(StoryFlags.KoboldKingDefeated);
            state.StoryChapter = Mathf.Max(state.StoryChapter, 3);
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvalidateCombatController();
            betaLabMode = false;
            showSpellbook = false;
            showAbilityPanel = false;

            int bonusGold = 35 + state.Depth * 7;
            state.Gold += bonusGold;
            state.Supplies += 2;
            state.Elixirs++;
            InventoryItem trophy = MakeSwordOfUnfathomableDarkness();
            EnsureInventoryList();
            state.Inventory.Add(trophy);
            string equipNote = AutoEquipItem(trophy);
            string swordNote = "Taken from a fallen adventurer in the king's hoard. " + (string.IsNullOrEmpty(equipNote) ? "It goes into the pack." : equipNote);
            ShowLootPanel(trophy, bonusGold, 2, 1, swordNote, "Kobold King's Hoard");

            MapObject cave = state.Map?.Objects?.FirstOrDefault(IsKoboldStoryCave);
            if (cave != null) RemoveObject(cave);

            state.ActiveStory = "Chapter II complete: The Dusk Market cave drums are silent. Find the next stair and push toward the Bone Road.";
            PushLog($"Varkh falls. The king's hoard adds {bonusGold} gold, supplies, an elixir, and the {trophy.DisplayName}.", Tone.Good);
            PushLog(state.ActiveStory, Tone.Good);
            ShowBanner("Kobold King Falls");
            PlaySfx("victory", 1.08f);
        }

        private void FinishCampaignVictory(int foundGold, int xp)
        {
            string encounterStyle = state.Combat?.EncounterStyle ?? "boss";
            InventoryItem relic = MakeBossLoot(encounterStyle);
            if (relic != null)
            {
                EnsureInventoryList();
                state.Inventory.Add(relic);
                string equipNote = AutoEquipItem(relic);
                ShowLootPanel(relic, foundGold, 0, 0, string.IsNullOrEmpty(equipNote) ? "A future art pass can give this final relic unique artwork." : equipNote, "Final Gate Relic");
            }
            state.Mode = GameMode.Victory;
            state.Combat = null;
            InvalidateCombatController();
            betaLabMode = false;
            showSpellbook = false;
            showAbilityPanel = false;
            state.ActiveStory = "Epilogue: The Old Road is sealed for now. Midgaard has one more dawn.";
            PushLog($"The final gate falls. {foundGold} gold and {xp} XP recovered.", Tone.Good);
            if (relic != null) PushLog($"A boss relic is recovered: {relic.DisplayName}.", Tone.Good);
            PushLog("Vhal Rakh's meteor crown breaks above the ritual heart. This beta route is complete.", Tone.Good);
            ShowBanner("The Old Road Is Sealed");
            PlaySfx("victory", 1.15f);
        }

        private int CombatExperienceReward()
        {
            if (state?.Combat?.Units == null) return 0;
            int reward = 0;
            foreach (CombatUnit enemy in state.Combat.Units.Where(u => u.Side == UnitSide.Enemy))
            {
                reward += Mathf.Max(6, enemy.MaxHp / 2 + enemy.Power * 2 + enemy.Defense * 3 + enemy.Range * 2);
                if (enemy.Rank == "veteran") reward += 12;
                if (enemy.Rank == "elite") reward += 28;
                if (IsCasterEnemy(enemy)) reward += 10;
            }
            return Mathf.Max(12, reward / Mathf.Max(1, state.Party.Count));
        }

        private void AwardWorldExperience(int amount, string reason)
        {
            if (amount <= 0) return;
            AwardExperience(amount);
            PushLog($"{reason}: {amount} XP.", Tone.Good);
        }

        private void AwardExperience(int amount)
        {
            if (state?.Party == null || amount <= 0) return;
            bool partyLeveled = false;
            foreach (PartyMember member in state.Party.Where(p => p.Hp > 0))
            {
                int oldLevel = Mathf.Max(1, member.Level);
                int oldMaxHp = member.MaxHp;
                int oldMaxMana = member.MaxMana;
                int oldStatPoints = member.StatPoints;
                int oldSkillPoints = member.SkillPoints;
                int gained = amount + RaceExperienceBonus(member, amount);
                member.Experience += gained;
                bool leveled = false;
                while (member.Experience >= ExperienceForNextLevel(member.Level))
                {
                    member.Experience -= ExperienceForNextLevel(member.Level);
                    member.Level++;
                    member.SkillPoints += 2;
                    member.StatPoints += member.Level % 2 == 0 ? 2 : 1;
                    leveled = true;
                }
                RecalculateMember(member);
                if (leveled)
                {
                    partyLeveled = true;
                    member.Hp = member.MaxHp;
                    member.Mana = member.MaxMana;
                    int hpGain = Mathf.Max(0, member.MaxHp - oldMaxHp);
                    int manaGain = Mathf.Max(0, member.MaxMana - oldMaxMana);
                    int statGain = Mathf.Max(0, member.StatPoints - oldStatPoints);
                    int skillGain = Mathf.Max(0, member.SkillPoints - oldSkillPoints);
                    string unlocks = LevelUnlockLine(member, oldLevel, member.Level);
                    PushLog($"{member.Name} reaches level {member.Level}: +{hpGain} HP{(manaGain > 0 ? $" / +{manaGain} MP" : "")}, +{statGain} stat point{(statGain == 1 ? "" : "s")}, +{skillGain} skill point{(skillGain == 1 ? "" : "s")}{unlocks}.", Tone.Good);
                    AddFloat(Mathf.Clamp(state.PlayerX, 0, CombatW - 1), Mathf.Clamp(state.PlayerY, 0, CombatH - 1), "level", gold);
                }
            }
            if (partyLeveled) PlaySfx("levelup", 0.84f);
        }

        private int RaceExperienceBonus(PartyMember member, int amount)
        {
            return string.Equals(member?.Race, "human", StringComparison.OrdinalIgnoreCase) ? Mathf.Max(1, amount / 10) : 0;
        }

        private string LevelUnlockLine(PartyMember member, int oldLevel, int newLevel)
        {
            List<string> parts = new List<string>();
            List<string> abilities = UnlockedAbilityNames(member, oldLevel, newLevel).ToList();
            List<string> formulas = UnlockedFormulaNames(member, oldLevel, newLevel).ToList();
            if (abilities.Count > 0) parts.Add("abilities: " + string.Join(", ", abilities.Take(2)) + (abilities.Count > 2 ? $" +{abilities.Count - 2}" : ""));
            if (formulas.Count > 0) parts.Add("spells: " + string.Join(", ", formulas.Take(3)) + (formulas.Count > 3 ? $" +{formulas.Count - 3}" : ""));
            return parts.Count == 0 ? "" : " / unlocks " + string.Join(" / ", parts);
        }

        private IEnumerable<string> UnlockedAbilityNames(PartyMember member, int oldLevel, int newLevel)
        {
            if (member == null) yield break;
            foreach (string id in AbilityIdsForClass(member.ClassKey))
            {
                MartialAbility ability = AbilityDef(id);
                if (ability != null && ability.RequiredLevel > oldLevel && ability.RequiredLevel <= newLevel) yield return ability.Name;
            }
        }

        private IEnumerable<string> UnlockedFormulaNames(PartyMember member, int oldLevel, int newLevel)
        {
            if (member == null || string.IsNullOrEmpty(member.Spell)) yield break;
            foreach (FormulaDef formula in ActiveFormulaBook())
            {
                int required = FormulaRequiredLevel(formula);
                if (required > oldLevel && required <= newLevel && SchoolMatches(formula, member.Spell)) yield return formula.Name;
            }
        }

        private IEnumerable<string> AbilityIdsForClass(string classKey)
        {
            return AbilityCatalog.IdsForClass(classKey).Where(id => ContentSetCatalog.AbilityActive(activeContentSet, id));
        }

        private bool ApplyStartTurnEffects(CombatUnit active)
        {
            if (active.Poisoned > 0)
            {
                DealDamage(active, 3 + state.Depth / 2, "poison", poison);
                active.Poisoned = Mathf.Max(0, active.Poisoned - 1);
                PlaySfx("poison", 0.55f);
            }
            if (active.Bleeding > 0)
            {
                DealDamage(active, 2, "physical", blood);
                active.Bleeding = Mathf.Max(0, active.Bleeding - 1);
            }
            if (active.Regenerating > 0 && active.Hp > 0)
            {
                int heal = 4 + state.Depth;
                active.Hp = Mathf.Min(active.MaxHp, active.Hp + heal);
                AddFloat(active.X, active.Y, "+" + heal, teal);
                active.Regenerating = Mathf.Max(0, active.Regenerating - 1);
            }
            if (active.Hexed > 0)
            {
                active.Hexed = Mathf.Max(0, active.Hexed - 1);
                if (active.Hexed > 0) AddFloat(active.X, active.Y, "hexed", violet);
            }
            if (active.Stealthed > 0)
            {
                active.Stealthed = Mathf.Max(0, active.Stealthed - 1);
                if (active.Stealthed > 0) AddFloat(active.X, active.Y, "hidden", teal);
            }
            if (active.DemonFormTurns > 0)
            {
                active.DemonFormTurns = Mathf.Max(0, active.DemonFormTurns - 1);
                if (active.DemonFormTurns > 0)
                {
                    AddFloat(active.X, active.Y, "demon " + active.DemonFormTurns, blood);
                }
                else
                {
                    MartialAbility pendingDemonAbility = AbilityDef(pendingAbilityId);
                    if (pendingDemonAbility != null && pendingDemonAbility.ClassKey == "demon") ClearAbilityEntry();
                    MartialAbility selectedDemonAbility = AbilityDef(abilitySelectedId);
                    if (selectedDemonAbility != null && selectedDemonAbility.ClassKey == "demon") abilitySelectedId = "";
                    AddFloat(active.X, active.Y, "mortal", muted);
                    AddBurst(active.X, active.Y, violet);
                    PushLog($"{active.Name}'s abyssal shape burns away.", Tone.Warn);
                }
            }

            // Expire effects that were already active before this turn, then let
            // the current terrain grant a fresh duration that survives the turn.
            if (active.Shielded > 0) active.Shielded = Mathf.Max(0, active.Shielded - 1);
            if (active.Webbed > 0) active.Webbed = Mathf.Max(0, active.Webbed - 1);

            Point terrain = ObstacleAt(active.X, active.Y);
            if (terrain != null && active.Hp > 0)
            {
                if (terrain.Kind == "fire")
                {
                    DealDamage(active, 4 + state.Depth, "fire", ember);
                    if (active.Webbed > 0)
                    {
                        active.Webbed = 0;
                        AddFloat(active.X, active.Y, "web burns", ember);
                    }
                }
                else if (terrain.Kind == "gas")
                {
                    DealDamage(active, 1 + state.Depth / 2, "poison", poison);
                    TryApplyStatus(active, "poison", 2, null, 1f, false);
                }
                else if (terrain.Kind == "web")
                {
                    active.Webbed = Mathf.Max(active.Webbed, 2);
                    AddFloat(active.X, active.Y, "webbed", poison);
                }
                else if (terrain.Kind == "ice" && rng.NextDouble() < 0.35)
                {
                    TryApplyStatus(active, "stun", 1, null, 1f, false);
                }
                else if (terrain.Kind == "sanctuary")
                {
                    if (active.Side == UnitSide.Party)
                    {
                        int heal = 3 + state.Depth / 2;
                        active.Hp = Mathf.Min(active.MaxHp, active.Hp + heal);
                        active.Shielded = Mathf.Max(active.Shielded, 1);
                        int afflictions = active.Poisoned + active.Bleeding + active.Hexed;
                        active.Poisoned = Mathf.Max(0, active.Poisoned - 1);
                        active.Bleeding = Mathf.Max(0, active.Bleeding - 1);
                        active.Hexed = Mathf.Max(0, active.Hexed - 1);
                        AddFloat(active.X, active.Y, "+" + heal + " ward", teal);
                        if (afflictions > active.Poisoned + active.Bleeding + active.Hexed) AddFloat(active.X, active.Y, "cleansed", teal);
                        AddBurst(active.X, active.Y, teal);
                    }
                    else
                    {
                        DealDamage(active, 2 + state.Depth / 2, "light", teal);
                    }
                }
                else if (terrain.Kind == "curse")
                {
                    DealDamage(active, 1 + state.Depth / 2, "mind", violet);
                    if (active.Shielded > 0)
                    {
                        active.Shielded = Mathf.Max(0, active.Shielded - 1);
                        AddFloat(active.X, active.Y, "ward cracked", violet);
                    }
                    TryApplyStatus(active, "hex", 2, null, active.Side == UnitSide.Enemy ? 0.86f : 0.52f, true);
                }
                AddFieldActivationFeedback(active, terrain);
            }

            bool skip = active.Hp > 0 && (active.Stunned > 0 || active.Sleeping > 0);
            if (active.Hp > 0
                && state?.Combat != null
                && string.Equals(state.Combat.ActiveId, active.Id, StringComparison.Ordinal))
            {
                // BeginTurn snapshots action and movement before automatic effects.
                // Reconcile that budget while the resulting statuses are still
                // present so damage wakeups, fresh ice stuns, and web/fire changes
                // all affect this turn rather than the following one.
                CombatLifecycle().RepairActiveTurnState(active, active.Side == UnitSide.Enemy);
            }

            if (skip)
            {
                string status = active.Sleeping > 0 ? "sleeping" : "stunned";
                if (active.Sleeping > 0) active.Sleeping = Mathf.Max(0, active.Sleeping - 1);
                if (active.Stunned > 0) active.Stunned = Mathf.Max(0, active.Stunned - 1);
                PushLog($"{active.Name} is {status} and loses the turn.", Tone.Warn);
                AddFloat(active.X, active.Y, status, violet);
            }
            return skip;
        }

        private void AddFieldActivationFeedback(CombatUnit active, Point terrain)
        {
            if (active == null || terrain == null || !CombatFieldPresentationRules.IsPersistentField(terrain.Kind)) return;
            CombatFieldPresentationProfile profile = CombatFieldPresentationRules.For(terrain.Kind);
            Color color = ObstacleAccent(profile.Kind);
            AddBurst(active.X, active.Y, color);
            float pan = CombatAudioMixRules.StereoPanForColumn(active.X, CombatW);
            float pitch = CombatAudioMixRules.PitchForCue(profile.ActivationSfx, active.X);
            PlaySfxSpatial(profile.ActivationSfx, profile.Kind == "fire" || profile.Kind == "curse" ? 0.48f : 0.40f, pan, pitch);
        }

        private CombatRoundPresentationSummary TickCombatRoundState()
        {
            if (state?.Combat == null)
            {
                return CombatRoundPresentationRules.Create(1, 0, 0, state != null && state.ReducedMotion);
            }
            NormalizeCombatObstacles();
            int ritualsOpened;
            int expired = TickTimedTerrainRound(out ritualsOpened);
            if (expired > 0)
            {
                PushLog($"{expired} temporary field mark{(expired == 1 ? "" : "s")} fade as round {state.Combat.Round} begins.", Tone.Normal);
            }
            return CombatRoundPresentationRules.Create(
                state.Combat.Round,
                expired,
                ritualsOpened,
                state.ReducedMotion);
        }

        private int TickTimedTerrainRound(out int ritualsOpened)
        {
            ritualsOpened = 0;
            if (state?.Combat?.Obstacles == null) return 0;
            int expired = 0;
            foreach (Point hazard in state.Combat.Obstacles.ToList())
            {
                if (hazard.Duration <= 0) continue;
                hazard.Duration--;
                if (hazard.Duration > 0) continue;
                if (IsDisruptableRitual(hazard))
                {
                    if (TryOpenEnemyRitual(hazard))
                    {
                        ritualsOpened++;
                    }
                    else
                    {
                        hazard.Duration = 1;
                        AddFloat(hazard.X, hazard.Y, "BLOCKED", ObstacleAccent(hazard.Kind));
                        PushLog($"The {RitualName(hazard)} strains open, but no creature can cross onto the crowded field.", Tone.Warn);
                    }
                    continue;
                }
                state.Combat.Obstacles.Remove(hazard);
                expired++;
                Color color = TerrainHighlightColor(hazard, 0.9f);
                string label = IsBlockingTerrain(hazard) ? (hazard.Kind == "tree" ? "withers" : "crumbles") : "fades";
                AddFloat(hazard.X, hazard.Y, label, color);
                AddFlash(hazard.X, hazard.Y, color);
            }
            return expired;
        }

        private bool TryOpenEnemyRitual(Point ritual)
        {
            if (!IsDisruptableRitual(ritual) || state?.Combat?.Units == null) return false;
            Point spawn = FindRitualSpawnTile(ritual);
            string role = CombatRitualRules.SpawnRole(ritual.Kind);
            if (spawn == null || string.IsNullOrEmpty(role)) return false;

            CombatUnit enemy = MakeEnemy(role, state.Combat.Units.Count, "ritual");
            enemy.X = spawn.X;
            enemy.Y = spawn.Y;
            enemy.Origin = "ritual";
            enemy.Name = ritual.Kind == "demonrift" ? "Rift-born Lesser Demon" : "Glyph-born Kobold";
            state.Combat.Obstacles.Remove(ritual);
            state.Combat.Units.Add(enemy);
            if (state.Combat.InitiativeQueue == null) state.Combat.InitiativeQueue = new List<string>();
            if (!state.Combat.InitiativeQueue.Contains(enemy.Id)) state.Combat.InitiativeQueue.Add(enemy.Id);

            Color color = ObstacleAccent(ritual.Kind);
            AddTileGlyph(spawn.X, spawn.Y, null, ritual.Kind == "demonrift" ? "death" : "area", color);
            AddEpicBurst(spawn.X, spawn.Y, Color.Lerp(color, ritual.Kind == "demonrift" ? blood : gold, 0.42f), ritual.Kind == "demonrift" ? 28 : 20, ritual.Kind == "demonrift" ? 1.62f : 1.34f);
            AddFlash(spawn.X, spawn.Y, color);
            AddFloat(spawn.X, spawn.Y, "BREACH", color);
            PushLog(ritual.Kind == "demonrift"
                ? "The demon rift tears open. A lesser demon joins the enemy line."
                : "The summoning glyph cracks open. A kobold reinforcement joins the fight.", Tone.Warn);
            PlaySfx("death", ritual.Kind == "demonrift" ? 0.82f : 0.62f);
            PlaySfx("resonance", 0.48f);
            return true;
        }

        private Point FindRitualSpawnTile(Point ritual)
        {
            if (ritual == null) return null;
            int[][] offsets =
            {
                new[] { 0, 0 },
                new[] { 1, 0 }, new[] { -1, 0 }, new[] { 0, 1 }, new[] { 0, -1 },
                new[] { 1, 1 }, new[] { 1, -1 }, new[] { -1, 1 }, new[] { -1, -1 }
            };
            foreach (int[] offset in offsets)
            {
                int x = ritual.X + offset[0];
                int y = ritual.Y + offset[1];
                if (x < 0 || x >= CombatW || y < 0 || y >= CombatH || UnitAt(x, y) != null) continue;
                Point obstacle = ObstacleAt(x, y);
                if (obstacle != null && !ReferenceEquals(obstacle, ritual)) continue;
                return new Point(x, y);
            }
            return null;
        }

        private void EnemyAct(CombatUnit enemy)
        {
            ResetEnemyActionPresentation();
            state.Combat.Phase = CombatPhase.EnemyThinking;
            CombatUnit target = BestEnemyTarget(enemy);
            if (target == null) return;
            bool triedArcingSpecial = EnemySpecialArcsOverCover(enemy);
            if (triedArcingSpecial && TryEnemySpecial(enemy, target)) return;
            if (TryEnemyBreakCover(enemy, target, true)) return;
            if (!triedArcingSpecial && TryEnemySpecial(enemy, target)) return;
            int moveBudget = enemy.Webbed > 0 ? 0 : UnitMoveAllowance(enemy);
            int spentMove = 0;
            Vector2 start = new Vector2(enemy.X, enemy.Y);
            bool moved = false;

            if (moveBudget > 0 && CanEnemyAttack(enemy, target) && ShouldEnemyRepositionBeforeAttack(enemy, target))
            {
                Point destination = BestEnemyMoveDestination(enemy, target, Mathf.Min(1, moveBudget), out spentMove, true);
                if (destination != null && spentMove > 0)
                {
                    enemy.X = destination.X;
                    enemy.Y = destination.Y;
                    moved = true;
                    target = BestEnemyTarget(enemy) ?? target;
                }
            }
            else if (moveBudget > 0 && !CanEnemyAttack(enemy, target))
            {
                Point destination = BestEnemyMoveDestination(enemy, target, moveBudget, out spentMove, false);
                if (destination != null && spentMove > 0)
                {
                    enemy.X = destination.X;
                    enemy.Y = destination.Y;
                    moved = true;
                    target = BestEnemyTarget(enemy) ?? target;
                }
                else if (TryEnemyBreakCover(enemy, target))
                {
                    return;
                }
            }
            CombatLifecycle().ApplyEnemyMovementResult(moved, moveBudget, spentMove);
            if (moved)
            {
                AddTween(enemy.Id, start, new Vector2(enemy.X, enemy.Y), TweenKind.Move);
            }
            if (enemy.Webbed > 0)
            {
                PushLog($"{enemy.Name} strains against webbing.", Tone.Normal);
            }
            if (target != null && CanEnemyAttack(enemy, target) && EnemyCanAttackAfterMove(enemy, spentMove))
            {
                Attack(enemy, target);
            }
            else if (target != null && TryEnemyBreakCover(enemy, target))
            {
                return;
            }
            else
            {
                PushLog($"{enemy.Name} advances.", Tone.Normal);
            }
        }

        private void FinishEnemyCombatAction(CombatUnit active)
        {
            float delay = enemyActionResolutionDelay;
            string label = enemyActionResolutionLabel;
            ResetEnemyActionPresentation();
            FinishCombatAction(active, true, delay, label);
        }

        private bool EnemyCanAttackAfterMove(CombatUnit enemy, int spentMoveCost)
        {
            return EnemyTacticsRules.CanAttackAfterMove(enemy, spentMoveCost);
        }

        private bool ShouldEnemyRepositionBeforeAttack(CombatUnit enemy, CombatUnit target)
        {
            if (enemy == null || target == null || enemy.Webbed > 0) return false;
            int distance = Distance(enemy.X, enemy.Y, target.X, target.Y);
            Point currentTerrain = ObstacleAt(enemy.X, enemy.Y);
            return EnemyTacticsRules.ShouldRepositionBeforeAttack(enemy, distance, EnemyTerrainRiskScore(enemy, currentTerrain));
        }

        private Point BestEnemyMoveDestination(CombatUnit enemy, CombatUnit target, int moveBudget, out int moveCost, bool onlyIfImproves)
        {
            moveCost = 0;
            if (enemy == null || target == null || moveBudget <= 0) return null;

            int[,] reachable = ReachableMoveCosts(enemy, moveBudget);
            int currentScore = EnemyDestinationScore(enemy, target, enemy.X, enemy.Y, 0);
            Point best = null;
            int bestCost = 0;
            int bestScore = int.MaxValue;
            for (int y = 0; y < CombatH; y++)
            for (int x = 0; x < CombatW; x++)
            {
                int cost = reachable[x, y];
                if (cost <= 0 || cost > moveBudget || cost >= UnreachableMoveCost) continue;
                if (IsBlockingTerrain(ObstacleAt(x, y))) continue;
                CombatUnit occupant = UnitAt(x, y);
                if (occupant != null && occupant.Id != enemy.Id) continue;

                int score = EnemyDestinationScore(enemy, target, x, y, cost);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestCost = cost;
                    best = new Point(x, y, "move");
                }
            }

            if (best == null) return null;
            if (onlyIfImproves && bestScore >= currentScore - 8) return null;
            moveCost = bestCost;
            return best;
        }

        private int EnemyDestinationScore(CombatUnit enemy, CombatUnit target, int x, int y, int moveCost)
        {
            int distance = Distance(x, y, target.X, target.Y);
            int score = distance * 10 + moveCost * 3;
            bool canAttack = CanEnemyAttackFrom(enemy, target, x, y);
            if (canAttack)
            {
                score -= EnemyCanAttackAfterMove(enemy, moveCost) ? 92 : 38;
            }
            else if (enemy.Range > 1 && EnemySpecialArcsOverCover(enemy) && Distance(x, y, target.X, target.Y) <= enemy.Range)
            {
                score -= 18;
            }

            Point terrain = ObstacleAt(x, y);
            score += EnemyTerrainRiskScore(enemy, terrain);
            bool sight = enemy.Range <= 1 || HasLineOfSight(x, y, target.X, target.Y, true);
            score += EnemyTacticsRules.PositionAdjustment(enemy, distance, moveCost, sight, EnemySpecialArcsOverCover(enemy));

            if (target.Guarding) score += Mathf.Max(4, target.GuardBonus * 2);
            if (target.Stealthed > 0) score += 16 + target.Stealthed * 5;
            if (HasTag(target.Weakness, enemy.DamageType)) score -= 8;
            if (HasTag(target.Resist, enemy.DamageType)) score += 8;
            return score;
        }

        private int EnemyTerrainRiskScore(CombatUnit enemy, Point terrain)
        {
            if (terrain == null) return 0;
            return EnemyTacticsRules.TerrainRisk(enemy, terrain.Kind, TerrainMoveExtraCost(terrain, enemy));
        }

        private bool CanEnemyAttackFrom(CombatUnit enemy, CombatUnit target, int x, int y)
        {
            return AttackLegalityForecastFrom(enemy, target, x, y).Legal;
        }

        private bool TryEnemyBreakCover(CombatUnit enemy, CombatUnit target, bool urgentOnly = false)
        {
            Point cover = BestCoverToBreak(enemy, target, urgentOnly);
            if (cover == null) return false;

            int damage = CoverBreakDamage(enemy, cover);
            int before = CoverIntegrity(cover);
            cover.Integrity = Mathf.Max(0, before - damage);
            Color color = cover.Kind == "tree" ? moss : stone;
            int distance = Distance(enemy.X, enemy.Y, cover.X, cover.Y);
            bool arcing = distance > 1 && enemy.Range > 1 && EnemySpecialArcsOverCover(enemy);
            bool ranged = distance > 1 && enemy.Range > 1 && !arcing;
            bool broken = cover.Integrity <= 0;
            if (!ranged && !arcing)
            {
                AddTween(enemy.Id, new Vector2(enemy.X, enemy.Y), new Vector2(enemy.X + Mathf.Sign(cover.X - enemy.X) * 0.16f, enemy.Y + Mathf.Sign(cover.Y - enemy.Y) * 0.16f), TweenKind.Lunge);
            }
            StageCoverImpactFeedback(enemy, cover, color, broken, ranged, arcing);
            AddFloat(cover.X, cover.Y, broken ? "broken" : $"-{damage}", color);
            if (broken)
            {
                state.Combat.Obstacles.Remove(cover);
                PushLog($"{enemy.Name} breaks through the {CoverName(cover)}.", Tone.Warn);
            }
            else
            {
                string verb = distance > 1 && enemy.Range > 1 ? "pressures" : "batters";
                PushLog($"{enemy.Name} {verb} the {CoverName(cover)}. {cover.Integrity} integrity remains.", Tone.Warn);
            }
            PlayCoverAttackSequence(enemy, cover, ranged, broken, arcing);
            return true;
        }

        private Point BestCoverToBreak(CombatUnit enemy, CombatUnit target, bool urgentOnly = false)
        {
            if (enemy == null || target == null || state?.Combat?.Obstacles == null) return null;
            List<Point> candidates = new List<Point>
            {
                ObstacleAt(enemy.X + 1, enemy.Y),
                ObstacleAt(enemy.X - 1, enemy.Y),
                ObstacleAt(enemy.X, enemy.Y + 1),
                ObstacleAt(enemy.X, enemy.Y - 1)
            };

            if (enemy.Range > 1)
            {
                candidates.AddRange(BlockingCoverAlongLine(enemy.X, enemy.Y, target.X, target.Y)
                    .Where(o => Distance(enemy.X, enemy.Y, o.X, o.Y) <= enemy.Range && HasLineOfSight(enemy.X, enemy.Y, o.X, o.Y, true)));
            }

            IEnumerable<Point> covers = candidates
                .Where(IsBreakableCover)
                .GroupBy(o => $"{o.X},{o.Y}")
                .Select(g => g.First());
            if (urgentOnly) covers = covers.Where(o => CoverBlocksEnemyPressure(enemy, target, o));
            return covers
                .OrderBy(o => CoverBreakScore(enemy, target, o))
                .FirstOrDefault();
        }

        private bool CoverBlocksEnemyPressure(CombatUnit enemy, CombatUnit target, Point cover)
        {
            if (enemy == null || target == null || !IsBreakableCover(cover)) return false;
            if (BlockingCoverAlongLine(enemy.X, enemy.Y, target.X, target.Y).Any(o => o.X == cover.X && o.Y == cover.Y)) return true;
            int distance = Distance(enemy.X, enemy.Y, target.X, target.Y);
            int coverToTarget = Distance(cover.X, cover.Y, target.X, target.Y);
            if (enemy.Range > 1 && distance <= enemy.Range && !HasLineOfSight(enemy.X, enemy.Y, target.X, target.Y, true)) return true;
            if (coverToTarget <= 1 && distance <= Mathf.Max(2, enemy.Range + 2)) return true;
            if (enemy.Range <= 1 && coverToTarget < distance && !HasUsefulStepToward(enemy, target)) return true;
            return false;
        }

        private bool HasUsefulStepToward(CombatUnit mover, CombatUnit target)
        {
            if (mover == null || target == null) return false;
            int current = Distance(mover.X, mover.Y, target.X, target.Y);
            Point[] steps =
            {
                new Point(mover.X + 1, mover.Y),
                new Point(mover.X - 1, mover.Y),
                new Point(mover.X, mover.Y + 1),
                new Point(mover.X, mover.Y - 1)
            };
            return steps.Any(p => CanStandAt(p.X, p.Y) && Distance(p.X, p.Y, target.X, target.Y) < current);
        }

        private int CoverBreakScore(CombatUnit enemy, CombatUnit target, Point cover)
        {
            int score = Distance(cover.X, cover.Y, target.X, target.Y) * 10;
            score += Distance(enemy.X, enemy.Y, cover.X, cover.Y) * 4;
            if (CoverBlocksEnemyPressure(enemy, target, cover)) score -= 36;
            if (enemy.Range > 1 && !HasLineOfSight(enemy.X, enemy.Y, target.X, target.Y, true)) score -= 24;
            if (cover.Kind == "tree") score -= 8;
            score += CoverIntegrity(cover) * 2;
            return score;
        }

        private bool IsBreakableCover(Point cover)
        {
            return cover != null && (cover.Kind == "tree" || cover.Kind == "stone");
        }

        private bool IsDisruptableRitual(Point ritual)
        {
            return CombatRitualRules.IsRitual(ritual);
        }

        private int RitualIntegrity(Point ritual)
        {
            if (!IsDisruptableRitual(ritual)) return 0;
            return ritual.Integrity > 0 ? ritual.Integrity : CombatRitualRules.MaxIntegrity(ritual.Kind);
        }

        private string RitualName(Point ritual)
        {
            return CombatRitualRules.DisplayName(ritual?.Kind);
        }

        private int RitualDisruptionDamage(CombatUnit attacker, Point ritual)
        {
            if (attacker == null || !IsDisruptableRitual(ritual)) return 0;
            bool ranged = UsesRangedAttackAt(attacker, attacker.X, attacker.Y, ritual.X, ritual.Y);
            return CombatRitualRules.PhysicalDisruptionDamage(attacker, ranged);
        }

        private int CoverBreakDamage(CombatUnit enemy, Point cover)
        {
            int damage = 1;
            if (IsBruteEnemy(enemy) || enemy.Power >= 9) damage++;
            if (enemy.Range <= 1 && enemy.WeaponName != null && enemy.WeaponName.ToLowerInvariant().Contains("axe")) damage++;
            if (enemy.Role == "cinderling" && cover?.Kind == "tree") damage++;
            if (enemy.Rank == "elite") damage++;
            return Mathf.Clamp(damage, 1, 4);
        }

        private int CoverIntegrity(Point cover)
        {
            if (cover == null) return 0;
            return cover.Integrity > 0 ? cover.Integrity : CoverMaxIntegrity(cover.Kind);
        }

        private int CoverMaxIntegrity(string kind)
        {
            if (kind == "tree") return 2;
            if (kind == "stone") return 3;
            return 0;
        }

        private string CoverName(Point cover)
        {
            if (cover == null) return "cover";
            if (cover.Kind == "tree") return "tree cover";
            if (cover.Kind == "stone") return "stone block";
            return cover.Kind;
        }

        private bool TryEnemySpecial(CombatUnit enemy, CombatUnit target)
        {
            if (enemy == null || target == null || rng.NextDouble() > EnemySpecialChance(enemy)) return false;

            if (enemy.Role == "bonepriest" || enemy.Role == "ratcleric" || enemy.Role == "drowpriest")
            {
                CombatUnit ally = EnemySupportTarget(enemy);
                if (ally != null)
                {
                    EnemyPowerBeat beat = BeginEnemyPowerBeat(enemy, ally, "graveward", ally.X, ally.Y, teal);
                    int heal = 7 + state.Depth + EnemyRankBonus(enemy) * 3;
                    ally.Hp = Mathf.Min(ally.MaxHp, ally.Hp + heal);
                    ally.Shielded = Mathf.Max(ally.Shielded, 2);
                    AddBeam(enemy.X, enemy.Y, ally.X, ally.Y, teal, "heal");
                    AddFloat(ally.X, ally.Y, "+" + heal, teal);
                    AddBurst(ally.X, ally.Y, teal);
                    PushLog($"{enemy.Name} rattles a ward over {ally.Name}.", Tone.Warn);
                    CompleteEnemyPowerBeat(beat);
                    return true;
                }
            }

            if (enemy.Role == "koboldking")
            {
                return TryKoboldKingSpecial(enemy, target);
            }

            if (!CanEnemySpecialReach(enemy, target)) return false;

            if (enemy.Role == "koboldshaman")
            {
                EnemyPowerBeat beat = BeginEnemyPowerBeat(enemy, target, "bonehex", target.X, target.Y, violet);
                AddEnemySpecialBeam(enemy, target, violet, "hex");
                DealDamage(target, Mathf.Max(3, enemy.Power - 1 + EnemyRankBonus(enemy)), "mind", violet);
                TryApplyStatus(target, "hex", 3, enemy, 0.66f + EnemyRankBonus(enemy) * 0.08f, true);
                Point web = BestHazardTileNear(target, "web");
                if (web != null && rng.NextDouble() < 0.58) state.Combat.Obstacles.Add(web);
                if (rng.NextDouble() < 0.62) PlaceEnemySummonMark(enemy, target, "glyph", Hex("d9d3c4"), "glyph");
                PushLog($"{enemy.Name} rattles a bone hex at {target.Name}.", Tone.Warn);
                CompleteEnemyPowerBeat(beat);
                return true;
            }

            if (enemy.Role == "koboldwizard")
            {
                EnemyPowerBeat beat = BeginEnemyPowerBeat(enemy, target, "deathball", target.X, target.Y, blood);
                AddEnemySpecialBeam(enemy, target, blood, "death");
                int damage = Mathf.Max(5, enemy.Power + 1 + EnemyRankBonus(enemy) * 2);
                DealDamage(target, damage, "death", blood);
                foreach (CombatUnit unit in state.Combat.Units.Where(u => u.Side == UnitSide.Party && u.Hp > 0 && u.Id != target.Id && Distance(u.X, u.Y, target.X, target.Y) <= 1))
                {
                    DealDamage(unit, Mathf.Max(2, damage / 3), "death", blood);
                }
                TryApplyStatus(target, "hex", 2, enemy, 0.42f + EnemyRankBonus(enemy) * 0.08f, true);
                if (rng.NextDouble() < 0.58) PlaceEnemySummonMark(enemy, target, "demonrift", violet, "rift");
                PushLog($"{enemy.Name} looses a red-black death ball.", Tone.Warn);
                CompleteEnemyPowerBeat(beat);
                return true;
            }

            if (enemy.Role == "adept")
            {
                EnemyPowerBeat beat = BeginEnemyPowerBeat(enemy, target, "shocksign", target.X, target.Y, gold);
                AddEnemySpecialBeam(enemy, target, gold, "arc");
                DealDamage(target, Mathf.Max(3, enemy.Power - 1), "shock", gold);
                TryApplyStatus(target, "stun", 1, enemy, 0.34f + EnemyRankBonus(enemy) * 0.08f, true);
                PushLog($"{enemy.Name} snaps a shock sign at {target.Name}.", Tone.Warn);
                CompleteEnemyPowerBeat(beat);
                return true;
            }

            if (enemy.Role == "glassmage")
            {
                EnemyPowerBeat beat = BeginEnemyPowerBeat(enemy, target, "coldsplinter", target.X, target.Y, frost);
                AddEnemySpecialBeam(enemy, target, frost, "ice");
                DealDamage(target, Mathf.Max(3, enemy.Power), "cold", frost);
                Point ice = BestHazardTileNear(target, "ice");
                if (ice != null) state.Combat.Obstacles.Add(ice);
                PushLog($"{enemy.Name} splinters cold light across the floor.", Tone.Warn);
                CompleteEnemyPowerBeat(beat);
                return true;
            }

            if (enemy.Role == "ratmage")
            {
                EnemyPowerBeat beat = BeginEnemyPowerBeat(enemy, target, "plaguesigns", target.X, target.Y, poison);
                AddEnemySpecialBeam(enemy, target, poison, "hex");
                DealDamage(target, Mathf.Max(3, enemy.Power - 1), "poison", poison);
                TryApplyStatus(target, "poison", 2, enemy, 0.58f + EnemyRankBonus(enemy) * 0.08f, true);
                Point gas = BestHazardTileNear(target, "gas");
                if (gas != null && rng.NextDouble() < 0.52) state.Combat.Obstacles.Add(gas);
                PushLog($"{enemy.Name} hisses plague signs through the cistern air.", Tone.Warn);
                CompleteEnemyPowerBeat(beat);
                return true;
            }

            if (enemy.Role == "drowmage" || enemy.Role == "drowpriest")
            {
                EnemyPowerBeat beat = BeginEnemyPowerBeat(enemy, target, "darklight", target.X, target.Y, violet);
                AddEnemySpecialBeam(enemy, target, violet, "hex");
                DealDamage(target, Mathf.Max(4, enemy.Power), "mind", violet);
                TryApplyStatus(target, "hex", 2, enemy, 0.48f + EnemyRankBonus(enemy) * 0.08f, true);
                PushLog($"{enemy.Name} bends dark light around {target.Name}.", Tone.Warn);
                CompleteEnemyPowerBeat(beat);
                return true;
            }

            if (enemy.Role == "spore")
            {
                EnemyPowerBeat beat = BeginEnemyPowerBeat(enemy, target, "venomdust", target.X, target.Y, poison);
                AddEnemySpecialBeam(enemy, target, poison, "hex");
                DealDamage(target, Mathf.Max(2, enemy.Power - 2), "poison", poison);
                TryApplyStatus(target, "poison", 2, enemy, 0.55f + EnemyRankBonus(enemy) * 0.08f, true);
                Point gas = BestHazardTileNear(target, "gas");
                if (gas != null && rng.NextDouble() < 0.45) state.Combat.Obstacles.Add(gas);
                PushLog($"{enemy.Name} coughs venom dust toward {target.Name}.", Tone.Warn);
                CompleteEnemyPowerBeat(beat);
                return true;
            }

            if (enemy.Role == "cinderling")
            {
                EnemyPowerBeat beat = BeginEnemyPowerBeat(enemy, target, "cindertrail", target.X, target.Y, ember);
                AddEnemySpecialBeam(enemy, target, ember, "fire");
                DealDamage(target, Mathf.Max(4, enemy.Power), "fire", ember);
                Point fire = BestHazardTileNear(target, "fire");
                if (fire != null) state.Combat.Obstacles.Add(fire);
                PushLog($"{enemy.Name} spits a low cinder trail.", Tone.Warn);
                CompleteEnemyPowerBeat(beat);
                return true;
            }

            if (enemy.Role == "lesserdemon")
            {
                EnemyPowerBeat beat = BeginEnemyPowerBeat(enemy, target, "burningpact", target.X, target.Y, ember);
                AddEnemySpecialBeam(enemy, target, ember, "fire");
                DealDamage(target, Mathf.Max(5, enemy.Power + EnemyRankBonus(enemy)), "fire", ember);
                TryApplyStatus(target, "bleed", 2, enemy, 0.34f + EnemyRankBonus(enemy) * 0.08f, true);
                Point fire = BestHazardTileNear(target, "fire");
                if (fire != null && rng.NextDouble() < 0.45) state.Combat.Obstacles.Add(fire);
                PushLog($"{enemy.Name} claws a burning pact mark into the floor.", Tone.Warn);
                CompleteEnemyPowerBeat(beat);
                return true;
            }

            if (enemy.Role == "shade")
            {
                EnemyPowerBeat beat = BeginEnemyPowerBeat(enemy, target, "dreamveil", target.X, target.Y, violet);
                AddEnemySpecialBeam(enemy, target, violet, "hex");
                TryApplyStatus(target, "sleep", 1, enemy, 0.42f + EnemyRankBonus(enemy) * 0.08f, true);
                DealDamage(target, Mathf.Max(2, enemy.Power - 3), "mind", violet);
                PushLog($"{enemy.Name} dims the air around {target.Name}.", Tone.Warn);
                CompleteEnemyPowerBeat(beat);
                return true;
            }

            return false;
        }

        private CombatUnit EnemySupportTarget(CombatUnit enemy)
        {
            if (enemy == null || state?.Combat?.Units == null) return null;
            EnemyTacticsArchetype archetype = EnemyTacticsRules.For(enemy).Archetype;
            if (archetype != EnemyTacticsArchetype.Support) return null;
            return state.Combat.Units
                .Where(u => u.Side == UnitSide.Enemy && u.Hp > 0 && u.Hp < u.MaxHp && Distance(u.X, u.Y, enemy.X, enemy.Y) <= 4)
                .OrderBy(u => (float)u.Hp / Mathf.Max(1, u.MaxHp))
                .ThenBy(u => Distance(u.X, u.Y, enemy.X, enemy.Y))
                .FirstOrDefault();
        }

        private bool TryKoboldKingSpecial(CombatUnit king, CombatUnit target)
        {
            if (king == null || target == null || king.Hp <= 0) return false;
            bool bloodied = king.MaxHp > 0 && king.Hp * 2 <= king.MaxHp;
            if (KoboldKingShouldBuff(king) && (bloodied || rng.NextDouble() < 0.42))
            {
                return TryKoboldKingRally(king);
            }

            int partyCluster = state.Combat.Units.Count(u => u.Side == UnitSide.Party && u.Hp > 0 && Distance(u.X, u.Y, target.X, target.Y) <= 1);
            if (partyCluster >= 2 && rng.NextDouble() < (bloodied ? 0.58 : 0.42) && TryKoboldKingFireball(king, target))
            {
                return true;
            }

            if (rng.NextDouble() < (bloodied ? 0.44 : 0.34) && TryKoboldKingChargeAndRetreat(king, target))
            {
                return true;
            }

            if (rng.NextDouble() < 0.56 && TryKoboldKingIceLance(king, target))
            {
                return true;
            }

            if (TryKoboldKingRally(king)) return true;
            if (TryKoboldKingFireball(king, target)) return true;
            return TryKoboldKingIceLance(king, target);
        }

        private bool KoboldKingShouldBuff(CombatUnit king)
        {
            if (king == null || state?.Combat?.Units == null) return false;
            return state.Combat.Units.Any(u => u.Side == UnitSide.Enemy
                && u.Hp > 0
                && u.Id != king.Id
                && Distance(u.X, u.Y, king.X, king.Y) <= 5
                && (u.Shielded <= 0 || u.Hp * 2 < u.MaxHp));
        }

        private bool TryKoboldKingRally(CombatUnit king)
        {
            if (king == null || state?.Combat?.Units == null) return false;
            List<CombatUnit> allies = state.Combat.Units
                .Where(u => u.Side == UnitSide.Enemy && u.Hp > 0 && Distance(u.X, u.Y, king.X, king.Y) <= 5)
                .OrderBy(u => u.Id == king.Id ? 1 : 0)
                .ThenBy(u => u.Shielded > 0 ? 1 : 0)
                .ThenBy(u => (float)u.Hp / Mathf.Max(1, u.MaxHp))
                .Take(4)
                .ToList();
            if (allies.Count == 0) return false;

            CombatUnit focus = allies.FirstOrDefault(ally => ally.Id != king.Id) ?? king;
            EnemyPowerBeat beat = BeginEnemyPowerBeat(king, focus, "royalrally", king.X, king.Y, gold);

            foreach (CombatUnit ally in allies)
            {
                int heal = ally.Id == king.Id ? 3 : 4 + state.Depth / 2;
                if (ally.Hp < ally.MaxHp) ally.Hp = Mathf.Min(ally.MaxHp, ally.Hp + heal);
                ally.Shielded = Mathf.Max(ally.Shielded, ally.Id == king.Id ? 2 : 3);
                AddBeam(king.X, king.Y, ally.X, ally.Y, gold, "heal");
                AddFloat(ally.X, ally.Y, ally.Hp < ally.MaxHp ? $"+{heal} ward" : "ward", gold);
                AddTileGlyph(ally.X, ally.Y, null, "status", gold);
            }
            AddEpicBurst(king.X, king.Y, gold, 18, 1.35f);
            PushLog($"{king.Name} slams the royal charm and wards the shield hall.", Tone.Warn);
            CompleteEnemyPowerBeat(beat);
            return true;
        }

        private bool TryKoboldKingChargeAndRetreat(CombatUnit king, CombatUnit target)
        {
            if (king == null || target == null || king.Webbed > 0) return false;
            Point landing = BestEnemyChargeLanding(king, target, 5);
            if (landing == null) return false;

            EnemyPowerBeat beat = BeginEnemyPowerBeat(king, target, "royalcharge", target.X, target.Y, gold);
            Vector2 start = new Vector2(king.X, king.Y);
            AddBeam(king.X, king.Y, target.X, target.Y, gold, "shot");
            AddFloat(king.X, king.Y, "charge", gold);
            king.X = landing.X;
            king.Y = landing.Y;
            int damage = DealDamage(target, Mathf.Max(7, king.DamageMax + king.Power / 2 + 2), "physical", gold);
            if (target.Hp > 0)
            {
                target.Stunned = Mathf.Max(target.Stunned, 1);
                AddFloat(target.X, target.Y, "stun", gold);
            }
            AddBurst(target.X, target.Y, gold);
            if (target.Hp <= 0) ReportUnitDown(target);

            Point retreat = KoboldKingRetreatTile(king);
            if (retreat != null && (retreat.X != king.X || retreat.Y != king.Y))
            {
                Vector2 fromLanding = new Vector2(king.X, king.Y);
                king.X = retreat.X;
                king.Y = retreat.Y;
                AddTween(king.Id, fromLanding, new Vector2(king.X, king.Y), TweenKind.Move);
                AddFloat(king.X, king.Y, "back!", violet);
                AddTileGlyph(king.X, king.Y, null, "impact", violet);
                PushLog($"{king.Name} charges {target.Name} for {damage} physical, stuns, then snaps back to the throne line.", Tone.Warn);
            }
            else
            {
                AddTween(king.Id, start, new Vector2(king.X, king.Y), TweenKind.Move);
                PushLog($"{king.Name} charges {target.Name} for {damage} physical and holds the breach.", Tone.Warn);
            }
            CompleteEnemyPowerBeat(beat);
            return true;
        }

        private bool TryKoboldKingFireball(CombatUnit king, CombatUnit target)
        {
            if (king == null || target == null) return false;
            if (Distance(king.X, king.Y, target.X, target.Y) > 6) return false;
            EnemyPowerBeat beat = BeginEnemyPowerBeat(king, target, "royalfireball", target.X, target.Y, ember);
            int damage = Mathf.Max(8, king.Power + state.Depth + 2);
            AddEnemySpecialBeam(king, target, ember, "fireball");
            AddTileGlyph(target.X, target.Y, null, "fireball", ember);
            foreach (CombatUnit unit in state.Combat.Units.Where(u => u.Side == UnitSide.Party && u.Hp > 0 && Distance(u.X, u.Y, target.X, target.Y) <= 1).ToList())
            {
                int dealt = DealDamage(unit, unit.Id == target.Id ? damage : Mathf.Max(3, damage / 2), "fire", ember);
                AddFloat(unit.X, unit.Y, unit.Id == target.Id ? "fireball" : $"-{dealt}", ember);
                if (unit.Hp <= 0) ReportUnitDown(unit);
            }
            Point fire = BestHazardTileNear(target, "fire");
            if (fire != null) state.Combat.Obstacles.Add(fire);
            PushLog($"{king.Name} throws a crooked royal fireball into the party line.", Tone.Warn);
            CompleteEnemyPowerBeat(beat);
            return true;
        }

        private bool TryKoboldKingIceLance(CombatUnit king, CombatUnit target)
        {
            if (king == null || target == null) return false;
            if (Distance(king.X, king.Y, target.X, target.Y) > 6) return false;
            EnemyPowerBeat beat = BeginEnemyPowerBeat(king, target, "royalicelance", target.X, target.Y, frost);
            AddEnemySpecialBeam(king, target, frost, "ice");
            int damage = DealDamage(target, Mathf.Max(7, king.Power + state.Depth), "cold", frost);
            TryApplyStatus(target, "stun", 1, king, 0.24f, true);
            AddTileGlyph(target.X, target.Y, null, "impact", frost);
            AddFloat(target.X, target.Y, "ice lance", frost);
            Point ice = BestHazardTileNear(target, "ice");
            if (ice != null) state.Combat.Obstacles.Add(ice);
            if (target.Hp <= 0) ReportUnitDown(target);
            PushLog($"{king.Name} sketches a blue rune and drives an ice lance for {damage} cold.", Tone.Warn);
            CompleteEnemyPowerBeat(beat);
            return true;
        }

        private Point BestEnemyChargeLanding(CombatUnit enemy, CombatUnit target, int maxCost)
        {
            if (enemy == null || target == null) return null;
            int[,] reachable = ReachableMoveCosts(enemy, maxCost);
            Point[] candidates =
            {
                new Point(target.X + 1, target.Y, "landing"),
                new Point(target.X - 1, target.Y, "landing"),
                new Point(target.X, target.Y + 1, "landing"),
                new Point(target.X, target.Y - 1, "landing")
            };
            return candidates
                .Where(p => p.X >= 0 && p.X < CombatW && p.Y >= 0 && p.Y < CombatH)
                .Where(p => CanStandAt(p.X, p.Y) && reachable[p.X, p.Y] < UnreachableMoveCost)
                .OrderBy(p => reachable[p.X, p.Y])
                .ThenBy(p => Distance(KoboldKingHomeTile().X, KoboldKingHomeTile().Y, p.X, p.Y))
                .FirstOrDefault();
        }

        private Point KoboldKingRetreatTile(CombatUnit king)
        {
            Point home = KoboldKingHomeTile();
            if (CanStandAt(home.X, home.Y)) return home;
            Point[] candidates =
            {
                new Point(home.X, home.Y - 1),
                new Point(home.X, home.Y + 1),
                new Point(home.X - 1, home.Y),
                new Point(home.X + 1, home.Y),
                new Point(home.X - 1, home.Y - 1),
                new Point(home.X - 1, home.Y + 1)
            };
            return candidates
                .Where(p => p.X >= 0 && p.X < CombatW && p.Y >= 0 && p.Y < CombatH)
                .Where(p => CanStandAt(p.X, p.Y))
                .OrderBy(p => Distance(king.X, king.Y, p.X, p.Y))
                .FirstOrDefault();
        }

        private Point KoboldKingHomeTile()
        {
            return new Point(10, 3, "throne");
        }

        private float EnemySpecialChance(CombatUnit enemy)
        {
            if (enemy == null) return 0f;
            float baseChance = 0f;
            if (enemy.Role == "bonepriest" || enemy.Role == "ratcleric" || enemy.Role == "drowpriest") baseChance = 0.62f;
            else if (enemy.Role == "koboldking") baseChance = 0.78f;
            else if (enemy.Role == "koboldwizard") baseChance = 0.52f;
            else if (enemy.Role == "koboldshaman") baseChance = 0.46f;
            else if (enemy.Role == "adept" || enemy.Role == "glassmage" || enemy.Role == "ratmage" || enemy.Role == "drowmage" || enemy.Role == "spore" || enemy.Role == "cinderling" || enemy.Role == "lesserdemon" || enemy.Role == "shade") baseChance = 0.34f;
            return Mathf.Clamp01(baseChance + EnemyRankBonus(enemy) * 0.10f);
        }

        private int EnemyRankBonus(CombatUnit enemy)
        {
            if (enemy == null) return 0;
            if (enemy.Rank == "elite") return 2;
            if (enemy.Rank == "veteran") return 1;
            return 0;
        }

        private Point BestHazardTileNear(CombatUnit target, string kind)
        {
            if (target == null) return null;
            int duration = EnemyHazardDurationRounds(kind);
            Point[] candidates =
            {
                new Point(target.X, target.Y, kind, duration),
                new Point(target.X + 1, target.Y, kind, duration),
                new Point(target.X - 1, target.Y, kind, duration),
                new Point(target.X, target.Y + 1, kind, duration),
                new Point(target.X, target.Y - 1, kind, duration)
            };
            bool preferOpen = kind == "glyph" || kind == "demonrift";
            return candidates.FirstOrDefault(p => p.X >= 0 && p.X < CombatW && p.Y >= 0 && p.Y < CombatH && ObstacleAt(p.X, p.Y) == null && !IsBlockingTerrain(p) && (!preferOpen || UnitAt(p.X, p.Y) == null));
        }

        private bool PlaceEnemySummonMark(CombatUnit enemy, CombatUnit target, string kind, Color color, string label)
        {
            if (state?.Combat?.Obstacles == null || enemy == null || target == null) return false;
            int activeRituals = state.Combat.Obstacles.Count(CombatRitualRules.IsRitual);
            int matchingRituals = state.Combat.Obstacles.Count(point => CombatRitualRules.IsRitual(point) && string.Equals(point.Kind, kind, StringComparison.OrdinalIgnoreCase));
            if (activeRituals >= 3 || matchingRituals >= 2) return false;
            Point mark = BestHazardTileNear(target, kind);
            if (mark == null) return false;
            mark.Duration = CombatRitualRules.DefaultCountdown(kind);
            mark.Integrity = CombatRitualRules.MaxIntegrity(kind);
            state.Combat.Obstacles.Add(mark);
            AddFlash(mark.X, mark.Y, color.WithAlpha(0.72f));
            AddFloat(mark.X, mark.Y, label, color);
            PushLog(kind == "demonrift"
                ? $"{enemy.Name} tears a demon rift into the floor. Seal it within {mark.Duration} rounds or a lesser demon crosses."
                : $"{enemy.Name} scratches a summoning glyph into the floor. Break it within {mark.Duration} rounds or kobolds answer.", Tone.Warn);
            return true;
        }

        private CombatUnit BestEnemyTarget(CombatUnit enemy)
        {
            return state.Combat.Units.Where(u => u.Side == UnitSide.Party && u.Hp > 0)
                .OrderBy(u => EnemyTargetScore(enemy, u))
                .FirstOrDefault();
        }

        private int EnemyTargetScore(CombatUnit enemy, CombatUnit target)
        {
            int distance = Distance(target.X, target.Y, enemy.X, enemy.Y);
            int score = distance * 10;
            CombatAttackForecast forecast = AttackForecast(enemy, target);
            if (forecast.Legal)
            {
                score -= 45 + Mathf.Clamp(forecast.ExpectedDamage, 0, 12);
                if (forecast.ThreatLevel == CombatThreatLevel.Lethal) score -= 8;
            }
            if (enemy.Range > 1 && HasLineOfSight(enemy.X, enemy.Y, target.X, target.Y, true)) score -= 12;
            score += EnemyTacticsRules.TargetPriorityAdjustment(enemy, target);
            return score;
        }

        private bool CanEnemyAttack(CombatUnit enemy, CombatUnit target)
        {
            return AttackLegalityForecast(enemy, target).Legal;
        }

        private bool CanEnemySpecialReach(CombatUnit enemy, CombatUnit target)
        {
            if (enemy == null || target == null || target.Hp <= 0) return false;
            if (Distance(enemy.X, enemy.Y, target.X, target.Y) > enemy.Range) return false;
            return HasLineOfSight(enemy.X, enemy.Y, target.X, target.Y, true) || EnemySpecialArcsOverCover(enemy);
        }

        private bool EnemySpecialArcsOverCover(CombatUnit enemy)
        {
            if (enemy == null) return false;
            return enemy.Role == "koboldwizard"
                || enemy.Role == "koboldking"
                || enemy.Role == "koboldshaman"
                || enemy.Role == "adept"
                || enemy.Role == "glassmage"
                || enemy.Role == "ratmage"
                || enemy.Role == "ratcleric"
                || enemy.Role == "drowmage"
                || enemy.Role == "drowpriest"
                || enemy.Role == "shade"
                || enemy.DamageType == "death"
                || enemy.DamageType == "mind"
                || enemy.DamageType == "shock"
                || enemy.DamageType == "cold";
        }

        private void AddEnemySpecialBeam(CombatUnit enemy, CombatUnit target, Color color, string kind)
        {
            if (enemy == null || target == null) return;
            bool arcing = EnemySpecialArcsOverCover(enemy) && !HasLineOfSight(enemy.X, enemy.Y, target.X, target.Y, true);
            AddBeam(enemy.X, enemy.Y, target.X, target.Y, color, arcing ? "arc" : kind);
            if (arcing) AddFloat(target.X, target.Y, "over cover", color);
        }

        private void ResetEnemyActionPresentation()
        {
            enemyActionResolutionDelay = 0f;
            enemyActionResolutionLabel = "";
        }

        private EnemyPowerBeat BeginEnemyPowerBeat(
            CombatUnit enemy,
            CombatUnit target,
            string powerKey,
            int impactX,
            int impactY,
            Color color)
        {
            CombatPowerIdentity identity = CombatPowerPresentationRules.ForEnemyPower(powerKey, enemy?.Name, target?.Name);
            CombatImpactProfile profile = CombatImpactRules.ForEnemyPower(powerKey);
            Texture2D texture = null;
            Rect source = default;
            FormulaDef artFormula = null;

            string formulaCode = CombatPowerPresentationRules.EnemyPowerFormulaArtCode(powerKey);
            if (!string.IsNullOrEmpty(formulaCode))
            {
                artFormula = formulaBook.FirstOrDefault(formula => string.Equals(formula.Code, formulaCode, StringComparison.OrdinalIgnoreCase));
                TryGetFormulaPowerArt(artFormula, out texture, out source);
            }
            else
            {
                string abilityId = CombatPowerPresentationRules.EnemyPowerAbilityArtId(powerKey);
                if (!string.IsNullOrEmpty(abilityId))
                {
                    TryGetAbilityPowerArt(AbilityDef(abilityId), out texture, out source);
                }
            }

            BeginCombatPowerReactionCapture();
            CombatPowerOutcomeSnapshot before = CombatPowerOutcomeRules.Capture(state?.Combat);
            ShowCombatPowerCue(identity, texture, source, profile.ImpactDelay);
            StageCombatPowerCast(profile, enemy.X, enemy.Y, impactX, impactY, color, false);
            enemyActionResolutionDelay = CombatPowerResolutionRules.DelayForEnemyPower(powerKey, state != null && state.ReducedMotion);
            enemyActionResolutionLabel = identity.Title;

            return new EnemyPowerBeat
            {
                Profile = profile,
                Before = before,
                PreviousVfxDelay = BeginCombatVfxTimeline(profile),
                ImpactX = impactX,
                ImpactY = impactY,
                Color = color
            };
        }

        private void CompleteEnemyPowerBeat(EnemyPowerBeat beat)
        {
            RestoreCombatVfxTimeline(beat.PreviousVfxDelay);
            ApplyCombatImpactFeedback(beat.Profile, beat.ImpactX, beat.ImpactY, beat.Color);
            SetCombatPowerOutcome(beat.Before);
        }

        private void FinishResolvedPlayerFormulaAction(CombatUnit active, FormulaDef formula)
        {
            float delay = CombatPowerResolutionRules.DelayForFormula(formula, state != null && state.ReducedMotion);
            AfterCombatAction(active, delay, formula?.Name ?? "spell");
        }

        private void FinishResolvedPlayerAbilityAction(CombatUnit active, MartialAbility ability)
        {
            float delay = CombatPowerResolutionRules.DelayForAbility(ability, state != null && state.ReducedMotion);
            AfterCombatAction(active, delay, ability?.Name ?? "skill");
        }

        private CombatController CombatLifecycle()
        {
            CombatState combat = state?.Combat;
            if (combatController != null
                && ReferenceEquals(combatControllerState, state)
                && ReferenceEquals(combatControllerCombat, combat))
            {
                return combatController;
            }

            combatController = new CombatController(
                state,
                UnreachableMoveCost,
                UnitMoveAllowance,
                (unit, x, y) => CanStandAt(x, y),
                MoveCostTo,
                IsHeroUnit);
            combatControllerState = state;
            combatControllerCombat = combat;
            return combatController;
        }

        private void FinishCombatAction(
            CombatUnit active,
            bool endMovement = false,
            float resolutionDelay = 0f,
            string resolutionLabel = "")
        {
            if (state?.Combat == null) return;
            CombatLifecycle().CompleteAction(active, endMovement);
            AfterCombatAction(active, resolutionDelay, resolutionLabel);
        }

        private void AfterCombatAction(CombatUnit active, float resolutionDelay = 0f, string resolutionLabel = "")
        {
            if (state?.Combat == null) return;
            showAbilityPanel = false;
            showSpellbook = false;
            ClearAbilityEntry();
            ClearFormulaEntry();
            TickSummonBindingEndOfTurn(active);
            SyncPartyFromCombat();
            if (state != null && !state.ReducedMotion)
            {
                resolutionDelay = Mathf.Max(
                    resolutionDelay,
                    CombatUnitPresentationRules.RemainingHoldDuration(combatUnitPresentationBeats, Time.time));
            }
            if (resolutionDelay > 0f)
            {
                QueueCombatAdvance(active, resolutionDelay, resolutionLabel);
                return;
            }
            NextTurn();
        }

        private bool IsCombatResolutionPending()
        {
            return combatAdvancePending && state?.Mode == GameMode.Combat && state.Combat != null;
        }

        private void QueueCombatAdvance(CombatUnit active, float delay, string label)
        {
            if (state?.Combat == null || active == null)
            {
                NextTurn();
                return;
            }

            combatAdvancePending = true;
            combatAdvanceAt = Time.time + Mathf.Clamp(delay, 0.01f, 0.85f);
            combatAdvanceUnitId = active.Id ?? "";
            combatAdvanceStartsReservedTurn = false;
            combatResolutionLabel = string.IsNullOrWhiteSpace(label) ? "power" : label;
            state.Combat.Phase = CombatPhase.Resolving;
            aiActAt = -1f;
            MarkUiDirty();
        }

        private void CompletePendingCombatAdvance()
        {
            if (!combatAdvancePending || Time.time < combatAdvanceAt) return;
            string expectedUnitId = combatAdvanceUnitId;
            bool startsReservedTurn = combatAdvanceStartsReservedTurn;
            if (state?.Mode != GameMode.Combat || state.Combat == null)
            {
                CancelCombatResolutionBeat(false);
                return;
            }
            if (!string.IsNullOrEmpty(expectedUnitId) && state.Combat.ActiveId != expectedUnitId)
            {
                CancelCombatResolutionBeat(false);
                EnsureCombatTurnState();
                return;
            }
            CancelCombatResolutionBeat(false);
            if (startsReservedTurn)
            {
                CombatUnit reservedUnit = LiveUnitById(expectedUnitId);
                if (reservedUnit == null)
                {
                    NextTurn();
                    return;
                }
                BeginQueuedCombatTurn(reservedUnit);
                return;
            }
            NextTurn();
        }

        private void CancelCombatResolutionBeat(bool restoreActionPhase)
        {
            bool wasPending = combatAdvancePending;
            combatAdvancePending = false;
            combatAdvanceAt = -1f;
            combatAdvanceUnitId = "";
            combatAdvanceStartsReservedTurn = false;
            combatResolutionLabel = "";
            if (restoreActionPhase && wasPending && state?.Combat != null && state.Combat.Phase == CombatPhase.Resolving)
            {
                state.Combat.Phase = CombatPhase.ChooseAction;
            }
            if (wasPending) MarkUiDirty();
        }

        private void TickSummonBindingEndOfTurn(CombatUnit active)
        {
            SummonBindingResult result = CombatLifecycle().TickSummonBindingEndOfTurn(active);
            if (!result.Ticked) return;
            if (!result.Expired)
            {
                AddFloat(active.X, active.Y, result.RemainingTurns + "t", violet.WithAlpha(0.92f));
                return;
            }

            AddFloat(active.X, active.Y, "unbound", violet);
            AddBurst(active.X, active.Y, violet);
            AddFlash(active.X, active.Y, violet);
            PushLog($"{active.Name}'s binding fades.", Tone.Normal);
            PlaySfx("death", 0.55f);
        }

        private void RepairCombatSummons(bool quiet)
        {
            if (state?.Combat?.Units == null) return;
            List<CombatUnit> summons = state.Combat.Units.Where(u => u.Summoned && u.Hp > 0).ToList();
            if (summons.Count == 0) return;

            foreach (CombatUnit summon in summons)
            {
                if (summon.SummonTurns <= 0) summon.SummonTurns = 1;
                if (LiveSummonerFor(summon) != null) continue;
                if (!string.IsNullOrEmpty(summon.SummonerId)) continue;

                CombatUnit repaired = BestSummonerRepairTarget(summon);
                if (repaired != null)
                {
                    summon.SummonerId = repaired.Id;
                    if (!quiet) PushLog($"{summon.Name}'s pact binding is reattached to {repaired.Name}.", Tone.Normal);
                }
                else
                {
                    FadePactSummon(summon, quiet ? "" : "missing summoner");
                }
            }

            foreach (CombatUnit summoner in state.Combat.Units.Where(u => IsHeroUnit(u) && u.Hp > 0).ToList())
            {
                int max = MaxPactSummonBurdenFor(summoner);
                int burden = 0;
                foreach (CombatUnit summon in state.Combat.Units
                    .Where(u => u.Summoned && u.Hp > 0 && u.SummonerId == summoner.Id)
                    .OrderByDescending(u => SummonBurden(u.Role))
                    .ThenByDescending(u => u.SummonTurns)
                    .ToList())
                {
                    int next = burden + SummonBurden(summon.Role);
                    if (next <= max)
                    {
                        burden = next;
                        continue;
                    }
                    FadePactSummon(summon, quiet ? "" : "pact burden");
                }
            }
        }

        private void ApplySummonerDeathToPactSummons()
        {
            if (state?.Combat?.Units == null) return;
            foreach (CombatUnit summon in state.Combat.Units.Where(u => u.Summoned && u.Hp > 0).ToList())
            {
                if (LiveSummonerFor(summon) != null) continue;
                FadePactSummon(summon, "summoner lost");
            }
        }

        private CombatUnit LiveSummonerFor(CombatUnit summon)
        {
            if (summon == null || string.IsNullOrEmpty(summon.SummonerId) || state?.Combat?.Units == null) return null;
            return state.Combat.Units.FirstOrDefault(u => IsHeroUnit(u) && u.Hp > 0 && u.Id == summon.SummonerId);
        }

        private CombatUnit BestSummonerRepairTarget(CombatUnit summon)
        {
            if (state?.Combat?.Units == null) return null;
            return state.Combat.Units
                .Where(u => IsHeroUnit(u) && u.Hp > 0)
                .OrderByDescending(u => CasterKnowsSchool(u.Spell, "pact") ? 3 : (u.ClassKey == "warlock" ? 2 : !string.IsNullOrEmpty(u.Spell) ? 1 : 0))
                .ThenBy(u => Distance(u.X, u.Y, summon?.X ?? u.X, summon?.Y ?? u.Y))
                .FirstOrDefault();
        }

        private void FadePactSummon(CombatUnit summon, string reason = "")
        {
            if (summon == null || summon.Hp <= 0) return;
            summon.Hp = 0;
            AddFloat(summon.X, summon.Y, "unbound", violet);
            AddBurst(summon.X, summon.Y, violet);
            AddFlash(summon.X, summon.Y, violet);
            if (!string.IsNullOrEmpty(reason)) PushLog($"{summon.Name}'s binding fades ({reason}).", Tone.Normal);
            PlaySfx("death", 0.48f);
        }

        private bool UseInstantAbility(CombatUnit active, string abilityId)
        {
            MartialAbility ability = AbilityDef(abilityId);
            string reason;
            if (!AbilityUsableNow(active, ability, out reason))
            {
                PushLog(reason, Tone.Warn);
                PlaySfx("blocked", 0.62f);
                return false;
            }
            CombatImpactProfile impactProfile = CombatImpactRules.ForAbility(ability);
            CombatPowerIdentity identity = CombatPowerPresentationRules.ForAbility(ability, active.Name, null);
            Color powerColor = string.IsNullOrWhiteSpace(identity.AccentHex) ? gold : identity.AccentHex.ToColor();
            PowerCastAura stagedAura = StageCombatPowerCast(impactProfile, active.X, active.Y, active.X, active.Y, powerColor, false);
            BeginCombatPowerReactionCapture();
            float previousVfxDelay = BeginCombatVfxTimeline(impactProfile);
            bool success;
            try
            {
                success = ability.Id == "stealth" ? UseStealth(active)
                    : ability.Id == "rally" ? UseRally(active)
                    : ability.Id == "smokebomb" ? UseSmokeBomb(active)
                    : ability.Id == "whirlwind" ? UseWhirlwind(active)
                    : ability.Id == "abyssalwhirl" ? UseAbyssalWhirl(active)
                    : ability.Id == "dreadroar" && UseDreadRoar(active);
            }
            finally
            {
                RestoreCombatVfxTimeline(previousVfxDelay);
            }
            if (!success)
            {
                combatPowerReactions.Clear();
                if (stagedAura != null) powerCastAuras.Remove(stagedAura);
            }
            if (success)
            {
                ShowAbilityPowerCue(active, ability, null);
                ApplyCombatImpactFeedback(impactProfile, active.X, active.Y, powerColor);
            }
            return success;
        }

        private bool UseTargetedAbility(CombatUnit active, string abilityId, CombatUnit target, int x, int y)
        {
            MartialAbility ability = AbilityDef(abilityId);
            string reason;
            if (!AbilityUsableNow(active, ability, out reason) || !CanTargetAbility(active, ability, target, x, y, out reason))
            {
                PushLog(reason, Tone.Warn);
                PlaySfx("blocked", 0.62f);
                return false;
            }
            CombatImpactProfile impactProfile = CombatImpactRules.ForAbility(ability);
            CombatPowerIdentity identity = CombatPowerPresentationRules.ForAbility(ability, active.Name, target?.Name);
            Color powerColor = string.IsNullOrWhiteSpace(identity.AccentHex) ? gold : identity.AccentHex.ToColor();
            int sourceX = active.X;
            int sourceY = active.Y;
            PowerCastAura stagedAura = StageCombatPowerCast(impactProfile, sourceX, sourceY, x, y, powerColor, false);
            BeginCombatPowerReactionCapture();
            float previousVfxDelay = BeginCombatVfxTimeline(impactProfile);
            targetedMartialHitConnected = true;
            bool success;
            try
            {
                success = ability.Id == "charge" ? UseCharge(active, target)
                    : ability.Id == "execute" ? UseExecute(active, target)
                    : ability.Id == "shieldbash" ? UseShieldBash(active, target)
                    : ability.Id == "cleave" ? UseCleave(active, target)
                    : ability.Id == "ambush" ? UseAmbush(active, target)
                    : ability.Id == "throwknife" ? UseThrowKnife(active, target)
                    : ability.Id == "eviscerate" ? UseEviscerate(active, target)
                    : ability.Id == "hamstring" ? UseHamstring(active, target)
                    : ability.Id == "aimedshot" ? UseAimedShot(active, target)
                    : ability.Id == "pinningshot" ? UsePinningShot(active, target)
                    : ability.Id == "volley" ? UseVolley(active, target)
                    : ability.Id == "scoutmark" ? UseScoutMark(active, target)
                    : ability.Id == "broadheadshot" ? UseBroadheadShot(active, target)
                    : ability.Id == "disruptingshot" ? UseDisruptingShot(active, target)
                    : ability.Id == "riftpounce" ? UseRiftPounce(active, target)
                    : ability.Id == "soulrend" && UseSoulRend(active, target);
            }
            finally
            {
                RestoreCombatVfxTimeline(previousVfxDelay);
            }
            if (!success)
            {
                combatPowerReactions.Clear();
                if (stagedAura != null) powerCastAuras.Remove(stagedAura);
            }
            if (success)
            {
                ShowAbilityPowerCue(active, ability, target);
                if (targetedMartialHitConnected) ApplyCombatImpactFeedback(impactProfile, x, y, powerColor);
                else ApplyCombatMissFeedback(impactProfile, x);
            }
            return success;
        }

        private bool CanTargetAbility(CombatUnit active, MartialAbility ability, CombatUnit target, int x, int y, out string reason)
        {
            reason = "";
            if (active == null || ability == null)
            {
                reason = "No skill selected";
                return false;
            }
            if (!ability.Targeted)
            {
                reason = "This skill is instant";
                return false;
            }
            if (target == null || target.Side != UnitSide.Enemy)
            {
                reason = "Choose an enemy";
                return false;
            }
            if (Distance(active.X, active.Y, x, y) > ability.Range)
            {
                reason = $"Out of range {ability.Range}";
                return false;
            }
            if (ability.Id == "charge")
            {
                if (BestChargeLanding(active, target) == null)
                {
                    reason = "No open charge lane";
                    return false;
                }
                return true;
            }
            if (ability.Id == "riftpounce")
            {
                if (BestRiftPounceLanding(active, target) == null)
                {
                    reason = "No open rift landing";
                    return false;
                }
                return true;
            }
            if (IsSightLineAbility(ability.Id))
            {
                bool arcing = ability.Id == "volley";
                if (!arcing && !HasLineOfSight(active.X, active.Y, target.X, target.Y, true))
                {
                    reason = "Line of sight blocked";
                    return false;
                }
                return true;
            }
            if (Distance(active.X, active.Y, target.X, target.Y) > 1)
            {
                reason = "Needs adjacent target";
                return false;
            }
            if (ability.Id == "execute" && !CanExecuteTarget(target))
            {
                reason = "Target above 35% HP";
                return false;
            }
            return true;
        }

        private bool UseStealth(CombatUnit active)
        {
            active.Stealthed = Mathf.Max(active.Stealthed, 2);
            active.Guarding = false;
            AddFloat(active.X, active.Y, "stealth", teal);
            AddBurst(active.X, active.Y, teal);
            PushLog($"{active.Name} slips into shadow.", Tone.Good);
            return true;
        }

        private bool UseRally(CombatUnit active)
        {
            active.Guarding = true;
            active.GuardBonus = Mathf.Max(active.GuardBonus, 3 + GearGuardBonus(active));
            active.Shielded = Mathf.Max(active.Shielded, 2);
            int allies = 0;
            foreach (CombatUnit ally in state.Combat.Units.Where(u => u.Side == UnitSide.Party && u.Hp > 0 && u.Id != active.Id && Distance(u.X, u.Y, active.X, active.Y) <= 1))
            {
                ally.Shielded = Mathf.Max(ally.Shielded, 1);
                AddFloat(ally.X, ally.Y, "ward", teal);
                AddTileGlyph(ally.X, ally.Y, null, "status", teal);
                allies++;
            }
            AddFloat(active.X, active.Y, allies > 0 ? "rally" : "brace", gold);
            AddBurst(active.X, active.Y, gold);
            AddTileGlyph(active.X, active.Y, null, "status", gold);
            ImproveSkill(active, "guard", 2);
            string allyWord = allies == 1 ? "ally" : "allies";
            PushLog($"{active.Name} rallies the line, bracing with a ward{(allies > 0 ? $" and steadying {allies} adjacent {allyWord}" : "")}.", Tone.Good);
            return true;
        }

        private bool UseSmokeBomb(CombatUnit active)
        {
            active.Stealthed = Mathf.Max(active.Stealthed, 2);
            int clouds = 0;
            Point[] spots =
            {
                new Point(active.X + 1, active.Y),
                new Point(active.X - 1, active.Y),
                new Point(active.X, active.Y + 1),
                new Point(active.X, active.Y - 1)
            };
            foreach (Point spot in spots)
            {
                if (spot.X < 0 || spot.X >= CombatW || spot.Y < 0 || spot.Y >= CombatH) continue;
                if (UnitAt(spot.X, spot.Y) != null) continue;
                Point existing = ObstacleAt(spot.X, spot.Y);
                if (IsBlockingTerrain(existing)) continue;
                state.Combat.Obstacles.RemoveAll(o => o.X == spot.X && o.Y == spot.Y);
                state.Combat.Obstacles.Add(new Point(spot.X, spot.Y, "smoke", 3));
                Color smokeColor = Hex("8fa7a2");
                AddFloat(spot.X, spot.Y, "smoke", smokeColor);
                AddTileGlyph(spot.X, spot.Y, null, "status", smokeColor);
                AddFlash(spot.X, spot.Y, smokeColor);
                clouds++;
            }
            AddFloat(active.X, active.Y, "vanish", teal);
            AddBurst(active.X, active.Y, teal);
            ImproveSkill(active, "arms", 1);
            PushLog($"{active.Name} throws a smoke bomb, vanishing behind {clouds} short-lived cloud{(clouds == 1 ? "" : "s")}.", Tone.Good);
            return true;
        }

        private bool UseWhirlwind(CombatUnit active)
        {
            List<CombatUnit> enemies = AdjacentEnemies(active).ToList();
            if (enemies.Count == 0)
            {
                PushLog("No adjacent enemies.", Tone.Warn);
                PlaySfx("blocked", 0.62f);
                return false;
            }
            int raw = WhirlwindRawDamage(active);
            AddTween(active.Id, new Vector2(active.X, active.Y), new Vector2(active.X + 0.18f, active.Y), TweenKind.Lunge);
            CombatImpactProfile profile = CombatImpactRules.ForAbility(AbilityDef("whirlwind"));
            for (int i = 0; i < enemies.Count; i++)
            {
                CombatUnit enemy = enemies[i];
                float previousDelay = combatVfxImpactDelay;
                combatVfxImpactDelay = Mathf.Max(previousDelay, CombatImpactRules.SequenceImpactDelay(profile, i));
                try
                {
                    int damage = DealDamage(enemy, raw, "physical", blood);
                    AddFloat(enemy.X, enemy.Y, "slash", gold);
                    if (enemy.Hp <= 0) ReportUnitDown(enemy);
                    PushLog($"{active.Name}'s whirlwind cuts {enemy.Name} for {damage} physical.", enemy.Hp <= 0 ? Tone.Good : Tone.Normal);
                }
                finally
                {
                    combatVfxImpactDelay = previousDelay;
                }
            }
            ImproveSkill(active, "arms", 2);
            return true;
        }

        private bool UseRiftPounce(CombatUnit active, CombatUnit target)
        {
            Point landing = BestRiftPounceLanding(active, target);
            if (landing == null)
            {
                PushLog("No open tile beside that enemy can receive the rift.", Tone.Warn);
                PlaySfx("blocked", 0.62f);
                return false;
            }

            Vector2 from = new Vector2(active.X, active.Y);
            int fromX = active.X;
            int fromY = active.Y;
            active.X = landing.X;
            active.Y = landing.Y;
            CombatLifecycle().ApplyMovementBudgetResult(true, 0, 0);
            AddTween(active.Id, from, new Vector2(active.X, active.Y), TweenKind.Move);
            AddBeam(fromX, fromY, active.X, active.Y, violet, "arc");
            int damage = DealDamage(target, RiftPounceRawDamage(active), "death", violet);
            AddFloat(target.X, target.Y, "RIFT POUNCE", violet);
            ImproveSkill(active, "hex", 2);
            PushLog($"{active.Name} tears through the rift and pounces on {target.Name} for {damage} death damage.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
            if (target.Hp <= 0) ReportUnitDown(target);
            return true;
        }

        private bool UseAbyssalWhirl(CombatUnit active)
        {
            List<CombatUnit> enemies = AdjacentEnemies(active).ToList();
            if (enemies.Count == 0)
            {
                PushLog("No adjacent enemies.", Tone.Warn);
                PlaySfx("blocked", 0.62f);
                return false;
            }

            int raw = AbyssalWhirlRawDamage(active);
            AddTween(active.Id, new Vector2(active.X, active.Y), new Vector2(active.X + 0.18f, active.Y), TweenKind.Lunge);
            CombatImpactProfile profile = CombatImpactRules.ForAbility(AbilityDef("abyssalwhirl"));
            for (int i = 0; i < enemies.Count; i++)
            {
                CombatUnit enemy = enemies[i];
                float previousDelay = combatVfxImpactDelay;
                combatVfxImpactDelay = Mathf.Max(previousDelay, CombatImpactRules.SequenceImpactDelay(profile, i));
                try
                {
                    int damage = DealDamage(enemy, raw, "death", violet);
                    AddFloat(enemy.X, enemy.Y, "abyssal cut", violet);
                    PushLog($"{active.Name}'s abyssal whirl rends {enemy.Name} for {damage} death damage.", enemy.Hp <= 0 ? Tone.Good : Tone.Normal);
                    if (enemy.Hp <= 0) ReportUnitDown(enemy);
                }
                finally
                {
                    combatVfxImpactDelay = previousDelay;
                }
            }
            ImproveSkill(active, "hex", 3);
            return true;
        }

        private bool UseSoulRend(CombatUnit active, CombatUnit target)
        {
            if (!RollMartialHit(active, target, 14, "soul rend", "hex")) return true;
            int damage = DealDamage(target, SoulRendRawDamage(active), "death", violet);
            int missing = Mathf.Max(0, active.MaxHp - active.Hp);
            int healed = Mathf.Min(missing, Mathf.Max(1, damage / 2));
            active.Hp += healed;
            AddBeam(target.X, target.Y, active.X, active.Y, violet, "death");
            AddFloat(target.X, target.Y, "SOUL REND", violet);
            if (healed > 0) AddFloat(active.X, active.Y, "+" + healed, teal);
            ImproveSkill(active, "hex", 3);
            PushLog($"{active.Name} rends {target.Name}'s soul for {damage} death damage and recovers {healed} HP.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
            if (target.Hp <= 0) ReportUnitDown(target);
            return true;
        }

        private bool UseDreadRoar(CombatUnit active)
        {
            List<CombatUnit> enemies = AdjacentEnemies(active).ToList();
            if (enemies.Count == 0)
            {
                PushLog("No adjacent enemies.", Tone.Warn);
                PlaySfx("blocked", 0.62f);
                return false;
            }

            int stripped = 0;
            int hexed = 0;
            foreach (CombatUnit enemy in enemies)
            {
                if (enemy.Guarding || enemy.GuardBonus > 0)
                {
                    enemy.Guarding = false;
                    enemy.GuardBonus = 0;
                    stripped++;
                }
                if (TryApplyStatus(enemy, "hex", 3, active, 0.78f, true)) hexed++;
                AddFloat(enemy.X, enemy.Y, enemy.Hexed > 0 ? "DREAD" : "RESIST", violet);
            }
            RecordCombatPowerReaction("Dread roar");
            ImproveSkill(active, "hex", 2 + (hexed > 1 ? 1 : 0));
            PushLog($"{active.Name}'s dread roar breaks {stripped} guard{(stripped == 1 ? "" : "s")} and hexes {hexed} foe{(hexed == 1 ? "" : "s")}.", hexed > 0 ? Tone.Good : Tone.Normal);
            return true;
        }

        private bool UseCharge(CombatUnit active, CombatUnit target)
        {
            Point landing = BestChargeLanding(active, target);
            if (landing == null)
            {
                PushLog("No open charge lane.", Tone.Warn);
                PlaySfx("blocked", 0.62f);
                return false;
            }
            Vector2 from = new Vector2(active.X, active.Y);
            active.X = landing.X;
            active.Y = landing.Y;
            CombatLifecycle().ApplyMovementBudgetResult(true, 0, 0);
            AddTween(active.Id, from, new Vector2(active.X, active.Y), TweenKind.Move);
            int damage = DealDamage(target, ChargeRawDamage(active), "physical", blood);
            if (target.Hp > 0)
            {
                target.Stunned = Mathf.Max(target.Stunned, 1);
                AddFloat(target.X, target.Y, "stun", gold);
            }
            AddBurst(target.X, target.Y, gold);
            ImproveSkill(active, "arms", 2);
            PushLog($"{active.Name} charges {target.Name} for {damage} physical and staggers the line.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
            if (target.Hp <= 0) ReportUnitDown(target);
            return true;
        }

        private bool UseExecute(CombatUnit active, CombatUnit target)
        {
            if (!RollMartialHit(active, target, 12, "execute")) return true;
            int damage = DealDamage(target, ExecuteRawDamage(active), "physical", blood);
            ImproveSkill(active, "arms", 2);
            PushLog($"{active.Name} executes a finishing cut on {target.Name} for {damage} physical.", target.Hp <= 0 ? Tone.Good : Tone.Warn);
            if (target.Hp <= 0) ReportUnitDown(target);
            return true;
        }

        private bool UseShieldBash(CombatUnit active, CombatUnit target)
        {
            if (!RollMartialHit(active, target, 8, "shield bash", "guard")) return true;
            int rawDamage = ShieldBashRawDamage(active);
            int damage = DealDamage(target, rawDamage, "physical", teal);
            string outcome = "";
            if (target.Hp > 0)
            {
                string collision;
                if (TryPushCombatUnitAway(active, target, out collision))
                {
                    bool stunned = TryApplyStatus(target, "stun", 1, active, 0.40f, true);
                    AddFloat(target.X, target.Y, stunned ? "pushed + stun" : "pushed", gold);
                    outcome = stunned ? " and drives it back stunned" : " and drives it back";
                }
                else
                {
                    int collisionDamage = DealDamage(target, LightningPowerRules.CollisionDamage(rawDamage), "physical", stone);
                    target.Stunned = Mathf.Max(target.Stunned, 1);
                    RecordCombatPowerReaction("Shield collision");
                    AddFloat(target.X, target.Y, "COLLISION", gold);
                    outcome = $" and crushes it into {collision} for {collisionDamage} more";
                }
            }
            AddTween(active.Id, new Vector2(active.X, active.Y), new Vector2(active.X + Mathf.Sign(target.X - active.X) * 0.18f, active.Y + Mathf.Sign(target.Y - active.Y) * 0.18f), TweenKind.Lunge);
            AddBurst(target.X, target.Y, teal);
            ImproveSkill(active, "guard", 2);
            PushLog($"{active.Name} shield-bashes {target.Name} for {damage} physical{outcome}.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
            if (target.Hp <= 0) ReportUnitDown(target);
            return true;
        }

        private bool UseCleave(CombatUnit active, CombatUnit target)
        {
            if (!RollMartialHit(active, target, 4, "cleave")) return true;
            int raw = CleaveRawDamage(active);
            int damage = DealDamage(target, raw, "physical", blood);
            AddTween(active.Id, new Vector2(active.X, active.Y), new Vector2(active.X + Mathf.Sign(target.X - active.X) * 0.22f, active.Y + Mathf.Sign(target.Y - active.Y) * 0.22f), TweenKind.Lunge);
            AddFloat(target.X, target.Y, "cleave", gold);
            CombatUnit secondary = CleaveSecondaryTarget(active, target);
            if (secondary != null)
            {
                int splash = DealDamage(secondary, Mathf.Max(2, raw / 2), "physical", blood);
                AddFloat(secondary.X, secondary.Y, "cleaved", gold);
                PushLog($"{active.Name}'s cleave clips {secondary.Name} for {splash} physical.", secondary.Hp <= 0 ? Tone.Good : Tone.Normal);
                if (secondary.Hp <= 0) ReportUnitDown(secondary);
            }
            ImproveSkill(active, "arms", secondary == null ? 2 : 3);
            PushLog($"{active.Name} cleaves {target.Name} for {damage} physical.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
            if (target.Hp <= 0) ReportUnitDown(target);
            return true;
        }

        private bool UseAmbush(CombatUnit active, CombatUnit target)
        {
            bool hidden = active.Stealthed > 0;
            if (!RollMartialHit(active, target, hidden ? 24 : 10, "ambush"))
            {
                active.Stealthed = 0;
                return true;
            }
            int damage = DealDamage(target, AmbushRawDamage(active, hidden), "physical", blood);
            active.Stealthed = 0;
            if (hidden && target.Hp > 0)
            {
                target.Stunned = Mathf.Max(target.Stunned, 1);
                AddFloat(target.X, target.Y, "stun", gold);
            }
            ImproveSkill(active, "arms", 2);
            PushLog($"{active.Name} ambushes {target.Name} for {damage} physical{(hidden ? " from stealth" : "")}.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
            if (target.Hp <= 0) ReportUnitDown(target);
            return true;
        }

        private bool UseThrowKnife(CombatUnit active, CombatUnit target)
        {
            if (!RollMartialHit(active, target, 12, "throw knife", "missile")) return true;
            int damage = DealDamage(target, ThrowKnifeRawDamage(active), "physical", blood);
            if (target.Hp > 0)
            {
                target.Bleeding = Mathf.Max(target.Bleeding, 2);
                AddFloat(target.X, target.Y, "bleed", blood);
            }
            AddBeam(active.X, active.Y, target.X, target.Y, Hex("d9d3c4"), "shot");
            AddFloat(target.X, target.Y, "knife", gold);
            ImproveSkill(active, "missile", 2);
            PushLog($"{active.Name} throws a knife at {target.Name} for {damage} physical.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
            if (target.Hp <= 0) ReportUnitDown(target);
            return true;
        }

        private bool UseEviscerate(CombatUnit active, CombatUnit target)
        {
            bool hidden = active.Stealthed > 0;
            if (!RollMartialHit(active, target, hidden ? 18 : 6, "eviscerate"))
            {
                active.Stealthed = 0;
                return true;
            }
            int damage = DealDamage(target, EviscerateRawDamage(active, target, hidden), "physical", blood);
            active.Stealthed = 0;
            if (target.Hp > 0)
            {
                target.Bleeding = Mathf.Max(target.Bleeding, 3);
                AddFloat(target.X, target.Y, "bleed", blood);
            }
            ImproveSkill(active, "arms", 2);
            PushLog($"{active.Name} eviscerates {target.Name} for {damage} physical.", target.Hp <= 0 ? Tone.Good : Tone.Warn);
            if (target.Hp <= 0) ReportUnitDown(target);
            return true;
        }

        private bool UseHamstring(CombatUnit active, CombatUnit target)
        {
            if (!RollMartialHit(active, target, 8, "hamstring")) return true;
            int damage = DealDamage(target, HamstringRawDamage(active), "physical", blood);
            if (target.Hp > 0)
            {
                target.Webbed = Mathf.Max(target.Webbed, 2);
                target.Bleeding = Mathf.Max(target.Bleeding, 2);
                AddFloat(target.X, target.Y, "hobbled", moss);
            }
            AddTween(active.Id, new Vector2(active.X, active.Y), new Vector2(active.X + Mathf.Sign(target.X - active.X) * 0.16f, active.Y + Mathf.Sign(target.Y - active.Y) * 0.16f), TweenKind.Lunge);
            ImproveSkill(active, "arms", 2);
            PushLog($"{active.Name} hamstrings {target.Name} for {damage} physical.", target.Hp <= 0 ? Tone.Good : Tone.Warn);
            if (target.Hp <= 0) ReportUnitDown(target);
            return true;
        }

        private bool UseAimedShot(CombatUnit active, CombatUnit target)
        {
            if (!RollMartialHit(active, target, 18, "aimed shot", "missile")) return true;
            int damage = DealDamage(target, AimedShotRawDamage(active, target), "physical", gold);
            AddBeam(active.X, active.Y, target.X, target.Y, gold, "shot");
            AddRangerTileGlyph(target.X, target.Y, 0, gold);
            ImproveSkill(active, "missile", 2);
            PushLog($"{active.Name} lands an aimed shot on {target.Name} for {damage} physical.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
            if (target.Hp <= 0) ReportUnitDown(target);
            return true;
        }

        private bool UsePinningShot(CombatUnit active, CombatUnit target)
        {
            if (!RollMartialHit(active, target, 10, "pinning shot", "missile")) return true;
            int damage = DealDamage(target, PinningShotRawDamage(active), "physical", moss);
            if (target.Hp > 0)
            {
                target.Webbed = Mathf.Max(target.Webbed, target.Range > 1 ? 2 : 1);
                AddFloat(target.X, target.Y, "pinned", Hex("d9d3c4"));
            }
            AddBeam(active.X, active.Y, target.X, target.Y, moss, "shot");
            AddRangerTileGlyph(target.X, target.Y, 1, moss);
            ImproveSkill(active, "missile", 2);
            PushLog($"{active.Name} pins {target.Name} for {damage} physical.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
            if (target.Hp <= 0) ReportUnitDown(target);
            return true;
        }

        private bool UseVolley(CombatUnit active, CombatUnit target)
        {
            List<CombatUnit> enemies = state.Combat.Units
                .Where(u => u.Side == UnitSide.Enemy && u.Hp > 0 && Distance(u.X, u.Y, target.X, target.Y) <= 1)
                .OrderBy(u => u.Id == target.Id ? 0 : 1)
                .ThenBy(u => Distance(active.X, active.Y, u.X, u.Y))
                .ToList();
            int raw = VolleyRawDamage(active);
            int hits = 0;
            CombatImpactProfile profile = CombatImpactRules.ForAbility(AbilityDef("volley"));
            for (int i = 0; i < enemies.Count; i++)
            {
                CombatUnit enemy = enemies[i];
                float sequenceDelay = CombatImpactRules.SequenceImpactDelay(profile, i);
                float previousDelay = combatVfxImpactDelay;
                combatVfxImpactDelay = Mathf.Max(previousDelay, sequenceDelay);
                try
                {
                    float deliveryStartDelay = Mathf.Max(0f, sequenceDelay - CombatPowerVisualRules.BeamDuration("arc"));
                    AddBeamDelayed(active.X, active.Y, enemy.X, enemy.Y, gold, "arc", deliveryStartDelay);
                    int damage = DealDamage(enemy, enemy.Id == target.Id ? raw : Mathf.Max(2, raw / 2), "physical", gold);
                    AddFloat(enemy.X, enemy.Y, "volley", gold);
                    AddRangerTileGlyph(enemy.X, enemy.Y, 15, gold);
                    if (enemy.Hp <= 0) ReportUnitDown(enemy);
                    hits++;
                    PushLog($"{active.Name}'s volley hits {enemy.Name} for {damage} physical.", enemy.Hp <= 0 ? Tone.Good : Tone.Normal);
                }
                finally
                {
                    combatVfxImpactDelay = previousDelay;
                }
            }
            ImproveSkill(active, "missile", Mathf.Max(2, hits));
            return hits > 0;
        }

        private bool UseScoutMark(CombatUnit active, CombatUnit target)
        {
            bool brokeGuard = target.Guarding || target.GuardBonus > 0;
            int wardRemoved = target.Shielded > 0 ? 1 : 0;
            target.Guarding = false;
            target.GuardBonus = 0;
            target.Shielded = Mathf.Max(0, target.Shielded - wardRemoved);
            target.Hexed = Mathf.Max(target.Hexed, 2);
            AddBeam(active.X, active.Y, target.X, target.Y, teal, "shot");
            AddFloat(target.X, target.Y, brokeGuard || wardRemoved > 0 ? "guard broken" : "marked", teal);
            AddRangerTileGlyph(target.X, target.Y, 3, teal);
            AddBurst(target.X, target.Y, teal);
            ImproveSkill(active, "missile", 1);
            string breakText = brokeGuard && wardRemoved > 0
                ? " breaks its guard and strips one ward"
                : brokeGuard
                    ? " breaks its guard"
                    : wardRemoved > 0
                        ? " strips one ward"
                        : "";
            PushLog($"{active.Name}{breakText} and marks {target.Name} for the party.", Tone.Good);
            return true;
        }

        private bool UseBroadheadShot(CombatUnit active, CombatUnit target)
        {
            if (!RollMartialHit(active, target, 12, "broadhead shot", "missile")) return true;
            int damage = DealDamage(target, BroadheadShotRawDamage(active), "physical", blood);
            if (target.Hp > 0)
            {
                target.Bleeding = Mathf.Max(target.Bleeding, 3);
                AddFloat(target.X, target.Y, "bleed", blood);
            }
            AddBeam(active.X, active.Y, target.X, target.Y, blood, "shot");
            AddRangerTileGlyph(target.X, target.Y, CombatFeedbackRules.RangerImpactIndex("broadheadshot"), blood);
            ImproveSkill(active, "missile", 2);
            PushLog($"{active.Name} sinks a broadhead into {target.Name} for {damage} physical.", target.Hp <= 0 ? Tone.Good : Tone.Warn);
            if (target.Hp <= 0) ReportUnitDown(target);
            return true;
        }

        private bool UseDisruptingShot(CombatUnit active, CombatUnit target)
        {
            if (!RollMartialHit(active, target, IsCasterEnemy(target) ? 18 : 8, "disrupting shot", "missile")) return true;
            int damage = DealDamage(target, DisruptingShotRawDamage(active), "physical", teal);
            if (target.Hp > 0)
            {
                target.Stunned = Mathf.Max(target.Stunned, 1);
                if (IsCasterEnemy(target)) target.Hexed = Mathf.Max(target.Hexed, 1);
                AddFloat(target.X, target.Y, IsCasterEnemy(target) ? "spell broken" : "stun", teal);
            }
            AddBeam(active.X, active.Y, target.X, target.Y, teal, "shot");
            AddRangerTileGlyph(target.X, target.Y, CombatFeedbackRules.RangerImpactIndex("disruptingshot"), teal);
            ImproveSkill(active, "missile", 2);
            PushLog($"{active.Name} disrupts {target.Name} for {damage} physical.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
            if (target.Hp <= 0) ReportUnitDown(target);
            return true;
        }

        private bool RollMartialHit(CombatUnit active, CombatUnit target, int bonus, string verb, string skillName = "arms")
        {
            int chance = MartialHitChance(active, target, bonus, skillName);
            if (rng.Next(100) < chance)
            {
                targetedMartialHitConnected = true;
                return true;
            }
            targetedMartialHitConnected = false;
            AddTween(active.Id, new Vector2(active.X, active.Y), new Vector2(active.X + Mathf.Sign(target.X - active.X) * 0.18f, active.Y + Mathf.Sign(target.Y - active.Y) * 0.18f), TweenKind.Lunge);
            AddFloat(target.X, target.Y, "miss", muted);
            AddFlash(target.X, target.Y, muted);
            ImproveSkill(active, skillName, 1);
            PushLog($"{active.Name}'s {verb} misses {target.Name}.", Tone.Normal);
            float pan = CombatAudioMixRules.StereoPanForColumn(target.X, CombatW);
            float pitch = CombatAudioMixRules.PitchForCue("miss", target.X);
            QueueSfx("miss", combatVfxImpactDelay, 0.64f, pan, pitch);
            return false;
        }

        private int MartialHitChance(CombatUnit active, CombatUnit target, int bonus, string skillName)
        {
            if (active == null || target == null) return 0;
            string skill = string.IsNullOrWhiteSpace(skillName) ? "arms" : skillName;
            if (skill == "arms") return Mathf.Clamp(AttackHitChance(active, target) + bonus, 18, 96);
            int skillValue = SkillValue(active.Skills, skill);
            int guard = target.Guarding ? Mathf.Max(2, target.GuardBonus) : 0;
            int statusShift = 0;
            if (active.Hexed > 0) statusShift -= 12;
            if (target.Hexed > 0) statusShift += 10;
            if (target.Webbed > 0) statusShift += 8;
            if (target.Sleeping > 0) statusShift += 18;
            int gear = skill == "missile" ? Mathf.Max(0, active.WeaponBonus * 2) : active.WeaponBonus * 4;
            return Mathf.Clamp(62 + skillValue + active.Agility * 2 + active.AttackSpeed + gear + RaceHitBonus(active) + statusShift + bonus - target.Agility * 3 - (target.Defense + target.ArmorBonus) * 3 - guard * 8, 18, 96);
        }

        private bool IsRangerAbility(string abilityId)
        {
            string id = (abilityId ?? "").ToLowerInvariant();
            return id == "aimedshot" || id == "pinningshot" || id == "volley" || id == "scoutmark" || id == "broadheadshot" || id == "disruptingshot";
        }

        private bool IsSightLineAbility(string abilityId)
        {
            string id = (abilityId ?? "").ToLowerInvariant();
            return IsRangerAbility(id) || id == "throwknife";
        }

        private IEnumerable<CombatUnit> AdjacentEnemies(CombatUnit active)
        {
            if (active == null || state?.Combat?.Units == null) yield break;
            foreach (CombatUnit unit in state.Combat.Units)
            {
                if (unit.Side == UnitSide.Enemy && unit.Hp > 0 && Distance(active.X, active.Y, unit.X, unit.Y) <= 1) yield return unit;
            }
        }

        private Point BestChargeLanding(CombatUnit active, CombatUnit target)
        {
            if (active == null || target == null) return null;
            if (Distance(active.X, active.Y, target.X, target.Y) <= 1) return new Point(active.X, active.Y, "landing");
            int[,] reachable = ReachableMoveCosts(active, 5);
            Point[] candidates =
            {
                new Point(target.X + 1, target.Y, "landing"),
                new Point(target.X - 1, target.Y, "landing"),
                new Point(target.X, target.Y + 1, "landing"),
                new Point(target.X, target.Y - 1, "landing")
            };
            return candidates
                .Where(p => p.X >= 0 && p.X < CombatW && p.Y >= 0 && p.Y < CombatH)
                .Where(p => CanStandAt(p.X, p.Y) && reachable[p.X, p.Y] < UnreachableMoveCost)
                .OrderBy(p => reachable[p.X, p.Y])
                .ThenBy(p => Distance(active.X, active.Y, p.X, p.Y))
                .FirstOrDefault();
        }

        private Point BestRiftPounceLanding(CombatUnit active, CombatUnit target)
        {
            if (active == null || target == null) return null;
            if (Distance(active.X, active.Y, target.X, target.Y) <= 1) return new Point(active.X, active.Y, "riftlanding");
            Point[] candidates =
            {
                new Point(target.X + 1, target.Y, "riftlanding"),
                new Point(target.X - 1, target.Y, "riftlanding"),
                new Point(target.X, target.Y + 1, "riftlanding"),
                new Point(target.X, target.Y - 1, "riftlanding")
            };
            return candidates
                .Where(p => p.X >= 0 && p.X < CombatW && p.Y >= 0 && p.Y < CombatH)
                .Where(p => CanStandAt(p.X, p.Y))
                .OrderBy(p => Distance(active.X, active.Y, p.X, p.Y))
                .ThenBy(p => p.Y)
                .ThenBy(p => p.X)
                .FirstOrDefault();
        }

        private bool CanExecuteTarget(CombatUnit target)
        {
            return target != null && target.MaxHp > 0 && target.Hp <= Mathf.CeilToInt(target.MaxHp * 0.35f);
        }

        private CombatUnit CleaveSecondaryTarget(CombatUnit active, CombatUnit target)
        {
            if (active == null || target == null || state?.Combat?.Units == null) return null;
            return state.Combat.Units
                .Where(u => u.Side == UnitSide.Enemy && u.Hp > 0 && u.Id != target.Id)
                .Where(u => Distance(active.X, active.Y, u.X, u.Y) <= 1 || Distance(target.X, target.Y, u.X, u.Y) <= 1)
                .OrderBy(u => Distance(target.X, target.Y, u.X, u.Y))
                .ThenBy(u => u.Hp)
                .FirstOrDefault();
        }

        private int WarriorEnrageBonus(CombatUnit unit)
        {
            if (unit == null || MartialClassKey(unit) != "warrior" || unit.MaxHp <= 0 || unit.Hp * 2 > unit.MaxHp) return 0;
            return Mathf.Max(2, unit.Power / 4 + SkillValue(unit.Skills, "arms") / 8);
        }

        private int AbilityStatDamageBonus(CombatUnit active, string abilityId)
        {
            if (active == null || PartyMemberForUnit(active) == null) return 0;
            string id = (abilityId ?? "").ToLowerInvariant();
            if (id == "charge" || id == "execute" || id == "shieldbash" || id == "cleave" || id == "whirlwind")
            {
                return Mathf.Max(0, (UnitStrengthScore(active) - 10) / 8);
            }
            if (id == "ambush" || id == "eviscerate" || id == "hamstring")
            {
                return Mathf.Max(0, (Mathf.Max(UnitAgilityScore(active), UnitStrengthScore(active)) - 10) / 8);
            }
            if (id == "throwknife" || id == "aimedshot" || id == "pinningshot" || id == "volley" || id == "broadheadshot" || id == "disruptingshot")
            {
                return Mathf.Max(0, (UnitAgilityScore(active) - 10) / 7);
            }
            if (id == "riftpounce" || id == "abyssalwhirl" || id == "soulrend")
            {
                return Mathf.Max(0, (Mathf.Max(UnitIntelligenceScore(active), UnitStrengthScore(active)) - 10) / 7);
            }
            return 0;
        }

        private string AbilityStatNote(CombatUnit active, string abilityId)
        {
            int bonus = AbilityStatDamageBonus(active, abilityId);
            if (bonus <= 0) return "";
            string id = (abilityId ?? "").ToLowerInvariant();
            string stat = id == "throwknife" || id == "aimedshot" || id == "pinningshot" || id == "volley" || id == "broadheadshot" || id == "disruptingshot"
                ? "AGI"
                : id == "ambush" || id == "eviscerate" || id == "hamstring"
                    ? "finesse"
                    : id == "riftpounce" || id == "abyssalwhirl" || id == "soulrend"
                        ? "demon"
                    : "STR";
            return $" / {stat} +{bonus}";
        }

        private int ChargeRawDamage(CombatUnit active)
        {
            return Mathf.Max(2, active.DamageMax + active.Power / 3 + SkillValue(active.Skills, "arms") / 6 + WarriorEnrageBonus(active) + AbilityStatDamageBonus(active, "charge"));
        }

        private int ExecuteRawDamage(CombatUnit active)
        {
            return Mathf.Max(4, active.DamageMax + active.Power + SkillValue(active.Skills, "arms") / 4 + active.Level * 2 + WarriorEnrageBonus(active) + AbilityStatDamageBonus(active, "execute"));
        }

        private int ShieldBashRawDamage(CombatUnit active)
        {
            return Mathf.Max(2, active.DamageMin + SkillValue(active.Skills, "guard") / 3 + GearGuardBonus(active) + active.Level + WarriorEnrageBonus(active) / 2 + AbilityStatDamageBonus(active, "shieldbash"));
        }

        private int CleaveRawDamage(CombatUnit active)
        {
            return Mathf.Max(3, active.DamageMax + SkillValue(active.Skills, "arms") / 4 + active.Level + WarriorEnrageBonus(active) + AbilityStatDamageBonus(active, "cleave"));
        }

        private int AmbushRawDamage(CombatUnit active, bool hidden)
        {
            return Mathf.Max(3, active.DamageMax + SkillValue(active.Skills, "arms") / 3 + (hidden ? 8 : 2) + AbilityStatDamageBonus(active, "ambush"));
        }

        private int ThrowKnifeRawDamage(CombatUnit active)
        {
            return Mathf.Max(2, active.DamageMin + SkillValue(active.Skills, "missile") / 4 + active.Level + AbilityStatDamageBonus(active, "throwknife"));
        }

        private int EviscerateRawDamage(CombatUnit active, CombatUnit target, bool hidden)
        {
            int bleedBonus = target != null && target.Bleeding > 0 ? 4 : 0;
            return Mathf.Max(4, active.DamageMax + SkillValue(active.Skills, "arms") / 3 + 6 + bleedBonus + (hidden ? 3 : 0) + AbilityStatDamageBonus(active, "eviscerate"));
        }

        private int HamstringRawDamage(CombatUnit active)
        {
            return Mathf.Max(3, active.DamageMin + SkillValue(active.Skills, "arms") / 4 + active.Level + 2 + AbilityStatDamageBonus(active, "hamstring"));
        }

        private int WhirlwindRawDamage(CombatUnit active)
        {
            return Mathf.Max(2, active.DamageMax + SkillValue(active.Skills, "arms") / 4 - 1 + WarriorEnrageBonus(active) + AbilityStatDamageBonus(active, "whirlwind"));
        }

        private int RiftPounceRawDamage(CombatUnit active)
        {
            return Mathf.Max(5, active.DamageMax + active.Power / 2 + SkillValue(active.Skills, "hex") / 4 + active.Level + AbilityStatDamageBonus(active, "riftpounce"));
        }

        private int AbyssalWhirlRawDamage(CombatUnit active)
        {
            return Mathf.Max(4, active.DamageMax + active.Power / 3 + SkillValue(active.Skills, "hex") / 4 + active.Level / 2 + AbilityStatDamageBonus(active, "abyssalwhirl"));
        }

        private int SoulRendRawDamage(CombatUnit active)
        {
            return Mathf.Max(6, active.DamageMax + active.Power + SkillValue(active.Skills, "hex") / 3 + active.Level + AbilityStatDamageBonus(active, "soulrend"));
        }

        private int AimedShotRawDamage(CombatUnit active, CombatUnit target)
        {
            int markBonus = target != null && target.Hexed > 0 ? 4 : 0;
            return Mathf.Max(3, active.DamageMax + SkillValue(active.Skills, "missile") / 3 + active.Level + markBonus + AbilityStatDamageBonus(active, "aimedshot"));
        }

        private int PinningShotRawDamage(CombatUnit active)
        {
            return Mathf.Max(2, active.DamageMin + SkillValue(active.Skills, "missile") / 4 + active.Level + AbilityStatDamageBonus(active, "pinningshot"));
        }

        private int VolleyRawDamage(CombatUnit active)
        {
            return Mathf.Max(3, active.DamageMax + SkillValue(active.Skills, "missile") / 4 + active.Level - 1 + AbilityStatDamageBonus(active, "volley"));
        }

        private int BroadheadShotRawDamage(CombatUnit active)
        {
            return Mathf.Max(3, active.DamageMax + SkillValue(active.Skills, "missile") / 3 + active.Level + 1 + AbilityStatDamageBonus(active, "broadheadshot"));
        }

        private int DisruptingShotRawDamage(CombatUnit active)
        {
            return Mathf.Max(2, active.DamageMin + SkillValue(active.Skills, "missile") / 4 + active.Level + 2 + AbilityStatDamageBonus(active, "disruptingshot"));
        }

        private void ReportUnitDown(CombatUnit unit)
        {
            if (unit == null || unit.Hp > 0) return;
            PushLog($"{unit.Name} is down.", unit.Side == UnitSide.Enemy ? Tone.Good : Tone.Warn);
            AddFloat(unit.X, unit.Y, "down", gold);
            AddFlash(unit.X, unit.Y, gold);
        }

        private bool IsCasterEnemy(CombatUnit enemy)
        {
            return EnemyTacticsRules.IsCaster(enemy);
        }

        private bool IsBruteEnemy(CombatUnit enemy)
        {
            return EnemyTacticsRules.IsBrute(enemy);
        }

        private void MoveActiveTo(CombatUnit active, int x, int y)
        {
            CombatCommandResult result = CombatLifecycle().TryMove(active, x, y);
            if (!result.Success)
            {
                if (result.Failure == CombatCommandFailure.Webbed)
                {
                    PushLog($"{active.Name} is caught in webbing.", Tone.Warn);
                    AddFloat(active.X, active.Y, "webbed", poison);
                    PlaySfx("web");
                }
                else if (result.Failure == CombatCommandFailure.Blocked)
                {
                    PlaySfx("blocked");
                }
                else if (result.Failure == CombatCommandFailure.PathBlocked)
                {
                    PushLog("No clear path to that tile.", Tone.Warn);
                    PlaySfx("blocked");
                }
                else if (result.Failure == CombatCommandFailure.TooFar)
                {
                    PushLog("That move is too far.", Tone.Warn);
                    PlaySfx("blocked");
                }
                return;
            }

            AddTween(active.Id, result.OldPosition, result.NewPosition, TweenKind.Move);
            string warning = TerrainLogWarning(ObstacleAt(x, y));
            PushLog($"{active.Name} takes position. {state.Combat.MovePoints} move left.", Tone.Normal);
            if (!string.IsNullOrEmpty(warning)) PushLog(warning, Tone.Warn);
            PlaySfx("move", 0.72f);
        }

        private bool UndoActiveMovement()
        {
            CombatUnit active = CurrentUnit();
            if (active == null || active.Side != UnitSide.Party)
            {
                PushLog("Only the active party member can undo movement.", Tone.Warn);
                PlaySfx("blocked", 0.55f);
                return false;
            }

            CombatCommandResult result = CombatLifecycle().TryUndoMove(active);
            if (!result.Success)
            {
                PushLog("No uncommitted movement to undo.", Tone.Warn);
                PlaySfx("blocked", 0.55f);
                return false;
            }

            ClearFormulaEntry();
            ClearAbilityEntry();
            showSpellbook = false;
            showAbilityPanel = false;
            selectedAction = ActionMode.Move;
            if (result.OldX != result.NewX || result.OldY != result.NewY)
            {
                AddTween(active.Id, result.OldPosition, result.NewPosition, TweenKind.Move);
            }
            PushLog($"{active.Name} returns to the turn's starting tile. Movement restored.", Tone.Good);
            PlaySfx("move", 0.62f);
            SuppressBoardPointer();
            MarkUiDirty();
            return true;
        }

        private bool Attack(CombatUnit attacker, CombatUnit target)
        {
            CombatAttackForecast forecast = AttackForecast(attacker, target);
            if (!forecast.Legal)
            {
                if (forecast.BlockReason == AttackForecastBlockReason.OutOfRange)
                {
                    PushLog($"{target.Name} is out of reach. {AttackModeLabel(attacker)} range is {forecast.Range}.", Tone.Warn);
                }
                else if (forecast.BlockReason == AttackForecastBlockReason.LineOfSight)
                {
                    PushLog("Cover breaks the shot line.", Tone.Warn);
                    AddFloat(target.X, target.Y, "cover", moss);
                    PlaySfx("blocked");
                }
                else
                {
                    PushLog(CombatThreatRules.BlockLabel(forecast.BlockReason) + ".", Tone.Warn);
                    PlaySfx("blocked");
                }
                return false;
            }
            bool ranged = forecast.Ranged;
            string skill = AttackSkillNameAt(attacker, target, attacker.X, attacker.Y);
            int skillValue = SkillValue(attacker.Skills, skill);
            int hitChance = forecast.HitChance;
            int critFloor = target.Sleeping > 0 || target.Hexed > 0 ? 72 : 82;
            critFloor -= Mathf.Clamp(attacker.AttackSpeed / 4, 0, 4);
            bool critical = rng.Next(100) >= Mathf.Max(critFloor, 97 - skillValue / 3);
            if (rng.Next(100) >= hitChance)
            {
                AddTween(attacker.Id, new Vector2(attacker.X, attacker.Y), new Vector2(attacker.X + Mathf.Sign(target.X - attacker.X) * 0.16f, attacker.Y + Mathf.Sign(target.Y - attacker.Y) * 0.16f), TweenKind.Lunge);
                AddFloat(target.X, target.Y, "miss", muted);
                StageWeaponImpactFeedback(attacker, target, attacker.DamageType, false, false, ranged);
                PushLog($"{attacker.Name} misses {target.Name}.", Tone.Normal);
                PlayWeaponAttackSequence(attacker, target, attacker.DamageType, false, false, ranged);
                ImproveSkill(attacker, skill, 1);
                if (attacker.Stealthed > 0)
                {
                    attacker.Stealthed = 0;
                    AddFloat(attacker.X, attacker.Y, "revealed", teal);
                }
                return true;
            }
            AttackDamageProfile damageProfile = AttackRules.BuildDamageProfile(attacker, target, skillValue, WarriorEnrageBonus(attacker), DemonFormAttackBonus(attacker));
            string damageType = damageProfile.DamageType;
            int enrageBonus = damageProfile.EnrageBonus;
            bool stealthStrike = attacker.Stealthed > 0;
            int rawDamage = damageProfile.RawDamageForBaseRoll(rng.Next(damageProfile.BaseMinDamage, damageProfile.BaseMaxDamage + 1));
            if (critical) rawDamage = Mathf.RoundToInt(rawDamage * 1.65f) + 2;
            int damage = DealDamage(target, rawDamage, damageType, DamageColor(damageType));
            if (enrageBonus > 0) AddFloat(attacker.X, attacker.Y, "enrage", gold);
            if (stealthStrike)
            {
                attacker.Stealthed = 0;
                AddFloat(attacker.X, attacker.Y, "revealed", teal);
            }
            AddTween(attacker.Id, new Vector2(attacker.X, attacker.Y), new Vector2(attacker.X + Mathf.Sign(target.X - attacker.X) * 0.25f, attacker.Y + Mathf.Sign(target.Y - attacker.Y) * 0.25f), TweenKind.Lunge);
            StageWeaponImpactFeedback(attacker, target, damageType, critical, true, ranged);
            if (critical) AddFloat(target.X, target.Y, "crit", gold);
            string verb = ranged ? "shoots" : critical ? "strikes hard" : "hits";
            PushLog($"{attacker.Name} {verb} {target.Name} for {damage} {damageType}.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
            PlayWeaponAttackSequence(attacker, target, damageType, critical, true, ranged);
            ImproveSkill(attacker, skill, 1);
            if (target.Hp > 0 && target.Guarding && Distance(attacker.X, attacker.Y, target.X, target.Y) <= 1)
            {
                int counter = Mathf.Max(1, target.GuardBonus / 2);
                DealDamage(attacker, counter, "physical", teal);
                AddFloat(attacker.X, attacker.Y, "counter", teal);
                PushLog($"{target.Name}'s guard bites back.", target.Side == UnitSide.Party ? Tone.Good : Tone.Warn);
                PlaySfx("counter", 0.62f);
            }
            if (target.Hp > 0 && !string.IsNullOrEmpty(attacker.StatusOnHit))
            {
                TryApplyStatus(target, attacker.StatusOnHit, 2, attacker, 0.45f, attacker.Side != target.Side);
            }
            int lifeDrain = GearLifeDrainAmount(attacker.WeaponName, damage);
            if (lifeDrain > 0 && attacker.Hp > 0)
            {
                attacker.Hp = Mathf.Min(attacker.MaxHp, attacker.Hp + lifeDrain);
                AddFloat(attacker.X, attacker.Y, "+" + lifeDrain + " drain", violet);
                AddBurst(attacker.X, attacker.Y, violet);
                PushLog($"{attacker.Name}'s blade drinks a little life.", attacker.Side == UnitSide.Party ? Tone.Good : Tone.Warn);
                PlaySfx("death", 0.38f);
            }
            if (target.Hp > 0)
            {
                string gearStatus = GearOnHitStatus(attacker.WeaponName);
                if (!string.IsNullOrEmpty(gearStatus))
                {
                    TryApplyStatus(target, gearStatus, 2, attacker, GearOnHitChance(attacker.WeaponName), true);
                }
            }
            if (target.Hp <= 0)
            {
                PushLog($"{target.Name} is down.", Tone.Good);
                AddFloat(target.X, target.Y, "down", gold);
            }
            return true;
        }

        private void StageWeaponImpactFeedback(CombatUnit attacker, CombatUnit target, string damageType, bool critical, bool hit, bool ranged)
        {
            if (attacker == null || target == null) return;
            StageWeaponImpactFeedbackAt(
                attacker,
                target.X,
                target.Y,
                hit ? DamageColor(damageType) : Hex("d9d3c4", 0.72f),
                critical,
                hit,
                ranged,
                hit && target.Guarding,
                hit && target.Hp <= 0);
        }

        private void StageWeaponImpactFeedbackAt(
            CombatUnit attacker,
            int targetX,
            int targetY,
            Color accent,
            bool critical,
            bool hit,
            bool ranged,
            bool guarded,
            bool defeated)
        {
            if (attacker == null) return;
            WeaponFeedbackProfile feedback = WeaponFeedbackRules.For(attacker.WeaponName, ranged);
            if (feedback.Kind == WeaponFeedbackKind.Projectile)
            {
                AddBeam(attacker.X, attacker.Y, targetX, targetY, accent, "shot");
            }

            AddBeamDelayed(
                attacker.X,
                attacker.Y,
                targetX,
                targetY,
                critical ? Color.Lerp(accent, gold, 0.44f) : accent,
                hit ? feedback.VisualKind : "weapon-miss",
                feedback.ImpactDelay);
            AddFlashDelayed(targetX, targetY, accent.WithAlpha(hit ? 0.76f : 0.52f), feedback.ImpactDelay);
            if (guarded)
            {
                AddBeamDelayed(attacker.X, attacker.Y, targetX, targetY, Color.Lerp(frost, cursorWhite, 0.20f), "weapon-guard", feedback.ImpactDelay + 0.014f);
            }
            if (defeated)
            {
                AddBeamDelayed(attacker.X, attacker.Y, targetX, targetY, Color.Lerp(accent, gold, 0.58f), "weapon-defeat", feedback.ImpactDelay + 0.032f);
                AddEpicBurstDelayed(targetX, targetY, Color.Lerp(accent, gold, 0.34f), 9, 0.88f, feedback.ImpactDelay + 0.032f);
            }
            if (!hit || state == null || state.ReducedMotion) return;

            int count = WeaponFeedbackRules.PresentationBurstCount(feedback, critical);
            float speed = feedback.Kind == WeaponFeedbackKind.Heavy ? 1.18f : feedback.Kind == WeaponFeedbackKind.Projectile ? 0.84f : 1.02f;
            AddEpicBurstDelayed(targetX, targetY, Color.Lerp(accent, cursorWhite, 0.18f), count, speed, feedback.ImpactDelay);
        }

        private void StageCoverImpactFeedback(CombatUnit attacker, Point cover, Color accent, bool broken, bool ranged, bool arcing = false)
        {
            if (attacker == null || cover == null) return;
            float impactDelay;
            if (arcing)
            {
                impactDelay = 0.09f;
                AddBeam(attacker.X, attacker.Y, cover.X, cover.Y, accent, "arc");
                AddFlashDelayed(cover.X, cover.Y, accent.WithAlpha(0.78f), impactDelay);
                AddEpicBurstDelayed(cover.X, cover.Y, Color.Lerp(accent, cursorWhite, 0.18f), broken ? 10 : 6, 0.92f, impactDelay);
            }
            else
            {
                WeaponFeedbackProfile feedback = WeaponFeedbackRules.For(attacker.WeaponName, ranged);
                impactDelay = feedback.ImpactDelay;
                StageWeaponImpactFeedbackAt(attacker, cover.X, cover.Y, accent, broken, true, ranged, false, false);
            }

            if (broken)
            {
                AddBeamDelayed(attacker.X, attacker.Y, cover.X, cover.Y, accent, WeaponFeedbackRules.CoverBreakVisualKind(cover.Kind), impactDelay + 0.026f);
            }
        }

        private bool AttackCover(CombatUnit attacker, Point cover)
        {
            if (attacker == null || !IsBreakableCover(cover)) return false;
            int distance = Distance(attacker.X, attacker.Y, cover.X, cover.Y);
            int attackRange = EffectiveAttackRangeTo(attacker, cover.X, cover.Y);
            bool ranged = UsesRangedAttackAt(attacker, attacker.X, attacker.Y, cover.X, cover.Y);
            if (distance > attackRange)
            {
                PushLog($"{CoverName(cover)} is out of reach. {AttackModeLabel(attacker)} range is {attackRange}.", Tone.Warn);
                PlaySfx("blocked", 0.62f);
                return false;
            }
            if (ranged && !HasLineOfSight(attacker.X, attacker.Y, cover.X, cover.Y, true))
            {
                PushLog("Other cover blocks the shot.", Tone.Warn);
                AddFloat(cover.X, cover.Y, "blocked", moss);
                PlaySfx("blocked", 0.62f);
                return false;
            }

            int damage = CoverBreakDamage(attacker, cover);
            cover.Integrity = Mathf.Max(0, CoverIntegrity(cover) - damage);
            Color color = cover.Kind == "tree" ? moss : stone;
            bool broken = cover.Integrity <= 0;
            if (!ranged)
            {
                AddTween(attacker.Id, new Vector2(attacker.X, attacker.Y), new Vector2(attacker.X + Mathf.Sign(cover.X - attacker.X) * 0.18f, attacker.Y + Mathf.Sign(cover.Y - attacker.Y) * 0.18f), TweenKind.Lunge);
            }
            StageCoverImpactFeedback(attacker, cover, color, broken, ranged);
            AddFloat(cover.X, cover.Y, broken ? "broken" : "-" + damage, color);
            ImproveSkill(attacker, ranged ? "missile" : "arms", 1);
            if (broken)
            {
                state.Combat.Obstacles.Remove(cover);
                PushLog($"{attacker.Name} breaks the {CoverName(cover)}.", Tone.Good);
            }
            else
            {
                PushLog($"{attacker.Name} damages the {CoverName(cover)}. {cover.Integrity} integrity remains.", Tone.Normal);
            }
            PlayCoverAttackSequence(attacker, cover, ranged, broken);
            return true;
        }

        private int AttackHitChance(CombatUnit attacker, CombatUnit target)
        {
            if (attacker == null || target == null) return 0;
            return AttackHitChanceAt(attacker, target, attacker.X, attacker.Y);
        }

        private int AttackHitChanceAt(CombatUnit attacker, CombatUnit target, int attackerX, int attackerY)
        {
            if (attacker == null || target == null) return 0;
            string skill = AttackSkillNameAt(attacker, target, attackerX, attackerY);
            int skillValue = SkillValue(attacker.Skills, skill);
            int guard = target.Guarding ? Mathf.Max(2, target.GuardBonus) : 0;
            int statusShift = 0;
            if (attacker.Hexed > 0) statusShift -= 12;
            if (target.Hexed > 0) statusShift += 10;
            if (target.Webbed > 0) statusShift += 8;
            if (target.Sleeping > 0) statusShift += 18;
            return Mathf.Clamp(62 + skillValue + attacker.Agility * 2 + attacker.AttackSpeed + attacker.WeaponBonus * 4 + WeaponHitBonus(attacker.WeaponName) + RaceHitBonus(attacker) + statusShift - target.Agility * 3 - (target.Defense + target.ArmorBonus) * 3 - guard * 8, 18, 95);
        }

        private bool CastFormula(CombatUnit caster, string code, CombatUnit target, int x, int y)
        {
            FormulaDef formula = GetFormula(code);
            string reason;
            if (!CanUseFormula(caster, code, out reason))
            {
                PushLog(reason, Tone.Warn);
                PlaySfx("blocked");
                return false;
            }
            if (formula == null)
            {
                PushLog(code + " has no stable shape.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }
            int range = EffectiveFormulaRange(formula, caster);
            if (Distance(caster.X, caster.Y, x, y) > range)
            {
                PushLog($"{formula.Name} cannot reach beyond range {range}.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }
            if (!CanTargetFormula(formula, caster, target, x, y))
            {
                if (formula != null && formula.Effect == "summon" && !CanSummonFormulaAt(formula, caster, x, y, out reason))
                {
                    PushLog(reason, Tone.Warn);
                }
                else
                {
                    PushLog(FormulaTargetPrompt(formula), Tone.Warn);
                }
                PlaySfx("blocked");
                return false;
            }
            if (!HasFormulaLineOfSight(formula, caster, x, y))
            {
                PushLog(FormulaSightBlockText(formula), Tone.Warn);
                AddFloat(x, y, "cover", moss);
                PlaySfx("blocked");
                return false;
            }
            if (formula.Code == "RSG" && !AdjacentEnemies(caster).Any())
            {
                PushLog("Thunderclap needs at least one adjacent enemy.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }
            int manaCost = EffectiveFormulaMana(formula, caster);
            if (caster.Mana < manaCost)
            {
                PushLog($"{caster.Name} lacks mana.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }

            bool focused = IsFocusedCaster(caster);
            caster.Mana -= manaCost;
            string skill = FormulaSkill(formula, caster);
            CombatImpactProfile impactProfile = CombatImpactRules.ForFormula(formula);
            PowerCastAura stagedAura = StageCombatPowerCast(impactProfile, caster.X, caster.Y, x, y, FormulaColor(formula), focused);
            BeginCombatPowerReactionCapture();
            float previousVfxDelay = BeginCombatVfxTimeline(impactProfile);
            bool success;
            try
            {
                success = ResolveFormula(formula, caster, target, x, y);
            }
            finally
            {
                RestoreCombatVfxTimeline(previousVfxDelay);
            }
            if (!success)
            {
                caster.Mana += manaCost;
                combatPowerReactions.Clear();
                if (stagedAura != null) powerCastAuras.Remove(stagedAura);
                PlaySfx("blocked");
                return false;
            }
            bool advancedStorm = formula.Effect == "chain" || formula.Effect == "tempest" || formula.Effect == "thunderclap";
            ImproveSkill(caster, skill, formula.Splash || advancedStorm ? 3 : 2);
            if (focused) AddFloat(caster.X, caster.Y, "focused", gold);
            ShowFormulaPowerCue(caster, formula, target, focused);
            ApplyFormulaImpactFeedback(formula, impactProfile, x, y);
            return true;
        }

        private bool AttackRitual(CombatUnit attacker, Point ritual)
        {
            if (attacker == null || !IsDisruptableRitual(ritual)) return false;
            int distance = Distance(attacker.X, attacker.Y, ritual.X, ritual.Y);
            int attackRange = EffectiveAttackRangeTo(attacker, ritual.X, ritual.Y);
            bool ranged = UsesRangedAttackAt(attacker, attacker.X, attacker.Y, ritual.X, ritual.Y);
            if (distance > attackRange)
            {
                PushLog($"{RitualName(ritual)} is out of reach. {AttackModeLabel(attacker)} range is {attackRange}.", Tone.Warn);
                PlaySfx("blocked", 0.62f);
                return false;
            }
            if (ranged && !HasLineOfSight(attacker.X, attacker.Y, ritual.X, ritual.Y, true))
            {
                PushLog("Cover blocks the ritual mark.", Tone.Warn);
                AddFloat(ritual.X, ritual.Y, "blocked", ObstacleAccent(ritual.Kind));
                PlaySfx("blocked", 0.62f);
                return false;
            }

            int damage = RitualDisruptionDamage(attacker, ritual);
            ritual.Integrity = Mathf.Max(0, RitualIntegrity(ritual) - damage);
            Color color = ObstacleAccent(ritual.Kind);
            AddTween(attacker.Id, new Vector2(attacker.X, attacker.Y), new Vector2(attacker.X + Mathf.Sign(ritual.X - attacker.X) * 0.18f, attacker.Y + Mathf.Sign(ritual.Y - attacker.Y) * 0.18f), TweenKind.Lunge);
            if (ranged) AddBeam(attacker.X, attacker.Y, ritual.X, ritual.Y, color, "shot");
            AddFloat(ritual.X, ritual.Y, ritual.Integrity <= 0 ? "SEALED" : "-" + damage, color);
            AddFlash(ritual.X, ritual.Y, color);
            ImproveSkill(attacker, ranged ? "missile" : "arms", 1);
            if (ritual.Integrity <= 0)
            {
                state.Combat.Obstacles.Remove(ritual);
                AddEpicBurst(ritual.X, ritual.Y, Color.Lerp(color, teal, 0.42f), 18, 1.28f);
                AddTileGlyph(ritual.X, ritual.Y, null, "impact", color);
                PushLog($"{attacker.Name} disrupts the {RitualName(ritual)} before it opens.", Tone.Good);
                ShowBanner("Ritual Disrupted");
                PlaySfx("resonance", 0.76f);
                PlaySfx("impactlow", 0.34f);
            }
            else
            {
                PushLog($"{attacker.Name} damages the {RitualName(ritual)}. {ritual.Integrity} integrity and {Mathf.Max(1, ritual.Duration)} round{(ritual.Duration == 1 ? "" : "s")} remain.", Tone.Normal);
                PlaySfx("death", 0.56f);
            }
            return true;
        }

        private void ApplyFormulaImpactFeedback(FormulaDef formula, CombatImpactProfile impactProfile, int x, int y)
        {
            string visualKind = CombatPowerVisualRules.ImpactKindForFormula(formula, impactProfile.ImpactSfx);
            if (formula != null
                && formula.Effect == "terrain"
                && string.Equals(visualKind, impactProfile.ImpactSfx, StringComparison.OrdinalIgnoreCase))
            {
                ApplyCombatImpactAudioFeedback(impactProfile, x, y);
                return;
            }
            ApplyCombatImpactFeedback(impactProfile, x, y, FormulaColor(formula), visualKind);
        }

        private bool ResolveFormula(FormulaDef formula, CombatUnit caster, CombatUnit target, int x, int y)
        {
            if (formula.Effect == "teleport")
            {
                if (!CanStandAt(x, y)) return false;
                int fromX = caster.X;
                int fromY = caster.Y;
                Color color = FormulaColor(formula);
                AddTileGlyph(fromX, fromY, formula, "impact", color);
                AddBurst(fromX, fromY, color);
                AddFlash(fromX, fromY, color);
                caster.X = x;
                caster.Y = y;
                AddTween(caster.Id, new Vector2(fromX, fromY), new Vector2(x, y), TweenKind.Move);
                AddBeam(fromX, fromY, x, y, color, "arc");
                AddTileGlyph(x, y, formula, "impact", color);
                AddEpicBurst(x, y, Color.Lerp(color, frost, 0.42f), 22, 1.55f);
                AddFlash(x, y, color);
                int shocked = formula.Code == "VST" ? ResolveThunderStepArrival(formula, caster) : 0;
                AddFloat(
                    x,
                    y,
                    formula.Code == "VST" ? "THUNDER STEP" : formula.Code == "VRS" ? "RIFT STEP" : "veil step",
                    color);
                PushLog(
                    formula.Code == "VST"
                        ? $"{caster.Name} rides the lightning {Distance(fromX, fromY, x, y)} tiles and shocks {shocked} nearby {(shocked == 1 ? "enemy" : "enemies")}."
                        : formula.Code == "VRS"
                            ? $"{caster.Name} crosses the rift and reappears {Distance(fromX, fromY, x, y)} tiles away."
                        : $"{caster.Name} folds the veil and reappears {Distance(fromX, fromY, x, y)} tiles away.",
                    Tone.Good);
                return true;
            }

            if (formula.Effect == "thunderclap")
            {
                return ResolveThunderclap(formula, caster);
            }

            if (formula.Effect == "chain")
            {
                return ResolveChainLightning(formula, caster, target);
            }

            if (formula.Effect == "tempest")
            {
                return ResolveArcaneTempest(formula, caster, target);
            }

            if (formula.Effect == "transform")
            {
                if (target == null || target.Id != caster.Id) return false;
                int turns = Mathf.Max(1, formula.Duration + (IsFocusedCaster(caster) ? 1 : 0));
                int heal = 6 + Mathf.Max(0, UnitIntelligenceScore(caster) - 10) / 3;
                // Status durations tick at the next turn start. Preserve the advertised
                // number of complete demon-form actions after the casting turn.
                caster.DemonFormTurns = Mathf.Max(caster.DemonFormTurns, turns + 1);
                caster.Shielded = Mathf.Max(caster.Shielded, 3);
                caster.Regenerating = Mathf.Max(caster.Regenerating, 3);
                caster.Hp = Mathf.Min(caster.MaxHp, caster.Hp + heal);
                Color color = FormulaColor(formula);
                AddBeam(caster.X, caster.Y, caster.X, caster.Y, color, "death");
                AddTileGlyph(caster.X, caster.Y, formula, "area", color);
                AddEpicBurst(caster.X, caster.Y, Color.Lerp(color, blood, 0.38f), 32, 1.90f);
                AddFlash(caster.X, caster.Y, color);
                AddFloat(caster.X, caster.Y, "ASCEND", color);
                PushLog($"{caster.Name} assumes an abyssal shape for {turns} turns and recovers {heal} HP.", Tone.Good);
                return true;
            }

            if (formula.Effect == "summon")
            {
                if (!CanSummonFormulaAt(formula, caster, x, y, out _)) return false;
                CombatUnit summon = MakeSummonedUnit(formula, caster, x, y);
                state.Combat.Units.Add(summon);
                StageCombatUnitPresentationBeat(
                    summon,
                    CombatUnitPresentationBeatKind.Reveal,
                    CombatEffectStart(),
                    0f);
                AddBeam(caster.X, caster.Y, x, y, FormulaColor(formula), FormulaBeamKind(formula, caster, x, y));
                AddFloat(x, y, "bound", FormulaColor(formula));
                AddBurst(x, y, FormulaColor(formula));
                AddTileGlyph(x, y, formula, "impact", FormulaColor(formula));
                AddFlash(x, y, FormulaColor(formula));
                PushLog($"{caster.Name} binds {summon.Name} for {summon.SummonTurns} turns.", Tone.Good);
                return true;
            }

            if (formula.Effect == "dispel")
            {
                Point field = ObstacleAt(x, y);
                if (!CombatRitualRules.IsDispelableField(field)) return false;
                bool ritual = IsDisruptableRitual(field);
                string fieldName = ritual ? RitualName(field) : TerrainDescription(field.Kind).TrimEnd('.');
                Color color = FormulaColor(formula);
                state.Combat.Obstacles.Remove(field);
                AddBeam(caster.X, caster.Y, x, y, color, "arc");
                AddTileGlyph(x, y, formula, ritual ? "death" : "impact", color);
                if (ritual) AddEpicBurst(x, y, Color.Lerp(color, teal, 0.48f), 26, 1.58f);
                else AddEpicBurst(x, y, Color.Lerp(color, frost, 0.34f), 16, 1.16f);
                AddFlash(x, y, color);
                AddFloat(x, y, ritual ? "SEALED" : "UNRAVELED", color);
                RecordCombatPowerReaction(ritual ? "Rift sealed" : "Field unraveled");
                PushLog($"{caster.Name} casts {formula.Name}. The {fieldName.ToLowerInvariant()} is {(ritual ? "sealed" : "unraveled")}.", Tone.Good);
                return true;
            }

            if (formula.Effect == "terrain")
            {
                Point existing = ObstacleAt(x, y);
                string reaction = ApplyTerrainPlacementReaction(formula, caster, x, y, existing);
                string finalTerrain = TerrainAfterPlacementReaction(formula, existing);
                state.Combat.Obstacles.RemoveAll(o => o.X == x && o.Y == y);
                if (!string.IsNullOrEmpty(finalTerrain))
                {
                    int duration = FieldDurationRounds(finalTerrain, formula.Duration);
                    state.Combat.Obstacles.Add(new Point(x, y, finalTerrain, duration));
                }
                AddBeam(caster.X, caster.Y, x, y, FormulaColor(formula), FormulaBeamKind(formula, caster, x, y));
                AddFloat(x, y, SpellFloatLabel(formula), FormulaColor(formula));
                AddFieldPlacementFlourish(x, y, finalTerrain, FormulaColor(formula));
                string placed = string.IsNullOrEmpty(finalTerrain) ? "The field collapses after the reaction." : TerrainDescription(finalTerrain);
                PushLog($"{caster.Name} casts {formula.Name}. {placed}", Tone.Good);
                if (!string.IsNullOrEmpty(reaction)) PushLog(reaction, Tone.Good);
                return true;
            }

            if (formula.Effect == "heal")
            {
                int heal = formula.Power + SkillValue(caster.Skills, formula.Skill) / 2 + FormulaStatPowerBonus(formula, caster) + rng.Next(0, 5);
                target.Hp = Mathf.Min(target.MaxHp, target.Hp + heal);
                AddBeam(caster.X, caster.Y, target.X, target.Y, teal, FormulaBeamKind(formula, caster, target.X, target.Y));
                AddFloat(target.X, target.Y, "+" + heal, teal);
                AddBurst(target.X, target.Y, teal);
                AddTileGlyph(target.X, target.Y, formula, formula.Splash ? "area" : "impact", teal);
                AddFlash(target.X, target.Y, teal);
                int splashHeals = 0;
                if (formula.Splash)
                {
                    foreach (CombatUnit ally in state.Combat.Units.Where(u => u.Side == target.Side && u.Hp > 0 && u.Id != target.Id && Distance(u.X, u.Y, target.X, target.Y) <= 1))
                    {
                        int splashHeal = Mathf.Max(2, heal / 2);
                        ally.Hp = Mathf.Min(ally.MaxHp, ally.Hp + splashHeal);
                        AddFloat(ally.X, ally.Y, "+" + splashHeal, teal);
                        AddBurst(ally.X, ally.Y, teal);
                        AddTileGlyph(ally.X, ally.Y, formula, "impact", teal);
                        splashHeals++;
                    }
                }
                PushLog($"{caster.Name} casts {formula.Name}. {target.Name} recovers {heal}.", Tone.Good);
                if (splashHeals > 0) PushLog("The mend rings through nearby allies.", Tone.Good);
                return true;
            }

            if (formula.Effect == "cure")
            {
                target.Poisoned = 0;
                target.Bleeding = 0;
                target.Webbed = 0;
                target.Stunned = 0;
                target.Sleeping = 0;
                target.Hexed = 0;
                AddBeam(caster.X, caster.Y, target.X, target.Y, teal, FormulaBeamKind(formula, caster, target.X, target.Y));
                AddFloat(target.X, target.Y, "cleansed", teal);
                AddBurst(target.X, target.Y, teal);
                AddTileGlyph(target.X, target.Y, formula, "impact", teal);
                AddFlash(target.X, target.Y, teal);
                PushLog($"{caster.Name} casts {formula.Name}. {target.Name} is cleansed.", Tone.Good);
                return true;
            }

            if (formula.Effect == "status")
            {
                if (target != null) AddBeam(caster.X, caster.Y, target.X, target.Y, FormulaColor(formula), FormulaBeamKind(formula, caster, target.X, target.Y));
                if (target != null) AddTileGlyph(target.X, target.Y, formula, formula.Splash ? "area" : "impact", FormulaColor(formula));
                bool applied = TryApplyStatus(target, formula.Status, formula.Duration, caster, 0.86f, formula.Target == "enemy");
                int splashApplied = 0;
                if (formula.Splash && target != null)
                {
                    foreach (CombatUnit unit in state.Combat.Units.Where(u => u.Side == target.Side && u.Hp > 0 && u.Id != target.Id && Distance(u.X, u.Y, target.X, target.Y) <= 1))
                    {
                        if (TryApplyStatus(unit, formula.Status, Mathf.Max(1, formula.Duration - 1), caster, 0.72f, formula.Target == "enemy")) splashApplied++;
                        AddBeam(target.X, target.Y, unit.X, unit.Y, FormulaColor(formula), "arc");
                        AddTileGlyph(unit.X, unit.Y, formula, "impact", FormulaColor(formula));
                    }
                }
                string result = target == null ? "" : applied ? $"{target.Name} is {StatusLabel(formula.Status)}." : $"{target.Name} resists.";
                PushLog($"{caster.Name} casts {formula.Name}. {result}", applied ? Tone.Good : Tone.Warn);
                if (splashApplied > 0) PushLog($"The sign spreads to {splashApplied} nearby target{(splashApplied == 1 ? "" : "s")}.", Tone.Good);
                return true;
            }

            if (formula.Effect == "drain")
            {
                int damage = FormulaDamage(formula, caster, target);
                AddBeam(caster.X, caster.Y, target.X, target.Y, FormulaColor(formula), FormulaBeamKind(formula, caster, target.X, target.Y));
                AddTileGlyph(target.X, target.Y, formula, "impact", FormulaColor(formula));
                int dealt = DealDamage(target, damage, formula.DamageType, FormulaColor(formula));
                string resonance = ApplyFormulaStatusResonance(formula, caster, target);
                int heal = Mathf.Max(2, dealt / 2);
                caster.Hp = Mathf.Min(caster.MaxHp, caster.Hp + heal);
                AddFloat(caster.X, caster.Y, "+" + heal, violet);
                PushLog($"{caster.Name} casts {formula.Name}. Life pulls loose from {target.Name}.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
                if (!string.IsNullOrEmpty(resonance)) PushLog(resonance, Tone.Good);
                if (target.Hp <= 0) PushLog($"{target.Name} is down.", Tone.Good);
                if (target.Hp <= 0) AddFlash(target.X, target.Y, gold);
                return true;
            }

            if (formula.Effect == "damage")
            {
                int damage = FormulaDamage(formula, caster, target);
                bool signatureFlourish = formula.Code == "FBL" || formula.Code == "MTR" || formula.Code == "AST";
                if (!signatureFlourish)
                {
                    AddBeam(caster.X, caster.Y, target.X, target.Y, FormulaColor(formula), FormulaBeamKind(formula, caster, target.X, target.Y));
                    AddTileGlyph(target.X, target.Y, formula, formula.Splash ? "area" : "impact", FormulaColor(formula));
                }
                DealDamage(target, damage, formula.DamageType, FormulaColor(formula));
                string statusResonance = ApplyFormulaStatusResonance(formula, caster, target);
                string terrainReaction = ApplyFormulaHitTerrainReaction(formula, caster, target);
                bool statusApplied = false;
                if (!string.IsNullOrEmpty(formula.Status) && target.Hp > 0)
                {
                    statusApplied = TryApplyStatus(target, formula.Status, formula.Duration, caster, 0.42f, true);
                }

                int splashCount = 0;
                int splashReactions = 0;
                if (formula.Splash)
                {
                    CombatImpactProfile profile = CombatImpactRules.ForFormula(formula);
                    foreach (CombatUnit enemy in state.Combat.Units.Where(u => u.Side == UnitSide.Enemy && u.Hp > 0 && u.Id != target.Id).ToList())
                    {
                        if (Distance(enemy.X, enemy.Y, target.X, target.Y) > 1) continue;
                        float sequenceDelay = CombatImpactRules.SequenceImpactDelay(profile, splashCount + 1);
                        float previousDelay = combatVfxImpactDelay;
                        combatVfxImpactDelay = Mathf.Max(previousDelay, sequenceDelay);
                        try
                        {
                            float deliveryStartDelay = Mathf.Max(0f, sequenceDelay - CombatPowerVisualRules.BeamDuration("arc"));
                            AddBeamDelayed(target.X, target.Y, enemy.X, enemy.Y, FormulaColor(formula), "arc", deliveryStartDelay);
                            AddTileGlyph(enemy.X, enemy.Y, formula, "impact", FormulaColor(formula));
                            DealDamage(enemy, Mathf.Max(3, damage / 3 + rng.Next(0, 3)), formula.DamageType, FormulaColor(formula));
                            string splashStatusResonance = ApplyFormulaStatusResonance(formula, caster, enemy);
                            string splashReaction = ApplyFormulaHitTerrainReaction(formula, caster, enemy);
                            if (!string.IsNullOrEmpty(splashStatusResonance) || !string.IsNullOrEmpty(splashReaction)) splashReactions++;
                            splashCount++;
                        }
                        finally
                        {
                            combatVfxImpactDelay = previousDelay;
                        }
                    }
                }

                if (formula.Code == "MTR")
                {
                    AddMeteorShowerFlourish(caster, target, formula);
                }
                else if (formula.Code == "FBL")
                {
                    AddFireballFlourish(caster, target, formula);
                }
                else if (formula.Code == "AST")
                {
                    AddArcaneTempestFlourish(caster, target, formula);
                }

                PushLog($"{caster.Name} casts {formula.Name}. {target.Name} takes the mark.", target.Hp <= 0 ? Tone.Good : Tone.Normal);
                if (!string.IsNullOrEmpty(formula.Status) && target.Hp > 0) PushLog(statusApplied ? $"{target.Name} is {StatusLabel(formula.Status)}." : $"{target.Name} resists the lingering sign.", statusApplied ? Tone.Good : Tone.Warn);
                if (!string.IsNullOrEmpty(statusResonance)) PushLog(statusResonance, Tone.Good);
                if (!string.IsNullOrEmpty(terrainReaction)) PushLog(terrainReaction, Tone.Good);
                if (splashCount > 0) PushLog("The spell spills through nearby foes.", Tone.Good);
                if (splashReactions > 0) PushLog($"{splashReactions} nearby mark{(splashReactions == 1 ? "" : "s")} resonate with the spell.", Tone.Good);
                if (target.Hp <= 0) PushLog($"{target.Name} is down.", Tone.Good);
                if (target.Hp <= 0) AddFlash(target.X, target.Y, gold);
                return true;
            }

            return false;
        }

        private int ResolveThunderStepArrival(FormulaDef formula, CombatUnit caster)
        {
            if (formula == null || caster == null || state?.Combat?.Units == null) return 0;
            List<CombatUnit> enemies = state.Combat.Units
                .Where(unit => unit.Side == UnitSide.Enemy
                    && unit.Hp > 0
                    && Distance(caster.X, caster.Y, unit.X, unit.Y) <= 1)
                .OrderBy(unit => unit.Hp)
                .ThenBy(unit => unit.Id)
                .ToList();
            CombatImpactProfile profile = CombatImpactRules.ForFormula(formula);
            Color color = FormulaColor(formula);
            for (int i = 0; i < enemies.Count; i++)
            {
                CombatUnit enemy = enemies[i];
                float delay = CombatImpactRules.SequenceImpactDelay(profile, i + 1, 0.06f);
                float previousDelay = combatVfxImpactDelay;
                combatVfxImpactDelay = Mathf.Max(previousDelay, delay);
                try
                {
                    float travelStart = Mathf.Max(0f, delay - CombatPowerVisualRules.BeamDuration("lightning"));
                    AddBeamDelayed(caster.X, caster.Y, enemy.X, enemy.Y, color, "lightning", travelStart);
                    AddTileGlyphDelayed(enemy.X, enemy.Y, formula, "impact", color, delay);
                    int damage = LightningPowerRules.ThunderStepDamage(FormulaDamage(formula, caster, enemy));
                    DealDamage(enemy, damage, "shock", color);
                    ApplyFormulaStatusResonance(formula, caster, enemy);
                    ApplyFormulaHitTerrainReaction(formula, caster, enemy);
                    if (enemy.Hp > 0) TryApplyStatus(enemy, "stun", 1, caster, 0.25f, true);
                    if (enemy.Hp <= 0) ReportUnitDown(enemy);
                }
                finally
                {
                    combatVfxImpactDelay = previousDelay;
                }
            }
            return enemies.Count;
        }

        private bool ResolveThunderclap(FormulaDef formula, CombatUnit caster)
        {
            if (formula == null || caster == null || state?.Combat?.Units == null) return false;
            List<CombatUnit> enemies = AdjacentEnemies(caster)
                .OrderBy(unit => unit.X)
                .ThenBy(unit => unit.Y)
                .ThenBy(unit => unit.Id)
                .ToList();
            if (enemies.Count == 0) return false;

            int damage = LightningPowerRules.ThunderclapDamage(FormulaDamage(formula, caster));
            CombatImpactProfile profile = CombatImpactRules.ForFormula(formula);
            Color color = FormulaColor(formula);
            int pushed = 0;
            int collisions = 0;
            AddTileGlyph(caster.X, caster.Y, formula, "area", color);
            AddEpicBurst(caster.X, caster.Y, Color.Lerp(color, frost, 0.48f), 22, 1.48f);

            for (int i = 0; i < enemies.Count; i++)
            {
                CombatUnit enemy = enemies[i];
                float delay = CombatImpactRules.SequenceImpactDelay(profile, i, 0.055f);
                float previousDelay = combatVfxImpactDelay;
                combatVfxImpactDelay = Mathf.Max(previousDelay, delay);
                try
                {
                    float travelStart = Mathf.Max(0f, delay - CombatPowerVisualRules.BeamDuration("lightning"));
                    AddBeamDelayed(caster.X, caster.Y, enemy.X, enemy.Y, color, "lightning", travelStart);
                    AddTileGlyphDelayed(enemy.X, enemy.Y, formula, "impact", color, delay);
                    DealDamage(enemy, damage, "shock", color);
                    ApplyFormulaStatusResonance(formula, caster, enemy);
                    ApplyFormulaHitTerrainReaction(formula, caster, enemy);
                    if (enemy.Hp <= 0)
                    {
                        ReportUnitDown(enemy);
                        continue;
                    }

                    string collision;
                    if (TryPushCombatUnitAway(caster, enemy, out collision))
                    {
                        pushed++;
                        TryApplyStatus(enemy, "stun", 1, caster, 0.35f, true);
                        AddFloat(enemy.X, enemy.Y, "pushed", gold);
                    }
                    else
                    {
                        int collisionDamage = DealDamage(enemy, LightningPowerRules.CollisionDamage(damage), "physical", stone);
                        enemy.Stunned = Mathf.Max(enemy.Stunned, 1);
                        collisions++;
                        RecordCombatPowerReaction("Thunder collision");
                        AddFloat(enemy.X, enemy.Y, "COLLISION", gold);
                        AddBurst(enemy.X, enemy.Y, color);
                        PushLog($"{enemy.Name} slams into {collision} for {collisionDamage} physical.", enemy.Hp <= 0 ? Tone.Good : Tone.Warn);
                        if (enemy.Hp <= 0) ReportUnitDown(enemy);
                    }
                }
                finally
                {
                    combatVfxImpactDelay = previousDelay;
                }
            }

            PushLog(
                $"{caster.Name}'s Thunderclap shocks {enemies.Count} {(enemies.Count == 1 ? "enemy" : "enemies")}, pushes {pushed}, and causes {collisions} {(collisions == 1 ? "collision" : "collisions")}.",
                Tone.Good);
            return true;
        }

        private bool ResolveChainLightning(FormulaDef formula, CombatUnit caster, CombatUnit target)
        {
            List<CombatUnit> chain = BuildLightningChain(target);
            if (formula == null || caster == null || chain.Count == 0) return false;

            int baseDamage = FormulaDamage(formula, caster, target);
            CombatImpactProfile profile = CombatImpactRules.ForFormula(formula);
            Color color = FormulaColor(formula);
            CombatUnit previous = null;
            int defeated = 0;
            for (int i = 0; i < chain.Count; i++)
            {
                CombatUnit enemy = chain[i];
                float delay = CombatImpactRules.SequenceImpactDelay(profile, i, 0.07f);
                float previousDelay = combatVfxImpactDelay;
                combatVfxImpactDelay = Mathf.Max(previousDelay, delay);
                try
                {
                    int fromX = previous == null ? caster.X : previous.X;
                    int fromY = previous == null ? caster.Y : previous.Y;
                    float travelStart = Mathf.Max(0f, delay - CombatPowerVisualRules.BeamDuration("lightning"));
                    AddBeamDelayed(fromX, fromY, enemy.X, enemy.Y, color, "lightning", travelStart);
                    AddTileGlyphDelayed(enemy.X, enemy.Y, formula, "impact", color, delay);
                    int damage = LightningPowerRules.ChainDamage(baseDamage, i);
                    DealDamage(enemy, damage, "shock", color);
                    string resonance = ApplyFormulaStatusResonance(formula, caster, enemy);
                    string terrain = ApplyFormulaHitTerrainReaction(formula, caster, enemy);
                    if (!string.IsNullOrEmpty(resonance) || !string.IsNullOrEmpty(terrain)) RecordCombatPowerReaction("Chain resonance");
                    AddFloat(enemy.X, enemy.Y, i == 0 ? "ARC" : $"JUMP {i + 1}", color);
                    if (enemy.Hp <= 0)
                    {
                        defeated++;
                        ReportUnitDown(enemy);
                    }
                }
                finally
                {
                    combatVfxImpactDelay = previousDelay;
                }
                previous = enemy;
            }

            PushLog(
                $"{caster.Name}'s Chain Lightning strikes {chain.Count} {(chain.Count == 1 ? "enemy" : "enemies")}{(defeated > 0 ? $" and drops {defeated}" : "")}.",
                defeated > 0 ? Tone.Good : Tone.Normal);
            return true;
        }

        private List<CombatUnit> BuildLightningChain(CombatUnit target)
        {
            List<CombatUnit> chain = new List<CombatUnit>();
            if (target == null || target.Side != UnitSide.Enemy || target.Hp <= 0 || state?.Combat?.Units == null) return chain;
            chain.Add(target);
            while (chain.Count < LightningPowerRules.MaximumChainTargets)
            {
                CombatUnit origin = chain[chain.Count - 1];
                Point terrain = ObstacleAt(origin.X, origin.Y);
                int jumpRange = LightningPowerRules.IsConductiveTerrain(terrain?.Kind)
                    ? LightningPowerRules.ConductiveJumpRange
                    : LightningPowerRules.NormalJumpRange;
                CombatUnit next = state.Combat.Units
                    .Where(unit => unit.Side == UnitSide.Enemy
                        && unit.Hp > 0
                        && !chain.Any(visited => visited.Id == unit.Id)
                        && Distance(origin.X, origin.Y, unit.X, unit.Y) <= jumpRange)
                    .OrderBy(unit => Distance(origin.X, origin.Y, unit.X, unit.Y))
                    .ThenBy(unit => (float)unit.Hp / Mathf.Max(1, unit.MaxHp))
                    .ThenBy(unit => unit.Id)
                    .FirstOrDefault();
                if (next == null) break;
                chain.Add(next);
            }
            return chain;
        }

        private bool ResolveArcaneTempest(FormulaDef formula, CombatUnit caster, CombatUnit target)
        {
            if (formula == null || caster == null || target == null || state?.Combat?.Units == null) return false;
            List<CombatUnit> enemies = state.Combat.Units
                .Where(unit => unit.Side == UnitSide.Enemy
                    && unit.Hp > 0
                    && Distance(target.X, target.Y, unit.X, unit.Y) <= LightningPowerRules.TempestRadius)
                .OrderBy(unit => unit.Id == target.Id ? 0 : 1)
                .ThenBy(unit => Distance(target.X, target.Y, unit.X, unit.Y))
                .ThenBy(unit => (float)unit.Hp / Mathf.Max(1, unit.MaxHp))
                .ThenBy(unit => unit.Id)
                .ToList();
            if (enemies.Count == 0) return false;

            int baseDamage = FormulaDamage(formula, caster, target);
            CombatImpactProfile profile = CombatImpactRules.ForFormula(formula);
            Color color = FormulaColor(formula);
            int stunned = 0;
            int defeated = 0;
            for (int i = 0; i < enemies.Count; i++)
            {
                CombatUnit enemy = enemies[i];
                bool center = enemy.Id == target.Id;
                float delay = CombatImpactRules.SequenceImpactDelay(profile, i, 0.065f);
                float previousDelay = combatVfxImpactDelay;
                combatVfxImpactDelay = Mathf.Max(previousDelay, delay);
                try
                {
                    int fromX = center ? caster.X : enemy.X;
                    int fromY = center || enemy.Y <= 0 ? caster.Y : 0;
                    float travelStart = Mathf.Max(0f, delay - CombatPowerVisualRules.BeamDuration("lightning"));
                    AddBeamDelayed(fromX, fromY, enemy.X, enemy.Y, color, "lightning", travelStart);
                    AddTileGlyphDelayed(enemy.X, enemy.Y, formula, center ? "area" : "impact", color, delay);
                    int damage = LightningPowerRules.TempestDamage(baseDamage, center);
                    DealDamage(enemy, damage, "shock", color);
                    ApplyFormulaStatusResonance(formula, caster, enemy);
                    ApplyFormulaHitTerrainReaction(formula, caster, enemy);
                    if (enemy.Hp > 0 && TryApplyStatus(enemy, "stun", 1, caster, 0.35f, true)) stunned++;
                    if (enemy.Hp <= 0)
                    {
                        defeated++;
                        ReportUnitDown(enemy);
                    }
                }
                finally
                {
                    combatVfxImpactDelay = previousDelay;
                }
            }

            AddEpicBurst(target.X, target.Y, Color.Lerp(color, frost, 0.58f), 30, 1.82f);
            AddFlash(target.X, target.Y, color);
            AddFloat(target.X, target.Y, "TEMPEST", color);
            PushLog(
                $"{caster.Name}'s Arcane Tempest strikes {enemies.Count} {(enemies.Count == 1 ? "enemy" : "enemies")}, stuns {stunned}{(defeated > 0 ? $", and drops {defeated}" : "")}.",
                defeated > 0 ? Tone.Good : Tone.Normal);
            return true;
        }

        private bool TryPushCombatUnitAway(CombatUnit source, CombatUnit target, out string collision)
        {
            collision = "the battlefield edge";
            if (source == null || target == null) return false;
            int dx = Math.Sign(target.X - source.X);
            int dy = Math.Sign(target.Y - source.Y);
            if (dx != 0 && dy != 0)
            {
                if (Mathf.Abs(target.X - source.X) >= Mathf.Abs(target.Y - source.Y)) dy = 0;
                else dx = 0;
            }
            if (dx == 0 && dy == 0) return false;

            int nextX = target.X + dx;
            int nextY = target.Y + dy;
            if (nextX < 0 || nextX >= CombatW || nextY < 0 || nextY >= CombatH) return false;
            Point obstacle = ObstacleAt(nextX, nextY);
            if (IsBlockingTerrain(obstacle))
            {
                collision = TerrainDescription(obstacle.Kind).TrimEnd('.');
                return false;
            }
            CombatUnit blocker = UnitAt(nextX, nextY);
            if (blocker != null)
            {
                collision = blocker.Name;
                return false;
            }

            Vector2 from = new Vector2(target.X, target.Y);
            target.X = nextX;
            target.Y = nextY;
            AddTween(target.Id, from, new Vector2(nextX, nextY), TweenKind.Move);
            return true;
        }

        private void AddFireballFlourish(CombatUnit caster, CombatUnit target, FormulaDef formula)
        {
            if (caster == null || target == null || formula == null) return;
            Color color = FormulaColor(formula);
            AddBeam(caster.X, caster.Y, target.X, target.Y, color, "fireball");
        }

        private void AddMeteorShowerFlourish(CombatUnit caster, CombatUnit target, FormulaDef formula)
        {
            if (caster == null || target == null || formula == null) return;
            Color color = FormulaColor(formula);
            CombatImpactProfile profile = CombatImpactRules.ForFormula(formula);
            int[][] offsets =
            {
                new[] { 0, 0 },
                new[] { -1, 0 },
                new[] { 1, 0 },
                new[] { 0, -1 },
                new[] { 0, 1 }
            };

            for (int i = 0; i < offsets.Length; i++)
            {
                int tx = Mathf.Clamp(target.X + offsets[i][0], 0, CombatW - 1);
                int ty = Mathf.Clamp(target.Y + offsets[i][1], 0, CombatH - 1);
                float impactDelay = CombatImpactRules.SequenceImpactDelay(profile, i);
                string deliveryKind = i == 0 ? "meteor" : "meteor-small";
                float travelDelay = Mathf.Max(0f, impactDelay - CombatPowerVisualRules.BeamDuration(deliveryKind));
                AddBeamDelayed(Mathf.Clamp(tx - 2, 0, CombatW - 1), 0, tx, ty, color, deliveryKind, travelDelay);
            }
            AddFloat(target.X, target.Y, "meteor", color);
        }

        private void AddArcaneTempestFlourish(CombatUnit caster, CombatUnit target, FormulaDef formula)
        {
            if (caster == null || target == null || formula == null) return;
            Color color = FormulaColor(formula);
            CombatImpactProfile profile = CombatImpactRules.ForFormula(formula);
            AddBeam(caster.X, caster.Y, target.X, target.Y, color, "arc");

            int[][] offsets =
            {
                new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, -1 }, new[] { 0, 1 }
            };
            for (int i = 0; i < offsets.Length; i++)
            {
                int tx = target.X + offsets[i][0];
                int ty = target.Y + offsets[i][1];
                if (tx < 0 || tx >= CombatW || ty < 0 || ty >= CombatH) continue;
                float delay = CombatImpactRules.SequenceImpactDelay(profile, i + 1);
                float deliveryStartDelay = Mathf.Max(0f, delay - CombatPowerVisualRules.BeamDuration("arc"));
                AddBeamDelayed(target.X, target.Y, tx, ty, Color.Lerp(color, gold, 0.22f), "arc", deliveryStartDelay);
            }
            AddFloat(target.X, target.Y, "TEMPEST", color);
        }

        private CombatUnit MakeSummonedUnit(FormulaDef formula, CombatUnit caster, int x, int y)
        {
            string role = string.IsNullOrWhiteSpace(formula.SummonRole) ? "boundimp" : formula.SummonRole;
            int skill = Mathf.Max(1, SkillValue(caster.Skills, FormulaSkill(formula, caster)));
            int focus = IsFocusedCaster(caster) ? 1 : 0;
            int tierDefense = role == "greaterdemon" ? 4 : role == "lesserdemon" ? 2 : 1;
            int tierSpeed = role == "greaterdemon" ? 8 : role == "lesserdemon" ? 10 : 12;
            int hp = SummonPreviewHp(formula, caster);
            int power = Mathf.Max(3, formula.Power + skill / 5 + FormulaStatPowerBonus(formula, caster) + focus);
            string displayName = role == "boundimp" ? "Bound Imp" : role == "greaterdemon" ? "Greater Demon" : "Lesser Demon";
            return new CombatUnit
            {
                Id = Guid.NewGuid().ToString("N"),
                PartyIndex = -1,
                Side = UnitSide.Party,
                Name = displayName,
                Role = role,
                Race = "demon",
                ClassKey = "summon",
                Rank = "summoned",
                Origin = "pact",
                Sigil = "flame",
                X = x,
                Y = y,
                Hp = hp,
                MaxHp = hp,
                Mana = 0,
                MaxMana = 0,
                Movement = role == "greaterdemon" ? 3 : 4,
                Power = power,
                Defense = tierDefense + skill / 12,
                Agility = 4 + skill / 9,
                Range = 1,
                AttackSpeed = tierSpeed + skill / 6,
                DamageMin = Mathf.Max(2, power - 3),
                DamageMax = SummonPreviewMaxDamage(formula, power),
                Spell = "",
                Skills = new SkillSet { Arms = Mathf.Max(5, skill / 2), Guard = 2 }.Normalize(),
                Color = Hex("b94b56").ToHex(),
                DamageType = "death",
                WeaponName = "pact claws",
                WeaponBonus = 0,
                ArmorName = "bound hide",
                ArmorBonus = 0,
                Resist = role == "greaterdemon" ? "death|mind|fire" : "death|mind",
                Weakness = "light",
                StatusOnHit = role == "boundimp" ? "hex" : role == "lesserdemon" ? "bleed" : "stun",
                MagicResist = role == "greaterdemon" ? 4 : role == "lesserdemon" ? 3 : 2,
                Fearless = true,
                Summoned = true,
                SummonTurns = Mathf.Max(1, formula.Duration + focus),
                SummonerId = caster.Id
            };
        }

        private int SummonPreviewHp(FormulaDef formula, CombatUnit caster)
        {
            if (formula == null) return 1;
            string role = string.IsNullOrWhiteSpace(formula.SummonRole) ? "boundimp" : formula.SummonRole.ToLowerInvariant();
            int skill = Mathf.Max(1, SkillValue(caster?.Skills, FormulaSkill(formula, caster)));
            int focus = IsFocusedCaster(caster) ? 1 : 0;
            int tierHp = role == "greaterdemon" ? 24 : role == "lesserdemon" ? 15 : 8;
            return tierHp + (state?.Depth ?? 1) + skill / 3 + FormulaStatPowerBonus(formula, caster) * 2 + focus * 2;
        }

        private int SummonPreviewMaxDamage(FormulaDef formula, int power)
        {
            string role = string.IsNullOrWhiteSpace(formula?.SummonRole) ? "boundimp" : formula.SummonRole.ToLowerInvariant();
            int extra = role == "greaterdemon" ? 4 : role == "lesserdemon" ? 2 : 1;
            return Mathf.Max(4, power + extra);
        }

        private int FormulaDamage(FormulaDef formula, CombatUnit caster, CombatUnit target = null)
        {
            string skill = FormulaSkill(formula, caster);
            int skillBonus = SkillValue(caster.Skills, skill) / 2;
            int focusBonus = IsFocusedCaster(caster) ? 3 : 0;
            int resonanceBonus = FormulaStatusResonanceDamageBonus(formula, target);
            return Mathf.Max(1, formula.Power + skillBonus + FormulaStatPowerBonus(formula, caster) + focusBonus + RaceFormulaPowerBonus(caster, formula) + DemonFormFormulaPowerBonus(caster, formula) + resonanceBonus + rng.Next(0, 6));
        }

        private int FormulaStatusResonanceDamageBonus(FormulaDef formula, CombatUnit target)
        {
            if (formula == null || target == null || target.Hp <= 0) return 0;
            string type = string.IsNullOrEmpty(formula.DamageType) ? "" : formula.DamageType;
            int bonus = 0;
            if (type == "fire")
            {
                if (target.Webbed > 0) bonus += 3;
                if (target.Bleeding > 0 || target.Poisoned > 0) bonus += 2;
            }
            else if (type == "shock")
            {
                if (target.Webbed > 0 || target.Stunned > 0) bonus += 3;
                if (target.Shielded > 0) bonus += 2;
            }
            else if (type == "cold")
            {
                if (target.Bleeding > 0) bonus += 3;
                if (target.Poisoned > 0) bonus += 1;
            }
            else if (type == "poison")
            {
                if (target.Bleeding > 0 || target.Webbed > 0) bonus += 2;
            }
            else if (type == "death" || type == "mind")
            {
                if (target.Hexed > 0) bonus += 4;
                if (target.Sleeping > 0) bonus += 2;
            }
            else if (type == "light")
            {
                if (target.Hexed > 0) bonus += 3;
                if (target.Poisoned > 0) bonus += 1;
            }
            return Mathf.Clamp(bonus, 0, 6);
        }

        private string ApplyFormulaStatusResonance(FormulaDef formula, CombatUnit caster, CombatUnit target)
        {
            if (formula == null || target == null || target.Hp <= 0) return "";
            string type = string.IsNullOrEmpty(formula.DamageType) ? "" : formula.DamageType;
            if (type == "fire" && target.Webbed > 0)
            {
                RecordCombatPowerReaction("Web flare");
                target.Webbed = 0;
                AddFloat(target.X, target.Y, "web burns", ember);
                AddBurst(target.X, target.Y, ember);
                return $"{formula.Name} burns away {target.Name}'s webbing.";
            }
            if (type == "shock" && target.Webbed > 0)
            {
                RecordCombatPowerReaction("Conductive stun");
                TryApplyStatus(target, "stun", 1, caster, 0.62f, target.Side != caster.Side);
                AddFloat(target.X, target.Y, "conduct", gold);
                return $"{formula.Name} conducts through the webbing.";
            }
            if (type == "shock" && target.Shielded > 0)
            {
                RecordCombatPowerReaction("Ward crack");
                target.Shielded = Mathf.Max(0, target.Shielded - 1);
                AddFloat(target.X, target.Y, "ward crack", gold);
                return $"{formula.Name} cracks {target.Name}'s ward.";
            }
            if (type == "cold" && target.Bleeding > 0)
            {
                RecordCombatPowerReaction("Frostbind");
                target.Bleeding = Mathf.Max(0, target.Bleeding - 1);
                TryApplyStatus(target, "web", 1, caster, 0.42f, target.Side != caster.Side);
                AddFloat(target.X, target.Y, "frostbind", frost);
                return $"{formula.Name} seals the wound into a slowing frostbind.";
            }
            if ((type == "death" || type == "mind") && target.Hexed > 0)
            {
                RecordCombatPowerReaction("Doom echo");
                target.Hexed = Mathf.Max(target.Hexed, 2);
                AddFloat(target.X, target.Y, "doom echo", violet);
                AddBurst(target.X, target.Y, violet);
                return $"{formula.Name} echoes through the existing hex.";
            }
            if (type == "light" && target.Hexed > 0)
            {
                RecordCombatPowerReaction("Hex seared");
                target.Hexed = Mathf.Max(0, target.Hexed - 1);
                AddFloat(target.X, target.Y, "hex seared", teal);
                return $"{formula.Name} sears part of the hex away.";
            }
            return "";
        }

        private string FormulaStatusResonancePreview(FormulaDef formula, CombatUnit target)
        {
            if (formula == null || target == null) return "";
            string type = string.IsNullOrEmpty(formula.DamageType) ? "" : formula.DamageType;
            List<string> notes = new List<string>();
            int bonus = FormulaStatusResonanceDamageBonus(formula, target);
            if (bonus > 0) notes.Add($"+{bonus} setup damage");
            if (type == "fire" && target.Webbed > 0) notes.Add("burns webbed free");
            if (type == "shock" && target.Webbed > 0) notes.Add("conducts; stun chance");
            if (type == "shock" && target.Shielded > 0) notes.Add("cracks ward");
            if (type == "cold" && target.Bleeding > 0) notes.Add("frostbind chance");
            if ((type == "death" || type == "mind") && target.Hexed > 0) notes.Add("doom echo");
            if (type == "light" && target.Hexed > 0) notes.Add("sears hex");
            return notes.Count == 0 ? "" : string.Join(" / ", notes.Take(3).ToArray());
        }

        private bool IsKnownFormula(string code)
        {
            return GetFormula(code) != null;
        }

        private string SpellFloatLabel(FormulaDef formula)
        {
            if (formula == null || string.IsNullOrEmpty(formula.Name)) return "spell";
            string first = formula.Name.Split(' ')[0];
            return first.Length <= 8 ? first : first.Substring(0, 8);
        }

        private bool IsFocusedCaster(CombatUnit caster)
        {
            return caster != null
                && state?.Combat != null
                && caster.Side == UnitSide.Party
                && state.Combat.ActionAvailable
                && !state.Combat.Moved
                && state.Combat.MovePoints >= UnitMoveAllowance(caster)
                && !string.IsNullOrEmpty(caster.Spell);
        }

        private int UnitMoveAllowance(CombatUnit unit)
        {
            if (unit == null) return CombatMoveAllowance;
            return Mathf.Clamp(unit.Movement > 0 ? unit.Movement : CombatMoveAllowance, 2, 5);
        }

        private PartyMember PartyMemberForUnit(CombatUnit unit)
        {
            if (unit == null || unit.Side != UnitSide.Party || unit.Summoned || state?.Party == null) return null;
            if (unit.PartyIndex < 0 || unit.PartyIndex >= state.Party.Count) return null;
            return state.Party[unit.PartyIndex];
        }

        private int UnitStrengthScore(CombatUnit unit)
        {
            PartyMember member = PartyMemberForUnit(unit);
            if (member != null) return EffectiveStrength(member);
            return Mathf.Max(1, (unit?.Power ?? 1) + (unit?.DamageMax ?? 0));
        }

        private int UnitIntelligenceScore(CombatUnit unit)
        {
            PartyMember member = PartyMemberForUnit(unit);
            if (member != null) return EffectiveIntelligence(member);
            if (unit == null) return 1;
            if (unit.MaxMana > 0) return Mathf.Max(1, unit.MaxMana - 8);
            if (IsCasterEnemy(unit)) return Mathf.Max(8, 10 + unit.Power / 2 + unit.MagicResist * 2);
            return Mathf.Max(1, unit.Power / 2 + unit.MagicResist);
        }

        private int UnitAgilityScore(CombatUnit unit)
        {
            PartyMember member = PartyMemberForUnit(unit);
            if (member != null) return EffectiveAgility(member);
            return Mathf.Max(1, unit?.Agility ?? 1);
        }

        private int FormulaStatPowerBonus(FormulaDef formula, CombatUnit caster)
        {
            if (formula == null || caster == null) return 0;
            int intelligence = UnitIntelligenceScore(caster);
            int baseBonus = Mathf.Max(0, (intelligence - 10) / 4);
            if (formula.Effect == "damage"
                || formula.Effect == "drain"
                || formula.Effect == "chain"
                || formula.Effect == "tempest"
                || formula.Effect == "thunderclap")
            {
                return baseBonus + Mathf.Max(0, FormulaTier(formula) - 1);
            }
            if (formula.Effect == "heal") return baseBonus;
            if (formula.Effect == "summon") return Mathf.Max(0, (intelligence - 10) / 5);
            return 0;
        }

        private string FormulaStatNote(FormulaDef formula, CombatUnit caster)
        {
            int bonus = FormulaStatPowerBonus(formula, caster);
            return bonus > 0 ? $" / INT +{bonus}" : "";
        }

        private float FormulaStatusStatChanceBonus(CombatUnit source)
        {
            if (source == null) return 0f;
            return Mathf.Clamp((UnitIntelligenceScore(source) - 10) * 0.006f, 0f, 0.12f);
        }

        private int RaceHitBonus(CombatUnit unit)
        {
            if (unit == null || unit.Side != UnitSide.Party) return 0;
            if (string.Equals(unit.Race, "dusk elf", StringComparison.OrdinalIgnoreCase)) return 5;
            if (string.Equals(unit.Race, "fenkin", StringComparison.OrdinalIgnoreCase) && unit.Range > 1) return 2;
            return 0;
        }

        private int RaceDamageReduction(CombatUnit target, string damageType)
        {
            if (target == null || target.Side != UnitSide.Party) return 0;
            if (string.Equals(target.Race, "stoneborn", StringComparison.OrdinalIgnoreCase) && (string.IsNullOrEmpty(damageType) || damageType == "physical")) return 1;
            if (string.Equals(target.Race, "ashling", StringComparison.OrdinalIgnoreCase) && damageType == "fire") return 1;
            return 0;
        }

        private int RaceFormulaPowerBonus(CombatUnit caster, FormulaDef formula)
        {
            if (caster == null || formula == null || caster.Side != UnitSide.Party) return 0;
            if (string.Equals(caster.Race, "ashling", StringComparison.OrdinalIgnoreCase) && (formula.DamageType == "fire" || formula.Terrain == "fire")) return 2;
            if (string.Equals(caster.Race, "fenkin", StringComparison.OrdinalIgnoreCase) && (formula.Terrain == "web" || formula.Terrain == "gas" || formula.DamageType == "poison")) return 1;
            return 0;
        }

        private int DemonFormAttackBonus(CombatUnit unit)
        {
            return unit != null && unit.DemonFormTurns > 0 ? 4 : 0;
        }

        private int DemonFormFormulaPowerBonus(CombatUnit caster, FormulaDef formula)
        {
            if (caster == null || caster.DemonFormTurns <= 0 || formula == null) return 0;
            bool pactPower = (formula.School ?? "").Contains("pact")
                || formula.DamageType == "death"
                || formula.DamageType == "mind";
            return pactPower ? 4 : 2;
        }

        private int DemonFormDamageReduction(CombatUnit target)
        {
            return target != null && target.DemonFormTurns > 0 ? 2 : 0;
        }

        private int EffectiveFormulaMana(FormulaDef formula, CombatUnit caster)
        {
            if (formula == null) return 0;
            return Mathf.Max(1, formula.Mana - (IsFocusedCaster(caster) ? 1 : 0));
        }

        private int EffectiveFormulaRange(FormulaDef formula, CombatUnit caster)
        {
            if (formula == null) return 0;
            return formula.Range + (IsFocusedCaster(caster) ? 1 : 0);
        }

        private string FormulaFocusNote(FormulaDef formula, CombatUnit caster)
        {
            if (!IsFocusedCaster(caster) || formula == null) return "";
            return $" / focused: {EffectiveFormulaMana(formula, caster)} MP, +1 range";
        }

        private bool CanUseFormula(CombatUnit caster, string code, out string reason)
        {
            reason = "";
            FormulaDef formula = GetFormula(code);
            if (caster == null)
            {
                reason = "No caster is ready.";
                return false;
            }
            if (string.IsNullOrEmpty(caster.Spell))
            {
                reason = $"{caster.Name} has no spell craft.";
                return false;
            }
            if (formula == null)
            {
                reason = code + " has no stable shape.";
                return false;
            }
            if (!SchoolMatches(formula, caster.Spell))
            {
                reason = $"{formula.Name} needs a different craft.";
                return false;
            }
            if (caster.Level < FormulaRequiredLevel(formula))
            {
                reason = $"{formula.Name} unlocks at level {FormulaRequiredLevel(formula)}.";
                return false;
            }
            return true;
        }

        private bool CastEmptyTile(CombatUnit caster, int x, int y)
        {
            if (string.IsNullOrEmpty(caster.Spell))
            {
                PushLog($"{caster.Name} knows no battle spell.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }
            if (!CasterKnowsSchool(caster.Spell, "mend"))
            {
                PushLog("That spell needs a living mark.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }
            if (Distance(caster.X, caster.Y, x, y) > 4)
            {
                PushLog("Tree Cover cannot reach that far.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }
            if (!CanGrowTreeAt(x, y))
            {
                PushLog("Tree Cover needs an open tile.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }
            if (caster.Mana < 7)
            {
                PushLog($"{caster.Name} lacks mana.", Tone.Warn);
                PlaySfx("blocked");
                return false;
            }

            caster.Mana -= 7;
            state.Combat.Obstacles.RemoveAll(o => o.X == x && o.Y == y);
            state.Combat.Obstacles.Add(new Point(x, y, "tree", FieldDurationRounds("tree", SummonedTreeDuration)));
            ImproveSkill(caster, "mend", 2);
            AddFloat(x, y, "Tree", moss);
            AddBurst(x, y, moss);
            PushLog($"{caster.Name} casts Tree Cover. Cover rises for {SummonedTreeDuration} rounds.", Tone.Good);
            PlaySfx("tree");
            return true;
        }

        private FormulaDef GetFormula(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;
            FormulaDef formula = formulaBook.FirstOrDefault(f => f.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            return formula != null && ContentSetCatalog.FormulaActive(activeContentSet, formula.Code) ? formula : null;
        }

        private IEnumerable<FormulaDef> KnownFormulasFor(CombatUnit caster)
        {
            if (caster == null || string.IsNullOrEmpty(caster.Spell)) return Enumerable.Empty<FormulaDef>();
            IEnumerable<FormulaDef> known = ActiveFormulaBook().Where(f => SchoolMatches(f, caster.Spell) && caster.Level >= FormulaRequiredLevel(f));
            if (string.Equals(caster.ClassKey, "warlock", StringComparison.OrdinalIgnoreCase) && CasterKnowsSchool(caster.Spell, "pact"))
            {
                return known
                    .OrderBy(f => SchoolMatches(f, "pact") ? 0 : 1)
                    .ThenBy(PactSpellbookSort)
                    .ThenBy(FormulaRequiredLevel)
                    .ThenBy(FormulaBookIndex);
            }
            return known;
        }

        private IEnumerable<FormulaDef> ActiveFormulaBook()
        {
            return formulaBook.Where(f => f != null && ContentSetCatalog.FormulaActive(activeContentSet, f.Code));
        }

        private int PactSpellbookSort(FormulaDef formula)
        {
            switch (formula?.Code ?? "")
            {
                case "IBD": return 0;
                case "IBF": return 1;
                case "IBG": return 2;
                case "PBR": return 3;
                default: return 10;
            }
        }

        private int FormulaBookIndex(FormulaDef formula)
        {
            if (formula == null) return 9999;
            for (int i = 0; i < formulaBook.Length; i++)
            {
                if (ReferenceEquals(formulaBook[i], formula) || formulaBook[i].Code == formula.Code) return i;
            }
            return 9999;
        }

        private bool SchoolMatches(FormulaDef formula, string school)
        {
            if (formula == null || string.IsNullOrEmpty(school)) return false;
            string[] formulaSchools = formula.School.Split('|');
            string[] casterSchools = school.Split('|');
            return formulaSchools.Any(f => casterSchools.Any(c => f.Equals(c, StringComparison.OrdinalIgnoreCase)));
        }

        private string FormulaSkill(FormulaDef formula, CombatUnit caster)
        {
            if (formula.Code == "RLM" && caster != null && CasterKnowsSchool(caster.Spell, "ember") && SkillValue(caster.Skills, "ember") >= SkillValue(caster.Skills, "hex")) return "ember";
            if (formula.Code == "SRF" && caster != null && !CasterKnowsSchool(caster.Spell, "mend") && CasterKnowsSchool(caster.Spell, "ember")) return "ember";
            return string.IsNullOrEmpty(formula.Skill) ? caster?.Spell ?? "arms" : formula.Skill;
        }

        private string FormulaCodexLine(CombatUnit active)
        {
            if (active == null || string.IsNullOrEmpty(active.Spell)) return "No spell craft. Use Cast for trained spellcasters.";
            if (selectedAction != ActionMode.Cast) return "Press 3 or Cast to open the Spellbook. I opens Armory; C opens Spell Reference.";
            if (!string.IsNullOrEmpty(pendingFormulaCode)) return "Spell selected. Click a highlighted target; Esc or right-click cancels.";
            return "Choose a spell card, then click a highlighted target.";
        }

        private bool CanTargetFormula(FormulaDef formula, CombatUnit caster, CombatUnit target, int x, int y)
        {
            if (formula == null || caster == null) return false;
            if (formula.Effect == "summon") return CanSummonFormulaAt(formula, caster, x, y, out _);
            if (formula.Effect == "teleport") return CanStandAt(x, y);
            if (formula.Effect == "dispel") return CombatRitualRules.IsDispelableField(ObstacleAt(x, y));
            if (formula.Target == "tile") return CanPlaceTerrainAt(formula, x, y);
            if (formula.Target == "ally") return target != null && target.Side == UnitSide.Party;
            if (formula.Target == "enemy") return target != null && target.Side == UnitSide.Enemy;
            if (formula.Target == "self") return target != null && target.Id == caster.Id;
            return false;
        }

        private bool CanSummonAt(int x, int y)
        {
            return CanStandAt(x, y);
        }

        private bool CanSummonFormulaAt(FormulaDef formula, CombatUnit caster, int x, int y, out string reason)
        {
            reason = "";
            if (formula == null || caster == null)
            {
                reason = "That pact has no stable caster.";
                return false;
            }
            if (!CanSummonAt(x, y))
            {
                reason = $"{formula.Name} needs an open tile.";
                return false;
            }

            int burden = SummonBurden(formula.SummonRole);
            int current = ActiveSummonBurdenFor(caster);
            int max = MaxPactSummonBurdenFor(caster);
            if (current + burden > max)
            {
                reason = $"{caster.Name}'s pact is full ({current}/{max}). End a binding before calling {SummonDisplayName(formula.SummonRole)}.";
                return false;
            }
            return true;
        }

        private int ActiveSummonBurdenFor(CombatUnit caster)
        {
            if (caster == null || state?.Combat?.Units == null) return 0;
            return state.Combat.Units
                .Where(u => u.Summoned && u.Hp > 0 && u.SummonerId == caster.Id)
                .Sum(u => SummonBurden(u.Role));
        }

        private int MaxPactSummonBurdenFor(CombatUnit caster)
        {
            int hex = caster == null ? 0 : SkillValue(caster.Skills, "hex");
            int levelBonus = caster == null ? 0 : Mathf.Max(0, caster.Level - 3) / 3;
            int skillBonus = hex >= 22 ? 2 : hex >= 14 ? 1 : 0;
            return Mathf.Clamp(BasePactSummonBurden + skillBonus + levelBonus, 3, 6);
        }

        private int SummonBurden(string role)
        {
            role = (role ?? "").ToLowerInvariant();
            if (role == "greaterdemon") return 3;
            if (role == "lesserdemon") return 2;
            return 1;
        }

        private bool IsFormulaActionable(FormulaDef formula, CombatUnit caster, CombatUnit target, int x, int y)
        {
            if (formula == null || caster == null) return false;
            if (Distance(caster.X, caster.Y, x, y) > EffectiveFormulaRange(formula, caster)) return false;
            if (!CanTargetFormula(formula, caster, target, x, y)) return false;
            if (!HasFormulaLineOfSight(formula, caster, x, y)) return false;
            return caster.Mana >= EffectiveFormulaMana(formula, caster);
        }

        private bool FormulaRequiresLineOfSight(FormulaDef formula)
        {
            if (formula == null || FormulaArcsOverCover(formula)) return false;
            return FormulaBaseRequiresLineOfSight(formula);
        }

        private bool FormulaBaseRequiresLineOfSight(FormulaDef formula)
        {
            if (formula == null) return false;
            if (formula.Target == "enemy") return true;
            if (formula.Target == "tile") return true;
            return false;
        }

        private bool FormulaArcsOverCover(FormulaDef formula)
        {
            return formula != null && (formula.Arc || (formula.Target == "enemy" && formula.Splash));
        }

        private bool HasFormulaLineOfSight(FormulaDef formula, CombatUnit caster, int x, int y)
        {
            if (caster == null) return true;
            if (FormulaCanArcOverCover(formula, x, y)) return true;
            if (!FormulaBaseRequiresLineOfSight(formula)) return true;
            return HasLineOfSight(caster.X, caster.Y, x, y, true);
        }

        private bool FormulaCanArcOverCover(FormulaDef formula, int x, int y)
        {
            if (!FormulaArcsOverCover(formula)) return false;
            if (formula.Code == "BTF")
            {
                Point existing = ObstacleAt(x, y);
                return IsBreakableCover(existing);
            }
            return true;
        }

        private string FormulaSightBlockText(FormulaDef formula)
        {
            if (formula != null && formula.Target == "tile") return "Cover hides that tile from the spell.";
            return "Cover breaks the spell line.";
        }

        private string FormulaTargetPrompt(FormulaDef formula)
        {
            if (formula == null) return "That spell has no target.";
            if (formula.Effect == "summon") return $"{formula.Name} needs an open tile and enough pact room.";
            if (formula.Effect == "teleport") return $"{formula.Name} needs an open destination tile.";
            if (formula.Effect == "dispel") return $"{formula.Name} needs a ritual or hostile magical field.";
            if (formula.Target == "tile") return $"{formula.Name} needs an open tile.";
            if (formula.Target == "ally") return $"{formula.Name} needs an ally.";
            if (formula.Target == "enemy") return $"{formula.Name} needs an enemy mark.";
            if (formula.Target == "self") return $"{formula.Name} must be cast on the caster.";
            return $"{formula.Name} cannot find a target.";
        }

        private bool CanPlaceTerrainAt(FormulaDef formula, int x, int y)
        {
            if (formula == null || x < 0 || x >= CombatW || y < 0 || y >= CombatH) return false;
            if (UnitAt(x, y) != null) return false;
            Point existing = ObstacleAt(x, y);
            if (existing == null) return true;
            if (formula.Terrain == "fire" && existing.Kind == "tree") return true;
            return !IsBlockingTerrain(existing);
        }

        private string TerrainAfterPlacementReaction(FormulaDef formula, Point existing)
        {
            if (formula == null) return "";
            string terrain = formula.Terrain ?? "";
            if (existing == null) return terrain;
            string old = existing.Kind ?? "";
            if (terrain == "web" && old == "fire") return "";
            if (terrain == "gas" && old == "fire") return "fire";
            if (terrain == "ice" && old == "gas") return "";
            if (terrain == "sanctuary" && old == "curse") return "sanctuary";
            if (terrain == "curse" && old == "sanctuary") return "curse";
            return terrain;
        }

        private string TerrainReactionPreview(FormulaDef formula, int x, int y)
        {
            string line = TerrainPreviewLine(new Point(x, y, formula?.Terrain)).Trim();
            Point existing = ObstacleAt(x, y);
            if (formula != null && formula.Terrain == "tree")
            {
                line = $"tree cover: {CoverMaxIntegrity("tree")} integrity, {SummonedTreeDuration} rounds / arcs pass / foes break";
            }
            if (formula == null || existing == null) return line;
            string prefix = string.IsNullOrEmpty(line) ? "" : line + " / ";
            if (formula.Terrain == "fire" && existing.Kind == "tree") return prefix + "burns tree cover";
            if (formula.Terrain == "fire" && (existing.Kind == "gas" || existing.Kind == "web")) return prefix + "ignites hazard";
            if (formula.Terrain == "gas" && existing.Kind == "fire") return "gas detonates on existing fire; fire remains";
            if (formula.Terrain == "web" && existing.Kind == "fire") return "web burns away on existing fire";
            if (formula.Terrain == "ice" && existing.Kind == "gas") return "cold drops the poison cloud";
            if (formula.Terrain == "ice" && existing.Kind == "fire") return prefix + "quenches fire into steam";
            if (formula.Terrain == "fire" && existing.Kind == "ice") return prefix + "melts ice";
            if (formula.Terrain == "sanctuary" && existing.Kind == "curse") return "sanctuary cleanses the doom circle";
            if (formula.Terrain == "curse" && existing.Kind == "sanctuary") return "doom circle corrupts the sanctuary";
            return line;
        }

        private string FormulaHitTerrainPreview(FormulaDef formula, CombatUnit target)
        {
            if (formula == null || target == null) return "";
            Point terrain = ObstacleAt(target.X, target.Y);
            if (terrain == null) return "";
            string type = formula.DamageType ?? "";
            if (type == "fire" && terrain.Kind == "gas") return "ignites gas splash";
            if (type == "fire" && terrain.Kind == "web") return "burns web away";
            if (type == "fire" && terrain.Kind == "ice") return "melts ice into steam";
            if (type == "cold" && terrain.Kind == "fire") return "quenches fire into ice";
            if (type == "cold" && terrain.Kind == "gas") return "settles poison cloud";
            if (type == "shock" && (terrain.Kind == "ice" || terrain.Kind == "gas" || terrain.Kind == "web")) return "conducts shock to adjacent units";
            if (type == "light" && terrain.Kind == "curse") return "cleanses doom circle";
            if ((type == "death" || type == "mind") && terrain.Kind == "sanctuary") return "breaks sanctuary";
            return "";
        }

        private string ApplyTerrainPlacementReaction(FormulaDef formula, CombatUnit caster, int x, int y, Point existing)
        {
            if (formula == null || existing == null) return "";
            if (formula.Terrain == "fire" && existing.Kind == "tree")
            {
                RecordCombatPowerReaction("Burning cover");
                DamageUnitsAround(x, y, 1, 3 + state.Depth / 2, "fire", FormulaColor(formula), caster);
                AddFloat(x, y, "burn cover", ember);
                AddFlash(x, y, ember);
                return "The cover burns, singeing anyone pressed close.";
            }
            if (formula.Terrain == "fire" && existing.Kind == "gas")
            {
                RecordCombatPowerReaction("Gas ignition");
                DamageUnitsAround(x, y, 1, 5 + state.Depth, "fire", FormulaColor(formula), caster);
                AddFloat(x, y, "flash", ember);
                AddBurst(x, y, ember);
                return "The gas catches in a sudden flash.";
            }
            if (formula.Terrain == "fire" && existing.Kind == "web")
            {
                RecordCombatPowerReaction("Web flare");
                DamageUnitsAround(x, y, 1, 4 + state.Depth / 2, "fire", FormulaColor(formula), caster);
                AddFloat(x, y, "web flare", ember);
                AddBurst(x, y, ember);
                return "The webbing flares and collapses into flame.";
            }
            if (formula.Terrain == "gas" && existing.Kind == "fire")
            {
                RecordCombatPowerReaction("Gas ignition");
                DamageUnitsAround(x, y, 1, 5 + state.Depth, "fire", FormulaColor(formula), caster);
                AddFloat(x, y, "gas flash", ember);
                AddBurst(x, y, ember);
                return "The poison cloud detonates before it can settle.";
            }
            if (formula.Terrain == "web" && existing.Kind == "fire")
            {
                RecordCombatPowerReaction("Web flare");
                DamageUnitsAround(x, y, 1, 3 + state.Depth / 2, "fire", FormulaColor(formula), caster);
                AddFloat(x, y, "burned away", ember);
                AddBurst(x, y, ember);
                return "The web burns away before it can hold.";
            }
            if (formula.Terrain == "ice" && existing.Kind == "gas")
            {
                RecordCombatPowerReaction("Cold settle");
                AddFloat(x, y, "settled", frost);
                AddBurst(x, y, frost);
                foreach (CombatUnit unit in UnitsAround(x, y, 1))
                {
                    if (unit.Poisoned > 0) unit.Poisoned = Mathf.Max(0, unit.Poisoned - 1);
                }
                return "Cold drops the poison cloud out of the air.";
            }
            if (formula.Terrain == "ice" && existing.Kind == "fire")
            {
                RecordCombatPowerReaction("Steam burst");
                AddFloat(x, y, "steam", frost);
                AddBurst(x, y, frost);
                foreach (CombatUnit unit in UnitsAround(x, y, 1))
                {
                    TryApplyStatus(unit, "stun", 1, caster, 0.24f, unit.Side != caster.Side);
                }
                return "Ice quenches the fire into blinding steam.";
            }
            if (formula.Terrain == "fire" && existing.Kind == "ice")
            {
                RecordCombatPowerReaction("Ice melt");
                AddFloat(x, y, "melt", ember);
                AddBurst(x, y, ember);
                return "The ice hisses away under the flame.";
            }
            if (formula.Terrain == "sanctuary" && existing.Kind == "curse")
            {
                RecordCombatPowerReaction("Hallowed clash");
                int cleansed = 0;
                foreach (CombatUnit unit in UnitsAround(x, y, 1))
                {
                    if (unit.Side == UnitSide.Party)
                    {
                        unit.Hexed = Mathf.Max(0, unit.Hexed - 1);
                        unit.Poisoned = Mathf.Max(0, unit.Poisoned - 1);
                        unit.Bleeding = Mathf.Max(0, unit.Bleeding - 1);
                        unit.Shielded = Mathf.Max(unit.Shielded, 1);
                        cleansed++;
                    }
                    else
                    {
                        DealDamage(unit, 2 + state.Depth / 2, "light", teal);
                    }
                }
                AddFloat(x, y, "hallowed", teal);
                AddBurst(x, y, teal);
                return cleansed > 0 ? $"Sanctuary breaks the doom circle and steadies {cleansed} {(cleansed == 1 ? "ally" : "allies")}." : "Sanctuary breaks the doom circle.";
            }
            if (formula.Terrain == "curse" && existing.Kind == "sanctuary")
            {
                RecordCombatPowerReaction("Doom breach");
                foreach (CombatUnit unit in UnitsAround(x, y, 1))
                {
                    DealDamage(unit, 2 + state.Depth / 2, "mind", violet);
                    TryApplyStatus(unit, "hex", 1, caster, unit.Side == UnitSide.Enemy ? 0.72f : 0.42f, true);
                }
                AddFloat(x, y, "spoiled", violet);
                AddBurst(x, y, violet);
                return "The doom circle spoils the sanctuary in a psychic snap.";
            }
            return "";
        }

        private string ApplyFormulaHitTerrainReaction(FormulaDef formula, CombatUnit caster, CombatUnit target)
        {
            if (formula == null || target == null) return "";
            Point terrain = ObstacleAt(target.X, target.Y);
            if (terrain == null) return "";
            if (formula.DamageType == "fire" && (terrain.Kind == "gas" || terrain.Kind == "web"))
            {
                RecordCombatPowerReaction(terrain.Kind == "gas" ? "Gas ignition" : "Web flare");
                state.Combat.Obstacles.Remove(terrain);
                DamageUnitsAround(target.X, target.Y, 1, terrain.Kind == "gas" ? 5 + state.Depth : 3 + state.Depth / 2, "fire", FormulaColor(formula), caster);
                AddFloat(target.X, target.Y, terrain.Kind == "gas" ? "flash" : "flare", ember);
                AddBurst(target.X, target.Y, ember);
                return terrain.Kind == "gas" ? "The gas ignites around the mark." : "The webbing burns away around the mark.";
            }
            if (formula.DamageType == "fire" && terrain.Kind == "ice")
            {
                RecordCombatPowerReaction("Steam burst");
                state.Combat.Obstacles.Remove(terrain);
                TryApplyStatus(target, "stun", 1, caster, 0.18f, true);
                AddFloat(target.X, target.Y, "steam", ember);
                AddBurst(target.X, target.Y, ember);
                return "Fire melts the ice under the mark into scalding steam.";
            }
            if (formula.DamageType == "cold" && terrain.Kind == "fire")
            {
                RecordCombatPowerReaction("Flash freeze");
                state.Combat.Obstacles.Remove(terrain);
                state.Combat.Obstacles.Add(new Point(target.X, target.Y, "ice", 8));
                TryApplyStatus(target, "stun", 1, caster, 0.28f, true);
                AddFloat(target.X, target.Y, "quenched", frost);
                return "Cold quenches the fire into a slick patch.";
            }
            if (formula.DamageType == "cold" && terrain.Kind == "gas")
            {
                RecordCombatPowerReaction("Cold settle");
                state.Combat.Obstacles.Remove(terrain);
                target.Poisoned = Mathf.Max(0, target.Poisoned - 1);
                AddFloat(target.X, target.Y, "settled", frost);
                AddBurst(target.X, target.Y, frost);
                return "Cold drops the poison cloud out of the air.";
            }
            if (formula.DamageType == "light" && terrain.Kind == "curse")
            {
                RecordCombatPowerReaction("Doom cleansed");
                state.Combat.Obstacles.Remove(terrain);
                target.Hexed = Mathf.Max(0, target.Hexed - 1);
                AddFloat(target.X, target.Y, "cleansed", teal);
                AddBurst(target.X, target.Y, teal);
                return "Light breaks the doom circle under the mark.";
            }
            if ((formula.DamageType == "death" || formula.DamageType == "mind") && terrain.Kind == "sanctuary")
            {
                RecordCombatPowerReaction("Sanctuary broken");
                state.Combat.Obstacles.Remove(terrain);
                AddFloat(target.X, target.Y, "ward broken", violet);
                AddBurst(target.X, target.Y, violet);
                return "The dark sign cracks the sanctuary.";
            }
            if (formula.DamageType == "shock" && (terrain.Kind == "ice" || terrain.Kind == "gas" || terrain.Kind == "web"))
            {
                RecordCombatPowerReaction("Shock conduction");
                TryApplyStatus(target, "stun", 1, caster, 0.34f, true);
                int arcDamage = terrain.Kind == "gas" ? 3 + state.Depth / 2 : 2 + state.Depth / 2;
                foreach (CombatUnit unit in UnitsAround(target.X, target.Y, 1).Where(u => u.Id != target.Id && u.Side == target.Side))
                {
                    AddBeam(target.X, target.Y, unit.X, unit.Y, gold, "lightning");
                    DealDamage(unit, arcDamage, "shock", gold);
                    TryApplyStatus(unit, "stun", 1, caster, 0.18f, unit.Side != caster.Side);
                }
                AddFloat(target.X, target.Y, "arc", gold);
                AddBurst(target.X, target.Y, gold);
                return "The hazard carries the shock outward.";
            }
            return "";
        }

        private IEnumerable<CombatUnit> UnitsAround(int x, int y, int radius)
        {
            if (state?.Combat?.Units == null) yield break;
            foreach (CombatUnit unit in state.Combat.Units)
            {
                if (unit.Hp <= 0) continue;
                if (Distance(unit.X, unit.Y, x, y) <= radius) yield return unit;
            }
        }

        private void DamageUnitsAround(int x, int y, int radius, int amount, string damageType, Color color, CombatUnit source)
        {
            foreach (CombatUnit unit in UnitsAround(x, y, radius).ToList())
            {
                DealDamage(unit, amount, damageType, color);
            }
        }

        private string FormulaBeamKind(FormulaDef formula, CombatUnit caster, int x, int y)
        {
            if (formula == null || caster == null) return "spell";
            if (formula.Code == "MTR") return "meteor";
            if (formula.Code == "FBL") return "fireball";
            if (formula.DamageType == "shock") return formula.Effect == "teleport" ? "arc" : "lightning";
            if (FormulaCanArcOverCover(formula, x, y) && !HasLineOfSight(caster.X, caster.Y, x, y, true)) return "arc";
            if (formula.Effect == "summon") return "death";
            if (formula.Effect == "heal" || formula.Effect == "cure" || formula.Status == "regen" || formula.Status == "shield") return "heal";
            if (formula.DamageType == "death") return "death";
            if (formula.DamageType == "fire" || formula.Terrain == "fire") return "fire";
            if (formula.DamageType == "cold" || formula.Terrain == "ice") return "ice";
            if (formula.Terrain == "sanctuary") return "heal";
            if (formula.DamageType == "mind" || formula.DamageType == "poison" || formula.Status == "hex" || formula.Status == "sleep" || formula.Terrain == "web" || formula.Terrain == "gas" || formula.Terrain == "curse") return "hex";
            return "spell";
        }

        private Color FormulaColor(FormulaDef formula, float alpha = 1f)
        {
            if (formula == null) return Hex("d7a84e", alpha);
            if (formula.Effect == "summon") return Hex("b94b56", alpha);
            if (formula.Terrain == "tree") return Hex("7f9d5b", alpha);
            if (formula.Terrain == "stone") return Hex("9aa09a", alpha);
            if (formula.Terrain == "fire" || formula.DamageType == "fire") return Hex("c65c3b", alpha);
            if (formula.Terrain == "ice" || formula.DamageType == "cold") return Hex("9ad6e8", alpha);
            if (formula.Terrain == "web" || formula.Terrain == "gas" || formula.DamageType == "poison") return Hex("8fc27b", alpha);
            if (formula.Terrain == "sanctuary") return Hex("58b7a5", alpha);
            if (formula.Terrain == "curse") return Hex("8d6dcc", alpha);
            if (formula.DamageType == "death") return Hex("b94b56", alpha);
            if (formula.DamageType == "shock") return Hex("d7a84e", alpha);
            if (formula.DamageType == "light") return Hex("97dbc2", alpha);
            if (formula.Effect == "heal" || formula.Effect == "cure") return Hex("58b7a5", alpha);
            return Hex("8d6dcc", alpha);
        }

        private string TerrainDescription(string terrain)
        {
            if (terrain == "tree") return $"A breakable tree blocks movement and direct shots for {SummonedTreeDuration} rounds.";
            if (terrain == "stone") return "A breakable stone block shoulders out of the floor.";
            if (terrain == "fire") return "Fire crawls across the stones.";
            if (terrain == "ice") return "A slick sheet of ice flashes into being.";
            if (terrain == "web") return "Sticky threads lace the floor.";
            if (terrain == "gas") return "A venom haze coils low to the ground.";
            if (terrain == "smoke") return "Dense smoke blocks direct sight without slowing movement.";
            if (terrain == "sanctuary") return "A hallowed circle mends allies and sears enemies who begin their turn inside it.";
            if (terrain == "curse") return "A doom circle stains the floor, harming the mind and inviting a hex.";
            return "The floor changes shape.";
        }

        private int DealDamage(CombatUnit target, int amount, string damageType, Color color)
        {
            if (target == null || target.Hp <= 0) return 0;
            string type = string.IsNullOrEmpty(damageType) ? "physical" : damageType;
            float multiplier = 1f;
            if (HasTag(target.Resist, type)) multiplier *= 0.55f;
            if (HasTag(target.Weakness, type)) multiplier *= 1.45f;
            if (target.Hexed > 0) multiplier *= 1.20f;
            int guard = target.Guarding ? Mathf.Max(2, target.GuardBonus) : 0;
            int shield = target.Shielded > 0 ? 3 : 0;
            int damage = Mathf.Max(1, Mathf.RoundToInt(amount * multiplier) - guard - shield - GearDamageReduction(target, type) - RaceDamageReduction(target, type) - DemonFormDamageReduction(target));
            target.Hp = Mathf.Max(0, target.Hp - damage);
            StageCombatUnitPresentationBeat(
                target,
                target.Hp <= 0
                    ? CombatUnitPresentationBeatKind.Defeat
                    : CombatUnitPresentationBeatKind.Hit,
                CombatEffectStart(),
                target.Side == UnitSide.Party ? -1f : 1f);
            if (damage > 0 && target.Sleeping > 0)
            {
                target.Sleeping = 0;
                AddFloat(target.X, target.Y, "wakes", muted);
            }
            if (multiplier < 0.9f) AddFloat(target.X, target.Y, "resist", muted);
            if (multiplier > 1.1f) AddFloat(target.X, target.Y, "weak", gold);
            AddFloat(target.X, target.Y, "-" + damage, color, type);
            AddBurst(target.X, target.Y, color);
            AddFlash(target.X, target.Y, color);
            return damage;
        }

        private Color DamageColor(string damageType)
        {
            if (damageType == "fire") return ember;
            if (damageType == "cold") return frost;
            if (damageType == "shock") return gold;
            if (damageType == "poison") return poison;
            if (damageType == "death") return blood;
            if (damageType == "mind") return violet;
            if (damageType == "light") return teal;
            return blood;
        }

        private int WeaponPowerBonus(string weaponName)
        {
            string text = (weaponName ?? "").ToLowerInvariant();
            int bonus = 0;
            if (text.Contains("broadsword") || text.Contains("war hammer") || text.Contains("war flail") || text.Contains("halberd")) bonus++;
            if (text.Contains("vicious") || text.Contains("vampiric") || text.Contains("death")) bonus++;
            if (text.Contains("unfathomable darkness")) bonus += 2;
            if (text.Contains("crude")) bonus--;
            return Mathf.Clamp(bonus, -1, 3);
        }

        private int WeaponHitBonus(string weaponName)
        {
            string text = (weaponName ?? "").ToLowerInvariant();
            int bonus = 0;
            if (text.Contains("epee") || text.Contains("sabre") || text.Contains("balanced") || text.Contains("elven")) bonus += 5;
            if (text.Contains("crossbow")) bonus += 4;
            if (text.Contains("unfathomable darkness")) bonus += 3;
            if (text.Contains("war hammer") || text.Contains("tower")) bonus -= 3;
            if (text.Contains("crude")) bonus -= 5;
            return bonus;
        }

        private int ArmorAgilityModifier(string armorName)
        {
            string text = (armorName ?? "").ToLowerInvariant();
            int mod = 0;
            if (text.Contains("plate") || text.Contains("tower") || text.Contains("chain") || text.Contains("mail")) mod -= 1;
            if (text.Contains("weightless") || text.Contains("silk") || text.Contains("mantle") || text.Contains("cloak") || text.Contains("leathers")) mod += 1;
            return Mathf.Clamp(mod, -2, 2);
        }

        private int GearGuardBonus(CombatUnit unit)
        {
            string armor = (unit?.ArmorName ?? "").ToLowerInvariant();
            string weapon = (unit?.WeaponName ?? "").ToLowerInvariant();
            int bonus = 0;
            if (armor.Contains("tower shield") || armor.Contains("kite shield")) bonus += 2;
            if (armor.Contains("buckler") || weapon.Contains("ward shield")) bonus++;
            if (armor.Contains("warding") || armor.Contains("guarding") || armor.Contains("anti-magic")) bonus++;
            if (armor.Contains("robe") && !string.IsNullOrEmpty(unit?.Spell)) bonus++;
            return Mathf.Clamp(bonus, 0, 4);
        }

        private int GearDamageReduction(CombatUnit target, string damageType)
        {
            string armor = (target?.ArmorName ?? "").ToLowerInvariant();
            string type = (damageType ?? "").ToLowerInvariant();
            int reduction = 0;
            if ((armor.Contains("warding") || armor.Contains("anti-magic") || armor.Contains("moonstone")) && type != "physical") reduction++;
            if ((armor.Contains("plate") || armor.Contains("tower shield")) && type == "physical") reduction++;
            if (armor.Contains("thorns") && type == "physical") reduction++;
            return Mathf.Clamp(reduction, 0, 3);
        }

        private string GearOnHitStatus(string weaponName)
        {
            string text = (weaponName ?? "").ToLowerInvariant();
            if (text.Contains("stunning") || text.Contains("storm") || text.Contains("war hammer")) return "stun";
            if (text.Contains("bleeding") || text.Contains("vicious") || text.Contains("epee") || text.Contains("sabre") || text.Contains("thorns")) return "bleed";
            if (text.Contains("venom")) return "poison";
            if (text.Contains("terror") || text.Contains("silence")) return "sleep";
            return "";
        }

        private int GearLifeDrainAmount(string weaponName, int dealtDamage)
        {
            string text = (weaponName ?? "").ToLowerInvariant();
            if (dealtDamage <= 0) return 0;
            if (text.Contains("unfathomable darkness")) return Mathf.Clamp(1 + dealtDamage / 8, 1, 3);
            if (text.Contains("vampiric")) return Mathf.Clamp(1 + dealtDamage / 10, 1, 2);
            return 0;
        }

        private float GearOnHitChance(string weaponName)
        {
            string text = (weaponName ?? "").ToLowerInvariant();
            float chance = 0.20f;
            if (text.Contains("masterwork") || text.Contains("vicious") || text.Contains("stormglass")) chance += 0.10f;
            if (text.Contains("crude")) chance -= 0.06f;
            return Mathf.Clamp(chance, 0.08f, 0.42f);
        }

        private bool TryApplyStatus(CombatUnit target, string status, int duration, CombatUnit source, float chance, bool hostile)
        {
            if (target == null || target.Hp <= 0 || string.IsNullOrEmpty(status)) return false;
            float rollChance = StatusApplyChance(target, status, source, chance, hostile);
            if (hostile && rng.NextDouble() > rollChance)
            {
                AddFloat(target.X, target.Y, "resist", muted);
                float pan = CombatAudioMixRules.StereoPanForColumn(target.X, CombatW);
                float pitch = CombatAudioMixRules.PitchForCue("resist", target.X);
                QueueSfx("resist", combatVfxImpactDelay, 0.34f, pan, pitch);
                return false;
            }

            int turns = Mathf.Max(1, duration);
            Color color = violet;
            if (status == "poison")
            {
                target.Poisoned = Mathf.Max(target.Poisoned, turns);
                color = poison;
            }
            else if (status == "bleed")
            {
                target.Bleeding = Mathf.Max(target.Bleeding, turns);
                color = blood;
            }
            else if (status == "stun")
            {
                target.Stunned = Mathf.Max(target.Stunned, turns);
                color = gold;
            }
            else if (status == "sleep")
            {
                target.Sleeping = Mathf.Max(target.Sleeping, turns);
                color = violet;
            }
            else if (status == "shield")
            {
                target.Shielded = Mathf.Max(target.Shielded, turns);
                color = teal;
            }
            else if (status == "regen")
            {
                target.Regenerating = Mathf.Max(target.Regenerating, turns);
                color = teal;
            }
            else if (status == "web")
            {
                target.Webbed = Mathf.Max(target.Webbed, turns);
                color = poison;
            }
            else if (status == "hex")
            {
                target.Hexed = Mathf.Max(target.Hexed, turns);
                color = violet;
            }
            else if (status == "stealth")
            {
                target.Stealthed = Mathf.Max(target.Stealthed, turns);
                color = teal;
            }
            else return false;

            AddFloat(target.X, target.Y, StatusLabel(status), color);
            AddBurst(target.X, target.Y, color);
            AddFlash(target.X, target.Y, color);
            AddTileGlyph(target.X, target.Y, null, "status", color);
            // Staged powers already own cast, impact and aftershock audio on this timeline.
            // Only immediate terrain/gear status changes need a separate status cue.
            if (combatVfxImpactDelay <= 0.005f) PlaySfx(StatusSfx(status), 0.42f);
            return true;
        }

        private string StatusSfx(string status)
        {
            if (status == "poison") return "poison";
            if (status == "bleed") return "blade";
            if (status == "stun") return "shock";
            if (status == "sleep" || status == "hex") return "curse";
            if (status == "shield" || status == "regen") return "ward";
            if (status == "web") return "web";
            if (status == "stealth") return "stealth";
            return "status";
        }

        private string StatusLabel(string status)
        {
            if (status == "poison") return "poisoned";
            if (status == "bleed") return "bleeding";
            if (status == "stun") return "stunned";
            if (status == "sleep") return "sleeping";
            if (status == "shield") return "warded";
            if (status == "regen") return "regen";
            if (status == "web") return "webbed";
            if (status == "hex") return "hexed";
            if (status == "stealth") return "hidden";
            return status;
        }

        private bool HasTag(string list, string tag)
        {
            if (string.IsNullOrEmpty(list) || string.IsNullOrEmpty(tag)) return false;
            return list.Split('|').Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
        }

        private void UseElixir()
        {
            if (state.Elixirs <= 0)
            {
                PushLog("No elixirs remain.", Tone.Warn);
                PlaySfx("blocked");
                return;
            }
            if (state.Mode == GameMode.Combat)
            {
                CombatUnit active = CurrentUnit();
                CombatCommandResult result = CombatLifecycle().TryUseItem(active, 18, 6);
                if (!result.Success)
                {
                    if (result.Failure == CombatCommandFailure.NoElixir)
                    {
                        PushLog("No elixirs remain.", Tone.Warn);
                        PlaySfx("blocked");
                    }
                    return;
                }
                AddFloat(active.X, active.Y, "elixir", teal);
                PushLog($"{active.Name} drinks an elixir.", Tone.Good);
                PlaySfx("elixir", 0.8f);
                AfterCombatAction(active);
                return;
            }
            PartyMember target = state.Party.Where(p => p.Hp > 0).OrderBy(p => (float)p.Hp / p.MaxHp).FirstOrDefault();
            if (target == null) return;
            state.Elixirs--;
            target.Hp = Mathf.Min(target.MaxHp, target.Hp + 18);
            target.Mana = Mathf.Min(target.MaxMana, target.Mana + 6);
            PushLog($"{target.Name} drinks an elixir.", Tone.Good);
            PlaySfx("elixir", 0.8f);
        }

        private void SyncPartyFromCombat()
        {
            if (state.Combat == null) return;
            foreach (CombatUnit unit in state.Combat.Units.Where(u => u.Side == UnitSide.Party && u.PartyIndex >= 0))
            {
                PartyMember member = state.Party[unit.PartyIndex];
                member.Hp = Mathf.Clamp(unit.Hp, 0, member.MaxHp);
                member.Mana = Mathf.Clamp(unit.Mana, 0, member.MaxMana);
                member.Skills = unit.Skills.Clone();
            }
        }

        private void ImproveSkill(CombatUnit unit, string skill, int amount)
        {
            if (unit.Side != UnitSide.Party) return;
            int before = SkillValue(unit.Skills, skill);
            SetSkill(unit.Skills, skill, Mathf.Clamp(before + amount, 1, 99));
            if (unit.PartyIndex >= 0) state.Party[unit.PartyIndex].Skills = unit.Skills.Clone();
            int after = SkillValue(unit.Skills, skill);
            if (before < 15 && after >= 15) PushLog($"{unit.Name} is no longer lousy at {skill}.", Tone.Good);
            if (before < 30 && after >= 30) PushLog($"{unit.Name} becomes steady at {skill}.", Tone.Good);
        }

        private void EnsureCombatTurnState()
        {
            CancelCombatResolutionBeat(false);
            if (state?.Combat == null) return;
            ClearFormulaEntry();
            ClearAbilityEntry();
            NormalizeCombatObstacles();
            RepairCombatSummons(true);
            ApplySummonerDeathToPactSummons();
            EnsureInitiativeQueue();
            CombatUnit active = CurrentUnit();
            if (active == null)
            {
                active = (state.Combat.InitiativeQueue ?? new List<string>()).Select(LiveUnitById).FirstOrDefault(u => u != null);
                if (active == null) active = InitiativeOrder().FirstOrDefault();
                if (active == null) return;
                state.Combat.ActiveId = active.Id;
            }
            CombatLifecycle().RepairActiveTurnState(active, active.Side == UnitSide.Enemy);
            selectedAction = ActionMode.Attack;
            aiActAt = active.Side == UnitSide.Enemy ? Time.time + (state.ReducedMotion ? 0.05f : 0.45f) : -1f;
        }

        private void NormalizeCombatObstacles()
        {
            if (state?.Combat?.Obstacles == null) return;
            List<Point> normalized = new List<Point>();
            foreach (IGrouping<string, Point> group in state.Combat.Obstacles.GroupBy(o => $"{o.X},{o.Y}"))
            {
                Point obstacle = group
                    .OrderByDescending(IsBlockingTerrain)
                    .ThenByDescending(o => o.Duration)
                    .ThenByDescending(o => o.Integrity)
                    .FirstOrDefault();
                if (obstacle == null) continue;
                obstacle.Duration = FieldDurationRounds(obstacle.Kind, Mathf.Max(0, obstacle.Duration));
                if (IsBreakableCover(obstacle) && obstacle.Integrity <= 0)
                {
                    obstacle.Integrity = CoverMaxIntegrity(obstacle.Kind);
                }
                if (IsDisruptableRitual(obstacle))
                {
                    if (obstacle.Duration <= 0) obstacle.Duration = CombatRitualRules.DefaultCountdown(obstacle.Kind);
                    if (obstacle.Integrity <= 0) obstacle.Integrity = CombatRitualRules.MaxIntegrity(obstacle.Kind);
                }
                normalized.Add(obstacle);
            }
            state.Combat.Obstacles = normalized;
        }

        private int FieldDurationRounds(string kind, int duration)
        {
            if (duration <= 0) return 0;
            switch ((kind ?? "").ToLowerInvariant())
            {
                case "tree": return Mathf.Clamp(duration, 1, SummonedTreeDuration);
                case "fire":
                case "gas":
                case "smoke":
                case "web":
                case "ice":
                case "sanctuary":
                case "curse":
                    return Mathf.Clamp(duration, 1, 4);
                case "glyph": return Mathf.Clamp(duration, 1, 3);
                case "demonrift": return Mathf.Clamp(duration, 1, 4);
                default: return duration;
            }
        }

        private int EnemyHazardDurationRounds(string kind)
        {
            switch ((kind ?? "").ToLowerInvariant())
            {
                case "demonrift": return 4;
                case "glyph": return 3;
                case "fire":
                case "gas":
                case "web":
                case "ice":
                    return 3;
                default: return 3;
            }
        }

        private void DrawDefeat()
        {
            boardRect = GetBoardRect();
            DrawPanel(boardRect);
            GUI.Label(new Rect(boardRect.x + 40, boardRect.y + 80, boardRect.width - 80, 50), "The party has fallen.", titleStyle);
            GUI.Label(new Rect(boardRect.x + 42, boardRect.y + 140, boardRect.width - 84, 60), "A new oath may yet be sworn. The old road waits.", labelStyle);
            if (GUI.Button(new Rect(boardRect.x + 42, boardRect.y + 220, 180, 48), "New Party", buttonStyle)) NewMuster();
        }

        private void DrawVictory()
        {
            boardRect = GetBoardRect();
            DrawRpgPanel(boardRect, gold);
            Rect hero = new Rect(boardRect.x + 46, boardRect.y + 52, boardRect.width - 92, 174);
            DrawRect(hero, Hex("080b0d", 0.86f));
            DrawBorder(hero, gold, 2);
            TryDrawQuestWorldAtlasIcon(new Rect(hero.xMax - 154, hero.y + 22, 116, 116), 19, Color.white.WithAlpha(0.88f));
            GUI.Label(new Rect(hero.x + 26, hero.y + 24, hero.width - 210, 42), "The Old Road Is Sealed", titleStyle);
            GUI.Label(new Rect(hero.x + 28, hero.y + 80, hero.width - 230, 54),
                "Vhal Rakh's meteor crown breaks above the ritual heart. Midgaard has one more dawn, and this beta route now has a complete ending.",
                labelStyle);

            Rect summary = new Rect(boardRect.x + 46, hero.yMax + 24, boardRect.width * 0.48f, 226);
            DrawRpgPanel(summary, teal);
            GUI.Label(new Rect(summary.x + 20, summary.y + 14, summary.width - 40, 24), "Party Ledger", h2Style);
            int living = state.Party.Count(p => p.Hp > 0);
            int level = state.Party.Count == 0 ? 1 : Mathf.RoundToInt((float)state.Party.Average(p => p.Level));
            string line = $"Survivors {living}/{state.Party.Count} / Avg level {level} / Gold {state.Gold} / Depth {state.Depth}";
            GUI.Label(new Rect(summary.x + 22, summary.y + 50, summary.width - 44, 24), line, CenterLeftStyle(13, gold));
            float y = summary.y + 84;
            foreach (PartyMember member in state.Party)
            {
                Rect row = new Rect(summary.x + 18, y, summary.width - 36, 30);
                DrawRect(row, Hex("151b20", 0.88f));
                DrawBorder(row, MemberColor(member).WithAlpha(0.66f), 1);
                GUI.Label(new Rect(row.x + 10, row.y + 4, row.width * 0.34f, 18), member.Name, CenterLeftStyle(12, ink));
                GUI.Label(new Rect(row.x + row.width * 0.36f, row.y + 4, row.width * 0.30f, 18), $"{DisplayClass(member.ClassKey)} L{member.Level}", CenterLeftStyle(11, muted));
                GUI.Label(new Rect(row.x + row.width * 0.66f, row.y + 4, row.width * 0.30f, 18), $"{BestSkillLabel(member)} {BestSkillValue(member)}", CenterLeftStyle(11, gold));
                y += 35;
            }

            Rect route = new Rect(summary.xMax + 22, hero.yMax + 24, boardRect.xMax - summary.xMax - 68, 226);
            DrawRpgPanel(route, violet);
            GUI.Label(new Rect(route.x + 20, route.y + 14, route.width - 40, 24), "Beta Route Complete", h2Style);
            string[] chapters =
            {
                "I  Midgaard Cisterns",
                "II Kobold Smoke",
                "III Bone Road",
                "IV Glass and Ash",
                "V  Red Gate",
                "VI Meteor Crown"
            };
            for (int i = 0; i < chapters.Length; i++)
            {
                Rect row = new Rect(route.x + 22, route.y + 50 + i * 24, route.width - 44, 20);
                DrawRect(new Rect(row.x, row.y + 8, 10, 4), i < chapters.Length - 1 ? teal : gold);
                GUI.Label(new Rect(row.x + 18, row.y, row.width - 18, row.height), chapters[i], CenterLeftStyle(11, i < chapters.Length - 1 ? muted : gold));
            }
            GUI.Label(new Rect(route.x + 22, route.yMax - 44, route.width - 44, 32), "Next passes can turn this scaffold into hand-authored dungeons, NPC quests, and multi-phase boss rules.", CenterLeftStyle(11, muted));

            Rect actions = new Rect(boardRect.x + 46, boardRect.yMax - 86, boardRect.width - 92, 56);
            if (GUI.Button(new Rect(actions.x, actions.y, 170, 46), "New Party", buttonStyle)) NewMuster();
            if (GUI.Button(new Rect(actions.x + 186, actions.y, 150, 46), "Tavern", buttonStyle))
            {
                state.Mode = GameMode.Tavern;
                PlaySfx("ui", 0.55f);
            }
            if (TavernMenuRules.ShowDeveloperTesting(DeveloperTestingBuildEnabled()) && GUI.Button(new Rect(actions.x + 352, actions.y, 150, 46), "Beta Lab", buttonStyle)) StartBetaCombatLab();
        }

        private Rect GetBoardRect()
        {
            if (state != null && state.Mode == GameMode.Explore)
            {
                ExplorationHudGeometry exploreHud = ExplorationHudScreenLayout.Calculate(Screen.width, Screen.height, !exploreHudCollapsed);
                float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
                float exploreTop = exploreHud.Top.yMax + 6f * scale;
                float exploreBottom = exploreHud.Command.yMin - 6f * scale;
                return new Rect(5f * scale, exploreTop, Screen.width - 10f * scale, Mathf.Max(360f * scale, exploreBottom - exploreTop));
            }

            float sideW = CombatHudScreenLayout.SideRailWidth(Screen.width);
            bool chrome = state != null && (state.Mode == GameMode.Explore || state.Mode == GameMode.Combat || state.Mode == GameMode.Defeat || state.Mode == GameMode.Victory);
            float top = chrome ? 78f : 12f;
            float bottom = state != null && state.Mode == GameMode.Combat
                ? Screen.height - CombatHudScreenLayout.Calculate(Screen.width, Screen.height).Command.yMin + 6f
                : state != null && state.Mode == GameMode.Explore ? 112f : 28f;
            return new Rect(12f, top, Screen.width - sideW - 36f, Mathf.Max(320f, Screen.height - top - bottom));
        }

        private Rect SidePanelRect()
        {
            if (state != null && state.Mode == GameMode.Explore)
            {
                return ExplorationHudScreenLayout.Calculate(Screen.width, Screen.height, !exploreHudCollapsed).Side;
            }

            float sideW = CombatHudScreenLayout.SideRailWidth(Screen.width);
            bool chrome = state != null && (state.Mode == GameMode.Explore || state.Mode == GameMode.Combat || state.Mode == GameMode.Defeat || state.Mode == GameMode.Victory);
            float top = chrome ? 78f : 62f;
            return new Rect(Screen.width - sideW - 12, top, sideW, Mathf.Max(280f, Screen.height - top - 12f));
        }

        private Rect BoardInnerRect(Rect outer, int w, int h)
        {
            Rect inner = new Rect(outer.x + 22, outer.y + 24, outer.width - 44, outer.height - 42);
            float aspect = (float)w / h;
            if (inner.width / inner.height > aspect)
            {
                float width = inner.height * aspect;
                inner.x += (inner.width - width) / 2f;
                inner.width = width;
            }
            else
            {
                float height = inner.width / aspect;
                inner.y += (inner.height - height) / 2f;
                inner.height = height;
            }
            return inner;
        }

        private Rect ExploreBoardInnerRect(Rect outer, int w, int h)
        {
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            Rect inner = new Rect(outer.x + 12f * scale, outer.y + 46f * scale, outer.width - 24f * scale, outer.height - 58f * scale);
            float aspect = (float)w / h;
            if (inner.width / inner.height > aspect)
            {
                float width = inner.height * aspect;
                inner.x += (inner.width - width) / 2f;
                inner.width = width;
            }
            else
            {
                float height = inner.width / aspect;
                inner.y += (inner.height - height) / 2f;
                inner.height = height;
            }
            return inner;
        }

        private Rect CombatBoardInnerRect(Rect outer)
        {
            // The migrated HUD owns the top and command chrome; the board only needs its frame inset.
            Rect inner = new Rect(outer.x + 14f, outer.y + 14f, outer.width - 28f, outer.height - 28f);
            float aspect = (float)CombatW / CombatH;
            if (inner.width / inner.height > aspect)
            {
                float width = inner.height * aspect;
                inner.x += (inner.width - width) / 2f;
                inner.width = width;
            }
            else
            {
                float height = inner.width / aspect;
                inner.y += (inner.height - height) / 2f;
                inner.height = height;
            }
            return inner;
        }

        private void ShowBanner(string text)
        {
            bannerText = text;
            bannerUntil = Time.time + (state != null && state.ReducedMotion ? 0.9f : 1.5f);
            MarkUiDirty();
        }

        private void ShowFormulaPowerCue(CombatUnit caster, FormulaDef formula, CombatUnit target, bool focused)
        {
            CombatPowerIdentity identity = CombatPowerPresentationRules.ForFormula(formula, caster?.Name, target?.Name, focused);
            TryGetFormulaPowerArt(formula, out Texture2D texture, out Rect source);
            ShowCombatPowerCue(identity, texture, source, CombatImpactRules.ForFormula(formula).ImpactDelay);
        }

        private void ShowAbilityPowerCue(CombatUnit active, MartialAbility ability, CombatUnit target)
        {
            CombatPowerIdentity identity = CombatPowerPresentationRules.ForAbility(ability, active?.Name, target?.Name);
            TryGetAbilityPowerArt(ability, out Texture2D texture, out Rect source);
            ShowCombatPowerCue(identity, texture, source, CombatImpactRules.ForAbility(ability).ImpactDelay);
        }

        private void ShowCombatPowerCue(
            CombatPowerIdentity identity,
            Texture2D texture,
            Rect source,
            float impactDelay)
        {
            float now = Time.time;
            float duration = state != null && state.ReducedMotion ? Mathf.Min(0.82f, identity.Duration) : identity.Duration;
            combatPowerCue = identity;
            combatPowerOutcomeText = "";
            combatPowerCueTexture = texture;
            combatPowerCueSource = source;
            combatPowerCueStarted = now;
            combatPowerCueUntil = now + duration;
            combatPowerCueImpactAt = now + (state != null && state.ReducedMotion ? 0f : Mathf.Clamp(impactDelay, 0f, duration));
            combatPowerPulseUntil = 0f;
            MarkUiDirty();
        }

        private void SetCombatPowerOutcome(CombatPowerOutcomeSnapshot before)
        {
            CombatPowerOutcome outcome = CombatPowerOutcomeRules.Compare(before, state?.Combat);
            combatPowerOutcomeText = CombatPowerOutcomeRules.FormatWithReactions(outcome, combatPowerReactions);
            combatPowerOutcomeVisibleAt = Time.time + pendingCombatPowerOutcomeDelay;
            MarkUiDirty();
        }

        private void BeginCombatPowerReactionCapture()
        {
            combatPowerReactions.Clear();
            pendingCombatPowerOutcomeDelay = 0f;
        }

        private void RecordCombatPowerReaction(string label)
        {
            string clean = (label ?? "").Trim();
            if (clean.Length == 0 || combatPowerReactions.Any(value => string.Equals(value, clean, StringComparison.OrdinalIgnoreCase))) return;
            if (combatPowerReactions.Count < 2) combatPowerReactions.Add(clean);
        }

        private float BeginCombatVfxTimeline(CombatImpactProfile profile)
        {
            float previous = combatVfxImpactDelay;
            combatVfxImpactDelay = state != null && state.ReducedMotion ? 0f : profile.ImpactDelay;
            return previous;
        }

        private void RestoreCombatVfxTimeline(float previous)
        {
            combatVfxImpactDelay = Mathf.Max(0f, previous);
        }

        private float CombatEffectStart(float explicitDelay = 0f)
        {
            return Time.time + Mathf.Max(combatVfxImpactDelay, Mathf.Clamp(explicitDelay, 0f, 0.55f));
        }

        private PowerCastAura StageCombatPowerCast(
            CombatImpactProfile profile,
            int sourceX,
            int sourceY,
            int targetX,
            int targetY,
            Color color,
            bool focused)
        {
            if (state == null || state.ReducedMotion) return null;
            float now = Time.time;
            PowerCastAura aura = new PowerCastAura
            {
                SourceX = sourceX,
                SourceY = sourceY,
                TargetX = targetX,
                TargetY = targetY,
                Color = color.ToHex(),
                Kind = profile.CastSfx,
                Intensity = CombatImpactRules.VisualIntensity(profile),
                Focused = focused,
                Start = now,
                ImpactAt = now + profile.ImpactDelay,
                Duration = CombatImpactRules.CastAuraDuration(profile)
            };
            powerCastAuras.Add(aura);
            if (powerCastAuras.Count > 10) powerCastAuras.RemoveRange(0, powerCastAuras.Count - 10);
            MarkUiDirty();
            return aura;
        }

        private void ApplyCombatImpactFeedback(
            CombatImpactProfile profile,
            int x,
            int y,
            Color color,
            string visualKind = "")
        {
            int reactionCount = ApplyCombatImpactAudioFeedback(profile, x, y);
            int visualIntensity = CombatImpactRules.VisualIntensity(profile, reactionCount);
            string resolvedVisualKind = string.IsNullOrWhiteSpace(visualKind) ? profile.ImpactSfx : visualKind;
            if (state == null) return;
            if (state.ReducedMotion)
            {
                powerImpactEchoes.Add(new PowerImpactEcho
                {
                    X = x,
                    Y = y,
                    Color = color.ToHex(),
                    Kind = resolvedVisualKind,
                    Intensity = visualIntensity,
                    ReactionCount = reactionCount,
                    StaticStamp = true,
                    Start = Time.time,
                    ImpactAt = Time.time,
                    Duration = 0.16f
                });
                if (powerImpactEchoes.Count > 12) powerImpactEchoes.RemoveRange(0, powerImpactEchoes.Count - 12);
                MarkUiDirty();
                return;
            }

            CombatPowerVisualMotif motif = CombatPowerVisualRules.MotifFor(resolvedVisualKind);
            powerImpactEchoes.Add(new PowerImpactEcho
            {
                X = x,
                Y = y,
                Color = color.ToHex(),
                Kind = resolvedVisualKind,
                Intensity = visualIntensity,
                ReactionCount = reactionCount,
                Start = Time.time,
                ImpactAt = Time.time + profile.ImpactDelay,
                Duration = CombatImpactRules.EchoDuration(profile, reactionCount)
            });
            if (powerImpactEchoes.Count > 12) powerImpactEchoes.RemoveRange(0, powerImpactEchoes.Count - 12);

            int burstCount = CombatImpactRules.PresentationBurstCount(profile, reactionCount);
            if (motif == CombatPowerVisualMotif.Generic && burstCount > 0)
            {
                AddEpicBurstDelayed(x, y, color, burstCount, profile.BurstSpeed, profile.ImpactDelay);
            }
            if (motif != CombatPowerVisualMotif.Generic)
            {
                AddPowerAftermathDelayed(
                    x,
                    y,
                    color,
                    motif,
                    visualIntensity,
                    CombatImpactRules.AftermathDelay(profile));
            }
            if (visualIntensity >= 3)
            {
                combatImpactFrameStarted = Time.time + profile.ImpactDelay;
                combatImpactFrameUntil = combatImpactFrameStarted + CombatImpactRules.ImpactFrameDuration(profile);
                combatImpactFrameColor = color.ToHex();
                combatImpactFrameIntensity = visualIntensity;
            }
            StartCombatImpactShake(profile);
            MarkUiDirty();
        }

        private int ApplyCombatImpactAudioFeedback(CombatImpactProfile profile, int x, int y)
        {
            int reactionCount = combatPowerReactions.Count;
            pendingCombatPowerOutcomeDelay = state != null && state.ReducedMotion ? 0f : profile.ImpactDelay;
            PlayCombatImpactSfx(profile, x, y, reactionCount);
            return reactionCount;
        }

        private void ApplyCombatMissFeedback(CombatImpactProfile profile, int targetX)
        {
            combatPowerReactions.Clear();
            pendingCombatPowerOutcomeDelay = 0f;
            PlayCombatMissSfx(profile, targetX);
            MarkUiDirty();
        }

        private void StartCombatImpactShake(CombatImpactProfile profile)
        {
            if (profile.ShakeMagnitude <= 0f || profile.ShakeDuration <= 0f) return;
            combatShakeStarted = Time.time + profile.ImpactDelay;
            combatShakeUntil = combatShakeStarted + profile.ShakeDuration;
            combatShakeMagnitude = profile.ShakeMagnitude;
        }

        private Rect ApplyCombatImpactShake(Rect grid)
        {
            if (state == null || state.ReducedMotion || combatShakeMagnitude <= 0f) return grid;
            float now = Time.time;
            if (now < combatShakeStarted || now >= combatShakeUntil) return grid;

            float duration = Mathf.Max(0.01f, combatShakeUntil - combatShakeStarted);
            float progress = Mathf.Clamp01((now - combatShakeStarted) / duration);
            float envelope = Mathf.Sin(progress * Mathf.PI) * (1f - progress * 0.18f);
            float phase = (now - combatShakeStarted) * 97f;
            grid.position += new Vector2(
                Mathf.Round(Mathf.Sin(phase + 0.65f) * combatShakeMagnitude * envelope),
                Mathf.Round(Mathf.Cos(phase * 1.23f) * combatShakeMagnitude * 0.72f * envelope));
            return grid;
        }

        private void ClearCombatMotionForReducedMotion()
        {
            if (state == null || !state.ReducedMotion) return;
            PowerImpactEcho latestImpact = powerImpactEchoes
                .Where(echo => echo != null)
                .OrderByDescending(echo => echo.ImpactAt)
                .FirstOrDefault();

            tweens.Clear();
            particles.Clear();
            beams.Clear();
            flashes.Clear();
            castGlyphs.Clear();
            powerCastAuras.Clear();
            powerImpactEchoes.Clear();
            combatUnitPresentationBeats.Clear();
            combatShakeStarted = 0f;
            combatShakeUntil = 0f;
            combatShakeMagnitude = 0f;
            combatImpactFrameStarted = 0f;
            combatImpactFrameUntil = 0f;
            combatImpactFrameIntensity = 0;
            combatPowerPulseUntil = 0f;

            if (latestImpact != null)
            {
                flashes.Add(new CellFlash
                {
                    X = latestImpact.X,
                    Y = latestImpact.Y,
                    Color = latestImpact.Color,
                    Start = Time.time,
                    Duration = 0.16f
                });
            }
            MarkUiDirty();
        }

        private void DrawCombatPowerPulse(Rect grid)
        {
            float now = Time.time;
            if (state != null
                && !state.ReducedMotion
                && now >= combatImpactFrameStarted
                && now < combatImpactFrameUntil
                && !string.IsNullOrWhiteSpace(combatImpactFrameColor))
            {
                float span = Mathf.Max(0.01f, combatImpactFrameUntil - combatImpactFrameStarted);
                float progress = Mathf.Clamp01((now - combatImpactFrameStarted) / span);
                float envelope = 1f - Mathf.SmoothStep(0f, 1f, progress);
                Color impactColor = combatImpactFrameColor.ToColor();
                float opacity = CombatPowerVisualRules.ImpactFlashOpacity(combatImpactFrameIntensity) * envelope;
                DrawRect(grid, Color.Lerp(impactColor, cursorWhite, 0.48f).WithAlpha(opacity));
                DrawBorder(Pad(grid, -3f), impactColor.WithAlpha(opacity * 2.2f), combatImpactFrameIntensity >= 3 ? 3 : 2);
            }

            if (Time.time >= combatPowerPulseUntil || string.IsNullOrWhiteSpace(combatPowerCue.AccentHex)) return;
            float duration = state != null && state.ReducedMotion ? 0.16f : 0.30f + combatPowerCue.Intensity * 0.08f;
            float remaining = Mathf.Clamp01((combatPowerPulseUntil - Time.time) / Mathf.Max(0.01f, duration));
            float wave = state != null && state.ReducedMotion ? 1f : Mathf.Sin((1f - remaining) * Mathf.PI);
            Color color = combatPowerCue.AccentHex.ToColor();
            int rings = Mathf.Clamp(combatPowerCue.Intensity, 1, 3);
            if (rings >= 3) DrawRect(grid, color.WithAlpha(0.025f + wave * 0.035f));
            for (int i = 0; i < rings; i++)
            {
                float expand = 3f + i * 4f;
                Rect ring = new Rect(grid.x - expand, grid.y - expand, grid.width + expand * 2f, grid.height + expand * 2f);
                DrawBorder(ring, color.WithAlpha((0.28f + wave * 0.42f) * remaining / (i + 1)), i == 0 ? 2 : 1);
            }
        }

        private void AddFloat(int x, int y, string text, Color color, string feedbackKind = null)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            float now = Time.time;
            floatTexts.RemoveAll(t => now > t.Start + t.Duration + 0.25f);
            float start = CombatEffectStart();
            float duration = state != null && state.ReducedMotion ? 0.72f : 1.08f;
            int nearby = floatTexts.Count(t =>
                Mathf.Abs(t.X - x) <= 1 &&
                Mathf.Abs(t.Y - y) <= 1 &&
                start <= t.Start + t.Duration + 0.22f &&
                start + duration + 0.22f >= t.Start);
            int lane = nearby % floatTextLaneOffsets.Length;
            floatTexts.Add(new FloatText
            {
                X = x,
                Y = y,
                Text = text.Trim(),
                Color = color.ToHex(),
                IconIndex = CombatFeedbackRules.FloatIconIndex(text, feedbackKind),
                Start = start,
                Duration = duration,
                Lane = lane,
                OffsetX = floatTextLaneOffsets[lane],
                OffsetY = Mathf.Min(1.55f, nearby * 0.18f + (lane / 3) * 0.10f),
                Serial = floatTextSerial++
            });
            if (floatTexts.Count > 64) floatTexts.RemoveRange(0, floatTexts.Count - 64);
        }

        private void StageVisualSmokeCombatFeedback()
        {
            if (state?.Combat?.Units == null) return;
            castGlyphs.Clear();
            floatTexts.Clear();
            powerImpactEchoes.Clear();
            powerCastAuras.Clear();
            combatUnitPresentationBeats.Clear();
            float now = Time.time;
            CombatUnit active = CurrentUnit();
            List<CombatUnit> enemies = state.Combat.Units
                .Where(unit => unit != null && unit.Side == UnitSide.Enemy && unit.Hp > 0)
                .Take(1)
                .ToList();
            CombatImpactProfile fireballProfile = CombatImpactRules.ForFormula(GetFormula("FBL"));
            FormulaDef fireball = GetFormula("FBL");
            // Leave a short board-establishing beat before the capture coroutine
            // follows this impact time and records the authored effect at its peak.
            float showcaseStart = now + 1.30f;
            float showcaseImpact = showcaseStart + fireballProfile.ImpactDelay;

            if (active != null && enemies.Count > 0)
            {
                CombatUnit smokeTarget = enemies[0];
                CombatPowerIdentity identity = CombatPowerPresentationRules.ForFormula(
                    fireball,
                    active.Name,
                    smokeTarget.Name,
                    true);
                TryGetFormulaPowerArt(fireball, out Texture2D cueTexture, out Rect cueSource);
                combatPowerCue = identity;
                combatPowerCueTexture = cueTexture;
                combatPowerCueSource = cueSource;
                combatPowerCueStarted = showcaseStart;
                combatPowerCueUntil = showcaseStart + identity.Duration;
                combatPowerCueImpactAt = showcaseImpact;
                combatPowerOutcomeText = "Gas ignition / 12 damage";
                combatPowerOutcomeVisibleAt = showcaseImpact;
                powerCastAuras.Add(new PowerCastAura
                {
                    SourceX = active.X,
                    SourceY = active.Y,
                    TargetX = smokeTarget.X,
                    TargetY = smokeTarget.Y,
                    Color = ember.ToHex(),
                    Kind = "castember",
                    Intensity = 3,
                    Focused = true,
                    Start = showcaseStart,
                    ImpactAt = showcaseImpact,
                    Duration = CombatImpactRules.CastAuraDuration(fireballProfile)
                });
                CombatUnitPresentationRules.AddBounded(
                    combatUnitPresentationBeats,
                    CombatUnitPresentationRules.Create(
                        smokeTarget.Id,
                        CombatUnitPresentationBeatKind.Hit,
                        showcaseImpact,
                        1f),
                    now);
            }
            if (enemies.Count > 0)
            {
                AddVisualSmokeFloat(enemies[0], "-12", ember, "fire", 1, showcaseImpact);
                powerImpactEchoes.Add(new PowerImpactEcho
                {
                    X = enemies[0].X,
                    Y = enemies[0].Y,
                    Color = ember.ToHex(),
                    Kind = "fireball",
                    Intensity = 3,
                    ReactionCount = 1,
                    Start = showcaseStart,
                    ImpactAt = showcaseImpact,
                    Duration = CombatImpactRules.EchoDuration(fireballProfile, 1)
                });
            }
            MarkUiDirty();
        }

        private void AddVisualSmokeFloat(CombatUnit unit, string text, Color color, string feedbackKind, int lane, float start)
        {
            if (unit == null) return;
            floatTexts.Add(new FloatText
            {
                X = unit.X,
                Y = unit.Y,
                Text = text,
                Color = color.ToHex(),
                IconIndex = CombatFeedbackRules.FloatIconIndex(text, feedbackKind),
                Start = start,
                Duration = state != null && state.ReducedMotion ? 0.72f : 1.08f,
                Lane = Mathf.Clamp(lane, 0, floatTextLaneOffsets.Length - 1),
                OffsetX = floatTextLaneOffsets[Mathf.Clamp(lane, 0, floatTextLaneOffsets.Length - 1)],
                OffsetY = 0.10f,
                Serial = floatTextSerial++
            });
        }

        private void AddBurst(int x, int y, Color color)
        {
            AddBurstDelayed(x, y, color, 0f);
        }

        private void AddFieldPlacementFlourish(int x, int y, string kind, Color color)
        {
            if (!CombatFieldPresentationRules.IsPersistentField(kind)) return;
            CombatFieldPresentationProfile profile = CombatFieldPresentationRules.For(kind);
            Color bright = Color.Lerp(color, profile.Kind == "curse" ? blood : gold, profile.Kind == "curse" ? 0.30f : 0.24f);
            AddEpicBurstDelayed(x, y, bright, profile.PlacementBurstCount, profile.PlacementBurstSpeed, 0.05f, true);
        }

        private string FieldGlyphKind(string kind)
        {
            switch ((kind ?? "").ToLowerInvariant())
            {
                case "fire": return "fireball";
                case "ice": return "spellanim:11";
                case "gas": return "magicui:9";
                case "smoke": return "magicui:9";
                case "web": return "magicui:8";
                case "sanctuary": return "magicui:14";
                case "curse": return "spellanim:8";
                default: return "area";
            }
        }

        private void AddBurstDelayed(int x, int y, Color color, float delay)
        {
            if (state.ReducedMotion || combatVfxImpactDelay > 0.005f) return;
            float start = CombatEffectStart(delay);
            for (int i = 0; i < 10; i++)
            {
                particles.Add(new ParticleDot
                {
                    X = x + 0.5f,
                    Y = y + 0.5f,
                    VX = UnityEngine.Random.Range(-0.8f, 0.8f),
                    VY = UnityEngine.Random.Range(-0.8f, 0.8f),
                    Color = color.ToHex(),
                    Kind = i % 4 == 0 ? "spark" : "mote",
                    Size = UnityEngine.Random.Range(0.040f, 0.075f),
                    Gravity = UnityEngine.Random.Range(0.08f, 0.22f),
                    Seed = UnityEngine.Random.Range(0, 1000000),
                    Start = start,
                    Duration = 0.55f
                });
            }
            TrimCombatParticles();
        }

        private void AddEpicBurst(int x, int y, Color color, int count, float speed)
        {
            AddEpicBurstDelayed(x, y, color, count, speed, 0f);
        }

        private void AddEpicBurstDelayed(int x, int y, Color color, int count, float speed, float delay)
        {
            AddEpicBurstDelayed(x, y, color, count, speed, delay, false);
        }

        private void AddEpicBurstDelayed(int x, int y, Color color, int count, float speed, float delay, bool fieldPlacementOwner)
        {
            if (state.ReducedMotion || (!fieldPlacementOwner && combatVfxImpactDelay > 0.005f)) return;
            count = Mathf.Clamp(count, 4, 32);
            speed = Mathf.Clamp(speed, 0.4f, 2.2f);
            float start = CombatEffectStart(delay);
            for (int i = 0; i < count; i++)
            {
                float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                float velocity = UnityEngine.Random.Range(0.45f, speed);
                particles.Add(new ParticleDot
                {
                    X = x + 0.5f,
                    Y = y + 0.5f,
                    VX = Mathf.Cos(angle) * velocity,
                    VY = Mathf.Sin(angle) * velocity,
                    Color = color.ToHex(),
                    Kind = i % 3 == 0 ? "spark" : "mote",
                    Size = UnityEngine.Random.Range(0.050f, 0.105f),
                    Gravity = UnityEngine.Random.Range(0.10f, 0.30f),
                    Seed = UnityEngine.Random.Range(0, 1000000),
                    Start = start,
                    Duration = UnityEngine.Random.Range(0.58f, 0.92f)
                });
            }
            TrimCombatParticles();
        }

        private void AddPowerAftermathDelayed(
            int x,
            int y,
            Color color,
            CombatPowerVisualMotif motif,
            int intensity,
            float delay)
        {
            if (state == null || state.ReducedMotion) return;
            int count = CombatPowerVisualRules.AftermathParticleCount(motif, intensity);
            string kind = CombatPowerVisualRules.AftermathParticleKind(motif);
            float speed = CombatPowerVisualRules.AftermathParticleSpeed(motif, intensity);
            float start = CombatEffectStart(delay);
            for (int i = 0; i < count; i++)
            {
                float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                float velocity = UnityEngine.Random.Range(speed * 0.42f, speed);
                float vx = Mathf.Cos(angle) * velocity;
                float vy = Mathf.Sin(angle) * velocity;
                float gravity = UnityEngine.Random.Range(0.10f, 0.34f);
                float size = UnityEngine.Random.Range(0.045f, 0.095f);
                float duration = UnityEngine.Random.Range(0.56f, 0.92f);
                Color particleColor = color;

                switch (kind)
                {
                    case "smoke":
                        vx *= 0.34f;
                        vy = UnityEngine.Random.Range(-0.46f, -0.16f);
                        gravity = UnityEngine.Random.Range(-0.08f, 0.04f);
                        size = UnityEngine.Random.Range(0.12f, 0.23f);
                        duration = UnityEngine.Random.Range(0.74f, 1.12f);
                        particleColor = Color.Lerp(color, retroBlack, 0.72f);
                        break;
                    case "ember":
                        vy -= UnityEngine.Random.Range(0.16f, 0.38f);
                        gravity = UnityEngine.Random.Range(0.34f, 0.68f);
                        particleColor = Color.Lerp(color, gold, UnityEngine.Random.Range(0.22f, 0.62f));
                        break;
                    case "shard":
                        gravity = UnityEngine.Random.Range(0.18f, 0.42f);
                        size = UnityEngine.Random.Range(0.055f, 0.11f);
                        particleColor = Color.Lerp(color, cursorWhite, UnityEngine.Random.Range(0.20f, 0.52f));
                        break;
                    case "glint":
                        vx *= 0.62f;
                        vy -= UnityEngine.Random.Range(0.18f, 0.42f);
                        gravity = UnityEngine.Random.Range(-0.08f, 0.12f);
                        size = UnityEngine.Random.Range(0.040f, 0.078f);
                        particleColor = Color.Lerp(color, cursorWhite, UnityEngine.Random.Range(0.36f, 0.72f));
                        break;
                    case "leaf":
                        vx *= 0.56f;
                        vy -= UnityEngine.Random.Range(0.08f, 0.28f);
                        gravity = UnityEngine.Random.Range(0.08f, 0.22f);
                        size = UnityEngine.Random.Range(0.052f, 0.096f);
                        particleColor = Color.Lerp(color, moss, UnityEngine.Random.Range(0.18f, 0.52f));
                        break;
                    case "streak":
                        gravity = UnityEngine.Random.Range(0.04f, 0.20f);
                        size = UnityEngine.Random.Range(0.042f, 0.072f);
                        duration = UnityEngine.Random.Range(0.42f, 0.68f);
                        particleColor = Color.Lerp(color, cursorWhite, UnityEngine.Random.Range(0.12f, 0.38f));
                        break;
                }

                particles.Add(new ParticleDot
                {
                    X = x + 0.5f + UnityEngine.Random.Range(-0.06f, 0.06f),
                    Y = y + 0.5f + UnityEngine.Random.Range(-0.06f, 0.06f),
                    VX = vx,
                    VY = vy,
                    Color = particleColor.ToHex(),
                    Kind = kind,
                    Size = size,
                    Gravity = gravity,
                    Seed = UnityEngine.Random.Range(0, 1000000),
                    Start = start + UnityEngine.Random.Range(0f, 0.08f),
                    Duration = duration
                });
            }
            TrimCombatParticles();
        }

        private void AddFireballAftermathDelayed(int x, int y, Color color, float delay)
        {
            if (state == null || state.ReducedMotion) return;
            float start = CombatEffectStart(delay);
            Color smoke = Color.Lerp(color, retroBlack, 0.78f);
            for (int i = 0; i < 7; i++)
            {
                particles.Add(new ParticleDot
                {
                    X = x + 0.5f + UnityEngine.Random.Range(-0.12f, 0.12f),
                    Y = y + 0.5f + UnityEngine.Random.Range(-0.06f, 0.10f),
                    VX = UnityEngine.Random.Range(-0.18f, 0.18f),
                    VY = UnityEngine.Random.Range(-0.58f, -0.22f),
                    Color = smoke.ToHex(),
                    Kind = "smoke",
                    Size = UnityEngine.Random.Range(0.16f, 0.28f),
                    Gravity = UnityEngine.Random.Range(-0.10f, 0.02f),
                    Seed = UnityEngine.Random.Range(0, 1000000),
                    Start = start + UnityEngine.Random.Range(0.08f, 0.22f),
                    Duration = UnityEngine.Random.Range(0.72f, 1.08f)
                });
            }
            for (int i = 0; i < 8; i++)
            {
                float angle = UnityEngine.Random.Range(Mathf.PI * 1.10f, Mathf.PI * 1.90f);
                float velocity = UnityEngine.Random.Range(0.42f, 1.12f);
                particles.Add(new ParticleDot
                {
                    X = x + 0.5f,
                    Y = y + 0.5f,
                    VX = Mathf.Cos(angle) * velocity,
                    VY = Mathf.Sin(angle) * velocity,
                    Color = Color.Lerp(color, gold, 0.58f).ToHex(),
                    Kind = "ember",
                    Size = UnityEngine.Random.Range(0.045f, 0.085f),
                    Gravity = UnityEngine.Random.Range(0.42f, 0.78f),
                    Seed = UnityEngine.Random.Range(0, 1000000),
                    Start = start,
                    Duration = UnityEngine.Random.Range(0.62f, 0.94f)
                });
            }
            TrimCombatParticles();
        }

        private void TrimCombatParticles()
        {
            const int maxParticles = 384;
            if (particles.Count > maxParticles) particles.RemoveRange(0, particles.Count - maxParticles);
        }

        private void AddBeam(int fromX, int fromY, int toX, int toY, Color color, string kind = "shot")
        {
            AddBeamDelayed(fromX, fromY, toX, toY, color, kind, 0f);
        }

        private void AddBeamDelayed(int fromX, int fromY, int toX, int toY, Color color, string kind, float delay)
        {
            if (state.ReducedMotion) return;
            beams.Add(new BeamEffect
            {
                FromX = fromX,
                FromY = fromY,
                ToX = toX,
                ToY = toY,
                Color = color.ToHex(),
                Kind = kind,
                Start = Time.time + Mathf.Clamp(delay, 0f, 0.55f),
                Duration = CombatPowerVisualRules.BeamDuration(kind)
            });
        }

        private void AddFlash(int x, int y, Color color)
        {
            AddFlashDelayed(x, y, color, 0f);
        }

        private void AddFlashDelayed(int x, int y, Color color, float delay)
        {
            if (state.ReducedMotion || combatVfxImpactDelay > 0.005f) return;
            flashes.Add(new CellFlash
            {
                X = x,
                Y = y,
                Color = color.ToHex(),
                Start = CombatEffectStart(delay),
                Duration = 0.28f
            });
        }

        private void AddCastGlyph(CombatUnit caster, FormulaDef formula, Color color)
        {
            if (state.ReducedMotion || caster == null) return;
            string kind = SpellAnimationGlyphKind(formula, "caster");
            if (string.IsNullOrEmpty(kind)) kind = CasterKnowsSchool(caster.Spell, "mend") || CasterKnowsSchool(formula?.School, "mend") ? "priest" : "wizard";
            castGlyphs.Add(new CastGlyph { X = caster.X, Y = caster.Y, Kind = kind, Color = color.ToHex(), Start = Time.time, Duration = 0.52f });
        }

        private void AddTileGlyph(int x, int y, FormulaDef formula, string kind, Color color)
        {
            AddTileGlyphDelayed(x, y, formula, kind, color, 0f);
        }

        private void AddTileGlyphDelayed(int x, int y, FormulaDef formula, string kind, Color color, float delay)
        {
            if (state.ReducedMotion || combatVfxImpactDelay > 0.005f) return;
            castGlyphs.Add(new CastGlyph
            {
                X = x,
                Y = y,
                Kind = SpellAnimationGlyphKind(formula, kind),
                Color = color.ToHex(),
                Start = CombatEffectStart(delay),
                Duration = formula != null && formula.Splash ? 0.62f : 0.46f
            });
        }

        private string SpellAnimationGlyphKind(FormulaDef formula, string requestedKind)
        {
            return CombatFeedbackRules.SpellGlyphKind(formula, requestedKind);
        }

        private void AddRangerTileGlyph(int x, int y, int atlasIndex, Color color)
        {
            if (state == null || state.ReducedMotion || combatVfxImpactDelay > 0.005f) return;
            castGlyphs.Add(new CastGlyph
            {
                X = x,
                Y = y,
                Kind = "ranger:" + Mathf.Clamp(atlasIndex, 0, 15),
                Color = color.ToHex(),
                Start = CombatEffectStart(),
                Duration = 0.50f
            });
        }

        private void AddAbilityCastGlyph(int x, int y, MartialAbility ability, Color color)
        {
            if (state == null || state.ReducedMotion || ability == null) return;
            int index = AbilityIconIndex(ability.Id);
            if (index < 0) return;
            castGlyphs.Add(new CastGlyph
            {
                X = x,
                Y = y,
                Kind = "ability:" + index,
                Color = color.ToHex(),
                Start = Time.time,
                Duration = 0.48f + CombatPowerPresentationRules.AbilityIntensity(ability.Id) * 0.06f
            });
        }

        private void DrawParticles(Rect grid, float cell)
        {
            float now = Time.time;
            foreach (ParticleDot p in particles)
            {
                if (now < p.Start) continue;
                float t = Mathf.Clamp01((now - p.Start) / p.Duration);
                Color c = p.Color.ToColor();
                string kind = p.Kind ?? "";
                float flicker = 0.86f + Mathf.Sin((p.Seed * 0.0017f + t * 12f) * Mathf.PI) * 0.14f;
                float size = p.Size > 0f ? p.Size * cell : 6f;
                float px = grid.x + (p.X + p.VX * t) * cell;
                float py = grid.y + (p.Y + p.VY * t + p.Gravity * t * t) * cell;
                if (kind == "smoke")
                {
                    float smokeSize = size * Mathf.Lerp(0.78f, 1.48f, t);
                    c.a = (1f - t) * 0.38f;
                    DrawRect(new Rect(px - smokeSize * 0.56f, py - smokeSize * 0.42f, smokeSize, smokeSize * 0.82f), c);
                    DrawRect(new Rect(px - smokeSize * 0.18f, py - smokeSize * 0.62f, smokeSize * 0.72f, smokeSize * 0.72f), Color.Lerp(c, retroBlack, 0.26f).WithAlpha(c.a * 0.72f));
                    continue;
                }
                if (kind == "spark")
                {
                    float sparkSize = size * Mathf.Lerp(1.16f, 0.54f, t) * flicker;
                    c.a = (1f - t) * 0.92f;
                    Vector2 velocity = new Vector2(p.VX, p.VY + p.Gravity * t);
                    Vector2 tail = velocity.sqrMagnitude > 0.001f ? velocity.normalized * sparkSize * 1.8f : Vector2.up * sparkSize;
                    DrawPixelLine(new Vector2(px, py), new Vector2(px, py) - tail, c, Mathf.Max(1f, sparkSize * 0.46f));
                    DrawRect(new Rect(px - sparkSize * 0.5f, py - sparkSize * 0.5f, sparkSize, sparkSize), Color.Lerp(c, cursorWhite, 0.34f));
                    continue;
                }
                if (kind == "ember")
                {
                    float emberSize = size * Mathf.Lerp(1.18f, 0.62f, t) * flicker;
                    c.a = (1f - t) * 0.96f;
                    DrawRect(new Rect(px - emberSize * 0.62f, py - emberSize * 0.62f, emberSize * 1.24f, emberSize * 1.24f), c);
                    DrawRect(new Rect(px - emberSize * 0.28f, py - emberSize * 0.28f, emberSize * 0.56f, emberSize * 0.56f), cursorWhite.WithAlpha(c.a * 0.82f));
                    continue;
                }
                if (kind == "shard")
                {
                    float shardSize = size * Mathf.Lerp(1.22f, 0.52f, t) * flicker;
                    c.a = (1f - t) * 0.90f;
                    Vector2 motion = new Vector2(p.VX, p.VY + p.Gravity * t);
                    Vector2 direction = motion.sqrMagnitude > 0.001f ? motion.normalized : Vector2.up;
                    Vector2 side = new Vector2(-direction.y, direction.x);
                    Vector2 center = new Vector2(px, py);
                    DrawPixelLine(center - direction * shardSize, center + direction * shardSize, c, Mathf.Max(1f, shardSize * 0.34f));
                    DrawPixelLine(center - side * shardSize * 0.42f, center + side * shardSize * 0.42f, Color.Lerp(c, cursorWhite, 0.36f), Mathf.Max(1f, shardSize * 0.24f));
                    continue;
                }
                if (kind == "glint")
                {
                    float glintSize = size * Mathf.Lerp(0.72f, 1.34f, Mathf.Sin(t * Mathf.PI)) * flicker;
                    c.a = (1f - t) * 0.88f;
                    Vector2 center = new Vector2(px, py);
                    DrawPixelLine(center + Vector2.left * glintSize, center + Vector2.right * glintSize, c, Mathf.Max(1f, glintSize * 0.22f));
                    DrawPixelLine(center + Vector2.up * glintSize, center + Vector2.down * glintSize, Color.Lerp(c, cursorWhite, 0.42f), Mathf.Max(1f, glintSize * 0.22f));
                    continue;
                }
                if (kind == "leaf")
                {
                    float leafSize = size * Mathf.Lerp(1.08f, 0.64f, t) * flicker;
                    c.a = (1f - t) * 0.82f;
                    float sway = Mathf.Sin((p.Seed * 0.0031f + t * 5f) * Mathf.PI);
                    Vector2 direction = new Vector2(0.72f, sway * 0.68f).normalized;
                    Vector2 center = new Vector2(px, py);
                    DrawPixelLine(center - direction * leafSize, center + direction * leafSize, c, Mathf.Max(1f, leafSize * 0.34f));
                    DrawRect(new Rect(px - leafSize * 0.28f, py - leafSize * 0.28f, leafSize * 0.56f, leafSize * 0.56f), Color.Lerp(c, gold, 0.12f));
                    continue;
                }
                if (kind == "streak")
                {
                    float streakSize = size * Mathf.Lerp(1.28f, 0.48f, t) * flicker;
                    c.a = (1f - t) * 0.90f;
                    Vector2 motion = new Vector2(p.VX, p.VY + p.Gravity * t);
                    Vector2 direction = motion.sqrMagnitude > 0.001f ? motion.normalized : Vector2.right;
                    Vector2 center = new Vector2(px, py);
                    DrawPixelLine(center - direction * streakSize * 1.9f, center + direction * streakSize * 0.42f, c, Mathf.Max(1f, streakSize * 0.32f));
                    continue;
                }
                float moteSize = size * Mathf.Lerp(1f, 0.58f, t) * flicker;
                c.a = 1f - t;
                DrawRect(new Rect(px - moteSize * 0.5f, py - moteSize * 0.5f, moteSize, moteSize), c);
            }
        }

        private void DrawFloatingTextLayer(Rect grid, float cell, float now)
        {
            if (floatTexts.Count == 0) return;
            bool reduced = state != null && state.ReducedMotion;
            List<Rect> placed = new List<Rect>(floatTexts.Count);
            List<Rect> occupied = LiveCombatUnitCoreRects(grid, cell);
            foreach (FloatText ft in floatTexts.OrderBy(t => t.Start).ThenBy(t => t.Serial))
            {
                if (now < ft.Start) continue;
                float t = Mathf.Clamp01((now - ft.Start) / ft.Duration);
                string label = FloatTextDisplay(ft.Text);
                int len = label.Length;
                float h = Mathf.Clamp(cell * 0.22f, 16f, 22f);
                bool hasIcon = ft.IconIndex >= 0 && IsCombatSpellFloatAtlas();
                float iconInset = hasIcon ? h - 2f : 0f;
                float w = Mathf.Clamp(cell * (0.48f + len * 0.045f) + iconInset, cell * 0.72f, cell * 1.55f);
                float drift = reduced ? 0f : EaseOutFloat(t) * 0.34f;
                float sway = reduced ? 0f : Mathf.Sin((ft.Serial + 1) * 1.73f + t * 2.4f) * cell * 0.025f;
                float laneLift = Mathf.Min(0.62f, ft.OffsetY * 0.45f);
                Rect r = new Rect(
                    grid.x + (ft.X + 0.5f + ft.OffsetX) * cell - w * 0.5f + sway,
                    grid.y + (ft.Y - 0.18f - laneLift - drift) * cell,
                    w,
                    h);
                r = ResolveFloatTextRect(r, grid, placed, occupied, cell);
                placed.Add(r);
                float alpha = CombatFeedbackRules.FloatAlpha(t);
                Color textColor = ft.Color.ToColor();
                DrawFloatBackplate(r, textColor.WithAlpha(0.72f * alpha), hasIcon ? ft.IconIndex : -1);
                int fontSize = Mathf.RoundToInt(Mathf.Clamp(cell * 0.13f, 10f, 14f));
                GUIStyle shadow = CenterStyle(fontSize, Hex("010203", 0.90f * alpha));
                GUIStyle style = CenterStyle(fontSize, Color.Lerp(VividColor(textColor), cursorWhite, 0.16f).WithAlpha(alpha));
                Rect textRect = hasIcon ? new Rect(r.x + iconInset, r.y, r.width - iconInset, r.height) : r;
                string fitted = FitText(label, textRect.width - 6f, style);
                GUI.Label(new Rect(textRect.x + 1f, textRect.y + 1f, textRect.width, textRect.height), fitted, shadow);
                GUI.Label(textRect, fitted, style);
            }
        }

        private void DrawFloatBackplate(Rect rect, Color accent, int iconIndex)
        {
            float alpha = Mathf.Clamp01(accent.a);
            if (alpha <= 0.02f) return;
            DrawRect(new Rect(rect.x + 1f, rect.y + 2f, rect.width, rect.height), Hex("000000", 0.22f * alpha));
            DrawRect(rect, Hex("030405", 0.62f * alpha));
            DrawBorder(rect, accent.WithAlpha(0.82f * alpha), 1);
            if (iconIndex >= 0)
            {
                float size = Mathf.Max(12f, rect.height - 4f);
                Rect icon = new Rect(rect.x + 2f, rect.y + 2f, size, size);
                DrawRect(icon, Hex("080b0d", 0.72f * alpha));
                if (TryDrawCombatSpellFloatAtlasIcon(Pad(icon, 1f), iconIndex, Color.white.WithAlpha(Mathf.Min(0.92f, alpha))))
                {
                    DrawBorder(icon, accent.WithAlpha(0.50f * alpha), 1);
                }
            }
        }

        private string FloatTextDisplay(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            text = text.Trim();
            return text.Length <= 18 ? text : text.Substring(0, 17) + ".";
        }

        private float EaseOutFloat(float t)
        {
            t = Mathf.Clamp01(t);
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        private List<Rect> LiveCombatUnitCoreRects(Rect grid, float cell)
        {
            List<Rect> occupied = new List<Rect>();
            if (state?.Combat?.Units == null) return occupied;
            foreach (CombatUnit unit in state.Combat.Units.Where(u => u != null && u.Hp > 0))
            {
                Vector2 pos = UnitDrawPos(unit);
                occupied.Add(new Rect(
                    grid.x + (pos.x + 0.12f) * cell,
                    grid.y + (pos.y + 0.08f) * cell,
                    cell * 0.76f,
                    cell * 0.86f));
            }
            return occupied;
        }

        private Rect ResolveFloatTextRect(Rect rect, Rect bounds, List<Rect> placed, List<Rect> occupied, float cell)
        {
            Rect best = ClampFloatTextRect(rect, bounds);
            if (FloatTextRectIsClear(best, placed, occupied)) return best;

            float stepY = rect.height + 3f;
            float stepX = Mathf.Max(rect.width * 0.54f, Mathf.Clamp(cell * 0.34f, 16f, 34f));
            for (int i = 1; i <= 10; i++)
            {
                Rect up = ClampFloatTextRect(new Rect(rect.x, rect.y - stepY * i, rect.width, rect.height), bounds);
                if (FloatTextRectIsClear(up, placed, occupied)) return up;
                Rect down = ClampFloatTextRect(new Rect(rect.x, rect.y + stepY * i, rect.width, rect.height), bounds);
                if (FloatTextRectIsClear(down, placed, occupied)) return down;
                float lane = Mathf.Ceil(i * 0.5f);
                float side = i % 2 == 0 ? -stepX : stepX;
                Rect lateral = ClampFloatTextRect(new Rect(rect.x + side * lane, rect.y, rect.width, rect.height), bounds);
                if (FloatTextRectIsClear(lateral, placed, occupied)) return lateral;
                Rect diagonal = ClampFloatTextRect(new Rect(rect.x + side * lane, rect.y - stepY * lane, rect.width, rect.height), bounds);
                if (FloatTextRectIsClear(diagonal, placed, occupied)) return diagonal;
            }
            return best;
        }

        private bool FloatTextRectIsClear(Rect rect, List<Rect> placed, List<Rect> occupied)
        {
            return !FloatTextOverlapsAny(rect, placed) && !FloatTextOverlapsAny(rect, occupied);
        }

        private Rect ClampFloatTextRect(Rect rect, Rect bounds)
        {
            float pad = 3f;
            rect.x = Mathf.Clamp(rect.x, bounds.x + pad, bounds.xMax - rect.width - pad);
            rect.y = Mathf.Clamp(rect.y, bounds.y + pad, bounds.yMax - rect.height - pad);
            return rect;
        }

        private bool FloatTextOverlapsAny(Rect rect, List<Rect> placed)
        {
            const float pad = 2f;
            for (int i = 0; i < placed.Count; i++)
            {
                Rect other = placed[i];
                if (rect.xMin < other.xMax + pad &&
                    rect.xMax > other.xMin - pad &&
                    rect.yMin < other.yMax + pad &&
                    rect.yMax > other.yMin - pad)
                {
                    return true;
                }
            }
            return false;
        }

        private int DemonSummonSpriteIndex(CombatUnit unit)
        {
            if (unit == null) return -1;
            if (unit.DemonFormTurns > 0) return unit.MaxHp > 0 && unit.Hp <= unit.MaxHp / 2 ? 11 : 8;
            string role = (unit.Role ?? "").ToLowerInvariant();
            bool wounded = unit.MaxHp > 0 && unit.Hp <= unit.MaxHp / 2;
            switch (role)
            {
                case "boundimp": return wounded ? 2 : 0;
                case "lesserdemon": return wounded ? 7 : 4;
                case "greaterdemon": return wounded ? 11 : 8;
                default: return -1;
            }
        }

        private void StageCombatUnitPresentationBeat(
            CombatUnit unit,
            CombatUnitPresentationBeatKind kind,
            float impactAt,
            float recoilDirection)
        {
            if (unit == null || state == null || state.ReducedMotion) return;
            CombatUnitPresentationBeat beat = CombatUnitPresentationRules.Create(
                unit.Id,
                kind,
                impactAt,
                recoilDirection);
            CombatUnitPresentationRules.AddBounded(
                combatUnitPresentationBeats,
                beat,
                Time.time);
            MarkUiDirty();
        }

        private void AddTween(string id, Vector2 from, Vector2 to, TweenKind kind)
        {
            if (state.ReducedMotion) return;
            tweens.RemoveAll(t => t.Id == id);
            tweens.Add(new Tween(id, from, to, Time.time, kind == TweenKind.Lunge ? 0.14f : 0.18f, kind));
        }

        private Vector2 UnitDrawPos(CombatUnit unit)
        {
            Tween tween = tweens.LastOrDefault(t => t.Id == unit.Id);
            if (tween == null) return new Vector2(unit.X, unit.Y);
            float t = Mathf.Clamp01((Time.time - tween.Start) / tween.Duration);
            if (tween.Kind == TweenKind.Lunge) t = Mathf.Sin(t * Mathf.PI);
            return Vector2.Lerp(tween.From, tween.To, Mathf.SmoothStep(0, 1, t));
        }

        private CombatUnit CurrentUnit()
        {
            return state?.Combat?.Units?.FirstOrDefault(u => u.Id == state.Combat.ActiveId && u.Hp > 0);
        }

        private bool IsHeroUnit(CombatUnit unit)
        {
            return unit != null && unit.Side == UnitSide.Party && unit.PartyIndex >= 0 && !unit.Summoned;
        }

        private List<CombatUnit> InitiativeOrder()
        {
            return InitiativeOrderFor(state?.Combat);
        }

        private List<CombatUnit> InitiativeOrderFor(CombatState combat)
        {
            if (combat?.Units == null) return new List<CombatUnit>();
            return combat.Units
                .Where(u => u.Hp > 0)
                .OrderByDescending(InitiativeScore)
                .ThenBy(u => u.Side == UnitSide.Party ? 0 : 1)
                .ThenBy(u => u.Name)
                .ThenBy(u => u.Id)
                .ToList();
        }

        private int InitiativeScore(CombatUnit unit)
        {
            return unit == null ? 0 : unit.Agility + unit.AttackSpeed / 3;
        }

        private void RebuildInitiativeQueue(CombatState combat)
        {
            if (combat == null) return;
            combat.InitiativeQueue = InitiativeOrderFor(combat).Select(u => u.Id).ToList();
        }

        private void EnsureInitiativeQueue()
        {
            CombatState combat = state?.Combat;
            if (combat == null) return;
            if (combat.InitiativeQueue == null) combat.InitiativeQueue = new List<string>();
            bool hasLiveQueuedUnit = combat.InitiativeQueue.Any(id => LiveUnitById(id) != null);
            if (combat.InitiativeQueue.Count == 0 || !hasLiveQueuedUnit)
            {
                RebuildInitiativeQueue(combat);
            }
        }

        private CombatUnit LiveUnitById(string id)
        {
            if (string.IsNullOrEmpty(id) || state?.Combat?.Units == null) return null;
            return state.Combat.Units.FirstOrDefault(u => u.Id == id && u.Hp > 0);
        }

        private CombatUnit NextQueuedCombatUnit(out bool newRound)
        {
            newRound = false;
            CombatState combat = state?.Combat;
            if (combat == null) return null;
            EnsureInitiativeQueue();
            if (combat.InitiativeQueue == null || combat.InitiativeQueue.Count == 0) return null;

            int activeIndex = combat.InitiativeQueue.FindIndex(id => id == combat.ActiveId);
            int startIndex = string.IsNullOrEmpty(combat.ActiveId) ? 0 : activeIndex >= 0 ? activeIndex + 1 : 0;
            for (int i = Mathf.Clamp(startIndex, 0, combat.InitiativeQueue.Count); i < combat.InitiativeQueue.Count; i++)
            {
                CombatUnit unit = LiveUnitById(combat.InitiativeQueue[i]);
                if (unit != null) return unit;
            }

            if (!string.IsNullOrEmpty(combat.ActiveId))
            {
                combat.Round++;
                newRound = true;
            }
            RebuildInitiativeQueue(combat);
            return combat.InitiativeQueue == null
                ? null
                : combat.InitiativeQueue.Select(LiveUnitById).FirstOrDefault(u => u != null);
        }

        private IEnumerable<CombatUnit> UpcomingUnits(int count)
        {
            CombatState combat = state?.Combat;
            if (combat == null) yield break;
            EnsureInitiativeQueue();
            List<string> queue = combat.InitiativeQueue ?? new List<string>();
            if (queue.Count == 0) yield break;
            int activeIndex = queue.FindIndex(id => id == combat.ActiveId);
            int startIndex = activeIndex >= 0 ? activeIndex : 0;
            int yielded = 0;
            HashSet<string> currentRoundYielded = new HashSet<string>();
            for (int i = startIndex; i < queue.Count && yielded < count; i++)
            {
                CombatUnit unit = LiveUnitById(queue[i]);
                if (unit == null || currentRoundYielded.Contains(unit.Id)) continue;
                currentRoundYielded.Add(unit.Id);
                yielded++;
                yield return unit;
            }
            if (yielded >= count) yield break;

            foreach (CombatUnit unit in InitiativeOrderFor(combat))
            {
                if (yielded >= count) yield break;
                yielded++;
                yield return unit;
            }
        }

        private IEnumerable<TurnQueueEntry> UpcomingTurnEntries(int count)
        {
            CombatState combat = state?.Combat;
            if (combat == null) yield break;
            EnsureInitiativeQueue();
            List<string> queue = combat.InitiativeQueue ?? new List<string>();
            if (queue.Count == 0) yield break;

            int activeIndex = queue.FindIndex(id => id == combat.ActiveId);
            int startIndex = activeIndex >= 0 ? activeIndex : 0;
            int yielded = 0;
            HashSet<string> currentRoundYielded = new HashSet<string>();
            for (int i = startIndex; i < queue.Count && yielded < count; i++)
            {
                CombatUnit unit = LiveUnitById(queue[i]);
                if (unit == null || currentRoundYielded.Contains(unit.Id)) continue;
                currentRoundYielded.Add(unit.Id);
                yielded++;
                yield return new TurnQueueEntry(unit, false);
            }
            if (yielded >= count) yield break;

            bool markedNextRound = false;
            foreach (CombatUnit unit in InitiativeOrderFor(combat))
            {
                if (yielded >= count) yield break;
                yielded++;
                yield return new TurnQueueEntry(unit, !markedNextRound);
                markedNextRound = true;
            }
        }

        private sealed class TurnQueueEntry
        {
            public readonly CombatUnit Unit;
            public readonly bool StartsNextRound;

            public TurnQueueEntry(CombatUnit unit, bool startsNextRound)
            {
                Unit = unit;
                StartsNextRound = startsNextRound;
            }
        }

        private string AttackPreview(CombatUnit attacker, CombatUnit target)
        {
            CombatAttackForecast forecast = AttackForecast(attacker, target);
            if (!forecast.Legal) return AttackForecastBlockedText(forecast, attacker);
            string guard = forecast.Guarded ? " / guarded" : "";
            string mode = forecast.Ranged ? "shot" : IsRangedAttackProfile(attacker) ? "melee engaged" : "melee";
            string status = CombatStatusPreview(attacker, target);
            return $"{mode}: {forecast.HitChance}% hit / {forecast.MinDamage}-{forecast.MaxDamage} {forecast.DamageType}{AttackStatNote(attacker)}\n{forecast.DamageMatch}{guard}{status}";
        }

        private string AttackForecastBlockedText(CombatAttackForecast forecast, CombatUnit attacker)
        {
            switch (forecast.BlockReason)
            {
                case AttackForecastBlockReason.FriendlyTarget: return "friendly target";
                case AttackForecastBlockReason.DefeatedTarget: return "target defeated";
                case AttackForecastBlockReason.OutOfRange:
                    return $"out of reach / {AttackModeLabel(attacker).ToLowerInvariant()} range {forecast.Range}";
                case AttackForecastBlockReason.LineOfSight: return "covered\nline of sight blocked";
                default: return CombatThreatRules.BlockLabel(forecast.BlockReason);
            }
        }

        private string AttackStatNote(CombatUnit attacker)
        {
            PartyMember member = PartyMemberForUnit(attacker);
            return member == null ? "" : $" / scales {WeaponPrimaryStatLabel(member)}";
        }

        private string CoverAttackPreview(CombatUnit attacker, Point cover)
        {
            if (attacker == null || !IsBreakableCover(cover)) return "";
            int distance = Distance(attacker.X, attacker.Y, cover.X, cover.Y);
            int attackRange = EffectiveAttackRangeTo(attacker, cover.X, cover.Y);
            bool ranged = UsesRangedAttackAt(attacker, attacker.X, attacker.Y, cover.X, cover.Y);
            if (distance > attackRange) return $"cover out of reach / {AttackModeLabel(attacker).ToLowerInvariant()} range {attackRange}";
            if (ranged && !HasLineOfSight(attacker.X, attacker.Y, cover.X, cover.Y, true)) return "cover blocked\nline of sight blocked";
            int damage = CoverBreakDamage(attacker, cover);
            int current = CoverIntegrity(cover);
            string time = cover.Duration > 0 ? $" / {cover.Duration} rounds" : "";
            return $"{(ranged ? "shoot" : "strike")} {CoverName(cover)} / {current} integrity{time}\nthis hit removes {damage}";
        }

        private string RitualAttackPreview(CombatUnit attacker, Point ritual)
        {
            if (attacker == null || !IsDisruptableRitual(ritual)) return "";
            int distance = Distance(attacker.X, attacker.Y, ritual.X, ritual.Y);
            int attackRange = EffectiveAttackRangeTo(attacker, ritual.X, ritual.Y);
            bool ranged = UsesRangedAttackAt(attacker, attacker.X, attacker.Y, ritual.X, ritual.Y);
            if (distance > attackRange) return $"ritual out of reach / {AttackModeLabel(attacker).ToLowerInvariant()} range {attackRange}";
            if (ranged && !HasLineOfSight(attacker.X, attacker.Y, ritual.X, ritual.Y, true)) return "ritual blocked\nline of sight blocked";
            int damage = RitualDisruptionDamage(attacker, ritual);
            int current = RitualIntegrity(ritual);
            string result = damage >= current ? "breaks it before it opens" : $"leaves {current - damage} integrity";
            return $"{(ranged ? "shoot" : "strike")} {RitualName(ritual)} / {current} integrity / opens in {Mathf.Max(1, ritual.Duration)}\nthis hit removes {damage}: {result}";
        }

        private string CombatStatusPreview(CombatUnit attacker, CombatUnit target)
        {
            List<string> notes = new List<string>();
            if (attacker != null && attacker.Hexed > 0) notes.Add($"attacker H{attacker.Hexed}");
            if (target != null)
            {
                string statuses = StatusCompactLine(target);
                if (!string.IsNullOrEmpty(statuses)) notes.Add("target " + statuses);
                if (target.Sleeping > 0) notes.Add("sleep wakes on hit");
            }
            return notes.Count == 0 ? "" : "\n" + string.Join(" / ", notes.Take(3).ToArray());
        }

        private string FormulaPreview(CombatUnit caster, FormulaDef formula, CombatUnit target, int x, int y)
        {
            if (formula == null || caster == null) return "";
            int range = EffectiveFormulaRange(formula, caster);
            int manaCost = EffectiveFormulaMana(formula, caster);
            if (Distance(caster.X, caster.Y, x, y) > range) return $"out of spell range {range}";
            if (!CanTargetFormula(formula, caster, target, x, y))
            {
                if (formula.Effect == "summon" && !CanSummonFormulaAt(formula, caster, x, y, out string summonReason)) return summonReason;
                return FormulaTargetPrompt(formula);
            }
            if (!HasFormulaLineOfSight(formula, caster, x, y)) return FormulaSightBlockText(formula) + "\nline of sight blocked";
            if (caster.Mana < manaCost) return $"needs {manaCost} mana";
            if (formula.Effect == "dispel")
            {
                Point field = ObstacleAt(x, y);
                if (field == null) return FormulaTargetPrompt(formula);
                string fieldName = IsDisruptableRitual(field) ? RitualName(field) : TerrainDescription(field.Kind).TrimEnd('.');
                string stakes = IsDisruptableRitual(field)
                    ? $" / {RitualIntegrity(field)} integrity / opens in {Mathf.Max(1, field.Duration)}"
                    : "";
                return $"{formula.Name}: seal {fieldName}{stakes}\nremoves the field immediately / {manaCost} MP{FormulaFocusNote(formula, caster)}";
            }
            if (formula.Code == "RSG")
            {
                Vector2Int baseDamage = FormulaDamagePreview(formula, caster, null);
                int minimum = LightningPowerRules.ThunderclapDamage(baseDamage.x);
                int maximum = LightningPowerRules.ThunderclapDamage(baseDamage.y);
                int adjacent = AdjacentEnemies(caster).Count();
                return $"{formula.Name}: {minimum}-{maximum} shock to {adjacent} adjacent {(adjacent == 1 ? "enemy" : "enemies")}\npush 1 / blocked push collides and stuns / {manaCost} MP{FormulaFocusNote(formula, caster)}";
            }
            if (formula.Code == "CLT")
            {
                Vector2Int chainDamage = FormulaDamagePreview(formula, caster, target);
                int targets = target == null ? 0 : BuildLightningChain(target).Count;
                return $"{formula.Name}: {chainDamage.x}-{chainDamage.y} shock, then 75% / 55% / 40%\n{targets} linked {(targets == 1 ? "target" : "targets")} / jumps 2, or 3 from conductive terrain / {manaCost} MP{FormulaFocusNote(formula, caster)}";
            }
            if (formula.Code == "AST")
            {
                Vector2Int stormDamage = FormulaDamagePreview(formula, caster, target);
                int outerMinimum = LightningPowerRules.TempestDamage(stormDamage.x, false);
                int outerMaximum = LightningPowerRules.TempestDamage(stormDamage.y, false);
                int targets = target == null || state?.Combat?.Units == null
                    ? 0
                    : state.Combat.Units.Count(unit => unit.Side == UnitSide.Enemy
                        && unit.Hp > 0
                        && Distance(target.X, target.Y, unit.X, unit.Y) <= LightningPowerRules.TempestRadius);
                return $"{formula.Name}: {stormDamage.x}-{stormDamage.y} center / {outerMinimum}-{outerMaximum} radius 2\n{targets} {(targets == 1 ? "enemy" : "enemies")} / 35% stun before resistance / {manaCost} MP{FormulaFocusNote(formula, caster)}";
            }
            if (formula.Effect == "damage" || formula.Effect == "drain")
            {
                Vector2Int damage = FormulaDamagePreview(formula, caster, target);
                string type = string.IsNullOrEmpty(formula.DamageType) ? "magic" : formula.DamageType;
                string spill = formula.Splash ? $" / splash {SplashTargetCount(target, UnitSide.Enemy)}" : "";
                string status = string.IsNullOrEmpty(formula.Status) ? "" : $" / {StatusLabel(formula.Status)} {StatusChanceText(target, formula.Status, caster, 0.42f, true)}";
                string drain = formula.Effect == "drain" ? " / heals caster" : "";
                string arc = FormulaArcsOverCover(formula) && !HasLineOfSight(caster.X, caster.Y, x, y, true) ? " / arcs cover" : "";
                string current = target == null ? "" : StatusCompactLine(target);
                current = string.IsNullOrEmpty(current) ? "" : $" / target {current}";
                string terrain = FormulaHitTerrainPreview(formula, target);
                terrain = string.IsNullOrEmpty(terrain) ? "" : $"\nterrain: {terrain}";
                string resonance = FormulaStatusResonancePreview(formula, target);
                resonance = string.IsNullOrEmpty(resonance) ? "" : $"\nresonance: {resonance}";
                return $"{formula.Name}: {damage.x}-{damage.y} {type}{spill}{drain}{arc}\n{DamageMatchNote(target, type)}{status}{current} / {manaCost} MP{FormulaFocusNote(formula, caster)}{FormulaStatNote(formula, caster)}{terrain}{resonance}";
            }
            if (formula.Effect == "terrain")
            {
                return $"{formula.Name}: {formula.Hint}\n{TerrainReactionPreview(formula, x, y)}{FormulaFocusNote(formula, caster)}";
            }
            if (formula.Effect == "summon")
            {
                int skill = SkillValue(caster.Skills, FormulaSkill(formula, caster));
                int turns = Mathf.Max(1, formula.Duration + (IsFocusedCaster(caster) ? 1 : 0));
                int power = Mathf.Max(3, formula.Power + skill / 5 + FormulaStatPowerBonus(formula, caster) + (IsFocusedCaster(caster) ? 1 : 0));
                int hp = SummonPreviewHp(formula, caster);
                int burden = SummonBurden(formula.SummonRole);
                int current = ActiveSummonBurdenFor(caster);
                int max = MaxPactSummonBurdenFor(caster);
                string arc = FormulaArcsOverCover(formula) && !HasLineOfSight(caster.X, caster.Y, x, y, true) ? " / arcs cover" : "";
                return $"{formula.Name}: {SummonDisplayName(formula.SummonRole)} {turns} turns / {hp} HP{arc}\nclaws {Mathf.Max(2, power - 3)}-{SummonPreviewMaxDamage(formula, power)} death / pact {current}+{burden}/{max} / {manaCost} MP{FormulaFocusNote(formula, caster)}{FormulaStatNote(formula, caster)}";
            }
            if (formula.Effect == "teleport")
            {
                int distance = Distance(caster.X, caster.Y, x, y);
                int adjacent = state?.Combat?.Units == null
                    ? 0
                    : state.Combat.Units.Count(unit => unit.Side == UnitSide.Enemy
                        && unit.Hp > 0
                        && Distance(x, y, unit.X, unit.Y) <= 1);
                Vector2Int arrivalDamage = FormulaDamagePreview(formula, caster, null);
                return formula.Code == "VST"
                    ? $"{formula.Name}: teleport {distance} tile{(distance == 1 ? "" : "s")} / shock {adjacent} nearby {(adjacent == 1 ? "enemy" : "enemies")} for {LightningPowerRules.ThunderStepDamage(arrivalDamage.x)}-{LightningPowerRules.ThunderStepDamage(arrivalDamage.y)}\nignores intervening units and cover / {manaCost} MP{FormulaFocusNote(formula, caster)}"
                    : $"{formula.Name}: teleport {distance} tile{(distance == 1 ? "" : "s")}\nignores intervening units and cover / {manaCost} MP{FormulaFocusNote(formula, caster)}";
            }
            if (formula.Effect == "transform")
            {
                int turns = Mathf.Max(1, formula.Duration + (IsFocusedCaster(caster) ? 1 : 0));
                int heal = 6 + Mathf.Max(0, UnitIntelligenceScore(caster) - 10) / 3;
                return $"{formula.Name}: greater demon form {turns} turns\n+4 physical/pact power / -2 incoming damage / heal {heal} / ward + regen / {manaCost} MP{FormulaFocusNote(formula, caster)}";
            }
            if (formula.Effect == "heal")
            {
                Vector2Int heal = FormulaHealPreview(formula, caster);
                string spread = formula.Splash ? $" / splash {SplashTargetCount(target, target?.Side ?? UnitSide.Party)}" : "";
                return $"{formula.Name}: +{heal.x}-{heal.y} HP{spread}\n{manaCost} MP{FormulaFocusNote(formula, caster)}{FormulaStatNote(formula, caster)}";
            }
            if (formula.Effect == "cure")
            {
                return $"{formula.Name}: cleanse afflictions\npoison bleed web stun sleep hex / {manaCost} MP{FormulaFocusNote(formula, caster)}";
            }
            if (formula.Effect == "status")
            {
                bool hostile = formula.Target == "enemy";
                string spread = formula.Splash ? $" / splash {SplashTargetCount(target, target?.Side ?? UnitSide.Party)}" : "";
                string chance = StatusChanceText(target, formula.Status, caster, hostile ? 0.86f : 1f, hostile);
                return $"{formula.Name}: {StatusLabel(formula.Status)} {chance}{spread}\n{Mathf.Max(1, formula.Duration)} turns / {manaCost} MP{FormulaFocusNote(formula, caster)}";
            }
            return $"{formula.Name}: {formula.Hint}\n{manaCost} MP{FormulaFocusNote(formula, caster)}";
        }

        private string AbilityPreview(CombatUnit active, CombatUnit target, int x, int y)
        {
            MartialAbility ability = AbilityDef(pendingAbilityId);
            if (ability == null) return "choose a combat skill first";
            string reason;
            if (!AbilityUsableNow(active, ability, out reason)) return reason;
            if (!CanTargetAbility(active, ability, target, x, y, out reason)) return reason;
            if (ability.Id == "charge")
            {
                int raw = ChargeRawDamage(active);
                int damage = PreviewDamageAfterTraits(target, raw, "physical");
                Point landing = BestChargeLanding(active, target);
                string landingLine = landing == null ? "no landing" : $"lands {Distance(active.X, active.Y, landing.X, landing.Y)} away";
                return $"Charge: {damage} physical{AbilityStatNote(active, ability.Id)} / stun 1\n{landingLine} / path-aware rush";
            }
            if (ability.Id == "execute")
            {
                int raw = ExecuteRawDamage(active);
                int damage = PreviewDamageAfterTraits(target, raw, "physical");
                return $"Execute: {damage} physical{AbilityStatNote(active, ability.Id)}\nrequires target at 35% HP or lower";
            }
            if (ability.Id == "shieldbash")
            {
                int damage = PreviewDamageAfterTraits(target, ShieldBashRawDamage(active), "physical");
                int collision = PreviewDamageAfterTraits(target, LightningPowerRules.CollisionDamage(ShieldBashRawDamage(active)), "physical");
                return $"Shield Bash: {damage} physical{AbilityStatNote(active, ability.Id)} / push 1\nblocked push: +{collision} collision and stun";
            }
            if (ability.Id == "cleave")
            {
                int damage = PreviewDamageAfterTraits(target, CleaveRawDamage(active), "physical");
                CombatUnit secondary = CleaveSecondaryTarget(active, target);
                string extra = secondary == null ? "no second target" : $"clips {secondary.Name}";
                return $"Cleave: {damage} physical{AbilityStatNote(active, ability.Id)}\n{extra}";
            }
            if (ability.Id == "ambush")
            {
                bool hidden = active.Stealthed > 0;
                int raw = AmbushRawDamage(active, hidden);
                int damage = PreviewDamageAfterTraits(target, raw, "physical");
                return $"Ambush: {damage} physical{AbilityStatNote(active, ability.Id)}{(hidden ? " / stun 1" : "")}\n{(hidden ? "from stealth" : "strong opening strike")}";
            }
            if (ability.Id == "throwknife")
            {
                int damage = PreviewDamageAfterTraits(target, ThrowKnifeRawDamage(active), "physical");
                return $"Throw Knife: {damage} physical{AbilityStatNote(active, ability.Id)} / bleed 2\nshort sight-line attack";
            }
            if (ability.Id == "eviscerate")
            {
                bool hidden = active.Stealthed > 0;
                int raw = EviscerateRawDamage(active, target, hidden);
                int damage = PreviewDamageAfterTraits(target, raw, "physical");
                return $"Eviscerate: {damage} physical{AbilityStatNote(active, ability.Id)} / bleed 3\nbonus damage against bleeding targets";
            }
            if (ability.Id == "hamstring")
            {
                int damage = PreviewDamageAfterTraits(target, HamstringRawDamage(active), "physical");
                return $"Hamstring: {damage} physical{AbilityStatNote(active, ability.Id)} / hobble 2\nbleeds and pins the target";
            }
            if (ability.Id == "aimedshot")
            {
                int damage = PreviewDamageAfterTraits(target, AimedShotRawDamage(active, target), "physical");
                return $"Aimed Shot: {damage} physical{AbilityStatNote(active, ability.Id)}\nneeds sight / bonus against marked targets";
            }
            if (ability.Id == "pinningshot")
            {
                int damage = PreviewDamageAfterTraits(target, PinningShotRawDamage(active), "physical");
                return $"Pinning Shot: {damage} physical{AbilityStatNote(active, ability.Id)} / pin 1-2\nholds ranged enemies longer";
            }
            if (ability.Id == "volley")
            {
                int damage = PreviewDamageAfterTraits(target, VolleyRawDamage(active), "physical");
                int splash = target == null ? 0 : state.Combat.Units.Count(u => u.Side == UnitSide.Enemy && u.Hp > 0 && u.Id != target.Id && Distance(u.X, u.Y, target.X, target.Y) <= 1);
                return $"Volley: {damage} physical{AbilityStatNote(active, ability.Id)} / splash {splash}\narcing arrows can pass over cover";
            }
            if (ability.Id == "scoutmark")
            {
                return $"Scout Mark: break guard / strip {(target != null && target.Shielded > 0 ? 1 : 0)} ward\nmark 2; party damage improves";
            }
            if (ability.Id == "broadheadshot")
            {
                int damage = PreviewDamageAfterTraits(target, BroadheadShotRawDamage(active), "physical");
                return $"Broadhead Shot: {damage} physical{AbilityStatNote(active, ability.Id)} / bleed 3\nsets up physical pressure";
            }
            if (ability.Id == "disruptingshot")
            {
                int damage = PreviewDamageAfterTraits(target, DisruptingShotRawDamage(active), "physical");
                return $"Disrupting Shot: {damage} physical{AbilityStatNote(active, ability.Id)} / stun 1\nstronger hit chance against casters";
            }
            if (ability.Id == "riftpounce")
            {
                int damage = PreviewDamageAfterTraits(target, RiftPounceRawDamage(active), "death");
                Point landing = BestRiftPounceLanding(active, target);
                return $"Rift Pounce: {damage} death{AbilityStatNote(active, ability.Id)}\n{(landing == null ? "no open landing" : "lands beside target / ignores intervening terrain")}";
            }
            if (ability.Id == "soulrend")
            {
                int damage = PreviewDamageAfterTraits(target, SoulRendRawDamage(active), "death");
                return $"Soul Rend: {damage} death{AbilityStatNote(active, ability.Id)}\nheal up to {Mathf.Max(1, damage / 2)} HP from actual damage";
            }
            return ability.Name;
        }

        private int MoveCostTo(CombatUnit active, int x, int y)
        {
            if (active == null) return UnreachableMoveCost;
            if (x == active.X && y == active.Y) return 0;
            if (x < 0 || x >= CombatW || y < 0 || y >= CombatH) return UnreachableMoveCost;
            return ReachableMoveCosts(active)[x, y];
        }

        private int[,] ReachableMoveCosts(CombatUnit active)
        {
            return ReachableMoveCosts(active, UnitMoveAllowance(active));
        }

        private int[,] ReachableMoveCosts(CombatUnit active, int maxCost)
        {
            return CombatGridRules.ReachableMoveCosts(
                active,
                CombatW,
                CombatH,
                maxCost,
                UnreachableMoveCost,
                CanEnterMoveTile,
                (unit, x, y) => 1 + TerrainMoveExtraCost(ObstacleAt(x, y), unit));
        }

        private IReadOnlyList<Vector2Int> ReachableMovePath(
            CombatUnit active,
            int destinationX,
            int destinationY,
            int maxCost)
        {
            if (active == null) return Array.Empty<Vector2Int>();
            int[,] costs = ReachableMoveCosts(active, maxCost);
            return CombatGridRules.ShortestReachablePath(
                active,
                costs,
                destinationX,
                destinationY,
                UnreachableMoveCost,
                (unit, x, y) => 1 + TerrainMoveExtraCost(ObstacleAt(x, y), unit));
        }

        private bool CanEnterMoveTile(CombatUnit active, int x, int y)
        {
            if (x < 0 || x >= CombatW || y < 0 || y >= CombatH) return false;
            if (IsBlockingTerrain(ObstacleAt(x, y))) return false;
            CombatUnit blocker = UnitAt(x, y);
            return blocker == null || active != null && !string.IsNullOrEmpty(active.Id) && blocker.Id == active.Id;
        }

        private int TerrainMoveExtraCost(Point terrain, CombatUnit active = null)
        {
            if (terrain == null) return 0;
            int extra = 0;
            if (terrain.Kind == "web") extra = 2;
            if (terrain.Kind == "ice" || terrain.Kind == "gas") extra = 1;
            if (terrain.Kind == "curse") extra = 1;
            if (terrain.Kind == "glyph" || terrain.Kind == "demonrift") extra = 1;
            if (extra > 0 && active != null && string.Equals(active.Race, "fenkin", StringComparison.OrdinalIgnoreCase)) extra = Mathf.Max(0, extra - 1);
            return extra;
        }

        private string TerrainPreviewLine(Point terrain)
        {
            if (terrain == null) return "";
            if (terrain.Kind == "tree") return terrain.Duration > 0 ? $"\ntree cover: blocks shots, arcs pass, {CoverIntegrity(terrain)} integrity, {terrain.Duration} rounds" : $"\ntree cover: blocks shots, arcs pass, {CoverIntegrity(terrain)} integrity";
            if (terrain.Kind == "stone") return $"\nstone block: blocks shots, {CoverIntegrity(terrain)} integrity";
            string rounds = terrain.Duration > 0 ? $", {terrain.Duration} round{(terrain.Duration == 1 ? "" : "s")} left" : "";
            if (terrain.Kind == "fire") return $"\nfire: hurts at turn start, burns webs, reacts with gas/ice{rounds}";
            if (terrain.Kind == "gas") return $"\ngas: poison risk, +1 move, fire/shock reactive{rounds}";
            if (terrain.Kind == "smoke") return $"\nsmoke: blocks direct sight, movement remains free{rounds}";
            if (terrain.Kind == "web") return $"\nweb: snare risk, +2 move, fire/shock reactive{rounds}";
            if (terrain.Kind == "ice") return $"\nice: slip risk, +1 move, fire/cold/shock reactive{rounds}";
            if (terrain.Kind == "sanctuary") return $"\nsanctuary: allies mend/ward/cleanse, enemies burn{rounds}";
            if (terrain.Kind == "curse") return $"\ncurse: mind harm, hex risk, cracks wards, +1 move{rounds}";
            if (terrain.Kind == "glyph") return $"\nsummon glyph: {RitualIntegrity(terrain)} integrity, opens in {Mathf.Max(1, terrain.Duration)} round{(terrain.Duration == 1 ? "" : "s")} as kobold reinforcement; +1 move";
            if (terrain.Kind == "demonrift") return $"\ndemon rift: {RitualIntegrity(terrain)} integrity, opens in {Mathf.Max(1, terrain.Duration)} round{(terrain.Duration == 1 ? "" : "s")} as lesser demon; +1 move";
            return "";
        }

        private string TerrainLogWarning(Point terrain)
        {
            if (terrain == null) return "";
            if (terrain.Kind == "fire") return "Fire burns at turn start, burns webs away, and reacts with gas or ice.";
            if (terrain.Kind == "gas") return "Gas may poison anyone lingering there; fire ignites it and shock conducts through it.";
            if (terrain.Kind == "smoke") return "Smoke blocks direct shots and spells without slowing movement.";
            if (terrain.Kind == "web") return "Webbing may hold that position; fire clears it and shock conducts through it.";
            if (terrain.Kind == "ice") return "Ice may slip the next step; fire melts it and shock conducts through it.";
            if (terrain.Kind == "sanctuary") return "Sanctuary wards and cleanses allies while burning enemies at turn start.";
            if (terrain.Kind == "curse") return "Doomed ground harms the mind, cracks wards, and may hex anyone lingering there.";
            if (terrain.Kind == "glyph") return $"The summoning glyph opens in {Mathf.Max(1, terrain.Duration)} round{(terrain.Duration == 1 ? "" : "s")}. Attack it or cast Rift Seal before kobolds answer.";
            if (terrain.Kind == "demonrift") return $"The demon rift opens in {Mathf.Max(1, terrain.Duration)} round{(terrain.Duration == 1 ? "" : "s")}. Attack it or cast Rift Seal before a lesser demon crosses.";
            return "";
        }

        private Color TerrainHighlightColor(Point terrain, float alpha)
        {
            if (terrain == null) return Hex("58b7a5", alpha);
            if (terrain.Kind == "fire") return Hex("c65c3b", Mathf.Max(alpha, 0.26f));
            if (terrain.Kind == "gas") return Hex("8fc27b", Mathf.Max(alpha, 0.24f));
            if (terrain.Kind == "smoke") return Hex("8fa7a2", Mathf.Max(alpha, 0.26f));
            if (terrain.Kind == "web") return Hex("d9d3c4", Mathf.Max(alpha, 0.24f));
            if (terrain.Kind == "ice") return Hex("9ad6e8", Mathf.Max(alpha, 0.24f));
            if (terrain.Kind == "sanctuary") return Hex("58b7a5", Mathf.Max(alpha, 0.28f));
            if (terrain.Kind == "curse") return Hex("8d6dcc", Mathf.Max(alpha, 0.30f));
            if (terrain.Kind == "glyph") return Hex("d9d3c4", Mathf.Max(alpha, 0.28f));
            if (terrain.Kind == "demonrift") return Hex("8d6dcc", Mathf.Max(alpha, 0.30f));
            return Hex("58b7a5", alpha);
        }

        private Vector2Int AttackDamagePreview(CombatUnit attacker, CombatUnit target)
        {
            if (attacker == null || target == null) return new Vector2Int(0, 0);
            return AttackDamagePreviewAt(attacker, target, attacker.X, attacker.Y);
        }

        private Vector2Int AttackDamagePreviewAt(CombatUnit attacker, CombatUnit target, int attackerX, int attackerY)
        {
            if (attacker == null || target == null) return new Vector2Int(0, 0);
            string skill = AttackSkillNameAt(attacker, target, attackerX, attackerY);
            AttackDamageProfile damageProfile = AttackRules.BuildDamageProfile(attacker, target, SkillValue(attacker.Skills, skill), WarriorEnrageBonus(attacker), DemonFormAttackBonus(attacker));
            return new Vector2Int(
                PreviewDamageAfterTraits(target, damageProfile.MinRawDamage, damageProfile.DamageType),
                PreviewDamageAfterTraits(target, damageProfile.MaxRawDamage, damageProfile.DamageType));
        }

        private Vector2Int FormulaDamagePreview(FormulaDef formula, CombatUnit caster, CombatUnit target)
        {
            if (formula == null || caster == null) return new Vector2Int(0, 0);
            string skill = FormulaSkill(formula, caster);
            int skillBonus = SkillValue(caster.Skills, skill) / 2;
            int focusBonus = IsFocusedCaster(caster) ? 3 : 0;
            int raceBonus = RaceFormulaPowerBonus(caster, formula);
            int resonanceBonus = FormulaStatusResonanceDamageBonus(formula, target);
            int statBonus = FormulaStatPowerBonus(formula, caster);
            int demonBonus = DemonFormFormulaPowerBonus(caster, formula);
            int minRaw = Mathf.Max(1, formula.Power + skillBonus + statBonus + focusBonus + raceBonus + demonBonus + resonanceBonus);
            int maxRaw = Mathf.Max(1, formula.Power + skillBonus + statBonus + focusBonus + raceBonus + demonBonus + resonanceBonus + 5);
            string type = string.IsNullOrEmpty(formula.DamageType) ? "magic" : formula.DamageType;
            return new Vector2Int(PreviewDamageAfterTraits(target, minRaw, type), PreviewDamageAfterTraits(target, maxRaw, type));
        }

        private Vector2Int FormulaHealPreview(FormulaDef formula, CombatUnit caster)
        {
            if (formula == null || caster == null) return new Vector2Int(0, 0);
            int skillBonus = SkillValue(caster.Skills, formula.Skill) / 2;
            int statBonus = FormulaStatPowerBonus(formula, caster);
            int min = Mathf.Max(1, formula.Power + skillBonus + statBonus);
            int max = Mathf.Max(min, formula.Power + skillBonus + statBonus + 4);
            return new Vector2Int(min, max);
        }

        private int SplashTargetCount(CombatUnit target, UnitSide side)
        {
            if (target == null || state?.Combat?.Units == null) return 0;
            return state.Combat.Units.Count(u => u.Side == side && u.Hp > 0 && u.Id != target.Id && Distance(u.X, u.Y, target.X, target.Y) <= 1);
        }

        private float StatusApplyChance(CombatUnit target, string status, CombatUnit source, float chance, bool hostile)
        {
            if (!hostile) return 1f;
            float rollChance = chance + FormulaStatusStatChanceBonus(source) - (target?.MagicResist ?? 0) * 0.07f;
            if (status == "sleep" && target != null && target.Fearless) rollChance *= 0.45f;
            return Mathf.Clamp01(rollChance);
        }

        private string StatusChanceText(CombatUnit target, string status, CombatUnit source, float chance, bool hostile)
        {
            if (target == null) return hostile ? "needs mark" : "100%";
            return $"{Mathf.RoundToInt(StatusApplyChance(target, status, source, chance, hostile) * 100f)}%";
        }

        private int PreviewDamageAfterTraits(CombatUnit target, int amount, string damageType)
        {
            if (target == null) return Mathf.Max(1, amount);
            string type = string.IsNullOrEmpty(damageType) ? "physical" : damageType;
            float multiplier = 1f;
            if (HasTag(target.Resist, type)) multiplier *= 0.55f;
            if (HasTag(target.Weakness, type)) multiplier *= 1.45f;
            if (target.Hexed > 0) multiplier *= 1.20f;
            int guard = target.Guarding ? Mathf.Max(2, target.GuardBonus) : 0;
            int shield = target.Shielded > 0 ? 3 : 0;
            return Mathf.Max(1, Mathf.RoundToInt(amount * multiplier) - guard - shield - GearDamageReduction(target, type) - RaceDamageReduction(target, type) - DemonFormDamageReduction(target));
        }

        private string DamageMatchNote(CombatUnit target, string damageType)
        {
            if (target == null) return "no target";
            string type = string.IsNullOrEmpty(damageType) ? "physical" : damageType;
            if (HasTag(target.Resist, type)) return "resists";
            if (HasTag(target.Weakness, type)) return "weak";
            return "normal";
        }

        private CombatUnit UnitAt(int x, int y)
        {
            return state.Combat.Units.FirstOrDefault(u => u.Hp > 0 && u.X == x && u.Y == y);
        }

        private bool CanStandAt(int x, int y)
        {
            return x >= 0 && x < CombatW && y >= 0 && y < CombatH && !IsObstacle(x, y) && UnitAt(x, y) == null;
        }

        private bool CanGrowTreeAt(int x, int y)
        {
            if (x < 0 || x >= CombatW || y < 0 || y >= CombatH || UnitAt(x, y) != null) return false;
            Point existing = ObstacleAt(x, y);
            return existing == null || !IsBlockingTerrain(existing);
        }

        private Point ObstacleAt(int x, int y)
        {
            if (state?.Combat?.Obstacles == null) return null;
            return state.Combat.Obstacles.FirstOrDefault(o => o.X == x && o.Y == y);
        }

        private bool IsObstacle(int x, int y)
        {
            return IsBlockingTerrain(ObstacleAt(x, y));
        }

        private bool IsBlockingTerrain(Point point)
        {
            return point != null && CombatTerrainRules.BlocksMovement(point.Kind);
        }

        private bool IsSightBlockingTerrain(Point point)
        {
            return point != null && CombatTerrainRules.BlocksSight(point.Kind);
        }

        private bool HasLineOfSight(int ax, int ay, int bx, int by, bool missiles)
        {
            return CombatGridRules.HasLineOfSight(ax, ay, bx, by, CombatW, CombatH, missiles, (x, y) => IsSightBlockingTerrain(ObstacleAt(x, y)));
        }

        private IEnumerable<Vector2Int> SupercoverLine(int ax, int ay, int bx, int by)
        {
            return CombatGridRules.SupercoverLine(ax, ay, bx, by, CombatW, CombatH);
        }

        private int TileAt(MapData map, int x, int y)
        {
            if (map == null || x < 0 || y < 0 || x >= map.Width || y >= map.Height) return 0;
            return map.Tiles[y * map.Width + x];
        }

        private void SetTile(MapData map, int x, int y, int tile)
        {
            if (map == null || map.Tiles == null || x < 0 || y < 0 || x >= map.Width || y >= map.Height) return;
            map.Tiles[y * map.Width + x] = tile == 0 ? 0 : 1;
            ExplorationSurfaceRules.EnsureGrid(map);
        }

        private MapObject ObjectAt(MapData map, int x, int y)
        {
            return map?.FindObjectAt(x, y);
        }

        private void RemoveObject(MapObject obj)
        {
            if (state?.Map?.Objects == null || obj == null) return;
            state.Map.Objects.Remove(obj);
            state.Map.InvalidateObjectLookup();
        }

        private int Distance(int ax, int ay, int bx, int by)
        {
            return CombatGridRules.ManhattanDistance(ax, ay, bx, by);
        }
    }
}
