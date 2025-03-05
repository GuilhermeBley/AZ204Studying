## How to create the azure resource

```bash
rg="gremlin-test" \
location="eastus" \
accountName="cosmosdbaccountgremlin-1293193012" \
databaseName="graphdatabase-1293193012" \
```

```bash
az group create --name $rg --location $location
```

Create Cosmos DB Account with Gremlin API
```bash
az cosmosdb create \
    --name $accountName \
    --resource-group $rg \
    --kind GlobalDocumentDB \
    --capabilities EnableGremlin \
    --locations regionName=$location \
    --default-consistency-level Session
```

Create a database
```bash
az cosmosdb gremlin database create \
    --account-name $accountName \
    --resource-group $rg \
    --name $databaseName
```

Get Gremlin Endpoint and Primary Key
```bash
az config set clients.show_secrets_warning=no
gremlinEndpoint=$(az cosmosdb show \
    --name $accountName \
    --resource-group $rg \
    --query "documentEndpoint" -o tsv)

primaryKey=$(az cosmosdb keys list \
    --name $accountName \
    --resource-group $rg \
    --type keys \
    --query "primaryMasterKey" -o tsv)
echo "COSMOSDB_URI: $gremlinEndpoint"
echo "COSMOSDB_PASSWORD: $primaryKey"
echo "COSMOSDB_USER: /dbs/$databaseName/colls/your-graph-name"
```