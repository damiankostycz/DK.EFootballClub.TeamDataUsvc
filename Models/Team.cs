using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DK.EFootballClub.TeamDataUsvc.Models;
public class Team
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string TeamId { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("league")]
    public List<TeamInLeague>? League { get; set; }
}