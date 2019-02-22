# TvMazeScraper

ASP.NET Core 2.2 API based off the [TvMaze API](http://www.tvmaze.com/api) that provides show and casting information with storage in [Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/).

To get the application up-and-running, create a Cosmos DB account and provide the following environment settings:

- Cosmos:Endpoint (e.g.: `https://tvmazescraper.documents.azure.com:443/`)
- Cosmos:Database (e.g.: `TvMaze`)
- Cosmos:Container (e.g.: `Shows`)
- Cosmos:PrimaryKey (primary connection key for accessing your Cosmos DB acount)

A version of this application is running on [http://tvmazeapi.cfapps.io/api/shows](http://tvmazeapi.cfapps.io/api/shows).


# Architecture
The application uses two `IHostedService` implementations to run two background jobs:
- querying the [show updates](http://www.tvmaze.com/api#show-updates) endpoint every 30 minutes
- picking show create/update requests from a queue every 100ms to create/update a show in Azure Cosmos DB

The application queries the [show updates](http://www.tvmaze.com/api#show-updates) endpoint to determine latest update timestamps of shows. The application then retrieves all current shows from the Azure Cosmos DB (if there are any).

For every show update we check whether the show is already inside Azure Cosmos DB:
- if not, post a message to an internal queue that [show details](http://www.tvmaze.com/api#show-main-information) need to be fetched from the TvMaze API
- if the show already exists inside Cosmos DB, compare updated timestamps and if a show is outdated, fetch it from the TvMaze API and update in Cosmos DB