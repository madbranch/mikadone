using System;
using System.Windows.Input;
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

  private readonly IRootGoalProvider _rootGoalProvider;
  private readonly IGoalFactory _goalFactory;
  private readonly IGoalEditing _goalEditing;

  public GoalsViewModel(IRootGoalProvider rootGoalProvider, IGoalFactory goalFactory, IGoalEditing goalEditing)
  {
    _rootGoalProvider = rootGoalProvider;
    _goalFactory = goalFactory;
    _goalEditing = goalEditing;
    _root = _rootGoalProvider.GetRootGoal();
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
  }

  [RelayCommand]
  private void Undo()
    => _goalEditing.TryUndo(Root);

  [RelayCommand]
  private void Redo()
    => _goalEditing.TryRedo(Root);

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
  }
}