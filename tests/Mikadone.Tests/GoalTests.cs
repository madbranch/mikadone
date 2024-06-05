using System.IO;
using System.Text;
using FluentAssertions;
using Mikadone.GoalEdit;
using NSubstitute;

namespace Mikadone;

public class GoalTests
{
  [Fact]
  public void Serialization()
  {
    IGoalEditing goalEditing = Substitute.For<IGoalEditing>();
    IGoalAutoSave goalAutoSave = Substitute.For<IGoalAutoSave>();
    GoalFactory goalFactory = new(new GoalIdProvider(), goalEditing, goalAutoSave);
    Goal root = new(new GoalId(0), false, string.Empty, [], goalEditing, goalAutoSave);
    root.AddPrerequisite(new Goal( new GoalId(1), false, "Oh yeah", [], goalEditing, goalAutoSave));
    root.Prerequisites[0].AddPrerequisite(new Goal(new GoalId(2), false, "Meh", [], goalEditing, goalAutoSave));
    root.Prerequisites[0].AddPrerequisite(new Goal(new GoalId(3), false, "Huh", [], goalEditing, goalAutoSave) );
    root.AddPrerequisite(new Goal(new GoalId(4), true, "Oh no", [], goalEditing, goalAutoSave));

    GoalSerialization goalSerialization = new();
    GoalDeserialization goalDeserialization = new(goalFactory);

    using MemoryStream stream = new();
    
    goalSerialization.Serialize(root, stream);

    string json = Encoding.UTF8.GetString(stream.ToArray());

    Goal deserializedRoot = goalDeserialization.Deserialize(json)!;

    deserializedRoot.Should().Be(root);
  }
}
