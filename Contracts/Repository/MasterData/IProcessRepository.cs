using Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.MasterData
{
    public interface IProcessRepository
    {
        Task<IEnumerable<ProcessDto>> GetProcess(GETProcess GETProcess);
        Task<IEnumerable<ProcessDto>> InsertNewData(CRUDProcessDto CRUDProcessDto);
        Task<IEnumerable<ProcessDto>> UpdateData(CRUDProcessDto CRUDProcessDto);
        Task<IEnumerable<ProcessDto>> DataDelete(CRUDProcessDto CRUDProcessDto);
        Task<IEnumerable<ProcessDto>> DataPermDelete(CRUDProcessDto CRUDProcessDto);
        Task<IEnumerable<ProcessDto>> DataRecover(CRUDProcessDto CRUDProcessDto);
        Task<byte[]> Template();
        Task<ImportResult> Import(string filePath, string userId);
    }
}
