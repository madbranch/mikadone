using ReactiveUI;

namespace Mikadone;

public class GoalsViewModel : ReactiveObject
{
  private Goal? _selectedGoal;

  public Goal Root { get; } = new Goal();

  public Goal? SelectedGoal
  {
    get => _selectedGoal;
    set => this.RaiseAndSetIfChanged(ref _selectedGoal, value);
  }
}