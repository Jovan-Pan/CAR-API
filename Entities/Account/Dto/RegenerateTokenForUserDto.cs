namespace Entities.Account.Dto;

public class RegenerateTokenForUserDto
{
    public string UserId { get; set; } = "";
    public string RefreshToken { get; set; } = "";
}
