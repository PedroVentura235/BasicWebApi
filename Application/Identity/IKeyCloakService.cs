namespace Application.Identity;

public interface IKeyCloakService
{
    Task<TokenResponseDto> GetTokenAsync(string username, string password);

    Task<string> GetUserIdByEmail(string email);

    Task<string> CreateUser(string username, string email, string password);

    Task<string> DeleteUserAsync(string userId);
}