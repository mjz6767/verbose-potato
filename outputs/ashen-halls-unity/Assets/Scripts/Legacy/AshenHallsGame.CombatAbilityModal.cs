using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private void EnsureCombatAbilityModalScreen()
        {
            if (combatAbilityModalScreen != null && combatAbilityModalScreen.IsReady) return;
            if (combatAbilityModalScreen != null)
            {
                Destroy(combatAbilityModalScreen.gameObject);
                combatAbilityModalScreen = null;
            }
            GameObject screen = new GameObject("Combat Ability Modal Screen");
            screen.transform.SetParent(transform, false);
            CombatAbilityModalScreen created = screen.AddComponent<CombatAbilityModalScreen>();
            try
            {
                created.Bind(new CombatAbilityModalBindings
                {
                    View = BuildCombatAbilityModalView,
                    Close = CloseCombatAbilityModal,
                    PreviewCard = PreviewCombatAbilityModalCard,
                    SelectCard = SelectCombatAbilityModalCard,
                    ActivateCard = ActivateCombatAbilityModalCard
                });
                created.SetVisible(false);
                combatAbilityModalScreen = created;
            }
            catch
            {
                created.SetVisible(false);
                screen.SetActive(false);
                Destroy(screen);
                throw;
            }
        }

        private void SyncCombatAbilityModalScreen()
        {
            bool visible = state != null
                && state.Mode == GameMode.Combat
                && !ShouldShowStartupSplash()
                && CurrentUiOverlay() == UiOverlay.AbilityPicker;
            if (visible && (combatAbilityModalScreen == null || !combatAbilityModalScreen.IsReady))
            {
                TryInitializePresentationScreen("Combat ability modal recovery", EnsureCombatAbilityModalScreen, false);
            }
            if (combatAbilityModalScreen == null) return;
            if (!visible)
            {
                combatAbilityModalScreen.SetVisible(false);
                return;
            }

            bool refresh = !combatAbilityModalScreen.CanOwnModal
                || ShouldRefreshPresentation(ref lastCombatAbilityModalRefreshKey, CombatAbilityModalRefreshKey());
            if (refresh)
            {
                try
                {
                    combatAbilityModalScreen.Refresh();
                    combatAbilityModalScreen.SetVisible(true);
                    Canvas.ForceUpdateCanvases();
                }
                catch (Exception ex)
                {
                    combatAbilityModalScreen.SetVisible(false);
                    Debug.LogException(new InvalidOperationException(VersionInfo.ProductName + " combat ability modal refresh failed; using recovery popup.", ex));
                }
            }

            if (!combatAbilityModalScreen.CanOwnModal) combatAbilityModalScreen.SetVisible(false);
        }

        private string CombatAbilityModalRefreshKey()
        {
            CombatUnit active = CurrentUnit();
            int hash = showSpellbook ? 31 : 47;
            hash = unchecked(hash * 31 + (showAbilityPanel ? 1 : 0));
            hash = unchecked(hash * 31 + (spellbookSelectedCode ?? "").GetHashCode());
            hash = unchecked(hash * 31 + (abilitySelectedId ?? "").GetHashCode());
            hash = unchecked(hash * 31 + (pendingFormulaCode ?? "").GetHashCode());
            hash = unchecked(hash * 31 + (pendingAbilityId ?? "").GetHashCode());
            hash = unchecked(hash * 31 + (active?.Id ?? "").GetHashCode());
            if (active != null)
            {
                hash = unchecked(hash * 31 + active.Mana);
                hash = unchecked(hash * 31 + active.MaxMana);
                hash = unchecked(hash * 31 + active.Hp);
                hash = unchecked(hash * 31 + active.Level);
                hash = unchecked(hash * 31 + active.X);
                hash = unchecked(hash * 31 + active.Y);
                hash = unchecked(hash * 31 + active.Stunned);
                hash = unchecked(hash * 31 + active.Sleeping);
                hash = unchecked(hash * 31 + active.Webbed);
                hash = unchecked(hash * 31 + active.DemonFormTurns);
                hash = unchecked(hash * 31 + (active.ClassKey ?? "").GetHashCode());
                hash = unchecked(hash * 31 + (active.Spell ?? "").GetHashCode());
            }
            if (state?.Combat != null)
            {
                hash = unchecked(hash * 31 + (state.Combat.ActionAvailable ? 1 : 0));
                hash = unchecked(hash * 31 + state.Combat.MovePoints);
                hash = unchecked(hash * 31 + (state.Combat.Moved ? 1 : 0));
                if (state.Combat.Units != null)
                {
                    foreach (CombatUnit unit in state.Combat.Units)
                    {
                        if (unit == null) continue;
                        hash = unchecked(hash * 31 + (unit.Id ?? "").GetHashCode());
                        hash = unchecked(hash * 31 + unit.X);
                        hash = unchecked(hash * 31 + unit.Y);
                        hash = unchecked(hash * 31 + unit.Hp);
                        hash = unchecked(hash * 31 + unit.Bleeding);
                        hash = unchecked(hash * 31 + unit.Hexed);
                    }
                }
                if (state.Combat.Obstacles != null)
                {
                    foreach (Point obstacle in state.Combat.Obstacles)
                    {
                        if (obstacle == null) continue;
                        hash = unchecked(hash * 31 + obstacle.X);
                        hash = unchecked(hash * 31 + obstacle.Y);
                        hash = unchecked(hash * 31 + obstacle.Duration);
                        hash = unchecked(hash * 31 + obstacle.Integrity);
                        hash = unchecked(hash * 31 + (obstacle.Kind ?? "").GetHashCode());
                    }
                }
            }
            return "abilityModal=" + hash;
        }

        private CombatAbilityModalView BuildCombatAbilityModalView()
        {
            CombatUnit active = CurrentUnit();
            bool playerTurn = active != null && active.Side == UnitSide.Party;
            bool spellbook = showSpellbook;
            IReadOnlyList<CombatAbilityModalCardView> cards = spellbook
                ? BuildFormulaModalCards(active, playerTurn)
                : BuildSkillModalCards(active, playerTurn);
            string selection = ResolveCombatAbilityBrowseSelection(active, spellbook, cards);
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].Selected = string.Equals(cards[i].Id, selection, StringComparison.Ordinal);
            }
            if (spellbook) spellbookSelectedCode = selection;
            else abilitySelectedId = selection;

            bool demonArts = !spellbook && MartialClassKey(active) == "demon";
            string discipline = spellbook
                ? BookTitleCase(SpellCraftLabel(active?.Spell))
                : demonArts ? "Demon Form" : DisplayClass(active?.ClassKey);
            string actor = active == null
                ? "No active combatant"
                : $"{active.Name}  •  L{active.Level}";
            string resource = spellbook && active != null
                ? $"MP  {active.Mana} / {active.MaxMana}"
                : demonArts && active != null
                    ? $"FORM  {DisplayedDemonFormTurns(active)}  â€¢  MOVE  {state?.Combat?.MovePoints ?? 0}"
                    : state?.Combat == null ? "" : $"MOVE  {state.Combat.MovePoints}";
            string actionState = CombatAbilityBookActionState(active, playerTurn);
            string trait = spellbook
                ? SpellbookTraitLine(active)
                : SkillbookTraitLine(active);

            return new CombatAbilityModalView
            {
                Visible = state != null && state.Mode == GameMode.Combat && CurrentUiOverlay() == UiOverlay.AbilityPicker,
                Spellbook = spellbook,
                Title = spellbook ? $"{discipline} Spellbook" : demonArts ? "Demon Arts" : $"{discipline} Skillbook",
                Header = spellbook ? SpellbookModalHeader(active, playerTurn) : AbilityHeaderLine(active),
                Actor = actor,
                Resource = resource,
                ActionState = actionState,
                Trait = trait,
                StateIconTexture = powerBookStateIconAtlas,
                ContextKey = (spellbook ? "spellbook|" : "skills|") + (active?.Id ?? ""),
                EmptyText = spellbook ? "No usable spell formulas are available on this turn." : "No combat skills are available on this turn.",
                SelectedId = selection,
                Cards = cards
            };
        }

        private IReadOnlyList<CombatAbilityModalCardView> BuildFormulaModalCards(CombatUnit active, bool playerTurn)
        {
            if (!playerTurn || active == null || string.IsNullOrEmpty(active.Spell)) return Array.Empty<CombatAbilityModalCardView>();
            List<CombatAbilityModalCardView> cards = new List<CombatAbilityModalCardView>();
            IEnumerable<FormulaDef> formulas = ActiveFormulaBook()
                .Where(formula => SchoolMatches(formula, active.Spell))
                .OrderBy(FormulaRequiredLevel)
                .ThenBy(formula => string.Equals(active.ClassKey, "warlock", StringComparison.OrdinalIgnoreCase) ? PactSpellbookSort(formula) : FormulaBookIndex(formula));
            foreach (FormulaDef formula in formulas)
            {
                CombatActionCard card = FormulaActionCard(formula, active);
                if (card == null) continue;
                CombatAbilityModalCardView view = ToModalCard(card, spellbookSelectedCode == formula.Code);
                view.UnlockLevel = FormulaRequiredLevel(formula);
                view.Locked = active.Level < view.UnlockLevel;
                view.Epic = FormulaTier(formula) >= 4;
                view.Focused = IsFocusedCaster(active);
                view.Tier = FormulaTierLabel(formula);
                view.RowSummary = FormulaModalRowSummary(formula, active);
                view.CurrentEffect = FormulaModalCurrentEffect(formula, active);
                int mana = EffectiveFormulaMana(formula, active);
                view.ResourceAfter = active.Mana >= mana
                    ? $"{Mathf.Max(0, active.Mana - mana)} MP after"
                    : $"{active.Mana}/{mana} MP";
                view.ValidTargetCount = view.Locked ? 0 : CountLegalFormulaTargets(formula, active);
                view.TargetCountKnown = !view.Locked;
                if (!view.Locked && view.Targeted && view.ValidTargetCount <= 0)
                {
                    view.TacticalNote = FormulaNoTargetReason(formula, active);
                }
                ApplyFormulaModalArt(view, formula);
                cards.Add(view);
            }
            return cards;
        }

        private IReadOnlyList<CombatAbilityModalCardView> BuildSkillModalCards(CombatUnit active, bool playerTurn)
        {
            if (!playerTurn || active == null || !HasMartialAbilities(active)) return Array.Empty<CombatAbilityModalCardView>();
            List<CombatAbilityModalCardView> cards = new List<CombatAbilityModalCardView>();
            foreach (MartialAbility ability in MartialAbilitiesFor(active))
            {
                CombatActionCard card = AbilityActionCard(ability, active);
                if (card == null) continue;
                CombatAbilityModalCardView view = ToModalCard(card, abilitySelectedId == ability.Id);
                view.UnlockLevel = ability.RequiredLevel;
                view.Locked = active.Level < ability.RequiredLevel;
                view.Epic = CombatPowerPresentationRules.AbilityIntensity(ability.Id) >= 3;
                view.Tier = ability.RequiredLevel <= 1 ? "starter" : ability.RequiredLevel <= 3 ? "trained" : "master";
                view.RowSummary = string.IsNullOrWhiteSpace(ability.Summary) ? card.Summary : ability.Summary;
                view.CurrentEffect = AbilityCurrentEffectLine(ability, active);
                view.ResourceAfter = "Action spent";
                view.ValidTargetCount = view.Locked ? 0 : CountLegalAbilityTargets(ability, active);
                view.TargetCountKnown = !view.Locked;
                if (!view.Locked && view.Targeted && view.ValidTargetCount <= 0)
                {
                    view.TacticalNote = AbilityNoTargetReason(ability, active);
                }
                ApplyAbilityModalArt(view, ability);
                cards.Add(view);
            }
            return cards;
        }

        private static CombatAbilityModalCardView ToModalCard(CombatActionCard card, bool selected)
        {
            return new CombatAbilityModalCardView
            {
                Id = card.Id,
                Name = card.Name,
                Kind = card.Kind,
                Cost = card.Cost,
                Range = card.Range,
                Target = card.Target,
                Path = card.Path,
                Impact = card.Impact,
                Summary = card.Summary,
                RowSummary = card.Summary,
                CurrentEffect = string.IsNullOrWhiteSpace(card.CurrentEffect) ? card.Summary : card.CurrentEffect,
                Detail = card.Detail,
                DisabledReason = card.DisabledReason,
                Targeted = card.Targeted,
                Ready = card.Ready,
                Usable = card.Usable,
                Selected = selected
            };
        }

        private string FormulaModalRowSummary(FormulaDef formula, CombatUnit active)
        {
            if (formula == null) return "Spell formula";
            switch ((formula.Code ?? "").ToUpperInvariant())
            {
                case "RSG": return "Adjacent shock  •  push 1";
                case "CLT": return "Up to 4 jumping shock hits";
                case "VST": return "Teleport  •  landing shock";
                case "AST": return "Radius 2 storm  •  may stun";
            }

            string effect = (formula.Effect ?? "").ToLowerInvariant();
            string type = BookTitleCase(string.IsNullOrWhiteSpace(formula.DamageType) ? "magic" : formula.DamageType);
            switch (effect)
            {
                case "damage":
                    return type
                        + (formula.Splash ? " area damage" : " damage")
                        + (string.IsNullOrWhiteSpace(formula.Status) ? "" : "  •  " + StatusLabel(formula.Status));
                case "drain":
                    return type + " damage  •  restores HP";
                case "terrain":
                    int duration = FieldDurationRounds(formula.Terrain, formula.Duration);
                    return BookTitleCase(formula.Terrain) + " field"
                        + (duration <= 0 ? "" : "  •  " + duration + " rounds");
                case "heal":
                    Vector2Int healing = FormulaHealPreview(formula, active);
                    return $"Restore {healing.x}–{healing.y} HP";
                case "cure":
                    return "Cleanse harmful conditions";
                case "status":
                    return StatusLabel(formula.Status) + "  •  " + Mathf.Max(1, formula.Duration) + " turns";
                case "dispel":
                    return "Remove hostile fields and rituals";
                case "summon":
                    return SummonDisplayName(formula.SummonRole) + "  •  " + Mathf.Max(1, formula.Duration) + " turns";
                case "teleport":
                    return "Teleport to an open tile";
                case "transform":
                    return "Battle transformation";
                default:
                    return string.IsNullOrWhiteSpace(formula.Hint) ? "Spell formula" : formula.Hint.Trim().TrimEnd('.');
            }
        }

        private string FormulaModalCurrentEffect(FormulaDef formula, CombatUnit active)
        {
            if (formula == null) return "Choose a spell.";
            Vector2Int baseDamage = FormulaDamagePreview(formula, active, null);
            switch ((formula.Code ?? "").ToUpperInvariant())
            {
                case "RSG":
                    return $"Deals {LightningPowerRules.ThunderclapDamage(baseDamage.x)}–{LightningPowerRules.ThunderclapDamage(baseDamage.y)} shock to every adjacent enemy and pushes each 1 tile. A blocked push collides and stuns.";
                case "CLT":
                    return $"Deals {baseDamage.x}–{baseDamage.y} shock to the first target, then 75%, 55%, and 40%. Hits up to 4 enemies within 2 tiles, or 3 tiles through conductive terrain.";
                case "VST":
                    return $"Teleport to an open tile within range {EffectiveFormulaRange(formula, active)}. Arrival deals {LightningPowerRules.ThunderStepDamage(baseDamage.x)}–{LightningPowerRules.ThunderStepDamage(baseDamage.y)} shock to adjacent enemies.";
                case "AST":
                    return $"Deals {baseDamage.x}–{baseDamage.y} shock at the center and {LightningPowerRules.TempestDamage(baseDamage.x, false)}–{LightningPowerRules.TempestDamage(baseDamage.y, false)} within radius 2. Each enemy has a 35% stun chance before resistance.";
            }

            string effect = (formula.Effect ?? "").ToLowerInvariant();
            if (effect == "damage" || effect == "drain")
            {
                string type = string.IsNullOrWhiteSpace(formula.DamageType) ? "magic" : formula.DamageType;
                string area = formula.Splash ? " in an area" : "";
                string status = string.IsNullOrWhiteSpace(formula.Status)
                    ? ""
                    : $"; may apply {StatusLabel(formula.Status)} for {Mathf.Max(1, formula.Duration)} turn{(Mathf.Max(1, formula.Duration) == 1 ? "" : "s")}";
                string drain = effect == "drain" ? "; restores about half as HP" : "";
                return $"Deals {baseDamage.x}–{baseDamage.y} {type} damage{area}{status}{drain}.";
            }

            return FormulaEffectSummary(formula, active);
        }

        private string ResolveCombatAbilityBrowseSelection(
            CombatUnit active,
            bool spellbook,
            IReadOnlyList<CombatAbilityModalCardView> cards)
        {
            if (active == null || cards == null || cards.Count == 0) return "";
            string key = CombatAbilityBrowseSelectionKey(active, spellbook);
            string candidate = "";
            if (combatAbilityBrowseSelections != null)
            {
                combatAbilityBrowseSelections.TryGetValue(key, out candidate);
            }
            if (!ContainsModalCard(cards, candidate))
            {
                candidate = spellbook ? spellbookSelectedCode : abilitySelectedId;
            }
            if (!ContainsModalCard(cards, candidate))
            {
                candidate = spellbook ? pendingFormulaCode : pendingAbilityId;
            }
            if (!ContainsModalCard(cards, candidate))
            {
                CombatAbilityModalCardView firstReady = cards.FirstOrDefault(CombatAbilityModalPresentationRules.IsReadyNow);
                CombatAbilityModalCardView firstLearned = cards.FirstOrDefault(card => card != null && !card.Locked);
                candidate = firstReady?.Id ?? firstLearned?.Id ?? cards[0]?.Id ?? "";
            }
            RememberCombatAbilityBrowseSelection(active, spellbook, candidate);
            return candidate ?? "";
        }

        private static bool ContainsModalCard(IReadOnlyList<CombatAbilityModalCardView> cards, string id)
        {
            if (cards == null || string.IsNullOrWhiteSpace(id)) return false;
            for (int i = 0; i < cards.Count; i++)
            {
                if (string.Equals(cards[i]?.Id, id, StringComparison.Ordinal)) return true;
            }
            return false;
        }

        private string CombatAbilityBrowseSelectionKey(CombatUnit active, bool spellbook)
        {
            return (spellbook ? "spell|" : "skill|") + (active?.Id ?? "");
        }

        private void RememberCombatAbilityBrowseSelection(CombatUnit active, bool spellbook, string id)
        {
            if (active == null || string.IsNullOrWhiteSpace(id) || combatAbilityBrowseSelections == null) return;
            combatAbilityBrowseSelections[CombatAbilityBrowseSelectionKey(active, spellbook)] = id;
        }

        private static string BookTitleCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "Battle";
            string[] words = value.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (string.Equals(words[i], "or", StringComparison.OrdinalIgnoreCase)) words[i] = "or";
                else words[i] = char.ToUpperInvariant(words[i][0]) + words[i].Substring(1).ToLowerInvariant();
            }
            return string.Join(" ", words);
        }

        private string SpellbookTraitLine(CombatUnit active)
        {
            if (active == null) return "";
            if (IsFocusedCaster(active)) return "FOCUS  •  -1 MP  •  +1 range until movement";
            return "";
        }

        private string SkillbookTraitLine(CombatUnit active)
        {
            if (active == null) return "";
            string cls = MartialClassKey(active);
            if (cls == "warrior")
            {
                return WarriorEnrageBonus(active) > 0
                    ? $"ENRAGE ACTIVE  •  +{WarriorEnrageBonus(active)} physical damage"
                    : "";
            }
            if (cls == "rogue")
            {
                return active.Stealthed > 0
                    ? $"STEALTH {active.Stealthed}  •  opening attacks empowered"
                    : "";
            }
            if (cls == "demon")
            {
                return $"ABYSSAL FORM {DisplayedDemonFormTurns(active)}  â€¢  death attacks empowered  â€¢  incoming damage reduced";
            }
            return "";
        }

        private string CombatAbilityBookActionState(CombatUnit active, bool playerTurn)
        {
            if (!playerTurn || active == null) return "WAITING";
            if (IsCombatResolutionPending()) return "RESOLVING";
            if (active.Stunned > 0) return $"STUNNED {active.Stunned}";
            if (active.Sleeping > 0) return $"SLEEPING {active.Sleeping}";
            return state?.Combat?.ActionAvailable == true ? "ACTION READY" : "ACTION USED";
        }

        private int CountLegalFormulaTargets(FormulaDef formula, CombatUnit caster)
        {
            if (formula == null || caster == null || state?.Combat == null) return 0;
            if (string.Equals(formula.Target, "self", StringComparison.OrdinalIgnoreCase))
            {
                return FormulaTargetCurrentlyLegal(formula, caster, caster, caster.X, caster.Y) ? 1 : 0;
            }

            int count = 0;
            for (int y = 0; y < CombatH; y++)
            for (int x = 0; x < CombatW; x++)
            {
                CombatUnit target = UnitAt(x, y);
                if (FormulaTargetCurrentlyLegal(formula, caster, target, x, y)) count++;
            }
            return count;
        }

        private bool FormulaTargetCurrentlyLegal(FormulaDef formula, CombatUnit caster, CombatUnit target, int x, int y)
        {
            if (formula == null || caster == null) return false;
            if (Distance(caster.X, caster.Y, x, y) > EffectiveFormulaRange(formula, caster)) return false;
            if (!CanTargetFormula(formula, caster, target, x, y)) return false;
            return HasFormulaLineOfSight(formula, caster, x, y);
        }

        private int CountLegalAbilityTargets(MartialAbility ability, CombatUnit active)
        {
            if (ability == null || active == null || state?.Combat?.Units == null) return 0;
            if (!ability.Targeted) return 1;
            int count = 0;
            foreach (CombatUnit target in state.Combat.Units)
            {
                if (target == null || target.Side != UnitSide.Enemy || target.Hp <= 0) continue;
                if (AbilityTargetCurrentlyLegal(ability, active, target)) count++;
            }
            return count;
        }

        private bool AbilityTargetCurrentlyLegal(MartialAbility ability, CombatUnit active, CombatUnit target)
        {
            if (ability == null || active == null || target == null) return false;
            return CanTargetAbility(active, ability, target, target.X, target.Y, out _);
        }

        private string FormulaNoTargetReason(FormulaDef formula, CombatUnit caster)
        {
            if (formula == null) return "No legal target is available.";
            if (formula.Effect == "dispel") return "No ritual or hostile field is currently in range. Move or choose another spell.";
            if (formula.Effect == "summon")
            {
                int current = ActiveSummonBurdenFor(caster);
                int burden = SummonBurden(formula.SummonRole);
                int maximum = MaxPactSummonBurdenFor(caster);
                if (current + burden > maximum)
                {
                    return $"Pact burden is full ({current}/{maximum}). End a binding before calling this summon.";
                }
                return "No open summon tile is currently in range. Move or clear space first.";
            }
            if (formula.Target == "enemy") return "No enemy is currently in range with a legal path. Move or choose another spell.";
            if (formula.Target == "ally") return "No ally is currently in range with a legal path. Move or choose another spell.";
            return "No legal tile is currently in range. Move or choose another spell.";
        }

        private string AbilityNoTargetReason(MartialAbility ability, CombatUnit active)
        {
            if (ability == null) return "No legal target is available.";
            if (ability.Id == "execute") return "No enemy at 35% HP or lower is currently within reach.";
            if (ability.Id == "charge") return "No enemy currently has an open charge lane.";
            if (ability.Id == "riftpounce") return "No enemy currently has an open rift-landing tile.";
            if (IsSightLineAbility(ability.Id)) return "No enemy is currently in range with a clear sight line.";
            return "No adjacent enemy is available. Reposition before using this skill.";
        }

        private bool ArmedPowerTargetCurrentlyLegal(CombatUnit active, CombatUnit target)
        {
            if (active == null || target == null) return false;
            FormulaDef formula = selectedAction == ActionMode.Cast ? GetFormula(pendingFormulaCode) : null;
            if (formula != null)
            {
                return FormulaTargetCurrentlyLegal(formula, active, target, target.X, target.Y);
            }
            MartialAbility ability = selectedAction == ActionMode.Ability ? AbilityDef(pendingAbilityId) : null;
            return ability != null && AbilityTargetCurrentlyLegal(ability, active, target);
        }

        private CombatUnit SuggestedArmedPowerTarget(CombatUnit active)
        {
            if (active == null || state?.Combat?.Units == null) return null;
            FormulaDef formula = selectedAction == ActionMode.Cast ? GetFormula(pendingFormulaCode) : null;
            if (formula != null)
            {
                if (formula.Target == "self") return active;
                UnitSide side = formula.Target == "ally" ? UnitSide.Party : UnitSide.Enemy;
                return state.Combat.Units
                    .Where(unit => unit != null
                        && unit.Side == side
                        && unit.Hp > 0
                        && FormulaTargetCurrentlyLegal(formula, active, unit, unit.X, unit.Y))
                    .OrderBy(unit => formula.Target == "ally"
                        ? (float)unit.Hp / Mathf.Max(1, unit.MaxHp)
                        : Distance(active.X, active.Y, unit.X, unit.Y))
                    .ThenBy(unit => Distance(active.X, active.Y, unit.X, unit.Y))
                    .FirstOrDefault();
            }

            MartialAbility ability = selectedAction == ActionMode.Ability ? AbilityDef(pendingAbilityId) : null;
            if (ability == null || !ability.Targeted) return null;
            return state.Combat.Units
                .Where(unit => unit != null
                    && unit.Side == UnitSide.Enemy
                    && unit.Hp > 0
                    && AbilityTargetCurrentlyLegal(ability, active, unit))
                .OrderBy(unit => Distance(active.X, active.Y, unit.X, unit.Y))
                .ThenBy(unit => (float)unit.Hp / Mathf.Max(1, unit.MaxHp))
                .FirstOrDefault();
        }

        private void ApplyFormulaModalArt(CombatAbilityModalCardView view, FormulaDef formula)
        {
            if (view == null || formula == null) return;
            view.Sigil = formula.Code;
            view.AccentHex = FormulaColor(formula).ToHex();
            TryGetFormulaPowerArt(formula, out view.IconTexture, out view.IconSource);
        }

        private void ApplyAbilityModalArt(CombatAbilityModalCardView view, MartialAbility ability)
        {
            if (view == null || ability == null) return;
            view.Sigil = ability.Short;
            switch ((ability.ClassKey ?? "").ToLowerInvariant())
            {
                case "warrior": view.AccentHex = "d7a84e"; break;
                case "rogue": view.AccentHex = "9d74c9"; break;
                case "ranger": view.AccentHex = "58b7a5"; break;
                case "demon": view.AccentHex = "c6576d"; break;
                default: view.AccentHex = "b7aa90"; break;
            }

            TryGetAbilityPowerArt(ability, out view.IconTexture, out view.IconSource);
        }

        private bool TryGetFormulaPowerArt(FormulaDef formula, out Texture2D texture, out Rect source)
        {
            texture = null;
            source = default;
            if (formula == null) return false;

            int signatureIndex = CombatIconCatalog.SignatureSpellIndex(formula.Code);
            if (signatureIndex < 0 || !IsSignatureSpellIconAtlas()) return false;
            texture = signatureSpellIconAtlas;
            source = SignatureSpellIconAtlasCell(signatureIndex);
            return source.width > 0f && source.height > 0f;
        }

        private bool TryGetAbilityPowerArt(MartialAbility ability, out Texture2D texture, out Rect source)
        {
            texture = null;
            source = default;
            int iconIndex = AbilityIconIndex(ability?.Id);
            if (iconIndex < 0 || !IsAbilityIconAtlas()) return false;
            texture = abilityIconAtlas;
            source = AbilityIconAtlasCell(iconIndex);
            return true;
        }

        private string SpellbookModalHeader(CombatUnit active, bool playerTurn)
        {
            if (!playerTurn || active == null) return "Waiting for a party spellcaster turn.";
            if (string.IsNullOrEmpty(active.Spell)) return $"{active.Name} has no trained spell craft.";
            List<FormulaDef> formulas = ActiveFormulaBook().Where(formula => SchoolMatches(formula, active.Spell)).ToList();
            int known = formulas.Count(formula => active.Level >= FormulaRequiredLevel(formula));
            int future = Mathf.Max(0, formulas.Count - known);
            string action = state?.Combat?.ActionAvailable == true ? "Action Ready" : "Action Used";
            string focus = IsFocusedCaster(active) ? "FOCUSED: -1 MP, +1 range" : state?.Combat?.Moved == true ? "Focus spent by movement" : "Hold position to focus";
            return $"{active.Name} / L{active.Level} {SpellCraftLabel(active.Spell)} / MP {active.Mana}/{active.MaxMana} / {known} known + {future} future / {action} / {focus}";
        }

        private static string FirstModalCopy(params string[] values)
        {
            if (values == null) return "";
            foreach (string value in values)
            {
                if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
            }
            return "";
        }

        private Color AvailabilityFallbackColor(CombatAbilityModalCardView card, Color accent)
        {
            if (card == null || card.Locked) return muted;
            if (card.Ready && card.Usable) return gold;
            if (!card.Usable || !CombatAbilityModalPresentationRules.HasCurrentTarget(card)) return Hex("e0b96a");
            return accent;
        }

        private bool NeedsEmergencyCombatAbilityModalFallback()
        {
            return state != null
                && state.Mode == GameMode.Combat
                && CurrentUiOverlay() == UiOverlay.AbilityPicker
                && !HasRenderableGameplayOverlay(UiOverlay.AbilityPicker)
                && !ShouldShowStartupSplash();
        }

        private void DrawEmergencyCombatAbilityModalFallback()
        {
            if (!NeedsEmergencyCombatAbilityModalFallback()) return;

            CombatAbilityModalView view = BuildCombatAbilityModalView();
            if (view == null) return;
            DrawRect(new Rect(0f, 0f, Screen.width, Screen.height), Hex("020303", 0.82f));

            float panelWidth = Mathf.Min(Mathf.Max(680f, Screen.width * 0.76f), Screen.width - 40f);
            float panelHeight = Mathf.Min(Mathf.Max(470f, Screen.height * 0.78f), Screen.height - 40f);
            Rect panel = new Rect((Screen.width - panelWidth) * 0.5f, (Screen.height - panelHeight) * 0.5f, panelWidth, panelHeight);
            DrawRect(panel, Hex("151b20", 0.995f));
            DrawBorder(panel, view.Spellbook ? frost : gold, 2);

            Color accent = view.Spellbook ? frost : gold;
            GUI.Label(new Rect(panel.x + 22f, panel.y + 14f, panel.width - 230f, 28f), view.Title, CenterLeftStyle(22, accent));
            string immediateHeader = string.Join(
                "  •  ",
                new[] { view.Actor, view.Resource, view.ActionState, view.Trait }
                    .Where(part => !string.IsNullOrWhiteSpace(part)));
            GUI.Label(new Rect(panel.x + 22f, panel.y + 48f, panel.width - 44f, 24f), immediateHeader, CenterLeftStyle(12, muted));
            string backLabel = CombatAbilityModalPresentationRules.BackButtonLabel(view.Cards);
            if (GUI.Button(new Rect(panel.xMax - 202f, panel.y + 16f, 178f, 30f), backLabel, buttonStyle))
            {
                CloseCombatAbilityModal();
                return;
            }

            Rect viewport = new Rect(panel.x + 20f, panel.y + 82f, panel.width - 40f, panel.height - 104f);
            int columns = viewport.width >= 1040f ? 3 : 2;
            float gap = 10f;
            float cardWidth = (viewport.width - gap * (columns - 1) - 14f) / columns;
            float cardHeight = 112f;
            int count = view.Cards?.Count ?? 0;
            int rows = Mathf.Max(1, Mathf.CeilToInt(count / (float)columns));
            Rect content = new Rect(0f, 0f, viewport.width - 18f, Mathf.Max(viewport.height, rows * (cardHeight + gap)));
            combatModalFallbackScroll = GUI.BeginScrollView(viewport, combatModalFallbackScroll, content);

            if (count == 0)
            {
                GUI.Label(new Rect(12f, 18f, content.width - 24f, 80f), view.EmptyText, CenterLeftStyle(14, muted));
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    CombatAbilityModalCardView card = view.Cards[i];
                    int row = i / columns;
                    int column = i % columns;
                    Rect cardRect = new Rect(column * (cardWidth + gap), row * (cardHeight + gap), cardWidth, cardHeight);
                    bool selected = string.Equals(view.SelectedId, card.Id, StringComparison.Ordinal);
                    bool canActivate = CombatAbilityModalPresentationRules.CanActivate(card);
                    bool unavailable = !canActivate;
                    DrawRect(cardRect, selected ? Hex("26353a", 0.98f) : Hex("0c1115", 0.96f));
                    DrawBorder(cardRect, (selected ? accent : stone).WithAlpha(selected ? 0.95f : 0.55f), selected ? 2 : 1);
                    Rect iconRect = new Rect(cardRect.x + 10f, cardRect.y + 10f, 64f, 64f);
                    DrawRect(iconRect, Hex("050708", 0.94f));
                    bool drewIcon = card.IconTexture != null
                        && card.IconSource.width > 0f
                        && DrawTextureRegionTint(card.IconTexture, Pad(iconRect, 3f), card.IconSource, Color.white.WithAlpha(unavailable ? 0.38f : 0.96f));
                    DrawBorder(iconRect, selected ? accent : line, 1);
                    if (!drewIcon) GUI.Label(iconRect, card.Sigil ?? "", CenterStyle(12, accent));
                    float textX = cardRect.x + 84f;
                    float textWidth = cardRect.xMax - textX - 12f;
                    bool showState = card.Ready && card.Usable
                        || card.Locked
                        || !card.Usable
                        || !CombatAbilityModalPresentationRules.HasCurrentTarget(card);
                    float statusWidth = showState ? Mathf.Min(94f, textWidth * 0.38f) : 0f;
                    GUI.Label(
                        new Rect(textX, cardRect.y + 8f, textWidth - statusWidth, 22f),
                        card.Name,
                        CenterLeftStyle(14, unavailable ? muted : ink));
                    if (showState)
                    {
                        GUI.Label(
                            new Rect(textX + textWidth - statusWidth, cardRect.y + 8f, statusWidth, 22f),
                            CombatAbilityModalPresentationRules.AvailabilityLabel(card),
                            CenterRightStyle(9, AvailabilityFallbackColor(card, accent)));
                    }
                    string chips = string.IsNullOrWhiteSpace(card.Range)
                        ? card.Cost
                        : $"{card.Cost}  •  {card.Range}";
                    GUI.Label(new Rect(textX, cardRect.y + 32f, textWidth, 18f), chips, CenterLeftStyle(10, accent));
                    string detail = FirstModalCopy(card.RowSummary, card.Summary, card.CurrentEffect);
                    GUIStyle detailStyle = new GUIStyle(labelStyle)
                    {
                        fontSize = 10,
                        wordWrap = true,
                        alignment = TextAnchor.UpperLeft
                    };
                    detailStyle.normal.textColor = unavailable ? muted : cursorWhite;
                    GUI.Label(new Rect(textX, cardRect.y + 52f, textWidth, 34f), detail, detailStyle);
                    string action = CombatAbilityModalPresentationRules.CardActionLabel(card);
                    GUI.enabled = canActivate;
                    if (GUI.Button(new Rect(cardRect.x + 12f, cardRect.yMax - 25f, cardRect.width - 24f, 19f), action, smallButtonStyle))
                    {
                        ActivateCombatAbilityModalCard(card.Id);
                    }
                    GUI.enabled = true;
                }
            }

            GUI.EndScrollView();
        }

        private void CloseCombatAbilityModal()
        {
            if (showSpellbook || showAbilityPanel)
            {
                ActionMode returnFocus = showSpellbook ? ActionMode.Cast : ActionMode.Ability;
                SuppressBoardPointer();
                showSpellbook = false;
                showAbilityPanel = false;
                SyncCombatAbilityModalScreen();
                SyncCombatHudScreen();
                combatHudScreen?.FocusCommand(returnFocus);
                PlaySfx("ui", 0.45f);
                MarkUiDirty();
            }
        }

        private void PreviewCombatAbilityModalCard(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;
            CombatUnit active = CurrentUnit();
            if (showSpellbook)
            {
                spellbookSelectedCode = id;
                RememberCombatAbilityBrowseSelection(active, true, id);
            }
            else if (showAbilityPanel)
            {
                abilitySelectedId = id;
                RememberCombatAbilityBrowseSelection(active, false, id);
            }
            SyncCombatAbilityModalScreen();
        }

        private void SelectCombatAbilityModalCard(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;
            CombatUnit active = CurrentUnit();
            if (showSpellbook)
            {
                spellbookSelectedCode = id;
                RememberCombatAbilityBrowseSelection(active, true, id);
            }
            else if (showAbilityPanel)
            {
                abilitySelectedId = id;
                RememberCombatAbilityBrowseSelection(active, false, id);
            }
            SyncCombatAbilityModalScreen();
            PlaySfx("ui", 0.28f);
        }

        private void ActivateCombatAbilityModalCard(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;
            CombatUnit active = CurrentUnit();
            if (active == null) return;
            if (showSpellbook)
            {
                FormulaDef formula = GetFormula(id);
                if (formula == null) return;
                CombatActionCard card = FormulaActionCard(formula, active);
                if (card == null) return;
                if (card.Ready && card.Usable)
                {
                    SuppressBoardPointer();
                    showSpellbook = false;
                    SyncCombatAbilityModalScreen();
                    SyncCombatHudScreen();
                    PlaySfx("ui", 0.45f);
                    return;
                }
                if (!card.Usable)
                {
                    PushLog(string.IsNullOrWhiteSpace(card.DisabledReason) ? $"{formula.Name} is unavailable." : card.DisabledReason, Tone.Warn);
                    PlaySfx("blocked", 0.62f);
                    SyncCombatAbilityModalScreen();
                    return;
                }
                int legalTargets = CountLegalFormulaTargets(formula, active);
                if (card.Targeted && legalTargets <= 0)
                {
                    PushLog(FormulaNoTargetReason(formula, active), Tone.Warn);
                    PlaySfx("blocked", 0.62f);
                    SyncCombatAbilityModalScreen();
                    return;
                }
                if (!card.Targeted)
                {
                    CombatPowerOutcomeSnapshot formulaOutcomeBefore = CombatPowerOutcomeRules.Capture(state?.Combat);
                    CombatCommandResult formulaResult = CombatLifecycle().TryResolveAction(
                        active,
                        () => CastFormula(active, formula.Code, active, active.X, active.Y));
                    if (formulaResult.Success)
                    {
                        SuppressBoardPointer();
                        SetCombatPowerOutcome(formulaOutcomeBefore);
                        FinishResolvedPlayerFormulaAction(active, formula);
                        showSpellbook = false;
                        SyncCombatAbilityModalScreen();
                        SyncCombatHudScreen();
                    }
                    return;
                }
                PrepareFormulaCode(active, id);
                SyncCombatAbilityModalScreen();
                SyncCombatHudScreen();
                return;
            }

            if (!showAbilityPanel) return;
            MartialAbility ability = AbilityDef(id);
            if (ability == null) return;
            CombatActionCard abilityCard = AbilityActionCard(ability, active);
            if (abilityCard == null) return;
            if (abilityCard.Ready && abilityCard.Usable)
            {
                SuppressBoardPointer();
                showAbilityPanel = false;
                SyncCombatAbilityModalScreen();
                SyncCombatHudScreen();
                PlaySfx("ui", 0.45f);
                return;
            }
            if (!abilityCard.Usable)
            {
                PushLog(string.IsNullOrWhiteSpace(abilityCard.DisabledReason) ? $"{ability.Name} is unavailable." : abilityCard.DisabledReason, Tone.Warn);
                PlaySfx("blocked", 0.62f);
                SyncCombatAbilityModalScreen();
                return;
            }
            if (ability.Targeted)
            {
                if (CountLegalAbilityTargets(ability, active) <= 0)
                {
                    PushLog(AbilityNoTargetReason(ability, active), Tone.Warn);
                    PlaySfx("blocked", 0.62f);
                    SyncCombatAbilityModalScreen();
                    return;
                }
                PrepareAbility(active, id);
                SyncCombatAbilityModalScreen();
                SyncCombatHudScreen();
                return;
            }
            CombatPowerOutcomeSnapshot outcomeBefore = CombatPowerOutcomeRules.Capture(state?.Combat);
            CombatCommandResult result = CombatLifecycle().TryResolveAction(active, () => UseInstantAbility(active, id));
            if (result.Success)
            {
                SuppressBoardPointer();
                SetCombatPowerOutcome(outcomeBefore);
                FinishResolvedPlayerAbilityAction(active, ability);
                showAbilityPanel = false;
                SyncCombatAbilityModalScreen();
                SyncCombatHudScreen();
            }
        }
    }
}
