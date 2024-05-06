using System;
using System.IO;
using System.Text;
using System.Text.Json;
using FluentAssertions;

namespace Mikadone;

public class GoalTests
{
  [Fact]
  public void Serialization()
  {
    Goal root = new(0);
    root.AddPrerequisite(new Goal(1) { Description = "Oh yeah" });
    root.Prerequisites[0].AddPrerequisite(new Goal(2) { Description = "Meh" });
    root.Prerequisites[0].AddPrerequisite(new Goal(3) { Description = "Huh" });
    root.AddPrerequisite(new Goal(4) { IsReached = true, Description = "Oh no" });

    GoalSerialization goalSerialization = new(new GoalFactory(new GoalIdProvider()));

    using MemoryStream stream = new();
    
    goalSerialization.Serialize(root, stream);

    string json = Encoding.UTF8.GetString(stream.ToArray());

    Goal deserializedRoot = goalSerialization.Deserialize(json)!;

    deserializedRoot.Should().Be(root);
  }
}
