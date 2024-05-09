namespace Mikadone.GoalEdit;

public record GoalEditIsReached(GoalPath Path, bool IsReached) : IGoalEdit
{
  public void Do(Goal root)
    => root.GetGoal(Path).IsReached = IsReached;
}
