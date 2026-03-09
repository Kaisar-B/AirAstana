using AirAstana.IntegrationTests.TestSetUp;
using Application.Flights.Commands.CreateFlight;
using Domain.Enums;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AirAstana.IntegrationTests;
public class CreateFlightTest : BaseIntegrationTest
{
    private readonly HttpClient _httpClient;
    public CreateFlightTest(MockWebAppFactory factory) : base(factory)
    {
        _httpClient = factory.CreateClient();
    }
    private async Task<string> GetJwtTokenAsync()
    {
        var login = new
        {
            username = "systemadmin",
            password = "admin!234"
        };

        var response = await _httpClient.PostAsJsonAsync("/api/Authorization/Login", login);

        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();

        return auth!.Token;
    }

    [Fact]
    public async Task CreateFlight_ShouldReturn201()
    {
        // Arrange
        var token = await GetJwtTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var command = new CreateFlightCommand
        {
            Origin = "Алматы",
            Destination = "Астана",
            Departure = DateTimeOffset.UtcNow.AddDays(1),
            Arrival = DateTimeOffset.UtcNow.AddDays(1).AddHours(2),
            Status = Status.InTime
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/Flights", command);

        // Assert
        Assert.Equal(response.StatusCode, HttpStatusCode.Created);
    }
}

public class AuthResponse
{
    public string Token { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}