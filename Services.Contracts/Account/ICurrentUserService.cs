namespace Services.Contracts.Account;

public interface ICurrentUserService
{
    string? UserId { get; }
    bool IsAuthenticated { get; }
}
