using System.Collections.Generic;

namespace Mikadone;

public interface IGoalFactory
{
  Goal CreateGoal(bool isReached, string description, IEnumerable<Goal> prerequisites);
}