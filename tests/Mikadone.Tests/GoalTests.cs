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
    Goal root = new();
    root.AddPrerequisite(new Goal() { Description = "Oh yeah" });
    root.Prerequisites[0].AddPrerequisite(new Goal() { Description = "Meh" });
    root.Prerequisites[0].AddPrerequisite(new Goal() { Description = "Huh" });
    root.AddPrerequisite(new Goal() { IsReached = true, Description = "Oh no" });

    GoalSerialization goalSerialization = new();

    using MemoryStream stream = new();
    
    goalSerialization.Serialize(root, stream);

    string json = Encoding.UTF8.GetString(stream.ToArray());

    Goal deserializedRoot = goalSerialization.Deserialize(json)!;

    deserializedRoot.Should().Be(root);
  }
}
