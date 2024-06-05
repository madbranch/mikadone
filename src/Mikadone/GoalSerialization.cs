using System.IO;
using System.Text.Json;

namespace Mikadone;

public class GoalSerialization : IGoalSerialization
{
  public Stream Serialize(Goal goal, Stream stream)
  {
    using Utf8JsonWriter writer = new(stream, WriterOptions);
    Serialize(goal, writer);
    return stream;
  }

  private static void Serialize(Goal goal, Utf8JsonWriter writer)
  {
    writer.WriteStartObject();
    writer.WriteBoolean("isReached", goal.IsReached);
    writer.WriteString("description", goal.Description);
    writer.WriteStartArray("prerequisites");
    foreach (Goal prerequisite in goal.Prerequisites)
    {
      Serialize(prerequisite, writer);
    }
    writer.WriteEndArray();
    writer.WriteEndObject();
  }

  private static readonly JsonWriterOptions WriterOptions = new() { Indented = true };
}