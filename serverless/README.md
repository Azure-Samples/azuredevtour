# Serverless (Functions and Logic Apps)

* What is Serverless?
    * No infrastructure, focus on code
    * Pay for what you use (cheap!)
    * Infinite scale
    * Responds to events/event-driven

* Why Azure Functions?
    * Powerful triggers and bindings
    * HTTP is a first class citizen, no API gateways
    * Best in class tooling
    * Open source

Create a local.settings.json in the root of the solution.
```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
    "Url": "url-of-cosmosdb-account",
    "Database": "name-of-cosmosdb-database",
    "user": "mongo-db-username",
    "Password": "mongo-db-password",
    "Collection": "database-collection-name",
    "ResultsQueue": "storage-account-queue",
  }
}
```

To run or debug, press f5. Function will execute after 5 minutes.