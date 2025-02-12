using Entities.MasterData;
using Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.MasterData
{
    public interface ITypeofcontraventionService
    {
        Task<ApiResponse<IEnumerable<TypeofcontraventionDto>>> GetTypeofcontravention(string? search, string? SearchADV, bool delflag);
        Task<ApiResponse<IEnumerable<TypeofcontraventionDto>>> InsertNewData(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto);
        Task<ApiResponse<IEnumerable<TypeofcontraventionDto>>> UpdateData(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto);
        Task<ApiResponse<IEnumerable<TypeofcontraventionDto>>> DataDelete(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto);
        Task<ApiResponse<IEnumerable<TypeofcontraventionDto>>> DataPermDelete(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto);
        Task<ApiResponse<IEnumerable<TypeofcontraventionDto>>> DataRecover(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto);
        Task<byte[]> Template();
        Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId);
    }
}
