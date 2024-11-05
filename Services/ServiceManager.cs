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
    private readonly Lazy<IImmidateActionService> _immActBusiness;
    private readonly Lazy<IRootCauseService> _rootCauseBusiness;
    private readonly Lazy<IErrorLogService> _errorLog;
    private readonly Lazy<IIssueSubmissionService> _IssueSubmissionService;
    private readonly Lazy<IIssueFeedbackReportService> _IssueFeedbackReportService;
    private readonly Lazy<ISendMailSettingService> _SendMailSettingService;
    private readonly Lazy<INCTextSentenceService> _NCTextSentenceService;

    public ServiceManager(IDataManager data,
        IMasterDataApi masterDataApi,
        ICacheManager cacheManager,
        ILocalizationService localization,
        IOptions<AppSettings> appSettings
        )
    {
        _accountBusiness = new Lazy<IAccountService>(() => new AccountService(this, masterDataApi, localization));
        _masterDataBusiness = new Lazy<IMasterDataService>(() => new MasterDataService(masterDataApi, data.MDM, cacheManager, localization));
        _immActBusiness = new Lazy<IImmidateActionService>(() => new ImmidateActionService(data, cacheManager, localization));
        _rootCauseBusiness = new Lazy<IRootCauseService>(() => new RootCauseService(data, cacheManager, localization));
        _NCTextSentenceService = new Lazy<INCTextSentenceService>(() => new NCTextSentenceService(data, cacheManager, localization));

        _errorLog = new Lazy<IErrorLogService>(() => new ErrorLogService(data));
        _IssueSubmissionService = new Lazy<IIssueSubmissionService>(() => new IssueSubmissionService(data, data.MDM, masterDataApi, cacheManager, localization));
        _IssueFeedbackReportService = new Lazy<IIssueFeedbackReportService>(() => new IssueFeedbackReportService(data, this,data.MDM, cacheManager, localization));
        _SendMailSettingService = new Lazy<ISendMailSettingService>(() => new SendMailSettingService(data, data.MDM, masterDataApi, cacheManager, localization));
    }
    public IAccountService Account => _accountBusiness.Value;
    public IMasterDataService MasterData => _masterDataBusiness.Value;
    public IImmidateActionService ImmAct => _immActBusiness.Value;
    public IRootCauseService RootCause => _rootCauseBusiness.Value;
    public IErrorLogService ErrorLog => _errorLog.Value;
    public IIssueSubmissionService IssueSubmission => _IssueSubmissionService.Value;
    public IIssueFeedbackReportService IFR => _IssueFeedbackReportService.Value;
    public ISendMailSettingService sendmailsetting => _SendMailSettingService.Value;
    public INCTextSentenceService NCTS => _NCTextSentenceService.Value;
}
