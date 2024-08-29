using Entities.MasterData;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Infrastructure;
using Contracts.Repository.MasterData;
using Contracts;
using Services.Resources;
using Services.Contracts.MasterData;
using Contracts.Repository;

namespace Services.MasterData
{
    internal sealed class ImmidateActionService(IDataManager data, ICacheManager memCache, ILocalizationService localization): IImmidateActionService
    {
        public async Task<ApiResponse<IEnumerable<string>>> getImmidateActionList(int plant)
        {
            var result = await data.ImmAct.getImmidateActionList(plant);
            return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
        }
    }
}
