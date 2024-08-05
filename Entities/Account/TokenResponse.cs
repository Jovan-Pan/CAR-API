namespace Entities.Account;

public class TokenResponse : UserInfo
{
    public string Token { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public DateTime RefreshTokenExpiry { get; set; }
}
