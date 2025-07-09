using DK.EFootballClub.TeamDataUsvc.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DK.EFootballClub.TeamDataUsvc;

public class MongoDbService
{
    private readonly IMongoCollection<Team> _Teams;

    public MongoDbService(string? connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _Teams = database.GetCollection<Team>("Teams");
    }

    public async Task<List<Team>> GetAllTeamsAsync()
    {
        return await _Teams.Find(_ => true).ToListAsync();
    }

    private async Task<Team?> GetTeamByIdAsync(string id)
    {
        var filter = Builders<Team>.Filter.Eq("_id", ObjectId.Parse(id));
        return await _Teams.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Team?> CreateTeamAsync(Team team)
    {
        await _Teams.InsertOneAsync(team);
        return team;
    }

    public async Task<Team?> UpdateTeamAsync(string id, Team updatedTeam)
    {
        var filter = Builders<Team>.Filter.Eq("_id", ObjectId.Parse(id));
        var result = await _Teams.ReplaceOneAsync(filter, updatedTeam);

        if (result.ModifiedCount > 0)
        {
            return await GetTeamByIdAsync(id);
        }

        return null;
    }

    public async Task<bool> DeleteTeamAsync(string id)
    {
        var filter = Builders<Team>.Filter.Eq("_id", ObjectId.Parse(id));
        var result = await _Teams.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }
}