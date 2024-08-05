using Contracts;
using Contracts.Infrastructure;
using Contracts.Repository;
using Microsoft.Extensions.Options;
using Services.Account;
using Services.Contracts;
using Services.Contracts.Account;
using Services.Contracts.MasterData;
using Services.MasterData;

namespace Services;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IAccountService> _accountBusiness;
    private readonly Lazy<IMasterDataService> _masterDataBusiness;

    public ServiceManager(IDataManager data,
        IMasterDataApi masterDataApi,
        ICacheManager cacheManager,
        ILocalizationService localization,
        IOptions<AppSettings> appSettings
        )
    {
        _accountBusiness = new Lazy<IAccountService>(() => new AccountService(this, masterDataApi, localization));
        _masterDataBusiness = new Lazy<IMasterDataService>(() => new MasterDataService(masterDataApi, data.MDM, cacheManager, localization));
    }
    public IAccountService Account => _accountBusiness.Value;
    public IMasterDataService MasterData => _masterDataBusiness.Value;
}
