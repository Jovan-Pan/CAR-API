using Services.Contracts.Account;
using Services.Contracts.MasterData;

namespace Services.Contracts;

public interface IServiceManager
{
    IAccountService Account { get; }
    IMasterDataService MasterData { get; }
}
