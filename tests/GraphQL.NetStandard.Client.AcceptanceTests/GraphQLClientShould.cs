using FluentAssertions;
using GraphQl.NetStandard.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GraphQL.NetStandard.Client.AcceptanceTests
{
    [TestClass]
    public class GraphQLClientShould
    {
        private static GraphQLClient graphQLClient = new GraphQLClient(new HttpClient(), "https://www.universe.com/graphql");

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        public async Task ReturnDataFromTestAPI_NoVariablesSerializeToType()
        {
            // Arrange
            var query = @"
                query {
                    event(id: ""5879ad8f6672e70036d58ba5"") {
                        title
                        address
                        currency
                    } 
                }
            ";

            // Act
            var response = await graphQLClient.QueryAsync<Event>(query);

            // Assert
            response.Title.Should().NotBeNullOrWhiteSpace();
            response.Address.Should().NotBeNullOrWhiteSpace();
            response.Currency.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        public async Task ReturnDataFromTestAPI_NoVariablesStringContent()
        {
            // Arrange
            var query = @"
                query {
                    event(id: ""5879ad8f6672e70036d58ba5"") {
                        title
                        address
                        currency
                    } 
                }
            ";

            // Act
            var response = await graphQLClient.QueryAsync(query);

            // Assert
            var jObject = JObject.Parse(response);

            var address = jObject["data"]["event"]["address"].Value<string>();
            address.Should().NotBeNullOrWhiteSpace();

            var currency = jObject["data"]["event"]["currency"].Value<string>();
            currency.Should().NotBeNullOrWhiteSpace();

            var title = jObject["data"]["event"]["title"].Value<string>();
            title.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        public async Task ReturnDataFromTestAPI_VariablesSerializeToType()
        {
            // Arrange
            var query = @"
                query($eventId:ID!) {
                    event(id: $eventId) {
                        title
                        address
                        currency
                    } 
                }
            ";

            var variables = new { eventId = "5879ad8f6672e70036d58ba5" };

            // Act
            var response = await graphQLClient.QueryAsync<Event>(query, variables);

            // Assert
            response.Title.Should().NotBeNullOrWhiteSpace();
            response.Address.Should().NotBeNullOrWhiteSpace();
            response.Currency.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        public async Task ReturnDataFromTestAPI_VariablesStringContent()
        {
            // Arrange
            var query = @"
                query($eventId:ID!) {
                    event(id: $eventId) {
                        title
                        address
                        currency
                    } 
                }
            ";

            var variables = new { eventId = "5879ad8f6672e70036d58ba5" };

            // Act
            var response = await graphQLClient.QueryAsync(query, variables);

            // Assert
            var jObject = JObject.Parse(response);

            var address = jObject["data"]["event"]["address"].Value<string>();
            address.Should().NotBeNullOrWhiteSpace();

            var currency = jObject["data"]["event"]["currency"].Value<string>();
            currency.Should().NotBeNullOrWhiteSpace();

            var title = jObject["data"]["event"]["title"].Value<string>();
            title.Should().NotBeNullOrWhiteSpace();
        }
    }
}
