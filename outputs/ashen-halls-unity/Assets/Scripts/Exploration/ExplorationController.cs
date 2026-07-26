using System;

namespace AshenHalls
{
    public sealed class ExplorationController
    {
        private readonly GameState state;
        private readonly Func<MapObject, bool> isCurrentObjective;
        private readonly Func<int, int, bool> canStepTo;
        private readonly Func<MapObject, bool> canUseAdjacentWithoutStanding;
        private readonly Func<MapObject, int, int, string> verbFor;
        private readonly Func<MapObject, string> iconFor;
        private readonly Func<MapObject, string> nameFor;
        private int preferredStepX;
        private int preferredStepY = -1;

        public ExplorationController(
            GameState state,
            Func<MapObject, bool> isCurrentObjective,
            Func<int, int, bool> canStepTo,
            Func<MapObject, bool> canUseAdjacentWithoutStanding,
            Func<MapObject, int, int, string> verbFor,
            Func<MapObject, string> iconFor,
            Func<MapObject, string> nameFor)
        {
            this.state = state;
            this.isCurrentObjective = isCurrentObjective;
            this.canStepTo = canStepTo;
            this.canUseAdjacentWithoutStanding = canUseAdjacentWithoutStanding;
            this.verbFor = verbFor;
            this.iconFor = iconFor;
            this.nameFor = nameFor;
        }

        public ExplorationInteraction CurrentInteraction()
        {
            if (state?.Map == null) return ExplorationInteraction.None;
            if (!ExplorationInteractionRules.TryFindUseTarget(
                    state.Map,
                    state.PlayerX,
                    state.PlayerY,
                    isCurrentObjective,
                    canStepTo,
                    canUseAdjacentWithoutStanding,
                    preferredStepX,
                    preferredStepY,
                    out ExplorationUseTarget target))
            {
                return ExplorationInteraction.None;
            }

            MapObject obj = target.Target;
            if (obj == null) return ExplorationInteraction.None;
            return new ExplorationInteraction(
                true,
                obj,
                target.StepX,
                target.StepY,
                verbFor?.Invoke(obj, target.StepX, target.StepY) ?? "Use",
                nameFor?.Invoke(obj) ?? obj.Type.ToString(),
                iconFor?.Invoke(obj) ?? "target");
        }

        public void SetPreferredDirection(int dx, int dy)
        {
            if (Math.Abs(dx) + Math.Abs(dy) != 1) return;
            preferredStepX = dx;
            preferredStepY = dy;
        }

        public bool TryMove(int dx, int dy, out ExplorationMoveResult result)
        {
            result = new ExplorationMoveResult(false, state?.PlayerX ?? 0, state?.PlayerY ?? 0, state?.PlayerX ?? 0, state?.PlayerY ?? 0);
            if (state?.Map == null) return false;
            if (Math.Abs(dx) + Math.Abs(dy) != 1) return false;

            int oldX = state.PlayerX;
            int oldY = state.PlayerY;
            int nx = oldX + dx;
            int ny = oldY + dy;
            if (!InsideMap(nx, ny) || canStepTo == null || !canStepTo(nx, ny))
            {
                result = new ExplorationMoveResult(false, oldX, oldY, nx, ny);
                return false;
            }

            state.PlayerX = nx;
            state.PlayerY = ny;
            result = new ExplorationMoveResult(true, oldX, oldY, nx, ny);
            return true;
        }

        public bool TryUseContextualTarget(out ExplorationCommandResult result)
        {
            ExplorationInteraction interaction = CurrentInteraction();
            if (!interaction.HasTarget)
            {
                result = new ExplorationCommandResult(ExplorationCommandKind.None, interaction, new ExplorationMoveResult(false, 0, 0, 0, 0));
                return false;
            }

            if (interaction.IsUnderfoot)
            {
                ExplorationCommandKind kind = interaction.Target.Type == ObjectType.Stairs
                    ? ExplorationCommandKind.Descend
                    : ExplorationCommandKind.ResolveTile;
                result = new ExplorationCommandResult(kind, interaction, new ExplorationMoveResult(false, state.PlayerX, state.PlayerY, state.PlayerX, state.PlayerY));
                return true;
            }

            if (canUseAdjacentWithoutStanding != null && canUseAdjacentWithoutStanding(interaction.Target))
            {
                result = new ExplorationCommandResult(ExplorationCommandKind.ResolveTarget, interaction, new ExplorationMoveResult(false, state.PlayerX, state.PlayerY, interaction.Target.X, interaction.Target.Y));
                return true;
            }

            TryMove(interaction.StepX, interaction.StepY, out ExplorationMoveResult move);
            result = new ExplorationCommandResult(move.Moved ? ExplorationCommandKind.Move : ExplorationCommandKind.None, interaction, move);
            return move.Moved;
        }

        private bool InsideMap(int x, int y)
        {
            return x >= 0 && y >= 0 && x < state.Map.Width && y < state.Map.Height;
        }
    }
}
