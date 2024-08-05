namespace Entities.Account;

public class UserToken(
    string userId,
    string refreshToken,
    DateTime refreshTokenExpiry)
{
    public string UserId { get; set; } = userId;
    public string RefreshToken { get; set; } = refreshToken;
    public DateTime RefreshTokenExpiry { get; set; } = refreshTokenExpiry;

    public bool isRefreshTokenExpired()
    {
        return DateTime.Now > RefreshTokenExpiry;
    }
}
