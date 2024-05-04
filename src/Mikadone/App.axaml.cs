using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Mikadone;

public partial class App : Application
{
  public override void Initialize() => AvaloniaXamlLoader.Load(this);

  public override void OnFrameworkInitializationCompleted()
  {
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      GoalsViewModel goals = new GoalsViewModel();

      goals.Root.AddPrerequisite( new Goal() { Description = "Oh yeah"} );
      goals.Root.Prerequisites[0].AddPrerequisite( new Goal() { Description = "Meh"} );
      goals.Root.Prerequisites[0].AddPrerequisite( new Goal() { Description = "Huh"} );
      goals.Root.AddPrerequisite( new Goal() { Description = "Oh no"} );

      desktop.MainWindow = new MainWindow
      {
        DataContext = new MainViewModel(goals),
      };
    }

    base.OnFrameworkInitializationCompleted();
  }
}