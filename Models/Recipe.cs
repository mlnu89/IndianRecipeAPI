using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IndianRecipeAPI.Models
{
    public class Recipe
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] // Convert ObjectId to string automatically
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string? Region { get; set; }
        public string? DietType { get; set; }
        public List<string> Ingredients { get; set; } = new();
        public string Instructions { get; set; } = string.Empty;
        public NutritionalInfo NutritionalInfo { get; set; } = new();
    }

    public class NutritionalInfo
    {
        public int Calories { get; set; }
        public int Protein { get; set; }
        public int Carbs { get; set; }
        public int Fat { get; set; }
    }
}
