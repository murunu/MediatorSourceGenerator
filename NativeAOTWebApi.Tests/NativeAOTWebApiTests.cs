using System.Net;

namespace NativeAOTWebApi.Tests;

public class NativeAOTWebApiTests : IClassFixture<NativeAOTWebApiFactory>
{
    private readonly HttpClient _httpClient;

    public NativeAOTWebApiTests(NativeAOTWebApiFactory nativeAotWebApiFactory)
    {
        _httpClient = nativeAotWebApiFactory.CreateClient();
    }

    [Theory]
    [InlineData("/async/send/success", "Only 1 message sent!")]
    [InlineData("/async/publish", "It worked!")]
    [InlineData("/async/sendwithvalue/success/message", "message")]
    [InlineData("/void/send/success", "Only 1 message sent!")]
    [InlineData("/void/publish", "It worked!")]
    [InlineData("/void/sendwithvalue/success/message", "message")]
    public async Task SendMessageShouldReturnSuccess(string endpoint, string expected)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        // Act
        var response = await _httpClient.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.Equal(expected, responseString);
    }
    
    [Theory]
    [InlineData("/async/send/failure")]
    [InlineData("/async/sendwithvalue/failure/message")]
    [InlineData("/void/send/failure")]
    [InlineData("/void/sendwithvalue/failure/message")]
    public async Task SendMessageShouldReturnError(string endpoint)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        
        // Act
        var response = await _httpClient.SendAsync(request);

        var content = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.StartsWith("System.AggregateException", content);
    }
}