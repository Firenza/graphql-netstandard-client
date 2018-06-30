using FluentAssertions;
using GraphQl.NetStandard.Client;
using GraphQL.NetStandard.Client.UnitTests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
            await graphQlClient.QueryAsync("query", new { x = 1, y = 2 });

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

        [TestMethod]
        public async Task SendsRequestAndDeserialzesSimpleRepsonse()
        {
            //Arrange
            var mockHandler = new Mock<HttpMessageHandlerWrapper> { CallBase = true };
            var httpClient = new HttpClient(mockHandler.Object);
            var url = "http://test";
            var returnString = "{data : { testDto : { property1: \"value1\", property2 : \"value2\"}}}";

            var graphQlClient = new GraphQLClient(httpClient, url);

            mockHandler
                .Setup(mock => mock.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage
                {
                    Content = new StringContent(returnString)
                });

            var expectedTestDTO = new TestDTO
            {
                Property1 = "value1",
                Property2 = "value2"
            };

            //Act
            var resposneTestDTO = await graphQlClient.QueryAsync<TestDTO>(null);

            //Assert
            resposneTestDTO.ShouldBeEquivalentTo(expectedTestDTO);
        }

        [TestMethod]
        public async Task SendsRequestAndDeserialesRepsonseWithNodes()
        {
            //Arrange
            var mockHandler = new Mock<HttpMessageHandlerWrapper> { CallBase = true };
            var httpClient = new HttpClient(mockHandler.Object);
            var url = "http://test";
            var returnString = @"
            {
              ""data"": {
                ""organization"": {
                  ""repository"": {
                    ""pullRequests"": {
                      ""nodes"": [
                        {
                          ""baseRefName"": ""master"",
                          ""headRefName"": ""development"",
                          ""reviews"": {
                            ""totalCount"": 100,
                            ""nodes"": [
                              {
                                ""state"": ""APPROVED"",
                                ""author"": {
                                  ""login"": ""bob""
                                }
                              }
                            ],
                            ""pageInfo"": {
                              ""hasNextPage"": true,
                              ""hasPreviousPage"": true,
                              ""endCursor"": ""ImDaEndCursor"",
                              ""startCursor"": ""ImDaStartCursor""
                            }
                          }
                        }
                      ]
                    }
                  }
                }
              }
            }
            ";

            var graphQlClient = new GraphQLClient(httpClient, url);

            mockHandler
                .Setup(mock => mock.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage
                {
                    Content = new StringContent(returnString)
                });

            var expectedOrganization = new Organization
            {
                Repository = new Repository
                {
                    PullRequests = new GraphQlNodesParent<PullRequest>
                    {
                        Nodes = new List<PullRequest>
                        {
                            new PullRequest
                            {
                                BaseRefName = "master",
                                HeadRefName = "development",
                                Reviews = new GraphQlNodesParent<Review>
                                {
                                    TotalCount = 100,
                                    Nodes = new List<Review>
                                    {
                                        new Review
                                        {
                                            State = "APPROVED",
                                            Author = new Author
                                            {
                                                Login = "bob"
                                            }
                                        }
                                    },
                                    PageInfo = new GraphQlPageInfo
                                    {
                                        HasNextPage = true,
                                        HasPreviousPage = true,
                                        EndCursor = "ImDaEndCursor",
                                        StartCursor = "ImDaStartCursor"
                                    }
                                }
                            }
                        }

                    }
                }
            };

            //Act
            var responseOrganization = await graphQlClient.QueryAsync<Organization>(null);

            //Assert
            responseOrganization.ShouldBeEquivalentTo(expectedOrganization);
        }

        [TestMethod]
        public async Task SendsRequestWithVariblesAndDeserialzesSimpleRepsonse()
        {
            //Arrange
            var mockHandler = new Mock<HttpMessageHandlerWrapper> { CallBase = true };
            var httpClient = new HttpClient(mockHandler.Object);
            var url = "http://test";
            var returnString = "{data : { testDto : { property1: \"value1\", property2 : \"value2\"}}}";

            var graphQlClient = new GraphQLClient(httpClient, url);

            mockHandler
                .Setup(mock => mock.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage
                {
                    Content = new StringContent(returnString)
                });

            var expectedTestDTO = new TestDTO
            {
                Property1 = "value1",
                Property2 = "value2"
            };

            //Act
            var resposneTestDTO = await graphQlClient.QueryAsync<TestDTO>(null, new { varible1 = "value" });

            //Assert
            resposneTestDTO.ShouldBeEquivalentTo(expectedTestDTO);
        }

        [TestMethod]
        public async Task HandlesErrorsInQuery()
        {
            //Arrange
            var mockHandler = new Mock<HttpMessageHandlerWrapper> { CallBase = true };
            var httpClient = new HttpClient(mockHandler.Object);
            var url = "http://test";
            var returnString = "{data : null, errors : [{message : \"errorMessage1\"}, {message: \"errorMessage2\"}]}";

            var graphQlClient = new GraphQLClient(httpClient, url);

            mockHandler
                .Setup(mock => mock.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage
                {
                    Content = new StringContent(returnString)
                });

            var expectedTestDTO = new TestDTO
            {
                Property1 = "value1",
                Property2 = "value2"
            };

            GraphQLQueryException expectedException = null;

            //Act
            try
            {
                await graphQlClient.QueryAsync(null);
            }
            catch (GraphQLQueryException ex)
            {
                expectedException = ex;
            }

            //Assert
            expectedException.Should().NotBeNull();
            expectedException.Message.Should().Be("errorMessage1|errorMessage2");
        }


        [TestMethod]
        public async Task HandlesRequestError()
        {
            //Arrange
            var mockHandler = new Mock<HttpMessageHandlerWrapper> { CallBase = true };
            var httpClient = new HttpClient(mockHandler.Object);
            var url = "http://test";
            var returnString = "{data : null, errors : [{message : \"errorMessage1\"}, {message: \"errorMessage2\"}]}";

            var graphQlClient = new GraphQLClient(httpClient, url);

            mockHandler
                .Setup(mock => mock.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(returnString)
                });

            var expectedTestDTO = new TestDTO
            {
                Property1 = "value1",
                Property2 = "value2"
            };

            GraphQLRequestException expectedException = null;

            //Act
            try
            {
                await graphQlClient.QueryAsync(null);
            }
            catch (GraphQLRequestException ex)
            {
                expectedException = ex;
            }

            //Assert
            expectedException.Should().NotBeNull();
            expectedException.Message.Should().Be("Request failed with status code of InternalServerError");
            expectedException.ResponseBody.Should().Be(returnString);
        }
    }
}

