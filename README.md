# graphql-netstandard-client
[![Build status](https://ci.appveyor.com/api/projects/status/bmovyxbqfxjxkigd/branch/master?svg=true)](https://ci.appveyor.com/project/Firenza/graphql-netstandard-client/branch/master)
![AppVeyor tests](https://img.shields.io/appveyor/tests/Firenza/graphql-netstandard-client.svg)
[![Coverage Status](https://coveralls.io/repos/github/Firenza/graphql-netstandard-client/badge.svg)](https://coveralls.io/github/Firenza/graphql-netstandard-client)
![NuGet](https://img.shields.io/nuget/v/GraphQl.Netstandard.Client.svg)

A simple and testable GraphQL client for .NET and .NET core. Thank you to [bkniffler](https://github.com/bkniffler/graphql-net-client) for providing the template for some of this! 

Now supports [Source Link](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/sourcelink) for easy debugging!

To use reference the `GraphQl.NetStandard.Client` package on [NuGet](https://www.nuget.org/packages/GraphQl.NetStandard.Client/)

An example request to read some basic repository information from GitHub

The example assumes the `Repository` type is defined as it is [here](https://github.com/Firenza/graphql-netstandard-client/blob/2bca117a5c29a24c1a0aaea197cb0216015fd076/tests/GraphQL.NetStandard.Client.UnitTests/Model/Repository.cs)
```csharp
var login = "";
var gitHubAuthToken = "";
var githubGraphQLApiUrl = "https://api.github.com/graphql";

// These are added on each request so you can safely use an HttpClient instance that is 
// shared across your application
var requestHeaders = new NameValueCollection();
requestHeaders.Add( "Authorization", $"Bearer {gitHubAuthToken}");
requestHeaders.Add( "User-Agent", "graphql-netstandard-client" );

IGraphQLClient graphQLClient = new GraphQLClient(new HttpClient(), githubGraphQLApiUrl, requestHeaders);

var query = @"
query ($login: String!) {
  organization(login: $login) {
    teams(first: 2) {
      nodes {        
        name
        // Use this object if you just want the list of child objects
        repositories(first:1){
          nodes{
            name
          }
        }
        // Use this object if you want the relationship information between parent and children
        repositoryEdges: repositories(first: 1) {
          edges {
            permission
            repository: node{
              name
            }
          }
        }
      }
      pageInfo {
        endCursor
        hasNextPage
      }
    }
  }
}
";

var variables = new { login = login}

// Have the client do the deserialization
var repository = await graphQLClient.QueryAsync<Repository>(query, variables);

// Get the raw response body string
var responseBodySTring = await graphQLClient.QueryAsync(query, variables);
```





### Handling response collections

Accomodating the way GraphQl returns collections (nodes and edges) can be difficult when deserializing into a .NET object.  To accomplish this with this client
see the sections below

#### Nodes

You can use the 
[`GraphQlNodesParent<T>`](https://github.com/Firenza/graphql-netstandard-client/blob/2bca117a5c29a24c1a0aaea197cb0216015fd076/src/GraphQl.NetStandard.Client/GraphQLNodesParent.cs) 
and [`GraphQlNodesParentConverter<T>`](https://github.com/Firenza/graphql-netstandard-client/blob/2bca117a5c29a24c1a0aaea197cb0216015fd076/src/GraphQl.NetStandard.Client/GraphQLNodesParentConverter.cs)
classes when defining the DTOs you want to deserialize your GraphQl response nodes into.  E.G. In the example code above the `Team` DTO is defined as follows

```csharp
public class Team
{
    [JsonConverter(typeof(GraphQlNodesParentConverter<Repository>))]
    public GraphQlNodesParent<Repository> Repositories { get; set; }
}
```

This results in the `Repositories` object having the following child properties populated which match the way GraphQl returns collections, which saves you from having to define this properties yourself for every collection.
* TotalCount
* PageInfo
* Nodes (A `List<Repository>` in this example)

#### Edges

Edges are a little more complicated as you don't just have a list of child objects, you have an object inbetween describing the relationship between
the parent and child objects.

First create a class representing this in between edge object.  The following class can be used to get the edge information 
for the Team -> Repository relationship from the GitHub GraphQL API.

```csharp
public class TeamToRepositoryEdge
{
    public string Permission { get; set; }
    public Repository Repository { get; set; }
}
```


Then you can use this class with the
[`GraphQlEdgesParent<T>`]() 
and [`GraphQlEdgesParentConverter<T>`]()
classes to describe the list of edges you want to deserialize  E.G. In the example code above the `Team` DTO is defined as follows

```csharp
public class Team
{
    [JsonConverter(typeof(GraphQlEdgesParentConverter<TeamToRepositoryEdge>))]
    // This property name must match an aliased GraphQl node in your query 
    // (I.E. you'll have alias the `edge` node to match the name of this property)
    public GraphQLEdgesParent<TeamToRepositoryEdge> RepositoryEdges { get; set; }
}
```

This results in the `RepositoryEdges` object having the following child properties.
* TotalCount
* PageInfo
* Edges (A `List<TeamToRepositoryEdge>` in this example)