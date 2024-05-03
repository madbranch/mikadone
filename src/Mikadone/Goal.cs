using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using Mikadone.Tree;
using ReactiveUI;

namespace Mikadone;

public class Goal : ReactiveObject, INode<Goal>
{
  private Guid _id;
  private bool _isReached;
  private string _description = string.Empty;
  private readonly SourceList<Goal> _prerequisitesSource = new();
  private readonly ReadOnlyObservableCollection<Goal> _prerequisites;
  private Goal? _parent;

  public Goal()
  {
    _ = _prerequisitesSource
      .Connect()
      .ObserveOn(RxApp.MainThreadScheduler)
      .Bind(out _prerequisites)
      .Subscribe();
  }

  public Guid Id
  {
    get => _id;
    set => this.RaiseAndSetIfChanged(ref _id, value);
  }

  public bool IsReached
  {
    get => _isReached;
    set => this.RaiseAndSetIfChanged(ref _isReached, value);
  }
  
  public string Description
  {
    get => _description;
    set => this.RaiseAndSetIfChanged(ref _description, value);
  }

  public ReadOnlyObservableCollection<Goal> Prerequisites => _prerequisites;

  public Goal? Parent
  {
    get => _parent;
    private set => this.RaiseAndSetIfChanged(ref _parent, value);
  }

  int ICollection<Goal>.Count => _prerequisites.Count;

  bool ICollection<Goal>.IsReadOnly => false;

  Goal IList<Goal>.this[int index]
  {
    get => _prerequisites[index];
    set
    {
      _prerequisites[index].Parent = null;

      value.Parent = this;
      _prerequisitesSource.ReplaceAt(index, value);
    }

  }

  int IList<Goal>.IndexOf(Goal item) => throw new NotImplementedException();
  void IList<Goal>.Insert(int index, Goal item) => throw new NotImplementedException();
  void IList<Goal>.RemoveAt(int index) => throw new NotImplementedException();
  void ICollection<Goal>.Add(Goal item)
  {
    item.Parent = this;
    _prerequisitesSource.Add(item);
  }
  void ICollection<Goal>.Clear() => throw new NotImplementedException();
  bool ICollection<Goal>.Contains(Goal item) => throw new NotImplementedException();
  void ICollection<Goal>.CopyTo(Goal[] array, int arrayIndex) => throw new NotImplementedException();
  bool ICollection<Goal>.Remove(Goal item) => throw new NotImplementedException();
  IEnumerator<Goal> IEnumerable<Goal>.GetEnumerator() => throw new NotImplementedException();
  IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

}