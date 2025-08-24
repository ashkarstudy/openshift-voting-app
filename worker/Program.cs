using StackExchange.Redis;
using Npgsql;
using Newtonsoft.Json;

var redis = ConnectionMultiplexer.Connect("redis:6379");
var db = redis.GetDatabase();

var connectionString = $"Host=postgres;Port=5432;Database={Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "postgres"};Username={Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres"};Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres"}";

using var connection = new NpgsqlConnection(connectionString);
await connection.OpenAsync();

var createTableCmd = new NpgsqlCommand(@"
    CREATE TABLE IF NOT EXISTS votes (
        id SERIAL PRIMARY KEY,
        voter_id VARCHAR(255) UNIQUE,
        vote VARCHAR(255),
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    )", connection);
await createTableCmd.ExecuteNonQueryAsync();

Console.WriteLine("Worker service started. Processing votes...");

while (true)
{
    try
    {
        var voteJson = await db.ListLeftPopAsync("votes");
        if (voteJson.HasValue)
        {
            var voteData = JsonConvert.DeserializeObject<VoteData>(voteJson);
            var insertCmd = new NpgsqlCommand(@"
                INSERT INTO votes (voter_id, vote) 
                VALUES (@voter_id, @vote) 
                ON CONFLICT (voter_id) 
                DO UPDATE SET vote = @vote, created_at = CURRENT_TIMESTAMP", connection);
            
            insertCmd.Parameters.AddWithValue("voter_id", voteData.VoterId);
            insertCmd.Parameters.AddWithValue("vote", voteData.Vote);
            
            await insertCmd.ExecuteNonQueryAsync();
            Console.WriteLine($"Processed vote: {voteData.Vote} from {voteData.VoterId}");
        }
        else
        {
            await Task.Delay(1000);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error processing vote: {ex.Message}");
        await Task.Delay(5000);
    }
}

public class VoteData
{
    [JsonProperty("voter_id")]
    public string VoterId { get; set; } = "";
    
    [JsonProperty("vote")]
    public string Vote { get; set; } = "";
}
