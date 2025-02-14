using Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.MasterData
{
    public interface ITableMappingFieldNameRepository
    {
        Task<IEnumerable<TableMappingFieldNameDto>> GetTableMappingFieldName(string? search, string? SearchADVFN, string? SearchADVUID, string? SearchADVLANG, bool delflag, string? plant);
        Task<IEnumerable<TableMappingFieldNameDto>> CheckExistingData(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto);
        Task<IEnumerable<TableMappingFieldNameDto>> InsertNewData(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto);
        Task<IEnumerable<TableMappingFieldNameDto>> UpdateData(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto);
        Task<IEnumerable<TableMappingFieldNameDto>> DataDelete(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto);
        Task<IEnumerable<TableMappingFieldNameDto>> DataPermDelete(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto);
        Task<IEnumerable<TableMappingFieldNameDto>> DataRecover(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto);
        Task<byte[]> Template();
        Task<ImportResult> Import(string filePath, string userId);
    }
}
