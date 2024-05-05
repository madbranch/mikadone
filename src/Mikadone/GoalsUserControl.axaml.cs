using Avalonia.Controls;
using Avalonia.Input;

namespace Mikadone;

public partial class GoalsUserControl : UserControl
{
  public GoalsUserControl() => InitializeComponent();

  private void TreeView_KeyDown(object? sender, KeyEventArgs e)
  {
    if (DataContext is not GoalsViewModel viewModel)
    {
      return;
    }

    if (viewModel.SelectedGoal is not Goal selectedGoal)
    {
      return;
    }

    if (e.Key == Key.Space)
    {
      selectedGoal.IsReached = !selectedGoal.IsReached;
      e.Handled = true;
    }
    if (e.Key == Key.F2)
    {
      selectedGoal.BeginEdit();
      e.Handled = true;
    }
    else if (e.Key == Key.Enter)
    {
      if (selectedGoal.IsEditing == true)
      {
        selectedGoal.EndEdit();
        e.Handled = true;
      }
      else if (e.KeyModifiers == KeyModifiers.Shift)
      {
        // todo: add prerequisite
        e.Handled = true;
      }
      else
      {
        // todo: add sibling
        e.Handled = true;
      }
    }
    else if (e.Key == Key.Escape)
    {
      selectedGoal.CancelEdit();
      e.Handled = true;
    }
    else if (e.Key == Key.Tab)
    {
      if (e.KeyModifiers == KeyModifiers.Shift)
      {
        // todo: move up one level
        e.Handled = true;
      }
      else
      {
        // todo: move down one level
        e.Handled = true;
      }
    }
  }
}