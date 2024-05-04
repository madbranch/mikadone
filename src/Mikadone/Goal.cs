using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;

namespace Mikadone;

public class Goal : ReactiveObject
{
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

  public void AddPrerequisite(Goal prerequisite)
  {
    if (prerequisite.Parent is not null)
    {
      throw new ArgumentException($"Prerequisite already has a parent: {prerequisite}");
    }

    prerequisite.Parent = this;
    _prerequisitesSource.Add(prerequisite);
  }

  public bool RemovePrerequisite(Goal prerequisite)
  {
    bool isRemoved = _prerequisitesSource.Remove(prerequisite);

    if (isRemoved)
    {
      prerequisite.Parent = null;
    }

    return isRemoved;
  }

  public override string ToString()
    => $"[{XIfTrue(IsReached)}] {Description}";

  private static char XIfTrue(bool value)
    => value ? 'x' : ' ';

}