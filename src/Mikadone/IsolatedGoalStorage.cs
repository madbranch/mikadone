using System.IO;
using System.IO.IsolatedStorage;

namespace Mikadone;

public sealed class IsolatedGoalStorage : IGoalStorage
{
  private const IsolatedStorageScope Scope = IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain;

  public Stream? OpenRead(string fileName)
  {
    using IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetStore(Scope, null, null);

    if (!isolatedStorageFile.FileExists(fileName))
    {
      return null;
    }

    return isolatedStorageFile.OpenFile(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
  }

  public Stream OpenWrite(string fileName)
  {
    using IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetStore(Scope, null, null);

    return isolatedStorageFile.OpenFile(fileName, FileMode.Create, FileAccess.Write);
  }
}