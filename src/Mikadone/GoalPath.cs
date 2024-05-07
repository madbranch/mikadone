using System.Collections.Immutable;

namespace Mikadone;

public record struct GoalPath(ImmutableArray<GoalId> Path)
{
  public static readonly GoalPath Empty = new GoalPath(ImmutableArray<GoalId>.Empty);

  public static GoalPath operator +(GoalPath path, GoalId id)
    => new GoalPath(path.Path.Add(id));
}
