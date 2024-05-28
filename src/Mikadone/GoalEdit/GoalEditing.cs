using System;
using System.Collections.Generic;
using System.Linq;

namespace Mikadone.GoalEdit;

public class GoalEditing : IGoalEditing
{
  public void AddEdit(IGoalEdit undo, IGoalEdit redo)
  {
    System.Diagnostics.Trace.WriteLine($"Adding edit: {undo}, {redo}");
    _redos.Clear();
    _undos.Add((undo, redo));

    if (_undos.Count > 100)
    {
      _undos.RemoveAt(0);
    }
  }
  
  public bool TryRedo(Goal root)
  {
    if (_isEditing)
    {
      throw new InvalidOperationException("Don't redo while already undoing or redoing.");
    }

    if (_redos.Count == 0)
    {
      return false;
    }

    int index = _redos.Count - 1;
    (IGoalEdit Undo, IGoalEdit Redo) item = _redos[index];
    _redos.RemoveAt(index);

    try
    {
      _isEditing = true;
      item.Redo.Do(root);
    }
    finally
    {
      _isEditing = false;
    }

    _undos.Add(item);
    return true;
  }

  public bool TryUndo(Goal root)
  {
    if (_isEditing)
    {
      throw new InvalidOperationException("Don't undo while already undoing or redoing.");
    }

    if (_undos.Count == 0)
    {
      return false;
    }

    int index = _undos.Count - 1;
    (IGoalEdit Undo, IGoalEdit Redo) item = _undos[index];
    _undos.RemoveAt(index);

    try
    {
      _isEditing = true;
      item.Undo.Do(root);
    }
    finally
    {
      _isEditing = false;
    }

    _redos.Add(item);
    return true;
  }

  private readonly List<(IGoalEdit Undo, IGoalEdit Redo)> _undos = [];
  private readonly List<(IGoalEdit Undo, IGoalEdit Redo)> _redos = [];
  bool _isEditing;
}
