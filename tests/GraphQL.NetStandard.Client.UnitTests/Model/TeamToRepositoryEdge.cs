using System.Runtime.Serialization;

namespace GraphQL.NetStandard.Client.UnitTests.Model
{
    public class TeamToRepositoryEdge
    {
        public string Permission { get; set; }

        public Repository Repository { get; set; }
    }
}
