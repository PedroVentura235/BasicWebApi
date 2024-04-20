using Infrastructure.Identity.Services;

namespace Infrastructure.Identity.KeyCloak;

public static class TokenUtils
{
    public static FormUrlEncodedContent GetTokenRequestBody(
        TokenRequestDto keycloakTokenDto)
    {
        var keyValuePairs = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(
                    AccessTokenConsts.GrantType, keycloakTokenDto.GrantType),
                new KeyValuePair<string, string>(
                    AccessTokenConsts.ClientId, keycloakTokenDto.ClientId),
                new KeyValuePair<string, string>(
                    AccessTokenConsts.ClientSecret, keycloakTokenDto.ClientSecret),
                new KeyValuePair<string, string>(
                    AccessTokenConsts.Username, keycloakTokenDto.Username),
                new KeyValuePair<string, string>(
                    AccessTokenConsts.Password, keycloakTokenDto.Password)
            };
        return new FormUrlEncodedContent(keyValuePairs);
    }
}