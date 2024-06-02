using System;
using System.IO;
using System.Text;

namespace Mikadone;

public class RootGoalProvider : IRootGoalProvider
{
  private readonly IGoalFactory _goalFactory;
  private readonly IGoalSerialization _goalSerialization;
  private readonly IGoalStorage _goalStorage;
  private static readonly Encoding UTF8WithoutBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

  public RootGoalProvider(IGoalFactory goalFactory, IGoalSerialization goalSerialization, IGoalStorage goalStorage)
  {
    _goalFactory = goalFactory;
    _goalSerialization = goalSerialization;
    _goalStorage = goalStorage;
  }

  public Goal GetRootGoal()
  {
    using Stream? stream = _goalStorage.OpenRead("todo.md");

    if (stream is null)
    {
      return GetDefaultRootGoal();
    }

    using StreamReader reader = new StreamReader(stream: stream,
                                                 encoding: UTF8WithoutBOM,
                                                 detectEncodingFromByteOrderMarks: false);

    return _goalSerialization.Deserialize(reader.ReadToEnd())
      ?? GetDefaultRootGoal();
  }

  private Goal GetDefaultRootGoal()
    => _goalFactory.CreateGoal(false, "", [_goalFactory.CreateGoal(false, "Please enjoy Mikadone!", [])]);

}