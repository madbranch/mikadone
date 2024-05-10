using System.Collections.Generic;
using System.Reactive.Linq;
using DynamicData.Binding;
using ReactiveUI;

namespace Mikadone;

public class GoalsViewModel : ReactiveObject
{
  private Goal? _selectedGoal;
  private Goal _root;

  public GoalsViewModel(IRootGoalProvider rootGoalProvider)
  {
    _root = rootGoalProvider.GetRootGoal();
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
}