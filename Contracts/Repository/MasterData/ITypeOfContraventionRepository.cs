using Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.MasterData
{
    public interface ITypeOfContraventionRepository
    {
        Task<IEnumerable<TypeofcontraventionDto>> GetTypeofcontravention(GETTypeofcontravention GETTypeofcontravention);
        Task<IEnumerable<TypeofcontraventionDto>> InsertNewData(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto);
        Task<IEnumerable<TypeofcontraventionDto>> UpdateData(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto);
        Task<IEnumerable<TypeofcontraventionDto>> DataDelete(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto);
        Task<IEnumerable<TypeofcontraventionDto>> DataPermDelete(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto);
        Task<IEnumerable<TypeofcontraventionDto>> DataRecover(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto);
        Task<byte[]> Template();
        Task<ImportResult> Import(string filePath, string userId);
    }
}
