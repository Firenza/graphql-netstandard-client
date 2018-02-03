# graphql-netstandard-client
A simple and testable GraphQL client for .NET and .NET core

To use reference the `GraphQl.NetStandard.Client` on NuGet

An example request to read some basic repository information from GitHub
```csharp
var repoName = "";
var repoOwner = "";
var gitHubAuthToken = "";
var githubGraphQLApiUrl = "https://api.github.com/graphql";

var requestHeaders = new List<KeyValuePair<string, string>>
{
    new KeyValuePair<string,string>("Authorization", gitHubAuthToken),
    new KeyValuePair<string,string>("User-Agent", "graphql-netstandard-client")
};

IGraphQLClient graphQLClient = new GraphQLClient(githubGraphQLApiUrl, requestHeaders);

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

// Use Newtonsoft JSON library to get at reponse data
dynamic dynamicJObject = JObject.Parse(responseBodyString);
Console.WriteLine(dynamicJObject.data.repository.pushedAt.Value<DateTime>());

// Of if dynamics aren't your cup of tea
jObject = JObject.Parse(responseBodyString);
Console.WriteLine(jObject["data"]["repository"]["pushedAt"].Value<DateTime>());
```