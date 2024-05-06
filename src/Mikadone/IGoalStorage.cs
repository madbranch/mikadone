using System.IO;

namespace Mikadone;

public interface IGoalStorage
{
  Stream? OpenRead(string fileName);
  Stream OpenWrite(string fileName);
}
