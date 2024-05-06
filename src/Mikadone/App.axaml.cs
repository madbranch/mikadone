using System.IO;
using System.Text;
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
      Goal root = GetRoot();

      GoalsViewModel goals = new GoalsViewModel(root);

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

  private static Goal GetRoot()
  {
      IsolatedGoalStorage isolatedGoalStorage = new();

      using Stream? stream = isolatedGoalStorage.OpenRead("todo.md");

      if (stream is null)
      {
        return new Goal();
      }
  
      using StreamReader reader = new StreamReader(stream: stream,
                                                   encoding: UTF8WithoutBOM,
                                                   detectEncodingFromByteOrderMarks: false);

      GoalSerialization goalSerialization = new();

      return goalSerialization.Deserialize(reader.ReadToEnd())
        ?? new Goal();
  }

  private static readonly Encoding UTF8WithoutBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
}