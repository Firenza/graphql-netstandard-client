using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.NetStandard.Client.UnitTests.Model
{
    public class Review
    {
        public string State { get; set; }
        public Author Author { get; set; }
    }
}
