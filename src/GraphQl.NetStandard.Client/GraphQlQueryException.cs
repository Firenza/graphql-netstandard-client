using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace GraphQl.NetStandard.Client
{
    /// <summary>
    /// Thrown when a query is processed but has errors
    /// </summary>
    public class GraphQLQueryException : Exception
    {
        public List<string> ErrorMessages { get; set; }
        public HttpResponseHeaders ResponseHeaders { get; set; }


        public override string Message
        {
            get
            {
                if (ErrorMessages != null)
                {
                    return string.Join("|", ErrorMessages);
                }
                else
                {
                    return null;
                }
            }
        }

        private GraphQLQueryException() { }

        public GraphQLQueryException(IEnumerable<string> errorMessages, HttpResponseHeaders httpResponseHeaders) : base()
        {
            ErrorMessages = errorMessages.ToList();
            ResponseHeaders = httpResponseHeaders;
        }
    }
}
