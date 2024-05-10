using System.IO;
using System.Text;
using FluentAssertions;

namespace Mikadone;

public class GoalTests
{
  [Fact]
  public void Serialization()
  {
    Goal root = new(new GoalId(0), false, string.Empty, []);
    root.AddPrerequisite(new Goal( new GoalId(1), false, "Oh yeah", []));
    root.Prerequisites[0].AddPrerequisite(new Goal(new GoalId(2), false, "Meh", []));
    root.Prerequisites[0].AddPrerequisite(new Goal(new GoalId(3), false, "Huh", []) );
    root.AddPrerequisite(new Goal(new GoalId(4), true, "Oh no", []));

    GoalSerialization goalSerialization = new(new GoalFactory(new GoalIdProvider()));

    using MemoryStream stream = new();
    
    goalSerialization.Serialize(root, stream);

    string json = Encoding.UTF8.GetString(stream.ToArray());

    Goal deserializedRoot = goalSerialization.Deserialize(json)!;

    deserializedRoot.Should().Be(root);
  }
}
