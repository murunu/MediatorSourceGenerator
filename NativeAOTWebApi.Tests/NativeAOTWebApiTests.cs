using System.Net;
using System.Web;

namespace NativeAOTWebApi.Tests;

public class NativeAOTWebApiTests : IClassFixture<NativeAOTWebApiFactory>
{
    private HttpClient _httpClient;

    public NativeAOTWebApiTests(NativeAOTWebApiFactory nativeAotWebApiFactory)
    {
        _httpClient = nativeAotWebApiFactory.CreateClient();
    }

    [Theory]
    [InlineData("/send/success", "Only 1 message sent!")]
    [InlineData("/publish", "It worked!")]
    [InlineData("/sendwithvalue/success?page=message", "message")]
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
    [InlineData("/send/error")]
    [InlineData("/sendwithvalue/error?page=message")]
    public async Task SendMessageShouldReturnError(string endpoint)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        
        // Act
        var response = await _httpClient.SendAsync(request);

        var content = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.StartsWith("Mediator.Exceptions.NoServiceException", content);
    }
}