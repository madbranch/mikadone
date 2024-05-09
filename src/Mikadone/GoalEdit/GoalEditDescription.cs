namespace Mikadone.GoalEdit;

public record GoalEditDescription(GoalPath Path, string Description) : IGoalEdit
{
  public void Do(Goal root)
    => root.GetGoal(Path).Description = Description;
}
