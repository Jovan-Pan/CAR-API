using Services.Contracts.Account;
using Services.Contracts.CAR;
using Services.Contracts.MasterData;

namespace Services.Contracts;

public interface IServiceManager
{
    IAccountService Account { get; }
    IMasterDataService MasterData { get; }
    IImmidateActionService ImmAct { get; }
    IRootCauseService RootCause { get; }
    IErrorLogService ErrorLog { get; }
    IIssueSubmissionService IssueSubmission { get; }
    IIssueFeedbackReportService IFR { get; }
    ISendMailSettingService sendmailsetting { get; }
    INCTextSentenceService NCTS { get; }
    ICARCategoryService CARCategory { get; }
    IRiskCategoryService RiskCategory { get; }
    ITypeofcontraventionService Typeofcontravention { get; }
    IPossibleHazardsService PossibleHazards { get; }
    IDynamicFormConfigurationService DynamicFormConfiguration { get; }
    IDynamicFlowConfigurationService DynamicFlowConfiguration { get; }
    ITableMappingFieldNameService TableMappingFieldName { get; }
    IDynamicNewFormService DynamicNewForm { get; }
    IWorkFlowHistoryService WorkFlowHistory { get; }
    IDraftCARService DraftCAR { get; }
    IProcessService Process { get; }
}
