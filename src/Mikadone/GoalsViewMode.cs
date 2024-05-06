using ReactiveUI;

namespace Mikadone;

public class GoalsViewModel : ReactiveObject
{
  private Goal? _selectedGoal;
  private Goal _root;

  public GoalsViewModel()
    : this(new Goal())
  {
  }

  public GoalsViewModel(Goal root)
    => _root = root;

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