using System.IO;
using System.Text;

namespace Mikadone;

public class RootGoalProvider : IRootGoalProvider
{
  private readonly IGoalFactory _goalFactory;
  private readonly IGoalDeserialization _goalDeserialization;
  private readonly IGoalStorage _goalStorage;
  private readonly IGoalFileNameProvider _goalFileNameProvider;

  private static readonly Encoding UTF8WithoutBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

  public RootGoalProvider(IGoalFactory goalFactory, IGoalDeserialization goalSerialization, IGoalStorage goalStorage, IGoalFileNameProvider goalFileNameProvider)
  {
    _goalFactory = goalFactory;
    _goalDeserialization = goalSerialization;
    _goalStorage = goalStorage;
    _goalFileNameProvider = goalFileNameProvider;
    Root = GetRootGoal();
  }

  public Goal Root { get; }

  private Goal GetRootGoal()
  {
    using Stream? stream = _goalStorage.OpenRead(_goalFileNameProvider.FileName);

    if (stream is null)
    {
      return GetDefaultRootGoal();
    }

    using StreamReader reader = new StreamReader(stream: stream,
                                                 encoding: UTF8WithoutBOM,
                                                 detectEncodingFromByteOrderMarks: false);

    return _goalDeserialization.Deserialize(reader.ReadToEnd())
      ?? GetDefaultRootGoal();
  }

  private Goal GetDefaultRootGoal()
    => _goalFactory.CreateGoal(false, "", [_goalFactory.CreateGoal(false, "Please enjoy Mikadone!", [])]);
}