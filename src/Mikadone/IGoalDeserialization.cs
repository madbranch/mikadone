namespace Mikadone;

public interface IGoalDeserialization
{
  Goal? Deserialize(string jsonString);
}
