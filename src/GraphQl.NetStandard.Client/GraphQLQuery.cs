using System.Runtime.Serialization;

namespace GraphQl.NetStandard.Client
{
    [DataContract]
    internal class GraphQLQuery
    {
        [DataMember(Name = "query")]
        public string Query { get; set; }
        [DataMember(Name = "variables")]
        public object Variables { get; set; }
    }
}
