using ReactiveUI;

namespace Mikadone;

public class MainViewModel : ReactiveObject
{
  public MainViewModel(GoalsViewModel goals)
  {
    Goals = goals;
  }

  public GoalsViewModel Goals { get; }
}