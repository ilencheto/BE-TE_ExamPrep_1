using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;

namespace IdeaCenterExamPrep.Models
{
    internal class ApiResponseDTO
    {
        [JsonPropertyName("msg")]

        public string? Msg { get; set; }

        [JsonPropertyName("id")]

        public string? Id { get; set; }

    }
}
