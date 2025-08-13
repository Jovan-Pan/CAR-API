using Contracts.Repository.CAR;
using Contracts.Repository.MasterData;

namespace Contracts.Repository;

public interface IDataManager
{
    IMDMRepository MDM {  get; }
    IImmidateActionRepository ImmAct { get; }
    IRootCauseRepository RootCause { get; }
    IErrorLogRepository ErrorLog { get; }
    IIssueSubmissionRepository ISM { get; }
    IIssueFeedbackReportRepository IFR { get; }
    ISendMailSettingRepository SMS { get; }
    INCTextSentenceRepository NCTS { get; }
    ICARCategoryRepository CARCategory { get; }
    IRiskCategoryRepository RiskCategory { get; }
    ITypeOfContraventionRepository Typeofcontravention { get; }
    IPossibleHazardsRepository PossibleHazards { get; }
    IDynamicFormConfigurationRepository DynamicFormConfiguration { get; }
    IDynamicNewFormRepository DynamicNewForm { get; }
    IDynamicFlowConfigurationRepository DynamicFlowConfiguration { get; }
    ITableMappingFieldNameRepository TableMappingFieldName { get; }
    IWorkFlowHistoryRepository WorkFlowHistory { get; }
}
