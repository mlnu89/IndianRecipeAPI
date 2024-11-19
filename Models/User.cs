namespace IndianRecipeAPI.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("Username")]
    public required string Username { get; set; }

    [BsonElement("Password")]
    public string? Password { get; set; }

    [BsonElement("DietaryPreference")]
    public string? DietaryPreference { get; set; }
}
