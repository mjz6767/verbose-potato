using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private void EnsureCombatHudScreen()
        {
            if (combatHudScreen != null && combatHudScreen.IsReady) return;
            if (combatHudScreen != null)
            {
                Destroy(combatHudScreen.gameObject);
                combatHudScreen = null;
            }
            GameObject screen = new GameObject("Combat HUD Screen");
            screen.transform.SetParent(transform, false);
            CombatHudScreen created = screen.AddComponent<CombatHudScreen>();
            created.Bind(new CombatHudScreenBindings
            {
                View = BuildCombatHudView,
                RunCommand = RunCombatHudCommand,
                RunUtility = RunCombatHudCommand,
                UndoMove = RunCombatHudUndoMove,
                CancelTarget = RunCombatHudCancelTarget,
                ToggleTimeline = ToggleCombatTimeline,
                OpenMenu = OpenPauseMenu
            });
            created.SetVisible(false);
            combatHudScreen = created;
        }

        private void SyncCombatHudScreen()
        {
            bool visible = state != null
                && state.Mode == GameMode.Combat
                && !ShouldShowStartupSplash();
            if (visible && (combatHudScreen == null || !combatHudScreen.IsReady))
            {
                TryInitializePresentationScreen("Combat HUD recovery", EnsureCombatHudScreen, false);
            }
            if (combatHudScreen == null) return;
            bool activated = combatHudScreen.SetVisible(visible);
            combatHudScreen.SetSuppressedByImguiFallback(false);
            combatHudScreen.SetUnderlay(visible && CurrentUiOverlay() != UiOverlay.None);
            if (visible && (activated || ShouldRefreshPresentation(ref lastCombatHudRefreshKey, CombatHudRefreshKey()))) combatHudScreen.Refresh();
        }

        private string CombatHudRefreshKey()
        {
            if (state?.Combat == null) return "empty";
            CombatUnit active = CurrentUnit();
            CombatUnit hovered = CombatHudHoveredUnit();
            CombatUnit focus = CombatHudTarget(active);
            int hash = 23;
            hash = unchecked(hash * 31 + state.Gold);
            hash = unchecked(hash * 31 + state.Supplies);
            hash = unchecked(hash * 31 + state.Elixirs);
            hash = unchecked(hash * 31 + state.Combat.Round);
            hash = unchecked(hash * 31 + state.Combat.MovePoints);
            hash = unchecked(hash * 31 + (state.Combat.ActionAvailable ? 1 : 0));
            hash = unchecked(hash * 31 + (int)state.Combat.Phase);
            hash = unchecked(hash * 31 + (combatAdvancePending ? 1 : 0));
            hash = unchecked(hash * 31 + (combatResolutionLabel ?? "").GetHashCode());
            hash = unchecked(hash * 31 + (int)selectedAction);
            hash = unchecked(hash * 31 + (combatTimelineExpanded ? 1 : 0));
            hash = unchecked(hash * 31 + (pendingFormulaCode ?? "").GetHashCode());
            hash = unchecked(hash * 31 + (pendingAbilityId ?? "").GetHashCode());
            hash = unchecked(hash * 31 + (active?.Id ?? "").GetHashCode());
            hash = unchecked(hash * 31 + (hovered?.Id ?? "").GetHashCode());
            hash = unchecked(hash * 31 + (focus?.Id ?? "").GetHashCode());
            if (active != null)
            {
                hash = unchecked(hash * 31 + active.Hp);
                hash = unchecked(hash * 31 + active.Mana);
                hash = unchecked(hash * 31 + active.X);
                hash = unchecked(hash * 31 + active.Y);
            }
            if (hovered != null)
            {
                hash = unchecked(hash * 31 + hovered.Hp);
                hash = unchecked(hash * 31 + hovered.Mana);
                hash = unchecked(hash * 31 + hovered.X);
                hash = unchecked(hash * 31 + hovered.Y);
            }
            if (focus != null)
            {
                hash = unchecked(hash * 31 + focus.Hp);
                hash = unchecked(hash * 31 + focus.GuardBonus);
                hash = unchecked(hash * 31 + (focus.Guarding ? 1 : 0));
                hash = unchecked(hash * 31 + focus.Shielded);
                hash = unchecked(hash * 31 + focus.X);
                hash = unchecked(hash * 31 + focus.Y);
            }
            if (state.Log != null && state.Log.Count > 0) hash = unchecked(hash * 31 + (state.Log[0].Text ?? "").GetHashCode());
            return "combat=" + hash;
        }

        private CombatHudView BuildCombatHudView()
        {
            if (state == null || state.Combat == null)
            {
                return new CombatHudView
                {
                    Title = GameTitle,
                    RouteLine = GameSubtitle,
                    Gold = state?.Gold.ToString() ?? "0",
                    Supplies = state?.Supplies.ToString() ?? "0",
                    Elixirs = state?.Elixirs.ToString() ?? "0",
                    RoundLine = "",
                    PhaseLine = "Combat HUD ready",
                    TacticalLine = "",
                    CommandPrompt = "Combat commands will appear when initiative begins.",
                    PlayerTurn = false,
                    TimelineExpanded = combatTimelineExpanded,
                    CanUndoMove = false,
                    CanCancelTarget = false,
                    CancelTargetLabel = "Cancel Target",
                    ActiveUnit = null,
                    TargetUnit = null,
                    Commands = Array.Empty<CombatHudCommandView>(),
                    GuardEnabled = false,
                    ElixirEnabled = false,
                    GuardReason = "No active combat.",
                    ElixirReason = "No active combat.",
                    Turns = Array.Empty<CombatHudTurnView>(),
                    Logs = BuildCombatHudLogViews()
                };
            }

            CombatUnit active = CurrentUnit();
            bool playerTurn = active != null && active.Side == UnitSide.Party;
            CombatUnit target = CombatHudTarget(active);
            return new CombatHudView
            {
                Title = GameTitle,
                RouteLine = $"{GameSubtitle} / Depth {state.Depth} / Combat",
                Gold = state.Gold.ToString(),
                Supplies = state.Supplies.ToString(),
                Elixirs = state.Elixirs.ToString(),
                RoundLine = state.Combat == null ? "" : $"Round {state.Combat.Round}",
                PhaseLine = CombatHudPhaseLine(active, playerTurn),
                TacticalLine = CombatHudTacticalLine(),
                CommandPrompt = CombatHudCommandPrompt(active, playerTurn),
                PlayerTurn = playerTurn,
                TimelineExpanded = combatTimelineExpanded,
                CanUndoMove = playerTurn && CombatLifecycle().CanUndoMove(active),
                CanCancelTarget = playerTurn && CanCancelCombatTargeting(),
                CancelTargetLabel = CombatTargetingRules.CancelLabel(selectedAction),
                TargetTitle = CombatHudTargetTitle(active, target),
                ActiveUnit = BuildCombatHudUnitView(active, true),
                TargetUnit = BuildCombatHudUnitView(target, false),
                Commands = BuildCombatHudCommandViews(active, playerTurn),
                GuardEnabled = playerTurn && ActionEnabled(ActionMode.Guard, active),
                ElixirEnabled = playerTurn && ActionEnabled(ActionMode.Elixir, active),
                GuardReason = DisabledActionReason(ActionMode.Guard, active, playerTurn),
                ElixirReason = DisabledActionReason(ActionMode.Elixir, active, playerTurn),
                Turns = BuildCombatHudTurnViews(),
                Logs = BuildCombatHudLogViews()
            };
        }

        private string CombatHudPhaseLine(CombatUnit active, bool playerTurn)
        {
            if (active == null) return "WAITING FOR INITIATIVE";
            string owner = playerTurn ? "YOUR TURN" : "ENEMY TURN";
            return $"{owner}  \u00b7  {active.Name}  \u00b7  {CombatPhaseLabel()}";
        }

        private string CombatHudTacticalLine()
        {
            if (state?.Combat == null
                || state.Combat.Round > 1
                || !EncounterGuidanceCatalog.TryFor(state.Combat.EncounterStyle, out EncounterGuidance guidance))
            {
                return "";
            }
            return guidance.Plan;
        }

        private IReadOnlyList<CombatHudTurnView> BuildCombatHudTurnViews()
        {
            combatHudTurnBuffer.Clear();
            CombatUnit active = CurrentUnit();
            foreach (TurnQueueEntry entry in UpcomingTurnEntries(6))
            {
                CombatUnit unit = entry?.Unit;
                if (unit == null) continue;
                combatHudTurnBuffer.Add(new CombatHudTurnView
                {
                    Name = unit.Name,
                    AccentHex = unit.Side == UnitSide.Party ? "58b7a5" : "b94b56",
                    Active = active != null && active.Id == unit.Id,
                    StartsNextRound = entry.StartsNextRound
                });
            }
            return combatHudTurnBuffer;
        }

        private IReadOnlyList<CombatHudCommandView> BuildCombatHudCommandViews(CombatUnit active, bool playerTurn)
        {
            combatHudCommandBuffer.Clear();
            bool promoteEndTurn = playerTurn && ShouldPromoteEndTurn(active);
            foreach (CombatCommandEntry command in CombatCommandPresentationRules.PrimaryCommandsFor(active))
            {
                bool actionable = playerTurn && ActionEnabled(command.Mode, active);
                bool enabled = playerTurn && CombatCommandEnabled(command.Mode, active);
                bool reviewOnly = enabled && !actionable && CanInspectCombatPowerBook(command.Mode, active);
                bool promoted = command.Mode == ActionMode.Wait && promoteEndTurn;
                bool selected = selectedAction == command.Mode;
                TryGetCombatHudCommandArt(command.Mode, out Texture2D iconTexture, out Rect iconSource);
                combatHudCommandBuffer.Add(new CombatHudCommandView
                {
                    Mode = command.Mode,
                    Label = command.Mode == ActionMode.Wait ? "End Turn" : ActionName(command.Mode, active),
                    Hotkey = command.Hotkey,
                    SubLabel = reviewOnly
                        ? "Review book"
                        : selected && enabled ? SelectedActionButtonSubLabel(command.Mode, active) : ActionButtonSubLabel(command.Mode, active),
                    Tooltip = ActionTooltipLine(command.Mode, active),
                    DisabledReason = enabled ? "" : DisabledActionReason(command.Mode, active, playerTurn),
                    IconTexture = iconTexture,
                    IconSource = iconSource,
                    Enabled = enabled,
                    Selected = selected,
                    Promoted = promoted
                });
            }
            return combatHudCommandBuffer;
        }

        private string CombatHudCommandPrompt(CombatUnit active, bool playerTurn)
        {
            if (active == null) return "Initiative is forming...";
            if (!playerTurn) return EnemyIntentLine(active);
            return $"{CombatFocusStateLine(active, true)}  |  {ActiveCommandPrompt(active)}";
        }

        private string EnemyIntentLine(CombatUnit enemy)
        {
            if (enemy == null) return "Enemy turn...";
            CombatUnit support = EnemySupportTarget(enemy);
            if (support != null)
            {
                return $"INTENT: Ward {support.Name}  |  {EnemyTacticsRules.StyleLabel(enemy)}";
            }

            CombatUnit target = BestEnemyTarget(enemy);
            if (target == null) return $"INTENT: {EnemyTacticsRules.AdvanceIntent(enemy, false)}";
            Point cover = BestCoverToBreak(enemy, target, true);
            string intent;
            string forecastLine = "";
            if (cover != null && !EnemySpecialArcsOverCover(enemy))
            {
                intent = "Break " + CoverName(cover);
            }
            else
            {
                CombatAttackForecast forecast = AttackForecast(enemy, target);
                if (forecast.Legal)
                {
                    intent = EnemyTacticsRules.AttackIntent(enemy);
                    string severity = forecast.ThreatLevel == CombatThreatLevel.Severe || forecast.ThreatLevel == CombatThreatLevel.Lethal
                        ? CombatThreatRules.SeverityLabel(forecast.ThreatLevel) + " "
                        : "";
                    forecastLine = $"  |  {severity}{forecast.HitChance}% / {forecast.MinDamage}-{forecast.MaxDamage} {forecast.DamageType}";
                }
                else if (CanEnemySpecialReach(enemy, target))
                {
                    intent = EnemyTacticsRules.AttackIntent(enemy);
                    forecastLine = "  |  power in range";
                }
                else
                {
                    bool hasSight = enemy.Range <= 1 || HasLineOfSight(enemy.X, enemy.Y, target.X, target.Y, true);
                    intent = EnemyTacticsRules.AdvanceIntent(enemy, hasSight);
                }
            }
            return $"INTENT: {intent} -> {target.Name}{forecastLine}  |  {EnemyTacticsRules.StyleLabel(enemy)}";
        }

        private CombatUnit EnemyIntentFocus(CombatUnit enemy)
        {
            return EnemySupportTarget(enemy) ?? BestEnemyTarget(enemy);
        }

        private bool TryGetCombatHudCommandArt(ActionMode mode, out Texture2D texture, out Rect source)
        {
            texture = null;
            source = Rect.zero;
            if (mode == ActionMode.Ability && IsAbilityIconAtlas())
            {
                int abilityIcon = AbilityIconIndex(string.IsNullOrEmpty(pendingAbilityId) ? "whirlwind" : pendingAbilityId);
                if (abilityIcon >= 0)
                {
                    texture = abilityIconAtlas;
                    source = AbilityIconAtlasCell(abilityIcon);
                    return true;
                }
            }

            int index = ActionCombatCommandIconIndex(mode);
            if (index >= 0 && IsCombatCommandIconAtlas())
            {
                texture = combatCommandIconAtlas;
                source = CombatCommandIconAtlasCell(index);
                return true;
            }

            index = ActionCombatHudIconIndex(mode);
            if (index >= 0 && IsCombatHudUiAtlas())
            {
                texture = combatHudUiAtlas;
                source = CombatHudUiAtlasCell(index);
                return true;
            }

            index = ActionCombatSpellbookUiIconIndex(mode);
            if (index >= 0 && IsCombatSpellbookUiAtlas())
            {
                texture = combatSpellbookUiAtlas;
                source = CombatSpellbookUiAtlasCell(index);
                return true;
            }

            index = ActionSpellbookIconIndex(mode);
            if (index >= 0 && IsSpellbookUiAtlas())
            {
                texture = spellbookUiAtlas;
                source = SpellbookUiAtlasCell(index);
                return true;
            }

            index = ActionCombatUiIconIndex(mode);
            if (index >= 0 && IsCombatUiAtlas())
            {
                texture = combatUiAtlas;
                source = CombatUiAtlasCell(index);
                return true;
            }

            index = ActionMagicIconIndex(mode);
            if (index >= 0 && IsMagicUiAtlas())
            {
                texture = formulaLabArt;
                source = MagicUiAtlasCell(index);
                return true;
            }

            return false;
        }

        private CombatHudUnitView BuildCombatHudUnitView(CombatUnit unit, bool activeCard)
        {
            if (unit == null) return null;
            return new CombatHudUnitView
            {
                Name = unit.Name,
                Header = activeCard ? CombatHudActiveHeader(unit) : CombatHudTargetHeader(unit),
                StateLine = CombatHudUnitStateLine(unit, activeCard),
                StatusLine = CombatHudStatusLine(unit),
                AccentHex = unit.Side == UnitSide.Party ? "58b7a5" : "b94b56",
                Hp = unit.Hp,
                MaxHp = unit.MaxHp,
                Mana = unit.Mana,
                MaxMana = unit.MaxMana
            };
        }

        private string CombatHudStatusLine(CombatUnit unit)
        {
            string status = StatusLine(unit);
            return string.Equals(status, "steady", StringComparison.OrdinalIgnoreCase)
                ? "No conditions"
                : status;
        }

        private string CombatHudActiveHeader(CombatUnit unit)
        {
            if (unit == null) return "";
            if (unit.Side == UnitSide.Party)
            {
                return $"L{Mathf.Max(1, unit.Level)} {DisplayRace(unit.Race)} {DisplayClass(unit.ClassKey)}";
            }
            string rank = string.IsNullOrWhiteSpace(unit.Rank) ? "Enemy" : unit.Rank;
            string role = string.IsNullOrWhiteSpace(unit.Role) ? "combatant" : unit.Role;
            return $"{rank} {role}";
        }

        private string CombatHudUnitStateLine(CombatUnit unit, bool activeCard)
        {
            if (unit == null) return "";
            CombatUnit active = CurrentUnit();
            if (!activeCard && active != null)
            {
                if (active.Side == UnitSide.Party && selectedAction == ActionMode.Cast)
                {
                    FormulaDef formula = GetFormula(pendingFormulaCode);
                    if (formula != null)
                    {
                        bool legal = FormulaTargetCurrentlyLegal(formula, active, unit, unit.X, unit.Y);
                        return legal
                            ? $"{formula.Name} / valid spell target"
                            : $"{formula.Name} / not a valid spell target";
                    }
                }
                if (active.Side == UnitSide.Party && selectedAction == ActionMode.Ability)
                {
                    MartialAbility ability = AbilityDef(pendingAbilityId);
                    if (ability != null)
                    {
                        bool legal = CanTargetAbility(active, ability, unit, unit.X, unit.Y, out string reason);
                        return legal
                            ? $"{ability.Name} / valid skill target"
                            : $"{ability.Name} / {reason}";
                    }
                }
                if (active.Id == unit.Id)
                {
                    string activeSide = unit.Side == UnitSide.Party ? "Party" : "Enemy";
                    return $"{activeSide} / active combatant";
                }
                if (active.Side != unit.Side)
                {
                    CombatAttackForecast forecast = AttackForecast(active, unit);
                    if (forecast.Legal)
                    {
                        string label = active.Side == UnitSide.Enemy
                            ? CombatThreatRules.SeverityLabel(forecast.ThreatLevel)
                            : "FORECAST";
                        string guardState = forecast.Guarded ? " / guarded" : "";
                        return $"{label} {forecast.HitChance}% / {forecast.MinDamage}-{forecast.MaxDamage} {forecast.DamageType}{guardState}";
                    }
                    if (active.Side == UnitSide.Enemy && CanEnemySpecialReach(active, unit)) return "POWER IN RANGE / special attack";
                    return $"{CombatThreatRules.BlockLabel(forecast.BlockReason)} / range {forecast.Range}";
                }
                return active.Side == UnitSide.Enemy ? "Enemy ally / support focus" : "Party ally";
            }
            string side = unit.Side == UnitSide.Party ? "Party" : "Enemy";
            string range = unit.Range > 1 ? $"range {unit.Range}" : "melee";
            string guard = unit.GuardBonus > 0 ? $" / guard {unit.GuardBonus}" : "";
            bool focused = active != null
                && active.Id == unit.Id
                && !string.IsNullOrEmpty(unit.Spell)
                && IsFocusedCaster(unit);
            string focus = focused ? " / FOCUS -1 MP +1R" : "";
            return $"{side} / {range}{guard}{focus}";
        }

        private string CombatHudTargetHeader(CombatUnit unit)
        {
            CombatUnit active = CurrentUnit();
            if (unit == null) return "";
            if (active != null && active.Id != unit.Id) return $"Distance {Distance(active.X, active.Y, unit.X, unit.Y)}";
            return unit.Side == UnitSide.Party ? "Party unit" : "Enemy unit";
        }

        private string CombatHudTargetTitle(CombatUnit active, CombatUnit target)
        {
            CombatUnit hovered = CombatHudHoveredUnit();
            bool inspected = hovered != null && target != null && hovered.Id == target.Id;
            return CombatHudTargetContextTitle(active, target, inspected);
        }

        private string CombatHudTargetContextTitle(CombatUnit active, CombatUnit target, bool inspected)
        {
            if (target == null) return "Inspect";
            if (active != null && active.Side == UnitSide.Enemy) return "Intent Target";
            if (selectedAction == ActionMode.Cast && GetFormula(pendingFormulaCode) != null)
            {
                return ArmedPowerTargetCurrentlyLegal(active, target) ? "Spell Target" : "Blocked Spell Target";
            }
            if (selectedAction == ActionMode.Ability && AbilityDef(pendingAbilityId) != null)
            {
                return ArmedPowerTargetCurrentlyLegal(active, target) ? "Skill Target" : "Blocked Skill Target";
            }
            if (inspected
                && selectedAction == ActionMode.Attack
                && active != null
                && active.Side == UnitSide.Party
                && active.Side != target.Side)
            {
                CombatAttackForecast forecast = AttackForecast(active, target);
                return forecast.Legal ? AttackModeLabel(active) + " Target" : "Blocked Target";
            }
            if (inspected) return "Inspect";
            return target.Side == UnitSide.Enemy ? "Nearest Enemy" : "Nearest Ally";
        }

        private CombatUnit CombatHudTarget(CombatUnit active)
        {
            CombatUnit hovered = CombatHudHoveredUnit();
            bool powerArmed = !string.IsNullOrEmpty(pendingFormulaCode) || !string.IsNullOrEmpty(pendingAbilityId);
            if (hovered != null
                && (active == null || hovered.Id != active.Id || powerArmed))
            {
                return hovered;
            }
            if (state?.Combat?.Units == null) return null;
            if (active != null && active.Side == UnitSide.Enemy) return EnemyIntentFocus(active);

            if (powerArmed) return SuggestedArmedPowerTarget(active);

            UnitSide wanted = active == null || active.Side == UnitSide.Party ? UnitSide.Enemy : UnitSide.Party;
            return state.Combat.Units
                .Where(u => u.Side == wanted && u.Hp > 0)
                .OrderBy(u => active == null ? 0 : Distance(active.X, active.Y, u.X, u.Y))
                .FirstOrDefault();
        }

        private CombatUnit CombatHudHoveredUnit()
        {
            if (state?.Combat == null || boardRect.width <= 0f || boardRect.height <= 0f) return null;
            Rect grid = CombatBoardInnerRect(boardRect);
            float cell = Mathf.Min(grid.width / CombatW, grid.height / CombatH);
            grid.width = cell * CombatW;
            grid.height = cell * CombatH;
            Vector2 mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            if (!grid.Contains(mouse)) return null;
            int x = Mathf.FloorToInt((mouse.x - grid.x) / cell);
            int y = Mathf.FloorToInt((mouse.y - grid.y) / cell);
            return UnitAt(x, y);
        }

        private IReadOnlyList<CombatHudLogView> BuildCombatHudLogViews()
        {
            if (state?.Log == null) return Array.Empty<CombatHudLogView>();
            combatHudLogBuffer.Clear();
            int count = Mathf.Min(combatTimelineExpanded ? 5 : 1, state.Log.Count);
            for (int i = 0; i < count; i++)
            {
                LogEntry entry = state.Log[i];
                combatHudLogBuffer.Add(new CombatHudLogView
                {
                    Text = entry.Text,
                    Tone = entry.Tone.ToString()
                });
            }
            return combatHudLogBuffer;
        }

        private void RunCombatHudCommand(ActionMode mode)
        {
            CombatUnit active = CurrentUnit();
            bool playerTurn = active != null && active.Side == UnitSide.Party;
            if (!CanAcceptGameplayInput() || IsBoardPointerSuppressed() || !playerTurn || !CombatCommandEnabled(mode, active))
            {
                string reason = !CanAcceptGameplayInput() || IsBoardPointerSuppressed()
                    ? "Close the active panel before issuing a combat command."
                    : DisabledActionReason(mode, active, playerTurn);
                PushLog(reason, Tone.Warn);
                PlaySfx("blocked", 0.55f);
                return;
            }
            SelectOrRunAction(mode, active);
        }

        private void RunCombatHudUndoMove()
        {
            if (!CanAcceptGameplayInput() || IsBoardPointerSuppressed())
            {
                PushLog("Close the active panel before undoing movement.", Tone.Warn);
                PlaySfx("blocked", 0.55f);
                return;
            }
            UndoActiveMovement();
        }

        private void RunCombatHudCancelTarget()
        {
            if (!CanAcceptGameplayInput() || IsBoardPointerSuppressed())
            {
                PushLog("Close the active panel before canceling targeting.", Tone.Warn);
                PlaySfx("blocked", 0.55f);
                return;
            }
            if (!CancelCombatTargeting())
            {
                PushLog("No spell or skill target is currently armed.", Tone.Warn);
                PlaySfx("blocked", 0.55f);
            }
        }

        private void ToggleCombatTimeline()
        {
            combatTimelineExpanded = !combatTimelineExpanded;
            PlaySfx("ui", 0.45f);
        }

        private bool NeedsEmergencyCombatHudFallback()
        {
            return state != null
                && state.Mode == GameMode.Combat
                && !ShouldShowStartupSplash()
                && (combatHudScreen == null || !combatHudScreen.HasUsableCommandBar);
        }

        private void DrawEmergencyCombatHudFallback()
        {
            if (!NeedsEmergencyCombatHudFallback()) return;
            bool previousGuiEnabled = GUI.enabled;
            GUI.enabled = previousGuiEnabled && CanAcceptGameplayInput() && !IsBoardPointerSuppressed();
            CombatUnit active = CurrentUnit();
            bool playerTurn = active != null && active.Side == UnitSide.Party;
            CombatHudGeometry geometry = CombatHudScreenLayout.Calculate(Screen.width, Screen.height);
            CombatHudView view = BuildCombatHudView();
            DrawCombatFallbackChrome(geometry.Top, view);
            DrawCombatFallbackSide(geometry.Side, view);
            Rect bar = geometry.Command;
            DrawRect(bar, Hex("080b0d", 0.98f));
            DrawBorder(bar, gold.WithAlpha(0.86f), 1);

            IReadOnlyList<ActionMode> modes = CombatHudFallbackModes(active);
            bool promoteEndTurn = playerTurn && ShouldPromoteEndTurn(active);
            Rect[] localButtons = CombatHudScreenLayout.CommandButtons(bar.width, promoteEndTurn);
            string prompt = view.CommandPrompt ?? "Combat HUD recovering...";
            for (int i = 0; i < localButtons.Length && i < view.Commands.Count; i++)
            {
                Rect hover = OffsetLocalRect(localButtons[i], bar);
                if (!hover.Contains(Event.current.mousePosition)) continue;
                CombatHudCommandView hovered = view.Commands[i];
                string detail = hovered.Enabled ? hovered.Tooltip : hovered.DisabledReason;
                prompt = $"{hovered.Label} [{hovered.Hotkey}]  {detail}";
                break;
            }
            float promptRight = bar.xMax - 78f;
            if (view.CanUndoMove) promptRight -= 122f;
            if (view.CanCancelTarget) promptRight -= 122f;
            float promptWidth = Mathf.Max(120f, promptRight - bar.x - 12f);
            GUI.Label(new Rect(bar.x + 12f, bar.y + 4f, promptWidth, 20f), FitText(prompt, promptWidth, CenterLeftStyle(10, promoteEndTurn ? gold : teal)), CenterLeftStyle(10, promoteEndTurn ? gold : teal));
            float contextX = bar.xMax - 190f;
            if (view.CanCancelTarget)
            {
                Rect cancel = new Rect(contextX, bar.y + 3f, 112f, 21f);
                if (GUI.Button(cancel, "Cancel [Esc]", smallButtonStyle)) RunCombatHudCancelTarget();
                contextX -= 122f;
            }
            if (view.CanUndoMove)
            {
                Rect undo = new Rect(contextX, bar.y + 3f, 112f, 21f);
                if (GUI.Button(undo, "Undo Move [U]", smallButtonStyle)) RunCombatHudUndoMove();
            }
            Rect menu = new Rect(bar.xMax - 68f, bar.y + 3f, 58f, 21f);
            if (GUI.Button(menu, "Menu", smallButtonStyle)) OpenPauseMenu();

            if (localButtons.Length >= 4)
            {
                float dividerX = bar.x + (localButtons[2].xMax + localButtons[3].xMin) * 0.5f;
                DrawRect(new Rect(dividerX - 1f, bar.y + 33f, 2f, 56f), gold.WithAlpha(0.42f));
            }

            for (int i = 0; i < modes.Count; i++)
            {
                ActionMode mode = modes[i];
                Rect button = OffsetLocalRect(localButtons[i], bar);
                CombatHudCommandView command = i < view.Commands.Count ? view.Commands[i] : null;
                bool enabled = command?.Enabled ?? (playerTurn && CombatCommandEnabled(mode, active));
                bool selected = selectedAction == mode;
                bool promoted = mode == ActionMode.Wait && promoteEndTurn;
                DrawRect(button, promoted ? Hex("352316", 0.98f) : selected ? Hex("243033", 0.98f) : Hex("151b20", 0.98f));
                DrawBorder(button, promoted ? gold : selected ? teal : line, promoted || selected ? 2 : 1);
                float iconSize = Mathf.Clamp(button.height - 20f, 40f, 48f);
                Rect icon = new Rect(button.x + 9f, button.y + 10f, iconSize, iconSize);
                DrawActionButtonGlyph(icon, mode, enabled, selected);
                Rect keycap = new Rect(button.x + 4f, button.y + 3f, 38f, 18f);
                DrawRect(keycap, promoted ? gold : selected ? teal : Hex("263035"));
                GUI.Label(keycap, command?.Hotkey ?? "", CenterStyle(8, promoted || selected ? retroBlack : ink));
                float textX = icon.xMax + 9f;
                float textW = Mathf.Max(36f, button.xMax - textX - 8f);
                string label = command?.Label ?? ActionName(mode, active);
                if (promoted) label = label.ToUpperInvariant();
                string subLabel = promoted ? "Next combatant" : enabled ? command?.SubLabel ?? ActionButtonSubLabel(mode, active) : command?.DisabledReason ?? DisabledActionReason(mode, active, playerTurn);
                GUI.Label(new Rect(textX, button.y + 9f, textW, 23f), FitText(label, textW, CenterLeftStyle(12, enabled ? ink : muted)), CenterLeftStyle(12, enabled ? ink : muted));
                GUI.Label(new Rect(textX, button.y + 34f, textW, 23f), FitText(subLabel, textW, CenterLeftStyle(9, enabled ? promoted ? gold : muted : muted)), CenterLeftStyle(9, enabled ? promoted ? gold : muted : muted));
                if (promoted || selected) DrawRect(new Rect(button.x, button.yMax - 4f, button.width, 4f), promoted ? gold : teal);
                if (enabled && GUI.Button(button, GUIContent.none, GUIStyle.none)) RunCombatHudCommand(mode);
            }
            GUI.enabled = previousGuiEnabled;
        }

        private IReadOnlyList<ActionMode> CombatHudFallbackModes(CombatUnit active)
        {
            return new[]
            {
                ActionMode.Move,
                ActionMode.Attack,
                PreferredThirdAction(active),
                ActionMode.Guard,
                ActionMode.Elixir,
                ActionMode.Wait
            };
        }

        private void DrawCombatFallbackChrome(Rect rect, CombatHudView view)
        {
            DrawRect(rect, Hex("080d10", 0.98f));
            DrawBorder(rect, Hex("3c4544", 0.72f), 1);
            GUI.Label(new Rect(rect.x + 14f, rect.y + 7f, rect.width * 0.24f, 22f), view.Title ?? GameTitle, CenterLeftStyle(16, ink));
            GUI.Label(new Rect(rect.x + rect.width * 0.25f, rect.y + 8f, rect.width * 0.38f, 18f), FitText(view.RouteLine, rect.width * 0.38f, CenterStyle(10, muted)), CenterStyle(10, muted));
            Color phaseColor = view.ActiveUnit == null ? muted : view.PlayerTurn ? teal : ember;
            GUI.Label(new Rect(rect.x + rect.width * 0.25f, rect.y + 29f, rect.width * 0.38f, 17f), FitText(view.PhaseLine, rect.width * 0.38f, CenterStyle(10, phaseColor)), CenterStyle(10, phaseColor));
            GUI.Label(new Rect(rect.x + rect.width * 0.64f, rect.y + 9f, rect.width * 0.10f, 18f), view.RoundLine ?? "", CenterRightStyle(10, gold));
            GUI.Label(new Rect(rect.xMax - 246f, rect.y + 9f, 72f, 18f), "Gold " + (view.Gold ?? "0"), CenterLeftStyle(9, gold));
            GUI.Label(new Rect(rect.xMax - 166f, rect.y + 9f, 72f, 18f), "Supplies " + (view.Supplies ?? "0"), CenterLeftStyle(9, moss));
            GUI.Label(new Rect(rect.xMax - 86f, rect.y + 9f, 72f, 18f), "Elixirs " + (view.Elixirs ?? "0"), CenterLeftStyle(9, teal));
        }

        private void DrawCombatFallbackSide(Rect side, CombatHudView view)
        {
            CombatHudScreenLayout.SidePanels(side, view.TimelineExpanded, out Rect activeLocal, out Rect targetLocal, out Rect timelineLocal);
            Rect active = OffsetLocalRect(activeLocal, side);
            Rect target = OffsetLocalRect(targetLocal, side);
            Rect timeline = OffsetLocalRect(timelineLocal, side);
            DrawCombatFallbackUnitCard(active, "Active Unit", view.ActiveUnit, teal);
            DrawCombatFallbackUnitCard(target, string.IsNullOrWhiteSpace(view.TargetTitle) ? "Inspect" : view.TargetTitle, view.TargetUnit, blood);

            DrawRect(timeline, Hex("080b0d", 0.98f));
            DrawBorder(timeline, gold.WithAlpha(0.72f), 1);
            Rect timelineHeader = new Rect(timeline.x + 10f, timeline.y + 7f, timeline.width - 20f, 24f);
            GUI.Label(timelineHeader, view.TimelineExpanded ? "Timeline (click to collapse)" : "Timeline (click to expand)", CenterLeftStyle(12, gold));
            if (GUI.Button(timelineHeader, GUIContent.none, GUIStyle.none)) ToggleCombatTimeline();
            Rect queue = new Rect(timeline.x + 10f, timeline.y + 36f, timeline.width - 20f, view.TimelineExpanded ? 40f : 28f);
            string queueText = CombatHudFallbackTurnLine(view.Turns);
            if (view.TimelineExpanded && !string.IsNullOrWhiteSpace(view.TacticalLine))
            {
                queueText += "\nPLAN  " + view.TacticalLine;
            }
            GUI.Label(queue, queueText, WrapStyle(9, ink));
            IReadOnlyList<CombatHudLogView> logs = view.Logs ?? Array.Empty<CombatHudLogView>();
            float y = timeline.y + (view.TimelineExpanded ? 82f : 68f);
            for (int i = 0; i < logs.Count && y < timeline.yMax - 22f; i++)
            {
                Rect row = new Rect(timeline.x + 10f, y, timeline.width - 20f, Mathf.Min(44f, timeline.yMax - y - 8f));
                DrawRect(row, Hex("151b20", 0.90f));
                Color stripe = string.Equals(logs[i].Tone, Tone.Warn.ToString(), StringComparison.OrdinalIgnoreCase) ? ember : string.Equals(logs[i].Tone, Tone.Good.ToString(), StringComparison.OrdinalIgnoreCase) ? teal : moss;
                DrawRect(new Rect(row.x, row.y, 4f, row.height), stripe);
                GUI.Label(new Rect(row.x + 10f, row.y + 5f, row.width - 16f, row.height - 10f), logs[i].Text ?? "", WrapStyle(9, ink));
                y += row.height + 5f;
            }
        }

        private string CombatHudFallbackTurnLine(IReadOnlyList<CombatHudTurnView> turns)
        {
            if (turns == null || turns.Count == 0) return "Initiative is forming...";
            return string.Join("  /  ", turns
                .Where(turn => turn != null)
                .Select(turn => (turn.StartsNextRound ? "Next round: " : turn.Active ? "> " : "") + turn.Name)
                .ToArray());
        }

        private void DrawCombatFallbackUnitCard(Rect rect, string title, CombatHudUnitView unit, Color fallbackAccent)
        {
            Color accent = unit == null || string.IsNullOrWhiteSpace(unit.AccentHex) ? fallbackAccent : Hex(unit.AccentHex);
            DrawRect(rect, Hex("080b0d", 0.98f));
            DrawBorder(rect, accent.WithAlpha(0.76f), 1);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 8f, rect.width - 24f, 18f), title, CenterLeftStyle(11, accent));
            if (unit == null)
            {
                GUI.Label(new Rect(rect.x + 12f, rect.y + 38f, rect.width - 24f, 20f), "No target selected", CenterLeftStyle(10, muted));
                return;
            }

            GUI.Label(new Rect(rect.x + 12f, rect.y + 31f, rect.width - 24f, 22f), FitText(unit.Name, rect.width - 24f, CenterLeftStyle(15, ink)), CenterLeftStyle(15, ink));
            GUI.Label(new Rect(rect.x + 12f, rect.y + 56f, rect.width - 24f, 18f), FitText(unit.Header, rect.width - 24f, CenterLeftStyle(10, gold)), CenterLeftStyle(10, gold));
            GUI.Label(new Rect(rect.x + 12f, rect.y + 77f, rect.width - 24f, 18f), FitText(unit.StateLine, rect.width - 24f, CenterLeftStyle(9, muted)), CenterLeftStyle(9, muted));
            GUI.Label(new Rect(rect.x + 12f, rect.y + 97f, rect.width - 24f, 34f), unit.StatusLine ?? "", WrapStyle(9, ink));
            DrawCombatFallbackMeter(new Rect(rect.x + 12f, rect.yMax - 38f, rect.width - 24f, 10f), unit.Hp, unit.MaxHp, blood);
            if (unit.MaxMana > 0) DrawCombatFallbackMeter(new Rect(rect.x + 12f, rect.yMax - 22f, rect.width - 24f, 8f), unit.Mana, unit.MaxMana, teal);
        }

        private void DrawCombatFallbackMeter(Rect rect, int value, int max, Color color)
        {
            DrawRect(rect, Hex("030405", 0.90f));
            float fill = max <= 0 ? 0f : Mathf.Clamp01((float)value / max);
            DrawRect(new Rect(rect.x, rect.y, rect.width * fill, rect.height), color);
            DrawBorder(rect, line.WithAlpha(0.66f), 1);
        }

        private Rect OffsetLocalRect(Rect local, Rect parent)
        {
            return new Rect(parent.x + local.x, parent.y + local.y, local.width, local.height);
        }

        private void DrawCombatDebugOverlay()
        {
            if (!betaLabMode || state?.Mode != GameMode.Combat) return;
            CombatUnit active = CurrentUnit();
            bool playerTurn = active != null && active.Side == UnitSide.Party;
            Rect baseRect = CombatHudScreenLayout.Calculate(Screen.width, Screen.height).Command;
            if (baseRect.width < 480f) return;
            // Keep tester controls on their own row above the canonical command
            // prompt. Sharing that row caused Audio/status/Menu overlap at 1280x720.
            DrawBetaLabToolbar(new Rect(baseRect.x, baseRect.y - 66f, Mathf.Min(900f, baseRect.width), 30f), active, playerTurn);
        }
    }
}
