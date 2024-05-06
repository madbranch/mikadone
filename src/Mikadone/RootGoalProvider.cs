using System.IO;
using System.Text;

namespace Mikadone;

public class RootGoalProvider : IRootGoalProvider
{
  private readonly IGoalFactory _goalFactory;
  private readonly IGoalSerialization _goalSerialization;

  private static readonly Encoding UTF8WithoutBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

  public RootGoalProvider(IGoalFactory goalFactory, IGoalSerialization goalSerialization)
  {
    _goalFactory = goalFactory;
    _goalSerialization = goalSerialization;
  }


  public Goal GetRootGoal()
  {
      IsolatedGoalStorage isolatedGoalStorage = new();

      using Stream? stream = isolatedGoalStorage.OpenRead("todo.md");

      if (stream is null)
      {
        return _goalFactory.CreateGoal(false, string.Empty, []);
      }
  
      using StreamReader reader = new StreamReader(stream: stream,
                                                   encoding: UTF8WithoutBOM,
                                                   detectEncodingFromByteOrderMarks: false);

      return _goalSerialization.Deserialize(reader.ReadToEnd())
        ?? _goalFactory.CreateGoal(false, string.Empty, []);
  }
}