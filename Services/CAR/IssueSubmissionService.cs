using Contracts.Infrastructure;
using Contracts.Repository.MasterData;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Repository.CAR;
using Entities;
using Services.Contracts.CAR;
using Contracts.Repository;

namespace Services.CAR
{
    internal sealed class IssueSubmissionService(IDataManager data, IMDMRepository mdm, ICacheManager memCache, ILocalizationService localization): IIssueSubmissionService
    {
        public async Task<ApiResponse<IEnumerable<string>>> GetIssueStatus(string FormNo)
        {
            var result = await data.ISM.GetIssueStatus(FormNo);
            return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
        }
    }
}
