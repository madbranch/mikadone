using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Mikadone.GoalEdit;

namespace Mikadone;

public sealed partial class Goal : ObservableObject, IEditableObject
{
  [ObservableProperty]
  private Goal? _parent;

  [ObservableProperty]
  private bool _isEditing;

  [ObservableProperty]
  private bool _isReached;

  partial void OnIsReachedChanged(bool oldValue, bool newValue)
  {
    GoalPath path = GetPath();
    _goalEditing.AddEdit(
      new GoalEditIsReached(path, oldValue),
      new GoalEditIsReached(path, newValue));
  }

  [ObservableProperty]
  private string _description;

  [ObservableProperty]
  private bool _isExpanded = true;

  private readonly IGoalEditing _goalEditing;
  private string? _originalDescription;

  public Goal(GoalId id,
              bool isReached,
              string description,
              IEnumerable<Goal> prerequisites,
              IGoalEditing goalEditing)
  {
    Id = id;
    _isReached = isReached;
    _description = description;
    _goalEditing = goalEditing;
    foreach (Goal prerequisite in prerequisites)
    {
      prerequisite.Parent = this;
    }

    Prerequisites = new ObservableCollection<Goal>(prerequisites);

    _goalEditing = goalEditing;
  }

  public GoalId Id { get; }

  public ObservableCollection<Goal> Prerequisites { get; }

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
    Prerequisites.Insert(index, prerequisite);
  }

  public void AddPrerequisite(Goal prerequisite)
  {
    if (prerequisite.Parent is not null)
    {
      throw new ArgumentException($"Prerequisite already has a parent: {prerequisite}");
    }

    prerequisite.Parent = this;
    Prerequisites.Add(prerequisite);
  }

  public bool RemovePrerequisite(Goal prerequisite)
  {
    bool isRemoved = Prerequisites.Remove(prerequisite);

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

    foreach (Goal prerequisite in Prerequisites)
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
    if (_originalDescription is string originalDescription
      && originalDescription != Description)
    {
      GoalPath path = GetPath();
      _goalEditing.AddEdit(
        new GoalEditDescription(path, originalDescription),
        new GoalEditDescription(path, Description)
      );
    }

    _originalDescription = null;
    IsEditing = false;
  }
}