# graphql-netstandard-client
[![Build status](https://ci.appveyor.com/api/projects/status/bmovyxbqfxjxkigd/branch/master?svg=true)](https://ci.appveyor.com/project/Firenza/graphql-netstandard-client/branch/master)

A simple and testable GraphQL client for .NET and .NET core. Thank you to [bkniffler](https://github.com/bkniffler/graphql-net-client) for providing the template for some of this! 

To use reference the `GraphQl.NetStandard.Client` package on [NuGet](https://www.nuget.org/packages/GraphQl.NetStandard.Client/)

An example request to read some basic repository information from GitHub
```csharp
var repoName = "";
var repoOwner = "";
var gitHubAuthToken = "";
var githubGraphQLApiUrl = "https://api.github.com/graphql";

// These are added on each request so you can safely use an HttpClient instance that is 
// shared across your application
var requestHeaders = new NameValueCollection();
requestHeaders.Add( "Authorization", $"Bearer {gitHubAuthToken}");
requestHeaders.Add( "User-Agent", "graphql-netstandard-client" );

IGraphQLClient graphQLClient = new GraphQLClient(new HttpClient(), githubGraphQLApiUrl, requestHeaders);

var query = @"
query ($repoName:String!, $repoOwner:String!){
  repository(name: $repoName, owner: $repoOwner) {
    pushedAt,
    createdAt,
    defaultBranchRef {
      name
    }
    repositoryTopics(first: 10) {
      nodes {
        topic{
          name
        }
      }
    }
  }
}
";

var variables = new { repoName = repoName, repoOwner = repoOwner }

var responseBodyString = await graphQLClient.QueryAsync(query, variables);

// Use Newtonsoft JSON library to get at response data
dynamic dynamicJObject = JObject.Parse(responseBodyString);
Console.WriteLine(dynamicJObject.data.repository.pushedAt.Value<DateTime>());

// Of if dynamics aren't your cup of tea
jObject = JObject.Parse(responseBodyString);
Console.WriteLine(jObject["data"]["repository"]["pushedAt"].Value<DateTime>());
```
