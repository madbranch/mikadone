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
    GoalFactory goalFactory = new(new GoalIdProvider(), goalEditing);
    Goal root = new(new GoalId(0), false, string.Empty, [], goalEditing, goalFactory);
    root.AddPrerequisite(new Goal( new GoalId(1), false, "Oh yeah", [], goalEditing, goalFactory));
    root.Prerequisites[0].AddPrerequisite(new Goal(new GoalId(2), false, "Meh", [], goalEditing, goalFactory));
    root.Prerequisites[0].AddPrerequisite(new Goal(new GoalId(3), false, "Huh", [], goalEditing, goalFactory) );
    root.AddPrerequisite(new Goal(new GoalId(4), true, "Oh no", [], goalEditing, goalFactory));

    GoalSerialization goalSerialization = new(goalFactory);

    using MemoryStream stream = new();
    
    goalSerialization.Serialize(root, stream);

    string json = Encoding.UTF8.GetString(stream.ToArray());

    Goal deserializedRoot = goalSerialization.Deserialize(json)!;

    deserializedRoot.Should().Be(root);
  }
}
