using MongoDB.Bson.Serialization.Attributes;

namespace DK.EFootballClub.TeamDataUsvc.Models;

public class TeamInLeague
    {
        [BsonElement("team_name")]
        public required string TeamName { get; set; }

        [BsonElement("matches_played")]
        public int MatchesPlayed { get; set; }

        [BsonElement("points")]
        public int Points { get; set; }
    }