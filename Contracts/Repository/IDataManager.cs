using Contracts.Repository.CAR;
using Contracts.Repository.MasterData;

namespace Contracts.Repository;

public interface IDataManager
{
    IMDMRepository MDM {  get; }
    IErrorLogRepository ErrorLog { get; }
    IIssueSubmissionRepository ISM { get; }
}
