using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mikadone.GoalEdit;

namespace Mikadone;

public partial class GoalsViewModel : ObservableObject
{
  [ObservableProperty]
  [NotifyCanExecuteChangedFor(nameof(AddNewPrerequisiteCommand))]
  [NotifyCanExecuteChangedFor(nameof(AddNewSiblingPrerequisiteCommand))]
  private Goal? _selectedGoal;

  [ObservableProperty]
  private Goal _root;

  private readonly IGoalFactory _goalFactory;
  private readonly IGoalEditing _goalEditing;
  private readonly IGoalAutoSave _goalAutoSave;

  public GoalsViewModel(IRootGoalProvider rootGoalProvider, IGoalFactory goalFactory, IGoalEditing goalEditing, IGoalAutoSave goalAutoSave)
  {
    _goalFactory = goalFactory;
    _goalEditing = goalEditing;
    _goalAutoSave = goalAutoSave;

    _root = rootGoalProvider.Root;
  }

  private bool CanAddNewPrerequisite(string description)
    => SelectedGoal is not null;

  [RelayCommand(CanExecute = nameof(CanAddNewPrerequisite))]
  private void AddNewPrerequisite(string description)
  {
    if (SelectedGoal is not Goal selectedGoal)
    {
      return;
    }

    Goal newPrerequisite = _goalFactory.CreateGoal(false, description, []);
    selectedGoal.AddPrerequisite(newPrerequisite);
    newPrerequisite.BeginEdit();
    SelectedGoal = newPrerequisite;
    _goalEditing.AddEdit(undo: new GoalRemoval(newPrerequisite.GetPath()),
                         redo: new GoalInsertion(selectedGoal.GetPath(), newPrerequisite, selectedGoal.Prerequisites.Count - 1));
    _goalAutoSave.Save(Root);
  }

  [RelayCommand]
  private void Undo()
  {
    if (_goalEditing.TryUndo(Root))
    {
      _goalAutoSave.Save(Root);
    }
  }

  [RelayCommand]
  private void Redo()
  {
    if (_goalEditing.TryRedo(Root))
    {
      _goalAutoSave.Save(Root);
    }
  }

  [RelayCommand(CanExecute = nameof(CanAddNewPrerequisite))]
  private void AddNewSiblingPrerequisite(string description)
  {
    if (SelectedGoal is not Goal selectedGoal
      || selectedGoal.Parent is not Goal parent)
    {
      return;
    }

    Goal newPrerequisite = _goalFactory.CreateGoal(false, description, []);
    int selectedGoalIndex = parent.Prerequisites.IndexOf(selectedGoal);
    int newPrerequisiteIndex = selectedGoalIndex + 1;
    parent.InsertPrerequisite(newPrerequisiteIndex, newPrerequisite);
    newPrerequisite.BeginEdit();
    SelectedGoal = newPrerequisite;
    _goalEditing.AddEdit(undo: new GoalRemoval(newPrerequisite.GetPath()),
                         redo: new GoalInsertion(parent.GetPath(), newPrerequisite, newPrerequisiteIndex));
    _goalAutoSave.Save(Root);
  }
}