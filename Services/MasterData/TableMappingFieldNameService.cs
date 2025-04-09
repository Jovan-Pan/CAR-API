using Contracts.Infrastructure;
using Contracts.Repository;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Contracts.MasterData;
using Entities.MasterData;
using Entities;
using Microsoft.AspNetCore.Http;

namespace Services.MasterData
{
    internal sealed class TableMappingFieldNameService(IDataManager data, ICacheManager memCache, ILocalizationService localization) : ITableMappingFieldNameService
    {
        public async Task<ApiResponse<IEnumerable<TableMappingFieldNameDto>>> GetTableMappingFieldName(string? search, string? SearchADVFN, string? SearchADVUID, string? SearchADVLANG, bool delflag, string? plant)
        {
            var result = await data.TableMappingFieldName.GetTableMappingFieldName(search, SearchADVFN, SearchADVUID, SearchADVLANG, delflag, plant);
            return ApiResponse<IEnumerable<TableMappingFieldNameDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<TableMappingFieldNameDto>>> CheckExistingData(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            var result = await data.TableMappingFieldName.CheckExistingData(CRUDTableMappingFieldNameDto);
            return ApiResponse<IEnumerable<TableMappingFieldNameDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<TableMappingFieldNameDto>>> InsertNewData(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            var result = await data.TableMappingFieldName.InsertNewData(CRUDTableMappingFieldNameDto);
            return ApiResponse<IEnumerable<TableMappingFieldNameDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<TableMappingFieldNameDto>>> UpdateData(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            var result = await data.TableMappingFieldName.UpdateData(CRUDTableMappingFieldNameDto);
            return ApiResponse<IEnumerable<TableMappingFieldNameDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<TableMappingFieldNameDto>>> DataDelete(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            var result = await data.TableMappingFieldName.DataDelete(CRUDTableMappingFieldNameDto);
            return ApiResponse<IEnumerable<TableMappingFieldNameDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<TableMappingFieldNameDto>>> DataPermDelete(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            var result = await data.TableMappingFieldName.DataPermDelete(CRUDTableMappingFieldNameDto);
            return ApiResponse<IEnumerable<TableMappingFieldNameDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<TableMappingFieldNameDto>>> DataRecover(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            var result = await data.TableMappingFieldName.DataRecover(CRUDTableMappingFieldNameDto);
            return ApiResponse<IEnumerable<TableMappingFieldNameDto>>.SuccessResponse(result);
        }

        public async Task<byte[]> Template()
        {
            var result = await data.TableMappingFieldName.Template();
            return result;
        }

        public async Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId)
        {
            string filePath = Path.Combine(file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var ImportResult = await data.TableMappingFieldName.Import(filePath, userId);

            var resultList = new List<ImportResult> { ImportResult };
            return resultList;
        }
    }
}
