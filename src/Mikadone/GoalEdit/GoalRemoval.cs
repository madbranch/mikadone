namespace Mikadone.GoalEdit;

public class GoalRemoval : IGoalEdit
{
  private readonly GoalPath _path;

  public GoalRemoval(GoalPath path)
    => _path = path;

  public void Do(Goal root)
  {
    Goal goal = root.GetGoal(_path);
    goal.Parent!.RemovePrerequisite(goal);
  }
}