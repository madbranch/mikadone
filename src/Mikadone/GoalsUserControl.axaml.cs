using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace Mikadone;

public partial class GoalsUserControl : UserControl
{
  public GoalsUserControl()
  {
    InitializeComponent();

    // We connect to the KeyDownEvent in code here to handle it when it's tunneling.
    // Connecting to it in the XAML would handle it when it's bubbling.
    GoalsList.AddHandler(KeyDownEvent, GoalsList_KeyDown, RoutingStrategies.Tunnel);
  }

  private void UserControl_KeyDown(object? sender, KeyEventArgs e)
  {
    if (DataContext is not GoalsViewModel viewModel)
    {
      return;
    }

    if (e.Key == Key.Z)
    {
      bool isMacOs = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

      KeyModifiers osKeyModifier = isMacOs ? KeyModifiers.Meta : KeyModifiers.Control;

      if (e.KeyModifiers == osKeyModifier)
      {
        viewModel.UndoCommand.Execute(null);
      }
      else if (e.KeyModifiers == (osKeyModifier | KeyModifiers.Shift))
      {
        viewModel.RedoCommand.Execute(null);
      }
    }
  }

  private void GoalsList_KeyDown(object? sender, KeyEventArgs e)
  {
    if (DataContext is not GoalsViewModel viewModel || viewModel.SelectedGoal is not Goal selectedGoal)
    {
      return;
    }

    switch (e.Key)
    {
      case Key.Space:
      {
        selectedGoal.IsReached = !selectedGoal.IsReached;
        e.Handled = true;
        break;
      }
      case Key.F2:
      {
        selectedGoal.BeginEdit();
        e.Handled = true;
        break;
      }
      case Key.Enter:
      {
        if (selectedGoal.IsEditing == true)
        {
          // We do nothing, we let the TextBox.KeyDown handle it.
          return;
        }

        if (e.KeyModifiers == KeyModifiers.Shift)
        {
          viewModel.AddNewPrerequisiteCommand.Execute("New task");
          e.Handled = true;
        }
        else
        {
          viewModel.AddNewSiblingPrerequisiteCommand.Execute("New task");
          e.Handled = true;
        }
        break;
      }
      case Key.Tab:
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
        break;
      }
    }
  }

  private void TextBox_KeyDown(object? sender, KeyEventArgs e)
  {
    if (sender is not TextBox textBox
      || textBox.DataContext is not Goal goal)
    {
      return;
    }

    // We want to handle the EndEdit() and CancelEdit() here to be
    // able to focus the TreeViewItem correctly to make arrows still
    // usable.

    if (e.Key == Key.Enter)
    {
      goal.EndEdit();
      textBox.GetLogicalAncestors().OfType<TreeViewItem>().FirstOrDefault()?.Focus();
      e.Handled = true;
    }
    else if (e.Key == Key.Escape)
    {
      goal.CancelEdit();
      textBox.GetLogicalAncestors().OfType<TreeViewItem>().FirstOrDefault()?.Focus();
      e.Handled = true;
    }
  }

  private void TextBox_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
  {
    if (sender is not TextBox textBox
      || !textBox.IsVisible)
    {
      return;
    }

    // When editing a new item, we don't get a PropertyChanged on the IsVisible property
    // so we react on the item getting attached to the visual tree.

    textBox.Focus();
    textBox.SelectAll();
  }

  private void TextBox_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
  {
    if (e.Property != IsVisibleProperty
      || sender is not TextBox textBox
      || !textBox.IsVisible)
    {
      return;
    }

    // When editing an existing item, we can simply react on the IsVisible
    // property of the TextBox being set to true.

    textBox.Focus();
    textBox.SelectAll();
  }
}