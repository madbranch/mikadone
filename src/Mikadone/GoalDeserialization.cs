using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Mikadone;

public class GoalDeserialization : IGoalDeserialization
{
  private readonly IGoalFactory _goalFactory;

  public GoalDeserialization(IGoalFactory goalFactory)
    => _goalFactory = goalFactory;

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
}
