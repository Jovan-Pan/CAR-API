using Contracts;
using Contracts.Infrastructure;
using Contracts.Repository;
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
    internal sealed class CARCategoryService(IDataManager data, ICacheManager memCache, ILocalizationService localization) : ICARCategoryService
    {
        public async Task<ApiResponse<IEnumerable<CARCategoryDto>>> GetCARCategory(string? search, string? SearchADV, bool delflag)
        {
            var result = await data.CARCategory.GetCARCategory(search, SearchADV, delflag);
            return ApiResponse<IEnumerable<CARCategoryDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<CARCategoryDto>>> InsertCARCategory(CRUDCARCategoryDto CRUDCARCategoryDto)
        {
            var result = await data.CARCategory.InsertCARCategory(CRUDCARCategoryDto);
            return ApiResponse<IEnumerable<CARCategoryDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<CARCategoryDto>>> UpdateCARCategory(CRUDCARCategoryDto CRUDCARCategoryDto)
        {
            var result = await data.CARCategory.UpdateCARCategory(CRUDCARCategoryDto);
            return ApiResponse<IEnumerable<CARCategoryDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<CARCategoryDto>>> DataDelete(CRUDCARCategoryDto CRUDCARCategoryDto)
        {
            var result = await data.CARCategory.DataDelete(CRUDCARCategoryDto);
            return ApiResponse<IEnumerable<CARCategoryDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<CARCategoryDto>>> DataPermDelete(CRUDCARCategoryDto CRUDCARCategoryDto)
        {
            var result = await data.CARCategory.DataPermDelete(CRUDCARCategoryDto);
            return ApiResponse<IEnumerable<CARCategoryDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<CARCategoryDto>>> DataRecover(CRUDCARCategoryDto CRUDCARCategoryDto)
        {
            var result = await data.CARCategory.DataRecover(CRUDCARCategoryDto);
            return ApiResponse<IEnumerable<CARCategoryDto>>.SuccessResponse(result);
        }

        public async Task<byte[]> Template()
        {
            var result = await data.CARCategory.Template();
            return result;
        }

        public async Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId)
        {
            string filePath = Path.Combine(file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var importResult = await data.CARCategory.Import(filePath, userId);

            var resultList = new List<ImportResult> { importResult };
            return resultList;
        }
    }
}


