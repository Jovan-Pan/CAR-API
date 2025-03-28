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
    public interface ICARCategoryService
    {
        Task<ApiResponse<IEnumerable<CARCategoryDto>>> GetCARCategory(GETCARCategory GETCARCategory);
        Task<ApiResponse<IEnumerable<CARCategoryDto>>> InsertCARCategory(CRUDCARCategoryDto CRUDCARCategoryDto);
        Task<ApiResponse<IEnumerable<CARCategoryDto>>> UpdateCARCategory(CRUDCARCategoryDto CRUDCARCategoryDto);
        Task<ApiResponse<IEnumerable<CARCategoryDto>>> DataDelete(CRUDCARCategoryDto CRUDCARCategoryDto);
        Task<ApiResponse<IEnumerable<CARCategoryDto>>> DataPermDelete(CRUDCARCategoryDto CRUDCARCategoryDto);
        Task<ApiResponse<IEnumerable<CARCategoryDto>>> DataRecover(CRUDCARCategoryDto CRUDCARCategoryDto);
        Task<byte[]> Template();
        Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId);
    }
}
