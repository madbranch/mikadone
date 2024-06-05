using System.IO;

namespace Mikadone;

public interface IGoalSerialization
{
  Stream Serialize(Goal goal, Stream stream);
}
