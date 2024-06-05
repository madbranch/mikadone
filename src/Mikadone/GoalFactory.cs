using System.Collections.Generic;
using Mikadone.GoalEdit;

namespace Mikadone;

public class GoalFactory : IGoalFactory
{
  private readonly IGoalIdProvider _goalIdProvider;
  private readonly IGoalEditing _goalEditing;
  private readonly IGoalAutoSave _goalAutoSave;

  public GoalFactory(IGoalIdProvider goalIdProvider, IGoalEditing goalEditing, IGoalAutoSave goalAutoSave)
  {
    _goalIdProvider = goalIdProvider;
    _goalEditing = goalEditing;
    _goalAutoSave = goalAutoSave;
  }

  public Goal CreateGoal(bool isReached, string description, IEnumerable<Goal> prerequisites)
    => new Goal(_goalIdProvider.GetNextId(), isReached, description, prerequisites, _goalEditing, _goalAutoSave);
}