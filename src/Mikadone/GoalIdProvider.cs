namespace Mikadone;

public class GoalIdProvider : IGoalIdProvider
{
  private int _nextId = 1;
  public int GetNextId() => _nextId++;
}