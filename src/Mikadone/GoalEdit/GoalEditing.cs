using System;
using System.Collections.Generic;

namespace Mikadone.GoalEdit;

public class GoalEditing : IGoalEditing
{
  public void AddEdit(IGoalEdit undo, IGoalEdit redo)
  {
    _redos.Clear();
    _undos.Push((undo, redo));
  }
  
  public bool TryRedo(Goal root)
  {
    if (_isEditing)
    {
      throw new InvalidOperationException("Don't redo while already undoing or redoing.");
    }

    if (!_redos.TryPop(out (IGoalEdit Undo, IGoalEdit Redo) item))
    {
      return false;
    }

    try
    {
      _isEditing = true;
      item.Redo.Do(root);
    }
    finally
    {
      _isEditing = false;
    }

    _undos.Push(item);
    return true;
  }

  public bool TryUndo(Goal root)
  {
    if (_isEditing)
    {
      throw new InvalidOperationException("Don't undo while already undoing or redoing.");
    }

    if (!_undos.TryPop(out (IGoalEdit Undo, IGoalEdit Redo) item))
    {
      return false;
    }

    try
    {
      _isEditing = true;
      item.Undo.Do(root);
    }
    finally
    {
      _isEditing = false;
    }

    _redos.Push(item);
    return true;
  }


  private readonly Stack<(IGoalEdit Undo, IGoalEdit Redo)> _undos = [];
  private readonly Stack<(IGoalEdit Undo, IGoalEdit Redo)> _redos = [];
  bool _isEditing;
}
