using Services.Contracts.Account;
using Services.Contracts.CAR;
using Services.Contracts.MasterData;

namespace Services.Contracts;

public interface IServiceManager
{
    IAccountService Account { get; }
    IMasterDataService MasterData { get; }
    IErrorLogService ErrorLog { get; }
    IIssueSubmissionService IssueSubmission { get; }
}
