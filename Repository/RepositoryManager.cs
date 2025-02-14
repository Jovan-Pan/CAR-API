using Contracts.Repository;
using Contracts.Repository.CAR;
using Contracts.Repository.MasterData;
using Microsoft.AspNetCore.Http;
using Repository.CAR;
using Repository.MasterData;

namespace Repository;

public sealed class RepositoryManager(DbContext dbContext) : IDataManager
{
    private readonly Lazy<IMDMRepository> _mdmRepo = new(() => new MDMRepository(dbContext));
    private readonly Lazy<IImmidateActionRepository> _immediteActrepo = new(() => new ImmidateActionRepository(dbContext));
    private readonly Lazy<IRootCauseRepository> _rootcauserepo = new(() => new RootCauseRepository(dbContext));
    private readonly Lazy<IErrorLogRepository> _ErrorLog = new(() => new ErrorLogRepository(dbContext));
    private readonly Lazy<IIssueSubmissionRepository> _ISM = new(() => new IssueSubmissionRepository(dbContext));
    private readonly Lazy<IIssueFeedbackReportRepository> _IFR = new(() => new IssueFeedbackReportRepository(dbContext));
    private readonly Lazy<ISendMailSettingRepository> _SMS = new(() => new SendMailSettingRepository(dbContext));
    private readonly Lazy<INCTextSentenceRepository> _NCTS = new(() => new NCTextSentenceRepository(dbContext));
    private readonly Lazy<ICARCategoryRepository> _CARCategory = new(() => new CARCategoryRepository(dbContext));
    private readonly Lazy<IPossibleHazardsRepository> _PossibleHazards = new(() => new PossibleHazardsRepository(dbContext));
    private readonly Lazy<ITypeOfContraventionRepository> _Typeofcontravention = new(() => new TypeofcontraventionRepository(dbContext));
    private readonly Lazy<IRiskCategoryRepository> _RiskCategory = new(() => new RiskCategoryRepository(dbContext));
    private readonly Lazy<IDynamicFormConfigurationRepository> _DynamicFormConfiguration = new(() => new DynamicFormConfigurationRepository(dbContext));
    private readonly Lazy<IDynamicNewFormRepository> _DynamicNewForm = new(() => new DynamicNewFormRepository(dbContext));
    private readonly Lazy<IDynamicFlowConfigurationRepository> _DynamicFlowConfiguration = new(() => new DynamicFlowConfigurationRepository(dbContext));
    private readonly Lazy<ITableMappingFieldNameRepository> _TableMappingFieldName = new(() => new TableMappingFieldNameRepository(dbContext));
    public IMDMRepository MDM => _mdmRepo.Value;
    public IImmidateActionRepository ImmAct => _immediteActrepo.Value;
    public IRootCauseRepository RootCause => _rootcauserepo.Value;
    public IErrorLogRepository ErrorLog => _ErrorLog.Value;
    public IIssueSubmissionRepository ISM => _ISM.Value;
    public IIssueFeedbackReportRepository IFR => _IFR.Value;
    public ISendMailSettingRepository SMS => _SMS.Value;
    public INCTextSentenceRepository NCTS => _NCTS.Value;
    public ICARCategoryRepository CARCategory => _CARCategory.Value;
    public IPossibleHazardsRepository PossibleHazards => _PossibleHazards.Value;
    public ITypeOfContraventionRepository Typeofcontravention => _Typeofcontravention.Value;
    public IRiskCategoryRepository RiskCategory => _RiskCategory.Value;
    public IDynamicFormConfigurationRepository DynamicFormConfiguration => _DynamicFormConfiguration.Value;
    public IDynamicNewFormRepository DynamicNewForm => _DynamicNewForm.Value;
    public IDynamicFlowConfigurationRepository DynamicFlowConfiguration => _DynamicFlowConfiguration.Value;
    public ITableMappingFieldNameRepository TableMappingFieldName => _TableMappingFieldName.Value;
}
