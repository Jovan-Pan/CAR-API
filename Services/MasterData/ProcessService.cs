using Contracts.Infrastructure;
using Contracts.Repository;
using Contracts;
using Services.Contracts.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.MasterData;
using Entities;
using Microsoft.AspNetCore.Http;

namespace Services.MasterData
{
    internal sealed class ProcessService(IDataManager data, ICacheManager memCache, ILocalizationService localization) : IProcessService
    {
        public async Task<ApiResponse<IEnumerable<ProcessDto>>> GetProcess(GETProcess GETProcess)
        {
            var result = await data.Process.GetProcess(GETProcess);
            return ApiResponse<IEnumerable<ProcessDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<ProcessDto>>> InsertNewData(CRUDProcessDto CRUDProcessDto)
        {
            var result = await data.Process.InsertNewData(CRUDProcessDto);
            return ApiResponse<IEnumerable<ProcessDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<ProcessDto>>> UpdateData(CRUDProcessDto CRUDProcessDto)
        {
            var result = await data.Process.UpdateData(CRUDProcessDto);
            return ApiResponse<IEnumerable<ProcessDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<ProcessDto>>> DataDelete(CRUDProcessDto CRUDProcessDto)
        {
            var result = await data.Process.DataDelete(CRUDProcessDto);
            return ApiResponse<IEnumerable<ProcessDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<ProcessDto>>> DataPermDelete(CRUDProcessDto CRUDProcessDto)
        {
            var result = await data.Process.DataPermDelete(CRUDProcessDto);
            return ApiResponse<IEnumerable<ProcessDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<ProcessDto>>> DataRecover(CRUDProcessDto CRUDProcessDto)
        {
            var result = await data.Process.DataRecover(CRUDProcessDto);
            return ApiResponse<IEnumerable<ProcessDto>>.SuccessResponse(result);
        }

        public async Task<byte[]> Template()
        {
            var result = await data.Process.Template();
            return result;
        }

        public async Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId)
        {
            string filePath = Path.Combine(file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var ImportResult = await data.Process.Import(filePath, userId);

            var resultList = new List<ImportResult> { ImportResult };
            return resultList;
        }
    }
}
