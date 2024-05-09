namespace Mikadone.GoalEdit;

public class GoalInsertion : IGoalEdit
{
  private readonly GoalPath _path;
  private readonly Goal _goal;
  private readonly int _index;

  public GoalInsertion(GoalPath path, Goal goal, int index)
  {
    _path = path;
    _goal = goal;
    _index = index;
  }

  public void Do(Goal root)
    => root.GetGoal(_path).InsertPrerequisite(_index, _goal);
}