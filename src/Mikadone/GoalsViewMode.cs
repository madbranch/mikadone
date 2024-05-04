using ReactiveUI;

namespace Mikadone;

public class GoalsViewModel : ReactiveObject
{
  public Goal Root { get; } = new Goal();
}