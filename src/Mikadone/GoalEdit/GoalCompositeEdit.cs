namespace Mikadone.GoalEdit;

public class GoalCompositeEdit : IGoalEdit
{
  private readonly IGoalEdit[] _edits;

  public GoalCompositeEdit(IGoalEdit[] edits)
    => _edits = edits;

  public void Do(Goal root)
  {
    foreach (IGoalEdit edit in _edits)
    {
      edit.Do(root);
    }
  }
}
