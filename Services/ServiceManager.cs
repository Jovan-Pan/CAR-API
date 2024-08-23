using Contracts;
using Contracts.Infrastructure;
using Contracts.Repository;
using Contracts.Repository.CAR;
using Microsoft.Extensions.Options;
using Services.Account;
using Services.CAR;
using Services.Contracts;
using Services.Contracts.Account;
using Services.Contracts.CAR;
using Services.Contracts.MasterData;
using Services.MasterData;

namespace Services;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IAccountService> _accountBusiness;
    private readonly Lazy<IMasterDataService> _masterDataBusiness;
    private readonly Lazy<IErrorLogService> _errorLog;
    private readonly Lazy<IIssueSubmissionService> _IssueSubmissionService;

    public ServiceManager(IDataManager data,
        IMasterDataApi masterDataApi,
        ICacheManager cacheManager,
        ILocalizationService localization,
        IOptions<AppSettings> appSettings
        )
    {
        _accountBusiness = new Lazy<IAccountService>(() => new AccountService(this, masterDataApi, localization));
        _masterDataBusiness = new Lazy<IMasterDataService>(() => new MasterDataService(masterDataApi, data.MDM, cacheManager, localization));
        _errorLog = new Lazy<IErrorLogService>(() => new ErrorLogService(data));
        _IssueSubmissionService = new Lazy<IIssueSubmissionService>(() => new IssueSubmissionService(data, data.MDM, cacheManager, localization));
    }
    public IAccountService Account => _accountBusiness.Value;
    public IMasterDataService MasterData => _masterDataBusiness.Value;
    public IErrorLogService ErrorLog => _errorLog.Value;
    public IIssueSubmissionService IssueSubmission => _IssueSubmissionService.Value;
}
