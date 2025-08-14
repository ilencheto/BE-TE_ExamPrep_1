using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using IdeaCenterExamPrep.Models;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;




namespace IdeaCenterExamPrep
{
    [TestFixture]
    public class IdeaCenterAPITestEK
    {
        private RestClient client;
        private static string lastCreatedIdeaId;

        private const string BaseUrl =
            "http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:84/api";

        private const string StaticToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKd3RTZXJ2aWNlQWNjZXNzVG9rZW4iLCJqdGkiOiJkZDM5YmNkZC02YWI3LTQzMzEtYjkyOS0xZjlkMTkyMDE3MzUiLCJpYXQiOiIwOC8xMy8yMDI1IDE3OjQ2OjEwIiwiVXNlcklkIjoiOTUxNWJmNDAtNGUwNy00YjQ5LWQyODctMDhkZGQ0ZTA4YmQ4IiwiRW1haWwiOiJFTElFbGVuYUBFTEluYS5iZyIsIlVzZXJOYW1lIjoiZXhhbVByZXBFSyIsImV4cCI6MTc1NTEyODc3MCwiaXNzIjoiSWRlYUNlbnRlcl9BcHBfU29mdFVuaSIsImF1ZCI6IklkZWFDZW50ZXJfV2ViQVBJX1NvZnRVbmkifQ.RyKijs6PijAEY2A9I85PJct1hhXGVkX4px1qI0xDfVY";

        private const string logInEMail = "ELIElena@ELIna.bg";
        private const string logInPassWord = "123456";

        [OneTimeSetUp]
        public void Setup()
        {
            string jwtToken;

            if (!string.IsNullOrWhiteSpace(StaticToken))
            {
                jwtToken = StaticToken;
            }
            else
            {
                jwtToken = GetJwtToken(logInEMail, logInPassWord);
            }

            var options = new RestClientOptions(BaseUrl)
            {
                Authenticator = new JwtAuthenticator(jwtToken),
            };

            this.client = new RestClient(options);
        }

        private string GetJwtToken(string email, string password)
        {
            var tempCLient = new RestClient(BaseUrl);
            var request = new RestRequest("/api/User/Authentication", Method.Post);
            request.AddJsonBody(new { email, password });

            var response = tempCLient.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = JsonSerializer.Deserialize<JsonElement>(response.Content);
                var token = content.GetProperty("accessToken").GetString();

                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new InvalidOperationException("Failed to retrieve JWT token from the response.");
                }
                return token;
            }
            else
            {
                throw new InvalidOperationException($"Failed to authenticate. Status code: {response.StatusCode}, Content: {response.Content}");
            }
        }

        [Order(1)]
        [Test]

        public void CreateIdea_WithRequiredFields_ShouldReturnSuccess()
        {
            var ideaRequest = new IdeaDTO
            {
                Title = "Test Idea",
                Description = "This is a test idea description.",
                Url = ""
            };

            var request = new RestRequest("/api/Idea/Create", Method.Post);
            request.AddJsonBody(ideaRequest);
            var response = this.client.Execute(request);

            Console.WriteLine($"StatusCode: {response.StatusCode}");
            Console.WriteLine($"Content: {response.Content}");

            var createResponse = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(createResponse.Msg, Is.EqualTo("Successfully created!"));
        }

    }
}