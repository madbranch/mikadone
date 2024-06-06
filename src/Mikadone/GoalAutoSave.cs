using System.IO;

namespace Mikadone;

public class GoalAutoSave : IGoalAutoSave
{
  private readonly IGoalSerialization _goalSerialization;
  private readonly IGoalStorage _goalStorage;

  private readonly IGoalFileNameProvider _goalFileNameProvider;

  public GoalAutoSave(IGoalSerialization goalSerialization,
                      IGoalStorage goalStorage,
                      IGoalFileNameProvider goalFileNameProvider)
  {
    _goalSerialization = goalSerialization;
    _goalStorage = goalStorage;
    _goalFileNameProvider = goalFileNameProvider;
  }

  public void Save(Goal root)
  {
    using Stream stream = GetStream();
    _goalSerialization.Serialize(root, stream);
  }


  private Stream GetStream()
    => _goalStorage.OpenWrite(_goalFileNameProvider.FileName);
}