using Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.MasterData
{
    public interface IRiskCategoryRepository
    {
        Task<IEnumerable<RiskCategoryDto>> GetRiskCategory(string search, string SearchADV, bool delflag);
        Task<IEnumerable<RiskCategoryDto>> InsertNewData(CRUDRiskCategoryDto CRUDRiskCategoryDto);
        Task<IEnumerable<RiskCategoryDto>> UpdateData(CRUDRiskCategoryDto CRUDRiskCategoryDto);
        Task<IEnumerable<RiskCategoryDto>> DataDelete(CRUDRiskCategoryDto CRUDRiskCategoryDto);
        Task<IEnumerable<RiskCategoryDto>> DataPermDelete(CRUDRiskCategoryDto CRUDRiskCategoryDto);
        Task<IEnumerable<RiskCategoryDto>> DataRecover(CRUDRiskCategoryDto CRUDRiskCategoryDto);
        Task<byte[]> Template();
        Task<ImportResult> Import(string filePath, string userId);
    }
}
