using CosmosDb;
using DotNetEnv;

Env.Load();

var api = new GremlinApi();

await api.SendMessage();