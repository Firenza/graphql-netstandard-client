using FluentAssertions;
using GraphQl.NetStandard.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.NetStandard.Client.UnitTests
{
    [TestClass]
    public class GraphQlClientShould
    {
        [TestMethod]
        public async Task SendRequestsToProvidedUrl()
        {
            //Arrange
            var mockHandler = new Mock<HttpMessageHandlerWrapper> { CallBase = true };
            var httpClient = new HttpClient(mockHandler.Object);
            var url = "http://test";
            var graphQlClient = new GraphQLClient(httpClient, url);

            mockHandler
                .Setup(mock => mock.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage());

            //Act
            await graphQlClient.QueryAsync("query", new {x = 1, y=2 });

            //Assert
            mockHandler
                .Verify(mock => mock.Send(It.Is<HttpRequestMessage>(value => value.RequestUri == new Uri(url))));
        }

        [TestMethod]
        public async Task SendRequestsUsingPost()
        {
            //Arrange
            var mockHandler = new Mock<HttpMessageHandlerWrapper> { CallBase = true };
            var httpClient = new HttpClient(mockHandler.Object);
            var url = "http://test";
            var graphQlClient = new GraphQLClient(httpClient, url);

            mockHandler
                .Setup(mock => mock.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage());

            //Act
            await graphQlClient.QueryAsync("query", new { x = 1, y = 2 });

            //Assert
            mockHandler
                .Verify(mock => mock.Send(It.Is<HttpRequestMessage>(value => value.Method == HttpMethod.Post)));
        }

        [TestMethod]
        public async Task SendRequestsWithProperQueryContent()
        {
            //Arrange
            var mockHandler = new Mock<HttpMessageHandlerWrapper> { CallBase = true };
            var httpClient = new HttpClient(mockHandler.Object);
            var url = "http://test";
            var graphQlClient = new GraphQLClient(httpClient, url);

            mockHandler
                .Setup(mock => mock.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage());

            var query = "QUERY";
            var expectedJsonContent = "{\"query\":\"QUERY\",\"variables\":null}";
            var expectedStringContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            

            //Act
            await graphQlClient.QueryAsync(query);

            //Assert
            mockHandler
                .Verify(mock => mock.Send(It.Is<HttpRequestMessage>(value => TestHelpers.AreEqual(value.Content, expectedStringContent))));

            mockHandler
                .Verify(mock => mock.Send(It.Is<HttpRequestMessage>(value => TestHelpers.AreEqual(value.Content.ReadAsStringAsync().Result, expectedJsonContent))));
        }

        [TestMethod]
        public async Task SendRequestsWithProperQueryAndVariablesContent()
        {
            //Arrange
            var mockHandler = new Mock<HttpMessageHandlerWrapper> { CallBase = true };
            var httpClient = new HttpClient(mockHandler.Object);
            var url = "http://test";
            var graphQlClient = new GraphQLClient(httpClient, url);

            mockHandler
                .Setup(mock => mock.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage());

            var query = "QUERY";
            var varibles = new { var1 = "test1", var2 = "test2" };
            var expectedJsonContent = "{\"query\":\"QUERY\",\"variables\":{\"var1\":\"test1\",\"var2\":\"test2\"}}";
            var expectedStringContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");


            //Act
            await graphQlClient.QueryAsync(query, varibles);

            //Assert
            mockHandler
                .Verify(mock => mock.Send(It.Is<HttpRequestMessage>(value => TestHelpers.AreEqual(value.Content, expectedStringContent))));

            mockHandler
                .Verify(mock => mock.Send(It.Is<HttpRequestMessage>(value => TestHelpers.AreEqual(value.Content.ReadAsStringAsync().Result, expectedJsonContent))));
        }

        [TestMethod]
        public async Task SendRequestsWithProvidedHeaders()
        {
            //Arrange
            var mockHandler = new Mock<HttpMessageHandlerWrapper> { CallBase = true };
            var httpClient = new HttpClient(mockHandler.Object);
            var url = "http://test";
        

            var headerDictionary = new Dictionary<string, IEnumerable<string>>();
            headerDictionary.Add("X-Header1", new List<string> { "Header1Value" });
            headerDictionary.Add("Authorization", new List<string> { "Bearer 324234234" });

            var requestHeaders = new NameValueCollection();
            foreach (var headerKvp in headerDictionary)
            {
                requestHeaders.Add(headerKvp.Key, headerKvp.Value.First());
            }

            var graphQlClient = new GraphQLClient(httpClient, url, requestHeaders);

            mockHandler
                .Setup(mock => mock.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage());

            //Act
            await graphQlClient.QueryAsync("query", new { x = 1, y = 2 });

            //Assert
            mockHandler
                .Verify(mock => mock.Send(It.Is<HttpRequestMessage>(value => TestHelpers.AreEqual(value.Headers.ToList(), headerDictionary.ToList()))));
        }

        [TestMethod]
        public async Task ReturnsResponseContent()
        {
            //Arrange
            var mockHandler = new Mock<HttpMessageHandlerWrapper> { CallBase = true };
            var httpClient = new HttpClient(mockHandler.Object);
            var url = "http://test";
            var expectedResponseContent = "{}";

            var graphQlClient = new GraphQLClient(httpClient, url);

            mockHandler
                .Setup(mock => mock.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage
                {
                    Content = new StringContent(expectedResponseContent)
                });

            //Act
            var responseContent = await graphQlClient.QueryAsync("query", new { x = 1, y = 2 });

            //Assert
            responseContent.Should().Be(expectedResponseContent);
        }
    }
}
