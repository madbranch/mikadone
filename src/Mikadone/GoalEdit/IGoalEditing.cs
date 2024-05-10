namespace Mikadone.GoalEdit;

public interface IGoalEditing
{
  void AddEdit(IGoalEdit undo, IGoalEdit redo);

  bool TryUndo(Goal root);
  bool TryRedo(Goal root);
}
