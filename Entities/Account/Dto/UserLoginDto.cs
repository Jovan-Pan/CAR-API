namespace Entities.Account.Dto;

public record UserLoginDto(string UserId, string Password, bool EnableOtp);