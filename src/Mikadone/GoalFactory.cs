using System.Collections.Generic;
using Mikadone.GoalEdit;

namespace Mikadone;

public class GoalFactory : IGoalFactory
{
  private readonly IGoalIdProvider _goalIdProvider;
  private readonly IGoalEditing _goalEditing;


  public GoalFactory(IGoalIdProvider goalIdProvider, IGoalEditing goalEditing)
  {
    _goalIdProvider = goalIdProvider;
    _goalEditing = goalEditing;
  }


  public Goal CreateGoal(bool isReached, string description, IEnumerable<Goal> prerequisites)
    => new Goal(_goalIdProvider.GetNextId(), isReached, description, prerequisites, _goalEditing);

}