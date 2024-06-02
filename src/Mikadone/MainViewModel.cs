using CommunityToolkit.Mvvm.ComponentModel;

namespace Mikadone;

public class MainViewModel : ObservableObject
{
  public MainViewModel(GoalsViewModel goals)
  {
    Goals = goals;
  }

  public GoalsViewModel Goals { get; }
}