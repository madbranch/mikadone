using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using DynamicData;
using Mikadone.GoalEdit;
using System.Reactive;
using System.Windows.Input;

namespace Mikadone;

public sealed class Goal : ReactiveObject, IEditableObject
{
  private bool _isReached;
  private string _description;
  private readonly SourceList<Goal> _prerequisitesSource = new();
  private readonly ReadOnlyObservableCollection<Goal> _prerequisites;
  private readonly IGoalEditing _goalEditing;
  private readonly IGoalFactory _goalFactory;
  private Goal? _parent;
  private string? _originalDescription;
  private bool _isEditing;
  private bool _isExpanded = true;

  public Goal(GoalId id,
              bool isReached,
              string description,
              IEnumerable<Goal> prerequisites,
              IGoalEditing goalEditing,
              IGoalFactory goalFactory)
  {
    Id = id;
    _isReached = isReached;
    _description = description;
    _goalEditing = goalEditing;
    _prerequisitesSource.AddRange(prerequisites);
    _goalEditing = goalEditing;
    _goalFactory = goalFactory;

    _ = _prerequisitesSource
      .Connect()
      .ObserveOn(RxApp.MainThreadScheduler)
      .Bind(out _prerequisites)
      .Subscribe();

    _ = this.WhenAny(x => x.IsReached, isReached => isReached)
      .Subscribe(AddIsReachedEdit);
    _ = this.WhenAny(x => x.Description, description => description)
      .Buffer(2, 1)
      .Subscribe(AddDescriptionEdit);

    static void AddIsReachedEdit(IObservedChange<Goal, bool> change)
    {
      GoalPath path = change.Sender.GetPath();
      change.Sender._goalEditing.AddEdit(
        new GoalEditIsReached(path, !change.Value),
        new GoalEditIsReached(path, change.Value));
    }

    static void AddDescriptionEdit(IList<IObservedChange<Goal, string>> changes)
    {
      Goal sender = changes[0].Sender;
      GoalPath path = sender.GetPath();
      sender._goalEditing.AddEdit(
        new GoalEditDescription(path, changes[0].Value),
        new GoalEditDescription(path, changes[1].Value)
      );
    }
  }

  public GoalId Id { get; }

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

  public bool IsEditing
  {
    get => _isEditing;
    private set => this.RaiseAndSetIfChanged(ref _isEditing, value);
  }

  public bool IsExpanded
  {
    get => _isExpanded;
    set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
  }

  public Goal GetRoot()
    => Parent?.GetRoot() ?? this;

  public GoalPath GetPath()
    => Parent is Goal parent
    ? parent.GetPath() + Id
    : GoalPath.Empty;
  
  public Goal GetGoal(GoalPath path)
  {
    if (path.Path.Length == 0)
    {
      return this;
    }

    GoalId id = path.Path[0];

    return Prerequisites.First(prerequisite => prerequisite.Id == id)
      .GetGoal(new GoalPath(path.Path.RemoveAt(0)));
  }

  public void InsertPrerequisite(int index, Goal prerequisite)
  {
    if (prerequisite.Parent is not null)
    {
      throw new ArgumentException($"Prerequisite already has a parent: {prerequisite}");
    }

    prerequisite.Parent = this;
    _prerequisitesSource.Insert(index, prerequisite);
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

  public override bool Equals(object? obj)
    => obj is Goal other
    && IsReached == other.IsReached
    && Description == other.Description
    && Prerequisites.SequenceEqual(other.Prerequisites);

  public override int GetHashCode()
  {
    HashCode hash = new();

    hash.Add(IsEditing);
    hash.Add(Description);

    foreach(Goal prerequisite in Prerequisites)
    {
      hash.Add(prerequisite);
    }

    return hash.ToHashCode();
  }

  private static char XIfTrue(bool value)
    => value ? 'x' : ' ';
  
  public void BeginEdit()
  {
    if (_originalDescription is not null)
    {
      // It's already being edited, so we ignore the call.
      return;
    }

    _originalDescription = Description;
    IsEditing = true;
  }

  public void CancelEdit()
  {
    if (_originalDescription is null)
    {
      // It's not being edited, so we ignore the call.
      return;
    }

    IsEditing = false;
    Description = _originalDescription;
    _originalDescription = null;
  }

  public void EndEdit()
  {
    _originalDescription = null;
    IsEditing = false;
  }
}