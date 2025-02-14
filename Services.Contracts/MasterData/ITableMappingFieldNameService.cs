using Entities;
using Entities.MasterData;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.MasterData
{
    public interface ITableMappingFieldNameService
    {
        Task<ApiResponse<IEnumerable<TableMappingFieldNameDto>>> GetTableMappingFieldName(string? search, string? SearchADVFN, string? SearchADVUID, string? SearchADVLANG, bool delflag, string? Language);
        Task<ApiResponse<IEnumerable<TableMappingFieldNameDto>>> InsertNewData(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto);
        Task<ApiResponse<IEnumerable<TableMappingFieldNameDto>>> UpdateData(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto);
        Task<ApiResponse<IEnumerable<TableMappingFieldNameDto>>> DataDelete(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto);
        Task<ApiResponse<IEnumerable<TableMappingFieldNameDto>>> DataPermDelete(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto);
        Task<ApiResponse<IEnumerable<TableMappingFieldNameDto>>> DataRecover(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto);
        Task<byte[]> Template();
        Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId);
    }
}
