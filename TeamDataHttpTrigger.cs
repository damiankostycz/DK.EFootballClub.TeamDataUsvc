using System.Net;
using System.Text.Json;
using DK.EFootballClub.TeamDataUsvc.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace DK.EFootballClub.TeamDataUsvc;

public class TeamDataHttpTrigger(ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<TeamDataHttpTrigger>();
    private readonly string? _dbConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");

    [Function("GetAllTeams")]
    public async Task<HttpResponseData> GetAllPlayers(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "teams")] HttpRequestData req)
    {
        var response = req.CreateResponse();
        try
        {
            var db = new MongoDbService(_dbConnectionString, "teams_coaches_players_db");
            var players = await db.GetAllTeamsAsync();
            await response.WriteAsJsonAsync(players);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching teams");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Internal server error");
            return response;
        }
    }

    [Function("CreateTeam")]
    public async Task<HttpResponseData> CreatePlayer(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "teams")] HttpRequestData req)
    {
        var response = req.CreateResponse();

        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var team = JsonSerializer.Deserialize<Team>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (team == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid player data.");
                return response;
            }

            var db = new MongoDbService(_dbConnectionString, "teams_coaches_players_db");
            var createdTeam = await db.CreateTeamAsync(team);

            response.StatusCode = HttpStatusCode.Created;
            response.Headers.Add("Location", $"/api/team/{createdTeam!.TeamId}");
            await response.WriteAsJsonAsync(createdTeam);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating team");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Internal server error");
            return response;
        }
    }

    [Function("UpdateTeam")]
    public async Task<HttpResponseData> UpdateTeamr(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "teams/{id}")] HttpRequestData req,
        string id)
    {
        var response = req.CreateResponse();

        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedTeamData = JsonSerializer.Deserialize<Team>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (updatedTeamData == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid player data.");
                return response;
            }

            var db = new MongoDbService(_dbConnectionString, "teams_coaches_players_db");
            var updatedTeam = await db.UpdateTeamAsync(id, updatedTeamData);

            if (updatedTeam == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                await response.WriteStringAsync($"Team with ID {id} not found.");
                return response;
            }

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteAsJsonAsync(updatedTeam);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating team with ID {id}");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Internal server error");
            return response;
        }
    }

    [Function("DeleteTeam")]
    public async Task<HttpResponseData> DeleteTeam(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "teams/{id}")] HttpRequestData req,
        string id)
    {
        var response = req.CreateResponse();

        try
        {
            var db = new MongoDbService(_dbConnectionString, "teams_coaches_players_db");
            var success = await db.DeleteTeamAsync(id);

            if (!success)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                await response.WriteStringAsync($"Team with ID {id} not found.");
                return response;
            }

            response.StatusCode = HttpStatusCode.NoContent;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting team with ID {id}");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Internal server error");
            return response;
        }
    }
}