using GraphQl.NetStandard.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GraphQL.NetStandard.Client.UnitTests.Model
{
   
    public class Team
    {
        public string Name { get; set; }

        [JsonConverter(typeof(GraphQlNodesParentConverter<Repository>))]
        public GraphQlNodesParent<Repository> Repositories { get; set; }

        [JsonConverter(typeof(GraphQlEdgesParentConverter<TeamToRepositoryEdge>))]
        public GraphQLEdgesParent<TeamToRepositoryEdge> RepositoryEdges { get; set; }
    }
}
