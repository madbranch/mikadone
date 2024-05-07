namespace Mikadone;

public class GoalIdProvider : IGoalIdProvider
{
  private int _nextId = 1;
  public GoalId GetNextId() => new GoalId(_nextId++);
}