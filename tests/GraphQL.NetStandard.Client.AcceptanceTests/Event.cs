using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GraphQL.NetStandard.Client.AcceptanceTests
{
    [DataContract]
    internal class Event
    {
        [DataMember(Name = "address")]
        public string Address { get; set; }
        [DataMember(Name = "currency")]
        public string Currency { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set;  }
    }
}
