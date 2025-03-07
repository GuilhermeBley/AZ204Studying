using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Driver;
using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure;
using System.Threading.Tasks;

namespace CosmosDb;

internal class GremlinApi
{
    private readonly static GremlinServer _server = new GremlinServer(
            Environment.GetEnvironmentVariable("COSMOSDB_URI") ?? throw new ArgumentException("Set the variable 'COSMOSDB_URI'."),
            443,
            enableSsl: true,
            username: Environment.GetEnvironmentVariable("COSMOSDB_USER") ?? throw new ArgumentException("Set the variable 'COSMOSDB_USER'."), // e.g., "/dbs/mydatabase/colls/mygraph"
            password: Environment.GetEnvironmentVariable("COSMOSDB_PASSWORD") ?? throw new ArgumentException("Set the variable 'COSMOSDB_PASSWORD'.")
        );

    public async Task SendMessage()
    {
        using var gremlinClient = new GremlinClient(_server);

        var g = AnonymousTraversalSource.Traversal().WithRemote(new DriverRemoteConnection(gremlinClient));

        var vertex = g.AddV("person").Property("name", "John Doe").AsString().Current;
        
        await gremlinClient.SubmitAsync<dynamic>(vertex ?? string.Empty);
    }

    public async Task<Vertex[]> GetPersonByNameAsync(string name)
    {
        using var gremlinClient = new GremlinClient(_server);

        var g = AnonymousTraversalSource.Traversal().WithRemote(new DriverRemoteConnection(gremlinClient));

        var query = await g.V().HasLabel("person").Has("name", name).Promise(e => e.ToList());

        return query.Where(e => e is not null).ToArray()!;
    }
}
