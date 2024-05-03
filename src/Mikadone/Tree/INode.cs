using System.Collections;
using System.Collections.Generic;

namespace Mikadone.Tree;

public interface INode<T> : IList<T>
  where T : INode<T>
{
  T? Parent { get; }
}