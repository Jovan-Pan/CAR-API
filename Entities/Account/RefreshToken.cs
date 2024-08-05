namespace Entities.Account;

public class RefreshToken
{
    public string RefreshTokenKey { get; set; } = "";
    public DateTime RefreshTokenExpiry { get; set; }
}
