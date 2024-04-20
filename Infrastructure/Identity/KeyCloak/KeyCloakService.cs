using Application.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Collections.Specialized;
using System;
using Application.Exceptions;

namespace Infrastructure.Identity.KeyCloak;

internal class KeyCloakService : IKeyCloakService
{
    private readonly HttpClient _client;
    private readonly KeyCloakSettings _keycloakSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public KeyCloakService(HttpClient client, IOptions<KeyCloakSettings> keycloakSettings, IHttpContextAccessor httpContextAccessor)
    {
        _client = client;
        _keycloakSettings = keycloakSettings.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TokenResponseDto> GetTokenAsync(string username, string password)
    {
        TokenRequestDto keycloakTokenRequestDto = new()
        {
            GrantType = AccessTokenConsts.GrantTypePassword,
            ClientId = _keycloakSettings.ClientId,
            ClientSecret = _keycloakSettings.ClientSecret,
            Username = username,
            Password = password
        };
        FormUrlEncodedContent tokenRequestBody = TokenUtils.GetTokenRequestBody(keycloakTokenRequestDto);

        string url = $"{_keycloakSettings.BaseUrlLogin}/protocol/openid-connect/token";
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = tokenRequestBody;
        var response = await _client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            string responseJson = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TokenResponseDto>(responseJson);
        }
        else
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException("The credentials to login failed");
            else
                throw new CustomException("Something went wrong");
        }
    }

    public async Task<string> GetUserIdByEmail(string email)
    {
        UriBuilder builder = new($"{_keycloakSettings.BaseUrl}/users");
        NameValueCollection query = HttpUtility.ParseQueryString(builder.Query);
        query["email"] = email;
        builder.Query = query.ToString();
        string url = builder.ToString();

        HttpRequestMessage request = new(HttpMethod.Get, url);
        string token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(token))
        {
            TokenResponseDto tokenResponse = await GetClientCredentialsTokenAsync();
            token = "Bearer " + tokenResponse.AccessToken;
        }
        request.Headers.Add("Authorization", token);
        HttpResponseMessage response = await _client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            string responseJson = await response.Content.ReadAsStringAsync();
            KeyCloakUser keyCloakUser = JsonConvert.DeserializeObject<KeyCloakUser[]>(responseJson).FirstOrDefault();

            return keyCloakUser.Id;
        }
        else
            throw new CustomException("Something went wrong");
    }

    public async Task<string> CreateUser(string username, string email, string password)
    {
        object user = new
        {
            username = username,
            email = email,
            credentials = new[]
            {
                new
                {
                    temporary = false,
                    type = "password",
                    value = password
                }
            },
            enabled = true
        };

        HttpResponseMessage response;
        using (HttpRequestMessage requestMessage =
            new(HttpMethod.Post, $"{_keycloakSettings.BaseUrl}/users"))
        {
            string token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(token))
            {
                TokenResponseDto tokenResponse = await GetClientCredentialsTokenAsync();
                token = "Bearer " + tokenResponse.AccessToken;
            }
            requestMessage.Headers.Add("Authorization", token);

            string jsonBody = JsonConvert.SerializeObject(user);
            StringContent stringContent = new(jsonBody, Encoding.UTF8, "application/json");
            requestMessage.Content = stringContent;

            response = await _client.SendAsync(requestMessage);
        }
        if (response.IsSuccessStatusCode)
        {
            return await GetUserIdByEmail(email);
        }
        else
            throw new CustomException("Something went wrong");
    }

    public async Task<string> DeleteUserAsync(string userId)
    {
        HttpResponseMessage response;
        using (HttpRequestMessage requestMessage =
            new(HttpMethod.Delete, $"{_keycloakSettings.BaseUrl}/users/{userId}"))
        {
            string token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(token))
            {
                TokenResponseDto tokenResponse = await GetClientCredentialsTokenAsync();
                token = "Bearer " + tokenResponse.AccessToken;
            }
            requestMessage.Headers.Add("Authorization", token);

            response = await _client.SendAsync(requestMessage);
        }
        if (response.IsSuccessStatusCode)
        {
            return userId;
        }
        else
            throw new CustomException("Something went wrong");
    }

    private async Task<TokenResponseDto> GetClientCredentialsTokenAsync()
    {
        TokenRequestDto keycloakTokenRequestDto = new()
        {
            GrantType = AccessTokenConsts.ClientCredentials,
            ClientId = _keycloakSettings.ClientId,
            ClientSecret = _keycloakSettings.ClientSecret
        };
        FormUrlEncodedContent tokenRequestBody = TokenUtils.GetTokenRequestBody(keycloakTokenRequestDto);
        HttpResponseMessage response = await _client.PostAsync($"{_keycloakSettings.BaseUrlLogin}/protocol/openid-connect/token", tokenRequestBody);

        string responseJson = await response.Content.ReadAsStringAsync();

        TokenResponseDto keycloakTokenResponseDto = JsonConvert.DeserializeObject<TokenResponseDto>(responseJson);

        return keycloakTokenResponseDto;
    }
}