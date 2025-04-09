using Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.MasterData
{
    public interface ICARCategoryRepository
    {
        Task<IEnumerable<CARCategoryDto>> GetCARCategory(GETCARCategory GETCARCategory);
        Task<IEnumerable<CARCategoryDto>> InsertCARCategory(CRUDCARCategoryDto CRUDCARCategoryDto);
        Task<IEnumerable<CARCategoryDto>> UpdateCARCategory(CRUDCARCategoryDto CRUDCARCategoryDto);
        Task<IEnumerable<CARCategoryDto>> DataDelete(CRUDCARCategoryDto CRUDCARCategoryDto);
        Task<IEnumerable<CARCategoryDto>> DataPermDelete(CRUDCARCategoryDto CRUDCARCategoryDto);
        Task<IEnumerable<CARCategoryDto>> DataRecover(CRUDCARCategoryDto CRUDCARCategoryDto);
        Task<byte[]> Template();
        Task<ImportResult> Import(string filePath, string userId);
    }
}
