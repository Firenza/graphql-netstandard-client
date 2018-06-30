using System;
using System.Net;

namespace GraphQl.NetStandard.Client
{
    /// <summary>
    /// Thrown when a request is not successfully processed
    /// </summary>
    public class GraphQLRequestException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string ResponseBody { get; set; }

        public override string Message
        {
            get
            {
                return $"Request failed with status code of {HttpStatusCode.ToString()}";
            }
        }

        private GraphQLRequestException() { }

        public GraphQLRequestException(HttpStatusCode httpStatusCode, string responseBody) : base()
        {
            HttpStatusCode = httpStatusCode;
            ResponseBody = responseBody;
        }
    }
}
