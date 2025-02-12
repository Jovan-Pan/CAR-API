using Contracts.Repository;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Contracts.MasterData;
using Contracts.Infrastructure;
using Entities;
using Entities.MasterData;
using Microsoft.AspNetCore.Http;

namespace Services.MasterData
{
    internal sealed class TypeofcontraventionService(IDataManager data, ICacheManager memCache, ILocalizationService localization) : ITypeofcontraventionService
    {
        public async Task<ApiResponse<IEnumerable<TypeofcontraventionDto>>> GetTypeofcontravention(string? search, string? SearchADV, bool delflag)
        {
            var result = await data.Typeofcontravention.GetTypeofcontravention(search, SearchADV, delflag);
            return ApiResponse<IEnumerable<TypeofcontraventionDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<TypeofcontraventionDto>>> InsertNewData(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto)
        {
            var result = await data.Typeofcontravention.InsertNewData(CRUDTypeofcontraventionDto);
            return ApiResponse<IEnumerable<TypeofcontraventionDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<TypeofcontraventionDto>>> UpdateData(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto)
        {
            var result = await data.Typeofcontravention.UpdateData(CRUDTypeofcontraventionDto);
            return ApiResponse<IEnumerable<TypeofcontraventionDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<TypeofcontraventionDto>>> DataDelete(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto)
        {
            var result = await data.Typeofcontravention.DataDelete(CRUDTypeofcontraventionDto);
            return ApiResponse<IEnumerable<TypeofcontraventionDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<TypeofcontraventionDto>>> DataPermDelete(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto)
        {
            var result = await data.Typeofcontravention.DataPermDelete(CRUDTypeofcontraventionDto);
            return ApiResponse<IEnumerable<TypeofcontraventionDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<TypeofcontraventionDto>>> DataRecover(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto)
        {
            var result = await data.Typeofcontravention.DataRecover(CRUDTypeofcontraventionDto);
            return ApiResponse<IEnumerable<TypeofcontraventionDto>>.SuccessResponse(result);
        }

        public async Task<byte[]> Template()
        {
            var result = await data.Typeofcontravention.Template();
            return result;
        }

        public async Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId)
        {
            string filePath = Path.Combine(file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var ImportResult = await data.Typeofcontravention.Import(filePath, userId);

            var resultList = new List<ImportResult> { ImportResult };
            return resultList;
        }
    }
}
