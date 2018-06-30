# graphql-netstandard-client
[![Build status](https://ci.appveyor.com/api/projects/status/bmovyxbqfxjxkigd/branch/master?svg=true)](https://ci.appveyor.com/project/Firenza/graphql-netstandard-client/branch/master)
![AppVeyor tests](https://img.shields.io/appveyor/tests/Firenza/graphql-netstandard-client.svg)
[![Coverage Status](https://coveralls.io/repos/github/Firenza/graphql-netstandard-client/badge.svg)](https://coveralls.io/github/Firenza/graphql-netstandard-client)
![NuGet](https://img.shields.io/nuget/v/GraphQl.Netstandard.Client.svg)

A simple and testable GraphQL client for .NET and .NET core. Thank you to [bkniffler](https://github.com/bkniffler/graphql-net-client) for providing the template for some of this! 

To use reference the `GraphQl.NetStandard.Client` package on [NuGet](https://www.nuget.org/packages/GraphQl.NetStandard.Client/)

An example request to read some basic repository information from GitHub

The example assumes the `Repository` type is defined as it is [here](https://github.com/Firenza/graphql-netstandard-client/blob/2bca117a5c29a24c1a0aaea197cb0216015fd076/tests/GraphQL.NetStandard.Client.UnitTests/Model/Repository.cs)
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
query ($repoName: String!, $repoOwner: String!) {
  repository(name: $repoName, owner: $repoOwner) {
    pullRequests(first: 1) {
      nodes {
        baseRefName
        headRefName
        reviews(first: 100) {
          totalCount
          nodes {
            state
            author {
              login
            }
          }
          pageInfo {
            hasNextPage
            hasPreviousPage
            endCursor
            startCursor
          }
        }
      }
    }
  }
}
";

var variables = new { repoName = repoName, repoOwner = repoOwner }

// Have the client do the deserialization
var repository = await graphQLClient.QueryAsync<Repository>(query, variables);

// Get the raw response body string
var responseBodySTring = await graphQLClient.QueryAsync(query, variables);
```

Accomodating the way GraphQl returns collections can be difficult when deseriaizing into a .NET object.  To help with this there are the following classes included with the client

* `GraphQlNodesParent<T>`
* `GraphQlNodesParentConverter<T>`

You can use these when defining the DTOs you want to deserialize your GraphQl response into.  E.G. In the example code above the `Repository` DTO is defined as follows

```csharp
public class Repository
{
    [JsonConverter(typeof(GraphQlNodesParentConverter<PullRequest>))]
    public GraphQlNodesParent<PullRequest> PullRequests { get; set; }
}
```

This results in the `PullRequests` object having the following child properties populated which match the way GraphQl returns collections, which saves you from having to define this properties yourself for every collection.
* TotalCount
* PageInfo
* Nodes (A `List<PullRequest>` in this example)
