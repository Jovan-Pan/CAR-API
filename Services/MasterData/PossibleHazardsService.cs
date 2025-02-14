using Contracts.Infrastructure;
using Contracts.Repository;
using Contracts;
using Entities.MasterData;
using Entities;
using Microsoft.AspNetCore.Http;
using Services.Contracts.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MasterData
{
    internal sealed class PossibleHazardsService(IDataManager data, ICacheManager memCache, ILocalizationService localization) : IPossibleHazardsService
    {
        public async Task<ApiResponse<IEnumerable<PossibleHazardsDto>>> GetPossibleHazards(string? search, string? SearchADV, bool delflag)
        {
            var result = await data.PossibleHazards.GetPossibleHazards(search, SearchADV, delflag);
            return ApiResponse<IEnumerable<PossibleHazardsDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<PossibleHazardsDto>>> InsertNewData(CRUDPossibleHazardsDto CRUDPossibleHazardsDto)
        {
            var result = await data.PossibleHazards.InsertNewData(CRUDPossibleHazardsDto);
            return ApiResponse<IEnumerable<PossibleHazardsDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<PossibleHazardsDto>>> UpdateData(CRUDPossibleHazardsDto CRUDPossibleHazardsDto)
        {
            var result = await data.PossibleHazards.UpdateData(CRUDPossibleHazardsDto);
            return ApiResponse<IEnumerable<PossibleHazardsDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<PossibleHazardsDto>>> DataDelete(CRUDPossibleHazardsDto CRUDPossibleHazardsDto)
        {
            var result = await data.PossibleHazards.DataDelete(CRUDPossibleHazardsDto);
            return ApiResponse<IEnumerable<PossibleHazardsDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<PossibleHazardsDto>>> DataPermDelete(CRUDPossibleHazardsDto CRUDPossibleHazardsDto)
        {
            var result = await data.PossibleHazards.DataPermDelete(CRUDPossibleHazardsDto);
            return ApiResponse<IEnumerable<PossibleHazardsDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<PossibleHazardsDto>>> DataRecover(CRUDPossibleHazardsDto CRUDPossibleHazardsDto)
        {
            var result = await data.PossibleHazards.DataRecover(CRUDPossibleHazardsDto);
            return ApiResponse<IEnumerable<PossibleHazardsDto>>.SuccessResponse(result);
        }

        public async Task<byte[]> Template()
        {
            var result = await data.PossibleHazards.Template();
            return result;
        }

        public async Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId)
        {
            string filePath = Path.Combine(file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var ImportResult = await data.PossibleHazards.Import(filePath, userId);

            var resultList = new List<ImportResult> { ImportResult };
            return resultList;
        }
    }
}
