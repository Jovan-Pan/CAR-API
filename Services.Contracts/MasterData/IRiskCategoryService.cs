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
    public interface IRiskCategoryService
    {
        Task<ApiResponse<IEnumerable<RiskCategoryDto>>> GetRiskCategory(string? search, string? SearchADV, bool delflag);
        Task<ApiResponse<IEnumerable<RiskCategoryDto>>> InsertNewData(CRUDRiskCategoryDto CRUDRiskCategoryDto);
        Task<ApiResponse<IEnumerable<RiskCategoryDto>>> UpdateData(CRUDRiskCategoryDto CRUDRiskCategoryDto);
        Task<ApiResponse<IEnumerable<RiskCategoryDto>>> DataDelete(CRUDRiskCategoryDto CRUDRiskCategoryDto);
        Task<ApiResponse<IEnumerable<RiskCategoryDto>>> DataPermDelete(CRUDRiskCategoryDto CRUDRiskCategoryDto);
        Task<ApiResponse<IEnumerable<RiskCategoryDto>>> DataRecover(CRUDRiskCategoryDto CRUDRiskCategoryDto);
        Task<byte[]> Template();
        Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId);
    }
}

