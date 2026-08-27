using UnityEngine;

namespace AshenHalls
{
    public enum ExplorationCommandKind
    {
        None,
        Move,
        Descend,
        ResolveTile,
        ResolveTarget
    }

    public readonly struct ExplorationInteraction
    {
        public readonly bool Available;
        public readonly MapObject Target;
        public readonly int StepX;
        public readonly int StepY;
        public readonly string Verb;
        public readonly string TargetName;
        public readonly string Icon;

        public ExplorationInteraction(bool available, MapObject target, int stepX, int stepY, string verb, string targetName, string icon)
        {
            Available = available;
            Target = target;
            StepX = stepX;
            StepY = stepY;
            Verb = verb ?? "";
            TargetName = targetName ?? "";
            Icon = icon ?? "target";
        }

        public bool HasTarget => Available && Target != null;
        public bool IsUnderfoot => StepX == 0 && StepY == 0;
        public string ActionLine => HasTarget ? $"Space/E/A {Verb}: {TargetName}" : "";

        public static ExplorationInteraction None => new ExplorationInteraction(false, null, 0, 0, "", "", "target");
    }

    public readonly struct ExplorationMoveResult
    {
        public readonly bool Moved;
        public readonly int OldX;
        public readonly int OldY;
        public readonly int NewX;
        public readonly int NewY;

        public ExplorationMoveResult(bool moved, int oldX, int oldY, int newX, int newY)
        {
            Moved = moved;
            OldX = oldX;
            OldY = oldY;
            NewX = newX;
            NewY = newY;
        }

        public Vector2 OldPosition => new Vector2(OldX, OldY);
        public Vector2 NewPosition => new Vector2(NewX, NewY);
    }

    public readonly struct ExplorationCommandResult
    {
        public readonly ExplorationCommandKind Kind;
        public readonly ExplorationInteraction Interaction;
        public readonly ExplorationMoveResult Move;

        public ExplorationCommandResult(ExplorationCommandKind kind, ExplorationInteraction interaction, ExplorationMoveResult move)
        {
            Kind = kind;
            Interaction = interaction;
            Move = move;
        }

        public bool Succeeded => Kind != ExplorationCommandKind.None;
    }
}
