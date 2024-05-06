using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Mikadone;

public class GoalSerialization : IGoalSerialization
{
  public GoalSerialization(IGoalFactory goalFactory)
    => _goalFactory = goalFactory;

  public Stream Serialize(Goal goal, Stream stream)
  {
    using Utf8JsonWriter writer = new(stream, WriterOptions);
    Serialize(goal, writer);
    return stream;
  }

  public Goal? Deserialize(string jsonString)
    => JsonNode.Parse(jsonString) is not JsonObject rootNode
      ? null
      : Deserialize(rootNode);

  private Goal Deserialize(JsonObject node)
    => _goalFactory.CreateGoal(GetIsReached(node), GetDescription(node), GetPrerequisites(node));

  private static bool GetIsReached(JsonObject node)
    => node["isReached"] is JsonValue isReachedValue && isReachedValue.GetValueKind() == JsonValueKind.True;
  
  private static string GetDescription(JsonObject node)
    => node["description"] is JsonValue descriptionValue && descriptionValue.TryGetValue(out string? description)
    ? description
    : string.Empty;
  
  private IEnumerable<Goal> GetPrerequisites(JsonObject node)
    => node["prerequisites"] is JsonArray prerequisitesArray
    ? prerequisitesArray.OfType<JsonObject>().Select(Deserialize)
    : [];

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
  private static readonly JsonReaderOptions ReaderOptions = new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip };
  private readonly IGoalFactory _goalFactory;

}