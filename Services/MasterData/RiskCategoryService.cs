using Contracts.Infrastructure;
using Contracts.Repository;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Contracts.MasterData;
using Entities;
using Entities.MasterData;
using Microsoft.AspNetCore.Http;

namespace Services.MasterData
{
    internal sealed class RiskCategoryService(IDataManager data, ICacheManager memCache, ILocalizationService localization) : IRiskCategoryService
    {
        public async Task<ApiResponse<IEnumerable<RiskCategoryDto>>> GetRiskCategory(GETRiskCategory GETRiskCategory)
        {
            var result = await data.RiskCategory.GetRiskCategory(GETRiskCategory);
            return ApiResponse<IEnumerable<RiskCategoryDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<RiskCategoryDto>>> InsertNewData(CRUDRiskCategoryDto CRUDRiskCategoryDto)
        {
            var result = await data.RiskCategory.InsertNewData(CRUDRiskCategoryDto);
            return ApiResponse<IEnumerable<RiskCategoryDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<RiskCategoryDto>>> UpdateData(CRUDRiskCategoryDto CRUDRiskCategoryDto)
        {
            var result = await data.RiskCategory.UpdateData(CRUDRiskCategoryDto);
            return ApiResponse<IEnumerable<RiskCategoryDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<RiskCategoryDto>>> DataDelete(CRUDRiskCategoryDto CRUDRiskCategoryDto)
        {
            var result = await data.RiskCategory.DataDelete(CRUDRiskCategoryDto);
            return ApiResponse<IEnumerable<RiskCategoryDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<RiskCategoryDto>>> DataPermDelete(CRUDRiskCategoryDto CRUDRiskCategoryDto)
        {
            var result = await data.RiskCategory.DataPermDelete(CRUDRiskCategoryDto);
            return ApiResponse<IEnumerable<RiskCategoryDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<RiskCategoryDto>>> DataRecover(CRUDRiskCategoryDto CRUDRiskCategoryDto)
        {
            var result = await data.RiskCategory.DataRecover(CRUDRiskCategoryDto);
            return ApiResponse<IEnumerable<RiskCategoryDto>>.SuccessResponse(result);
        }

        public async Task<byte[]> Template()
        {
            var result = await data.RiskCategory.Template();
            return result;
        }

        public async Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId)
        {
            string filePath = Path.Combine(file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var ImportResult = await data.RiskCategory.Import(filePath, userId);

            var resultList = new List<ImportResult> { ImportResult };
            return resultList;
        }
    }
}
