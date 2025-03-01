using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Driver;
using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure;
using System.Threading.Tasks;

namespace CosmosDb;

internal class GremiliApi
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

        var vertex = await g.AddV("person").Property("name", "John Doe").NextAsync();
        
        // Query vertices
        var people = await g.V().HasLabel("person").ToListAsync();
        foreach (var person in people)
        {
            Console.WriteLine($"Person: {person["name"]}");
        }

        // Add an edge
        var vertex2 = await g.AddV("person").Property("name", "Jane Doe").NextAsync();
        await g.V(vertex.Id).AddE("knows").To(g.V(vertex2.Id)).NextAsync();
        Console.WriteLine("Added edge between John Doe and Jane Doe");

        // Query edges
        var edges = await g.V(vertex.Id).OutE("knows").ToListAsync();
        foreach (var edge in edges)
        {
            Console.WriteLine($"Edge: {edge.Id}");
        }

        // Close the client
        gremlinClient.Dispose();
    }
}
