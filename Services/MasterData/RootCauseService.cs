using Contracts.Infrastructure;
using Contracts.Repository;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Services.Contracts.MasterData;

namespace Services.MasterData
{
    internal sealed class RootCauseService(IDataManager data, ICacheManager memCache, ILocalizationService localization): IRootCauseService
    {
        public async Task<ApiResponse<IEnumerable<string>>> getRootCauseList(int plant)
        {
            var result = await data.RootCause.getRootCauseList(plant);
            return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
        }
    }
}
