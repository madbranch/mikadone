using System.Collections.Generic;

namespace Mikadone;

public class GoalFactory : IGoalFactory
{
  private readonly IGoalIdProvider _goalIdProvider;

  public GoalFactory(IGoalIdProvider goalIdProvider)
    => _goalIdProvider = goalIdProvider;
  
  public Goal CreateGoal(bool isReached, string description, IEnumerable<Goal> prerequisites)
    => new Goal(_goalIdProvider.GetNextId(), isReached, description, prerequisites);

}