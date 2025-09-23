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
    private readonly Lazy<ICARCategoryService> _CARCategoryService;
    private readonly Lazy<IRiskCategoryService> _RiskCategoryService;
    private readonly Lazy<ITypeofcontraventionService> _TypeofcontraventionService;
    private readonly Lazy<IPossibleHazardsService> _PossibleHazardsService;
    private readonly Lazy<IDynamicFormConfigurationService> _DynamicFormConfigurationService;
    private readonly Lazy<IDynamicFlowConfigurationService> _DynamicFlowConfigurationService;
    private readonly Lazy<ITableMappingFieldNameService> _TableMappingFieldNameService;
    private readonly Lazy<IDynamicNewFormService> _DynamicNewFormService;
    private readonly Lazy<IWorkFlowHistoryService> _WorkFlowHistoryService;
    private readonly Lazy<IDraftCARService> _DraftCARService;
    private readonly Lazy<IProcessService> _ProcessService;

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
        _CARCategoryService = new Lazy<ICARCategoryService>(() => new CARCategoryService(data, cacheManager, localization));
        _RiskCategoryService = new Lazy<IRiskCategoryService>(() => new RiskCategoryService(data, cacheManager, localization));
        _TypeofcontraventionService = new Lazy<ITypeofcontraventionService>(() => new TypeofcontraventionService(data, cacheManager, localization));
        _PossibleHazardsService = new Lazy<IPossibleHazardsService>(() => new PossibleHazardsService(data, cacheManager, localization));
        _DynamicFormConfigurationService = new Lazy<IDynamicFormConfigurationService>(() => new DynamicFormConfigurationService(data, cacheManager, localization));
        _DynamicFlowConfigurationService = new Lazy<IDynamicFlowConfigurationService>(() => new DynamicFlowConfigurationService(data, cacheManager, localization));
        _TableMappingFieldNameService = new Lazy<ITableMappingFieldNameService>(() => new TableMappingFieldNameService(data, cacheManager, localization));


        _errorLog = new Lazy<IErrorLogService>(() => new ErrorLogService(data));
        _IssueSubmissionService = new Lazy<IIssueSubmissionService>(() => new IssueSubmissionService(data, data.MDM, masterDataApi, cacheManager, localization));
        _IssueFeedbackReportService = new Lazy<IIssueFeedbackReportService>(() => new IssueFeedbackReportService(data, this,data.MDM, cacheManager, localization));
        _SendMailSettingService = new Lazy<ISendMailSettingService>(() => new SendMailSettingService(data, data.MDM, masterDataApi, cacheManager, localization));
        _DynamicNewFormService = new Lazy<IDynamicNewFormService>(() => new DynamicNewFormService(data, data.MDM, masterDataApi, cacheManager, localization));
        _WorkFlowHistoryService = new Lazy<IWorkFlowHistoryService>(() => new WorkFlowHistoryService(data, data.MDM, masterDataApi, cacheManager, localization));
        _DraftCARService = new Lazy<IDraftCARService>(() => new DraftCARService(data, cacheManager, localization));
        _ProcessService = new Lazy<IProcessService>(() => new ProcessService(data, cacheManager, localization));

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
    public ICARCategoryService CARCategory => _CARCategoryService.Value;
    public IPossibleHazardsService PossibleHazards => _PossibleHazardsService.Value;
    public ITypeofcontraventionService Typeofcontravention => _TypeofcontraventionService.Value;
    public IRiskCategoryService RiskCategory => _RiskCategoryService.Value;
    public IDynamicFormConfigurationService DynamicFormConfiguration => _DynamicFormConfigurationService.Value;
    public IDynamicFlowConfigurationService DynamicFlowConfiguration => _DynamicFlowConfigurationService.Value;
    public ITableMappingFieldNameService TableMappingFieldName => _TableMappingFieldNameService.Value;
    public IDynamicNewFormService DynamicNewForm => _DynamicNewFormService.Value;
    public IWorkFlowHistoryService WorkFlowHistory => _WorkFlowHistoryService.Value;
    public IDraftCARService DraftCAR => _DraftCARService.Value;
    public IProcessService Process => _ProcessService.Value;
}
