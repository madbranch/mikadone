using System;
using System.Windows.Input;
using Mikadone.GoalEdit;
using ReactiveUI;

namespace Mikadone;

public class GoalsViewModel : ReactiveObject
{
  private Goal? _selectedGoal;
  private Goal _root;
  private readonly ReactiveCommand<string, Goal?> _addNewPrerequisiteCommand;
  private readonly IRootGoalProvider _rootGoalProvider;
  private readonly IGoalFactory _goalFactory;
  private readonly IGoalEditing _goalEditing;


  public GoalsViewModel(IRootGoalProvider rootGoalProvider, IGoalFactory goalFactory, IGoalEditing goalEditing)
  {
    _rootGoalProvider = rootGoalProvider;
    _goalFactory = goalFactory;
    _goalEditing = goalEditing;
    _root = _rootGoalProvider.GetRootGoal();
    IObservable<bool> canExecuteAddNewPrerequisite = this.WhenAnyValue<GoalsViewModel, bool, Goal?>(x => x.SelectedGoal, x => x is not null);
    _addNewPrerequisiteCommand = ReactiveCommand.Create<string, Goal?>(ExecuteAddNewPrerequisiteCommand, canExecuteAddNewPrerequisite);
  }

  public Goal Root
  {
    get => _root;
    set => this.RaiseAndSetIfChanged(ref _root, value);
  }

  public Goal? SelectedGoal
  {
    get => _selectedGoal;
    set => this.RaiseAndSetIfChanged(ref _selectedGoal, value);
  }

  public ICommand AddNewPrerequisite => _addNewPrerequisiteCommand;

  private Goal? ExecuteAddNewPrerequisiteCommand(string description)
  {
    if (SelectedGoal is not Goal selectedGoal)
    {
      return null;
    }

    Goal newPrerequisite = _goalFactory.CreateGoal(false, description, []);
    selectedGoal.AddPrerequisite(newPrerequisite);
    newPrerequisite.BeginEdit();
    SelectedGoal = newPrerequisite;
    _goalEditing.AddEdit(undo: new GoalRemoval(newPrerequisite.GetPath()),
                         redo: new GoalInsertion(selectedGoal.GetPath(), newPrerequisite, selectedGoal.Prerequisites.Count - 1));
    return newPrerequisite;
  }
}